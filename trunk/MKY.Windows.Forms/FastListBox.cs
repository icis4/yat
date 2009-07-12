//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	#region VScrollMode
	//==============================================================================================
	// VScrollMode
	//==============================================================================================

	/// <summary></summary>
	public enum VScrollMode
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		AnchorBegin,
		/// <summary></summary>
		AnchorEnd,
	}

	#endregion

	/// <summary>
	/// Fast implementation of a list box. <see cref="FastListBox"/> is rather slow
	/// if there are many consequent updates/adds.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public class FastListBox : ListBox
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private class ItemData
		{
			private object _item = null;
			private bool _hasChanged = false;

			public ItemData(object item)
			{
				_item = item;
			}

			public ItemData(ItemData itemData)
			{
				_item = itemData._item;
				_hasChanged = itemData._hasChanged;
			}

			public object Item
			{
				get { return (_item); }
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const VScrollMode _VScrollModeDefault = VScrollMode.None;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ObjectCollection _items;
		private bool _updateIsSuspended = false;
		private VScrollMode _vScrollMode = _VScrollModeDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public FastListBox()
		{
			_items = new ObjectCollection(this);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>The scroll mode.</summary>
		[Category("Behaviour")]
		[Description("The vertical scroll mode.")]
		[DefaultValue(_VScrollModeDefault)]
		public VScrollMode VScrollMode
		{
			get { return (_vScrollMode); }
			set
			{
				_vScrollMode = value;
				VScroll();
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected bool UpdateIsSuspended
		{
			get { return (_updateIsSuspended); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Maintains performance while items are added to the <see cref="FastListBox"/> one at a
		/// time by preventing the control from drawing until the <see cref="FastListBox.EndUpdate()"/>
		/// method is called.
		/// </summary>
		new public void BeginUpdate()
		{
			_updateIsSuspended = true;
		}

		/// <summary>
		/// Resumes painting the <see cref="FastListBox"/> control after painting is suspended by
		/// the <see cref="FastListBox.BeginUpdate()"/> method.
		/// </summary>
		new public void EndUpdate()
		{
			_updateIsSuspended = false;
		}

		/// <summary></summary>
		public void Add()
		{
			//
		}

		/// <summary></summary>
		public void RefreshAt(int index)
		{
		}

		/// <summary></summary>
		public void RefreshRangeFrom(int index)
		{
		}

		/// <summary></summary>
		public void ShiftUp()
		{
		}

		/// <summary></summary>
		public void ShiftRangeUp(int count)
		{
		}

		/// <summary></summary>
		protected void VScroll()
		{
			//
		}

		#endregion

		#region Form/Control Special Keys
		//==========================================================================================
		// Form/Control Special Keys
		//==========================================================================================

		#endregion

		#region Form/Control Event Handlers
		//==========================================================================================
		// Form/Control Event Handlers
		//==========================================================================================

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		/// <summary></summary>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
		}

		/// <summary></summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		/// <summary></summary>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			//base.OnDrawItem(e);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		#endregion

		#region ObjectCollection
		//==========================================================================================
		// ObjectCollection
		//==========================================================================================

		/// <summary>
		/// Represents the collection of items in a <see cref="FastListBox"/>.
		/// </summary>
		[ListBindable(false)]
		new public class ObjectCollection : IList, ICollection, IEnumerable
		{
			#region Fields
			//======================================================================================
			// Fields
			//======================================================================================

			private FastListBox _owner;
			private List<ItemData> _items;

			#endregion

			#region Object Lifetime
			//======================================================================================
			// Object Lifetime
			//======================================================================================

			/// <summary>
			/// Initializes a new instance of <see cref="ObjectCollection"/>.
			/// </summary>
			/// <param name="owner">The <see cref="FastListBox"/> that owns the collection.</param>
			public ObjectCollection(FastListBox owner)
			{
				_owner = owner;
				_items = new List<ItemData>();
				_owner.Refresh();
			}

			/// <summary>
			/// Initializes a new instance of <see cref="ObjectCollection"/>based on another <see cref="ObjectCollection"/>.
			/// </summary>
			/// <param name="owner">The <see cref="FastListBox"/> that owns the collection.</param>
			/// <param name="items">A <see cref="ObjectCollection"/> from which the contents are copied to the collection.</param>
			public ObjectCollection(FastListBox owner, ObjectCollection items)
			{
				_owner = owner;
				_items = new List<ItemData>(items.Capacity);
				_items.AddRange(items._items);
				_owner.Refresh();
			}

			/// <summary>
			/// Initializes a new instance of <see cref="ObjectCollection"/> containing an array of objects.
			/// </summary>
			/// <param name="owner">The <see cref="FastListBox"/> that owns the collection.</param>
			/// <param name="items">An array of objects to add to the collection.</param>
			public ObjectCollection(FastListBox owner, object[] items)
			{
				_owner = owner;

				_items = new List<ItemData>();
				foreach (object obj in items)
					_items.Add(new ItemData(obj));

				_owner.Refresh();
			}

			#endregion

			#region Properties
			//======================================================================================
			// Properties
			//======================================================================================

			/// <summary>
			/// Gets the number of items in the collection.
			/// </summary>
			/// <value>The number of items in the collection.</value>
			public int Count
			{
				get { return (_items.Count); }
			}

			/// <summary>
			/// Gets or sets the capacity of items in the collection. Opposed to
			/// <see cref="ListBox.ObjectCollection"/>, this property doesn't automatically
			/// grow when adding items. Instead, excess items are removed before items are added.
			/// </summary>
			/// <value>The capacity of items in the collection.</value>
			/// <exception cref="ArgumentOutOfRangeException">Capacity is below 1.</exception>
			public int Capacity
			{
				get { return (_items.Capacity); }
				set
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("Capacity", value, "Capacity must be at least 1"));

					// Remove items if capacity exceeded
					int excess = Count - value;
					if (excess > 0)
						RemoveRangeAtBegin(excess);

					_items.Capacity = value;
				}
			}

			/// <summary>
			/// Gets a value indicating whether the collection has a fixed size.
			/// </summary>
			/// <value>true if the collection has a fixed size; otherwise, false.</value>
			public bool IsFixedSize
			{
				get { return (false); }
			}

			/// <summary>
			/// Gets a value indicating whether the collection is read-only.
			/// </summary>
			/// <value>true if the collection is read-only; otherwise, false.</value>
			public bool IsReadOnly
			{
				get { return (false); }
			}

			/// <summary>
			/// Gets a value indicating whether access to the collection is synchronized (thread safe).
			/// </summary>
			/// <value>true if access to the collection is synchronized (thread safe); otherwise, false.</value>
			public bool IsSynchronized
			{
				get { return (false); }
			}

			/// <summary>
			/// Gets an object that can be used to synchronize access to the collection.
			/// </summary>
			/// <value>An object that can be used to synchronize access to the collection.</value>
			public object SyncRoot
			{
				get { return (_items); }
			}

			/// <summary>
			/// Gets or sets the item at the specified index within the collection.
			/// </summary>
			/// <param name="index">The index of the item in the collection to get or set.</param>
			/// <returns>An object representing the item located at the specified index within the collection.</returns>
			/// <exception cref="ArgumentOutOfRangeException">
			/// The index parameter is less than zero or greater than or equal to the value of the
			/// <see cref="ObjectCollection.Count"/> property of the <see cref="ObjectCollection"/> class.
			/// </exception>
			[Browsable(false)]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public virtual object this[int index]
			{
				get { return (_items[index].Item); }
				set
				{
					_items[index] = new ItemData(value);
					_owner.RefreshItem(index);
				}
			}

			#endregion

			#region Methods
			//======================================================================================
			// Methods
			//======================================================================================

			/// <summary>
			/// Adds an item to the list of items for a <see cref="FastListBox"/>.
			/// </summary>
			/// <param name="item"> An object representing the item to add to the collection.</param>
			/// <returns>
			/// The zero-based index of the item in the collection, or -1 if
			/// <see cref="FastListBox.BeginUpdate()"/> has been called.
			/// </returns>
			/// <exception cref="SystemException">
			/// There is insufficient space available to add the new item to the list.
			/// </exception>
			public int Add(object item)
			{
				// Remove item if capacity exceeded
				if (Count >= Capacity)
					RemoveAt(0);

				_items.Add(new ItemData(item));
				_owner.RefreshAt(_items.Count - 1);

				if (!_owner.UpdateIsSuspended)
					return (_items.Count - 1);
				else
					return (-1);
			}

			/// <summary>
			/// Adds the items of an existing <see cref="ObjectCollection"/> to the list of items in a <see cref="FastListBox"/>.
			/// </summary>
			/// <param name="items">A <see cref="ObjectCollection"/> to load into the collection.</param>
			public void AddRange(ObjectCollection items)
			{
				// Remove items if capacity exceeded
				int excess = (Count + items.Count) - Capacity;
				if (excess > 0)
					RemoveRangeAtBegin(excess);

				// Only add the items the capacity allows
				int first = Count - 1;

				int remainingCapacity = Capacity - Count;
				for (int i = items.Count - remainingCapacity - 1; i < items.Count; i++)
					_items.Add(new ItemData(items[i]));

				_owner.RefreshRangeFrom(first);
			}

			/// <summary>
			/// Adds an array of items to the list of items for a <see cref="FastListBox"/>.
			/// </summary>
			/// <param name="items">An array of objects to add to the list.</param>
			public void AddRange(object[] items)
			{
				// Remove items if capacity exceeded
				int excess = (Count + items.Length) - Capacity;
				if (excess > 0)
					RemoveRangeAtBegin(excess);

				// Only add the items the capacity allows
				int first = Count - 1;

				int remainingCapacity = Capacity - Count;
				for (int i = items.Length - remainingCapacity - 1; i < items.Length; i++)
					_items.Add(new ItemData(items[i]));

				_owner.RefreshRangeFrom(first);
			}

			/// <summary>
			/// Removes all items from the collection.
			/// </summary>
			public virtual void Clear()
			{
				_items.Clear();
				_owner.Refresh();
			}

			/// <summary>
			/// Determines whether the specified item is located within the collection.
			/// </summary>
			/// <param name="value">An object representing the item to locate in the collection.</param>
			/// <returns>true if the item is located within the collection; otherwise, false.</returns>
			public bool Contains(object value)
			{
				foreach (ItemData item in _items)
				{
					if (ReferenceEquals(item.Item, value))
						return (true);
				}
				return (false);
			}

			/// <summary>
			/// Copies the entire collection into an existing array of objects at a specified location within the array.
			/// </summary>
			/// <param name="array">The object array in which the items from the collection are copied to.</param>
			/// <param name="index">The location within the destination array to copy the items from the collection to.</param>
			public void CopyTo(Array array, int index)
			{
				for (int i = index, j = 0; i < array.Length; i++, j++)
					array.SetValue(_items[j].Item, i);
			}

			/// <summary>
			/// Returns an enumerator to use to iterate through the item collection.
			/// </summary>
			/// <returns>An <see cref="System.Collections.IEnumerator"/> that represents the item collection.</returns>
			public IEnumerator GetEnumerator()
			{
				return (_items.GetEnumerator());
			}

			/// <summary>
			/// Returns the index within the collection of the specified item.
			/// </summary>
			/// <param name="value">An object representing the item to locate in the collection.</param>
			/// <returns>The zero-based index where the item is located within the collection; otherwise, negative one (-1).</returns>
			/// <exception cref="System.ArgumentNullException">The value parameter is null.</exception>
			public int IndexOf(object value)
			{
				int i = 0;
				foreach (ItemData item in _items)
				{
					if (ReferenceEquals(item.Item, value))
						return (0);

					i++;
				}
				return (-1);
			}

			/// <summary>
			/// Inserts an item into the list box at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index location where the item is inserted.</param>
			/// <param name="item">An object representing the item to insert.</param>
			/// <exception cref="System.ArgumentOutOfRangeException">
			/// The index parameter is less than zero or greater than value of the
			/// <see cref="ObjectCollection.Count"/> property of the <see cref="FastListBox.ObjectCollection"/> class.
			/// </exception>
			public void Insert(int index, object item)
			{
				throw (new NotSupportedException("Insert() is not support for FastListBox"));
			}

			/// <summary>
			/// Removes the specified object from the collection.
			/// </summary>
			/// <param name="value">An object representing the item to remove from the collection.</param>
			public void Remove(object value)
			{
				throw (new NotSupportedException("Remove() is not support for FastListBox"));
			}

			/// <summary>
			/// Removes the item at the specified index within the collection.
			/// </summary>
			/// <param name="index">The zero-based index of the item to remove.</param>
			/// <exception cref="System.ArgumentOutOfRangeException">
			/// The index parameter is less than zero or greater than or equal to the value of the
			/// <see cref="ObjectCollection.Count"/> property of the <see cref="FastListBox.ObjectCollection"/> class.
			/// </exception>
			public void RemoveAt(int index)
			{
				throw (new NotSupportedException("RemoveAt() is not support for FastListBox"));
			}

			/// <summary>
			/// Removes the item at the beginning of the collection.
			/// </summary>
			public void RemoveAtBegin()
			{
				_items.RemoveAt(0);
				_owner.ShiftUp();
			}

			/// <summary>
			/// Removes the items at the beginning of the collection.
			/// </summary>
			/// <param name="count">The number of items to remove.</param>
			public void RemoveRangeAtBegin(int count)
			{
				_items.RemoveRange(0, count);
				_owner.ShiftRangeUp(count);
			}

			#endregion
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
