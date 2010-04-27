//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace MKY.Test
{
	/// <summary></summary>
	public enum SettingsMode
	{
		/// <summary></summary>
		CreateDefaultSolutionFile,

		/// <summary></summary>
		LoadFromExistingFiles,
	}

	/// <summary>
	/// This static class contains the mode how test settings should be handled.
	/// Used to either create default settings or load settings from existing files.
	/// </summary>
	public static class SettingsModeProvider
	{
		static SettingsMode staticMode = SettingsMode.LoadFromExistingFiles;

		/// <summary>
		/// Gets or sets the settings mode.
		/// </summary>
		/// <value>The settings mode.</value>
		public static SettingsMode Mode
		{
			get { return (staticMode); }
			set { staticMode = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
