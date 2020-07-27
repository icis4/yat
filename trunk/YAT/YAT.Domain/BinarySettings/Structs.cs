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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY;

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in YAT.Domain\BinarySettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public struct BinarySequenceSettingTuple : IEquatable<BinarySequenceSettingTuple>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary></summary>
		[XmlElement("Sequence")]
		public string Sequence { get; set; }

		/// <summary></summary>
		public BinarySequenceSettingTuple(bool enabled, string sequence)
		{
			Enabled  = enabled;
			Sequence = sequence;
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

				hashCode =                                        Enabled .GetHashCode();
				hashCode = (hashCode * 397) ^ (Sequence != null ? Sequence.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is BinarySequenceSettingTuple)
				return (Equals((BinarySequenceSettingTuple)obj));
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
		public bool Equals(BinarySequenceSettingTuple other)
		{
			return
			(
				Enabled.Equals(                            other.Enabled) &&
				StringEx.EqualsOrdinalIgnoreCase(Sequence, other.Sequence)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(BinarySequenceSettingTuple lhs, BinarySequenceSettingTuple rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(BinarySequenceSettingTuple lhs, BinarySequenceSettingTuple rhs)
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
