//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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

using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.Settings.Test
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Text is given by the test case identification/name.")]
	[TestFixture]
	public class FileVersionsTest
	{
		private const string UnderscoreSuppressionJustification = "As always, there are exceptions to the rules...";

		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
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
			// Close temporary in-memory application settings.
			ApplicationSettings.Close();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > YAT 2.0 Beta 1 Version 1.99.12
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 1 Version 1.99.12
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_12_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_12_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal, true); // Ignore baud rate because it changed from enum to int.
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_12_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_13_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_13_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_13_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_13_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_17_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_17_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_17_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_17_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_18_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_18_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_18_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_18_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_19_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_19_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_19_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_19_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_20_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_20_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_20_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_20_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_22_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_22_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_22_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_22_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 2 Version 1.99.24
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 2 Version 1.99.24
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_24_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_24_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_24_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_24_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_24.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_24_TerminalSettingsCase05()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase05(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 3 Version 1.99.25
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 3 Version 1.99.25
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_25_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_25_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_25.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_TerminalSettingsCase05()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase05(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_WorkspaceSettingsCase06()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_25.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase06(workspace);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_TerminalSettingsCase07()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase07(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 4 Version 1.99.26
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 4 Version 1.99.26
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_26_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_26_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_TerminalSettingsCase05()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase05(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_WorkspaceSettingsCase06()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase06(workspace);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_TerminalSettingsCase07()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase07(terminal);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_WorkspaceSettingsCase08()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_26.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase08(workspace);
			}
		}

		#endregion

		#endregion

		#region Set Up Settings
		//==========================================================================================
		// Set Up Settings
		//==========================================================================================

		private static DocumentSettingsHandler<TerminalSettingsRoot> SetupTerminalSettingsFromFilePath(string filePath)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();
			settingsHandler.SettingsFilePath = filePath;
			if (!settingsHandler.Load())
			{
				Assert.Fail("Terminal settings could not be loaded from " + settingsHandler.SettingsFilePath);
			}
			return (settingsHandler);
		}

		private static DocumentSettingsHandler<WorkspaceSettingsRoot> SetupWorkspaceSettingsFromFilePath(string filePath)
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			settingsHandler.SettingsFilePath = filePath;
			if (!settingsHandler.Load())
			{
				Assert.Fail("Workspace settings could not be loaded from " + settingsHandler.SettingsFilePath);
			}
			return (settingsHandler);
		}

		#endregion

		#region Settings Case Verifications
		//==========================================================================================
		// Settings Case Verifications
		//==========================================================================================

		#region Settings Case Verifications > 01 :: Terminal :: COM1 / Open / Default
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 01 :: Terminal :: COM1 / Open / Default
		//------------------------------------------------------------------------------------------

		private void VerifySettingsCase01(Model.Terminal terminal)
		{
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1!");

			if (MKY.IO.Ports.SerialPortId.Equals(MKY.IO.Ports.Test.SettingsProvider.Settings.SerialPortA, "COM1"))
			{
				if (MKY.IO.Ports.Test.SettingsProvider.Settings.SerialPortAIsAvailable)
					Assert.IsTrue(terminal.IsOpen, "Terminal is not open on COM1!");
				else
					Assert.Ignore("'SerialPortA' is configured to 'COM1' but isn't available on this machine.");
			}
			else
			{
				Assert.Fail("This test case requires that 'SerialPortA' is configured to 'COM1'!");
			}
		}

		#endregion

		#region Settings Case Verifications > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------

		private void VerifySettingsCase02(Model.Terminal terminal)
		{
			VerifySettingsCase02(terminal, false);
		}

		private void VerifySettingsCase02(Model.Terminal terminal, bool ignoreBaudRate)
		{
			Assert.AreEqual(Domain.TerminalType.Binary, terminal.SettingsRoot.TerminalType, "Terminal isn't binary!");
			Assert.AreEqual(2, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM2!");

			if (!ignoreBaudRate)
				Assert.AreEqual(115200, terminal.SettingsRoot.IO.SerialPort.Communication.BaudRate, "Serial port baud rate isn't set to 115200!");

			if (MKY.IO.Ports.SerialPortId.Equals(MKY.IO.Ports.Test.SettingsProvider.Settings.SerialPortB, "COM2"))
			{
				if (MKY.IO.Ports.Test.SettingsProvider.Settings.SerialPortBIsAvailable)
					Assert.IsTrue(terminal.IsOpen, "Terminal is not open on COM2!");
				else
					Assert.Ignore("'SerialPortB' is configured to 'COM2' but isn't available on this machine.");
			}
			else
			{
				Assert.Fail("This test case requires that 'SerialPortB' is configured to 'COM2'!");
			}
		}

		#endregion

		#region Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase03(Model.Terminal terminal)
		{
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1!");
			Assert.IsFalse(terminal.IsOpen, "Terminal is not closed on COM1!");

			Assert.AreEqual(2, terminal.SettingsRoot.PredefinedCommand.Pages.Count, "Predefined commands do not contain 2 pages!");

			Model.Types.PredefinedCommandPage page;
			Model.Types.Command command;

			page = terminal.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.AreEqual("First Page", page.PageName, "First predefined command pages has wrong name!");
			Assert.AreEqual(4, page.Commands.Count, "First predefined command page doesn't contain 4 commands!");
			command = page.Commands[0];
			Assert.AreEqual("1A", command.Description);
			Assert.AreEqual("1A", command.CommandLines[0]);
			command = page.Commands[1];
			Assert.AreEqual("1B", command.Description);
			Assert.AreEqual("1B", command.CommandLines[0]);
			command = page.Commands[2];
			Assert.AreEqual("1C", command.Description);
			Assert.AreEqual("1C", command.CommandLines[0]);
			command = page.Commands[3];
			Assert.AreEqual("1D", command.Description);
			Assert.AreEqual("1D", command.CommandLines[0]);

			page = terminal.SettingsRoot.PredefinedCommand.Pages[1];
			Assert.AreEqual("Second Page", page.PageName, "Second predefined command pages has wrong name!");
			Assert.AreEqual(4, page.Commands.Count, "Second predefined command page doesn't contain 4 commands!");
			command = page.Commands[0];
			Assert.AreEqual("2A", command.Description);
			Assert.AreEqual("2A", command.CommandLines[0]);
			command = page.Commands[1];
			Assert.AreEqual("2B", command.Description);
			Assert.AreEqual("2B", command.CommandLines[0]);
			command = page.Commands[2];
			Assert.AreEqual("2C", command.Description);
			Assert.AreEqual("2C", command.CommandLines[0]);
			command = page.Commands[3];
			Assert.AreEqual("2D", command.Description);
			Assert.AreEqual("2D", command.CommandLines[0]);
		}

		#endregion

		#region Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase04(Model.Workspace workspace)
		{
			Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals!");
		}

		#endregion

		#region Settings Case Verifications > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase05(Model.Terminal terminal)
		{
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1!");

			// \todo:
			// Add tests that verify that recent contains three commands.
			// Add tests that verify that recent files contains three files commands.
		}

		#endregion

		#region Settings Case Verifications > 06 :: Workspace :: 2 TCP AutoSocket Terminals
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 06 :: Workspace :: 2 TCP AutoSocket Terminals
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase06(Model.Workspace workspace)
		{
			Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals!");

			// \todo:
			// Add tests that verify that terminals are interconnected.
		}

		#endregion

		#region Settings Case Verifications > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2200) MK.8 / Closed
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2200) MK.8 / Closed
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase07(Model.Terminal terminal)
		{
			Assert.AreEqual(Domain.IOType.UsbSerialHid, terminal.SettingsRoot.IOType, "Terminal isn't USB Ser/HID!");

			// \todo:
			// Add tests that verify that USB device info is correct.
		}

		#endregion

		#region Settings Case Verifications > 08 :: Workspace :: 2 TCP AutoSocket Terminals with Unicode Predefined
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 08 :: Workspace :: 2 TCP AutoSocket Terminals with Unicode Predefined
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "terminal2", Justification = "See ToDo's below.")]
		private static void VerifySettingsCase08(Model.Workspace workspace)
		{
			Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals!");

			Model.Terminal terminal1 = workspace.Terminals[0];
			Model.Terminal terminal2 = workspace.Terminals[1];

			Assert.AreEqual(1, terminal1.SettingsRoot.PredefinedCommand.Pages.Count, "Predefined commands do not contain 1 page!");

			Model.Types.PredefinedCommandPage page;
			Model.Types.Command command;

			page = terminal1.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.AreEqual("Page 1", page.PageName, "First predefined command pages has wrong name!");
			Assert.AreEqual(2, page.Commands.Count, "First predefined command page doesn't contain 2 commands!");
			command = page.Commands[0];
			Assert.AreEqual("äöü", command.Description);
			Assert.AreEqual("äöü", command.CommandLines[0]);
			command = page.Commands[1];
			Assert.AreEqual("ÄÖÜ", command.Description);
			Assert.AreEqual("ÄÖÜ", command.CommandLines[0]);

			// \todo:
			// Add tests that verify that terminals are interconnected.

			// \todo:
			// Add tests that send the commands to terminal 2.

			// \todo:
			// Add tests that verify the commands at terminal 2.
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
