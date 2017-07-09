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
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum ControlCharRadix

	/// <summary></summary>
	public enum ControlCharRadix
	{
		/// <summary></summary>
		None = Radix.None,

		/// <remarks>
		/// Keep this item to ensure that items can 1:1 be mapped to <see cref="Radix"/>
		/// and that additional items get distinct enum identifiers.
		/// </remarks>
		[Obsolete("String makes no sense for single byte/character replacement.")]
		String = Radix.String,

		/// <summary></summary>
		Char = Radix.Char,

		/// <summary></summary>
		Bin = Radix.Bin,

		/// <summary></summary>
		Oct = Radix.Oct,

		/// <summary></summary>
		Dec = Radix.Dec,

		/// <summary></summary>
		Hex = Radix.Hex,

		/// <remarks>
		/// Keep this item to ensure that items can 1:1 be mapped to <see cref="Radix"/>
		/// and that additional items get distinct enum identifiers.
		/// </remarks>
		[Obsolete("Unicode makes no sense for control character replacement.")]
		Unicode = Radix.Unicode,

		/// <summary></summary>
		AsciiMnemonic,
	}

	#endregion

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class ControlCharRadixEx : RadixEx
	{
		#region String Definitions

		private const string AsciiMnemonic_string = "ASCII mnemonics";

		#endregion

		/// <summary></summary>
		protected ControlCharRadixEx()
			: base()
		{
		}

		/// <summary></summary>
		protected ControlCharRadixEx(ControlCharRadix radix)
			: base((Radix)radix)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			switch ((ControlCharRadix)UnderlyingEnum)
			{
				case ControlCharRadix.AsciiMnemonic: return (AsciiMnemonic_string);
				default:                             return (base.ToString());
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static new ControlCharRadixEx[] GetItems()
		{
			RadixEx[] radices = RadixEx.GetItems();
			List<ControlCharRadixEx> items = new List<ControlCharRadixEx>(radices.Length - 1); // Preset the required capacity to improve memory management.

			// Re-use items from base:
			foreach (RadixEx radix in radices)
			{
				if (radix == Radix.String) // String makes no sense for single byte/character replacement.
					continue;              // See remark for 'ControlCharRadix.String' for details.

				if (radix == Radix.Unicode) // Unicode makes no sense for single byte/character replacement.
					continue;               // See remark for 'ControlCharRadix.Unicode' for details.

				items.Add((Radix)radix);
			}

			// Add additional items:
			items.Add(new ControlCharRadixEx(ControlCharRadix.AsciiMnemonic));

			return (items.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static new ControlCharRadixEx Parse(string s)
		{
			ControlCharRadixEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid control char radix string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out ControlCharRadixEx result)
		{
			ControlCharRadix enumResult;
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
		public static bool TryParse(string s, out ControlCharRadix result)
		{
			if (s != null)
				s = s.Trim();

			if (StringEx.EqualsOrdinalIgnoreCase(s, AsciiMnemonic_string))
			{
				result = new ControlCharRadixEx(ControlCharRadix.AsciiMnemonic);
				return (false);
			}
			else
			{
				RadixEx radix;
				if (TryParse(s, out radix))
				{
					result = (ControlCharRadixEx)radix;
					return (true);
				}
				else // Invalid string!
				{
					result = new ControlCharRadixEx(); // Default!
					return (true);
				}
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator ControlCharRadix(ControlCharRadixEx radix)
		{
			return ((ControlCharRadix)radix.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator ControlCharRadixEx(ControlCharRadix radix)
		{
			return (new ControlCharRadixEx(radix));
		}

		/// <summary></summary>
		public static explicit operator Radix(ControlCharRadixEx radix)
		{
			return ((Radix)radix.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator ControlCharRadixEx(Radix radix)
		{
			return (new ControlCharRadixEx((ControlCharRadix)radix));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
