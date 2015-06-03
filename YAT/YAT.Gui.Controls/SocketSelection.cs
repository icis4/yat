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
// YAT 2.0 Gamma 1' Version 1.99.33
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Windows.Forms;

using MKY;
using MKY.IO.Serial.Socket;
using MKY.Net;
using MKY.Windows.Forms;

#endregion

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("HostNameOrAddressChanged")]
	public partial class SocketSelection : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketHostType DefaultHostType                      = SocketHostType.TcpAutoSocket;

		private static readonly IPHost DefaultRemoteHost                  = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteHost;
		private static readonly IPAddress DefaultResolvedRemoteIPAddress  = MKY.IO.Serial.Socket.SocketSettings.DefaultResolvedRemoteIPAddress;
		private const int DefaultRemoteTcpPort                            = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteTcpPort;
		private const int DefaultRemoteUdpPort                            = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteUdpPort;

		private static readonly IPNetworkInterface DefaultLocalInterface  = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalInterface;
		private static readonly IPAddress DefaultResolvedLocalIPAddress   = MKY.IO.Serial.Socket.SocketSettings.DefaultResolvedLocalIPAddress;
		private const int DefaultLocalTcpPort                             = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalTcpPort;
		private const int DefaultLocalUdpPort                             = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalUdpPort;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// Only set interface list and controls once as soon as this control is enabled. This saves
		/// some time on startup since scanning for the interfaces may take some time.
		/// </summary>
		private bool localInterfaceListIsInitialized; // = false;

		private SettingControlsHelper isSettingControls;

		private SocketHostType hostType = DefaultHostType;

		private IPHost remoteHost                 = DefaultRemoteHost;
		private IPAddress resolvedRemoteIPAddress = DefaultResolvedRemoteIPAddress;
		private int remoteTcpPort                 = DefaultRemoteTcpPort;
		private int remoteUdpPort                 = DefaultRemoteUdpPort;

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
		[Description("Event raised when the RemoteTcpPort property is changed.")]
		public event EventHandler RemoteTcpPortChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the RemoteUdpPort property is changed.")]
		public event EventHandler RemoteUdpPortChanged;

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
		public virtual SocketHostType HostType
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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPHost RemoteHost
		{
			get { return (this.remoteHost); }
			set
			{
				if (this.remoteHost != value)
				{
					this.remoteHost = value;
					SetControls();
					OnRemoteHostChanged(EventArgs.Empty);
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
		[Description("The remote TCP port.")]
		[DefaultValue(DefaultRemoteTcpPort)]
		public virtual int RemoteTcpPort
		{
			get { return (this.remoteTcpPort); }
			set
			{
				if (this.remoteTcpPort != value)
				{
					this.remoteTcpPort = value;
					SetControls();
					OnRemoteTcpPortChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Socket")]
		[Description("The remote UDP port.")]
		[DefaultValue(DefaultRemoteUdpPort)]
		public virtual int RemoteUdpPort
		{
			get { return (this.remoteUdpPort); }
			set
			{
				if (this.remoteUdpPort != value)
				{
					this.remoteUdpPort = value;
					SetControls();
					OnRemoteUdpPortChanged(EventArgs.Empty);
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
				if (this.localInterface != value)
				{
					this.localInterface = value;

					if (value != null) // In case there is no interface at all (e.g. offline laptop).
						this.resolvedLocalIPAddress = value.IPAddress;
					else
						this.resolvedLocalIPAddress = IPAddress.None;

					SetControls();
					OnLocalInterfaceChanged(EventArgs.Empty);
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
				if (this.localTcpPort != value)
				{
					this.localTcpPort = value;
					SetControls();
					OnLocalTcpPortChanged(EventArgs.Empty);
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
				if (this.localUdpPort != value)
				{
					this.localUdpPort = value;
					SetControls();
					OnLocalUdpPortChanged(EventArgs.Empty);
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
		private void SocketSelection_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				InitializeControls();
				SetControls();
			}

			// Ensure that interface list is set as soon as this control gets enabled. Could
			// also be implemented in a EnabledChanged event handler. However, it's easier
			// to implement this here so it also done on initial 'Paint' event.
			if (Enabled && !this.localInterfaceListIsInitialized)
			{
				SetLocalInterfaceList();
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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Emphasize line breaks.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
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
					IPHost ipHost;
					IPAddress ipAddress;
					string nameOrAddress;
					nameOrAddress = comboBox_RemoteHost.Text;

					if (IPHost.TryParse(nameOrAddress, out ipHost))
					{
						RemoteHost = ipHost;
					}
					else if (IPAddress.TryParse(nameOrAddress, out ipAddress))
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
							string message =
								"Remote host name or address is invalid!" + Environment.NewLine + Environment.NewLine +
								"System error message:" + Environment.NewLine + ex.Message;

							MessageBoxEx.Show
							(
								this,
								message,
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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Table-style coding.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_RemotePort_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int port;
				if (int.TryParse(textBox_RemotePort.Text, out port) &&
					(port >= System.Net.IPEndPoint.MinPort) && (port <= System.Net.IPEndPoint.MaxPort))
				{
					if ((this.hostType == SocketHostType.TcpClient) || (this.hostType == SocketHostType.TcpAutoSocket))
					{
						RemoteTcpPort = port;

						// Also set the local port to same number:
						//  > For Client: Makes it easier setting the server settings for a same connection.
						//  > For AutoSocket: Typically same port for client and server.
						LocalTcpPort = port;
					}
					else if (this.hostType == SocketHostType.Udp)
					{
						RemoteUdpPort = port;

						if (port < System.Net.IPEndPoint.MaxPort)
							LocalUdpPort = port + 1;
						else
							LocalUdpPort = System.Net.IPEndPoint.MaxPort - 1;
					}
				}
				else
				{
					string message =
						"Remote port is invalid, valid values are numbers from " +
						System.Net.IPEndPoint.MinPort.ToString(CultureInfo.InvariantCulture) + " to " +
						System.Net.IPEndPoint.MaxPort.ToString(CultureInfo.InvariantCulture) + "."; // 'InvariantCulture' for TCP and UDP ports!

					MessageBoxEx.Show
					(
						this,
						message,
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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Table-style coding.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_LocalPort_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int port;
				if (int.TryParse(textBox_LocalPort.Text, out port) &&
					(port >= System.Net.IPEndPoint.MinPort) && (port <= System.Net.IPEndPoint.MaxPort))
				{
					if ((this.hostType == SocketHostType.TcpServer) || (this.hostType == SocketHostType.TcpAutoSocket))
					{
						LocalTcpPort = port;

						// Also set the remote port to same number:
						//  > For Server: Makes it easier setting the client settings for a same connection.
						if (this.hostType == SocketHostType.TcpServer)
							RemoteTcpPort = port;
					}
					else if (this.hostType == SocketHostType.Udp)
					{
						LocalUdpPort = port;
					}
				}
				else
				{
					string message =
						"Local port is invalid, valid values are numbers from " +
						System.Net.IPEndPoint.MinPort.ToString(CultureInfo.InvariantCulture) + " to " +
						System.Net.IPEndPoint.MaxPort.ToString(CultureInfo.InvariantCulture) + "."; // 'InvariantCulture' for TCP and UDP ports!

					MessageBoxEx.Show
					(
						this,
						message,
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

		// MKY.Diagnostics.DebugEx.WriteStack(Type type)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		// YAT.Gui.Controls.SocketSelection.SetLocalInterfaceList()
		// YAT.Gui.Controls.SocketSelection.SocketSelection_Paint(Object sender, PaintEventArgs e)
		// System.Windows.Forms.Control.PaintWithErrorHandling(PaintEventArgs e, Int16 layer, Boolean disposeEventArgs)
		// System.Windows.Forms.Control.WmPaint(Message m)
		// System.Windows.Forms.Control.WndProc(Message m)
		// System.Windows.Forms.Control.ControlNativeWindow.WndProc(Message m)
		// System.Windows.Forms.NativeWindow.DebuggableCallback(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)
		// System.Windows.Forms.MessageBox.ShowCore(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, Boolean showHelp)
		// System.Windows.Forms.MessageBox.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		// YAT.Gui.Controls.SocketSelection.SetLocalInterfaceList()

		/// <remarks>
		/// Without precaution, and in case of no interfaces, the message box may appear twice due to
		/// the recursion described above (out of doc tag due to words not recognized by StyleCop).
		/// This issue is fixed by setting 'this.interfaceListIsInitialized' upon entering this method.
		/// 
		/// Note that the same fix has been implemented in <see cref="SerialPortSelection"/> and <see cref="UsbSerialHidDeviceSelection"/>.
		/// </remarks>
		[ModalBehavior(ModalBehavior.InCaseOfNonUserError, Approval = "Is only called when displaying or refreshing the control on a form.")]
		private void SetLocalInterfaceList()
		{
			if (Enabled)
			{
				this.localInterfaceListIsInitialized = true; // Purpose see remarks above.
				this.isSettingControls.Enter();

				IPNetworkInterfaceCollection localInterfaces = new IPNetworkInterfaceCollection();
				localInterfaces.FillWithAvailableInterfaces();

				comboBox_LocalInterface.Items.Clear();
				comboBox_LocalInterface.Items.AddRange(localInterfaces.ToArray());

				if (comboBox_LocalInterface.Items.Count > 0)
				{
					if ((this.localInterface != null) && (localInterfaces.Contains(this.localInterface)))
					{
						// Nothing has changed, just restore the selected item:
						comboBox_LocalInterface.SelectedItem = this.localInterface;
					}
					else
					{
						string localInterfaceNoLongerAvailable = this.localInterface;

						// Ensure that the settings item is defaulted and shown by SetControls().
						// Set property instead of member to ensure that changed event is fired.
						LocalInterface = localInterfaces[0];

						comboBox_LocalInterface.SelectedIndex = 0;

						if (!string.IsNullOrEmpty(localInterfaceNoLongerAvailable))
						{
							string message =
								"The given local network interface " + localInterfaceNoLongerAvailable + " is currently not available." + Environment.NewLine + Environment.NewLine +
								"The setting has been defaulted to the first available interface.";

							MessageBoxEx.Show
							(
								this,
								message,
								"Interface not available",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning
							);
						}
					}
				}
				else
				{
					// Ensure that the settings item is nulled and reset by SetControls().
					// Set property instead of member to ensure that changed event is fired.
					LocalInterface = null;

					MessageBoxEx.Show
					(
						this,
						"No local network interfaces available, check network system settings.",
						"No interfaces",
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
					{
						if (comboBox_RemoteHost.Items.Contains(this.remoteHost))
						{	// Applies if an item of the combo box is selected.
							comboBox_RemoteHost.SelectedItem = this.remoteHost;
						}
						else
						{	// Applies if an item that is not in the combo box is selected.
							comboBox_RemoteHost.SelectedIndex = ControlEx.InvalidIndex;
							comboBox_RemoteHost.Text = this.remoteHost;
						}
					}
					else
					{	// Item doesn't exist, use default = first item in the combo box.
						comboBox_RemoteHost.SelectedIndex = 0;
					}
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
			if (!DesignMode && Enabled && ((this.hostType == SocketHostType.TcpClient) || (this.hostType == SocketHostType.TcpAutoSocket)))
			{
				textBox_RemotePort.Enabled = true;
				textBox_RemotePort.Text = this.remoteTcpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
			}
			else if (!DesignMode && Enabled && (this.hostType == SocketHostType.Udp))
			{
				textBox_RemotePort.Enabled = true;
				textBox_RemotePort.Text = this.remoteUdpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
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
				textBox_LocalPort.Text = this.localTcpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
			}
			else if (Enabled && (this.hostType == SocketHostType.Udp))
			{
				textBox_LocalPort.Enabled = true;
				textBox_LocalPort.Text = this.localUdpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
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
		protected virtual void OnRemoteTcpPortChanged(EventArgs e)
		{
			EventHelper.FireSync(RemoteTcpPortChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRemoteUdpPortChanged(EventArgs e)
		{
			EventHelper.FireSync(RemoteUdpPortChanged, this, e);
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
