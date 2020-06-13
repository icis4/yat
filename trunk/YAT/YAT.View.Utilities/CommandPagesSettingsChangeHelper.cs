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
	public static class CommandPagesSettingsChangeHelper
	{
		private enum Mode
		{
			Cancel,
			Neutral,
			Spread,
			Merge,
			Truncate
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static bool TryChange(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageLayout pageLayoutRequested, out PredefinedCommandSettings settingsNew)
		{
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
						{
							// ...add:
							settingsNew.Pages.AddRange(new PredefinedCommandPageCollection(settingsOld.Pages)); // Clone to ensure decoupling.

							return (true);
						}

						case (Mode.Spread):
						{
							var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutNew).CommandCapacityPerPage;

							// ...spread:
							settingsNew.Pages.AddSpreaded(new PredefinedCommandPageCollection(settingsOld.Pages), commandCapacityPerPageNew); // Clone to ensure decoupling.

							return (true);
						}

						case (Mode.Merge):
						{
							var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutNew).CommandCapacityPerPage;
							var commandCapacityPerPageOld = ((PredefinedCommandPageLayoutEx)(settingsOld.PageLayout)).CommandCapacityPerPage;

							// ...merge:
							settingsNew.Pages.AddMerged(new PredefinedCommandPageCollection(settingsOld.Pages), commandCapacityPerPageOld, commandCapacityPerPageNew); // Clone to ensure decoupling.

							return (true);
						}

						case (Mode.Truncate):
						{
							var commandCapacityPerPageNew = ((PredefinedCommandPageLayoutEx)pageLayoutNew).CommandCapacityPerPage;

							// ...truncate:
							settingsNew.Pages.AddTruncated(new PredefinedCommandPageCollection(settingsOld.Pages), commandCapacityPerPageNew); // Clone to ensure decoupling.

							return (true);
						}

						case (Mode.Cancel):
						default:
						{
							break; // Do nothing.
						}
					}
				}
				else
				{
					// ...add default page since empty:
					settingsNew.Pages.Add(PredefinedCommandPageCollection.DefaultPage);

					return (true);
				}
			}

			settingsNew = null;
			return (false);
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private static bool ConfirmChange(IWin32Window owner, PredefinedCommandSettings settingsOld, PredefinedCommandPageLayout pageLayoutRequested, out Mode mode, out PredefinedCommandPageLayout pageLayoutNew)
		{
			var pageLayoutOld = settingsOld.PageLayout;

			var commandCapacityPerPageOld       = ((PredefinedCommandPageLayoutEx)pageLayoutOld)      .CommandCapacityPerPage;
			var commandCapacityPerPageRequested = ((PredefinedCommandPageLayoutEx)pageLayoutRequested).CommandCapacityPerPage;
			if (commandCapacityPerPageRequested >= settingsOld.Pages.MaxDefinedCommandCountPerPage)
			{
				int potentialMergeRatio = (commandCapacityPerPageRequested / commandCapacityPerPageOld);

				if ((settingsOld.Pages.Count > 1) && // There are pages for potential merge.
				    (potentialMergeRatio > 1))       // Ratio is OK for merge, e.g. 24:12 or 48:24, but not 36:24.
				{
					var message = new StringBuilder();
					message.Append("The currently configured predefined commands ");
					message.Append(settingsOld.Pages.Count == 1 ? "contain " : "contain up to ");
					message.Append(settingsOld.Pages.MaxDefinedCommandCountPerPage);
					message.Append(" commands per page, and ");
					message.Append(commandCapacityPerPageRequested);
					message.AppendLine(" commands per page are requested now.");
					message.AppendLine();
					message.Append("Would you like to merge each " + potentialMergeRatio.ToString(CultureInfo.CurrentCulture));
					message.Append(" pages to a single page of " + commandCapacityPerPageRequested.ToString(CultureInfo.CurrentCulture) + " commands per page?");

					switch (MessageBoxEx.Show
						(
							owner,
							message.ToString(),
							"Change Mode",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question,
							MessageBoxDefaultButton.Button3
						))
					{
						case DialogResult.Yes: mode = Mode.Merge;  pageLayoutNew = pageLayoutRequested; return (true);
						case DialogResult.No:                                                           break;
						default:               mode = Mode.Cancel; pageLayoutNew = pageLayoutOld;       return (false);
					}
				}

				// The page layout can be kept:
				mode = Mode.Neutral;
				pageLayoutNew = pageLayoutRequested;
				return (true);
			}
			else // The current pages no longer fit the requested page layout:
			{
				int potentialSpreadRatio = (int)(Math.Ceiling(((double)(commandCapacityPerPageOld)) / (double)(commandCapacityPerPageRequested)));

				var message = new StringBuilder();
				message.Append("The currently configured predefined commands");
				message.Append(settingsOld.Pages.Count == 1 ? "contain " : "contain up to ");
				message.Append(settingsOld.Pages.MaxDefinedCommandCountPerPage);
				message.Append(" commands per page, but only ");
				message.Append(commandCapacityPerPageRequested);
				message.AppendLine(" commands per page are requested now.");
				message.AppendLine();
				message.Append("Would you like to spread each page to " + potentialSpreadRatio.ToString(CultureInfo.CurrentCulture));
				message.Append(" pages of " + commandCapacityPerPageRequested.ToString(CultureInfo.CurrentCulture) + " commands per page [Yes],");
				message.Append(" or truncate the pages to " + commandCapacityPerPageRequested.ToString(CultureInfo.CurrentCulture) + " commands per page [No]?");

				switch (MessageBoxEx.Show
					(
						owner,
						message.ToString(),
						"Change Mode",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question,
						MessageBoxDefaultButton.Button3
					))
				{
					case DialogResult.Yes: mode = Mode.Spread;   pageLayoutNew = pageLayoutRequested; return (true);
					case DialogResult.No:  mode = Mode.Truncate; pageLayoutNew = pageLayoutRequested; return (true);
					default:               mode = Mode.Cancel;   pageLayoutNew = pageLayoutOld;       return (false);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
