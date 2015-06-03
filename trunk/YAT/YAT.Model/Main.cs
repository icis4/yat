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

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

using YAT.Utilities;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Provides the YAT application model which can handle workspaces (.yaw) and terminals (.yat).
	/// </summary>
	public class Main : IDisposable, IGuidProvider
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
		// warnings for each undocumented member below. Documenting each member makes little sense
		// since they pretty much tell their purpose and documentation tags between the members
		// makes the code less readable.
		#pragma warning disable 1591

		/// <summary>
		/// Enumeration of all the main result return codes.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Intentionally using nested type, instead of replicating the parent's name to 'MainResult'.")]
		public enum Result
		{
			Success,
			CommandLineError,
			ApplicationStartError,
			ApplicationRunError,
			ApplicationExitError,
			UnhandledException,
		}

		#pragma warning restore 1591

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private CommandLineArgs commandLineArgs;
		private MainStartArgs startArgs;
		private Guid guid;

		private Workspace workspace;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<WorkspaceEventArgs> WorkspaceOpened;

		/// <summary></summary>
		public event EventHandler WorkspaceClosed;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler Started;

		/// <summary></summary>
		public event EventHandler Exited;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Main()
		{
			Initialize();
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

			Initialize();
		}

		private void Initialize()
		{
			WriteDebugMessageLine("Creating...");

			this.guid = Guid.NewGuid();

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
					// In the 'normal' case, the workspace has already been closed, otherwise...

					// ...first, detach event handlers to ensure that no more events are received...
					DetachWorkspaceEventHandlers();

					// ...then, dispose of objects.
					if (this.workspace != null)
						this.workspace.Dispose();
				}

				// Set state to disposed:
				this.workspace = null;
				this.isDisposed = true;

				WriteDebugMessageLine("...successfully disposed.");
			}
		}

		/// <summary></summary>
		~Main()
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
		public virtual MainStartArgs StartArgs
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.startArgs);
			}
		}

		/// <summary>
		/// This is the automatically assigned workspace name. The name is corresponding to the
		/// name of the currently active terminal.
		/// </summary>
		public virtual string AutoName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.workspace != null)
					return (this.workspace.AutoName);
				else
					return (ApplicationInfo.ProductName);
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

		#endregion

		#region Start
		//==========================================================================================
		// Start
		//==========================================================================================

		/// <summary>
		/// This method is used to test the command line argument processing.
		/// </summary>
		public virtual Result PrepareStart()
		{
			AssertNotDisposed();

			// Process command line args into start requests:
			if (ProcessCommandLineArgsIntoStartRequests())
				return (Result.Success);
			else
				return (Result.CommandLineError);
		}

		/// <summary>
		/// If a file was requested by command line argument, this method tries to open the
		/// requested file.
		/// Else, this method tries to open the most recent workspace of the current user.
		/// If still unsuccessful, a new workspace is created.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if either operation succeeded, <c>false</c> otherwise.
		/// </returns>
		public virtual Result Start()
		{
			AssertNotDisposed();

			// Process command line args into start requests:
			if (!ProcessCommandLineArgsIntoStartRequests())
				return (Result.CommandLineError);

			// Start YAT according to the start requests:
			bool success = false;

			bool workspaceIsRequested = (this.startArgs.WorkspaceSettings != null);
			bool terminalIsRequested  = (this.startArgs.TerminalSettings  != null);

			if (workspaceIsRequested || terminalIsRequested)
			{
				if (workspaceIsRequested && terminalIsRequested)
				{
					success = OpenWorkspaceFromSettings(this.startArgs.WorkspaceSettings, this.startArgs.RequestedDynamicTerminalIndex, this.startArgs.TerminalSettings);
				}
				else if (workspaceIsRequested) // Workspace only.
				{
					success = OpenWorkspaceFromSettings(this.startArgs.WorkspaceSettings);
				}
				else // Terminal only.
				{
					success = OpenTerminalFromSettings(this.startArgs.TerminalSettings);
				}

				// Note that any existing auto workspace settings are kept as they are.
				// Thus, they can be used again at the next 'normal' start of YAT.
			}

			if (!success && ApplicationSettings.LocalUserSettingsAreCurrentlyOwnedByThisInstance)
			{
				if (ApplicationSettings.LocalUserSettings.General.AutoOpenWorkspace)
				{
					string filePath = ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath;
					if (PathEx.IsValid(filePath))
					{
						if (File.Exists(filePath))
							success = OpenWorkspaceFromFile(filePath);

						if (!success)
						{
							OnFixedStatusTextRequest("Error opening workspace!");

							StringBuilder sb = new StringBuilder();
							sb.AppendLine("Unable to open workspace");
							sb.AppendLine(filePath);
							sb.AppendLine();
							sb.AppendLine("A new empty workspace will be created.");

							OnMessageInputRequest
							(
								sb.ToString(),
								"Workspace File Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Stop
							);

							// Do not yet create a new empty workspace, it will be created when
							// needed. Not creating it now allows the user to exit YAT, restore
							// the .yaw file, and try again.
							return (Main.Result.ApplicationStartError);
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

			// Start the workspace, i.e. start all included terminals:
			if (success)
				success = this.workspace.Start();

			if (success)
			{
				OnStarted(EventArgs.Empty);
				return (Result.Success);
			}
			else
			{
				return (Result.ApplicationStartError);
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
				// This is the standard case, the 'New Terminal' dialog will get shown.
				if (this.commandLineArgs == null)
				{
					this.startArgs.ShowNewTerminalDialog = true;
					this.startArgs.KeepOpen              = true;
					this.startArgs.KeepOpenOnError       = true;

					return (true);
				}
				// In case of "YATConsole.exe" the controller will set the 'NonInteractive'
				// option and no 'New Terminal' dialog shall get shown. The behavior will be
				// the same as with the 'Empty' option.
				else
				{
					this.startArgs.ShowNewTerminalDialog = this.commandLineArgs.Interactive;
					this.startArgs.KeepOpen              = true;
					this.startArgs.KeepOpenOnError       = true;

					return (true);
				}
			}

			// Prio 1 = Invalid:
			if ((this.commandLineArgs != null) && (!this.commandLineArgs.IsValid))
			{
				return (false);
			}

			// Arguments are available and valid, transfer 'NonInteractive' option:
			this.startArgs.NonInteractive = this.commandLineArgs.NonInteractive;

			// Prio 2 = Empty:
			if (this.commandLineArgs.Empty)
			{
				this.startArgs.ShowNewTerminalDialog = false;
				this.startArgs.KeepOpen              = true;
				this.startArgs.KeepOpenOnError       = true;

				return (true);
			}

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
				if (ExtensionSettings.IsWorkspaceFile(requestedFilePath))
				{
					DocumentSettingsHandler<WorkspaceSettingsRoot> sh;
					if (OpenWorkspaceFile(requestedFilePath, out sh))
						this.startArgs.WorkspaceSettings = sh;
					else
						return (false);
				}
				else if (ExtensionSettings.IsTerminalFile(requestedFilePath))
				{
					DocumentSettingsHandler<TerminalSettingsRoot> sh;
					if (OpenTerminalFile(requestedFilePath, out sh))
						this.startArgs.TerminalSettings = sh;
					else
						return (false);
				}
				else
				{
					return (false);
				}
			}

			// Prio 7 = Retrieve the requested terminal and validate it:
			if (this.startArgs.WorkspaceSettings != null) // Applies to a terminal within a workspace.
			{
				int requestedDynamicTerminalIndex = this.commandLineArgs.RequestedDynamicTerminalIndex;
				int lastDynamicIndex = Indices.IndexToDynamicIndex(this.startArgs.WorkspaceSettings.Settings.TerminalSettings.Count - 1);
				
				if     ((requestedDynamicTerminalIndex >= Indices.FirstDynamicIndex) && (requestedDynamicTerminalIndex <= lastDynamicIndex))
					this.startArgs.RequestedDynamicTerminalIndex = requestedDynamicTerminalIndex;
				else if (requestedDynamicTerminalIndex == Indices.DefaultDynamicIndex)
					this.startArgs.RequestedDynamicTerminalIndex = lastDynamicIndex;
				else if (requestedDynamicTerminalIndex == Indices.InvalidDynamicIndex)
					this.startArgs.RequestedDynamicTerminalIndex = Indices.InvalidDynamicIndex;
				else
					return (false);

				if (this.startArgs.RequestedDynamicTerminalIndex != Indices.InvalidDynamicIndex)
				{
					DocumentSettingsHandler<TerminalSettingsRoot> sh;
					string workspaceFilePath = this.startArgs.WorkspaceSettings.SettingsFilePath;
					string terminalFilePath = this.startArgs.WorkspaceSettings.Settings.TerminalSettings[Indices.DynamicIndexToIndex(this.startArgs.RequestedDynamicTerminalIndex)].FilePath;
					if (OpenTerminalFile(workspaceFilePath, terminalFilePath, out sh))
						this.startArgs.TerminalSettings = sh;
					else
						return (false);
				}
			}
			else if (this.startArgs.TerminalSettings != null) // Applies to a dedicated terminal.
			{
				switch (this.commandLineArgs.RequestedDynamicTerminalIndex)
				{
					case Indices.InvalidDynamicIndex:
						this.startArgs.RequestedDynamicTerminalIndex = Indices.InvalidDynamicIndex;
						break;

					case Indices.DefaultDynamicIndex:
					case Indices.FirstDynamicIndex:
						this.startArgs.RequestedDynamicTerminalIndex = Indices.FirstDynamicIndex;
						break;

					default:
						return (false);
				}
			}
			else if (this.commandLineArgs.NewIsRequested) // Applies to new settings.
			{
				this.startArgs.TerminalSettings = new DocumentSettingsHandler<TerminalSettingsRoot>();
			}

			// Prio 8 = Override settings as desired:
			if (this.startArgs.TerminalSettings != null)
			{
				bool terminalIsStarted = this.startArgs.TerminalSettings.Settings.TerminalIsStarted;
				bool logIsStarted      = this.startArgs.TerminalSettings.Settings.LogIsStarted;

				if (ProcessCommandLineArgsIntoExistingTerminalSettings(this.startArgs.TerminalSettings.Settings.Terminal, ref terminalIsStarted, ref logIsStarted))
				{
					this.startArgs.TerminalSettings.Settings.TerminalIsStarted = terminalIsStarted;
					this.startArgs.TerminalSettings.Settings.LogIsStarted      = logIsStarted;
				}
				else
				{
					return (false);
				}
			}

			// Prio 9 = Perform requested operation:
			if (this.commandLineArgs.OptionIsGiven("TransmitFile"))
			{
				this.startArgs.RequestedTransmitFilePath = this.commandLineArgs.RequestedTransmitFilePath;
				this.startArgs.PerformOperationOnRequestedTerminal = true;
			}

			// Prio 10 = Set behavior:
			if (this.startArgs.PerformOperationOnRequestedTerminal)
			{
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
		private bool ProcessCommandLineArgsIntoExistingTerminalSettings(Domain.Settings.TerminalSettings terminalSettings, ref bool terminalIsStarted, ref bool logIsStarted)
		{
			if (this.commandLineArgs.OptionIsGiven("TerminalType"))
			{
				Domain.TerminalTypeEx terminalType;
				if (Domain.TerminalTypeEx.TryParse(this.commandLineArgs.TerminalType, out terminalType))
					terminalSettings.TerminalType = terminalType;
				else
					return (false);
			}

			if (this.commandLineArgs.OptionIsGiven("PortType"))
			{
				Domain.IOTypeEx ioType;
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
					MKY.IO.Ports.DataBitsEx dataBits;
					if (MKY.IO.Ports.DataBitsEx.TryFrom(this.commandLineArgs.DataBits, out dataBits))
						terminalSettings.IO.SerialPort.Communication.DataBits = dataBits;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("Parity"))
				{
					MKY.IO.Ports.ParityEx parity;
					if (MKY.IO.Ports.ParityEx.TryParse(this.commandLineArgs.Parity, out parity))
						terminalSettings.IO.SerialPort.Communication.Parity = parity;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("StopBits"))
				{
					MKY.IO.Ports.StopBitsEx stopBits;
					if (MKY.IO.Ports.StopBitsEx.TryFrom(this.commandLineArgs.StopBits, out stopBits))
						terminalSettings.IO.SerialPort.Communication.StopBits = stopBits;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("FlowControl"))
				{
					MKY.IO.Serial.SerialPort.SerialFlowControlEx flowControl;
					if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryParse(this.commandLineArgs.FlowControl, out flowControl))
						terminalSettings.IO.SerialPort.Communication.FlowControl = flowControl;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("SerialPortAutoReopen"))
				{
					if (this.commandLineArgs.SerialPortAutoReopen == 0)
						terminalSettings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.AutoRetry(false, 0);
					else if (this.commandLineArgs.SerialPortAutoReopen >= MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenMinimumInterval)
						terminalSettings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.AutoRetry(true, this.commandLineArgs.SerialPortAutoReopen);
					else
						return (false);
				}
			}
			else if ((finalIOType == Domain.IOType.TcpClient) ||
					 (finalIOType == Domain.IOType.TcpServer) ||
					 (finalIOType == Domain.IOType.TcpAutoSocket) ||
					 (finalIOType == Domain.IOType.Udp))
			{
				if (this.commandLineArgs.OptionIsGiven("RemoteHost"))
				{
					MKY.Net.IPHost remoteHost;
					if (MKY.Net.IPHost.TryParse(this.commandLineArgs.RemoteHost, out remoteHost))
						terminalSettings.IO.Socket.RemoteHost = remoteHost;
					else
						return (false);
				}
				if (((finalIOType == Domain.IOType.TcpClient) ||
					 (finalIOType == Domain.IOType.TcpAutoSocket) ||
					 (finalIOType == Domain.IOType.Udp)) &&
					this.commandLineArgs.OptionIsGiven("RemotePort"))
				{
					if (Int32Ex.IsWithin(this.commandLineArgs.RemotePort, System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MaxPort))
						terminalSettings.IO.Socket.RemotePort = this.commandLineArgs.RemotePort;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("LocalInterface"))
				{
					MKY.Net.IPNetworkInterface localInterface;
					if (MKY.Net.IPNetworkInterface.TryParse(this.commandLineArgs.LocalInterface, out localInterface))
						terminalSettings.IO.Socket.LocalInterface = localInterface;
					else
						return (false);
				}
				if (((finalIOType == Domain.IOType.TcpServer) ||
					 (finalIOType == Domain.IOType.TcpAutoSocket) ||
					 (finalIOType == Domain.IOType.Udp)) &&
					this.commandLineArgs.OptionIsGiven("LocalPort"))
				{
					if (Int32Ex.IsWithin(this.commandLineArgs.LocalPort, System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MaxPort))
						terminalSettings.IO.Socket.LocalPort = this.commandLineArgs.LocalPort;
					else
						return (false);
				}
				if ((finalIOType == Domain.IOType.TcpClient) &&
					this.commandLineArgs.OptionIsGiven("TCPAutoReconnect"))
				{
					if (this.commandLineArgs.TcpAutoReconnect == 0)
						terminalSettings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.AutoRetry(false, 0);
					else if (this.commandLineArgs.TcpAutoReconnect >= MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectMinimumInterval)
						terminalSettings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.AutoRetry(true, this.commandLineArgs.TcpAutoReconnect);
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

						if (!Int32Ex.IsWithin(vendorId, MKY.IO.Usb.DeviceInfo.FirstVendorId, MKY.IO.Usb.DeviceInfo.LastVendorId))
							return (false);

						if (!int.TryParse(this.commandLineArgs.ProductId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out productId))
							return (false);

						if (!Int32Ex.IsWithin(productId, MKY.IO.Usb.DeviceInfo.FirstProductId, MKY.IO.Usb.DeviceInfo.LastProductId))
							return (false);

						// The SNR is optional:
						if (!this.commandLineArgs.OptionIsGiven("SerialString"))
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.DeviceInfo(vendorId, productId);
						else
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.DeviceInfo(vendorId, productId, this.commandLineArgs.SerialString);
					}
					else
					{
						return (false);
					}
				}
				if (this.commandLineArgs.OptionIsGiven("FormatPreset"))
				{
					terminalSettings.IO.UsbSerialHidDevice.ReportFormat = (MKY.IO.Usb.SerialHidReportFormatPresetEx)this.commandLineArgs.FormatPreset;
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

			if (this.commandLineArgs.OptionIsGiven("BeginLog"))
				logIsStarted = this.commandLineArgs.BeginLog;

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
		public bool ProcessCommandLineArgsIntoExistingNewTerminalSettings(Model.Settings.NewTerminalSettings newTerminalSettings)
		{
			// These are temporary settings. Therefore, child items of these settings are not
			// cloned below. They can simply be assigned and will then later be assigned back.
			Domain.Settings.TerminalSettings terminalSettings = new Domain.Settings.TerminalSettings();

			terminalSettings.TerminalType                       = newTerminalSettings.TerminalType;
			terminalSettings.IO.IOType                          = newTerminalSettings.IOType;

			terminalSettings.IO.SerialPort.PortId               = newTerminalSettings.SerialPortId;
			terminalSettings.IO.SerialPort.Communication        = newTerminalSettings.SerialPortCommunication;
			terminalSettings.IO.SerialPort.AutoReopen           = newTerminalSettings.SerialPortAutoReopen;

			terminalSettings.IO.Socket.RemoteHost               = newTerminalSettings.SocketRemoteHost;
			terminalSettings.IO.Socket.RemoteTcpPort            = newTerminalSettings.SocketRemoteTcpPort;
			terminalSettings.IO.Socket.RemoteUdpPort            = newTerminalSettings.SocketRemoteUdpPort;
			terminalSettings.IO.Socket.LocalInterface           = newTerminalSettings.SocketLocalInterface;
			terminalSettings.IO.Socket.LocalTcpPort             = newTerminalSettings.SocketLocalTcpPort;
			terminalSettings.IO.Socket.LocalUdpPort             = newTerminalSettings.SocketLocalUdpPort;
			terminalSettings.IO.Socket.TcpClientAutoReconnect   = newTerminalSettings.TcpClientAutoReconnect;

			terminalSettings.IO.UsbSerialHidDevice.DeviceInfo   = newTerminalSettings.UsbSerialHidDeviceInfo;
			terminalSettings.IO.UsbSerialHidDevice.ReportFormat = newTerminalSettings.UsbSerialHidReportFormat;
			terminalSettings.IO.UsbSerialHidDevice.RxIdUsage    = newTerminalSettings.UsbSerialHidRxIdUsage;
			terminalSettings.IO.UsbSerialHidDevice.AutoOpen     = newTerminalSettings.UsbSerialHidAutoOpen;

			bool terminalIsStarted = newTerminalSettings.StartTerminal;
			bool logIsStarted      = false; // Doesn't matter, new terminal settings do not have this option.

			if (ProcessCommandLineArgsIntoExistingTerminalSettings(terminalSettings, ref terminalIsStarted, ref logIsStarted))
			{
				newTerminalSettings.TerminalType             = terminalSettings.TerminalType;
				newTerminalSettings.IOType                   = terminalSettings.IO.IOType;

				newTerminalSettings.SerialPortId             = terminalSettings.IO.SerialPort.PortId;
				newTerminalSettings.SerialPortCommunication  = terminalSettings.IO.SerialPort.Communication;
				newTerminalSettings.SerialPortAutoReopen     = terminalSettings.IO.SerialPort.AutoReopen;

				newTerminalSettings.SocketRemoteHost         = terminalSettings.IO.Socket.RemoteHost;
				newTerminalSettings.SocketRemoteTcpPort      = terminalSettings.IO.Socket.RemoteTcpPort;
				newTerminalSettings.SocketRemoteUdpPort      = terminalSettings.IO.Socket.RemoteUdpPort;
				newTerminalSettings.SocketLocalInterface     = terminalSettings.IO.Socket.LocalInterface;
				newTerminalSettings.SocketLocalTcpPort       = terminalSettings.IO.Socket.LocalTcpPort;
				newTerminalSettings.SocketLocalUdpPort       = terminalSettings.IO.Socket.LocalUdpPort;
				newTerminalSettings.TcpClientAutoReconnect   = terminalSettings.IO.Socket.TcpClientAutoReconnect;

				newTerminalSettings.UsbSerialHidDeviceInfo   = terminalSettings.IO.UsbSerialHidDevice.DeviceInfo;
				newTerminalSettings.UsbSerialHidReportFormat = terminalSettings.IO.UsbSerialHidDevice.ReportFormat;
				newTerminalSettings.UsbSerialHidRxIdUsage    = terminalSettings.IO.UsbSerialHidDevice.RxIdUsage;
				newTerminalSettings.UsbSerialHidAutoOpen     = terminalSettings.IO.UsbSerialHidDevice.AutoOpen;

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
			// Get all file paths in default directory.
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

			// Ensure to leave application settings untouched.
			localUserDirectoryFilePaths.Remove(ApplicationSettings.LocalUserSettingsFilePath);

			// Get all active file paths.
			List<string> activeFilePaths = new List<string>();
			if (this.workspace != null)
			{
				// Add workspace settings file.
				if (this.workspace.SettingsFileExists)
					activeFilePaths.Add(this.workspace.SettingsFilePath);

				// Add terminal settings files.
				activeFilePaths.AddRange(this.workspace.TerminalSettingsFilePaths);
			}

			// Ensure to leave all active settings untouched.
			foreach (string afp in activeFilePaths)
			{
				localUserDirectoryFilePaths.Remove(afp);
			}

			// Delete all obsolete file paths in default directory.
			foreach (string ddfp in localUserDirectoryFilePaths)
			{
				FileEx.TryDelete(ddfp);
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
		/// <returns><c>true</c> if successfully opened the workspace or terminal.</returns>
		public virtual bool OpenFromFile(string filePath)
		{
			AssertNotDisposed();

			string extension = Path.GetExtension(filePath);
			if (ExtensionSettings.IsWorkspaceFile(extension))
			{
				if (OpenWorkspaceFromFile(filePath))
				{
					if (this.workspace.Start())
					{
						OnStarted(EventArgs.Empty);
						return (true);
					}
					return (false);
				}
				else
				{
					return (false);
				}
			}
			else if (ExtensionSettings.IsTerminalFile(extension))
			{
				// Create workspace if it doesn't exist yet.
				bool newWorkspaceSoSignalStarted = false;
				if (this.workspace == null)
				{
					if (!CreateNewWorkspace())
						return (false);

					newWorkspaceSoSignalStarted = true;
				}

				if (this.workspace.OpenTerminalFromFile(filePath))
				{
					if (this.workspace.ActiveTerminal.Start())
					{
						if (newWorkspaceSoSignalStarted)
							OnStarted(EventArgs.Empty);
						
						return (true);
					}
					return (false);
				}
				else
				{
					return (false);
				}
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

		#region Recents
		//==========================================================================================
		// Recents
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
		public virtual Result Exit()
		{
			bool cancel;
			return (Exit(out cancel));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Exit", Justification = "Exit() as method name is the obvious name and should be OK for other languages, .NET itself uses it in Application.Exit().")]
		public virtual Result Exit(out bool cancel)
		{
			bool success;
			if (this.workspace != null)
				success = this.workspace.Close(true);
			else
				success = true;

			if (success)
			{
				OnFixedStatusTextRequest("Exiting " + ApplicationInfo.ProductName + "...");
				cancel = false;

				// Close the static application settings:
				success = ApplicationSettings.Close();

				// Signal the exit:
				OnExited(EventArgs.Empty);

				// Ensure that all resources of the workspace get disposed of:
				Dispose();

				if (success)
					return (Result.Success);
				else
					return (Result.ApplicationExitError);
			}
			else
			{
				OnTimedStatusTextRequest("Exit cancelled.");
				cancel = true;
				return (Result.ApplicationExitError);
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
				this.workspace.Saved  += new EventHandler<SavedEventArgs> (workspace_Saved);
				this.workspace.Closed += new EventHandler<ClosedEventArgs>(workspace_Closed);
			}
		}

		private void DetachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.Saved  -= new EventHandler<SavedEventArgs> (workspace_Saved);
				this.workspace.Closed -= new EventHandler<ClosedEventArgs>(workspace_Closed);
			}
		}

		#endregion

		#region Workspace > Event Handlers
		//------------------------------------------------------------------------------------------
		// Workspace > Event Handlers
		//------------------------------------------------------------------------------------------

		private void workspace_Saved(object sender, SavedEventArgs e)
		{
			ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath = e.FilePath;
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
			this.workspace = null;

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
			// Close workspace, only one workspace can exist within application:
			if (this.workspace != null)
			{
				if (!this.workspace.Close())
					return (false);
			}

			OnFixedStatusTextRequest("Creating new workspace...");

			// Create workspace:
			this.workspace = new Workspace(this.startArgs.ToWorkspaceStartArgs());
			AttachWorkspaceEventHandlers();
			OnWorkspaceOpened(new WorkspaceEventArgs(this.workspace));

			OnTimedStatusTextRequest("New workspace created.");
			return (true);
		}

		/// <summary></summary>
		public virtual bool OpenWorkspaceFromFile(string filePath)
		{
			AssertNotDisposed();

			string fileName = Path.GetFileName(filePath);
			OnFixedStatusTextRequest("Opening workspace " + fileName + "...");

			DocumentSettingsHandler<WorkspaceSettingsRoot> settings;
			Guid guid;
			System.Xml.XmlException ex;
			if (OpenWorkspaceFile(filePath, out settings, out guid, out ex))
			{
				return (OpenWorkspaceFromSettings(settings, guid));
			}
			else
			{
				OnFixedStatusTextRequest("Error opening workspace!");
				if (ex is System.Xml.XmlException)
				{
					OnMessageInputRequest
					(
						"Unable to open file" + Environment.NewLine + filePath   + Environment.NewLine + Environment.NewLine +
						"XML error message:"  + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message:" + Environment.NewLine + ex.InnerException.Message,
						"Invalid Workspace File",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop
					);
				}
				else if (ex is Exception)
				{
					OnMessageInputRequest
					(
						"Unable to open file"   + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
						"System error message:" + Environment.NewLine + ex.Message,
						"Invalid Workspace File",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop
					);
				}
				else
				{
					OnMessageInputRequest
					(
						"Unable to open file" + Environment.NewLine + filePath,
						"Invalid Workspace File",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop
					);
				}
				OnTimedStatusTextRequest("No workspace opened!");
				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings)
		{
			return (OpenWorkspaceFromSettings(settings, Guid.NewGuid()));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings, Guid guid)
		{
			return (OpenWorkspaceFromSettings(settings, guid, Indices.InvalidIndex, null));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings, int dynamicTerminalIndexToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
		{
			return (OpenWorkspaceFromSettings(settings, Guid.NewGuid(), dynamicTerminalIndexToReplace, terminalSettingsToReplace));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings, Guid guid, int dynamicTerminalIndexToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
		{
			AssertNotDisposed();

			// Ensure that the workspace file is not already open.
			if (!CheckWorkspaceFile(settings.SettingsFilePath))
				return (false);

			// Close workspace, only one workspace can exist within the application.
			if (!CloseWorkspace())
				return (false);

			// Create new workspace.
			OnFixedStatusTextRequest("Opening workspace...");
			this.workspace = new Workspace(this.startArgs.ToWorkspaceStartArgs(), settings, guid);
			AttachWorkspaceEventHandlers();

			// Save auto workspace.
			ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath = settings.SettingsFilePath;
			ApplicationSettings.Save();

			// Save recent.
			if (!settings.Settings.AutoSaved)
				SetRecent(settings.SettingsFilePath);

			OnWorkspaceOpened(new WorkspaceEventArgs(this.workspace));
			OnTimedStatusTextRequest("Workspace opened.");

			// Open workspace terminals.
			int terminalCount = this.workspace.OpenTerminals(dynamicTerminalIndexToReplace, terminalSettingsToReplace);
			if (terminalCount == 1)
				OnTimedStatusTextRequest("Workspace terminal opened.");
			else if (terminalCount > 1)
				OnTimedStatusTextRequest("Workspace terminals opened.");
			else
				OnTimedStatusTextRequest("Workspace contains no terminal to open.");

			return (true);
		}

		/// <summary>
		/// Closes the active workspace.
		/// </summary>
		public virtual bool CloseWorkspace()
		{
			if (this.workspace != null)
				return (this.workspace.Close());
			else
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

		private bool OpenWorkspaceFile(string filePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settings)
		{
			Guid guid;
			System.Xml.XmlException exception;
			return (OpenWorkspaceFile(filePath, out settings, out guid, out exception));
		}

		private bool OpenWorkspaceFile(string filePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settings, out Guid guid, out System.Xml.XmlException exception)
		{
			try
			{
				DocumentSettingsHandler<WorkspaceSettingsRoot> s = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
				s.SettingsFilePath = filePath;
				if (s.Load())
				{
					settings = s;

					// Try to retrieve GUID from file path (in case of auto saved workspace files).
					if (!GuidEx.TryCreateGuidFromFilePath(filePath, GeneralSettings.AutoSaveWorkspaceFileNamePrefix, out guid))
						guid = Guid.NewGuid();

					exception = null;
					return (true);
				}
				else
				{
					settings = null;
					guid = Guid.Empty;
					exception = null;
					return (false);
				}
			}
			catch (System.Xml.XmlException ex)
			{
				DebugEx.WriteException(GetType(), ex);
				settings = null;
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
				CreateNewWorkspace();

			return (this.workspace.CreateNewTerminal(settingsHandler));
		}

		/// <summary></summary>
		public virtual bool OpenTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			if (this.workspace == null)
				CreateNewWorkspace();

			return (this.workspace.OpenTerminalFromSettings(settingsHandler));
		}

		#region Terminal > Private Methods
		//------------------------------------------------------------------------------------------
		// Terminal > Private Methods
		//------------------------------------------------------------------------------------------

		private bool OpenTerminalFile(string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settings)
		{
			System.Xml.XmlException exception;
			return (OpenTerminalFile("", terminalFilePath, out settings, out exception));
		}

		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settings)
		{
			System.Xml.XmlException exception;
			return (OpenTerminalFile(workspaceFilePath, terminalFilePath, out settings, out exception));
		}

		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settings, out System.Xml.XmlException exception)
		{
			// Combine absolute workspace path with terminal path if that one is relative.
			terminalFilePath = PathEx.CombineFilePaths(workspaceFilePath, terminalFilePath);

			try
			{
				DocumentSettingsHandler<TerminalSettingsRoot> s = new DocumentSettingsHandler<TerminalSettingsRoot>();
				s.SettingsFilePath = terminalFilePath;
				if (s.Load())
				{
					settings = s;
					exception = null;
					return (true);
				}
				else
				{
					settings = null;
					exception = null;
					return (false);
				}
			}
			catch (System.Xml.XmlException ex)
			{
				DebugEx.WriteException(GetType(), ex);
				settings = null;
				exception = ex;
				return (false);
			}
		}

		#endregion

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnWorkspaceOpened(WorkspaceEventArgs e)
		{
			EventHelper.FireSync<WorkspaceEventArgs>(WorkspaceOpened, this, e);
		}

		/// <summary></summary>
		protected virtual void OnWorkspaceClosed(EventArgs e)
		{
			EventHelper.FireSync(WorkspaceClosed, this, e);
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
			MessageInputEventArgs e = new MessageInputEventArgs(text, caption, buttons, icon);
			EventHelper.FireSync<MessageInputEventArgs>(MessageInputRequest, this, e);

			// Ensure that the request is processed!
			if (e.Result == DialogResult.None)
				throw (new InvalidOperationException("A 'Message Input' request by main was not processed by the application!"));

			return (e.Result);
		}

		/// <summary></summary>
		protected virtual void OnStarted(EventArgs e)
		{
			EventHelper.FireSync(Started, this, e);
		}

		/// <summary></summary>
		protected virtual void OnExited(EventArgs e)
		{
			EventHelper.FireSync(Exited, this, e);
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
