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
	[DefaultEvent("SendCommandRequest")]
	public partial class SendText : UserControl
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

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypeDefault;
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
		private RecentItemCollection<Command> recent;
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
		public SendText()
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
					CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

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

					CommandDebugMessageLeave();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> Recent
		{
			set
			{
				CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

				// Do not check if (this.recent != value) because the collection will always be the same!

				this.recent = value;
				SetRecentControls(); // Recent must immediately be updated, otherwise order will be wrong on arrow-up/down.

				CommandDebugMessageLeave();
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
					SetSendControls();
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
					SetSendControls();
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
					SetSendControls();
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
					SetSendControls();
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
				// Do not check if (this.splitterDistance != value) because the distance (position)
				// will be limited to the control's width, and that may change AFTER the distance
				// has been set.

				this.splitterDistance = value;

				// No need to call SetControls(); as only the splitter will be moved, and that will
				// not be accessed anywhere else.

				splitContainer.SplitterDistance = Int32Ex.LimitToBounds((this.splitterDistance - splitContainer.Left), 0, (splitContainer.Width - 1));
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
			comboBox_SingleLineText.Select();
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

		private void comboBox_SingleLineText_Enter(object sender, EventArgs e)
		{
			CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

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

			CommandDebugMessageLeave();
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
			CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (this.isValidated)
				SetEditFocusState(EditFocusState.EditIsInactive);
			else
				SetEditFocusState(EditFocusState.IsLeavingEdit);

			CommandDebugMessageLeave();
		}

		private void comboBox_SingleLineText_KeyPress(object sender, KeyPressEventArgs e)
		{
			CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (this.sendImmediately)
			{
				this.isValidated = true;
				CreateAndConfirmPartialCommand(e.KeyChar.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for keys!
				InvokeSendCommandRequest();
			}

			CommandDebugMessageLeave();
		}

		private void comboBox_SingleLineText_TextChanged(object sender, EventArgs e)
		{
			CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (!this.isSettingControls)
			{
				if (this.sendImmediately)
					comboBox_SingleLineText.Text = ""; // Instantly reset the text.
				else
					this.isValidated = false; // Reset the validation flag.

				SetSendControls();
			}

			CommandDebugMessageLeave();
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
				CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

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

							CreateAndConfirmSingleLineText(comboBox_SingleLineText.Text);

							CommandDebugMessageLeave();
							return;
						}

						// Single line => Validate!
						int invalidTextStart;
						int invalidTextLength;
						if (Validation.ValidateText(this, "text", comboBox_SingleLineText.Text, /* FR#238 add this.defaultRadix */ this.parseMode, out invalidTextStart, out invalidTextLength))
						{
							this.isValidated = true;

							if (this.editFocusState == EditFocusState.IsLeavingEdit)
								SetEditFocusState(EditFocusState.EditIsInactive);

							CreateAndConfirmSingleLineText(comboBox_SingleLineText.Text);

							CommandDebugMessageLeave();
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

				CommandDebugMessageLeave();
			}
		}

		private void comboBox_SingleLineText_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

				this.isValidated = true; // Commands in history have already been validated.

				if (comboBox_SingleLineText.SelectedItem != null)
				{
					var ri = (comboBox_SingleLineText.SelectedItem as RecentItem<Command>);
					if (ri != null)
					{
						this.command = ri.Item;

						SetCommandControls();
						//// Do not call OnCommandChanged(), event shall only be invoked when command is requested.
					}
				}

				CommandDebugMessageLeave();
			}
		}
		
		private void button_MultiLine_Click(object sender, EventArgs e)
		{
			ShowMultiLineBox(button_MultiLine);
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

		#region Private Methods > Set Controls
		//------------------------------------------------------------------------------------------
		// Private Methods > Set Controls
		//------------------------------------------------------------------------------------------

		private void SetRecentControls()
		{
			CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
			CommandDebugMessageLeave();
		}

		/// <remarks>
		/// Separate function as it is not needed to set this text on any change.
		/// </remarks>
		private void SetCommandControls()
		{
			CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);
			this.isSettingControls.Enter();

			if (this.editFocusState == EditFocusState.EditIsInactive)
			{
				if (this.command.IsText)
				{
					comboBox_SingleLineText.Text      = this.command.SingleLineText;
					comboBox_SingleLineText.ForeColor = SystemColors.ControlText;
					comboBox_SingleLineText.Font      = SystemFonts.DefaultFont;
				}
				else
				{
					comboBox_SingleLineText.Text      = Command.EnterTextText;
					comboBox_SingleLineText.ForeColor = SystemColors.GrayText;
					comboBox_SingleLineText.Font      = Utilities.Drawing.ItalicDefaultFont;
				}
			}
			else
			{
				if (this.command.IsText && !this.command.IsPartialText)
					comboBox_SingleLineText.Text = this.command.SingleLineText;
				else
					comboBox_SingleLineText.Text = "";

				comboBox_SingleLineText.ForeColor = SystemColors.ControlText;
				comboBox_SingleLineText.Font      = SystemFonts.DefaultFont;
			}

			SetCursorToEnd();
			SetSendControls();

			this.isSettingControls.Leave();
			CommandDebugMessageLeave();
		}

		private void SetCursorToEnd()
		{
			CommandDebugMessageEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);
			this.isSettingControls.Enter();

			comboBox_SingleLineText.SelectionStart = comboBox_SingleLineText.Text.Length;

			this.isSettingControls.Leave();
			CommandDebugMessageLeave();
		}

		private void SetSendControls()
		{
			this.isSettingControls.Enter();

			// Prepare the button properties based on state and settings.
			//
			// \attention:
			// Similar code exists in the following locations:
			//  > YAT.Gui.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
			//  > YAT.Gui.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
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
			MultiLineBox f = new MultiLineBox(this.command, formStartupLocation, this.parseMode);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				this.isValidated = true; // Command has been validated by multi-line box.
				this.command = f.CommandResult;

				SetCommandControls();
				OnCommandChanged(EventArgs.Empty);
			}
			else
			{
				SetCommandControls();
				// Do not call OnCommandChanged(), nothing has changed.
			}

			button_Send.Select();
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

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreateAndConfirmSingleLineText(string singleLineText)
		{
			this.command = new Command(singleLineText);

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreateAndConfirmPartialCommand(string partialCommand)
		{
			this.command = new Command(partialCommand, true);

			SetCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreateAndConfirmPartialEolCommand()
		{
			this.command = new Command(true);

			SetCommandControls();
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
				CreateAndConfirmPartialEolCommand();
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
					if (ValidateChildren()) // CreateAndConfirmSingleLineText() gets called here.
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
		protected virtual void CommandDebugMessageEnter(string methodName)
		{
			Debug.WriteLine(methodName);
			Debug.Indent();

			CommandDebugMessage();
		}

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void CommandDebugMessageLeave()
		{
			CommandDebugMessage();

			Debug.Unindent();
		}

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void CommandDebugMessage()
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
