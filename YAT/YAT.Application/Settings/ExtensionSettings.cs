﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	[Serializable]
	public class ExtensionSettings : MKY.Settings.SettingsItem
	{
		private string textSendFiles;
		private string binarySendFiles;
		private string rawLogFiles;
		private string neatLogFiles;
		private string monitorFiles;

		/// <summary></summary>
		public ExtensionSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public ExtensionSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public ExtensionSettings(ExtensionSettings rhs)
			: base(rhs)
		{
			TextSendFiles   = rhs.TextSendFiles;
			BinarySendFiles = rhs.BinarySendFiles;
			RawLogFiles     = rhs.RawLogFiles;
			NeatLogFiles    = rhs.NeatLogFiles;
			MonitorFiles    = rhs.MonitorFiles;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TextSendFiles   = Utilities.ExtensionHelper.TextFilesDefault;
			BinarySendFiles = Utilities.ExtensionHelper.BinaryFilesDefault;
			RawLogFiles     = Utilities.ExtensionHelper.NeatLogFilesDefault;
			NeatLogFiles    = Utilities.ExtensionHelper.RawLogFilesDefault;
			MonitorFiles    = Utilities.ExtensionHelper.MonitorFilesDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TextSendFiles")]
		public virtual string TextSendFiles
		{
			get { return (this.textSendFiles); }
			set
			{
				if (this.textSendFiles != value)
				{
					this.textSendFiles = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("BinarySendFiles")]
		public virtual string BinarySendFiles
		{
			get { return (this.binarySendFiles); }
			set
			{
				if (this.binarySendFiles != value)
				{
					this.binarySendFiles = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RawLogFiles")]
		public virtual string RawLogFiles
		{
			get { return (this.rawLogFiles); }
			set
			{
				if (this.rawLogFiles != value)
				{
					this.rawLogFiles = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NeatLogFiles")]
		public virtual string NeatLogFiles
		{
			get { return (this.neatLogFiles); }
			set
			{
				if (this.neatLogFiles != value)
				{
					this.neatLogFiles = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MonitorFiles")]
		public virtual string MonitorFiles
		{
			get { return (this.monitorFiles); }
			set
			{
				if (this.monitorFiles != value)
				{
					this.monitorFiles = value;
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

			ExtensionSettings other = (ExtensionSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(TextSendFiles   == other.TextSendFiles) &&
				(BinarySendFiles == other.BinarySendFiles) &&
				(RawLogFiles     == other.RawLogFiles) &&
				(NeatLogFiles    == other.NeatLogFiles) &&
				(MonitorFiles    == other.MonitorFiles)
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

				this.TextSendFiles  .GetHashCode() ^
				this.BinarySendFiles.GetHashCode() ^
				this.RawLogFiles    .GetHashCode() ^
				this.NeatLogFiles   .GetHashCode() ^
				this.MonitorFiles   .GetHashCode()
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
