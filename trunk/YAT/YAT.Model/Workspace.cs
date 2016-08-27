//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Settings;

using YAT.Application.Utilities;
using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Workspaces (.yaw) of the YAT application model.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Workspace : IDisposable, IGuidProvider
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary>
		/// An invalid index is represented by -1.
		/// </summary>
		private const int InvalidIndex = -1;

		private const int FirstValidIndex = 0;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private WorkspaceStartArgs startArgs;
		private Guid guid;

		// Settings.
		private DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler;
		private WorkspaceSettingsRoot settingsRoot;

		// Terminal list.
		private GuidList<Terminal> terminals = new GuidList<Terminal>();
		private Terminal activeTerminal; // = null;
		private Dictionary<int, Terminal> fixedIndices = new Dictionary<int, Terminal>();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>Fired when a new terminal was added to the workspace.</summary>
		public event EventHandler<EventArgs<Terminal>> TerminalAdded;

		/// <summary>Fired when a terminal was removed from the workspace.</summary>
		public event EventHandler<EventArgs<Terminal>> TerminalRemoved;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<DialogEventArgs> SaveAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<Cursor>> CursorRequest;

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
		public Workspace(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
			: this(new WorkspaceStartArgs(), settingsHandler, Guid.Empty)
		{
		}

		/// <summary></summary>
		public Workspace(WorkspaceStartArgs startArgs)
			: this(startArgs, new DocumentSettingsHandler<WorkspaceSettingsRoot>(), Guid.Empty)
		{
		}

		/// <remarks><see cref="Guid.Empty"/> cannot be used as default argument as it is read-only.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public Workspace(WorkspaceStartArgs startArgs, DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid)
		{
			try
			{
				DebugMessage("Creating...");

				this.startArgs = startArgs;

				if (guid != Guid.Empty)
					this.guid = guid;
				else
					this.guid = Guid.NewGuid();

				// Link and attach to settings:
				this.settingsHandler = settingsHandler;
				this.settingsRoot = this.settingsHandler.Settings;
				this.settingsRoot.ClearChanged();
				AttachSettingsEventHandlers();

				DebugMessage("...successfully created.");
			}
			catch (Exception ex)
			{
				DebugMessage("...failed!");
				DebugEx.WriteException(GetType(), ex);

				Dispose(); // Immediately call Dispose() to ensure no zombies remain!
				throw; // Re-throw!
			}
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
			DebugEventManagement.DebugNotifyAllEventRemains(this);

			if (!this.isDisposed)
			{
				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the terminals have already been closed, otherwise...

					// ...detach event handlers to ensure that no more events are received...
					DetachAllTerminalEventHandlers();

					// ...dispose of terminals (normally they dispose of themselves)...
					if (this.terminals != null)
					{
						foreach (Terminal t in this.terminals)
							t.Dispose();

						this.terminals.Clear();
					}

					// ...and finally the settings:
					DetachSettingsEventHandlers();
					DisposeSettingsHandler();
				}

				// Set state to disposed:
				this.terminals = null;
				this.isDisposed = true;

				DebugMessage("...successfully disposed.");
			}
		}

#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Workspace()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

#endif // DEBUG

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.guid);
			}
		}

		/// <summary>
		/// This is the automatically assigned workspace name. The name is corresponding to the
		/// name of the currently active terminal.
		/// </summary>
		public virtual string AutoName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.activeTerminal != null)
					return (this.activeTerminal.AutoName);
				else
					return (ApplicationEx.ProductName);
			}
		}

		/// <summary></summary>
		public virtual WorkspaceStartArgs StartArgs
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.startArgs);
			}
		}

		/// <summary></summary>
		public virtual string SettingsFilePath
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFilePath);
				else
					return ("");
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileExists
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsWritable
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileIsWritable);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileHasAlreadyBeenNormallySaved
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if ((this.settingsHandler != null) && (this.settingsRoot != null))
					return (this.settingsHandler.SettingsFileSuccessfullyLoaded && !this.settingsRoot.AutoSaved);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileNoLongerExists
		{
			get
			{
				return (SettingsFileHasAlreadyBeenNormallySaved && !SettingsFileExists);
			}
		}

		/// <summary>
		/// Returns number of terminals within workspace.
		/// </summary>
		public virtual int TerminalCount
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminals != null)
					return (this.terminals.Count);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns an array of all terminals within workspace or <c>null</c> if there are no terminals.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required for testing only.")]
		public virtual Terminal[] Terminals
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminals != null)
					return (this.terminals.ToArray());
				else
					return (null);
			}
		}

		/// <summary>
		/// Returns active terminal within workspace or <c>null</c> if no terminal is active.
		/// </summary>
		public virtual Terminal ActiveTerminal
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.activeTerminal);
			}
		}

		/// <summary>
		/// Returns the one based sequential index of the active terminal.
		/// </summary>
		public virtual int ActiveTerminalSequentialIndex
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.activeTerminal != null)
					return (this.activeTerminal.SequentialIndex);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the dynamic one based index of the active terminal.
		/// </summary>
		public virtual int ActiveTerminalDynamicIndex
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.activeTerminal != null)
					return (GetDynamicIndexByTerminal(this.activeTerminal));
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns the fixed one based index of the active terminal.
		/// </summary>
		public virtual int ActiveTerminalFixedIndex
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.activeTerminal != null)
					return (GetFixedIndexByTerminal(this.activeTerminal));
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns a text containing information about the active terminal.
		/// </summary>
		public virtual string ActiveTerminalStatusText
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (ActiveTerminal != null)
				{
					StringBuilder sb = new StringBuilder(ActiveTerminal.AutoName);
					sb.Append("/Seq#");
					sb.Append(ActiveTerminalSequentialIndex);
					sb.Append("/Dyn#");
					sb.Append(ActiveTerminalDynamicIndex);
					sb.Append("/Fix#");
					sb.Append(ActiveTerminalFixedIndex);
					return (sb.ToString());
				}
				else
				{
					return ("");
				}
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
			if (this.settingsRoot != null)
				this.settingsRoot.Changed += settingsRoot_Changed;
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed -= settingsRoot_Changed;
		}

		private void DisposeSettingsHandler()
		{
			if (this.settingsHandler != null)
			{
				this.settingsHandler.Dispose();
				this.settingsHandler = null;
			}
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object settingsRoot_Changed_SyncObj = new object();

		private void settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			// Prevent recursive calls to improve performance:
			if (Monitor.TryEnter(settingsRoot_Changed_SyncObj))
			{
				try
				{
					if (ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace)
						TryAutoSaveIfFileAlreadyAutoSaved();
				}
				finally
				{
					Monitor.Exit(settingsRoot_Changed_SyncObj);
				}
			} // Monitor.TryEnter()
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
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsRoot.HaveChanged);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual WorkspaceSettingsRoot SettingsRoot
		{
			get
			{
				// Do not call AssertNotDisposed() to still allow reading the settings after the
				// workspace has been disposed. This is required for certain test cases.

				return (this.settingsRoot);
			}
		}

		#endregion

		#endregion

		#region Save
		//==========================================================================================
		// Save
		//==========================================================================================

		/// <summary>
		/// Performs auto save on previously auto saved files.
		/// </summary>
		private bool TryAutoSaveIfFileAlreadyAutoSaved()
		{
			bool success = false;

			if (this.settingsHandler.SettingsFilePathIsValid && this.settingsRoot.AutoSaved)
				success = SaveDependentOnState(true, false); // Try auto save, i.e. no user interaction.

			return (success);
		}

		/// <summary>
		/// Saves all terminals and workspace to files, prompts for files if they don't exist yet.
		/// </summary>
		public virtual bool Save()
		{
			AssertNotDisposed();

			// First, save all contained terminals:
			if (!SaveAllTerminals())
				return (false);

			// Then, save the workspace itself:
			return (SaveDependentOnState(true, true)); // Do normal/manual save.
		}

		/// <summary>
		/// This method implements the logic that is needed when saving, opposed to the method
		/// <see cref="SaveToFile"/> which just performs the actual save, i.e. file handling.
		/// </summary>
		/// <param name="autoSaveIsAllowed">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="userInteractionIsAllowed">Indicates whether user interaction is allowed.</param>
		private bool SaveDependentOnState(bool autoSaveIsAllowed, bool userInteractionIsAllowed)
		{
			// Evaluate auto save.
			bool isAutoSave;
			if (this.settingsHandler.SettingsFilePathIsValid && !this.settingsRoot.AutoSaved)
				isAutoSave = false;
			else
				isAutoSave = autoSaveIsAllowed;

			// -------------------------------------------------------------------------------------
			// Skip auto save if there is no reason to save, in order to increase speed.
			// -------------------------------------------------------------------------------------

			if (isAutoSave && this.settingsHandler.SettingsFileIsUpToDate && !this.settingsRoot.HaveChanged)
			{
				// Event must be fired anyway to ensure that dependent objects are updated.
				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, isAutoSave));
				OnTimedStatusTextRequest("Workspace has no changes to be saved.");
				return (true);
			}

			// -------------------------------------------------------------------------------------
			// Create auto save file path or request manual/normal file path if necessary.
			// -------------------------------------------------------------------------------------

			if (!this.settingsHandler.SettingsFilePathIsValid)
			{
				if (isAutoSave)
				{
					StringBuilder autoSaveFilePath = new StringBuilder();

					autoSaveFilePath.Append(Application.Settings.GeneralSettings.AutoSaveRoot);
					autoSaveFilePath.Append(Path.DirectorySeparatorChar);
					autoSaveFilePath.Append(Application.Settings.GeneralSettings.AutoSaveWorkspaceFileNamePrefix);
					autoSaveFilePath.Append(Guid.ToString());
					autoSaveFilePath.Append(ExtensionHelper.WorkspaceFile);

					this.settingsHandler.SettingsFilePath = autoSaveFilePath.ToString();
				}
				else if (userInteractionIsAllowed)
				{
					// This Save As... request will request the file path from the user and then call
					// the 'SaveAs()' method below.
					switch (OnSaveAsFileDialogRequest())
					{
						case DialogResult.OK:
						case DialogResult.Yes:
							return (true);

						case DialogResult.No:
							OnTimedStatusTextRequest("Workspace not saved!");
							return (true);

						default:
							return (false);
					}
				}
				else
				{
					// Let save fail if the file path is invalid and no user interaction is allowed.
					return (false);
				}
			}
			else // SettingsFilePathIsValid
			{
				if (userInteractionIsAllowed)
				{
					// Ensure that existing former auto files are 'Saved As' if this is no auto save.
					if (this.settingsRoot.AutoSaved && !isAutoSave)
					{
						// This Save As... request will request the file path from the user and then call
						// the 'SaveAs()' method below.
						switch (OnSaveAsFileDialogRequest())
						{
							case DialogResult.OK:
							case DialogResult.Yes:
								return (true);

							case DialogResult.No:
								OnTimedStatusTextRequest("Workspace not saved!");
								return (true);

							default:
								return (false);
						}
					}

					// Ensure that normal files which are write-protected or no longer exist are 'Saved As'.
					if (!SettingsFileIsWritable || SettingsFileNoLongerExists)
					{
						string reason;
						if (!SettingsFileIsWritable)
							reason = "The file is write-protected.";
						else
							reason = "The file no longer exists.";

						DialogResult dr = OnMessageInputRequest
						(
							"Unable to save file" + Environment.NewLine + this.settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
							reason + " Would you like to save the file at another location or cancel?",
							"File Error",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question
						);

						switch (dr)
						{
							case DialogResult.Yes:
								switch (OnSaveAsFileDialogRequest())
								{
									case DialogResult.OK:
									case DialogResult.Yes:
										return (true);

									case DialogResult.No:
										OnTimedStatusTextRequest("Workspace not saved!");
										return (true);

									default:
										return (false);
								}

							case DialogResult.No:
								OnTimedStatusTextRequest("Workspace not saved!");
								return (true);

							default:
								OnTimedStatusTextRequest("Cancelled!");
								return (false);
						}
					}
				}
			}

			// -------------------------------------------------------------------------------------
			// Save if allowed so.
			// -------------------------------------------------------------------------------------

			if (this.settingsHandler.SettingsFileIsWritable)
				return (SaveToFile(isAutoSave, null));
			else
				return (false); // Let save fail if file shall not be written.
		}

		/// <summary>
		/// Saves workspace settings to given file, also saves all terminals and prompts for
		/// files if they don't exist yet.
		/// </summary>
		/// <remarks>
		/// Note that not only the workspace gets saved, but also the terminals. Consider the
		/// default case:
		/// 1. Application start, default workspace is created
		/// 2. Create a terminal
		/// 3. Intentionally save the workspace as
		/// => The user expects to save the terminal as well.
		/// => No saving the terminal would lead to a normal file referring to an auto file!
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public virtual bool SaveAs(string filePath)
		{
			AssertNotDisposed();

			// First, save all contained terminals:
			if (!SaveAllTerminals(false, true))
				return (false);

			// Request the deletion of the obsolete auto saved settings file given the new file is different:
			string autoSaveFilePathToDelete = null;
			if (this.settingsRoot.AutoSaved && (!StringEx.EqualsOrdinalIgnoreCase(filePath, this.settingsHandler.SettingsFilePath)))
				autoSaveFilePathToDelete = this.settingsHandler.SettingsFilePath;

			// Set the new file path:
			this.settingsHandler.SettingsFilePath = filePath;

			// Then, save the workspace itself:
			return (SaveToFile(false, autoSaveFilePathToDelete));
		}

		/// <param name="isAutoSave">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="autoSaveFilePathToDelete">
		/// The path to the former auto saved file, it will be deleted if the file can successfully
		/// be stored in the new location.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private bool SaveToFile(bool isAutoSave, string autoSaveFilePathToDelete)
		{
			OnFixedStatusTextRequest("Saving workspace...");

			bool success = false;

			try
			{
				this.settingsHandler.Settings.AutoSaved = isAutoSave;
				this.settingsHandler.Save();
				success = true;

				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, isAutoSave));
				OnTimedStatusTextRequest("Workspace saved.");

				if (!isAutoSave)
					SetRecent(this.settingsHandler.SettingsFilePath);

				// Try to delete existing auto save file:
				if (!string.IsNullOrEmpty(autoSaveFilePathToDelete))
				{
					// Ensure that this is not the current file!
					if (!StringEx.EqualsOrdinalIgnoreCase(autoSaveFilePathToDelete, this.settingsHandler.SettingsFilePath))
						FileEx.TryDelete(autoSaveFilePathToDelete);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Error saving workspace!");

				OnFixedStatusTextRequest("Error saving workspace!");
				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to save workspace file", this.settingsHandler.SettingsFilePath, ex),
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest("Workspace not saved!");
			}

			return (success);
		}

		#endregion

		#region Close
		//==========================================================================================
		// Close
		//==========================================================================================

		/// <summary>
		/// Closes the workspace and prompts if the settings have changed.
		/// </summary>
		/// <remarks>
		/// In case of a main close, <see cref="Close(bool)"/> below must be called with
		/// the first argument set to <c>true</c>.
		/// </remarks>
		public virtual bool Close()
		{
			return (Close(false)); // See remarks above.
		}

		/// <summary>
		/// Closes the workspace and tries to auto save if desired.
		/// </summary>
		/// <remarks>
		/// Attention:
		/// This method is needed for MDI applications. In case of MDI parent/application closing,
		/// Close() of the workspace is called. Without taking care of this, the workspace would
		/// be removed as the active workspace from the local user settings. Therefore, the
		/// workspace has to signal such cases to main.
		/// 
		/// Cases (similar to cases in Model.Terminal):
		/// - Main exit
		///   - auto,   no file,       auto save    => auto save, if it fails => nothing  : (m1a)
		///   - auto,   no file,       no auto save => nothing                            : (m1b)
		///   - auto,   existing file, auto save    => auto save, if it fails => delete   : (m2a)
		///   - auto,   existing file, no auto save => delete                             : (m2b)
		///   - normal, no file                     => N/A (normal files have been saved) : (m3)
		///   - normal, no file anymore             => question                           :  --
		///   - normal, existing file, auto save    => auto save, if it fails => question : (m4a)
		///   - normal, existing file, no auto save => question                           : (m4b)
		/// - Workspace close
		///   - auto,   no file                     => nothing                            : (w1)
		///   - auto,   existing file               => delete                             : (w2)
		///   - normal, no file                     => N/A (normal files have been saved) : (w3)
		///   - normal, no file anymore             => question                           :  --
		///   - normal, existing file, auto save    => auto save, if it fails => question : (w4a)
		///   - normal, existing file, no auto save => question                           : (w4b)
		///
		/// Save and close must be done sequentially:
		/// 1. Save terminals and workspace
		/// 2. Close terminals and workspace, but only if save was successful
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public virtual bool Close(bool isMainExit)
		{
			AssertNotDisposed();

			OnFixedStatusTextRequest("Closing workspace...");

			// Keep info of existing former auto file:
			bool formerExistingAutoFileAutoSaved = this.settingsRoot.AutoSaved;
			string formerExistingAutoFilePath = null;
			if (this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFileExists)
				formerExistingAutoFilePath = this.settingsHandler.SettingsFilePath;

			// -------------------------------------------------------------------------------------
			// Evaluate save requirements for workspace and terminals.
			// -------------------------------------------------------------------------------------

			bool doSaveWorkspace = true;
			bool successWithWorkspace = false;
			bool autoSaveIsAllowedForWorkspace = ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace;

			bool doSaveTerminals = true;
			bool successWithTerminals = false;
			bool autoSaveIsAllowedForTerminals = ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace;

			// Do neither try to auto save nor manually save if there is no existing file (m1, m3)
			// or (w1, w3), except in case of m1a, i.e. when the file has never been loaded so far.
			if (autoSaveIsAllowedForWorkspace && !this.settingsHandler.SettingsFileExists)
			{
				if (!isMainExit || this.settingsHandler.SettingsFileSuccessfullyLoaded)
				{
					doSaveWorkspace = false;
					autoSaveIsAllowedForWorkspace = false;
				}
			}

			// Enforce normal save if workspace has already been normally saved. This applies to
			// workspace as well as terminals. This ensures that normal workspaces do not refer
			// to auto terminals.
			// Note that SaveAllTerminals() already ensures that terminals are not auto saved if
			// workspace file already exists but isn't auto saved.
			if (SettingsFileHasAlreadyBeenNormallySaved)
			{
				autoSaveIsAllowedForTerminals = false;
				autoSaveIsAllowedForWorkspace = false;
			}

			// -------------------------------------------------------------------------------------
			// First, save all contained terminals.
			// -------------------------------------------------------------------------------------

			if (doSaveTerminals)
			{
				if (doSaveWorkspace)
				{
					if (autoSaveIsAllowedForTerminals)
						successWithTerminals = SaveAllTerminals(true, true);
					else
						successWithTerminals = SaveAllTerminals(false, true);
				}
				else
				{
					// Save normally saved terminals even if workspace was or will not be auto saved!
					if (autoSaveIsAllowedForTerminals)
						successWithTerminals = SaveAllTerminalsWhereFileHasAlreadyBeenNormallySaved();
					else
						successWithTerminals = SaveAllTerminals(false, true);
				}
			}

			// -------------------------------------------------------------------------------------
			// Finalize save requirements for workspace.
			// -------------------------------------------------------------------------------------

			if (isMainExit)
			{
				if (!this.settingsRoot.HaveChanged)
				{
					// Nothing has changed, no need to do anything with workspace.
					doSaveWorkspace = false;
					autoSaveIsAllowedForWorkspace = false;
					successWithWorkspace = true;
				}
				else if (!this.settingsRoot.ExplicitHaveChanged)
				{
					// Implicit have changed, save is not required but try to auto save if desired.
					doSaveWorkspace = autoSaveIsAllowedForWorkspace;
				}
				else
				{
					// Explicit have changed, save is required, but only if desired.
					if (!ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace && !SettingsFileHasAlreadyBeenNormallySaved)
						doSaveWorkspace = false;
				}
			}
			else
			{
				if (!this.settingsRoot.HaveChanged)
				{
					// Nothing has changed, no need to do anything with workspace.
					doSaveWorkspace = false;
					autoSaveIsAllowedForWorkspace = false;
					successWithWorkspace = true;
				}
				else if (!this.settingsRoot.ExplicitHaveChanged)
				{
					// Implicit have changed, but do not try to auto save since user intends to close.
					doSaveWorkspace = false;
					autoSaveIsAllowedForWorkspace = false;
					successWithWorkspace = true;
				}
				else
				{
					// Explicit have changed, save is required.
				}
			}

			// -------------------------------------------------------------------------------------
			// Try auto save workspace itself, if allowed.
			// -------------------------------------------------------------------------------------

			if (successWithTerminals && !successWithWorkspace && doSaveWorkspace)
				successWithWorkspace = SaveDependentOnState(autoSaveIsAllowedForWorkspace, false); // Try auto save, i.e. no user interaction.

			// -------------------------------------------------------------------------------------
			// If not successfully saved so far, evaluate next step according to rules above.
			// -------------------------------------------------------------------------------------

			// Normal file (m3, m4, w3, w4):
			if (successWithTerminals && !successWithWorkspace && doSaveWorkspace && !this.settingsRoot.AutoSaved)
			{
				DialogResult dr = OnMessageInputRequest
				(
					"Save workspace?",
					AutoName,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				);

				switch (dr)
				{
					case DialogResult.Yes: successWithWorkspace = SaveDependentOnState(true, true); break;
					case DialogResult.No:  successWithWorkspace = true;                             break;

					default:
						successWithWorkspace = false;  break; // Also covers 'DialogResult.Cancel'.
				}
			}

			// Delete existing former auto file which is no longer needed (m2a):
			if (isMainExit && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null) && !successWithWorkspace)
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				successWithWorkspace = true;
			}

			// Delete existing former auto file which is no longer needed (w2):
			if (!isMainExit && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null))
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				successWithWorkspace = true;
			}

			// Write-protected file:
			if (!successWithWorkspace && !this.settingsHandler.SettingsFileIsWritable)
			{
				successWithWorkspace = true; // Consider it successful if file shall not be saved.
			}

			// No file (m1, w1):
			if (!successWithWorkspace && !this.settingsHandler.SettingsFileExists)
			{
				this.settingsHandler.ResetSettingsFilePath();
				successWithWorkspace = true; // Consider it successful if there was no file to save.
			}

			// -------------------------------------------------------------------------------------
			// Finally, cleanup and signal state.
			// -------------------------------------------------------------------------------------

			if (successWithTerminals && successWithWorkspace)
			{
				// Close all contained terminals signaling them a workspace close to ensure that the
				// workspace is not modified when the terminals get closed, but do not save anymore:
				if (this.settingsHandler.SettingsFileExists)
					successWithTerminals = CloseAllTerminals(true, false, false, false);
				else
					successWithTerminals = CloseAllTerminals(true, false, false, true);

				// The terminal event handlers will get detached in the 'terminal_Closed' event.
			}

			if (successWithTerminals && successWithWorkspace)
			{
				// Status text request must be before closed event, closed event may close the view:
				if (isMainExit)
					OnTimedStatusTextRequest("Workspace successfully closed, exiting.");
				else
					OnTimedStatusTextRequest("Workspace successfully closed.");

				OnClosed(new ClosedEventArgs(isMainExit));

				// The workspace shall dispose of itself to free all resources for sure. It must be
				// done AFTER it fired the 'Closed' event and all subscribers of the event may still
				// refer to a non-disposed object. This is especially important, as the order of the
				// subscribers is not fixed, i.e. Model.Main may dispose of the workspace before
				// View.Main receives the event callback!
				Dispose();

				return (true);
			}
			else
			{
				if (isMainExit)
					OnTimedStatusTextRequest("Exit cancelled, workspace not closed.");
				else
					OnTimedStatusTextRequest("Close cancelled, workspace not closed.");

				return (false);
			}
		}

		#endregion

		#region Recent Files
		//==========================================================================================
		// Recent Files
		//==========================================================================================

		/// <summary>
		/// Update recent entry.
		/// </summary>
		/// <param name="recentFile">Recent file.</param>
		private static void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(recentFile);
			ApplicationSettings.LocalUserSettings.RecentFiles.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.Save();
		}

		#endregion

		#region Layout
		//==========================================================================================
		// Layout
		//==========================================================================================

		/// <summary>
		/// Notifies the workspace about a change in the layout, so it can keep the setting. But
		/// layouting itself is done in the form as the MDI functionality is an integral part of
		/// the Windows.Forms environment.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Layouting' is a correct English term.")]
		public virtual void NotifyLayout(WorkspaceLayout layout)
		{
			this.settingsRoot.Workspace.Layout = layout;
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
			if (terminal != null)
			{
				terminal.Saved  += terminal_Saved;
				terminal.Closed += terminal_Closed;
			}
		}

		private void DetachTerminalEventHandlers(Terminal terminal)
		{
			if (terminal != null)
			{
				terminal.Saved  -= terminal_Saved;
				terminal.Closed -= terminal_Closed;
			}
		}

		private void DetachAllTerminalEventHandlers()
		{
			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
					DetachTerminalEventHandlers(t);
			}
		}

		#endregion

		#region Terminals > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminals > Event Handlers
		//------------------------------------------------------------------------------------------

		private void terminal_Saved(object sender, SavedEventArgs e)
		{
			Terminal t = (Terminal)sender;

			ReplaceTerminalInWorkspace(t);

			if (!e.IsAutoSave)
				SetRecent(e.FilePath);
		}

		/// <remarks>
		/// See remarks of <see cref="Terminal.Close(bool, bool, bool, bool)"/> for details on why
		/// this event handler needs to treat the Closed event differently in case of a parent
		/// (i.e. workspace) close.
		/// </remarks>
		private void terminal_Closed(object sender, ClosedEventArgs e)
		{
			Terminal t = (Terminal)sender;

			DetachTerminalEventHandlers(t);
			RemoveTerminalFromWorkspace(t, !e.IsParentClose); // Simply remove the terminal from the workspace, it disposes of itself.
			OnTerminalRemoved(t);
		}

		#endregion

		#region Terminals > General Properties
		//------------------------------------------------------------------------------------------
		// Terminals > General Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns settings file paths of the all the terminals in the workspace.
		/// </summary>
		public ReadOnlyCollection<string> TerminalSettingsFilePaths
		{
			get
			{
				AssertNotDisposed();

				if (this.terminals != null)
				{
					List<string> filePaths = new List<string>(this.terminals.Count); // Preset the initial capacity to improve memory management.
					foreach (Terminal t in this.terminals)
					{
						if (t.SettingsFileExists)
							filePaths.Add(t.SettingsFilePath);
					}

					return (filePaths.AsReadOnly());
				}

				return (null);
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
			AssertNotDisposed();

			// Create new terminal:
			OnFixedStatusTextRequest("Creating new terminal...");
			OnCursorRequest(Cursors.WaitCursor);

			Terminal terminal = new Terminal(this.startArgs.ToTerminalStartArgs(), settingsHandler);
			AttachTerminalEventHandlers(terminal);
			AddTerminalToWorkspace(terminal);
			OnTerminalAdded(terminal);

			OnCursorReset();
			OnTimedStatusTextRequest("New terminal created.");

			// Start terminal:
			terminal.Start(); // Errors are handled within Start().

			return (true); // Successfully created.
		}

		/// <summary>
		/// Opens terminals according to workspace settings and returns number of successfully opened terminals.
		/// </summary>
		public virtual int OpenTerminals()
		{
			return (OpenTerminals(Indices.InvalidIndex, null));
		}

		/// <summary>
		/// Opens terminals according to workspace settings and returns number of successfully opened terminals.
		/// </summary>
		public virtual int OpenTerminals(int dynamicTerminalIndexToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
		{
			AssertNotDisposed();

			int requestedTerminalCount = this.settingsRoot.TerminalSettings.Count;
			if (requestedTerminalCount == 1)
				OnFixedStatusTextRequest("Opening workspace terminal...");
			else if (requestedTerminalCount > 1)
				OnFixedStatusTextRequest("Opening workspace terminals...");

			OnCursorRequest(Cursors.WaitCursor);

			int openedTerminalCount = 0;
			GuidList<TerminalSettingsItem> clone = new GuidList<TerminalSettingsItem>(this.settingsRoot.TerminalSettings);
			for (int i = 0; i < clone.Count; i++)
			{
				TerminalSettingsItem item = clone[i];

				// \remind
				// Check whether the item is defined. Because under certain error conditions there 
				// were occasions when the item contained an empty file path and an empty GUID. That
				// has lead to an exception in an underlying System.IO call and would have lead to
				// an error message which isn't really understandable to the user. Therefore, check
				// the item and remove it if not defined.

				if (item.IsDefined)
				{
					bool success = false;
					string errorMessage = null;

					// Replace the desired terminal settings if requested:
					bool isToReplace = false;
					if (dynamicTerminalIndexToReplace == Indices.DefaultDynamicIndex)
					{
						if (i == (clone.Count - 1)) // The last terminal is the default.
							isToReplace = true;
					}
					else
					{
						if (i == Indices.DynamicIndexToIndex(dynamicTerminalIndexToReplace))
							isToReplace = true;
					}

					if (isToReplace)
					{
						Exception ex;
						if (OpenTerminalFromSettings(terminalSettingsToReplace, item.Guid, item.FixedIndex, item.Window, out ex))
						{
							openedTerminalCount++;
							success = true;
						}
						else
						{
							if (!string.IsNullOrEmpty(item.FilePath))
								errorMessage = ErrorHelper.ComposeMessage("Unable to open terminal", item.FilePath, ex);
							else
								errorMessage = ErrorHelper.ComposeMessage("Unable to open terminal!", ex);
						}
					}
					else // In all other cases, 'normally' open the terminal from the given file:
					{
						if (OpenTerminalFromFile(item.FilePath, item.Guid, item.FixedIndex, item.Window, out errorMessage))
						{                                   // Error must be handled here because of looping over terminals.
							openedTerminalCount++;
							success = true;
						}
					}

					if (!success)
					{
						this.settingsRoot.TerminalSettings.Remove(item);
						this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.

						if (string.IsNullOrEmpty(errorMessage))
						{
							if (!string.IsNullOrEmpty(item.FilePath))
								errorMessage = ErrorHelper.ComposeMessage("Unable to open terminal", item.FilePath);
							else
								errorMessage = ErrorHelper.ComposeMessage("Unable to open terminal!");
						}

						OnCursorReset();
						OnFixedStatusTextRequest("Error opening terminal!");
						DialogResult result = OnMessageInputRequest
						(
							errorMessage + Environment.NewLine + Environment.NewLine + "Continue loading workspace?",
							"Terminal Error",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Exclamation
						);
						OnTimedStatusTextRequest("Terminal not opened!");
						OnCursorRequest(Cursors.WaitCursor);

						if (result == DialogResult.No)
						{
							// Remove all remaining items:
							int remainingCount = (clone.Count - (i + 1));
							int remainingStartIndex = (this.settingsRoot.TerminalSettings.Count - remainingCount);
							this.settingsRoot.TerminalSettings.RemoveRange(remainingStartIndex, remainingCount);
							this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.

							// Break looping over terminals:
							break;
						}
					}
				}
				else
				{
					this.settingsRoot.TerminalSettings.Remove(item);
					this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.
				}
			} // for (each terminal in workspace)

			OnCursorReset();

			// On success, clear changed flag since all terminals got openend.
			if (openedTerminalCount == requestedTerminalCount)
				this.settingsRoot.ClearChanged();

			return (openedTerminalCount);
		}

		/// <summary>
		/// Opens the terminal file given.
		/// </summary>
		/// <param name="filePath">Terminal file.</param>
		/// <returns><c>true</c> if successfully opened the terminal, <c>false</c> otherwise.</returns>
		public virtual bool OpenTerminalFromFile(string filePath)
		{
			AssertNotDisposed();

			string fileName = Path.GetFileName(filePath);
			OnFixedStatusTextRequest("Opening terminal " + fileName + "...");
			OnCursorRequest(Cursors.WaitCursor);

			string errorMessage;
			if (OpenTerminalFromFile(filePath, Guid.Empty, Indices.DefaultFixedIndex, null, out errorMessage))
			{
				OnCursorReset();
				return (true);
			}
			else
			{
				OnCursorReset();
				OnFixedStatusTextRequest("Error opening terminal!");
				OnMessageInputRequest
				(
					errorMessage,
					"Terminal File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
				);
				OnTimedStatusTextRequest("Terminal not opened!");

				return (false);
			}
		}

		private bool OpenTerminalFromFile(string filePath, Guid guid, int fixedIndex, WindowSettings windowSettings, out string errorMessage)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> sh;
			Exception ex;
			if (OpenTerminalFile(filePath, out sh, out ex))
			{
				if (OpenTerminalFromSettings(sh, guid, fixedIndex, windowSettings, out ex))
				{
					errorMessage = null;
					return (true);
				}
				else
				{
					errorMessage = ErrorHelper.ComposeMessage("Unable to open terminal", filePath, ex);
					return (false);
				}
			}
			else
			{
				errorMessage = ErrorHelper.ComposeMessage("Unable to open terminal file", filePath, ex);
				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool OpenTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			Exception exception;
			return (OpenTerminalFromSettings(settingsHandler, Guid.Empty, Indices.DefaultFixedIndex, null, out exception));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure to handle any case.")]
		private bool OpenTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid, int fixedIndex, WindowSettings windowSettings, out Exception exception)
		{
			AssertNotDisposed();

			// Ensure that the terminal file is not already open:
			if (!CheckTerminalFiles(settingsHandler.SettingsFilePath))
			{
				exception = null;
				return (false);
			}

			OnFixedStatusTextRequest("Opening terminal...");

			// Set window settings if there are:
			if (windowSettings != null)
				settingsHandler.Settings.Window = windowSettings;

			// Create terminal:
			Terminal terminal;
			try
			{
				terminal = new Terminal(this.startArgs.ToTerminalStartArgs(), settingsHandler, guid);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to create/open terminal from settings!");
				exception = ex;
				return (false);
			}

			AttachTerminalEventHandlers(terminal);
			AddTerminalToWorkspace(terminal, fixedIndex);
			OnTerminalAdded(terminal);

			if (!settingsHandler.Settings.AutoSaved)
				SetRecent(settingsHandler.SettingsFilePath);

			OnTimedStatusTextRequest("Terminal opened.");

			exception = null;
			return (true);
		}

		/// <summary></summary>
		public virtual bool StartAllTerminals()
		{
			bool success = true;

			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
				{
					if (!t.Start())
						success = false;
				}
			}

			return (success);
		}

		#endregion

		#region Terminal > Private Methods
		//------------------------------------------------------------------------------------------
		// Terminal > Private Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Check whether terminal is already contained in workspace.
		/// </summary>
		private bool CheckTerminalFiles(string terminalFilePath)
		{
			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
				{
					if (PathEx.Equals(terminalFilePath, t.SettingsFilePath))
					{
						OnFixedStatusTextRequest("Terminal is already open.");
						OnMessageInputRequest
						(
							"Terminal is already open and will not be re-openend.",
							"Terminal Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
						OnTimedStatusTextRequest("Terminal not re-opened.");
						return (false);
					}
				}
			}
			return (true);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure to handle any case.")]
		private bool OpenTerminalFile(string terminalFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exception)
		{
			// Combine absolute workspace path with terminal path if that one is relative:
			terminalFilePath = PathEx.CombineFilePaths(this.settingsHandler.SettingsFilePath, terminalFilePath);

			try
			{
				DocumentSettingsHandler<TerminalSettingsRoot> sh = new DocumentSettingsHandler<TerminalSettingsRoot>();
				sh.SettingsFilePath = terminalFilePath;
				if (sh.Load())
				{
					settingsHandler = sh;
					exception = null;
					return (true);
				}
				else
				{
					settingsHandler = null;
					exception = null;
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open terminal file!");

				settingsHandler = null;
				exception = ex;
				return (false);
			}
		}

		private TerminalSettingsItem CreateTerminalSettingsItem(Terminal terminal, int fixedIndex)
		{
			TerminalSettingsItem tsi = new TerminalSettingsItem();

			string filePath = terminal.SettingsFilePath;
			if (ApplicationSettings.LocalUserSettings.General.UseRelativePaths)
			{
				PathCompareResult pcr = PathEx.CompareFilePaths(this.settingsHandler.SettingsFilePath, terminal.SettingsFilePath);
				if (pcr.AreRelative)
					filePath = pcr.RelativePath;
			}

			tsi.Guid = terminal.Guid;
			tsi.FilePath = filePath;

			if (fixedIndex >= Indices.FirstFixedIndex)
				tsi.FixedIndex = fixedIndex;

			tsi.Window = new WindowSettings(terminal.SettingsRoot.Window); // Clone window settings.

			return (tsi);
		}

		private void AddTerminalToWorkspace(Terminal terminal, int requestedFixedIndex = Indices.DefaultFixedIndex)
		{
			// Add terminal to terminal list:
			this.terminals.Add(terminal);
			this.activeTerminal = terminal;
			int effectiveIndex = AddTerminalToFixedIndices(terminal, requestedFixedIndex);

			// Add terminal settings for new terminals:
			// Replace terminal settings if workspace settings have been loaded from file prior.
			this.settingsRoot.TerminalSettings.AddOrReplace(CreateTerminalSettingsItem(terminal, effectiveIndex));
			this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.
		}

		private void ReplaceTerminalInWorkspace(Terminal terminal)
		{
			// Replace terminal in terminal list:
			this.terminals.Replace(terminal);
			this.activeTerminal = terminal;

			// Replace terminal in workspace settings if the settings have indeed changed:
			TerminalSettingsItem tsiNew = CreateTerminalSettingsItem(terminal, GetFixedIndexByTerminal(terminal));
			TerminalSettingsItem tsiOld = this.settingsRoot.TerminalSettings.Find(terminal.Guid);
			if ((tsiOld == null) || (tsiNew != tsiOld))
			{
				this.settingsRoot.TerminalSettings.Replace(tsiNew);
				this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.
			}
		}

		private void RemoveTerminalFromWorkspace(Terminal terminal, bool doNotRemoveFromSettings)
		{
			// Remove terminal from workspace settings:
			if (doNotRemoveFromSettings)
			{
				this.settingsRoot.TerminalSettings.Remove(terminal.Guid);
				this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.
			}

			// Remove terminal from workspace lists:
			this.terminals.Remove(terminal.Guid);
			RemoveTerminalFromFixedIndices(terminal);
			DeactivateTerminal(terminal);
		}

		private int AddTerminalToFixedIndices(Terminal terminal, int requestedFixedIndex)
		{
			// First, try to lookup the requested fixed index if suitable:
			if (requestedFixedIndex >= Indices.FirstFixedIndex)
			{
				if (!this.fixedIndices.ContainsKey(requestedFixedIndex))
				{
					this.fixedIndices.Add(requestedFixedIndex, terminal);
					return (requestedFixedIndex);
				}
			}

			// As fallback, use the next available fixed index:
			for (int i = Indices.FirstFixedIndex; i <= int.MaxValue; i++)
			{
				if (!this.fixedIndices.ContainsKey(i))
				{
					this.fixedIndices.Add(i, terminal);
					return (i);
				}
			}

			// If both fail, no good! It means that there are more than 2'000'000'000 terminals ;-)
			throw (new OverflowException("Constant index of terminals exceeded!"));
		}

		private void RemoveTerminalFromFixedIndices(Terminal terminal)
		{
			this.fixedIndices.Remove(GetFixedIndexByTerminal(terminal));
		}

		#endregion

		#region Terminals > Get Methods
		//------------------------------------------------------------------------------------------
		// Terminals > Get Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the fixed index of the given terminal.
		/// </summary>
		public virtual int GetFixedIndexByTerminal(Terminal terminal)
		{
			foreach (KeyValuePair<int, Terminal> kvp in this.fixedIndices)
			{
				if (kvp.Value == terminal)
					return (kvp.Key);
			}

			throw (new ArgumentOutOfRangeException("terminal", terminal, "Terminal not found in index table!"));
		}

		/// <summary>
		/// Returns the dynamic index of the given terminal.
		/// </summary>
		public virtual int GetDynamicIndexByTerminal(Terminal terminal)
		{
			AssertNotDisposed();

			int index = this.terminals.IndexOf(this.activeTerminal);
			return (Indices.IndexToDynamicIndex(index));
		}

		/// <summary>
		/// Returns the terminal with the given GUID. If no terminal with this GUID exists,
		/// <c>null</c> is returned.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public virtual Terminal GetTerminalByGuid(Guid guid)
		{
			AssertNotDisposed();

			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
				{
					if (t.Guid == guid)
						return (t);
				}
			}

			return (null);
		}

		/// <summary>
		/// Returns the terminal with the given sequential index. The sequential index relates to the
		/// number indicated in the terminal name, e.g. "Terminal1" or "Terminal2". The sequential
		/// index starts at 1 and is unique throughout the execution of the program. If no terminal
		/// with this index exists, <c>null</c> is returned.
		/// </summary>
		public virtual Terminal GetTerminalBySequentialIndex(int sequentialIndex)
		{
			AssertNotDisposed();

			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
				{
					if (t.SequentialIndex == sequentialIndex)
						return (t);
				}
			}

			return (null);
		}

		/// <summary>
		/// Returns the terminal with the given dynamic index. The dynamic index represents
		/// the order in which the terminals were created. If a terminal is closed, the dynamic
		/// index of all latter terminals is adjusted. If no terminal with this index exists,
		/// <c>null</c> is returned.
		/// </summary>
		/// <remarks>
		/// The index must be in the range of 1...NumberOfTerminals, or 0 to return the currently
		/// active terminal.
		/// </remarks>
		public virtual Terminal GetTerminalByDynamicIndex(int dynamicIndex)
		{
			AssertNotDisposed();

			if (dynamicIndex == Indices.DefaultDynamicIndex)
			{
				return (this.activeTerminal);
			}
			else
			{
				int index = Indices.DynamicIndexToIndex(dynamicIndex);
				if ((index >= 0) && (index < this.terminals.Count))
					return (this.terminals[index]);
				else
					return (null);
			}
		}

		/// <summary>
		/// Returns the terminal with the given fixed index. The fixed index represents the
		/// order in which the terminals initially were created but doesn't change throughout the
		/// execution of the program. If a terminal is closed, the corresponding index becomes
		/// available and will be used for the next terminal that is opened, i.e. a new terminal
		/// always gets the lowest available fixed index. If no terminal with this index exists,
		/// <c>null</c> is returned.
		/// </summary>
		public virtual Terminal GetTerminalByFixedIndex(int index)
		{
			AssertNotDisposed();

			Terminal t;
			if (this.fixedIndices.TryGetValue(index, out t))
				return (t);
			else
				return (null);
		}

		/// <summary>
		/// Returns the terminal with the given user name. The user name can freely be chosen in
		/// the terminal settings. There are no restrictions on the name. If no terminal with this
		/// name exists, <c>null</c> is returned.
		/// </summary>
		public virtual Terminal GetTerminalByUserName(string userName)
		{
			AssertNotDisposed();

			foreach (Terminal t in this.terminals)
			{
				if (t.SettingsRoot.UserName == userName)
					return (t);
			}

			foreach (Terminal t in this.terminals)
			{
				if (StringEx.EqualsOrdinalIgnoreCase(t.SettingsRoot.UserName, userName))
					return (t);
			}

			return (null);
		}

		#endregion

		#region Terminals > All Terminals Methods
		//------------------------------------------------------------------------------------------
		// Terminals > All Terminals Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool SaveAllTerminals()
		{
			AssertNotDisposed();

			return (SaveAllTerminals(true, true));
		}

		/// <summary></summary>
		private bool SaveAllTerminals(bool autoSaveIsAllowed, bool userInteractionIsAllowed)
		{
			bool success = true;

			if (autoSaveIsAllowed)
				autoSaveIsAllowed = EvaluateWhetherAutoSaveIsAllowedIndeed(autoSaveIsAllowed);

			// Calling Save() on a terminal may modify 'this.terminals' in the terminal_Saved()
			// event, therefore clone the list first.
			List<Terminal> clone = new List<Terminal>(this.terminals);
			foreach (Terminal t in clone)
			{
				bool saveIsRequiredForThisTerminal = true;
				if (!EvaluateWhetherSaveIsFeasibleForThisTerminal(t))
					saveIsRequiredForThisTerminal = EvaluateWhetherSaveIsRequiredForThisTerminal(t);

				if (saveIsRequiredForThisTerminal)
				{
					if (!t.Save(autoSaveIsAllowed, userInteractionIsAllowed))
						success = false;
				}

				// Consider it successful if save is not required.
			}

			return (success);
		}

		/// <summary></summary>
		private static bool EvaluateWhetherSaveIsFeasibleForThisTerminal(Terminal t)
		{
			// Save is not feasible for write-protected files:
			if (t.SettingsFileHasAlreadyBeenNormallySaved && !t.SettingsFileIsWritable)
				return (false);

			// Save is not feasible for files which no longer exist:
			if (t.SettingsFileHasAlreadyBeenNormallySaved && !t.SettingsFileExists)
				return (false);

			return (true);
		}

		/// <summary></summary>
		private static bool EvaluateWhetherSaveIsRequiredForThisTerminal(Terminal t)
		{
			// Save is not required for write-protected files...
			// ...with no or only implicit changes would have to be saved:
			if (t.SettingsFileHasAlreadyBeenNormallySaved && !t.SettingsFileIsWritable)
				return (false);

			// Save is not required if file does no longer exist...
			// ...and no or only implicit changes would have to be saved:
			if (t.SettingsFileHasAlreadyBeenNormallySaved && !t.SettingsFileExists)
				return (t.SettingsRoot.ExplicitHaveChanged);

			return (true);
		}

		/// <summary></summary>
		private bool SaveAllTerminalsWhereFileHasAlreadyBeenNormallySaved()
		{
			bool success = true;

			// Calling Save() on a terminal may modify 'this.terminals' in the terminal_Saved()
			// event, therefore clone the list first.
			List<Terminal> clone = new List<Terminal>(this.terminals);
			foreach (Terminal t in clone)
			{
				if (t.SettingsFileHasAlreadyBeenNormallySaved)
				{
					if (!t.Save(false, true))
						success = false;
				}
			}

			return (success);
		}

		/// <remarks>
		/// In case of a workspace close, <see cref="CloseAllTerminals(bool, bool, bool, bool)"/> below
		/// must be called with the first argument set to <c>true</c>.
		/// 
		/// In case of intended close of one or all terminals, the user intentionally wants to close
		/// the terminals, thus, this method will not try to auto save.
		/// </remarks>
		public virtual bool CloseAllTerminals()
		{
			AssertNotDisposed();

			return (CloseAllTerminals(false, true, false, true)); // See remarks above.
		}

		/// <remarks>
		/// See remarks of <see cref="Terminal.Close(bool, bool, bool, bool)"/> for details on 'WorkspaceClose'.
		/// </remarks>
		private bool CloseAllTerminals(bool isWorkspaceClose, bool doSave, bool autoSaveIsAllowed, bool autoDeleteIsRequested)
		{
			bool success = true;

			if (doSave && autoSaveIsAllowed)
				autoSaveIsAllowed = EvaluateWhetherAutoSaveIsAllowedIndeed(autoSaveIsAllowed);
			else
				autoSaveIsAllowed = false;

			// Calling Close() on a terminal will modify 'this.terminals' in the terminal_Closed()
			// event, therefore clone the list first.
			List<Terminal> clone = new List<Terminal>(this.terminals);
			foreach (Terminal t in clone)
			{
				if (!t.Close(isWorkspaceClose, doSave, autoSaveIsAllowed, autoDeleteIsRequested))
					success = false;
			}

			return (success);
		}

		/// <summary></summary>
		protected virtual bool EvaluateWhetherAutoSaveIsAllowedIndeed(bool autoSaveIsAllowed)
		{
			AssertNotDisposed();

			// Do not auto save if workspace file already exists but isn't auto saved.
			// Ensures that normal workspaces do not refer to auto terminals.
			if (autoSaveIsAllowed && this.settingsHandler.SettingsFileSuccessfullyLoaded && !this.settingsRoot.AutoSaved)
				return (false);

			return (autoSaveIsAllowed);
		}

		/// <summary></summary>
		public virtual bool AllLogOn()
		{
			AssertNotDisposed();

			bool success = true;

			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
				{
					if (!t.SwitchLogOn())
						success = false;
				}
			}

			return (success);
		}

		/// <summary></summary>
		public virtual bool AllLogOff()
		{
			AssertNotDisposed();

			bool success = true;

			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
				{
					if (!t.SwitchLogOff())
						success = false;
				}
			}

			return (success);
		}

		/// <summary></summary>
		public virtual bool AllLogClear()
		{
			AssertNotDisposed();

			bool success = true;

			if (this.terminals != null)
			{
				foreach (Terminal t in this.terminals)
				{
					if (!t.ClearLog())
						success = false;
				}
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
			AssertNotDisposed();

			this.activeTerminal = terminal;
		}

		/// <summary></summary>
		public virtual void ActivateTerminalBySequentialIndex(int index)
		{
			ActivateTerminal(GetTerminalBySequentialIndex(index));
		}

		/// <summary></summary>
		public virtual void DeactivateTerminal(Terminal terminal)
		{
			AssertNotDisposed();

			if (this.activeTerminal == terminal)
				this.activeTerminal = null;
		}

		/// <summary></summary>
		public virtual bool SaveActiveTerminal()
		{
			AssertNotDisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.Save());
			else
				return (false);
		}

		/// <summary></summary>
		public virtual bool CloseActiveTerminal()
		{
			AssertNotDisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.Close());
			else
				return (false);
		}

		/// <summary></summary>
		public virtual bool OpenActiveTerminalIO()
		{
			AssertNotDisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.StartIO());
			else
				return (false);
		}

		/// <summary></summary>
		public virtual bool CloseActiveTerminalIO()
		{
			AssertNotDisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.StopIO());
			else
				return (false);
		}

		#endregion

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTerminalAdded(Terminal terminal)
		{
			EventHelper.FireSync<EventArgs<Terminal>>(TerminalAdded, this, new EventArgs<Terminal>(terminal));
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTerminalRemoved(Terminal terminal)
		{
			EventHelper.FireSync<EventArgs<Terminal>>(TerminalRemoved, this, new EventArgs<Terminal>(terminal));
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			DebugMessage(text);
			EventHelper.FireSync<EventArgs<string>>(FixedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			DebugMessage(text);
			EventHelper.FireSync<EventArgs<string>>(TimedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			if (this.startArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				MessageInputEventArgs e = new MessageInputEventArgs(text, caption, buttons, icon);
				EventHelper.FireSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				// Ensure that the request is processed!
				if (e.Result == DialogResult.None)
					throw (new InvalidOperationException("A 'Message Input' request by the workspace was not processed by the application!"));

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <summary></summary>
		protected virtual DialogResult OnSaveAsFileDialogRequest()
		{
			if (this.startArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				DialogEventArgs e = new DialogEventArgs();
				EventHelper.FireSync<DialogEventArgs>(SaveAsFileDialogRequest, this, e);

				// Ensure that the request is processed!
				if (e.Result == DialogResult.None)
					throw (new InvalidOperationException("A 'Save As' request by the workspace was not processed by the application!"));

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnCursorRequest(Cursor cursor)
		{
			EventHelper.FireSync<EventArgs<Cursor>>(CursorRequest, this, new EventArgs<Cursor>(cursor));
		}

		/// <summary></summary>
		protected virtual void OnCursorReset()
		{
			OnCursorRequest(Cursors.Default);
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					"",
					"[" + Guid + "]",
					message
				)
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
