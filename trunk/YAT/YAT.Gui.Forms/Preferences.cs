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
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class Preferences : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

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

			this.settings = settings;
			this.settings_Form = new Settings.Application.LocalUserSettingsRoot(settings);
			InitializeControls();
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

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void Preferences_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_ShowTerminalInfo_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.MainWindow.ShowTerminalInfo = checkBox_ShowTerminalInfo.Checked;
				SetControls();
			}
		}

		private void checkBox_ShowChrono_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.MainWindow.ShowChrono = checkBox_ShowChrono.Checked;
				SetControls();
			}
		}

		private void checkBox_AutoOpenWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.General.AutoOpenWorkspace = checkBox_AutoOpenWorkspace.Checked;
				SetControls();
			}
		}

		private void checkBox_AutoSaveWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.General.AutoSaveWorkspace = checkBox_AutoSaveWorkspace.Checked;
				SetControls();
			}
		}

		private void checkBox_UseRelativePaths_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.General.UseRelativePaths = checkBox_UseRelativePaths.Checked;
				SetControls();
			}
		}

		private void checkBox_DetectSerialPortsInUse_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.General.DetectSerialPortsInUse = checkBox_DetectSerialPortsInUse.Checked;
				SetControls();
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.settings = this.settings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show
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
				SetControls();
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls = true;

			// Nothing to do yet.

			this.isSettingControls = false;
		}

		private void SetControls()
		{
			this.isSettingControls = true;

			checkBox_ShowTerminalInfo.Checked = this.settings_Form.MainWindow.ShowTerminalInfo;
			checkBox_ShowChrono.Checked       = this.settings_Form.MainWindow.ShowChrono;

			checkBox_AutoOpenWorkspace.Checked = this.settings_Form.General.AutoOpenWorkspace;
			checkBox_AutoSaveWorkspace.Checked = this.settings_Form.General.AutoSaveWorkspace;
			checkBox_UseRelativePaths.Checked  = this.settings_Form.General.UseRelativePaths;

			checkBox_DetectSerialPortsInUse.Checked = this.settings_Form.General.DetectSerialPortsInUse;

			this.isSettingControls = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
