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
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("CommandChanged")]
	public partial class PredefinedCommandSettingsSet : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum TextEditState
		{
			Inactive,
			HasFocusButIsNotValidated,
			HasFocusAndIsValidated,
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

		private TextEditState _commandEditState = TextEditState.Inactive;

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
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Command always returns a Command object, it never returns null.
		/// </summary>
		[Browsable(false)]
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
		[DefaultValue(_TerminalTypeDefault)]
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

				// initially set controls and validate its contents where needed
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textBox_Description_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				if (textBox_Description.Text != "")
					SetDescription(textBox_Description.Text);
			}
		}

		private void textBox_Command_Enter(object sender, EventArgs e)
		{
			// clear "<Enter a command...>" if needed
			if (!_command.IsSingleLineCommand)
			{
				_isSettingControls = true;
				textBox_Command.Text = "";
				_isSettingControls = false;
			}
			_commandEditState = TextEditState.HasFocusButIsNotValidated;
		}

		private void textBox_Command_Leave(object sender, System.EventArgs e)
		{
			_commandEditState = TextEditState.IsLeaving;
		}

		private void textBox_Command_TextChanged(object sender, System.EventArgs e)
		{
			if (!_isSettingControls)
				_commandEditState = TextEditState.HasFocusButIsNotValidated;
		}

		private void textBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				if (Model.Settings.SendCommandSettings.IsEasterEggCommand(textBox_Command.Text))
				{
					if (_commandEditState == TextEditState.IsLeaving)
						_commandEditState = TextEditState.Inactive;
					else
						_commandEditState = TextEditState.HasFocusAndIsValidated;

					SetSingleLineCommand(textBox_Command.Text);
					return;
				}
				if (Validation.ValidateSequence(this, "Command", textBox_Command.Text))
				{
					if (_commandEditState == TextEditState.IsLeaving)
						_commandEditState = TextEditState.Inactive;
					else
						_commandEditState = TextEditState.HasFocusAndIsValidated;

					SetSingleLineCommand(textBox_Command.Text);
					return;
				}
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
				textBox_Command.Text = _command.SingleLineCommand;

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
					pathLabel_FilePath.Text = _command.FilePath;
				else
					pathLabel_FilePath.Text = Command.UndefinedFilePathText;
				checkBox_IsFile.Checked = true;

				// delete
				button_Delete.Enabled = true;
			}
			else
			{
				// command
				textBox_Command.Visible = true;
				if (_commandEditState == TextEditState.Inactive)
					textBox_Command.Text = Command.EnterCommandText;

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
			_command.Description = description;

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
			textBox_Command.Text = Command.MultiLineCommandText;
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
				ApplicationSettings.SaveLocalUser();

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
