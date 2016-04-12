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
// MKY Development Version 1.0.14
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
	[Serializable]
	public class SocketSettings : MKY.Settings.SettingsItem
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Why not, the .NET framework itself does it everywhere...")]
		public static readonly IPHost DefaultRemoteHost = new IPHost(IPHostType.Localhost);

		/// <summary></summary>
		public static readonly IPAddress DefaultResolvedRemoteIPAddress = IPAddress.Loopback;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Why not, the .NET framework itself does it everywhere...")]
		public static readonly IPNetworkInterface DefaultLocalInterface = new IPNetworkInterface(IPNetworkInterfaceType.Any);

		/// <summary></summary>
		public static readonly IPAddress DefaultResolvedLocalIPAddress = IPAddress.Any;

		/// <summary></summary>
		public static readonly IPAddressFilter DefaultLocalFilter = new IPAddressFilter(IPAddressFilterType.Any);

		/// <summary></summary>
		public static readonly IPAddress DefaultResolvedLocalIPAddressFilter = IPAddress.Any;

		/// <summary></summary>
		public const int DefaultPort = 10000;

		/// <summary></summary>
		public const int DefaultRemoteTcpPort = DefaultPort;

		/// <summary></summary>
		public const int DefaultRemoteUdpPort = DefaultPort;

		/// <summary></summary>
		public const int DefaultLocalTcpPort = DefaultPort;

		/// <summary></summary>
		public const int DefaultLocalUdpPort = DefaultPort + 1;

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static AutoRetry DefaultTcpClientAutoReconnect
		{
			get { return (new AutoRetry(false, 500)); }
		}

		/// <summary></summary>
		public const int TcpClientAutoReconnectMinimumInterval = 100;

		/// <summary></summary>
		public const UdpServerSendMode DefaultUdpServerSendMode = UdpServerSendMode.MostRecent;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SocketType type;

		private string remoteHost;
		private IPAddress resolvedRemoteIPAddress;
		private int remoteTcpPort;
		private int remoteUdpPort;

		private string localInterface;
		private IPAddress resolvedLocalIPAddress;
		private string localFilter;
		private IPAddress resolvedLocalIPAddressFilter;
		private int localTcpPort;
		private int localUdpPort;

		private AutoRetry tcpClientAutoReconnect;
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
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SocketSettings(SocketType type, string remoteHost, int remoteTcpPort, int remoteUdpPort, string localInterface, string localIPAddressFilter, int localTcpPort, int localUdpPort, AutoRetry tcpClientAutoReconnect, UdpServerSendMode udpServerSendMode)
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

		/// <summary></summary>
		public SocketSettings(Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
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

			RemoteHost     = DefaultRemoteHost;
			RemoteTcpPort  = DefaultRemoteTcpPort;
			RemoteUdpPort  = DefaultRemoteUdpPort;

			LocalInterface = DefaultLocalInterface;
			LocalFilter    = DefaultLocalFilter;
			LocalTcpPort   = DefaultLocalTcpPort;
			LocalUdpPort   = DefaultLocalUdpPort;

			TcpClientAutoReconnect = DefaultTcpClientAutoReconnect;
			UdpServerSendMode      = DefaultUdpServerSendMode;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Type")]
		public virtual SocketType Type
		{
			get { return (this.type); }
			set
			{
				if (this.type != value)
				{
					this.type = value;
					SetChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[XmlElement("RemoteHost")]
		public virtual string RemoteHost
		{
			get { return (this.remoteHost); }
			set
			{
				if (this.remoteHost != value)
				{
					this.remoteHost = value;
					SetChanged();

					// Immediately try to resolve the corresponding remote IP address.
					IPAddress ipAddress;
					if (IPResolver.TryResolveRemoteHost(this.remoteHost, out ipAddress))
						this.resolvedRemoteIPAddress = ipAddress;
					else
						this.resolvedRemoteIPAddress = DefaultResolvedRemoteIPAddress;
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPAddress ResolvedRemoteIPAddress
		{
			get { return (this.resolvedRemoteIPAddress); }
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
					SetChanged();
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
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPEndPoint RemoteEndPoint
		{
			get { return (new IPEndPoint(ResolvedRemoteIPAddress, RemotePort)); }
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[XmlElement("LocalInterface")]
		public virtual string LocalInterface
		{
			get { return (this.localInterface); }
			set
			{
				if (this.localInterface != value)
				{
					this.localInterface = value;
					SetChanged();

					// Immediately try to resolve the corresponding local IP address.
					IPAddress ipAddress;
					if (IPResolver.TryResolveRemoteHost(this.localInterface, out ipAddress))
						this.resolvedLocalIPAddress = ipAddress;
					else
						this.resolvedLocalIPAddress = DefaultResolvedLocalIPAddress;
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPAddress ResolvedLocalIPAddress
		{
			get { return (this.resolvedLocalIPAddress); }
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[XmlElement("LocalFilter")]
		public virtual string LocalFilter
		{
			get { return (this.localFilter); }
			set
			{
				if (this.localFilter != value)
				{
					this.localFilter = value;
					SetChanged();

					// Immediately try to resolve the corresponding remote IP address.
					IPAddress ipAddress;
					if (IPResolver.TryResolveRemoteHost(this.localFilter, out ipAddress))
						this.resolvedLocalIPAddressFilter = ipAddress;
					else
						this.resolvedLocalIPAddressFilter = DefaultResolvedLocalIPAddressFilter;
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual IPAddress ResolvedLocalIPAddressFilter
		{
			get { return (this.resolvedLocalIPAddressFilter); }
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
				if (this.localUdpPort != value)
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
				if (this.tcpClientAutoReconnect != value)
				{
					this.tcpClientAutoReconnect = value;
					SetChanged();
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

				(Type                                         == other.Type) &&
				StringEx.EqualsOrdinalIgnoreCase(RemoteHost,     other.RemoteHost) &&
				(RemoteTcpPort                                == other.RemoteTcpPort) &&
				(RemoteUdpPort                                == other.RemoteUdpPort) &&
				StringEx.EqualsOrdinalIgnoreCase(LocalInterface, other.LocalInterface) &&
				StringEx.EqualsOrdinalIgnoreCase(LocalFilter,    other.LocalFilter) &&
				(LocalTcpPort                                 == other.LocalTcpPort) &&
				(LocalUdpPort                                 == other.LocalUdpPort) &&
				(TcpClientAutoReconnect                       == other.TcpClientAutoReconnect) &&
				(UdpServerSendMode                            == other.UdpServerSendMode)
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
			return
			(
				base.GetHashCode() ^ // Get hash code of all settings nodes.

				this.type                  .GetHashCode() ^
				this.remoteHost            .GetHashCode() ^
				this.remoteTcpPort         .GetHashCode() ^
				this.remoteUdpPort         .GetHashCode() ^
				this.localInterface        .GetHashCode() ^
				this.localFilter           .GetHashCode() ^
				this.localTcpPort          .GetHashCode() ^
				this.localUdpPort          .GetHashCode() ^
				this.tcpClientAutoReconnect.GetHashCode()
			);
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
			if (sa.Length == 11)
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
							string localInterface = sa[4].Trim();
							string localIPAddressFilter = sa[5].Trim();

							int localTcpPort;
							if (int.TryParse(sa[6], out localTcpPort))
							{
								int localUdpPort;
								if (int.TryParse(sa[7], out localUdpPort))
								{
									bool arEnabled;
									if (bool.TryParse(sa[8], out arEnabled))
									{
										int arInterval;
										if (int.TryParse(sa[9], out arInterval))
										{
											AutoRetry autoRetry = new AutoRetry(arEnabled, arInterval);

											int smValue;
											if (int.TryParse(sa[10], out smValue))
											{
												UdpServerSendMode sendMode = (UdpServerSendModeEx)smValue;

												settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localIPAddressFilter, localTcpPort, localUdpPort, autoRetry, sendMode);
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
				case SocketType.TcpClient:     return (                                         IPHost.ToUrlString(this.remoteHost) + ":" + this.remoteTcpPort);
				case SocketType.TcpServer:     return ("Server:"  + this.localTcpPort                                                                         );
				case SocketType.TcpAutoSocket: return ("Server:"  + this.localTcpPort + " / " + IPHost.ToUrlString(this.remoteHost) + ":" + this.remoteTcpPort);
				case SocketType.UdpClient:     return (                                         IPHost.ToUrlString(this.remoteHost) + ":" + this.remoteUdpPort);
				case SocketType.UdpServer:     return ("Receive:" + this.localUdpPort                                                                         );
				case SocketType.UdpPairSocket: return ("Receive:" + this.localUdpPort + " / " + IPHost.ToUrlString(this.remoteHost) + ":" + this.remoteUdpPort);

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
