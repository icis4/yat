﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Epsilon Version 1.99.90
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
			using (var m = new Main())
			{
				Workspace w;
				Terminal t;
				Utilities.InitialStart("Preparation: ", m, out w, this.normalTerminalFilePath, out t);

				terminalLastWriteTimeInitially = File.GetLastWriteTimeUtc(t.SettingsFilePath);

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion

			#region Step 1
			// - Subsequent start
			// - No changes on terminal settings
			using (var m = new Main())
			{
				Workspace w;
				Terminal t;
				Utilities.SubsequentStart("Step 1: ", m, out w, out t);

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

				var normalTerminalLastWriteTimeAfterStart = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(normalTerminalLastWriteTimeAfterStart, Is.EqualTo(terminalLastWriteTimeInitially));

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

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
			using (var m = new Main())
			{
				Workspace w;
				Terminal t;
				Utilities.InitialStart("Preparation: ", m, out w, this.normalTerminalFilePath, out t);

				terminalLastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion

			#region Step 1
			// - Subsequent start
			// - Implicit change on terminal settings
			using (var m = new Main())
			{
				Workspace w;
				Terminal t;
				Utilities.SubsequentStart("Step 1: ", m, out w, out t);

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

				var normalTerminalLastWriteTimeAfterStart = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(normalTerminalLastWriteTimeAfterStart, Is.EqualTo(terminalLastWriteTimeInitially));

				t.SettingsRoot.SendText.Command = new Types.Command(Guid.NewGuid().ToString()); // Implicit change.
				Assert.That(t.SettingsRoot.ExplicitHaveChanged, Is.False, step + "Settings have explicitly changed!");

				var terminalLastWriteTimeAfterChange = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterChange, Is.EqualTo(terminalLastWriteTimeInitially));

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

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
			using (var m = new Main())
			{
				Workspace w;
				Terminal t;
				Utilities.InitialStart("Preparation: ", m, out w, this.normalTerminalFilePath, out t);

				terminalLastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

				var terminalLastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterExit, Is.EqualTo(terminalLastWriteTimeInitially));
			}
			#endregion

			#region Step 1
			// - Subsequent start
			// - Explicit change on terminal settings
			using (var m = new Main())
			{
				Workspace w;
				Terminal t;
				Utilities.SubsequentStart("Step 1: ", m, out w, out t);

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

				var normalTerminalLastWriteTimeAfterStart = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(normalTerminalLastWriteTimeAfterStart, Is.EqualTo(terminalLastWriteTimeInitially));

				t.SettingsRoot.UserName = Guid.NewGuid().ToString(); // Explicit change.
				Assert.That(t.SettingsRoot.ExplicitHaveChanged, Is.True, step + "Settings have not explicitly changed!");

				var terminalLastWriteTimeAfterChange = File.GetLastWriteTimeUtc(this.normalTerminalFilePath);
				Assert.That(terminalLastWriteTimeAfterChange, Is.EqualTo(terminalLastWriteTimeInitially));

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, true, t, true, false);

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
			using (var m = new Main())
			{
				step = "Step 1: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				w.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal could not be created!");

				var t = w.ActiveTerminal;
				Assert.That(t, Is.Not.Null, step + "Terminal could not be created!");
				t.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, t, true);

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
			using (var m = new Main())
			{
				step = "Step 1: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				w.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 1 could not be created!");
				var t = w.ActiveTerminal;
				Assert.That(t, Is.Not.Null, step + "Terminal 1 could not be created!");
				t.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = t.SaveAs(this.normalTerminalFilePath);
				Assert.That(success, Is.True, step + "Terminal 1 could not be saved as!");

				Utilities.VerifyFiles(step, w, false, t, true, false);

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, false, t, true, false);

				Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count, Is.EqualTo(1), step + "Wrong number of recent file entries!");
			}
			#endregion

			#region Step 2
			// - Start and request recent terminal
			//   => New workspace with recent terminal
			using (var m = new Main())
			{
				step = "Step 2: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				w.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = m.OpenRecent(1);
				Assert.That(success, Is.True, step + "Recent terminal could not be opened!");
				var t = w.ActiveTerminal;
				Assert.That(t, Is.Not.Null, step + "Recent terminal could not be opened!");
				t.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles(step, w, false, t, true, false);

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, false, t, true, false);

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
			using (var m = new Main())
			{
				step = "Step 1: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				w.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal could not be created!");
				var t = w.ActiveTerminal;
				Assert.That(t, Is.Not.Null, step + "Terminal could not be created!");
				t.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = t.SaveAs(this.normalTerminalFilePath);
				Assert.That(success, Is.True, step + "Terminal could not be saved as!");

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, t, true, false);
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
			using (var m = new Main())
			{
				step = "Step 3: ";
				int countBefore = this.workspace_MessageInputRequest_No_counter;
				m.WorkspaceOpened += main_WorkspaceOpened_AttachToWorkspace_MessageInputRequest_No;
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");
				m.WorkspaceOpened -= main_WorkspaceOpened_AttachToWorkspace_MessageInputRequest_No;
				int countAfter = this.workspace_MessageInputRequest_No_counter;
				Assert.That(countAfter, Is.Not.EqualTo(countBefore), "Workspace 'MessageInputRequest' was not called!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				w.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				w.MessageInputRequest -= workspace_MessageInputRequest_No; // Remaining event sink from 'AttachToWorkspace' above.

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true);
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
			using (var m = new Main())
			{
				step = "Step 1: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");
				w.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal could not be created!");
				var t = w.ActiveTerminal;
				Assert.That(t, Is.Not.Null, step + "Terminal could not be created!");
				t.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = t.SaveAs(this.normalTerminalFilePath);
				Assert.That(success, Is.True, step + "Terminal could not be saved as!");

				success = w.SaveAs(this.normalWorkspaceFilePath);
				Assert.That(success, Is.True, step + "Workspace could not be saved as!");

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, false, t, true, false);
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
			using (var m = new Main())
			{
				step = "Step 3: ";
				int countBefore = this.main_MessageInputRequest_Cancel_counter;
				m.MessageInputRequest += main_MessageInputRequest_Cancel;
				success = (m.Start() == MainResult.ApplicationStartCancel);
				Assert.That(success, Is.True, step + "Main could be started even though workspace file is missing!");
				m.MessageInputRequest -= main_MessageInputRequest_Cancel;
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
			using (var m = new Main())
			{
				Workspace w;
				Terminal t1;
				Utilities.InitialStart("Preparation: ", m, this.normalWorkspaceFilePath, out w, this.normalTerminal1FilePath, out t1);

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);
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
			using (var m = new Main())
			{
				Workspace w;
				Terminal t1;
				Utilities.SubsequentStart("Step 2: ", m, out w, out t1);

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				t1.SettingsRoot.SendText.Command = new Types.Command(Guid.NewGuid().ToString()); // Implicit change.
				Assert.That(t1.SettingsRoot.ExplicitHaveChanged, Is.False, step + "Settings have explicitly changed!");

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), Is.True, "Terminal 1 file is not write-protected!");
				Assert.That((terminalTS == File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp mismatches!");
			}
			#endregion

			#region Step 3
			// - Subsequent start on write-protected files
			//   => Explicit change to terminal must not be written
			using (var m = new Main())
			{
				Workspace w;
				Terminal t1;
				Utilities.SubsequentStart("Step 3: ", m, out w, out t1);

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				t1.SettingsRoot.UserName = Guid.NewGuid().ToString(); // Explicit change.
				Assert.That(t1.SettingsRoot.ExplicitHaveChanged, Is.True, step + "Settings have not explicitly changed!");

				int countBefore = this.terminal_MessageInputRequest_No_counter;
				t1.MessageInputRequest += terminal_MessageInputRequest_No; // Ignore the "remaining event sink" message that will be output during Exit() below.
				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
				int countAfter = this.terminal_MessageInputRequest_No_counter;
				Assert.That(countAfter, Is.Not.EqualTo(countBefore), "Terminal 1 'MessageInputRequest' was not called!");

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), Is.True, "Terminal 1 file is not write-protected!");
				Assert.That((terminalTS == File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp mismatches!");
			}
			#endregion

			#region Step 4
			// - Subsequent start on write-protected files
			//   => Explicit change to workspace must not be written
			using (var m = new Main())
			{
				Workspace w;
				Terminal t1;
				Utilities.SubsequentStart("Step 4: ", m, out w, out t1);

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 2 could not be created!");
				Terminal t2 = w.ActiveTerminal;
				Assert.That(t2, Is.Not.Null, step + "Terminal 2 could not be created!");
				t2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Assert.That(w.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				int workspaceCountBefore = this.workspace_MessageInputRequest_No_counter;
				w.MessageInputRequest += workspace_MessageInputRequest_No; // Ignore the "remaining event sink" message that will be output during Exit() below.
				int terminalCountBefore = this.terminal_SaveAsFileDialogRequest_No_counter;
				t2.SaveAsFileDialogRequest += terminal_SaveAsFileDialogRequest_No; // Ignore the "remaining event sink" message that will be output during Exit() below.
				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
				int terminalCountAfter = this.terminal_SaveAsFileDialogRequest_No_counter;
				Assert.That(terminalCountAfter, Is.Not.EqualTo(terminalCountBefore), "Terminal 2 'SaveAsFileDialogRequest' was not called!");
				int workspaceCountAfter = this.workspace_MessageInputRequest_No_counter;
				Assert.That(workspaceCountAfter, Is.Not.EqualTo(workspaceCountBefore), "Workspace 'MessageInputRequest' was not called!");

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

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
			using (var m = new Main())
			{
				Workspace w;
				Terminal t1;
				Utilities.SubsequentStart("Step 6: ", m, out w, out t1);

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				t1.SettingsRoot.SendText.Command = new Types.Command(Guid.NewGuid().ToString()); // Implicit change.
				Assert.That(t1.SettingsRoot.ExplicitHaveChanged, Is.False, step + "Settings have explicitly changed!");

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), Is.True, "Terminal 1 file is not writable!");
				Assert.That((terminalTS != File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp still unchanged!");
			}
			#endregion

			#region Step 7
			// - Subsequent start on writable files
			//   => Explicit change to terminal must be written again
			using (var m = new Main())
			{
				Workspace w;
				Terminal t1;
				Utilities.SubsequentStart("Step 7: ", m, out w, out t1);

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				t1.SettingsRoot.UserName = Guid.NewGuid().ToString(); // Explicit change.
				Assert.That(t1.SettingsRoot.ExplicitHaveChanged, Is.True, step + "Settings have not explicitly changed!");

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.That(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), Is.True, "Terminal 1 file is not writable!");
				Assert.That((workspaceTS != File.GetLastWriteTimeUtc(filePath)), Is.True, "Terminal 1 file time stamp still unchanged!");
			}
			#endregion

			#region Step 8
			// - Subsequent start on writable files
			//   => Explicit change to workspace must be written again
			using (var m = new Main())
			{
				Workspace w;
				Terminal t1;
				Utilities.SubsequentStart("Step 8: ", m, out w, out t1);

				Utilities.VerifyFiles(step, w, true, false, t1, true, false);

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 2 could not be created!");
				Terminal t2 = w.ActiveTerminal;
				Assert.That(t2, Is.Not.Null, step + "Terminal 2 could not be created!");
				t2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Assert.That(w.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				success = t2.SaveAs(this.normalTerminal2FilePath);
				Assert.That(success, Is.True, step + "Terminal could not be saved as!");

				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					step,
					w,
					true,
					false,
					new Terminal[] { t1, t2 },
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
			using (var m = new Main())
			{
				step = "Step 9: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				success = (m.Exit() == MainResult.Success);
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
			using (var m = new Main())
			{
				step = "Step 1: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w, Is.Not.Null, step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(0), step + "Workspace doesn't contain 0 terminals!");

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, step + "Terminal 1 could not be created!");
				var t1 = w.ActiveTerminal;
				Assert.That(t1, Is.Not.Null, step + "Terminal 1 could not be created!");

				success = t1.SaveAs(this.normalTerminal1FilePath);
				Assert.That(success, Is.True, step + "Terminal 1 could not be saved as!");

				success = w.SaveAs(this.normalWorkspaceFilePath);
				Assert.That(success, Is.True, step + "Workspace could not be saved as!");

				success = (m.Exit() == MainResult.Success);
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
			using (var m = new Main())
			{
				step = "Step 2: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w,               Is.Not.Null,   step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(1), step + "Workspace doesn't contain 1 terminal!");

				Assert.That(w.ActiveTerminalFixedIndex,      Is.EqualTo(Indices.FirstFixedIndex),   step + "Fixed index of terminal 1 isn't "   + Indices.FirstFixedIndex + "!");
				Assert.That(w.ActiveTerminalDynamicIndex,    Is.EqualTo(Indices.FirstDynamicIndex), step + "Dynamic index of terminal 1 isn't " + Indices.FirstDynamicIndex + "!");
				Assert.That(w.ActiveTerminalSequentialIndex, Is.EqualTo(1),                         step + "Sequential index of terminal 1 isn't 1!");

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success,   Is.True,     step + "Terminal 2 could not be created!");
				var t2 = w.ActiveTerminal;
				Assert.That(t2, Is.Not.Null, step + "Terminal 2 could not be created!");

				success = t2.SaveAs(this.normalTerminal2FilePath);
				Assert.That(success,   Is.True,      step + "Terminal 2 could not be saved as!");

				success = w.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success,   Is.True,     step + "Terminal 3 could not be created!");
				var terminal3 = w.ActiveTerminal;
				Assert.That(terminal3, Is.Not.Null, step + "Terminal 3 could not be created!");

				success = terminal3.SaveAs(this.normalTerminal3FilePath);
				Assert.That(success,   Is.True,     step + "Terminal 3 could not be saved as!");

				success = w.Save();
				Assert.That(success, Is.True, step + "Workspace could not be saved!");

				success = (m.Exit() == MainResult.Success);
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
			using (var m = new Main())
			{
				step = "Step 3: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w,               Is.Not.Null,   step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(3), step + "Workspace doesn't contain 3 terminals!");

				int first = Indices.FirstSequentialIndex;
				int last  = Indices.FirstSequentialIndex + w.TerminalCount - 1;
				for (int i = first; i <= last; i++)
				{
					w.ActivateTerminalBySequentialIndex(i);
					Assert.That(w.ActiveTerminalFixedIndex,      Is.EqualTo(i), step + "Fixed index of terminal "      + i + " isn't " + i + "!");
					Assert.That(w.ActiveTerminalDynamicIndex,    Is.EqualTo(i), step + "Dynamic index of terminal "    + i + " isn't " + i + "!");
					Assert.That(w.ActiveTerminalSequentialIndex, Is.EqualTo(i), step + "Sequential index of terminal " + i + " isn't " + i + "!");
				}

				w.ActivateTerminalBySequentialIndex(2);
				success = w.CloseActiveTerminal();
				Assert.That(success,                 Is.True,       step + "Terminal 2 could not be closed!");
				Assert.That(w.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				success = w.Save();
				Assert.That(success, Is.True, step + "Workspace could not be saved!");
				success = (m.Exit() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be exited successfully!");
			}
			#endregion

			#region Step 4
			// - Subsequent start
			//   => Workspace must contain 2 terminals with fixed indices 1 and 3
			Terminal.ResetSequentialIndexCounter();
			using (var m = new Main())
			{
				step = "Step 4: ";
				success = (m.Start() == MainResult.Success);
				Assert.That(success, Is.True, step + "Main could not be started!");

				var w = m.Workspace;
				Assert.That(w,               Is.Not.Null,   step + "Workspace not created!");
				Assert.That(w.TerminalCount, Is.EqualTo(2), step + "Workspace doesn't contain 2 terminals!");

				w.ActivateTerminalBySequentialIndex(1);
				Assert.That(w.ActiveTerminalFixedIndex,      Is.EqualTo(Indices.FirstFixedIndex),   step + "Fixed index of terminal 1 isn't "   + Indices.FirstFixedIndex + "!");
				Assert.That(w.ActiveTerminalDynamicIndex,    Is.EqualTo(Indices.FirstDynamicIndex), step + "Dynamic index of terminal 1 isn't " + Indices.FirstDynamicIndex + "!");
				Assert.That(w.ActiveTerminalSequentialIndex, Is.EqualTo(1),                         step + "Sequential index of terminal 1 isn't 1!");

				w.ActivateTerminalBySequentialIndex(2);
				Assert.That(w.ActiveTerminalFixedIndex,      Is.EqualTo(3), step + "Fixed index of terminal 3 isn't 3!");
				Assert.That(w.ActiveTerminalDynamicIndex,    Is.EqualTo(2), step + "Dynamic index of terminal 3 isn't 2!");
				Assert.That(w.ActiveTerminalSequentialIndex, Is.EqualTo(2), step + "Sequential index of terminal 3 isn't 2!");

				success = (m.Exit() == MainResult.Success);
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
