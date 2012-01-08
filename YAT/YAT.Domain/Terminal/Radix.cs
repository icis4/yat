//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
	#region Enum Radix

	/// <summary></summary>
	public enum Radix
	{
		/// <summary></summary>
		None,

		/// <summary></summary>
		Bin,

		/// <summary></summary>
		Oct,

		/// <summary></summary>
		Dec,

		/// <summary></summary>
		Hex,

		/// <summary></summary>
		Char,

		/// <summary></summary>
		String,
	}

	#endregion

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class RadixEx : MKY.EnumEx
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
		public RadixEx()
			: base(Radix.String)
		{
		}

		/// <summary></summary>
		protected RadixEx(Radix radix)
			: base(radix)
		{
		}

		#region ToString

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
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
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
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		/// <summary></summary>
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
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static RadixEx[] GetItems()
		{
			List<RadixEx> a = new List<RadixEx>();
			a.Add(new RadixEx(Radix.Bin));
			a.Add(new RadixEx(Radix.Oct));
			a.Add(new RadixEx(Radix.Dec));
			a.Add(new RadixEx(Radix.Hex));
			a.Add(new RadixEx(Radix.Char));
			a.Add(new RadixEx(Radix.String));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static RadixEx Parse(string radix)
		{
			RadixEx result;

			if (TryParse(radix, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("radix", radix, "Invalid radix."));
		}

		/// <summary></summary>
		public static bool TryParse(string radix, out RadixEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(radix, Bin_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Bin_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Bin_string))
			{
				result = new RadixEx(Radix.Bin);
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(radix, Oct_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Oct_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Oct_string))
			{
				result = new RadixEx(Radix.Oct);
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(radix, Dec_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Dec_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Dec_string))
			{
				result = new RadixEx(Radix.Dec);
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(radix, Hex_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Hex_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Hex_string))
			{
				result = new RadixEx(Radix.Hex);
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(radix, Char_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Char_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, Char_string))
			{
				result = new RadixEx(Radix.Char);
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(radix, String_stringShort) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, String_stringMiddle) ||
			         StringEx.EqualsOrdinalIgnoreCase(radix, String_string))
			{
				result = new RadixEx(Radix.String);
				return (false);
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
