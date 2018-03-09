﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Almost Final Version 1.99.95
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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

using MKY;
using MKY.Collections.Specialized;
using MKY.Windows.Forms;

using YAT.Settings.Application;

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

		private Settings.Terminal.ExplicitSettings settings;
		private Settings.Terminal.ExplicitSettings settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminalSettings"/> class.
		/// </summary>
		/// <remarks>
		/// Using <see cref="Settings.Terminal.ExplicitSettings"/> instead of simply using
		/// <see cref="Domain.Settings.TerminalSettings"/> for two reasons:
		/// <list type="bullet">
		/// <item><description>Handling of <see cref="Settings.Terminal.ExplicitSettings.UserName"/>.</description></item>
		/// <item><description>Prepared for future migration to tree view dialog containing all settings.</description></item>
		/// </list>
		/// </remarks>
		public TerminalSettings(Settings.Terminal.ExplicitSettings settings)
		{
			InitializeComponent();

			this.settings = settings;
			this.settingsInEdit = new Settings.Terminal.ExplicitSettings(settings);

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

		/// <summary></summary>
		public int TerminalId
		{
			get { return (this.terminalId); }
			set { this.terminalId = value;  }
		}

		/// <summary></summary>
		public bool TerminalIsOpen
		{
			get { return (this.terminalIsOpen); }
			set { this.terminalIsOpen = value;  }
		}

		/// <summary></summary>
		public Settings.Terminal.ExplicitSettings SettingsResult
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
			SetControls();
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var ioTypeOldWasUdpSocket = ((Domain.IOTypeEx)this.settingsInEdit.Terminal.IO.IOType).IsUdpSocket;

			this.settingsInEdit.Terminal.IO.IOType =                   terminalSelection.IOType;
			var ioTypeNewIsUdpSocket               = ((Domain.IOTypeEx)terminalSelection.IOType).IsUdpSocket;

			if (ioTypeNewIsUdpSocket != ioTypeOldWasUdpSocket)
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
						dr = MessageBoxEx.Show
						(
							this,
							"Port type has changed to UDP/IP. Shall UDP/IP related settings be changed accordingly?" + Environment.NewLine + Environment.NewLine +
							"Confirming with [Yes] will..." + Environment.NewLine +
							"...change the 'EOL sequence(s)' to [None]." + Environment.NewLine +
							"...enable 'break lines on each chunk'.",
							"Settings",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question
						);
					}
					else // ioTypeOldWasUdpSocket
					{
						dr = MessageBoxEx.Show
						(
							this,
							"Port type has changed to other than UDP/IP. Shall UDP/IP related settings be changed accordingly?" + Environment.NewLine + Environment.NewLine +
							"Confirming with [Yes] will..." + Environment.NewLine +
							"...change the 'EOL sequence(s)' to the system default." + Environment.NewLine +
							"...disable 'break lines on each chunk'.",
							"Settings",
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
			this.settingsInEdit.Terminal.IO.SerialPort.PortId = serialPortSelection.PortId;
		}

		private void serialPortSettings_BaudRateChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.SerialPort.Communication.BaudRate = serialPortSettings.BaudRate;
		}

		private void serialPortSettings_DataBitsChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.SerialPort.Communication.DataBits = serialPortSettings.DataBits;
		}

		private void serialPortSettings_ParityChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.SerialPort.Communication.Parity = serialPortSettings.Parity;
		}

		private void serialPortSettings_StopBitsChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.SerialPort.Communication.StopBits = serialPortSettings.StopBits;
		}

		private void serialPortSettings_FlowControlChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.SerialPort.Communication.FlowControl = serialPortSettings.FlowControl;
		}

		private void serialPortSettings_AliveMonitorChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.SerialPort.AliveMonitor = serialPortSettings.AliveMonitor;
		}

		private void serialPortSettings_AutoReopenChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.SerialPort.AutoReopen = serialPortSettings.AutoReopen;
		}

		#endregion

		#region Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------

		private void socketSelection_RemoteHostChanged(object sender, EventArgs e)
		{
			var host = socketSelection.RemoteHost;
			this.settingsInEdit.Terminal.IO.Socket.RemoteHost = host;
			ApplicationSettings.RoamingUserSettings.Socket.RecentRemoteHosts.Add(host);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_RemoteTcpPortChanged(object sender, EventArgs e)
		{
			var port = socketSelection.RemoteTcpPort;
			this.settingsInEdit.Terminal.IO.Socket.RemoteTcpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_RemoteUdpPortChanged(object sender, EventArgs e)
		{
			var port = socketSelection.RemoteUdpPort;
			this.settingsInEdit.Terminal.IO.Socket.RemoteUdpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.LocalInterface = socketSelection.LocalInterface;
		}

		private void socketSelection_LocalFilterChanged(object sender, EventArgs e)
		{
			var filter = socketSelection.LocalFilter;
			this.settingsInEdit.Terminal.IO.Socket.LocalFilter = filter;
			ApplicationSettings.RoamingUserSettings.Socket.RecentLocalFilters.Add(filter);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			var port = socketSelection.LocalTcpPort;
			this.settingsInEdit.Terminal.IO.Socket.LocalTcpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			var port = socketSelection.LocalUdpPort;
			this.settingsInEdit.Terminal.IO.Socket.LocalUdpPort = port;
			ApplicationSettings.RoamingUserSettings.Socket.RecentPorts.Add(port);
			ApplicationSettings.RoamingUserSettings.Socket.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		private void socketSettings_UdpServerSendModeChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.UdpServerSendMode = socketSettings.UdpServerSendMode;
		}

		#endregion

		#region Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------

		private void usbSerialHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			// Attention:
			// Same code exists in in the following location:
			//  > NewTerminal.usbSerialHidDeviceSelection_DeviceInfoChanged()
			// Changes here must be applied there too.

			MKY.IO.Usb.DeviceInfo deviceInfo = usbSerialHidDeviceSelection.DeviceInfo;
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = deviceInfo;

			// Try to automatically select one of the report format presets:
			if (deviceInfo != null)
			{
				MKY.IO.Usb.SerialHidReportFormatPresetEx preset;
				if (MKY.IO.Usb.SerialHidReportFormatPresetEx.TryParse(deviceInfo, out preset))
				{
					usbSerialHidDeviceSettings.ReportFormat  = preset.ToReportFormat();
					usbSerialHidDeviceSettings.RxFilterUsage = preset.ToRxFilterUsage();
				}
			}
		}

		private void usbSerialHidDeviceSettings_ReportFormatChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.ReportFormat = usbSerialHidDeviceSettings.ReportFormat;
		}

		private void usbSerialHidDeviceSettings_RxFilterUsageChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.RxFilterUsage = usbSerialHidDeviceSettings.RxFilterUsage;
		}

		private void usbSerialHidDeviceSettings_FlowControlChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.FlowControl = usbSerialHidDeviceSettings.FlowControl;
		}

		private void usbSerialHidDeviceSettings_AutoOpenChanged(object sender, EventArgs e)
		{
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

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.settings = this.settingsInEdit;
			this.settings.Terminal.IO.UsbSerialHidDevice.MatchSerial = ApplicationSettings.LocalUserSettings.General.MatchUsbSerial; // Defined by the LocalUserSettings.
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Emphasize line breaks.")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			string type;
			switch (this.settingsInEdit.Terminal.TerminalType)
			{
				case Domain.TerminalType.Text:   type = "Text";   break;
				case Domain.TerminalType.Binary: type = "Binary"; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.settingsInEdit.Terminal.TerminalType + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			string message =
				"Reset all settings to default values?" + Environment.NewLine +
				type + " and advanced settings will also be reset!";
				//// Actually text and binary are reset. But only one of both is in use anyway...

			if (MessageBoxEx.Show
				(
				this,
				message,
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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
						toolTipCaption = "Text terminal dependent settings such as encoding, end-of-line and comments.";
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

				serialPortSelection.PortId = this.settingsInEdit.Terminal.IO.SerialPort.PortId;
				serialPortSelection.ActivePortInUseInfo = new MKY.IO.Ports.InUseInfo
				(
					this.terminalId,
					this.settingsInEdit.Terminal.IO.SerialPort.PortId,
					this.terminalIsOpen,
					(this.terminalIsOpen ? "(in use by this terminal)" : "(selected by this terminal)")
				);

				serialPortSettings.BaudRate     = this.settingsInEdit.Terminal.IO.SerialPort.Communication.BaudRate;
				serialPortSettings.DataBits     = this.settingsInEdit.Terminal.IO.SerialPort.Communication.DataBits;
				serialPortSettings.Parity       = this.settingsInEdit.Terminal.IO.SerialPort.Communication.Parity;
				serialPortSettings.StopBits     = this.settingsInEdit.Terminal.IO.SerialPort.Communication.StopBits;
				serialPortSettings.FlowControl  = this.settingsInEdit.Terminal.IO.SerialPort.Communication.FlowControl;
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
				usbSerialHidDeviceSettings.ReportFormat  = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.ReportFormat;
				usbSerialHidDeviceSettings.RxFilterUsage = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.RxFilterUsage;
				usbSerialHidDeviceSettings.FlowControl   = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.FlowControl;
				usbSerialHidDeviceSettings.AutoOpen      = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.AutoOpen;

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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowTextOrBinarySettings()
		{
			var tt = this.settingsInEdit.Terminal.TerminalType;
			switch (tt)
			{
				case Domain.TerminalType.Text:
				{
					var f = new TextTerminalSettings(this.settingsInEdit.Terminal.TextTerminal);
					if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
					{
						Refresh();
						this.settingsInEdit.Terminal.TextTerminal = f.SettingsResult;
					}
					break;
				}

				case Domain.TerminalType.Binary:
				{
					var f = new BinaryTerminalSettings(this.settingsInEdit.Terminal.BinaryTerminal);
					if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
					{
						Refresh();
						this.settingsInEdit.Terminal.BinaryTerminal = f.SettingsResult;
					}
					break;
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + tt + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <remarks>
		/// The following list must handle the same properties as
		/// <see cref="View.Forms.AdvancedTerminalSettings.SetDefaults()"/> defaults.
		/// </remarks>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowAdvancedSettings()
		{
			var f = new AdvancedTerminalSettings(this.settingsInEdit);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				Refresh();

				// Radix:
				this.settingsInEdit.Terminal.Display.SeparateTxRxRadix = f.SettingsResult.Terminal.Display.SeparateTxRxRadix;
				this.settingsInEdit.Terminal.Display.TxRadix           = f.SettingsResult.Terminal.Display.TxRadix;
				this.settingsInEdit.Terminal.Display.RxRadix           = f.SettingsResult.Terminal.Display.RxRadix;

				// Display:
				this.settingsInEdit.Terminal.Display.ShowRadix             = f.SettingsResult.Terminal.Display.ShowRadix;
				this.settingsInEdit.Terminal.Display.ShowBufferLineNumbers = f.SettingsResult.Terminal.Display.ShowBufferLineNumbers;
				this.settingsInEdit.Terminal.Display.ShowTotalLineNumbers  = f.SettingsResult.Terminal.Display.ShowTotalLineNumbers;
				this.settingsInEdit.Terminal.Display.ShowTimeStamp         = f.SettingsResult.Terminal.Display.ShowTimeStamp;
				this.settingsInEdit.Terminal.Display.ShowTimeSpan          = f.SettingsResult.Terminal.Display.ShowTimeSpan;
				this.settingsInEdit.Terminal.Display.ShowTimeDelta         = f.SettingsResult.Terminal.Display.ShowTimeDelta;
				this.settingsInEdit.Terminal.Display.ShowPort              = f.SettingsResult.Terminal.Display.ShowPort;
				this.settingsInEdit.Terminal.Display.ShowDirection         = f.SettingsResult.Terminal.Display.ShowDirection;
				this.settingsInEdit.Terminal.Display.ShowLength            = f.SettingsResult.Terminal.Display.ShowLength;
				this.settingsInEdit.Terminal.Display.ShowCopyOfActiveLine  = f.SettingsResult.Terminal.Display.ShowCopyOfActiveLine;
				this.settingsInEdit.Terminal.Status.ShowConnectTime        = f.SettingsResult.Terminal.Status.ShowConnectTime;
				this.settingsInEdit.Terminal.Status.ShowCountAndRate       = f.SettingsResult.Terminal.Status.ShowCountAndRate;
				this.settingsInEdit.Terminal.Status.ShowFlowControlCount   = f.SettingsResult.Terminal.Status.ShowFlowControlCount;
				this.settingsInEdit.Terminal.Status.ShowBreakCount         = f.SettingsResult.Terminal.Status.ShowBreakCount;

				this.settingsInEdit.Terminal.Display.PortLineBreakEnabled      = f.SettingsResult.Terminal.Display.PortLineBreakEnabled;
				this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled = f.SettingsResult.Terminal.Display.DirectionLineBreakEnabled;
				this.settingsInEdit.Terminal.Display.ChunkLineBreakEnabled     = f.SettingsResult.Terminal.Display.ChunkLineBreakEnabled;
				this.settingsInEdit.Terminal.Display.MaxLineCount              = f.SettingsResult.Terminal.Display.MaxLineCount;
				this.settingsInEdit.Terminal.Display.MaxBytePerLineCount       = f.SettingsResult.Terminal.Display.MaxBytePerLineCount;

				// Char replace:
				this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars = f.SettingsResult.Terminal.CharReplace.ReplaceControlChars;
				this.settingsInEdit.Terminal.CharReplace.ControlCharRadix    = f.SettingsResult.Terminal.CharReplace.ControlCharRadix;
				this.settingsInEdit.Terminal.CharReplace.ReplaceTab          = f.SettingsResult.Terminal.CharReplace.ReplaceTab;
				this.settingsInEdit.Terminal.CharHide.HideXOnXOff            = f.SettingsResult.Terminal.CharHide.HideXOnXOff;
				this.settingsInEdit.Terminal.CharReplace.ReplaceSpace        = f.SettingsResult.Terminal.CharReplace.ReplaceSpace;
				this.settingsInEdit.Terminal.CharHide.Hide0x00               = f.SettingsResult.Terminal.CharHide.Hide0x00;
				this.settingsInEdit.Terminal.CharHide.Hide0xFF               = f.SettingsResult.Terminal.CharHide.Hide0xFF;

				// USB Ser/HID:
				this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData = f.SettingsResult.Terminal.IO.UsbSerialHidDevice.IncludeNonPayloadData;

				// Communication:
				this.settingsInEdit.Terminal.IO.Endianness                        = f.SettingsResult.Terminal.IO.Endianness;
				this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates     = f.SettingsResult.Terminal.IO.IndicateSerialPortBreakStates;
				this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable = f.SettingsResult.Terminal.IO.SerialPortOutputBreakIsModifiable;

				// Send:
				this.settingsInEdit.Terminal.Send.UseExplicitDefaultRadix         = f.SettingsResult.Terminal.Send.UseExplicitDefaultRadix;
				this.settingsInEdit.Terminal.Send.CopyPredefined                  = f.SettingsResult.Terminal.Send.CopyPredefined;
				this.settingsInEdit.Terminal.Send.Text.KeepSendText               = f.SettingsResult.Terminal.Send.Text.KeepSendText;
				this.settingsInEdit.Terminal.Send.Text.SendImmediately            = f.SettingsResult.Terminal.Send.Text.SendImmediately;

				this.settingsInEdit.Terminal.Send.SignalXOnBeforeEachTransmission = f.SettingsResult.Terminal.Send.SignalXOnBeforeEachTransmission;
				this.settingsInEdit.Terminal.Send.SignalXOnPeriodically           = f.SettingsResult.Terminal.Send.SignalXOnPeriodically;
				this.settingsInEdit.Terminal.IO.SerialPort.OutputBufferSize       = f.SettingsResult.Terminal.IO.SerialPort.OutputBufferSize;
				this.settingsInEdit.Terminal.IO.SerialPort.BufferMaxBaudRate      = f.SettingsResult.Terminal.IO.SerialPort.BufferMaxBaudRate;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxChunkSize           = f.SettingsResult.Terminal.IO.SerialPort.MaxChunkSize;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxSendRate            = f.SettingsResult.Terminal.IO.SerialPort.MaxSendRate;
				this.settingsInEdit.Terminal.IO.SerialPort.IgnoreFramingErrors    = f.SettingsResult.Terminal.IO.SerialPort.IgnoreFramingErrors;
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
