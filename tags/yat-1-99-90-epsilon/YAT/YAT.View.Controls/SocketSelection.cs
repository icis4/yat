﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
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
using MKY.Collections.Specialized;
using MKY.IO.Serial.Socket;
using MKY.Net;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[DefaultEvent("HostNameOrAddressChanged")]
	public partial class SocketSelection : UserControl, IOnFormDeactivateWorkaround
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const SocketType SocketTypeDefault                         =  SocketTypeEx.Default;

		private static readonly IPHostEx RemoteHostDefault                 =  MKY.IO.Serial.Socket.SocketSettings.RemoteHostDefault;
		private const int                RemotePortDefault                 =  MKY.IO.Serial.Socket.SocketSettings.RemotePortDefault;

		private static readonly IPNetworkInterfaceEx LocalInterfaceDefault =  MKY.IO.Serial.Socket.SocketSettings.LocalInterfaceDefault;
		private static readonly IPFilterEx           LocalFilterDefault    =  MKY.IO.Serial.Socket.SocketSettings.LocalFilterDefault;
		private const int                            LocalTcpPortDefault   =  MKY.IO.Serial.Socket.SocketSettings.LocalPortDefault;
		private const int                            LocalUdpPortDefault   = (MKY.IO.Serial.Socket.SocketSettings.LocalPortDefault + 1);

		// Note that separate local port defaults are needed to account for the fact, that this
		// control implements an additional logic for UDP ports:
		//  > LocalUdpPort = (port + 1);
		//  > RemoteUdpPort = (port - 1);
		// The first is the "normal" case, thus using this logic as default.

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

		private SocketType socketType               = SocketTypeDefault;

		private IPHostEx remoteHost                 = RemoteHostDefault;
		private int remoteTcpPort                   = RemotePortDefault;
		private int remoteUdpPort                   = RemotePortDefault;

		private IPNetworkInterfaceEx localInterface = LocalInterfaceDefault;
		private IPFilterEx localFilter              = LocalFilterDefault;
		private int localTcpPort                    = LocalTcpPortDefault;
		private int localUdpPort                    = LocalUdpPortDefault;

		private RecentIPHostCollection recentRemoteHosts; // = null;
		private RecentIPFilterCollection recentLocalFilters; // = null;
		private RecentItemCollection<int> recentPorts; // = null;

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
			get
			{
				return (this.socketType);
			}
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
				if ((this.remoteHost != value) || // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
				    (value.Address.Equals(IPAddress.Loopback))) // Always SetControls() to be able to
				{                                               //   deal with the different types of
					this.remoteHost = value;                    //   localhost/loopback.
					SetControls();
					OnRemoteHostChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Socket")]
		[Description("The remote TCP port.")]
		[DefaultValue(RemotePortDefault)]
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
		[DefaultValue(RemotePortDefault)]
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
				if ((this.localInterface != value) || // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
					(value.Address.Equals(IPAddress.Loopback))) // Always SetControls() to be able to
				{	                                            //   deal with the different types of
					this.localInterface = value;                //   localhost/loopback.
					SetControls();
					OnLocalInterfaceChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IPFilterEx LocalFilter
		{
			get { return (this.localFilter); }
			set
			{
				if ((this.localFilter != value) || // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
					(value.Address.Equals(IPAddress.Any))) // Always SetControls() to be able to
				{                                          //   deal with the different types of
					this.localFilter = value;              //   any.
					SetControls();
					OnLocalFilterChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Socket")]
		[Description("The local TCP port.")]
		[DefaultValue(LocalTcpPortDefault)]
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
		[DefaultValue(LocalUdpPortDefault)]
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Using specialized collection.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentIPHostCollection RecentRemoteHosts
		{
			get { return (this.recentRemoteHosts); }
			set
			{
				if (this.recentRemoteHosts != value)
				{
					this.recentRemoteHosts = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Using specialized collection.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentIPFilterCollection RecentLocalFilters
		{
			get { return (this.recentLocalFilters); }
			set
			{
				if (this.recentLocalFilters != value)
				{
					this.recentLocalFilters = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Using specialized collection.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<int> RecentPorts
		{
			get { return (this.recentPorts); }
			set
			{
				if (this.recentPorts != value)
				{
					this.recentPorts = value;
					SetControls();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void RefreshLocalInterfaceList()
		{
			SetLocalInterfaceList();
		}

		/// <remarks>See remarks in <see cref="ComboBoxEx"/>.</remarks>
		public virtual void OnFormDeactivateWorkaround()
		{
			comboBox_RemoteHost .OnFormDeactivateWorkaround();
			comboBox_RemotePort .OnFormDeactivateWorkaround();
		////comboBox_LocalInterface is a standard ComboBox.
			comboBox_LocalFilter.OnFormDeactivateWorkaround();
			comboBox_LocalPort  .OnFormDeactivateWorkaround();
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
				SetLocalInterfaceList();
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void SocketSelection_EnabledChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

	////private void comboBox_RemoteHost_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since      "    _Validating() below gets called anyway.

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_RemoteHost_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			// Attention:
			// Do not assume that the selected item matches the actual text in the box
			//   because 'SelectedItem' is kept even if text has changed in the meantime.

			var remoteHost = (comboBox_RemoteHost.SelectedItem as IPHostEx);
			if ((remoteHost != null) && (IPAddressEx.NotEqualsNone(remoteHost.Address)) &&
				StringEx.EqualsOrdinalIgnoreCase(remoteHost.ToString(), comboBox_RemoteHost.Text))
			{
				RemoteHost = remoteHost;
			}
			else
			{
				// Immediately try to resolve the corresponding remote IP address:
				IPHostEx ipHost;
				if (IPHostEx.TryParseAndResolve(comboBox_RemoteHost.Text, out ipHost))
				{
					RemoteHost = ipHost;
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

	////private void comboBox_LocalFilter_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since      "     _Validating() below gets called anyway.

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_LocalFilter_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			// Attention:
			// Do not assume that the selected item matches the actual text in the box
			//   because 'SelectedItem' is kept even if text has changed in the meantime.

			var localFilter = (comboBox_LocalFilter.SelectedItem as IPFilterEx);
			if ((localFilter != null) && (IPAddressEx.NotEqualsNone(localFilter.Address)) &&
				StringEx.EqualsOrdinalIgnoreCase(localFilter.ToString(), comboBox_LocalFilter.Text))
			{
				LocalFilter = localFilter;
			}
			else
			{
				// Immediately try to resolve the corresponding IP address:
				IPFilterEx ipAddressFilter;
				if (IPFilterEx.TryParseAndResolve(comboBox_LocalFilter.Text, out ipAddressFilter))
				{
					LocalFilter = ipAddressFilter;
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

	////private void comboBox_RemotePort_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since     "     _Validating() below gets called anyway.

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Table-style coding.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_RemotePort_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int port;
			if (int.TryParse(comboBox_RemotePort.Text, out port) && IPEndPointEx.IsValidPort(port))
			{
				if ((this.socketType == SocketType.TcpClient) || (this.socketType == SocketType.TcpAutoSocket))
				{
					// Also set the local port:
					//  > For client:     Same port, makes it easier setting the server settings for a same connection.
					//  > For AutoSocket: Typically using same port for client and server.
					LocalTcpPort = port;

					// Set the remote port after setting the local port, thus on top in recents:
					RemoteTcpPort = port;
				}
				else if ((this.socketType == SocketType.UdpClient) || (this.socketType == SocketType.UdpPairSocket))
				{
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
						if (RemoteHost.IsLocalhost)
						{
							if (port < IPEndPoint.MaxPort)
								LocalUdpPort = (port + 1);
							else
								LocalUdpPort = (IPEndPoint.MaxPort - 1);
						}
						else
						{
							LocalUdpPort = port;
						}
					}

					// Set the remote port after setting the local port, thus on top in recents:
					RemoteUdpPort = port;
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

		private void comboBox_LocalInterface_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			LocalInterface = (comboBox_LocalInterface.SelectedItem as IPNetworkInterfaceEx);
		}

		private void button_RefreshLocalInterfaces_Click(object sender, EventArgs e)
		{
			SetLocalInterfaceList();
		}

	////private void comboBox_LocalPort_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since     "    _Validating() below gets called anyway.

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Table-style coding.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_LocalPort_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int port;
			if (int.TryParse(comboBox_LocalPort.Text, out port) && IPEndPointEx.IsValidPort(port))
			{
				if ((this.socketType == SocketType.TcpServer) || (this.socketType == SocketType.TcpAutoSocket))
				{
					// Also set the remote port:
					//  > For server: Same port, makes it easier setting the client settings for a same connection.
					if (this.socketType == SocketType.TcpServer)
						RemoteTcpPort = port;

					// Set the local port after setting the remote port, thus on top in recents:
					LocalTcpPort = port;
				}
				else if ((this.socketType == SocketType.UdpServer) || (this.socketType == SocketType.UdpPairSocket))
				{
					// Also set the remote port:
					//  > For server:
					//     > On local host, typically using adjecent ports for client and server.
					//     > On remote host, typically using same port for client and server.
					if (this.socketType == SocketType.UdpServer)
					{
						if (RemoteHost.IsLocalhost)
						{
							if (port > IPEndPoint.MinPort)
								RemoteUdpPort = (port - 1);
							else
								RemoteUdpPort = (IPEndPoint.MinPort + 1);
						}
						else
						{
							RemoteUdpPort = port;
						}
					}

					// Set the local port after setting the remote port, thus on top in recents:
					LocalUdpPort = port;
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

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls.Enter();
			try
			{
				comboBox_RemoteHost.Items.Clear();
				if (this.recentRemoteHosts != null)
					comboBox_RemoteHost.Items.AddRange(this.recentRemoteHosts.ToArray());

				comboBox_RemotePort.Items.Clear();
				if (this.recentPorts != null)
					comboBox_RemotePort.Items.AddRange(this.recentPorts.ToArray());

				comboBox_LocalFilter.Items.Clear();
				if (this.recentLocalFilters != null)
					comboBox_LocalFilter.Items.AddRange(this.recentLocalFilters.ToArray());

				comboBox_LocalPort.Items.Clear();
				if (this.recentPorts != null)
					comboBox_LocalPort.Items.AddRange(this.recentPorts.ToArray());
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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
			if (Enabled && !DesignMode)
			{
				ResetOnDialogMessage();

				this.localInterfaceListIsBeingSetOrIsAlreadySet = true; // Purpose see remarks above.

				var localInterfaces = new IPNetworkInterfaceCollection();
				localInterfaces.FillWithAvailableLocalInterfaces();

				// Attention:
				// Similar code exists in Model.Terminal.ValidateIO().
				// Changes here may have to be applied there too!

				this.isSettingControls.Enter();
				try
				{
					comboBox_LocalInterface.Items.Clear();

					if (localInterfaces.Count > 0)
					{
						comboBox_LocalInterface.Items.AddRange(localInterfaces.ToArray());

						if ((this.localInterface != null) && (localInterfaces.Contains(this.localInterface)))
						{
							// Nothing has changed, just restore the selected item:
							comboBox_LocalInterface.SelectedItem = this.localInterface;
						}
						else if ((this.localInterface != null) && (localInterfaces.ContainsDescription(this.localInterface)))
						{
							// A device with same description is available, use that:
							int sameDescriptionIndex = localInterfaces.FindIndexDescription(this.localInterface);

							// Get the 'NotAvailable' string BEFORE defaulting!
							string localInterfaceNotAvailable = null;
							if (this.localInterface != null)
								localInterfaceNotAvailable = this.localInterface;

							// Ensure that the settings item is switched and shown by SetControls().
							// Set property instead of member to ensure that changed event is raised.
							LocalInterface = localInterfaces[sameDescriptionIndex];

							ShowNotAvailableSwitchedMessage(localInterfaceNotAvailable, localInterfaces[sameDescriptionIndex]);
						}
						else
						{
							// Get the 'NotAvailable' string BEFORE defaulting!
							string localInterfaceNotAvailable = this.localInterface;

							// Ensure that the settings item is defaulted and shown by SetControls().
							// Set property instead of member to ensure that changed event is raised.
							LocalInterface = localInterfaces[0];

							ShowNotAvailableDefaultedMessage(localInterfaceNotAvailable, localInterfaces[0]);
						}
					}
					else // localInterfaces.Count == 0
					{
						// Ensure that the settings item is nulled and reset by SetControls().
						// Set property instead of member to ensure that changed event is raised.
						LocalInterface = null;

						ShowNoLocalInterfacesMessage();
					}
				}
				finally
				{
					this.isSettingControls.Leave();
				}
			}
		}

		private void ResetOnDialogMessage()
		{
			label_OnDialogMessage.Text = "";
		}

		/// <remarks>
		/// Showing this as on dialog message instead of <see cref="MessageBox"/> to reduce the number of potentially annoying popups.
		/// </remarks>
		private void ShowNoLocalInterfacesMessage()
		{
			label_OnDialogMessage.Text = "No local network interfaces currently available";
		}

		private void ShowNotAvailableDefaultedMessage(string localInterfaceNotAvailable, string localInterfaceDefaulted)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"The selection has been defaulted to the first available interface '" + localInterfaceDefaulted + "'.";

			MessageBoxEx.Show
			(
				this,
				message,
				"Previous interface not available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		private void ShowNotAvailableSwitchedMessage(string localInterfaceNotAvailable, string localInterfaceSwitched)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"The selection has been switched to '" + localInterfaceSwitched + "' (first available device with same description).";

			MessageBoxEx.Show
			(
				this,
				message,
				"Previous interface not available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				// Remote host address:
				if (!DesignMode && Enabled && (this.remoteHost != null) &&
				    ((this.socketType == SocketType.TcpClient) || (this.socketType == SocketType.TcpAutoSocket) ||
				     (this.socketType == SocketType.UdpClient) || (this.socketType == SocketType.UdpPairSocket)))
				{
					comboBox_RemoteHost.Enabled = true;
					ComboBoxHelper.Select(comboBox_RemoteHost, this.remoteHost, this.remoteHost);
				}
				else
				{
					comboBox_RemoteHost.Enabled = false;
					ComboBoxHelper.Deselect(comboBox_RemoteHost);
				}

				// Remote port label:
				if (Enabled && ((SocketTypeEx)this.socketType).IsUdp)
					label_RemotePort.Text = "Remote UDP port:";
				else
					label_RemotePort.Text = "Remote TCP port:";

				// Remote port:
				if (!DesignMode && Enabled && ((this.socketType == SocketType.TcpClient) || (this.socketType == SocketType.TcpAutoSocket)))
				{
					comboBox_RemotePort.Enabled = true;
					comboBox_RemotePort.Text = this.remoteTcpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
				}
				else if (!DesignMode && Enabled && ((this.socketType == SocketType.UdpClient) || (this.socketType == SocketType.UdpPairSocket)))
				{
					comboBox_RemotePort.Enabled = true;
					comboBox_RemotePort.Text = this.remoteUdpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
				}
				else
				{
					comboBox_RemotePort.Enabled = false;
					comboBox_RemotePort.Text = "";
				}

				// Local interface:
				comboBox_LocalInterface.Visible = true;
				comboBox_LocalInterface.Enabled = true;
				if (comboBox_LocalInterface.Items.Count > 0)
				{
					if (this.socketType != SocketType.UdpClient)
					{
						if (this.localInterface != null)
							comboBox_LocalInterface.SelectedItem = this.localInterface;
						else
							comboBox_LocalInterface.SelectedItem = (IPNetworkInterfaceEx)IPNetworkInterfaceEx.Default;
					}
					else
					{
						comboBox_LocalInterface.Enabled = false;
						comboBox_LocalInterface.SelectedItem = (IPNetworkInterfaceEx)IPNetworkInterfaceEx.Default;
					}
				}
				else // Note that 'SelectionHelper' is not used for this 'DropDownList'-style ComboBox.
				{
					comboBox_LocalInterface.SelectedIndex = ControlEx.InvalidIndex;
				}

				button_RefreshLocalInterfaces.Visible = true;
				button_RefreshLocalInterfaces.Enabled = true;

				// Local port label:
				if (Enabled && ((SocketTypeEx)this.socketType).IsUdp)
					label_LocalPort.Text = "Local UDP port:";
				else
					label_LocalPort.Text = "Local TCP port:";

				// Local port:
				if (Enabled && ((this.socketType == SocketType.TcpServer) || (this.socketType == SocketType.TcpAutoSocket)))
				{
					comboBox_LocalPort.Enabled = true;
					comboBox_LocalPort.Text = this.localTcpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
				}
				else if (Enabled && ((this.socketType == SocketType.UdpServer) || (this.socketType == SocketType.UdpPairSocket)))
				{
					comboBox_LocalPort.Enabled = true;
					comboBox_LocalPort.Text = this.localUdpPort.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for TCP and UDP ports!
				}
				else
				{
					comboBox_LocalPort.Enabled = false;
					comboBox_LocalPort.Text = "";
				}

				// Local filter:
				if (!DesignMode && Enabled && ((SocketTypeEx)this.socketType).IsUdp)
				{
					label_LocalFilter.Visible = true;
					comboBox_LocalFilter.Visible = true;
					switch (this.socketType)
					{
						case SocketType.UdpClient:
						{
							comboBox_LocalFilter.Enabled = false;
							ComboBoxHelper.Select(comboBox_LocalFilter, (IPFilterEx)IPFilterEx.Default);
							break;
						}

						case SocketType.UdpServer:
						{
							comboBox_LocalFilter.Enabled = true;

							if (this.localFilter != null)
								ComboBoxHelper.Select(comboBox_LocalFilter, this.localFilter, this.localFilter);
							else
								ComboBoxHelper.Select(comboBox_LocalFilter, (IPFilterEx)IPFilterEx.Default);

							break;
						}

						case SocketType.UdpPairSocket:
						{
							comboBox_LocalFilter.Enabled = false;
							ComboBoxHelper.Select(comboBox_LocalFilter, (IPFilterEx)IPFilterEx.Default);
							break;
						}

						default:
						{
							throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.socketType + "' is invalid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
					}
				}
				else
				{
					label_LocalFilter.Visible = false;
					comboBox_LocalFilter.Visible = false;
					comboBox_LocalFilter.Enabled = false;
					ComboBoxHelper.Deselect(comboBox_LocalFilter);
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnRemoteHostChanged(EventArgs e)
		{
			EventHelper.RaiseSync(RemoteHostChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRemoteTcpPortChanged(EventArgs e)
		{
			EventHelper.RaiseSync(RemoteTcpPortChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRemoteUdpPortChanged(EventArgs e)
		{
			EventHelper.RaiseSync(RemoteUdpPortChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnLocalInterfaceChanged(EventArgs e)
		{
			EventHelper.RaiseSync(LocalInterfaceChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnLocalFilterChanged(EventArgs e)
		{
			EventHelper.RaiseSync(LocalFilterChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnLocalTcpPortChanged(EventArgs e)
		{
			EventHelper.RaiseSync(LocalTcpPortChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnLocalUdpPortChanged(EventArgs e)
		{
			EventHelper.RaiseSync(LocalUdpPortChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
