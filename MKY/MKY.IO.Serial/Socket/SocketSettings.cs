//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Xml.Serialization;

using MKY.Diagnostics;
using MKY.Net;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\Socket for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	[Serializable]
	public class SocketSettings : MKY.Settings.SettingsItem
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public static readonly IPHost DefaultRemoteHost = new IPHost(IPHostType.Localhost);

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

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary></summary>
		public static AutoRetry TcpClientAutoReconnectDefault
		{
			// Must be implemented as property that creates a new id object on each call to
			// ensure that there aren't multiple clients referencing (and modifying) the same
			// id object.
			get { return (new AutoRetry(false, 500)); }
		}

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
		public SocketSettings(MKY.Settings.SettingsType settingsType)
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
			HostType                = rhs.HostType;

			RemoteHost              = rhs.RemoteHost;
			ResolvedRemoteIPAddress = rhs.ResolvedRemoteIPAddress;
			RemotePort              = rhs.RemotePort;

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
			HostType                = SocketHostType.TcpAutoSocket;

			RemoteHost              = DefaultRemoteHost;
			ResolvedRemoteIPAddress = DefaultResolvedRemoteIPAddress;
			RemotePort              = DefaultRemotePort;

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
				if (value != this.hostType)
				{
					this.hostType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SocketSettings other = (SocketSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.hostType               == other.hostType) &&
				StringEx.EqualsOrdinalIgnoreCase(this.remoteHost, other.remoteHost) &&
				(this.remotePort             == other.remotePort) &&
				StringEx.EqualsOrdinalIgnoreCase(this.localInterface, other.localInterface) &&
				(this.localTcpPort           == other.localTcpPort) &&
				(this.localUdpPort           == other.localUdpPort) &&
				(this.tcpClientAutoReconnect == other.tcpClientAutoReconnect)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.hostType              .GetHashCode() ^
				this.remoteHost            .GetHashCode() ^
				this.remotePort            .GetHashCode() ^
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

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
