//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.IO;
using System.Xml.Serialization;

using MKY.IO;

#endregion

namespace YAT.Application.Settings
{
	/// <summary></summary>
	public class PathSettings : MKY.Settings.SettingsItem, IEquatable<PathSettings>
	{
		/// <summary></summary>
		public static readonly string MainFilesDefault    = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + ApplicationEx.ProductName; // File location shall differ for "YAT" and "YATConsole".

		/// <summary></summary>
		public static readonly string CommandFilesDefault = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + ApplicationEx.ProductName; // File location shall differ for "YAT" and "YATConsole".

		/// <summary></summary>
		public static readonly string SendFilesDefault    = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + ApplicationEx.ProductName; // File location shall differ for "YAT" and "YATConsole".

		/// <summary></summary>
		public static readonly string LogFilesDefault     = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + ApplicationEx.ProductName; // File location shall differ for "YAT" and "YATConsole".

		/// <summary></summary>
		public static readonly string MonitorFilesDefault = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + ApplicationEx.ProductName; // File location shall differ for "YAT" and "YATConsole".

		private string mainFiles;
		private string commandFiles;
		private string sendFiles;
		private string logFiles;
		private string monitorFiles;

		/// <summary></summary>
		public PathSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public PathSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public PathSettings(PathSettings rhs)
			: base(rhs)
		{
			MainFiles    = rhs.MainFiles;
			CommandFiles = rhs.CommandFiles;
			SendFiles    = rhs.SendFiles;
			LogFiles     = rhs.LogFiles;
			MonitorFiles = rhs.MonitorFiles;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			MainFiles    = MainFilesDefault;
			CommandFiles = CommandFilesDefault;
			SendFiles    = SendFilesDefault;
			LogFiles     = LogFilesDefault;
			MonitorFiles = MonitorFilesDefault;
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CommandFiles")]
		public virtual string CommandFiles
		{
			get { return (this.commandFiles); }
			set
			{
				if (this.commandFiles != value)
				{
					this.commandFiles = value;
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ (MainFiles    != null ? MainFiles   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (CommandFiles != null ? CommandFiles.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SendFiles    != null ? SendFiles   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LogFiles     != null ? LogFiles    .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MonitorFiles != null ? MonitorFiles.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as PathSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(PathSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(MainFiles,    other.MainFiles)    &&
				PathEx.Equals(CommandFiles, other.CommandFiles) &&
				PathEx.Equals(SendFiles,    other.SendFiles)    &&
				PathEx.Equals(LogFiles,     other.LogFiles)     &&
				PathEx.Equals(MonitorFiles, other.MonitorFiles)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PathSettings lhs, PathSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(PathSettings lhs, PathSettings rhs)
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
