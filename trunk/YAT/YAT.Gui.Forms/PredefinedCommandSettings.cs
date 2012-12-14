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
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class PredefinedCommandSettings : System.Windows.Forms.Form
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct StartupControl
		{
			/// <summary></summary>
			public int RequestedPage;

			/// <summary></summary>
			public int RequestedCommand;

			/// <param name="requestedPage">Page 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
			/// <param name="requestedCommand">Command 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
			public StartupControl(int requestedPage, int requestedCommand)
			{
				RequestedPage    = requestedPage;
				RequestedCommand = requestedCommand;
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private StartupControl startupControl = new StartupControl(1, 1);
		private SettingControlsHelper isSettingControls;

		private Model.Settings.PredefinedCommandSettings settings;
		private Model.Settings.PredefinedCommandSettings settings_Form;
		private int selectedPage = 1;

		private List<Label> predefinedCommandSettingsSetLabels;
		private List<Controls.PredefinedCommandSettingsSet> predefinedCommandSettingsSets;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <param name="settings">Settings to be displayed.</param>
		/// <param name="requestedPage">Page 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
		/// <param name="requestedCommand">Command 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
		public PredefinedCommandSettings(Model.Settings.PredefinedCommandSettings settings, int requestedPage, int requestedCommand)
		{
			InitializeComponent();

			this.settings = settings;
			this.settings_Form = new Model.Settings.PredefinedCommandSettings(settings);
			this.startupControl.RequestedPage = requestedPage;
			this.startupControl.RequestedCommand = requestedCommand;
			InitializeControls();
		
			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Model.Settings.PredefinedCommandSettings SettingsResult
		{
			get { return (this.settings); }
		}

		/// <summary></summary>
		public int SelectedPage
		{
			get { return (this.selectedPage); }
		}

		#endregion

		#region Form Special Keys
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		/// <summary></summary>
		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.F1)
				return (false);
			else
				return (base.IsInputKey(keyData));
		}

		/// <summary></summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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
		private void PredefinedCommandSettings_Shown(object sender, EventArgs e)
		{
			// Create a page if no pages exist yet:
			int pageCount = this.settings_Form.Pages.Count;
			if (pageCount > 0)
			{
				this.selectedPage = Int32Ex.LimitToBounds(this.startupControl.RequestedPage, 1, pageCount);
			}
			else
			{
				this.settings_Form.CreateDefaultPage();
				this.selectedPage = 1;
			}

			// Initially set controls and validate its contents where needed:
			SetControls();

			int selectedCommand = Int32Ex.LimitToBounds(this.startupControl.RequestedCommand, 1, Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage);
			this.predefinedCommandSettingsSetLabels[selectedCommand - 1].Select();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void listBox_Pages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
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

		private void button_CopyPage_Click(object sender, EventArgs e)
		{
			CopyPage();
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
			if (!this.isSettingControls)
				GetCommandFromSettingsSet(int.Parse((string)(((Controls.PredefinedCommandSettingsSet)sender).Tag), NumberFormatInfo.InvariantInfo));
		}

		private void button_ClearPage_Click(object sender, EventArgs e)
		{
			ClearPage();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.settings = this.settings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
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
			get { return (this.selectedPage - 1); }
			set { this.selectedPage = value + 1; }
		}

		#region Private Methods > Controls
		//------------------------------------------------------------------------------------------
		// Private Methods > Controls
		//------------------------------------------------------------------------------------------

		private void InitializeControls()
		{
			this.predefinedCommandSettingsSetLabels = new List<Label>(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_1);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_2);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_3);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_4);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_5);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_6);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_7);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_8);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_9);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_10);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_11);
			this.predefinedCommandSettingsSetLabels.Add(label_predefinedCommandSettingsSet_12);

			this.predefinedCommandSettingsSets = new List<Controls.PredefinedCommandSettingsSet>(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_1);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_2);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_3);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_4);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_5);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_6);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_7);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_8);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_9);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_10);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_11);
			this.predefinedCommandSettingsSets.Add(predefinedCommandSettingsSet_12);
		}

		private void SetControls()
		{
			SetPagesControls();
			SetPageControls();
		}

		private void SetPagesControls()
		{
			this.isSettingControls.Enter();

			int pageCount = this.settings_Form.Pages.Count;
			bool pageIsSelected = (this.selectedPage != 0);

			// Page list.
			if (pageCount > 0)
			{
				listBox_Pages.Enabled = true;
				listBox_Pages.Items.Clear();

				foreach (Model.Types.PredefinedCommandPage p in this.settings_Form.Pages)
					listBox_Pages.Items.Add(p.PageName);

				if (pageIsSelected)
					listBox_Pages.SelectedIndex = SelectedPageIndex;
			}
			else
			{
				listBox_Pages.Enabled = false;
				listBox_Pages.Items.Clear();
			}

			// Page list buttons.
			button_NamePage.Enabled = pageIsSelected;
			button_InsertPage.Enabled = pageIsSelected;
			button_AddPage.Enabled = true;
			button_CopyPage.Enabled = pageIsSelected;
			button_DeletePage.Enabled = pageIsSelected;
			button_MovePageUp.Enabled = pageIsSelected && (this.selectedPage > 1);
			button_MovePageDown.Enabled = pageIsSelected && (this.selectedPage < pageCount);
			button_DeletePages.Enabled = (pageCount > 0);

			// Selected page.
			if (pageIsSelected)
				groupBox_Page.Text = this.settings_Form.Pages[SelectedPageIndex].PageName;
			else
				groupBox_Page.Text = "<No Page Selected>";

			this.isSettingControls.Leave();
		}

		private void SetPageControls()
		{
			this.isSettingControls.Enter();

			if (this.selectedPage != 0)
			{
				groupBox_Page.Enabled = true;

				int pageCount = this.settings_Form.Pages.Count;
				int commandCount = 0;
				if (pageCount >= this.selectedPage)
					commandCount = this.settings_Form.Pages[SelectedPageIndex].Commands.Count;

				for (int i = 0; i < commandCount; i++)
					this.predefinedCommandSettingsSets[i].Command = this.settings_Form.Pages[SelectedPageIndex].Commands[i];

				for (int i = commandCount; i < Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage; i++)
					this.predefinedCommandSettingsSets[i].Command = new Model.Types.Command();

				button_ClearPage.Enabled = (commandCount > 0);
			}
			else
			{
				groupBox_Page.Enabled = true;
			}

			this.isSettingControls.Leave();
		}

		#endregion

		#region Private Methods > Pages
		//------------------------------------------------------------------------------------------
		// Private Methods > Pages
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always)]
		private void NamePage()
		{
			string pageName;
			if (TextInputBox.Show
				(
				this,
				"Enter page name:",
				"Page Name",
				this.settings_Form.Pages[SelectedPageIndex].PageName,
				out pageName
				)
				== DialogResult.OK)
			{
				this.settings_Form.Pages[SelectedPageIndex].PageName = pageName;
				SetPagesControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void InsertPage()
		{
			int pageNumber = this.selectedPage;
			string pageName;
			if (TextInputBox.Show
				(
				this,
				"Enter name of inserted page:",
				"Page Name",
				"Page " + pageNumber,
				out pageName
				)
				== DialogResult.OK)
			{
				Model.Types.PredefinedCommandPage pcp = new Model.Types.PredefinedCommandPage(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage, pageName);
				this.settings_Form.Pages.Insert(SelectedPageIndex, pcp);
				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void AddPage()
		{
			int pageNumber = this.settings_Form.Pages.Count + 1;
			string pageName;
			if (TextInputBox.Show
				(
				this,
				"Enter name of added page:",
				"Page Name",
				"Page " + pageNumber,
				out pageName
				)
				== DialogResult.OK)
			{
				Model.Types.PredefinedCommandPage pcp = new Model.Types.PredefinedCommandPage(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage, pageName);
				this.settings_Form.Pages.Add(pcp);
				this.selectedPage = this.settings_Form.Pages.Count;
				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void CopyPage()
		{
			string pageName;
			if (TextInputBox.Show
				(
				this,
				"Enter name of copy:",
				"Page Name",
				this.settings_Form.Pages[SelectedPageIndex].PageName + " (copy)",
				out pageName
				)
				== DialogResult.OK)
			{
				Model.Types.PredefinedCommandPage pcp = new Model.Types.PredefinedCommandPage(this.settings_Form.Pages[SelectedPageIndex]);
				pcp.PageName = pageName;
				this.settings_Form.Pages.Insert(SelectedPageIndex + 1, pcp);
				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void DeletePage()
		{
			if (MessageBox.Show
				(
				this,
				"Delete page '" + this.settings_Form.Pages[SelectedPageIndex].PageName + "'?",
				"Delete?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				this.settings_Form.Pages.RemoveAt(SelectedPageIndex);
				this.selectedPage = Int32Ex.LimitToBounds(this.selectedPage, 1, this.settings_Form.Pages.Count);
				SetControls();
			}
		}

		private void MovePageUp()
		{
			Model.Types.PredefinedCommandPage pcp = this.settings_Form.Pages[SelectedPageIndex];
			this.settings_Form.Pages.RemoveAt(SelectedPageIndex);
			this.selectedPage--;
			this.settings_Form.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		private void MovePageDown()
		{
			Model.Types.PredefinedCommandPage pcp = this.settings_Form.Pages[SelectedPageIndex];
			this.settings_Form.Pages.RemoveAt(SelectedPageIndex);
			this.selectedPage++;
			this.settings_Form.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
				this.settings_Form.CreateDefaultPage();
				this.selectedPage = 1;
				SetControls();
			}
		}

		#endregion

		#region Private Methods > Selected Page
		//------------------------------------------------------------------------------------------
		// Private Methods > Selected Page
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <param name="command">Command 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
		private void GetCommandFromSettingsSet(int command)
		{
			Model.Types.PredefinedCommandPage page = this.settings_Form.Pages[SelectedPageIndex];
			page.SetCommand(command - 1, this.predefinedCommandSettingsSets[command - 1].Command);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ClearPage()
		{
			if (MessageBox.Show
				(
				this,
				"Clear all commands of page '" + this.settings_Form.Pages[SelectedPageIndex].PageName + "'?",
				"Clear?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				this.settings_Form.Pages[SelectedPageIndex].Commands.Clear();
				SetControls();
			}
		}

		#endregion

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
