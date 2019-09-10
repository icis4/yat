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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.IO;
using MKY.Windows.Forms;

using YAT.Application.Settings;
using YAT.Application.Utilities;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class LogSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Log.Settings.LogSettings settings;
		private Log.Settings.LogSettings settingsInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public LogSettings(Log.Settings.LogSettings settings)
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
		public Log.Settings.LogSettings SettingsResult
		{
			get { return (this.settings); }
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void KeepAndCloneAndAttachSettings(Log.Settings.LogSettings settings)
		{
			this.settings = settings;
			this.settingsInEdit = new Log.Settings.LogSettings(settings); // Clone to ensure decoupling.
			this.settingsInEdit.Changed += settings_Form_Changed;

			// Note that extensions are handled via 'ApplicationSettings'.
		}

		private void DetachAndAcceptSettings()
		{
			this.settingsInEdit.Changed -= settings_Form_Changed;
			this.settings = this.settingsInEdit;

			ApplicationSettings.RoamingUserSettings.Extensions.PortLogFiles = this.settings.PortExtension;
			ApplicationSettings.RoamingUserSettings.Extensions.RawLogFiles  = this.settings.RawExtension;
			ApplicationSettings.RoamingUserSettings.Extensions.NeatLogFiles = this.settings.NeatExtension;
			ApplicationSettings.SaveRoamingUserSettings();
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
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void LogSettings_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		private void LogSettings_Deactivate(object sender, EventArgs e)
		{
			comboBox_Raw_Extension        .OnFormDeactivateWorkaround();
			comboBox_Neat_Extension       .OnFormDeactivateWorkaround();
			comboBox_Options_NameSeparator.OnFormDeactivateWorkaround();
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void button_RootOpen_Click(object sender, EventArgs e)
		{
			// Create directory if not existing yet:
			if (!Directory.Exists(Path.GetDirectoryName(this.settingsInEdit.RootPath)))
			{
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(this.settingsInEdit.RootPath));
				}
				catch (Exception exCreate)
				{
					string message = "Unable to create folder." + Environment.NewLine + Environment.NewLine +
					                 "System error message:" + Environment.NewLine + exCreate.Message;

					MessageBoxEx.Show
					(
						Parent,
						message,
						"Folder Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);

					return;
				}
			}

			// Open directory:
			Exception exBrowse;
			if (!DirectoryEx.TryBrowse(this.settingsInEdit.RootPath, out exBrowse))
			{
				string message = "Unable to open folder." + Environment.NewLine + Environment.NewLine +
				                 "System error message:" + Environment.NewLine + exBrowse.Message;

				MessageBoxEx.Show
				(
					Parent,
					message,
					"Folder Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return;
			}
		}

		private void checkBox_Port_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.PortLog = checkBox_Port.Checked;
		}

		private void pathLabel_Port_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.PortLog = !this.settingsInEdit.PortLog;
		}

	////private void comboBox_Port_Extension_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since        "      _Validating() below gets called anyway.

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_Port_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFileNamePart(comboBox_Port_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}

			if ((this.settingsInEdit.Multiple && (StringEx.EqualsOrdinalIgnoreCase(comboBox_Port_Extension.Text, this.settingsInEdit.RawExtension) || StringEx.EqualsOrdinalIgnoreCase(comboBox_Port_Extension.Text, this.settingsInEdit.NeatExtension))) &&
			    (!(this.settingsInEdit.FolderType || this.settingsInEdit.NameType)))
			{
				ExtensionConflictMessage();
				e.Cancel = true;
				return;
			}

			SetControls();
		}

		private void comboBox_Port_Extension_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.PortExtension = comboBox_Port_Extension.Text;
		}

		private void checkBox_Raw_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.RawLogTx = checkBox_Raw_Tx.Checked;
		}

		private void pathLabel_Raw_Tx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.RawLogTx = !this.settingsInEdit.RawLogTx;
		}

		private void checkBox_Raw_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.RawLogBidir = checkBox_Raw_Bidir.Checked;
		}

		private void pathLabel_Raw_Bidir_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.RawLogBidir = !this.settingsInEdit.RawLogBidir;
		}

		private void checkBox_Raw_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.RawLogRx = checkBox_Raw_Rx.Checked;
		}

		private void pathLabel_Raw_Rx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.RawLogRx = !this.settingsInEdit.RawLogRx;
		}

	////private void comboBox_Raw_Extension_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since       "      _Validating() below gets called anyway.

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_Raw_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFileNamePart(comboBox_Raw_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}

			if ((this.settingsInEdit.Multiple && (StringEx.EqualsOrdinalIgnoreCase(comboBox_Raw_Extension.Text, this.settingsInEdit.PortExtension) || StringEx.EqualsOrdinalIgnoreCase(comboBox_Raw_Extension.Text, this.settingsInEdit.NeatExtension))) &&
			    (!(this.settingsInEdit.FolderType || this.settingsInEdit.NameType)))
			{
				ExtensionConflictMessage();
				e.Cancel = true;
				return;
			}

			SetControls();
		}

		private void comboBox_Raw_Extension_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.RawExtension = comboBox_Raw_Extension.Text;
		}

		private void checkBox_Neat_Tx_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NeatLogTx = checkBox_Neat_Tx.Checked;
		}

		private void pathLabel_Neat_Tx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.NeatLogTx = !this.settingsInEdit.NeatLogTx;
		}

		private void checkBox_Neat_Bidir_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NeatLogBidir = checkBox_Neat_Bidir.Checked;
		}

		private void pathLabel_Neat_Bidir_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.NeatLogBidir = !this.settingsInEdit.NeatLogBidir;
		}

		private void checkBox_Neat_Rx_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NeatLogRx = checkBox_Neat_Rx.Checked;
		}

		private void pathLabel_Neat_Rx_Click(object sender, EventArgs e)
		{
			this.settingsInEdit.NeatLogRx = !this.settingsInEdit.NeatLogRx;
		}

	////private void comboBox_Neat_Extension_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since        "      _Validating() below gets called anyway.

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_Neat_Extension_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateFileNamePart(comboBox_Neat_Extension.Text, "Extension"))
			{
				e.Cancel = true;
				return;
			}

			if ((this.settingsInEdit.Multiple && (StringEx.EqualsOrdinalIgnoreCase(comboBox_Neat_Extension.Text, this.settingsInEdit.PortExtension) || StringEx.EqualsOrdinalIgnoreCase(comboBox_Neat_Extension.Text, this.settingsInEdit.RawExtension))) &&
			    (!(this.settingsInEdit.FolderType || this.settingsInEdit.NameType)))
			{
				ExtensionConflictMessage();
				e.Cancel = true;
				return;
			}

			SetControls();
		}

		private void comboBox_Neat_Extension_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NeatExtension = comboBox_Neat_Extension.Text;
		}

		private void checkBox_Options_NameType_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NameType = checkBox_Options_NameType.Checked;
		}

		private void checkBox_Options_NameDirection_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NameDirection = checkBox_Options_NameDirection.Checked;
		}

		private void checkBox_Options_NameDate_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NameDate = checkBox_Options_NameDate.Checked;
		}

		private void checkBox_Options_NameTime_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.NameTime = checkBox_Options_NameTime.Checked;
		}

	////private void comboBox_Options_NameSeparator_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since           "          _Validating() below gets called anyway.

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_Options_NameSeparator_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			Log.FileNameSeparatorEx separator;
			if (Log.FileNameSeparatorEx.TryParse(comboBox_Options_NameSeparator.Text, out separator))
			{
				this.settingsInEdit.NameSeparator = separator;
			}
			else
			{
				if (ValidateFileNamePart(comboBox_Options_NameSeparator.Text, "Separator"))
					this.settingsInEdit.NameSeparator = comboBox_Options_NameSeparator.Text;
				else
					e.Cancel = true;
			}
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_Options_NameSeparator_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Log.FileNameSeparatorEx separator;
			if (Log.FileNameSeparatorEx.TryParse(comboBox_Options_NameSeparator.Text, out separator))
			{
				this.settingsInEdit.NameSeparator = separator;
			}
			else
			{
				if (ValidateFileNamePart(comboBox_Options_NameSeparator.Text, "Separator"))
					this.settingsInEdit.NameSeparator = comboBox_Options_NameSeparator.Text;
			}
		}

		private void checkBox_Options_FolderType_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.FolderType = checkBox_Options_FolderType.Checked;
		}

		private void checkBox_Options_FolderDirection_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.FolderDirection = checkBox_Options_FolderDirection.Checked;
		}

		private void radioButton_Options_ModeCreate_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (radioButton_Options_ModeCreate.Checked)
			{
				this.settingsInEdit.WriteMode = Log.LogFileWriteMode.Create;
				this.settingsInEdit.NameDate = this.settings.NameDate;
				this.settingsInEdit.NameTime = this.settings.NameTime;
			}
		}

		private void radioButton_Options_ModeAppend_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (radioButton_Options_ModeAppend.Checked)
			{
				this.settingsInEdit.WriteMode = Log.LogFileWriteMode.Append;
				this.settingsInEdit.NameDate = false;
				this.settingsInEdit.NameTime = false;
			}
		}

		private void radioButton_Options_TextEncodingUTF8_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (radioButton_Options_TextEncodingUTF8.Checked)
				this.settingsInEdit.TextEncoding = Log.TextEncoding.UTF8;
		}

		private void radioButton_Options_TextEncodingTerminal_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (radioButton_Options_TextEncodingTerminal.Checked)
				this.settingsInEdit.TextEncoding = Log.TextEncoding.Terminal;
		}

		private void checkBox_Options_EmitEncodingPreamble_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.EmitEncodingPreamble = checkBox_Options_EmitEncodingPreamble.Checked;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			if (InhibitNamingConflicts())
				DetachAndAcceptSettings();
			else
				DialogResult = DialogResult.None;
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
				"Reset all settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				ApplicationSettings.RoamingUserSettings.Extensions.PortLogFiles = ExtensionSettings.PortLogFilesDefault;
				ApplicationSettings.RoamingUserSettings.Extensions.RawLogFiles  = ExtensionSettings.RawLogFilesDefault;
				ApplicationSettings.RoamingUserSettings.Extensions.NeatLogFiles = ExtensionSettings.NeatLogFilesDefault;

				this.settingsInEdit.SetDefaults();

				// Note that SetDefaults() will trigger SetControls().
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
				comboBox_Port_Extension.Items.Clear();
				comboBox_Port_Extension.Items.AddRange(ExtensionHelper.PortLogFileExtensionsWithDot.ToArray());

				comboBox_Raw_Extension.Items.Clear();
				comboBox_Raw_Extension.Items.AddRange(ExtensionHelper.RawLogFileExtensionsWithDot.ToArray());

				comboBox_Neat_Extension.Items.Clear();
				comboBox_Neat_Extension.Items.AddRange(ExtensionHelper.NeatLogFileExtensionsWithDot.ToArray());

				comboBox_Options_NameSeparator.Items.Clear();
				comboBox_Options_NameSeparator.Items.AddRange(Log.FileNameSeparatorEx.GetItems());
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
				if (!string.IsNullOrEmpty(this.settingsInEdit.RootPath))
					pathLabel_Root.Text = this.settingsInEdit.RootPath + Path.DirectorySeparatorChar + this.settingsInEdit.RootFileName;
				else
					pathLabel_Root.Text = "<Set a root file...>";

				checkBox_Port.Checked = this.settingsInEdit.PortLog;
				checkBox_Port.Checked = this.settingsInEdit.PortLog;
				checkBox_Port.Checked = this.settingsInEdit.PortLog;

				pathLabel_Port.Text = this.settingsInEdit.PortRootRelativeFilePath;
				pathLabel_Port.Text = this.settingsInEdit.PortRootRelativeFilePath;
				pathLabel_Port.Text = this.settingsInEdit.PortRootRelativeFilePath;

				comboBox_Port_Extension.Text = this.settingsInEdit.PortExtension;

				checkBox_Raw_Tx.Checked    = this.settingsInEdit.RawLogTx;
				checkBox_Raw_Bidir.Checked = this.settingsInEdit.RawLogBidir;
				checkBox_Raw_Rx.Checked    = this.settingsInEdit.RawLogRx;

				pathLabel_Raw_Tx.Text    = this.settingsInEdit.RawTxRootRelativeFilePath;
				pathLabel_Raw_Bidir.Text = this.settingsInEdit.RawBidirRootRelativeFilePath;
				pathLabel_Raw_Rx.Text    = this.settingsInEdit.RawRxRootRelativeFilePath;

				comboBox_Raw_Extension.Text = this.settingsInEdit.RawExtension;

				checkBox_Neat_Tx.Checked    = this.settingsInEdit.NeatLogTx;
				checkBox_Neat_Bidir.Checked = this.settingsInEdit.NeatLogBidir;
				checkBox_Neat_Rx.Checked    = this.settingsInEdit.NeatLogRx;

				pathLabel_Neat_Tx.Text    = this.settingsInEdit.NeatTxRootRelativeFilePath;
				pathLabel_Neat_Bidir.Text = this.settingsInEdit.NeatBidirRootRelativeFilePath;
				pathLabel_Neat_Rx.Text    = this.settingsInEdit.NeatRxRootRelativeFilePath;

				comboBox_Neat_Extension.Text = this.settingsInEdit.NeatExtension;

				checkBox_Options_NameType.Checked      = this.settingsInEdit.NameType;
				checkBox_Options_NameDirection.Checked = this.settingsInEdit.NameDirection;
				checkBox_Options_NameDate.Checked      = this.settingsInEdit.NameDate;
				checkBox_Options_NameTime.Checked      = this.settingsInEdit.NameTime;

				Log.FileNameSeparatorEx separator = this.settingsInEdit.NameSeparator;
				ComboBoxHelper.Select(comboBox_Options_NameSeparator, separator, separator);

				bool dateTimeEnabled = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Create);
				checkBox_Options_NameDate.Enabled = dateTimeEnabled;
				checkBox_Options_NameTime.Enabled = dateTimeEnabled;

				checkBox_Options_FolderType.Checked      = this.settingsInEdit.FolderType;
				checkBox_Options_FolderDirection.Checked = this.settingsInEdit.FolderDirection;

				radioButton_Options_ModeCreate.Checked = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Create);
				radioButton_Options_ModeAppend.Checked = (this.settingsInEdit.WriteMode == Log.LogFileWriteMode.Append);

				if (this.settingsInEdit.TextEncodingIsSupported)
				{
					groupBox_Options_TextEncoding.Enabled            =  true;
					radioButton_Options_TextEncodingUTF8.Checked     = (this.settingsInEdit.TextEncoding == Log.TextEncoding.UTF8);
					radioButton_Options_TextEncodingTerminal.Checked = (this.settingsInEdit.TextEncoding == Log.TextEncoding.Terminal);
					checkBox_Options_EmitEncodingPreamble.Checked    =  this.settingsInEdit.EmitEncodingPreamble;
					checkBox_Options_EmitEncodingPreamble.Text       = (this.settingsInEdit.EmitEncodingPreamble ? "with BOM" : "without BOM");
				}
				else
				{
					groupBox_Options_TextEncoding.Enabled            = false;
					radioButton_Options_TextEncodingUTF8.Checked     = true; // Show default, XML is UTF-8 too, RTF don't care.
					radioButton_Options_TextEncodingTerminal.Checked = false;
					checkBox_Options_EmitEncodingPreamble.Checked    = true; // Show default, XML is UTF-8 too, RTF don't care.
					checkBox_Options_EmitEncodingPreamble.Text       = "with BOM";
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowSetRootDirectoryDialog()
		{
			var ofd = new OpenFileDialog();
			ofd.Title = "Set Root";
			ofd.Filter = ExtensionHelper.AllFilesFilter;

			if (Directory.Exists(this.settingsInEdit.RootPath))
				ofd.InitialDirectory = this.settingsInEdit.RootPath;
			else
				ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.LogFiles;

			ofd.FileName = this.settingsInEdit.RootFileName;
			ofd.CheckPathExists = false;
			ofd.CheckFileExists = false;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.LogFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				this.settingsInEdit.RootPath = Path.GetDirectoryName(ofd.FileName);
				this.settingsInEdit.RootFileName = Path.GetFileNameWithoutExtension(ofd.FileName);
			}
		}

		//------------------------------------------------------------------------------------------
		// Validation
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private bool ValidateFileNamePart(string fileNamePart, string title)
		{
			char[] invalid = Path.GetInvalidFileNameChars();
			if (!StringEx.ContainsAny(fileNamePart, invalid))
			{
				return (true);
			}
			else
			{
				var sb = new StringBuilder(invalid.Length);
				sb.Append(title);
				sb.AppendLine(" contains invalid characters.");
				sb.AppendLine();

				foreach (char c in invalid)
				{
					if (!char.IsControl(c) && !char.IsWhiteSpace(c))
						sb.Append(c);
				}

				sb.Append(" are not allowed in file names.");

				MessageBoxEx.Show
				(
					this,
					sb.ToString(),
					"Invalid Characters",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);

				return (false);
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ExtensionConflictMessage()
		{
			string message =
				"To avoid naming conflicts, files must either be named by type or separated into folders or have different extensions. " +
				Environment.NewLine + Environment.NewLine +
				"First, select a different extension. You may then change one of the other options to resolve the naming conflict.";

			MessageBoxEx.Show
			(
				this,
				message,
				"Extension Conflict",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
			);
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private bool InhibitNamingConflicts()
		{
			if ((this.settingsInEdit.SameDirection) && (this.settingsInEdit.SameExtension) &&
			    (!(this.settingsInEdit.FolderType || this.settingsInEdit.NameType)))
			{
				var sb = new StringBuilder();
				sb.AppendLine("To avoid naming conflicts, files must either be named by type or separated into folders (Port/Raw/Neat) or have different extensions.");
				sb.AppendLine();
				sb.AppendLine("Do you want to name the files by type [Yes] or separate them into folders [No]?");
				sb.AppendLine();
				sb.Append    ("You may also cancel and set different extensions, or manually change the settings.");

				switch (MessageBoxEx.Show
					(
						this,
						sb.ToString(),
						"Naming Conflict",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settingsInEdit.NameType   = true; break;
					case DialogResult.No:  this.settingsInEdit.FolderType = true; break;
					default: return (false);
				}
			}

			if ((this.settingsInEdit.SameType) &&
			    (!(this.settingsInEdit.FolderDirection || this.settingsInEdit.NameDirection)))
			{
				string message =
					"To avoid naming conflicts, files must either be named by direction or separated into folders (Tx/Bidir/Rx)." +
					Environment.NewLine + Environment.NewLine +
					"Do you want to name the files by direction [Yes] or separate them into folders [No]?" +
					Environment.NewLine + Environment.NewLine +
					"You may also cancel and manually change the settings.";

				switch (MessageBoxEx.Show
					(
						this,
						message,
						"Naming Conflict",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settingsInEdit.NameDirection   = true; break;
					case DialogResult.No:  this.settingsInEdit.FolderDirection = true; break;
					default: return (false);
				}
			}

			if ((this.settingsInEdit.AnyPort) && (this.settingsInEdit.AnyRaw || this.settingsInEdit.AnyNeat) && (this.settingsInEdit.SameExtension) &&
			    (!(this.settingsInEdit.FolderType      || this.settingsInEdit.NameType)) &&
			    (!(this.settingsInEdit.FolderDirection || this.settingsInEdit.NameDirection))) // Special tricky case since port is 'any' direction.
			{
				string message =
					"To avoid naming conflicts, files must either be named by type or separated into folders (Port/Raw/Neat) or have different extensions." +
					Environment.NewLine + Environment.NewLine +
					"Do you want to name the files by type [Yes] or separate them into folders [No]?" +
					Environment.NewLine + Environment.NewLine +
					"You may also cancel and set different extensions, or manually change the settings.";

				switch (MessageBoxEx.Show
					(
						this,
						message,
						"Naming Conflict",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
					))
				{
					case DialogResult.Yes: this.settingsInEdit.NameType   = true; break;
					case DialogResult.No:  this.settingsInEdit.FolderType = true; break;
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
