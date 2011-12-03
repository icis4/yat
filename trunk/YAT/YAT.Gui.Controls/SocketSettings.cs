//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;

using MKY.Event;
using MKY.IO.Serial;
using MKY.Windows.Forms;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("TcpClientAutoReconnectChanged")]
	public partial class SocketSettings : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketHostType HostTypeDefault = SocketHostType.TcpAutoSocket;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private SocketHostType hostType = HostTypeDefault;
		private AutoRetry tcpClientAutoReconnect = MKY.IO.Serial.SocketSettings.TcpClientAutoReconnectDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the TcpClientAutoReconnect property is changed.")]
		public event EventHandler TcpClientAutoReconnectChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SocketSettings()
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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SocketHostType HostType
		{
			set
			{
				if (value != this.hostType)
				{
					this.hostType = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Socket")]
		[Description("Sets TCP client auto reconnect.")]
		public AutoRetry TcpClientAutoReconnect
		{
			get { return (this.tcpClientAutoReconnect); }
			set
			{
				if (value != this.tcpClientAutoReconnect)
				{
					this.tcpClientAutoReconnect = value;
					SetControls();
					OnTcpClientAutoReconnectChanged(new EventArgs());
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
			if (!this.isSettingControls)
				SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_TcpClientAutoReconnect_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				MKY.IO.Serial.AutoRetry ar = this.tcpClientAutoReconnect;
				ar.Enabled = checkBox_TcpClientAutoReconnect.Checked;
				TcpClientAutoReconnect = ar;
			}
		}

		private void textBox_TcpClientAutoReconnectInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_TcpClientAutoReconnectInterval.Text, out interval) && (interval >= MKY.IO.Serial.SocketSettings.TcpClientAutoReconnectMinimumInterval))
				{
					MKY.IO.Serial.AutoRetry ar = this.tcpClientAutoReconnect;
					ar.Interval = interval;
					TcpClientAutoReconnect = ar;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Reconnect interval must be at least " + MKY.IO.Serial.SocketSettings.TcpClientAutoReconnectMinimumInterval + " ms!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			bool enabledTcpClient = (Enabled && (this.hostType == SocketHostType.TcpClient));

			bool autoReconnectEnabled;
			if (enabledTcpClient)
				autoReconnectEnabled = this.tcpClientAutoReconnect.Enabled;
			else
				autoReconnectEnabled = false;

			checkBox_TcpClientAutoReconnect.Enabled = enabledTcpClient;
			checkBox_TcpClientAutoReconnect.Checked = autoReconnectEnabled;

			string autoReconnectIntervalText;
			if (enabledTcpClient)
				autoReconnectIntervalText = this.tcpClientAutoReconnect.Interval.ToString();
			else
				autoReconnectIntervalText = "";

			textBox_TcpClientAutoReconnectInterval.Enabled = autoReconnectEnabled;
			textBox_TcpClientAutoReconnectInterval.Text = autoReconnectIntervalText;

			label_TcpClientAutoReconnectInterval.Enabled = enabledTcpClient;
			label_TcpClientAutoReconnectIntervalUnit.Enabled = enabledTcpClient;

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTcpClientAutoReconnectChanged(EventArgs e)
		{
			EventHelper.FireSync(TcpClientAutoReconnectChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
