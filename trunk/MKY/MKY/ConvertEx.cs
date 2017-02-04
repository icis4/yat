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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MKY
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ConvertEx
	{
		/// <summary>
		/// Converts values into hexadecimal string (e.g. "0A FF 20").
		/// </summary>
		public static string ToHexadecimalString(byte[] values)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				if (i > 0)
					sb.Append(" ");

				sb.Append(values[i].ToString("X2", CultureInfo.InvariantCulture));
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
