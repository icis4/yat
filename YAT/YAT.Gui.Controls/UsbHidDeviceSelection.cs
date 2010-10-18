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
using System.ComponentModel;
using System.Windows.Forms;

using MKY.IO.Usb;
using MKY.Event;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("DeviceInfoChanged")]
	public partial class UsbHidDeviceSelection : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		private DeviceInfo deviceInfo = null;

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
		public UsbHidDeviceSelection()
		{
			InitializeComponent();
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
					if (value != this.deviceInfo)
					{
						this.deviceInfo = value;
						SetControls();
						OnDeviceInfoChanged(new EventArgs());
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

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Only set device list and controls once as soon as this control is enabled. This saves
		/// some time on startup since scanning for the ports takes quite some time.
		/// </summary>
		private bool deviceListIsInitialized = false;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void UsbHidPortSelection_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
			}

			// Ensure that device list is set as soon as this control gets enabled.
			// Could also be implemented in a EnabledChanged event handler. However, it's easier
			// to implement this here so it also done on initial Paint event.
			if (Enabled && !this.deviceListIsInitialized)
			{
				this.deviceListIsInitialized = true;
				SetDeviceList();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void UsbHidDeviceSelection_EnabledChanged(object sender, EventArgs e)
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
				DeviceInfo = comboBox_Device.SelectedItem as DeviceInfo;
		}

		private void button_RefreshPorts_Click(object sender, EventArgs e)
		{
			SetDeviceList();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetDeviceList()
		{
			// Only scan for ports if control is enabled. This saves some time.
			if (Enabled && !DesignMode)
			{
				this.isSettingControls = true;

				DeviceInfo old = comboBox_Device.SelectedItem as DeviceInfo;

				SerialHidDeviceCollection devices = new SerialHidDeviceCollection();
				devices.FillWithAvailableDevices();

				comboBox_Device.Items.Clear();
				comboBox_Device.Items.AddRange(devices.ToArray());

				if (comboBox_Device.Items.Count > 0)
				{
					if ((this.deviceInfo != null) && (devices.Contains(this.deviceInfo)))
						comboBox_Device.SelectedItem = this.deviceInfo;
					else if ((old != null) && (devices.Contains(old)))
						comboBox_Device.SelectedItem = old;
					else
						comboBox_Device.SelectedIndex = 0;

					// Set property instead of member to ensure that changed event is fired.
					DeviceInfo = comboBox_Device.SelectedItem as DeviceInfo;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"No Ser/HID capable USB devices available.",
						"No USB Ser/HID Devices",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
				}

				this.isSettingControls = false;
			}
		}

		private void SetControls()
		{
			this.isSettingControls = true;

			if (!DesignMode && Enabled && (comboBox_Device.Items.Count > 0))
			{
				if (this.deviceInfo != null)
					comboBox_Device.SelectedItem = this.deviceInfo;
				else
					comboBox_Device.SelectedIndex = 0;
			}
			else
			{
				comboBox_Device.SelectedIndex = -1;
			}

			this.isSettingControls = false;
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
