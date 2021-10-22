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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.CommandLine;
using MKY.Diagnostics;
using MKY.Drawing;
using MKY.IO;
using MKY.Settings;

#if (WITH_SCRIPTING)
using MT.Albatros.Core;
#endif

using YAT.Application.Utilities;
//// "YAT.Model.Utilities" is explicitly used due to ambiguity of "MessageHelper".
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
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int OperationAndExitTimerInterval = 1000;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private readonly EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Main).FullName);

		private CommandLineArgs commandLineArgs;
		private MainLaunchArgs launchArgs;
		private Guid guid;

		private Workspace workspace; // = null;
	#if (WITH_SCRIPTING)
		private ScriptBridge scriptBridge; // = null;
	#endif

		private MainResult result = MainResult.Success;

		private System.Threading.Timer operationTimer; // Ambiguity with 'System.Windows.Forms.Timer'.
		private readonly object operationTimerSyncObj = new object();

		private System.Threading.Timer exitTimer; // Ambiguity with 'System.Windows.Forms.Timer'.
		private readonly object exitTimerSyncObj = new object();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<EventArgs<Workspace>> WorkspaceOpened;

		/// <summary></summary>
		public event EventHandler<EventArgs<Workspace>> WorkspaceSaved;

		/// <summary></summary>
		public event EventHandler<ClosedEventArgs> WorkspaceClosed;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<ExtendedMessageInputEventArgs> ExtendedMessageInputRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<Cursor>> CursorRequest;

		/// <summary></summary>
		public event EventHandler Launched;

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
			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				DebugMessage("Disposing...");

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

				DebugMessage("...successfully disposed.");
			}

		////base.Dispose(disposing) of 'DisposableBase' doesn't need and cannot be called since abstract.
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual CommandLineArgs CommandLineArgs
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.commandLineArgs);
			}
		}

		/// <summary></summary>
		public virtual MainLaunchArgs LaunchArgs
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.launchArgs);
			}
		}

		/// <summary></summary>
		public virtual Guid Guid
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.guid);
			}
		}

		/// <summary>
		/// The indicated name, i.e. <see cref="ApplicationEx.ProductName"/>.
		/// </summary>
		public virtual string IndicatedName
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (ApplicationEx.ProductName); // "YAT" or "YATConsole" shall be indicated in main title bar.
			}
		}

		/// <summary></summary>
		public virtual string Caption
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.workspace != null)
					return (this.workspace.Caption);
				else
					return (IndicatedName); // "YAT" or "YATConsole" shall be indicated in main title bar.
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
		/// Returns interface between main and script host; or <c>null</c> if script host is not ready yet.
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

		#region Launch
		//==========================================================================================
		// Launch
		//==========================================================================================

		/// <summary>
		/// This method is used to reset the command line arguments to launch with default values.
		/// </summary>
		public virtual void ResetCommandLineArgs()
		{
			this.commandLineArgs = new CommandLineArgs(ArgsDefault.Empty);
		}

		/// <summary>
		/// This method is used to test the command line argument processing.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		public virtual MainResult PrepareLaunch_ForTestOnly()
		{
			AssertUndisposed();

			if (ProcessCommandLineArgsIntoLaunchRequests())
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
		/// <remarks>
		/// Using term "launch" rather than "start" for distinction with "start/stop" I/O.
		/// </remarks>
		/// <returns>
		/// Returns <c>true</c> if either operation succeeded; otherwise, <c>false</c>.
		/// </returns>
		public virtual MainResult Launch()
		{
			AssertUndisposed();

			DebugMessage("Launching...");

			// Process command line args into launch requests:
			if (!ProcessCommandLineArgsIntoLaunchRequests())
			{
				return (MainResult.CommandLineError);
			}

			// Checks:
			if (ApplicationSettings.RoamingUserSettings.Font.CheckAvailability)
			{
				bool check = true;
				bool cancelLaunch = CheckFontAvailabilityAndPotentiallyCancelLaunch(ref check);

				if (!check)
				{
					ApplicationSettings.RoamingUserSettings.Font.CheckAvailability = false;
					ApplicationSettings.SaveRoamingUserSettings();
				}

				if (cancelLaunch)
				{
					return (MainResult.ApplicationLaunchCancel);
				}
			}

			// Application will launch, hence initialize required resources:
			ProcessorLoad.Initialize();
		#if (WITH_SCRIPTING)
			this.scriptBridge = new ScriptBridge(this, this.launchArgs.ScriptRunIsRequested);
		#endif

			// Start according to the launch requests:
			bool success = false;

			bool workspaceIsRequested = (this.launchArgs.WorkspaceSettingsHandler != null);
			bool terminalIsRequested  = (this.launchArgs.TerminalSettingsHandler  != null);

			if (workspaceIsRequested || terminalIsRequested)
			{
				if (workspaceIsRequested && terminalIsRequested)
				{
					success = OpenWorkspaceFromSettings(this.launchArgs.WorkspaceSettingsHandler, this.launchArgs.RequestedDynamicTerminalId, this.launchArgs.RequestedFixedTerminalId, this.launchArgs.TerminalSettingsHandler);
				}
				else if (workspaceIsRequested) // Workspace only:
				{
					success = OpenWorkspaceFromSettings(this.launchArgs.WorkspaceSettingsHandler);
				}
				else // Terminal only:
				{
					success = OpenTerminalFromSettings(this.launchArgs.TerminalSettingsHandler);
				}

				// Note that existing auto workspace settings are kept as they are, such they can
				// again be used at the next 'normal' (i.e. without command line args) launch of YAT.
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

							var errorMessage = Utilities.MessageHelper.ComposeMessage
							(
								"Unable to open the previous workspace file", // Neither '.' nor '!' shall be appended, the file path will be.
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
								return (MainResult.ApplicationLaunchCancel);
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
				this.workspace.LaunchAllTerminals(); // Don't care about success, workspace itself is fine.
			}

			// If requested, trigger operation:
			if (success && this.launchArgs.PerformOperationOnRequestedTerminal)
			{
				OnFixedStatusTextRequest("Triggering operation...");
				StartOperationTimer();
			}

			if (success)
			{
				OnLaunched();
				return (MainResult.Success);
			}
			else
			{
				return (MainResult.ApplicationLaunchError);
			}
		}

		#region Launch > Private Methods
		//------------------------------------------------------------------------------------------
		// Launch > Private Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Process the command line arguments according to their priority and translate them into
		/// launch requests.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Agree, could be refactored. Could be.")]
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:SingleLineCommentMustBePrecededByBlankLine", Justification = "Consistent section titles.")]
		private bool ProcessCommandLineArgsIntoLaunchRequests()
		{
			// Always create launch requests to ensure that object exists.
			this.launchArgs = new MainLaunchArgs();

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
					this.launchArgs.ShowNewTerminalDialog = true;
					this.launchArgs.KeepOpen              = true;
					this.launchArgs.KeepOpenOnNonSuccess  = true;

					return (true);
				}
				// In case of "YATConsole.exe" the application will set the 'NonInteractive' option,
				// i.e. 'Interactive' will be cleared, and no 'New Terminal' dialog shall get shown:
				else
				{
					this.launchArgs.ShowNewTerminalDialog = this.commandLineArgs.Interactive;
					this.launchArgs.KeepOpen              = true;
					this.launchArgs.KeepOpenOnNonSuccess  = true;

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
				this.launchArgs.ShowNewTerminalDialog = false;
				this.launchArgs.KeepOpen              = this.commandLineArgs.KeepOpen;
				this.launchArgs.KeepOpenOnNonSuccess  = this.commandLineArgs.KeepOpenOnNonSuccess;

				return (true);
			}

			// Arguments are available and valid, transfer 'NonInteractive' option:
			this.launchArgs.NonInteractive = this.commandLineArgs.NonInteractive;

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
						this.launchArgs.WorkspaceSettingsHandler = sh;
					}
					else
					{                                                                                                        // Neither '.' nor '!' shall be appended, the file path will be.
						this.commandLineArgs.Invalidate(Utilities.MessageHelper.ComposeMessage("Unable to open workspace file", absoluteFilePath, ex));
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
						this.launchArgs.TerminalSettingsHandler = sh;
					}
					else
					{                                                                                                       // Neither '.' nor '!' shall be appended, the file path will be.
						this.commandLineArgs.Invalidate(Utilities.MessageHelper.ComposeMessage("Unable to open terminal file", absoluteFilePath, ex));
						return (false);
					}
				}
			#if (!WITH_SCRIPTING)
				else
				{
					return (false);
				}
			#else
				else //  incl. ...Helper.IsScriptFile(requestedFilePath) but not limited to, as any extension shall be usable as a script file.
				{
					var absoluteFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(requestedFilePath); // !string.IsNullOrEmpty() for sure, see further above.
					if (File.Exists(absoluteFilePath)) // Validate fully expanded absolute path...
					{
						this.launchArgs.RequestedScriptFilePath = requestedFilePath; // ...but keep given path for further processing, as path may e.g. be relative to the executable directory.
						this.launchArgs.ScriptRunIsRequested = true;

						string messageOnError;
						if (!ProcessCommandLineArgsIntoScriptLaunchOptions(absoluteFilePath, out messageOnError))
						{
							this.commandLineArgs.Invalidate(messageOnError);
							return (false);
						}
					}
					else
					{                                                                                                     // Neither '.' nor '!' shall be appended, the file path will be.
						this.commandLineArgs.Invalidate(Utilities.MessageHelper.ComposeMessage("Script file does not exist", absoluteFilePath));
						return (false);
					}
				}
			#endif
			}

			// Prio 7 = Retrieve the requested terminal within the workspace and validate it:
			Domain.IOType implicitIOType = Domain.IOType.Unknown;
			bool updateDependentSettings = false;
			if (this.launchArgs.WorkspaceSettingsHandler != null) // Applies to a terminal within a workspace.
			{
				if (this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count > 0)
				{
					var dynamicTerminalIdOptionIsGiven = this.commandLineArgs.OptionIsGiven("DynamicTerminalId");
					var fixedTerminalIdOptionIsGiven   = this.commandLineArgs.OptionIsGiven("FixedTerminalId");

					var requestedDynamicTerminalId = this.commandLineArgs.RequestedDynamicTerminalId;
					var requestedFixedTerminalId   = this.commandLineArgs.RequestedFixedTerminalId;

					// Prevent non-matching combination:
					if (dynamicTerminalIdOptionIsGiven && fixedTerminalIdOptionIsGiven)
					{
						if      ((requestedDynamicTerminalId == TerminalIds.ActiveDynamicId) &&
						         (requestedFixedTerminalId   == TerminalIds.ActiveDynamicId)) {
							// OK, both refer to the active terminal.
						}
						else if ((requestedDynamicTerminalId == TerminalIds.InvalidDynamicId) &&
						         (requestedFixedTerminalId   == TerminalIds.InvalidDynamicId)) {
							// OK, both refer to no terminal, i.e. disabled the operation.
						}
						else {
							this.commandLineArgs.Invalidate(string.Format(CultureInfo.CurrentCulture, "If dynamic as well as fixed terminal ID is given, both IDs must either be 0 (active) or -1 (invalid), but dynamic ID = {0} and fixed ID = {1}", requestedDynamicTerminalId, requestedFixedTerminalId));
							return (false);
						}
					}

					string terminalFilePathOfGivenId = null;

					if (dynamicTerminalIdOptionIsGiven)
					{
						int lastDynamicId = TerminalIds.IndexToDynamicId(this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count - 1);

						if     ((           requestedDynamicTerminalId >= TerminalIds.FirstDynamicId) && (requestedDynamicTerminalId <= lastDynamicId)) {
							this.launchArgs.RequestedDynamicTerminalId = requestedDynamicTerminalId;

							// Invalidate 'the other':
							                requestedFixedTerminalId = TerminalIds.InvalidFixedId; // Required to skip processing further below.
							this.launchArgs.RequestedFixedTerminalId = TerminalIds.InvalidFixedId;
						}
						else if (           requestedDynamicTerminalId == TerminalIds.ActiveDynamicId) {
							this.launchArgs.RequestedDynamicTerminalId = TerminalIds.ActiveDynamicId;
						}
						else if (           requestedDynamicTerminalId == TerminalIds.InvalidDynamicId) {
							this.launchArgs.RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Usable to disable the operation.
						}
						else {
							this.commandLineArgs.Invalidate(string.Format(CultureInfo.CurrentCulture, "A terminal with dynamic ID = {0} is not available", requestedDynamicTerminalId));
							return (false);
						}

						if (this.launchArgs.RequestedDynamicTerminalId != TerminalIds.InvalidDynamicId)
						{
							if (this.launchArgs.RequestedDynamicTerminalId == TerminalIds.ActiveDynamicId) // The active terminal is located last in the collection:
								terminalFilePathOfGivenId = this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings[this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count - 1].FilePath;
							else
								terminalFilePathOfGivenId = this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings[TerminalIds.DynamicIdToIndex(this.launchArgs.RequestedDynamicTerminalId)].FilePath;
						}
					}

					if (fixedTerminalIdOptionIsGiven)
					{
						if       (          requestedFixedTerminalId == TerminalIds.InvalidFixedId) {
							this.launchArgs.RequestedFixedTerminalId = TerminalIds.InvalidFixedId; // Usable to disable the operation.
						}
						else if (           requestedFixedTerminalId == TerminalIds.ActiveFixedId) {
							this.launchArgs.RequestedFixedTerminalId = TerminalIds.ActiveFixedId; // The active terminal is located last in the collection:
							terminalFilePathOfGivenId = this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings[this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings.Count - 1].FilePath;
						}
						else {
							foreach (var terminal in this.launchArgs.WorkspaceSettingsHandler.Settings.TerminalSettings)
							{
								if (terminal.FixedId == requestedFixedTerminalId)
								{
									this.launchArgs.RequestedFixedTerminalId = requestedFixedTerminalId;
									terminalFilePathOfGivenId = terminal.FilePath;
									break;
								}
							}

							if (string.IsNullOrEmpty(terminalFilePathOfGivenId))
							{
								this.commandLineArgs.Invalidate(string.Format(CultureInfo.CurrentCulture, "A terminal with fixed ID = {0} is not available", requestedFixedTerminalId));
								return (false);
							}

							// Invalidate 'the other':
							                requestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Consistency with processing further above.
							this.launchArgs.RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId;
						}
					}

					if (!string.IsNullOrEmpty(terminalFilePathOfGivenId))
					{
						string workspaceFilePath = this.launchArgs.WorkspaceSettingsHandler.SettingsFilePath;
						DocumentSettingsHandler<TerminalSettingsRoot> sh;
						Exception exceptionOnFailure;
						if (OpenTerminalFile(workspaceFilePath, terminalFilePathOfGivenId, out sh, out exceptionOnFailure))
						{
							this.launchArgs.TerminalSettingsHandler = sh;
						}
						else
						{
							this.commandLineArgs.Invalidate(exceptionOnFailure.Message);
							return (false);
						}
					}
				}
			}
			else if (this.launchArgs.TerminalSettingsHandler != null) // Applies to a dedicated terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalId == TerminalIds.InvalidDynamicId)
					this.launchArgs     .RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Usable to disable the operation.

				if (this.commandLineArgs.RequestedFixedTerminalId == TerminalIds.InvalidFixedId)
					this.launchArgs     .RequestedFixedTerminalId = TerminalIds.InvalidFixedId; // Usable to disable the operation.
			}
			else if (this.commandLineArgs.NewIsRequested) // Results in a new terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalId == TerminalIds.InvalidDynamicId)
					this.launchArgs     .RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Usable to disable the operation.

				if (this.commandLineArgs.RequestedFixedTerminalId == TerminalIds.InvalidFixedId)
					this.launchArgs     .RequestedFixedTerminalId = TerminalIds.InvalidFixedId; // Usable to disable the operation.

				this.launchArgs.TerminalSettingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();
			}
			else if (this.commandLineArgs.NewIsRequestedImplicitly(out implicitIOType)) // Also results in a new terminal.
			{
				if (this.commandLineArgs.RequestedDynamicTerminalId == TerminalIds.InvalidDynamicId)
					this.launchArgs     .RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Usable to disable the operation.

				if (this.commandLineArgs.RequestedFixedTerminalId == TerminalIds.InvalidFixedId)
					this.launchArgs     .RequestedFixedTerminalId = TerminalIds.InvalidFixedId; // Usable to disable the operation.

				this.launchArgs.TerminalSettingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();

				if (implicitIOType != Domain.IOType.Unknown)
					this.launchArgs.TerminalSettingsHandler.Settings.IOType = implicitIOType;

				updateDependentSettings = true; // Same as when using the on the 'New Terminal' or 'Terminal Settings'
			}                                   // dialog, dependent settings must be updated accordingly. But this
			else                                // must happen AFTER having processed the args into the settings!
			{
				this.launchArgs.RequestedDynamicTerminalId = TerminalIds.InvalidDynamicId; // Disable the operation in any case.
				this.launchArgs.RequestedFixedTerminalId   = TerminalIds.InvalidFixedId;   // Disable the operation in any case.
			}

			// Prio 8 = Override explicit settings as desired:
			if (this.launchArgs.TerminalSettingsHandler != null) // Applies to a dedicated terminal.
			{
				if (!ProcessCommandLineArgsIntoExistingTerminalSettings(this.launchArgs.TerminalSettingsHandler.Settings.Terminal))
					return (false);

				if (updateDependentSettings) // See comments further above.
					this.launchArgs.TerminalSettingsHandler.Settings.Terminal.UpdateAllDependentSettings();

				if (this.commandLineArgs.OptionIsGiven("StartTerminal"))
					this.launchArgs.TerminalSettingsHandler.Settings.TerminalIsStarted = this.commandLineArgs.StartTerminal;

				if (this.commandLineArgs.OptionIsGiven("LogOn"))
					this.launchArgs.TerminalSettingsHandler.Settings.LogIsOn           = this.commandLineArgs.LogOn;
			}

			// Prio 9 = Override overall settings as desired:
			if (this.commandLineArgs.OptionIsGiven("StartTerminal"))
				this.launchArgs.Override.StartTerminal       = this.commandLineArgs.StartTerminal;

			if (this.commandLineArgs.OptionIsGiven("KeepTerminalStopped"))
				this.launchArgs.Override.KeepTerminalStopped = this.commandLineArgs.KeepTerminalStopped;

			if (this.commandLineArgs.OptionIsGiven("LogOn"))
				this.launchArgs.Override.LogOn               = this.commandLineArgs.LogOn;

			if (this.commandLineArgs.OptionIsGiven("KeepLogOff"))
				this.launchArgs.Override.KeepLogOff          = this.commandLineArgs.KeepLogOff;

			// Prio 10 = Perform requested operation:
			if ((this.launchArgs.RequestedDynamicTerminalId != TerminalIds.InvalidDynamicId) ||
			    (this.launchArgs.RequestedFixedTerminalId   != TerminalIds.InvalidFixedId))
			{
				if (this.commandLineArgs.OptionIsGiven("TransmitText"))
				{
					var text = this.commandLineArgs.RequestedTransmitText;
					string messageOnFailure;
					if (Domain.Utilities.ValidationHelper.ValidateText("TransmitText", text, out messageOnFailure, Domain.Parser.Mode.Default))
					{
						this.launchArgs.RequestedTransmitText = text;
						this.launchArgs.PerformOperationOnRequestedTerminal = true;
					}
					else
					{
						this.commandLineArgs.Invalidate(messageOnFailure);
						return (false);
					}
				}
				else if (this.commandLineArgs.OptionIsGiven("TransmitFile"))
				{
					var filePath = this.commandLineArgs.RequestedTransmitFilePath;
					if (!string.IsNullOrEmpty(filePath))
					{
						var absoluteFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(filePath);
						if (File.Exists(absoluteFilePath)) // Validate fully expanded absolute path...
						{
							this.launchArgs.RequestedTransmitFilePath = filePath; // ...but keep given path for further processing, as path may e.g. be relative to the requested terminal.
							this.launchArgs.PerformOperationOnRequestedTerminal = true;
						}
						else
						{                                                                                                          // Neither '.' nor '!' shall be appended, the file path will be.
							this.commandLineArgs.Invalidate(Utilities.MessageHelper.ComposeMessage("File to transmit does not exist", absoluteFilePath));
							return (false);
						}
					}
					else
					{
						this.commandLineArgs.Invalidate("File to transmit is undefined!");
						return (false);
					}
				}
			}

		#if (WITH_SCRIPTING)
			// Prio 11 = Run script:
			if (this.commandLineArgs.OptionIsGiven("Script"))
			{
				var filePath = this.commandLineArgs.RequestedScriptFilePath;
				if (!string.IsNullOrEmpty(filePath))
				{
					var absoluteFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(filePath);
					if (File.Exists(absoluteFilePath)) // Validate fully expanded absolute path...
					{
						this.launchArgs.RequestedScriptFilePath = filePath; // ...but keep given path for further processing, as path may e.g. be relative to the requested executable directory.
						this.launchArgs.ScriptRunIsRequested = true;

						string messageOnFailure;
						if (!ProcessCommandLineArgsIntoScriptLaunchOptions(absoluteFilePath, out messageOnFailure))
						{
							this.commandLineArgs.Invalidate(messageOnFailure);
							return (false);
						}
					}
					else
					{                                                                                                     // Neither '.' nor '!' shall be appended, the file path will be.
						this.commandLineArgs.Invalidate(Utilities.MessageHelper.ComposeMessage("Script file does not exist", absoluteFilePath));
						return (false);
					}
				}
				else
				{
					this.commandLineArgs.Invalidate("Script file path is undefined!");
					return (false);
				}
			}
		#endif

			// Prio 12 = Set behavior:
			if (this.launchArgs.PerformOperationOnRequestedTerminal)
			{
				this.launchArgs.OperationDelay = this.commandLineArgs.OperationDelay;
				this.launchArgs.ExitDelay      = this.commandLineArgs.ExitDelay;
			}

		#if (!WITH_SCRIPTING)
			if (this.launchArgs.PerformOperationOnRequestedTerminal)
		#else
			if (this.launchArgs.PerformOperationOnRequestedTerminal || this.launchArgs.ScriptRunIsRequested)
		#endif
			{
				this.launchArgs.KeepOpen             = this.commandLineArgs.KeepOpen;
				this.launchArgs.KeepOpenOnNonSuccess = this.commandLineArgs.KeepOpenOnNonSuccess;
			}
			else
			{
				this.launchArgs.KeepOpen             = true;
				this.launchArgs.KeepOpenOnNonSuccess = true;
			}

			// Prio 13 = Set layout:
			this.launchArgs.TileHorizontal = this.commandLineArgs.TileHorizontal;
			this.launchArgs.TileVertical   = this.commandLineArgs.TileVertical;

			return (true);
		}

	#if (WITH_SCRIPTING)

		private bool ProcessCommandLineArgsIntoScriptLaunchOptions(string absoluteScriptFilePath, out string messageOnFailure)
		{
			if (this.commandLineArgs.OptionIsGiven("ScriptLog"))
			{
				var filePath = this.commandLineArgs.RequestedScriptLogFilePath;
				var rootDirectory = Path.GetDirectoryName(absoluteScriptFilePath);
				var absoluteFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(rootDirectory, filePath);
				if (!PathEx.IsValid(absoluteFilePath)) // Validate fully expanded absolute path...
				{                                                                                          // Neither '.' nor '!' shall be appended, the file path will be.
					messageOnFailure = Utilities.MessageHelper.ComposeMessage("Invalid script log file path", absoluteFilePath);
					return (false);
				}

				this.launchArgs.RequestedScriptLogFilePath = filePath; // ...but keep given path for further processing, as path may e.g. be relative to the requested executable directory.
			}

			if (this.commandLineArgs.OptionIsGiven("ScriptLogTimeStamp"))
				this.launchArgs.AppendTimeStampToScriptLogFileName = true;

			if (this.commandLineArgs.OptionIsGiven("ScriptArgs"))
				this.launchArgs.RequestedScriptArgs = this.commandLineArgs.RequestedScriptArgs;

			messageOnFailure = null;
			return (true);
		}

	#endif

		/// <summary>
		/// This method takes existing terminal settings and modifies/overrides those settings that
		/// are given by the given command line args.
		/// </summary>
		/// <remarks>
		/// Unfortunately, 'normal' terminal settings and new terminal settings are defined rather
		/// differently. Therefore, this implementation looks a bit weird.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Agree, could be refactored. Could be.")]
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

			if (this.commandLineArgs.OptionIsGiven("IOType"))
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
						terminalSettings.IO.SerialPort.AliveMonitor = new MKY.IO.Serial.IntervalSettingTuple(false, 0);
					else if (this.commandLineArgs.SerialPortAliveMonitor >= MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorMinInterval)
						terminalSettings.IO.SerialPort.AliveMonitor = new MKY.IO.Serial.IntervalSettingTuple(true, this.commandLineArgs.SerialPortAliveMonitor);
					else
						return (false);
				}
				if (this.commandLineArgs.OptionIsGiven("SerialPortAutoReopen"))
				{
					if (this.commandLineArgs.SerialPortAutoReopen == 0)
						terminalSettings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.IntervalSettingTuple(false, 0);
					else if (this.commandLineArgs.SerialPortAutoReopen >= MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenMinInterval)
						terminalSettings.IO.SerialPort.AutoReopen = new MKY.IO.Serial.IntervalSettingTuple(true, this.commandLineArgs.SerialPortAutoReopen);
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
						terminalSettings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.IntervalSettingTuple(false, 0);
					else if (this.commandLineArgs.TcpAutoReconnect >= MKY.IO.Serial.Socket.SocketSettings.TcpClientAutoReconnectMinInterval)
						terminalSettings.IO.Socket.TcpClientAutoReconnect = new MKY.IO.Serial.IntervalSettingTuple(true, this.commandLineArgs.TcpAutoReconnect);
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

						// The usage is optional:
						int usagePage = MKY.IO.Usb.HidDeviceInfo.AnyUsagePage;
						if (this.commandLineArgs.OptionIsGiven("UsagePage"))
						{
							if (!int.TryParse(this.commandLineArgs.UsagePage, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out usagePage))
								return (false);

							if (!MKY.IO.Usb.HidDeviceInfo.IsValidUsagePageOrAny(usagePage))
								return (false);
						}

						int usageId = MKY.IO.Usb.HidDeviceInfo.AnyUsageId;
						if (this.commandLineArgs.OptionIsGiven("UsageId"))
						{
							if (!int.TryParse(this.commandLineArgs.UsageId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out usageId))
								return (false);

							if (!MKY.IO.Usb.HidDeviceInfo.IsValidUsageIdOrAny(usageId))
								return (false);
						}

						// The SNR is optional:
						if (this.commandLineArgs.OptionIsGiven("SerialString"))
						{
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.HidDeviceInfo(vendorId, productId, this.commandLineArgs.SerialString, usagePage, usageId);
							terminalSettings.IO.UsbSerialHidDevice.MatchSerial = true; // Command line option shall override 'ApplicationSettings.LocalUserSettings.General.MatchUsbSerial'.
						}
						else
						{
							terminalSettings.IO.UsbSerialHidDevice.DeviceInfo = new MKY.IO.Usb.HidDeviceInfo(vendorId, productId, usagePage, usageId);
							terminalSettings.IO.UsbSerialHidDevice.MatchSerial = false; // Command line option shall override 'ApplicationSettings.LocalUserSettings.General.MatchUsbSerial'.
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Setting is required to be received, modified and returned.")]
		protected virtual bool CheckFontAvailabilityAndPotentiallyCancelLaunch(ref bool check)
		{
			var fontName = Format.Types.FontFormat.NameDefault;
			var fontSize = Format.Types.FontFormat.SizeDefault;
			var fontOK = false;
			var retry = false;

			do
			{
				Exception exceptionOnFailure;
				fontOK = FontEx.TryGet(fontName, fontSize, FontStyle.Regular, out exceptionOnFailure);
				if (!fontOK)
				{
					StringBuilder text;
					List<LinkLabel.Link> links;
					Utilities.MessageHelper.MakeMissingFontMessage(fontName, exceptionOnFailure, out text, out links);
					text.AppendLine();
					text.AppendLine();
					text.Append("[Abort] to exit " + ApplicationEx.CommonName + ". ");
					text.Append("[Retry] to check availability again (e.g. after having installed the font). ");
					text.Append("[Ignore] to launch " + ApplicationEx.CommonName + " nevertheless.");

					var dr = OnExtendedMessageInputRequest
					(
						text.ToString(),
						links,
						"Default Font Not Available",
						"On startup, check that 'Deja Vu Sans Mono' font is available.", // Unusual period, making
						ref check,                                                       // setting text consistent
						MessageBoxButtons.AbortRetryIgnore,                              // with message text for
						MessageBoxIcon.Exclamation                                       // this dialog.
					);

					switch (dr)
					{
						case DialogResult.Abort:
							return (true);

						case DialogResult.Retry:
							retry = true;
							break;

						case DialogResult.Ignore:
						case DialogResult.Cancel: // This is the case when closing the dialog by [x]. Canceling launch seems inappropriate for this case.
							retry = false;
							break;

						default:
							throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The dialog result must be 'AbortRetryIgnore'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
			while (retry);

			return (false);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected virtual void CleanupLocalUserDirectory()
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

			if (ExtensionHelper.IsWorkspaceFile(filePath))
			{
				if (OpenWorkspaceFromFile(filePath))
				{
					this.workspace.LaunchAllTerminals(); // Don't care about success, workspace itself is fine.

					OnLaunched(); // Same as at OpenTerminalFromFile() below.
					return (true);
				}

				return (false);
			}
			else if (ExtensionHelper.IsTerminalFile(filePath))
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
					if (this.workspace.ActiveTerminal.Launch())
					{
						if (signalStarted)
							OnLaunched(); // Same as at OpenWorkspaceFromFile() above.

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

		/// <summary></summary>
		public virtual bool OpenRecent(int userIndex)
		{
			AssertUndisposed();

			return (OpenFromFile(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[userIndex - 1].Item));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Recents", Justification = "Multiple items.")]
		public virtual void ClearRecents()
		{
			AssertUndisposed();

			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Clear();
			ApplicationSettings.LocalUserSettings.RecentFiles.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveLocalUserSettings();
		}

		#endregion

		#region Exit
		//==========================================================================================
		// Exit
		//==========================================================================================

		/// <summary>
		/// So far, testing solely deals with <see cref="ExitMode.Manual"/>, i.e. what a user would
		/// be doing. Thus, providing this method for simplicity and obviousness.
		/// </summary>
		/// <remarks>
		/// Note that it is not possible to mark a void-method with [Conditional("TEST")].
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize purpose of this method.")]
		public virtual MainResult Exit_ForTestOnly()
		{
			return (Exit(ExitMode.Manual));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Exit", Justification = "Exit() as method name is the obvious name and should be OK for other languages, .NET itself uses it in Application.Exit().")]
		public virtual MainResult Exit(ExitMode exitMode)
		{
			bool cancel;
			return (Exit(exitMode, out cancel));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Exit", Justification = "Exit() as method name is the obvious name and should be OK for other languages, .NET itself uses it in Application.Exit().")]
		public virtual MainResult Exit(ExitMode exitMode, out bool cancel)
		{
		#if (WITH_SCRIPTING)

			bool scriptSuccess = true;
			if (this.scriptBridge != null)
			{
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
						this.scriptBridge.BreakScript(userInteractionIsAllowed: true); // Interaction is OK as dialog has just been shown anyway.
					else
						scriptSuccess = false;
				}

				if (scriptSuccess)
					this.scriptBridge.ProcessCommand("Script:CloseDialog");
			}

		#endif // WITH_SCRIPTING

			bool workspaceSuccess;
			if (this.workspace != null)
				workspaceSuccess = this.workspace.CloseWithOptions(true, exitMode);
			else
				workspaceSuccess = true;

		#if (!WITH_SCRIPTING)
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
				// "View.Main" receives the event callback!
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
						var portName = t.IOSerialPortName;
						if (!string.IsNullOrEmpty(portName))
						{
							string inUseText; // Attention: Same texts are used in YAT.View.Forms.TerminalSettings.SetControls().
							if (t.IsOpen)     //            Changes below likely have to be applied there too.
								inUseText = "(in use by " + t.IndicatedName + ")";
							else
								inUseText = "(selected by " + t.IndicatedName + ")";

							inUseLookup.Add(new MKY.IO.Ports.InUseInfo(t.SequentialId, portName, t.IsOpen, inUseText));
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

			OnWorkspaceSaved(this.workspace);
		}

		/// <remarks>
		/// See remarks of <see cref="Workspace.CloseWithOptions"/> for details on why this handler
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
			Exit(LaunchArgs.IsAutoExit ? ExitMode.Auto : ExitMode.Manual);
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

			this.workspace = new Workspace(this.launchArgs.ToWorkspaceLaunchArgs());
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

			string messageOnFailure;
			if (OpenWorkspaceFromFile(filePath, out messageOnFailure))
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
					messageOnFailure,
					"Workspace File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
				);
				OnTimedStatusTextRequest("Workspace not opened!");

				return (false);
			}
		}

		/// <summary></summary>
		private bool OpenWorkspaceFromFile(string filePath, out string messageOnFailure)
		{
			string absoluteFilePath;
			DocumentSettingsHandler<WorkspaceSettingsRoot> sh;
			Guid guid;
			Exception ex;
			if (OpenWorkspaceFile(filePath, out absoluteFilePath, out sh, out guid, out ex))
			{
				if (OpenWorkspaceFromSettings(sh, guid, out ex))
				{
					messageOnFailure = null;
					return (true);
				}
				else
				{                                                                                      // Neither '.' nor '!' shall be appended, the file path will be.
					messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open workspace", absoluteFilePath, ex);
					return (false);
				}
			}
			else
			{                                                                                           // Neither '.' nor '!' shall be appended, the file path will be.
				messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open workspace file", absoluteFilePath, ex);
				return (false);
			}
		}

		/// <summary></summary>
		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
		{
			Exception exceptionOnFailure;
			return (OpenWorkspaceFromSettings(settingsHandler, Guid.NewGuid(), out exceptionOnFailure));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, int dynamicTerminalIdToReplace, int fixedTerminalIdToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
		{
			Exception exceptionOnFailure;
			return (OpenWorkspaceFromSettings(settingsHandler, Guid.NewGuid(), dynamicTerminalIdToReplace, fixedTerminalIdToReplace, terminalSettingsToReplace, out exceptionOnFailure));
		}

		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid, out Exception exceptionOnFailure)
		{
			return (OpenWorkspaceFromSettings(settingsHandler, guid, TerminalIds.InvalidDynamicId, TerminalIds.InvalidFixedId, null, out exceptionOnFailure));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenWorkspaceFromSettings(DocumentSettingsHandler<WorkspaceSettingsRoot> settings, Guid guid, int dynamicTerminalIdToReplace, int fixedTerminalIdToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace, out Exception exceptionOnFailure)
		{
			AssertUndisposed();

			// Ensure that the workspace file is not already open:
			if (!CheckWorkspaceFile(settings.SettingsFilePath))
			{
				exceptionOnFailure = null;
				return (false);
			}

			// Close workspace, only one workspace can exist within the application:
			if (!CloseWorkspace())
			{
				exceptionOnFailure = null;
				return (false);
			}

			// Create new workspace:
			OnFixedStatusTextRequest("Opening workspace...");

			Workspace w;
			try
			{
				w = new Workspace(this.launchArgs.ToWorkspaceLaunchArgs(), settings, guid);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open workspace from settings!");
				exceptionOnFailure = ex;
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
			int terminalCount = this.workspace.OpenTerminals(dynamicTerminalIdToReplace, fixedTerminalIdToReplace, terminalSettingsToReplace);
			if (terminalCount == 1)
				OnTimedStatusTextRequest("Workspace terminal opened.");
			else if (terminalCount > 1)
				OnTimedStatusTextRequest("Workspace terminals opened.");
			else
				OnTimedStatusTextRequest("Workspace contains no terminal to open.");

			exceptionOnFailure = null;
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

		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
		private bool OpenWorkspaceFile(string filePath, out string absoluteFilePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, out Exception exceptionOnFailure)
		{
			Guid guid;
			return (OpenWorkspaceFile(filePath, out absoluteFilePath, out settingsHandler, out guid, out exceptionOnFailure));
		}

		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenWorkspaceFile(string filePath, out string absoluteFilePath, out DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, out Guid guid, out Exception exceptionOnFailure)
		{
			absoluteFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(filePath);

			try
			{
				var sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
				sh.SettingsFilePath = absoluteFilePath;
				if (sh.Load())
				{
					settingsHandler = sh;

					// Try to retrieve GUID from file path (in case of auto saved workspace files):
					if (!GuidEx.TryParseCommonTolerantly(Path.GetFileNameWithoutExtension(filePath), out guid))
						guid = Guid.NewGuid();

					exceptionOnFailure = null;
					return (true);
				}
				else
				{
					settingsHandler = null;
					guid = Guid.Empty;
					exceptionOnFailure = null;
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open workspace file!");

				settingsHandler = null;
				guid = Guid.Empty;
				exceptionOnFailure = ex;
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
		/// <exception cref="ArgumentNullException"><paramref name="terminalFilePath"/> is null.</exception>
		private bool OpenTerminalFile(string terminalFilePath, out string absoluteTerminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exceptionOnFailure)
		{
			return (OpenTerminalFile("", terminalFilePath, out absoluteTerminalFilePath, out settingsHandler, out exceptionOnFailure));
		}

		/// <remarks>Needed for opening command line requested terminal files without yet creating a workspace.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="terminalFilePath"/> is null.</exception>
		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exceptionOnFailure)
		{
			string absoluteTerminalFilePath;
			return (OpenTerminalFile(workspaceFilePath, terminalFilePath, out absoluteTerminalFilePath, out settingsHandler, out exceptionOnFailure));
		}

		/// <remarks>Needed for opening command line requested terminal files without yet creating a workspace.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="terminalFilePath"/> is null.</exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenTerminalFile(string workspaceFilePath, string terminalFilePath, out string absoluteTerminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exceptionOnFailure)
		{
			// Attention:
			// Similar code exists in Workspace.OpenTerminalFile().
			// Changes here may have to be applied there too.

			absoluteTerminalFilePath = null;

			// Try to combine workspace and terminal path:
			if (!string.IsNullOrEmpty(workspaceFilePath))
				absoluteTerminalFilePath = PathEx.CombineFilePaths(PathEx.GetNormalizedRootedExpandingEnvironmentVariables(workspaceFilePath), terminalFilePath);

			// Alternatively, try to use terminal file path only:
			if (string.IsNullOrEmpty(absoluteTerminalFilePath))
				absoluteTerminalFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(terminalFilePath);

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
					exceptionOnFailure = null;
					return (true);
				}
				else
				{
					settingsHandler = null;
					exceptionOnFailure = null;
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open terminal file!");

				settingsHandler = null;
				exceptionOnFailure = ex;
				return (false);
			}
		}

		#endregion

		#endregion

		#region PerformOperation/Exit
		//------------------------------------------------------------------------------------------
		// PerformOperation/Exit
		//------------------------------------------------------------------------------------------

		private void StartOperationTimer()
		{
			var dueTime = OperationAndExitTimerInterval;
			var period  = OperationAndExitTimerInterval; // Periodic!

			lock (this.operationTimerSyncObj)
			{
				if (this.operationTimer == null)
					this.operationTimer = new System.Threading.Timer(new TimerCallback(operationTimer_Periodic_Elapsed), null, dueTime, period);
				else
					this.operationTimer.Change(dueTime, period);
			}
		}

		private void StopAndDisposeOperationTimer()
		{
			lock (this.operationTimerSyncObj)
			{
				if (this.operationTimer != null)
				{
					this.operationTimer.Dispose();
					this.operationTimer = null;
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private readonly object operationTimer_Periodic_Elapsed_SyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void operationTimer_Periodic_Elapsed(object obj)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(operationTimer_Periodic_Elapsed_SyncObj))
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

						StartExitTimerIfNeeded(false);
						return;
					}

					Terminal requestedTerminal;
					string requestedTerminalIdText;

					if (this.launchArgs.RequestedDynamicTerminalId != TerminalIds.InvalidDynamicId)
					{
						var id = this.launchArgs.RequestedDynamicTerminalId;
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

							StartExitTimerIfNeeded(false);
							return;
						}

						requestedTerminalIdText = string.Format(CultureInfo.CurrentCulture, "Dyn#{0}", id);
					}
					else if (this.launchArgs.RequestedFixedTerminalId != TerminalIds.InvalidFixedId)
					{
						var id = this.launchArgs.RequestedFixedTerminalId;
						if (!this.workspace.TryGetTerminalByFixedId(id, out requestedTerminal))
						{
							StopAndDisposeOperationTimer();

							OnFixedStatusTextRequest("Invalid requested fixed ID!");
							OnMessageInputRequest
							(
								"Invalid requested fixed ID " + id.ToString(CultureInfo.CurrentCulture) + " to perform the operation!" + Environment.NewLine + Environment.NewLine +
								"Check the command line arguments. See command line help for details.",
								"Invalid Terminal ID",
								MessageBoxButtons.OK,
								MessageBoxIcon.Stop
							);

							StartExitTimerIfNeeded(false);
							return;
						}

						requestedTerminalIdText = string.Format(CultureInfo.CurrentCulture, "Fix#{0}", id);
					}
					else
					{
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "An start operation can only be triggered if a terminal ID is given!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
						return; // Pend!                               Using term "Transmission" to indicate potential
					}           //                                     "intelligence" to send + receive/verify the data.

					// Preconditions fullfilled!
					StopAndDisposeOperationTimer();

					int delay = this.launchArgs.OperationDelay;
					if (delay > 0)
					{
						OnFixedStatusTextRequest("Operation triggered, delaying for " + delay.ToString(CultureInfo.CurrentCulture) + " ms...");
						Thread.Sleep(delay);
					}

					OnFixedStatusTextRequest("Operation triggered, preparing...");

					// Automatically transmit text if desired:
					string text = this.launchArgs.RequestedTransmitText;
					if (!string.IsNullOrEmpty(text))
					{
						bool success;
						try
						{
							OnFixedStatusTextRequest("Automatically transmitting text on terminal " + requestedTerminalIdText);
							requestedTerminal.SendText(text, addToRecents: false); // No explicit default radix available (yet).
							success = true;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Exception while trying to transmit text!");

							OnFixedStatusTextRequest("Unable to transmit text!");
							OnMessageInputRequest
							(                                                                  // Neither '.' nor '!' shall be appended, the text content will be.
								Utilities.MessageHelper.ComposeMessage("Unable to transmit text", text, ex),
								"Transmission Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Stop
							);
							success = false;
						}
						StartExitTimerIfNeeded(success);
					}

					// Automatically transmit file if desired:
					string filePath = this.launchArgs.RequestedTransmitFilePath;
					if (!string.IsNullOrEmpty(filePath))
					{
						bool success;
						try
						{
							OnFixedStatusTextRequest("Automatically transmitting file on terminal " + requestedTerminalIdText);
							requestedTerminal.SendFile(filePath, addToRecents: false); // No explicit default radix available (yet).
							success = true;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Exception while trying to transmit file!");

							OnFixedStatusTextRequest("Unable to transmit file!");
							OnMessageInputRequest
							(                                                                  // Neither '.' nor '!' shall be appended, the file path will be.
								Utilities.MessageHelper.ComposeMessage("Unable to transmit file", filePath, ex),
								"Transmission Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Stop
							);
							success = false;
						}
						StartExitTimerIfNeeded(success);
					}
				}
				finally
				{
					Monitor.Exit(operationTimer_Periodic_Elapsed_SyncObj);
				}
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("operationTimer_Elapsed() monitor has timed out, skipping this concurrent event.");
			}
		}

		private void StartExitTimerIfNeeded(bool operationSuccess)
		{
			if ((!this.launchArgs.KeepOpen) &&
			    (!(this.launchArgs.KeepOpenOnNonSuccess && !operationSuccess)))
			{
				if (operationSuccess)
					OnFixedStatusTextRequest("Operation successfully performed, triggering exit...");
				else
					OnFixedStatusTextRequest("No operation performed, triggering exit...");

				var dueTime = OperationAndExitTimerInterval;
				var period  = OperationAndExitTimerInterval; // Periodic!

				lock (this.exitTimerSyncObj)
				{
					if (this.exitTimer == null)
						this.exitTimer = new System.Threading.Timer(new TimerCallback(exitTimer_Periodic_Elapsed), null, dueTime, period);
					else
						this.exitTimer.Change(dueTime, period);
				}
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
			lock (this.exitTimerSyncObj)
			{
				if (this.exitTimer != null)
				{
					this.exitTimer.Dispose();
					this.exitTimer = null;
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private readonly object exitTimer_Periodic_Elapsed_SyncObj = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void exitTimer_Periodic_Elapsed(object obj)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(exitTimer_Periodic_Elapsed_SyncObj))
			{
				try
				{
					if (this.workspace != null)
					{
						Terminal requestedTerminal = null;

						if (this.launchArgs.RequestedDynamicTerminalId != TerminalIds.InvalidDynamicId)
						{
							int id = this.launchArgs.RequestedDynamicTerminalId;
							this.workspace.TryGetTerminalByDynamicId(id, out requestedTerminal);
						}

						if (this.launchArgs.RequestedFixedTerminalId != TerminalIds.InvalidFixedId)
						{
							int id = this.launchArgs.RequestedFixedTerminalId;
							this.workspace.TryGetTerminalByFixedId(id, out requestedTerminal);
						}

						if ((requestedTerminal != null) && (requestedTerminal.IsSending))
						{
							OnTimedStatusTextRequest("Exit triggered, pending while terminal is sending...");
							return; // Pend!
						}
					}

					// --- Precondition fullfilled, terminal is no longer busy. ---

					StopAndDisposeExitTimer();

					int delay = this.launchArgs.ExitDelay;
					if (delay > 0)
					{
						OnFixedStatusTextRequest("Exit triggered, delaying for " + delay.ToString(CultureInfo.CurrentCulture) + " ms...");
						Thread.Sleep(delay);
					}

					OnFixedStatusTextRequest("Exit triggered, preparing...");

					try
					{
						OnFixedStatusTextRequest("Automatically exiting " + ApplicationEx.ProductName + "..."); // "YAT" or "YATConsole", as indicated in main title bar.
						Exit(ExitMode.Auto);
					}
					catch (Exception ex)
					{
						DebugEx.WriteException(GetType(), ex, "Exception while trying to exit!");

						OnFixedStatusTextRequest("Unable to exit!");
						OnMessageInputRequest
						(
							Utilities.MessageHelper.ComposeMessage("Unable to exit!", ex),
							"Exit Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Stop
						);
					}
				}
				finally
				{
					Monitor.Exit(exitTimer_Periodic_Elapsed_SyncObj);
				}
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("exitTimer_Elapsed() monitor has timed out, skipping this concurrent event.");
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

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnWorkspaceSaved(Workspace workspace)
		{
			this.eventHelper.RaiseSync<EventArgs<Workspace>>(WorkspaceSaved, this, new EventArgs<Workspace>(workspace));
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
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			if (this.launchArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				var e = new MessageInputEventArgs(text, caption, buttons, icon, defaultButton);
				this.eventHelper.RaiseSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break(); // OK as conditional and disabled by default.
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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Setting is required to be received, modified and returned.")]
		protected virtual DialogResult OnExtendedMessageInputRequest(string text, string caption, string checkText, ref bool checkValue, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			return (OnExtendedMessageInputRequest(text, null, caption, checkText, ref checkValue, buttons, icon, defaultButton));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Setting is required to be received, modified and returned.")]
		protected virtual DialogResult OnExtendedMessageInputRequest(string text, ICollection<LinkLabel.Link> links, string caption, string checkText, ref bool checkValue, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			if (this.launchArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				var e = new ExtendedMessageInputEventArgs(text, links, caption, checkText, checkValue, buttons, icon, defaultButton);
				this.eventHelper.RaiseSync<ExtendedMessageInputEventArgs>(ExtendedMessageInputRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break(); // OK as conditional and disabled by default.
				#else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Message Input' request by main has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				#endif
				}

				checkValue = e.CheckValue;
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
		protected virtual void OnLaunched()
		{
			this.eventHelper.RaiseSync(Launched, this, EventArgs.Empty);
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnExited(MainResult result)
		{
			this.eventHelper.RaiseSync<EventArgs<MainResult>>(Exited, this, new EventArgs<MainResult>(result));
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
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
				return (Caption);
			else
				return (base.ToString());
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
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
