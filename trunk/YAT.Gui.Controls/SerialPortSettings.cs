using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("BaudRateChanged")]
	public partial class SerialPortSettings : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int                      _BaudRateDefault    = (int)MKY.IO.Ports.BaudRate.Baud009600;
		private const MKY.IO.Ports.DataBits    _DataBitsDefault    = MKY.IO.Ports.DataBits.Eight;
		private const System.IO.Ports.Parity   _ParityDefault      = System.IO.Ports.Parity.None;
		private const System.IO.Ports.StopBits _StopBitsDefault    = System.IO.Ports.StopBits.One;
		private const Domain.IO.FlowControl    _FlowControlDefault = Domain.IO.FlowControl.None;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private int                      _baudRate    = _BaudRateDefault;
		private MKY.IO.Ports.DataBits    _dataBits    = _DataBitsDefault;
		private System.IO.Ports.Parity   _parity      = _ParityDefault;
		private System.IO.Ports.StopBits _stopBits    = _StopBitsDefault;
		private Domain.IO.FlowControl    _flowControl = _FlowControlDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the BaudRate property is changed.")]
		public event EventHandler BaudRateChanged;

		[Category("Property Changed")]
		[Description("Event raised when the DataBits property is changed.")]
		public event EventHandler DataBitsChanged;

		[Category("Property Changed")]
		[Description("Event raised when the Parity property is changed.")]
		public event EventHandler ParityChanged;

		[Category("Property Changed")]
		[Description("Event raised when the StopBits property is changed.")]
		public event EventHandler StopBitsChanged;

		[Category("Property Changed")]
		[Description("Event raised when the FlowControl property is changed.")]
		public event EventHandler FlowControlChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================
	
		public SerialPortSettings()
		{
			InitializeComponent();

			InitializeControls();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Category("Serial Port")]
		[Description("The baud rate.")]
		[DefaultValue(_BaudRateDefault)]
		public int BaudRate
		{
			get { return (_baudRate); }
			set
			{
				if (_baudRate != value)
				{
					_baudRate = value;
					SetControls();
					OnBaudRateChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The number of data bits.")]
		[DefaultValue(_DataBitsDefault)]
		public MKY.IO.Ports.DataBits DataBits
		{
			get { return (_dataBits); }
			set
			{
				if (_dataBits != value)
				{
					_dataBits = value;
					SetControls();
					OnDataBitsChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The partiy type.")]
		[DefaultValue(_ParityDefault)]
		public System.IO.Ports.Parity Parity
		{
			get { return (_parity); }
			set
			{
				if (_parity != value)
				{
					_parity = value;
					SetControls();
					OnParityChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The number of stop bits.")]
		[DefaultValue(_StopBitsDefault)]
		public System.IO.Ports.StopBits StopBits
		{
			get { return (_stopBits); }
			set
			{
				if (_stopBits != value)
				{
					_stopBits = value;
					SetControls();
					OnStopBitsChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The flow control type.")]
		[DefaultValue(_FlowControlDefault)]
		public Domain.IO.FlowControl FlowControl
		{
			get { return (_flowControl); }
			set
			{
				if (_flowControl != value)
				{
					_flowControl = value;
					SetControls();
					OnFlowControlChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_BaudRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				BaudRate = (MKY.IO.Ports.XBaudRate)comboBox_BaudRate.SelectedItem;
		}

		private void comboBox_BaudRate_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				MKY.IO.Ports.XBaudRate baudRate = comboBox_BaudRate.SelectedItem as MKY.IO.Ports.XBaudRate;

				if (baudRate != null)
				{
					BaudRate = baudRate;
				}
				else
				{
					int intBaudRate;
					if (int.TryParse(comboBox_BaudRate.Text, out intBaudRate) && (intBaudRate > 0))
					{
						BaudRate = (MKY.IO.Ports.XBaudRate)intBaudRate;
					}
					else
					{
						MessageBox.Show
							(
							this,
							"Baud rate must be a positive number!",
							"Invalid Input",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
							);
						e.Cancel = true;
					}
				}
			}
		}

		private void comboBox_DataBits_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				DataBits = (MKY.IO.Ports.XDataBits)comboBox_DataBits.SelectedItem;
		}

		private void comboBox_Parity_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				Parity = (MKY.IO.Ports.XParity)comboBox_Parity.SelectedItem;
		}

		private void comboBox_StopBits_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				StopBits = (MKY.IO.Ports.XStopBits)comboBox_StopBits.SelectedItem;
		}

		private void comboBox_FlowControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				FlowControl = (Domain.IO.XFlowControl)comboBox_FlowControl.SelectedItem;
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			_isSettingControls = true;

			comboBox_BaudRate.Items.AddRange(MKY.IO.Ports.XBaudRate.GetItems());
			comboBox_DataBits.Items.AddRange(MKY.IO.Ports.XDataBits.GetItems());
			comboBox_Parity.Items.AddRange(MKY.IO.Ports.XParity.GetItems());
			comboBox_StopBits.Items.AddRange(MKY.IO.Ports.XStopBits.GetItems());
			comboBox_FlowControl.Items.AddRange(Domain.IO.XFlowControl.GetItems());

			_isSettingControls = false;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			MKY.IO.Ports.XBaudRate baudRate = (MKY.IO.Ports.XBaudRate)_baudRate;
			if (baudRate != MKY.IO.Ports.BaudRate.UserDefined)
				comboBox_BaudRate.SelectedItem = baudRate;
			else
				comboBox_BaudRate.Text = _baudRate.ToString();

			comboBox_DataBits.SelectedItem  = (MKY.IO.Ports.XDataBits)_dataBits;
			comboBox_Parity.SelectedItem    = (MKY.IO.Ports.XParity)_parity;
			comboBox_StopBits.SelectedItem  = (MKY.IO.Ports.XStopBits)_stopBits;
			comboBox_FlowControl.SelectedItem = (Domain.IO.XFlowControl)_flowControl;

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnBaudRateChanged(EventArgs e)
		{
			EventHelper.FireSync(BaudRateChanged, this, e);
		}

		protected virtual void OnDataBitsChanged(EventArgs e)
		{
			EventHelper.FireSync(DataBitsChanged, this, e);
		}

		protected virtual void OnParityChanged(EventArgs e)
		{
			EventHelper.FireSync(ParityChanged, this, e);
		}

		protected virtual void OnStopBitsChanged(EventArgs e)
		{
			EventHelper.FireSync(StopBitsChanged, this, e);
		}

		protected virtual void OnFlowControlChanged(EventArgs e)
		{
			EventHelper.FireSync(FlowControlChanged, this, e);
		}

		#endregion
	}
}
