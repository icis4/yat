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
	/// Note there are similar implementations "Change", "Clipboard", "File" and "FileLink" to deal
	/// with subtle differences in behavior. Intentionally kept in parallel rather than making the
	/// implementation all-inclusive but less comprehensive. Separate classes to ease diffing.
	/// </remarks>
	public static class CommandPagesSettingsFileLinkHelper
	{
		private enum Mode
		{
			Cancel,
			Neutral,
			Enlarge
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static DialogResult ShowSaveAsFileDialog(Control owner, string filePathOld, out string filePathNew)
		{
			var sfd = new SaveFileDialog();
			sfd.Title       = "Link Command Page";
			sfd.Filter      = ExtensionHelper.CommandPageFilesFilter;
			sfd.FilterIndex = ExtensionHelper.CommandPageFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageFile);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;
			sfd.FileName    = Path.GetFileName(filePathOld);

			var dr = sfd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				owner.Refresh();

				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				filePathNew = sfd.FileName;
			}
			else
			{
				filePathNew = null;
			}

			return (dr);
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static DialogResult ShowOpenFileDialog(Control owner, string filePathOld, out string filePathNew)
		{
			var ofd = new OpenFileDialog();
			ofd.Title       = "Link Command Page";
			ofd.Filter      = ExtensionHelper.CommandPageFilesFilter;
			ofd.FilterIndex = ExtensionHelper.CommandPageFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageFile);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;
			ofd.FileName    = Path.GetFileName(filePathOld);

			var dr = ofd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				owner.Refresh();

				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				filePathNew = ofd.FileName;
			}
			else
			{
				filePathNew = null;
			}

			return (dr);
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool ShowOpenFileDialogAndTryLoad(IWin32Window owner, out string filePath, out PredefinedCommandPage page)
		{
			var ofd = new OpenFileDialog();
			ofd.Title       = "Link Command Page";
			ofd.Filter      = ExtensionHelper.CommandPageFilesFilter;
			ofd.FilterIndex = ExtensionHelper.CommandPageFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.CommandPageFile);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.CommandFiles;

			var dr = ofd.ShowDialog(owner);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.CommandFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Exception ex;
				if (TryLoad(ofd.FileName, out page, out ex))
				{
					if (page.DefinedCommandCount < 1)
					{
						MessageBoxEx.Show
						(
							"File contains no commands.",
							"No Commands",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation
						);

						filePath = null;
						return (false);
					}

					filePath = ofd.FileName;
					return (true);
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
			page = null;
			return (false);
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryLoadAndLink(IWin32Window owner, PredefinedCommandSettings settingsOld, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			string filePathToLink;
			PredefinedCommandPage pageToLink;
			if (ShowOpenFileDialogAndTryLoad(owner, out filePathToLink, out pageToLink))
				return (TryLink(owner, settingsOld, selectedPageId, filePathToLink, pageToLink, out settingsNew));

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool TryLink(IWin32Window owner, PredefinedCommandSettings settingsOld, int selectedPageId, string filePathToLink, PredefinedCommandPage pageToLink, out PredefinedCommandSettings settingsNew)
		{
			Mode mode;
			PredefinedCommandPageLayout pageLayoutNew;
			if (ConfirmLink(owner, pageToLink, settingsOld.PageLayout, out mode, out pageLayoutNew))
			{
				// Clone settings to preserve pages and other properties...
				settingsNew = new PredefinedCommandSettings(settingsOld);

				// ...potentially adjust layout...
				settingsNew.PageLayout = pageLayoutNew;

				// ...add default page if yet empty...
				if (settingsNew.Pages.Count == 0)
					settingsNew.Pages.Add(PredefinedCommandPageCollection.DefaultPage);

				// ...and then...
				switch (mode)
				{
					case (Mode.Neutral):
					case (Mode.Enlarge):
					{
						// ...link:
						settingsNew.Pages[selectedPageId - 1].Link(filePathToLink, pageToLink.Name, pageToLink.Commands); // No clone needed as just loaded.

						return (true);
					}

					default: // incl. Mode.Cancel
					{
						break; // Do nothing.
					}
				}
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool ConfirmLink(IWin32Window owner, PredefinedCommandPage pageToLink, PredefinedCommandPageLayout pageLayoutOld, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			var commandCapacityPerPageOld = ((PredefinedCommandPageLayoutEx)pageLayoutOld).CommandCapacityPerPage;
			if (pageToLink.DefinedCommandCount <= commandCapacityPerPageOld)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutOld;
				return (true);
			}
			else // The pages to link do not fit the currently configured page layout:
			{
				var nextPageLayout = PredefinedCommandPageLayoutEx.GetMatchingItem(pageToLink.DefinedCommandCount);
				var nextCommandCapacityPerPage = nextPageLayout.CommandCapacityPerPage;

				var message = new StringBuilder();
				message.Append("The file to link contains ");
				message.Append(pageToLink.DefinedCommandCount);
				message.Append(" commands, but currently ");
				message.Append(commandCapacityPerPageOld);
				message.AppendLine(" commands per page are configured.");
				message.AppendLine();
				message.Append("Enlarge the current pages to " + nextCommandCapacityPerPage.ToString(CultureInfo.CurrentUICulture) + " commands per page?");

				if (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Confirm Link",
						MessageBoxButtons.OKCancel,
						MessageBoxIcon.Question,
						MessageBoxDefaultButton.Button2
					) == DialogResult.OK)
				{
					mode = Mode.Enlarge;
					pageLayoutNew = nextPageLayout;
					return (true);
				}

				mode = Mode.Cancel;
				pageLayoutNew = pageLayoutOld;
				return (false);
			}
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryClearLink(IWin32Window owner, PredefinedCommandSettings settingsOld, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			var message = new StringBuilder();
			var linkedCount = settingsOld.Pages.LinkedToFilePathCount;

			var selectedIsLinked = settingsOld.Pages[selectedPageId - 1].IsLinkedToFilePath;
			if (selectedIsLinked)
			{
				if (linkedCount > 1)
				{
					message.Append("Would you like to clear the link of all " + linkedCount.ToString(CultureInfo.CurrentUICulture) + " linked pages [Yes],");
					message.Append(" or just of the currently selected linked page " + selectedPageId.ToString(CultureInfo.CurrentUICulture) + " [No]?");

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
				message.Append("Clear the link of the page?");
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
			// Clone settings to preserve pages and other properties...
			settingsNew = new PredefinedCommandSettings(settingsOld);

			// ...and tell the collection to unlink each page:
			settingsNew.Pages.UnlinkAll();
			return (true);
		}

		/// <remarks>Boolean return for symmericity with other methods.</remarks>
		private static bool TryClearLinkOne(PredefinedCommandSettings settingsOld, int pageId, out PredefinedCommandSettings settingsNew)
		{
			// Clone settings to preserve pages and other properties...
			settingsNew = new PredefinedCommandSettings(settingsOld);

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
