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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Model.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("SendCommandRequest")]
	public partial class PredefinedCommands : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const PredefinedCommandPageLayout PageLayoutDefault = PredefinedCommandPageLayoutEx.Default;
		private const int SelectedPageIdDefault = 1;

		private const Domain.Parser.Modes ParseModeForTextDefault = Domain.Parser.Modes.Default;
		private const bool TerminalIsReadyToSendDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private List<PredefinedCommandPageButtons> pageButtons;

		private SettingControlsHelper isSettingControls;

		private PredefinedCommandPageLayout pageLayout;
		private PredefinedCommandPageCollection pages;
		private int selectedIdPage = SelectedPageIdDefault;

		private Domain.Parser.Modes parseModeForText = ParseModeForTextDefault;
		private string rootDirectoryForFile; // = null;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the PageLayoutChanged property is changed.")]
		public event EventHandler PageLayoutChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the SelectedPageId property is changed.")]
		public event EventHandler SelectedPageIdChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> SendCommandRequest;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when defining a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> DefineCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public PredefinedCommands()
		{
			InitializeComponent();

			InitializeControls();
		////SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Appearance")]
		[Description("The page layout.")]
		[DefaultValue(PageLayoutDefault)]
		public virtual PredefinedCommandPageLayout PageLayout
		{
			get { return (this.pageLayout); }
			set
			{
				if (this.pageLayout != value)
				{
					this.pageLayout = value;
					SetControls();
					OnPageLayoutChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual PredefinedCommandPageCollection Pages
		{
			get { return (this.pages); }
			set
			{
				this.pages = value;

				if ((this.pages == null) || (this.pages.Count == 0)) // even if no pages, select page 1 anyway
					SelectedIdPage = 1;
				else if (this.selectedIdPage > this.pages.Count)
					SelectedIdPage = this.pages.Count;
				else
					SetControls();
			}
		}

		/// <summary></summary>
		[Category("Commands")]
		[Description("The selected page.")]
		[DefaultValue(SelectedPageIdDefault)]
		public virtual int SelectedIdPage
		{
			get { return (this.selectedIdPage); }
			set
			{
				int selectedPageIdNew;
				if ((this.pages != null) && (this.pages.Count > 0))
					selectedPageIdNew = Int32Ex.Limit(value, SelectedPageIdDefault, this.pages.Count); // 'Count' is 1 or above.
				else
					selectedPageIdNew = SelectedPageIdDefault;

				if (this.selectedIdPage != selectedPageIdNew)
				{
					this.selectedIdPage = selectedPageIdNew;
					SetControls();
					OnSelectedPageIdChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.Parser.Modes ParseModeForText
		{
			set
			{
				this.parseModeForText = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string RootDirectoryForFile
		{
			set
			{
				this.rootDirectoryForFile = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSend
		{
			set
			{
				this.terminalIsReadyToSend = value;
				SetControls();
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void NextPage()
		{
			SelectedIdPage++;
		}

		/// <summary></summary>
		public virtual void PreviousPage()
		{
			SelectedIdPage--;
		}

		/// <summary>
		/// Returns command at the specified <paramref name="id"/>.
		/// Returns <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromId(int id)
		{
			return (pageButtons_1A.GetCommandFromId(id));
		}

		/// <summary>
		/// Returns command ID (1..max) that is assigned to the button at the specified location.
		/// Returns 0 if no button.
		/// </summary>
		public virtual int GetCommandIdFromLocation(Point point)
		{
			return (pageButtons_1A.GetCommandIdFromLocation(point));
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromLocation(Point point)
		{
			return (pageButtons_1A.GetCommandFromLocation(point));
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void PredefinedCommands_Paint(object sender, PaintEventArgs e)
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

		private void pageButtons_SendCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			RequestSendCommand(e.CommandId);
		}

		private void pageButtons_DefineCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			RequestDefineCommand(e.CommandId);
		}

		private void button_PagePrevious_Click(object sender, EventArgs e)
		{
			PreviousPage();
		}

		private void button_PageNext_Click(object sender, EventArgs e)
		{
			NextPage();
		}

		private void comboBox_Pages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SelectedIdPage = (comboBox_Pages.SelectedIndex + 1);
		}

		#endregion

		#region Non-Public Properties
		//==========================================================================================
		// Non-Public Properties
		//==========================================================================================

		private int SelectedPageIndex
		{
			get { return (this.selectedIdPage - 1); }
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.pageButtons = new List<PredefinedCommandPageButtons>(PredefinedCommandPage.MaxSubpageCount); // Preset the required capacity to improve memory management.
			this.pageButtons.Add(pageButtons_1A);
			this.pageButtons.Add(pageButtons_1B);
			this.pageButtons.Add(pageButtons_1C);
			this.pageButtons.Add(pageButtons_2A);
			this.pageButtons.Add(pageButtons_2B);
			this.pageButtons.Add(pageButtons_2C);
			this.pageButtons.Add(pageButtons_3A);
			this.pageButtons.Add(pageButtons_3B);
			this.pageButtons.Add(pageButtons_3C);
		}

		private void SetControls()
		{
			SuspendLayout();
			this.isSettingControls.Enter();
			try
			{
				PredefinedCommandPageLayout pageLayout = this.pageLayout;

				// Attention:
				// Similar code exists in...
				// ...View.Forms.PredefinedCommandSettings.SetControls()
				// Changes here may have to be applied there too.

			////pageButtons_1A.Subpage = 1 is fixed.
				pageButtons_2A.Subpage = ((pageLayout == PredefinedCommandPageLayout.ThreeByTwo)   ? (4) : (2));
			////pageButtons_3A.Subpage = 3 is fixed.
				pageButtons_1B.Subpage = ((pageLayout == PredefinedCommandPageLayout.OneByTwo)   ||
				                          (pageLayout == PredefinedCommandPageLayout.OneByThree) ||
				                          (pageLayout == PredefinedCommandPageLayout.TwoByThree)   ? (2) : (4));
				pageButtons_2B.Subpage = ((pageLayout == PredefinedCommandPageLayout.TwoByTwo)     ? (4) : (5));
				pageButtons_3B.Subpage = ((pageLayout == PredefinedCommandPageLayout.ThreeByThree) ? (8) : (6));
				pageButtons_1C.Subpage = ((pageLayout == PredefinedCommandPageLayout.OneByThree) ||
				                          (pageLayout == PredefinedCommandPageLayout.TwoByThree)   ? (3) : (7));
				pageButtons_2C.Subpage = ((pageLayout == PredefinedCommandPageLayout.TwoByThree)   ? (6) : (8));
			////pageButtons_3C.Subpage = 9 is fixed.

				foreach (var pb in this.pageButtons)
				{
					pb.ParseModeForText      = this.parseModeForText;
					pb.RootDirectoryForFile  = this.rootDirectoryForFile;
					pb.TerminalIsReadyToSend = this.terminalIsReadyToSend;
				}

				if ((this.pages != null) && (this.pages.Count > 0) &&
				    (this.selectedIdPage >= 1) && (this.selectedIdPage <= this.pages.Count))
				{
					foreach (var pb in this.pageButtons)
						pb.Commands = this.pages[SelectedPageIndex].Commands;

					button_PagePrevious.Enabled = (this.selectedIdPage > 1);
					button_PageNext.Enabled     = (this.selectedIdPage < this.pages.Count);

					label_Page.Enabled = this.terminalIsReadyToSend;
					label_Page.Text = "Page " + this.selectedIdPage + "/" + this.pages.Count;

					comboBox_Pages.Enabled = (this.pages.Count > 1); // No need to navigate a single page.
					comboBox_Pages.Items.Clear();

					foreach (var p in this.pages)
						comboBox_Pages.Items.Add(p.PageName);

					comboBox_Pages.SelectedIndex = SelectedPageIndex;
				}
				else
				{
					foreach (var pb in this.pageButtons)
						pb.Commands = null;

					button_PagePrevious.Enabled = false;
					button_PageNext.Enabled = false;

					label_Page.Enabled = false;
					label_Page.Text = "<No Pages>";

					comboBox_Pages.Enabled = false;
					comboBox_Pages.Items.Clear();
				}
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout();
			}
		}

		private void RequestSendCommand(int commandId)
		{
			OnSendCommandRequest(new PredefinedCommandEventArgs(this.selectedIdPage, commandId));
		}

		private void RequestDefineCommand(int commandId)
		{
			OnDefineCommandRequest(new PredefinedCommandEventArgs(this.selectedIdPage, commandId));
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnPageLayoutChanged(EventArgs e)
		{
			EventHelper.RaiseSync(PageLayoutChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSelectedPageIdChanged(EventArgs e)
		{
			EventHelper.RaiseSync(SelectedPageIdChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.RaiseSync<PredefinedCommandEventArgs>(SendCommandRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDefineCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.RaiseSync<PredefinedCommandEventArgs>(DefineCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
