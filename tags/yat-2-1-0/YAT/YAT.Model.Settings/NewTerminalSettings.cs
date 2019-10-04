//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;
using MKY.Net;

namespace YAT.Model.Settings
{
	/// <remarks>
	/// \remind (2017-11-19 / MKY), (2019-08-02 / MKY)
	/// Should be migrated to a separate 'YAT.Application.Settings' project. Not easily possible
	/// because of dependencies among 'YAT.*' and 'YAT.Application', e.g. 'ExtensionSettings'.
	/// Requires slight refactoring of project dependencies. Could be done when refactoring the
	/// projects on integration with Albatros.
	/// </remarks>
	public class NewTerminalSettings : MKY.Settings.SettingsItem, IEquatable<NewTerminalSettings>
	{
		private Domain.TerminalType terminalType;
		private Domain.IOType ioType;

		private MKY.IO.Ports.SerialPortId serialPortId;
		private MKY.IO.Serial.SerialPort.SerialCommunicationSettings serialPortCommunication;
		private MKY.IO.Serial.AutoInterval serialPortAliveMonitor;
		private MKY.IO.Serial.AutoInterval serialPortAutoReopen;

		private IPHostEx socketRemoteHost;
		private int socketRemoteTcpPort;
		private int socketRemoteUdpPort;
		private IPNetworkInterfaceDescriptorPair socketLocalInterface;
		private IPFilterEx socketLocalFilter;
		private int socketLocalTcpPort;
		private int socketLocalUdpPort;
		private MKY.IO.Serial.AutoInterval tcpClientAutoReconnect;
		private MKY.IO.Serial.Socket.UdpServerSendMode udpServerSendMode;

		private MKY.IO.Usb.DeviceInfo usbSerialHidDeviceInfo;
		private bool usbSerialHidMatchSerial;
		private MKY.IO.Usb.SerialHidDeviceSettingsPreset usbSerialHidPreset;
		private MKY.IO.Usb.SerialHidReportFormat usbSerialHidReportFormat;
		private MKY.IO.Usb.SerialHidRxFilterUsage usbSerialHidRxFilterUsage;
		private MKY.IO.Serial.Usb.SerialHidFlowControl usbSerialHidFlowControl;
		private bool usbSerialHidAutoOpen;

		private bool startTerminal;

		/// <summary></summary>
		public NewTerminalSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public NewTerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			SerialPortCommunication = new MKY.IO.Serial.SerialPort.SerialCommunicationSettings(settingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public NewTerminalSettings(NewTerminalSettings rhs)
			: base(rhs)
		{
			TerminalType              = rhs.TerminalType;
			IOType                    = rhs.IOType;

			SerialPortId              = rhs.SerialPortId;
			SerialPortCommunication   = rhs.SerialPortCommunication;
			SerialPortAliveMonitor    = rhs.SerialPortAliveMonitor;
			SerialPortAutoReopen      = rhs.SerialPortAutoReopen;

			SocketRemoteHost          = rhs.SocketRemoteHost;
			SocketRemoteTcpPort       = rhs.SocketRemoteTcpPort;
			SocketRemoteUdpPort       = rhs.SocketRemoteUdpPort;
			SocketLocalInterface      = rhs.SocketLocalInterface;
			SocketLocalFilter         = rhs.SocketLocalFilter;
			SocketLocalTcpPort        = rhs.SocketLocalTcpPort;
			SocketLocalUdpPort        = rhs.SocketLocalUdpPort;
			TcpClientAutoReconnect    = rhs.TcpClientAutoReconnect;
			UdpServerSendMode         = rhs.UdpServerSendMode;

			UsbSerialHidDeviceInfo    = rhs.UsbSerialHidDeviceInfo;
			UsbSerialHidMatchSerial   = rhs.UsbSerialHidMatchSerial;
			UsbSerialHidPreset        = rhs.UsbSerialHidPreset;
			UsbSerialHidReportFormat  = rhs.UsbSerialHidReportFormat;
			UsbSerialHidRxFilterUsage = rhs.UsbSerialHidRxFilterUsage;
			UsbSerialHidFlowControl   = rhs.UsbSerialHidFlowControl;
			UsbSerialHidAutoOpen      = rhs.UsbSerialHidAutoOpen;

			StartTerminal             = rhs.StartTerminal;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		///
		/// Attention: Do not use <see cref="MKY.IO.Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="MKY.IO.Ports.SerialPortId.FirstStandardPort"/>
		/// is way better performing and good enough for most use cases.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TerminalType              = Domain.Settings.TerminalSettings.TerminalTypeDefault;
			IOType                    = Domain.IOType.SerialPort;

			SerialPortId              = MKY.IO.Ports.SerialPortId.FirstStandardPort;
		////SerialPortCommunication is attached as settings object.
			SerialPortAliveMonitor    = MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorDefault;
			SerialPortAutoReopen      = MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault;

			SocketRemoteHost          = MKY.IO.Serial.Socket.SocketSettings.RemoteHostDefault;
			SocketRemoteTcpPort       = MKY.IO.Serial.Socket.SocketSettings.RemotePortDefault;
			SocketRemoteUdpPort       = MKY.IO.Serial.Socket.SocketSettings.RemotePortDefault;
			SocketLocalInterface      = MKY.IO.Serial.Socket.SocketSettings.LocalInterfaceDefault;
			SocketLocalFilter         = MKY.IO.Serial.Socket.SocketSettings.LocalFilterDefault;
			SocketLocalTcpPort        = MKY.IO.Serial.Socket.SocketSettings.LocalPortDefault;
			SocketLocalUdpPort        = MKY.IO.Serial.Socket.SocketSettings.LocalPortDefault;
			TcpClientAutoReconnect    = MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectDefault;
			UdpServerSendMode         = MKY.IO.Serial.Socket.SocketSettings.UdpServerSendModeDefault;

			UsbSerialHidDeviceInfo    = null;
			UsbSerialHidMatchSerial   = MKY.IO.Serial.Usb.SerialHidDeviceSettings.MatchSerialDefault;
			UsbSerialHidPreset        = MKY.IO.Serial.Usb.SerialHidDeviceSettings.PresetDefault;
			UsbSerialHidReportFormat  = MKY.IO.Serial.Usb.SerialHidDeviceSettings.ReportFormatDefault;
			UsbSerialHidRxFilterUsage = MKY.IO.Serial.Usb.SerialHidDeviceSettings.RxFilterUsageDefault;
			UsbSerialHidFlowControl   = MKY.IO.Serial.Usb.SerialHidDeviceSettings.FlowControlDefault;
			UsbSerialHidAutoOpen      = MKY.IO.Serial.Usb.SerialHidDeviceSettings.AutoOpenDefault;

			StartTerminal             = true;
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
				if (this.serialPortCommunication != value)
				{
					var oldNode = this.serialPortCommunication;
					this.serialPortCommunication = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPortAliveMonitor")]
		public virtual MKY.IO.Serial.AutoInterval SerialPortAliveMonitor
		{
			get { return (this.serialPortAliveMonitor); }
			set
			{
				if (this.serialPortAliveMonitor != value)
				{
					this.serialPortAliveMonitor = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPortAutoReopen")]
		public virtual MKY.IO.Serial.AutoInterval SerialPortAutoReopen
		{
			get { return (this.serialPortAutoReopen); }
			set
			{
				if (this.serialPortAutoReopen != value)
				{
					this.serialPortAutoReopen = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public virtual IPHostEx SocketRemoteHost
		{
			get { return (this.socketRemoteHost); }
			set
			{
				if (this.socketRemoteHost != value)
				{
					this.socketRemoteHost = value;
					SetMyChanged();

					// Do not try to resolve the IP address as this may take quite some time!
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("SocketRemoteHost")]
		public virtual string SocketRemoteHost_ForSerialization
		{
			get { return (SocketRemoteHost.ToCompactString()); } // Use compact string represenation, only taking host name or address into account!
			set { SocketRemoteHost = value;                    }
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
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SocketLocalInterface")]
		public virtual IPNetworkInterfaceDescriptorPair SocketLocalInterface
		{
			get { return (this.socketLocalInterface); }
			set
			{
				if (this.socketLocalInterface != value)
				{
					this.socketLocalInterface = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public virtual IPFilterEx SocketLocalFilter
		{
			get { return (this.socketLocalFilter); }
			set
			{
				if (this.socketLocalFilter != value)
				{
					this.socketLocalFilter = value;
					SetMyChanged();

					// Do not try to resolve the IP address as this may take quite some time!
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("SocketLocalFilter")]
		public virtual string SocketLocalFilter_ForSerialization
		{
			get { return (SocketLocalFilter.ToCompactString()); } // Use compact string represenation, only taking host name or address into account!
			set { SocketLocalFilter = value;                    }
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
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		/// <remarks>Item is already named 'TcpClient', therefore no 'Socket' is prepended.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Tcp' like this...")]
		[XmlElement("TcpClientAutoReconnect")]
		public virtual MKY.IO.Serial.AutoInterval TcpClientAutoReconnect
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

		/// <remarks>Item is already named 'UdpServer', therefore no 'Socket' is prepended.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Udp' like this...")]
		[XmlElement("UdpServerSendMode")]
		public virtual MKY.IO.Serial.Socket.UdpServerSendMode UdpServerSendMode
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidMatchSerial")]
		public virtual bool UsbSerialHidMatchSerial
		{
			get { return (this.usbSerialHidMatchSerial); }
			set
			{
				if (this.usbSerialHidMatchSerial != value)
				{
					this.usbSerialHidMatchSerial = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidPreset")]
		public virtual MKY.IO.Usb.SerialHidDeviceSettingsPreset UsbSerialHidPreset
		{
			get { return (this.usbSerialHidPreset); }
			set
			{
				if (this.usbSerialHidPreset != value)
				{
					this.usbSerialHidPreset = value;
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidRxFilterUsage")]
		public virtual MKY.IO.Usb.SerialHidRxFilterUsage UsbSerialHidRxFilterUsage
		{
			get { return (this.usbSerialHidRxFilterUsage); }
			set
			{
				if (this.usbSerialHidRxFilterUsage != value)
				{
					this.usbSerialHidRxFilterUsage = value;
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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

				hashCode = (hashCode * 397) ^  TerminalType                                                                   .GetHashCode();
				hashCode = (hashCode * 397) ^  IOType                                                                         .GetHashCode();

				hashCode = (hashCode * 397) ^ (SerialPortId                       != null ? SerialPortId                      .GetHashCode() : 0); // May be 'null' if no ports are available!
				hashCode = (hashCode * 397) ^  SerialPortCommunication                                                        .GetHashCode();
				hashCode = (hashCode * 397) ^  SerialPortAliveMonitor                                                         .GetHashCode();
				hashCode = (hashCode * 397) ^  SerialPortAutoReopen                                                           .GetHashCode();

				hashCode = (hashCode * 397) ^ ( SocketRemoteHost_ForSerialization != null ?  SocketRemoteHost_ForSerialization.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  SocketRemoteTcpPort;
				hashCode = (hashCode * 397) ^  SocketRemoteUdpPort;
				hashCode = (hashCode * 397) ^  SocketLocalInterface                                                           .GetHashCode();
				hashCode = (hashCode * 397) ^ (SocketLocalFilter_ForSerialization != null ? SocketLocalFilter_ForSerialization.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  SocketLocalTcpPort;
				hashCode = (hashCode * 397) ^  SocketLocalUdpPort;
				hashCode = (hashCode * 397) ^  TcpClientAutoReconnect                                                         .GetHashCode();
				hashCode = (hashCode * 397) ^  UdpServerSendMode                                                              .GetHashCode();

				hashCode = (hashCode * 397) ^ (UsbSerialHidDeviceInfo             != null ? UsbSerialHidDeviceInfo            .GetHashCode() : 0); // May be 'null' if no devices are available!
				hashCode = (hashCode * 397) ^  UsbSerialHidMatchSerial                                                        .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidPreset                                                             .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidReportFormat                                                       .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidRxFilterUsage                                                      .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidFlowControl                                                        .GetHashCode();
				hashCode = (hashCode * 397) ^  UsbSerialHidAutoOpen                                                           .GetHashCode();

				hashCode = (hashCode * 397) ^  StartTerminal                                                                  .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as NewTerminalSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(NewTerminalSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				TerminalType.Equals(other.TerminalType) &&
				IOType      .Equals(other.IOType)       &&

				ObjectEx.Equals(SerialPortId, other.SerialPortId)                 &&
				SerialPortCommunication  .Equals(other.SerialPortCommunication)   &&
				SerialPortAliveMonitor   .Equals(other.SerialPortAliveMonitor)    &&
				SerialPortAutoReopen     .Equals(other.SerialPortAutoReopen)      &&

				StringEx.EqualsOrdinalIgnoreCase(SocketRemoteHost_ForSerialization, other.SocketRemoteHost_ForSerialization) &&
				SocketRemoteTcpPort      .Equals(other.SocketRemoteTcpPort)       &&
				SocketRemoteUdpPort      .Equals(other.SocketRemoteUdpPort)       &&
				SocketLocalInterface     .Equals(other.SocketLocalInterface)      &&
				StringEx.EqualsOrdinalIgnoreCase(SocketLocalFilter_ForSerialization, other.SocketLocalFilter_ForSerialization) &&
				SocketLocalTcpPort       .Equals(other.SocketLocalTcpPort)        &&
				SocketLocalUdpPort       .Equals(other.SocketLocalUdpPort)        &&
				TcpClientAutoReconnect   .Equals(other.TcpClientAutoReconnect)    &&
				UdpServerSendMode        .Equals(other.UdpServerSendMode)         &&

				ObjectEx.Equals(UsbSerialHidDeviceInfo, other.UsbSerialHidDeviceInfo) &&
				UsbSerialHidMatchSerial  .Equals(other.UsbSerialHidMatchSerial)   &&
				UsbSerialHidPreset       .Equals(other.UsbSerialHidPreset)        &&
				UsbSerialHidReportFormat .Equals(other.UsbSerialHidReportFormat)  &&
				UsbSerialHidRxFilterUsage.Equals(other.UsbSerialHidRxFilterUsage) &&
				UsbSerialHidFlowControl  .Equals(other.UsbSerialHidFlowControl)   &&
				UsbSerialHidAutoOpen     .Equals(other.UsbSerialHidAutoOpen)      &&

				StartTerminal.Equals(other.StartTerminal)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(NewTerminalSettings lhs, NewTerminalSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(NewTerminalSettings lhs, NewTerminalSettings rhs)
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
