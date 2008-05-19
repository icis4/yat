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
		public event EventHandler IOConnectTimeChanged;
		/// <summary></summary>
		public event EventHandler IOCountChanged;
		/// <summary></summary>
		public event EventHandler<Domain.ErrorEventArgs> IOError;

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
		{
			Initialize(new DocumentSettingsHandler<TerminalSettingsRoot>(), Guid.NewGuid());
		}

		/// <summary></summary>
		public Terminal(TerminalSettingsRoot settings)
		{
			Initialize(new DocumentSettingsHandler<TerminalSettingsRoot>(settings), Guid.NewGuid());
		}

		/// <summary></summary>
		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			Initialize(settingsHandler, Guid.NewGuid());
		}

		/// <summary></summary>
		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid)
		{
			Initialize(settingsHandler, guid);
		}

		private void Initialize(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid)
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

			// create underlying terminal
			_terminal = Domain.TerminalFactory.CreateTerminal(_settingsRoot.Terminal);
			AttachTerminalEventHandlers();

			// create log
			_log = new Log.Logs(_settingsRoot.Log);

			// create chrono
			_ioConnectChrono = new Chronometer();
			_ioConnectChrono.Interval = 1000;
			_ioConnectChrono.TimeSpanChanged += new EventHandler(_ioConnectChrono_TimeSpanChanged);
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
					if (_ioConnectChrono != null)
						_ioConnectChrono.Dispose();
					if (_log != null)
						_log.Dispose();
					if (_terminal != null)
						_terminal.Dispose();
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
		public bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.IsOpen);
			}
		}

		/// <summary></summary>
		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.IsConnected);
			}
		}

		/// <summary></summary>
		public bool LogIsOpen
		{
			get
			{
				AssertNotDisposed();
				return (_log.IsOpen);
			}
		}

		/// <summary></summary>
		public MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (_terminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public object UnderlyingIOInstance
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
		public void Start()
		{
			// begin logging (in case opening of terminal needs to be logged)
			if (_settingsRoot.LogIsOpen)
				BeginLog();

			// then open terminal
			if (_settingsRoot.TerminalIsOpen)
				OpenIO();
		}

		/// <summary>
		/// Sets terminal settings
		/// </summary>
		public void SetSettings(Domain.Settings.TerminalSettings settings)
		{
			// settings have changed, recreate terminal with new settings
			if (_terminal.IsOpen)
			{
				// terminal is open, re-open it with the new settings
				if (CloseIO(false))
				{
					DetachTerminalEventHandlers();    // detach to suspend events
					_settingsRoot.Terminal = settings;
					_terminal = Domain.TerminalFactory.RecreateTerminal(_settingsRoot.Terminal, _terminal);
					AttachTerminalEventHandlers();    // attach and resume events
					_terminal.ReloadRepositories();

					OpenIO(false);

					OnTimedStatusTextRequest("Terminal settings applied.");
				}
				else
				{
					OnTimedStatusTextRequest("Terminal settings not applied!");
				}
			}
			else
			{
				// terminal is closed, simply set the new settings
				DetachTerminalEventHandlers();        // detach to suspend events
				_settingsRoot.Terminal = settings;
				_terminal = Domain.TerminalFactory.RecreateTerminal(_settingsRoot.Terminal, _terminal);
				AttachTerminalEventHandlers();        // attach an resume events
				_terminal.ReloadRepositories();

				OnTimedStatusTextRequest("Terminal settings applied.");
			}
		}

		/// <summary>
		/// Sets log settings
		/// </summary>
		public void SetLogSettings(Log.Settings.LogSettings settings)
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
			_settingsRoot.Changed += new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		private void DetachSettingsEventHandlers()
		{
			_settingsRoot.Changed -= new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			// nothing to do yet
		}

		#endregion

		#region Settings > Properties
		//------------------------------------------------------------------------------------------
		// Settings > Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool SettingsFileExists
		{
			get
			{
				AssertNotDisposed();
				return (_settingsHandler.SettingsFileExists);
			}
		}

		/// <summary></summary>
		public string SettingsFilePath
		{
			get
			{
				AssertNotDisposed();
				return (_settingsHandler.SettingsFilePath);
			}
		}

		/// <summary></summary>
		public TerminalSettingsRoot SettingsRoot
		{
			get
			{
				AssertNotDisposed();
				return (_settingsRoot);
			}
		}

		/// <summary></summary>
		public WindowSettings WindowSettings
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
		/// </summary>
		private bool TryAutoSave()
		{
			bool success = false;
			if (!_settingsHandler.SettingsFileExists ||
				(_settingsHandler.SettingsFileExists && _settingsRoot.AutoSaved))
			{
				success = SaveToFile(true);
			}
			return (success);
		}

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet
		/// </summary>
		public bool Save()
		{
			return (Save(true));
		}

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet
		/// </summary>
		public bool Save(bool autoSaveIsAllowed)
		{
			AssertNotDisposed();

			bool success = false;

			// save terminal if file path is valid
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
			else // auto save creates default file path
			{
				if (autoSaveIsAllowed)
					success = SaveToFile(true);
			}

			// if not successful yet, request new file path
			if (!success)
				success = (OnSaveAsFileDialogRequest() == DialogResult.OK);

			return (success);
		}

		/// <summary>
		/// Saves terminal to given file
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

		private bool SaveToFile(bool doAutoSave)
		{
			return (SaveToFile(doAutoSave, ""));
		}

		private bool SaveToFile(bool doAutoSave, string autoSaveFilePathToDelete)
		{
			// -------------------------------------------------------------------------------------
			// skip save if file is up to date and there were no changes
			// -------------------------------------------------------------------------------------

			if (_settingsHandler.SettingsFileIsUpToDate && (!_settingsRoot.HaveChanged))
			{
				// event must be fired anyway to ensure that dependent objects are updated
				OnSaved(new SavedEventArgs(_settingsHandler.SettingsFilePath, doAutoSave));
				return (true);
			}

			// -------------------------------------------------------------------------------------
			// save terminal
			// -------------------------------------------------------------------------------------

			bool success = false;

			if (!doAutoSave)
				OnFixedStatusTextRequest("Saving terminal...");

			if (doAutoSave && (!_settingsHandler.SettingsFilePathIsValid))
			{
				string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveTerminalFileNamePrefix + Guid.ToString() + ExtensionSettings.TerminalFiles;
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
				// try to delete existing auto save file
				// ---------------------------------------------------------------------------------

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

		/// <summary>Closes the terminal and prompts if the settings have changed.</summary>
		public bool Close()
		{
			return (Close(false));
		}

		/// <summary>
		/// Closes the terminal and tries to auto save if desired.
		/// </summary>
		/// <remarks>
		/// Attention:
		/// This method is needed for MDI applications. In case of MDI parent/application closing,
		/// Close() of the terminal is called before Close() of the workspace. Without taking care
		/// of this, the workspace would be saved after the terminal has already been close, i.e.
		/// removed from the workspace. Therefore, the terminal has to signal such cases to the
		/// workspace.
		/// </remarks>
		public bool Close(bool isWorkspaceClose)
		{
			bool success = false;

			OnFixedStatusTextRequest("Closing terminal...");

			// try to auto save or delete if never saved yet or changed...
			if (isWorkspaceClose)
			{
				if (!_settingsHandler.SettingsFileExists || _settingsRoot.HaveChanged)
					success = TryAutoSave();
			}
			else
			{
				if (_settingsHandler.SettingsFileExists && _settingsRoot.AutoSaved)
					success = _settingsHandler.Delete();
			}

			// ...or save it manually if necessary
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
			else
			{
				success = true; // consider it successful if there was nothing to save
			}

			// next, close underlying terminal
			if (_terminal.IsOpen)
				success = CloseIO(false);

			// last, close log
			if (_log.IsOpen)
				EndLog();

			if (success)
			{
				// status text request must be before closed event, closed event may close the view
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
			ApplicationSettings.SaveLocalUser();
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
			_terminal.Changed        += new EventHandler(_terminal_Changed);
			_terminal.ControlChanged += new EventHandler(_terminal_ControlChanged);
			_terminal.Error          += new EventHandler<Domain.ErrorEventArgs>(_terminal_Error);

			_terminal.RawElementSent          += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
			_terminal.RawElementReceived      += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
			_terminal.DisplayElementsSent     += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent        += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived    += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
			_terminal.RepositoryCleared       += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded      += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
		}

		private void DetachTerminalEventHandlers()
		{
			_terminal.Changed        -= new EventHandler(_terminal_Changed);
			_terminal.ControlChanged -= new EventHandler(_terminal_ControlChanged);
			_terminal.Error          -= new EventHandler<Domain.ErrorEventArgs>(_terminal_Error);

			_terminal.RawElementSent          -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
			_terminal.RawElementReceived      -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
			_terminal.DisplayElementsSent     -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent        -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived    -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
			_terminal.RepositoryCleared       -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded      -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Local field to maintain connection state in order to be able to detect
		/// a change of the connection state.
		/// </summary>
		private bool _terminal_Changed_isConnected;

		private void _terminal_Changed(object sender, EventArgs e)
		{
			OnIOChanged(e);

			if      (_terminal.IsConnected && !_terminal_Changed_isConnected)
				_ioConnectChrono.Start();
			else if (!_terminal.IsConnected && _terminal_Changed_isConnected)
				_ioConnectChrono.Stop();

			_terminal_Changed_isConnected = _terminal.IsConnected;
		}

		private void _terminal_ControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void _terminal_Error(object sender, Domain.ErrorEventArgs e)
		{
			OnIOError(e);
		}

		private void _terminal_RawElementSent(object sender, Domain.RawElementEventArgs e)
		{
			// count
			_txByteCount += e.Element.Data.Length;
			OnIOCountChanged(new EventArgs());

			// log
			if (_log.IsOpen)
			{
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawTx);
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
			}
		}

		private void _terminal_RawElementReceived(object sender, Domain.RawElementEventArgs e)
		{
			// count
			_rxByteCount += e.Element.Data.Length;
			OnIOCountChanged(new EventArgs());

			// log
			if (_log.IsOpen)
			{
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawRx);
			}
		}

		private void _terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			// display
			OnDisplayElementsSent(e);

			// log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (_log.IsOpen)
				{
					if (de.IsEol)
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
			// display
			OnDisplayElementsReceived(e);

			// log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (_log.IsOpen)
				{
					if (de.IsEol)
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
			// count
			_txLineCount += e.Lines.Count;
			OnIOCountChanged(new EventArgs());

			// display
			OnDisplayLinesSent(e);
		}

		private void _terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			// count
			_rxLineCount += e.Lines.Count;
			OnIOCountChanged(new EventArgs());

			// display
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

		#region Terminal > Open/Close
		//------------------------------------------------------------------------------------------
		// Terminal > Open/Close
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Opens the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public bool OpenIO()
		{
			return (OpenIO(true));
		}

		/// <summary>
		/// Opens the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		private bool OpenIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Opening terminal...");
			try
			{
				_terminal.Open();

				if (saveStatus)
					_settingsRoot.TerminalIsOpen = _terminal.IsOpen;

				OnTimedStatusTextRequest("Terminal opened");
				success = true;
			}
			catch (Exception ex)
			{
				OnFixedStatusTextRequest("Error opening terminal!");

				string ioText;
				if (_settingsRoot.IOType == MKY.IO.Serial.IOType.SerialPort)
					ioText = "Port";
				else
					ioText = "Socket";

				OnMessageInputRequest
					(
					"Unable to open terminal:" + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					ioText + " could be in use by another process.",
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				OnTimedStatusTextRequest("Terminal not opened!");
			}

			return (success);
		}

		/// <summary>
		/// Closes the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public bool CloseIO()
		{
			return (CloseIO(true));
		}

		/// <summary>
		/// Closes the terminal's I/O instance
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		private bool CloseIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Closing terminal...");
			try
			{
				_terminal.Close();

				if (saveStatus)
					_settingsRoot.TerminalIsOpen = _terminal.IsOpen;

				OnTimedStatusTextRequest("Terminal closed");
				success = true;
			}
			catch (Exception ex)
			{
				OnTimedStatusTextRequest("Error closing terminal!");

				OnMessageInputRequest
					(
					"Unable to close terminal:" + Environment.NewLine + Environment.NewLine + ex.Message,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				OnTimedStatusTextRequest("Terminal not closed!");
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
		public void RequestToggleRts()
		{
			if (_settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.FlowControl.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleRts();
				_settingsRoot.Terminal.IO.SerialPort.RtsEnabled = port.RtsEnable;
			}
		}

		/// <summary>
		/// Toggles DTR line if current flow control settings allow this
		/// </summary>
		public void RequestToggleDtr()
		{
			if (_settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.FlowControl.Manual)
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
					case MKY.IO.Serial.IOType.SerialPort: text += "port"; title = "Serial Port"; break;
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
					case MKY.IO.Serial.IOType.SerialPort: text += "port"; title = "Serial Port"; break;
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
		public void SendCommand()
		{
			SendCommand(_settingsRoot.SendCommand.Command);
			_settingsRoot.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary
				(
				new RecentItem<Command>(new Command(_settingsRoot.SendCommand.Command))
				);

			// clear command if desired
			if (!_settingsRoot.Send.KeepCommand)
				_settingsRoot.SendCommand.Command = new Command(); // set command to ""
		}

		/// <summary>
		/// Sends given command.
		/// </summary>
		/// <param name="command">Command to be sent.</param>
		public void SendCommand(Command command)
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
		public void SendFile()
		{
			SendFile(_settingsRoot.SendFile.Command);
		}

		/// <summary>
		/// Sends given file
		/// </summary>
		/// <param name="command">File to be sent</param>
		public void SendFile(Command command)
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
		public void SendPredefined(int page, int command)
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
		public void ReloadRepositories()
		{
			AssertNotDisposed();
			_terminal.ReloadRepositories();
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public List<Domain.DisplayElement> RepositoryToDisplayElements(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (_terminal.RepositoryToDisplayElements(repositoryType));
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public List<List<Domain.DisplayElement>> RepositoryToDisplayLines(Domain.RepositoryType repositoryType)
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
		public string RepositoryToString(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (_terminal.RepositoryToString(repositoryType));
		}

		/// <summary>
		/// Clears given repository.
		/// </summary>
		public void ClearRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			_terminal.ClearRepository(repositoryType);
		}

		/// <summary>
		/// Clears all repositories.
		/// </summary>
		public void ClearRepositories()
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
		public TimeSpan IOConnectTime
		{
			get { return (_ioConnectChrono.TimeSpan); }
		}

		/// <summary></summary>
		public void RestartIOConnectTime()
		{
			_ioConnectChrono.Restart();
		}

		private void _ioConnectChrono_TimeSpanChanged(object sender, EventArgs e)
		{
			OnIOConnectTimeChanged(new EventArgs());
		}

		#endregion

		#region Terminal > Count Status
		//------------------------------------------------------------------------------------------
		// Terminal > Count Status
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public int TxByteCount
		{
			get { return (_txByteCount); }
		}

		/// <summary></summary>
		public int TxLineCount
		{
			get { return (_txLineCount); }
		}

		/// <summary></summary>
		public int RxByteCount
		{
			get { return (_rxByteCount); }
		}

		/// <summary></summary>
		public int RxLineCount
		{
			get { return (_rxLineCount); }
		}

		/// <summary></summary>
		public void ResetIOCount()
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
		public void BeginLog()
		{
			try
			{
				// reapply settings NOW, makes sure date/time in filenames is refreshed
				_log.Settings = _settingsRoot.Log;
				_log.Begin();
				_settingsRoot.LogIsOpen = true;
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
		public void ClearLog()
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
		public void EndLog()
		{
			EndLog(true);
		}

		/// <summary></summary>
		public void EndLog(bool saveStatus)
		{
			try
			{
				_log.End();

				if (saveStatus)
					_settingsRoot.LogIsOpen = false;
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
		protected virtual void OnIOConnectTimeChanged(EventArgs e)
		{
			EventHelper.FireSync(IOConnectTimeChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOCountChanged(EventArgs e)
		{
			EventHelper.FireSync(IOCountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(Domain.ErrorEventArgs e)
		{
			EventHelper.FireSync<Domain.ErrorEventArgs>(IOError, this, e);
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
