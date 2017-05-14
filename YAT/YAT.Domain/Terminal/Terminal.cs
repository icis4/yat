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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;

using MKY;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Text;

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
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Terminal : IDisposable
	{
		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
		//==========================================================================================

		/// <summary></summary>
		public static readonly string SerialPortHelp =
			@"For serial COM ports, if one of the following error conditions occurs, the according error indication will be shown in the terminal window:" + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxFramingErrorString + ">" + Environment.NewLine +
			@"An input framing error occurs when the last bit received is not a stop bit. This may occur due to a timing error. You will most commonly encounter a framing error when the speed at which the data is being sent is different to that of what you have YAT set to receive it at." + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxBufferOverrunErrorString + ">" + Environment.NewLine +
			@"An input overrun error occurs when the input gets out of synch. The next character will be lost and the input will be re-synch'd." + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxBufferOverflowErrorString + ">" + Environment.NewLine +
			@"An input overflow occurs when there is no more space in the input buffer, i.e. the serial driver, the operating system or YAT doesn't manage to process the incoming data fast enough." + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxParityErrorString + ">" + Environment.NewLine +
			@"An input parity error occurs when a parity check is enabled but the parity bit mismatches. You will most commonly encounter a parity error when the parity setting at which the data is being sent is different to that of what you have YAT set to receive it at." + Environment.NewLine +
			Environment.NewLine +
			@"<" + TxBufferFullErrorString + ">" + Environment.NewLine +
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds...")]
		private struct IOChangedEventHelper
		{
			/// <summary></summary>
			public const int ThresholdMs = 400; // 400 Bytes @ 9600 Baud ~= 400 ms

			/// <summary></summary>
			public bool EventMustBeRaised;

			private DateTime initialTimeStamp;

			/// <summary></summary>
			public void Initialize()
			{
				this.EventMustBeRaised = false;
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
				// Only let the event get raised if it has'nt been yet:
				if (!this.EventMustBeRaised && ChunkSizeIsAboveThreshold(chunkSize))
				{
					this.EventMustBeRaised = true;
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
				// Only let the event get raised if it has'nt been yet:
				if (!this.EventMustBeRaised && DelayIsAboveThreshold(delay))
				{
					this.EventMustBeRaised = true;
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
				// Let the event get raised in any case. This ensures the terminal
				// state gets properly updated during an ongoing long delay:
				if (TotalTimeLagIsAboveThreshold())
				{
					this.EventMustBeRaised = true;
					return (true);
				}

				return (false);
			}

			/// <summary></summary>
			public void EventMustBeRaisedBecauseStatusHasBeenAccessed()
			{
				this.EventMustBeRaised = true;
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

		private const int ThreadWaitTimeout = 200;
		private const int ClearAndRefreshTimeout = 400;

		private const string Undefined = "<Undefined>";

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

		private int instanceId;
		private bool isDisposed;

		private Settings.TerminalSettings terminalSettings;

		private RawTerminal rawTerminal;

		private Queue<SendItem> sendQueue = new Queue<SendItem>();

		private bool sendThreadRunFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;
		private object sendThreadSyncObj = new object();

		private bool sendingIsOngoing;
		private IOChangedEventHelper ioChangedEventHelper;

		private bool breakState;
		private object breakStateSyncObj = new object();

		private DisplayRepository txRepository;
		private DisplayRepository bidirRepository;
		private DisplayRepository rxRepository;
		private object repositorySyncObj = new object();

		private object clearAndRefreshSyncObj = new object();
		private bool isReloading;

		private System.Timers.Timer periodicXOnTimer;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;

		/// <summary></summary>
		public event EventHandler IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		/// <summary></summary>
		public event EventHandler<EventArgs<RawChunk>> RawChunkSent;

		/// <summary></summary>
		public event EventHandler<EventArgs<RawChunk>> RawChunkReceived;

		/// <summary></summary>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsSent;

		/// <summary></summary>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsReceived;

		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesSent;

		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesReceived;

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
		public Terminal(Settings.TerminalSettings settings)
		{
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

			this.txRepository    = new DisplayRepository(settings.Display.MaxLineCount);
			this.bidirRepository = new DisplayRepository(settings.Display.MaxLineCount);
			this.rxRepository    = new DisplayRepository(settings.Display.MaxLineCount);

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(this.terminalSettings.IO, this.terminalSettings.Buffer));

		////this.isReloading has been initialized to false.

			CreateAndStartSendThread();
		}

		/// <summary></summary>
		public Terminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

			this.txRepository    = new DisplayRepository(terminal.txRepository);
			this.bidirRepository = new DisplayRepository(terminal.bidirRepository);
			this.rxRepository    = new DisplayRepository(terminal.rxRepository);

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(terminal.rawTerminal, this.terminalSettings.IO, this.terminalSettings.Buffer));

			this.isReloading = terminal.isReloading;

			CreateAndStartSendThread();
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			DebugEventManagement.DebugNotifyAllEventRemains(this);

			if (!this.isDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the timer will already have been stopped in Stop()...
					DisposePeriodicXOnTimer();

					// ...and the send thread will already have been stopped in Close()...
					StopSendThread();

					// ...and objects will already have been detached and disposed of in Close():
					DetachTerminalSettings();
					DetachAndDisposeRawTerminal();
					DisposeRepositories();
				}

				// Set state to disposed:
				this.isDisposed = true;
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
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Send Thread
		//------------------------------------------------------------------------------------------
		// Send Thread
		//------------------------------------------------------------------------------------------

		private void CreateAndStartSendThread()
		{
			lock (this.sendThreadSyncObj)
			{
				DebugThreadStateMessage("SendThread() gets created...");

				if (this.sendThread == null)
				{
					this.sendThreadRunFlag = true;
					this.sendThreadEvent = new AutoResetEvent(false);
					this.sendThread = new Thread(new ThreadStart(SendThread));
					this.sendThread.Name = "Terminal [" + (1000 + this.instanceId) + "] Send Thread";
					this.sendThread.Start(); // Offset with 1000 to distinguish this ID from the 'real' terminal ID.

					DebugThreadStateMessage("...successfully created.");
				}
#if (DEBUG)
				else
				{
					DebugThreadStateMessage("...failed as it already exists.");
				}
#endif
			}
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendThreadSafely()
		{
			try
			{
				if (this.sendThreadEvent != null)
					this.sendThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopSendThread()
		{
			lock (this.sendThreadSyncObj)
			{
				if (this.sendThread != null)
				{
					DebugThreadStateMessage("SendThread() gets stopped...");

					this.sendThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.sendThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalSendThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadStateMessage("...failed! Aborting...");
								DebugThreadStateMessage("(Abort is likely required due to failed synchronization back the calling thread, which is typically the GUI/main thread.)");
								this.sendThread.Abort();
								break;
							}

							DebugThreadStateMessage("...trying to join at " + accumulatedTimeout + " ms...");
						}

						DebugThreadStateMessage("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadStateMessage("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.sendThread = null;
				}
#if (DEBUG)
				else
				{
					DebugThreadStateMessage("...not necessary as it doesn't exist anymore.");
				}
#endif
				if (this.sendThreadEvent != null)
				{
					try     { this.sendThreadEvent.Close(); }
					finally { this.sendThreadEvent = null; }
				}
			} // lock (sendThreadSyncObj)
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual Settings.TerminalSettings TerminalSettings
		{
			get
			{
				AssertNotDisposed();

				return (this.terminalSettings);
			}
			set
			{
				AssertNotDisposed();

				AttachTerminalSettings(value);
				ApplyTerminalSettings();
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

				bool result = this.rawTerminal.Start();

				ConfigurePeriodicXOnTimer();

				return (result);
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
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
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
				// is called from the same thread where the ..Sent events get invoked (e.g. the UI
				// thread).

				DisablePeriodicXOnTimer();

				this.rawTerminal.Stop();

				lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
				{
					this.sendQueue.Clear();
				}
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

			StopSendThread();
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParse(string s, out byte[] result, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			using (Parser.Parser p = new Parser.Parser(TerminalSettings.IO.Endianness, TerminalSettings.Send.ToParseMode()))
				return (p.TryParse(s, out result, defaultRadix));
		}

		#endregion

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			// AssertNotDisposed() is called by Send() below.

			DoSend(new RawSendItem(data));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public virtual void Send(string data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by Send() below.

			DoSend(new ParsableSendItem(data, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendLine(string data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by Send() below.

			DoSend(new ParsableSendItem(data, defaultRadix, true));
		}

		/// <remarks>
		/// Required to allow sending multi-line commands in a single operation. Otherwise, using
		/// <see cref="SendLine"/>, sending gets mixed-up because of the following sequence:
		///  1. First line gets sent/enqueued.
		///  2. Second line gets sent/enqueued.
		///  3. Response to first line is received and displayed
		///     and so on, mix-up among sent and received lines...
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendLines(string[] data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by Send() below.

			List<ParsableSendItem> l = new List<ParsableSendItem>(data.Length);
			foreach (string line in data)
				l.Add(new ParsableSendItem(line, defaultRadix, true));

			DoSend(l.ToArray());
		}

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendItem"/> instead.
		/// </remarks>
		protected void DoSend(SendItem item)
		{
			DoSend(new SendItem[] { item });
		}

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendItem"/> instead.
		/// </remarks>
		protected void DoSend(IEnumerable<SendItem> items)
		{
			AssertNotDisposed();

			// Each send request shall resume a pending break condition:
			ResumeBreak();

			if (this.terminalSettings.Send.SignalXOnBeforeEachTransmission)
				RequestSignalInputXOn();

			// Enqueue the items for sending:
			lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
			{
				foreach (SendItem item in items)
					this.sendQueue.Enqueue(item);
			}

			// Signal send thread:
			SignalSendThreadSafely();
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not invoked
		/// on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events. However,
		/// since <see cref="OnDisplayElementProcessed"/> synchronously invokes the event, it will
		/// take some time until the send queue is checked again. During this time, no more new
		/// events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by the DoSend() methods above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendThread()
		{
			DebugThreadStateMessage("SendThread() has started.");

			// Outer loop, processes data after a signal was received:
			while (!IsDisposed && this.sendThreadRunFlag) // Check 'IsDisposed' first!
			{
				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					if (!this.sendThreadEvent.WaitOne(staticRandom.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendThread()!");
					break;
				}

				// Inner loop, runs as long as there is data in the send queue.
				// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
				while (!IsDisposed && this.sendThreadRunFlag && IsReadyToSend_Internal && (this.sendQueue.Count > 0))
				{                                                                      // No lock required, just checking for empty.
					// Initially, yield to other threads before starting to read the queue, since it is very
					// likely that more data is to be enqueued, thus resulting in larger chunks processed.
					// Subsequently, yield to other threads to allow processing the data.
					Thread.Sleep(TimeSpan.Zero);

					SendItem[] pendingItems;
					lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
					{
						pendingItems = this.sendQueue.ToArray();
						this.sendQueue.Clear();
					}

					if (pendingItems.Length > 0)
					{
						this.ioChangedEventHelper.Initialize();
						this.sendingIsOngoing = true;

						foreach (SendItem si in pendingItems)
						{
							DebugMessage(@"Processing item """ + si.ToString() + @""" of " + pendingItems.Length + " send item(s)...");

							ProcessSendItem(si);

							if (this.ioChangedEventHelper.TotalTimeLagIsAboveThreshold())
							{
								// Break if requested or terminal has stopped or closed!
								lock (this.breakStateSyncObj)
								{
									if (this.breakState || !(!IsDisposed && this.sendThreadRunFlag && IsTransmissive)) // Check 'IsDisposed' first!
									{
										this.breakState = false;
										break;
									}
								}
							}
						}

						this.sendingIsOngoing = false;
						if (this.ioChangedEventHelper.EventMustBeRaised)
							OnIOChanged(EventArgs.Empty); // Again raise the event to indicate that
					}                                     //   sending is no longer ongoing.
				} // Inner loop
			} // Outer loop

			DebugThreadStateMessage("SendThread() has terminated.");
		}

		/// <summary></summary>
		protected virtual void ProcessSendItem(SendItem item)
		{
			var rsi = (item as RawSendItem);
			if (rsi != null)
			{
				ProcessRawSendItem(rsi);
			}
			else
			{
				var psi = (item as ParsableSendItem);
				if (psi != null)
					ProcessParsableSendItem(psi);
				else
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + item.GetType() + "' is an invalid send item type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void ProcessRawSendItem(RawSendItem item)
		{
			// Nothing to further process, simply forward:
			ForwardDataToRawTerminal(item.Data);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
		protected virtual void ProcessParsableSendItem(ParsableSendItem item)
		{
			bool hasSucceeded;
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;

			using (Parser.Parser p = new Parser.Parser(TerminalSettings.IO.Endianness, TerminalSettings.Send.ToParseMode()))
				hasSucceeded = p.TryParse(item.Data, out parseResult, out textSuccessfullyParsed, item.DefaultRadix);

			if (hasSucceeded)
				ProcessParserResult(parseResult, item.IsLine);
			else
				OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(item.Data, textSuccessfullyParsed)));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		protected virtual void ProcessParserResult(Parser.Result[] results, bool sendEol = false)
		{
			bool performLineRepeat    = false; // \remind For binary terminals, this is rather a 'PacketRepeat'.
			bool lineRepeatIsInfinite = (TerminalSettings.Send.DefaultLineRepeat == Settings.SendSettings.LineRepeatInfinite);
			int  lineRepeatRemaining  =  TerminalSettings.Send.DefaultLineRepeat;
			bool isFirstRepetition    = true;

			do // Process at least once, potentially repeat.
			{
				// --- Initialize the line/packet ---

				DateTime lineBeginTimeStamp = DateTime.Now; // \remind For binary terminals, this is rather a 'PacketBegin'.
				bool performLineDelay       = false;        // \remind For binary terminals, this is rather a 'PacketDelay'.
				int lineDelay               = TerminalSettings.Send.DefaultLineDelay;
				bool performLineInterval    = false;        // \remind For binary terminals, this is rather a 'PacketInterval'.
				int lineInterval            = TerminalSettings.Send.DefaultLineInterval;

				// --- Process the line/packet ---

				foreach (Parser.Result result in results)
				{
					var byteResult = (result as Parser.BytesResult);
					if (byteResult != null)
					{
						// Raise the 'IOChanged' event if a large chunk is about to be sent:
						if (this.ioChangedEventHelper.RaiseEventIfChunkSizeIsAboveThreshold(byteResult.Bytes.Length))
							OnIOChanged(EventArgs.Empty);

						ForwardDataToRawTerminal(byteResult.Bytes);
					}
					else // if keyword result (will not occur if keywords are disabled while parsing)
					{
						var keywordResult = (result as Parser.KeywordResult);
						if (keywordResult != null)
						{
							switch (keywordResult.Keyword)
							{
								// Process line related keywords:
								case Parser.Keyword.NoEol: // \remind Only needed for text terminals.
								{
									sendEol = false;
									break;
								}

								case Parser.Keyword.LineDelay: // \remind For binary terminals, this is rather a 'PacketDelay'.
								{
									performLineDelay = true;

									if (!ArrayEx.IsNullOrEmpty(keywordResult.Args))
										lineDelay = keywordResult.Args[0];

									break;
								}

								case Parser.Keyword.LineInterval: // \remind For binary terminals, this is rather a 'PacketInterval'.
								{
									performLineInterval = true;

									if (!ArrayEx.IsNullOrEmpty(keywordResult.Args))
										lineInterval = keywordResult.Args[0];

									break;
								}

								case Parser.Keyword.LineRepeat: // \remind For binary terminals, this is rather a 'PacketRepeat'.
								{
									if (isFirstRepetition)
									{
										performLineRepeat = true;

										if (!ArrayEx.IsNullOrEmpty(keywordResult.Args))
										{
											lineRepeatIsInfinite = (keywordResult.Args[0] == Settings.SendSettings.LineRepeatInfinite);
											lineRepeatRemaining  =  keywordResult.Args[0];
										}
									}

									break;
								}

								// Process in-line keywords:
								default:
								{
									ProcessInLineKeywords(keywordResult);
									break;
								}
							}
						}
					}

					// Raise the 'IOChanged' event if sending already takes quite long:
					if (this.ioChangedEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
						OnIOChanged(EventArgs.Empty);
				}

				// --- Finalize the line/packet ---

				ProcessLineEnd(sendEol);

				DateTime lineEndTimeStamp = DateTime.Now; // \remind For binary terminals, this is rather a 'packetEnd'.

				// --- Perform line/packet related post-processing ---

				// Break if requested or terminal has stopped or closed! Note that breaking is
				// done prior to a potential Sleep() or repeat.
				lock (this.breakStateSyncObj)
				{
					if (this.breakState || !(!IsDisposed && this.sendThreadRunFlag && IsTransmissive)) // Check 'IsDisposed' first!
					{
						this.breakState = false;
						break;
					}
				}

				ProcessLineDelayOrInterval(performLineDelay, lineDelay, performLineInterval, lineInterval, lineBeginTimeStamp, lineEndTimeStamp);

				// Process repeat:
				if (!lineRepeatIsInfinite)
				{
					if (lineRepeatRemaining > 0)
						lineRepeatRemaining--;
				}

				isFirstRepetition = false;
			}
			while (performLineRepeat && (lineRepeatIsInfinite || (lineRepeatRemaining > 0)));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		protected virtual void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Clear:
				{
					// Wait some time to allow previous data being transmitted.
					// Wait quite long as the 'DataSent' event will take time.
					// This even has the advantage that data is quickly shown.
					Thread.Sleep(150);

					this.ClearRepositories();
					break;
				}

				case Parser.Keyword.Delay:
				{
					int delay = this.terminalSettings.Send.DefaultDelay;
					if (!ArrayEx.IsNullOrEmpty(result.Args))
						delay = result.Args[0];

					// Raise the 'IOChanged' event if sending is about to be delayed:
					if (this.ioChangedEventHelper.RaiseEventIfDelayIsAboveThreshold(delay))
						OnIOChanged(EventArgs.Empty);

					Thread.Sleep(delay);
					break;
				}

				case Parser.Keyword.OutputBreakOn:
				{
					if (this.terminalSettings.IO.IOType == IOType.SerialPort)
					{
						MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = true;
					}
					else
					{
						OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports."));
					}
					break;
				}

				case Parser.Keyword.OutputBreakOff:
				{
					if (this.terminalSettings.IO.IOType == IOType.SerialPort)
					{
						MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = false;
					}
					else
					{
						OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports."));
					}
					break;
				}

				case Parser.Keyword.OutputBreakToggle:
				{
					if (this.terminalSettings.IO.IOType == IOType.SerialPort)
					{
						MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.ToggleOutputBreak();
					}
					else
					{
						OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports."));
					}
					break;
				}

				case Parser.Keyword.ReportId:
				{
					if (this.terminalSettings.IO.IOType == IOType.UsbSerialHid)
					{
						byte reportId = this.terminalSettings.IO.UsbSerialHidDevice.ReportFormat.Id;
						if (!ArrayEx.IsNullOrEmpty(result.Args))
							reportId = (byte)result.Args[0];

						MKY.IO.Usb.SerialHidDevice device = (MKY.IO.Usb.SerialHidDevice)this.UnderlyingIOInstance;
						device.ActiveReportId = reportId;
					}
					else
					{
						OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Report ID can only be used with USB Ser/HID."));
					}
					break;
				}

				default: // = Unknown or not-yet-supported keyword.
				{
					// Add space if necessary:
					if (ElementsAreSeparate(IODirection.Tx))
						OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.DataSpace());

					OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(MessageHelper.InvalidExecutionPreamble + "The '" + (Parser.KeywordEx)result.Keyword + "' keyword is unknown! " + MessageHelper.SubmitBug));
					break;
				}
			}
		}

		/// <remarks>For binary terminals, this is rather a 'ProcessPacketEnd'.</remarks>
		protected virtual void ProcessLineEnd(bool sendEol)
		{
			// Nothing to do (yet).
		}

		/// <remarks>For binary terminals, this is rather a 'ProcessPacketDelayOrInterval'.</remarks>
		protected virtual int ProcessLineDelayOrInterval(bool performLineDelay, int lineDelay, bool performLineInterval, int lineInterval, DateTime lineBeginTimeStamp, DateTime lineEndTimeStamp)
		{
			int effectiveDelay = 0;

			if (performLineInterval) // 'Interval' has precendence over 'Delay' as it requires more accuracy.
			{
				TimeSpan elapsed = (lineEndTimeStamp - lineBeginTimeStamp);
				effectiveDelay = lineInterval - (int)elapsed.TotalMilliseconds;
			}
			else if (performLineDelay)
			{
				effectiveDelay = lineDelay;
			}

			if (effectiveDelay > 0)
			{
				// Raise the 'IOChanged' event if sending is about to be delayed for too long:
				if (this.ioChangedEventHelper.RaiseEventIfDelayIsAboveThreshold(effectiveDelay))
					OnIOChanged(EventArgs.Empty);

				Thread.Sleep(effectiveDelay);
				return (effectiveDelay);
			}
			else
			{
				return (0);
			}
		}

		/// <summary>
		/// Creates a parser error message which can be displayed in the terminal.
		/// </summary>
		/// <param name="textToParse">The string to be parsed.</param>
		/// <param name="successfullyParsed">The substring that could successfully be parsed.</param>
		/// <returns>The error message to display.</returns>
		protected static string CreateParserErrorMessage(string textToParse, string successfullyParsed)
		{
			var sb = new StringBuilder();

			sb.Append(@"""");
			sb.Append(    textToParse);
			sb.Append(             @"""");
			if (successfullyParsed != null)
			{
				sb.Append(            " is invalid at position ");
				sb.Append(                                    (successfullyParsed.Length + 1).ToString(CultureInfo.InvariantCulture) + ".");
				if (successfullyParsed.Length > 0)
				{
					sb.Append(                                           @" Only """);
					sb.Append(                                                     successfullyParsed);
					sb.Append(                                                                     @""" is valid.");
				}
			}
			else
			{
				sb.Append(            " is invalid.");
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		protected virtual void ForwardDataToRawTerminal(byte[] data)
		{
			AssertNotDisposed();

			try
			{
				this.rawTerminal.Send(data);
			}
			catch (ObjectDisposedException ex)
			{
				var sb = new StringBuilder();
				sb.AppendLine("'ObjectDisposedException' while trying to forward data to the underlying RawTerminal.");
				sb.AppendLine("This exception is ignored as it can happen during closing of the terminal or application.");
				sb.AppendLine();
				DebugEx.WriteException(GetType(), ex, sb.ToString());
			}
			catch (ThreadAbortException ex)
			{
				var sb = new StringBuilder();
				sb.AppendLine("'ThreadAbortException' while trying to forward data to the underlying RawTerminal.");
				sb.AppendLine("This exception is ignored as it can happen during closing of the terminal or application.");
				sb.AppendLine();
				DebugEx.WriteException(GetType(), ex, sb.ToString());
			}
			catch (Exception ex)
			{
				var leadMessage = "Unable to send data:";
				DebugEx.WriteException(GetType(), ex, leadMessage);
				OnIOError(new IOErrorEventArgs(IOErrorSeverity.Fatal, IODirection.Tx, leadMessage + Environment.NewLine + ex.Message));
			}
		}

		#endregion

		#region Methods > Break/Resume
		//------------------------------------------------------------------------------------------
		// Methods > Break/Resume
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Breaks all currently ongoing operations in the terminal.
		/// </summary>
		public virtual void Break()
		{
			lock (this.breakStateSyncObj)
				this.breakState = true;
		}

		/// <summary>
		/// Resumes all currently suspended operations in the terminal.
		/// </summary>
		public virtual void ResumeBreak()
		{
			lock (this.breakStateSyncObj)
				this.breakState = false;
		}

		/// <summary>
		/// Resumes all currently ongoing operations in the terminal.
		/// </summary>
		public virtual bool BreakState
		{
			get
			{
				lock (this.breakStateSyncObj)
					return (this.breakState);
			}
		}

		#endregion

		#region Methods > I/O Control
		//------------------------------------------------------------------------------------------
		// Methods > I/O Control
		//------------------------------------------------------------------------------------------

		private bool IsSerialPort
		{
			get { return ((this.terminalSettings != null) && (this.terminalSettings.IO.IOType == IOType.SerialPort)); }
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
		/// Toggles RFR control pin if current flow control settings allow this.
		/// </summary>
		/// <param name="pinState">
		/// <c>true</c> if the control pin has become enabled.; otherwise, <c>false</c>
		/// </param>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		public virtual bool RequestToggleRfr(out MKY.IO.Serial.SerialPort.SerialControlPinState pinState)
		{
			AssertNotDisposed();

			if (IsSerialPort)
			{
				if (!this.terminalSettings.IO.SerialPort.Communication.FlowControlManagesRfrCtsAutomatically)
				{
					var p = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
					if (p != null)
					{
						if (p.ToggleRfr())
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		public virtual bool RequestToggleDtr(out MKY.IO.Serial.SerialPort.SerialControlPinState pinState)
		{
			AssertNotDisposed();

			if (IsSerialPort)
			{
				if (!this.terminalSettings.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically)
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

			if (this.terminalSettings.IO.FlowControlManagesXOnXOffManually)
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
			if (this.terminalSettings.IO.FlowControlUsesXOnXOff)
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
				if (this.terminalSettings.IO.SerialPortOutputBreakIsModifiable)
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
			if (this.terminalSettings.Send.SignalXOnPeriodically.Enabled)
				EnablePeriodicXOnTimer(this.terminalSettings.Send.SignalXOnPeriodically.Interval);
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
					if (!this.isDisposed && IsReadyToSend_Internal)
						RequestSignalInputXOn();
				}
				finally
				{
					Monitor.Exit(periodicXOnTimer_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
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
				case IODirection.Tx: return (ByteToElement(b, d, this.terminalSettings.Display.TxRadix));
				case IODirection.Rx: return (ByteToElement(b, d, this.terminalSettings.Display.RxRadix));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement ByteToElement(byte b, IODirection d, Radix r)
		{
			bool isByteToHide = false;
			if (b == 0x00)
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

			bool isControl = Ascii.IsControl(b);
			bool error = false;
			string text = "";

			switch (r)
			{
				// String/Char:
				case Radix.String:
				case Radix.Char:
				{
					if (isByteToHide)
					{
						// Do nothing, ignore the character, this results in hiding.
					}
					else if (isControl)
					{
						if (TerminalSettings.CharReplace.ReplaceControlChars)
							text = ByteToControlCharReplacementString(b, TerminalSettings.CharReplace.ControlCharRadix);
						else
							text = ByteToCharacterString(b);
					}
					else if (b == ' ') // Space.
					{
						if (TerminalSettings.CharReplace.ReplaceSpace)
							text = Settings.CharReplaceSettings.SpaceReplaceChar;
						else
							text = " ";
					}
					else
					{
						text = ByteToCharacterString(b);
					}
					break;
				}

				// Bin/Oct/Dec/Hex/Unicode:
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				case Radix.Unicode:
				{
					if (isByteToHide)
					{
						// Do nothing, ignore the character, this results in hiding.
					}
					else if (isControl)
					{
						if (TerminalSettings.CharReplace.ReplaceControlChars)
							text = ByteToControlCharReplacementString(b, TerminalSettings.CharReplace.ControlCharRadix);
						else
							text = ByteToNumericRadixString(b, r); // Current display radix.
					}
					else
					{
						text = ByteToNumericRadixString(b, r); // Current display radix.
					}
					break;
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' radix is missing here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			if (!error)
			{
				if (isByteToHide)
				{
					return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
				}
				else if (isControl)
				{
					if (TerminalSettings.CharReplace.ReplaceControlChars)
					{
						// Attention:
						// In order to get well aligned tab stops, tab characters must be data elements.
						// If they were control elements (i.e. sequence of data and control elements),
						// tabs would only get aligned within the respective control element,
						// thus resulting in misaligned tab stops.
						if ((b == '\t') && !TerminalSettings.CharReplace.ReplaceTab)
						{
							switch (d) // Keep tab if desired:
							{
								case IODirection.Tx: return (new DisplayElement.TxData(b, text));
								case IODirection.Rx: return (new DisplayElement.RxData(b, text));

								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else
						{
							switch (d) // Use dedicated control elements:
							{
								case IODirection.Tx: return (new DisplayElement.TxControl(b, text));
								case IODirection.Rx: return (new DisplayElement.RxControl(b, text));

								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
					}
					else
					{
						switch (d) // Use normal data elements:
						{
							case IODirection.Tx: return (new DisplayElement.TxData(b, text));
							case IODirection.Rx: return (new DisplayElement.RxData(b, text));

							default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
					}
				}
				else
				{
					switch (d)
					{
						case IODirection.Tx: return (new DisplayElement.TxData(b, text));
						case IODirection.Rx: return (new DisplayElement.RxData(b, text));

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
			else
			{
				return (new DisplayElement.ErrorInfo((Direction)d, text));
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
			if ((b == 0x09) && !this.terminalSettings.CharReplace.ReplaceTab)
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
					if (this.terminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToBinaryString(b) + "b");
					else
						return (ByteEx.ConvertToBinaryString(b));
				}
				case Radix.Oct:
				{
					if (this.terminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToOctalString(b) + "o");
					else
						return (ByteEx.ConvertToOctalString(b));
				}
				case Radix.Dec:
				{
					if (this.terminalSettings.Display.ShowRadix)
						return (b.ToString("D3", CultureInfo.InvariantCulture) + "d");
					else
						return (b.ToString("D3", CultureInfo.InvariantCulture));
				}
				case Radix.Hex:
				{
					if (this.terminalSettings.Display.ShowRadix)
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
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is an invalid numeric radix!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		protected virtual string UnicodeValueToNumericString(ushort value)
		{
			if (this.terminalSettings.Display.ShowRadix)
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
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is an invalid ASCII control character radix!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual bool ElementsAreSeparate(IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx: return (ElementsAreSeparate(this.terminalSettings.Display.TxRadix));
				case IODirection.Rx: return (ElementsAreSeparate(this.terminalSettings.Display.RxRadix));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an unknown direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
			}
			throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' radix is missing here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ps", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual void PrepareLineBeginInfo(DateTime ts, string ps, IODirection d, out DisplayLinePart lp)
		{
			if (TerminalSettings.Display.ShowDate || TerminalSettings.Display.ShowTime ||
				TerminalSettings.Display.ShowPort || TerminalSettings.Display.ShowDirection)
			{
				lp = new DisplayLinePart(8); // Preset the required capacity to improve memory management.

				if (TerminalSettings.Display.ShowDate)
				{
					lp.Add(new DisplayElement.DateInfo(ts, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowTime)
				{
					lp.Add(new DisplayElement.TimeInfo(ts, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowPort)
				{
					lp.Add(new DisplayElement.PortInfo(ps, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowDirection)
				{
					lp.Add(new DisplayElement.DirectionInfo((Direction)d, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));

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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void PrepareLineEndInfo(int dataCount, out DisplayLinePart lp)
		{
			if (TerminalSettings.Display.ShowLength)
			{
				lp = new DisplayLinePart(2); // Preset the required capacity to improve memory management.

				if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
					lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));

				lp.Add(new DisplayElement.DataLength(dataCount, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));
			}
			else
			{
				lp = null;
			}
		}

		/// <summary></summary>
		protected virtual void ProcessRawChunk(RawChunk raw, DisplayElementCollection elements, List<DisplayLine> lines)
		{                            // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
			var dl = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			// Line begin:
			dl.Add(new DisplayElement.LineStart((Direction)raw.Direction));

			if (TerminalSettings.Display.ShowDate || TerminalSettings.Display.ShowTime ||
				TerminalSettings.Display.ShowPort || TerminalSettings.Display.ShowDirection)
			{
				DisplayLinePart info;
				PrepareLineBeginInfo(raw.TimeStamp, raw.PortStamp, raw.Direction, out info);
				dl.AddRange(info);
			}

			// Data:
			foreach (byte b in raw.Data)
			{
				dl.Add(ByteToElement(b, raw.Direction));
			}

			// Line end:
			if (TerminalSettings.Display.ShowLength)
			{
				DisplayLinePart info;
				PrepareLineEndInfo(raw.Data.Length, out info);
				dl.AddRange(info);
			}

			dl.Add(new DisplayElement.LineBreak((Direction)raw.Direction));

			elements.AddRange(dl.Clone()); // Clone elements because they are needed again a line below.
			lines.Add(dl);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalRawChunk(RawChunk raw)
		{
			// Collection of elements processed, extends over one or multiple lines,
			// depending on the number of bytes in raw chunk.
			DisplayElementCollection elements = new DisplayElementCollection(); // Default behavior regarding initial capacity is OK.
			List<DisplayLine> lines = new List<DisplayLine>();

			ProcessRawChunk(raw, elements, lines);

			if (elements.Count > 0)
			{
				OnDisplayElementsProcessed(raw.Direction, elements);

				if (lines.Count > 0)
				{
					OnDisplayLinesProcessed(raw.Direction, lines);
				}
			}
		}

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool ClearRepository(RepositoryType repository)
		{
			AssertNotDisposed();

			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{                                     // Only try for some time, otherwise ignore.
				try                               // Prevents deadlocks among main thread (view)
				{                                 //   and large amounts of incoming data.
					this.rawTerminal.ClearRepository(repository);
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
		public virtual bool RefreshRepository(RepositoryType repository)
		{
			AssertNotDisposed();

			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{                                     // Only try for some time, otherwise ignore.
				try                               // Prevents deadlocks among main thread (view)
				{                                 //   and large amounts of incoming data.
					// Clear repository:
					ClearMyRepository(repository);
					OnRepositoryCleared(new EventArgs<RepositoryType>(repository));

					// Reload repository:
					this.isReloading = true;
					foreach (RawChunk rawChunk in this.rawTerminal.RepositoryToChunks(repository))
					{
						ProcessAndSignalRawChunk(rawChunk);
					}
					this.isReloading = false;
					OnRepositoryReloaded(new EventArgs<RepositoryType>(repository));
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
		/// Therefore, decided to keep the implementation synchonous until new issues pop up.
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
					foreach (RawChunk rawChunk in this.rawTerminal.RepositoryToChunks(RepositoryType.Bidir))
					{
						ProcessAndSignalRawChunk(rawChunk);
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
		protected virtual void ClearMyRepository(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    this.txRepository   .Clear(); break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository   .Clear(); break;
					default: throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual int GetRepositoryDataCount(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository   .DataCount);
					case RepositoryType.Bidir: return (this.bidirRepository.DataCount);
					case RepositoryType.Rx:    return (this.rxRepository   .DataCount);
				}
				throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual int GetRepositoryLineCount(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository   .Count);
					case RepositoryType.Bidir: return (this.bidirRepository.Count);
					case RepositoryType.Rx:    return (this.rxRepository   .Count);
				}
				throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual List<DisplayElement> RepositoryToDisplayElements(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToElements());
					case RepositoryType.Bidir: return (this.bidirRepository.ToElements());
					case RepositoryType.Rx:    return (this.rxRepository   .ToElements());
				}
				throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual List<DisplayLine> RepositoryToDisplayLines(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository.   ToLines());
					case RepositoryType.Bidir: return (this.bidirRepository.ToLines());
					case RepositoryType.Rx:    return (this.rxRepository   .ToLines());
				}
				throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual DisplayLine LastDisplayLineAuxiliary(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository.   LastLineAuxiliary());
					case RepositoryType.Bidir: return (this.bidirRepository.LastLineAuxiliary());
					case RepositoryType.Rx:    return (this.rxRepository   .LastLineAuxiliary());
				}
				throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual void ClearLastDisplayLineAuxiliary(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    this.txRepository.   ClearLastLineAuxiliary(); break;
					case RepositoryType.Bidir: this.bidirRepository.ClearLastLineAuxiliary(); break;
					case RepositoryType.Rx:    this.rxRepository   .ClearLastLineAuxiliary(); break;
					default: throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repository)
		{
			return (RepositoryToString(repository, ""));
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repository, string indent)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToString(indent));
					case RepositoryType.Bidir: return (this.bidirRepository.ToString(indent));
					case RepositoryType.Rx:    return (this.rxRepository   .ToString(indent));
				}
				throw (new ArgumentOutOfRangeException("repository", repository, MessageHelper.InvalidExecutionPreamble + "'" + repository + "' is an invalid repository!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual List<RawChunk> RepositoryToRawChunks(RepositoryType repository)
		{
			AssertNotDisposed();

			return (this.rawTerminal.RepositoryToChunks(repository));
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

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

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
		//==========================================================================================
		// Settings Events
		//==========================================================================================
		
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
		//==========================================================================================
		// Raw Terminal
		//==========================================================================================

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
		//==========================================================================================
		// Raw Terminal Events
		//==========================================================================================

		private void rawTerminal_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void rawTerminal_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void rawTerminal_IOError(object sender, IOErrorEventArgs e)
		{
			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until reloading has completed.
			{
				SerialPortErrorEventArgs serialPortErrorEventArgs = (e as SerialPortErrorEventArgs);
				if (serialPortErrorEventArgs != null)
				{
					// Handle serial port errors whenever possible.
					switch (serialPortErrorEventArgs.SerialPortError)
					{
						case System.IO.Ports.SerialError.Frame:    OnDisplayElementProcessed(IODirection.Rx, new DisplayElement.ErrorInfo(RxFramingErrorString));        break;
						case System.IO.Ports.SerialError.Overrun:  OnDisplayElementProcessed(IODirection.Rx, new DisplayElement.ErrorInfo(RxBufferOverrunErrorString));  break;
						case System.IO.Ports.SerialError.RXOver:   OnDisplayElementProcessed(IODirection.Rx, new DisplayElement.ErrorInfo(RxBufferOverflowErrorString)); break;
						case System.IO.Ports.SerialError.RXParity: OnDisplayElementProcessed(IODirection.Rx, new DisplayElement.ErrorInfo(RxParityErrorString));         break;
						case System.IO.Ports.SerialError.TXFull:   OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(TxBufferFullErrorString));     break;
						default:                                   OnIOError(e); break;
					}
				}
				else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Rx)) // Acceptable errors are only shown as terminal text.
				{
					OnDisplayElementProcessed(IODirection.Rx, new DisplayElement.ErrorInfo(Direction.Rx, e.Message, true));
				}
				else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Tx)) // Acceptable errors are only shown as terminal text.
				{
					OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, e.Message, true));
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
			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until reloading has completed.
			{
				OnRawChunkSent(e);
				ProcessAndSignalRawChunk(e.Value);
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "RawTerminal.RawChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void rawTerminal_RawChunkReceived(object sender, EventArgs<RawChunk> e)
		{
			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until reloading has completed.
			{
				OnRawChunkReceived(e);
				ProcessAndSignalRawChunk(e.Value);
			}
		}

		private void rawTerminal_RepositoryCleared(object sender, EventArgs<RepositoryType> e)
		{
			lock (this.clearAndRefreshSyncObj) // Delay processing new raw data until reloading has completed.
			{
				ClearMyRepository(e.Value);
				OnRepositoryCleared(e);
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			EventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			EventHelper.FireSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawChunkSent(EventArgs<RawChunk> e)
		{
			EventHelper.FireSync<EventArgs<RawChunk>>(RawChunkSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawChunkReceived(EventArgs<RawChunk> e)
		{
			EventHelper.FireSync<EventArgs<RawChunk>>(RawChunkReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementProcessed(IODirection direction, DisplayElement element)
		{
			DisplayElementCollection elements = new DisplayElementCollection(1); // Preset the required capacity to improve memory management.
			elements.Add(element); // No clone needed as the element must be created when calling this event method.
			OnDisplayElementsProcessed(direction, elements);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsProcessed(IODirection direction, DisplayElementCollection elements)
		{
			switch (direction)
			{
				case IODirection.Tx:
				{
					lock (this.repositorySyncObj)
					{
						this.txRepository   .Enqueue(elements.Clone()); // Clone elements because they are needed again below.
						this.bidirRepository.Enqueue(elements.Clone()); // Clone elements because they are needed again below.
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
						OnDisplayElementsSent(new DisplayElementsEventArgs(elements)); // No clone needed as the elements must be created when calling this event method.

					break;
				}

				case IODirection.Rx:
				{
					lock (this.repositorySyncObj)
					{
						this.bidirRepository.Enqueue(elements.Clone()); // Clone elements because they are needed again below.
						this.rxRepository   .Enqueue(elements.Clone()); // Clone elements because they are needed again below.
					}

					if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
						OnDisplayElementsReceived(new DisplayElementsEventArgs(elements)); // No clone needed as the elements must be created when calling this event method.

					break;
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(DisplayElementsEventArgs e)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
				EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(DisplayElementsEventArgs e)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
				EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual void OnDisplayLinesProcessed(IODirection d, List<DisplayLine> lines)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
			{
				switch (d)
				{
					case IODirection.Tx: OnDisplayLinesSent    (new DisplayLinesEventArgs(lines)); break;
					case IODirection.Rx: OnDisplayLinesReceived(new DisplayLinesEventArgs(lines)); break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesSent(DisplayLinesEventArgs e)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
				EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(DisplayLinesEventArgs e)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
				EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(EventArgs<RepositoryType> e)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
				EventHelper.FireSync<EventArgs<RepositoryType>>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(EventArgs<RepositoryType> e)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be invoked after completion.
				EventHelper.FireSync<EventArgs<RepositoryType>>(RepositoryReloaded, this, e);
		}

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
			AssertNotDisposed();

			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			AssertNotDisposed();

			var sb = new StringBuilder();
			lock (this.repositorySyncObj)
			{
				sb.AppendLine(indent + "> Settings: " + this.terminalSettings);

				sb.AppendLine(indent + "> RawTerminal: ");
				sb.AppendLine(this.rawTerminal.ToString(indent + "   "));

				sb.AppendLine(indent + "> TxRepository: ");
				sb.Append(this.txRepository.ToString(indent + "   ")); // Repository will add 'NewLine'.

				sb.AppendLine(indent + "> BidirRepository: ");
				sb.Append(this.txRepository.ToString(indent + "   ")); // Repository will add 'NewLine'.

				sb.AppendLine(indent + "> RxRepository: ");
				sb.Append(this.txRepository.ToString(indent + "   ")); // Repository will add 'NewLine'.
			}
			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortIOString()
		{
			if (this.rawTerminal != null)
				return (this.rawTerminal.ToShortIOString());
			else
				return (Undefined);
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
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					"#" + this.instanceId.ToString("D2", CultureInfo.InvariantCulture),
					"[" + ToShortIOString() + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_THREAD_STATE")]
		private void DebugThreadStateMessage(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
