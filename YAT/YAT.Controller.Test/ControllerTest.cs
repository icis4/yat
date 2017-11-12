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
using System.Windows.Forms;

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

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] TerminalArgs = new string[] { Settings.Test.SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[Settings.Test.TerminalSettingsTestCase.T_03_COM1_Closed_Predefined] };

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] WorkspaceArgs = new string[] { Settings.Test.SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[Settings.Test.WorkspaceSettingsTestCase.W_04_Matthias] };

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] SerialPortArgs = new string[] { "--TerminalType=Binary", "--SerialPort=5", "--DataBits=7", "--Parity=E", "--FlowControl=Software" };

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
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
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
			using (var m = new Main(TerminalArgs))
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
			using (var m = new Main(WorkspaceArgs))
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
			using (var m = new Main(TerminalArgs))
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
			// \remind (2016-05-26 / MKY) should be guarded by if (isRunningFromGui) to prevent the message box in case of automatic test runs.
			// \remind (2017-10-09 / MKY) even better to be eliminated and moved to related tests as attributes.
			var dr = MessageBoxEx.Show
			(
				"This test requires open serial ports 'COM1' and 'COM2'." + Environment.NewLine +
				"Ensure that VSPE is running and providing these ports.",
				"Precondition",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Fail("User cancel!");

			using (var m = new Main(WorkspaceArgs))
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
			using (var m = new Main(TerminalArgs))
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
			// \remind (2016-05-26 / MKY) should be guarded by if (isRunningFromGui) to prevent the message box in case of automatic test runs.
			// \remind (2017-10-09 / MKY) even better to be eliminated and moved to related tests as attributes.
			var dr = MessageBoxEx.Show
			(
				"This test requires open serial ports 'COM1' and 'COM2'." + Environment.NewLine +
				"Ensure that VSPE is running and providing these ports.",
				"Precondition",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Fail("User cancel!");

			using (var m = new Main(WorkspaceArgs))
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
			MainResult mainResult = main.PrepareRun();
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		private static void RunAndVerifyApplicationWithView(Main main)
		{
			RunAndVerifyApplicationWithView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithView(Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run(false, true);
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		private static void RunAndVerifyApplicationWithoutView(Main main)
		{
			RunAndVerifyApplicationWithoutView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithoutView(Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run(false, false);
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
