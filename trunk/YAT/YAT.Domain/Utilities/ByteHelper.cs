//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Globalization;
using System.Text;

#endregion

namespace YAT.Domain.Utilities
{
	/// <summary>
	/// Static utility class providing byte functionality for YAT.
	/// </summary>
	public static class ByteHelper
	{
		/// <summary>
		/// Formats the given data into a hexadecimal string.
		/// </summary>
		public static string FormatHexString(byte data, bool showRadix = true)
		{
			return (FormatHexString(new byte[] { data }, showRadix));
		}

		/// <summary>
		/// Formats the given data into a hexadecimal string.
		/// </summary>
		public static string FormatHexString(byte[] data, bool showRadix = true)
		{
			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (byte b in data)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(" ");

				if (showRadix)
					sb.Append(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
				else
					sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
