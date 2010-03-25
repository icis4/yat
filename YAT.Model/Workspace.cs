//==================================================================================================
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

		private bool _isDisposed;

		private Guid _guid;

		// settings
		private DocumentSettingsHandler<WorkspaceSettingsRoot> _settingsHandler;
		private WorkspaceSettingsRoot _settingsRoot;

		// terminal list
		private GuidList<Terminal> _terminals = new GuidList<Terminal>();
		private Terminal _activeTerminal = null;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>Fired when a new terminal was added to the workspace.</summary>
		public event EventHandler<TerminalEventArgs> TerminalAdded;
		/// <summary>Fired when a terminal was removed from the workspace.</summary>
		public event EventHandler<TerminalEventArgs> TerminalRemoved;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> FixedStatusTextRequest;
		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> TimedStatusTextRequest;
		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<DialogEventArgs> SaveAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<SavedEventArgs> Saved;
		/// <summary></summary>
		public event EventHandler<ClosedEventArgs> Closed;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Workspace()
			: this(new DocumentSettingsHandler<WorkspaceSettingsRoot>(), Guid.NewGuid())
		{
		}

		/// <summary></summary>
		public Workspace(WorkspaceSettingsRoot settings)
			: this(new DocumentSettingsHandler<WorkspaceSettingsRoot>(settings), Guid.Empty)
		{
		}

		/// <summary></summary>
		public Workspace(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
			: this(settingsHandler, Guid.Empty)
		{
		}

		/// <summary></summary>
		public Workspace(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid)
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
					if (_terminals != null)
					{
						// First, detach event handlers to ensure that no more events are received
						foreach (Terminal t in _terminals)
							DetachTerminalEventHandlers(t);
					}

					DetachSettingsEventHandlers();

					if (_terminals != null)
					{
						// Then, dispose of objects
						foreach (Terminal t in _terminals)
							t.Dispose();

						_terminals.Clear();
						_terminals = null;
					}
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
		public virtual Guid Guid
		{
			get
			{
				AssertNotDisposed();
				return (_guid);
			}
		}

		/// <summary></summary>
		public virtual string UserName
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

		/// <summary></summary>
		public virtual bool SettingsFileExists
		{
			get
			{
				AssertNotDisposed();
				return (_settingsHandler.SettingsFileExists);
			}
		}

		/// <summary></summary>
		public virtual string SettingsFilePath
		{
			get
			{
				AssertNotDisposed();
				return (_settingsHandler.SettingsFilePath);
			}
		}

		/// <summary>
		/// Returns number of terminals within workspace.
		/// </summary>
		public virtual int TerminalCount
		{
			get
			{
				AssertNotDisposed();
				return (_terminals.Count);
			}
		}

		/// <summary>
		/// Returns an array of all terminals within workspace or <c>null</c> if there are no terminals.
		/// </summary>
		public virtual Terminal[] Terminals
		{
			get
			{
				AssertNotDisposed();
				return (_terminals.ToArray());
			}
		}

		/// <summary>
		/// Returns active terminal within workspace or <c>null</c> if no terminal is active.
		/// </summary>
		public virtual Terminal ActiveTerminal
		{
			get
			{
				AssertNotDisposed();
				return (_activeTerminal);
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
			if (_settingsRoot != null)
				_settingsRoot.Changed += new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		private void DetachSettingsEventHandlers()
		{
			if (_settingsRoot != null)
				_settingsRoot.Changed -= new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		private bool _settingsRoot_Changed_handlingSettingsIsSuspended = false;

		private void _settingsRoot_Changed(object sender, SettingsEventArgs e)
		{

			if (_settingsRoot_Changed_handlingSettingsIsSuspended)
				return;

			if (ApplicationSettings.LocalUser.General.AutoSaveWorkspace)
			{
				// prevent recursive calls
				_settingsRoot_Changed_handlingSettingsIsSuspended = true;
				TryAutoSaveIfFileAlreadyAutoSaved();
				_settingsRoot_Changed_handlingSettingsIsSuspended = false;
			}
		}

		#endregion

		#region Settings > Properties
		//------------------------------------------------------------------------------------------
		// Settings > Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool SettingsHaveChanged
		{
			get
			{
				AssertNotDisposed();
				return (_settingsRoot.HaveChanged);
			}
		}

		/// <summary></summary>
		public virtual WorkspaceSettingsRoot SettingsRoot
		{
			get
			{
				AssertNotDisposed();
				return (_settingsRoot);
			}
		}

		#endregion

		#endregion

		#region Save
		//==========================================================================================
		// Save
		//==========================================================================================

		/// <summary>
		/// Performs auto save if no file yet or on previously auto saved files.
		/// Performs normal save on existing normal files.
		/// </summary>
		private bool TryAutoSave()
		{
			bool success = false;
			if (_settingsHandler.SettingsFileExists && !_settingsRoot.AutoSaved)
				success = SaveToFile(false);
			else
				success = SaveToFile(true);
			return (success);
		}

		/// <summary>
		/// Performs auto save on previously auto saved files.
		/// </summary>
		/// <remarks>
		/// This method is intentionally named this long to emphasize the difference to
		/// <see cref="TryAutoSave()"/> above.
		/// </remarks>
		private bool TryAutoSaveIfFileAlreadyAutoSaved()
		{
			bool success = false;
			if (_settingsHandler.SettingsFileExists && _settingsRoot.AutoSaved)
			{
				success = SaveToFile(true);
			}
			return (success);
		}

		/// <summary>
		/// Saves all terminals and workspace to file(s), prompts for file(s) if they doesn't exist yet
		/// </summary>
		public virtual bool Save()
		{
			return (Save(true));
		}

		/// <summary>
		/// Saves all terminals and workspace to file(s), prompts for file(s) if they doesn't exist yet
		/// </summary>
		public virtual bool Save(bool autoSaveIsAllowed)
		{
			AssertNotDisposed();

			bool success = false;

			// Save workspace if file path is valid
			if (_settingsHandler.SettingsFilePathIsValid)
			{
				if (_settingsHandler.Settings.AutoSaved)
				{
					if (autoSaveIsAllowed)
						success = SaveToFile(true);
				}
				else
				{
					success = SaveToFile(false);
				}
			}
			else // Auto save creates default file path
			{
				if (autoSaveIsAllowed)
					success = SaveToFile(true);
			}

			// If not successful yet, request new file path
			if (!success)
				success = (OnSaveAsFileDialogRequest() == DialogResult.OK);

			return (success);
		}

		/// <summary>
		/// Saves all terminals and workspace to given file
		/// </summary>
		public virtual bool SaveAs(string filePath)
		{
			AssertNotDisposed();

			string autoSaveFilePathToDelete = "";
			if (_settingsRoot.AutoSaved)
				autoSaveFilePathToDelete = _settingsHandler.SettingsFilePath;

			_settingsHandler.SettingsFilePath = filePath;
			return (SaveToFile(false, autoSaveFilePathToDelete));
		}

		private bool SaveToFile(bool doAutoSave)
		{
			return (SaveToFile(doAutoSave, ""));
		}

		private bool SaveToFile(bool doAutoSave, string autoSaveFilePathToDelete)
		{
			// -------------------------------------------------------------------------------------
			// Skip save if file is up to date and there were no changes
			// -------------------------------------------------------------------------------------

			if (_settingsHandler.SettingsFileIsUpToDate && (!_settingsRoot.HaveChanged))
			{
				// Event must be fired anyway to ensure that dependent objects are updated
				OnSaved(new SavedEventArgs(_settingsHandler.SettingsFilePath, doAutoSave));
				return (true);
			}

			// -------------------------------------------------------------------------------------
			// First, save all contained terminals
			// -------------------------------------------------------------------------------------

			bool success = false;

			if (!doAutoSave)
				OnFixedStatusTextRequest("Saving workspace...");

			// In case of auto save, assign workspace settings file path before saving terminals
			// This ensures that relative paths are correctly retrieved by SaveAllTerminals()
			if (doAutoSave && (!_settingsHandler.SettingsFilePathIsValid))
			{
				string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveWorkspaceFileNamePrefix + Guid.ToString() + ExtensionSettings.WorkspaceFile;
				_settingsHandler.SettingsFilePath = autoSaveFilePath;
			}

			if (!SaveAllTerminals(doAutoSave))
			{
				OnTimedStatusTextRequest("Workspace not saved!");
				return (false);
			}

			try
			{
				// ---------------------------------------------------------------------------------
				// Save workspace
				// ---------------------------------------------------------------------------------

				_settingsHandler.Settings.AutoSaved = doAutoSave;
				_settingsHandler.Save();

				success = true;
				OnSaved(new SavedEventArgs(_settingsHandler.SettingsFilePath, doAutoSave));

				if (!doAutoSave)
				{
					SetRecent(_settingsHandler.SettingsFilePath);
					OnTimedStatusTextRequest("Workspace saved");
				}

				// ---------------------------------------------------------------------------------
				// Try to delete existing auto save file
				// ---------------------------------------------------------------------------------

				try
				{
					if (File.Exists(autoSaveFilePathToDelete))
						File.Delete(autoSaveFilePathToDelete);
				}
				catch
				{
				}
			}
			catch (System.Xml.XmlException ex)
			{
				if (!doAutoSave)
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

		/// <summary>Closes the workspace and prompts if the settings have changed.</summary>
		public virtual bool Close()
		{
			return (Close(false));
		}

		/// <summary>
		/// Closes the workspace and tries to auto save if desired.
		/// </summary>
		/// <remarks>
		/// \attention
		/// This method is needed for MDI applications. In case of MDI parent/application closing,
		/// Close() of the workspace is called. Without taking care of this, the workspace would
		/// be removed as the active workspace from the local user settings. Therefore, the
		/// workspace has to signal such cases to main.
		/// 
		/// Cases (similar to cases in Model.Terminal):
		/// - Main close
		///   - auto,   no file,       auto save    => auto save, if it fails => nothing  : (m1a)
		///   - auto,   no file,       no auto save => nothing                            : (m1b)
		///   - auto,   existing file, auto save    => auto save, if it fails => delete   : (m2a)
		///   - auto,   existing file, no auto save => delete                             : (m2b)
		///   - normal, no file                     => N/A (normal files have been saved) : (m3)
		///   - normal, existing file, auto save    => auto save, if it fails => question : (m4a)
		///   - normal, existing file, no auto save => question                           : (m4b)
		/// - Workspace close
		///   - auto,   no file                     => nothing                            : (w1)
		///   - auto,   existing file               => delete                             : (w2)
		///   - normal, no file                     => N/A (normal files have been saved) : (w3)
		///   - normal, existing file, auto save    => auto save, if it fails => question : (w4a)
		///   - normal, existing file, no auto save => question                           : (w4b)
		/// </remarks>
		public virtual bool Close(bool isMainClose)
		{
			bool tryAutoSave = ApplicationSettings.LocalUser.General.AutoSaveWorkspace;

			// Don't try to auto save if there is no existing file (w1)
			if (!isMainClose && !_settingsHandler.SettingsFileExists)
				tryAutoSave = false;

			OnFixedStatusTextRequest("Closing workspace...");

			// First, close all contained terminals signaling them a workspace close
			if (!CloseAllTerminals(true, tryAutoSave))
			{
				OnTimedStatusTextRequest("Workspace not closed");
				return (false);
			}

			bool success = false;

			// Try to auto save if desired
			if (tryAutoSave)
				success = TryAutoSave();

			// No success on auto save or auto save not desired
			if (!success)
			{
				// No file (m1, m3, w1, w3)
				if (!_settingsHandler.SettingsFileExists)
				{
					success = true; // Consider it successful if there was no file to save
				}
				// Existing file
				else
				{
					if (_settingsRoot.AutoSaved) // Existing auto file (m2a/b, w2)
					{
						_settingsHandler.Delete();
						success = true; // Don't care if auto file not successfully deleted
					}

					// Existing normal file (m4a/b, w4a/b) will be handled below
				}

				// Normal (m4a/b, w4a/b)
				if (!success && _settingsRoot.ExplicitHaveChanged)
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
						default:
							OnTimedStatusTextRequest("Workspace not closed");
							return (false);
					}
				}
				else // Else means settings have not changed
				{
					success = true; // Consider it successful if there was nothing to save
				}
			} // End of if no success on auto save or auto save disabled

			if (success)
			{
				// Status text request must be before closed event, closed event may close the view
				OnTimedStatusTextRequest("Workspace successfully closed");
				OnClosed(new ClosedEventArgs(isMainClose));
			}
			else
			{
				OnTimedStatusTextRequest("Workspace not closed");
			}
			return (success);
		}

		/// <summary>
		/// Method to check wheter auto save is really desired. Needed because of the MDI issue
		/// on close described in YAT.Gui.Forms.Main/Terminal.
		/// </summary>
		public virtual bool TryTerminalAutoSaveIsDesired(bool tryAutoSave, Terminal terminal)
		{
			// Do not auto save if terminal file already exists but workspace doesn't.
			// Applies to terminal use case w4a/b.
			if (tryAutoSave && !SettingsFileExists && terminal.SettingsFileExists)
				return (false);

			return (tryAutoSave);
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
			ApplicationSettings.Save();
		}

		#endregion

		#region Terminals
		//==========================================================================================
		// Terminals
		//==========================================================================================

		#region Terminals > Lifetime
		//------------------------------------------------------------------------------------------
		// Terminals > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachTerminalEventHandlers(Terminal terminal)
		{
			terminal.Saved  += new EventHandler<SavedEventArgs>(terminal_Saved);
			terminal.Closed += new EventHandler<ClosedEventArgs>(terminal_Closed);
		}

		private void DetachTerminalEventHandlers(Terminal terminal)
		{
			terminal.Saved  -= new EventHandler<SavedEventArgs>(terminal_Saved);
			terminal.Closed -= new EventHandler<ClosedEventArgs>(terminal_Closed);
		}

		#endregion

		#region Terminals > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminals > Event Handlers
		//------------------------------------------------------------------------------------------

		private void terminal_Saved(object sender, SavedEventArgs e)
		{
			ReplaceInWorkspace((Terminal)sender);

			if (!e.IsAutoSave)
				SetRecent(e.FilePath);
		}

		/// <remarks>
		/// See remarks of <see cref="Terminal.Close(bool, bool)"/> for details on why this event handler
		/// needs to treat the Closed event differently in case of a parent (i.e. workspace) close.
		/// </remarks>
		private void terminal_Closed(object sender, ClosedEventArgs e)
		{
			if (!e.IsParentClose)
				RemoveFromWorkspace((Terminal)sender);
		}

		#endregion

		#region Terminals > General Properties
		//------------------------------------------------------------------------------------------
		// Terminals > General Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns settings file paths of the all the terminals in the workspace
		/// </summary>
		public List<string> TerminalSettingsFilePaths
		{
			get
			{
				AssertNotDisposed();

				List<string> filePaths = new List<string>();
				foreach (Terminal t in _terminals)
				{
					if (t.SettingsFileExists)
						filePaths.Add(t.SettingsFilePath);
				}
				return (filePaths);
			}
		}

		#endregion

		#region Terminals > General Methods
		//------------------------------------------------------------------------------------------
		// Terminals > General Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool CreateNewTerminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			OnFixedStatusTextRequest("Creating new terminal...");

			// Create terminal
			Terminal terminal = new Terminal(settingsHandler);
			AddToWorkspace(terminal);

			OnTimedStatusTextRequest("New terminal created");
			return (true);
		}

		/// <summary>
		/// Opens terminals according to workspace settings and returns number of successfully
		/// opened terminals.
		/// </summary>
		public virtual int OpenTerminals()
		{
			int requestedTerminalCount = _settingsRoot.TerminalSettings.Count;
			if (requestedTerminalCount == 1)
				OnFixedStatusTextRequest("Opening workspace terminal...");
			else if (requestedTerminalCount > 1)
				OnFixedStatusTextRequest("Opening workspace terminals...");

			int openedTerminalCount = 0;
			GuidList<TerminalSettingsItem> clone = new GuidList<TerminalSettingsItem>(_settingsRoot.TerminalSettings);
			foreach (TerminalSettingsItem item in clone)
			{
				try
				{
					if (OpenTerminalFromFile(item.FilePath, item.Guid, item.Window, true))
						openedTerminalCount++;
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
						"Terminal File Error",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Exclamation
						);
					OnTimedStatusTextRequest("Terminal not opened!");

					if (result == DialogResult.No)
						break;
				}
			}

			// On success, clear changed flag since all terminals got openend
			if (openedTerminalCount == requestedTerminalCount)
				_settingsRoot.ClearChanged();

			return (openedTerminalCount);
		}

		/// <summary></summary>
		public virtual bool OpenTerminalFromFile(string filePath)
		{
			return (OpenTerminalFromFile(filePath, Guid.Empty, null, false));
		}

		private bool OpenTerminalFromFile(string filePath, Guid guid, Settings.WindowSettings windowSettings, bool suppressErrorHandling)
		{
			string absoluteFilePath = filePath;

			OnFixedStatusTextRequest("Opening terminal...");
			try
			{
				// Combine absolute workspace path with terminal path if that one is relative
				absoluteFilePath = XPath.CombineFilePaths(_settingsHandler.SettingsFilePath, filePath);

				// Check whether terminal is already contained in workspace
				foreach (Terminal t in _terminals)
				{
					if (absoluteFilePath == t.SettingsFilePath)
					{
						OnFixedStatusTextRequest("Terminal is already open.");
						OnMessageInputRequest
							(
							"Terminal is already open and will not be re-openend.",
							"Terminal File Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
							);
						OnTimedStatusTextRequest("Terminal not re-opened.");
						return (false);
					}
				}

				// Load settings
				DocumentSettingsHandler<TerminalSettingsRoot> sh = new DocumentSettingsHandler<TerminalSettingsRoot>();
				sh.SettingsFilePath = absoluteFilePath;
				sh.Load();

				// Replace window settings with those saved in workspace
				if (windowSettings != null)
					sh.Settings.Window = windowSettings;

				// Create terminal
				Terminal terminal = new Terminal(sh, guid);
				AddToWorkspace(terminal);

				if (!sh.Settings.AutoSaved)
					SetRecent(absoluteFilePath);

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
						"Invalid Terminal File",
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

		private TerminalSettingsItem CreateTerminalSettingsItem(Terminal terminal)
		{
			TerminalSettingsItem tsi = new TerminalSettingsItem();

			string filePath = terminal.SettingsFilePath;
			if (ApplicationSettings.LocalUser.General.UseRelativePaths)
			{
				XPathCompareResult pcr = XPath.CompareFilePaths(_settingsHandler.SettingsFilePath, terminal.SettingsFilePath);
				if (pcr.AreRelative)
					filePath = pcr.RelativePath;
			}

			tsi.Guid = terminal.Guid;
			tsi.FilePath = filePath;
			tsi.Window = new WindowSettings(terminal.WindowSettings); // Clone window settings

			return (tsi);
		}

		private void AddToWorkspace(Terminal terminal)
		{
			AttachTerminalEventHandlers(terminal);

			// Add terminal to terminal list
			_terminals.Add(terminal);
			_activeTerminal = terminal;

			// Add terminal settings for new terminals
			// Replace terminal settings if workspace settings have been loaded from file prior
			_settingsRoot.TerminalSettings.AddOrReplaceGuidItem(CreateTerminalSettingsItem(terminal));
			_settingsRoot.SetChanged();

			// Fire terminal added event
			OnTerminalAdded(new TerminalEventArgs(terminal));
		}

		private void ReplaceInWorkspace(Terminal terminal)
		{
			// Replace terminal in terminal list
			_terminals.ReplaceGuidItem(terminal);
			_activeTerminal = terminal;

			// Replace terminal in workspace settings if the settings have indeed changed
			TerminalSettingsItem tsiNew = CreateTerminalSettingsItem(terminal);
			TerminalSettingsItem tsiOld = _settingsRoot.TerminalSettings.GetGuidItem(terminal.Guid);
			if ((tsiOld == null) || (tsiNew != tsiOld))
			{
				_settingsRoot.TerminalSettings.ReplaceGuidItem(tsiNew);
				_settingsRoot.SetChanged();
			}
		}

		private void RemoveFromWorkspace(Terminal terminal)
		{
			DetachTerminalEventHandlers(terminal);

			// Remove terminal from terminal list
			_terminals.RemoveGuid(terminal.Guid);
			_activeTerminal = null;

			// Remove terminal from workspace settings
			_settingsRoot.TerminalSettings.RemoveGuid(terminal.Guid);
			_settingsRoot.SetChanged();

			// Fire terminal added event
			OnTerminalRemoved(new TerminalEventArgs(terminal));
		}

		#endregion

		#region Terminals > All Terminals Methods
		//------------------------------------------------------------------------------------------
		// Terminals > All Terminals Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool SaveAllTerminals()
		{
			return (SaveAllTerminals(true));
		}

		/// <summary></summary>
		public virtual bool SaveAllTerminals(bool autoSaveIsAllowed)
		{
			bool success = true;

			// Calling Close() on a terminal will modify the list, therefore clone it first
			List<Terminal> clone = new List<Terminal>(_terminals);
			foreach (Terminal t in clone)
			{
				if (!t.Save(autoSaveIsAllowed))
					success = false;
			}
			return (success);
		}

		/// <summary></summary>
		public virtual bool CloseAllTerminals()
		{
			return (CloseAllTerminals(false, false));
		}

		/// <remarks>
		/// See remarks of <see cref="Terminal.Close(bool, bool)"/> for details on 'WorkspaceClose'.
		/// </remarks>
		private bool CloseAllTerminals(bool isWorkspaceClose, bool tryAutoSave)
		{
			bool success = true;

			// Calling Close() on a terminal will modify the list, therefore clone it first
			List<Terminal> clone = new List<Terminal>(_terminals);
			foreach (Terminal t in clone)
			{
				if (!t.Close(isWorkspaceClose, TryTerminalAutoSaveIsDesired(tryAutoSave, t)))
					success = false;
			}
			return (success);
		}

		#endregion

		#region Terminals > Active Terminal Methods
		//------------------------------------------------------------------------------------------
		// Terminals > Active Terminal Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void ActivateTerminal(Terminal terminal)
		{
			_activeTerminal = terminal;
		}

		/// <summary></summary>
		public virtual bool SaveActiveTerminal()
		{
			return (_activeTerminal.Save());
		}

		/// <summary></summary>
		public virtual bool CloseActiveTerminal()
		{
			return (_activeTerminal.Close());
		}

		/// <summary></summary>
		public virtual bool OpenActiveTerminalIO()
		{
			return (_activeTerminal.StartIO());
		}

		/// <summary></summary>
		public virtual bool CloseActiveTerminalIO()
		{
			return (_activeTerminal.StopIO());
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
		protected virtual DialogResult OnSaveAsFileDialogRequest()
		{
			DialogEventArgs e = new DialogEventArgs();
			EventHelper.FireSync<DialogEventArgs>(SaveAsFileDialogRequest, this, e);
			return (e.Result);
		}

		/// <summary></summary>
		protected virtual void OnSaved(SavedEventArgs e)
		{
			EventHelper.FireSync<SavedEventArgs>(Saved, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(ClosedEventArgs e)
		{
			EventHelper.FireSync<ClosedEventArgs>(Closed, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
