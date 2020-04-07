//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

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

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "CR", Justification = "<CR> is an ASCII mnemonic.")]
		CR,

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "LF", Justification = "<LF> is an ASCII mnemonic.")]
		LF,

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "CRLF", Justification = "<CR><LF> are ASCII mnemonics.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CRLF", Justification = "Same as above.")]
		CRLF,

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "LFCR", Justification = "<LF><CR> are ASCII mnemonics.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LFCR", Justification = "Same as above.")]
		LFCR,

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nul", Justification = "<NUL> is an ASCII mnemonic.")]
		Nul,

		Tab,
		Space
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
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class EolEx : EnumEx
	{
		#region String Definitions

		private const string None_stringNice = "[None]";
		private const string None_stringOld1 =  "None";
		private const string None_stringSequence = "";

		private const string CR_stringSequence = "<CR>";
		private const string CR_stringNative = "\r";

		private const string LF_stringSequence = "<LF>";
		private const string LF_stringNative = "\n";

		private const string CRLF_stringSequence = "<CR><LF>";
		private const string CRLF_stringNative = "\r\n";

		private const string LFCR_stringSequence = "<LF><CR>";
		private const string LFCR_stringNative = "\n\r";

		private const string Nul_stringSequence = "<NUL>";
		private const string Nul_stringNative = "\0";

		private const string Tab_stringSequence = "<TAB>";
		private const string Tab_stringNative = "\t";

		private const string Space_stringNice = "[Space]";
		private const string Space_stringOld1 =  "Space";
		private const string Space_stringSequence = " ";

		#endregion

		/// <summary>Default is <see cref="Eol.CRLF"/>.</summary>
		public const Eol Default = Eol.CRLF;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public EolEx()
			: base(Default)
		{
		}

		/// <summary></summary>
		public EolEx(Eol type)
			: base(type)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((Eol)UnderlyingEnum)
			{
				case Eol.None:  return ( None_stringNice);
				case Eol.CR:    return (   CR_stringSequence);
				case Eol.LF:    return (   LF_stringSequence);
				case Eol.CRLF:  return ( CRLF_stringSequence);
				case Eol.LFCR:  return ( LFCR_stringSequence);
				case Eol.Nul:   return (  Nul_stringSequence);
				case Eol.Tab:   return (  Tab_stringSequence);
				case Eol.Space: return (Space_stringNice);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>Named ..String() to emphasize that this is not the binary sequence.</remarks>
		public virtual string ToSequenceString()
		{
			switch ((Eol)UnderlyingEnum)
			{
				case Eol.None:  return ( None_stringSequence);
				case Eol.CR:    return (   CR_stringSequence);
				case Eol.LF:    return (   LF_stringSequence);
				case Eol.CRLF:  return ( CRLF_stringSequence);
				case Eol.LFCR:  return ( LFCR_stringSequence);
				case Eol.Nul:   return (  Nul_stringSequence);
				case Eol.Tab:   return (  Tab_stringSequence);
				case Eol.Space: return (Space_stringSequence);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static EolEx[] GetItems()
		{
			var a = new List<EolEx>(8); // Preset the required capacity to improve memory management.

			a.Add(new EolEx(Eol.None));
			a.Add(new EolEx(Eol.CR));
			a.Add(new EolEx(Eol.LF));
			a.Add(new EolEx(Eol.CRLF));
			a.Add(new EolEx(Eol.LFCR));
			a.Add(new EolEx(Eol.Nul));
			a.Add(new EolEx(Eol.Tab));
			a.Add(new EolEx(Eol.Space));

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as all EOL sequences are whitespaces.
		/// </remarks>
		public static EolEx Parse(string s)
		{
			EolEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid EOL string! String must be a valid EOL sequence."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out EolEx result)
		{
			Eol enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new EolEx(enumResult);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as all EOL sequences are whitespaces.
		/// </remarks>
		public static bool TryParse(string s, out Eol result)
		{
			// Do not s = s.Trim(); due to reason described above.

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = Eol.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_stringNice) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringSequence))
			{
				result = Eol.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CR_stringSequence) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CR_stringNative))
			{
				result = Eol.CR;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LF_stringSequence) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, LF_stringNative))
			{
				result = Eol.LF;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CRLF_stringSequence) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CRLF_stringNative))
			{
				result = Eol.CRLF;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LFCR_stringSequence) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, LFCR_stringNative))
			{
				result = Eol.LFCR;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Nul_stringSequence) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Nul_stringNative))
			{
				result = Eol.Nul;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Tab_stringSequence) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Tab_stringNative))
			{
				result = Eol.Tab;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Space_stringNice) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_stringOld1) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_stringSequence))
			{
				result = Eol.Space;
				return (true);
			}
			else // Invalid string!
			{
				result = new EolEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
