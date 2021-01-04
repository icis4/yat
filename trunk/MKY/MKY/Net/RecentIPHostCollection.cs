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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.Collections.Specialized;

namespace MKY.Net
{
	/// <summary>
	/// Collection for recent items like recent files, provides methods to handle the recent items.
	/// </summary>
	[Serializable]
	public class RecentIPHostCollection : RecentItemCollection<string>
	{
		/// <summary></summary>
		public RecentIPHostCollection()
			: base()
		{
		}

		/// <summary></summary>
		public RecentIPHostCollection(int capacity)
			: base(capacity)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		public RecentIPHostCollection(IEnumerable<RecentItem<string>> collection)
			: base(collection)
		{
		}

		/// <summary>
		/// Remove the most recent item if the collection already contains <see cref="T:RecentItemCollection`1.Capacity" /> items.
		/// </summary>
		/// <returns>
		/// <c>true</c> if an item is successfully removed; otherwise, <c>false</c>.
		/// <c>false</c> is also returned if no item was not found in the collection.
		/// </returns>
		public override bool RemoveMostRecent()
		{
			if (Count > 0)
			{
				var sortedReversed = new List<RecentItem<string>>(this);

				sortedReversed.Sort();
				sortedReversed.Reverse();

				foreach (var item in sortedReversed)
				{
					if (!IPHostEx.HasItem(item.Item)) // Do not remove the fixed items.
					{
						Remove(item.Item);
						return (true);
					}
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
