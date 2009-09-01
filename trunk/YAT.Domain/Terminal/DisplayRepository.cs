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

		private int _capacity = 0;
		private DisplayLine _currentLine;
		private int _dataCount = 0;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayRepository(int capacity)
			: base(capacity)
		{
			_capacity = capacity;
			_currentLine = new DisplayLine();
			_dataCount = 0;
		}

		/// <summary></summary>
		public DisplayRepository(DisplayRepository rhs)
			: base(rhs)
		{
			_capacity = rhs._capacity;
			_currentLine = new DisplayLine(rhs._currentLine);
			_dataCount = rhs._dataCount;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public int Capacity
		{
			get { return (_capacity); }
			set
			{
				if (value > Count)
				{
					_capacity = value;
				}
				else if (value < Count)
				{
					while (Count > value)
						DequeueExcessLine();

					_capacity = value;
				}
			}
		}

		/// <summary>
		/// Returns number of lines within repository.
		/// </summary>
		new public int Count
		{
			get
			{
				if (_currentLine.Count <= 0)
					return (base.Count);
				else
					return (base.Count + 1); // Current line adds one line
			}
		}

		/// <summary>
		/// Returns number of data elements within repository.
		/// </summary>
		public int DataCount
		{
			get { return (_dataCount); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public void Enqueue(DisplayElement item)
		{
			// Add element to current line
			_currentLine.Add(item);
			if (item.IsData)
				_dataCount++;

			// Check whether a line break is needed
			if (item is Domain.DisplayElement.LineBreak)
			{
				// Excess must be manually dequeued
				if (Count >= Capacity)
					DequeueExcessLine();

				// Enqueue new line and reset current line
				base.Enqueue(_currentLine.Clone());
				_currentLine.Clear();
			}
		}

		/// <summary></summary>
		public void Enqueue(IEnumerable<DisplayElement> collection)
		{
			foreach (DisplayElement de in collection)
				Enqueue(de);
		}

		/// <summary></summary>
		new public void Clear()
		{
			base.Clear();
			_currentLine.Clear();
			_dataCount = 0;
		}

		/// <summary></summary>
		new public DisplayLine[] ToArray()
		{
			return (ToLines().ToArray());
		}

		/// <summary></summary>
		public List<DisplayLine> ToLines()
		{
			List<DisplayLine> lines = new List<DisplayLine>(base.ToArray());

			// Add current line if it contains elements
			if (_currentLine.Count > 0)
				lines.Add(new DisplayLine(_currentLine));

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
						_dataCount--;
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		new public string ToString()
		{
			return (ToString(""));
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public string ToString(string indent)
		{
			return (indent + "- LineCapacity: " + Capacity.ToString("D") + Environment.NewLine +
					indent + "- LineCount: " + Count.ToString("D") + Environment.NewLine +
					indent + "- DataCount: " + _dataCount.ToString("D") + Environment.NewLine +
					indent + "- Lines: " + Environment.NewLine + LinesToString(indent + "--"));
		}

		/// <summary></summary>
		public string LinesToString()
		{
			return (LinesToString(""));
		}

		/// <summary></summary>
		public string LinesToString(string indent)
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
