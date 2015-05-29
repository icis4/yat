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
// YAT 2.0 Gamma 1 Version 1.99.32
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.IO;
using MKY.Net;
using MKY.Recent;
using MKY.Settings;
using MKY.Time;

using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Utilities;

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
		[SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Justification = "The initialization of 'staticSequentialIndexCounter' is not unnecesary, it is based on a constant that contains a default value!")]
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
		private Log.Logs log;

		// Time status:
		private Chronometer connectChrono;
		private Chronometer totalConnectChrono;

		// Count status:
		private int txByteCount;
		private int rxByteCount;
		private int txLineCount;
		private int rxLineCount;

		// Rate status:
		private Rate txByteRate;
		private Rate rxByteRate;
		private Rate txLineRate;
		private Rate rxLineRate;

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
		public event EventHandler<Domain.RepositoryEventArgs> RepositoryCleared;

		/// <summary></summary>
		public event EventHandler<Domain.RepositoryEventArgs> RepositoryReloaded;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<DialogEventArgs> SaveAsFileDialogRequest;

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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public Terminal(TerminalStartArgs startArgs, DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid)
		{
			WriteDebugMessageLine("Creating...");

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

			// Set ID and user name.
			this.sequentialIndex = ++staticSequentialIndexCounter;
			if (!this.settingsHandler.SettingsFilePathIsValid || this.settingsRoot.AutoSaved)
				this.autoName = TerminalText + this.sequentialIndex.ToString(CultureInfo.CurrentCulture);
			else
				AutoNameFromFile = this.settingsHandler.SettingsFilePath;

			// Create underlying terminal.
			this.terminal = Domain.TerminalFactory.CreateTerminal(this.settingsRoot.Terminal);
			AttachTerminalEventHandlers();

			// Create log.
			this.log = new Log.Logs(this.settingsRoot.Log);

			// Create chronos.
			CreateChronos();

			// Create rates.
			CreateRates();

			WriteDebugMessageLine("...successfully created.");
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
			if (!this.isDisposed)
			{
				WriteDebugMessageLine("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, terminal and log have already been closed, otherwise...

					// ...first, detach event handlers to ensure that no more events are received...
					DetachTerminalEventHandlers();
					DetachSettingsEventHandlers();

					// ...then, dispose of objects.
					DisposeRates();
					DisposeChronos();

					if (this.log != null)
						this.log.Dispose();

					if (this.terminal != null)
						this.terminal.Dispose();
				}

				// Set state to disposed:
				this.log = null;
				this.terminal = null;
				this.isDisposed = true;

				WriteDebugMessageLine("...successfully disposed.");
			}
		}

		/// <summary></summary>
		~Terminal()
		{
			Dispose(false);

			WriteDebugMessageLine("The finalizer should have never been called! Ensure to call Dispose()!");
		}

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
		public virtual bool LogIsStarted
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.log.IsStarted);
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "IOIs", Justification = "What's wrong? 'IO' is according to the guideline, 'Is' is a verb...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",   MessageId = "IOIs", Justification = "What's wrong? 'IO' is according to the guideline, 'Is' is a verb...")]
		public virtual bool UnderlyingIOIsSerialPort
		{
			get
			{
				AssertNotDisposed();

				return ((this.settingsRoot != null) && (this.settingsRoot.IOType == Domain.IOType.SerialPort));
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
								else
								{
									sb.Append("Closed - Waiting for reconnect");
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
							sb.Append(IPHost.ToUrlString(s.ResolvedRemoteIPAddress));
							sb.Append(":");
							sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
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

								MKY.IO.Serial.Socket.TcpAutoSocket socket = this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpAutoSocket;
								if (socket != null)
								{
									isClient = socket.IsClient;
									isServer = socket.IsServer;
								}

								if (isClient)
								{
									sb.Append(" - ");
									sb.Append(IPHost.ToUrlString(s.ResolvedRemoteIPAddress));
									sb.Append(":");
									sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
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

						case Domain.IOType.Udp:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append(" - ");
							sb.Append(IPHost.ToUrlString(s.ResolvedRemoteIPAddress));
							sb.Append(":");
							sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" - ");
							sb.Append("Receive:");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" - ");
							sb.Append(IsOpen ? "Open" : "Closed");
							break;
						}

						case Domain.IOType.UsbSerialHid:
						{
							MKY.IO.Serial.Usb.SerialHidDevice device = this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Usb.SerialHidDevice;
							if (device != null)
							{
								sb.Append(" - ");
								sb.Append(device.DeviceInfoString);
								sb.Append(" - ");
								if (IsStarted)
								{
									if (IsConnected)
										sb.Append("Connected - Open");
									else
										sb.Append("Disconnected - Waiting for reconnect");
								}
								else
								{
									sb.Append("Closed");
								}
							}
							break;
						}
					}
				}

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
							sb.Append(" (" + s.Communication + ") is ");
							if (IsStarted)
							{
								if (IsOpen)
								{
									sb.Append("open and ");
									sb.Append(IsConnected ? "connected" : "disconnected");
								}
								else
								{
									sb.Append("closed and waiting for reconnect");
								}
							}
							else
							{
								sb.Append("closed");
							}
							break;
						}

						case Domain.IOType.TcpClient:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append("TCP/IP Client is ");

							if (IsConnected)
								sb.Append("connected to ");
							else if (IsStarted && s.TcpClientAutoReconnect.Enabled)
								sb.Append("disconnected and waiting for reconnect to ");
							else
								sb.Append("disconnected from ");

							sb.Append(s.ResolvedRemoteIPAddress.ToString());
							sb.Append(" on remote port ");
							sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							break;
						}

						case Domain.IOType.TcpServer:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append("TCP/IP Server is ");
							if (IsStarted)
							{
								if (IsConnected)
								{
									int count = 0;

									MKY.IO.Serial.Socket.TcpServer server = this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpServer;
									if (server != null)
										count = server.ConnectedClientCount;

									sb.Append("connected to ");
									if (count == 1)
									{
										sb.Append("a client");
									}
									else
									{
										sb.Append(count.ToString(CultureInfo.CurrentCulture));
										sb.Append(" clients");
									}
								}
								else
								{
									sb.Append("listening");
								}
							}
							else
							{
								sb.Append("closed");
							}
							sb.Append(" on local port ");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							break;
						}

						case Domain.IOType.TcpAutoSocket:
						{
							sb.Append("TCP auto socket is ");

							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							if (IsStarted)
							{
								bool isClient = false;
								bool isServer = false;

								MKY.IO.Serial.Socket.TcpAutoSocket socket = this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpAutoSocket;
								if (socket != null)
								{
									isClient = socket.IsClient;
									isServer = socket.IsServer;
								}

								if (isClient)
								{
									sb.Append("connected to ");
									sb.Append(s.ResolvedRemoteIPAddress.ToString());
									sb.Append(" on remote port ");
									sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								}
								else if (isServer)
								{
									sb.Append(IsConnected ? "connected" : "listening");
									sb.Append(" on local port ");
									sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								}
								else
								{
									sb.Append("starting to connect to ");
									sb.Append(s.ResolvedRemoteIPAddress.ToString());
									sb.Append(" on remote port ");
									sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								}
							}
							else
							{
								sb.Append("disconnected from ");
								sb.Append(s.ResolvedRemoteIPAddress.ToString());
								sb.Append(" on remote port ");
								sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							}
							break;
						}

						case Domain.IOType.Udp:
						{
							MKY.IO.Serial.Socket.SocketSettings s = this.settingsRoot.IO.Socket;
							sb.Append("UDP socket is ");
							sb.Append(IsOpen ? "open" : "closed");
							sb.Append(" for sending to ");
							sb.Append(s.ResolvedRemoteIPAddress.ToString());
							sb.Append(" on remote port ");
							sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" and receiving on local port ");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							break;
						}

						case Domain.IOType.UsbSerialHid:
						{
							MKY.IO.Serial.Usb.SerialHidDevice device = this.terminal.UnderlyingIOProvider as MKY.IO.Serial.Usb.SerialHidDevice;
							if (device != null)
							{
								sb.Append("USB HID device '");
								sb.Append(device.DeviceInfoString);
								sb.Append("' is ");
								if (IsStarted)
								{
									if (IsConnected)
										sb.Append("connected and open");
									else
										sb.Append("disconnected and waiting for reconnect");
								}
								else
								{
									sb.Append("closed");
								}
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

			// Begin logging (in case opening of terminal needs to be logged).
			if (this.settingsRoot.LogIsStarted)
			{
				if (!BeginLog())
					return (false);
			}

			// Then open terminal.
			if (this.settingsRoot.TerminalIsStarted)
			{
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

			// Settings have changed, recreate terminal with new settings.
			if (this.terminal.IsStarted)
			{
				// Terminal is open, close and re-open it with the new settings.
				if (StopIO(false))
				{
					DetachTerminalEventHandlers(); // Detach to suspend events.
					this.settingsRoot.Explicit = settings;
					Domain.Terminal oldTerminal = this.terminal;
					this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
					oldTerminal.Dispose();         // Ensure to dispose of the 'old' resources.
					AttachTerminalEventHandlers(); // Attach and resume events.

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
				// Terminal is closed, simply set the new settings.
				DetachTerminalEventHandlers(); // Detach to suspend events.
				this.settingsRoot.Explicit = settings;
				Domain.Terminal oldTerminal = this.terminal;
				this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
				oldTerminal.Dispose();         // Ensure to dispose of the 'old' resources.
				AttachTerminalEventHandlers(); // Attach and resume events.

				this.terminal.ReloadRepositories();

				OnTimedStatusTextRequest("Terminal settings applied.");
			}
		}

		/// <summary>
		/// Sets log settings.
		/// </summary>
		public virtual void SetLogSettings(Log.Settings.LogSettings settings)
		{
			AssertNotDisposed();

			this.settingsRoot.Log = settings;
			this.log.Settings = this.settingsRoot.Log;
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
				this.settingsRoot.Changed += new EventHandler<SettingsEventArgs>(settingsRoot_Changed);
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed -= new EventHandler<SettingsEventArgs>(settingsRoot_Changed);
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Required to solve the issue described in bug #223 "Settings events should state the exact settings diff".
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'sendImmediatelyOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool settingsRoot_Changed_sendImmediatelyOld = Domain.Settings.SendSettings.SendImmediatelyDefault;

		private void settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			// Note that GUI settings are handled in Gui.Forms.Terminal::settingsRoot_Changed().
			// Below, only those settings that need to be managed by the model are handled.

			if ((e.Inner != null) && ReferenceEquals(e.Inner.Source, this.settingsRoot.Explicit))
			{
				// Explicit settings have changed.
				SettingsEventArgs explicitEventArgs = e.Inner;
				if ((explicitEventArgs.Inner != null) && ReferenceEquals(explicitEventArgs.Inner.Source, this.settingsRoot.Terminal))
				{
					// Terminal settings have changed.
					SettingsEventArgs terminalEventArgs = explicitEventArgs.Inner;
					if ((terminalEventArgs.Inner != null) && ReferenceEquals(terminalEventArgs.Inner.Source, this.settingsRoot.Send))
					{
						// Send settings have changed.
						if (settingsRoot_Changed_sendImmediatelyOld != this.settingsRoot.Send.SendImmediately) {
							settingsRoot_Changed_sendImmediatelyOld = this.settingsRoot.Send.SendImmediately;

							// Send immediately has changed, reset the last command:
							this.settingsRoot.SendCommand.Command.Clear();
						}
					}
				}
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
				return (true);
			}

			// -------------------------------------------------------------------------------------
			// Create auto save file path or request manual/normal file path if necessary.
			// -------------------------------------------------------------------------------------

			if (!this.settingsHandler.SettingsFilePathIsValid)
			{
				if (isAutoSave)
				{
					string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveTerminalFileNamePrefix + Guid.ToString() + ExtensionSettings.TerminalFile;
					this.settingsHandler.SettingsFilePath = autoSaveFilePath;
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
					// Let save fail if the file path is not valid and no user interaction is allowed.
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
				return (SaveToFile(isAutoSave, ""));
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
			string autoSaveFilePathToDelete = "";
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
			if (!isAutoSave)
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

				if (!isAutoSave)
				{
					SetRecent(this.settingsHandler.SettingsFilePath);
					OnTimedStatusTextRequest("Terminal saved.");
				}

				// Try to delete existing auto save file, but ensure that this is not the current file:
				if (!StringEx.EqualsOrdinalIgnoreCase(autoSaveFilePathToDelete, this.settingsHandler.SettingsFilePath))
					FileEx.TryDelete(autoSaveFilePathToDelete);
			}
			catch (System.Xml.XmlException ex)
			{
				if (!isAutoSave)
				{
					OnFixedStatusTextRequest("Error saving terminal!");

					string message =
						"Unable to save file" + Environment.NewLine + this.settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message:"  + Environment.NewLine + ex.Message                            + Environment.NewLine + Environment.NewLine +
						"File error message:" + Environment.NewLine + ex.InnerException.Message;

					OnMessageInputRequest
					(
						message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					OnTimedStatusTextRequest("Terminal not saved!");
				}
			}
			catch (Exception ex)
			{
				if (!isAutoSave)
				{
					OnFixedStatusTextRequest("Error saving terminal!");

					string message =
						"Unable to save file"   + Environment.NewLine + this.settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"System error message:" + Environment.NewLine + ex.Message;

					OnMessageInputRequest
					(
						message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					OnTimedStatusTextRequest("Terminal not saved!");
				}
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
		/// \attention:
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
					// Implicit have changed, try to auto save but save is not required.
					doSave = false;
				}
				else
				{
					// Explicit have changed, try to auto save and save is required if auto save is desired.
					doSave = autoSaveIsAllowed;
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
					doSave = true;
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
			// Finally, close the terminal and signal state.
			// -------------------------------------------------------------------------------------

			if (success)
			{
				// Next, stop underlying terminal...
				if (this.terminal.IsStarted)
					success = StopIO(false);

				// ...and signal that the terminal can definitely close:
				this.terminal.Close();

				// Then, close log:
				if (this.log.IsStarted)
					EndLog();

				// Finally, ensure that chronos are stopped and do not fire events anymore:
				StopChronos();
			}

			if (success)
			{
				// Status text request must be before closed event, closed event may close the view:
				OnTimedStatusTextRequest("Terminal successfully closed.");
				OnClosed(new ClosedEventArgs(isWorkspaceClose));

				// Ensure that all resources of this terminal get disposed of:
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

		#region Recents
		//==========================================================================================
		// Recents
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
				this.terminal.IOChanged        += new EventHandler(terminal_IOChanged);
				this.terminal.IOControlChanged += new EventHandler(terminal_IOControlChanged);
				this.terminal.IOError          += new EventHandler<Domain.IOErrorEventArgs>(terminal_IOError);

				this.terminal.RawElementSent          += new EventHandler<Domain.RawElementEventArgs>(terminal_RawElementSent);
				this.terminal.RawElementReceived      += new EventHandler<Domain.RawElementEventArgs>(terminal_RawElementReceived);
				this.terminal.DisplayElementsSent     += new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsSent);
				this.terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsReceived);
				this.terminal.DisplayLinesSent        += new EventHandler<Domain.DisplayLinesEventArgs>(terminal_DisplayLinesSent);
				this.terminal.DisplayLinesReceived    += new EventHandler<Domain.DisplayLinesEventArgs>(terminal_DisplayLinesReceived);
				this.terminal.RepositoryCleared       += new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryCleared);
				this.terminal.RepositoryReloaded      += new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryReloaded);
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged        -= new EventHandler(terminal_IOChanged);
				this.terminal.IOControlChanged -= new EventHandler(terminal_IOControlChanged);
				this.terminal.IOError          -= new EventHandler<Domain.IOErrorEventArgs>(terminal_IOError);

				this.terminal.RawElementSent          -= new EventHandler<Domain.RawElementEventArgs>(terminal_RawElementSent);
				this.terminal.RawElementReceived      -= new EventHandler<Domain.RawElementEventArgs>(terminal_RawElementReceived);
				this.terminal.DisplayElementsSent     -= new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsSent);
				this.terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsReceived);
				this.terminal.DisplayLinesSent        -= new EventHandler<Domain.DisplayLinesEventArgs>(terminal_DisplayLinesSent);
				this.terminal.DisplayLinesReceived    -= new EventHandler<Domain.DisplayLinesEventArgs>(terminal_DisplayLinesReceived);
				this.terminal.RepositoryCleared       -= new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryCleared);
				this.terminal.RepositoryReloaded      -= new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryReloaded);
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
		private bool terminal_IOChanged_isConnected;

		private void terminal_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(e);

			if      ( this.terminal.IsConnected && !this.terminal_IOChanged_isConnected)
			{
				this.connectChrono.Restart();
				this.totalConnectChrono.Start();
			}
			else if (!this.terminal.IsConnected &&  this.terminal_IOChanged_isConnected)
			{
				this.connectChrono.Stop();
				this.totalConnectChrono.Stop();
			}

			this.terminal_IOChanged_isConnected = this.terminal.IsConnected;
		}

		private void terminal_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			OnIOError(e);
		}

		private void terminal_RawElementSent(object sender, Domain.RawElementEventArgs e)
		{
			OnTimedStatusTextRequest("Sending...");

			// Count.
			this.txByteCount += e.Element.Data.Count;
			OnIOCountChanged(EventArgs.Empty);

			// Rate.
			if (this.txByteRate.Update(e.Element.Data.Count))
				OnIORateChanged(EventArgs.Empty);

			// Log.
			if (this.log.IsStarted)
			{
				this.log.WriteBytes(e.Element.Data, Log.LogChannel.RawTx);
				this.log.WriteBytes(e.Element.Data, Log.LogChannel.RawBidir);
			}
		}

		private void terminal_RawElementReceived(object sender, Domain.RawElementEventArgs e)
		{
			OnTimedStatusTextRequest("Receiving...");

			// Count.
			this.rxByteCount += e.Element.Data.Count;
			OnIOCountChanged(EventArgs.Empty);

			// Rate.
			if (this.rxByteRate.Update(e.Element.Data.Count))
				OnIORateChanged(EventArgs.Empty);

			// Log.
			if (this.log.IsStarted)
			{
				this.log.WriteBytes(e.Element.Data, Log.LogChannel.RawBidir);
				this.log.WriteBytes(e.Element.Data, Log.LogChannel.RawRx);
			}
		}

		private void terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			// Display.
			OnDisplayElementsSent(e);

			// Log.
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (this.log.IsStarted)
				{
					if (de is Domain.DisplayElement.LineBreak)
					{
						this.log.WriteEol(Log.LogChannel.NeatTx);
						this.log.WriteEol(Log.LogChannel.NeatBidir);
					}
					else
					{
						this.log.WriteString(de.Text, Log.LogChannel.NeatTx);
						this.log.WriteString(de.Text, Log.LogChannel.NeatBidir);
					}
				}
			}
		}

		private void terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			// Display.
			OnDisplayElementsReceived(e);

			// Log.
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (this.log.IsStarted)
				{
					if (de is Domain.DisplayElement.LineBreak)
					{
						this.log.WriteEol(Log.LogChannel.NeatBidir);
						this.log.WriteEol(Log.LogChannel.NeatRx);
					}
					else
					{
						this.log.WriteString(de.Text, Log.LogChannel.NeatBidir);
						this.log.WriteString(de.Text, Log.LogChannel.NeatRx);
					}
				}
			}
		}

		private void terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count.
			this.txLineCount += e.Lines.Count;
			OnIOCountChanged(EventArgs.Empty);

			// Rate.
			if (this.txLineRate.Update(e.Lines.Count))
				OnIORateChanged(EventArgs.Empty);

			// Display.
			OnDisplayLinesSent(e);
		}

		private void terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count.
			this.rxLineCount += e.Lines.Count;
			OnIOCountChanged(EventArgs.Empty);

			// Rate.
			if (this.rxLineRate.Update(e.Lines.Count))
				OnIORateChanged(EventArgs.Empty);

			// Display.
			OnDisplayLinesReceived(e);
		}

		private void terminal_RepositoryCleared(object sender, Domain.RepositoryEventArgs e)
		{
			OnRepositoryCleared(e);
		}

		private void terminal_RepositoryReloaded(object sender, Domain.RepositoryEventArgs e)
		{
			OnRepositoryReloaded(e);
		}

		#endregion

		#region Terminal > Start/Stop
		//------------------------------------------------------------------------------------------
		// Terminal > Start/Stop
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

					OnTimedStatusTextRequest("Terminal started.");
					success = true;
				}
				else
				{
					OnFixedStatusTextRequest("Terminal could not be started!");
				}
			}
			catch (Exception ex)
			{
				OnFixedStatusTextRequest("Error starting terminal!");

				string yatTitle;
				string yatText;
				switch (this.settingsRoot.IOType)
				{
					case Domain.IOType.SerialPort:
					{
						yatTitle = ApplicationInfo.ProductName + " hints:";
						yatText  = "Make sure the selected serial port is available and not already in use. " +
						           "Also, check the communication settings and keep in mind that hardware and driver may limit the allowed communication settings.";
						break;
					}
					case Domain.IOType.TcpClient:
					case Domain.IOType.TcpServer:
					case Domain.IOType.TcpAutoSocket:
					case Domain.IOType.Udp:
					{
						yatTitle = ApplicationInfo.ProductName + " hint:";
						yatText  = "Make sure the selected socket is not already in use.";
						break;
					}
					case Domain.IOType.UsbSerialHid:
					{
						yatTitle = ApplicationInfo.ProductName + " hint:";
						yatText  = "Make sure the selected USB device is connected.";
						break;
					}
					default:
					{
						yatTitle = ApplicationInfo.ProductName + " error:";
						yatText  = "The I/O type " + this.settingsRoot.IOType  + " is unknown! " +
						           "Please report this issue as described in YAT > Help > Submit Bug. Thanks.";
						break;
					}
				}

				string message =
					"Unable to start terminal!" + Environment.NewLine + Environment.NewLine +
					"System error message:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
					yatTitle + Environment.NewLine +
					yatText;

				OnMessageInputRequest
				(
					message,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				OnTimedStatusTextRequest("Terminal not started!");
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

		#region Terminal > IO Control
		//------------------------------------------------------------------------------------------
		// Terminal > IO Control
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Toggles RFR line if current flow control settings allow this.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		public virtual void RequestToggleRfr()
		{
			if (this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesRfrCtsDtrDsrManually)
			{
				MKY.IO.Ports.ISerialPort p = (MKY.IO.Ports.ISerialPort)this.terminal.UnderlyingIOInstance;
				if (p != null)
					p.ToggleRfr();
				else
					throw (new InvalidOperationException("The underlying I/O provider is no serial COM port!"));
			}
		}

		/// <summary>
		/// Toggles DTR line if current flow control settings allow this.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		public virtual void RequestToggleDtr()
		{
			if (this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesRfrCtsDtrDsrManually)
			{
				MKY.IO.Ports.ISerialPort p = (MKY.IO.Ports.ISerialPort)this.terminal.UnderlyingIOInstance;
				if (p != null)
					p.ToggleDtr();
				else
					throw (new InvalidOperationException("The underlying I/O provider is no serial COM port!"));
			}
		}

		/// <summary>
		/// Toggles the input XOn/XOff state.
		/// </summary>
		public virtual void RequestToggleInputXOnXOff()
		{
			if (this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesXOnXOffManually)
			{
				MKY.IO.Serial.SerialPort.IXOnXOffHandler x = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.SerialPort.IXOnXOffHandler);
				if (x != null)
				{
					// Since the underlying I/O provider's 'DataSent' events are no longer used to
					// feed the outgoing data into the repositories, outgoing XOn/XOff characters
					// must manually be fed into the repositories. Do so before actually sending the
					// character to ensure that it is placed before eventual data.
					if (x.InputIsXOn)
						this.terminal.ManuallyEnqueueRawOutgoingDataWithoutSendingIt(new byte[] { MKY.IO.Serial.SerialPort.SerialPortSettings.XOffByte });
					else
						this.terminal.ManuallyEnqueueRawOutgoingDataWithoutSendingIt(new byte[] { MKY.IO.Serial.SerialPort.SerialPortSettings.XOnByte });

					x.ToggleInputXOnXOff();
				}
				else
				{
					throw (new InvalidOperationException("The underlying I/O provider is no XOn/XOff handler!"));
				}
			}
		}

		/// <summary>
		/// Toggles the output break state.
		/// </summary>
		public virtual void RequestToggleOutputBreak()
		{
			if (this.settingsRoot.Terminal.IO.SerialPortOutputBreakIsModifiable)
			{
				MKY.IO.Ports.ISerialPort p = (this.terminal.UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
				if (p != null)
					p.ToggleOutputBreak();
				else
					throw (new InvalidOperationException("The underlying I/O instance is no serial port!"));
			}
		}

		#endregion

		#region Terminal > Send
		//------------------------------------------------------------------------------------------
		// Terminal > Send
		//------------------------------------------------------------------------------------------

		private void Send(byte[] data)
		{
			OnFixedStatusTextRequest("Sending " + data.Length + " bytes...");
			try
			{
				this.terminal.Send(data);
			}
			catch (System.IO.IOException ex)
			{
				OnFixedStatusTextRequest("Error sending " + data.Length + " bytes!");

				string text;
				string title;
				PrepareSendMessageInputRequest(out text, out title);
				OnMessageInputRequest
				(
					text + Environment.NewLine + Environment.NewLine +
					"System error message:" + Environment.NewLine + ex.Message,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				OnTimedStatusTextRequest("Data not sent!");
			}
		}

		private void Send(string data)
		{
			Send(data, false);
		}

		private void SendEol()
		{
			Send("", true);
		}

		private void SendLine(string data)
		{
			Send(data, true);
		}

		private void Send(string data, bool isLine)
		{
			string sendStatusText;
			if (!string.IsNullOrEmpty(data))
				sendStatusText = @"""" + data + @"""";
			else if (isLine)
				sendStatusText = "EOL";
			else
				sendStatusText = "<Nothing>";

			OnFixedStatusTextRequest("Sending " + sendStatusText + "...");
			try
			{
				if (isLine)
					this.terminal.SendLine(data);
				else
					this.terminal.Send(data);
			}
			catch (System.IO.IOException ex)
			{
				OnFixedStatusTextRequest("Error sending " + sendStatusText + "!");

				string text;
				string title;
				PrepareSendMessageInputRequest(out text, out title);
				OnMessageInputRequest
				(
					text + Environment.NewLine + Environment.NewLine +
					"System error message:" + Environment.NewLine + ex.Message,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				OnTimedStatusTextRequest("Data not sent!");
			}
			catch (Domain.Parser.FormatException ex)
			{
				OnFixedStatusTextRequest("Error sending " + sendStatusText + "!");
				OnMessageInputRequest
				(
					"Bad data format:" + Environment.NewLine +
					ex.Message,
					"Format Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest("Data not sent!");
			}
		}

		private void PrepareSendMessageInputRequest(out string text, out string title)
		{
			StringBuilder textBuilder = new StringBuilder();
			StringBuilder titleBuilder = new StringBuilder();

			textBuilder.Append("Unable to write to ");
			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
					textBuilder.Append("port");
					titleBuilder.Append("Serial Port");
					break;

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				case Domain.IOType.Udp:
					textBuilder.Append("socket");
					titleBuilder.Append("Socket");
					break;

				case Domain.IOType.UsbSerialHid:
					textBuilder.Append("device");
					titleBuilder.Append("Device");
					break;

				default:
					throw (new NotImplementedException("I/O type " + this.settingsRoot.IOType + "misses implementation!"));
			}
			textBuilder.Append(":");
			titleBuilder.Append(" Error!");

			text = textBuilder.ToString();
			title = titleBuilder.ToString();
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
			Command c = this.settingsRoot.SendCommand.Command;
			if (c.IsValidText)
			{
				SendText(c);

				// Clear command if desired:
				if (!this.settingsRoot.Send.KeepCommand)
					this.settingsRoot.SendCommand.Command = new Command(); // Set command to "".
			}
		}

		/// <summary>
		/// Sends partial text EOL.
		/// </summary>
		public virtual void SendPartialTextEol()
		{
			SendText(new Command(true));
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="c">Text command to be sent.</param>
		public virtual void SendText(Command c)
		{
			if (c.IsValidText)
			{
				if (c.IsSingleLineText)
				{
					if (SendCommandSettings.IsEasterEggCommand(c.SingleLineText))
						SendLine(SendCommandSettings.EasterEggCommandText);
					else
						SendLine(c.SingleLineText);
				}
				else if (c.IsMultiLineText)
				{
					foreach (string line in c.MultiLineText)
						SendLine(line);
				}
				else if (c.IsPartialText)
				{
					if (!c.IsPartialTextEol)
					{
						Send(c.PartialText);

						// Compile the partial command line for later use:
						if (string.IsNullOrEmpty(this.partialCommandLine))
							this.partialCommandLine = string.Copy(c.PartialText);
						else
							this.partialCommandLine += c.PartialText;
					}
					else
					{
						SendEol();
					}
				}
				else
				{
					throw (new NotSupportedException(@"The command """ + c + @""" has an unknown type!"));
				}

				// Copy line commands into recents, include compiled partial commands:
				if (c.IsSingleLineText || c.IsMultiLineText || c.IsPartialTextEol)
				{
					// Clone the command for the recents collection:
					Command clone;
					if (!c.IsPartialTextEol)
						clone = new Command(c); // 'Normal' case, simply clone the command.
					else                        // Partial, create an equivalent single line command.
						clone = new Command(this.partialCommandLine);

					// Put clone into recent history:
					this.settingsRoot.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(new RecentItem<Command>(clone));
					this.settingsRoot.SendCommand.SetChanged(); // Manual change required because underlying collection is modified.

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
			Command c = this.settingsRoot.SendFile.Command;
			if (c.IsFilePath)
			{
				SendFile(c);
			}
		}

		/// <summary>
		/// Sends given file.
		/// </summary>
		/// <param name="c">File to be sent.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public virtual void SendFile(Command c)
		{
			if (!c.IsValidFilePath)
				return;

			string filePath = c.FilePath;

			try
			{
				if (this.terminal is Domain.TextTerminal)
				{
					string[] lines;
					if (ExtensionSettings.IsXmlFile(filePath))
					{
						// XML.
						lines = XmlReader.LinesFromXmlFile(filePath);
					}
					else if (ExtensionSettings.IsRtfFile(filePath))
					{
						// RTF.
						lines = RtfReader.LinesFromRtfFile(filePath);
					}
					else
					{
						// Text.
						using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
						{
							string s;
							List<string> l = new List<string>();
							while ((s = sr.ReadLine()) != null)
							{
								l.Add(s);
							}
							sr.Close(); // Close file before sending.
							lines = l.ToArray();
						}
					}

					foreach (string line in lines)
					{
						SendLine(line);
					}
				}
				else
				{
					using (FileStream fs = File.OpenRead(filePath))
					{
						byte[] a = new byte[(int)fs.Length];
						fs.Read(a, 0, (int)fs.Length);
						fs.Close(); // Close file before sending.
						Send(a);
					}
				}

				// Put file into history.
				Command clone = new Command(c);
				this.settingsRoot.SendFile.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(new RecentItem<Command>(clone));
				this.settingsRoot.SendFile.SetChanged(); // Manual change required because underlying collection is modified.
			}
			catch (Exception ex)
			{
				OnMessageInputRequest
				(
					"Error while accessing file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
					"System error message:"      + Environment.NewLine + ex.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
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
			if (!VerifyPredefinedArguments(page, command))
				return (false);

			// Verify and process command:
			Model.Types.Command c = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];
			if (c.IsValidText)
			{
				SendText(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendCommand.Command = new Command(c); // Copy command if desired.

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
			if (!VerifyPredefinedArguments(page, command))
				return (false);

			// Verify and process command:
			Model.Types.Command c = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];
			if (c.IsValidText)
			{
				this.settingsRoot.SendCommand.Command = new Command(c);
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

		/// <summary>
		/// Verifies the requested predefined arguments.
		/// </summary>
		/// <param name="page">Page 1..max.</param>
		/// <param name="command">Command 1..max.</param>
		private bool VerifyPredefinedArguments(int page, int command)
		{
			// Verify page index:
			List<Model.Types.PredefinedCommandPage> pages = this.settingsRoot.PredefinedCommand.Pages;
			if ((page < 1) || (page > pages.Count))
				return (false);

			// Verify command index:
			List<Model.Types.Command> commands = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands;
			bool isDefined =
			(
				(commands != null) &&
				(commands.Count >= command) &&
				(commands[command - 1] != null) &&
				(commands[command - 1].IsDefined)
			);

			if (!isDefined)
				return (false);

			// Success!
			return (true);
		}

		#endregion

		#region Terminal > Break
		//------------------------------------------------------------------------------------------
		// Terminal > Break
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Breaks all currently ongoing operations in the terminal.
		/// </summary>
		public virtual void Break()
		{
			OnFixedStatusTextRequest("Breaking operation...");
			this.terminal.Break();
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
		/// Returns the last display line of desired repository for auxiliary purposes.
		/// </summary>
		public virtual Domain.DisplayLine LastDisplayLineAuxiliary(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (this.terminal.LastDisplayLineAuxiliary(repositoryType));
		}

		/// <summary>
		/// Clears the last display line of desired repository for auxiliary purposes.
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
			this.connectChrono = new Chronometer();
			this.connectChrono.Interval = 1000;
			this.connectChrono.TimeSpanChanged += new EventHandler<TimeSpanEventArgs>(totalConnectChrono_TimeSpanChanged);
			this.totalConnectChrono = new Chronometer();
			this.totalConnectChrono.Interval = 1000;
			this.totalConnectChrono.TimeSpanChanged += new EventHandler<TimeSpanEventArgs>(connectChrono_TimeSpanChanged);
		}

		private void StopChronos()
		{
			this.connectChrono.Stop();
			this.totalConnectChrono.Stop();
		}

		private void DisposeChronos()
		{
			if (this.connectChrono != null)
			{
				this.connectChrono.TimeSpanChanged -= new EventHandler<TimeSpanEventArgs>(totalConnectChrono_TimeSpanChanged);
				this.connectChrono.Dispose();
				this.connectChrono = null;
			}
			if (this.totalConnectChrono != null)
			{
				this.totalConnectChrono.TimeSpanChanged -= new EventHandler<TimeSpanEventArgs>(connectChrono_TimeSpanChanged);
				this.totalConnectChrono.Dispose();
				this.totalConnectChrono = null;
			}
		}

		/// <summary></summary>
		public virtual TimeSpan ConnectTime
		{
			get
			{
				AssertNotDisposed();
				return (this.connectChrono.TimeSpan);
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
		public virtual void RestartConnectTime()
		{
			AssertNotDisposed();
			this.connectChrono.Restart();
			this.totalConnectChrono.Restart();
		}

		private void connectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			// Don't fire event. Events are fired by total connect chrono anyway.
		}

		private void totalConnectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			OnIOConnectTimeChanged(e);
		}

		#endregion

		#region Terminal > Count and Rate Status
		//------------------------------------------------------------------------------------------
		// Terminal > Count and Rate Status
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
				return (this.txByteRate.Value);
			}
		}

		/// <summary></summary>
		public virtual int TxLineRate
		{
			get
			{
				AssertNotDisposed();
				return (this.txLineRate.Value);
			}
		}

		/// <summary></summary>
		public virtual int RxByteRate
		{
			get
			{
				AssertNotDisposed();
				return (this.rxByteRate.Value);
			}
		}

		/// <summary></summary>
		public virtual int RxLineRate
		{
			get
			{
				AssertNotDisposed();
				return (this.rxLineRate.Value);
			}
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
			int tick = 250;
			int interval = 1000;
			int window = 5000;

			this.txByteRate = new Rate(tick, interval, window);
			this.txLineRate = new Rate(tick, interval, window);
			this.rxByteRate = new Rate(tick, interval, window);
			this.rxLineRate = new Rate(tick, interval, window);

			this.txByteRate.Changed += new EventHandler<RateEventArgs>(rate_Changed);
			this.txLineRate.Changed += new EventHandler<RateEventArgs>(rate_Changed);
			this.rxByteRate.Changed += new EventHandler<RateEventArgs>(rate_Changed);
			this.rxLineRate.Changed += new EventHandler<RateEventArgs>(rate_Changed);
		}

		private void DisposeRates()
		{
			if (this.txByteRate != null)
			{
				this.txByteRate.Changed -= new EventHandler<RateEventArgs>(rate_Changed);
				this.txByteRate.Dispose();
				this.txByteRate = null;
			}

			if (this.txLineRate != null)
			{
				this.txLineRate.Changed -= new EventHandler<RateEventArgs>(rate_Changed);
				this.txLineRate.Dispose();
				this.txLineRate = null;
			}

			if (this.rxByteRate != null)
			{
				this.rxByteRate.Changed -= new EventHandler<RateEventArgs>(rate_Changed);
				this.rxByteRate.Dispose();
				this.rxByteRate = null;
			}

			if (this.rxLineRate != null)
			{
				this.rxLineRate.Changed -= new EventHandler<RateEventArgs>(rate_Changed);
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

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.ControlPins);
				}

				return (new MKY.IO.Ports.SerialPortControlPins());
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

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.ControlPinCount);
				}

				return (new MKY.IO.Ports.SerialPortControlPinCount());
			}
		}

		/// <summary></summary>
		public virtual int SentXOnCount
		{
			get
			{
				AssertNotDisposed();

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.SentXOnCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int SentXOffCount
		{
			get
			{
				AssertNotDisposed();

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.SentXOffCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOnCount
		{
			get
			{
				AssertNotDisposed();

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.ReceivedXOnCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOffCount
		{
			get
			{
				AssertNotDisposed();

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.ReceivedXOffCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual void ResetFlowControlCount()
		{
			AssertNotDisposed();

			if (UnderlyingIOIsSerialPort)
			{
				MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
				if (port != null)
					port.ResetFlowControlCount();
			}
		}

		/// <summary></summary>
		public virtual int OutputBreakCount
		{
			get
			{
				AssertNotDisposed();

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.OutputBreakCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual int InputBreakCount
		{
			get
			{
				AssertNotDisposed();

				if (UnderlyingIOIsSerialPort)
				{
					MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
					if (port != null)
						return (port.InputBreakCount);
				}

				return (0);
			}
		}

		/// <summary></summary>
		public virtual void ResetBreakCount()
		{
			AssertNotDisposed();

			if (UnderlyingIOIsSerialPort)
			{
				MKY.IO.Serial.SerialPort.SerialPort port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
				if (port != null)
					port.ResetBreakCount();
			}
		}

		#endregion

		#endregion

		#region Log
		//==========================================================================================
		// Log
		//==========================================================================================

		/// <summary></summary>
		public virtual bool BeginLog()
		{
			try
			{
				// reapply settings NOW, makes sure date/time in filenames is refreshed
				this.log.Settings = this.settingsRoot.Log;
				this.log.Begin();
				this.settingsRoot.LogIsStarted = true;

				return (true);
			}
			catch (System.IO.IOException ex)
			{
				OnMessageInputRequest
				(
					"Unable to begin log."                               + Environment.NewLine + Environment.NewLine +
					"System message:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
					ApplicationInfo.ProductName + " hint:" + Environment.NewLine +
					"Log file could already be in use.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual void ClearLog()
		{
			try
			{
				this.log.Clear();
			}
			catch (System.IO.IOException ex)
			{
				OnMessageInputRequest
				(
					"Unable to clear log."                               + Environment.NewLine + Environment.NewLine +
					"System message:" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
					ApplicationInfo.ProductName + " hint:" + Environment.NewLine +
					"Log file could already be in use.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
			}
		}

		/// <summary></summary>
		public virtual void EndLog()
		{
			EndLog(true);
		}

		/// <summary></summary>
		public virtual void EndLog(bool saveStatus)
		{
			try
			{
				this.log.End();

				if (saveStatus)
					this.settingsRoot.LogIsStarted = false;
			}
			catch (System.IO.IOException ex)
			{
				OnMessageInputRequest
				(
					"Unable to end log." + Environment.NewLine + Environment.NewLine +
					"System message:" + Environment.NewLine + ex.Message,
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
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
		protected virtual void OnRepositoryCleared(Domain.RepositoryEventArgs e)
		{
			EventHelper.FireSync<Domain.RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(Domain.RepositoryEventArgs e)
		{
			EventHelper.FireSync<Domain.RepositoryEventArgs>(RepositoryReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			EventHelper.FireSync<StatusTextEventArgs>(FixedStatusTextRequest, this, new StatusTextEventArgs(text));
		}

		/// <summary></summary>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			EventHelper.FireSync<StatusTextEventArgs>(TimedStatusTextRequest, this, new StatusTextEventArgs(text));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			if (this.startArgs.Interactive)
			{
				MessageInputEventArgs e = new MessageInputEventArgs(text, caption, buttons, icon);
				EventHelper.FireSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				// Ensure that the request is processed!
				if (e.Result == DialogResult.None)
					throw (new InvalidOperationException(@"A 'Message Input' request by terminal """ + Caption + @""" was not processed by the application!"));

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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void WriteDebugMessageLine(string message)
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
