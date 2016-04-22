//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MKY;
using MKY.Drawing;
using MKY.IO;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Gui.Utilities;
using YAT.Model.Types;
using YAT.Settings.Application;

#endregion

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

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypeDefault;
		private const Domain.Parser.Modes ParseModeDefault = Domain.Parser.Modes.Default;
		private const string ShortcutStringDefault = "Shift+F1";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private Domain.Parser.Modes parseMode = ParseModeDefault;

		private Model.Types.Command command = new Model.Types.Command();

		private FocusState focusState = FocusState.Inactive;
		private bool isValidated; // = false;

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

			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Command")]
		[Description("The terminal type related to the command.")]
		[DefaultValue(TerminalTypeDefault)]
		public virtual Domain.TerminalType TerminalType
		{
			get { return (this.terminalType); }
			set
			{
				this.terminalType = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[Category("Command")]
		[Description("The parse mode related to the command.")]
		[DefaultValue(ParseModeDefault)]
		public virtual Domain.Parser.Modes ParseMode
		{
			get { return (this.parseMode); }
			set
			{
				this.parseMode = value;
				SetControls();
			}
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

		/// <summary>
		/// This property always returns a <see cref="Model.Types.Command"/> object,
		/// it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command Command
		{
			get { return (this.command); }
			set
			{
				if (value != null)
					this.command = value;
				else
					this.command = new Command();

				OnCommandChanged(EventArgs.Empty);
				SetControls();
			}
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
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void PredefinedCommandSettingsSet_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();

				// Move cursor to end
				textBox_SingleLineText.SelectionStart = textBox_SingleLineText.Text.Length;
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
					OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		private void textBox_SingleLineText_Enter(object sender, EventArgs e)
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
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void textBox_SingleLineText_Leave(object sender, System.EventArgs e)
		{
			if (this.isValidated)
				this.focusState = FocusState.Inactive;
			else
				this.focusState = FocusState.IsLeaving;
		}

		private void textBox_SingleLineText_TextChanged(object sender, System.EventArgs e)
		{
			if (!this.isSettingControls)
				this.isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void textBox_SingleLineText_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				if (Model.Settings.SendTextSettings.IsEasterEggCommand(textBox_SingleLineText.Text))
				{
					this.isValidated = true;

					if (this.focusState == FocusState.IsLeaving)
						this.focusState = FocusState.Inactive;
					else
						this.focusState = FocusState.HasFocus;

					SetSingleLineText(textBox_SingleLineText.Text);
					return;
				}

				int invalidTextStart;
				int invalidTextLength;
				if (Validation.ValidateText(this, "text", textBox_SingleLineText.Text, /* FR#238 add this.defaultRadix */ this.parseMode, out invalidTextStart, out invalidTextLength))
				{
					this.isValidated = true;

					if (this.focusState == FocusState.IsLeaving)
						this.focusState = FocusState.Inactive;
					else
						this.focusState = FocusState.HasFocus;

					SetSingleLineText(textBox_SingleLineText.Text);
					return;
				}

				this.focusState = FocusState.HasFocus;
				textBox_SingleLineText.Select(invalidTextStart, invalidTextLength);
				e.Cancel = true;
			}
		}

		private void pathLabel_FilePath_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void button_SetMultiLineText_Click(object sender, EventArgs e)
		{
			ShowMultiLineBox(button_SetMultiLineText);
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
			OnCommandChanged(EventArgs.Empty);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			// Description:
			textBox_Description.Text = this.command.Description;

			if (this.command.IsText)
			{
				// Command:
				textBox_SingleLineText.Visible = true;
				if (this.focusState == FocusState.Inactive)
				{
					if (textBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
						textBox_SingleLineText.ForeColor = SystemColors.ControlText;

					if (textBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
						textBox_SingleLineText.Font = SystemFonts.DefaultFont;

					textBox_SingleLineText.Text = this.command.SingleLineText;
				}

				// Buttons:
				button_SetMultiLineText.Visible = true;
				button_SetMultiLineText.Enabled = true;
				button_SetFile.Visible = false;
				button_SetFile.Enabled = false;

				// File path:
				pathLabel_FilePath.Visible = false;
				pathLabel_FilePath.Text = "";
				checkBox_IsFile.Checked = false;

				// Delete:
				button_Delete.Enabled = true;
			}
			else if (this.command.IsFilePath)
			{
				// Command:
				textBox_SingleLineText.Visible = false;
				textBox_SingleLineText.Text = "";

				// Buttons:
				button_SetMultiLineText.Visible = false;
				button_SetMultiLineText.Enabled = false;
				button_SetFile.Visible = true;
				button_SetFile.Enabled = true;

				// File path:
				pathLabel_FilePath.Visible = true;
				if (this.command.IsFilePath)
				{
					if (pathLabel_FilePath.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
						pathLabel_FilePath.ForeColor = SystemColors.ControlText;

					if (pathLabel_FilePath.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
						pathLabel_FilePath.Font = SystemFonts.DefaultFont;

					pathLabel_FilePath.Text = this.command.FilePath;
				}
				else
				{
					if (pathLabel_FilePath.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						pathLabel_FilePath.ForeColor = SystemColors.GrayText;

					if (pathLabel_FilePath.Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
						pathLabel_FilePath.Font = DrawingEx.DefaultFontItalic;

					pathLabel_FilePath.Text = Command.UndefinedFilePathText;
				}

				checkBox_IsFile.Checked = true;

				// Delete:
				button_Delete.Enabled = true;
			}
			else
			{
				// Command:
				textBox_SingleLineText.Visible = true;
				if (this.focusState == FocusState.Inactive)
				{
					if (textBox_SingleLineText.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						textBox_SingleLineText.ForeColor = SystemColors.GrayText;

					if (textBox_SingleLineText.Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
						textBox_SingleLineText.Font = DrawingEx.DefaultFontItalic;

					textBox_SingleLineText.Text = Command.EnterTextText;
				}

				// Buttons:
				button_SetMultiLineText.Visible = true;
				button_SetMultiLineText.Enabled = true;
				button_SetFile.Visible = false;
				button_SetFile.Enabled = false;

				// File path:
				pathLabel_FilePath.Visible = false;
				pathLabel_FilePath.Text = "";
				checkBox_IsFile.Checked = false;

				// Delete:
				button_Delete.Enabled = false;
			}

			this.isSettingControls.Leave();
		}

		private void SetDescription(string description)
		{
			if (!string.IsNullOrEmpty(description))
				this.command.Description = description;
			else
				this.command.ClearDescription();

			SetControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void SetSingleLineText(string text)
		{
			this.command.IsFilePath = false;
			this.command.SingleLineText = text;

			SetControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ClearCommand()
		{
			this.isSettingControls.Enter();
			textBox_SingleLineText.Text      = "";
			textBox_SingleLineText.ForeColor = SystemColors.ControlText;
			textBox_SingleLineText.Font      = SystemFonts.DefaultFont;
			this.isSettingControls.Leave();
		}

		/// <remarks>
		/// Almost duplicated code in <see cref="SendText.ShowMultiLineBox"/>.
		/// </remarks>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowMultiLineBox(Control requestingControl)
		{
			// Indicate multi-line text:
			this.isSettingControls.Enter();
			textBox_SingleLineText.Text      = Command.MultiLineTextText;
			textBox_SingleLineText.ForeColor = SystemColors.ControlText;
			textBox_SingleLineText.Font      = SystemFonts.DefaultFont;
			this.isSettingControls.Leave();

			// Calculate startup location:
			Rectangle area = requestingControl.RectangleToScreen(requestingControl.DisplayRectangle);
			Point formStartupLocation = new Point();
			formStartupLocation.X = area.X + area.Width;
			formStartupLocation.Y = area.Y + area.Height;

			// Show multi-line box:
			MultiLineBox f = new MultiLineBox(this.command, formStartupLocation, this.parseMode);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				this.command = f.CommandResult;
				this.isValidated = true; // Command has been validated by multi-line box.

				SetControls();
				textBox_Description.Select();
				OnCommandChanged(EventArgs.Empty);
			}
			else
			{
				SetControls();
				textBox_Description.Select();
				// Do not call OnCommandChanged(), nothing has changed.
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenFileDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set File";

			string initialExtension;
			switch (this.terminalType)
			{
				case Domain.TerminalType.Binary:
				{
					initialExtension = ApplicationSettings.LocalUserSettings.Extensions.BinarySendFiles;

					ofd.Filter      = ExtensionHelper.BinaryFilesFilter;
					ofd.FilterIndex = ExtensionHelper.BinaryFilesFilterHelper(initialExtension);
					break;
				}

				case Domain.TerminalType.Text:
				default:
				{
					initialExtension = ApplicationSettings.LocalUserSettings.Extensions.TextSendFiles;

					ofd.Filter      = ExtensionHelper.TextFilesFilter;
					ofd.FilterIndex = ExtensionHelper.TextFilesFilterHelper(initialExtension);
					break;
				}
			}

			ofd.DefaultExt = PathEx.DenormalizeExtension(initialExtension);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.SendFiles;

			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				Refresh();

				switch (this.terminalType)
				{
					case Domain.TerminalType.Binary:
					{
						ApplicationSettings.LocalUserSettings.Extensions.BinarySendFiles = Path.GetExtension(ofd.FileName);
						break;
					}

					case Domain.TerminalType.Text:
					default:
					{
						ApplicationSettings.LocalUserSettings.Extensions.TextSendFiles = Path.GetExtension(ofd.FileName);
						break;
					}
				}

				ApplicationSettings.LocalUserSettings.Paths.SendFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.command.IsFilePath = true;
				this.command.FilePath = ofd.FileName;
				OnCommandChanged(EventArgs.Empty);
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
