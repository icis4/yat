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
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.IO;
using MKY.Settings;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.View.Utilities
{
	/// <summary></summary>
	public static class CommandPagesSettingsHelper
	{
		private enum ImportMode
		{
			None,
			Neutral,
			Enlarge,
			Spread
		}

		/// <summary></summary>
		public static bool ExportToFile(IWin32Window owner, PredefinedCommandSettings commandPages, int selectedPage, string indicatedName)
		{
			var pageCount = commandPages.Pages.Count;
			if (pageCount > 1)
			{
				var message = new StringBuilder();
				message.AppendLine("Would you like to export all " + pageCount.ToString(CultureInfo.CurrentUICulture) + " pages [Yes],");
				message.Append("or just the currently selected page " + selectedPage.ToString(CultureInfo.CurrentUICulture) + " [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Export Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: return (ExportPagesToFile(       owner, commandPages,               indicatedName));
					case DialogResult.No:  return (ExportSelectedPageToFile(owner, commandPages, selectedPage, indicatedName));
					default:               return (false);
				}
			}
			else // Just a single page => save without asking:
			{
				return (ExportPagesToFile(owner, commandPages, indicatedName));
			}
		}

		/// <summary>
		/// Prompts the user to export the given page to a file.
		/// </summary>
		public static bool ExportSelectedPageToFile(IWin32Window owner, PredefinedCommandSettings commandPages, int selectedPage, string indicatedName)
		{
			var p = new PredefinedCommandSettings(commandPages); // Clone page to get same properties.
			p.Pages.Clear();
			p.Pages.Add(new PredefinedCommandPage(commandPages.Pages[selectedPage - 1])); // Clone page to ensure decoupling.

			return (ExportPagesToFile(owner, p, indicatedName));
		}

		/// <summary>
		/// Prompts the user to export all pages to a file.
		/// </summary>
		public static bool ExportPagesToFile(IWin32Window owner, PredefinedCommandSettings commandPages, string indicatedName)
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
			}

			return (false);
		}

		/// <summary></summary>
		private static bool SaveToFile(PredefinedCommandSettings commandPages, string fileName, out Exception exception)
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

		/// <summary></summary>
		public static bool ImportAllPagesFromFile(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (ShowFileOpenDialogAndLoadFromFile(owner, out imported))
			{
				int cpMaxCommandsPerPage = PredefinedCommandSettings.MaxCommandsPerPage; // Preparation for 12/24/36/48/72 commands per page.

				var message = new StringBuilder();
				message.Append("File contains ");
				message.Append(imported.Pages.Count);
				message.Append(imported.Pages.Count == 1 ? " page" : " pages");
				message.Append(" with a total of ");
				message.Append(imported.TotalDefinedCommandCount);
				message.AppendLine(" commands.");
				message.AppendLine();
				message.AppendLine("Would you like to replace all currently configured predefined commands by the imported [Yes],");
				message.Append("or add the imported to the currently configured predefined commands [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Import Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes:
					{
						commandPagesNew = imported;
						return (true);
					}

					case DialogResult.No:
					{                                                                                                 // Specifying 'NoPage' will add (not insert).
						AddOrInsert(owner, commandPagesOld, imported, cpMaxCommandsPerPage, PredefinedCommandSettings.NoPage, out commandPagesNew);
						break;
					}

					default:
					{
						break; // Nothing to do.
					}
				}
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool ImportFromFileAndInsert(IWin32Window owner, PredefinedCommandSettings commandPagesOld, int selectedPage, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (ShowFileOpenDialogAndLoadFromFile(owner, out imported))
			{
				int cpMaxCommandsPerPage = PredefinedCommandSettings.MaxCommandsPerPage; // Preparation for 12/24/36/48/72 commands per page.
				                                                                            // Specifying a page will insert (instead of add).
				return (AddOrInsert(owner, commandPagesOld, imported, cpMaxCommandsPerPage, selectedPage, out commandPagesNew));
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool ImportFromFileAndAdd(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (ShowFileOpenDialogAndLoadFromFile(owner, out imported))
			{
				int cpMaxCommandsPerPage = PredefinedCommandSettings.MaxCommandsPerPage; // Preparation for 12/24/36/48/72 commands per page.
				                                                                                                      // Specifying 'NoPage' will add (not insert).
				return (AddOrInsert(owner, commandPagesOld, imported, cpMaxCommandsPerPage, PredefinedCommandSettings.NoPage, out commandPagesNew));
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool AddOrInsert(IWin32Window owner, PredefinedCommandSettings commandPagesOld, PredefinedCommandSettings imported, int cpMaxCommandsPerPage, int selectedPage, out PredefinedCommandSettings commandPagesNew)
		{
			ImportMode mode;
			if (ConfirmImport(owner, imported, cpMaxCommandsPerPage, out mode))
			{
				// Clone...
				commandPagesNew = new PredefinedCommandSettings(commandPagesOld);

				// ...add default page if yet empty...
				if (commandPagesOld.Pages.Count == 0)
					commandPagesNew.Pages.Add(PredefinedCommandSettings.DefaultPage);

				switch (mode)
				{
					case (ImportMode.Neutral):
					{
						// ...then add or insert:
						if (selectedPage == PredefinedCommandSettings.NoPage)
							commandPagesNew.Pages.AddRange(imported.Pages); // No clone needed as just imported.
						else
							commandPagesNew.Pages.InsertRange((selectedPage - 1), imported.Pages); // No clone needed as just imported.

						return (true);
					}
					case (ImportMode.Enlarge):
					{
						// ...enlarge...
						//commandPagesNew.MaxCommandsPerPage = cpMaxCommandsPerPage;
						// !!! PENDING !!!

						// ... then add or insert:
						if (selectedPage == PredefinedCommandSettings.NoPage)
							commandPagesNew.Pages.AddRange(imported.Pages); // No clone needed as just imported.
						else
							commandPagesNew.Pages.InsertRange((selectedPage - 1), imported.Pages); // No clone needed as just imported.

						return (true);
					}
					case (ImportMode.Spread):
					{
						// ...and then spread:
						if (selectedPage == PredefinedCommandSettings.NoPage)
							commandPagesNew.Pages.AddSpreaded(imported.Pages, PredefinedCommandSettings.MaxCommandsPerPage); // No clone needed as just imported.
						else
							commandPagesNew.Pages.InsertSpreaded((selectedPage - 1), imported.Pages, PredefinedCommandSettings.MaxCommandsPerPage); // No clone needed as just imported.

						return (true);
					}

					default:
					{
						break; // Nothing to do.
					}
				}
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool ConfirmImport(IWin32Window owner, PredefinedCommandSettings imported, int cpMaxCommandsPerPage, out ImportMode mode)
		{
			if (cpMaxCommandsPerPage <= PredefinedCommandSettings.MaxCommandsPerPage)
			{
				mode = ImportMode.Neutral;
				return (true);
			}
			else
			{
				var message = new StringBuilder();
				message.Append("File contains ");
				message.Append(imported.Pages.Count == 1 ? " page" : " pages");
				message.Append(" with up to ");
				message.Append(cpMaxCommandsPerPage);
				message.Append(" commands per page, but currently ");
				message.Append(PredefinedCommandSettings.MaxCommandsPerPage);
				message.AppendLine(" commands per page are configured.");
				message.AppendLine();
				message.Append("Would you like to enlarge all pages to " + cpMaxCommandsPerPage.ToString(CultureInfo.CurrentUICulture) + " commands per page [Yes],");
				message.Append(" or spread the imported commands to " + PredefinedCommandSettings.MaxCommandsPerPage.ToString(CultureInfo.CurrentUICulture) + " commands per page [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Import Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: mode = ImportMode.Enlarge; return (true);
					case DialogResult.No:  mode = ImportMode.Spread;  return (true);
					default:               mode = ImportMode.None;    return (false);
				}
			}
		}

		/// <summary></summary>
		public static bool ShowFileOpenDialogAndLoadFromFile(IWin32Window owner, out PredefinedCommandSettings commandPages)
		{
			var ofd = new OpenFileDialog();
			ofd.Title = "Open Command Page(s)";
			ofd.Filter      = ExtensionHelper.CommandPagesFilesFilter;
			ofd.FilterIndex = ExtensionHelper.CommandPagesFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPagesFile);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;

			var dr = ofd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Exception ex;
				if (LoadFromFile(ofd.FileName, out commandPages, out ex))
				{
					if (commandPages.Pages.Count >= 1)
					{
						return (true);
					}
					else
					{
						MessageBoxEx.Show
						(
							"File contains no pages.",
							"No Pages",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
				else
				{
					string errorMessage;
					if (!string.IsNullOrEmpty(ofd.FileName))
						errorMessage = ErrorHelper.ComposeMessage("Unable to open", ofd.FileName, ex);
					else
						errorMessage = ErrorHelper.ComposeMessage("Unable to open file!", ex);

					MessageBoxEx.Show
					(
						errorMessage,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}

			commandPages = null;
			return (false);
		}

		/// <summary></summary>
		private static bool LoadFromFile(string fileName, out PredefinedCommandSettings commandPages, out Exception exception)
		{
			try
			{
				var sh = new DocumentSettingsHandler<CommandPagesSettingsRoot>();
				sh.SettingsFilePath = fileName;
				if (sh.Load())
				{
					commandPages = sh.Settings.PredefinedCommand;
					exception = null;
					return (true);
				}
				else
				{
					commandPages = null;
					exception = null;
					return (true);
				}
			}
			catch (Exception ex)
			{
				commandPages = null;
				exception = ex;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
