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
// MKY Version 1.0.27
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
				var success = TryBrowseUriAndShowErrorIfItFails(owner, linkUri);

				if (success)
					e.Link.Visited = true;

				return (success);
			}
			else
			{
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Link data is invalid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Clicks the given <paramref name="linkUri"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "Flexibility!")]
		[SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Flexibility!")]
		public static bool TryBrowseUriAndShowErrorIfItFails(IWin32Window owner, string linkUri)
		{
			if (!string.IsNullOrEmpty(linkUri))
			{
				Exception ex;
				if (Net.Browser.TryBrowseUri(linkUri, out ex))
				{
					return (true);
				}
				else
				{
					string message = "Unable to open <" + linkUri + ">!" + Environment.NewLine + Environment.NewLine +
					                 "System error message:" + Environment.NewLine + ex.Message;
					MessageBoxEx.Show
					(
						owner,
						message,
						"Link Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					return (false);
				}
			}
			else
			{
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Link is invalid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
