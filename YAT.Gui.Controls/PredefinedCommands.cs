//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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

using MKY.Utilities.Types;
using MKY.Utilities.Event;

using YAT.Model.Types;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class PredefinedCommands : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int SelectedPageDefault = 1;
		private const bool TerminalIsOpenDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;
		private PredefinedCommandPageCollection pages;
		private int selectedPage = SelectedPageDefault;
		private bool terminalIsOpen = TerminalIsOpenDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the SelectedPage property is changed.")]
		public event EventHandler SelectedPageChanged;

		[Category("Action")]
		[Description("Event raised when sending a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> SendCommandRequest;

		[Category("Action")]
		[Description("Event raised when defining a command is requested.")]
		public event EventHandler<PredefinedCommandEventArgs> DefineCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

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
					selectedPageNew = XInt32.LimitToBounds(value, SelectedPageDefault, this.pages.Count);
				else
					selectedPageNew = SelectedPageDefault;

				if (this.selectedPage != selectedPageNew)
				{
					this.selectedPage = selectedPageNew;
					SetControls();
					OnSelectedPageChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsOpen
		{
			set
			{
				this.terminalIsOpen = value;
				SetControls();
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public virtual void NextPage()
		{
			SelectedPage++;
		}

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
		public virtual int GetCommandIdFromScreenPoint(Point p)
		{
			return (pageButtons.GetCommandIdFromScreenPoint(p));
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or not valid.
		/// </summary>
		public virtual Command GetCommandFromScreenPoint(Point p)
		{
			return (pageButtons.GetCommandFromScreenPoint(p));
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
			this.isSettingControls = true;

			pageButtons.TerminalIsOpen = this.terminalIsOpen;

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

			this.isSettingControls = false;
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

		protected virtual void OnSelectedPageChanged(EventArgs e)
		{
			EventHelper.FireSync(SelectedPageChanged, this, e);
		}

		protected virtual void OnSendCommandRequest(PredefinedCommandEventArgs e)
		{
			EventHelper.FireSync<PredefinedCommandEventArgs>(SendCommandRequest, this, e);
		}

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
