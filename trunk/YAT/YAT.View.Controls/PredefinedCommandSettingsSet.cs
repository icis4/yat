//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MKY;
using MKY.Drawing;
using MKY.IO;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Controls
{
	/// <summary>
	/// Provides command edit. Control keeps track of the edit state to properly
	/// react on all possible edit states.
	/// </summary>
	/// <remarks>
	/// On focus enter, edit state is always reset.
	/// On focus leave, edit state is kept depending on how focus is leaving.
	/// </remarks>
	/// <remarks>
	/// Note that similar code exists in <see cref="SendText"/> and <see cref="SendFile"/>.
	/// The diff among these three implementations shall be kept as small as possible.
	/// 
	/// For a future refactoring, consider to separate the common code into a common view-model.
	/// </remarks>
	[DefaultEvent("CommandChanged")]
	public partial class PredefinedCommandSettingsSet : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum EditFocusState
		{
			EditIsInactive,
			EditHasFocus,
			IsLeavingEdit
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

		private Command command = new Command();

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool useExplicitDefaultRadix = Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault;
		private Domain.Parser.Modes parseMode = ParseModeDefault;

		private EditFocusState editFocusState = EditFocusState.EditIsInactive;
		private bool isValidated; // = false;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when any command related property has changed.")]
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

			InitializeControls();
		////SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// This property always returns a <see cref="Command"/> object, it never returns <c>null</c>.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command Command
		{
			get { return (this.command); }
			set
			{
				if (this.command != value)
				{
					if (value != null)
					{
						this.command = value;
						this.isValidated = value.IsValidText;
					}
					else
					{
						this.command = new Command();
						this.isValidated = false;
					}

					SetControls();
					OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			set
			{
				if (this.terminalType != value)
				{
					this.terminalType = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Command")]
		[Description("Whether to use an explicit default radix.")]
		[DefaultValue(Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault)]
		public virtual bool UseExplicitDefaultRadix
		{
			get { return (this.useExplicitDefaultRadix); }
			set
			{
				if (this.useExplicitDefaultRadix != value)
				{
					this.useExplicitDefaultRadix = value;

					if (value) // Explicit => Refresh the command controls.
						SetControls();

					SetExplicitDefaultRadixControls();

					if (!value) // Implicit => Reset default radix.
						ValidateAndConfirmRadix(Command.DefaultRadixDefault);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.Parser.Modes ParseMode
		{
			set
			{
				if (this.parseMode != value)
				{
					this.parseMode = value;
					SetControls();
				}
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

		/// <remarks>Dedicated function for symmetricity with <see cref="SendText"/>.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		private void SetEditFocusState(EditFocusState state)
		{
			if (this.editFocusState != state)
				this.editFocusState = state;
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
				SetExplicitDefaultRadixControls();
				SetControls();
				SetCursorToEnd();
			}
		}

		/// <remarks>
		/// Do not modify <see cref="isValidated"/>. Command may already have been validated.
		/// </remarks>
		private void PredefinedCommandSettingsSet_Enter(object sender, EventArgs e)
		{
			SetEditFocusState(EditFocusState.EditIsInactive);
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_ExplicitDefaultRadix_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.Radix radix = this.command.DefaultRadix;
				Domain.RadixEx selectedItem = comboBox_ExplicitDefaultRadix.SelectedItem as Domain.RadixEx;
				if (selectedItem != null) // Can be 'null' when validating all controls before an item got selected.
					radix = selectedItem;

				if (!ValidateAndConfirmRadix(radix))
				{
					e.Cancel = true;

					// Automatically reset the radix for convenience:
					comboBox_ExplicitDefaultRadix.SelectedItem = this.command.DefaultRadix;
					SetControls();
				}
			}
		}

		private bool ValidateAndConfirmRadix(Domain.Radix radix)
		{
			if (this.command.IsSingleLineText)
			{
				string s = this.command.SingleLineText;
				if (Utilities.ValidationHelper.ValidateRadix(this, "default radix", s, radix, this.parseMode))
				{
					this.command.DefaultRadix = radix;
				////this.isValidated is intentionally not set, as the validation above only verifies the changed radix but not the text.
					return (true);
				}
			}
			else if (this.command.IsMultiLineText)
			{
				bool isValid = true;

				foreach (string s in this.command.MultiLineText)
				{
					if (Utilities.ValidationHelper.ValidateRadix(this, "default radix", s, radix, this.parseMode))
						isValid = false;
				}

				if (isValid)
				{
					this.command.DefaultRadix = radix;
				////this.isValidated is intentionally not set, as the validation above only verifies the changed radix but not the text.
					return (true);
				}
			}
			else // Neither single- nor multi-line, simply set the radix.
			{
				this.command.DefaultRadix = radix;
				return (true);
			}

			return (false);
		}

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
			if ((this.editFocusState == EditFocusState.EditIsInactive) && !this.command.IsText)
			{
				this.isSettingControls.Enter();
				textBox_SingleLineText.Text      = "";
				textBox_SingleLineText.ForeColor = SystemColors.ControlText;
				textBox_SingleLineText.Font      = SystemFonts.DefaultFont;
				this.isSettingControls.Leave();
			}

			SetEditFocusState(EditFocusState.EditHasFocus);

			// No need to set this.isValidated = false yet. The 'TextChanged' event will do so.
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. ComboBox.Leave()
		/// 2. ComboBox.Validating()
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void textBox_SingleLineText_Leave(object sender, EventArgs e)
		{
			if (this.isValidated)
				SetEditFocusState(EditFocusState.EditIsInactive);
			else
				SetEditFocusState(EditFocusState.IsLeavingEdit);
		}

		private void textBox_SingleLineText_TextChanged(object sender, EventArgs e)
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

					if (this.editFocusState == EditFocusState.IsLeavingEdit)
								SetEditFocusState(EditFocusState.EditIsInactive);

					ConfirmSingleLineText(textBox_SingleLineText.Text);
					return;
				}

				// Single line => Validate!
				int invalidTextStart;
				int invalidTextLength;
				if (Utilities.ValidationHelper.ValidateText(this, "text", textBox_SingleLineText.Text, out invalidTextStart, out invalidTextLength, this.command.DefaultRadix, this.parseMode))
				{
					this.isValidated = true;

					if (this.editFocusState == EditFocusState.IsLeavingEdit)
						SetEditFocusState(EditFocusState.EditIsInactive);

					ConfirmSingleLineText(textBox_SingleLineText.Text);
					return;
				}

				SetEditFocusState(EditFocusState.EditHasFocus);
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
				ConfirmDescription(textBox_Description.Text);
		}

		private void button_Delete_Click(object sender, EventArgs e)
		{
			this.command.Clear();
			SetControls();
			OnCommandChanged(EventArgs.Empty);
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------

		private void InitializeControls()
		{
			this.isSettingControls.Enter();

			comboBox_ExplicitDefaultRadix.Items.Clear();
			comboBox_ExplicitDefaultRadix.Items.AddRange(Domain.RadixEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetExplicitDefaultRadixControls()
		{
			this.isSettingControls.Enter();

			splitContainer_ExplicitDefaultRadix.Panel1Collapsed = !this.useExplicitDefaultRadix;

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			if (this.useExplicitDefaultRadix)
				SelectionHelper.Select(comboBox_ExplicitDefaultRadix, (Domain.RadixEx)this.command.DefaultRadix, (Domain.RadixEx)this.command.DefaultRadix);
			else
				SelectionHelper.Deselect(comboBox_ExplicitDefaultRadix);

			// Description:
			textBox_Description.Text = this.command.Description;

			if (this.command.IsText)
			{
				// Command:
				textBox_SingleLineText.Visible = true;
				if (this.editFocusState == EditFocusState.EditIsInactive)
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
				if (this.editFocusState == EditFocusState.EditIsInactive)
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

		private void SetCursorToEnd()
		{
			this.isSettingControls.Enter();

			textBox_SingleLineText.SelectionStart = textBox_SingleLineText.Text.Length;

			this.isSettingControls.Leave();
		}

		#endregion

		#region Non-Public Methods > Handle Command
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Handle Command
		//------------------------------------------------------------------------------------------

		private void ConfirmDescription(string description)
		{
			if (!string.IsNullOrEmpty(description))
				this.command.Description = description;
			else
				this.command.ClearDescription();

			SetControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmSingleLineText(string text)
		{
			this.command.SingleLineText = text;

			SetControls();
			OnCommandChanged(EventArgs.Empty);
		}

		#endregion

		#region Non-Public Methods > Multi-Line Text
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Multi-Line Text
		//------------------------------------------------------------------------------------------

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
			MultiLineBox f = new MultiLineBox(this.command, formStartupLocation, this.command.DefaultRadix, this.parseMode);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				this.command = f.CommandResult;
				this.isValidated = true; // Command has been validated by multi-line box.

				SetControls();
				OnCommandChanged(EventArgs.Empty);
			}
			else
			{
				SetControls();
			////OnCommandChanged() is not called, nothing has changed.
			}

			textBox_Description.Select();
		}

		#endregion

		#region Non-Public Methods > File
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > File
		//------------------------------------------------------------------------------------------

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
