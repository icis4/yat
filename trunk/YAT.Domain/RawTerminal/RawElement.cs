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
using System.IO;

using MKY.Utilities.Types;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
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
			StringWriter to = new StringWriter();
			foreach (byte b in _data)
				to.Write(Convert.ToChar(b));

			return (to.ToString());
		}

		/// <summary></summary>
		public string ToDetailedString()
		{
			return (ToDetailedString(""));
		}

		/// <summary></summary>
		public string ToDetailedString(string indent)
		{
			bool begin = true;
			StringWriter data = new StringWriter();
			foreach (byte b in _data)
			{
				if (!begin)
					data.Write(" ");

				begin = false;
				data.Write(b.ToString("X2") + "h");
			}
			return (indent + "- Data: " + data + Environment.NewLine +
					indent + "- Direction: " + _direction + Environment.NewLine +
					indent + "- TimeStamp: " + _timestamp.ToLongTimeString() + "." + XString.Left(_timestamp.Millisecond.ToString("D3"), 2) + Environment.NewLine);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
