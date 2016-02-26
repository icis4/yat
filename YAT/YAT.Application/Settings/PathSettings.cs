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

using MKY.IO;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	[Serializable]
	public class PathSettings : MKY.Settings.SettingsItem
	{
		private string mainFiles;
		private string sendFiles;
		private string logFiles;
		private string monitorFiles;

		/// <summary></summary>
		public PathSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public PathSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public PathSettings(PathSettings rhs)
			: base(rhs)
		{
			MainFiles    = rhs.MainFiles;
			SendFiles    = rhs.SendFiles;
			LogFiles     = rhs.LogFiles;
			MonitorFiles = rhs.MonitorFiles;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			MainFiles    = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			SendFiles    = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			LogFiles     = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			MonitorFiles = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("MainFiles")]
		public virtual string MainFiles
		{
			get { return (this.mainFiles); }
			set
			{
				if (this.mainFiles != value)
				{
					this.mainFiles = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendFiles")]
		public virtual string SendFiles
		{
			get { return (this.sendFiles); }
			set
			{
				if (this.sendFiles != value)
				{
					this.sendFiles = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LogFiles")]
		public virtual string LogFiles
		{
			get { return (this.logFiles); }
			set
			{
				if (this.logFiles != value)
				{
					this.logFiles = value;
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

			PathSettings other = (PathSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(MainFiles,    other.MainFiles) &&
				PathEx.Equals(SendFiles,    other.SendFiles) &&
				PathEx.Equals(LogFiles,     other.LogFiles) &&
				PathEx.Equals(MonitorFiles, other.MonitorFiles)
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

				this.MainFiles   .GetHashCode() ^
				this.SendFiles   .GetHashCode() ^
				this.LogFiles    .GetHashCode() ^
				this.MonitorFiles.GetHashCode()
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
