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
// YAT Version 2.4.1
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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of state changes and validation related to user input:
////#define DEBUG_USER_INPUT

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY;
using MKY.Collections;
using MKY.Collections.Specialized;
using MKY.Drawing;
using MKY.Windows.Forms;

using YAT.Model.Settings;
using YAT.Model.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary>
	/// Provides command edit and send. Control keeps track of the edit state to properly
	/// react on all possible edit states.
	/// </summary>
	/// <remarks>
	/// On focus enter, edit state is always reset.
	/// On focus leave, edit state is kept depending on how focus is leaving.
	/// </remarks>
	/// <remarks>
	/// Note that similar code exists in <see cref="SendFile"/> and <see cref="PredefinedCommandSettingsSet"/>.
	/// The diff among these three implementations shall be kept as small as possible.
	///
	/// For a future refactoring, consider to separate the common code into a common view-model.
	/// </remarks>
	[DefaultEvent("SendCommandRequest")]
	public partial class SendText : UserControl, IOnFormDeactivateWorkaround
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary></summary>
		protected enum TextFocusState
		{
			/// <summary></summary>
			EditIsInactive,

			/// <summary></summary>
			EditHasFocus,

			/// <summary></summary>
			IsLeavingEdit,

			/// <summary></summary>
			IsLeavingParent
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypeDefault;
		private const Domain.Parser.Mode ParseModeDefault = Domain.Parser.Mode.Default;

		/// <summary></summary>
		public const bool SendImmediatelyDefault = false;

		private const bool TerminalIsReadyToSendForSomeTimeDefault = false;

		/// <remarks>
		/// The designer requires that this is a constant.
		/// Set same value as splitContainer.SplitterDistance is designed.
		/// </remarks>
		private const int SendSplitterDistanceDefault = 356;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		/// <summary>
		/// Flag to work around the issue described further below.
		/// </summary>
		private bool userInputIsInStandby; // = false;

		private Command command = new Command();
		private RecentItemCollection<Command> recent;

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool useExplicitDefaultRadix = Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault;
		private Domain.Parser.Mode parseMode = ParseModeDefault;

		private bool sendImmediately = SendImmediatelyDefault;
		private bool terminalIsReadyToSendForSomeTime = TerminalIsReadyToSendForSomeTimeDefault;

		private int sendSplitterDistance = SendSplitterDistanceDefault;

		private TextFocusState textFocusState; // = TextFocusState.EditIsInactive;
		private bool isValidated; // = false;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler CommandChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the TextFocused property is changed.")]
		public event EventHandler TextFocusedChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler<SendTextOptionEventArgs> SendCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SendText()
		{
			InitializeComponent();

			InitializeControls();
		////Set...Controls() is initially called in the 'Paint' event handler.
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
				// Similar code exists in PredefinedCommandSettingsSet.Command.set{}.
				// Changes here may have to be applied there too.

				if (this.command != value)
				{
					DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
					{
						if (value != null)
						{
							this.command = value;
							this.isValidated = value.IsValidText(this.parseMode);
						}
						else
						{
							this.command = new Command();
							this.isValidated = false;
						}

						SetCommandControls();
						OnCommandChanged(EventArgs.Empty);
					}
					DebugUserInputLeave();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> Recent
		{
			set
			{
				if (!IEnumerableEx.ItemsEqual(this.recent, value))
				{
					DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
					{
						this.recent = new RecentItemCollection<Command>(value); // Clone to ensure decoupling.
						SetRecentControls(); // Recent must immediately be updated, otherwise order will be wrong on arrow-up/down.
					}
					DebugUserInputLeave();
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
					SetSendControls();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool UseExplicitDefaultRadix
		{
			set
			{
				// Attention:
				// Similar code exists in PredefinedCommandSettingsSet.UseExplicitDefaultRadix.set{}.
				// Changes here may have to be applied there too.

				if (this.useExplicitDefaultRadix != value)
				{
					this.useExplicitDefaultRadix = value;

					if (value) // Explicit => Simply refresh the command (the radix hasn't changed (yet)).
						ConfirmCommand();

					SetExplicitDefaultRadixControls();

					if (!value) // Implicit => Reset default radix.
						ValidateAndConfirmDefaultRadix(Command.DefaultRadixDefault); // \remind (2019-09-01 / MKY / bug #469)
				}                                                                    // Better set 'isValidated' to 'false'
			}                                                                        // and validate later. But that requires
		}                                                                            // some work and re-testing => future.

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.Parser.Mode ParseMode
		{
			set
			{
				// Attention:
				// Similar code exists in PredefinedCommandSettingsSet.ParseModeForText.set{}.
				// Changes here may have to be applied there too.

				if (this.parseMode != value)
				{
					this.parseMode = value;
					SetSendControls();

					// \remind (2019-09-01 / MKY / bug #469)
					// Re-validation would be needed because change could enable or disable escapes.
					// But neither ValidateChildren() nor setting 'isValidated' to 'false' and
					// validate later is sufficient. Fix requires some work and re-testing => future.
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool SendImmediately
		{
			set
			{
				if (this.sendImmediately != value)
				{
					this.sendImmediately = value;
					SetSendControls();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSendForSomeTime
		{
			set
			{
				if (this.terminalIsReadyToSendForSomeTime != value)
				{
					this.terminalIsReadyToSendForSomeTime = value;
					SetSendControls();
				}
			}
		}

		/// <summary></summary>
		[DefaultValue(SendSplitterDistanceDefault)]
		public virtual int SendSplitterDistance
		{
			get { return (this.sendSplitterDistance); }
			set
			{
				// Do not check if (this.splitterDistance != value) because the distance (position)
				// will be limited to the control's width, and that may change AFTER the distance
				// has been set.

				this.sendSplitterDistance = value;

				// No need to call SetControls(); as only the splitter will be moved, and that will
				// not be accessed anywhere else.

				int limitedDistance;
				if (SplitContainerHelper.TryLimitSplitterDistance(splitContainer_Send, this.sendSplitterDistance, out limitedDistance))
				{
					if (splitContainer_Send.SplitterDistance != limitedDistance)
						splitContainer_Send.SplitterDistance = limitedDistance;
				}
			#if (DEBUG)
				else
				{
					Debugger.Break(); // See debug output for issue and potential root cause.
				}
			#endif
			}
		}

		/// <remarks>
		/// Function instead of property to emphasize purpose and prevent naming conflict among enum and property.
		/// </remarks>
		private void SetTextFocusState(TextFocusState value)
		{
			if (this.textFocusState != value)
			{
				this.textFocusState = value;
				OnTextFocusedChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		public virtual bool TextFocused
		{
			get { return (this.textFocusState != TextFocusState.EditIsInactive); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Required to be called "from the outside" because...
		/// ...if called in the constructor of the control, SetControls() has not yet been called.
		/// ...if called in the 'Paint' handler of the control, the last terminal in the application
		///    gets selected.    (due to the fact that an application  ^ ^ only has one focus)
		/// ...if called in the 'Enter' handler of the control, the (front) input gets selected even
		///    when focus enters "from the back".
		/// </remarks>
		public virtual void StandbyInUserInput()
		{
			if (SendText_Paint_IsFirst)
				this.userInputIsInStandby = true;

			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				comboBox_SingleLineText.Select();
				comboBox_SingleLineText.SelectionLength = 0;
				comboBox_SingleLineText.SelectionStart = comboBox_SingleLineText.Text.Length;
			}
			DebugUserInputLeave();
		}

		/// <remarks>
		/// Required to be called "from the outside" because...
		/// ...if called in the constructor of the control, SetControls() has not yet been called.
		/// ...if called in the 'Paint' handler of the control, the last terminal in the application
		///    gets selected.    (due to the fact that an application  ^ ^ only has one focus)
		/// ...if called in the 'Enter' handler of the control, the (front) input gets selected even
		///    when focus enters "from the back".
		/// </remarks>
		public virtual void PrepareUserInput()
		{
			if (SendText_Paint_IsFirst)
				this.userInputIsInStandby = false;

			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				comboBox_SingleLineText.Select();

				// No need to set the cursor to the end by "SelectionStart = Text.Length" as the
				// combo box is a ComboBoxEx that remembers cursor location and text selection.
			}
			DebugUserInputLeave();
		}

		/// <summary></summary>
		public virtual void ValidateInput()
		{
			ValidateChildren(); // Simplest way to invoke comboBox_SingleLineText_Validating().
		}

		/// <remarks>Somewhat ugly workaround to handle key events...</remarks>
		public virtual void NotifyKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
				SetSendControls();
		}

		/// <remarks>Somewhat ugly workaround to handle key events...</remarks>
		public virtual void NotifyKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
				SetSendControls();
		}

		/// <remarks>See remarks in <see cref="ComboBoxEx"/>.</remarks>
		public virtual void OnFormDeactivateWorkaround()
		{
			comboBox_SingleLineText.OnFormDeactivateWorkaround();
		}

		#endregion

		#region Control Special Keys
		//==========================================================================================
		// Control Special Keys
		//==========================================================================================

		/// <remarks>
		/// In case of pressing a modifier key (e.g. [Shift]), this method is invoked twice! Both
		/// invocations will state msg=0x100 (WM_KEYDOWN)! See:
		/// https://msdn.microsoft.com/en-us/library/system.windows.forms.control.processcmdkey.aspx:
		/// The ProcessCmdKey method first determines whether the control has a ContextMenu, and if
		/// so, enables the ContextMenu to process the command key. If the command key is not a menu
		/// shortcut and the control has a parent, the key is passed to the parent's ProcessCmdKey
		/// method. The net effect is that command keys are "bubbled" up the control hierarchy. In
		/// addition to the key the user pressed, the key data also indicates which, if any, modifier
		/// keys were pressed at the same time as the key. Modifier keys include the SHIFT, CTRL, and
		/// ALT keys.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (TextFocused)
			{
				if ((keyData & Keys.KeyCode) == Keys.Enter)
				{
					if (button_Send.Enabled)
					{
						RequestSendCommand();
						return (true);
					}
				}
			}

			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Flag only used by the following event handler.
		/// </summary>
		private bool SendText_Paint_IsFirst { get; set; } = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void SendText_Paint(object sender, PaintEventArgs e)
		{
			if (SendText_Paint_IsFirst) {
				SendText_Paint_IsFirst = false;

				DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
				{
					SetExplicitDefaultRadixControls(); // Attention, this method typically collapses the explicit default radix panel, and layouting apparently resets text selection in the combo box!
					SetRecentControls();
					SetCommandControls();

					if (this.userInputIsInStandby) // Attention, see comments below!
					{
						comboBox_SingleLineText.SelectionLength = 0;
						comboBox_SingleLineText.SelectionStart = comboBox_SingleLineText.Text.Length;
					}
				}
				DebugUserInputLeave();

				// Without the workaround above, the "Send Text" content of all terminals is fully
				// selected, i.e. same as after calling "SelectAndPrepareUserInput()", even though
				// "StandbyInUserInput()" has been called before. Sequence:
				//  > Main_Shown (event handler in 'Forms.Main')
				//     > main.Start()
				//        > 1st terminal is created
				//           > 'Send' is initialized
				//              > 'set_Command' => Cursor @ 0 / Selection @ 0 / Selected index @ -1
				//              > 'set_Recent'  => Cursor @ 0 / Selection @ 0 / Selected index @ 0
				//        > 1st Terminal_Activated (event handler in 'Forms.Terminal')
				//           > "Send.SelectAndPrepareUserInput()"
				//              > Cursor @ 0 / Selection @ <ALL> / Selected index @ 0
				//        > 2nd terminal is created
				//           > 'Send' is initialized
				//              > 'set_Command' => Cursor @ 0 / Selection @ 0 / Selected index @ -1
				//              > 'set_Recent'  => Cursor @ 0 / Selection @ 0 / Selected index @ 0
				//        > 1st Terminal_Deactivate (event handler in 'Forms.Terminal')
				//           > "Send.StandbyInUserInput()"
				//              > Cursor @ <END> / Selection @ 0 / Selected index @ 0
				//        > 2nd Terminal_Activated (event handler in 'Forms.Terminal')
				//           > "Send.SelectAndPrepareUserInput()"
				//              > Cursor @ 0 / Selection @ <ALL> / Selected index @ 0
				//
				//     All fine so far, but then something happens, and when Start() returns,
				//     the state is => Cursor @ 0 / Selection @ <ALL> / Selected index @ 0 !?!
				//
				//     Suspecting the issue is caused by layouting, as there is a similar issue with
				//     SetExplicitDefaultRadixControls() above. That method typically collapses the
				//     explicit default radix panel, and layouting apparently resets text selection
				//     in the combo box!
			}
		}

		/// <remarks>
		/// Do not modify <see cref="isValidated"/>. Command may already have been validated.
		/// </remarks>
		private void SendText_Enter(object sender, EventArgs e)
		{
			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				SetTextFocusState(TextFocusState.EditIsInactive);
			}
			DebugUserInputLeave();
		}

		/// <remarks>
		/// Event sequence when focus is leaving control, e.g. other MDI child activated.
		/// 1. ComboBox.Leave()
		/// 2. UserControl.Leave()
		/// 3. ComboBox.Validating()
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void SendText_Leave(object sender, EventArgs e)
		{
			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				if (this.isValidated)
					SetTextFocusState(TextFocusState.EditIsInactive);
				else
					SetTextFocusState(TextFocusState.IsLeavingParent);

				SetSendControls(); // Required to restore "Send Text [F3]"
			}                      // after "Send Text w/o EOL [Ctrl+F3]".
			DebugUserInputLeave();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

	////private void comboBox_ExplicitDefaultRadix_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since          "          _Validating() below gets called anyway.

		private void comboBox_ExplicitDefaultRadix_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (!comboBox_ExplicitDefaultRadix.Visible) // ValidateInput() calls ValidateChildren() which calls this method.
				return;                                 // But a potential validation error message makes no sense if invisible!

			// Attention:
			// Similar code exists in PredefinedCommandSettingsSet.comboBox_ExplicitDefaultRadix_Validating().
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
				SetCommandControls();
			}
		}

		private bool ValidateAndConfirmDefaultRadix(Domain.Radix defaultRadix)
		{
			// Attention:
			// Similar code exists in PredefinedCommandSettingsSet.ValidateAndConfirmRadix().
			// Changes here may have to be applied there too.

			if (this.command.IsSingleLineText)
			{
				string s = this.command.SingleLineText;
				if (Utilities.ValidationHelper.ValidateRadix(this, "default radix", s, this.parseMode, defaultRadix))
				{
					this.command.DefaultRadix = defaultRadix;
				////this.isValidated is intentionally not set, as the validation above only verifies the changed radix but not the text.
				////ConfirmCommand() is intentionally not called, as that may confirm the command with not yet updated text on ValidateChildren().
					return (true);
				}
			}
			else if (this.command.IsMultiLineText)
			{
				bool isValid = true;

				foreach (string line in this.command.MultiLineText)
				{
					if (!Utilities.ValidationHelper.ValidateRadix(this, "default radix", line, this.parseMode, defaultRadix))
						isValid = false;
				}

				if (isValid)
				{
					this.command.DefaultRadix = defaultRadix;
				////this.isValidated is intentionally not set, as the validation above only verifies the changed radix but not the text.
				////ConfirmCommand() is intentionally not called, as that may confirm the command with not yet updated text on ValidateChildren().
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

		private void comboBox_SingleLineText_Enter(object sender, EventArgs e)
		{
			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				// Attention:
				// Similar code exists in PredefinedCommandSettingsSet.textBox_SingleLineText_Enter().
				// Changes here may have to be applied there too.

				// Clear "<Enter a command...>" if needed.
				if ((this.textFocusState == TextFocusState.EditIsInactive) && !this.command.IsText)
				{
					this.isSettingControls.Enter();
					try
					{
						comboBox_SingleLineText.Text = "";

						if (comboBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
							comboBox_SingleLineText.ForeColor = SystemColors.ControlText;  // Improves because "ForeColor" is managed by a "PropertyStore".
						                                                //// Time consuming operation! See "FontEx.DefaultFontItalic" for background!
						if (comboBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
							comboBox_SingleLineText.Font = SystemFonts.DefaultFont;  // Improves because "Font" is managed by a "PropertyStore".
					}
					finally
					{
						this.isSettingControls.Leave();
					}
				}

				SetTextFocusState(TextFocusState.EditHasFocus);

				// No need to set this.isValidated = false yet. The "TextChanged" event will do so.
			}
			DebugUserInputLeave();
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
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void comboBox_SingleLineText_Leave(object sender, EventArgs e)
		{
			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				// Attention:
				// Similar code exists in PredefinedCommandSettingsSet.textBox_SingleLineText_Leave().
				// Changes here may have to be applied there too.

				if (this.isValidated)
					SetTextFocusState(TextFocusState.EditIsInactive);
				else
					SetTextFocusState(TextFocusState.IsLeavingEdit);
			}
			DebugUserInputLeave();
		}

		private void comboBox_SingleLineText_KeyPress(object sender, KeyPressEventArgs e)
		{
			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				if (this.sendImmediately)
				{
					this.isValidated = true; // Implicitly in any case.

					ConfirmPartialText(Domain.Terminal.ConvertToSendableText(e.KeyChar));
					OnSendCommandRequest(new SendTextOptionEventArgs(SendTextOption.Normal));
				}
			}
			DebugUserInputLeave();
		}

		private void comboBox_SingleLineText_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				// Attention:
				// Similar code exists in PredefinedCommandSettingsSet.textBox_SingleLineText_TextChanged().
				// Changes here may have to be applied there too.

				if (this.sendImmediately)
					comboBox_SingleLineText.Text = ""; // Instantly reset the text.
				else
					this.isValidated = false; // Reset the validation flag.

				SetSendControls();
			}
			DebugUserInputLeave();
		}

		/// <remarks>
		/// This _SelectedIndexChanged() handler is useful even though
		///      _ExplicitDefaultRadix_Validating() below gets called anyway.
		/// because <see cref="isValidated"/> gets set here.
		/// </remarks>
		private void comboBox_SingleLineText_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				this.isValidated = true; // Commands in history have already been validated.

				if (comboBox_SingleLineText.SelectedItem != null)
				{
					var ri = (comboBox_SingleLineText.SelectedItem as RecentItem<Command>);
					if (ri != null)
					{
						this.command = new Command(ri.Item); // Clone to ensure decoupling.

						ConfirmCommand();
					}
				}
			}
			DebugUserInputLeave();
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
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void comboBox_SingleLineText_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				// Attention:
				// Similar code exists in PredefinedCommandSettingsSet.textBox_SingleLineText_Validating().
				// Changes here may have to be applied there too.

				if (!this.isValidated)
				{
					// Postpone validation if focus is leaving the parent!
					// Validation will again be done after re-entering edit.
					if (this.textFocusState != TextFocusState.IsLeavingParent)
					{
						// Easter egg ;-)
						if (SendTextSettings.IsEasterEggCommand(comboBox_SingleLineText.Text))
						{
							this.isValidated = true;

							if (this.textFocusState == TextFocusState.IsLeavingEdit)
								SetTextFocusState(TextFocusState.EditIsInactive);

							ConfirmSingleLineText(comboBox_SingleLineText.Text);

							DebugUserInputLeave();
							return;
						}

						// Single line => Validate!
						int invalidTextStart;
						int invalidTextLength;
						if (Utilities.ValidationHelper.ValidateText(this, "command text", comboBox_SingleLineText.Text, out invalidTextStart, out invalidTextLength, this.parseMode, this.command.DefaultRadix))
						{
							this.isValidated = true;

							if (this.textFocusState == TextFocusState.IsLeavingEdit)
								SetTextFocusState(TextFocusState.EditIsInactive);

							ConfirmSingleLineText(comboBox_SingleLineText.Text);

							DebugUserInputLeave();
							return;
						}

						SetTextFocusState(TextFocusState.EditHasFocus);
						comboBox_SingleLineText.Select(invalidTextStart, invalidTextLength);
						e.Cancel = true;
					}
					else // EditFocusState.IsLeavingParent
					{
						SetTextFocusState(TextFocusState.EditIsInactive);
					}
				}
			}
			DebugUserInputLeave();
		}

		private void button_SetMultiLineText_Click(object sender, EventArgs e)
		{
			ShowMultiLineBox(button_SetMultiLineText);
		}

		private void button_Send_Click(object sender, EventArgs e)
		{
			RequestSendCommand();
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
				splitContainer_ExplicitDefaultRadix.Panel1Collapsed = !this.useExplicitDefaultRadix;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetRecentControls()
		{
			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				this.isSettingControls.Enter();
				try
				{
					if (this.recent != null)
						ComboBoxHelper.UpdateItemsKeepingCursorAndSelection(comboBox_SingleLineText, this.recent.ToArray());
					else
						ComboBoxHelper.ClearItemsKeepingCursorAndSelection(comboBox_SingleLineText);
				}
				finally
				{
					this.isSettingControls.Leave();
				}
			}
			DebugUserInputLeave();
		}

	////private int SetCommandControls_updateCounter; // Also exists in several other locations. Can temporarily be used for debugging the command state update (performance relevant).

		/// <remarks>
		/// Separate function as it is not needed to set this text on any change.
		/// </remarks>
		private void SetCommandControls()
		{
		////Debug.WriteLine("ST @ " + SetCommandControls_updateCounter++); // Also exists in several other locations. Can temporarily be used for debugging the command state update (performance relevant).

			DebugUserInputEnter(MethodBase.GetCurrentMethod().Name);
			{
				this.isSettingControls.Enter();
				try
				{
					if (this.useExplicitDefaultRadix)
						ComboBoxHelper.Select(comboBox_ExplicitDefaultRadix, (Domain.RadixEx)this.command.DefaultRadix, (Domain.RadixEx)this.command.DefaultRadix);
					else
						ComboBoxHelper.Deselect(comboBox_ExplicitDefaultRadix);

					if (this.textFocusState == TextFocusState.EditIsInactive)
					{
						if (this.command.IsText)
						{
							if (comboBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
								comboBox_SingleLineText.ForeColor = SystemColors.ControlText;  // Improves because "ForeColor" is managed by a "PropertyStore".
							                                                //// Time consuming operation! See "FontEx.DefaultFontItalic" for background!
							if (comboBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
								comboBox_SingleLineText.Font = SystemFonts.DefaultFont;  // Improves because "Font" is managed by a "PropertyStore".

							ComboBoxHelper.UpdateTextKeepingCursorAndSelection(comboBox_SingleLineText, this.command.SingleLineText);
						}
						else
						{
							if (comboBox_SingleLineText.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
								comboBox_SingleLineText.ForeColor = SystemColors.GrayText;  // Improves because "ForeColor" is managed by a "PropertyStore".
							                                           //// Time consuming operation! See "FontEx.DefaultFontItalic" for background!
							if (comboBox_SingleLineText.Font != FontEx.DefaultFontItalic) // Improve performance by only assigning if different.
								comboBox_SingleLineText.Font = FontEx.DefaultFontItalic;  // Improves because "Font" is managed by a "PropertyStore".

							comboBox_SingleLineText.Text = Command.EnterTextText;
						}
					}
					else
					{
						if (comboBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
							comboBox_SingleLineText.ForeColor = SystemColors.ControlText;  // Improves because "ForeColor" is managed by a "PropertyStore".
						                                                //// Time consuming operation! See "FontEx.DefaultFontItalic" for background!
						if (comboBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
							comboBox_SingleLineText.Font = SystemFonts.DefaultFont;  // Improves because "Font" is managed by a "PropertyStore".

						if (this.command.IsText && !this.command.IsPartialText)
						{
							ComboBoxHelper.UpdateTextKeepingCursorAndSelection(comboBox_SingleLineText, this.command.SingleLineText);
						}
						else
						{
							comboBox_SingleLineText.Text = "";
						}
					}

					SetSendControls();
				}
				finally
				{
					this.isSettingControls.Leave();
				}
			}
			DebugUserInputLeave();
		}

		private void SetSendControls()
		{
			bool isTextTerminal = (this.terminalType == Domain.TerminalType.Text);

			this.isSettingControls.Enter();
			try
			{
				// Prepare the button properties based on state and settings.
				//
				// Attention:
				// Similar code exists in...
				// ...YAT.View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
				// ...YAT.View.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
				// Changes here may have to be applied there too.

				var text = "Send Text [F3]";
				if (WithoutEolIsRequestedAndAllowed)
					text = "Send Text w/o EOL [Ctrl+F3]";

				bool enabled = this.terminalIsReadyToSendForSomeTime;
				if (this.sendImmediately)
				{
					if (isTextTerminal)
						text = "Send EOL [F3]";
					else
						enabled = false;
				}

				string commandText = "";
				if (!string.IsNullOrEmpty(comboBox_SingleLineText.Text))
					commandText = comboBox_SingleLineText.Text;

				string toolTipText = @"Send """ + commandText + @"""";
				if (this.sendImmediately)
				{
					if (isTextTerminal)
						toolTipText = "Send EOL";
					else
						toolTipText = "";
				}

				// Set the button properties:
				button_Send.Text = text;
				button_Send.Enabled = enabled;
				toolTip.SetToolTip(button_Send, toolTipText);
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

		private void ConfirmCommand()
		{
			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmSingleLineText(string text)
		{
			this.command.ClearDescription(); // An immediate command never has a description.
			this.command.SingleLineText = text;

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmPartialText(string text)
		{
			this.command.ClearDescription(); // An immediate command never has a description.
			this.command.PartialText = text;

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmPartialTextEolCommand()
		{
			this.command.ClearDescription(); // An immediate command never has a description.
			this.command.IsPartialTextEol = true;

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		#endregion

		#region Non-Public Methods > Multi-Line Text
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Multi-Line Text
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Almost duplicated code in <see cref="PredefinedCommandSettingsSet.ShowMultiLineBox"/>.
		/// </remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowMultiLineBox(Control requestingControl)
		{
			var previousText            = comboBox_SingleLineText.Text;
			var previousForeColor       = comboBox_SingleLineText.ForeColor;
			var previousFont            = comboBox_SingleLineText.Font;

			var previousSelectionStart  = comboBox_SingleLineText.PreviousSelectionStart;
			var previousSelectionLength = comboBox_SingleLineText.PreviousSelectionLength;

			// Indicate multi-line text:
			this.isSettingControls.Enter();
			try
			{
				comboBox_SingleLineText.Text      = Command.MultiLineTextText; // No need to improve performance on
				comboBox_SingleLineText.ForeColor = SystemColors.ControlText;  // accessing 'SystemColors' and 'SystemFonts'
				comboBox_SingleLineText.Font      = SystemFonts.DefaultFont;   // as Show() is a slow operation anyway.
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
			var f = new MultiLineBox(this.command, this.command.DefaultRadix, this.parseMode);
			f.Location = ScreenEx.CalculateLocationWithinWorkingArea(requestedLocation, f.Size);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				this.command = f.CommandResult;
				this.isValidated = true; // Command has been validated by multi-line box.

				Refresh(); // Ensure that control has been refreshed before continuing.
				ConfirmCommand();

				// Select the whole line for ComboBox' standard behavior when scrolling through item list:
				this.isSettingControls.Enter();
				try
				{
					comboBox_SingleLineText.Select();    // First select...
					comboBox_SingleLineText.SelectAll(); // ...then override default behavior.
				}
				finally
				{
					this.isSettingControls.Leave();
				}
			}
			else
			{
				// Restore former text:
				this.isSettingControls.Enter();
				try
				{
					// Restore former text:
					comboBox_SingleLineText.Text            = previousText;
					comboBox_SingleLineText.ForeColor       = previousForeColor;
					comboBox_SingleLineText.Font            = previousFont;

					// Also restore cursor location and text selection:
					if (previousSelectionStart != ControlEx.InvalidIndex)
					{
						comboBox_SingleLineText.Select();                                 // First select...
						comboBox_SingleLineText.SelectionStart  = previousSelectionStart; // ...then override default behavior.
						comboBox_SingleLineText.SelectionLength = previousSelectionLength;
					}
					else // Fallback:
					{
						comboBox_SingleLineText.Select();    // First select...
						comboBox_SingleLineText.SelectAll(); // ...then override default behavior.
					}
				}
				finally
				{
					this.isSettingControls.Leave();
				}
			}
		}

		#endregion

		#region Non-Public Methods > Request Send
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Request Send
		//------------------------------------------------------------------------------------------

		private void RequestSendCommand()
		{
			var option = SendTextOption.Normal;
			if (WithoutEolIsRequestedAndAllowed)
				option = SendTextOption.WithoutEol;

			if (this.sendImmediately)
			{
				ConfirmPartialTextEolCommand();
				OnSendCommandRequest(new SendTextOptionEventArgs(option));
			}
			else
			{
				if (this.isValidated)
				{
					ConfirmCommand(); // Required to invoke OnCommandChanged().
					OnSendCommandRequest(new SendTextOptionEventArgs(option));
				}
				else
				{
					if (ValidateChildren()) // comboBox_SingleLineText_Validating() will call ConfirmSingleLineText().
						OnSendCommandRequest(new SendTextOptionEventArgs(option));
				}
			}
		}

		private bool WithoutEolIsRequestedAndAllowed
		{
			get
			{
				if (ContainsFocus && !this.command.IsMultiLineText)
					return ((ModifierKeys & Keys.Control) != 0);
				else
					return (false);
			}
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

		/// <summary></summary>
		protected virtual void OnTextFocusedChanged(EventArgs e)
		{
			EventHelper.RaiseSync(TextFocusedChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendCommandRequest(SendTextOptionEventArgs e)
		{
			EventHelper.RaiseSync(SendCommandRequest, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is an FxCop false-positive, the called DebugUserInputState() cannot be static.")]
		[Conditional("DEBUG_USER_INPUT")]
		private void DebugUserInputEnter(string methodName)
		{
			Debug.WriteLine(methodName);
			Debug.Indent();

			DebugUserInputState();
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is an FxCop false-positive, the called DebugUserInputState() cannot be static.")]
		[Conditional("DEBUG_USER_INPUT")]
		private void DebugUserInputLeave()
		{
			DebugUserInputState();

			Debug.Unindent();
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_USER_INPUT")]
		private void DebugUserInputState()
		{
			Debug.Write    (@"Text   = """         + comboBox_SingleLineText.Text);
			Debug.Write    (@""" / Cursor @ "      + comboBox_SingleLineText.SelectionStart);
			Debug.Write    (" / Selection @ "      + comboBox_SingleLineText.SelectionLength);
			Debug.WriteLine(" / Selected index @ " + comboBox_SingleLineText.SelectedIndex);

			if (this.recent != null)
				Debug.WriteLine(@"Recent = """ + ArrayEx.ValuesToString(this.recent.ToArray()) + @"""");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
