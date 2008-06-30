using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace YAT.Domain
{
	/// <summary>
	/// DisplayRepository is a pseudo fixed-sized Queue holding DisplayElements.
	/// </summary>
	public class DisplayRepository
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int _lineCapacity;
		private int _perLineCapacity;

		private int _dataCount;

		private Queue<List<DisplayElement>> _lines;
		private List<DisplayElement> _currentLine;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayRepository(int lineCapacity, int perLineCapacity)
		{
			_lineCapacity    = lineCapacity;
			_perLineCapacity = perLineCapacity;

			_dataCount = 0;

			_lines       = new Queue<List<DisplayElement>>(_lineCapacity);
			_currentLine = new List<DisplayElement>(_perLineCapacity);
		}

		/// <summary></summary>
		public DisplayRepository(DisplayRepository rhs)
		{
			_lineCapacity    = rhs._lineCapacity;
			_perLineCapacity = rhs._perLineCapacity;

			_dataCount = rhs._dataCount;

			_lines       = new Queue<List<DisplayElement>>(rhs._lines);
			_currentLine = new List<DisplayElement>(rhs._currentLine);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public int LineCapacity
		{
			get { return (_lineCapacity); }
			set
			{
				if (value > _lines.Count)
				{
					_lineCapacity = value;
				}
				else if (value < _lines.Count)
				{
					while (_lines.Count > value)
						DequeueExcessLine();

					_lineCapacity = value;
				}
			}
		}

		/// <summary>
		/// Returns number of data elements within repository.
		/// </summary>
		public int DataCount
		{
			get { return (_dataCount); }
		}

		/// <summary>
		/// Returns number of lines within repository.
		/// </summary>
		public int LineCount
		{
			get
			{
				if (_currentLine.Count <= 0)
					return (_lines.Count);
				else
					return (_lines.Count + 1); // Current line adds one line
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public void Enqueue(List<DisplayElement> elements)
		{
			foreach (DisplayElement de in elements)
				Enqueue(de);
		}

		/// <summary></summary>
		public void Enqueue(DisplayElement de)
		{
			// Add element to current line if there is space left
			if (_currentLine.Count < _perLineCapacity)
			{
				_currentLine.Add(de);
				if (de.IsDataElement)
					_dataCount++;
			}

			// Check whether a line break is needed
			if (de.IsEol)
			{
				// Excess must be manually dequeued
				if (_lines.Count >= _lineCapacity)
					DequeueExcessLine();

				// Enqueue new line and reset current line
				_lines.Enqueue(new List<DisplayElement>(_currentLine));
				_currentLine.Clear();
			}
		}

		/// <summary></summary>
		public void Clear()
		{
			_lines.Clear();
			_currentLine.Clear();

			_dataCount = 0;
		}

		/// <summary></summary>
		public List<List<DisplayElement>> ToLines()
		{
			List<List<DisplayElement>> lines = new List<List<DisplayElement>>(_lines.ToArray());

			// Add current line if it contains elements
			if (_currentLine.Count > 0)
				lines.Add(new List<DisplayElement>(_currentLine));

			return (lines);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void DequeueExcessLine()
		{
			List<DisplayElement> line = _lines.Dequeue();
			foreach (DisplayElement de in line)
			{
				if (de.IsDataElement)
					_dataCount--;
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
			return (indent + "- DataCapacity: " + _lineCapacity.ToString("D") + Environment.NewLine +
					indent + "- DataCount: " + _dataCount.ToString("D") + Environment.NewLine +
					indent + "- LineCount: " + _lines.Count.ToString("D") + Environment.NewLine +
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
			StringWriter to = new StringWriter();
			int l = 0;
			foreach (List<DisplayElement> line in ToLines())
			{
				l++;
				int e = 0;
				foreach (DisplayElement de in line)
				{
					e++;
					to.Write(indent + "Line " + l + " DisplayElement " + e + ":" + Environment.NewLine);
					to.Write(de.ToString(indent + "--"));
				}
			}
			return (to.ToString());
		}

		#endregion

		#endregion
	}
}
