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

using MKY.Utilities.Guid;
using MKY.Utilities.Event;
using MKY.Utilities.Recent;
using MKY.Utilities.Settings;
using MKY.Utilities.Time;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;

using YAT.Model.Types;
using YAT.Model.Settings;
using YAT.Model.Utilities;

namespace YAT.Model
{
	/// <summary></summary>
	public class Terminal : IDisposable, IGuidProvider
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string _TerminalText = "Terminal";

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		/// <summary>
		/// Static counter to number terminals. Counter is incremented before first use, first
		/// terminal therefore is "Terminal1".
		/// </summary>
		private static int _terminalIdCounter = 0;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private Guid _guid;
		private string _userName;

		// settings
		private DocumentSettingsHandler<TerminalSettingsRoot> _settingsHandler;
		private TerminalSettingsRoot _settingsRoot;

		// terminal
		private Domain.Terminal _terminal;

		// logs
		private Log.Logs _log;

		// time status
		private Chronometer _ioConnectChrono;

		// count status
		private int _txByteCount;
		private int _rxByteCount;
		private int _txLineCount;
		private int _rxLineCount;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;
		/// <summary></summary>
		public event EventHandler IOControlChanged;
		/// <summary></summary>
		public event EventHandler<TimeSpanEventArgs> IOConnectTimeChanged;
		/// <summary></summary>
		public event EventHandler IOCountChanged;
		/// <summary></summary>
		public event EventHandler<Domain.IORequestEventArgs> IORequest;
		/// <summary></summary>
		public event EventHandler<Domain.IOErrorEventArgs> IOError;

		/// <summary></summary>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsSent;
		/// <summary></summary>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsReceived;
		/// <summary></summary>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesSent;
		/// <summary></summary>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesReceived;

		/// <summary></summary>
		public event EventHandler<Domain.RepositoryEventArgs> RepositoryCleared;
		/// <summary></summary>
		public event EventHandler<Domain.RepositoryEventArgs> RepositoryReloaded;

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
		public Terminal()
			: this(new DocumentSettingsHandler<TerminalSettingsRoot>(), Guid.NewGuid())
		{
		}

		/// <summary></summary>
		public Terminal(TerminalSettingsRoot settings)
			: this(new DocumentSettingsHandler<TerminalSettingsRoot>(settings), Guid.NewGuid())
		{
		}

		/// <summary></summary>
		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
			: this(settingsHandler, Guid.NewGuid())
		{
		}

		/// <summary></summary>
		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid)
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

			// set user name
			_terminalIdCounter++;
			if (!_settingsHandler.SettingsFilePathIsValid || _settingsRoot.AutoSaved)
				_userName = _TerminalText + _terminalIdCounter.ToString();
			else
				UserNameFromFile = _settingsHandler.SettingsFilePath;

			// Create underlying terminal
			_terminal = Domain.TerminalFactory.CreateTerminal(_settingsRoot.Terminal);
			AttachTerminalEventHandlers();

			// Create log
			_log = new Log.Logs(_settingsRoot.Log);

			// Create chrono
			_ioConnectChrono = new Chronometer();
			_ioConnectChrono.Interval = 1000;
			_ioConnectChrono.TimeSpanChanged += new EventHandler<TimeSpanEventArgs>(_ioConnectChrono_TimeSpanChanged);
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
					// First, detach event handlers to ensure that no more events are received
					DetachTerminalEventHandlers();
					DetachSettingsEventHandlers();

					// Then, dispose of objects
					if (_ioConnectChrono != null)
					{
						_ioConnectChrono.Dispose();
						_ioConnectChrono = null;
					}
					if (_log != null)
					{
						_log.Dispose();
						_log = null;
					}
					if (_terminal != null)
					{
						_terminal.Dispose();
						_terminal = null;
					}
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~Terminal()
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
				return (_userName);
			}
		}

		private string UserNameFromFile
		{
			set
			{
				_userName = Path.GetFileNameWithoutExtension(value);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.IsStarted);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.IsOpen);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.IsConnected);
			}
		}

		/// <summary></summary>
		public virtual bool LogIsStarted
		{
			get
			{
				AssertNotDisposed();
				return (_log.IsStarted);
			}
		}

		/// <summary></summary>
		public virtual MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.UnderlyingIOInstance);
			}
		}

		#endregion

		#region General Methods
		//==========================================================================================
		// General Methods
		//==========================================================================================

		/// <summary>
		/// Starts terminal, i.e. starts log and open I/O.
		/// </summary>
		public virtual void Start()
		{
			// begin logging (in case opening of terminal needs to be logged)
			if (_settingsRoot.LogIsStarted)
				BeginLog();

			// then open terminal
			if (_settingsRoot.TerminalIsStarted)
				StartIO();
		}

		/// <summary>
		/// Sets terminal settings
		/// </summary>
		public virtual void SetSettings(Domain.Settings.TerminalSettings settings)
		{
			// Settings have changed, recreate terminal with new settings
			if (_terminal.IsStarted)
			{
				// Terminal is open, re-open it with the new settings
				if (StopIO(false))
				{
					DetachTerminalEventHandlers();    // Detach to suspend events
					_settingsRoot.Terminal = settings;
					_terminal = Domain.TerminalFactory.RecreateTerminal(_settingsRoot.Terminal, _terminal);
					AttachTerminalEventHandlers();    // Attach and resume events
					_terminal.ReloadRepositories();

					StartIO(false);

					OnTimedStatusTextRequest("Terminal settings applied.");
				}
				else
				{
					OnTimedStatusTextRequest("Terminal settings not applied!");
				}
			}
			else
			{
				// Terminal is closed, simply set the new settings
				DetachTerminalEventHandlers();        // Detach to suspend events
				_settingsRoot.Terminal = settings;
				_terminal = Domain.TerminalFactory.RecreateTerminal(_settingsRoot.Terminal, _terminal);
				AttachTerminalEventHandlers();        // Attach and resume events
				_terminal.ReloadRepositories();

				OnTimedStatusTextRequest("Terminal settings applied.");
			}
		}

		/// <summary>
		/// Sets log settings
		/// </summary>
		public virtual void SetLogSettings(Log.Settings.LogSettings settings)
		{
			_settingsRoot.Log = settings;
			_log.Settings = _settingsRoot.Log;
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

		private void _settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			// Nothing to do yet
		}

		#endregion

		#region Settings > Properties
		//------------------------------------------------------------------------------------------
		// Settings > Properties
		//------------------------------------------------------------------------------------------

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

		/// <summary></summary>
		public virtual TerminalSettingsRoot SettingsRoot
		{
			get
			{
				AssertNotDisposed();
				return (_settingsRoot);
			}
		}

		/// <summary></summary>
		public virtual WindowSettings WindowSettings
		{
			get
			{
				AssertNotDisposed();
				return (_settingsRoot.Window);
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
		/// Saves terminal to file, prompts for file if it doesn't exist yet
		/// </summary>
		public virtual bool Save()
		{
			return (Save(true));
		}

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet
		/// </summary>
		public virtual bool Save(bool autoSaveIsAllowed)
		{
			AssertNotDisposed();

			bool success = false;

			// Save terminal if file path is valid
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
		/// Saves terminal to given file
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
			// Save terminal
			// -------------------------------------------------------------------------------------

			bool success = false;

			if (!doAutoSave)
				OnFixedStatusTextRequest("Saving terminal...");

			if (doAutoSave && (!_settingsHandler.SettingsFilePathIsValid))
			{
				string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveTerminalFileNamePrefix + Guid.ToString() + ExtensionSettings.TerminalFile;
				_settingsHandler.SettingsFilePath = autoSaveFilePath;
			}

			try
			{
				_settingsHandler.Settings.AutoSaved = doAutoSave;
				_settingsHandler.Save();

				if (!doAutoSave)
					UserNameFromFile = _settingsHandler.SettingsFilePath;

				success = true;
				OnSaved(new SavedEventArgs(_settingsHandler.SettingsFilePath, doAutoSave));

				if (!doAutoSave)
				{
					SetRecent(_settingsHandler.SettingsFilePath);
					OnTimedStatusTextRequest("Terminal saved");
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
					OnFixedStatusTextRequest("Error saving terminal!");
					OnMessageInputRequest
						(
						"Unable to save file" + Environment.NewLine + _settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					OnTimedStatusTextRequest("Terminal not saved!");
				}
			}
			return (success);
		}

		#endregion

		#region Close
		//==========================================================================================
		// Close
		//==========================================================================================

		/// <summary>Closes the terminal and prompts if needed if settings have changed.</summary>
		public virtual bool Close()
		{
			return (Close(false, false));
		}

		/// <summary>
		/// Closes the terminal and tries to auto save if desired.
		/// </summary>
		/// <remarks>
		/// \attention
		/// This method is needed for MDI applications. In case of MDI parent/application closing,
		/// Close() of the terminal is called before Close() of the workspace. Without taking care
		/// of this, the workspace would be saved after the terminal has already been close, i.e.
		/// removed from the workspace. Therefore, the terminal has to signal such cases to the
		/// workspace.
		/// 
		/// Cases (similar to cases in Model.Workspace):
		/// - Workspace close
		///   - auto,   no file,       auto save    => auto save, if it fails => nothing  : (w1a)
		///   - auto,   no file,       no auto save => nothing                            : (w1b)
		///   - auto,   existing file, auto save    => auto save, if it fails => delete   : (w2a)
		///   - auto,   existing file, no auto save => delete                             : (w2b)
		///   - normal, no file                     => N/A (normal files have been saved) : (w3)
		///   - normal, existing file, auto save    => auto save, if it fails => question : (w4a)
		///   - normal, existing file, no auto save => question                           : (w4b)
		/// - Terminal close
		///   - auto,   no file                     => nothing                            : (t1)
		///   - auto,   existing file               => delete                             : (t2)
		///   - normal, no file                     => N/A (normal files have been saved) : (t3)
		///   - normal, existing file, auto save    => auto save, if it fails => question : (t4a)
		///   - normal, existing file, no auto save => question                           : (t4b)
		/// </remarks>
		public virtual bool Close(bool isWorkspaceClose, bool tryAutoSave)
		{
			// Don't try to auto save if there is no existing file (w1)
			if (!isWorkspaceClose && !_settingsHandler.SettingsFileExists)
				tryAutoSave = false;

			OnFixedStatusTextRequest("Closing terminal...");

			bool success = false;

			// Try to auto save if desired
			if (tryAutoSave)
				success = TryAutoSave();

			// No success on auto save or auto save not desired
			if (!success)
			{
				// No file (w1, w3, t1, t3)
				if (!_settingsHandler.SettingsFileExists)
				{
					success = true; // Consider it successful if there was no file to save
				}
				// Existing file
				else
				{
					if (_settingsRoot.AutoSaved) // Existing auto file (w2a/b, t2)
					{
						_settingsHandler.Delete();
						success = true; // Don't care if auto file not successfully deleted
					}

					// Existing normal file (w4a/b, t4a/b) will be handled below
				}

				// Normal (w4a/b, t4a/b)
				if (!success && _settingsRoot.ExplicitHaveChanged)
				{
					DialogResult dr = OnMessageInputRequest
						(
						"Save terminal?",
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
							OnTimedStatusTextRequest("Terminal not closed");
							return (false);
					}
				}
				else // Else means settings have not changed
				{
					success = true; // Consider it successful if there was nothing to save
				}
			} // End of if no success on auto save or auto save disabled

			// Next, close underlying terminal
			if (_terminal.IsStarted)
				success = StopIO(false);

			// Last, close log
			if (_log.IsStarted)
				EndLog();

			if (success)
			{
				// Status text request must be before closed event, closed event may close the view
				OnTimedStatusTextRequest("Terminal successfully closed");
				OnClosed(new ClosedEventArgs(isWorkspaceClose));
			}
			else
			{
				OnTimedStatusTextRequest("Terminal not closed");
			}
			return (success);
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

		#region Terminal
		//==========================================================================================
		// Terminal
		//==========================================================================================

		#region Terminal > Lifetime
		//------------------------------------------------------------------------------------------
		// Terminal > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachTerminalEventHandlers()
		{
			if (_terminal != null)
			{
				_terminal.IOChanged        += new EventHandler(_terminal_IOChanged);
				_terminal.IOControlChanged += new EventHandler(_terminal_IOControlChanged);
				_terminal.IORequest        += new EventHandler<Domain.IORequestEventArgs>(_terminal_IORequest);
				_terminal.IOError          += new EventHandler<Domain.IOErrorEventArgs>(_terminal_IOError);

				_terminal.RawElementSent          += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
				_terminal.RawElementReceived      += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
				_terminal.DisplayElementsSent     += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
				_terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
				_terminal.DisplayLinesSent        += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
				_terminal.DisplayLinesReceived    += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
				_terminal.RepositoryCleared       += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
				_terminal.RepositoryReloaded      += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (_terminal != null)
			{
				_terminal.IOChanged        -= new EventHandler(_terminal_IOChanged);
				_terminal.IOControlChanged -= new EventHandler(_terminal_IOControlChanged);
				_terminal.IORequest        -= new EventHandler<Domain.IORequestEventArgs>(_terminal_IORequest);
				_terminal.IOError          -= new EventHandler<Domain.IOErrorEventArgs>(_terminal_IOError);

				_terminal.RawElementSent          -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
				_terminal.RawElementReceived      -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
				_terminal.DisplayElementsSent     -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
				_terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
				_terminal.DisplayLinesSent        -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
				_terminal.DisplayLinesReceived    -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
				_terminal.RepositoryCleared       -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
				_terminal.RepositoryReloaded      -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
			}
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Local field to maintain connection state in order to be able to detect a change of the
		/// connection state.
		/// </summary>
		private bool _terminal_IOChanged_isConnected;

		private void _terminal_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(e);

			if      ( _terminal.IsConnected && !_terminal_IOChanged_isConnected)
				_ioConnectChrono.Start();
			else if (!_terminal.IsConnected &&  _terminal_IOChanged_isConnected)
				_ioConnectChrono.Stop();

			_terminal_IOChanged_isConnected = _terminal.IsConnected;
		}

		private void _terminal_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void _terminal_IORequest(object sender, Domain.IORequestEventArgs e)
		{
			OnIORequest(e);
		}

		private void _terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			OnIOError(e);
		}

		private void _terminal_RawElementSent(object sender, Domain.RawElementEventArgs e)
		{
			// Count
			_txByteCount += e.Element.Data.Length;
			OnIOCountChanged(new EventArgs());

			// Log
			if (_log.IsStarted)
			{
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawTx);
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
			}
		}

		private void _terminal_RawElementReceived(object sender, Domain.RawElementEventArgs e)
		{
			// Count
			_rxByteCount += e.Element.Data.Length;
			OnIOCountChanged(new EventArgs());

			// Log
			if (_log.IsStarted)
			{
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawRx);
			}
		}

		private void _terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			// Display
			OnDisplayElementsSent(e);

			// Log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (_log.IsStarted)
				{
					if (de is Domain.DisplayElement.LineBreak)
					{
						_log.WriteEol(Log.LogStreams.NeatTx);
						_log.WriteEol(Log.LogStreams.NeatBidir);
					}
					else
					{
						_log.WriteString(de.Text, Log.LogStreams.NeatTx);
						_log.WriteString(de.Text, Log.LogStreams.NeatBidir);
					}
				}
			}
		}

		private void _terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			// Display
			OnDisplayElementsReceived(e);

			// Log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (_log.IsStarted)
				{
					if (de is Domain.DisplayElement.LineBreak)
					{
						_log.WriteEol(Log.LogStreams.NeatBidir);
						_log.WriteEol(Log.LogStreams.NeatRx);
					}
					else
					{
						_log.WriteString(de.Text, Log.LogStreams.NeatBidir);
						_log.WriteString(de.Text, Log.LogStreams.NeatRx);
					}
				}
			}
		}

		private void _terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count
			_txLineCount += e.Lines.Count;
			OnIOCountChanged(new EventArgs());

			// Display
			OnDisplayLinesSent(e);
		}

		private void _terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count
			_rxLineCount += e.Lines.Count;
			OnIOCountChanged(new EventArgs());

			// Display
			OnDisplayLinesReceived(e);
		}

		private void _terminal_RepositoryCleared(object sender, Domain.RepositoryEventArgs e)
		{
			OnRepositoryCleared(e);
		}

		private void _terminal_RepositoryReloaded(object sender, Domain.RepositoryEventArgs e)
		{
			OnRepositoryReloaded(e);
		}

		#endregion

		#region Terminal > Start/Stop
		//------------------------------------------------------------------------------------------
		// Terminal > Start/Stop
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public virtual bool StartIO()
		{
			return (StartIO(true));
		}

		/// <summary>
		/// Starts the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		private bool StartIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Starting terminal...");
			try
			{
				if (_terminal.Start())
				{
					if (saveStatus)
						_settingsRoot.TerminalIsStarted = _terminal.IsStarted;

					OnTimedStatusTextRequest("Terminal started");
					success = true;
				}
			}
			catch (Exception ex)
			{
				OnFixedStatusTextRequest("Error starting terminal!");

				string ioText;
				if (_settingsRoot.IOType == Domain.IOType.SerialPort)
					ioText = "Port";
				else
					ioText = "Socket";

				OnMessageInputRequest
					(
					"Unable to start terminal:" + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					ioText + " could be in use by another process.",
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				OnTimedStatusTextRequest("Terminal not started!");
			}

			return (success);
		}

		/// <summary>
		/// Stops the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public virtual bool StopIO()
		{
			return (StopIO(true));
		}

		/// <summary>
		/// Stops the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		private bool StopIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Stopping terminal...");
			try
			{
				_terminal.Stop();

				if (saveStatus)
					_settingsRoot.TerminalIsStarted = _terminal.IsStarted;

				OnTimedStatusTextRequest("Terminal stopped");
				success = true;
			}
			catch (Exception ex)
			{
				OnTimedStatusTextRequest("Error stopping terminal!");

				OnMessageInputRequest
					(
					"Unable to stop terminal:" + Environment.NewLine + Environment.NewLine + ex.Message,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				OnTimedStatusTextRequest("Terminal not stopped!");
			}

			return (success);
		}

		#endregion

		#region Terminal > IO Control
		//------------------------------------------------------------------------------------------
		// Terminal > IO Control
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Toggles RTS line if current flow control settings allow this
		/// </summary>
		public virtual void RequestToggleRts()
		{
			if (_settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.SerialFlowControl.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleRts();
				_settingsRoot.Terminal.IO.SerialPort.RtsEnabled = port.RtsEnable;
			}
		}

		/// <summary>
		/// Toggles DTR line if current flow control settings allow this
		/// </summary>
		public virtual void RequestToggleDtr()
		{
			if (_settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.SerialFlowControl.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleDtr();
				_settingsRoot.Terminal.IO.SerialPort.DtrEnabled = port.DtrEnable;
			}
		}

		#endregion

		#region Terminal > Send
		//------------------------------------------------------------------------------------------
		// Terminal > Send
		//------------------------------------------------------------------------------------------

		private void Send(byte[] b)
		{
			OnFixedStatusTextRequest("Sending " + b.Length + " bytes...");
			try
			{
				_terminal.Send(b);
				OnTimedStatusTextRequest(b.Length + " bytes sent");
			}
			catch (System.IO.IOException ex)
			{
				OnFixedStatusTextRequest("Error sending " + b.Length + " bytes!");

				string text = "Unable to write to ";
				string title;
				switch (_settingsRoot.IOType)
				{
					case Domain.IOType.SerialPort: text += "port"; title = "Serial Port"; break;
					default: text += "socket"; title = "Socket"; break;
				}
				text += ":";
				title += " Error";
				OnMessageInputRequest
					(
					text + Environment.NewLine + Environment.NewLine + ex.Message,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				OnTimedStatusTextRequest("Data not sent!");
			}
		}

		private void SendLine(string s)
		{
			OnFixedStatusTextRequest(@"Sending """ + s + @"""...");
			try
			{
				_terminal.SendLine(s);
				OnTimedStatusTextRequest(@"""" + s + @""" sent");
			}
			catch (System.IO.IOException ex)
			{
				OnFixedStatusTextRequest(@"Error sending """ + s + @"""!");

				string text = "Unable to write to ";
				string title;
				switch (_settingsRoot.IOType)
				{
					case Domain.IOType.SerialPort: text += "port"; title = "Serial Port"; break;
					default: text += "socket"; title = "Socket"; break;
				}
				text += ":";
				title += " Error";
				OnMessageInputRequest
					(
					text + Environment.NewLine + Environment.NewLine + ex.Message,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				OnTimedStatusTextRequest("Data not sent!");
			}
			catch (Domain.Parser.FormatException ex)
			{
				OnFixedStatusTextRequest(@"Error sending """ + s + @"""!");
				OnMessageInputRequest
					(
					"Bad data format:" + Environment.NewLine + Environment.NewLine + ex.Message,
					"Format Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
				OnTimedStatusTextRequest("Data not sent!");
			}
		}

		#endregion

		#region Terminal > Send Command
		//------------------------------------------------------------------------------------------
		// Terminal > Send Command
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends command given by terminal settings.
		/// </summary>
		public virtual void SendCommand()
		{
			SendCommand(_settingsRoot.SendCommand.Command);
			_settingsRoot.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary
				(
				new RecentItem<Command>(new Command(_settingsRoot.SendCommand.Command))
				);

			// Clear command if desired
			if (!_settingsRoot.Send.KeepCommand)
				_settingsRoot.SendCommand.Command = new Command(); // set command to ""
		}

		/// <summary>
		/// Sends given command.
		/// </summary>
		/// <param name="command">Command to be sent.</param>
		public virtual void SendCommand(Command command)
		{
			if (command.IsValidCommand)
			{
				if (command.IsSingleLineCommand)
				{
					if (SendCommandSettings.IsEasterEggCommand(command.SingleLineCommand))
						SendLine(SendCommandSettings.EasterEggCommandText);
					else
						SendLine(command.SingleLineCommand);
				}
				else
				{
					foreach (string line in command.MultiLineCommand)
						SendLine(line);
				}
			}
			else
			{
				SendLine("");
			}
		}

		#endregion

		#region Terminal > Send File
		//------------------------------------------------------------------------------------------
		// Terminal > Send File
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends file given by terminal settings
		/// </summary>
		public virtual void SendFile()
		{
			SendFile(_settingsRoot.SendFile.Command);
		}

		/// <summary>
		/// Sends given file
		/// </summary>
		/// <param name="command">File to be sent</param>
		public virtual void SendFile(Command command)
		{
			if (!command.IsValidFilePath)
				return;

			string filePath = command.FilePath;

			try
			{
				if (_terminal is Domain.TextTerminal)
				{
					string[] lines;
					if (ExtensionSettings.IsXmlFile(System.IO.Path.GetExtension(filePath)))
					{
						// xml
						lines = XmlReader.LinesFromXmlFile(filePath);
					}
					else if (ExtensionSettings.IsRtfFile(System.IO.Path.GetExtension(filePath)))
					{
						// rtf
						lines = RtfReader.LinesFromRtfFile(filePath);
					}
					else
					{
						// text
						using (StreamReader sr = new StreamReader(filePath))
						{
							string s;
							List<string> l = new List<string>();
							while ((s = sr.ReadLine()) != null)
							{
								l.Add(s);
							}
							sr.Close();                   // close file before sending
							lines = l.ToArray();
						}
					}

					foreach (string line in lines)
					{
						SendLine(line);
					}
				}
				else
				{
					using (FileStream fs = File.OpenRead(filePath))
					{
						byte[] a = new byte[(int)fs.Length];
						fs.Read(a, 0, (int)fs.Length);
						fs.Close();                   // close file before sending
						Send(a);
					}
				}
			}
			catch (Exception e)
			{
				OnMessageInputRequest
					(
					"Error while accessing file" + Environment.NewLine +
					filePath + Environment.NewLine + Environment.NewLine +
					e.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
			}
		}

		#endregion

		#region Terminal > Send Predefined
		//------------------------------------------------------------------------------------------
		// Terminal > Send Predefined
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Send requested predefined command.
		/// </summary>
		/// <param name="page">Page 1..max</param>
		/// <param name="command">Command 1..max</param>
		public virtual void SendPredefined(int page, int command)
		{
			Model.Types.Command c = _settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];

			if (c.IsValidCommand)
			{
				SendCommand(c);

				if (_settingsRoot.Send.CopyPredefined)
					_settingsRoot.SendCommand.Command = new Command(c); // copy command if desired
			}
			else if (c.IsValidFilePath)
			{
				SendFile(c);

				if (_settingsRoot.Send.CopyPredefined)
					_settingsRoot.SendFile.Command = new Command(c); // copy command if desired
			}
		}

		#endregion

		#region Terminal > Repositories
		//------------------------------------------------------------------------------------------
		// Terminal > Repositories
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Forces complete reload of repositories.
		/// </summary>
		public virtual void ReloadRepositories()
		{
			AssertNotDisposed();
			_terminal.ReloadRepositories();
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual List<Domain.DisplayLine> RepositoryToDisplayLines(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (_terminal.RepositoryToDisplayLines(repositoryType));
		}

		/// <summary>
		/// Returns contents of desired repository as string.
		/// </summary>
		/// <remarks>
		/// Can be used for debugging purposes.
		/// </remarks>
		public virtual string RepositoryToString(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (_terminal.RepositoryToString(repositoryType));
		}

		/// <summary>
		/// Clears given repository.
		/// </summary>
		public virtual void ClearRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			_terminal.ClearRepository(repositoryType);
		}

		/// <summary>
		/// Clears all repositories.
		/// </summary>
		public virtual void ClearRepositories()
		{
			AssertNotDisposed();
			_terminal.ClearRepositories();
		}

		#endregion

		#region Terminal > Time Status
		//------------------------------------------------------------------------------------------
		// Terminal > Time Status
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual TimeSpan IOConnectTime
		{
			get
			{
				AssertNotDisposed();
				return (_ioConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		public virtual void RestartIOConnectTime()
		{
			AssertNotDisposed();
			_ioConnectChrono.Restart();
		}

		private void _ioConnectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			OnIOConnectTimeChanged(e);
		}

		#endregion

		#region Terminal > Count Status
		//------------------------------------------------------------------------------------------
		// Terminal > Count Status
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual int TxByteCount
		{
			get { return (_txByteCount); }
		}

		/// <summary></summary>
		public virtual int TxLineCount
		{
			get { return (_txLineCount); }
		}

		/// <summary></summary>
		public virtual int RxByteCount
		{
			get { return (_rxByteCount); }
		}

		/// <summary></summary>
		public virtual int RxLineCount
		{
			get { return (_rxLineCount); }
		}

		/// <summary></summary>
		public virtual void ResetIOCount()
		{
			_txByteCount = 0;
			_txLineCount = 0;
			_rxByteCount = 0;
			_rxLineCount = 0;

			OnIOCountChanged(new EventArgs());
		}

		#endregion

		#endregion

		#region Log
		//==========================================================================================
		// Log
		//==========================================================================================

		/// <summary></summary>
		public virtual void BeginLog()
		{
			try
			{
				// reapply settings NOW, makes sure date/time in filenames is refreshed
				_log.Settings = _settingsRoot.Log;
				_log.Begin();
				_settingsRoot.LogIsStarted = true;
			}
			catch (System.IO.IOException ex)
			{
				OnMessageInputRequest
					(
					"Unable to begin log." + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					"Log file may be in use by another process.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
			}
		}

		/// <summary></summary>
		public virtual void ClearLog()
		{
			try
			{
				_log.Clear();
			}
			catch (System.IO.IOException ex)
			{
				OnMessageInputRequest
					(
					"Unable to clear log." + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					"Log file may be in use by another process.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
			}
		}

		/// <summary></summary>
		public virtual void EndLog()
		{
			EndLog(true);
		}

		/// <summary></summary>
		public virtual void EndLog(bool saveStatus)
		{
			try
			{
				_log.End();

				if (saveStatus)
					_settingsRoot.LogIsStarted = false;
			}
			catch (System.IO.IOException ex)
			{
				OnMessageInputRequest
					(
					"Unable to end log." + Environment.NewLine + Environment.NewLine +
					ex.Message,
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			EventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			EventHelper.FireSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOConnectTimeChanged(TimeSpanEventArgs e)
		{
			EventHelper.FireSync<TimeSpanEventArgs>(IOConnectTimeChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOCountChanged(EventArgs e)
		{
			EventHelper.FireSync(IOCountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIORequest(Domain.IORequestEventArgs e)
		{
			EventHelper.FireSync<Domain.IORequestEventArgs>(IORequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(Domain.IOErrorEventArgs e)
		{
			EventHelper.FireSync<Domain.IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(Domain.DisplayElementsEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(Domain.DisplayElementsEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesSent(Domain.DisplayLinesEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(Domain.DisplayLinesEventArgs e)
		{
			EventHelper.FireSync<Domain.DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(Domain.RepositoryEventArgs e)
		{
			EventHelper.FireSync<Domain.RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(Domain.RepositoryEventArgs e)
		{
			EventHelper.FireSync<Domain.RepositoryEventArgs>(RepositoryReloaded, this, e);
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
