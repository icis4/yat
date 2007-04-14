using System;
using System.Collections.Generic;
using System.Text;

using HSR.Utilities.Types;

namespace HSR.YAT.Domain
{
	#region Enum CharSubstitution

	/// <summary></summary>
	public enum CharSubstitution
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		ToUpper,
		/// <summary></summary>
		ToLower,
	}

	#endregion

	/// <summary>
	/// Extended enum XCharSubstitution.
	/// </summary>
	[Serializable]
	class XCharSubstitution : XEnum
	{
		#region String Definitions

		private const string None_string = "None (mixed case)";
		private const string ToUpper_string = "Upper case";
		private const string ToLower_string = "Lower case";

		#endregion

		/// <summary>Default is <see cref="CharSubstitution.None"/></summary>
		public XCharSubstitution()
			: base(CharSubstitution.None)
		{
		}

		/// <summary></summary>
		protected XCharSubstitution(CharSubstitution substitution)
			: base(substitution)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((CharSubstitution)UnderlyingEnum)
			{
				case CharSubstitution.None:    return (None_string);
				case CharSubstitution.ToUpper: return (ToUpper_string);
				case CharSubstitution.ToLower: return (ToLower_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XCharSubstitution[] GetItems()
		{
			List<XCharSubstitution> a = new List<XCharSubstitution>();
			a.Add(new XCharSubstitution(CharSubstitution.None));
			a.Add(new XCharSubstitution(CharSubstitution.ToUpper));
			a.Add(new XCharSubstitution(CharSubstitution.ToLower));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XCharSubstitution Parse(string substitution)
		{
			if      (string.Compare(substitution, None_string, true) == 0)
			{
				return (new XCharSubstitution(CharSubstitution.None));
			}
			else if (string.Compare(substitution, ToUpper_string, true) == 0)
			{
				return (new XCharSubstitution(CharSubstitution.ToUpper));
			}
			else if (string.Compare(substitution, ToLower_string, true) == 0)
			{
				return (new XCharSubstitution(CharSubstitution.ToLower));
			}

			throw (new ArgumentOutOfRangeException(substitution));
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator CharSubstitution(XCharSubstitution substitution)
		{
			return ((CharSubstitution)substitution.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XCharSubstitution(CharSubstitution substitution)
		{
			return (new XCharSubstitution(substitution));
		}

		/// <summary></summary>
		public static implicit operator int(XCharSubstitution substitution)
		{
			return (substitution.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XCharSubstitution(int substitution)
		{
			return (new XCharSubstitution((CharSubstitution)substitution));
		}

		/// <summary></summary>
		public static implicit operator string(XCharSubstitution substitution)
		{
			return (substitution.ToString());
		}

		/// <summary></summary>
		public static implicit operator XCharSubstitution(string substitution)
		{
			return (Parse(substitution));
		}

		#endregion
	}
}
