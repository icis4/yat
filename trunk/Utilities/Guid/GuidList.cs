using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Guid
{
	/// <summary>
	/// List with additional methods to handle items providing a <see cref="Guid"/>
	/// </summary>
	/// <typeparam name="T">Type that implements <see cref="IGuidProvider"/></typeparam>
	public class GuidList<T> : List<T>
		where T : IGuidProvider
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public GuidList()
			: base()
		{
		}

		/// <summary></summary>
		public GuidList(IEnumerable<T> collection)
			: base(collection)
		{
		}

		/// <summary></summary>
		public GuidList(int capacity)
			: base(capacity)
		{
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Add or replaces the item that has the same <see cref="Guid"/> as item.
		/// </summary>
		public void AddOrReplaceGuid(T item)
		{
			GuidList<T> clone = new GuidList<T>(this);

			for (int i = 0; i < clone.Count; i++)
			{
				if (this[i].Guid.Equals(item.Guid))
				{
					this[i] = item;
					return;
				}
			}

			// add if not contained yet
			Add(item);
		}

		/// <summary>
		/// Removes all items that have the specified GUID.
		/// </summary>
		public void RemoveGuid(System.Guid guid)
		{
			GuidList<T> obsoleteItems = new GuidList<T>();

			foreach (T item in this)
			{
				if (item.Guid == guid)
					obsoleteItems.Add(item);
			}

			foreach (T item in obsoleteItems)
			{
				Remove(item);
			}
		}

		#endregion
	}
}
