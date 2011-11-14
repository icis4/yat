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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;

using MKY.Event;
using MKY.Windows.Forms;

#endregion

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("BaudRateChanged")]
	public partial class SerialPortSettings : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int                             BaudRateDefault    = (int)MKY.IO.Ports.BaudRate.Baud009600;
		private const MKY.IO.Ports.DataBits           DataBitsDefault    = MKY.IO.Ports.DataBits.Eight;
		private const System.IO.Ports.Parity          ParityDefault      = System.IO.Ports.Parity.None;
		private const System.IO.Ports.StopBits        StopBitsDefault    = System.IO.Ports.StopBits.One;
		private const MKY.IO.Serial.SerialFlowControl FlowControlDefault = MKY.IO.Serial.SerialFlowControl.None;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private int                             baudRate    = BaudRateDefault;
		private MKY.IO.Ports.DataBits           dataBits    = DataBitsDefault;
		private System.IO.Ports.Parity          parity      = ParityDefault;
		private System.IO.Ports.StopBits        stopBits    = StopBitsDefault;
		private MKY.IO.Serial.SerialFlowControl flowControl = FlowControlDefault;
		private MKY.IO.Serial.AutoRetry         autoReopen  = MKY.IO.Serial.SerialPortSettings.AutoReopenDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the BaudRate property is changed.")]
		public event EventHandler BaudRateChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the DataBits property is changed.")]
		public event EventHandler DataBitsChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the Parity property is changed.")]
		public event EventHandler ParityChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the StopBits property is changed.")]
		public event EventHandler StopBitsChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the FlowControl property is changed.")]
		public event EventHandler FlowControlChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the AutoReopen property is changed.")]
		public event EventHandler AutoReopenChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
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

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("The baud rate.")]
		[DefaultValue(BaudRateDefault)]
		public virtual int BaudRate
		{
			get { return (this.baudRate); }
			set
			{
				if (value != this.baudRate)
				{
					this.baudRate = value;
					SetControls();
					OnBaudRateChanged(new EventArgs());
				}
			}
		}

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("The number of data bits.")]
		[DefaultValue(DataBitsDefault)]
		public virtual MKY.IO.Ports.DataBits DataBits
		{
			get { return (this.dataBits); }
			set
			{
				if (value != this.dataBits)
				{
					this.dataBits = value;
					SetControls();
					OnDataBitsChanged(new EventArgs());
				}
			}
		}

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("The partiy type.")]
		[DefaultValue(ParityDefault)]
		public virtual System.IO.Ports.Parity Parity
		{
			get { return (this.parity); }
			set
			{
				if (value != this.parity)
				{
					this.parity = value;
					SetControls();
					OnParityChanged(new EventArgs());
				}
			}
		}

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("The number of stop bits.")]
		[DefaultValue(StopBitsDefault)]
		public virtual System.IO.Ports.StopBits StopBits
		{
			get { return (this.stopBits); }
			set
			{
				if (value != this.stopBits)
				{
					this.stopBits = value;
					SetControls();
					OnStopBitsChanged(new EventArgs());
				}
			}
		}

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("The flow control type.")]
		[DefaultValue(FlowControlDefault)]
		public virtual MKY.IO.Serial.SerialFlowControl FlowControl
		{
			get { return (this.flowControl); }
			set
			{
				if (value != this.flowControl)
				{
					this.flowControl = value;
					SetControls();
					OnFlowControlChanged(new EventArgs());
				}
			}
		}

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("Auto reopen optione.")]
		public virtual MKY.IO.Serial.AutoRetry AutoReopen
		{
			get { return (this.autoReopen); }
			set
			{
				if (value != this.autoReopen)
				{
					this.autoReopen = value;
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
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void SerialPortSettings_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void SerialPortSettings_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_BaudRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				BaudRate = (MKY.IO.Ports.BaudRateEx)comboBox_BaudRate.SelectedItem;
		}

		private void comboBox_BaudRate_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				// \attention:
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				int intBaudRate;
				if (int.TryParse(comboBox_BaudRate.Text, out intBaudRate) && (intBaudRate > 0))
				{
					BaudRate = (MKY.IO.Ports.BaudRateEx)intBaudRate;
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
			if (!this.isSettingControls)
				DataBits = (MKY.IO.Ports.DataBitsEx)comboBox_DataBits.SelectedItem;
		}

		private void comboBox_Parity_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				Parity = (MKY.IO.Ports.ParityEx)comboBox_Parity.SelectedItem;
		}

		private void comboBox_StopBits_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				StopBits = (MKY.IO.Ports.StopBitsEx)comboBox_StopBits.SelectedItem;
		}

		private void comboBox_FlowControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				FlowControl = (MKY.IO.Serial.SerialFlowControlEx)comboBox_FlowControl.SelectedItem;
		}

		private void checkBox_AutoReopen_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				MKY.IO.Serial.AutoRetry ar = this.autoReopen;
				ar.Enabled = checkBox_AutoReopen.Checked;
				AutoReopen = ar;
			}
		}

		private void textBox_AutoReopenInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_AutoReopenInterval.Text, out interval) && (interval >= MKY.IO.Serial.SerialPortSettings.AutoReopenMinimumInterval))
				{
					MKY.IO.Serial.AutoRetry ar = this.autoReopen;
					ar.Interval = interval;
					AutoReopen = ar;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Reconnect interval must be at least " + MKY.IO.Serial.SerialPortSettings.AutoReopenMinimumInterval + " ms!",
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
			this.isSettingControls.Enter();

			comboBox_BaudRate.Items.AddRange(MKY.IO.Ports.BaudRateEx.GetItems());
			comboBox_DataBits.Items.AddRange(MKY.IO.Ports.DataBitsEx.GetItems());
			comboBox_Parity.Items.AddRange(MKY.IO.Ports.ParityEx.GetItems());
			comboBox_StopBits.Items.AddRange(MKY.IO.Ports.StopBitsEx.GetItems());
			comboBox_FlowControl.Items.AddRange(MKY.IO.Serial.SerialFlowControlEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			MKY.IO.Ports.BaudRateEx baudRate = (MKY.IO.Ports.BaudRateEx)this.baudRate;
			if (Enabled && (baudRate != MKY.IO.Ports.BaudRate.UserDefined))
				comboBox_BaudRate.SelectedItem = baudRate;
			else if (Enabled)
				comboBox_BaudRate.Text = this.baudRate.ToString();
			else
				comboBox_BaudRate.SelectedIndex = ControlEx.InvalidIndex;

			if (Enabled)
			{
				comboBox_DataBits.SelectedItem    = (MKY.IO.Ports.DataBitsEx)this.dataBits;
				comboBox_Parity.SelectedItem      = (MKY.IO.Ports.ParityEx)this.parity;
				comboBox_StopBits.SelectedItem    = (MKY.IO.Ports.StopBitsEx)this.stopBits;
				comboBox_FlowControl.SelectedItem = (MKY.IO.Serial.SerialFlowControlEx)this.flowControl;
			}
			else
			{
				comboBox_DataBits.SelectedIndex    = ControlEx.InvalidIndex;
				comboBox_Parity.SelectedIndex      = ControlEx.InvalidIndex;
				comboBox_StopBits.SelectedIndex    = ControlEx.InvalidIndex;
				comboBox_FlowControl.SelectedIndex = ControlEx.InvalidIndex;
			}

			if (Enabled)
			{
				bool autoReopenEnabled = this.autoReopen.Enabled;
				checkBox_AutoReopen.Checked = autoReopenEnabled;
				textBox_AutoReopenInterval.Enabled = autoReopenEnabled;
				textBox_AutoReopenInterval.Text = this.autoReopen.Interval.ToString();
			}
			else
			{
				checkBox_AutoReopen.Checked = false;
				textBox_AutoReopenInterval.Enabled = false;
				textBox_AutoReopenInterval.Text = "";
			}

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnBaudRateChanged(EventArgs e)
		{
			EventHelper.FireSync(BaudRateChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataBitsChanged(EventArgs e)
		{
			EventHelper.FireSync(DataBitsChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnParityChanged(EventArgs e)
		{
			EventHelper.FireSync(ParityChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnStopBitsChanged(EventArgs e)
		{
			EventHelper.FireSync(StopBitsChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFlowControlChanged(EventArgs e)
		{
			EventHelper.FireSync(FlowControlChanged, this, e);
		}

		/// <summary></summary>
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
