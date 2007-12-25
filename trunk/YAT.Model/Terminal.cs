using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.Guid;
using MKY.Utilities.Event;
using MKY.Utilities.Recent;
using MKY.Utilities.Settings;

using YAT.Domain;

using YAT.Settings;
using YAT.Settings.Terminal;

using YAT.Model.Types;
using YAT.Model.Settings;
using YAT.Model.Utilities;

namespace YAT.Model
{
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

		private static int _terminalIdCounter = 0;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed = false;

		private Guid _guid;
		private string _userName;

		// settings
		private DocumentSettingsHandler<TerminalSettingsRoot> _settingsHandler;
		private YAT.Settings.Terminal.TerminalSettingsRoot _settingsRoot;

		// terminal
		private Domain.Terminal _terminal;

		// logs
		private Log.Logs _log;

		// count status
		private int _txByteCount = 0;
		private int _rxByteCount = 0;
		private int _txLineCount = 0;
		private int _rxLineCount = 0;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		public event EventHandler<SavedEventArgs> Saved;
		public event EventHandler Closed;

		public event EventHandler Changed;
		public event EventHandler ControlChanged;
		public event EventHandler<Domain.ErrorEventArgs> Error;

		/// <summary></summary>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsSent;
		/// <summary></summary>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsReceived;
		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesSent;
		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesReceived;
		/// <summary></summary>
		public event EventHandler<RepositoryEventArgs> RepositoryCleared;
		/// <summary></summary>
		public event EventHandler<RepositoryEventArgs> RepositoryReloaded;

		public event EventHandler CountChanged;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		public event EventHandler SaveAsFileDialogRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Terminal()
		{
			Initialize(new DocumentSettingsHandler<TerminalSettingsRoot>(), Guid.NewGuid());
		}

		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			Initialize(settingsHandler, Guid.NewGuid());
		}

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

			_settingsHandler = settingsHandler;
			_settingsRoot = _settingsHandler.Settings;
			AttachSettingsHandlers();

			if (!_settingsHandler.SettingsFilePathIsValid || _settingsRoot.AutoSaved)
				_userName = _TerminalText + _terminalIdCounter.ToString();
			else
				UserNameFromFile = _settingsHandler.SettingsFilePath;

			_terminal = Domain.Factory.TerminalFactory.CreateTerminal(_settingsRoot.Terminal);
			AttachTerminalHandlers();

			_log = new Log.Logs(_settingsRoot.Log);
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

		public Guid Guid
		{
			get { return (_guid); }
		}

		public string UserName
		{
			get { return (_userName); }
		}

		private string UserNameFromFile
		{
			set { _userName = Path.GetFileName(value); }
		}

		public bool IsOpen
		{
			get { return (_terminal.IsOpen); }
		}

		#endregion

		#region Save
		//==========================================================================================
		// Save
		//==========================================================================================

		public void AutoSave()
		{
			// only perform auto save if no file yet or on previously auto saved files
			if (!_settingsHandler.SettingsFileExists ||
				(_settingsHandler.SettingsFileExists && _settingsRoot.AutoSaved))
				Save(true);
		}

		private void Save()
		{
			Save(false);
		}

		private void Save(bool autoSave)
		{
			if (autoSave)
			{
				SaveToFile(true);
			}
			else
			{
				if (_settingsHandler.SettingsFilePathIsValid && !_settingsHandler.Settings.AutoSaved)
					SaveToFile(false);
				else
					OnSaveAsFileDialogRequest(new EventArgs());
			}
		}

		private void SaveToFile(bool autoSave)
		{
			SaveToFile(autoSave, "");
		}

		private void SaveToFile(bool autoSave, string autoSaveFilePathToDelete)
		{
			if (!autoSave)
				OnFixedStatusTextRequest("Saving terminal...");

			try
			{
				if (autoSave)
				{
					string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveTerminalFileNamePrefix + Guid.ToString() + ExtensionSettings.TerminalFiles;
					if (!_settingsHandler.SettingsFilePathIsValid)
						_settingsHandler.SettingsFilePath = autoSaveFilePath;
				}
				_settingsHandler.Settings.AutoSaved = autoSave;
				_settingsHandler.Save();

				if (!autoSave)
					UserNameFromFile = _settingsHandler.SettingsFilePath;

				OnSaved(new SavedEventArgs(_settingsHandler.SettingsFilePath, autoSave));
				OnChanged(new EventArgs());

				if (!autoSave)
					OnTimedStatusTextRequest("Terminal saved");

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
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		public string SettingsFilePath
		{
			get
			{
				if (_settingsHandler.SettingsFileExists)
					return (_settingsHandler.SettingsFilePath);
				else
					return ("");
			}
		}

		public WindowSettings WindowSettings
		{
			get { return (_settingsRoot.Window); }
		}

		private void AttachSettingsHandlers()
		{
			_settingsRoot.ClearChanged();
			_settingsRoot.Changed += new EventHandler<SettingsEventArgs>(_settings_Changed);
		}

		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void _settings_Changed(object sender, SettingsEventArgs e)
		{
			// nothing to do yet
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

		private void AttachTerminalHandlers()
		{
			_terminal.Changed += new EventHandler(_terminal_Changed);
			_terminal.ControlChanged += new EventHandler(_terminal_ControlChanged);
			_terminal.Error += new EventHandler<Domain.ErrorEventArgs>(_terminal_Error);

			_terminal.RawElementSent += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
			_terminal.RawElementReceived += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
			_terminal.DisplayElementsSent += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
			_terminal.RepositoryCleared += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
		}

		private void DetachTerminalHandlers()
		{
			_terminal.Changed -= new EventHandler(_terminal_Changed);
			_terminal.ControlChanged -= new EventHandler(_terminal_ControlChanged);
			_terminal.Error -= new EventHandler<Domain.ErrorEventArgs>(_terminal_Error);

			_terminal.RawElementSent -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
			_terminal.RawElementReceived -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
			_terminal.DisplayElementsSent -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
			_terminal.RepositoryCleared -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
		}

		#endregion

		#region Terminal > Open/Close
		//------------------------------------------------------------------------------------------
		// Terminal > Open/Close
		//------------------------------------------------------------------------------------------

		private bool OpenTerminal()
		{
			return (OpenTerminal(true));
		}

		private bool OpenTerminal(bool saveStatus)
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
				if (_settingsRoot.IOType == Domain.IOType.SerialPort)
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

		private bool CloseTerminal()
		{
			return (CloseTerminal(true));
		}

		private bool CloseTerminal(bool saveStatus)
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

		private void RequestToggleRts()
		{
			if (_settingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleRts();
				_settingsRoot.Terminal.IO.SerialPort.RtsEnabled = port.RtsEnable;
			}
		}

		private void RequestToggleDtr()
		{
			if (_settingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
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
			OnFixedStatusTextRequest("Sending \"" + s + "\"...");
			try
			{
				_terminal.SendLine(s);
				OnTimedStatusTextRequest("\"" + s + "\" sent");
			}
			catch (System.IO.IOException ex)
			{
				OnFixedStatusTextRequest("Error sending \"" + s + "\"!");

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
				OnFixedStatusTextRequest("Error sending \"" + s + "\"!");
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

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _terminal_Changed(object sender, EventArgs e)
		{
			OnChanged(e);
		}

		private void _terminal_ControlChanged(object sender, EventArgs e)
		{
			OnControlChanged(e);
		}

		private void _terminal_Error(object sender, Domain.ErrorEventArgs e)
		{
			OnChanged(new EventArgs());
			OnError(e);
		}

		private void _terminal_RawElementSent(object sender, Domain.RawElementEventArgs e)
		{
			// count
			_txByteCount += e.Element.Data.Length;
			OnCountChanged(new EventArgs());

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
			OnCountChanged(new EventArgs());

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
			OnCountChanged(new EventArgs());

			// display
			OnDisplayLinesSent(e);
		}

		private void _terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			// count
			_rxLineCount += e.Lines.Count;
			OnCountChanged(new EventArgs());

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

		#endregion

		#region Send
		//==========================================================================================
		// Send
		//==========================================================================================

		#region Send > Command
		//------------------------------------------------------------------------------------------
		// Send > Command
		//------------------------------------------------------------------------------------------

		private void SendCommand()
		{
			SendCommand(_settingsRoot.SendCommand.Command);
			_settingsRoot.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary
				(
				new RecentItem<Command>(new Command(_settingsRoot.SendCommand.Command))
				);
		}

		private void SendCommand(Command command)
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

		#region Send > File
		//------------------------------------------------------------------------------------------
		// Send > File
		//------------------------------------------------------------------------------------------

		private void SendFile()
		{
			SendFile(_settingsRoot.SendFile.Command);
		}

		private void SendFile(Command command)
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

		#endregion

		#region Log
		//==========================================================================================
		// Log
		//==========================================================================================

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

		public void EndLog()
		{
			EndLog(true);
		}

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

		#region Count Status
		//==========================================================================================
		// Count Status
		//==========================================================================================

		public int TxByteCount
		{
			get { return (_txByteCount); }
		}

		public int TxLineCount
		{
			get { return (_txLineCount); }
		}

		public int RxByteCount
		{
			get { return (_rxByteCount); }
		}

		public int RxLineCount
		{
			get { return (_rxLineCount); }
		}

		public void ResetCount()
		{
			_txByteCount = 0;
			_txLineCount = 0;
			_rxByteCount = 0;
			_rxLineCount = 0;

			OnCountChanged(new EventArgs());
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnSaved(SavedEventArgs e)
		{
			EventHelper.FireSync<SavedEventArgs>(Saved, this, e);
		}

		protected virtual void OnClosed(EventArgs e)
		{
			EventHelper.FireSync(Closed, this, e);
		}

		protected virtual void OnChanged(EventArgs e)
		{
			EventHelper.FireSync(Changed, this, e);
		}

		protected virtual void OnControlChanged(EventArgs e)
		{
			EventHelper.FireSync(ControlChanged, this, e);
		}

		protected virtual void OnError(Domain.ErrorEventArgs e)
		{
			EventHelper.FireSync(Error, this, e);
		}

		protected virtual void OnCountChanged(EventArgs e)
		{
			EventHelper.FireSync(CountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(DisplayElementsEventArgs e)
		{
			EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(DisplayElementsEventArgs e)
		{
			EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesSent(DisplayLinesEventArgs e)
		{
			EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(DisplayLinesEventArgs e)
		{
			EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(RepositoryEventArgs e)
		{
			EventHelper.FireSync<RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(RepositoryEventArgs e)
		{
			EventHelper.FireSync<RepositoryEventArgs>(RepositoryReloaded, this, e);
		}

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
		protected virtual void OnSaveAsFileDialogRequest(EventArgs e)
		{
			EventHelper.FireSync(SaveAsFileDialogRequest, this, e);
		}

		#endregion
	}
}
