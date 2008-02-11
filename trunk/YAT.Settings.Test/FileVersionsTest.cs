using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using MKY.Utilities.Settings;

using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

namespace YAT.Settings.Test
{
	[TestFixture]
	public class FileVersionsTest
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _autoSaveWorkspaceToRestore;
		private MKY.IO.Ports.SerialPortList _serialPorts;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public FileVersionsTest()
		{
			// serial ports
			_serialPorts = new MKY.IO.Ports.SerialPortList();
			_serialPorts.FillWithAvailablePorts();
		}

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

		#region Tests > YAT 2.0 Beta 1 Version 1.99.12 > Terminal
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 1 Version 1.99.12 > Terminal
		//------------------------------------------------------------------------------------------

		// 01_COM1_Open_Default
		[Test]
		[Category("Support for COM1 required")]
		public void Test_V1_99_12_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// create terminal from settings and check whether settings are correctly set
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		// 02_COM2_Open_Binary_115200
		[Test]
		[Category("Support for COM2 required")]
		public void Test_V1_99_12_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// create terminal from settings and check whether settings are correctly set
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal, true); // ignore baud rate because it changed from enum to int
			}
		}

		// 03_COM1_Closed_Predefined
		[Test]
		public void Test_V1_99_12_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// create terminal from settings and check whether settings are correctly set
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

		// 01_COM1_Open_Default
		[Test]
		[Category("Support for COM1 required")]
		public void Test_V1_99_13_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// create terminal from settings and check whether settings are correctly set
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		// 02_COM2_Open_Binary_115200
		[Test]
		[Category("Support for COM2 required")]
		public void Test_V1_99_13_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// create terminal from settings and check whether settings are correctly set
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		// 03_COM1_Closed_Predefined
		[Test]
		public void Test_V1_99_13_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// create terminal from settings and check whether settings are correctly set
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

		[Test]
		public void Test_V1_99_13_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_13.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// create workspace from settings and check whether settings are correctly set
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

		// 01_COM1_Open_Default
		[Test]
		[Category("Support for COM1 required")]
		public void Test_V1_99_17_TerminalSettingsCase01()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCases.T_01_COM1_Open_Default]
				);

			// create terminal from settings and check whether settings are correctly set
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase01(terminal);
			}
		}

		// 02_COM2_Open_Binary_115200
		[Test]
		[Category("Support for COM2 required")]
		public void Test_V1_99_17_TerminalSettingsCase02()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200]
				);

			// create terminal from settings and check whether settings are correctly set
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				terminal.Start();
				VerifySettingsCase02(terminal);
			}
		}

		// 03_COM1_Closed_Predefined
		[Test]
		public void Test_V1_99_17_TerminalSettingsCase03()
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCases.T_03_COM1_Closed_Predefined]
				);

			// create terminal from settings and check whether settings are correctly set
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

		[Test]
		public void Test_V1_99_17_WorkspaceSettingsCase04()
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath
				(
				SettingsFilesProvider.FilePaths_V1_99_17.WorkspaceFilePaths[WorkspaceSettingsTestCases.W_04_Matthias]
				);

			// create workspace from settings and check whether settings are correctly set
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
			Assert.AreEqual(terminal.SettingsRoot.IO.SerialPort.PortId, 1, "Serial port isn't set to COM1");

			if (_serialPorts.Contains(1))
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
			Assert.AreEqual(terminal.SettingsRoot.TerminalType, Domain.TerminalType.Binary, "Terminal isn't binary");
			Assert.AreEqual(terminal.SettingsRoot.IO.SerialPort.PortId, 2, "Serial port isn't set to COM2");

			if (!ignoreBaudRate)
				Assert.AreEqual(terminal.SettingsRoot.IO.SerialPort.Communication.BaudRate, 115200, "Serial port baud rate isn't set to 115200");

			if (_serialPorts.Contains(2))
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
			Assert.AreEqual(terminal.SettingsRoot.IO.SerialPort.PortId, 1, "Serial port isn't set to COM1");
			Assert.IsFalse(terminal.IsOpen, "Terminal is not closed on COM1");

			Assert.AreEqual(terminal.SettingsRoot.PredefinedCommand.Pages.Count, 2, "Predefined commands do not contain 2 pages");

			Model.Types.PredefinedCommandPage page;
			Model.Types.Command command;

			page = terminal.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.AreEqual(page.PageName, "First Page", "First predefined command pages has wrong name");
			Assert.AreEqual(page.Commands.Count, 4, "First predefined command page doesn't contain 4 commands");
			command = page.Commands[0];
			Assert.AreEqual(command.Description, "1A");
			Assert.AreEqual(command.CommandLines[0], "1A");
			command = page.Commands[1];
			Assert.AreEqual(command.Description, "1B");
			Assert.AreEqual(command.CommandLines[0], "1B");
			command = page.Commands[2];
			Assert.AreEqual(command.Description, "1C");
			Assert.AreEqual(command.CommandLines[0], "1C");
			command = page.Commands[3];
			Assert.AreEqual(command.Description, "1D");
			Assert.AreEqual(command.CommandLines[0], "1D");

			page = terminal.SettingsRoot.PredefinedCommand.Pages[1];
			Assert.AreEqual(page.PageName, "Second Page", "Second predefined command pages has wrong name");
			Assert.AreEqual(page.Commands.Count, 4, "Second predefined command page doesn't contain 4 commands");
			command = page.Commands[0];
			Assert.AreEqual(command.Description, "2A");
			Assert.AreEqual(command.CommandLines[0], "2A");
			command = page.Commands[1];
			Assert.AreEqual(command.Description, "2B");
			Assert.AreEqual(command.CommandLines[0], "2B");
			command = page.Commands[2];
			Assert.AreEqual(command.Description, "2C");
			Assert.AreEqual(command.CommandLines[0], "2C");
			command = page.Commands[3];
			Assert.AreEqual(command.Description, "2D");
			Assert.AreEqual(command.CommandLines[0], "2D");
		}

		#endregion

		#region Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------

		private void VerifySettingsCase04(Model.Workspace workspace)
		{
			Assert.AreEqual(workspace.TerminalCount, 2, "Workspace doesn't contain 2 terminals");
		}

		#endregion

		#endregion
	}
}
