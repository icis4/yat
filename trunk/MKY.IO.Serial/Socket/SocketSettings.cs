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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// The MKY.IO.Serial namespace combines serial port and socket infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\Socket for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	[Serializable]
	public class SocketSettings : MKY.Utilities.Settings.Settings, IEquatable<SocketSettings>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const string DefaultRemoteHostName = "localhost";
		/// <summary></summary>
		public const string DefaultLocalHostName = "<Any>";
		/// <summary></summary>
		public const int DefaultPort = 10000;
		/// <summary></summary>
		public static readonly AutoRetry TcpClientAutoReconnectDefault = new AutoRetry(false, 500);

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SocketHostType _hostType;

		private string _remoteHostNameOrAddress;
		private System.Net.IPAddress _resolvedRemoteIPAddress;
		private int _remotePort;

		private string _localHostNameOrAddress;
		private System.Net.IPAddress _resolvedLocalIPAddress;
		private int _localTcpPort;
		private int _localUdpPort;

		private AutoRetry _tcpClientAutoReconnect;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SocketSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SocketSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public SocketSettings(SocketSettings rhs)
			: base(rhs)
		{
			_hostType                = rhs.HostType;

			_remoteHostNameOrAddress = rhs.RemoteHostNameOrAddress;
			_resolvedRemoteIPAddress = rhs.ResolvedRemoteIPAddress;
			_remotePort              = rhs.RemotePort;

			_localHostNameOrAddress  = rhs.LocalHostNameOrAddress;
			_resolvedLocalIPAddress  = rhs.ResolvedLocalIPAddress;
			_localTcpPort            = rhs.LocalTcpPort;
			_localUdpPort            = rhs.LocalUdpPort;

			_tcpClientAutoReconnect  = rhs.TcpClientAutoReconnect;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			HostType                = SocketHostType.TcpAutoSocket;

			RemoteHostNameOrAddress = DefaultRemoteHostName;
			ResolvedRemoteIPAddress = System.Net.IPAddress.Loopback;
			RemotePort              = DefaultPort;

			LocalHostNameOrAddress  = DefaultLocalHostName;
			ResolvedLocalIPAddress  = System.Net.IPAddress.Any;
			LocalTcpPort            = DefaultPort;
			LocalUdpPort            = DefaultPort + 1;

			TcpClientAutoReconnect  = new AutoRetry(false, 500);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("HostType")]
		public SocketHostType HostType
		{
			get { return (_hostType); }
			set
			{
				if (_hostType != value)
				{
					_hostType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RemoteHostNameOrAddress")]
		public string RemoteHostNameOrAddress
		{
			get { return (_remoteHostNameOrAddress); }
			set
			{
				if (_remoteHostNameOrAddress != value)
				{
					_remoteHostNameOrAddress = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public System.Net.IPAddress ResolvedRemoteIPAddress
		{
			get { return (_resolvedRemoteIPAddress); }
			set { _resolvedRemoteIPAddress = value;  }
		}

		/// <summary></summary>
		[XmlElement("RemotePort")]
		public int RemotePort
		{
			get { return (_remotePort); }
			set
			{
				if (_remotePort != value)
				{
					_remotePort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LocalHostNameOrAddress")]
		public string LocalHostNameOrAddress
		{
			get { return (_localHostNameOrAddress); }
			set
			{
				if (_localHostNameOrAddress != value)
				{
					_localHostNameOrAddress = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public System.Net.IPAddress ResolvedLocalIPAddress
		{
			get { return (_resolvedLocalIPAddress); }
			set { _resolvedLocalIPAddress = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public int LocalPort
		{
			get
			{
				switch (_hostType)
				{
					case SocketHostType.TcpClient:
					case SocketHostType.TcpServer:
					case SocketHostType.TcpAutoSocket:
						return (LocalTcpPort);

					case SocketHostType.Udp:
						return (LocalUdpPort);

					default:
						return (0);
				}
			}
			set
			{
				switch (_hostType)
				{
					case SocketHostType.TcpClient:
					case SocketHostType.TcpServer:
					case SocketHostType.TcpAutoSocket:
						LocalTcpPort = value;
						break;

					case SocketHostType.Udp:
						LocalUdpPort = value;
						break;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LocalTcpPort")]
		public int LocalTcpPort
		{
			get { return (_localTcpPort); }
			set
			{
				if (_localTcpPort != value)
				{
					_localTcpPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LocalUdpPort")]
		public int LocalUdpPort
		{
			get { return (_localUdpPort); }
			set
			{
				if (_localUdpPort != value)
				{
					_localUdpPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TcpClientAutoReconnect")]
		public AutoRetry TcpClientAutoReconnect
		{
			get { return (_tcpClientAutoReconnect); }
			set
			{
				if (_tcpClientAutoReconnect != value)
				{
					_tcpClientAutoReconnect = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Tries to resolve the IP address from <see cref="RemoteHostNameOrAddress"/> and
		/// stores it in <see cref="ResolvedRemoteIPAddress"/>
		/// </summary>
		/// <returns>
		/// true if successfully resolved; false otherwise
		/// </returns>
		public bool TryResolveIPAddresses()
		{
			try
			{
				System.Net.IPAddress[] ipAddresses;

				ipAddresses = System.Net.Dns.GetHostAddresses(_remoteHostNameOrAddress);
				_resolvedRemoteIPAddress = ipAddresses[0];

				ipAddresses = System.Net.Dns.GetHostAddresses(_localHostNameOrAddress);
				_resolvedLocalIPAddress = ipAddresses[0];

				return (true);
			}
			catch
			{
				return (false);
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SocketSettings)
				return (Equals((SocketSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SocketSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_hostType.Equals(value._hostType) &&
					_remoteHostNameOrAddress.Equals(value._remoteHostNameOrAddress) &&
					_remotePort.Equals(value._remotePort) &&
					_localHostNameOrAddress.Equals(value._localHostNameOrAddress) &&
					_localTcpPort.Equals(value._localTcpPort) &&
					_localUdpPort.Equals(value._localUdpPort) &&
					_tcpClientAutoReconnect.Equals(value._tcpClientAutoReconnect)
					);
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary></summary>
		public override string ToString()
		{
			return
			  (
			  ((XSocketHostType)_hostType).ToString() + ", " +
			  _remoteHostNameOrAddress + ", " +
			  _remotePort.ToString() + ", " +
			  _localHostNameOrAddress + ", " +
			  _localTcpPort.ToString() + ", " +
			  _localUdpPort.ToString() + ", " +
			  _tcpClientAutoReconnect.ToString()
			  );
		}

		#endregion

		#region Comparison Operators
		//==========================================================================================
		// Comparison Operators
		//==========================================================================================

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SocketSettings lhs, SocketSettings rhs)
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
		public static bool operator !=(SocketSettings lhs, SocketSettings rhs)
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
