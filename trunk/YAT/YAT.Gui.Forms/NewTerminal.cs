﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

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
		private Model.Settings.NewTerminalSettings newTerminalSettingsInEdit;

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
			this.newTerminalSettingsInEdit = new Model.Settings.NewTerminalSettings(newTerminalSettings);

			// SetControls() is initially called in the 'Shown' event handler.
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
				this.newTerminalSettingsInEdit.TerminalType = terminalType;
				SetControls();
			}
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.IOType ioType = terminalSelection.IOType;
				this.newTerminalSettingsInEdit.IOType = ioType;
				SetControls();
			}
		}

		private void checkBox_StartTerminal_CheckedChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.StartTerminal = checkBox_StartTerminal.Checked;
		}

		#endregion

		#region Controls Event Handlers > Serial Port
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Serial Port
		//------------------------------------------------------------------------------------------

		private void serialPortSelection_PortIdChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SerialPortId = serialPortSelection.PortId;
		}

		private void serialPortSettings_BaudRateChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SerialPortCommunication.BaudRate = serialPortSettings.BaudRate;
		}

		private void serialPortSettings_DataBitsChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SerialPortCommunication.DataBits = serialPortSettings.DataBits;
		}

		private void serialPortSettings_ParityChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SerialPortCommunication.Parity = serialPortSettings.Parity;
		}

		private void serialPortSettings_StopBitsChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SerialPortCommunication.StopBits = serialPortSettings.StopBits;
		}

		private void serialPortSettings_FlowControlChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SerialPortCommunication.FlowControl = serialPortSettings.FlowControl;
		}

		private void serialPortSettings_AutoReopenChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SerialPortAutoReopen = serialPortSettings.AutoReopen;
		}

		#endregion

		#region Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------

		private void socketSelection_RemoteHostChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SocketRemoteHost = socketSelection.RemoteHost;
		}

		private void socketSelection_RemoteTcpPortChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SocketRemoteTcpPort = socketSelection.RemoteTcpPort;
		}

		private void socketSelection_RemoteUdpPortChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SocketRemoteUdpPort = socketSelection.RemoteUdpPort;
		}

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SocketLocalInterface = socketSelection.LocalInterface;
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SocketLocalTcpPort = socketSelection.LocalTcpPort;
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.SocketLocalUdpPort = socketSelection.LocalUdpPort;
		}

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		#endregion

		#region Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------

		private void usbSerialHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.UsbSerialHidDeviceInfo = usbSerialHidDeviceSelection.DeviceInfo;
		}

		private void usbSerialHidDeviceSettings_AutoOpenChanged(object sender, EventArgs e)
		{
			this.newTerminalSettingsInEdit.UsbSerialHidAutoOpen = usbSerialHidDeviceSettings.AutoOpen;
		}

		#endregion

		#region Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------

		private void button_OK_Click(object sender, EventArgs e)
		{
			// New terminal settings.
			this.newTerminalSettings = this.newTerminalSettingsInEdit;

			// Create document settings and fill it with new terminal settings.
			this.terminalSettings = new Settings.Terminal.TerminalSettingsRoot();

			this.terminalSettings.Terminal.TerminalType = this.newTerminalSettings.TerminalType;
			this.terminalSettings.Terminal.IO.IOType    = this.newTerminalSettings.IOType;

			this.terminalSettings.Terminal.IO.SerialPort.PortId                    = this.newTerminalSettings.SerialPortId;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.BaudRate    = this.newTerminalSettings.SerialPortCommunication.BaudRate;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.DataBits    = this.newTerminalSettings.SerialPortCommunication.DataBits;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.Parity      = this.newTerminalSettings.SerialPortCommunication.Parity;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.StopBits    = this.newTerminalSettings.SerialPortCommunication.StopBits;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.FlowControl = this.newTerminalSettings.SerialPortCommunication.FlowControl;
			this.terminalSettings.Terminal.IO.SerialPort.AutoReopen                = this.newTerminalSettings.SerialPortAutoReopen;

			this.terminalSettings.Terminal.IO.Socket.RemoteHost              = this.newTerminalSettings.SocketRemoteHost;
			this.terminalSettings.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
			this.terminalSettings.Terminal.IO.Socket.RemoteTcpPort           = this.newTerminalSettings.SocketRemoteTcpPort;
			this.terminalSettings.Terminal.IO.Socket.RemoteUdpPort           = this.newTerminalSettings.SocketRemoteUdpPort;
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
			if (MessageBoxEx.Show
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
				this.newTerminalSettingsInEdit.SetDefaults();
				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Help_Click(object sender, EventArgs e)
		{
			// \fixme: Replace MessageBox with a real help.
			MessageBoxEx.Show
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

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'ioTypeOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.IOType SetControls_ioTypeOld = Domain.IOType.SerialPort;

		/// <remarks>
		/// This functionality is partly duplicated in <see cref="TerminalSettings.SetControls"/>.
		/// Changes here must also be applied there.
		/// </remarks>
		private void SetControls()
		{
			this.isSettingControls.Enter();

			terminalSelection.TerminalType = this.newTerminalSettingsInEdit.TerminalType;

			Domain.IOType ioType = this.newTerminalSettingsInEdit.IOType;
			terminalSelection.IOType = ioType;

			bool isSerialPort   = (ioType == Domain.IOType.SerialPort);
			bool isUsbSerialHid = (ioType == Domain.IOType.UsbSerialHid);
			bool isSocket       = (!isSerialPort && !isUsbSerialHid);

			// Set socket and USB control before serial port control since that might need to refresh
			// the serial port list first (which takes time, which looks ulgy).
			socketSelection.Visible        = isSocket;
			socketSelection.HostType       = (Domain.IOTypeEx)ioType;
			socketSelection.RemoteHost     = this.newTerminalSettingsInEdit.SocketRemoteHost;
			socketSelection.RemoteTcpPort  = this.newTerminalSettingsInEdit.SocketRemoteTcpPort;
			socketSelection.RemoteUdpPort  = this.newTerminalSettingsInEdit.SocketRemoteUdpPort;
			socketSelection.LocalInterface = this.newTerminalSettingsInEdit.SocketLocalInterface;
			socketSelection.LocalTcpPort   = this.newTerminalSettingsInEdit.SocketLocalTcpPort;
			socketSelection.LocalUdpPort   = this.newTerminalSettingsInEdit.SocketLocalUdpPort;

			socketSettings.Visible                 = isSocket;
			socketSettings.HostType                = (Domain.IOTypeEx)ioType;
			socketSettings.TcpClientAutoReconnect  = this.newTerminalSettingsInEdit.TcpClientAutoReconnect;

			usbSerialHidDeviceSelection.Visible    = isUsbSerialHid;
			usbSerialHidDeviceSelection.DeviceInfo = this.newTerminalSettingsInEdit.UsbSerialHidDeviceInfo;

			usbSerialHidDeviceSettings.Visible     = isUsbSerialHid;
			usbSerialHidDeviceSettings.AutoOpen    = this.newTerminalSettingsInEdit.UsbSerialHidAutoOpen;

			serialPortSelection.Visible    = isSerialPort;
			serialPortSelection.PortId     = this.newTerminalSettingsInEdit.SerialPortId;

			serialPortSettings.Visible     = isSerialPort;
			serialPortSettings.BaudRate    = this.newTerminalSettingsInEdit.SerialPortCommunication.BaudRate;
			serialPortSettings.DataBits    = this.newTerminalSettingsInEdit.SerialPortCommunication.DataBits;
			serialPortSettings.Parity      = this.newTerminalSettingsInEdit.SerialPortCommunication.Parity;
			serialPortSettings.StopBits    = this.newTerminalSettingsInEdit.SerialPortCommunication.StopBits;
			serialPortSettings.FlowControl = this.newTerminalSettingsInEdit.SerialPortCommunication.FlowControl;
			serialPortSettings.AutoReopen  = this.newTerminalSettingsInEdit.SerialPortAutoReopen;

			checkBox_StartTerminal.Checked = this.newTerminalSettingsInEdit.StartTerminal;

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
