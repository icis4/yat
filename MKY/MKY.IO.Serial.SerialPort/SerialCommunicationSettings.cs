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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
	public class SerialCommunicationSettings : Settings.SettingsItem
	{
		/// <summary></summary>
		public const int BaudRateDefault = (int)MKY.IO.Ports.BaudRate.Baud009600;

		/// <summary></summary>
		public const MKY.IO.Ports.DataBits DataBitsDefault = MKY.IO.Ports.DataBits.Eight;

		/// <summary></summary>
		public const System.IO.Ports.Parity ParityDefault = System.IO.Ports.Parity.None;

		/// <summary></summary>
		public const System.IO.Ports.StopBits StopBitsDefault = System.IO.Ports.StopBits.One;

		/// <summary></summary>
		public const SerialFlowControl FlowControlDefault = SerialFlowControl.None;

		private int baudRate;
		private MKY.IO.Ports.DataBits dataBits;
		private System.IO.Ports.Parity parity;
		private System.IO.Ports.StopBits stopBits;
		private SerialFlowControl flowControl;

		private SerialControlPinState rfrPin;
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
		public SerialCommunicationSettings(int baudRate, MKY.IO.Ports.DataBits dataBits, System.IO.Ports.Parity parity, System.IO.Ports.StopBits stopBits, SerialFlowControl flowControl)
			: this(baudRate, dataBits, parity, stopBits, flowControl, ToRfrPinDefault(flowControl), ToDtrPinDefault(flowControl))
		{
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SerialCommunicationSettings(int baudRate, MKY.IO.Ports.DataBits dataBits, System.IO.Ports.Parity parity, System.IO.Ports.StopBits stopBits, SerialFlowControl flowControl, SerialControlPinState rfrPin, SerialControlPinState dtrPin)
		{
			BaudRate    = baudRate;
			DataBits    = dataBits;
			Parity      = parity;
			StopBits    = stopBits;
			FlowControl = flowControl;

			// Override the default pin settings by the provided values:
			RfrPin = rfrPin;
			DtrPin = dtrPin;
		}

		/// <summary>
		/// Creates new port settings from <paramref name="rhs"/>.
		/// </summary>
		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
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
			RfrPin = rhs.RfrPin;
			DtrPin = rhs.DtrPin;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
		/// The frame length in seconds.
		/// </summary>
		[XmlIgnore]
		public virtual double FrameLength
		{
			get
			{
				return (BitsPerFrame / (MKY.IO.Ports.BaudRateEx)BaudRate);
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
					RfrPin = ToRfrPinDefault(FlowControl);
					DtrPin = ToDtrPinDefault(FlowControl);

					SetChanged();
				}
			}
		}

		/// <summary>
		/// Returns <c>true</c> if flow control is active, i.e. the receiver can pause the sender.
		/// </summary>
		public virtual bool FlowControlIsActive
		{
			get { return (!FlowControlIsInactive); }
		}

		/// <summary>
		/// Returns <c>true</c> if flow control is inactive, i.e. the receiver cannot pause the sender.
		/// </summary>
		public virtual bool FlowControlIsInactive
		{
			get
			{
				return ((this.flowControl == SerialFlowControl.None) ||
						(this.flowControl == SerialFlowControl.RS485));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the RFR/CTS lines are use, i.e. if one or the other kind of
		/// hardware flow control is active.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
		public virtual bool FlowControlUsesRfrCts
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
		/// Returns <c>true</c> if the RFR/CTS control pins are managed automatically.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
		public virtual bool FlowControlManagesRfrCtsAutomatically
		{
			get
			{
				return ((this.flowControl == SerialFlowControl.Hardware) ||
						(this.flowControl == SerialFlowControl.Combined));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the DTR/DSR control pins are managed automatically.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "DSR is a common term for serial ports.")]
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
		[XmlElement("RfrPin")]
		public virtual SerialControlPinState RfrPin
		{
			get { return (this.rfrPin); }
			set
			{
				if (this.rfrPin != value)
				{
					this.rfrPin = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DtrPin")]
		public virtual SerialControlPinState DtrPin
		{
			get { return (this.dtrPin); }
			set
			{
				if (this.dtrPin != value)
				{
					this.dtrPin = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public static SerialControlPinState ToRfrPinDefault(SerialFlowControl flowControl)
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
		public static SerialControlPinState ToDtrPinDefault(SerialFlowControl flowControl)
		{
			switch (flowControl)
			{
				case SerialFlowControl.Hardware:
				case SerialFlowControl.Combined:
				case SerialFlowControl.ManualHardware:
				case SerialFlowControl.ManualCombined:
					return (SerialControlPinState.Enabled);

					// Note that certain devices require the DTR pin to be active in case of
					// hardware flow control. This e.g. applies to USB Ser/CDC devices, which
					// indicate "DTE not present" if DTR is inactive. Also applies to modems
					// and similar, where DTR/DSR are supposed to be active for the session.

				case SerialFlowControl.RS485:
					return (SerialControlPinState.Disabled);

				default: // Includes 'None', 'Software', 'ManualSoftware'
					return (SerialControlPinState.Disabled);
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

			SerialCommunicationSettings other = (SerialCommunicationSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(BaudRate    == other.BaudRate)    &&
				(DataBits    == other.DataBits)    &&
				(Parity      == other.Parity)      &&
				(StopBits    == other.StopBits)    &&
				(FlowControl == other.FlowControl) &&

				(RfrPin      == other.RfrPin)      &&
				(DtrPin      == other.DtrPin)
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

				hashCode = (hashCode * 397) ^ RfrPin     .GetHashCode();
				hashCode = (hashCode * 397) ^ DtrPin     .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary></summary>
		public override string ToString()
		{
			return
			(
				BaudRate                            + ", " +
				((MKY.IO.Ports.DataBitsEx)DataBits) + ", " +
				((MKY.IO.Ports.ParityEx)  Parity)   + ", " +
				((MKY.IO.Ports.StopBitsEx)StopBits) + ", " +
				((SerialFlowControlEx)FlowControl).ToShortString()

				// Do not include the state of the RFR and DTR pins, as these are advanced settings typically not displayed.
			);
		}

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
			string delimiters = "/,;";
			string[] sa = s.Trim().Split(delimiters.ToCharArray());
			if (sa.Length == 5)
			{
				MKY.IO.Ports.BaudRateEx baudRate;
				if (MKY.IO.Ports.BaudRateEx.TryParse(sa[0], out baudRate))
				{
					MKY.IO.Ports.DataBits dataBits;
					if (MKY.IO.Ports.DataBitsEx.TryParse(sa[1], out dataBits))
					{
						System.IO.Ports.Parity parity;
						if (MKY.IO.Ports.ParityEx.TryParse(sa[2], out parity))
						{
							System.IO.Ports.StopBits stopBits;
							if (MKY.IO.Ports.StopBitsEx.TryParse(sa[3], out stopBits))
							{
								SerialFlowControl flowControl;
								if (SerialFlowControlEx.TryParse(sa[4], out flowControl))
								{
									settings = new SerialCommunicationSettings(baudRate, dataBits, parity, stopBits, flowControl);
									return (true);
								}
							}
						}
					}
				}
			}

			settings = null;
			return (false);
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string ToShortString()
		{
			return
			(
				BaudRate                            + ", " +
				((MKY.IO.Ports.DataBitsEx)DataBits) + ", " +
				((MKY.IO.Ports.ParityEx)  Parity).ToShortString()
			);
		}

		#endregion

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
