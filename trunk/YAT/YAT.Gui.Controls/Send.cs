﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
using System.Windows.Forms;

using MKY;
using MKY.Recent;

using YAT.Model.Types;

#endregion

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendCommandRequest")]
	public partial class Send : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypefault;
		private const bool TerminalIsReadyToSendDefault = false;
		private const float SplitterRatioDefault = (float)0.75;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;
		private float splitterRatio = SplitterRatioDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler CommandChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the EditFocusState property is changed.")]
		public event EventHandler EditFocusStateChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler SendCommandRequest;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the FileCommand property is changed.")]
		public event EventHandler FileCommandChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending the file is requested.")]
		public event EventHandler SendFileCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Justification = "The initialization of 'terminalIsReadyToSend' is not unnecesary, it is based on a constant that contains a default value!")]
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

		/// <summary></summary>
		public virtual void SelectSendCommandInput()
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
		/// This property always returns a <see cref="Command"/> object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command Command
		{
			get { return (sendCommand.Command); }
			set { sendCommand.Command = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> RecentCommands
		{
			set { sendCommand.RecentCommands = value; }
		}

		/// <summary>
		/// Command always returns a Command object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command FileCommand
		{
			get { return (sendFile.FileCommand); }
			set	{ sendFile.FileCommand = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> RecentFileCommands
		{
			set { sendFile.RecentCommands = value; }
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			set
			{
				this.terminalType = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[DefaultValue(SendCommand.SendImmediatelyDefault)]
		public virtual bool SendCommandImmediately
		{
			get { return (sendCommand.SendImmediately); }
			set { sendCommand.SendImmediately = value; }
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

		/// <summary></summary>
		[DefaultValue(true)]
		public virtual bool CommandPanelIsVisible
		{
			get { return (!splitContainer_Send.Panel1Collapsed); }
			set { splitContainer_Send.Panel1Collapsed = !value;  }
		}

		/// <summary></summary>
		[DefaultValue(true)]
		public virtual bool FilePanelIsVisible
		{
			get { return (!splitContainer_Send.Panel2Collapsed); }
			set { splitContainer_Send.Panel2Collapsed = !value;  }
		}

		/// <summary></summary>
		[DefaultValue(SplitterRatioDefault)]
		public virtual float SplitterRatio
		{
			get { return (this.splitterRatio); }
			set
			{
				this.splitterRatio = value;
				SetControls();
			}
		}

		/// <summary></summary>
		public virtual bool EditIsActive
		{
			get { return (sendCommand.EditIsActive); }
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

		private void sendCommand_EditFocusStateChanged(object sender, EventArgs e)
		{
			OnEditFocusStateChanged(e);
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
			sendCommand.TerminalType          = this.terminalType;
			sendCommand.TerminalIsReadyToSend = this.terminalIsReadyToSend;
			sendCommand.SplitterRatio         = this.splitterRatio;

			sendFile.TerminalType          = this.terminalType;
			sendFile.TerminalIsReadyToSend = this.terminalIsReadyToSend;
			sendFile.SplitterRatio         = this.splitterRatio;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(CommandChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnEditFocusStateChanged(EventArgs e)
		{
			EventHelper.FireSync(EditFocusStateChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendCommandRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFileCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(FileCommandChanged, this, e);
		}

		/// <summary></summary>
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
