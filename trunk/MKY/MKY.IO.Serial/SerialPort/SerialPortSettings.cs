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
	public class SerialPortSettings : MKY.Settings.Settings
	{
		/// <summary></summary>
		public static readonly AutoRetry AutoReopenDefault = new AutoRetry(true, 2000);

		/// <summary></summary>
		public const bool ReplaceParityErrorsDefault = false;

		/// <summary></summary>
		public const byte ParityErrorReplacementDefault = 0x00;

		/// <summary></summary>
		public const bool NoSendOnOutputBreakDefault = true;

		/// <summary></summary>
		public const bool NoSendOnInputBreakDefault = false;

		private SerialPortId portId;
		private SerialCommunicationSettings communication;
		private AutoRetry autoReopen;
		private bool replaceParityErrors;
		private byte parityErrorReplacement;
		private bool rtsEnabled;
		private bool dtrEnabled;
		private bool noSendOnOutputBreak;
		private bool noSendOnInputBreak;

		/// <summary></summary>
		public SerialPortSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public SerialPortSettings(MKY.Settings.SettingsType settingsType)
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

			Communication          = new SerialCommunicationSettings(rhs.Communication);
			AutoReopen             = rhs.autoReopen;
			ReplaceParityErrors    = rhs.replaceParityErrors;
			ParityErrorReplacement = rhs.parityErrorReplacement;
			RtsEnabled             = rhs.RtsEnabled;
			DtrEnabled             = rhs.DtrEnabled;
			NoSendOnOutputBreak    = rhs.NoSendOnOutputBreak;
			NoSendOnInputBreak       = rhs.NoSendOnInputBreak;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// 
		/// Attention: Do not use <see cref="MKY.IO.Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="MKY.IO.Ports.SerialPortId.FirstStandardPort"/>
		/// is way more performant and good enough for most use cases.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			// Attention: See remarks above.
			PortId                 = SerialPortId.FirstStandardPort;

			AutoReopen             = AutoReopenDefault;
			ReplaceParityErrors    = ReplaceParityErrorsDefault;
			ParityErrorReplacement = ParityErrorReplacementDefault;
			RtsEnabled             = true;
			DtrEnabled             = true;
			NoSendOnOutputBreak    = NoSendOnOutputBreakDefault;
			NoSendOnInputBreak       = NoSendOnInputBreakDefault;
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
				if (value == null)
				{
					this.communication = value;
					DetachNode(this.communication);
				}
				else if (this.communication == null)
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

		/// <summary></summary>
		[XmlElement("NoSendOnOutputBreak")]
		public virtual bool NoSendOnOutputBreak
		{
			get { return (this.noSendOnOutputBreak); }
			set
			{
				if (value != this.noSendOnOutputBreak)
				{
					this.noSendOnOutputBreak = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NoSendOnInputBreak")]
		public virtual bool NoSendOnInputBreak
		{
			get { return (this.noSendOnInputBreak); }
			set
			{
				if (value != this.noSendOnInputBreak)
				{
					this.noSendOnInputBreak = value;
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SerialPortSettings other = (SerialPortSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.portId                 == other.portId) &&
				(this.communication          == other.communication) &&
				(this.autoReopen             == other.autoReopen) &&
				(this.replaceParityErrors    == other.replaceParityErrors) &&
				(this.parityErrorReplacement == other.parityErrorReplacement) &&
				(this.rtsEnabled             == other.rtsEnabled) &&
				(this.dtrEnabled             == other.dtrEnabled) &&
				(this.noSendOnOutputBreak    == other.noSendOnOutputBreak) &&
				(this.noSendOnInputBreak       == other.noSendOnInputBreak)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			int portIdHashCode = 0;
			if (this.portId != null)
				portIdHashCode = this.portId.GetHashCode();

			return
			(
				base.GetHashCode() ^

				portIdHashCode             .GetHashCode() ^
				this.communication         .GetHashCode() ^
				this.autoReopen            .GetHashCode() ^
				this.replaceParityErrors   .GetHashCode() ^
				this.parityErrorReplacement.GetHashCode() ^
				this.rtsEnabled            .GetHashCode() ^
				this.dtrEnabled            .GetHashCode() ^
				this.noSendOnOutputBreak   .GetHashCode() ^
				this.noSendOnInputBreak      .GetHashCode()
			);
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
