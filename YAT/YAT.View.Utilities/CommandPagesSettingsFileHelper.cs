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
	/// <remarks>
	/// Duplicate implementation as "Clipboard" and "File" to deal with subtle differences rather
	/// than making the implementation less comprehensive. Separate classes to ease diffing.
	/// </remarks>
	public static class CommandPagesSettingsFileHelper
	{
		private enum Mode
		{
			Cancel,
			Neutral,
			Enlarge,
			Spread
		}

		/// <summary></summary>
		public static bool TryExport(IWin32Window owner, PredefinedCommandSettings commandPages, int selectedPageId, string indicatedName)
		{
			var pageCount = commandPages.Pages.Count;
			if (pageCount > 1)
			{
				var message = new StringBuilder();
				message.AppendLine("Would you like to export all " + pageCount.ToString(CultureInfo.CurrentUICulture) + " pages [Yes],");
				message.Append("or just the currently selected page " + selectedPageId.ToString(CultureInfo.CurrentUICulture) + " [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Export Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: return (TryExportAllPages(    owner, commandPages,                 indicatedName));
					case DialogResult.No:  return (TryExportSelectedPage(owner, commandPages, selectedPageId, indicatedName));
					default:               return (false);
				}
			}
			else // Just a single page => export all without asking:
			{                           // Specifying 'false' will export all pages (not the selected).
				return (TryExport(owner, commandPages, false, indicatedName));
			}
		}

		/// <summary>
		/// Prompts the user to export all pages to a .yapcs file.
		/// </summary>
		public static bool TryExportAllPages(IWin32Window owner, PredefinedCommandSettings commandPages, string indicatedName)
		{                           // Specifying 'false' will export all pages (not the selected).
			return (TryExport(owner, commandPages, false, indicatedName));
		}

		/// <summary>
		/// Prompts the user to export the given page to a .yapc file.
		/// </summary>
		public static bool TryExportSelectedPage(IWin32Window owner, PredefinedCommandSettings commandPages, int selectedPageId, string indicatedName)
		{
			var p = new PredefinedCommandSettings(commandPages); // Clone page to get same properties.
			p.Pages.Clear();
			p.Pages.Add(new PredefinedCommandPage(commandPages.Pages[selectedPageId - 1])); // Clone page to ensure decoupling.
			             // Specifying 'true' will export a single page (not all pages).
			return (TryExport(owner, p, true, indicatedName));
		}

		/// <summary></summary>
		private static bool TryExport(IWin32Window owner, PredefinedCommandSettings commandPages, bool isSelected, string indicatedName)
		{
			var sfd = new SaveFileDialog();
			if ((commandPages.Pages.Count == 1) && isSelected)
			{
				sfd.Title       = "Save Command Page As";
				sfd.Filter      = ExtensionHelper.CommandPageFilesFilter;
				sfd.FilterIndex = ExtensionHelper.CommandPageFilesFilterDefault;
				sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageFile);
			}
			else
			{
				sfd.Title       = "Save Command Pages As";
				sfd.Filter      = ExtensionHelper.CommandPageOrPagesFilesFilter;
				sfd.FilterIndex = ExtensionHelper.CommandPageOrPagesFilesFilterDefault;
				sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPagesFile);
			}
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
				if (TrySave(commandPages, sfd.FileName, out ex))
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
		private static bool TrySave(PredefinedCommandSettings commandPages, string filePath, out Exception exception)
		{
			try
			{
				if ((commandPages.Pages.Count == 1) && ExtensionHelper.IsCommandPageFile(filePath))
				{
					var root = new CommandPageSettingsRoot();
					root.Page = commandPages.Pages[0];

					var sh = new DocumentSettingsHandler<CommandPageSettingsRoot>(root);
					sh.SettingsFilePath = filePath;
					sh.Save();
				}
				else
				{
					var root = new CommandPagesSettingsRoot();
					root.PredefinedCommand = commandPages;

					var sh = new DocumentSettingsHandler<CommandPagesSettingsRoot>(root);
					sh.SettingsFilePath = filePath;
					sh.Save();
				}

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
		public static bool ShowFileOpenDialogAndTryLoad(IWin32Window owner, out PredefinedCommandSettings commandPages)
		{
			var ofd = new OpenFileDialog();
			ofd.Title       = "Open Command Page(s)";
			ofd.Filter      = ExtensionHelper.CommandPageOrPagesFilesFilter;
			ofd.FilterIndex = ExtensionHelper.CommandPageOrPagesFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageFile);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;

			var dr = ofd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Exception ex;
				if (TryLoad(ofd.FileName, out commandPages, out ex))
				{
					if (commandPages.Pages.Count >= 1)
					{
						return (true);
					}
					else
					{
						if (ExtensionHelper.IsCommandPageFile(ofd.FileName))
						{
							MessageBoxEx.Show
							(
								"File contains no page.",
								"No Page",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning
							);
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
		private static bool TryLoad(string filePath, out PredefinedCommandSettings commandPages, out Exception exception)
		{
			try
			{
				if (ExtensionHelper.IsCommandPageFile(filePath))
				{
					var sh = new DocumentSettingsHandler<CommandPageSettingsRoot>();
					sh.SettingsFilePath = filePath;
					if (sh.Load())
					{
						commandPages = new PredefinedCommandSettings();
						commandPages.Pages.Add(sh.Settings.Page); // No clone needed as just imported.
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
				else
				{
					var sh = new DocumentSettingsHandler<CommandPagesSettingsRoot>();
					sh.SettingsFilePath = filePath;
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
			}
			catch (Exception ex)
			{
				commandPages = null;
				exception = ex;
				return (false);
			}
		}

		/// <summary></summary>
		public static bool TryLoadAndImport(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (ShowFileOpenDialogAndTryLoad(owner, out imported))
			{
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
						TryImport(owner, commandPagesOld.PageLayout, imported, out commandPagesNew);
						return (true);
					}

					case DialogResult.No:
					{                                                                     // Specifying 'NoPageId' will add (not insert).
						TryAddOrInsert(owner, commandPagesOld, imported, PredefinedCommandPageCollection.NoPageId, out commandPagesNew);
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
		public static bool TryLoadAndInsert(IWin32Window owner, PredefinedCommandSettings commandPagesOld, int selectedPageId, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (ShowFileOpenDialogAndTryLoad(owner, out imported))
			{
				                                        // Specifying a 'selectedPageId' will insert (instead of add).
				return (TryAddOrInsert(owner, commandPagesOld, imported, selectedPageId, out commandPagesNew));
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryLoadAndAdd(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (ShowFileOpenDialogAndTryLoad(owner, out imported))
			{
				                                                                          // Specifying 'NoPageId' will add (not insert).
				return (TryAddOrInsert(owner, commandPagesOld, imported, PredefinedCommandPageCollection.NoPageId, out commandPagesNew));
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool TryImport(IWin32Window owner, PredefinedCommandPageLayout pageLayoutOld, PredefinedCommandSettings imported, out PredefinedCommandSettings commandPagesNew)
		{
			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmImport(owner, imported, pageLayoutOld, out mode, out pageLayoutNew))
			{
				commandPagesNew = imported;
				return (true);
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool TryAddOrInsert(IWin32Window owner, PredefinedCommandSettings commandPagesOld, PredefinedCommandSettings imported, int selectedPageId, out PredefinedCommandSettings commandPagesNew)
		{
			// Attention:
			// Similar code exists in Change() further below.
			// Changes here may have to be applied there too.

			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmImport(owner, imported, commandPagesOld.PageLayout, out mode, out pageLayoutNew))
			{
				// Clone...
				commandPagesNew = new PredefinedCommandSettings(commandPagesOld);

				// ...potentially adjust layout...
				commandPagesNew.PageLayout = pageLayoutNew;

				// ...add default page if yet empty...
				if (commandPagesOld.Pages.Count == 0)
					commandPagesNew.Pages.Add(PredefinedCommandPageCollection.DefaultPage);

				switch (mode)
				{
					case (Mode.Neutral):
					{
						// ...then add or insert:
						if (selectedPageId == PredefinedCommandPageCollection.NoPageId)
							commandPagesNew.Pages.AddRange(imported.Pages); // No clone needed as just imported.
						else
							commandPagesNew.Pages.InsertRange((selectedPageId - 1), imported.Pages); // No clone needed as just imported.

						return (true);
					}

					case (Mode.Enlarge):
					{
						// ... then add or insert:
						if (selectedPageId == PredefinedCommandPageCollection.NoPageId)
							commandPagesNew.Pages.AddRange(imported.Pages); // No clone needed as just imported.
						else
							commandPagesNew.Pages.InsertRange((selectedPageId - 1), imported.Pages); // No clone needed as just imported.

						return (true);
					}

					case (Mode.Spread):
					{
							var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutNew).CommandCapacityPerPage;

						// ...and then spread:
						if (selectedPageId == PredefinedCommandPageCollection.NoPageId)
							commandPagesNew.Pages.AddSpreaded(imported.Pages, commandCapacityPerPageNew); // No clone needed as just imported.
						else
							commandPagesNew.Pages.InsertSpreaded((selectedPageId - 1), imported.Pages, commandCapacityPerPageNew); // No clone needed as just imported.

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
		private static bool ConfirmImport(IWin32Window owner, PredefinedCommandSettings imported, PredefinedCommandPageLayout pageLayoutOld, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			// Attention:
			// Similar code exists in ConfirmChange() below.
			// Changes here may have to be applied there too.

			var commandCapacityPerPageOld = ((PredefinedCommandPageLayoutEx)pageLayoutOld).CommandCapacityPerPage;
			if (imported.Pages.MaxCommandCountPerPage <= commandCapacityPerPageOld)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutOld;
				return (true);
			}
			else
			{
				var nextPageLayout = PredefinedCommandPageLayoutEx.GetMatchingItem(imported.Pages.MaxCommandCountPerPage);
				var nextCommandCapacityPerPage = nextPageLayout.CommandCapacityPerPage;

				var message = new StringBuilder();
				message.Append("The imported file contains ");
				message.Append(imported.Pages.Count == 1 ? " page" : " pages");
				message.Append(" with up to ");
				message.Append(imported.Pages.MaxCommandCountPerPage);
				message.Append(" commands per page, but currently ");
				message.Append(commandCapacityPerPageOld);
				message.AppendLine(" commands per page are configured.");
				message.AppendLine();
				message.Append("Would you like to enlarge all pages to " + nextCommandCapacityPerPage.ToString(CultureInfo.CurrentUICulture) + " commands per page [Yes],");
				message.Append(" or spread the imported commands to " + commandCapacityPerPageOld.ToString(CultureInfo.CurrentUICulture) + " commands per page [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Import Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: mode = Mode.Enlarge; pageLayoutNew = nextPageLayout; return (true);
					case DialogResult.No:  mode = Mode.Spread;  pageLayoutNew = pageLayoutOld;  return (true);
					default:               mode = Mode.Cancel;  pageLayoutNew = pageLayoutOld;  return (false);
				}
			}
		}

		/// <summary></summary>
		private static bool ConfirmChange(IWin32Window owner, PredefinedCommandSettings commandPagesOld, PredefinedCommandPageLayout pageLayoutRequested, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			// Attention:
			// Similar code exists in ConfirmImport() above.
			// Changes here may have to be applied there too.

			var pageLayoutOld = commandPagesOld.PageLayout;

			var commandCapacityPerPageOld       = ((PredefinedCommandPageLayoutEx)pageLayoutOld)      .CommandCapacityPerPage;
			var commandCapacityPerPageRequested = ((PredefinedCommandPageLayoutEx)pageLayoutRequested).CommandCapacityPerPage;
			if (commandPagesOld.Pages.MaxCommandCountPerPage <= commandCapacityPerPageRequested)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutRequested;
				return (true);
			}
			else
			{
				var message = new StringBuilder();
				message.Append("The currently configured predefined commands contain up to ");
				message.Append(commandPagesOld.Pages.MaxCommandCountPerPage);
				message.Append(" commands per page, but only ");
				message.Append(commandCapacityPerPageRequested);
				message.AppendLine(" commands per page are requested now.");
				message.AppendLine();
				message.Append("Would you like to enlarge all pages to " + commandCapacityPerPageRequested.ToString(CultureInfo.CurrentUICulture) + " commands per page [Yes],");
				message.Append(" or spread the pages to " + commandCapacityPerPageOld.ToString(CultureInfo.CurrentUICulture) + " commands per page [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Change Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: mode = Mode.Enlarge; pageLayoutNew = pageLayoutRequested; return (true);
					case DialogResult.No:  mode = Mode.Spread;  pageLayoutNew = pageLayoutOld;       return (true);
					default:               mode = Mode.Cancel;  pageLayoutNew = pageLayoutOld;       return (false);
				}
			}
		}

		/// <summary></summary>
		public static bool TryChange(IWin32Window owner, PredefinedCommandSettings commandPagesOld, PredefinedCommandPageLayout pageLayoutRequested, out PredefinedCommandSettings commandPagesNew)
		{
			// Attention:
			// Similar code exists in AddOrInsert() further above.
			// Changes here may have to be applied there too.

			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmChange(owner, commandPagesOld, pageLayoutRequested, out mode, out pageLayoutNew))
			{
				// Create...
				commandPagesNew = new PredefinedCommandSettings();

				// ...set layout...
				commandPagesNew.PageLayout = pageLayoutNew;

				// ...add default page if yet empty...
				if (commandPagesOld.Pages.Count == 0)
					commandPagesNew.Pages.Add(PredefinedCommandPageCollection.DefaultPage);

				switch (mode)
				{
					case (Mode.Neutral):
					case (Mode.Enlarge):
					{
						commandPagesNew.Pages.AddRange(new PredefinedCommandPageCollection(commandPagesOld.Pages)); // Clone pages to ensure decoupling.
						return (true);
					}

					case (Mode.Spread):
					{
						var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutRequested).CommandCapacityPerPage;
						commandPagesNew.Pages.AddSpreaded(new PredefinedCommandPageCollection(commandPagesOld.Pages), commandCapacityPerPageNew); // Clone pages to ensure decoupling.
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
