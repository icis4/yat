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
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public virtual void Add(T item)
		{
			base.Add(new RecentItem<T>(item));
		}

		/// <summary>
		/// Determines whether an element is in the collections.
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
		/// Inserts the recent item at the beginning of the collection (least recent). The most
		/// recent item will be removed in case the collection already contains <see cref="T:RecentItemCollection`1.Capacity"/> items.
		/// </summary>
		public virtual void ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(T item)
		{
			ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(new RecentItem<T>(item));
		}

		/// <summary>
		/// Inserts the recent item at the beginning of the collection (least recent). The most
		/// recent item will be removed in case the collection already contains <see cref="T:RecentItemCollection`1.Capacity"/> items.
		/// </summary>
		public virtual void ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(RecentItem<T> item)
		{
			// Remove all equal items, as they are becoming less recent than the new item:
			RemoveAll(li => (li == item));

			// Ensure there is space for the item to be inserted:
			while ((Count > 0) && (Count >= Capacity))
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
				var duplicates = new List<RecentItem<T>>(Count); // Preset the initial capacity to improve memory management.

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
			var invalids = new List<RecentItem<T>>(Count); // Preset the initial capacity to improve memory management.

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
		/// Copies the items of this collection to a new array.
		/// </summary>
		public virtual T[] ToItemArray()
		{
			var items = new List<T>(Count); // Preset the initial capacity to improve memory management.

			foreach (var ri in this)
				items.Add(ri.Item);

			return (items.ToArray());
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
					ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(new RecentItem<T>(item));
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
