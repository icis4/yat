//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\SerialPort for better separation of the implementation files.
namespace MKY.IO.Serial
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
		/// Returns <c>true</c> if the RTS/CTS lines are use, i.e. if one or the other kind of RTS/CTS
		/// flow control is active.
		/// </summary>
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

				(this.baudRate    == other.baudRate) &&
				(this.dataBits    == other.dataBits) &&
				(this.parity      == other.parity) &&
				(this.stopBits    == other.stopBits) &&
				(this.flowControl == other.flowControl)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.baudRate   .GetHashCode() ^
				this.dataBits   .GetHashCode() ^
				this.parity     .GetHashCode() ^
				this.stopBits   .GetHashCode() ^
				this.flowControl.GetHashCode()
			);
		}

		/// <summary></summary>
		public override string ToString()
		{
			return
			(
				this.baudRate                           + ", " +
				((MKY.IO.Ports.DataBitsEx)this.dataBits) + ", " +
				((MKY.IO.Ports.ParityEx)  this.parity)   + ", " +
				((MKY.IO.Ports.StopBitsEx)this.stopBits) + ", " +
				((SerialFlowControlEx)this.flowControl).ToShortString()
			);
		}

		#endregion

		/// <summary></summary>
		public virtual string ToShortString()
		{
			return
			(
				this.baudRate                            + ", " +
				((MKY.IO.Ports.DataBitsEx)this.dataBits) + ", " +
				((MKY.IO.Ports.ParityEx)  this.parity).ToShortString()
			);
		}

		/// <summary></summary>
		public virtual string ToLongString()
		{
			return
			(
				this.baudRate                            + ", " +
				((MKY.IO.Ports.DataBitsEx)this.dataBits) + ", " +
				((MKY.IO.Ports.ParityEx)  this.parity)   + ", " +
				((MKY.IO.Ports.StopBitsEx)this.stopBits) + ", " +
				((SerialFlowControlEx)    this.flowControl)
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
