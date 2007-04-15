using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HSR.YAT.Domain
{
	/// <summary>
	/// RawRepository is a fixed-sized Queue holding RawElements.
	/// </summary>
	public class RawRepository
	{
		private int _capacity;
		private Queue<RawElement> _queue;

		public RawRepository(int capacity)
		{
			_capacity = capacity;
			_queue = new Queue<RawElement>(_capacity);
		}

		public RawRepository(RawRepository rhs)
		{
			_capacity = rhs._capacity;
			_queue = new Queue<RawElement>(rhs._queue);
		}

		public int Capacity
		{
			get { return (_capacity); }
			set
			{
				if (value == _queue.Count)
					return;

				if (value < _queue.Count)
				{
					while (_queue.Count > value)
						_queue.Dequeue();

					_capacity = value;
				}
			}
		}

		public void Enqueue(RawElement re)
		{
			while (_queue.Count >= _capacity)
				_queue.Dequeue();

			_queue.Enqueue(re);
		}

		public void Clear()
		{
			_queue.Clear();
		}

		public byte[] ToByteArray()
		{
			MemoryStream to = new MemoryStream(_queue.Count);
			foreach (object o in _queue.ToArray())
			{
				byte[] data = ((RawElement)o).Data;
				int length = data.Length;
				to.Write(data, 0, length);
			}
			return (to.ToArray());
		}

		public List<RawElement> ToElements()
		{
			return (new List<RawElement>(_queue));
		}

		new public string ToString()
		{
			return (ToString(""));
		}

		public string ToString(string indent)
		{
			return (indent + "- Capacity: " + _capacity.ToString("D") + Environment.NewLine +
					indent + "- Queue: " + Environment.NewLine + QueueToString(indent + "--"));
		}

		public string QueueToString()
		{
			return (QueueToString(""));
		}

		public string QueueToString(string indent)
		{
			StringWriter to = new StringWriter();
			int i = 1;
			foreach (RawElement re in ToElements())
			{
				to.Write(indent + "RawElement " + i++ + ":" + Environment.NewLine);
				to.Write(re.ToString(indent + "--"));
			}
			return (to.ToString());
		}
	}
}
