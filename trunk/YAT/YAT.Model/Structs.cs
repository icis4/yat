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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Model
{
	/// <remarks>
	/// Could further be split into a Tx and Rx struct containing bytes/lines. But not worth so far.
	/// </remarks>
	[Serializable]
	public struct BytesLinesTuple : IEquatable<BytesLinesTuple>
	{
		/// <summary></summary>
		[XmlElement("TxBytes")]
		public int TxBytes { get; set; }

		/// <summary></summary>
		[XmlElement("TxLines")]
		public int TxLines { get; set; }

		/// <summary></summary>
		[XmlElement("RxBytes")]
		public int RxBytes { get; set; }

		/// <summary></summary>
		[XmlElement("RxLines")]
		public int RxLines { get; set; }

		/// <summary></summary>
		public BytesLinesTuple(int txBytes, int txLines, int rxBytes, int rxLines)
		{
			TxBytes = txBytes;
			TxLines = txLines;
			RxBytes = rxBytes;
			RxLines = rxLines;
		}

		/// <summary></summary>
		public void Reset()
		{
			TxBytes = 0;
			TxLines = 0;
			RxBytes = 0;
			RxLines = 0;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

				hashCode =                    TxBytes;
				hashCode = (hashCode * 397) ^ TxLines;
				hashCode = (hashCode * 397) ^ RxBytes;
				hashCode = (hashCode * 397) ^ RxLines;

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is BytesLinesTuple)
				return (Equals((BytesLinesTuple)obj));
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
		public bool Equals(BytesLinesTuple other)
		{
			return
			(
				TxBytes.Equals(other.TxBytes) &&
				TxLines.Equals(other.TxLines) &&
				RxBytes.Equals(other.RxBytes) &&
				RxLines.Equals(other.RxLines)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(BytesLinesTuple lhs, BytesLinesTuple rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(BytesLinesTuple lhs, BytesLinesTuple rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct CountsRatesTuple : IEquatable<CountsRatesTuple>
	{
		/// <summary></summary>
		[XmlElement("Counts")]
		public BytesLinesTuple Counts { get; set; }

		/// <summary></summary>
		[XmlElement("Rates")]
		public BytesLinesTuple Rates { get; set; }

		/// <summary></summary>
		public CountsRatesTuple(BytesLinesTuple counts, BytesLinesTuple rates)
		{
			Counts = counts;
			Rates  = rates;
		}

		/// <summary></summary>
		public void Reset()
		{
			Counts.Reset();
			Rates .Reset();
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

				hashCode =                    Counts.GetHashCode();
				hashCode = (hashCode * 397) ^ Rates .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is CountsRatesTuple)
				return (Equals((CountsRatesTuple)obj));
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
		public bool Equals(CountsRatesTuple other)
		{
			return
			(
				Counts.Equals(other.Counts) &&
				Rates .Equals(other.Rates)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(CountsRatesTuple lhs, CountsRatesTuple rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have inequality.
		/// </summary>
		public static bool operator !=(CountsRatesTuple lhs, CountsRatesTuple rhs)
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
