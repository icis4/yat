using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HSR.YAT.Domain
{
	public class EolQueue
	{
		private byte[] _eol;
		private Queue<byte> _queue;

		public EolQueue(byte[] eol)
		{
			_eol = eol;
			_queue = new Queue<byte>(_eol.Length);
		}

		public byte[] Eol
		{
			get
			{
				return (_eol);
			}
		}

		public void Enqueue(byte b)
		{
			while (_queue.Count >= _eol.Length)
			{
				_queue.Dequeue();
			}
			_queue.Enqueue(b);
		}

		public bool EolMatch()
		{
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

		public void Reset()
		{
			_queue.Clear();
		}
	}
}
