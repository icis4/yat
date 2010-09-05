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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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

using MKY.IO.Ports;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\SerialPort for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	[Serializable]
	public class SerialPortSettings : MKY.Utilities.Settings.Settings, IEquatable<SerialPortSettings>
	{
		/// <summary></summary>
		public static readonly AutoRetry AutoReopenDefault = new AutoRetry(true, 2000);

		/// <summary></summary>
		public const bool ReplaceParityErrorsDefault = false;

		/// <summary></summary>
		public const byte ParityErrorReplacementDefault = 0x00;

		private SerialPortId portId;
		private SerialCommunicationSettings communication;
		private AutoRetry autoReopen;
		private bool replaceParityErrors;
		private byte parityErrorReplacement;
		private bool rtsEnabled;
		private bool dtrEnabled;

		/// <summary></summary>
		public SerialPortSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public SerialPortSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			Communication = new SerialCommunicationSettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public SerialPortSettings(SerialPortSettings rhs)
			: base(rhs)
		{
			// Attention: Port ID can be null (if no COM ports are available on system).
			if (rhs.PortId != null)
				PortId = new SerialPortId(rhs.PortId);
			else
				PortId = null;

			Communication               = new SerialCommunicationSettings(rhs.Communication);
			this.autoReopen             = rhs.autoReopen;
			this.replaceParityErrors    = rhs.replaceParityErrors;
			this.parityErrorReplacement = rhs.parityErrorReplacement;
			this.rtsEnabled             = rhs.RtsEnabled;
			this.dtrEnabled             = rhs.DtrEnabled;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			PortId                 = SerialPortId.DefaultPort;
			AutoReopen             = AutoReopenDefault;
			ReplaceParityErrors    = ReplaceParityErrorsDefault;
			ParityErrorReplacement = ParityErrorReplacementDefault;
			RtsEnabled             = true;
			DtrEnabled             = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("PortId")]
		public virtual SerialPortId PortId
		{
			get { return (this.portId); }
			set
			{
				if (value != this.portId)
				{
					this.portId = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Communication")]
		public virtual SerialCommunicationSettings Communication
		{
			get { return (this.communication); }
			set
			{
				if (this.communication == null)
				{
					this.communication = value;
					AttachNode(this.communication);
				}
				else if (value != this.communication)
				{
					SerialCommunicationSettings old = this.communication;
					this.communication = value;
					ReplaceNode(old, this.communication);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoReopen")]
		public virtual AutoRetry AutoReopen
		{
			get { return (this.autoReopen); }
			set
			{
				if (value != this.autoReopen)
				{
					this.autoReopen = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ReplaceParityErrors")]
		public virtual bool ReplaceParityErrors
		{
			get { return (this.replaceParityErrors); }
			set
			{
				if (value != this.replaceParityErrors)
				{
					this.replaceParityErrors = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ParityErrorReplacement")]
		public virtual byte ParityErrorReplacement
		{
			get { return (this.parityErrorReplacement); }
			set
			{
				if (value != this.parityErrorReplacement)
				{
					this.parityErrorReplacement = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "Abbreviation RTS is given by RS-232.")]
		[XmlElement("RtsEnabled")]
		public virtual bool RtsEnabled
		{
			get { return (this.rtsEnabled); }
			set
			{
				if (value != this.rtsEnabled)
				{
					this.rtsEnabled = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "Abbreviation DTR is given by RS-232.")]
		[XmlElement("DtrEnabled")]
		public virtual bool DtrEnabled
		{
			get { return (this.dtrEnabled); }
			set
			{
				if (value != this.dtrEnabled)
				{
					this.dtrEnabled = value;
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

			SerialPortSettings casted = obj as SerialPortSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SerialPortSettings other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)other) && // Compare all settings nodes.

				(this.portId                 == other.portId) &&
				(this.communication          == other.communication) &&
				(this.autoReopen             == other.autoReopen) &&
				(this.replaceParityErrors    == other.replaceParityErrors) &&
				(this.parityErrorReplacement == other.parityErrorReplacement) &&
				(this.rtsEnabled             == other.rtsEnabled) &&
				(this.dtrEnabled             == other.dtrEnabled)
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
		public static bool operator ==(SerialPortSettings lhs, SerialPortSettings rhs)
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
		public static bool operator !=(SerialPortSettings lhs, SerialPortSettings rhs)
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
