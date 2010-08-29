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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
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
			return (base.Equals(obj));
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryLengthLineBreak lhs, BinaryLengthLineBreak rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

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
			return (base.Equals(obj));
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinarySequenceLineBreak lhs, BinarySequenceLineBreak rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

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
			return (base.Equals(obj));
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryTimedLineBreak lhs, BinaryTimedLineBreak rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

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
