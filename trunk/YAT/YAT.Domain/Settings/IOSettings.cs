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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class IOSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const IOType IOTypeDefault = IOType.SerialPort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
		public const Endianness EndiannessDefault = Endianness.BigEndian;

		/// <summary></summary>
		public const bool IndicateSerialPortBreakStatesDefault = false;

		/// <summary></summary>
		public const bool SerialPortOutputBreakIsModifiableDefault = false;

		private const string Undefined = "<Undefined>";

		private IOType ioType;
		private MKY.IO.Serial.SerialPort.SerialPortSettings serialPort;
		private MKY.IO.Serial.Socket.SocketSettings socket;
		private MKY.IO.Serial.Usb.SerialHidDeviceSettings usbSerialHidDevice;
		private Endianness endianness;

		// Serial port specific I/O settings. Located here (and not in 'SerialPortSettings) as they are endemic to YAT.
		private bool indicateSerialPortBreakStates;
		private bool serialPortOutputBreakIsModifiable;

		/// <summary></summary>
		public IOSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public IOSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			SerialPort         = new MKY.IO.Serial.SerialPort.SerialPortSettings(SettingsType);
			Socket             = new MKY.IO.Serial.Socket.SocketSettings(SettingsType);
			UsbSerialHidDevice = new MKY.IO.Serial.Usb.SerialHidDeviceSettings(SettingsType);
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public IOSettings(IOSettings rhs)
			: base(rhs)
		{
			IOType             = rhs.IOType;
			SerialPort         = new MKY.IO.Serial.SerialPort.SerialPortSettings(rhs.SerialPort);
			Socket             = new MKY.IO.Serial.Socket.SocketSettings(rhs.Socket);
			UsbSerialHidDevice = new MKY.IO.Serial.Usb.SerialHidDeviceSettings(rhs.UsbSerialHidDevice);
			Endianness         = rhs.Endianness;

			IndicateSerialPortBreakStates     = rhs.IndicateSerialPortBreakStates;
			SerialPortOutputBreakIsModifiable = rhs.SerialPortOutputBreakIsModifiable;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			IOType     = IOTypeDefault;
			Endianness = EndiannessDefault;

			IndicateSerialPortBreakStates     = IndicateSerialPortBreakStatesDefault;
			SerialPortOutputBreakIsModifiable = SerialPortOutputBreakIsModifiableDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("IOType")]
		public virtual IOType IOType
		{
			get { return (this.ioType); }
			set
			{
				if (this.ioType != value)
				{
					this.ioType = value;

					// Set socket host type as well:
					if (Socket != null)
						Socket.HostType = (IOTypeEx)value;

					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPort")]
		public virtual MKY.IO.Serial.SerialPort.SerialPortSettings SerialPort
		{
			get { return (this.serialPort); }
			set
			{
				if (value == null)
				{
					DetachNode(this.serialPort);
					this.serialPort = null;
				}
				else if (this.serialPort == null)
				{
					this.serialPort = value;
					AttachNode(this.serialPort);
				}
				else if (this.serialPort != value)
				{
					MKY.IO.Serial.SerialPort.SerialPortSettings old = this.serialPort;
					this.serialPort = value;
					ReplaceNode(old, this.serialPort);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Socket")]
		public virtual MKY.IO.Serial.Socket.SocketSettings Socket
		{
			get { return (this.socket); }
			set
			{
				if (value == null)
				{
					DetachNode(this.socket);
					this.socket = null;
				}
				else if (this.socket == null)
				{
					this.socket = value;
					AttachNode(this.socket);
				}
				else if (this.socket != value)
				{
					MKY.IO.Serial.Socket.SocketSettings old = this.socket;
					this.socket = value;
					ReplaceNode(old, this.socket);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbSerialHidDevice")]
		public virtual MKY.IO.Serial.Usb.SerialHidDeviceSettings UsbSerialHidDevice
		{
			get { return (this.usbSerialHidDevice); }
			set
			{
				if (value == null)
				{
					DetachNode(this.usbSerialHidDevice);
					this.usbSerialHidDevice = null;
				}
				else if (this.usbSerialHidDevice == null)
				{
					this.usbSerialHidDevice = value;
					AttachNode(this.usbSerialHidDevice);
				}
				else if (this.usbSerialHidDevice != value)
				{
					MKY.IO.Serial.Usb.SerialHidDeviceSettings old = this.usbSerialHidDevice;
					this.usbSerialHidDevice = value;
					ReplaceNode(old, this.usbSerialHidDevice);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
		[XmlElement("Endianness")]
		public virtual Endianness Endianness
		{
			get { return (this.endianness); }
			set
			{
				if (this.endianness != value)
				{
					this.endianness = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IndicateSerialPortBreakStates")]
		public virtual bool IndicateSerialPortBreakStates
		{
			get { return (this.indicateSerialPortBreakStates); }
			set
			{
				if (this.indicateSerialPortBreakStates != value)
				{
					this.indicateSerialPortBreakStates = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPortOutputBreakIsModifiable")]
		public virtual bool SerialPortOutputBreakIsModifiable
		{
			get { return (this.serialPortOutputBreakIsModifiable); }
			set
			{
				if (this.serialPortOutputBreakIsModifiable != value)
				{
					this.serialPortOutputBreakIsModifiable = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool FlowControlUsesXOnXOff
		{
			get
			{
				switch (this.ioType)
				{
					case IOType.SerialPort:   return (this.serialPort.Communication.FlowControlUsesXOnXOff);
					case IOType.UsbSerialHid: return (this.usbSerialHidDevice.FlowControlUsesXOnXOff);
					default:                  return (false);
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool FlowControlManagesXOnXOffManually
		{
			get
			{
				switch (this.ioType)
				{
					case IOType.SerialPort:   return (this.serialPort.Communication.FlowControlManagesXOnXOffManually);
					case IOType.UsbSerialHid: return (this.usbSerialHidDevice.FlowControlManagesXOnXOffManually);
					default:                  return (false);
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

			IOSettings other = (IOSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(IOType     == other.IOType) &&
				(Endianness == other.Endianness) &&

				(IndicateSerialPortBreakStates     == other.IndicateSerialPortBreakStates) &&
				(SerialPortOutputBreakIsModifiable == other.SerialPortOutputBreakIsModifiable)
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

				IOType    .GetHashCode() ^
				Endianness.GetHashCode() ^

				IndicateSerialPortBreakStates    .GetHashCode() ^
				SerialPortOutputBreakIsModifiable.GetHashCode()
			);
		}

		/// <summary></summary>
		public virtual string ToShortIOString()
		{
			switch (ioType)
			{
				case IOType.SerialPort:
					return (this.serialPort.ToShortPortString());

				case IOType.TcpClient:
				case IOType.TcpServer:
				case IOType.TcpAutoSocket:
				case IOType.Udp:
					return (this.socket.ToShortEndPointString());

				case IOType.UsbSerialHid:
					return (this.usbSerialHidDevice.ToShortDeviceInfoString());

				default:
					return (Undefined);
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
