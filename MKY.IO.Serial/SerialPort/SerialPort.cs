using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Event;

namespace YAT.Domain.IO
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

		private Settings.SerialPort.SerialPortSettings _settings;
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
		public SerialPort(Settings.SerialPort.SerialPortSettings settings)
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
					CloseAndDisposePortAsync();
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
		public bool HasStarted
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

		/// <summary></summary>
		public bool IsConnected
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
			// AssertNotDisposed() is called by HasStarted

			if (!HasStarted)
				CreateAndOpenPort();
		}

		/// <summary></summary>
		public void Stop()
		{
			if (HasStarted)
			{
				lock (_stateSyncObj)
					_state = PortState.Reset;

				StopAndDisposeReopenTimer();
				CloseAndDisposePort();
			}
		}

		/// <summary></summary>
		public int Receive(out byte[] buffer)
		{
			// AssertNotDisposed() is called by IsConnected

			int bytesReceived = 0;
			if (IsConnected)
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
			// AssertNotDisposed() is called by IsConnected

			if (IsConnected)
			{
				if (_settings.Communication.FlowControl == FlowControl.RS485)
					_port.RtsEnable = true;

				_port.Write(buffer, 0, buffer.Length);

				if (_settings.Communication.FlowControl == FlowControl.RS485)
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
			Parser.Parser p = new Parser.Parser(Encoding.Default);
			byte[] bytes;
			if (p.TryParse(_settings.ParityErrorReplacement, out bytes) && (bytes.Length >= 1))
				_port.ParityReplace = bytes[0];
			else
				_port.ParityReplace = Settings.SerialPort.SerialPortSettings.ParityErrorReplacementDefaultAsByte;

			// RTS and DTR
			switch (_settings.Communication.FlowControl)
			{
				case FlowControl.Manual:
					_port.RtsEnable = _settings.RtsEnabled;
					_port.DtrEnable = _settings.DtrEnabled;
					break;

				case FlowControl.RS485:
					_port.RtsEnable = false;
					break;
			}

			// auto reopen
			if (_settings.AutoReopen.Enabled)
			{
				if (!IsDisposed && HasStarted && !IsConnected)
					StartReopenTimer();
			}
		}

		private void ApplyCommunicationSettings()
		{
			if (_port == null)
				return;

			Settings.SerialPort.SerialCommunicationSettings s = _settings.Communication;
			_port.BaudRate = (MKY.IO.Ports.XBaudRate)s.BaudRate;
			_port.DataBits = (MKY.IO.Ports.XDataBits)s.DataBits;
			_port.Parity = s.Parity;
			_port.StopBits = s.StopBits;
			_port.Handshake = (Domain.IO.XFlowControl)s.FlowControl;
		}

		#endregion

		#region Simple Port Methods
		//==========================================================================================
		// Simple Port Methods
		//==========================================================================================

		private void CreatePort()
		{
			if (_port != null)
				DisposePort();

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

		private void ClosePort()
		{
			if (_port.IsOpen)
				_port.Close();
		}

		private void DisposePort()
		{
			if (_port != null)
			{
				_port.Dispose();
				_port = null;
			}
		}

		private delegate void AsyncInvokeDelegate();

		/// <summary></summary>
		/// <remarks>
		/// Asynchronously invoke close/dispose to prevent potential dead-locks if
		/// close/dispose was called from a ISynchronizeInvoke target (i.e. a form).
		/// </remarks>
		private void CloseAndDisposePortAsync()
		{
			AsyncInvokeDelegate asyncInvoker = new AsyncInvokeDelegate(DoCloseAndDisposePortAsync);
			asyncInvoker.BeginInvoke(null, null);
		}

		private void DoCloseAndDisposePortAsync()
		{
			ClosePort();
			DisposePort();
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
				case FlowControl.None:
				case FlowControl.XOnXOff:
					_port.RtsEnable = false;
					break;

				case FlowControl.Manual:
					_port.RtsEnable = _settings.RtsEnabled;
					break;

				case FlowControl.RS485:
					_port.RtsEnable = false;
					break;

				case FlowControl.RequestToSend:
				case FlowControl.RequestToSendXOnXOff:
					// do nothing, RTS is used for hand shake
					break;
			}

			// DTR
			switch (_settings.Communication.FlowControl)
			{
				case FlowControl.None:
				case FlowControl.RequestToSend:
				case FlowControl.XOnXOff:
				case FlowControl.RequestToSendXOnXOff:
				case FlowControl.RS485:
					_port.DtrEnable = false;
					break;

				case FlowControl.Manual:
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
				lock (_stateSyncObj)
					_state = PortState.Closed;

				CloseAndDisposePort();
				StartReopenTimer();
			}
			else
			{
				Stop();
			}
		}

		/// <summary></summary>
		/// <remarks>
		/// State must be set by calling function.
		/// </remarks>
		private void CloseAndDisposePort()
		{
			#if false

			// RTS/DTR are reset upon ClosePort()
			_port.RtsEnable = false;
			_port.DtrEnable = false;

			#endif

			StopAndDisposeAliveTimer();
			CloseAndDisposePortAsync();

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
			System.Diagnostics.Trace.WriteLine("PinChanged");
			try
			{
				System.Diagnostics.Trace.WriteLine("PinChanged::Trying");

				// force access to port to check whether it's still alive
				bool cts = _port.CtsHolding;

				System.Diagnostics.Trace.WriteLine("PinChanged::OK");

				if (_state == PortState.Openend) // ensure not to forward any events during closing anymore
					OnIOControlChanged(new EventArgs());
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("PinChanged::Closing");
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

		private void _aliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!IsDisposed && HasStarted)
			{
				try
				{
					// if port isn't open anymore, or access to port throws exception,
					//   port has been shut down, e.g. USB to serial converter disconnected
					if (_port.IsOpen)
						System.Diagnostics.Trace.WriteLine("Alive and open");
					else
					{
						System.Diagnostics.Trace.WriteLine("Alive and closed");
						StopOrClosePort();
					}
				}
				catch
				{
					System.Diagnostics.Trace.WriteLine("Non more alive");
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
			if (!IsDisposed && HasStarted && !IsConnected)
			{
				try
				{
					Start();
				}
				catch
				{
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
