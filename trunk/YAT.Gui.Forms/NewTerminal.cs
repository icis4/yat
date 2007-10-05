using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	public partial class NewTerminal : System.Windows.Forms.Form
	{
		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Settings.NewTerminalSettings _newTerminalSettings;
		private Settings.NewTerminalSettings _newTerminalSettings_Form;

		private YAT.Settings.Terminal.TerminalSettingsRoot _terminalSettings;

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

		public YAT.Settings.Terminal.TerminalSettingsRoot TerminalSettingsResult
		{
			get { return (_terminalSettings); }
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
				MKY.IO.Ports.SerialPortId serialPortId = serialPortSelection.PortId;
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

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				int port = socketSelection.LocalTcpPort;
				_newTerminalSettings_Form.SocketLocalTcpPort = port;
				SetControls();
			}
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				int port = socketSelection.LocalUdpPort;
				_newTerminalSettings_Form.SocketLocalUdpPort = port;
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
			_terminalSettings = new YAT.Settings.Terminal.TerminalSettingsRoot();

			_terminalSettings.Terminal.TerminalType = _newTerminalSettings.TerminalType;
			_terminalSettings.Terminal.IO.IOType = _newTerminalSettings.IOType;

			_terminalSettings.Terminal.IO.SerialPort.PortId = _newTerminalSettings.SerialPortId;

			_terminalSettings.Terminal.IO.Socket.RemoteHostNameOrAddress = _newTerminalSettings.SocketRemoteHostNameOrAddress;
			_terminalSettings.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
			_terminalSettings.Terminal.IO.Socket.RemotePort = _newTerminalSettings.SocketRemotePort;

			_terminalSettings.Terminal.IO.Socket.LocalHostNameOrAddress = _newTerminalSettings.SocketLocalHostNameOrAddress;
			_terminalSettings.Terminal.IO.Socket.ResolvedLocalIPAddress = socketSelection.ResolvedLocalIPAddress;
			_terminalSettings.Terminal.IO.Socket.LocalTcpPort = _newTerminalSettings.SocketLocalTcpPort;
			_terminalSettings.Terminal.IO.Socket.LocalUdpPort = _newTerminalSettings.SocketLocalUdpPort;

			_terminalSettings.TerminalIsOpen = _newTerminalSettings.OpenTerminal;

			switch (_newTerminalSettings.TerminalType)
			{
				case Domain.TerminalType.Binary:
					_terminalSettings.Display.Radix = Domain.Radix.Hex;
					break;

				case Domain.TerminalType.Text:
				default:
					_terminalSettings.Display.Radix = Domain.Radix.String;
					break;
			}
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

			bool isSerialPort = (ioType == Domain.IOType.SerialPort);

			serialPortSelection.Enabled = isSerialPort;
			serialPortSelection.ShowSerialPort = isSerialPort;
			serialPortSelection.PortId = _newTerminalSettings_Form.SerialPortId;

			socketSelection.Enabled = !isSerialPort;
			socketSelection.HostType = (Domain.XIOType)ioType;
			socketSelection.RemoteHostNameOrAddress = _newTerminalSettings_Form.SocketRemoteHostNameOrAddress;
			socketSelection.RemotePort = _newTerminalSettings_Form.SocketRemotePort;
			socketSelection.LocalHostNameOrAddress = _newTerminalSettings_Form.SocketLocalHostNameOrAddress;
			socketSelection.LocalTcpPort = _newTerminalSettings_Form.SocketLocalTcpPort;
			socketSelection.LocalUdpPort = _newTerminalSettings_Form.SocketLocalUdpPort;

			checkBox_OpenTerminal.Checked = _newTerminalSettings_Form.OpenTerminal;

			_isSettingControls = false;
		}

		#endregion
	}
}
