//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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

using MKY.Utilities.Event;
using MKY.Utilities.Recent;

using YAT.Gui.Utilities;
using YAT.Model.Types;
using YAT.Model.Settings;

namespace YAT.Gui.Controls
{
	/// <summary>
	/// Provides command edit and send. Control keeps track of the edit state to properly
	/// react on all possible edit states.
	/// </summary>
	/// <remarks>
	/// On focus enter, edit state is always reset.
	/// On focus leave, edit state is kept depending on how focus is leaving.
	/// </remarks>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class SendCommand : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum FocusState
		{
			Inactive,
			HasFocus,
			IsLeaving,
			IsLeavingControl,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool _TerminalIsOpenDefault = false;
		private const float _SplitterRatioDefault = (float)0.75;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private Command _command = new Command();
		private RecentItemCollection<Command> _recents;
		private bool _terminalIsOpen = _TerminalIsOpenDefault;
		private float _splitterRatio = _SplitterRatioDefault;

		private FocusState _focusState = FocusState.Inactive;
		private bool _isValidated = false;
		private bool _sendIsRequested = false;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler CommandChanged;

		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler SendCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public SendCommand()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Command always returns a Command object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command Command
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
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> RecentCommands
		{
			set
			{
				_recents = value;
				// don't call SetControls(), recents are shown at DropDown
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsOpen
		{
			set
			{
				_terminalIsOpen = value;
				SetControls();
			}
		}

		[DefaultValue(_SplitterRatioDefault)]
		public virtual float SplitterRatio
		{
			get { return (_splitterRatio); }
			set
			{
				_splitterRatio = value;
				SetControls();
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public virtual void SelectInput()
		{
			comboBox_Command.Select();
		}

		#endregion

		#region Control Special Keys
		//==========================================================================================
		// Control Special Keys
		//==========================================================================================

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if ((_focusState != FocusState.Inactive) && (keyData == Keys.Enter))
			{
				if (button_SendCommand.Enabled)
				{
					if (_isValidated)
					{
						RequestSendCommand();
					}
					else
					{
						if (ValidateChildren())
							RequestSendCommand();
					}
				}
				return (true);
			}
			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool _isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void SendCommand_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;
				SetControls();

				// Move cursor to end.
				comboBox_Command.SelectionStart = comboBox_Command.Text.Length;
			}
		}

		private void SendCommand_Enter(object sender, EventArgs e)
		{
			_focusState = FocusState.Inactive;
			_isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving control, e.g. other MDI child activated.
		/// 1. ComboBox.Leave()
		/// 2. UserControl.Leave()
		/// 3. ComboBox.Validating()
		/// </remarks>
		private void SendCommand_Leave(object sender, EventArgs e)
		{
			if (_isValidated)
				_focusState = FocusState.Inactive;
			else
				_focusState = FocusState.IsLeavingControl;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Command_DropDown(object sender, EventArgs e)
		{
			SetRecents();
		}

		private void comboBox_Command_Enter(object sender, EventArgs e)
		{
			// Clear "<Enter a command...>" if needed
			if ((_focusState == FocusState.Inactive) && !_command.IsSingleLineCommand)
			{
				_isSettingControls = true;
				comboBox_Command.Text      = "";
				comboBox_Command.ForeColor = SystemColors.ControlText;
				comboBox_Command.Font      = SystemFonts.DefaultFont; 
				_isSettingControls = false;
			}

			_focusState = FocusState.HasFocus;
			_isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// 
		/// Event sequence when focus is leaving control, e.g. other MDI child activated.
		/// 1. ComboBox.Leave()
		/// 2. UserControl.Leave()
		/// 3. ComboBox.Validating()
		/// </remarks>
		private void comboBox_Command_Leave(object sender, EventArgs e)
		{
			if (_isValidated)
				_focusState = FocusState.Inactive;
			else
				_focusState = FocusState.IsLeaving;
		}

		private void comboBox_Command_TextChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// 
		/// Event sequence when focus is leaving control, e.g. other MDI child activated.
		/// 1. ComboBox.Leave()
		/// 2. UserControl.Leave()
		/// 3. ComboBox.Validating()
		/// </remarks>
		private void comboBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				if (!_isValidated && (_focusState != FocusState.IsLeavingControl))
				{
					if (SendCommandSettings.IsEasterEggCommand(comboBox_Command.Text))
					{
						_isValidated = true;

						if (_focusState == FocusState.IsLeaving)
							_focusState = FocusState.Inactive;
						else
							_focusState = FocusState.HasFocus;

						CreateSingleLineCommand(comboBox_Command.Text);
						return;
					}

					int invalidTextStart;
					int invalidTextLength;
					if (Validation.ValidateSequence(this, "Command", comboBox_Command.Text, out invalidTextStart, out invalidTextLength))
					{
						_isValidated = true;

						if (_focusState == FocusState.IsLeaving)
							_focusState = FocusState.Inactive;
						else
							_focusState = FocusState.HasFocus;

						CreateSingleLineCommand(comboBox_Command.Text);
						return;
					}

					_focusState = FocusState.HasFocus;
					comboBox_Command.Select(invalidTextStart, invalidTextLength);
					e.Cancel = true;
				}
			}
		}

		private void comboBox_Command_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_isValidated = true; // Commands in history have already been validated
				SetCommand((Command)((RecentItem<Command>)comboBox_Command.SelectedItem));
			}
		}
		
		private void button_MultiLineCommand_Click(object sender, EventArgs e)
		{
			ShowMultiLineCommandBox(button_MultiLineCommand);
		}

		private void button_SendCommand_Click(object sender, EventArgs e)
		{
			if (_isValidated)
			{
				RequestSendCommand();
			}
			else
			{
				if (ValidateChildren())
					RequestSendCommand();
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			_isSettingControls = true;

			splitContainer.SplitterDistance = (int)(_splitterRatio * splitContainer.Width);

			if (_focusState == FocusState.Inactive)
			{
				if (_command.IsCommand)
				{
					comboBox_Command.Text      = _command.SingleLineCommand;
					comboBox_Command.ForeColor = SystemColors.ControlText;
					comboBox_Command.Font      = SystemFonts.DefaultFont;
				}
				else
				{
					comboBox_Command.Text      = Command.EnterCommandText;
					comboBox_Command.ForeColor = SystemColors.GrayText;
					comboBox_Command.Font      = Utilities.Drawing.ItalicDefaultFont;
				}
			}
			else if (_sendIsRequested)
			{   // Needed when command is modified (e.g. cleared) after send
				if (_command.IsCommand)
					comboBox_Command.Text  = _command.SingleLineCommand;
				else
					comboBox_Command.Text  = "";

				comboBox_Command.ForeColor = SystemColors.ControlText;
				comboBox_Command.Font      = SystemFonts.DefaultFont;
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

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreateSingleLineCommand(string commandLine)
		{
			_command = new Command(commandLine);

			SetControls();
			OnCommandChanged(new EventArgs());
		}

		private void ShowMultiLineCommandBox(Control requestingControl)
		{
			// indicate multi line command
			_isSettingControls = true;
			comboBox_Command.Text      = Command.MultiLineCommandText;
			comboBox_Command.ForeColor = SystemColors.ControlText;
			comboBox_Command.Font      = SystemFonts.DefaultFont;
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
			if (_focusState == FocusState.Inactive)
			{
				OnSendCommandRequest(new EventArgs());
			}
			else
			{
				// notifying the send state is needed when command is automatically
				//   modified (e.g. cleared) after send
				_sendIsRequested = true;
				OnSendCommandRequest(new EventArgs());
				_sendIsRequested = false;
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(CommandChanged, this, e);
		}

		protected virtual void OnSendCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
