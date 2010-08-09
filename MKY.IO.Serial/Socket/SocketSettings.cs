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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Diagnostics;
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
		public static readonly XIPHost DefaultRemoteHost = new XIPHost(IPHostType.Localhost);

		/// <summary></summary>
		public static readonly IPAddress DefaultResolvedRemoteIPAddress = IPAddress.Loopback;

		/// <summary></summary>
		public static readonly IPNetworkInterface DefaultLocalInterface = new IPNetworkInterface(IPNetworkInterfaceType.Any);

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

		private SocketHostType hostType;

		private string remoteHost;
		private IPAddress resolvedRemoteIPAddress;
		private int remotePort;

		private string localInterface;
		private IPAddress resolvedLocalIPAddress;
		private int localTcpPort;
		private int localUdpPort;

		private AutoRetry tcpClientAutoReconnect;

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
			this.hostType                = rhs.HostType;

			this.remoteHost              = rhs.RemoteHost;
			this.resolvedRemoteIPAddress = rhs.ResolvedRemoteIPAddress;
			this.remotePort              = rhs.RemotePort;

			this.localInterface          = rhs.LocalInterface;
			this.resolvedLocalIPAddress  = rhs.ResolvedLocalIPAddress;
			this.localTcpPort            = rhs.LocalTcpPort;
			this.localUdpPort            = rhs.LocalUdpPort;

			this.tcpClientAutoReconnect  = rhs.TcpClientAutoReconnect;

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
			get { return (this.hostType); }
			set
			{
				if (value != this.hostType)
				{
					this.hostType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RemoteHost")]
		public virtual string RemoteHost
		{
			get { return (this.remoteHost); }
			set
			{
				if (value != this.remoteHost)
				{
					this.remoteHost = value;
					SetChanged();

					// Immediately try to resolve the corresponding remote IP address.
					XIPHost ipHost;
					if (XIPHost.TryParse(this.remoteHost, out ipHost))
					{
						switch ((IPHostType)ipHost)
						{
							case IPHostType.Localhost:
							case IPHostType.IPv4Localhost:
							case IPHostType.IPv6Localhost:
							{
								this.resolvedRemoteIPAddress = ipHost.IPAddress;
								break;
							}

							case IPHostType.Other:
							{
								try
								{
									IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses(this.remoteHost);
									this.resolvedRemoteIPAddress = ipAddresses[0];
								}
								catch (Exception ex)
								{
									XDebug.WriteException(this, ex);
								}
								break;
							}

							default:
							{
								throw (new ArgumentOutOfRangeException("IP Host Type", (IPHostType)ipHost, "Unknown IP host type"));
							}
						}
					}
					else
					{
						this.resolvedRemoteIPAddress = IPAddress.None;
					}
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPAddress ResolvedRemoteIPAddress
		{
			get { return (this.resolvedRemoteIPAddress); }
			set { this.resolvedRemoteIPAddress = value;  }
		}

		/// <summary></summary>
		[XmlElement("RemotePort")]
		public virtual int RemotePort
		{
			get { return (this.remotePort); }
			set
			{
				if (value != this.remotePort)
				{
					this.remotePort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LocalInterface")]
		public virtual string LocalInterface
		{
			get { return (this.localInterface); }
			set
			{
				if (value != this.localInterface)
				{
					this.localInterface = value;
					SetChanged();

					// Immediately try to resolve the corresponding local IP address.
					IPNetworkInterface networkInterface;
					if (IPNetworkInterface.TryParse(this.localInterface, out networkInterface))
					{
						switch ((IPNetworkInterfaceType)networkInterface)
						{
							case IPNetworkInterfaceType.Any:
							case IPNetworkInterfaceType.IPv4Any:
							case IPNetworkInterfaceType.IPv4Loopback:
							case IPNetworkInterfaceType.IPv6Any:
							case IPNetworkInterfaceType.IPv6Loopback:
							{
								this.resolvedLocalIPAddress = networkInterface.IPAddress;
								break;
							}

							case IPNetworkInterfaceType.Other:
							{
								try
								{
									IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses(this.localInterface);
									this.resolvedLocalIPAddress = ipAddresses[0];
								}
								catch (Exception ex)
								{
									XDebug.WriteException(this, ex);
								}
								break;
							}

							default:
							{
								throw (new ArgumentOutOfRangeException("IP Network Interface", (IPNetworkInterfaceType)networkInterface, "Unknown network interface type"));
							}
						}
					}
					else
					{
						this.resolvedLocalIPAddress = IPAddress.None;
					}
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPAddress ResolvedLocalIPAddress
		{
			get { return (this.resolvedLocalIPAddress); }
			set { this.resolvedLocalIPAddress = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int LocalPort
		{
			get
			{
				switch (this.hostType)
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
				switch (this.hostType)
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
			get { return (this.localTcpPort); }
			set
			{
				if (value != this.localTcpPort)
				{
					this.localTcpPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LocalUdpPort")]
		public virtual int LocalUdpPort
		{
			get { return (this.localUdpPort); }
			set
			{
				if (value != this.localUdpPort)
				{
					this.localUdpPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TcpClientAutoReconnect")]
		public virtual AutoRetry TcpClientAutoReconnect
		{
			get { return (this.tcpClientAutoReconnect); }
			set
			{
				if (value != this.tcpClientAutoReconnect)
				{
					this.tcpClientAutoReconnect = value;
					SetChanged();
				}
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
			if (obj == null)
				return (false);

			SocketSettings casted = obj as SocketSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SocketSettings casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.

				(this.hostType               == casted.hostType) &&
				(this.remoteHost             == casted.remoteHost) &&
				(this.remotePort             == casted.remotePort) &&
				(this.localInterface         == casted.localInterface) &&
				(this.localTcpPort           == casted.localTcpPort) &&
				(this.localUdpPort           == casted.localUdpPort) &&
				(this.tcpClientAutoReconnect == casted.tcpClientAutoReconnect)
			);
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
				((XSocketHostType)this.hostType) + ", " +
				this.remoteHost                  + ", " +
				this.remotePort                  + ", " +
				this.localInterface              + ", " +
				this.localTcpPort                + ", " +
				this.localUdpPort                + ", " +
				this.tcpClientAutoReconnect
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
