using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace YAT.Domain
{
	/// <summary>
	/// Defines an element received from or sent to a serial interface. In
	/// addition to the serial data it also contains interface and time
	/// information.
	/// </summary>
	public class RawElement
	{
		private byte[] _data;
		private SerialDirection _direction;
		private DateTime _timestamp;

		/// <summary></summary>
		public RawElement(byte[] data, SerialDirection direction)
		{
			_data = data;
			_direction = direction;
			_timestamp = DateTime.Now;
		}

		/// <summary></summary>
		public byte[] Data
		{
			get { return (_data); }
		}

		/// <summary></summary>
		public SerialDirection Direction
		{
			get { return (_direction); }
		}

		/// <summary></summary>
		public DateTime TimeStamp
		{
			get { return (_timestamp); }
		}

		/// <summary></summary>
		new public string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public string ToString(string indent)
		{
			string dataString = "";
			foreach (byte b in _data)
			{
				if (dataString != "")
					dataString += " ";

				dataString += b.ToString("X2") + "h";
			}
			return (indent + "- Data: " + dataString + Environment.NewLine +
					indent + "- Direction: " + _direction.ToString() + Environment.NewLine +
					indent + "- TimeStamp: " + _timestamp.ToLongTimeString() + "." + Utilities.Types.XString.Left(_timestamp.Millisecond.ToString("D3"), 2) + Environment.NewLine );
		}
	}
}
