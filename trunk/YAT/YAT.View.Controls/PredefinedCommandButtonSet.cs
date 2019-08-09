﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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

		private const Domain.Parser.Modes ParseModeForTextDefault = Domain.Parser.Modes.Default;
		private const bool TerminalIsReadyToSendDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<Button> buttons_commands;

	////private SettingControlsHelper isSettingControls; is not needed (yet).

		private int subpageId = SubpageIdDefault;
		private List<Command> commands; // = null;

		private Domain.Parser.Modes parseModeForText = ParseModeForTextDefault;
		private string rootDirectoryForFile; // = null;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;

		private int commandStateUpdatedSuspendedCount; // = 0;

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
		public virtual Domain.Parser.Modes ParseModeForText
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
		public virtual bool TerminalIsReadyToSend
		{
			set
			{
				this.terminalIsReadyToSend = value;
				SetCommandStateControls();
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Returns command at the specified <paramref name="id"/>.
		/// Returns <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromId(int id)
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
							return (c);
					}
				}
			}

			return (null);
		}

		/// <summary>
		/// Returns command ID (1..max) that is assigned to the button at the specified location.
		/// Returns 0 if no button.
		/// </summary>
		public virtual int GetCommandIdFromLocation(Point location)
		{
			Point pt = PointToClient(location); // Using Control.PointToClient() is OK since buttons are directly placed onto control.
			// No using GetChildAtPoint() to also support clicking inbetween buttons.

			// Ensure that location is within control:
			if ((pt.X < 0) || (pt.X > Width))  return (0);
			if ((pt.Y < 0) || (pt.Y > Height)) return (0);

			// Find the corresponding button:
			for (int i = 0; i < this.buttons_commands.Count; i++)
			{
				if (pt.Y <= this.buttons_commands[i].Bottom)
				{
					int relativeCommandIndex = i;
					int absoluteCommandIndex = (SubpageCommandIndexOffset + relativeCommandIndex);
					int absoluteCommandId    = (absoluteCommandIndex + 1);
					return (absoluteCommandId);
				}
			}

			return (0);
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromLocation(Point location)
		{
			return (GetCommandFromId(GetCommandIdFromLocation(location)));
		}

		/// <remarks>Useful to improve performance.</remarks>
		public virtual void SuspendCommandStateUpdate()
		{
			this.commandStateUpdatedSuspendedCount++;
		}

		/// <remarks>Useful to improve performance.</remarks>
		public virtual void ResumeCommandStateUpdate()
		{
			this.commandStateUpdatedSuspendedCount--;
			if (this.commandStateUpdatedSuspendedCount <= 0)
			{
				this.commandStateUpdatedSuspendedCount = 0; // Prevent misuse.
				SetCommandStateControls();
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
		private void PredefinedCommandButtonSet_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;

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
		}

		private void SetControls()
		{
		////this.isSettingControls.Enter(); is not needed (yet).
			try
			{
				switch (SubpageId)
				{
					case 1:
						label_Hint.Text = "[Ctrl+] Shift+F1..F12 to send | to copy";
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
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + SubpageId.ToString() + "' is an ID that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				SetCommandControls();
			}
			finally
			{
			////this.isSettingControls.Leave(); is not needed (yet).
			}
		}

		private void SetCommandControls()
		{
		////this.isSettingControls.Enter(); is not needed (yet).
			try
			{
				SetCommandTextControls();
				SetCommandStateControls();
			}
			finally
			{
			////this.isSettingControls.Leave(); is not needed (yet).
			}
		}

		private void SetCommandTextControls()
		{
		////this.isSettingControls.Enter(); is not needed (yet).
			try
			{
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
			}
			finally
			{
			////this.isSettingControls.Leave(); is not needed (yet).
			}
		}

		private void SetCommandStateControls()
		{
		////this.isSettingControls.Enter(); is not needed (yet).
			try
			{
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

					if (isDefined)
					{
						bool isValid = (this.terminalIsReadyToSend && this.commands[commandIndex].IsValid(this.parseModeForText, this.rootDirectoryForFile));

						if (this.buttons_commands[i].ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
							this.buttons_commands[i].ForeColor = SystemColors.ControlText;

						if (this.buttons_commands[i].Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
							this.buttons_commands[i].Font = SystemFonts.DefaultFont;

						this.buttons_commands[i].Enabled = isValid;
					}
					else
					{
						if (this.buttons_commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
							this.buttons_commands[i].ForeColor = SystemColors.GrayText;

						if (this.buttons_commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
							this.buttons_commands[i].Font = DrawingEx.DefaultFontItalic;

						this.buttons_commands[i].Enabled = true;
					}
				}
			}
			finally
			{
			////this.isSettingControls.Leave(); is not needed (yet).
			}
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
