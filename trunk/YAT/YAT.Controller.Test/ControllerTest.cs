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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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
using System.Windows.Forms;

using MKY.IO;
using MKY.Settings;
using MKY.Windows.Forms;

using NUnit;
using NUnit.Framework;

using YAT.Settings.Application;

#endregion

namespace YAT.Controller.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ControllerTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] EmptyArgs = new string[] { };

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Orthogonality with underlying test case.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string TerminalFilePath_TestCase03 = Settings.Test.SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[Settings.Test.TerminalSettingsTestCase.T_03_COM1_Closed_Predefined];

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Orthogonality with underlying test case.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string WorkspaceFilePath_TestCase04 = Settings.Test.SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[Settings.Test.WorkspaceSettingsTestCase.W_04_Matthias];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] SerialPortArgs = new string[] { "--TerminalType=Binary", "--SerialPort=5", "--DataBits=7", "--Parity=E", "--FlowControl=Software" };

		#endregion

		#region Fields
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		string tempPath;

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
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;

			tempPath = Temp.MakeTempPath(GetType());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();

			Temp.CleanTempPath(GetType());
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
			using (var m = new Main(EmptyArgs))
			{
				PrepareMainAndVerifyResult(m, MainResult.Success);

				Assert.That(m.CommandLineIsValid,         Is.True);
				Assert.That(m.CommandLineHelpIsRequested, Is.False);
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
			var terminalFilePathForTest = CloneForTest(TerminalFilePath_TestCase03, "03 - *.*");
			using (var m = new Main(new string[]{ terminalFilePathForTest }))
			{
				PrepareMainAndVerifyResult(m, MainResult.Success);

				Assert.That(m.CommandLineIsValid,         Is.True);
				Assert.That(m.CommandLineHelpIsRequested, Is.False);
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
			var workspaceFilePathForTest = CloneForTest(WorkspaceFilePath_TestCase04, "04 - *.*");
			using (var m = new Main(new string[]{ workspaceFilePathForTest }))
			{
				PrepareMainAndVerifyResult(m, MainResult.Success);

				Assert.That(m.CommandLineIsValid,         Is.True);
				Assert.That(m.CommandLineHelpIsRequested, Is.False);
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
			using (var m = new Main(EmptyArgs))
			{
				RunAndVerifyApplicationWithoutView(m);
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
			var terminalFilePathForTest = CloneForTest(TerminalFilePath_TestCase03, "03 - *.*");
			using (var m = new Main(new string[]{ terminalFilePathForTest }))
			{
				RunAndVerifyApplicationWithoutView(m);
			}
		}

		#endregion

		#region Tests > WorkspaceCommandLineArgRun
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArgRun
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// COM1 and COM2 will be opened in this test case
		/// (WorkspaceArgs refers to WorkspaceSettingsTestCase.W_04_Matthias).
		/// </remarks>
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void TestWorkspaceCommandLineArgRun()
		{
			var workspaceFilePathForTest = CloneForTest(WorkspaceFilePath_TestCase04, "04 - *.*");
			using (var m = new Main(new string[]{ workspaceFilePathForTest }))
			{
				RunAndVerifyApplicationWithoutView(m);
			}
		}

		#endregion

		#region Tests > SerialPortCommandLineArgRun
		//------------------------------------------------------------------------------------------
		// Tests > SerialPortCommandLineArgRun
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSerialPortCommandLineArgRun()
		{
			using (var m = new Main(SerialPortArgs))
			{
				RunAndVerifyApplicationWithoutView(m);
			}
		}

		#endregion

		#region Tests > EmptyCommandLineRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > EmptyCommandLineRunInteractive
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, InteractiveCategory]
		public virtual void TestEmptyCommandLineRunInteractive()
		{
			var dr = MessageBoxEx.Show
			(
				"This test will open YAT and show the [New Terminal] dialog." + Environment.NewLine +
				"(YAT will be called [NUnit] due to the NUnit environment)." + Environment.NewLine +
				Environment.NewLine +
				"Simply exit YAT to complete this test.",
				"Instruction",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Ignore("Tester has canceled");

			using (var m = new Main(EmptyArgs))
			{
				RunAndVerifyApplicationWithView(m);
			}
		}

		#endregion

		#region Tests > TerminalCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > TerminalCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, InteractiveCategory]
		public virtual void TestTerminalCommandLineArgRunInteractive()
		{
			var dr = MessageBoxEx.Show
			(
				"This test will open YAT with a serial COM port terminal." + Environment.NewLine +
				"(YAT will be called [NUnit] due to the NUnit environment)." + Environment.NewLine +
				Environment.NewLine +
				"Simply exit YAT to complete this test.",
				"Instruction",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Ignore("Tester has canceled");

			var terminalFilePathForTest = CloneForTest(TerminalFilePath_TestCase03, "03 - *.*");
			using (var m = new Main(new string[]{ terminalFilePathForTest }))
			{
				RunAndVerifyApplicationWithView(m);
			}
		}

		#endregion

		#region Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// COM1 and COM2 will be opened in this test case
		/// (WorkspaceArgs refers to WorkspaceSettingsTestCase.W_04_Matthias).
		/// </remarks>
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory, MKY.IO.Ports.Test.PortBIsAvailableCategory, InteractiveCategory]
		public virtual void TestWorkspaceCommandLineArgRunInteractive()
		{
			var dr = MessageBoxEx.Show
			(
				"This test will open YAT with two serial COM port terminals." + Environment.NewLine +
				"(YAT will be called [NUnit] due to the NUnit environment)." + Environment.NewLine +
				Environment.NewLine +
				"Simply exit YAT to complete this test.",
				"Instruction",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Ignore("Tester has canceled");

			var workspaceFilePathForTest = CloneForTest(WorkspaceFilePath_TestCase04, "04 - *.*");
			using (var m = new Main(new string[]{ workspaceFilePathForTest }))
			{
				RunAndVerifyApplicationWithView(m);
			}
		}

		#endregion

		#region Tests > SerialPortCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > SerialPortCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, InteractiveCategory]
		public virtual void TestSerialPortCommandLineArgRunInteractive()
		{
			var dr = MessageBoxEx.Show
			(
				"This test will open YAT with a serial COM port terminal." + Environment.NewLine +
				"(YAT will be called [NUnit] due to the NUnit environment)." + Environment.NewLine +
				Environment.NewLine +
				"Simply exit YAT to complete this test.",
				"Instruction",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Ignore("Tester has canceled");

			using (var m = new Main(SerialPortArgs))
			{
				RunAndVerifyApplicationWithView(m);
			}
		}

		#endregion

		#region Tests > ClearedOptions
		//------------------------------------------------------------------------------------------
		// Tests > ClearedOptions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestClearedOptions()
		{
			using (var m = new Main(null))
			{
				PrepareMainAndVerifyResult(m, MainResult.Success);

				Assert.That(m.CommandLineIsValid,         Is.True);
				Assert.That(m.CommandLineHelpIsRequested, Is.False);
				Assert.That(m.CommandLineLogoIsRequested, Is.True);
			}
		}

		#endregion

		#region Tests > SetOptions
		//------------------------------------------------------------------------------------------
		// Tests > SetOptions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSetOptions()
		{
			using (var m = new Main(new string[] { "--NoLogo" }))
			{
				PrepareMainAndVerifyResult(m, MainResult.Success);

				Assert.That(m.CommandLineIsValid,         Is.True);
				Assert.That(m.CommandLineLogoIsRequested, Is.False);
			}
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private static void PrepareMainAndVerifyResult(Main main, MainResult expectedMainResult)
		{
			var mainResult = main.PrepareRun();
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		private static void RunAndVerifyApplicationWithView(Main main)
		{
			RunAndVerifyApplicationWithView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithView(Main main, MainResult expectedMainResult)
		{
			var mainResult = main.Run(false, true);
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		private static void RunAndVerifyApplicationWithoutView(Main main)
		{
			RunAndVerifyApplicationWithoutView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithoutView(Main main, MainResult expectedMainResult)
		{
			var mainResult = main.Run(false, false);
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		/// <summary>
		/// Clones files for a test case.
		/// </summary>
		protected virtual string CloneForTest(string filePath, string fileNamePatternToClone)
		{
			var path = Path.GetDirectoryName(filePath);
			foreach (var src in Directory.GetFiles(path, fileNamePatternToClone))
			{
				var dest = this.tempPath + Path.DirectorySeparatorChar + Path.GetFileName(src);
				File.Copy(src, dest, true);
			}

			var clonedFilePath = this.tempPath + Path.DirectorySeparatorChar + Path.GetFileName(filePath);
			return (clonedFilePath);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
