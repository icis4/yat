﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		private bool deviceListIsInitialized; // = false;

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
						SetControls();
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
			SetControls();
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
				SetControls();
			}

			// Ensure that device list is set as soon as this control gets enabled. Could
			// also be implemented in a EnabledChanged event handler. However, it's easier
			// to implement this here so it also done on initial 'Paint' event.
			if (Enabled && !this.deviceListIsInitialized)
			{
				SetDeviceList();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void UsbSerialHidDeviceSelection_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
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

		#region Private Methods
		//==========================================================================================
		// Private Methods
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
		/// This issue is fixed by setting 'this.deviceListIsInitialized' upon entering this method.
		/// 
		/// Note that the same fix has been implemented in <see cref="SerialPortSelection"/> and <see cref="SocketSelection"/>.
		/// </remarks>
		[ModalBehavior(ModalBehavior.InCaseOfNonUserError, Approval = "Is only called when displaying or refreshing the control on a form.")]
		private void SetDeviceList()
		{
			// Only scan for devices if control is enabled. This saves some time and prevents issues.
			if (Enabled && !DesignMode)
			{
				this.deviceListIsInitialized = true; // Purpose see remarks above.
				this.isSettingControls.Enter();

				SerialHidDeviceCollection devices = new SerialHidDeviceCollection();
				devices.FillWithAvailableDevices();

				comboBox_Device.Items.Clear();
				comboBox_Device.Items.AddRange(devices.ToArray());

				if (comboBox_Device.Items.Count > 0)
				{
					if ((this.deviceInfo != null) && (devices.Contains(this.deviceInfo)))
					{
						// Nothing has changed, just restore the selected item:
						comboBox_Device.SelectedItem = this.deviceInfo;
					}
					else if ((this.deviceInfo != null) && (devices.ContainsVidPid(this.deviceInfo)))
					{
						// Attention:
						// Similar code exists in Model.Terminal.ValidateIO().
						// Changes here may have to be applied there too!

						// A device with same VID/PID is available, use that:
						int sameVidPidIndex = devices.FindIndexVidPid(this.deviceInfo);

						// Inform the user if serial is required:
						if (ApplicationSettings.LocalUserSettings.General.UseUsbSerial)
						{
							// Compose the message (that includes the former device info) BEFORE switching!
							string message =
								"The previous device '" + this.deviceInfo + "' is currently not available." + Environment.NewLine + Environment.NewLine +
								"The selection has been switched to '" + devices[sameVidPidIndex] + "' (first available device with previous VID and PID).";

							// Ensure that the settings item is switched and shown by SetControls().
							// Set property instead of member to ensure that changed event is fired.
							DeviceInfo                    = devices[sameVidPidIndex];
							comboBox_Device.SelectedIndex         = sameVidPidIndex;

							MessageBoxEx.Show
							(
								this,
								message,
								"Previous USB HID device not available",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information
							);
						}
						else
						{
							// Ensure that the settings item is defaulted and shown by SetControls().
							// Set property instead of member to ensure that changed event is fired.
							DeviceInfo                    = devices[sameVidPidIndex];
							comboBox_Device.SelectedIndex         = sameVidPidIndex;
						}
					}
					else
					{
						// Compose the message (that includes the former device info) BEFORE defaulting!
						string message;
						if (this.deviceInfo != null)
							message =
								"The previous device '" + this.deviceInfo + "' is currently not available." + Environment.NewLine + Environment.NewLine +
								"The selection has been defaulted to the first available device '" + devices[0] + "'.";
						else
							message =
								"The previous device is currently not available." + Environment.NewLine + Environment.NewLine +
								"The selection has been defaulted to the first available device '" + devices[0] + "'.";

						// Ensure that the settings item is defaulted and shown by SetControls().
						// Set property instead of member to ensure that changed event is fired.
						DeviceInfo = devices[0];
						comboBox_Device.SelectedIndex = 0;

						MessageBoxEx.Show
						(
							this,
							message,
							"Previous USB HID device not available",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information
						);
					}
				}
				else
				{
					// Ensure that the settings item is nulled and reset by SetControls().
					// Set property instead of member to ensure that changed event is fired.
					DeviceInfo = null;

					string message =
						"There are currently no HID capable USB devices available." + Environment.NewLine +
						"Check the USB devices of your system.";

					MessageBoxEx.Show
					(
						this,
						message,
						"No USB HID devices available",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
				}

				this.isSettingControls.Leave();
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			if (!DesignMode && Enabled)
			{
				if (comboBox_Device.Items.Count > 0)
				{
					if (this.deviceInfo != null)
					{
						if (comboBox_Device.Items.Contains(this.deviceInfo))
						{	// Applies if an item of the combo box is selected.
							comboBox_Device.SelectedItem = this.deviceInfo;
						}
						else
						{	// Applies if an item that is not in the combo box is selected.
							comboBox_Device.SelectedIndex = ControlEx.InvalidIndex;
							comboBox_Device.Text = this.deviceInfo;
						}
					}
					else
					{	// Item doesn't exist, use default = first item in the combo box.
						comboBox_Device.SelectedIndex = 0;
					}
				}
				else
				{
					comboBox_Device.SelectedIndex = ControlEx.InvalidIndex;
					if (this.deviceInfo != null)
						comboBox_Device.Text = this.deviceInfo;
					else
						comboBox_Device.Text = "";
				}
			}
			else
			{
				comboBox_Device.SelectedIndex = ControlEx.InvalidIndex;
				comboBox_Device.Text = "";
			}

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
