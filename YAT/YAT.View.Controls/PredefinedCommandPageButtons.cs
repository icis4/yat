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

using YAT.Model.Settings;
using YAT.Model.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("SendCommandRequest")]
	public partial class PredefinedCommandPageButtons : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool TerminalIsReadyToSendDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<Button> buttons_commands;

		private List<Command> commands;

		private string rootDirectory; // = null;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;

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
				SetControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string RootPath
		{
			set
			{
				this.rootDirectory = value;
				SetControls();
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
		/// Returns <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromId(int id)
		{
			int i = (id - 1); // ID = 1..max

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
		public virtual int GetCommandIdFromLocation(Point point)
		{
			Point requested = PointToClient(point);

			// Ensure that location is within control:
			if ((requested.X < 0) || (requested.X > Width))  return (0);
			if ((requested.Y < 0) || (requested.Y > Height)) return (0);

			// Find corresponding button:
			for (int i = 0; i < this.buttons_commands.Count; i++)
			{
				if (requested.Y <= this.buttons_commands[i].Bottom)
					return (i + 1); // ID = 1..max
			}

			return (0);
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or invalid.
		/// </summary>
		public Command GetCommandFromLocation(Point point)
		{
			return (GetCommandFromId(GetCommandIdFromLocation(point)));
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void button_Command_Click(object sender, EventArgs e)
		{
			CommandRequest(ControlEx.TagToIndex(sender));
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeButtons()
		{
			this.buttons_commands = new List<Button>(PredefinedCommandSettings.MaxCommandsPerPage); // Preset the required capacity to improve memory management.
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
				bool isValid = (isDefined && this.terminalIsReadyToSend && this.commands[i].IsValid(this.rootDirectory));

				if (isDefined)
				{
					if (this.buttons_commands[i].ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
						this.buttons_commands[i].ForeColor = SystemColors.ControlText;

					if (this.buttons_commands[i].Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
						this.buttons_commands[i].Font = SystemFonts.DefaultFont;

					this.buttons_commands[i].Text = this.commands[i].Description;
					this.buttons_commands[i].Enabled = isValid;

					toolTip.SetToolTip(this.buttons_commands[i], @"Send """ + this.commands[i].SingleLineText + @"""");
				}
				else
				{
					if (this.buttons_commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						this.buttons_commands[i].ForeColor = SystemColors.GrayText;

					if (this.buttons_commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
						this.buttons_commands[i].Font = DrawingEx.DefaultFontItalic;

					this.buttons_commands[i].Text = Command.DefineCommandText;
					this.buttons_commands[i].Enabled = true;

					toolTip.SetToolTip(this.buttons_commands[i], Command.DefineCommandText);
				}
			}
			for (int i = commandCount; i < PredefinedCommandSettings.MaxCommandsPerPage; i++)
			{
				if (this.buttons_commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
					this.buttons_commands[i].ForeColor = SystemColors.GrayText;

				if (this.buttons_commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
					this.buttons_commands[i].Font = DrawingEx.DefaultFontItalic;

				this.buttons_commands[i].Text = Command.DefineCommandText;
				this.buttons_commands[i].Enabled = true;

				toolTip.SetToolTip(this.buttons_commands[i], Command.DefineCommandText);
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
