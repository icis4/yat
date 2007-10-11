using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;

using YAT.Gui.Types;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class Send : UserControl
	{
		//------------------------------------------------------------------------------------------
		// Constants
		//------------------------------------------------------------------------------------------

		private const Domain.TerminalType _TerminalTypeDefault = Domain.TerminalType.Text;
		private const bool _TerminalIsOpenDefault = false;

		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private Domain.TerminalType _terminalType = _TerminalTypeDefault;
		private bool _terminalIsOpen = _TerminalIsOpenDefault;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler CommandChanged;

		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler SendCommandRequest;

		[Category("Property Changed")]
		[Description("Event raised when the FileCommand property is changed.")]
		public event EventHandler FileCommandChanged;

		[Category("Action")]
		[Description("Event raised when sending the file is requested.")]
		public event EventHandler SendFileCommandRequest;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public Send()
		{
			InitializeComponent();
			SetControls();
		}

		#region Methods
		//******************************************************************************************
		// Methods
		//******************************************************************************************

		public void SelectSendCommandInput()
		{
			sendCommand.Select();
			sendCommand.SelectInput();
		}

		#endregion

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Command always returns a Command object, it never returns null.
		/// </summary>
		[Browsable(false)]
		public Command FileCommand
		{
			get { return (sendFile.FileCommand); }
			set	{ sendFile.FileCommand = value;  }
		}

		[Browsable(false)]
		[DefaultValue(_TerminalTypeDefault)]
		public Domain.TerminalType TerminalType
		{
			set
			{
				_terminalType = value;
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

		#region Controls Event Handlers
		//******************************************************************************************
		// Controls Event Handlers
		//******************************************************************************************

		#region Controls Event Handlers > Send Command
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send Command
		//------------------------------------------------------------------------------------------

		private void sendCommand_CommandChanged(object sender, EventArgs e)
		{
			OnCommandChanged(e);
		}

		private void sendCommand_SendCommandRequest(object sender, EventArgs e)
		{
			OnSendCommandRequest(e);
		}

		#endregion

		#region Controls Event Handlers > Send File
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send File
		//------------------------------------------------------------------------------------------

		private void sendFile_FileCommandChanged(object sender, EventArgs e)
		{
			OnFileCommandChanged(e);
		}

		private void sendFile_SendFileCommandRequest(object sender, EventArgs e)
		{
			OnSendFileCommandRequest(e);
		}

		#endregion

		#region Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Send_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_SendContextMenu_SendCommand.Enabled = _terminalSettingsRoot.SendCommand.Command.IsValidCommand;
			toolStripMenuItem_SendContextMenu_SendFile.Enabled = _terminalSettingsRoot.SendCommand.Command.IsValidFilePath;

			toolStripMenuItem_SendContextMenu_Panels_SendCommand.Checked = _terminalSettingsRoot.Layout.SendCommandPanelIsVisible;
			toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = _terminalSettingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_SendCommand_Click(object sender, EventArgs e)
		{
			SendCommand();
		}

		private void toolStripMenuItem_SendContextMenu_SendFile_Click(object sender, EventArgs e)
		{
			SendFile();
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendCommand_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.SendCommandPanelIsVisible = !_terminalSettingsRoot.Layout.SendCommandPanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendFile_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.SendFilePanelIsVisible = !_terminalSettingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Hide_Click(object sender, EventArgs e)
		{
			Control source = contextMenuStrip_Send.SourceControl;

			if (source == sendCommand)
				_terminalSettingsRoot.Layout.SendCommandPanelIsVisible = false;
			if (source == sendFile)
				_terminalSettingsRoot.Layout.SendFilePanelIsVisible = false;
		}

		#endregion

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			sendCommand.TerminalIsOpen = _terminalIsOpen;

			sendFile.TerminalType = _terminalType;
			sendFile.TerminalIsOpen = _terminalIsOpen;
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(CommandChanged, this, e);
		}

		protected virtual void OnSendCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendCommandRequest, this, e);
		}

		protected virtual void OnFileCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(FileCommandChanged, this, e);
		}

		protected virtual void OnSendFileCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendFileCommandRequest, this, e);
		}

		#endregion
	}
}
