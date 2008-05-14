using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain
{
	/// <summary></summary>
	public struct AutoRetry
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary>Interval of reconnect in ms</summary>
		[XmlElement("Interval")]
		public int Interval;

		/// <summary></summary>
		public AutoRetry(bool enabled, int interval)
		{
			Enabled = enabled;
			Interval = interval;
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
		public static bool operator ==(AutoRetry lhs, AutoRetry rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(AutoRetry lhs, AutoRetry rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
