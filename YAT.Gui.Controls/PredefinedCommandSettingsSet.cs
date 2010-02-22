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
using System.IO;

using MKY.Utilities.Event;

using YAT.Model.Types;
using YAT.Settings;
using YAT.Settings.Application;
using YAT.Gui.Utilities;

namespace YAT.Gui.Controls
{
	/// <summary>
	/// Provides command edit. Control keeps track of the edit state to properly
	/// react on all possible edit states.
	/// </summary>
	/// <remarks>
	/// On focus enter, edit state is always reset.
	/// On focus leave, edit state is kept depending on how focus is leaving.
	/// </remarks>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("CommandChanged")]
	public partial class PredefinedCommandSettingsSet : UserControl
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
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string _ShortcutStringDefault = "Shift+F1";
		private const Domain.TerminalType _TerminalTypeDefault = Domain.TerminalType.Text;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Model.Types.Command _command = new Model.Types.Command();
		private Domain.TerminalType _terminalType = _TerminalTypeDefault;

		private FocusState _focusState = FocusState.Inactive;
		private bool _isValidated = false;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when any of the commands properties have changed.")]
		public event EventHandler CommandChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public PredefinedCommandSettingsSet()
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
		public Model.Types.Command Command
		{
			get { return (_command); }
			set
			{
				if (value != null)
					_command = value;
				else
					_command = new Model.Types.Command();

				OnCommandChanged(new EventArgs());
				SetControls();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Domain.TerminalType TerminalType
		{
			set { _terminalType = value; }
		}

		[Category("Command")]
		[Description("The command shortcut.")]
		[DefaultValue(_ShortcutStringDefault)]
		public string ShortcutString
		{
			get { return (label_Shortcut.Text); }
			set { label_Shortcut.Text = value; }
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		private void PredefinedCommandSettingsSet_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// Initially set controls and validate its contents where needed
				SetControls();

				// Move cursor to end
				textBox_Command.SelectionStart = textBox_Command.Text.Length;
			}
		}

		private void PredefinedCommandSettingsSet_Enter(object sender, EventArgs e)
		{
			_focusState = FocusState.Inactive;
			_isValidated = false;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textBox_Description_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
				SetDescription(textBox_Description.Text);
		}

		private void textBox_Command_Enter(object sender, EventArgs e)
		{
			// Clear "<Enter a command...>" if needed
			if ((_focusState == FocusState.Inactive) && !_command.IsSingleLineCommand)
				ClearCommand();

			_focusState = FocusState.HasFocus;
			_isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// </remarks>
		private void textBox_Command_Leave(object sender, System.EventArgs e)
		{
			if (_isValidated)
				_focusState = FocusState.Inactive;
			else
				_focusState = FocusState.IsLeaving;
		}

		private void textBox_Command_TextChanged(object sender, System.EventArgs e)
		{
			if (!_isSettingControls)
				_isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// </remarks>
		private void textBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				if (Model.Settings.SendCommandSettings.IsEasterEggCommand(textBox_Command.Text))
				{
					_isValidated = true;

					if (_focusState == FocusState.IsLeaving)
						_focusState = FocusState.Inactive;
					else
						_focusState = FocusState.HasFocus;

					SetSingleLineCommand(textBox_Command.Text);
					return;
				}

				int invalidTextStart;
				int invalidTextLength;
				if (Validation.ValidateSequence(this, "Command", textBox_Command.Text, out invalidTextStart, out invalidTextLength))
				{
					_isValidated = true;

					if (_focusState == FocusState.IsLeaving)
						_focusState = FocusState.Inactive;
					else
						_focusState = FocusState.HasFocus;

					SetSingleLineCommand(textBox_Command.Text);
					return;
				}

				_focusState = FocusState.HasFocus;
				textBox_Command.Select(invalidTextStart, invalidTextLength);
				e.Cancel = true;
			}
		}

		private void pathLabel_FilePath_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void checkBox_IsFile_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				if (checkBox_IsFile.Checked && !_command.IsValidFilePath)
				{
					ShowOpenFileDialog();
				}
				else
				{
					_command.IsFilePath = checkBox_IsFile.Checked;
					SetControls();
					OnCommandChanged(new EventArgs());
				}
			}
		}

		private void button_SetMultiLineCommand_Click(object sender, EventArgs e)
		{
			ShowMultiLineCommandBox(button_SetMultiLineCommand);
		}

		private void button_SetFile_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void button_Delete_Click(object sender, EventArgs e)
		{
			_command.Clear();
			SetControls();
			OnCommandChanged(new EventArgs());
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			_isSettingControls = true;

			// description
			textBox_Description.Text = _command.Description;

			if (_command.IsCommand)
			{
				// command
				textBox_Command.Visible = true;
				if (_focusState == FocusState.Inactive)
				{
					textBox_Command.Text      = _command.SingleLineCommand;
					textBox_Command.ForeColor = SystemColors.ControlText;
					textBox_Command.Font      = SystemFonts.DefaultFont;
				}

				// buttons
				button_SetMultiLineCommand.Visible = true;
				button_SetMultiLineCommand.Enabled = true;
				button_SetFile.Visible = false;
				button_SetFile.Enabled = false;

				// file path
				pathLabel_FilePath.Visible = false;
				pathLabel_FilePath.Text = "";
				checkBox_IsFile.Checked = false;

				// delete
				button_Delete.Enabled = true;
			}
			else if (_command.IsFilePath)
			{
				// command
				textBox_Command.Visible = false;
				textBox_Command.Text = "";

				// buttons
				button_SetMultiLineCommand.Visible = false;
				button_SetMultiLineCommand.Enabled = false;
				button_SetFile.Visible = true;
				button_SetFile.Enabled = true;

				// file path
				pathLabel_FilePath.Visible = true;
				if (_command.IsFilePath)
				{
					pathLabel_FilePath.Text      = _command.FilePath;
					pathLabel_FilePath.ForeColor = SystemColors.ControlText;
					pathLabel_FilePath.Font      = SystemFonts.DefaultFont;
				}
				else
				{
					pathLabel_FilePath.Text      = Command.UndefinedFilePathText;
					pathLabel_FilePath.ForeColor = SystemColors.GrayText;
					pathLabel_FilePath.Font      = Utilities.Drawing.ItalicDefaultFont;
				}

				checkBox_IsFile.Checked = true;

				// delete
				button_Delete.Enabled = true;
			}
			else
			{
				// command
				textBox_Command.Visible = true;
				if (_focusState == FocusState.Inactive)
				{
					textBox_Command.Text      = Command.EnterCommandText;
					textBox_Command.ForeColor = SystemColors.GrayText;
					textBox_Command.Font      = Utilities.Drawing.ItalicDefaultFont;
				}

				// buttons
				button_SetMultiLineCommand.Visible = true;
				button_SetMultiLineCommand.Enabled = true;
				button_SetFile.Visible = false;
				button_SetFile.Enabled = false;

				// file path
				pathLabel_FilePath.Visible = false;
				pathLabel_FilePath.Text = "";
				checkBox_IsFile.Checked = false;

				// delete
				button_Delete.Enabled = false;
			}

			_isSettingControls = false;
		}

		private void SetDescription(string description)
		{
			if (description != "")
				_command.Description = description;
			else
				_command.ClearDescription();

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

		private void ClearCommand()
		{
			_isSettingControls = true;
			textBox_Command.Text      = "";
			textBox_Command.ForeColor = SystemColors.ControlText;
			textBox_Command.Font      = SystemFonts.DefaultFont;
			_isSettingControls = false;
		}

		private void ShowMultiLineCommandBox(Control requestingControl)
		{
			// indicate multi line command
			_isSettingControls = true;
			textBox_Command.Text      = Command.MultiLineCommandText;
			textBox_Command.ForeColor = SystemColors.ControlText;
			textBox_Command.Font      = SystemFonts.DefaultFont;
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
				Parent.SelectNextControl(this, true, true, true, false);

				OnCommandChanged(new EventArgs());
			}
			else
			{
				SetControls();
				textBox_Command.Select();
			}
		}

		private void ShowOpenFileDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set File";
			switch (_terminalType)
			{
				case Domain.TerminalType.Text:
				{
					ofd.Filter = ExtensionSettings.TextFilesFilter;
					ofd.DefaultExt = ExtensionSettings.TextFilesDefault;
					break;
				}
				case Domain.TerminalType.Binary:
				{
					ofd.Filter = ExtensionSettings.BinaryFilesFilter;
					ofd.DefaultExt = ExtensionSettings.BinaryFilesDefault;
					break;
				}
				default:
				{
					throw (new NotImplementedException("Terminal type \"" + (Domain.XTerminalType)_terminalType + "\" unknown"));
				}
			}
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.SendFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.SendFilesPath = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				_command.IsFilePath = true;
				_command.FilePath = ofd.FileName;
				OnCommandChanged(new EventArgs());
				SetControls();
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
