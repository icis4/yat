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
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
using System.ComponentModel;
using System.Windows.Forms;

using MKY;
using MKY.IO.Usb;
using MKY.Windows.Forms;

using YAT.Settings.Application;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("DeviceInfoChanged")]
	public partial class UsbSerialHidDeviceSelection : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// Only set device list and controls once as soon as this control is enabled. This saves
		/// some time on startup since scanning for the devices may take some time.
		/// </summary>
		private bool deviceListIsBeingSetOrIsAlreadySet; // = false;

		private SettingControlsHelper isSettingControls;

		private DeviceInfo deviceInfo; // = null;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the DeviceInfo property is changed.")]
		public event EventHandler DeviceInfoChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public UsbSerialHidDeviceSelection()
		{
			InitializeComponent();

			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("USB Device")]
		[Description("USB device info.")]
		public DeviceInfo DeviceInfo
		{
			get { return (this.deviceInfo); }
			set
			{
				// Don't accept to set the device to null/nothing. Master is the device list. If
				// devices are available, there is always a device selected.
				if (value != null)
				{
					if (this.deviceInfo != value)
					{
						this.deviceInfo = value;
						SetDeviceSelection();
						OnDeviceInfoChanged(EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the device selection is a valid device.
		/// </summary>
		public bool IsValid
		{
			get { return (this.deviceInfo != null); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void RefreshDeviceList()
		{
			SetDeviceList();
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void UsbSerialHidPortSelection_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetDeviceSelection();
			}

			// Ensure that device list is set as soon as this control gets enabled. Could
			// also be implemented in a EnabledChanged event handler. However, it's easier
			// to implement this here so it also done on initial 'Paint' event.
			if (Enabled && !this.deviceListIsBeingSetOrIsAlreadySet)
				SetDeviceList();
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void UsbSerialHidDeviceSelection_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetDeviceSelection();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Device_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				DeviceInfo = (comboBox_Device.SelectedItem as DeviceInfo);
		}

		private void button_RefreshPorts_Click(object sender, EventArgs e)
		{
			RefreshDeviceList();
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		// MKY.Diagnostics.DebugEx.WriteStack(Type type)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		// YAT.View.Controls.UsbSerialHidDeviceSelection.SetDeviceList()
		// YAT.View.Controls.UsbSerialHidDeviceSelection.UsbSerialHidPortSelection_Paint(Object sender, PaintEventArgs e)
		// System.Windows.Forms.Control.PaintWithErrorHandling(PaintEventArgs e, Int16 layer, Boolean disposeEventArgs)
		// System.Windows.Forms.Control.WmPaint(Message m)
		// System.Windows.Forms.Control.WndProc(Message m)
		// System.Windows.Forms.Control.ControlNativeWindow.WndProc(Message m)
		// System.Windows.Forms.NativeWindow.DebuggableCallback(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)
		// System.Windows.Forms.MessageBox.ShowCore(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, Boolean showHelp)
		// System.Windows.Forms.MessageBox.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		// YAT.View.Controls.UsbSerialHidDeviceSelection.SetDeviceList()
		// YAT.View.Controls.UsbSerialHidDeviceSelection.RefreshDeviceList()

		/// <remarks>
		/// Without precaution, and in case of no devices, the message box may appear twice due to
		/// the recursion described above (out of doc tag due to words not recognized by StyleCop).
		/// This issue is fixed by setting 'deviceListIsBeingSetOrIsAlreadySet' upon entering this method.
		/// 
		/// Note that the same fix has been implemented in <see cref="SerialPortSelection"/> and <see cref="SocketSelection"/>.
		/// </remarks>
		[ModalBehavior(ModalBehavior.InCaseOfNonUserError, Approval = "Is only called when displaying or refreshing the control on a form.")]
		private void SetDeviceList()
		{
			// Only scan for devices if control is enabled. This saves some time and prevents issues.
			if (Enabled && !DesignMode)
			{
				ResetOnDialogMessage();

				this.deviceListIsBeingSetOrIsAlreadySet = true; // Purpose see remarks above.

				SerialHidDeviceCollection devices = new SerialHidDeviceCollection();
				devices.FillWithAvailableDevices(true); // Retrieve strings from devices in order to get serial strings.

				// Attention:
				// Similar code exists in Model.Terminal.ValidateIO().
				// Changes here may have to be applied there too!

				this.isSettingControls.Enter();

				comboBox_Device.Items.Clear();

				if (devices.Count > 0)
				{
					comboBox_Device.Items.AddRange(devices.ToArray());

					if ((this.deviceInfo != null) && (devices.Contains(this.deviceInfo)))
					{
						// Nothing has changed, just restore the selected item:
						comboBox_Device.SelectedItem = this.deviceInfo;
					}
					else if ((this.deviceInfo != null) && (devices.ContainsVidPid(this.deviceInfo)))
					{
						// A device with same VID/PID is available, use that:
						int sameVidPidIndex = devices.FindIndexVidPid(this.deviceInfo);

						// Inform the user if serial is required:
						if (ApplicationSettings.LocalUserSettings.General.MatchUsbSerial)
						{
							// Get the 'NotAvailable' string BEFORE defaulting!
							string deviceInfoNotAvailable = null;
							if (this.deviceInfo != null)
								deviceInfoNotAvailable = this.deviceInfo;

							// Ensure that the settings item is switched and shown by SetControls().
							// Set property instead of member to ensure that changed event is fired.
							DeviceInfo = devices[sameVidPidIndex];

							if (!string.IsNullOrEmpty(deviceInfoNotAvailable))
								ShowNotAvailableSwitchedMessage(deviceInfoNotAvailable, devices[sameVidPidIndex]);
						}
						else
						{
							// Ensure that the settings item is defaulted and shown by SetControls().
							// Set property instead of member to ensure that changed event is fired.
							DeviceInfo = devices[sameVidPidIndex];
						}
					}
					else // devices.Count == 0
					{
						// Get the 'NotAvailable' string BEFORE defaulting!
						string deviceInfoNotAvailable = null;
						if (this.deviceInfo != null)
							deviceInfoNotAvailable = this.deviceInfo;

						// Ensure that the settings item is defaulted and shown by SetControls().
						// Set property instead of member to ensure that changed event is fired.
						DeviceInfo = devices[0];

						if (!string.IsNullOrEmpty(deviceInfoNotAvailable))
							ShowNotAvailableDefaultedMessage(deviceInfoNotAvailable, devices[0]);
					}
				}
				else
				{
					// Ensure that the settings item is nulled and reset by SetControls().
					// Set property instead of member to ensure that changed event is fired.
					DeviceInfo = null;

					ShowNoDevicesMessage();
				}

				this.isSettingControls.Leave();
			}
		}

		private void ResetOnDialogMessage()
		{
			label_OnDialogMessage.Text = "";
		}

		/// <remarks>
		/// Showing this as on dialog message instead of <see cref="MessageBox"/> to reduce the number of potentially annoying popups.
		/// </remarks>
		private void ShowNoDevicesMessage()
		{
			label_OnDialogMessage.Text = "No HID capable USB devices currently available";
		}

		private void ShowNotAvailableDefaultedMessage(string deviceInfoNotAvailable, string deviceInfoDefaulted)
		{
			string message =
				"The previous device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"The selection has been defaulted to the first available device '" + deviceInfoDefaulted + "'.";

			MessageBoxEx.Show
			(
				this,
				message,
				"Previous USB HID device not available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		private void ShowNotAvailableSwitchedMessage(string deviceInfoNotAvailable, string deviceInfoSwitched)
		{
			string message =
				"The previous device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"The selection has been switched to '" + deviceInfoSwitched + "' (first available device with previous VID and PID).";

			MessageBoxEx.Show
			(
				this,
				message,
				"Previous USB HID device not available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		private void SetDeviceSelection()
		{
			this.isSettingControls.Enter();

			if (!DesignMode && Enabled && (this.deviceInfo != null))
				SelectionHelper.Select(comboBox_Device, this.deviceInfo, this.deviceInfo);
			else
				SelectionHelper.Deselect(comboBox_Device);

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnDeviceInfoChanged(EventArgs e)
		{
			EventHelper.FireSync(DeviceInfoChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
