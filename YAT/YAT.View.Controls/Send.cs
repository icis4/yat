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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Windows.Forms;

using MKY;
using MKY.Collections.Specialized;
using MKY.Windows.Forms;

using YAT.Model.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("SendCommandRequest")]
	public partial class Send : UserControl, IOnFormDeactivateWorkaround
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary>
		/// The designed height when both (text and file) panels are visible.
		/// </summary>
		public const int DesignedFullHeight = 93;

		/// <summary>
		/// The designed height when one (text or file) panel is visible.
		/// </summary>
		public const int DesignedHalfHeight = 46;

		private const int SendSplitterDistanceDefault = 353; // Designer requires that this is a constant.
		                                                     // Set same value as underlying controls (less the left margin of 3).
		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int sendSplitterDistance = SendSplitterDistanceDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler TextCommandChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the TextFocused property is changed.")]
		public event EventHandler TextFocusedChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending the command is requested.")]
		public event EventHandler<SendTextOptionEventArgs> SendTextCommandRequest;

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
		public virtual void StandbyInUserInput()
		{
			sendText.StandbyInUserInput();
			sendText.Select();
		}

		/// <summary></summary>
		public virtual void SelectAndPrepareUserInput()
		{
			sendText.PrepareUserInput();
			sendText.Select();
		}

		/// <summary></summary>
		public virtual void ValidateSendTextInput()
		{
			sendText.ValidateInput();
		}

		/// <summary></summary>
		public virtual void NotifyKeyDown(KeyEventArgs e)
		{
			if (sendText != null)
			{
				if (sendText.ContainsFocus)
					sendText.NotifyKeyDown(e); // Somewhat ugly workaround to handle key events...
			}
		}

		/// <summary></summary>
		public virtual void NotifyKeyUp(KeyEventArgs e)
		{
			if (sendText != null)
			{
				if (sendText.ContainsFocus)
					sendText.NotifyKeyUp(e); // Somewhat ugly workaround to handle key events...
			}
		}

		/// <remarks>See remarks in <see cref="MKY.Windows.Forms.ComboBoxEx"/>.</remarks>
		public virtual void OnFormDeactivateWorkaround()
		{
			sendText.OnFormDeactivateWorkaround();
		////sendFile doesn't contain a ComboBoxEx
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// This property always returns a <see cref="Model.Types.Command"/> object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command TextCommand
		{
			get { return (sendText.Command); }
			set { sendText.Command = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> RecentTextCommands
		{
			set { sendText.Recent = value; }
		}

		/// <summary>
		/// Command always returns a <see cref="Model.Types.Command"/> object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command FileCommand
		{
			get { return (sendFile.Command); }
			set { sendFile.Command = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> RecentFileCommands
		{
			set { sendFile.Recent = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			set
			{
				sendText.TerminalType = value;
				sendFile.TerminalType = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[DefaultValue(Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault)]
		public virtual bool UseExplicitDefaultRadix
		{
			set
			{
				sendText.UseExplicitDefaultRadix = value;
				sendFile.UseExplicitDefaultRadix = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.Parser.Mode ParseModeForText
		{
			set
			{
				sendText.ParseMode = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[DefaultValue(SendText.SendImmediatelyDefault)]
		public virtual bool SendTextImmediately
		{
			set
			{
				sendText.SendImmediately = value;
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
				sendFile.RootDirectory = value;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSendForSomeTime
		{
			set
			{
				sendText.TerminalIsReadyToSendForSomeTime = value;
				sendFile.TerminalIsReadyToSendForSomeTime = value;
			}
		}

		/// <remarks>
		/// No explicit 'Default' constant as the default is given by <see cref="SplitContainer"/>.
		/// </remarks>
		[DefaultValue(true)]
		public virtual bool TextPanelIsVisible
		{
			get { return (!splitContainer_Send.Panel1Collapsed); }
			set { splitContainer_Send.Panel1Collapsed = !value;  }
		}

		/// <remarks>
		/// No explicit 'Default' constant as the default is given by <see cref="SplitContainer"/>.
		/// </remarks>
		[DefaultValue(true)]
		public virtual bool FilePanelIsVisible
		{
			get { return (!splitContainer_Send.Panel2Collapsed); }
			set { splitContainer_Send.Panel2Collapsed = !value;  }
		}

		/// <summary></summary>
		[DefaultValue(SendSplitterDistanceDefault)]
		public virtual int SendSplitterDistance
		{
			get { return (this.sendSplitterDistance); }
			set
			{
				// Do not check if (this.splitterDistance != value) because the distance (position)
				// will be limited to the control's width, and that may change AFTER the distance
				// has been set.

				this.sendSplitterDistance = value;
				SetSendSplitterControls();
			}
		}

		/// <summary></summary>
		public virtual bool TextFocused
		{
			get { return (sendText.TextFocused); }
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		#region Controls Event Handlers > Send Text
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send Text
		//------------------------------------------------------------------------------------------

		private void sendText_CommandChanged(object sender, EventArgs e)
		{
			OnTextCommandChanged(e);
		}

		private void sendText_TextFocusedChanged(object sender, EventArgs e)
		{
			OnTextFocusedChanged(e);
		}

		private void sendText_SendCommandRequest(object sender, SendTextOptionEventArgs e)
		{
			OnSendTextCommandRequest(e);
		}

		#endregion

		#region Controls Event Handlers > Send File
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send File
		//------------------------------------------------------------------------------------------

		private void sendFile_CommandChanged(object sender, EventArgs e)
		{
			OnFileCommandChanged(e);
		}

		private void sendFile_SendCommandRequest(object sender, EventArgs e)
		{
			OnSendFileCommandRequest(e);
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void SetControls()
		{
			SetSendSplitterControls();
		}

		private void SetSendSplitterControls()
		{
			sendText.SendSplitterDistance = (this.sendSplitterDistance - sendText.Left);
			sendFile.SendSplitterDistance = (this.sendSplitterDistance - sendFile.Left);
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTextCommandChanged(EventArgs e)
		{
			EventHelper.RaiseSync(TextCommandChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnTextFocusedChanged(EventArgs e)
		{
			EventHelper.RaiseSync(TextFocusedChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendTextCommandRequest(SendTextOptionEventArgs e)
		{
			EventHelper.RaiseSync(SendTextCommandRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFileCommandChanged(EventArgs e)
		{
			EventHelper.RaiseSync(FileCommandChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendFileCommandRequest(EventArgs e)
		{
			EventHelper.RaiseSync(SendFileCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
