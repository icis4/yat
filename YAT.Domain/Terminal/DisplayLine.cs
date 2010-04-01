//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Implements a collection of display elements.
	/// </summary>
	/// <remarks>
	/// This calls inherits <see cref="T:List`"/>. However, it only overrides functions required
	/// for YAT use cases. It is only allowed to add to the list, removing items results in an
	/// undefined behaviour.
	/// </remarks>
	[Serializable]
	public class DisplayElementCollection : List<DisplayElement>
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int _dataCount;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayElementCollection()
		{
			_dataCount = 0;
		}

		/// <summary></summary>
		public DisplayElementCollection(int elementCapacity)
			: base(elementCapacity)
		{
			_dataCount = 0;
		}

		/// <summary></summary>
		public DisplayElementCollection(DisplayElementCollection collection)
		{
			_dataCount = 0;
			foreach (DisplayElement item in collection)
				Add(item);
		}

		/// <summary></summary>
		public DisplayElementCollection(DisplayElement displayElement)
		{
			_dataCount = 0;
			Add(displayElement);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Returns number of data elements within repository.
		/// </summary>
		public virtual int DataCount
		{
			get { return (_dataCount); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Item must be cloned to prevent unexpected behaviour of the reference type.
		/// </remarks>
		public new void Add(DisplayElement item)
		{
			if (Count <= 0)
			{
				base.Add(item.Clone());
			}
			else
			{
				// For performance reasons, append the item to the last item if possible.
				int lastIndex = Count - 1;
				if (this[lastIndex].IsSameKindAs(item))
					this[lastIndex].Append(item.Clone());
				else
					base.Add(item.Clone());
			}
			_dataCount += item.DataCount;
		}

		/// <summary></summary>
		public new void AddRange(IEnumerable<DisplayElement> collection)
		{
			// \fixme 2010-04-01 / mky
			// Weird InvalidOperationException when receiving large chunks of data.
			try
			{
				foreach (DisplayElement item in collection)
					Add(item);
			}
			catch (Exception ex)
			{
				MKY.Utilities.Diagnostics.XDebug.WriteException(this, ex);
				System.Diagnostics.Debug.WriteLine(collection.ToString());
			}
		}

		/// <summary></summary>
		public new void RemoveAt(int index)
		{
			if (this[index].IsData)
				_dataCount -= this[index].DataCount;

			base.RemoveAt(index);
		}

		/// <summary></summary>
		public virtual void RemoveAtEnd()
		{
			RemoveAt(Count - 1);
		}

		/// <summary></summary>
		public virtual void RemoveAtEnd(int count)
		{
			for (int i = 0; i < count; i++)
				RemoveAtEnd();
		}

		/// <summary></summary>
		public new void RemoveRange(int index, int count)
		{
			for (int i = 0; i < count; i++)
				RemoveAt(index + i);
		}

		/// <summary>
		/// Creates a shallow copy of the collection.
		/// </summary>
		/// <returns>A shallow copy of the collection.</returns>
		public virtual DisplayElementCollection Clone()
		{
			DisplayElementCollection c = new DisplayElementCollection();

			foreach (DisplayElement de in this)
				c.Add(de.Clone());

			return (c);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Standard ToString method returning the element contents only.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (DisplayElement de in this)
				sb.Append(de.ToString());

			return (sb.ToString());
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Extended ToString method which can be used for trace/debug.
		/// </summary>
		public virtual string ToString(string indent)
		{
			return (indent + "- ElementCount: " + Count.ToString("D") + Environment.NewLine +
					indent + "- DataCount: " + _dataCount.ToString("D") + Environment.NewLine +
					indent + "- Elements: " + Environment.NewLine + ElementsToString(indent + "--"));
		}

		/// <summary></summary>
		public virtual string ElementsToString()
		{
			return (ElementsToString(""));
		}

		/// <summary></summary>
		public virtual string ElementsToString(string indent)
		{
			StringBuilder sb = new StringBuilder();
			int i = 0;
			foreach (DisplayElement de in this)
			{
				i++;
				sb.Append(indent + "DisplayElement " + i + ":" + Environment.NewLine);
				sb.Append(de.ToString(indent + "--"));
			}
			return (sb.ToString());
		}

		#endregion

		#endregion
	}

	/// <summary>
	/// Implements a display line containing a list of display elements.
	/// </summary>
	/// <remarks>
	/// This calls inherits <see cref="T:List`"/>. However, it only overrides functions required
	/// for YAT use cases. It is only allowed to add to the list, removing items results in an
	/// undefined behaviour.
	/// </remarks>
	[Serializable]
	public class DisplayLine : DisplayElementCollection
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayLine()
		{
		}

		/// <summary></summary>
		public DisplayLine(int elementCapacity)
			: base(elementCapacity)
		{
		}

		/// <summary></summary>
		public DisplayLine(DisplayElementCollection collection)
			: base(collection)
		{
		}

		/// <summary></summary>
		public DisplayLine(DisplayElement displayElement)
			: base(displayElement)
		{
		}

		#endregion

		#region DisplayElementCollection Members
		//==========================================================================================
		// DisplayElementCollection Members
		//==========================================================================================

		/// <summary>
		/// Creates a shallow copy of the collection.
		/// </summary>
		/// <returns>A shallow copy of the collection.</returns>
		public new DisplayLine Clone()
		{
			DisplayLine dl = new DisplayLine();

			foreach (DisplayElement de in this)
				dl.Add(de.Clone());

			return (dl);
		}

		#endregion
	}

	/// <summary>
	/// Implements a part of a display line containing a list of display elements.
	/// </summary>
	/// <remarks>
	/// This calls inherits <see cref="T:List`"/>. However, it only overrides functions required
	/// for YAT use cases. It is only allowed to add to the list, removing items results in an
	/// undefined behaviour.
	/// </remarks>
	[Serializable]
	public class DisplayLinePart : DisplayLine
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayLinePart()
		{
		}

		/// <summary></summary>
		public DisplayLinePart(int elementCapacity)
			: base(elementCapacity)
		{
		}

		/// <summary></summary>
		public DisplayLinePart(DisplayElementCollection collection)
			: base(collection)
		{
		}

		/// <summary></summary>
		public DisplayLinePart(DisplayElement displayElement)
			: base(displayElement)
		{
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
