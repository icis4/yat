//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/MKY/MKY.Windows.Forms/ToolStripEx.cs $
// $Author: klaey-1 $
// $Date: 2016/09/19 15:12:00MESZ $
// $Revision: 1.1 $
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.ComponentModel;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a transparent panel. It can be used to handle user input on an invisible rectangle.
	/// </summary>
	public class TransparentPanel : Panel
	{
		/// <summary>
		/// Gets the required creation parameters when the control handle is created.
		/// </summary>
		[Browsable(false)]
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x20; // 0x20 = WS_EX_TRANSPARENT = one of the 'Extended Window Styles' of the Win32 API.
				return (cp);
			}
		}

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="e">A <see cref="PaintEventArgs"/> that contains the event data.</param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// Do nothing to leave the background invisible, i.e. leave the control transparent.

			UnusedArg.PreventAnalysisWarning(e);
		}
	}
}

//==================================================================================================
// End of
// $URL: http://svn.code.sf.net/p/y-a-terminal/code/trunk/MKY/MKY.Windows.Forms/PathLabel.cs $
//==================================================================================================
