using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Settings;

using YAT.Settings.Workspace;

namespace YAT.Model
{
	public class Workspace : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed = false;

		private Guid _guid = Guid.NewGuid();
		private DocumentSettingsHandler<WorkspaceSettingsRoot> _workspaceSettingsHandler;
		private WorkspaceSettingsRoot _workspaceSettingsRoot;
		private bool _handlingWorkspaceSettingsIsSuspended = false;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>

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

		#region Workspace Settings
		//==========================================================================================
		// Workspace Settings
		//==========================================================================================

		private void AttachWorkspaceSettingsHandlers()
		{
			_workspaceSettingsRoot.ClearChanged();
			_workspaceSettingsRoot.Changed += new EventHandler<SettingsEventArgs>(_workspaceSettings_Changed);
		}

		//------------------------------------------------------------------------------------------
		// Workspace Settings Events
		//------------------------------------------------------------------------------------------

		private void _workspaceSettings_Changed(object sender, SettingsEventArgs e)
		{
			if (_handlingWorkspaceSettingsIsSuspended)
				return;

			if (ApplicationSettings.LocalUser.General.AutoSaveWorkspace)
				SaveWorkspaceToFile(true);
		}

		private void SuspendHandlingWorkspaceSettings()
		{
			_handlingWorkspaceSettingsIsSuspended = true;
		}

		private void ResumeHandlingWorkspaceSettings()
		{
			_handlingWorkspaceSettingsIsSuspended = false;
		}

		#endregion

		#region Workspace
		//==========================================================================================
		// Workspace
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
				if (_workspaceSettingsHandler.SettingsFilePathIsValid && !_workspaceSettingsHandler.Settings.AutoSaved)
					SaveWorkspaceToFile(false);
				else
					ShowSaveWorkspaceAsFileDialog();
			}
		}

		private void ShowSaveWorkspaceAsFileDialog()
		{
			SetFixedStatusText("Saving workspace as...");
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save Workspace As";
			sfd.Filter = ExtensionSettings.WorkspaceFilesFilter;
			sfd.DefaultExt = ExtensionSettings.WorkspaceFiles;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath;
			sfd.FileName = Environment.UserName + "." + sfd.DefaultExt;
			if ((sfd.ShowDialog(this) == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUser();

				string autoSaveFilePathToDelete = "";
				if (_workspaceSettingsRoot.AutoSaved)
					autoSaveFilePathToDelete = _workspaceSettingsHandler.SettingsFilePath;

				_workspaceSettingsHandler.SettingsFilePath = sfd.FileName;
				SaveWorkspaceToFile(false, autoSaveFilePathToDelete);
			}
			else
			{
				ResetStatusText();
			}
		}

		private void SaveWorkspaceToFile(bool autoSave)
		{
			SaveWorkspaceToFile(autoSave, "");
		}

		private void SaveWorkspaceToFile(bool autoSave, string autoSaveFilePathToDelete)
		{
			if (!autoSave)
				SetFixedStatusText("Saving workspace...");

			try
			{
				_workspaceSettingsRoot.SuspendChangeEvent();

				// set workspace file path before terminals are saved in order
				// to correctly retrieve relative paths
				if (autoSave)
				{
					string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveWorkspaceFileNamePrefix + Guid.ToString() + ExtensionSettings.WorkspaceFiles;
					if (!_workspaceSettingsHandler.SettingsFilePathIsValid)
						_workspaceSettingsHandler.SettingsFilePath = autoSaveFilePath;
				}

				// save all contained terminals
				foreach (Form f in MdiChildren)
				{
					((Gui.Forms.Terminal)f).RequestAutoSaveFile();
				}

				// update workspace
				foreach (Form f in MdiChildren)
				{
					AddToOrReplaceInWorkspace((Gui.Forms.Terminal)f);
				}

				// save workspace
				_workspaceSettingsHandler.Settings.AutoSaved = autoSave;
				_workspaceSettingsHandler.Save();

				_workspaceSettingsRoot.ClearChanged();
				_workspaceSettingsRoot.ResumeChangeEvent();

				ApplicationSettings.LocalUser.General.CurrentWorkspaceFilePath = _workspaceSettingsHandler.SettingsFilePath;
				ApplicationSettings.SaveLocalUser();

				if (!autoSave)
				{
					SetRecent(_workspaceSettingsHandler.SettingsFilePath);
					SetTimedStatusText("Workspace saved");
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
					SetFixedStatusText("Error saving workspace!");
					MessageBox.Show
						(
						this,
						"Unable to save file" + Environment.NewLine + _workspaceSettingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					SetTimedStatusText("Workspace not saved!");
				}
			}
		}

		#endregion

		#region Terminals
		//==========================================================================================
		// Terminals
		//==========================================================================================

		private int GetNextTerminalId()
		{
			_terminalIdCounter++;
			return (_terminalIdCounter);
		}

		private void ShowNewTerminalDialog()
		{
			SetFixedStatusText("New terminal...");
			Gui.Forms.NewTerminal f = new Gui.Forms.NewTerminal(ApplicationSettings.LocalUser.NewTerminal);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUser.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.SaveLocalUser();

				SetFixedStatusText("Creating new terminal...");

				DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> sh = new DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot>(f.TerminalSettingsResult);
				Gui.Forms.Terminal terminal = new Gui.Forms.Terminal(sh);

				terminal.UserName = _TerminalText + GetNextTerminalId();
				terminal.MdiParent = this;
				terminal.TerminalChanged += new EventHandler(mdi_child_TerminalChanged);
				terminal.TerminalSaved += new EventHandler<TerminalSavedEventArgs>(mdi_child_TerminalSaved);
				terminal.FormClosed += new FormClosedEventHandler(mdi_child_FormClosed);
				terminal.Show();

				AddToOrReplaceInWorkspace(terminal);

				SetTimedStatusText("New terminal created");
			}
			else
			{
				ResetStatusText();
			}
		}

		private void ShowOpenTerminalFromFileDialog()
		{
			SetFixedStatusText("Opening terminal...");
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Open";
			ofd.Filter = ExtensionSettings.TerminalFilesFilter;
			ofd.DefaultExt = ExtensionSettings.TerminalFiles;
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.TerminalFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName != ""))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.TerminalFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUser();

				OpenTerminalFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		private void CloseTerminal()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestCloseFile();
		}

		private void CloseAllTerminals()
		{
			foreach (Form f in MdiChildren)
				((Gui.Forms.Terminal)f).RequestCloseFile();
		}

		private void SaveTerminal()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestSaveFile();
		}

		private void SaveAllTerminals()
		{
			foreach (Form f in MdiChildren)
				((Gui.Forms.Terminal)f).RequestSaveFile();
		}

		private void OpenTerminalIO()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestOpenTerminal();
		}

		private void CloseTerminalIO()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestCloseTerminal();
		}

		private void EditTerminalSettings()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestEditTerminalSettings();
		}

		private void AddToOrReplaceInWorkspace(Terminal terminal)
		{
			WindowSettings ws = new WindowSettings();
			ws.State = terminal.WindowState;
			ws.Location = terminal.Location;
			ws.Size = terminal.Size;

			TerminalSettingsItem tsi = new TerminalSettingsItem();
			tsi.Guid = terminal.Guid;
			string filePath = terminal.SettingsFilePath;
			if (ApplicationSettings.LocalUser.General.UseRelativePaths)
			{
				XPathCompareResult pcr = XPath.CompareFilePaths(_workspaceSettingsHandler.SettingsFilePath, terminal.SettingsFilePath);
				if (pcr.AreRelative)
					filePath = pcr.RelativePath;
			}
			tsi.FilePath = filePath;
			tsi.Window = ws;

			_workspaceSettingsRoot.TerminalSettings.AddOrReplaceGuid(tsi);
			_workspaceSettingsRoot.SetChanged();
		}

		private void RemoveFromWorkspace(Terminal terminal)
		{
			_workspaceSettingsRoot.TerminalSettings.RemoveGuid(terminal.Guid);
			_workspaceSettingsRoot.SetChanged();
		}

		//------------------------------------------------------------------------------------------
		// MDI Children > Events
		//------------------------------------------------------------------------------------------

		private void mdi_child_TerminalChanged(object sender, EventArgs e)
		{
			SetTimedStatus(Status.ChildChanged);
			SetToolControls();
		}

		private void mdi_child_TerminalSaved(object sender, TerminalSavedEventArgs e)
		{
			AddToOrReplaceInWorkspace((Terminal)sender);
			if (!e.AutoSave)
			{
				SetRecent(e.FilePath);
				SetTimedStatus(Status.ChildSaved);
			}
			SetToolControls();
		}

		private void mdi_child_FormClosed(object sender, FormClosedEventArgs e)
		{
			RemoveFromWorkspace((Terminal)sender);
			SetTimedStatus(Status.ChildClosed);
			SetToolControls();
		}

		#endregion
	}
}
