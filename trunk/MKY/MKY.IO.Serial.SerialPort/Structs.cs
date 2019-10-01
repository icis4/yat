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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace MKY.IO.Serial.SerialPort
{
	/// <summary></summary>
	[Serializable]
	public struct OutputBufferSize : IEquatable<OutputBufferSize>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary></summary>
		[XmlElement("Size")]
		public int Size { get; set; }

		/// <summary></summary>
		public OutputBufferSize(bool enabled, int size)
		{
			Enabled = enabled;
			Size = size;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return
			(
				Enabled + ", " +
				Size
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

				hashCode =                    Enabled.GetHashCode();
				hashCode = (hashCode * 397) ^ Size;

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is OutputBufferSize)
				return (Equals((OutputBufferSize)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(OutputBufferSize other)
		{
			return
			(
				Enabled.Equals(other.Enabled) &&
				Size   .Equals(other.Size)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(OutputBufferSize lhs, OutputBufferSize rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(OutputBufferSize lhs, OutputBufferSize rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct ChunkSize : IEquatable<ChunkSize>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary></summary>
		[XmlElement("Size")]
		public int Size { get; set; }

		/// <summary></summary>
		public ChunkSize(bool enabled, int size)
		{
			Enabled = enabled;
			Size    = size;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return
			(
				Enabled + ", " +
				Size
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

				hashCode =                    Enabled.GetHashCode();
				hashCode = (hashCode * 397) ^ Size;

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is ChunkSize)
				return (Equals((ChunkSize)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(ChunkSize other)
		{
			return
			(
				Enabled.Equals(other.Enabled) &&
				Size   .Equals(other.Size)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(ChunkSize lhs, ChunkSize rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(ChunkSize lhs, ChunkSize rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct SendRate : IEquatable<SendRate>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary></summary>
		[XmlElement("Size")]
		public int Size { get; set; }

		/// <summary></summary>
		[XmlElement("Interval")]
		public int Interval { get; set; }

		/// <summary></summary>
		public SendRate(bool enabled, int size, int interval)
		{
			Enabled  = enabled;
			Size     = size;
			Interval = interval;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return
			(
				Enabled + ", " +
				Size    + ", " +
				Interval
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

				hashCode =                    Enabled.GetHashCode();
				hashCode = (hashCode * 397) ^ Size;
				hashCode = (hashCode * 397) ^ Interval;

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SendRate)
				return (Equals((SendRate)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SendRate other)
		{
			return
			(
				Enabled .Equals(other.Enabled) &&
				Size    .Equals(other.Size) &&
				Interval.Equals(other.Interval)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(SendRate lhs, SendRate rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(SendRate lhs, SendRate rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
