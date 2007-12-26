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

using YAT.Utilities;

namespace YAT.Model
{
	/// <summary></summary>
	public class Workspace : IDisposable, IGuidProvider
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed = false;

		private Guid _guid;

		// settings
		private DocumentSettingsHandler<WorkspaceSettingsRoot> _settingsHandler;
		private WorkspaceSettingsRoot _settingsRoot;
		private bool _handlingSettingsIsSuspended = false;

		// terminal list
		private List<Terminal> _terminals = new List<Terminal>();
		private Terminal _activeTerminal = null;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<TerminalEventArgs> TerminalAdded;
		/// <summary></summary>
		public event EventHandler<TerminalEventArgs> TerminalRemoved;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> FixedStatusTextRequest;
		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> TimedStatusTextRequest;
		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler SaveAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<SavedEventArgs> Saved;
		/// <summary></summary>
		public event EventHandler Closed;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Workspace()
		{
			// create workspace settings from most recent workspace file
			DocumentSettingsHandler<WorkspaceSettingsRoot> sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			sh.SettingsFilePath = ApplicationSettings.LocalUser.General.WorkspaceFilePath;
			Initialize(sh, Guid.Empty);
		}

		/// <summary></summary>
		public Workspace(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
		{
			Initialize(settingsHandler, Guid.Empty);
		}

		/// <summary></summary>
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

			// link and attach to settings
			_settingsHandler = settingsHandler;
			_settingsRoot = _settingsHandler.Settings;
			_settingsRoot.ClearChanged();
			AttachSettingsEventHandlers();
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

		#region General Properties
		//==========================================================================================
		// General Properties
		//==========================================================================================

		/// <summary></summary>
		public Guid Guid
		{
			get
			{
				AssertNotDisposed();
				return (_guid);
			}
		}

		/// <summary></summary>
		public string UserName
		{
			get
			{
				AssertNotDisposed();

				if (_activeTerminal != null)
					return (_activeTerminal.UserName);
				else
					return (ApplicationInfo.ProductName);
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
		private void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(recentFile);
			ApplicationSettings.SaveLocalUser();
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		/// <summary></summary>
		public bool SettingsHaveChanged
		{
			get
			{
				AssertNotDisposed();
				return (_settingsRoot.HaveChanged);
			}
		}

		/// <summary></summary>
		public WorkspaceSettingsRoot SettingsRoot
		{
			get
			{
				AssertNotDisposed();
				return (_settingsRoot);
			}
		}

		private void AttachSettingsEventHandlers()
		{
			_settingsRoot.Changed += new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		private void DetachSettingsEventHandlers()
		{
			_settingsRoot.Changed -= new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			if (_handlingSettingsIsSuspended)
				return;

			if (ApplicationSettings.LocalUser.General.AutoSaveWorkspace)
				SaveToFile(true);
		}

		private void SuspendHandlingSettings()
		{
			_handlingSettingsIsSuspended = true;
		}

		private void ResumeHandlingSettings()
		{
			_handlingSettingsIsSuspended = false;
		}

		#endregion

		#region Save
		//==========================================================================================
		// Save
		//==========================================================================================

		/// <summary>
		/// Only performs auto save if no file yet or on previously auto saved files
		/// </summary>
		private bool TryAutoSave()
		{
			AssertNotDisposed();

			bool success = false;
			if (!_settingsHandler.SettingsFileExists ||
				(_settingsHandler.SettingsFileExists && _settingsRoot.AutoSaved))
			{
				success = Save(true);
			}
			return (success);
		}

		/// <summary>
		/// Saves all terminals and workspace to file(s), prompts for file(s) if they doesn't exist yet
		/// </summary>
		public bool Save()
		{
			AssertNotDisposed();

			return (Save(false));
		}

		/// <summary>
		/// Saves all terminals and workspace to file(s), prompts for file(s) if they doesn't exist yet
		/// </summary>
		public bool Save(bool autoSave)
		{
			AssertNotDisposed();

			bool success = false;
			if (autoSave)
			{
				success = SaveToFile(true);
			}
			else
			{
				if (_settingsHandler.SettingsFilePathIsValid && !_settingsHandler.Settings.AutoSaved)
					success = SaveToFile(false);
				else
					success = (OnSaveAsFileDialogRequest() == DialogResult.OK);
			}
			return (success);
		}

		/// <summary>
		/// Saves all terminals and workspace to given file
		/// </summary>
		public bool SaveAs(string filePath)
		{
			AssertNotDisposed();

			string autoSaveFilePathToDelete = "";
			if (_settingsRoot.AutoSaved)
				autoSaveFilePathToDelete = _settingsHandler.SettingsFilePath;

			_settingsHandler.SettingsFilePath = filePath;
			return (SaveToFile(false, autoSaveFilePathToDelete));
		}

		private bool SaveToFile(bool autoSave)
		{
			return (SaveToFile(autoSave, ""));
		}

		private bool SaveToFile(bool autoSave, string autoSaveFilePathToDelete)
		{
			bool success = false;

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
				SaveAllTerminals(false);

				// save workspace
				_settingsHandler.Settings.AutoSaved = autoSave;
				_settingsHandler.Save();

				_settingsRoot.ClearChanged();
				_settingsRoot.ResumeChangeEvent();

				ApplicationSettings.LocalUser.General.WorkspaceFilePath = _settingsHandler.SettingsFilePath;
				ApplicationSettings.SaveLocalUser();

				success = true;
				OnSaved(new SavedEventArgs(_settingsHandler.SettingsFilePath, autoSave));

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
			return (success);
		}

		#endregion

		#region Close
		//==========================================================================================
		// Close
		//==========================================================================================

		/// <summary></summary>
		public bool Close()
		{
			bool success = false;

			// first, save all terminals
			success = SaveAllTerminals();
			if (!success)
				return (false);

			// next, save workspace
			if (_settingsRoot.HaveChanged)
			{
				// try to auto save it
				if (ApplicationSettings.LocalUser.General.AutoSaveWorkspace)
					success = TryAutoSave();

				// or save it manually
				if (!success)
				{
					DialogResult dr = OnMessageInputRequest
						(
						"Save workspace?",
						UserName,
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
						);

					switch (dr)
					{
						case DialogResult.Yes:    success = Save(); break;
						case DialogResult.No:     success = true;   break;
						case DialogResult.Cancel:
						default:                  return (false);
					}
				}
			}

			// last, close all contained terminals
			success = CloseAllTerminals();

			return (success);
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

		private void AttachTerminalEventHandlers(Terminal terminal)
		{
			terminal.Saved  += new EventHandler<SavedEventArgs>(terminal_Saved);
			terminal.Closed += new EventHandler(terminal_Closed);
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

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

		#region Terminal > General Methods
		//------------------------------------------------------------------------------------------
		// Terminal > General Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Opens terminals according to workspace settings and returns number of successfully
		/// opened terminals
		/// </summary>
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

		/// <summary></summary>
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

				AttachTerminalEventHandlers(terminal);
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

		/// <summary></summary>
		public bool CreateNewTerminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			OnFixedStatusTextRequest("Creating new terminal...");

			// create terminal
			Terminal terminal = new Terminal(settingsHandler);

			AttachTerminalEventHandlers(terminal);
			AddToOrReplaceInWorkspace(terminal);

			OnTimedStatusTextRequest("New terminal created");
			return (true);
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

			// fire terminal added event
			OnTerminalAdded(new TerminalEventArgs(terminal));
		}

		private void RemoveFromWorkspace(Terminal terminal)
		{
			_settingsRoot.TerminalSettings.RemoveGuid(terminal.Guid);
			_settingsRoot.SetChanged();

			// fire terminal added event
			OnTerminalRemoved(new TerminalEventArgs(terminal));
		}

		#endregion

		#region Terminal > All Terminals Methods
		//------------------------------------------------------------------------------------------
		// Terminal > All Terminals Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool SaveAllTerminals()
		{
			return (SaveAllTerminals(false));
		}

		private bool SaveAllTerminals(bool autoSave)
		{
			bool success = true;
			foreach (Terminal t in _terminals)
			{
				if (!t.Save(autoSave))
					success = false;
			}
			return (success);
		}

		/// <summary></summary>
		public bool CloseAllTerminals()
		{
			bool success = true;
			foreach (Terminal t in _terminals)
			{
				if (!t.Close())
					success = false;
			}
			return (success);
		}

		#endregion

		#region Terminal > Active Terminal Methods
		//------------------------------------------------------------------------------------------
		// Terminal > Active Terminal Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool SaveActiveTerminal()
		{
			return (_activeTerminal.Save());
		}

		/// <summary></summary>
		public bool CloseActiveTerminal()
		{
			return (_activeTerminal.Close());
		}

		/// <summary></summary>
		public bool OpenActiveTerminalIO()
		{
			return (_activeTerminal.OpenIO());
		}

		/// <summary></summary>
		public bool CloseActiveTerminalIO()
		{
			return (_activeTerminal.CloseIO());
		}

		#endregion

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTerminalAdded(TerminalEventArgs e)
		{
			EventHelper.FireSync<TerminalEventArgs>(TerminalAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnTerminalRemoved(TerminalEventArgs e)
		{
			EventHelper.FireSync<TerminalEventArgs>(TerminalRemoved, this, e);
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
		protected virtual DialogResult OnSaveAsFileDialogRequest()
		{
			DialogEventArgs e = new DialogEventArgs();
			EventHelper.FireSync(SaveAsFileDialogRequest, this, e);
			return (e.Result);
		}

		/// <summary></summary>
		protected virtual void OnSaved(SavedEventArgs e)
		{
			EventHelper.FireSync<SavedEventArgs>(Saved, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(EventArgs e)
		{
			EventHelper.FireSync(Closed, this, e);
		}

		#endregion
	}
}
