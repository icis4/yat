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
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	[Serializable]
	public struct BinaryLengthLineBreak
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary></summary>
		[XmlElement("LineLength", typeof(int))]
		public int LineLength; // In chars/bytes.

		/// <summary></summary>
		public BinaryLengthLineBreak(bool enabled, int lineLength)
		{
			Enabled = enabled;
			LineLength = lineLength;
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			BinaryLengthLineBreak other = (BinaryLengthLineBreak)obj;
			return
			(
				(Enabled == other.Enabled) &&
				(LineLength == other.LineLength)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				Enabled.GetHashCode() ^
				LineLength.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryLengthLineBreak lhs, BinaryLengthLineBreak rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(BinaryLengthLineBreak lhs, BinaryLengthLineBreak rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct BinarySequenceLineBreak
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary></summary>
		[XmlElement("Sequence")]
		public string Sequence;

		/// <summary></summary>
		public BinarySequenceLineBreak(bool enabled, string sequence)
		{
			Enabled = enabled;
			Sequence = sequence;
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			BinarySequenceLineBreak other = (BinarySequenceLineBreak)obj;
			return
			(
				(Enabled == other.Enabled) &&
				(StringEx.EqualsOrdinalIgnoreCase(Sequence, other.Sequence))
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				Enabled.GetHashCode() ^
				Sequence.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinarySequenceLineBreak lhs, BinarySequenceLineBreak rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(BinarySequenceLineBreak lhs, BinarySequenceLineBreak rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct BinaryTimedLineBreak
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary>Timeout in ms.</summary>
		[XmlElement("Timeout")]
		public int Timeout;

		/// <summary></summary>
		public BinaryTimedLineBreak(bool enabled, int timeout)
		{
			Enabled = enabled;
			Timeout = timeout;
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			BinaryTimedLineBreak other = (BinaryTimedLineBreak)obj;
			return
			(
				(Enabled == other.Enabled) &&
				(Timeout == other.Timeout)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				Enabled.GetHashCode() ^
				Timeout.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryTimedLineBreak lhs, BinaryTimedLineBreak rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(BinaryTimedLineBreak lhs, BinaryTimedLineBreak rhs)
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
