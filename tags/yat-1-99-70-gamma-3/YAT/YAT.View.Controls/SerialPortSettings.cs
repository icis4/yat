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
// YAT 2.0 Gamma 3 Version 1.99.70
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
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("BaudRateChanged")]
	public partial class SerialPortSettings : UserControl
	{
		// \fixme
		// This settings control should be simplified to use SerialPort.SerialCommunicationSettings
		// instead of replicating all contained items.

		// \fixme
		// This settings control should use Ports.BaudRate instead of int. Same applies to the
		// NewTerminal and TerminalSettings forms. No clue why this shouldn't work...

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int                                        BaudRateDefault     = MKY.IO.Serial.SerialPort.SerialCommunicationSettings.BaudRateDefault;
		private const MKY.IO.Ports.DataBits                      DataBitsDefault     = MKY.IO.Serial.SerialPort.SerialCommunicationSettings.DataBitsDefault;
		private const System.IO.Ports.Parity                     ParityDefault       = MKY.IO.Serial.SerialPort.SerialCommunicationSettings.ParityDefault;
		private const System.IO.Ports.StopBits                   StopBitsDefault     = MKY.IO.Serial.SerialPort.SerialCommunicationSettings.StopBitsDefault;
		private const MKY.IO.Serial.SerialPort.SerialFlowControl FlowControlDefault  = MKY.IO.Serial.SerialPort.SerialCommunicationSettings.FlowControlDefault;
		private static readonly MKY.IO.Serial.AutoInterval       AliveMonitorDefault = MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorDefault;
		private static readonly MKY.IO.Serial.AutoInterval       AutoReopenDefault   = MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private int                                        baudRate     = BaudRateDefault;
		private MKY.IO.Ports.DataBits                      dataBits     = DataBitsDefault;
		private System.IO.Ports.Parity                     parity       = ParityDefault;
		private System.IO.Ports.StopBits                   stopBits     = StopBitsDefault;
		private MKY.IO.Serial.SerialPort.SerialFlowControl flowControl  = FlowControlDefault;
		private MKY.IO.Serial.AutoInterval                 aliveMonitor = AliveMonitorDefault;
		private MKY.IO.Serial.AutoInterval                 autoReopen   = AutoReopenDefault;

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
		[Description("Event raised when the AliveMonitor property is changed.")]
		public event EventHandler AliveMonitorChanged;

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
			//// SetControls() is initially called in the 'Paint' event handler.
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
				if (this.baudRate != value)
				{
					this.baudRate = value;
					SetControls();
					OnBaudRateChanged(EventArgs.Empty);
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
				if (this.dataBits != value)
				{
					this.dataBits = value;
					SetControls();
					OnDataBitsChanged(EventArgs.Empty);
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
				if (this.parity != value)
				{
					this.parity = value;
					SetControls();
					OnParityChanged(EventArgs.Empty);
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
				if (this.stopBits != value)
				{
					this.stopBits = value;
					SetControls();
					OnStopBitsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("The flow control type.")]
		[DefaultValue(FlowControlDefault)]
		public virtual MKY.IO.Serial.SerialPort.SerialFlowControl FlowControl
		{
			get { return (this.flowControl); }
			set
			{
				if (this.flowControl != value)
				{
					this.flowControl = value;
					SetControls();
					OnFlowControlChanged(EventArgs.Empty);
				}
			}
		}

		/// <remarks>
		/// Structs cannot be used with the designer.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual MKY.IO.Serial.AutoInterval AliveMonitor
		{
			get { return (this.aliveMonitor); }
			set
			{
				if (this.aliveMonitor != value)
				{
					this.aliveMonitor = value;
					SetControls();
					OnAliveMonitorChanged(EventArgs.Empty);
				}
			}
		}

		/// <remarks>
		/// Structs cannot be used with the designer.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual MKY.IO.Serial.AutoInterval AutoReopen
		{
			get { return (this.autoReopen); }
			set
			{
				if (this.autoReopen != value)
				{
					this.autoReopen = value;
					SetControls();
					OnAutoReopenChanged(EventArgs.Empty);
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
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
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

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_BaudRate_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				// Attention:
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				int intBaudRate;
				if (int.TryParse(comboBox_BaudRate.Text, out intBaudRate) && (intBaudRate > 0))
				{
					BaudRate = (MKY.IO.Ports.BaudRateEx)intBaudRate;
				}
				else
				{
					MessageBoxEx.Show
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
				FlowControl = (MKY.IO.Serial.SerialPort.SerialFlowControlEx)comboBox_FlowControl.SelectedItem;
		}

		private void checkBox_AliveMonitor_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				MKY.IO.Serial.AutoInterval ar = AliveMonitor;
				ar.Enabled = checkBox_AliveMonitor.Checked;
				AliveMonitor = ar;
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_AliveMonitorInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_AliveMonitorInterval.Text, out interval) && (interval >= MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorMinInterval))
				{
					MKY.IO.Serial.AutoInterval ar = AliveMonitor;
					ar.Interval = interval;
					AliveMonitor = ar;
				}
				else
				{
					MessageBoxEx.Show
					(
						this,
						"Monitoring interval must be at least " + MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorMinInterval + " ms!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					e.Cancel = true;
				}
			}
		}

		private void checkBox_AutoReopen_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				MKY.IO.Serial.AutoInterval ar = AutoReopen;
				ar.Enabled = checkBox_AutoReopen.Checked;
				AutoReopen = ar;
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_AutoReopenInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_AutoReopenInterval.Text, out interval) && (interval >= MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenMinInterval))
				{
					MKY.IO.Serial.AutoInterval ar = AutoReopen;
					ar.Interval = interval;
					AutoReopen = ar;
				}
				else
				{
					MessageBoxEx.Show
					(
						this,
						"Reopen interval must be at least " + MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenMinInterval + " ms!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					e.Cancel = true;
				}
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls.Enter();

			comboBox_BaudRate.Items.AddRange   (MKY.IO.Ports.BaudRateEx.GetItems());
			comboBox_DataBits.Items.AddRange   (MKY.IO.Ports.DataBitsEx.GetItems());
			comboBox_Parity.Items.AddRange     (MKY.IO.Ports.ParityEx.GetItems());
			comboBox_StopBits.Items.AddRange   (MKY.IO.Ports.StopBitsEx.GetItems());
			comboBox_FlowControl.Items.AddRange(MKY.IO.Serial.SerialPort.SerialFlowControlEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			MKY.IO.Ports.BaudRateEx baudRate = this.baudRate;
			if (Enabled && (baudRate == MKY.IO.Ports.BaudRate.Explicit))
				comboBox_BaudRate.Text = baudRate;
			else if (Enabled)
				comboBox_BaudRate.SelectedItem = baudRate;
			else
				comboBox_BaudRate.SelectedIndex = ControlEx.InvalidIndex;

			if (Enabled)
			{
				comboBox_DataBits.SelectedItem    = (MKY.IO.Ports.DataBitsEx)this.dataBits;
				comboBox_Parity.SelectedItem      = (MKY.IO.Ports.ParityEx)this.parity;
				comboBox_StopBits.SelectedItem    = (MKY.IO.Ports.StopBitsEx)this.stopBits;
				comboBox_FlowControl.SelectedItem = (MKY.IO.Serial.SerialPort.SerialFlowControlEx)this.flowControl;
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
				bool aliveMonitorEnabled             = this.aliveMonitor.Enabled;
				checkBox_AliveMonitor.Checked        = aliveMonitorEnabled;
				textBox_AliveMonitorInterval.Enabled = aliveMonitorEnabled;
				textBox_AliveMonitorInterval.Text    = this.aliveMonitor.Interval.ToString(CultureInfo.CurrentCulture);

				bool autoReopenEnabled             = this.autoReopen.Enabled;
				checkBox_AutoReopen.Checked        = autoReopenEnabled;
				textBox_AutoReopenInterval.Enabled = autoReopenEnabled;
				textBox_AutoReopenInterval.Text    = this.autoReopen.Interval.ToString(CultureInfo.CurrentCulture);
			}
			else
			{
				checkBox_AliveMonitor.Checked        = false;
				textBox_AliveMonitorInterval.Enabled = false;
				textBox_AliveMonitorInterval.Text    = "";

				checkBox_AutoReopen.Checked        = false;
				textBox_AutoReopenInterval.Enabled = false;
				textBox_AutoReopenInterval.Text    = "";
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
		protected virtual void OnAliveMonitorChanged(EventArgs e)
		{
			EventHelper.FireSync(AliveMonitorChanged, this, e);
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