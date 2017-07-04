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
// MKY Version 1.0.19
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

namespace MKY.Windows.Forms.Test
{
	/// <summary>
	/// Test application for <see cref="MKY.Windows.Forms"/>.
	/// </summary>
	public static class TestMain
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Note that this test application can be used with ENABLE_HORIZONTAL_AUTO_SCROLL enabled in MKY.Windows.Forms.ListBoxEx.");
			sb.AppendLine();
			sb.AppendLine("To fully enable ENABLE_HORIZONTAL_AUTO_SCROLL, lines #281 and #282 of MKY.Windows.Forms.Test.WindowsFormsTest.Designer have to be un-commented.");
			sb.AppendLine();
			sb.Append    ("You may also run this test application without ENABLE_HORIZONTAL_AUTO_SCROLL enabled.");

			var dr = MessageBoxEx.Show(sb.ToString(), "Warning related to ENABLE_HORIZONTAL_AUTO_SCROLL", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
			if (dr != DialogResult.OK)
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new WindowsFormsTest());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
