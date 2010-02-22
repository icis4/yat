//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.IO.Ports;

using MKY.Utilities.Types;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Extended enum XParity.
	/// </summary>
	public class XParity : XEnum
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

		/// <summary>Default is <see cref="Parity.None"/></summary>
		public XParity()
			: base(Parity.None)
		{
		}

		/// <summary></summary>
		protected XParity(Parity parity)
			: base(parity)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Parity)UnderlyingEnum)
			{
				case Parity.Even: return (Even_string);
				case Parity.Odd: return (Odd_string);
				case Parity.None: return (None_string);
				case Parity.Mark: return (Mark_string);
				case Parity.Space: return (Space_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		/// <summary></summary>
		public string ToShortString()
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
		public static XParity[] GetItems()
		{
			List<XParity> a = new List<XParity>();
			a.Add(new XParity(Parity.Even));
			a.Add(new XParity(Parity.Odd));
			a.Add(new XParity(Parity.None));
			a.Add(new XParity(Parity.Mark));
			a.Add(new XParity(Parity.Space));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XParity Parse(string parity)
		{
			XParity result;

			if (TryParse(parity, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("parity", parity, "Invalid parity."));
		}

		/// <summary></summary>
		public static bool TryParse(string parity, out XParity result)
		{
			if      ((string.Compare(parity, Even_string, true) == 0) ||
			         (string.Compare(parity, Even_stringShort, true) == 0))
			{
				result = new XParity(Parity.Even);
				return (true);
			}
			else if ((string.Compare(parity, Odd_string, true) == 0) ||
			         (string.Compare(parity, Odd_stringShort, true) == 0))
			{
				result = new XParity(Parity.Odd);
				return (true);
			}
			else if ((string.Compare(parity, None_string, true) == 0) ||
			         (string.Compare(parity, None_stringShort, true) == 0))
			{
				result = new XParity(Parity.None);
				return (true);
			}
			else if ((string.Compare(parity, Mark_string, true) == 0) ||
			         (string.Compare(parity, Mark_stringShort, true) == 0))
			{
				result = new XParity(Parity.Mark);
				return (true);
			}
			else if ((string.Compare(parity, Space_string, true) == 0) ||
			         (string.Compare(parity, Space_stringShort, true) == 0))
			{
				result = new XParity(Parity.Space);
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
		public static implicit operator Parity(XParity parity)
		{
			return ((Parity)parity.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XParity(Parity parity)
		{
			return (new XParity(parity));
		}

		/// <summary></summary>
		public static implicit operator int(XParity parity)
		{
			return (parity.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XParity(int parity)
		{
			return (new XParity((Parity)parity));
		}

		/// <summary></summary>
		public static implicit operator string(XParity parity)
		{
			return (parity.ToString());
		}

		/// <summary></summary>
		public static implicit operator XParity(string parity)
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
