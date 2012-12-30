//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class Help : Form
	{
		/// <remarks>
		/// \fixme:
		/// Add a true help to YAT.
		/// </remarks>
		public Help()
		{
			InitializeComponent();

			// Form title/caption:
			string text = Application.ProductName;
			text += " Help";
			Text = text;

			// Help texts:
			textBox_ParserFormat.Text = Domain.Parser.Parser.FormatHelp;
			textBox_ParserFormat.SelectionStart = 0;
			textBox_ParserFormat.SelectionLength = 0;

			textBox_ParserKeyword.Text = Domain.Parser.Parser.KeywordHelp;
			textBox_ParserKeyword.SelectionStart = 0;
			textBox_ParserKeyword.SelectionLength = 0;

			textBox_TextTerminalKeyword.Text = Domain.TextTerminal.KeywordHelp;
			textBox_TextTerminalKeyword.SelectionStart = 0;
			textBox_TextTerminalKeyword.SelectionLength = 0;

			textBox_SerialPort.Text = Domain.Terminal.SerialPortHelp;
			textBox_SerialPort.SelectionStart = 0;
			textBox_SerialPort.SelectionLength = 0;
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
