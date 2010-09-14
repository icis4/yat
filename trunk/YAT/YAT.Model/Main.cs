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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.Event;
using MKY.Utilities.Guid;
using MKY.Utilities.Settings;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

using YAT.Utilities;

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

		private CommandLineOptions commandLineOptions;

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
			this.guid = Guid.NewGuid();
		}

		/// <summary></summary>
		public Main(string requestedFilePath)
		{
			this.guid = Guid.NewGuid();
			this.commandLineOptions = new CommandLineOptions();
			this.commandLineOptions.RequestedFilePath = requestedFilePath;
		}

		/// <summary></summary>
		public Main(CommandLineOptions commandLineOptions)
		{
			this.guid = Guid.NewGuid();
			this.commandLineOptions = commandLineOptions;
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
		public virtual CommandLineOptions CommandLineOptions
		{
			get
			{
				AssertNotDisposed();
				return (this.commandLineOptions);
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
		/// If a file was requested by command line argument, this method tries to open the
		/// requested file.
		/// Else, this method tries to open the most recent workspace of the current user.
		/// If still unsuccessful, a new workspace is created.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if either operation succeeded, <c>false</c> otherwise.
		/// </returns>
		public virtual bool Start()
		{
			AssertNotDisposed();

			bool otherInstanceIsAlreadyRunning = OtherInstanceIsAlreadyRunning();
			bool success = false;

			if ((this.commandLineOptions != null) && !string.IsNullOrEmpty(this.commandLineOptions.RequestedFilePath))
			{
				success = OpenFromFile(this.commandLineOptions.RequestedFilePath);

				if (success)
				{
					// Reset workspace file path.
					ApplicationSettings.LocalUser.AutoWorkspace.ResetFilePathAndUser();
					ApplicationSettings.Save();

					// Clean up all default workspaces/terminals since they're not needed anymore.
					if (!otherInstanceIsAlreadyRunning)
						CleanupLocalUserDirectory();
				}
			}

			if (!success && ApplicationSettings.LocalUser.General.AutoOpenWorkspace)
			{
				// Make sure that auto workspace is not already in use by another instance of YAT.
				if (ApplicationSettings.LocalUser.AutoWorkspace.FilePathUser == Guid.Empty)
				{
					string filePath = ApplicationSettings.LocalUser.AutoWorkspace.FilePath;
					if (File.Exists(filePath))
						success = OpenWorkspaceFromFile(filePath);
				}
			}

			// Clean up the default directory.
			if (!otherInstanceIsAlreadyRunning)
				CleanupLocalUserDirectory();

			// If no success so far, create a new empty workspace.
			if (!success)
			{
				// Reset workspace file path.
				ApplicationSettings.LocalUser.AutoWorkspace.ResetFilePathAndUser();
				ApplicationSettings.Save();

				success = CreateNewWorkspace();
			}

			// Update number of running instances.
			if (success)
				staticInstancesRunning++;

			return (success);
		}

		private bool OtherInstanceIsAlreadyRunning()
		{
			List<Process> processes = new List<Process>();

			// Get all processes named "YAT" (also "NUnit" while testing).
			processes.AddRange(Process.GetProcessesByName(Application.ProductName));

			// Also get all debug processes named "YAT.vshost".
			processes.AddRange(Process.GetProcessesByName(Application.ProductName + ".vshost"));

			// Remove current instance
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
		public virtual bool Exit()
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
				OnTimedStatusTextRequest("Exit cancelled");

			if (success)
				OnExited(new EventArgs());

			return (success);
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
				this.workspace.Saved  += new EventHandler<SavedEventArgs> (this.workspace_Saved);
				this.workspace.Closed += new EventHandler<ClosedEventArgs>(this.workspace_Closed);
			}
		}

		private void DetachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.Saved  -= new EventHandler<SavedEventArgs> (this.workspace_Saved);
				this.workspace.Closed -= new EventHandler<ClosedEventArgs>(this.workspace_Closed);
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

		#region Workspace > Methods
		//------------------------------------------------------------------------------------------
		// Workspace > Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool CreateNewWorkspace()
		{
			// close workspace, only one workspace can exist within application
			if (this.workspace != null)
			{
				if (!this.workspace.Close())
					return (false);
			}

			OnFixedStatusTextRequest("Creating new workspace...");

			// Create workspace
			this.workspace = new Workspace(new DocumentSettingsHandler<WorkspaceSettingsRoot>());
			AttachWorkspaceEventHandlers();
			OnWorkspaceOpened(new WorkspaceEventArgs(this.workspace));

			OnTimedStatusTextRequest("New workspace created");
			return (true);
		}

		/// <summary></summary>
		public virtual bool CreateNewWorkspaceAndTerminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			if (!CreateNewWorkspace())
				return (false);

			return (this.workspace.CreateNewTerminal(settingsHandler));
		}

		/// <summary>
		/// Opens YAT and opens the workspace or terminal file given. This method can directly
		/// be called from the main providing the command line arguments.
		/// </summary>
		/// <param name="filePath">Workspace or terminal file.</param>
		/// <returns><c>true</c> if successfully opened the workspace or terminal.</returns>
		public virtual bool OpenFromFile(string filePath)
		{
			AssertNotDisposed();

			string fileName = Path.GetFileName(filePath);
			string extension = Path.GetExtension(filePath);
			if (ExtensionSettings.IsWorkspaceFile(extension))
			{
				OnFixedStatusTextRequest("Opening workspace " + fileName + "...");
				return (OpenWorkspaceFromFile(filePath));
			}
			else if (ExtensionSettings.IsTerminalFile(extension))
			{
				if (!CreateNewWorkspace())
					return (false);

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

		/// <summary></summary>
		public virtual bool OpenWorkspaceFromFile(string filePath)
		{
			AssertNotDisposed();

			// -------------------------------------------------------------------------------------
			// Check whether workspace is already opened.
			// -------------------------------------------------------------------------------------

			if (this.workspace != null)
			{
				if (filePath == this.workspace.SettingsFilePath)
				{
					OnFixedStatusTextRequest("Workspace is already open.");
					OnMessageInputRequest
						(
						"Workspace is already open and will not be re-openend.",
						"Workspace File Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					OnTimedStatusTextRequest("Workspace not re-opened.");
					return (false);
				}
			}

			// Close workspace, only one workspace can exist within application.
			if (this.workspace != null)
			{
				if (!this.workspace.Close())
					return (false);
			}

			// -------------------------------------------------------------------------------------
			// Open the workspace itself.
			// -------------------------------------------------------------------------------------

			OnFixedStatusTextRequest("Opening workspace...");
			try
			{
				DocumentSettingsHandler<WorkspaceSettingsRoot> sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
				sh.SettingsFilePath = filePath;
				sh.Load();

				ApplicationSettings.LocalUser.AutoWorkspace.SetFilePathAndUser(filePath, Guid);
				ApplicationSettings.Save();

				// Try to retrieve GUID from file path (in case of auto saved workspace files).
				Guid guid = XGuid.CreateGuidFromFilePath(filePath, GeneralSettings.AutoSaveWorkspaceFileNamePrefix);

				// Create workspace.
				this.workspace = new Workspace(sh, guid);
				AttachWorkspaceEventHandlers();

				if (!sh.Settings.AutoSaved)
					SetRecent(filePath);

				OnWorkspaceOpened(new WorkspaceEventArgs(this.workspace));
				OnTimedStatusTextRequest("Workspace opened");
			}
			catch (System.Xml.XmlException ex)
			{
				OnFixedStatusTextRequest("Error opening workspace!");
				OnMessageInputRequest
					(
					"Unable to open file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
					"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
					"File error message: " + ex.InnerException.Message,
					"Invalid Workspace File",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
					);
				OnTimedStatusTextRequest("No workspace opened!");
				return (false);
			}

			// -------------------------------------------------------------------------------------
			// Open workspace terminals.
			// -------------------------------------------------------------------------------------

			int terminalCount = this.workspace.OpenTerminals();

			if (terminalCount == 1)
				OnTimedStatusTextRequest("Workspace terminal opened");
			else if (terminalCount > 1)
				OnTimedStatusTextRequest("Workspace terminals opened");
			else
				OnTimedStatusTextRequest("Workspace contains no terminal to open");

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
				return (false);
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
