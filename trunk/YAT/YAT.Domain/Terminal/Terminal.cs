//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of thread state:
////#define DEBUG_THREAD_STATE

	// Enable debugging of thread state:
////#define DEBUG_CONTENT_EVENTS

// Enable debug output line handling for scripting:
////#define DEBUG_SCRIPTING

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
#if (WITH_SCRIPTING)
using System.Collections;
#endif
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using MKY;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Text;

using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Terminal with byte/string functionality and settings.
	/// </summary>
	/// <remarks>
	/// Terminal and its specializations <see cref="TextTerminal"/> and <see cref="BinaryTerminal"/>
	/// implement the method pattern. Terminal provides general processing and formatting functions,
	/// its specializations add additional functionality.
	/// </remarks>
	/// <remarks>
	/// This class is implemented using partial classes separating sending/processing functionality.
	/// Using partial classes rather than aggregated sender, processor,... so far for these reasons:
	/// <list type="bullet">
	/// <item><description>Simpler for implementing text/binary specialization</description></item>
	/// <item><description>Simpler for implementing synchronization among Tx and Rx.</description></item>
	/// <item><description>Less "Durchlauferhitzer", e.g. directly raising events.</description></item>
	/// </list>
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public abstract partial class Terminal : IDisposable, IDisposableEx
	{
		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
		//==========================================================================================

		/// <summary></summary>
		public static readonly string SerialPortHelp =
			@"For serial COM ports, if one of the following error conditions occurs, the according error indication will be shown in the terminal window:" + Environment.NewLine +
			Environment.NewLine +
			@"[" + RxFramingErrorString + "]" + Environment.NewLine +
			@"An input framing error occurs when the last bit received is not a stop bit. This may occur due to a timing error. You will most commonly encounter a framing error when the speed at which the data is being sent is different to that of what you have YAT set to receive it at." + Environment.NewLine +
			Environment.NewLine +
			@"[" + RxBufferOverrunErrorString + "]" + Environment.NewLine +
			@"An input overrun error occurs when the input gets out of synch. The next character will be lost and the input will be re-synch'd." + Environment.NewLine +
			Environment.NewLine +
			@"[" + RxBufferOverflowErrorString + "]" + Environment.NewLine +
			@"An input overflow occurs when there is no more space in the input buffer, i.e. the serial driver, the operating system or YAT doesn't manage to process the incoming data fast enough." + Environment.NewLine +
			Environment.NewLine +
			@"[" + RxParityErrorString + "]" + Environment.NewLine +
			@"An input parity error occurs when a parity check is enabled but the parity bit mismatches. You will most commonly encounter a parity error when the parity setting at which the data is being sent is different to that of what you have YAT set to receive it at." + Environment.NewLine +
			Environment.NewLine +
			@"[" + TxBufferFullErrorString + "]" + Environment.NewLine +
			@"An output buffer full error occurs when there is no more space in the output buffer, i.e. the serial driver, the operating system or YAT doesn't manage to send the data fast enough.";

		#endregion

		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary>
		/// While sending, the 'IOChanged' event must be raised if intensive processing is done.
		/// This is required because a client may want to indicate that time intensive sending is
		/// currently ongoing and no further data shall be sent.
		/// The event shall be raised if the time lag will significantly be noticeable by the user
		/// (i.e. >= 400 ms). But the event shall be raised BEFORE the actual time lag. This helper
		/// struct manages the state and the various criteria.
		/// </summary>
		/// <remarks>
		/// Struct to improve performance as a struct only needs to be created once.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		private struct IOChangedEventHelper
		{
			/// <summary></summary>
			public const int ThresholdMs = 400; // 400 Bytes @ 9600 Baud ~= 400 ms

			private bool eventMustBeRaised;
			private DateTime initialTimeStamp;

			/// <summary></summary>
			public bool EventMustBeRaised
			{
				get { return (this.eventMustBeRaised); }
			}

			/// <summary></summary>
			public void Initialize()
			{
				this.eventMustBeRaised = false;
				this.initialTimeStamp = DateTime.Now;
			}

			/// <summary></summary>
			public static bool ChunkSizeIsAboveThreshold(int chunkSize)
			{
				return (chunkSize >= ThresholdMs);
			}

			/// <summary></summary>
			public bool RaiseEventIfChunkSizeIsAboveThreshold(int chunkSize)
			{
				// Only let the event get raised if it hasn't been yet:
				if (!this.eventMustBeRaised && ChunkSizeIsAboveThreshold(chunkSize))
				{
					this.eventMustBeRaised = true;
					return (true);
				}

				return (false);
			}

			/// <summary></summary>
			public static bool DelayIsAboveThreshold(int delay)
			{
				return (delay >= ThresholdMs);
			}

			/// <summary></summary>
			public bool RaiseEventIfDelayIsAboveThreshold(int delay)
			{
				// Only let the event get raised if it hasn't been yet:
				if (!this.eventMustBeRaised && DelayIsAboveThreshold(delay))
				{
					this.eventMustBeRaised = true;
					return (true);
				}

				return (false);
			}

			/// <summary></summary>
			public bool TotalTimeLagIsAboveThreshold()
			{
				TimeSpan totalTimeLag = (DateTime.Now - this.initialTimeStamp);
				return (totalTimeLag.Milliseconds >= ThresholdMs);
			}

			/// <summary></summary>
			public bool RaiseEventIfTotalTimeLagIsAboveThreshold()
			{
				// Only let the event get raised if it hasn't been yet:
				if (!this.eventMustBeRaised && TotalTimeLagIsAboveThreshold())
				{
					this.eventMustBeRaised = true;
					return (true);
				}

				return (false);
			}

			/// <summary></summary>
			public void EventMustBeRaisedBecauseStatusHasBeenAccessed()
			{
				this.eventMustBeRaised = true;
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string RxFramingErrorString        = "RX FRAMING ERROR";
		private const string RxBufferOverrunErrorString  = "RX BUFFER OVERRUN";
		private const string RxBufferOverflowErrorString = "RX BUFFER OVERFLOW";
		private const string RxParityErrorString         = "RX PARITY ERROR";
		private const string TxBufferFullErrorString     = "TX BUFFER FULL";

		private const int ClearAndRefreshTimeout = 400;

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticInstanceCounter;
		private static Random staticRandom = new Random(RandomEx.NextPseudoRandomSeed());

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		/// <remarks> \remind (2019-08-22 / MKY)
		///
		/// Explicitly setting <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/>
		/// to handle/workaround the issue described in <see cref="RawTerminal"/>.
		///
		/// Temporarily disabling this handling/workaround can be useful for debugging, i.e. to
		/// continue program execution even in case of exceptions and let the debugger handle it.
		/// </remarks>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Terminal).FullName, exceptionHandling: EventHelper.ExceptionHandlingMode.DiscardDisposedTarget);
	////private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Terminal).FullName); // See remarks above!

		private int instanceId;

		private Settings.TerminalSettings terminalSettings;

		private RawTerminal rawTerminal;
		private DateTime initialTimeStamp;

		private IOChangedEventHelper ioChangedEventHelper;

		private IOControlState ioControlStateCache;
		private object ioControlStateCacheSyncObj = new object();

		private DisplayRepository txRepository;
		private DisplayRepository bidirRepository;
		private DisplayRepository rxRepository;
		private object repositorySyncObj = new object();

	#if (WITH_SCRIPTING)

		private Queue<string> availableReceivedMessagesForScripting = new Queue<string>();
		private string lastEnqueuedReceivedMessageForScripting; // = null;
		private object lastEnqueuedReceivedMessageForScriptingSyncObj = new object();
		private string lastDequeuedReceivedMessageForScripting; // = null;
		private object lastDequeuedReceivedMessageForScriptingSyncObj = new object();

	#endif

		private object clearAndRefreshSyncObj = new object();
		private bool isReloading;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;

		/// <summary></summary>
		public event EventHandler<IOControlEventArgs> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

	#if (WITH_SCRIPTING)

		// Note that e.g. a 'SendingText' or 'SendingMessage' event doesn't make sense, as it would
		// contain parsable text that may even include a keyword to be processed.

		/// <summary>
		/// Occurs when a packet is being sent in the host application. The event args contain the
		/// binary raw data that is being sent, including control characters, EOL,...
		/// </summary>
		/// <remarks>
		/// Named 'Sending...' rather than '...Sent' since sending is just about to happen and
		/// can be modified using the <see cref="ModifiablePacketEventArgs.Data"/> property or
		/// even canceled using the <see cref="ModifiablePacketEventArgs.Cancel"/> property.
		/// This is similar to the behavior of e.g. the 'OnValidating' event of WinForms controls.
		/// </remarks>
		public event EventHandler<ModifiablePacketEventArgs> SendingPacket;

	#endif

		/// <summary></summary>
		public event EventHandler<RawChunkEventArgs> RawChunkSent;

		/// <summary></summary>
		public event EventHandler<RawChunkEventArgs> RawChunkReceived;

		/// <summary></summary>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsSent;

		/// <summary></summary>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsReceived;

		/// <remarks>
		/// Using "current line replaced" rather than "element(s) removed" semantic because removing
		/// elements would likely be more error prone since...
		/// ...exact sequence of adding and removing elements has to exactly match.
		/// ...an already added element would likely have to be unfolded to remove parts of it!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public event EventHandler<DisplayElementsEventArgs> CurrentDisplayLineSentReplaced;

		/// <remarks>
		/// Using "current line replaced" rather than "element(s) removed" semantic because removing
		/// elements would likely be more error prone since...
		/// ...exact sequence of adding and removing elements has to exactly match.
		/// ...an already added element would likely have to be unfolded to remove parts of it!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public event EventHandler<DisplayElementsEventArgs> CurrentDisplayLineReceivedReplaced;

		/// <remarks><see cref="CurrentDisplayLineSentReplaced"/> above.</remarks>
		public event EventHandler CurrentDisplayLineSentCleared;

		/// <remarks><see cref="CurrentDisplayLineReceivedReplaced"/> above.</remarks>
		public event EventHandler CurrentDisplayLineReceivedCleared;

		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesSent;

		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesReceived;

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Occurs when a packet has been received by the host application. The event args contain
		/// the binary raw data that has been received, including control characters, EOL,...
		/// In contrast, the <see cref="ScriptMessageReceived"/> event args contain the message in
		/// formatted text representation.
		/// </summary>
		public event EventHandler<PacketEventArgs> ScriptPacketReceived;

		/// <summary>
		/// Occurs when a message has been received by the host application. The event args contain
		/// the message in formatted text representation. For text terminals, the text is composed
		/// of the decoded characters, excluding control characters. For binary terminals, the text
		/// represents the received data in hexadecimal notation.
		/// In contrast, the <see cref="ScriptPacketReceived"/> event args contain the binary raw
		/// data that has been received.
		/// </summary>
		public event EventHandler<MessageEventArgs> ScriptMessageReceived;

	#endif

		/// <summary></summary>
		public event EventHandler<EventArgs<RepositoryType>> RepositoryCleared;

		/// <summary></summary>
		public event EventHandler<EventArgs<RepositoryType>> RepositoryReloaded;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		protected Terminal(Settings.TerminalSettings settings)
		{
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

			this.txRepository    = new DisplayRepository(settings.Display.MaxLineCount);
			this.bidirRepository = new DisplayRepository(settings.Display.MaxLineCount);
			this.rxRepository    = new DisplayRepository(settings.Display.MaxLineCount);

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(this.terminalSettings.IO, this.terminalSettings.Buffer));

		////this.isReloading has been initialized to false.

			CreateAndStartSendThreads();
		}

		/// <summary></summary>
		protected Terminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

			this.txRepository    = new DisplayRepository(terminal.txRepository);
			this.bidirRepository = new DisplayRepository(terminal.bidirRepository);
			this.rxRepository    = new DisplayRepository(terminal.rxRepository);

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(terminal.rawTerminal, this.terminalSettings.IO, this.terminalSettings.Buffer));

			this.isReloading = terminal.isReloading;

			CreateAndStartSendThreads();
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the timer will already have been stopped in Stop()...
					DisposePeriodicXOnTimer();

					// ...and the send thread will already have been stopped in Close()...
					StopSendThreads();

					// ...and objects will already have been detached and disposed of in Close():
					DetachTerminalSettings();
					DetachAndDisposeRawTerminal();
					DisposeRepositories();
				}

				// Set state to disposed:
				IsDisposed = true;

				DebugMessage("...successfully disposed.");
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		///
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		///
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Terminal()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		protected virtual Settings.TerminalSettings TerminalSettings
		{
			get { return (this.terminalSettings); }
		}

		/// <summary></summary>
		public virtual DateTime InitialTimeStamp
		{
			get
			{
				AssertNotDisposed();

				return (this.initialTimeStamp);
			}

			set
			{
				AssertNotDisposed();

				this.initialTimeStamp = value;
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.rawTerminal != null)
					return (this.rawTerminal.IsStopped);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.rawTerminal != null)
					return (this.rawTerminal.IsStarted);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.rawTerminal != null)
					return (this.rawTerminal.IsConnected);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.rawTerminal != null)
					return (this.rawTerminal.IsOpen);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.rawTerminal != null)
					return (this.rawTerminal.IsTransmissive);
				else
					return (false);
			}
		}

		/// <remarks>Required to prevent superfluous 'IOChanged' events.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the scope.")]
		protected virtual bool IsReadyToSend_Internal
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (IsTransmissive && !this.sendingIsOngoing);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				this.ioChangedEventHelper.EventMustBeRaisedBecauseStatusHasBeenAccessed();

				return (IsReadyToSend_Internal);
			}
		}

		/// <remarks>Required to prevent superfluous 'IOChanged' events.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the scope.")]
		protected virtual bool IsBusy_Internal
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (IsTransmissive && this.sendingIsOngoing);
			}
		}

		/// <summary></summary>
		public virtual bool IsBusy
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				this.ioChangedEventHelper.EventMustBeRaisedBecauseStatusHasBeenAccessed();

				return (IsBusy_Internal);
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Gets a value indicating whether the terminal has received a message that is available for scripting.
		/// </summary>
		public virtual bool HasAvailableReceivedMessageForScripting
		{
			get
			{
				// AssertNotDisposed() is called by 'ReceivedLineCount' below.

				return (AvailableReceivedMessageCountForScripting > 0);
			}
		}

		/// <summary>
		/// Gets a value indicating the number of received messages that are available for scripting.
		/// </summary>
		public virtual int AvailableReceivedMessageCountForScripting
		{
			get
			{
				AssertNotDisposed();

				lock (this.availableReceivedMessagesForScripting)
				{
					var count = this.availableReceivedMessagesForScripting.Count;

					DebugScriptingQueueCount(count);

					return (count);
				}
			}
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		public virtual MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.rawTerminal != null)
					return (this.rawTerminal.UnderlyingIOProvider);
				else
					return (null);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.rawTerminal != null)
					return (this.rawTerminal.UnderlyingIOInstance);
				else
					return (null);
			}
		}

		/// <summary></summary>
		protected virtual bool IsReloading
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.isReloading);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > Start/Stop/Close
		//------------------------------------------------------------------------------------------
		// Methods > Start/Stop/Close
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Start the terminal. To stop it again, call <see cref="Stop()"/>.
		/// </summary>
		public virtual bool Start()
		{
			AssertNotDisposed();

			if (IsStopped)
			{
				// Do not clear the send queue, it already got cleared when stopping. This setup
				// potentially allows to call Send() and buffer data before starting the terminal.

				var success = this.rawTerminal.Start();
				if (success)
				{
					this.initialTimeStamp = DateTime.Now;

					ConfigurePeriodicXOnTimer();
				}
				return (success);
			}
			else
			{
				return (true); // Return 'true' as terminal has already been started.
			}
		}

		/// <summary>
		/// Stop the terminal. To start it again, call <see cref="Start()"/>.
		/// Or call <see cref="Close()"/> to definitely close the terminal.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			if (IsStarted)
			{
				// Note that send processing may continue for some instants, until the send thread
				// has noticed that it should stop/pause. The system (e.g. event handling) must be
				// able to deal with this design!
				// An alternative approach would be to lock/synchronize here, i.e. wait until the
				// send thread has indeed stopped. However, this results in dead-locks if Stop()
				// is called from the same thread where the ...Sent events get invoked (e.g. the UI
				// thread).

				DisablePeriodicXOnTimer();

				this.rawTerminal.Stop();

				lock (this.sendDataQueue) // Lock is required because Queue<T> is not synchronized.
					this.sendDataQueue.Clear();

				lock (this.sendFileQueue) // Lock is required because Queue<T> is not synchronized.
					this.sendFileQueue.Clear();
			}
		}

		/// <summary>
		/// Definitely close the terminal. After closing, the terminal cannot be started anymore
		/// and must be terminated.
		/// </summary>
		/// <remarks>
		/// This method is required to stop the send thread prior to calling <see cref="Dispose()"/>.
		/// </remarks>
		public virtual void Close()
		{
			AssertNotDisposed();

			StopSendThreads();
			this.rawTerminal.Close();
			DetachAndDisposeRawTerminal();
			DisposeRepositories();
		}

		#endregion

		#region Methods > Parse
		//------------------------------------------------------------------------------------------
		// Methods > Parse
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Tries to parse <paramref name="s"/>, taking the current settings into account.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParseText(string s, out byte[] result, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			using (var p = new Parser.Parser(TerminalSettings.IO.Endianness, TerminalSettings.Send.Text.ToParseMode()))
				return (p.TryParse(s, out result, defaultRadix));
		}

		#endregion

		#region Methods > I/O Control
		//------------------------------------------------------------------------------------------
		// Methods > I/O Control
		//------------------------------------------------------------------------------------------

		private bool IsUsbSerialHid
		{
			get { return ((this.terminalSettings != null) && (TerminalSettings.IO.IOType == IOType.UsbSerialHid)); }
		}

		private bool IsSerialPort
		{
			get { return ((this.terminalSettings != null) && (TerminalSettings.IO.IOType == IOType.SerialPort)); }
		}

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPins SerialPortControlPins
		{
			get
			{
				AssertNotDisposed();

				if (IsSerialPort)
				{
					var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.ControlPins);
				}

				return (new MKY.IO.Ports.SerialPortControlPins());
			}
		}

		/// <summary>
		/// Serial port control pin counts.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPinCount SerialPortControlPinCount
		{
			get
			{
				AssertNotDisposed();

				if (IsSerialPort)
				{
					var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.ControlPinCount);
				}

				return (new MKY.IO.Ports.SerialPortControlPinCount());
			}
		}

		/// <summary></summary>
		public virtual int SentXOnCount
		{
			get
			{
				AssertNotDisposed();

				var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
				if (x != null)
					return (x.SentXOnCount);

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int SentXOffCount
		{
			get
			{
				AssertNotDisposed();

				var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
				if (x != null)
					return (x.SentXOffCount);

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOnCount
		{
			get
			{
				AssertNotDisposed();

				var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
				if (x != null)
					return (x.ReceivedXOnCount);

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOffCount
		{
			get
			{
				AssertNotDisposed();

				var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
				if (x != null)
					return (x.ReceivedXOffCount);

				return (0);
			}
		}

		/// <summary></summary>
		public virtual void ResetFlowControlCount()
		{
			AssertNotDisposed();

			if (IsSerialPort)
			{
				var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
				if (port != null)
					port.ResetFlowControlCount();
			}
			else
			{
				var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
				if (x != null)
					x.ResetXOnXOffCount();
			}
		}

		/// <summary></summary>
		public virtual int InputBreakCount
		{
			get
			{
				AssertNotDisposed();

				if (IsSerialPort)
				{
					var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.InputBreakCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int OutputBreakCount
		{
			get
			{
				AssertNotDisposed();

				if (IsSerialPort)
				{
					var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.OutputBreakCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual void ResetBreakCount()
		{
			AssertNotDisposed();

			if (IsSerialPort)
			{
				var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
				if (port != null)
					port.ResetBreakCount();
			}
		}

		/// <summary>
		/// Toggles RTS control pin if current flow control settings allow this.
		/// </summary>
		/// <param name="pinState">
		/// <c>true</c> if the control pin has become enabled.; otherwise, <c>false</c>
		/// </param>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		public virtual bool RequestToggleRts(out MKY.IO.Serial.SerialPort.SerialControlPinState pinState)
		{
			AssertNotDisposed();

			if (IsSerialPort)
			{
				if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesRtsCtsAutomatically)
				{
					var p = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
					if (p != null)
					{
						if (p.ToggleRts())
							pinState = MKY.IO.Serial.SerialPort.SerialControlPinState.Enabled;
						else
							pinState = MKY.IO.Serial.SerialPort.SerialControlPinState.Disabled;

						return (true);
					}
				}
			}

			pinState = MKY.IO.Serial.SerialPort.SerialControlPinState.Automatic;
			return (false);
		}

		/// <summary>
		/// Toggles DTR control pin if current flow control settings allow this.
		/// </summary>
		/// <param name="pinState">
		/// <c>true</c> if the control pin has become enabled.; otherwise, <c>false</c>
		/// </param>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		public virtual bool RequestToggleDtr(out MKY.IO.Serial.SerialPort.SerialControlPinState pinState)
		{
			AssertNotDisposed();

			if (IsSerialPort)
			{
				if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically)
				{
					var p = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
					if (p != null)
					{
						if (p.ToggleDtr())
							pinState = MKY.IO.Serial.SerialPort.SerialControlPinState.Enabled;
						else
							pinState = MKY.IO.Serial.SerialPort.SerialControlPinState.Disabled;

						return (true);
					}
				}
			}

			pinState = MKY.IO.Serial.SerialPort.SerialControlPinState.Automatic;
			return (false);
		}

		/// <summary>
		/// Toggles the input XOn/XOff state if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool RequestToggleInputXOnXOff()
		{
			AssertNotDisposed();

			if (TerminalSettings.IO.FlowControlManagesXOnXOffManually)
			{
				var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
				if (x != null)
				{
					x.ToggleInputXOnXOff();

					return (true);
				}
			}

			return (false);
		}

		/// <summary>
		/// Signals XOn if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool RequestSignalInputXOn()
		{
			if (TerminalSettings.IO.FlowControlUsesXOnXOff)
			{
				var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
				if (x != null)
				{
					x.SignalInputXOn();

					return (true);
				}
			}

			return (false);
		}

		/// <summary>
		/// Toggles the output break state if current port settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool RequestToggleOutputBreak()
		{
			AssertNotDisposed();

			if (IsSerialPort)
			{
				if (TerminalSettings.IO.SerialPortOutputBreakIsModifiable)
				{
					var p = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
					if (p != null)
					{
						p.ToggleOutputBreak();
						return (true);
					}
				}
			}

			return (false);
		}

		private void ConfigurePeriodicXOnTimer()
		{
			if (TerminalSettings.Send.SignalXOnPeriodically.Enabled)
				EnablePeriodicXOnTimer(TerminalSettings.Send.SignalXOnPeriodically.Interval);
			else
				DisablePeriodicXOnTimer();
		}

		private void EnablePeriodicXOnTimer(int interval)
		{
			if (this.periodicXOnTimer == null)
				CreatePeriodicXOnTimer();

			this.periodicXOnTimer.Interval = interval;

			if (!this.periodicXOnTimer.Enabled)
				this.periodicXOnTimer.Start();
		}

		private void DisablePeriodicXOnTimer()
		{
			if (this.periodicXOnTimer != null)
			{
				if (this.periodicXOnTimer.Enabled)
					this.periodicXOnTimer.Stop();
			}
		}

		private void CreatePeriodicXOnTimer()
		{
			if (this.periodicXOnTimer == null)
			{
				this.periodicXOnTimer = new System.Timers.Timer();
				this.periodicXOnTimer.AutoReset = true;
				this.periodicXOnTimer.Elapsed += periodicXOnTimer_Elapsed;
			}
		}

		private void DisposePeriodicXOnTimer()
		{
			if (this.periodicXOnTimer != null)
			{
				this.periodicXOnTimer.Elapsed -= periodicXOnTimer_Elapsed;
				this.periodicXOnTimer.Dispose();
				this.periodicXOnTimer = null;
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object periodicXOnTimer_Elapsed_SyncObj = new object();

		private void periodicXOnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(periodicXOnTimer_Elapsed_SyncObj))
			{
				try
				{
					// Ensure not to forward events during closing anymore:
					if (!IsDisposed && IsReadyToSend_Internal)
						RequestSignalInputXOn();
				}
				finally
				{
					Monitor.Exit(periodicXOnTimer_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
		}

		/// <summary>
		/// Returns a collection of <see cref="DisplayElement"/> objects reflecting the changed I/O control status.
		/// </summary>
		/// <remarks>
		/// Private to ensure proper update of <see cref="ioControlStateCache"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Fail-safe implementation, especially catching exceptions while closing.")]
		private ReadOnlyCollection<string> IOControlChangeTexts()
		{
			var currentState = new IOControlState();

			// Pin and IBS/OBS state and count:
			if (IsSerialPort)
			{
				currentState.SerialPortControlPinCount = SerialPortControlPinCount;

				if (IsOpen)
				{
					var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
					if (port != null)
					{
						try // Fail-safe implementation, especially catching exceptions while closing.
						{
							currentState.SerialPortControlPins = port.ControlPins;
							currentState.InputBreak            = port.InputBreak;
							currentState.OutputBreak           = port.OutputBreak;
							currentState.InputBreakCount       = port.InputBreakCount;
							currentState.OutputBreakCount      = port.OutputBreakCount;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Failed to retrieve control pin state");
						}
					}
					else
					{
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying I/O instance is no serial COM port!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}

			// IXS/OXS state and count:
			if ((IsSerialPort || IsUsbSerialHid) && TerminalSettings.IO.FlowControlManagesXOnXOffManually)
			{
				if (IsOpen)
				{
					var x = (UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
					if (x != null)
					{
						try // Fail-safe implementation, especially catching exceptions while closing.
						{
							currentState.InputIsXOn        = x.InputIsXOn;
							currentState.OutputIsXOn       = x.OutputIsXOn;
							currentState.SentXOnCount      = x.SentXOnCount;
							currentState.SentXOffCount     = x.SentXOffCount;
							currentState.ReceivedXOnCount  = x.ReceivedXOnCount;
							currentState.ReceivedXOffCount = x.ReceivedXOffCount;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Failed to retrieve XOn/XOff state");
						}
					}
				}
			}

			var previousState = new IOControlState();
			lock (this.ioControlStateCacheSyncObj)
			{
				previousState = this.ioControlStateCache;
				this.ioControlStateCache = currentState;
			}

			var l = new List<string>(3); // Preset the required capacity to improve memory management.

			if (IsSerialPort)
			{
				var pinText = new StringBuilder();

				if (currentState.SerialPortControlPins.Rts != previousState.SerialPortControlPins.Rts)
				{
					pinText.Append("RTS=");
					pinText.Append(currentState.SerialPortControlPins.Rts ? "on" : "off");

					if (TerminalSettings.Status.ShowFlowControlCount)
						pinText.Append("|" + currentState.SerialPortControlPinCount.RtsDisableCount.ToString(CultureInfo.CurrentCulture));
				}

				if (currentState.SerialPortControlPins.Cts != previousState.SerialPortControlPins.Cts)
				{
					if (pinText.Length > 0)
						pinText.Append(",");

					pinText.Append("CTS=");
					pinText.Append(currentState.SerialPortControlPins.Cts ? "on" : "off");

					if (TerminalSettings.Status.ShowFlowControlCount)
						pinText.Append("|" + currentState.SerialPortControlPinCount.CtsDisableCount.ToString(CultureInfo.CurrentCulture));
				}

				if (currentState.SerialPortControlPins.Dtr != previousState.SerialPortControlPins.Dtr)
				{
					if (pinText.Length > 0)
						pinText.Append(",");

					pinText.Append("DTR=");
					pinText.Append(currentState.SerialPortControlPins.Dtr ? "on" : "off");

					if (TerminalSettings.Status.ShowFlowControlCount)
						pinText.Append("|" + currentState.SerialPortControlPinCount.DtrDisableCount.ToString(CultureInfo.CurrentCulture));
				}

				if (currentState.SerialPortControlPins.Dsr != previousState.SerialPortControlPins.Dsr)
				{
					if (pinText.Length > 0)
						pinText.Append(",");

					pinText.Append("DSR=");
					pinText.Append(currentState.SerialPortControlPins.Dsr ? "on" : "off");

					if (TerminalSettings.Status.ShowFlowControlCount)
						pinText.Append("|" + currentState.SerialPortControlPinCount.DsrDisableCount.ToString(CultureInfo.CurrentCulture));
				}

				if (currentState.SerialPortControlPins.Dsr != previousState.SerialPortControlPins.Dsr)
				{
					if (pinText.Length > 0)
						pinText.Append(",");

					pinText.Append("DCD=");
					pinText.Append(currentState.SerialPortControlPins.Dsr ? "on" : "off");

					if (TerminalSettings.Status.ShowFlowControlCount)
						pinText.Append("|" + currentState.SerialPortControlPinCount.DcdCount.ToString(CultureInfo.CurrentCulture));
				}

				if (pinText.Length > 0)
					l.Add(pinText.ToString());
			}

			if ((IsSerialPort || IsUsbSerialHid) && TerminalSettings.IO.FlowControlManagesXOnXOffManually)
			{
				var flowControlText = new StringBuilder();

				if (currentState.InputIsXOn != previousState.InputIsXOn)
				{
					flowControlText.Append("IXS=");
					flowControlText.Append(currentState.InputIsXOn ? "on" : "off");

					if (TerminalSettings.Status.ShowFlowControlCount)
						flowControlText.Append("|" + currentState.SentXOnCount.ToString(CultureInfo.CurrentCulture) + "|" + currentState.SentXOffCount.ToString(CultureInfo.CurrentCulture));
				}

				if (currentState.OutputIsXOn != previousState.OutputIsXOn)
				{
					if (flowControlText.Length > 0)
						flowControlText.Append(",");

					flowControlText.Append("OXS=");
					flowControlText.Append(currentState.OutputIsXOn ? "on" : "off");

					if (TerminalSettings.Status.ShowFlowControlCount)
						flowControlText.Append("|" + currentState.ReceivedXOnCount.ToString(CultureInfo.CurrentCulture) + "|" + currentState.ReceivedXOffCount.ToString(CultureInfo.CurrentCulture));
				}

				if (flowControlText.Length > 0)
					l.Add(flowControlText.ToString());
			}

			if (IsSerialPort && TerminalSettings.IO.IndicateSerialPortBreakStates)
			{
				var breakText = new StringBuilder();

				if (currentState.InputBreak != previousState.InputBreak)
				{
					breakText.Append("IBS=");
					breakText.Append(currentState.InputBreak ? "on" : "off");

					if (TerminalSettings.Status.ShowBreakCount)
						breakText.Append("|" + currentState.InputBreakCount.ToString(CultureInfo.CurrentCulture));
				}

				if (currentState.OutputBreak != previousState.OutputBreak)
				{
					if (breakText.Length > 0)
						breakText.Append(",");

					breakText.Append("OBS=");
					breakText.Append(currentState.OutputBreak ? "on" : "off");

					if (TerminalSettings.Status.ShowBreakCount)
						breakText.Append("|" + currentState.OutputBreakCount.ToString(CultureInfo.CurrentCulture));
				}

				if (breakText.Length > 0)
					l.Add(breakText.ToString());
			}

			return (l.AsReadOnly());
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement ByteToElement(byte b, IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx: return (ByteToElement(b, d, TerminalSettings.Display.TxRadix));
				case IODirection.Rx: return (ByteToElement(b, d, TerminalSettings.Display.RxRadix));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement ByteToElement(byte b, IODirection d, Radix r)
		{
			bool isControl;
			bool isByteToHide;
			bool isError;

			string text = ByteToText(b, d, r, out isControl, out isByteToHide, out isError);

			if      (isError)
			{
				return (new DisplayElement.ErrorInfo((Direction)d, text));
			}
			else if (isByteToHide)
			{
				return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
			}
			else if (isControl)
			{
				if (TerminalSettings.CharReplace.ReplaceControlChars)
					return (CreateControlElement(b, d, text));
				else                         // !ReplaceControlChars => Use normal data element:
					return (CreateDataElement(b, d, text));
			}
			else // Neither 'isError' nor 'isByteToHide' nor 'isError' => Use normal data element:
			{
				return (CreateDataElement(b, d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToText(byte b, IODirection d, Radix r, out bool isControl, out bool isByteToHide, out bool isError)
		{
			isByteToHide = false;
			if      (b == 0x00)
			{
				if (TerminalSettings.CharHide.Hide0x00)
					isByteToHide = true;
			}
			else if (b == 0xFF)
			{
				if (TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
					isByteToHide = true;
			}
			else if (MKY.IO.Serial.XOnXOff.IsXOnOrXOffByte(b))
			{
				if (TerminalSettings.IO.FlowControlUsesXOnXOff && TerminalSettings.CharHide.HideXOnXOff)
					isByteToHide = true;
			}

			isControl = Ascii.IsControl(b);
			isError = false;

			switch (r)
			{
				case Radix.String:
				case Radix.Char:
				{
					if (isByteToHide)
					{
						return (null); // Return nothing, ignore the character, this results in hiding.
					}
					else if (isControl)
					{
						if (TerminalSettings.CharReplace.ReplaceControlChars)
							return (ByteToControlCharReplacementString(b, TerminalSettings.CharReplace.ControlCharRadix));
						else
							return (ByteToCharacterString(b));
					}
					else if (b == ' ') // Space.
					{
						if (TerminalSettings.CharReplace.ReplaceSpace)
							return (Settings.CharReplaceSettings.SpaceReplaceChar);
						else
							return (" ");
					}
					else
					{
						return (ByteToCharacterString(b));
					}
				}

				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				case Radix.Unicode:
				{
					if (isByteToHide)
					{
						return (null); // Return nothing, ignore the character, this results in hiding.
					}
					else if (isControl)
					{
						if (TerminalSettings.CharReplace.ReplaceControlChars)
							return (ByteToControlCharReplacementString(b, TerminalSettings.CharReplace.ControlCharRadix));
						else
							return (ByteToNumericRadixString(b, r)); // Current display radix.
					}
					else
					{
						return (ByteToNumericRadixString(b, r)); // Current display radix.
					}
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToCharacterString(byte b)
		{
			return (((char)b).ToString(CultureInfo.InvariantCulture));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToAsciiString(byte b)
		{
		////if      ((b == '\a') && !TerminalSettings.CharReplace.ReplaceBell) does not exist, CharAction.BeepOnBell exists instead and is handled elsewhere.
		////	return ("\a");
		////else if ((b == '\b') && !TerminalSettings.CharReplace.ReplaceBackspace)
			if      ((b == '\b') && !TerminalSettings.CharReplace.ReplaceBackspace)
				return ("\b");
			else if ((b == '\t') && !TerminalSettings.CharReplace.ReplaceTab)
				return ("\t");
			else
				return ("<" + Ascii.ConvertToMnemonic(b) + ">");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToNumericRadixString(byte b, Radix r)
		{
			switch (r)
			{
				case Radix.Bin:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToBinaryString(b) + "b");
					else
						return (ByteEx.ConvertToBinaryString(b));
				}

				case Radix.Oct:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToOctalString(b) + "o");
					else
						return (ByteEx.ConvertToOctalString(b));
				}

				case Radix.Dec:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (b.ToString("D3", CultureInfo.InvariantCulture) + "d");
					else
						return (b.ToString("D3", CultureInfo.InvariantCulture));
				}

				case Radix.Hex:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (b.ToString("X2", CultureInfo.InvariantCulture) + "h");
					else
						return (b.ToString("X2", CultureInfo.InvariantCulture));
				}

				case Radix.Unicode:
				{
					return (UnicodeValueToNumericString(b));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a numeric radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <remarks>
		/// Note the limitation FR #329:
		/// Unicode is limited to the basic multilingual plane (U+0000..U+FFFF).
		/// </remarks>
		[CLSCompliant(false)]
		protected virtual string UnicodeValueToNumericString(ushort value)
		{
			if (TerminalSettings.Display.ShowRadix)
				return ("U+" + value.ToString("X4", CultureInfo.InvariantCulture));
			else
				return (       value.ToString("X4", CultureInfo.InvariantCulture));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToControlCharReplacementString(byte b, ControlCharRadix r)
		{
			switch (r)
			{
				case ControlCharRadix.Char:
					return (ByteToCharacterString(b));

				case ControlCharRadix.Bin:
				case ControlCharRadix.Oct:
				case ControlCharRadix.Dec:
				case ControlCharRadix.Hex:
					return (ByteToNumericRadixString(b, (Radix)TerminalSettings.CharReplace.ControlCharRadix));

				case ControlCharRadix.AsciiMnemonic:
					return (ByteToAsciiString(b));

				default: // Includes 'String' and 'Unicode' which are not supported for control character replacement.
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is an ASCII control character radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte origin, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx: return (new DisplayElement.TxData(origin, text));
				case IODirection.Rx: return (new DisplayElement.RxData(origin, text));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx: return (new DisplayElement.TxData(origin, text));
				case IODirection.Rx: return (new DisplayElement.RxData(origin, text));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateControlElement(byte origin, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx: return (new DisplayElement.TxControl(origin, text));
				case IODirection.Rx: return (new DisplayElement.RxControl(origin, text));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual bool ElementsAreSeparate(IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx: return (ElementsAreSeparate(TerminalSettings.Display.TxRadix));
				case IODirection.Rx: return (ElementsAreSeparate(TerminalSettings.Display.RxRadix));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual bool ElementsAreSeparate(Radix r)
		{
			switch (r)
			{
				case Radix.String:  return (false);
				case Radix.Char:    return (true);

				case Radix.Bin:     return (true);
				case Radix.Oct:     return (true);
				case Radix.Dec:     return (true);
				case Radix.Hex:     return (true);

				case Radix.Unicode: return (true);

				default: throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void PrepareLineBeginInfo(DateTime ts, TimeSpan diff, TimeSpan delta, string dev, IODirection dir, out DisplayElementCollection lp)
		{
			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowDevice    ||
			    TerminalSettings.Display.ShowDirection)
			{
				lp = new DisplayElementCollection(10); // Preset the required capacity to improve memory management.

				if (TerminalSettings.Display.ShowTimeStamp)
				{
					lp.Add(new DisplayElement.TimeStampInfo(ts, TerminalSettings.Display.TimeStampFormat, TerminalSettings.Display.TimeStampUseUtc, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowTimeSpan)
				{
					lp.Add(new DisplayElement.TimeSpanInfo(diff, TerminalSettings.Display.TimeSpanFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowTimeDelta)
				{
					lp.Add(new DisplayElement.TimeDeltaInfo(delta, TerminalSettings.Display.TimeDeltaFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowDevice)
				{
					lp.Add(new DisplayElement.DeviceInfo(dev, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowDirection)
				{
					lp.Add(new DisplayElement.DirectionInfo((Direction)dir, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}
			}
			else
			{
				lp = null;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "Why not? 'Byte' not only is a type, but also emphasizes a purpose.")]
		protected virtual void PrepareLineEndInfo(int length, TimeSpan duration, out DisplayElementCollection lp)
		{
			if (TerminalSettings.Display.ShowLength || TerminalSettings.Display.ShowDuration) // Meaning: "byte count"/"char count" and "line duration".
			{
				lp = new DisplayElementCollection(4); // Preset the required capacity to improve memory management.

				if (TerminalSettings.Display.ShowLength)
				{
					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));

					lp.Add(new DisplayElement.DataLength(length, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may be both!
				}

				if (TerminalSettings.Display.ShowDuration)
				{
					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));

					lp.Add(new DisplayElement.TimeDurationInfo(duration, TerminalSettings.Display.TimeDurationFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may be both!
				}
			}
			else
			{
				lp = null;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Setting is required to be received, modified and returned.")]
		protected abstract void ProcessRawChunk(RawChunk chunk, LineChunkAttribute attribute, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine);

		/// <summary></summary>
		protected virtual void ProcessAndSignalRawChunk(RawChunk chunk, LineChunkAttribute attribute)
		{
			// Collection of elements resulting from this chunk, typically a partial line,
			// but may also be a complete line or even span across multiple lines.
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.

			// Collection of lines being completed by this chunk, typically none or a single line,
			// but may also be multiple lines.
			var linesToAdd = new DisplayLineCollection(); // No preset needed, the default initial capacity is good enough.

			bool clearAlreadyStartedLine = false;

			ProcessRawChunk(chunk, attribute, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0) {
				OnDisplayElementsAdded(chunk.Direction, elementsToAdd);

				if (linesToAdd.Count > 0) {
					OnDisplayLinesAdded(chunk.Direction, linesToAdd);
				}
			}

			if (clearAlreadyStartedLine) {
				OnCurrentDisplayLineCleared(chunk.Direction);
			}
		}

	#if (WITH_SCRIPTING)

		/// <remarks>
		/// Processing for scripting differs from "normal" processing for displaying because...
		/// ...received messages must not be impacted by 'DirectionLineBreak'.
		/// ...received data must not be processed individually, only as packets/messages.
		/// ...received data must not be reprocessed on <see cref="RefreshRepositories"/>.
		/// </remarks>
		protected virtual void ProcessAndSignalRawChunkForScripting(RawChunk chunk)
		{
			if (chunk.Direction == IODirection.Rx)
			{
				var data = new byte[chunk.Content.Count];
				chunk.Content.CopyTo(data, 0);

				var message = Format(data, IODirection.Rx);

				EnqueueReceivedMessageForScripting(message.ToString()); // Enqueue before invoking event to
				                                                        // have message ready for event.
				OnScriptPacketReceived(new PacketEventArgs(data));
				OnScriptMessageReceived(new MessageEventArgs(message.ToString()));
			}
		}

	#endif // WITH_SCRIPTING

		#endregion

		#region Methods > Special ;-)
		//------------------------------------------------------------------------------------------
		// Methods > Special ;-)
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Enqueues the easter egg message.
		/// </summary>
		public virtual void EnqueueEasterEggMessage()
		{
			OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "The bites have been eaten by the rabbit ;-]", true));
		}

		#endregion

		#region Methods > Format
		//------------------------------------------------------------------------------------------
		// Methods > Format
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Formats the specified time stamp.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		public virtual string Format(DateTime ts, Direction d)
		{
			var de = new DisplayElement.TimeStampInfo(d, ts, TerminalSettings.Display.TimeStampFormat, TerminalSettings.Display.TimeStampUseUtc, "", "");
			return (de.Text);
		}

		/// <summary>
		/// Formats the specified data sequence.
		/// </summary>
		/// <remarks>
		/// \remind (2017-12-11 / MKY)
		/// Currently limited to data of a single line. Refactoring would be required to format multiple lines
		/// (<see cref="ProcessRawChunk"/> instead of <see cref="ByteToElement(byte, IODirection, Radix)"/>).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		public virtual string Format(byte[] data, IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx: return (Format(data, d, TerminalSettings.Display.TxRadix));
				case IODirection.Rx: return (Format(data, d, TerminalSettings.Display.RxRadix));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Formats the specified data sequence.
		/// </summary>
		/// <remarks>
		/// \remind (2017-12-11 / MKY)
		/// Currently limited to data of a single line. Refactoring would be required to format multiple lines
		/// (<see cref="ProcessRawChunk"/> instead of <see cref="ByteToElement(byte, IODirection, Radix)"/>).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		public virtual string Format(byte[] data, IODirection d, Radix r)
		{
			var lp = new DisplayElementCollection();

			foreach (byte b in data)
			{
				var de = ByteToElement(b, d, r);
				lp.Add(de);
				AddSpaceIfNecessary(d, lp, de);
			}

			return (lp.ElementsToString());
		}

		/// <summary>
		/// Add a space to the given line part, depending on the give state.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual void AddSpaceIfNecessary(IODirection d, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(d) && !string.IsNullOrEmpty(de.Text))
			{
				if (lp.ByteCount > 0)
					lp.Add(new DisplayElement.DataSpace());
			}
		}

		#endregion

		#region Methods > Convert
		//------------------------------------------------------------------------------------------
		// Methods > Convert
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts the given value to a sendable text.
		/// </summary>
		/// <remarks>
		/// If the value is a printable character, the string contains that character.
		/// If the value is a control character, the ASCII mnemonic or Unicode representation is returned.
		/// </remarks>
		/// <remarks>
		/// This method is a copy of the code in MKY.CharEx.ConvertToPrintableString().
		/// Intentionally copied since the angle bracket and Unicode representations
		/// are explicitly required by YAT.
		/// </remarks>
		public static string ConvertToSendableText(char value)
		{
			if (!char.IsControl(value))
				return (value.ToString());

			// ASCII control characters:
			byte asciiCode;
			if ((CharEx.TryConvertToByte(value, out asciiCode)) && (Ascii.IsControl(asciiCode)))
				return ("<" + Ascii.ConvertToMnemonic(asciiCode) + ">");

			// Unicode control characters U+0080..U+009F:
			return (@"\U+" + ((ushort)(value)).ToString("X4", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Converts the given value to a sendable text.
		/// </summary>
		/// <remarks>
		/// Printable characters are kept, control characters are converted into the ASCII mnemonic
		/// or Unicode representation as required.
		/// </remarks>
		/// <remarks>
		/// This method is a copy of the code in MKY.CharEx.ConvertToPrintableString().
		/// Intentionally copied since the angle bracket and Unicode representations
		/// are explicitly required by YAT.
		/// </remarks>
		public static string ConvertToSendableText(string value)
		{
			var sb = new StringBuilder();

			foreach (char c in value)
				sb.Append(ConvertToSendableText(c));

			return (sb.ToString());
		}

		#endregion

		#region Methods > Scripting
		//------------------------------------------------------------------------------------------
		// Methods > Scripting
		//------------------------------------------------------------------------------------------

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Enqueues a received message to make it available for scripting.
		/// </summary>
		/// <remarks>
		/// Implemented here instead of 'Model.ScriptConnection' as a separate queue for each
		/// terminal is required, whereas the 'Model.ScriptConnection' is a singleton.
		/// </remarks>
		protected virtual void EnqueueReceivedMessageForScripting(string value)
		{
			lock (this.lastEnqueuedReceivedMessageForScriptingSyncObj) // Access to both must be synchronized!
			{                                                          // Otherwise, e.g. 'LastEnqueued' could
				lock (this.availableReceivedMessagesForScripting)      // yet be emtpy while the queue already
				{                                                      // contains an item!
					this.availableReceivedMessagesForScripting.Enqueue(value);
					this.lastEnqueuedReceivedMessageForScripting = value;

					DebugScriptingPostfixedQuoted(value, "enqueued for scripting."); // Same reason as above, acceptable
				}                                                                         // to do inside lock since debug only.
			}
		}

		/// <summary>
		/// Returns the message that has last been enqueued into the receive queue that is available for scripting.
		/// </summary>
		public virtual void GetLastEnqueuedReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			lock (this.lastEnqueuedReceivedMessageForScriptingSyncObj)
			{
				value = this.lastEnqueuedReceivedMessageForScripting;

				DebugScriptingPostfixedQuoted(value, "retrieved as last enqueued received message for scripting.");
			}
		}

		/// <summary>
		/// Clears the last enqueued message that is available for scripting.
		/// </summary>
		public virtual void ClearLastEnqueuedReceivedMessageForScripting(out string cleared)
		{
			AssertNotDisposed();

			lock (this.lastEnqueuedReceivedMessageForScriptingSyncObj)
			{
				DebugScriptingPostfixedQuoted(this.lastEnqueuedReceivedMessageForScripting, "cleared as last enqueued received message for scripting.");

				cleared = this.lastEnqueuedReceivedMessageForScripting;

				this.lastEnqueuedReceivedMessageForScripting = null;
			}
		}

		/// <summary>
		/// Gets the next received message that is available for scripting and removes it from the queue.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The underlying <see cref="Queue{T}"/> is empty.
		/// </exception>
		public virtual void DequeueNextAvailableReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			lock (this.lastDequeuedReceivedMessageForScriptingSyncObj) // Access to both must be synchronized!
			{                                                          // Otherwise, e.g. 'NextAvailable' could
				lock (this.availableReceivedMessagesForScripting)      // yet be emtpy while the queue already
				{                                                      // contains an item!
					value = this.availableReceivedMessagesForScripting.Dequeue();
					this.lastDequeuedReceivedMessageForScripting = value;

					DebugScriptingPostfixedQuoted(value, "dequeued for scripting."); // Same reason as above, acceptable
				}                                                                         // to do inside lock since debug only.
			}
		}

		/// <summary>
		/// Returns the received message that has last been dequeued from the receive queue for scripting.
		/// </summary>
		public virtual void GetLastDequeuedReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			lock (this.lastDequeuedReceivedMessageForScriptingSyncObj)
			{
				value = this.lastDequeuedReceivedMessageForScripting;

				DebugScriptingPostfixedQuoted(value, "retrieved as last dequeued received message for scripting.");
			}
		}

		/// <summary>
		/// Clears the last dequeued message that is available for scripting.
		/// </summary>
		public virtual void ClearLastDequeuedReceivedMessageForScripting(out string cleared)
		{
			AssertNotDisposed();

			lock (this.lastDequeuedReceivedMessageForScriptingSyncObj)
			{
				DebugScriptingPostfixedQuoted(this.lastDequeuedReceivedMessageForScripting, "cleared as last dequeued received message for scripting.");

				cleared = this.lastDequeuedReceivedMessageForScripting;

				this.lastDequeuedReceivedMessageForScripting = null;
			}
		}

		/// <remarks>
		/// \remind (2018-03-27 / MKY)
		/// 'LastAvailable' only works properly for a terminating number of received messages, but
		/// not for consecutive receiving. This method shall be eliminated as soon as the obsolete
		/// GetLastReceived(), CheckLastReceived() and WaitFor() have been removed.
		/// </remarks>
		public virtual void GetLastAvailableReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			lock (this.availableReceivedMessagesForScripting)
			{
				if (this.availableReceivedMessagesForScripting.Count > 0)
				{
					var messages = this.availableReceivedMessagesForScripting.ToArray();
					value = messages[messages.Length - 1];

					DebugScriptingPostfixedQuoted(value, "retrieved as last available received message for scripting.");
				}
				else
				{
					value = null;

					DebugScripting("[nothing] retrieved as last available received message for scripting.");
				}
			}
		}

		/// <summary>
		/// Cleares all available messages in the receive queue for scripting.
		/// </summary>
		public void ClearAvailableReceivedMessagesForScripting(out string[] clearedMessages)
		{
			AssertNotDisposed();

			lock (this.availableReceivedMessagesForScripting)
			{
				clearedMessages = this.availableReceivedMessagesForScripting.ToArray();
				this.availableReceivedMessagesForScripting.Clear();

				DebugScriptingQueueCleared(clearedMessages);
			}
		}

	#endif // WITH_SCRIPTING

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool ClearRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{                                     // Only try for some time, otherwise ignore.
				try                               // Prevents deadlocks among main thread (view)
				{                                 //   and large amounts of incoming data.
					this.rawTerminal.ClearRepository(repositoryType);
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool ClearRepositories()
		{
			AssertNotDisposed();

			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{                                     // Only try for some time, otherwise ignore.
				try                               // Prevents deadlocks among main thread (view)
				{                                 //   and large amounts of incoming data.
					this.rawTerminal.ClearRepositories();
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool RefreshRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{                                     // Only try for some time, otherwise ignore.
				try                               // Prevents deadlocks among main thread (view)
				{                                 //   and large amounts of incoming data.
					// Clear repository:
					ClearMyRepository(repositoryType);
					OnRepositoryCleared(new EventArgs<RepositoryType>(repositoryType));

					// Reload repository:
					this.isReloading = true;
					foreach (var raw in this.rawTerminal.RepositoryToChunks(repositoryType))
					{
						ProcessAndSignalRawChunk(raw, LineChunkAttribute.None); // Attributes are not (yet) supported on reloading => bug #211.
					}
					this.isReloading = false;
					OnRepositoryReloaded(new EventArgs<RepositoryType>(repositoryType));
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <remarks>
		/// Alternatively, clear/refresh operations could be implemented asynchronously.
		/// Advantages:
		///  > No deadlock possible below.
		/// Disadvantages:
		///  > User does not get immediate feedback that a time consuming operation is taking place.
		///  > User actually cannot trigger any other operation.
		///  > Other synchronization issues?
		/// Therefore, decided to keep the implementation synchronous until new issues pop up.
		/// </remarks>
		public virtual bool RefreshRepositories()
		{
			AssertNotDisposed();

			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{                                     // Only try for some time, otherwise ignore.
				try                               // Prevents deadlocks among main thread (view)
				{                                 //   and large amounts of incoming data.
					// Clear repositories:
					ClearMyRepository(RepositoryType.Tx);
					ClearMyRepository(RepositoryType.Bidir);
					ClearMyRepository(RepositoryType.Rx);
					OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Tx));
					OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Bidir));
					OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Rx));

					// Reload repositories:
					this.isReloading = true;
					foreach (var raw in this.rawTerminal.RepositoryToChunks(RepositoryType.Bidir))
					{
						ProcessAndSignalRawChunk(raw, LineChunkAttribute.None); // Attributes are not (yet) supported on reloading => bug #211.
					}
					this.isReloading = false;
					OnRepositoryReloaded(new EventArgs<RepositoryType>(RepositoryType.Tx));
					OnRepositoryReloaded(new EventArgs<RepositoryType>(RepositoryType.Bidir));
					OnRepositoryReloaded(new EventArgs<RepositoryType>(RepositoryType.Rx));
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary></summary>
		protected virtual void ClearMyRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:      /* Nothing to do. */      break;

					case RepositoryType.Tx:    this.txRepository   .Clear(); break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository   .Clear(); break;

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <remarks>
		/// Note that value reflects the byte count of the elements contained in the repository,
		/// i.e. the byte count of the elements shown. The value thus not necessarily reflects the
		/// total byte count of a sent or received sequence, a hidden EOL is e.g. not reflected.
		/// </remarks>
		public virtual int GetRepositoryByteCount(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:  return (0);

					case RepositoryType.Tx:    return (this.txRepository   .ByteCount);
					case RepositoryType.Bidir: return (this.bidirRepository.ByteCount);
					case RepositoryType.Rx:    return (this.rxRepository   .ByteCount);

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual int GetRepositoryLineCount(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:  return (0);

					case RepositoryType.Tx:    return (this.txRepository   .Count);
					case RepositoryType.Bidir: return (this.bidirRepository.Count);
					case RepositoryType.Rx:    return (this.rxRepository   .Count);

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual DisplayElementCollection RepositoryToDisplayElements(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:  return (null);

					case RepositoryType.Tx:    return (this.txRepository   .ToElements());
					case RepositoryType.Bidir: return (this.bidirRepository.ToElements());
					case RepositoryType.Rx:    return (this.rxRepository   .ToElements());

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual DisplayLineCollection RepositoryToDisplayLines(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:  return (null);

					case RepositoryType.Tx:    return (this.txRepository.   ToLines());
					case RepositoryType.Bidir: return (this.bidirRepository.ToLines());
					case RepositoryType.Rx:    return (this.rxRepository   .ToLines());

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual DisplayLine LastDisplayLineAuxiliary(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:  return (null);

					case RepositoryType.Tx:    return (this.txRepository.   LastLineAuxiliary());
					case RepositoryType.Bidir: return (this.bidirRepository.LastLineAuxiliary());
					case RepositoryType.Rx:    return (this.rxRepository   .LastLineAuxiliary());

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual void ClearLastDisplayLineAuxiliary(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:    /* Nothing to do. */                         break;

					case RepositoryType.Tx:    this.txRepository.   ClearLastLineAuxiliary(); break;
					case RepositoryType.Bidir: this.bidirRepository.ClearLastLineAuxiliary(); break;
					case RepositoryType.Rx:    this.rxRepository   .ClearLastLineAuxiliary(); break;

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual string RepositoryToExtendedDiagnosticsString(RepositoryType repositoryType)
		{
			return (RepositoryToExtendedDiagnosticsString(repositoryType, ""));
		}

		/// <summary></summary>
		public virtual string RepositoryToExtendedDiagnosticsString(RepositoryType repositoryType, string indent)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:  return (null);

					case RepositoryType.Tx:    return (this.txRepository   .ToExtendedDiagnosticsString(indent));
					case RepositoryType.Bidir: return (this.bidirRepository.ToExtendedDiagnosticsString(indent));
					case RepositoryType.Rx:    return (this.rxRepository   .ToExtendedDiagnosticsString(indent));

					default: throw (new ArgumentOutOfRangeException("repository", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual List<RawChunk> RepositoryToRawChunks(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.rawTerminal.RepositoryToChunks(repositoryType));
		}

		private void DisposeRepositories()
		{
			lock (this.repositorySyncObj)
			{
				if (this.txRepository != null)
				{
					this.txRepository.Clear();
					this.txRepository = null;
				}

				if (this.bidirRepository != null)
				{
					this.bidirRepository.Clear();
					this.bidirRepository = null;
				}

				if (this.rxRepository != null)
				{
					this.rxRepository.Clear();
					this.rxRepository = null;
				}
			}
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Settings
		//------------------------------------------------------------------------------------------
		// Settings
		//------------------------------------------------------------------------------------------

		private void AttachTerminalSettings(Settings.TerminalSettings terminalSettings)
		{
			if (ReferenceEquals(this.terminalSettings, terminalSettings))
				return;

			if (this.terminalSettings != null)
				DetachTerminalSettings();

			this.terminalSettings = terminalSettings;
			this.terminalSettings.Changed += terminalSettings_Changed;
		}

		private void DetachTerminalSettings()
		{
			if (this.terminalSettings != null)
			{
				this.terminalSettings.Changed -= terminalSettings_Changed;
				this.terminalSettings = null;
			}
		}

		private void ApplyTerminalSettings()
		{
			ApplyIOSettings();
			ApplyBufferSettings();
			ApplyDisplaySettings();
			ApplySendSettings();
		}

		private void ApplyIOSettings()
		{
			this.rawTerminal.IOSettings = this.terminalSettings.IO;
		}

		private void ApplyBufferSettings()
		{
			this.rawTerminal.BufferSettings = this.terminalSettings.Buffer;

			RefreshRepositories();
		}

		private void ApplyDisplaySettings()
		{
			this.txRepository.Capacity    = this.terminalSettings.Display.MaxLineCount;
			this.bidirRepository.Capacity = this.terminalSettings.Display.MaxLineCount;
			this.rxRepository.Capacity    = this.terminalSettings.Display.MaxLineCount;

			RefreshRepositories();
		}

		private void ApplySendSettings()
		{
			ConfigurePeriodicXOnTimer();
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void terminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// TerminalSettings changed
				ApplyTerminalSettings();
			}
			else
			{
				if (ReferenceEquals(e.Inner.Source, this.terminalSettings.IO))
				{
					// IOSettings changed
					ApplyIOSettings();
				}
				else if (ReferenceEquals(e.Inner.Source, this.terminalSettings.Buffer))
				{
					// BufferSettings changed
					ApplyBufferSettings();
				}
				else if (ReferenceEquals(e.Inner.Source, this.terminalSettings.Display))
				{
					// DisplaySettings changed
					ApplyDisplaySettings();
				}
				else if (ReferenceEquals(e.Inner.Source, this.terminalSettings.Send))
				{
					// SendSettings changed
					ApplySendSettings();
				}
			}
		}

		#endregion

		#region Raw Terminal
		//------------------------------------------------------------------------------------------
		// Raw Terminal
		//------------------------------------------------------------------------------------------

		private void AttachRawTerminal(RawTerminal rawTerminal)
		{
			this.rawTerminal = rawTerminal;
			{
				this.rawTerminal.IOChanged         += rawTerminal_IOChanged;
				this.rawTerminal.IOControlChanged  += rawTerminal_IOControlChanged;
				this.rawTerminal.IOError           += rawTerminal_IOError;

				this.rawTerminal.RawChunkSent      += rawTerminal_RawChunkSent;
				this.rawTerminal.RawChunkReceived  += rawTerminal_RawChunkReceived;
				this.rawTerminal.RepositoryCleared += rawTerminal_RepositoryCleared;
			}
		}

		private void DetachAndDisposeRawTerminal()
		{
			if (this.rawTerminal != null)
			{
				this.rawTerminal.IOChanged         -= rawTerminal_IOChanged;
				this.rawTerminal.IOControlChanged  -= rawTerminal_IOControlChanged;
				this.rawTerminal.IOError           -= rawTerminal_IOError;

				this.rawTerminal.RawChunkSent      -= rawTerminal_RawChunkSent;
				this.rawTerminal.RawChunkReceived  -= rawTerminal_RawChunkReceived;
				this.rawTerminal.RepositoryCleared -= rawTerminal_RepositoryCleared;

				this.rawTerminal.Dispose();
				this.rawTerminal = null;
			}
		}

		#endregion

		#region Raw Terminal Events
		//------------------------------------------------------------------------------------------
		// Raw Terminal Events
		//------------------------------------------------------------------------------------------

		private void rawTerminal_IOChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnIOChanged(e);
		}

		private void rawTerminal_IOControlChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalSettings.Display.IncludeIOControl)
			{
				var texts = IOControlChangeTexts();
				                                                 //// Forsee capacity for separators.
				var c = new DisplayElementCollection(texts.Count * 2); // Preset the required capacity to improve memory management.
				foreach (var t in texts)
				{
					if (c.Count > 0)
						c.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));

					c.Add(new DisplayElement.IOControl(Direction.Bidir, t));
				}

				// Do not lock (this.clearAndRefreshSyncObj)! That would lead to deadlocks if close/dispose
				// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread!
				{
					foreach (var de in c)
						OnDisplayElementAdded((IODirection)de.Direction, de);
				}

				OnIOControlChanged(new IOControlEventArgs(IODirection.Bidir, texts));
			}
			else
			{
				OnIOControlChanged(new IOControlEventArgs(IODirection.Bidir));
			}
		}

		private void rawTerminal_IOError(object sender, IOErrorEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			// Do not lock (this.clearAndRefreshSyncObj)! That would lead to deadlocks if close/dispose
			// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread!
			{
				var spe = (e as SerialPortErrorEventArgs);
				if (spe != null)
				{
					// Handle serial port errors whenever possible:
					switch (spe.SerialPortError)
					{                                                                                // Same as 'spe.Direction'.
						case System.IO.Ports.SerialError.Frame:    OnDisplayElementAdded(IODirection.Rx, new DisplayElement.ErrorInfo(Direction.Rx, RxFramingErrorString));        break;
						case System.IO.Ports.SerialError.Overrun:  OnDisplayElementAdded(IODirection.Rx, new DisplayElement.ErrorInfo(Direction.Rx, RxBufferOverrunErrorString));  break;
						case System.IO.Ports.SerialError.RXOver:   OnDisplayElementAdded(IODirection.Rx, new DisplayElement.ErrorInfo(Direction.Rx, RxBufferOverflowErrorString)); break;
						case System.IO.Ports.SerialError.RXParity: OnDisplayElementAdded(IODirection.Rx, new DisplayElement.ErrorInfo(Direction.Rx, RxParityErrorString));         break;
						case System.IO.Ports.SerialError.TXFull:   OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, TxBufferFullErrorString));     break;
						default:                                   OnIOError(e);                                                                                                   break;
					}
				}
				else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Rx)) // Acceptable errors are only shown as terminal text.
				{
					OnDisplayElementAdded(IODirection.Rx, new DisplayElement.ErrorInfo(Direction.Rx, e.Message, true));
				}
				else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Tx)) // Acceptable errors are only shown as terminal text.
				{
					OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, e.Message, true));
				}
				else
				{
					OnIOError(e);
				}
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "RawTerminal.RawChunkReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void rawTerminal_RawChunkSent(object sender, EventArgs<RawChunk> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until reloading has completed.
			{
				var args = new RawChunkEventArgs(e.Value); // 'RawChunk' object is immutable, subsequent use is OK.
				OnRawChunkSent(args);
				ProcessAndSignalRawChunk(e.Value, args.Attribute); // 'RawChunk' object is immutable, subsequent use is OK.
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "RawTerminal.RawChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void rawTerminal_RawChunkReceived(object sender, EventArgs<RawChunk> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until reloading has completed.
			{
				var args = new RawChunkEventArgs(e.Value); // 'RawChunk' object is immutable, subsequent use is OK.
				OnRawChunkReceived(args);
				ProcessAndSignalRawChunk(e.Value, args.Attribute); // 'RawChunk' object is immutable, subsequent use is OK.
			#if (WITH_SCRIPTING)
				ProcessAndSignalRawChunkForScripting(e.Value);     // See method's remarks for background information.
			#endif // WITH_SCRIPTING
			}
		}

		private void rawTerminal_RepositoryCleared(object sender, EventArgs<RepositoryType> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until reloading has completed.
			{
				ClearMyRepository(e.Value);
				OnRepositoryCleared(e);
			}
		}

		#endregion

		#region Event Raising
		//------------------------------------------------------------------------------------------
		// Event Raising
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(IOControlEventArgs e)
		{
			this.eventHelper.RaiseSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

	#if (WITH_SCRIPTING)

		/// <remarks>
		/// Named 'Sending...' rather than '...Sent' since sending is just about to happen and
		/// can be modified using the <see cref="ModifiablePacketEventArgs.Data"/> property or
		/// even canceled using the <see cref="ModifiablePacketEventArgs.Cancel"/> property.
		/// This is similar to the behavior of e.g. the 'OnValidating' event of WinForms controls.
		/// </remarks>
		protected virtual void OnSendingPacket(ModifiablePacketEventArgs e)
		{
			if (!this.isReloading) // This plug-in event must only be raised once.
				this.eventHelper.RaiseSync<ModifiablePacketEventArgs>(SendingPacket, this, e);
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		[CallingContract(IsAlwaysSequentialIncluding = "OnRawChunkReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		protected virtual void OnRawChunkSent(RawChunkEventArgs e)
		{
			DebugContentEvents("OnRawChunkSent " + e.Value.ToString());

			this.eventHelper.RaiseSync<RawChunkEventArgs>(RawChunkSent, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsAlwaysSequentialIncluding = "OnRawChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		protected virtual void OnRawChunkReceived(RawChunkEventArgs e)
		{
			DebugContentEvents("OnRawChunkReceived " + e.Value.ToString());

			this.eventHelper.RaiseSync<RawChunkEventArgs>(RawChunkReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementAdded(IODirection direction, DisplayElement element)
		{
			var elements = new DisplayElementCollection(1); // Preset the required capacity to improve memory management.
			elements.Add(element); // No clone needed as the element must be created when calling this event method.
			OnDisplayElementsAdded(direction, elements);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsAdded(IODirection direction, DisplayElementCollection elements)
		{
			switch (direction)
			{
				case IODirection.Tx:
				{
					lock (this.repositorySyncObj)
					{
						this.txRepository   .Enqueue(elements.Clone()); // Clone elements as they are needed again below.
						this.bidirRepository.Enqueue(elements.Clone()); // Clone elements as they are needed again below.
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
						OnDisplayElementsSent(new DisplayElementsEventArgs(elements)); // No clone needed as elements are not needed again.

					break;
				}

				case IODirection.Rx:
				{
					lock (this.repositorySyncObj)
					{
						this.bidirRepository.Enqueue(elements.Clone()); // Clone elements as they are needed again below.
						this.rxRepository   .Enqueue(elements.Clone()); // Clone elements as they are needed again below.
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
						OnDisplayElementsReceived(new DisplayElementsEventArgs(elements)); // No clone needed as elements are not needed again.

					break;
				}

				case IODirection.Bidir:
				case IODirection.None:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(DisplayElementsEventArgs e)
		{
			DebugContentEvents("OnDisplayElementsSent " + e.Elements.ToString());

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(DisplayElementsEventArgs e)
		{
			DebugContentEvents("OnDisplayElementsReceived " + e.Elements.ToString());

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineReplaced(IODirection direction, DisplayElementCollection currentLineElements)
		{
			switch (direction)
			{
				case IODirection.Tx:
				{
					lock (this.repositorySyncObj)
					{
						this.txRepository   .ReplaceCurrentLine(currentLineElements.Clone()); // Clone elements as they are needed again below.
						this.bidirRepository.ReplaceCurrentLine(currentLineElements.Clone()); // Clone elements as they are needed again below.
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
						OnCurrentDisplayLineSentReplaced(new DisplayElementsEventArgs(currentLineElements)); // No clone needed as elements are not needed again.

					break;
				}

				case IODirection.Rx:
				{
					lock (this.repositorySyncObj)
					{
						this.bidirRepository.ReplaceCurrentLine(currentLineElements.Clone()); // Clone elements as they are needed again below.
						this.rxRepository   .ReplaceCurrentLine(currentLineElements.Clone()); // Clone elements as they are needed again below.
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
						OnCurrentDisplayLineReceivedReplaced(new DisplayElementsEventArgs(currentLineElements)); // No clone needed as elements are not needed again.

					break;
				}

				case IODirection.Bidir:
				case IODirection.None:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineSentReplaced(DisplayElementsEventArgs e)
		{
			DebugContentEvents("OnCurrentDisplayLineSentReplaced " + e.Elements.ToString());

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<DisplayElementsEventArgs>(CurrentDisplayLineSentReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineReceivedReplaced(DisplayElementsEventArgs e)
		{
			DebugContentEvents("OnCurrentDisplayLineReceivedReplaced " + e.Elements.ToString());

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<DisplayElementsEventArgs>(CurrentDisplayLineReceivedReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineCleared(IODirection direction)
		{
			switch (direction)
			{
				case IODirection.Tx:
				{
					lock (this.repositorySyncObj)
					{
						this.txRepository   .ClearCurrentLine();
						this.bidirRepository.ClearCurrentLine();
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
						OnCurrentDisplayLineSentCleared(new EventArgs());

					break;
				}

				case IODirection.Rx:
				{
					lock (this.repositorySyncObj)
					{
						this.bidirRepository.ClearCurrentLine();
						this.rxRepository   .ClearCurrentLine();
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
						OnCurrentDisplayLineReceivedCleared(new EventArgs());

					break;
				}

				case IODirection.Bidir:
				case IODirection.None:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineSentCleared(EventArgs e)
		{
			DebugContentEvents("OnCurrentDisplayLineSentCleared");

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync(CurrentDisplayLineSentCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineReceivedCleared(EventArgs e)
		{
			DebugContentEvents("OnCurrentDisplayLineReceivedCleared");

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync(CurrentDisplayLineReceivedCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesAdded(IODirection direction, DisplayLineCollection lines)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
			{
				switch (direction)
				{
					case IODirection.Tx: OnDisplayLinesSent    (new DisplayLinesEventArgs(lines)); break;
					case IODirection.Rx: OnDisplayLinesReceived(new DisplayLinesEventArgs(lines)); break;

					case IODirection.Bidir:
					case IODirection.None:
						throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

					default:
						throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesSent(DisplayLinesEventArgs e)
		{
			DebugContentEvents("OnDisplayLinesSent " + e.Lines.Count);

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(DisplayLinesEventArgs e)
		{
			DebugContentEvents("OnDisplayLinesReceived " + e.Lines.Count);

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		protected virtual void OnScriptPacketReceived(PacketEventArgs e)
		{
			if (!this.isReloading) // This plug-in event must only be raised once.
				this.eventHelper.RaiseSync<PacketEventArgs>(ScriptPacketReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnScriptMessageReceived(MessageEventArgs e)
		{
			if (!this.isReloading) // This plug-in event must only be raised once.
				this.eventHelper.RaiseSync<PacketEventArgs>(ScriptMessageReceived, this, e);
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(EventArgs<RepositoryType> e)
		{
			DebugContentEvents("OnRepositoryCleared");

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<EventArgs<RepositoryType>>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(EventArgs<RepositoryType> e)
		{
			DebugContentEvents("OnRepositoryReloaded");

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				this.eventHelper.RaiseSync<EventArgs<RepositoryType>>(RepositoryReloaded, this, e);
		}

		#endregion

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			// See below why AssertNotDisposed() is not called on such basic method!

			return (ToExtendedDiagnosticsString()); // No 'real' ToString() method required yet.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToExtendedDiagnosticsString(string indent = "")
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			var sb = new StringBuilder();
			lock (this.repositorySyncObj)
			{
				if (this.terminalSettings != null) // Possible during disposing.
				{
					sb.AppendLine(indent + "> Settings: " + this.terminalSettings);
				}

				if (this.rawTerminal != null) // Possible during disposing.
				{
					sb.AppendLine(indent + "> RawTerminal: ");
					sb.AppendLine(this.rawTerminal.ToExtendedDiagnosticsString(indent + "   "));
				}

				if (this.txRepository != null) // Possible during disposing.
				{
					sb.AppendLine(indent + "> TxRepository: ");
					sb.Append    (this.txRepository.ToExtendedDiagnosticsString(indent + "   ")); // Repository will add 'NewLine'.
				}

				if (this.bidirRepository != null) // Possible during disposing.
				{
					sb.AppendLine(indent + "> BidirRepository: ");
					sb.Append    (this.bidirRepository.ToExtendedDiagnosticsString(indent + "   ")); // Repository will add 'NewLine'.
				}

				if (this.bidirRepository != null) // Possible during disposing.
				{
					sb.AppendLine(indent + "> RxRepository: ");
					sb.Append    (this.rxRepository.ToExtendedDiagnosticsString(indent + "   ")); // Repository will add 'NewLine'.
				}
			}
			return (sb.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		public virtual string ToShortIOString()
		{
			if (IsDisposed)
				return (typeof(RawTerminal).ToString()); // Do not call AssertNotDisposed() on such basic method!

			if (this.rawTerminal != null)
				return (this.rawTerminal.ToShortIOString());
			else
				return (typeof(RawTerminal).ToString());
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"#" + this.instanceId.ToString("D2", CultureInfo.CurrentCulture),
					"[" + ToShortIOString() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadState(string message)
		{
			DebugMessage(message);
		}

		[Conditional("DEBUG_CONTENT_EVENTS")]
		private void DebugContentEvents(string message)
		{
			DebugMessage(message);
		}

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		[Conditional("DEBUG_SCRIPTING")]
		protected virtual void DebugScripting(string message)
		{
			Debug.WriteLine(string.Format("{0,-26}", GetType()) + " '" + ToShortIOString() + "': " + message);
		}

		/// <summary></summary>
		[Conditional("DEBUG_SCRIPTING")]
		protected virtual void DebugScriptingPrefixedQuoted(string prefix, string quoted)
		{
			Debug.WriteLine(string.Format("{0,-26}", GetType()) + " '" + ToShortIOString() + "': " + prefix + @" """ + quoted + @""".");
		}

		/// <summary></summary>
		[Conditional("DEBUG_SCRIPTING")]
		protected virtual void DebugScriptingPostfixedQuoted(string quoted, string postfix)
		{
			Debug.WriteLine(string.Format("{0,-26}", GetType()) + " '" + ToShortIOString() + @"': """ + quoted + @""" " + postfix);
		}

		/// <summary></summary>
		[Conditional("DEBUG_SCRIPTING")]
		protected virtual void DebugScriptingQueueCount(int count)
		{
			if (count > 0) // Otherwise, debug output gets spoilt...
				DebugScripting(string.Format("{0} received messages available for scripting.", count));
		}

		/// <summary></summary>
		[Conditional("DEBUG_SCRIPTING")]
		protected virtual void DebugScriptingQueueCleared(string[] cleared)
		{
			if (ArrayEx.IsNullOrEmpty(cleared))
				DebugScripting("Message queue for scripting cleared, contained [nothing].");
			else
				DebugScripting("Message queue for scripting cleared, contained { " + ArrayEx.ValuesToString(cleared, '"') + " }.");
		}

	#endif // WITH_SCRIPTING

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
