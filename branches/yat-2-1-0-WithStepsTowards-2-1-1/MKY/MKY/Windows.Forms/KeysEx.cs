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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="System.Windows.Forms"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class KeysEx
	{
		/// <summary>
		/// Tries to convert the key data into the according function key value.
		/// </summary>
		/// <param name="keyData">A <see cref="Keys"/> value that represents a key.</param>
		/// <param name="functionKey">The according function key value; 0 if no function key.</param>
		/// <returns><c>true</c> if key data is a function key; otherwise, <c>false</c>.</returns>
		public static bool TryConvertFunctionKey(Keys keyData, out int functionKey)
		{
			Keys keyCode = keyData & Keys.KeyCode;
			if ((keyCode >= Keys.F1) && (keyCode <= Keys.F24))
			{
				functionKey = (int)keyCode - (int)Keys.F1 + 1; // F1 must result in 1 and so on.
				return (true);
			}
			else
			{
				functionKey = 0;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
