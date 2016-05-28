//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using MKY.Collections.Generic;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Implements a collection of display elements.
	/// </summary>
	/// <remarks>
	/// This calls inherits <see cref="T:List`"/>. However, it only overrides functions required
	/// for YAT use cases. It is only allowed to add to the list, removing items results in an
	/// undefined behavior.
	/// </remarks>
	[Serializable]
	public class DisplayElementCollection : List<DisplayElement>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>
		/// Can be use to preset the initial capacity of collections. The value reflects typcial
		/// maximum settings in case of string radix:
		/// date, sep, time, sep, port, sep, direction, sep, data, eol, sep, length, linebreak = 13
		/// </remarks>
		public const int TypicalNumberOfElementsPerLine = 16; // Use 16 = on of the 'normal' values.

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int dataCount; // = 0;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayElementCollection()
		{
		}

		/// <summary></summary>
		public DisplayElementCollection(int elementCapacity)
			: base(elementCapacity)
		{
		}

		/// <summary></summary>
		public DisplayElementCollection(DisplayElementCollection collection)
		{
			foreach (DisplayElement de in collection) // Clone the whole collection.
				Add(de.Clone());
		}

		/// <summary></summary>
		public DisplayElementCollection(DisplayElement displayElement)
		{
			Add(displayElement.Clone()); // Clone the element.
		}

#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~DisplayElementCollection()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

#endif // DEBUG

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
			get { return (this.dataCount); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public new void Add(DisplayElement item)
		{
			if (Count <= 0)
			{
				base.Add(item);
			}
			else
			{
				// For performance reasons, append the item to the last item if possible:
				int lastIndex = Count - 1;
				if (this[lastIndex].AcceptsAppendOf(item))
					this[lastIndex].Append(item);
				else
					base.Add(item);
			}

			this.dataCount += item.DataCount;
		}

		/// <remarks>
		/// Required because <see cref="T:List`1.AddRange"/> doesn't call <see cref="Add"/>
		/// method above because it is 'new'. Call to <see cref="Add"/> method above is
		/// required to properly perform data counting.
		/// </remarks>
		public new void AddRange(IEnumerable<DisplayElement> collection)
		{
			foreach (DisplayElement item in collection)
				Add(item);
		}

		/// <summary></summary>
		public new void RemoveAt(int index)
		{
			if (this[index].IsData)
				this.dataCount -= this[index].DataCount;

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
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		public virtual DisplayElementCollection Clone()
		{
			DisplayElementCollection c = new DisplayElementCollection(this.Capacity); // Preset the required capactiy to improve memory management.

			foreach (DisplayElement de in this) // Clone the whole collection.
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
			return (indent + "> ElementCount: " +       Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
					indent + "> DataCount: " + this.dataCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
					indent + "> Elements: " + Environment.NewLine + ElementsToString(indent + "   "));
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
				sb.Append(indent + "> DisplayElement#" + (i++) + ":" + Environment.NewLine);
				sb.Append(de.ToString(indent + "   "));
			}

			if (i == 0)
				sb.AppendLine(indent + "<NONE>");

			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual byte[] ElementsToOrigin()
		{
			List<byte> l = new List<byte>(this.DataCount); // Preset the initial capactiy to improve memory management.

			foreach (DisplayElement de in this)
			{
				foreach (Pair<byte[], string> p in de.Origin)
					l.AddRange(p.Value1);
			}

			return (l.ToArray());
		}

		#endregion

		#endregion
	}

	/// <summary>
	/// Implements a part of a display line containing a list of display elements.
	/// </summary>
	/// <remarks>
	/// This calls inherits <see cref="T:List`"/>. However, it only overrides functions required
	/// for YAT use cases. It is only allowed to add to the list, removing items results in an
	/// undefined behavior.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A display line part is a collection of multiple elements, but not a collection of display lines as a name ending in 'Collection' would suggest.")]
	[Serializable]
	public class DisplayLinePart : DisplayElementCollection
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

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		public new DisplayLine Clone()
		{
			DisplayLine dl = new DisplayLine(this.Capacity); // Preset the required capactiy to improve memory management.

			foreach (DisplayElement de in this) // Clone the whole collection.
				dl.Add(de.Clone());

			return (dl);
		}

		#endregion
	}

	/// <summary>
	/// Implements a display line containing a list of display elements.
	/// </summary>
	/// <remarks>
	/// This calls inherits <see cref="T:List`"/>. However, it only overrides functions required
	/// for YAT use cases. It is only allowed to add to the list, removing items results in an
	/// undefined behavior.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A display line is a collection of multiple elements, but not a collection of display lines as a name ending in 'Collection' would suggest.")]
	[Serializable]
	public class DisplayLine : DisplayLinePart
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
