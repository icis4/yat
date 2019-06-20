﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
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
		/// Can be use to preset the initial capacity of collections. The value reflects typical
		/// maximum settings in case of string radix:
		/// date, sep, time, sep, port, sep, direction, sep, content, eol, sep, length, linebreak = 13
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'linebreak' is a YAT display element.")]
		public const int TypicalNumberOfElementsPerLine = 16; // Use 16 = on of the 'normal' values.

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int byteCount; // = 0;

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
			foreach (var de in collection) // Clone the whole collection.
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
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
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
		/// Returns number of raw byte content within collection.
		/// </summary>
		/// <remarks>
		/// Note that this value reflects the byte count of the elements contained in the collection,
		/// i.e. the byte count of the elements shown. The value thus not necessarily reflects the
		/// total byte count of a sent or received sequence.
		/// </remarks>
		public virtual int ByteCount
		{
			get { return (this.byteCount); }
		}

		/// <summary>
		/// Appends and returns the text of all display elements.
		/// </summary>
		public string Text
		{
			get
			{
				var sb = new StringBuilder();
				foreach (var de in this)
					sb.Append(de.Text);

				return (sb.ToString());
			}
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

			this.byteCount += item.ByteCount;
		}

		/// <remarks>
		/// Required because <see cref="T:List`1.AddRange"/> doesn't call <see cref="Add"/>
		/// method above because it is 'new'. Call to <see cref="Add"/> method above is
		/// required to properly perform content counting.
		/// </remarks>
		public new void AddRange(IEnumerable<DisplayElement> collection)
		{
			foreach (var item in collection)
				Add(item);
		}

		/// <summary></summary>
		public new void RemoveAt(int index)
		{
			if (this[index].IsContent)
				this.byteCount -= this[index].ByteCount;

			base.RemoveAt(index);
		}

		/// <summary></summary>
		public virtual void RemoveOneAtEnd()
		{
			RemoveAt(Count - 1);
		}

		/// <summary></summary>
		public virtual void RemoveRangeAtEnd(int count)
		{
			RemoveRange(Count - count, count);
		}

		/// <summary>
		/// Removes all elements at the end until (and including!) an element of the given type is found.
		/// </summary>
		public virtual void RemoveAtEndUntilIncluding(Type type)
		{
			while (Count > 0)
			{
				bool typeFound = (this[Count - 1].GetType() == type);

				RemoveOneAtEnd();

				if (typeFound)
					break;
			}
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
			var c = new DisplayElementCollection(this.Capacity); // Preset the required capacity to improve memory management.

			foreach (var de in this) // Clone the whole collection.
				c.Add(de.Clone());

			return (c);
		}

		/// <summary></summary>
		public virtual byte[] ElementsToOrigin()
		{
			var l = new List<byte>(this.ByteCount); // Preset the initial capacity to improve memory management.

			foreach (var de in this)
			{
				if (de.Origin != null) // Foreach element where origin exists.
				{
					foreach (Pair<byte[], string> p in de.Origin)
						l.AddRange(p.Value1);
				}
			}

			return (l.ToArray());
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (var de in this)
				sb.Append(de.ToString());

			return (sb.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		public virtual string ToString(string indent)
		{
			return (indent + "> ElementCount: " +       Count.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
					indent + "> ByteCount: " + this.byteCount.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
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
			var sb = new StringBuilder();

			int i = 0;
			foreach (var de in this)
			{
				sb.Append(indent + "> DisplayElement#" + (i++) + ":" + Environment.NewLine);
				sb.Append(de.ToString(indent + "   "));
			}

			if (i == 0)
				sb.AppendLine(indent + "<NONE>");

			return (sb.ToString());
		}

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
			var dl = new DisplayLine(this.Capacity); // Preset the required capacity to improve memory management.

			foreach (var de in this) // Clone the whole collection.
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