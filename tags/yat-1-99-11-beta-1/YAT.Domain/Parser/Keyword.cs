using System;
using System.Collections.Generic;
using System.Text;

using HSR.Utilities.Types;

namespace HSR.YAT.Domain.Parser
{
	#region Enum Keyword

	/// <summary></summary>
	public enum Keyword
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		Delay,
		/// <summary></summary>
		Eol,
		/// <summary></summary>
		NoEol,
	}

	#endregion

	/// <summary>
	/// Extended enum XKeyword.
	/// </summary>
	[Serializable]
	class XKeyword : XEnum
	{
		#region String Definitions

		private const string Delay_string = "Delay";
		private const string Eol_string = "EOL";
		private const string NoEol_string = "NoEOL";

		#endregion

		/// <summary>Default is <see cref="Keyword.None"/></summary>
		public XKeyword()
			: base(Keyword.None)
		{
		}

		/// <summary></summary>
		protected XKeyword(Keyword keyword)
			: base(keyword)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Delay: return (Delay_string);
				case Keyword.Eol:   return (Eol_string);
				case Keyword.NoEol: return (NoEol_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XKeyword[] GetItems()
		{
			List<XKeyword> a = new List<XKeyword>();
			a.Add(new XKeyword(Keyword.Delay));
			a.Add(new XKeyword(Keyword.Eol));
			a.Add(new XKeyword(Keyword.NoEol));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XKeyword Parse(string keyword)
		{
			if      (string.Compare(keyword, Delay_string, true) == 0)
			{
				return (new XKeyword(Keyword.Delay));
			}
			else if (string.Compare(keyword, Eol_string, true) == 0)
			{
				return (new XKeyword(Keyword.Eol));
			}
			else if (string.Compare(keyword, NoEol_string, true) == 0)
			{
				return (new XKeyword(Keyword.NoEol));
			}
			else
			{
				return (new XKeyword(Keyword.None));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Keyword(XKeyword keyword)
		{
			return ((Keyword)keyword.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XKeyword(Keyword keyword)
		{
			return (new XKeyword(keyword));
		}

		/// <summary></summary>
		public static implicit operator int(XKeyword keyword)
		{
			return (keyword.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XKeyword(int keyword)
		{
			return (new XKeyword((Keyword)keyword));
		}

		/// <summary></summary>
		public static implicit operator string(XKeyword keyword)
		{
			return (keyword.ToString());
		}

		/// <summary></summary>
		public static implicit operator XKeyword(string keyword)
		{
			return (Parse(keyword));
		}

		#endregion
	}
}
