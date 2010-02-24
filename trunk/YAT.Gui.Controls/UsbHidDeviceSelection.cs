//==================================================================================================
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using MKY.Utilities.Event;
using MKY.Windows.Forms;
using MKY.IO.Serial;

using YAT.Settings.Application;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("DeviceIdChanged")]
	public partial class UsbHidDeviceSelection : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		//private bool _isSettingControls = false;

		private UsbDeviceId _deviceId = UsbDeviceId.DefaultDevice;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
        [Description("Event raised when the DeviceId property is changed.")]
        public event EventHandler DeviceIdChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public UsbHidDeviceSelection()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Category("USB Device")]
		[Description("USB device ID.")]
		public UsbDeviceId DeviceId
		{
			get { return (_deviceId); }
			set
			{
				if (_deviceId != value)
				{
					_deviceId = value;
					SetControls();
                    OnDeviceIdChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		private void UsbHidPortSelection_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// Initially set controls and validate its contents where needed
				SetDeviceList();
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void button_RefreshPorts_Click(object sender, EventArgs e)
		{
			SetDeviceList();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private class MarkDevicesInUseThread
		{
			private UsbDeviceCollection _portList;
			private bool _isScanning = true;
			private string _status2 = "";
			private bool _cancelScanning = false;

            public MarkDevicesInUseThread(UsbDeviceCollection portList)
			{
				_portList = portList;
			}

			public UsbDeviceCollection PortList
			{
				get { return (_portList); }
			}

			public bool IsScanning
			{
				get { return (_isScanning); }
			}

			public string Status2
			{
				get { return (_status2); }
			}

			public void MarkDevicesInUse()
			{
                _portList.MarkDevicesInUse(portList_MarkDevicesInUseCallback);
				_isScanning = false;

				StatusBox.AcceptAndClose();
			}

			public void CancelScanning()
			{
				_cancelScanning = true;
			}

            private void portList_MarkDevicesInUseCallback(object sender, UsbDeviceCollection.DeviceChangedAndCancelEventArgs e)
			{
				_status2 = "Scanning " + e.Device + "...";
				StatusBox.UpdateStatus2(_status2);
				e.Cancel = _cancelScanning;
			}
		}

        private MarkDevicesInUseThread _markDevicesInUseThread;

		private void timer_ShowScanDialog_Tick(object sender, EventArgs e)
		{
			timer_ShowScanDialog.Stop();

			bool setting = ApplicationSettings.LocalUser.General.DetectSerialPortsInUse;

            if (StatusBox.Show(this, "Scanning devices...", "USB Device Scan", _markDevicesInUseThread.Status2, "&Detect ports that are in use", ref setting) != DialogResult.OK)
                _markDevicesInUseThread.CancelScanning();
			
			ApplicationSettings.LocalUser.General.DetectSerialPortsInUse = setting;
			ApplicationSettings.Save();
		}

		private void SetDeviceList()
		{
            if (Enabled && !DesignMode)
			{
				//_isSettingControls = true;

				UsbDeviceId old = comboBox_Device.SelectedItem as UsbDeviceId;

				UsbDeviceCollection devices = new UsbDeviceCollection();
				devices.FillWithAvailableDevices();

				if (ApplicationSettings.LocalUser.General.DetectSerialPortsInUse)
				{
					// Install timer which shows a dialog if scanning takes more than 500ms
					timer_ShowScanDialog.Start();

					// Start scanning on different thread
                    _markDevicesInUseThread = new MarkDevicesInUseThread(devices);
                    Thread t = new Thread(new ThreadStart(_markDevicesInUseThread.MarkDevicesInUse));
					t.Start();

                    while (_markDevicesInUseThread.IsScanning)
						Application.DoEvents();

					t.Join();

					// Cleanup
					timer_ShowScanDialog.Stop();
				}

				comboBox_Device.Items.Clear();
				comboBox_Device.Items.AddRange(devices.ToArray());

				if (comboBox_Device.Items.Count > 0)
				{
					if ((_deviceId != null) && (devices.Contains(_deviceId)))
						comboBox_Device.SelectedItem = _deviceId;
					else if ((old != null) && (devices.Contains(old)))
						comboBox_Device.SelectedItem = old;
					else
						comboBox_Device.SelectedIndex = 0;

					// set property instead of member to ensure that changed event is fired
					DeviceId = comboBox_Device.SelectedItem as UsbDeviceId;
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

				//_isSettingControls = false;
			}
		}

		private void SetControls()
		{
			//_isSettingControls = true;

            if ((comboBox_Device.Items.Count > 0) && !DesignMode)
			{
				if (_deviceId != null)
					comboBox_Device.SelectedItem = _deviceId;
                else
                    comboBox_Device.SelectedIndex = 0;
			}
			else
			{
				comboBox_Device.SelectedIndex = -1;
			}

			//_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnDeviceIdChanged(EventArgs e)
		{
            EventHelper.FireSync(DeviceIdChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
