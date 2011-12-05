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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY.Windows.Forms;

using YAT.Gui.Utilities;
using YAT.Model.Types;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	public partial class MultiLineBox : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Command command;
		private Command command_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public MultiLineBox(Command command, Point startupLocation)
		{
			InitializeComponent();

			SuspendLayout();
			Location = startupLocation;
			ResumeLayout();

			this.command = command;
			this.command_Form = new Command(command);

			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Command CommandResult
		{
			get { return (this.command); }
		}

		#endregion

		#region Form Special Keys
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		/// <summary></summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.Enter))
			{
				if (ValidateChildren())
					AcceptAndClose();

				return (true);
			}
			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void MultiLineBox_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();

				// move cursor to end
				textBox_Command.SelectionStart = textBox_Command.Text.Length;
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textBox_Command_TextChanged(object sender, EventArgs e)
		{
			bool empty = string.IsNullOrEmpty(textBox_Command.Text);
			label_Remarks.Visible = empty;
			button_OK.Enabled = !empty;
		}

		private void textBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				// retrieve lines from text box with Environment.NewLine
				StringReader reader = new StringReader(textBox_Command.Text);
				List<string> multiLineCommand = new List<string>();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					multiLineCommand.Add(line);
				}

				// validate each line
				bool isValid = true;
				int textLength = 0;
				foreach (string s in multiLineCommand)
				{
					int invalidTextStart;
					int invalidTextLength;
					if (!Validation.ValidateSequence(this, "Command", s, out invalidTextStart, out invalidTextLength))
					{
						invalidTextStart += textLength;
						invalidTextLength = textBox_Command.Text.Length - invalidTextStart;
						textBox_Command.Select(invalidTextStart, invalidTextLength);
						isValid = false;
						break;
					}
					textLength += s.Length + Environment.NewLine.Length;
				}
				if (isValid)
				{
					this.command_Form.MultiLineText = multiLineCommand.ToArray();
					SetControls();
				}
				else
				{
					e.Cancel = true;
				}
			}
		}

		private void textBox_Command_Leave(object sender, EventArgs e)
		{
			bool empty = string.IsNullOrEmpty(textBox_Command.Text);
			label_Remarks.Visible = empty;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			AcceptAndClose();
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			if (this.command_Form.IsSingleLineText)
			{
				textBox_Command.Text = this.command_Form.SingleLineText;
			}
			else
			{
				string text = "";
				for (int i = 0; i < this.command_Form.MultiLineText.Length; i++)
				{
					text += this.command_Form.MultiLineText[i];
					if (i < (this.command_Form.MultiLineText.Length - 1))
						text += Environment.NewLine;
				}
				textBox_Command.Text = text;
			}

			this.isSettingControls.Leave();
		}

		private void AcceptAndClose()
		{
			this.command = this.command_Form;
			DialogResult = DialogResult.OK;
			Close();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
