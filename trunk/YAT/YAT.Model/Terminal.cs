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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Specialized;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.IO;
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
		private const int SequentialIdCounterDefault = (TerminalIds.FirstSequentialId - 1);

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticSequentialIdCounter = SequentialIdCounterDefault;

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
		/// Needed to test the ID feature of terminals and workspace.
		/// </remarks>
		public static void ResetSequentialIdCounter()
		{
			staticSequentialIdCounter = SequentialIdCounterDefault;
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// Workaround to the following issue:
		/// 
		/// A test (e.g. 'FileHandlingTest') needs to verify the settings files after calling
		/// <see cref="Main.Exit()"/>. But at that moment, the settings have already been disposed
		/// of and can no longer be accessed.
		/// The first approach was to disable disposal in <see cref="Close()"/>. But that leads to
		/// remaining resources, resulting in significant slow-down when exiting NUnit.
		/// The second approach was to retrieve the required information *before* exiting, i.e.
		/// calling <see cref="Main.Exit()"/>. But that doesn't work at all, since auto-save paths
		/// are only evaluated *at* <see cref="Main.Exit()"/>.
		/// 
		/// This workaround is considered the best option to solve this issue.
		/// </summary>
		/// <remarks>
		/// Note that it is not possible to mark a property with [Conditional("TEST")].
		/// </remarks>
		public bool DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification { get; set; }

		/// <summary>
		/// A dedicated event helper to allow autonomously ignoring exceptions when disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Terminal).FullName);

		private TerminalStartArgs startArgs;
		private Guid guid;
		private int sequentialId;
		private string sequentialName;
		private string fileName;

		// Settings:
		private DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler;
		private TerminalSettingsRoot settingsRoot;

		// Terminal:
		private Domain.Terminal terminal;

		// Logs:
		private Log.Provider log;

		// AutoResponse:
		private int autoResponseCount;
		private AutoTriggerHelper autoResponseTriggerHelper;
		private object autoResponseTriggerHelperSyncObj = new object();

		// AutoAction:
		private int autoActionCount;
		private AutoTriggerHelper autoActionTriggerHelper;
		private object autoActionTriggerHelperSyncObj = new object();
		private bool autoActionClearRepositoriesOnSubsequentRxIsArmed; // = false;

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
		public event EventHandler<Domain.IOControlEventArgs> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<TimeSpanEventArgs> IOConnectTimeChanged;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		public event EventHandler IOCountChanged_Promptly;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		public event EventHandler IORateChanged_Promptly;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		public event EventHandler IORateChanged_Decimated;

		/// <summary></summary>
		public event EventHandler<Domain.IOErrorEventArgs> IOError;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsSent;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsReceived;

		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLinePartSentReplaced"/></remarks>
		public event EventHandler<Domain.DisplayLinePartEventArgs> CurrentDisplayLinePartSentReplaced;

		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLinePartReceivedReplaced"/></remarks>
		public event EventHandler<Domain.DisplayLinePartEventArgs> CurrentDisplayLinePartReceivedReplaced;

		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineSentCleared"/></remarks>
		public event EventHandler CurrentDisplayLineSentCleared;

		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineReceivedCleared"/></remarks>
		public event EventHandler CurrentDisplayLineReceivedCleared;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesSent;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesReceived;

		/// <summary></summary>
		public event EventHandler<EventArgs<Domain.RepositoryType>> RepositoryCleared;

		/// <summary></summary>
		public event EventHandler<EventArgs<Domain.RepositoryType>> RepositoryReloaded;

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoResponseCountChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoActionCountChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler ResetStatusTextRequest;

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

		/// <summary></summary>
		public event EventHandler<EventArgs> ExitRequest;

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

				// Set ID and name(s):
				this.sequentialId = ++staticSequentialIdCounter;
				this.sequentialName = TerminalText + this.sequentialId.ToString(CultureInfo.CurrentCulture);
				if (!this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFilePathIsValid)
					this.fileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

				// Create underlying terminal:
				this.terminal = Domain.TerminalFactory.CreateTerminal(this.settingsRoot.Terminal);
				AttachTerminalEventHandlers();

				// Create log:
				this.log = new Log.Provider(this.settingsRoot.Log, (EncodingEx)this.settingsRoot.TextTerminal.Encoding, this.settingsRoot.Format);

				// Create AutoResponse/Action:
				CreateAutoResponseHelper();
				CreateAutoActionHelper();

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
				throw; // Rethrow!
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, terminal and log have already been closed, otherwise...

					// ...detach event handlers to ensure that no more events are received...
					DetachTerminalEventHandlers();

					// ...ensure that timed objects are stopped and do not raise events anymore...
					DisposeRates();
					DisposeChronos();
					DisposeAutoResponseHelper();
					DisposeAutoActionHelper();

					// ...close and dispose of terminal and log...
					CloseAndDisposeTerminal();
					DisposeLog();

					// ...and finally dispose of the settings:
					if (!DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification)
					{
						DetachSettingsEventHandlers();
						DisposeSettingsHandler();
					}
				}

				// Set state to disposed:
				IsDisposed = true;

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
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
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
		public virtual int SequentialId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.sequentialId);
			}
		}

		/// <summary>
		/// The name incrementally assigned terminal name 'Terminal1', 'Terminal2',...
		/// </summary>
		public virtual string SequentialName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.sequentialName);
			}
		}

		/// <summary>
		/// The file name if the user has saved the terminal; otherwise <see cref="string.Empty"/>.
		/// </summary>
		public virtual string FileName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (!string.IsNullOrEmpty(this.fileName))
					return (this.fileName);

				return ("");
			}
		}

		/// <summary>
		/// The optional user defined terminal name; otherwise <see cref="string.Empty"/>.
		/// </summary>
		public virtual string UserName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsRoot != null)
				{
					if (!string.IsNullOrEmpty(this.settingsRoot.UserName))
						return (this.settingsRoot.UserName);
				}

				return ("");
			}
		}

		/// <summary>
		/// The indicated name, i.e. either the <see cref="UserName"/>, <see cref="FileName"/>
		/// or <see cref="SequentialName"/>.
		/// </summary>
		public virtual string IndicatedName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (!string.IsNullOrEmpty(UserName))
					return (UserName);

				if (!string.IsNullOrEmpty(FileName))
					return (FileName);

				return (SequentialName);
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
		public virtual Domain.IOType IOType
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsRoot != null)
					return (this.settingsRoot.IOType);
				else
					return (Domain.IOType.Unknown);
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
					return (this.log.AnyIsOn);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool AllLogsAreOn
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.log.AllAreOn);
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

				var sb = new StringBuilder();

				if ((this.settingsHandler == null) || (this.settingsRoot == null))
				{
					sb.Append("[");
					sb.Append(IndicatedName);
					sb.Append("]");
				}
				else
				{
					sb.Append("[");
					{
						if (this.settingsHandler.SettingsFileIsReadOnly)
							sb.Append("#");

						sb.Append(IndicatedName);

						if (this.settingsHandler.SettingsFileIsReadOnly)
							sb.Append("#");

						if (this.settingsRoot.ExplicitHaveChanged)
							sb.Append(" *");
					}
					sb.Append("]");

					switch (this.settingsRoot.IOType)
					{
						case Domain.IOType.SerialPort:
						{
							string portNameAndCaption;
							bool autoReopenEnabled;

							var port = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
							if (port != null) // Effective settings from port object:
							{
								var s = port.Settings;
								portNameAndCaption = s.PortId.ToNameAndCaptionString();
								autoReopenEnabled  = s.AutoReopen.Enabled;
							}
							else // Fallback to settings object tree:
							{
								var s = this.settingsRoot.IO.SerialPort;
								portNameAndCaption = s.PortId.ToNameAndCaptionString();
								autoReopenEnabled  = s.AutoReopen.Enabled;
							}

							sb.Append(" - ");
							sb.Append(portNameAndCaption);
							sb.Append(" - ");

							if (IsStarted)
							{
								if (IsOpen)
								{
									sb.Append("Open");
									sb.Append(" - ");
									sb.Append(IsConnected ? "Connected" : "Disconnected"); // Break?
								}
								else if (autoReopenEnabled)
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
							var s = this.settingsRoot.IO.Socket;

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
							var s = this.settingsRoot.IO.Socket;

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
							var s = this.settingsRoot.IO.Socket;
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
							var s = this.settingsRoot.IO.Socket;
							sb.Append(" - ");
							sb.Append(s.RemoteEndPointString);
							sb.Append(" - ");
							sb.Append(IsOpen ? "Open" : "Closed");
							break;
						}

						case Domain.IOType.UdpServer:
						{
							var s = this.settingsRoot.IO.Socket;
							sb.Append(" - ");
							sb.Append("Receive:");
							sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" - ");
							sb.Append(IsOpen ? "Open" : "Closed");
							break;
						}

						case Domain.IOType.UdpPairSocket:
						{
							var s = this.settingsRoot.IO.Socket;
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
							var s = this.settingsRoot.IO.UsbSerialHidDevice;
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
									else if (device.Settings.AutoOpen)
										sb.Append("Connected - Waiting for reopen");
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
							throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.settingsRoot.IOType + "' is an I/O type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				var sb = new StringBuilder();

				if (this.settingsRoot != null)
				{
					switch (this.settingsRoot.IOType)
					{
						case Domain.IOType.SerialPort:
						{
							string portNameAndCaption;
							string communication;
							bool autoReopenEnabled;

							var port = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
							if (port != null) // Effective settings from port object:
							{
								var s = port.Settings;
								portNameAndCaption = s.PortId.ToNameAndCaptionString();
								communication      = s.Communication.ToString();
								autoReopenEnabled  = s.AutoReopen.Enabled;
							}
							else // Fallback to settings object tree;
							{
								var s = this.settingsRoot.IO.SerialPort;
								portNameAndCaption = s.PortId.ToNameAndCaptionString();
								communication      = s.Communication.ToString();
								autoReopenEnabled  = s.AutoReopen.Enabled;
							}

							sb.Append("Serial port "); // Not adding "COM" as the port name will already state that.
							sb.Append(portNameAndCaption);
							sb.Append(" (" + communication + ")");

							if (IsStarted)
							{
								if (IsOpen)
								{
									sb.Append(" is open and ");
									sb.Append(IsConnected ? "connected" : "disconnected");
								}
								else if (autoReopenEnabled)
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
							var s = this.settingsRoot.IO.Socket;
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
										sb.Append(" to ");
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
							var s = this.settingsRoot.IO.Socket;
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
									if ((remoteEndPoint != null) && (MKY.Net.IPAddressEx.NotEqualsNone(remoteEndPoint.Address)))
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
							var s = this.settingsRoot.IO.UsbSerialHidDevice;
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
									else if (device.Settings.AutoOpen)
										sb.Append(" is connected but waiting for reopen");
									else
										sb.Append(" is connected but closed");
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

		/// <summary>
		/// Returns the <see cref="MKY.IO.Ports.SerialPortId"/> for <see cref="Domain.IOType.SerialPort"/>,
		/// <c>null</c> for all other terminal types.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortId IOSerialPortId
		{
			get
			{
				AssertNotDisposed();

				if (this.settingsRoot != null)
				{
					if (this.settingsRoot.IOType == Domain.IOType.SerialPort)
						return (this.settingsRoot.IO.SerialPort.PortId);
				}

				return (null);
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
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
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
				var result = CheckIOAvailability();
				switch (result)
				{
					case CheckResult.OK:     return (StartIO());
					case CheckResult.Cancel: return (false);
					case CheckResult.Ignore: return (true);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + result.ToString() + "' is a result that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			return (true);
		}

		/// <summary>
		/// Applies new terminal settings.
		/// </summary>
		/// <remarks>
		/// Using <see cref="ExplicitSettings"/> instead of simply using
		/// <see cref="Domain.Settings.TerminalSettings"/> for two reasons:
		/// <list type="bullet">
		/// <item><description>Handling of <see cref="ExplicitSettings.UserName"/>.</description></item>
		/// <item><description>Prepared for future migration to tree view dialog containing all settings.</description></item>
		/// </list>
		/// </remarks>
		public virtual void ApplyTerminalSettings(ExplicitSettings settings)
		{
			AssertNotDisposed();

			// Attention:
			// Similar code exists in Domain.Terminal.ApplySettings() but without changing the terminal settings (.yat file).
			// Changes here may have to be applied there too.

			if (this.terminal.IsStarted) // Terminal is started, stop and restart it with the new settings:
			{
				// Note that the whole terminal will be recreated. Thus, it must also be recreated if non-IO settings have changed.

				if (StopIO(false))
				{
					this.settingsRoot.SuspendChangeEvent();
					try
					{
						this.settingsRoot.Explicit = settings;

						DetachTerminalEventHandlers();
						using (var oldTerminal = this.terminal)
						{
							this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
						}
						AttachTerminalEventHandlers();
					}
					finally
					{
						this.settingsRoot.ResumeChangeEvent();
					}
					this.terminal.RefreshRepositories();

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
			else // Terminal is stopped, simply set the new settings:
			{
				this.settingsRoot.SuspendChangeEvent();
				try
				{
					this.settingsRoot.Explicit = settings;

					DetachTerminalEventHandlers();
					using (var oldTerminal = this.terminal)
					{
						this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
					}
					AttachTerminalEventHandlers();
				}
				finally
				{
					this.settingsRoot.ResumeChangeEvent();
				}
				this.terminal.RefreshRepositories();

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
		private Domain.Endianness settingsRoot_Changed_endiannessOld = Domain.Settings.IOSettings.EndiannessDefault;

		/// <remarks>
		/// Required to solve the issue described in bug #223 "Settings events should state the exact settings diff".
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'sendImmediatelyOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool settingsRoot_Changed_sendImmediatelyOld = Domain.Settings.SendSettingsText.SendImmediatelyDefault;

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
				UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoResponse))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoAction))
			{
				UpdateAutoAction(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
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
			// \ToDo: ApplySettings should be called here => FR#309.

			if (e.Inner == null)
			{
				if (settingsRoot_Changed_terminalTypeOld != this.settingsRoot.TerminalType) {
					settingsRoot_Changed_terminalTypeOld = this.settingsRoot.TerminalType;

					// Terminal type has changed, recreate AutoResponse/Action:
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
					UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal.IO))
			{
				if (settingsRoot_Changed_endiannessOld != this.settingsRoot.Terminal.IO.Endianness) {
					settingsRoot_Changed_endiannessOld = this.settingsRoot.Terminal.IO.Endianness;

					// Endianness has changed, recreate AutoResponse/Action:
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
					UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal.Send))
			{
				if (settingsRoot_Changed_sendImmediatelyOld != this.settingsRoot.Terminal.Send.Text.SendImmediately) {
					settingsRoot_Changed_sendImmediatelyOld = this.settingsRoot.Terminal.Send.Text.SendImmediately;

					// Send immediately has changed, reset the command:
					this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal.TextTerminal))
			{
				this.log.TextTerminalEncoding = (EncodingEx)this.settingsRoot.Terminal.TextTerminal.Encoding;

				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
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
		public virtual bool SettingsFileIsReadOnly
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileIsReadOnly);
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
			// AssertNotDisposed() is called by 'Save(...)' below.

			bool isCanceled;                               // Save even if not changed since explicitly requesting saving.
			return (SaveConsiderately(false, true, true, true, false, out isCanceled));
		}

		/// <summary>
		/// Silently tries to save terminal to file, i.e. without any user interaction.
		/// </summary>
		public virtual bool TrySaveConsideratelyWithoutUserInteraction(bool isWorkspaceClose, bool autoSaveIsAllowed)
		{
			bool isCanceled;
			return (SaveConsiderately(isWorkspaceClose, autoSaveIsAllowed, false, false, false, out isCanceled));
		}

		/// <summary>
		/// This method implements the logic that is needed when saving, opposed to the method
		/// <see cref="SaveToFile"/> which just performs the actual save, i.e. file handling.
		/// </summary>
		/// <param name="isWorkspaceClose">Indicates that workspace closes.</param>
		/// <param name="autoSaveIsAllowed">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="userInteractionIsAllowed">Indicates whether user interaction is allowed.</param>
		/// <param name="saveEvenIfNotChanged">Indicates whether save must happen even if not changed.</param>
		/// <param name="canBeCanceled">Indicates whether save can be canceled.</param>
		/// <param name="isCanceled">Indicates whether save has been canceled.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool SaveConsiderately(bool isWorkspaceClose, bool autoSaveIsAllowed, bool userInteractionIsAllowed, bool saveEvenIfNotChanged, bool canBeCanceled, out bool isCanceled)
		{
			AssertNotDisposed();

			isCanceled = false;

			autoSaveIsAllowed = EvaluateWhetherAutoSaveIsAllowedIndeed(autoSaveIsAllowed);

			// -------------------------------------------------------------------------------------
			// Skip save if there is no reason to save:
			// -------------------------------------------------------------------------------------

			if (ThereIsNoReasonToSave(autoSaveIsAllowed, saveEvenIfNotChanged))
				return (true);

			// -------------------------------------------------------------------------------------
			// Create auto save file path or request manual/normal file path if necessary:
			// -------------------------------------------------------------------------------------

			if (!this.settingsHandler.SettingsFilePathIsValid)
			{
				if (autoSaveIsAllowed) {
					this.settingsHandler.SettingsFilePath = ComposeAbsoluteAutoSaveFilePath();
				}
				else if (userInteractionIsAllowed) {
					return (RequestNormalSaveAsFromUser(isWorkspaceClose, false, out isCanceled));
				}
				else {
					return (false); // Let save fail if the file path is invalid and no user interaction is allowed
				}
			}
			else if (this.settingsRoot.AutoSaved && !autoSaveIsAllowed)
			{
				// Ensure that former auto files are 'Saved As' if auto save is no longer allowed:
				if (userInteractionIsAllowed) {
					return (RequestNormalSaveAsFromUser(isWorkspaceClose, false, out isCanceled));
				}
				else {
					return (false);
				}
			}

			// -------------------------------------------------------------------------------------
			// Handle write-protected or non-existant file:
			// -------------------------------------------------------------------------------------

			if (!SettingsFileIsWritable || SettingsFileNoLongerExists)
			{
				if (this.settingsRoot.ExplicitHaveChanged || saveEvenIfNotChanged)
				{
					if (userInteractionIsAllowed) {
						return (RequestRestrictedSaveAsFromUser(isWorkspaceClose, autoSaveIsAllowed, canBeCanceled, out isCanceled));
					}
					else {
						return (false); // Let save of explicit change fail if file is restricted.
					}
				}
				else // ImplicitHaveChanged:
				{
					return (true); // Skip save of implicit change as save is currently not feasible.
				}
			}

			// -------------------------------------------------------------------------------------
			// Save is feasible:
			// -------------------------------------------------------------------------------------

			return (SaveToFile(autoSaveIsAllowed, null));
		}

		/// <summary></summary>
		protected virtual bool EvaluateWhetherAutoSaveIsAllowedIndeed(bool autoSaveIsAllowed)
		{
			// Do not auto save if file already exists but isn't auto saved:
			if (autoSaveIsAllowed && this.settingsHandler.SettingsFilePathIsValid && !this.settingsRoot.AutoSaved)
				return (false);

			return (autoSaveIsAllowed);
		}

		/// <summary></summary>
		protected virtual bool ThereIsNoReasonToSave(bool autoSaveIsAllowed, bool saveEvenIfNotChanged)
		{
			if (saveEvenIfNotChanged)
				return (false);

			// Ensure that former auto files are 'Saved As' if auto save is no longer allowed:
			if (this.settingsRoot.AutoSaved && !autoSaveIsAllowed)
				return (false);

			// No need to save if settings are up to date:
			return (this.settingsHandler.SettingsFileIsUpToDate && !this.settingsRoot.HaveChanged);
		}

		/// <summary></summary>
		protected virtual string ComposeAbsoluteAutoSaveFilePath()
		{
			var sb = new StringBuilder();

			sb.Append(Application.Settings.GeneralSettings.AutoSaveRoot);
			sb.Append(Path.DirectorySeparatorChar);
			sb.Append(Application.Settings.GeneralSettings.AutoSaveTerminalFileNamePrefix);
			sb.Append(Guid.ToString());
			sb.Append(ExtensionHelper.TerminalFile);

			return (sb.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool RequestNormalSaveAsFromUser(bool isWorkspaceClose, bool autoSaveIsAllowed, out bool isCanceled)
		{
			if (isWorkspaceClose && !autoSaveIsAllowed)
			{
				// Ask user whether to save terminal, to give user to possibility to chose 'No'.
				// This is required since the 'Save File Dialog' only offers [OK] and [Cancel].

				var dr = OnMessageInputRequest
				(
					"Save terminal?",
					IndicatedName,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				);

				switch (dr)
				{
					case DialogResult.Yes:
						break; // Continue with save below.

					case DialogResult.No:
						isCanceled = false;
						return (true); // Success, as user explicitly chose 'No'.

					case DialogResult.Cancel:
					default:
						isCanceled = true;
						return (false);
				}
			}

			switch (OnSaveAsFileDialogRequest()) // 'Save File Dialog' offers [OK] and [Cancel].
			{
				case DialogResult.OK:
					isCanceled = false;
					return (true);

				default: // incl. Cancel:
					isCanceled = true;
					return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool RequestRestrictedSaveAsFromUser(bool isWorkspaceClose, bool autoSaveIsAllowed, bool canBeCanceled, out bool isCanceled)
		{
			isCanceled = false;

			string reason;
			if      ( SettingsFileNoLongerExists)
				reason = "The file no longer exists.";
			else if (!SettingsFileIsWritable)
				reason = "The file is write-protected.";
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'SaveAs'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			var message = new StringBuilder();
			message.AppendLine("Unable to save file");
			message.AppendLine(this.settingsHandler.SettingsFilePath);
			message.AppendLine();
			message.Append    (reason + " Would you like to save the file at another location?");

			var dr = OnMessageInputRequest
			(
				message.ToString(),
				"File Error",
				(canBeCanceled ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo),
				MessageBoxIcon.Question
			);

			switch (dr)
			{
				case DialogResult.Yes:
					return (RequestNormalSaveAsFromUser(isWorkspaceClose, autoSaveIsAllowed, out isCanceled));

				case DialogResult.No:
					OnTimedStatusTextRequest("Terminal not saved!");
					return (true);

				default:
					// No need for TextRequest("Canceled!") as parent will handle cancel.
					isCanceled = true;
					return (false);
			}
		}

		/// <summary>
		/// Saves settings to given file.
		/// </summary>
		public virtual bool SaveAs(string filePath)
		{
			AssertNotDisposed();

			var absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(filePath);

			// Request the deletion of the obsolete auto saved settings file given the new file is different:
			string autoSaveFilePathToDelete = null;
			if (this.settingsRoot.AutoSaved && (!PathEx.Equals(absoluteFilePath, this.settingsHandler.SettingsFilePath)))
				autoSaveFilePathToDelete = this.settingsHandler.SettingsFilePath;

			// Set the new file path...
			this.settingsHandler.SettingsFilePath = absoluteFilePath;

			// ...and save the terminal:
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual bool SaveToFile(bool isAutoSave, string autoSaveFilePathToDelete)
		{
			OnFixedStatusTextRequest("Saving terminal...");

			bool success = false;

			try
			{
				this.settingsHandler.Settings.AutoSaved = isAutoSave;
				this.settingsHandler.Save();
				success = true;

				if (!isAutoSave)
					this.fileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, isAutoSave));
				OnTimedStatusTextRequest("Terminal saved.");

				if (!isAutoSave)
					SetRecent(this.settingsHandler.SettingsFilePath);

				// Try to delete existing auto save file:
				if (!string.IsNullOrEmpty(autoSaveFilePathToDelete))
				{
					// Ensure that this is not the current file!
					if (!PathEx.Equals(autoSaveFilePathToDelete, this.settingsHandler.SettingsFilePath))
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
		/// In case of a workspace close, <see cref="CloseConsiderately"/> below must be called
		/// with the first argument set to <c>true</c>.
		/// 
		/// In case of intended close of one or all terminals, the user intentionally wants to close
		/// the terminal(s), thus, this method will not try to auto save.
		/// </remarks>
		public virtual bool Close()
		{
			return (CloseConsiderately(false, true, false, true)); // See remarks above.
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
		public virtual bool CloseConsiderately(bool isWorkspaceClose, bool doSave, bool autoSaveIsAllowed, bool autoDeleteIsRequested)
		{
			AssertNotDisposed();

			OnFixedStatusTextRequest("Closing terminal...");

			// Keep info of existing former auto file:
			bool formerExistingAutoFileAutoSaved = this.settingsRoot.AutoSaved;
			string formerExistingAutoFilePath = null;
			if (this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFileExists)
				formerExistingAutoFilePath = this.settingsHandler.SettingsFilePath;

			// -------------------------------------------------------------------------------------
			// Evaluate save requirements:
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
			// Try auto save if allowed:
			// -------------------------------------------------------------------------------------

			if (!success && doSave)
				success = TrySaveConsideratelyWithoutUserInteraction(isWorkspaceClose, autoSaveIsAllowed); // Try auto save.

			// -------------------------------------------------------------------------------------
			// If not successfully saved so far, evaluate next step according to rules above:
			// -------------------------------------------------------------------------------------

			// Normal file (w3, w4, t3, t4):
			if (!success && doSave && !this.settingsRoot.AutoSaved)
			{
				var dr = OnMessageInputRequest
				(
					"Save terminal?",
					IndicatedName,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				);

				switch (dr)
				{
					case DialogResult.Yes: success = Save(); break;
					case DialogResult.No:  success = true;   break;

					case DialogResult.Cancel:
					default:
					{
						OnTimedStatusTextRequest("Canceled, terminal not closed.");
						return (false);
					}
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
			// Stop the underlying items:
			// -------------------------------------------------------------------------------------

			if (success && this.terminal.IsStarted)
			{
				success = StopIO(false);
			}

			if (success && this.log.AnyIsOn)
			{
				SwitchLogOff();
			}

			// -------------------------------------------------------------------------------------
			// Finally, cleanup and signal state:
			// -------------------------------------------------------------------------------------

			if (success)
			{
				// Status text request must be before closed event, closed event may close the view:
				OnTimedStatusTextRequest("Terminal successfully closed.");

				// Discard potential exceptions already before signalling the close! Required to
				// prevent exceptions on still ongoing asynchronous callbacks trying to synchronize
				// event callbacks onto the terminal form which is going to be closed/disposed by
				// the handler of the 'Closed' event below!
				this.eventHelper.DiscardAllExceptions();

				OnClosed(new ClosedEventArgs(isWorkspaceClose));

				// The terminal shall dispose of itself to free all resources for sure. It must be
				// done AFTER it raised the 'Closed' event and all subscribers of the event may still
				// refer to a non-disposed object. This is especially important, as the order of the
				// subscribers is not fixed, i.e. 'Model.Workspace' may dispose of the terminal
				// before 'View.Terminal' receives the event callback!
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
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Add(recentFile);
			ApplicationSettings.LocalUserSettings.RecentFiles.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveLocalUserSettings();
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

				this.terminal.CurrentDisplayLinePartSentReplaced     += terminal_CurrentDisplayLinePartSentReplaced;
				this.terminal.CurrentDisplayLinePartReceivedReplaced += terminal_CurrentDisplayLinePartReceivedReplaced;
				this.terminal.CurrentDisplayLineSentCleared          += terminal_CurrentDisplayLineSentCleared;
				this.terminal.CurrentDisplayLineReceivedCleared      += terminal_CurrentDisplayLineReceivedCleared;

				this.terminal.DisplayLinesSent     += terminal_DisplayLinesSent;
				this.terminal.DisplayLinesReceived += terminal_DisplayLinesReceived;
				this.terminal.RepositoryCleared    += terminal_RepositoryCleared;
				this.terminal.RepositoryReloaded   += terminal_RepositoryReloaded;
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

				this.terminal.CurrentDisplayLinePartSentReplaced     -= terminal_CurrentDisplayLinePartSentReplaced;
				this.terminal.CurrentDisplayLinePartReceivedReplaced -= terminal_CurrentDisplayLinePartReceivedReplaced;
				this.terminal.CurrentDisplayLineSentCleared          -= terminal_CurrentDisplayLineSentCleared;
				this.terminal.CurrentDisplayLineReceivedCleared      -= terminal_CurrentDisplayLineReceivedCleared;

				this.terminal.DisplayLinesSent     -= terminal_DisplayLinesSent;
				this.terminal.DisplayLinesReceived -= terminal_DisplayLinesReceived;
				this.terminal.RepositoryCleared    -= terminal_RepositoryCleared;
				this.terminal.RepositoryReloaded   -= terminal_RepositoryReloaded;
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

		#region Terminal > Auxiliary
		//------------------------------------------------------------------------------------------
		// Terminal > Auxiliary
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual Domain.DisplayLine IOStatusToDisplayLine(DateTime ts)
		{
			var sep   = SettingsRoot.Display.InfoSeparatorCache;
			var left  = SettingsRoot.Display.InfoEnclosureLeftCache;
			var right = SettingsRoot.Display.InfoEnclosureRightCache;

			var virtualLine = new Domain.DisplayLine(5); // Preset the required capacity to improve memory management.
			virtualLine.Add(new Domain.DisplayElement.LineStart());

			if (SettingsRoot.Display.ShowTimeStamp)
			{
				virtualLine.Add(new Domain.DisplayElement.TimeStampInfo(ts, SettingsRoot.Display.TimeStampFormat, SettingsRoot.Display.TimeStampUseUtc, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			virtualLine.Add(new Domain.DisplayElement.PortInfo(Domain.Direction.None, IOStatusText, left, right));
			virtualLine.Add(new Domain.DisplayElement.LineBreak());

			return (virtualLine);
		}

		/// <summary></summary>
		protected virtual Domain.DisplayLine IOControlToDisplayLine(DateTime ts, Domain.IOControlEventArgs e, bool includeIOStatusText)
		{
			var sep   = SettingsRoot.Display.InfoSeparatorCache;
			var left  = SettingsRoot.Display.InfoEnclosureLeftCache;
			var right = SettingsRoot.Display.InfoEnclosureRightCache;
			                                                                    // Forsee capacity for separators.
			var virtualLine = new Domain.DisplayLine(1 + 2 + 2 + (e.Texts.Count * 2) + 1); // Preset the required capacity to improve memory management.
			virtualLine.Add(new Domain.DisplayElement.LineStart());

			if (SettingsRoot.Display.ShowTimeStamp)
			{
				virtualLine.Add(new Domain.DisplayElement.TimeStampInfo(ts, SettingsRoot.Display.TimeStampFormat, SettingsRoot.Display.TimeStampUseUtc, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			if (includeIOStatusText)
			{
				virtualLine.Add(new Domain.DisplayElement.PortInfo(Domain.Direction.None, IOStatusText, left, right));

				if (e.Texts.Count > 0)
					virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}
			                                                          // Forsee capacity for separators.
			var c = new Domain.DisplayElementCollection(e.Texts.Count * 2); // Preset the required capacity to improve memory management.
			foreach (var t in e.Texts)
			{
				if (c.Count > 0)
					c.Add(new Domain.DisplayElement.InfoSeparator(SettingsRoot.Display.InfoSeparatorCache));

				c.Add(new Domain.DisplayElement.IOControl((Domain.Direction)e.Direction, t));
			}

			virtualLine.AddRange(c);
			virtualLine.Add(new Domain.DisplayElement.LineBreak());

			return (virtualLine);
		}

		/// <summary></summary>
		protected virtual Domain.DisplayLine IOErrorToDisplayLine(DateTime ts, Domain.IOErrorEventArgs e, bool includeIOStatusText)
		{
			var sep   = SettingsRoot.Display.InfoSeparatorCache;
			var left  = SettingsRoot.Display.InfoEnclosureLeftCache;
			var right = SettingsRoot.Display.InfoEnclosureRightCache;

			var virtualLine = new Domain.DisplayLine(1 + 2 + 4); // Preset the required capacity to improve memory management.
			virtualLine.Add(new Domain.DisplayElement.LineStart());

			if (SettingsRoot.Display.ShowTimeStamp)
			{
				virtualLine.Add(new Domain.DisplayElement.TimeStampInfo(ts, SettingsRoot.Display.TimeStampFormat, SettingsRoot.Display.TimeStampUseUtc, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			if (includeIOStatusText)
			{
				virtualLine.Add(new Domain.DisplayElement.PortInfo(Domain.Direction.None, IOStatusText, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			virtualLine.Add(new Domain.DisplayElement.ErrorInfo((Domain.Direction)e.Direction, e.Message, (e.Severity == Domain.IOErrorSeverity.Acceptable)));
			virtualLine.Add(new Domain.DisplayElement.LineBreak());

			return (virtualLine);
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
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			// Log:
			if (this.log.AnyPortIsOn)
				this.log.WriteLine(IOStatusToDisplayLine(DateTime.Now), Log.LogChannel.Port); // Terminology is user = "port" and code = "IO".

			// Forward:
			OnIOChanged(e);

			// Attention, the 'IOChanged' event could trigger close = dispose of terminal!

			bool hasBeenConnected = this.terminal_IOChanged_hasBeenConnected;
			bool isConnectedNow = ((this.terminal != null) ? (this.terminal.IsConnected) : (false));

			if (!IsDisposed)
			{
				if      ( isConnectedNow && !hasBeenConnected)
				{
					var now = DateTime.Now; // Ensure that all use exactly the same instant.

					this.activeConnectChrono.Restart(now); // Restart, i.e. reset and start from zero.
					this.totalConnectChrono.Start(now);    // Start again, i.e. continue at last time.

					this.terminal.InitialTimeStamp = now;       // The initial time stamp is used for
					                                            // time spans. Consequently, the spans
					if (this.settingsRoot.Display.ShowTimeSpan) // will be based on the active connect
						this.terminal.RefreshRepositories();    // time, not the total connect time.
				}
				else if (!isConnectedNow &&  hasBeenConnected)
				{
					this.activeConnectChrono.Stop();
					this.totalConnectChrono.Stop();
				}
			}

			this.terminal_IOChanged_hasBeenConnected = isConnectedNow;
		}

		private void terminal_IOControlChanged(object sender, Domain.IOControlEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			// Log:
			if ((e.Texts != null) && (e.Texts.Count > 0))
			{
				if (this.log.AnyPortIsOn)                                     // Status text is always included (so far).
					this.log.WriteLine(IOControlToDisplayLine(e.TimeStamp, e, true), Log.LogChannel.Port); // Terminology is user = "port" and code = "IO".

				if (this.log.AnyNeatIsOn) // Workaround to bug #447 "Acceptable errors (e.g. <RX PARITY ERROR>) and I/O control events (e.g. <RTS=on>) are not contained in neat logs."
				{
					var dl = IOControlToDisplayLine(e.TimeStamp, e, false);
					this.log.WriteLine(dl, Log.LogChannel.NeatTx);
					this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
					this.log.WriteLine(dl, Log.LogChannel.NeatRx);
				}
			}

			// Forward:
			OnIOControlChanged(e);
		}

		private void terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			// Log:
			if (this.log.AnyPortIsOn)                                   // Status text is always included (so far).
				this.log.WriteLine(IOErrorToDisplayLine(e.TimeStamp, e, true), Log.LogChannel.Port); // Terminology is user = "port" and code = "IO".

			if (this.log.AnyNeatIsOn) // Workaround to bug #447 "Acceptable errors (e.g. <RX PARITY ERROR>) and I/O control events (e.g. <RTS=on>) are not contained in neat logs."
			{
				var dl = IOErrorToDisplayLine(e.TimeStamp, e, false);
				this.log.WriteLine(dl, Log.LogChannel.NeatTx);
				this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
				this.log.WriteLine(dl, Log.LogChannel.NeatRx);
			}

			// Forward:
			OnIOError(e);
		}

		/// <remarks>
		/// Interval can be quite long, because...
		/// ...first request will be done immediately.
		/// ...timed requests will be shown for 2 seconds.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly long TimedStatusTextRequestTickInterval = StopwatchEx.TimeToTicks(757); // = prime number around 750 milliseconds.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private long terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp; // = 0;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private long terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp; // = 0;

		/// <remarks>
		/// \remind (2017-08-27 / MKY) (bug #383 freeze while receiving a lot of fast data)
		/// In case of a lot of fast data, this event is raised very often. This handler itself
		/// raises up to three events, thus leading to a sequence of events. If these events need
		/// to be synchronized onto the main thread, this likely results in poor performance. This
		/// situation is anticipated in two ways:
		///  > The <see cref="TimedStatusTextRequest"/> will only be raised
		///    each <see cref="TimedStatusTextRequestTickInterval"/> milliseconds.
		///  > The <see cref="IOCountChanged_Promptly"/> and <see cref="IORateChanged_Promptly"/> events
		///    will not be used by the terminal form. Instead, the values will synchronously be retrieved
		///    when processing <see cref="DisplayElementsSent"/>, <see cref="DisplayElementsReceived"/>,
		///    <see cref="CurrentDisplayLinePartSentReplaced"/>, <see cref="CurrentDisplayLinePartReceivedReplaced"/>,
		///    <see cref="CurrentDisplayLineSentCleared"/>, <see cref="CurrentDisplayLineReceivedCleared"/>,
		///    <see cref="DisplayLinesSent"/> and <see cref="DisplayLinesReceived"/> events.
		///    In addition, the <see cref="IORateChanged_Decimated"/> event is used to get
		///    notified on updates after transmission.
		/// </remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RawChunkReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_RawChunkSent(object sender, Domain.RawChunkEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			var currentTickStamp = Stopwatch.GetTimestamp();
			if (currentTickStamp >= this.terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp)
			{
				OnTimedStatusTextRequest("Sending...");

				unchecked // Calculate tick stamp of next request:
				{
					this.terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp = (currentTickStamp + TimedStatusTextRequestTickInterval); // Loop-around is OK.
				}
			}

			// Count:
			this.txByteCount += e.Value.Content.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.txByteRate.Update(e.Value.Content.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Log:
			if (this.log.AnyRawIsOn)
			{
				this.log.Write(e.Value, Log.LogChannel.RawTx);
				this.log.Write(e.Value, Log.LogChannel.RawBidir);
			}
		}

		/// <remarks>
		/// \remind (2017-08-27 / MKY) (bug #383 freeze while receiving a lot of fast data)
		/// In case of a lot of fast data, this event is raised very often. This handler itself
		/// raises up to three events, thus leading to a sequence of events. If these events need
		/// to be synchronized onto the main thread, this likely results in poor performance. This
		/// situation is anticipated in two ways:
		///  > The <see cref="TimedStatusTextRequest"/> will only be raised
		///    each <see cref="TimedStatusTextRequestTickInterval"/> milliseconds.
		///  > The <see cref="IOCountChanged_Promptly"/> and <see cref="IORateChanged_Promptly"/> events
		///    will not be used by the terminal form. Instead, the values will synchronously be retrieved
		///    when processing <see cref="DisplayElementsSent"/>, <see cref="DisplayElementsReceived"/>,
		///    <see cref="CurrentDisplayLinePartSentReplaced"/>, <see cref="CurrentDisplayLinePartReceivedReplaced"/>,
		///    <see cref="CurrentDisplayLineSentCleared"/>, <see cref="CurrentDisplayLineReceivedCleared"/>,
		///    <see cref="DisplayLinesSent"/> and <see cref="DisplayLinesReceived"/> events.
		///    In addition, the <see cref="IORateChanged_Decimated"/> event is used to get
		///    notified on updates after transmission.
		/// </remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RawChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_RawChunkReceived(object sender, Domain.RawChunkEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			var currentTickStamp = Stopwatch.GetTimestamp();
			if (currentTickStamp >= this.terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp)
			{
				OnTimedStatusTextRequest("Receiving...");

				unchecked // Calculate tick stamp of next request:
				{
					this.terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp = (currentTickStamp + TimedStatusTextRequestTickInterval); // Loop-around is OK.
				}
			}

			// Rate:
			this.rxByteCount += e.Value.Content.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.rxByteRate.Update(e.Value.Content.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Log:
			if (this.log.AnyRawIsOn)
			{
				this.log.Write(e.Value, Log.LogChannel.RawBidir);
				this.log.Write(e.Value, Log.LogChannel.RawRx);
			}

			// AutoResponse (by specification only active on receive-path):
			if (this.settingsRoot.AutoResponse.IsActive)
			{
				bool isTriggered = false;

				foreach (byte b in e.Value.Content)
				{
					lock (this.autoResponseTriggerHelperSyncObj)
					{
						if (this.autoResponseTriggerHelper != null)
						{
							if (this.autoResponseTriggerHelper.EnqueueAndMatchTrigger(b))
								isTriggered = true;
						}
						else
						{
							break; // Break the for-loop if [AutoResponse] got disposed in the meantime.
						}          // Though unlikely, it may happen when deactivating [AutoResponse]
					}              // while receiving a very large chunk.
				}

				if (isTriggered)
				{
					// Invoke sending on different thread than the receive thread:
					byte[] triggerSequence = null;

					lock (this.autoResponseTriggerHelperSyncObj)
					{
						if (this.autoResponseTriggerHelper != null)
							triggerSequence = this.autoResponseTriggerHelper.TriggerSequence;
					}

					var asyncInvoker = new Action<byte[]>(terminal_RawChunkReceived_SendAutoResponseAsync);
					asyncInvoker.BeginInvoke(triggerSequence, null, null);

					// Highlighting is done for all auto responses (so far):
					e.Attribute = Domain.LineChunkAttribute.Highlight;
				}
			}

			// AutoAction (by specification only active on receive-path):
			if (this.settingsRoot.AutoAction.IsActive)
			{
				if (this.autoActionClearRepositoriesOnSubsequentRxIsArmed)
				{
					this.autoActionClearRepositoriesOnSubsequentRxIsArmed = false;

					byte[] triggerSequence = null;

					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
							triggerSequence = this.autoActionTriggerHelper.TriggerSequence;
					}

					var asyncInvoker = new Action<AutoAction, byte[], DateTime>(terminal_RawChunkReceived_InvokeAutoActionAsync);
					asyncInvoker.BeginInvoke(AutoAction.ClearRepositories, triggerSequence, e.Value.TimeStamp, null, null);
				}                                    // ClearRepositories is to be invoked, not ClearRepositoriesOnSubsequentRx!

				bool isTriggered = false;

				foreach (byte b in e.Value.Content)
				{
					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
						{
							if (this.autoActionTriggerHelper.EnqueueAndMatchTrigger(b))
								isTriggered = true;
						}
						else
						{
							break; // Break the for-loop if [AutoAction] got disposed in the meantime.
						}          // Though unlikely, it may happen when deactivating [AutoAction]
					}              // while receiving a very large chunk.
				}

				if (isTriggered)
				{
					// Invoke sending on different thread than the receive thread:
					byte[] triggerSequence = null;

					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
							triggerSequence = this.autoActionTriggerHelper.TriggerSequence;
					}

					var asyncInvoker = new Action<AutoAction, byte[], DateTime>(terminal_RawChunkReceived_InvokeAutoActionAsync);
					asyncInvoker.BeginInvoke(this.settingsRoot.AutoAction.Action, triggerSequence, e.Value.TimeStamp, null, null);

					// Mark the received chunk as needed (triggered):
					switch ((AutoAction)this.settingsRoot.AutoAction.Action)
					{
						case AutoAction.Filter:   e.Attribute = Domain.LineChunkAttribute.Filter;           break;
						case AutoAction.Suppress: e.Attribute = Domain.LineChunkAttribute.SuppressForSure;  break;
						default:                  e.Attribute = Domain.LineChunkAttribute.Highlight;        break;
					}
				}
				else
				{
					// Mark the received chunk as needed (non-triggered):
					switch ((AutoAction)this.settingsRoot.AutoAction.Action)
					{
						case AutoAction.Filter: e.Attribute = Domain.LineChunkAttribute.PotentiallySuppress; break;
					}
				}
			}
		}

		private void terminal_RawChunkReceived_SendAutoResponseAsync(byte[] triggerSequence)
		{
			SendAutoResponse(triggerSequence);
		}

		private void terminal_RawChunkReceived_InvokeAutoActionAsync(AutoAction action, byte[] triggerSequence, DateTime ts)
		{
			InvokeAutoAction(action, triggerSequence, ts);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnDisplayElementsSent(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnDisplayElementsReceived(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLinePartReceivedReplaced", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_CurrentDisplayLinePartSentReplaced(object sender, Domain.DisplayLinePartEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnCurrentDisplayLinePartSentReplaced(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLinePartSentReplaced", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_CurrentDisplayLinePartReceivedReplaced(object sender, Domain.DisplayLinePartEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnCurrentDisplayLinePartReceivedReplaced(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineReceivedCleared", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_CurrentDisplayLineSentCleared(object sender, EventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnCurrentDisplayLineSentCleared(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineSentCleared", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_CurrentDisplayLineReceivedCleared(object sender, EventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnCurrentDisplayLineReceivedCleared(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			// Count:
			this.txLineCount += e.Lines.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.txLineRate.Update(e.Lines.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Display:
			OnDisplayLinesSent(e);

			// Log:
			if (this.log.AnyNeatIsOn)
			{
				foreach (var dl in e.Lines)
				{
					this.log.WriteLine(dl, Log.LogChannel.NeatTx);
					this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
				}
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			// Count:
			this.rxLineCount += e.Lines.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.rxLineRate.Update(e.Lines.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Display:
			OnDisplayLinesReceived(e);

			// Log:
			if (this.log.AnyNeatIsOn)
			{
				foreach (var dl in e.Lines)
				{
					this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
					this.log.WriteLine(dl, Log.LogChannel.NeatRx);
				}
			}

			// AutoResponse:
			if (this.settingsRoot.AutoResponse.IsActive && (this.settingsRoot.AutoResponse.Trigger == AutoTrigger.AnyLine))
			{
				foreach (var dl in e.Lines)
				{
					SendAutoResponse(LineWithoutRxEolToOrigin(dl));
				}

				// Note that trigger line is not highlighted if [Trigger == AnyLine] since that
				// would result in all received lines highligted.
				//
				// Also note that implementation wouldn't be that simple, since "e.Highlight = true"
				// doesn't help in this 'LinesReceived' event, as the monitors already get updated
				// in the 'ElementsReceived' event above.
			}

			// AutoAction:
			if (this.settingsRoot.AutoAction.IsActive && (this.settingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine))
			{
				foreach (var dl in e.Lines)
				{
					InvokeAutoAction(this.settingsRoot.AutoAction.Action, LineWithoutRxEolToOrigin(dl), dl.TimeStamp);
				}

				// Note that trigger line is not highlighted if [Trigger == AnyLine] since that
				// would result in all received lines highligted.
				//
				// Also note that implementation wouldn't be that simple, since "e.Highlight = true"
				// doesn't help in this 'LinesReceived' event, as the monitors already get updated
				// in the 'ElementsReceived' event above.
			}
		}

		private byte[] LineWithoutRxEolToOrigin(Domain.DisplayLine dl)
		{
			var l = new List<byte>(dl.ElementsToOrigin());

			if (this.settingsRoot.TerminalType == Domain.TerminalType.Text)
			{
				var textTerminal = (this.terminal as Domain.TextTerminal);

				// Remove Rx EOL:
				if (this.settingsRoot.TextTerminal.ShowEol)
				{
					var rxEolSequence = textTerminal.RxEolSequence;
					l.RemoveRange((l.Count - rxEolSequence.Length), rxEolSequence.Length);
				}
			}

			return (l.ToArray());
		}

		private void terminal_RepositoryCleared(object sender, EventArgs<Domain.RepositoryType> e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnRepositoryCleared(e);
		}

		private void terminal_RepositoryReloaded(object sender, EventArgs<Domain.RepositoryType> e)
		{
			if (IsDisposed) // Ensure not to handle events during closing anymore.
				return;

			OnRepositoryReloaded(e);
		}

		#endregion

		#region Terminal > Check I/O
		//------------------------------------------------------------------------------------------
		// Terminal > Check I/O
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks the terminal's I/O port availability. If I/O port is not available, and settings
		/// allow so, user is asked whether to change to a different I/O port.
		/// </summary>
		/// <remarks>
		/// Note that only the availability of the I/O port is checked, not whether the port is
		/// already in use, because that may take quite some time and thus unnecessarily delay the
		/// open/check/start sequence.
		/// </remarks>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public virtual CheckResult CheckIOAvailability()
		{
			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
				{
					var portId = this.settingsRoot.Terminal.IO.SerialPort.PortId;
					if (portId != null)
						return (CheckSerialPortAvailability(portId));
					else
						return (CheckResult.Ignore);
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				{
					MKY.Net.IPNetworkInterfaceEx localInterface = this.settingsRoot.Terminal.IO.Socket.LocalInterface;
					if (localInterface != null)
						return (CheckLocalInterfaceAvailability(localInterface));
					else
						return (CheckResult.Ignore);
				}

				case Domain.IOType.UsbSerialHid:
				{
					var deviceInfo = this.settingsRoot.Terminal.IO.UsbSerialHidDevice.DeviceInfo;
					if (deviceInfo != null)
						return (CheckUsbDeviceAvailability(deviceInfo));
					else
						return (CheckResult.Ignore);
				}

				default: // Not (yet) useful for UDP/IP sockets.
				{
					return (CheckResult.OK); // All 'non-handled' cases.
				}
			}
		}

		private CheckResult CheckSerialPortAvailability(MKY.IO.Ports.SerialPortId portId)
		{
			OnFixedStatusTextRequest("Checking availability of " + portId +  "...");

			var ports = new MKY.IO.Ports.SerialPortCollection();
			ports.FillWithAvailablePorts(false); // Explicitly not getting captions, thus faster.

			// Attention:
			// Similar code exists in View.Controls.SerialPortSelection.SetPortList().
			// Changes here may have to be applied there too!

			if (ports.Count > 0)
			{
				if (ports.Contains(portId))
				{
					return (CheckResult.OK);
				}
				else
				{
					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateSerialPort)
					{
						MKY.IO.Ports.SerialPortId portIdAlternate;
						if (TryGetSerialPortAlternate(ports, out portIdAlternate))
						{
							var dr = ShowSerialPortNotAvailableSwitchQuestionYesNo(portId, portIdAlternate);
							if (dr == DialogResult.Yes)
							{
								this.settingsRoot.Explicit.Terminal.IO.SerialPort.PortId = portIdAlternate;
								ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR#309.
							}

							return (CheckResult.OK); // Device may not yet be available but 'AutoOpen'.
						}
						else
						{
							OnTimedStatusTextRequest(portId + " currently not available");
							return (CheckResult.Cancel);
						}
					}
					else
					{
						OnResetStatusTextRequest();
						return (CheckResult.Ignore); // Silently ignore.
					}
				}
			}
			else // ports.Count == 0
			{
				if (ApplicationSettings.LocalUserSettings.General.AskForAlternateSerialPort)
				{
					var dr = ShowNoSerialPortsStartAnywayQuestionYesNo(portId);
					if (dr == DialogResult.Yes)
					{
						return (CheckResult.OK);
					}
					else
					{
						OnTimedStatusTextRequest("No serial COM ports available");
						return (CheckResult.Cancel);
					}
				}
				else
				{
					OnResetStatusTextRequest();
					return (CheckResult.Ignore); // Silently ignore.
				}
			}
		}

		private CheckResult CheckLocalInterfaceAvailability(MKY.Net.IPNetworkInterfaceEx localInterface)
		{
			OnFixedStatusTextRequest("Checking availability of '" + localInterface + "'...");

			var localInterfaces = new MKY.Net.IPNetworkInterfaceCollection();
			localInterfaces.FillWithAvailableLocalInterfaces();

			// Attention:
			// Similar code exists in View.Controls.SocketSelection.SetLocalInterfaceList().
			// Changes here may have to be applied there too!

			if (localInterfaces.Count > 0)
			{
				if (localInterfaces.Contains(localInterface))
				{
					return (CheckResult.OK);
				}
				else if (localInterfaces.ContainsDescription(localInterface))
				{
					// A device with same description is available, use that:
					int sameDescriptionIndex = localInterfaces.FindIndexDescription(localInterface);

					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateNetworkInterface)
					{
						var dr = ShowLocalInterfaceNotAvailableAlternateQuestionYesNo(localInterface, localInterfaces[sameDescriptionIndex]);
						if (dr == DialogResult.Yes)
						{
							this.settingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[sameDescriptionIndex];
							ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR#309.
							return (CheckResult.OK);
						}
						else
						{
							OnResetStatusTextRequest();
							return (CheckResult.Cancel);
						}
					}
					else // Silently switch interface:
					{
						this.settingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[sameDescriptionIndex];
						ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR#309.
						return (CheckResult.OK);
					}
				}
				else
				{
					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateNetworkInterface)
					{
						var dr = ShowLocalInterfaceNotAvailableDefaultQuestionYesNo(localInterface, localInterfaces[0]);
						if (dr == DialogResult.Yes)
						{
							this.settingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[0];
							ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR#309.
							return (CheckResult.OK);
						}
						else
						{
							OnTimedStatusTextRequest("'" + localInterface + "' currently not available");
							return (CheckResult.Cancel);
						}
					}
					else
					{
						OnResetStatusTextRequest();
						return (CheckResult.Ignore); // Silently ignore.
					}
				}
			}
			else // localInterfaces.Count == 0
			{
				if (ApplicationSettings.LocalUserSettings.General.AskForAlternateNetworkInterface)
				{
					ShowNoLocalInterfacesMessageOK();
					OnTimedStatusTextRequest("No local network interfaces available");
					return (CheckResult.Cancel);
				}
				else
				{
					OnResetStatusTextRequest();
					return (CheckResult.Ignore); // Silently ignore.
				}
			}
		}

		private CheckResult CheckUsbDeviceAvailability(MKY.IO.Usb.DeviceInfo deviceInfo)
		{
			OnFixedStatusTextRequest("Checking availability of '" + deviceInfo + "'...");

			var devices = new MKY.IO.Usb.SerialHidDeviceCollection();
			devices.FillWithAvailableDevices(true); // Retrieve strings from devices in order to get serial strings.

			// Attention:
			// Similar code exists in View.Controls.UsbSerialHidDeviceSelection.SetDeviceList().
			// Changes here may have to be applied there too!

			if (devices.Count > 0)
			{
				if (devices.Contains(deviceInfo))
				{
					return (CheckResult.OK);
				}
				else if (devices.ContainsVidPid(deviceInfo))
				{
					// A device with same VID/PID is available, use that:
					int sameVidPidIndex = devices.FindIndexVidPid(deviceInfo);

					// Inform the user if serial is required:
					if (ApplicationSettings.LocalUserSettings.General.MatchUsbSerial)
					{
						if (ApplicationSettings.LocalUserSettings.General.AskForAlternateUsbDevice)
						{
							var dr = ShowUsbSerialHidDeviceNotAvailableAlternateQuestionYesNo(deviceInfo, devices[sameVidPidIndex]);
							if (dr == DialogResult.Yes)
							{
								this.settingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];
								ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR#309.
							}

							return (CheckResult.OK); // Device may not yet be available but 'AutoOpen'.
						}
						else
						{
							OnResetStatusTextRequest();
							return (CheckResult.Ignore); // Silently ignore.
						}
					}
					else
					{
						// Clear the 'Changed' flag in case of automatically changing settings:
						bool hadAlreadyBeenChanged = this.settingsRoot.Terminal.IO.UsbSerialHidDevice.HaveChanged;
						this.settingsRoot.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];
						if (!hadAlreadyBeenChanged)
							this.settingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.ClearChanged();

						ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR#309.

						return (CheckResult.OK); // Device may not yet be available but 'AutoOpen'.
					}
				}
				else
				{
					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateUsbDevice)
					{
						var dr = ShowUsbSerialHidDeviceNotAvailableStartAnywayQuestionYesNo(deviceInfo);
						if (dr == DialogResult.Yes)
						{
							return (CheckResult.OK);
						}
						else
						{
							OnTimedStatusTextRequest("'" + deviceInfo + "' currently not available");
							return (CheckResult.Cancel);
						}
					}
					else
					{
						OnResetStatusTextRequest();
						return (CheckResult.Ignore); // Silently ignore.
					}
				}
			}
			else // devices.Count == 0
			{
				if (ApplicationSettings.LocalUserSettings.General.AskForAlternateUsbDevice)
				{
					var dr = ShowNoUsbSerialHidDevicesStartAnywayQuestionYesNo(deviceInfo);
					if (dr == DialogResult.Yes)
					{
						return (CheckResult.OK);
					}
					else
					{
						OnTimedStatusTextRequest("No HID capable USB devices available");
						return (CheckResult.Cancel);
					}
				}
				else
				{
					OnResetStatusTextRequest();
					return (CheckResult.Ignore); // Silently ignore.
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private bool TryGetSerialPortAlternate(MKY.IO.Ports.SerialPortCollection ports, out MKY.IO.Ports.SerialPortId portIdAlternate)
		{
			// If not allowed to detect 'InUse', no reliable alternate can be evaluted:
			if (!ApplicationSettings.LocalUserSettings.General.DetectSerialPortsInUse)
			{
				portIdAlternate = null;
				return (false);
			}

			// If allowed, try to retrieve captions:
			if (ApplicationSettings.LocalUserSettings.General.RetrieveSerialPortCaptions)
			{
				// Done once for all ports because:
				//  > Underlying operation is relatively fast.
				//  > Underlying operation needs to retrieve *all* captions anyway.

				try
				{
					ports.RetrieveCaptions();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex, "Failed to retrieve serial COM port captions!");
				}
			}

			// Select the first available port that is not 'InUse':
			foreach (var port in ports)
			{
				ports.DetectWhetherPortIsInUse(port);

				if (!port.IsInUse)
				{
					portIdAlternate = port;
					return (true);
				}
			}

			// No alternate that is not 'InUse':
			portIdAlternate = null;
			return (false);
		}

		private DialogResult ShowNoSerialPortsStartAnywayQuestionYesNo(string portIdNotAvailable)
		{
			string message =
				"There are currently no serial COM ports available." + Environment.NewLine + Environment.NewLine +
				"Start " + portIdNotAvailable + " anyway?";

			var dr = OnMessageInputRequest
			(
				message,
				"No serial COM ports available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowSerialPortNotAvailableSwitchQuestionYesNo(string portIdNotAvailable, string portIdAlternate)
		{
			string message =
				"The previous serial port " + portIdNotAvailable + " is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to " + portIdAlternate + " (first available port that is currently not in use) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch serial COM port?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private void ShowNoLocalInterfacesMessageOK()
		{
			string message =
				"There are currently no local network interfaces available." + Environment.NewLine + Environment.NewLine +
				"Terminal will not be started.";

			OnMessageInputRequest
			(
				message,
				"No interfaces available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
			);
		}

		private DialogResult ShowLocalInterfaceNotAvailableDefaultQuestionYesNo(string localInterfaceNotAvailable, string localInterfaceDefaulted)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + localInterfaceDefaulted + "' (default) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch interface?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private DialogResult ShowLocalInterfaceNotAvailableAlternateQuestionYesNo(string localInterfaceNotAvailable, string localInterfaceAlternate)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + localInterfaceAlternate + "' (first available interface with same description) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch interface?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private DialogResult ShowNoUsbSerialHidDevicesStartAnywayQuestionYesNo(string deviceInfoNotAvailable)
		{
			string message =
				"There are currently no HID capable USB devices available." + Environment.NewLine + Environment.NewLine +
				"Start " + deviceInfoNotAvailable + " anyway?";

			var dr = OnMessageInputRequest
			(
				message,
				"No USB HID devices available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceNotAvailableStartAnywayQuestionYesNo(string deviceInfoNotAvailable)
		{
			string message =
				"The previous USB HID device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Start anyway?";

			var dr = OnMessageInputRequest
			(
				message,
				"Previous USB HID device not available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceNotAvailableAlternateQuestionYesNo(string deviceInfoNotAvailable, string deviceInfoAlternate)
		{
			string message =
				"The previous device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + deviceInfoAlternate + "' (first available device with same VID and PID) instead?";

			var dr = OnMessageInputRequest
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
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public virtual bool StartIO()
		{
			return (StartIO(true));
		}

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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

					if (ApplicationSettings.LocalUserSettings.General.NotifyNonAvailableIO)
					{
						string yatLead, yatText;
						ErrorHelper.MakeStartHint(this.settingsRoot.IOType, out yatLead, out yatText);

						OnMessageInputRequest
						(
							ErrorHelper.ComposeMessage("Terminal could not be started!", string.Empty, yatLead, yatText),
							"Terminal Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
			}
			catch (Exception ex)
			{
				OnFixedStatusTextRequest("Error starting terminal!");

				if (ApplicationSettings.LocalUserSettings.General.NotifyNonAvailableIO)
				{
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
			}

			return (success);
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public virtual bool StopIO()
		{
			return (StopIO(true));
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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

		#region Terminal > Send Command
		//------------------------------------------------------------------------------------------
		// Terminal > Send Command
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends given command.
		/// </summary>
		public virtual void SendCommand(Command command)
		{
			// AssertNotDisposed() is called by 'Send...' below.

			if (command.IsText)
				SendText(command);
			else if (command.IsFilePath)
				SendFile(command);
			else
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + command + "' does not specify a known command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "command"));
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
			// AssertNotDisposed() is called by 'Send...' below.

			SendText(this.settingsRoot.SendText.Command);

			// Clear command if desired:
			if (!this.settingsRoot.Send.Text.KeepSendText)
				this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
		}

		/// <summary>
		/// Sends text command given by terminal settings.
		/// </summary>
		public virtual void SendTextWithoutEol()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendTextWithoutEol(this.settingsRoot.SendText.Command);

			// Clear command if desired:
			if (!this.settingsRoot.Send.Text.KeepSendText)
				this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
		}

		/// <summary>
		/// Sends partial text EOL.
		/// </summary>
		public virtual void SendPartialTextEol()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendText(new Command(true, this.settingsRoot.SendText.Command.DefaultRadix));
		}

		/// <summary>
		/// Sends given text.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		public virtual void SendText(string text)
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendText(new Command(text));
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="command">Text command to be sent.</param>
		public virtual void SendText(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
				DoSendText(command);
			else
				DoSendText(command.ToCommandWithoutDefaultRadix());
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="command">Text command to be sent.</param>
		public virtual void SendTextWithoutEol(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
				DoSendTextWithoutEol(command);
			else
				DoSendTextWithoutEol(command.ToCommandWithoutDefaultRadix());
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void DoSendText(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidText)
			{
				if (c.IsSingleLineText)
				{
					if (SendTextSettings.IsEasterEggCommand(c.SingleLineText))
						this.terminal.EnqueueEasterEggMessage();
					else
						this.terminal.SendTextLine(c.SingleLineText, c.DefaultRadix);
				}
				else if (c.IsMultiLineText)
				{
					this.terminal.SendTextLines(c.MultiLineText, c.DefaultRadix);
				}
				else if (c.IsPartialText)
				{
					this.terminal.SendText(c.PartialText, c.DefaultRadix);

					// Compile the partial command line for later use:
					if (string.IsNullOrEmpty(this.partialCommandLine))
						this.partialCommandLine = string.Copy(c.PartialText);
					else
						this.partialCommandLine += c.PartialText;
				}
				else if (c.IsPartialTextEol)
				{
					this.terminal.SendTextLine("", Domain.Radix.String);
				}
				else
				{
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known text command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
				}

				CloneIntoRecentTextCommandsIfNeeded(c);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid text command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
			}
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void DoSendTextWithoutEol(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidText)
			{
				if (c.IsSingleLineText)
				{
					if (SendTextSettings.IsEasterEggCommand(c.SingleLineText))
						this.terminal.EnqueueEasterEggMessage();
					else
						this.terminal.SendText(c.SingleLineText, c.DefaultRadix);
				}
				else if (c.IsPartialText)
				{
					this.terminal.SendText(c.PartialText, c.DefaultRadix);

					// Compile the partial command line for later use:
					if (string.IsNullOrEmpty(this.partialCommandLine))
						this.partialCommandLine = string.Copy(c.PartialText);
					else
						this.partialCommandLine += c.PartialText;
				}
				else // Covers 'c.IsMultiLineText' and 'c.IsPartialTextEol' which are invalid 'WithoutEol'.
				{
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known text command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
				}

				CloneIntoRecentTextCommandsIfNeeded(c);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid text command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
			}
		}

		/// <remarks>
		/// Includes compiled partial text.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void CloneIntoRecentTextCommandsIfNeeded(Command c)
		{
			if (c.IsSingleLineText || c.IsMultiLineText /* || do not add c.IsPartialText to recents */ || c.IsPartialTextEol)
			{
				// Clone the command for the recent commands collection:
				Command clone;
				if (c.IsSingleLineText || c.IsMultiLineText)
				{
					clone = new Command(c); // 'Normal' case, simply clone the command to ensure decoupling.
				}
				else if (c.IsPartialTextEol)
				{                                        // Create a single line text command,
					if (this.partialCommandLine != null) // using the previously sent characters:
						clone = new Command(this.partialCommandLine, false, c.DefaultRadix);
					else
						clone = new Command(c); // Keep partial EOL.
				}
				else
				{
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Condition is invalid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				// Reset the partial command line, in any case:
				this.partialCommandLine = null;

				// Clear potential description, as that shall not be considered for recents,
				// e.g. same command with different description shall only be listed once:
				clone.ClearDescription();

				// Put clone into recent history:
				this.settingsRoot.SendText.RecentCommands.Add(new RecentItem<Command>(clone));
				this.settingsRoot.SendText.SetChanged(); // Manual change required because underlying collection is modified.
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
			// AssertNotDisposed() is called by 'Send...' below.

			SendFile(this.settingsRoot.SendFile.Command);
		}

		/// <summary>
		/// Sends given file.
		/// </summary>
		/// <param name="command">File to be sent.</param>
		public virtual void SendFile(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
				DoSendFile(command);
			else
				DoSendFile(command.ToCommandWithoutDefaultRadix());
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'symmetricity' is a correct English term.")]
		protected virtual void DoSendFile(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
			{
				string absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(c.FilePath, Path.GetDirectoryName(SettingsFilePath));

				this.terminal.SendFile(absoluteFilePath, c.DefaultRadix);

				CloneIntoRecentFileCommands(c);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid file command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
			}
		}

		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void CloneIntoRecentFileCommands(Command c)
		{
			// Clone the command for the recent commands collection:
			var clone = new Command(c);

			// Clear potential description, as that shall not be considered for recents,
			// e.g. same command with different description shall only be listed once:
			clone.ClearDescription();

			// Put clone into recent history:
			this.settingsRoot.SendFile.RecentCommands.Add(new RecentItem<Command>(clone));
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
			AssertNotDisposed();

			// Verify arguments:
			if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(page - 1, command - 1))
				return (false);

			// Process command:
			var c = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];
			if (c.IsValidText)
			{
				SendText(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendText.Command = new Command(c); // Clone command to ensure decoupling.

				return (true);
			}
			else if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
			{
				SendFile(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendFile.Command = new Command(c); // Clone command to ensure decoupling.

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
			AssertNotDisposed();

			// Verify arguments:
			if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(page - 1, command - 1))
				return (false);

			// Process command:
			var c = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];
			if (c.IsValidText)
			{
				this.settingsRoot.SendText.Command = new Command(c); // Clone command to ensure decoupling.
				return (true);
			}
			else if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
			{
				this.settingsRoot.SendFile.Command = new Command(c); // Clone command to ensure decoupling.
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
			AssertNotDisposed();

			this.terminal.Break();
		}

		/// <summary>
		/// Resumes all currently suspended operations in the terminal.
		/// </summary>
		public virtual void ResumeBreak()
		{
			AssertNotDisposed();

			this.terminal.ResumeBreak();
		}

		#endregion

		#region Terminal > Repositories
		//------------------------------------------------------------------------------------------
		// Terminal > Repositories
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Clears given repository.
		/// </summary>
		public virtual void ClearRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			if (!this.terminal.ClearRepository(repositoryType))
				OnTimedStatusTextRequest("Clear request has timed out");
		}

		/// <summary>
		/// Clears all repositories.
		/// </summary>
		public virtual void ClearRepositories()
		{
			AssertNotDisposed();

			if (!this.terminal.ClearRepositories())
				OnTimedStatusTextRequest("Clear request has timed out");
		}

		/// <summary>
		/// Forces complete refresh of repositories.
		/// </summary>
		public virtual void RefreshRepositories()
		{
			AssertNotDisposed();

			if (!this.terminal.RefreshRepositories())
				OnTimedStatusTextRequest("Refresh request has timed out");
		}

		/// <summary>
		/// Forces complete refresh of given repository.
		/// </summary>
		public virtual void RefreshRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			if (!this.terminal.RefreshRepository(repositoryType))
				OnTimedStatusTextRequest("Refresh request has timed out");
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual Domain.DisplayElementCollection RepositoryToDisplayElements(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.RepositoryToDisplayElements(repositoryType));
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual Domain.DisplayLineCollection RepositoryToDisplayLines(Domain.RepositoryType repositoryType)
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
		public virtual string RepositoryToDiagnosticsString(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.RepositoryToDiagnosticsString(repositoryType));
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
		////this.activeConnectChrono.TimeSpanChanged shall not be used, events are invoked by 'totalConnectChrono' below.

			this.totalConnectChrono = new Chronometer();
			this.totalConnectChrono.Interval = 1000;
			this.totalConnectChrono.TimeSpanChanged += totalConnectChrono_TimeSpanChanged;
		}

		private void DisposeChronos()
		{
			if (this.activeConnectChrono != null)
			{
				// No elapsed event, events are invoked by total connect chrono.
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

			var now = DateTime.Now; // Ensure that all use exactly the same instant.

			this.activeConnectChrono.Reset(now);
			this.totalConnectChrono .Reset(now);

			this.terminal.InitialTimeStamp = now;

			if (this.settingsRoot.Display.ShowTimeSpan)
				this.terminal.RefreshRepositories();
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

			OnIOCountChanged_Promptly(EventArgs.Empty);

			this.txByteRate.Reset();
			this.txLineRate.Reset();
			this.rxByteRate.Reset();
			this.rxLineRate.Reset();

			OnIORateChanged_Promptly(EventArgs.Empty);
			OnIORateChanged_Decimated(EventArgs.Empty);
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
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnIORateChanged_Promptly(e);
			OnIORateChanged_Decimated(e);
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
		public virtual int InputBreakCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.InputBreakCount);
			}
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
		/// Toggles RTS control pin if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		public virtual bool RequestToggleRts()
		{
			AssertNotDisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleRts(out pinState);
			this.SettingsRoot.IO.SerialPort.Communication.RtsPin = pinState;
			return (isSuccess);

			// Note, this user requested change of the current settings is handled here,
			// and not in the 'terminal_IOControlChanged' event handler, for two reasons:
			//  1. The event will be raised after any change of the control pins, separating
			//     user indended and other events would be cumbersome.
			//  2. The event will be raised asynchronously, accessing the settings here
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		public virtual bool RequestToggleDtr()
		{
			AssertNotDisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleDtr(out pinState);
			this.SettingsRoot.IO.SerialPort.Communication.DtrPin = pinState;
			return (isSuccess);

			// Note, this user requested change of the current settings is handled here,
			// and not in the 'terminal_IOControlChanged' event handler, for two reasons:
			//  1. The event will be raised after any change of the control pins, separating
			//     user indended and other events would be cumbersome.
			//  2. The event will be raised asynchronously, accessing the settings here
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool SwitchLogOn()
		{
			try
			{
				this.log.SwitchOn();
				this.settingsRoot.LogIsOn = true;

				return (true);
			}
			catch (Exception ex)
			{
				string yatLead, yatText;
				ErrorHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to switch log on.", ex, yatLead, yatText),
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool ClearLog()
		{
			try
			{
				this.log.Clear();

				return (true);
			}
			catch (Exception ex)
			{
				string yatLead, yatText;
				ErrorHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to clear log.", ex, yatLead, yatText),
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool SwitchLogOff()
		{
			try
			{
				this.log.SwitchOff();
				this.settingsRoot.LogIsOn = false;

				return (true);
			}
			catch (Exception ex)
			{
				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to clear log.", ex),
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
					if (this.log.AnyIsOn && ExtensionHelper.IsFileTypeThatCanOnlyBeOpenedWhenCompleted(filePath))
					{
						string message =
							@"Log is still on and """ + Path.GetFileName(filePath) + @""" is incomplete." +
							Environment.NewLine + Environment.NewLine +
							"Shall the log be switched off before opening the file? [recommended]";

						var dr = OnMessageInputRequest
						(
							message,
							"Log File Warning",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Warning
						);

						if (dr == DialogResult.Yes)
						{
							SwitchLogOff();
						}

						// dr == DialogResult.No => Open the incomplete file anyway.

						if (dr == DialogResult.Cancel)
						{
							success = false;
							break; // Cancel all files.
						}
					}

					Exception ex;
					if (!Editor.TryOpenFile(filePath, out ex))
					{
						var dr = OnMessageInputRequest
						(
							ErrorHelper.ComposeMessage("Unable to open log file", filePath, ex),
							"Log File Error",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Error
						);

						if (dr == DialogResult.Cancel)
						{
							success = false;
							break; // Cancel all files.
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

				// Create directory if not existing yet:
				if (!Directory.Exists(Path.GetDirectoryName(rootPath)))
				{
					try
					{
						Directory.CreateDirectory(Path.GetDirectoryName(rootPath));
					}
					catch (Exception exCreate)
					{
						string message = "Unable to create folder." + Environment.NewLine + Environment.NewLine +
						                 "System error message:" + Environment.NewLine + exCreate.Message;

						OnMessageInputRequest
						(
							message,
							"Folder Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);

						return (false);
					}
				}

				// Open directory:
				Exception exBrowse;
				if (!DirectoryEx.TryBrowse(rootPath, out exBrowse))
				{
					OnMessageInputRequest
					(
						ErrorHelper.ComposeMessage("Unable to open log folder", rootPath, exBrowse),
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

		#region Auto
		//==========================================================================================
		// Auto
		//==========================================================================================

		// Note that AutoResponse/Action is intentionally implemented in 'Model' instead of 'Domain'
		// for the following reasons:
		//  > Triggers relate to 'Model' items SendText/SendFile/PredefinedCommand/....
		//  > Responses relate to 'Model' items SendText/SendFile/PredefinedCommand/....
		//  > Actions relate to 'Model' items as well.
		//
		// If needed one day, trigger evaluation could be moved to 'Domain', same as EOL evaluation.
		// Moving this to 'Domain' would e.g. allow for coloring. However, this would require two or
		// even four more colors (Tx/Rx data/control highlight). This becomes too complicated...

		#region Auto > Response
		//------------------------------------------------------------------------------------------
		// Auto > Response
		//------------------------------------------------------------------------------------------

		private void CreateAutoResponseHelper()
		{
			UpdateAutoResponse(); // Simply forward to general Update() method.
		}

		private void DisposeAutoResponseHelper()
		{
			lock (this.autoResponseTriggerHelperSyncObj)
				this.autoResponseTriggerHelper = null; // Simply delete the reference to the object.
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
					byte[] triggerSequence;
					if (TryParseCommandToSequence(this.settingsRoot.ActiveAutoResponseTrigger, out triggerSequence))
					{
						lock (this.autoResponseTriggerHelperSyncObj)
						{
							if (this.autoResponseTriggerHelper == null)
								this.autoResponseTriggerHelper = new AutoTriggerHelper(triggerSequence);
							else
								this.autoResponseTriggerHelper.TriggerSequence = triggerSequence;
						}
					}
					else
					{
						DeactivateAutoResponse();
						DisposeAutoResponseHelper();

						OnMessageInputRequest
						(
							"Failed to parse the automatic response trigger! Automatic response has been disabled!" + Environment.NewLine + Environment.NewLine +
							"To enable again, re-configure the automatic response.",
							"Automatic Response Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
				else // No command required = no sequence required = no helper required.
				{
					DisposeAutoResponseHelper();
				}
			}
			else // Disabled.
			{
				DisposeAutoResponseHelper();
			}
		}

		/// <summary>
		/// Sends the automatic response.
		/// </summary>
		protected virtual void SendAutoResponse(byte[] triggerSequence)
		{
			this.autoResponseCount++; // Incrementing before sending to have the effective count available during sending.
			OnAutoResponseCountChanged(new EventArgs<int>(this.autoResponseCount));

			int page = this.settingsRoot.Predefined.SelectedPage;
			switch ((AutoResponse)this.settingsRoot.AutoResponse.Response)
			{
				case AutoResponse.Trigger:             SendAutoResponseTrigger(triggerSequence); break;
				case AutoResponse.SendText:            SendText();                               break;
				case AutoResponse.SendFile:            SendFile();                               break;
				case AutoResponse.PredefinedCommand1:  SendPredefined(page, 1);                  break;
				case AutoResponse.PredefinedCommand2:  SendPredefined(page, 2);                  break;
				case AutoResponse.PredefinedCommand3:  SendPredefined(page, 3);                  break;
				case AutoResponse.PredefinedCommand4:  SendPredefined(page, 4);                  break;
				case AutoResponse.PredefinedCommand5:  SendPredefined(page, 5);                  break;
				case AutoResponse.PredefinedCommand6:  SendPredefined(page, 6);                  break;
				case AutoResponse.PredefinedCommand7:  SendPredefined(page, 7);                  break;
				case AutoResponse.PredefinedCommand8:  SendPredefined(page, 8);                  break;
				case AutoResponse.PredefinedCommand9:  SendPredefined(page, 9);                  break;
				case AutoResponse.PredefinedCommand10: SendPredefined(page, 10);                 break;
				case AutoResponse.PredefinedCommand11: SendPredefined(page, 11);                 break;
				case AutoResponse.PredefinedCommand12: SendPredefined(page, 12);                 break;

				case AutoResponse.Explicit:
					SendCommand(new Command(this.settingsRoot.AutoResponse.Response)); // No explicit default radix available (yet).
					break;

				case AutoResponse.None:
				default:
					break;
			}
		}

		/// <summary>
		/// Sends the automatic response trigger.
		/// </summary>
		protected virtual void SendAutoResponseTrigger(byte[] triggerSequence)
		{
			if (triggerSequence != null)
				this.terminal.Send(TriggerSequenceWithTxEol(triggerSequence));
		}

		private byte[] TriggerSequenceWithTxEol(byte[] triggerSequence)
		{
			var l = new List<byte>(triggerSequence);

			if (this.settingsRoot.TerminalType == Domain.TerminalType.Text)
			{
				var textTerminal = (this.terminal as Domain.TextTerminal);

				// Add Tx EOL:
				var txEolSequence = textTerminal.TxEolSequence;
				l.AddRange(txEolSequence);
			}

			return (l.ToArray());
		}

		/// <summary>
		/// Gets the automatic response count.
		/// </summary>
		public virtual int AutoResponseCount
		{
			get
			{
				AssertNotDisposed();

				return (this.autoResponseCount);
			}
		}

		/// <summary>
		/// Resets the automatic response count.
		/// </summary>
		public virtual void ResetAutoResponseCount()
		{
			AssertNotDisposed();

			this.autoResponseCount = 0;
			OnAutoResponseCountChanged(new EventArgs<int>(this.autoResponseCount));
		}

		/// <summary>
		/// Deactivates the automatic response.
		/// </summary>
		public virtual void DeactivateAutoResponse()
		{
			AssertNotDisposed();

			this.settingsRoot.AutoResponse.Deactivate();
			ResetAutoResponseCount();
		}

		#endregion

		#region Auto > Action
		//------------------------------------------------------------------------------------------
		// Auto > Action
		//------------------------------------------------------------------------------------------

		private void CreateAutoActionHelper()
		{
			UpdateAutoAction(); // Simply forward to general Update() method.
		}

		private void DisposeAutoActionHelper()
		{
			lock (this.autoActionTriggerHelperSyncObj)
				this.autoActionTriggerHelper = null; // Simply delete the reference to the object.
		}

		/// <summary>
		/// Updates the automatic action helper.
		/// </summary>
		protected virtual void UpdateAutoAction()
		{
			if (this.settingsRoot.AutoAction.IsActive)
			{
				if (this.settingsRoot.AutoAction.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					byte[] triggerSequence;
					if (TryParseCommandToSequence(this.settingsRoot.ActiveAutoActionTrigger, out triggerSequence))
					{
						lock (this.autoActionTriggerHelperSyncObj)
						{
							if (this.autoActionTriggerHelper == null)
								this.autoActionTriggerHelper = new AutoTriggerHelper(triggerSequence);
							else
								this.autoActionTriggerHelper.TriggerSequence = triggerSequence;
						}
					}
					else
					{
						DeactivateAutoAction();
						DisposeAutoActionHelper();

						OnMessageInputRequest
						(
							"Failed to parse the automatic action trigger! Automatic action has been disabled!" + Environment.NewLine + Environment.NewLine +
							"To enable again, re-configure the automatic action.",
							"Automatic Action Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
				else // No command required = no sequence required = no helper required.
				{
					DisposeAutoActionHelper();
				}
			}
			else // Disabled.
			{
				DisposeAutoActionHelper();
			}
		}

		/// <summary>
		/// Invokes the automatic action.
		/// </summary>
		protected virtual void InvokeAutoAction(AutoAction action, byte[] triggerSequence, DateTime ts)
		{
			this.autoActionCount++; // Incrementing before invoking to have the effective count available during invoking.
			OnAutoActionCountChanged(new EventArgs<int>(this.autoActionCount));

			switch (action)
			{
				case AutoAction.Highlight:                       /* no additional action */                                    break;
				case AutoAction.Filter:                          /* no additional action */                                    break;
				case AutoAction.Suppress:                        /* no additional action */                                    break;
				case AutoAction.Beep:                            SystemSounds.Beep.Play();                                     break;
				case AutoAction.ShowMessageBox:                  RequestAutoActionMessage(triggerSequence, ts);                break;
				case AutoAction.ClearRepositories:               ClearRepositories();                                          break;
				case AutoAction.ClearRepositoriesOnSubsequentRx: this.autoActionClearRepositoriesOnSubsequentRxIsArmed = true; break;
				case AutoAction.ResetCountAndRate:               ResetIOCountAndRate();                                        break;
				case AutoAction.SwitchLogOn:                     SwitchLogOn();                                                break;
				case AutoAction.SwitchLogOff:                    SwitchLogOff();                                               break;
				case AutoAction.StopIO:                          StopIO();                                                     break;
				case AutoAction.CloseTerminal:                   Close();                                                      break;
				case AutoAction.ExitApplication:                 OnExitRequest(EventArgs.Empty);                               break;

				case AutoAction.None:
				default:
					break;
			}
		}

		/// <summary>
		/// Notifies the user about the action.
		/// </summary>
		protected virtual void RequestAutoActionMessage(byte[] triggerSequence, DateTime ts)
		{
			var sb = new StringBuilder();
			sb.Append(@"Automatic message has been triggered by """);
			sb.Append(this.terminal.Format(triggerSequence, Domain.IODirection.Rx));
			sb.Append(@""" the ");
			sb.Append(this.autoActionCount);
			sb.Append(Int32Ex.ToEnglishSuffix(this.autoActionCount));
			sb.Append(" time at ");
			sb.Append(this.terminal.Format(ts, Domain.Direction.Rx));
			sb.Append(".");

			OnMessageInputRequest
			(
				sb.ToString(),
				IndicatedName,
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		/// <summary>
		/// Gets the automatic action count.
		/// </summary>
		public virtual int AutoActionCount
		{
			get
			{
				AssertNotDisposed();

				return (this.autoActionCount);
			}
		}

		/// <summary>
		/// Resets the automatic action count.
		/// </summary>
		public virtual void ResetAutoActionCount()
		{
			AssertNotDisposed();

			this.autoActionCount = 0;
			OnAutoActionCountChanged(new EventArgs<int>(this.autoActionCount));
		}

		/// <summary>
		/// Deactivates the automatic action.
		/// </summary>
		public virtual void DeactivateAutoAction()
		{
			AssertNotDisposed();

			this.settingsRoot.AutoAction.Deactivate();
			ResetAutoActionCount();
		}

		#endregion

		#region Auto > Common
		//------------------------------------------------------------------------------------------
		// Auto > Common
		//------------------------------------------------------------------------------------------

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
					if (this.terminal.TryParseText(c.SingleLineText, out lineResult, c.DefaultRadix))
					{
						sequence = lineResult;
						return (true);
					}
				}
				else if (c.IsMultiLineText)
				{
					List<byte> commandResult = new List<byte>(256); // Preset the initial capacity to improve memory management; 256 is an arbitrary value.

					foreach (string line in c.MultiLineText)
					{
						byte[] lineResult;
						if (this.terminal.TryParseText(line, out lineResult, c.DefaultRadix))
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

		#endregion

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(Domain.IOControlEventArgs e)
		{
			this.eventHelper.RaiseSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOConnectTimeChanged(TimeSpanEventArgs e)
		{
			this.eventHelper.RaiseSync<TimeSpanEventArgs>(IOConnectTimeChanged, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		protected virtual void OnIOCountChanged_Promptly(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOCountChanged_Promptly, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		protected virtual void OnIORateChanged_Promptly(EventArgs e)
		{
			this.eventHelper.RaiseSync(IORateChanged_Promptly, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		protected virtual void OnIORateChanged_Decimated(EventArgs e)
		{
			this.eventHelper.RaiseSync(IORateChanged_Decimated, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(Domain.IOErrorEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLinePartSentReplaced(Domain.DisplayLinePartEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayLinePartEventArgs>(CurrentDisplayLinePartSentReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLinePartReceivedReplaced(Domain.DisplayLinePartEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayLinePartEventArgs>(CurrentDisplayLinePartReceivedReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineSentCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(CurrentDisplayLineSentCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineReceivedCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(CurrentDisplayLineReceivedCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesSent(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(EventArgs<Domain.RepositoryType> e)
		{
			this.eventHelper.RaiseSync<EventArgs<Domain.RepositoryType>>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(EventArgs<Domain.RepositoryType> e)
		{
			this.eventHelper.RaiseSync<EventArgs<Domain.RepositoryType>>(RepositoryReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoResponseCountChanged(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoResponseCountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoActionCountChanged(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoActionCountChanged, this, e);
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(FixedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(TimedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Not using event args parameter for simplicity.</remarks>
		protected virtual void OnResetStatusTextRequest()
		{
			this.eventHelper.RaiseSync(ResetStatusTextRequest, this, EventArgs.Empty);
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

				var e = new MessageInputEventArgs(text, caption, buttons, icon, defaultButton);
				this.eventHelper.RaiseSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Message Input' request by terminal '" + Caption + "' was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <summary>
		/// Requests to show the 'SaveAs' dialog to let the user chose a file path.
		/// If confirmed, the file will be saved to that path.
		/// </summary>
		protected virtual DialogResult OnSaveAsFileDialogRequest()
		{
			if (this.startArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new DialogEventArgs();
				this.eventHelper.RaiseSync<DialogEventArgs>(SaveAsFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Save As' request by terminal '" + Caption + "' was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

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
			this.eventHelper.RaiseSync<EventArgs<Cursor>>(CursorRequest, this, new EventArgs<Cursor>(cursor));
		}

		/// <summary></summary>
		protected virtual void OnCursorReset()
		{
			OnCursorRequest(Cursors.Default);
		}

		/// <summary></summary>
		protected virtual void OnSaved(SavedEventArgs e)
		{
			this.eventHelper.RaiseSync<SavedEventArgs>(Saved, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(ClosedEventArgs e)
		{
			this.eventHelper.RaiseSync<ClosedEventArgs>(Closed, this, e);
		}

		/// <summary></summary>
		protected virtual void OnExitRequest(EventArgs e)
		{
			this.eventHelper.RaiseSync<EventArgs>(ExitRequest, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method!

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
			if ((message == "Sending...") || (message == "Receiving..."))
				return; // Skip messages not useful for debugging.

			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
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
