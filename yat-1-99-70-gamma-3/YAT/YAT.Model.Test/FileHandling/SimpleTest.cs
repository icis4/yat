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
// YAT 2.0 Gamma 3 Version 1.99.70
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY;
using MKY.IO;
using MKY.Settings;

using NUnit.Framework;

using YAT.Application.Utilities;
using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.FileHandling
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1123:DoNotPlaceRegionsWithinElements", Justification = "Group use cases into regions.")]
	[TestFixture]
	public class SimpleTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private readonly string normalWorkspaceFilePath = Temp.MakeTempFilePath(typeof(SimpleTest), "NormalWorkspace", ExtensionHelper.WorkspaceFile);
		private readonly string normalTerminalFilePath  = Temp.MakeTempFilePath(typeof(SimpleTest), "NormalTerminal",  ExtensionHelper.TerminalFile);
		private readonly string normalTerminal1FilePath = Temp.MakeTempFilePath(typeof(SimpleTest), "NormalTerminal1", ExtensionHelper.TerminalFile);
		private readonly string normalTerminal2FilePath = Temp.MakeTempFilePath(typeof(SimpleTest), "NormalTerminal2", ExtensionHelper.TerminalFile);
		private readonly string normalTerminal3FilePath = Temp.MakeTempFilePath(typeof(SimpleTest), "NormalTerminal3", ExtensionHelper.TerminalFile);

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool autoOpenWorkspaceToRestore;
		private bool autoSaveWorkspaceToRestore;
		private string workspaceFilePathToRestore;

		#endregion

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create 'normal' file-based application settings for this test run.
			// The 'normal' application settings allow easy check of the settings file.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadSharedWriteIfOwned);
			ApplicationSettings.Load();

			// Allow modification of auto-save setting.
			this.autoOpenWorkspaceToRestore = ApplicationSettings.LocalUserSettings.General.AutoOpenWorkspace;
			this.autoSaveWorkspaceToRestore = ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace;
			this.workspaceFilePathToRestore = ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath;

			ApplicationSettings.Save();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Restore auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoOpenWorkspace = this.autoOpenWorkspaceToRestore;
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = this.autoSaveWorkspaceToRestore;
			ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath    = this.workspaceFilePathToRestore;

			// Restore 'normal' file-based application settings.
			ApplicationSettings.Save();
			ApplicationSettings.CloseAndDispose();

			Temp.CleanTempPath(GetType());
		}

		#endregion

		#region TestInstance
		//==========================================================================================
		// TestInstance
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[SetUp]
		public virtual void SetUp()
		{
			// By default, reset the application settings to their defaults before running any test.
			ApplicationSettings.LocalUserSettings.General.AutoOpenWorkspace = true;
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = true;
			ApplicationSettings.LocalUserSettings.AutoWorkspace.ResetFilePath();

			ApplicationSettings.Save();

			Temp.MakeTempPath(GetType());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TearDown]
		public virtual void TearDown()
		{
			Temp.CleanTempPath(GetType());
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > TestInitialAutoSaveOnExit
		//------------------------------------------------------------------------------------------
		// Tests > TestInitialAutoSaveOnExit
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  Auto save of terminal and workspace.
		/// </summary>
		[Test]
		public virtual void TestInitialAutoSaveOnMainExit()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			Utilities.StartAndCreateDefaultTerminal(out main, out workspace, out terminal);

			workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
			terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

			using (main)
			{
				bool success = false;

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, "Main could not be exited successfully!");

				Utilities.VerifyFiles(workspace, true, terminal, true);
			}
		}

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  No auto save.
		/// </summary>
		[Test]
		public virtual void TestInitialAutoSaveOnWorkspaceAndMainExit()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			Utilities.StartAndCreateDefaultTerminal(out main, out workspace, out terminal);

			workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
			terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

			using (main)
			{
				bool success = false;

				success = workspace.Close();
				Assert.That(success, Is.True, "Workspace could not be closed successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, "Main could not be exited successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);
			}
		}

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  Auto save of workspace.
		/// </summary>
		[Test]
		public virtual void TestInitialAutoSaveOnTerminalAndMainExit()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			Utilities.StartAndCreateDefaultTerminal(out main, out workspace, out terminal);

			workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
			terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

			using (main)
			{
				bool success = false;

				success = terminal.Close();
				Assert.That(success, Is.True, "Terminal could not be closed successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, "Main could not be exited successfully!");

				Utilities.VerifyFiles(workspace, true, terminal, false);
			}
		}

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  No auto save.
		/// </summary>
		[Test]
		public virtual void TestInitialAutoSaveOnTerminalAndWorkspaceAndMainExit()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			Utilities.StartAndCreateDefaultTerminal(out main, out workspace, out terminal);

			workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
			terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

			using (main)
			{
				bool success = false;

				success = terminal.Close();
				Assert.That(success, Is.True, "Terminal could not be closed successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);

				success = workspace.Close();
				Assert.That(success, Is.True, "Workspace could not be closed successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, "Main could not be exited successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);
			}
		}

		#endregion

		#region Tests > TestAutoSave
		//------------------------------------------------------------------------------------------
		// Tests > TestAutoSave
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestAutoSaveNoChanges()
		{
			bool success = false;
			string step = "";
			DateTime terminalLastWriteTimeInitially;

			#region Preparation
			// - Initial start
			// - Create new terminal
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal;
				Utilities.InitialStart("Preparation: ", main, out workspace, this.normalTerminalFilePath, out terminal);

				terminalLastWriteTimeInitially = File.GetLastWriteTimeUtc(terminal.SettingsFilePath);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion

			#region Step 1
			// - Subsequent start
			// - No changes on terminal settings
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal;
				Utilities.SubsequentStart("Step 1: ", main, out workspace, out terminal);

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var normalTerminalLastWriteTimeAfterStart = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(normalTerminalLastWriteTimeAfterStart, Is.EqualTo(terminalLastWriteTimeInitially));

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion
		}

		/// <summary></summary>
		[Test]
		public virtual void TestAutoSaveImplicitChange()
		{
			bool success = false;
			string step = "";
			DateTime terminalLastWriteTimeInitially;

			#region Preparation
			// - Initial start
			// - Create new terminal
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal;
				Utilities.InitialStart("Preparation: ", main, out workspace, this.normalTerminalFilePath, out terminal);

				terminalLastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion

			#region Step 1
			// - Subsequent start
			// - Implicit change on terminal settings
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal;
				Utilities.SubsequentStart("Step 1: ", main, out workspace, out terminal);

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var normalTerminalLastWriteTimeAfterStart = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(normalTerminalLastWriteTimeAfterStart, Is.EqualTo(terminalLastWriteTimeInitially));

				terminal.SettingsRoot.SendText.Command = new Types.Command(Guid.NewGuid().ToString()); // Implicit change.
				Assert.That(terminal.SettingsRoot.ExplicitHaveChanged, Is.False, step + "Settings have explicitly changed!");

				var terminalLastWriteTimeAfterChange = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterChange, Is.EqualTo(terminalLastWriteTimeInitially));

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.Not.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion
		}

		/// <summary></summary>
		[Test]
		public virtual void TestAutoSaveExplicitChange()
		{
			bool success = false;
			string step = "";
			DateTime terminalLastWriteTimeInitially;

			#region Preparation
			// - Initial start
			// - Create new terminal
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal;
				Utilities.InitialStart("Preparation: ", main, out workspace, this.normalTerminalFilePath, out terminal);

				terminalLastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion

			#region Step 1
			// - Subsequent start
			// - Explicit change on terminal settings
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal;
				Utilities.SubsequentStart("Step 1: ", main, out workspace, out terminal);

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var normalTerminalLastWriteTimeAfterStart = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(normalTerminalLastWriteTimeAfterStart, Is.EqualTo(terminalLastWriteTimeInitially));

				terminal.SettingsRoot.UserName = Guid.NewGuid().ToString(); // Explicit change.
				Assert.That(terminal.SettingsRoot.ExplicitHaveChanged, Is.True, step + "Settings have not explicitly changed!");

				var terminalLastWriteTimeAfterChange = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterChange, Is.EqualTo(terminalLastWriteTimeInitially));

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, true, terminal, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.Not.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion
		}

		#endregion

		#region Tests > TestAutoDeleteOnClose
		//------------------------------------------------------------------------------------------
		// Tests > TestAutoDeleteOnClose
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  Auto save of terminal and workspace.
		/// </summary>
		[Test]
		public virtual void TestAutoDeleteOnClose()
		{
			bool success = false;

			Main main;
			Workspace workspace;
			Terminal terminal;

			// Initial start with auto save on exit:

			Utilities.StartAndCreateDefaultTerminal(out main, out workspace, out terminal);

			workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
			terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

			using (main)
			{
				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, "Main could not be exited successfully!");

				Utilities.VerifyFiles(workspace, true, terminal, true);
			}

			// Subsequent start with auto delete on close of workspace:

			using (main = new Main())
			{
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, "Main could not be started!");

				workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), "Workspace doesn't contain 1 terminal!");

				terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, "Terminal not opened from file!");

				Utilities.VerifyFiles(workspace, true, terminal, true);

				success = workspace.Close();
				Assert.That(success, Is.True, "Workspace could not be closed successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, "Main could not be exited successfully!");

				Utilities.VerifyFiles(workspace, false, terminal, false);
			}
		}

		#endregion

		#region Tests > TestRecentInCaseOfAutoSave
		//------------------------------------------------------------------------------------------
		// Tests > TestRecentInCaseOfAutoSave
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  File saved as.
		/// </summary>
		[Test]
		public virtual void TestRecentInCaseOfAutoSave()
		{
			bool success = false;
			string step = "";

			ApplicationSettings.LocalUserSettings.General.AutoOpenWorkspace = true;
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = true;
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Clear();

			#region Step 1
			// - Initial start
			// - Create new terminal
			//   => Auto workspace with 1 auto terminal
			//   => Recent must still be empty
			using (Main main = new Main())
			{
				step = "Step 1: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal could not be created!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, step + "Terminal could not be created!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, terminal, true);

				Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count, Is.EqualTo(0), step + "Recent file list is not empty!!");
			}
			#endregion
		}

		#endregion

		#region Tests > TestRecentInCaseOfManualSave
		//------------------------------------------------------------------------------------------
		// Tests > TestRecentInCaseOfManualSave
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  File saved as.
		/// </summary>
		[Test]
		public virtual void TestRecentInCaseOfManualSave()
		{
			bool success = false;
			string step = "";

			ApplicationSettings.LocalUserSettings.General.AutoOpenWorkspace = false;
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Clear();

			#region Step 1
			// - Initial start
			// - Create new terminal
			// - Save terminal as
			//   => Recent contains the terminal
			using (Main main = new Main())
			{
				step = "Step 1: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 1 could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, step + "Terminal 1 could not be created!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = terminal.SaveAs(this.normalTerminalFilePath);
				Assert.That(success, Is.True, step + "Terminal 1 could not be saved as!");

				Utilities.VerifyFiles(step, workspace, false, terminal, true, false);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, false, terminal, true, false);

				Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count, Is.EqualTo(1), step + "Wrong number of recent file entries!");
			}
			#endregion

			#region Step 2
			// - Start and request recent terminal
			//   => New workspace with recent terminal
			using (Main main = new Main())
			{
				step = "Step 2: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = main.OpenRecent(1);
				Assert.That(success, Is.True, step + "Recent terminal could not be opened!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, step + "Recent terminal could not be opened!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles(step, workspace, false, terminal, true, false);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, false, terminal, true, false);

				Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count, Is.EqualTo(1), step + "Wrong number of recent file entries!");
			}
			#endregion
		}

		#endregion

		#region Tests > TestDeletedTerminalFile
		//------------------------------------------------------------------------------------------
		// Tests > TestDeletedTerminalFile
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: Formerly saved file gets deleted.
		/// Expected:  Error message upon start.
		/// </summary>
		[Test]
		public virtual void TestDeletedTerminalFile()
		{
			bool success = false;
			string step = "";

			#region Step 1
			// - Initial start
			// - Create new terminal
			// - Save terminal as
			using (Main main = new Main())
			{
				step = "Step 1: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, step + "Terminal could not be created!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = terminal.SaveAs(this.normalTerminalFilePath);
				Assert.That(success, Is.True, step + "Terminal could not be saved as!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, terminal, true, false);
			}
			#endregion

			#region Step 2
			// - Delete the terminal file
			{
				step = "Step 2: ";
				File.Delete(this.normalTerminalFilePath);
			}
			#endregion

			#region Step 3
			// - Subsequent start
			//   => Error message because of deleted file
			using (Main main = new Main())
			{
				step = "Step 3: ";
				int countBefore = this.workspace_MessageInputRequest_No_counter;
				main.WorkspaceOpened += main_WorkspaceOpened_AttachToWorkspace_MessageInputRequest_No;
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");
				main.WorkspaceOpened -= main_WorkspaceOpened_AttachToWorkspace_MessageInputRequest_No;
				int countAfter = this.workspace_MessageInputRequest_No_counter;
				Assert.That(countAfter, Is.Not.EqualTo(countBefore), "Workspace 'MessageInputRequest' was not called!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				workspace.MessageInputRequest -= workspace_MessageInputRequest_No; // Remaining event sink from 'AttachToWorkspace' above.

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true);
			}
			#endregion
		}

		#endregion

		#region Tests > TestDeletedWorkspaceFile
		//------------------------------------------------------------------------------------------
		// Tests > TestDeletedWorkspaceFile
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: Formerly saved file gets deleted.
		/// Expected:  Error message upon start.
		/// </summary>
		[Test]
		public virtual void TestDeletedWorkspaceFile()
		{
			bool success = false;
			string step = "";

			#region Step 1
			// - Initial start
			// - Create new terminal
			// - Save terminal as
			// - Save workspace as
			using (Main main = new Main())
			{
				step = "Step 1: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, step + "Terminal could not be created!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = terminal.SaveAs(this.normalTerminalFilePath);
				Assert.That(success, Is.True, step + "Terminal could not be saved as!");

				success = workspace.SaveAs(this.normalWorkspaceFilePath);
				Assert.That(success, Is.True, step + "Workspace could not be saved as!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, false, terminal, true, false);
			}
			#endregion

			#region Step 2
			// - Delete the workspace file
			{
				step = "Step 2: ";
				File.Delete(this.normalWorkspaceFilePath);
			}
			#endregion

			#region Step 3
			// - Subsequent start
			//   => Error message because of deleted file
			using (Main main = new Main())
			{
				step = "Step 3: ";
				int countBefore = this.main_MessageInputRequest_Cancel_counter;
				main.MessageInputRequest += main_MessageInputRequest_Cancel;
				success = (main.Start() == MainResult.ApplicationStartCancel);
				Assert.That(success, Is.True, step + "Main could be started even though workspace file is missing!");
				main.MessageInputRequest -= main_MessageInputRequest_Cancel;
				int countAfter = this.main_MessageInputRequest_Cancel_counter;
				Assert.That(countAfter, Is.Not.EqualTo(countBefore), "Workspace 'MessageInputRequest' was not called!");
			}
			#endregion
		}

		#endregion

		#region Tests > TestWriteProtection
		//------------------------------------------------------------------------------------------
		// Tests > TestWriteProtection
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  File saved as.
		/// </summary>
		[Test]
		public virtual void TestWriteProtection()
		{
			bool success = false;
			string step = "";
			DateTime workspaceTS;
			DateTime terminalTS;

			#region Preparation
			// - Initial start
			// - Create new terminal
			// - Save terminal as
			// - Save workspace as
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal1;
				Utilities.InitialStart("Preparation: ", main, this.normalWorkspaceFilePath, out workspace, this.normalTerminal1FilePath, out terminal1);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);
			}
			#endregion

			#region Step 1
			// - Make files write-protected
			{
				step = "Step 1: ";
				{
					string filePath = this.normalWorkspaceFilePath;
					File.SetAttributes(filePath, FileAttributes.ReadOnly);
					Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), Is.True, "Workspace file is not write-protected!");
					workspaceTS = File.GetLastWriteTimeUtc(filePath);
				}
				{
					string filePath = this.normalTerminal1FilePath;
					File.SetAttributes(filePath, FileAttributes.ReadOnly);
					Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), Is.True, "Terminal 1 file is not write-protected!");
					terminalTS = File.GetLastWriteTimeUtc(filePath);
				}
			}
			#endregion

			#region Step 2
			// - Subsequent start on write-protected files
			//   => Implicit change to terminal must not be written
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal1;
				Utilities.SubsequentStart("Step 2: ", main, out workspace, out terminal1);

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				terminal1.SettingsRoot.SendText.Command = new Types.Command(Guid.NewGuid().ToString()); // Implicit change.
				Assert.That(terminal1.SettingsRoot.ExplicitHaveChanged, Is.False, step + "Settings have explicitly changed!");

				int countBefore = this.terminal_MessageInputRequest_No_counter;
				terminal1.MessageInputRequest += terminal_MessageInputRequest_No; // Ignore the "remaining event sink" message that will be output during Exit() below.
				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
				int countAfter = this.terminal_MessageInputRequest_No_counter;
				Assert.That(countAfter, Is.Not.EqualTo(countBefore), "Terminal 1 'MessageInputRequest' was not called!");

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), Is.True, "Terminal 1 file is not write-protected!");
				Assert.That((terminalTS == File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp mismatches!");
			}
			#endregion

			#region Step 3
			// - Subsequent start on write-protected files
			//   => Explicit change to terminal must not be written
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal1;
				Utilities.SubsequentStart("Step 3: ", main, out workspace, out terminal1);

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				terminal1.SettingsRoot.UserName = Guid.NewGuid().ToString(); // Explicit change.
				Assert.That(terminal1.SettingsRoot.ExplicitHaveChanged, Is.True, step + "Settings have not explicitly changed!");

				int countBefore = this.terminal_MessageInputRequest_No_counter;
				terminal1.MessageInputRequest += terminal_MessageInputRequest_No; // Ignore the "remaining event sink" message that will be output during Exit() below.
				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
				int countAfter = this.terminal_MessageInputRequest_No_counter;
				Assert.That(countAfter, Is.Not.EqualTo(countBefore), "Terminal 1 'MessageInputRequest' was not called!");

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), Is.True, "Terminal 1 file is not write-protected!");
				Assert.That((terminalTS == File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp mismatches!");
			}
			#endregion

			#region Step 4
			// - Subsequent start on write-protected files
			//   => Explicit change to workspace must not be written
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal1;
				Utilities.SubsequentStart("Step 4: ", main, out workspace, out terminal1);

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.That(terminal2, Is.Not.Null, step + "Terminal 2 could not be created!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				int workspaceCountBefore = this.workspace_MessageInputRequest_No_counter;
				workspace.MessageInputRequest += workspace_MessageInputRequest_No; // Ignore the "remaining event sink" message that will be output during Exit() below.
				int terminalCountBefore = this.terminal_SaveAsFileDialogRequest_No_counter;
				terminal2.SaveAsFileDialogRequest += terminal_SaveAsFileDialogRequest_No; // Ignore the "remaining event sink" message that will be output during Exit() below.
				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
				int terminalCountAfter = this.terminal_SaveAsFileDialogRequest_No_counter;
				Assert.That(terminalCountAfter, Is.Not.EqualTo(terminalCountBefore), "Terminal 2 'SaveAsFileDialogRequest' was not called!");
				int workspaceCountAfter = this.workspace_MessageInputRequest_No_counter;
				Assert.That(workspaceCountAfter, Is.Not.EqualTo(workspaceCountBefore), "Workspace 'MessageInputRequest' was not called!");

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				string filePath = this.normalWorkspaceFilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), Is.True, "Workspace file is not write-protected!");
				Assert.That((workspaceTS == File.GetLastWriteTimeUtc(filePath)), Is.True, "Workspace file time stamp mismatches!");
			}
			#endregion

			#region Step 5
			// - Make files writable again and then perform steps 2 through 4 again
			{
				step = "Step 5: ";
				{
					string filePath = this.normalWorkspaceFilePath;
					File.SetAttributes(filePath, FileAttributes.Normal);
					Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), Is.True, "Workspace file is not writable!");
				}
				{
					string filePath = this.normalTerminal1FilePath;
					File.SetAttributes(filePath, FileAttributes.Normal);
					Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), Is.True, "Terminal 1 file is not writable!");
				}
			}
			#endregion

			#region Step 6
			// - Subsequent start on writable files
			//   => Implicit change to terminal must be written again
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal1;
				Utilities.SubsequentStart("Step 6: ", main, out workspace, out terminal1);

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				terminal1.SettingsRoot.SendText.Command = new Types.Command(Guid.NewGuid().ToString()); // Implicit change.
				Assert.That(terminal1.SettingsRoot.ExplicitHaveChanged, Is.False, step + "Settings have explicitly changed!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), Is.True, "Terminal 1 file is not writable!");
				Assert.That((terminalTS != File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp still unchanged!");
			}
			#endregion

			#region Step 7
			// - Subsequent start on writable files
			//   => Explicit change to terminal must be written again
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal1;
				Utilities.SubsequentStart("Step 7: ", main, out workspace, out terminal1);

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				terminal1.SettingsRoot.UserName = Guid.NewGuid().ToString(); // Explicit change.
				Assert.That(terminal1.SettingsRoot.ExplicitHaveChanged, Is.True, step + "Settings have not explicitly changed!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), Is.True, "Terminal 1 file is not writable!");
				Assert.That((workspaceTS != File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp still unchanged!");
			}
			#endregion

			#region Step 8
			// - Subsequent start on writable files
			//   => Explicit change to workspace must be written again
			using (Main main = new Main())
			{
				Workspace workspace;
				Terminal terminal1;
				Utilities.SubsequentStart("Step 8: ", main, out workspace, out terminal1);

				Utilities.VerifyFiles(step, workspace, true, false, terminal1, true, false);

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.That(terminal2, Is.Not.Null, step + "Terminal 2 could not be created!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				success = terminal2.SaveAs(this.normalTerminal2FilePath);
				Assert.That(success, Is.True, step + "Terminal could not be saved as!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					step,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);

				string filePath = this.normalWorkspaceFilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), Is.True, "Workspace file is not writable!");
				Assert.That((workspaceTS != File.GetLastWriteTimeUtc(filePath)), Is.True, "Workspace file time stamp still unchanged!");
			}
			#endregion

			#region Step 9
			// - Final start for verification
			using (Main main = new Main())
			{
				step = "Step 9: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
			}
			#endregion
		}

		#endregion

		#region Tests > TestIndicesWithinWorkspace
		//------------------------------------------------------------------------------------------
		// Tests > TestIndicesWithinWorkspace
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// This test verifies the 'DynamicIndex', 'SequentialIndex' and 'FixedIndex' indices within a workspace.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		[Test]
		public virtual void TestIndicesWithinWorkspace()
		{
			bool success = false;
			string step = "";

			#region Step 1
			// - Initial start
			// - Create new terminal
			// - Save terminal as
			// - Save workspace as
			//   => Workspace must contain 1 terminal with fixed index 1
			Terminal.ResetSequentialIndexCounter();
			using (Main main = new Main())
			{
				step = "Step 1: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 1 could not be created!");
				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.That(terminal1, Is.Not.Null, step + "Terminal 1 could not be created!");

				success = terminal1.SaveAs(this.normalTerminal1FilePath);
				Assert.That(success, Is.True, step + "Terminal 1 could not be saved as!");

				success = workspace.SaveAs(this.normalWorkspaceFilePath);
				Assert.That(success, Is.True, step + "Workspace could not be saved as!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
			}
			#endregion

			#region Step 2
			// - Subsequent start
			//   => Workspace must contain 1 terminal with fixed index 1
			// - Create 2 additional terminals
			// - Save terminals as
			// - Save workspace
			//   => Workspace must contain 3 terminals with fixed indices 1, 2 and 3
			Terminal.ResetSequentialIndexCounter();
			using (Main main = new Main())
			{
				step = "Step 2: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace,               Is.Not.Null,   step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), step + "Workspace doesn't contain 1 terminal!");

				Assert.That(workspace.ActiveTerminalFixedIndex,      Is.EqualTo(Indices.FirstFixedIndex),   step + "Fixed index of terminal 1 isn't "   + Indices.FirstFixedIndex + "!");
				Assert.That(workspace.ActiveTerminalDynamicIndex,    Is.EqualTo(Indices.FirstDynamicIndex), step + "Dynamic index of terminal 1 isn't " + Indices.FirstDynamicIndex + "!");
				Assert.That(workspace.ActiveTerminalSequentialIndex, Is.EqualTo(1),                         step + "Sequential index of terminal 1 isn't 1!");

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success,   Is.True,     step + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.That(terminal2, Is.Not.Null, step + "Terminal 2 could not be created!");

				success = terminal2.SaveAs(this.normalTerminal2FilePath);
				Assert.That(success,   Is.True,      step + "Terminal 2 could not be saved as!");

				success = workspace.CreateNewTerminal(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.That(success,   Is.True,     step + "Terminal 3 could not be created!");
				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.That(terminal3, Is.Not.Null, step + "Terminal 3 could not be created!");

				success = terminal3.SaveAs(this.normalTerminal3FilePath);
				Assert.That(success,   Is.True,     step + "Terminal 3 could not be saved as!");

				success = workspace.Save();
				Assert.That(success, Is.True, step + "Workspace could not be saved!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
			}
			#endregion

			#region Step 3
			// - Subsequent start
			//   => Workspace must contain 3 terminals with fixed indices 1, 2 and 3
			// - Close the second terminal
			// - Save workspace
			//   => Workspace must contain 2 terminals with fixed indices 1 and 3
			Terminal.ResetSequentialIndexCounter();
			using (Main main = new Main())
			{
				step = "Step 3: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace,               Is.Not.Null,   step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(3), step + "Workspace doesn't contain 3 terminals!");

				int first = Indices.FirstSequentialIndex;
				int last  = Indices.FirstSequentialIndex + workspace.TerminalCount - 1;
				for (int i = first; i <= last; i++)
				{
					workspace.ActivateTerminalBySequentialIndex(i);
					Assert.That(workspace.ActiveTerminalFixedIndex,      Is.EqualTo(i), step + "Fixed index of terminal "      + i + " isn't " + i + "!");
					Assert.That(workspace.ActiveTerminalDynamicIndex,    Is.EqualTo(i), step + "Dynamic index of terminal "    + i + " isn't " + i + "!");
					Assert.That(workspace.ActiveTerminalSequentialIndex, Is.EqualTo(i), step + "Sequential index of terminal " + i + " isn't " + i + "!");
				}

				workspace.ActivateTerminalBySequentialIndex(2);
				success = workspace.CloseActiveTerminal();
				Assert.That(success,                 Is.True,       step + "Terminal 2 could not be closed!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				success = workspace.Save();
				Assert.That(success, Is.True, step + "Workspace could not be saved!");
				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
			}
			#endregion

			#region Step 4
			// - Subsequent start
			//   => Workspace must contain 2 terminals with fixed indices 1 and 3
			Terminal.ResetSequentialIndexCounter();
			using (Main main = new Main())
			{
				step = "Step 4: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace,               Is.Not.Null,   step + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				workspace.ActivateTerminalBySequentialIndex(1);
				Assert.That(workspace.ActiveTerminalFixedIndex,      Is.EqualTo(Indices.FirstFixedIndex),   step + "Fixed index of terminal 1 isn't "   + Indices.FirstFixedIndex + "!");
				Assert.That(workspace.ActiveTerminalDynamicIndex,    Is.EqualTo(Indices.FirstDynamicIndex), step + "Dynamic index of terminal 1 isn't " + Indices.FirstDynamicIndex + "!");
				Assert.That(workspace.ActiveTerminalSequentialIndex, Is.EqualTo(1),                         step + "Sequential index of terminal 1 isn't 1!");

				workspace.ActivateTerminalBySequentialIndex(2);
				Assert.That(workspace.ActiveTerminalFixedIndex,      Is.EqualTo(3), step + "Fixed index of terminal 3 isn't 3!");
				Assert.That(workspace.ActiveTerminalDynamicIndex,    Is.EqualTo(2), step + "Dynamic index of terminal 3 isn't 2!");
				Assert.That(workspace.ActiveTerminalSequentialIndex, Is.EqualTo(2), step + "Sequential index of terminal 3 isn't 2!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
			}
			#endregion
		}

		#endregion

		#endregion

		#region Event Handlers
		//==========================================================================================
		// Event Handlers
		//==========================================================================================

		/// <remarks>Counter can be used to assert that handler indeed was called.</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int main_MessageInputRequest_Cancel_counter; // = 0;

		private void main_MessageInputRequest_Cancel(object sender, MessageInputEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.Cancel;
			this.main_MessageInputRequest_Cancel_counter++;
		}

		/// <remarks>Counter can be used to assert that handler indeed was called.</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int main_WorkspaceOpened_AttachToWorkspace_MessageInputRequest_No_counter; // = 0;

		private void main_WorkspaceOpened_AttachToWorkspace_MessageInputRequest_No(object sender, EventArgs<Workspace> e)
		{
			e.Value.MessageInputRequest += workspace_MessageInputRequest_No;
			this.main_WorkspaceOpened_AttachToWorkspace_MessageInputRequest_No_counter++;
		}

		/// <remarks>Counter can be used to assert that handler indeed was called.</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int workspace_MessageInputRequest_No_counter; // = 0;

		private void workspace_MessageInputRequest_No(object sender, MessageInputEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.No;
			this.workspace_MessageInputRequest_No_counter++;
		}

		/// <remarks>Counter can be used to assert that handler indeed was called.</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int terminal_MessageInputRequest_No_counter; // = 0;

		private void terminal_MessageInputRequest_No(object sender, MessageInputEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.No;
			this.terminal_MessageInputRequest_No_counter++;
		}

		/// <remarks>Counter can be used to assert that handler indeed was called.</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int terminal_SaveAsFileDialogRequest_No_counter; // = 0;

		private void terminal_SaveAsFileDialogRequest_No(object sender, DialogEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.No;
			this.terminal_SaveAsFileDialogRequest_No_counter++;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
