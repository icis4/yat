using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings
{
	public class IOSettings : Utilities.Settings.Settings
	{
		private Domain.IOType _ioType;
		private SerialPort.SerialPortSettings _serialPort;
		private Socket.SocketSettings _socket;

		public IOSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public IOSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public IOSettings(IOSettings rhs)
			: base(rhs)
		{
			IOType = rhs.IOType;
			SerialPort = new SerialPort.SerialPortSettings(rhs.SerialPort);
			Socket = new Socket.SocketSettings(rhs.Socket);
			ClearChanged();
		}

		private void InitializeNodes()
		{
			SerialPort = new SerialPort.SerialPortSettings(SettingsType);
			Socket = new Socket.SocketSettings(SettingsType);
		}

		protected override void SetMyDefaults()
		{
			IOType = IOType.SerialPort;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("IOType")]
		public IOType IOType
		{
			get { return (_ioType); }
			set
			{
				if (_ioType != value)
				{
					_ioType = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SerialPort")]
		public SerialPort.SerialPortSettings SerialPort
		{
			get { return (_serialPort); }
			set
			{
				if (_serialPort == null)
				{
					_serialPort = value;
					AttachNode(_serialPort);
				}
				else if (_serialPort != value)
				{
					SerialPort.SerialPortSettings old = _serialPort;
					_serialPort = value;
					ReplaceNode(old, _serialPort);
				}
			}
		}

		[XmlElement("Socket")]
		public Socket.SocketSettings Socket
		{
			get { return (_socket); }
			set
			{
				if (_socket == null)
				{
					_socket = value;
					AttachNode(_socket);
				}
				else if (_socket != value)
				{
					Socket.SocketSettings old = _socket;
					_socket = value;
					ReplaceNode(old, _socket);
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is IOSettings)
				return (Equals((IOSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(IOSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return 
					(
					_ioType.Equals(value._ioType) &&
					base.Equals((Utilities.Settings.Settings)value) // compares all settings nodes
					);
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
		/// </summary>
		public static bool operator ==(IOSettings lhs, IOSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(IOSettings lhs, IOSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
