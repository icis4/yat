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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NUnit;
using NUnit.Framework;

using MKY;
using MKY.Collections.Generic;

using YAT.Settings.Application;

#endregion

namespace YAT.Controller.Test
{
	/// <summary></summary>
	public static class OptionFormatsAndHelpTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<bool, string[]>> TestCasePairs
		{
			get
			{
				yield return (new Pair<bool, string[]>(true, new string[] { "/?"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-?"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--?"    }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/h"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-h"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--h"    }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/H"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-H"     }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--H"    }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--help" }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/Help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-Help"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--Help" }));
				yield return (new Pair<bool, string[]>(true, new string[] { "/hELp"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "-HeLp"  }));
				yield return (new Pair<bool, string[]>(true, new string[] { "--HELP" }));

				yield return (new Pair<bool, string[]>(true, new string[] { "/?", "-h", "--help", "-HELP", "--h" }));

				yield return (new Pair<bool, string[]>(false, new string[] { "/??"      }));
				yield return (new Pair<bool, string[]>(false, new string[] { "-hh"      }));
				yield return (new Pair<bool, string[]>(false, new string[] { "--helper" }));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<bool, string[]> pair in TestCasePairs)
					yield return (new TestCaseData(pair.Value1, pair.Value2).SetName(ArrayEx.ElementsToString(pair.Value2)));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class ControllerTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private readonly string[] EmptyArgs = new string[] {};
		private readonly string[] TerminalArgs  = new string[] { Settings.Test.SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[Settings.Test.TerminalSettingsTestCases.T_03_COM1_Closed_Predefined] };
		private readonly string[] WorkspaceArgs = new string[] { Settings.Test.SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[Settings.Test.WorkspaceSettingsTestCases.W_04_Matthias] };

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
				Assert.IsTrue(main.CommandLineIsValid);
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
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
				StringAssert.AreEqualIgnoringCase(TerminalArgs[0], main.RequestedFilePath, "Invalid requested terminal settings file path!");
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
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
				StringAssert.AreEqualIgnoringCase(WorkspaceArgs[0], main.RequestedFilePath, "Invalid requested workspace settings file path!");
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
		[InteractiveCategory]
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
		[InteractiveCategory]
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
		[InteractiveCategory]
		public virtual void TestWorkspaceCommandLineArgRunInteractive()
		{
			using (Controller.Main main = new Main(WorkspaceArgs))
			{
				RunAndVerifyApplication(main);
			}
		}

		#endregion

		#region Tests > OptionFormatsAndHelp
		//------------------------------------------------------------------------------------------
		// Tests > OptionFormatsAndHelp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(OptionFormatsAndHelpTestData), "TestCases")]
		public virtual void TestOptionFormatsAndHelp(bool isValid, params string[] commandLineArgs)
		{
			using (Controller.Main main = new Main(commandLineArgs))
			{
				if (isValid)
				{
					Assert.IsTrue(main.CommandLineIsValid);
					Assert.IsTrue(main.CommandLineHelpIsRequested);
				}
				else
				{
					Assert.IsFalse(main.CommandLineIsValid);
				}
			}
		}

		#endregion

		#region Tests > OptionValueFormats
		//------------------------------------------------------------------------------------------
		// Tests > OptionValueFormats
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestOptionValueFormats()
		{
			using (Controller.Main main = new Main(new string[] { "--Terminal=1" }))
			{
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.AreEqual(1, main.RequestedDynamicTerminalIndex);
			}

			using (Controller.Main main = new Main(new string[] { "--Terminal:1" }))
			{
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.AreEqual(1, main.RequestedDynamicTerminalIndex);
			}

			using (Controller.Main main = new Main(new string[] { "--Terminal", "1" }))
			{
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.AreEqual(1, main.RequestedDynamicTerminalIndex);
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
			using (Controller.Main main = new Main(null))
			{
				Assert.IsTrue (main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineHelpIsRequested);
				Assert.IsTrue (main.CommandLineLogoIsRequested);

				Assert.IsTrue(string.IsNullOrEmpty(main.RequestedFilePath));
				Assert.IsFalse(main.MostRecentIsRequested);
				Assert.AreEqual(0, main.RequestedDynamicTerminalIndex);
				Assert.IsTrue(string.IsNullOrEmpty(main.RequestedTransmitFilePath));
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
			using (Controller.Main main = new Main(new string[] { "--NoLogo" }))
			{
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.IsFalse(main.CommandLineLogoIsRequested);
			}

			using (Controller.Main main = new Main(new string[] { "--Recent" }))
			{
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.IsTrue(main.MostRecentIsRequested);
			}

			using (Controller.Main main = new Main(new string[] { "--Terminal=1" }))
			{
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.AreEqual(1, main.RequestedDynamicTerminalIndex);
			}

			using (Controller.Main main = new Main(new string[] { "--TransmitFile=MyFile.txt" }))
			{
				Assert.IsTrue(main.CommandLineIsValid);
				Assert.IsFalse(string.IsNullOrEmpty(main.RequestedTransmitFilePath));
			}
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
