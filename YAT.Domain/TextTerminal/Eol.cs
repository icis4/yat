using System;
using System.Collections.Generic;
using System.Text;

using HSR.Utilities.Types;

namespace HSR.YAT.Domain
{
	#region Enum Eol

	/// <summary></summary>
	public enum Eol
	{
		None,
		Cr,
		Lf,
		CrLf,
		LfCr,
		Tab,
		Nul,
	}

	#endregion

	/// <summary>
	/// Extended enum XEol.
	/// </summary>
	[Serializable]
	public class XEol : XEnum
	{
		#region String Definitions

		private const string None_string = "None";
		private const string None_stringSequence = "";

		private const string Cr_string = "<CR>";
		private const string Lf_string = "<LF>";
		private const string CrLf_string = "<CR><LF>";
		private const string LfCr_string = "<LF><CR>";
		private const string Tab_string = "<TAB>";
		private const string Nul_string = "<NUL>";

		#endregion

		/// <summary>Default is <see cref="Eol.CrLf"/></summary>
		public XEol()
			: base(Eol.CrLf)
		{
		}

		/// <summary></summary>
		protected XEol(Eol type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Eol)UnderlyingEnum)
			{
				case Eol.None: return (None_string);
				case Eol.Cr:   return (Cr_string);
				case Eol.Lf:   return (Lf_string);
				case Eol.CrLf: return (CrLf_string);
				case Eol.LfCr: return (LfCr_string);
				case Eol.Tab:  return (Tab_string);
				case Eol.Nul:  return (Nul_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		public string ToSequenceString()
		{
			switch ((Eol)UnderlyingEnum)
			{
				case Eol.None: return (None_stringSequence);
				case Eol.Cr:   return (Cr_string);
				case Eol.Lf:   return (Lf_string);
				case Eol.CrLf: return (CrLf_string);
				case Eol.LfCr: return (LfCr_string);
				case Eol.Tab:  return (Tab_string);
				case Eol.Nul:  return (Nul_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XEol[] GetItems()
		{
			List<XEol> a = new List<XEol>();
			a.Add(new XEol(Eol.None));
			a.Add(new XEol(Eol.Cr));
			a.Add(new XEol(Eol.Lf));
			a.Add(new XEol(Eol.CrLf));
			a.Add(new XEol(Eol.LfCr));
			a.Add(new XEol(Eol.Tab));
			a.Add(new XEol(Eol.Nul));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XEol Parse(string eol)
		{
			if      ((string.Compare(eol, None_string, true) == 0) ||
			         (string.Compare(eol, None_stringSequence, true) == 0))
			{
				return (new XEol(Eol.None));
			}
			else if (string.Compare(eol, Cr_string, true) == 0)
			{
				return (new XEol(Eol.Cr));
			}
			else if (string.Compare(eol, Lf_string, true) == 0)
			{
				return (new XEol(Eol.Lf));
			}
			else if (string.Compare(eol, CrLf_string, true) == 0)
			{
				return (new XEol(Eol.CrLf));
			}
			else if (string.Compare(eol, LfCr_string, true) == 0)
			{
				return (new XEol(Eol.LfCr));
			}
			else if (string.Compare(eol, Tab_string, true) == 0)
			{
				return (new XEol(Eol.Tab));
			}
			else if (string.Compare(eol, Nul_string, true) == 0)
			{
				return (new XEol(Eol.Nul));
			}

			throw (new ArgumentOutOfRangeException(eol));
		}

		#endregion

		#region FromString

		/// <summary></summary>
		public static XEol FromString(string eol)
		{
			if (eol == null)
			{
				return (new XEol(Eol.None));
			}
			else if (string.Compare(eol, "", true) == 0)
			{
				return (new XEol(Eol.None));
			}
			else if (string.Compare(eol, "\r", true) == 0)
			{
				return (new XEol(Eol.Cr));
			}
			else if (string.Compare(eol, "\n", true) == 0)
			{
				return (new XEol(Eol.Lf));
			}
			else if (string.Compare(eol, "\r\n", true) == 0)
			{
				return (new XEol(Eol.CrLf));
			}
			else if (string.Compare(eol, "\n\r", true) == 0)
			{
				return (new XEol(Eol.LfCr));
			}
			else if (string.Compare(eol, "\t", true) == 0)
			{
				return (new XEol(Eol.Tab));
			}
			else if (string.Compare(eol, "\0", true) == 0)
			{
				return (new XEol(Eol.Nul));
			}

			throw (new ArgumentOutOfRangeException(eol));
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Eol(XEol eol)
		{
			return ((Eol)eol.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XEol(Eol eol)
		{
			return (new XEol(eol));
		}

		/// <summary></summary>
		public static implicit operator int(XEol eol)
		{
			return (eol.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XEol(int eol)
		{
			return (new XEol((Eol)eol));
		}

		/// <summary></summary>
		public static implicit operator string(XEol eol)
		{
			return (eol.ToString());
		}

		/// <summary></summary>
		public static implicit operator XEol(string eol)
		{
			return (Parse(eol));
		}

		#endregion
	}
}
