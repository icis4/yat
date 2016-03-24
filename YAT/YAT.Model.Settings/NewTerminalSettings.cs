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
		private int socketLocalTcpPort;
		private int socketLocalUdpPort;
		private MKY.IO.Serial.AutoRetry tcpClientAutoReconnect;

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
			SocketLocalTcpPort       = rhs.SocketLocalTcpPort;
			SocketLocalUdpPort       = rhs.SocketLocalUdpPort;
			TcpClientAutoReconnect   = rhs.TcpClientAutoReconnect;

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
			SocketLocalTcpPort       = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalTcpPort;
			SocketLocalUdpPort       = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalUdpPort;
			TcpClientAutoReconnect   = MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectDefault;

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

					// Ignore any other case.
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

		/// <summary></summary>
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
				(SocketLocalTcpPort       == other.SocketLocalTcpPort) &&
				(SocketLocalUdpPort       == other.SocketLocalUdpPort) &&
				(TcpClientAutoReconnect   == other.TcpClientAutoReconnect) &&

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
			int serialPortIdHashCode = 0;
			if (SerialPortId != null)
				serialPortIdHashCode = SerialPortId.GetHashCode();

			int usbSerialHidDeviceInfoHashCode = 0;
			if (UsbSerialHidDeviceInfo != null)
				usbSerialHidDeviceInfoHashCode = UsbSerialHidDeviceInfo.GetHashCode();

			return
			(
				base.GetHashCode() ^ // Get hash code of all settings nodes.

				TerminalType            .GetHashCode() ^
				IOType                  .GetHashCode() ^

				serialPortIdHashCode ^
				SerialPortCommunication .GetHashCode() ^
				SerialPortAutoReopen    .GetHashCode() ^

				SocketRemoteHost        .GetHashCode() ^
				SocketRemoteTcpPort     .GetHashCode() ^
				SocketRemoteUdpPort     .GetHashCode() ^
				SocketLocalInterface    .GetHashCode() ^
				SocketLocalTcpPort      .GetHashCode() ^
				SocketLocalUdpPort      .GetHashCode() ^
				TcpClientAutoReconnect  .GetHashCode() ^

				usbSerialHidDeviceInfoHashCode ^
				UsbSerialHidReportFormat.GetHashCode() ^
				UsbSerialHidRxIdUsage   .GetHashCode() ^
				UsbSerialHidFlowControl .GetHashCode() ^
				UsbSerialHidAutoOpen    .GetHashCode() ^

				StartTerminal           .GetHashCode()
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
