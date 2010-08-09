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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
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

		private Settings.GeneralSettings settings;
		private Settings.GeneralSettings settings_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Preferences(Settings.GeneralSettings settings)
		{
			InitializeComponent();

			this.settings = settings;
			this.settings_Form = new Settings.GeneralSettings(settings);
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Settings.GeneralSettings SettingsResult
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

		private void checkBox_AutoOpenWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.AutoOpenWorkspace = checkBox_AutoOpenWorkspace.Checked;
				SetControls();
			}
		}

		private void checkBox_AutoSaveWorkspace_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.AutoSaveWorkspace = checkBox_AutoSaveWorkspace.Checked;
				SetControls();
			}
		}

		private void checkBox_UseRelativePaths_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.UseRelativePaths = checkBox_UseRelativePaths.Checked;
				SetControls();
			}
		}

		private void checkBox_DetectSerialPortsInUse_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				this.settings_Form.DetectSerialPortsInUse = checkBox_DetectSerialPortsInUse.Checked;
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
			this.settings_Form.SetDefaults();
			SetControls();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls = true;

			// Nothing to do yet

			this.isSettingControls = false;
		}

		private void SetControls()
		{
			this.isSettingControls = true;

			checkBox_AutoOpenWorkspace.Checked = this.settings_Form.AutoOpenWorkspace;
			checkBox_AutoSaveWorkspace.Checked = this.settings_Form.AutoSaveWorkspace;
			checkBox_UseRelativePaths.Checked  = this.settings_Form.UseRelativePaths;

			checkBox_DetectSerialPortsInUse.Checked = this.settings_Form.DetectSerialPortsInUse;

			this.isSettingControls = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
