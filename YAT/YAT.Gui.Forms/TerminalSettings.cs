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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY.Windows.Forms;

#endregion

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class TerminalSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Settings.Terminal.ExplicitSettings settings;
		private Settings.Terminal.ExplicitSettings settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TerminalSettings(Settings.Terminal.ExplicitSettings settings)
		{
			InitializeComponent();

			this.settings = settings;
			this.settingsInEdit = new Settings.Terminal.ExplicitSettings(settings);

			// SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
		/// </remarks>
		private void TerminalSettings_Shown(object sender, EventArgs e)
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
				this.settingsInEdit.Terminal.TerminalType = terminalSelection.TerminalType;
				SetControls();
			}
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settingsInEdit.Terminal.IO.IOType = terminalSelection.IOType;
				SetControls();
			}
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
			this.settingsInEdit.Terminal.IO.Socket.RemoteHost = socketSelection.RemoteHost;
			this.settingsInEdit.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
		}

		private void socketSelection_RemoteTcpPortChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.RemoteTcpPort = socketSelection.RemoteTcpPort;
		}

		private void socketSelection_RemoteUdpPortChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.RemoteUdpPort = socketSelection.RemoteUdpPort;
		}

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.LocalInterface = socketSelection.LocalInterface;
			this.settingsInEdit.Terminal.IO.Socket.ResolvedLocalIPAddress = socketSelection.ResolvedLocalIPAddress;
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.LocalTcpPort = socketSelection.LocalTcpPort;
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.LocalUdpPort = socketSelection.LocalUdpPort;
		}

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.Socket.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		#endregion

		#region Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------

		private void usbSerialHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			MKY.IO.Usb.DeviceInfo deviceInfo = usbSerialHidDeviceSelection.DeviceInfo;
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = deviceInfo;

			// Try to automatically select one of the report format presets:
			if (deviceInfo != null)
			{
				MKY.IO.Usb.SerialHidReportFormatPresetEx preset;
				if (MKY.IO.Usb.SerialHidReportFormatPresetEx.TryParse(deviceInfo, out preset))
					usbSerialHidDeviceSettings.ReportFormat = preset.ToReportFormat();
			}
		}

		private void usbSerialHidDeviceSettings_ReportFormatChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.ReportFormat = usbSerialHidDeviceSettings.ReportFormat;
		}

		private void usbSerialHidDeviceSettings_RxIdUsageChanged(object sender, EventArgs e)
		{
			this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.RxIdUsage = usbSerialHidDeviceSettings.RxIdUsage;
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
				default: throw (new NotImplementedException("Invalid terminal type"));
			}

			string message =
				"Reset all settings to default values?" + Environment.NewLine +
				type + " and extended settings will also be reset!";

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
		/// This functionality is partly duplicated in <see cref="NewTerminal.SetControls"/>.
		/// Changes here must also be applied there.
		/// </remarks>
		private void SetControls()
		{
			this.isSettingControls.Enter();

			Domain.TerminalType terminalType = this.settingsInEdit.Terminal.TerminalType;
			terminalSelection.TerminalType = terminalType;

			string button = "&";
			string label = "";
			switch (terminalType)
			{
				case Domain.TerminalType.Text:
					button += "Text";
					label = "Text terminal dependent" + Environment.NewLine +
							"settings such as encoding," + Environment.NewLine +
							"end-of-line and comments.";
					break;

				case Domain.TerminalType.Binary:
					button += "Binary";
					label = "Binary terminal dependent" + Environment.NewLine +
							"settings such as sequence" + Environment.NewLine +
							"and timeout line breaks.";
					break;

				default:
					throw (new NotImplementedException("Invalid terminal type"));
			}
			button += " Settings...";
			button_TextOrBinarySettings.Text = button;
			label_TextOrBinarySettings.Text = label;

			Domain.IOType ioType = this.settingsInEdit.Terminal.IO.IOType;
			terminalSelection.IOType = ioType;

			bool isSerialPort   = (ioType == Domain.IOType.SerialPort);
			bool isUsbSerialHid = (ioType == Domain.IOType.UsbSerialHid);
			bool isSocket       = (!isSerialPort && !isUsbSerialHid);

			// Set socket and USB control before serial port control since that might need to refresh
			// the serial port list first (which takes time, which looks ulgy).
			socketSelection.Visible        = isSocket;
			socketSelection.HostType       = (Domain.IOTypeEx)ioType;
			socketSelection.RemoteHost     = this.settingsInEdit.Terminal.IO.Socket.RemoteHost;
			socketSelection.RemoteTcpPort  = this.settingsInEdit.Terminal.IO.Socket.RemoteTcpPort;
			socketSelection.RemoteUdpPort  = this.settingsInEdit.Terminal.IO.Socket.RemoteUdpPort;
			socketSelection.LocalInterface = this.settingsInEdit.Terminal.IO.Socket.LocalInterface;
			socketSelection.LocalTcpPort   = this.settingsInEdit.Terminal.IO.Socket.LocalTcpPort;
			socketSelection.LocalUdpPort   = this.settingsInEdit.Terminal.IO.Socket.LocalUdpPort;

			socketSettings.Visible                  = isSocket;
			socketSettings.HostType                 = (Domain.IOTypeEx)ioType;
			socketSettings.TcpClientAutoReconnect   = this.settingsInEdit.Terminal.IO.Socket.TcpClientAutoReconnect;

			usbSerialHidDeviceSelection.Visible     = isUsbSerialHid;
			usbSerialHidDeviceSelection.DeviceInfo  = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.DeviceInfo;

			usbSerialHidDeviceSettings.Visible      = isUsbSerialHid;
			usbSerialHidDeviceSettings.ReportFormat = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.ReportFormat;
			usbSerialHidDeviceSettings.RxIdUsage    = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.RxIdUsage;
			usbSerialHidDeviceSettings.AutoOpen     = this.settingsInEdit.Terminal.IO.UsbSerialHidDevice.AutoOpen;

			serialPortSelection.Visible    = isSerialPort;
			serialPortSelection.PortId     = this.settingsInEdit.Terminal.IO.SerialPort.PortId;

			serialPortSettings.Visible     = isSerialPort;
			serialPortSettings.BaudRate    = this.settingsInEdit.Terminal.IO.SerialPort.Communication.BaudRate;
			serialPortSettings.DataBits    = this.settingsInEdit.Terminal.IO.SerialPort.Communication.DataBits;
			serialPortSettings.Parity      = this.settingsInEdit.Terminal.IO.SerialPort.Communication.Parity;
			serialPortSettings.StopBits    = this.settingsInEdit.Terminal.IO.SerialPort.Communication.StopBits;
			serialPortSettings.FlowControl = this.settingsInEdit.Terminal.IO.SerialPort.Communication.FlowControl;
			serialPortSettings.AutoReopen  = this.settingsInEdit.Terminal.IO.SerialPort.AutoReopen;

			// Trigger refresh of ports/devices if selection of I/O type has changed.
			if      ((ioType == Domain.IOType.SerialPort)   && (this.SetControls_ioTypeOld != Domain.IOType.SerialPort))
				serialPortSelection.RefreshSerialPortList();
			else if ((ioType == Domain.IOType.UsbSerialHid) && (this.SetControls_ioTypeOld != Domain.IOType.UsbSerialHid))
				usbSerialHidDeviceSelection.RefreshDeviceList();

			this.SetControls_ioTypeOld = ioType;

			this.isSettingControls.Leave();
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowTextOrBinarySettings()
		{
			switch (this.settingsInEdit.Terminal.TerminalType)
			{
				case Domain.TerminalType.Text:
				{
					Gui.Forms.TextTerminalSettings f = new Gui.Forms.TextTerminalSettings(this.settingsInEdit.Terminal.TextTerminal);
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						Refresh();
						this.settingsInEdit.Terminal.TextTerminal = f.SettingsResult;
					}
					break;
				}
				case Domain.TerminalType.Binary:
				{
					Gui.Forms.BinaryTerminalSettings f = new Gui.Forms.BinaryTerminalSettings(this.settingsInEdit.Terminal.BinaryTerminal);
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						Refresh();
						this.settingsInEdit.Terminal.BinaryTerminal = f.SettingsResult;
					}
					break;
				}
				default:
				{
					throw (new NotImplementedException("Invalid terminal type"));
				}
			}
		}

		/// <remarks>
		/// The following list must handle the same properties as
		/// <see cref="Gui.Forms.AdvancedTerminalSettings.SetDefaults()"/> defaults.
		/// </remarks>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowAdvancedSettings()
		{
			Gui.Forms.AdvancedTerminalSettings f = new Gui.Forms.AdvancedTerminalSettings(this.settingsInEdit);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				// Radix:
				this.settingsInEdit.Terminal.Display.SeparateTxRxRadix = f.SettingsResult.Terminal.Display.SeparateTxRxRadix;
				this.settingsInEdit.Terminal.Display.TxRadix           = f.SettingsResult.Terminal.Display.TxRadix;
				this.settingsInEdit.Terminal.Display.RxRadix           = f.SettingsResult.Terminal.Display.RxRadix;

				// Display:
				this.settingsInEdit.Terminal.Display.ShowRadix           = f.SettingsResult.Terminal.Display.ShowRadix;
				this.settingsInEdit.Terminal.Display.ShowTimeStamp       = f.SettingsResult.Terminal.Display.ShowTimeStamp;
				this.settingsInEdit.Terminal.Display.ShowLength          = f.SettingsResult.Terminal.Display.ShowLength;
				this.settingsInEdit.Terminal.Display.ShowLineNumbers     = f.SettingsResult.Terminal.Display.ShowLineNumbers;
				this.settingsInEdit.Terminal.Status.ShowConnectTime      = f.SettingsResult.Terminal.Status.ShowConnectTime;
				this.settingsInEdit.Terminal.Status.ShowCountAndRate     = f.SettingsResult.Terminal.Status.ShowCountAndRate;
				this.settingsInEdit.Terminal.Status.ShowFlowControlCount = f.SettingsResult.Terminal.Status.ShowFlowControlCount;
				this.settingsInEdit.Terminal.Status.ShowBreakCount       = f.SettingsResult.Terminal.Status.ShowBreakCount;

				this.settingsInEdit.Terminal.Display.DirectionLineBreakEnabled = f.SettingsResult.Terminal.Display.DirectionLineBreakEnabled;
				this.settingsInEdit.Terminal.Display.TxMaxLineCount            = f.SettingsResult.Terminal.Display.TxMaxLineCount;
				this.settingsInEdit.Terminal.Display.RxMaxLineCount            = f.SettingsResult.Terminal.Display.RxMaxLineCount;

				// Char replace:
				this.settingsInEdit.Terminal.CharReplace.ReplaceControlChars = f.SettingsResult.Terminal.CharReplace.ReplaceControlChars;
				this.settingsInEdit.Terminal.CharReplace.ControlCharRadix    = f.SettingsResult.Terminal.CharReplace.ControlCharRadix;
				this.settingsInEdit.Terminal.CharReplace.ReplaceSpace        = f.SettingsResult.Terminal.CharReplace.ReplaceSpace;
				this.settingsInEdit.Terminal.CharReplace.HideXOnXOff         = f.SettingsResult.Terminal.CharReplace.HideXOnXOff;

				// Communication:
				this.settingsInEdit.Terminal.IO.Endianness                        = f.SettingsResult.Terminal.IO.Endianness;
				this.settingsInEdit.Terminal.IO.IndicateSerialPortBreakStates     = f.SettingsResult.Terminal.IO.IndicateSerialPortBreakStates;
				this.settingsInEdit.Terminal.IO.SerialPortOutputBreakIsModifiable = f.SettingsResult.Terminal.IO.SerialPortOutputBreakIsModifiable;

				// Send:
				this.settingsInEdit.Terminal.Send.KeepCommand                  = f.SettingsResult.Terminal.Send.KeepCommand;
				this.settingsInEdit.Terminal.Send.CopyPredefined               = f.SettingsResult.Terminal.Send.CopyPredefined;
				this.settingsInEdit.Terminal.Send.SendImmediately              = f.SettingsResult.Terminal.Send.SendImmediately;
				this.settingsInEdit.Terminal.IO.SerialPort.MaxSendChunkSize    = f.SettingsResult.Terminal.IO.SerialPort.MaxSendChunkSize;
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnOutputBreak = f.SettingsResult.Terminal.IO.SerialPort.NoSendOnOutputBreak;
				this.settingsInEdit.Terminal.IO.SerialPort.NoSendOnInputBreak  = f.SettingsResult.Terminal.IO.SerialPort.NoSendOnInputBreak;
				this.settingsInEdit.Terminal.Send.DefaultDelay                 = f.SettingsResult.Terminal.Send.DefaultDelay;
				this.settingsInEdit.Terminal.Send.DefaultLineDelay             = f.SettingsResult.Terminal.Send.DefaultLineDelay;
				this.settingsInEdit.Terminal.Send.DefaultLineRepeat            = f.SettingsResult.Terminal.Send.DefaultLineRepeat;
				this.settingsInEdit.Terminal.Send.DisableKeywords              = f.SettingsResult.Terminal.Send.DisableKeywords;

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
