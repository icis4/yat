using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.Event;
using MKY.Utilities.Recent;
using MKY.Utilities.Settings;
using MKY.Utilities.IO;

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
	public class Main : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed = false;

		private string _requestedFilePath = "";

		private Workspace _workspace;

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
		public event EventHandler Closed;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Main()
		{
			Initialize("");
		}

		/// <summary></summary>
		public Main(string requestedFilePath)
		{
			Initialize(requestedFilePath);
		}

		private void Initialize(string requestedFilePath)
		{
			_requestedFilePath = requestedFilePath;
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
			if (!_isDisposed)
			{
				if (disposing)
				{
					if (_workspace != null)
						_workspace.Dispose();
				}
				_isDisposed = true;
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
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region General Properties
		//==========================================================================================
		// General Properties
		//==========================================================================================

		/// <summary></summary>
		public string UserName
		{
			get
			{
				AssertNotDisposed();

				if (_workspace != null)
					return (_workspace.UserName);
				else
					return (ApplicationInfo.ProductName);
			}
		}

		#endregion

		#region General Methods
		//==========================================================================================
		// General Methods
		//==========================================================================================

		/// <summary>
		/// If a file was requested by command line argument, this method tries to open the
		/// requested file.
		/// Else, this method tries to open the most recent workspace of the current user.
		/// </summary>
		/// <returns>Returns true if either operation succeeded, false otherwise</returns>
		public bool Start()
		{
			AssertNotDisposed();

			bool success = false;

			if ((_requestedFilePath != null) && (_requestedFilePath != ""))
			{
				success = OpenFromFile(_requestedFilePath);
			}

			if (!success && ApplicationSettings.LocalUser.General.AutoOpenWorkspace)
			{
				string filePath = ApplicationSettings.LocalUser.General.WorkspaceFilePath;
				if (File.Exists(filePath))
					success = OpenWorkspaceFromFile(filePath);
			}

			return (success);
		}

		#endregion

		#region Recents
		//==========================================================================================
		// Recents
		//==========================================================================================

		/// <summary></summary>
		public bool OpenRecent(int userIndex)
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
			ApplicationSettings.SaveLocalUser();
		}

		#endregion

		#region Close
		//==========================================================================================
		// Close
		//==========================================================================================

		/// <summary></summary>
		public bool Close()
		{
			bool success = _workspace.Close();

			if (success)
				OnFixedStatusTextRequest("Exiting " + Application.ProductName + "...");
			else
				OnTimedStatusTextRequest("Exit cancelled");

			if (success)
				OnClosed(new EventArgs());

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
			_workspace.TerminalRemoved += new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalRemoved);
			_workspace.TerminalAdded   += new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalAdded);

			_workspace.Closed += new EventHandler(_workspace_Closed);
		}

		private void DetachWorkspaceEventHandlers()
		{
			_workspace.TerminalRemoved -= new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalRemoved);
			_workspace.TerminalAdded   -= new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalAdded);

			_workspace.Closed -= new EventHandler(_workspace_Closed);
		}

		#endregion

		#region Workspace > Event Handlers
		//------------------------------------------------------------------------------------------
		// Workspace > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _workspace_TerminalRemoved(object sender, Model.TerminalEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private void _workspace_TerminalAdded(object sender, Model.TerminalEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private void _workspace_Closed(object sender, EventArgs e)
		{
			OnWorkspaceClosed(e);
		}

		#endregion

		#region Workspace > Methods
		//------------------------------------------------------------------------------------------
		// Workspace > Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Opens YAT and opens the workspace or terminal file given. This method can directly
		/// be called from the main providing the command line arguments.
		/// </summary>
		/// <param name="filePath">Workspace or terminal file</param>
		/// <returns>true if successfully opened the workspace or terminal</returns>
		public bool OpenFromFile(string filePath)
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
				OnFixedStatusTextRequest("Opening terminal " + fileName + "...");
				return (_workspace.OpenTerminalFromFile(filePath));
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
		public bool OpenWorkspaceFromFile(string filePath)
		{
			AssertNotDisposed();

			// close workspace, only one workspace can exist within application
			if (!_workspace.Close())
				return (false);

			OnFixedStatusTextRequest("Opening workspace...");
			try
			{
				DocumentSettingsHandler<WorkspaceSettingsRoot> sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
				sh.SettingsFilePath = filePath;
				sh.Load();

				ApplicationSettings.LocalUser.General.WorkspaceFilePath = filePath;
				ApplicationSettings.SaveLocalUser();

				// create workspace
				_workspace = new Workspace(sh);

				if (!sh.Settings.AutoSaved)
					SetRecent(filePath);

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
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
					);

				OnTimedStatusTextRequest("No workspace opened!");
				return (false);
			}

			// open workspace terminals
			int terminalCount = _workspace.OpenTerminals();

			if (terminalCount == 1)
				OnTimedStatusTextRequest("Workspace terminal opened");
			else if (terminalCount > 1)
				OnTimedStatusTextRequest("Workspace terminals opened");
			else
				OnTimedStatusTextRequest("Workspace contains no terminal to open");

			// attach workspace event handlers after terminals have been opened
			AttachWorkspaceEventHandlers();

			// fire workspace event
			OnWorkspaceOpened(new WorkspaceEventArgs(_workspace));

			return (true);
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
			EventHelper.FireSync(FixedStatusTextRequest, this, new StatusTextEventArgs(text));
		}

		/// <summary></summary>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			EventHelper.FireSync(TimedStatusTextRequest, this, new StatusTextEventArgs(text));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			MessageInputEventArgs e = new MessageInputEventArgs(text, caption, buttons, icon);
			EventHelper.FireSync(MessageInputRequest, this, e);
			return (e.Result);
		}

		/// <summary></summary>
		protected virtual void OnClosed(EventArgs e)
		{
			EventHelper.FireSync(Closed, this, e);
		}

		#endregion
	}
}
