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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Model.Types;
using YAT.View.Utilities;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.PredefinedCommandSettings.resources", MessageId = "yat",  Justification = "This is a YAT specific file extension.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.PredefinedCommandSettings.resources", MessageId = "yacp", Justification = "This is a YAT specific file extension.")]

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

			/// <param name="requestedPageId">Page 1..<see cref="PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
			/// <param name="requestedCommandId">Command 1..<see cref="PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
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

		private StartupControl startupControl = new StartupControl(1, 1);
		private SettingControlsHelper isSettingControls; // = false;

		private Model.Settings.PredefinedCommandSettings settings;       // = null;
		private Model.Settings.PredefinedCommandSettings settingsInEdit; // = null;
		private int selectedPageId = 1;
		private int selectedSubpageId = 1;
		private string indicatedTerminalName;                                    // = null;

		private Point subpageCheckBoxLocationTopLeft;
		private Point subpageCheckBoxLocationLeftAbove;
		private Point subpageCheckBoxLocationCenter;
		private Point subpageCheckBoxLocationRightBelow;
	////private Point subpageCheckBoxLocationBottomRight; is not needed.
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
		/// <param name="requestedPageId">Page 1..<see cref="PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		/// <param name="requestedCommandId">Command 1..<see cref="PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		/// <param name="indicatedTerminalName">The indicated terminal name.</param>
		public PredefinedCommandSettings(Model.Settings.PredefinedCommandSettings settings, Domain.TerminalType terminalType, bool useExplicitDefaultRadix, Domain.Parser.Mode parseModeForText, string rootDirectoryForFile, int requestedPageId, int requestedCommandId, string indicatedTerminalName)
		{
			InitializeComponent();

			this.settings = settings;
			this.settingsInEdit = new Model.Settings.PredefinedCommandSettings(settings); // Clone to ensure decoupling.

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
			this.indicatedTerminalName = indicatedTerminalName;

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
			else // Create a page if no page exists yet, same behavior as DeleteAllPages():
			{
				this.settingsInEdit.ClearAndCreateDefaultPage();
				this.selectedPageId = 1;
			}

			ActivateSubpage(this.startupControl.RequestedCommandId);

			// Initially set controls and validate its contents where needed:
			SetControls();

			SelectSet(this.startupControl.RequestedCommandId);
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		#region Controls Event Handlers > Non-Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Non-Menu
		//------------------------------------------------------------------------------------------

		private void comboBox_Layout_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsChangeHelper.TryChange(this, this.settingsInEdit, (PredefinedCommandPageLayoutEx)comboBox_Layout.SelectedItem, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void subpageCheckBox_I_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.selectedSubpageId = ((Controls.PredefinedCommandSubpageCheckBox)sender).SubpageId;
			SetPageControls();
		}

		private void listBox_Pages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.selectedPageId = (listBox_Pages.SelectedIndex + 1);
			SetPageControls();
		}

		private void button_NamePage_Click(object sender, EventArgs e)
		{
			NamePage();
		}

		private void button_RenumberPages_Click(object sender, EventArgs e)
		{
			RenumberPages();
		}

		private void button_InsertPage_Click(object sender, EventArgs e)
		{
			InsertPage();
		}

		private void button_InsertPageFromClipboard_Click(object sender, EventArgs e)
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsClipboardHelper.TryGetAndInsert(this, this.settingsInEdit, this.selectedPageId, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void button_InsertPagesFromFile_Click(object sender, EventArgs e)
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsFileHelper.TryLoadAndInsert(this, this.settingsInEdit, this.selectedPageId, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void button_AddPage_Click(object sender, EventArgs e)
		{
			AddPage();
		}

		private void button_AddPagesFromClipboard_Click(object sender, EventArgs e)
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsClipboardHelper.TryGetAndAdd(this, this.settingsInEdit, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void button_AddPagesFromFile_Click(object sender, EventArgs e)
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsFileHelper.TryLoadAndAdd(this, this.settingsInEdit, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void button_DuplicatePage_Click(object sender, EventArgs e)
		{
			DuplicatePage();
		}

		private void button_CopyPageToClipboard_Click(object sender, EventArgs e)
		{
			Clipboard.Clear(); // Prevent handling errors in case copying takes long.
			CommandPagesSettingsClipboardHelper.TrySetOne(this, this.settingsInEdit, this.selectedPageId);
		}

		private void button_ExportPageToFile_Click(object sender, EventArgs e)
		{
			CommandPagesSettingsFileHelper.TryExportOne(this, this.settingsInEdit, this.selectedPageId);
		}

		private void button_DeletePage_Click(object sender, EventArgs e)
		{
			DeletePage();
		}

		private void button_CutPageToClipboard_Click(object sender, EventArgs e)
		{
			Clipboard.Clear(); // Prevent handling errors in case copying takes long.
			if (CommandPagesSettingsClipboardHelper.TrySetOne(this, this.settingsInEdit, this.selectedPageId))
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

		private void button_DeleteAllPages_Click(object sender, EventArgs e)
		{
			DeleteAllPages();
		}

		private void button_ExportAllPagesToFile_Click(object sender, EventArgs e)
		{
			CommandPagesSettingsFileHelper.TryExportAll(this, this.settingsInEdit, this.indicatedTerminalName);
		}

		private void button_ImportAllPagesFromFile_Click(object sender, EventArgs e)
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsFileHelper.TryLoadAndImportAll(this, this.settingsInEdit, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void button_ExportAllPagesToClipboard_Click(object sender, EventArgs e)
		{
			CommandPagesSettingsClipboardHelper.TrySetAll(this, this.settingsInEdit);
		}

		private void button_ImportAllPagesFromClipboard_Click(object sender, EventArgs e)
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsClipboardHelper.TryGetAndImportAll(this, this.settingsInEdit, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void predefinedCommandSettingsSet_CommandChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			UpdateCommandFromSettingsSetId(ControlEx.TagToInt32(sender));
		////SetPageControls() is not invoked for reasons described in remarks of method below.
			SetClearControls(); // See remarks of method.
		////SetPagesControls() is not invoked for reasons described in remarks of method below.
			SetPagesButtonsControls(); // Needed since some 'Copy/Export...' options depend on command availability.
		}

		private void button_ClearPage_Click(object sender, EventArgs e)
		{
			ClearPage();
		}

		private void button_LinkToFile_Click(object sender, EventArgs e)
		{
			ShowLinkFileDialog();
		}

		private void pathLabel_LinkedTo_Click(object sender, EventArgs e)
		{
			ShowLinkFileDialog();
		}

		private void ShowLinkFileDialog()
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsFileLinkHelper.TryLoadAndLink(this, this.settingsInEdit, this.selectedPageId, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
		}

		private void button_ClearLink_Click(object sender, EventArgs e)
		{
			Model.Settings.PredefinedCommandSettings settingsInEditNew;
			if (CommandPagesSettingsFileLinkHelper.TryClearLink(this, this.settingsInEdit, this.selectedPageId, out settingsInEditNew))
				ApplySettingsAndSetControls(settingsInEditNew);
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

		#region Controls Event Handlers > Commands Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Commands Context Menu
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int contextMenuStrip_Commands_SelectedCommandId; // = 0;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'c' = Command.")]
		private void contextMenuStrip_Commands_Opening(object sender, CancelEventArgs e)
		{
			this.isSettingControls.Enter();
			try
			{
				// Attention:
				// Similar code exists in...
				// ...View.Forms.Terminal.contextMenuStrip_Predefined_Opening()
				// Changes here may have to be applied there too.

				int id = 0;
				Command c = null;
				bool cIsDefined = false;

				if (TryGetCommandIdFromLocation(Cursor.Position, out id))
				{
					if (TryGetCommandFromId(id, out c))
						cIsDefined = c.IsDefined;
				}

				PredefinedCommandPageLayoutEx pageLayoutEx = this.settingsInEdit.PageLayout;
				var np = pageLayoutEx.CommandCapacityPerPage;

				contextMenuStrip_Commands_SelectedCommandId = id;

				toolStripMenuItem_CommandContextMenu_MoveTo.Enabled = ((id != 0) && (c != null) && (c.IsDefined));
				toolStripMenuItem_CommandContextMenu_CopyTo.Enabled = ((id != 0) && (c != null) && (c.IsDefined));

				toolStripMenuItem_CommandContextMenu_CopyTo_1 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_1 .Enabled =           (cIsDefined && (id != 1));
				toolStripMenuItem_CommandContextMenu_CopyTo_2 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_2 .Enabled =           (cIsDefined && (id != 2));
				toolStripMenuItem_CommandContextMenu_CopyTo_3 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_3 .Enabled =           (cIsDefined && (id != 3));
				toolStripMenuItem_CommandContextMenu_CopyTo_4 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_4 .Enabled =           (cIsDefined && (id != 4));
				toolStripMenuItem_CommandContextMenu_CopyTo_5 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_5 .Enabled =           (cIsDefined && (id != 5));
				toolStripMenuItem_CommandContextMenu_CopyTo_6 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_6 .Enabled =           (cIsDefined && (id != 6));
				toolStripMenuItem_CommandContextMenu_CopyTo_7 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_7 .Enabled =           (cIsDefined && (id != 7));
				toolStripMenuItem_CommandContextMenu_CopyTo_8 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_8 .Enabled =           (cIsDefined && (id != 8));
				toolStripMenuItem_CommandContextMenu_CopyTo_9 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_9 .Enabled =           (cIsDefined && (id != 9));
				toolStripMenuItem_CommandContextMenu_CopyTo_10.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_10.Enabled =           (cIsDefined && (id != 10));
				toolStripMenuItem_CommandContextMenu_CopyTo_11.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_11.Enabled =           (cIsDefined && (id != 11));
				toolStripMenuItem_CommandContextMenu_CopyTo_12.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_CopyTo_12.Enabled =           (cIsDefined && (id != 12));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_12.Visible = (cIsDefined && (np >= 13));
				toolStripMenuItem_CommandContextMenu_CopyTo_13.Visible =           (cIsDefined && (np >= 13));
				toolStripMenuItem_CommandContextMenu_CopyTo_13.Enabled =           (cIsDefined && (id != 13));
				toolStripMenuItem_CommandContextMenu_CopyTo_14.Visible =           (cIsDefined && (np >= 14));
				toolStripMenuItem_CommandContextMenu_CopyTo_14.Enabled =           (cIsDefined && (id != 14));
				toolStripMenuItem_CommandContextMenu_CopyTo_15.Visible =           (cIsDefined && (np >= 15));
				toolStripMenuItem_CommandContextMenu_CopyTo_15.Enabled =           (cIsDefined && (id != 15));
				toolStripMenuItem_CommandContextMenu_CopyTo_16.Visible =           (cIsDefined && (np >= 16));
				toolStripMenuItem_CommandContextMenu_CopyTo_16.Enabled =           (cIsDefined && (id != 16));
				toolStripMenuItem_CommandContextMenu_CopyTo_17.Visible =           (cIsDefined && (np >= 17));
				toolStripMenuItem_CommandContextMenu_CopyTo_17.Enabled =           (cIsDefined && (id != 17));
				toolStripMenuItem_CommandContextMenu_CopyTo_18.Visible =           (cIsDefined && (np >= 18));
				toolStripMenuItem_CommandContextMenu_CopyTo_18.Enabled =           (cIsDefined && (id != 18));
				toolStripMenuItem_CommandContextMenu_CopyTo_19.Visible =           (cIsDefined && (np >= 19));
				toolStripMenuItem_CommandContextMenu_CopyTo_19.Enabled =           (cIsDefined && (id != 19));
				toolStripMenuItem_CommandContextMenu_CopyTo_20.Visible =           (cIsDefined && (np >= 20));
				toolStripMenuItem_CommandContextMenu_CopyTo_20.Enabled =           (cIsDefined && (id != 20));
				toolStripMenuItem_CommandContextMenu_CopyTo_21.Visible =           (cIsDefined && (np >= 21));
				toolStripMenuItem_CommandContextMenu_CopyTo_21.Enabled =           (cIsDefined && (id != 21));
				toolStripMenuItem_CommandContextMenu_CopyTo_22.Visible =           (cIsDefined && (np >= 22));
				toolStripMenuItem_CommandContextMenu_CopyTo_22.Enabled =           (cIsDefined && (id != 22));
				toolStripMenuItem_CommandContextMenu_CopyTo_23.Visible =           (cIsDefined && (np >= 23));
				toolStripMenuItem_CommandContextMenu_CopyTo_23.Enabled =           (cIsDefined && (id != 23));
				toolStripMenuItem_CommandContextMenu_CopyTo_24.Visible =           (cIsDefined && (np >= 24));
				toolStripMenuItem_CommandContextMenu_CopyTo_24.Enabled =           (cIsDefined && (id != 24));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_24.Visible = (cIsDefined && (np >= 25));
				toolStripMenuItem_CommandContextMenu_CopyTo_25.Visible =           (cIsDefined && (np >= 25));
				toolStripMenuItem_CommandContextMenu_CopyTo_25.Enabled =           (cIsDefined && (id != 25));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_36.Visible = (cIsDefined && (np >= 37));
				toolStripMenuItem_CommandContextMenu_CopyTo_37.Visible =           (cIsDefined && (np >= 37));
				toolStripMenuItem_CommandContextMenu_CopyTo_37.Enabled =           (cIsDefined && (id != 37));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_48.Visible = (cIsDefined && (np >= 49));
				toolStripMenuItem_CommandContextMenu_CopyTo_49.Visible =           (cIsDefined && (np >= 49));
				toolStripMenuItem_CommandContextMenu_CopyTo_49.Enabled =           (cIsDefined && (id != 49));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_60.Visible = (cIsDefined && (np >= 61));
				toolStripMenuItem_CommandContextMenu_CopyTo_61.Visible =           (cIsDefined && (np >= 61));
				toolStripMenuItem_CommandContextMenu_CopyTo_61.Enabled =           (cIsDefined && (id != 61));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_72.Visible = (cIsDefined && (np >= 73));
				toolStripMenuItem_CommandContextMenu_CopyTo_73.Visible =           (cIsDefined && (np >= 73));
				toolStripMenuItem_CommandContextMenu_CopyTo_73.Enabled =           (cIsDefined && (id != 73));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_84.Visible = (cIsDefined && (np >= 85));
				toolStripMenuItem_CommandContextMenu_CopyTo_85.Visible =           (cIsDefined && (np >= 85));
				toolStripMenuItem_CommandContextMenu_CopyTo_85.Enabled =           (cIsDefined && (id != 85));
				toolStripMenuItem_CommandContextMenu_CopyTo_Separator_96.Visible = (cIsDefined && (np >= 97));
				toolStripMenuItem_CommandContextMenu_CopyTo_97.Visible =           (cIsDefined && (np >= 97));
				toolStripMenuItem_CommandContextMenu_CopyTo_97.Enabled =           (cIsDefined && (id != 97));

				toolStripMenuItem_CommandContextMenu_MoveTo_1 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_1 .Enabled =           (cIsDefined && (id != 1));
				toolStripMenuItem_CommandContextMenu_MoveTo_2 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_2 .Enabled =           (cIsDefined && (id != 2));
				toolStripMenuItem_CommandContextMenu_MoveTo_3 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_3 .Enabled =           (cIsDefined && (id != 3));
				toolStripMenuItem_CommandContextMenu_MoveTo_4 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_4 .Enabled =           (cIsDefined && (id != 4));
				toolStripMenuItem_CommandContextMenu_MoveTo_5 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_5 .Enabled =           (cIsDefined && (id != 5));
				toolStripMenuItem_CommandContextMenu_MoveTo_6 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_6 .Enabled =           (cIsDefined && (id != 6));
				toolStripMenuItem_CommandContextMenu_MoveTo_7 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_7 .Enabled =           (cIsDefined && (id != 7));
				toolStripMenuItem_CommandContextMenu_MoveTo_8 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_8 .Enabled =           (cIsDefined && (id != 8));
				toolStripMenuItem_CommandContextMenu_MoveTo_9 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_9 .Enabled =           (cIsDefined && (id != 9));
				toolStripMenuItem_CommandContextMenu_MoveTo_10.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_10.Enabled =           (cIsDefined && (id != 10));
				toolStripMenuItem_CommandContextMenu_MoveTo_11.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_11.Enabled =           (cIsDefined && (id != 11));
				toolStripMenuItem_CommandContextMenu_MoveTo_12.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_MoveTo_12.Enabled =           (cIsDefined && (id != 12));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_12.Visible = (cIsDefined && (np >= 13));
				toolStripMenuItem_CommandContextMenu_MoveTo_13.Visible =           (cIsDefined && (np >= 13));
				toolStripMenuItem_CommandContextMenu_MoveTo_13.Enabled =           (cIsDefined && (id != 13));
				toolStripMenuItem_CommandContextMenu_MoveTo_14.Visible =           (cIsDefined && (np >= 14));
				toolStripMenuItem_CommandContextMenu_MoveTo_14.Enabled =           (cIsDefined && (id != 14));
				toolStripMenuItem_CommandContextMenu_MoveTo_15.Visible =           (cIsDefined && (np >= 15));
				toolStripMenuItem_CommandContextMenu_MoveTo_15.Enabled =           (cIsDefined && (id != 15));
				toolStripMenuItem_CommandContextMenu_MoveTo_16.Visible =           (cIsDefined && (np >= 16));
				toolStripMenuItem_CommandContextMenu_MoveTo_16.Enabled =           (cIsDefined && (id != 16));
				toolStripMenuItem_CommandContextMenu_MoveTo_17.Visible =           (cIsDefined && (np >= 17));
				toolStripMenuItem_CommandContextMenu_MoveTo_17.Enabled =           (cIsDefined && (id != 17));
				toolStripMenuItem_CommandContextMenu_MoveTo_18.Visible =           (cIsDefined && (np >= 18));
				toolStripMenuItem_CommandContextMenu_MoveTo_18.Enabled =           (cIsDefined && (id != 18));
				toolStripMenuItem_CommandContextMenu_MoveTo_19.Visible =           (cIsDefined && (np >= 19));
				toolStripMenuItem_CommandContextMenu_MoveTo_19.Enabled =           (cIsDefined && (id != 19));
				toolStripMenuItem_CommandContextMenu_MoveTo_20.Visible =           (cIsDefined && (np >= 20));
				toolStripMenuItem_CommandContextMenu_MoveTo_20.Enabled =           (cIsDefined && (id != 20));
				toolStripMenuItem_CommandContextMenu_MoveTo_21.Visible =           (cIsDefined && (np >= 21));
				toolStripMenuItem_CommandContextMenu_MoveTo_21.Enabled =           (cIsDefined && (id != 21));
				toolStripMenuItem_CommandContextMenu_MoveTo_22.Visible =           (cIsDefined && (np >= 22));
				toolStripMenuItem_CommandContextMenu_MoveTo_22.Enabled =           (cIsDefined && (id != 22));
				toolStripMenuItem_CommandContextMenu_MoveTo_23.Visible =           (cIsDefined && (np >= 23));
				toolStripMenuItem_CommandContextMenu_MoveTo_23.Enabled =           (cIsDefined && (id != 23));
				toolStripMenuItem_CommandContextMenu_MoveTo_24.Visible =           (cIsDefined && (np >= 24));
				toolStripMenuItem_CommandContextMenu_MoveTo_24.Enabled =           (cIsDefined && (id != 24));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_24.Visible = (cIsDefined && (np >= 25));
				toolStripMenuItem_CommandContextMenu_MoveTo_25.Visible =           (cIsDefined && (np >= 25));
				toolStripMenuItem_CommandContextMenu_MoveTo_25.Enabled =           (cIsDefined && (id != 25));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_36.Visible = (cIsDefined && (np >= 37));
				toolStripMenuItem_CommandContextMenu_MoveTo_37.Visible =           (cIsDefined && (np >= 37));
				toolStripMenuItem_CommandContextMenu_MoveTo_37.Enabled =           (cIsDefined && (id != 37));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_48.Visible = (cIsDefined && (np >= 49));
				toolStripMenuItem_CommandContextMenu_MoveTo_49.Visible =           (cIsDefined && (np >= 49));
				toolStripMenuItem_CommandContextMenu_MoveTo_49.Enabled =           (cIsDefined && (id != 49));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_60.Visible = (cIsDefined && (np >= 61));
				toolStripMenuItem_CommandContextMenu_MoveTo_61.Visible =           (cIsDefined && (np >= 61));
				toolStripMenuItem_CommandContextMenu_MoveTo_61.Enabled =           (cIsDefined && (id != 61));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_72.Visible = (cIsDefined && (np >= 73));
				toolStripMenuItem_CommandContextMenu_MoveTo_73.Visible =           (cIsDefined && (np >= 73));
				toolStripMenuItem_CommandContextMenu_MoveTo_73.Enabled =           (cIsDefined && (id != 73));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_84.Visible = (cIsDefined && (np >= 85));
				toolStripMenuItem_CommandContextMenu_MoveTo_85.Visible =           (cIsDefined && (np >= 85));
				toolStripMenuItem_CommandContextMenu_MoveTo_85.Enabled =           (cIsDefined && (id != 85));
				toolStripMenuItem_CommandContextMenu_MoveTo_Separator_96.Visible = (cIsDefined && (np >= 96));
				toolStripMenuItem_CommandContextMenu_MoveTo_97.Visible =           (cIsDefined && (np >= 96));
				toolStripMenuItem_CommandContextMenu_MoveTo_97.Enabled =           (cIsDefined && (id != 96));

				toolStripMenuItem_CommandContextMenu_UpBy  .Enabled = ((id != 0) && (c != null) && (c.IsDefined));
				toolStripMenuItem_CommandContextMenu_DownBy.Enabled = ((id != 0) && (c != null) && (c.IsDefined));
				toolStripMenuItem_CommandContextMenu_UpBy_1 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_2 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_3 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_4 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_5 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_6 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_7 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_8 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_9 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_10.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_11.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_12.Visible = (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_12.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_13.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_14.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_15.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_16.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_17.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_18.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_19.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_20.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_21.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_22.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_23.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_24.Visible = (cIsDefined && (np > 24));
				toolStripMenuItem_CommandContextMenu_UpBy_24.Visible =           (cIsDefined && (np > 24));
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_36.Visible = (cIsDefined && (np > 36));
				toolStripMenuItem_CommandContextMenu_UpBy_36.Visible =           (cIsDefined && (np > 36));
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_48.Visible = (cIsDefined && (np > 48));
				toolStripMenuItem_CommandContextMenu_UpBy_48.Visible =           (cIsDefined && (np > 48));
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_60.Visible = (cIsDefined && (np > 60));
				toolStripMenuItem_CommandContextMenu_UpBy_60.Visible =           (cIsDefined && (np > 60));
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_72.Visible = (cIsDefined && (np > 72));
				toolStripMenuItem_CommandContextMenu_UpBy_72.Visible =           (cIsDefined && (np > 72));
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_84.Visible = (cIsDefined && (np > 84));
				toolStripMenuItem_CommandContextMenu_UpBy_84.Visible =           (cIsDefined && (np > 84));
				toolStripMenuItem_CommandContextMenu_UpBy_Separator_96.Visible = (cIsDefined && (np > 96));
				toolStripMenuItem_CommandContextMenu_UpBy_96.Visible =           (cIsDefined && (np > 96));

				toolStripMenuItem_CommandContextMenu_DownBy_1 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_2 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_3 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_4 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_5 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_6 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_7 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_8 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_9 .Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_10.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_11.Visible =            cIsDefined;
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_12.Visible = (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_12.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_13.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_14.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_15.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_16.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_17.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_18.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_19.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_20.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_21.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_22.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_23.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_24.Visible = (cIsDefined && (np > 24));
				toolStripMenuItem_CommandContextMenu_DownBy_24.Visible =           (cIsDefined && (np > 24));
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_36.Visible = (cIsDefined && (np > 36));
				toolStripMenuItem_CommandContextMenu_DownBy_36.Visible =           (cIsDefined && (np > 36));
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_48.Visible = (cIsDefined && (np > 48));
				toolStripMenuItem_CommandContextMenu_DownBy_48.Visible =           (cIsDefined && (np > 48));
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_60.Visible = (cIsDefined && (np > 60));
				toolStripMenuItem_CommandContextMenu_DownBy_60.Visible =           (cIsDefined && (np > 60));
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_72.Visible = (cIsDefined && (np > 72));
				toolStripMenuItem_CommandContextMenu_DownBy_72.Visible =           (cIsDefined && (np > 72));
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_84.Visible = (cIsDefined && (np > 84));
				toolStripMenuItem_CommandContextMenu_DownBy_84.Visible =           (cIsDefined && (np > 84));
				toolStripMenuItem_CommandContextMenu_DownBy_Separator_96.Visible = (cIsDefined && (np > 96));
				toolStripMenuItem_CommandContextMenu_DownBy_96.Visible =           (cIsDefined && (np > 96));

				toolStripMenuItem_CommandContextMenu_Cut   .Enabled = cIsDefined;
				toolStripMenuItem_CommandContextMenu_Copy  .Enabled = cIsDefined;
				toolStripMenuItem_CommandContextMenu_Paste .Enabled = ((id != 0) && (CommandSettingsClipboardHelper.ClipboardContainsText));
				toolStripMenuItem_CommandContextMenu_Clear .Enabled = cIsDefined;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_CommandContextMenu_CopyTo_I_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_CommandContextMenu_CopyTo_I_Click()
			// Changes here may have to be applied there too.

			var targetCommandIndex = (ToolStripMenuItemEx.TagToInt32(sender) - 1); // Attention, 'ToolStripMenuItem' is no 'Control'!

			Command sc;
			if (TryGetCommandFromId(contextMenuStrip_Commands_SelectedCommandId, out sc))
			{
				sc = new Command(sc); // Clone to ensure decoupling. // Replace target by selected:
				this.settingsInEdit.SetCommand(SelectedPageIndex, targetCommandIndex, sc);
			}
			else
			{                                              // Clear target:
				this.settingsInEdit.ClearCommand(SelectedPageIndex, targetCommandIndex);
			}

			DeselectSets(); // Ensure that subsequent actions are done without cursor in one of the sets!
			ActivateSubpage(targetCommandIndex + 1);
			SetPageControls();
			SelectSet(targetCommandIndex + 1);
		}

		private void toolStripMenuItem_CommandContextMenu_MoveTo_I_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_CommandContextMenu_MoveTo_I_Click()
			// Changes here may have to be applied there too.

			var targetCommandIndex = (ToolStripMenuItemEx.TagToInt32(sender) - 1); // Attention, 'ToolStripMenuItem' is no 'Control'!

			Command sc;
			if (TryGetCommandFromId(contextMenuStrip_Commands_SelectedCommandId, out sc))
			{
				this.settingsInEdit.SuspendChangeEvent();

				sc = new Command(sc); // Clone to ensure decoupling. // Replace target by selected:
				this.settingsInEdit.SetCommand(SelectedPageIndex, targetCommandIndex, sc); // Clear selected:
				this.settingsInEdit.ClearCommand(SelectedPageIndex, (contextMenuStrip_Commands_SelectedCommandId - 1));

				this.settingsInEdit.ResumeChangeEvent();
			}
			else
			{                                              // Clear target:
				this.settingsInEdit.ClearCommand(SelectedPageIndex, targetCommandIndex);
			}

			DeselectSets(); // Ensure that subsequent actions are done without cursor in one of the sets!
			ActivateSubpage(targetCommandIndex + 1);
			SetPageControls();
			SelectSet(targetCommandIndex + 1);
		}

		private void toolStripMenuItem_CommandContextMenu_UpBy_N_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_CommandContextMenu_UpBy_N_Click()
			// Changes here may have to be applied there too.

			this.settingsInEdit.SuspendChangeEvent();

			int lastCommandIdPerPage = ((PredefinedCommandPageLayoutEx)this.settingsInEdit.PageLayout).CommandCapacityPerPage;
			int resultingTargetCommandId = 0;
			int selectedCommandId = contextMenuStrip_Commands_SelectedCommandId;
			int n = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
			for (int i = 0; i < n; i++)
			{
				Up(selectedCommandId, lastCommandIdPerPage, out resultingTargetCommandId);

				selectedCommandId--;
				if (selectedCommandId < PredefinedCommandPage.FirstCommandIdPerPage)
					selectedCommandId =                       lastCommandIdPerPage;
			}

			this.settingsInEdit.ResumeChangeEvent();

			DeselectSets(); // Ensure that subsequent actions are done without cursor in one of the sets!
			ActivateSubpage(resultingTargetCommandId);
			SetPageControls();
			SelectSet(resultingTargetCommandId);
		}

		/// <remarks>This class-internal method does not call <see cref="SetControls()"/>.</remarks>
		private void Up(int selectedCommandId, int lastCommandIdPerPage, out int targetCommandId)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.Up()
			// Changes here may have to be applied there too.

			Command sc = null;
			if (TryGetCommandFromId(selectedCommandId, out sc))
				sc = new Command(sc); // Clone to ensure decoupling.

			targetCommandId = ((selectedCommandId > PredefinedCommandPage.FirstCommandIdPerPage) ? (selectedCommandId - 1) : (lastCommandIdPerPage));

			Command tc = null;
			if (TryGetCommandFromId(targetCommandId, out tc))
				tc = new Command(tc); // Clone to ensure decoupling.

			if (tc != null)                            // Replace selected by target:
				this.settingsInEdit.SetCommand(SelectedPageIndex, selectedCommandId - 1, tc);
			else                                           // Clear selected:
				this.settingsInEdit.ClearCommand(SelectedPageIndex, selectedCommandId - 1);

			if (sc != null)                            // Replace target by selected:
				this.settingsInEdit.SetCommand(SelectedPageIndex, targetCommandId - 1, sc);
			else                                           // Clear target:
				this.settingsInEdit.ClearCommand(SelectedPageIndex, targetCommandId - 1);
		}

		private void toolStripMenuItem_CommandContextMenu_DownBy_N_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_CommandContextMenu_DownBy_N_Click()
			// Changes here may have to be applied there too.

			this.settingsInEdit.SuspendChangeEvent();

			int lastCommandIdPerPage = ((PredefinedCommandPageLayoutEx)this.settingsInEdit.PageLayout).CommandCapacityPerPage;
			int resultingTargetCommandId = 0;
			int selectedCommandId = contextMenuStrip_Commands_SelectedCommandId;
			int n = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
			for (int i = 0; i < n; i++)
			{
				Down(selectedCommandId, lastCommandIdPerPage, out resultingTargetCommandId);

				selectedCommandId++;
				if (selectedCommandId >                       lastCommandIdPerPage)
					selectedCommandId = PredefinedCommandPage.FirstCommandIdPerPage;
			}

			this.settingsInEdit.ResumeChangeEvent();

			DeselectSets(); // Ensure that subsequent actions are done without cursor in one of the sets!
			ActivateSubpage(resultingTargetCommandId);
			SetPageControls();
			SelectSet(resultingTargetCommandId);
		}

		/// <remarks>This class-internal method does not call <see cref="SetControls()"/>.</remarks>
		private void Down(int selectedCommandId, int lastCommandIdPerPage, out int targetCommandId)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.Down()
			// Changes here may have to be applied there too.

			Command sc = null;
			if (TryGetCommandFromId(selectedCommandId, out sc))
				sc = new Command(sc); // Clone to ensure decoupling.

			targetCommandId = ((selectedCommandId < lastCommandIdPerPage) ? (selectedCommandId + 1) : (PredefinedCommandPage.FirstCommandIdPerPage));

			Command tc = null;
			if (TryGetCommandFromId(targetCommandId, out tc))
				tc = new Command(tc); // Clone to ensure decoupling.

			if (tc != null)                            // Replace selected by target:
				this.settingsInEdit.SetCommand(SelectedPageIndex, selectedCommandId - 1, tc);
			else                                           // Clear selected:
				this.settingsInEdit.ClearCommand(SelectedPageIndex, selectedCommandId - 1);

			if (sc != null)                            // Replace target by selected:
				this.settingsInEdit.SetCommand(SelectedPageIndex, targetCommandId - 1, sc);
			else                                           // Clear target:
				this.settingsInEdit.ClearCommand(SelectedPageIndex, targetCommandId - 1);
		}

		private void toolStripMenuItem_CommandContextMenu_Cut_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_CommandContextMenu_Cut_Click()
			// Changes here may have to be applied there too.

			Command sc;
			if (TryGetCommandFromId(contextMenuStrip_Commands_SelectedCommandId, out sc))
			{
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case cutting takes long.
				if (CommandSettingsClipboardHelper.TrySet(this, sc))
				{
					this.settingsInEdit.ClearCommand(SelectedPageIndex, (contextMenuStrip_Commands_SelectedCommandId - 1));
					SetControls();
				}
				Cursor = Cursors.Default;
			}
		}

		private void toolStripMenuItem_CommandContextMenu_Copy_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.Terminal.toolStripMenuItem_CommandContextMenu_Copy_Click()
			// Changes here may have to be applied there too.

			Command sc;
			if (TryGetCommandFromId(contextMenuStrip_Commands_SelectedCommandId, out sc))
			{
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case cutting takes long.
				CommandSettingsClipboardHelper.TrySet(this, sc);
				Cursor = Cursors.Default;
			}
		}

		private void toolStripMenuItem_CommandContextMenu_Paste_Click(object sender, EventArgs e)
		{
			Command cc;
			if (CommandSettingsClipboardHelper.TryGet(this, out cc))
			{
				this.settingsInEdit.SetCommand(SelectedPageIndex, contextMenuStrip_Commands_SelectedCommandId - 1, cc);
				SetControls();
			}
		}

		private void toolStripMenuItem_CommandContextMenu_Clear_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			this.settingsInEdit.ClearCommand(SelectedPageIndex, contextMenuStrip_Commands_SelectedCommandId);
			SetControls();
		}

		#endregion

		#endregion

		#region Non-Public Properties
		//==========================================================================================
		// Non-Public Properties
		//==========================================================================================

		private int SelectedPageIndex
		{
			get { return (this.selectedPageId - 1); }
		}

		private int SelectedSubpageIndex
		{
			get { return (this.selectedSubpageId - 1); }
		}

		private int SelectedSubpageCommandIndexOffset
		{
			get { return (SelectedSubpageIndex * PredefinedCommandPage.CommandCapacityPerSubpage); }
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Controls
		//------------------------------------------------------------------------------------------

		private void InitializeControls(bool useExplicitDefaultRadix)
		{
			comboBox_Layout.Items.AddRange(PredefinedCommandPageLayoutEx.GetItems());

			          // (                     distance / 2                      )
			var deltaX = ((subpageCheckBox_1B.Left - subpageCheckBox_1A.Left) / 2);
			var deltaY = ((subpageCheckBox_2A.Top  - subpageCheckBox_1A.Top ) / 2);

			this.subpageCheckBoxLocationTopLeft     = subpageCheckBox_1A.Location;
			this.subpageCheckBoxLocationLeftAbove   = new Point((subpageCheckBox_1A.Left + deltaX), (subpageCheckBox_1A.Top + deltaY));
			this.subpageCheckBoxLocationCenter      = subpageCheckBox_2B.Location;
			this.subpageCheckBoxLocationRightBelow  = new Point((subpageCheckBox_2B.Left + deltaX), (subpageCheckBox_2B.Top + deltaY));
		////this.subpageCheckBoxLocationBottomRight = subpageCheckBox_3C.Location; is not needed.

			if (!useExplicitDefaultRadix)
			{
				label_ExplicitDefaultRadix.Visible = false;
				label_Data.Left = label_ExplicitDefaultRadix.Left;
			}
			else
			{
				label_ExplicitDefaultRadix.Visible = true;
			////label_Data.Left is kept at the designed location.
			}

			this.predefinedCommandSettingsSetLabels = new List<Label>(PredefinedCommandPage.CommandCapacityPerSubpage); // Preset the required capacity to improve memory management.
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

			this.predefinedCommandSettingsSets = new List<Controls.PredefinedCommandSettingsSet>(PredefinedCommandPage.CommandCapacityPerSubpage); // Preset the required capacity to improve memory management.
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
		}

		private void SetLayoutControls()
		{
			this.isSettingControls.Enter();
			try
			{
				comboBox_Layout.SelectedItem = (PredefinedCommandPageLayoutEx)this.settingsInEdit.PageLayout;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetPagesControls()
		{
			this.isSettingControls.Enter();
			try
			{
				// Attention:
				// Similar code exists in...
				// ...View.Controls.PredefinedCommands.SetSelectedPageControls()
				// Changes here may have to be applied there too.

				var pageCount = this.settingsInEdit.Pages.Count;
				if (pageCount > 0)
				{
					listBox_Pages.Enabled = true;
					listBox_Pages.Items.Clear();

					int id = 1;
					foreach (var p in this.settingsInEdit.Pages)
						listBox_Pages.Items.Add(PredefinedCommandPage.CaptionOrFallback(p, id++));

					var pageIsSelected = (this.selectedPageId != 0);
					if (pageIsSelected)
						listBox_Pages.SelectedIndex = SelectedPageIndex;
				}
				else
				{
					listBox_Pages.Enabled = false;
					listBox_Pages.Items.Clear();
				}

				SetPagesButtonsControls();
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Needed as separate method for performance/flickering optimization.
		/// <see cref="predefinedCommandSettingsSet_CommandChanged"/> requires to
		/// update the pages buttons, but does not require to update the pages list.
		/// </remarks>
		private void SetPagesButtonsControls()
		{
			this.isSettingControls.Enter();
			try
			{
				int pageCount = this.settingsInEdit.Pages.Count;
				bool pageIsSelected = (this.selectedPageId != 0);

				int totalDefinedCommandCount = this.settingsInEdit.Pages.TotalDefinedCommandCount;
				int selectedPageDefinedCommandCount = 0;
				if (pageIsSelected)
					selectedPageDefinedCommandCount = this.settingsInEdit.Pages[SelectedPageIndex].DefinedCommandCount;

				button_NamePage                   .Enabled =  pageIsSelected;
				button_RenumberPages              .Enabled = (pageCount > 0);
				button_InsertPage                 .Enabled =  pageIsSelected;
				button_InsertPagesFromFile        .Enabled =  pageIsSelected;
				button_InsertPageFromClipboard    .Enabled =  pageIsSelected;
			////button_AddPage                    .Enabled =  true;
			////button_AddPagesFromFile           .Enabled =  true;
			////button_AddPagesFromClipboard      .Enabled =  true;
				button_DuplicatePage              .Enabled =  pageIsSelected;
				button_ExportPageToFile           .Enabled = (pageIsSelected && (selectedPageDefinedCommandCount > 0));
				button_CopyPageToClipboard        .Enabled = (pageIsSelected && (selectedPageDefinedCommandCount > 0));
				button_CutPageToClipboard         .Enabled = (pageIsSelected && (selectedPageDefinedCommandCount > 0)); // Deleting a sole page is permissible.
				button_DeletePage                 .Enabled =  pageIsSelected;                                           // Deleting a sole page is permissible.
				button_MovePageUp                 .Enabled = (pageIsSelected && (this.selectedPageId > 1));
				button_MovePageDown               .Enabled = (pageIsSelected && (this.selectedPageId < pageCount));
				button_DeleteAllPages             .Enabled = (pageCount > 0);                                           // Deleting a sole page is permissible.
				label_CopyExportAllPages          .Enabled = (pageCount > 0) && (totalDefinedCommandCount > 0);
				button_ExportAllPagesToFile       .Enabled = (pageCount > 0) && (totalDefinedCommandCount > 0);
				button_ExportAllPagesToClipboard  .Enabled = (pageCount > 0) && (totalDefinedCommandCount > 0);
			////label_PasteImportAllPages         .Enabled =  true;
			////button_ImportAllPagesFromFile     .Enabled =  true;
			////button_ImportAllPagesFromClipboard.Enabled =  true;
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
				var pageIsLinked = false;
				var pageIsSelected = (this.selectedPageId != 0);

				var sb = new StringBuilder();

				if (pageIsSelected)
					sb.Append(PredefinedCommandPage.CaptionOrFallback(this.settingsInEdit.Pages[SelectedPageIndex], SelectedPageIndex));
				else
					sb.Append("<No Page Selected>");

				if ((this.settingsInEdit.PageLayout != PredefinedCommandPageLayout.OneByOne) && (this.selectedSubpageId != PredefinedCommandPage.NoSubpageId))
					sb.Append(" | Subpage " + PredefinedCommandPage.SubpageIdToString(this.selectedSubpageId));

				groupBox_Page.Text = sb.ToString();
				groupBox_Page.Enabled = pageIsSelected;

				if (pageIsSelected)
				{
					// Attention:
					// Similar code exists in...
					// ...View.Controls.PredefinedCommandButtonSet.SetCommandTextControls()
					// ...View.Controls.PredefinedCommandButtonSet.SetCommandStateControls()
					// ...View.Controls.PredefinedCommandButtonSet.CommandRequest()
					// ...View.Forms.Terminal.contextMenuStrip_Command_SetMenuItems()
					// Changes here may have to be applied there too.

					int commandCount = 0;
					var commands = this.settingsInEdit.Pages[SelectedPageIndex].Commands;
					if (commands != null)
						commandCount = commands.Count;

					for (int i = 0; i < PredefinedCommandPage.CommandCapacityPerSubpage; i++)
					{
						int commandIndex = (SelectedSubpageCommandIndexOffset + i);
						if ((commandIndex < commandCount) && (commands[commandIndex] != null))
							this.predefinedCommandSettingsSets[i].Command = new Command(commands[commandIndex]); // Clone to ensure decoupling.
						else
							this.predefinedCommandSettingsSets[i].Command = null;

						int commandId = (commandIndex + 1);
						string commandIdValue = commandId.ToString(CultureInfo.CurrentCulture);
						string commandIdText = commandIdValue.Insert((commandIdValue.Length - 1), "&") + ":";
						this.predefinedCommandSettingsSetLabels[i].Text = commandIdText;
					}

					pageIsLinked            = this.settingsInEdit.Pages[SelectedPageIndex].IsLinkedToFilePath;
					pathLabel_LinkedTo.Text = this.settingsInEdit.Pages[SelectedPageIndex].LinkFilePath; // Setting 'null' is permissible.
				}

				button_LinkToFile.Enabled  =  pageIsSelected;
				pathLabel_LinkedTo.Enabled = (pageIsSelected && pageIsLinked);
				pathLabel_LinkedTo.Visible = (pageIsSelected && pageIsLinked);
				button_ClearLink.Enabled   = (pageIsSelected && pageIsLinked);
				button_ClearLink.Visible   = (pageIsSelected && pageIsLinked);

				SetPageSubpageControls();
			}
			finally
			{
				this.isSettingControls.Leave();
			}

			SetClearControls(); // See remarks below.
		}

		/// <remarks>
		/// Needed as separate method for performance/flickering optimization.
		/// <see cref="predefinedCommandSettingsSet_CommandChanged"/> will update
		/// the command settings, but does not require the whole page to be updated.
		/// Still, the clear controls need to be updated. This is done by this method.
		/// </remarks>
		private void SetClearControls()
		{
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
			}
		}

		/// <remarks>
		/// Separate method for not overburdening <see cref="SetPageControls"/>.
		/// </remarks>
		private void SetPageSubpageControls()
		{
			SuspendLayout(); // Useful as the 'Size' and 'Location' properties will get changed.
			this.isSettingControls.Enter();
			try
			{
				Point location;
				int subpageId1, subpageId2;
				int leftA, leftB;
				int top1, top2;

				PredefinedCommandPageLayout pageLayout = this.settingsInEdit.PageLayout;
				PredefinedCommandPageLayoutEx pageLayoutEx = pageLayout;

				switch (pageLayout)
				{                                                // \remind (2019-08-05 / MKY):
					case PredefinedCommandPageLayout.TwoByOne:   // Could be calculated initially.
					case PredefinedCommandPageLayout.ThreeByOne: location = new Point(101, 16); break;
					case PredefinedCommandPageLayout.OneByTwo:   location = new Point(165, 31); break;
					case PredefinedCommandPageLayout.OneByThree: location = new Point(135, 31); break;
					case PredefinedCommandPageLayout.TwoByTwo:   location = new Point(165, 16); break;
					case PredefinedCommandPageLayout.TwoByThree: location = new Point(135, 16); break;
					case PredefinedCommandPageLayout.ThreeByTwo: location = new Point( 69, 16); break;
					default:                                     location = new Point( 38, 16); break;
				}

				label_SubpageSelection.Location = location;
				label_SubpageSelection.Visible = (pageLayout != PredefinedCommandPageLayout.OneByOne);

				// Attention:
				// Similar code exists in...
				// ...View.Controls.PredefinedCommands.SetControls()
				// Changes here may have to be applied there too.

			////subpageCheckBox_1A.SubpageId = 1 is fixed.
			////subpageCheckBox_2A.SubpageId = 2 is fixed.
			////subpageCheckBox_3A.SubpageId = 3 is fixed.

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.OneByTwo:
					case PredefinedCommandPageLayout.OneByThree: subpageId1 = 2; break;
					case PredefinedCommandPageLayout.TwoByTwo:
					case PredefinedCommandPageLayout.TwoByThree: subpageId1 = 3; break;
					default:                                                 subpageId1 = 4; break;
				}

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.TwoByTwo:
					case PredefinedCommandPageLayout.TwoByThree: subpageId2 = 4; break;
					default:                                                 subpageId2 = 5; break;
				}

				subpageCheckBox_1B.SubpageId = subpageId1;
				subpageCheckBox_2B.SubpageId = subpageId2;
			////subpageCheckBox_3B.SubpageId = 6 is fixed

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.OneByThree: subpageId1 = 3; break;
					case PredefinedCommandPageLayout.TwoByThree: subpageId1 = 5; break;
					default:                                                 subpageId1 = 7; break;
				}

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.TwoByThree: subpageId2 = 6; break;
					default:                                                 subpageId2 = 8; break;
				}

				subpageCheckBox_1C.SubpageId = subpageId1;
				subpageCheckBox_2C.SubpageId = subpageId2;
			////subpageCheckBox_3C.SubpageId = 9 is fixed.

				subpageCheckBox_1A.Visible = (pageLayoutEx.ColumnsPerPage >  1) || (pageLayoutEx.RowsPerPage >  1);
				subpageCheckBox_2A.Visible =                                       (pageLayoutEx.RowsPerPage >= 2);
				subpageCheckBox_3A.Visible =                                       (pageLayoutEx.RowsPerPage >= 3);
				subpageCheckBox_1B.Visible = (pageLayoutEx.ColumnsPerPage >= 2);
				subpageCheckBox_2B.Visible = (pageLayoutEx.ColumnsPerPage >= 2) && (pageLayoutEx.RowsPerPage >= 2);
				subpageCheckBox_3B.Visible = (pageLayoutEx.ColumnsPerPage >= 2) && (pageLayoutEx.RowsPerPage >= 3);
				subpageCheckBox_1C.Visible = (pageLayoutEx.ColumnsPerPage >= 3);
				subpageCheckBox_2C.Visible = (pageLayoutEx.ColumnsPerPage >= 3) && (pageLayoutEx.RowsPerPage >= 2);
				subpageCheckBox_3C.Visible = (pageLayoutEx.ColumnsPerPage >= 3) && (pageLayoutEx.RowsPerPage >= 3);

				if      (pageLayoutEx.ColumnsPerPage >= 3)    leftA = this.subpageCheckBoxLocationTopLeft   .X;
				else if (pageLayoutEx.ColumnsPerPage >= 2)    leftA = this.subpageCheckBoxLocationLeftAbove .X;
				else /* (pageLayoutEx.ColumnsPerPage >= 1) */ leftA = this.subpageCheckBoxLocationCenter    .X;

				if      (pageLayoutEx.ColumnsPerPage >= 3)    leftB = this.subpageCheckBoxLocationCenter    .X;
				else /* (pageLayoutEx.ColumnsPerPage >= 2) */ leftB = this.subpageCheckBoxLocationRightBelow.X;

				if      (pageLayoutEx.RowsPerPage    >= 3)    top1  = this.subpageCheckBoxLocationTopLeft   .Y;
				else if (pageLayoutEx.RowsPerPage    >= 2)    top1  = this.subpageCheckBoxLocationLeftAbove .Y;
				else /* (pageLayoutEx.RowsPerPage    >= 1) */ top1  = this.subpageCheckBoxLocationCenter    .Y;

				if      (pageLayoutEx.RowsPerPage    >= 3)    top2  = this.subpageCheckBoxLocationCenter    .Y;
				else /* (pageLayoutEx.RowsPerPage    >= 2) */ top2  = this.subpageCheckBoxLocationRightBelow.Y;

				subpageCheckBox_1A.Left = leftA;
				subpageCheckBox_1A.Top  = top1;
				subpageCheckBox_2A.Left = leftA;
				subpageCheckBox_2A.Top  = top2;
				subpageCheckBox_3A.Left = leftA;
			////subpageCheckBox_3A.Top  = top3 is fixed.
				subpageCheckBox_1B.Left = leftB;
				subpageCheckBox_1B.Top  = top1;
				subpageCheckBox_2B.Left = leftB;
				subpageCheckBox_2B.Top  = top2;
				subpageCheckBox_3B.Left = leftB;
			////subpageCheckBox_3B.Top  = top3 is fixed.
			////subpageCheckBox_1C.Left = leftC is fixed.
				subpageCheckBox_1C.Top  = top1;
			////subpageCheckBox_2C.Left = leftC is fixed.
				subpageCheckBox_2C.Top  = top2;
			////subpageCheckBox_3C.Left = leftC is fixed.
			////subpageCheckBox_3C.Top  = top3 is fixed.

				subpageCheckBox_1A.Checked = (subpageCheckBox_1A.SubpageId == this.selectedSubpageId);
				subpageCheckBox_2A.Checked = (subpageCheckBox_2A.SubpageId == this.selectedSubpageId);
				subpageCheckBox_3A.Checked = (subpageCheckBox_3A.SubpageId == this.selectedSubpageId);
				subpageCheckBox_1B.Checked = (subpageCheckBox_1B.SubpageId == this.selectedSubpageId);
				subpageCheckBox_2B.Checked = (subpageCheckBox_2B.SubpageId == this.selectedSubpageId);
				subpageCheckBox_3B.Checked = (subpageCheckBox_3B.SubpageId == this.selectedSubpageId);
				subpageCheckBox_1C.Checked = (subpageCheckBox_1C.SubpageId == this.selectedSubpageId);
				subpageCheckBox_2C.Checked = (subpageCheckBox_2C.SubpageId == this.selectedSubpageId);
				subpageCheckBox_3C.Checked = (subpageCheckBox_3C.SubpageId == this.selectedSubpageId);
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout(false);
			}
		}

		#endregion

		#region Non-Public Methods > Pages
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Pages
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void NamePage()
		{
			string pageName;
			if (TextInputBox.Show
				(
					this,
					"Enter page name:",
					"Page Name",
					this.settingsInEdit.Pages[SelectedPageIndex].Name,
					out pageName
				)
				== DialogResult.OK)
			{
				this.settingsInEdit.Pages[SelectedPageIndex].Name = pageName;
				SetPagesControls();
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void RenumberPages()
		{
			string message;
			switch (this.settingsInEdit.Pages.Count)
			{
				case 1:  message = @"Renumber page to ""Page 1""?";                               break;
				case 2:  message = @"Renumber pages to ""Page 1"", ""Page 2""?";                  break;
				case 3:  message = @"Renumber pages to ""Page 1"", ""Page 2"" , ""Page 3""?";     break;
				default: message = @"Renumber pages to ""Page 1"", ""Page 2"" , ""Page 3"",...?"; break;
			}

			if (MessageBoxEx.Show
				(
					this,
					message,
					"Renumber?",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button2
				)
				== DialogResult.OK)
			{
				this.settingsInEdit.Pages.Renumber();
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
				int commandCapacityPerPage = ((PredefinedCommandPageLayoutEx)this.settingsInEdit.PageLayout).CommandCapacityPerPage;
				var pcp = new PredefinedCommandPage(commandCapacityPerPage, pageName);
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
				int commandCapacityPerPage = ((PredefinedCommandPageLayoutEx)this.settingsInEdit.PageLayout).CommandCapacityPerPage;
				var pcp = new PredefinedCommandPage(commandCapacityPerPage, pageName);
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
					this.settingsInEdit.Pages[SelectedPageIndex].Name + " (copy)",
					out pageName
				)
				== DialogResult.OK)
			{
				var pcp = new PredefinedCommandPage(this.settingsInEdit.Pages[SelectedPageIndex]);
				pcp.Name = pageName;
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
					"Delete page '" + PredefinedCommandPage.CaptionOrFallback(this.settingsInEdit.Pages[SelectedPageIndex], SelectedPageIndex) + "'?",
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
				else // Same behavior as DeleteAllPages():
				{
					this.settingsInEdit.ClearAndCreateDefaultPage();
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
				this.settingsInEdit.ClearAndCreateDefaultPage();
				this.selectedPageId = 1;
				SetControls();
			}
		}

		private void ApplySettingsAndSetControls(Model.Settings.PredefinedCommandSettings settings)
		{
			if (settings.Pages.Count > 0)
			{
				if (this.selectedPageId > settings.Pages.Count)
					this.selectedPageId = settings.Pages.Count;

				this.settingsInEdit = settings;
				SetControls();
			}
			else // Same behavior as DeleteAllPages():
			{
				this.settingsInEdit.ClearAndCreateDefaultPage();
				this.selectedPageId = 1;
				SetControls();
			}
		}

		#endregion

		#region Non-Public Methods > Page/Subpage
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Page/Subpage
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sets <paramref name="command"/> to the command specified by <paramref name="id"/>.
		/// Returns <c>false</c> and sets <paramref name="command"/> to <c>null</c> if command is undefined or invalid.
		/// </summary>
		protected virtual bool TryGetCommandFromId(int id, out Command command)
		{
			if (this.settingsInEdit.Pages != null)
			{
				var p = this.settingsInEdit.Pages[SelectedPageIndex];
				if (p != null)
				{
					var i = (id - 1);
					if (i < p.Commands.Count)
					{
						command = p.Commands[i];
						return (true);
					}
				}
			}

			command = null;
			return (false);
		}

		/// <summary>
		/// Sets <paramref name="id"/> to settings set (1..max) that is assigned to the set at the specified location.
		/// Returns <c>false</c> and sets <paramref name="id"/> to <c>0</c> if no set.
		/// </summary>
		protected virtual bool TryGetCommandIdFromLocation(Point location, out int id)
		{
			Point pt = groupBox_Page.PointToClient(location);
			//// Not using GetChildAtPoint() to also support clicking inbetween sets.

			// Ensure that location is within control:
			if ((pt.X < 0) || (pt.X > Width))  { id = 0; return (false); }
			if ((pt.Y < 0) || (pt.Y > Height)) { id = 0; return (false); }

			// Ensure that location is around sets:
			if (pt.Y < predefinedCommandSettingsSet_1.Top)     { id = 0; return (false); }
			if (pt.Y > predefinedCommandSettingsSet_12.Bottom) { id = 0; return (false); }

			// Find the corresponding set:
			for (int i = 0; i < this.predefinedCommandSettingsSets.Count; i++)
			{
				if (pt.Y <= this.predefinedCommandSettingsSets[i].Bottom)
				{
					id = (i + 1); // ID = 1..max
					id += (SelectedSubpageIndex * PredefinedCommandPage.CommandCapacityPerSubpage);
					return (true);
				}
			}

			id = 0;
			return (false);
		}

		/// <param name="setId">Set 1..<see cref="PredefinedCommandPage.CommandCapacityPerSubpage"/>.</param>
		protected virtual void UpdateCommandFromSettingsSetId(int setId)
		{
			if (this.settingsInEdit.Pages != null)
			{
				var relativeCommandIndex = (setId - 1);
				var absoluteCommandIndex = (SelectedSubpageCommandIndexOffset + relativeCommandIndex);

				var p = this.settingsInEdit.Pages[SelectedPageIndex];
				var c = this.predefinedCommandSettingsSets[relativeCommandIndex].Command;
				if ((c != null) && (c.IsDefined)) // Filter-out "<Enter text...>" dummy commands.
					p.SetCommand(absoluteCommandIndex, new Command(c)); // Clone to ensure decoupling.
				else
					p.ClearCommand(absoluteCommandIndex);
			}
		}

		/// <remarks>This class-internal method does not call <see cref="SetControls()"/>.</remarks>
		protected virtual void ActivateSubpage(int requestedCommandId)
		{
			requestedCommandId = Int32Ex.Limit(requestedCommandId, 1, PredefinedCommandPage.MaxCommandCapacityPerPage); // 'Max' is 1 or above.

			var requestedCommandIndex = (requestedCommandId - 1);
			var requestedSubpageIndex = (requestedCommandIndex / PredefinedCommandPage.CommandCapacityPerSubpage);
			this.selectedSubpageId = (requestedSubpageIndex + 1);
		}

		/// <remarks>This class-internal method does not call <see cref="SetControls()"/>.</remarks>
		protected virtual void SelectSet(int requestedCommandId)
		{
			requestedCommandId = Int32Ex.Limit(requestedCommandId, 1, PredefinedCommandPage.MaxCommandCapacityPerPage); // 'Max' is 1 or above.

			var requestedCommandIndex = (requestedCommandId - 1);
			var requestedSetIndex = (requestedCommandIndex % PredefinedCommandPage.CommandCapacityPerSubpage);
			var requestedSet = this.predefinedCommandSettingsSets[requestedSetIndex];
			requestedSet.PrepareUserInput(); // See remarks of that method!
			requestedSet.Select();
		}

		/// <remarks>This class-internal method does not call <see cref="SetControls()"/>.</remarks>
		protected virtual void DeselectSets()
		{
			button_Cancel.Select(); // Select a control that is always active.
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		protected virtual void ClearPage()
		{
			if (MessageBoxEx.Show
				(
				this,
				"Clear all commands of page '" + PredefinedCommandPage.CaptionOrFallback(this.settingsInEdit.Pages[SelectedPageIndex], SelectedPageIndex) + "'?",
				"Clear Page?",
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

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		protected virtual void ShowHelp()
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
