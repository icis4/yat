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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

using MKY.Settings;

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
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings.
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
			using (Main main = new Main(EmptyArgs))
			{
				PrepareMainAndVerifyResult(main, MainResult.Success);

				Assert.IsTrue (main.CommandLineIsValid);
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
			using (Main main = new Main(TerminalArgs))
			{
				PrepareMainAndVerifyResult(main, MainResult.Success);

				Assert.IsTrue (main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
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
			using (Main main = new Main(WorkspaceArgs))
			{
				PrepareMainAndVerifyResult(main, MainResult.Success);

				Assert.IsTrue (main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
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
			using (Main main = new Main(EmptyArgs))
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
			using (Main main = new Main(TerminalArgs))
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
		/// <remarks> COM A and COM B will be opened in this test case
		/// (WorkspaceArgs refers to WorkspaceSettingsTestCase.W_04_Matthias).</remarks>
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void TestWorkspaceCommandLineArgRun()
		{
			using (Main main = new Main(WorkspaceArgs))
			{
				RunAndVerifyApplicationWithoutView(main);
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
			using (Main main = new Main(SerialPortArgs))
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
		[Test, InteractiveCategory]
		public virtual void TestEmptyCommandLineRunInteractive()
		{
			using (Main main = new Main(EmptyArgs))
			{
				RunAndVerifyApplicationWithView(main);
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
			using (Main main = new Main(TerminalArgs))
			{
				RunAndVerifyApplicationWithView(main);
			}
		}

		#endregion

		#region Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceCommandLineArgRunInteractive
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <remarks> COM A and COM B will be opened in this test case
		/// (WorkspaceArgs refers to WorkspaceSettingsTestCase.W_04_Matthias).</remarks>
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory, MKY.IO.Ports.Test.PortBIsAvailableCategory, InteractiveCategory]
		public virtual void TestWorkspaceCommandLineArgRunInteractive()
		{
			using (Main main = new Main(WorkspaceArgs))
			{
				RunAndVerifyApplicationWithView(main);
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
			using (Main main = new Main(SerialPortArgs))
			{
				RunAndVerifyApplicationWithView(main);
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
			using (Main main = new Main(null))
			{
				PrepareMainAndVerifyResult(main, MainResult.Success);

				Assert.IsTrue (main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
				Assert.IsTrue (main.CommandLineLogoIsRequested);
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
			using (Main main = new Main(new string[] { "--NoLogo" }))
			{
				PrepareMainAndVerifyResult(main, MainResult.Success);

				Assert.IsTrue (main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineLogoIsRequested);
			}
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private static void PrepareMainAndVerifyResult(Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.PrepareRun();
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		private static void RunAndVerifyApplicationWithView(Main main)
		{
			RunAndVerifyApplicationWithView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithView(Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run(false, true);
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		private static void RunAndVerifyApplicationWithoutView(Main main)
		{
			RunAndVerifyApplicationWithoutView(main, MainResult.Success);
		}

		private static void RunAndVerifyApplicationWithoutView(Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.Run(false, false);
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
