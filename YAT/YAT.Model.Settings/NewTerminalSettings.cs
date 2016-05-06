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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY.Net;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class NewTerminalSettings : MKY.Settings.SettingsItem
	{
		private Domain.TerminalType terminalType;
		private Domain.IOType ioType;

		private MKY.IO.Ports.SerialPortId serialPortId;
		private MKY.IO.Serial.SerialPort.SerialCommunicationSettings serialPortCommunication;
		private MKY.IO.Serial.AutoRetry serialPortAutoReopen;

		private string socketRemoteHost;
		private int socketRemoteTcpPort;
		private int socketRemoteUdpPort;
		private string socketLocalInterface;
		private string socketLocalFilter;
		private int socketLocalTcpPort;
		private int socketLocalUdpPort;
		private MKY.IO.Serial.AutoRetry tcpClientAutoReconnect;
		private MKY.IO.Serial.Socket.UdpServerSendMode udpServerSendMode;

		private MKY.IO.Usb.DeviceInfo usbSerialHidDeviceInfo;
		private MKY.IO.Usb.SerialHidReportFormat usbSerialHidReportFormat;
		private MKY.IO.Usb.SerialHidRxIdUsage usbSerialHidRxIdUsage;
		private MKY.IO.Serial.Usb.SerialHidFlowControl usbSerialHidFlowControl;
		private bool usbSerialHidAutoOpen;

		private bool startTerminal;

		/// <summary></summary>
		public NewTerminalSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public NewTerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			SerialPortCommunication = new MKY.IO.Serial.SerialPort.SerialCommunicationSettings(SettingsType);
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public NewTerminalSettings(NewTerminalSettings rhs)
			: base(rhs)
		{
			TerminalType             = rhs.TerminalType;
			IOType                   = rhs.IOType;

			SerialPortId             = rhs.SerialPortId;
			SerialPortCommunication  = rhs.SerialPortCommunication;
			SerialPortAutoReopen     = rhs.SerialPortAutoReopen;

			SocketRemoteHost         = rhs.SocketRemoteHost;
			SocketRemoteTcpPort      = rhs.SocketRemoteTcpPort;
			SocketRemoteUdpPort      = rhs.SocketRemoteUdpPort;
			SocketLocalInterface     = rhs.SocketLocalInterface;
			SocketLocalFilter        = rhs.SocketLocalFilter;
			SocketLocalTcpPort       = rhs.SocketLocalTcpPort;
			SocketLocalUdpPort       = rhs.SocketLocalUdpPort;
			TcpClientAutoReconnect   = rhs.TcpClientAutoReconnect;
			UdpServerSendMode        = rhs.UdpServerSendMode;

			UsbSerialHidDeviceInfo   = rhs.UsbSerialHidDeviceInfo;
			UsbSerialHidReportFormat = rhs.UsbSerialHidReportFormat;
			UsbSerialHidRxIdUsage    = rhs.UsbSerialHidRxIdUsage;
			UsbSerialHidFlowControl  = rhs.UsbSerialHidFlowControl;
			UsbSerialHidAutoOpen     = rhs.UsbSerialHidAutoOpen;

			StartTerminal            = rhs.StartTerminal;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// 
		/// Attention: Do not use <see cref="MKY.IO.Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="MKY.IO.Ports.SerialPortId.FirstStandardPort"/>
		/// is way better performing and good enough for most use cases.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TerminalType             = Domain.Settings.TerminalSettings.TerminalTypeDefault;
			IOType                   = Domain.IOType.SerialPort;

			SerialPortId             = MKY.IO.Ports.SerialPortId.FirstStandardPort;
			//// SerialPortCommunication is attached as settings object.
			SerialPortAutoReopen     = MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault;

			SocketRemoteHost         = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteHost;
			SocketRemoteTcpPort      = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteTcpPort;
			SocketRemoteUdpPort      = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteUdpPort;
			SocketLocalInterface     = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalInterface;
			SocketLocalFilter        = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalFilter;
			SocketLocalTcpPort       = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalTcpPort;
			SocketLocalUdpPort       = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalUdpPort;
			TcpClientAutoReconnect   = MKY.IO.Serial.Socket.SocketSettings.DefaultTcpClientAutoReconnect;
			UdpServerSendMode        = MKY.IO.Serial.Socket.SocketSettings.DefaultUdpServerSendMode;

			UsbSerialHidDeviceInfo   = null;
			UsbSerialHidReportFormat = new MKY.IO.Usb.SerialHidReportFormat();
			UsbSerialHidRxIdUsage    = new MKY.IO.Usb.SerialHidRxIdUsage();
			UsbSerialHidFlowControl  = MKY.IO.Serial.Usb.SerialHidDeviceSettings.FlowControlDefault;
			UsbSerialHidAutoOpen     = MKY.IO.Serial.Usb.SerialHidDeviceSettings.AutoOpenDefault;

			StartTerminal            = true;
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
				if (this.terminalType != value)
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
				if (this.ioType != value)
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
				if (this.serialPortId != value)
				{
					this.serialPortId = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPortCommunication")]
		public virtual MKY.IO.Serial.SerialPort.SerialCommunicationSettings SerialPortCommunication
		{
			get { return (this.serialPortCommunication); }
			set
			{
				if (value == null)
				{
					DetachNode(this.serialPortCommunication);
					this.serialPortCommunication = null;
				}
				else if (this.serialPortCommunication == null)
				{
					this.serialPortCommunication = value;
					AttachNode(this.serialPortCommunication);
				}
				else if (this.serialPortCommunication != value)
				{
					MKY.IO.Serial.SerialPort.SerialCommunicationSettings old = this.serialPortCommunication;
					this.serialPortCommunication = value;
					ReplaceNode(old, this.serialPortCommunication);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPortAutoReopen")]
		public virtual MKY.IO.Serial.AutoRetry SerialPortAutoReopen
		{
			get { return (this.serialPortAutoReopen); }
			set
			{
				if (this.serialPortAutoReopen != value)
				{
					this.serialPortAutoReopen = value;
					SetChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[XmlElement("SocketRemoteHost")]
		public virtual string SocketRemoteHost
		{
			get { return (this.socketRemoteHost); }
			set
			{
				if (this.socketRemoteHost != value)
				{
					this.socketRemoteHost = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SocketRemoteTcpPort")]
		public virtual int SocketRemoteTcpPort
		{
			get { return (this.socketRemoteTcpPort); }
			set
			{
				if (this.socketRemoteTcpPort != value)
				{
					this.socketRemoteTcpPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SocketRemoteUdpPort")]
		public virtual int SocketRemoteUdpPort
		{
			get { return (this.socketRemoteUdpPort); }
			set
			{
				if (this.socketRemoteUdpPort != value)
				{
					this.socketRemoteUdpPort = value;
					SetChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[XmlElement("SocketLocalInterface")]
		public virtual string SocketLocalInterface
		{
			get { return (this.socketLocalInterface); }
			set
			{
				if (this.socketLocalInterface != value)
				{
					this.socketLocalInterface = value;
					SetChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[XmlElement("SocketLocalFilter")]
		public virtual string SocketLocalFilter
		{
			get { return (this.socketLocalFilter); }
			set
			{
				if (this.socketLocalFilter != value)
				{
					this.socketLocalFilter = value;
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

					case Domain.IOType.UdpClient:
					case Domain.IOType.UdpServer:
					case Domain.IOType.UdpPairSocket:
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

					case Domain.IOType.UdpClient:
					case Domain.IOType.UdpServer:
					case Domain.IOType.UdpPairSocket:
						SocketLocalUdpPort = value;
						break;

					default:
						break; // Ignore any other case.
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
				if (this.socketLocalTcpPort != value)
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
				if (this.socketLocalUdpPort != value)
				{
					this.socketLocalUdpPort = value;
					SetChanged();
				}
			}
		}

		/// <remarks>Item is already named 'TcpClient', therefore no 'Socket' is prepended.</remarks>
		[XmlElement("TcpClientAutoReconnect")]
		public virtual MKY.IO.Serial.AutoRetry TcpClientAutoReconnect
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

		/// <remarks>Item is already named 'UdpServer', therefore no 'Socket' is prepended.</remarks>
		[XmlElement("UdpServerSendMode")]
		public virtual MKY.IO.Serial.Socket.UdpServerSendMode UdpServerSendMode
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

		/// <summary></summary>
		[XmlElement("UsbSerialHidDeviceInfo")]
		public virtual MKY.IO.Usb.DeviceInfo UsbSerialHidDeviceInfo
		{
			get { return (this.usbSerialHidDeviceInfo); }
			set
			{
				if (this.usbSerialHidDeviceInfo != value)
				{
					this.usbSerialHidDeviceInfo = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidReportFormat")]
		public virtual MKY.IO.Usb.SerialHidReportFormat UsbSerialHidReportFormat
		{
			get { return (this.usbSerialHidReportFormat); }
			set
			{
				if (this.usbSerialHidReportFormat != value)
				{
					this.usbSerialHidReportFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidRxIdUsage")]
		public virtual MKY.IO.Usb.SerialHidRxIdUsage UsbSerialHidRxIdUsage
		{
			get { return (this.usbSerialHidRxIdUsage); }
			set
			{
				if (this.usbSerialHidRxIdUsage != value)
				{
					this.usbSerialHidRxIdUsage = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidFlowControl")]
		public virtual MKY.IO.Serial.Usb.SerialHidFlowControl UsbSerialHidFlowControl
		{
			get { return (this.usbSerialHidFlowControl); }
			set
			{
				if (this.usbSerialHidFlowControl != value)
				{
					this.usbSerialHidFlowControl = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidAutoOpen")]
		public virtual bool UsbSerialHidAutoOpen
		{
			get { return (this.usbSerialHidAutoOpen); }
			set
			{
				if (this.usbSerialHidAutoOpen != value)
				{
					this.usbSerialHidAutoOpen = value;
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
				if (this.startTerminal != value)
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

			NewTerminalSettings other = (NewTerminalSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(TerminalType             == other.TerminalType) &&
				(IOType                   == other.IOType) &&

				(SerialPortId             == other.SerialPortId) &&
				(SerialPortCommunication  == other.SerialPortCommunication) &&
				(SerialPortAutoReopen     == other.SerialPortAutoReopen) &&

				(SocketRemoteHost         == other.SocketRemoteHost) &&
				(SocketRemoteTcpPort      == other.SocketRemoteTcpPort) &&
				(SocketRemoteUdpPort      == other.SocketRemoteUdpPort) &&
				(SocketLocalInterface     == other.SocketLocalInterface) &&
				(SocketLocalFilter        == other.SocketLocalFilter) &&
				(SocketLocalTcpPort       == other.SocketLocalTcpPort) &&
				(SocketLocalUdpPort       == other.SocketLocalUdpPort) &&
				(TcpClientAutoReconnect   == other.TcpClientAutoReconnect) &&
				(UdpServerSendMode        == other.UdpServerSendMode) &&

				(UsbSerialHidDeviceInfo   == other.UsbSerialHidDeviceInfo) &&
				(UsbSerialHidReportFormat == other.UsbSerialHidReportFormat) &&
				(UsbSerialHidRxIdUsage    == other.UsbSerialHidRxIdUsage) &&
				(UsbSerialHidFlowControl  == other.UsbSerialHidFlowControl) &&
				(UsbSerialHidAutoOpen     == other.UsbSerialHidAutoOpen) &&

				(StartTerminal            == other.StartTerminal)
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

				hashCode = (hashCode * 397) ^  TerminalType                                           .GetHashCode();
				hashCode = (hashCode * 397) ^  IOType                                                 .GetHashCode();

				hashCode = (hashCode * 397) ^ (SerialPortId           != null ? SerialPortId          .GetHashCode() : 0); // May be 'null' if no ports are available!
				hashCode = (hashCode * 397) ^  SerialPortCommunication                                .GetHashCode();
				hashCode = (hashCode * 397) ^  SerialPortAutoReopen                                   .GetHashCode();

				hashCode = (hashCode * 397) ^ (SocketRemoteHost       != null ? SocketRemoteHost      .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  SocketRemoteTcpPort;
				hashCode = (hashCode * 397) ^  SocketRemoteUdpPort;
				hashCode = (hashCode * 397) ^ (SocketLocalInterface   != null ? SocketLocalInterface  .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SocketLocalFilter      != null ? SocketLocalFilter     .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  SocketLocalTcpPort;
				hashCode = (hashCode * 397) ^  SocketLocalUdpPort;
				hashCode = (hashCode * 397) ^  TcpClientAutoReconnect                                 .GetHashCode();
				hashCode = (hashCode * 397) ^  UdpServerSendMode                                      .GetHashCode();

				hashCode = (hashCode * 397) ^ (UsbSerialHidDeviceInfo != null ? UsbSerialHidDeviceInfo.GetHashCode() : 0); // May be 'null' if no devices are available!
				hashCode = (hashCode * 397) ^  UsbSerialHidReportFormat                               .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidRxIdUsage                                  .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidFlowControl                                .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidAutoOpen                                   .GetHashCode();

				hashCode = (hashCode * 397) ^  StartTerminal                                          .GetHashCode();

				return (hashCode);
			}
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
