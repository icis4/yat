//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Model.Types;

#endregion

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DefaultEvent("SendCommandRequest")]
	public partial class PredefinedCommands : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SelectedPageDefault = 1;
		private const bool TerminalIsReadyToSendDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;
		private PredefinedCommandPageCollection pages;
		private int selectedPage = SelectedPageDefault;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the SelectedPage property is changed.")]
		public event EventHandler SelectedPageChanged;

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
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
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
					SelectedPage = 1;
				else if (this.selectedPage > this.pages.Count)
					SelectedPage = this.pages.Count;
				else
					SetControls();
			}
		}

		/// <summary></summary>
		[Category("Commands")]
		[Description("The selected page.")]
		[DefaultValue(SelectedPageDefault)]
		public virtual int SelectedPage
		{
			get { return (this.selectedPage); }
			set
			{
				int selectedPageNew;
				if ((this.pages != null) && (this.pages.Count > 0))
					selectedPageNew = Int32Ex.Limit(value, SelectedPageDefault, this.pages.Count);
				else
					selectedPageNew = SelectedPageDefault;

				if (this.selectedPage != selectedPageNew)
				{
					this.selectedPage = selectedPageNew;
					SetControls();
					OnSelectedPageChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
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
			SelectedPage++;
		}

		/// <summary></summary>
		public virtual void PreviousPage()
		{
			SelectedPage--;
		}

		/// <summary>
		/// Returns command at the specified id.
		/// Returns <c>null</c> if command is undefined or not valid.
		/// </summary>
		public virtual Command GetCommandFromId(int id)
		{
			return (pageButtons.GetCommandFromId(id));
		}

		/// <summary>
		/// Returns command ID (1..max) that is assigned to the button at the specified location.
		/// Returns 0 if no button.
		/// </summary>
		public virtual int GetCommandIdFromScreenPoint(Point point)
		{
			return (pageButtons.GetCommandIdFromScreenPoint(point));
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or not valid.
		/// </summary>
		public virtual Command GetCommandFromScreenPoint(Point point)
		{
			return (pageButtons.GetCommandFromScreenPoint(point));
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void pageButtons_SendCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			RequestSendCommand(e.Command);
		}

		private void pageButtons_DefineCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			RequestDefineCommand(e.Command);
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
			if (!this.isSettingControls)
				SelectedPage = comboBox_Pages.SelectedIndex + 1;
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private int SelectedPageIndex
		{
			get { return (this.selectedPage - 1); }
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			pageButtons.TerminalIsReadyToSend = this.terminalIsReadyToSend;

			if ((this.pages != null) && (this.pages.Count > 0) && (this.selectedPage >= 1) && (this.selectedPage <= this.pages.Count))
			{
				pageButtons.Commands = this.pages[SelectedPageIndex].Commands;

				button_PagePrevious.Enabled = (this.selectedPage > 1);
				button_PageNext.Enabled = (this.selectedPage < this.pages.Count);

				label_Page.Enabled = true;
				label_Page.Text = "Page " + this.selectedPage + "/" + this.pages.Count;

				comboBox_Pages.Enabled = true;
				comboBox_Pages.Items.Clear();
				foreach (PredefinedCommandPage p in this.pages)
					comboBox_Pages.Items.Add(p.PageName);
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

			this.isSettingControls.Leave();
		}

		private void RequestSendCommand(int command)
		{
			OnSendCommandRequest(new PredefinedCommandEventArgs(this.selectedPage, command));
		}

		private void RequestDefineCommand(int command)
		{
			OnDefineCommandRequest(new PredefinedCommandEventArgs(this.selectedPage, command));
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnSelectedPageChanged(EventArgs e)
		{
			EventHelper.FireSync(SelectedPageChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.FireSync<PredefinedCommandEventArgs>(SendCommandRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDefineCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.FireSync<PredefinedCommandEventArgs>(DefineCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
