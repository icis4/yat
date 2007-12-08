using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	public partial class TerminalSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Domain.Settings.TerminalSettings _settings;
		private Domain.Settings.TerminalSettings _settings_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public TerminalSettings(Domain.Settings.TerminalSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Domain.Settings.TerminalSettings(settings);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Domain.Settings.TerminalSettings SettingsResult
		{
			get { return (_settings); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		private void TerminalSettings_Paint(object sender, PaintEventArgs e)
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
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void terminalSelection_TerminalTypeChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.TerminalType = terminalSelection.TerminalType;
				SetControls();
			}
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.IOType ioType = terminalSelection.IOType;
				_settings_Form.IO.IOType = ioType;
				_settings_Form.IO.Socket.HostType = (Domain.XIOType)ioType;
				SetControls();
			}
		}

		private void button_TextOrBinarySettings_Click(object sender, EventArgs e)
		{
			ShowTextOrBinarySettings();
		}

		private void serialPortSelection_PortIdChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.SerialPort.PortId = serialPortSelection.PortId;
		}

		private void serialPortSettings_BaudRateChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.SerialPort.Communication.BaudRate = serialPortSettings.BaudRate;
		}

		private void serialPortSettings_DataBitsChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.SerialPort.Communication.DataBits = serialPortSettings.DataBits;
		}

		private void serialPortSettings_ParityChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.SerialPort.Communication.Parity = serialPortSettings.Parity;
		}

		private void serialPortSettings_StopBitsChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.SerialPort.Communication.StopBits = serialPortSettings.StopBits;
		}

		private void serialPortSettings_HandshakeChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.SerialPort.Communication.Handshake = serialPortSettings.Handshake;
		}

		private void socketSelection_RemoteHostNameOrAddressChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.IO.Socket.RemoteHostNameOrAddress = socketSelection.RemoteHostNameOrAddress;
				_settings_Form.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
			}
		}

		private void socketSelection_RemotePortChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.Socket.RemotePort = socketSelection.RemotePort;
		}

		private void socketSelection_LocalHostNameOrAddressChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.IO.Socket.LocalHostNameOrAddress = socketSelection.LocalHostNameOrAddress;
				_settings_Form.IO.Socket.ResolvedLocalIPAddress = socketSelection.ResolvedLocalIPAddress;
			}
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.Socket.LocalTcpPort = socketSelection.LocalTcpPort;
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.Socket.LocalUdpPort = socketSelection.LocalUdpPort;
		}

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings_Form.IO.Socket.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		private void button_AdvancedSettings_Click(object sender, EventArgs e)
		{
			ShowAdvancedSettings();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			_settings = _settings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			string text;
			switch (_settings_Form.TerminalType)
			{
				case Domain.TerminalType.Text:   text = "Text";   break;
				case Domain.TerminalType.Binary: text = "Binary"; break;
				default: throw (new NotImplementedException("Unknown terminal type"));
			}
			text += " and extended";

			if (MessageBox.Show
				(
				this,
				"Reset all settings to default values?" + Environment.NewLine +
				text + " settings will also be reset!",
				"Defaults?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				_settings_Form.SetDefaults();
				SetControls();
			}
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
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			_isSettingControls = true;

			terminalSelection.TerminalType = _settings_Form.TerminalType;

			Domain.IOType ioType = _settings_Form.IO.IOType;
			terminalSelection.IOType = ioType;

			string text = "&";
			switch (_settings_Form.TerminalType)
			{
				case Domain.TerminalType.Text:   text += "Text";   break;
				case Domain.TerminalType.Binary: text += "Binary"; break;
				default: throw (new NotImplementedException("Unknown terminal type"));
			}
			text += " Settings...";
			button_TextOrBinarySettings.Text = text;

			bool isSerialPort = (ioType == Domain.IOType.SerialPort);

			serialPortSelection.Visible  = isSerialPort;
			serialPortSelection.PortId   = _settings_Form.IO.SerialPort.PortId;

			serialPortSettings.Visible   = isSerialPort;
			serialPortSettings.BaudRate  = _settings_Form.IO.SerialPort.Communication.BaudRate;
			serialPortSettings.DataBits  = _settings_Form.IO.SerialPort.Communication.DataBits;
			serialPortSettings.Parity    = _settings_Form.IO.SerialPort.Communication.Parity;
			serialPortSettings.StopBits  = _settings_Form.IO.SerialPort.Communication.StopBits;
			serialPortSettings.Handshake = _settings_Form.IO.SerialPort.Communication.Handshake;

			socketSelection.Visible = !isSerialPort;
			socketSelection.HostType = (Domain.XIOType)ioType;
			socketSelection.RemoteHostNameOrAddress = _settings_Form.IO.Socket.RemoteHostNameOrAddress;
			socketSelection.RemotePort = _settings_Form.IO.Socket.RemotePort;
			socketSelection.LocalHostNameOrAddress = _settings_Form.IO.Socket.LocalHostNameOrAddress;
			socketSelection.LocalTcpPort = _settings_Form.IO.Socket.LocalTcpPort;
			socketSelection.LocalUdpPort = _settings_Form.IO.Socket.LocalUdpPort;

			socketSettings.Visible = !isSerialPort;
			socketSettings.HostType = (Domain.XIOType)ioType;
			socketSettings.TcpClientAutoReconnect = _settings_Form.IO.Socket.TcpClientAutoReconnect;

			_isSettingControls = false;
		}

		private void ShowTextOrBinarySettings()
		{
			switch (_settings_Form.TerminalType)
			{
				case Domain.TerminalType.Text:
				{
					Gui.Forms.TextTerminalSettings f = new Gui.Forms.TextTerminalSettings(_settings_Form.TextTerminal);
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						Refresh();
						_settings_Form.TextTerminal = f.SettingsResult;
					}
					break;
				}
				case Domain.TerminalType.Binary:
				{
					Gui.Forms.BinaryTerminalSettings f = new Gui.Forms.BinaryTerminalSettings(_settings_Form.BinaryTerminal);
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						Refresh();
						_settings_Form.BinaryTerminal = f.SettingsResult;
					}
					break;
				}
				default:
				{
					throw (new NotImplementedException("Unknown terminal type"));
				}
			}
		}

		private void ShowAdvancedSettings()
		{
			Gui.Forms.AdvancedTerminalSettings f = new Gui.Forms.AdvancedTerminalSettings(_settings_Form);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				_settings_Form.Display.Radix = f.SettingsResult.Display.Radix;
				_settings_Form.Display.ShowTimeStamp = f.SettingsResult.Display.ShowTimeStamp;
				_settings_Form.Display.ShowLength = f.SettingsResult.Display.ShowLength;
				_settings_Form.Display.ShowCounters = f.SettingsResult.Display.ShowCounters;

				_settings_Form.Display.TxMaximalLineCount = f.SettingsResult.Display.TxMaximalLineCount;
				_settings_Form.Display.RxMaximalLineCount = f.SettingsResult.Display.RxMaximalLineCount;

				_settings_Form.IO.Endianess = f.SettingsResult.IO.Endianess;
				_settings_Form.Transmit.LocalEchoEnabled = f.SettingsResult.Transmit.LocalEchoEnabled;

				_settings_Form.IO.SerialPort.ParityErrorReplacement = f.SettingsResult.IO.SerialPort.ParityErrorReplacement;
			}
		}

		#endregion
	}
}
