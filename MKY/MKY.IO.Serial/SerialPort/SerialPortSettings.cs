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
// MKY Development Version 1.0.8
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
	public class SerialPortSettings : MKY.Settings.SettingsItem
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>
		/// Must be implemented as property that creates a new id object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same id object.
		/// </remarks>
		public static AutoRetry AutoReopenDefault
		{
			get { return (new AutoRetry(true, 2000)); }
		}

		/// <summary></summary>
		public const int AutoReopenMinimumInterval = 100;

		/// <summary></summary>
		public const byte XOnByte = MKY.IO.Ports.SerialPortSettings.XOnByte;

		/// <summary></summary>
		public const byte XOffByte = MKY.IO.Ports.SerialPortSettings.XOffByte;

		/// <summary></summary>
		public const string XOnDescription = MKY.IO.Ports.SerialPortSettings.XOnDescription;

		/// <summary></summary>
		public const string XOffDescription = MKY.IO.Ports.SerialPortSettings.XOffDescription;

		/// <summary></summary>
		public const bool NoSendOnOutputBreakDefault = true;

		/// <summary></summary>
		public const bool NoSendOnInputBreakDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SerialPortId portId;
		private SerialCommunicationSettings communication;
		private AutoRetry autoReopen;
		private bool noSendOnOutputBreak;
		private bool noSendOnInputBreak;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

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

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SerialPortSettings(SerialPortSettings rhs)
			: base(rhs)
		{
			// Attention: Port ID can be null (if no COM ports are available on system).
			if (rhs.PortId != null)
				PortId = new SerialPortId(rhs.PortId);
			else
				PortId = null;

			Communication       = new SerialCommunicationSettings(rhs.Communication);
			AutoReopen          = rhs.autoReopen;
			NoSendOnOutputBreak = rhs.NoSendOnOutputBreak;
			NoSendOnInputBreak  = rhs.NoSendOnInputBreak;

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
			base.SetMyDefaults();

			// Attention: See remarks above.
			PortId                 = SerialPortId.FirstStandardPort;

			AutoReopen          = AutoReopenDefault;
			NoSendOnOutputBreak = NoSendOnOutputBreakDefault;
			NoSendOnInputBreak  = NoSendOnInputBreakDefault;
		}

		#endregion

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

				(this.portId              == other.portId) &&
				(this.communication       == other.communication) &&
				(this.autoReopen          == other.autoReopen) &&
				(this.noSendOnOutputBreak == other.noSendOnOutputBreak) &&
				(this.noSendOnInputBreak  == other.noSendOnInputBreak)
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

				portIdHashCode           .GetHashCode() ^
				this.communication       .GetHashCode() ^
				this.autoReopen          .GetHashCode() ^
				this.noSendOnOutputBreak .GetHashCode() ^
				this.noSendOnInputBreak  .GetHashCode()
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
