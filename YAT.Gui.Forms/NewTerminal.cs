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
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Net;

namespace YAT.Gui.Forms
{
	public partial class NewTerminal : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private Model.Settings.NewTerminalSettings _newTerminalSettings;
		private Model.Settings.NewTerminalSettings _newTerminalSettings_Form;

		private Settings.Terminal.TerminalSettingsRoot _terminalSettings;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public NewTerminal(Model.Settings.NewTerminalSettings newTerminalSettings)
		{
			InitializeComponent();

			_newTerminalSettings = newTerminalSettings;
			_newTerminalSettings_Form = new Model.Settings.NewTerminalSettings(newTerminalSettings);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Model.Settings.NewTerminalSettings NewTerminalSettingsResult
		{
			get { return (_newTerminalSettings); }
		}

		public Settings.Terminal.TerminalSettingsRoot TerminalSettingsResult
		{
			get { return (_terminalSettings); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool _isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void NewTerminal_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;
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

		private void socketSelection_RemoteHostChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				XIPHost host = socketSelection.RemoteHost;
				_newTerminalSettings_Form.SocketRemoteHost = host;
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

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				XNetworkInterface localInterface = socketSelection.LocalInterface;
				_newTerminalSettings_Form.SocketLocalInterface = localInterface;
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

		private void usbHidPortSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				MKY.IO.Usb.DeviceInfo di = usbHidPortSelection.DeviceInfo;
				_newTerminalSettings_Form.UsbHidDeviceInfo = di;
				SetControls();
			}
		}

		private void checkBox_StartTerminal_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				bool start = checkBox_StartTerminal.Checked;
				_newTerminalSettings_Form.StartTerminal = start;
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			// New terminal settings
			_newTerminalSettings = _newTerminalSettings_Form;

			// Create document settings and fill it with new terminal settings
			_terminalSettings = new Settings.Terminal.TerminalSettingsRoot();

			_terminalSettings.Terminal.TerminalType                      = _newTerminalSettings.TerminalType;
			_terminalSettings.Terminal.IO.IOType                         = _newTerminalSettings.IOType;

			_terminalSettings.Terminal.IO.SerialPort.PortId              = _newTerminalSettings.SerialPortId;

			_terminalSettings.Terminal.IO.Socket.RemoteHost              = _newTerminalSettings.SocketRemoteHost;
			_terminalSettings.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
			_terminalSettings.Terminal.IO.Socket.RemotePort              = _newTerminalSettings.SocketRemotePort;

			_terminalSettings.Terminal.IO.Socket.LocalInterface          = _newTerminalSettings.SocketLocalInterface;
			_terminalSettings.Terminal.IO.Socket.ResolvedLocalIPAddress  = socketSelection.ResolvedLocalIPAddress;
			_terminalSettings.Terminal.IO.Socket.LocalTcpPort            = _newTerminalSettings.SocketLocalTcpPort;
			_terminalSettings.Terminal.IO.Socket.LocalUdpPort            = _newTerminalSettings.SocketLocalUdpPort;

			_terminalSettings.Terminal.IO.UsbHidDevice.DeviceInfo          = _newTerminalSettings.UsbHidDeviceInfo;

			_terminalSettings.TerminalIsStarted                          = _newTerminalSettings.StartTerminal;

			switch (_newTerminalSettings.TerminalType)
			{
				case Domain.TerminalType.Binary:
					_terminalSettings.Display.TxRadix = Domain.Radix.Hex;
					_terminalSettings.Display.RxRadix = Domain.Radix.Hex;
					break;

				case Domain.TerminalType.Text:
				default:
					_terminalSettings.Display.TxRadix = Domain.Radix.String;
					_terminalSettings.Display.RxRadix = Domain.Radix.String;
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
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			_isSettingControls = true;

			terminalSelection.TerminalType = _newTerminalSettings_Form.TerminalType;

			Domain.IOType ioType = _newTerminalSettings_Form.IOType;
			terminalSelection.IOType = ioType;

			bool isSerialPort = (ioType == Domain.IOType.SerialPort);
			bool isUsbHid     = (ioType == Domain.IOType.UsbHid);

			// Set socket control before serial port control since that might need to refresh the
			//   serial port list first (which takes time, which looks ulgy)
			socketSelection.Enabled        = !isSerialPort && !isUsbHid;
			socketSelection.HostType       = (Domain.XIOType)ioType;
			socketSelection.RemoteHost     = _newTerminalSettings_Form.SocketRemoteHost;
			socketSelection.RemotePort     = _newTerminalSettings_Form.SocketRemotePort;
			socketSelection.LocalInterface = _newTerminalSettings_Form.SocketLocalInterface;
			socketSelection.LocalTcpPort   = _newTerminalSettings_Form.SocketLocalTcpPort;
			socketSelection.LocalUdpPort   = _newTerminalSettings_Form.SocketLocalUdpPort;

			serialPortSelection.Enabled    = isSerialPort;
			serialPortSelection.PortId     = _newTerminalSettings_Form.SerialPortId;

			usbHidPortSelection.Enabled    = isUsbHid;
			usbHidPortSelection.DeviceInfo = _newTerminalSettings_Form.UsbHidDeviceInfo;

			checkBox_StartTerminal.Checked = _newTerminalSettings_Form.StartTerminal;

			_isSettingControls = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
