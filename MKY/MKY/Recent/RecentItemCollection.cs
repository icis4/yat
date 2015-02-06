//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MKY.Recent
{
	/// <summary>
	/// Collection for recent items like recent files, provides methods to handle the recent items.
	/// </summary>
	/// <typeparam name="T">The underlying type of the recent item.</typeparam>
	[Serializable]
	public class RecentItemCollection<T> : List<RecentItem<T>>
		where T : IEquatable<T>
	{
		/// <summary></summary>
		public RecentItemCollection()
			: base()
		{
		}

		/// <summary></summary>
		public RecentItemCollection(int capacity)
			: base(capacity)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		public RecentItemCollection(IEnumerable<RecentItem<T>> collection)
			: base(collection)
		{
		}

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Inserts the recent item at the beginning of the collection (least recent). The most
		/// recent item will be removed in case the collection already contains <see cref="T:RecentItemCollection`1.Capacity"/> items.
		/// </summary>
		public virtual void ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(RecentItem<T> item)
		{
			// Remove all equal items, as they are becoming more recent than the new item:
			RemoveAll(li => (li == item));

			// Ensure there is space for the item to be inserted:
			while (Count >= Capacity)
				RemoveMostRecent();

			Insert(0, item);
		}

		/// <summary>
		/// Remove the most recent item if the collection already contains <see cref="T:RecentItemCollection`1.Capacity"/> items.
		/// </summary>
		/// <returns>
		/// <c>true</c> if an item is successfully removed; otherwise, <c>false</c>.
		/// <c>false</c> is also returned if no item was not found in the collection.
		/// </returns>
		public virtual bool RemoveMostRecent()
		{
			if (Count > 0)
			{
				List<RecentItem<T>> sorted = new List<RecentItem<T>>(this);
				sorted.Sort();
				return (Remove(sorted[sorted.Count - 1])); // Remove the last item in the collection.
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Removes all more recent duplicates from the collection, i.e. copies of items that have
		/// an older time stamp.
		/// </summary>
		/// <returns>
		/// The number of duplicates removed from the collection.
		/// </returns>
		public virtual int RemoveDuplicates()
		{
			// Keep the original item count:
			int countWithDuplicates = Count;

			// If there are indeed duplicates, take the long to remove the recent ones...
			while (this.Distinct().ToList<RecentItem<T>>().Count < Count)
			{
				List<RecentItem<T>> duplicates = new List<RecentItem<T>>();

				// Traverse the collection and search for duplicates:
				for (int outer = 0; outer < (Count-1); outer++)
				{
					for (int inner = (outer+1); inner < Count; inner++)
					{
						if (this[inner].Item.Equals(this[outer].Item))
						{
							if (this[inner].TimeStamp < this[outer].TimeStamp)
								duplicates.Add(this[inner]);
							else
								duplicates.Add(this[outer]);
						}
					}
				}

				// Remove the duplicates from the collection:
				foreach (RecentItem<T> ri in duplicates)
				{
					Remove(ri);
				}
			}

			return (countWithDuplicates - Count);
		}

		/// <summary>
		/// Validates all recent items, invalid items are removed from the collection.
		/// </summary>
		public virtual void ValidateAll()
		{
			List<RecentItem<T>> invalids = new List<RecentItem<T>>();
			foreach (RecentItem<T> ri in this)
			{
				if (!ri.IsValid)
					invalids.Add(ri);
			}
			foreach (RecentItem<T> ri in invalids)
			{
				Remove(ri);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
