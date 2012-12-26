//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

using MKY.Windows.Forms;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class Preferences : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Settings.Application.LocalUserSettingsRoot settings;
		private Settings.Application.LocalUserSettingsRoot settings_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Preferences(Settings.Application.LocalUserSettingsRoot settings)
		{
			InitializeComponent();

			KeepAndCloneAndAttachSettings(settings);
			InitializeControls();

			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Settings.Application.LocalUserSettingsRoot SettingsResult
		{
			get { return (this.settings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(Settings.Application.LocalUserSettingsRoot settings)
		{
			this.settings = settings;
			this.settings_Form = new Settings.Application.LocalUserSettingsRoot(settings);
			this.settings_Form.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
		}

		private void DetachAndAcceptSettings()
		{
			this.settings_Form.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(settings_Form_Changed);
			this.settings = this.settings_Form;
		}

		private void settings_Form_Changed(object sender, MKY.Settings.SettingsEventArgs e)
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
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
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
			if (!this.isSettingControls)
				this.settings_Form.MainWindow.ShowTerminalInfo = checkBox_ShowTerminalInfo.Checked;
		}

		private void checkBox_ShowChrono_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.MainWindow.ShowChrono = checkBox_ShowChrono.Checked;
		}

		private void checkBox_AutoOpenWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.General.AutoOpenWorkspace = checkBox_AutoOpenWorkspace.Checked;
		}

		private void checkBox_AutoSaveWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.General.AutoSaveWorkspace = checkBox_AutoSaveWorkspace.Checked;
		}

		private void checkBox_UseRelativePaths_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.General.UseRelativePaths = checkBox_UseRelativePaths.Checked;
		}

		private void checkBox_DetectSerialPortsInUse_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settings_Form.General.DetectSerialPortsInUse = checkBox_DetectSerialPortsInUse.Checked;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			DetachAndAcceptSettings();
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBoxEx.Show
				(
				this,
				"Reset all settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.settings_Form.SetDefaults();
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

			// Nothing to do yet.

			this.isSettingControls.Leave();
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			checkBox_ShowTerminalInfo.Checked = this.settings_Form.MainWindow.ShowTerminalInfo;
			checkBox_ShowChrono.Checked       = this.settings_Form.MainWindow.ShowChrono;

			checkBox_AutoOpenWorkspace.Checked = this.settings_Form.General.AutoOpenWorkspace;
			checkBox_AutoSaveWorkspace.Checked = this.settings_Form.General.AutoSaveWorkspace;
			checkBox_UseRelativePaths.Checked  = this.settings_Form.General.UseRelativePaths;

			checkBox_DetectSerialPortsInUse.Checked = this.settings_Form.General.DetectSerialPortsInUse;

			this.isSettingControls.Leave();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
