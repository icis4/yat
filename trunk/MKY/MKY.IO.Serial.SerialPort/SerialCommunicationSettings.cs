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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace MKY.IO.Serial.SerialPort
{
	/// <summary></summary>
	[Serializable]
	public class SerialCommunicationSettings : Settings.SettingsItem, IEquatable<SerialCommunicationSettings>
	{
		/// <summary></summary>
		public const int BaudRateDefault = MKY.IO.Ports.SerialPortSettings.BaudRateDefault;

		/// <summary></summary>
		public const MKY.IO.Ports.DataBits DataBitsDefault = MKY.IO.Ports.SerialPortSettings.DataBitsDefault;

		/// <summary></summary>
		public const System.IO.Ports.Parity ParityDefault = MKY.IO.Ports.SerialPortSettings.ParityDefault;

		/// <summary></summary>
		public const System.IO.Ports.StopBits StopBitsDefault = MKY.IO.Ports.SerialPortSettings.StopBitsDefault;

		/// <summary></summary>
		public const SerialFlowControl FlowControlDefault = SerialFlowControl.None;

		private int baudRate;
		private MKY.IO.Ports.DataBits dataBits;
		private System.IO.Ports.Parity parity;
		private System.IO.Ports.StopBits stopBits;
		private SerialFlowControl flowControl;

		private SerialControlPinState rtsPin;
		private SerialControlPinState dtrPin;

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SerialCommunicationSettings()
			: this(Settings.SettingsType.Explicit)
		{
		}

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SerialCommunicationSettings(Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public SerialCommunicationSettings(int baudRate, MKY.IO.Ports.DataBits dataBits = DataBitsDefault, System.IO.Ports.Parity parity = ParityDefault, System.IO.Ports.StopBits stopBits = StopBitsDefault, SerialFlowControl flowControl = FlowControlDefault)
			: this(baudRate, dataBits, parity, stopBits, flowControl, ToRtsPinDefault(flowControl), ToDtrPinDefault(flowControl))
		{
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "rts", Justification = "'RTS' is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dtr", Justification = "'DTR' is a common term for serial ports.")]
		public SerialCommunicationSettings(int baudRate, MKY.IO.Ports.DataBits dataBits, System.IO.Ports.Parity parity, System.IO.Ports.StopBits stopBits, SerialFlowControl flowControl, SerialControlPinState rtsPin, SerialControlPinState dtrPin)
		{
			BaudRate    = baudRate;
			DataBits    = dataBits;
			Parity      = parity;
			StopBits    = stopBits;
			FlowControl = flowControl;

			// Override the default pin settings by the provided values:
			RtsPin = rtsPin;
			DtrPin = dtrPin;
		}

		/// <summary>
		/// Creates new port settings from <paramref name="rhs"/>.
		/// </summary>
		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SerialCommunicationSettings(SerialCommunicationSettings rhs)
			: base(rhs)
		{
			BaudRate    = rhs.BaudRate;
			DataBits    = rhs.DataBits;
			Parity      = rhs.Parity;
			StopBits    = rhs.StopBits;
			FlowControl = rhs.FlowControl;

			// Override the default pin settings by the provided values:
			RtsPin = rhs.RtsPin;
			DtrPin = rhs.DtrPin;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			BaudRate    = BaudRateDefault;
			DataBits    = DataBitsDefault;
			Parity      = ParityDefault;
			StopBits    = StopBitsDefault;
			FlowControl = FlowControlDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("BaudRate")]
		public virtual int BaudRate
		{
			get { return (this.baudRate); }
			set
			{
				if (this.baudRate != value)
				{
					this.baudRate = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DataBits")]
		public virtual MKY.IO.Ports.DataBits DataBits
		{
			get { return (this.dataBits); }
			set
			{
				if (this.dataBits != value)
				{
					this.dataBits = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Parity")]
		public virtual System.IO.Ports.Parity Parity
		{
			get { return (this.parity); }
			set
			{
				if (this.parity != value)
				{
					this.parity = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("StopBits")]
		public virtual System.IO.Ports.StopBits StopBits
		{
			get { return (this.stopBits); }
			set
			{
				if (this.stopBits != value)
				{
					this.stopBits = value;
					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// The number of bits per frame.
		/// </summary>
		/// <remarks>
		/// Typically an integral value, but may be .5 in case of
		/// <see cref="System.IO.Ports.StopBits.OnePointFive"/>.
		/// </remarks>
		[XmlIgnore]
		public virtual double BitsPerFrame
		{
			get
			{
				double value = 1.0; // Start bit.

				value += (MKY.IO.Ports.DataBitsEx)DataBits;

				switch (Parity)
				{
					case System.IO.Ports.Parity.Odd:
					case System.IO.Ports.Parity.Even:
					case System.IO.Ports.Parity.Mark:
					case System.IO.Ports.Parity.Space:
					{
						value += 1.0; // Parity bit.
						break;
					}
				}

				value += (MKY.IO.Ports.StopBitsEx)StopBits;

				return (value);
			}
		}

		/// <summary>
		/// The frame duration in milliseconds.
		/// </summary>
		[XmlIgnore]
		public virtual double FrameTime
		{
			get
			{
				return ((BitsPerFrame / (MKY.IO.Ports.BaudRateEx)BaudRate) * 1000);
			}
		}

		/// <summary>
		/// The number of frames per millisecond. Typically equivalent to
		/// the number of bytes per millisecond.
		/// </summary>
		[XmlIgnore]
		public virtual double FramesPerMillisecond
		{
			get
			{
				return (1 / FrameTime);
			}
		}

		/// <summary></summary>
		[XmlElement("FlowControl")]
		public virtual SerialFlowControl FlowControl
		{
			get { return (this.flowControl); }
			set
			{
				if (this.flowControl != value)
				{
					this.flowControl = value;

					// Set the default pin values accordingly:
					RtsPin = ToRtsPinDefault(FlowControl);
					DtrPin = ToDtrPinDefault(FlowControl);

					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// Returns <c>true</c> if flow control is in use.
		/// </summary>
		public virtual bool FlowControlIsInUse
		{
			get
			{
				return ((this.flowControl != SerialFlowControl.None) &&
				        (this.flowControl != SerialFlowControl.RS485));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the RTS/CTS lines are use, i.e. if one or the other kind of
		/// hardware flow control is active.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "'CTS' is a common term for serial ports.")]
		public virtual bool FlowControlUsesRtsCts
		{
			get
			{
				return ((this.flowControl == SerialFlowControl.Hardware) ||
				        (this.flowControl == SerialFlowControl.Combined) ||
				        (this.flowControl == SerialFlowControl.ManualHardware) ||
				        (this.flowControl == SerialFlowControl.ManualCombined));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the RTS/CTS control pins are managed automatically.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "'CTS' is a common term for serial ports.")]
		public virtual bool FlowControlManagesRtsCtsAutomatically
		{
			get
			{
				return ((this.flowControl == SerialFlowControl.Hardware) ||
				        (this.flowControl == SerialFlowControl.Combined) ||
				        (this.flowControl == SerialFlowControl.RS485));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the DTR/DSR control pins are managed automatically.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "'DSR' is a common term for serial ports.")]
		public virtual bool FlowControlManagesDtrDsrAutomatically
		{
			get { return (false); } // Not given for any of the supported settings.
		}

		/// <summary>
		/// Returns <c>true</c> if XOn/XOff is in use, i.e. if one or the other kind of XOn/XOff
		/// flow control is active.
		/// </summary>
		public virtual bool FlowControlUsesXOnXOff
		{
			get
			{
				return ((this.flowControl == SerialFlowControl.Software) ||
				        (this.flowControl == SerialFlowControl.Combined) ||
				        (this.flowControl == SerialFlowControl.ManualSoftware) ||
				        (this.flowControl == SerialFlowControl.ManualCombined));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if XOn/XOff is managed manually.
		/// </summary>
		public virtual bool FlowControlManagesXOnXOffManually
		{
			get
			{
				return ((this.flowControl == SerialFlowControl.ManualSoftware) ||
				        (this.flowControl == SerialFlowControl.ManualCombined));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		[XmlElement("RtsPin")]
		public virtual SerialControlPinState RtsPin
		{
			get { return (this.rtsPin); }
			set
			{
				if (this.rtsPin != value)
				{
					this.rtsPin = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		[XmlElement("DtrPin")]
		public virtual SerialControlPinState DtrPin
		{
			get { return (this.dtrPin); }
			set
			{
				if (this.dtrPin != value)
				{
					this.dtrPin = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ToRts", Justification = "'RTS' is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		public static SerialControlPinState ToRtsPinDefault(SerialFlowControl flowControl)
		{
			switch (flowControl)
			{
				case SerialFlowControl.Hardware:
				case SerialFlowControl.Combined:
					return (SerialControlPinState.Automatic);

				case SerialFlowControl.ManualHardware:
				case SerialFlowControl.ManualCombined:
					return (SerialControlPinState.Enabled);

				case SerialFlowControl.RS485:
					return (SerialControlPinState.Disabled); // Will be enabled for each frame.

				default: // Includes 'None', 'Software', 'ManualSoftware'
					return (SerialControlPinState.Disabled);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		public static SerialControlPinState ToDtrPinDefault(SerialFlowControl flowControl)
		{
			switch (flowControl)
			{
				case SerialFlowControl.Hardware:
				case SerialFlowControl.Combined:
				case SerialFlowControl.ManualHardware:
				case SerialFlowControl.ManualCombined:
					return (SerialControlPinState.Enabled);

					// Certain devices require the DTR pin to be active in case of hardware flow
					// control. This e.g. applies to USB Ser/CDC (i.e. USB CDC/ACM) devices, which
					// indicate "DTE not present" if DTR is inactive. Also applies to modems and
					// and similar, where DTR/DSR are supposed to be active for the session.

				case SerialFlowControl.RS485:
					return (SerialControlPinState.Disabled);

				default: // Includes 'None', 'Software', 'ManualSoftware'
					return (SerialControlPinState.Disabled);
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return
			(
				                         BaudRate + ", " +
				(MKY.IO.Ports.DataBitsEx)DataBits + ", " +
				(MKY.IO.Ports.ParityEx)  Parity   + ", " +
				(MKY.IO.Ports.StopBitsEx)StopBits + ", " +
				((SerialFlowControlEx)   FlowControl).ToShortString()

				// Do not include the state of the RTS and DTR pins, as these are advanced settings typically not displayed.
			);
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			return
			(
				                         BaudRate + ", " +
				(MKY.IO.Ports.DataBitsEx)DataBits + ", " +
				((MKY.IO.Ports.ParityEx) Parity).ToShortString()
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

				hashCode = (hashCode * 397) ^ BaudRate;
				hashCode = (hashCode * 397) ^ DataBits   .GetHashCode();
				hashCode = (hashCode * 397) ^ Parity     .GetHashCode();
				hashCode = (hashCode * 397) ^ StopBits   .GetHashCode();
				hashCode = (hashCode * 397) ^ FlowControl.GetHashCode();

				hashCode = (hashCode * 397) ^ RtsPin     .GetHashCode();
				hashCode = (hashCode * 397) ^ DtrPin     .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialCommunicationSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialCommunicationSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				BaudRate   .Equals(other.BaudRate)    &&
				DataBits   .Equals(other.DataBits)    &&
				Parity     .Equals(other.Parity)      &&
				StopBits   .Equals(other.StopBits)    &&
				FlowControl.Equals(other.FlowControl) &&

				RtsPin     .Equals(other.RtsPin)      &&
				DtrPin     .Equals(other.DtrPin)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SerialCommunicationSettings lhs, SerialCommunicationSettings rhs)
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
		public static bool operator !=(SerialCommunicationSettings lhs, SerialCommunicationSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <summary>
		/// Parses <paramref name="s"/> for serial communication settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialCommunicationSettings Parse(string s)
		{
			SerialCommunicationSettings result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify valid serial communication settings"));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for serial communication settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialCommunicationSettings settings)
		{
			var delimiters = " ,;|";
			var sa = s.Trim().Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (sa.Length > 0)
			{
				MKY.IO.Ports.BaudRateEx baudRate;
				if (MKY.IO.Ports.BaudRateEx.TryParse(sa[0], out baudRate))
				{
					if (sa.Length > 1)
					{
						MKY.IO.Ports.DataBits dataBits;
						if (MKY.IO.Ports.DataBitsEx.TryParse(sa[1], out dataBits))
						{
							if (sa.Length > 2)
							{
								System.IO.Ports.Parity parity;
								if (MKY.IO.Ports.ParityEx.TryParse(sa[2], out parity))
								{
									if (sa.Length > 3)
									{
										System.IO.Ports.StopBits stopBits;
										if (MKY.IO.Ports.StopBitsEx.TryParse(sa[3], out stopBits))
										{
											if (sa.Length > 4)
											{
												SerialFlowControl flowControl;
												if (SerialFlowControlEx.TryParse(sa[4], out flowControl))
												{
													settings = new SerialCommunicationSettings(baudRate, dataBits, parity, stopBits, flowControl);
													return (true);
												}
											}
											else
											{
												settings = new SerialCommunicationSettings(baudRate, dataBits, parity, stopBits);
												return (true);
											}
										}
									}
									else
									{
										settings = new SerialCommunicationSettings(baudRate, dataBits, parity);
										return (true);
									}
								}
							}
							else
							{
								settings = new SerialCommunicationSettings(baudRate, dataBits);
								return (true);
							}
						}
					}

					settings = new SerialCommunicationSettings(baudRate);
					return (true);
				}
			}

			settings = null;
			return (false);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
