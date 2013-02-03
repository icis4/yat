//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MKY.IO.Ports
{
	#region Enum BaudRate

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum BaudRate
	{
		Baud000075 = 75,
		Baud000110 = 110,
		Baud000134 = 134,
		Baud000150 = 150,
		Baud000300 = 300,
		Baud000600 = 600,
		Baud001200 = 1200,
		Baud001800 = 1800,
		Baud002400 = 2400,
		Baud004800 = 4800,
		Baud007200 = 7200,
		Baud009600 = 9600,
		Baud014400 = 14400,
		Baud019200 = 19200,
		Baud038400 = 38400,
		Baud057600 = 57600,
		Baud115200 = 115200,
		Baud128000 = 128000,
		Baud230400 = 230400,
		Baud460800 = 460800,
		Baud921600 = 921600,
		UserDefined = 0
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum BaudRateEx.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class BaudRateEx : EnumEx
	{
		private int userDefinedBaudRate;

		/// <summary>Default is <see cref="BaudRate.Baud009600"/>.</summary>
		public BaudRateEx()
			: base(BaudRate.Baud009600)
		{
		}

		/// <summary></summary>
		protected BaudRateEx(BaudRate baudRate)
			: base(baudRate)
		{
		}

		/// <summary></summary>
		protected BaudRateEx(int baudRate)
			: base(BaudRate.UserDefined)
		{
			if (IsPotentiallyValidBaudRate(baudRate))
			{
				this.userDefinedBaudRate = baudRate;
			}
			else
			{
				throw (new ArgumentOutOfRangeException
				(
					"baudRate",
					baudRate,
					"Baud rate must be a positive integral value"
				));
			}
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static BaudRateEx[] GetItems()
		{
			List<BaudRateEx> a = new List<BaudRateEx>();
			a.Add(new BaudRateEx(BaudRate.Baud000075));
			a.Add(new BaudRateEx(BaudRate.Baud000110));
			a.Add(new BaudRateEx(BaudRate.Baud000134));
			a.Add(new BaudRateEx(BaudRate.Baud000150));
			a.Add(new BaudRateEx(BaudRate.Baud000300));
			a.Add(new BaudRateEx(BaudRate.Baud000600));
			a.Add(new BaudRateEx(BaudRate.Baud001200));
			a.Add(new BaudRateEx(BaudRate.Baud001800));
			a.Add(new BaudRateEx(BaudRate.Baud002400));
			a.Add(new BaudRateEx(BaudRate.Baud004800));
			a.Add(new BaudRateEx(BaudRate.Baud007200));
			a.Add(new BaudRateEx(BaudRate.Baud009600));
			a.Add(new BaudRateEx(BaudRate.Baud014400));
			a.Add(new BaudRateEx(BaudRate.Baud019200));
			a.Add(new BaudRateEx(BaudRate.Baud038400));
			a.Add(new BaudRateEx(BaudRate.Baud057600));
			a.Add(new BaudRateEx(BaudRate.Baud115200));
			a.Add(new BaudRateEx(BaudRate.Baud128000));
			a.Add(new BaudRateEx(BaudRate.Baud230400));
			a.Add(new BaudRateEx(BaudRate.Baud460800));
			a.Add(new BaudRateEx(BaudRate.Baud921600));
			return (a.ToArray());
		}

		#endregion

		#region Parse/Form

		/// <summary></summary>
		public static BaudRateEx Parse(string baudRate)
		{
			return ((BaudRateEx)int.Parse(baudRate, CultureInfo.InvariantCulture));
		}

		/// <summary></summary>
		public static bool TryParse(string baudRate, out BaudRateEx result)
		{
			int intResult;
			if (int.TryParse(baudRate, out intResult))
				return (TryFrom(intResult, out result));
			
			result = null;
			return (false);
		}

		/// <summary>
		/// Tries to create a <see cref="BaudRateEx"/> object from the given port number.
		/// </summary>
		public static bool TryFrom(int baudRate, out BaudRateEx result)
		{
			if (IsPotentiallyValidBaudRate(baudRate))
			{
				result = new BaudRateEx(baudRate);
				return (true);
			}

			result = null;
			return (false);
		}

		/// <summary></summary>
		public static bool IsPotentiallyValidBaudRate(int baudRate)
		{
			return (baudRate > 0);
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator BaudRate(BaudRateEx baudRate)
		{
			return ((BaudRate)baudRate.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator BaudRateEx(BaudRate baudRate)
		{
			return (new BaudRateEx(baudRate));
		}

		/// <summary></summary>
		public static implicit operator int(BaudRateEx baudRate)
		{
			if (baudRate == BaudRate.UserDefined)
				return (baudRate.userDefinedBaudRate);
			else
				return (baudRate.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator BaudRateEx(int baudRate)
		{
			if      (baudRate == (int)BaudRate.Baud921600) return (new BaudRateEx(BaudRate.Baud921600));
			else if (baudRate == (int)BaudRate.Baud460800) return (new BaudRateEx(BaudRate.Baud460800));
			else if (baudRate == (int)BaudRate.Baud230400) return (new BaudRateEx(BaudRate.Baud230400));
			else if (baudRate == (int)BaudRate.Baud128000) return (new BaudRateEx(BaudRate.Baud128000));
			else if (baudRate == (int)BaudRate.Baud115200) return (new BaudRateEx(BaudRate.Baud115200));
			else if (baudRate == (int)BaudRate.Baud057600) return (new BaudRateEx(BaudRate.Baud057600));
			else if (baudRate == (int)BaudRate.Baud038400) return (new BaudRateEx(BaudRate.Baud038400));
			else if (baudRate == (int)BaudRate.Baud019200) return (new BaudRateEx(BaudRate.Baud019200));
			else if (baudRate == (int)BaudRate.Baud014400) return (new BaudRateEx(BaudRate.Baud014400));
			else if (baudRate == (int)BaudRate.Baud009600) return (new BaudRateEx(BaudRate.Baud009600));
			else if (baudRate == (int)BaudRate.Baud007200) return (new BaudRateEx(BaudRate.Baud007200));
			else if (baudRate == (int)BaudRate.Baud004800) return (new BaudRateEx(BaudRate.Baud004800));
			else if (baudRate == (int)BaudRate.Baud002400) return (new BaudRateEx(BaudRate.Baud002400));
			else if (baudRate == (int)BaudRate.Baud001200) return (new BaudRateEx(BaudRate.Baud001200));
			else if (baudRate == (int)BaudRate.Baud000600) return (new BaudRateEx(BaudRate.Baud000600));
			else if (baudRate == (int)BaudRate.Baud000300) return (new BaudRateEx(BaudRate.Baud000300));
			else if (baudRate == (int)BaudRate.Baud000150) return (new BaudRateEx(BaudRate.Baud000150));
			else if (baudRate == (int)BaudRate.Baud000134) return (new BaudRateEx(BaudRate.Baud000134));
			else if (baudRate == (int)BaudRate.Baud000110) return (new BaudRateEx(BaudRate.Baud000110));
			else if (baudRate == (int)BaudRate.Baud000075) return (new BaudRateEx(BaudRate.Baud000075));
			else return (new BaudRateEx(baudRate));
		}

		/// <summary></summary>
		public static implicit operator string(BaudRateEx baudRate)
		{
			return (baudRate.ToString());
		}

		/// <summary></summary>
		public static implicit operator BaudRateEx(string baudRate)
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
