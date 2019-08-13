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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

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
	public static class CommandPagesSettingsFileLinkHelper
	{
		private enum Mode
		{
			Cancel,
			Neutral,
			Enlarge,
			Spread
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
		public static bool ShowOpenFileDialogAndTryLoad(IWin32Window owner, out string filePath, out PredefinedCommandPageCollection pages)
		{
			var ofd = new OpenFileDialog();
			ofd.Title       = "Link Command Page(s)";
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
				if (ExtensionHelper.IsCommandPagesFile(ofd.FileName))
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
								filePath = null;
								pages = null;
								return (false);
							}
						}

						filePath = ofd.FileName;
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
						filePath = ofd.FileName;
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

			filePath = null;
			pages = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryLoadAndLink(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			// Attention:
			// Similar code exists in TryLoadAndImport() further above.
			// Changes here may have to be applied there too.

			string filePathToLink;
			PredefinedCommandPageCollection pagesToLink;
			if (ShowOpenFileDialogAndTryLoad(owner, out filePathToLink, out pagesToLink))
			{
				var message = new StringBuilder();
				message.Append("File contains ");
				message.Append(pagesToLink.Count);
				message.Append(pagesToLink.Count == 1 ? " page" : " pages");
				message.Append(" with a total of ");
				message.Append(pagesToLink.TotalDefinedCommandCount);
				message.AppendLine(" commands.");
				message.AppendLine();
				message.AppendLine("Would you like to link all configured predefined commands to the file [Yes],");
				message.Append("or link the");
				message.Append(pagesToLink.Count == 1 ? " page" : " pages");
				message.Append(" in addition to the currently configured predefined commands [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Link Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes:
					{
						return (TryLinkAll(owner, settingsOld, filePathToLink, pagesToLink, out settingsNew));
					}

					case DialogResult.No:
					{
						return (TryAddLinked(owner, settingsOld, filePathToLink, pagesToLink, out settingsNew));
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
		private static void LinkExchange(string filePathToLink, PredefinedCommandPageCollection pagesToLink)
		{
			foreach (var p in pagesToLink)
			{
				if (p.IsLinkedToFilePath)
					continue; // Nothing to do, itself linked to yet another file.

				// Set file path...
				p.LinkFilePath = filePathToLink;

				// ...and exchange commands:
				p.CommandsLinked = p.CommandsIntegrated;
				p.CommandsIntegrated = new List<Command>();
			}
		}

		/// <summary></summary>
		private static bool TryLinkAll(IWin32Window owner, PredefinedCommandSettings settingsOld, string filePathToLink, PredefinedCommandPageCollection pagesToLink, out PredefinedCommandSettings settingsNew)
		{
			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmLink(owner, pagesToLink, settingsOld.PageLayout, out mode, out pageLayoutNew))
			{
				// Prepare the pages to link...
				LinkExchange(filePathToLink, pagesToLink); // No clone needed as just loaded.

				// ...clone the settings...
				settingsNew = new PredefinedCommandSettings(settingsOld); // Clone settings to preserve properties.

				// ...and replace the pages:
				settingsNew.Pages.Clear();
				settingsNew.Pages = pagesToLink;
				return (true);
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		private static bool TryAddLinked(IWin32Window owner, PredefinedCommandSettings settingsOld, string filePathToLink, PredefinedCommandPageCollection pagesToLink, out PredefinedCommandSettings settingsNew)
		{
			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmLink(owner, pagesToLink, settingsOld.PageLayout, out mode, out pageLayoutNew))
			{
				// Prepare the pages to link...
				LinkExchange(filePathToLink, pagesToLink); // No clone needed as just loaded.

				// ...clone the settings...
				settingsNew = new PredefinedCommandSettings(settingsOld); // Clone settings to preserve pages and other properties.

				// ...potentially adjust layout...
				settingsNew.PageLayout = pageLayoutNew;

				// ...and then...
				if (pagesToLink.Count > 0)
				{
					switch (mode)
					{
						case (Mode.Neutral):
						{
							// ...add:
							settingsNew.Pages.AddRange(pagesToLink); // No clone needed as just loaded.

							return (true);
						}

						case (Mode.Enlarge):
						{
							// ...add:
							settingsNew.Pages.AddRange(pagesToLink); // No clone needed as just loaded.

							return (true);
						}

						case (Mode.Spread):
						{
								var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutNew).CommandCapacityPerPage;

							// ...spread:
							settingsNew.Pages.AddSpreaded(pagesToLink, commandCapacityPerPageNew); // No clone needed as just loaded.

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
		private static bool ConfirmLink(IWin32Window owner, PredefinedCommandPageCollection pagesToLink, PredefinedCommandPageLayout pageLayoutOld, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			var commandCapacityPerPageOld = ((PredefinedCommandPageLayoutEx)pageLayoutOld).CommandCapacityPerPage;
			if (pagesToLink.MaxCommandCountPerPage <= commandCapacityPerPageOld)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutOld;
				return (true);
			}
			else
			{
				var nextPageLayout = PredefinedCommandPageLayoutEx.GetMatchingItem(pagesToLink.MaxCommandCountPerPage);
				var nextCommandCapacityPerPage = nextPageLayout.CommandCapacityPerPage;

				var message = new StringBuilder();
				message.Append("The file contains ");
				message.Append(pagesToLink.Count == 1 ? " page" : " pages");
				message.Append(" with up to ");
				message.Append(pagesToLink.MaxCommandCountPerPage);
				message.Append(" commands per page, but currently ");
				message.Append(commandCapacityPerPageOld);
				message.AppendLine(" commands per page are configured.");
				message.AppendLine();
				message.Append("Would you like to enlarge all pages to " + nextCommandCapacityPerPage.ToString(CultureInfo.CurrentUICulture) + " commands per page [Yes],");
				message.Append(" or spread the linked commands to " + commandCapacityPerPageOld.ToString(CultureInfo.CurrentUICulture) + " commands per page [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Link Mode",
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
		public static bool TryClearLink(IWin32Window owner, PredefinedCommandSettings settingsOld, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			var message = new StringBuilder();
			var linkedCount = settingsOld.Pages.LinkedToFilePathCount;

			var selectedIsLinked = settingsOld.Pages[selectedPageId].IsLinkedToFilePath;
			if (selectedIsLinked)
			{
				if (linkedCount > 1)
				{
					message.AppendLine("Would you like to clear the link of all " + linkedCount.ToString(CultureInfo.CurrentUICulture) + " linked pages [Yes],");
					message.Append("or just of the currently selected linked page " + selectedPageId.ToString(CultureInfo.CurrentUICulture) + " [No]?");

					switch (MessageBoxEx.Show
						(
							owner,
							message.ToString(),
							"Clear Mode",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question
						))
					{
						case DialogResult.Yes: return (TryClearLinkAll(settingsOld,                 out settingsNew));
						case DialogResult.No:  return (TryClearLinkOne(settingsOld, selectedPageId, out settingsNew));

						default: settingsNew = null; return (false);
					}
				}
				else // Just the selected page is linked:
				{
					message.Append("Clear the link of page " + selectedPageId.ToString(CultureInfo.CurrentUICulture) + "?");
				}
			}
			else if (linkedCount > 1)
			{
				message.Append("Clear the link of all " + linkedCount.ToString(CultureInfo.CurrentUICulture) + " linked pages?");
			}
			else if (linkedCount == 1)
			{
				message.Append("Clear the link of the linked page?");
			}

			if ((message != null) && (message.Length > 0))
			{
				if (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Clear?",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question,
						MessageBoxDefaultButton.Button2
					)
					== DialogResult.Yes)
				{
					return (TryClearLinkAll(settingsOld, out settingsNew));
				}
			}

			settingsNew = null;
			return (false);
		}

		/// <remarks>Boolean return for symmericity with other methods.</remarks>
		private static bool TryClearLinkAll(PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			// Clone the settings...
			settingsNew = new PredefinedCommandSettings(settingsOld); // Clone settings to preserve pages and other properties.

			// ...and tell the collection to unlink each page:
			settingsNew.Pages.UnlinkAll();
			return (true);
		}

		/// <remarks>Boolean return for symmericity with other methods.</remarks>
		private static bool TryClearLinkOne(PredefinedCommandSettings settingsOld, int pageId, out PredefinedCommandSettings settingsNew)
		{
			// Clone the settings...
			settingsNew = new PredefinedCommandSettings(settingsOld); // Clone settings to preserve pages and other properties.

			// ...and unlink the page:
			settingsNew.Pages[pageId - 1].Unlink();
			return (true);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
