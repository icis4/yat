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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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

using MKY;
using MKY.Collections.Generic;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Implements a collection of display elements representing a part of a line.
	/// </summary>
	/// <remarks>
	/// This class inherits from <see cref="T:List`"/>. However, it only overrides functions
	/// required for YAT use cases. It is only allowed to add to the list, removing items results
	/// in an undefined behavior.
	/// </remarks>
	[Serializable]
	public class DisplayElementCollection : List<DisplayElement>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>
		/// Can be used to preset the initial capacity of display element collections. A value of
		/// 18 would reflect the maximum elements in case of string radix and EOL shown:
		/// start, stamp, sep, span, sep, delta, sep, port, sep, direction, sep, content, eol, sep, length, sep, duration, linebreak
		///
		/// However, this is not a typical use case, thus using a reduced value of 16 which is the
		/// most typical 2^n value.
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'linebreak' is a YAT display element.")]
		public const int TypicalNumberOfElementsPerLine = 16;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int charCount; // = 0;
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
		public DisplayElementCollection(DisplayElement element)
		{
			Add(element.Clone()); // Clone the element to ensure decoupling.
		}

		/// <summary></summary>
		public DisplayElementCollection(IEnumerable<DisplayElement> collection)
		{
			foreach (var de in collection) // Clone the whole collection to ensure decoupling.
				Add(de.Clone());
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
		/// The time stamp of the elements in the collection.
		/// </summary>
		/// <remarks>
		/// The value corresponds to <see cref="DisplayElement.TimeStamp"/> of the first element in
		/// the collection which has a value other than <see cref="DisplayElement.TimeStampDefault"/>;
		/// or <see cref="DisplayElement.TimeStampDefault"/>/> in case the collection is empty or no
		/// element has a value other than <see cref="DisplayElement.TimeStampDefault"/>.
		/// </remarks>
		public virtual DateTime TimeStamp
		{
			get
			{
				foreach (var de in this)
				{
					if (de.TimeStamp != DisplayElement.TimeStampDefault)
						return (de.TimeStamp);
				}

				return (DisplayElement.TimeStampDefault);
			}
		}

		/// <summary>
		/// The direction of the elements in the collection.
		/// </summary>
		/// <remarks>
		/// The value typically corresponds to <see cref="DisplayElement.Direction"/> of all
		/// elements in the collection, but may also be <see cref="Direction.Bidir"/> in case
		/// direction line break is disabled.
		/// </remarks>
		public virtual Direction Direction
		{
			get
			{
				bool txIsContained = false;
				bool rxIsContained = false;

				foreach (var de in this)
				{
					if (de.Direction == Direction.Tx) { txIsContained = true; }
					if (de.Direction == Direction.Rx) { rxIsContained = true; }
				}

				if (txIsContained)
				{
					if (!rxIsContained)
						return (Direction.Tx); // The typical case if Tx is contained.
					else
						return (Direction.Bidir);
				}

				if (rxIsContained)
				{
					if (!txIsContained)
						return (Direction.Rx); // The typical case if Rx is contained.
					else
						return (Direction.Bidir);
				}

				return (Direction.None);
			}
		}

		/// <summary>
		/// Appends and returns the text of all display elements.
		/// </summary>
		public virtual string Text
		{
			get
			{
				var sb = new StringBuilder();

				foreach (var de in this)
				{
					sb.Append(de.Text);
				}

				return (sb.ToString());
			}
		}

		/// <summary>
		/// The number of characters of the elements contained in the collection.
		/// </summary>
		/// <remarks>
		/// ASCII mnemonics (e.g. &lt;CR&gt;) are considered a single shown character.
		/// </remarks>
		public virtual int CharCount
		{
			get { return (this.charCount); }
		}

		/// <summary>
		/// The number of characters of the data content elements (i.e. without control
		/// data like <code>CRLF</code>) contained in the collection.
		/// </summary>
		public virtual int DataContentCharCount
		{
			get
			{
				int count = 0;

				foreach (var de in this)
				{
					if (de.IsDataContent)
						count += de.CharCount;
				}

				return (count);
			}
		}

		/// <summary>
		/// The number of characters of the control content elements (e.g. control
		/// data like <code>CRLF</code>) contained in the collection.
		/// </summary>
		public virtual int ControlContentCharCount
		{
			get
			{
				int count = 0;

				foreach (var de in this)
				{
					if (de.IsControlContent)
						count += de.CharCount;
				}

				return (count);
			}
		}

		/// <summary>
		/// The number of raw bytes of the elements contained in the collection.
		/// </summary>
		/// <remarks>
		/// Note that value reflects the byte count of the elements contained in the collection,
		/// i.e. the byte count of the elements shown. The value thus not necessarily reflects the
		/// total byte count of a sent or received sequence, a hidden EOL is e.g. not reflected.
		/// </remarks>
		public virtual int ByteCount
		{
			get { return (this.byteCount); }
		}

		/// <summary>
		/// Indicates whether any <see cref="DisplayElement"/> of this <see cref="DisplayElementCollection"/> is highlighted.
		/// </summary>
		public virtual bool Highlight
		{
			get
			{
				foreach (var de in this)
				{
					if (de.Highlight)
						return (true);
				}

				return (false);
			}

			set
			{
				foreach (var de in this)
				{
					de.Highlight = value;
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Required to properly perform content counting.
		/// </remarks>
		public new void Add(DisplayElement item)
		{
			if (Count <= 0)
			{
				base.Add(item);
			}
			else
			{
				// For performance reasons, append the item to the last item if possible:
				int lastIndex = (Count - 1);
				if (this[lastIndex].AcceptsAppendOf(item))
					this[lastIndex].Append(item);
				else
					base.Add(item);
			}

			this.charCount += item.CharCount;
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

		/// <remarks>
		/// Required to properly perform content counting.
		/// </remarks>
		public new void Insert(int index, DisplayElement item)
		{
			if (index <= 0)
			{
				base.Insert(index, item);
			}
			else
			{
				// For performance reasons, append the item to the previous item if possible:
				int previousIndex = (index - 1);
				if (this[previousIndex].AcceptsAppendOf(item))
					this[previousIndex].Append(item);
				else
					base.Insert(index, item);
			}

			this.charCount += item.CharCount;
			this.byteCount += item.ByteCount;
		}

		/// <remarks>
		/// Required because <see cref="T:List`1.InsertRange"/> doesn't call <see cref="Insert"/>
		/// method above because it is 'new'. Call to <see cref="Insert"/> method above is
		/// required to properly perform content counting.
		/// </remarks>
		public new void InsertRange(int index, IEnumerable<DisplayElement> collection)
		{
			foreach (var item in collection)
				Insert(index++, item);
		}

		/// <remarks>
		/// Required because <see cref="T:List`1.Remove"/> doesn't call <see cref="RemoveAt"/>
		/// method below because it is 'new'. Call to <see cref="RemoveAt"/> method below is
		/// required to properly perform content counting.
		/// </remarks>
		public new bool Remove(DisplayElement item)
		{
			var itemCharCount = item.CharCount;
			var itemByteCount = item.ByteCount;

			if (base.Remove(item))
			{
				this.charCount -= itemCharCount;
				this.byteCount -= itemByteCount;

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Removes the element at the specified index of the <see cref="DisplayElementCollection"/>.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <list type="table">
		/// <item><description><paramref name="index"/> is less than 0.</description></item>
		/// <item><description>-or-</description></item>
		/// <item><description><paramref name="index"/> is equal to or greater than <see cref="T:List`1.Count"/>.</description></item>
		/// </list>
		/// </exception>
		/// <remarks>
		/// Required to properly perform content counting.
		/// </remarks>
		public new void RemoveAt(int index)
		{
			this.charCount -= this[index].CharCount;
			this.byteCount -= this[index].ByteCount;

			base.RemoveAt(index);
		}

		/// <summary>
		/// Removes the last element of the <see cref="DisplayElementCollection"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The collection is empty.
		/// </exception>
		public virtual void RemoveLast()
		{
			if (Count == 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The collection is empty!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			RemoveAt(Count - 1);
		}

		/// <summary>
		/// Removes all elements at the end until (and including!) an element of the given type is found.
		/// </summary>
		public virtual void RemoveLastUntil(Type type)
		{
			while (Count > 0)
			{
				bool typeFound = (this[Count - 1].GetType() == type);

				RemoveLast();

				if (typeFound)
					break;
			}
		}

		/// <summary>
		/// Tries to remove the last "true" character from the <see cref="DisplayElementCollection"/>.
		/// </summary>
		/// <remarks>
		/// Needed to handle backspace, thus applies to data content only.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// The collection is empty.
		/// </exception>
		public virtual void RemoveLastDataContentChar()
		{
			if (Count == 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The collection is empty!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (CharCount == 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The collection contains no content character!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			for (int index = (Count - 1); index >= 0; index--)
			{
				var current = this[index];
				if (current.IsDataContent)        // Data content only, e.g. <BS> must not be applied to a <BEL>,
				{                                 // but has to be applied to a <TAB> if that is executed.
					if (current.CharCount < 1)
					{
						RemoveAt(index);          // A preceeding whitespace content has to be removed,
						continue;                 // but then continue searching for char.
					}
					else if (current.CharCount == 1)
					{
						RemoveAt(index);          // A single element can be removed,
						break;                    // done.
					}
					else if (current.CharCount > 1)
					{
						var formerCharCount = current.CharCount;
						var formerByteCount = current.ByteCount;

						current.RemoveLastChar(); // A single element can be removed,
						                        //// done after adjusting the counts.
						this.charCount -= (formerCharCount - current.CharCount);
						this.byteCount -= (formerByteCount - current.ByteCount);
						break;
					}
				}
			}
		}

		/// <remarks>
		/// Required because <see cref="T:List`1.RemoveRange"/> doesn't call <see cref="RemoveAt"/>
		/// method above because it is 'new'. Call to <see cref="RemoveAt"/> method above is
		/// required to properly perform content counting.
		/// </remarks>
		public new void RemoveRange(int index, int count)
		{
			for (int i = 0; i < count; i++)
				RemoveAt(index + i);
		}

		/// <remarks>
		/// Required because <see cref="T:List`1.RemoveAll"/> doesn't call <see cref="RemoveAt"/>
		/// method above because it is 'new'. Call to <see cref="RemoveAt"/> method above is
		/// required to properly perform content counting.
		/// </remarks>
		public new int RemoveAll(Predicate<DisplayElement> match)
		{
			int result = 0;

			int startIndex = 0;
			while (Count > 0)
			{
				var index = FindIndex(startIndex, match); // Not the ideal implementation, but considered good enough.
				if (index >= 0)
				{
					RemoveAt(index);
					result++;

					if (index < Count) // Continue at same index:
						startIndex = index;
					else // Was already at last index:
						break;
				}
				else // -1 indicates not found:
				{
					break;
				}
			}

			return (result);
		}

		/// <remarks>
		/// Required to properly perform content counting.
		/// </remarks>
		public new void Clear()
		{
			base.Clear();

			this.charCount = 0;
			this.byteCount = 0;
		}

		/// <summary>
		/// Replaces all occurrences of <see cref="DisplayElement.TimeStampInfo"/> with the given arguments.
		/// </summary>
		public virtual void ReplaceTimeStamp(DateTime timeStamp, string format, bool useUtc, string infoEnclosureLeft, string infoEnclosureRight)
		{
			foreach (var de in this)
			{
				var casted = (de as DisplayElement.TimeStampInfo);
				if (casted != null)
					casted.Change(timeStamp, format, useUtc, infoEnclosureLeft, infoEnclosureRight);
			}
		}

		/// <summary>
		/// Replaces all occurrences of <see cref="DisplayElement.TimeSpanInfo"/> with the given arguments.
		/// </summary>
		public virtual void ReplaceTimeSpan(TimeSpan timeSpan, string format, string infoEnclosureLeft, string infoEnclosureRight)
		{
			foreach (var de in this)
			{
				var casted = (de as DisplayElement.TimeSpanInfo);
				if (casted != null)
					casted.Change(timeSpan, format, infoEnclosureLeft, infoEnclosureRight);
			}
		}

		/// <summary>
		/// Replaces all occurrences of <see cref="DisplayElement.TimeDeltaInfo"/> with the given arguments.
		/// </summary>
		public virtual void ReplaceTimeDelta(TimeSpan timeSpan, string format, string infoEnclosureLeft, string infoEnclosureRight)
		{
			foreach (var de in this)
			{
				var casted = (de as DisplayElement.TimeDeltaInfo);
				if (casted != null)
					casted.Change(timeSpan, format, infoEnclosureLeft, infoEnclosureRight);
			}
		}

		/// <summary>
		/// Replaces all occurrences of <see cref="DisplayElement.DirectionInfo"/> with the given arguments.
		/// </summary>
		public virtual void ReplaceDirection(Direction direction, string infoEnclosureLeft, string infoEnclosureRight)
		{
			foreach (var de in this)
			{
				var casted = (de as DisplayElement.DirectionInfo);
				if (casted != null)
					casted.Change(direction, infoEnclosureLeft, infoEnclosureRight);
			}
		}

		/// <summary>
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		public virtual DisplayElementCollection Clone()
		{
			var c = new DisplayElementCollection(Count); // Preset the required capacity to improve memory management.
			CloneTo(c);
			return (c);
		}

		/// <summary>
		/// Clones this collection to the given collection.
		/// </summary>
		public virtual void CloneTo(DisplayElementCollection collection)
		{
			foreach (var de in this)
				collection.Add(de.Clone());
		}

		/// <summary>
		/// Clones a range of this collection to the given collection.
		/// </summary>
		/// <remarks>
		/// Signature following <see cref="List{T}.CopyTo(int, T[], int, int)"/>.
		/// </remarks>
		/// <param name="index">The zero-based index in the source collection at which cloning begins.</param>
		/// <param name="collection">The collection that is the destination of the elements cloned to.</param>
		/// <param name="count">The number of items to copy.</param>
		public virtual void CloneTo(int index, DisplayElementCollection collection, int count)
		{
			for (int i = index; i < count; i++)
				collection.Add(this[i].Clone());
		}

		/// <summary></summary>
		public virtual byte[] ElementsToOrigin()
		{
			var l = new List<byte>(this.ByteCount); // Preset the required capacity to improve memory management.

			foreach (var de in this)
			{
				var origin = de.ToOrigin();
				if (origin != null) // AddRainge(): "The collection itself cannot be null,..."
					l.AddRange(origin);
			}

			return (l.ToArray());
		}

		/// <summary></summary>
		public virtual string ElementsToString()
		{
			var sb = new StringBuilder();

			foreach (var de in this)
			{
				var s = de.ToString();
				if (!string.IsNullOrEmpty(s)) // Similar implementation as 'ToOrigin()' above.
					sb.Append(s);
			}

			return (sb.ToString());
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
			return (ElementsToString()); // Opposed to ...Diagnostics...() below, this is a 'real' ToString() method.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToExtendedDiagnosticsString(string indent = "")
		{
			return (indent + "> Direction: " +      Direction.ToString()                           + Environment.NewLine +
					indent + "> ElementCount: " +       Count.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
					indent + "> CharCount: " + this.charCount.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
					indent + "> ByteCount: " + this.byteCount.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
					indent + "> Elements: " + Environment.NewLine + ElementsToExtendedDiagnosticsString(indent + "   "));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ElementsToExtendedDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder();

			int i = 0;
			foreach (var de in this)
			{
				sb.Append(indent);
				sb.Append("> #");
				sb.Append(i++);
				sb.Append(" = ");
				sb.Append(de.ToDiagnosticsString());
				sb.AppendLine();
			}

			if (i == 0)
			{
				sb.Append(indent);
				sb.Append("None");
				sb.AppendLine();
			}

			return (sb.ToString());
		}

		#endregion
	}

	/// <summary>
	/// Implements a display line containing a collection of display elements.
	/// </summary>
	/// <remarks>
	/// This class redirects to <see cref="DisplayElementCollection"/>. The purpose of this
	/// collection is to add functionality that explicitly applies to a line.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A display line is a collection of multiple elements, but not a collection of display lines as a name ending in 'Collection' would suggest.")]
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
		public DisplayLine(DisplayElement element)
			: base(element)
		{
		}

		/// <summary></summary>
		public DisplayLine(IEnumerable<DisplayElement> collection)
			: base(collection)
		{
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Indicates whether the line is complete, i.e. the collection of elements contains a
		/// <see cref="DisplayElement.LineBreak"/> at the end.
		/// </summary>
		public virtual bool IsComplete
		{
			get
			{
				if (Count > 0)
					return (this[Count - 1] is DisplayElement.LineBreak);

				return (false);
			}
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
			var c = new DisplayLine(Count); // Preset the required capacity to improve memory management.
			CloneTo(c);
			return (c);
		}

		#endregion
	}

	/// <summary>
	/// Implements a collection of display lines.
	/// </summary>
	/// <remarks>
	/// This class redirects to <see cref="T:List`"/>. The sole purpose of this
	/// collection is consistency with <see cref="DisplayElementCollection"/>.
	/// </remarks>
	[Serializable]
	public class DisplayLineCollection : List<DisplayLine>
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayLineCollection()
		{
		}

		/// <summary></summary>
		public DisplayLineCollection(int elementCapacity)
			: base(elementCapacity)
		{
		}

		/// <summary></summary>
		public DisplayLineCollection(IEnumerable<DisplayLine> lines)
			: base(lines)
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
		public DisplayLineCollection Clone()
		{
			var c = new DisplayLineCollection(Count); // Preset the required capacity to improve memory management.
			CloneTo(c);
			return (c);
		}

		/// <summary>
		/// Clones this collection to the given collection.
		/// </summary>
		public virtual void CloneTo(DisplayLineCollection collection)
		{
			foreach (var line in this)
				collection.Add(line.Clone());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
