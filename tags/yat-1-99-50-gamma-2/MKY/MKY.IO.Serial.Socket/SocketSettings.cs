//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Xml.Serialization;

using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	public class SocketSettings : Settings.SettingsItem
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Why not, the .NET framework itself does it everywhere...")]
		public static readonly IPHostEx RemoteHostDefault = new IPHostEx(IPHost.Localhost);

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static IPNetworkInterfaceDescriptorPair LocalInterfaceDefault
		{
			get { return (new IPNetworkInterfaceEx(IPNetworkInterface.Any).ToDescriptorPair()); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Why not, the .NET framework itself does it everywhere...")]
		public static readonly IPFilterEx LocalFilterDefault = new IPFilterEx(IPFilter.Any);

		/// <summary></summary>
		public const int PortDefault = 10000;

		/// <summary></summary>
		public const int RemoteTcpPortDefault = PortDefault;

		/// <summary></summary>
		public const int RemoteUdpPortDefault = PortDefault;

		/// <summary></summary>
		public const int LocalTcpPortDefault = PortDefault;

		/// <summary></summary>
		public const int LocalUdpPortDefault = PortDefault + 1;

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static AutoInterval TcpClientAutoReconnectDefault
		{
			get { return (new AutoInterval(false, 500)); }
		}

		/// <summary></summary>
		public const int TcpClientAutoReconnectMinInterval = 100;

		/// <summary></summary>
		public const UdpServerSendMode UdpServerSendModeDefault = UdpServerSendMode.MostRecent;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SocketType type;

		private IPHostEx remoteHost;
		private int remoteTcpPort;
		private int remoteUdpPort;

		private IPNetworkInterfaceDescriptorPair localInterface;
		private IPFilterEx localFilter;
		private int localTcpPort;
		private int localUdpPort;

		private AutoInterval tcpClientAutoReconnect;
		private UdpServerSendMode udpServerSendMode;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SocketSettings()
			: this(Settings.SettingsType.Explicit)
		{
		}

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SocketSettings(Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SocketSettings(SocketType type, string remoteHost, int remoteTcpPort, int remoteUdpPort, IPNetworkInterfaceDescriptorPair localInterface, string localFilter, int localTcpPort, int localUdpPort, AutoInterval tcpClientAutoReconnect, UdpServerSendMode udpServerSendMode)
		{
			Type           = type;

			RemoteHost     = remoteHost;
			RemoteTcpPort  = remoteTcpPort;
			RemoteUdpPort  = remoteUdpPort;

			LocalInterface = localInterface;
			LocalFilter    = localFilter;
			LocalTcpPort   = localTcpPort;
			LocalUdpPort   = localUdpPort;

			TcpClientAutoReconnect = tcpClientAutoReconnect;
			UdpServerSendMode      = udpServerSendMode;

			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings from <paramref name="rhs"/>.
		/// </summary>
		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SocketSettings(SocketSettings rhs)
			: base(rhs)
		{
			Type           = rhs.Type;

			RemoteHost     = rhs.RemoteHost;
			RemoteTcpPort  = rhs.RemoteTcpPort;
			RemoteUdpPort  = rhs.RemoteUdpPort;

			LocalInterface = rhs.LocalInterface;
			LocalFilter    = rhs.LocalFilter;
			LocalTcpPort   = rhs.LocalTcpPort;
			LocalUdpPort   = rhs.LocalUdpPort;

			TcpClientAutoReconnect = rhs.TcpClientAutoReconnect;
			UdpServerSendMode      = rhs.UdpServerSendMode;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Type           = SocketType.TcpAutoSocket;

			RemoteHost     = RemoteHostDefault;
			RemoteTcpPort  = RemoteTcpPortDefault;
			RemoteUdpPort  = RemoteUdpPortDefault;

			LocalInterface = LocalInterfaceDefault;
			LocalFilter    = LocalFilterDefault;
			LocalTcpPort   = LocalTcpPortDefault;
			LocalUdpPort   = LocalUdpPortDefault;

			TcpClientAutoReconnect = TcpClientAutoReconnectDefault;
			UdpServerSendMode      = UdpServerSendModeDefault;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Well, there are other types than object types...")]
		[XmlElement("Type")]
		public virtual SocketType Type
		{
			get { return (this.type); }
			set
			{
				if (this.type != value)
				{
					this.type = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public virtual IPHostEx RemoteHost
		{
			get { return (this.remoteHost); }
			set
			{
				if (this.remoteHost != value)
				{
					this.remoteHost = value;
					SetMyChanged();

					// Do not try to resolve the IP address as this may take quite some time!
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("RemoteHost")]
		public virtual string RemoteHost_ForSerialization
		{
			get { return (RemoteHost.ToCompactString()); } // Use compact string represenation, only taking host name or address into account!
			set { RemoteHost = value;                    }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int RemotePort
		{
			get
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						return (RemoteTcpPort);

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						return (RemoteUdpPort);

					default:
						return (0);
				}
			}
			set
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						RemoteTcpPort = value;
						break;

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						RemoteUdpPort = value;
						break;

					default:
						break; // Do nothing.
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RemoteTcpPort")]
		public virtual int RemoteTcpPort
		{
			get { return (this.remoteTcpPort); }
			set
			{
				if (this.remoteTcpPort != value)
				{
					this.remoteTcpPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RemoteUdpPort")]
		public virtual int RemoteUdpPort
		{
			get { return (this.remoteUdpPort); }
			set
			{
				if (this.remoteUdpPort != value)
				{
					this.remoteUdpPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RemoteEndPointString
		{
			get { return (this.remoteHost.ToEndpointAddressString() + ":" + RemotePort); }
		}

		/// <summary></summary>
		[XmlElement("LocalInterface")]
		public virtual IPNetworkInterfaceDescriptorPair LocalInterface
		{
			get { return (this.localInterface); }
			set
			{
				if (this.localInterface != value)
				{
					this.localInterface = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public virtual IPFilterEx LocalFilter
		{
			get { return (this.localFilter); }
			set
			{
				if (this.localFilter != value)
				{
					this.localFilter = value;
					SetMyChanged();

					// Do not try to resolve the IP address as this may take quite some time!
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("LocalFilter")]
		public virtual string LocalFilter_ForSerialization
		{
			get { return (LocalFilter.ToCompactString()); } // Use compact string represenation, only taking host name or address into account!
			set { LocalFilter = value;                    }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int LocalPort
		{
			get
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						return (LocalTcpPort);

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						return (LocalUdpPort);

					default:
						return (0);
				}
			}
			set
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						LocalTcpPort = value;
						break;

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						LocalUdpPort = value;
						break;

					default:
						break; // Do nothing.
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
				if (this.localTcpPort != value)
				{
					this.localTcpPort = value;
					SetMyChanged();
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
				if (this.localUdpPort != value)
				{
					this.localUdpPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TcpClientAutoReconnect")]
		public virtual AutoInterval TcpClientAutoReconnect
		{
			get { return (this.tcpClientAutoReconnect); }
			set
			{
				if (this.tcpClientAutoReconnect != value)
				{
					this.tcpClientAutoReconnect = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UdpServerSendMode")]
		public virtual UdpServerSendMode UdpServerSendMode
		{
			get { return (this.udpServerSendMode); }
			set
			{
				if (this.udpServerSendMode != value)
				{
					this.udpServerSendMode = value;
					SetMyChanged();
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
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SocketSettings other = (SocketSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(Type                                                       == other.Type) &&
				StringEx.EqualsOrdinalIgnoreCase( RemoteHost_ForSerialization, other.RemoteHost_ForSerialization) &&
				(RemoteTcpPort                                              == other.RemoteTcpPort) &&
				(RemoteUdpPort                                              == other.RemoteUdpPort) &&
				(LocalInterface                                             == other.LocalInterface) &&
				StringEx.EqualsOrdinalIgnoreCase(LocalFilter_ForSerialization, other.LocalFilter_ForSerialization) &&
				(LocalTcpPort                                               == other.LocalTcpPort) &&
				(LocalUdpPort                                               == other.LocalUdpPort) &&
				(TcpClientAutoReconnect                                     == other.TcpClientAutoReconnect) &&
				(UdpServerSendMode                                          == other.UdpServerSendMode)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^  Type                                                               .GetHashCode();
				hashCode = (hashCode * 397) ^ ( RemoteHost_ForSerialization != null ?  RemoteHost_ForSerialization.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  RemoteTcpPort;
				hashCode = (hashCode * 397) ^  RemoteUdpPort;
				hashCode = (hashCode * 397) ^  LocalInterface                                                     .GetHashCode();
				hashCode = (hashCode * 397) ^ (LocalFilter_ForSerialization != null ? LocalFilter_ForSerialization.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  LocalTcpPort;
				hashCode = (hashCode * 397) ^  LocalUdpPort;
				hashCode = (hashCode * 397) ^  TcpClientAutoReconnect                                             .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary></summary>
		public override string ToString()
		{
			return
			(
				((SocketTypeEx)this.type)   + ", " +
				this.remoteHost             + ", " +
				this.remoteTcpPort          + ", " +
				this.remoteUdpPort          + ", " +
				this.localInterface         + ", " +
				this.localFilter            + ", " +
				this.localTcpPort           + ", " +
				this.localUdpPort           + ", " +
				this.tcpClientAutoReconnect
			);
		}

		/// <summary>
		/// Parses <paramref name="s"/> for socket settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SocketSettings Parse(string s)
		{
			SocketSettings result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify valid socket settings."));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for socket settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SocketSettings settings)
		{
			string delimiters = "/,;";
			string[] sa = s.Trim().Split(delimiters.ToCharArray());
			if (sa.Length == 12)
			{
				SocketType socketType;
				if (SocketTypeEx.TryParse(sa[0], out socketType))
				{
					string remoteHost = sa[1].Trim();

					int remoteTcpPort;
					if (int.TryParse(sa[2], out remoteTcpPort))
					{
						int remoteUdpPort;
						if (int.TryParse(sa[3], out remoteUdpPort))
						{
							IPNetworkInterfaceDescriptorPair localInterface = new IPNetworkInterfaceDescriptorPair(sa[4].Trim(), sa[5].Trim());

							string localFilter = sa[6].Trim();

							int localTcpPort;
							if (int.TryParse(sa[7], out localTcpPort))
							{
								int localUdpPort;
								if (int.TryParse(sa[8], out localUdpPort))
								{
									bool arEnabled;
									if (bool.TryParse(sa[9], out arEnabled))
									{
										int arInterval;
										if (int.TryParse(sa[10], out arInterval))
										{
											AutoInterval autoRetry = new AutoInterval(arEnabled, arInterval);

											int smValue;
											if (int.TryParse(sa[11], out smValue))
											{
												UdpServerSendMode sendMode = (UdpServerSendModeEx)smValue;

												settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localFilter, localTcpPort, localUdpPort, autoRetry, sendMode);
												return (true);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			settings = null;
			return (false);
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			switch (type)
			{
				case SocketType.TcpClient:     return (                                         this.remoteHost.ToEndpointAddressString() + ":" + this.remoteTcpPort);
				case SocketType.TcpServer:     return ("Server:"  + this.localTcpPort                                                                         );
				case SocketType.TcpAutoSocket: return ("Server:"  + this.localTcpPort + " / " + this.remoteHost.ToEndpointAddressString() + ":" + this.remoteTcpPort);
				case SocketType.UdpClient:     return (                                         this.remoteHost.ToEndpointAddressString() + ":" + this.remoteUdpPort);
				case SocketType.UdpServer:     return ("Receive:" + this.localUdpPort                                                                         );
				case SocketType.UdpPairSocket: return ("Receive:" + this.localUdpPort + " / " + this.remoteHost.ToEndpointAddressString() + ":" + this.remoteUdpPort);

				default:                       return (Undefined);
			}
		}

		#endregion

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
