//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY;
using MKY.Recent;
using MKY.Windows.Forms;

using YAT.Gui.Utilities;
using YAT.Model.Settings;
using YAT.Model.Types;

#endregion

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

		// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
		// warnings for each undocumented member below. Documenting each member makes little sense
		// since they pretty much tell their purpose and documentation tags between the members
		// makes the code less readable.
		#pragma warning disable 1591

		/// <summary></summary>
		protected enum EditFocusState
		{
			EditIsInactive,
			EditHasFocus,
			IsLeavingEdit,
			IsLeavingParent,
		}

		#pragma warning restore 1591

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypefault;
		private const bool TerminalIsReadyToSendDefault = false;
		private const int SplitterDistanceDefault = 356; // Designer requires that this is a constant.
		                                                 // Set same value as splitContainer.SplitterDistance is designed.
		private const Domain.Parser.Modes ParseModeDefault = Domain.Parser.Modes.Default;

		/// <summary></summary>
		public const bool SendImmediatelyDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Command command = new Command();
		private RecentItemCollection<Command> recents;
		private Domain.Parser.Modes parseMode = ParseModeDefault;
		private bool sendImmediately = SendImmediatelyDefault;

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;
		private int splitterDistance = SplitterDistanceDefault;

		private EditFocusState editFocusState = EditFocusState.EditIsInactive;
		private bool isValidated;

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
		[SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Justification = "The initialization of 'terminalIsReadyToSend' is not unnecesary, it is based on a constant that contains a default value!")]
		public SendCommand()
		{
			InitializeComponent();

			// SetControls() is initially called in the 'Paint' event handler.
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
					SetCursorToEnd();
					OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> Recents
		{
			set
			{
				// Do not check if (this.recents != value) because the collection will always be the same!

				this.recents = value;
				SetRecents(); // Recents must immediately be updated, otherwise order will be wrong on arrow-up/down.
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
				if (this.parseMode != value)
				{
					this.parseMode = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Command")]
		[Description("The send mode related to the command.")]
		[DefaultValue(SendImmediatelyDefault)]
		public virtual bool SendImmediately
		{
			get { return (this.sendImmediately); }
			set
			{
				if (this.sendImmediately != value)
				{
					this.sendImmediately = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSend
		{
			set
			{
				if (this.terminalIsReadyToSend != value)
				{
					this.terminalIsReadyToSend = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[DefaultValue(SplitterDistanceDefault)]
		public virtual int SplitterDistance
		{
			get { return (this.splitterDistance); }
			set
			{
				if (this.splitterDistance != value)
				{
					this.splitterDistance = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		public virtual bool EditIsActive
		{
			get { return (this.editFocusState != EditFocusState.EditIsInactive); }
		}

		/// <summary></summary>
		protected virtual void SetEditFocusState(EditFocusState editFocusSet)
		{
			if (this.editFocusState != editFocusSet)
			{
				this.editFocusState = editFocusSet;
				OnEditFocusStateChanged(EventArgs.Empty);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void SelectInput()
		{
			comboBox_Command.Select();
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
					if (button_SendCommand.Enabled)
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
		private void SendCommand_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
				SetCursorToEnd();
			}
		}

		/// <remarks>
		/// Do not modify <see cref="isValidated"/>. Command may already have been validated.
		/// </remarks>
		private void SendCommand_Enter(object sender, EventArgs e)
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
		private void SendCommand_Leave(object sender, EventArgs e)
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

		private void comboBox_Command_Enter(object sender, EventArgs e)
		{
			// Clear "<Enter a command...>" if needed.
			if ((this.editFocusState == EditFocusState.EditIsInactive) && !this.command.IsText)
			{
				this.isSettingControls.Enter();
				comboBox_Command.Text      = "";
				comboBox_Command.ForeColor = SystemColors.ControlText;
				comboBox_Command.Font      = SystemFonts.DefaultFont;
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
		/// Event sequence when focus is leaving control, e.g. other MDI child activated.
		/// 1. ComboBox.Leave()
		/// 2. UserControl.Leave()
		/// 3. ComboBox.Validating()
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void comboBox_Command_Leave(object sender, EventArgs e)
		{
			if (this.isValidated)
				SetEditFocusState(EditFocusState.EditIsInactive);
			else
				SetEditFocusState(EditFocusState.IsLeavingEdit);
		}

		private void comboBox_Command_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (this.sendImmediately)
			{
				this.isValidated = true;
				CreatePartialCommand(e.KeyChar.ToString(CultureInfo.InvariantCulture));
				RequestSendPartialCommand();
			}
		}

		private void comboBox_Command_TextChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				if (this.sendImmediately)
					comboBox_Command.Text = ""; // Instantly reset the text.
				else
					this.isValidated = false; // Reset the validation flag.

				SetButtonToolTip();
			}
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
		private void comboBox_Command_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				if (!this.isValidated)
				{
					// Postpone validation if focus is leaving the parent!
					// Validation will again be done after re-entering edit.
					if (this.editFocusState != EditFocusState.IsLeavingParent)
					{
						// Easter egg ;-)
						if (SendCommandSettings.IsEasterEggCommand(comboBox_Command.Text))
						{
							this.isValidated = true;

							if (this.editFocusState == EditFocusState.IsLeavingEdit)
								SetEditFocusState(EditFocusState.EditIsInactive);

							CreateSingleLineCommand(comboBox_Command.Text);
							return;
						}

						// Single line => Validate!
						int invalidTextStart;
						int invalidTextLength;
						if (Validation.ValidateSequence(this, "Command", comboBox_Command.Text, this.parseMode, out invalidTextStart, out invalidTextLength))
						{
							this.isValidated = true;

							if (this.editFocusState == EditFocusState.IsLeavingEdit)
								SetEditFocusState(EditFocusState.EditIsInactive);

							CreateSingleLineCommand(comboBox_Command.Text);
							return;
						}

						SetEditFocusState(EditFocusState.EditHasFocus);
						comboBox_Command.Select(invalidTextStart, invalidTextLength);
						e.Cancel = true;
					}
					else // EditFocusState.IsLeavingParent
					{
						SetEditFocusState(EditFocusState.EditIsInactive);
					}
				}
			}
		}

		private void comboBox_Command_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.isValidated = true; // Commands in history have already been validated.

				if (comboBox_Command.SelectedItem != null)
				{
					RecentItem<Command> ri = (comboBox_Command.SelectedItem as RecentItem<Command>);
					if (ri != null)
					{
						this.command = ri.Item;

						OnCommandChanged(EventArgs.Empty);
						SetControls();
					}
				}
			}
		}
		
		private void button_MultiLineCommand_Click(object sender, EventArgs e)
		{
			ShowMultiLineCommandBox(button_MultiLineCommand);
		}

		private void button_SendCommand_Click(object sender, EventArgs e)
		{
			RequestSendCommand();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > Set Controls
		//------------------------------------------------------------------------------------------
		// Private Methods > Set Controls
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			this.isSettingControls.Enter();

			splitContainer.SplitterDistance = Int32Ex.LimitToBounds((this.splitterDistance - splitContainer.Left), 0, (splitContainer.Width - 1));

			if (this.editFocusState == EditFocusState.EditIsInactive)
			{
				if (this.command.IsText)
				{
					comboBox_Command.Text      = this.command.SingleLineText;
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
			else
			{
				if (this.command.IsText && !this.command.IsPartialText)
					comboBox_Command.Text = this.command.SingleLineText;
				else
					comboBox_Command.Text = "";

				comboBox_Command.ForeColor = SystemColors.ControlText;
				comboBox_Command.Font      = SystemFonts.DefaultFont;
			}

			// Prepare the button properties based on state and settings.
			//
			// Attention: Similar code exists in the following locations:
			//  > YAT.Gui.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
			//  > YAT.Gui.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
			// Changes here may have to be applied there.

			string text = "Send Command (F3)";
			bool enabled = this.terminalIsReadyToSend;
			if (this.sendImmediately)
			{
				switch (this.terminalType)
				{
					case Domain.TerminalType.Text: text = "Send EOL (F3)"; break;
					default: /* Binary or <New> */ enabled = false;        break;
				}
			}

			button_SendCommand.Text = text;
			button_SendCommand.Enabled = enabled;

			SetButtonToolTip();

			this.isSettingControls.Leave();
		}

		private void SetButtonToolTip()
		{
			this.isSettingControls.Enter();

			string commandText = "";
			if (!string.IsNullOrEmpty(comboBox_Command.Text))
				commandText = comboBox_Command.Text;

			string caption = @"Send """ + commandText + @"""";
			if (this.sendImmediately)
			{
				switch (this.terminalType)
				{
					case Domain.TerminalType.Text: caption = "Send EOL"; break;
					default: /* Binary or <New> */ caption = "";         break;
				}
			}
			toolTip.SetToolTip(button_SendCommand, caption);

			this.isSettingControls.Leave();
		}

		private void SetRecents()
		{
			this.isSettingControls.Enter();

			// Keep cursor position and selection because Items.Clear() will reset this:
			int selectionStart  = comboBox_Command.SelectionStart;
			int selectionLength = comboBox_Command.SelectionLength;

			comboBox_Command.Items.Clear();
			if (this.recents != null)
				comboBox_Command.Items.AddRange(this.recents.ToArray());

			// Immediately update the updated item list:
			comboBox_Command.Refresh();

			// Restore cursor position and selection:
			comboBox_Command.SelectionLength = selectionLength;
			comboBox_Command.SelectionStart = selectionStart;

			this.isSettingControls.Leave();
		}

		private void SetCursorToEnd()
		{
			this.isSettingControls.Enter();

			comboBox_Command.SelectionStart = comboBox_Command.Text.Length;

			this.isSettingControls.Leave();
		}

		#endregion

		#region Private Methods > Multi Line Command
		//------------------------------------------------------------------------------------------
		// Private Methods > Multi Line Command
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Almost duplicated code in <see cref="YAT.Gui.Controls.PredefinedCommandSettingsSet.ShowMultiLineCommandBox"/>.
		/// </remarks>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowMultiLineCommandBox(Control requestingControl)
		{
			// Indicate multi line command.
			this.isSettingControls.Enter();
			comboBox_Command.Text      = Command.MultiLineCommandText;
			comboBox_Command.ForeColor = SystemColors.ControlText;
			comboBox_Command.Font      = SystemFonts.DefaultFont;
			this.isSettingControls.Leave();

			// Calculate startup location.
			Rectangle area = requestingControl.RectangleToScreen(requestingControl.DisplayRectangle);
			Point formStartupLocation = new Point();
			formStartupLocation.X = area.X + area.Width;
			formStartupLocation.Y = area.Y + area.Height;

			// Show multi line box.
			MultiLineBox f = new MultiLineBox(this.command, formStartupLocation, this.parseMode);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				this.isValidated = true; // Command has been validated by multi line box.
				this.command = f.CommandResult;

				SetControls();
				OnCommandChanged(EventArgs.Empty);
			}
			else
			{
				SetControls();
			}

			button_SendCommand.Select();
		}

		#endregion

		#region Private Methods > Handle Command
		//------------------------------------------------------------------------------------------
		// Private Methods > Handle Command
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreateSingleLineCommand(string singleLineCommand)
		{
			this.command = new Command(singleLineCommand);

			SetControls();
			OnCommandChanged(EventArgs.Empty);
		}

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreatePartialCommand(string partialCommand)
		{
			this.command = new Command(partialCommand, true);

			SetControls();
			OnCommandChanged(EventArgs.Empty);
		}

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreatePartialEolCommand()
		{
			this.command = new Command(true);

			SetControls();
			OnCommandChanged(EventArgs.Empty);
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
				CreatePartialEolCommand();
				RequestSendPartialEolCommand();
			}
			else
			{
				// No need to create the command again, it has already been created on validation.
				RequestSendCompleteCommand();
			}
		}

		private void RequestSendCompleteCommand()
		{
			if (this.isValidated)
			{
				InvokeSendCommandRequest();
			}
			else
			{
				if (ValidateChildren())
					InvokeSendCommandRequest();
			}
		}

		/// <remarks>Required when sending immediately.</remarks>
		private void RequestSendPartialCommand()
		{
			InvokeSendCommandRequest();
		}

		/// <remarks>Required when sending EOL immediately.</remarks>
		private void RequestSendPartialEolCommand()
		{
			InvokeSendCommandRequest();
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
