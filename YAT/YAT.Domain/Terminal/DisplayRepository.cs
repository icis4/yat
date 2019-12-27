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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// This class wraps a size limited <see cref="Queue{T}"/> holding <see cref="DisplayLine"/> items.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "The repository is intentionally named without any indication of the underlying implementation.")]
	public class DisplayRepository : Queue<DisplayLine>
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int capacity; // = 0;
		private int byteCount; // = 0;

		private DisplayLine currentLine; // = null;

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
		////this.byteCount   = 0;                       // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
			this.currentLine = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
			                                                  // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
			this.lastLineAuxiliary = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
		}

		/// <summary></summary>
		public DisplayRepository(DisplayRepository rhs)
			: base(rhs)
		{
			this.capacity  = rhs.capacity;
			this.byteCount = rhs.byteCount;

			this.currentLine = rhs.currentLine.Clone();

			this.lastLineAuxiliary = rhs.lastLineAuxiliary.Clone();
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
		/// Returns number of raw byte content within repository.
		/// </summary>
		/// <remarks>
		/// Note that value reflects the byte count of the elements contained in the repository,
		/// i.e. the byte count of the elements shown. The value thus not necessarily reflects the
		/// total byte count of a sent or received sequence, a hidden EOL is e.g. not reflected.
		/// </remarks>
		public virtual int ByteCount
		{
			get { return (this.byteCount); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Enqueue(DisplayElement item)
		{
			// Add element to current line:
			this.byteCount += item.ByteCount;
			this.currentLine.Add(item);

			// Check whether a line break is needed:
			if (item is DisplayElement.LineBreak)
			{
				// Excess must be manually dequeued, typically just one element has to:
				while (Count >= Capacity)
					Dequeue();

				// Enqueue new line and reset current line:
				base.Enqueue(this.currentLine.Clone());            // Clone to ensure decoupling.
				this.lastLineAuxiliary = this.currentLine.Clone(); // Clone to ensure decoupling.
				this.currentLine.Clear();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "Emphasize use.")]
		public virtual void Enqueue(IEnumerable<DisplayElement> collection)
		{
			foreach (var de in collection)
				Enqueue(de);
		}

		/// <remarks>
		/// Ensure that <see cref="Enqueue(DisplayElement)"/> is called, which keeps track of the counts.
		/// </remarks>
		public new void Enqueue(DisplayLine line)
		{
			foreach (var de in line)
				Enqueue(de);
		}

		/// <remarks>
		/// Ensure that track of the counts is kept.
		/// </remarks>
		public new DisplayLine Dequeue()
		{
			var dl = base.Dequeue();

			foreach (var de in dl)
				this.byteCount -= de.ByteCount;

			return (dl);
		}

		/// <summary></summary>
		public void ReplaceCurrentLine(DisplayElementCollection currentLineElements)
		{
			if (this.currentLine.Count > 0) // Only something to do if there is something in current line indeed.
			{
				ClearCurrentLine();
				Enqueue(currentLineElements);
			}
		}

		/// <summary></summary>
		public void ClearCurrentLine()
		{
			if (this.currentLine.Count > 0) // Only something to do if there is something in current line indeed.
			{
				this.byteCount -= this.currentLine.ByteCount;

				this.currentLine.Clear();
			}
		}

		/// <summary></summary>
		public new void Clear()
		{
			base.Clear();
			this.byteCount = 0;

			this.currentLine.Clear();

			this.lastLineAuxiliary.Clear();
		}

		/// <summary></summary>
		public new DisplayLine[] ToArray()
		{
			return (ToLines().ToArray());
		}

		/// <summary></summary>
		public virtual DisplayLineCollection ToLines()
		{
			var lines = new DisplayLineCollection(base.ToArray()); // Using this.ToArray() would result in stack overflow!

			// Add current line if it contains elements:
			if (this.currentLine.Count > 0)
				lines.Add(this.currentLine.Clone()); // Clone to ensure decoupling.

			return (lines);
		}

		/// <summary></summary>
		public virtual DisplayElementCollection ToElements()
		{
			var elements = new DisplayElementCollection(256); // Preset the initial capacity to improve memory management; 256 is an arbitrary value.

			foreach (var line in ToLines())
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

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToExtendedDiagnosticsString()); // No 'real' ToString() method required yet.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		public virtual string ToExtendedDiagnosticsString(string indent = "")
		{
			return (indent + "> LineCapacity: " +    Capacity.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
					indent + "> LineCount: " +          Count.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
					indent + "> ByteCount: " + this.byteCount.ToString(CultureInfo.CurrentCulture) + Environment.NewLine +
					indent + "> Lines: " + Environment.NewLine + LinesToExtendedDiagnosticsString(indent + "   "));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string LinesToExtendedDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder();

			int i = 0;
			foreach (var dl in ToLines())
			{
				sb.Append(indent + "> DisplayLine#" + (i++) + ":" + Environment.NewLine);
				sb.Append(dl.ToExtendedDiagnosticsString(indent + "   "));
			}

			if (i == 0)
				sb.AppendLine(indent + "<None>");

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
