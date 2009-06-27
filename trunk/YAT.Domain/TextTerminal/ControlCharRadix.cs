//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace YAT.Domain
{
	#region Enum ControlCharRadix

	/// <summary></summary>
	public enum ControlCharRadix
	{
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
	[Serializable]
	public class XControlCharRadix : XRadix
	{
		#region String Definitions

		private const string AsciiMnemonic_string = "ASCII mnemonics";

		#endregion

		/// <summary></summary>
		protected XControlCharRadix(ControlCharRadix radix)
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
		new public static XControlCharRadix[] GetItems()
		{
			List<XControlCharRadix> items = new List<XControlCharRadix>();
			foreach (XRadix radix in XRadix.GetItems())
			{
				items.Add((XControlCharRadix)((Radix)radix));
			}
			items.Add(new XControlCharRadix(ControlCharRadix.AsciiMnemonic));
			return (items.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		new public static XControlCharRadix Parse(string radix)
		{
			if (string.Compare(radix, AsciiMnemonic_string, true) == 0)
			{
				return (new XControlCharRadix(ControlCharRadix.AsciiMnemonic));
			}
			else
			{
				return ((XControlCharRadix)XRadix.Parse(radix));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator ControlCharRadix(XControlCharRadix radix)
		{
			return ((ControlCharRadix)radix.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XControlCharRadix(ControlCharRadix radix)
		{
			return (new XControlCharRadix(radix));
		}

		/// <summary></summary>
		public static explicit operator Radix(XControlCharRadix radix)
		{
			return ((Radix)radix.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XControlCharRadix(Radix radix)
		{
			return (new XControlCharRadix((ControlCharRadix)radix));
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
