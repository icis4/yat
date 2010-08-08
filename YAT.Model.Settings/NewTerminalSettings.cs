//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Net;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class NewTerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<NewTerminalSettings>
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
		public NewTerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			this.terminalType         = rhs.TerminalType;
			this.ioType               = rhs.IOType;

			this.serialPortId         = rhs.SerialPortId;

			this.socketRemoteHost     = rhs.SocketRemoteHost;
			this.socketRemotePort     = rhs.SocketRemotePort;

			this.socketLocalInterface = rhs.SocketLocalInterface;
			this.socketLocalTcpPort   = rhs.SocketLocalTcpPort;
			this.socketLocalUdpPort   = rhs.SocketLocalUdpPort;

			this.usbHidDeviceInfo     = rhs.UsbHidDeviceInfo;

			this.startTerminal        = rhs.StartTerminal;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalType         = Domain.TerminalType.Text;
			IOType               = Domain.IOType.SerialPort;

			SerialPortId         = MKY.IO.Ports.SerialPortId.DefaultPort;

			SocketRemoteHost     = MKY.IO.Serial.SocketSettings.DefaultRemoteHost;
			SocketRemotePort     = MKY.IO.Serial.SocketSettings.DefaultPort;

			SocketLocalInterface = MKY.IO.Serial.SocketSettings.DefaultLocalInterface;
			SocketLocalTcpPort   = MKY.IO.Serial.SocketSettings.DefaultPort;
			SocketLocalUdpPort   = MKY.IO.Serial.SocketSettings.DefaultPort + 1;

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
			if (obj == null)
				return (false);

			NewTerminalSettings casted = obj as NewTerminalSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(NewTerminalSettings casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.

				(this.terminalType == casted.terminalType) &&
				(this.ioType               == casted.ioType) &&
				(this.serialPortId         == casted.serialPortId) &&
				(this.socketRemoteHost     == casted.socketRemoteHost) &&
				(this.socketRemotePort     == casted.socketRemotePort) &&
				(this.socketLocalInterface == casted.socketLocalInterface) &&
				(this.socketLocalTcpPort   == casted.socketLocalTcpPort) &&
				(this.socketLocalUdpPort   == casted.socketLocalUdpPort) &&
				(this.usbHidDeviceInfo     == casted.usbHidDeviceInfo) &&
				(this.startTerminal        == casted.startTerminal)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(NewTerminalSettings lhs, NewTerminalSettings rhs)
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
