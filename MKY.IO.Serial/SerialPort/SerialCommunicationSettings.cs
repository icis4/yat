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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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
	public class SerialCommunicationSettings : MKY.Utilities.Settings.Settings, IEquatable<SerialCommunicationSettings>
	{
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
		public SerialCommunicationSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public SerialCommunicationSettings(SerialCommunicationSettings rhs)
			: base(rhs)
		{
			this.baudRate    = rhs.BaudRate;
			this.dataBits    = rhs.DataBits;
			this.parity      = rhs.Parity;
			this.stopBits    = rhs.StopBits;
			this.flowControl = rhs.FlowControl;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			BaudRate    = (int)MKY.IO.Ports.BaudRate.Baud009600;
			DataBits    = MKY.IO.Ports.DataBits.Eight;
			Parity      = System.IO.Ports.Parity.None;
			StopBits    = System.IO.Ports.StopBits.One;
			FlowControl = SerialFlowControl.None;
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

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			SerialCommunicationSettings casted = obj as SerialCommunicationSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SerialCommunicationSettings other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)other) && // Compare all settings nodes.

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
			return (base.GetHashCode());
		}

		/// <summary></summary>
		public override string ToString()
		{
			return
				(
				this.baudRate                           + ", " +
				((MKY.IO.Ports.XDataBits)this.dataBits) + ", " +
				((MKY.IO.Ports.XParity)this.parity)     + ", " +
				((MKY.IO.Ports.XStopBits)this.stopBits) + ", " +
				((XSerialFlowControl)this.flowControl).ToShortString()
				);
		}

		#endregion

		/// <summary></summary>
		public virtual string ToShortString()
		{
			return
				(
				this.baudRate                           + ", " +
				((MKY.IO.Ports.XDataBits)this.dataBits) + ", " +
				((MKY.IO.Ports.XParity)this.parity).ToShortString()
				);
		}

		/// <summary></summary>
		public virtual string ToLongString()
		{
			return
				(
				this.baudRate                           + ", " +
				((MKY.IO.Ports.XDataBits)this.dataBits) + ", " +
				((MKY.IO.Ports.XParity)this.parity)     + ", " +
				((MKY.IO.Ports.XStopBits)this.stopBits) + ", " +
				((XSerialFlowControl)this.flowControl)
				);
		}

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SerialCommunicationSettings lhs, SerialCommunicationSettings rhs)
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
		public static bool operator !=(SerialCommunicationSettings lhs, SerialCommunicationSettings rhs)
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
