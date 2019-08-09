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
			else // Just a single page => export without asking:
			{
				return (TryExport(owner, commandPages, indicatedName));
			}
		}

		/// <summary>
		/// Export all pages to the clipboard.
		/// </summary>
		public static bool TryExportAllPages(IWin32Window owner, PredefinedCommandSettings commandPages, string indicatedName)
		{
			return (TryExport(owner, commandPages, indicatedName));
		}

		/// <summary>
		/// Export the given page to the clipboard.
		/// </summary>
		public static bool TryExportSelectedPage(IWin32Window owner, PredefinedCommandSettings commandPages, int selectedPageId, string indicatedName)
		{
			var p = new PredefinedCommandSettings(commandPages); // Clone page to get same properties.
			p.Pages.Clear();
			p.Pages.Add(new PredefinedCommandPage(commandPages.Pages[selectedPageId - 1])); // Clone page to ensure decoupling.

			return (TryExport(owner, p, indicatedName));
		}

		/// <summary></summary>
		private static bool TryExport(IWin32Window owner, PredefinedCommandSettings commandPages, string indicatedName)
		{
			return (false); // PENDING
		}

		/// <summary></summary>
		public static bool TrySet(PredefinedCommandSettings commandPages, int selectedPageId)
		{
			var root = new CommandPageSettingsRoot();
			root.Page = commandPages.Pages[selectedPageId - 1];

			var sb = new StringBuilder();
			XmlSerializerEx.SerializeToString(typeof(CommandSettingsRoot), root, ref sb);

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
			// PENDING
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
		public static bool TryGetAndInsert(IWin32Window owner, PredefinedCommandSettings commandPagesOld, int selectedPageId, out PredefinedCommandSettings commandPagesNew)
		{
			// PENDING
			commandPagesNew = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryGetAndAdd(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{
			// PENDING
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
