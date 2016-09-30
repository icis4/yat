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
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
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

namespace YAT.Model.Test
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1123:DoNotPlaceRegionsWithinElements", Justification = "Group use cases into regions.")]
	[TestFixture]
	public class FileHandlingTest
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool autoOpenWorkspaceToRestore;
		private bool autoSaveWorkspaceToRestore;
		private string workspaceFilePathToRestore;

		private string normalWorkspaceFilePath = Temp.MakeTempFilePath(typeof(FileHandlingTest), "NormalWorkspace", ExtensionHelper.WorkspaceFile);
		private string normalTerminal1FilePath = Temp.MakeTempFilePath(typeof(FileHandlingTest), "NormalTerminal1", ExtensionHelper.TerminalFile);
		private string normalTerminal2FilePath = Temp.MakeTempFilePath(typeof(FileHandlingTest), "NormalTerminal2", ExtensionHelper.TerminalFile);
		private string normalTerminal3FilePath = Temp.MakeTempFilePath(typeof(FileHandlingTest), "NormalTerminal3", ExtensionHelper.TerminalFile);

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

			StartAndCreateDefaultTerminal(out main, out workspace, out terminal);
			
			using (main)
			{
				bool success = false;

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, "Main could not be exited successfully!");

				VerifyFiles(workspace, true, terminal, true);
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

			StartAndCreateDefaultTerminal(out main, out workspace, out terminal);
			
			using (main)
			{
				bool success = false;

				success = workspace.Close();
				Assert.IsTrue(success, "Workspace could not be closed successfully!");

				VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, "Main could not be exited successfully!");

				VerifyFiles(workspace, false, terminal, false);
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

			StartAndCreateDefaultTerminal(out main, out workspace, out terminal);
			
			using (main)
			{
				bool success = false;

				success = terminal.Close();
				Assert.IsTrue(success, "Terminal could not be closed successfully!");

				VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, "Main could not be exited successfully!");

				VerifyFiles(workspace, true, terminal, false);
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

			StartAndCreateDefaultTerminal(out main, out workspace, out terminal);
	
			using (main)
			{
				bool success = false;

				success = terminal.Close();
				Assert.IsTrue(success, "Terminal could not be closed successfully!");

				VerifyFiles(workspace, false, terminal, false);

				success = workspace.Close();
				Assert.IsTrue(success, "Workspace could not be closed successfully!");

				VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, "Main could not be exited successfully!");

				VerifyFiles(workspace, false, terminal, false);
			}
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

			StartAndCreateDefaultTerminal(out main, out workspace, out terminal);

			using (main)
			{
				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, "Main could not be exited successfully!");

				VerifyFiles(workspace, true, terminal, true);
			}

			// Consecutive start with auto delete on close of workspace:

			using (main = new Main())
			{
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, "Main could not be started!");

				workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file!");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal!");

				terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, "Terminal not opened from file!");

				VerifyFiles(workspace, true, terminal, true);

				success = workspace.Close();
				Assert.IsTrue(success, "Workspace could not be closed successfully!");

				VerifyFiles(workspace, false, terminal, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, "Main could not be exited successfully!");

				VerifyFiles(workspace, false, terminal, false);
			}
		}

		#endregion

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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, uc + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, uc + "Terminal could not be created!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, uc + "Terminal could not be created!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles(uc, workspace, true, terminal, true);
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, uc + "Terminal not opened from file!");

				VerifyFiles(uc, workspace, true, terminal, true);

				success = terminal.Save();
				Assert.IsTrue(success, uc + "Terminal could not be saved!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles(uc, workspace, true, terminal, true);
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, uc + "Terminal not opened from file!");

				VerifyFiles(uc, workspace, true, terminal, true);

				string defaultTerminal1FilePath = terminal.SettingsFilePath;
				success = terminal.SaveAs(this.normalTerminal1FilePath);
				Assert.IsTrue(success, uc + "Terminal could not be saved as!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");
				Assert.IsFalse(File.Exists(defaultTerminal1FilePath), uc + "Auto terminal file not deleted!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles(uc, workspace, true, terminal, true, false);
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, uc + "Terminal 1 not opened from file!");

				VerifyFiles(uc, workspace, true, terminal1, true, false);
				StringAssert.AreEqualIgnoringCase(this.normalTerminal1FilePath, terminal1.SettingsFilePath, uc + "Terminal 1 is not stored at user terminal 1 location!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, uc + "Terminal 2 could not be created!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");

				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, uc + "Terminal 2 could not be created!");

				Assert.IsTrue(workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler()), "Terminal 3 could not be created!");
				Assert.AreEqual(3, workspace.TerminalCount, uc + "Workspace doesn't contain 3 terminals!");

				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal3, uc + "Terminal 3 could not be created!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
					(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     true,      true      }  // Auto.
					);
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(3, workspace.TerminalCount, uc + "Workspace doesn't contain 3 terminals!");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, uc + "Terminal 1 not opened from file!");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, uc + "Terminal 2 not opened from file!");
				Terminal terminal3 = workspace.Terminals[2];
				Assert.IsNotNull(terminal3, uc + "Terminal 3 not opened from file!");

				VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     true,      true      }  // Auto.
				);

				string autoTerminal3FilePath = terminal3.SettingsFilePath;
				success = terminal3.Close();
				Assert.IsTrue(success, uc + "Terminal 3 could not be closed!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");
				Assert.IsFalse(File.Exists(autoTerminal3FilePath), uc + "Auto terminal 3 file not deleted!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
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

			#region Use case 4
			// - Start with auto workspace that contains 1 normal and 1 auto terminal
			// - Save workspace
			//   => Auto workspace stays auto workspace
			using (Main main = new Main())
			{
				uc = "UC4: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, uc + "Terminal 1 not opened from file!");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, uc + "Terminal 2 not opened from file!");

				VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     true      }  // Auto.
				);

				success = workspace.Save();
				Assert.IsTrue(success, uc + "Workspace could not be saved!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
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

			#region Use case 4a
			// - Start with auto workspace that contains 1 normal and 1 auto terminal
			// - Save workspace as
			//   => Auto workspace becomes normal workspace
			//   => Auto terminal becomes normal terminal
			using (Main main = new Main())
			{
				uc = "UC4a: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, uc + "Terminal 1 not opened from file!");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, uc + "Terminal 2 not opened from file!");

				VerifyFiles
				(
					uc,
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     true      }  // Auto.
				);

				// Install callback handler that sets the normal file path for terminal 2:
				terminal2.SaveAsFileDialogRequest += terminal2_SaveAsFileDialogRequest_SaveAs;

				string autoWorkspaceFilePath = workspace.SettingsFilePath;
				string autoTerminal2FilePath = terminal2.SettingsFilePath;
				success = workspace.SaveAs(this.normalWorkspaceFilePath);
				Assert.IsTrue(success, uc + "Workspace could not be saved as!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");
				Assert.IsFalse(File.Exists(autoWorkspaceFilePath), uc + "Auto workspace file not deleted!");
				Assert.IsFalse(File.Exists(autoTerminal2FilePath), uc + "Auto terminal 2 file not deleted!");

				VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, uc + "Terminal 1 not opened from file!");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, uc + "Terminal 2 not opened from file!");

				VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // Exists.
					new bool[]     { false,     false     }  // Auto.
				);

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, uc + "Terminal 3 could not be created!");
				Assert.AreEqual(3, workspace.TerminalCount, uc + "Workspace doesn't contain 3 terminals!");

				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal3, uc + "Terminal 3 could not be created!");

				// Install callback handler that sets the normal file path for terminal 3:
				terminal3.SaveAsFileDialogRequest += terminal3_SaveAsFileDialogRequest_SaveAsOK;

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not opened from file!");
				Assert.AreEqual(3, workspace.TerminalCount, uc + "Workspace doesn't contain 3 terminals!");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, uc + "Terminal 1 not opened from file!");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, uc + "Terminal 2 not opened from file!");
				Terminal terminal3 = workspace.Terminals[2];
				Assert.IsNotNull(terminal3, uc + "Terminal 3 not opened from file!");

				VerifyFiles
				(
					uc,
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // Exists.
					new bool[]     { false,     false,     false     }  // Auto.
				);

				success = terminal3.Close();
				Assert.IsTrue(success, uc + "Terminal 3 could not be closed!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, uc + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, uc + "Terminal 1 could not be created!");
				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, uc + "Terminal 1 could not be created!");
				success = terminal1.SaveAs(this.normalTerminal1FilePath);
				Assert.IsTrue(success, uc + "Terminal 1 could not be saved as!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, uc + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, uc + "Terminal 2 could not be created!");
				success = terminal2.SaveAs(this.normalTerminal2FilePath);
				Assert.IsTrue(success, uc + "Terminal 2 could not be saved as!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
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

			#region Use case 6
			// - Start and request normal terminal
			//   => Auto workspace with 1 normal terminal
			using (Main main = new Main(this.normalTerminal1FilePath))
			{
				uc = "UC6: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not created!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, uc + "Terminal could not be opened!");

				// The auto workspace may still be set to some other workspace. Keep it to compare below.
				string formerLocalUserAutoWorkspaceFilePath = ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath;
				VerifyFiles(uc, workspace, false, terminal, true, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles(uc, workspace, true, terminal, true, false);

				// Ensure that the auto workspace has been changed.
				string currentLocalUserAutoWorkspaceFilePath = ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath;
				Assert.AreNotEqual(formerLocalUserAutoWorkspaceFilePath, currentLocalUserAutoWorkspaceFilePath, uc + "Auto workspace not created new!");
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
				Assert.IsTrue(success, uc + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, uc + "Workspace not created!");
				Assert.AreEqual(1, workspace.TerminalCount, uc + "Workspace doesn't contain 1 terminal!");

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, uc + "Terminal 1 could not be opened!");

				VerifyFiles(uc, workspace, false, terminal1, true, false);

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, uc + "Terminal 2 could not be created!");
				Assert.AreEqual(2, workspace.TerminalCount, uc + "Workspace doesn't contain 2 terminals!");

				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, uc + "Terminal 2 could not be created!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, uc + "Main could not be exited successfully!");

				VerifyFiles
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

				// Main 1 start.
				{
					success = (main1.Start() == MainResult.Success);
					Assert.IsTrue(success, uc + "Main 1 could not be started!");

					workspace1 = main1.Workspace;
					Assert.IsNotNull(workspace1, uc + "Workspace 1 not created!");
					Assert.AreEqual(1, workspace1.TerminalCount, uc + "Workspace 1 doesn't contain 1 terminal!");

					terminal1 = workspace1.ActiveTerminal;
					Assert.IsNotNull(terminal1, uc + "Terminal 1 could not be opened!");

					VerifyFiles(uc, workspace1, false, terminal1, true, false);
				}

				// Main 2 start.
				{
					success = (main2.Start() == MainResult.Success);
					Assert.IsTrue(success, uc + "Main 2 could not be started!");

					workspace2 = main2.Workspace;
					Assert.IsNotNull(workspace2, uc + "Workspace 2 not created!");
					Assert.AreEqual(1, workspace2.TerminalCount, uc + "Workspace 2 doesn't contain 1 terminal!");

					terminal2 = workspace2.ActiveTerminal;
					Assert.IsNotNull(terminal2, uc + "Terminal 2 could not be opened!");

					VerifyFiles(uc, workspace2, false, terminal2, true, false);
				}

				// Main 1 exit.
				{
					success = (main1.Exit() == MainResult.Success);
					Assert.IsTrue(success, uc + "Main 1 could not be exited!");

					VerifyFiles(uc, workspace1, true, terminal1, true, false);
				}

				// Main 2 exit.
				{
					success = (main2.Exit() == MainResult.Success);
					Assert.IsTrue(success, uc + "Main 2 could not be exited!");

					VerifyFiles(uc, workspace2, true, terminal2, true, false);
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

				// Main 2 start.
				{
					success = (main2.Start() == MainResult.Success);
					Assert.IsTrue(success, uc + "Main 2 could not be started!");

					workspace2 = main2.Workspace;
					Assert.IsNotNull(workspace2, uc + "Workspace 2 not created!");
					Assert.AreEqual(1, workspace2.TerminalCount, uc + "Workspace 2 doesn't contain 1 terminal!");

					terminal2 = workspace2.ActiveTerminal;
					Assert.IsNotNull(terminal2, uc + "Terminal 2 could not be opened!");

					VerifyFiles(uc, workspace2, true, terminal2, true, false);
				}

				// Main 2 exit.
				{
					success = (main2.Exit() == MainResult.Success);
					Assert.IsTrue(success, uc + "Main 2 could not be exited!");

					VerifyFiles(uc, workspace2, true, terminal2, true, false);
				}
			}
			#endregion
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal could not be created!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal could not be created!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, true, terminal, true);

				Assert.AreEqual(0, ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count, step + "Recent file list is not empty!!");
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal 1 could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal 1 could not be created!");
				success = terminal.SaveAs(this.normalTerminal1FilePath);
				Assert.IsTrue(success, step + "Terminal 1 could not be saved as!");

				VerifyFiles(step, workspace, false, terminal, true, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, false, terminal, true, false);

				Assert.AreEqual(1, ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count, step + "Wrong number of recent file entries!");
			}
			#endregion

			#region Step 2
			// - Start and request recent terminal
			//   => New workspace with recent terminal
			using (Main main = new Main())
			{
				step = "Step 2: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");

				success = main.OpenRecent(1);
				Assert.IsTrue(success, step + "Recent terminal could not be opened!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Recent terminal could not be opened!");

				VerifyFiles(step, workspace, false, terminal, true, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, false, terminal, true, false);

				Assert.AreEqual(1, ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count, step + "Wrong number of recent file entries!");
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal could not be created!");
				success = terminal.SaveAs(this.normalTerminal1FilePath);
				Assert.IsTrue(success, step + "Terminal could not be saved as!");

				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, true, terminal, true, false);
			}
			#endregion

			#region Step 2
			// - Delete the terminal file
			{
				step = "Step 2: ";
				File.Delete(this.normalTerminal1FilePath);
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
				Assert.IsTrue(success, step + "Main could not be started!");
				int countAfter = this.workspace_MessageInputRequest_No_counter;
				Assert.AreNotEqual(countBefore, countAfter, "Workspace 'MessageInputRequest' was not called!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal could not be created!");
				success = terminal.SaveAs(this.normalTerminal1FilePath);
				Assert.IsTrue(success, step + "Terminal could not be saved as!");

				success = workspace.SaveAs(this.normalWorkspaceFilePath);
				Assert.IsTrue(success, step + "Workspace could not be saved as!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, true, false, terminal, true, false);
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
				int countBefore = this.main_MessageInputRequest_OK_counter;
				main.MessageInputRequest += main_MessageInputRequest_OK;
				success = (main.Start() == MainResult.ApplicationStartError);
				Assert.IsTrue(success, step + "Main could be started even though workspace file is missing!");
				int countAfter = this.main_MessageInputRequest_OK_counter;
				Assert.AreNotEqual(countBefore, countAfter, "Workspace 'MessageInputRequest' was not called!");
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

			#region Step 1
			// - Initial start
			// - Create new terminal
			// - Save terminal as
			// - Save workspace as
			using (Main main = new Main())
			{
				step = "Step 1: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal could not be created!");
				success = terminal.SaveAs(this.normalTerminal1FilePath);
				Assert.IsTrue(success, step + "Terminal could not be saved as!");

				success = workspace.SaveAs(this.normalWorkspaceFilePath);
				Assert.IsTrue(success, step + "Workspace could not be saved as!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				VerifyFiles(step, workspace, true, false, terminal, true, false);

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, true, false, terminal, true, false);
			}
			#endregion

			#region Step 2
			// - Make files write-protected
			{
				step = "Step 2: ";
				{
					string filePath = this.normalWorkspaceFilePath;
					File.SetAttributes(filePath, FileAttributes.ReadOnly);
					Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), "Workspace file is not write-protected!");
					workspaceTS = File.GetLastWriteTimeUtc(filePath);
				}
				{
					string filePath = this.normalTerminal1FilePath;
					File.SetAttributes(filePath, FileAttributes.ReadOnly);
					Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), "Terminal file is not write-protected!");
					terminalTS = File.GetLastWriteTimeUtc(filePath);
				}
			}
			#endregion

			#region Step 3
			// - Subsequent start on write-protected files
			//   => Implicit changes to terminal must not be written
			using (Main main = new Main())
			{
				step = "Step 3: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal not opened from file!");

				VerifyFiles(step, workspace, true, false, terminal, true, false);

				terminal.StopIO();
				Assert.IsFalse(terminal.IsStarted, step + "Terminal not stopped!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, true, false, terminal, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), "Terminal file is not write-protected!");
				Assert.IsTrue((terminalTS == File.GetLastWriteTimeUtc(filePath)), "Terminal file time stamp mismatches!");
			}
			#endregion

			#region Step 4
			// - Subsequent start on write-protected files
			//   => Explicit changes to workspace must not be written
			using (Main main = new Main())
			{
				step = "Step 4: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, step + "Terminal not opened from file!");

				VerifyFiles(step, workspace, true, false, terminal1, true, false);

				Assert.IsTrue(terminal1.IsStarted, step + "Terminal is no longer started!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, step + "Terminal 2 could not be created!");
				Assert.AreEqual(2, workspace.TerminalCount, step + "Workspace doesn't contain 2 terminals!");

				// Install callback handler that does not save terminal 2:
				terminal2.SaveAsFileDialogRequest += terminal2_SaveAsFileDialogRequest_No;

				// Workspace also has to be saved:
				int countBefore = this.workspace_MessageInputRequest_No_counter;
				workspace.MessageInputRequest += workspace_MessageInputRequest_No;
				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");
				int countAfter = this.workspace_MessageInputRequest_No_counter;
				Assert.AreNotEqual(countBefore, countAfter, "Workspace 'MessageInputRequest' was not called!");

				VerifyFiles(step, workspace, true, false, terminal1, true, false);

				string filePath = this.normalWorkspaceFilePath;
				Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0), "Workspace file is not write-protected!");
				Assert.IsTrue((workspaceTS == File.GetLastWriteTimeUtc(filePath)), "Workspace file time stamp mismatches!");
			}
			#endregion

			#region Step 5
			// - Make files writable again and then perform steps 3 and 4 again
			{
				step = "Step 5: ";
				{
					string filePath = this.normalWorkspaceFilePath;
					File.SetAttributes(filePath, FileAttributes.Normal);
					Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), "Workspace file is not writable!");
				}
				{
					string filePath = this.normalTerminal1FilePath;
					File.SetAttributes(filePath, FileAttributes.Normal);
					Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), "Terminal file is not writable!");
				}
			}
			#endregion

			#region Step 6
			// - Subsequent start on writable files
			//   => Implicit changes to terminal must be written again
			using (Main main = new Main())
			{
				step = "Step 6: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal not opened from file!");

				VerifyFiles(step, workspace, true, false, terminal, true, false);

				terminal.StopIO();
				Assert.IsFalse(terminal.IsStarted, step + "Terminal not stopped!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles(step, workspace, true, false, terminal, true, false);

				string filePath = this.normalTerminal1FilePath;
				Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), "Workspace file is not writable!");
				Assert.IsTrue((terminalTS != File.GetLastWriteTimeUtc(filePath)), "Terminal file time stamp still unchanged!");
			}
			#endregion

			#region Step 7
			// - Subsequent start on writable files
			//   => Explicit changes to workspace must be written again
			using (Main main = new Main())
			{
				step = "Step 7: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, step + "Terminal not opened from file!");

				VerifyFiles(step, workspace, true, false, terminal1, true, false);

				Assert.IsFalse(terminal1.IsStarted, step + "Terminal is started but should be stopped!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, step + "Terminal 2 could not be created!");
				success = terminal2.SaveAs(this.normalTerminal2FilePath);
				Assert.IsTrue(success, step + "Terminal could not be saved as!");

				Assert.AreEqual(2, workspace.TerminalCount, step + "Workspace doesn't contain 2 terminals!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");

				VerifyFiles
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
				Assert.IsTrue(((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == 0), "Workspace file is not writable!");
				Assert.IsTrue((workspaceTS != File.GetLastWriteTimeUtc(filePath)), "Workspace file time stamp still unchanged!");
			}
			#endregion

			#region Step 8
			// - Final start for verification
			using (Main main = new Main())
			{
				step = "Step 8: ";
				success = (main.Start() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(2, workspace.TerminalCount, step + "Workspace doesn't contain 2 terminals!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");
			}
			#endregion
		}

		#endregion

		#region Tests > TestFixedIndexFeatureOfWorkspace
		//------------------------------------------------------------------------------------------
		// Tests > TestFixedIndexFeatureOfWorkspace
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// This test verifies the 'DynamicIndex', 'SequentialIndex' and 'FixedIndex' feature of a YAT workspace.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		[Test]
		public virtual void TestIndicesFeatureOfWorkspace()
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(0, workspace.TerminalCount, step + "Workspace doesn't contain 0 terminals!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal 1 could not be created!");
				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, step + "Terminal 1 could not be created!");
				success = terminal.SaveAs(this.normalTerminal1FilePath);
				Assert.IsTrue(success, step + "Terminal 1 could not be saved as!");

				success = workspace.SaveAs(this.normalWorkspaceFilePath);
				Assert.IsTrue(success, step + "Workspace could not be saved as!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(1, workspace.TerminalCount, step + "Workspace doesn't contain 1 terminal!");

				Assert.AreEqual(Indices.FirstFixedIndex,   workspace.ActiveTerminalFixedIndex,      step + "Fixed index of terminal 1 isn't "   + Indices.FirstFixedIndex + "!");
				Assert.AreEqual(Indices.FirstDynamicIndex, workspace.ActiveTerminalDynamicIndex,    step + "Dynamic index of terminal 1 isn't " + Indices.FirstDynamicIndex + "!");
				Assert.AreEqual(1,                         workspace.ActiveTerminalSequentialIndex, step + "Sequential index of terminal 1 isn't 1!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal 2 could not be created!");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, step + "Terminal 2 could not be created!");
				success = terminal2.SaveAs(this.normalTerminal2FilePath);
				Assert.IsTrue(success, step + "Terminal 2 could not be saved as!");

				success = workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
				Assert.IsTrue(success, step + "Terminal 3 could not be created!");
				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal3, step + "Terminal 3 could not be created!");
				success = terminal3.SaveAs(this.normalTerminal3FilePath);
				Assert.IsTrue(success, step + "Terminal 3 could not be saved as!");

				success = workspace.Save();
				Assert.IsTrue(success, step + "Workspace could not be saved!");
				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(3, workspace.TerminalCount, step + "Workspace doesn't contain 3 terminals!");

				int first = Indices.FirstSequentialIndex;
				int last  = Indices.FirstSequentialIndex + workspace.TerminalCount - 1;
				for (int i = first; i <= last; i++)
				{
					workspace.ActivateTerminalBySequentialIndex(i);
					Assert.AreEqual(i, workspace.ActiveTerminalFixedIndex,      step + "Fixed index of terminal "      + i + " isn't " + i + "!");
					Assert.AreEqual(i, workspace.ActiveTerminalDynamicIndex,    step + "Dynamic index of terminal "    + i + " isn't " + i + "!");
					Assert.AreEqual(i, workspace.ActiveTerminalSequentialIndex, step + "Sequential index of terminal " + i + " isn't " + i + "!");
				}

				workspace.ActivateTerminalBySequentialIndex(2);
				success = workspace.CloseActiveTerminal();
				Assert.IsTrue(success, step + "Terminal 2 could not be closed!");
				Assert.AreEqual(2, workspace.TerminalCount, step + "Workspace doesn't contain 2 terminals!");

				success = workspace.Save();
				Assert.IsTrue(success, step + "Workspace could not be saved!");
				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");
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
				Assert.IsTrue(success, step + "Main could not be started!");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, step + "Workspace not created!");
				Assert.AreEqual(2, workspace.TerminalCount, step + "Workspace doesn't contain 2 terminals!");

				workspace.ActivateTerminalBySequentialIndex(1);
				Assert.AreEqual(Indices.FirstFixedIndex,   workspace.ActiveTerminalFixedIndex,      step + "Fixed index of terminal 1 isn't "   + Indices.FirstFixedIndex + "!");
				Assert.AreEqual(Indices.FirstDynamicIndex, workspace.ActiveTerminalDynamicIndex,    step + "Dynamic index of terminal 1 isn't " + Indices.FirstDynamicIndex + "!");
				Assert.AreEqual(1,                         workspace.ActiveTerminalSequentialIndex, step + "Sequential index of terminal 1 isn't 1!");

				workspace.ActivateTerminalBySequentialIndex(2);
				Assert.AreEqual(3, workspace.ActiveTerminalFixedIndex,      step + "Fixed index of terminal 3 isn't 3!");
				Assert.AreEqual(2, workspace.ActiveTerminalDynamicIndex,    step + "Dynamic index of terminal 3 isn't 2!");
				Assert.AreEqual(2, workspace.ActiveTerminalSequentialIndex, step + "Sequential index of terminal 3 isn't 2!");

				success = (main.Exit() == MainResult.Success);
				Assert.IsTrue(success, step + "Main could not be exited successfully!");
			}
			#endregion
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > Settings, Start
		//------------------------------------------------------------------------------------------
		// Private Methods > Settings, Start
		//------------------------------------------------------------------------------------------

		private static DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler()
		{
			return (new DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot>(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()));
		}

		private static void StartAndCreateDefaultTerminal(out Main main, out Workspace workspace, out Terminal terminal)
		{
			main = new Main();
			main.Start();              // Creates empty workspace
			workspace = main.Workspace;
			workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
			terminal = workspace.ActiveTerminal;
		}

		#endregion

		#region Private Methods > Verify
		//------------------------------------------------------------------------------------------
		// Private Methods > Verify
		//------------------------------------------------------------------------------------------

		private static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected)
		{
			VerifyFiles("", workspace, workspaceFileExpected, new Terminal[] { }, new bool[] { });
		}

		private static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected)
		{
			VerifyFiles(prefix, workspace, workspaceFileExpected, new Terminal[] { }, new bool[] { });
		}

		private static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected, Terminal terminal, bool terminalFileExpected)
		{
			VerifyFiles("", workspace, workspaceFileExpected, new Terminal[] { terminal }, new bool[] { terminalFileExpected });
		}

		private static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, Terminal terminal, bool terminalFileExpected)
		{
			VerifyFiles(prefix, workspace, workspaceFileExpected, new Terminal[] { terminal }, new bool[] { terminalFileExpected });
		}

		private static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected, Terminal terminal, bool terminalFileExpected, bool terminalFileAutoExpected)
		{
			VerifyFiles("", workspace, workspaceFileExpected, new Terminal[] { terminal }, new bool[] { terminalFileExpected }, new bool[] { terminalFileAutoExpected });
		}

		private static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, Terminal terminal, bool terminalFileExpected, bool terminalFileAutoExpected)
		{
			VerifyFiles(prefix, workspace, workspaceFileExpected, new Terminal[] { terminal }, new bool[] { terminalFileExpected }, new bool[] { terminalFileAutoExpected });
		}

		private static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected, bool workspaceFileAutoExpected, Terminal terminal, bool terminalFileExpected, bool terminalFileAutoExpected)
		{
			VerifyFiles("", workspace, workspaceFileExpected, workspaceFileAutoExpected, new Terminal[] { terminal }, new bool[] { terminalFileExpected }, new bool[] { terminalFileAutoExpected });
		}

		private static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, bool workspaceFileAutoExpected, Terminal terminal, bool terminalFileExpected, bool terminalFileAutoExpected)
		{
			VerifyFiles(prefix, workspace, workspaceFileExpected, workspaceFileAutoExpected, new Terminal[] { terminal }, new bool[] { terminalFileExpected }, new bool[] { terminalFileAutoExpected });
		}

		private static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected, Terminal[] terminal, bool[] terminalFileExpected)
		{
			VerifyFiles("", workspace, workspaceFileExpected, true, terminal, terminalFileExpected, new bool[] { true });
		}

		private static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, Terminal[] terminal, bool[] terminalFileExpected)
		{
			VerifyFiles(prefix, workspace, workspaceFileExpected, true, terminal, terminalFileExpected, new bool[] { true });
		}

		private static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected, Terminal[] terminal, bool[] terminalFileExpected, bool[] terminalFileAutoExpected)
		{
			VerifyFiles("", workspace, workspaceFileExpected, true, terminal, terminalFileExpected, terminalFileAutoExpected);
		}

		private static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, Terminal[] terminal, bool[] terminalFileExpected, bool[] terminalFileAutoExpected)
		{
			VerifyFiles(prefix, workspace, workspaceFileExpected, true, terminal, terminalFileExpected, terminalFileAutoExpected);
		}

		private static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected, bool workspaceFileAutoExpected, Terminal[] terminal, bool[] terminalFileExpected, bool[] terminalFileAutoExpected)
		{
			VerifyFiles("", workspace, workspaceFileExpected, workspaceFileAutoExpected, terminal, terminalFileExpected, terminalFileAutoExpected);
		}

		private static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, bool workspaceFileAutoExpected, Terminal[] terminal, bool[] terminalFileExpected, bool[] terminalFileAutoExpected)
		{
			// Verify workspace file:
			if (workspaceFileExpected)
			{
				Assert.IsNotNullOrEmpty(workspace.SettingsFilePath, prefix + "Workspace settings file path is empty!");
				Assert.IsTrue(File.Exists(workspace.SettingsFilePath), prefix + "Workspace file doesn't exist!");

				if (workspaceFileAutoExpected)
					Assert.IsTrue(workspace.SettingsRoot.AutoSaved, prefix + "Workspace file not auto saved!");
				else
					Assert.IsFalse(workspace.SettingsRoot.AutoSaved, prefix + "Workspace file must not be auto saved!");
			}
			else
			{
				Assert.IsFalse(File.Exists(workspace.SettingsFilePath), prefix + "Workspace file exists unexpectantly!");
			}

			// Verify terminal file(s):
			for (int i = 0; i < terminal.Length; i++)
			{
				if (terminalFileExpected[i])
				{
					Assert.IsNotNullOrEmpty(terminal[i].SettingsFilePath, prefix + "Terminal settings file path is empty!");
					Assert.IsTrue(File.Exists(terminal[i].SettingsFilePath), prefix + "Terminal file doesn't exist!");

					if (terminalFileAutoExpected[i])
						Assert.IsTrue(terminal[i].SettingsRoot.AutoSaved, prefix + "Terminal file not auto saved!");
					else
						Assert.IsFalse(terminal[i].SettingsRoot.AutoSaved, prefix + "Terminal file must not be auto saved!");
				}
				else
				{
					Assert.IsFalse(File.Exists(terminal[i].SettingsFilePath), prefix + "Terminal file exists unexpectantly!");
				}
			}

			// Verify application settings:
			if (workspaceFileExpected)
				StringAssert.AreEqualIgnoringCase(workspace.SettingsFilePath, ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath, prefix + "Workspace file path not set!");
			else
				//// Note that the application settings may still contain a former workspace file path.
				//// This is required to test certain use cases with normal and command line execution.
				//// Therefore, do not check the local user settings, instead, check that the workspace file path is reset.
				Assert.IsNullOrEmpty(workspace.SettingsFilePath, prefix + "Workspace file path not reset!");

			// Verify recent settings:
			if (workspaceFileExpected && (!workspaceFileAutoExpected))
				Assert.IsTrue(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(workspace.SettingsFilePath), prefix + "Workspace file path doesn't exist in recent files!");
			else
				Assert.IsFalse(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(workspace.SettingsFilePath), prefix + "Workspace file path must not be in recent files!");

			for (int i = 0; i < terminal.Length; i++)
			{
				if (terminalFileExpected[i] && (!terminalFileAutoExpected[i]))
					Assert.IsTrue(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(terminal[i].SettingsFilePath), prefix + "Terminal file path doesn't exist in recent files!");
				else
					Assert.IsFalse(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(terminal[i].SettingsFilePath), prefix + "Terminal file path must not be in recent files!");
			}
		}

		#endregion

		#region Private Methods > Event Handlers
		//------------------------------------------------------------------------------------------
		// Private Methods > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <remarks>Counter can be used to assert that handler indeed was called.</remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int main_MessageInputRequest_OK_counter; // = 0;

		private void main_MessageInputRequest_OK(object sender, MessageInputEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.OK;
			this.main_MessageInputRequest_OK_counter++;
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

		private void terminal2_SaveAsFileDialogRequest_SaveAs(object sender, DialogEventArgs e)
		{
			Terminal terminal = (sender as Terminal);
			Assert.IsTrue(terminal.SaveAs(this.normalTerminal2FilePath), "Terminal 2 could not be saved as!");
			e.Result = System.Windows.Forms.DialogResult.OK;
		}

		private void terminal2_SaveAsFileDialogRequest_No(object sender, DialogEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.No;
		}

		private void terminal3_SaveAsFileDialogRequest_SaveAsOK(object sender, DialogEventArgs e)
		{
			Terminal terminal = (sender as Terminal);
			Assert.IsTrue(terminal.SaveAs(this.normalTerminal3FilePath), "Terminal 3 could not be saved as!");
			e.Result = System.Windows.Forms.DialogResult.OK;
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
