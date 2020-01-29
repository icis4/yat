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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
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

using YAT.Settings.Model.Test;

#endregion

namespace YAT.Application.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ApplicationTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] EmptyArgs = new string[] { };

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Orthogonality with underlying test case.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Orthogonality with underlying test case.")]
		private readonly string TerminalFilePath_TestCase03 = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Orthogonality with underlying test case.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Orthogonality with underlying test case.")]
		private readonly string WorkspaceFilePath_TestCase04 = SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] SerialPortArgs = new string[] { "--TerminalType=Binary", "--SerialPort=101", "--DataBits=7", "--Parity=E", "--FlowControl=Software" };
		                                                                                                //// \remind (2019-10-02 / MKY) could be migrated to use configured port A instead.
		#endregion

		#region Fields
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		private string tempPath;

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
			tempPath = Temp.MakeTempPath(GetType());

			// Temporary in-memory application settings are useless for this YAT.Application based
			// test, as YAT.Application will retrieve the application settings, that's its job...

			// There is another issue when running YAT.Controller from NUnit:
			// The first test that invokes YAT, i.e. the first time the welcome screen should get
			// shown, leads to an invalid [DialogResult] value of the welcome screen:
			//
			// YAT.View.Forms.WelcomeScreen : Invoking LoadApplicationSettingsAsync()...
			// YAT.Application.Main         : ...showing...
			// YAT.View.Forms.WelcomeScreen : ...loading application settings...
			// YAT.View.Forms.WelcomeScreen : ...successfully done.
			// YAT.Application.Forms.WelcomeScreen : Closing dialog, result is [OK].
			// YAT.Controller.Main          : ...failed with [Cancel]!
			//
			// The returned value is [Cancel] instead of [OK]! The root cause to this issue has
			// not been found (yet). It could be related to the fact that the NUnit-GUI "owns" the
			// main thread, and the YAT's welcome screen is shown without that form as parent.
			// This could also be related to the following note in .NET WinForms:
			// "If a Form is displayed as a modeless window, the value returned by the DialogResult
			//  property might not return a value assigned to the form because the form's resources
			//  are automatically released when the form is closed."
			//
			// This issue is worked around by not showing the welcome screen when running NUnit
			// based tests. A welcome screen doesn't make much sense during testing anyway...
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
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
			using (var m = new Main(new string[] { terminalFilePathForTest }))
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
			using (var m = new Main(new string[] { workspaceFilePathForTest }))
			{
				PrepareMainAndVerifyResult(m, MainResult.Success);

				Assert.That(m.CommandLineIsValid,         Is.True);
				Assert.That(m.CommandLineHelpIsRequested, Is.False);
			}
		}

		#endregion

		#region Tests > SerialPortCommandLineArg
		//------------------------------------------------------------------------------------------
		// Tests > SerialPortCommandLineArg
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSerialPortCommandLineArg()
		{
			using (var m = new Main(SerialPortArgs))
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
			using (var m = new Main(new string[] { terminalFilePathForTest }))
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
		/// COM101 and COM102 will be opened in this test case
		/// (WorkspaceArgs refers to WorkspaceSettingsTestCase.W_04_Matthias).
		/// </remarks>
		[Test] // Test is mandatory, it shall not be excludable.
		public virtual void TestWorkspaceCommandLineArgRun()
		{
			AssertWorkspaceCommandLineArgRunPreconditions();

			var workspaceFilePathForTest = CloneForTest(WorkspaceFilePath_TestCase04, "04 - *.*");
			using (var m = new Main(new string[] { workspaceFilePathForTest }))
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
				"Simply [Cancel] and then exit YAT to complete this test.",
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

			dr = MessageBoxEx.Show
			(
				"Did YAT open and showed the [New Terminal] dialog?",
				"Confirmation",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button2
			);

			if (dr != DialogResult.Yes)
				Assert.Fail("Tester has let the test failed!");
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
			using (var m = new Main(new string[] { terminalFilePathForTest }))
			{
				RunAndVerifyApplicationWithView(m);
			}

			dr = MessageBoxEx.Show
			(
				"Did YAT open with a serial COM port terminal?",
				"Confirmation",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button2
			);

			if (dr != DialogResult.Yes)
				Assert.Fail("Tester has let the test failed!");
		}

		#endregion

		#region Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// COM101 and COM102 will be opened in this test case
		/// (WorkspaceArgs refers to WorkspaceSettingsTestCase.W_04_Matthias).
		/// </remarks>
		[Test] // Test is mandatory, it shall not be excludable.
		public virtual void TestWorkspaceCommandLineArgRunInteractive()
		{
			AssertWorkspaceCommandLineArgRunPreconditions();

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
			using (var m = new Main(new string[] { workspaceFilePathForTest }))
			{
				RunAndVerifyApplicationWithView(m);
			}

			dr = MessageBoxEx.Show
			(
				"Did YAT open with two serial COM port terminals?",
				"Confirmation",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button2
			);

			if (dr != DialogResult.Yes)
				Assert.Fail("Tester has let the test failed!");
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

			dr = MessageBoxEx.Show
			(
				"Did YAT open a binary serial COM port terminal automatically sending '11h' (XOn)?",
				"Confirmation",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button2
			);

			if (dr != DialogResult.Yes)
				Assert.Fail("Tester has let the test failed!");
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

		/// <summary>
		/// Asserts that COM101 and COM102 are configured and available.
		/// </summary>
		protected virtual void AssertWorkspaceCommandLineArgRunPreconditions()
		{
			if ((MKY.IO.Ports.SerialPortId)MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortA != "COM101")
				Assert.Fail("This test case requires that 'PortA' is configured to 'COM101'!");

			if ((MKY.IO.Ports.SerialPortId)MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortB != "COM102")
				Assert.Fail("This test case requires that 'PortB' is configured to 'COM102'!");

			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortAIsAvailable)
				Assert.Ignore("'PortA' is configured to 'COM101' but isn't available on this machine, therefore this test is excluded.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortBIsAvailable)
				Assert.Ignore("'PortB' is configured to 'COM102' but isn't available on this machine, therefore this test is excluded.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

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
			var mainResult = main.Run(false, true, ApplicationSettingsFileAccess.None, false); // <= see TestFixtureSetUp() for background why without welcome screen.
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		private static void RunAndVerifyApplicationWithoutView(Main main)
		{
			RunAndVerifyApplicationWithoutView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithoutView(Main main, MainResult expectedMainResult)
		{
			var mainResult = main.Run(false, false, ApplicationSettingsFileAccess.None, false);
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================