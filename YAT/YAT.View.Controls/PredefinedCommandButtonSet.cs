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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Drawing;
using MKY.Windows.Forms;

using YAT.Model.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("SendCommandRequest")]
	public partial class PredefinedCommandButtonSet : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SubpageIdDefault = PredefinedCommandPage.FirstSubpageId;

		private const Domain.Parser.Mode ParseModeForTextDefault = Domain.Parser.Mode.Default;
		private const bool TerminalIsReadyToSendForSomeTimeDefault = false;

		private const bool HideUndefinedCommandsDefault = Model.Settings.PredefinedCommandSettings.HideUndefinedCommandsDefault;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<Button> buttons_commands;

		/// <summary>Array of flags indicating the previous state of each button.</summary>
		/// <remarks>Used for improving performance because <see cref="SystemFonts.DefaultFont"/> and <see cref="DrawingEx.DefaultFontItalic"/> are quite slow.</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool[] buttons_commands_wasDefined;

	////private SettingControlsHelper isSettingControls; is not needed (yet).

		private int subpageId = SubpageIdDefault;
		private List<Command> commands; // = null;

		private Domain.Parser.Mode parseModeForText = ParseModeForTextDefault;
		private string rootDirectoryForFile; // = null;
		private bool terminalIsReadyToSendForSomeTime = TerminalIsReadyToSendForSomeTimeDefault;

		private bool hideUndefinedCommands = HideUndefinedCommandsDefault;

		private int commandStateUpdateSuspendedCount; // = 0;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> SendCommandRequest;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when defining a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> DefineCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public PredefinedCommandButtonSet()
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

		/// <summary></summary>
		[Category("Behavior")]
		[Description("The represented subpage.")]
		[DefaultValue(SubpageIdDefault)]
		public virtual int SubpageId
		{
			get { return (this.subpageId); }
			set
			{
				this.subpageId = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual List<Command> Commands
		{
			set
			{
				this.commands = value;
				SetCommandControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.Parser.Mode ParseModeForText
		{
			set
			{
				this.parseModeForText = value;
				SetCommandStateControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string RootDirectoryForFile
		{
			set
			{
				this.rootDirectoryForFile = value;
				SetCommandStateControls();
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
				this.terminalIsReadyToSendForSomeTime = value;
				SetCommandStateControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool HideUndefinedCommands
		{
			set
			{
				this.hideUndefinedCommands = value;
				SetCommandStateControls();
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Sets <paramref name="command"/> to the command specified by <paramref name="id"/>.
		/// Returns <c>false</c> and sets <paramref name="command"/> to <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual bool TryGetCommandFromId(int id, out Command command)
		{
			int i = (id - 1); // ID = 1..max

			if (this.commands != null)
			{
				if ((i >= 0) && (i < this.commands.Count))
				{
					var c = this.commands[i];
					if (c != null)
					{
						if (c.IsDefined)
						{
							command = c;
							return (true);
						}
					}
				}
			}

			command = null;
			return (false);
		}

		/// <summary>
		/// Sets <paramref name="id"/> to command ID (1..max) that is assigned to the button at the specified location.
		/// Returns <c>false</c> and sets <paramref name="id"/> to <c>0</c> if no button.
		/// </summary>
		public virtual bool TryGetCommandIdFromLocation(Point location, out int id)
		{
			Point pt = PointToClient(location); // Using Control.PointToClient() is OK since buttons are directly placed onto control.
			//// Not using GetChildAtPoint() to also support clicking inbetween buttons.

			// Ensure that location is within control:
			if ((pt.X < 0) || (pt.X > Width))  { id = 0; return (false); }
			if ((pt.Y < 0) || (pt.Y > Height)) { id = 0; return (false); }

			// Find the corresponding button:
			for (int i = 0; i < this.buttons_commands.Count; i++)
			{
				if (pt.Y <= this.buttons_commands[i].Bottom)
				{
					int relativeCommandIndex = i;
					int absoluteCommandIndex = (SubpageCommandIndexOffset + relativeCommandIndex);
					int  absoluteCommandId   = (absoluteCommandIndex + 1);
					id = absoluteCommandId;
					return (true);
				}
			}

			id = 0;
			return (false);
		}

		/// <summary>
		/// Sets <paramref name="command"/> to the command that is assigned to the button at the specified location.
		/// Returns <c>false</c> and sets <paramref name="command"/> to <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual bool TryGetCommandFromLocation(Point location, out Command command)
		{
			int id;
			if (TryGetCommandIdFromLocation(location, out id))
				return (TryGetCommandFromId(id, out command));

			command = null;
			return (false);
		}

		/// <remarks>Useful to improve performance.</remarks>
		public virtual void SuspendCommandStateUpdate()
		{
			this.commandStateUpdateSuspendedCount++; // No need for "lock"/"Interlocked...()" as WinForms is synchronized on main thread.
		}

		/// <remarks>Useful to improve performance.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void ResumeCommandStateUpdate(bool performUpdate = true)
		{
			this.commandStateUpdateSuspendedCount--; // No need for "lock"/"Interlocked...()" as WinForms is synchronized on main thread.
			if (this.commandStateUpdateSuspendedCount == 0)
			{
				if (performUpdate)
					SetCommandStateControls();
			}

			if (this.commandStateUpdateSuspendedCount < 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The " + this.GetType() + " command state update suspend counter has become less than 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <remarks>Useful to improve performance.</remarks>
		public virtual bool CommandStateUpdateIsSuspended
		{
			get { return (this.commandStateUpdateSuspendedCount > 0); }
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Flag only used by the following event handler.
		/// </summary>
		private bool PredefinedCommandButtonSet_Paint_IsFirst { get; set; } = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void PredefinedCommandButtonSet_Paint(object sender, PaintEventArgs e)
		{
			if (PredefinedCommandButtonSet_Paint_IsFirst) {
				PredefinedCommandButtonSet_Paint_IsFirst = false;

				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void button_Command_Click(object sender, EventArgs e)
		{
			CommandRequest(ControlEx.TagToInt32(sender));
		}

		#endregion

		#region Non-Public Properties
		//==========================================================================================
		// Non-Public Properties
		//==========================================================================================

		/// <summary></summary>
		protected virtual int SubpageIndex
		{
			get { return (this.subpageId - 1); }
		}

		/// <summary></summary>
		protected virtual int SubpageCommandIndexOffset
		{
			get { return (SubpageIndex * PredefinedCommandPage.CommandCapacityPerSubpage); }
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.buttons_commands = new List<Button>(PredefinedCommandPage.CommandCapacityPerSubpage); // Preset the required capacity to improve memory management.
			this.buttons_commands.Add(button_Command_1);
			this.buttons_commands.Add(button_Command_2);
			this.buttons_commands.Add(button_Command_3);
			this.buttons_commands.Add(button_Command_4);
			this.buttons_commands.Add(button_Command_5);
			this.buttons_commands.Add(button_Command_6);
			this.buttons_commands.Add(button_Command_7);
			this.buttons_commands.Add(button_Command_8);
			this.buttons_commands.Add(button_Command_9);
			this.buttons_commands.Add(button_Command_10);
			this.buttons_commands.Add(button_Command_11);
			this.buttons_commands.Add(button_Command_12);

			this.buttons_commands_wasDefined = new bool[PredefinedCommandPage.CommandCapacityPerSubpage];
			for (var i = 0; i < PredefinedCommandPage.CommandCapacityPerSubpage; i++)
				this.buttons_commands_wasDefined[i] = false;
		}

		private void SetControls()
		{
		////this.isSettingControls.Enter(); is not needed (yet).
		////try
		////{
				switch (SubpageId)
				{
					case 1:
						label_Hint.Text = "[Ctrl+] Shift+F1..F12 to send [to copy]";
						break;

					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						label_Hint.Text = PredefinedCommandPage.SubpageIdToString(SubpageId);
						break;

					default:
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + SubpageId.ToString(CultureInfo.CurrentCulture) + "' is an ID that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				SetCommandControls();
		////}
		////finally
		////{
		////	this.isSettingControls.Leave(); is not needed (yet).
		////}
		}

		private void SetCommandControls()
		{
		////this.isSettingControls.Enter(); is not needed (yet).
		////try
		////{
				SetCommandTextControls();
				SetCommandStateControls();
		////}
		////finally
		////{
		////	this.isSettingControls.Leave(); is not needed (yet).
		////}
		}

		private void SetCommandTextControls()
		{
		////this.isSettingControls.Enter(); is not needed (yet).
		////try
		////{
				// Attention:
				// Similar code exists in...
				// ...SetCommandStateControls() below
				// ...CommandRequest() further below
				// ...View.Forms.PredefinedCommandSettings.SetPageControls()
				// ...View.Forms.Terminal.contextMenuStrip_Command_SetMenuItems()
				// Changes here may have to be applied there too.

				int commandCount = 0;
				if (this.commands != null)
					commandCount = this.commands.Count;

				for (int i = 0; i < PredefinedCommandPage.CommandCapacityPerSubpage; i++)
				{
					int commandIndex = (SubpageCommandIndexOffset + i);

					bool isDefined =
					(
						(commandIndex < commandCount) &&
						(this.commands[commandIndex] != null) &&
						(this.commands[commandIndex].IsDefined)
					);

					if (isDefined)
					{
						this.buttons_commands[i].Text = this.commands[commandIndex].Description;
						toolTip.SetToolTip(this.buttons_commands[i], @"Send """ + this.commands[commandIndex].SingleLineText + @"""");
					}
					else
					{
						this.buttons_commands[i].Text = Command.DefineCommandText;
						toolTip.SetToolTip(this.buttons_commands[i], Command.DefineCommandText);
					}
				}
		////}
		////finally
		////{
		////	this.isSettingControls.Leave(); is not needed (yet).
		////}
		}

	////private int SetCommandStateControls_updateCounter; // Also exists in several other locations. Can temporarily be used for debugging the command state update (performance relevant).

		private void SetCommandStateControls()
		{
			if (CommandStateUpdateIsSuspended)
				return;

		////System.Diagnostics.Debug.WriteLine("BS @ " + SetCommandStateControls_updateCounter++); // Also exists in several other locations. Can temporarily be used for debugging the command state update (performance relevant).

		////this.isSettingControls.Enter(); is not needed (yet).
		////try
		////{
				// Attention:
				// Similar code exists in...
				// ...SetCommandTextControls() above
				// ...CommandRequest() further below
				// ...View.Forms.PredefinedCommandSettings.SetPageControls()
				// ...View.Forms.Terminal.contextMenuStrip_Command_SetMenuItems()
				// Changes here may have to be applied there too.

				int commandCount = 0;
				if (this.commands != null)
					commandCount = this.commands.Count;

				for (int i = 0; i < PredefinedCommandPage.CommandCapacityPerSubpage; i++)
				{
					int commandIndex = (SubpageCommandIndexOffset + i);

					bool isDefined =
					(
						(commandIndex < commandCount) &&
						(this.commands[commandIndex] != null) &&
						(this.commands[commandIndex].IsDefined)
					);

					bool isVisible =
					(
						(!this.hideUndefinedCommands) ||
						((i == 0) && (commandCount == 0)) // At least one command shall be shown:
					);

					if (isDefined)
					{
						if (!this.buttons_commands_wasDefined[i]) // Improve performance by only accessing 'SystemFonts.DefaultFont' when really needed!
						{
							if (this.buttons_commands[i].ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
								this.buttons_commands[i].ForeColor = SystemColors.ControlText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
								                                             //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
							if (this.buttons_commands[i].Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
								this.buttons_commands[i].Font = SystemFonts.DefaultFont;  // Improves because 'Font' is managed by a 'PropertyStore'.
						}

						bool isValid = (this.terminalIsReadyToSendForSomeTime && this.commands[commandIndex].IsValid(this.parseModeForText, this.rootDirectoryForFile));
						this.buttons_commands[i].Enabled = isValid;
						this.buttons_commands[i].Visible = true;
					}
					else
					{
						if (this.buttons_commands_wasDefined[i]) // Improve performance by only accessing 'DrawingEx.DefaultFontItalic' when really needed!
						{
							if (this.buttons_commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
								this.buttons_commands[i].ForeColor = SystemColors.GrayText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
							                                               //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
							if (this.buttons_commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
								this.buttons_commands[i].Font = DrawingEx.DefaultFontItalic;  // Improves because 'Font' is managed by a 'PropertyStore'.
						}

						this.buttons_commands[i].Enabled = true;
						this.buttons_commands[i].Visible = isVisible;
					}

					this.buttons_commands_wasDefined[i] = isDefined;
				}
		////}
		////finally
		////{
		////	this.isSettingControls.Leave(); is not needed (yet).
		////}
		}

		private void CommandRequest(int relativeCommandId)
		{
			int relativeCommandIndex = (relativeCommandId - 1);
			int absoluteCommandIndex = (SubpageCommandIndexOffset + relativeCommandIndex);
			int absoluteCommandId    = (absoluteCommandIndex + 1);

			// Attention:
			// Similar code exists in...
			// ...SetCommandTextControls() further above
			// ...SetCommandStateControls() further above
			// ...View.Forms.PredefinedCommandSettings.SetPageControls()
			// ...View.Forms.Terminal.contextMenuStrip_Command_SetMenuItems()
			// Changes here may have to be applied there too.

			int commandCount = 0;
			if (this.commands != null)
				commandCount = this.commands.Count;

			bool isDefined =
			(
				(absoluteCommandIndex < commandCount) &&
				(this.commands[absoluteCommandIndex] != null) &&
				(this.commands[absoluteCommandIndex].IsDefined)
			);

			if (isDefined)
				RequestSendCommand(absoluteCommandId);
			else
				RequestDefineCommand(absoluteCommandId);
		}

		/// <summary></summary>
		protected virtual void RequestSendCommand(int commandId)
		{
			OnSendCommandRequest(new PredefinedCommandEventArgs(commandId)); // e.PageId is not defined since set doesn't know the page.
		}

		/// <summary></summary>
		protected virtual void RequestDefineCommand(int commandId)
		{
			OnDefineCommandRequest(new PredefinedCommandEventArgs(commandId)); // e.PageId is not defined since set doesn't know the page.
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnSendCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.RaiseSync<PredefinedCommandEventArgs>(SendCommandRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDefineCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.RaiseSync<PredefinedCommandEventArgs>(DefineCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
