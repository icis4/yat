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
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.System.Net;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class NewTerminalSettings : MKY.System.Settings.Settings
	{
		private Domain.TerminalType terminalType;
		private Domain.IOType ioType;

		private MKY.IO.Ports.SerialPortId serialPortId;

		private XIPHost socketRemoteHost;
		private int socketRemotePort;

		private IPNetworkInterface socketLocalInterface;
		private int socketLocalTcpPort;
		private int socketLocalUdpPort;

		private MKY.IO.Usb.DeviceInfo usbHidDeviceInfo;

		private bool startTerminal;

		/// <summary></summary>
		public NewTerminalSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public NewTerminalSettings(MKY.System.Settings.SettingsType settingsType)
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
			TerminalType         = rhs.TerminalType;
			IOType               = rhs.IOType;

			SerialPortId         = rhs.SerialPortId;

			SocketRemoteHost     = rhs.SocketRemoteHost;
			SocketRemotePort     = rhs.SocketRemotePort;

			SocketLocalInterface = rhs.SocketLocalInterface;
			SocketLocalTcpPort   = rhs.SocketLocalTcpPort;
			SocketLocalUdpPort   = rhs.SocketLocalUdpPort;

			UsbHidDeviceInfo     = rhs.UsbHidDeviceInfo;

			StartTerminal        = rhs.StartTerminal;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// 
		/// Attention: Do not use <see cref="MKY.IO.Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="MKY.IO.Ports.SerialPortId.FirstStandardPort"/>
		/// is way more performant and good enough for most use cases.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalType         = Domain.TerminalType.Text;
			IOType               = Domain.IOType.SerialPort;

			SerialPortId         = MKY.IO.Ports.SerialPortId.FirstStandardPort;

			SocketRemoteHost     = MKY.IO.Serial.SocketSettings.DefaultRemoteHost;
			SocketRemotePort     = MKY.IO.Serial.SocketSettings.DefaultRemotePort;

			SocketLocalInterface = MKY.IO.Serial.SocketSettings.DefaultLocalInterface;
			SocketLocalTcpPort   = MKY.IO.Serial.SocketSettings.DefaultLocalTcpPort;
			SocketLocalUdpPort   = MKY.IO.Serial.SocketSettings.DefaultLocalUdpPort;

			UsbHidDeviceInfo     = null;

			StartTerminal        = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TerminalType")]
		public virtual Domain.TerminalType TerminalType
		{
			get { return (this.terminalType); }
			set
			{
				if (value != this.terminalType)
				{
					this.terminalType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IOType")]
		public virtual Domain.IOType IOType
		{
			get { return (this.ioType); }
			set
			{
				if (value != this.ioType)
				{
					this.ioType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPortId")]
		public virtual MKY.IO.Ports.SerialPortId SerialPortId
		{
			get { return (this.serialPortId); }
			set
			{
				if (value != this.serialPortId)
				{
					this.serialPortId = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SocketRemoteHost")]
		public virtual XIPHost SocketRemoteHost
		{
			get { return (this.socketRemoteHost); }
			set
			{
				if (value != this.socketRemoteHost)
				{
					this.socketRemoteHost = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SocketRemotePort")]
		public virtual int SocketRemotePort
		{
			get { return (this.socketRemotePort); }
			set
			{
				if (value != this.socketRemotePort)
				{
					this.socketRemotePort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SocketLocalInterface")]
		public virtual IPNetworkInterface SocketLocalInterface
		{
			get { return (this.socketLocalInterface); }
			set
			{
				if (value != this.socketLocalInterface)
				{
					this.socketLocalInterface = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int SocketLocalPort
		{
			get
			{
				switch (this.ioType)
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
				switch (this.ioType)
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
		public virtual int SocketLocalTcpPort
		{
			get { return (this.socketLocalTcpPort); }
			set
			{
				if (value != this.socketLocalTcpPort)
				{
					this.socketLocalTcpPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SocketLocalUdpPort")]
		public virtual int SocketLocalUdpPort
		{
			get { return (this.socketLocalUdpPort); }
			set
			{
				if (value != this.socketLocalUdpPort)
				{
					this.socketLocalUdpPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbHidDeviceInfo")]
		public virtual MKY.IO.Usb.DeviceInfo UsbHidDeviceInfo
		{
			get { return (this.usbHidDeviceInfo); }
			set
			{
				if (value != this.usbHidDeviceInfo)
				{
					this.usbHidDeviceInfo = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("StartTerminal")]
		public virtual bool StartTerminal
		{
			get { return (this.startTerminal); }
			set
			{
				if (value != this.startTerminal)
				{
					this.startTerminal = value;
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			NewTerminalSettings other = (NewTerminalSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.terminalType         == other.terminalType) &&
				(this.ioType               == other.ioType) &&
				(this.serialPortId         == other.serialPortId) &&
				(this.socketRemoteHost     == other.socketRemoteHost) &&
				(this.socketRemotePort     == other.socketRemotePort) &&
				(this.socketLocalInterface == other.socketLocalInterface) &&
				(this.socketLocalTcpPort   == other.socketLocalTcpPort) &&
				(this.socketLocalUdpPort   == other.socketLocalUdpPort) &&
				(this.usbHidDeviceInfo     == other.usbHidDeviceInfo) &&
				(this.startTerminal        == other.startTerminal)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			int serialPortIdHashCode = 0;
			if (this.serialPortId != null)
				serialPortIdHashCode = this.serialPortId.GetHashCode();

			int usbHidDeviceInfoHashCode = 0;
			if (this.usbHidDeviceInfo != null)
				usbHidDeviceInfoHashCode = this.usbHidDeviceInfo.GetHashCode();

			return
			(
				base.GetHashCode() ^

				this.terminalType        .GetHashCode() ^
				this.ioType              .GetHashCode() ^
				serialPortIdHashCode     .GetHashCode() ^
				this.socketRemoteHost    .GetHashCode() ^
				this.socketRemotePort    .GetHashCode() ^
				this.socketLocalInterface.GetHashCode() ^
				this.socketLocalTcpPort  .GetHashCode() ^
				this.socketLocalUdpPort  .GetHashCode() ^
				usbHidDeviceInfoHashCode .GetHashCode() ^
				this.startTerminal       .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.System.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
