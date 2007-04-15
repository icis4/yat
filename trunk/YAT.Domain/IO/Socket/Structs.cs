using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// disable warning CS0660
// 'type' defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable 660

namespace HSR.YAT.Domain
{
	public struct TcpClientAutoReconnect
	{
		[XmlElement("Enabled")]
		public bool Enabled;

		[XmlElement("Interval")]
		public int Interval;                     // in ms

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

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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
