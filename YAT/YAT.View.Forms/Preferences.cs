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
// YAT Version 2.4.0
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Windows.Forms;

using MKY.Windows.Forms;

using YAT.Settings.Application;

#endregion

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

		private LocalUserSettingsRoot localUserSettings;
		private LocalUserSettingsRoot localUserSettingsInEdit;

		private RoamingUserSettingsRoot roamingUserSettings;
		private RoamingUserSettingsRoot roamingUserSettingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Preferences(LocalUserSettingsRoot localUserSettings, RoamingUserSettingsRoot roamingUserSettings)
		{
			InitializeComponent();

			KeepAndCloneAndAttachSettings(localUserSettings, roamingUserSettings);

		////SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public LocalUserSettingsRoot LocalUserSettingsResult
		{
			get { return (this.localUserSettings); }
		}

		/// <summary></summary>
		public RoamingUserSettingsRoot RoamingUserSettingsResult
		{
			get { return (this.roamingUserSettings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(LocalUserSettingsRoot localUserSettings, RoamingUserSettingsRoot roamingUserSettings)
		{
			this.localUserSettings = localUserSettings;
			this.localUserSettingsInEdit = new LocalUserSettingsRoot(localUserSettings); // Clone to ensure decoupling.
			this.localUserSettingsInEdit.Changed += settingsInEdit_Changed;

			this.roamingUserSettings = roamingUserSettings;
			this.roamingUserSettingsInEdit = new RoamingUserSettingsRoot(roamingUserSettings); // Clone to ensure decoupling.
			this.roamingUserSettingsInEdit.Changed += settingsInEdit_Changed;
		}

		private void DetachAndAcceptSettings()
		{
			this.localUserSettingsInEdit.Changed -= settingsInEdit_Changed;
			this.localUserSettings = this.localUserSettingsInEdit;

			this.roamingUserSettingsInEdit.Changed -= settingsInEdit_Changed;
			this.roamingUserSettings = this.roamingUserSettingsInEdit;
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

		private void checkBox_CheckFontAvailability_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.roamingUserSettingsInEdit.Font.CheckAvailability = checkBox_CheckFontAvailability.Checked;
		}

		private void checkBox_ShowTerminalInfo_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.MainWindow.ShowTerminalInfo = checkBox_ShowTerminalInfo.Checked;
		}

		private void checkBox_ShowTime_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.MainWindow.ShowTime = checkBox_ShowTime.Checked;
		}

		private void checkBox_ShowChrono_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.MainWindow.ShowChrono = checkBox_ShowChrono.Checked;
		}

		private void checkBox_AutoOpenWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.AutoOpenWorkspace = checkBox_AutoOpenWorkspace.Checked;
		}

		private void checkBox_AutoSaveWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.AutoSaveWorkspace = checkBox_AutoSaveWorkspace.Checked;
		}

		private void checkBox_UseRelativePaths_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.UseRelativePaths = checkBox_UseRelativePaths.Checked;
		}

		private void checkBox_CheckTerminalFont_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.roamingUserSettingsInEdit.Font.CheckTerminal = checkBox_CheckTerminalFont.Checked;
		}

		private void checkBox_NotifyNonAvailableIO_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.NotifyNonAvailableIO = checkBox_NotifyNonAvailableIO.Checked;
		}

		private void checkBox_RetrieveSerialPortCaptions_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.RetrieveSerialPortCaptions = checkBox_RetrieveSerialPortCaptions.Checked;
		}

		private void checkBox_DetectSerialPortsInUse_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.DetectSerialPortsInUse = checkBox_DetectSerialPortsInUse.Checked;
		}

		private void checkBox_AskForAlternateSerialPort_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.AskForAlternateSerialPort = checkBox_AskForAlternateSerialPort.Checked;
		}

		private void checkBox_AskForAlternateNetworkInterface_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.AskForAlternateNetworkInterface = checkBox_AskForAlternateNetworkInterface.Checked;
		}

		private void checkBox_MatchUsbSerial_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.MatchUsbSerial = checkBox_MatchUsbSerial.Checked;
		}

		private void checkBox_AskForAlternateUsbDevice_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.localUserSettingsInEdit.General.AskForAlternateUsbDevice = checkBox_AskForAlternateUsbDevice.Checked;
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
				this.localUserSettingsInEdit.SetDefaults();
				this.roamingUserSettingsInEdit.SetDefaults();
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				checkBox_CheckFontAvailability.Checked           = this.roamingUserSettingsInEdit.Font.CheckAvailability;
				checkBox_ShowTerminalInfo.Checked                = this.localUserSettingsInEdit.MainWindow.ShowTerminalInfo;
				checkBox_ShowTime.Checked                        = this.localUserSettingsInEdit.MainWindow.ShowTime;
				checkBox_ShowChrono.Checked                      = this.localUserSettingsInEdit.MainWindow.ShowChrono;

				checkBox_AutoOpenWorkspace.Checked               = this.localUserSettingsInEdit.General.AutoOpenWorkspace;
				checkBox_AutoSaveWorkspace.Checked               = this.localUserSettingsInEdit.General.AutoSaveWorkspace;
				checkBox_UseRelativePaths.Checked                = this.localUserSettingsInEdit.General.UseRelativePaths;

				checkBox_CheckTerminalFont.Checked               = this.roamingUserSettingsInEdit.Font.CheckTerminal;
				checkBox_NotifyNonAvailableIO.Checked            = this.localUserSettingsInEdit.General.NotifyNonAvailableIO;

				checkBox_RetrieveSerialPortCaptions.Checked      = this.localUserSettingsInEdit.General.RetrieveSerialPortCaptions;
				checkBox_DetectSerialPortsInUse.Checked          = this.localUserSettingsInEdit.General.DetectSerialPortsInUse;
				checkBox_AskForAlternateSerialPort.Checked       = this.localUserSettingsInEdit.General.AskForAlternateSerialPort;

				checkBox_AskForAlternateNetworkInterface.Checked = this.localUserSettingsInEdit.General.AskForAlternateNetworkInterface;

				checkBox_MatchUsbSerial.Checked                  = this.localUserSettingsInEdit.General.MatchUsbSerial;
				checkBox_AskForAlternateUsbDevice.Checked        = this.localUserSettingsInEdit.General.AskForAlternateUsbDevice;
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
