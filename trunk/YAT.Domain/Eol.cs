//==================================================================================================
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

using MKY.Utilities.Types;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum Eol

	/// <summary></summary>
	public enum Eol
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		Cr,
		/// <summary></summary>
		Lf,
		/// <summary></summary>
		CrLf,
		/// <summary></summary>
		LfCr,
		/// <summary></summary>
		Tab,
		/// <summary></summary>
		Nul,
	}

	#endregion

	/// <summary>
	/// Extended enum XEol.
	/// </summary>
	/// <remarks>
	/// LF:   Unix, Linux, Mac OS from version X, AmigaOS, BSD
	/// CRLF: Windows, DOS, OS/2, CP/M
	/// CR:   Mac OS up to version 9, Apple II
	/// </remarks>
	public class XEol : XEnum
	{
		#region String Definitions

		private const string None_string = "None";
		private const string None_stringSequence = "";

		private const string Cr_string = "<CR>";
		private const string Cr_stringNative = "\r";

		private const string Lf_string = "<LF>";
		private const string Lf_stringNative = "\n";

		private const string CrLf_string = "<CR><LF>";
		private const string CrLf_stringNative = "\r\n";
		
		private const string LfCr_string = "<LF><CR>";
		private const string LfCr_stringNative = "\n\r";
		
		private const string Tab_string = "<TAB>";
		private const string Tab_stringNative = "\t";
		
		private const string Nul_string = "<NUL>";
		private const string Nul_stringNative = "\0";

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

		/// <summary></summary>
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
			XEol result;

			if (TryParse(eol, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("eol", eol, "Invalid EOL."));
		}

		/// <summary></summary>
		public static bool TryParse(string eol, out XEol result)
		{
			if      ((string.Compare(eol, None_string, true) == 0) ||
					 (string.Compare(eol, None_stringSequence, true) == 0))
			{
				result = new XEol(Eol.None);
				return (true);
			}
			else if ((string.Compare(eol, Cr_string, true) == 0) ||
					 (string.Compare(eol, Cr_stringNative, true) == 0))
			{
				result = new XEol(Eol.Cr);
				return (true);
			}
			else if ((string.Compare(eol, Lf_string, true) == 0) ||
					 (string.Compare(eol, Lf_stringNative, true) == 0))
			{
				result = new XEol(Eol.Lf);
				return (true);
			}
			else if ((string.Compare(eol, CrLf_string, true) == 0) ||
					 (string.Compare(eol, CrLf_stringNative, true) == 0))
			{
				result = new XEol(Eol.CrLf);
				return (true);
			}
			else if ((string.Compare(eol, LfCr_string, true) == 0) ||
					 (string.Compare(eol, LfCr_stringNative, true) == 0))
			{
				result = new XEol(Eol.LfCr);
				return (true);
			}
			else if ((string.Compare(eol, Tab_string, true) == 0) ||
					 (string.Compare(eol, Tab_stringNative, true) == 0))
			{
				result = new XEol(Eol.Tab);
				return (true);
			}
			else if ((string.Compare(eol, Nul_string, true) == 0) ||
				     (string.Compare(eol, Nul_stringNative, true) == 0))
			{
				result = new XEol(Eol.Nul);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
