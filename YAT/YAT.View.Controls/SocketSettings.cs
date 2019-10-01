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
// YAT Version 2.1.0
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
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("TcpClientAutoReconnectChanged")]
	public partial class SocketSettings : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const MKY.IO.Serial.Socket.SocketType SocketTypeDefault               = MKY.IO.Serial.Socket.SocketTypeEx.Default;

		private const MKY.IO.Serial.Socket.UdpServerSendMode UdpServerSendModeDefault = MKY.IO.Serial.Socket.SocketSettings.UdpServerSendModeDefault;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private MKY.IO.Serial.Socket.SocketType socketType               = SocketTypeDefault;

		private MKY.IO.Serial.AutoInterval tcpClientAutoReconnect        = MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectDefault;
		private MKY.IO.Serial.Socket.UdpServerSendMode udpServerSendMode = UdpServerSendModeDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the TcpClientAutoReconnect property is changed.")]
		public event EventHandler TcpClientAutoReconnectChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the UdpServerSendMode property is changed.")]
		public event EventHandler UdpServerSendModeChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SocketSettings()
		{
			InitializeComponent();

			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Socket")]
		[Description("Sets socket type.")]
		[DefaultValue(SocketTypeDefault)]
		public MKY.IO.Serial.Socket.SocketType SocketType
		{
			get
			{
				return (this.socketType);
			}
			set
			{
				if (this.socketType != value)
				{
					this.socketType = value;
					SetControls();
				}
			}
		}

		/// <remarks>
		/// Structs cannot be used with the designer.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MKY.IO.Serial.AutoInterval TcpClientAutoReconnect
		{
			get { return (this.tcpClientAutoReconnect); }
			set
			{
				if (this.tcpClientAutoReconnect != value)
				{
					this.tcpClientAutoReconnect = value;
					SetControls();
					OnTcpClientAutoReconnectChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Socket")]
		[Description("Sets UDP/IP server send mode.")]
		[DefaultValue(UdpServerSendModeDefault)]
		public MKY.IO.Serial.Socket.UdpServerSendMode UdpServerSendMode
		{
			get { return (this.udpServerSendMode); }
			set
			{
				if (this.udpServerSendMode != value)
				{
					this.udpServerSendMode = value;
					SetControls();
					OnUdpServerSendModeChanged(EventArgs.Empty);
				}
			}
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
		private void SocketSettings_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;

				SetControls();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void SocketSettings_EnabledChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_TcpClientAutoReconnect_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var ai = TcpClientAutoReconnect;
			ai.Enabled = checkBox_TcpClientAutoReconnect.Checked;
			TcpClientAutoReconnect = ai;
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_TcpClientAutoReconnectInterval_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int interval;
			if (int.TryParse(textBox_TcpClientAutoReconnectInterval.Text, out interval) && (interval >= MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectMinInterval))
			{
				var ai = TcpClientAutoReconnect;
				ai.Interval = interval;
				TcpClientAutoReconnect = ai;
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Reconnect interval must be at least " + MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectMinInterval + " ms!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void radioButton_UdpServerSendMode_MostRecent_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			UdpServerSendMode = MKY.IO.Serial.Socket.UdpServerSendMode.MostRecent;
		}

		private void radioButton_UdpServerSendMode_First_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			UdpServerSendMode = MKY.IO.Serial.Socket.UdpServerSendMode.First;
		}

		private void radioButton_UdpServerSendMode_None_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			UdpServerSendMode = MKY.IO.Serial.Socket.UdpServerSendMode.None;
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				// TCP/IP client:
				bool isTcp = ((MKY.IO.Serial.Socket.SocketTypeEx)this.socketType).IsTcp;
				bool enabledAndTcpClient = (Enabled && (this.socketType == MKY.IO.Serial.Socket.SocketType.TcpClient));
				bool autoReconnectEnabled = TcpClientAutoReconnect.Enabled;

				panel_Tcp.Visible = isTcp;

				checkBox_TcpClientAutoReconnect.Enabled = enabledAndTcpClient;
				checkBox_TcpClientAutoReconnect.Checked = autoReconnectEnabled;

				textBox_TcpClientAutoReconnectInterval.Enabled = (enabledAndTcpClient && autoReconnectEnabled);
				textBox_TcpClientAutoReconnectInterval.Text    = TcpClientAutoReconnect.Interval.ToString(CultureInfo.CurrentCulture);

				label_TcpClientAutoReconnectInterval.Enabled     = enabledAndTcpClient;
				label_TcpClientAutoReconnectIntervalUnit.Enabled = enabledAndTcpClient;

				// UDP/IP server:
				bool isUdp = ((MKY.IO.Serial.Socket.SocketTypeEx)this.socketType).IsUdp;
				bool enabledAndUdpServer = (Enabled && (this.socketType == MKY.IO.Serial.Socket.SocketType.UdpServer));

				panel_Udp.Visible = isUdp;

				label_UdpServerSendMode.Enabled                  = enabledAndUdpServer;
				radioButton_UdpServerSendMode_MostRecent.Enabled = enabledAndUdpServer;
				radioButton_UdpServerSendMode_MostRecent.Checked = (UdpServerSendMode == MKY.IO.Serial.Socket.UdpServerSendMode.MostRecent);
				radioButton_UdpServerSendMode_First.Enabled      = enabledAndUdpServer;
				radioButton_UdpServerSendMode_First.Checked      = (UdpServerSendMode == MKY.IO.Serial.Socket.UdpServerSendMode.First);
				radioButton_UdpServerSendMode_None.Enabled       = enabledAndUdpServer;
				radioButton_UdpServerSendMode_None.Checked       = (UdpServerSendMode == MKY.IO.Serial.Socket.UdpServerSendMode.None);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTcpClientAutoReconnectChanged(EventArgs e)
		{
			EventHelper.RaiseSync(TcpClientAutoReconnectChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnUdpServerSendModeChanged(EventArgs e)
		{
			EventHelper.RaiseSync(UdpServerSendModeChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
