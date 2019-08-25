﻿//==================================================================================================
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

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum ControlCharRadix

	/// <summary></summary>
	public enum ControlCharRadix
	{
		/// <summary></summary>
		None = Radix.None,

		/// <summary></summary>
		Bin = Radix.Bin,

		/// <summary></summary>
		Oct = Radix.Oct,

		/// <summary></summary>
		Dec = Radix.Dec,

		/// <summary></summary>
		Hex = Radix.Hex,

		/// <summary></summary>
		Chr = Radix.Char,

		/// <summary></summary>
		Str = Radix.String,

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
		protected ControlCharRadixEx(ControlCharRadix radix)
			: base((Radix)radix)
		{
		}

		#region ToString

		/// <summary></summary>
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

		/// <summary></summary>
		public static new ControlCharRadixEx[] GetItems()
		{
			List<ControlCharRadixEx> items = new List<ControlCharRadixEx>();
			foreach (RadixEx radix in RadixEx.GetItems())
			{
				items.Add((ControlCharRadixEx)((Radix)radix));
			}
			items.Add(new ControlCharRadixEx(ControlCharRadix.AsciiMnemonic));
			return (items.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static new ControlCharRadixEx Parse(string s)
		{
			ControlCharRadixEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid control char radix string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out ControlCharRadixEx result)
		{
			s = s.Trim();

			if (StringEx.EqualsOrdinalIgnoreCase(s, AsciiMnemonic_string))
			{
				result = new ControlCharRadixEx(ControlCharRadix.AsciiMnemonic);
				return (false);
			}
			else
			{
				RadixEx radix;
				bool ret = RadixEx.TryParse(s, out radix);
				result = (ControlCharRadixEx)radix;
				return (ret);
			}
		}

		#endregion

		#region Conversion Operators

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