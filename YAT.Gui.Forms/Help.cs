//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	public partial class Help : System.Windows.Forms.Form
	{
		public Help()
		{
			InitializeComponent();

			// form title
			string text = Application.ProductName;
			text += ApplicationInfo.ProductNamePostFix;
			text += " Help";
			Text = text;

			// help texts
			textBox_ParserFormat.Text = Domain.Parser.Parser.FormatHelp;
			textBox_ParserFormat.SelectionStart = 0;
			textBox_ParserFormat.SelectionLength = 0;

			textBox_ParserKeyword.Text = Domain.Parser.Parser.KeywordHelp;
			textBox_ParserKeyword.SelectionStart = 0;
			textBox_ParserKeyword.SelectionLength = 0;

			textBox_TextTerminalKeyword.Text = Domain.TextTerminal.KeywordHelp;
			textBox_TextTerminalKeyword.SelectionStart = 0;
			textBox_TextTerminalKeyword.SelectionLength = 0;
		}

		private void button_Close_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
