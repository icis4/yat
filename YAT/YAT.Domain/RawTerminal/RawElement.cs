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
using System.IO;
using System.Globalization;

using MKY.Types;

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
		private byte[] data;
		private SerialDirection direction;
		private DateTime timestamp;

		/// <summary></summary>
		public RawElement(byte[] data, SerialDirection direction)
		{
			this.data = data;
			this.direction = direction;
			this.timestamp = DateTime.Now;
		}

		/// <summary></summary>
		public virtual byte[] Data
		{
			get { return (this.data); }
		}

		/// <summary></summary>
		public virtual SerialDirection Direction
		{
			get { return (this.direction); }
		}

		/// <summary></summary>
		public virtual DateTime TimeStamp
		{
			get { return (this.timestamp); }
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			StringWriter to = new StringWriter();
			foreach (byte b in this.data)
				to.Write(Convert.ToChar(b));

			return (to.ToString());
		}

		/// <summary></summary>
		public virtual string ToDetailedString()
		{
			return (ToDetailedString(""));
		}

		/// <summary></summary>
		public virtual string ToDetailedString(string indent)
		{
			bool begin = true;
			StringWriter data = new StringWriter();
			foreach (byte b in this.data)
			{
				if (!begin)
					data.Write(" ");

				begin = false;
				data.Write(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
			}
			return (indent + "- Data: " + data + Environment.NewLine +
					indent + "- Direction: " + this.direction + Environment.NewLine +
					indent + "- TimeStamp: " + this.timestamp.ToLongTimeString() + "." + StringEx.Left(this.timestamp.Millisecond.ToString("D3"), 2) + Environment.NewLine);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
