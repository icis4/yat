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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Windows.Forms;

namespace MKY.Windows.Forms.Test
{
	/// <summary>
	/// Test application for <see cref="Forms"/>.
	/// </summary>
	public partial class TestMain : Form
	{
		/// <summary>
		/// The main form for the application.
		/// </summary>
		public TestMain()
		{
			InitializeComponent();
		}

		private void button_TestMessageBoxes_Click(object sender, EventArgs e)
		{
			MessageBoxesTest.Run(this);
		}

		private void button_TextInputControls_Click(object sender, EventArgs e)
		{
			var f = new TextInputTest();
			f.Show(this);
		}

		private void button_ListBoxControls_Click(object sender, EventArgs e)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Note that this test application may used with ENABLE_HORIZONTAL_AUTO_SCROLL enabled in MKY.Windows.Forms.ListBoxEx.");
			sb.AppendLine();
			sb.AppendLine("To enable ENABLE_HORIZONTAL_AUTO_SCROLL, lines #281 and #282 of MKY.Windows.Forms.Test.ListBoxTest.Designer have to be un-commented.");
			sb.AppendLine();
			sb.Append    ("You may also run this test application without ENABLE_HORIZONTAL_AUTO_SCROLL enabled.");

			var dr = MessageBoxEx.Show(sb.ToString(), "Note related to ENABLE_HORIZONTAL_AUTO_SCROLL", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
			if (dr != DialogResult.OK)
				return;

			var f = new ListBoxTest();
			f.Show(this);
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new TestMain());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
