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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

		/// <summary></summary>
		public static bool TryExport(IWin32Window owner, PredefinedCommandSettings settings, int selectedPageId)
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
					case DialogResult.Yes: return (TryExportAll(settings));
					case DialogResult.No:  return (TryExportOne(settings, selectedPageId));

					default:               return (false);
				}
			}
			else // If (pageCount <= 1) export without asking:
			{                      // Specifying '1' will export a single page (not all).
				return (TryExport(settings.Pages, 1));
			}
		}

		/// <summary>
		/// Export all pages to the clipboard.
		/// </summary>
		public static bool TryExportAll(PredefinedCommandSettings settings)
		{                                                      // Specifying 'NoPageId' will export all pages (not a single).
			return (TryExport(settings.Pages, PredefinedCommandPageCollection.NoPageId));
		}

		/// <summary>
		/// Export the given page to the clipboard.
		/// </summary>
		public static bool TryExportOne(PredefinedCommandSettings settings, int pageId)
		{
			var pages = new PredefinedCommandPageCollection();
			pages.Add(new PredefinedCommandPage(settings.Pages[pageId - 1])); // Clone to ensure decoupling.
			          // Specifying '1' will export a single page (not all).
			return (TryExport(pages, 1));
		}

		/// <summary></summary>
		private static bool TryExport(PredefinedCommandPageCollection pages, int pageId)
		{
			if ((pages.Count == 1) && (pageId != PredefinedCommandPageCollection.NoPageId))
				return (TrySet(pages[0]));
			else
				return (TrySet(pages));
		}

		/// <summary></summary>
		private static bool TrySet(PredefinedCommandPage page)
		{
			try
			{
				var root = new CommandPageSettingsRoot();
				root.Page = page;

				var sb = new StringBuilder();
				XmlSerializerEx.SerializeToString(typeof(CommandPageSettingsRoot), root, ref sb);
				Clipboard.SetText(sb.ToString());

				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				return (false);
			}
		}

		/// <summary></summary>
		private static bool TrySet(PredefinedCommandPageCollection pages)
		{
			try
			{
				var root = new CommandPagesSettingsRoot();
				root.Pages = pages;

				var sb = new StringBuilder();
				XmlSerializerEx.SerializeToString(typeof(CommandPagesSettingsRoot), root, ref sb);
				Clipboard.SetText(sb.ToString());

				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				return (false);
			}
		}

		/// <summary></summary>
		private static bool TryGet(out PredefinedCommandPageCollection pages)
		{
			string s;

			try
			{
				s = Clipboard.GetText();
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
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
				var rootCasted = (CommandPagesSettingsRoot)root;
				if ((rootCasted.Pages != null) && (rootCasted.Pages.Count > 0))
				{
					pages = rootCasted.Pages;
					return (true);
				}

				// For some reason, default deserialization will wrongly deserializes a 'CommandPageSettingsRoot'
				// as a 'CommandPagesSettingsRoot' above. Working around this by checking for (Pages.Count > 0).

				// Alternatively, this helper could always set/get a 'CommandPagesSettingsRoot' to/from the clipboard.
				// However, decided to not do this for four reasons:
				//  > A single page simply is not a collection of pages.
				//  > Simplicity for those who manually edit the XML text.
				//  > Symmetricity to export/import to .yacp/.yacps file.
				//  > Issue described here and below still applies, thus a workaround/check would still be needed.
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
				var rootCasted = (CommandPageSettingsRoot)root;
				if ((rootCasted.Page != null) && (rootCasted.Page.DefinedCommandCount > 0))
				{
					pages = new PredefinedCommandPageCollection();
					pages.Add(rootCasted.Page);
					return (true);
				}

				// For the same reason as above, default deserialization likely wrongly deserializes something else
				// as a 'CommandPageSettingsRoot' above. Working around this by checking for (Page.DefinedCommandCount > 0).
			}

			pages = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryGetAndImport(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (TryGet(out pagesImported))
			{
				var message = new StringBuilder();
				message.Append("Clipboard contains ");
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
		public static bool TryGetAndInsert(IWin32Window owner, PredefinedCommandSettings settingsOld, int selectedPageId, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (TryGet(out pagesImported))
			{
				                                           // Specifying 'selectedPageId' will insert (instead of add).
				return (TryAddOrInsert(owner, settingsOld, pagesImported, selectedPageId, out settingsNew));
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryGetAndAdd(IWin32Window owner, PredefinedCommandSettings settingsOld, out PredefinedCommandSettings settingsNew)
		{
			PredefinedCommandPageCollection pagesImported;
			if (TryGet(out pagesImported))
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
			var commandCapacityPerPageOld = ((PredefinedCommandPageLayoutEx)pageLayoutOld).CommandCapacityPerPage;
			if (pagesImported.MaxCommandCountPerPage <= commandCapacityPerPageOld)
			{
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutOld;
				return (true);
			}
			else // The pages to import do not fit the currently configured page layout:
			{
				var nextPageLayout = PredefinedCommandPageLayoutEx.GetMatchingItem(pagesImported.MaxCommandCountPerPage);
				var nextCommandCapacityPerPage = nextPageLayout.CommandCapacityPerPage;

				var message = new StringBuilder();
				message.Append("Clipboard contains ");
				message.Append(pagesImported.Count == 1 ? " page" : " pages");
				message.Append(" with up to ");
				message.Append(pagesImported.MaxCommandCountPerPage);
				message.Append(" commands per page, but currently ");
				message.Append(commandCapacityPerPageOld);
				message.AppendLine(" commands per page are configured.");
				message.AppendLine();
				message.Append("Would you like to enlarge the pages to " + nextCommandCapacityPerPage.ToString(CultureInfo.CurrentUICulture) + " commands per page [Yes],");
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
