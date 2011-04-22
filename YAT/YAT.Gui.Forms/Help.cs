//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class Help : System.Windows.Forms.Form
	{
		/// <summary>
		/// \fixme
		/// Add a true help to YAT.
		/// 
		/// \remind
		/// Explain framing errors:
		/// "A framing error occurs when the last bit is not a stop bit. This may occur due to a timing error. You will most commonly encounter a framing error when using a null modem linking two computers or a protocol analyzer when the speed at which the data is being sent is different to that of what you have the UART set to receive it at."
		/// In YAT, these errors are shown in red.
		/// </summary>
		public Help()
		{
			InitializeComponent();

			// Form title
			string text = Application.ProductName;
			text += " Help";
			Text = text;

			// Help texts
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
// End of
// $URL$
//==================================================================================================
