//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="System.Windows.Forms"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ListBoxEx
	{
		/// <summary>
		/// Scroll list to index.
		/// </summary>
		public static void ScrollToIndex(ListBox lb, int index)
		{
			lb.TopIndex = index;
		}

		/// <summary>
		/// Scroll list to bottom.
		/// </summary>
		public static void ScrollToBottom(ListBox lb)
		{
			lb.TopIndex = (lb.Items.Count - 1);
		}

		/// <summary>
		/// Scroll list to bottom if no items are selected.
		/// </summary>
		public static bool ScrollToBottomIfNoItemsAreSelected(ListBox lb)
		{
			if ((lb.SelectedItems.Count == 0) && (lb.Items.Count > 0))
			{
				ScrollToBottom(lb);
				return (true);
			}

			return (false);
		}

		/// <summary>
		/// Scroll list to bottom if no items are selected, except the last.
		/// </summary>
		public static bool ScrollToBottomIfNoItemButTheLastIsSelected(ListBox lb)
		{
			if (ScrollToBottomIfNoItemsAreSelected(lb))
				return (true);

			if (lb.SelectedIndices.Count == 1)
			{
				if (lb.SelectedIndices[0] == (lb.Items.Count - 1))
				{
					lb.SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.
					ScrollToBottom(lb);
					return (true);
				}
				if (lb.SelectedIndices[0] == (lb.Items.Count - 2))
				{
					lb.SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.
					ScrollToBottom(lb);
					return (true);
				}
			}
			
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
