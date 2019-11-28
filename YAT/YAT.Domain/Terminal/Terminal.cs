﻿//==================================================================================================
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
	/// This class is implemented using partial classes separating sending/repositories/processing
	/// functionality. Using partial classes rather than aggregated sender, repositories, processor,...
	/// so far for these reasons:
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
		public event EventHandler<EventArgs<DateTime>> IOChanged;

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
		public event EventHandler<EventArgs<RawChunk>> RawChunkSent;

		/// <summary></summary>
		public event EventHandler<EventArgs<RawChunk>> RawChunkReceived;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsTxAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsBidirAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsRxAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks>
		/// Using "current line replaced" rather than "element(s) removed" semantic because removing
		/// elements would likely be more error prone since...
		/// ...exact sequence of adding and removing elements has to exactly match.
		/// ...an already added element would likely have to be unfolded to remove parts of it!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public event EventHandler<DisplayElementsEventArgs> CurrentDisplayLineTxReplaced;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks>
		/// Using "current line replaced" rather than "element(s) removed" semantic because removing
		/// elements would likely be more error prone since...
		/// ...exact sequence of adding and removing elements has to exactly match.
		/// ...an already added element would likely have to be unfolded to remove parts of it!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public event EventHandler<DisplayElementsEventArgs> CurrentDisplayLineBidirReplaced;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks>
		/// Using "current line replaced" rather than "element(s) removed" semantic because removing
		/// elements would likely be more error prone since...
		/// ...exact sequence of adding and removing elements has to exactly match.
		/// ...an already added element would likely have to be unfolded to remove parts of it!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public event EventHandler<DisplayElementsEventArgs> CurrentDisplayLineRxReplaced;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="CurrentDisplayLineTxReplaced"/> above.</remarks>
		public event EventHandler CurrentDisplayLineTxCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="CurrentDisplayLineBidirReplaced"/> above.</remarks>
		public event EventHandler CurrentDisplayLineBidirCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="CurrentDisplayLineRxReplaced"/> above.</remarks>
		public event EventHandler CurrentDisplayLineRxCleared;

		/// <remarks>Intentionally no additional 'Line' event: Covered by 'Lines', ease of use.</remarks>
		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesTxAdded;

		/// <remarks>Intentionally no additional 'Line' event: Covered by 'Lines', ease of use.</remarks>
		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesBidirAdded;

		/// <remarks>Intentionally no additional 'Line' event: Covered by 'Lines', ease of use.</remarks>
		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesRxAdded;

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Occurs when a packet has been received by the host application. The event args contain
		/// the binary raw data that has been received, including control characters, EOL,...
		/// </summary>
		public event EventHandler<PacketEventArgs> ScriptPacketReceived;

	#endif

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryTxCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryBidirCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryRxCleared;

		/// <remarks>Separated from <see cref="DisplayLinesTxAdded"/> for not processing count/rate/log... on reload again.</remarks>
		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayLinesEventArgs> RepositoryTxReloaded;

		/// <remarks>Separated from <see cref="DisplayLinesBidirAdded"/> for not processing count/rate/log... on reload again.</remarks>
		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayLinesEventArgs> RepositoryBidirReloaded;

		/// <remarks>Separated from <see cref="DisplayLinesRxAdded"/> for not processing count/rate/log... on reload again.</remarks>
		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<DisplayLinesEventArgs> RepositoryRxReloaded;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		protected Terminal(Settings.TerminalSettings settings)
		{
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

			AttachTerminalSettings(settings);
			CreateRepositories(settings);
			InitializeProcess();
			AttachRawTerminal(new RawTerminal(this.terminalSettings.IO, this.terminalSettings.Buffer));

		////this.isReloading has been initialized to false.

			CreateAndStartSendThreads();
		}

		/// <summary></summary>
		protected Terminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

			AttachTerminalSettings(settings);
			CreateRepositories(terminal);
			InitializeProcess();
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
					// In the 'normal' case, the related timers will already have been stopped in Stop()...
					DisposeProcess();
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
		protected virtual bool IsReloading
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.isReloading);
			}
		}

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

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		#region Start/Stop/Close
		//------------------------------------------------------------------------------------------
		// Start/Stop/Close
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

		#region Parse
		//------------------------------------------------------------------------------------------
		// Parse
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

		#region I/O Control
		//------------------------------------------------------------------------------------------
		// I/O Control
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

		#region Special ;-)
		//------------------------------------------------------------------------------------------
		// Special ;-)
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Enqueues the easter egg message.
		/// </summary>
		public virtual void EnqueueEasterEggMessage()
		{
			InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "The bites have been eaten by the rabbit ;-]", true));
		}

		#endregion

		#region Format
		//------------------------------------------------------------------------------------------
		// Format
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Formats the specified time stamp.
		/// </summary>
		public virtual string Format(DateTime ts)
		{
			var de = new DisplayElement.TimeStampInfo(ts, TerminalSettings.Display.TimeStampFormat, TerminalSettings.Display.TimeStampUseUtc, "", "");
			return (de.Text);
		}

		/// <summary>
		/// Formats the specified data sequence.
		/// </summary>
		/// <remarks>
		/// \remind (2017-12-11 / MKY)
		/// Currently limited to data of a single line. Refactoring would be required to format multiple lines
		/// (<see cref="ProcessRawChunk"/> instead of <see cref="ByteToElement(byte, DateTime, IODirection, Radix)"/>).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		public virtual string Format(byte[] data, IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx:    return (Format(data, d, TerminalSettings.Display.TxRadix));
				case IODirection.Rx:    return (Format(data, d, TerminalSettings.Display.RxRadix));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Formats the specified data sequence.
		/// </summary>
		/// <remarks>
		/// \remind (2017-12-11 / MKY)
		/// Currently limited to data of a single line. Refactoring would be required to format multiple lines
		/// (<see cref="ProcessRawChunk"/> instead of <see cref="ByteToElement(byte, DateTime, IODirection, Radix)"/>).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		public virtual string Format(byte[] data, IODirection d, Radix r)
		{
			var lp = new DisplayElementCollection();

			foreach (byte b in data)
			{                                                     // Time stamp is irrelevant for formatting.
				var de = ByteToElement(b, DisplayElement.TimeStampDefault, d, r);
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
					lp.Add(new DisplayElement.ContentSpace((Direction)d));
			}
		}

		#endregion

		#region Convert
		//------------------------------------------------------------------------------------------
		// Convert
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

		#region Scripting
		//------------------------------------------------------------------------------------------
		// Scripting
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

				this.rawTerminal.ChunkSent         += rawTerminal_ChunkSent;
				this.rawTerminal.ChunkReceived     += rawTerminal_ChunkReceived;
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

				this.rawTerminal.ChunkSent         -= rawTerminal_ChunkSent;
				this.rawTerminal.ChunkReceived     -= rawTerminal_ChunkReceived;
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

		private void rawTerminal_IOChanged(object sender, EventArgs<DateTime> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnIOChanged(e);
		}

		private void rawTerminal_IOControlChanged(object sender, EventArgs<DateTime> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalSettings.Display.IncludeIOControl)
			{
				var texts = IOControlChangeTexts();
				var c = new DisplayElementCollection(texts.Count); // Preset the required capacity to improve memory management.
				foreach (var t in texts)
				{                        // 'IOControlInfo' elements are inline elements, thus neither add info separators nor content spaces inbetween.
					c.Add(new DisplayElement.IOControlInfo(e.Value, Direction.Bidir, t));
				}

				// Do not lock (this.clearAndRefreshSyncObj)! That would lead to deadlocks if close/dispose
				// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread!
				{
					InlineDisplayElements(IODirection.Bidir, c);
				}

				OnIOControlChanged(new IOControlEventArgs(IODirection.Bidir, texts));
			}
			else
			{
				OnIOControlChanged(new IOControlEventArgs());
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
					{                                                                               // Same as 'spe.Direction'.
						case System.IO.Ports.SerialError.Frame:    InlineDisplayElement(IODirection.Rx, new DisplayElement.ErrorInfo(e.TimeStamp, Direction.Rx, RxFramingErrorString));        break;
						case System.IO.Ports.SerialError.Overrun:  InlineDisplayElement(IODirection.Rx, new DisplayElement.ErrorInfo(e.TimeStamp, Direction.Rx, RxBufferOverrunErrorString));  break;
						case System.IO.Ports.SerialError.RXOver:   InlineDisplayElement(IODirection.Rx, new DisplayElement.ErrorInfo(e.TimeStamp, Direction.Rx, RxBufferOverflowErrorString)); break;
						case System.IO.Ports.SerialError.RXParity: InlineDisplayElement(IODirection.Rx, new DisplayElement.ErrorInfo(e.TimeStamp, Direction.Rx, RxParityErrorString));         break;
						case System.IO.Ports.SerialError.TXFull:   InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(e.TimeStamp, Direction.Tx, TxBufferFullErrorString));     break;
						default:                                   OnIOError(e);                                                                                                               break;
					}
				}
				else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Rx)) // Acceptable errors are only shown as terminal text.
				{
					InlineDisplayElement(IODirection.Rx, new DisplayElement.ErrorInfo(e.TimeStamp, Direction.Rx, e.Message, true));
				}
				else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Tx)) // Acceptable errors are only shown as terminal text.
				{
					InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(e.TimeStamp, Direction.Tx, e.Message, true));
				}
				else
				{
					OnIOError(e);
				}
			}
		}

		/// <remarks>
		/// This event is raised when a chunk is sent by the <see cref="UnderlyingIOProvider"/>.
		/// The event is not raised on reloading, reloading is done by the 'Refresh...()' methods.
		/// </remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "RawTerminal.ChunkReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void rawTerminal_ChunkSent(object sender, EventArgs<RawChunk> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until clearing or refreshing has completed.
			{
				OnRawChunkSent(e);        // 'RawChunk' objects are immutable, subsequent use is OK.
				ProcessRawChunk(e.Value); // 'RawChunk' objects are immutable, subsequent use is OK.
			}
		}

		/// <remarks>
		/// This event is raised when a chunk is received by the <see cref="UnderlyingIOProvider"/>.
		/// The event is not raised on reloading, reloading is done by the 'Refresh...()' methods.
		/// </remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "RawTerminal.ChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void rawTerminal_ChunkReceived(object sender, EventArgs<RawChunk> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until clearing or refreshing has completed.
			{
				OnRawChunkReceived(e);    // 'RawChunk' objects are immutable, subsequent use is OK.
				ProcessRawChunk(e.Value); // 'RawChunk' objects are immutable, subsequent use is OK.
			}
		}

		private void rawTerminal_RepositoryCleared(object sender, EventArgs<RepositoryType> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until clearing or refreshing has completed.
			{
				// Reset processing:
				ResetProcess(e.Value);

				// Clear repository:
				ClearMyRepository(e.Value);
			}
		}

		#endregion

		#region Event Raising
		//------------------------------------------------------------------------------------------
		// Event Raising
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs<DateTime> e)
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
			if (!IsReloading) // This plug-in event must only be raised once.
				this.eventHelper.RaiseSync<ModifiablePacketEventArgs>(SendingPacket, this, e);
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		[CallingContract(IsAlwaysSequentialIncluding = "OnRawChunkReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		protected virtual void OnRawChunkSent(EventArgs<RawChunk> e)
		{
			DebugContentEvents("OnRawChunkSent " + e.Value.ToString());

			this.eventHelper.RaiseSync<EventArgs<RawChunk>>(RawChunkSent, this, e);
		}

		/// <summary></summary>
		[CallingContract(IsAlwaysSequentialIncluding = "OnRawChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		protected virtual void OnRawChunkReceived(EventArgs<RawChunk> e)
		{
			DebugContentEvents("OnRawChunkReceived " + e.Value.ToString());

			this.eventHelper.RaiseSync<EventArgs<RawChunk>>(RawChunkReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsAdded(RepositoryType repositoryType, DisplayElementsEventArgs e)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    OnDisplayElementsTxAdded   (e); break;
				case RepositoryType.Bidir: OnDisplayElementsBidirAdded(e); break;
				case RepositoryType.Rx:    OnDisplayElementsRxAdded   (e); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsTxAdded(DisplayElementsEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnDisplayElementsTxAdded " + e.Elements.ToString());

			this.eventHelper.RaiseSync<DisplayElementsEventArgs>(DisplayElementsTxAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsBidirAdded(DisplayElementsEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnDisplayElementsBidirAdded " + e.Elements.ToString());

			this.eventHelper.RaiseSync<DisplayElementsEventArgs>(DisplayElementsBidirAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsRxAdded(DisplayElementsEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnDisplayElementsRxAdded " + e.Elements.ToString());

			this.eventHelper.RaiseSync<DisplayElementsEventArgs>(DisplayElementsRxAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineReplaced(RepositoryType repositoryType, DisplayElementsEventArgs e)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    OnCurrentDisplayLineTxReplaced   (e); break;
				case RepositoryType.Bidir: OnCurrentDisplayLineBidirReplaced(e); break;
				case RepositoryType.Rx:    OnCurrentDisplayLineRxReplaced   (e); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineTxReplaced(DisplayElementsEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnCurrentDisplayLineTxReplaced " + e.Elements.ToString());

			this.eventHelper.RaiseSync<DisplayElementsEventArgs>(CurrentDisplayLineTxReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineBidirReplaced(DisplayElementsEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnCurrentDisplayLineBidirReplaced " + e.Elements.ToString());

			this.eventHelper.RaiseSync<DisplayElementsEventArgs>(CurrentDisplayLineBidirReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineRxReplaced(DisplayElementsEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnCurrentDisplayLineRxReplaced " + e.Elements.ToString());

			this.eventHelper.RaiseSync<DisplayElementsEventArgs>(CurrentDisplayLineRxReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineCleared(RepositoryType repositoryType, EventArgs e)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    OnCurrentDisplayLineTxCleared   (e); break;
				case RepositoryType.Bidir: OnCurrentDisplayLineBidirCleared(e); break;
				case RepositoryType.Rx:    OnCurrentDisplayLineRxCleared   (e); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineTxCleared(EventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnCurrentDisplayLineTxCleared");

			this.eventHelper.RaiseSync(CurrentDisplayLineTxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineBidirCleared(EventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnCurrentDisplayLineBidirCleared");

			this.eventHelper.RaiseSync(CurrentDisplayLineBidirCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineRxCleared(EventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnCurrentDisplayLineRxCleared");

			this.eventHelper.RaiseSync(CurrentDisplayLineRxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesAdded(RepositoryType repositoryType, DisplayLinesEventArgs e)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    OnDisplayLinesTxAdded   (e); break;
				case RepositoryType.Bidir: OnDisplayLinesBidirAdded(e); break;
				case RepositoryType.Rx:    OnDisplayLinesRxAdded   (e); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesTxAdded(DisplayLinesEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnDisplayLinesTxAdded " + e.Lines.Count);

			this.eventHelper.RaiseSync<DisplayLinesEventArgs>(DisplayLinesTxAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesBidirAdded(DisplayLinesEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnDisplayLinesBidirAdded " + e.Lines.Count);

			this.eventHelper.RaiseSync<DisplayLinesEventArgs>(DisplayLinesBidirAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesRxAdded(DisplayLinesEventArgs e)
		{
			if (IsReloading) // For performance reasons, skip 'normal' events during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.
				return;

			DebugContentEvents("OnDisplayLinesRxAdded " + e.Lines.Count);

			this.eventHelper.RaiseSync<DisplayLinesEventArgs>(DisplayLinesRxAdded, this, e);
		}

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		protected virtual void OnScriptPacketReceived(PacketEventArgs e)
		{
			if (!IsReloading) // This plug-in event must only be raised once.
				this.eventHelper.RaiseSync<PacketEventArgs>(ScriptPacketReceived, this, e);
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(RepositoryType repositoryType, EventArgs e)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    OnRepositoryTxCleared   (e); break;
				case RepositoryType.Bidir: OnRepositoryBidirCleared(e); break;
				case RepositoryType.Rx:    OnRepositoryRxCleared   (e); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void OnRepositoryTxCleared(EventArgs e)
		{
			DebugContentEvents("OnRepositoryTxCleared");

			this.eventHelper.RaiseSync(RepositoryTxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryBidirCleared(EventArgs e)
		{
			DebugContentEvents("OnRepositoryBidirCleared");

			this.eventHelper.RaiseSync(RepositoryBidirCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryRxCleared(EventArgs e)
		{
			DebugContentEvents("OnRepositoryRxCleared");

			this.eventHelper.RaiseSync(RepositoryRxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(RepositoryType repositoryType, DisplayLinesEventArgs e)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    OnRepositoryTxReloaded   (e); break;
				case RepositoryType.Bidir: OnRepositoryBidirReloaded(e); break;
				case RepositoryType.Rx:    OnRepositoryRxReloaded   (e); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void OnRepositoryTxReloaded(DisplayLinesEventArgs e)
		{
			DebugContentEvents("OnRepositoryTxReloaded");

			this.eventHelper.RaiseSync(RepositoryTxReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryBidirReloaded(DisplayLinesEventArgs e)
		{
			DebugContentEvents("OnRepositoryBidirReloaded");

			this.eventHelper.RaiseSync(RepositoryBidirReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryRxReloaded(DisplayLinesEventArgs e)
		{
			DebugContentEvents("OnRepositoryRxReloaded");

			this.eventHelper.RaiseSync(RepositoryRxReloaded, this, e);
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
