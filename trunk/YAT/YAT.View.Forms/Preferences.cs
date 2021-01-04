//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

using MKY.Windows.Forms;

using YAT.Settings.Application;

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class Preferences : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private LocalUserSettingsRoot settings;
		private LocalUserSettingsRoot settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Preferences(LocalUserSettingsRoot settings)
		{
			InitializeComponent();

			KeepAndCloneAndAttachSettings(settings);
			InitializeControls();
		////SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public LocalUserSettingsRoot SettingsResult
		{
			get { return (this.settings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(LocalUserSettingsRoot settings)
		{
			this.settings = settings;
			this.settingsInEdit = new LocalUserSettingsRoot(settings); // Clone to ensure decoupling.
			this.settingsInEdit.Changed += settingsInEdit_Changed;
		}

		private void DetachAndAcceptSettings()
		{
			this.settingsInEdit.Changed -= settingsInEdit_Changed;
			this.settings = this.settingsInEdit;
		}

		private void settingsInEdit_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void Preferences_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_ShowTerminalInfo_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.MainWindow.ShowTerminalInfo = checkBox_ShowTerminalInfo.Checked;
		}

		private void checkBox_ShowTime_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.MainWindow.ShowTime = checkBox_ShowTime.Checked;
		}

		private void checkBox_ShowChrono_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.MainWindow.ShowChrono = checkBox_ShowChrono.Checked;
		}

		private void checkBox_AutoOpenWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.AutoOpenWorkspace = checkBox_AutoOpenWorkspace.Checked;
		}

		private void checkBox_AutoSaveWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.AutoSaveWorkspace = checkBox_AutoSaveWorkspace.Checked;
		}

		private void checkBox_UseRelativePaths_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.UseRelativePaths = checkBox_UseRelativePaths.Checked;
		}

		private void checkBox_NotifyNonAvailableIO_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.NotifyNonAvailableIO = checkBox_NotifyNonAvailableIO.Checked;
		}

		private void checkBox_RetrieveSerialPortCaptions_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.RetrieveSerialPortCaptions = checkBox_RetrieveSerialPortCaptions.Checked;
		}

		private void checkBox_DetectSerialPortsInUse_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.DetectSerialPortsInUse = checkBox_DetectSerialPortsInUse.Checked;
		}

		private void checkBox_AskForAlternateSerialPort_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.AskForAlternateSerialPort = checkBox_AskForAlternateSerialPort.Checked;
		}

		private void checkBox_AskForAlternateNetworkInterface_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.AskForAlternateNetworkInterface = checkBox_AskForAlternateNetworkInterface.Checked;
		}

		private void checkBox_MatchUsbSerial_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.MatchUsbSerial = checkBox_MatchUsbSerial.Checked;
		}

		private void checkBox_AskForAlternateUsbDevice_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.General.AskForAlternateUsbDevice = checkBox_AskForAlternateUsbDevice.Checked;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			DetachAndAcceptSettings();
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBoxEx.Show
				(
				this,
				"Reset all application preferences to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.settingsInEdit.SetDefaults();
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
			try
			{
				// Nothing to do yet.
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				checkBox_ShowTerminalInfo.Checked = this.settingsInEdit.MainWindow.ShowTerminalInfo;
				checkBox_ShowTime.Checked         = this.settingsInEdit.MainWindow.ShowTime;
				checkBox_ShowChrono.Checked       = this.settingsInEdit.MainWindow.ShowChrono;

				checkBox_AutoOpenWorkspace.Checked = this.settingsInEdit.General.AutoOpenWorkspace;
				checkBox_AutoSaveWorkspace.Checked = this.settingsInEdit.General.AutoSaveWorkspace;
				checkBox_UseRelativePaths.Checked  = this.settingsInEdit.General.UseRelativePaths;

				checkBox_NotifyNonAvailableIO.Checked = this.settingsInEdit.General.NotifyNonAvailableIO;

				checkBox_RetrieveSerialPortCaptions.Checked   = this.settingsInEdit.General.RetrieveSerialPortCaptions;
				checkBox_DetectSerialPortsInUse.Checked       = this.settingsInEdit.General.DetectSerialPortsInUse;
				checkBox_AskForAlternateSerialPort.Checked    = this.settingsInEdit.General.AskForAlternateSerialPort;

				checkBox_AskForAlternateNetworkInterface.Checked    = this.settingsInEdit.General.AskForAlternateNetworkInterface;

				checkBox_MatchUsbSerial.Checked              = this.settingsInEdit.General.MatchUsbSerial;
				checkBox_AskForAlternateUsbDevice.Checked    = this.settingsInEdit.General.AskForAlternateUsbDevice;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
