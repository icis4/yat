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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Windows.Forms;

using MKY.IO.Serial;
using MKY.Utilities.Event;
using MKY.Utilities.Net;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("HostNameOrAddressChanged")]
	public partial class SocketSelection : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketHostType DefaultHostType                     = SocketHostType.TcpAutoSocket;

		private static readonly XIPHost DefaultRemoteHost                = MKY.IO.Serial.SocketSettings.DefaultRemoteHost;
		private static readonly IPAddress DefaultResolvedRemoteIPAddress = MKY.IO.Serial.SocketSettings.DefaultResolvedRemoteIPAddress;
		private const int DefaultRemotePort                              = MKY.IO.Serial.SocketSettings.DefaultRemotePort;

		private static readonly IPNetworkInterface DefaultLocalInterface  = MKY.IO.Serial.SocketSettings.DefaultLocalInterface;
		private static readonly IPAddress DefaultResolvedLocalIPAddress  = MKY.IO.Serial.SocketSettings.DefaultResolvedLocalIPAddress;
		private const int DefaultLocalTcpPort                            = MKY.IO.Serial.SocketSettings.DefaultLocalTcpPort;
		private const int DefaultLocalUdpPort                            = MKY.IO.Serial.SocketSettings.DefaultLocalUdpPort;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		private SocketHostType hostType = DefaultHostType;

		private XIPHost remoteHost                = DefaultRemoteHost;
		private IPAddress resolvedRemoteIPAddress = DefaultResolvedRemoteIPAddress;
		private int remotePort                    = DefaultRemotePort;

		private IPNetworkInterface localInterface  = DefaultLocalInterface;
		private IPAddress resolvedLocalIPAddress  = DefaultResolvedLocalIPAddress;
		private int localTcpPort                  = DefaultLocalTcpPort;
		private int localUdpPort                  = DefaultLocalUdpPort;

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
		public virtual SocketHostType HostType
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual XIPHost RemoteHost
		{
			get { return (this.remoteHost); }
			set
			{
				if (value != this.remoteHost)
				{
					this.remoteHost = value;
					SetControls();
					OnRemoteHostChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPAddress ResolvedRemoteIPAddress
		{
			get { return (this.resolvedRemoteIPAddress); }
		}

		[Category("Socket")]
		[Description("The remote TCP or UDP port.")]
		[DefaultValue(DefaultRemotePort)]
		public virtual int RemotePort
		{
			get { return (this.remotePort); }
			set
			{
				if (value != this.remotePort)
				{
					this.remotePort = value;
					SetControls();
					OnRemotePortChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPNetworkInterface LocalInterface
		{
			get { return (this.localInterface); }
			set
			{
				if (value != this.localInterface)
				{
					this.localInterface = value;
					SetControls();
					OnLocalInterfaceChanged(new EventArgs());
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPAddress ResolvedLocalIPAddress
		{
			get { return (this.resolvedLocalIPAddress); }
		}

		[Category("Socket")]
		[Description("The local TCP port.")]
		[DefaultValue(DefaultLocalTcpPort)]
		public virtual int LocalTcpPort
		{
			get { return (this.localTcpPort); }
			set
			{
				if (value != this.localTcpPort)
				{
					this.localTcpPort = value;
					SetControls();
					OnLocalTcpPortChanged(new EventArgs());
				}
			}
		}

		[Category("Socket")]
		[Description("The local UDP port.")]
		[DefaultValue(DefaultLocalUdpPort)]
		public virtual int LocalUdpPort
		{
			get { return (this.localUdpPort); }
			set
			{
				if (value != this.localUdpPort)
				{
					this.localUdpPort = value;
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
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void SocketSelection_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;

				InitializeControls();
				SetLocalInterfaceList();
				SetControls();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void SocketSelection_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Emphasize line breaks.")]
		private void comboBox_RemoteHost_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				// \attention
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				XIPHost host = comboBox_RemoteHost.SelectedItem as XIPHost;
				if ((host != null) && (host.IPAddress != IPAddress.None) &&
					(host.ToString() == comboBox_RemoteHost.Text))
				{
					RemoteHost = host;
					this.resolvedRemoteIPAddress = RemoteHost.IPAddress;
				}
				else
				{
					IPAddress ipAddress;
					string nameOrAddress;
					nameOrAddress = comboBox_RemoteHost.Text;

					if (IPAddress.TryParse(nameOrAddress, out ipAddress))
					{
						this.resolvedRemoteIPAddress = ipAddress;
						RemoteHost = new XIPHost(this.resolvedRemoteIPAddress);
					}
					else
					{
						try
						{
							IPAddress[] ipAddresses;
							ipAddresses = Dns.GetHostAddresses(nameOrAddress);
							this.resolvedRemoteIPAddress = ipAddresses[0];
							RemoteHost = new XIPHost(this.resolvedRemoteIPAddress);
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

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Table-style coding.")]
		private void textBox_RemotePort_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int port;
				if (int.TryParse(textBox_RemotePort.Text, out port) &&
					(port >= System.Net.IPEndPoint.MinPort) && (port <= System.Net.IPEndPoint.MaxPort))
				{
					RemotePort = port;

					// Also set local port to same number
					if ((this.hostType == SocketHostType.TcpClient) || (this.hostType == SocketHostType.TcpAutoSocket) || (this.hostType == SocketHostType.Udp))
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
			if (!this.isSettingControls)
			{
				LocalInterface = comboBox_LocalInterface.SelectedItem as IPNetworkInterface;
				this.resolvedLocalIPAddress = LocalInterface.IPAddress;
			}
		}

		private void button_RefreshLocalInterfaces_Click(object sender, EventArgs e)
		{
			SetLocalInterfaceList();
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Table-style coding.")]
		private void textBox_LocalPort_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int port;
				if (int.TryParse(textBox_LocalPort.Text, out port) &&
					(port >= System.Net.IPEndPoint.MinPort) && (port <= System.Net.IPEndPoint.MaxPort))
				{
					LocalTcpPort = port;
					LocalUdpPort = port;

					// Also set remote port to same number
					if (this.hostType == SocketHostType.TcpServer)
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
			this.isSettingControls = true;

			// Remote host.
			comboBox_RemoteHost.Items.Clear();
			comboBox_RemoteHost.Items.AddRange(XIPHost.GetItems());

			this.isSettingControls = false;
		}

		private void SetLocalInterfaceList()
		{
			if (Enabled)
			{
				this.isSettingControls = true;

				IPNetworkInterface old = comboBox_LocalInterface.SelectedItem as IPNetworkInterface;

				IPNetworkInterfaceCollection localInterfaces = new IPNetworkInterfaceCollection();
				localInterfaces.FillWithAvailableInterfaces();

				comboBox_LocalInterface.Items.Clear();
				comboBox_LocalInterface.Items.AddRange(localInterfaces.ToArray());

				if (comboBox_LocalInterface.Items.Count > 0)
				{
					if ((this.localInterface != null) && (localInterfaces.Contains(this.localInterface)))
						comboBox_LocalInterface.SelectedItem = this.localInterface;
					else if ((old != null) && (localInterfaces.Contains(old)))
						comboBox_LocalInterface.SelectedItem = old;
					else
						comboBox_LocalInterface.SelectedIndex = 0;

					// Set property instead of member to ensure that changed event is fired.
					LocalInterface = comboBox_LocalInterface.SelectedItem as IPNetworkInterface;
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

				this.isSettingControls = false;
			}
		}

		private void SetControls()
		{
			this.isSettingControls = true;

			// Remote host address.
			if (Enabled && ((this.hostType == SocketHostType.TcpClient) || (this.hostType == SocketHostType.TcpAutoSocket) || (this.hostType == SocketHostType.Udp)))
			{
				comboBox_RemoteHost.Enabled = true;
				comboBox_RemoteHost.Text = this.remoteHost;
			}
			else
			{
				comboBox_RemoteHost.Enabled = false;
				comboBox_RemoteHost.Text = "";
			}

			// Remote port label.
			if (Enabled && (this.hostType == SocketHostType.Udp))
				label_RemotePort.Text = "Remote UDP port:";
			else
				label_RemotePort.Text = "Remote TCP port:";

			// Remote port.
			if (Enabled && ((this.hostType == SocketHostType.TcpClient) || (this.hostType == SocketHostType.TcpAutoSocket) || (this.hostType == SocketHostType.Udp)))
			{
				textBox_RemotePort.Enabled = true;
				textBox_RemotePort.Text = this.remotePort.ToString();
			}
			else
			{
				textBox_RemotePort.Enabled = false;
				textBox_RemotePort.Text = "";
			}

			// Local interface.
			if (!DesignMode && Enabled && (this.hostType != SocketHostType.Unknown) && (comboBox_LocalInterface.Items.Count > 0))
			{
				if (this.localInterface != null)
					comboBox_LocalInterface.SelectedItem = this.localInterface;
				else
					comboBox_LocalInterface.SelectedItem = (IPNetworkInterface)IPNetworkInterfaceType.Any;
			}
			else
			{
				comboBox_LocalInterface.SelectedIndex = -1;
			}

			// Local port label.
			if (Enabled && (this.hostType == SocketHostType.Udp))
				label_LocalPort.Text = "Local UDP port:";
			else
				label_LocalPort.Text = "Local TCP port:";

			// Local port.
			if (Enabled && ((this.hostType == SocketHostType.TcpServer) || (this.hostType == SocketHostType.TcpAutoSocket)))
			{
				textBox_LocalPort.Enabled = true;
				textBox_LocalPort.Text = this.localTcpPort.ToString();
			}
			else if (Enabled && (this.hostType == SocketHostType.Udp))
			{
				textBox_LocalPort.Enabled = true;
				textBox_LocalPort.Text = this.localUdpPort.ToString();
			}
			else
			{
				textBox_LocalPort.Enabled = false;
				textBox_LocalPort.Text = "";
			}

			this.isSettingControls = false;
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
