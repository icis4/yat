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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class IOSettings : MKY.Settings.SettingsItem, IEquatable<IOSettings>
	{
		/// <summary></summary>
		public const IOType IOTypeDefault = IOType.SerialPort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
		public const Endianness EndiannessDefault = EndiannessEx.Default;

		/// <summary></summary>
		public const bool IndicateSerialPortBreakStatesDefault = false;

		/// <summary></summary>
		public const bool SerialPortOutputBreakIsModifiableDefault = false;

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

		#endregion

		#region Property Combinations
		//==========================================================================================
		// Property Combinations
		//==========================================================================================

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IOTypeIsSocket
		{
			get { return (((IOTypeEx)IOType).IsSocket); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IOTypeIsTcpSocket
		{
			get { return (((IOTypeEx)IOType).IsTcpSocket); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IOTypeIsUdpSocket
		{
			get { return (((IOTypeEx)IOType).IsUdpSocket); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IOTypeIsServerSocket
		{
			get { return (((IOTypeEx)IOType).IsServerSocket); }
		}

		/// <remarks>
		/// Value is approximate! It may be off by a factor of 2..5, depending on environment and I/O related settings!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <remarks>
		/// Named "Bytes" rather than "Octets" as that is more common, and .NET uses "Byte" as well.
		/// </remarks>
		/// <remarks>
		/// Named this long to make inaccuracy obvious.
		/// </remarks>
		[XmlIgnore]
		public virtual double ApproximateTypicalNumberOfBytesPerMillisecond
		{
			get
			{
				switch (IOType)
				{
					case IOType.Unknown:
					{
						return (0);
					}

					case IOType.SerialPort:
					{
						return (CalculatedApproximateTypicalNumberOfBytesPerMillisecond_SerialPort);
					}

					case IOType.TcpClient:
					case IOType.TcpServer:
					case IOType.TcpAutoSocket:
					{
						// In theory:
						// Less than 6.55 MiBit/s (https://www.switch.ch/network/tools/tcp_throughput/), i.e. less than ~800 KiByte/s.
						//
						// In YAT practise (e.g. 'Huge.txt'):
						//  > ~19.5 KiByte/s for [Debug]
						//  > ~43.6 KiByte/s for [Release]
						// See 'MKY.IO.Serial.Socket.SocketDefaults' for measurements.
					#if (DEBUG)
						return (19.5);
					#else
						return (43.6);
					#endif
						// But attention, the view consumes a lot of CPU. Sending e.g. 'EnormousLine.txt'
						// where view has almost nothing to do happens almost instantly, at up to ~250 KiByte/s!
					}

					case IOType.UdpClient:
					case IOType.UdpServer:
					case IOType.UdpPairSocket:
					{
						// In YAT practise (e.g. 'Huge.txt'):
						// Approx. 5% less than TCP/IP.
					#if (DEBUG)
						return (19.5 * 0.95);
					#else
						return (43.6 * 0.95);
					#endif
					}

					case IOType.UsbSerialHid:
					{
						// In theory:
						// 64 KiByte/s (https://www.tracesystemsinc.com/USB_Tutorials_web/USB/B1_USB_Classes/Books/A3_A_Closer_Look_at_HID_Class/slide01.htm).
						//
						// In YAT practise:
						// Likely less.
					#if (DEBUG)
						return (10);
					#else
						return (20);
					#endif
					}

					default:
					{
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + IOType + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		/// <remarks>
		/// Value is approximate! It may be off by a factor of 2..5, depending on environment and I/O related settings!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <remarks>
		/// Named "Bytes" rather than "Octets" as that is more common, and .NET uses "Byte" as well.
		/// </remarks>
		/// <remarks>
		/// Named this long to make inaccuracy obvious.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the scope.")]
		[XmlIgnore]
		protected virtual double CalculatedApproximateTypicalNumberOfBytesPerMillisecond_SerialPort
		{
			get
			{
				// Measurements 2020-08-17 using two interconnected serial COM port terminals,
				// transmitting 'Large.txt' (~80 KiB) / 'Huge.txt' (~1 MiB):
				//
				// [Debug]
				//  >     0  baud                      :          =>      0 bytes/s
				//  >  9600  baud =   0.96 frames/ms @1: t = 1:55 =>   ~715 bytes/s
				//  > 115.2 kbaud =  11.52 frames/ms @1: t = 0:16 =>  ~5150 bytes/s ]
				//  > 115.2 kbaud =  11.52 frames/ms @2: t = 1:55 =>  ~9500 bytes/s ] interpolated to 7500 bytes/s
				//  > 256   kbaud =  25.6  frames/ms @2: t = 1:00 => ~18150 bytes/s
				//  >   1   Mbaud = 100    frames/ms @2: t = 0:53 => ~20600 bytes/s
				//  >   3   Mbaud = 300    frames/ms @2: t = 0:49 => ~22250 bytes/s
				//
				// [Release]
				//  >     0  baud                      :          =>      0 bytes/s
				//  >  9600  baud =   0.96 frames/ms @1: t = 1:55 =>   ~715 bytes/s
				//  > 115.2 kbaud =  11.52 frames/ms @1: t = 0:15 =>  ~5500 bytes/s ]
				//  > 115.2 kbaud =  11.52 frames/ms @2: t = 1:34 => ~11600 bytes/s ] interpolated to 8000 bytes/s
				//  > 256   kbaud =  25.6  frames/ms @2: t = 0:46 => ~23700 bytes/s
				//  >   1   Mbaud = 100    frames/ms @2: t = 0:26 => ~41900 bytes/s
				//  >   3   Mbaud = 300    frames/ms @2: t = 0:23 => ~47400 bytes/s
				//
				// @1 'Large.txt' using default settings, i.e. limited to baud rate and 48 byte chunks.
				// @2 'Huge.txt' using optimized settings, i.e. unlimited.
				//
				// measured throughput [bytes/ms]
				//  ^
				//  |                                        *
				//  |       *
				//  |   *
				//  | *
				//  |*
				//  *-----------------------------------------> configured rate [frames/ms]
				//  0 0.96 11.52 25.6 100                   300
				//
				// Using https://mycurvefit.com/ to fit the measured values into a function:
				//  > [Power] [y = a * x^b] would require using Pow(), i.e. less performant.
				//  > [Michaelis-Menten] [y = (Vmax * x) / (Km + x)] is more performant.
				//
				// Attention, while fitting may be fine, individual rates may not, e.g. the
				// initial y = (24.08415 * x) / (15.65619 + x) with 9600 baud:
				// x = 0.96
				// y = (24.08415 * 0.96) / (15.65619 + 0.96) = 1.39 instead of 0.715.
				// Also 115.2 kbaud which is 5500...11600 bytes/s, i.e. also factor ~2.
				//
				// Therefore decided to fit without the data point at 3 Mbaud, resulting in
				// better fit for lower baud rates:
				// x = 0.96
				// y = (26.63939 * 0.96) / (22.87870 + 0.96) = 1.07 instead of 0.715.
				// x = 300
				// y = (26.63939 * 300) / (22.87870 + 300) = 24.75 instead of 22.25.

				var x = this.serialPort.Communication.FramesPerMillisecond;
			#if (DEBUG)
				//   x     y
				//   0     0
				//   0.96  0.715
				//  11.52  5.15
				//  25.6  18.15
				// 100    20.6
				// 300    22.25
				//
				// y = (24.61117 * x) / (19.36483 + x) with 115.2 kbaud ~5150 bytes/s (measured)
				// y = (23.65423 * x) / (12.84387 + x) with 115.2 kbaud ~9500 bytes/s (measured)
				// y = (24.08415 * x) / (15.65619 + x) with 115.2 kbaud ~7500 bytes/s (interpolated)
				//
				//   x     y
				//   0     0
				//   0.96  0.715
				//  11.52  5.15
				//  25.6  18.15
				// 100    20.6
				//
				// y = (26.63939 * x) / (22.87870 + x) with 115.2 kbaud ~7500 bytes/s (interpolated)
				return (26.63939 * x) / (22.87870 + x);
			#else
				//   x     y
				//   0     0
				//   0.96  0.715
				//  11.52  8
				//  25.6  23.7
				// 100    41.9
				// 300    47.7
				//
				// y = (56.47081 * x) / (43.93722 + x) with 115.2 kbaud  ~5500 bytes/s (measured)
				// y = (54.15617 * x) / (34.62719 + x) with 115.2 kbaud ~11600 bytes/s (measured)
				// y = (55.52183 * x) / (40.01806 + x) with 115.2 kbaud  ~8000 bytes/s (interpolated)
				//
				//   x     y
				//   0     0
				//   0.96  0.715
				//  11.52  8
				//  25.6  23.7
				// 100    41.9
				//
				// y = (65.25333 * x) / (54.17651 + x) with 115.2 kbaud  ~8000 bytes/s (interpolated)
				return (65.25333 * x) / (54.17651 + x);
			#endif

				// Notes:
				//  > Ignoring mismatch in case 7 data bits are being used.
				//  > Ignoring mismatch in case non-8-bit bytes are being used.
				//  > No need to consider overhead, incl. safety margin as measure values already include this.
				//     > 20% typical overhead for lower baud rates
				//     > Overhead increases for higher baud rates.
			}
		}

		/// <summary>
		/// Hiding XOn/XOff only makes sense for I/O where XOn/XOff is known to be used.
		/// </summary>
		[XmlIgnore]
		public bool SupportsHideXOnXOff
		{
			get
			{
				switch (IOType)
				{                                     //// Always support, as users may intentionally configure YAT's
					case IOType.SerialPort:   return (true); // [FlowControl] to [None] even though a device supports XOn/XOff.
					case IOType.UsbSerialHid: return (FlowControlIsInUse);

					default:                  return (false);
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool FlowControlIsInUse
		{
			get
			{
				switch (IOType)
				{
					case IOType.SerialPort:   return (this.serialPort.Communication.FlowControlIsInUse);
					case IOType.UsbSerialHid: return (this.usbSerialHidDevice      .FlowControlIsInUse);

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
				switch (IOType)
				{
					case IOType.SerialPort:   return (this.serialPort.Communication.FlowControlUsesXOnXOff);
					case IOType.UsbSerialHid: return (this.usbSerialHidDevice      .FlowControlUsesXOnXOff);

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
				switch (IOType)
				{
					case IOType.SerialPort:   return (this.serialPort.Communication.FlowControlManagesXOnXOffManually);
					case IOType.UsbSerialHid: return (this.usbSerialHidDevice      .FlowControlManagesXOnXOffManually);

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

		/// <remarks>Intentionally not orthogonal, returning <c>false</c> on get but throwing on set.</remarks>
		[XmlIgnore]
		public virtual bool SignalXOnWhenOpened
		{
			get
			{
				switch (IOType)
				{
					case IOType.SerialPort:   return (this.serialPort        .SignalXOnWhenOpened);
					case IOType.UsbSerialHid: return (this.usbSerialHidDevice.SignalXOnWhenOpened);

					default:                  return (false);
				}
			}
			set
			{
				switch (IOType)
				{
					case IOType.SerialPort:   this.serialPort        .SignalXOnWhenOpened = value; break;
					case IOType.UsbSerialHid: this.usbSerialHidDevice.SignalXOnWhenOpened = value; break;

					default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'SignalXOnWhenOpened' can only be applied to serial COM ports or USB Ser/HID devices!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + ioType + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
