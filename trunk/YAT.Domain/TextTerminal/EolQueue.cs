using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MKY.YAT.Domain
{
	/// <summary></summary>
	public class EolQueue
	{
		private byte[] _eol;
		private Queue<byte> _queue;

		/// <summary></summary>
		public EolQueue(byte[] eol)
		{
			_eol = eol;
			_queue = new Queue<byte>(_eol.Length);
		}

		/// <summary></summary>
		public byte[] Eol
		{
			get
			{
				return (_eol);
			}
		}

		/// <summary></summary>
		public void Enqueue(byte b)
		{
			if (_eol.Length > 0)
			{
				while (_queue.Count >= _eol.Length)
				{
					_queue.Dequeue();
				}
				_queue.Enqueue(b);
			}
			else if (_queue.Count > 0)
			{
				_queue.Clear();
			}
		}

		/// <summary></summary>
		public bool EolMatch()
		{
			if (_eol.Length <= 0)
				return (false);

			byte[] queue = ToByteArray();

			if (queue.Length != _eol.Length)
				return (false);

			for (int i = 0; i < queue.Length; i++)
			{
				if (queue[i] != _eol[i])
					return (false);
			}

			return (true);
		}

		private byte[] ToByteArray()
		{
			MemoryStream to = new MemoryStream(_queue.Count);
			foreach (byte b in _queue.ToArray())
			{
				to.WriteByte(b);
			}
			return (to.ToArray());
		}

		/// <summary></summary>
		public void Reset()
		{
			_queue.Clear();
		}
	}
}
