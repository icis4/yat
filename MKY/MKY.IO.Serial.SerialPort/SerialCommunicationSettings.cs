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
	public class SerialCommunicationSettings : MKY.Settings.SettingsItem
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

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SerialCommunicationSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SerialCommunicationSettings(int baudRate, MKY.IO.Ports.DataBits dataBits, System.IO.Ports.Parity parity, System.IO.Ports.StopBits stopBits, SerialFlowControl flowControl)
		{
			BaudRate    = baudRate;
			DataBits    = dataBits;
			Parity      = parity;
			StopBits    = stopBits;
			FlowControl = flowControl;
		}

		/// <summary></summary>
		public SerialCommunicationSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
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
					SetChanged();
				}
			}
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
		/// Returns <c>true</c> if flow control is active, i.e. the receiver can pause the sender.
		/// </summary>
		public virtual bool FlowControlIsActive
		{
			get { return (!FlowControlIsInactive); }
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
		/// Returns <c>true</c> if the RFR/CTS and/or DTR/DSR lines are managed manually.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "DSR is a common term for serial ports.")]
		public virtual bool FlowControlManagesRfrCtsDtrDsrManually
		{
			get
			{
				return ((this.flowControl == SerialFlowControl.ManualHardware) ||
						(this.flowControl == SerialFlowControl.ManualCombined));
			}
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

				(BaudRate    == other.BaudRate) &&
				(DataBits    == other.DataBits) &&
				(Parity      == other.Parity)   &&
				(StopBits    == other.StopBits) &&
				(FlowControl == other.FlowControl)
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

				BaudRate   .GetHashCode() ^
				DataBits   .GetHashCode() ^
				Parity     .GetHashCode() ^
				StopBits   .GetHashCode() ^
				FlowControl.GetHashCode()
			);
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
