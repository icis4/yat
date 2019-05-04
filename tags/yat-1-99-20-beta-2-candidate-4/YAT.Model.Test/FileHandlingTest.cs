using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using NUnit.Framework;

using YAT.Settings.Application;

namespace YAT.Model.Test
{
	[TestFixture]
	public class FileHandlingTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string _TempPath = @"c:\";
		private const string _TempPrefix = "YAT-Test-";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _autoOpenWorkspaceToRestore;
		private bool _autoSaveWorkspaceToRestore;
		private string _workspaceFilePathToRestore;

		string _normalWorkspaceFilePath = MakeTempFilePath("NormalWorkspace", YAT.Settings.ExtensionSettings.WorkspaceFiles);
		string _normalTerminal1FilePath = MakeTempFilePath("NormalTerminal1", YAT.Settings.ExtensionSettings.TerminalFiles);
		string _normalTerminal2FilePath = MakeTempFilePath("NormalTerminal2", YAT.Settings.ExtensionSettings.TerminalFiles);
		string _normalTerminal3FilePath = MakeTempFilePath("NormalTerminal3", YAT.Settings.ExtensionSettings.TerminalFiles);

		#endregion

		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// allow modification of auto save setting
			_autoOpenWorkspaceToRestore = ApplicationSettings.LocalUser.General.AutoOpenWorkspace;
			_autoSaveWorkspaceToRestore = ApplicationSettings.LocalUser.General.AutoSaveWorkspace;
			_workspaceFilePathToRestore = ApplicationSettings.LocalUser.General.WorkspaceFilePath;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			ApplicationSettings.LocalUser.General.AutoOpenWorkspace = _autoOpenWorkspaceToRestore;
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = _autoSaveWorkspaceToRestore;
			ApplicationSettings.LocalUser.General.WorkspaceFilePath = _workspaceFilePathToRestore;
		}

		#endregion

		#region Tear Down
		//==========================================================================================
		// Tear Down
		//==========================================================================================

		[TearDown]
		public void TearDown()
		{
			foreach (string filePath in Directory.GetFiles(MakeTempPath(), MakeTempFileName("*", ".*")))
				File.Delete(filePath);
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > TestInitialAutoSaveOnClose
		//------------------------------------------------------------------------------------------
		// Tests > TestInitialAutoSaveOnClose
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  Auto save of terminal and workspace.
		/// </summary>
		[Test]
		public void TestInitialAutoSaveOnMainClose()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			PrepareInitialAutoSave(out main, out workspace, out terminal);
			main.Exit();
			VerifyFiles(workspace, true, terminal, true);
		}

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  No auto save.
		/// </summary>
		[Test]
		public void TestInitialAutoSaveOnWorkspaceAndMainClose()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			PrepareInitialAutoSave(out main, out workspace, out terminal);
			workspace.Close();
			VerifyFiles(workspace, false, terminal, false);
			main.Exit();
			VerifyFiles(workspace, false, terminal, false);
		}

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  Auto save of workspace.
		/// </summary>
		[Test]
		public void TestInitialAutoSaveOnTerminalAndMainClose()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			PrepareInitialAutoSave(out main, out workspace, out terminal);
			terminal.Close();
			VerifyFiles(workspace, false, terminal, false);
			main.Exit();
			VerifyFiles(workspace, true, terminal, false);
		}

		/// <summary>
		/// Condition: No files existing.
		/// Expected:  No auto save.
		/// </summary>
		[Test]
		public void TestInitialAutoSaveOnTerminalAndWorkspaceAndMainClose()
		{
			Main main;
			Workspace workspace;
			Terminal terminal;

			PrepareInitialAutoSave(out main, out workspace, out terminal);
			terminal.Close();
			VerifyFiles(workspace, false, terminal, false);
			workspace.Close();
			VerifyFiles(workspace, false, terminal, false);
			main.Exit();
			VerifyFiles(workspace, false, terminal, false);
		}

		#endregion

		#region Tests > TestSequenceOfUseCases_1_through_5a_
		//------------------------------------------------------------------------------------------
		// Tests > TestSequenceOfUseCases_1_through_5a_
		//------------------------------------------------------------------------------------------
		/// <summary>
		/// Use cases according to ufi.
		/// </summary>
		[Test]
		public void TestSequenceOfUseCases_1_through_5a_()
		{
			bool success = false;

			ApplicationSettings.LocalUser.General.AutoOpenWorkspace = true;
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = true;
			ApplicationSettings.LocalUser.General.WorkspaceFilePath = "";

			#region Use case 1
			// - Initial start
			// - Create new terminal
			//   => Auto workspace with 1 auto terminal
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not created");
				Assert.AreEqual(0, workspace.TerminalCount, "Workspace doesn't contain 0 terminals");

				success = workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler());
				Assert.IsTrue(success, "Terminal could not be created");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, "Terminal could not be created");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles(workspace, true, terminal, true);
			}
			#endregion

			#region Use case 2
			// - Start with auto workspace that contains 1 auto terminal
			// - Save terminal
			//   => Auto terminal stays auto
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, "Terminal not opened from file");

				VerifyFiles(workspace, true, terminal, true);

				success = terminal.Save();
				Assert.IsTrue(success, "Terminal could not be saved");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");

				main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles(workspace, true, terminal, true);
			}
			#endregion

			#region Use case 2a
			// - Start with auto workspace that contains 1 auto terminal
			// - Save terminal as
			//   => Auto terminal becomes normal terminal
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, "Terminal not opened from file");

				VerifyFiles(workspace, true, terminal, true);

				string defaultTerminal1FilePath = terminal.SettingsFilePath;
				success = terminal.SaveAs(_normalTerminal1FilePath);
				Assert.IsTrue(success, "Terminal could not be saved as");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");
				Assert.IsFalse(File.Exists(defaultTerminal1FilePath), "Auto terminal file not deleted");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles(workspace, true, terminal, true, false);
			}
			#endregion

			#region Use case 3
			// - Start with auto workspace that contains 1 normal terminal
			// - Create 2 more terminals
			//   => Auto workspace with 1 normal and 2 auto terminals
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, "Terminal 1 not opened from file");

				VerifyFiles(workspace, true, terminal1, true, false);
				StringAssert.AreEqualIgnoringCase(_normalTerminal1FilePath, terminal1.SettingsFilePath, "Terminal 1 is not stored at user terminal 1 location");

				success = workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler());
				Assert.IsTrue(success, "Terminal 2 could not be created");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");

				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, "Terminal 2 could not be created");

				Assert.IsTrue(workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler()), "Terminal 3 could not be created");
				Assert.AreEqual(3, workspace.TerminalCount, "Workspace doesn't contain 3 terminals");

				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal3, "Terminal 3 could not be created");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // exists
					new bool[]     { false,     true,      true      }  // auto
					);
			}
			#endregion

			#region Use case 3a
			// - Start with auto workspace that contains 1 normal and 2 auto terminals
			// - Close terminal 3
			//   => Auto workspace with 1 normal and 1 auto terminal
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(3, workspace.TerminalCount, "Workspace doesn't contain 3 terminals");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, "Terminal 1 not opened from file");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, "Terminal 2 not opened from file");
				Terminal terminal3 = workspace.Terminals[2];
				Assert.IsNotNull(terminal3, "Terminal 3 not opened from file");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // exists
					new bool[]     { false,     true,      true      }  // auto
					);

				string autoTerminal3FilePath = terminal3.SettingsFilePath;
				success = terminal3.Close();
				Assert.IsTrue(success, "Terminal 3 could not be closed");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");
				Assert.IsFalse(File.Exists(autoTerminal3FilePath), "Auto terminal 3 file not deleted");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     true      }  // auto
					);
			}
			#endregion

			#region Use case 4
			// - Start with auto workspace that contains 1 normal and 1 auto terminal
			// - Save workspace
			//   => Auto workspace stays auto workspace
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, "Terminal 1 not opened from file");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, "Terminal 2 not opened from file");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     true      }  // auto
					);

				success = workspace.Save();
				Assert.IsTrue(success, "Workspace could not be saved");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     true      }  // auto
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
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, "Terminal 1 not opened from file");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, "Terminal 2 not opened from file");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     true      }  // auto
					);

				// install callback handler that sets the normal file path for terminal 2
				terminal2.SaveAsFileDialogRequest += new EventHandler<DialogEventArgs>(terminal2_SaveAsFileDialogRequest);

				string autoWorkspaceFilePath = workspace.SettingsFilePath;
				string autoTerminal2FilePath = terminal2.SettingsFilePath;
				success = workspace.SaveAs(_normalWorkspaceFilePath);
				Assert.IsTrue(success, "Workspace could not be saved as");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");
				Assert.IsFalse(File.Exists(autoWorkspaceFilePath), "Auto workspace file not deleted");
				Assert.IsFalse(File.Exists(autoTerminal2FilePath), "Auto terminal 2 file not deleted");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     false     }  // auto
					);
			}
			#endregion

			#region Use case 5
			// - Start with normal workspace that contains 2 normal terminals
			// - Create new terminal
			//   => Terminal becomes normal terminal
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, "Terminal 1 not opened from file");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, "Terminal 2 not opened from file");

				VerifyFiles
					(
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     false     }  // auto
					);

				success = workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler());
				Assert.IsTrue(success, "Terminal 3 could not be created");
				Assert.AreEqual(3, workspace.TerminalCount, "Workspace doesn't contain 3 terminals");

				Terminal terminal3 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal3, "Terminal 3 could not be created");

				// install callback handler that sets the normal file path for terminal 3
				terminal3.SaveAsFileDialogRequest += new EventHandler<DialogEventArgs>(terminal3_SaveAsFileDialogRequest);

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // exists
					new bool[]     { false,     false,     false     }  // auto
					);
			}
			#endregion

			#region Use case 5a
			// - Start with normal workspace that contains 3 normal terminals
			// - Close terminal 3
			//   => Normal workspace contains 2 normal terminals but terminal 3 file stays
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not opened from file");
				Assert.AreEqual(3, workspace.TerminalCount, "Workspace doesn't contain 3 terminals");

				Terminal terminal1 = workspace.Terminals[0];
				Assert.IsNotNull(terminal1, "Terminal 1 not opened from file");
				Terminal terminal2 = workspace.Terminals[1];
				Assert.IsNotNull(terminal2, "Terminal 2 not opened from file");
				Terminal terminal3 = workspace.Terminals[2];
				Assert.IsNotNull(terminal3, "Terminal 3 not opened from file");

				VerifyFiles
					(
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // exists
					new bool[]     { false,     false,     false     }  // auto
					);

				success = terminal3.Close();
				Assert.IsTrue(success, "Terminal 3 could not be closed");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					false,
					new Terminal[] { terminal1, terminal2, terminal3 },
					new bool[]     { true,      true,      true      }, // exists
					new bool[]     { false,     false,     false     }  // auto
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
		[Test]
		public void TestSequenceOfUseCases_6_through_9_()
		{
			bool success = false;

			ApplicationSettings.LocalUser.General.AutoOpenWorkspace = true;
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = true;
			ApplicationSettings.LocalUser.General.WorkspaceFilePath = "";

			#region Preparation
			// - Initial start
			// - Create two new terminals
			// - Save terminals as
			//   => Auto workspace with 2 normal terminals
			using (Main main = new Main())
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not created");
				Assert.AreEqual(0, workspace.TerminalCount, "Workspace doesn't contain 0 terminals");

				success = workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler());
				Assert.IsTrue(success, "Terminal 1 could not be created");
				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, "Terminal 1 could not be created");
				success = terminal1.SaveAs(_normalTerminal1FilePath);
				Assert.IsTrue(success, "Terminal 1 could not be saved as");

				success = workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler());
				Assert.IsTrue(success, "Terminal 2 could not be created");
				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, "Terminal 2 could not be created");
				success = terminal2.SaveAs(_normalTerminal2FilePath);
				Assert.IsTrue(success, "Terminal 2 could not be saved as");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     false     }  // auto
					);
			}
			#endregion

			#region Use case 6
			// - Start and request normal terminal
			//   => Auto workspace with 1 normal terminal
			using (Main main = new Main(_normalTerminal1FilePath))
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not created");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");

				Terminal terminal = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal, "Terminal could not be opened");

				VerifyFiles(workspace, false, terminal, true, false);

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles(workspace, true, terminal, true, false);
			}
			#endregion

			#region Use case 7
			// - Start and request normal terminal
			// - Create another terminal
			//   => Auto workspace with 1 normal and 1 auto terminal
			using (Main main = new Main(_normalTerminal1FilePath))
			{
				success = main.Start();
				Assert.IsTrue(success, "Main could not be started");

				Workspace workspace = main.Workspace;
				Assert.IsNotNull(workspace, "Workspace not created");
				Assert.AreEqual(1, workspace.TerminalCount, "Workspace doesn't contain 1 terminal");

				Terminal terminal1 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal1, "Terminal 1 could not be opened");

				VerifyFiles(workspace, false, terminal1, true, false);

				success = workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler());
				Assert.IsTrue(success, "Terminal 2 could not be created");
				Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");

				Terminal terminal2 = workspace.ActiveTerminal;
				Assert.IsNotNull(terminal2, "Terminal 2 could not be created");

				success = main.Exit();
				Assert.IsTrue(success, "Main could not be exited");

				VerifyFiles
					(
					workspace,
					true,
					new Terminal[] { terminal1, terminal2 },
					new bool[]     { true,      true      }, // exists
					new bool[]     { false,     true      }  // auto
					);
			}
			#endregion

			#region Use case 8
			// - Start and request normal terminal
			// - Start and request normal terminal in another main
			// - Close first main, then second
			//   => 2 auto workspaces with 1 normal terminal each
			using (Main main1 = new Main(_normalTerminal1FilePath),
				        main2 = new Main(_normalTerminal2FilePath))
			{
				Workspace workspace1;
				Workspace workspace2;

				Terminal terminal1;
				Terminal terminal2;

				// main 1 start
				{
					success = main1.Start();
					Assert.IsTrue(success, "Main 1 could not be started");

					workspace1 = main1.Workspace;
					Assert.IsNotNull(workspace1, "Workspace 1 not created");
					Assert.AreEqual(1, workspace1.TerminalCount, "Workspace 1 doesn't contain 1 terminal");

					terminal1 = workspace1.ActiveTerminal;
					Assert.IsNotNull(terminal1, "Terminal 1 could not be opened");

					VerifyFiles(workspace1, false, terminal1, true, false);
				}

				// main 2 start
				{
					success = main2.Start();
					Assert.IsTrue(success, "Main 2 could not be started");

					workspace2 = main2.Workspace;
					Assert.IsNotNull(workspace2, "Workspace 2 not created");
					Assert.AreEqual(1, workspace2.TerminalCount, "Workspace 2 doesn't contain 1 terminal");

					terminal2 = workspace2.ActiveTerminal;
					Assert.IsNotNull(terminal2, "Terminal 2 could not be opened");

					VerifyFiles(workspace2, false, terminal2, true, false);
				}

				// main 1 exit
				{
					success = main1.Exit();
					Assert.IsTrue(success, "Main 1 could not be exited");

					VerifyFiles(workspace1, true, terminal1, true, false);
				}

				// main 2 exit
				{
					success = main2.Exit();
					Assert.IsTrue(success, "Main 2 could not be exited");

					VerifyFiles(workspace2, true, terminal2, true, false);
				}
			}
			#endregion

			#region Use case 9
			// - Start with auto workspace that contains 1 normal terminal
			// - Start another main
			//   => 2 auto workspaces, first with 1 normal terminal
			using (Main main2 = new Main(),
						main3 = new Main())
			{
				Workspace workspace2;
				Workspace workspace3;

				Terminal terminal2;

				// main 2 start
				{
					success = main2.Start();
					Assert.IsTrue(success, "Main 2 could not be started");

					workspace2 = main2.Workspace;
					Assert.IsNotNull(workspace2, "Workspace 2 not created");
					Assert.AreEqual(1, workspace2.TerminalCount, "Workspace 2 doesn't contain 1 terminal");

					terminal2 = workspace2.ActiveTerminal;
					Assert.IsNotNull(terminal2, "Terminal 2 could not be opened");

					VerifyFiles(workspace2, true, terminal2, true, false);
				}

				// main 3 start
				{
					success = main3.Start();
					Assert.IsTrue(success, "Main 3 could not be started");

					workspace3 = main3.Workspace;
					Assert.IsNotNull(workspace3, "Workspace 3 not created");
					Assert.AreEqual(0, workspace3.TerminalCount, "Workspace 3 doesn't contain 0 terminals");

					VerifyFiles(workspace3, false);
				}

				// main 3 exit
				{
					success = main3.Exit();
					Assert.IsTrue(success, "Main 3 could not be exited");

					VerifyFiles(workspace3, true);
				}

				// main 2 exit
				{
					success = main2.Exit();
					Assert.IsTrue(success, "Main 2 could not be exited");

					VerifyFiles(workspace2, true, terminal2, true, false);
				}
			}
			#endregion
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > Temp Path
		//------------------------------------------------------------------------------------------
		// Private Methods > Temp Path
		//------------------------------------------------------------------------------------------

		private static string MakeTempPath()
		{
			return (_TempPath);
		}

		private static string MakeTempFileName(string name, string extension)
		{
			return (_TempPrefix + name + extension);
		}

		private static string MakeTempFilePath(string name, string extension)
		{
			return (MakeTempPath() + MakeTempFileName(name, extension));
		}

		#endregion

		#region Private Methods > Prepare
		//------------------------------------------------------------------------------------------
		// Private Methods > Prepare
		//------------------------------------------------------------------------------------------

		private void PrepareInitialAutoSave(out Main main, out Workspace workspace, out Terminal terminal)
		{
			ApplicationSettings.LocalUser.General.AutoOpenWorkspace = false;
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = true;
			ApplicationSettings.LocalUser.General.WorkspaceFilePath = "";

			main = new Main();
			main.Start();              // creates empty workspace
			workspace = main.Workspace;
			workspace.CreateNewTerminal(Utilities.GetTextTCPSettingsHandler());
			terminal = workspace.ActiveTerminal;
		}

		#endregion

		#region Private Methods > Verify
		//------------------------------------------------------------------------------------------
		// Private Methods > Verify
		//------------------------------------------------------------------------------------------

		private void VerifyFiles(Workspace workspace, bool workspaceFileExpected)
		{
			VerifyFiles(workspace, workspaceFileExpected,
						new Terminal[] { }, new bool[] { });
		}

		private void VerifyFiles(Workspace workspace, bool workspaceFileExpected,
								 Terminal terminal, bool terminalFileExpected)
		{
			VerifyFiles(workspace, workspaceFileExpected,
						new Terminal[] { terminal }, new bool[] { terminalFileExpected });
		}

		private void VerifyFiles(Workspace workspace, bool workspaceFileExpected,
								 Terminal terminal, bool terminalFileExpected, bool terminalFileAutoExpected)
		{
			VerifyFiles(workspace, workspaceFileExpected,
						new Terminal[] { terminal }, new bool[] { terminalFileExpected }, new bool[] { terminalFileAutoExpected });
		}

		private void VerifyFiles(Workspace workspace, bool workspaceFileExpected,
								 Terminal[] terminal, bool[] terminalFileExpected)
		{
			VerifyFiles(workspace, workspaceFileExpected, workspaceFileExpected,
				        terminal, terminalFileExpected, terminalFileExpected);
		}

		private void VerifyFiles(Workspace workspace, bool workspaceFileExpected,
								 Terminal[] terminal, bool[] terminalFileExpected, bool[] terminalFileAutoExpected)
		{
			VerifyFiles(workspace, workspaceFileExpected, workspaceFileExpected,
				        terminal, terminalFileExpected, terminalFileAutoExpected);
		}

		private void VerifyFiles(Workspace workspace, bool workspaceFileExpected, bool workspaceFileAutoExpected,
			                     Terminal[] terminal, bool[] terminalFileExpected, bool[] terminalFileAutoExpected)
		{
			// verify workspace file(s)
			if (workspaceFileExpected)
			{
				Assert.IsTrue(File.Exists(workspace.SettingsFilePath), "Workspace file doesn't exist");

				if (workspaceFileAutoExpected)
					Assert.IsTrue(workspace.SettingsRoot.AutoSaved, "Workspace file not auto saved");
				else
					Assert.IsFalse(workspace.SettingsRoot.AutoSaved, "Workspace file must not be auto saved");
			}
			else
			{
				Assert.IsFalse(File.Exists(workspace.SettingsFilePath), "Workspace file exists unexpectantly");
			}

			// verify terminal file
			for (int i = 0; i < terminal.Length; i++)
			{
				if (terminalFileExpected[i])
				{
					Assert.IsTrue(File.Exists(terminal[i].SettingsFilePath), "Terminal file doesn't exist");

					if (terminalFileAutoExpected[i])
						Assert.IsTrue(terminal[i].SettingsRoot.AutoSaved, "Terminal file not auto saved");
					else
						Assert.IsFalse(terminal[i].SettingsRoot.AutoSaved, "Terminal file must not be auto saved");
				}
				else
				{
					Assert.IsFalse(File.Exists(terminal[i].SettingsFilePath), "Terminal file exists unexpectantly");
				}
			}

			// verify application settings
			if (workspaceFileExpected)
				StringAssert.AreEqualIgnoringCase(workspace.SettingsFilePath, ApplicationSettings.LocalUser.General.WorkspaceFilePath, "Workspace file path not set");
			else
				StringAssert.AreEqualIgnoringCase("", ApplicationSettings.LocalUser.General.WorkspaceFilePath, "Workspace file path not reset");

			// verify recent settings
			if (workspaceFileExpected && (!workspaceFileAutoExpected))
				Assert.IsTrue(ApplicationSettings.LocalUser.RecentFiles.FilePaths.Contains(workspace.SettingsFilePath), "Workspace file path doesn't exist in recents");
			else
				Assert.IsFalse(ApplicationSettings.LocalUser.RecentFiles.FilePaths.Contains(workspace.SettingsFilePath), "Workspace file path must not be in recents");

			for (int i = 0; i < terminal.Length; i++)
			{
				if (terminalFileExpected[i] && (!terminalFileAutoExpected[i]))
					Assert.IsTrue(ApplicationSettings.LocalUser.RecentFiles.FilePaths.Contains(terminal[i].SettingsFilePath), "Terminal file path doesn't exist in recents");
				else
					Assert.IsFalse(ApplicationSettings.LocalUser.RecentFiles.FilePaths.Contains(terminal[i].SettingsFilePath), "Terminal file path must not be in recents");
			}
		}

		#endregion

		#endregion

		#region Event Handlers
		//==========================================================================================
		// Event Handlers
		//==========================================================================================

		private void terminal2_SaveAsFileDialogRequest(object sender, DialogEventArgs e)
		{
			Terminal terminal = sender as Terminal;
			Assert.IsTrue(terminal.SaveAs(_normalTerminal2FilePath), "Terminal 2 could not be saved as");
			e.Result = System.Windows.Forms.DialogResult.OK;
		}

		private void terminal3_SaveAsFileDialogRequest(object sender, DialogEventArgs e)
		{
			Terminal terminal = sender as Terminal;
			Assert.IsTrue(terminal.SaveAs(_normalTerminal3FilePath), "Terminal 3 could not be saved as");
			e.Result = System.Windows.Forms.DialogResult.OK;
		}

		#endregion
	}
}