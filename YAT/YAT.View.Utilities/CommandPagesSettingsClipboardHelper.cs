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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using MKY.Collections;
using MKY.Diagnostics;
using MKY.Windows.Forms;
using MKY.Xml;
using MKY.Xml.Serialization;

using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Settings.Model;

#endregion

namespace YAT.View.Utilities
{
	/// <remarks>
	/// Note there are similar implementations "Change", "Clipboard", "File" and "FileLink" to deal
	/// with subtle differences in behavior. Intentionally kept in parallel rather than making the
	/// implementation all-inclusive but less comprehensive. Separate classes to ease diffing.
	/// </remarks>
	public static class CommandPagesSettingsClipboardHelper
	{
		private enum Mode
		{
			Cancel,
			Neutral,
			Enlarge,
			Spread
		}

		/// <summary>
		/// Copies to the clipboard, prompting the user as required.
		/// </summary>
		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Set" same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TrySet(IWin32Window owner, PredefinedCommandSettings settings, int selectedPageId)
		{
			var pageCount = settings.Pages.Count;
			if (pageCount <= 1)        // Copy without asking:
			{                          // Specifying '1' will copy a single page (not all).
				return (TrySet(owner, settings.Pages, 1));
			}
			else // if (pageCount > 1)
			{
				var message = new StringBuilder();
				message.Append("Would you like to copy all " + pageCount.ToString(CultureInfo.CurrentCulture) + " pages [Yes],");
				message.Append(" or just the currently selected page " + selectedPageId.ToString(CultureInfo.CurrentCulture) + " [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Copy Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: return (TrySetAll(owner, settings));
					case DialogResult.No:  return (TrySetOne(owner, settings, selectedPageId));

					default:               return (false);
				}
			}
		}

		/// <summary>
		/// Copies all pages to the clipboard.
		/// </summary>
		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Set" same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TrySetAll(IWin32Window owner, PredefinedCommandSettings settings)
		{                                                          // Specifying 'NoPageId' will copy all pages (not a single).
			return (TrySet(owner, settings.Pages, PredefinedCommandPageCollection.NoPageId));
		}

		/// <summary>
		/// Copies the given page to the clipboard.
		/// </summary>
		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Set" same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TrySetOne(IWin32Window owner, PredefinedCommandSettings settings, int pageId)
		{
			var pages = new PredefinedCommandPageCollection();
			pages.Add(new PredefinedCommandPage(settings.Pages[pageId - 1])); // Clone to ensure decoupling.
			              // Specifying '1' will copy a single page (not all).
			return (TrySet(owner, pages, 1));
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Set" same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool TrySet(IWin32Window owner, PredefinedCommandPageCollection pages, int pageId)
		{
			if ((pages.Count == 1) && (pageId != PredefinedCommandPageCollection.NoPageId))
				return (TrySet(owner, pages[0]));
			else
				return (TrySet(owner, pages));
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Set" same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		private static bool TrySet(IWin32Window owner, PredefinedCommandPage page)
		{
			try
			{
				var root = new CommandPageSettingsRoot();
				root.Page = page;

				var sb = new StringBuilder();
				XmlSerializerEx.SerializeToString(typeof(CommandPageSettingsRoot), root, CultureInfo.CurrentCulture, ref sb);
				Clipboard.SetText(sb.ToString());

				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				var text = new StringBuilder();
				text.AppendLine("Failed to copy to clipboard!");
				text.AppendLine();
				text.Append    ("Make sure the clipboard is not blocked by another process.");

				MessageBoxEx.Show
				(
					owner,
					text.ToString(),
					"Clipboard Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Set" same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		private static bool TrySet(IWin32Window owner, PredefinedCommandPageCollection pages)
		{
			try
			{
				var root = new CommandPagesSettingsRoot();
				root.Pages = pages;

				var sb = new StringBuilder();
				XmlSerializerEx.SerializeToString(typeof(CommandPagesSettingsRoot), root, CultureInfo.CurrentCulture, ref sb);
				Clipboard.SetText(sb.ToString());

				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				var text = new StringBuilder();
				text.AppendLine("Failed to copy to clipboard!");
				text.AppendLine();
				text.Append    ("Make sure the clipboard is not blocked by another process.");

				MessageBoxEx.Show
				(
					owner,
					text.ToString(),
					"Clipboard Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Get" same as e.g. <see cref="Clipboard.GetText()"/>.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for fallback handling.")]
		private static bool TryGet(IWin32Window owner, out PredefinedCommandPageCollection pages)
		{
			string s;

			try
			{
				s = Clipboard.GetText();
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				var text = new StringBuilder();
				text.AppendLine("Failed to paste from clipboard!");
				text.AppendLine();
				text.Append    ("Make sure the clipboard is not blocked by another process.");

				MessageBoxEx.Show
				(
					owner,
					text.ToString(),
					"Clipboard Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				pages = null;
				return (false);
			}

			// First, try to deserialize from whole set of pages:
			object root = null;
			try
			{
				AlternateXmlElement[] alternateXmlElements = null; // Neither CommandPagesSettingsRoot nor CommandPageSettingsRoot (yet) have alternate elements.
				root = XmlSerializerEx.DeserializeFromStringInsisting(typeof(CommandPagesSettingsRoot), alternateXmlElements, s);
			}
			catch (Exception exPages)
			{
				DebugEx.WriteException(typeof(CommandPagesSettingsClipboardHelper), exPages, "Deserialization from whole set of pages has failed, trying from single page.");
			}

			if (root != null)
			{
				// For some reason, default deserialization will wrongly deserialize a 'CommandPageSettingsRoot'
				// as a 'CommandPagesSettingsRoot'. Working around this by checking for (Pages.Count > 0).

				// Alternatively, this helper could always set/get a 'CommandPagesSettingsRoot' to/from the clipboard.
				// However, decided to not do this for four reasons:
				//  > A single page simply is not a collection of pages.
				//  > Simplicity for those who manually edit the XML text.
				//  > Symmetricity to export/import to .yacp/.yacps file.
				//  > Issue described here and below still applies, thus a workaround/check would still be needed.

				var rootCasted = (CommandPagesSettingsRoot)root;
				if (!ICollectionEx.IsNullOrEmpty(rootCasted.Pages))
				{
					if (rootCasted.Pages.TotalDefinedCommandCount < 1)
					{
						MessageBoxEx.Show
						(
							owner,
							((rootCasted.Pages.Count == 1) ? "Page contains" : "Pages contain") + " no commands.",
							"No Commands",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation
						);

						pages = null;
						return (false);
					}

					pages = rootCasted.Pages;
					return (true);
				}
			}

			// Then, try to deserialize from single page:
			try
			{
				AlternateXmlElement[] alternateXmlElements = null; // Neither CommandPagesSettingsRoot nor CommandPageSettingsRoot (yet) have alternate elements.
				root = XmlSerializerEx.DeserializeFromStringInsisting(typeof(CommandPageSettingsRoot), alternateXmlElements, s);
			}
			catch (Exception exPage)
			{
				DebugEx.WriteException(typeof(CommandPagesSettingsClipboardHelper), exPage, "Deserialization from single page has failed too!");
			}

			if (root != null)
			{
				// For the same reason as further above, default deserialization likely wrongly deserializes something else
				// as a 'CommandPageSettingsRoot'. Working around this by checking for (Page.DefinedCommandCount > 0).

				var rootCasted = (CommandPageSettingsRoot)root;
				if ((rootCasted.Page != null) && (rootCasted.Page.DefinedCommandCount > 0))
				{
					pages = new PredefinedCommandPageCollection();
					pages.Add(rootCasted.Page);
					return (true);
				}
			}

			MessageBoxEx.Show
			(
				owner,
				"Clipboard does not contain valid " + ApplicationEx.CommonName + " command page(s) definition content.",
				"Clipboard Content Not Valid",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
			);

			pages = null;
			return (false);
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Get" same as e.g. <see cref="Clipboard.GetText()"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryGetAndImportAll(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (TryGet(owner, out pagesImported))
			{
				if (settingsOld.Pages.TotalDefinedCommandCount > 0)
				{
					var message = new StringBuilder();
					message.Append("The clipboard contains ");
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
							"Confirm Paste",
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
		/// <remarks>Named "Get" same as e.g. <see cref="Clipboard.GetText()"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryGetAndImport(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (TryGet(owner, out pagesImported))
			{
				if (settingsOld.Pages.TotalDefinedCommandCount > 0)
				{
					var message = new StringBuilder();
					message.Append("The clipboard contains ");
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
							"Paste Mode",
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
		/// <remarks>Named "Get" same as e.g. <see cref="Clipboard.GetText()"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryGetAndInsert(IWin32Window owner, PredefinedCommandSettings settingsOld, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (TryGet(owner, out pagesImported))
			{
				                                           // Specifying 'selectedPageId' will insert (instead of add).
				return (TryAddOrInsert(owner, settingsOld, pagesImported, selectedPageId, out settingsNew));
			}

			settingsNew = null;
			return (false);
		}

		/// <remarks>In case of an error, a modal message box is shown to the user.</remarks>
		/// <remarks>Named "Get" same as e.g. <see cref="Clipboard.GetText()"/>.</remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryGetAndAdd(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (TryGet(owner, out pagesImported))
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
				message.Append("The clipboard contains ");
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
				message.Append(         " or spread the pasted pages to " + commandCapacityPerPageOld.ToString(CultureInfo.CurrentCulture) + " commands per page [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Paste Mode", // Note that message states "Paste" while code uses "Import" for file as well as clipboard.
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
