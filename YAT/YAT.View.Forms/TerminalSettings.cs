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
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.IO.Ports;
using MKY.Windows.Forms;

using YAT.Settings.Application;
using YAT.Settings.Model;
using YAT.View.Utilities;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class TerminalSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private int terminalId; // = 0;
		private bool terminalIsOpen; // = false;
		private string terminalSerialPortName; // = null;

		private TerminalExplicitSettings settings;
		private TerminalExplicitSettings settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminalSettings"/> class.
		/// </summary>
		/// <remarks>
		/// Using <see cref="TerminalExplicitSettings"/> instead of simply using
		/// <see cref="Domain.Settings.TerminalSettings"/> for two reasons:
		/// <list type="bullet">
		/// <item><description>Handling of <see cref="TerminalExplicitSettings.UserName"/>.</description></item>
		/// <item><description>Prepared for future migration to tree view dialog containing all settings.</description></item>
		/// </list>
		/// </remarks>
		public TerminalSettings(TerminalExplicitSettings settings)
		{
			InitializeComponent();

			this.settings = settings;
			this.settingsInEdit = new TerminalExplicitSettings(settings); // Clone to ensure decoupling.

			// Set visible/invisible before accessing any settings, to ensure that the correct
			// control is shown in case one of the settings leads to an exception (e.g. bug #307).
			SetControlsVisibiliy(this.settingsInEdit.Terminal.IO.IOType);

			// SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>Meta information needed for e.g. "(in use by this terminal)".</remarks>
		public int TerminalId
		{
			get { return (this.terminalId); }
			set { this.terminalId = value;  }
		}

		/// <remarks>Meta information needed for e.g. "(in use by this terminal)".</remarks>
		public bool TerminalIsOpen
		{
			get { return (this.terminalIsOpen); }
			set { this.terminalIsOpen = value;  }
		}

		/// <remarks>Meta information needed for e.g. "(in use by this terminal)".</remarks>
		public string TerminalSerialPortName
		{
			get { return (this.terminalSerialPortName); }
			set { this.terminalSerialPortName = value;  }
		}

		/// <summary></summary>
		public TerminalExplicitSettings SettingsResult
		{
			get { return (this.settings); }
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
		private void TerminalSettings_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		private void TerminalSettings_Deactivate(object sender, EventArgs e)
		{
			serialPortSelection.OnFormDeactivateWorkaround();
			serialPortSettings .OnFormDeactivateWorkaround();
			socketSelection    .OnFormDeactivateWorkaround();
		}

		private void TerminalSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				// Validate:
				var settingsAreInvalid = !ValidationHelper.ValidateSettings(this, this.settingsInEdit);
				if (settingsAreInvalid)
				{
					e.Cancel = true;
					return;
				}

				// Accept the new settings:
				this.settings = this.settingsInEdit;         // Note that 'MatchSerial' is an integral part of MKY.IO.Serial.Usb, will thus be contained in the .yat file, even though always overridden by the 'LocalUserSettings'.
				this.settings.Terminal.IO.UsbSerialHidDevice.MatchSerial = ApplicationSettings.LocalUserSettings.General.MatchUsbSerial; // Given by the 'LocalUserSettings'.
			}
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
			if (this.isSettingControls)
				return;

			this.settingsInEdit.Terminal.TerminalType = terminalSelection.TerminalType;

			// Silently update the terminal type dependent settings, user shall not get "annoyed"
			// to confirm, and the changes are that basic that they need to be changed anyway.
			this.settingsInEdit.Terminal.UpdateTerminalTypeDependentSettings();

			SetControls();
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var ioTypeOldWasUdpSocket              = ((Domain.IOTypeEx)this.settingsInEdit.Terminal.IO.IOType).IsUdpSocket;
			this.settingsInEdit.Terminal.IO.IOType =                                 terminalSelection.IOType;
			var ioTypeNewIsUdpSocket               = ((Domain.IOTypeEx)              terminalSelection.IOType).IsUdpSocket;

			if (ioTypeNewIsUdpSocket != ioTypeOldWasUdpSocket)
				PotentiallyUpdateIOTypeDependentSettings(ioTypeOldWasUdpSocket, ioTypeNewIsUdpSocket);

			SetControls();
		}

		private void button_TextOrBinarySettings_Click(object sender, EventArgs e)
		{
			ShowTextOrBinarySettings();
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

			this.settingsInEdit.Terminal.IO.SerialPort.PortId = serialPortSelection.PortId;
		}

		private void serialPortSettings_BaudRateChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.SerialPort.Communication.BaudRate = serialPortSettings.BaudRate;
		}

		private void serialPortSettings_DataBitsChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.SerialPort.Communication.DataBits = serialPortSettings.DataBits;
		}

		private void serialPortSettings_ParityChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.SerialPort.Communication.Parity = serialPortSettings.Parity;
		}

		private void serialPortSettings_StopBitsChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.SerialPort.Communication.StopBits = serialPortSettings.StopBits;
		}

		private void serialPortSettings_FlowControlChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			var flowControlOldUsedXOnXOffAutomatically = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOffAutomatically;
			this.settingsInEdit.Terminal.IO.SerialPort.Communication.FlowControl = serialPortSettings.FlowControl;
			var flowControlNewUsesXOnXOffAutomatically = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOffAutomatically;

			if (flowControlNewUsesXOnXOffAutomatically != flowControlOldUsedXOnXOffAutomatically)
				PotentiallyUpdateIOSettingsDependentSettings(flowControlOldUsedXOnXOffAutomatically, flowControlNewUsesXOnXOffAutomatically);
		}

		private void serialPortSettings_AliveMonitorChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.SerialPort.AliveMonitor = serialPortSettings.AliveMonitor;
		}

		private void serialPortSettings_AutoReopenChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.SerialPort.AutoReopen = serialPortSettings.AutoReopen;
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
			this.settingsInEdit.Terminal.IO.Socket.RemoteHost = host;
			ApplicationSettings.RoamingUserSettings.Socket.RecentRemoteHosts.Add(host);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_RemoteTcpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.RemoteTcpPort;
			this.settingsInEdit.Terminal.IO.Socket.RemoteTcpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_RemoteUdpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.RemoteUdpPort;
			this.settingsInEdit.Terminal.IO.Socket.RemoteUdpPort = port;
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

			this.settingsInEdit.Terminal.IO.Socket.LocalInterface = socketSelection.LocalInterface;
		}

		private void socketSelection_LocalFilterChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var filter = socketSelection.LocalFilter;
			this.settingsInEdit.Terminal.IO.Socket.LocalFilter = filter;
			ApplicationSettings.RoamingUserSettings.Socket.RecentLocalFilters.Add(filter);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.LocalTcpPort;
			this.settingsInEdit.Terminal.IO.Socket.LocalTcpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var port = socketSelection.LocalUdpPort;
			this.settingsInEdit.Terminal.IO.Socket.LocalUdpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.Socket.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		private void socketSettings_UdpServerSendModeChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.Socket.UdpServerSendMode = socketSettings.UdpServerSendMode;
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
			//  > NewTerminal.usbSerialHidDeviceSelection_DeviceInfoChanged()
			// Changes here must be applied there too.

			var di = usbSerialHidDeviceSelection.DeviceInfo;
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = di;

			// Also update settings control (not via SetControls(), that would be an overkill):
			usbSerialHidDeviceSettings.DeviceInfo = di;
		}

		private void usbSerialHidDeviceSettings_PresetChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.Preset = usbSerialHidDeviceSettings.Preset;
		}

		private void usbSerialHidDeviceSettings_ReportFormatChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.ReportFormat = usbSerialHidDeviceSettings.ReportFormat;
		}

		private void usbSerialHidDeviceSettings_RxFilterUsageChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.RxFilterUsage = usbSerialHidDeviceSettings.RxFilterUsage;
		}

		private void usbSerialHidDeviceSettings_FlowControlChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			var flowControlOldUsedXOnXOffAutomatically = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOffAutomatically;
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.FlowControl = usbSerialHidDeviceSettings.FlowControl;
			var flowControlNewUsesXOnXOffAutomatically = this.settingsInEdit.Terminal.IO.FlowControlUsesXOnXOffAutomatically;

			if (flowControlNewUsesXOnXOffAutomatically != flowControlOldUsedXOnXOffAutomatically)
				PotentiallyUpdateIOSettingsDependentSettings(flowControlOldUsedXOnXOffAutomatically, flowControlNewUsesXOnXOffAutomatically);
		}

		private void usbSerialHidDeviceSettings_AutoOpenChanged(object sender, EventArgs e)
		{
		////if (this.isSettingControls)
		////	return; see above!

			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.AutoOpen = usbSerialHidDeviceSettings.AutoOpen;
		}

		#endregion

		#region Controls Event Handlers > Advanced
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Advanced
		//------------------------------------------------------------------------------------------

		private void button_AdvancedSettings_Click(object sender, EventArgs e)
		{
			ShowAdvancedSettings();
		}

		#endregion

		#region Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Buttons
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			string type;
			switch (this.settingsInEdit.Terminal.TerminalType)
			{
				case Domain.TerminalType.Text:   type = "[Text]";   break;
				case Domain.TerminalType.Binary: type = "[Binary]"; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.settingsInEdit.Terminal.TerminalType + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			var sb = new StringBuilder();
			sb.AppendLine("Reset all settings to default values?");
			sb.Append    (type);
			sb.Append       (" and [Advanced] settings will be reset too!");
			//// Actually text and binary are reset. But only one of both is in use anyway...

			if (MessageBoxEx.Show
				(
					this,
					sb.ToString(),
					"Defaults?",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.settingsInEdit.SetDefaults();
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
		/// This functionality is partly duplicated in <see cref="NewTerminal.SetControls"/>.
		/// Changes here must be applied there too!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				var tt = this.settingsInEdit.Terminal.TerminalType;
				terminalSelection.TerminalType = tt;

				string button = "&";
				string toolTipCaption = "";
				switch (tt)
				{
					case Domain.TerminalType.Text:
						button += "Text";
						toolTipCaption = "Text terminal dependent settings such as encoding, EOL (end-of-line) and comments.";
						break;

					case Domain.TerminalType.Binary:
						button += "Binary";
						toolTipCaption = "Binary terminal dependent settings such as sequence and timeout line breaks.";
						break;

					default:
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + tt + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
				button += " Settings...";
				button_TextOrBinarySettings.Text = button;
				toolTip.SetToolTip(button_TextOrBinarySettings, toolTipCaption);

				var ioType = this.settingsInEdit.Terminal.IO.IOType;

				SetControlsVisibiliy(ioType);

				terminalSelection.IOType = ioType;

				var forceSerialPortListRefresh = (serialPortSelection.PortId != this.settingsInEdit.Terminal.IO.SerialPort.PortId); // Required to potentially perform fallback to default other than COM1.

				serialPortSelection.PortId = this.settingsInEdit.Terminal.IO.SerialPort.PortId;
				serialPortSelection.ActivePortInUseInfo = new InUseInfo
				(
					this.terminalId,
					this.terminalSerialPortName, // Attention: Not using the currently selected port as that would result in
					this.terminalIsOpen,         //            "(in use by another application)" on resetting to [Defaults...].
					(this.terminalIsOpen ? "(in use by this terminal)" : "(selected by this terminal)")
				);                           // Attention: Same texts are used in YAT.Model.Main.SerialPortCollection_InUseLookupRequest().
				                           ////            Changes above likely have to be applied there too.
				serialPortSettings.BaudRate     = this.settingsInEdit.Terminal.IO.SerialPort.Communication.BaudRate;
				serialPortSettings.DataBits     = this.settingsInEdit.Terminal.IO.SerialPort.Communication.DataBits;
				serialPortSettings.Parity       = this.settingsInEdit.Terminal.IO.SerialPort.Communication.Parity;
				serialPortSettings.StopBits     = this.settingsInEdit.Terminal.IO.SerialPort.Communication.StopBits;
				serialPortSettings.FlowControl  = this.settingsInEdit.Terminal.IO.SerialPort.Communication.FlowControl;
			////serialPortSettings.SignalXOnWhenOpened is an advanced setting, i.e. not available in the [Terminal Settings] dialog.
				serialPortSettings.AliveMonitor = this.settingsInEdit.Terminal.IO.SerialPort.AliveMonitor;
				serialPortSettings.AutoReopen   = this.settingsInEdit.Terminal.IO.SerialPort.AutoReopen;

				socketSelection.SocketType         = (Domain.IOTypeEx)ioType;
				socketSelection.RemoteHost         = this.settingsInEdit.Terminal.IO.Socket.RemoteHost;
				socketSelection.RemoteTcpPort      = this.settingsInEdit.Terminal.IO.Socket.RemoteTcpPort;
				socketSelection.RemoteUdpPort      = this.settingsInEdit.Terminal.IO.Socket.RemoteUdpPort;
				socketSelection.LocalInterface     = this.settingsInEdit.Terminal.IO.Socket.LocalInterface;
				socketSelection.LocalFilter        = this.settingsInEdit.Terminal.IO.Socket.LocalFilter;
				socketSelection.LocalTcpPort       = this.settingsInEdit.Terminal.IO.Socket.LocalTcpPort;
				socketSelection.LocalUdpPort       = this.settingsInEdit.Terminal.IO.Socket.LocalUdpPort;
				socketSelection.RecentRemoteHosts  = ApplicationSettings.RoamingUserSettings.Socket.RecentRemoteHosts;
				socketSelection.RecentLocalFilters = ApplicationSettings.RoamingUserSettings.Socket.RecentLocalFilters;
				socketSelection.RecentPorts        = ApplicationSettings.RoamingUserSettings.Socket.RecentPorts;

				socketSettings.SocketType             = (Domain.IOTypeEx)ioType;
				socketSettings.TcpClientAutoReconnect = this.settingsInEdit.Terminal.IO.Socket.TcpClientAutoReconnect;
				socketSettings.UdpServerSendMode      = this.settingsInEdit.Terminal.IO.Socket.UdpServerSendMode;

				usbSerialHidDeviceSelection.DeviceInfo = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.DeviceInfo;

				                                       ////this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.MatchSerial is defined by the LocalUserSettings.
				usbSerialHidDeviceSettings.Preset        = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.Preset;
				usbSerialHidDeviceSettings.ReportFormat  = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.ReportFormat;
				usbSerialHidDeviceSettings.RxFilterUsage = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.RxFilterUsage;
				usbSerialHidDeviceSettings.FlowControl   = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.FlowControl;
			////usbSerialHidDeviceSettings.SignalXOnWhenOpened is an advanced setting, i.e. not available in the [Terminal Settings] dialog.
				usbSerialHidDeviceSettings.AutoOpen      = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.AutoOpen;
			////usbSerialHidDeviceSettings.IncludeNonPayloadData is an advanced setting, i.e. not available in the [Terminal Settings] dialog.

				// Trigger refresh of ports/devices if selection of I/O type has changed:
				bool isSerialPort   = ((Domain.IOTypeEx)ioType).IsSerialPort;
				bool isSocket       = ((Domain.IOTypeEx)ioType).IsSocket;
				bool isUsbSerialHid = ((Domain.IOTypeEx)ioType).IsUsbSerialHid;

				bool wasSerialPort   = ((Domain.IOTypeEx)this.SetControls_ioTypeOld).IsSerialPort;
				bool wasSocket       = ((Domain.IOTypeEx)this.SetControls_ioTypeOld).IsSocket;
				bool wasUsbSerialHid = ((Domain.IOTypeEx)this.SetControls_ioTypeOld).IsUsbSerialHid;

				if      (isSerialPort   && (!wasSerialPort || forceSerialPortListRefresh))
					serialPortSelection        .RefreshPortList();
				else if (isSocket       && (!wasSocket))
					socketSelection            .RefreshLocalInterfaceList();
				else if (isUsbSerialHid && (!wasUsbSerialHid))
					usbSerialHidDeviceSelection.RefreshDeviceList();

				this.SetControls_ioTypeOld = ioType;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// This functionality is partly duplicated in <see cref="NewTerminal.SetControls"/>.
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
			serialPortSettings.Visible  = isSerialPort;

			socketSelection.Visible = isSocket;
			socketSettings.Visible  = isSocket;

			usbSerialHidDeviceSelection.Visible = isUsbSerialHid;
			usbSerialHidDeviceSettings.Visible  = isUsbSerialHid;
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowTextOrBinarySettings()
		{
			var tt = this.settingsInEdit.Terminal.TerminalType;
			switch (tt)
			{
				case Domain.TerminalType.Text:
				{
					var f = new TextTerminalSettings(this.settingsInEdit.Terminal.TextTerminal);
					if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
						this.settingsInEdit.Terminal.TextTerminal = f.SettingsResult;

					break;
				}

				case Domain.TerminalType.Binary:
				{
					var f = new BinaryTerminalSettings(this.settingsInEdit.Terminal.BinaryTerminal);
					if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
						this.settingsInEdit.Terminal.BinaryTerminal = f.SettingsResult;

					break;
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + tt + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <remarks>
		/// The following list is used to reduce the number of changed settings as
		/// <c>this.settingsInEdit = f.SettingsResult</c> would.
		/// </remarks>
		/// <remarks>
		/// The following list must handle the same properties as
		/// <see cref="AdvancedTerminalSettings.SetDefaults()"/> defaults.
		/// </remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowAdvancedSettings()
		{
			var f = new AdvancedTerminalSettings(this.settingsInEdit);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				// Status:
				this.settingsInEdit.Terminal.Status.ShowConnectTime  = f.SettingsResult.Terminal.Status.ShowConnectTime;
				this.settingsInEdit.Terminal.Status.ShowCountAndRate = f.SettingsResult.Terminal.Status.ShowCountAndRate;

				// Radix:
				this.settingsInEdit.Terminal.Display.SeparateTxRxRadix = f.SettingsResult.Terminal.Display.SeparateTxRxRadix;
				this.settingsInEdit.Terminal.Display.TxRadix           = f.SettingsResult.Terminal.Display.TxRadix;
				this.settingsInEdit.Terminal.Display.RxRadix           = f.SettingsResult.Terminal.Display.RxRadix;

				// Display:
				this.settingsInEdit.Terminal.Display.ShowRadix            = f.SettingsResult.Terminal.Display.ShowRadix;
				this.settingsInEdit.Terminal.Display.ShowLineNumbers      = f.SettingsResult.Terminal.Display.ShowLineNumbers;
				this.settingsInEdit.Terminal.Display.LineNumberSelection  = f.SettingsResult.Terminal.Display.LineNumberSelection;
				this.settingsInEdit.Terminal.Display.ShowTimeStamp        = f.SettingsResult.Terminal.Display.ShowTimeStamp;
				this.settingsInEdit.Terminal.Display.ShowTimeSpan         = f.SettingsResult.Terminal.Display.ShowTimeSpan;
				this.settingsInEdit.Terminal.Display.ShowTimeDelta        = f.SettingsResult.Terminal.Display.ShowTimeDelta;
				this.settingsInEdit.Terminal.Display.ShowDevice           = f.SettingsResult.Terminal.Display.ShowDevice;
				this.settingsInEdit.Terminal.Display.ShowDirection        = f.SettingsResult.Terminal.Display.ShowDirection;
				this.settingsInEdit.Terminal.Display.ShowLength           = f.SettingsResult.Terminal.Display.ShowLength;
				this.settingsInEdit.Terminal.Display.LengthSelection      = f.SettingsResult.Terminal.Display.LengthSelection;
				this.settingsInEdit.Terminal.Display.ShowDuration         = f.SettingsResult.Terminal.Display.ShowDuration;
				this.settingsInEdit.Terminal.Display.IncludeIOControl     = f.SettingsResult.Terminal.Display.IncludeIOControl;

				this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled = f.SettingsResult.Terminal.Display.DirectionLineBreakEnabled;
				this.settingsInEdit.Terminal.Display.DeviceLineBreakEnabled    = f.SettingsResult.Terminal.Display.DeviceLineBreakEnabled;
				this.settingsInEdit.Terminal.TextTerminal.GlueCharsOfLine      = f.SettingsResult.Terminal.TextTerminal.GlueCharsOfLine; // Tightly coupled to settings above, thus located in advanced dialog.

				this.settingsInEdit.Terminal.Display.MaxLineCount         = f.SettingsResult.Terminal.Display.MaxLineCount;
				this.settingsInEdit.Terminal.Display.MaxLineLength        = f.SettingsResult.Terminal.Display.MaxLineLength;
				this.settingsInEdit.Terminal.Display.ShowCopyOfActiveLine = f.SettingsResult.Terminal.Display.ShowCopyOfActiveLine;

				// Char replace/hide:
				this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars = f.SettingsResult.Terminal.CharReplace.ReplaceControlChars;
				this.settingsInEdit.Terminal.CharReplace.ControlCharRadix    = f.SettingsResult.Terminal.CharReplace.ControlCharRadix;
				this.settingsInEdit.Terminal.CharReplace.ReplaceBackspace    = f.SettingsResult.Terminal.CharReplace.ReplaceBackspace;
				this.settingsInEdit.Terminal.CharReplace.ReplaceTab          = f.SettingsResult.Terminal.CharReplace.ReplaceTab;
				this.settingsInEdit.Terminal.CharHide.HideXOnXOff            = f.SettingsResult.Terminal.CharHide.HideXOnXOff;
				this.settingsInEdit.Terminal.CharAction.BeepOnBell           = f.SettingsResult.Terminal.CharAction.BeepOnBell;
				this.settingsInEdit.Terminal.CharReplace.ReplaceSpace        = f.SettingsResult.Terminal.CharReplace.ReplaceSpace;
				this.settingsInEdit.Terminal.CharHide.Hide0x00               = f.SettingsResult.Terminal.CharHide.Hide0x00;
				this.settingsInEdit.Terminal.CharHide.Hide0xFF               = f.SettingsResult.Terminal.CharHide.Hide0xFF;

				// USB Ser/HID:
				this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData = f.SettingsResult.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData;

				// Communication:
				this.settingsInEdit.Terminal.IO.Endianness                        = f.SettingsResult.Terminal.IO.Endianness;
				this.settingsInEdit.Terminal.IO.SerialPort.IgnoreFramingErrors    = f.SettingsResult.Terminal.IO.SerialPort.IgnoreFramingErrors;
				this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates     = f.SettingsResult.Terminal.IO.IndicateSerialPortBreakStates;
				this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable = f.SettingsResult.Terminal.IO.SerialPortOutputBreakIsModifiable;

				this.settingsInEdit.Terminal.Status.ShowBreakCount       = f.SettingsResult.Terminal.Status.ShowBreakCount;
				this.settingsInEdit.Terminal.Status.ShowFlowControlCount = f.SettingsResult.Terminal.Status.ShowFlowControlCount;

				// Send:
				this.settingsInEdit.Terminal.Send.UseExplicitDefaultRadix         = f.SettingsResult.Terminal.Send.UseExplicitDefaultRadix;
				this.settingsInEdit.Terminal.Send.AllowConcurrency                = f.SettingsResult.Terminal.Send.AllowConcurrency;
				this.settingsInEdit.Terminal.Send.Text.KeepSendText               = f.SettingsResult.Terminal.Send.Text.KeepSendText;
				this.settingsInEdit.Terminal.Send.Text.SendImmediately            = f.SettingsResult.Terminal.Send.Text.SendImmediately;
				this.settingsInEdit.Terminal.Send.File.SkipEmptyLines             = f.SettingsResult.Terminal.Send.File.SkipEmptyLines;
				this.settingsInEdit.Terminal.Send.CopyPredefined                  = f.SettingsResult.Terminal.Send.CopyPredefined;

				this.settingsInEdit.Terminal.IO.SerialPort        .SignalXOnWhenOpened = f.SettingsResult.Terminal.IO.SerialPort.SignalXOnWhenOpened;
				this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.SignalXOnWhenOpened = f.SettingsResult.Terminal.IO.UsbSerialHidDevice.SignalXOnWhenOpened;

				this.settingsInEdit.Terminal.Send.SignalXOnBeforeEachTransmission = f.SettingsResult.Terminal.Send.SignalXOnBeforeEachTransmission;
				this.settingsInEdit.Terminal.Send.SignalXOnPeriodically           = f.SettingsResult.Terminal.Send.SignalXOnPeriodically;
				this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize       = f.SettingsResult.Terminal.IO.SerialPort.OutputBufferSize;
				this.settingsInEdit.Terminal.IO.SerialPort.BufferMaxBaudRate      = f.SettingsResult.Terminal.IO.SerialPort.BufferMaxBaudRate;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize           = f.SettingsResult.Terminal.IO.SerialPort.MaxChunkSize;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate            = f.SettingsResult.Terminal.IO.SerialPort.MaxSendRate;
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak    = f.SettingsResult.Terminal.IO.SerialPort.NoSendOnOutputBreak;
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak     = f.SettingsResult.Terminal.IO.SerialPort.NoSendOnInputBreak;

				this.settingsInEdit.Terminal.Send.Text.EnableEscapes              = f.SettingsResult.Terminal.Send.Text.EnableEscapes;
				this.settingsInEdit.Terminal.Send.File.EnableEscapes              = f.SettingsResult.Terminal.Send.File.EnableEscapes;

				this.settingsInEdit.Terminal.Send.DefaultDelay                    = f.SettingsResult.Terminal.Send.DefaultDelay;
				this.settingsInEdit.Terminal.Send.DefaultLineDelay                = f.SettingsResult.Terminal.Send.DefaultLineDelay;
				this.settingsInEdit.Terminal.Send.DefaultLineInterval             = f.SettingsResult.Terminal.Send.DefaultLineInterval;
				this.settingsInEdit.Terminal.Send.DefaultLineRepeat               = f.SettingsResult.Terminal.Send.DefaultLineRepeat;

				// User:
				this.settingsInEdit.UserName = f.SettingsResult.UserName;
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void PotentiallyUpdateIOTypeDependentSettings(bool ioTypeOldWasUdpSocket, bool ioTypeNewIsUdpSocket)
		{
			DialogResult dr;

			if (this.settingsInEdit.Terminal.IOTypeDependentSettingsWereDefaults(ioTypeOldWasUdpSocket))
			{
				dr = DialogResult.Yes; // Update silently.
			}
			else // Update only if confirmed by the user.
			{
				if (ioTypeNewIsUdpSocket)
				{
					var message = new StringBuilder();
					message.AppendLine("Port type has changed to UDP/IP. Shall UDP/IP related settings be changed accordingly?");
					message.AppendLine("");
					message.AppendLine("Confirming [Yes] will...");
					message.AppendLine("...change [EOL sequence] to [None] in [Text Settings...].");
					message.Append    ("...enable [Break lines on every chunk] in [Text Settings...].");

					dr = MessageBoxEx.Show
					(
						this,
						message.ToString(),
						"Change Related Settings?",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question
					);
				}
				else // ioTypeOldWasUdpSocket
				{
					var message = new StringBuilder();
					message.AppendLine("Port type has changed to other than UDP/IP. Shall UDP/IP related settings be changed accordingly?");
					message.AppendLine("");
					message.AppendLine("Confirming [Yes] will...");
					message.AppendLine("...change [EOL sequence] in [Text Settings...] to the system default.");
					message.Append    ("...disable [Break lines on every chunk] in [Text Settings...].");

					dr = MessageBoxEx.Show
					(
						this,
						message.ToString(),
						"Change Related Settings?",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question
					);
				}
			}

			if (dr == DialogResult.Yes)
			{
				this.settingsInEdit.Terminal.UpdateIOTypeDependentSettings(ioTypeNewIsUdpSocket);
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void PotentiallyUpdateIOSettingsDependentSettings(bool flowControlOldUsedXOnXOffAutomatically, bool flowControlNewUsesXOnXOffAutomatically)
		{
			DialogResult dr;

			if (this.settingsInEdit.Terminal.IOSettingsDependentSettingsWereDefaults(flowControlOldUsedXOnXOffAutomatically))
			{
				dr = DialogResult.Yes; // Update silently.
			}
			else // Update only if confirmed by the user.
			{
				if (flowControlNewUsesXOnXOffAutomatically)
				{
					var message = new StringBuilder();
					message.AppendLine("Flow control has changed to use XOn/XOff automatically. Shall XOn/XOff related settings be changed accordingly?");
					message.AppendLine("");
					message.Append    ("Confirming [Yes] will enable [Hide XOn/XOff] in [Advanced Settings...].");

					dr = MessageBoxEx.Show
					(
						this,
						message.ToString(),
						"Change Related Settings?",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question
					);
				}
				else // flowControlOldUsedXOnXOffAutomatically
				{
					var message = new StringBuilder();
					message.AppendLine("Flow control has changed to no longer use XOn/XOff automatically. Shall XOn/XOff related settings be changed accordingly?");
					message.AppendLine("");
					message.Append    ("Confirming [Yes] will disable [Hide XOn/XOff] in [Advanced Settings...].");

					dr = MessageBoxEx.Show
					(
						this,
						message.ToString(),
						"Change Related Settings?",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question
					);
				}
			}

			if (dr == DialogResult.Yes)
			{
				this.settingsInEdit.Terminal.UpdateIOSettingsDependentSettings(flowControlNewUsesXOnXOffAutomatically);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
