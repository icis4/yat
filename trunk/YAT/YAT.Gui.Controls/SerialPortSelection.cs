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
using System.Threading;
using System.Windows.Forms;

using MKY.Event;
using MKY.IO.Ports;
using MKY.Windows.Forms;

using YAT.Settings.Application;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("PortIdChanged")]
	public partial class SerialPortSelection : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private class MarkPortsInUseThread
		{
			private SerialPortCollection portList;
			private bool isScanning = true;
			private string status2 = "";
			private bool cancelScanning = false;

			public MarkPortsInUseThread(SerialPortCollection portList)
			{
				this.portList = portList;
			}

			public virtual SerialPortCollection PortList
			{
				get { return (this.portList); }
			}

			public virtual bool IsScanning
			{
				get { return (this.isScanning); }
			}

			public virtual string Status2
			{
				get { return (this.status2); }
			}

			public virtual void MarkPortsInUse()
			{
				this.portList.MarkPortsInUse(portList_MarkPortsInUseCallback);
				this.isScanning = false;

				StatusBox.AcceptAndClose();
			}

			public virtual void CancelScanning()
			{
				this.cancelScanning = true;
			}

			private void portList_MarkPortsInUseCallback(object sender, SerialPortCollection.PortChangedAndCancelEventArgs e)
			{
				this.status2 = "Scanning " + e.Port + "...";
				StatusBox.UpdateStatus2(this.status2);
				e.Cancel = this.cancelScanning;
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		/// <remarks>
		/// Attention: Do not use <see cref="MKY.IO.Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="MKY.IO.Ports.SerialPortId.FirstStandardPort"/>
		/// is way more performant and good enough for most use cases.
		/// </remarks>
		private SerialPortId portId = SerialPortId.FirstStandardPort;

		private MarkPortsInUseThread markPortsInUseThread;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the PortId property is changed.")]
		public event EventHandler PortIdChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialPortSelection()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Serial Port")]
		[Description("Serial port ID.")]
		[DefaultValue(SerialPortId.FirstStandardPortNumber)]
		public SerialPortId PortId
		{
			get { return (this.portId); }
			set
			{
				// Don't accept to set the device to null/nothing. Master is the device list. If
				// devices are available, there is always a device selected.
				if (value != null)
				{
					if (value != this.portId)
					{
						this.portId = value;
						SetControls();
						OnPortIdChanged(new EventArgs());
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the device selection is a valid device.
		/// </summary>
		public bool IsValid
		{
			get { return (this.portId != null); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void RefreshSerialPortList()
		{
			SetSerialPortList();
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
		private void SerialPortSelection_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetSerialPortList();
				SetControls();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void SerialPortSelection_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Port_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
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
				else if (comboBox_Port.Text.Length == 0)
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

		private void timer_ShowScanDialog_Tick(object sender, EventArgs e)
		{
			timer_ShowScanDialog.Stop();

			bool setting = ApplicationSettings.LocalUser.General.DetectSerialPortsInUse;

			if (StatusBox.Show(this, "Scanning ports...", "Serial Port Scan", this.markPortsInUseThread.Status2, "&Detect ports that are in use", ref setting) != DialogResult.OK)
				this.markPortsInUseThread.CancelScanning();

			ApplicationSettings.LocalUser.General.DetectSerialPortsInUse = setting;
			ApplicationSettings.Save();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetSerialPortList()
		{
			// Only scan for ports if control is enabled. This saves some time.
			if (Enabled && !DesignMode)
			{
				this.isSettingControls = true;

				SerialPortId old = comboBox_Port.SelectedItem as SerialPortId;

				SerialPortCollection ports = new SerialPortCollection();
				ports.FillWithAvailablePorts(true);

				if (ApplicationSettings.LocalUser.General.DetectSerialPortsInUse)
				{
					// Install timer which shows a dialog if scanning takes more than 500ms.
					timer_ShowScanDialog.Start();

					// Start scanning on different thread.
					this.markPortsInUseThread = new MarkPortsInUseThread(ports);
					Thread t = new Thread(new ThreadStart(this.markPortsInUseThread.MarkPortsInUse));
					t.Start();

					while (this.markPortsInUseThread.IsScanning)
						Application.DoEvents();

					t.Join();

					// Cleanup.
					timer_ShowScanDialog.Stop();
				}

				comboBox_Port.Items.Clear();
				comboBox_Port.Items.AddRange(ports.ToArray());

				if (comboBox_Port.Items.Count > 0)
				{
					if ((this.portId != null) && (ports.Contains(this.portId)))
						comboBox_Port.SelectedItem = this.portId;
					else if ((old != null) && (ports.Contains(old)))
						comboBox_Port.SelectedItem = old;
					else
						comboBox_Port.SelectedIndex = 0;

					// Set property instead of member to ensure that changed event is fired.
					PortId = comboBox_Port.SelectedItem as SerialPortId;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"No serial COM ports available, check the serial COM ports of your system.",
						"No COM Ports",
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

			if (!DesignMode && Enabled && (comboBox_Port.Items.Count > 0))
			{
				if (this.portId != null)
					comboBox_Port.SelectedItem = this.portId;
				else
					comboBox_Port.SelectedIndex = 0;
			}
			else
			{
				comboBox_Port.SelectedIndex = -1;
			}

			this.isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
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
