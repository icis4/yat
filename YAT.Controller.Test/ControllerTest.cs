//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using YAT.Settings.Application;

namespace YAT.Controller.Test
{
	[TestFixture]
	public class ControllerTest
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _autoSaveWorkspaceToRestore;

		#endregion

		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// prevent auto-save of workspace settings
			_autoSaveWorkspaceToRestore = ApplicationSettings.LocalUser.General.AutoSaveWorkspace;
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = false;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = _autoSaveWorkspaceToRestore;
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

		[Test]
		public void TestEmptyCommandLine()
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

		[Test]
		public void TestTerminalCommandLineArg()
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

		[Test]
		public void TestWorkspaceCommandLineArg()
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

		[Test]
		public void TestEmptyCommandLineRun()
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

		[Test]
		public void TestTerminalCommandLineArgRun()
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

		[Test]
		public void TestWorkspaceCommandLineArgRun()
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

		[Test]
		[Category("Interactive")]
		public void TestEmptyCommandLineRunInteractive()
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

		[Test]
		[Category("Interactive")]
		public void TestTerminalCommandLineArgRunInteractive()
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

		[Test]
		[Category("Interactive")]
		public void TestWorkspaceCommandLineArgRunInteractive()
		{
			using (Controller.Main main = new Main(WorkspaceArgs))
			{
				RunAndVerifyApplication(main);
			}
		}

		#endregion

		#endregion

		#region Private Properties
		//==========================================================================================
		// Private Properties
		//==========================================================================================

		private string[] EmptyArgs
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

		private string[] TerminalArgs
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

		private string[] WorkspaceArgs
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

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void RunAndVerifyApplication(Controller.Main main)
		{
			RunAndVerifyApplication(main, MainResult.OK);
		}

		private void RunAndVerifyApplication(Controller.Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run();
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		private void RunAndVerifyApplicationWithoutView(Controller.Main main)
		{
			RunAndVerifyApplicationWithoutView(main, MainResult.OK);
		}

		private void RunAndVerifyApplicationWithoutView(Controller.Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run(false);
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
