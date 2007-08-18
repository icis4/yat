using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Types;

namespace MKY.YAT.Domain
{
	#region Enum Radix

	/// <summary></summary>
	public enum Radix
	{
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
	[Serializable]
	public class XRadix : XEnum
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

		/// <summary>Default is <see cref="Radix.String"/></summary>
		public XRadix()
			: base(Radix.String)
		{
		}

		/// <summary></summary>
		protected XRadix(Radix radix)
			: base(radix)
		{
		}

		#region ToString

		/// <summary></summary>
		public string ToShortString()
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
		public string ToMiddleString()
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
		public static XRadix[] GetItems()
		{
			List<XRadix> a = new List<XRadix>();
			a.Add(new XRadix(Radix.Bin));
			a.Add(new XRadix(Radix.Oct));
			a.Add(new XRadix(Radix.Dec));
			a.Add(new XRadix(Radix.Hex));
			a.Add(new XRadix(Radix.Char));
			a.Add(new XRadix(Radix.String));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XRadix Parse(string radix)
		{
			XRadix result;

			if (TryParse(radix, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException(radix));
		}

		/// <summary></summary>
		public static bool TryParse(string radix, out XRadix result)
		{
			if      ((string.Compare(radix, Bin_stringShort, true) == 0) ||
			         (string.Compare(radix, Bin_stringMiddle, true) == 0) ||
			         (string.Compare(radix, Bin_string, true) == 0))
			{
				result = new XRadix(Radix.Bin);
				return (false);
			}
			else if ((string.Compare(radix, Oct_stringShort, true) == 0) ||
			         (string.Compare(radix, Oct_stringMiddle, true) == 0) ||
			         (string.Compare(radix, Oct_string, true) == 0))
			{
				result = new XRadix(Radix.Oct);
				return (false);
			}
			else if ((string.Compare(radix, Dec_stringShort, true) == 0) ||
			         (string.Compare(radix, Dec_stringMiddle, true) == 0) ||
			         (string.Compare(radix, Dec_string, true) == 0))
			{
				result = new XRadix(Radix.Dec);
				return (false);
			}
			else if ((string.Compare(radix, Hex_stringShort, true) == 0) ||
			         (string.Compare(radix, Hex_stringMiddle, true) == 0) ||
			         (string.Compare(radix, Hex_string, true) == 0))
			{
				result = new XRadix(Radix.Hex);
				return (false);
			}
			else if ((string.Compare(radix, Char_stringShort, true) == 0) ||
			         (string.Compare(radix, Char_stringMiddle, true) == 0) ||
			         (string.Compare(radix, Char_string, true) == 0))
			{
				result = new XRadix(Radix.Char);
				return (false);
			}
			else if ((string.Compare(radix, String_stringShort, true) == 0) ||
			         (string.Compare(radix, String_stringMiddle, true) == 0) ||
			         (string.Compare(radix, String_string, true) == 0))
			{
				result = new XRadix(Radix.String);
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
		public static implicit operator Radix(XRadix radix)
		{
			return ((Radix)radix.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XRadix(Radix radix)
		{
			return (new XRadix(radix));
		}

		/// <summary></summary>
		public static implicit operator int(XRadix radix)
		{
			return (radix.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XRadix(int radix)
		{
			return (new XRadix((Radix)radix));
		}

		/// <summary></summary>
		public static implicit operator string(XRadix radix)
		{
			return (radix.ToString());
		}

		/// <summary></summary>
		public static implicit operator XRadix(string radix)
		{
			return (Parse(radix));
		}

		#endregion
	}
}
