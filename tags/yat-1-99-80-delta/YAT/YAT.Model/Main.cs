//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
using MKY.Diagnostics;
using MKY.IO;
using MKY.Settings;

using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Provides the YAT application model which can handle workspaces (.yaw) and terminals (.yat).
	/// </summary>
	public class Main : IDisposable, IGuidProvider
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow autonomously ignoring exceptions when disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Main).FullName);

		private CommandLineArgs commandLineArgs;
		private MainStartArgs startArgs;
		private Guid guid;

		private Workspace workspace;

		private MainResult result = MainResult.Success;

		private System.Timers.Timer operationTimer;
		private System.Timers.Timer exitTimer;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<EventArgs<Workspace>> WorkspaceOpened;

		/// <summary></summary>
		public event EventHandler<ClosedEventArgs> WorkspaceClosed;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<Cursor>> CursorRequest;

		/// <summary></summary>
		public event EventHandler Started;

		/// <summary></summary>
		public event EventHandler<EventArgs<MainResult>> Exited;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Main()
			: this(default(string))
		{
		}

		/// <summary></summary>
		public Main(string requestedFilePath)
			: this(new CommandLineArgs(new string[] { requestedFilePath }))
		{
		}

		/// <summary></summary>
		public Main(CommandLineArgs commandLineArgs)
		{
			this.commandLineArgs = commandLineArgs;

			DebugMessage("Creating...");
			this.guid = Guid.NewGuid();
			AttachStaticSerialPortCollectionEventHandlers();
			DebugMessage("...successfully created.");
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
					StopAndDisposeOperationTimer();
					StopAndDisposeExitTimer();

					// In the 'normal' case, the workspace has already been closed, otherwise...

					// ...detach event handlers to ensure that no more events are received...
					DetachWorkspaceEventHandlers();

					// ...dispose of workspace (normally it disposes of itself)...
					if (this.workspace != null)
						this.workspace.Dispose();

					DetachStaticSerialPortCollectionEventHandlers();
				}

				// Set state to disposed:
				this.workspace = null;
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
		~Main()
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

		#region Properties
		//==========================================================================================
		// Properties
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
		public virtual MainStartArgs StartArgs
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.startArgs);
			}
		}

		/// <summary>
		/// This is the indicated main name. The name is corresponding to the indicated name of
		/// the currently active workspace, which is corresponding to the currently active terminal;
		/// <see cref="ApplicationEx.ProductName"/> otherwise.
		/// </summary>
		public virtual string IndicatedName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.workspace != null)
					return (this.workspace.IndicatedName);
				else
					return (ApplicationEx.ProductName);
			}
		}

		/// <summary>
		/// Returns workspace within main or <c>null</c> if no workspace is active.
		/// </summary>
		public virtual Workspace Workspace
		{
			get
			{
				AssertNotDisposed();

				return (this.workspace);
			}
		}

		/// <summary></summary>
		public MainResult Result
		{
			get { return (this.result); }
		}

		#endregion

		#region Start
		//==========================================================================================
		// Start
		//==========================================================================================

		/// <summary>
		/// This method is used to test the command line argument processing.
		/// </summary>
		public virtual MainResult PrepareStart()
		{
			AssertNotDisposed();

			// Process command line args into start requests:
			if (ProcessCommandLineArgsIntoStartRequests())
				return (MainResult.Success);
			else
				return (MainResult.CommandLineError);
		}

		/// <summary>
		/// If a file was requested by command line argument, this method tries to open the
		/// requested file.
		/// Else, this method tries to open the most recent workspace of the current user.
		/// If still unsuccessful, a new workspace is created.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if either operation succeeded; otherwise, <c>false</c>.
		/// </returns>
		public virtual MainResult Start()
		{
			AssertNotDisposed();

			// Process command line args into start requests:
			if (!ProcessCommandLineArgsIntoStartRequests())
			{
				if ((this.commandLineArgs == null) || (!this.commandLineArgs.IsValid))
					return (MainResult.CommandLineError);
				else
					return (MainResult.ApplicationStartError);
			}

			// YAT will fully start, hence initialize required resources:
			ProcessorLoad.Initialize();

			// Start YAT according to the start requests:
			bool success = false;

			bool workspaceIsRequested = (this.startArgs.WorkspaceSettingsHandler != null);
			bool terminalIsRequested  = (this.startArgs.TerminalSettingsHandler  != null);

			if (workspaceIsRequested || terminalIsRequested)
			{
				if (workspaceIsRequested && terminalIsRequested)
				{
					success = OpenWorkspaceFromSettings(this.startArgs.WorkspaceSettingsHandler, this.startArgs.RequestedDynamicTerminalIndex, this.startArgs.TerminalSettingsHandler);
				}
				else if (workspaceIsRequested) // Workspace only.
				{
					success = OpenWorkspaceFromSettings(this.startArgs.WorkspaceSettingsHandler);
				}
				else // Terminal only.
				{
					success = OpenTerminalFromSettings(this.startArgs.TerminalSettingsHandler);
				}

				// Note that any existing auto workspace settings are kept as they are.
				// Thus, they can be used again at the next 'normal' start of YAT.
			}

			if (!success && ApplicationSettings.LocalUserSettingsAreCurrentlyOwnedByThisInstance && !this.commandLineArgs.Empty)
			{
				if (ApplicationSettings.LocalUserSettings.General.AutoOpenWorkspace)
				{
					string filePath = ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath;
					if (PathEx.IsValid(filePath))
					{
						if (File.Exists(filePath))
							success = OpenWorkspaceFromFile(filePath);

						if (!success && !ApplicationSettings.LocalUserSettings.AutoWorkspace.AutoSaved)
						{
							OnFixedStatusTextRequest("Error opening workspace!");

							var errorMessage = ErrorHelper.ComposeMessage
							(
								"Unable to open the previous workspace file",
								filePath,
								"Confirm with [OK] to create a new empty workspace, or [Cancel] to let you restore the workspace and try again."
							);

							var dr = OnMessageInputRequest
							(
								errorMessage,
								"Workspace File Error",
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Warning
							);

							if (dr == DialogResult.Cancel)
							{
								OnTimedStatusTextRequest("Canceled");

								// Not creating create a new empty workspace allows the user to exit YAT,
								// restore the .yaw file, and try again.
								return (MainResult.ApplicationStartCancel);
							}

							// [OK] => A new empty workspace will be created below.
						}
					}
				}

				// Clean up the local user directory:
				CleanupLocalUserDirectory();
			}

			// If no success so far, create a new empty workspace:
			if (!success)
			{
				// Reset workspace file path:
				ApplicationSettings.LocalUserSettings.AutoWorkspace.ResetFilePath();
				ApplicationSettings.Save();

				success = CreateNewWorkspace();
			}

			// Start all included terminals:
			if (success)
			{
				this.workspace.StartAllTerminals(); // Don't care about success, workspace itself is fine.
			}

			// If requested, trigger operation:
			if (success && this.StartArgs.PerformOperationOnRequestedTerminal)
			{
				OnFixedStatusTextRequest("Triggering operation...");
				CreateAndStartOperationTimer();
			}

			if (success)
			{
				OnStarted();
				return (MainResult.Success);
			}
			else
			{
				return (MainResult.ApplicationStartError);
			}
		}

		#region Start > Private Methods
		//------------------------------------------------------------------------------------------
		// Start > Private Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Process the command line arguments according to their priority and translate them into
		/// start requests.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:SingleLineCommentMustBePrecededByBlankLine", Justification = "Consistent section titles.")]
		private bool ProcessCommandLineArgsIntoStartRequests()
		{
			// Always create start requests to ensure that object exists.
			this.startArgs = new MainStartArgs();

			// Process and validate command line arguments:
			if (this.commandLineArgs != null)
				this.commandLineArgs.ProcessAndValidate();

			// In normal operation this is the location where the command line arguments are
			// processed and validated for a second time AFTER the application settings have been
			// created/loaded. They have already been processed and validated for a first time
			// BEFORE the application settings were created/loaded. This first processing happens
			// in YAT.Controller.Main.Run().
			// 
			// In case of automated testing, the command line arguments may also be processed and
			// validated here for the first time.

			// Prio 0 = None:
			if ((this.commandLineArgs == null) || this.commandLineArgs.NoArgs)
			{
				// This is the standard case, the 'New Terminal' dialog will get shown:
				if (this.commandLineArgs == null)
				{
					this.startArgs.ShowNewTerminalDialog = true;
					this.startArgs.KeepOpen              = true;
					this.startArgs.KeepOpenOnError       = true;

					return (true);
				}
				// In case of "YATConsole.exe" the controller will set the 'NonInteractive' option,
				// i.e. 'Interactive' will be cleared, and no 'New Terminal' dialog shall get shown:
				else
				{
					this.startArgs.ShowNewTerminalDialog = this.commandLineArgs.Interactive;
					this.startArgs.KeepOpen              = true;
					this.startArgs.KeepOpenOnError       = true;

					return (true);
				}
			}

			// Prio 1 = Invalid:
			if (!this.commandLineArgs.IsValid)
			{
				return (false);
			}

			// Prio 2 = Empty:
			if (this.commandLineArgs.Empty)
			{
				this.startArgs.ShowNewTerminalDialog = false;
				this.startArgs.KeepOpen              = this.commandLineArgs.KeepOpen;
				this.startArgs.KeepOpenOnError       = this.commandLineArgs.KeepOpenOnError;

				return (true);
			}

			// Arguments are available and valid, transfer 'NonInteractive' option:
			this.startArgs.NonInteractive = this.commandLineArgs.NonInteractive;

			// Prio 3 = Recent:
			string requestedFilePath = null;
			if (this.commandLineArgs.MostRecentIsRequested)
			{
				if (!string.IsNullOrEmpty(this.commandLineArgs.MostRecentFilePath))
					requestedFilePath = this.commandLineArgs.MostRecentFilePath;
				else
					return (false);
			}
			// Prio 4 = New:
			else if (this.commandLineArgs.NewIsRequested)
			{
				// No file path to open.
			}
			// Prio 5 = Requested file path as option:
			else if (!string.IsNullOrEmpty(this.commandLineArgs.RequestedFilePath))
			{
				requestedFilePath = this.commandLineArgs.RequestedFilePath;
			}
			// Prio 6 = Value argument:
			else if (this.commandLineArgs.ValueArgsCount > 0)
			{
				requestedFilePath = this.commandLineArgs.ValueArgs[0];
			}

			// Open the requested settings file:
			if (!string.IsNullOrEmpty(requestedFilePath))
			{
				if (ExtensionHelper.IsWorkspaceFile(requestedFilePath))
				{
					DocumentSettingsHandler<WorkspaceSettingsRoot> sh;
					Exception ex;
					if (OpenWorkspaceFile(requestedFilePath, out sh, out ex))
					{
						this.startArgs.WorkspaceSettingsHandler = sh;
					}
					else
					{
						this.startArgs.ErrorMessage = ErrorHelper.ComposeMessage("Unable to open workspace file", requestedFilePath, ex);
						return (false);
					}
				}
				else if (ExtensionHelper.IsTerminalFile(requestedFilePath))
				{
					DocumentSettingsHandler<TerminalSettingsRoot> sh;
					Exception ex;
					if (OpenTerminalFile(requestedFilePath, out sh, out ex))
					{
						this.startArgs.TerminalSettingsHandler = sh;
					}
					else
					{
						this.startArgs.ErrorMessage = ErrorHelper.ComposeMessage("Unable to open terminal file", requestedFilePath, ex);
						return (false);
					}
				}
				else
				{
					return (false);
				}
			}

			// Prio 7 = Retrieve the requested terminal within the workspace and validate it:
			if (this.startArgs.WorkspaceSettingsHandler != null) // Applies to a terminal within a workspace.
			{
				int requestedDynamicTerminalIndex = this.commandLineArgs.RequestedDynamicTerminalIndex;
				int lastDynamicIndex = Indices.IndexToDynamicIndex(this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count - 1);
				
				if     ((requestedDynamicTerminalIndex >= Indices.FirstDynamicIndex) && (requestedDynamicTerminalIndex <= lastDynamicIndex))
					this.startArgs.RequestedDynamicTerminalIndex = requestedDynamicTerminalIndex;
				else if (requestedDynamicTerminalIndex == Indices.DefaultDynamicIndex)
					this.startArgs.RequestedDynamicTerminalIndex = Indices.DefaultDynamicIndex;
				else if (requestedDynamicTerminalIndex == Indices.InvalidDynamicIndex)
					this.startArgs.RequestedDynamicTerminalIndex = Indices.InvalidDynamicIndex; // Usable to disable the operation.
				else
					return (false);

				if (this.startArgs.RequestedDynamicTerminalIndex != Indices.InvalidDynamicIndex)
				{
					string workspaceFilePath = this.startArgs.WorkspaceSettingsHandler.SettingsFilePath;

					string terminalFilePath;
					if (this.startArgs.RequestedDynamicTerminalIndex == Indices.DefaultDynamicIndex) // The last terminal is the default.
						terminalFilePath = this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings[this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count - 1].FilePath;
					else
						terminalFilePath = this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings[Indices.DynamicIndexToIndex(this.startArgs.RequestedDynamicTerminalIndex)].FilePath;

					DocumentSettingsHandler<TerminalSettingsRoot> sh;
					if (OpenTerminalFile(workspaceFilePath, terminalFilePath, out sh))
						this.startArgs.TerminalSettingsHandler = sh;
					else
						return (false);
				}
			}
			else if (this.startArgs.TerminalSettingsHandler != null) // Applies to a dedicated terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalIndex == Indices.InvalidDynamicIndex)
					this.startArgs.RequestedDynamicTerminalIndex = Indices.InvalidDynamicIndex; // Usable to disable the operation.
			}
			else if (this.commandLineArgs.NewIsRequested) // Applies to a new terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalIndex == Indices.InvalidDynamicIndex)
					this.startArgs.RequestedDynamicTerminalIndex = Indices.InvalidDynamicIndex; // Usable to disable the operation.

				this.startArgs.TerminalSettingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();
			}
			else
			{
				this.startArgs.RequestedDynamicTerminalIndex = Indices.InvalidDynamicIndex; // Disable the operation.
			}

			// Prio 8 = Override settings as desired:
			if (this.startArgs.TerminalSettingsHandler != null)
			{
				bool terminalIsStarted = this.startArgs.TerminalSettingsHandler.Settings.TerminalIsStarted;
				bool logIsOn           = this.startArgs.TerminalSettingsHandler.Settings.LogIsOn;

				if (ProcessCommandLineArgsIntoExistingTerminalSettings(this.startArgs.TerminalSettingsHandler.Settings.Terminal, ref terminalIsStarted, ref logIsOn))
				{
					this.startArgs.TerminalSettingsHandler.Settings.TerminalIsStarted = terminalIsStarted;
					this.startArgs.TerminalSettingsHandler.Settings.LogIsOn           = logIsOn;
				}
				else
				{
					return (false);
				}
			}

			// Prio 9 = Perform requested operation:
			if (this.startArgs.RequestedDynamicTerminalIndex != Indices.InvalidDynamicIndex)
			{
				if (this.commandLineArgs.OptionIsGiven("TransmitText"))
				{
					this.startArgs.RequestedTransmitText = this.commandLineArgs.RequestedTransmitText;
					this.startArgs.PerformOperationOnRequestedTerminal = true;
				}
				else if (this.commandLineArgs.OptionIsGiven("TransmitFile"))
				{
					this.startArgs.RequestedTransmitFilePath = this.commandLineArgs.RequestedTransmitFilePath;
					this.startArgs.PerformOperationOnRequestedTerminal = true;
				}
			}

			// Prio 10 = Set behavior:
			if (this.startArgs.PerformOperationOnRequestedTerminal)
			{
				this.startArgs.OperationDelay = this.commandLineArgs.OperationDelay;
				this.startArgs.ExitDelay      = this.commandLineArgs.ExitDelay;

				this.startArgs.KeepOpen        = this.commandLineArgs.KeepOpen;
				this.startArgs.KeepOpenOnError = this.commandLineArgs.KeepOpenOnError;
			}
			else
			{
				this.startArgs.KeepOpen        = true;
				this.startArgs.KeepOpenOnError = true;
			}

			// Prio 11 = Tile:
			this.startArgs.TileHorizontal = this.commandLineArgs.TileHorizontal;
			this.startArgs.TileVertical   = this.commandLineArgs.TileVertical;

			return (true);
		}

		/// <summary>
		/// This method takes existing terminal settings and modifies/overrides those settings that
		/// are given by the given command line args.
		/// </summary>
		/// <remarks>
		/// Unfortunately, 'normal' terminal settings and new terminal settings are defined rather
		/// differently. Therefore, this implementation looks a bit weird.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Well...")]
		private bool ProcessCommandLineArgsIntoExistingTerminalSettings(Domain.Settings.TerminalSettings terminalSettings, ref bool terminalIsStarted, ref bool logIsOn)
		{
			if (this.commandLineArgs.OptionIsGiven("TerminalType"))
			{
				Domain.TerminalType terminalType;
				if (Domain.TerminalTypeEx.TryParse(this.commandLineArgs.TerminalType, out terminalType))
					terminalSettings.TerminalType = terminalType;
				else
					return (false);
			}

			if (this.commandLineArgs.OptionIsGiven("PortType"))
			{
				Domain.IOType ioType;
				if (Domain.IOTypeEx.TryParse(this.commandLineArgs.IOType, out ioType))
					terminalSettings.IO.IOType = ioType;
				else
					return (false);
			}

			Domain.IOType finalIOType = terminalSettings.IO.IOType;
			if (finalIOType == Domain.IOType.SerialPort)
			{
				if (this.commandLineArgs.OptionIsGiven("SerialPort"))
				{
					MKY.IO.Ports.SerialPortId portId;
					if (MKY.IO.Ports.SerialPortId.TryFrom(this.commandLineArgs.SerialPort, out portId))
						terminalSettings.IO.SerialPort.PortId = portId;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("BaudRate"))
				{
					MKY.IO.Ports.BaudRateEx baudRate;
					if (MKY.IO.Ports.BaudRateEx.TryFrom(this.commandLineArgs.BaudRate, out baudRate))
						terminalSettings.IO.SerialPort.Communication.BaudRate = baudRate;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("DataBits"))
				{
					MKY.IO.Ports.DataBits dataBits;
					if (MKY.IO.Ports.DataBitsEx.TryFrom(this.commandLineArgs.DataBits, out dataBits))
						terminalSettings.IO.SerialPort.Communication.DataBits = dataBits;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("Parity"))
				{
					System.IO.Ports.Parity parity;
					if (MKY.IO.Ports.ParityEx.TryParse(this.commandLineArgs.Parity, out parity))
						terminalSettings.IO.SerialPort.Communication.Parity = parity;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("StopBits"))
				{
					System.IO.Ports.StopBits stopBits;
					if (MKY.IO.Ports.StopBitsEx.TryFrom(this.commandLineArgs.StopBits, out stopBits))
						terminalSettings.IO.SerialPort.Communication.StopBits = stopBits;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("FlowControl"))
				{
					MKY.IO.Serial.SerialPort.SerialFlowControl flowControl;
					if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryParse(this.commandLineArgs.FlowControl, out flowControl))
						terminalSettings.IO.SerialPort.Communication.FlowControl = flowControl;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("SerialPortAliveMonitor"))
				{
					if (this.commandLineArgs.SerialPortAliveMonitor == 0)
						terminalSettings.IO.SerialPort.AliveMonitor = new MKY.IO.Serial.AutoInterval(false, 0);
					else if (this.commandLineArgs.SerialPortAliveMonitor >= MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorMinInterval)
						terminalSettings.IO.SerialPort.AliveMonitor = new MKY.IO.Serial.AutoInterval(true, this.commandLineArgs.SerialPortAliveMonitor);
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("SerialPortAutoReopen"))
				{
					if (this.commandLineArgs.SerialPortAutoReopen == 0)
						terminalSettings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.AutoInterval(false, 0);
					else if (this.commandLineArgs.SerialPortAutoReopen >= MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenMinInterval)
						terminalSettings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.AutoInterval(true, this.commandLineArgs.SerialPortAutoReopen);
					else
						return (false);
				}
			}
			else if ((finalIOType == Domain.IOType.TcpClient) ||
					 (finalIOType == Domain.IOType.TcpServer) ||
					 (finalIOType == Domain.IOType.TcpAutoSocket) ||
					 (finalIOType == Domain.IOType.UdpClient) ||
					 (finalIOType == Domain.IOType.UdpServer) ||
					 (finalIOType == Domain.IOType.UdpPairSocket))
			{
				if (this.commandLineArgs.OptionIsGiven("RemoteHost"))
				{
					MKY.Net.IPHostEx remoteHost;
					if (MKY.Net.IPHostEx.TryParse(this.commandLineArgs.RemoteHost, out remoteHost))
						terminalSettings.IO.Socket.RemoteHost = remoteHost;
					else
						return (false);
				}
				if (((finalIOType == Domain.IOType.TcpClient) ||
					 (finalIOType == Domain.IOType.TcpAutoSocket) ||
					 (finalIOType == Domain.IOType.UdpClient) ||
					 (finalIOType == Domain.IOType.UdpPairSocket)) &&
					this.commandLineArgs.OptionIsGiven("RemotePort"))
				{
					if (MKY.Net.IPEndPointEx.IsValidPort(this.commandLineArgs.RemotePort))
						terminalSettings.IO.Socket.RemotePort = this.commandLineArgs.RemotePort;
					else
						return (false);
				}
				if (((finalIOType == Domain.IOType.TcpClient) || 
					 (finalIOType == Domain.IOType.TcpServer) ||
					 (finalIOType == Domain.IOType.TcpAutoSocket)) &&
					this.commandLineArgs.OptionIsGiven("LocalInterface"))
				{
					MKY.Net.IPNetworkInterfaceEx localInterface;
					if (MKY.Net.IPNetworkInterfaceEx.TryParse(this.commandLineArgs.LocalInterface, out localInterface))
						terminalSettings.IO.Socket.LocalInterface = localInterface;
					else
						return (false);
				}
				if (((finalIOType == Domain.IOType.UdpServer) ||
					 (finalIOType == Domain.IOType.UdpPairSocket)) &&
					this.commandLineArgs.OptionIsGiven("LocalFilter"))
				{
					MKY.Net.IPFilterEx localFilter;
					if (MKY.Net.IPFilterEx.TryParse(this.commandLineArgs.LocalFilter, out localFilter))
						terminalSettings.IO.Socket.LocalFilter = localFilter;
					else
						return (false);
				}
				if (((finalIOType == Domain.IOType.TcpServer) ||
					 (finalIOType == Domain.IOType.TcpAutoSocket) ||
					 (finalIOType == Domain.IOType.UdpServer) ||
					 (finalIOType == Domain.IOType.UdpPairSocket)) &&
					this.commandLineArgs.OptionIsGiven("LocalPort"))
				{
					if (MKY.Net.IPEndPointEx.IsValidPort(this.commandLineArgs.LocalPort))
						terminalSettings.IO.Socket.LocalPort = this.commandLineArgs.LocalPort;
					else
						return (false);
				}
				if ((finalIOType == Domain.IOType.TcpClient) &&
					this.commandLineArgs.OptionIsGiven("TCPAutoReconnect"))
				{
					if (this.commandLineArgs.TcpAutoReconnect == 0)
						terminalSettings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.AutoInterval(false, 0);
					else if (this.commandLineArgs.TcpAutoReconnect >= MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectMinInterval)
						terminalSettings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.AutoInterval(true, this.commandLineArgs.TcpAutoReconnect);
					else
						return (false);
				}
				if ((finalIOType == Domain.IOType.UdpServer) &&
					this.commandLineArgs.OptionIsGiven("UdpServerSendMode"))
				{
					MKY.IO.Serial.Socket.UdpServerSendMode sendMode;
					if (MKY.IO.Serial.Socket.UdpServerSendModeEx.TryFrom(this.commandLineArgs.UdpServerSendMode, out sendMode))
						terminalSettings.IO.Socket.UdpServerSendMode = sendMode;
					else
						return (false);
				}
			}
			else if (finalIOType == Domain.IOType.UsbSerialHid)
			{
				bool vendorIdIsGiven  = this.commandLineArgs.OptionIsGiven("VendorID");
				bool productIdIsGiven = this.commandLineArgs.OptionIsGiven("ProductId");
				if (vendorIdIsGiven || productIdIsGiven)
				{
					// Both VID and PID must be given!
					if (vendorIdIsGiven && productIdIsGiven)
					{
						int vendorId;
						int productId;

						if (!int.TryParse(this.commandLineArgs.VendorId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out vendorId))
							return (false);

						if (!MKY.IO.Usb.DeviceInfo.IsValidVendorId(vendorId))
							return (false);

						if (!int.TryParse(this.commandLineArgs.ProductId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out productId))
							return (false);

						if (!MKY.IO.Usb.DeviceInfo.IsValidProductId(productId))
							return (false);

						// The SNR is optional:
						if (!this.commandLineArgs.OptionIsGiven("SerialString"))
						{
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.DeviceInfo(vendorId, productId);
							terminalSettings.IO.UsbSerialHidDevice.MatchSerial = false;
						}
						else
						{
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.DeviceInfo(vendorId, productId, this.commandLineArgs.SerialString);
							terminalSettings.IO.UsbSerialHidDevice.MatchSerial = true;
						}
					}
					else
					{
						return (false);
					}
				}
				if (this.commandLineArgs.OptionIsGiven("FormatPreset"))
				{
					terminalSettings.IO.UsbSerialHidDevice.ReportFormat  = (MKY.IO.Usb.SerialHidReportFormatPresetEx)this.commandLineArgs.FormatPreset;
					terminalSettings.IO.UsbSerialHidDevice.RxFilterUsage = (MKY.IO.Usb.SerialHidReportFormatPresetEx)this.commandLineArgs.FormatPreset;
				}
				if (this.commandLineArgs.OptionIsGiven("FlowControl"))
				{
					MKY.IO.Serial.Usb.SerialHidFlowControl flowControl;
					if (MKY.IO.Serial.Usb.SerialHidFlowControlEx.TryParse(this.commandLineArgs.FlowControl, out flowControl))
						terminalSettings.IO.UsbSerialHidDevice.FlowControl = flowControl;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("NoUSBAutoOpen"))
				{
					terminalSettings.IO.UsbSerialHidDevice.AutoOpen = !this.commandLineArgs.NoUsbAutoOpen;
				}
			}
			else
			{
				return (false);
			}

			if (this.commandLineArgs.OptionIsGiven("OpenTerminal"))
				terminalIsStarted = this.commandLineArgs.OpenTerminal;

			if (this.commandLineArgs.OptionIsGiven("LogOn"))
				logIsOn = this.commandLineArgs.LogOn;

			return (true);
		}

		/// <summary>
		/// This method takes existing terminal settings and modifies/overrides those settings that
		/// are given by the given command line args.
		/// </summary>
		/// <remarks>
		/// Unfortunately, 'normal' terminal settings and new terminal settings are defined rather
		/// differently. Therefore, this implementation looks a bit weird.
		/// </remarks>
		public bool ProcessCommandLineArgsIntoExistingNewTerminalSettings(Settings.NewTerminalSettings newTerminalSettings)
		{
			// These are temporary settings. Therefore, child items of these settings are not
			// cloned below. They can simply be assigned and will then later be assigned back.
			Domain.Settings.TerminalSettings terminalSettings = new Domain.Settings.TerminalSettings();

			terminalSettings.TerminalType                        = newTerminalSettings.TerminalType;
			terminalSettings.IO.IOType                           = newTerminalSettings.IOType;

			terminalSettings.IO.SerialPort.PortId                = newTerminalSettings.SerialPortId;
			terminalSettings.IO.SerialPort.Communication         = newTerminalSettings.SerialPortCommunication;
			terminalSettings.IO.SerialPort.AliveMonitor          = newTerminalSettings.SerialPortAliveMonitor;
			terminalSettings.IO.SerialPort.AutoReopen            = newTerminalSettings.SerialPortAutoReopen;

			terminalSettings.IO.Socket.RemoteHost                = newTerminalSettings.SocketRemoteHost;
			terminalSettings.IO.Socket.RemoteTcpPort             = newTerminalSettings.SocketRemoteTcpPort;
			terminalSettings.IO.Socket.RemoteUdpPort             = newTerminalSettings.SocketRemoteUdpPort;
			terminalSettings.IO.Socket.LocalInterface            = newTerminalSettings.SocketLocalInterface;
			terminalSettings.IO.Socket.LocalFilter               = newTerminalSettings.SocketLocalFilter;
			terminalSettings.IO.Socket.LocalTcpPort              = newTerminalSettings.SocketLocalTcpPort;
			terminalSettings.IO.Socket.LocalUdpPort              = newTerminalSettings.SocketLocalUdpPort;
			terminalSettings.IO.Socket.TcpClientAutoReconnect    = newTerminalSettings.TcpClientAutoReconnect;
			terminalSettings.IO.Socket.UdpServerSendMode         = newTerminalSettings.UdpServerSendMode;

			terminalSettings.IO.UsbSerialHidDevice.DeviceInfo    = newTerminalSettings.UsbSerialHidDeviceInfo;
			terminalSettings.IO.UsbSerialHidDevice.MatchSerial   = newTerminalSettings.UsbSerialHidMatchSerial;
			terminalSettings.IO.UsbSerialHidDevice.ReportFormat  = newTerminalSettings.UsbSerialHidReportFormat;
			terminalSettings.IO.UsbSerialHidDevice.RxFilterUsage = newTerminalSettings.UsbSerialHidRxFilterUsage;
			terminalSettings.IO.UsbSerialHidDevice.FlowControl   = newTerminalSettings.UsbSerialHidFlowControl;
			terminalSettings.IO.UsbSerialHidDevice.AutoOpen      = newTerminalSettings.UsbSerialHidAutoOpen;

			bool terminalIsStarted = newTerminalSettings.StartTerminal;
			bool logIsOn           = false; // Doesn't matter, new terminal settings do not have this option.

			if (ProcessCommandLineArgsIntoExistingTerminalSettings(terminalSettings, ref terminalIsStarted, ref logIsOn))
			{
				newTerminalSettings.TerminalType              = terminalSettings.TerminalType;
				newTerminalSettings.IOType                    = terminalSettings.IO.IOType;

				newTerminalSettings.SerialPortId              = terminalSettings.IO.SerialPort.PortId;
				newTerminalSettings.SerialPortCommunication   = terminalSettings.IO.SerialPort.Communication;
				newTerminalSettings.SerialPortAliveMonitor    = terminalSettings.IO.SerialPort.AliveMonitor;
				newTerminalSettings.SerialPortAutoReopen      = terminalSettings.IO.SerialPort.AutoReopen;

				newTerminalSettings.SocketRemoteHost          = terminalSettings.IO.Socket.RemoteHost;
				newTerminalSettings.SocketRemoteTcpPort       = terminalSettings.IO.Socket.RemoteTcpPort;
				newTerminalSettings.SocketRemoteUdpPort       = terminalSettings.IO.Socket.RemoteUdpPort;
				newTerminalSettings.SocketLocalInterface      = terminalSettings.IO.Socket.LocalInterface;
				newTerminalSettings.SocketLocalFilter         = terminalSettings.IO.Socket.LocalFilter;
				newTerminalSettings.SocketLocalTcpPort        = terminalSettings.IO.Socket.LocalTcpPort;
				newTerminalSettings.SocketLocalUdpPort        = terminalSettings.IO.Socket.LocalUdpPort;
				newTerminalSettings.TcpClientAutoReconnect    = terminalSettings.IO.Socket.TcpClientAutoReconnect;
				newTerminalSettings.UdpServerSendMode         = terminalSettings.IO.Socket.UdpServerSendMode;

				newTerminalSettings.UsbSerialHidDeviceInfo    = terminalSettings.IO.UsbSerialHidDevice.DeviceInfo;
				newTerminalSettings.UsbSerialHidMatchSerial   = terminalSettings.IO.UsbSerialHidDevice.MatchSerial;
				newTerminalSettings.UsbSerialHidReportFormat  = terminalSettings.IO.UsbSerialHidDevice.ReportFormat;
				newTerminalSettings.UsbSerialHidRxFilterUsage = terminalSettings.IO.UsbSerialHidDevice.RxFilterUsage;
				newTerminalSettings.UsbSerialHidFlowControl   = terminalSettings.IO.UsbSerialHidDevice.FlowControl;
				newTerminalSettings.UsbSerialHidAutoOpen      = terminalSettings.IO.UsbSerialHidDevice.AutoOpen;

				return (true);
			}
			else
			{
				return (false);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void CleanupLocalUserDirectory()
		{
			// Get all file paths in default directory:
			List<string> localUserDirectoryFilePaths = new List<string>();
			try
			{
				DirectoryInfo localUserDirectory = Directory.GetParent(ApplicationSettings.LocalUserSettingsFilePath);
				localUserDirectoryFilePaths.AddRange(Directory.GetFiles(localUserDirectory.FullName));
			}
			catch
			{
				// Don't care about exceptions.
			}

			// Ensure to leave application settings untouched:
			localUserDirectoryFilePaths.Remove(ApplicationSettings.LocalUserSettingsFilePath);

			// Get all active files:
			List<string> activeFilePaths = new List<string>();
			if (this.workspace != null)
			{
				// Add workspace settings file:
				if (this.workspace.SettingsFileExists)
					activeFilePaths.Add(this.workspace.SettingsFilePath);

				// Add terminal settings files:
				activeFilePaths.AddRange(this.workspace.TerminalSettingsFilePaths);
			}

			// Ensure to leave all active settings untouched:
			foreach (string path in activeFilePaths)
			{
				localUserDirectoryFilePaths.Remove(path);
			}

			// Delete all obsolete files:
			foreach (string path in localUserDirectoryFilePaths)
			{
				FileEx.TryDelete(path);
			}
		}

		#endregion

		#endregion

		#region Open
		//==========================================================================================
		// Open
		//==========================================================================================

		/// <summary>
		/// Opens the workspace or terminal file given.
		/// </summary>
		/// <param name="filePath">Workspace or terminal file.</param>
		/// <returns><c>true</c> if successfully opened the workspace or terminal; otherwise, <c>false</c>.</returns>
		public virtual bool OpenFromFile(string filePath)
		{
			AssertNotDisposed();

			string extension = Path.GetExtension(filePath);
			if (ExtensionHelper.IsWorkspaceFile(extension))
			{
				if (OpenWorkspaceFromFile(filePath))
				{
					this.workspace.StartAllTerminals(); // Don't care about success, workspace itself is fine.

					OnStarted(); // Same as at OpenTerminalFromFile() below.
					return (true);
				}

				return (false);
			}
			else if (ExtensionHelper.IsTerminalFile(extension))
			{
				// Create workspace if it doesn't exist yet.
				bool signalStarted = false;
				if (this.workspace == null)
				{
					if (!CreateNewWorkspace())
						return (false);

					signalStarted = true;
				}

				if (this.workspace.OpenTerminalFromFile(filePath))
				{
					if (this.workspace.ActiveTerminal.Start())
					{
						if (signalStarted)
							OnStarted(); // Same as at OpenWorkspaceFromFile() above.
						
						return (true);
					}
				}

				return (false);
			}
			else
			{
				OnFixedStatusTextRequest("Unknown file type!");
				OnMessageInputRequest
				(
					"File" + Environment.NewLine + filePath + Environment.NewLine +
					"has unknown type!",
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
				);
				OnTimedStatusTextRequest("No file opened!");

				return (false);
			}
		}

		#endregion

		#region Recent Files
		//==========================================================================================
		// Recent Files
		//==========================================================================================

		/// <summary></summary>
		public virtual bool OpenRecent(int userIndex)
		{
			AssertNotDisposed();

			return (OpenFromFile(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[userIndex - 1].Item));
		}

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

		#region Exit
		//==========================================================================================
		// Exit
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Exit", Justification = "Exit() as method name is the obvious name and should be OK for other languages, .NET itself uses it in Application.Exit().")]
		public virtual MainResult Exit()
		{
			bool cancel;
			return (Exit(out cancel));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Exit", Justification = "Exit() as method name is the obvious name and should be OK for other languages, .NET itself uses it in Application.Exit().")]
		public virtual MainResult Exit(out bool cancel)
		{
			bool workspaceSuccess;
			if (this.workspace != null)
				workspaceSuccess = this.workspace.Close(true);
			else
				workspaceSuccess = true;

			if (workspaceSuccess)
			{
				OnFixedStatusTextRequest("Exiting " + ApplicationEx.ProductName + "...");

				// Discard potential exceptions already before signalling the close! Required to
				// prevent exceptions on still ongoing asynchronous callbacks trying to synchronize
				// event callbacks onto the main form which is going to be closed/disposed by
				// the handler of the 'Exited' event below!
				this.eventHelper.DiscardAllExceptions();

				// Signal the exit:
				OnExited(this.result);

				// The main shall dispose of itself to free all resources for sure. It must be done
				// AFTER it raised the 'Exited' event and all subscribers of the event may still
				// refer to a non-disposed object. This is especially important, as the order of
				// the subscribers is not fixed, i.e. a subscriber may dispose of the main before
				// 'View.Main' receives the event callback!
				Dispose();

				cancel = false;
				return (this.result);
			}
			else
			{
				OnTimedStatusTextRequest("Exit canceled.");

				cancel = true;
				return (this.result);
			}
		}

		#endregion

		#region SerialPortCollection
		//==========================================================================================
		// SerialPortCollection
		//==========================================================================================

		private void AttachStaticSerialPortCollectionEventHandlers()
		{
			MKY.IO.Ports.SerialPortCollection.InUseLookupRequest += SerialPortCollection_InUseLookupRequest;
		}

		private void DetachStaticSerialPortCollectionEventHandlers()
		{
			MKY.IO.Ports.SerialPortCollection.InUseLookupRequest -= SerialPortCollection_InUseLookupRequest;
		}

		private void SerialPortCollection_InUseLookupRequest(object sender, MKY.IO.Ports.SerialPortInUseLookupEventArgs e)
		{
			if (this.workspace != null)
			{
				var terminals = this.workspace.Terminals;
				if (terminals != null)
				{
					var inUseLookup = new List<MKY.IO.Ports.InUseInfo>(terminals.Length); // Preset the initial capacity to improve memory management.

					foreach (var t in this.workspace.Terminals)
					{
						var portId = t.IOSerialPortId;
						if (portId != null)
						{
							string inUseText;
							if (t.IsOpen)
								inUseText = "(in use by " + t.IndicatedName + ")";
							else
								inUseText = "(selected by " + t.IndicatedName + ")";

							inUseLookup.Add(new MKY.IO.Ports.InUseInfo(t.SequentialIndex, portId, t.IsOpen, inUseText));
						}
					}

					e.InUseLookup = inUseLookup;
				}
			}
		}

		#endregion

		#region Workspace
		//==========================================================================================
		// Workspace
		//==========================================================================================

		#region Workspace > Lifetime
		//------------------------------------------------------------------------------------------
		// Workspace > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.Saved  += workspace_Saved;
				this.workspace.Closed += workspace_Closed;
			}
		}

		private void DetachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.Saved  -= workspace_Saved;
				this.workspace.Closed -= workspace_Closed;
			}
		}

		#endregion

		#region Workspace > Event Handlers
		//------------------------------------------------------------------------------------------
		// Workspace > Event Handlers
		//------------------------------------------------------------------------------------------

		private void workspace_Saved(object sender, SavedEventArgs e)
		{
			ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath  = e.FilePath;
			ApplicationSettings.LocalUserSettings.AutoWorkspace.AutoSaved = e.IsAutoSave;
			ApplicationSettings.Save();
		}

		/// <remarks>
		/// See remarks of <see cref="YAT.Model.Workspace.Close(bool)"/> for details on why this event handler
		/// needs to treat the Closed event differently in case of a parent (i.e. main) close.
		/// </remarks>
		private void workspace_Closed(object sender, ClosedEventArgs e)
		{
			if (!e.IsParentClose) // In case of workspace intended close, reset workspace info.
			{
				ApplicationSettings.LocalUserSettings.AutoWorkspace.ResetFilePath();
				ApplicationSettings.Save();
			}

			DetachWorkspaceEventHandlers();
			this.workspace = null; // Simply de-reference the workspace, it disposes of itself.
			OnWorkspaceClosed(e);
		}

		#endregion

		#region Workspace > Public Methods
		//------------------------------------------------------------------------------------------
		// Workspace > Public Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool CreateNewWorkspace()
		{
			// Close existing, only one workspace can exist within application:
			if (this.workspace != null)
			{
				if (!this.workspace.Close()) // Note that the reference to the workspace will be
					return (false);          // removed in the 'workspace_Closed' event handler.
			}

			// Create new workspace:
			OnFixedStatusTextRequest("Creating new workspace...");
			OnCursorRequest(Cursors.WaitCursor);

			this.workspace = new Workspace(this.startArgs.ToWorkspaceStartArgs());
			AttachWorkspaceEventHandlers();
			OnWorkspaceOpened(this.workspace);

			OnCursorReset();
			OnTimedStatusTextRequest("New workspace created.");

			return (true);
		}

		/// <summary></summary>
		public virtual bool OpenWorkspaceFromFile(string filePath)
		{
			AssertNotDisposed();

			string fileName = Path.GetFileName(filePath);
			OnFixedStatusTextRequest("Opening workspace " + fileName + "...");
			OnCursorRequest(Cursors.WaitCursor);

			string errorMessage;
			if (OpenWorkspaceFromFile(filePath, out errorMessage))
			{
				OnCursorReset();
				return (true);
			}
			else
			{
				OnCursorReset();
				OnFixedStatusTextRequest("Error opening workspace!");
				OnMessageInputRequest
				(
					errorMessage,
					"Workspace File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
				);
				OnTimedStatusTextRequest("Workspace not opened!");

				return (false);
			}
		}

		/// <summary></summary>
		private bool OpenWorkspaceFromFile(string filePath, out string errorMessage)
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> sh;
			Guid guid;
			Exception ex;
			if (OpenWorkspaceFile(filePath, out sh, out guid, out ex))
			{
				if (OpenWorkspaceFromSettings(sh, guid, out ex))
				{
					errorMessage = null;
					return (true);
				}
				else
				{
					errorMessage = ErrorHelper.ComposeMessage("Unable to open workspace", filePath, ex);
					return (false);
				}
			}
			else
			{
				errorMessage = ErrorHelper.ComposeMessage("Unable to open workspace file", filePath, ex);
				return (false);
			}
		}

		/// <summary></summary>
		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
		{
			Exception exception;
			return (OpenWorkspaceFromSettings(settingsHandler, Guid.NewGuid(), out exception));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, int dynamicTerminalIndexToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
		{
			Exception exception;
			return (OpenWorkspaceFromSettings(settingsHandler, Guid.NewGuid(), dynamicTerminalIndexToReplace, terminalSettingsToReplace, out exception));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid, out Exception exception)
		{
			return (OpenWorkspaceFromSettings(settingsHandler, guid, Indices.InvalidIndex, null, out exception));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure to handle any case.")]
		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings, Guid guid, int dynamicTerminalIndexToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace, out Exception exception)
		{
			AssertNotDisposed();

			// Ensure that the workspace file is not already open:
			if (!CheckWorkspaceFile(settings.SettingsFilePath))
			{
				exception = null;
				return (false);
			}

			// Close workspace, only one workspace can exist within the application:
			if (!CloseWorkspace())
			{
				exception = null;
				return (false);
			}

			// Create new workspace:
			OnFixedStatusTextRequest("Opening workspace...");

			Workspace w;
			try
			{
				w = new Workspace(this.startArgs.ToWorkspaceStartArgs(), settings, guid);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open workspace from settings!");
				exception = ex;
				return (false);
			}

			this.workspace = w;
			AttachWorkspaceEventHandlers();

			// Save auto workspace:
			ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath  = settings.SettingsFilePath;
			ApplicationSettings.LocalUserSettings.AutoWorkspace.AutoSaved = settings.Settings.AutoSaved;
			ApplicationSettings.Save();

			// Save recent:
			if (!settings.Settings.AutoSaved)
				SetRecent(settings.SettingsFilePath);

			OnWorkspaceOpened(this.workspace);
			OnTimedStatusTextRequest("Workspace opened.");

			// Open workspace terminals:
			int terminalCount = this.workspace.OpenTerminals(dynamicTerminalIndexToReplace, terminalSettingsToReplace);
			if (terminalCount == 1)
				OnTimedStatusTextRequest("Workspace terminal opened.");
			else if (terminalCount > 1)
				OnTimedStatusTextRequest("Workspace terminals opened.");
			else
				OnTimedStatusTextRequest("Workspace contains no terminal to open.");

			exception = null;
			return (true);
		}

		/// <summary>
		/// Closes the active workspace.
		/// </summary>
		public virtual bool CloseWorkspace()
		{
			if (this.workspace != null)
				return (this.workspace.Close()); // Note that the reference to the workspace will be
			else                                 // removed in the 'workspace_Closed' event handler.
				return (true);
		}

		#endregion

		#region Workspace > Private Methods
		//------------------------------------------------------------------------------------------
		// Workspace > Private Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Check whether workspace is already open.
		/// </summary>
		private bool CheckWorkspaceFile(string workspaceFilePath)
		{
			if (this.workspace != null)
			{
				if (PathEx.Equals(workspaceFilePath, this.workspace.SettingsFilePath))
				{
					OnFixedStatusTextRequest("Workspace is already open.");
					OnMessageInputRequest
					(
						"Workspace is already open and will not be re-openend.",
						"Workspace Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
					OnTimedStatusTextRequest("Workspace not re-opened.");
					return (false);
				}
			}
			return (true);
		}

		private bool OpenWorkspaceFile(string filePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, out Exception exception)
		{
			Guid guid;
			return (OpenWorkspaceFile(filePath, out settingsHandler, out guid, out exception));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure to handle any case.")]
		private bool OpenWorkspaceFile(string filePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, out Guid guid, out Exception exception)
		{
			try
			{
				var sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
				sh.SettingsFilePath = EnvironmentEx.ResolveAbsolutePath(filePath);
				if (sh.Load())
				{
					settingsHandler = sh;

					// Try to retrieve GUID from file path (in case of auto saved workspace files):
					if (!GuidEx.TryParseTolerantly(Path.GetFileNameWithoutExtension(filePath), out guid))
						guid = Guid.NewGuid();

					exception = null;
					return (true);
				}
				else
				{
					settingsHandler = null;
					guid = Guid.Empty;
					exception = null;
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open workspace file!");

				settingsHandler = null;
				guid = Guid.Empty;
				exception = ex;
				return (false);
			}
		}

		#endregion

		#endregion

		#region Terminal
		//==========================================================================================
		// Terminal
		//==========================================================================================

		/// <summary></summary>
		public virtual bool CreateNewTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			if (this.workspace == null)
			{
				if (!CreateNewWorkspace())
					return (false);
			}

			return (this.workspace.CreateNewTerminal(settingsHandler));
		}

		/// <summary></summary>
		public virtual bool OpenTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			if (this.workspace == null)
			{
				if (!CreateNewWorkspace())
					return (false);
			}

			return (this.workspace.OpenTerminalFromSettings(settingsHandler));
		}

		#region Terminal > Private Methods
		//------------------------------------------------------------------------------------------
		// Terminal > Private Methods
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		private bool OpenTerminalFile(string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			Exception exception;
			return (OpenTerminalFile(terminalFilePath, out settingsHandler, out exception));
		}

		private bool OpenTerminalFile(string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exception)
		{
			return (OpenTerminalFile("", terminalFilePath, out settingsHandler, out exception));
		}

		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			Exception exception;
			return (OpenTerminalFile(workspaceFilePath, terminalFilePath, out settingsHandler, out exception));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure to handle any case.")]
		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exception)
		{
			// Try to combine the workspace path with terminal path, but only if that is a relative path:
			var absoluteTerminalFilePath = PathEx.CombineFilePaths(EnvironmentEx.ResolveAbsolutePath(workspaceFilePath), terminalFilePath);

			// Alternatively, try to use terminal file path only:
			if (string.IsNullOrEmpty(absoluteTerminalFilePath) || !File.Exists(absoluteTerminalFilePath))
				absoluteTerminalFilePath = EnvironmentEx.ResolveAbsolutePath(terminalFilePath);

			try
			{
				var sh = new DocumentSettingsHandler<TerminalSettingsRoot>();
				sh.SettingsFilePath = absoluteTerminalFilePath;
				if (sh.Load())
				{
					settingsHandler = sh;
					exception = null;
					return (true);
				}
				else
				{
					settingsHandler = null;
					exception = null;
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open terminal file!");
				settingsHandler = null;
				exception = ex;
				return (false);
			}
		}

		#endregion

		#endregion

		#region PerformOperation/Exit
		//------------------------------------------------------------------------------------------
		// PerformOperation/Exit
		//------------------------------------------------------------------------------------------

		private void CreateAndStartOperationTimer()
		{
			StopAndDisposeOperationTimer();

			this.operationTimer = new System.Timers.Timer(1000);
			this.operationTimer.Elapsed += this.operationTimer_Elapsed;
			this.operationTimer.Start();
		}

		private void StopAndDisposeOperationTimer()
		{
			if (this.operationTimer != null)
			{
				this.operationTimer.Stop();
				this.operationTimer.Dispose();
				this.operationTimer = null;
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object operationTimer_Elapsed_SyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void operationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(operationTimer_Elapsed_SyncObj))
			{
				try
				{
					if (this.workspace == null)
					{
						StopAndDisposeOperationTimer();

						OnFixedStatusTextRequest("Invalid program operation!");
						OnMessageInputRequest
						(
							MessageHelper.InvalidExecutionPreamble + "workspace is 'null' while attempting to perform the operation!" + Environment.NewLine + Environment.NewLine +
							MessageHelper.SubmitBug,
							"Invalid Program Operation",
							MessageBoxButtons.OK,
							MessageBoxIcon.Stop
						);

						CreateAndStartExitTimerIfNeeded(false);
						return;
					}

					int id = this.startArgs.RequestedDynamicTerminalIndex;
					Terminal requestedTerminal = this.workspace.GetTerminalByDynamicIndex(id);
					if (requestedTerminal == null)
					{
						StopAndDisposeOperationTimer();

						OnFixedStatusTextRequest("Invalid requested dynamic index!");
						OnMessageInputRequest
						(
							"Invalid requested dynamic index " + id.ToString(CultureInfo.CurrentCulture) + " to perform the operation!" + Environment.NewLine + Environment.NewLine +
							"Check the command line arguments. See command line help for details.",
							"Invalid Terminal Index",
							MessageBoxButtons.OK,
							MessageBoxIcon.Stop
						);

						CreateAndStartExitTimerIfNeeded(false);
						return;
					}

					// --- Preconditions fullfilled, workspace and terminal exist. ---

					if (!requestedTerminal.IsStarted)
					{
						OnTimedStatusTextRequest("Operation triggered, pending until terminal has been started...");
						return; // Pend!
					}
					if (!requestedTerminal.IsReadyToSend)
					{
						OnTimedStatusTextRequest("Operation triggered, pending until terminal is ready to transmit...");
						return; // Pend!
					}                                   // Using term 'Transmission' to indicate potential
														// 'intelligence' to send + receive/verify the data.
														// Preconditions fullfilled!
					StopAndDisposeOperationTimer();

					int delay = this.startArgs.OperationDelay;
					if (delay > 0)
					{
						OnFixedStatusTextRequest("Operation triggered, delaying for " + delay.ToString(CultureInfo.CurrentCulture) + " ms...");
						Thread.Sleep(delay);
					}

					OnFixedStatusTextRequest("Operation triggered, preparing...");

					// Automatically transmit text if desired:
					string text = this.startArgs.RequestedTransmitText;
					if (!string.IsNullOrEmpty(text))
					{
						bool success;
						try
						{
							OnFixedStatusTextRequest("Automatically transmitting text on terminal " + id);
							requestedTerminal.SendText(new Command(text)); // No explicit default radix available (yet).
							success = true;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Exception while trying to transmit text!");

							OnFixedStatusTextRequest("Unable to transmit text!");
							OnMessageInputRequest
							(
								ErrorHelper.ComposeMessage("Unable to transmit text", text, ex),
								"Transmission Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Stop
							);
							success = false;
						}
						CreateAndStartExitTimerIfNeeded(success);
					}

					// Automatically transmit file if desired:
					string filePath = this.startArgs.RequestedTransmitFilePath;
					if (!string.IsNullOrEmpty(filePath))
					{
						bool success;
						try
						{
							OnFixedStatusTextRequest("Automatically transmitting file on terminal " + id);
							requestedTerminal.SendFile(new Command("", true, filePath)); // No explicit default radix available (yet).
							success = true;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Exception while trying to transmit file!");

							OnFixedStatusTextRequest("Unable to transmit file!");
							OnMessageInputRequest
							(
								ErrorHelper.ComposeMessage("Unable to transmit file", filePath, ex),
								"Transmission Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Stop
							);
							success = false;
						}
						CreateAndStartExitTimerIfNeeded(success);
					}
				}
				finally
				{
					Monitor.Exit(operationTimer_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
		}

		private void CreateAndStartExitTimerIfNeeded(bool operationSuccess)
		{
			if ((!this.startArgs.KeepOpen) &&
				(!(this.startArgs.KeepOpenOnError && !operationSuccess)))
			{
				if (operationSuccess)
					OnFixedStatusTextRequest("Operation successfully performed, triggering exit...");
				else
					OnFixedStatusTextRequest("No operation performed, triggering exit...");

				StopAndDisposeExitTimer();

				this.exitTimer = new System.Timers.Timer(1000);
				this.exitTimer.Elapsed += this.exitTimer_Elapsed;
				this.exitTimer.Start();
			}
			else
			{
				if (operationSuccess)
					OnTimedStatusTextRequest("Operation successfully performed");
				else
					OnTimedStatusTextRequest("No operation performed");
			}
		}

		private void StopAndDisposeExitTimer()
		{
			if (this.exitTimer != null)
			{
				this.exitTimer.Stop();
				this.exitTimer.Dispose();
				this.exitTimer = null;
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object exitTimer_Elapsed_SyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void exitTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(exitTimer_Elapsed_SyncObj))
			{
				try
				{
					if (this.workspace != null)
					{
						int id = this.startArgs.RequestedDynamicTerminalIndex;
						Terminal terminal = this.workspace.GetTerminalByDynamicIndex(id);
						if ((terminal != null) && (terminal.IsBusy))
						{
							OnTimedStatusTextRequest("Exit triggered, pending while terminal is busy...");
							return; // Pend!
						}
					}

					// --- Precondition fullfilled, terminal is no longer busy. ---

					StopAndDisposeExitTimer();

					int delay = this.startArgs.ExitDelay;
					if (delay > 0)
					{
						OnFixedStatusTextRequest("Exit triggered, delaying for " + delay.ToString(CultureInfo.CurrentCulture) + " ms...");
						Thread.Sleep(delay);
					}

					OnFixedStatusTextRequest("Exit triggered, preparing...");

					try
					{
						OnFixedStatusTextRequest("Automatically exiting " + ApplicationEx.ProductName + "...");
						Exit();
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Exception while trying to exit!");

						OnFixedStatusTextRequest("Unable to exit!");
						OnMessageInputRequest
						(
							ErrorHelper.ComposeMessage("Unable to exit!", ex),
							"Exit Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Stop
						);
					}
				}
				finally
				{
					Monitor.Exit(exitTimer_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnWorkspaceOpened(Workspace workspace)
		{
			this.eventHelper.RaiseSync<EventArgs<Workspace>>(WorkspaceOpened, this, new EventArgs<Workspace>(workspace));
		}

		/// <summary></summary>
		protected virtual void OnWorkspaceClosed(ClosedEventArgs e)
		{
			this.eventHelper.RaiseSync<ClosedEventArgs>(WorkspaceClosed, this, e);
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(FixedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(TimedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			if (this.startArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				MessageInputEventArgs e = new MessageInputEventArgs(text, caption, buttons, icon);
				this.eventHelper.RaiseSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				// Ensure that the request is processed!
				if (e.Result == DialogResult.None)
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + " 'Message Input' request by main was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

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
		protected virtual void OnStarted()
		{
			this.eventHelper.RaiseSync(Started, this, EventArgs.Empty);
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnExited(MainResult result)
		{
			this.eventHelper.RaiseSync<EventArgs<MainResult>>(Exited, this, new EventArgs<MainResult>(result));
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
