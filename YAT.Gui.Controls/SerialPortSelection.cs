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
using MKY.IO.Ports;

using YAT.Settings.Application;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("PortIdChanged")]
	public partial class SerialPortSelection : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private SerialPortId _portId = SerialPortId.DefaultPort;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the PortId property is changed.")]
		public event EventHandler PortIdChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public SerialPortSelection()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Category("Serial Port")]
		[Description("Serial port ID.")]
		[DefaultValue(SerialPortId.FirstStandardPortNumber)]
		public SerialPortId PortId
		{
			get { return (_portId); }
			set
			{
				if (_portId != value)
				{
					_portId = value;
					SetControls();
					OnPortIdChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		private void SerialPortSelection_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// Initially set controls and validate its contents where needed
				SetSerialPortList();
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Port_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				// \attention
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				SerialPortId id = comboBox_Port.SelectedItem as SerialPortId;
				if ((id != null) && (id.ToString() == comboBox_Port.Text))
				{
					PortId = id;
				}
				else if (SerialPortId.TryParse(comboBox_Port.Text, out id))
				{
					PortId = id;
				}
				else if (comboBox_Port.Text == "")
                {
                    PortId = null;
                }
                else
                {
                    MessageBox.Show
                        (
                        this,
                        "Serial port name is invalid",
                        "Invalid Input",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                        );
                    e.Cancel = true;
                }
			}
		}

		private void button_RefreshPorts_Click(object sender, EventArgs e)
		{
			SetSerialPortList();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private class MarkPortsInUseThread
		{
			private SerialPortCollection _portList;
			private bool _isScanning = true;
			private string _status2 = "";
			private bool _cancelScanning = false;

			public MarkPortsInUseThread(SerialPortCollection portList)
			{
				_portList = portList;
			}

			public SerialPortCollection PortList
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

			public void MarkPortsInUse()
			{
				_portList.MarkPortsInUse(portList_MarkPortsInUseCallback);
				_isScanning = false;

				StatusBox.AcceptAndClose();
			}

			public void CancelScanning()
			{
				_cancelScanning = true;
			}

			private void portList_MarkPortsInUseCallback(object sender, SerialPortCollection.PortChangedAndCancelEventArgs e)
			{
				_status2 = "Scanning " + e.Port + "...";
				StatusBox.UpdateStatus2(_status2);
				e.Cancel = _cancelScanning;
			}
		}

		private MarkPortsInUseThread _markPortsInUseThread;

		private void timer_ShowScanDialog_Tick(object sender, EventArgs e)
		{
			timer_ShowScanDialog.Stop();

			bool setting = ApplicationSettings.LocalUser.General.DetectSerialPortsInUse;

			if (StatusBox.Show(this, "Scanning ports...", "Serial Port Scan", _markPortsInUseThread.Status2, "&Detect ports that are in use", ref setting) != DialogResult.OK)
				_markPortsInUseThread.CancelScanning();
			
			ApplicationSettings.LocalUser.General.DetectSerialPortsInUse = setting;
			ApplicationSettings.Save();
		}

		private void SetSerialPortList()
		{
            if (Enabled && !DesignMode)
			{
				_isSettingControls = true;

				SerialPortId old = comboBox_Port.SelectedItem as SerialPortId;

				SerialPortCollection ports = new SerialPortCollection();
				ports.FillWithAvailablePorts(true);

				if (ApplicationSettings.LocalUser.General.DetectSerialPortsInUse)
				{
					// Install timer which shows a dialog if scanning takes more than 500ms
					timer_ShowScanDialog.Start();

					// Start scanning on different thread
					_markPortsInUseThread = new MarkPortsInUseThread(ports);
					Thread t = new Thread(new ThreadStart(_markPortsInUseThread.MarkPortsInUse));
					t.Start();

					while (_markPortsInUseThread.IsScanning)
						Application.DoEvents();

					t.Join();

					// Cleanup
					timer_ShowScanDialog.Stop();
				}

				comboBox_Port.Items.Clear();
				comboBox_Port.Items.AddRange(ports.ToArray());

				if (comboBox_Port.Items.Count > 0)
				{
					if ((_portId != null) && (ports.Contains(_portId)))
						comboBox_Port.SelectedItem = _portId;
					else if ((old != null) && (ports.Contains(old)))
						comboBox_Port.SelectedItem = old;
					else
						comboBox_Port.SelectedIndex = 0;

					// set property instead of member to ensure that changed event is fired
					PortId = comboBox_Port.SelectedItem as SerialPortId;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"No serial COM ports available, check serial COM port system settings.",
						"No COM Ports",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
				}

				_isSettingControls = false;
			}
		}

		private void SetControls()
		{
			_isSettingControls = true;

            if ((comboBox_Port.Items.Count > 0) && !DesignMode)
            {
				if (_portId != null)
					comboBox_Port.SelectedItem = _portId;
				else
					comboBox_Port.SelectedIndex = 0;
			}
			else
			{
				comboBox_Port.SelectedIndex = -1;
			}

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnPortIdChanged(EventArgs e)
		{
			EventHelper.FireSync(PortIdChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
