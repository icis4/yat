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
// YAT Version 2.3.90 Development
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
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public abstract class Terminal : IDisposable, IDisposableEx
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

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...
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

		private Queue<DataSendItem> sendDataQueue = new Queue<DataSendItem>();
		private bool sendDataThreadRunFlag;
		private AutoResetEvent sendDataThreadEvent;
		private Thread sendDataThread;
		private object sendDataThreadSyncObj = new object();

		private Queue<FileSendItem> sendFileQueue = new Queue<FileSendItem>();
		private bool sendFileThreadRunFlag;
		private AutoResetEvent sendFileThreadEvent;
		private Thread sendFileThread;
		private object sendFileThreadSyncObj = new object();

		private bool sendingIsOngoing;
		private IOChangedEventHelper ioChangedEventHelper;

		private bool breakState;
		private object breakStateSyncObj = new object();

		private IOControlState ioControlStateCache;
		private object ioControlStateCacheSyncObj = new object();

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
		public event EventHandler<IOControlEventArgs> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

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

		#region Send Threads
		//------------------------------------------------------------------------------------------
		// Send Threads
		//------------------------------------------------------------------------------------------

		private void CreateAndStartSendThreads()
		{
			lock (this.sendDataThreadSyncObj)
			{
				DebugThreadState("SendDataThread() gets created...");

				if (this.sendDataThread == null)
				{
					this.sendDataThreadRunFlag = true;
					this.sendDataThreadEvent = new AutoResetEvent(false);
					this.sendDataThread = new Thread(new ThreadStart(SendDataThread));
					this.sendDataThread.Name = "Terminal [" + (1000 + this.instanceId) + "] Send Data Thread";
					this.sendDataThread.Start();  // Offset of 1000 to distinguish this ID from the 'real' terminal ID.

					DebugThreadState("...successfully created.");
				}
			#if (DEBUG)
				else
				{
					DebugThreadState("...failed as it already exists.");
				}
			#endif
			}

			lock (this.sendFileThreadSyncObj)
			{
				DebugThreadState("SendFileThread() gets created...");

				if (this.sendFileThread == null)
				{
					this.sendFileThreadRunFlag = true;
					this.sendFileThreadEvent = new AutoResetEvent(false);
					this.sendFileThread = new Thread(new ThreadStart(SendFileThread));
					this.sendFileThread.Name = "Terminal [" + (1000 + this.instanceId) + "] Send File Thread";
					this.sendFileThread.Start();  // Offset of 1000 to distinguish this ID from the 'real' terminal ID.

					DebugThreadState("...successfully created.");
				}
			#if (DEBUG)
				else
				{
					DebugThreadState("...failed as it already exists.");
				}
			#endif
			}
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopSendThreads()
		{
			lock (this.sendFileThreadSyncObj)
			{
				if (this.sendFileThread != null)
				{
					DebugThreadState("SendFileThread() gets stopped...");

					this.sendFileThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.sendFileThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendFileThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalSendFileThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.sendFileThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreadState("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreadState("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadState("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.sendFileThread = null;
				}
			#if (DEBUG)
				else // (this.sendFileThread == null)
				{
					DebugThreadState("...not necessary as it doesn't exist anymore.");
				}
			#endif

				if (this.sendFileThreadEvent != null)
				{
					try     { this.sendFileThreadEvent.Close(); }
					finally { this.sendFileThreadEvent = null; }
				}
			} // lock (sendFileThreadSyncObj)

			lock (this.sendDataThreadSyncObj)
			{
				if (this.sendDataThread != null)
				{
					DebugThreadState("SendDataThread() gets stopped...");

					this.sendDataThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.sendDataThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendDataThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalSendDataThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.sendDataThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreadState("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreadState("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadState("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.sendDataThread = null;
				}
			#if (DEBUG)
				else // (this.sendDataThread == null)
				{
					DebugThreadState("...not necessary as it doesn't exist anymore.");
				}
			#endif

				if (this.sendDataThreadEvent != null)
				{
					try     { this.sendDataThreadEvent.Close(); }
					finally { this.sendDataThreadEvent = null; }
				}
			} // lock (sendDataThreadSyncObj)
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendDataThreadSafely()
		{
			try
			{
				if (this.sendDataThreadEvent != null)
					this.sendDataThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendFileThreadSafely()
		{
			try
			{
				if (this.sendFileThreadEvent != null)
					this.sendFileThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

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

		#region Methods > Send Data
		//------------------------------------------------------------------------------------------
		// Methods > Send Data
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			// AssertNotDisposed() is called by DoSendData().

			DoSendData(new RawDataSendItem(data));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendText(string data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.Text.ToParseMode();

			DoSendData(new TextDataSendItem(data, defaultRadix, parseMode, SendMode.Text, false));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLine(string dataLine, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.Text.ToParseMode();

			DoSendData(new TextDataSendItem(dataLine, defaultRadix, parseMode, SendMode.Text, true));
		}

		/// <remarks>
		/// Required to allow sending multi-line commands in a single operation. Otherwise, using
		/// <see cref="SendTextLine"/>, sending gets mixed-up because of the following sequence:
		///  1. First line gets sent/enqueued.
		///  2. Second line gets sent/enqueued.
		///  3. Response to first line is received and displayed
		///     and so on, mix-up among sent and received lines...
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLines(string[] dataLines, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.Text.ToParseMode();

			var l = new List<TextDataSendItem>(dataLines.Length); // Preset the required capacity to improve memory management.
			foreach (string dataLine in dataLines)
				l.Add(new TextDataSendItem(dataLine, defaultRadix, parseMode, SendMode.Text, true));

			DoSendData(l.ToArray());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public abstract void SendFileLine(string dataLine, Radix defaultRadix = Parser.Parser.DefaultRadixDefault);

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendDataItem"/> instead.
		/// </remarks>
		protected void DoSendData(DataSendItem item)
		{
			DoSendData(new DataSendItem[] { item });
		}

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendDataItem"/> instead.
		/// </remarks>
		protected void DoSendData(IEnumerable<DataSendItem> items)
		{
			AssertNotDisposed();

			// Each send request shall resume a pending break condition:
			ResumeBreak();

			if (TerminalSettings.Send.SignalXOnBeforeEachTransmission)
				RequestSignalInputXOn();

			// Enqueue the items for sending:
			lock (this.sendDataQueue) // Lock is required because Queue<T> is not synchronized.
			{
				foreach (var item in items)
					this.sendDataQueue.Enqueue(item);
			}

			// Signal thread:
			SignalSendDataThreadSafely();
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not invoked
		/// on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events. However,
		/// since <see cref="OnDisplayElementAdded"/> synchronously invokes the event, it will
		/// take some time until the send queue is checked again. During this time, no more new
		/// events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="DoSendData(IEnumerable{DataSendItem})"/> above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendDataThread()
		{
			DebugThreadState("SendDataThread() has started.");

			try
			{
				// Outer loop, processes data after a signal was received:
				while (!IsDisposed && this.sendDataThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.sendDataThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendDataThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the send queue.
					// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.sendDataThreadRunFlag && IsReadyToSend_Internal && (this.sendDataQueue.Count > 0))
					{                                                                          // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						DataSendItem[] pendingItems;
						lock (this.sendDataQueue) // Lock is required because Queue<T> is not synchronized.
						{
							pendingItems = this.sendDataQueue.ToArray();
							this.sendDataQueue.Clear();
						}

						if (pendingItems.Length > 0)
						{
							this.ioChangedEventHelper.Initialize();
							this.sendingIsOngoing = true;

							foreach (var item in pendingItems)
							{
								DebugMessage(@"Processing item """ + item.ToString() + @""" of " + pendingItems.Length + " send item(s)...");

								ProcessSendDataItem(item);

								if (BreakSendData)
								{
									if (this.ioChangedEventHelper.EventMustBeRaised)
										OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.

									break;
								}

								// \remind (2017-09-16 / MKY) related to FR #262 "IIOProvider should be..."
								// In case of many pending items, 'EventMustBeRaised' will become 'true',
								// e.g. due to RaiseEventIfTotalTimeLagIsAboveThreshold(). This indicates
								// that there are really many pending items, and this foreach-loop would
								// result in kind of freezing all other threads => Yield!
								if (this.ioChangedEventHelper.EventMustBeRaised)
									Thread.Sleep(1); // Yield to other threads to e.g. allow refreshing of view.
							}                        // Note that Thread.Sleep(TimeSpan.Zero) is not sufficient.

							this.sendingIsOngoing = false;
							if (this.ioChangedEventHelper.EventMustBeRaised)
								OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendDataThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("SendDataThread() has terminated.");
		}

		/// <remarks>
		/// Break if requested or terminal has stopped or closed.
		/// </remarks>
		protected virtual bool BreakSendData
		{
			get
			{
				return (BreakState || !(!IsDisposed && this.sendDataThreadRunFlag && IsTransmissive)); // Check 'IsDisposed' first!
			}
		}

		/// <summary></summary>
		protected virtual void ProcessSendDataItem(DataSendItem item)
		{
			var rsi = (item as RawDataSendItem);
			if (rsi != null)
			{
				ProcessRawDataSendItem(rsi);
			}
			else
			{
				var psi = (item as TextDataSendItem);
				if (psi != null)
					ProcessTextDataSendItem(psi);
				else
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + item.GetType() + "' is a send item type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void ProcessRawDataSendItem(RawDataSendItem item)
		{
			ForwardDataToRawTerminal(item.Data); // Nothing for further processing, simply forward.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
		protected virtual void ProcessTextDataSendItem(TextDataSendItem item)
		{
			bool hasSucceeded;
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;

			using (var p = new Parser.Parser(TerminalSettings.IO.Endianness, item.ParseMode))
				hasSucceeded = p.TryParse(item.Data, out parseResult, out textSuccessfullyParsed, item.DefaultRadix);

			if (hasSucceeded)
				ProcessParserResult(parseResult, item.IsLine);
			else
				OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(item.Data, textSuccessfullyParsed)));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
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

				foreach (var result in results)
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

									if (!ArrayEx.IsNullOrEmpty(keywordResult.Args)) // If arg is non-existant, the default value will be used.
										lineDelay = keywordResult.Args[0];

									break;
								}

								case Parser.Keyword.LineInterval: // \remind For binary terminals, this is rather a 'PacketInterval'.
								{
									performLineInterval = true;

									if (!ArrayEx.IsNullOrEmpty(keywordResult.Args)) // If arg is non-existant, the default value will be used.
										lineInterval = keywordResult.Args[0];

									break;
								}

							////case Parser.Keyword.Repeat: is yet pending (FR #13) and requires parser support for strings.
							////{
							////}

								case Parser.Keyword.LineRepeat: // \remind For binary terminals, this is rather a 'PacketRepeat'.
								{
									if (isFirstRepetition)
									{
										performLineRepeat = true;

										if (!ArrayEx.IsNullOrEmpty(keywordResult.Args)) // If arg is non-existant, the default value will be used.
										{
											lineRepeatIsInfinite = (keywordResult.Args[0] == Settings.SendSettings.LineRepeatInfinite);
											lineRepeatRemaining  =  keywordResult.Args[0];
										}
									}
									else
									{
										Thread.Sleep(TimeSpan.Zero); // Make sure the application stays responsive while repeating.
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

				DateTime lineEndTimeStamp = DateTime.Now; // \remind For binary terminals, this is rather a 'packetEndTimeStamp'.

				// --- Perform line/packet related post-processing ---

				// Break if requested or terminal has stopped or closed!
				// Note that breaking is done prior to a potential Sleep() or repeat.
				if (BreakState || !(!IsDisposed && this.sendDataThreadRunFlag && IsTransmissive)) // Check 'IsDisposed' first!
					break;

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
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InLine", Justification = "It's 'in line' and not inline!")]
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Agreed, could be refactored. Could be.")]
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
					int delay = TerminalSettings.Send.DefaultDelay;
					if (!ArrayEx.IsNullOrEmpty(result.Args))
						delay = result.Args[0];

					// Raise the 'IOChanged' event if sending is about to be delayed:
					if (this.ioChangedEventHelper.RaiseEventIfDelayIsAboveThreshold(delay))
						OnIOChanged(EventArgs.Empty);

					Thread.Sleep(delay);
					break;
				}

				case Parser.Keyword.PortSettings:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							if (result.Args.Length > 0)
							{
								MKY.IO.Ports.BaudRateEx baudRate;
								if (MKY.IO.Ports.BaudRateEx.TryFrom(result.Args[0], out baudRate))
									setting.Communication.BaudRate = baudRate;

								if (result.Args.Length > 1)
								{
									MKY.IO.Ports.DataBitsEx dataBits;
									if (MKY.IO.Ports.DataBitsEx.TryFrom(result.Args[1], out dataBits))
										setting.Communication.DataBits = dataBits;

									if (result.Args.Length > 2)
									{
										MKY.IO.Ports.ParityEx parity;
										if (MKY.IO.Ports.ParityEx.TryFrom(result.Args[2], out parity))
											setting.Communication.Parity = parity;

										if (result.Args.Length > 3)
										{
											MKY.IO.Ports.StopBitsEx stopBits;   // 1.5 is not (yet) supported as the keyword args are limited to int.
											if (MKY.IO.Ports.StopBitsEx.TryFrom(result.Args[3], out stopBits))
												setting.Communication.StopBits = stopBits;

											if (result.Args.Length > 4)
											{
												MKY.IO.Serial.SerialPort.SerialFlowControlEx flowControl;
												if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryFrom(result.Args[4], out flowControl))
													setting.Communication.FlowControl = flowControl;
											}
										}
									}
								}
							}
						}

						if (setting.Communication.HaveChanged)
						{
							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								OnDisplayElementAdded(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing port settings has failed! " + ex.Message));
						}
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Changing port settings is yet limited to serial COM ports (limitation #71).", true));
					}
					break;
				}

				case Parser.Keyword.Baud:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.BaudRateEx baudRate;
							if (MKY.IO.Ports.BaudRateEx.TryFrom(result.Args[0], out baudRate))
								setting.Communication.BaudRate = baudRate;
						}

						if (setting.Communication.HaveChanged)
						{
							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								OnDisplayElementAdded(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing baud rate has failed! " + ex.Message));
						}
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Baud rate can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.StopBits:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.StopBitsEx stopBits;
							if (MKY.IO.Ports.StopBitsEx.TryFrom(result.Args[0], out stopBits))
								setting.Communication.StopBits = stopBits;
						}

						if (setting.Communication.HaveChanged)
						{
							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								OnDisplayElementAdded(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing stop bits has failed! " + ex.Message));
						}
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Stop bits can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.DataBits:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.DataBitsEx dataBits;
							if (MKY.IO.Ports.DataBitsEx.TryFrom(result.Args[0], out dataBits))
								setting.Communication.DataBits = dataBits;
						}

						if (setting.Communication.HaveChanged)
						{
							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								OnDisplayElementAdded(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing data bits has failed! " + ex.Message));
						}
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Data bits can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.Parity:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.ParityEx parity;
							if (MKY.IO.Ports.ParityEx.TryFrom(result.Args[0], out parity))
								setting.Communication.Parity = parity;
						}

						if (setting.Communication.HaveChanged)
						{
							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								OnDisplayElementAdded(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing parity has failed! " + ex.Message));
						}
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Parity can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FlowControl:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Serial.SerialPort.SerialFlowControlEx flowControl;
							if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryFrom(result.Args[0], out flowControl))
								setting.Communication.FlowControl = flowControl;
						}

						if (setting.Communication.HaveChanged)
						{
							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								OnDisplayElementAdded(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing flow control has failed! " + ex.Message));
						}
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Flow control can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FramingErrorsOn:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = false;
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FramingErrorsOff:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = true;
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FramingErrorsRestore:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = TerminalSettings.IO.SerialPort.IgnoreFramingErrors;
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.OutputBreakOn:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = true;
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.OutputBreakOff:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = false;
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.OutputBreakToggle:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.ToggleOutputBreak();
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.ReportId:
				{
					if (TerminalSettings.IO.IOType == IOType.UsbSerialHid)
					{
						byte reportId = TerminalSettings.IO.UsbSerialHidDevice.ReportFormat.Id;
						if (!ArrayEx.IsNullOrEmpty(result.Args))
							reportId = (byte)result.Args[0];

						var device = (MKY.IO.Usb.SerialHidDevice)this.UnderlyingIOInstance;
						device.ActiveReportId = reportId;
					}
					else
					{
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Report ID can only be used with USB Ser/HID.", true));
					}
					break;
				}

				default: // = Unknown or not-yet-supported keyword.
				{
					// Add space if necessary:
					if (ElementsAreSeparate(IODirection.Tx))
						OnDisplayElementAdded(IODirection.Tx, new DisplayElement.DataSpace());

					OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, MessageHelper.InvalidExecutionPreamble + "The '" + (Parser.KeywordEx)result.Keyword + "' keyword is unknown! " + MessageHelper.SubmitBug));
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
				var elapsed = (lineEndTimeStamp - lineBeginTimeStamp);
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
				sb.Append(                                    (successfullyParsed.Length + 1).ToString(CultureInfo.CurrentCulture) + ".");
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual void ForwardDataToRawTerminal(byte[] data)
		{
			try
			{
				this.rawTerminal.Send(data);
			}
			catch (ThreadAbortException ex)
			{
				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				var sb = new StringBuilder();
				sb.AppendLine("'ThreadAbortException' while trying to forward data to the underlying RawTerminal.");
				sb.AppendLine("This exception is ignored as it can happen during closing of the terminal or application.");
				sb.AppendLine("Confirming the abort, i.e. Thread.ResetAbort() will be called...");
				sb.AppendLine();
				DebugEx.WriteException(GetType(), ex, sb.ToString());

				Thread.ResetAbort();
			}
			catch (ObjectDisposedException ex)
			{
				var sb = new StringBuilder();
				sb.AppendLine("'ObjectDisposedException' while trying to forward data to the underlying RawTerminal.");
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual bool TryApplySettings(MKY.IO.Serial.SerialPort.SerialPort port, MKY.IO.Serial.SerialPort.SerialPortSettings settings, out Exception exception)
		{
			try
			{
				// Attention:
				// Similar code exists in Model.Terminal.ApplySettings() but including change of terminal settings (.yat file).
				// Changes here may have to be applied there too.

				if (port.IsStarted) // Port is started, stop and restart it with the new settings:
				{
					port.Stop(); // Attention, do not Stop() the whole terminal as that will also stop the currently ongoing send thread!
					port.Settings = settings;
					port.Start();
				}
				else // Port is stopped, simply set the new settings:
				{
					port.Settings = settings;
				}

				exception = null;
				return (true);
			}
			catch (Exception ex)
			{
				exception = ex;
				return (false);
			}
		}

		#endregion

		#region Methods > Send File
		//------------------------------------------------------------------------------------------
		// Methods > Send File
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendFile(string filePath, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendFile().

			DoSendFile(filePath, defaultRadix);
		}

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendFileItem"/> instead.
		/// </remarks>
		/// <remarks>
		/// Separate "Do...()" method for symmetricity with <see cref="DoSendData(IEnumerable{DataSendItem})"/>.
		/// </remarks>
		protected void DoSendFile(string filePath, Radix defaultRadix)
		{
			AssertNotDisposed();

			// Enqueue the items for sending:
			lock (this.sendFileQueue) // Lock is required because Queue<T> is not synchronized.
			{
				this.sendFileQueue.Enqueue(new FileSendItem(filePath, defaultRadix));
			}

			// Signal thread:
			SignalSendFileThreadSafely();
		}

		/// <remarks>
		/// Will be signaled by <see cref="DoSendFile(string, Radix)"/> above.
		/// </remarks>
		/// <remarks>
		/// Separate thread (and not integrated into <see cref="SendDataThread"/>) because that
		/// thread queues <see cref="TextDataSendItem"/> objects, thus some kind of a two-level
		/// infrastructure is required (SendFile => SendData). The considered \!(SendFile("..."))
		/// keyword doesn't help either since the file may again contain keywords, thus again some
		/// kind of a two-level infrastructure is required.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendFileThread()
		{
			DebugThreadState("SendFileThread() has started.");

			try
			{
				// Outer loop, processes data after a signal was received:
				while (!IsDisposed && this.sendFileThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.sendFileThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendFileThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the send queue.
					// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.sendFileThreadRunFlag && IsReadyToSend_Internal && (this.sendFileQueue.Count > 0))
					{                                                                      // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						FileSendItem[] pendingItems;
						lock (this.sendFileQueue) // Lock is required because Queue<T> is not synchronized.
						{
							pendingItems = this.sendFileQueue.ToArray();
							this.sendFileQueue.Clear();
						}

						if (pendingItems.Length > 0)
						{
							foreach (var item in pendingItems)
							{
								DebugMessage(@"Processing item """ + item.ToString() + @""" of " + pendingItems.Length + " send item(s)...");

								ProcessSendFileItem(item);

								if (BreakSendFile)
								{
									OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
									break;
								}

								// \remind (2017-09-16 / MKY) related to FR #262 "IIOProvider should be..."
								// No need to yield here (like done in SendDataThread()) since...
								// ...it is very unlikely that very many files are sent at once.
								// ...and the for-loop in ProcessSendFileItem() already yields.
							}
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendFileThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("SendFileThread() has terminated.");
		}

		/// <remarks>
		/// Break if requested or terminal has stopped or closed.
		/// </remarks>
		protected virtual bool BreakSendFile
		{
			get
			{
				return (BreakState || !(!IsDisposed && this.sendFileThreadRunFlag && IsTransmissive)); // Check 'IsDisposed' first!
			}
		}

		/// <summary></summary>
		protected abstract void ProcessSendFileItem(FileSendItem item);

		/// <summary></summary>
		protected virtual void ProcessSendTextFileItem(FileSendItem item)
		{
			ProcessSendTextFileItem(item, Encoding.Default);
		}

		/// <summary></summary>
		protected virtual void ProcessSendTextFileItem(FileSendItem item, Encoding encodingFallback)
		{
			using (var sr = new StreamReader(item.FilePath, encodingFallback, true))
			{                             // Automatically detect encoding from BOM, otherwise use fallback.
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
						continue;

					SendFileLine(line, item.DefaultRadix);

					if (BreakSendFile)
					{
						OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
						break;
					}

					Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
				}
			}
		}

		/// <summary></summary>
		protected virtual void ProcessSendXmlFileItem(FileSendItem item)
		{
			string[] lines;
			XmlReaderHelper.LinesFromFile(item.FilePath, out lines); // Read all at once for simplicity.
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
					continue;

				SendFileLine(line, item.DefaultRadix);

				if (BreakSendFile)
				{
					OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
					break;
				}

				Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
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
		/// Returns the current break state.
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' radix is missing here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ps", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual void PrepareLineBeginInfo(DateTime ts, TimeSpan diff, TimeSpan delta, string ps, IODirection d, out DisplayElementCollection lp)
		{
			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowPort      ||
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		private void AddSpaceIfNecessary(IODirection d, DisplayElementCollection lp, DisplayElement de)
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
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnIOChanged(e);
		}

		private void rawTerminal_IOControlChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalSettings.Display.IncludePortControl)
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
		//==========================================================================================
		// Event Raising
		//==========================================================================================

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
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual void OnDisplayLinesAdded(IODirection d, DisplayLineCollection lines)
		{
			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
			{
				switch (d)
				{
					case IODirection.Tx: OnDisplayLinesSent    (new DisplayLinesEventArgs(lines)); break;
					case IODirection.Rx: OnDisplayLinesReceived(new DisplayLinesEventArgs(lines)); break;

					case IODirection.Bidir:
					case IODirection.None:
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

					default:
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
