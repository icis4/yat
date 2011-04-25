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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MKY.Event;

using YAT.Gui.Utilities;
using YAT.Model.Types;
using YAT.Settings;
using YAT.Settings.Application;

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

		private const string ShortcutStringDefault = "Shift+F1";
		private const Domain.TerminalType TerminalTypeDefault = Domain.TerminalType.Text;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		private Model.Types.Command command = new Model.Types.Command();
		private Domain.TerminalType terminalType = TerminalTypeDefault;

		private FocusState focusState = FocusState.Inactive;
		private bool isValidated = false;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when any of the commands properties have changed.")]
		public event EventHandler CommandChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
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
		public virtual Model.Types.Command Command
		{
			get { return (this.command); }
			set
			{
				if (value != null)
					this.command = value;
				else
					this.command = new Model.Types.Command();

				OnCommandChanged(new EventArgs());
				SetControls();
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			set { this.terminalType = value; }
		}

		/// <summary></summary>
		[Category("Command")]
		[Description("The command shortcut.")]
		[DefaultValue(ShortcutStringDefault)]
		public virtual string ShortcutString
		{
			get { return (label_Shortcut.Text); }
			set { label_Shortcut.Text = value; }
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void PredefinedCommandSettingsSet_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();

				// Move cursor to end
				textBox_Command.SelectionStart = textBox_Command.Text.Length;
			}
		}

		private void PredefinedCommandSettingsSet_Enter(object sender, EventArgs e)
		{
			this.focusState = FocusState.Inactive;
			this.isValidated = false;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_IsFile_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				if (checkBox_IsFile.Checked && !this.command.IsValidFilePath)
				{
					ShowOpenFileDialog();
				}
				else
				{
					this.command.IsFilePath = checkBox_IsFile.Checked;
					SetControls();
					OnCommandChanged(new EventArgs());
				}
			}
		}

		private void textBox_Command_Enter(object sender, EventArgs e)
		{
			// Clear "<Enter a command...>" if needed.
			if ((this.focusState == FocusState.Inactive) && !this.command.IsSingleLineText)
				ClearCommand();

			this.focusState = FocusState.HasFocus;
			this.isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// </remarks>
		private void textBox_Command_Leave(object sender, System.EventArgs e)
		{
			if (this.isValidated)
				this.focusState = FocusState.Inactive;
			else
				this.focusState = FocusState.IsLeaving;
		}

		private void textBox_Command_TextChanged(object sender, System.EventArgs e)
		{
			if (!this.isSettingControls)
				this.isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// </remarks>
		private void textBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				if (Model.Settings.SendCommandSettings.IsEasterEggCommand(textBox_Command.Text))
				{
					this.isValidated = true;

					if (this.focusState == FocusState.IsLeaving)
						this.focusState = FocusState.Inactive;
					else
						this.focusState = FocusState.HasFocus;

					SetSingleLineCommand(textBox_Command.Text);
					return;
				}

				int invalidTextStart;
				int invalidTextLength;
				if (Validation.ValidateSequence(this, "Command", textBox_Command.Text, out invalidTextStart, out invalidTextLength))
				{
					this.isValidated = true;

					if (this.focusState == FocusState.IsLeaving)
						this.focusState = FocusState.Inactive;
					else
						this.focusState = FocusState.HasFocus;

					SetSingleLineCommand(textBox_Command.Text);
					return;
				}

				this.focusState = FocusState.HasFocus;
				textBox_Command.Select(invalidTextStart, invalidTextLength);
				e.Cancel = true;
			}
		}

		private void pathLabel_FilePath_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void button_SetMultiLineCommand_Click(object sender, EventArgs e)
		{
			ShowMultiLineCommandBox(button_SetMultiLineCommand);
		}

		private void button_SetFile_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void textBox_Description_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
				SetDescription(textBox_Description.Text);
		}

		private void button_Delete_Click(object sender, EventArgs e)
		{
			this.command.Clear();
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
			this.isSettingControls = true;

			// description
			textBox_Description.Text = this.command.Description;

			if (this.command.IsText)
			{
				// command
				textBox_Command.Visible = true;
				if (this.focusState == FocusState.Inactive)
				{
					textBox_Command.Text      = this.command.SingleLineText;
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
			else if (this.command.IsFilePath)
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
				if (this.command.IsFilePath)
				{
					pathLabel_FilePath.Text      = this.command.FilePath;
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
				if (this.focusState == FocusState.Inactive)
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

			this.isSettingControls = false;
		}

		private void SetDescription(string description)
		{
			if (description.Length > 0)
				this.command.Description = description;
			else
				this.command.ClearDescription();

			SetControls();
			OnCommandChanged(new EventArgs());
		}

		private void SetSingleLineCommand(string commandLine)
		{
			this.command.IsFilePath = false;
			this.command.SingleLineText = commandLine;

			SetControls();
			OnCommandChanged(new EventArgs());
		}

		private void ClearCommand()
		{
			this.isSettingControls = true;
			textBox_Command.Text      = "";
			textBox_Command.ForeColor = SystemColors.ControlText;
			textBox_Command.Font      = SystemFonts.DefaultFont;
			this.isSettingControls = false;
		}

		/// <remarks>
		/// Almost duplicated code in <see cref="YAT.Gui.Controls.SendCommand.ShowMultiLineCommandBox"/>.
		/// </remarks>
		private void ShowMultiLineCommandBox(Control requestingControl)
		{
			// Indicate multi line command.
			this.isSettingControls = true;
			textBox_Command.Text      = Command.MultiLineCommandText;
			textBox_Command.ForeColor = SystemColors.ControlText;
			textBox_Command.Font      = SystemFonts.DefaultFont;
			this.isSettingControls = false;

			// Calculate startup location.
			Rectangle area = requestingControl.RectangleToScreen(requestingControl.DisplayRectangle);
			Point formStartupLocation = new Point();
			formStartupLocation.X = area.X + area.Width;
			formStartupLocation.Y = area.Y + area.Height;

			// Show multi line box.
			MultiLineBox f = new MultiLineBox(this.command, formStartupLocation);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				this.command = f.CommandResult;
				this.isValidated = true; // Command has been validated by multi line box.

				SetControls();
				textBox_Description.Select();

				OnCommandChanged(new EventArgs());
			}
			else
			{
				SetControls();
				textBox_Description.Select();
			}
		}

		private void ShowOpenFileDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set File";
			switch (this.terminalType)
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
					throw (new NotImplementedException("Terminal type \"" + (Domain.TerminalTypeEx)this.terminalType + "\" unknown"));
				}
			}
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.SendFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.SendFilesPath = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.command.IsFilePath = true;
				this.command.FilePath = ofd.FileName;
				OnCommandChanged(new EventArgs());
			}

			// Set controls in any case:
			//   OK => Command needs to be refreshed.
			//   C  => Checkbox needs to be refreshed.
			SetControls();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
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
