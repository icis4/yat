using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

using MKY.Utilities.Event;

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
	///       @ System.IO.Ports.InternalResources.WinIOError(Int32 errorCode, String str)
	///       @ System.IO.Ports.SerialStream.Dispose(Boolean disposing)
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
	public class SerialPort : IIOProvider, IDisposable
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum PortState
		{
			Reset,
			Closed,
			Openend,
			WaitingForReopen,
			Error,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int _AliveInterval = 500;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private PortState _state = PortState.Reset;
		private object _stateSyncObj = new object();
		
		private SerialPortSettings _settings;
		private MKY.IO.Ports.ISerialPort _port;
		private object _portSyncObj = new object();

		// async receiving
		private Queue<byte> _receiveQueue = new Queue<byte>();
		
		/// <summary>
		/// Alive timer detects port break states, i.e. when a USB to serial converter is disconnected.
		/// </summary>
		private System.Timers.Timer _aliveTimer;
		private System.Timers.Timer _reopenTimer;

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
		public event EventHandler<IORequestEventArgs> IORequest;
		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;
		/// <summary></summary>
		public event EventHandler DataReceived;
		/// <summary></summary>
		public event EventHandler DataSent;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialPort(SerialPortSettings settings)
		{
			_settings = settings;
			ApplySettings();
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
			if (!_isDisposed)
			{
				if (disposing)
				{
					StopAndDisposeAliveTimer();
					StopAndDisposeReopenTimer();
					CloseAndDisposePort();
				}
				_isDisposed = true;
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
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
				{
					case PortState.Closed:
					case PortState.Openend:
					case PortState.WaitingForReopen:
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
						_settings.AutoReopen.Enabled
					);
			}
		}

		/// <summary></summary>
		public bool IsOpen
		{
			get
			{
				AssertNotDisposed();

				if (_port != null)
					return (_port.IsOpen);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();

				if (_port != null)
					return (_port.IsOpen && !_port.BreakState);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public int BytesAvailable
		{
			get
			{
				AssertNotDisposed();

				int bytesAvailable = 0;
				lock (_receiveQueue)
				{
					bytesAvailable = _receiveQueue.Count;
				}
				return (bytesAvailable); 
			}
		}

		/// <summary></summary>
		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_port);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public void Start()
		{
			// AssertNotDisposed() is called by IsStarted

			if (!IsStarted)
				CreateAndOpenPort();
		}

		/// <summary></summary>
		public void Stop()
		{
			// AssertNotDisposed() is called by IsStarted

			if (IsStarted)
			{
				// \fixme Auto-reopen doesn't work because of deadlock issue mentioned above.
				//if (_isInternalStopRequest && _settings.AutoReopen.Enabled)
				//	ClosePortAndStartReopenTimer();
				//else
					ResetPort();
			}
		}

		/// <summary></summary>
		public int Receive(out byte[] buffer)
		{
			// AssertNotDisposed() is called by IsOpen

			int bytesReceived = 0;
			if (IsOpen)
			{
				lock (_receiveQueue)
				{
					bytesReceived = _receiveQueue.Count;
					buffer = new byte[bytesReceived];
					for (int i = 0; i < bytesReceived; i++)
						buffer[i] = _receiveQueue.Dequeue();
				}
			}
			else
			{
				buffer = new byte[] { };
			}
			return (bytesReceived);
		}

		/// <summary></summary>
		public void Send(byte[] buffer)
		{
			// AssertNotDisposed() is called by IsOpen

			if (IsOpen)
			{
				lock (_portSyncObj)
				{
					if (_settings.Communication.FlowControl == SerialFlowControl.RS485)
						_port.RtsEnable = true;

					_port.Write(buffer, 0, buffer.Length);

					if (_settings.Communication.FlowControl == SerialFlowControl.RS485)
						_port.RtsEnable = false;
				}

				OnDataSent(new EventArgs());
			}
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void ApplySettings()
		{
			if (_port == null)
				return;

			lock (_portSyncObj)
			{
				// no need to set encoding, only bytes are handled, encoding is done by text terminal
				//_port.Encoding = _ioSettings.Encoding;

				_port.PortId = _settings.PortId;

				SerialCommunicationSettings s = _settings.Communication;
				_port.BaudRate = (MKY.IO.Ports.XBaudRate)s.BaudRate;
				_port.DataBits = (MKY.IO.Ports.XDataBits)s.DataBits;
				_port.Parity = s.Parity;
				_port.StopBits = s.StopBits;
				_port.Handshake = (XSerialFlowControl)s.FlowControl;

				// parity replace
				_port.ParityReplace = _settings.ParityErrorReplacement;

				// RTS and DTR
				switch (_settings.Communication.FlowControl)
				{
					case SerialFlowControl.Manual:
						_port.RtsEnable = _settings.RtsEnabled;
						_port.DtrEnable = _settings.DtrEnabled;
						break;

					case SerialFlowControl.RS485:
						_port.RtsEnable = false;
						break;
				}
			}
		}

		#endregion

		#region Simple Port Methods
		//==========================================================================================
		// Simple Port Methods
		//==========================================================================================

		private void CreatePort()
		{
			if (_port != null)
				CloseAndDisposePort();

			lock (_portSyncObj)
			{
				_port = new MKY.IO.Ports.SerialPortDotNet();
				_port.DataReceived += new MKY.IO.Ports.SerialDataReceivedEventHandler(_port_DataReceived);
				_port.PinChanged += new MKY.IO.Ports.SerialPinChangedEventHandler(_port_PinChanged);
				_port.ErrorReceived += new MKY.IO.Ports.SerialErrorReceivedEventHandler(_port_ErrorReceived);
			}
		}

		private void OpenPort()
		{
			if (!_port.IsOpen)
			{
				lock (_portSyncObj)
					_port.Open();
			}
		}

		private void CloseAndDisposePort()
		{
			if (_port != null)
			{
				try
				{
					lock (_portSyncObj)
					{
						if (_port.IsOpen)
							_port.Close();

						_port.Dispose();
						_port = null;
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
			CreatePort();          // port must be created each time because _port.Close()
			ApplySettings();       //   disposes the underlying IO instance

			lock (_portSyncObj)
			{
				// RTS
				switch (_settings.Communication.FlowControl)
				{
					case SerialFlowControl.None:
					case SerialFlowControl.XOnXOff:
						_port.RtsEnable = false;
						break;

					case SerialFlowControl.Manual:
						_port.RtsEnable = _settings.RtsEnabled;
						break;

					case SerialFlowControl.RS485:
						_port.RtsEnable = false;
						break;

					case SerialFlowControl.RequestToSend:
					case SerialFlowControl.RequestToSendXOnXOff:
						// do nothing, RTS is used for hand shake
						break;
				}

				// DTR
				switch (_settings.Communication.FlowControl)
				{
					case SerialFlowControl.None:
					case SerialFlowControl.RequestToSend:
					case SerialFlowControl.XOnXOff:
					case SerialFlowControl.RequestToSendXOnXOff:
					case SerialFlowControl.RS485:
						_port.DtrEnable = false;
						break;

					case SerialFlowControl.Manual:
						_port.DtrEnable = _settings.DtrEnabled;
						break;
				}
			} // lock (_portSyncObj)

			OpenPort();
			StartAliveTimer();

			lock (_stateSyncObj)
				_state = PortState.Openend;

			OnIOChanged(new EventArgs());
			OnIOControlChanged(new EventArgs());
		}

		// \fixme Auto-reopen doesn't work because of deadlock issue mentioned above.
		/*/// <summary></summary>
		private void StopOrClosePort()
		{
			if (_settings.AutoReopen.Enabled)
			{
				StopAndDisposeAliveTimer();

				lock (_stateSyncObj)
					_state = PortState.Closed;

				CloseAndDisposePort();

				OnIOChanged(new EventArgs());
				OnIOControlChanged(new EventArgs());

				StartReopenTimer();
			}
			else
			{
				Stop();
			}
		}*/

		/// <summary></summary>
		private void ClosePortAndStartReopenTimer()
		{
			StopAndDisposeAliveTimer();

			lock (_stateSyncObj)
				_state = PortState.Closed;

			CloseAndDisposePort();

			OnIOChanged(new EventArgs());
			OnIOControlChanged(new EventArgs());

			StartReopenTimer();
		}

		/// <summary></summary>
		private void ResetPort()
		{
			lock (_stateSyncObj)
				_state = PortState.Reset;

			StopAndDisposeAliveTimer();
			StopAndDisposeReopenTimer();
			CloseAndDisposePort();

			OnIOChanged(new EventArgs());
			OnIOControlChanged(new EventArgs());
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void _port_DataReceivedDelegate(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e);
		private object _port_DataReceivedSyncObj = new object();

		private void _port_DataReceived(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			if (_state == PortState.Openend) // Ensure not to forward any events during closing anymore
			{
				// Immediately read data on this thread
				int bytesToRead = _port.BytesToRead;
				byte[] buffer = new byte[bytesToRead];
				_port.Read(buffer, 0, bytesToRead);

				lock (_receiveQueue)
				{
					foreach (byte b in buffer)
						_receiveQueue.Enqueue(b);
				}

				// Ensure that only one data received event thread is active at the same time.
				// Without this exclusivity, two receive threads could create a race condition.
				if (Monitor.TryEnter(_port_DataReceivedSyncObj))
				{
					Monitor.Exit(_port_DataReceivedSyncObj);

					_port_DataReceivedDelegate asyncInvoker = new _port_DataReceivedDelegate(_port_DataReceivedAsync);
					asyncInvoker.BeginInvoke(sender, e, null, null);
				}
			}
		}

		private void _port_DataReceivedAsync(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			// Ensure that only one data received event thread is active at the same time.
			// Without this exclusivity, two receive threads could create a race condition.
			Monitor.Enter(_port_DataReceivedSyncObj);
			try
			{
				// Fire events until there is no more data available. Must be done to ensure
				// that events are fired even for data that was enqueued above while the sync
				// obj was busy.
				while (BytesAvailable > 0)
					OnDataReceived(new EventArgs());
			}
			finally
			{
				Monitor.Exit(_port_DataReceivedSyncObj);
			}
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void _port_PinChangedDelegate(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e);

		private void _port_PinChanged(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			if (_state == PortState.Openend) // ensure not to forward any events during closing anymore
			{
				_port_PinChangedDelegate asyncInvoker = new _port_PinChangedDelegate(_port_PinChangedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		private void _port_PinChangedAsync(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			// if pin has changed, but access to port throws exception, port has been shut down,
			//   e.g. USB to serial converter disconnected
			try
			{
				// force access to port to check whether it's still alive
				bool cts = _port.CtsHolding;

				if (_state == PortState.Openend) // ensure not to forward any events during closing anymore
					OnIOControlChanged(new EventArgs());
			}
			catch
			{
				OnIORequest(new IORequestEventArgs(Serial.IORequest.Close));
				// \fixme Auto-reopen doesn't work because of deadlock issue mentioned above.
				//StopOrClosePort();
			}
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		private delegate void _port_ErrorReceivedDelegate(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e);

		private void _port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (_state == PortState.Openend) // ensure not to forward any events during closing anymore
			{
				_port_ErrorReceivedDelegate asyncInvoker = new _port_ErrorReceivedDelegate(_port_ErrorReceivedAsync);
				asyncInvoker.BeginInvoke(sender, e, null, null);
			}
		}

		private void _port_ErrorReceivedAsync(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			string message;
			switch (e.EventType)
			{
				case System.IO.Ports.SerialError.Frame:    message = "Serial port framing error!";            break;
				case System.IO.Ports.SerialError.Overrun:  message = "Serial port character buffer overrun!"; break;
				case System.IO.Ports.SerialError.RXOver:   message = "Serial port input buffer overflow!";    break;
				case System.IO.Ports.SerialError.RXParity: message = "Serial port parity error!";             break;
				case System.IO.Ports.SerialError.TXFull:   message = "Serial port output buffer full!";       break;
				default:                                   message = "Unknown serial port error!";            break;
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
			if (_aliveTimer == null)
			{
				_aliveTimer = new System.Timers.Timer(_AliveInterval);
				_aliveTimer.AutoReset = true;
				_aliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(_aliveTimer_Elapsed);
				_aliveTimer.Start();
			}
		}

		private void StopAndDisposeAliveTimer()
		{
			if (_aliveTimer != null)
			{
				_aliveTimer.Stop();
				_aliveTimer.Dispose();
				_aliveTimer = null;
			}
		}

		#if false
		// \fixme break state detection doesn't work
		private bool _aliveTimer_BreakState = false;
		#endif

		private void _aliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!IsDisposed && IsStarted)
			{
				try
				{
					// if port isn't open anymore, or access to port throws exception,
					//   port has been shut down, e.g. USB to serial converter disconnected
					if (!_port.IsOpen)
					{
						OnIORequest(new IORequestEventArgs(Serial.IORequest.Close));
						// \fixme Auto-reopen doesn't work because of deadlock issue mentioned above.
						//StopOrClosePort();
					}
					#if false
					// \fixme break state detection doesn't work
					else
					{
						// detect break state changes
						if (_aliveTimer_BreakState != _port.BreakState)
							OnIOChanged(new EventArgs());

						_aliveTimer_BreakState = _port.BreakState;
					}
					#endif
				}
				catch
				{
					OnIORequest(new IORequestEventArgs(Serial.IORequest.Close));
					// \fixme Auto-reopen doesn't work because of deadlock issue mentioned above.
					//StopOrClosePort();
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
			if (_reopenTimer == null)
			{
				_reopenTimer = new System.Timers.Timer(_settings.AutoReopen.Interval);
				_reopenTimer.AutoReset = false;
				_reopenTimer.Elapsed += new System.Timers.ElapsedEventHandler(_reopenTimer_Elapsed);
			}
			_reopenTimer.Start();
		}

		private void StopAndDisposeReopenTimer()
		{
			if (_reopenTimer != null)
			{
				_reopenTimer.Stop();
				_reopenTimer.Dispose();
				_reopenTimer = null;
			}
		}

		private void _reopenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (AutoReopenEnabledAndAllowed)
			{
				try
				{
					// try to re-open port
					OnIORequest(new IORequestEventArgs(Serial.IORequest.Open));
					// \fixme Auto-reopen doesn't work because of deadlock issue mentioned above.
					//CreateAndOpenPort();
				}
				catch
				{
					// re-open failed, cleanup and restart
					lock (_stateSyncObj)
						_state = PortState.Closed;

					CloseAndDisposePort();
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
		protected virtual void OnIORequest(IORequestEventArgs e)
		{
			// \fixme Auto-reopen doesn't work because of deadlock issue mentioned above.
			//if (e.Request == Serial.IORequest.Close)
			//	_isInternalStopRequest = true;

			EventHelper.FireSync<IORequestEventArgs>(IORequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
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

		#endregion
	}
}