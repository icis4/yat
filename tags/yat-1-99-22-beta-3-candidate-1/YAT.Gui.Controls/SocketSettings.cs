//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
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
using MKY.IO.Serial;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("TcpClientAutoReconnectChanged")]
	public partial class SocketSettings : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketHostType _HostTypeDefault = SocketHostType.TcpAutoSocket;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private SocketHostType _hostType = _HostTypeDefault;
		private AutoRetry _tcpClientAutoReconnect = MKY.IO.Serial.SocketSettings.TcpClientAutoReconnectDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the TcpClientAutoReconnect property is changed.")]
		public event EventHandler TcpClientAutoReconnectChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SocketHostType HostType
		{
			set
			{
				if (_hostType != value)
				{
					_hostType = value;
					SetControls();
				}
			}
		}

		[Category("Socket")]
		[Description("Sets TCP client auto reconnect.")]
		public AutoRetry TcpClientAutoReconnect
		{
			get { return (_tcpClientAutoReconnect); }
			set
			{
				if (_tcpClientAutoReconnect != value)
				{
					_tcpClientAutoReconnect = value;
					SetControls();
					OnTcpClientAutoReconnectChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		private void SocketSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// Initially set controls and validate its contents where needed
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_TcpClientAutoReconnect_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				MKY.IO.Serial.AutoRetry ar = _tcpClientAutoReconnect;
				ar.Enabled = checkBox_TcpClientAutoReconnect.Checked;
				TcpClientAutoReconnect = ar;
			}
		}

		private void textBox_TcpClientAutoReconnectInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_TcpClientAutoReconnectInterval.Text, out interval) && (interval >= 100))
				{
					MKY.IO.Serial.AutoRetry ar = _tcpClientAutoReconnect;
					ar.Interval = interval;
					TcpClientAutoReconnect = ar;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Reconnect interval must be at least 100 ms!",
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
			_isSettingControls = true;

			bool isTcpClient = (_hostType == SocketHostType.TcpClient);

			bool autoReconnectEnabled;
			if (isTcpClient)
				autoReconnectEnabled = _tcpClientAutoReconnect.Enabled;
			else
				autoReconnectEnabled = false;

			checkBox_TcpClientAutoReconnect.Enabled = isTcpClient;
			checkBox_TcpClientAutoReconnect.Checked = autoReconnectEnabled;

			string autoReconnectIntervalText;
			if (isTcpClient)
				autoReconnectIntervalText = _tcpClientAutoReconnect.Interval.ToString();
			else
				autoReconnectIntervalText = "";

			textBox_TcpClientAutoReconnectInterval.Enabled = autoReconnectEnabled;
			textBox_TcpClientAutoReconnectInterval.Text = autoReconnectIntervalText;

			label_TcpClientAutoReconnectInterval.Enabled = isTcpClient;
			label_TcpClientAutoReconnectIntervalUnit.Enabled = isTcpClient;

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

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