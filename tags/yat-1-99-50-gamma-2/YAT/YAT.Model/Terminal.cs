﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Recent;
using MKY.Settings;
using MKY.Text;
using MKY.Time;

using YAT.Application.Utilities;
using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Terminal;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Terminals (.yat) of the YAT application model.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Terminal : IDisposable, IGuidProvider
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string TerminalText = "Terminal";

		/// <summary>
		/// Static counter to number terminals. Counter is incremented before first use, first
		/// terminal therefore is "Terminal1".
		/// </summary>
		private const int SequentialIndexCounterDefault = (Indices.FirstSequentialIndex - 1);

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticSequentialIndexCounter = SequentialIndexCounterDefault;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <remarks>
		/// 'StyleCop' asks to remove this static constructor. But 'Code Analysis' (FxCop) requires
		/// the suppressions at this static constructor. So what...
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.Maintainability", "SA1409:RemoveUnnecessaryCode", Justification = "See below ;-) But unfortunately it seems that StyleCop doesn't allow a suppression at the constructor itself. So what...")]
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Well, kind of a code analysis deadlock ;-)")]
		static Terminal()
		{
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <remarks>
		/// Needed to test the indices feature of terminals and workspace.
		/// </remarks>
		public static void ResetSequentialIndexCounter()
		{
			staticSequentialIndexCounter = SequentialIndexCounterDefault;
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private TerminalStartArgs startArgs;
		private Guid guid;
		private int sequentialIndex;
		private string autoName;

		// Settings:
		private DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler;
		private TerminalSettingsRoot settingsRoot;

		// Terminal:
		private Domain.Terminal terminal;

		// Logs:
		private Log.Provider log;

		// AutoResponse:
		private AutoResponseHelper autoResponseHelper;
		private object autoResponseHelperSyncObj = new object();

		// Time status:
		private Chronometer activeConnectChrono;
		private Chronometer totalConnectChrono;

		// Count status:
		private int txByteCount;
		private int rxByteCount;
		private int txLineCount;
		private int rxLineCount;

		// Rate status:
		private RateProvider txByteRate;
		private RateProvider rxByteRate;
		private RateProvider txLineRate;
		private RateProvider rxLineRate;

		// Partial commands:
		private string partialCommandLine;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;

		/// <summary></summary>
		public event EventHandler IOControlChanged;

		/// <summary></summary>
		public event EventHandler<TimeSpanEventArgs> IOConnectTimeChanged;

		/// <summary></summary>
		public event EventHandler IOCountChanged;

		/// <summary></summary>
		public event EventHandler IORateChanged;

		/// <summary></summary>
		public event EventHandler<Domain.IOErrorEventArgs> IOError;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsSent;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsReceived;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesSent;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesReceived;

		/// <summary></summary>
		public event EventHandler<EventArgs<Domain.RepositoryType>> RepositoryCleared;

		/// <summary></summary>
		public event EventHandler<EventArgs<Domain.RepositoryType>> RepositoryReloaded;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<DialogEventArgs> SaveAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<Cursor>> CursorRequest;

		/// <summary></summary>
		public event EventHandler<SavedEventArgs> Saved;

		/// <summary></summary>
		public event EventHandler<ClosedEventArgs> Closed;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Terminal()
			: this(new TerminalSettingsRoot())
		{
		}

		/// <summary></summary>
		public Terminal(TerminalSettingsRoot settings)
			: this(new DocumentSettingsHandler<TerminalSettingsRoot>(settings))
		{
		}

		/// <summary></summary>
		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
			: this(new TerminalStartArgs(), settingsHandler)
		{
		}

		/// <summary></summary>
		public Terminal(TerminalStartArgs startArgs, DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
			: this(startArgs, settingsHandler, Guid.Empty)
		{
		}

		/// <remarks><see cref="Guid.Empty"/> cannot be used as default argument as it is read-only.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public Terminal(TerminalStartArgs startArgs, DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid)
		{
			try
			{
				DebugMessage("Creating...");

				this.startArgs = startArgs;

				if (guid != Guid.Empty)
					this.guid = guid;
				else
					this.guid = Guid.NewGuid();

				// Link and attach to settings:
				this.settingsHandler = settingsHandler;
				this.settingsRoot = this.settingsHandler.Settings;
				this.settingsRoot.ClearChanged();
				AttachSettingsEventHandlers();

				// Set ID and user name:
				this.sequentialIndex = ++staticSequentialIndexCounter;
				if (!this.settingsHandler.SettingsFilePathIsValid || this.settingsRoot.AutoSaved)
					this.autoName = TerminalText + this.sequentialIndex.ToString(CultureInfo.CurrentCulture);
				else
					AutoNameFromFile = this.settingsHandler.SettingsFilePath;

				// Create underlying terminal:
				this.terminal = Domain.TerminalFactory.CreateTerminal(this.settingsRoot.Terminal);
				AttachTerminalEventHandlers();

				// Create log:
				this.log = new Log.Provider(this.settingsRoot.Log, (EncodingEx)this.settingsRoot.TextTerminal.Encoding, this.settingsRoot.Format);

				// Create AutoResponse:
				CreateAutoResponse();

				// Create chronos:
				CreateChronos();

				// Create rates:
				CreateRates();

				DebugMessage("...successfully created.");
			}
			catch (Exception ex)
			{
				DebugMessage("...failed!");
				DebugEx.WriteException(GetType(), ex);

				Dispose(); // Immediately call Dispose() to ensure no zombies remain!
				throw; // Re-throw!
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			DebugEventManagement.DebugNotifyAllEventRemains(this);

			if (!this.isDisposed)
			{
				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, terminal and log have already been closed, otherwise...

					// ...detach event handlers to ensure that no more events are received...
					DetachTerminalEventHandlers();

					// ...ensure that timed objects are stopped and do not fire events anymore...
					DisposeRates();
					DisposeChronos();
					DisposeAutoResponse();

					// ...close and dispose of terminal and log...
					CloseAndDisposeTerminal();
					DisposeLog();

					// ...and finally dispose of the settings:
					DetachSettingsEventHandlers();
					DisposeSettingsHandler();
				}

				// Set state to disposed:
				this.isDisposed = true;

				DebugMessage("...successfully disposed.");
			}
		}

#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Terminal()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

#endif // DEBUG

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region General Properties
		//==========================================================================================
		// General Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual Guid Guid
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.guid);
			}
		}

		/// <summary></summary>
		public virtual int SequentialIndex
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.sequentialIndex);
			}
		}

		/// <summary>
		/// This is the automatically assigned terminal name. The name is either an incrementally
		/// assigned 'Terminal1', 'Terminal2',... or the file name once the terminal has been saved
		/// by the user, e.g. 'MyTerminal.yat'.
		/// </summary>
		public virtual string AutoName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.autoName);
			}
		}

		private string AutoNameFromFile
		{
			set
			{
				this.autoName = Path.GetFileName(value);
			}
		}

		/// <summary></summary>
		public virtual TerminalStartArgs StartArgs
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.startArgs);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsStopped);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsStarted);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsOpen);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsConnected);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsTransmissive
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsTransmissive);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsReadyToSend);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsBusy
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsBusy);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool LogIsOn
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.log.IsOn);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool LogFileExists
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.log.FileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.UnderlyingIOInstance);
			}
		}

		/// <summary></summary>
		public virtual string Caption
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				StringBuilder sb = new StringBuilder();

				if (this.settingsRoot == null)
				{
					sb.Append("[");
					sb.Append(AutoName);
					sb.Append("]");
				}
				else
				{
					sb.Append("[");

					if (this.settingsHandler.SettingsFileIsReadOnly)
						sb.Append("#");

					sb.Append(AutoName);

					if (this.settingsHandler.SettingsFileIsReadOnly)
						sb.Append("#");

					if (this.settingsRoot.ExplicitHaveChanged)
						sb.Append(" *");

					sb.Append("]");

					string userName = this.settingsRoot.UserName;
					if (!string.IsNullOrEmpty(userName))
					{
						sb.Append(" - ");
						sb.Append(userName);
					}

					switch (this.settingsRoot.IOType)
					{
						case Domain.IOType.SerialPort:
						{
							MKY.IO.Serial.SerialPort.SerialPortSettings s = this.settingsRoot.IO.SerialPort;
							sb.Append(" - ");
							sb.Append(s.PortId.ToString(true, false));
							sb.Append(" - ");
							if (IsStarted)
							{
								if (IsOpen)
								{
									sb.Append("Open");
									sb.Append(" - ");
									sb.Append(IsConnected ? "Connected" : "Disconnected"); // Break?
								}
								else if (s.AutoReopen.Enabled)
								{
									sb.Append("Closed - Waiting for reconnect");
								}
								else
								{
									sb.Append("Closed");
								}
							}
							else
							{
								sb.Append("Closed");
							}
							break;
						}

						case Domain.IOType.TcpClient:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;

							sb.Append(" - ");
							sb.Append(s.RemoteEndPointString);
							sb.Append(" - ");

							if (IsConnected)
								sb.Append("Connected");
							else if (IsStarted && s.TcpClientAutoReconnect.Enabled)
								sb.Append("Disconnected - Waiting for reconnect");
							else
								sb.Append("Disconnected");

							break;
						}

						case Domain.IOType.TcpServer:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;

							sb.Append(" - ");
							sb.Append("Server:");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" - ");

							if (IsStarted)
								sb.Append(IsConnected ? "Connected" : "Listening");
							else
								sb.Append("Closed");

							break;
						}

						case Domain.IOType.TcpAutoSocket:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							if (IsStarted)
							{
								bool isClient = false;
								bool isServer = false;

								var socket = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpAutoSocket);
								if (socket != null)
								{
									isClient = socket.IsClient;
									isServer = socket.IsServer;
								}

								if (isClient)
								{
									sb.Append(" - ");
									sb.Append(s.RemoteEndPointString);
									sb.Append(" - ");
									sb.Append(IsConnected ? "Connected" : "Disconnected");
								}
								else if (isServer)
								{
									sb.Append(" - ");
									sb.Append("Server:");
									sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
									sb.Append(" - ");
									sb.Append(IsConnected ? "Connected" : "Listening");
								}
								else
								{
									sb.Append(" - ");
									sb.Append("Starting on port ");
									sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								}
							}
							else
							{
								sb.Append(" - ");
								sb.Append("AutoSocket:");
								sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								sb.Append(" - ");
								sb.Append("Disconnected");
							}
							break;
						}

						case Domain.IOType.UdpClient:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append(" - ");
							sb.Append(s.RemoteEndPointString);
							sb.Append(" - ");
							sb.Append(IsOpen ? "Open" : "Closed");
							break;
						}

						case Domain.IOType.UdpServer:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append(" - ");
							sb.Append("Receive:");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" - ");
							sb.Append(IsOpen ? "Open" : "Closed");
							break;
						}

						case Domain.IOType.UdpPairSocket:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append(" - ");
							sb.Append(s.RemoteEndPointString);
							sb.Append(" - ");
							sb.Append("Receive:");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" - ");
							sb.Append(IsOpen ? "Open" : "Closed");
							break;
						}

						case Domain.IOType.UsbSerialHid:
						{
							MKY.IO.Serial.Usb.SerialHidDeviceSettings s = this.settingsRoot.IO.UsbSerialHidDevice;
							sb.Append(" - ");
							var device = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Usb.SerialHidDevice);
							if (device != null)
								sb.Append(device.DeviceInfoString);
							else
								s.DeviceInfo.ToString(true, false);

							sb.Append(" - ");

							if (IsStarted)
							{
								if (IsConnected)
								{
									if (IsOpen)
										sb.Append("Connected - Open");
									else
										sb.Append("Connected - Closed");
								}
								else if (device.Settings.AutoOpen)
								{
									sb.Append("Disconnected - Waiting for reconnect");
								}
								else
								{
									sb.Append("Disconnected - Closed");
								}
							}
							else
							{
								sb.Append("Closed");
							}

							break;
						}

						default:
						{
							throw (new NotSupportedException("Program execution should never get here, '" + this.settingsRoot.IOType + "' is an invalid I/O type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
					} // switch (I/O type)
				} // if (settings available)

				return (sb.ToString());
			}
		}

		/// <summary></summary>
		public virtual string IOStatusText
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				StringBuilder sb = new StringBuilder();

				if (this.settingsRoot != null)
				{
					switch (this.settingsRoot.IOType)
					{
						case Domain.IOType.SerialPort:
						{
							MKY.IO.Serial.SerialPort.SerialPortSettings s = this.settingsRoot.IO.SerialPort;
							sb.Append("Serial port ");
							sb.Append(s.PortId.ToString(true, false));
							sb.Append(" (" + s.Communication + ")");
							if (IsStarted)
							{
								if (IsOpen)
								{
									sb.Append(" is open and ");
									sb.Append(IsConnected ? "connected" : "disconnected");
								}
								else if (s.AutoReopen.Enabled)
								{
									sb.Append(" is closed and waiting for reconnect");
								}
								else
								{
									sb.Append(" is closed");
								}
							}
							else
							{
								sb.Append(" is closed");
							}
							break;
						}

						case Domain.IOType.TcpClient:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append("TCP/IP client");

							if (IsConnected)
								sb.Append(" is connected to ");
							else if (IsStarted && s.TcpClientAutoReconnect.Enabled)
								sb.Append(" is disconnected and waiting for reconnect to ");
							else
								sb.Append(" is disconnected from ");

							sb.Append(s.RemoteEndPointString);
							break;
						}

						case Domain.IOType.TcpServer:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append("TCP/IP server");
							if (IsStarted)
							{
								if (IsConnected)
								{
									int count = 0;

									var server = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpServer);
									if (server != null)
										count = server.ConnectedClientCount;

									sb.Append(" is connected");
									if (count == 1)
									{
										sb.Append(" to a client");
									}
									else
									{
										sb.Append(" to");
										sb.Append(count.ToString(CultureInfo.CurrentCulture));
										sb.Append(" clients");
									}
								}
								else
								{
									sb.Append(" is listening");
								}
							}
							else
							{
								sb.Append(" is closed");
							}
							sb.Append(" on local port ");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							break;
						}

						case Domain.IOType.TcpAutoSocket:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append("TCP/IP AutoSocket");
							if (IsStarted)
							{
								bool isClient = false;
								bool isServer = false;

								var socket = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpAutoSocket);
								if (socket != null)
								{
									isClient = socket.IsClient;
									isServer = socket.IsServer;
								}

								if (isClient)
								{
									sb.Append(" is connected to ");
									sb.Append(s.RemoteEndPointString);
								}
								else if (isServer)
								{
									sb.Append(IsConnected ? " is connected" : " is listening");
									sb.Append(" on local port ");
									sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								}
								else
								{
									sb.Append(" is starting to connect to ");
									sb.Append(s.RemoteEndPointString);
								}
							}
							else
							{
								sb.Append(" is disconnected from ");
								sb.Append(s.RemoteEndPointString);
							}
							break;
						}

						case Domain.IOType.UdpClient:
						{
							sb.Append("UDP/IP client");
							if (IsOpen)
							{
								sb.Append(" is open");
								var socket = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.UdpSocket);
								if ((socket != null) && (socket.SocketType == MKY.IO.Serial.Socket.UdpSocketType.Client))
								{
									sb.Append(" for sending to ");
									sb.Append(socket.RemoteEndPoint.ToString());

									int localPort = socket.LocalPort;
									if (localPort != 0)
									{
										sb.Append(" and receiving on local port ");
										sb.Append(localPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
									}
								}
							}
							else
							{
								sb.Append(" is closed");
							}
							break;
						}

						case Domain.IOType.UdpServer:
						{
							sb.Append("UDP/IP server is ");
							if (IsOpen)
							{
								sb.Append(" is open");
								var socket = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.UdpSocket);
								if ((socket != null) && (socket.SocketType == MKY.IO.Serial.Socket.UdpSocketType.Server))
								{
									sb.Append(" for receiving on local port ");
									sb.Append(socket.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!

									System.Net.IPEndPoint remoteEndPoint = socket.RemoteEndPoint;
									if ((remoteEndPoint != null) && (!remoteEndPoint.Address.Equals(System.Net.IPAddress.None))) // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
									{
										sb.Append(" and sending to ");
										sb.Append(socket.RemoteEndPoint.ToString());
									}
								}
							}
							else
							{
								sb.Append(" is closed");
							}

							break;
						}

						case Domain.IOType.UdpPairSocket:
						{
							sb.Append("UDP/IP PairSocket");
							if (IsOpen)
							{
								sb.Append(" is open");

								var socket = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.UdpSocket);
								if ((socket != null) && (socket.SocketType == MKY.IO.Serial.Socket.UdpSocketType.PairSocket))
								{
									sb.Append(" for sending to ");
									sb.Append(socket.RemoteEndPoint.ToString());
									sb.Append(" and receiving on local port ");
									sb.Append(socket.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								}
							}
							else
							{
								sb.Append(" is closed");
							}
							break;
						}

						case Domain.IOType.UsbSerialHid:
						{
							MKY.IO.Serial.Usb.SerialHidDeviceSettings s = this.settingsRoot.IO.UsbSerialHidDevice;
							sb.Append("USB HID device '");

							var device = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Usb.SerialHidDevice);
							if (device != null)
								sb.Append(device.DeviceInfoString);
							else
								s.DeviceInfo.ToString(true, false);

							sb.Append("'");

							if (IsStarted)
							{
								if (IsConnected)
								{
									if (IsOpen)
										sb.Append(" is connected and open");
									else
										sb.Append(" is connected and closed");
								}
								else if (device.Settings.AutoOpen)
								{
									sb.Append(" is disconnected and waiting for reconnect");
								}
								else
								{
									sb.Append(" is disconnected and closed");
								}
							}
							else
							{
								sb.Append(" is closed");
							}

							break;
						}

						default:
						{
							// Do nothing.
							break;
						}
					}
				}

				return (sb.ToString());
			}
		}

		#endregion

		#region General Methods
		//==========================================================================================
		// General Methods
		//==========================================================================================

		/// <summary>
		/// Starts terminal, i.e. starts log and opens I/O.
		/// </summary>
		public virtual bool Start()
		{
			AssertNotDisposed();

			// Switch log on if selected:
			if (this.settingsRoot.LogIsOn)
			{
				if (!SwitchLogOn())
					return (false);
			}

			// Then start terminal if selected:
			if (this.settingsRoot.TerminalIsStarted)
			{
				// Check availability of I/O before starting:
				if (!CheckIOAvailability())
					return (false);

				if (!StartIO())
					return (false);
			}

			return (true);
		}

		/// <summary>
		/// Sets terminal settings.
		/// </summary>
		public virtual void SetSettings(ExplicitSettings settings)
		{
			AssertNotDisposed();

			// Settings have changed, recreate terminal with new settings:
			if (this.terminal.IsStarted)
			{
				// \todo
				// The code below could be improved such that the I/O is only restarted/recreated
				// in case the I/O settings have changed (but not e.g. 'ShowPort'). This could be
				// done along with FR#251 "Migrate settings to tabbed dialog".

				// Terminal is open, close and re-open it with the new settings:
				if (StopIO(false))
				{
					this.settingsRoot.SuspendChangeEvent();
					this.settingsRoot.Explicit = settings;

					DetachTerminalEventHandlers();
					Domain.Terminal oldTerminal = this.terminal;
					this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
					oldTerminal.Dispose();
					AttachTerminalEventHandlers();

					this.settingsRoot.ResumeChangeEvent();
					this.terminal.ReloadRepositories();

					if (StartIO(false))
						OnTimedStatusTextRequest("Terminal settings applied.");
					else
						OnFixedStatusTextRequest("Terminal settings applied but terminal could not be started anymore.");
				}
				else
				{
					OnTimedStatusTextRequest("Terminal settings not applied!");
				}
			}
			else
			{
				// Terminal is closed, simply set the new settings:
				this.settingsRoot.SuspendChangeEvent();
				this.settingsRoot.Explicit = settings;

				DetachTerminalEventHandlers();
				Domain.Terminal oldTerminal = this.terminal;
				this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
				oldTerminal.Dispose();
				AttachTerminalEventHandlers();

				this.settingsRoot.ResumeChangeEvent();
				this.terminal.ReloadRepositories();

				OnTimedStatusTextRequest("Terminal settings applied.");
			}
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		#region Settings > Lifetime
		//------------------------------------------------------------------------------------------
		// Settings > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed += settingsRoot_Changed;
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed -= settingsRoot_Changed;
		}

		private void DisposeSettingsHandler()
		{
			if (this.settingsHandler != null)
			{
				this.settingsHandler.Dispose();
				this.settingsHandler = null;
			}
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'terminalTypeOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.TerminalType settingsRoot_Changed_terminalTypeOld = Domain.Settings.TerminalSettings.TerminalTypeDefault;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'endianessOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.Endianness settingsRoot_Changed_endianessOld = Domain.Settings.IOSettings.EndiannessDefault;

		/// <remarks>
		/// Required to solve the issue described in bug #223 "Settings events should state the exact settings diff".
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'sendImmediatelyOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool settingsRoot_Changed_sendImmediatelyOld = Domain.Settings.SendSettings.SendImmediatelyDefault;

		private void settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			// Note that view settings are handled in View.Forms.Terminal::settingsRoot_Changed().
			// Below, only those settings that need to be managed by the model are handled.

			if (e.Inner == null)
			{
				// Nothing to do, no need to care about 'ProductVersion' and such.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Explicit))
			{
				HandleExplicitSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Implicit))
			{
				HandleImplicitSettings(e.Inner);
			}
		}

		private void HandleExplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// Nothing to do.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal))
			{
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.PredefinedCommand))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoResponse))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Format))
			{
				this.log.NeatFormat = this.settingsRoot.Format;
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Log))
			{
				this.log.Settings = this.settingsRoot.Log;
			}
		}

		private void HandleTerminalSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				if (settingsRoot_Changed_terminalTypeOld != this.settingsRoot.TerminalType) {
					settingsRoot_Changed_terminalTypeOld = this.settingsRoot.TerminalType;

					// Terminal type has changed, recreate the auto response:
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.IO))
			{
				if (settingsRoot_Changed_endianessOld != this.settingsRoot.IO.Endianness) {
					settingsRoot_Changed_endianessOld = this.settingsRoot.IO.Endianness;

					// Endianess has changed, recreate the auto response:
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Send))
			{
				if (settingsRoot_Changed_sendImmediatelyOld != this.settingsRoot.Send.SendImmediately) {
					settingsRoot_Changed_sendImmediatelyOld = this.settingsRoot.Send.SendImmediately;

					// Send immediately has changed, reset the last command:
					this.settingsRoot.SendText.Command.Clear();
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.TextTerminal))
			{
				this.log.TextTerminalEncoding = (EncodingEx)this.settingsRoot.TextTerminal.Encoding;

				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
		}

		private void HandleImplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// Nothing to do.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.SendText))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.SendFile))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
		}

		#endregion

		#region Settings > Properties
		//------------------------------------------------------------------------------------------
		// Settings > Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string SettingsFilePath
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFilePath);
				else
					return (null);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileExists
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsWritable
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileIsWritable);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileHasAlreadyBeenNormallySaved
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileSuccessfullyLoaded && !this.settingsRoot.AutoSaved);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileNoLongerExists
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (SettingsFileHasAlreadyBeenNormallySaved && !SettingsFileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual TerminalSettingsRoot SettingsRoot
		{
			get
			{
				// Do not call AssertNotDisposed() to still allow reading the settings after the
				// terminal has been disposed. This is required for certain test cases.

				return (this.settingsRoot);
			}
		}

		#endregion

		#endregion

		#region Save
		//==========================================================================================
		// Save
		//==========================================================================================

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet.
		/// </summary>
		public virtual bool Save()
		{
			return (Save(true, true));
		}

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet.
		/// </summary>
		/// <param name="autoSaveIsAllowed">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="userInteractionIsAllowed">Indicates whether user interaction is allowed.</param>
		public virtual bool Save(bool autoSaveIsAllowed, bool userInteractionIsAllowed)
		{
			AssertNotDisposed();

			return (SaveDependentOnState(autoSaveIsAllowed, userInteractionIsAllowed));
		}

		/// <summary>
		/// This method implements the logic that is needed when saving, opposed to the method
		/// <see cref="SaveToFile"/> which just performs the actual save, i.e. file handling.
		/// </summary>
		/// <param name="autoSaveIsAllowed">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="userInteractionIsAllowed">Indicates whether user interaction is allowed.</param>
		private bool SaveDependentOnState(bool autoSaveIsAllowed, bool userInteractionIsAllowed)
		{
			// Evaluate auto save.
			bool isAutoSave;
			if (this.settingsHandler.SettingsFilePathIsValid && !this.settingsRoot.AutoSaved)
				isAutoSave = false;
			else
				isAutoSave = autoSaveIsAllowed;

			// -------------------------------------------------------------------------------------
			// Skip auto save if there is no reason to save, in order to increase speed.
			// -------------------------------------------------------------------------------------

			if (isAutoSave && this.settingsHandler.SettingsFileIsUpToDate && !this.settingsRoot.HaveChanged)
			{
				// Event must be fired anyway to ensure that dependent objects are updated.
				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, isAutoSave));
				OnTimedStatusTextRequest("Terminal has no changes to be saved.");
				return (true);
			}

			// -------------------------------------------------------------------------------------
			// Create auto save file path or request manual/normal file path if necessary.
			// -------------------------------------------------------------------------------------

			if (!this.settingsHandler.SettingsFilePathIsValid)
			{
				if (isAutoSave)
				{
					StringBuilder autoSaveFilePath = new StringBuilder();

					autoSaveFilePath.Append(Application.Settings.GeneralSettings.AutoSaveRoot);
					autoSaveFilePath.Append(Path.DirectorySeparatorChar);
					autoSaveFilePath.Append(Application.Settings.GeneralSettings.AutoSaveTerminalFileNamePrefix);
					autoSaveFilePath.Append(Guid.ToString());
					autoSaveFilePath.Append(ExtensionHelper.TerminalFile);

					this.settingsHandler.SettingsFilePath = autoSaveFilePath.ToString();
				}
				else if (userInteractionIsAllowed)
				{
					// This Save As... request will request the file path from the user and then call
					// the 'SaveAs()' method below.
					switch (OnSaveAsFileDialogRequest())
					{
						case DialogResult.OK:
						case DialogResult.Yes:
							return (true);

						case DialogResult.No:
							OnTimedStatusTextRequest("Terminal not saved!");
							return (true);

						default:
							return (false);
					}
				}
				else
				{
					// Let save fail if the file path is invalid and no user interaction is allowed.
					return (false);
				}
			}
			else // SettingsFilePathIsValid
			{
				if (userInteractionIsAllowed)
				{
					// Ensure that existing former auto files are 'Saved As' if this is no auto save.
					if (this.settingsRoot.AutoSaved && !isAutoSave)
					{
						// This Save As... request will request the file path from the user and then call
						// the 'SaveAs()' method below.
						switch (OnSaveAsFileDialogRequest())
						{
							case DialogResult.OK:
							case DialogResult.Yes:
								return (true);

							case DialogResult.No:
								OnTimedStatusTextRequest("Terminal not saved!");
								return (true);

							default:
								return (false);
						}
					}

					// Ensure that normal files which are write-protected or no longer exist are 'Saved As'.
					if (!SettingsFileIsWritable || SettingsFileNoLongerExists)
					{
						string reason;
						if (!SettingsFileIsWritable)
							reason = "The file is write-protected.";
						else
							reason = "The file no longer exists.";

						string message =
							"Unable to save file" + Environment.NewLine + this.settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
							reason + " Would you like to save the file at another location or cancel?";

						DialogResult dr = OnMessageInputRequest
						(
							message,
							"File Error",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question
						);

						switch (dr)
						{
							case DialogResult.Yes:
								switch (OnSaveAsFileDialogRequest())
								{
									case DialogResult.OK:
									case DialogResult.Yes:
										return (true);

									case DialogResult.No:
										OnTimedStatusTextRequest("Terminal not saved!");
										return (true);

									default:
										return (false);
								}

							case DialogResult.No:
								OnTimedStatusTextRequest("Terminal not saved!");
								return (true);

							default:
								OnTimedStatusTextRequest("Cancelled!");
								return (false);
						}
					}
				}
			}

			// -------------------------------------------------------------------------------------
			// Save if allowed so.
			// -------------------------------------------------------------------------------------

			if (this.settingsHandler.SettingsFileIsWritable)
				return (SaveToFile(isAutoSave, null));
			else
				return (false); // Let save fail if file shall not be written.
		}

		/// <summary>
		/// Saves settings to given file.
		/// </summary>
		public virtual bool SaveAs(string filePath)
		{
			AssertNotDisposed();

			// Request the deletion of the obsolete auto saved settings file given the new file is different:
			string autoSaveFilePathToDelete = null;
			if (this.settingsRoot.AutoSaved && (!StringEx.EqualsOrdinalIgnoreCase(filePath, this.settingsHandler.SettingsFilePath)))
				autoSaveFilePathToDelete = this.settingsHandler.SettingsFilePath;

			// Set the new file path:
			this.settingsHandler.SettingsFilePath = filePath;

			return (SaveToFile(false, autoSaveFilePathToDelete));
		}

		/// <param name="isAutoSave">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="autoSaveFilePathToDelete">
		/// The path to the former auto saved file, it will be deleted if the file can successfully
		/// be stored in the new location.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private bool SaveToFile(bool isAutoSave, string autoSaveFilePathToDelete)
		{
			OnFixedStatusTextRequest("Saving terminal...");

			bool success = false;

			try
			{
				this.settingsHandler.Settings.AutoSaved = isAutoSave;
				this.settingsHandler.Save();
				success = true;

				if (!isAutoSave)
					AutoNameFromFile = this.settingsHandler.SettingsFilePath;

				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, isAutoSave));
				OnTimedStatusTextRequest("Terminal saved.");

				if (!isAutoSave)
					SetRecent(this.settingsHandler.SettingsFilePath);

				// Try to delete existing auto save file:
				if (!string.IsNullOrEmpty(autoSaveFilePathToDelete))
				{
					// Ensure that this is not the current file!
					if (!StringEx.EqualsOrdinalIgnoreCase(autoSaveFilePathToDelete, this.settingsHandler.SettingsFilePath))
						FileEx.TryDelete(autoSaveFilePathToDelete);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Error saving terminal!");

				OnFixedStatusTextRequest("Error saving terminal!");
				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to save terminal file", this.settingsHandler.SettingsFilePath, ex),
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest("Terminal not saved!");
			}

			return (success);
		}

		#endregion

		#region Close
		//==========================================================================================
		// Close
		//==========================================================================================

		/// <summary>
		/// Closes the terminal and prompts if needed if settings have changed.
		/// </summary>
		/// <remarks>
		/// In case of a workspace close, <see cref="Close(bool, bool, bool, bool)"/> below must be called
		/// with the first argument set to <c>true</c>.
		/// 
		/// In case of intended close of one or all terminals, the user intentionally wants to close
		/// the terminals, thus, this method will not try to auto save.
		/// </remarks>
		public virtual bool Close()
		{
			return (Close(false, true, false, true)); // See remarks above.
		}

		/// <summary>
		/// Closes the terminal and tries to auto save if desired.
		/// </summary>
		/// <remarks>
		/// Attention:
		/// This method is needed for MDI applications. In case of MDI parent/application closing,
		/// Close() of the terminal is called before Close() of the workspace. Without taking care
		/// of this, the workspace would be saved after the terminal has already been closed, i.e.
		/// removed from the workspace. Therefore, the terminal has to signal such cases to the
		/// workspace.
		/// 
		/// Cases (similar to cases in Model.Workspace):
		/// - Workspace close
		///   - auto,   no file,       auto save    => auto save, if it fails => nothing  : (w1a)
		///   - auto,   no file,       no auto save => nothing                            : (w1b)
		///   - auto,   existing file, auto save    => auto save, if it fails => delete   : (w2a)
		///   - auto,   existing file, no auto save => delete                             : (w2b)
		///   - normal, no file                     => N/A (normal files have been saved) : (w3)
		///   - normal, no file anymore             => question                           :  --
		///   - normal, existing file, auto save    => auto save, if it fails => question : (w4a)
		///   - normal, existing file, no auto save => question                           : (w4b)
		/// - Terminal close
		///   - auto,   no file                     => nothing                            : (t1)
		///   - auto,   existing file               => delete                             : (t2)
		///   - normal, no file                     => N/A (normal files have been saved) : (t3)
		///   - normal, no file anymore             => question                           :  --
		///   - normal, existing file, auto save    => auto save, if it fails => question : (t4a)
		///   - normal, existing file, no auto save => question                           : (t4b)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public virtual bool Close(bool isWorkspaceClose, bool doSave, bool autoSaveIsAllowed, bool autoDeleteIsRequested)
		{
			AssertNotDisposed();

			OnFixedStatusTextRequest("Closing terminal...");

			// Keep info of existing former auto file:
			bool formerExistingAutoFileAutoSaved = this.settingsRoot.AutoSaved;
			string formerExistingAutoFilePath = null;
			if (this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFileExists)
				formerExistingAutoFilePath = this.settingsHandler.SettingsFilePath;

			// -------------------------------------------------------------------------------------
			// Evaluate save requirements.
			// -------------------------------------------------------------------------------------

			bool success = false;

			// Do not try to auto save if save is not inteded at all.
			if (autoSaveIsAllowed && !doSave)
				autoSaveIsAllowed = false;

			// Do not neither try to auto save nor manually save if there is no existing file (w1, w3, t1, t3),
			// except in case of w1a, i.e. when the file has never been loaded so far.
			if (autoSaveIsAllowed && !this.settingsHandler.SettingsFileExists)
			{
				if (!isWorkspaceClose || this.settingsHandler.SettingsFileSuccessfullyLoaded)
				{
					doSave = false;
					autoSaveIsAllowed = false;
				}
			}

			if (isWorkspaceClose)
			{
				if (!this.settingsRoot.HaveChanged)
				{
					// Nothing has changed, no need to do anything.
					doSave = false;
					autoSaveIsAllowed = false;
					success = true;
				}
				else if (!this.settingsRoot.ExplicitHaveChanged)
				{
					// Implicit have changed, save is not required but try to auto save if desired.
					if (autoSaveIsAllowed)
						doSave = true;
					else
						success = true;
				}
				else
				{
					// Explicit have changed, save is required.
				}
			}
			else
			{
				if (!this.settingsRoot.HaveChanged)
				{
					// Nothing has changed, no need to do anything.
					doSave = false;
					autoSaveIsAllowed = false;
					success = true;
				}
				else if (!this.settingsRoot.ExplicitHaveChanged)
				{
					// Implicit have changed, but do not try to auto save since user intended to close.
					doSave = false;
					autoSaveIsAllowed = false;
					success = true;
				}
				else
				{
					// Explicit have changed, save is required.
				}
			}

			// -------------------------------------------------------------------------------------
			// Try auto save if allowed.
			// -------------------------------------------------------------------------------------

			if (!success && doSave)
				success = SaveDependentOnState(autoSaveIsAllowed, false); // Try auto save, i.e. no user interaction.

			// -------------------------------------------------------------------------------------
			// If not successfully saved so far, evaluate next step according to rules above.
			// -------------------------------------------------------------------------------------

			// Normal file (w3, w4, t3, t4):
			if (!success && doSave && !this.settingsRoot.AutoSaved)
			{
				DialogResult dr = OnMessageInputRequest
				(
					"Save terminal?",
					AutoName,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				);

				switch (dr)
				{                                    // Do normal/manual save.
					case DialogResult.Yes: success = SaveDependentOnState(true, true); break;
					case DialogResult.No:  success = true;                             break;

					default:
						success = false; break; // Also covers 'DialogResult.Cancel'.
				}
			}

			// Delete existing former auto file which is no longer needed (w2a):
			if (isWorkspaceClose && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null) && !success)
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				success = true;
			}

			// Delete existing former auto file which is no longer needed (w2b):
			if (isWorkspaceClose && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null) && autoDeleteIsRequested)
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				success = true;
			}

			// Delete existing former auto file which is no longer needed (t2):
			if (!isWorkspaceClose && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null) && autoDeleteIsRequested)
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				success = true;
			}

			// Write-protected file:
			if (!success && !this.settingsHandler.SettingsFileIsWritable)
			{
				success = true; // Consider it successful if file shall not be saved.
			}

			// No file (w1, t1):
			if (!success && !this.settingsHandler.SettingsFileExists)
			{
				this.settingsHandler.ResetSettingsFilePath();
				success = true; // Consider it successful if there was no file to save.
			}

			// -------------------------------------------------------------------------------------
			// Stop the underlying items.
			// -------------------------------------------------------------------------------------

			if (success && this.terminal.IsStarted)
			{
				success = StopIO(false);
			}

			if (success && this.log.IsOn)
			{
				SwitchLogOff();
			}

			// -------------------------------------------------------------------------------------
			// Finally, cleanup and signal state.
			// -------------------------------------------------------------------------------------

			if (success)
			{
				// Status text request must be before closed event, closed event may close the view:
				OnTimedStatusTextRequest("Terminal successfully closed.");

				OnClosed(new ClosedEventArgs(isWorkspaceClose));

				// The terminal shall dispose of itself to free all resources for sure. It must be
				// done AFTER it fired the 'Closed' event and all subscribers of the event may still
				// refer to a non-disposed object. This is especially important, as the order of the
				// subscribers is not fixed, i.e. Model.Workspace may dispose of the terminal
				// before View.Terminal receives the event callback!
				Dispose();

				return (true);
			}
			else
			{
				OnTimedStatusTextRequest("Terminal not closed.");

				return (false);
			}
		}

		#endregion

		#region Recent Files
		//==========================================================================================
		// Recent Files
		//==========================================================================================

		/// <summary>
		/// Update recent entry.
		/// </summary>
		/// <param name="recentFile">Recent file.</param>
		private static void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(recentFile);
			ApplicationSettings.LocalUserSettings.RecentFiles.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.Save();
		}

		#endregion

		#region Terminal
		//==========================================================================================
		// Terminal
		//==========================================================================================

		#region Terminal > Lifetime
		//------------------------------------------------------------------------------------------
		// Terminal > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged        += terminal_IOChanged;
				this.terminal.IOControlChanged += terminal_IOControlChanged;
				this.terminal.IOError          += terminal_IOError;

				this.terminal.RawChunkSent            += terminal_RawChunkSent;
				this.terminal.RawChunkReceived        += terminal_RawChunkReceived;
				this.terminal.DisplayElementsSent     += terminal_DisplayElementsSent;
				this.terminal.DisplayElementsReceived += terminal_DisplayElementsReceived;
				this.terminal.DisplayLinesSent        += terminal_DisplayLinesSent;
				this.terminal.DisplayLinesReceived    += terminal_DisplayLinesReceived;
				this.terminal.RepositoryCleared       += terminal_RepositoryCleared;
				this.terminal.RepositoryReloaded      += terminal_RepositoryReloaded;
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged        -= terminal_IOChanged;
				this.terminal.IOControlChanged -= terminal_IOControlChanged;
				this.terminal.IOError          -= terminal_IOError;

				this.terminal.RawChunkSent            -= terminal_RawChunkSent;
				this.terminal.RawChunkReceived        -= terminal_RawChunkReceived;
				this.terminal.DisplayElementsSent     -= terminal_DisplayElementsSent;
				this.terminal.DisplayElementsReceived -= terminal_DisplayElementsReceived;
				this.terminal.DisplayLinesSent        -= terminal_DisplayLinesSent;
				this.terminal.DisplayLinesReceived    -= terminal_DisplayLinesReceived;
				this.terminal.RepositoryCleared       -= terminal_RepositoryCleared;
				this.terminal.RepositoryReloaded      -= terminal_RepositoryReloaded;
			}
		}

		private void CloseAndDisposeTerminal()
		{
			if (this.terminal != null)
			{
				this.terminal.Close();
				this.terminal.Dispose();
				this.terminal = null;
			}
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Local field to maintain connection state in order to be able to detect a change of the
		/// connection state.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool terminal_IOChanged_hasBeenConnected;

		private void terminal_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(e);

			bool hasBeenConnected = this.terminal_IOChanged_hasBeenConnected;
			bool isConnectedNow   = this.terminal.IsConnected;

			if      ( isConnectedNow && !hasBeenConnected)
			{
				this.activeConnectChrono.Restart();
				this.totalConnectChrono.Start();
			}
			else if (!isConnectedNow &&  hasBeenConnected)
			{
				this.activeConnectChrono.Stop();
				this.totalConnectChrono.Stop();
			}

			this.terminal_IOChanged_hasBeenConnected = this.terminal.IsConnected;
		}

		private void terminal_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			OnIOError(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RawChunkReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_RawChunkSent(object sender, EventArgs<Domain.RawChunk> e)
		{
			OnTimedStatusTextRequest("Sending...");

			// Count:
			this.txByteCount += e.Value.Data.Length;
			OnIOCountChanged(EventArgs.Empty);

			// Rate:
			if (this.txByteRate.Update(e.Value.Data.Length))
				OnIORateChanged(EventArgs.Empty);

			// Log:
			if (this.log.IsOn)
			{
				this.log.Write(e.Value, Log.LogChannel.RawTx);
				this.log.Write(e.Value, Log.LogChannel.RawBidir);
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RawChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_RawChunkReceived(object sender, EventArgs<Domain.RawChunk> e)
		{
			OnTimedStatusTextRequest("Receiving...");

			// Count:
			this.rxByteCount += e.Value.Data.Length;
			OnIOCountChanged(EventArgs.Empty);

			// Rate:
			if (this.rxByteRate.Update(e.Value.Data.Length))
				OnIORateChanged(EventArgs.Empty);

			// Log:
			if (this.log.IsOn)
			{
				this.log.Write(e.Value, Log.LogChannel.RawBidir);
				this.log.Write(e.Value, Log.LogChannel.RawRx);
			}

			// AutoRespose:
			if (this.settingsRoot.AutoResponse.IsActive)
			{
				bool isMatch = false;

				foreach (byte b in e.Value.Data)
				{
					lock (this.autoResponseHelperSyncObj)
					{
						if (this.autoResponseHelper != null)
						{
							if (this.autoResponseHelper.EnqueueAndMatch(b))
								isMatch = true;
						}
						else
						{
							break; // Break the for-loop if AutoResponse got disposed in the meantime.
						}
					}
				}

				if (isMatch) // Invoke sending on different thread than the receive thread.
				{
					VoidDelegateVoid asyncInvoker = new VoidDelegateVoid(terminal_RawChunkReceived_SendAutoResponseAsync);
					asyncInvoker.BeginInvoke(null, null);
				}
			}
		}

		private void terminal_RawChunkReceived_SendAutoResponseAsync()
		{
			SendAutoResponse();
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			OnDisplayElementsSent(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			OnDisplayElementsReceived(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count:
			this.txLineCount += e.Lines.Count;
			OnIOCountChanged(EventArgs.Empty);

			// Rate:
			if (this.txLineRate.Update(e.Lines.Count))
				OnIORateChanged(EventArgs.Empty);

			// Display:
			OnDisplayLinesSent(e);

			// Log:
			foreach (Domain.DisplayLine de in e.Lines)
			{
				if (this.log.IsOn)
				{
					this.log.WriteLine(de, Log.LogChannel.NeatTx);
					this.log.WriteLine(de, Log.LogChannel.NeatBidir);
				}
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count:
			this.rxLineCount += e.Lines.Count;
			OnIOCountChanged(EventArgs.Empty);

			// Rate:
			if (this.rxLineRate.Update(e.Lines.Count))
				OnIORateChanged(EventArgs.Empty);

			// Display:
			OnDisplayLinesReceived(e);

			// Log:
			foreach (Domain.DisplayLine de in e.Lines)
			{
				if (this.log.IsOn)
				{
					this.log.WriteLine(de, Log.LogChannel.NeatBidir);
					this.log.WriteLine(de, Log.LogChannel.NeatRx);
				}
			}

			// AutoRespose:
			if (this.settingsRoot.AutoResponse.IsActive && (this.settingsRoot.AutoResponse.Trigger == AutoTrigger.AnyLine))
				SendAutoResponse();
		}

		private void terminal_RepositoryCleared(object sender, EventArgs<Domain.RepositoryType> e)
		{
			OnRepositoryCleared(e);
		}

		private void terminal_RepositoryReloaded(object sender, EventArgs<Domain.RepositoryType> e)
		{
			OnRepositoryReloaded(e);
		}

		#endregion

		#region Terminal > Check I/O
		//------------------------------------------------------------------------------------------
		// Terminal > Check I/O
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks the terminal's I/O port availability. If I/O port is not available, user is asked
		/// whether to change to a different I/O port.
		/// </summary>
		/// <remarks>
		/// Note that only the availability of the I/O port is checked, not whether the port is
		/// already in use, because that may take quite some time and thus unnecessarily delay the
		/// open/check/start sequence.
		/// </remarks>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public virtual bool CheckIOAvailability()
		{
			OnFixedStatusTextRequest("Checking terminal...");

			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
				{
					MKY.IO.Ports.SerialPortId portId = this.settingsRoot.IO.SerialPort.PortId;
					if (portId != null)
					{
						MKY.IO.Ports.SerialPortCollection ports = new MKY.IO.Ports.SerialPortCollection();
						ports.FillWithAvailablePorts(false); // No need to get descriptions, thus faster.

						// Attention:
						// Similar code exists in View.Controls.SerialPortSelection.SetPortList().
						// Changes here may have to be applied there too!

						if (ports.Count > 0)
						{
							if (ports.Contains(portId))
							{
								return (true);
							}
							else
							{
								DialogResult dr = ShowSerialPortNotAvailableSwitchQuestion(portId, ports[0]);
								if (dr == DialogResult.Yes)
									this.settingsRoot.IO.SerialPort.PortId = ports[0];

								return (true); // Return 'true' in any case, device may not yet be available but 'AutoOpen'.
							}
						}
						else // ports.Count == 0
						{
							DialogResult dr = ShowNoSerialPortsStartAnywayQuestion(portId);
							return (dr == DialogResult.Yes);
						}
					}

					return (true); // Return 'true' in all 'non-handled' cases => still try to start I/O to force the underlying exception message.
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				{
					MKY.Net.IPNetworkInterfaceEx localInterface = this.settingsRoot.IO.Socket.LocalInterface;
					if (localInterface != null)
					{
						MKY.Net.IPNetworkInterfaceCollection localInterfaces = new MKY.Net.IPNetworkInterfaceCollection();
						localInterfaces.FillWithAvailableLocalInterfaces();

						// Attention:
						// Similar code exists in View.Controls.SocketSelection.SetLocalInterfaceList().
						// Changes here may have to be applied there too!

						if (localInterfaces.Count > 0)
						{
							if (localInterfaces.Contains(localInterface))
							{
								return (true);
							}
							else if (localInterfaces.ContainsDescription(localInterface))
							{
								// A device with same description is available, use that:
								int sameDescriptionIndex = localInterfaces.FindIndexDescription(localInterface);

								DialogResult dr = ShowLocalInterfaceNotAvailableAlternateQuestion(localInterface, localInterfaces[sameDescriptionIndex]);
								if (dr == DialogResult.Yes)
									this.settingsRoot.IO.Socket.LocalInterface = localInterfaces[sameDescriptionIndex];

								return (true); // Return 'true' in any case, device may not yet be available but 'AutoOpen'.
							}
							else
							{
								DialogResult dr = ShowLocalInterfaceNotAvailableDefaultQuestion(localInterface, localInterfaces[0]);
								if (dr == DialogResult.Yes)
								{
									this.settingsRoot.IO.Socket.LocalInterface = localInterfaces[0];
									return (true);
								}
								else
								{
									return (false);
								}
							}
						}
						else // localInterfaces.Count == 0
						{
							ShowNoLocalInterfacesMessage();
							return (false);
						}
					}

					return (true); // Return 'true' in all 'non-handled' cases => still try to start I/O to force the underlying exception message.
				}

				case Domain.IOType.UsbSerialHid:
				{
					MKY.IO.Usb.DeviceInfo deviceInfo = this.settingsRoot.IO.UsbSerialHidDevice.DeviceInfo;
					if (deviceInfo != null)
					{
						MKY.IO.Usb.SerialHidDeviceCollection devices = new MKY.IO.Usb.SerialHidDeviceCollection();
						devices.FillWithAvailableDevices(true); // Retrieve strings from devices in order to get serial strings.

						// Attention:
						// Similar code exists in View.Controls.UsbSerialHidDeviceSelection.SetDeviceList().
						// Changes here may have to be applied there too!

						if (devices.Count > 0)
						{
							if (devices.Contains(deviceInfo))
							{
								return (true);
							}
							else if (devices.ContainsVidPid(deviceInfo))
							{
								// A device with same VID/PID is available, use that:
								int sameVidPidIndex = devices.FindIndexVidPid(deviceInfo);

								// Inform the user if serial is required:
								if (ApplicationSettings.LocalUserSettings.General.MatchUsbSerial)
								{
									DialogResult dr = ShowUsbSerialHidDeviceNotAvailableAlternateQuestion(deviceInfo, devices[sameVidPidIndex]);
									if (dr == DialogResult.Yes)
										this.settingsRoot.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];

									return (true); // Return 'true' in any case, device may not yet be available but 'AutoOpen'.
								}
								else
								{
									// Clear the 'Changed' flag in case of automatically changing settings:
									bool haveAlreadyBeenChanged = this.settingsRoot.IO.UsbSerialHidDevice.HaveChanged;
									this.settingsRoot.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];
									if (!haveAlreadyBeenChanged)
										this.settingsRoot.IO.UsbSerialHidDevice.ClearChanged();

									return (true); // Return 'true' in any case, device may not yet be available but 'AutoOpen'.
								}
							}
							else
							{
								DialogResult dr = ShowUsbSerialHidDeviceNotAvailableStartAnywayQuestion(deviceInfo);
								return (dr == DialogResult.Yes);
							}
						}
						else // devices.Count == 0
						{
							DialogResult dr = ShowNoUsbSerialHidDevicesStartAnywayQuestion(deviceInfo);
							return (dr == DialogResult.Yes);
						}
					}

					return (true); // Return 'true' in all 'non-handled' cases => still try to start I/O to force the underlying exception message.
				}

				default: // Not (yet) useful for UDP/IP sockets.
				{
					return (true); // Return 'true' in all 'non-handled' cases.
				}
			}
		}

		private DialogResult ShowNoSerialPortsStartAnywayQuestion(string portIdNotAvailable)
		{
			string message =
				"There are currently no serial COM ports available." + Environment.NewLine + Environment.NewLine +
				"Start with " + portIdNotAvailable + " anyway?";

			DialogResult dr = OnMessageInputRequest
			(
				message,
				"No serial COM ports available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowSerialPortNotAvailableSwitchQuestion(string portIdNotAvailable, string portIdAlternate)
		{
			string message =
				"The previous serial port " + portIdNotAvailable + " is currently not available. " +
				"Switch to " + portIdAlternate + " instead?";

			DialogResult dr = OnMessageInputRequest
			(
				message,
				"Switch serial COM port?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private void ShowNoLocalInterfacesMessage()
		{
			string message =
				"There are currently no local network interfaces available." + Environment.NewLine + Environment.NewLine +
				"Terminal will not be started.";

			OnMessageInputRequest
			(
				message,
				"No interfaces available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning
			);
		}

		private DialogResult ShowLocalInterfaceNotAvailableDefaultQuestion(string localInterfaceNotAvailable, string localInterfaceDefaulted)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + localInterfaceDefaulted + "' (default) instead?";

			DialogResult dr = OnMessageInputRequest
			(
				message,
				"Switch interface?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private DialogResult ShowLocalInterfaceNotAvailableAlternateQuestion(string localInterfaceNotAvailable, string localInterfaceAlternate)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + localInterfaceAlternate + "' (first available interface with same description) instead?";

			DialogResult dr = OnMessageInputRequest
			(
				message,
				"Switch interface?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private DialogResult ShowNoUsbSerialHidDevicesStartAnywayQuestion(string deviceInfoNotAvailable)
		{
			string message =
				"There are currently no HID capable USB devices available." + Environment.NewLine + Environment.NewLine +
				"Start with " + deviceInfoNotAvailable + " anyway?";

			DialogResult dr = OnMessageInputRequest
			(
				message,
				"No USB HID devices available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceNotAvailableStartAnywayQuestion(string deviceInfoNotAvailable)
		{
			string message =
				"The previous device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Start anyway?";

			DialogResult dr = OnMessageInputRequest
			(
				message,
				"Previous USB HID device not available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceNotAvailableAlternateQuestion(string deviceInfoNotAvailable, string deviceInfoAlternate)
		{
			string message =
				"The previous device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + deviceInfoAlternate + "' (first available device with same VID and PID) instead?";

			DialogResult dr = OnMessageInputRequest
			(
				message,
				"Switch USB HID device?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		#endregion

		#region Terminal > Start/Stop I/O
		//------------------------------------------------------------------------------------------
		// Terminal > Start/Stop I/O
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public virtual bool StartIO()
		{
			return (StartIO(true));
		}

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private bool StartIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Starting terminal...");
			try
			{
				if (this.terminal.Start())
				{
					if (saveStatus)
						this.settingsRoot.TerminalIsStarted = this.terminal.IsStarted;

					OnTimedStatusTextRequest("Terminal successfully started.");
					success = true;
				}
				else
				{
					OnFixedStatusTextRequest("Terminal could not be started!");

					string yatLead, yatText;
					ErrorHelper.MakeStartHint(this.settingsRoot.IOType, out yatLead, out yatText);

					OnMessageInputRequest
					(
						ErrorHelper.ComposeMessage("Terminal could not be started!", yatLead, yatText),
						"Terminal Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
				}
			}
			catch (Exception ex)
			{
				OnFixedStatusTextRequest("Error starting terminal!");

				string yatLead, yatText;
				ErrorHelper.MakeExceptionHint(this.settingsRoot.IOType, out yatLead, out yatText);

				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to start terminal!", ex, yatLead, yatText),
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}

			return (success);
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public virtual bool StopIO()
		{
			return (StopIO(true));
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private bool StopIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Stopping terminal...");
			try
			{
				this.terminal.Stop();

				if (saveStatus)
					this.settingsRoot.TerminalIsStarted = this.terminal.IsStarted;

				OnTimedStatusTextRequest("Terminal stopped.");
				success = true;
			}
			catch (Exception ex)
			{
				OnTimedStatusTextRequest("Error stopping terminal!");
				OnMessageInputRequest
				(
					"Unable to stop terminal:" + Environment.NewLine + Environment.NewLine + ex.Message,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest("Terminal not stopped!");
			}

			return (success);
		}

		#endregion

		#region Terminal > Send
		//------------------------------------------------------------------------------------------
		// Terminal > Send
		//------------------------------------------------------------------------------------------

		private void Send(byte[] data)
		{
			try
			{
				this.terminal.Send(data);
			}
			catch (IOException ex)
			{
				// Attention:
				// Similar code can be found in functions below!
				// Changes here may have to be applied there too!

				string dataText = "";
				if (data != null)
				{
					if (data.Length == 1)
						dataText = data.Length.ToString(CultureInfo.InvariantCulture) + " byte";
					else
						dataText = data.Length.ToString(CultureInfo.InvariantCulture) + " bytes";
				}

				string lead;
				string title;
				PrepareIOErrorMessageInputRequest(out lead, out title);

				OnFixedStatusTextRequest("Error sending " + dataText + "!");
				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage(lead, ex),
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest(dataText + " not sent!");
			}
		}

		private void Send(string data, Domain.Radix defaultRadix = Domain.Parser.Parser.DefaultRadixDefault)
		{
			Send(data, defaultRadix, false);
		}

		private void SendEol()
		{
			Send("", Domain.Radix.String, true);
		}

		private void SendLine(string line, Domain.Radix defaultRadix = Domain.Parser.Parser.DefaultRadixDefault)
		{
			Send(line, defaultRadix, true);
		}

		private void Send(string data, Domain.Radix defaultRadix, bool isLine)
		{
			try
			{
				DebugMessage(@"Sending """ + (!string.IsNullOrEmpty(data) ? data : "") + @"""...");

				if (isLine)
					this.terminal.SendLine(data, defaultRadix);
				else
					this.terminal.Send(data, defaultRadix);
			}
			catch (IOException ex)
			{
				// Attention:
				// Similar code can be found in functions above and below!
				// Changes here may have to be applied there too!

				string dataText = (!string.IsNullOrEmpty(data) ? @"""" + data + @"""" : "");

				string lead;
				string title;
				PrepareIOErrorMessageInputRequest(out lead, out title);

				OnFixedStatusTextRequest("Error sending " + dataText + "!");
				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage(lead, ex),
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest(dataText + " not sent!");
			}
			catch (Domain.Parser.FormatException ex)
			{
				// Attention:
				// Similar code can be found in functions below!
				// Changes here may have to be applied there too!

				string dataText = (!string.IsNullOrEmpty(data) ? @"""" + data + @"""" : "");

				OnFixedStatusTextRequest("Error sending " + dataText + "!");
				OnMessageInputRequest
				(
					"Bad data format:" + Environment.NewLine +
					ex.Message,
					"Format Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest(dataText + " not sent!");
			}
		}

		private void PrepareIOErrorMessageInputRequest(out string text, out string title)
		{
			StringBuilder textBuilder = new StringBuilder();
			StringBuilder titleBuilder = new StringBuilder();

			textBuilder.Append("Unable to write to ");
			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
					textBuilder.Append("port");
					titleBuilder.Append("Serial COM Port");
					break;

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				case Domain.IOType.UdpClient:
				case Domain.IOType.UdpServer:
				case Domain.IOType.UdpPairSocket:
					textBuilder.Append("socket");
					titleBuilder.Append("Socket");
					break;

				case Domain.IOType.UsbSerialHid:
					textBuilder.Append("device");
					titleBuilder.Append("Device");
					break;

				default:
					throw (new NotSupportedException("Program execution should never get here, '" + this.settingsRoot.IOType + "' is an invalid I/O type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
			textBuilder.Append(":");
			titleBuilder.Append(" Error!");

			text = textBuilder.ToString();
			title = titleBuilder.ToString();
		}

		/// <remarks>
		/// Required to allow sending multi-line commands in a single operation. Otherwise, using
		/// <see cref="SendLine"/>, sending gets mixed-up because of the following sequence:
		///  1. First line gets sent/enqueued.
		///  2. Second line gets sent/enqueued.
		///  3. Response to first line is received and displayed
		///     and so on, mix-up among sent and received lines...
		/// </remarks>
		private void SendMultiLine(string[] multiLineText, Domain.Radix defaultRadix, string singleLineText)
		{
			try
			{
				DebugMessage(@"Sending """ + (!string.IsNullOrEmpty(singleLineText) ? singleLineText : "") + @"""...");

				this.terminal.SendLines(multiLineText, defaultRadix);
			}
			catch (IOException ex)
			{
				// Attention:
				// Similar code can be found in functions above!
				// Changes here may have to be applied there too!

				string dataText = (!string.IsNullOrEmpty(singleLineText) ? @"""" + singleLineText + @"""" : "");

				string lead;
				string title;
				PrepareIOErrorMessageInputRequest(out lead, out title);

				OnFixedStatusTextRequest("Error sending " + dataText + "!");
				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage(lead, ex),
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest(dataText + " not sent!");
			}
			catch (Domain.Parser.FormatException ex)
			{
				// Attention:
				// Similar code can be found in functions above!
				// Changes here may have to be applied there too!

				string dataText = (!string.IsNullOrEmpty(singleLineText) ? @"""" + singleLineText + @"""" : "");

				OnFixedStatusTextRequest("Error sending " + dataText + "!");
				OnMessageInputRequest
				(
					"Bad data format:" + Environment.NewLine +
					ex.Message,
					"Format Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest(dataText + " not sent!");
			}
		}

		#endregion

		#region Terminal > Send Command
		//------------------------------------------------------------------------------------------
		// Terminal > Send Command
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends given command.
		/// </summary>
		public virtual void SendCommand(Command command)
		{
			if (command.IsText)
				SendText(command);
			else if (command.IsFilePath)
				SendFile(command);
		}

		#endregion

		#region Terminal > Send Text
		//------------------------------------------------------------------------------------------
		// Terminal > Send Text
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends text command given by terminal settings.
		/// </summary>
		public virtual void SendText()
		{
			DoSendText(this.settingsRoot.SendText.Command);

			// Clear command if desired:
			if (!this.settingsRoot.Send.KeepCommand)
				this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
		}

		/// <summary>
		/// Sends partial text EOL.
		/// </summary>
		public virtual void SendPartialTextEol()
		{
			DoSendText(new Command(true, this.settingsRoot.SendText.Command.DefaultRadix));
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="c">Text command to be sent.</param>
		public virtual void SendText(Command c)
		{
			DoSendText(c);
		}

		/// <remarks>
		/// This method shall not be overridden. All text sending shall be requested using this
		/// method, to ensure that pending break conditions are resumed.
		/// </remarks>
		protected void DoSendText(Command c)
		{
			if (c.IsValidText)
			{
				if (c.IsSingleLineText)
				{
					if (SendTextSettings.IsEasterEggCommand(c.SingleLineText))
						SendLine(SendTextSettings.EasterEggCommandText, Domain.Radix.String);
					else
						SendLine(c.SingleLineText, c.DefaultRadix);
				}
				else if (c.IsMultiLineText)
				{
					SendMultiLine(c.MultiLineText, c.DefaultRadix, c.SingleLineText);
				}
				else if (c.IsPartialText)
				{
					Send(c.PartialText, c.DefaultRadix);

					// Compile the partial command line for later use:
					if (string.IsNullOrEmpty(this.partialCommandLine))
						this.partialCommandLine = string.Copy(c.PartialText);
					else
						this.partialCommandLine += c.PartialText;
				}
				else if (c.IsPartialTextEol)
				{
					SendEol();
				}
				else
				{
					throw (new NotSupportedException(@"The command """ + c + @""" has an unknown type!"));
				}

				// Copy line text into recent commands, include compiled partial text:
				if (c.IsSingleLineText || c.IsMultiLineText /* || do not add c.IsPartialText to recents */ || c.IsPartialTextEol)
				{
					// Clone the command for the recent commands collection:
					Command clone;
					if (!c.IsPartialTextEol)
						clone = new Command(c); // 'Normal' case, simply clone the command.
					else                        // Partial, create an equivalent single line text.
						clone = new Command(this.partialCommandLine, false, c.DefaultRadix);

					// Put clone into recent history:
					this.settingsRoot.SendText.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(new RecentItem<Command>(clone));
					this.settingsRoot.SendText.SetChanged(); // Manual change required because underlying collection is modified.

					// Reset the partial command line:
					this.partialCommandLine = null;
				}
			}
		}

		#endregion

		#region Terminal > Send File
		//------------------------------------------------------------------------------------------
		// Terminal > Send File
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends file given by terminal settings.
		/// </summary>
		public virtual void SendFile()
		{
			SendFile(this.settingsRoot.SendFile.Command);
		}

		/// <summary>
		/// Sends given file.
		/// </summary>
		/// <param name="c">File to be sent.</param>
		public virtual void SendFile(Command c)
		{
			DoSendFile(c);
		}

		/// <remarks>
		/// This method shall not be overridden. All file sending shall be requested using this
		/// method, to ensure that pending break conditions are resumed.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		protected void DoSendFile(Command c)
		{
			if (!c.IsValidFilePath)
				return;

			string filePath = c.FilePath;

			try
			{
				if (this.terminal is Domain.TextTerminal)
				{
					if (ExtensionHelper.IsXmlFile(filePath))
					{
						// XML => Read all at once for simplicity:
						string[] lines;
						XmlReaderHelper.LinesFromFile(filePath, out lines);
						foreach (string line in lines)
							SendLine(line);
					}
					else if (ExtensionHelper.IsRtfFile(filePath))
					{
						// RTF => Read all at once for simplicity:
						string[] lines;
						RtfReaderHelper.LinesFromRtfFile(filePath, out lines);
						foreach (string line in lines)
							SendLine(line);
					}
					else
					{
						// Text => Send in lines to enable breaking:
						using (StreamReader sr = new StreamReader(filePath, (EncodingEx)this.settingsRoot.TextTerminal.Encoding, true))
						{										// Automatically detect encoding from BOM, otherwise use given setting.
							string line;
							while (((line = sr.ReadLine()) != null) && (!this.terminal.BreakState))
								SendLine(line, c.DefaultRadix); // Enable parsing.
						}
					}
				}
				else
				{
					if (ExtensionHelper.IsXmlFile(filePath))
					{
						// XML => Read all at once for simplicity:
						string[] lines;
						XmlReaderHelper.LinesFromFile(filePath, out lines);
						foreach (string line in lines)
							SendLine(line);
					}
					else
					{
						// Binary => Send in chunks to enable breaking:
						using (FileStream fs = File.OpenRead(filePath))
						{
							long remaining = fs.Length;
							while ((remaining > 0) && (!this.terminal.BreakState))
							{
								byte[] a = new byte[1024]; // 1 KB chunks.
								int n = fs.Read(a, 0, a.Length);
								Array.Resize<byte>(ref a, n);
								Send(a);
								remaining -= n;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				OnMessageInputRequest
				(
					"Error reading file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
					ex.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}

			// Clone the command for the recent commands collection:
			Command clone = new Command(c);

			// Put clone into recent history:
			this.settingsRoot.SendFile.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(new RecentItem<Command>(clone));
			this.settingsRoot.SendFile.SetChanged(); // Manual change required because underlying collection is modified.
		}

		#endregion

		#region Terminal > Send and/or Copy Predefined
		//------------------------------------------------------------------------------------------
		// Terminal > Send and/or Copy Predefined
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Send requested predefined command.
		/// </summary>
		/// <param name="page">Page 1..max.</param>
		/// <param name="command">Command 1..max.</param>
		public virtual bool SendPredefined(int page, int command)
		{
			// Verify arguments:
			if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(page - 1, command - 1))
				return (false);

			// Process command:
			Command c = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];
			if (c.IsValidText)
			{
				SendText(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendText.Command = new Command(c); // Copy command if desired.

				return (true);
			}
			else if (c.IsValidFilePath)
			{
				SendFile(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendFile.Command = new Command(c); // Copy command if desired.

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Copy the requested predefined command, not taking copy predefined settings
		/// <see cref="Domain.Settings.SendSettings.CopyPredefined"/> into account.
		/// </summary>
		/// <param name="page">Page 1..max.</param>
		/// <param name="command">Command 1..max.</param>
		public virtual bool CopyPredefined(int page, int command)
		{
			// Verify arguments:
			if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(page - 1, command - 1))
				return (false);

			// Process command:
			Command c = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];
			if (c.IsValidText)
			{
				this.settingsRoot.SendText.Command = new Command(c);
				return (true);
			}
			else if (c.IsValidFilePath)
			{
				this.settingsRoot.SendFile.Command = new Command(c);
				return (true);
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#region Terminal > Break/Resume
		//------------------------------------------------------------------------------------------
		// Terminal > Break/Resume
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Breaks all currently ongoing operations in the terminal.
		/// </summary>
		public virtual void Break()
		{
			this.terminal.Break();
		}

		/// <summary>
		/// Resumes all currently suspended operations in the terminal.
		/// </summary>
		public virtual void ResumeBreak()
		{
			this.terminal.ResumeBreak();
		}

		#endregion

		#region Terminal > Repositories
		//------------------------------------------------------------------------------------------
		// Terminal > Repositories
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Forces complete reload of repositories.
		/// </summary>
		public virtual void ReloadRepositories()
		{
			AssertNotDisposed();
			this.terminal.ReloadRepositories();
		}

		/// <summary>
		/// Forces complete reload of given repository.
		/// </summary>
		public virtual void ReloadRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			this.terminal.ReloadRepository(repositoryType);
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual List<Domain.DisplayElement> RepositoryToDisplayElements(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (this.terminal.RepositoryToDisplayElements(repositoryType));
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual List<Domain.DisplayLine> RepositoryToDisplayLines(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (this.terminal.RepositoryToDisplayLines(repositoryType));
		}

		/// <summary>
		/// Returns the last display line of desired repository for auxiliary purposes (e.g. automated testing).
		/// </summary>
		public virtual Domain.DisplayLine LastDisplayLineAuxiliary(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (this.terminal.LastDisplayLineAuxiliary(repositoryType));
		}

		/// <summary>
		/// Clears the last display line of desired repository for auxiliary purposes (e.g. automated testing).
		/// </summary>
		public virtual void ClearLastDisplayLineAuxiliary(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			this.terminal.ClearLastDisplayLineAuxiliary(repositoryType);
		}

		/// <summary>
		/// Returns contents of desired repository as string.
		/// </summary>
		/// <remarks>
		/// Can be used for debugging purposes.
		/// </remarks>
		public virtual string RepositoryToString(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (this.terminal.RepositoryToString(repositoryType));
		}

		/// <summary>
		/// Clears given repository.
		/// </summary>
		public virtual void ClearRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			this.terminal.ClearRepository(repositoryType);
		}

		/// <summary>
		/// Clears all repositories.
		/// </summary>
		public virtual void ClearRepositories()
		{
			AssertNotDisposed();
			this.terminal.ClearRepositories();
		}

		#endregion

		#region Terminal > Time Status
		//------------------------------------------------------------------------------------------
		// Terminal > Time Status
		//------------------------------------------------------------------------------------------

		private void CreateChronos()
		{
			this.activeConnectChrono = new Chronometer();
			this.activeConnectChrono.Interval = 1000;
		////this.activeConnectChrono.TimeSpanChanged shall not be used, events are fired by 'totalConnectChrono' below.

			this.totalConnectChrono = new Chronometer();
			this.totalConnectChrono.Interval = 1000;
			this.totalConnectChrono.TimeSpanChanged += totalConnectChrono_TimeSpanChanged;
		}

		private void DisposeChronos()
		{
			if (this.activeConnectChrono != null)
			{
				// No elapsed event, events are fired by total connect chrono.
				this.activeConnectChrono.Dispose();
				this.activeConnectChrono = null;
			}

			if (this.totalConnectChrono != null)
			{
				this.totalConnectChrono.TimeSpanChanged -= totalConnectChrono_TimeSpanChanged;
				this.totalConnectChrono.Dispose();
				this.totalConnectChrono = null;
			}
		}

		/// <summary></summary>
		public virtual TimeSpan ActiveConnectTime
		{
			get
			{
				AssertNotDisposed();
				return (this.activeConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		public virtual TimeSpan TotalConnectTime
		{
			get
			{
				AssertNotDisposed();
				return (this.totalConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual void GetConnectTime(out TimeSpan activeConnectTime, out TimeSpan totalConnectTime)
		{
			AssertNotDisposed();

			activeConnectTime = this.activeConnectChrono.TimeSpan;
			totalConnectTime  = this.totalConnectChrono.TimeSpan;
		}

		/// <summary></summary>
		public virtual void ResetConnectTime()
		{
			AssertNotDisposed();

			this.activeConnectChrono.Reset();
			this.totalConnectChrono .Reset();
		}

		private void totalConnectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			OnIOConnectTimeChanged(e);
		}

		#endregion

		#region Terminal > Data Status
		//------------------------------------------------------------------------------------------
		// Terminal > Data Status
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual int TxByteCount
		{
			get
			{
				AssertNotDisposed();
				return (this.txByteCount);
			}
		}

		/// <summary></summary>
		public virtual int TxLineCount
		{
			get
			{
				AssertNotDisposed();
				return (this.txLineCount);
			}
		}

		/// <summary></summary>
		public virtual int RxByteCount
		{
			get
			{
				AssertNotDisposed();
				return (this.rxByteCount);
			}
		}

		/// <summary></summary>
		public virtual int RxLineCount
		{
			get
			{
				AssertNotDisposed();
				return (this.rxLineCount);
			}
		}

		/// <summary></summary>
		public virtual int TxByteRate
		{
			get
			{
				AssertNotDisposed();
				return (this.txByteRate.RateValue);
			}
		}

		/// <summary></summary>
		public virtual int TxLineRate
		{
			get
			{
				AssertNotDisposed();
				return (this.txLineRate.RateValue);
			}
		}

		/// <summary></summary>
		public virtual int RxByteRate
		{
			get
			{
				AssertNotDisposed();
				return (this.rxByteRate.RateValue);
			}
		}

		/// <summary></summary>
		public virtual int RxLineRate
		{
			get
			{
				AssertNotDisposed();
				return (this.rxLineRate.RateValue);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual void GetDataCount(out int txByteCount, out int txLineCount, out int rxByteCount, out int rxLineCount)
		{
			AssertNotDisposed();

			txByteCount = this.txByteCount;
			txLineCount = this.txLineCount;
			rxByteCount = this.rxByteCount;
			rxLineCount = this.rxLineCount;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual void GetDataRate(out int txByteRate, out int txLineRate, out int rxByteRate, out int rxLineRate)
		{
			AssertNotDisposed();

			txByteRate = this.txByteRate.RateValue;
			txLineRate = this.txLineRate.RateValue;
			rxByteRate = this.rxByteRate.RateValue;
			rxLineRate = this.rxLineRate.RateValue;
		}

		/// <summary></summary>
		public virtual void ResetIOCountAndRate()
		{
			AssertNotDisposed();

			this.txByteCount = 0;
			this.txLineCount = 0;
			this.rxByteCount = 0;
			this.rxLineCount = 0;

			OnIOCountChanged(EventArgs.Empty);

			this.txByteRate.Reset();
			this.txLineRate.Reset();
			this.rxByteRate.Reset();
			this.rxLineRate.Reset();

			OnIORateChanged(EventArgs.Empty);
		}

		private void CreateRates()
		{
			int rateInterval   = 1000;
			int rateWindow     = 5000;
			int updateInterval =  250;

			this.txByteRate = new RateProvider(rateInterval, rateWindow, updateInterval);
			this.txLineRate = new RateProvider(rateInterval, rateWindow, updateInterval);
			this.rxByteRate = new RateProvider(rateInterval, rateWindow, updateInterval);
			this.rxLineRate = new RateProvider(rateInterval, rateWindow, updateInterval);

			this.txByteRate.Changed += rate_Changed;
			this.txLineRate.Changed += rate_Changed;
			this.rxByteRate.Changed += rate_Changed;
			this.rxLineRate.Changed += rate_Changed;
		}

		private void DisposeRates()
		{
			if (this.txByteRate != null)
			{
				this.txByteRate.Changed -= rate_Changed;
				this.txByteRate.Dispose();
				this.txByteRate = null;
			}

			if (this.txLineRate != null)
			{
				this.txLineRate.Changed -= rate_Changed;
				this.txLineRate.Dispose();
				this.txLineRate = null;
			}

			if (this.rxByteRate != null)
			{
				this.rxByteRate.Changed -= rate_Changed;
				this.rxByteRate.Dispose();
				this.rxByteRate = null;
			}

			if (this.rxLineRate != null)
			{
				this.rxLineRate.Changed -= rate_Changed;
				this.rxLineRate.Dispose();
				this.rxLineRate = null;
			}
		}

		private void rate_Changed(object sender, RateEventArgs e)
		{
			if (!this.isDisposed)
				OnIORateChanged(e);
		}

		#endregion

		#region Terminal > Flow Control and Break Status
		//------------------------------------------------------------------------------------------
		// Terminal > Flow Control and Break Status
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPins SerialPortControlPins
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SerialPortControlPins);
			}
		}

		/// <summary>
		/// Serial port control pin counts.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPinCount SerialPortControlPinCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SerialPortControlPinCount);
			}
		}

		/// <summary></summary>
		public virtual int SentXOnCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SentXOnCount);
			}
		}

		/// <summary></summary>
		public virtual int SentXOffCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SentXOffCount);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOnCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.ReceivedXOnCount);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOffCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.ReceivedXOffCount);
			}
		}

		/// <summary></summary>
		public virtual void ResetFlowControlCount()
		{
			AssertNotDisposed();

			this.terminal.ResetFlowControlCount();
		}

		/// <summary></summary>
		public virtual int OutputBreakCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.OutputBreakCount);
			}
		}

		/// <summary></summary>
		public virtual int InputBreakCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.InputBreakCount);
			}
		}

		/// <summary></summary>
		public virtual void ResetBreakCount()
		{
			AssertNotDisposed();

			this.terminal.ResetBreakCount();
		}

		#endregion

		#region Terminal > I/O Control
		//------------------------------------------------------------------------------------------
		// Terminal > I/O Control
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Toggles RFR control pin if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		public virtual bool RequestToggleRfr()
		{
			AssertNotDisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleRfr(out pinState);
			this.SettingsRoot.IO.SerialPort.Communication.RfrPin = pinState;
			return (isSuccess);

			// Note, this user requested change of the current settings is handled here,
			// and not in the 'terminal_IOControlChanged' event handler, for two reasons:
			//  1. The event will be fired after any change of the control pins, separating
			//     user indended and other events would be cumbersome.
			//  2. The event will be fired asynchronously, accessing the settings here
			//     synchronously is the better option.
			//  3. Here, it is obvious that the user indeed wants to change a pin setting,
			//     and therefore expects the setting to be modified.
		}

		/// <summary>
		/// Toggles DTR control pin if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		public virtual bool RequestToggleDtr()
		{
			AssertNotDisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleDtr(out pinState);
			this.SettingsRoot.IO.SerialPort.Communication.DtrPin = pinState;
			return (isSuccess);

			// Note, this user requested change of the current settings is handled here,
			// and not in the 'terminal_IOControlChanged' event handler, for two reasons:
			//  1. The event will be fired after any change of the control pins, separating
			//     user indended and other events would be cumbersome.
			//  2. The event will be fired asynchronously, accessing the settings here
			//     synchronously is the better option.
			//  3. Here, it is obvious that the user indeed wants to change a pin setting,
			//     and therefore expects the setting to be modified.
		}

		/// <summary>
		/// Toggles the input XOn/XOff state if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool RequestToggleInputXOnXOff()
		{
			AssertNotDisposed();

			return (this.terminal.RequestToggleInputXOnXOff());
		}

		/// <summary>
		/// Toggles the output break state if current port settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool RequestToggleOutputBreak()
		{
			AssertNotDisposed();

			return (this.terminal.RequestToggleOutputBreak());
		}

		#endregion

		#endregion

		#region Log
		//==========================================================================================
		// Log
		//==========================================================================================

		private void DisposeLog()
		{
			if (this.log != null)
			{
				this.log.Dispose();
				this.log = null;
			}
		}

		/// <summary></summary>
		public virtual bool SwitchLogOn()
		{
			try
			{
				// Re-apply settings immediately, makes sure date/time in filenames is refreshed:
				this.log.Settings = this.settingsRoot.Log;
				this.log.SwitchOn();
				this.settingsRoot.LogIsOn = true;

				return (true);
			}
			catch (IOException ex)
			{
				OnMessageInputRequest
				(
					"Unable to switch log on."                           + Environment.NewLine + Environment.NewLine +
					"System message:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
					ApplicationEx.ProductName + " hint:" + Environment.NewLine +
					"Log file could already be in use.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool ClearLog()
		{
			try
			{
				this.log.Clear();

				return (true);
			}
			catch (IOException ex)
			{
				OnMessageInputRequest
				(
					"Unable to clear log."                               + Environment.NewLine + Environment.NewLine +
					"System message:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
					ApplicationEx.ProductName + " hint:" + Environment.NewLine +
					"Log file could already be in use.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SwitchLogOff()
		{
			return SwitchLogOff(true);
		}

		/// <summary></summary>
		public virtual bool SwitchLogOff(bool saveStatus)
		{
			try
			{
				this.log.SwitchOff();

				if (saveStatus)
					this.settingsRoot.LogIsOn = false;

				return (true);
			}
			catch (IOException ex)
			{
				OnMessageInputRequest
				(
					"Unable to switch log off." + Environment.NewLine + Environment.NewLine +
					"System message:" + Environment.NewLine + ex.Message,
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool OpenLogFile()
		{
			int fileCount = 0;

			if (this.log != null)
				fileCount = this.log.FilePaths.Count;

			if (fileCount > 0)
			{
				bool success = true;

				foreach (string filePath in this.log.FilePaths)
				{
					Exception ex;
					if (!Editor.TryOpenFile(filePath, out ex))
					{
						DialogResult dr = OnMessageInputRequest
						(
							ErrorHelper.ComposeMessage("Unable to open log file", filePath, ex),
							"Log File Error",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Error
						);

						if (dr == DialogResult.Cancel)
						{
							success = false;
							break;
						}
					}
				}

				return (success);
			}
			else
			{
				OnMessageInputRequest
				(
					"No log file(s) available (yet).",
					"Log File Information",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				);

				return (true);
			}
		}

		/// <summary></summary>
		public virtual bool OpenLogDirectory()
		{
			if (this.log != null)
			{
				string rootPath = this.log.Settings.RootPath;

				Exception ex;
				if (!DirectoryEx.TryOpen(rootPath, out ex))
				{
					OnMessageInputRequest
					(
						ErrorHelper.ComposeMessage("Unable to open log folder", rootPath, ex),
						"Log Folder Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					return (false);
				}
				else
				{
					return (true);
				}
			}
			else
			{
				OnMessageInputRequest
				(
					"No log folder available (yet).",
					"Log File Information",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				);

				return (true);
			}
		}

		#endregion

		#region Auto Response
		//==========================================================================================
		// Auto Response
		//==========================================================================================

		private void CreateAutoResponse()
		{
			UpdateAutoResponse(); // Simply forward to general Update() method.
		}

		private void DisposeAutoResponse()
		{
			lock (this.autoResponseHelperSyncObj)
				this.autoResponseHelper = null; // Simply delete the reference to the object.
		}

		/// <summary>
		/// Updates the automatic response helper.
		/// </summary>
		protected virtual void UpdateAutoResponse()
		{
			if (this.settingsRoot.AutoResponse.IsActive)
			{
				if (this.settingsRoot.AutoResponse.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					byte[] sequence;
					if (TryParseCommandToSequence(this.settingsRoot.ActiveAutoResponseTrigger, out sequence))
					{
						lock (this.autoResponseHelperSyncObj)
						{
							if (this.autoResponseHelper == null)
								this.autoResponseHelper = new AutoResponseHelper(sequence);
							else
								this.autoResponseHelper.Sequence = sequence;
						}
					}
					else
					{
						lock (this.autoResponseHelperSyncObj)
							this.autoResponseHelper = null;

						this.settingsRoot.AutoResponse.Deactivate();

						OnMessageInputRequest
						(
							"Failed to parse the automatic response trigger! Automatic response has been disabled!" + Environment.NewLine + Environment.NewLine +
							"To enable again, re-configure the automatic response.",
							"Automatic Response Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation
						);
					}
				}
				else // No command required = no sequence required = no helper required.
				{
					lock (this.autoResponseHelperSyncObj)
						this.autoResponseHelper = null;
				}
			}
			else // Disabled.
			{
				lock (this.autoResponseHelperSyncObj)
					this.autoResponseHelper = null;
			}
		}

		/// <summary>
		/// Tries to parse the given command into the corresponding byte sequence, taking the current settings into account.
		/// </summary>
		private bool TryParseCommandToSequence(Command c, out byte[] sequence)
		{
			if ((c != null) && (this.terminal != null))
			{
				if (c.IsSingleLineText)
				{
					byte[] lineResult;
					if (this.terminal.TryParse(c.SingleLineText, out lineResult, c.DefaultRadix))
					{
						sequence = lineResult;
						return (true);
					}
				}
				else if (c.IsMultiLineText)
				{
					List<byte> commandResult = new List<byte>(256); // Preset the initial capacity to improve memory management, 256 is an arbitrary value.

					foreach (string line in c.MultiLineText)
					{
						byte[] lineResult;
						if (this.terminal.TryParse(line, out lineResult, c.DefaultRadix))
							commandResult.AddRange(lineResult);
					}

					if (commandResult.Count > 0)
					{
						sequence = commandResult.ToArray();
						return (true);
					}
				}
			}

			sequence = null;
			return (false);
		}

		/// <summary>
		/// Sends the automatic response.
		/// </summary>
		protected virtual void SendAutoResponse()
		{
			int page = this.settingsRoot.Predefined.SelectedPage;

			switch ((AutoResponse)this.settingsRoot.AutoResponse.Response)
			{
				case AutoResponse.PredefinedCommand1:  SendPredefined(page, 1);  break;
				case AutoResponse.PredefinedCommand2:  SendPredefined(page, 2);  break;
				case AutoResponse.PredefinedCommand3:  SendPredefined(page, 3);  break;
				case AutoResponse.PredefinedCommand4:  SendPredefined(page, 4);  break;
				case AutoResponse.PredefinedCommand5:  SendPredefined(page, 5);  break;
				case AutoResponse.PredefinedCommand6:  SendPredefined(page, 6);  break;
				case AutoResponse.PredefinedCommand7:  SendPredefined(page, 7);  break;
				case AutoResponse.PredefinedCommand8:  SendPredefined(page, 8);  break;
				case AutoResponse.PredefinedCommand9:  SendPredefined(page, 9);  break;
				case AutoResponse.PredefinedCommand10: SendPredefined(page, 10); break;
				case AutoResponse.PredefinedCommand11: SendPredefined(page, 11); break;
				case AutoResponse.PredefinedCommand12: SendPredefined(page, 12); break;
				case AutoResponse.SendText:            SendText();               break;
				case AutoResponse.SendFile:            SendFile();               break;

				case AutoResponse.Explicit:
					SendCommand(new Command(this.settingsRoot.AutoResponse.Response)); // No explicit default radix available (yet).
					break;

				case AutoResponse.None:
				default:
					break;
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			EventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			EventHelper.FireSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOConnectTimeChanged(TimeSpanEventArgs e)
		{
			EventHelper.FireSync<TimeSpanEventArgs>(IOConnectTimeChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOCountChanged(EventArgs e)
		{
			EventHelper.FireSync(IOCountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIORateChanged(EventArgs e)
		{
			EventHelper.FireSync(IORateChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(Domain.IOErrorEventArgs e)
		{
			EventHelper.FireSync<Domain.IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(Domain.DisplayElementsEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(Domain.DisplayElementsEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesSent(Domain.DisplayLinesEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(Domain.DisplayLinesEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(EventArgs<Domain.RepositoryType> e)
		{
			EventHelper.FireSync<EventArgs<Domain.RepositoryType>>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(EventArgs<Domain.RepositoryType> e)
		{
			EventHelper.FireSync<EventArgs<Domain.RepositoryType>>(RepositoryReloaded, this, e);
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			DebugMessage(text);
			EventHelper.FireSync<EventArgs<string>>(FixedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			DebugMessage(text);
			EventHelper.FireSync<EventArgs<string>>(TimedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return (OnMessageInputRequest(text, caption, buttons, icon, MessageBoxDefaultButton.Button1));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			if (this.startArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				MessageInputEventArgs e = new MessageInputEventArgs(text, caption, buttons, icon, defaultButton);
				EventHelper.FireSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				// Ensure that the request is processed!
				if (e.Result == DialogResult.None)
					throw (new InvalidOperationException(@"A 'Message Input' request by terminal """ + Caption + @""" was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <summary></summary>
		protected virtual DialogResult OnSaveAsFileDialogRequest()
		{
			if (this.startArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				DialogEventArgs e = new DialogEventArgs();
				EventHelper.FireSync<DialogEventArgs>(SaveAsFileDialogRequest, this, e);

				// Ensure that the request is processed!
				if (e.Result == DialogResult.None)
					throw (new InvalidOperationException(@"A 'Save As' request by terminal """ + Caption + @""" was not processed by the application!"));

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnCursorRequest(Cursor cursor)
		{
			EventHelper.FireSync<EventArgs<Cursor>>(CursorRequest, this, new EventArgs<Cursor>(cursor));
		}

		/// <summary></summary>
		protected virtual void OnCursorReset()
		{
			OnCursorRequest(Cursors.Default);
		}

		/// <summary></summary>
		protected virtual void OnSaved(SavedEventArgs e)
		{
			EventHelper.FireSync<SavedEventArgs>(Saved, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(ClosedEventArgs e)
		{
			EventHelper.FireSync<ClosedEventArgs>(Closed, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			AssertNotDisposed();

			return (Caption);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					"",
					"[" + Guid + "]",
					message
				)
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================