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
// YAT 2.0 Gamma 3 Development Version 1.99.53
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

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <remarks>
		/// "Guidelines for Collections": "Do use byte arrays instead of collections of bytes."
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private byte[]      data;
		private DateTime    timeStamp;
		private string      portStamp;
		private IODirection direction;

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
			this.data      = data;
			this.timeStamp = timeStamp;
			this.portStamp = portStamp;
			this.direction = direction;
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

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public virtual byte[] Data
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
		public virtual RawChunk Clone()
		{
			return (new RawChunk((byte[])this.data.Clone(), this.timeStamp, this.portStamp, this.direction));
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
				foreach (byte b in this.data)
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
				foreach (byte b in this.data)
				{
					if (!begin)
						sw.Write(" ");

					begin = false;
					sw.Write(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
				}

				var sb = new StringBuilder();
				sb.AppendLine(indent + "> Data: " + sw);
				sb.AppendLine(indent + "> TimeStamp: " + this.timeStamp.ToLongTimeString() + "." + StringEx.Left(this.timeStamp.Millisecond.ToString("D3", CultureInfo.InvariantCulture), 2));
				sb.AppendLine(indent + "> PortStamp: " + this.portStamp);
				sb.AppendLine(indent + "> Direction: " + this.direction);
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
