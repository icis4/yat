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
			this.newTerminalSettings_Form.SerialPortId = serialPortSelection.PortId;
		}

		private void socketSelection_RemoteHostChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SocketRemoteHost = socketSelection.RemoteHost;
		}

		private void socketSelection_RemotePortChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SocketRemotePort = socketSelection.RemotePort;
		}

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SocketLocalInterface = socketSelection.LocalInterface;
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SocketLocalTcpPort = socketSelection.LocalTcpPort;
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SocketLocalUdpPort = socketSelection.LocalUdpPort;
		}

		private void usbSerialHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.UsbSerialHidDeviceInfo = usbSerialHidDeviceSelection.DeviceInfo;
		}

		private void checkBox_StartTerminal_CheckedChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.StartTerminal = checkBox_StartTerminal.Checked;
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

			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.DeviceInfo        = this.newTerminalSettings.UsbSerialHidDeviceInfo;

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

		/// <remarks>
		/// This functionality is partly duplicated in <see cref="TerminalSettings.SetControls"/>.
		/// Changes here must also be applied there.
		/// </remarks>
		private void SetControls()
		{
			this.isSettingControls = true;

			terminalSelection.TerminalType = this.newTerminalSettings_Form.TerminalType;

			Domain.IOType ioType = this.newTerminalSettings_Form.IOType;
			terminalSelection.IOType = ioType;

			bool isSerialPort   = (ioType == Domain.IOType.SerialPort);
			bool isUsbSerialHid = (ioType == Domain.IOType.UsbSerialHid);

			// Set socket control before serial port control since that might need to refresh the
			//   serial port list first (which takes time, which looks ulgy).
			socketSelection.Enabled        = !isSerialPort && !isUsbSerialHid;
			socketSelection.HostType       = (Domain.IOTypeEx)ioType;
			socketSelection.RemoteHost     = this.newTerminalSettings_Form.SocketRemoteHost;
			socketSelection.RemotePort     = this.newTerminalSettings_Form.SocketRemotePort;
			socketSelection.LocalInterface = this.newTerminalSettings_Form.SocketLocalInterface;
			socketSelection.LocalTcpPort   = this.newTerminalSettings_Form.SocketLocalTcpPort;
			socketSelection.LocalUdpPort   = this.newTerminalSettings_Form.SocketLocalUdpPort;

			serialPortSelection.Enabled    = isSerialPort;
			serialPortSelection.PortId     = this.newTerminalSettings_Form.SerialPortId;

			usbSerialHidDeviceSelection.Enabled    = isUsbSerialHid;
			usbSerialHidDeviceSelection.DeviceInfo = this.newTerminalSettings_Form.UsbSerialHidDeviceInfo;

			checkBox_StartTerminal.Checked = this.newTerminalSettings_Form.StartTerminal;

			// Trigger refresh of ports/devices if selection of I/O type has changed.
			if      ((ioType == Domain.IOType.SerialPort)   && (this.SetControls_ioTypeOld != Domain.IOType.SerialPort))
				serialPortSelection.RefreshSerialPortList();
			else if ((ioType == Domain.IOType.UsbSerialHid) && (this.SetControls_ioTypeOld != Domain.IOType.UsbSerialHid))
				usbSerialHidDeviceSelection.RefreshDeviceList();

			this.SetControls_ioTypeOld = ioType;

			// Finally, enable OK button if port/device is valid.
			bool isValid = true;
			switch (ioType)
			{
				case Domain.IOType.SerialPort:   isValid = serialPortSelection.IsValid;         break;
				case Domain.IOType.UsbSerialHid: isValid = usbSerialHidDeviceSelection.IsValid; break;
			}
			button_OK.Enabled = isValid;

			this.isSettingControls = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
