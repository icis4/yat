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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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

#if (WITH_SCRIPTING)
using MT.Albatros.Core;
#endif

using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Provides the YAT application model which can handle workspaces (.yaw) and terminals (.yat).
	/// </summary>
	public class Main : DisposableBase, IGuidProvider
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Main).FullName);

		private CommandLineArgs commandLineArgs;
		private MainStartArgs startArgs;
		private Guid guid;

		private Workspace workspace; // = null;
	#if (WITH_SCRIPTING)
		private ScriptBridge scriptBridge; // = null;
	#endif

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

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			this.eventHelper.DiscardAllEventsAndExceptions();

			DebugMessage("Disposing...");

			// Dispose of managed resources:
			if (disposing)
			{
				StopAndDisposeOperationTimer();
				StopAndDisposeExitTimer();

				// In the 'normal' case, the workspace has already been closed, otherwise...

				// ...detach event handlers to ensure that no more events are received...
				DetachWorkspaceEventHandlers();

				// ...dispose of workspace (normally it disposes of itself)...
				if (this.workspace != null) {
					this.workspace.Dispose();
					this.workspace = null;
				}

				DetachStaticSerialPortCollectionEventHandlers();
			}

			DebugMessage("...successfully disposed.");
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
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.guid);
			}
		}

		/// <summary></summary>
		public virtual MainStartArgs StartArgs
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.workspace != null)
					return (this.workspace.IndicatedName);
				else
					return (ApplicationEx.ProductName); // "YAT" or "YATConsole" shall be indicated in main title bar.
			}
		}

		/// <summary>
		/// Returns workspace within main; or <c>null</c> if no workspace is active.
		/// </summary>
		public virtual Workspace Workspace
		{
			get
			{
				AssertUndisposed();

				return (this.workspace);
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Returns interface between main and script engine; or <c>null</c> if script engine is not ready yet.
		/// </summary>
		public virtual ScriptBridge ScriptBridge
		{
			get
			{
			////AssertUndisposed(); \todo !!! (2017-11-14 / MKY) accessed after call to Exit() !!!

				return (this.scriptBridge);
			}
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		public MainResult Result
		{
			get { return (this.result); }

		#if (WITH_SCRIPTING)
			set { this.result = value; }
		#endif
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
			AssertUndisposed();

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
			AssertUndisposed();

			// Process command line args into start requests:
			if (!ProcessCommandLineArgsIntoStartRequests())
			{
				if ((this.commandLineArgs == null) || (!this.commandLineArgs.IsValid))
					return (MainResult.CommandLineError);
				else
					return (MainResult.ApplicationStartError);
			}

			// Application will start, hence initialize required resources:
			ProcessorLoad.Initialize();
		#if (WITH_SCRIPTING)
			this.scriptBridge = new ScriptBridge(this, this.startArgs.ScriptRunIsRequested);
		#endif

			// Start according to the start requests:
			bool success = false;

			bool workspaceIsRequested = (this.startArgs.WorkspaceSettingsHandler != null);
			bool terminalIsRequested  = (this.startArgs.TerminalSettingsHandler  != null);

			if (workspaceIsRequested || terminalIsRequested)
			{
				if (workspaceIsRequested && terminalIsRequested)
				{
					success = OpenWorkspaceFromSettings(this.startArgs.WorkspaceSettingsHandler, this.startArgs.RequestedDynamicTerminalId, this.startArgs.TerminalSettingsHandler);
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
				ApplicationSettings.SaveLocalUserSettings();

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
			// in YAT.Application.Main.Run().
			//
			// In case of automated testing, the command line arguments may also be processed and
			// validated here for the first time.

			// Prio 0 = None:
			if ((this.commandLineArgs == null) || this.commandLineArgs.HasNoArgs)
			{
				// This is the standard case, the 'New Terminal' dialog will get shown:
				if (this.commandLineArgs == null)
				{
					this.startArgs.ShowNewTerminalDialog = true;
					this.startArgs.KeepOpen              = true;
					this.startArgs.KeepOpenOnError       = true;

					return (true);
				}
				// In case of "YATConsole.exe" the application will set the 'NonInteractive' option,
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
					string absoluteFilePath;
					DocumentSettingsHandler<WorkspaceSettingsRoot> sh;
					Exception ex;
					if (OpenWorkspaceFile(requestedFilePath, out absoluteFilePath, out sh, out ex))
					{
						this.startArgs.WorkspaceSettingsHandler = sh;
					}
					else
					{
						this.startArgs.ErrorMessage = ErrorHelper.ComposeMessage("Unable to open workspace file", absoluteFilePath, ex);
						return (false);
					}
				}
				else if (ExtensionHelper.IsTerminalFile(requestedFilePath))
				{
					string absoluteFilePath;
					DocumentSettingsHandler<TerminalSettingsRoot> sh;
					Exception ex;
					if (OpenTerminalFile(requestedFilePath, out absoluteFilePath, out sh, out ex))
					{
						this.startArgs.TerminalSettingsHandler = sh;
					}
					else
					{
						this.startArgs.ErrorMessage = ErrorHelper.ComposeMessage("Unable to open terminal file", absoluteFilePath, ex);
						return (false);
					}
				}
				else
				{
					return (false);
				}
			}

			// Prio 7 = Retrieve the requested terminal within the workspace and validate it:
			Domain.IOType implicitIOType = Domain.IOType.Unknown;
			bool updateDependentSettings = false;
			if (this.startArgs.WorkspaceSettingsHandler != null) // Applies to a terminal within a workspace.
			{
				if (this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count > 0)
				{
					int requestedDynamicTerminalId = this.commandLineArgs.RequestedDynamicTerminalId;
					int lastDynamicId = TerminalIds.IndexToDynamicId(this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count - 1);

					if     ((          requestedDynamicTerminalId >= TerminalIds.FirstDynamicId) && (requestedDynamicTerminalId <= lastDynamicId))
						this.startArgs.RequestedDynamicTerminalId = requestedDynamicTerminalId;
					else if (          requestedDynamicTerminalId == TerminalIds.ActiveDynamicId)
						this.startArgs.RequestedDynamicTerminalId  = TerminalIds.ActiveDynamicId;
					else if (          requestedDynamicTerminalId == TerminalIds.InvalidDynamicId)
						this.startArgs.RequestedDynamicTerminalId  = TerminalIds.InvalidDynamicId; // Usable to disable the operation.
					else
						return (false);

					if (this.startArgs.RequestedDynamicTerminalId != TerminalIds.InvalidDynamicId)
					{
						string workspaceFilePath = this.startArgs.WorkspaceSettingsHandler.SettingsFilePath;

						string terminalFilePath;
						if (this.startArgs.RequestedDynamicTerminalId == TerminalIds.ActiveDynamicId) // The active terminal is located last in the collection:
							terminalFilePath = this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings[this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count - 1].FilePath;
						else
							terminalFilePath = this.startArgs.WorkspaceSettingsHandler.Settings.TerminalSettings[TerminalIds.DynamicIdToIndex(this.startArgs.RequestedDynamicTerminalId)].FilePath;

						DocumentSettingsHandler<TerminalSettingsRoot> sh;
						if (OpenTerminalFile(workspaceFilePath, terminalFilePath, out sh))
							this.startArgs.TerminalSettingsHandler = sh;
						else
							return (false);
					}
				}
			}
			else if (this.startArgs.TerminalSettingsHandler != null) // Applies to a dedicated terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalId == TerminalIds.InvalidDynamicId)
					this.startArgs.RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Usable to disable the operation.
			}
			else if (this.commandLineArgs.NewIsRequested) // Applies to a new terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalId == TerminalIds.InvalidDynamicId)
					this.startArgs.RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Usable to disable the operation.

				this.startArgs.TerminalSettingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();
			}
			else if (this.commandLineArgs.NewIsRequestedImplicitly(out implicitIOType)) // Also applies to a new terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalId == TerminalIds.InvalidDynamicId)
					this.startArgs.RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Usable to disable the operation.

				this.startArgs.TerminalSettingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();

				if (implicitIOType != Domain.IOType.Unknown)
					this.startArgs.TerminalSettingsHandler.Settings.IOType = implicitIOType;

				updateDependentSettings = true; // Same as when using the on the 'New Terminal' or 'Terminal Settings'
			}                                   // dialog, dependent settings must be updated accordingly. But this
			else                                // must happen AFTER having processed the args into the settings!
			{
				this.startArgs.RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Disable the operation.
			}

			// Prio 8 = Override explicit settings as desired:
			if (this.startArgs.TerminalSettingsHandler != null) // Applies to a dedicated terminal.
			{
				if (!ProcessCommandLineArgsIntoExistingTerminalSettings(this.startArgs.TerminalSettingsHandler.Settings.Terminal))
					return (false);

				if (updateDependentSettings) // See comments further above.
					this.startArgs.TerminalSettingsHandler.Settings.Terminal.UpdateAllDependentSettings();

				if (this.commandLineArgs.OptionIsGiven("StartTerminal"))
					this.startArgs.TerminalSettingsHandler.Settings.TerminalIsStarted = this.commandLineArgs.StartTerminal;

				if (this.commandLineArgs.OptionIsGiven("LogOn"))
					this.startArgs.TerminalSettingsHandler.Settings.LogIsOn           = this.commandLineArgs.LogOn;
			}

			// Prio 9 = Override overall settings as desired:
			if (this.commandLineArgs.OptionIsGiven("StartTerminal"))
				this.startArgs.Override.StartTerminal       = this.commandLineArgs.StartTerminal;

			if (this.commandLineArgs.OptionIsGiven("KeepTerminalStopped"))
				this.startArgs.Override.KeepTerminalStopped = this.commandLineArgs.KeepTerminalStopped;

			if (this.commandLineArgs.OptionIsGiven("LogOn"))
				this.startArgs.Override.LogOn               = this.commandLineArgs.LogOn;

			if (this.commandLineArgs.OptionIsGiven("KeepLogOff"))
				this.startArgs.Override.KeepLogOff          = this.commandLineArgs.KeepLogOff;

			// Prio 10 = Perform requested operation:
			if (this.startArgs.RequestedDynamicTerminalId != TerminalIds.InvalidDynamicId)
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

		#if (WITH_SCRIPTING)
			// Prio 11 = Run script:
			if (this.commandLineArgs.OptionIsGiven("Script"))
			{
				this.startArgs.RequestedScriptFilePath = this.commandLineArgs.RequestedScriptFilePath;

				if (this.commandLineArgs.OptionIsGiven("ScriptLog"))
				{
					if (PathEx.IsValid(this.commandLineArgs.RequestedScriptLogFilePath))
						this.startArgs.RequestedScriptLogFilePath = this.commandLineArgs.RequestedScriptLogFilePath;
					else
						return (false);
				}

				if (this.commandLineArgs.OptionIsGiven("ScriptLogTimeStamp"))
					this.startArgs.AppendTimeStampToScriptLogFileName = true;

				if (this.commandLineArgs.OptionIsGiven("ScriptArgs"))
					this.startArgs.RequestedScriptArgs = this.commandLineArgs.RequestedScriptArgs;

				this.startArgs.ScriptRunIsRequested = true;
			}
		#endif

			// Prio 12 = Set behavior:
			if (this.startArgs.PerformOperationOnRequestedTerminal)
			{
				this.startArgs.OperationDelay = this.commandLineArgs.OperationDelay;
				this.startArgs.ExitDelay      = this.commandLineArgs.ExitDelay;
			}

		#if !(WITH_SCRIPTING)
			if (this.startArgs.PerformOperationOnRequestedTerminal)
		#else
			if (this.startArgs.PerformOperationOnRequestedTerminal || this.startArgs.ScriptRunIsRequested)
		#endif
			{
				this.startArgs.KeepOpen        = this.commandLineArgs.KeepOpen;
				this.startArgs.KeepOpenOnError = this.commandLineArgs.KeepOpenOnError;
			}
			else
			{
				this.startArgs.KeepOpen        = true;
				this.startArgs.KeepOpenOnError = true;
			}

			// Prio 13 = Set layout:
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
		private bool ProcessCommandLineArgsIntoExistingTerminalSettings(Domain.Settings.TerminalSettings terminalSettings)
		{
			if (this.commandLineArgs.OptionIsGiven("TerminalType"))
			{
				Domain.TerminalType terminalType;
				if (Domain.TerminalTypeEx.TryParse(this.commandLineArgs.TerminalType, out terminalType))
				{
					if (terminalSettings.TerminalType != terminalType)
					{
						terminalSettings.TerminalType = terminalType;
						terminalSettings.UpdateTerminalTypeDependentSettings(); // Needed because the parent settings don't notice the 'IOType' change above!
					}
				}
				else
				{
					return (false);
				}
			}

			if (this.commandLineArgs.OptionIsGiven("PortType")) // Called 'PortType' because it shall match the name on the 'New Terminal' and 'Terminal Settings' dialog.
			{
				Domain.IOType ioType;
				if (Domain.IOTypeEx.TryParse(this.commandLineArgs.IOType, out ioType))
				{
					if (terminalSettings.IO.IOType != ioType)
					{
						terminalSettings.IO.IOType = ioType;
						terminalSettings.UpdateIOTypeDependentSettings(); // Needed because the parent settings don't notice the 'IOType' change above!
					}
				}
				else
				{
					return (false);
				}
			}

			var finalIOType = terminalSettings.IO.IOType;
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
					{
						if (terminalSettings.IO.SerialPort.Communication.FlowControl != flowControl)
						{
							terminalSettings.IO.SerialPort.Communication.FlowControl = flowControl;
							terminalSettings.UpdateIOSettingsDependentSettings(); // Needed because the parent settings don't notice the 'IOType' change above!
						}
					}
					else
					{
						return (false);
					}
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

						int usagePage;
						int usageId;

						if (!int.TryParse(this.commandLineArgs.UsagePage, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out usagePage))
							return (false);

						if (!MKY.IO.Usb.HidDeviceInfo.IsValidUsagePageOrAny(usagePage))
							return (false);

						if (!int.TryParse(this.commandLineArgs.UsageId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out usageId))
							return (false);

						if (!MKY.IO.Usb.HidDeviceInfo.IsValidUsageIdOrAny(usageId))
							return (false);

						// The SNR is optional:
						if (!this.commandLineArgs.OptionIsGiven("SerialString"))
						{
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.HidDeviceInfo(vendorId, productId, usagePage, usageId);
							terminalSettings.IO.UsbSerialHidDevice.MatchSerial = false; // Command line option shall override 'ApplicationSettings.LocalUserSettings.General.MatchUsbSerial'.
						}
						else
						{
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.HidDeviceInfo(vendorId, productId, this.commandLineArgs.SerialString, usagePage, usageId);
							terminalSettings.IO.UsbSerialHidDevice.MatchSerial = true; // Command line option shall override 'ApplicationSettings.LocalUserSettings.General.MatchUsbSerial'.
						}
					}
					else
					{
						return (false);
					}
				}
				if (this.commandLineArgs.OptionIsGiven("FormatPreset"))
				{
					MKY.IO.Usb.SerialHidDeviceSettingsPresetEx preset;
					if (MKY.IO.Usb.SerialHidDeviceSettingsPresetEx.TryParse(this.commandLineArgs.FormatPreset, out preset))
						terminalSettings.IO.UsbSerialHidDevice.Preset = preset;
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("FlowControl"))
				{
					MKY.IO.Serial.Usb.SerialHidFlowControl flowControl;
					if (MKY.IO.Serial.Usb.SerialHidFlowControlEx.TryParse(this.commandLineArgs.FlowControl, out flowControl))
					{
						if (terminalSettings.IO.UsbSerialHidDevice.FlowControl != flowControl)
						{
							terminalSettings.IO.UsbSerialHidDevice.FlowControl = flowControl;
							terminalSettings.UpdateIOSettingsDependentSettings(); // Needed because the parent settings don't notice the 'IOType' change above!
						}
					}
					else
					{
						return (false);
					}
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
			var temp = new Domain.Settings.TerminalSettings();

			temp.TerminalType                        = newTerminalSettings.TerminalType;
			temp.IO.IOType                           = newTerminalSettings.IOType;

			temp.IO.SerialPort.PortId                = newTerminalSettings.SerialPortId;
			temp.IO.SerialPort.Communication         = newTerminalSettings.SerialPortCommunication;
			temp.IO.SerialPort.AliveMonitor          = newTerminalSettings.SerialPortAliveMonitor;
			temp.IO.SerialPort.AutoReopen            = newTerminalSettings.SerialPortAutoReopen;

			temp.IO.Socket.RemoteHost                = newTerminalSettings.SocketRemoteHost;
			temp.IO.Socket.RemoteTcpPort             = newTerminalSettings.SocketRemoteTcpPort;
			temp.IO.Socket.RemoteUdpPort             = newTerminalSettings.SocketRemoteUdpPort;
			temp.IO.Socket.LocalInterface            = newTerminalSettings.SocketLocalInterface;
			temp.IO.Socket.LocalFilter               = newTerminalSettings.SocketLocalFilter;
			temp.IO.Socket.LocalTcpPort              = newTerminalSettings.SocketLocalTcpPort;
			temp.IO.Socket.LocalUdpPort              = newTerminalSettings.SocketLocalUdpPort;
			temp.IO.Socket.TcpClientAutoReconnect    = newTerminalSettings.TcpClientAutoReconnect;
			temp.IO.Socket.UdpServerSendMode         = newTerminalSettings.UdpServerSendMode;

			temp.IO.UsbSerialHidDevice.DeviceInfo    = newTerminalSettings.UsbSerialHidDeviceInfo;
		////temp.IO.UsbSerialHidDevice.MatchSerial is defined by 'ApplicationSettings.LocalUserSettings.General.MatchUsbSerial'.
			temp.IO.UsbSerialHidDevice.Preset        = newTerminalSettings.UsbSerialHidPreset;
			temp.IO.UsbSerialHidDevice.ReportFormat  = newTerminalSettings.UsbSerialHidReportFormat;
			temp.IO.UsbSerialHidDevice.RxFilterUsage = newTerminalSettings.UsbSerialHidRxFilterUsage;
			temp.IO.UsbSerialHidDevice.FlowControl   = newTerminalSettings.UsbSerialHidFlowControl;
			temp.IO.UsbSerialHidDevice.AutoOpen      = newTerminalSettings.UsbSerialHidAutoOpen;
		////temp.IO.UsbSerialHidDevice.IncludeNonPayloadData is an advanced setting, i.e. not available in the [New Terminal] dialog.

			if (ProcessCommandLineArgsIntoExistingTerminalSettings(temp))
			{
				newTerminalSettings.TerminalType              = temp.TerminalType;
				newTerminalSettings.IOType                    = temp.IO.IOType;

				newTerminalSettings.SerialPortId              = temp.IO.SerialPort.PortId;
				newTerminalSettings.SerialPortCommunication   = temp.IO.SerialPort.Communication;
				newTerminalSettings.SerialPortAliveMonitor    = temp.IO.SerialPort.AliveMonitor;
				newTerminalSettings.SerialPortAutoReopen      = temp.IO.SerialPort.AutoReopen;

				newTerminalSettings.SocketRemoteHost          = temp.IO.Socket.RemoteHost;
				newTerminalSettings.SocketRemoteTcpPort       = temp.IO.Socket.RemoteTcpPort;
				newTerminalSettings.SocketRemoteUdpPort       = temp.IO.Socket.RemoteUdpPort;
				newTerminalSettings.SocketLocalInterface      = temp.IO.Socket.LocalInterface;
				newTerminalSettings.SocketLocalFilter         = temp.IO.Socket.LocalFilter;
				newTerminalSettings.SocketLocalTcpPort        = temp.IO.Socket.LocalTcpPort;
				newTerminalSettings.SocketLocalUdpPort        = temp.IO.Socket.LocalUdpPort;
				newTerminalSettings.TcpClientAutoReconnect    = temp.IO.Socket.TcpClientAutoReconnect;
				newTerminalSettings.UdpServerSendMode         = temp.IO.Socket.UdpServerSendMode;

				newTerminalSettings.UsbSerialHidDeviceInfo    = temp.IO.UsbSerialHidDevice.DeviceInfo;
			////newTerminalSettings.UsbSerialHidMatchSerial is defined by 'ApplicationSettings.LocalUserSettings.General.MatchUsbSerial'.
				newTerminalSettings.UsbSerialHidPreset        = temp.IO.UsbSerialHidDevice.Preset;
				newTerminalSettings.UsbSerialHidReportFormat  = temp.IO.UsbSerialHidDevice.ReportFormat;
				newTerminalSettings.UsbSerialHidRxFilterUsage = temp.IO.UsbSerialHidDevice.RxFilterUsage;
				newTerminalSettings.UsbSerialHidFlowControl   = temp.IO.UsbSerialHidDevice.FlowControl;
				newTerminalSettings.UsbSerialHidAutoOpen      = temp.IO.UsbSerialHidDevice.AutoOpen;
			////newTerminalSettings.UsbSerialHidIncludeNonPayloadData is an advanced setting, i.e. not available in the [New Terminal] dialog.

				return (true);
			}
			else
			{
				return (false);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void CleanupLocalUserDirectory()
		{
			// Get all file paths in default directory:
			var localUserDirectoryFilePaths = new List<string>();
			try
			{
				var localUserDirectory = Directory.GetParent(ApplicationSettings.LocalUserSettingsFilePath);
				localUserDirectoryFilePaths.AddRange(Directory.GetFiles(localUserDirectory.FullName));
			}
			catch
			{
				// Don't care about exceptions.
			}

			// Ensure to leave application settings untouched:
			localUserDirectoryFilePaths.Remove(ApplicationSettings.LocalUserSettingsFilePath);

			// Get all active files:
			var activeFilePaths = new List<string>();
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
			AssertUndisposed();

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
			AssertUndisposed();

			return (OpenFromFile(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[userIndex - 1].Item));
		}

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
		#if (WITH_SCRIPTING)

			bool scriptSuccess = true;
			if (this.scriptBridge.ScriptRunIsOngoing)
			{
				var text = new StringBuilder();

				var scriptFileName = this.scriptBridge.ScriptFileName;
				if (string.IsNullOrEmpty(scriptFileName))
					text.AppendFormat(@"A script is still running.");
				else
					text.AppendFormat(@"Script ""{0}"" is still running.", scriptFileName);

				text.AppendLine();
				text.AppendLine();
				text.AppendFormat(@"Confirm to break the script and exit {0}; or cancel.", ApplicationEx.ProductName);

				var dr = OnMessageInputRequest(text.ToString(), "Exiting...", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
				if (dr == DialogResult.OK)
					this.scriptBridge.BreakScript();
				else
					scriptSuccess = false;
			}

			if (scriptSuccess)
				this.scriptBridge.ProcessCommand("Script:CloseDialog");

		#endif // WITH_SCRIPTING

			bool workspaceSuccess;
			if (this.workspace != null)
				workspaceSuccess = this.workspace.CloseConsiderately(true);
			else
				workspaceSuccess = true;

		#if !(WITH_SCRIPTING)
			if (workspaceSuccess)
		#else
			if (scriptSuccess && workspaceSuccess)
		#endif
			{
				OnFixedStatusTextRequest("Exiting " + ApplicationEx.ProductName + "..."); // "YAT" or "YATConsole", as indicated in main title bar.

				// Discard potential exceptions already BEFORE signalling the close! Required to
				// prevent exceptions on still pending asynchronous callbacks trying to synchronize
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
					var inUseLookup = new List<MKY.IO.Ports.InUseInfo>(terminals.Length); // Preset the required capacity to improve memory management.

					foreach (var t in this.workspace.Terminals)
					{
						var portId = t.IOSerialPortId;
						if (portId != null)
						{
							string inUseText; // Attention: Same texts are used in YAT.View.Forms.TerminalSettings.SetControls().
							if (t.IsOpen)     //            Changes below likely have to be applied there too.
								inUseText = "(in use by " + t.IndicatedName + ")";
							else
								inUseText = "(selected by " + t.IndicatedName + ")";

							inUseLookup.Add(new MKY.IO.Ports.InUseInfo(t.SequentialId, portId, t.IsOpen, inUseText));
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

				this.workspace.ExitRequest += workspace_ExitRequest;
			}
		}

		private void DetachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.Saved  -= workspace_Saved;
				this.workspace.Closed -= workspace_Closed;

				this.workspace.ExitRequest -= workspace_ExitRequest;
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
			ApplicationSettings.SaveLocalUserSettings();
		}

		/// <remarks>
		/// See remarks of <see cref="Workspace.CloseConsiderately"/> for details on why this handler
		/// needs to treat the event differently in case of a parent (i.e. main) close.
		/// </remarks>
		private void workspace_Closed(object sender, ClosedEventArgs e)
		{
			if (!e.IsParentClose) // In case of workspace intended close, reset workspace info.
			{
				ApplicationSettings.LocalUserSettings.AutoWorkspace.ResetFilePath();
				ApplicationSettings.SaveLocalUserSettings();
			}

			DetachWorkspaceEventHandlers();
			this.workspace = null; // Simply de-reference the workspace, it disposes of itself.
			OnWorkspaceClosed(e);
		}

		private void workspace_ExitRequest(object sender, EventArgs e)
		{
			Exit();
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
			AssertUndisposed();

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
			string absoluteFilePath;
			DocumentSettingsHandler<WorkspaceSettingsRoot> sh;
			Guid guid;
			Exception ex;
			if (OpenWorkspaceFile(filePath, out absoluteFilePath, out sh, out guid, out ex))
			{
				if (OpenWorkspaceFromSettings(sh, guid, out ex))
				{
					errorMessage = null;
					return (true);
				}
				else
				{
					errorMessage = ErrorHelper.ComposeMessage("Unable to open workspace", absoluteFilePath, ex);
					return (false);
				}
			}
			else
			{
				errorMessage = ErrorHelper.ComposeMessage("Unable to open workspace file", absoluteFilePath, ex);
				return (false);
			}
		}

		/// <summary></summary>
		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
		{
			Exception exception;
			return (OpenWorkspaceFromSettings(settingsHandler, Guid.NewGuid(), out exception));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, int dynamicTerminalIdToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
		{
			Exception exception;
			return (OpenWorkspaceFromSettings(settingsHandler, Guid.NewGuid(), dynamicTerminalIdToReplace, terminalSettingsToReplace, out exception));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid, out Exception exception)
		{
			return (OpenWorkspaceFromSettings(settingsHandler, guid, TerminalIds.InvalidIndex, null, out exception));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings, Guid guid, int dynamicTerminalIdToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace, out Exception exception)
		{
			AssertUndisposed();

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
			ApplicationSettings.SaveLocalUserSettings();

			// Save recent:
			if (!settings.Settings.AutoSaved)
				SetRecent(settings.SettingsFilePath);

			OnWorkspaceOpened(this.workspace);
			OnTimedStatusTextRequest("Workspace opened.");

			// Open workspace terminals:
			int terminalCount = this.workspace.OpenTerminals(dynamicTerminalIdToReplace, terminalSettingsToReplace);
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
						"Workspace",
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation
					);
					OnTimedStatusTextRequest("Workspace not re-opened.");
					return (false);
				}
			}
			return (true);
		}

		private bool OpenWorkspaceFile(string filePath, out string absoluteFilePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, out Exception exception)
		{
			Guid guid;
			return (OpenWorkspaceFile(filePath, out absoluteFilePath, out settingsHandler, out guid, out exception));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenWorkspaceFile(string filePath, out string absoluteFilePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, out Guid guid, out Exception exception)
		{
			absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(filePath);

			try
			{
				var sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
				sh.SettingsFilePath = absoluteFilePath;
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

		/// <remarks>Needed for opening command line requested terminal files without yet creating a workspace.</remarks>
		private bool OpenTerminalFile(string terminalFilePath, out string absoluteTerminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exception)
		{
			return (OpenTerminalFile("", terminalFilePath, out absoluteTerminalFilePath, out settingsHandler, out exception));
		}

		/// <remarks>Needed for opening command line requested terminal files without yet creating a workspace.</remarks>
		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			string absoluteTerminalFilePath;
			Exception exception;
			return (OpenTerminalFile(workspaceFilePath, terminalFilePath, out absoluteTerminalFilePath, out settingsHandler, out exception));
		}

		/// <remarks>Needed for opening command line requested terminal files without yet creating a workspace.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out string absoluteTerminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exception)
		{
			// Attention:
			// Similar code exists in Workspace.OpenTerminalFile().
			// Changes here may have to be applied there too.

			absoluteTerminalFilePath = null;

			// Try to combine workspace and terminal path:
			if (!string.IsNullOrEmpty(workspaceFilePath))
				absoluteTerminalFilePath = PathEx.CombineFilePaths(EnvironmentEx.ResolveAbsolutePath(workspaceFilePath), terminalFilePath);

			// Alternatively, try to use terminal file path only:
			if (string.IsNullOrEmpty(absoluteTerminalFilePath))
				absoluteTerminalFilePath = EnvironmentEx.ResolveAbsolutePath(terminalFilePath);

			try
			{
				var sh = new DocumentSettingsHandler<TerminalSettingsRoot>();
				sh.SettingsFilePath = absoluteTerminalFilePath;
				if (sh.Load())
				{
					// The 'MatchSerial' setting is given by the 'LocalUserSettings' and always overridden.
					// Still, it is an integral part of MKY.IO.Serial.Usb, will thus be contained in the .yat file.
					sh.Settings.Terminal.IO.UsbSerialHidDevice.MatchSerial = ApplicationSettings.LocalUserSettings.General.MatchUsbSerial;
					sh.Settings.ClearChanged(); // Overriding such setting shall not be reflected in the settings,
					                          //// i.e. neither be indicated by a '*' nor lead to a file write.
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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

					int id = this.startArgs.RequestedDynamicTerminalId;

					Terminal requestedTerminal;
					if (!this.workspace.TryGetTerminalByDynamicId(id, out requestedTerminal))
					{
						StopAndDisposeOperationTimer();

						OnFixedStatusTextRequest("Invalid requested dynamic ID!");
						OnMessageInputRequest
						(
							"Invalid requested dynamic ID " + id.ToString(CultureInfo.CurrentCulture) + " to perform the operation!" + Environment.NewLine + Environment.NewLine +
							"Check the command line arguments. See command line help for details.",
							"Invalid Terminal ID",
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
						return; // Pend!                               Using term 'Transmission' to indicate potential
					}           //                                     'intelligence' to send + receive/verify the data.

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
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("operationTimer_Elapsed() monitor has timed out!");
			}
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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
						int id = this.startArgs.RequestedDynamicTerminalId;

						Terminal t;
						if (this.workspace.TryGetTerminalByDynamicId(id, out t) && (t.SendingIsOngoing))
						{
							OnTimedStatusTextRequest("Exit triggered, pending while terminal is sending...");
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
						OnFixedStatusTextRequest("Automatically exiting " + ApplicationEx.ProductName + "..."); // "YAT" or "YATConsole", as indicated in main title bar.
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
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("exitTimer_Elapsed() monitor has timed out!");
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnWorkspaceOpened(Workspace workspace)
		{
			this.eventHelper.RaiseSync<EventArgs<Workspace>>(WorkspaceOpened, this, new EventArgs<Workspace>(workspace));
		}

		/// <summary></summary>
		protected virtual void OnWorkspaceClosed(ClosedEventArgs e)
		{
			this.eventHelper.RaiseSync<ClosedEventArgs>(WorkspaceClosed, this, e);
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			if (this.startArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				var e = new MessageInputEventArgs(text, caption, buttons, icon, defaultButton);
				this.eventHelper.RaiseSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Message Input' request by main has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				#endif
				}

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
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

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnExited(MainResult result)
		{
			this.eventHelper.RaiseSync<EventArgs<MainResult>>(Exited, this, new EventArgs<MainResult>(result));
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that somthing will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
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
