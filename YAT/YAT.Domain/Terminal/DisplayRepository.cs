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

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// DisplayRepository is a pseudo fixed-sized Queue holding DisplayElements.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "The repository is intentionally named without any indication of the underlying implementation.")]
	public class DisplayRepository : Queue<DisplayLine>
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int capacity; // = 0;
		private DisplayLinePart currentLine; // = null;
		private int dataCount; // = 0;

		private DisplayLine lastLineAuxiliary; // = null;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayRepository(int capacity)
			: base(capacity)
		{
			this.capacity    = capacity;
			this.currentLine = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
		////this.dataCount   = 0;

			this.lastLineAuxiliary = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
		}

		/// <summary></summary>
		public DisplayRepository(DisplayRepository rhs)
			: base(rhs)
		{
			this.capacity    = rhs.capacity;
			this.currentLine = new DisplayLinePart(rhs.currentLine.Clone());
			this.dataCount   = rhs.dataCount;

			this.lastLineAuxiliary = new DisplayLine(rhs.lastLineAuxiliary.Clone());
		}

#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~DisplayRepository()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

#endif // DEBUG

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual int Capacity
		{
			get { return (this.capacity); }
			set
			{
				if (value > Count)
				{
					this.capacity = value;
				}
				else if (value < Count)
				{
					while (Count > value)
						Dequeue();

					TrimExcess();

					this.capacity = value;
				}
			}
		}

		/// <summary>
		/// Returns number of lines within repository.
		/// </summary>
		public new int Count
		{
			get
			{
				if (this.currentLine.Count <= 0)
					return (base.Count);
				else
					return (base.Count + 1); // Current line adds one line.
			}
		}

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
		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "No clue why Queue<T>.Enqueue(T item) cannot be overridden...")]
		public virtual void Enqueue(DisplayElement item)
		{
			// Add element to current line:
			this.currentLine.Add(item);
			if (item.IsData)
				this.dataCount += item.DataCount;

			// Check whether a line break is needed:
			if (item is DisplayElement.LineBreak)
			{
				// Excess must be manually dequeued:
				if (Count >= Capacity)
					Dequeue();

				// Enqueue new line and reset current line:
				base.Enqueue(this.currentLine.Clone());            // Clone elements to ensure decoupling.
				this.lastLineAuxiliary = this.currentLine.Clone(); // Clone elements to ensure decoupling.
				this.currentLine.Clear();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "No clue why Queue<T>.Enqueue(T item) cannot be overridden...")]
		public virtual void Enqueue(IEnumerable<DisplayElement> collection)
		{
			foreach (DisplayElement de in collection)
				Enqueue(de);
		}

		/// <summary></summary>
		public new DisplayLine Dequeue()
		{
			DisplayLine dl = base.Dequeue();

			foreach (DisplayElement de in dl)
				this.dataCount -= de.DataCount;

			return (dl);
		}

		/// <summary></summary>
		public new void Clear()
		{
			base.Clear();
			this.currentLine.Clear();
			this.dataCount = 0;

			this.lastLineAuxiliary.Clear();
		}

		/// <summary></summary>
		public new DisplayLine[] ToArray()
		{
			return (ToLines().ToArray());
		}

		/// <summary></summary>
		public virtual List<DisplayLine> ToLines()
		{
			List<DisplayLine> lines = new List<DisplayLine>(base.ToArray()); // Using this.ToArray() would result in stack overflow!

			// Add current line if it contains elements:
			if (this.currentLine.Count > 0)
				lines.Add(new DisplayLine(this.currentLine));

			return (lines);
		}

		/// <summary></summary>
		public virtual List<DisplayElement> ToElements()
		{
			List<DisplayElement> elements = new List<DisplayElement>(256); // Preset the initial capacity to improve memory management, 256 is an arbitrary value.

			foreach (DisplayLine line in ToLines())
				elements.AddRange(line.ToArray());

			return (elements);
		}

		/// <summary></summary>
		public virtual DisplayLine LastLineAuxiliary()
		{
			return (this.lastLineAuxiliary);
		}

		/// <summary></summary>
		public virtual void ClearLastLineAuxiliary()
		{
			this.lastLineAuxiliary.Clear();
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			return (indent + "> LineCapacity: " +    Capacity.ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
					indent + "> LineCount: " +          Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
					indent + "> DataCount: " + this.dataCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
					indent + "> Lines: " + Environment.NewLine + LinesToString(indent + "   "));
		}

		/// <summary></summary>
		public virtual string LinesToString()
		{
			return (LinesToString(""));
		}

		/// <summary></summary>
		public virtual string LinesToString(string indent)
		{
			StringBuilder sb = new StringBuilder();

			int i = 0;
			foreach (DisplayLine dl in ToLines())
			{
				sb.Append(indent + "> DisplayLine#" + (i++) + ":" + Environment.NewLine);
				sb.Append(dl.ToString(indent + "   "));
			}

			if (i == 0)
				sb.AppendLine(indent + "<NONE>");

			return (sb.ToString());
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
