using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.Utilities.Recent
{
	/// <summary>
	/// Collection for recent items like recent files, provides methods to handle the recent items.
	/// </summary>
	public class RecentItemCollection<T> : List<RecentItem<T>>
	{
		private int _maximumCapacity = 8;

		/// <summary></summary>
		public RecentItemCollection()
			: base()
		{
		}

		/// <summary></summary>
		public RecentItemCollection(int maximumCapacity)
			: base()
		{
			_maximumCapacity = maximumCapacity;
		}

		/// <summary></summary>
		public RecentItemCollection(IEnumerable<RecentItem<T>> collection)
			: base(collection)
		{
			if (collection is RecentItemCollection<T>)
			{
				RecentItemCollection<T> casted = (RecentItemCollection<T>)collection;
				_maximumCapacity = casted._maximumCapacity;
			}
		}

		/// <summary></summary>
		public RecentItemCollection(int capacity, int maximumCapacity)
			: base(capacity)
		{
			_maximumCapacity = maximumCapacity;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// The maximum capacity of the collection.
		/// </summary>
		public int MaximumCapacity
		{
			get { return (_maximumCapacity); }
			set { _maximumCapacity = value;  }
		}

		#endregion

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Inserts the recent item at the beginning of the collection (least recent) and remove the
		/// most recent item if the collection already contains <see cref="MaximumCapacity"/> items.
		/// </summary>
		public virtual void ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(RecentItem<T> item)
		{
			if (Contains(item))
			{
				Remove(item);
			}
			else
			{
				while (Count >= MaximumCapacity)
					RemoveMostRecent();
			}
			Insert(0, item);
		}

		/// <summary>
		/// Remove the most recent item if the collection already contains
		/// <see cref="MaximumCapacity"/> items.
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
