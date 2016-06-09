//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Windows.Forms;

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
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("This test application is meant to be used with ENABLE_HORIZONTAL_AUTO_SCROLL enabled in MKY.Windows.Forms.ListBoxEx.");
			sb.AppendLine();
			sb.AppendLine("In addition, lines #281 and #282 of MKY.Windows.Forms.Test.WindowsFormsTest.Designer have to be un-commented.");
			sb.AppendLine();
			sb.AppendLine("Either confirm this, or cancel to quit.");

			DialogResult dr = MessageBox.Show(sb.ToString(), "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
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
