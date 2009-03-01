using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;

using YAT.Model.Types;
using YAT.Model.Settings;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class PredefinedCommandPageButtons : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool _TerminalIsOpenDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private List<Button> _buttons_commands;

		private List<Command> _commands;
		private bool _terminalIsOpen = _TerminalIsOpenDefault;

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
		public List<Command> Commands
		{
			set
			{
				_commands = value;
				SetControls();
			}
		}

		[Browsable(false)]
		[DefaultValue(_TerminalIsOpenDefault)]
		public bool TerminalIsOpen
		{
			set
			{
				_terminalIsOpen = value;
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
		public Command GetCommandFromId(int id)
		{
			int i = (id - 1); // command ID = 1..max

			if (_commands != null)
			{
				if ((i >= 0) && (i < _commands.Count))
				{
					if (_commands[i] != null)
					{
						if (_commands[i].IsDefined)
							return (_commands[i]);
					}
				}
			}
			return (null);
		}

		/// <summary>
		/// Returns command ID (1..max) that is assigned to the button at the specified location.
		/// Returns 0 if no button.
		/// </summary>
		public int GetCommandIdFromScreenPoint(Point p)
		{
			Point client = PointToClient(p);

			// ensure that location is within control
			if ((client.X < 0) || (client.X > Width))
				return (0);
			if ((client.Y < 0) || (client.Y > Height))
				return (0);

			int ySum = 0;
			for (int i = 0; i < _buttons_commands.Count; i++)
			{
				ySum += _buttons_commands[i].Height;
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
			_buttons_commands = new List<Button>(PredefinedCommandSettings.MaximumCommandsPerPage);
			_buttons_commands.Add(button_Command_1);
			_buttons_commands.Add(button_Command_2);
			_buttons_commands.Add(button_Command_3);
			_buttons_commands.Add(button_Command_4);
			_buttons_commands.Add(button_Command_5);
			_buttons_commands.Add(button_Command_6);
			_buttons_commands.Add(button_Command_7);
			_buttons_commands.Add(button_Command_8);
			_buttons_commands.Add(button_Command_9);
			_buttons_commands.Add(button_Command_10);
			_buttons_commands.Add(button_Command_11);
			_buttons_commands.Add(button_Command_12);
		}

		private void SetControls()
		{
			int commandCount = 0;
			if (_commands != null)
				commandCount = _commands.Count;

			for (int i = 0; i < commandCount; i++)
			{
				bool isDefined = ((_commands[i] != null) && _commands[i].IsDefined);
				bool isValid = (isDefined && _terminalIsOpen && _commands[i].IsValid);

				if (isDefined)
				{
					_buttons_commands[i].Text = _commands[i].Description;
					_buttons_commands[i].Enabled = isValid;
				}
				else
				{
					_buttons_commands[i].Text = Command.DefineCommandText;
					_buttons_commands[i].Enabled = true;
				}
			}
			for (int i = commandCount; i < PredefinedCommandSettings.MaximumCommandsPerPage; i++)
			{
				_buttons_commands[i].Text = Command.DefineCommandText;
				_buttons_commands[i].Enabled = true;
			}
		}

		private void CommandRequest(int command)
		{
			bool isDefined =
				(
				(_commands != null) &&
				(_commands.Count >= command) &&
				(_commands[command - 1] != null) &&
				(_commands[command - 1].IsDefined)
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
