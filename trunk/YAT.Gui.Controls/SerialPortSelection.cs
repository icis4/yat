using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using HSR.IO.Ports;

namespace HSR.YAT.Gui.Controls
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
		// Attributes
		//------------------------------------------------------------------------------------------

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

			SetSerialPortList();
			SetControls();
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

		private void SetSerialPortList()
		{
			_isSettingControls = true;

			SerialPortId old = (SerialPortId)comboBox_Port.SelectedItem;

			SerialPortList portList = new SerialPortList();
			portList.FillWithAvailablePorts();
			portList.MarkPortsInUse();

			comboBox_Port.Items.Clear();
			comboBox_Port.Items.AddRange(portList.ToArray());

			if (comboBox_Port.Items.Count > 0)
			{
				if (old != null)
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
