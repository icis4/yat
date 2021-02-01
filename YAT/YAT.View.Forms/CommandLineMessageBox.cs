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
// YAT Version 2.4.0
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

using System;
using System.Windows.Forms;

namespace YAT.View.Forms
{
	/// <remarks>
	/// Could be generalized into an 'TextInfo' form, covering <see cref="ReleaseNotes"/>,
	/// but is not considered worth it for the few lines of duplicated code.
	/// </remarks>
	public partial class CommandLineMessageBox : Form
	{
		/// <summary></summary>
		public CommandLineMessageBox(string text, string caption)
		{
			InitializeComponent();

			// Form:
			Text = caption;
		#if (WITH_SCRIPTING)
			Icon = Properties.Resources.Icon_Albatros;
		#endif

			// Text:
			textBox_Text.Text = text;
		}

		private void button_Close_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
