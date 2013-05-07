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
// MKY Development Version 1.0.10
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
using System.Xml.Serialization;

using MKY.IO.Ports;

namespace MKY.IO.Serial.SerialPort
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
		public const int MaxSendChunkSizeDefault = 50;
		
		/// <summary></summary>
		public const bool NoSendOnOutputBreakDefault = true;

		/// <summary></summary>
		public const bool NoSendOnInputBreakDefault = false;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SerialPortId portId;
		private SerialCommunicationSettings communication;
		private AutoRetry autoReopen;
		private int maxSendChunkSize;
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
			MaxSendChunkSize    = rhs.MaxSendChunkSize;
			NoSendOnOutputBreak = rhs.NoSendOnOutputBreak;
			NoSendOnInputBreak  = rhs.NoSendOnInputBreak;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// 
		/// Attention: Do not use <see cref="MKY.IO.Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="MKY.IO.Ports.SerialPortId.FirstStandardPort"/>
		/// is way better performing and good enough for most use cases.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			// Attention: See remarks above.
			PortId              = SerialPortId.FirstStandardPort;

			AutoReopen          = AutoReopenDefault;
			MaxSendChunkSize    = MaxSendChunkSizeDefault;
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
				if (this.portId != value)
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
					DetachNode(this.communication);
					this.communication = null;
				}
				else if (this.communication == null)
				{
					this.communication = value;
					AttachNode(this.communication);
				}
				else if (this.communication != value)
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
				if (this.autoReopen != value)
				{
					this.autoReopen = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MaxSendChunkSize")]
		public virtual int MaxSendChunkSize
		{
			get { return (this.maxSendChunkSize); }
			set
			{
				if (this.maxSendChunkSize != value)
				{
					this.maxSendChunkSize = value;
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
				if (this.noSendOnOutputBreak != value)
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
				if (this.noSendOnInputBreak != value)
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

			SerialPortSettings other = (SerialPortSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(PortId              == other.PortId) &&
				(Communication       == other.Communication) &&
				(AutoReopen          == other.AutoReopen) &&
				(MaxSendChunkSize    == other.MaxSendChunkSize) &&
				(NoSendOnOutputBreak == other.NoSendOnOutputBreak) &&
				(NoSendOnInputBreak  == other.NoSendOnInputBreak)
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
			int portIdHashCode = 0;
			if (PortId != null)
				portIdHashCode = PortId.GetHashCode();

			return
			(
				base.GetHashCode() ^

				portIdHashCode     .GetHashCode() ^
				Communication      .GetHashCode() ^
				AutoReopen         .GetHashCode() ^
				MaxSendChunkSize   .GetHashCode() ^
				NoSendOnOutputBreak.GetHashCode() ^
				NoSendOnInputBreak .GetHashCode()
			);
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string ToShortPortString()
		{
			if (PortId != null)
				return (PortId);
			else
				return (Undefined);
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
