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
// YAT 2.0 Delta Version 1.99.80
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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.IO.Ports;
using MKY.Windows.Forms;

using YAT.Model.Utilities;
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
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string PortIdDefault = SerialPortId.FirstStandardPortName;

		#endregion

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
		private SerialPortId portId = PortIdDefault;

		private InUseInfo activePortInUseInfo; // = null;

		private StatusBox showStatusDialog; // = null;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the PortId property is changed.")]
		public event EventHandler PortIdChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the ActivePortInUseInfo property is changed.")]
		public event EventHandler ActivePortInUseInfoChanged;

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
		[Description("The serial COM port ID that is selected.")]
		[DefaultValue(PortIdDefault)]
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

		/// <summary></summary>
		[Category("Serial Port")]
		[Description(@"The currently active serial COM port indicated as ""this serial port"".")]
		[DefaultValue(null)]
		public InUseInfo ActivePortInUseInfo
		{
			get { return (this.activePortInUseInfo); }
			set
			{
				if (this.activePortInUseInfo != value)
				{
					this.activePortInUseInfo = value;
					RefreshPortList();
					OnActivePortInUseInfoChanged(EventArgs.Empty);
				}
			}
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
				if ((id != null) && id.EqualsName(comboBox_Port.Text))
				{
					PortId = id;
				}
				else if (SerialPortId.TryParse(comboBox_Port.Text, out id))
				{
					PortId = id;
				}
				else if (string.IsNullOrEmpty(comboBox_Port.Text))
				{
					PortId = null;
				}
				else
				{
					MessageBoxEx.Show
					(
						this,
						"Serial COM port name is invalid",
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

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
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
			// Only scan for ports if control is enabled and visible. This saves some time and prevents issues. And refresh makes no sense if not visible.
			if (Enabled && Visible && !DesignMode)
			{
				ResetOnDialogMessage();

				this.portListIsBeingSetOrIsAlreadySet = true; // Purpose see remarks above.

				SerialPortCollection ports = null;

				bool scanSuccess;
				Exception errorException = null;
				string errorMessageLead = null;
				string errorMessageHint = null;

				using (this.showStatusDialog = StatusBox.Create("Serial Port Scan", null, null))
				{
					bool retrieveCaptions = ApplicationSettings.LocalUserSettings.General.RetrieveSerialPortCaptions;
					bool detectPortsInUse = ApplicationSettings.LocalUserSettings.General.DetectSerialPortsInUse;

					var worker = new SerialPortSelectionWorker(retrieveCaptions, detectPortsInUse, this.activePortInUseInfo);
					worker.Status1Changed += worker_Status1Changed;
					worker.Status2Changed += worker_Status2Changed;
					worker.IsDone         += worker_IsDone;

					var t = new Thread(new ThreadStart(worker.DoWork));
					t.Name = GetType() + " Worker Thread";

					var started = DateTime.Now;
					t.Start();

					// Let's work for approx. 150 ms:
					while (worker.IsBusy)
					{
						System.Windows.Forms.Application.DoEvents(); // Ensure that application stays responsive!
						Thread.Sleep(10); // Wait a short time to reduce the CPU load of this thread.

						var ongoing = (DateTime.Now - started);
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
					if (t.Join(250)) // Allow some (not really noticable) time to let the worker thread get terminated.
					{
						switch (result)
						{
							case DialogResult.OK:
							case DialogResult.Yes:
							case DialogResult.Cancel:
							{
								scanSuccess = true;

								ports = worker.Ports;
								break;
							}

							case DialogResult.Abort:
							{
								scanSuccess = false;

								errorException   = worker.Exception;
								errorMessageLead = worker.ExceptionLead;
								errorMessageHint = worker.ExceptionHint;
								break;
							}

							default:
							{
								scanSuccess = true;

								// Ignore resulting ports.
								break;
							}
						}
					}
					else // t.Join() has timed out!
					{
						worker.NotifyThreadAbortWillHappen();
						t.Abort(); // Thread.Abort() must not be used whenever possible!
						           // This is only the fall-back in case joining fails for too long.
						if (result != DialogResult.Cancel)
						{
							scanSuccess = false;

							errorMessageLead = "Timeout while scanning the ports!";
							errorMessageHint = "If the issue cannot be solved, tell YAT to differently scan the ports by going to 'File > Preferences...' and change the port related settings.";
						}
						else
						{
							scanSuccess = true;

							ports = worker.Ports;
						}
					}

					worker.Status1Changed -= worker_Status1Changed;
					worker.Status2Changed -= worker_Status2Changed;
					worker.IsDone         -= worker_IsDone;
				} // using (showStatusDialog)

				// Attention:
				// Similar code exists in Model.Terminal.ValidateIO().
				// Changes here may have to be applied there too!

				this.isSettingControls.Enter();

				comboBox_Port.Items.Clear();

				if ((ports != null) && (ports.Count > 0))
				{
					comboBox_Port.Items.AddRange(ports.ToArray());

					if ((this.portId != null) && (ports.Contains(this.portId)))
					{
						// Nothing has changed, just restore the selected item:
						comboBox_Port.SelectedItem = this.portId;

						if (!scanSuccess)
							ShowErrorMessage(errorException, errorMessageLead, errorMessageHint);
					}
					else
					{
						// Get the 'NotAvailable' string BEFORE defaulting!
						string portIdNotAvailable = this.portId;

						SerialPortId portIdAlternate;
						if (scanSuccess && TryGetAlternate(ports, out portIdAlternate))
						{
							// Ensure that the settings item is defaulted and shown by SetControls().
							// Set property instead of member to ensure that changed event is fired.
							PortId = portIdAlternate;

							ShowNotAvailableSwitchMessage(portIdNotAvailable, portIdAlternate);
						}
						else
						{
							// Ensure that the settings item is defaulted and shown by SetControls().
							// Set property instead of member to ensure that changed event is fired.
							PortId = ports[0];

							if (scanSuccess)
								ShowNotAvailableDefaultedMessage(portIdNotAvailable, ports[0]);
							else
								ShowErrorMessage(errorException, errorMessageLead, errorMessageHint);
						}
					}
				}
				else // ports.Count == 0
				{
					// Ensure that the settings item is nulled and reset by SetControls().
					// Set property instead of member to ensure that changed event is fired.
					PortId = null;

					if (scanSuccess)
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
		////IsShowing is not checked for, as status box may get shown after this event.
		}

		private void worker_Status2Changed(object sender, EventArgs<string> e)
		{
			this.showStatusDialog.SetStatus2Synchronized(e.Value);
		////IsShowing is not checked for, as status box may get shown after this event.
		}

		private void worker_IsDone(object sender, EventArgs<DialogResult> e)
		{
			if (this.showStatusDialog.IsShowing)
				this.showStatusDialog.CloseSynchronized(e.Value);
		}

		private void ResetOnDialogMessage()
		{
			label_OnDialogMessage.Text = "";
		}

		private bool TryGetAlternate(SerialPortCollection ports, out SerialPortId portIdAlternate)
		{
			// Select the first available port that is not in use:
			foreach (var port in ports)
			{
				if (!port.IsInUse)
				{
					portIdAlternate = port;
					return (true);
				}
			}

			// No alternate that is not 'InUse':
			portIdAlternate = null;
			return (false);
		}

		/// <remarks>
		/// Showing this as on dialog message instead of <see cref="MessageBox"/> to reduce the number of potentially annoying popups.
		/// </remarks>
		private void ShowNoPortsMessage()
		{
			label_OnDialogMessage.Text = "No serial COM ports currently available";
		}

		private void ShowNotAvailableSwitchMessage(string portIdNotAvailable, string portIdAlternate)
		{
			string message =
				"The previous serial port " + portIdNotAvailable + " is currently not available." + Environment.NewLine + Environment.NewLine +
				"The selection has been switched to " + portIdAlternate + " (first available port that is currently not in use).";

			MessageBoxEx.Show
			(
				this,
				message,
				"Previous serial COM port not available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		private void ShowNotAvailableDefaultedMessage(string portIdNotAvailable, string portIdDefaulted)
		{
			string message =
				"The previous serial port " + portIdNotAvailable + " is currently not available." + Environment.NewLine + Environment.NewLine +
				"The selection has been defaulted to " + portIdDefaulted + " (first available port).";

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
			var sb = new StringBuilder(ErrorHelper.ComposeMessage(info, ex));

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

			if (!DesignMode && Enabled && (this.portId != null))
				SelectionHelper.Select(comboBox_Port, this.portId, this.portId);
			else
				SelectionHelper.Deselect(comboBox_Port);

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary>
		/// Invokes the <see cref="PortIdChanged"/> event.
		/// </summary>
		protected virtual void OnPortIdChanged(EventArgs e)
		{
			EventHelper.FireSync(PortIdChanged, this, e);
		}

		/// <summary>
		/// Invokes the <see cref="ActivePortInUseInfoChanged"/> event.
		/// </summary>
		protected virtual void OnActivePortInUseInfoChanged(EventArgs e)
		{
			EventHelper.FireSync(ActivePortInUseInfoChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
