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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using System.Globalization;
using System.Net;
using System.Windows.Forms;

using MKY;
using MKY.Event;
using MKY.IO.Serial;
using MKY.Net;
using MKY.Windows.Forms;

#endregion

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

		private static readonly IPHost DefaultRemoteHost                 = MKY.IO.Serial.SocketSettings.DefaultRemoteHost;
		private static readonly IPAddress DefaultResolvedRemoteIPAddress = MKY.IO.Serial.SocketSettings.DefaultResolvedRemoteIPAddress;
		private const int DefaultRemotePort                              = MKY.IO.Serial.SocketSettings.DefaultRemotePort;

		private static readonly IPNetworkInterface DefaultLocalInterface = MKY.IO.Serial.SocketSettings.DefaultLocalInterface;
		private static readonly IPAddress DefaultResolvedLocalIPAddress  = MKY.IO.Serial.SocketSettings.DefaultResolvedLocalIPAddress;
		private const int DefaultLocalTcpPort                            = MKY.IO.Serial.SocketSettings.DefaultLocalTcpPort;
		private const int DefaultLocalUdpPort                            = MKY.IO.Serial.SocketSettings.DefaultLocalUdpPort;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private SocketHostType hostType = DefaultHostType;

		private IPHost remoteHost                = DefaultRemoteHost;
		private IPAddress resolvedRemoteIPAddress = DefaultResolvedRemoteIPAddress;
		private int remotePort                    = DefaultRemotePort;

		private IPNetworkInterface localInterface = DefaultLocalInterface;
		private IPAddress resolvedLocalIPAddress  = DefaultResolvedLocalIPAddress;
		private int localTcpPort                  = DefaultLocalTcpPort;
		private int localUdpPort                  = DefaultLocalUdpPort;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the RemoteHost property is changed.")]
		public event EventHandler RemoteHostChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the RemotePort property is changed.")]
		public event EventHandler RemotePortChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the LocalInterface property is changed.")]
		public event EventHandler LocalInterfaceChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the LocalTcpPort property is changed.")]
		public event EventHandler LocalTcpPortChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the LocalUdpPort property is changed.")]
		public event EventHandler LocalUdpPortChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SocketSelection()
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

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPHost RemoteHost
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

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPAddress ResolvedRemoteIPAddress
		{
			get { return (this.resolvedRemoteIPAddress); }
		}

		/// <summary></summary>
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

		/// <summary></summary>
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

					if (value != null) // In case there is no interface at all (e.g. offline laptop).
						this.resolvedLocalIPAddress = value.IPAddress;
					else
						this.resolvedLocalIPAddress = IPAddress.None;

					SetControls();
					OnLocalInterfaceChanged(new EventArgs());
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPAddress ResolvedLocalIPAddress
		{
			get { return (this.resolvedLocalIPAddress); }
		}

		/// <summary></summary>
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

		/// <summary></summary>
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
				// \attention:
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				IPHost host = comboBox_RemoteHost.SelectedItem as IPHost;
				if ((host != null) && (host.IPAddress != IPAddress.None) && StringEx.EqualsOrdinalIgnoreCase(host.ToString(), comboBox_RemoteHost.Text))
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
						RemoteHost = new IPHost(this.resolvedRemoteIPAddress);
					}
					else
					{
						try
						{
							IPAddress[] ipAddresses;
							ipAddresses = Dns.GetHostAddresses(nameOrAddress);
							this.resolvedRemoteIPAddress = ipAddresses[0];
							RemoteHost = new IPHost(this.resolvedRemoteIPAddress);
						}
						catch (ArgumentException ex)
						{
							MessageBox.Show
								(
								this,
								"Remote host name or address is invalid!" + Environment.NewLine + Environment.NewLine +
								"System error message:" + Environment.NewLine +
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

					// Also set local port to same number.
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
							System.Net.IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) + " to " +
							System.Net.IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".",
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
				LocalInterface = comboBox_LocalInterface.SelectedItem as IPNetworkInterface;
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
							System.Net.IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) + " to " +
							System.Net.IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".",
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
			this.isSettingControls.Enter();

			// Remote host.
			comboBox_RemoteHost.Items.Clear();
			comboBox_RemoteHost.Items.AddRange(IPHost.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetLocalInterfaceList()
		{
			if (Enabled)
			{
				this.isSettingControls.Enter();

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
						"No Interfaces",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
				}

				this.isSettingControls.Leave();
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			// Remote host address.
			if (!DesignMode && Enabled && ((this.hostType == SocketHostType.TcpClient) || (this.hostType == SocketHostType.TcpAutoSocket) || (this.hostType == SocketHostType.Udp)))
			{
				comboBox_RemoteHost.Enabled = true;
				if (comboBox_RemoteHost.Items.Count > 0)
				{
					if (this.remoteHost != null)
						comboBox_RemoteHost.SelectedItem = this.remoteHost;
					else
						comboBox_RemoteHost.SelectedIndex = 0;
				}
				else
				{
					comboBox_RemoteHost.SelectedIndex = ControlEx.InvalidIndex;
					if (this.remoteHost != null)
						comboBox_RemoteHost.Text = this.remoteHost;
					else
						comboBox_RemoteHost.Text = "";
				}
			}
			else
			{
				comboBox_RemoteHost.Enabled = false;
				comboBox_RemoteHost.SelectedIndex = ControlEx.InvalidIndex;
				comboBox_RemoteHost.Text = "";
			}

			// Remote port label.
			if (Enabled && (this.hostType == SocketHostType.Udp))
				label_RemotePort.Text = "Remote UDP port:";
			else
				label_RemotePort.Text = "Remote TCP port:";

			// Remote port.
			if (!DesignMode && Enabled && ((this.hostType == SocketHostType.TcpClient) || (this.hostType == SocketHostType.TcpAutoSocket) || (this.hostType == SocketHostType.Udp)))
			{
				textBox_RemotePort.Enabled = true;
				textBox_RemotePort.Text = this.remotePort.ToString(NumberFormatInfo.CurrentInfo);
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
				comboBox_LocalInterface.SelectedIndex = ControlEx.InvalidIndex;
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
				textBox_LocalPort.Text = this.localTcpPort.ToString(NumberFormatInfo.InvariantInfo);
			}
			else if (Enabled && (this.hostType == SocketHostType.Udp))
			{
				textBox_LocalPort.Enabled = true;
				textBox_LocalPort.Text = this.localUdpPort.ToString(NumberFormatInfo.InvariantInfo);
			}
			else
			{
				textBox_LocalPort.Enabled = false;
				textBox_LocalPort.Text = "";
			}

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnRemoteHostChanged(EventArgs e)
		{
			EventHelper.FireSync(RemoteHostChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRemotePortChanged(EventArgs e)
		{
			EventHelper.FireSync(RemotePortChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnLocalInterfaceChanged(EventArgs e)
		{
			EventHelper.FireSync(LocalInterfaceChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnLocalTcpPortChanged(EventArgs e)
		{
			EventHelper.FireSync(LocalTcpPortChanged, this, e);
		}

		/// <summary></summary>
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
