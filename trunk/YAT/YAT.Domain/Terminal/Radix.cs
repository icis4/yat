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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum Radix

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum Radix
	{
		None,

		String,
		Char,

		Bin,
		Oct,
		Dec,
		Hex,

		Unicode
	}

	#pragma warning restore 1591

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

		private const string None_string      = "None";
		private const string None_stringValue = "None";
		private const string None_stringShort = "N";

		private const string String_string      = "String";
		private const string String_stringValue = "Str";
		private const string String_stringShort = "S";

		private const string Char_string      = "Character";
		private const string Char_stringValue = "Chr";
		private const string Char_stringShort = "C";

		private const string Bin_string      = "Binary";
		private const string Bin_stringShort = "B";
		private const string Bin_stringValue = "2";
		private const string Oct_string      = "Octal";
		private const string Oct_stringShort = "O";
		private const string Oct_stringValue = "8";
		private const string Dec_string      = "Decimal";
		private const string Dec_stringShort = "D";
		private const string Dec_stringValue = "10";
		private const string Hex_string      = "Hexadecimal";
		private const string Hex_stringShort = "H";
		private const string Hex_stringValue = "16";

		private const string Unicode_string      = "Unicode";
		private const string Unicode_stringShort = "U";
		private const string Unicode_stringValue = "U+";

		#endregion

		/// <summary>Default for text terminals is <see cref="Radix.String"/>.</summary>
		public const Radix TextDefault = Radix.String;

		/// <summary>Default for binary terminals is <see cref="Radix.Hex"/>.</summary>
		public const Radix BinaryDefault = Radix.Hex;

		/// <summary>
		/// Default is <see cref="TextDefault"/> as its value of
		/// <see cref="Radix.String"/> is more general.</summary>
		public const Radix Default = TextDefault;

		/// <summary>
		/// Default is <see cref="Default"/>.
		/// </summary>
		public RadixEx()
			: this(Default)
		{
		}

		/// <summary>
		/// Default is <see cref="TextDefault"/> for text
		/// and <see cref="BinaryDefault"/> for binary terminals.
		/// </summary>
		public RadixEx(TerminalType terminalType)
			: this((terminalType == TerminalType.Text) ? TextDefault : BinaryDefault)
		{
		}

		/// <summary></summary>
		public RadixEx(Radix radix)
			: base(radix)
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
			switch ((Radix)UnderlyingEnum)
			{
				case Radix.None:    return (None_string);
				case Radix.String:  return (String_string);
				case Radix.Char:    return (Char_string);
				case Radix.Bin:     return (Bin_string);
				case Radix.Oct:     return (Oct_string);
				case Radix.Dec:     return (Dec_string);
				case Radix.Hex:     return (Hex_string);
				case Radix.Unicode: return (Unicode_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is a radix that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			switch ((Radix)UnderlyingEnum)
			{
				case Radix.None:    return (None_stringShort);
				case Radix.String:  return (String_stringShort);
				case Radix.Char:    return (Char_stringShort);
				case Radix.Bin:     return (Bin_stringShort);
				case Radix.Oct:     return (Oct_stringShort);
				case Radix.Dec:     return (Dec_stringShort);
				case Radix.Hex:     return (Hex_stringShort);
				case Radix.Unicode: return (Unicode_stringShort);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is a radix that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToValueString()
		{
			switch ((Radix)UnderlyingEnum)
			{
				case Radix.None:    return (None_stringValue);
				case Radix.String:  return (String_stringValue);
				case Radix.Char:    return (Char_stringValue);
				case Radix.Bin:     return (Bin_stringValue);
				case Radix.Oct:     return (Oct_stringValue);
				case Radix.Dec:     return (Dec_stringValue);
				case Radix.Hex:     return (Hex_stringValue);
				case Radix.Unicode: return (Unicode_stringValue);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is a radix that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		public static RadixEx[] GetItems()
		{
			var a = new List<RadixEx>(7); // Preset the required capacity to improve memory management.

			// 'None' shall not be listed.

			// Alpha:
			a.Add(new RadixEx(Radix.String));
			a.Add(new RadixEx(Radix.Char));

			// Numeric:
			a.Add(new RadixEx(Radix.Bin));
			a.Add(new RadixEx(Radix.Oct));
			a.Add(new RadixEx(Radix.Dec));
			a.Add(new RadixEx(Radix.Hex));

			// Special:
			a.Add(new RadixEx(Radix.Unicode));

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static RadixEx Parse(string s)
		{
			RadixEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid radix string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out RadixEx result)
		{
			Radix enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new RadixEx(enumResult);
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

			// 'None' is located at end of if-else-if.

			if      (StringEx.EqualsOrdinalIgnoreCase(s, String_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, String_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, String_string))
			{
				result = Radix.String;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Char_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Char_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Char_string))
			{
				result = Radix.Char;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Bin_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Bin_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Bin_string))
			{
				result = Radix.Bin;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Oct_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Oct_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Oct_string))
			{
				result = Radix.Oct;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dec_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dec_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dec_string))
			{
				result = Radix.Dec;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Hex_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Hex_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Hex_string))
			{
				result = Radix.Hex;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Unicode_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Unicode_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Unicode_string))
			{
				result = Radix.Unicode;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringValue) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = Radix.None; // Last in if-else-if since this is the least used selection.
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
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
