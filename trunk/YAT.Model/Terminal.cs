using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.Event;
using MKY.Utilities.Settings;

using YAT.Model.Types;
using YAT.Settings;

namespace YAT.Model
{
	public class Terminal : IDisposable
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

		private Guid _guid = Guid.NewGuid();
		private string _userName;

		// settings
		private DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> _terminalSettingsHandler;
		private YAT.Settings.Terminal.TerminalSettingsRoot _terminalSettingsRoot;
		private bool _handlingTerminalSettingsIsSuspended = false;

		// terminal
		private Domain.Terminal _terminal;

		// logs
		private Log.Logs _log;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		public event EventHandler TerminalChanged;
		public event EventHandler<TerminalSavedEventArgs> TerminalSaved;
		public event EventHandler TerminalClosed;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<StatusTextEventArgs> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		public event EventHandler SaveTerminalAsFileDialogRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>

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

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Guid Guid
		{
			get { return (_guid); }
			set { _guid = value; }
		}

		public string UserName
		{
			get { return (_userName); }
			set { _userName = value; }
		}

		public string UserNameFromFile
		{
			set { _userName = Path.GetFileName(value); }
		}

		public string SettingsFilePath
		{
			get
			{
				if (_terminalSettingsHandler.SettingsFileExists)
					return (_terminalSettingsHandler.SettingsFilePath);
				else
					return ("");
			}
		}

		public bool AutoSaved
		{
			get { return (_terminalSettingsRoot.AutoSaved); }
		}

		public bool IsOpen
		{
			get { return (_terminal.IsOpen); }
		}

		#endregion

		#region File
		//==========================================================================================
		// File
		//==========================================================================================

		private void SaveTerminal()
		{
			SaveTerminal(false);
		}

		private void SaveTerminal(bool autoSave)
		{
			if (autoSave)
			{
				SaveTerminalToFile(true);
			}
			else
			{
				if (_terminalSettingsHandler.SettingsFilePathIsValid && !_terminalSettingsHandler.Settings.AutoSaved)
					SaveTerminalToFile(false);
				else
					OnSaveTerminalAsFileDialogRequest();
			}
		}

		private void SaveTerminalToFile(bool autoSave)
		{
			SaveTerminalToFile(autoSave, "");
		}

		private void SaveTerminalToFile(bool autoSave, string autoSaveFilePathToDelete)
		{
			if (!autoSave)
				OnFixedStatusTextRequest("Saving terminal...");

			try
			{
				if (autoSave)
				{
					string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveTerminalFileNamePrefix + Guid.ToString() + ExtensionSettings.TerminalFiles;
					if (!_terminalSettingsHandler.SettingsFilePathIsValid)
						_terminalSettingsHandler.SettingsFilePath = autoSaveFilePath;
				}
				_terminalSettingsHandler.Settings.AutoSaved = autoSave;
				_terminalSettingsHandler.Save();

				if (!autoSave)
					UserNameFromFile = _terminalSettingsHandler.SettingsFilePath;

				OnTerminalSaved(new TerminalSavedEventArgs(_terminalSettingsHandler.SettingsFilePath, autoSave));

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
						"Unable to save file" + Environment.NewLine + _terminalSettingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					OnTimedStatusTextRequest("Terminal not saved!");
				}
			}
			SetTerminalCaption();

			SelectSendCommandInput();
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
			_terminal.TerminalChanged += new EventHandler(_terminal_TerminalChanged);
			_terminal.TerminalControlChanged += new EventHandler(_terminal_TerminalControlChanged);
			_terminal.TerminalError += new EventHandler<Domain.TerminalErrorEventArgs>(_terminal_TerminalError);

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
			_terminal.TerminalChanged -= new EventHandler(_terminal_TerminalChanged);
			_terminal.TerminalControlChanged -= new EventHandler(_terminal_TerminalControlChanged);
			_terminal.TerminalError -= new EventHandler<Domain.TerminalErrorEventArgs>(_terminal_TerminalError);

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

		#region Terminal > View
		//------------------------------------------------------------------------------------------
		// Terminal > View
		//------------------------------------------------------------------------------------------

		private void InitializeIOControlStatusLabels()
		{
			_statusLabels_ioControl = new List<ToolStripStatusLabel>();
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_RTS);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_CTS);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_DTR);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_DSR);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_DCD);
		}

		private void SetTerminalCaption()
		{
			bool isOpen = false;
			bool isConnected = false;

			if (_terminal != null)
			{
				isOpen = _terminal.IsOpen;
				isConnected = _terminal.IsConnected;
			}

			StringBuilder sb = new StringBuilder(UserName);

			if ((_terminalSettingsRoot != null) && _terminalSettingsRoot.ExplicitHaveChanged)
				sb.Append("*");

			if (_terminalSettingsRoot != null)
			{
				if (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort)
				{
					Domain.Settings.SerialPort.SerialPortSettings s = _terminalSettingsRoot.IO.SerialPort;
					sb.Append(" - ");
					sb.Append(s.PortId.ToString());
					sb.Append(" - ");
					sb.Append(isOpen ? "Open" : "Closed");
				}
				else
				{
					Domain.Settings.Socket.SocketSettings s = _terminalSettingsRoot.IO.Socket;
					switch (_terminalSettingsRoot.IOType)
					{
						case Domain.IOType.TcpClient:
							sb.Append(" - ");
							sb.Append(s.ResolvedRemoteIPAddress.ToString());
							sb.Append(":");
							sb.Append(s.RemotePort.ToString());
							sb.Append(" - ");
							sb.Append(isConnected ? "Connected" : "Disconnected");
							break;

						case Domain.IOType.TcpServer:
							sb.Append(" - ");
							sb.Append("Server:");
							sb.Append(s.LocalPort.ToString());
							sb.Append(" - ");
							if (isOpen)
								sb.Append(isConnected ? "Connected" : "Listening");
							else
								sb.Append("Closed");
							break;

						case Domain.IOType.TcpAutoSocket:
							bool isClient = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsClient;
							bool isServer = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsServer;
							if (isOpen)
							{
								if (isClient)
								{
									sb.Append(" - ");
									sb.Append(s.ResolvedRemoteIPAddress.ToString());
									sb.Append(":");
									sb.Append(s.RemotePort.ToString());
									sb.Append(" - ");
									sb.Append(isConnected ? "Connected" : "Disconnected");
								}
								else if (isServer)
								{
									sb.Append(" - ");
									sb.Append("Server:");
									sb.Append(s.LocalPort.ToString());
									sb.Append(" - ");
									sb.Append(isConnected ? "Connected" : "Listening");
								}
								else
								{
									sb.Append(" - ");
									sb.Append("Starting on port ");
									sb.Append(s.RemotePort.ToString());
								}
							}
							else
							{
								sb.Append(" - ");
								sb.Append("AutoSocket:");
								sb.Append(s.RemotePort.ToString());
								sb.Append(" - ");
								sb.Append("Disconnected");
							}
							break;

						case Domain.IOType.Udp:
							sb.Append(" - ");
							sb.Append(s.ResolvedRemoteIPAddress.ToString());
							sb.Append(":");
							sb.Append(s.RemotePort.ToString());
							sb.Append(" - ");
							sb.Append("Receive:");
							sb.Append(s.LocalPort.ToString());
							sb.Append(" - ");
							sb.Append(isOpen ? "Open" : "Closed");
							break;
					}
				}
			}

			Text = sb.ToString();
		}

		private void SetTerminalControls()
		{
			bool isOpen = _terminal.IsOpen;

			// main menu
			toolStripMenuItem_TerminalMenu_Terminal_Open.Enabled = !isOpen;
			toolStripMenuItem_TerminalMenu_Terminal_Close.Enabled = isOpen;

			// terminal panel
			SetTerminalCaption();
			SetIOStatus();
			SetIOControlControls();

			// send panel
			send.TerminalIsOpen = isOpen;

			// predefined panel
			predefined.TerminalIsOpen = isOpen;
		}

		private void SetIOStatus()
		{
			bool isOpen = _terminal.IsOpen;
			bool isConnected = _terminal.IsConnected;

			StringBuilder sb = new StringBuilder();

			if (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort)
			{
				Domain.Settings.SerialPort.SerialPortSettings s = _terminalSettingsRoot.IO.SerialPort;
				sb.Append("Serial port ");
				sb.Append(s.PortId.ToString());
				sb.Append(" (" + s.Communication.ToString() + ") is ");
				sb.Append(isOpen ? "open" : "closed");

				toolStripStatusLabel_TerminalStatus_Connection.Visible = false;
			}
			else
			{
				Domain.Settings.Socket.SocketSettings s = _terminalSettingsRoot.IO.Socket;
				switch (_terminalSettingsRoot.IOType)
				{
					case Domain.IOType.TcpClient:
						sb.Append("TCP client is ");
						sb.Append(isConnected ? "connected to " : "disconnected from ");
						sb.Append(s.ResolvedRemoteIPAddress.ToString());
						sb.Append(" on remote port ");
						sb.Append(s.RemotePort.ToString());
						break;

					case Domain.IOType.TcpServer:
						sb.Append("TCP server is ");
						if (isOpen)
						{
							if (isConnected)
							{
								Domain.IO.TcpServer server = (Domain.IO.TcpServer)_terminal.UnderlyingIOProvider;
								int count = server.ConnectedClientCount;

								sb.Append("connected to ");
								sb.Append(count.ToString());
								if (count == 1)
									sb.Append(" client");
								else
									sb.Append(" clients");
							}
							else
							{
								sb.Append("listening");
							}
						}
						else
						{
							sb.Append("closed");
						}
						sb.Append(" on local port ");
						sb.Append(s.LocalPort.ToString());
						break;

					case Domain.IOType.TcpAutoSocket:
						bool isClient = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsClient;
						bool isServer = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsServer;
						sb.Append("TCP auto socket is ");
						if (isOpen)
						{
							if (isClient)
							{
								sb.Append("connected to ");
								sb.Append(s.ResolvedRemoteIPAddress.ToString());
								sb.Append(" on remote port ");
								sb.Append(s.RemotePort.ToString());
							}
							else if (isServer)
							{
								sb.Append(isConnected ? "connected" : "listening");
								sb.Append(" on local port ");
								sb.Append(s.LocalPort.ToString());
							}
							else
							{
								sb.Append("starting on port ");
								sb.Append(s.RemotePort.ToString());
							}
						}
						else
						{
							sb.Append("closed on port ");
							sb.Append(s.RemotePort.ToString());
						}
						break;

					case Domain.IOType.Udp:
						sb.Append("UDP socket is ");
						sb.Append(isOpen ? "open" : "closed");
						sb.Append(" for sending to ");
						sb.Append(s.ResolvedRemoteIPAddress.ToString());
						sb.Append(" on remote port ");
						sb.Append(s.RemotePort.ToString());
						sb.Append(" and receiving on local port ");
						sb.Append(s.LocalPort.ToString());
						break;
				}

				Image on = Properties.Resources.Image_On_12x12;
				Image off = Properties.Resources.Image_Off_12x12;

				toolStripStatusLabel_TerminalStatus_Connection.Visible = true;
				toolStripStatusLabel_TerminalStatus_Connection.Image = (isConnected ? on : off);
			}

			toolStripStatusLabel_TerminalStatus_IOStatus.Text = sb.ToString();
		}

		private void SetIOControlControls()
		{
			bool isOpen = _terminal.IsOpen;
			bool isSerialPort = (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort);

			foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
				sl.Visible = isSerialPort;

			if (isSerialPort)
			{
				foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
					sl.Enabled = isOpen;

				Image on = Properties.Resources.Image_On_12x12;
				Image off = Properties.Resources.Image_Off_12x12;

				if (isOpen)
				{
					MKY.IO.Ports.SerialPortControlPins pins;
					pins = ((MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance).ControlPins;

					bool rs485Handshake = (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.RS485);

					if (rs485Handshake)
					{
						if (pins.Rts)
							TriggerRtsLuminescence();
					}
					else
					{
						toolStripStatusLabel_TerminalStatus_RTS.Image = (pins.Rts ? on : off);
					}

					toolStripStatusLabel_TerminalStatus_CTS.Image = (pins.Cts ? on : off);
					toolStripStatusLabel_TerminalStatus_DTR.Image = (pins.Dtr ? on : off);
					toolStripStatusLabel_TerminalStatus_DSR.Image = (pins.Dsr ? on : off);
					toolStripStatusLabel_TerminalStatus_DCD.Image = (pins.Cd ? on : off);

					bool manualHandshake = (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual);

					toolStripStatusLabel_TerminalStatus_RTS.ForeColor = (manualHandshake ? SystemColors.ControlText : SystemColors.GrayText);
					toolStripStatusLabel_TerminalStatus_CTS.ForeColor = SystemColors.GrayText;
					toolStripStatusLabel_TerminalStatus_DTR.ForeColor = (manualHandshake ? SystemColors.ControlText : SystemColors.GrayText);
					toolStripStatusLabel_TerminalStatus_DSR.ForeColor = SystemColors.GrayText;
					toolStripStatusLabel_TerminalStatus_DCD.ForeColor = SystemColors.GrayText;
				}
				else
				{
					foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
						sl.Image = off;

					foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
						sl.ForeColor = SystemColors.ControlText;
				}
			}
		}

		private void TriggerRtsLuminescence()
		{
			timer_RtsLuminescence.Enabled = false;
			toolStripStatusLabel_TerminalStatus_RTS.Image = Properties.Resources.Image_On_12x12;
			timer_RtsLuminescence.Interval = _RtsLuminescenceInterval;
			timer_RtsLuminescence.Enabled = true;
		}

		private void ResetRts()
		{
			Image on = Properties.Resources.Image_On_12x12;
			Image off = Properties.Resources.Image_Off_12x12;

			if (_terminal.IsOpen)
			{
				MKY.IO.Ports.SerialPortControlPins pins;
				pins = ((MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance).ControlPins;

				toolStripStatusLabel_TerminalStatus_RTS.Image = (pins.Rts ? on : off);
			}
			else
			{
				toolStripStatusLabel_TerminalStatus_RTS.Image = off;
			}
		}

		private void timer_RtsLuminescence_Tick(object sender, EventArgs e)
		{
			timer_RtsLuminescence.Enabled = false;
			ResetRts();
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

			Cursor = Cursors.WaitCursor;
			OnFixedStatusTextRequest("Opening terminal...");
			Refresh();
			try
			{
				_terminal.Open();

				if (saveStatus)
					_terminalSettingsRoot.TerminalIsOpen = _terminal.IsOpen;

				OnTimedStatusTextRequest("Terminal opened");
				success = true;
			}
			catch (Exception ex)
			{
				OnFixedStatusTextRequest("Error opening terminal!");

				string ioText;
				if (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort)
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
				success = false;
			}
			Cursor = Cursors.Default;

			SelectSendCommandInput();

			return (success);
		}

		private bool CloseTerminal()
		{
			return (CloseTerminal(true));
		}

		private bool CloseTerminal(bool saveStatus)
		{
			bool success = false;

			Cursor = Cursors.WaitCursor;
			OnFixedStatusTextRequest("Closing terminal...");
			Refresh();
			try
			{
				_terminal.Close();

				if (saveStatus)
					_terminalSettingsRoot.TerminalIsOpen = _terminal.IsOpen;

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
				success = false;
			}
			Cursor = Cursors.Default;

			return (success);
		}

		#endregion

		#region Terminal > IO Control
		//------------------------------------------------------------------------------------------
		// Terminal > IO Control
		//------------------------------------------------------------------------------------------

		private void RequestToggleRts()
		{
			if (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleRts();
				_terminalSettingsRoot.Terminal.IO.SerialPort.RtsEnabled = port.RtsEnable;
			}
		}

		private void RequestToggleDtr()
		{
			if (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleDtr();
				_terminalSettingsRoot.Terminal.IO.SerialPort.DtrEnabled = port.DtrEnable;
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
				switch (_terminalSettingsRoot.IOType)
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
				switch (_terminalSettingsRoot.IOType)
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
				ResetStatusText();
				OnMessageInputRequest
					(
					"Bad data format:" + Environment.NewLine + Environment.NewLine + ex.Message,
					"Format Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
			}
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _terminal_TerminalChanged(object sender, EventArgs e)
		{
			SetTerminalControls();
			OnTerminalChanged(new EventArgs());
		}

		private void _terminal_TerminalControlChanged(object sender, EventArgs e)
		{
			SetIOControlControls();
		}

		private void _terminal_TerminalError(object sender, Domain.TerminalErrorEventArgs e)
		{
			SetTerminalControls();
			OnTerminalChanged(new EventArgs());

			MessageBox.Show
				(
				this,
				"Terminal error:" + Environment.NewLine + Environment.NewLine + e.Message,
				"Terminal Error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
				);
		}

		private void _terminal_RawElementSent(object sender, Domain.RawElementEventArgs e)
		{
			// counter
			int byteCount = e.Element.Data.Length;
			monitor_Tx.TxByteCountStatus += byteCount;
			monitor_Bidir.TxByteCountStatus += byteCount;

			// log
			if (_log.IsOpen)
			{
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawTx);
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
			}
		}

		private void _terminal_RawElementReceived(object sender, Domain.RawElementEventArgs e)
		{
			// counter
			int byteCount = e.Element.Data.Length;
			monitor_Bidir.RxByteCountStatus += byteCount;
			monitor_Rx.RxByteCountStatus += byteCount;

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
			monitor_Tx.AddElements(e.Elements);
			monitor_Bidir.AddElements(e.Elements);

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
			monitor_Bidir.AddElements(e.Elements);
			monitor_Rx.AddElements(e.Elements);

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
			if (e.Lines.Count > 0)
			{
				monitor_Tx.ReplaceLastLine(e.Lines[0]);
				monitor_Bidir.ReplaceLastLine(e.Lines[0]);
			}
			for (int i = 1; i < e.Lines.Count; i++)
			{
				monitor_Tx.AddLine(e.Lines[i]);
				monitor_Bidir.AddLine(e.Lines[i]);
			}

			monitor_Tx.TxLineCountStatus += e.Lines.Count;
			monitor_Bidir.TxLineCountStatus += e.Lines.Count;
		}

		private void _terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (e.Lines.Count > 0)
			{
				monitor_Bidir.ReplaceLastLine(e.Lines[0]);
				monitor_Rx.ReplaceLastLine(e.Lines[0]);
			}
			for (int i = 1; i < e.Lines.Count; i++)
			{
				monitor_Bidir.AddLine(e.Lines[i]);
				monitor_Rx.AddLine(e.Lines[i]);
			}

			monitor_Bidir.RxLineCountStatus += e.Lines.Count;
			monitor_Rx.RxLineCountStatus += e.Lines.Count;
		}

		private void _terminal_RepositoryCleared(object sender, Domain.RepositoryEventArgs e)
		{
			switch (e.Repository)
			{
				case Domain.RepositoryType.Tx: monitor_Tx.Clear(); break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.Clear(); break;
				case Domain.RepositoryType.Rx: monitor_Rx.Clear(); break;
			}
		}

		private void _terminal_RepositoryReloaded(object sender, Domain.RepositoryEventArgs e)
		{
			switch (e.Repository)
			{
				case Domain.RepositoryType.Tx: monitor_Tx.AddLines(_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Tx)); break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.AddLines(_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Bidir)); break;
				case Domain.RepositoryType.Rx: monitor_Rx.AddLines(_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Rx)); break;
			}
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

		private void SelectSendCommandInput()
		{
			send.SelectSendCommandInput();
		}

		private void SendCommand()
		{
			SendCommand(_terminalSettingsRoot.SendCommand.Command);
			_terminalSettingsRoot.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary
				(
				new RecentItem<Command>(new Command(_terminalSettingsRoot.SendCommand.Command))
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
			SendFile(_terminalSettingsRoot.SendFile.Command);
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
				_log.Settings = _terminalSettingsRoot.Log;
				_log.Begin();
				_terminalSettingsRoot.LogIsOpen = true;
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
					_terminalSettingsRoot.LogIsOpen = false;
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

		protected virtual void OnTerminalChanged(EventArgs e)
		{
			EventHelper.FireSync(TerminalChanged, this, e);
		}

		protected virtual void OnTerminalSaved(TerminalSavedEventArgs e)
		{
			EventHelper.FireSync<TerminalSavedEventArgs>(TerminalSaved, this, e);
		}

		protected virtual void OnTerminalClosed(EventArgs e)
		{
			EventHelper.FireSync(TerminalClosed, this, e);
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
		protected virtual void OnSaveTerminalAsFileDialogRequest(EventArgs e)
		{
			EventHelper.FireSync(SaveTerminalAsFileDialogRequest, this, e);
		}

		#endregion
	}
}
