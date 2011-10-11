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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY.IO;

namespace YAT.Settings
{
	/// <summary></summary>
	[Serializable]
	public class AutoWorkspaceSettings : MKY.Settings.SettingsItem
	{
		private string filePath;
		private Guid filePathUser;

		/// <summary></summary>
		public AutoWorkspaceSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public AutoWorkspaceSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public AutoWorkspaceSettings(AutoWorkspaceSettings rhs)
			: base(rhs)
		{
			FilePath     = rhs.FilePath;
			FilePathUser = rhs.FilePathUser;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			FilePath     = "";
			FilePathUser = Guid.Empty;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("FilePath")]
		public virtual string FilePath
		{
			get { return (this.filePath); }
			set
			{
				if (value != this.filePath)
				{
					this.filePath = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("FilePathUser")]
		public virtual Guid FilePathUser
		{
			get { return (this.filePathUser); }
			set
			{
				if (value != this.filePathUser)
				{
					this.filePathUser = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		public virtual void SetFilePathAndUser(string filePath, Guid filePathUser)
		{
			FilePath = filePath;
			FilePathUser = filePathUser;
		}

		/// <summary></summary>
		public virtual void ResetFilePathAndUser()
		{
			FilePath = "";
			FilePathUser = Guid.Empty;
		}

		/// <summary></summary>
		public virtual void ResetUserOnly()
		{
			FilePathUser = Guid.Empty;
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

			AutoWorkspaceSettings other = (AutoWorkspaceSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(this.filePath, other.filePath) &&
				(this.filePathUser == other.filePathUser)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.filePath    .GetHashCode() ^
				this.filePathUser.GetHashCode()
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
