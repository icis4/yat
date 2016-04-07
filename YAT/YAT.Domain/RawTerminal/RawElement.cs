﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

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
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ReadOnlyCollection<byte> data;
		private DateTime    timeStamp;
		private string      portStamp;
		private IODirection direction;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RawElement(IEnumerable<byte> data, IODirection direction)
			: this(data, Undefined, direction)
		{
		}

		/// <summary></summary>
		public RawElement(IEnumerable<byte> data, string portStamp, IODirection direction)
			: this(data, DateTime.Now, portStamp, direction)
		{
		}

		/// <summary></summary>
		public RawElement(IEnumerable<byte> data, DateTime timeStamp, string portStamp, IODirection direction)
		{
			List<byte> l = new List<byte>();
			foreach (byte b in data)
				l.Add(b);

			this.data      = new ReadOnlyCollection<byte>(l);
			this.timeStamp = timeStamp;
			this.portStamp = portStamp;
			this.direction = direction;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual ReadOnlyCollection<byte> Data
		{
			get { return (this.data); }
		}

		/// <summary></summary>
		public virtual DateTime TimeStamp
		{
			get { return (this.timeStamp); }
		}

		/// <summary></summary>
		public virtual string PortStamp
		{
			get { return (this.portStamp); }
		}

		/// <summary></summary>
		public virtual IODirection Direction
		{
			get { return (this.direction); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		public virtual RawElement Clone()
		{
			return (new RawElement(this.data, this.timeStamp, this.portStamp, this.direction));
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

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
				data.Write(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(indent + "> Data: " + data);
			sb.AppendLine(indent + "> TimeStamp: " + this.timeStamp.ToLongTimeString() + "." + StringEx.Left(this.timeStamp.Millisecond.ToString("D3", CultureInfo.InvariantCulture), 2));
			sb.AppendLine(indent + "> PortStamp: " + this.portStamp);
			sb.AppendLine(indent + "> Direction: " + this.direction);
			return (sb.ToString());
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
