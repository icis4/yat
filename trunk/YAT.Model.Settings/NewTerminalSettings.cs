//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class NewTerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<NewTerminalSettings>
	{
		private Domain.TerminalType _terminalType;
		private Domain.IOType _ioType;

		private MKY.IO.Ports.SerialPortId _serialPortId;

		private string _socketRemoteHostNameOrAddress;
		private int _socketRemotePort;

		private string _socketLocalHostNameOrAddress;
		private int _socketLocalTcpPort;
		private int _socketLocalUdpPort;

		private bool _startTerminal;

		/// <summary></summary>
		public NewTerminalSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public NewTerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public NewTerminalSettings(NewTerminalSettings rhs)
			: base(rhs)
		{
			_terminalType = rhs.TerminalType;
			_ioType       = rhs.IOType;

			_serialPortId = rhs.SerialPortId;

			_socketRemoteHostNameOrAddress = rhs.SocketRemoteHostNameOrAddress;
			_socketRemotePort              = rhs.SocketRemotePort;

			_socketLocalHostNameOrAddress  = rhs.SocketLocalHostNameOrAddress;
			_socketLocalTcpPort            = rhs.SocketLocalTcpPort;
			_socketLocalUdpPort            = rhs.SocketLocalUdpPort;

			_startTerminal = rhs.StartTerminal;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalType = Domain.TerminalType.Text;
			IOType       = Domain.IOType.SerialPort;

			SerialPortId = MKY.IO.Ports.SerialPortId.DefaultPort;

			SocketRemoteHostNameOrAddress = MKY.IO.Serial.SocketSettings.DefaultRemoteHostName;
			SocketRemotePort              = MKY.IO.Serial.SocketSettings.DefaultPort;

			SocketLocalHostNameOrAddress  = MKY.IO.Serial.SocketSettings.DefaultLocalHostName;
			SocketLocalTcpPort            = MKY.IO.Serial.SocketSettings.DefaultPort;
			SocketLocalUdpPort            = MKY.IO.Serial.SocketSettings.DefaultPort + 1;

			StartTerminal = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
		[XmlElement("SerialPortId")]
		public MKY.IO.Ports.SerialPortId SerialPortId
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
		[XmlElement("StartTerminal")]
		public bool StartTerminal
		{
			get { return (_startTerminal); }
			set
			{
				if (_startTerminal != value)
				{
					_startTerminal = value;
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
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_terminalType.Equals      (value._terminalType) &&
					_ioType.Equals            (value._ioType) &&
					_serialPortId.Equals      (value._serialPortId) &&
					_socketRemoteHostNameOrAddress.Equals(value._socketRemoteHostNameOrAddress) &&
					_socketRemotePort.Equals  (value._socketRemotePort) &&
					_socketLocalHostNameOrAddress.Equals(value._socketLocalHostNameOrAddress) &&
					_socketLocalTcpPort.Equals(value._socketLocalTcpPort) &&
					_socketLocalUdpPort.Equals(value._socketLocalUdpPort) &&
					_startTerminal.Equals     (value._startTerminal)
					);
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
