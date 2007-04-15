using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain.IO
{
	public class SerialPort : IIOProvider, IDisposable
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isDisposed = false;

		private Settings.SerialPort.SerialPortSettings _settings;
		private HSR.IO.Ports.ISerialPort _port;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		public event EventHandler IOChanged;
		public event EventHandler IOControlChanged;
		public event EventHandler<IOErrorEventArgs> IOError;
		public event EventHandler DataReceived;
		public event EventHandler DataSent;

		//------------------------------------------------------------------------------------------
		// Object Lifetime
		//------------------------------------------------------------------------------------------

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
					DisposePort();
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~SerialPort()
		{
			Dispose(false);
		}

		protected bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public bool HasStarted
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
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		public void Start()
		{
			// AssertNotDisposed() is called by HasStarted

			if (!HasStarted)
			{
				CreatePort();          // port must be created each time because _port.Close()
				ApplySettings();       //   disposes the underlying IO instance
				OpenPort();

				// RTS
				switch (_settings.Communication.Handshake)
				{
					case Handshake.None:
					case Handshake.XOnXOff:
						_port.RtsEnable = true;
						break;

					case Handshake.Manual:
						_port.RtsEnable = _settings.RtsEnabled;
						break;

					case Handshake.RS485:
						_port.RtsEnable = false;
						break;

					case Handshake.RequestToSend:
					case Handshake.RequestToSendXOnXOff:
						// do nothing, RTS is used for hand shake
						break;
				}

				// DTR
				switch (_settings.Communication.Handshake)
				{
					case Handshake.None:
					case Handshake.RequestToSend:
					case Handshake.XOnXOff:
					case Handshake.RequestToSendXOnXOff:
					case Handshake.RS485:
						_port.DtrEnable = true;
						break;

					case Handshake.Manual:
						_port.DtrEnable = _settings.DtrEnabled;
						break;
				}

				OnIOChanged(new EventArgs());
				OnIOControlChanged(new EventArgs());
			}
		}

		public void Stop()
		{
			// AssertNotDisposed() is called by HasStarted

			if (HasStarted)
			{
				#if false

				// RTS/DTR are reset upon ClosePort()
				_port.RtsEnable = false;
				_port.DtrEnable = false;

				#endif

				ClosePort();
				DisposePort();

				OnIOChanged(new EventArgs());
				OnIOControlChanged(new EventArgs());
			}
		}

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

		public void Send(byte[] buffer)
		{
			// AssertNotDisposed() is called by IsConnected

			if (IsConnected)
			{
				if (_settings.Communication.Handshake == Handshake.RS485)
					_port.RtsEnable = true;

				_port.Write(buffer, 0, buffer.Length);

				if (_settings.Communication.Handshake == Handshake.RS485)
					_port.RtsEnable = false;

				OnDataSent(new EventArgs());
			}
		}

		#endregion

		#region Settings
		//------------------------------------------------------------------------------------------
		// Settings
		//------------------------------------------------------------------------------------------

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
			switch (_settings.Communication.Handshake)
			{
				case Handshake.Manual:
					_port.RtsEnable = _settings.RtsEnabled;
					_port.DtrEnable = _settings.DtrEnabled;
					break;

				case Handshake.RS485:
					_port.RtsEnable = false;
					break;
			}
		}

		private void ApplyCommunicationSettings()
		{
			if (_port == null)
				return;

			Settings.SerialPort.SerialCommunicationSettings s = _settings.Communication;
			_port.BaudRate = (HSR.IO.Ports.XBaudRate)s.BaudRate;
			_port.DataBits = (HSR.IO.Ports.XDataBits)s.DataBits;
			_port.Parity = s.Parity;
			_port.StopBits = s.StopBits;
			_port.Handshake = (Domain.IO.XHandshake)s.Handshake;
		}

		#endregion

		#region Port
		//------------------------------------------------------------------------------------------
		// Port
		//------------------------------------------------------------------------------------------

		private void CreatePort()
		{
			if (_port != null)
				DisposePort();
			
			_port = new HSR.IO.Ports.SerialPortDotNet();
			_port.DataReceived += new HSR.IO.Ports.SerialDataReceivedEventHandler(_port_DataReceived);
			_port.PinChanged += new HSR.IO.Ports.SerialPinChangedEventHandler(_port_PinChanged);
			_port.ErrorReceived += new HSR.IO.Ports.SerialErrorReceivedEventHandler(_port_ErrorReceived);
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

		#endregion

		#region Port Events
		//------------------------------------------------------------------------------------------
		// Port Events
		//------------------------------------------------------------------------------------------

		private void _port_DataReceived(object sender, HSR.IO.Ports.SerialDataReceivedEventArgs e)
		{
			OnDataReceived(new EventArgs());
		}

		private void _port_PinChanged(object sender, HSR.IO.Ports.SerialPinChangedEventArgs e)
		{
			OnIOControlChanged(new EventArgs());
		}

		private void _port_ErrorReceived(object sender, HSR.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			bool fireEvent;
			string message;
			switch (e.EventType)
			{
				case System.IO.Ports.SerialError.Frame:    fireEvent = false; message = "Serial communication framing error!";            break;
				case System.IO.Ports.SerialError.Overrun:  fireEvent = true;  message = "Serial communication character buffer overrun!"; break;
				case System.IO.Ports.SerialError.RXOver:   fireEvent = true;  message = "Serial communication input buffer overflow!";    break;
				case System.IO.Ports.SerialError.RXParity: fireEvent = true;  message = "Serial communication parity error!";             break;
				case System.IO.Ports.SerialError.TXFull:   fireEvent = true;  message = "Serial communication output buffer full!";       break;
				default:                                   fireEvent = true;  message = "Serial communication error!";                    break;
			}

			// then post error
			if (fireEvent)
				OnIOError(new IOErrorEventArgs(message));
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnIOChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(IOChanged, this, e);
		}

		protected virtual void OnIOControlChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(IOControlChanged, this, e);
		}

		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			Utilities.Event.EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		protected virtual void OnDataReceived(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(DataReceived, this, e);
		}

		protected virtual void OnDataSent(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(DataSent, this, e);
		}

		#endregion
	}
}
