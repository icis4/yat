//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum Eol

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

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

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum EolEx.
	/// </summary>
	/// <remarks>
	/// LF:   Unix, Linux, Mac OS from version X, AmigaOS, BSD
	/// CRLF: Windows, DOS, OS/2, CP/M
	/// CR:   Mac OS up to version 9, Apple II
	/// 
	/// Saying hello to StyleCop ;-.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class EolEx : EnumEx
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

		/// <summary>Default is <see cref="Eol.CrLf"/>.</summary>
		public EolEx()
			: base(Eol.CrLf)
		{
		}

		/// <summary></summary>
		protected EolEx(Eol type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
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
			throw (new InvalidOperationException("Code execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug"));
		}

		/// <summary></summary>
		public virtual string ToSequenceString()
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
			throw (new InvalidOperationException("Code execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static EolEx[] GetItems()
		{
			List<EolEx> a = new List<EolEx>();
			a.Add(new EolEx(Eol.None));
			a.Add(new EolEx(Eol.Cr));
			a.Add(new EolEx(Eol.Lf));
			a.Add(new EolEx(Eol.CrLf));
			a.Add(new EolEx(Eol.LfCr));
			a.Add(new EolEx(Eol.Tab));
			a.Add(new EolEx(Eol.Nul));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static EolEx Parse(string s)
		{
			EolEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException("'" + s + "' is no valid EOL string"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out EolEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, None_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringSequence))
			{
				result = new EolEx(Eol.None);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Cr_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Cr_stringNative))
			{
				result = new EolEx(Eol.Cr);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Lf_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Lf_stringNative))
			{
				result = new EolEx(Eol.Lf);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CrLf_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CrLf_stringNative))
			{
				result = new EolEx(Eol.CrLf);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LfCr_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, LfCr_stringNative))
			{
				result = new EolEx(Eol.LfCr);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Tab_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Tab_stringNative))
			{
				result = new EolEx(Eol.Tab);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Nul_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Nul_stringNative))
			{
				result = new EolEx(Eol.Nul);
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
		public static implicit operator Eol(EolEx eol)
		{
			return ((Eol)eol.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator EolEx(Eol eol)
		{
			return (new EolEx(eol));
		}

		/// <summary></summary>
		public static implicit operator int(EolEx eol)
		{
			return (eol.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator EolEx(int eol)
		{
			return (new EolEx((Eol)eol));
		}

		/// <summary></summary>
		public static implicit operator string(EolEx eol)
		{
			return (eol.ToString());
		}

		/// <summary></summary>
		public static implicit operator EolEx(string eol)
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
