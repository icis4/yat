//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.Types;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;

namespace YAT.Gui.Forms
{
	public partial class LogSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Log.Settings.LogSettings _settings;
		private Log.Settings.LogSettings _settings_Form;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public LogSettings(Log.Settings.LogSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Log.Settings.LogSettings(settings);
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Log.Settings.LogSettings SettingsResult
		{
			get { return (_settings); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		private void LogSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// Initially set controls and validate its contents where needed
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void pathLabel_Root_Click(object sender, EventArgs e)
		{
			ShowSetRootDirectoryDialog();
		}

		private void button_Root_Click(object sender, EventArgs e)
		{
			ShowSetRootDirectoryDialog();
		}

		private void checkBox_Raw_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.RawLogTx = checkBox_Raw_Tx.Checked;
				SetControls();
			}
		}

		private void pathLabel_Raw_Tx_Click(object sender, EventArgs e)
		{
			_settings_Form.RawLogTx = !_settings_Form.RawLogTx;
			SetControls();
		}

		private void checkBox_Raw_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.RawLogBidir = checkBox_Raw_Bidir.Checked;
				SetControls();
			}
		}

		private void pathLabel_Raw_Bidir_Click(object sender, EventArgs e)
		{
			_settings_Form.RawLogBidir = !_settings_Form.RawLogBidir;
			SetControls();
		}

		private void checkBox_Raw_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.RawLogRx = checkBox_Raw_Rx.Checked;
				SetControls();
			}
		}

		private void pathLabel_Raw_Rx_Click(object sender, EventArgs e)
		{
			_settings_Form.RawLogRx = !_settings_Form.RawLogRx;
			SetControls();
		}

		private void comboBox_Raw_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFilenameChars(comboBox_Raw_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if (_settings_Form.BothRawAndNeat && (comboBox_Raw_Extension.Text == _settings_Form.NeatExtension))
			{
				ExtensionConflictMessage();
				e.Cancel = true;
				return;
			}
			SetControls();
		}

		private void comboBox_Raw_Extension_TextChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.RawExtension = comboBox_Raw_Extension.Text;
				SetControls();
			}
		}

		private void checkBox_Neat_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NeatLogTx = checkBox_Neat_Tx.Checked;
				SetControls();
			}
		}

		private void pathLabel_Neat_Tx_Click(object sender, EventArgs e)
		{
			_settings_Form.NeatLogTx = !_settings_Form.NeatLogTx;
			SetControls();
		}

		private void checkBox_Neat_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NeatLogBidir = checkBox_Neat_Bidir.Checked;
				SetControls();
			}
		}

		private void pathLabel_Neat_Bidir_Click(object sender, EventArgs e)
		{
			_settings_Form.NeatLogBidir = !_settings_Form.NeatLogBidir;
			SetControls();
		}

		private void checkBox_Neat_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NeatLogRx = checkBox_Neat_Rx.Checked;
				SetControls();
			}
		}

		private void pathLabel_Neat_Rx_Click(object sender, EventArgs e)
		{
			_settings_Form.NeatLogRx = !_settings_Form.NeatLogRx;
			SetControls();
		}

		private void comboBox_Neat_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFilenameChars(comboBox_Neat_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}
			if (_settings_Form.BothRawAndNeat && (comboBox_Neat_Extension.Text == _settings_Form.RawExtension))
			{
				ExtensionConflictMessage();
				e.Cancel = true;
				return;
			}
			SetControls();
		}

		private void comboBox_Neat_Extension_TextChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NeatExtension = comboBox_Neat_Extension.Text;
				SetControls();
			}
		}

		private void rad_Options_ModeCreate_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls && rad_Options_ModeCreate.Checked)
			{
				_settings_Form.WriteMode = Log.LogFileWriteMode.Create;
				_settings_Form.NameDate = _settings.NameDate;
				_settings_Form.NameTime = _settings.NameTime;
				SetControls();
			}
		}

		private void rad_Options_ModeAppend_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls && rad_Options_ModeAppend.Checked)
			{
				_settings_Form.WriteMode = Log.LogFileWriteMode.Append;
				_settings_Form.NameDate = false;
				_settings_Form.NameTime = false;
				SetControls();
			}
		}

		private void checkBox_Options_SubdirectoriesFormat_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.SubdirectoriesFormat = checkBox_Options_SubdirectoriesFormat.Checked;
				SetControls();
			}
		}

		private void checkBox_Options_SubdirectoriesChannel_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.SubdirectoriesChannel = checkBox_Options_SubdirectoriesChannel.Checked;
				SetControls();
			}
		}

		private void checkBox_Options_NameFormat_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NameFormat = checkBox_Options_NameFormat.Checked;
				SetControls();
			}
		}

		private void checkBox_Options_NameChannel_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NameChannel = checkBox_Options_NameChannel.Checked;
				SetControls();
			}
		}

		private void checkBox_Options_NameDate_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NameDate = checkBox_Options_NameDate.Checked;
				SetControls();
			}
		}

		private void checkBox_Options_NameTime_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NameTime = checkBox_Options_NameTime.Checked;
				SetControls();
			}
		}

		private void comboBox_Options_NameSeparator_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFilenameChars(comboBox_Options_NameSeparator.Text, "Separator"))
				e.Cancel = true;
		}

		private void comboBox_Options_NameSeparator_TextChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_settings_Form.NameSeparator = (Log.FileNameSeparator)(string)comboBox_Options_NameSeparator.Text;
				SetControls();
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			if (ResolveNamingConflicts())
				_settings = _settings_Form;
			else
				DialogResult = DialogResult.None;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			_settings_Form.SetDefaults();
			SetControls();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			_isSettingControls = true;

			comboBox_Neat_Extension.Items.Clear();
			foreach (string s in ExtensionSettings.TextFilesWithDot)
				comboBox_Neat_Extension.Items.Add(s);

			comboBox_Raw_Extension.Items.Clear();
			foreach (string s in ExtensionSettings.BinaryFilesWithDot)
				comboBox_Raw_Extension.Items.Add(s);

			comboBox_Options_NameSeparator.Items.Clear();
			foreach (string s in Log.FileNameSeparator.Items)
				comboBox_Options_NameSeparator.Items.Add(s);

			_isSettingControls = false;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			if (_settings_Form.RootPath != "")
				pathLabel_Root.Text = _settings_Form.RootPath + Path.DirectorySeparatorChar + _settings_Form.RootFileName;
			else
				pathLabel_Root.Text = "<Set a root file...>";

			checkBox_Raw_Tx.Checked    = _settings_Form.RawLogTx;
			checkBox_Raw_Bidir.Checked = _settings_Form.RawLogBidir;
			checkBox_Raw_Rx.Checked    = _settings_Form.RawLogRx;

			pathLabel_Raw_Tx.Text    = _settings_Form.RawTxRootRelativeFilePath;
			pathLabel_Raw_Bidir.Text = _settings_Form.RawBidirRootRelativeFilePath;
			pathLabel_Raw_Rx.Text    = _settings_Form.RawRxRootRelativeFilePath;

			comboBox_Raw_Extension.Text = _settings_Form.RawExtension;

			checkBox_Neat_Tx.Checked    = _settings_Form.NeatLogTx;
			checkBox_Neat_Bidir.Checked = _settings_Form.NeatLogBidir;
			checkBox_Neat_Rx.Checked    = _settings_Form.NeatLogRx;

			pathLabel_Neat_Tx.Text    = _settings_Form.NeatTxRootRelativeFilePath;
			pathLabel_Neat_Bidir.Text = _settings_Form.NeatBidirRootRelativeFilePath;
			pathLabel_Neat_Rx.Text    = _settings_Form.NeatRxRootRelativeFilePath;

			comboBox_Neat_Extension.Text = _settings_Form.NeatExtension;

			rad_Options_ModeCreate.Checked = (_settings_Form.WriteMode == Log.LogFileWriteMode.Create);
			rad_Options_ModeAppend.Checked = (_settings_Form.WriteMode == Log.LogFileWriteMode.Append);

			checkBox_Options_SubdirectoriesFormat.Checked  = _settings_Form.SubdirectoriesFormat;
			checkBox_Options_SubdirectoriesChannel.Checked = _settings_Form.SubdirectoriesChannel;

			checkBox_Options_NameFormat.Checked  = _settings_Form.NameFormat;
			checkBox_Options_NameChannel.Checked = _settings_Form.NameChannel;
			checkBox_Options_NameDate.Checked    = _settings_Form.NameDate;
			checkBox_Options_NameTime.Checked    = _settings_Form.NameTime;
			comboBox_Options_NameSeparator.Text  = _settings_Form.NameSeparator;

			bool dateTimeEnabled = (_settings_Form.WriteMode == Log.LogFileWriteMode.Create);
			checkBox_Options_NameDate.Enabled = dateTimeEnabled;
			checkBox_Options_NameTime.Enabled = dateTimeEnabled;

			_isSettingControls = false;
		}

		private void ShowSetRootDirectoryDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set Root";
			ofd.Filter = ExtensionSettings.AllFilesFilter;
			if (Directory.Exists(_settings_Form.RootPath))
				ofd.InitialDirectory = _settings_Form.RootPath;
			else
				ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.LogFilesPath;
			ofd.FileName = _settings_Form.RootFileName;
			ofd.CheckPathExists = false;
			ofd.CheckFileExists = false;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName != ""))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.LogFilesPath = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				_settings_Form.RootPath = Path.GetDirectoryName(ofd.FileName);
				_settings_Form.RootFileName = Path.GetFileNameWithoutExtension(ofd.FileName);

				SetControls();
			}
		}

		//------------------------------------------------------------------------------------------
		// Validation
		//------------------------------------------------------------------------------------------

		private bool ValidateFilenameChars(string filenameChars, string title)
		{
			StringWriter invalid = new StringWriter();
			invalid.Write(Path.GetInvalidPathChars());
			invalid.Write(Path.VolumeSeparatorChar);
			invalid.Write(Path.DirectorySeparatorChar);
			invalid.Write(Path.AltDirectorySeparatorChar);
			invalid.Write(Path.PathSeparator);

			if (XString.Contains(filenameChars, invalid.ToString().ToCharArray()))
			{
				StringWriter invalidPrintable = new StringWriter();
				foreach (char c in invalid.ToString().ToCharArray())
				{
					if (!Char.IsControl(c))
						invalidPrintable.Write(c);
				}
				MessageBox.Show
					(
					this,
					title + " contains invalid characters." + Environment.NewLine + Environment.NewLine +
					invalidPrintable.ToString() + " are not allowed in file names.",
					"Invalid Characters",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
					);
				return (false);
			}
			return (true);
		}

		private void ExtensionConflictMessage()
		{
			MessageBox.Show
				(
				this,
				"To avoid naming conflicts, raw and neat log files must " +
				"have different extensions. Choose a different extension.",
				"Extension Conflict",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
				);
		}

		private bool ResolveNamingConflicts()
		{
			if ((_settings_Form.SameRawAndNeat) && (_settings_Form.RawExtension == _settings_Form.NeatExtension) &&
				(!_settings_Form.SubdirectoriesFormat && !_settings_Form.NameFormat))
			{
				switch (MessageBox.Show
					(
					this,
					"To avoid naming conflicts, files must either be placed in " +
					"format subdirectories or named by format (Raw/Neat). Do you " +
					"want to place the files in subdirectories (Yes) or name " +
					"them by format (No)? You can also press Cancel and set " +
					"different extensions.",
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: _settings_Form.SubdirectoriesFormat = true; break;
					case DialogResult.No: _settings_Form.NameFormat = true; break;
					default: return (false);
				}
			}

			if ((_settings_Form.MultipleRaw || _settings_Form.MultipleNeat) &&
				(!_settings_Form.SubdirectoriesChannel && !_settings_Form.NameChannel))
			{
				switch (MessageBox.Show
					(
					this,
					"To avoid naming conflicts, files must either be placed in " +
					"channel subdirectories or named by channel (Tx/Bidir/Rx). " +
					"Do you want to place the files in subdirectories (Yes) or " +
					"name them by channel (No)?",
					"Naming Conflict",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: _settings_Form.SubdirectoriesChannel = true; break;
					case DialogResult.No: _settings_Form.NameChannel = true; break;
					default: return (false);
				}
			}
			return (true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
