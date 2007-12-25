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

		private const int _SelectedPageDefault = 1;
		private const bool _TerminalIsOpenDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;
		private List<PredefinedCommandPage> _pages;
		private int _selectedPage = _SelectedPageDefault;
		private bool _terminalIsOpen = _TerminalIsOpenDefault;

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
		public List<PredefinedCommandPage> Pages
		{
			set
			{
				_pages = value;

				// if no page was selected and page 1 is available select it
				if ((_selectedPage == 0) && (_pages.Count > 0))
					SelectedPage = 1;
				else if (_selectedPage > _pages.Count)
					SelectedPage = _pages.Count;
				else
					SetControls();
			}
		}

		[Category("Commands")]
		[Description("The selected page.")]
		[DefaultValue(_SelectedPageDefault)]
		public int SelectedPage
		{
			get { return (_selectedPage); }
			set
			{
				int selectedPageNew = XInt.LimitToBounds(value, 0, _pages.Count);
				if (_selectedPage != selectedPageNew)
				{
					_selectedPage = selectedPageNew;
					SetControls();
					OnSelectedPageChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		[DefaultValue(_TerminalIsOpenDefault)]
		public bool TerminalIsOpen
		{
			set
			{
				_terminalIsOpen = value;
				SetControls();
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public void NextPage()
		{
			SelectedPage++;
		}

		public void PreviousPage()
		{
			SelectedPage--;
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
			if (!_isSettingControls)
				SelectedPage = comboBox_Pages.SelectedIndex + 1;
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private int SelectedPageIndex
		{
			get { return (_selectedPage - 1); }
			set { _selectedPage = value + 1;  }
		}

		private void SetControls()
		{
			_isSettingControls = true;

			pageButtons.TerminalIsOpen = _terminalIsOpen;

			if ((_pages != null) && (_pages.Count > 0) && (_selectedPage >= 1) && (_selectedPage <= _pages.Count))
			{
				pageButtons.Commands = _pages[SelectedPageIndex].Commands;

				button_PagePrevious.Enabled = (_selectedPage > 1);
				button_PageNext.Enabled = (_selectedPage < _pages.Count);

				label_Page.Enabled = true;
				label_Page.Text = "Page " + _selectedPage.ToString() + "/" + _pages.Count.ToString();

				comboBox_Pages.Enabled = true;
				comboBox_Pages.Items.Clear();
				foreach (PredefinedCommandPage p in _pages)
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

			_isSettingControls = false;
		}

		private void RequestSendCommand(int command)
		{
			OnSendCommandRequest(new PredefinedCommandEventArgs(_selectedPage, command));
		}

		private void RequestDefineCommand(int command)
		{
			OnDefineCommandRequest(new PredefinedCommandEventArgs(_selectedPage, command));
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
