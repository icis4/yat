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

using System.Text;

namespace MKY.System.Windows.Forms
{
	/// <summary>
	/// System.Windows.Forms utility methods.
	/// </summary>
	/// <remarks>
	/// This class is intentionally not placed into <c>MKY.Windows.Forms</c> since it's a
	/// pure utility containing no visual contents.
	/// </remarks>
	public static class XMenu
	{
		/// <summary>
		/// Convert a menu index into a prependable string, e.g. "1: ".
		/// Indecies 1 through 10 will be accessible via ALT + nummeric key using the ampersand.
		/// </summary>
		public static string PrependIndex(int i, string text)
		{
			StringBuilder sb = new StringBuilder();

			if ((i >= 1) && (i <= 9))
			{
				sb.Append("&");
				sb.Append(i);
			}
			else if (i == 10)
			{
				sb.Append("1&0");
			}
			else
			{
				sb.Append(i);
			}
			sb.Append(": ");
			sb.Append(text);

			return (sb.ToString());
		}

	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
