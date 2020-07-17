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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY.Windows.Forms;

using YAT.Settings.Application;
using YAT.Settings.Model;
using YAT.View.Utilities;

#endregion

namespace YAT.View.Forms
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

		private TerminalSettingsRoot terminalSettings;

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

			// Set visible/invisible before accessing any settings, to ensure that the correct
			// control is shown in case one of the settings leads to an exception (e.g. bug #307).
			SetControlsVisibiliy(this.newTerminalSettingsInEdit.IOType);

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
		public TerminalSettingsRoot TerminalSettingsResult
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
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void NewTerminal_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		private void NewTerminal_Deactivate(object sender, EventArgs e)
		{
			serialPortSelection.OnFormDeactivateWorkaround();
			serialPortSettings .OnFormDeactivateWorkaround();
			socketSelection    .OnFormDeactivateWorkaround();
		}

		private void NewTerminal_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Validate:
			if (DialogResult == DialogResult.OK)
			{
				var settingsAreInvalid = !ValidationHelper.ValidateSettings(this, this.newTerminalSettingsInEdit);
				if (settingsAreInvalid)
				{
					e.Cancel = true;
					return;
				}
			}

			// Update new terminal settings in any case:
			UpdateNewTerminalSettings();

			// Accept the new settings:
			if (DialogResult == DialogResult.OK)
				AcceptSettings();
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

		/// <remarks>
		/// Attention:
		/// Same code exists in YAT.View.Forms.TerminalSettings.terminalSelection_TerminalTypeChanged().
		/// Changes here will have to be applied there too.
		/// </remarks>
		private void terminalSelection_TerminalTypeChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var tt = terminalSelection.TerminalType;
			this.newTerminalSettingsInEdit.TerminalType = tt;
			SetControls();
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.newTerminalSettingsInEdit.IOType = terminalSelection.IOType;
			SetControls();
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
		////if (this.isSettingControls)
		////	return; shall not be done, as...
		////	...the control will automatically switch the port if not or no longer available.
		////	...the control may automatically switch related settings such as Ser/HID presets.
		////	...this event handler only updates the settings tree anyway.

			this.newTerminalSettingsInEdit.SerialPortId = serialPortSelection.PortId;
		}

		private void serialPortSettings_BaudRateChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.SerialPortCommunication.BaudRate = serialPortSettings.BaudRate;
		}

		private void serialPortSettings_DataBitsChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.SerialPortCommunication.DataBits = serialPortSettings.DataBits;
		}

		private void serialPortSettings_ParityChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.SerialPortCommunication.Parity = serialPortSettings.Parity;
		}

		private void serialPortSettings_StopBitsChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.SerialPortCommunication.StopBits = serialPortSettings.StopBits;
		}

		private void serialPortSettings_FlowControlChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.SerialPortCommunication.FlowControl = serialPortSettings.FlowControl;
		}

		private void serialPortSettings_AliveMonitorChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.SerialPortAliveMonitor = serialPortSettings.AliveMonitor;
		}

		private void serialPortSettings_AutoReopenChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.SerialPortAutoReopen = serialPortSettings.AutoReopen;
		}

		#endregion

		#region Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------

		private void socketSelection_RemoteHostChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var host = socketSelection.RemoteHost;
			this.newTerminalSettingsInEdit.SocketRemoteHost = host;
			ApplicationSettings.RoamingUserSettings.Socket.RecentRemoteHosts.Add(host);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_RemoteTcpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.RemoteTcpPort;
			this.newTerminalSettingsInEdit.SocketRemoteTcpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_RemoteUdpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.RemoteUdpPort;
			this.newTerminalSettingsInEdit.SocketRemoteUdpPort = socketSelection.RemoteUdpPort;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; shall not be done, as...
		////	...the control will automatically switch the interface if not or no longer available.
		////	...the control may automatically switch related settings such as Ser/HID presets.
		////	...this event handler only updates the settings tree anyway.

			this.newTerminalSettingsInEdit.SocketLocalInterface = socketSelection.LocalInterface;
		}

		private void socketSelection_LocalFilterChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var filter = socketSelection.LocalFilter;
			this.newTerminalSettingsInEdit.SocketLocalFilter = filter;
			ApplicationSettings.RoamingUserSettings.Socket.RecentLocalFilters.Add(filter);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.LocalTcpPort;
			this.newTerminalSettingsInEdit.SocketLocalTcpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.LocalUdpPort;
			this.newTerminalSettingsInEdit.SocketLocalUdpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		private void socketSettings_UdpServerSendModeChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.UdpServerSendMode = socketSettings.UdpServerSendMode;
		}

		#endregion

		#region Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------

		private void usbSerialHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; shall not be done, as...
		////	...the control will automatically switch the device if not or no longer available.
		////	...the control may automatically switch related settings such as Ser/HID presets.
		////	...this event handler only updates the settings tree anyway.

			// Attention:
			// Same code exists in in the following location:
			//  > TerminalSettings.usbSerialHidDeviceSelection_DeviceInfoChanged()
			// Changes here must be applied there too.

			var di = usbSerialHidDeviceSelection.DeviceInfo;
			this.newTerminalSettingsInEdit.UsbSerialHidDeviceInfo = di;

			// Also update settings control (not via SetControls(), that would be an overkill):
			usbSerialHidDeviceSettings.DeviceInfo = di;
		}

		private void usbSerialHidDeviceSettings_PresetChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.UsbSerialHidPreset = usbSerialHidDeviceSettings.Preset;
		}

		private void usbSerialHidDeviceSettings_ReportFormatChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.UsbSerialHidReportFormat = usbSerialHidDeviceSettings.ReportFormat;
		}

		private void usbSerialHidDeviceSettings_RxFilterUsageChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.UsbSerialHidRxFilterUsage = usbSerialHidDeviceSettings.RxFilterUsage;
		}

		private void usbSerialHidDeviceSettings_FlowControlChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.UsbSerialHidFlowControl = usbSerialHidDeviceSettings.FlowControl;
		}

		private void usbSerialHidDeviceSettings_AutoOpenChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.newTerminalSettingsInEdit.UsbSerialHidAutoOpen = usbSerialHidDeviceSettings.AutoOpen;
		}

		#endregion

		#region Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Help_Click(object sender, EventArgs e)
		{
			// \fixme: Replace MessageBox with a real help.
			MessageBoxEx.Show
			(
				this,
				View.Controls.TerminalSelection.NewTerminalHelpText,
				"New Terminal Help",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'ioTypeOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.IOType SetControls_ioTypeOld = Domain.IOType.SerialPort;

		/// <remarks>
		/// This functionality is partly duplicated in <see cref="TerminalSettings.SetControls"/>.
		/// Changes here must be applied there too!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				terminalSelection.TerminalType = this.newTerminalSettingsInEdit.TerminalType;

				Domain.IOType ioType = this.newTerminalSettingsInEdit.IOType;

				SetControlsVisibiliy(ioType);

				terminalSelection.IOType = ioType;

				serialPortSelection.PortId = this.newTerminalSettingsInEdit.SerialPortId;
			////serialPortSelection.ActivePortInUseInfo is kept at 'null' as no port is yet activated/selected.

				serialPortSettings.BaudRate     = this.newTerminalSettingsInEdit.SerialPortCommunication.BaudRate;
				serialPortSettings.DataBits     = this.newTerminalSettingsInEdit.SerialPortCommunication.DataBits;
				serialPortSettings.Parity       = this.newTerminalSettingsInEdit.SerialPortCommunication.Parity;
				serialPortSettings.StopBits     = this.newTerminalSettingsInEdit.SerialPortCommunication.StopBits;
				serialPortSettings.FlowControl  = this.newTerminalSettingsInEdit.SerialPortCommunication.FlowControl;
				serialPortSettings.AliveMonitor = this.newTerminalSettingsInEdit.SerialPortAliveMonitor;
				serialPortSettings.AutoReopen   = this.newTerminalSettingsInEdit.SerialPortAutoReopen;

				socketSelection.SocketType         = (Domain.IOTypeEx)ioType;
				socketSelection.RemoteHost         = this.newTerminalSettingsInEdit.SocketRemoteHost;
				socketSelection.RemoteTcpPort      = this.newTerminalSettingsInEdit.SocketRemoteTcpPort;
				socketSelection.RemoteUdpPort      = this.newTerminalSettingsInEdit.SocketRemoteUdpPort;
				socketSelection.LocalInterface     = this.newTerminalSettingsInEdit.SocketLocalInterface;
				socketSelection.LocalFilter        = this.newTerminalSettingsInEdit.SocketLocalFilter;
				socketSelection.LocalTcpPort       = this.newTerminalSettingsInEdit.SocketLocalTcpPort;
				socketSelection.LocalUdpPort       = this.newTerminalSettingsInEdit.SocketLocalUdpPort;
				socketSelection.RecentRemoteHosts  = ApplicationSettings.RoamingUserSettings.Socket.RecentRemoteHosts;
				socketSelection.RecentLocalFilters = ApplicationSettings.RoamingUserSettings.Socket.RecentLocalFilters;
				socketSelection.RecentPorts        = ApplicationSettings.RoamingUserSettings.Socket.RecentPorts;

				socketSettings.SocketType             = (Domain.IOTypeEx)ioType;
				socketSettings.TcpClientAutoReconnect = this.newTerminalSettingsInEdit.TcpClientAutoReconnect;
				socketSettings.UdpServerSendMode      = this.newTerminalSettingsInEdit.UdpServerSendMode;

				usbSerialHidDeviceSelection.DeviceInfo = this.newTerminalSettingsInEdit.UsbSerialHidDeviceInfo;

				usbSerialHidDeviceSettings.DeviceInfo    = this.newTerminalSettingsInEdit.UsbSerialHidDeviceInfo;
				                                       ////this.newTerminalSettingsInEdit.UsbSerialHidMatchSerial is defined by the 'LocalUserSettings'.
				usbSerialHidDeviceSettings.Preset        = this.newTerminalSettingsInEdit.UsbSerialHidPreset;
				usbSerialHidDeviceSettings.ReportFormat  = this.newTerminalSettingsInEdit.UsbSerialHidReportFormat;
				usbSerialHidDeviceSettings.RxFilterUsage = this.newTerminalSettingsInEdit.UsbSerialHidRxFilterUsage;
				usbSerialHidDeviceSettings.FlowControl   = this.newTerminalSettingsInEdit.UsbSerialHidFlowControl;
				usbSerialHidDeviceSettings.AutoOpen      = this.newTerminalSettingsInEdit.UsbSerialHidAutoOpen;

				checkBox_StartTerminal.Checked = this.newTerminalSettingsInEdit.StartTerminal;

				// Trigger refresh of ports/devices if selection of I/O type has changed:
				bool isSerialPort   = ((Domain.IOTypeEx)ioType).IsSerialPort;
				bool isSocket       = ((Domain.IOTypeEx)ioType).IsSocket;
				bool isUsbSerialHid = ((Domain.IOTypeEx)ioType).IsUsbSerialHid;

				bool wasSerialPort   = ((Domain.IOTypeEx)this.SetControls_ioTypeOld).IsSerialPort;
				bool wasSocket       = ((Domain.IOTypeEx)this.SetControls_ioTypeOld).IsSocket;
				bool wasUsbSerialHid = ((Domain.IOTypeEx)this.SetControls_ioTypeOld).IsUsbSerialHid;

				if      (isSerialPort   && !wasSerialPort)
					serialPortSelection        .RefreshPortList();
				else if (isSocket       && !wasSocket)
					socketSelection            .RefreshLocalInterfaceList();
				else if (isUsbSerialHid && !wasUsbSerialHid)
					usbSerialHidDeviceSelection.RefreshDeviceList();

				this.SetControls_ioTypeOld = ioType;

				// Finally, enable OK button if port/device is valid:
				bool isValid = true;
				switch (ioType)
				{
					case Domain.IOType.SerialPort:   isValid = serialPortSelection        .IsValid; break;
					case Domain.IOType.UsbSerialHid: isValid = usbSerialHidDeviceSelection.IsValid; break;
				}
				button_OK.Enabled = isValid;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// This functionality is partly duplicated in <see cref="TerminalSettings.SetControls"/>.
		/// Changes here must be applied there too!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void SetControlsVisibiliy(Domain.IOType ioType)
		{
			bool isSerialPort   = ((Domain.IOTypeEx)ioType).IsSerialPort;
			bool isSocket       = ((Domain.IOTypeEx)ioType).IsSocket;
			bool isUsbSerialHid = ((Domain.IOTypeEx)ioType).IsUsbSerialHid;

			serialPortSelection.Visible = isSerialPort;
			serialPortSettings .Visible = isSerialPort;

			socketSelection.Visible = isSocket;
			socketSettings .Visible = isSocket;

			usbSerialHidDeviceSelection.Visible = isUsbSerialHid;
			usbSerialHidDeviceSettings .Visible = isUsbSerialHid;
		}

		private void UpdateNewTerminalSettings()
		{
			this.newTerminalSettings = this.newTerminalSettingsInEdit;
		}

		private void AcceptSettings()
		{
			// Create document settings and fill it with new terminal settings:
			this.terminalSettings = new TerminalSettingsRoot();

			this.terminalSettings.Terminal.TerminalType                            = this.newTerminalSettings.TerminalType;
			this.terminalSettings.Terminal.UpdateTerminalTypeDependentSettings(); // Needed because the parent settings don't notice the 'TerminalType' change above!
			this.terminalSettings.Terminal.IO.IOType                               = this.newTerminalSettings.IOType;
			this.terminalSettings.Terminal.UpdateIOTypeDependentSettings(); // Needed because the parent settings don't notice the 'IOType' change above!

			this.terminalSettings.Terminal.IO.SerialPort.PortId                    = this.newTerminalSettings.SerialPortId;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.BaudRate    = this.newTerminalSettings.SerialPortCommunication.BaudRate;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.DataBits    = this.newTerminalSettings.SerialPortCommunication.DataBits;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.Parity      = this.newTerminalSettings.SerialPortCommunication.Parity;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.StopBits    = this.newTerminalSettings.SerialPortCommunication.StopBits;
			this.terminalSettings.Terminal.IO.SerialPort.Communication.FlowControl = this.newTerminalSettings.SerialPortCommunication.FlowControl;
		////this.terminalSettings.Terminal.IO.SerialPort.SignalXOnWhenOpened is an advanced setting, i.e. not available in the [New Terminal] dialog.
			this.terminalSettings.Terminal.IO.SerialPort.AliveMonitor              = this.newTerminalSettings.SerialPortAliveMonitor;
			this.terminalSettings.Terminal.IO.SerialPort.AutoReopen                = this.newTerminalSettings.SerialPortAutoReopen;

			this.terminalSettings.Terminal.IO.Socket.RemoteHost                    = this.newTerminalSettings.SocketRemoteHost;
			this.terminalSettings.Terminal.IO.Socket.RemoteTcpPort                 = this.newTerminalSettings.SocketRemoteTcpPort;
			this.terminalSettings.Terminal.IO.Socket.RemoteUdpPort                 = this.newTerminalSettings.SocketRemoteUdpPort;
			this.terminalSettings.Terminal.IO.Socket.LocalInterface                = this.newTerminalSettings.SocketLocalInterface;
			this.terminalSettings.Terminal.IO.Socket.LocalFilter                   = this.newTerminalSettings.SocketLocalFilter;
			this.terminalSettings.Terminal.IO.Socket.LocalTcpPort                  = this.newTerminalSettings.SocketLocalTcpPort;
			this.terminalSettings.Terminal.IO.Socket.LocalUdpPort                  = this.newTerminalSettings.SocketLocalUdpPort;
			this.terminalSettings.Terminal.IO.Socket.TcpClientAutoReconnect        = this.newTerminalSettings.TcpClientAutoReconnect;
			this.terminalSettings.Terminal.IO.Socket.UdpServerSendMode             = this.newTerminalSettings.UdpServerSendMode;

			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.DeviceInfo        = this.newTerminalSettings.UsbSerialHidDeviceInfo;              // Note that 'MatchSerial' is an integral part of MKY.IO.Serial.Usb, will thus be contained in the .yat file, even though always overridden by the 'LocalUserSettings'.
			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.MatchSerial       = ApplicationSettings.LocalUserSettings.General.MatchUsbSerial; // Given by the 'LocalUserSettings'.
			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.Preset            = this.newTerminalSettings.UsbSerialHidPreset;
			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.ReportFormat      = this.newTerminalSettings.UsbSerialHidReportFormat;
			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.RxFilterUsage     = this.newTerminalSettings.UsbSerialHidRxFilterUsage;
			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.FlowControl       = this.newTerminalSettings.UsbSerialHidFlowControl;
		////this.terminalSettings.Terminal.IO.UsbSerialHidDevice.SignalXOnWhenOpened is an advanced setting, i.e. not available in the [New Terminal] dialog.
			this.terminalSettings.Terminal.IO.UsbSerialHidDevice.AutoOpen          = this.newTerminalSettings.UsbSerialHidAutoOpen;
		////this.terminalSettings.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData is an advanced setting, i.e. not available in the [New Terminal] dialog.

			this.terminalSettings.Terminal.UpdateIOSettingsDependentSettings(); // Needed because the parent settings don't notice the changes above!

			this.terminalSettings.TerminalIsStarted                                = this.newTerminalSettings.StartTerminal;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
