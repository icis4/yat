//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using MKY.Utilities.Event;

using YAT.Model.Settings;
using YAT.Model.Types;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class PredefinedCommandPageButtons : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool TerminalIsOpenDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private List<Button> buttons_commands;

		private List<Command> commands;
		private bool terminalIsOpen = TerminalIsOpenDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Action")]
		[Description("Event raised when sending a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> SendCommandRequest;

		[Category("Action")]
		[Description("Event raised when defining a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> DefineCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public PredefinedCommandPageButtons()
		{
			InitializeComponent();
			InitializeButtons();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual List<Command> Commands
		{
			set
			{
				this.commands = value;
				SetControls();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsOpen
		{
			set
			{
				this.terminalIsOpen = value;
				SetControls();
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Returns command at the specified id.
		/// Returns <c>null</c> if command is undefined or not valid.
		/// </summary>
		public virtual Command GetCommandFromId(int id)
		{
			int i = (id - 1); // command ID = 1..max

			if (this.commands != null)
			{
				if ((i >= 0) && (i < this.commands.Count))
				{
					if (this.commands[i] != null)
					{
						if (this.commands[i].IsDefined)
							return (this.commands[i]);
					}
				}
			}
			return (null);
		}

		/// <summary>
		/// Returns command ID (1..max) that is assigned to the button at the specified location.
		/// Returns 0 if no button.
		/// </summary>
		public virtual int GetCommandIdFromScreenPoint(Point p)
		{
			Point client = PointToClient(p);

			// ensure that location is within control
			if ((client.X < 0) || (client.X > Width))
				return (0);
			if ((client.Y < 0) || (client.Y > Height))
				return (0);

			int ySum = 0;
			for (int i = 0; i < this.buttons_commands.Count; i++)
			{
				ySum += this.buttons_commands[i].Height;
				if (client.Y <= ySum)
					return (i + 1); // commmand ID = 1..max
			}
			return (0);
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or not valid.
		/// </summary>
		public Command GetCommandFromScreenPoint(Point p)
		{
			return (GetCommandFromId(GetCommandIdFromScreenPoint(p)));
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void button_Command_Click(object sender, EventArgs e)
		{
			CommandRequest(int.Parse((string)(((Button)sender).Tag)));
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeButtons()
		{
			this.buttons_commands = new List<Button>(PredefinedCommandSettings.MaxCommandsPerPage);
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
			int commandCount = 0;
			if (this.commands != null)
				commandCount = this.commands.Count;

			for (int i = 0; i < commandCount; i++)
			{
				bool isDefined = ((this.commands[i] != null) && this.commands[i].IsDefined);
				bool isValid = (isDefined && this.terminalIsOpen && this.commands[i].IsValid);

				if (isDefined)
				{
					this.buttons_commands[i].Text      = this.commands[i].Description;
					this.buttons_commands[i].ForeColor = SystemColors.ControlText;
					this.buttons_commands[i].Font      = SystemFonts.DefaultFont;
					this.buttons_commands[i].Enabled   = isValid;
				}
				else
				{
					this.buttons_commands[i].Text      = Command.DefineCommandText;
					this.buttons_commands[i].ForeColor = SystemColors.GrayText;
					this.buttons_commands[i].Font      = Utilities.Drawing.ItalicDefaultFont;
					this.buttons_commands[i].Enabled   = true;
				}
			}
			for (int i = commandCount; i < PredefinedCommandSettings.MaxCommandsPerPage; i++)
			{
				this.buttons_commands[i].Text      = Command.DefineCommandText;
				this.buttons_commands[i].ForeColor = SystemColors.GrayText;
				this.buttons_commands[i].Font      = Utilities.Drawing.ItalicDefaultFont;
				this.buttons_commands[i].Enabled   = true;
			}
		}

		private void CommandRequest(int command)
		{
			bool isDefined =
				(
				(this.commands != null) &&
				(this.commands.Count >= command) &&
				(this.commands[command - 1] != null) &&
				(this.commands[command - 1].IsDefined)
				);

			if (isDefined)
				RequestSendCommand(command);
			else
				RequestDefineCommand(command);
		}

		private void RequestSendCommand(int command)
		{
			OnSendCommandRequest(new PredefinedCommandEventArgs(command));
		}

		private void RequestDefineCommand(int command)
		{
			OnDefineCommandRequest(new PredefinedCommandEventArgs(command));
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnSendCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.FireSync<PredefinedCommandEventArgs>(SendCommandRequest, this, e);
		}

		protected virtual void OnDefineCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.FireSync<PredefinedCommandEventArgs>(DefineCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
