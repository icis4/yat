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
// YAT 2.0 Gamma 1 Version 1.99.32
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

using MKY;

namespace YAT.Domain.Parser
{
	#region Enum Keyword

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum Keyword
	{
		None,
		Clear,
		Delay,
		LineDelay,
		LineRepeat,
		Eol,
		NoEol,
		OutputBreakOn,
		OutputBreakOff,
		OutputBreakToggle,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum KeywordEx.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class KeywordEx : EnumEx
	{
		#region String Definitions

		private const string Clear_string             = "Clear";
		private const string Delay_string             = "Delay";
		private const string LineDelay_string         = "LineDelay";
		private const string LineRepeat_string        = "LineRepeat";
		private const string Eol_string               = "EOL";
		private const string NoEol_string             = "NoEOL";
		private const string OutputBreakOn_string     = "OutputBreakOn";
		private const string OutputBreakOff_string    = "OutputBreakOff";
		private const string OutputBreakToggle_string = "OutputBreakToggle";

		#endregion

		/// <summary>Default is <see cref="Keyword.None"/>.</summary>
		public KeywordEx()
			: base(Keyword.None)
		{
		}

		/// <summary></summary>
		protected KeywordEx(Keyword keyword)
			: base(keyword)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Clear:      return (Clear_string);
				case Keyword.Delay:      return (Delay_string);
				case Keyword.LineDelay:  return (LineDelay_string);
				case Keyword.LineRepeat: return (LineRepeat_string);

				case Keyword.Eol:   return (Eol_string);
				case Keyword.NoEol: return (NoEol_string);

				case Keyword.OutputBreakOn:     return (OutputBreakOn_string);
				case Keyword.OutputBreakOff:    return (OutputBreakOff_string);
				case Keyword.OutputBreakToggle: return (OutputBreakToggle_string);
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static KeywordEx[] GetItems()
		{
			List<KeywordEx> a = new List<KeywordEx>();
			a.Add(new KeywordEx(Keyword.Clear));
			a.Add(new KeywordEx(Keyword.Delay));
			a.Add(new KeywordEx(Keyword.LineDelay));
			a.Add(new KeywordEx(Keyword.LineRepeat));
			a.Add(new KeywordEx(Keyword.Eol));
			a.Add(new KeywordEx(Keyword.NoEol));
			a.Add(new KeywordEx(Keyword.OutputBreakOn));
			a.Add(new KeywordEx(Keyword.OutputBreakOff));
			a.Add(new KeywordEx(Keyword.OutputBreakToggle));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static KeywordEx Parse(string s)
		{
			KeywordEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid keyword string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out KeywordEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Clear_string))
			{
				result = new KeywordEx(Keyword.Clear);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Delay_string))
			{
				result = new KeywordEx(Keyword.Delay);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineDelay_string))
			{
				result = new KeywordEx(Keyword.LineDelay);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineRepeat_string))
			{
				result = new KeywordEx(Keyword.LineRepeat);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Eol_string))
			{
				result = new KeywordEx(Keyword.Eol);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, NoEol_string))
			{
				result = new KeywordEx(Keyword.NoEol);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakOn_string))
			{
				result = new KeywordEx(Keyword.OutputBreakOn);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakOff_string))
			{
				result = new KeywordEx(Keyword.OutputBreakOff);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakToggle_string))
			{
				result = new KeywordEx(Keyword.OutputBreakToggle);
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
		public static implicit operator Keyword(KeywordEx keyword)
		{
			return ((Keyword)keyword.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(Keyword keyword)
		{
			return (new KeywordEx(keyword));
		}

		/// <summary></summary>
		public static implicit operator int(KeywordEx keyword)
		{
			return (keyword.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(int keyword)
		{
			return (new KeywordEx((Keyword)keyword));
		}

		/// <summary></summary>
		public static implicit operator string(KeywordEx keyword)
		{
			return (keyword.ToString());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(string keyword)
		{
			return (Parse(keyword));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
