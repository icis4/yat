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

		private const int SubpageDefault = 1;
		private const bool ShowSeparatorLineDefault = false;

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

		private int subpage;
		private bool showSeparatorLine;
		private List<Command> commands;

		private Domain.Parser.Modes parseModeForText = ParseModeForTextDefault;
		private string rootDirectoryForFile; // = null;
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
		[DefaultValue(SubpageDefault)]
		public virtual int Subpage
		{
			get { return (this.subpage); }
			set
			{
				this.subpage = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[Category("Appearance")]
		[Description("An optional separator line.")]
		[DefaultValue(ShowSeparatorLineDefault)]
		public virtual bool ShowSeparatorLine
		{
			get { return (this.showSeparatorLine); }
			set
			{
				this.showSeparatorLine = value;
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
				SetCommandControls();
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
				SetCommandControls();
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
				SetCommandControls();
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
		public virtual int GetCommandIdFromLocation(Point point)
		{
			Point requested = PointToClient(point); // Using Control.PointToClient() is OK since buttons
			                                        // are directly placed onto control (no group box).
			// Ensure that location is within control:
			if ((requested.X < 0) || (requested.X > Width))  return (0);
			if ((requested.Y < 0) || (requested.Y > Height)) return (0);

			// Find the corresponding button:
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
		public virtual Command GetCommandFromLocation(Point point)
		{
			return (GetCommandFromId(GetCommandIdFromLocation(point)));
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
		private void PredefinedCommandPageButtons_Paint(object sender, PaintEventArgs e)
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
			SuspendLayout();
		////this.isSettingControls.Enter(); is not needed (yet).
			try
			{
				label_SeparatorLine.Visible = this.showSeparatorLine;

				label_Shortcuts_1_12 .Visible = (this.subpage == 1);
				label_Shortcuts_13_24.Visible = (this.subpage == 2);

				SetCommandControls();
			}
			finally
			{
			////this.isSettingControls.Leave(); is not needed (yet).
				ResumeLayout();
			}
		}

		private void SetCommandControls()
		{
			SuspendLayout();
		////this.isSettingControls.Enter(); is not needed (yet).
			try
			{
				int commandCount = 0;
				if (this.commands != null)
					commandCount = this.commands.Count;

				for (int i = 0; i < commandCount; i++)
				{
					bool isDefined = ((this.commands[i] != null) && this.commands[i].IsDefined);
					bool isValid = (isDefined && this.terminalIsReadyToSend && this.commands[i].IsValid(this.parseModeForText, this.rootDirectoryForFile));

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

				for (int i = commandCount; i < PredefinedCommandPage.CommandCapacityPerSubpage; i++)
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
			finally
			{
			////this.isSettingControls.Leave(); is not needed (yet).
				ResumeLayout();
			}
		}

		private void CommandRequest(int commandId)
		{
			bool isDefined =
			(
				(this.commands != null) &&
				(this.commands.Count >= commandId) &&
				(this.commands[commandId - 1] != null) &&
				(this.commands[commandId - 1].IsDefined)
			);

			if (isDefined)
				RequestSendCommand(commandId);
			else
				RequestDefineCommand(commandId);
		}

		private void RequestSendCommand(int commandId)
		{
			OnSendCommandRequest(new PredefinedCommandEventArgs(commandId));
		}

		private void RequestDefineCommand(int commandId)
		{
			OnDefineCommandRequest(new PredefinedCommandEventArgs(commandId));
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
