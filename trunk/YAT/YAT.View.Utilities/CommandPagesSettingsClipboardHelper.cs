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

using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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
	/// Duplicate implementation as "Clipboard" and "File" to deal with subtle differences rather
	/// than making the implementation less comprehensive. Separate classes to ease diffing.
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
		public static bool TryExport(IWin32Window owner, PredefinedCommandSettings commandPages, int selectedPageId)
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
					case DialogResult.Yes: return (TryExportAllPages(    commandPages));
					case DialogResult.No:  return (TryExportSelectedPage(commandPages, selectedPageId));
					default:               return (false);
				}
			}
			else // Just a single page => export all without asking:
			{                    // Specifying 'selectedPageId' will export the single page.
				return (TryExport(commandPages, selectedPageId));
			}
		}

		/// <summary>
		/// Export all pages to the clipboard.
		/// </summary>
		public static bool TryExportAllPages(PredefinedCommandSettings commandPages)
		{                                                    // Specifying 'NoPageId' will export all pages (not the selected).
			return (TryExport(commandPages, PredefinedCommandPageCollection.NoPageId));
		}

		/// <summary>
		/// Export the given page to the clipboard.
		/// </summary>
		public static bool TryExportSelectedPage(PredefinedCommandSettings commandPages, int selectedPageId)
		{
			var p = new PredefinedCommandSettings(commandPages); // Clone page to get same properties.
			p.Pages.Clear();
			p.Pages.Add(new PredefinedCommandPage(commandPages.Pages[selectedPageId - 1])); // Clone page to ensure decoupling.
			      // Specifying 'selectedPageId' will export a single page (not all pages).
			return (TryExport(p, selectedPageId));
		}

		/// <remarks>
		/// 1:1 wrapper for <see cref="TrySet(PredefinedCommandSettings, int)"/> for symmetricity with
		/// <see cref="CommandPagesSettingsFileHelper.TryExport(IWin32Window, PredefinedCommandSettings, int, string)"/>.
		/// </remarks>
		private static bool TryExport(PredefinedCommandSettings commandPages, int selectedPageId)
		{
			return (TrySet(commandPages, selectedPageId));
		}

		/// <summary></summary>
		public static bool TrySet(PredefinedCommandSettings commandPages, int selectedPageId)
		{
			var sb = new StringBuilder();

			if ((commandPages.Pages.Count == 1) && (selectedPageId != PredefinedCommandPageCollection.NoPageId))
			{
				var root = new CommandPageSettingsRoot();
				root.Page = commandPages.Pages[selectedPageId - 1];
				XmlSerializerEx.SerializeToString(typeof(CommandPageSettingsRoot), root, ref sb);
			}
			else
			{
				var root = new CommandPagesSettingsRoot();
				root.PredefinedCommand = commandPages;
				XmlSerializerEx.SerializeToString(typeof(CommandPagesSettingsRoot), root, ref sb);
			}

			try
			{
				Clipboard.SetText(sb.ToString());
				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				return (false);
			}
		}

		/// <summary></summary>
		public static bool TryGet(out PredefinedCommandSettings commandPages)
		{
			string s;

			try
			{
				s = Clipboard.GetText();
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				commandPages = null;
				return (false);
			}

			AlternateXmlElement[] alternateXmlElements = null; // Neither CommandPagesSettingsRoot nor CommandPageSettingsRoot (yet) have alternate elements.
			object root;

			// First, try to deserialize from whole set of pages:
			if (XmlSerializerEx.TryDeserializeFromStringInsisting(typeof(CommandPagesSettingsRoot), alternateXmlElements, s, out root))
			{
				var rootCasted = (CommandPagesSettingsRoot)root;
				if ((rootCasted.PredefinedCommand.Pages != null) && (rootCasted.PredefinedCommand.Pages.Count > 0))
				{
					commandPages = rootCasted.PredefinedCommand;
					return (true);
				}

				// For some reason, default deserialization will wrongly deserializes a 'CommandPageSettingsRoot'
				// as a 'CommandPagesSettingsRoot' above. Working around this by checking for (Pages.Count > 0).

				// Alternatively, this helper could always set/get a 'CommandPagesSettingsRoot' to/from the clipboard.
				// However, decided to not do this for two reasons:
				//  > Symmetricity with file export/import to .yacp/.yacps
				//  > Issue described here and below still applies, thus a workaround/check would still be needed.
			}

			// Then, try to deserialize from single page:
			if (XmlSerializerEx.TryDeserializeFromStringInsisting(typeof(CommandPageSettingsRoot), alternateXmlElements, s, out root))
			{
				var rootCasted = (CommandPageSettingsRoot)root;
				if ((rootCasted.Page != null) && (rootCasted.Page.DefinedCommandCount > 0))
				{
					commandPages = new PredefinedCommandSettings();
					commandPages.Pages.Add(rootCasted.Page);
					return (true);
				}

				// For the same reason as above, default deserialization likely wrongly deserializes something else
				// as a 'CommandPageSettingsRoot' above. Working around this by checking for (Page.DefinedCommandCount > 0).
			}

			commandPages = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryGetAndImport(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (TryGet(out imported))
			{
				var message = new StringBuilder();
				message.Append("Clipboard contains ");
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
		public static bool TryGetAndInsert(IWin32Window owner, PredefinedCommandSettings commandPagesOld, int selectedPageId, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (TryGet(out imported))
			{
				                                          // Specifying 'selectedPageId' will insert (instead of add).
				return (TryAddOrInsert(owner, commandPagesOld, imported, selectedPageId, out commandPagesNew));
			}

			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryGetAndAdd(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{
			PredefinedCommandSettings imported;
			if (TryGet(out imported))
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
