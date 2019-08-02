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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.View.Utilities;

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
			public int RequestedPageId;

			/// <summary></summary>
			public int RequestedCommandId;

			/// <param name="requestedPageId">Page 1..<see cref="Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
			/// <param name="requestedCommandId">Command 1..<see cref="Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
			public StartupControl(int requestedPageId, int requestedCommandId)
			{
				RequestedPageId    = requestedPageId;
				RequestedCommandId = requestedCommandId;
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SizeF scale = new SizeF(1.0f, 1.0f);

		private StartupControl startupControl = new StartupControl(1, 1);
		private SettingControlsHelper isSettingControls; // = false;

		private Model.Settings.PredefinedCommandSettings settings;       // = null;
		private Model.Settings.PredefinedCommandSettings settingsInEdit; // = null;
		private int selectedPageId = 1;
		private int selectedSubpageId = 1;
		private string indicatedName;                                    // = null;

		private Point subpageCheckBoxLocationTopLeft;
		private Point subpageCheckBoxLocationLeftAbove;
		private Point subpageCheckBoxLocationCenter;
		private Point subpageCheckBoxLocationRightBelow;
		private Point subpageCheckBoxLocationBottomRight;
		private List<Label> predefinedCommandSettingsSetLabels;                            // = null;
		private List<Controls.PredefinedCommandSettingsSet> predefinedCommandSettingsSets; // = null;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <param name="settings">Settings to be displayed.</param>
		/// <param name="terminalType">The terminal type related to the command.</param>
		/// <param name="useExplicitDefaultRadix">Whether to use an explicit default radix.</param>
		/// <param name="parseModeForText">The parse mode related to the command.</param>
		/// <param name="rootDirectoryForFile">The root path for file commands.</param>
		/// <param name="pageLayout">The current page layout.</param>
		/// <param name="requestedPageId">Page 1..<see cref="Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		/// <param name="requestedCommandId">Command 1..<see cref="Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		/// <param name="indicatedName">The indicated terminal name.</param>
		public PredefinedCommandSettings(Model.Settings.PredefinedCommandSettings settings, Domain.TerminalType terminalType, bool useExplicitDefaultRadix, Domain.Parser.Modes parseModeForText, string rootDirectoryForFile, int requestedPageId, int requestedCommandId, string indicatedName)
		{
			InitializeComponent();

			this.settings = settings;
			this.settingsInEdit = new Model.Settings.PredefinedCommandSettings(settings);

			InitializeControls(useExplicitDefaultRadix);

			foreach (var pcss in this.predefinedCommandSettingsSets)
			{
				pcss.TerminalType            = terminalType;
				pcss.UseExplicitDefaultRadix = useExplicitDefaultRadix;
				pcss.ParseModeForText        = parseModeForText;
				pcss.RootDirectoryForFile    = rootDirectoryForFile;
			}

			this.startupControl.RequestedPageId    = requestedPageId;
			this.startupControl.RequestedCommandId = requestedCommandId;

		////this.selectedPageId will be set by PredefinedCommandSettings_Shown().
			this.indicatedName = indicatedName;

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
		public int SelectedPageId
		{
			get { return (this.selectedPageId); }
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
			int pageCount = this.settingsInEdit.Pages.Count;
			if (pageCount > 0)
			{
				this.selectedPageId = Int32Ex.Limit(this.startupControl.RequestedPageId, 1, pageCount); // 'Count' is 1 or above.
			}
			else // Create a page if no page exists yet:
			{
				this.settingsInEdit.CreateDefaultPage();
				this.selectedPageId = 1;
			}

			var requestedCommandId = Int32Ex.Limit(this.startupControl.RequestedCommandId, 1, Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage); // 'Max' is 1 or above.
			var requestedCommandIndex = (requestedCommandId - 1);
			var requestedSubpageIndex = (requestedCommandIndex / Model.Types.PredefinedCommandPage.CommandCapacityPerSubpage);
			this.selectedSubpageId = (requestedSubpageIndex + 1);

			// Initially set controls and validate its contents where needed:
			SetControls();

			var requestedControlIndex = (requestedCommandIndex % Model.Types.PredefinedCommandPage.CommandCapacityPerSubpage);
			var requestedControl = this.predefinedCommandSettingsSets[requestedControlIndex];
			requestedControl.PrepareUserInput(); // See remarks of this method!
			requestedControl.Select();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_Layout_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settingsInEdit.PageLayout = (Model.Types.PredefinedCommandPageLayoutEx)comboBox_Layout.SelectedItem;
			SetControls();
		}

		private void subpageCheckBox_I_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.selectedSubpageId = ((Controls.PredefinedCommandSubpageCheckBox)sender).Subpage;
			SetControls();
		}

		private void listBox_Pages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SelectedPageIndex = listBox_Pages.SelectedIndex;
			SetControls();
		}

		private void button_NamePage_Click(object sender, EventArgs e)
		{
			NamePage();
		}

		private void button_InsertPage_Click(object sender, EventArgs e)
		{
			InsertPage();
		}

		private void button_InsertPageFromClipboard_Click(object sender, EventArgs e)
		{
			// PENDING
		}

		private void button_InsertPagesFromFile_Click(object sender, EventArgs e)
		{
			int commandCapacityPerPageOld = CommandCapacityPerPage;
			int commandCapacityPerPageNew;
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsHelper.ImportFromFileAndInsert(this, this.settingsInEdit, commandCapacityPerPageOld, this.selectedPageId, out settingsInEditNew, out commandCapacityPerPageNew))
			{
				CommandCapacityPerPage = commandCapacityPerPageNew;
				this.settingsInEdit = settingsInEditNew;
				SetControls();
			}
		}

		private void button_AddPage_Click(object sender, EventArgs e)
		{
			AddPage();
		}

		private void button_AddPagesFromClipboard_Click(object sender, EventArgs e)
		{
			// PENDING
		}

		private void button_AddPagesFromFile_Click(object sender, EventArgs e)
		{
			int commandCapacityPerPageOld = CommandCapacityPerPage;
			int commandCapacityPerPageNew;
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsHelper.ImportFromFileAndAdd(this, this.settingsInEdit, commandCapacityPerPageOld, out settingsInEditNew, out commandCapacityPerPageNew))
			{
				CommandCapacityPerPage = commandCapacityPerPageNew;
				this.settingsInEdit = settingsInEditNew;
				SetControls();
			}
		}

		private void button_DuplicatePage_Click(object sender, EventArgs e)
		{
			DuplicatePage();
		}

		private void button_CopyPageToClipboard_Click(object sender, EventArgs e)
		{
			// PENDING
		}

		private void button_ExportPageToFile_Click(object sender, EventArgs e)
		{
			CommandPagesSettingsHelper.ExportSelectedPageToFile(this, this.settingsInEdit, this.selectedPageId, this.indicatedName);
		}

		private void button_DeletePage_Click(object sender, EventArgs e)
		{
			DeletePage();
		}

		private void button_CutPageToClipboard_Click(object sender, EventArgs e)
		{
			// PENDING
		}

		private void button_MovePageUp_Click(object sender, EventArgs e)
		{
			MovePageUp();
		}

		private void button_MovePageDown_Click(object sender, EventArgs e)
		{
			MovePageDown();
		}

		private void button_DeleteAllPages_Click(object sender, EventArgs e)
		{
			DeleteAllPages();
		}

		private void button_ExportAllPagesToFile_Click(object sender, EventArgs e)
		{
			CommandPagesSettingsHelper.ExportAllPagesToFile(this, this.settingsInEdit, this.indicatedName);
		}

		private void button_ImportAllPagesFromFile_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_ImportFromFile_Click()
			// Changes here may have to be applied there too.

			int commandCapacityPerPageOld = CommandCapacityPerPage;
			int commandCapacityPerPageNew;
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsHelper.ImportFromFile(this, this.settingsInEdit, commandCapacityPerPageOld, out settingsInEditNew, out commandCapacityPerPageNew))
			{
				CommandCapacityPerPage = commandCapacityPerPageNew;
				this.settingsInEdit = settingsInEditNew;
				SetControls();
			}
		}

		private void predefinedCommandSettingsSet_CommandChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetCommandFromSettingsSet(ControlEx.TagToInt32(sender));
			SetClearControls();
		}

		private void button_ClearPage_Click(object sender, EventArgs e)
		{
			ClearPage();
		}

		private void button_LinkToFile_Click(object sender, EventArgs e)
		{
			// PENDING ShowLinkFileDialog();
		}

		private void pathLabel_LinkedTo_Click(object sender, EventArgs e)
		{
			// PENDING ShowLinkFileDialog();
		}

		private void ShowLinkFileDialog()
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_LinkToFile_Click()
			// Changes here may have to be applied there too.

		}

		private void button_ClearLink_Click(object sender, EventArgs e)
		{
			// PENDING ClearLink();
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

		#region Controls Event Handlers > Commands Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Commands Context Menu
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int contextMenuStrip_Commands_SelectedCommandId; // = 0;

		private void contextMenuStrip_Commands_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.contextMenuStrip_Predefined_Opening()
			// Changes here may have to be applied there too.

			var id = GetSettingsSetIdFromLocation(new Point(contextMenuStrip_Commands.Left, contextMenuStrip_Commands.Top));
			var c = GetCommandFromId(id);

			contextMenuStrip_Commands_SelectedCommandId = id;

			toolStripMenuItem_CommandContextMenu_UpBy  .Enabled = ((id != 0) && (c != null) && (c.IsDefined));
			toolStripMenuItem_CommandContextMenu_DownBy.Enabled = ((id != 0) && (c != null) && (c.IsDefined));

			toolStripMenuItem_CommandContextMenu_MoveTo_1 .Enabled = (id != 1);
			toolStripMenuItem_CommandContextMenu_MoveTo_2 .Enabled = (id != 2);
			toolStripMenuItem_CommandContextMenu_MoveTo_3 .Enabled = (id != 3);
			toolStripMenuItem_CommandContextMenu_MoveTo_4 .Enabled = (id != 4);
			toolStripMenuItem_CommandContextMenu_MoveTo_5 .Enabled = (id != 5);
			toolStripMenuItem_CommandContextMenu_MoveTo_6 .Enabled = (id != 6);
			toolStripMenuItem_CommandContextMenu_MoveTo_7 .Enabled = (id != 7);
			toolStripMenuItem_CommandContextMenu_MoveTo_8 .Enabled = (id != 8);
			toolStripMenuItem_CommandContextMenu_MoveTo_9 .Enabled = (id != 9);
			toolStripMenuItem_CommandContextMenu_MoveTo_10.Enabled = (id != 10);
			toolStripMenuItem_CommandContextMenu_MoveTo_11.Enabled = (id != 11);
			toolStripMenuItem_CommandContextMenu_MoveTo_12.Enabled = (id != 12);

			toolStripMenuItem_CommandContextMenu_CopyTo_1 .Enabled = (id != 1);
			toolStripMenuItem_CommandContextMenu_CopyTo_2 .Enabled = (id != 2);
			toolStripMenuItem_CommandContextMenu_CopyTo_3 .Enabled = (id != 3);
			toolStripMenuItem_CommandContextMenu_CopyTo_4 .Enabled = (id != 4);
			toolStripMenuItem_CommandContextMenu_CopyTo_5 .Enabled = (id != 5);
			toolStripMenuItem_CommandContextMenu_CopyTo_6 .Enabled = (id != 6);
			toolStripMenuItem_CommandContextMenu_CopyTo_7 .Enabled = (id != 7);
			toolStripMenuItem_CommandContextMenu_CopyTo_8 .Enabled = (id != 8);
			toolStripMenuItem_CommandContextMenu_CopyTo_9 .Enabled = (id != 9);
			toolStripMenuItem_CommandContextMenu_CopyTo_10.Enabled = (id != 10);
			toolStripMenuItem_CommandContextMenu_CopyTo_11.Enabled = (id != 11);
			toolStripMenuItem_CommandContextMenu_CopyTo_12.Enabled = (id != 12);

			toolStripMenuItem_CommandContextMenu_MoveTo.Enabled = ((id != 0) && (c != null) && (c.IsDefined));
			toolStripMenuItem_CommandContextMenu_CopyTo.Enabled = ((id != 0) && (c != null) && (c.IsDefined));
			toolStripMenuItem_CommandContextMenu_Cut   .Enabled = ((id != 0) && (c != null) && (c.IsDefined));
			toolStripMenuItem_CommandContextMenu_Copy  .Enabled = ((id != 0) && (c != null) && (c.IsDefined));
			toolStripMenuItem_CommandContextMenu_Paste .Enabled = ((id != 0) /* && (PENDING ClipboardContainsCommand)*/);
			toolStripMenuItem_CommandContextMenu_Clear .Enabled = ((id != 0) && (c != null) && (c.IsDefined));
		}

		private void toolStripMenuItem_CommandContextMenu_UpBy_N_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click()
			// Changes here may have to be applied there too.

			var selectedCommandId = contextMenuStrip_Commands_SelectedCommandId;
			var n = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
			for (int i = 0; i < n; i++)
			{
				Up(selectedCommandId);

				selectedCommandId--;
				if (selectedCommandId < Model.Types.PredefinedCommandPage.FirstCommandIdPerPage)
					selectedCommandId = Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage;
			}
		}

		private void Up(int selectedCommandId)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.Up()
			// Changes here may have to be applied there too.

			var ss = GetSettingsSetFromId(selectedCommandId);
			var sc = ss.Command;
			if (sc != null)
				sc = new Model.Types.Command(sc); // Clone command to ensure decoupling.

			var targetCommandId = ((selectedCommandId > Model.Types.PredefinedCommandPage.FirstCommandIdPerPage) ? (selectedCommandId - 1) : (Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage));
			var ts = GetSettingsSetFromId(targetCommandId);
			var tc = ts.Command;
			if (tc != null)
				tc = new Model.Types.Command(tc); // Clone command to ensure decoupling.

			ts.Command = sc;
			ss.Command = tc;
		}

		private void toolStripMenuItem_CommandContextMenu_DownBy_N_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click()
			// Changes here may have to be applied there too.

			var selectedCommandId = contextMenuStrip_Commands_SelectedCommandId;
			var n = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
			for (int i = 0; i < n; i++)
			{
				Down(selectedCommandId);

				selectedCommandId++;
				if (selectedCommandId > Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage)
					selectedCommandId = Model.Types.PredefinedCommandPage.FirstCommandIdPerPage;
			}
		}

		private void Down(int selectedCommandId)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.Down()
			// Changes here may have to be applied there too.

			var ss = GetSettingsSetFromId(selectedCommandId);
			var sc = ss.Command;
			if (sc != null)
				sc = new Model.Types.Command(sc); // Clone command to ensure decoupling.

			var targetCommandId = ((selectedCommandId < Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage) ? (selectedCommandId + 1) : (Model.Types.PredefinedCommandPage.FirstCommandIdPerPage));
			var ts = GetSettingsSetFromId(targetCommandId);
			var tc = ts.Command;
			if (tc != null)
				tc = new Model.Types.Command(tc); // Clone command to ensure decoupling.

			ts.Command = sc;
			ss.Command = tc;
		}

		private void toolStripMenuItem_CommandContextMenu_MoveTo_I_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click()
			// Changes here may have to be applied there too.

			var ss = GetSettingsSetFromId(contextMenuStrip_Commands_SelectedCommandId);
			var sc = ss.Command;
			if (sc != null)
				sc = new Model.Types.Command(sc); // Clone command to ensure decoupling.

			var targetCommandId = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
			var ts = GetSettingsSetFromId(targetCommandId);
			ts.Command = sc;
			ss.ClearCommand();
		}

		private void toolStripMenuItem_CommandContextMenu_CopyTo_I_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click()
			// Changes here may have to be applied there too.

			var ss = GetSettingsSetFromId(contextMenuStrip_Commands_SelectedCommandId);
			var sc = ss.Command;
			if (sc != null)
				sc = new Model.Types.Command(sc); // Clone command to ensure decoupling.

			var targetCommandId = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
			var ts = GetSettingsSetFromId(targetCommandId);
			ts.Command = sc;
		}

		private void toolStripMenuItem_CommandContextMenu_Cut_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_Cut_Click()
			// Changes here may have to be applied there too.

			// PENDING CutToClipboard();
		}

		private void toolStripMenuItem_CommandContextMenu_Copy_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_Copy_Click()
			// Changes here may have to be applied there too.

			// PENDING CopyToClipboard();
		}

		private void toolStripMenuItem_CommandContextMenu_Paste_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_Paste_Click()
			// Changes here may have to be applied there too.

			// PENDING PasteFromClipboard();
		}

		private void toolStripMenuItem_CommandContextMenu_Clear_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_Clear_Click()
			// Changes here may have to be applied there too.

			var selectedSet = GetSettingsSetFromId(contextMenuStrip_Commands_SelectedCommandId);
			selectedSet.ClearCommand();
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <remarks><see cref="SetControls"/> is not called by this private property.</remarks>
		private int CommandCapacityPerPage
		{
			get { return (((Model.Types.PredefinedCommandPageLayoutEx)this.settingsInEdit.PageLayout).CommandCapacityPerPage); }
			set
			{
				if (CommandCapacityPerPage != value)
					this.settingsInEdit.PageLayout = Model.Types.PredefinedCommandPageLayoutEx.GetMatchingItem(value);
			}
		}

		/// <remarks><see cref="SetControls"/> is not called by this private property.</remarks>
		private int SelectedPageIndex
		{
			get { return (this.selectedPageId - 1);  }
			set { this.selectedPageId = (value + 1); }
		}

		#region Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------

		private void InitializeControls(bool useExplicitDefaultRadix)
		{
			comboBox_Layout.Items.AddRange(Model.Types.PredefinedCommandPageLayoutEx.GetItems());

			var deltaX = ((subpageCheckBox_1B.Left - subpageCheckBox_1A.Left) + (subpageCheckBox_1B.Left - subpageCheckBox_1A.Right));
			var deltaY = ((subpageCheckBox_2A.Top  - subpageCheckBox_1A.Top)  + (subpageCheckBox_2A.Top  - subpageCheckBox_1A.Bottom));

			this.subpageCheckBoxLocationTopLeft     = subpageCheckBox_1A.Location;
			this.subpageCheckBoxLocationLeftAbove   = new Point((subpageCheckBox_1A.Left + deltaX), (subpageCheckBox_2A.Top - deltaY));
			this.subpageCheckBoxLocationCenter      = subpageCheckBox_2B.Location;
			this.subpageCheckBoxLocationRightBelow  = new Point((subpageCheckBox_2B.Left + deltaX), (subpageCheckBox_3B.Top - deltaY));
			this.subpageCheckBoxLocationBottomRight = subpageCheckBox_3C.Location;

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

			this.predefinedCommandSettingsSetLabels = new List<Label>(Model.Types.PredefinedCommandPage.CommandCapacityPerSubpage); // Preset the required capacity to improve memory management.
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

			this.predefinedCommandSettingsSets = new List<Controls.PredefinedCommandSettingsSet>(Model.Types.PredefinedCommandPage.CommandCapacityPerSubpage); // Preset the required capacity to improve memory management.
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
			SetLayoutControls();
			SetPagesControls();
			SetPageControls();
			SetClearControls();
			SetLinkControls();
		}

		private void SetLayoutControls()
		{
			SuspendLayout();
			this.isSettingControls.Enter();
			try
			{
				Model.Types.PredefinedCommandPageLayoutEx pageLayoutEx = this.settingsInEdit.PageLayout;
				comboBox_Layout.SelectedItem = pageLayoutEx;

			////subpageCheckBox_1A.Visible = true;
				subpageCheckBox_2A.Visible = (pageLayoutEx.RowsPerPage >= 2);
				subpageCheckBox_3A.Visible = (pageLayoutEx.RowsPerPage >= 3);
				subpageCheckBox_1B.Visible =                                    (pageLayoutEx.ColumnsPerPage >= 2);
				subpageCheckBox_2B.Visible = (pageLayoutEx.RowsPerPage >= 2) && (pageLayoutEx.ColumnsPerPage >= 2);
				subpageCheckBox_3B.Visible = (pageLayoutEx.RowsPerPage >= 3) && (pageLayoutEx.ColumnsPerPage >= 2);
				subpageCheckBox_1C.Visible =                                    (pageLayoutEx.ColumnsPerPage >= 3);
				subpageCheckBox_2C.Visible = (pageLayoutEx.RowsPerPage >= 2) && (pageLayoutEx.ColumnsPerPage >= 3);
				subpageCheckBox_3C.Visible = (pageLayoutEx.RowsPerPage >= 3) && (pageLayoutEx.ColumnsPerPage >= 3);

				//subpageCheckBox_1A.Location = PENDING
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout();
			}
		}

		private void SetPagesControls()
		{
			SuspendLayout();
			this.isSettingControls.Enter();
			try
			{
				int pageCount = this.settingsInEdit.Pages.Count;
				bool pageIsSelected = (this.selectedPageId != 0);
				int totalDefinedCommandCount = this.settingsInEdit.TotalDefinedCommandCount;

				// Page list:
				if (pageCount > 0)
				{
					listBox_Pages.Enabled = true;
					listBox_Pages.Items.Clear();

					foreach (var p in this.settingsInEdit.Pages)
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
				button_NamePage              .Enabled = pageIsSelected;
				button_InsertPage            .Enabled = pageIsSelected;
				button_InsertPagesFromFile   .Enabled = pageIsSelected;
			////button_AddPage               .Enabled = true;
			////button_AddPagesFromFile      .Enabled = true;
				button_DuplicatePage              .Enabled = pageIsSelected;
				button_ExportPageToFile      .Enabled = pageIsSelected;
				button_DeletePage            .Enabled = pageIsSelected; // Deleting a sole page is permissible.
				button_MovePageUp            .Enabled = pageIsSelected && (this.selectedPageId > 1);
				button_MovePageDown          .Enabled = pageIsSelected && (this.selectedPageId < pageCount);
				button_DeleteAllPages        .Enabled = (pageCount > 0); // Deleting a sole page is permissible.
				button_ExportAllPagesToFile  .Enabled = (pageCount > 0) && (totalDefinedCommandCount > 0);
			////button_ImportAllPagesFromFile.Enabled = true;

				// Selected page:
				if (pageIsSelected)
					groupBox_Page.Text = this.settingsInEdit.Pages[SelectedPageIndex].PageName;
				else
					groupBox_Page.Text = "<No Page Selected>";
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout();
			}
		}

		private void SetPageControls()
		{
			SuspendLayout();
			this.isSettingControls.Enter();
			try
			{
				if (this.selectedPageId != 0)
				{
					groupBox_Page.Enabled = true;

					int pageCount = this.settingsInEdit.Pages.Count;
					int commandCount = 0;
					if (pageCount >= this.selectedPageId)
						commandCount = this.settingsInEdit.Pages[SelectedPageIndex].Commands.Count;

					for (int i = 0; i < commandCount; i++)
						this.predefinedCommandSettingsSets[i].Command = this.settingsInEdit.Pages[SelectedPageIndex].Commands[i];

					for (int i = commandCount; i < Model.Types.PredefinedCommandPage.CommandCapacityPerSubpage; i++)
						this.predefinedCommandSettingsSets[i].Command = null;
				}
				else
				{
					groupBox_Page.Enabled = true;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout();
			}
		}

		private void SetClearControls()
		{
			SuspendLayout();
			this.isSettingControls.Enter();
			try
			{
				int pageCount = this.settingsInEdit.Pages.Count;
				int commandCount = 0;
				if (pageCount >= this.selectedPageId)
					commandCount = this.settingsInEdit.Pages[SelectedPageIndex].Commands.Count;

				button_ClearPage.Enabled = (commandCount > 0);
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout();
			}
		}

		private void SetLinkControls()
		{
			SuspendLayout();
			this.isSettingControls.Enter();
			try
			{
				// PENDING

				pathLabel_LinkedTo.Enabled = false;
				pathLabel_LinkedTo.Visible = false;
				button_ClearLink.Enabled = false;
				button_ClearLink.Visible = false;
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout();
			}
		}

		#endregion

		#region Non-Public Methods > Pages
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Pages
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always)]
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

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void InsertPage()
		{
			int pageNumber = this.selectedPageId;
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
				var pcp = new Model.Types.PredefinedCommandPage(Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage, pageName);
				this.settingsInEdit.Pages.Insert(SelectedPageIndex, pcp);
				SetControls();
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
				var pcp = new Model.Types.PredefinedCommandPage(Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage, pageName);
				this.settingsInEdit.Pages.Add(pcp);
				this.selectedPageId = this.settingsInEdit.Pages.Count;
				SetControls();
			}
		}

		/// <remarks>
		/// Code uses term "duplicate" whereas view uses "copy".
		/// </remarks>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void DuplicatePage()
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
				var pcp = new Model.Types.PredefinedCommandPage(this.settingsInEdit.Pages[SelectedPageIndex]);
				pcp.PageName = pageName;
				this.settingsInEdit.Pages.Insert(SelectedPageIndex + 1, pcp);
				SetControls();
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
				if (this.settingsInEdit.Pages.Count > 1)
				{
					this.settingsInEdit.Pages.RemoveAt(SelectedPageIndex);
					this.selectedPageId = Int32Ex.Limit(this.selectedPageId, 1, Math.Max(this.settingsInEdit.Pages.Count, 1)); // 'max' must be 1 or above.
					SetControls();
				}
				else // Same behavior as DeletePages() further below.
				{
					this.settingsInEdit.CreateDefaultPage();
					this.selectedPageId = 1;
					SetControls();
				}
			}
		}

		private void MovePageUp()
		{
			var pcp = this.settingsInEdit.Pages[SelectedPageIndex];
			this.settingsInEdit.Pages.RemoveAt(SelectedPageIndex);
			this.selectedPageId--;
			this.settingsInEdit.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		private void MovePageDown()
		{
			var pcp = this.settingsInEdit.Pages[SelectedPageIndex];
			this.settingsInEdit.Pages.RemoveAt(SelectedPageIndex);
			this.selectedPageId++;
			this.settingsInEdit.Pages.Insert(SelectedPageIndex, pcp);
			SetPagesControls();
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void DeleteAllPages()
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
				this.selectedPageId = 1;
				SetControls();
			}
		}

		#endregion

		#region Non-Public Methods > Selected Page
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Selected Page
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns command at the specified <paramref name="id"/>.
		/// Returns <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual Model.Types.Command GetCommandFromId(int id)
		{
			var ss = GetSettingsSetFromId(id);
			if (ss != null)
				return (ss.Command);

			return (null);
		}

		/// <summary>
		/// Returns settings set at the specified <paramref name="id"/>.
		/// Returns <c>null</c> if command is undefined or invalid.
		/// </summary>
		protected virtual Controls.PredefinedCommandSettingsSet GetSettingsSetFromId(int id)
		{
			int i = (id - 1); // ID = 1..max
			if ((i >= 0) && (i < this.predefinedCommandSettingsSets.Count))
				return (this.predefinedCommandSettingsSets[i]);

			return (null);
		}

		/// <summary>
		/// Returns settings set ID (1..max) that is assigned to the set at the specified location.
		/// Returns 0 if no set.
		/// </summary>
		protected virtual int GetSettingsSetIdFromLocation(Point point)
		{
			Point requested = groupBox_Page.PointToClient(point);

			// Ensure that location is within control:
			if ((requested.X < 0) || (requested.X > Width))  return (0);
			if ((requested.Y < 0) || (requested.Y > Height)) return (0);

			// Ensure that location is around sets:
			if (requested.Y < predefinedCommandSettingsSet_1.Top)     return (0);
			if (requested.Y > predefinedCommandSettingsSet_12.Bottom) return (0);

			// Find the corresponding set:
			for (int i = 0; i < this.predefinedCommandSettingsSets.Count; i++)
			{
				if (requested.Y <= this.predefinedCommandSettingsSets[i].Bottom)
					return (i + 1); // ID = 1..max
			}

			return (0);
		}

		/// <summary>
		/// Returns settings set that is assigned to the set at the specified location.
		/// Returns <c>null</c> if no set or if set is undefined or invalid.
		/// </summary>
		protected virtual Controls.PredefinedCommandSettingsSet GetSettingsSetFromLocation(Point point)
		{
			return (GetSettingsSetFromId(GetSettingsSetIdFromLocation(point)));
		}

		/// <param name="id">Command 1..<see cref="Model.Types.PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		protected virtual void SetCommandFromSettingsSet(int id)
		{
			if (this.settingsInEdit.Pages != null)
			{
				var i = (id - 1);
				var p = this.settingsInEdit.Pages[SelectedPageIndex];
				p.SetCommand(i, this.predefinedCommandSettingsSets[i].Command);
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
