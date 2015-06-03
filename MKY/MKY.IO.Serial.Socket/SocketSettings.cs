//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2007-2015 Matthias Kläy.
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
		/// Must be implemented as property that creates a new id object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same id object.
		/// </remarks>
		public static AutoRetry TcpClientAutoReconnectDefault
		{
			get { return (new AutoRetry(false, 500)); }
		}

		/// <summary></summary>
		public const int TcpClientAutoReconnectMinimumInterval = 100;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SocketHostType hostType;

		private string remoteHost;
		private IPAddress resolvedRemoteIPAddress;
		private int remoteTcpPort;
		private int remoteUdpPort;

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
		public SocketSettings(SocketHostType hostType, string remoteHost, int remoteTcpPort, int remoteUdpPort, string localInterface, int localTcpPort, int localUdpPort, AutoRetry tcpClientAutoReconnect)
		{
			HostType               = hostType;

			RemoteHost             = remoteHost;
			RemoteTcpPort          = remoteTcpPort;
			RemoteUdpPort          = remoteUdpPort;

			LocalInterface         = localInterface;
			LocalTcpPort           = localTcpPort;
			LocalUdpPort           = localUdpPort;

			TcpClientAutoReconnect = tcpClientAutoReconnect;

			ClearChanged();
		}

		/// <summary></summary>
		public SocketSettings(MKY.Settings.SettingsType settingsType)
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
			HostType                = rhs.HostType;

			RemoteHost              = rhs.RemoteHost;
			ResolvedRemoteIPAddress = rhs.ResolvedRemoteIPAddress;
			RemoteTcpPort           = rhs.RemoteTcpPort;
			RemoteUdpPort           = rhs.RemoteUdpPort;

			LocalInterface          = rhs.LocalInterface;
			ResolvedLocalIPAddress  = rhs.ResolvedLocalIPAddress;
			LocalTcpPort            = rhs.LocalTcpPort;
			LocalUdpPort            = rhs.LocalUdpPort;

			TcpClientAutoReconnect  = rhs.TcpClientAutoReconnect;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			HostType                = SocketHostType.TcpAutoSocket;

			RemoteHost              = DefaultRemoteHost;
			ResolvedRemoteIPAddress = DefaultResolvedRemoteIPAddress;
			RemoteTcpPort           = DefaultRemoteTcpPort;
			RemoteUdpPort           = DefaultRemoteUdpPort;

			LocalInterface          = DefaultLocalInterface;
			ResolvedLocalIPAddress  = DefaultResolvedLocalIPAddress;
			LocalTcpPort            = DefaultLocalTcpPort;
			LocalUdpPort            = DefaultLocalUdpPort;

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
				if (this.hostType != value)
				{
					this.hostType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
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
					this.resolvedRemoteIPAddress = IPResolver.ResolveRemoteHost(this.remoteHost);
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
		[XmlIgnore]
		public virtual int RemotePort
		{
			get
			{
				switch (this.hostType)
				{
					case SocketHostType.TcpClient:
					case SocketHostType.TcpServer:
					case SocketHostType.TcpAutoSocket:
						return (RemoteTcpPort);

					case SocketHostType.Udp:
						return (RemoteUdpPort);

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
						RemoteTcpPort = value;
						break;

					case SocketHostType.Udp:
						RemoteUdpPort = value;
						break;
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
					this.resolvedLocalIPAddress = IPResolver.ResolveLocalInterface(this.localInterface);
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

				(HostType                                     == other.HostType) &&
				StringEx.EqualsOrdinalIgnoreCase(RemoteHost,     other.RemoteHost) &&
				(RemoteTcpPort                                == other.RemoteTcpPort) &&
				(RemoteUdpPort                                == other.RemoteUdpPort) &&
				StringEx.EqualsOrdinalIgnoreCase(LocalInterface, other.LocalInterface) &&
				(LocalTcpPort                                 == other.LocalTcpPort) &&
				(LocalUdpPort                                 == other.LocalUdpPort) &&
				(TcpClientAutoReconnect                       == other.TcpClientAutoReconnect)
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
				base.GetHashCode() ^

				this.hostType              .GetHashCode() ^
				this.remoteHost            .GetHashCode() ^
				this.remoteTcpPort         .GetHashCode() ^
				this.remoteUdpPort         .GetHashCode() ^
				this.localInterface        .GetHashCode() ^
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
				((SocketHostTypeEx)this.hostType) + ", " +
				this.remoteHost                   + ", " +
				this.remoteTcpPort                + ", " +
				this.remoteUdpPort                + ", " +
				this.localInterface               + ", " +
				this.localTcpPort                 + ", " +
				this.localUdpPort                 + ", " +
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
			if (sa.Length == 5)
			{
				SocketHostTypeEx hostType;
				if (SocketHostTypeEx.TryParse(sa[0], out hostType))
				{
					string remoteHost = sa[1].Trim();

					int remoteTcpPort;
					if (int.TryParse(sa[2], out remoteTcpPort))
					{
						int remoteUdpPort;
						if (int.TryParse(sa[3], out remoteUdpPort))
						{
							string localInterface = sa[4].Trim();

							int localTcpPort;
							if (int.TryParse(sa[5], out localTcpPort))
							{
								int localUdpPort;
								if (int.TryParse(sa[6], out localUdpPort))
								{
									bool arEnabled;
									if (bool.TryParse(sa[7], out arEnabled))
									{
										int arInterval;
										if (int.TryParse(sa[8], out arInterval))
										{
											AutoRetry ar = new AutoRetry(arEnabled, arInterval);
											settings = new SocketSettings(hostType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localTcpPort, localUdpPort, ar);
											return (true);
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
			switch (hostType)
			{
				case SocketHostType.TcpClient:     return (                                         IPHost.ToUrlString(this.remoteHost) + ":" + this.remoteTcpPort);
				case SocketHostType.TcpServer:     return ("Server:"  + this.localTcpPort.ToString(CultureInfo.InvariantCulture)                                  );
				case SocketHostType.TcpAutoSocket: return ("Server:"  + this.localTcpPort + " / " + IPHost.ToUrlString(this.remoteHost) + ":" + this.remoteTcpPort);
				case SocketHostType.Udp:           return ("Receive:" + this.localUdpPort + " / " + IPHost.ToUrlString(this.remoteHost) + ":" + this.remoteUdpPort);

				default:                           return (Undefined);
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
