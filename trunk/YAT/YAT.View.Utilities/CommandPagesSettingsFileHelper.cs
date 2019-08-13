﻿//==================================================================================================
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
	/// Note there are similar implementations "Clipboard", "File" and "FileLink" to deal with
	/// subtle differences. Intentionally kept in parallel rather than making the implementation
	/// all-inclusive but less comprehensive. Separate classes to ease diffing.
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
		public static bool TryExport(IWin32Window owner, PredefinedCommandSettings settings, int selectedPageId, string indicatedName)
		{
			var pageCount = settings.Pages.Count;
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
					case DialogResult.Yes: return (TryExportAll(owner, settings,                 indicatedName));
					case DialogResult.No:  return (TryExportOne(owner, settings, selectedPageId, indicatedName));

					default:               return (false);
				}
			}
			else // If (pageCount <= 1) export without asking, the chosen file extension will select the format:
			{
				return (TryExport(owner, settings.Pages, indicatedName));
			}
		}

		/// <summary>
		/// Prompts the user to export all pages to a .yapcs file.
		/// </summary>
		public static bool TryExportAll(IWin32Window owner, PredefinedCommandSettings settings, string indicatedName)
		{
			return (TryExport(owner, settings.Pages, indicatedName));
		}

		/// <summary>
		/// Prompts the user to export the given page to a .yapc file.
		/// </summary>
		public static bool TryExportOne(IWin32Window owner, PredefinedCommandSettings settings, int pageId, string indicatedName)
		{
			var pages = new PredefinedCommandPageCollection();
			pages.Add(new PredefinedCommandPage(settings.Pages[pageId - 1])); // Clone to ensure decoupling.

			return (TryExport(owner, pages, indicatedName));
		}

		/// <summary></summary>
		private static bool TryExport(IWin32Window owner, PredefinedCommandPageCollection pages, string indicatedName)
		{
			var sfd = new SaveFileDialog();
			if (pages.Count == 1)
			{
				sfd.Title       = "Save Command Page As";
				sfd.Filter      = ExtensionHelper.CommandPageOrPagesFilesFilter;
				sfd.FilterIndex = ExtensionHelper.CommandPageOrPagesFilesFilterDefault;
				sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageFile);
			}
			else
			{
				sfd.Title       = "Save Command Pages As";
				sfd.Filter      = ExtensionHelper.CommandPagesFilesFilter;
				sfd.FilterIndex = ExtensionHelper.CommandPagesFilesFilterDefault;
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
				if ((pages.Count == 1) && ExtensionHelper.IsCommandPageFile(sfd.FileName))
				{
					if (TrySave(pages[0], sfd.FileName, out ex))
						return (true);
				}
				else
				{
					if (TrySave(pages, sfd.FileName, out ex))
						return (true);
				}

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
		private static bool TrySave(PredefinedCommandPage page, string filePath, out Exception exception)
		{
			try
			{
				var root = new CommandPageSettingsRoot();
				root.Page = page; // No clone needed as just temporary.

				var sh = new DocumentSettingsHandler<CommandPageSettingsRoot>(root);
				sh.SettingsFilePath = filePath;
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
		private static bool TrySave(PredefinedCommandPageCollection pages, string filePath, out Exception exception)
		{
			try
			{
				var root = new CommandPagesSettingsRoot();
				root.Pages = pages; // No clone needed as just temporary.

				var sh = new DocumentSettingsHandler<CommandPagesSettingsRoot>(root);
				sh.SettingsFilePath = filePath;
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
		private static bool TryLoad(string filePath, out PredefinedCommandPage page, out Exception exception)
		{
			try
			{
				var sh = new DocumentSettingsHandler<CommandPageSettingsRoot>();
				sh.SettingsFilePath = filePath;
				if (sh.Load())
				{
					page = sh.Settings.Page; // No clone needed as just loaded.
					exception = null;
					return (true);
				}
				else
				{
					page = null;
					exception = null;
					return (true);
				}
			}
			catch (Exception ex)
			{
				page = null;
				exception = ex;
				return (false);
			}
		}

		/// <summary></summary>
		private static bool TryLoad(string filePath, out PredefinedCommandPageCollection pages, out Exception exception)
		{
			try
			{
				var sh = new DocumentSettingsHandler<CommandPagesSettingsRoot>();
				sh.SettingsFilePath = filePath;
				if (sh.Load())
				{
					pages = sh.Settings.Pages; // No clone needed as just loaded.
					exception = null;
					return (true);
				}
				else
				{
					pages = null;
					exception = null;
					return (true);
				}
			}
			catch (Exception ex)
			{
				pages = null;
				exception = ex;
				return (false);
			}
		}

		/// <summary></summary>
		private static bool TryLoad(string filePath, out PredefinedCommandSettings settings, out Exception exception)
		{
			try
			{
				var sh = new DocumentSettingsHandler<TerminalSettingsRoot>();
				sh.SettingsFilePath = filePath;
				if (sh.Load())
				{
					settings = sh.Settings.PredefinedCommand; // No clone needed as just loaded.
					exception = null;
					return (true);
				}
				else
				{
					settings = null;
					exception = null;
					return (true);
				}
			}
			catch (Exception ex)
			{
				settings = null;
				exception = ex;
				return (false);
			}
		}

		/// <summary></summary>
		public static bool ShowOpenFileDialogAndTryLoad(IWin32Window owner, out PredefinedCommandPageCollection pages)
		{
			var ofd = new OpenFileDialog();
			ofd.Title       = "Open Command Page(s)";
			ofd.Filter      = ExtensionHelper.CommandPageOrPagesOrTerminalFilesFilter;
			ofd.FilterIndex = ExtensionHelper.CommandPageOrPagesOrTerminalFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageFile);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;

			var dr = ofd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Exception ex;
				if (ExtensionHelper.IsTerminalFile(ofd.FileName))
				{
					PredefinedCommandSettings settings;
					if (TryLoad(ofd.FileName, out settings, out ex))
					{
						if (settings.Pages.Count < 1)
						{
							if (MessageBoxEx.Show
								(
								"File contains no pages.",
								"No Pages",
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Warning
								) == DialogResult.Cancel)
							{
								pages = null;
								return (false);
							}
						}

						pages = new PredefinedCommandPageCollection();
						pages.AddRange(settings.Pages); // No clone needed as just loaded.
						return (true);
					}
				}
				else if (ExtensionHelper.IsCommandPagesFile(ofd.FileName))
				{
					if (TryLoad(ofd.FileName, out pages, out ex))
					{
						if (pages.Count < 1)
						{
							if (MessageBoxEx.Show
								(
								"File contains no pages.",
								"No Pages",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning
								) == DialogResult.Cancel)
							{
								pages = null;
								return (false);
							}
						}

						return (true);
					}
				}
				else // ExtensionHelper.IsCommandPageFile(ofd.FileName) and .txt or .xml or whatever
				{
					PredefinedCommandPage page;
					if (TryLoad(ofd.FileName, out page, out ex))
					{
						pages = new PredefinedCommandPageCollection();
						pages.Add(page); // No clone needed as just loaded.
						return (true);
					}
				}

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

			pages = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryLoadAndImport(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (ShowOpenFileDialogAndTryLoad(owner, out pagesImported))
			{
				var message = new StringBuilder();
				message.Append("File contains ");
				message.Append(pagesImported.Count);
				message.Append(pagesImported.Count == 1 ? " page" : " pages");
				message.Append(" with a total of ");
				message.Append(pagesImported.TotalDefinedCommandCount);
				message.AppendLine(" commands.");
				message.AppendLine();
				message.Append("Would you like to replace all currently configured predefined commands by the imported");
				message.Append(pagesImported.Count == 1 ? " page" : " pages");
				message.AppendLine(" [Yes],");
				message.Append("or add the imported");
				message.Append(pagesImported.Count == 1 ? " page" : " pages");
				message.Append(" to the currently configured predefined commands [No]?");

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
						return (TryReplace(owner, settingsOld, pagesImported, out settingsNew));
					}

					case DialogResult.No:
					{                                                                              // Specifying 'NoPageId' will add (not insert).
						return (TryAddOrInsert(owner, settingsOld, pagesImported, PredefinedCommandPageCollection.NoPageId, out settingsNew));
					}

					default:
					{
						break; // Nothing to do.
					}
				}
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryLoadAndInsert(IWin32Window owner, PredefinedCommandSettings settingsOld, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (ShowOpenFileDialogAndTryLoad(owner, out pagesImported))
			{
				                                           // Specifying 'selectedPageId' will insert (instead of add).
				return (TryAddOrInsert(owner, settingsOld, pagesImported, selectedPageId, out settingsNew));
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryLoadAndAdd(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (ShowOpenFileDialogAndTryLoad(owner, out pagesImported))
			{
				                                                                           // Specifying 'NoPageId' will add (not insert).
				return (TryAddOrInsert(owner, settingsOld, pagesImported, PredefinedCommandPageCollection.NoPageId, out settingsNew));
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool TryReplace(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageCollection pagesImported, out PredefinedCommandSettings settingsNew)
		{
			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmImport(owner, pagesImported, settingsOld.PageLayout, out mode, out pageLayoutNew))
			{
				// Clone...
				settingsNew = new PredefinedCommandSettings(settingsOld); // Clone settings to preserve properties.

				// ...and replace the pages:
				settingsNew.Pages.Clear();
				settingsNew.Pages = pagesImported;
				return (true);
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool TryAddOrInsert(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageCollection pagesImported, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			// Attention:
			// Similar code exists in Change() further below.
			// Changes here may have to be applied there too.

			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmImport(owner, pagesImported, settingsOld.PageLayout, out mode, out pageLayoutNew))
			{
				// Clone...
				settingsNew = new PredefinedCommandSettings(settingsOld); // Clone settings to preserve pages and other properties.

				// ...potentially adjust layout...
				settingsNew.PageLayout = pageLayoutNew;

				// ...and then...
				if (pagesImported.Count > 0)
				{
					switch (mode)
					{
						case (Mode.Neutral):
						{
							// ...add or insert:
							if (selectedPageId == PredefinedCommandPageCollection.NoPageId)
								settingsNew.Pages.AddRange(pagesImported); // No clone needed as just loaded.
							else
								settingsNew.Pages.InsertRange((selectedPageId - 1), pagesImported); // No clone needed as just loaded.

							return (true);
						}

						case (Mode.Enlarge):
						{
							// ...add or insert:
							if (selectedPageId == PredefinedCommandPageCollection.NoPageId)
								settingsNew.Pages.AddRange(pagesImported); // No clone needed as just loaded.
							else
								settingsNew.Pages.InsertRange((selectedPageId - 1), pagesImported); // No clone needed as just loaded.

							return (true);
						}

						case (Mode.Spread):
						{
								var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutNew).CommandCapacityPerPage;

							// ...spread:
							if (selectedPageId == PredefinedCommandPageCollection.NoPageId)
								settingsNew.Pages.AddSpreaded(pagesImported, commandCapacityPerPageNew); // No clone needed as just loaded.
							else
								settingsNew.Pages.InsertSpreaded((selectedPageId - 1), pagesImported, commandCapacityPerPageNew); // No clone needed as just loaded.

							return (true);
						}

						default:
						{
							break; // Nothing to do.
						}
					}
				}
				else // ...add default page since empty:
				{
					settingsNew.Pages.Add(PredefinedCommandPageCollection.DefaultPage);
				}
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool ConfirmImport(IWin32Window owner, PredefinedCommandPageCollection pagesImported, PredefinedCommandPageLayout pageLayoutOld, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			// Attention:
			// Similar code exists in ConfirmChange() below.
			// Changes here may have to be applied there too.

			var commandCapacityPerPageOld = ((PredefinedCommandPageLayoutEx)pageLayoutOld).CommandCapacityPerPage;
			if (pagesImported.MaxCommandCountPerPage <= commandCapacityPerPageOld)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutOld;
				return (true);
			}
			else
			{
				var nextPageLayout = PredefinedCommandPageLayoutEx.GetMatchingItem(pagesImported.MaxCommandCountPerPage);
				var nextCommandCapacityPerPage = nextPageLayout.CommandCapacityPerPage;

				var message = new StringBuilder();
				message.Append("The imported file contains ");
				message.Append(pagesImported.Count == 1 ? " page" : " pages");
				message.Append(" with up to ");
				message.Append(pagesImported.MaxCommandCountPerPage);
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
		private static bool ConfirmChange(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageLayout pageLayoutRequested, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			// Attention:
			// Similar code exists in ConfirmImport() above.
			// Changes here may have to be applied there too.

			var pageLayoutOld = settingsOld.PageLayout;

			var commandCapacityPerPageOld       = ((PredefinedCommandPageLayoutEx)pageLayoutOld)      .CommandCapacityPerPage;
			var commandCapacityPerPageRequested = ((PredefinedCommandPageLayoutEx)pageLayoutRequested).CommandCapacityPerPage;
			if (settingsOld.Pages.MaxCommandCountPerPage <= commandCapacityPerPageRequested)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutRequested;
				return (true);
			}
			else
			{
				var message = new StringBuilder();
				message.Append("The currently configured predefined commands contain up to ");
				message.Append(settingsOld.Pages.MaxCommandCountPerPage);
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
		public static bool TryChange(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageLayout pageLayoutRequested, out PredefinedCommandSettings settingsNew)
		{
			// Attention:
			// Similar code exists in AddOrInsert() further above.
			// Changes here may have to be applied there too.

			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmChange(owner, settingsOld, pageLayoutRequested, out mode, out pageLayoutNew))
			{
				// Create...
				settingsNew = new PredefinedCommandSettings();

				// ...set layout...
				settingsNew.PageLayout = pageLayoutNew;

				// ...and then...
				if (settingsOld.Pages.Count > 0)
				{
					switch (mode)
					{
						case (Mode.Neutral):
						case (Mode.Enlarge):
						{
							// ...add:
							settingsNew.Pages.AddRange(new PredefinedCommandPageCollection(settingsOld.Pages)); // Clone to ensure decoupling.
							return (true);
						}

						case (Mode.Spread):
						{
							// ...spread:
							var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutRequested).CommandCapacityPerPage;
							settingsNew.Pages.AddSpreaded(new PredefinedCommandPageCollection(settingsOld.Pages), commandCapacityPerPageNew); // Clone to ensure decoupling.
							return (true);
						}

						default:
						{
							break; // Nothing to do.
						}
					}
				}
				else // ...add default page since empty:
				{
					settingsNew.Pages.Add(PredefinedCommandPageCollection.DefaultPage);
				}
			}

			settingsNew = null;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================