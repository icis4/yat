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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Extended enum ParityEx.
	/// </summary>
	public class ParityEx : EnumEx
	{
		#region String Definitions

		private const string Even_string = "Even";
		private const string Even_stringShort = "E";
		private const string Odd_string = "Odd";
		private const string Odd_stringShort = "O";
		private const string None_string = "None";
		private const string None_stringShort = "N";
		private const string Mark_string = "Mark";
		private const string Mark_stringShort = "M";
		private const string Space_string = "Space";
		private const string Space_stringShort = "S";

		#endregion

		/// <summary>Default is <see cref="Parity.None"/>.</summary>
		public ParityEx()
			: base(Parity.None)
		{
		}

		/// <summary></summary>
		protected ParityEx(Parity parity)
			: base(parity)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Parity)UnderlyingEnum)
			{
				case Parity.Even:  return (Even_string);
				case Parity.Odd:   return (Odd_string);
				case Parity.None:  return (None_string);
				case Parity.Mark:  return (Mark_string);
				case Parity.Space: return (Space_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			switch ((Parity)UnderlyingEnum)
			{
				case Parity.Even: return (Even_stringShort);
				case Parity.Odd: return (Odd_stringShort);
				case Parity.None: return (None_stringShort);
				case Parity.Mark: return (Mark_stringShort);
				case Parity.Space: return (Space_stringShort);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static ParityEx[] GetItems()
		{
			List<ParityEx> a = new List<ParityEx>();
			a.Add(new ParityEx(Parity.Even));
			a.Add(new ParityEx(Parity.Odd));
			a.Add(new ParityEx(Parity.None));
			a.Add(new ParityEx(Parity.Mark));
			a.Add(new ParityEx(Parity.Space));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static ParityEx Parse(string parity)
		{
			ParityEx result;

			if (TryParse(parity, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("parity", parity, "Invalid parity."));
		}

		/// <summary></summary>
		public static bool TryParse(string parity, out ParityEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(parity, Even_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(parity, Even_stringShort))
			{
				result = new ParityEx(Parity.Even);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(parity, Odd_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(parity, Odd_stringShort))
			{
				result = new ParityEx(Parity.Odd);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(parity, None_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(parity, None_stringShort))
			{
				result = new ParityEx(Parity.None);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(parity, Mark_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(parity, Mark_stringShort))
			{
				result = new ParityEx(Parity.Mark);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(parity, Space_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(parity, Space_stringShort))
			{
				result = new ParityEx(Parity.Space);
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
		public static implicit operator Parity(ParityEx parity)
		{
			return ((Parity)parity.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator ParityEx(Parity parity)
		{
			return (new ParityEx(parity));
		}

		/// <summary></summary>
		public static implicit operator int(ParityEx parity)
		{
			return (parity.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator ParityEx(int parity)
		{
			return (new ParityEx((Parity)parity));
		}

		/// <summary></summary>
		public static implicit operator string(ParityEx parity)
		{
			return (parity.ToString());
		}

		/// <summary></summary>
		public static implicit operator ParityEx(string parity)
		{
			return (Parse(parity));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
