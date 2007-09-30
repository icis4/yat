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
		private int _lineCapacity;
		private int _dataCount;
		private int _lineCount;

		private Queue<DisplayElement> _queue;
		private DisplayElement _lastQueued;

		/// <summary></summary>
		public DisplayRepository(int lineCapacity)
		{
			_lineCapacity = lineCapacity;
			_queue = new Queue<DisplayElement>(_lineCapacity);
			ResetInternalState();
		}

		/// <summary></summary>
		public DisplayRepository(DisplayRepository rhs)
		{
			_lineCapacity = rhs._lineCapacity;
			_dataCount = rhs._dataCount;
			_lineCount = rhs._lineCount;
			_queue = new Queue<DisplayElement>(rhs._queue);
		}

		/// <summary></summary>
		public DisplayRepository(int lineCapacity, DisplayRepository rhs)
		{
			_lineCapacity = lineCapacity;
			_queue = new Queue<DisplayElement>(_lineCapacity);
			foreach (DisplayElement de in rhs._queue.ToArray())
				Enqueue(de);
		}

		private void ResetInternalState()
		{
			_dataCount = 0;
			_lineCount = 0;
			_lastQueued = new DisplayElement.LineBreak();
		}

		/// <summary></summary>
		public int LineCapacity
		{
			get { return (_lineCapacity); }
			set
			{
				if (value > _lineCount)
				{
					_lineCapacity = value;
				}
				else if (value < _lineCount)
				{
					while (_lineCount > value)
					{
						DisplayElement d = (DisplayElement)_queue.Dequeue();
						if (d.IsDataElement)
							_dataCount--;
						if (d.IsEol)
							_lineCount--;
					}
					_lineCapacity = value;
				}
			}
		}

		/// <summary></summary>
		public int DataCount
		{
			get { return (_dataCount); }
		}

		/// <summary></summary>
		public int LineCount
		{
			get { return (_lineCount); }
		}

		/// <summary></summary>
		public void Enqueue(List<DisplayElement> elements)
		{
			foreach (DisplayElement de in elements)
				Enqueue(de);
		}

		/// <summary></summary>
		public void Enqueue(DisplayElement de)
		{
			if (de.IsDataElement)
			{
				while (_dataCount >= _lineCapacity)
				{
					DisplayElement temp = (DisplayElement)_queue.Dequeue();
					if (temp.IsDataElement)
						_dataCount--;
					if (temp.IsEol)
						_lineCount--;
				}
			}
			_queue.Enqueue(de);

			if (de.IsDataElement)
				_dataCount++;
			if ((_lastQueued != null) && _lastQueued.IsEol)
				_lineCount++;

			_lastQueued = de;
		}

		/// <summary></summary>
		public void Clear()
		{
			_queue.Clear();
			ResetInternalState();
		}

		/// <summary></summary>
		public List<DisplayElement> ToElements()
		{
			return (new List<DisplayElement>(_queue));
		}

		/// <summary></summary>
		public List<List<DisplayElement>> ToLines()
		{
			List<List<DisplayElement>> lines = new List<List<DisplayElement>>();
			List<DisplayElement> line = new List<DisplayElement>();
			foreach (DisplayElement de in ToElements())
			{
				line.Add(de);
				if (de.IsEol)
				{
					lines.Add(new List<DisplayElement>(line));
					line.Clear();
				}
			}
			if (line.Count > 0)
			{
				lines.Add(new List<DisplayElement>(line)); // add last line
				line.Clear();
			}
			return (lines);
		}

		/// <summary></summary>
		new public string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public string ToString(string indent)
		{
			return (indent + "- DataCapacity: " + _lineCapacity.ToString("D") + Environment.NewLine +
					indent + "- DataCount: " + _dataCount.ToString("D") + Environment.NewLine +
					indent + "- LineCount: " + _lineCount.ToString("D") + Environment.NewLine +
					indent + "- Queue: " + Environment.NewLine + QueueToString(indent + "--"));
		}

		/// <summary></summary>
		public string QueueToString()
		{
			return (QueueToString(""));
		}

		/// <summary></summary>
		public string QueueToString(string indent)
		{
			StringWriter to = new StringWriter();
			int i = 1;
			foreach (DisplayElement de in ToElements())
			{
				to.Write(indent + "DisplayElement " + i++ + ":" + Environment.NewLine);
				to.Write(de.ToString(indent + "--"));
			}
			return (to.ToString());
		}
	}
}
