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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class PredefinedCommandSettings : Form
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

		private SizeF scale = new SizeF(1.0f, 1.0f);

		private StartupControl startupControl = new StartupControl(1, 1);
		private SettingControlsHelper isSettingControls;

		private Model.Settings.PredefinedCommandSettings settings;
		private Model.Settings.PredefinedCommandSettings settingsInEdit;
		private int selectedPage = 1;

		private List<Label> predefinedCommandSettingsSetLabels;
		private List<Controls.PredefinedCommandSettingsSet> predefinedCommandSettingsSets;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <param name="settings">Settings to be displayed.</param>
		/// <param name="terminalType">The terminal type related to the command.</param>
		/// <param name="useExplicitDefaultRadix">Whether to use an explicit default radix.</param>
		/// <param name="parseMode">The parse mode related to the command.</param>
		/// <param name="requestedPage">Page 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
		/// <param name="requestedCommand">Command 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
		public PredefinedCommandSettings(Model.Settings.PredefinedCommandSettings settings, Domain.TerminalType terminalType, bool useExplicitDefaultRadix, Domain.Parser.Modes parseMode, int requestedPage, int requestedCommand)
		{
			InitializeComponent();

			this.settings = settings;
			this.settingsInEdit = new Model.Settings.PredefinedCommandSettings(settings);

			InitializeControls(useExplicitDefaultRadix);

			foreach (Controls.PredefinedCommandSettingsSet s in this.predefinedCommandSettingsSets)
			{
				s.TerminalType            = terminalType;
				s.UseExplicitDefaultRadix = useExplicitDefaultRadix;
				s.ParseMode               = parseMode;
			}

			this.startupControl.RequestedPage    = requestedPage;
			this.startupControl.RequestedCommand = requestedCommand;

			// SetControls() is initially called in the 'Shown' event handler.
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

		#region Form Scaling
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		/// <summary></summary>
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			this.scale = new SizeF(this.scale.Width * factor.Width, this.scale.Height * factor.Height);

			base.ScaleControl(factor, specified);
		}

		#endregion

		#region Form Special Keys
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		/// <remarks>
		/// In case of pressing a modifier key (e.g. [Shift]), this method is invoked twice! Both
		/// invocations will state msg=0x100 (WM_KEYDOWN)! See:
		/// https://msdn.microsoft.com/en-us/library/system.windows.forms.control.processcmdkey.aspx:
		/// The ProcessCmdKey method first determines whether the control has a ContextMenu, and if
		/// so, enables the ContextMenu to process the command key. If the command key is not a menu
		/// shortcut and the control has a parent, the key is passed to the parent's ProcessCmdKey
		/// method. The net effect is that command keys are "bubbled" up the control hierarchy. In
		/// addition to the key the user pressed, the key data also indicates which, if any, modifier
		/// keys were pressed at the same time as the key. Modifier keys include the SHIFT, CTRL, and
		/// ALT keys.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
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
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void PredefinedCommandSettings_Shown(object sender, EventArgs e)
		{
			// Create a page if no pages exist yet:
			int pageCount = this.settingsInEdit.Pages.Count;
			if (pageCount > 0)
			{
				this.selectedPage = Int32Ex.Limit(this.startupControl.RequestedPage, 1, pageCount);
			}
			else
			{
				this.settingsInEdit.CreateDefaultPage();
				this.selectedPage = 1;
			}

			// Initially set controls and validate its contents where needed:
			SetControls();

			int selectedCommand = Int32Ex.Limit(this.startupControl.RequestedCommand, 1, Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage);
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
			{
				GetCommandFromSettingsSet(ControlEx.TagToIndex(sender));
				SetClearControls();
			}
		}

		private void button_ClearPage_Click(object sender, EventArgs e)
		{
			ClearPage();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.settings = this.settingsInEdit;
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

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private int SelectedPageIndex
		{
			get { return (this.selectedPage - 1); }
			set { this.selectedPage = value + 1; }
		}

		#region Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------

		private void InitializeControls(bool useExplicitDefaultRadix)
		{
			if (!useExplicitDefaultRadix) // Default
			{
				label_ExplicitDefaultRadix.Visible = false;
				label_File.Left = (int)((this.scale.Width *   6) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
				label_Data.Left = (int)((this.scale.Width *  54) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
			}
			else
			{
				label_ExplicitDefaultRadix.Visible = true;
				label_File.Left = (int)((this.scale.Width *  87) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
				label_Data.Left = (int)((this.scale.Width * 135) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
			}

			this.predefinedCommandSettingsSetLabels = new List<Label>(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage); // Preset the required capacity to improve memory management.
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

			this.predefinedCommandSettingsSets = new List<Controls.PredefinedCommandSettingsSet>(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage); // Preset the required capacity to improve memory management.
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
			SetClearControls();
		}

		private void SetPagesControls()
		{
			this.isSettingControls.Enter();
			try
			{
				int pageCount = this.settingsInEdit.Pages.Count;
				bool pageIsSelected = (this.selectedPage != 0);

				// Page list:
				if (pageCount > 0)
				{
					listBox_Pages.Enabled = true;
					listBox_Pages.Items.Clear();

					foreach (Model.Types.PredefinedCommandPage p in this.settingsInEdit.Pages)
						listBox_Pages.Items.Add(p.PageName);

					if (pageIsSelected)
						listBox_Pages.SelectedIndex = SelectedPageIndex;
				}
				else
				{
					listBox_Pages.Enabled = false;
					listBox_Pages.Items.Clear();
				}

				// Page list buttons:
				button_NamePage.Enabled = pageIsSelected;
				button_InsertPage.Enabled = pageIsSelected;
				button_AddPage.Enabled = true;
				button_CopyPage.Enabled = pageIsSelected;
				button_DeletePage.Enabled = pageIsSelected;
				button_MovePageUp.Enabled = pageIsSelected && (this.selectedPage > 1);
				button_MovePageDown.Enabled = pageIsSelected && (this.selectedPage < pageCount);
				button_DeletePages.Enabled = (pageCount > 0);

				// Selected page:
				if (pageIsSelected)
					groupBox_Page.Text = this.settingsInEdit.Pages[SelectedPageIndex].PageName;
				else
					groupBox_Page.Text = "<No Page Selected>";
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetPageControls()
		{
			this.isSettingControls.Enter();
			try
			{
				if (this.selectedPage != 0)
				{
					groupBox_Page.Enabled = true;

					int pageCount = this.settingsInEdit.Pages.Count;
					int commandCount = 0;
					if (pageCount >= this.selectedPage)
						commandCount = this.settingsInEdit.Pages[SelectedPageIndex].Commands.Count;

					for (int i = 0; i < commandCount; i++)
						this.predefinedCommandSettingsSets[i].Command = this.settingsInEdit.Pages[SelectedPageIndex].Commands[i];

					for (int i = commandCount; i < Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage; i++)
						this.predefinedCommandSettingsSets[i].Command = new Model.Types.Command();
				}
				else
				{
					groupBox_Page.Enabled = true;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetClearControls()
		{
			this.isSettingControls.Enter();
			try
			{
				int pageCount = this.settingsInEdit.Pages.Count;
				int commandCount = 0;
				if (pageCount >= this.selectedPage)
					commandCount = this.settingsInEdit.Pages[SelectedPageIndex].Commands.Count;

				button_ClearPage.Enabled = (commandCount > 0);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Non-Public Methods > Pages
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Pages
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
				this.settingsInEdit.Pages[SelectedPageIndex].PageName,
				out pageName
				)
				== DialogResult.OK)
			{
				this.settingsInEdit.Pages[SelectedPageIndex].PageName = pageName;
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
				this.settingsInEdit.Pages.Insert(SelectedPageIndex, pcp);
				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void AddPage()
		{
			int pageNumber = this.settingsInEdit.Pages.Count + 1;
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
				this.settingsInEdit.Pages.Add(pcp);
				this.selectedPage = this.settingsInEdit.Pages.Count;
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
				this.settingsInEdit.Pages[SelectedPageIndex].PageName + " (copy)",
				out pageName
				)
				== DialogResult.OK)
			{
				Model.Types.PredefinedCommandPage pcp = new Model.Types.PredefinedCommandPage(this.settingsInEdit.Pages[SelectedPageIndex]);
				pcp.PageName = pageName;
				this.settingsInEdit.Pages.Insert(SelectedPageIndex + 1, pcp);
				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void DeletePage()
		{
			if (MessageBoxEx.Show
				(
				this,
				"Delete page '" + this.settingsInEdit.Pages[SelectedPageIndex].PageName + "'?",
				"Delete?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				this.settingsInEdit.Pages.RemoveAt(SelectedPageIndex);
				this.selectedPage = Int32Ex.Limit(this.selectedPage, 1, this.settingsInEdit.Pages.Count);
				SetControls();
			}
		}

		private void MovePageUp()
		{
			Model.Types.PredefinedCommandPage pcp = this.settingsInEdit.Pages[SelectedPageIndex];
			this.settingsInEdit.Pages.RemoveAt(SelectedPageIndex);
			this.selectedPage--;
			this.settingsInEdit.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		private void MovePageDown()
		{
			Model.Types.PredefinedCommandPage pcp = this.settingsInEdit.Pages[SelectedPageIndex];
			this.settingsInEdit.Pages.RemoveAt(SelectedPageIndex);
			this.selectedPage++;
			this.settingsInEdit.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void DeletePages()
		{
			if (MessageBoxEx.Show
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
				this.settingsInEdit.CreateDefaultPage();
				this.selectedPage = 1;
				SetControls();
			}
		}

		#endregion

		#region Non-Public Methods > Selected Page
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Selected Page
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <param name="command">Command 1..<see cref="Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage"/>.</param>
		private void GetCommandFromSettingsSet(int command)
		{
			Model.Types.PredefinedCommandPage page = this.settingsInEdit.Pages[SelectedPageIndex];
			page.SetCommand(command - 1, this.predefinedCommandSettingsSets[command - 1].Command);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ClearPage()
		{
			if (MessageBoxEx.Show
				(
				this,
				"Clear all commands of page '" + this.settingsInEdit.Pages[SelectedPageIndex].PageName + "'?",
				"Clear?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				this.settingsInEdit.Pages[SelectedPageIndex].Commands.Clear();
				SetControls();
			}
		}

		#endregion

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowHelp()
		{
			MessageBoxEx.Show
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
