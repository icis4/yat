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
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

namespace YAT.View.Forms
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
			string text = ApplicationEx.ProductName;
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
