﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

//==================================================================================================
// Configuration
//==================================================================================================

// Choose whether SerialPort should automatically detect and handle live disconnects/reconnects:
// - Uncomment to enable
// - Comment out to disable
//
// \fixme:
// Auto-reopen doesn't work because of deadlock issue mentioned below.
//#define DETECT_BREAKS_AND_TRY_AUTO_REOPEN

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Event;

#endregion

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\SerialPort for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	/// <remarks>
	/// There is a serious deadlock issue in <see cref="System.IO.Ports.SerialPort"/>.
	/// Google for [UnauthorizedAccessException "Access to the port"] for more information and
	/// work-arounds suggestions.
	/// 
	/// ============================================================================================
	/// Source: http://msdn.microsoft.com/en-us/library/system.io.ports.serialport_methods.aspx
	/// Author: Dan Randolph
	/// 
	/// There is a deadlock problem with the internal close operation of
	/// <see cref="System.IO.Ports.SerialPort"/>. Use BeginInvoke instead of Invoke from the
	/// serialPort_DataReceived event handler to start the method that reads from the
	/// SerialPort buffer and it will solve the problem. I finally tracked down the problem
	/// to the Close method by putting a start/stop button on the form. Then I was able to
	/// lock up the application and found that Close was the culpret. I'm pretty sure that
	/// components.Dispose() will end up calling the SerialPort Close method if it is open.
	/// 
	/// In my application, the user can change the baud rate and the port. In order to do
	/// this, the SerialPort must be closed fist and this caused a random deadlock in my
	/// application. Microsoft should document this better!
	/// ============================================================================================
	/// 
	/// Use case 1: Open/close a single time from GUI
	/// ---------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Close port
	/// 4. Exit YAT
	/// 
	/// Use case 2: Close/open multiple times from GUI
	/// ----------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Close port
	/// 4. Open port
	/// 5. Repeat close/open multiple times
	/// 6. Exit YAT
	/// 
	/// Use case 3: Close/disconnect/reconnect/open multiple times
	/// ----------------------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Close port
	/// 4. Disconnect USB-to-serial adapter
	/// 5. Reconnect USB-to-serial adapter
	/// 6. Open port
	/// 7. Repeat close/disconnect/reconnect/open multiple times
	/// 8. Exit YAT
	/// 
	/// Use case 4: Disconnect/reconnect multiple times
	/// -----------------------------------------------
	/// 1. Start YAT
	/// 2. Open port
	/// 3. Disconnect USB-to-serial adapter
	/// 4. Reconnect USB-to-serial adapter
	///    => System.UnauthorizedAccssException("Access is denied.")
	///       @ System.IO.Ports.InternalResources.WinIOError(int errorCode, string str)
	///       @ System.IO.Ports.SerialStream.Dispose(bool disposing)
	///       @ System.IO.Ports.SerialStream.Finalize()
	/// 5. Repeat disconnect/reconnect multiple times
	/// 6. Exit YAT
	/// 
	/// ============================================================================================
	/// Work-arounds tried 2008-05
	/// - Async close
	/// - Async DataReceived event
	/// - Immediate async read
	/// - Dispatch of all open/close operations onto Windows.Forms main thread using OnRequest event
	/// - try GC.Collect(Forced) => no exceptions on GC, exception gets fired afterwards
	/// ============================================================================================
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Different root namespace.")]
	public class SerialPort : IIOProvider, IDisposable
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum State
		{
			Reset,
			Closed,
			Opened,
			WaitingForReopen,
			Error,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int AliveInterval = 500;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private State state = State.Reset;
		private object stateSyncObj = new object();
		
		private SerialPortSettings settings;

		/// <summary>
		/// Separate string containing the port name. Used for diagnostics/debug purposes.
		/// </summary>
		private string portName;

		private Ports.ISerialPort port;
		private object portSyncObj = new object();

		/// <summary>
		/// Async receiving.
		/// </summary>
		private Queue<byte> receiveQueue = new Queue<byte>();
		
		/// <summary>
		/// Alive timer detects port break states, i.e. when a USB to serial converter is disconnected.
		/// </summary>
		private System.Timers.Timer aliveTimer;
		private System.Timers.Timer reopenTimer;

	#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
		private bool isInternalStopRequest = false;
	#endif
		private ReaderWriterLockSlim isInternalStopRequestLock = new ReaderWriterLockSlim();

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
		public event EventHandler DataReceived;

		/// <summary></summary>
		public event EventHandler DataSent;

		/// <summary></summary>
		public event EventHandler<IORequestEventArgs> IORequest;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialPort(SerialPortSettings settings)
		{
			this.settings = settings;
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
			if (!this.isDisposed)
			{
				if (disposing)
				{
					// Ensure to reset state during Dispose().
					ResetPort();
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~SerialPort()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual SerialPortSettings Settings
		{
			get
			{
				AssertNotDisposed();
				return (this.settings);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				AssertNotDisposed();
				switch (this.state)
				{
					case State.Reset:
					case State.Error:
					{
						return (true);
					}
					default:
					{
						return (false);
					}
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				switch (this.state)
				{
					case State.Closed:
					case State.Opened:
					case State.WaitingForReopen:
					{
						return (true);
					}
					default:
					{
						return (false);
					}
				}
			}
		}

		private bool AutoReopenEnabledAndAllowed
		{
			get
			{
				return
					(
						!IsDisposed && IsStarted && !IsOpen &&
						this.settings.AutoReopen.Enabled
					);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();

				if (this.port != null)
					return (this.port.IsOpen);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();

				if (IsOpen)
					return (!this.port.OutputBreak && !this.port.InputBreak);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				AssertNotDisposed();

				if (IsOpen)
				{
					bool outputBreak = (this.settings.NoSendOnOutputBreak && this.port.OutputBreak);
					bool inputBreak  = (this.settings.NoSendOnInputBreak  && this.port.InputBreak);
					return (!outputBreak && !inputBreak);
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual int BytesAvailable
		{
			get
			{
				AssertNotDisposed();

				int bytesAvailable = 0;
				lock (this.receiveQueue)
				{
					bytesAvailable = this.receiveQueue.Count;
				}
				return (bytesAvailable); 
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.port);
			}
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool Start()
		{
			// AssertNotDisposed() is called by IsStarted.

			if (!IsStarted)
				CreateAndOpenPort();

			return (true);
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by IsStarted.

			if (IsStarted)
			{
			#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
				this.isInternalStopRequestLock.EnterReadLock();
				bool isInternalStopRequest = this.isInternalStopRequest;
				this.isInternalStopRequestLock.ExitReadLock();

				if (isInternalStopRequest && this.settings.AutoReopen.Enabled)
					ClosePortAndStartReopenTimer();
				else
					ResetPort();
			#else
					ResetPort();
			#endif
			}
		}

		/// <summary></summary>
		/// <remarks>
		/// Typically, 'OnDataReceived' has been fired before this method is called. However, this
		/// method can also be called after the port got closed to retrieve the remaining data.
		/// </remarks>
		public virtual int Receive(out byte[] data)
		{
			AssertNotDisposed();

			// Don't care whether the port actually is open. It shall also be possible to retrieve
			// remaining data after the port got closed.
			int bytesReceived = 0;
			lock (this.receiveQueue)
			{
				bytesReceived = this.receiveQueue.Count;
				data = new byte[bytesReceived];
				for (int i = 0; i < bytesReceived; i++)
					data[i] = this.receiveQueue.Dequeue();
			}
			return (bytesReceived);
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			// AssertNotDisposed() is called by IsOpen.

			if (IsOpen)
			{
				if (!this.port.OutputBreak)
				{
					lock (this.portSyncObj)
					{
						if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
							this.port.RtsEnable = true;

						this.port.Write(data, 0, data.Length);

						if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
							this.port.RtsEnable = false;
					}

					OnDataSent(new EventArgs());
				}
				else
				{
					OnIOError(new IOErrorEventArgs(IOErrorSeverity.Acceptable, IODirection.Output, "No data can be sent while port is in output break state"));
				}
			}
		}

		#endregion

		#region Settings Methods
		//==========================================================================================
		// Settings Methods
		//==========================================================================================

		private void ApplySettings()
		{
			if (this.port == null)
				return;

			lock (this.portSyncObj)
			{
				// No need to set encoding, only bytes are handled, encoding is done by text terminal
				//this.port.Encoding = this.ioSettings.Encoding;

				// Keep port name for diagnostics/debug purposes
				this.portName = this.settings.PortId;
				this.port.PortId = this.settings.PortId;

				SerialCommunicationSettings s = this.settings.Communication;
				this.port.BaudRate = (MKY.IO.Ports.BaudRateEx)s.BaudRate;
				this.port.DataBits = (MKY.IO.Ports.DataBitsEx)s.DataBits;
				this.port.Parity = s.Parity;
				this.port.StopBits = s.StopBits;
				this.port.Handshake = (SerialFlowControlEx)s.FlowControl;

				// Parity replace
				this.port.ParityReplace = this.settings.ParityErrorReplacement;

				// RTS and DTR
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.Manual:
						this.port.RtsEnable = this.settings.RtsEnabled;
						this.port.DtrEnable = this.settings.DtrEnabled;
						break;

					case SerialFlowControl.RS485:
						this.port.RtsEnable = false;
						break;
				}
			}
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private State GetStateSynchronized()
		{
			State state;

			lock (this.stateSyncObj)
				state = this.state;

			return (state);
		}

		private void SetStateSynchronizedAndNotify(State state)
		{
#if (DEBUG)
			State oldState = this.state;
#endif
			lock (this.stateSyncObj)
				this.state = state;
#if (DEBUG)
			Debug.WriteLine(GetType() + " '" + ToShortPortString() + "': State has changed from " + oldState + " to " + this.state + ".");
#endif
			OnIOChanged(new EventArgs());
			OnIOControlChanged(new EventArgs());
		}

		#endregion

		#region Simple Port Methods
		//==========================================================================================
		// Simple Port Methods
		//==========================================================================================

		private void CreatePort()
		{
			if (this.port != null)
				CloseAndDisposePort();

			lock (this.portSyncObj)
			{
				this.port = new Ports.SerialPortEx();
				this.port.DataReceived  += new Ports.SerialDataReceivedEventHandler (port_DataReceived);
				this.port.PinChanged    += new Ports.SerialPinChangedEventHandler   (port_PinChanged);
				this.port.ErrorReceived += new Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);
			}
		}

		private void OpenPort()
		{
			if (!this.port.IsOpen)
			{
				lock (this.portSyncObj)
					this.port.Open();
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void CloseAndDisposePort()
		{
			if (this.port != null)
			{
				try
				{
					lock (this.portSyncObj)
					{
						if (this.port.IsOpen)
							this.port.Close();

						this.port.Dispose();
						this.port = null;
					}
				}
				catch { }
			}
		}

		#endregion

		#region Port Methods
		//==========================================================================================
		// Port Methods
		//==========================================================================================

		/// <summary></summary>
		private void CreateAndOpenPort()
		{
			CreatePort();          // Port must be created each time because this.port.Close()
			ApplySettings();       //   disposes the underlying IO instance

			lock (this.portSyncObj)
			{
				// RTS
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.None:
					case SerialFlowControl.XOnXOff:
						this.port.RtsEnable = false;
						break;

					case SerialFlowControl.Manual:
						this.port.RtsEnable = this.settings.RtsEnabled;
						break;

					case SerialFlowControl.RS485:
						this.port.RtsEnable = false;
						break;

					case SerialFlowControl.RequestToSend:
					case SerialFlowControl.RequestToSendXOnXOff:
						// Do nothing, RTS is used for hand shake
						break;
				}

				// DTR
				switch (this.settings.Communication.FlowControl)
				{
					case SerialFlowControl.None:
					case SerialFlowControl.RequestToSend:
					case SerialFlowControl.XOnXOff:
					case SerialFlowControl.RequestToSendXOnXOff:
					case SerialFlowControl.RS485:
						this.port.DtrEnable = false;
						break;

					case SerialFlowControl.Manual:
						this.port.DtrEnable = this.settings.DtrEnabled;
						break;
				}
			} // lock (this.portSyncObj)

			OpenPort();
			StartAliveTimer();
			SetStateSynchronizedAndNotify(State.Opened);
		}

	#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
		/// <summary></summary>
		private void StopOrClosePort()
		{
			if (this.settings.AutoReopen.Enabled)
			{
				StopAndDisposeAliveTimer();
				CloseAndDisposePort();
				SetStateAndNotify(PortState.Closed);
				OnIOControlChanged(new EventArgs());

				StartReopenTimer();
			}
			else
			{
				Stop();
			}
		}
	#endif // DETECT_BREAKS_AND_TRY_AUTO_REOPEN

		/// <summary></summary>
		private void ClosePortAndStartReopenTimer()
		{
			StopAndDisposeAliveTimer();
			StopAndDisposeReopenTimer();
			CloseAndDisposePort();
			SetStateSynchronizedAndNotify(State.Closed);

			StartReopenTimer();
		}

		/// <summary></summary>
		private void ResetPort()
		{
			StopAndDisposeAliveTimer();
			StopAndDisposeReopenTimer();
			CloseAndDisposePort();
			SetStateSynchronizedAndNotify(State.Reset);
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="port_DataReceived"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the monitor is released
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
		/// </summary>
		private delegate void port_DataReceivedDelegate(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e);
		private object port_DataReceivedSyncObj = new object();

		private void port_DataReceived(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
			{
				// Immediately read data on this thread.
				int bytesToRead = this.port.BytesToRead;
				byte[] buffer = new byte[bytesToRead];
				this.port.Read(buffer, 0, bytesToRead);

				lock (this.receiveQueue)
				{
					foreach (byte b in buffer)
						this.receiveQueue.Enqueue(b);
				}

				// Ensure that only one data received event thread is active at a time.
				// Without this exclusivity, two receive threads could create a race condition.
				if (Monitor.TryEnter(this.port_DataReceivedSyncObj))
				{
					try
					{
						port_DataReceivedDelegate asyncInvoker = new port_DataReceivedDelegate(port_DataReceivedAsync);
						asyncInvoker.BeginInvoke(sender, e, null, null);
					}
					finally
					{
						Monitor.Exit(this.port_DataReceivedSyncObj);
					}
				}
			}
		}

		private void port_DataReceivedAsync(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			// Ensure that only one data received event thread is active at a time.
			// Without this exclusivity, two receive threads could create a race condition.
			Monitor.Enter(this.port_DataReceivedSyncObj);
			try
			{
				// Fire events until there is no more data. Must be done to ensure that events
				// are fired even for data that was enqueued above while the sync obj was busy.
				// In addition, wait for the minimal time possible to allow other threads to
				// execute and to prevent that 'OnDataReceived' events are fired consecutively.
				//
				// Measurements 2011-04-24 on an Intel Core 2 Duo running Win7 at 2.4 GHz and 3 GB of RAM:
				// > 0.0% CPU load in idle
				// > Up to an short-term-average of 20% CPU load while sending a large chuck of text (\YAT\_SendFiles\Stress-2-Large.txt, 106 kB)
				// This is an acceptable CPU load.
				//
				while (BytesAvailable > 0)
				{
					OnDataReceived(new EventArgs());
					Thread.Sleep(0);
				}
			}
			finally
			{
				Monitor.Exit(this.port_DataReceivedSyncObj);
			}
		}

		// Additional information to the 'DataReceived' event
		// --------------------------------------------------
		// An improvement suggested by Marco Stroppel on 2011-02-17 doesn't work in case of YAT. Suggestion:
		// 
		//   The while(BytesAvailable > 0) fires endless events, because I did not call the Receive() method.
		//   That was, because I receive only the data when the other port to write the data is opened. So the
		//   BytesAvailable got never zero. My idea was (not knowing if this is good) to do something like:
		//   
		//   while(BytesAvailable > LastTimeBytesAvailable)
		//   {
		//       LastTimeBytesAvailable = BytesAvailable;
		//       OnDataReceived(new EventArgs());
		//   }
		// 
		// This suggestions doesn't work because YAT shall show every single byte as soon as it get's received.
		// If 3 bytes are received while 5 bytes are taken out of the receive queue, no more event gets fired.
		// Thus, the 3 bytes do not get shown until new data arrives. This is not acceptable.

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void port_PinChangedDelegate(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e);

		private void port_PinChanged(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
			{
				port_PinChangedDelegate asyncInvoker = new port_PinChangedDelegate(port_PinChangedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void port_PinChangedAsync(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			// If pin has changed, but access to port throws exception, port has been shut down,
			//   e.g. USB to serial converter disconnected.
			try
			{
				// Force access to port to check whether it's still alive.
				bool cts = this.port.CtsHolding;

				if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
				{
					switch (e.EventType)
					{
						case MKY.IO.Ports.SerialPinChange.InputBreak:
							if (this.settings.NoSendOnInputBreak)
								OnIOChanged(new EventArgs());
							break;

						case MKY.IO.Ports.SerialPinChange.OutputBreak:
							OnIOChanged(new EventArgs());
							break;

						default:
							// Do not fire general event, I/O control event is fired below.
							break;
					}
					OnIOControlChanged(new EventArgs());
				}
			}
			catch
			{
				OnIORequest(new IORequestEventArgs(Serial.IORequest.Close));

			#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
				StopOrClosePort();
			#endif
			}
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void port_ErrorReceivedDelegate(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e);

		private void port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (this.state == State.Opened) // Ensure not to forward any events during closing anymore.
			{
				port_ErrorReceivedDelegate asyncInvoker = new port_ErrorReceivedDelegate(port_ErrorReceivedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		private void port_ErrorReceivedAsync(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			string message;
			switch (e.EventType)
			{
				case System.IO.Ports.SerialError.Frame:    message = "Serial port input framing error!";            break;
				case System.IO.Ports.SerialError.Overrun:  message = "Serial port input character buffer overrun!"; break;
				case System.IO.Ports.SerialError.RXOver:   message = "Serial port input buffer overflow!";          break;
				case System.IO.Ports.SerialError.RXParity: message = "Serial port input parity error!";             break;
				case System.IO.Ports.SerialError.TXFull:   message = "Serial port output buffer full!";             break;
				default:                                   message = "Unknown serial port error!";                  break;
			}
			OnIOError(new SerialPortIOErrorEventArgs(message, e.EventType));
		}

		#endregion

		#region Alive Timer
		//==========================================================================================
		// Alive Timer
		//==========================================================================================

		private void StartAliveTimer()
		{
			if (this.aliveTimer == null)
			{
				this.aliveTimer = new System.Timers.Timer(AliveInterval);
				this.aliveTimer.AutoReset = true;
				this.aliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.aliveTimer_Elapsed);
				this.aliveTimer.Start();
			}
		}

		private void StopAndDisposeAliveTimer()
		{
			if (this.aliveTimer != null)
			{
				this.aliveTimer.Stop();
				this.aliveTimer.Dispose();
				this.aliveTimer = null;
			}
		}

#if (FALSE)
		// \fixme:
		// Break state detection doesn't work.
		private bool aliveTimer_BreakState = false;
#endif

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void aliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!IsDisposed && IsStarted)
			{
				try
				{
					// If port isn't open anymore, or access to port throws exception,
					//   port has been shut down, e.g. USB to serial converter disconnected
					if (!this.port.IsOpen)
					{
						OnIORequest(new IORequestEventArgs(Serial.IORequest.Close));

					#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
						StopOrClosePort();
					#endif
					}
#if (FALSE)
					// \fixme
					// Break state detection doesn't work.
					else
					{
						// detect break state changes
						if (this.aliveTimer_BreakState != this.port.BreakState)
							OnIOChanged(new EventArgs());

						this.aliveTimer_BreakState = this.port.BreakState;
					}
#endif
				}
				catch
				{
					OnIORequest(new IORequestEventArgs(Serial.IORequest.Close));

				#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
					StopOrClosePort();
				#endif
				}
			}
			else
			{
				StopAndDisposeAliveTimer();
			}
		}

		#endregion

		#region Reopen Timer
		//==========================================================================================
		// Reopen Timer
		//==========================================================================================

		private void StartReopenTimer()
		{
			if (this.reopenTimer == null)
			{
				this.reopenTimer = new System.Timers.Timer(this.settings.AutoReopen.Interval);
				this.reopenTimer.AutoReset = false;
				this.reopenTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.reopenTimer_Elapsed);
			}
			this.reopenTimer.Start();
		}

		private void StopAndDisposeReopenTimer()
		{
			if (this.reopenTimer != null)
			{
				this.reopenTimer.Stop();
				this.reopenTimer.Dispose();
				this.reopenTimer = null;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void reopenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (AutoReopenEnabledAndAllowed)
			{
				try
				{
					// Try to re-open port
					OnIORequest(new IORequestEventArgs(Serial.IORequest.Open));

				#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
					CreateAndOpenPort();
				#endif
				}
				catch
				{
					CloseAndDisposePort();
					SetStateSynchronizedAndNotify(State.Closed); // Re-open failed, cleanup and restart
					StartReopenTimer();
				}
			}
			else
			{
				StopAndDisposeReopenTimer();
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
		protected virtual void OnDataReceived(EventArgs e)
		{
			EventHelper.FireSync(DataReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataSent(EventArgs e)
		{
			EventHelper.FireSync(DataSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIORequest(IORequestEventArgs e)
		{
		#if DETECT_BREAKS_AND_TRY_AUTO_REOPEN
			if (e.Request == Serial.IORequest.Close)
			{
				this.isInternalStopRequestLock.EnterWriteLock();
				this.isInternalStopRequest = true;
				this.isInternalStopRequestLock.ExitWriteLock();
			}
		#endif

			EventHelper.FireSync<IORequestEventArgs>(IORequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToShortPortString()
		{
			if (this.port != null)
				return (this.port.PortId);
			else if (!string.IsNullOrEmpty(this.portName))
				return (this.portName);
			else
				return (Undefined);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================