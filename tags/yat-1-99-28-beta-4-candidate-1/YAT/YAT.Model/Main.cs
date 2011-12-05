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
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.Event;
using MKY.IO;
using MKY.Settings;

using YAT.Model.Settings;
using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;
using YAT.Utilities;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Provides the YAT application model. This model can handle terminals (.yat),
	/// workspaces (.yaw) and logs.
	/// </summary>
	public class Main : IDisposable, IGuidProvider
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		/// <summary>
		/// Keep track of how many instances of main are running, needed to check for multiple
		/// instances in <see cref="Start()"/>.
		/// </summary>
		private static int staticInstancesRunning = 0;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Guid guid;

		private CommandLineArgs commandLineArgs;
		private StartArgs startArgs;

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
		{
			this.commandLineArgs = new CommandLineArgs(new string[] { requestedFilePath });
			Initialize();
		}

		/// <summary></summary>
		public Main(CommandLineArgs commandLineArgs)
		{
			this.commandLineArgs = commandLineArgs;
			Initialize();
		}

		private void Initialize()
		{
			this.guid = Guid.NewGuid();
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
				if (disposing)
				{
					// First, detach event handlers to ensure that no more events are received
					DetachWorkspaceEventHandlers();

					// Then, dispose of objects
					if (this.workspace != null)
					{
						this.workspace.Dispose();
						this.workspace = null;
					}
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Main()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
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
				AssertNotDisposed();
				return (this.guid);
			}
		}

		/// <summary></summary>
		public virtual StartArgs StartArgs
		{
			get
			{
				AssertNotDisposed();
				return (this.startArgs);
			}
		}

		/// <summary></summary>
		public virtual string UserName
		{
			get
			{
				AssertNotDisposed();

				if (this.workspace != null)
					return (this.workspace.UserName);
				else
					return (Application.ProductName);
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
		/// Returns <c>true</c> if either operation succeeded, <c>false</c> otherwise.
		/// </returns>
		public virtual MainResult Start()
		{
			AssertNotDisposed();

			// Process command line args into start requests:
			if (!ProcessCommandLineArgsIntoStartRequests())
				return (MainResult.CommandLineError);

			// Start YAT according to the start requests:
			bool success = false;
			bool otherInstanceIsAlreadyRunning = OtherInstanceIsAlreadyRunning();

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

				if (success)
				{
					// Reset workspace file path to ensure that command line files are not used on
					// next 'normal' start:
					ApplicationSettings.LocalUser.AutoWorkspace.ResetFilePathAndUser();
					ApplicationSettings.Save();

					// Clean up all default workspaces/terminals since they're not needed anymore:
					if (!otherInstanceIsAlreadyRunning)
						CleanupLocalUserDirectory();
				}
			}

			if (!success && ApplicationSettings.LocalUser.General.AutoOpenWorkspace)
			{
				// Make sure that auto workspace is not already in use by another instance of YAT:
				if (ApplicationSettings.LocalUser.AutoWorkspace.FilePathUser == Guid.Empty)
				{
					string filePath = ApplicationSettings.LocalUser.AutoWorkspace.FilePath;
					if (File.Exists(filePath))
						success = OpenWorkspaceFromFile(filePath);
				}
			}

			// Clean up the default directory:
			if (!otherInstanceIsAlreadyRunning)
				CleanupLocalUserDirectory();

			// If no success so far, create a new empty workspace:
			if (!success)
			{
				// Reset workspace file path:
				ApplicationSettings.LocalUser.AutoWorkspace.ResetFilePathAndUser();
				ApplicationSettings.Save();

				success = CreateNewWorkspace();
			}

			// Update number of running instances:
			if (success)
				staticInstancesRunning++;

			if (success)
				return (MainResult.Success);
			else
				return (MainResult.ApplicationStartError);
		}

		#region Start > Private Methods
		//------------------------------------------------------------------------------------------
		// Start > Private Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Process the command line arguments according to their priority and translate them into
		/// start requests.
		/// </summary>
		private bool ProcessCommandLineArgsIntoStartRequests()
		{
			// Always create start requests to ensure that object exists.
			this.startArgs = new StartArgs();

			// Prio 0 = Invalid:
			if ((this.commandLineArgs != null) && (!this.commandLineArgs.IsValid))
			{
				return (false);
			}

			// Prio 1 = None:
			if (this.commandLineArgs == null || this.commandLineArgs.NoArgs)
			{
				this.startArgs.ShowNewTerminalDialog = true;
				this.startArgs.KeepOpen              = true;
				this.startArgs.KeepOpenOnError       = true;

				return (true);
			}

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
				if (ExtensionSettings.IsWorkspaceFile(Path.GetExtension(requestedFilePath)))
				{
					DocumentSettingsHandler<WorkspaceSettingsRoot> sh;
					if (OpenWorkspaceFile(requestedFilePath, out sh))
						this.startArgs.WorkspaceSettings = sh;
					else
						return (false);
				}
				else if (ExtensionSettings.IsTerminalFile(Path.GetExtension(requestedFilePath)))
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

			// Prio 8 = If no settings loaded so far, create a new terminal anyway:
			if ((this.startArgs.WorkspaceSettings == null) && (this.startArgs.TerminalSettings == null))
				 this.startArgs.TerminalSettings = new DocumentSettingsHandler<TerminalSettingsRoot>();

			// Prio 9 = Override settings as desired:
			if (this.commandLineArgs.OptionIsGiven("TerminalType"))
			{
				Domain.TerminalTypeEx terminalType;
				if (Domain.TerminalTypeEx.TryParse(this.commandLineArgs.TerminalType, out terminalType))
					this.startArgs.TerminalSettings.Settings.TerminalType = terminalType;
				else
					return (false);
			}
			if (this.commandLineArgs.OptionIsGiven("PortType"))
			{
				Domain.IOTypeEx ioType;
				if (Domain.IOTypeEx.TryParse(this.commandLineArgs.IOType, out ioType))
					this.startArgs.TerminalSettings.Settings.IOType = ioType;
				else
					return (false);
			}

			if (this.startArgs.TerminalSettings != null)
			{
				Domain.IOType finalIOType = this.startArgs.TerminalSettings.Settings.IOType;
				if (finalIOType == Domain.IOType.SerialPort)
				{
					if (this.commandLineArgs.OptionIsGiven("SerialPort"))
					{
						MKY.IO.Ports.SerialPortId portId;
						if (MKY.IO.Ports.SerialPortId.TryFrom(this.commandLineArgs.SerialPort, out portId))
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.PortId = portId;
						else
							return (false);
					}
					if (this.commandLineArgs.OptionIsGiven("BaudRate"))
					{
						MKY.IO.Ports.BaudRateEx baudRate;
						if (MKY.IO.Ports.BaudRateEx.TryFrom(this.commandLineArgs.BaudRate, out baudRate))
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.Communication.BaudRate = baudRate;
						else
							return (false);
					}
					if (this.commandLineArgs.OptionIsGiven("DataBits"))
					{
						MKY.IO.Ports.DataBitsEx dataBits;
						if (MKY.IO.Ports.DataBitsEx.TryFrom(this.commandLineArgs.DataBits, out dataBits))
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.Communication.DataBits = dataBits;
						else
							return (false);
					}
					if (this.commandLineArgs.OptionIsGiven("Parity"))
					{
						MKY.IO.Ports.ParityEx parity;
						if (MKY.IO.Ports.ParityEx.TryParse(this.commandLineArgs.Parity, out parity))
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.Communication.Parity = parity;
						else
							return (false);
					}
					if (this.commandLineArgs.OptionIsGiven("StopBits"))
					{
						MKY.IO.Ports.StopBitsEx stopBits;
						if (MKY.IO.Ports.StopBitsEx.TryFrom(this.commandLineArgs.StopBits, out stopBits))
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.Communication.StopBits = stopBits;
						else
							return (false);
					}
					if (this.commandLineArgs.OptionIsGiven("FlowControl"))
					{
						MKY.IO.Serial.SerialFlowControlEx flowControl;
						if (MKY.IO.Serial.SerialFlowControlEx.TryParse(this.commandLineArgs.FlowControl, out flowControl))
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.Communication.FlowControl = flowControl;
						else
							return (false);
					}
					if (this.commandLineArgs.OptionIsGiven("SerialPortAutoReopen"))
					{
						if (this.commandLineArgs.SerialPortAutoReopen == 0)
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.AutoRetry(false, 0);
						else if (this.commandLineArgs.SerialPortAutoReopen >= MKY.IO.Serial.SerialPortSettings.AutoReopenMinimumInterval)
							this.startArgs.TerminalSettings.Settings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.AutoRetry(true, this.commandLineArgs.SerialPortAutoReopen);
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
							this.startArgs.TerminalSettings.Settings.IO.Socket.RemoteHost = remoteHost;
						else
							return (false);
					}
					if (((finalIOType == Domain.IOType.TcpClient) ||
						 (finalIOType == Domain.IOType.TcpAutoSocket) ||
						 (finalIOType == Domain.IOType.Udp)) &&
						this.commandLineArgs.OptionIsGiven("RemotePort"))
					{
						if (Int32Ex.IsWithin(this.commandLineArgs.RemotePort, System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MaxPort))
							this.startArgs.TerminalSettings.Settings.IO.Socket.RemotePort = this.commandLineArgs.RemotePort;
						else
							return (false);
					}
					if (this.commandLineArgs.OptionIsGiven("LocalInterface"))
					{
						MKY.Net.IPNetworkInterface localInterface;
						if (MKY.Net.IPNetworkInterface.TryParse(this.commandLineArgs.LocalInterface, out localInterface))
							this.startArgs.TerminalSettings.Settings.IO.Socket.LocalInterface = localInterface;
						else
							return (false);
					}
					if (((finalIOType == Domain.IOType.TcpServer) ||
						 (finalIOType == Domain.IOType.TcpAutoSocket) ||
						 (finalIOType == Domain.IOType.Udp)) &&
						this.commandLineArgs.OptionIsGiven("LocalPort"))
					{
						if (Int32Ex.IsWithin(this.commandLineArgs.LocalPort, System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MaxPort))
							this.startArgs.TerminalSettings.Settings.IO.Socket.LocalPort = this.commandLineArgs.LocalPort;
						else
							return (false);
					}
					if ((finalIOType == Domain.IOType.TcpClient) &&
						this.commandLineArgs.OptionIsGiven("TCPAutoReconnect"))
					{
						if (this.commandLineArgs.TcpAutoReconnect == 0)
							this.startArgs.TerminalSettings.Settings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.AutoRetry(false, 0);
						else if (this.commandLineArgs.TcpAutoReconnect >= MKY.IO.Serial.SocketSettings.TcpClientAutoReconnectMinimumInterval)
							this.startArgs.TerminalSettings.Settings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.AutoRetry(true, this.commandLineArgs.TcpAutoReconnect);
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
						// Both vendor and product ID must be given!
						if (vendorIdIsGiven && productIdIsGiven)
						{
							int vendorId;
							int productId;

							if (!int.TryParse(this.commandLineArgs.VendorId, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out vendorId))
								return (false);

							if (!Int32Ex.IsWithin(vendorId, MKY.IO.Usb.DeviceInfo.FirstVendorId, MKY.IO.Usb.DeviceInfo.LastVendorId))
								return (false);

							if (!int.TryParse(this.commandLineArgs.ProductId, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out productId))
								return (false);
							
							if (!Int32Ex.IsWithin(productId, MKY.IO.Usb.DeviceInfo.FirstProductId, MKY.IO.Usb.DeviceInfo.LastProductId))
								return (false);

							this.startArgs.TerminalSettings.Settings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.DeviceInfo(vendorId, productId);
						}
						else
						{
							return (false);
						}
					}
					if (this.commandLineArgs.OptionIsGiven("NoUSBAutoOpen"))
					{
						this.startArgs.TerminalSettings.Settings.IO.UsbSerialHidDevice.AutoOpen = !this.commandLineArgs.NoUsbAutoOpen;
					}
				}
				else
				{
					return (false);
				}
			}

			if (this.commandLineArgs.OptionIsGiven("OpenTerminal"))
				this.startArgs.TerminalSettings.Settings.TerminalIsStarted = this.commandLineArgs.OpenTerminal;

			if (this.commandLineArgs.OptionIsGiven("BeginLog"))
				this.startArgs.TerminalSettings.Settings.LogIsStarted = this.commandLineArgs.BeginLog;

			// Prio 10 = Perform actions:
			if (this.commandLineArgs.OptionIsGiven("TransmitFile"))
			{
				this.startArgs.RequestedTransmitFilePath = this.commandLineArgs.RequestedTransmitFilePath;
				this.startArgs.PerformActionOnRequestedTerminal = true;
			}

			// Prio 11 = Set behavior:
			if (this.startArgs.PerformActionOnRequestedTerminal)
			{
				this.startArgs.KeepOpen        = this.commandLineArgs.KeepOpen;
				this.startArgs.KeepOpenOnError = this.commandLineArgs.KeepOpenOnError;
			}
			else
			{
				this.startArgs.KeepOpen        = true;
				this.startArgs.KeepOpenOnError = true;
			}

			// Prio 12 = Tile:
			this.startArgs.TileHorizontal = this.commandLineArgs.TileHorizontal;
			this.startArgs.TileVertical   = this.commandLineArgs.TileVertical;

			return (true);
		}

		private bool OtherInstanceIsAlreadyRunning()
		{
			List<Process> processes = new List<Process>();

			// Get all processes named "YAT" (also "NUnit" while testing).
			processes.AddRange(Process.GetProcessesByName(Application.ProductName));

			// Also get all debug processes named "YAT.vshost".
			processes.AddRange(Process.GetProcessesByName(Application.ProductName + ".vshost"));

			// Remove current instance.
			Process currentProcess = Process.GetCurrentProcess();
			foreach (Process p in processes)
			{
				// Comparision must happen through process ID.
				if (p.Id == currentProcess.Id)
				{
					processes.Remove(p);
					break;
				}
			}

			// Consider multiple processes as well as multiple instances within this process.
			return ((processes.Count > 0) || (staticInstancesRunning > 0));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that really all exceptions get caught.")]
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
				try
				{
					File.Delete(ddfp);
				}
				catch
				{
					// Don't care about exceptions.
				}
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

			string fileName = Path.GetFileName(filePath);
			string extension = Path.GetExtension(filePath);

			if      (ExtensionSettings.IsWorkspaceFile(extension))
			{
				OnFixedStatusTextRequest("Opening workspace " + fileName + "...");
				return (OpenWorkspaceFromFile(filePath));
			}
			else if (ExtensionSettings.IsTerminalFile(extension))
			{
				// Create workspace if it doesn't exist yet.
				if (this.workspace == null)
				{
					if (!CreateNewWorkspace())
						return (false);
				}

				OnFixedStatusTextRequest("Opening terminal " + fileName + "...");
				return (this.workspace.OpenTerminalFromFile(filePath));
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
			return (OpenFromFile(ApplicationSettings.LocalUser.RecentFiles.FilePaths[userIndex - 1].Item));
		}

		/// <summary>
		/// Update recent entry.
		/// </summary>
		/// <param name="recentFile">Recent file.</param>
		private void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(recentFile);
			ApplicationSettings.Save();
		}

		#endregion

		#region Exit
		//==========================================================================================
		// Exit
		//==========================================================================================

		/// <summary></summary>
		public virtual MainResult Exit()
		{
			bool success;

			if (this.workspace != null)
				success = this.workspace.Close(true);
			else
				success = true;

			if (success)
				staticInstancesRunning--;

			if (success)
				OnFixedStatusTextRequest("Exiting " + Application.ProductName + "...");
			else
				OnTimedStatusTextRequest("Exit cancelled.");

			if (success)
				OnExited(new EventArgs());

			if (success)
				return (MainResult.Success);
			else
				return (MainResult.ApplicationExitError);
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
			ApplicationSettings.LocalUser.AutoWorkspace.SetFilePathAndUser(e.FilePath, Guid);
			ApplicationSettings.Save();
		}

		/// <remarks>
		/// See remarks of <see cref="YAT.Model.Workspace.Close(bool)"/> for details on why this event handler
		/// needs to treat the Closed event differently in case of a parent (i.e. main) close.
		/// </remarks>
		private void workspace_Closed(object sender, ClosedEventArgs e)
		{
			if (!e.IsParentClose) // In case of workspace intended close, completely reset workspace info
				ApplicationSettings.LocalUser.AutoWorkspace.ResetFilePathAndUser();
			else                  // In case of parent close, just signal that workspace isn't in use anymore
				ApplicationSettings.LocalUser.AutoWorkspace.ResetUserOnly();

			ApplicationSettings.Save();

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
			// Close workspace, only one workspace can exist within application.
			if (this.workspace != null)
			{
				if (!this.workspace.Close())
					return (false);
			}

			OnFixedStatusTextRequest("Creating new workspace...");

			// Create workspace.
			this.workspace = new Workspace(new DocumentSettingsHandler<WorkspaceSettingsRoot>());
			AttachWorkspaceEventHandlers();
			OnWorkspaceOpened(new WorkspaceEventArgs(this.workspace));

			OnTimedStatusTextRequest("New workspace created.");
			return (true);
		}

		/// <summary></summary>
		public virtual bool OpenWorkspaceFromFile(string filePath)
		{
			AssertNotDisposed();

			// Open the workspace file, then the workspace itself.
			// The workspace file and the workspace itself is checked within OpenWorkspaceFromSettings().
			DocumentSettingsHandler<WorkspaceSettingsRoot> settings;
			Guid guid;
			System.Xml.XmlException ex;

			OnFixedStatusTextRequest("Opening workspace file...");
			if (OpenWorkspaceFile(filePath, out settings, out guid, out ex))
			{
				return (OpenWorkspaceFromSettings(settings));
			}
			else
			{
				OnFixedStatusTextRequest("Error opening workspace!");
				OnMessageInputRequest
					(
					"Unable to open file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
					"XML error message: " + ex.Message + Environment.NewLine +
					"File error message: " + ex.InnerException.Message,
					"Invalid Workspace File",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
					);
				OnTimedStatusTextRequest("No workspace opened!");
				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings)
		{
			return (OpenWorkspaceFromSettings(settings, Indices.InvalidIndex, null));
		}

		/// <summary></summary>
		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings, int dynamicTerminalIndexToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
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
			this.workspace = new Workspace(settings, guid);
			AttachWorkspaceEventHandlers();

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
				settings = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
				settings.SettingsFilePath = filePath;
				settings.Load();

				ApplicationSettings.LocalUser.AutoWorkspace.SetFilePathAndUser(filePath, Guid);
				ApplicationSettings.Save();

				// Try to retrieve GUID from file path (in case of auto saved workspace files).
				if (!GuidEx.TryCreateGuidFromFilePath(filePath, GeneralSettings.AutoSaveWorkspaceFileNamePrefix, out guid))
					guid = Guid.NewGuid();

				exception = null;
				return (true);
			}
			catch (System.Xml.XmlException ex)
			{
				DebugEx.WriteException(this.GetType(), ex);
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
		public virtual bool CreateNewTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> sh)
		{
			if (this.workspace == null)
				CreateNewWorkspace();

			return (this.workspace.CreateNewTerminal(sh));
		}

		/// <summary></summary>
		public virtual bool OpenTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> sh)
		{
			if (this.workspace == null)
				CreateNewWorkspace();

			return (this.workspace.OpenTerminalFromSettings(sh));
		}

		/// <summary></summary>
		public virtual bool OpenTerminalFromFile(string filePath)
		{
			if (this.workspace == null)
				CreateNewWorkspace();

			return (this.workspace.OpenTerminalFromFile(filePath));
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
				settings = new DocumentSettingsHandler<TerminalSettingsRoot>();
				settings.SettingsFilePath = terminalFilePath;
				settings.Load();

				exception = null;
				return (true);
			}
			catch (System.Xml.XmlException ex)
			{
				DebugEx.WriteException(this.GetType(), ex);
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
			return (e.Result);
		}

		/// <summary></summary>
		protected virtual void OnExited(EventArgs e)
		{
			EventHelper.FireSync(Exited, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
