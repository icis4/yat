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
	public static class ApplicationEx
	{
		private static bool staticVisualStylesAndTextRenderingHasBeenInitialized;

		/// <summary>
		/// Helper method to deal with application startup requirements of .NET 2.0 and later.
		/// </summary>
		/// <remarks>
		/// <see cref="Application.EnableVisualStyles"/> must be called before creating any
		/// controls in the application. Typically, it is the first line in the Main function.
		/// <see cref="Application.SetCompatibleTextRenderingDefault"/> has to be set to false
		/// for all controls included with Windows Forms 2.0 or later. However, this method can
		/// only be called before the first window is created by the application. This helper
		/// method ensures that this only happens once. This is particularly useful when
		/// calling application startup from a test environment such as NUnit.
		/// </remarks>
		public static bool EnableVisualStylesAndSetTextRenderingIfNotInitializedYet()
		{
			if (!staticVisualStylesAndTextRenderingHasBeenInitialized)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				staticVisualStylesAndTextRenderingHasBeenInitialized = true;

				return (true);
			}
			else
			{
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
