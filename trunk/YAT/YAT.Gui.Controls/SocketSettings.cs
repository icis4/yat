//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

#endregion

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DefaultEvent("TcpClientAutoReconnectChanged")]
	public partial class SocketSettings : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const MKY.IO.Serial.Socket.SocketHostType HostTypeDefault = MKY.IO.Serial.Socket.SocketHostType.TcpAutoSocket;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private MKY.IO.Serial.Socket.SocketHostType hostType = HostTypeDefault;
		private MKY.IO.Serial.AutoRetry tcpClientAutoReconnect = MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectDefault;

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

			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MKY.IO.Serial.Socket.SocketHostType HostType
		{
			set
			{
				if (this.hostType != value)
				{
					this.hostType = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Socket")]
		[Description("Sets TCP/IP Client auto reconnect.")]
		public MKY.IO.Serial.AutoRetry TcpClientAutoReconnect
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

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_TcpClientAutoReconnectInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_TcpClientAutoReconnectInterval.Text, out interval) && (interval >= MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectMinimumInterval))
				{
					MKY.IO.Serial.AutoRetry ar = this.tcpClientAutoReconnect;
					ar.Interval = interval;
					TcpClientAutoReconnect = ar;
				}
				else
				{
					MessageBoxEx.Show
					(
						this,
						"Reconnect interval must be at least " + MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectMinimumInterval + " ms!",
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

			bool enabledTcpClient = (Enabled && (this.hostType == MKY.IO.Serial.Socket.SocketHostType.TcpClient));

			bool autoReconnectEnabled;
			if (enabledTcpClient)
				autoReconnectEnabled = this.tcpClientAutoReconnect.Enabled;
			else
				autoReconnectEnabled = false;

			checkBox_TcpClientAutoReconnect.Enabled = enabledTcpClient;
			checkBox_TcpClientAutoReconnect.Checked = autoReconnectEnabled;

			string autoReconnectIntervalText;
			if (enabledTcpClient)
				autoReconnectIntervalText = this.tcpClientAutoReconnect.Interval.ToString(CultureInfo.CurrentCulture);
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
