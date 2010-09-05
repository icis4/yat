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
	/// Extended enum XStopBits.
	/// </summary>
	public class XStopBits : XEnum
	{
		#region Double Definitions

		private const double None_double = 0.0;
		private const double One_double = 1.0;
		private const double OnePointFive_double = 1.5;
		private const double Two_double = 2.0;

		#endregion

		/// <summary>Default is <see cref="StopBits.One"/>.</summary>
		public XStopBits()
			: base(StopBits.One)
		{
		}

		/// <summary></summary>
		protected XStopBits(StopBits bits)
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
		public static XStopBits[] GetItems()
		{
			List<XStopBits> a = new List<XStopBits>();
			a.Add(new XStopBits(StopBits.None));
			a.Add(new XStopBits(StopBits.One));
			a.Add(new XStopBits(StopBits.OnePointFive));
			a.Add(new XStopBits(StopBits.Two));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XStopBits Parse(string bits)
		{
			return ((XStopBits)(double.Parse(bits)));
		}

		/// <summary></summary>
		public static bool TryParse(string bits, out XStopBits result)
		{
			double doubleResult;

			if (double.TryParse(bits, out doubleResult))
			{
				result = (XStopBits)doubleResult;
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
		public static implicit operator StopBits(XStopBits bits)
		{
			return ((StopBits)bits.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XStopBits(StopBits bits)
		{
			return (new XStopBits(bits));
		}

		/// <summary></summary>
		public static implicit operator double(XStopBits bits)
		{
			switch ((StopBits)bits.UnderlyingEnum)
			{
				case StopBits.None:         return (None_double);
				case StopBits.One:          return (One_double);
				case StopBits.OnePointFive: return (OnePointFive_double);
				case StopBits.Two:          return (Two_double);
			}
			throw (new NotImplementedException(bits.UnderlyingEnum.ToString()));
		}

		/// <summary></summary>
		public static implicit operator XStopBits(double bits)
		{
			if      (bits >= Two_double)          return (new XStopBits(StopBits.Two));
			else if (bits >= OnePointFive_double) return (new XStopBits(StopBits.OnePointFive));
			else if (bits >= One_double)          return (new XStopBits(StopBits.One));
			else                                  return (new XStopBits(StopBits.None));
		}

		/// <summary></summary>
		public static implicit operator string(XStopBits bits)
		{
			return (bits.ToString());
		}

		/// <summary></summary>
		public static implicit operator XStopBits(string bits)
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
