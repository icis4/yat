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
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace MKY.System.Collections
{
	/// <summary>
	/// Serializable string key/value-pair.
	/// </summary>
	/// <remarks>
	/// Required because <see cref="T:KeyValuePair`1"/> doesn't properly serialize
	/// its contents even though it is marked <see cref="SerializableAttribute"/>.
	/// </remarks>
	[Serializable]
	public struct StringKeyValuePair
	{
		private string key;
		private string value;

		/// <summary>
		/// Initializes a new instance of the <see cref="StringKeyValuePair"/> struct.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public StringKeyValuePair(string key, string value)
		{
			this.key = key;
			this.value = value;
		}

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[XmlElement("Key")]
		public string Key
		{
			get { return (this.key); }
			set { this.key = value;  }
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[XmlElement("Value")]
		public string Value
		{
			get { return (this.value); }
			set { this.value = value;  }
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

			StringKeyValuePair other = (StringKeyValuePair)obj;
			return
			(
				(this.key   == other.key) &&
				(this.value == other.value)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				this.key  .GetHashCode() ^
				this.value.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(StringKeyValuePair lhs, StringKeyValuePair rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.System.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(StringKeyValuePair lhs, StringKeyValuePair rhs)
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
