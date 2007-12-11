using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.Event;
using MKY.Utilities.IO;

using YAT.Settings;
using YAT.Settings.Application;

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

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Main()
		{
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

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public bool OpenRecent(int userIndex)
		{
			return (OpenFromFile(ApplicationSettings.LocalUser.RecentFiles.FilePaths[userIndex - 1].Item));
		}

		/// <summary>
		/// Opens YAT and opens the workspace or terminal file given. This method can directly
		/// be called from the main providing the command line arguments.
		/// </summary>
		/// <param name="filePath">Workspace or terminal file</param>
		/// <returns>true if successfully opened the workspace or terminal</returns>
		public bool OpenFromFile(string filePath)
		{
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
				return (OpenTerminalFromFile(filePath));
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

		private bool OpenWorkspaceFromFile(string filePath)
		{
			// close all terminal 
			CloseAllTerminals();
			if (MdiChildren.Length > 0)
				return (false);

			OnFixedStatusTextRequest("Opening workspace...");
			try
			{
				_workspaceSettingsHandler.SettingsFilePath = filePath;
				_workspaceSettingsHandler.Load();
				_workspaceSettingsRoot = _workspaceSettingsHandler.Settings;

				ApplicationSettings.LocalUser.General.CurrentWorkspaceFilePath = filePath;
				ApplicationSettings.SaveLocalUser();

				if (!_workspaceSettingsRoot.AutoSaved)
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

			int terminalCount = _workspaceSettingsRoot.TerminalSettings.Count;
			if (terminalCount == 1)
				OnFixedStatusTextRequest("Opening workspace terminal...");
			else if (terminalCount > 1)
				OnFixedStatusTextRequest("Opening workspace terminals...");

			TerminalSettingsItemCollection clone = new TerminalSettingsItemCollection(_workspaceSettingsRoot.TerminalSettings);
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
						"Unable to open file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
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

			// attach workspace settings after terminals have been opened
			AttachWorkspaceSettingsHandlers();

			if (terminalCount == 1)
				OnTimedStatusTextRequest("Workspace terminal opened");
			else if (terminalCount > 1)
				OnTimedStatusTextRequest("Workspace terminals opened");
			else
				OnTimedStatusTextRequest("Workspace contains no terminal to open");

			return (true);
		}

		/// <summary>
		/// Saves all terminal files and the workspace.
		/// </summary>
		private void SaveAll()
		{
			SaveAllTerminals();
			SaveWorkspace();
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

		#endregion
	}
}
