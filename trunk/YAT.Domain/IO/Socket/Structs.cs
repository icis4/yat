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
	public struct TcpClientAutoReconnect
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary></summary>
		[XmlElement("Interval")]
		public int Interval;                     // in ms

		/// <summary></summary>
		public TcpClientAutoReconnect(bool enabled, int interval)
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
		public static bool operator ==(TcpClientAutoReconnect lhs, TcpClientAutoReconnect rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TcpClientAutoReconnect lhs, TcpClientAutoReconnect rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
