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
	#region Enum Radix

	/// <summary></summary>
	public enum Radix
	{
		/// <summary></summary>
		None,

		/// <summary></summary>
		String,

		/// <summary></summary>
		Char,

		/// <summary></summary>
		Bin,

		/// <summary></summary>
		Oct,

		/// <summary></summary>
		Dec,

		/// <summary></summary>
		Hex
	}

	#endregion

	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class RadixEx : EnumEx
	{
		#region String Definitions

		private const string Bin_stringShort  = "B";
		private const string Bin_stringMiddle = "Bin";
		private const string Bin_string       = "Binary";
		private const string Oct_stringShort  = "O";
		private const string Oct_stringMiddle = "Oct";
		private const string Oct_string       = "Octal";
		private const string Dec_stringShort  = "D";
		private const string Dec_stringMiddle = "Dec";
		private const string Dec_string       = "Decimal";
		private const string Hex_stringShort  = "H";
		private const string Hex_stringMiddle = "Hex";
		private const string Hex_string       = "Hexadecimal";
		private const string Char_stringShort  = "C";
		private const string Char_stringMiddle = "Chr";
		private const string Char_string       = "Character";
		private const string String_stringShort  = "S";
		private const string String_stringMiddle = "Str";
		private const string String_string       = "String";

		#endregion

		/// <summary>Default is <see cref="Radix.String"/>.</summary>
		public const Radix Default = Radix.String;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public RadixEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public RadixEx(Radix radix)
			: base(radix)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((Radix)UnderlyingEnum)
			{
				case Radix.Bin:    return (Bin_string);
				case Radix.Oct:    return (Oct_string);
				case Radix.Dec:    return (Dec_string);
				case Radix.Hex:    return (Hex_string);
				case Radix.Char:   return (Char_string);
				case Radix.String: return (String_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			switch ((Radix)UnderlyingEnum)
			{
				case Radix.Bin:    return (Bin_stringShort);
				case Radix.Oct:    return (Oct_stringShort);
				case Radix.Dec:    return (Dec_stringShort);
				case Radix.Hex:    return (Hex_stringShort);
				case Radix.Char:   return (Char_stringShort);
				case Radix.String: return (String_stringShort);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToMiddleString()
		{
			switch ((Radix)UnderlyingEnum)
			{
				case Radix.Bin:    return (Bin_stringMiddle);
				case Radix.Oct:    return (Oct_stringMiddle);
				case Radix.Dec:    return (Dec_stringMiddle);
				case Radix.Hex:    return (Hex_stringMiddle);
				case Radix.Char:   return (Char_stringMiddle);
				case Radix.String: return (String_stringMiddle);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static RadixEx[] GetItems()
		{
			List<RadixEx> a = new List<RadixEx>(6); // Preset the required capacity to improve memory management.

			a.Add(new RadixEx(Radix.String)); // Alpha radices.
			a.Add(new RadixEx(Radix.Char));

			a.Add(new RadixEx(Radix.Bin)); // Numeric radices.
			a.Add(new RadixEx(Radix.Oct));
			a.Add(new RadixEx(Radix.Dec));
			a.Add(new RadixEx(Radix.Hex));

			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static RadixEx Parse(string s)
		{
			RadixEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid radix string! String must one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out RadixEx result)
		{
			Radix enumResult;
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
		public static bool TryParse(string s, out Radix result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = new RadixEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Bin_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Bin_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Bin_string))
			{
				result = Radix.Bin;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Oct_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Oct_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Oct_string))
			{
				result = Radix.Oct;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dec_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dec_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dec_string))
			{
				result = Radix.Dec;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Hex_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Hex_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Hex_string))
			{
				result = Radix.Hex;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Char_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Char_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Char_string))
			{
				result = Radix.Char;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, String_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, String_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, String_string))
			{
				result = Radix.String;
				return (true);
			}
			else // Invalid string!
			{
				result = new RadixEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Radix(RadixEx radix)
		{
			return ((Radix)radix.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator RadixEx(Radix radix)
		{
			return (new RadixEx(radix));
		}

		/// <summary></summary>
		public static implicit operator int(RadixEx radix)
		{
			return (radix.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator RadixEx(int radix)
		{
			return (new RadixEx((Radix)radix));
		}

		/// <summary></summary>
		public static implicit operator string(RadixEx radix)
		{
			return (radix.ToString());
		}

		/// <summary></summary>
		public static implicit operator RadixEx(string radix)
		{
			return (Parse(radix));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
