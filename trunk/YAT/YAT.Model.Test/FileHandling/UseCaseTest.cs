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
// YAT 2.0 Delta Version 1.99.80
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
	public class UseCaseTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private readonly string normalWorkspaceFilePath = Temp.MakeTempFilePath(typeof(UseCaseTest), "NormalWorkspace", ExtensionHelper.WorkspaceFile);
		private readonly string normalTerminal1FilePath = Temp.MakeTempFilePath(typeof(UseCaseTest), "NormalTerminal1", ExtensionHelper.TerminalFile);
		private readonly string normalTerminal2FilePath = Temp.MakeTempFilePath(typeof(UseCaseTest), "NormalTerminal2", ExtensionHelper.TerminalFile);
		private readonly string normalTerminal3FilePath = Temp.MakeTempFilePath(typeof(UseCaseTest), "NormalTerminal3", ExtensionHelper.TerminalFile);

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

		#region Tests > TestSequenceOfUseCases_1_through_5a_
		//------------------------------------------------------------------------------------------
		// Tests > TestSequenceOfUseCases_1_through_5a_
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Use cases according to ufi.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'ufi' is 'ufi', isn't he?")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Naming for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "through", Justification = "Naming for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "a", Justification = "Naming for improved readability.")]
		[Test]
		public virtual void TestSequenceOfUseCases_1_through_5a_()
		{
			bool success = false;
			string uc = "";

			#region Use case 1
			// - Initial start
			// - Create new terminal
			//   => Auto workspace with 1 auto terminal

			using (Main main = new Main())
			{
				uc = "UC1: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), uc + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = workspace.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, uc + "Terminal could not be created!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, uc + "Terminal could not be created!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles(uc, workspace, true, terminal, true);
			}
			#endregion

			#region Use case 2
			// - Start with auto workspace that contains 1 auto terminal
			// - Save terminal
			//   => Auto terminal stays auto

			using (Main main = new Main())
			{
				uc = "UC2: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), uc + "Workspace doesn't contain 1 terminal!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, uc + "Terminal not opened from file!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles(uc, workspace, true, terminal, true);

				success = terminal.Save();
				Assert.That(success, Is.True, uc + "Terminal could not be saved!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), uc + "Workspace doesn't contain 1 terminal!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles(uc, workspace, true, terminal, true);
			}
			#endregion

			#region Use case 2a
			// - Start with auto workspace that contains 1 auto terminal
			// - Save terminal as
			//   => Auto terminal becomes normal terminal

			using (Main main = new Main())
			{
				uc = "UC2a: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), uc + "Workspace doesn't contain 1 terminal!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, uc + "Terminal not opened from file!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles(uc, workspace, true, terminal, true);

				string defaultTerminal1FilePath = terminal.SettingsFilePath;
				success = terminal.SaveAs(this.normalTerminal1FilePath);
				Assert.That(success, Is.True, uc + "Terminal could not be saved as!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), uc + "Workspace doesn't contain 1 terminal!");
				Assert.That(File.Exists(defaultTerminal1FilePath), Is.False, uc + "Auto terminal file not deleted!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles(uc, workspace, true, terminal, true, false);
			}
			#endregion

			#region Use case 3
			// - Start with auto workspace that contains 1 normal terminal
			// - Create 2 more terminals
			//   => Auto workspace with 1 normal and 2 auto terminals

			using (Main main = new Main())
			{
				uc = "UC3: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), uc + "Workspace doesn't contain 1 terminal!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 not opened from file!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles(uc, workspace, true, terminal1, true, false);
				Assert.That(PathEx.Equals(terminal1.SettingsFilePath, this.normalTerminal1FilePath), uc + "Terminal 1 is not stored at user terminal 1 location!");
				var normalTerminal1LastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);

				success = workspace.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, uc + "Terminal 2 could not be created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");

				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 could not be created!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Assert.That(workspace.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler()), Is.True, "Terminal 3 could not be created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(3), uc + "Workspace doesn't contain 3 terminals!");

				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.That(terminal3, Is.Not.Null, uc + "Terminal 3 could not be created!");
				terminal3.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
					(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     true,      true      }  // Auto.
					);

				var normalTerminal1LastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);
				Assert.That(normalTerminal1LastWriteTimeAfterExit, Is.EqualTo(normalTerminal1LastWriteTimeInitially));
			}
			#endregion

			#region Use case 3a
			// - Start with auto workspace that contains 1 normal and 2 auto terminals
			// - Close terminal 3
			//   => Auto workspace with 1 normal and 1 auto terminal

			using (Main main = new Main())
			{
				uc = "UC3a: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(3), uc + "Workspace doesn't contain 3 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal1 = workspace.Terminals[0];
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 not opened from file!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Terminal terminal2 = workspace.Terminals[1];
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 not opened from file!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Terminal terminal3 = workspace.Terminals[2];
				Assert.That(terminal3, Is.Not.Null, uc + "Terminal 3 not opened from file!");
				terminal3.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     true,      true      }  // Auto.
				);

				var normalTerminal1LastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);

				string autoTerminal3FilePath = terminal3.SettingsFilePath;
				success = terminal3.Close();
				Assert.That(success, Is.True, uc + "Terminal 3 could not be closed!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");
				Assert.That(File.Exists(autoTerminal3FilePath), Is.False, uc + "Auto terminal 3 file not deleted!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     true      }  // Auto.
				);

				var normalTerminal1LastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);
				Assert.That(normalTerminal1LastWriteTimeAfterExit, Is.EqualTo(normalTerminal1LastWriteTimeInitially));
			}
			#endregion

			#region Use case 4
			// - Start with auto workspace that contains 1 normal and 1 auto terminal
			// - Save workspace
			//   => Auto workspace stays auto workspace

			using (Main main = new Main())
			{
				uc = "UC4: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal1 = workspace.Terminals[0];
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 not opened from file!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Terminal terminal2 = workspace.Terminals[1];
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 not opened from file!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     true      }  // Auto.
				);

				var normalTerminal1LastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);
				var normalTerminal2LastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminal2FilePath);

				success = workspace.Save();
				Assert.That(success, Is.True, uc + "Workspace could not be saved!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     true      }  // Auto.
				);

				var normalTerminal1LastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);
				Assert.That(normalTerminal1LastWriteTimeAfterExit, Is.EqualTo(normalTerminal1LastWriteTimeInitially));
				var normalTerminal2LastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminal2FilePath);
				Assert.That(normalTerminal2LastWriteTimeAfterExit, Is.EqualTo(normalTerminal2LastWriteTimeInitially));
			}
			#endregion

			#region Use case 4a
			// - Start with auto workspace that contains 1 normal and 1 auto terminal
			// - Save workspace as
			//   => Auto workspace becomes normal workspace
			//   => Auto terminal becomes normal terminal

			using (Main main = new Main())
			{
				uc = "UC4a: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal1 = workspace.Terminals[0];
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 not opened from file!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Terminal terminal2 = workspace.Terminals[1];
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 not opened from file!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     true      }  // Auto.
				);

				string autoWorkspaceFilePath = workspace.SettingsFilePath;
				string autoTerminal2FilePath = terminal2.SettingsFilePath;
				terminal2.SaveAsFileDialogRequest += terminal2_SaveAsFileDialogRequest_SaveAsOK;
				success = workspace.SaveAs(this.normalWorkspaceFilePath);
				terminal2.SaveAsFileDialogRequest -= terminal2_SaveAsFileDialogRequest_SaveAsOK;
				Assert.That(success, Is.True, uc + "Workspace could not be saved as!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");
				Assert.That(File.Exists(autoWorkspaceFilePath), Is.False, uc + "Auto workspace file not deleted!");
				Assert.That(File.Exists(autoTerminal2FilePath), Is.False, uc + "Auto terminal 2 file not deleted!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);

				var normalTerminal1LastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);
				var normalTerminal2LastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminal2FilePath);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);

				var normalTerminal1LastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminal1FilePath);
				Assert.That(normalTerminal1LastWriteTimeAfterExit, Is.EqualTo(normalTerminal1LastWriteTimeInitially));
				var normalTerminal2LastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminal2FilePath);
				Assert.That(normalTerminal2LastWriteTimeAfterExit, Is.EqualTo(normalTerminal2LastWriteTimeInitially));
			}
			#endregion

			#region Use case 5
			// - Start with normal workspace that contains 2 normal terminals
			// - Create new terminal
			//   => Terminal becomes normal terminal

			using (Main main = new Main())
			{
				uc = "UC5: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal1 = workspace.Terminals[0];
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 not opened from file!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Terminal terminal2 = workspace.Terminals[1];
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 not opened from file!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);

				success = workspace.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, uc + "Terminal 3 could not be created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(3), uc + "Workspace doesn't contain 3 terminals!");

				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.That(terminal3, Is.Not.Null, uc + "Terminal 3 could not be created!");
				terminal3.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				terminal3.SaveAsFileDialogRequest += terminal3_SaveAsFileDialogRequest_SaveAsOK; // Ignore the "remaining event sink" message that will be output during Exit() below.
				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     false,     false     }  // Auto.
				);
			}
			#endregion

			#region Use case 5a
			// - Start with normal workspace that contains 3 normal terminals
			// - Close terminal 3
			//   => Normal workspace contains 2 normal terminals but terminal 3 file stays

			using (Main main = new Main())
			{
				uc = "UC5a: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not opened from file!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(3), uc + "Workspace doesn't contain 3 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal1 = workspace.Terminals[0];
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 not opened from file!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Terminal terminal2 = workspace.Terminals[1];
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 not opened from file!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
				Terminal terminal3 = workspace.Terminals[2];
				Assert.That(terminal3, Is.Not.Null, uc + "Terminal 3 not opened from file!");
				terminal3.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     false,     false     }  // Auto.
				);

				var normalTerminal3LastWriteTimeInitially = File.GetLastWriteTimeUtc(this.normalTerminal3FilePath);

				success = terminal3.Close();
				Assert.That(success, Is.True, uc + "Terminal 3 could not be closed!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     false,     false     }  // Auto.
				);

				var normalTerminal3LastWriteTimeAfterExit = File.GetLastWriteTimeUtc(this.normalTerminal3FilePath);
				Assert.That(normalTerminal3LastWriteTimeAfterExit, Is.EqualTo(normalTerminal3LastWriteTimeInitially));
			}
			#endregion
		}

		#endregion

		#region Tests > TestSequenceOfUseCases_6_through_9_
		//------------------------------------------------------------------------------------------
		// Tests > TestSequenceOfUseCases_6_through_9_
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Use cases according to ufi.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'ufi' is 'ufi', isn't he?")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Naming for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "through", Justification = "Naming for improved readability.")]
		[Test]
		public virtual void TestSequenceOfUseCases_6_through_9_()
		{
			bool success = false;
			string uc = "";

			#region Preparation
			// - Initial start
			// - Create two new terminals
			// - Save terminals as
			//   => Auto workspace with 2 normal terminals

			using (Main main = new Main())
			{
				uc = "UC6..9prep: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(0), uc + "Workspace doesn't contain 0 terminals!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = workspace.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, uc + "Terminal 1 could not be created!");
				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 could not be created!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = terminal1.SaveAs(this.normalTerminal1FilePath);
				Assert.That(success, Is.True, uc + "Terminal 1 could not be saved as!");

				success = workspace.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, uc + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 could not be created!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = terminal2.SaveAs(this.normalTerminal2FilePath);
				Assert.That(success, Is.True, uc + "Terminal 2 could not be saved as!");

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);
			}
			#endregion

			#region Use case 6 using absolute and relative paths

			// Using absolute path:
			success = UseCase6and6a("UC6: ", this.normalTerminal1FilePath);

			// Using relative path:
			string currentDirectoryToRestore = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(this.normalTerminal1FilePath);
				success = UseCase6and6a("UC6a: ", Path.GetFileName(this.normalTerminal1FilePath));
			}
			finally
			{
				Environment.CurrentDirectory = currentDirectoryToRestore;
			}

			#endregion

			#region Use case 7
			// - Start and request normal terminal
			// - Create another terminal
			//   => Auto workspace with 1 normal and 1 auto terminal

			using (Main main = new Main(this.normalTerminal1FilePath))
			{
				uc = "UC7: ";
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), uc + "Workspace doesn't contain 1 terminal!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 could not be opened!");
				terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Utilities.VerifyFiles(uc, workspace, false, terminal1, true, false);

				success = workspace.CreateNewTerminal(Utilities.GetStartedTcpAutoSocketOnIPv4LoopbackTextSettingsHandler());
				Assert.That(success, Is.True, uc + "Terminal 2 could not be created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(2), uc + "Workspace doesn't contain 2 terminals!");

				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 could not be created!");
				terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     true      }  // Auto.
				);
			}
			#endregion

			#region Use case 8
			// - Start and request normal terminal
			// - Start and request normal terminal in another main
			// - Close first main, then second
			//   => 2 auto workspaces with 1 normal terminal each

			using (Main main1 = new Main(this.normalTerminal1FilePath),
			            main2 = new Main(this.normalTerminal2FilePath))
			{
				uc = "UC8: ";

				Workspace workspace1;
				Workspace workspace2;

				Terminal terminal1;
				Terminal terminal2;

				// Main 1 start:
				{
					success = (main1.Start() == MainResult.Success);
					Assert.That(success, Is.True, uc + "Main 1 could not be started!");

					workspace1 = main1.Workspace;
					Assert.That(workspace1, Is.Not.Null, uc + "Workspace 1 not created!");
					workspace1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
					Assert.That(workspace1.TerminalCount, Is.EqualTo(1), uc + "Workspace 1 doesn't contain 1 terminal!");

					terminal1 = workspace1.ActiveTerminal;
					Assert.That(terminal1, Is.Not.Null, uc + "Terminal 1 could not be opened!");
					terminal1.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

					Utilities.VerifyFiles(uc, workspace1, false, terminal1, true, false);
				}

				// Main 2 start:
				{
					success = (main2.Start() == MainResult.Success);
					Assert.That(success, Is.True, uc + "Main 2 could not be started!");

					workspace2 = main2.Workspace;
					Assert.That(workspace2, Is.Not.Null, uc + "Workspace 2 not created!");
					workspace2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
					Assert.That(workspace2.TerminalCount, Is.EqualTo(1), uc + "Workspace 2 doesn't contain 1 terminal!");

					terminal2 = workspace2.ActiveTerminal;
					Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 could not be opened!");
					terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

					Utilities.VerifyFiles(uc, workspace2, false, terminal2, true, false);
				}

				// Main 1 exit:
				{
					success = (main1.Exit() == MainResult.Success);
					Assert.That(success, Is.True, uc + "Main 1 could not be exited!");

					Utilities.VerifyFiles(uc, workspace1, true, terminal1, true, false);
				}

				// Main 2 exit:
				{
					success = (main2.Exit() == MainResult.Success);
					Assert.That(success, Is.True, uc + "Main 2 could not be exited!");

					Utilities.VerifyFiles(uc, workspace2, true, terminal2, true, false);
				}
			}
			#endregion

			#region Use case 9
			// - Start with auto workspace that contains 1 normal terminal
			//   => 1 auto workspaces with 1 normal terminal

			using (Main main2 = new Main())
			{
				uc = "UC9: ";

				Workspace workspace2;

				Terminal terminal2;

				// Main 2 start:
				{
					success = (main2.Start() == MainResult.Success);
					Assert.That(success, Is.True, uc + "Main 2 could not be started!");

					workspace2 = main2.Workspace;
					Assert.That(workspace2, Is.Not.Null, uc + "Workspace 2 not created!");
					workspace2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;
					Assert.That(workspace2.TerminalCount, Is.EqualTo(1), uc + "Workspace 2 doesn't contain 1 terminal!");

					terminal2 = workspace2.ActiveTerminal;
					Assert.That(terminal2, Is.Not.Null, uc + "Terminal 2 could not be opened!");
					terminal2.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

					Utilities.VerifyFiles(uc, workspace2, true, terminal2, true, false);
				}

				// Main 2 exit:
				{
					success = (main2.Exit() == MainResult.Success);
					Assert.That(success, Is.True, uc + "Main 2 could not be exited!");

					Utilities.VerifyFiles(uc, workspace2, true, terminal2, true, false);
				}
			}
			#endregion
		}

		#region Use case 6 implementation

		private static bool UseCase6and6a(string uc, string requestedPath)
		{
			bool success = false;

			using (Main main = new Main(requestedPath))
			{
				success = (main.Start() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.That(workspace, Is.Not.Null, uc + "Workspace not created!");
				Assert.That(workspace.TerminalCount, Is.EqualTo(1), uc + "Workspace doesn't contain 1 terminal!");
				workspace.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				Terminal terminal = workspace.ActiveTerminal;
				Assert.That(terminal, Is.Not.Null, uc + "Terminal could not be opened!");
				terminal.DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification = true;

				// The auto workspace may still be set to some other workspace. Keep it to compare below.
				string formerLocalUserAutoWorkspaceFilePath = ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath;
				Utilities.VerifyFiles(uc, workspace, false, terminal, true, false);

				success = (main.Exit() == MainResult.Success);
				Assert.That(success, Is.True, uc + "Main could not be exited successfully!");

				Utilities.VerifyFiles(uc, workspace, true, terminal, true, false);

				// Ensure that the auto workspace has been changed.
				string currentLocalUserAutoWorkspaceFilePath = ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath;
				Assert.That(currentLocalUserAutoWorkspaceFilePath, Is.Not.EqualTo(formerLocalUserAutoWorkspaceFilePath), uc + "Auto workspace not created new!");
			}

			return (success);
		}

		#endregion

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

		private void terminal2_SaveAsFileDialogRequest_SaveAsOK(object sender, DialogEventArgs e)
		{
			Terminal terminal = (sender as Terminal);
			Assert.That(terminal.SaveAs(this.normalTerminal2FilePath), Is.True, "Terminal 2 could not be saved as!");
			e.Result = System.Windows.Forms.DialogResult.OK;
		}

		private void terminal3_SaveAsFileDialogRequest_SaveAsOK(object sender, DialogEventArgs e)
		{
			Terminal terminal = (sender as Terminal);
			Assert.That(terminal.SaveAs(this.normalTerminal3FilePath), Is.True, "Terminal 3 could not be saved as!");
			e.Result = System.Windows.Forms.DialogResult.OK;
		}

		private void terminal_SaveAsFileDialogRequest_No(object sender, DialogEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.No;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
