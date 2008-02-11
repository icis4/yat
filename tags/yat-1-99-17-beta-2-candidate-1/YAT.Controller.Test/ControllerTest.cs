using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

namespace YAT.Controller.Test
{
	[TestFixture]
	public class ControllerTest
	{
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
			string[] args =
				{
				};

			Controller.Main main = new Main(args);

			Assert.IsFalse(main.CommandLineError);
			Assert.IsFalse(main.CommandLineHelpIsRequested);
		}

		#endregion

		#region Tests > TerminalCommandLineArg
		//------------------------------------------------------------------------------------------
		// Tests > TerminalCommandLineArg
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestTerminalCommandLineArg()
		{
			string[] args =
				{
					Settings.Test.SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[Settings.Test.TerminalSettingsTestCases.T_03_COM1_Closed_Predefined],
				};

			Controller.Main main = new Main(args);

			Assert.IsFalse(main.CommandLineError);
			Assert.IsFalse(main.CommandLineHelpIsRequested);
			StringAssert.AreEqualIgnoringCase(args[0], main.RequestedFilePath, "Invalid requested terminal settings file path");
		}

		#endregion

		#region Tests > WorkspaceCommandLineArg
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArg
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestWorkspaceCommandLineArg()
		{
			string[] args =
				{
					Settings.Test.SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[Settings.Test.WorkspaceSettingsTestCases.W_04_Matthias],
				};

			Controller.Main main = new Main(args);

			Assert.IsFalse(main.CommandLineError);
			Assert.IsFalse(main.CommandLineHelpIsRequested);
			StringAssert.AreEqualIgnoringCase(args[0], main.RequestedFilePath, "Invalid requested workspace settings file path");
		}

		#endregion

		#endregion
	}
}
