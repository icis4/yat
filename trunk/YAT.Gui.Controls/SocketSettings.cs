using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;
using MKY.Net.Sockets;

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

		private const HostType _HostTypeDefault = HostType.TcpAutoSocket;

		private static readonly Domain.TcpClientAutoReconnect _TcpClientAutoReconnectDefault = new YAT.Domain.TcpClientAutoReconnect(false, 500);

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private HostType _hostType = _HostTypeDefault;

		private Domain.TcpClientAutoReconnect _tcpClientAutoReconnect = _TcpClientAutoReconnectDefault;

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
		[DefaultValue(_HostTypeDefault)]
		public HostType HostType
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
		[Browsable(false)]
		public Domain.TcpClientAutoReconnect TcpClientAutoReconnect
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

				// initially set controls and validate its contents where needed
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
				Domain.TcpClientAutoReconnect ar = _tcpClientAutoReconnect;
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
					Domain.TcpClientAutoReconnect ar = _tcpClientAutoReconnect;
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

			bool enabled;
			if (_hostType == HostType.TcpClient)
				enabled = _tcpClientAutoReconnect.Enabled;
			else
				enabled = false;

			checkBox_TcpClientAutoReconnect.Checked = enabled;
			textBox_TcpClientAutoReconnectInterval.Enabled = enabled;
			textBox_TcpClientAutoReconnectInterval.Text = _tcpClientAutoReconnect.Interval.ToString();

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
