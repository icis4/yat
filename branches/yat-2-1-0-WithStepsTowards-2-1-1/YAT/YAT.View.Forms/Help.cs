﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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

			// Form:
			Text = ApplicationEx.CommonName + " Help"; // Fixed to "YAT".

			// Contents:
			textBox_ParserFormat.Text        = Domain.Parser.Parser.FormatHelp;
			textBox_ParserKeyword.Text       = Domain.Parser.Parser.KeywordHelp;
			textBox_TextTerminalKeyword.Text = Domain.TextTerminal.KeywordHelp;
			textBox_SerialPort.Text          = Domain.Terminal.SerialPortHelp;
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