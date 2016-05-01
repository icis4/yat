//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum Eol

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum InfoElementEnclosure
	{
		None,

		Parentheses,
		SquareBrackets,
		CurlyBraces
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum InfoElementEnclosureEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class InfoElementEnclosureEx : EnumEx
	{
		#region String Definitions

		private const string None_stringEnclosure                = "";
		private const string None_stringDescription              = "[None]";

		private const string Parentheses_stringEnclosure         = "()";
		private const string Parentheses_stringEnclosureLeft     = "(";
		private const string Parentheses_stringEnclosureRight    = ")";
		private const string Parentheses_stringDescription       = "Parentheses ()";

		private const string SquareBrackets_stringEnclosure      = "[]";
		private const string SquareBrackets_stringEnclosureLeft  = "[";
		private const string SquareBrackets_stringEnclosureRight = "]";
		private const string SquareBrackets_stringDescription    = "Brackets []";

		private const string CurlyBraces_stringEnclosure         = "{}";
		private const string CurlyBraces_stringEnclosureLeft     = "{";
		private const string CurlyBraces_stringEnclosureRight    = "}";
		private const string CurlyBraces_stringDescription       = "Braces {}";

		#endregion

		/// <summary>Default is <see cref="InfoElementEnclosure.None"/>.</summary>
		public InfoElementEnclosureEx()
			: this(InfoElementEnclosure.None)
		{
		}

		/// <summary></summary>
		public InfoElementEnclosureEx(InfoElementEnclosure enclosure)
			: base(enclosure)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (ToDescription());
		}

		/// <summary></summary>
		public virtual string ToEnclosure()
		{
			switch ((InfoElementEnclosure)UnderlyingEnum)
			{
				case InfoElementEnclosure.None:           return (None_stringEnclosure);

				case InfoElementEnclosure.Parentheses:    return (Parentheses_stringEnclosure);
				case InfoElementEnclosure.SquareBrackets: return (SquareBrackets_stringEnclosure);
				case InfoElementEnclosure.CurlyBraces:    return (CurlyBraces_stringEnclosure);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToEnclosureLeft()
		{
			switch ((InfoElementEnclosure)UnderlyingEnum)
			{
				case InfoElementEnclosure.None:           return (None_stringEnclosure);

				case InfoElementEnclosure.Parentheses:    return (Parentheses_stringEnclosureLeft);
				case InfoElementEnclosure.SquareBrackets: return (SquareBrackets_stringEnclosureLeft);
				case InfoElementEnclosure.CurlyBraces:    return (CurlyBraces_stringEnclosureLeft);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToEnclosureRight()
		{
			switch ((InfoElementEnclosure)UnderlyingEnum)
			{
				case InfoElementEnclosure.None:           return (None_stringEnclosure);

				case InfoElementEnclosure.Parentheses:    return (Parentheses_stringEnclosureRight);
				case InfoElementEnclosure.SquareBrackets: return (SquareBrackets_stringEnclosureRight);
				case InfoElementEnclosure.CurlyBraces:    return (CurlyBraces_stringEnclosureRight);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((InfoElementEnclosure)UnderlyingEnum)
			{
				case InfoElementEnclosure.None:           return (None_stringDescription);

				case InfoElementEnclosure.Parentheses:    return (Parentheses_stringDescription);
				case InfoElementEnclosure.SquareBrackets: return (SquareBrackets_stringDescription);
				case InfoElementEnclosure.CurlyBraces:    return (CurlyBraces_stringDescription);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static InfoElementEnclosureEx[] GetItems()
		{
			List<InfoElementEnclosureEx> a = new List<InfoElementEnclosureEx>(4); // Preset the required capactiy to improve memory management.
			a.Add(new InfoElementEnclosureEx(InfoElementEnclosure.None));
			a.Add(new InfoElementEnclosureEx(InfoElementEnclosure.Parentheses));
			a.Add(new InfoElementEnclosureEx(InfoElementEnclosure.SquareBrackets));
			a.Add(new InfoElementEnclosureEx(InfoElementEnclosure.CurlyBraces));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static InfoElementEnclosure Parse(string s)
		{
			InfoElementEnclosure result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid info element enclosure."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out InfoElementEnclosureEx result)
		{
			InfoElementEnclosure enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = enumResult;
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out InfoElementEnclosure result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, None_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringDescription))
			{
				result = InfoElementEnclosure.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_stringEnclosureLeft) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_stringEnclosureRight) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_stringDescription))
			{
				result = InfoElementEnclosure.Parentheses;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_stringEnclosureLeft) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_stringEnclosureRight) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_stringDescription))
			{
				result = InfoElementEnclosure.SquareBrackets;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_stringEnclosureLeft) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_stringEnclosureRight) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_stringDescription))
			{
				result = InfoElementEnclosure.CurlyBraces;
				return (true);
			}
			else if (string.IsNullOrEmpty(s))
			{
				result = new InfoElementEnclosureEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else // Invalid string!
			{
				result = new InfoElementEnclosureEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator InfoElementEnclosure(InfoElementEnclosureEx enclosure)
		{
			return ((InfoElementEnclosure)enclosure.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator InfoElementEnclosureEx(InfoElementEnclosure enclosure)
		{
			return (new InfoElementEnclosureEx(enclosure));
		}

		/// <summary></summary>
		public static implicit operator string(InfoElementEnclosureEx enclosure)
		{
			return (enclosure.ToString());
		}

		/// <summary></summary>
		public static implicit operator InfoElementEnclosureEx(string enclosure)
		{
			return (Parse(enclosure));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
