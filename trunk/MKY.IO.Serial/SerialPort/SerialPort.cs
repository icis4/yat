using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Event;

namespace MKY.IO.Serial
{
	/// <summary></summary>
	/// <remarks>
	/// <see cref="System.IO.Ports.SerialPort"/> is not thread-safe. Therefore, access to
	/// _port is done using lock.
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

		private int _AliveInterval = 500;

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
					BeginInvokeCloseAndDisposePort();
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

				if (_port != null)
					return (_port.BytesToRead);
				else
					return (0);
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
				ResetPort();
		}

		/// <summary></summary>
		public int Receive(out byte[] buffer)
		{
			// AssertNotDisposed() is called by IsOpen

			int bytesReceived = 0;
			if (IsOpen)
			{
				int bytesToRead = _port.BytesToRead;
				buffer = new byte[bytesToRead];
				bytesReceived = _port.Read(buffer, 0, bytesToRead);
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
				if (_settings.Communication.FlowControl == SerialFlowControl.RS485)
					_port.RtsEnable = true;

				_port.Write(buffer, 0, buffer.Length);

				if (_settings.Communication.FlowControl == SerialFlowControl.RS485)
					_port.RtsEnable = false;

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

			// no need to set encoding, only bytes are handled, encoding is done by text terminal
			//_port.Encoding = _ioSettings.Encoding;

			_port.PortId = _settings.PortId;

			ApplyCommunicationSettings();

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

		private void ApplyCommunicationSettings()
		{
			if (_port == null)
				return;

			SerialCommunicationSettings s = _settings.Communication;
			_port.BaudRate = (MKY.IO.Ports.XBaudRate)s.BaudRate;
			_port.DataBits = (MKY.IO.Ports.XDataBits)s.DataBits;
			_port.Parity = s.Parity;
			_port.StopBits = s.StopBits;
			_port.Handshake = (XSerialFlowControl)s.FlowControl;
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

			_port = new MKY.IO.Ports.SerialPortDotNet();
			_port.DataReceived  += new MKY.IO.Ports.SerialDataReceivedEventHandler(_port_DataReceived);
			_port.PinChanged    += new MKY.IO.Ports.SerialPinChangedEventHandler(_port_PinChanged);
			_port.ErrorReceived += new MKY.IO.Ports.SerialErrorReceivedEventHandler(_port_ErrorReceived);
		}

		private void OpenPort()
		{
			if (!_port.IsOpen)
				_port.Open();
		}

		private void CloseAndDisposePort()
		{
			if (_port != null)
			{
				try
				{
					if (_port.IsOpen)
						_port.Close();

					_port.Dispose();
					_port = null;
				}
				catch { }
			}
		}

		#endregion

		#region Async Port Methods
		//==========================================================================================
		// Async Port Methods
		//==========================================================================================

		private MKY.IO.Ports.ISerialPort _portAsync = null;
		private object _portAsyncSyncObj = new object();
		private delegate void AsyncInvokeDelegate();

		/// <summary></summary>
		/// <remarks>
		/// Asynchronously invoke close/dispose to prevent potential dead-locks if
		/// close/dispose was called from a ISynchronizeInvoke target (i.e. a form).
		/// </remarks>
		private void BeginInvokeCloseAndDisposePort()
		{
			// Copy reference and immediately set _port to null to prevent any
			//   further access to it.
			lock (_portAsyncSyncObj)
			{
				_portAsync = _port;
			}
			_port = null;

			AsyncInvokeDelegate asyncInvoker = new AsyncInvokeDelegate(DoCloseAndDisposePortAsync);
			asyncInvoker.BeginInvoke(null, null);
		}

		private void DoCloseAndDisposePortAsync()
		{
			lock (_portAsyncSyncObj)
			{
				try
				{
					if (_portAsync != null)
					{
						if (_portAsync.IsOpen)
							_portAsync.Close();

						_portAsync.Dispose();
						_portAsync = null;
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

			OpenPort();
			StartAliveTimer();

			lock (_stateSyncObj)
				_state = PortState.Openend;

			OnIOChanged(new EventArgs());
			OnIOControlChanged(new EventArgs());
		}

		/// <summary></summary>
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
		}

		/// <summary></summary>
		private void ResetPort()
		{
			lock (_stateSyncObj)
				_state = PortState.Reset;

			StopAndDisposeAliveTimer();
			StopAndDisposeReopenTimer();
			BeginInvokeCloseAndDisposePort();

			OnIOChanged(new EventArgs());
			OnIOControlChanged(new EventArgs());
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		private void _port_DataReceived(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			if (_state == PortState.Openend) // ensure not to forward any events during closing anymore
				OnDataReceived(new EventArgs());
		}

		private void _port_PinChanged(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
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
				StopOrClosePort();
			}
		}

		private void _port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			if (_state == PortState.Openend) // ensure not to forward any events during closing anymore
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
						StopOrClosePort();
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
					StopOrClosePort();
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
			if (_reopenTimer != null)
				StopAndDisposeReopenTimer();

			_reopenTimer = new System.Timers.Timer(_settings.AutoReopen.Interval);
			_reopenTimer.AutoReset = false;
			_reopenTimer.Elapsed += new System.Timers.ElapsedEventHandler(_reopenTimer_Elapsed);
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
					CreateAndOpenPort();
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
