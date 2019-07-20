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

		private const int SelectedPageDefault = 1;

		private const Domain.Parser.Modes ParseModeForTextDefault = Domain.Parser.Modes.Default;
		private const bool TerminalIsReadyToSendDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;
		private PredefinedCommandPageCollection pages;
		private int selectedPage = SelectedPageDefault;

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
					selectedPageNew = Int32Ex.Limit(value, SelectedPageDefault, this.pages.Count); // 'Count' is 1 or above.
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
			SelectedPage++;
		}

		/// <summary></summary>
		public virtual void PreviousPage()
		{
			SelectedPage--;
		}

		/// <summary>
		/// Returns command at the specified <paramref name="id"/>.
		/// Returns <c>null</c> if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromId(int id)
		{
			return (pageButtons.GetCommandFromId(id));
		}

		/// <summary>
		/// Returns command ID (1..max) that is assigned to the button at the specified location.
		/// Returns 0 if no button.
		/// </summary>
		public virtual int GetCommandIdFromLocation(Point point)
		{
			return (pageButtons.GetCommandIdFromLocation(point));
		}

		/// <summary>
		/// Returns command that is assigned to the button at the specified location.
		/// Returns <c>null</c> if no button or if command is undefined or invalid.
		/// </summary>
		public virtual Command GetCommandFromLocation(Point point)
		{
			return (pageButtons.GetCommandFromLocation(point));
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
			if (this.isSettingControls)
				return;

			SelectedPage = comboBox_Pages.SelectedIndex + 1;
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private int SelectedPageIndex
		{
			get { return (this.selectedPage - 1); }
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				pageButtons.ParseModeForText      = this.parseModeForText;
				pageButtons.RootDirectoryForFile  = this.rootDirectoryForFile;
				pageButtons.TerminalIsReadyToSend = this.terminalIsReadyToSend;

				if ((this.pages != null) && (this.pages.Count > 0) &&
				    (this.selectedPage >= 1) && (this.selectedPage <= this.pages.Count))
				{
					pageButtons.Commands = this.pages[SelectedPageIndex].Commands;

					button_PagePrevious.Enabled = (this.selectedPage > 1);
					button_PageNext.Enabled     = (this.selectedPage < this.pages.Count);

					label_Page.Enabled = this.terminalIsReadyToSend;
					label_Page.Text = "Page " + this.selectedPage + "/" + this.pages.Count;

					comboBox_Pages.Enabled = (this.pages.Count > 1); // No need to navigate a single page.
					comboBox_Pages.Items.Clear();

					foreach (var p in this.pages)
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
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnSelectedPageChanged(EventArgs e)
		{
			EventHelper.RaiseSync(SelectedPageChanged, this, e);
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
