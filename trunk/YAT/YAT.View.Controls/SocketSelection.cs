//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2016 Matthias Kläy.
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

namespace YAT.View.Controls
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[DefaultEvent("HostNameOrAddressChanged")]
	public partial class SocketSelection : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketType DefaultSocketType                       = SocketType.TcpAutoSocket;

		private static readonly IPHostEx DefaultRemoteHost                 = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteHost;
		private const int DefaultRemoteTcpPort                           = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteTcpPort;
		private const int DefaultRemoteUdpPort                           = MKY.IO.Serial.Socket.SocketSettings.DefaultRemoteUdpPort;

		private static readonly IPNetworkInterfaceEx DefaultLocalInterface = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalInterface;
		private static readonly IPAddressFilterEx DefaultLocalFilter       = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalFilter;
		private const int DefaultLocalTcpPort                            = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalTcpPort;
		private const int DefaultLocalUdpPort                            = MKY.IO.Serial.Socket.SocketSettings.DefaultLocalUdpPort;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// Only set interface list and controls once as soon as this control is enabled. This saves
		/// some time on startup since scanning for the interfaces may take some time.
		/// </summary>
		private bool localInterfaceListIsBeingSetOrIsAlreadySet; // = false;

		private SettingControlsHelper isSettingControls;

		private SocketType socketType = DefaultSocketType;

		private IPHostEx remoteHost                 = DefaultRemoteHost;
		private int remoteTcpPort                 = DefaultRemoteTcpPort;
		private int remoteUdpPort                 = DefaultRemoteUdpPort;

		private IPNetworkInterfaceEx localInterface = DefaultLocalInterface;
		private IPAddressFilterEx localFilter       = DefaultLocalFilter;
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
		[Description("Event raised when the LocalFilter property is changed.")]
		public event EventHandler LocalFilterChanged;

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
		public virtual SocketType SocketType
		{
			set
			{
				if (this.socketType != value)
				{
					this.socketType = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPHostEx RemoteHost
		{
			get { return (this.remoteHost); }
			set
			{
				if ((this.remoteHost != value) ||
					(value.IPAddress == IPAddress.Loopback)) // Always SetControls() to be able to
				{	                                         //   deal with the different types of
					this.remoteHost = value;                 //   localhost/loopback.
					SetControls();
					OnRemoteHostChanged(EventArgs.Empty);
				}
			}
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
		public virtual IPNetworkInterfaceEx LocalInterface
		{
			get { return (this.localInterface); }
			set
			{
				if ((this.localInterface != value) ||
					(value.IPAddress == IPAddress.Loopback)) // Always SetControls() to be able to
				{	                                         //   deal with the different types of
					this.localInterface = value;             //   localhost/loopback.
					SetControls();
					OnLocalInterfaceChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPAddressFilterEx LocalFilter
		{
			get { return (this.localFilter); }
			set
			{
				if ((this.localFilter != value) ||
					(value.IPAddress == IPAddress.Any)) // Always SetControls() to be able to
				{                                       //   deal with the different types of
					this.localFilter = value;           //   any.
					SetControls();
					OnLocalFilterChanged(EventArgs.Empty);
				}
			}
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
			if (Enabled && !this.localInterfaceListIsBeingSetOrIsAlreadySet)
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

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_RemoteHost_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				// Attention:
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				var remoteHost = (comboBox_RemoteHost.SelectedItem as IPHostEx);
				if ((remoteHost != null) && (remoteHost.IPAddress != IPAddress.None) &&
					StringEx.EqualsOrdinalIgnoreCase(remoteHost.ToString(), comboBox_RemoteHost.Text))
				{
					RemoteHost = remoteHost;
				}
				else
				{
					IPAddress ipAddress;
					if (IPResolver.TryResolveRemoteHost(comboBox_RemoteHost.Text, out ipAddress))
					{
						RemoteHost = new IPHostEx(ipAddress);
					}
					else
					{
						MessageBoxEx.Show
						(
							this,
							"Remote host name or address is invalid!",
							"Invalid Input",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
						);

						e.Cancel = true;
					}
				}
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_LocalFilter_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				// Attention:
				// Do not assume that the selected item maches the actual text in the box
				//   because SelectedItem is also set if text has changed in the meantime.

				var localFilter = (comboBox_LocalFilter.SelectedItem as IPAddressFilterEx);
				if ((localFilter != null) && (localFilter.IPAddress != IPAddress.None) &&
					StringEx.EqualsOrdinalIgnoreCase(localFilter.ToString(), comboBox_LocalFilter.Text))
				{
					LocalFilter = localFilter;
				}
				else
				{
					IPAddress ipAddress;
					if (IPResolver.TryResolveRemoteHost(comboBox_LocalFilter.Text, out ipAddress))
					{
						LocalFilter = new IPAddressFilterEx(ipAddress);
					}
					else
					{
						MessageBoxEx.Show
						(
							this,
							"Address filter is invalid!",
							"Invalid Input",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
						);

						e.Cancel = true;
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
					(port >= IPEndPoint.MinPort) && (port <= IPEndPoint.MaxPort))
				{
					if ((this.socketType == SocketType.TcpClient) || (this.socketType == SocketType.TcpAutoSocket))
					{
						RemoteTcpPort = port;

						// Also set the local port:
						//  > For client:     Same port, makes it easier setting the server settings for a same connection.
						//  > For AutoSocket: Typically using same port for client and server.
						LocalTcpPort = port;
					}
					else if ((this.socketType == SocketType.UdpClient) || (this.socketType == SocketType.UdpPairSocket))
					{
						RemoteUdpPort = port;

						// Also set the local port:
						//  > For client: Same port, makes it easier setting the server settings for a same connection.
						//  > For socket:
						//     > On local host, typically using adjecent ports for client and server.
						//     > On remote host, typically using same port for client and server.
						if (this.socketType == SocketType.UdpClient)
						{
							LocalUdpPort = port;
						}
						else
						{
							if (RemoteHost.IsLocalHost)
							{
								if (port < IPEndPoint.MaxPort)
									LocalUdpPort = port + 1;
								else
									LocalUdpPort = IPEndPoint.MaxPort - 1;
							}
							else
							{
								LocalUdpPort = port;
							}
						}
					}
				}
				else
				{
					string message =
						"Remote port is invalid, valid values are numbers from " +
						IPEndPoint.MinPort.ToString(CultureInfo.InvariantCulture) + " to " +
						IPEndPoint.MaxPort.ToString(CultureInfo.InvariantCulture) + "."; // 'InvariantCulture' for TCP and UDP ports!

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
				LocalInterface = (comboBox_LocalInterface.SelectedItem as IPNetworkInterfaceEx);
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
					(port >= IPEndPoint.MinPort) && (port <= IPEndPoint.MaxPort))
				{
					if ((this.socketType == SocketType.TcpServer) || (this.socketType == SocketType.TcpAutoSocket))
					{
						LocalTcpPort = port;

						// Also set the remote port:
						//  > For server: Same port, makes it easier setting the client settings for a same connection.
						if (this.socketType == SocketType.TcpServer)
							RemoteTcpPort = port;
					}
					else if ((this.socketType == SocketType.UdpServer) || (this.socketType == SocketType.UdpPairSocket))
					{
						LocalUdpPort = port;

						// Also set the remote port:
						//  > For server:
						//     > On local host, typically using adjecent ports for client and server.
						//     > On remote host, typically using same port for client and server.
						if (this.socketType == SocketType.UdpServer)
						{
							if (RemoteHost.IsLocalHost)
							{
								if (port > IPEndPoint.MinPort)
									RemoteUdpPort = port - 1;
								else
									RemoteUdpPort = IPEndPoint.MinPort + 1;
							}
							else
							{
								RemoteUdpPort = port;
							}
						}
					}
				}
				else
				{
					string message =
						"Local port is invalid, valid values are numbers from " +
						IPEndPoint.MinPort.ToString(CultureInfo.InvariantCulture) + " to " +
						IPEndPoint.MaxPort.ToString(CultureInfo.InvariantCulture) + "."; // 'InvariantCulture' for TCP and UDP ports!

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

			// Remote host:
			comboBox_RemoteHost.Items.Clear();
			comboBox_RemoteHost.Items.AddRange(IPHostEx.GetItems());

			// Local filter:
			comboBox_LocalFilter.Items.Clear();
			comboBox_LocalFilter.Items.AddRange(IPAddressFilterEx.GetItems());

			this.isSettingControls.Leave();
		}

		// MKY.Diagnostics.DebugEx.WriteStack(Type type)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		// MKY.Windows.Forms.MessageBoxEx.Show(IWin32Window owner, String text, String caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		// YAT.View.Controls.SocketSelection.SetLocalInterfaceList()
		// YAT.View.Controls.SocketSelection.SocketSelection_Paint(Object sender, PaintEventArgs e)
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
		// YAT.View.Controls.SocketSelection.SetLocalInterfaceList()

		/// <remarks>
		/// Without precaution, and in case of no interfaces, the message box may appear twice due to
		/// the recursion described above (out of doc tag due to words not recognized by StyleCop).
		/// This issue is fixed by setting 'localInterfaceListIsBeingSetOrIsAlreadySet' upon entering this method.
		/// 
		/// Note that the same fix has been implemented in <see cref="SerialPortSelection"/> and <see cref="UsbSerialHidDeviceSelection"/>.
		/// </remarks>
		[ModalBehavior(ModalBehavior.InCaseOfNonUserError, Approval = "Is only called when displaying or refreshing the control on a form.")]
		private void SetLocalInterfaceList()
		{
			if (Enabled)
			{
				this.localInterfaceListIsBeingSetOrIsAlreadySet = true; // Purpose see remarks above.
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
								"The local network interface " + localInterfaceNoLongerAvailable + " is currently not available." + Environment.NewLine + Environment.NewLine +
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

			// Remote host address:
			if (!DesignMode && Enabled &&
				((this.socketType == SocketType.TcpClient) || (this.socketType == SocketType.TcpAutoSocket) ||
				 (this.socketType == SocketType.UdpClient) || (this.socketType == SocketType.UdpPairSocket)))
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

			// Remote port label:
			if (Enabled && ((SocketTypeEx)this.socketType).IsUdp)
				label_RemotePort.Text = "Remote UDP port:";
			else
				label_RemotePort.Text = "Remote TCP port:";

			// Remote port:
			if (!DesignMode && Enabled && ((this.socketType == SocketType.TcpClient) || (this.socketType == SocketType.TcpAutoSocket)))
			{
				textBox_RemotePort.Enabled = true;
				textBox_RemotePort.Text = this.remoteTcpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
			}
			else if (!DesignMode && Enabled && (this.socketType == SocketType.UdpClient) || (this.socketType == SocketType.UdpPairSocket))
			{
				textBox_RemotePort.Enabled = true;
				textBox_RemotePort.Text = this.remoteUdpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
			}
			else
			{
				textBox_RemotePort.Enabled = false;
				textBox_RemotePort.Text = "";
			}

			// Local interface/filter label:
			if (Enabled && ((SocketTypeEx)this.socketType).IsUdp)
				label_LocalAddress.Text = "Local &Filter:";
			else
				label_LocalAddress.Text = "Local &Interface:";

			// Local interface:
			if (!DesignMode && Enabled && ((SocketTypeEx)this.socketType).IsTcp)
			{
				comboBox_LocalInterface.Visible = true;
				comboBox_LocalInterface.Enabled = true;
				if (comboBox_LocalInterface.Items.Count > 0)
				{
					if (this.localInterface != null)
						comboBox_LocalInterface.SelectedItem = this.localInterface;
					else
						comboBox_LocalInterface.SelectedItem = (IPNetworkInterfaceEx)IPNetworkInterface.Any;
				}
				else
				{
					comboBox_LocalInterface.SelectedIndex = ControlEx.InvalidIndex;
				}

				button_RefreshLocalInterfaces.Visible = true;
				button_RefreshLocalInterfaces.Enabled = true;
			}
			else
			{
				comboBox_LocalInterface.Visible = false;
				comboBox_LocalInterface.Enabled = false;
				comboBox_LocalInterface.SelectedIndex = ControlEx.InvalidIndex;

				button_RefreshLocalInterfaces.Visible = false;
				button_RefreshLocalInterfaces.Enabled = false;
			}

			// Local filter:
			if (!DesignMode && Enabled && ((SocketTypeEx)this.socketType).IsUdp)
			{
				comboBox_LocalFilter.Visible = true;
				switch (this.socketType)
				{
					case SocketType.UdpClient:
					{
						comboBox_LocalFilter.Enabled = false;
						comboBox_LocalFilter.SelectedIndex = ControlEx.InvalidIndex;
						comboBox_LocalFilter.Text = "";
						break;
					}

					case SocketType.UdpServer:
					{
						comboBox_LocalFilter.Enabled = true;
						if (comboBox_LocalFilter.Items.Count > 0)
						{
							if (this.localFilter != null)
							{
								if (comboBox_LocalFilter.Items.Contains(this.localFilter))
								{   // Applies if an item of the combo box is selected.
									comboBox_LocalFilter.SelectedItem = this.localFilter;
								}
								else
								{   // Applies if an item that is not in the combo box is selected.
									comboBox_LocalFilter.SelectedIndex = ControlEx.InvalidIndex;
									comboBox_LocalFilter.Text = this.localFilter;
								}
							}
							else
							{   // Item doesn't exist, use default = first item in the combo box.
								comboBox_LocalFilter.SelectedIndex = 0;
							}
						}
						else
						{
							comboBox_LocalFilter.SelectedIndex = ControlEx.InvalidIndex;
							if (this.localFilter != null)
								comboBox_LocalFilter.Text = this.localFilter;
							else
								comboBox_LocalFilter.Text = "";
						}
						break;
					}

					case SocketType.UdpPairSocket:
					{
						comboBox_LocalFilter.Enabled = false;
						comboBox_LocalFilter.SelectedIndex = ControlEx.InvalidIndex;
						comboBox_LocalFilter.Text = this.remoteHost;
						break;
					}

					default:
					{
						throw (new NotSupportedException("Program execution should never get here,'" + this.socketType + "' is invalid here." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
			else
			{
				comboBox_LocalFilter.Visible = false;
				comboBox_LocalFilter.Enabled = false;
				comboBox_LocalFilter.SelectedIndex = ControlEx.InvalidIndex;
				comboBox_LocalFilter.Text = "";
			}

			// Local port label:
			if (Enabled && ((SocketTypeEx)this.socketType).IsUdp)
				label_LocalPort.Text = "Local UDP port:";
			else
				label_LocalPort.Text = "Local TCP port:";

			// Local port:
			if (Enabled && ((this.socketType == SocketType.TcpServer) || (this.socketType == SocketType.TcpAutoSocket)))
			{
				textBox_LocalPort.Enabled = true;
				textBox_LocalPort.Text = this.localTcpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
			}
			else if (Enabled && (this.socketType == SocketType.UdpServer) || (this.socketType == SocketType.UdpPairSocket))
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
		protected virtual void OnLocalFilterChanged(EventArgs e)
		{
			EventHelper.FireSync(LocalFilterChanged, this, e);
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
