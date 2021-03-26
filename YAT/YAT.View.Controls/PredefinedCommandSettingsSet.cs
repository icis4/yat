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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	/// Provides command edit. Control keeps track of the edit state to properly react on all
	/// possible edit states.
	/// </summary>
	/// <remarks>
	/// On focus enter, edit state is always reset.
	/// On focus leave, edit state is kept depending on how focus is leaving.
	/// </remarks>
	/// <remarks>
	/// Note that similar code exists in <see cref="SendText"/> and <see cref="SendFile"/>.
	/// The diff among these three implementations shall be kept as small as possible.
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
		private const Domain.Parser.Mode ParseModeForTextDefault = Domain.Parser.Mode.Default;

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
		private Domain.Parser.Mode parseModeForText = ParseModeForTextDefault;
		private string rootDirectoryForFile; // = null;

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
				// Attention:
				// Similar code exists in SendText.Command.set{}.
				// Changes here may have to be applied there too.

				if (this.command != value)
				{
					if (value != null)
					{
						this.command = value;
						this.isValidated = value.IsValid(this.parseModeForText, this.rootDirectoryForFile);
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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			get { return (this.terminalType); }
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
		[Category("Behavior")]
		[Description("Whether to use an explicit default radix.")]
		[DefaultValue(Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault)]
		public virtual bool UseExplicitDefaultRadix
		{
			get { return (this.useExplicitDefaultRadix); }
			set
			{
				// Attention:
				// Similar code exists in SendText.UseExplicitDefaultRadix.set{}.
				// Changes here may have to be applied there too.

				if (this.useExplicitDefaultRadix != value)
				{
					this.useExplicitDefaultRadix = value;

					if (value) // Explicit => Simply refresh the command (the radix hasn't changed (yet)).
						SetControls();

					SetExplicitDefaultRadixControls();

					if (!value) // Implicit => Reset default radix.
						ValidateAndConfirmDefaultRadix(Command.DefaultRadixDefault); // \remind (2019-09-01 / MKY / bug #469)
				}                                                                    // Better set 'isValidated' to 'false'
			}                                                                        // and validate later. But that requires
		}                                                                            // some work and re-testing => future.

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.Parser.Mode ParseModeForText
		{
			get { return (this.parseModeForText); }
			set
			{
				// Attention:
				// Similar code exists in SendText.ParseMode.set{}.
				// Changes here may have to be applied there too.

				if (this.parseModeForText != value)
				{
					this.parseModeForText = value;
					SetControls();

					// \remind (2019-09-01 / MKY / bug #469)
					// Re-validation would be needed because change could enable or disable escapes.
					// But neither ValidateChildren() nor setting 'isValidated' to 'false' and
					// validate later is sufficient. Fix requires some work and re-testing => future.
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string RootDirectoryForFile
		{
			get { return (this.rootDirectoryForFile); }
			set
			{
				if (this.rootDirectoryForFile != value)
				{
					this.rootDirectoryForFile = value;
					SetControls();
				}
			}
		}

		/// <remarks>Dedicated function for symmetricity with <see cref="SendText"/>.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		private void SetEditFocusState(EditFocusState state)
		{
			if (this.editFocusState != state)
				this.editFocusState = state;
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Required to be called "from the outside" because...
		/// ...if called in the constructor of the control, SetControls() has not yet been called.
		/// ...if called in the 'Enter' handler of the control, it doesn't work ?!?
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public virtual void PrepareUserInput()
		{
			this.isSettingControls.Enter();
			try
			{
				if (this.command.IsFilePath)
				{
					button_SetFile.Select();
				}
				else  // command.IsText:
				{
					textBox_SingleLineText.Select();
					textBox_SingleLineText.SelectionStart = textBox_SingleLineText.Text.Length;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <summary>
		/// Clears the command.
		/// </summary>
		public virtual void ClearCommand()
		{
			Command = null;
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Flag only used by the following event handler.
		/// </summary>
		private bool PredefinedCommandSettingsSet_Paint_IsFirst { get; set; } = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void PredefinedCommandSettingsSet_Paint(object sender, PaintEventArgs e)
		{
			if (PredefinedCommandSettingsSet_Paint_IsFirst) {
				PredefinedCommandSettingsSet_Paint_IsFirst = false;

				SetExplicitDefaultRadixControls();
				SetControls();
			}
		}

		private void PredefinedCommandSettingsSet_Enter(object sender, EventArgs e)
		{
			// Reset the focus state each time the control is entered.
			// In case the text field initially gets the focus, that event will set the state.
			// In case [Default Radix] initially gets the focus, the state will remain inactive.
			SetEditFocusState(EditFocusState.EditIsInactive);

			// \remind (2019-08-08 / MKY)
			// There is a not-so-nice behavior in case [Default Radix] is active:
			//  > Upon entering the dialog, the text field gets selected, OK.
			//  > When using an Alt+<0..9> shortcut, the [Default Radix] gets selected.
			// No solution found, but considered acceptable since most won't use [Default Radix].
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

	////private void comboBox_ExplicitDefaultRadix_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since           "         _Validating() below gets called anyway.

		private void comboBox_ExplicitDefaultRadix_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (!comboBox_ExplicitDefaultRadix.Visible) // Same implementation as SendText.comboBox_ExplicitDefaultRadix_Validating().
				return;

			// Attention:
			// Similar code exists in SendText.comboBox_ExplicitDefaultRadix_Validating().
			// Changes here may have to be applied there too.

			Domain.Radix defaultRadix = this.command.DefaultRadix;
			Domain.RadixEx selectedItem = comboBox_ExplicitDefaultRadix.SelectedItem as Domain.RadixEx;
			if (selectedItem != null) // Can be 'null' when validating all controls before an item got selected.
				defaultRadix = selectedItem;

			if (!ValidateAndConfirmDefaultRadix(defaultRadix))
			{
				e.Cancel = true;

				// Automatically reset the radix for convenience:
				comboBox_ExplicitDefaultRadix.SelectedItem = this.command.DefaultRadix;
				SetControls();
			}
		}

		private bool ValidateAndConfirmDefaultRadix(Domain.Radix defaultRadix)
		{
			// Attention:
			// Similar code exists in SendText.ValidateAndConfirmRadix().
			// Changes here may have to be applied there too.

			if (this.command.IsSingleLineText)
			{
				var text = this.command.SingleLineText;
				if (Utilities.ValidationHelper.ValidateRadix(this, "default radix", text, this.parseModeForText, defaultRadix))
				{
					ConfirmDefaultRadix(defaultRadix);
					return (true);
				}
			}
			else if (this.command.IsMultiLineText)
			{
				bool isValid = true;

				foreach (var line in this.command.MultiLineText)
				{
					if (Utilities.ValidationHelper.ValidateRadix(this, "default radix", line, this.parseModeForText, defaultRadix))
						isValid = false;
				}

				if (isValid)
				{
					ConfirmDefaultRadix(defaultRadix);
					return (true);
				}
			}
			else // Neither single- nor multi-line, simply set the radix.
			{
				this.command.DefaultRadix = defaultRadix;
				return (true);
			}

			return (false);
		}

		private void checkBox_IsFile_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (checkBox_IsFile.Checked && !this.command.IsValidFilePath(this.rootDirectoryForFile))
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

		private void textBox_SingleLineText_Enter(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in SendText.comboBox_SingleLineText_Enter().
			// Changes here may have to be applied there too.

			// Clear "<Enter a command...>" if needed.
			if ((this.editFocusState == EditFocusState.EditIsInactive) && !this.command.IsText)
			{
				this.isSettingControls.Enter();
				try
				{
					textBox_SingleLineText.Text = "";

					if (textBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
						textBox_SingleLineText.ForeColor = SystemColors.ControlText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
					                                               //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
					if (textBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
						textBox_SingleLineText.Font = SystemFonts.DefaultFont;  // Improves because 'Font' is managed by a 'PropertyStore'.
				}
				finally
				{
					this.isSettingControls.Leave();
				}
			}

			SetEditFocusState(EditFocusState.EditHasFocus);

			// No need to set this.isValidated = false yet. The "TextChanged" event will do so.
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. TextBox.Leave()
		/// 2. TextBox.Validating()
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void textBox_SingleLineText_Leave(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in SendText.comboBox_SingleLineText_Leave().
			// Changes here may have to be applied there too.

			if (this.isValidated)
				SetEditFocusState(EditFocusState.EditIsInactive);
			else
				SetEditFocusState(EditFocusState.IsLeavingEdit);
		}

		private void textBox_SingleLineText_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			// Attention:
			// Similar code exists in SendText.comboBox_SingleLineText_TextChanged().
			// Changes here may have to be applied there too.

			this.isValidated = false;
		}

		/// <remarks>
		/// Event sequence when focus is leaving, e.g. TAB is pressed.
		/// 1. TextBox.Leave()
		/// 2. TextBox.Validating()
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void textBox_SingleLineText_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			// Attention:
			// Similar code exists in SendText.comboBox_SingleLineText_Validating().
			// Changes here may have to be applied there too.

			if (!this.isValidated)
			{
				// Easter egg ;-)
				if (Model.Settings.SendTextSettings.IsEasterEggCommand(textBox_SingleLineText.Text))
				{
					if (this.editFocusState == EditFocusState.IsLeavingEdit)
						SetEditFocusState(EditFocusState.EditIsInactive);

					ConfirmSingleLineText(textBox_SingleLineText.Text);
					return;
				}

				// Single line => Validate!
				int invalidTextStart;
				int invalidTextLength;
				if (Utilities.ValidationHelper.ValidateText(this, "command text", textBox_SingleLineText.Text, out invalidTextStart, out invalidTextLength, this.parseModeForText, this.command.DefaultRadix))
				{
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
			if (this.isSettingControls)
				return;

			var description = textBox_Description.Text;
			if (description != this.command.SingleLineText)
				ConfirmDescription(textBox_Description.Text);

			// Do not explicitly set description as long it did not get changed, otherwise all
			// commands eventually get an explicit description equal to the default description.
			//              ^ e.g. when tabbing through dialog
			//                     or after returning from multi-line text dialog.
		}

		private void button_Clear_Click(object sender, EventArgs e)
		{
			checkBox_IsFile.Select(); // Move cursor to a "safe" location as the clear button will
			                          // get disabled, which would lead the cursor being moved into
			ClearCommand();           // the text field, thus immediately creating a command again!
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
			try
			{
				// Attention:
				// Same code exists in SendFile.InitializeControls().
				// Changes here must be applied there too.

				comboBox_ExplicitDefaultRadix.Items.Clear();
				comboBox_ExplicitDefaultRadix.Items.AddRange(Domain.RadixEx.GetItems());
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetExplicitDefaultRadixControls()
		{
			this.isSettingControls.Enter();
			try
			{
				// Attention:
				// Same code exists in SendFile.SetExplicitDefaultRadixControls().
				// Changes here must be applied there too.

				splitContainer_ExplicitDefaultRadix.Panel1Collapsed = !this.useExplicitDefaultRadix;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				// Default radix:

				// Attention:
				// Same code exists in SendFile.SetRecentAndCommandControls().
				// Changes here must be applied there too.

				if (this.useExplicitDefaultRadix)
				{
					bool explicitDefaultRadixIsTakenIntoAccount = true;

					if ((this.command != null) && (this.command.IsFilePath))
					{
						if (this.terminalType == Domain.TerminalType.Text)
						{
							explicitDefaultRadixIsTakenIntoAccount = true; // Supported for text, RTF, XML,...
						}
						else   // incl. (Type == Domain.TerminalType.Binary)
						{
							explicitDefaultRadixIsTakenIntoAccount = false; // Not supported for any binary format.

							if (ExtensionHelper.IsTextFile(this.command.FilePath) ||
								ExtensionHelper.IsXmlFile(this.command.FilePath))
							{
								explicitDefaultRadixIsTakenIntoAccount = true; // Supported for text and XML.
							}
						}
					}

					comboBox_ExplicitDefaultRadix.Enabled = explicitDefaultRadixIsTakenIntoAccount;
					comboBox_ExplicitDefaultRadix.TabStop = explicitDefaultRadixIsTakenIntoAccount;

					Domain.RadixEx resultingDefaultRadix = this.command.DefaultRadix;
					ComboBoxHelper.Select(comboBox_ExplicitDefaultRadix, resultingDefaultRadix, resultingDefaultRadix);

					// Note: It is not possible to select 'None' as that item is not contained in the
					// drop down list and the 'DropDownStyle' is set to 'ComboBoxStyle.DropDownList'.
				}
				else
				{
					comboBox_ExplicitDefaultRadix.Enabled = false;

					ComboBoxHelper.Deselect(comboBox_ExplicitDefaultRadix);
				}

				// Description:
				textBox_Description.Text = this.command.Description;

				if (this.command.IsText)
				{
					// Command:
					textBox_SingleLineText.Visible = true;
					if (this.editFocusState == EditFocusState.EditIsInactive)
					{
						if (textBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
							textBox_SingleLineText.ForeColor = SystemColors.ControlText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
						                                               //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
						if (textBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
							textBox_SingleLineText.Font = SystemFonts.DefaultFont;  // Improves because 'Font' is managed by a 'PropertyStore'.

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
					button_Clear.Enabled = true;
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
							pathLabel_FilePath.ForeColor = SystemColors.ControlText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
						                                           //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
						if (pathLabel_FilePath.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
							pathLabel_FilePath.Font = SystemFonts.DefaultFont;  // Improves because 'Font' is managed by a 'PropertyStore'.

						pathLabel_FilePath.Text = this.command.FilePath;
					}
					else
					{
						if (pathLabel_FilePath.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
							pathLabel_FilePath.ForeColor = SystemColors.GrayText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
						                                         //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
						if (pathLabel_FilePath.Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
							pathLabel_FilePath.Font = DrawingEx.DefaultFontItalic;  // Improves because 'Font' is managed by a 'PropertyStore'.

						pathLabel_FilePath.Text = Command.UndefinedFilePathText;
					}

					checkBox_IsFile.Checked = true;

					// Delete:
					button_Clear.Enabled = true;
				}
				else
				{
					// Command:
					textBox_SingleLineText.Visible = true;
					if (this.editFocusState == EditFocusState.EditIsInactive)
					{
						if (textBox_SingleLineText.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
							textBox_SingleLineText.ForeColor = SystemColors.GrayText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
						                                             //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
						if (textBox_SingleLineText.Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
							textBox_SingleLineText.Font = DrawingEx.DefaultFontItalic;  // Improves because 'Font' is managed by a 'PropertyStore'.

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

					// Delete:                                                             // A description-only command shall also be clearable.
					button_Clear.Enabled = ((this.command == null) ? (false) : (this.command.HasDescription));
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Non-Public Methods > Handle Command
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Handle Command
		//------------------------------------------------------------------------------------------

		private void ConfirmDefaultRadix(Domain.Radix radix)
		{
			this.command.DefaultRadix = radix;
			this.isValidated = true;

			ConfirmCommand();
		}

		private void ConfirmSingleLineText(string text)
		{
			this.command.SingleLineText = text;
			this.isValidated = true;

			ConfirmCommand();
		}

		private void ConfirmMultiLineCommand(Command command)
		{
			this.command = command;
			this.isValidated = true;

			ConfirmCommand();
		}

		private void ConfirmDescription(string description)
		{
			this.command.Description = description;
		////this.isValidated must not be set as this flag solely applies to the radix/text-pair (i.e. not the description).

			ConfirmCommand();
		}

		private void ConfirmCommand()
		{
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
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowMultiLineBox(Control requestingControl)
		{
			// Indicate multi-line text:
			this.isSettingControls.Enter();
			try
			{
				textBox_SingleLineText.Text      = Command.MultiLineTextText; // No need to improve performance on
				textBox_SingleLineText.ForeColor = SystemColors.ControlText;  // accessing 'SystemColors' and 'SystemFonts'
				textBox_SingleLineText.Font      = SystemFonts.DefaultFont;   // as Show() is a slow operation anyway.
			}
			finally
			{
				this.isSettingControls.Leave();
			}

			// Calculate startup location:
			var requestingArea = requestingControl.RectangleToScreen(requestingControl.DisplayRectangle);
			var requestedLocation = new Point();
			requestedLocation.X = (requestingArea.X + requestingArea.Width);
			requestedLocation.Y = (requestingArea.Y + requestingArea.Height);

			// Show multi-line box:
			var f = new MultiLineBox(this.command, this.command.DefaultRadix, this.parseModeForText);
			f.Location = ScreenEx.CalculateLocationWithinWorkingArea(requestedLocation, f.Size);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
				ConfirmMultiLineCommand(f.CommandResult); // Command has been validated by multi-line box.
			else
				SetControls(); // Nothing has changed, just potentially restore multi-line text indication.

			textBox_Description.Select();
		}

		#endregion

		#region Non-Public Methods > File
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > File
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenFileDialog()
		{
			var ofd = new OpenFileDialog();
			ofd.Title = "Set File";

			string initialExtension;
			switch (this.terminalType)
			{
				case Domain.TerminalType.Binary:
				{
					initialExtension = ApplicationSettings.RoamingUserSettings.Extensions.BinarySendFiles;

					ofd.Filter      = ExtensionHelper.BinarySendFilesFilter;
					ofd.FilterIndex = ExtensionHelper.BinarySendFilesFilterHelper(initialExtension);
					break;
				}

				case Domain.TerminalType.Text:
				default:
				{
					initialExtension = ApplicationSettings.RoamingUserSettings.Extensions.TextSendFiles;

					ofd.Filter      = ExtensionHelper.TextFilesFilter;
					ofd.FilterIndex = ExtensionHelper.TextFilesFilterHelper(initialExtension);
					break;
				}
			}

			ofd.DefaultExt = PathEx.DenormalizeExtension(initialExtension);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.SendFiles;

			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				switch (this.terminalType)
				{
					case Domain.TerminalType.Binary:
					{
						ApplicationSettings.RoamingUserSettings.Extensions.BinarySendFiles = Path.GetExtension(ofd.FileName);
						break;
					}

					case Domain.TerminalType.Text:
					default:
					{
						ApplicationSettings.RoamingUserSettings.Extensions.TextSendFiles = Path.GetExtension(ofd.FileName);
						break;
					}
				}

				ApplicationSettings.LocalUserSettings.Paths.SendFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();
				ApplicationSettings.SaveRoamingUserSettings();

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

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnCommandChanged(EventArgs e)
		{
			EventHelper.RaiseSync(CommandChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
