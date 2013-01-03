//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
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

		/// <summary></summary>
		public SerialCommunicationSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SerialCommunicationSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

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
				if (value != this.baudRate)
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
				if (value != this.dataBits)
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
				if (value != this.parity)
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
				if (value != this.stopBits)
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
				if (value != this.flowControl)
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
		/// Returns <c>true</c> if the RTS/CTS lines are use, i.e. if one or the other kind of RTS/CTS
		/// flow control is active.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "RTS is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
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
		/// Returns <c>true</c> if the RTS/CTS and/or DTR/DSR lines are managed manually.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "RTS is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "DSR is a common term for serial ports.")]
		public virtual bool FlowControlManagesRtsCtsDtrDsrManually
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
				base.GetHashCode() ^

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

		#endregion

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

		/// <summary></summary>
		public virtual string ToLongString()
		{
			return
			(
				BaudRate                            + ", " +
				((MKY.IO.Ports.DataBitsEx)DataBits) + ", " +
				((MKY.IO.Ports.ParityEx)  Parity)   + ", " +
				((MKY.IO.Ports.StopBitsEx)StopBits) + ", " +
				((SerialFlowControlEx)    FlowControl)
			);
		}

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
