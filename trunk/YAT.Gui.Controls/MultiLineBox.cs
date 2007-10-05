using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using YAT.Gui.Types;

namespace YAT.Gui.Controls
{
	public partial class MultiLineBox : Form
	{
		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Command _command;
		private Command _command_Form;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public MultiLineBox(Command command, Point startupLocation)
		{
			InitializeComponent();

			SuspendLayout();
			Location = startupLocation;
			ResumeLayout();

			_command = command;
			_command_Form = new Command(command);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public Command CommandResult
		{
			get { return (_command); }
		}

		#endregion

		#region Form Special Keys
		//------------------------------------------------------------------------------------------
		// Form Special Keys
		//------------------------------------------------------------------------------------------

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
		//------------------------------------------------------------------------------------------
		// Form Event Handlers
		//------------------------------------------------------------------------------------------

		private void MultiLineBox_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// initially set controls and validate its contents where needed
				SetControls();

				// move cursor to end
				textBox_Command.SelectionStart = textBox_Command.Text.Length;
			}
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void textBox_Command_TextChanged(object sender, EventArgs e)
		{
			bool empty = string.IsNullOrEmpty(textBox_Command.Text);
			label_Remarks.Visible = empty;
			button_OK.Enabled = !empty;
		}

		private void textBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
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
				foreach (string s in multiLineCommand)
				{
					if (!Validation.ValidateSequence(this, "Command", s))
					{
						isValid = false;
						break;
					}
				}
				if (isValid)
				{
					_command_Form.MultiLineCommand = multiLineCommand.ToArray();
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
			// do nothing
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			_isSettingControls = true;

			if (_command_Form.IsSingleLineCommand)
			{
				textBox_Command.Text = _command_Form.SingleLineCommand;
			}
			else
			{
				string text = "";
				for (int i = 0; i < _command_Form.MultiLineCommand.Length; i++)
				{
					text += _command_Form.MultiLineCommand[i];
					if (i < (_command_Form.MultiLineCommand.Length - 1))
						text += Environment.NewLine;
				}
				textBox_Command.Text = text;
			}

			_isSettingControls = false;
		}

		private void AcceptAndClose()
		{
			_command = _command_Form;
			DialogResult = DialogResult.OK;
			Close();
		}

		#endregion
	}
}