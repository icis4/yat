//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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

using MKY.Utilities.Event;
using MKY.Utilities.Recent;

using YAT.Model.Types;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class Send : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType _TerminalTypeDefault = Domain.TerminalType.Text;
		private const bool _TerminalIsOpenDefault = false;
		private const float _SplitterRatioDefault = (float)0.75;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Domain.TerminalType _terminalType = _TerminalTypeDefault;
		private bool _terminalIsOpen = _TerminalIsOpenDefault;
        private float _splitterRatio = _SplitterRatioDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler CommandChanged;

		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler SendCommandRequest;

		[Category("Property Changed")]
		[Description("Event raised when the FileCommand property is changed.")]
		public event EventHandler FileCommandChanged;

		[Category("Action")]
		[Description("Event raised when sending the file is requested.")]
		public event EventHandler SendFileCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Send()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public void SelectSendCommandInput()
		{
			sendCommand.Select();
			sendCommand.SelectInput();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

        /// <summary>
		/// Command always returns a Command object, it never returns <c>null</c>.
        /// </summary>
        [Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command Command
        {
            get { return (sendCommand.Command); }
            set { sendCommand.Command = value;  }
        }

        [Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RecentItemCollection<Command> RecentCommands
        {
            set { sendCommand.RecentCommands = value;  }
        }

        /// <summary>
		/// Command always returns a Command object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command FileCommand
		{
			get { return (sendFile.FileCommand); }
			set	{ sendFile.FileCommand = value;  }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Domain.TerminalType TerminalType
		{
			set
			{
				_terminalType = value;
				SetControls();
			}
		}

        [Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TerminalIsOpen
        {
            set
            {
                _terminalIsOpen = value;
                SetControls();
            }
        }

        [DefaultValue(true)]
        public bool CommandPanelIsVisible
        {
            get { return (!splitContainer_Send.Panel1Collapsed); }
            set { splitContainer_Send.Panel1Collapsed = !value;  }
        }

        [DefaultValue(true)]
        public bool FilePanelIsVisible
        {
            get { return (!splitContainer_Send.Panel2Collapsed); }
            set { splitContainer_Send.Panel2Collapsed = !value;  }
        }

        [DefaultValue(_SplitterRatioDefault)]
        public float SplitterRatio
        {
            get { return (_splitterRatio); }
            set
            {
                _splitterRatio = value;
                SetControls();
            }
        }

        #endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		#region Controls Event Handlers > Send Command
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send Command
		//------------------------------------------------------------------------------------------

		private void sendCommand_CommandChanged(object sender, EventArgs e)
		{
			OnCommandChanged(e);
		}

		private void sendCommand_SendCommandRequest(object sender, EventArgs e)
		{
			OnSendCommandRequest(e);
		}

		#endregion

		#region Controls Event Handlers > Send File
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send File
		//------------------------------------------------------------------------------------------

		private void sendFile_FileCommandChanged(object sender, EventArgs e)
		{
			OnFileCommandChanged(e);
		}

		private void sendFile_SendFileCommandRequest(object sender, EventArgs e)
		{
			OnSendFileCommandRequest(e);
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			sendCommand.TerminalIsOpen = _terminalIsOpen;
            sendCommand.SplitterRatio = _splitterRatio;

			sendFile.TerminalType = _terminalType;
			sendFile.TerminalIsOpen = _terminalIsOpen;
            sendFile.SplitterRatio = _splitterRatio;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(CommandChanged, this, e);
		}

		protected virtual void OnSendCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendCommandRequest, this, e);
		}

		protected virtual void OnFileCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(FileCommandChanged, this, e);
		}

		protected virtual void OnSendFileCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendFileCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
