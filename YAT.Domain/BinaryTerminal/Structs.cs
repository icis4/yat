using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// disable warning CS0660
// 'type' defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable 660

namespace HSR.YAT.Domain
{
	public struct BinaryLengthLineBreak
	{
		[XmlElement("Enabled")]
		public bool Enabled;

		[XmlElement("LineLength")]
		public int LineLength;                       // in chars/bytes

	
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

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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

	public struct BinarySequenceLineBreak
	{
		[XmlElement("Enabled")]
		public bool Enabled;

		[XmlElement("Sequence")]
		public string Sequence;

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

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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

	public struct BinaryTimedLineBreak
	{
		[XmlElement("Enabled")]
		public bool Enabled;

		[XmlElement("Timeout")]
		public int Timeout;                          // in ms

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

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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
