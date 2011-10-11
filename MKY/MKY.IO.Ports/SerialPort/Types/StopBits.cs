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
	/// Extended enum StopBitsEx.
	/// </summary>
	public class StopBitsEx : EnumEx
	{
		#region Double Definitions

		private const double None_double = 0.0;
		private const double One_double = 1.0;
		private const double OnePointFive_double = 1.5;
		private const double Two_double = 2.0;

		#endregion

		/// <summary>Default is <see cref="StopBits.One"/>.</summary>
		public StopBitsEx()
			: base(StopBits.One)
		{
		}

		/// <summary></summary>
		protected StopBitsEx(StopBits bits)
			: base(bits)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((StopBits)UnderlyingEnum)
			{
				case StopBits.None:         return (None_double.ToString());
				case StopBits.One:          return (One_double.ToString());
				case StopBits.OnePointFive: return (OnePointFive_double.ToString());
				case StopBits.Two:          return (Two_double.ToString());
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static StopBitsEx[] GetItems()
		{
			List<StopBitsEx> a = new List<StopBitsEx>();
			a.Add(new StopBitsEx(StopBits.None));
			a.Add(new StopBitsEx(StopBits.One));
			a.Add(new StopBitsEx(StopBits.OnePointFive));
			a.Add(new StopBitsEx(StopBits.Two));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static StopBitsEx Parse(string bits)
		{
			return ((StopBitsEx)(double.Parse(bits)));
		}

		/// <summary></summary>
		public static bool TryParse(string bits, out StopBitsEx result)
		{
			double doubleResult;

			if (double.TryParse(bits, out doubleResult))
			{
				result = (StopBitsEx)doubleResult;
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
		public static implicit operator StopBits(StopBitsEx bits)
		{
			return ((StopBits)bits.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator StopBitsEx(StopBits bits)
		{
			return (new StopBitsEx(bits));
		}

		/// <summary></summary>
		public static implicit operator double(StopBitsEx bits)
		{
			switch ((StopBits)bits.UnderlyingEnum)
			{
				case StopBits.None:         return (None_double);
				case StopBits.One:          return (One_double);
				case StopBits.OnePointFive: return (OnePointFive_double);
				case StopBits.Two:          return (Two_double);
			}
			throw (new ArgumentOutOfRangeException("bits", bits, "Invalid bits value"));
		}

		/// <summary></summary>
		public static implicit operator StopBitsEx(double bits)
		{
			if      (bits >= Two_double)          return (new StopBitsEx(StopBits.Two));
			else if (bits >= OnePointFive_double) return (new StopBitsEx(StopBits.OnePointFive));
			else if (bits >= One_double)          return (new StopBitsEx(StopBits.One));
			else                                  return (new StopBitsEx(StopBits.None));
		}

		/// <summary></summary>
		public static implicit operator string(StopBitsEx bits)
		{
			return (bits.ToString());
		}

		/// <summary></summary>
		public static implicit operator StopBitsEx(string bits)
		{
			return (Parse(bits));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
