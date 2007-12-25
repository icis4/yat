using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.IO;
using MKY.Utilities.Guid;
using MKY.Utilities.Event;
using MKY.Utilities.Recent;
using MKY.Utilities.Settings;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

using YAT.Model.Types;
using YAT.Model.Settings;
using YAT.Model.Utilities;

namespace YAT.Model
{
	public class Workspace : IDisposable, IGuidProvider
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed = false;

		private Guid _guid;
		private DocumentSettingsHandler<WorkspaceSettingsRoot> _settingsHandler;
		private WorkspaceSettingsRoot _settingsRoot;
		private bool _handlingSettingsIsSuspended = false;

		private List<Terminal> _terminals = new List<Terminal>();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		public event EventHandler SaveWorkspaceAsFileDialogRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Workspace()
		{
			// create workspace settings from most recent workspace file
			DocumentSettingsHandler<WorkspaceSettingsRoot> sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			sh.SettingsFilePath = ApplicationSettings.LocalUser.General.WorkspaceFilePath;
			Initialize(sh, Guid.Empty);
		}

		public Workspace(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
		{
			Initialize(settingsHandler, Guid.Empty);
		}

		public Workspace(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid)
		{
			Initialize(settingsHandler, guid);
		}

		private void Initialize(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid)
		{
			if (guid != Guid.Empty)
				_guid = guid;
			else
				_guid = Guid.NewGuid();

			_settingsHandler = settingsHandler;
			_settingsRoot = _settingsHandler.Settings;
			AttachWorkspaceSettingsHandlers();
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
					// nothing yet
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~Workspace()
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

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Guid Guid
		{
			get { return (_guid); }
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
		private void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(recentFile);
			ApplicationSettings.SaveLocalUser();
		}

		#endregion

		#region Workspace Settings
		//==========================================================================================
		// Workspace Settings
		//==========================================================================================

		private void AttachWorkspaceSettingsHandlers()
		{
			_settingsRoot.ClearChanged();
			_settingsRoot.Changed += new EventHandler<SettingsEventArgs>(_workspaceSettings_Changed);
		}

		//------------------------------------------------------------------------------------------
		// Workspace Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _workspaceSettings_Changed(object sender, SettingsEventArgs e)
		{
			if (_handlingSettingsIsSuspended)
				return;

			if (ApplicationSettings.LocalUser.General.AutoSaveWorkspace)
				SaveWorkspaceToFile(true);
		}

		private void SuspendHandlingWorkspaceSettings()
		{
			_handlingSettingsIsSuspended = true;
		}

		private void ResumeHandlingWorkspaceSettings()
		{
			_handlingSettingsIsSuspended = false;
		}

		#endregion

		#region Save
		//==========================================================================================
		// Save
		//==========================================================================================

		private void SaveWorkspace()
		{
			SaveWorkspace(false);
		}

		private void SaveWorkspace(bool autoSave)
		{
			if (autoSave)
			{
				SaveWorkspaceToFile(true);
			}
			else
			{
				if (_settingsHandler.SettingsFilePathIsValid && !_settingsHandler.Settings.AutoSaved)
					SaveWorkspaceToFile(false);
				else
					OnSaveWorkspaceAsFileDialogRequest(new EventArgs());
			}
		}

		private void SaveWorkspaceToFile(bool autoSave)
		{
			SaveWorkspaceToFile(autoSave, "");
		}

		private void SaveWorkspaceToFile(bool autoSave, string autoSaveFilePathToDelete)
		{
			if (!autoSave)
				OnFixedStatusTextRequest("Saving workspace...");

			try
			{
				_settingsRoot.SuspendChangeEvent();

				// set workspace file path before terminals are saved in order
				// to correctly retrieve relative paths
				if (autoSave)
				{
					string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveWorkspaceFileNamePrefix + Guid.ToString() + ExtensionSettings.WorkspaceFiles;
					if (!_settingsHandler.SettingsFilePathIsValid)
						_settingsHandler.SettingsFilePath = autoSaveFilePath;
				}

				// save all contained terminals
				foreach (Terminal t in _terminals)
				{
					t.AutoSave();
				}

				// update workspace
				foreach (Terminal t in _terminals)
				{
					AddToOrReplaceInWorkspace(t);
				}

				// save workspace
				_settingsHandler.Settings.AutoSaved = autoSave;
				_settingsHandler.Save();

				_settingsRoot.ClearChanged();
				_settingsRoot.ResumeChangeEvent();

				ApplicationSettings.LocalUser.General.WorkspaceFilePath = _settingsHandler.SettingsFilePath;
				ApplicationSettings.SaveLocalUser();

				if (!autoSave)
				{
					SetRecent(_settingsHandler.SettingsFilePath);
					OnTimedStatusTextRequest("Workspace saved");
				}

				// try to delete existing auto save file
				try
				{
					if (File.Exists(autoSaveFilePathToDelete))
						File.Delete(autoSaveFilePathToDelete);
				}
				catch (Exception)
				{
				}
			}
			catch (System.Xml.XmlException ex)
			{
				if (!autoSave)
				{
					OnFixedStatusTextRequest("Error saving workspace!");
					OnMessageInputRequest
						(
						"Unable to save file" + Environment.NewLine + _settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					OnTimedStatusTextRequest("Workspace not saved!");
				}
			}
		}

		#endregion

		#region Terminals
		//==========================================================================================
		// Terminals
		//==========================================================================================

		public int OpenTerminals()
		{
			int terminalCount = _settingsRoot.TerminalSettings.Count;
			if (terminalCount == 1)
				OnFixedStatusTextRequest("Opening workspace terminal...");
			else if (terminalCount > 1)
				OnFixedStatusTextRequest("Opening workspace terminals...");

			GuidList<TerminalSettingsItem> clone = new GuidList<TerminalSettingsItem>(_settingsRoot.TerminalSettings);
			foreach (TerminalSettingsItem item in clone)
			{
				try
				{
					OpenTerminalFromFile(item.FilePath, item.Guid, item.Window, true);
				}
				catch (System.Xml.XmlException ex)
				{
					OnFixedStatusTextRequest("Error opening terminal!");

					DialogResult result = OnMessageInputRequest
						(
						"Unable to open terminal" + Environment.NewLine + item.FilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message + Environment.NewLine + Environment.NewLine +
						"Continue loading workspace?",
						"File Error",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Exclamation
						);

					OnTimedStatusTextRequest("Terminal not opened!");

					if (result == DialogResult.No)
						break;
				}
			}

			return (terminalCount);
		}

		public bool OpenTerminalFromFile(string filePath)
		{
			return (OpenTerminalFromFile(filePath, Guid.Empty, null, false));
		}

		private bool OpenTerminalFromFile(string filePath, Guid guid, Settings.WindowSettings windowSettings, bool suppressErrorHandling)
		{
			string absoluteFilePath = filePath;

			OnFixedStatusTextRequest("Opening terminal...");
			try
			{
				DocumentSettingsHandler<TerminalSettingsRoot> sh = new DocumentSettingsHandler<TerminalSettingsRoot>();

				// combine absolute workspace path with terminal path if that one is relative
				absoluteFilePath = XPath.CombineFilePaths(_settingsHandler.SettingsFilePath, filePath);
				sh.SettingsFilePath = absoluteFilePath;
				sh.Load();

				// replace window settings with those saved in workspace
				if (windowSettings != null)
					sh.Settings.Window = windowSettings;

				// create terminal
				Terminal terminal = new Terminal(sh, guid);

				terminal.Changed += new EventHandler(terminal_Changed);
				terminal.Saved   += new EventHandler<SavedEventArgs>(terminal_Saved);
				terminal.Closed  += new EventHandler(terminal_Closed);

				AddToOrReplaceInWorkspace(terminal);
				if (!sh.Settings.AutoSaved)
					SetRecent(filePath);

				OnTimedStatusTextRequest("Terminal opened");
				return (true);
			}
			catch (System.Xml.XmlException ex)
			{
				if (!suppressErrorHandling)
				{
					OnFixedStatusTextRequest("Error opening terminal!");

					OnMessageInputRequest
						(
						"Unable to open file" + Environment.NewLine + absoluteFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop
						);

					OnTimedStatusTextRequest("Terminal not opened!");
					return (false);
				}
				else
				{
					throw (ex);
				}
			}
		}

		private void AddToOrReplaceInWorkspace(Terminal terminal)
		{
			string filePath = terminal.SettingsFilePath;
			if (ApplicationSettings.LocalUser.General.UseRelativePaths)
			{
				XPathCompareResult pcr = XPath.CompareFilePaths(_settingsHandler.SettingsFilePath, terminal.SettingsFilePath);
				if (pcr.AreRelative)
					filePath = pcr.RelativePath;
			}

			// clone window settings
			WindowSettings ws = new WindowSettings(terminal.WindowSettings);

			// create settings item
			TerminalSettingsItem tsi = new TerminalSettingsItem();
			tsi.Guid = terminal.Guid;
			tsi.FilePath = filePath;
			tsi.Window = ws;

			_settingsRoot.TerminalSettings.AddOrReplaceGuid(tsi);
			_settingsRoot.SetChanged();
		}

		private void RemoveFromWorkspace(Terminal terminal)
		{
			_settingsRoot.TerminalSettings.RemoveGuid(terminal.Guid);
			_settingsRoot.SetChanged();
		}

		public bool Close()
		{
			//foreach (Terminal t in _terminals)
			//	t.Close();
			return (false);
		}

		//------------------------------------------------------------------------------------------
		// Terminals > Events
		//------------------------------------------------------------------------------------------

		private void terminal_Changed(object sender, EventArgs e)
		{
			// nothing to do
		}

		private void terminal_Saved(object sender, SavedEventArgs e)
		{
			AddToOrReplaceInWorkspace((Terminal)sender);

			if (!e.AutoSave)
				SetRecent(e.FilePath);
		}

		private void terminal_Closed(object sender, EventArgs e)
		{
			RemoveFromWorkspace((Terminal)sender);
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			OnFixedStatusTextRequest(new StatusTextEventArgs(text));
		}

		/// <summary></summary>
		protected virtual void OnFixedStatusTextRequest(StatusTextEventArgs e)
		{
			EventHelper.FireSync(FixedStatusTextRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			OnTimedStatusTextRequest(new StatusTextEventArgs(text));
		}

		/// <summary></summary>
		protected virtual void OnTimedStatusTextRequest(StatusTextEventArgs e)
		{
			EventHelper.FireSync(TimedStatusTextRequest, this, e);
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			MessageInputEventArgs e = new MessageInputEventArgs(text, caption, buttons, icon);
			OnMessageInputRequest(e);
			return (e.Result);
		}

		/// <summary></summary>
		protected virtual void OnMessageInputRequest(MessageInputEventArgs e)
		{
			EventHelper.FireSync(MessageInputRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSaveWorkspaceAsFileDialogRequest(EventArgs e)
		{
			EventHelper.FireSync(SaveWorkspaceAsFileDialogRequest, this, e);
		}

		#endregion
	}
}
