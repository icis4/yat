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
// MKY Version 1.0.30
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
using System.Linq;

namespace MKY.Collections.Specialized
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
		/// Adds the item to the collection. The item will be inserted at the beginning of
		/// the collection (least recent). If the collection already contains the specified number
		/// of items (<see cref="T:RecentItemCollection`1.Capacity"/>), the most recent item will
		/// be removed from the collection.
		/// </summary>
		public virtual void Add(T item)
		{
			Add(new RecentItem<T>(item));
		}

		/// <summary>
		/// Adds the item to the collection. The item will be inserted at the location according
		/// to the time stamp of the item. If the collection already contains the specified number
		/// of items (<see cref="T:RecentItemCollection`1.Capacity"/>), the most recent item will
		/// be removed from the collection.
		/// </summary>
		public new void Add(RecentItem<T> item)
		{
			// Remove all equal items, as they are becoming less recent than the new item:
			RemoveAll(li => (li == item));

			// Ensure there is space for the item to be added:
			while ((Count > 0) && (Count >= Capacity))
			{
				RemoveMostRecent();
			}

			// Insert the item at the according location:
			Insert(0, item);
			Sort(); // Typically not needed, but item could contain a more recent time stamp.
		}

		/// <summary>
		/// Adds the items to the collection. The items will be inserted at the location according
		/// to the time stamp of the item. If the collection already contains the specified number
		/// of items (<see cref="T:RecentItemCollection`1.Capacity"/>), the most recent item will
		/// be removed from the collection.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		public new void AddRange(IEnumerable<RecentItem<T>> collection)
		{
			foreach (var item in collection)
				Add(item);
		}

		/// <summary>
		/// Determines whether collection contains the specified item.
		/// </summary>
		public virtual bool Contains(T item)
		{
			foreach (var ri in this)
			{
				if (ri.Item.Equals(item))
					return (true);
			}

			return (false);
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
				var sorted = new List<RecentItem<T>>(this);

				sorted.Sort();

				return (Remove(sorted.Last()));
			}

			return (false);
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

			// If there are indeed duplicates, take the long way to remove the recent ones...
			while (this.Distinct().ToList().Count < Count)
			{
				var duplicates = new List<RecentItem<T>>(Count); // Preset the required capacity to improve memory management.

				// Traverse the collection and search for duplicates:
				for (int outer = 0; outer < (Count - 1); outer++)
				{
					for (int inner = (outer + 1); inner < Count; inner++)
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
				foreach (var ri in duplicates)
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
			var invalids = new List<RecentItem<T>>(Count); // Preset the required capacity to improve memory management.

			foreach (var ri in this)
			{
				if (!ri.IsValid)
					invalids.Add(ri);
			}

			foreach (var ri in invalids)
			{
				Remove(ri);
			}
		}

		/// <summary>
		/// Updates this collection from the given array.
		/// </summary>
		/// <returns>
		/// Returns whether collection has changed.
		/// </returns>
		public virtual bool UpdateFrom(IEnumerable<T> items)
		{
			bool hasChanged = false;

			foreach (T item in items)
			{
				if (!Contains(item))
				{
					Add(new RecentItem<T>(item));
					hasChanged = true;
				}
			}

			return (hasChanged);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
