﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

	// Enable debugging of state changes and validation related to the handled command:
////#define DEBUG_COMMAND

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
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.Drawing;
using MKY.Recent;
using MKY.Text;
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
	public partial class SendText : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum EditFocusState
		{
			EditIsInactive,
			EditHasFocus,
			IsLeavingEdit,
			IsLeavingParent
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypeDefault;
		private const Domain.Parser.Modes ParseModeDefault = Domain.Parser.Modes.Default;

		private const bool TerminalIsReadyToSendDefault = false;

		/// <remarks>
		/// The designer requires that this is a constant.
		/// Set same value as splitContainer.SplitterDistance is designed.
		/// </remarks>
		private const int SendSplitterDistanceDefault = 356;

		/// <summary></summary>
		public const bool SendImmediatelyDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Command command = new Command();
		private RecentItemCollection<Command> recent;

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool useExplicitDefaultRadix = Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault;
		private Domain.Parser.Modes parseMode = ParseModeDefault;

		private bool sendImmediately = SendImmediatelyDefault;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;

		private int sendSplitterDistance = SendSplitterDistanceDefault;

		private EditFocusState editFocusState = EditFocusState.EditIsInactive;
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
		[Description("Event raised when the EditFocusState property is changed.")]
		public event EventHandler EditFocusStateChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler SendCommandRequest;

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
				if (this.command != value)
				{
					DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

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

					SetCommandControls();
					OnCommandChanged(EventArgs.Empty);

					DebugCommandLeave();
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
				DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

				// Do not check if (this.recent != value) because the collection will always be the same!

				this.recent = value;
				SetRecentControls(); // Recent must immediately be updated, otherwise order will be wrong on arrow-up/down.

				DebugCommandLeave();
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			get
			{
				return (this.terminalType);
			}
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

					if (value) // Explicit => Refresh the command.
						ConfirmCommand();

					SetExplicitDefaultRadixControls();

					if (!value) // Implicit => Reset default radix.
						ValidateAndConfirmRadix(Command.DefaultRadixDefault);
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.Parser.Modes ParseMode
		{
			get
			{
				return (this.parseMode);
			}
			set
			{
				if (this.parseMode != value)
				{
					this.parseMode = value;
					SetSendControls();
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool SendImmediately
		{
			get
			{
				return (this.sendImmediately);
			}
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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSend
		{
			get
			{
				return (this.terminalIsReadyToSend);
			}
			set
			{
				if (this.terminalIsReadyToSend != value)
				{
					this.terminalIsReadyToSend = value;
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

				splitContainer_Send.SplitterDistance = Int32Ex.Limit((this.sendSplitterDistance - splitContainer_Send.Left), 0, (splitContainer_Send.Width - 1));
			}
		}

		/// <remarks>Function instead of property to emphasize purpose and prevent naming conflict among enum and property.</remarks>
		private void SetEditFocusState(EditFocusState state)
		{
			if (this.editFocusState != state)
			{
				this.editFocusState = state;
				OnEditFocusStateChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		public virtual bool EditIsActive
		{
			get { return (this.editFocusState != EditFocusState.EditIsInactive); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void SelectInput()
		{
			comboBox_SingleLineText.Select();
		}

		/// <summary></summary>
		public virtual void ValidateInput()
		{
			ValidateChildren(); // Simplest way to invoke comboBox_SingleLineText_Validating().
		}

		#endregion

		#region Control Special Keys
		//==========================================================================================
		// Control Special Keys
		//==========================================================================================

		/// <summary></summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (EditIsActive)
			{
				if (keyData == Keys.Enter)
				{
					if (button_Send.Enabled)
					{
						RequestSendCommand();
						SetCursorToEnd();
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
		private void SendText_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetExplicitDefaultRadixControls();
				SetRecentControls();
				SetCommandControls();
				SetCursorToEnd();
			}
		}

		/// <remarks>
		/// Do not modify <see cref="isValidated"/>. Command may already have been validated.
		/// </remarks>
		private void SendText_Enter(object sender, EventArgs e)
		{
			SetEditFocusState(EditFocusState.EditIsInactive);
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
			if (this.isValidated)
				SetEditFocusState(EditFocusState.EditIsInactive);
			else
				SetEditFocusState(EditFocusState.IsLeavingParent);
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
					SetCommandControls();
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
				////ConfirmCommand() is intentionally not called, as that may confirm the command with not yet updated text on ValidateChildren().
					return (true);
				}
			}
			else if (this.command.IsMultiLineText)
			{
				bool isValid = true;

				foreach (string s in this.command.MultiLineText)
				{
					if (!Utilities.ValidationHelper.ValidateRadix(this, "default radix", s, radix, this.parseMode))
						isValid = false;
				}

				if (isValid)
				{
					this.command.DefaultRadix = radix;
				////this.isValidated is intentionally not set, as the validation above only verifies the changed radix but not the text.
				////ConfirmCommand() is intentionally not called, as that may confirm the command with not yet updated text on ValidateChildren().
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

		private void comboBox_SingleLineText_Enter(object sender, EventArgs e)
		{
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

			// Clear "<Enter a command...>" if needed.
			if ((this.editFocusState == EditFocusState.EditIsInactive) && !this.command.IsText)
			{
				this.isSettingControls.Enter();
				comboBox_SingleLineText.Text      = "";
				comboBox_SingleLineText.ForeColor = SystemColors.ControlText;
				comboBox_SingleLineText.Font      = SystemFonts.DefaultFont;
				this.isSettingControls.Leave();
			}

			SetEditFocusState(EditFocusState.EditHasFocus);

			// No need to set this.isValidated = false yet. The 'TextChanged' event will do so.

			DebugCommandLeave();
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
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (this.isValidated)
				SetEditFocusState(EditFocusState.EditIsInactive);
			else
				SetEditFocusState(EditFocusState.IsLeavingEdit);

			DebugCommandLeave();
		}

		private void comboBox_SingleLineText_KeyPress(object sender, KeyPressEventArgs e)
		{
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (this.sendImmediately)
			{
				this.isValidated = true; // Implicitly in any case.

				byte asciiCode;
				string text;

				if (!char.IsControl(e.KeyChar))
					text = e.KeyChar.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for keys!
				else if ((CharEx.TryConvertToByte(e.KeyChar, out asciiCode)) && (Ascii.IsControl(asciiCode)))
					text = "<" + Ascii.ConvertToMnemonic(asciiCode) + ">";
				else // Applies to Unicode control characters U+0080..U+009F
					text = "\\h(" + ConvertEx.ToHexadecimalString(Encoding.Unicode.GetBytes(new char[] { e.KeyChar })) + ")";

				ConfirmPartialText(text);
				InvokeSendCommandRequest();
			}

			DebugCommandLeave();
		}

		private void comboBox_SingleLineText_TextChanged(object sender, EventArgs e)
		{
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (!this.isSettingControls)
			{
				if (this.sendImmediately)
					comboBox_SingleLineText.Text = ""; // Instantly reset the text.
				else
					this.isValidated = false; // Reset the validation flag.

				SetSendControls();
			}

			DebugCommandLeave();
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
			if (!this.isSettingControls)
			{
				DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

				if (!this.isValidated)
				{
					// Postpone validation if focus is leaving the parent!
					// Validation will again be done after re-entering edit.
					if (this.editFocusState != EditFocusState.IsLeavingParent)
					{
						// Easter egg ;-)
						if (SendTextSettings.IsEasterEggCommand(comboBox_SingleLineText.Text))
						{
							this.isValidated = true;

							if (this.editFocusState == EditFocusState.IsLeavingEdit)
								SetEditFocusState(EditFocusState.EditIsInactive);

							ConfirmSingleLineText(comboBox_SingleLineText.Text);

							DebugCommandLeave();
							return;
						}

						// Single line => Validate!
						int invalidTextStart;
						int invalidTextLength;
						if (Utilities.ValidationHelper.ValidateText(this, "text", comboBox_SingleLineText.Text, out invalidTextStart, out invalidTextLength, this.command.DefaultRadix, this.parseMode))
						{
							this.isValidated = true;

							if (this.editFocusState == EditFocusState.IsLeavingEdit)
								SetEditFocusState(EditFocusState.EditIsInactive);

							ConfirmSingleLineText(comboBox_SingleLineText.Text);

							DebugCommandLeave();
							return;
						}

						SetEditFocusState(EditFocusState.EditHasFocus);
						comboBox_SingleLineText.Select(invalidTextStart, invalidTextLength);
						e.Cancel = true;
					}
					else // EditFocusState.IsLeavingParent
					{
						SetEditFocusState(EditFocusState.EditIsInactive);
					}
				}

				DebugCommandLeave();
			}
		}

		private void comboBox_SingleLineText_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

				this.isValidated = true; // Commands in history have already been validated.

				if (comboBox_SingleLineText.SelectedItem != null)
				{
					var ri = (comboBox_SingleLineText.SelectedItem as RecentItem<Command>);
					if (ri != null)
					{
						this.command = ri.Item;

						ConfirmCommand();
					}
				}

				DebugCommandLeave();
			}
		}
		
		private void button_SetMultiLineText_Click(object sender, EventArgs e)
		{
			ShowMultiLineBox(button_SetMultiLineText);
		}

		private void button_Send_Click(object sender, EventArgs e)
		{
			RequestSendCommand();
			SetCursorToEnd();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > Controls
		//------------------------------------------------------------------------------------------
		// Private Methods > Controls
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

		private void SetRecentControls()
		{
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);
			this.isSettingControls.Enter();

			// Keep text field because Items.Clear() will reset this:
			string text         = comboBox_SingleLineText.Text;
			int selectionStart  = comboBox_SingleLineText.SelectionStart;
			int selectionLength = comboBox_SingleLineText.SelectionLength;

			comboBox_SingleLineText.Items.Clear();
			if ((this.recent != null) && (this.recent.Count > 0))
				comboBox_SingleLineText.Items.AddRange(this.recent.ToArray());

			// Immediately update the updated item list:
			comboBox_SingleLineText.Refresh();

			// Restore text field:
			comboBox_SingleLineText.Text            = text;
			comboBox_SingleLineText.SelectionStart  = selectionStart;
			comboBox_SingleLineText.SelectionLength = selectionLength;

			this.isSettingControls.Leave();
			DebugCommandLeave();
		}

		/// <remarks>
		/// Separate function as it is not needed to set this text on any change.
		/// </remarks>
		private void SetCommandControls()
		{
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);
			this.isSettingControls.Enter();

			if (this.useExplicitDefaultRadix)
				SelectionHelper.Select(comboBox_ExplicitDefaultRadix, (Domain.RadixEx)this.command.DefaultRadix, (Domain.RadixEx)this.command.DefaultRadix);
			else
				SelectionHelper.Deselect(comboBox_ExplicitDefaultRadix);

			if (this.editFocusState == EditFocusState.EditIsInactive)
			{
				if (this.command.IsText)
				{
					if (comboBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
						comboBox_SingleLineText.ForeColor = SystemColors.ControlText;

					if (comboBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
						comboBox_SingleLineText.Font = SystemFonts.DefaultFont;

					comboBox_SingleLineText.Text = this.command.SingleLineText;
				}
				else
				{
					if (comboBox_SingleLineText.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						comboBox_SingleLineText.ForeColor = SystemColors.GrayText;

					if (comboBox_SingleLineText.Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
						comboBox_SingleLineText.Font = DrawingEx.DefaultFontItalic;

					comboBox_SingleLineText.Text = Command.EnterTextText;
				}
			}
			else
			{
				if (comboBox_SingleLineText.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
					comboBox_SingleLineText.ForeColor = SystemColors.ControlText;

				if (comboBox_SingleLineText.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
					comboBox_SingleLineText.Font = SystemFonts.DefaultFont;

				if (this.command.IsText && !this.command.IsPartialText)
					comboBox_SingleLineText.Text = this.command.SingleLineText;
				else
					comboBox_SingleLineText.Text = "";
			}

			SetCursorToEnd();
			SetSendControls();

			this.isSettingControls.Leave();
			DebugCommandLeave();
		}

		private void SetCursorToEnd()
		{
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);
			this.isSettingControls.Enter();

			comboBox_SingleLineText.SelectionStart = comboBox_SingleLineText.Text.Length;

			this.isSettingControls.Leave();
			DebugCommandLeave();
		}

		private void SetSendControls()
		{
			this.isSettingControls.Enter();

			// Prepare the button properties based on state and settings.
			//
			// Attention:
			// Similar code exists in the following locations:
			//  > YAT.View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
			//  > YAT.View.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
			// Changes here may have to be applied there too.

			string text = "Send Text (F3)";
			bool enabled = this.terminalIsReadyToSend;
			if (this.sendImmediately)
			{
				switch (this.terminalType)
				{
					case Domain.TerminalType.Text: text = "Send EOL (F3)"; break;
					default: /* Binary or <New> */ enabled = false;        break;
				}
			}

			string commandText = "";
			if (!string.IsNullOrEmpty(comboBox_SingleLineText.Text))
				commandText = comboBox_SingleLineText.Text;

			string toolTipText = @"Send """ + commandText + @"""";
			if (this.sendImmediately)
			{
				switch (this.terminalType)
				{
					case Domain.TerminalType.Text: toolTipText = "Send EOL"; break;
					default: /* Binary or <New> */ toolTipText = "";         break;
				}
			}

			// Set the button properties:
			button_Send.Text = text;
			button_Send.Enabled = enabled;
			toolTip.SetToolTip(button_Send, toolTipText);

			this.isSettingControls.Leave();
		}

		#endregion

		#region Private Methods > Handle Command
		//------------------------------------------------------------------------------------------
		// Private Methods > Handle Command
		//------------------------------------------------------------------------------------------

		private void ConfirmCommand()
		{
			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmSingleLineText(string text)
		{
			this.command.SingleLineText = text;

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmPartialText(string text)
		{
			this.command.PartialText = text;

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmPartialTextEolCommand()
		{
			this.command.IsPartialTextEol = true;

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		#endregion

		#region Private Methods > Multi-Line Text
		//------------------------------------------------------------------------------------------
		// Private Methods > Multi-Line Text
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Almost duplicated code in <see cref="PredefinedCommandSettingsSet.ShowMultiLineBox"/>.
		/// </remarks>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowMultiLineBox(Control requestingControl)
		{
			// Indicate multi-line text:
			this.isSettingControls.Enter();
			comboBox_SingleLineText.Text      = Command.MultiLineTextText;
			comboBox_SingleLineText.ForeColor = SystemColors.ControlText;
			comboBox_SingleLineText.Font      = SystemFonts.DefaultFont;
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

				ConfirmCommand();
			}

			button_Send.Select();
		}

		#endregion

		#region Private Methods > Request Send
		//------------------------------------------------------------------------------------------
		// Private Methods > Request Send
		//------------------------------------------------------------------------------------------

		private void RequestSendCommand()
		{
			if (this.sendImmediately)
			{
				ConfirmPartialTextEolCommand();
				InvokeSendCommandRequest();
			}
			else
			{
				if (this.isValidated)
				{
					ConfirmCommand(); // Required to invoke OnCommandChanged().
					InvokeSendCommandRequest();
				}
				else
				{
					if (ValidateChildren()) // ConfirmSingleLineText() gets called here.
						InvokeSendCommandRequest();
				}
			}
		}

		private void InvokeSendCommandRequest()
		{
			OnSendCommandRequest(EventArgs.Empty);
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

		/// <summary></summary>
		protected virtual void OnEditFocusStateChanged(EventArgs e)
		{
			EventHelper.FireSync(EditFocusStateChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendCommandRequest, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void DebugCommandEnter(string methodName)
		{
			Debug.WriteLine(methodName);
			Debug.Indent();

			DebugCommandState();
		}

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void DebugCommandLeave()
		{
			DebugCommandState();

			Debug.Unindent();
		}

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void DebugCommandState()
		{
			Debug.Write    ("Text    = "      + comboBox_SingleLineText.Text);
			Debug.Write    (" with cursor @ " + comboBox_SingleLineText.SelectionStart);
			Debug.WriteLine(" and sel.idx @ " + comboBox_SingleLineText.SelectedIndex);

			if (this.recent != null)
				Debug.WriteLine("Recent = " + ArrayEx.ElementsToString(this.recent.ToArray()));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================