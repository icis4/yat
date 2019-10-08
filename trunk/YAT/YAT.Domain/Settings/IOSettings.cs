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
// YAT Version 2.3.90 Development
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

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class IOSettings : MKY.Settings.SettingsItem, IEquatable<IOSettings>
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
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public IOSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			SerialPort         = new MKY.IO.Serial.SerialPort.SerialPortSettings(settingsType);
			Socket             = new MKY.IO.Serial.Socket.SocketSettings(settingsType);
			UsbSerialHidDevice = new MKY.IO.Serial.Usb.SerialHidDeviceSettings(settingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
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
		/// Fields are assigned via properties to ensure correct setting of changed flag.
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
						Socket.Type = (IOTypeEx)value;

					SetMyChanged();
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
				if (this.serialPort != value)
				{
					var oldNode = this.serialPort;
					this.serialPort = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
				if (this.socket != value)
				{
					var oldNode = this.socket;
					this.socket = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
				if (this.usbSerialHidDevice != value)
				{
					var oldNode = this.usbSerialHidDevice;
					this.usbSerialHidDevice = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <remarks>
		/// \remind (2017-12-11 / MKY)
		/// The endianness is currently fixed to 'Big-Endian (Network, Motorola)'.
		/// It was used by former versions of YAT but is currently not used anymore.
		/// Still, the setting is kept for future enhancements as documented in bug #343.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Endianness' is a correct English term.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
		[XmlElement("Endianness")]
		public virtual Endianness Endianness
		{
			get { return (this.endianness); }
			set
			{
				if (this.endianness != EndiannessDefault) // value)
				{
					this.endianness = EndiannessDefault; // value;
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool FlowControlIsInUse
		{
			get
			{
				switch (this.ioType)
				{
					case IOType.SerialPort:   return (this.serialPort.Communication.FlowControlIsInUse);
					case IOType.UsbSerialHid: return (this.usbSerialHidDevice.FlowControlIsInUse);
					default:                  return (false);
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

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool FlowControlUsesXOnXOffAutomatically
		{
			get { return (FlowControlUsesXOnXOff && !FlowControlManagesXOnXOffManually); }
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

				hashCode = (hashCode * 397) ^ IOType    .GetHashCode();
				hashCode = (hashCode * 397) ^ Endianness.GetHashCode();

				hashCode = (hashCode * 397) ^ IndicateSerialPortBreakStates    .GetHashCode();
				hashCode = (hashCode * 397) ^ SerialPortOutputBreakIsModifiable.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as IOSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(IOSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				IOType    .Equals(other.IOType)     &&
				Endianness.Equals(other.Endianness) &&

				IndicateSerialPortBreakStates    .Equals(other.IndicateSerialPortBreakStates) &&
				SerialPortOutputBreakIsModifiable.Equals(other.SerialPortOutputBreakIsModifiable)
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
				case IOType.UdpClient:
				case IOType.UdpServer:
				case IOType.UdpPairSocket:
					return (this.socket.ToShortEndPointString());

				case IOType.UsbSerialHid:
					return (this.usbSerialHidDevice.ToShortDeviceInfoString());

				default:
					return (Undefined);
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(IOSettings lhs, IOSettings rhs)
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
		public static bool operator !=(IOSettings lhs, IOSettings rhs)
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
