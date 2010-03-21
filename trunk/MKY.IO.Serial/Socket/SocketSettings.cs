//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Net;

using MKY.Utilities.Net;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
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
		public static readonly XIPHost DefaultRemoteHost = new XIPHost(CommonIPHost.Localhost);
		/// <summary></summary>
		public static readonly IPAddress DefaultResolvedRemoteIPAddress = IPAddress.Loopback;
		/// <summary></summary>
		public static readonly XNetworkInterface DefaultLocalInterface = new XNetworkInterface(CommonNetworkInterface.Any);
		/// <summary></summary>
		public static readonly IPAddress DefaultResolvedLocalIPAddress = IPAddress.Any;

		/// <summary></summary>
		public const int DefaultPort = 10000;
		/// <summary></summary>
		public const int DefaultRemotePort = DefaultPort;
		/// <summary></summary>
		public const int DefaultLocalTcpPort = DefaultPort;
		/// <summary></summary>
		public const int DefaultLocalUdpPort = DefaultPort + 1;

		/// <summary></summary>
		public static readonly AutoRetry TcpClientAutoReconnectDefault = new AutoRetry(false, 500);

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SocketHostType _hostType;

		private string _remoteHost;
		private IPAddress _resolvedRemoteIPAddress;
		private int _remotePort;

		private string _localInterface;
		private IPAddress _resolvedLocalIPAddress;
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

			_remoteHost              = rhs.RemoteHost;
			_resolvedRemoteIPAddress = rhs.ResolvedRemoteIPAddress;
			_remotePort              = rhs.RemotePort;

			_localInterface          = rhs.LocalInterface;
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

			RemoteHost              = DefaultRemoteHost;
			ResolvedRemoteIPAddress = DefaultResolvedRemoteIPAddress;
			RemotePort              = DefaultPort;

			LocalInterface          = DefaultLocalInterface;
			ResolvedLocalIPAddress  = DefaultResolvedLocalIPAddress;
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
		public virtual SocketHostType HostType
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
		[XmlElement("RemoteHost")]
		public virtual string RemoteHost
		{
			get { return (_remoteHost); }
			set
			{
				if (_remoteHost != value)
				{
					_remoteHost = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPAddress ResolvedRemoteIPAddress
		{
			get { return (_resolvedRemoteIPAddress); }
			set { _resolvedRemoteIPAddress = value;  }
		}

		/// <summary></summary>
		[XmlElement("RemotePort")]
		public virtual int RemotePort
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
		[XmlElement("LocalInterface")]
		public virtual string LocalInterface
		{
			get { return (_localInterface); }
			set
			{
				if (_localInterface != value)
				{
					_localInterface = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPAddress ResolvedLocalIPAddress
		{
			get { return (_resolvedLocalIPAddress); }
			set { _resolvedLocalIPAddress = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int LocalPort
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
		public virtual int LocalTcpPort
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
		public virtual int LocalUdpPort
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
		public virtual AutoRetry TcpClientAutoReconnect
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
		/// Tries to resolve the IP address from <see cref="RemoteHost"/> and
		/// stores it in <see cref="ResolvedRemoteIPAddress"/>
		/// </summary>
		/// <returns>
		/// true if successfully resolved; false otherwise
		/// </returns>
		public virtual bool TryResolveIPAddresses()
		{
			try
			{
				IPAddress[] ipAddresses;

				ipAddresses = System.Net.Dns.GetHostAddresses(_remoteHost);
				_resolvedRemoteIPAddress = ipAddresses[0];

				ipAddresses = System.Net.Dns.GetHostAddresses(_localInterface);
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
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_hostType.Equals(value._hostType) &&
					_remoteHost.Equals(value._remoteHost) &&
					_remotePort.Equals(value._remotePort) &&
					_localInterface.Equals(value._localInterface) &&
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
			  _remoteHost + ", " +
			  _remotePort.ToString() + ", " +
			  _localInterface + ", " +
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
