using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// disable warning CS0660
// 'type' defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable 660

namespace MKY.YAT.Domain
{
	/// <summary></summary>
	public struct TextLineSendDelay
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary></summary>
		[XmlElement("Delay")]
		public int Delay;                            // in ms

		/// <summary></summary>
		[XmlElement("LineInterval")]
		public int LineInterval;

		/// <summary></summary>
		public TextLineSendDelay(bool enabled, int delay, int lineInterval)
		{
			Enabled = enabled;
			Delay = delay;
			LineInterval = lineInterval;
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
		public static bool operator ==(TextLineSendDelay lhs, TextLineSendDelay rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TextLineSendDelay lhs, TextLineSendDelay rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	public struct WaitForResponse
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary></summary>
		[XmlElement("Timeout")]
		public int Timeout;                            // in ms

		/// <summary></summary>
		public WaitForResponse(bool enabled, int timeout)
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
		public static bool operator ==(WaitForResponse lhs, WaitForResponse rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(WaitForResponse lhs, WaitForResponse rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
