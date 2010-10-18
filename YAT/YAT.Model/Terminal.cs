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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Windows.Forms;

using MKY.Event;
using MKY.Guid;
using MKY.Recent;
using MKY.Settings;
using MKY.Time;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;

using YAT.Model.Types;
using YAT.Model.Settings;
using YAT.Model.Utilities;

namespace YAT.Model
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Terminal : IDisposable, IGuidProvider
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string TerminalText = "Terminal";

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		/// <summary>
		/// Static counter to number terminals. Counter is incremented before first use, first
		/// terminal therefore is "Terminal1".
		/// </summary>
		private static int staticTerminalIdCounter = 0;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Guid guid;
		private string userName;

		// Settings.
		private DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler;
		private TerminalSettingsRoot settingsRoot;

		// Terminal.
		private Domain.Terminal terminal;

		// Logs.
		private Log.Logs log;

		// Time status.
		private Chronometer connectChrono;
		private Chronometer totalConnectChrono;

		// Count status.
		private int txByteCount;
		private int rxByteCount;
		private int txLineCount;
		private int rxLineCount;

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
				this.guid = guid;
			else
				this.guid = Guid.NewGuid();

			// Link and attach to settings.
			this.settingsHandler = settingsHandler;
			this.settingsRoot = this.settingsHandler.Settings;
			this.settingsRoot.ClearChanged();
			AttachSettingsEventHandlers();

			// Set user name.
			staticTerminalIdCounter++;
			if (!this.settingsHandler.SettingsFilePathIsValid || this.settingsRoot.AutoSaved)
				this.userName = TerminalText + staticTerminalIdCounter.ToString();
			else
				UserNameFromFile = this.settingsHandler.SettingsFilePath;

			// Create underlying terminal.
			this.terminal = Domain.TerminalFactory.CreateTerminal(this.settingsRoot.Terminal);
			AttachTerminalEventHandlers();

			// Create log.
			this.log = new Log.Logs(this.settingsRoot.Log);

			// Create chronos.
			this.connectChrono = new Chronometer();
			this.connectChrono.Interval = 1000;
			this.connectChrono.TimeSpanChanged += new EventHandler<TimeSpanEventArgs>(this.totalConnectChrono_TimeSpanChanged);
			this.totalConnectChrono = new Chronometer();
			this.totalConnectChrono.Interval = 1000;
			this.totalConnectChrono.TimeSpanChanged += new EventHandler<TimeSpanEventArgs>(this.connectChrono_TimeSpanChanged);
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
					DetachTerminalEventHandlers();
					DetachSettingsEventHandlers();

					// Then, dispose of objects
					if (this.connectChrono != null)
					{
						this.connectChrono.Dispose();
						this.connectChrono = null;
					}
					if (this.totalConnectChrono != null)
					{
						this.totalConnectChrono.Dispose();
						this.totalConnectChrono = null;
					}
					if (this.log != null)
					{
						this.log.Dispose();
						this.log = null;
					}
					if (this.terminal != null)
					{
						this.terminal.Dispose();
						this.terminal = null;
					}
				}
				this.isDisposed = true;
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
		public virtual string UserName
		{
			get
			{
				AssertNotDisposed();
				return (this.userName);
			}
		}

		private string UserNameFromFile
		{
			set
			{
				this.userName = Path.GetFileNameWithoutExtension(value);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				AssertNotDisposed();
				return (this.terminal.IsStopped);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				return (this.terminal.IsStarted);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (this.terminal.IsOpen);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (this.terminal.IsConnected);
			}
		}

		/// <summary></summary>
		public virtual bool LogIsStarted
		{
			get
			{
				AssertNotDisposed();
				return (this.log.IsStarted);
			}
		}

		/// <summary></summary>
		public virtual MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (this.terminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.terminal.UnderlyingIOInstance);
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
			// Begin logging (in case opening of terminal needs to be logged).
			if (this.settingsRoot.LogIsStarted)
				BeginLog();

			// Then open terminal.
			if (this.settingsRoot.TerminalIsStarted)
				StartIO();
		}

		/// <summary>
		/// Sets terminal settings.
		/// </summary>
		public virtual void SetSettings(Domain.Settings.TerminalSettings settings)
		{
			// Settings have changed, recreate terminal with new settings.
			if (this.terminal.IsStarted)
			{
				// Terminal is open, re-open it with the new settings.
				if (StopIO(false))
				{
					DetachTerminalEventHandlers();    // Detach to suspend events.
					this.settingsRoot.Terminal = settings;
					this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Terminal, this.terminal);
					AttachTerminalEventHandlers();    // Attach and resume events.
					this.terminal.ReloadRepositories();

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
				// Terminal is closed, simply set the new settings.
				DetachTerminalEventHandlers();        // Detach to suspend events.
				this.settingsRoot.Terminal = settings;
				this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Terminal, this.terminal);
				AttachTerminalEventHandlers();        // Attach and resume events.
				this.terminal.ReloadRepositories();

				OnTimedStatusTextRequest("Terminal settings applied.");
			}
		}

		/// <summary>
		/// Sets log settings.
		/// </summary>
		public virtual void SetLogSettings(Log.Settings.LogSettings settings)
		{
			this.settingsRoot.Log = settings;
			this.log.Settings = this.settingsRoot.Log;
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
			if (this.settingsRoot != null)
				this.settingsRoot.Changed += new EventHandler<SettingsEventArgs>(this.settingsRoot_Changed);
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed -= new EventHandler<SettingsEventArgs>(this.settingsRoot_Changed);
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		private void settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			// Nothing to do yet.
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
				return (this.settingsHandler.SettingsFileExists);
			}
		}

		/// <summary></summary>
		public virtual string SettingsFilePath
		{
			get
			{
				AssertNotDisposed();
				return (this.settingsHandler.SettingsFilePath);
			}
		}

		/// <summary></summary>
		public virtual TerminalSettingsRoot SettingsRoot
		{
			get
			{
				AssertNotDisposed();
				return (this.settingsRoot);
			}
		}

		/// <summary></summary>
		public virtual WindowSettings WindowSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.settingsRoot.Window);
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
			if (this.settingsHandler.SettingsFileExists && !this.settingsRoot.AutoSaved)
				success = SaveToFile(false);
			else
				success = SaveToFile(true);
			return (success);
		}

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet.
		/// </summary>
		public virtual bool Save()
		{
			return (Save(true));
		}

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet.
		/// </summary>
		public virtual bool Save(bool autoSaveIsAllowed)
		{
			AssertNotDisposed();

			bool success = false;

			// Save terminal if file path is valid.
			if (this.settingsHandler.SettingsFilePathIsValid)
			{
				if (this.settingsHandler.Settings.AutoSaved)
				{
					if (autoSaveIsAllowed)
						success = SaveToFile(true);
				}
				else
				{
					success = SaveToFile(false);
				}
			}
			else // Auto save creates default file path.
			{
				if (autoSaveIsAllowed)
					success = SaveToFile(true);
			}

			// If not successful yet, request new file path.
			if (!success)
				success = (OnSaveAsFileDialogRequest() == DialogResult.OK);

			return (success);
		}

		/// <summary>
		/// Saves terminal to given file.
		/// </summary>
		public virtual bool SaveAs(string filePath)
		{
			AssertNotDisposed();

			string autoSaveFilePathToDelete = "";
			if (this.settingsRoot.AutoSaved)
				autoSaveFilePathToDelete = this.settingsHandler.SettingsFilePath;

			this.settingsHandler.SettingsFilePath = filePath;
			return (SaveToFile(false, autoSaveFilePathToDelete));
		}

		private bool SaveToFile(bool doAutoSave)
		{
			return (SaveToFile(doAutoSave, ""));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that really all exceptions get caught.")]
		private bool SaveToFile(bool doAutoSave, string autoSaveFilePathToDelete)
		{
			// -------------------------------------------------------------------------------------
			// Skip save if file is up to date and there were no changes.
			// -------------------------------------------------------------------------------------

			if (this.settingsHandler.SettingsFileIsUpToDate && (!this.settingsRoot.HaveChanged))
			{
				// Event must be fired anyway to ensure that dependent objects are updated.
				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, doAutoSave));
				return (true);
			}

			// -------------------------------------------------------------------------------------
			// Save terminal.
			// -------------------------------------------------------------------------------------

			bool success = false;

			if (!doAutoSave)
				OnFixedStatusTextRequest("Saving terminal...");

			if (doAutoSave && (!this.settingsHandler.SettingsFilePathIsValid))
			{
				string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveTerminalFileNamePrefix + Guid.ToString() + ExtensionSettings.TerminalFile;
				this.settingsHandler.SettingsFilePath = autoSaveFilePath;
			}

			try
			{
				this.settingsHandler.Settings.AutoSaved = doAutoSave;
				this.settingsHandler.Save();

				if (!doAutoSave)
					UserNameFromFile = this.settingsHandler.SettingsFilePath;

				success = true;
				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, doAutoSave));

				if (!doAutoSave)
				{
					SetRecent(this.settingsHandler.SettingsFilePath);
					OnTimedStatusTextRequest("Terminal saved");
				}

				// ---------------------------------------------------------------------------------
				// Try to delete existing auto save file.
				// ---------------------------------------------------------------------------------

				try
				{
					if (File.Exists(autoSaveFilePathToDelete))
						File.Delete(autoSaveFilePathToDelete);
				}
				catch { }
			}
			catch (System.Xml.XmlException ex)
			{
				if (!doAutoSave)
				{
					OnFixedStatusTextRequest("Error saving terminal!");
					OnMessageInputRequest
						(
						"Unable to save file" + Environment.NewLine + this.settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
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
			if (!isWorkspaceClose && !this.settingsHandler.SettingsFileExists)
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
				if (!this.settingsHandler.SettingsFileExists)
				{
					success = true; // Consider it successful if there was no file to save
				}
				else // Existing file
				{
					if (this.settingsRoot.AutoSaved) // Existing auto file (w2a/b, t2)
					{
						this.settingsHandler.TryDelete();
						success = true; // Don't care if auto file not successfully deleted
					}

					// Existing normal file (w4a/b, t4a/b) will be handled below
				}

				// Normal (w4a/b, t4a/b)
				if (!success && this.settingsRoot.ExplicitHaveChanged)
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
			if (this.terminal.IsStarted)
				success = StopIO(false);

			// Last, close log
			if (this.log.IsStarted)
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
			if (this.terminal != null)
			{
				this.terminal.IOChanged        += new EventHandler(this.terminal_IOChanged);
				this.terminal.IOControlChanged += new EventHandler(this.terminal_IOControlChanged);
				this.terminal.IORequest        += new EventHandler<Domain.IORequestEventArgs>(this.terminal_IORequest);
				this.terminal.IOError          += new EventHandler<Domain.IOErrorEventArgs>(this.terminal_IOError);

				this.terminal.RawElementSent          += new EventHandler<Domain.RawElementEventArgs>(this.terminal_RawElementSent);
				this.terminal.RawElementReceived      += new EventHandler<Domain.RawElementEventArgs>(this.terminal_RawElementReceived);
				this.terminal.DisplayElementsSent     += new EventHandler<Domain.DisplayElementsEventArgs>(this.terminal_DisplayElementsSent);
				this.terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(this.terminal_DisplayElementsReceived);
				this.terminal.DisplayLinesSent        += new EventHandler<Domain.DisplayLinesEventArgs>(this.terminal_DisplayLinesSent);
				this.terminal.DisplayLinesReceived    += new EventHandler<Domain.DisplayLinesEventArgs>(this.terminal_DisplayLinesReceived);
				this.terminal.RepositoryCleared       += new EventHandler<Domain.RepositoryEventArgs>(this.terminal_RepositoryCleared);
				this.terminal.RepositoryReloaded      += new EventHandler<Domain.RepositoryEventArgs>(this.terminal_RepositoryReloaded);
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged        -= new EventHandler(this.terminal_IOChanged);
				this.terminal.IOControlChanged -= new EventHandler(this.terminal_IOControlChanged);
				this.terminal.IORequest        -= new EventHandler<Domain.IORequestEventArgs>(this.terminal_IORequest);
				this.terminal.IOError          -= new EventHandler<Domain.IOErrorEventArgs>(this.terminal_IOError);

				this.terminal.RawElementSent          -= new EventHandler<Domain.RawElementEventArgs>(this.terminal_RawElementSent);
				this.terminal.RawElementReceived      -= new EventHandler<Domain.RawElementEventArgs>(this.terminal_RawElementReceived);
				this.terminal.DisplayElementsSent     -= new EventHandler<Domain.DisplayElementsEventArgs>(this.terminal_DisplayElementsSent);
				this.terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(this.terminal_DisplayElementsReceived);
				this.terminal.DisplayLinesSent        -= new EventHandler<Domain.DisplayLinesEventArgs>(this.terminal_DisplayLinesSent);
				this.terminal.DisplayLinesReceived    -= new EventHandler<Domain.DisplayLinesEventArgs>(this.terminal_DisplayLinesReceived);
				this.terminal.RepositoryCleared       -= new EventHandler<Domain.RepositoryEventArgs>(this.terminal_RepositoryCleared);
				this.terminal.RepositoryReloaded      -= new EventHandler<Domain.RepositoryEventArgs>(this.terminal_RepositoryReloaded);
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
		private bool terminal_IOChanged_isConnected;

		private void terminal_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(e);

			if      ( this.terminal.IsConnected && !this.terminal_IOChanged_isConnected)
			{
				this.connectChrono.Restart();
				this.totalConnectChrono.Start();
			}
			else if (!this.terminal.IsConnected &&  this.terminal_IOChanged_isConnected)
			{
				this.connectChrono.Stop();
				this.totalConnectChrono.Stop();
			}

			this.terminal_IOChanged_isConnected = this.terminal.IsConnected;
		}

		private void terminal_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void terminal_IORequest(object sender, Domain.IORequestEventArgs e)
		{
			OnIORequest(e);
		}

		private void terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			OnIOError(e);
		}

		private void terminal_RawElementSent(object sender, Domain.RawElementEventArgs e)
		{
			// Count
			this.txByteCount += e.Element.Data.Length;
			OnIOCountChanged(new EventArgs());

			// Log
			if (this.log.IsStarted)
			{
				this.log.WriteBytes(e.Element.Data, Log.LogStreams.RawTx);
				this.log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
			}
		}

		private void terminal_RawElementReceived(object sender, Domain.RawElementEventArgs e)
		{
			// Count
			this.rxByteCount += e.Element.Data.Length;
			OnIOCountChanged(new EventArgs());

			// Log
			if (this.log.IsStarted)
			{
				this.log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
				this.log.WriteBytes(e.Element.Data, Log.LogStreams.RawRx);
			}
		}

		private void terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			// Display
			OnDisplayElementsSent(e);

			// Log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (this.log.IsStarted)
				{
					if (de is Domain.DisplayElement.LineBreak)
					{
						this.log.WriteEol(Log.LogStreams.NeatTx);
						this.log.WriteEol(Log.LogStreams.NeatBidir);
					}
					else
					{
						this.log.WriteString(de.Text, Log.LogStreams.NeatTx);
						this.log.WriteString(de.Text, Log.LogStreams.NeatBidir);
					}
				}
			}
		}

		private void terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			// Display
			OnDisplayElementsReceived(e);

			// Log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (this.log.IsStarted)
				{
					if (de is Domain.DisplayElement.LineBreak)
					{
						this.log.WriteEol(Log.LogStreams.NeatBidir);
						this.log.WriteEol(Log.LogStreams.NeatRx);
					}
					else
					{
						this.log.WriteString(de.Text, Log.LogStreams.NeatBidir);
						this.log.WriteString(de.Text, Log.LogStreams.NeatRx);
					}
				}
			}
		}

		private void terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count
			this.txLineCount += e.Lines.Count;
			OnIOCountChanged(new EventArgs());

			// Display
			OnDisplayLinesSent(e);
		}

		private void terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			// Count
			this.rxLineCount += e.Lines.Count;
			OnIOCountChanged(new EventArgs());

			// Display
			OnDisplayLinesReceived(e);
		}

		private void terminal_RepositoryCleared(object sender, Domain.RepositoryEventArgs e)
		{
			OnRepositoryCleared(e);
		}

		private void terminal_RepositoryReloaded(object sender, Domain.RepositoryEventArgs e)
		{
			OnRepositoryReloaded(e);
		}

		#endregion

		#region Terminal > Start/Stop
		//------------------------------------------------------------------------------------------
		// Terminal > Start/Stop
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public virtual bool StartIO()
		{
			return (StartIO(true));
		}

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that really all exceptions get caught.")]
		private bool StartIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Starting terminal...");
			try
			{
				if (this.terminal.Start())
				{
					if (saveStatus)
						this.settingsRoot.TerminalIsStarted = this.terminal.IsStarted;

					OnTimedStatusTextRequest("Terminal started");
					success = true;
				}
			}
			catch (Exception ex)
			{
				OnFixedStatusTextRequest("Error starting terminal!");

				string ioText;
				if (this.settingsRoot.IOType == Domain.IOType.SerialPort)
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
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public virtual bool StopIO()
		{
			return (StopIO(true));
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that really all exceptions get caught.")]
		private bool StopIO(bool saveStatus)
		{
			bool success = false;

			OnFixedStatusTextRequest("Stopping terminal...");
			try
			{
				this.terminal.Stop();

				if (saveStatus)
					this.settingsRoot.TerminalIsStarted = this.terminal.IsStarted;

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
		/// Toggles RTS line if current flow control settings allow this.
		/// </summary>
		public virtual void RequestToggleRts()
		{
			if (this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.SerialFlowControl.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.terminal.UnderlyingIOInstance;
				port.ToggleRts();
				this.settingsRoot.Terminal.IO.SerialPort.RtsEnabled = port.RtsEnable;
			}
		}

		/// <summary>
		/// Toggles DTR line if current flow control settings allow this.
		/// </summary>
		public virtual void RequestToggleDtr()
		{
			if (this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.SerialFlowControl.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.terminal.UnderlyingIOInstance;
				port.ToggleDtr();
				this.settingsRoot.Terminal.IO.SerialPort.DtrEnabled = port.DtrEnable;
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
				this.terminal.Send(b);
				OnTimedStatusTextRequest(b.Length + " bytes sent");
			}
			catch (System.IO.IOException ex)
			{
				OnFixedStatusTextRequest("Error sending " + b.Length + " bytes!");

				string text;
				string title;
				PrepareSendMessageInputRequest(out text, out title);
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
			Send(s, true);
		}


		private void Send(string s)
		{
			Send(s, false);
		}

		private void Send(string s, bool isLine)
		{
			string sent;
			if (!string.IsNullOrEmpty(s))
				sent = @"""" + s + @"""";
			else if (isLine)
				sent = "EOL";
			else
				sent = "<Nothing>";

			OnFixedStatusTextRequest("Sending " + sent + "...");
			try
			{
				if (isLine)
					this.terminal.SendLine(s);
				else
					this.terminal.Send(s);

				OnTimedStatusTextRequest(sent + " sent");
			}
			catch (System.IO.IOException ex)
			{
				OnFixedStatusTextRequest("Error sending " + sent + "!");

				string text;
				string title;
				PrepareSendMessageInputRequest(out text, out title);
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
				OnFixedStatusTextRequest("Error sending " + sent + "!");
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

		private void PrepareSendMessageInputRequest(out string text, out string title)
		{
			StringBuilder textBuilder = new StringBuilder();
			StringBuilder titleBuilder = new StringBuilder();

			textBuilder.Append("Unable to write to ");
			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
					textBuilder.Append("port");
					titleBuilder.Append("Serial Port");
					break;

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				case Domain.IOType.Udp:
					textBuilder.Append("socket");
					titleBuilder.Append("Socket");
					break;

				case Domain.IOType.UsbHid:
					textBuilder.Append("device");
					titleBuilder.Append("Device");
					break;

				default:
					throw (new NotImplementedException("I/O type " + this.settingsRoot.IOType + "misses implementation"));
			}
			textBuilder.Append(":");
			titleBuilder.Append(" Error");

			text = textBuilder.ToString();
			title = titleBuilder.ToString();
		}

		#endregion

		#region Terminal > Send Text
		//------------------------------------------------------------------------------------------
		// Terminal > Send Text
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends text command given by terminal settings.
		/// </summary>
		public virtual void SendText()
		{
			Command command = this.settingsRoot.SendCommand.Command;
			if (command.IsValidText)
			{
				SendText(command);

				// Copy line commands into history.
				if (command.IsSingleLineText || command.IsMultiLineText || command.IsPartialEolText)
				{
					// Clone to a normal single line command.
					Command clone;
					if (command.IsPartialEolText)
						clone = new Command(command.Description, command.PartialText, command.DefaultRadix);
					else
						clone = new Command(command);

					// Put clone into history.
					this.settingsRoot.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary
						(
							new RecentItem<Command>(clone)
						);
				}

				// Clear command if desired.
				if (!this.settingsRoot.Send.KeepCommand)
					this.settingsRoot.SendCommand.Command = new Command(); // Set command to "".
			}
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="command">Text command to be sent.</param>
		public virtual void SendText(Command command)
		{
			if (command.IsValidText)
			{
				if (command.IsSingleLineText)
				{
					if (SendCommandSettings.IsEasterEggCommand(command.SingleLineText))
						SendLine(SendCommandSettings.EasterEggCommandText);
					else
						SendLine(command.SingleLineText);
				}
				else if (command.IsMultiLineText)
				{
					foreach (string line in command.MultiLineText)
						SendLine(line);
				}
				else if (command.IsPartialText)
				{
					if (!command.IsPartialEolText)
						Send(command.PartialText);
					else
						SendLine(""); // Simply add EOL to finalize a partial line.
				}
			}
		}

		#endregion

		#region Terminal > Send File
		//------------------------------------------------------------------------------------------
		// Terminal > Send File
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends file given by terminal settings.
		/// </summary>
		public virtual void SendFile()
		{
			SendFile(this.settingsRoot.SendFile.Command);
		}

		/// <summary>
		/// Sends given file.
		/// </summary>
		/// <param name="command">File to be sent.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that really all exceptions get caught.")]
		public virtual void SendFile(Command command)
		{
			if (!command.IsValidFilePath)
				return;

			string filePath = command.FilePath;

			try
			{
				if (this.terminal is Domain.TextTerminal)
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
							sr.Close(); // Close file before sending.
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
						fs.Close(); // Close file before sending.
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
		/// <param name="page">Page 1..max.</param>
		/// <param name="command">Command 1..max.</param>
		public virtual bool SendPredefined(int page, int command)
		{
			// Verify page index.
			List<Model.Types.PredefinedCommandPage> pages = this.settingsRoot.PredefinedCommand.Pages;
			if ((page < 1) && (page > pages.Count))
				return (false);

			// Verify command index.
			List<Model.Types.Command> commands = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands;
			bool isDefined =
				(
					(commands != null) &&
					(commands.Count >= command) &&
					(commands[command - 1] != null) &&
					(commands[command - 1].IsDefined)
				);
			if (!isDefined)
				return (false);

			// Verify command.
			Model.Types.Command c = this.settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];
			if (c.IsValidText)
			{
				SendText(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendCommand.Command = new Command(c); // Copy command if desired.

				return (true);
			}
			else if (c.IsValidFilePath)
			{
				SendFile(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendFile.Command = new Command(c); // Copy command if desired.

				return (true);
			}
			else
			{
				return (true);
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
			this.terminal.ReloadRepositories();
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual List<Domain.DisplayLine> RepositoryToDisplayLines(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			return (this.terminal.RepositoryToDisplayLines(repositoryType));
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
			return (this.terminal.RepositoryToString(repositoryType));
		}

		/// <summary>
		/// Clears given repository.
		/// </summary>
		public virtual void ClearRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();
			this.terminal.ClearRepository(repositoryType);
		}

		/// <summary>
		/// Clears all repositories.
		/// </summary>
		public virtual void ClearRepositories()
		{
			AssertNotDisposed();
			this.terminal.ClearRepositories();
		}

		#endregion

		#region Terminal > Time Status
		//------------------------------------------------------------------------------------------
		// Terminal > Time Status
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual TimeSpan ConnectTime
		{
			get
			{
				AssertNotDisposed();
				return (this.connectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		public virtual TimeSpan TotalConnectTime
		{
			get
			{
				AssertNotDisposed();
				return (this.totalConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		public virtual void RestartConnectTime()
		{
			AssertNotDisposed();
			connectChrono.Restart();
			totalConnectChrono.Restart();
		}

		private void connectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			// Don't fire event. Events are fired by total connect chrono anyway.
		}

		private void totalConnectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
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
			get { return (this.txByteCount); }
		}

		/// <summary></summary>
		public virtual int TxLineCount
		{
			get { return (this.txLineCount); }
		}

		/// <summary></summary>
		public virtual int RxByteCount
		{
			get { return (this.rxByteCount); }
		}

		/// <summary></summary>
		public virtual int RxLineCount
		{
			get { return (this.rxLineCount); }
		}

		/// <summary></summary>
		public virtual void ResetIOCount()
		{
			this.txByteCount = 0;
			this.txLineCount = 0;
			this.rxByteCount = 0;
			this.rxLineCount = 0;

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
				this.log.Settings = this.settingsRoot.Log;
				this.log.Begin();
				this.settingsRoot.LogIsStarted = true;
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
				this.log.Clear();
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
				this.log.End();

				if (saveStatus)
					this.settingsRoot.LogIsStarted = false;
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
