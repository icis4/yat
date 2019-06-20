using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Gui.Settings
{
	public class NewTerminalSettings : Utilities.Settings.Settings
	{
		private Domain.TerminalType _terminalType;
		private Domain.IOType _ioType;

		private IO.Ports.SerialPortId _serialPortId;

		private string _socketRemoteHostNameOrAddress;
		private int _socketRemotePort;

		private string _socketLocalHostNameOrAddress;
		private int _socketLocalTcpPort;
		private int _socketLocalUdpPort;

		private bool _openTerminal;

		public NewTerminalSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public NewTerminalSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public NewTerminalSettings(NewTerminalSettings rhs)
			: base(rhs)
		{
			TerminalType = rhs.TerminalType;
			IOType       = rhs.IOType;

			SerialPortId = rhs.SerialPortId;

			SocketRemoteHostNameOrAddress = rhs.SocketRemoteHostNameOrAddress;
			SocketRemotePort              = rhs.SocketRemotePort;

			SocketLocalHostNameOrAddress  = rhs.SocketLocalHostNameOrAddress;
			SocketLocalTcpPort            = rhs.SocketLocalTcpPort;
			SocketLocalUdpPort            = rhs.SocketLocalUdpPort;

			OpenTerminal = rhs.OpenTerminal;

			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			TerminalType = Domain.TerminalType.Text;
			IOType       = Domain.IOType.SerialPort;

			SerialPortId = IO.Ports.SerialPortId.DefaultPort;

			SocketRemoteHostNameOrAddress = Domain.Settings.Socket.SocketSettings.DefaultRemoteHostName;
			SocketRemotePort              = Domain.Settings.Socket.SocketSettings.DefaultPort;

			SocketLocalHostNameOrAddress  = Domain.Settings.Socket.SocketSettings.DefaultLocalHostName;
			SocketLocalTcpPort            = Domain.Settings.Socket.SocketSettings.DefaultPort;
			SocketLocalUdpPort            = Domain.Settings.Socket.SocketSettings.DefaultPort + 1;

			OpenTerminal = true;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("TerminalType")]
		public Domain.TerminalType TerminalType
		{
			get { return (_terminalType); }
			set
			{
				if (_terminalType != value)
				{
					_terminalType = value;
					SetChanged();
				}
			}
		}

		[XmlElement("IOType")]
		public Domain.IOType IOType
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

		[XmlElement("SerialPortId")]
		public HSR.IO.Ports.SerialPortId SerialPortId
		{
			get { return (_serialPortId); }
			set
			{
				if (_serialPortId != value)
				{
					_serialPortId = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SocketRemoteHostNameOrAddress")]
		public string SocketRemoteHostNameOrAddress
		{
			get { return (_socketRemoteHostNameOrAddress); }
			set
			{
				if (_socketRemoteHostNameOrAddress != value)
				{
					_socketRemoteHostNameOrAddress = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SocketRemotePort")]
		public int SocketRemotePort
		{
			get { return (_socketRemotePort); }
			set
			{
				if (_socketRemotePort != value)
				{
					_socketRemotePort = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SocketLocalHostNameOrAddress")]
		public string SocketLocalHostNameOrAddress
		{
			get { return (_socketLocalHostNameOrAddress); }
			set
			{
				if (_socketLocalHostNameOrAddress != value)
				{
					_socketLocalHostNameOrAddress = value;
					SetChanged();
				}
			}
		}

		[XmlIgnore]
		public int SocketLocalPort
		{
			get
			{
				switch (_ioType)
				{
					case Domain.IOType.TcpClient:
					case Domain.IOType.TcpServer:
					case Domain.IOType.TcpAutoSocket:
						return (SocketLocalTcpPort);

					case Domain.IOType.Udp:
						return (SocketLocalUdpPort);

					default:
						return (0);
				}
			}
			set
			{
				switch (_ioType)
				{
					case Domain.IOType.TcpClient:
					case Domain.IOType.TcpServer:
					case Domain.IOType.TcpAutoSocket:
						SocketLocalTcpPort = value;
						break;

					case Domain.IOType.Udp:
						SocketLocalUdpPort = value;
						break;
				}
			}
		}

		[XmlElement("SocketLocalTcpPort")]
		public int SocketLocalTcpPort
		{
			get { return (_socketLocalTcpPort); }
			set
			{
				if (_socketLocalTcpPort != value)
				{
					_socketLocalTcpPort = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SocketLocalUdpPort")]
		public int SocketLocalUdpPort
		{
			get { return (_socketLocalUdpPort); }
			set
			{
				if (_socketLocalUdpPort != value)
				{
					_socketLocalUdpPort = value;
					SetChanged();
				}
			}
		}

		[XmlElement("OpenTerminal")]
		public bool OpenTerminal
		{
			get { return (_openTerminal); }
			set
			{
				if (_openTerminal != value)
				{
					_openTerminal = value;
					SetChanged();
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
			if (obj is NewTerminalSettings)
				return (Equals((NewTerminalSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(NewTerminalSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_terminalType.Equals    (value._terminalType) &&
					_ioType.Equals          (value._ioType) &&
					_serialPortId.Equals    (value._serialPortId) &&
					_socketRemoteHostNameOrAddress.Equals(value._socketRemoteHostNameOrAddress) &&
					_socketRemotePort.Equals(value._socketRemotePort) &&
					_socketLocalHostNameOrAddress.Equals(value._socketLocalHostNameOrAddress) &&
					_socketLocalTcpPort.Equals(value._socketLocalTcpPort) &&
					_socketLocalUdpPort.Equals(value._socketLocalUdpPort) &&

					_openTerminal.Equals    (value._openTerminal)
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
		public static bool operator ==(NewTerminalSettings lhs, NewTerminalSettings rhs)
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
		public static bool operator !=(NewTerminalSettings lhs, NewTerminalSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}