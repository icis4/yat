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
using System.Net;
using System.Net.NetworkInformation;

using MKY.Utilities.Event;
using MKY.Utilities.Net;
using MKY.IO.Serial;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("HostNameOrAddressChanged")]
	public partial class SocketSelection : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketHostType _DefaultHostType                     = SocketHostType.TcpAutoSocket;

		private static readonly XIPHost _DefaultRemoteHost                = MKY.IO.Serial.SocketSettings.DefaultRemoteHost;
		private static readonly IPAddress _DefaultResolvedRemoteIPAddress = MKY.IO.Serial.SocketSettings.DefaultResolvedRemoteIPAddress;
		private const int _DefaultRemotePort                              = MKY.IO.Serial.SocketSettings.DefaultRemotePort;

		private static readonly XNetworkInterface _DefaultLocalInterface  = MKY.IO.Serial.SocketSettings.DefaultLocalInterface;
		private static readonly IPAddress _DefaultResolvedLocalIPAddress  = MKY.IO.Serial.SocketSettings.DefaultResolvedLocalIPAddress;
		private const int _DefaultLocalTcpPort                            = MKY.IO.Serial.SocketSettings.DefaultLocalTcpPort;
		private const int _DefaultLocalUdpPort                            = MKY.IO.Serial.SocketSettings.DefaultLocalUdpPort;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private SocketHostType _hostType = _DefaultHostType;

		private XIPHost _remoteHost                = _DefaultRemoteHost;
		private IPAddress _resolvedRemoteIPAddress = _DefaultResolvedRemoteIPAddress;
		private int _remotePort                    = _DefaultRemotePort;

		private XNetworkInterface _localInterface  = _DefaultLocalInterface;
		private IPAddress _resolvedLocalIPAddress  = _DefaultResolvedLocalIPAddress;
		private int _localTcpPort                  = _DefaultLocalTcpPort;
		private int _localUdpPort                  = _DefaultLocalUdpPort;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the RemoteHost property is changed.")]
		public event EventHandler RemoteHostChanged;

		[Category("Property Changed")]
		[Description("Event raised when the RemotePort property is changed.")]
		public event EventHandler RemotePortChanged;

		[Category("Property Changed")]
		[Description("Event raised when the LocalInterface property is changed.")]
		public event EventHandler LocalInterfaceChanged;

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
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XIPHost RemoteHost
		{
			get { return (_remoteHost); }
			set
			{
				if (_remoteHost != value)
				{
					_remoteHost = value;
					SetControls();
					OnRemoteHostChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IPAddress ResolvedRemoteIPAddress
		{
			get { return (_resolvedRemoteIPAddress); }
		}

		[Category("Socket")]
		[Description("The remote TCP or UDP port.")]
		[DefaultValue(_DefaultRemotePort)]
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string LocalInterface
		{
			get { return (_localInterface); }
			set
			{
				if (_localInterface != value)
				{
					_localInterface = value;
					SetControls();
					OnLocalInterfaceChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IPAddress ResolvedLocalIPAddress
		{
			get { return (_resolvedLocalIPAddress); }
		}

		[Category("Socket")]
		[Description("The local TCP port.")]
		[DefaultValue(_DefaultLocalTcpPort)]
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
		[DefaultValue(_DefaultLocalUdpPort)]
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

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

        /// <summary>
        /// Startup flag only used in the following event handler.
        /// </summary>
        private bool _isStartingUp = true;

        /// <summary>
        /// Initially set controls and validate its contents where needed.
        /// </summary>
        private void SocketSelection_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				InitializeControls();
				SetLocalInterfaceList();
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_RemoteHost_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				// \attention
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				XIPHost host = comboBox_RemoteHost.SelectedItem as XIPHost;
				if ((host != null) && (host.IPAddress != IPAddress.None) &&
					(host.ToString() == comboBox_RemoteHost.Text))
				{
					RemoteHost = host;
					_resolvedRemoteIPAddress = host.IPAddress;
				}
				else
				{
					IPAddress ipAddress;
					string nameOrAddress;
					nameOrAddress = comboBox_RemoteHost.Text;

					if (IPAddress.TryParse(nameOrAddress, out ipAddress))
					{
						_resolvedRemoteIPAddress = ipAddress;
						RemoteHost = new XIPHost(_resolvedRemoteIPAddress);
					}
					else
					{
						try
						{
							IPAddress[] ipAddresses;
							ipAddresses = Dns.GetHostAddresses(nameOrAddress);
							_resolvedRemoteIPAddress = ipAddresses[0];
							RemoteHost = new XIPHost(_resolvedRemoteIPAddress);
						}
						catch (Exception ex)
						{
							MessageBox.Show
								(
								this,
								"Remote host name or address is invalid:" + Environment.NewLine + Environment.NewLine +
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

					// Also set local port to same number
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

		private void comboBox_LocalInterface_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				_localInterface = comboBox_LocalInterface.SelectedItem as XNetworkInterface;
				_resolvedLocalIPAddress = _localInterface.IPAddress;
			}
		}

		private void button_RefreshLocalInterfaces_Click(object sender, EventArgs e)
		{
			SetLocalInterfaceList();
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

					// Also set remote port to same number
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

			// Remote host
			comboBox_RemoteHost.Items.Clear();
			comboBox_RemoteHost.Items.AddRange(XIPHost.GetItems());

			_isSettingControls = false;
		}

		private void SetLocalInterfaceList()
		{
			if (Enabled)
			{
				_isSettingControls = true;

				XNetworkInterface old = comboBox_LocalInterface.SelectedItem as XNetworkInterface;

				NetworkInterfaceCollection localInterfaces = new NetworkInterfaceCollection();
				localInterfaces.FillWithAvailableInterfaces();

				comboBox_LocalInterface.Items.Clear();
				comboBox_LocalInterface.Items.AddRange(localInterfaces.ToArray());

				if (comboBox_LocalInterface.Items.Count > 0)
				{
					if ((_localInterface != null) && (localInterfaces.Contains(_localInterface)))
						comboBox_LocalInterface.SelectedItem = _localInterface;
					else if ((old != null) && (localInterfaces.Contains(old)))
						comboBox_LocalInterface.SelectedItem = old;
					else
						comboBox_LocalInterface.SelectedIndex = 0;

					// Set property instead of member to ensure that changed event is fired
					LocalInterface = comboBox_LocalInterface.SelectedItem as XNetworkInterface;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"No local network interfaces available, check network system settings.",
						"No interfaces",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
				}

				_isSettingControls = false;
			}
		}

		private void SetControls()
		{
			_isSettingControls = true;

			// Remote host address
			if ((_hostType == SocketHostType.TcpClient) || (_hostType == SocketHostType.TcpAutoSocket) || (_hostType == SocketHostType.Udp))
			{
				comboBox_RemoteHost.Enabled = true;
				comboBox_RemoteHost.Text = _remoteHost;
			}
			else
			{
				comboBox_RemoteHost.Enabled = false;
				comboBox_RemoteHost.Text = "";
			}

			// Remote port label
			if (_hostType == SocketHostType.Udp)
				label_RemotePort.Text = "Remote UDP port:";
			else
				label_RemotePort.Text = "Remote TCP port:";

			// Remote port
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

			// Local host address
			if (_hostType != SocketHostType.Unknown)
			{
				comboBox_LocalInterface.Enabled = true;
				comboBox_LocalInterface.Text = _localInterface;
			}
			else
			{
				comboBox_LocalInterface.Enabled = false;
				comboBox_LocalInterface.SelectedIndex = -1;
			}

			// Local port label
			if (_hostType == SocketHostType.Udp)
				label_LocalPort.Text = "Local UDP port:";
			else
				label_LocalPort.Text = "Local TCP port:";

			// Local port
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

		protected virtual void OnRemoteHostChanged(EventArgs e)
		{
			EventHelper.FireSync(RemoteHostChanged, this, e);
		}

		protected virtual void OnRemotePortChanged(EventArgs e)
		{
			EventHelper.FireSync(RemotePortChanged, this, e);
		}

		protected virtual void OnLocalInterfaceChanged(EventArgs e)
		{
			EventHelper.FireSync(LocalInterfaceChanged, this, e);
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
