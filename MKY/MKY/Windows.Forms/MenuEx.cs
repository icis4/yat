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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// System.Windows.Forms utility methods.
	/// </summary>
	/// <remarks>
	/// This class is intentionally not placed into <c>MKY.Windows.Forms</c> since it's a
	/// pure utility containing no visual contents.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class MenuEx
	{
		/// <summary>
		/// Convert a menu index into a prependable string, e.g. "1: ".
		/// Indices 1 through 10 will be accessible via ALT + numeric key using the ampersand.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Prependable' is a correct English term.")]
		public static string PrependIndex(int index, string text)
		{
			var sb = new StringBuilder();

			if ((index >= 1) && (index <= 9))
			{
				sb.Append("&");
				sb.Append(index);
			}
			else if (index == 10)
			{
				sb.Append("1&0");
			}
			else
			{
				sb.Append(index);
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
