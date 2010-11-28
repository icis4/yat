//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
using System.Text;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// DisplayRepository is a pseudo fixed-sized Queue holding DisplayElements.
	/// </summary>
	public class DisplayRepository : Queue<DisplayLine>
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int capacity = 0;
		private DisplayLine currentLine;
		private int dataCount = 0;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayRepository(int capacity)
			: base(capacity)
		{
			this.capacity = capacity;
			this.currentLine = new DisplayLine();
			this.dataCount = 0;
		}

		/// <summary></summary>
		public DisplayRepository(DisplayRepository rhs)
			: base(rhs)
		{
			this.capacity = rhs.capacity;
			this.currentLine = new DisplayLine(rhs.currentLine);
			this.dataCount = rhs.dataCount;
		}

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
						DequeueExcessLine();

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
					return (base.Count + 1); // Current line adds one line
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
		public virtual void Enqueue(DisplayElement item)
		{
			// Add element to current line
			this.currentLine.Add(item);
			if (item.IsData)
				this.dataCount++;

			// Check whether a line break is needed
			if (item is Domain.DisplayElement.LineBreak)
			{
				// Excess must be manually dequeued
				if (Count >= Capacity)
					DequeueExcessLine();

				// Enqueue new line and reset current line
				base.Enqueue(this.currentLine.Clone());
				this.currentLine.Clear();
			}
		}

		/// <summary></summary>
		public virtual void Enqueue(IEnumerable<DisplayElement> collection)
		{
			foreach (DisplayElement de in collection)
				Enqueue(de);
		}

		/// <summary></summary>
		public new void Clear()
		{
			base.Clear();
			this.currentLine.Clear();
			this.dataCount = 0;
		}

		/// <summary></summary>
		public new DisplayLine[] ToArray()
		{
			return (ToLines().ToArray());
		}

		/// <summary></summary>
		public virtual List<DisplayLine> ToLines()
		{
			List<DisplayLine> lines = new List<DisplayLine>(base.ToArray());

			// Add current line if it contains elements
			if (this.currentLine.Count > 0)
				lines.Add(new DisplayLine(this.currentLine));

			return (lines);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void DequeueExcessLine()
		{
			if (Count > 0)
			{
				DisplayLine dl = Dequeue();
				foreach (DisplayElement de in dl)
				{
					if (de.IsData)
						this.dataCount--;
				}
			}
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
			return (indent + "- LineCapacity: " + Capacity.ToString("D") + Environment.NewLine +
					indent + "- LineCount: " + Count.ToString("D") + Environment.NewLine +
					indent + "- DataCount: " + this.dataCount.ToString("D") + Environment.NewLine +
					indent + "- Lines: " + Environment.NewLine + LinesToString(indent + "--"));
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
				i++;
				sb.Append(indent + "DisplayLine " + i + ":" + Environment.NewLine);
				sb.Append(dl.ToString(indent + "--"));
			}
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
