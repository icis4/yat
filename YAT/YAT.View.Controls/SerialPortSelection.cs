//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.IO.Ports;
using MKY.Windows.Forms;

using YAT.Settings.Application;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Scope = "member", Target = "YAT.View.Controls.SerialPortSelection.#InitializeComponent()", Justification = "The timer is only used for a well-defined interval.")]

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("PortIdChanged")]
	public partial class SerialPortSelection : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// Only set device list and controls once as soon as this control is enabled. This saves
		/// some time on startup since scanning for the ports may take some time.
		/// </summary>
		private bool portListIsBeingSetOrIsAlreadySet; // = false;

		private SettingControlsHelper isSettingControls;

		/// <remarks>
		/// Attention: Do not use <see cref="MKY.IO.Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="MKY.IO.Ports.SerialPortId.FirstStandardPort"/>
		/// is way better performing and good enough for most use cases.
		/// </remarks>
		private SerialPortId portId = SerialPortId.FirstStandardPort;

		private StatusBox showStatusDialog; // = null

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

			// SetControls() is initially called in the 'Paint' event handler.
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
					if (this.portId != value)
					{
						this.portId = value;
						SetPortSelection();
						OnPortIdChanged(EventArgs.Empty);
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
		public virtual void RefreshPortList()
		{
			SetPortList();
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
		private void SerialPortSelection_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetPortSelection();
			}

			// Ensure that port list is set as soon as this control gets enabled. Could
			// also be implemented in a EnabledChanged event handler. However, it's easier
			// to implement this here so it also done on initial 'Paint' event.
			if (Enabled && !this.portListIsBeingSetOrIsAlreadySet)
				SetPortList();
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void SerialPortSelection_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetPortSelection();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_Port_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				// Attention:
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				var id = comboBox_Port.SelectedItem as SerialPortId;
				if ((id != null) && id.Equals(comboBox_Port.Text))
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
					MessageBoxEx.Show
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
			RefreshPortList();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		// MKY.Diagnostics.DebugEx.WriteStack(Type type)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		// YAT.View.Controls.SerialPortSelection.SetSerialPortList()
		// YAT.View.Controls.SerialPortSelection.SerialPortSelection_Paint(Object sender, PaintEventArgs e)
		// System.Windows.Forms.Control.PaintWithErrorHandling(PaintEventArgs e, Int16 layer, Boolean disposeEventArgs)
		// System.Windows.Forms.Control.WmPaint(Message m)
		// System.Windows.Forms.Control.WndProc(Message m)
		// System.Windows.Forms.Control.ControlNativeWindow.WndProc(Message m)
		// System.Windows.Forms.NativeWindow.DebuggableCallback(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)
		// System.Windows.Forms.MessageBox.ShowCore(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, Boolean showHelp)
		// System.Windows.Forms.MessageBox.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		// YAT.View.Controls.SerialPortSelection.SetSerialPortList()
		// YAT.View.Controls.SerialPortSelection.RefreshSerialPortList()

		/// <remarks>
		/// Without precaution, and in case of no ports, the message box may appear twice due to
		/// the recursion described above (out of doc tag due to words not recognized by StyleCop).
		/// This issue is fixed by setting 'portListIsBeingSetOrIsAlreadySet' upon entering this method.
		/// 
		/// Note that the same fix has been implemented in <see cref="SocketSelection"/> and <see cref="UsbSerialHidDeviceSelection"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[ModalBehavior(ModalBehavior.InCaseOfNonUserError, Approval = "Is only called when displaying or refreshing the control on a form.")]
		private void SetPortList()
		{
			// Only scan for ports if control is enabled. This saves some time and prevents issues.
			if (Enabled && !DesignMode)
			{
				this.portListIsBeingSetOrIsAlreadySet = true; // Purpose see remarks above.

				SerialPortCollection ports = null;

				bool isSuccess;
				Exception errorException = null;
				string errorMessageLead = null;
				string errorMessageHint = null;

				using (this.showStatusDialog = StatusBox.Create("Serial Port Scan", null, null))
				{
					bool retrieveCaptions = ApplicationSettings.LocalUserSettings.General.RetrieveSerialPortCaptions;
					bool detectPortsInUse = ApplicationSettings.LocalUserSettings.General.DetectSerialPortsInUse;

					SerialPortSelectionWorker worker = new SerialPortSelectionWorker(retrieveCaptions, detectPortsInUse);
					worker.Status1Changed += worker_Status1Changed;
					worker.Status2Changed += worker_Status2Changed;
					worker.IsDone         += worker_IsDone;

					Thread t = new Thread(new ThreadStart(worker.DoWork));
					t.Name = GetType() + " Worker Thread";

					DateTime started = DateTime.Now;
					t.Start();

					// Let's work for approx. 150 ms:
					while (worker.IsBusy)
					{
						System.Windows.Forms.Application.DoEvents(); // Ensure that application stays responsive!
						Thread.Sleep(10); // Wait a short time to reduce the CPU load of this thread.

						TimeSpan ongoing = (DateTime.Now - started);
						if (ongoing.TotalMilliseconds > 150)
							break;
					}

					// Show status dialog since case doing work takes longer...
					DialogResult result;
					if (worker.IsBusy)
					{
						result = this.showStatusDialog.ShowDialog(this, "&Detect ports that are in use", ref detectPortsInUse, true);
						if (result == DialogResult.Cancel)
							worker.CancelWork();
						else
							result = worker.Result;

						ApplicationSettings.LocalUserSettings.General.DetectSerialPortsInUse = detectPortsInUse;
						ApplicationSettings.Save();
					}
					else
					{
						result = worker.Result;
					}

					// Clean up:
					if (t.Join(150)) // Allow some time to let the worker thread get terminated.
					{
						switch (result)
						{
							case DialogResult.OK:
							case DialogResult.Yes:
							case DialogResult.Cancel:
							{
								isSuccess = true;
								ports = worker.Ports;
								break;
							}

							case DialogResult.Abort:
							{
								isSuccess = false;
								errorException   = worker.Exception;
								errorMessageLead = worker.ExceptionLead;
								errorMessageHint = worker.ExceptionHint;
								break;
							}

							default:
							{
								isSuccess = true; // Ignore resulting ports.
								break;
							}
						}
					}
					else
					{
						t.Abort(); // Terminate non-joinable threads!

						if (result != DialogResult.Cancel)
						{
							isSuccess = false;
							errorMessageLead = "Timeout while scanning the ports!";
							errorMessageHint = "If the issue cannot be solved, tell YAT to differently scan the ports by going to 'File > Preferences...' and change the port related settings.";
						}
						else
						{
							isSuccess = true;
						}
					}

					worker.Status1Changed -= worker_Status1Changed;
					worker.Status2Changed -= worker_Status2Changed;
					worker.IsDone         -= worker_IsDone;
				} // using (showStatusDialog)

				this.isSettingControls.Enter();

				comboBox_Port.Items.Clear();

				if ((ports != null) && (ports.Count > 0))
				{
					comboBox_Port.Items.AddRange(ports.ToArray());

					if ((this.portId != null) && (ports.Contains(this.portId)))
					{
						// Nothing has changed, just restore the selected item:
						comboBox_Port.SelectedItem = this.portId;

						if (!isSuccess)
							ShowErrorMessage(errorException, errorMessageLead, errorMessageHint);
					}
					else
					{
						// Get the 'NotAvailable' string BEFORE defaulting!
						string portIdNotAvailable = this.portId;

						// Ensure that the settings item is defaulted and shown by SetControls().
						// Set property instead of member to ensure that changed event is fired.
						PortId = ports[0];

						if (isSuccess)
							ShowNotAvailableDefaultedMessage(portIdNotAvailable, ports[0]);
						else
							ShowErrorMessage(errorException, errorMessageLead, errorMessageHint);
					}
				}
				else
				{
					// Ensure that the settings item is nulled and reset by SetControls().
					// Set property instead of member to ensure that changed event is fired.
					PortId = null;

					if (isSuccess)
						ShowNoPortsMessage();
					else
						ShowErrorMessage(errorException, errorMessageLead, errorMessageHint);
				}

				this.isSettingControls.Leave();
			}
		}

		private void worker_Status1Changed(object sender, EventArgs<string> e)
		{
			this.showStatusDialog.SetStatus1Synchronized(e.Value);
			// Do not check for 'IsShowing', as status box may get shown after this event.
		}

		private void worker_Status2Changed(object sender, EventArgs<string> e)
		{
			this.showStatusDialog.SetStatus2Synchronized(e.Value);
			// Do not check for 'IsShowing', as status box may get shown after this event.
		}

		private void worker_IsDone(object sender, EventArgs<DialogResult> e)
		{
			if (this.showStatusDialog.IsShowing)
				this.showStatusDialog.CloseSynchronized(e.Value);
		}

		private void ShowNoPortsMessage()
		{
			string message =
				"There are currently no serial COM ports available." + Environment.NewLine +
				"Check the serial COM ports of your system.";

			MessageBoxEx.Show
			(
				this,
				message,
				"No serial COM ports available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning
			);
		}

		private void ShowNotAvailableDefaultedMessage(string portIdNotAvailable, string portIdDefaulted)
		{
			string message =
				"The previous serial port " + portIdNotAvailable + " is currently not available." + Environment.NewLine + Environment.NewLine +
				"The selection has been defaulted to the first available port '" + portIdDefaulted + "'.";

			MessageBoxEx.Show
			(
				this,
				message,
				"Previous serial COM port not available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		private void ShowErrorMessage(Exception ex, string info, string hint)
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(info))
				sb.Append(info);

			if (ex != null)
			{
				if (sb.Length > 0)
				{
					sb.AppendLine();
					sb.AppendLine();
				}

				sb.AppendLine("System error message:");
				sb.Append(ex.Message);

				if (ex.InnerException != null)
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.AppendLine("Additional error message:");
					sb.Append(ex.InnerException.Message);
				}
			}

			if (sb.Length > 0)
			{
				sb.AppendLine();
				sb.AppendLine();
			}

			sb.Append("Check the serial COM ports of your system.");

			if (!string.IsNullOrEmpty(hint))
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.Append(hint);
			}

			MessageBoxEx.Show
			(
				this,
				sb.ToString(),
				"Error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
			);
		}

		private void SetPortSelection()
		{
			this.isSettingControls.Enter();

			if (!DesignMode && Enabled)
			{
				if (comboBox_Port.Items.Count > 0)
				{
					if (this.portId != null)
					{
						if (comboBox_Port.Items.Contains(this.portId))
						{	// Applies if an item of the combo box is selected.
							comboBox_Port.SelectedItem = this.portId;
						}
						else
						{	// Applies if an item that is not in the combo box is selected.
							comboBox_Port.SelectedIndex = ControlEx.InvalidIndex;
							comboBox_Port.Text = this.portId;
						}
					}
					else
					{	// Item doesn't exist, use default = first item in the combo box.
						comboBox_Port.SelectedIndex = 0;
					}
				}
				else
				{
					comboBox_Port.SelectedIndex = ControlEx.InvalidIndex;
					if (this.portId != null)
						comboBox_Port.Text = this.portId;
					else
						comboBox_Port.Text = "";
				}
			}
			else
			{
				comboBox_Port.SelectedIndex = ControlEx.InvalidIndex;
				comboBox_Port.Text = "";
			}

			this.isSettingControls.Leave();
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
