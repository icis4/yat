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
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private Settings.SerialPort.SerialPortSettings _settings;
		private MKY.IO.Ports.ISerialPort _port;

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

				OnIOChanged(new EventArgs());
				OnIOControlChanged(new EventArgs());
			}
		}

		/// <summary></summary>
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

		#region Port
		//==========================================================================================
		// Port
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

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		private void _port_DataReceived(object sender, MKY.IO.Ports.SerialDataReceivedEventArgs e)
		{
			OnDataReceived(new EventArgs());
		}

		private void _port_PinChanged(object sender, MKY.IO.Ports.SerialPinChangedEventArgs e)
		{
			OnIOControlChanged(new EventArgs());
		}

		private void _port_ErrorReceived(object sender, MKY.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			string message;
			switch (e.EventType)
			{
				case System.IO.Ports.SerialError.Frame:    message = "Serial communication framing error!";            break;
				case System.IO.Ports.SerialError.Overrun:  message = "Serial communication character buffer overrun!"; break;
				case System.IO.Ports.SerialError.RXOver:   message = "Serial communication input buffer overflow!";    break;
				case System.IO.Ports.SerialError.RXParity: message = "Serial communication parity error!";             break;
				case System.IO.Ports.SerialError.TXFull:   message = "Serial communication output buffer full!";       break;
				default:                                   message = "Unknown serial communication error!";            break;
			}
			OnIOError(new SerialPortIOErrorEventArgs(message, e.EventType));
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
