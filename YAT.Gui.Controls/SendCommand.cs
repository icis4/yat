using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using HSR.Utilities.Recent;

namespace HSR.YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class SendCommand : UserControl
	{
		//------------------------------------------------------------------------------------------
		// Types
		//------------------------------------------------------------------------------------------

		private enum TextEditState
		{
			Inactive,
			HasFocusButIsNotValidated,
			HasFocusAndIsValidated,
			IsLeavingButIsNotValidated,
		}

		//------------------------------------------------------------------------------------------
		// Constants
		//------------------------------------------------------------------------------------------

		private const bool _TerminalIsOpenDefault = false;

		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Command _command = new Command();
		private RecentItemCollection<Command> _recents;
		private bool _terminalIsOpen = _TerminalIsOpenDefault;

		private TextEditState _commandEditState = TextEditState.Inactive;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler CommandChanged;

		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler SendCommandRequest;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public SendCommand()
		{
			InitializeComponent();
			SetControls();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Command always returns a Command object, it never returns null.
		/// </summary>
		[Browsable(false)]
		public Command Command
		{
			get { return (_command); }
			set
			{
				if (value != null)
					_command = value;
				else
					_command = new Command();

				OnCommandChanged(new EventArgs());
				SetControls();
			}
		}

		[Browsable(false)]
		public RecentItemCollection<Command> RecentCommands
		{
			set
			{
				_recents = value;
				// don't call SetControls(), recents are shown at DropDown
			}
		}

		[Browsable(false)]
		[DefaultValue(_TerminalIsOpenDefault)]
		public bool TerminalIsOpen
		{
			set
			{
				_terminalIsOpen = value;
				SetControls();
			}
		}

		#endregion

		#region Control Special Keys
		//------------------------------------------------------------------------------------------
		// Control Special Keys
		//------------------------------------------------------------------------------------------

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if ((_commandEditState != TextEditState.Inactive) && (keyData == Keys.Enter))
			{
				if (button_SendCommand.Enabled)
				{
					if (_commandEditState == TextEditState.HasFocusButIsNotValidated)
					{
						if (ValidateChildren())
							RequestSendCommand();
					}
					else
					{
						RequestSendCommand();
					}
				}
				return (true);
			}
			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Control Event Handlers
		//------------------------------------------------------------------------------------------
		// Control Event Handlers
		//------------------------------------------------------------------------------------------

		private void SendCommand_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// initially set controls and validate its contents where needed
				SetControls();

				// move cursor to end
				comboBox_Command.SelectionStart = comboBox_Command.Text.Length;
			}
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void comboBox_Command_DropDown(object sender, EventArgs e)
		{
			SetRecents();
		}

		private void comboBox_Command_Enter(object sender, EventArgs e)
		{
			// clear "<Enter a command...>" if needed
			if (!_command.IsSingleLineCommand)
			{
				_isSettingControls = true;
				comboBox_Command.Text = "";
				_isSettingControls = false;
			}
			_commandEditState = TextEditState.HasFocusButIsNotValidated;
		}

		private void comboBox_Command_Leave(object sender, EventArgs e)
		{
            if (_commandEditState == TextEditState.HasFocusAndIsValidated)
                _commandEditState = TextEditState.Inactive;
            else
                _commandEditState = TextEditState.IsLeavingButIsNotValidated;
		}

		private void comboBox_Command_TextChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_commandEditState = TextEditState.HasFocusButIsNotValidated;
		}

		private void comboBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
                if ((_commandEditState == TextEditState.HasFocusButIsNotValidated) ||
                    (_commandEditState == TextEditState.IsLeavingButIsNotValidated))
                {
                    if (Settings.SendCommandSettings.IsEasterEggCommand(comboBox_Command.Text))
				    {
					    if (_commandEditState == TextEditState.IsLeavingButIsNotValidated)
						    _commandEditState = TextEditState.Inactive;
					    else
						    _commandEditState = TextEditState.HasFocusAndIsValidated;

					    SetSingleLineCommand(comboBox_Command.Text);
					    return;
				    }
				    if (Validation.ValidateSequence(this, "Command", comboBox_Command.Text))
				    {
					    if (_commandEditState == TextEditState.IsLeavingButIsNotValidated)
						    _commandEditState = TextEditState.Inactive;
					    else
						    _commandEditState = TextEditState.HasFocusAndIsValidated;

					    SetSingleLineCommand(comboBox_Command.Text);
					    return;
				    }
				    e.Cancel = true;
                }
			}
		}

        private void comboBox_Command_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isSettingControls)
            {
                _commandEditState = TextEditState.HasFocusAndIsValidated;
                SetCommand((Command)((RecentItem<Command>)comboBox_Command.SelectedItem));
            }
       }
        
        private void button_MultiLineCommand_Click(object sender, EventArgs e)
		{
			ShowMultiLineCommandBox(button_MultiLineCommand);
		}

		private void button_SendCommand_Click(object sender, EventArgs e)
		{
			if (_commandEditState == TextEditState.HasFocusButIsNotValidated)
			{
				if (ValidateChildren())
					RequestSendCommand();
			}
			else
			{
				RequestSendCommand();
			}
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			_isSettingControls = true;

			if (_commandEditState == TextEditState.Inactive)
			{
				if (_command.IsCommand)
                    comboBox_Command.Text = _command.SingleLineCommand;
				else
					comboBox_Command.Text = Command.EmptyCommandText;
			}
			button_SendCommand.Enabled = _terminalIsOpen;

			_isSettingControls = false;
		}

		private void SetRecents()
		{
			_isSettingControls = true;

			comboBox_Command.Items.Clear();
			if (_recents != null)
				comboBox_Command.Items.AddRange(_recents.ToArray());

			_isSettingControls = false;
		}

        private void SetCommand(Command command)
        {
            _command = command;

            SetControls();
            OnCommandChanged(new EventArgs());
        }

        private void SetSingleLineCommand(string commandLine)
        {
            _command.IsFilePath = false;
            _command.SingleLineCommand = commandLine;

            SetControls();
            OnCommandChanged(new EventArgs());
        }

        private void ShowMultiLineCommandBox(Control requestingControl)
		{
			// indicate multi line command
			_isSettingControls = true;
			comboBox_Command.Text = Command.MultiLineCommandText;
			_isSettingControls = false;

			// calculate startup location
			Rectangle area = requestingControl.RectangleToScreen(requestingControl.DisplayRectangle);
			Point formStartupLocation = new Point();
			formStartupLocation.X = area.X + area.Width;
			formStartupLocation.Y = area.Y + area.Height;

			// show multi line box
			MultiLineBox f = new MultiLineBox(_command, formStartupLocation);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_command = f.CommandResult;

				SetControls();
				button_SendCommand.Select();

				OnCommandChanged(new EventArgs());
			}
			else
			{
				SetControls();
				comboBox_Command.Select();
			}
		}

		private void RequestSendCommand()
		{
			OnSendCommandRequest(new EventArgs());
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnCommandChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(CommandChanged, this, e);
		}

		protected virtual void OnSendCommandRequest(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(SendCommandRequest, this, e);
		}

		#endregion
	}
}
