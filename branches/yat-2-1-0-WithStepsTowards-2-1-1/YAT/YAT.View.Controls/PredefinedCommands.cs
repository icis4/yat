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
using MKY.Collections;
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

		private List<PredefinedCommandButtonSet> buttonSets;
		private float buttonSetHeight;

		private SettingControlsHelper isSettingControls;

		private PredefinedCommandPageLayout pageLayout;
		private PredefinedCommandPageCollection pages;
		private int selectedPageId = SelectedPageIdDefault;

		private Domain.Parser.Modes parseModeForText = ParseModeForTextDefault;
		private string rootDirectoryForFile; // = null;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;

		private int commandStateUpdateSuspendedCount; // = 0;

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
					SetPageLayoutControls();
					OnPageLayoutChanged(EventArgs.Empty);
				}
			}
		}

		/// <remarks>
		/// For performance reasons, opposed to other control properties, this property does not
		/// only set the properly if it has changed, but rather sets it always. Reasons:
		/// <list type="bullet">
		/// <item><description><code>IEnumerableEx.ItemsEqual()</code> takes time.</description></item>
		/// <item><description>This property is only set by the parent terminal if the commands changed indeed.</description></item>
		/// </list>
		/// </remarks>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual PredefinedCommandPageCollection Pages
		{
			get { return (this.pages); }
			set
			{
			////if (!IEnumerableEx.ItemsEqual(this.pages, value)) <= See remarks above for explanation.
			////{
					this.pages = value;

					if ((this.pages == null) || (this.pages.Count == 0)) // Select page 1 even if there are no pages.
						SelectedPageId = 1;
					else if (this.selectedPageId > this.pages.Count)
						SelectedPageId = this.pages.Count;
					else
						SetSelectedPageControls();

					SetCommandControls(); // Update commands to newly selected page.
			////}
			}
		}

		/// <summary></summary>
		[Category("Commands")]
		[Description("The selected page.")]
		[DefaultValue(SelectedPageIdDefault)]
		public virtual int SelectedPageId
		{
			get { return (this.selectedPageId); }
			set
			{
				int selectedPageIdNew;
				if ((this.pages != null) && (this.pages.Count > 0))
					selectedPageIdNew = Int32Ex.Limit(value, SelectedPageIdDefault, this.pages.Count); // 'Count' is 1 or above.
				else
					selectedPageIdNew = SelectedPageIdDefault;

				if (this.selectedPageId != selectedPageIdNew)
				{
					this.selectedPageId = selectedPageIdNew;
					SetSelectedPageControls();
					OnSelectedPageIdChanged(EventArgs.Empty);

					SetCommandControls(); // Update commands to newly selected page.
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int SelectedPageIndex
		{
			get { return (SelectedPageId - 1); }
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
				SetCommandStateControls();
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
				SetCommandStateControls();
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
				SetCommandStateControls();
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
			SelectedPageId++;
		}

		/// <summary></summary>
		public virtual void PreviousPage()
		{
			SelectedPageId--;
		}

		/// <summary>
		/// Returns command at the specified <paramref name="id"/>.
		/// Returns <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromId(int id)
		{
			List<Command> commands = null;
			if ((this.pages != null) && (this.pages.Count > 0))
				commands = this.pages[SelectedPageIndex].Commands;

			if (commands != null)
			{
				int i = (id - 1); // ID = 1..max
				if ((i >= 0) && (i < commands.Count))
				{
					var c = commands[i];
					if (c != null)
					{
						if (c.IsDefined)
							return (c);
					}
				}
			}

			return (null);
		}

		/// <summary>
		/// Returns command ID (1..max) that is assigned to the button at the specified location.
		/// Returns 0 if no button.
		/// </summary>
		public virtual int GetCommandIdFromLocation(Point location)
		{
			Point pt = tableLayoutPanel_Subpages.PointToClient(location);

			var child = tableLayoutPanel_Subpages.GetChildAtPoint(pt);
			if (child != null)
			{
				var set = (child as PredefinedCommandButtonSet);
				if (set != null)
					return (set.GetCommandIdFromLocation(location));
			}

			return (0);
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromLocation(Point location)
		{
			return (GetCommandFromId(GetCommandIdFromLocation(location)));
		}

		/// <remarks>Useful to improve performance.</remarks>
		public virtual void SuspendCommandStateUpdate()
		{
			this.commandStateUpdateSuspendedCount++;

			foreach (var set in this.buttonSets)
				set.SuspendCommandStateUpdate();
		}

		/// <remarks>Useful to improve performance.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void ResumeCommandStateUpdate(bool performUpdate = true)
		{
			foreach (var set in this.buttonSets)
				set.ResumeCommandStateUpdate(!performUpdate); // Otherwise, update will be done by SetCommandStateControls() below.

			this.commandStateUpdateSuspendedCount--;
			if (this.commandStateUpdateSuspendedCount == 0)
			{
				if (performUpdate)
					SetCommandStateControls();
			}

			if (this.commandStateUpdateSuspendedCount < 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The " + this.GetType() + " command state update suspend counter has become less than 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <remarks>Useful to improve performance.</remarks>
		public virtual bool CommandStateUpdateIsSuspended
		{
			get { return (this.commandStateUpdateSuspendedCount > 0); }
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

		private void buttonSet_I_SendCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			RequestSendCommand(SelectedPageId, e.CommandId); // e.PageId is not defined since set doesn't know the page.
		}

		private void buttonSet_I_DefineCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			RequestDefineCommand(SelectedPageId, e.CommandId); // e.PageId is not defined since set doesn't know the page.
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

			SelectedPageId = (comboBox_Pages.SelectedIndex + 1);
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.buttonSets = new List<PredefinedCommandButtonSet>(PredefinedCommandPage.MaxSubpageCount); // Preset the required capacity to improve memory management.
			this.buttonSets.Add(buttonSet_1A);
			this.buttonSets.Add(buttonSet_1B);
			this.buttonSets.Add(buttonSet_1C);
			this.buttonSets.Add(buttonSet_2A);
			this.buttonSets.Add(buttonSet_2B);
			this.buttonSets.Add(buttonSet_2C);
			this.buttonSets.Add(buttonSet_3A);
			this.buttonSets.Add(buttonSet_3B);
			this.buttonSets.Add(buttonSet_3C);

			this.buttonSetHeight = buttonSet_1A.Height;
		}

		private void SetControls()
		{
			SetPageLayoutControls();
			SetCommandControls();
			SetSelectedPageControls();
		}

		private void SetPageLayoutControls()
		{
			this.isSettingControls.Enter();
			try
			{
				int subpageId1, subpageId2;

				var pageLayout = this.pageLayout;
				AdjustSubpagePanel(pageLayout);

				// Attention:
				// Similar code exists in...
				// ...View.Forms.PredefinedCommandSettings.SetControls()
				// Changes here may have to be applied there too.

			////buttonSet_1A.SubpageId = 1 is fixed.
			////buttonSet_2A.SubpageId = 2 is fixed.
			////buttonSet_3A.SubpageId = 3 is fixed.

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.OneByTwo:
					case PredefinedCommandPageLayout.OneByThree: subpageId1 = 2; break;
					case PredefinedCommandPageLayout.TwoByTwo:
					case PredefinedCommandPageLayout.TwoByThree: subpageId1 = 3; break;
					default:                                     subpageId1 = 4; break;
				}

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.TwoByTwo:
					case PredefinedCommandPageLayout.TwoByThree: subpageId2 = 4; break;
					default:                                     subpageId2 = 5; break;
				}

				buttonSet_1B.SubpageId = subpageId1;
				buttonSet_2B.SubpageId = subpageId2;
			////buttonSet_3B.SubpageId = 6 is fixed.

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.OneByThree: subpageId1 = 3; break;
					case PredefinedCommandPageLayout.TwoByThree: subpageId1 = 5; break;
					default:                                     subpageId1 = 7; break;
				}

				switch (pageLayout)
				{
					case PredefinedCommandPageLayout.TwoByThree: subpageId2 = 6; break;
					default:                                     subpageId2 = 8; break;
				}

				buttonSet_1C.SubpageId = subpageId1;
				buttonSet_2C.SubpageId = subpageId2;
			////buttonSet_3C.SubpageId = 9 is fixed.
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetCommandControls()
		{
			this.isSettingControls.Enter();
			try
			{
				SetCommandTextControls();
				SetCommandStateControls();
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetCommandTextControls()
		{
			this.isSettingControls.Enter();
			try
			{
				List<Command> commands = null;
				if ((this.pages != null) && (this.pages.Count > 0))
					commands = this.pages[SelectedPageIndex].Commands;

				foreach (var set in this.buttonSets)
				{
					if (set.Visible)
					{
						set.Commands = commands;
					}
					else
					{
						// Simply skip in order to improve performance.
					}
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetCommandStateControls()
		{
			if (CommandStateUpdateIsSuspended)
				return;

			this.isSettingControls.Enter();
			try
			{
				foreach (var set in this.buttonSets)
				{
					if (set.Visible)
					{
						set.SuspendCommandStateUpdate();
						try
						{
							set.ParseModeForText      = this.parseModeForText;
							set.RootDirectoryForFile  = this.rootDirectoryForFile;
							set.TerminalIsReadyToSend = this.terminalIsReadyToSend;
						}
						finally
						{
							set.ResumeCommandStateUpdate();
						}
					}
					else
					{
						// Simply skip in order to improve performance.
					}
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetSelectedPageControls()
		{
			this.isSettingControls.Enter();
			try
			{
				// Attention:
				// Similar code exists in...
				// ...View.Forms.PredefinedCommandSettings.SetPagesControls()
				// Changes here may have to be applied there too.

				var pageCount = ((this.pages != null) ? (this.pages.Count) : (0));
				if (pageCount > 0)
				{
					button_PagePrevious.Enabled = (this.selectedPageId > 1);
					button_PageNext.Enabled     = (this.selectedPageId < pageCount);

					label_Page.Enabled = this.terminalIsReadyToSend;
					label_Page.Text = "Page " + this.selectedPageId + "/" + pageCount;

					comboBox_Pages.Enabled = (pageCount > 1); // No need to navigate a single page.
					comboBox_Pages.Items.Clear();

					int id = 1;
					foreach (var p in this.pages)
						comboBox_Pages.Items.Add(PredefinedCommandPage.CaptionOrFallback(p, id++));

					var pageIsSelected = (this.selectedPageId != 0);
					if (pageIsSelected)
						comboBox_Pages.SelectedIndex = SelectedPageIndex;
				}
				else
				{
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
			}
		}

		private void AdjustSubpagePanel(PredefinedCommandPageLayout layout)
		{
			SuspendLayout(); // Useful as the 'Size' and 'Location' properties of the underlying sets will get changed.
			this.isSettingControls.Enter();
			try
			{
				// Reset layout...
				tableLayoutPanel_Subpages.ColumnStyles.Clear();
				tableLayoutPanel_Subpages.RowStyles.Clear();
				tableLayoutPanel_Subpages.Controls.Clear();

				// ...then recreate:
				switch (layout)
				{
					case PredefinedCommandPageLayout.OneByOne:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = false;
						buttonSet_1C.Visible = false;
						buttonSet_2A.Visible = false;
						buttonSet_2B.Visible = false;
						buttonSet_2C.Visible = false;
						buttonSet_3A.Visible = false;
						buttonSet_3B.Visible = false;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 1;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.RowCount = 1;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.TwoByOne:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = false;
						buttonSet_1C.Visible = false;
						buttonSet_2A.Visible = true;
						buttonSet_2B.Visible = false;
						buttonSet_2C.Visible = false;
						buttonSet_3A.Visible = false;
						buttonSet_3B.Visible = false;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 1;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.RowCount = 2;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2A, 0, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.ThreeByOne:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = false;
						buttonSet_1C.Visible = false;
						buttonSet_2A.Visible = true;
						buttonSet_2B.Visible = false;
						buttonSet_2C.Visible = false;
						buttonSet_3A.Visible = true;
						buttonSet_3B.Visible = false;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 1;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.RowCount = 3;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_3A, 0, 2);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2A, 0, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.OneByTwo:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = true;
						buttonSet_1C.Visible = false;
						buttonSet_2A.Visible = false;
						buttonSet_2B.Visible = false;
						buttonSet_2C.Visible = false;
						buttonSet_3A.Visible = false;
						buttonSet_3B.Visible = false;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 2;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
						tableLayoutPanel_Subpages.RowCount = 1;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1B, 1, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.TwoByTwo:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = true;
						buttonSet_1C.Visible = false;
						buttonSet_2A.Visible = true;
						buttonSet_2B.Visible = true;
						buttonSet_2C.Visible = false;
						buttonSet_3A.Visible = false;
						buttonSet_3B.Visible = false;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 2;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
						tableLayoutPanel_Subpages.RowCount = 2;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2B, 1, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2A, 0, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1B, 1, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.ThreeByTwo:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = true;
						buttonSet_1C.Visible = false;
						buttonSet_2A.Visible = true;
						buttonSet_2B.Visible = true;
						buttonSet_2C.Visible = false;
						buttonSet_3A.Visible = true;
						buttonSet_3B.Visible = true;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 2;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
						tableLayoutPanel_Subpages.RowCount = 2;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_3B, 1, 2);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_3A, 0, 2);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2B, 1, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2A, 0, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1B, 1, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.OneByThree:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = true;
						buttonSet_1C.Visible = true;
						buttonSet_2A.Visible = false;
						buttonSet_2B.Visible = false;
						buttonSet_2C.Visible = false;
						buttonSet_3A.Visible = false;
						buttonSet_3B.Visible = false;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 3;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.RowCount = 1;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1C, 2, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1B, 1, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.TwoByThree:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = true;
						buttonSet_1C.Visible = true;
						buttonSet_2A.Visible = true;
						buttonSet_2B.Visible = true;
						buttonSet_2C.Visible = true;
						buttonSet_3A.Visible = false;
						buttonSet_3B.Visible = false;
						buttonSet_3C.Visible = false;

						tableLayoutPanel_Subpages.ColumnCount = 3;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.RowCount = 2;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2C, 2, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2B, 1, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2A, 0, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1C, 2, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1B, 1, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					case PredefinedCommandPageLayout.ThreeByThree:
					{
					////buttonSet_1A.Visible = true never changes.
						buttonSet_1B.Visible = true;
						buttonSet_1C.Visible = true;
						buttonSet_2A.Visible = true;
						buttonSet_2B.Visible = true;
						buttonSet_2C.Visible = true;
						buttonSet_3A.Visible = true;
						buttonSet_3B.Visible = true;
						buttonSet_3C.Visible = true;

						tableLayoutPanel_Subpages.ColumnCount = 3;
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
						tableLayoutPanel_Subpages.RowCount = 3;
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Absolute, buttonSetHeight));
						tableLayoutPanel_Subpages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_3C, 2, 2);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_3B, 1, 2);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_3A, 0, 2);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2C, 2, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2B, 1, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_2A, 0, 1);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1C, 2, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1B, 1, 0);
						tableLayoutPanel_Subpages.Controls.Add(buttonSet_1A, 0, 0);
						break;
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("layout", MessageHelper.InvalidExecutionPreamble + "'" + layout + "' is a page layout that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout();
			}
		}

		/// <summary></summary>
		protected virtual void RequestSendCommand(int pageId, int commandId)
		{
			OnSendCommandRequest(new PredefinedCommandEventArgs(pageId, commandId));
		}

		/// <summary></summary>
		protected virtual void RequestDefineCommand(int pageId, int commandId)
		{
			OnDefineCommandRequest(new PredefinedCommandEventArgs(pageId, commandId));
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
