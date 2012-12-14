//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

using MKY.Net;
using MKY.Windows.Forms;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class NewTerminal : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

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

			// SetControls() is initially called in the 'Paint' event handler.
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
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
		/// </remarks>
		private void NewTerminal_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		#region Controls Event Handlers > General
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > General
		//------------------------------------------------------------------------------------------

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

		private void checkBox_StartTerminal_CheckedChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.StartTerminal = checkBox_StartTerminal.Checked;
		}

		#endregion

		#region Controls Event Handlers > Serial Port
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Serial Port
		//------------------------------------------------------------------------------------------

		private void serialPortSelection_PortIdChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SerialPortId = serialPortSelection.PortId;
		}

		private void serialPortSettings_BaudRateChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SerialPortCommunication.BaudRate = serialPortSettings.BaudRate;
		}

		private void serialPortSettings_DataBitsChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SerialPortCommunication.DataBits = serialPortSettings.DataBits;
		}

		private void serialPortSettings_ParityChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SerialPortCommunication.Parity = serialPortSettings.Parity;
		}

		private void serialPortSettings_StopBitsChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SerialPortCommunication.StopBits = serialPortSettings.StopBits;
		}

		private void serialPortSettings_FlowControlChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SerialPortCommunication.FlowControl = serialPortSettings.FlowControl;
		}

		private void serialPortSettings_AutoReopenChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.SerialPortAutoReopen = serialPortSettings.AutoReopen;
		}

		#endregion

		#region Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------

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

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		#endregion

		#region Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------

		private void usbSerialHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.UsbSerialHidDeviceInfo = usbSerialHidDeviceSelection.DeviceInfo;
		}

		private void usbSerialHidDeviceSettings_AutoOpenChanged(object sender, EventArgs e)
		{
			this.newTerminalSettings_Form.UsbSerialHidAutoOpen = usbSerialHidDeviceSettings.AutoOpen;
		}

		#endregion

		#region Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------

		private void button_OK_Click(object sender, EventArgs e)
		{
			// New terminal settings.
			this.newTerminalSettings = this.newTerminalSettings_Form;

			// Create document settings and fill it with new terminal settings.
			this.terminalSettings = new Settings.Terminal.TerminalSettingsRoot();

			this.terminalSettings.Terminal.TerminalType = this.newTerminalSettings.TerminalType;
			this.terminalSettings.Terminal.IO.IOType    = this.newTerminalSettings.IOType;

			this.terminalSettings.Terminal.IO.SerialPort.PortId        = this.newTerminalSettings.SerialPortId;
			this.terminalSettings.Terminal.IO.SerialPort.Communication = this.newTerminalSettings.SerialPortCommunication;
			this.terminalSettings.Terminal.IO.SerialPort.AutoReopen    = this.newTerminalSettings.SerialPortAutoReopen;

			this.terminalSettings.Terminal.IO.Socket.RemoteHost              = this.newTerminalSettings.SocketRemoteHost;
			this.terminalSettings.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
			this.terminalSettings.Terminal.IO.Socket.RemotePort              = this.newTerminalSettings.SocketRemotePort;
			this.terminalSettings.Terminal.IO.Socket.LocalInterface          = this.newTerminalSettings.SocketLocalInterface;
			this.terminalSettings.Terminal.IO.Socket.ResolvedLocalIPAddress  = socketSelection.ResolvedLocalIPAddress;
			this.terminalSettings.Terminal.IO.Socket.LocalTcpPort            = this.newTerminalSettings.SocketLocalTcpPort;
			this.terminalSettings.Terminal.IO.Socket.LocalUdpPort            = this.newTerminalSettings.SocketLocalUdpPort;
			this.terminalSettings.Terminal.IO.Socket.TcpClientAutoReconnect  = this.newTerminalSettings.TcpClientAutoReconnect;

			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.DeviceInfo = this.newTerminalSettings.UsbSerialHidDeviceInfo;
			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.AutoOpen   = this.newTerminalSettings.UsbSerialHidAutoOpen;

			this.terminalSettings.TerminalIsStarted = this.newTerminalSettings.StartTerminal;

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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show
				(
				this,
				"Reset all settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.newTerminalSettings_Form.SetDefaults();
				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Help_Click(object sender, EventArgs e)
		{
			// \fixme: Replace MessageBox with a real help.
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
			this.isSettingControls.Enter();

			terminalSelection.TerminalType = this.newTerminalSettings_Form.TerminalType;

			Domain.IOType ioType = this.newTerminalSettings_Form.IOType;
			terminalSelection.IOType = ioType;

			bool isSerialPort   = (ioType == Domain.IOType.SerialPort);
			bool isUsbSerialHid = (ioType == Domain.IOType.UsbSerialHid);
			bool isSocket       = (!isSerialPort && !isUsbSerialHid);

			// Set socket and USB control before serial port control since that might need to refresh
			// the serial port list first (which takes time, which looks ulgy).
			socketSelection.Visible        = isSocket;
			socketSelection.HostType       = (Domain.IOTypeEx)ioType;
			socketSelection.RemoteHost     = this.newTerminalSettings_Form.SocketRemoteHost;
			socketSelection.RemotePort     = this.newTerminalSettings_Form.SocketRemotePort;
			socketSelection.LocalInterface = this.newTerminalSettings_Form.SocketLocalInterface;
			socketSelection.LocalTcpPort   = this.newTerminalSettings_Form.SocketLocalTcpPort;
			socketSelection.LocalUdpPort   = this.newTerminalSettings_Form.SocketLocalUdpPort;

			socketSettings.Visible                 = isSocket;
			socketSettings.HostType                = (Domain.IOTypeEx)ioType;
			socketSettings.TcpClientAutoReconnect  = this.newTerminalSettings_Form.TcpClientAutoReconnect;

			usbSerialHidDeviceSelection.Visible    = isUsbSerialHid;
			usbSerialHidDeviceSelection.DeviceInfo = this.newTerminalSettings_Form.UsbSerialHidDeviceInfo;

			usbSerialHidDeviceSettings.Visible     = isUsbSerialHid;
			usbSerialHidDeviceSettings.AutoOpen    = this.newTerminalSettings_Form.UsbSerialHidAutoOpen;

			serialPortSelection.Visible    = isSerialPort;
			serialPortSelection.PortId     = this.newTerminalSettings_Form.SerialPortId;

			serialPortSettings.Visible     = isSerialPort;
			serialPortSettings.BaudRate    = this.newTerminalSettings_Form.SerialPortCommunication.BaudRate;
			serialPortSettings.DataBits    = this.newTerminalSettings_Form.SerialPortCommunication.DataBits;
			serialPortSettings.Parity      = this.newTerminalSettings_Form.SerialPortCommunication.Parity;
			serialPortSettings.StopBits    = this.newTerminalSettings_Form.SerialPortCommunication.StopBits;
			serialPortSettings.FlowControl = this.newTerminalSettings_Form.SerialPortCommunication.FlowControl;
			serialPortSettings.AutoReopen  = this.newTerminalSettings_Form.SerialPortAutoReopen;

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

			this.isSettingControls.Leave();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
