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
		private const MKY.IO.Serial.SerialFlowControl _FlowControlDefault = MKY.IO.Serial.SerialFlowControl.None;

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
		private MKY.IO.Serial.SerialFlowControl _flowControl = _FlowControlDefault;
		private MKY.IO.Serial.AutoRetry _autoReopen = MKY.IO.Serial.SerialPortSettings.AutoReopenDefault;

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

		[Category("Property Changed")]
		[Description("Event raised when the AutoReopen property is changed.")]
		public event EventHandler AutoReopenChanged;

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
		public virtual int BaudRate
		{
			get { return (_baudRate); }
			set
			{
				if (value != _baudRate)
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
		public virtual MKY.IO.Ports.DataBits DataBits
		{
			get { return (_dataBits); }
			set
			{
				if (value != _dataBits)
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
		public virtual System.IO.Ports.Parity Parity
		{
			get { return (_parity); }
			set
			{
				if (value != _parity)
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
		public virtual System.IO.Ports.StopBits StopBits
		{
			get { return (_stopBits); }
			set
			{
				if (value != _stopBits)
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
		public virtual MKY.IO.Serial.SerialFlowControl FlowControl
		{
			get { return (_flowControl); }
			set
			{
				if (value != _flowControl)
				{
					_flowControl = value;
					SetControls();
					OnFlowControlChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("Auto reopen optione.")]
		public virtual MKY.IO.Serial.AutoRetry AutoReopen
		{
			get { return (_autoReopen); }
			set
			{
				if (value != _autoReopen)
				{
					_autoReopen = value;
					SetControls();
					OnAutoReopenChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool _isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void SerialPortSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;
				SetControls();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void SerialPortSettings_EnabledChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				SetControls();
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
				// \attention
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

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
				FlowControl = (MKY.IO.Serial.XSerialFlowControl)comboBox_FlowControl.SelectedItem;
		}

		private void checkBox_AutoReopen_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				MKY.IO.Serial.AutoRetry ar = _autoReopen;
				ar.Enabled = checkBox_AutoReopen.Checked;
				AutoReopen = ar;
			}
		}

		private void textBox_AutoReopenInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_AutoReopenInterval.Text, out interval) && (interval >= 100))
				{
					MKY.IO.Serial.AutoRetry ar = _autoReopen;
					ar.Interval = interval;
					AutoReopen = ar;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Reconnect interval must be at least 100 ms!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
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
			comboBox_FlowControl.Items.AddRange(MKY.IO.Serial.XSerialFlowControl.GetItems());

			_isSettingControls = false;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			MKY.IO.Ports.XBaudRate baudRate = (MKY.IO.Ports.XBaudRate)_baudRate;
			if (Enabled && (baudRate != MKY.IO.Ports.BaudRate.UserDefined))
				comboBox_BaudRate.SelectedItem = baudRate;
			else if (Enabled)
				comboBox_BaudRate.Text = _baudRate.ToString();
			else
				comboBox_BaudRate.SelectedIndex = -1;

			if (Enabled)
			{
				comboBox_DataBits.SelectedItem    = (MKY.IO.Ports.XDataBits)_dataBits;
				comboBox_Parity.SelectedItem      = (MKY.IO.Ports.XParity)_parity;
				comboBox_StopBits.SelectedItem    = (MKY.IO.Ports.XStopBits)_stopBits;
				comboBox_FlowControl.SelectedItem = (MKY.IO.Serial.XSerialFlowControl)_flowControl;
			}
			else
			{
				comboBox_DataBits.SelectedIndex    = -1;
				comboBox_Parity.SelectedIndex      = -1;
				comboBox_StopBits.SelectedIndex    = -1;
				comboBox_FlowControl.SelectedIndex = -1;
			}

			// \fixme Auto-reopen doesn't work because of deadlock issue mentioned in SerialPort.
			if (Enabled)
			{
				checkBox_AutoReopen.Visible = false;
				textBox_AutoReopenInterval.Visible = false;
				label_AutoReopenIntervalUnit.Visible = false;
			}
			else
			{
				checkBox_AutoReopen.Visible = false;
				textBox_AutoReopenInterval.Visible = false;
				label_AutoReopenIntervalUnit.Visible = false;
			}

			if (Enabled)
			{
				bool autoReopenEnabled = _autoReopen.Enabled;
				checkBox_AutoReopen.Checked = autoReopenEnabled;
				textBox_AutoReopenInterval.Enabled = autoReopenEnabled;
				textBox_AutoReopenInterval.Text = _autoReopen.Interval.ToString();
			}
			else
			{
				checkBox_AutoReopen.Checked = false;
				textBox_AutoReopenInterval.Enabled = false;
				textBox_AutoReopenInterval.Text = "";
			}

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

		protected virtual void OnAutoReopenChanged(EventArgs e)
		{
			EventHelper.FireSync(AutoReopenChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
