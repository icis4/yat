//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
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
	public class RawChunk
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] Data { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public string PortStamp { get; }

		/// <summary></summary>
		public IODirection Direction { get; }

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RawChunk(byte[] data, IODirection direction)
			: this(data, Undefined, direction)
		{
		}

		/// <summary></summary>
		public RawChunk(byte[] data, string portStamp, IODirection direction)
			: this(data, DateTime.Now, portStamp, direction)
		{
		}

		/// <summary></summary>
		public RawChunk(byte[] data, DateTime timeStamp, string portStamp, IODirection direction)
		{
			Data      = data;
			TimeStamp = timeStamp;
			PortStamp = portStamp;
			Direction = direction;
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~RawChunk()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		public virtual RawChunk Clone()
		{
			return (new RawChunk((byte[])Data.Clone(), TimeStamp, PortStamp, Direction));
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			using (var sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				foreach (byte b in Data)
					sw.Write(Convert.ToChar(b));

				return (indent + sw.ToString());
			}
		}

		/// <summary></summary>
		public virtual string ToDetailedString()
		{
			return (ToDetailedString(""));
		}

		/// <summary></summary>
		public virtual string ToDetailedString(string indent)
		{
			using (var sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				bool begin = true;
				foreach (byte b in Data)
				{
					if (!begin)
						sw.Write(" ");

					begin = false;
					sw.Write(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
				}

				var sb = new StringBuilder();
				sb.AppendLine(indent + "> Data: " + sw);
				sb.AppendLine(indent + "> TimeStamp: " + TimeStamp.ToLongTimeString() + "." + StringEx.Left(TimeStamp.Millisecond.ToString("D3", CultureInfo.InvariantCulture), 2));
				sb.AppendLine(indent + "> PortStamp: " + PortStamp);
				sb.AppendLine(indent + "> Direction: " + Direction);
				return (sb.ToString());
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
