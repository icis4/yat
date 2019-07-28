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
// YAT Version 2.0.1 Development
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
using System.Windows.Forms;

using MKY;
using MKY.IO;
using MKY.Settings;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model.Settings;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.View.Utilities
{
	/// <summary></summary>
	public static class CommandPagesSettingsHelper
	{
		/// <summary></summary>
		public static bool ShowSaveAsFileDialog(PredefinedCommandSettings commandPages, string indicatedName, IWin32Window owner)
		{
			var sfd = new SaveFileDialog();
			sfd.Title = ((commandPages.Pages.Count <= 1) ? "Save Command Page As" : "Save Command Pages As");
			sfd.Filter      = ExtensionHelper.CommandPagesFilesFilter;
			sfd.FilterIndex = ExtensionHelper.CommandPagesFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPagesFile);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;

			// Check whether the terminal has already been saved as a .yat file:
			if (StringEx.EndsWithOrdinalIgnoreCase(indicatedName, ExtensionHelper.TerminalFile))
				sfd.FileName = indicatedName;
			else
				sfd.FileName = indicatedName + PathEx.NormalizeExtension(sfd.DefaultExt); // Note that 'DefaultExt' states "the returned string does not include the period".

			var dr = sfd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Exception ex;
				if (SaveToFile(commandPages, sfd.FileName, out ex))
					return (true);

				string errorMessage;
				if (!string.IsNullOrEmpty(sfd.FileName))
					errorMessage = ErrorHelper.ComposeMessage("Unable to save", sfd.FileName, ex);
				else
					errorMessage = ErrorHelper.ComposeMessage("Unable to save file!", ex);

				MessageBoxEx.Show
				(
					errorMessage,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}

			return (false);
		}

		/// <summary></summary>
		public static bool SaveToFile(PredefinedCommandSettings commandPages, string fileName, out Exception exception)
		{
			try
			{
				var root = new CommandPagesSettingsRoot();
				root.PredefinedCommand = commandPages;

				var sh = new DocumentSettingsHandler<CommandPagesSettingsRoot>(root);
				sh.SettingsFilePath = fileName;

				sh.Save();
				exception = null;
				return (true);
			}
			catch (Exception ex)
			{
				exception = ex;
				return (false);
			}
		}


//				"The file contains {1} page{} with a total of {} command definitions"
	//			"Would you like to replace the currently configured predefined commands by the imported commands (Yes), or append the imported commands to the currently configured (No)?

	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
