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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
	public partial class TerminalSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Settings.Terminal.ExplicitSettings settings;
		private Settings.Terminal.ExplicitSettings settings_Form;

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
			this.settings_Form = new Settings.Terminal.ExplicitSettings(settings);

			// SetControls() is initially called in the 'Paint' event handler.
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
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void TerminalSettings_Paint(object sender, PaintEventArgs e)
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

		#region Controls Event Handlers > General
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > General
		//------------------------------------------------------------------------------------------

		private void terminalSelection_TerminalTypeChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.TerminalType tt = terminalSelection.TerminalType;
				this.settings_Form.Terminal.TerminalType = tt;
				switch (tt)
				{
					case Domain.TerminalType.Binary:
						this.settings_Form.Terminal.Display.TxRadix = Domain.Radix.Hex;
						this.settings_Form.Terminal.Display.RxRadix = Domain.Radix.Hex;
						break;

					case Domain.TerminalType.Text:
					default:
						this.settings_Form.Terminal.Display.TxRadix = Domain.Radix.String;
						this.settings_Form.Terminal.Display.RxRadix = Domain.Radix.String;
						break;
				}

				SetControls();
			}
		}

		private void terminalSelection_IOTypeChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.IOType ioType = terminalSelection.IOType;
				this.settings_Form.Terminal.IO.IOType = ioType;
				this.settings_Form.Terminal.IO.Socket.HostType = (Domain.IOTypeEx)ioType;
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
			this.settings_Form.Terminal.IO.SerialPort.PortId = serialPortSelection.PortId;
		}

		private void serialPortSettings_BaudRateChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.SerialPort.Communication.BaudRate = serialPortSettings.BaudRate;
		}

		private void serialPortSettings_DataBitsChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.SerialPort.Communication.DataBits = serialPortSettings.DataBits;
		}

		private void serialPortSettings_ParityChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.SerialPort.Communication.Parity = serialPortSettings.Parity;
		}

		private void serialPortSettings_StopBitsChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.SerialPort.Communication.StopBits = serialPortSettings.StopBits;
		}

		private void serialPortSettings_FlowControlChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.SerialPort.Communication.FlowControl = serialPortSettings.FlowControl;
		}

		private void serialPortSettings_AutoReopenChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.SerialPort.AutoReopen = serialPortSettings.AutoReopen;
		}

		#endregion

		#region Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Socket
		//------------------------------------------------------------------------------------------

		private void socketSelection_RemoteHostChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.Socket.RemoteHost = socketSelection.RemoteHost;
			this.settings_Form.Terminal.IO.Socket.ResolvedRemoteIPAddress = socketSelection.ResolvedRemoteIPAddress;
		}

		private void socketSelection_RemotePortChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.Socket.RemotePort = socketSelection.RemotePort;
		}

		private void socketSelection_LocalInterfaceChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.Socket.LocalInterface = socketSelection.LocalInterface;
			this.settings_Form.Terminal.IO.Socket.ResolvedLocalIPAddress = socketSelection.ResolvedLocalIPAddress;
		}

		private void socketSelection_LocalTcpPortChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.Socket.LocalTcpPort = socketSelection.LocalTcpPort;
		}

		private void socketSelection_LocalUdpPortChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.Socket.LocalUdpPort = socketSelection.LocalUdpPort;
		}

		private void socketSettings_TcpClientAutoReconnectChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.Socket.TcpClientAutoReconnect = socketSettings.TcpClientAutoReconnect;
		}

		#endregion

		#region Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > USB Ser/HID
		//------------------------------------------------------------------------------------------

		private void usbSerialHidDeviceSelection_DeviceInfoChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.UsbSerialHidDevice.DeviceInfo = usbSerialHidDeviceSelection.DeviceInfo;
		}

		private void usbSerialHidDeviceSettings_AutoOpenChanged(object sender, EventArgs e)
		{
			this.settings_Form.Terminal.IO.UsbSerialHidDevice.AutoOpen = usbSerialHidDeviceSettings.AutoOpen;
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
			this.settings = this.settings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Emphasize line breaks.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			string text;
			switch (this.settings_Form.Terminal.TerminalType)
			{
				case Domain.TerminalType.Text:   text = "Text";   break;
				case Domain.TerminalType.Binary: text = "Binary"; break;
				default: throw (new NotImplementedException("Invalid terminal type"));
			}
			text += " and extended";

			if (MessageBox.Show
				(
				this,
				"Reset all settings to default values?" + Environment.NewLine +
				text + " settings will also be reset!",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.settings_Form.SetDefaults();
				SetControls();
			}
		}

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
		/// This functionality is partly duplicated in <see cref="NewTerminal.SetControls"/>.
		/// Changes here must also be applied there.
		/// </remarks>
		private void SetControls()
		{
			this.isSettingControls.Enter();

			Domain.TerminalType terminalType = this.settings_Form.Terminal.TerminalType;
			terminalSelection.TerminalType = terminalType;

			string text = "&";
			switch (terminalType)
			{
				case Domain.TerminalType.Text:   text += "Text";   break;
				case Domain.TerminalType.Binary: text += "Binary"; break;
				default: throw (new NotImplementedException("Invalid terminal type"));
			}
			text += " Settings...";
			button_TextOrBinarySettings.Text = text;

			Domain.IOType ioType = this.settings_Form.Terminal.IO.IOType;
			terminalSelection.IOType = ioType;

			bool isSerialPort   = (ioType == Domain.IOType.SerialPort);
			bool isUsbSerialHid = (ioType == Domain.IOType.UsbSerialHid);
			bool isSocket       = (!isSerialPort && !isUsbSerialHid);

			// Set socket and USB control before serial port control since that might need to refresh
			// the serial port list first (which takes time, which looks ulgy).
			socketSelection.Visible        = isSocket;
			socketSelection.HostType       = (Domain.IOTypeEx)ioType;
			socketSelection.RemoteHost     = this.settings_Form.Terminal.IO.Socket.RemoteHost;
			socketSelection.RemotePort     = this.settings_Form.Terminal.IO.Socket.RemotePort;
			socketSelection.LocalInterface = this.settings_Form.Terminal.IO.Socket.LocalInterface;
			socketSelection.LocalTcpPort   = this.settings_Form.Terminal.IO.Socket.LocalTcpPort;
			socketSelection.LocalUdpPort   = this.settings_Form.Terminal.IO.Socket.LocalUdpPort;

			socketSettings.Visible         = isSocket;
			socketSettings.HostType        = (Domain.IOTypeEx)ioType;
			socketSettings.TcpClientAutoReconnect = this.settings_Form.Terminal.IO.Socket.TcpClientAutoReconnect;

			usbSerialHidDeviceSelection.Visible    = isUsbSerialHid;
			usbSerialHidDeviceSelection.DeviceInfo = this.settings_Form.Terminal.IO.UsbSerialHidDevice.DeviceInfo;

			usbSerialHidDeviceSettings.Visible  = isUsbSerialHid;
			usbSerialHidDeviceSettings.AutoOpen = this.settings_Form.Terminal.IO.UsbSerialHidDevice.AutoOpen;

			serialPortSelection.Visible    = isSerialPort;
			serialPortSelection.PortId     = this.settings_Form.Terminal.IO.SerialPort.PortId;

			serialPortSettings.Visible     = isSerialPort;
			serialPortSettings.BaudRate    = this.settings_Form.Terminal.IO.SerialPort.Communication.BaudRate;
			serialPortSettings.DataBits    = this.settings_Form.Terminal.IO.SerialPort.Communication.DataBits;
			serialPortSettings.Parity      = this.settings_Form.Terminal.IO.SerialPort.Communication.Parity;
			serialPortSettings.StopBits    = this.settings_Form.Terminal.IO.SerialPort.Communication.StopBits;
			serialPortSettings.FlowControl = this.settings_Form.Terminal.IO.SerialPort.Communication.FlowControl;
			serialPortSettings.AutoReopen  = this.settings_Form.Terminal.IO.SerialPort.AutoReopen;

			// Trigger refresh of ports/devices if selection of I/O type has changed.
			if      ((ioType == Domain.IOType.SerialPort)   && (this.SetControls_ioTypeOld != Domain.IOType.SerialPort))
				serialPortSelection.RefreshSerialPortList();
			else if ((ioType == Domain.IOType.UsbSerialHid) && (this.SetControls_ioTypeOld != Domain.IOType.UsbSerialHid))
				usbSerialHidDeviceSelection.RefreshDeviceList();

			this.SetControls_ioTypeOld = ioType;

			this.isSettingControls.Leave();
		}

		private void ShowTextOrBinarySettings()
		{
			switch (this.settings_Form.Terminal.TerminalType)
			{
				case Domain.TerminalType.Text:
				{
					Gui.Forms.TextTerminalSettings f = new Gui.Forms.TextTerminalSettings(this.settings_Form.Terminal.TextTerminal);
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						Refresh();
						this.settings_Form.Terminal.TextTerminal = f.SettingsResult;
					}
					break;
				}
				case Domain.TerminalType.Binary:
				{
					Gui.Forms.BinaryTerminalSettings f = new Gui.Forms.BinaryTerminalSettings(this.settings_Form.Terminal.BinaryTerminal);
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						Refresh();
						this.settings_Form.Terminal.BinaryTerminal = f.SettingsResult;
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
		private void ShowAdvancedSettings()
		{
			Gui.Forms.AdvancedTerminalSettings f = new Gui.Forms.AdvancedTerminalSettings(this.settings_Form);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				// Radix:
				this.settings_Form.Terminal.Display.SeparateTxRxRadix = f.SettingsResult.Terminal.Display.SeparateTxRxRadix;
				this.settings_Form.Terminal.Display.TxRadix           = f.SettingsResult.Terminal.Display.TxRadix;
				this.settings_Form.Terminal.Display.RxRadix           = f.SettingsResult.Terminal.Display.RxRadix;

				// Display:
				this.settings_Form.Terminal.Display.ShowRadix       = f.SettingsResult.Terminal.Display.ShowRadix;
				this.settings_Form.Terminal.Display.ShowTimeStamp   = f.SettingsResult.Terminal.Display.ShowTimeStamp;
				this.settings_Form.Terminal.Display.ShowLength      = f.SettingsResult.Terminal.Display.ShowLength;
				this.settings_Form.Terminal.Display.ShowLineNumbers = f.SettingsResult.Terminal.Display.ShowLineNumbers;
				this.settings_Form.Terminal.Status.ShowConnectTime  = f.SettingsResult.Terminal.Status.ShowConnectTime;
				this.settings_Form.Terminal.Status.ShowCountAndRate = f.SettingsResult.Terminal.Status.ShowCountAndRate;

				this.settings_Form.Terminal.Display.DirectionLineBreakEnabled = f.SettingsResult.Terminal.Display.DirectionLineBreakEnabled;
				this.settings_Form.Terminal.Display.TxMaxLineCount            = f.SettingsResult.Terminal.Display.TxMaxLineCount;
				this.settings_Form.Terminal.Display.RxMaxLineCount            = f.SettingsResult.Terminal.Display.RxMaxLineCount;

				// Char replace:
				this.settings_Form.Terminal.CharReplace.ReplaceControlChars = f.SettingsResult.Terminal.CharReplace.ReplaceControlChars;
				this.settings_Form.Terminal.CharReplace.ControlCharRadix    = f.SettingsResult.Terminal.CharReplace.ControlCharRadix;
				this.settings_Form.Terminal.CharReplace.ReplaceSpace        = f.SettingsResult.Terminal.CharReplace.ReplaceSpace;

				// Communication:
				this.settings_Form.Terminal.IO.Endianess                         = f.SettingsResult.Terminal.IO.Endianess;
				this.settings_Form.Terminal.IO.IndicateSerialPortBreakStates     = f.SettingsResult.Terminal.IO.IndicateSerialPortBreakStates;
				this.settings_Form.Terminal.IO.SerialPortOutputBreakIsModifiable = f.SettingsResult.Terminal.IO.SerialPortOutputBreakIsModifiable;

				// Send:
				this.settings_Form.Terminal.Send.KeepCommand                  = f.SettingsResult.Terminal.Send.KeepCommand;
				this.settings_Form.Terminal.Send.CopyPredefined               = f.SettingsResult.Terminal.Send.CopyPredefined;
				this.settings_Form.Terminal.Send.SendImmediately              = f.SettingsResult.Terminal.Send.SendImmediately;
				this.settings_Form.Terminal.IO.SerialPort.NoSendOnOutputBreak = f.SettingsResult.Terminal.IO.SerialPort.NoSendOnOutputBreak;

				// Receive:
				this.settings_Form.Terminal.IO.SerialPort.NoSendOnInputBreak = f.SettingsResult.Terminal.IO.SerialPort.NoSendOnInputBreak;

				this.settings_Form.UserName = f.SettingsResult.UserName;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
