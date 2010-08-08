//==================================================================================================
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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Settings
{
	[Serializable]
	public class AutoWorkspaceSettings : MKY.Utilities.Settings.Settings, IEquatable<AutoWorkspaceSettings>
	{
		private string filePath;
		private Guid filePathUser;

		public AutoWorkspaceSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public AutoWorkspaceSettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			this.filePath     = rhs.FilePath;
			this.filePathUser = rhs.FilePathUser;

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
			if (obj == null)
				return (false);

			AutoWorkspaceSettings casted = obj as AutoWorkspaceSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(AutoWorkspaceSettings casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.

				(this.filePath     == casted.filePath) &&
				(this.filePathUser == casted.filePathUser)
			);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AutoWorkspaceSettings lhs, AutoWorkspaceSettings rhs)
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
		public static bool operator !=(AutoWorkspaceSettings lhs, AutoWorkspaceSettings rhs)
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
