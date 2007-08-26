using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using MKY.IO.Ports;
using MKY.YAT.Settings.Application;

namespace MKY.YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("PortIdChanged")]
	public partial class SerialPortSelection : UserControl
	{
		//------------------------------------------------------------------------------------------
		// Constants
		//------------------------------------------------------------------------------------------

		private const bool _ShowSerialPortDefault = true;

		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private bool _showSerialPort = _ShowSerialPortDefault;

		private SerialPortId _portId = SerialPortId.DefaultPort;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		[Category("Property Changed")]
		[Description("Event raised when the PortId property is changed.")]
		public event EventHandler PortIdChanged;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public SerialPortSelection()
		{
			InitializeComponent();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[Browsable(false)]
		[DefaultValue(_ShowSerialPortDefault)]
		public bool ShowSerialPort
		{
			set
			{
				if (_showSerialPort != value)
				{
					_showSerialPort = value;
					SetControls();
				}
			}
		}

		[Category("Serial Port")]
		[Description("Serial port ID.")]
		[DefaultValue(SerialPortId.DefaultPortNumber)]
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
		//------------------------------------------------------------------------------------------
		// Control Event Handlers
		//------------------------------------------------------------------------------------------

		private void SerialPortSelection_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// initially set controls and validate its contents where needed
				SetSerialPortList();
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void comboBox_Port_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				PortId = (SerialPortId)comboBox_Port.SelectedItem;
		}

		private void button_RefreshPorts_Click(object sender, EventArgs e)
		{
			SetSerialPortList();
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private class MarkPortsInUseThread
		{
			private SerialPortList _portList;
			private bool _isScanning = true;
			private string _status2 = "";
			private bool _cancelScanning = false;

			public MarkPortsInUseThread(SerialPortList portList)
			{
				_portList = portList;
			}

			public SerialPortList PortList
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

				Windows.Forms.StatusBox.AcceptAndClose();
			}

			public void CancelScanning()
			{
				_cancelScanning = true;
			}

			private void portList_MarkPortsInUseCallback(object sender, SerialPortList.PortChangedAndCancelEventArgs e)
			{
				_status2 = "Scanning " + e.Port.ToString() + "...";
				Windows.Forms.StatusBox.UpdateStatus2(_status2);
				e.Cancel = _cancelScanning;
			}
		}

		private MarkPortsInUseThread _markPortsInUseThread;

		private void timer_ShowScanDialog_Tick(object sender, EventArgs e)
		{
			timer_ShowScanDialog.Stop();

			bool setting = ApplicationSettings.LocalUser.General.DetectSerialPortsInUse;

			if (Windows.Forms.StatusBox.Show(this, "Scanning ports...", "Serial Port Scan", _markPortsInUseThread.Status2, "&Detect ports that are in use", ref setting) != DialogResult.OK)
				_markPortsInUseThread.CancelScanning();
			
			ApplicationSettings.LocalUser.General.DetectSerialPortsInUse = setting;
			ApplicationSettings.SaveLocalUser();
		}

		private void SetSerialPortList()
		{
			_isSettingControls = true;

			SerialPortId old = (SerialPortId)comboBox_Port.SelectedItem;

			SerialPortList portList = new SerialPortList();
			portList.FillWithAvailablePorts();

			if (!DesignMode && ApplicationSettings.LocalUser.General.DetectSerialPortsInUse)
			{
				// install timer which shows a dialog if scanning takes more than 500ms
				timer_ShowScanDialog.Start();

				// start scanning on different thread
				_markPortsInUseThread = new MarkPortsInUseThread(portList);
				Thread t = new Thread(new ThreadStart(_markPortsInUseThread.MarkPortsInUse));
				t.Start();

				while (_markPortsInUseThread.IsScanning)
					Application.DoEvents();

				t.Join();

				// cleanup
				timer_ShowScanDialog.Stop();
			}

			comboBox_Port.Items.Clear();
			comboBox_Port.Items.AddRange(portList.ToArray());

			if (comboBox_Port.Items.Count > 0)
			{
				if (_portId != null)
					comboBox_Port.SelectedItem = _portId;
				else if (old != null)
					comboBox_Port.SelectedItem = old;
				else
					comboBox_Port.SelectedIndex = 0;

				_portId = (SerialPortId)comboBox_Port.SelectedItem;
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

		private void SetControls()
		{
			_isSettingControls = true;

			if (_showSerialPort)
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
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnPortIdChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(PortIdChanged, this, e);
		}

		#endregion
	}
}
