//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
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
using System.Net;
using System.Net.NetworkInformation;

using MKY.Utilities.Event;
using MKY.IO.Serial;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("HostNameOrAddressChanged")]
	public partial class SocketSelection : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private class HostItem
		{
			public readonly string HostNameOrAddress;
			public readonly string Description;

			public HostItem(string hostNameOrAddress)
			{
				HostNameOrAddress = hostNameOrAddress;
				Description = "";
			}

			public HostItem(string hostNameOrAddress, string description)
			{
				HostNameOrAddress = hostNameOrAddress;
				Description = description;
			}

			public override string ToString()
			{
				if ((Description == null) || (Description == ""))
					return (HostNameOrAddress);

				if (Description == _LocalHostNameOrAddressDefault)
					return (Description);

				return (HostNameOrAddress + " (" + Description + ")");
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketHostType _HostTypeDefault = SocketHostType.TcpAutoSocket;

		private const string _RemoteHostNameOrAddressDefault = MKY.IO.Serial.SocketSettings.DefaultRemoteHostName;
		private const int _RemotePortDefault                 = MKY.IO.Serial.SocketSettings.DefaultPort;

		private const string _LocalHostNameOrAddressDefault = MKY.IO.Serial.SocketSettings.DefaultLocalHostName;
		private const int _LocalTcpPortDefault              = MKY.IO.Serial.SocketSettings.DefaultPort;
		private const int _LocalUdpPortDefault              = MKY.IO.Serial.SocketSettings.DefaultPort + 1;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private MKY.IO.Serial.SocketHostType _hostType = _HostTypeDefault;

		private string _remoteHostNameOrAddress = _RemoteHostNameOrAddressDefault;
		private IPAddress _resolvedRemoteIPAddress = IPAddress.Loopback;
		private int _remotePort = _RemotePortDefault;

		private string _localHostNameOrAddress = _LocalHostNameOrAddressDefault;
		private IPAddress _resolvedLocalIPAddress = IPAddress.Any;
		private int _localTcpPort = _LocalTcpPortDefault;
		private int _localUdpPort = _LocalUdpPortDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the RemoteHostNameOrAddress property is changed.")]
		public event EventHandler RemoteHostNameOrAddressChanged;

		[Category("Property Changed")]
		[Description("Event raised when the RemotePort property is changed.")]
		public event EventHandler RemotePortChanged;

		[Category("Property Changed")]
		[Description("Event raised when the LocalHostNameOrAddress property is changed.")]
		public event EventHandler LocalHostNameOrAddressChanged;

		[Category("Property Changed")]
		[Description("Event raised when the LocalTcpPort property is changed.")]
		public event EventHandler LocalTcpPortChanged;

		[Category("Property Changed")]
		[Description("Event raised when the LocalUdpPort property is changed.")]
		public event EventHandler LocalUdpPortChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public SocketSelection()
		{
			InitializeComponent();

			InitializeControls();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Browsable(false)]
		[DefaultValue(_HostTypeDefault)]
		public MKY.IO.Serial.SocketHostType HostType
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
		[Description("The remote host name or address.")]
		[DefaultValue(_RemoteHostNameOrAddressDefault)]
		public string RemoteHostNameOrAddress
		{
			get { return (_remoteHostNameOrAddress); }
			set
			{
				if (_remoteHostNameOrAddress != value)
				{
					_remoteHostNameOrAddress = value;
					SetControls();
					OnRemoteHostNameOrAddressChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		public IPAddress ResolvedRemoteIPAddress
		{
			get { return (_resolvedRemoteIPAddress); }
		}

		[Category("Socket")]
		[Description("The remote TCP or UDP port.")]
		[DefaultValue(_RemotePortDefault)]
		public int RemotePort
		{
			get { return (_remotePort); }
			set
			{
				if (_remotePort != value)
				{
					_remotePort = value;
					SetControls();
					OnRemotePortChanged(new EventArgs());
				}
			}
		}

		[Category("Socket")]
		[Description("The local host name or address.")]
		[DefaultValue(_LocalHostNameOrAddressDefault)]
		public string LocalHostNameOrAddress
		{
			get { return (_localHostNameOrAddress); }
			set
			{
				if (_localHostNameOrAddress != value)
				{
					_localHostNameOrAddress = value;
					SetControls();
					OnLocalHostNameOrAddressChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		public IPAddress ResolvedLocalIPAddress
		{
			get { return (_resolvedLocalIPAddress); }
		}

		[Category("Socket")]
		[Description("The local TCP port.")]
		[DefaultValue(_LocalTcpPortDefault)]
		public int LocalTcpPort
		{
			get { return (_localTcpPort); }
			set
			{
				if (_localTcpPort != value)
				{
					_localTcpPort = value;
					SetControls();
					OnLocalTcpPortChanged(new EventArgs());
				}
			}
		}

		[Category("Socket")]
		[Description("The local UDP port.")]
		[DefaultValue(_LocalUdpPortDefault)]
		public int LocalUdpPort
		{
			get { return (_localUdpPort); }
			set
			{
				if (_localUdpPort != value)
				{
					_localUdpPort = value;
					SetControls();
					OnLocalUdpPortChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_RemoteHostNameOrAddress_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				string nameOrAddress;
				HostItem hi = comboBox_RemoteHostNameOrAddress.SelectedItem as HostItem;
				if (hi != null)
					nameOrAddress = hi.HostNameOrAddress;
				else
					nameOrAddress = comboBox_RemoteHostNameOrAddress.Text;

				IPAddress ipAddress;
				if (IPAddress.TryParse(nameOrAddress, out ipAddress))
				{
					_resolvedRemoteIPAddress = ipAddress;
					RemoteHostNameOrAddress = nameOrAddress;
				}
				else
				{
					try
					{
						IPAddress[] ipAddresses;
						ipAddresses = Dns.GetHostAddresses(nameOrAddress);
						_resolvedRemoteIPAddress = ipAddresses[0];
						RemoteHostNameOrAddress = nameOrAddress;
					}
					catch (Exception ex)
					{
						MessageBox.Show
							(
							this,
							"Remote host address is invalid:" + Environment.NewLine + Environment.NewLine +
							ex.Message,
							"Invalid Input",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
							);
						e.Cancel = true;
					}
				}
			}
		}

		private void textBox_RemotePort_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int port;
				if (int.TryParse(textBox_RemotePort.Text, out port) &&
					(port >= System.Net.IPEndPoint.MinPort) && (port <= System.Net.IPEndPoint.MaxPort))
				{
					RemotePort = port;

					// also set local port to same number
					if ((_hostType == SocketHostType.TcpClient) || (_hostType == SocketHostType.TcpAutoSocket) || (_hostType == SocketHostType.Udp))
					{
						LocalTcpPort = port;

						if (port < System.Net.IPEndPoint.MaxPort)
							LocalUdpPort = port + 1;
						else
							LocalUdpPort = System.Net.IPEndPoint.MaxPort - 1;
					}
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Remote port is invalid, valid values are numbers from " +
							System.Net.IPEndPoint.MinPort.ToString() + " to " +
							System.Net.IPEndPoint.MaxPort.ToString() + ".",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		private void comboBox_LocalHostNameOrAddress_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				HostItem hi = comboBox_LocalHostNameOrAddress.SelectedItem as HostItem;
				IPAddress ipAddress = IPAddress.Parse(hi.HostNameOrAddress);
				_resolvedLocalIPAddress = ipAddress;
			}
		}

		private void textBox_LocalPort_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int port;
				if (int.TryParse(textBox_LocalPort.Text, out port) &&
					(port >= System.Net.IPEndPoint.MinPort) && (port <= System.Net.IPEndPoint.MaxPort))
				{
					LocalTcpPort = port;
					LocalUdpPort = port;

					// also set remote port to same number
					if (_hostType == SocketHostType.TcpServer)
					{
						RemotePort = port;
					}
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Local port is invalid, valid values are numbers from " +
							System.Net.IPEndPoint.MinPort.ToString() + " to " +
							System.Net.IPEndPoint.MaxPort.ToString() + ".",
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

		private void InitializeControls()
		{
			_isSettingControls = true;

			// remote host
			comboBox_RemoteHostNameOrAddress.Items.Clear();
			comboBox_RemoteHostNameOrAddress.Items.Add(new HostItem(_RemoteHostNameOrAddressDefault));
			comboBox_RemoteHostNameOrAddress.Items.Add(new HostItem("127.0.0.1", "IPv4 localhost"));
			comboBox_RemoteHostNameOrAddress.Items.Add(new HostItem("::1", "IPv6 localhost"));

			// local host/interface
			comboBox_LocalHostNameOrAddress.Items.Clear();
			comboBox_LocalHostNameOrAddress.Items.Add(new HostItem(IPAddress.Any.ToString(), _LocalHostNameOrAddressDefault));
			comboBox_LocalHostNameOrAddress.Items.Add(new HostItem("127.0.0.1", "IPv4 loopback"));
			comboBox_LocalHostNameOrAddress.Items.Add(new HostItem("::1", "IPv6 loopback"));

			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			foreach (IPAddress address in Dns.GetHostAddresses(""))
			{
				string description = "";
				foreach (NetworkInterface nic in nics)
				{
					if (nic.OperationalStatus == OperationalStatus.Up)
					{
						foreach (UnicastIPAddressInformation ai in nic.GetIPProperties().UnicastAddresses)
						{
							if (address.Equals(ai.Address))
							{
								description = nic.Description;
								break;
							}
						}

						if (description != "")
							break;
					}
				}
				comboBox_LocalHostNameOrAddress.Items.Add(new HostItem(address.ToString(), description));
			}

			_isSettingControls = false;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			// remote host address
			if ((_hostType == SocketHostType.TcpClient) || (_hostType == SocketHostType.TcpAutoSocket) || (_hostType == SocketHostType.Udp))
			{
				comboBox_RemoteHostNameOrAddress.Enabled = true;
				comboBox_RemoteHostNameOrAddress.Text = _remoteHostNameOrAddress;
			}
			else
			{
				comboBox_RemoteHostNameOrAddress.Enabled = false;
				comboBox_RemoteHostNameOrAddress.Text = "";
			}

			// remote port label
			if (_hostType == SocketHostType.Udp)
				label_RemotePort.Text = "Remote UDP port:";
			else
				label_RemotePort.Text = "Remote TCP port:";

			// remote port
			if ((_hostType == SocketHostType.TcpClient) || (_hostType == SocketHostType.TcpAutoSocket) || (_hostType == SocketHostType.Udp))
			{
				textBox_RemotePort.Enabled = true;
				textBox_RemotePort.Text = _remotePort.ToString();
			}
			else
			{
				textBox_RemotePort.Enabled = false;
				textBox_RemotePort.Text = "";
			}

			// local host address
			if (_hostType != SocketHostType.Unknown)
			{
				comboBox_LocalHostNameOrAddress.Enabled = true;
				comboBox_LocalHostNameOrAddress.Text = _localHostNameOrAddress;
			}
			else
			{
				comboBox_LocalHostNameOrAddress.Enabled = false;
				comboBox_LocalHostNameOrAddress.SelectedIndex = -1;
			}

			// local port label
			if (_hostType == SocketHostType.Udp)
				label_LocalPort.Text = "Local UDP port:";
			else
				label_LocalPort.Text = "Local TCP port:";

			// local port
			if ((_hostType == SocketHostType.TcpServer) || (_hostType == SocketHostType.TcpAutoSocket))
			{
				textBox_LocalPort.Enabled = true;
				textBox_LocalPort.Text = _localTcpPort.ToString();
			}
			else if (_hostType == SocketHostType.Udp)
			{
				textBox_LocalPort.Enabled = true;
				textBox_LocalPort.Text = _localUdpPort.ToString();
			}
			else
			{
				textBox_LocalPort.Enabled = false;
				textBox_LocalPort.Text = "";
			}

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnRemoteHostNameOrAddressChanged(EventArgs e)
		{
			EventHelper.FireSync(RemoteHostNameOrAddressChanged, this, e);
		}

		protected virtual void OnRemotePortChanged(EventArgs e)
		{
			EventHelper.FireSync(RemotePortChanged, this, e);
		}

		protected virtual void OnLocalHostNameOrAddressChanged(EventArgs e)
		{
			EventHelper.FireSync(LocalHostNameOrAddressChanged, this, e);
		}

		protected virtual void OnLocalTcpPortChanged(EventArgs e)
		{
			EventHelper.FireSync(LocalTcpPortChanged, this, e);
		}

		protected virtual void OnLocalUdpPortChanged(EventArgs e)
		{
			EventHelper.FireSync(LocalUdpPortChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
