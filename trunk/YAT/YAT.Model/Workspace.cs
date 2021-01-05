//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
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
using MKY.Windows.Forms;

using YAT.Application.Settings;
using YAT.Application.Utilities;
using YAT.Model.Settings;
using YAT.Model.Types;
//// 'YAT.Model.Utilities' is explicitly used due to ambiguity of 'MessageHelper'.
using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Workspaces (.yaw) of the YAT application model.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Workspace : DisposableBase, IGuidProvider
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

		/// <summary>
		/// Workaround to the following issue:
		///
		/// A test (e.g. 'FileHandlingTest') needs to verify the settings files after calling
		/// <see cref="Main.Exit_ForTestOnly()"/>. But at that moment, the settings have already
		/// been disposed of and can no longer be accessed.
		/// The first approach was to disable disposal in <see cref="Close()"/>. But that leads to
		/// remaining resources, resulting in significant slow-down when exiting NUnit.
		/// The second approach was to retrieve the required information *before* exiting, i.e.
		/// calling <see cref="Main.Exit_ForTestOnly()"/>. But that doesn't work at all, since
		/// auto-save paths are only evaluated *at* <see cref="Main.Exit_ForTestOnly()"/>.
		///
		/// This workaround is considered the best option to solve this issue.
		/// </summary>
		/// <remarks>
		/// Note that it is not possible to mark a property with [Conditional("TEST")].
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize purpose of this property.")]
		public bool DoNotDetachSettingsBecauseTheyAreRequiredForVerification_ForTestOnly { get; set; }

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Workspace).FullName);

		private WorkspaceLaunchArgs launchArgs;
		private Guid guid;
		private string userFileName;

		// Settings.
		private DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler;
		private WorkspaceSettingsRoot settingsRoot;

		// Terminal list.
		private GuidList<Terminal> terminals = new GuidList<Terminal>();
		private Terminal activeTerminal; // = null;
		private Dictionary<int, Terminal> fixedIds = new Dictionary<int, Terminal>();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>Event raised when a new terminal was added to the workspace.</summary>
		public event EventHandler<TerminalEventArgs> TerminalAdded;

		/// <summary>Event raised when a terminal was removed from the workspace.</summary>
		public event EventHandler<TerminalEventArgs> TerminalRemoved;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<Cursor>> CursorRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<DialogEventArgs> SaveAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<FilePathDialogEventArgs> SaveCommandPageAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<FilePathDialogEventArgs> OpenCommandPageFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<SavedEventArgs> Saved;

		/// <summary></summary>
		public event EventHandler<ClosedEventArgs> Closed;

		/// <summary></summary>
		public event EventHandler ExitRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Workspace(DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler)
			: this(new WorkspaceLaunchArgs(), settingsHandler, Guid.Empty)
		{
		}

		/// <summary></summary>
		public Workspace(WorkspaceLaunchArgs launchArgs)
			: this(launchArgs, new DocumentSettingsHandler<WorkspaceSettingsRoot>(), Guid.Empty)
		{
		}

		/// <remarks><see cref="Guid.Empty"/> cannot be used as default argument as it is read-only.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public Workspace(WorkspaceLaunchArgs launchArgs, DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler, Guid guid)
		{
			try
			{
				DebugMessage("Creating...");

				this.launchArgs = launchArgs;

				if (guid != Guid.Empty)
					this.guid = guid;
				else
					this.guid = Guid.NewGuid();

				// Link and attach to settings:
				this.settingsHandler = settingsHandler;
				this.settingsRoot = this.settingsHandler.Settings;
				this.settingsRoot.ClearChanged();
				AttachSettingsEventHandlers();

				// Set ID and name(s):
				if (!this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFilePathIsValid)
					this.userFileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

				DebugMessage("...successfully created.");
			}
			catch (Exception ex)
			{
				DebugMessage("...failed!");
				DebugEx.WriteException(GetType(), ex);

				Dispose(); // Immediately call Dispose() to ensure no zombies remain!
				throw; // Rethrow!
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				DebugMessage("Disposing...");

				// In the 'normal' case, the terminals have already been closed, otherwise...

				// ...detach event handlers to ensure that no more events are received...
				DetachAllTerminalEventHandlers();

				// ...dispose of terminals (normally they dispose of themselves)...
				if (this.terminals != null)
				{
					foreach (var t in this.terminals)
						t.Dispose();

					this.terminals.Clear();
					this.terminals = null;
				}

				// ...and finally detach the settings:
				if (!DoNotDetachSettingsBecauseTheyAreRequiredForVerification_ForTestOnly)
				{
					DetachSettingsEventHandlers();
					DetachSettingsHandler();
				}

				DebugMessage("...successfully disposed.");
			}

		////base.Dispose(disposing) of 'DisposableBase' doesn't need and cannot be called since abstract.
		}

		#endregion

		#endregion

		#region General
		//==========================================================================================
		// General
		//==========================================================================================

		/// <summary></summary>
		public virtual WorkspaceLaunchArgs LaunchArgs
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.launchArgs);
			}
		}

		/// <summary></summary>
		public virtual Guid Guid
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.guid);
			}
		}

		/// <summary>
		/// The file name if the user has saved the terminal; otherwise <see cref="string.Empty"/>.
		/// </summary>
		/// <remarks>
		/// Cached from <see cref="SettingsFilePath"/> for...
		/// ...limiting to user files (i.e. not 'AutoSaved').
		/// ...having to compose the name only once.
		/// </remarks>
		public virtual string UserFileName
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (!string.IsNullOrEmpty(this.userFileName))
					return (this.userFileName);

				return ("");
			}
		}

		/// <summary>
		/// The indicated name, i.e. either the <see cref="UserFileName"/> or <see cref="string.Empty"/>.
		/// </summary>
		public virtual string IndicatedName
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (!string.IsNullOrEmpty(UserFileName))
					return (UserFileName);

				return ("");
			}
		}

		/// <summary></summary>
		public virtual string Caption
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Utilities.CaptionHelper.ComposeMain(IndicatedName));
			}
		}

		/// <summary></summary>
		public virtual string SettingsFilePath
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsReadOnly
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileIsReadOnly);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsWritable
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.terminals != null)
					return (this.terminals.Count);
				else
					return (0);
			}
		}

		/// <summary>
		/// Returns an array of all terminals within workspace; or <c>null</c> if there are no terminals.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required for testing only.")]
		public virtual Terminal[] Terminals
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.terminals != null)
					return (this.terminals.ToArray());
				else
					return (null);
			}
		}

		/// <summary>
		/// Returns active terminal within workspace; or <c>null</c> if no terminal is active.
		/// </summary>
		public virtual Terminal ActiveTerminal
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.activeTerminal);
			}
		}

		/// <summary>
		/// Returns the sequential ID of the active terminal.
		/// </summary>
		public virtual int ActiveTerminalSequentialId
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.activeTerminal != null)
					return (this.activeTerminal.SequentialId);
				else
					return (TerminalIds.InvalidSequentialId);
			}
		}

		/// <summary>
		/// Returns the dynamic ID of the active terminal.
		/// </summary>
		public virtual int ActiveTerminalDynamicId
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.activeTerminal != null)
					return (GetDynamicIdByTerminal(this.activeTerminal));
				else
					return (TerminalIds.InvalidDynamicId);
			}
		}

		/// <summary>
		/// Returns the fixed ID of the active terminal.
		/// </summary>
		public virtual int ActiveTerminalFixedId
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.activeTerminal != null)
					return (GetFixedIdByTerminal(this.activeTerminal));
				else
					return (TerminalIds.InvalidFixedId);
			}
		}

		/// <summary>
		/// Gets a text containing information about the active terminal.
		/// </summary>
		/// <remarks>
		/// Using term 'Info' since the info contains name and IDs.
		/// </remarks>
		public virtual string ActiveTerminalInfoText
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (ActiveTerminal != null)
				{
					var sb = new StringBuilder();

					// Attention:
					// Same "[IndicatedName] - Info - Info - Info" as in...
					// ...CaptionHelper.Compose().
					// Changes here may have to be applied there too.

					sb.Append("[");
					sb.Append(ActiveTerminal.IndicatedName);
					sb.Append("] - Seq#");
					sb.Append(ActiveTerminalSequentialId);
					sb.Append(" - Dyn#");
					sb.Append(ActiveTerminalDynamicId);
					sb.Append(" - Fix#");
					sb.Append(ActiveTerminalFixedId);
				#if (WITH_SCRIPTING)
					sb.Append(" (Connection.TerminalId)");
				#endif

					return (sb.ToString());

					// Note:
					// It has been considered to include the fix ID used for scripting in the
					// terminal caption. The idea has been discarded for various reasons:
					//  > The user interface shall not be overloaded more than necessary.
					//  > The user interface shall always behave the same, not only sometimes
					//    include something, i.e. only when scripting is active.
					//  > Most users will only deal with a single terminal, always being Fix#1.
					//  > Most users won't need the terminal ID as they can use the implicit
					//    methods, i.e. implicitly use 'ITerminalEx.ActiveTerminalId'.
					//  > Advanced users can still activate this 'ActiveTerminalInfoText'.
					//  > The terminal doesn't know its fix ID, composing the caption gets tricky.
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
			{
				this.settingsRoot.Changed += settingsRoot_Changed;
			}
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
			{
				this.settingsRoot.Changed -= settingsRoot_Changed;
			}
		}

		private void DetachSettingsHandler()
		{
			if (this.settingsHandler != null)
			{
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
						TryAutoSaveIfAlreadyAutoSaved();
				}
				finally
				{
					Monitor.Exit(settingsRoot_Changed_SyncObj);
				}
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("settingsRoot_Changed() monitor has timed out, skipping this recursive event.");
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
			////AssertUndisposed() shall not be called from this simple get-property.

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
				// Attention:
				// AssertUndisposed() must not be called to still allow reading the settings after
				// the workspace has been disposed. This is required for certain test cases.

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
		/// Tries to performs auto save.
		/// </summary>
		protected virtual bool TryAutoSaveIfAlreadyAutoSaved()
		{
			bool success = false;

			if (this.settingsHandler.SettingsFileSuccessfullyLoaded && this.settingsRoot.AutoSaved)
				success = TrySaveWithOptionsWithoutUserInteraction(true);

			return (success);
		}

		/// <summary>
		/// Saves all terminals and workspace to files, prompts for files if they don't exist yet.
		/// </summary>
		public virtual bool Save()
		{
			AssertUndisposed();

			// First, save all contained terminals:
			if (!SaveAllTerminalsNormally(true))
				return (false);

			// Then, save the workspace itself:
			bool isCanceled;                       // Save even if not changed since explicitly requesting saving.
			return (SaveWithOptions(true, true, true, false, out isCanceled));
		}

		/// <summary>
		/// Silently tries to save terminal to file, i.e. without any user interaction.
		/// </summary>
		public virtual bool TrySaveWithOptionsWithoutUserInteraction(bool autoSaveIsAllowed)
		{
			bool isCanceled;
			return (SaveWithOptions(autoSaveIsAllowed, false, false, false, out isCanceled));
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
		/// <param name="saveEvenIfNotChanged">Indicates whether save must happen even if not changed.</param>
		/// <param name="canBeCanceled">Indicates whether save can be canceled.</param>
		/// <param name="isCanceled">Indicates whether save has been canceled.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool SaveWithOptions(bool autoSaveIsAllowed, bool userInteractionIsAllowed, bool saveEvenIfNotChanged, bool canBeCanceled, out bool isCanceled)
		{
			AssertUndisposed();

			isCanceled = false;

			autoSaveIsAllowed = EvaluateWhetherAutoSaveIsAllowedIndeed(autoSaveIsAllowed);

			// -------------------------------------------------------------------------------------
			// Skip save if there is no reason to save:
			// -------------------------------------------------------------------------------------

			if (ThereIsNoReasonToSave(autoSaveIsAllowed, saveEvenIfNotChanged))
				return (true);

			// -------------------------------------------------------------------------------------
			// Create auto save file path or request manual/normal file path if necessary:
			// -------------------------------------------------------------------------------------

			if (!this.settingsHandler.SettingsFilePathIsValid)
			{
				if (autoSaveIsAllowed) {
					// Set the new file path...
					this.settingsHandler.SettingsFilePath = ComposeAbsoluteAutoSaveFilePath();

					// ...adjust the potentially relative terminal files paths to the new absolute workspace file path...
					foreach (var t in this.terminals)
						ReplaceTerminalInWorkspaceSettings(t);

					// ...and further down, save the workspace itself.
				}
				else if (userInteractionIsAllowed) {
					return (RequestNormalSaveAsFromUser());
				}
				else {
					return (false); // Let save fail if the file path is invalid and no user interaction is allowed
				}
			}
			else if (this.settingsRoot.AutoSaved && !autoSaveIsAllowed)
			{
				// Ensure that former auto files are 'Saved As' if auto save is no longer allowed:
				if (userInteractionIsAllowed) {
					return (RequestNormalSaveAsFromUser());
				}
				else {
					return (false);
				}
			}

			// -------------------------------------------------------------------------------------
			// Handle write-protected or non-existant file:
			// -------------------------------------------------------------------------------------

			if (!SettingsFileIsWritable || SettingsFileNoLongerExists)
			{
				if (this.settingsRoot.ExplicitHaveChanged || saveEvenIfNotChanged)
				{
					if (userInteractionIsAllowed) {
						return (RequestRestrictedSaveAsFromUser(canBeCanceled, out isCanceled));
					}
					else {
						return (false); // Let save of explicit change fail if file is restricted.
					}
				}
				else // ImplicitHaveChanged:
				{
					return (true); // Skip save of implicit change as save is currently not feasible.
				}
			}

			// -------------------------------------------------------------------------------------
			// Save is feasible:
			// -------------------------------------------------------------------------------------

			return (SaveToFile(autoSaveIsAllowed, null));
		}

		/// <summary></summary>
		protected virtual bool EvaluateWhetherAutoSaveIsAllowedIndeed(bool autoSaveIsAllowed)
		{
			// Do not auto save if file already exists but isn't auto saved:
			if (autoSaveIsAllowed && this.settingsHandler.SettingsFilePathIsValid && !this.settingsRoot.AutoSaved)
				return (false);

			return (autoSaveIsAllowed);
		}

		/// <summary></summary>
		protected virtual bool ThereIsNoReasonToSave(bool autoSaveIsAllowed, bool saveEvenIfNotChanged)
		{
			if (saveEvenIfNotChanged)
				return (false);

			// Ensure that former auto files are 'Saved As' if auto save is no longer allowed:
			if (this.settingsRoot.AutoSaved && !autoSaveIsAllowed)
				return (false);

			// No need to save if settings are up to date:
			return (this.settingsHandler.SettingsFileIsUpToDate && !this.settingsRoot.HaveChanged);
		}

		/// <summary></summary>
		protected virtual string ComposeAbsoluteAutoSaveFilePath()
		{
			var sb = new StringBuilder();

			sb.Append(Application.Settings.GeneralSettings.AutoSaveRoot);
			sb.Append(Path.DirectorySeparatorChar);
			sb.Append(Application.Settings.GeneralSettings.AutoSaveWorkspaceFileNamePrefix);
			sb.Append(Guid.ToString());
			sb.Append(ExtensionHelper.WorkspaceExtension);

			return (sb.ToString());
		}

		/// <summary></summary>
		protected virtual bool RequestNormalSaveAsFromUser()
		{
			switch (OnSaveAsFileDialogRequest()) // 'Save File Dialog' offers [OK] and [Cancel].
			{
				case DialogResult.OK:
					return (true);

				case DialogResult.Cancel:
				default:
					return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool RequestRestrictedSaveAsFromUser(bool canBeCanceled, out bool isCanceled)
		{
			string reason;
			if      ( SettingsFileNoLongerExists) // Shall be checked first, as that is first thing to verify.
				reason = "The file no longer exists.";
			else if (!SettingsFileIsWritable)
				reason = "The file is write-protected.";
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'SaveAs'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			var message = new StringBuilder();
			message.AppendLine("Unable to save");
			message.AppendLine(this.settingsHandler.SettingsFilePath);
			message.AppendLine();
			message.Append(reason + " Would you like to save the file at another location? You may also fix the file and then confirm the current location.");

			var dr = OnMessageInputRequest
			(
				message.ToString(),
				"File Error",
				(canBeCanceled ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo),
				MessageBoxIcon.Question
			);

			switch (dr)
			{
				case DialogResult.Yes:
					isCanceled = false;
					return (RequestNormalSaveAsFromUser());

				case DialogResult.No:
					OnTimedStatusTextRequest("Workspace not saved!");
					isCanceled = false;
					return (true);

				case DialogResult.Cancel:
				default:
					// No need for TextRequest("Canceled!") as parent will handle cancel.
					isCanceled = true;
					return (false);
			}
		}

		/// <summary>
		/// Saves settings to given file, also saves all terminals and prompts for files if they
		/// don't exist yet.
		/// </summary>
		/// <remarks>
		/// Note that not only the workspace gets saved, but also the terminals. Consider the
		/// default case:
		///  1. Application start, default workspace is created.
		///  2. Create a terminal.
		///  3. Intentionally save the workspace as.
		///      => The user expects to save the terminal as well.
		///      => No saving the terminal would lead to a normal file referring to an auto file!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public virtual bool SaveAs(string filePath)
		{
			AssertUndisposed();

			// First, save all contained terminals:
			if (!SaveAllTerminalsNormally(false))
				return (false);

			var absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(filePath);

			// Request the deletion of the obsolete auto saved settings file given the new file is different:
			string autoSaveFilePathToDelete = null;
			if (this.settingsRoot.AutoSaved && (!PathEx.Equals(absoluteFilePath, this.settingsHandler.SettingsFilePath)))
				autoSaveFilePathToDelete = this.settingsHandler.SettingsFilePath;

			// Set the new file path...
			this.settingsHandler.SettingsFilePath = absoluteFilePath;

			// ...adjust the potentially relative terminal files paths to the new absolute workspace file path...
			foreach (var t in this.terminals)
				ReplaceTerminalInWorkspaceSettings(t);

			// ...and then, save the workspace itself:
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual bool SaveToFile(bool isAutoSave, string autoSaveFilePathToDelete)
		{
			OnFixedStatusTextRequest("Saving workspace...");

			bool success = false;

			try
			{
				this.settingsHandler.Settings.AutoSaved = isAutoSave;
				this.settingsHandler.Save();
				success = true;

				if (!isAutoSave)
					this.userFileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, isAutoSave));
				OnTimedStatusTextRequest("Workspace saved.");

				if (!isAutoSave)
					SetRecent(this.settingsHandler.SettingsFilePath);

				// Try to delete existing auto save file:
				if (!string.IsNullOrEmpty(autoSaveFilePathToDelete))
				{
					// Ensure that this is not the current file!
					if (!PathEx.Equals(autoSaveFilePathToDelete, this.settingsHandler.SettingsFilePath))
						FileEx.TryDelete(autoSaveFilePathToDelete);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Error saving workspace!");

				OnFixedStatusTextRequest("Error saving workspace!");
				this.OnMessageInputRequest
				(
					Utilities.MessageHelper.ComposeMessage("Unable to save workspace file", this.settingsHandler.SettingsFilePath, ex),
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
		/// In case of a main exit, <see cref="CloseWithOptions"/> below must be called
		/// with the first argument set to <c>true</c>.
		/// </remarks>
		public virtual bool Close()
		{
			return (CloseWithOptions(false, ExitMode.None)); // See remarks above.
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
		/// 1. Save terminals and workspace.
		/// 2. Close terminals and workspace, but only if save was successful.
		/// </remarks>
		public virtual bool CloseWithOptions(bool isMainExit, ExitMode exitMode)
		{
			AssertUndisposed();

			OnFixedStatusTextRequest("Closing workspace...");

			// Keep info of existing former auto file:
			bool formerExistingAutoFileAutoSaved = this.settingsRoot.AutoSaved;

			string formerExistingAutoFilePath = null;
			if (this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFileExists)
				formerExistingAutoFilePath = this.settingsHandler.SettingsFilePath;

			// -------------------------------------------------------------------------------------
			// Evaluate save requirements:
			// -------------------------------------------------------------------------------------

			bool doSaveWorkspace = true;
			bool autoSaveIsAllowedForWorkspace = ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace;
			bool successWithWorkspace = false;

			bool doSaveTerminals = true;
			bool autoSaveIsAllowedForTerminals = ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace;
			bool successWithTerminals = false;

			// Do not save anything in case of an auto exit, e.g. an exit after a command line triggered operation.
			if (isMainExit && (exitMode == ExitMode.Auto))
			{
				doSaveWorkspace = false;
				autoSaveIsAllowedForWorkspace = false;
				successWithWorkspace = true;

				doSaveTerminals = false;
				autoSaveIsAllowedForTerminals = false;
				successWithTerminals = true;
			}

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
			// First, save all contained terminals:
			// -------------------------------------------------------------------------------------

			if (doSaveTerminals)
			{
				if (doSaveWorkspace)
				{
					if (autoSaveIsAllowedForTerminals)
						successWithTerminals = SaveAllTerminalsOnClose(true);
					else
						successWithTerminals = SaveAllTerminalsOnClose(false);
				}
				else
				{
					// Save normally saved terminals even if workspace was or will not be auto saved!
					if (autoSaveIsAllowedForTerminals)
						successWithTerminals = SaveAllTerminalsWhereFileHasAlreadyBeenNormallySavedOnClose();
					else
						successWithTerminals = SaveAllTerminalsOnClose(false);
				}
			}

			// -------------------------------------------------------------------------------------
			// Finalize save requirements for workspace:
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

					if (autoSaveIsAllowedForWorkspace)
						doSaveWorkspace = true;
					else
						successWithWorkspace = true;
				}
				else
				{
					// Explicit have changed, save is required, except if auto-save is not desired.

					if (!ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace && !SettingsFileHasAlreadyBeenNormallySaved)
					{
						doSaveWorkspace = false;
						successWithWorkspace = true;
					}
				}
			}
			else // isWorkspaceClose
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
					// Explicit have changed, save is required, except if auto-save makes no sense.

					if (!SettingsFileHasAlreadyBeenNormallySaved)
					{
						doSaveWorkspace = false;
						successWithWorkspace = true;
					}
				}
			}

			// -------------------------------------------------------------------------------------
			// Try auto save workspace itself, if allowed:
			// -------------------------------------------------------------------------------------

			if (successWithTerminals && !successWithWorkspace && doSaveWorkspace)
				successWithWorkspace = TrySaveWithOptionsWithoutUserInteraction(autoSaveIsAllowedForWorkspace); // Try auto save.

			// -------------------------------------------------------------------------------------
			// If not successfully saved so far, evaluate next step according to rules above:
			// -------------------------------------------------------------------------------------

			// Normal file (m3, m4, w3, w4):
			if (successWithTerminals && !successWithWorkspace && doSaveWorkspace && !this.settingsRoot.AutoSaved)
			{
				var dr = OnMessageInputRequest
				(
					"Save workspace?", // Do not use 'IndicatedName' as that would refer to the active terminal.
					ApplicationEx.ProductName, // "YAT" or "YATConsole", as indicated in main title bar.
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				);

				switch (dr)
				{
					case DialogResult.Yes: successWithWorkspace = Save(); break;
					case DialogResult.No:  successWithWorkspace = true;   break;

					case DialogResult.Cancel:
					default:
					{
						OnTimedStatusTextRequest("Canceled, workspace not closed.");
						return (false);
					}
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
			// Finally, cleanup and signal state:
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
					OnTimedStatusTextRequest("Workspace successfully closed, exiting...");
				else
					OnTimedStatusTextRequest("Workspace successfully closed.");

				// Discard potential exceptions already BEFORE signalling the close! Required to
				// prevent exceptions on still pending asynchronous callbacks trying to synchronize
				// event callbacks onto the workspace form which is going to be closed/disposed by
				// the handler of the 'Closed' event below!
				// Note there so far is no "workspace" form. Still be prepared for potential future
				// changes, or applications that implement a different view.
				this.eventHelper.DiscardAllExceptions();

				OnClosed(new ClosedEventArgs(isMainExit));

				// The workspace shall dispose of itself to free all resources for sure. It must be
				// done AFTER it raised the 'Closed' event and all subscribers of the event may still
				// refer to a non-disposed object. This is especially important, as the order of the
				// subscribers is not fixed, i.e. 'Model.Main' may dispose of the workspace before
				// 'View.Main' receives the event callback!
				Dispose();

				return (true);
			}
			else
			{
				if (isMainExit)
					OnTimedStatusTextRequest("Exit canceled, workspace not closed.");
				else
					OnTimedStatusTextRequest("Close canceled.");

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
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Add(recentFile);
			ApplicationSettings.LocalUserSettings.RecentFiles.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveLocalUserSettings();
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

				terminal.ExitRequest += terminal_ExitRequest;
			}
		}

		private void DetachTerminalEventHandlers(Terminal terminal)
		{
			if (terminal != null)
			{
				terminal.Saved  -= terminal_Saved;
				terminal.Closed -= terminal_Closed;

				terminal.ExitRequest -= terminal_ExitRequest;
			}
		}

		private void DetachAllTerminalEventHandlers()
		{
			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
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
			var t = (Terminal)sender;

			ReplaceTerminalInWorkspace(t);

			if (!e.IsAutoSave)
				SetRecent(e.FilePath);
		}

		/// <remarks>
		/// See remarks of <see cref="Terminal.CloseWithOptions"/> for details on why this handler
		/// needs to treat the event differently in case of a parent (i.e. workspace) close.
		/// </remarks>
		private void terminal_Closed(object sender, ClosedEventArgs e)
		{
			var t = (Terminal)sender;

			// Retrieve event args before removing the terminal:
			var sequentialId = t.SequentialId;
			var dynamicId    = GetDynamicIdByTerminal(t);
			var fixedId      = GetFixedIdByTerminal(t);

			DetachTerminalEventHandlers(t);
			RemoveTerminalFromWorkspace(t, !e.IsParentClose); // Simply remove the terminal from the workspace, it disposes of itself.
			OnTerminalRemoved(new TerminalEventArgs(t, sequentialId, dynamicId, fixedId));
		}

		private void terminal_ExitRequest(object sender, EventArgs e)
		{
			OnExitRequest(e);
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
				AssertUndisposed();

				if (this.terminals != null)
				{
					var filePaths = new List<string>(this.terminals.Count); // Preset the required capacity to improve memory management.
					foreach (var t in this.terminals)
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool CreateNewTerminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			AssertUndisposed();

			OnFixedStatusTextRequest("Creating new terminal...");
			OnCursorRequest(Cursors.WaitCursor);

			// Create new terminal:
			Terminal t;
			try
			{
				t = new Terminal(this.launchArgs.ToTerminalLaunchArgs(), settingsHandler);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to create/open new terminal!");

				var errorMessage = Utilities.MessageHelper.ComposeMessage("Unable to create terminal!", ex);

				OnCursorReset();
				OnFixedStatusTextRequest("Error creating terminal!");
				OnMessageInputRequest
				(
					errorMessage,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
				);
				OnTimedStatusTextRequest("Terminal not creating!");

				return (false);
			}

			// Attention:
			// Similar code exists in OpenTerminalFromSettings() further below.
			// Changes here may have to be applied there too.

			AttachTerminalEventHandlers(t);
			AddTerminalToWorkspace(t);
			OnTerminalAdded(new TerminalEventArgs(t, t.SequentialId, GetDynamicIdByTerminal(t), GetFixedIdByTerminal(t)));

			OnCursorReset();
			OnTimedStatusTextRequest("New terminal created.");

			if (t.AutoIsActive)
				t.NotifyAutoIsReady(); // See remarks of this method.

			if (t.HasLinkedSettings)
			{
			////OnFixedStatusTextRequest("Loading linked settings..."); is done within TryLoadLinkedSettings().

				bool isCanceled;              // Shall not be cancelable, nobody would understand canceling closes the terminal.
				t.TryLoadLinkedSettings(true, false, out isCanceled); // User interaction is done within TryLoadLinkedSettings().
				// No need to handle return value, errors are handled within TryLoadLinkedSettings().

			////OnTimedStatusTextRequest("Linked settings loaded."); is done within TryLoadLinkedSettings().
			}

			t.Launch(); // Errors during launch are handled there.

			return (true); // Return success even in case of errors since terminal successfully got created.
		}

		/// <summary>
		/// Opens terminals according to workspace settings and returns number of successfully opened terminals.
		/// </summary>
		public virtual int OpenTerminals()
		{
			return (OpenTerminals(TerminalIds.InvalidDynamicId, TerminalIds. InvalidFixedId, null));
		}

		/// <summary>
		/// Opens terminals according to workspace settings and returns number of successfully opened terminals.
		/// </summary>
		public virtual int OpenTerminals(int dynamicTerminalIdToReplace, int fixedTerminalIdToReplace, DocumentSettingsHandler<TerminalSettingsRoot> terminalSettingsToReplace)
		{
			AssertUndisposed();

			int requestedTerminalCount = this.settingsRoot.TerminalSettings.Count;
			if (requestedTerminalCount == 1)
				OnFixedStatusTextRequest("Opening workspace's terminal...");
			else if (requestedTerminalCount > 1)
				OnFixedStatusTextRequest("Opening workspace's terminals...");

			OnCursorRequest(Cursors.WaitCursor);

			int openedTerminalCount = 0;
			var clone = new GuidList<TerminalSettingsItem>(this.settingsRoot.TerminalSettings);
			for (int i = 0; i < clone.Count; i++)
			{
				var item = clone[i];

				// \remind
				// Check whether the item is defined. Because under certain error conditions there
				// were occasions when the item contained an empty file path and an empty GUID. That
				// has lead to an exception in an underlying 'System.IO' call and would have lead to
				// an error message which isn't really understandable to the user. Therefore, check
				// the item and remove it if not defined.

				if (item.IsDefined)
				{
					bool success = false;
					string messageOnFailure = null;

					// Replace the desired terminal settings if requested:
					bool isToReplace = false;

					if (dynamicTerminalIdToReplace == TerminalIds.ActiveDynamicId)
					{
						if (i == (clone.Count - 1)) // The active terminal is located last in the collection.
							isToReplace = true;
					}
					else if (dynamicTerminalIdToReplace != TerminalIds.InvalidDynamicId)
					{
						if (i == TerminalIds.DynamicIdToIndex(dynamicTerminalIdToReplace))
							isToReplace = true;
					}

					if (fixedTerminalIdToReplace == TerminalIds.ActiveFixedId)
					{
						if (i == (clone.Count - 1)) // The active terminal is located last in the collection.
							isToReplace = true;
					}
					else if (fixedTerminalIdToReplace != TerminalIds.InvalidFixedId)
					{
						if (item.FixedId == fixedTerminalIdToReplace)
							isToReplace = true;
					}

					if (isToReplace)
					{
						Exception ex;
						if (OpenTerminalFromSettings(terminalSettingsToReplace, item.Guid, item.FixedId, item.Window, out ex))
						{
							openedTerminalCount++;
							success = true;
						}
						else
						{
							if (!string.IsNullOrEmpty(item.FilePath))
								messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open terminal", item.FilePath, ex);
							else
								messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open terminal!", ex);
						}
					}
					else // In all other cases, 'normally' open the terminal from the given file:
					{
						if (OpenTerminalFromFile(item.FilePath, item.Guid, item.FixedId, item.Window, out messageOnFailure))
						{                                   // Error must be handled here because of looping over terminals.
							openedTerminalCount++;
							success = true;
						}
					}

					if (!success)
					{
						this.settingsRoot.TerminalSettings.Remove(item);
						this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.

						if (string.IsNullOrEmpty(messageOnFailure))
						{
							if (!string.IsNullOrEmpty(item.FilePath))
								messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open terminal file", item.FilePath);
							else
								messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open terminal!");
						}

						string caption;
						if (!string.IsNullOrEmpty(item.FilePath))
							caption = "Terminal File Error";
						else
							caption = "Terminal Error";

						OnCursorReset();
						OnFixedStatusTextRequest("Error opening terminal!");
						var dr = OnMessageInputRequest
						(
							messageOnFailure + Environment.NewLine + Environment.NewLine + "Continue loading workspace?",
							caption,
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Error
						);
						OnTimedStatusTextRequest("Terminal not opened!");
						OnCursorRequest(Cursors.WaitCursor);

						if (dr == DialogResult.No)
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
		/// <returns><c>true</c> if successfully opened the terminal; otherwise, <c>false</c>.</returns>
		public virtual bool OpenTerminalFromFile(string filePath)
		{
			AssertUndisposed();

			string fileName = Path.GetFileName(filePath);
			OnFixedStatusTextRequest("Opening terminal " + fileName + "...");
			OnCursorRequest(Cursors.WaitCursor);

			string messageOnFailure;
			if (OpenTerminalFromFile(filePath, Guid.Empty, TerminalIds.ActiveFixedId, null, out messageOnFailure))
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
					messageOnFailure,
					"Terminal File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
				);
				OnTimedStatusTextRequest("Terminal not opened!");

				return (false);
			}
		}

		private bool OpenTerminalFromFile(string filePath, Guid guid, int fixedId, WindowSettings windowSettings, out string messageOnFailure)
		{
			string absoluteFilePath;
			DocumentSettingsHandler<TerminalSettingsRoot> sh;
			Exception ex;
			if (OpenTerminalFile(filePath, out absoluteFilePath, out sh, out ex))
			{
				if (OpenTerminalFromSettings(sh, guid, fixedId, windowSettings, out ex))
				{
					messageOnFailure = null;
					return (true);
				}
				else
				{
					messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open terminal", absoluteFilePath, ex);
					return (false);
				}
			}
			else
			{
				messageOnFailure = Utilities.MessageHelper.ComposeMessage("Unable to open terminal file", absoluteFilePath, ex);
				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool OpenTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
		{
			Exception exceptionOnFailure;
			return (OpenTerminalFromSettings(settingsHandler, Guid.Empty, TerminalIds.ActiveFixedId, null, out exceptionOnFailure));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenTerminalFromSettings(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid, int fixedId, WindowSettings windowSettings, out Exception exceptionOnFailure)
		{
			AssertUndisposed();

			// Ensure that the terminal file is not already open:
			if (!CheckTerminalFiles(settingsHandler.SettingsFilePath))
			{
				exceptionOnFailure = null;
				return (false);
			}

			OnFixedStatusTextRequest("Opening terminal...");

			// Set window settings if there are:
			if (windowSettings != null)
				settingsHandler.Settings.Window = windowSettings;

			// Override settings if requested:
			if (this.launchArgs.Override.StartTerminal)
				settingsHandler.Settings.TerminalIsStarted = true;

			if (this.launchArgs.Override.KeepTerminalStopped)
				settingsHandler.Settings.TerminalIsStarted = false;

			if (this.launchArgs.Override.LogOn)
				settingsHandler.Settings.LogIsOn = true;

			if (this.launchArgs.Override.KeepLogOff)
				settingsHandler.Settings.LogIsOn = false;

			// Create terminal from settings:
			Terminal t;
			try
			{
				t = new Terminal(this.launchArgs.ToTerminalLaunchArgs(), settingsHandler, guid);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to create/open terminal from settings!");
				exceptionOnFailure = ex;
				return (false);
			}

			// Attention:
			// Similar code exists in CreateNewTerminal() further above.
			// Changes here may have to be applied there too.

			AttachTerminalEventHandlers(t);
			AddTerminalToWorkspace(t, fixedId);
			OnTerminalAdded(new TerminalEventArgs(t, t.SequentialId, GetDynamicIdByTerminal(t), GetFixedIdByTerminal(t)));

			if (!settingsHandler.Settings.AutoSaved)
				SetRecent(settingsHandler.SettingsFilePath);

			OnTimedStatusTextRequest("Terminal opened.");

			if (t.AutoIsActive)
				t.NotifyAutoIsReady(); // See remarks of this method.

			if (t.HasLinkedSettings)
			{
			////OnFixedStatusTextRequest("Loading linked settings..."); is done within TryLoadLinkedSettings().

				bool isCanceled;              // Shall not be cancelable, nobody would understand canceling closes the terminal.
				t.TryLoadLinkedSettings(true, false, out isCanceled); // User interaction is done within TryLoadLinkedSettings().
				// No need to handle return value, errors are handled within TryLoadLinkedSettings().

			////OnTimedStatusTextRequest("Linked settings loaded."); is done within TryLoadLinkedSettings().
			}

		////t.Start(); is done by StartAllTerminals() below.

			exceptionOnFailure = null;
			return (true);
		}

		/// <summary></summary>
		public virtual bool LaunchAllTerminals()
		{
			bool success = true;

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
				{
					if (!t.Launch())
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
				foreach (var t in this.terminals)
				{
					if (PathEx.Equals(terminalFilePath, t.SettingsFilePath))
					{
						OnFixedStatusTextRequest("Terminal is already open.");
						OnMessageInputRequest
						(
							"Terminal is already open and will not be re-openend.",
							"Terminal",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation
						);
						OnTimedStatusTextRequest("Terminal not re-opened.");
						return (false);
					}
				}
			}
			return (true);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool OpenTerminalFile(string filePath, out string absoluteFilePath, out DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, out Exception exceptionOnFailure)
		{
			// Attention:
			// Similar code exists in Main.OpenTerminalFile().
			// Changes here may have to be applied there too.

			absoluteFilePath = null;

			// Try to combine workspace and terminal path:
			if (!string.IsNullOrEmpty(this.settingsHandler.SettingsFilePath))
				absoluteFilePath = PathEx.CombineFilePaths(this.settingsHandler.SettingsFilePath, filePath);

			// Alternatively, try to use terminal file path only:
			if (string.IsNullOrEmpty(absoluteFilePath))
				absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(filePath);

			try
			{
				var sh = new DocumentSettingsHandler<TerminalSettingsRoot>();
				sh.SettingsFilePath = absoluteFilePath;
				if (sh.Load())
				{
					// The 'MatchSerial' setting is given by the 'LocalUserSettings' and always overridden.
					// Still, it is an integral part of MKY.IO.Serial.Usb, will thus be contained in the .yat file.
					sh.Settings.Terminal.IO.UsbSerialHidDevice.MatchSerial = ApplicationSettings.LocalUserSettings.General.MatchUsbSerial;
					sh.Settings.ClearChanged(); // Overriding such setting shall not be reflected in the settings,
					                          //// i.e. neither be indicated by a '*' nor lead to a file write.
					settingsHandler = sh;
					exceptionOnFailure = null;
					return (true);
				}
				else
				{
					settingsHandler = null;
					exceptionOnFailure = null;
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to open terminal file!");

				settingsHandler = null;
				exceptionOnFailure = ex;
				return (false);
			}
		}

		private TerminalSettingsItem CreateTerminalSettingsItem(Terminal terminal, int fixedId)
		{
			var tsi = new TerminalSettingsItem();

			string filePath = terminal.SettingsFilePath;
			if (ApplicationSettings.LocalUserSettings.General.UseRelativePaths)
			{
				var pcr = PathEx.CompareFilePaths(this.settingsHandler.SettingsFilePath, terminal.SettingsFilePath);
				if (pcr.AreRelative)
					filePath = pcr.RelativePath;
			}

			tsi.Guid = terminal.Guid;
			tsi.FilePath = filePath;

			if (fixedId >= TerminalIds.FirstFixedId)
				tsi.FixedId = fixedId;

			tsi.Window = new WindowSettings(terminal.SettingsRoot.Window); // Clone to ensure decoupling.

			return (tsi);
		}

		private void AddTerminalToWorkspace(Terminal terminal, int requestedFixedId = TerminalIds.ActiveFixedId)
		{
			// Add terminal to terminal list:
			this.terminals.Add(terminal);
			this.activeTerminal = terminal;
			int effectiveId = AddTerminalToFixedIds(terminal, requestedFixedId);

			// Add terminal settings for new terminals:
			// Replace terminal settings if workspace settings have been loaded from file prior.
			this.settingsRoot.TerminalSettings.AddOrReplace(CreateTerminalSettingsItem(terminal, effectiveId));
			this.settingsRoot.SetChanged(); // Has to be called explicitly because a 'normal' list is being modified.
		}

		private void ReplaceTerminalInWorkspace(Terminal terminal)
		{
			// Replace terminal in terminal list:
			this.terminals.Replace(terminal);
			this.activeTerminal = terminal;

			// Replace terminal in workspace settings:
			ReplaceTerminalInWorkspaceSettings(terminal);
		}

		/// <summary>
		/// Replaces terminal in workspace settings if the settings have changed.
		/// </summary>
		private void ReplaceTerminalInWorkspaceSettings(Terminal terminal)
		{
			TerminalSettingsItem tsiNew = CreateTerminalSettingsItem(terminal, GetFixedIdByTerminal(terminal));
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
			RemoveTerminalFromFixedIds(terminal);
			DeactivateTerminal(terminal);
		}

		private int AddTerminalToFixedIds(Terminal terminal, int requestedFixedId)
		{
			// First, try to lookup the requested fixed ID if suitable:
			if (requestedFixedId >= TerminalIds.FirstFixedId)
			{
				if (!this.fixedIds.ContainsKey(requestedFixedId))
				{
					this.fixedIds.Add(requestedFixedId, terminal);
					return (requestedFixedId);
				}
			}

			// As fallback, use the next available fixed ID:
			for (int id = TerminalIds.FirstFixedId; id <= int.MaxValue; id++)
			{
				if (!this.fixedIds.ContainsKey(id))
				{
					this.fixedIds.Add(id, terminal);
					return (id);
				}
			}

			// If both fail, no good! It means that there are more than 2'000'000'000 terminals ;-)
			throw (new OverflowException("Constant index of terminals exceeded!"));
		}

		private void RemoveTerminalFromFixedIds(Terminal terminal)
		{
			this.fixedIds.Remove(GetFixedIdByTerminal(terminal));
		}

		#endregion

		#region Terminals > Get Methods
		//------------------------------------------------------------------------------------------
		// Terminals > Get Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the fixed ID of the given terminal.
		/// </summary>
		public virtual int GetFixedIdByTerminal(Terminal terminal)
		{
			foreach (KeyValuePair<int, Terminal> kvp in this.fixedIds)
			{
				if (kvp.Value == terminal)
					return (kvp.Key);
			}

			throw (new ArgumentOutOfRangeException("terminal", terminal, MessageHelper.InvalidExecutionPreamble + "Terminal'" + terminal + "'not found in ID table!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary>
		/// Returns the dynamic ID of the given terminal.
		/// </summary>
		public virtual int GetDynamicIdByTerminal(Terminal terminal)
		{
			AssertUndisposed();

			int index = this.terminals.IndexOf(this.activeTerminal);
			return (TerminalIds.IndexToDynamicId(index));
		}

		/// <summary>
		/// Tries to return the terminal with the given GUID.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public virtual bool TryGetTerminalByGuid(Guid guid, out Terminal terminal)
		{
			AssertUndisposed();

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
				{
					if (t.Guid == guid)
					{
						terminal = t;
						return (true);
					}
				}
			}

			terminal = null;
			return (false);
		}

		/// <summary>
		/// Tries to return the terminal with the given sequential ID. The ID corresponds to the
		/// number indicated in the terminal name, e.g. "Terminal1" or "Terminal2". The ID starts
		/// at 1 and is unique throughout the execution of the program.
		/// </summary>
		public virtual bool TryGetTerminalBySequentialId(int sequentialId, out Terminal terminal)
		{
			AssertUndisposed();

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
				{
					if (t.SequentialId == sequentialId)
					{
						terminal = t;
						return (true);
					}
				}
			}

			terminal = null;
			return (false);
		}

		/// <summary>
		/// Tries to return the terminal with the given dynamic ID. The dynamic ID represents the
		/// order in which the terminals got created. If a terminal gets closed, the dynamic ID of
		/// all subsequent terminals is adjusted.
		/// </summary>
		/// <remarks>
		/// The index must be in the range of 1...NumberOfTerminals, or 0 to select the currently
		/// active terminal.
		/// </remarks>
		public virtual bool TryGetTerminalByDynamicId(int dynamicId, out Terminal terminal)
		{
			AssertUndisposed();

			if (dynamicId == TerminalIds.ActiveDynamicId)
			{
				terminal = this.activeTerminal;
				return (true);
			}
			else
			{
				int index = TerminalIds.DynamicIdToIndex(dynamicId);
				if ((index >= 0) && (index < this.terminals.Count))
				{
					terminal = this.terminals[index];
					return (true);
				}
				else
				{
					terminal = null;
					return (false);
				}
			}
		}

		/// <summary>
		/// Tries to return the terminal with the given fixed ID. The fixed ID represents the order
		/// in which the terminals initially got created and doesn't change throughout the execution
		/// of the program. If a terminal is closed, the corresponding ID becomes available and will
		/// be used for the next terminal that is opened, i.e. a new terminal always gets the lowest
		/// available fixed ID.
		/// </summary>
		public virtual bool TryGetTerminalByFixedId(int index, out Terminal terminal)
		{
			AssertUndisposed();

			if (this.fixedIds.TryGetValue(index, out terminal))
			{
				return (true);
			}
			else
			{
				terminal = null;
				return (false);
			}
		}

		#endregion

		#region Terminals > All Terminals Methods
		//------------------------------------------------------------------------------------------
		// Terminals > All Terminals Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool SaveAllTerminals()
		{
			AssertUndisposed();

			return (SaveAllTerminalsEvenIfNotChanged(true)); // Save even if not changed since explicitly requesting saving all.
		}

		/// <summary></summary>
		protected virtual bool SaveAllTerminalsEvenIfNotChanged(bool autoSaveIsAllowed)
		{
			return (SaveAllTerminalsWithOptions(false, autoSaveIsAllowed, true));
		}

		/// <summary></summary>
		protected virtual bool SaveAllTerminalsNormally(bool autoSaveIsAllowed)
		{
			return (SaveAllTerminalsWithOptions(false, autoSaveIsAllowed, false));
		}

		/// <summary></summary>
		protected virtual bool SaveAllTerminalsOnClose(bool autoSaveIsAllowed)
		{
			return (SaveAllTerminalsWithOptions(true, autoSaveIsAllowed, false));
		}

		private bool SaveAllTerminalsWithOptions(bool isWorkspaceClose, bool autoSaveIsAllowed, bool saveEvenIfNotChanged)
		{
			bool success = true;

			// Ensure that normal workspaces do not refer to auto terminals:
			bool autoSaveIsAllowedOnTerminals = false;
			if (autoSaveIsAllowed)
				autoSaveIsAllowedOnTerminals = EvaluateWhetherAutoSaveIsAllowedIndeed(autoSaveIsAllowed);

			// Calling Save() on a terminal may modify 'this.terminals' in the terminal_Saved()
			// event, therefore clone the list first.
			var clonedTerminalCollection = new List<Terminal>(this.terminals);
			foreach (var t in clonedTerminalCollection)
			{
				bool isCanceled;
				if (!t.SaveWithOptions(isWorkspaceClose, autoSaveIsAllowedOnTerminals, true, saveEvenIfNotChanged, true, out isCanceled))
				{
					success = false;

					if (isCanceled)
						break;
				}
			}

			return (success);
		}

		/// <summary></summary>
		private bool SaveAllTerminalsWhereFileHasAlreadyBeenNormallySavedOnClose()
		{
			bool success = true;

			// Calling Save() on a terminal may modify 'this.terminals' in the terminal_Saved()
			// event, therefore clone the list first.
			var clonedTerminalCollection = new List<Terminal>(this.terminals);
			foreach (var t in clonedTerminalCollection)
			{
				if (t.SettingsFileHasAlreadyBeenNormallySaved)
				{
					bool isCanceled;                 // 'false' since "where file has already been *normally* saved"!
					if (!t.SaveWithOptions(true, false, true, false, true, out isCanceled))
					{
						success = false;

						if (isCanceled)
							break;
					}
				}
			}

			return (success);
		}

		/// <remarks>
		/// In case of a workspace close, <see cref="CloseAllTerminals(bool, bool, bool, bool)"/>
		/// further below must be called with the first argument set to <c>true</c>.
		///
		/// In case of intended close of one or all terminals, the user intentionally wants to close
		/// the terminal(s), thus, this method will not try to auto save.
		/// </remarks>
		public virtual bool CloseAllTerminals()
		{
			AssertUndisposed();

			return (CloseAllTerminals(false, true, false, true)); // See remarks above.
		}

		/// <remarks>
		/// See remarks of <see cref="Terminal.CloseWithOptions"/> for details on 'WorkspaceClose'.
		/// </remarks>
		private bool CloseAllTerminals(bool isWorkspaceClose, bool doSave, bool autoSaveIsAllowed, bool autoDeleteIsRequested)
		{
			bool success = true;

			// Ensure that normal workspaces do not refer to auto terminals:
			bool autoSaveIsAllowedOnTerminals = false;
			if (doSave && autoSaveIsAllowed)
				autoSaveIsAllowedOnTerminals = EvaluateWhetherAutoSaveIsAllowedIndeed(autoSaveIsAllowed);

			// Calling Close() on a terminal will modify 'this.terminals' in the terminal_Closed()
			// event, therefore clone the list first.
			var clonedTerminalCollection = new List<Terminal>(this.terminals);
			foreach (var t in clonedTerminalCollection)
			{
				if (!t.CloseWithOptions(isWorkspaceClose, doSave, autoSaveIsAllowedOnTerminals, autoDeleteIsRequested))
					success = false;
			}

			return (success);
		}

		/// <summary></summary>
		public virtual void AllClear()
		{
			AssertUndisposed();

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
					t.ClearRepositories();
			}
		}

		/// <summary></summary>
		public virtual void AllRefresh()
		{
			AssertUndisposed();

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
					t.RefreshRepositories();
			}
		}

		/// <summary></summary>
		public virtual bool AllLogOn()
		{
			AssertUndisposed();

			bool success = true;

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
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
			AssertUndisposed();

			bool success = true;

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
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
			AssertUndisposed();

			bool success = true;

			if (this.terminals != null)
			{
				foreach (var t in this.terminals)
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
			AssertUndisposed();

			this.activeTerminal = terminal;
		}

		/// <summary></summary>
		public virtual void ActivateTerminalBySequentialId(int sequentialId)
		{
			Terminal t;
			if (TryGetTerminalBySequentialId(sequentialId, out t))
				ActivateTerminal(t);
		}

		/// <summary></summary>
		public virtual void DeactivateTerminal(Terminal terminal)
		{
			AssertUndisposed();

			if (this.activeTerminal == terminal)
				this.activeTerminal = null;
		}

		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool SaveActiveTerminal()
		{
			AssertUndisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.Save());
			else
				return (false);
		}

		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool CloseActiveTerminal()
		{
			AssertUndisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.Close());
			else
				return (false);
		}

		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool StartActiveTerminal()
		{
			AssertUndisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.Start());
			else
				return (false);
		}

		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool StopActiveTerminal()
		{
			AssertUndisposed();

			if (this.activeTerminal != null)
				return (this.activeTerminal.Stop());
			else
				return (false);
		}

		#endregion

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTerminalAdded(TerminalEventArgs e)
		{
			this.eventHelper.RaiseSync<TerminalEventArgs>(TerminalAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnTerminalRemoved(TerminalEventArgs e)
		{
			this.eventHelper.RaiseSync<TerminalEventArgs>(TerminalRemoved, this, e);
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(FixedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(TimedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnCursorRequest(Cursor cursor)
		{
			this.eventHelper.RaiseSync<EventArgs<Cursor>>(CursorRequest, this, new EventArgs<Cursor>(cursor));
		}

		/// <summary></summary>
		protected virtual void OnCursorReset()
		{
			OnCursorRequest(Cursors.Default);
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			if (this.launchArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				var e = new MessageInputEventArgs(text, caption, buttons, icon);
				this.eventHelper.RaiseSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Message Input' request by the workspace has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				#endif
				}

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <summary>
		/// Requests to show the 'SaveAs' dialog to let the user chose a file path.
		/// If confirmed, the file will be saved to that path.
		/// </summary>
		protected virtual DialogResult OnSaveAsFileDialogRequest()
		{
			if (this.launchArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new DialogEventArgs();
				this.eventHelper.RaiseSync<DialogEventArgs>(SaveAsFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Save As' request by the workspace has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				#endif
				}

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <summary>
		/// Requests to show the 'SaveAs' dialog to let the user chose a file path.
		/// If confirmed, the file will be saved to that path.
		/// </summary>
		protected virtual FilePathDialogResult OnSaveCommandPageAsFileDialogRequest(string filePathOld)
		{
			if (this.launchArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new FilePathDialogEventArgs(filePathOld);
				this.eventHelper.RaiseSync<FilePathDialogEventArgs>(SaveCommandPageAsFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Save As' request by the workspace has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				#endif
				}

				return (new FilePathDialogResult(e.Result, e.FilePathNew));
			}
			else
			{
				return (new FilePathDialogResult(DialogResult.None));
			}
		}

		/// <summary>
		/// Requests to show the 'Open' dialog to let the user chose a file path.
		/// If confirmed, the file will be saved to that path.
		/// </summary>
		protected virtual FilePathDialogResult OnOpenCommandPageFileDialogRequest(string filePathOld)
		{
			if (this.launchArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new FilePathDialogEventArgs(filePathOld);
				this.eventHelper.RaiseSync<FilePathDialogEventArgs>(OpenCommandPageFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "An 'Open' request by the workspace has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				#endif
				}

				return (new FilePathDialogResult(e.Result, e.FilePathNew));
			}
			else
			{
				return (new FilePathDialogResult(DialogResult.None));
			}
		}

		/// <summary></summary>
		protected virtual void OnSaved(SavedEventArgs e)
		{
			this.eventHelper.RaiseSync<SavedEventArgs>(Saved, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(ClosedEventArgs e)
		{
			this.eventHelper.RaiseSync<ClosedEventArgs>(Closed, this, e);
		}

		/// <summary></summary>
		protected virtual void OnExitRequest(EventArgs e)
		{
			this.eventHelper.RaiseSync(ExitRequest, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
				return (Caption);
			else
				return (base.ToString());
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name 'DebugWriteLine' would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named 'Message' for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. 'Common' for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
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
