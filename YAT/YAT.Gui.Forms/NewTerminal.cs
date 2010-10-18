//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
using System.Windows.Forms;

using MKY.Net;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class NewTerminal : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		private Model.Settings.NewTerminalSettings newTerminalSettings;
		private Model.Settings.NewTerminalSettings newTerminalSettings_Form;

		private Settings.Terminal.TerminalSettingsRoot terminalSettings;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public NewTerminal(Model.Settings.NewTerminalSettings newTerminalSettings)
		{
			InitializeComponent();

			this.newTerminalSettings = newTerminalSettings;
			this.newTerminalSettings_Form = new Model.Settings.NewTerminalSettings(newTerminalSettings);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Model.Settings.NewTerminalSettings NewTerminalSettingsResult
		{
			get { return (this.newTerminalSettings); }
		}

		/// <summary></summary>
		public Settings.Terminal.TerminalSettingsRoot TerminalSettingsResult
		{
			get { return (this.terminalSettings); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void NewTerminal_Paint(object sender, PaintEventArgs e)
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

		private void terminalSelection_TerminalTypeChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.TerminalType terminalType = terminalSelection.TerminalType;
				this.newTerminalSettings_Form.TerminalType = terminalType;
				SetControls();
			}
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.IOType ioType = terminalSelection.IOType;
				this.newTerminalSettings_Form.IOType = ioType;
				SetControls();
			}
		}

		private void serialPortSelection_PortIdChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				MKY.IO.Ports.SerialPortId serialPortId = serialPortSelection.PortId;
				this.newTerminalSettings_Form.SerialPortId = serialPortId;
				SetControls();
			}
		}

		private void socketSelection_RemoteHostChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				IPHost host = socketSelection.RemoteHost;
				this.newTerminalSettings_Form.SocketRemoteHost = host;
				SetControls();
			}
		}

		private void socketSelection_RemotePortChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				int port = socketSelection.RemotePort;
				this.newTerminalSettings_Form.SocketRemotePort = port;
				SetControls();
			}
		}

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				IPNetworkInterface localInterface = socketSelection.LocalInterface;
				this.newTerminalSettings_Form.SocketLocalInterface = localInterface;
				SetControls();
			}
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				int port = socketSelection.LocalTcpPort;
				this.newTerminalSettings_Form.SocketLocalTcpPort = port;
				SetControls();
			}
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				int port = socketSelection.LocalUdpPort;
				this.newTerminalSettings_Form.SocketLocalUdpPort = port;
				SetControls();
			}
		}

		private void usbHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				MKY.IO.Usb.DeviceInfo di = usbHidDeviceSelection.DeviceInfo;
				this.newTerminalSettings_Form.UsbHidDeviceInfo = di;
				SetControls();
			}
		}

		private void checkBox_StartTerminal_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				bool start = checkBox_StartTerminal.Checked;
				this.newTerminalSettings_Form.StartTerminal = start;
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			// New terminal settings.
			this.newTerminalSettings = this.newTerminalSettings_Form;

			// Create document settings and fill it with new terminal settings.
			this.terminalSettings = new Settings.Terminal.TerminalSettingsRoot();

			this.terminalSettings.Terminal.TerminalType                      = this.newTerminalSettings.TerminalType;
			this.terminalSettings.Terminal.IO.IOType                         = this.newTerminalSettings.IOType;

			this.terminalSettings.Terminal.IO.SerialPort.PortId              = this.newTerminalSettings.SerialPortId;

			this.terminalSettings.Terminal.IO.Socket.RemoteHost              = this.newTerminalSettings.SocketRemoteHost;
			this.terminalSettings.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
			this.terminalSettings.Terminal.IO.Socket.RemotePort              = this.newTerminalSettings.SocketRemotePort;

			this.terminalSettings.Terminal.IO.Socket.LocalInterface          = this.newTerminalSettings.SocketLocalInterface;
			this.terminalSettings.Terminal.IO.Socket.ResolvedLocalIPAddress  = socketSelection.ResolvedLocalIPAddress;
			this.terminalSettings.Terminal.IO.Socket.LocalTcpPort            = this.newTerminalSettings.SocketLocalTcpPort;
			this.terminalSettings.Terminal.IO.Socket.LocalUdpPort            = this.newTerminalSettings.SocketLocalUdpPort;

			this.terminalSettings.Terminal.IO.UsbHidDevice.DeviceInfo        = this.newTerminalSettings.UsbHidDeviceInfo;

			this.terminalSettings.TerminalIsStarted                          = this.newTerminalSettings.StartTerminal;

			switch (this.terminalSettings.TerminalType)
			{
				case Domain.TerminalType.Binary:
					this.terminalSettings.Display.TxRadix = Domain.Radix.Hex;
					this.terminalSettings.Display.RxRadix = Domain.Radix.Hex;
					break;

				case Domain.TerminalType.Text:
				default:
					this.terminalSettings.Display.TxRadix = Domain.Radix.String;
					this.terminalSettings.Display.RxRadix = Domain.Radix.String;
					break;
			}
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		private void button_Help_Click(object sender, EventArgs e)
		{
			// \fixme Replace MessageBox with a real help.
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

		private Domain.IOType SetControls_ioTypeOld = Domain.IOType.SerialPort;

		private void SetControls()
		{
			this.isSettingControls = true;

			terminalSelection.TerminalType = this.newTerminalSettings_Form.TerminalType;

			Domain.IOType ioType = this.newTerminalSettings_Form.IOType;
			terminalSelection.IOType = ioType;

			bool isSerialPort = false;
			bool isUsbHid = false;
			
			bool isValid = true;

			switch (ioType)
			{
				case Domain.IOType.SerialPort:
					isSerialPort = true;
					isValid = serialPortSelection.IsValid;
					break;

				case Domain.IOType.UsbHid:
					isUsbHid = true;
					isValid = usbHidDeviceSelection.IsValid;
					break;
			}

			// Set socket control before serial port control since that might need to refresh the
			//   serial port list first (which takes time, which looks ulgy).
			socketSelection.Enabled        = !isSerialPort && !isUsbHid;
			socketSelection.HostType       = (Domain.XIOType)ioType;
			socketSelection.RemoteHost     = this.newTerminalSettings_Form.SocketRemoteHost;
			socketSelection.RemotePort     = this.newTerminalSettings_Form.SocketRemotePort;
			socketSelection.LocalInterface = this.newTerminalSettings_Form.SocketLocalInterface;
			socketSelection.LocalTcpPort   = this.newTerminalSettings_Form.SocketLocalTcpPort;
			socketSelection.LocalUdpPort   = this.newTerminalSettings_Form.SocketLocalUdpPort;

			serialPortSelection.Enabled    = isSerialPort;
			serialPortSelection.PortId     = this.newTerminalSettings_Form.SerialPortId;

			usbHidDeviceSelection.Enabled    = isUsbHid;
			usbHidDeviceSelection.DeviceInfo = this.newTerminalSettings_Form.UsbHidDeviceInfo;

			checkBox_StartTerminal.Checked = this.newTerminalSettings_Form.StartTerminal;

			button_OK.Enabled = isValid;

			// Trigger refresh of COM ports if selection of I/O type has changed
			if ((ioType == Domain.IOType.SerialPort) && (this.SetControls_ioTypeOld != Domain.IOType.SerialPort))
				serialPortSelection.RefreshSerialPortList();

			this.SetControls_ioTypeOld = ioType;

			this.isSettingControls = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
