//==================================================================================================
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

using NUnit.Framework;

using MKY.Utilities.Settings;

using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

namespace YAT.Settings.Test
{
	/// <summary></summary>
	[TestFixture]
	public class FileVersionsTest
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool autoSaveWorkspaceToRestore;
		private MKY.IO.Ports.SerialPortCollection serialPorts;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public FileVersionsTest()
		{
			// serial ports
			this.serialPorts = new MKY.IO.Ports.SerialPortCollection();
			this.serialPorts.FillWithAvailablePorts(false);
		}

		#endregion

		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		/// <summary></summary>
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// prevent auto-save of workspace settings
			this.autoSaveWorkspaceToRestore = ApplicationSettings.LocalUser.General.AutoSaveWorkspace;
			ApplicationSettings.LocalUser.General.AutoSaveWorkspace = false;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
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

		#region Tests > YAT 2.0 Beta 1 Version 1.99.12 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 1 Version 1.99.12 > Terminal
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_12_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_12_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal, true); // Ignore baud rate because it changed from enum to int.
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[Test]
		public virtual void Test_V1_99_12_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13 > Terminal
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_13_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_13_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[Test]
		public virtual void Test_V1_99_13_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13 > Workspace
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13 > Workspace
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void Test_V1_99_13_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17 > Terminal
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_17_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_17_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[Test]
		public virtual void Test_V1_99_17_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17 > Workspace
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17 > Workspace
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void Test_V1_99_17_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18 > Terminal
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_18_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_18_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[Test]
		public virtual void Test_V1_99_18_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18 > Workspace
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18 > Workspace
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void Test_V1_99_18_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_18.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19 > Terminal
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_19_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_19_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[Test]
		public virtual void Test_V1_99_19_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19 > Workspace
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19 > Workspace
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void Test_V1_99_19_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_19.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20 > Terminal
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_20_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_20_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[Test]
		public virtual void Test_V1_99_20_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20 > Workspace
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20 > Workspace
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void Test_V1_99_20_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_20.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22 > Terminal
		//------------------------------------------------------------------------------------------

		/// <summary>01_COM1_Open_Default.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortAIsAvailableCategory]
		public virtual void Test_V1_99_22_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		/// <summary>02_COM2_Open_Binary_115200.</summary>
		[Test, MKY.IO.Ports.Test.SerialPortBIsAvailableCategory]
		public virtual void Test_V1_99_22_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		/// <summary>03_COM1_Closed_Predefined.</summary>
		[Test]
		public virtual void Test_V1_99_22_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22 > Workspace
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22 > Workspace
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void Test_V1_99_22_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_22.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#endregion

		#region Set Up Settings
		//==========================================================================================
		// Set Up Settings
		//==========================================================================================

		private DocumentSettingsHandler<TerminalSettingsRoot> SetupTerminalSettingsFromFilePath(string filePath)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();
			settingsHandler.SettingsFilePath = filePath;
			if (!settingsHandler.Load())
			{
				Assert.Fail("Terminal settings could not be loaded from " + settingsHandler.SettingsFilePath);
			}
			return (settingsHandler);
		}

		private DocumentSettingsHandler<WorkspaceSettingsRoot> SetupWorkspaceSettingsFromFilePath(string filePath)
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
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1");

			if (this.serialPorts.Contains(1))
				Assert.IsTrue(terminal.IsOpen, "Terminal is not open on COM1");
			else
				Assert.Ignore("COM1 isn't supported on this machine");
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
			Assert.AreEqual(Domain.TerminalType.Binary, terminal.SettingsRoot.TerminalType, "Terminal isn't binary");
			Assert.AreEqual(2, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM2");

			if (!ignoreBaudRate)
				Assert.AreEqual(115200, terminal.SettingsRoot.IO.SerialPort.Communication.BaudRate, "Serial port baud rate isn't set to 115200");

			if (this.serialPorts.Contains(2))
				Assert.IsTrue(terminal.IsOpen, "Terminal is not open on COM2");
			else
				Assert.Ignore("COM2 isn't supported on this machine");
		}

		#endregion

		#region Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------

		private void VerifySettingsCase03(Model.Terminal terminal)
		{
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1");
			Assert.IsFalse(terminal.IsOpen, "Terminal is not closed on COM1");

			Assert.AreEqual(2, terminal.SettingsRoot.PredefinedCommand.Pages.Count, "Predefined commands do not contain 2 pages");

			Model.Types.PredefinedCommandPage page;
			Model.Types.Command command;

			page = terminal.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.AreEqual("First Page", page.PageName, "First predefined command pages has wrong name");
			Assert.AreEqual(4, page.Commands.Count, "First predefined command page doesn't contain 4 commands");
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
			Assert.AreEqual("Second Page", page.PageName, "Second predefined command pages has wrong name");
			Assert.AreEqual(4, page.Commands.Count, "Second predefined command page doesn't contain 4 commands");
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

		private void VerifySettingsCase04(Model.Workspace workspace)
		{
			Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals");
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
