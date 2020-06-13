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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
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
	/// Note there are similar implementations "Change", "Clipboard", "File" and "FileLink" to deal
	/// with subtle differences in behavior. Intentionally kept in parallel rather than making the
	/// implementation all-inclusive but less comprehensive. Separate classes to ease diffing.
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

		/// <summary>
		/// Exports to a .yacp or .yacps file, prompting the user as required.
		/// </summary>
		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'yacp' and 'yacps' are YAT specific file extension.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryExport(IWin32Window owner, PredefinedCommandSettings settings, int selectedPageId, string indicatedTerminalName)
		{
			var pageCount = settings.Pages.Count;
			if (pageCount == 0) // Export without asking, the chosen file extension will select the format:
			{
				return (TryExport(owner, settings.Pages, indicatedTerminalName));
			}
			else if (pageCount == 1) // Export without asking, the chosen file extension will select the format:
			{
				return (TryExport(owner, settings.Pages, settings.Pages[0].Name));
			}
			else // if (pageCount > 1)
			{
				var message = new StringBuilder();
				message.Append("Would you like to export all " + pageCount.ToString(CultureInfo.CurrentCulture) + " pages [Yes],");
				message.Append(" or just the currently selected page " + selectedPageId.ToString(CultureInfo.CurrentCulture) + " [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Export Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: return (TryExportAll(owner, settings, indicatedTerminalName));
					case DialogResult.No:  return (TryExportOne(owner, settings, selectedPageId));

					default:               return (false);
				}
			}
		}

		/// <summary>
		/// Prompts the user to export all pages to a .yacps file.
		/// </summary>
		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'yacps' is a YAT specific file extension.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryExportAll(IWin32Window owner, PredefinedCommandSettings settings, string indicatedTerminalName)
		{
			return (TryExport(owner, settings.Pages, indicatedTerminalName));
		}

		/// <summary>
		/// Prompts the user to export the given page to a .yacp file.
		/// </summary>
		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'yacp' is a YAT specific file extension.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryExportOne(IWin32Window owner, PredefinedCommandSettings settings, int pageId)
		{
			var pages = new PredefinedCommandPageCollection();
			pages.Add(new PredefinedCommandPage(settings.Pages[pageId - 1])); // Clone to ensure decoupling.

			return (TryExport(owner, pages, pages[0].Name));
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool TryExport(IWin32Window owner, PredefinedCommandPageCollection pages, string initialName)
		{
			var sfd = new SaveFileDialog();
			if (pages.Count == 1)
			{
				sfd.Title       = "Save Command Page As";
				sfd.Filter      = ExtensionHelper.CommandPageOrPagesFilesFilter;
				sfd.FilterIndex = ExtensionHelper.CommandPageOrPagesFilesFilterDefault;
				sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageExtension);
			}
			else
			{
				sfd.Title       = "Save Command Pages As";
				sfd.Filter      = ExtensionHelper.CommandPagesFilesFilter;
				sfd.FilterIndex = ExtensionHelper.CommandPagesFilesFilterDefault;
				sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPagesExtension);
			}
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;

			// Potentially remove .yat from the initial name:
			if (Path.HasExtension(initialName))
				initialName = Path.GetFileNameWithoutExtension(initialName);

			sfd.FileName = initialName + PathEx.NormalizeExtension(sfd.DefaultExt); // Note that 'DefaultExt' states "the returned string does not include the period".

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
					owner,
					errorMessage,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}

			return (false);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
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

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool ShowOpenFileDialogAndTryLoad(IWin32Window owner, out PredefinedCommandPageCollection pages)
		{
			var ofd = new OpenFileDialog();
			ofd.Title       = "Open Command Page(s)";
			ofd.Filter      = ExtensionHelper.CommandPageOrPagesOrTerminalFilesFilter;
			ofd.FilterIndex = ExtensionHelper.CommandPageOrPagesOrTerminalFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageExtension);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;

			var dr = ofd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Exception ex;
				if (ExtensionHelper.IsCommandPagesFile(ofd.FileName)) // .yacps explicitly.
				{
					if (TryLoad(ofd.FileName, out pages, out ex))
					{
						if (pages.Count < 1)
						{
							MessageBoxEx.Show
							(
								owner,
								"File contains no command pages.",
								"No Pages",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation
							);

							pages = null;
							return (false);
						}

						if (pages.TotalDefinedCommandCount < 1)
						{
							MessageBoxEx.Show
							(
								owner,
								((pages.Count == 1) ? "Page contains" : "Pages contain") + " no commands.",
								"No Commands",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation
							);

							pages = null;
							return (false);
						}

						return (true);
					}
				}
				else if (ExtensionHelper.IsTerminalFile(ofd.FileName)) // .yat integrated.
				{
					PredefinedCommandSettings settings;
					if (TryLoad(ofd.FileName, out settings, out ex))
					{
						if (settings.Pages.Count < 1)
						{
							MessageBoxEx.Show
							(
								owner,
								"File contains no command pages.",
								"No Pages",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation
							);

							pages = null;
							return (false);
						}

						if (settings.Pages.TotalDefinedCommandCount < 1)
						{
							MessageBoxEx.Show
							(
								owner,
								((settings.Pages.Count == 1) ? "Page contains" : "Pages contain") + " no commands.",
								"No Commands",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation
							);

							pages = null;
							return (false);
						}

						pages = new PredefinedCommandPageCollection();
						pages.AddRange(settings.Pages); // No clone needed as just loaded.
						return (true);
					}
				}
				else // ExtensionHelper.IsCommandPageFile(ofd.FileName) .yacp and .xml or .txt or whatever.
				{
					PredefinedCommandPage page;
					if (TryLoad(ofd.FileName, out page, out ex))
					{
						if (page.DefinedCommandCount < 1)
						{
							MessageBoxEx.Show
							(
								owner,
								"File contains no commands.",
								"No Commands",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation
							);

							pages = null;
							return (false);
						}

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
					owner,
					errorMessage,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}

			pages = null;
			return (false);
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryLoadAndImportAll(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (ShowOpenFileDialogAndTryLoad(owner, out pagesImported))
			{
				if (settingsOld.Pages.TotalDefinedCommandCount > 0)
				{
					var message = new StringBuilder();
					message.Append("The file contains ");
					message.Append(pagesImported.Count);
					message.Append(pagesImported.Count == 1 ? " page" : " pages");
					message.Append(" with a total of ");
					message.Append(pagesImported.TotalDefinedCommandCount);
					message.AppendLine(" commands.");
					message.AppendLine();
					message.Append("Replace the current");
					message.Append(settingsOld.Pages.Count == 1 ? " page" : " pages");
					message.Append("?");

					if (MessageBoxEx.Show
						(
							owner,
							message.ToString(),
							"Confirm Import",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Question,
							MessageBoxDefaultButton.Button2
						) == DialogResult.OK)
					{
						return (TryReplace(owner, settingsOld, pagesImported, out settingsNew));
					}
				}
				else // If (settingsOld.Pages.TotalDefinedCommandCount == 0) replace without asking:
				{
					return (TryReplace(owner, settingsOld, pagesImported, out settingsNew));
				}
			}

			settingsNew = null;
			return (false);
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryLoadAndImport(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (ShowOpenFileDialogAndTryLoad(owner, out pagesImported))
			{
				if (settingsOld.Pages.TotalDefinedCommandCount > 0)
				{
					var message = new StringBuilder();
					message.Append("The file contains ");
					message.Append(pagesImported.Count);
					message.Append(pagesImported.Count == 1 ? " page" : " pages");
					message.Append(" with a total of ");
					message.Append(pagesImported.TotalDefinedCommandCount);
					message.AppendLine(" commands.");
					message.AppendLine();
					message.Append("Would you like to replace the current");
					message.Append(settingsOld.Pages.Count == 1 ? " page" : " pages");
					message.Append(" by the");
					message.Append(pagesImported.Count == 1 ? " page" : " pages");
					message.Append(" [Yes], or add the");
					message.Append(pagesImported.Count == 1 ? " page" : " pages");
					message.Append(" to the current");
					message.Append(settingsOld.Pages.Count == 1 ? " page" : " pages");
					message.Append(" [No]?");

					switch (MessageBoxEx.Show
						(
							owner,
							message.ToString(),
							"Import Mode",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question,
							MessageBoxDefaultButton.Button3
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
				else // If (settingsOld.Pages.TotalDefinedCommandCount == 0) replace without asking:
				{
					return (TryReplace(owner, settingsOld, pagesImported, out settingsNew));
				}
			}

			settingsNew = null;
			return (false);
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool TryReplace(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageCollection pagesImported, out PredefinedCommandSettings settingsNew)
		{
			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmImport(owner, settingsOld.PageLayout, pagesImported, out mode, out pageLayoutNew))
			{
				// Clone settings to preserve pages and other properties...
				settingsNew = new PredefinedCommandSettings(settingsOld);

				// ...potentially adjust layout...
				settingsNew.PageLayout = pageLayoutNew;

				// ...and then...
				switch (mode)
				{
					case (Mode.Neutral):
					case (Mode.Enlarge):
					{
						// ...replace:
						settingsNew.Pages.Clear();
						settingsNew.Pages.AddRange(pagesImported); // No clone needed as just loaded.

						return (true);
					}

					case (Mode.Spread):
					{
						var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutNew).CommandCapacityPerPage;

						// ...replace spreaded:
						settingsNew.Pages.Clear();
						settingsNew.Pages.AddSpreaded(pagesImported, commandCapacityPerPageNew); // No clone needed as just loaded.

						return (true);
					}

					case (Mode.Cancel):
					default:
					{
						break; // Do nothing.
					}
				}
			} // ConfirmImport()

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool TryAddOrInsert(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageCollection pagesImported, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmImport(owner, settingsOld.PageLayout, pagesImported, out mode, out pageLayoutNew))
			{
				// Clone settings to preserve pages and other properties...
				settingsNew = new PredefinedCommandSettings(settingsOld);

				// ...potentially adjust layout...
				settingsNew.PageLayout = pageLayoutNew;

				// ...and then...
				switch (mode)
				{
					case (Mode.Neutral):
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

					case (Mode.Cancel):
					default:
					{
						break; // Do nothing.
					}
				}
			} // ConfirmImport()

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool ConfirmImport(IWin32Window owner, PredefinedCommandPageLayout pageLayoutOld, PredefinedCommandPageCollection pagesImported, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			var commandCapacityPerPageOld = ((PredefinedCommandPageLayoutEx)pageLayoutOld).CommandCapacityPerPage;
			if (pagesImported.MaxDefinedCommandCountPerPage <= commandCapacityPerPageOld)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutOld;
				return (true);
			}
			else // The pages to import do not fit the currently configured page layout:
			{
				var nextPageLayout = PredefinedCommandPageLayoutEx.GetMatchingItem(pagesImported.MaxDefinedCommandCountPerPage);
				var nextCommandCapacityPerPage = nextPageLayout.CommandCapacityPerPage;

				var message = new StringBuilder();
				message.Append("The file contains ");
				if (pagesImported.Count <= 1)
				{
					message.Append("1 page with ");
					message.Append(pagesImported.MaxDefinedCommandCountPerPage);
					message.Append(" commands,");
				}
				else
				{
					message.Append(pagesImported.Count);
					message.Append(" pages with up to ");
					message.Append(pagesImported.MaxDefinedCommandCountPerPage);
					message.Append(" commands per page,");
				}
				message.Append(" but currently ");
				message.Append(commandCapacityPerPageOld);
				message.AppendLine(" commands per page are configured.");
				message.AppendLine();
				message.Append("Would you like to enlarge the pages to " + nextCommandCapacityPerPage.ToString(CultureInfo.CurrentCulture) + " commands per page [Yes],");
				message.Append(       " or spread the imported pages to " + commandCapacityPerPageOld.ToString(CultureInfo.CurrentCulture) + " commands per page [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Import Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question,
						MessageBoxDefaultButton.Button3
					))
				{
					case DialogResult.Yes: mode = Mode.Enlarge; pageLayoutNew = nextPageLayout; return (true);
					case DialogResult.No:  mode = Mode.Spread;  pageLayoutNew = pageLayoutOld;  return (true);
					default:               mode = Mode.Cancel;  pageLayoutNew = pageLayoutOld;  return (false);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
