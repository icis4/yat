﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Globalization;
using System.IO;

using MKY;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Defines an element received from or sent to a serial interface. In addition to the serial
	/// data itself, it also contains interface and time information.
	/// </summary>
	public class RawElement
	{
		private byte[] data;
		private SerialDirection direction;
		private DateTime timeStamp;

		/// <summary></summary>
		public RawElement(byte[] data, SerialDirection direction)
			: this (data, direction, DateTime.Now)
		{
		}

		/// <summary></summary>
		public RawElement(byte[] data, SerialDirection direction, DateTime timeStamp)
		{
			this.data = data;
			this.direction = direction;
			this.timeStamp = timeStamp;
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
			get { return (this.timeStamp); }
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			StringWriter to = new StringWriter(CultureInfo.InvariantCulture);
			foreach (byte b in this.data)
				to.Write(Convert.ToChar(b));

			return (indent + to.ToString());
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
			StringWriter data = new StringWriter(CultureInfo.InvariantCulture);
			foreach (byte b in this.data)
			{
				if (!begin)
					data.Write(" ");

				begin = false;
				data.Write(b.ToString("X2", NumberFormatInfo.InvariantInfo) + "h");
			}

			return (indent + "- Data: " + data + Environment.NewLine +
					indent + "- Direction: " + this.direction + Environment.NewLine +
					indent + "- TimeStamp: " + this.timeStamp.ToLongTimeString() + "." + StringEx.Left(this.timeStamp.Millisecond.ToString("D3", NumberFormatInfo.InvariantInfo), 2) + Environment.NewLine);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
