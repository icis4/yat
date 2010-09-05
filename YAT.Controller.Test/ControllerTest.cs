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

using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using YAT.Settings.Application;

namespace YAT.Controller.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ControllerTest
	{
		#region Private Static Properties
		//==========================================================================================
		// Private Static Properties
		//==========================================================================================

		private static string[] EmptyArgs
		{
			get
			{
				return
				(
					new string[]
					{
					}
				);
			}
		}

		private static string[] TerminalArgs
		{
			get
			{
				return
				(
					new string[]
					{
						Settings.Test.SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[Settings.Test.TerminalSettingsTestCases.T_03_COM1_Closed_Predefined],
					}
				);
			}
		}

		private static string[] WorkspaceArgs
		{
			get
			{
				return
				(
					new string[]
					{
						Settings.Test.SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[Settings.Test.WorkspaceSettingsTestCases.W_04_Matthias],
					}
				);
			}
		}

		#endregion

		#region Private Static Methods
		//==========================================================================================
		// Private Static Methods
		//==========================================================================================

		private static void RunAndVerifyApplication(Controller.Main main)
		{
			RunAndVerifyApplication(main, MainResult.Success);
		}

		private static void RunAndVerifyApplication(Controller.Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run();
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		private static void RunAndVerifyApplicationWithoutView(Controller.Main main)
		{
			RunAndVerifyApplicationWithoutView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithoutView(Controller.Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run(false);
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool autoSaveWorkspaceToRestore;

		#endregion

		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Prevent auto-save of workspace settings.
			this.autoSaveWorkspaceToRestore = ApplicationSettings.LocalUser.General.AutoSaveWorkspace;
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = false;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = this.autoSaveWorkspaceToRestore;
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > EmptyCommandLine
		//------------------------------------------------------------------------------------------
		// Tests > EmptyCommandLine
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyCommandLine()
		{
			using (Controller.Main main = new Main(EmptyArgs))
			{
				Assert.IsFalse(main.CommandLineError);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
			}
		}

		#endregion

		#region Tests > TerminalCommandLineArg
		//------------------------------------------------------------------------------------------
		// Tests > TerminalCommandLineArg
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTerminalCommandLineArg()
		{
			using (Controller.Main main = new Main(TerminalArgs))
			{
				Assert.IsFalse(main.CommandLineError);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
				StringAssert.AreEqualIgnoringCase(TerminalArgs[0], main.RequestedFilePath, "Invalid requested terminal settings file path");
			}
		}

		#endregion

		#region Tests > WorkspaceCommandLineArg
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArg
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestWorkspaceCommandLineArg()
		{
			using (Controller.Main main = new Main(WorkspaceArgs))
			{
				Assert.IsFalse(main.CommandLineError);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
				StringAssert.AreEqualIgnoringCase(WorkspaceArgs[0], main.RequestedFilePath, "Invalid requested workspace settings file path");
			}
		}

		#endregion

		#region Tests > EmptyCommandLineRun
		//------------------------------------------------------------------------------------------
		// Tests > EmptyCommandLineRun
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyCommandLineRun()
		{
			using (Controller.Main main = new Main(EmptyArgs))
			{
				RunAndVerifyApplicationWithoutView(main);
			}
		}

		#endregion

		#region Tests > TerminalCommandLineArgRun
		//------------------------------------------------------------------------------------------
		// Tests > TerminalCommandLineArgRun
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTerminalCommandLineArgRun()
		{
			using (Controller.Main main = new Main(TerminalArgs))
			{
				RunAndVerifyApplicationWithoutView(main);
			}
		}

		#endregion

		#region Tests > WorkspaceCommandLineArgRun
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArgRun
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestWorkspaceCommandLineArgRun()
		{
			using (Controller.Main main = new Main(WorkspaceArgs))
			{
				RunAndVerifyApplicationWithoutView(main);
			}
		}

		#endregion

		#region Tests > EmptyCommandLineRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > EmptyCommandLineRunInteractive
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[Category("Interactive")]
		public virtual void TestEmptyCommandLineRunInteractive()
		{
			using (Controller.Main main = new Main(EmptyArgs))
			{
				RunAndVerifyApplication(main);
			}
		}

		#endregion

		#region Tests > TerminalCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > TerminalCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[Category("Interactive")]
		public virtual void TestTerminalCommandLineArgRunInteractive()
		{
			using (Controller.Main main = new Main(TerminalArgs))
			{
				RunAndVerifyApplication(main);
			}
		}

		#endregion

		#region Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		[Category("Interactive")]
		public virtual void TestWorkspaceCommandLineArgRunInteractive()
		{
			using (Controller.Main main = new Main(WorkspaceArgs))
			{
				RunAndVerifyApplication(main);
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
