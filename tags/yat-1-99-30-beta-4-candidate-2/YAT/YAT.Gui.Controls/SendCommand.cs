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
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
		protected enum FocusState
		{
			Inactive,
			HasFocus,
			IsLeaving,
			IsLeavingControl,
		}

		#pragma warning restore 1591

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool TerminalIsReadyToSendDefault = false;
		private const float SplitterRatioDefault = (float)0.75;

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
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;
		private float splitterRatio = SplitterRatioDefault;

		private FocusState editFocusState = FocusState.Inactive;
		private bool isValidated;
		private bool sendIsRequested;

		private bool sendImmediately = SendImmediatelyDefault;
		private string partialCommandLine;

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

		/// <summary>
		/// Command always returns a Command object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command Command
		{
			get { return (this.command); }
			set
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

				OnCommandChanged(new EventArgs());
				SetControls();
			}
		}

		/// <summary></summary>
		[DefaultValue(SendImmediatelyDefault)]
		public virtual bool SendImmediately
		{
			get { return (this.sendImmediately); }
			set
			{
				this.sendImmediately = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> RecentCommands
		{
			set
			{
				this.recents = value;

				// Don't call SetControls(), recents are shown at DropDown.
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSend
		{
			set
			{
				this.terminalIsReadyToSend = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[DefaultValue(SplitterRatioDefault)]
		public virtual float SplitterRatio
		{
			get { return (this.splitterRatio); }
			set
			{
				this.splitterRatio = value;
				SetControls();
			}
		}

		/// <summary></summary>
		public virtual bool EditIsActive
		{
			get { return (this.editFocusState != FocusState.Inactive); }
		}

		/// <summary></summary>
		protected virtual void SetEditFocusState(FocusState editFocusSet)
		{
			this.editFocusState = editFocusSet;
			OnEditFocusStateChanged(new EventArgs());
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
						if (this.sendImmediately)
						{
							CreatePartialEolCommand();
							RequestSendPartialEolCommand();
						}
						else
						{
							RequestSendCompleteCommand();
						}

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

				// Move cursor to end.
				comboBox_Command.SelectionStart = comboBox_Command.Text.Length;
			}
		}

		/// <remarks>
		/// Do not modify <see cref="isValidated"/>. Command may already have been validated.
		/// </remarks>
		private void SendCommand_Enter(object sender, EventArgs e)
		{
			SetEditFocusState(FocusState.Inactive);
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
				SetEditFocusState(FocusState.Inactive);
			else
				SetEditFocusState(FocusState.IsLeavingControl);
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Command_DropDown(object sender, EventArgs e)
		{
			SetRecents();
		}

		private void comboBox_Command_Enter(object sender, EventArgs e)
		{
			// Clear "<Enter a command...>" if needed.
			if ((this.editFocusState == FocusState.Inactive) && !this.command.IsText)
			{
				this.isSettingControls.Enter();
				comboBox_Command.Text      = "";
				comboBox_Command.ForeColor = SystemColors.ControlText;
				comboBox_Command.Font      = SystemFonts.DefaultFont; 
				this.isSettingControls.Leave();
			}

			SetEditFocusState(FocusState.HasFocus);

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
				SetEditFocusState(FocusState.Inactive);
			else
				SetEditFocusState(FocusState.IsLeaving);
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
				if (!this.sendImmediately)
					this.isValidated = false;
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
				if (!this.isValidated && (this.editFocusState != FocusState.IsLeavingControl))
				{
					// Easter egg ;-)
					if (SendCommandSettings.IsEasterEggCommand(comboBox_Command.Text))
					{
						this.isValidated = true;

						if (this.editFocusState == FocusState.IsLeaving)
							SetEditFocusState(FocusState.Inactive);
						else
							SetEditFocusState(FocusState.HasFocus);

						CreateSingleLineCommand(comboBox_Command.Text);
						return;
					}

					// Single line => Validate!
					int invalidTextStart;
					int invalidTextLength;
					if (Validation.ValidateSequence(this, "Command", comboBox_Command.Text, out invalidTextStart, out invalidTextLength))
					{
						this.isValidated = true;

						if (this.editFocusState == FocusState.IsLeaving)
							SetEditFocusState(FocusState.Inactive);
						else
							SetEditFocusState(FocusState.HasFocus);

						CreateSingleLineCommand(comboBox_Command.Text);
						return;
					}

					SetEditFocusState(FocusState.HasFocus);
					comboBox_Command.Select(invalidTextStart, invalidTextLength);
					e.Cancel = true;
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
						SetCommand(ri.Item);
				}
			}
		}
		
		private void button_MultiLineCommand_Click(object sender, EventArgs e)
		{
			ShowMultiLineCommandBox(button_MultiLineCommand);
		}

		private void button_SendCommand_Click(object sender, EventArgs e)
		{
			RequestSendCompleteCommand();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			splitContainer.SplitterDistance = Int32Ex.LimitToBounds((int)(this.splitterRatio * splitContainer.Width), 0, splitContainer.Width);

			if (this.editFocusState == FocusState.Inactive)
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
			else if (this.sendIsRequested)
			{   // Needed when command is modified (e.g. cleared) after send.
				if (this.command.IsText)
					comboBox_Command.Text  = this.command.SingleLineText;
				else
					comboBox_Command.Text  = "";

				comboBox_Command.ForeColor = SystemColors.ControlText;
				comboBox_Command.Font      = SystemFonts.DefaultFont;
			}

			if (this.sendImmediately)
				button_SendCommand.Text = "Send EOL (F3)";
			else
				button_SendCommand.Text = "Send Command (F3)";

			button_SendCommand.Enabled = this.terminalIsReadyToSend;

			this.isSettingControls.Leave();
		}

		private void SetRecents()
		{
			this.isSettingControls.Enter();

			comboBox_Command.Items.Clear();
			if (this.recents != null)
				comboBox_Command.Items.AddRange(this.recents.ToArray());

			this.isSettingControls.Leave();
		}

		private void SetCommand(Command command)
		{
			this.command = command;

			SetControls();
			OnCommandChanged(new EventArgs());
		}

		/// <remarks>
		/// Always create new command to ensure that not only command but also description is updated.
		/// </remarks>
		private void CreateSingleLineCommand(string commandLine)
		{
			this.command = new Command(commandLine);

			SetControls();
			OnCommandChanged(new EventArgs());
		}

		private void CreatePartialCommand(string partialCommand)
		{
			this.command = new Command(partialCommand, true);

			if (this.partialCommandLine == null)
				this.partialCommandLine = partialCommand;
			else
				this.partialCommandLine += partialCommand;

			SetControls();
			OnCommandChanged(new EventArgs());
		}

		private void CreatePartialEolCommand()
		{
			this.command = new Command(true, this.partialCommandLine);

			SetControls();
			OnCommandChanged(new EventArgs());
		}

		private void ResetPartialCommand()
		{
			this.partialCommandLine = null;
		}

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
			MultiLineBox f = new MultiLineBox(this.command, formStartupLocation);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				this.isValidated = true; // Command has been validated by multi line box.
				SetCommand(f.CommandResult);
			}
			else
			{
				SetControls();
			}

			button_SendCommand.Select();
		}

		private void RequestSendCompleteCommand()
		{
			ResetPartialCommand();

			if (this.isValidated)
			{
				RequestSendCommand();
			}
			else
			{
				if (ValidateChildren())
					RequestSendCommand();
			}
		}

		private void RequestSendPartialCommand()
		{
			RequestSendCommand();
		}

		private void RequestSendPartialEolCommand()
		{
			ResetPartialCommand();
			RequestSendCommand();
		}

		private void RequestSendCommand()
		{
			if (this.editFocusState == FocusState.Inactive)
			{
				OnSendCommandRequest(new EventArgs());
			}
			else
			{
				// Notifying the send state is needed when command is automatically
				//   modified (e.g. cleared) after send.
				this.sendIsRequested = true;
				OnSendCommandRequest(new EventArgs());
				this.sendIsRequested = false;
			}
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
