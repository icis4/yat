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
// MKY Version 1.0.22
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	public static class LinkHelper
	{
		/// <summary>
		/// Clicks the link of the given <see cref="LinkLabel"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "'LinkData' just happens to contain a string...")]
		public static bool TryBrowseUriAndShowErrorIfItFails(IWin32Window owner, LinkLabelLinkClickedEventArgs e)
		{
			var linkUri = (e.Link.LinkData as string);
			if (linkUri != null)
			{
				Exception ex;
				if (Net.Browser.TryBrowseUri(linkUri, out ex))
				{
					e.Link.Visited = true;

					return (true);
				}
				else
				{
					string message = "Unable to open link!" + Environment.NewLine + Environment.NewLine +
					                 "System error message:" + Environment.NewLine + ex.Message;
					MessageBoxEx.Show
					(
						owner,
						message,
						"Link Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);

					return (false);
				}
			}
			else
			{
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Link data is invalid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
