using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HSR.YAT.Gui.Forms
{
	public partial class NewTerminal : System.Windows.Forms.Form
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Settings.NewTerminalSettings _newTerminalSettings;
		private Settings.NewTerminalSettings _newTerminalSettings_Form;

		private YAT.Settings.Document.DocumentSettings _documentSettings;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public NewTerminal(Settings.NewTerminalSettings newTerminalSettings)
		{
			InitializeComponent();

			_newTerminalSettings = newTerminalSettings;
			_newTerminalSettings_Form = new Settings.NewTerminalSettings(newTerminalSettings);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public Settings.NewTerminalSettings NewTerminalSettingsResult
		{
			get { return (_newTerminalSettings); }
		}

		public YAT.Settings.Document.DocumentSettings DocumentSettingsResult
		{
			get { return (_documentSettings); }
		}

		#endregion

		#region Form Event Handlers
		//------------------------------------------------------------------------------------------
		// Form Event Handlers
		//------------------------------------------------------------------------------------------

		private void NewTerminal_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// initially set controls and validate its contents where needed
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void terminalSelection_TerminalTypeChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.TerminalType terminalType = terminalSelection.TerminalType;
				_newTerminalSettings_Form.TerminalType = terminalType;
				SetControls();
			}
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.IOType ioType = terminalSelection.IOType;
				_newTerminalSettings_Form.IOType = ioType;
				SetControls();
			}
		}

		private void serialPortSelection_PortIdChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				IO.Ports.SerialPortId serialPortId = serialPortSelection.PortId;
				_newTerminalSettings_Form.SerialPortId = serialPortId;
				SetControls();
			}
		}

		private void socketSelection_RemoteHostNameOrAddressChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				string nameOrAddress = socketSelection.RemoteHostNameOrAddress;
				_newTerminalSettings_Form.SocketRemoteHostNameOrAddress = nameOrAddress;
				SetControls();
			}
		}

		private void socketSelection_RemotePortChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				int port = socketSelection.RemotePort;
				_newTerminalSettings_Form.SocketRemotePort = port;
				SetControls();
			}
		}

		private void socketSelection_LocalHostNameOrAddressChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				string nameOrAddress = socketSelection.LocalHostNameOrAddress;
				_newTerminalSettings_Form.SocketLocalHostNameOrAddress = nameOrAddress;
				SetControls();
			}
		}

		private void socketSelection_LocalPortChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				int port = socketSelection.LocalPort;
				_newTerminalSettings_Form.SocketLocalPort = port;
				SetControls();
			}
		}

		private void checkBox_OpenTerminal_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				bool open = checkBox_OpenTerminal.Checked;
				_newTerminalSettings_Form.OpenTerminal = open;
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			// new terminal settings
			_newTerminalSettings = _newTerminalSettings_Form;

			// create document settings and fill it with new terminal settings
			_documentSettings = new YAT.Settings.Document.DocumentSettings();

			_documentSettings.Terminal.TerminalType = _newTerminalSettings.TerminalType;
			_documentSettings.Terminal.IO.IOType = _newTerminalSettings.IOType;

			_documentSettings.Terminal.IO.SerialPort.PortId = _newTerminalSettings.SerialPortId;

			_documentSettings.Terminal.IO.Socket.RemoteHostNameOrAddress = _newTerminalSettings.SocketRemoteHostNameOrAddress;
			_documentSettings.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
			_documentSettings.Terminal.IO.Socket.RemotePort = _newTerminalSettings.SocketRemotePort;

			_documentSettings.Terminal.IO.Socket.LocalHostNameOrAddress = _newTerminalSettings.SocketLocalHostNameOrAddress;
			_documentSettings.Terminal.IO.Socket.ResolvedLocalIPAddress = socketSelection.ResolvedLocalIPAddress;
			_documentSettings.Terminal.IO.Socket.LocalTcpPort = _newTerminalSettings.SocketLocalTcpPort;
			_documentSettings.Terminal.IO.Socket.LocalUdpPort = _newTerminalSettings.SocketLocalUdpPort;

			_documentSettings.TerminalIsOpen = _newTerminalSettings.OpenTerminal;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Help_Click(object sender, EventArgs e)
		{
			// \fixme Replace MessageBox with a real help
			MessageBox.Show
				(
				this,
				YAT.Gui.Controls.TerminalSelection.NewTerminalHelpText,
				"New Terminal Help",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
				);
		}

        #endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			_isSettingControls = true;

			terminalSelection.TerminalType = _newTerminalSettings_Form.TerminalType;

			Domain.IOType ioType = _newTerminalSettings_Form.IOType;
			terminalSelection.IOType = ioType;

			serialPortSelection.Enabled = (ioType == Domain.IOType.SerialPort);
			serialPortSelection.ShowSerialPort = (ioType == Domain.IOType.SerialPort);
			serialPortSelection.PortId = _newTerminalSettings_Form.SerialPortId;

			socketSelection.Enabled = (ioType != Domain.IOType.SerialPort);
			socketSelection.HostType = (Domain.XIOType)ioType;
			socketSelection.RemoteHostNameOrAddress = _newTerminalSettings_Form.SocketRemoteHostNameOrAddress;
			socketSelection.RemotePort = _newTerminalSettings_Form.SocketRemotePort;
			socketSelection.LocalHostNameOrAddress = _newTerminalSettings_Form.SocketLocalHostNameOrAddress;
			socketSelection.LocalPort = _newTerminalSettings_Form.SocketLocalPort;

			checkBox_OpenTerminal.Checked = _newTerminalSettings_Form.OpenTerminal;

			_isSettingControls = false;
		}

		#endregion
	}
}
