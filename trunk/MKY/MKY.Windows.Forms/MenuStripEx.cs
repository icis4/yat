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
// Copyright © 2003-2021 Matthias Kläy.
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
	/// Extends <see cref="MenuStrip"/>.
	/// </summary>
	/// <remarks>
	/// Workaround to the Windows/WinForms requirement to first activate the application before
	/// a menu or tool strip can be invoked. Based on:
	/// http://stackoverflow.com/questions/3427696/windows-requires-a-click-to-activate-a-window-before-a-second-click-will-select.
	/// 
	/// Attention:
	/// The same code also exists in <see cref="StatusStripEx"/> and <see cref="ToolStripEx"/>.
	/// Changes here must be applied there too.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class MenuStripEx : MenuStrip
	{
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field name is given by the Win32 API.")]
		private const int WM_LBUTTONDOWN = 0x201;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field name is given by the Win32 API.")]
		private const int WM_LBUTTONUP   = 0x202;

		private static bool isDown = false;

		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_LBUTTONUP && !isDown)
			{
				m.Msg = WM_LBUTTONDOWN;
				base.WndProc(ref m);
				m.Msg = WM_LBUTTONUP;
			}

			if (m.Msg == WM_LBUTTONDOWN) isDown = true;
			if (m.Msg == WM_LBUTTONUP)   isDown = false;

			base.WndProc(ref m);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
