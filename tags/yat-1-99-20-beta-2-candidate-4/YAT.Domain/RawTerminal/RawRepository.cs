using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace YAT.Domain
{
	/// <summary>
	/// RawRepository is a fixed-sized Queue holding RawElements.
	/// </summary>
	public class RawRepository
	{
		private int _capacity;
		private Queue<RawElement> _queue;

		/// <summary></summary>
		public RawRepository(int capacity)
		{
			_capacity = capacity;
			_queue = new Queue<RawElement>(_capacity);
		}

		/// <summary></summary>
		public RawRepository(RawRepository rhs)
		{
			_capacity = rhs._capacity;
			_queue = new Queue<RawElement>(rhs._queue);
		}

		/// <summary></summary>
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

		/// <summary></summary>
		public void Enqueue(RawElement re)
		{
			while (_queue.Count >= _capacity)
				_queue.Dequeue();

			_queue.Enqueue(re);
		}

		/// <summary></summary>
		public void Clear()
		{
			_queue.Clear();
		}

		/// <summary></summary>
		public byte[] ToByteArray()
		{
			List<byte> to = new List<byte>(_queue.Count);
			foreach (RawElement re in _queue.ToArray())
				to.AddRange(re.Data);

			return (to.ToArray());
		}

		/// <summary></summary>
		public List<RawElement> ToElements()
		{
			return (new List<RawElement>(_queue));
		}

		/// <summary></summary>
		new public string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public string ToString(string indent)
		{
			return (QueueToString(indent));
		}

		/// <summary></summary>
		public string ToDetailedString()
		{
			return (ToDetailedString(""));
		}

		/// <summary></summary>
		public string ToDetailedString(string indent)
		{
			return (indent + "- Capacity: " + _capacity + Environment.NewLine +
					indent + "- Queue: " + Environment.NewLine +
					QueueToDetailedString(indent + "--"));
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
			foreach (RawElement re in ToElements())
				to.Write(re.ToString(indent));

			return (to.ToString());
		}

		/// <summary></summary>
		public string QueueToDetailedString()
		{
			return (QueueToDetailedString(""));
		}

		/// <summary></summary>
		public string QueueToDetailedString(string indent)
		{
			StringWriter to = new StringWriter();
			int i = 1;
			foreach (RawElement re in ToElements())
			{
				to.Write(indent + "RawElement " + i++ + ":" + Environment.NewLine);
				to.Write(re.ToDetailedString(indent + "--"));
			}
			return (to.ToString());
		}
	}
}