//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Recent
{
	/// <summary>
	/// Collection for recent items like recent files, provides methods to handle the recent items.
	/// </summary>
	public class RecentItemCollection<T> : List<RecentItem<T>>
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
		public RecentItemCollection(IEnumerable<RecentItem<T>> collection)
			: base(collection)
		{
		}

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Inserts the recent item at the beginning of the collection (least recent) and remove the
		/// most recent item if the collection already contains <see cref="T:RecentItemCollection`1.Capacity"/> items.
		/// </summary>
		public virtual void ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(RecentItem<T> item)
		{
			if (Contains(item))
			{
				Remove(item);
			}
			else
			{
				while (Count >= Capacity)
					RemoveMostRecent();
			}
			Insert(0, item);
		}

		/// <summary>
		/// Remove the most recent item if the collection already contains <see cref="T:RecentItemCollection`1.Capacity"/> items.
		/// </summary>
		/// <returns>
		/// true if an item is successfully removed; otherwise, false. This method also returns
		/// false if no item was not found in the collection.
		/// </returns>
		public virtual bool RemoveMostRecent()
		{
			List<RecentItem<T>> sorted = new List<RecentItem<T>>(this);
			sorted.Sort();
			if (sorted.Count > 0)
				return (Remove(sorted[sorted.Count - 1])); // remove last item in the collection
			else
				return (false);
		}

		/// <summary>
		/// Validates all recent items, invalid items are removed from the collection
		/// </summary>
		public virtual void ValidateAll()
		{
			List<RecentItem<T>> invalidItems = new List<RecentItem<T>>();
			foreach (RecentItem<T> ri in this)
			{
				if (!ri.IsValid)
					invalidItems.Add(ri);
			}
			foreach (RecentItem<T> ri in invalidItems)
			{
				this.Remove(ri);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
