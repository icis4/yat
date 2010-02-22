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

using MKY.Utilities.Types;

namespace MKY.IO.Ports
{
	#region Enum BaudRate

	/// <summary></summary>
	public enum BaudRate
	{
		/// <summary></summary>
		Baud000075 = 75,
		/// <summary></summary>
		Baud000110 = 110,
		/// <summary></summary>
		Baud000134 = 134,
		/// <summary></summary>
		Baud000150 = 150,
		/// <summary></summary>
		Baud000300 = 300,
		/// <summary></summary>
		Baud000600 = 600,
		/// <summary></summary>
		Baud001200 = 1200,
		/// <summary></summary>
		Baud001800 = 1800,
		/// <summary></summary>
		Baud002400 = 2400,
		/// <summary></summary>
		Baud004800 = 4800,
		/// <summary></summary>
		Baud007200 = 7200,
		/// <summary></summary>
		Baud009600 = 9600,
		/// <summary></summary>
		Baud014400 = 14400,
		/// <summary></summary>
		Baud019200 = 19200,
		/// <summary></summary>
		Baud038400 = 38400,
		/// <summary></summary>
		Baud057600 = 57600,
		/// <summary></summary>
		Baud115200 = 115200,
		/// <summary></summary>
		Baud128000 = 128000,
		/// <summary></summary>
		Baud230400 = 230400,
		/// <summary></summary>
		Baud460800 = 460800,
		/// <summary></summary>
		Baud921600 = 921600,
		/// <summary></summary>
		UserDefined = 0
	}

	#endregion

	/// <summary>
	/// Extended enum XBaudRate.
	/// </summary>
	public class XBaudRate : XEnum
	{
		private int _userDefinedBaudRate = 0;

		/// <summary>Default is <see cref="BaudRate.Baud009600"/></summary>
		public XBaudRate()
			: base(BaudRate.Baud009600)
		{
		}

		/// <summary></summary>
		protected XBaudRate(BaudRate baudRate)
			: base(baudRate)
		{
		}

		/// <summary></summary>
		protected XBaudRate(int baudRate)
			: base(BaudRate.UserDefined)
		{
			_userDefinedBaudRate = baudRate;
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString());
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XBaudRate[] GetItems()
		{
			List<XBaudRate> a = new List<XBaudRate>();
			a.Add(new XBaudRate(BaudRate.Baud000075));
			a.Add(new XBaudRate(BaudRate.Baud000110));
			a.Add(new XBaudRate(BaudRate.Baud000134));
			a.Add(new XBaudRate(BaudRate.Baud000150));
			a.Add(new XBaudRate(BaudRate.Baud000300));
			a.Add(new XBaudRate(BaudRate.Baud000600));
			a.Add(new XBaudRate(BaudRate.Baud001200));
			a.Add(new XBaudRate(BaudRate.Baud001800));
			a.Add(new XBaudRate(BaudRate.Baud002400));
			a.Add(new XBaudRate(BaudRate.Baud004800));
			a.Add(new XBaudRate(BaudRate.Baud007200));
			a.Add(new XBaudRate(BaudRate.Baud009600));
			a.Add(new XBaudRate(BaudRate.Baud014400));
			a.Add(new XBaudRate(BaudRate.Baud019200));
			a.Add(new XBaudRate(BaudRate.Baud038400));
			a.Add(new XBaudRate(BaudRate.Baud057600));
			a.Add(new XBaudRate(BaudRate.Baud115200));
			a.Add(new XBaudRate(BaudRate.Baud128000));
			a.Add(new XBaudRate(BaudRate.Baud230400));
			a.Add(new XBaudRate(BaudRate.Baud460800));
			a.Add(new XBaudRate(BaudRate.Baud921600));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XBaudRate Parse(string baudRate)
		{
			return ((XBaudRate)int.Parse(baudRate));
		}

		/// <summary></summary>
		public static bool TryParse(string baudRate, out XBaudRate result)
		{
			int intResult;

			if (int.TryParse(baudRate, out intResult))
			{
				result = (XBaudRate)intResult;
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
		public static implicit operator BaudRate(XBaudRate baudRate)
		{
			return ((BaudRate)baudRate.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XBaudRate(BaudRate baudRate)
		{
			return (new XBaudRate(baudRate));
		}

		/// <summary></summary>
		public static implicit operator int(XBaudRate baudRate)
		{
			if (baudRate == BaudRate.UserDefined)
				return (baudRate._userDefinedBaudRate);
			else
				return (baudRate.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XBaudRate(int baudRate)
		{
			if      (baudRate == (int)BaudRate.Baud921600) return (new XBaudRate(BaudRate.Baud921600));
			else if (baudRate == (int)BaudRate.Baud460800) return (new XBaudRate(BaudRate.Baud460800));
			else if (baudRate == (int)BaudRate.Baud230400) return (new XBaudRate(BaudRate.Baud230400));
			else if (baudRate == (int)BaudRate.Baud128000) return (new XBaudRate(BaudRate.Baud128000));
			else if (baudRate == (int)BaudRate.Baud115200) return (new XBaudRate(BaudRate.Baud115200));
			else if (baudRate == (int)BaudRate.Baud057600) return (new XBaudRate(BaudRate.Baud057600));
			else if (baudRate == (int)BaudRate.Baud038400) return (new XBaudRate(BaudRate.Baud038400));
			else if (baudRate == (int)BaudRate.Baud019200) return (new XBaudRate(BaudRate.Baud019200));
			else if (baudRate == (int)BaudRate.Baud014400) return (new XBaudRate(BaudRate.Baud014400));
			else if (baudRate == (int)BaudRate.Baud009600) return (new XBaudRate(BaudRate.Baud009600));
			else if (baudRate == (int)BaudRate.Baud007200) return (new XBaudRate(BaudRate.Baud007200));
			else if (baudRate == (int)BaudRate.Baud004800) return (new XBaudRate(BaudRate.Baud004800));
			else if (baudRate == (int)BaudRate.Baud002400) return (new XBaudRate(BaudRate.Baud002400));
			else if (baudRate == (int)BaudRate.Baud001200) return (new XBaudRate(BaudRate.Baud001200));
			else if (baudRate == (int)BaudRate.Baud000600) return (new XBaudRate(BaudRate.Baud000600));
			else if (baudRate == (int)BaudRate.Baud000300) return (new XBaudRate(BaudRate.Baud000300));
			else if (baudRate == (int)BaudRate.Baud000150) return (new XBaudRate(BaudRate.Baud000150));
			else if (baudRate == (int)BaudRate.Baud000134) return (new XBaudRate(BaudRate.Baud000134));
			else if (baudRate == (int)BaudRate.Baud000110) return (new XBaudRate(BaudRate.Baud000110));
			else if (baudRate == (int)BaudRate.Baud000075) return (new XBaudRate(BaudRate.Baud000075));
			else return (new XBaudRate(baudRate));
		}

		/// <summary></summary>
		public static implicit operator string(XBaudRate baudRate)
		{
			return (baudRate.ToString());
		}

		/// <summary></summary>
		public static implicit operator XBaudRate(string baudRate)
		{
			return (Parse(baudRate));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
