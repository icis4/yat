using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.Types;
using MKY.Windows.Forms;

using YAT.Settings.Application;

namespace YAT.Gui.Forms
{
	public partial class PredefinedCommandSettings : System.Windows.Forms.Form
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct StartupControl
		{
			public bool Startup;
			public int RequestedPage;
			public int RequestedCommand;

			public StartupControl(bool startup, int requestedPage, int requestedCommand)
			{
				Startup = startup;
				RequestedPage = requestedPage;
				RequestedCommand = requestedCommand;
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private StartupControl _startupControl = new StartupControl(true, 1, 1);
		private bool _isSettingControls = false;

		private Model.Settings.PredefinedCommandSettings _settings;
		private Model.Settings.PredefinedCommandSettings _settings_Form;
		private int _selectedPage = 1;

		private List<Label> _predefinedCommandSettingsSetLabels;
		private List<Controls.PredefinedCommandSettingsSet> _predefinedCommandSettingsSets;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public PredefinedCommandSettings(Model.Settings.PredefinedCommandSettings settings, int requestedPage, int requestedCommand)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Model.Settings.PredefinedCommandSettings(settings);
			_startupControl.RequestedPage = requestedPage;
			_startupControl.RequestedCommand = requestedCommand;
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Model.Settings.PredefinedCommandSettings SettingsResult
		{
			get { return (_settings); }
		}

		public int SelectedPage
		{
			get { return (_selectedPage); }
		}

		#endregion

		#region Form Special Keys
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.F1)
				return (false);
			else
				return (base.IsInputKey(keyData));
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.F1)
			{
				ShowHelp();
				return (true);
			}
			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		private void PredefinedCommandSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_startupControl.Startup)
			{
				_startupControl.Startup = false;

				// create a page if no pages exist yet
				int pageCount = _settings_Form.Pages.Count;
				if (pageCount > 0)
				{
					_selectedPage = XInt32.LimitToBounds(_startupControl.RequestedPage, 1, pageCount);
				}
				else
				{
					_settings_Form.CreateDefaultPage();
					_selectedPage = 1;
				}

				// initially set controls and validate its contents where needed
				SetControls();

				int selectedCommand = XInt32.LimitToBounds(_startupControl.RequestedCommand, 1, Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage);
				_predefinedCommandSettingsSetLabels[selectedCommand - 1].Select();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void listBox_Pages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				SelectedPageIndex = listBox_Pages.SelectedIndex;
				SetControls();
			}
		}

		private void button_NamePage_Click(object sender, EventArgs e)
		{
			NamePage();
		}

		private void button_InsertPage_Click(object sender, EventArgs e)
		{
			InsertPage();
		}

		private void button_AddPage_Click(object sender, EventArgs e)
		{
			AddPage();
		}

		private void button_DeletePage_Click(object sender, EventArgs e)
		{
			DeletePage();
		}

		private void button_MovePageUp_Click(object sender, EventArgs e)
		{
			MovePageUp();
		}

		private void button_MovePageDown_Click(object sender, EventArgs e)
		{
			MovePageDown();
		}

		private void button_DeletePages_Click(object sender, EventArgs e)
		{
			DeletePages();
		}

		private void predefinedCommandSettingsSet_CommandChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				GetCommand(int.Parse((string)(((Controls.PredefinedCommandSettingsSet)sender).Tag)));
		}

		private void button_ClearPage_Click(object sender, EventArgs e)
		{
			ClearPage();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			_settings = _settings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Help_Click(object sender, EventArgs e)
		{
			ShowHelp();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private int SelectedPageIndex
		{
			get { return (_selectedPage - 1); }
			set { _selectedPage = value + 1; }
		}

		#region Private Methods > Controls
		//------------------------------------------------------------------------------------------
		// Private Methods > Controls
		//------------------------------------------------------------------------------------------

		private void InitializeControls()
		{
			_predefinedCommandSettingsSetLabels = new List<Label>(Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_1);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_2);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_3);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_4);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_5);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_6);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_7);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_8);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_9);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_10);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_11);
			_predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_12);

			_predefinedCommandSettingsSets = new List<Controls.PredefinedCommandSettingsSet>(Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_1);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_2);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_3);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_4);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_5);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_6);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_7);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_8);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_9);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_10);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_11);
			_predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_12);
		}

		private void SetControls()
		{
			SetPagesControls();
			SetPageControls();
		}

		private void SetPagesControls()
		{
			_isSettingControls = true;

			int pageCount = _settings_Form.Pages.Count;
			bool pageIsSelected = (_selectedPage != 0);

			// page list
			if (pageCount > 0)
			{
				listBox_Pages.Enabled = true;
				listBox_Pages.Items.Clear();

				foreach (Model.Types.PredefinedCommandPage p in _settings_Form.Pages)
					listBox_Pages.Items.Add(p.PageName);

				if (pageIsSelected)
					listBox_Pages.SelectedIndex = SelectedPageIndex;
			}
			else
			{
				listBox_Pages.Enabled = false;
				listBox_Pages.Items.Clear();
			}

			// page list buttons
			button_NamePage.Enabled = pageIsSelected;
			button_InsertPage.Enabled = pageIsSelected;
			button_AddPage.Enabled = true;
			button_DeletePage.Enabled = pageIsSelected;
			button_MovePageUp.Enabled = pageIsSelected && (_selectedPage > 1);
			button_MovePageDown.Enabled = pageIsSelected && (_selectedPage < pageCount);
			button_DeletePages.Enabled = (pageCount > 0);

			// selected page
			if (pageIsSelected)
				groupBox_Page.Text = _settings_Form.Pages[SelectedPageIndex].PageName;
			else
				groupBox_Page.Text = "<No Page Selected>";

			_isSettingControls = false;
		}

		private void SetPageControls()
		{
			_isSettingControls = true;

			if (_selectedPage != 0)
			{
				groupBox_Page.Enabled = true;

				int pageCount = _settings_Form.Pages.Count;
				int commandCount = 0;
				if (pageCount >= _selectedPage)
					commandCount = _settings_Form.Pages[SelectedPageIndex].Commands.Count;

				for (int i = 0; i < commandCount; i++)
					_predefinedCommandSettingsSets[i].Command = _settings_Form.Pages[SelectedPageIndex].Commands[i];

				for (int i = commandCount; i < Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage; i++)
					_predefinedCommandSettingsSets[i].Command = null;

				button_ClearPage.Enabled = (commandCount > 0);
			}
			else
			{
				groupBox_Page.Enabled = true;
			}

			_isSettingControls = false;
		}

		#endregion

		#region Private Methods > Pages
		//------------------------------------------------------------------------------------------
		// Private Methods > Pages
		//------------------------------------------------------------------------------------------

		private void NamePage()
		{
			string pageName;
			if (TextInputBox.Show
				(
				this,
				"Enter page name:",
				"Page Name",
				_settings_Form.Pages[SelectedPageIndex].PageName,
				out pageName
				)
				== DialogResult.OK)
			{
				_settings_Form.Pages[SelectedPageIndex].PageName = pageName;
				SetPagesControls();
			}
		}

		private void InsertPage()
		{
			int pageNumber = _selectedPage;
			string pageName;
			if (TextInputBox.Show
				(
				this,
				"Enter page name:",
				"Page Name",
				"Page " + pageNumber.ToString(),
				out pageName
				)
				== DialogResult.OK)
			{
				Model.Types.PredefinedCommandPage pcp = new Model.Types.PredefinedCommandPage(Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage, pageName);
				_settings_Form.Pages.Insert(SelectedPageIndex, pcp);
				SetControls();
			}
		}

		private void AddPage()
		{
			int pageNumber = _settings_Form.Pages.Count + 1;
			string pageName;
			if (TextInputBox.Show
				(
				this,
				"Enter page name:",
				"Page Name",
				"Page " + pageNumber.ToString(),
				out pageName
				)
				== DialogResult.OK)
			{
				Model.Types.PredefinedCommandPage pcp = new Model.Types.PredefinedCommandPage(Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage, pageName);
				_settings_Form.Pages.Add(pcp);
				_selectedPage = _settings_Form.Pages.Count;
				SetControls();
			}
		}

		private void DeletePage()
		{
			if (MessageBox.Show
				 (
				 this,
				 "Delete page \"" + _settings_Form.Pages[SelectedPageIndex].PageName + "\"?",
				 "Delete?",
				 MessageBoxButtons.YesNoCancel,
				 MessageBoxIcon.Question,
				 MessageBoxDefaultButton.Button2
				 )
				 == DialogResult.Yes)
			{
				_settings_Form.Pages.RemoveAt(SelectedPageIndex);
				_selectedPage = XInt32.LimitToBounds(_selectedPage, 1, _settings_Form.Pages.Count);
				SetControls();
			}
		}

		private void MovePageUp()
		{
			Model.Types.PredefinedCommandPage pcp = _settings_Form.Pages[SelectedPageIndex];
			_settings_Form.Pages.RemoveAt(SelectedPageIndex);
			_selectedPage--;
			_settings_Form.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		private void MovePageDown()
		{
			Model.Types.PredefinedCommandPage pcp = _settings_Form.Pages[SelectedPageIndex];
			_settings_Form.Pages.RemoveAt(SelectedPageIndex);
			_selectedPage++;
			_settings_Form.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		private void DeletePages()
		{
			if (MessageBox.Show
				 (
				 this,
				 "Delete all pages?",
				 "Delete All?",
				 MessageBoxButtons.YesNoCancel,
				 MessageBoxIcon.Question,
				 MessageBoxDefaultButton.Button2
				 )
				 == DialogResult.Yes)
			{
				_settings_Form.CreateDefaultPage();
				_selectedPage = 1;
				SetControls();
			}
		}

		#endregion

		#region Private Methods > Selected Page
		//------------------------------------------------------------------------------------------
		// Private Methods > Selected Page
		//------------------------------------------------------------------------------------------

		private void GetCommand(int command)
		{
			List<Model.Types.Command> page = _settings_Form.Pages[SelectedPageIndex].Commands;

			if (page.Count >= command)
			{
				page[command - 1] = _predefinedCommandSettingsSets[command - 1].Command;
			}
			else
			{
				while (page.Count < (command - 1))
					page.Add(new Model.Types.Command());

				page.Add(_predefinedCommandSettingsSets[command - 1].Command);
			}
		}

		private void ClearPage()
		{
			if (MessageBox.Show
				 (
				 this,
				 "Clear all commands of page \"" + _settings_Form.Pages[SelectedPageIndex].PageName + "\"?",
				 "Clear?",
				 MessageBoxButtons.YesNoCancel,
				 MessageBoxIcon.Question,
				 MessageBoxDefaultButton.Button2
				 )
				 == DialogResult.Yes)
			{
				_settings_Form.Pages[SelectedPageIndex].Commands.Clear();
				SetControls();
			}
		}

		#endregion

		private void ShowHelp()
		{
			MessageBox.Show
				(
				this,
				"Command Format:" + Environment.NewLine + Environment.NewLine + Domain.Parser.Parser.FormatHelp,
				"Command Format Help",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
				);
		}

		#endregion
	}
}
