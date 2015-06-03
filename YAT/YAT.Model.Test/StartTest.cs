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
// YAT 2.0 Gamma 1' Version 1.99.33
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
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

using MKY;
using MKY.Collections.Generic;
using MKY.Settings;

using NUnit;
using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Test;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[TestFixture]
	public class StartTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is meant to be a constant.")]
		private readonly string TerminalFilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM1_Closed_Default];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is meant to be a constant.")]
		private readonly string Terminal1FilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM1_Closed_Default];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is meant to be a constant.")]
		private readonly string Terminal2FilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM2_Closed_Default];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is meant to be a constant.")]
		private readonly string WorkspaceFilePath = SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias];

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
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;

			// Ensure that recents contains files.
			ApplicationSettings.LocalUserSettings.RecentFiles = new Model.Settings.RecentFileSettings();
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Add(TerminalFilePath);
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Add(WorkspaceFilePath);
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

		#region Tests > EmptyPrepare
		//------------------------------------------------------------------------------------------
		// Tests > EmptyPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyPrepare()
		{
			using (Model.Main main = new Main())
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull(main.StartArgs.WorkspaceSettings);
				Assert.IsNull(main.StartArgs.TerminalSettings);
				Assert.IsTrue(main.StartArgs.ShowNewTerminalDialog);

				Assert.IsFalse(main.StartArgs.PerformOperationOnRequestedTerminal);
				Assert.AreEqual(Indices.InvalidDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.IsNullOrEmpty(main.StartArgs.RequestedTransmitFilePath);

				Assert.IsTrue (main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > TerminalFilePathPrepare
		//------------------------------------------------------------------------------------------
		// Tests > TerminalFilePathPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTerminalFilePathPrepare()
		{
			using (Model.Main main = new Main(TerminalFilePath))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull   (main.StartArgs.WorkspaceSettings);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettings.SettingsFilePath);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull   (main.StartArgs.WorkspaceSettings);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettings.SettingsFilePath);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}
		}

		#endregion

		#region Tests > WorkspaceFilePathPrepare
		//------------------------------------------------------------------------------------------
		// Tests > WorkspaceFilePathPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestWorkspaceFilePathPrepare()
		{
			using (Model.Main main = new Main(WorkspaceFilePath))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.WorkspaceSettings);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettings.SettingsFilePath);
				Assert.IsNull   (main.StartArgs.TerminalSettings);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.WorkspaceSettings);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettings.SettingsFilePath);
				Assert.IsNull   (main.StartArgs.TerminalSettings);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}
		}

		#endregion

		#region Tests > OpenOptionPrepare
		//------------------------------------------------------------------------------------------
		// Tests > OpenOptionPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestOpenOptionPrepare()
		{
			// Workspace only.
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.WorkspaceSettings);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettings.SettingsFilePath);
				Assert.IsNull   (main.StartArgs.TerminalSettings);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}

			// Terminal only.
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull   (main.StartArgs.WorkspaceSettings);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (Terminal1FilePath, main.StartArgs.TerminalSettings.SettingsFilePath);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}

			// Workspace + Terminal = Terminal. (The last argument is used.)
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + WorkspaceFilePath, "--Open=" + Terminal1FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull   (main.StartArgs.WorkspaceSettings);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (Terminal1FilePath, main.StartArgs.TerminalSettings.SettingsFilePath);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}

			// Terminal + Workspace = Workspace. (The last argument is used.)
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath, "--Open=" + WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.WorkspaceSettings);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettings.SettingsFilePath);
				Assert.IsNull   (main.StartArgs.TerminalSettings);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}

			// Terminal1 + Terminal2 = Terminal2. (The last argument is used.)
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath, "--Open=" + Terminal2FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull   (main.StartArgs.WorkspaceSettings);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (Terminal2FilePath, main.StartArgs.TerminalSettings.SettingsFilePath);
				Assert.IsFalse  (main.StartArgs.ShowNewTerminalDialog);
			}

			// Invalid file.
			string invalidFilePath = "MyFile.txt";
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + invalidFilePath })))
			{
				PrepareMainAndVerifyResult(main, Main.Result.CommandLineError);

				Assert.IsNull(main.StartArgs.WorkspaceSettings);
				Assert.IsNull(main.StartArgs.TerminalSettings);
			}
		}

		#endregion

		#region Tests > RecentOptionPrepare
		//------------------------------------------------------------------------------------------
		// Tests > RecentOptionPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestRecentOptionPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Recent" })))
			{
				PrepareMainAndVerifyResult(main);

				bool hasRecent = ((main.StartArgs.TerminalSettings != null) || ((main.StartArgs.WorkspaceSettings != null)));
				Assert.IsTrue (hasRecent);
				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);
			}
		}

		#endregion

		#region Tests > NewOptionPrepare
		//------------------------------------------------------------------------------------------
		// Tests > NewOptionPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestNewOptionPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--New" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);
			}
		}

		#endregion

		#region Tests > NewSerialPortOptionsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > NewSerialPortOptionsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestNewSerialPortOptionsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--New", "--TerminalType=Binary", "--SerialPort=5", "--DataBits=7", "--Parity=E", "--FlowControl=Software" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.TerminalSettings);

				Assert.AreEqual(Domain.TerminalType.Binary, main.StartArgs.TerminalSettings.Settings.TerminalType);
				Assert.AreEqual(Domain.IOType.SerialPort,   main.StartArgs.TerminalSettings.Settings.IOType);

				Assert.AreEqual(5, (int)main.StartArgs.TerminalSettings.Settings.IO.SerialPort.PortId); // COM5

				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialCommunicationSettings.BaudRateDefault,   main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.BaudRate);
				Assert.AreEqual(MKY.IO.Ports.DataBits.Seven,                                            main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.DataBits);
				Assert.AreEqual(System.IO.Ports.Parity.Even,                                            main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.Parity);
				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialCommunicationSettings.StopBitsDefault,   main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.StopBits);
				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialFlowControl.Software,                    main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.FlowControl);

				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault.Enabled,  main.StartArgs.TerminalSettings.Settings.IO.SerialPort.AutoReopen.Enabled);
				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault.Interval, main.StartArgs.TerminalSettings.Settings.IO.SerialPort.AutoReopen.Interval);

				Assert.IsFalse(main.StartArgs.TerminalSettings.Settings.TerminalIsStarted);
				Assert.IsFalse(main.StartArgs.TerminalSettings.Settings.LogIsStarted);

				Assert.IsFalse(main.StartArgs.PerformOperationOnRequestedTerminal);
				Assert.AreEqual(Indices.InvalidDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.IsNullOrEmpty(main.StartArgs.RequestedTransmitFilePath);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);

				Assert.IsTrue (main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > NewTcpAutoSocketOptionsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > NewTcpAutoSocketOptionsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestNewTcpAutoSocketOptionsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--New", "--IOType=TCPAutoSocket", "--RemotePort=12345", "--LocalPort=56789", "--OpenTerminal", "--BeginLog" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.TerminalSettings);

				Assert.AreEqual(Domain.TerminalType.Text,    main.StartArgs.TerminalSettings.Settings.TerminalType);
				Assert.AreEqual(Domain.IOType.TcpAutoSocket, main.StartArgs.TerminalSettings.Settings.IOType);

				Assert.AreEqual((MKY.Net.IPHost)MKY.Net.IPHostType.Localhost, (MKY.Net.IPHost)main.StartArgs.TerminalSettings.Settings.IO.Socket.RemoteHost);
				Assert.AreEqual(12345, main.StartArgs.TerminalSettings.Settings.IO.Socket.RemotePort);

				Assert.AreEqual((MKY.Net.IPNetworkInterface)MKY.Net.IPNetworkInterfaceType.Any, (MKY.Net.IPNetworkInterface)main.StartArgs.TerminalSettings.Settings.IO.Socket.LocalInterface);
				Assert.AreEqual(56789, main.StartArgs.TerminalSettings.Settings.IO.Socket.LocalPort);

				Assert.IsTrue(main.StartArgs.TerminalSettings.Settings.TerminalIsStarted);
				Assert.IsTrue(main.StartArgs.TerminalSettings.Settings.LogIsStarted);

				Assert.IsFalse(main.StartArgs.PerformOperationOnRequestedTerminal);
				Assert.AreEqual(Indices.InvalidDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.IsNullOrEmpty(main.StartArgs.RequestedTransmitFilePath);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);

				Assert.IsTrue (main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > NewUsbSerialHidOptionsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > NewUsbSerialHidOptionsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestNewUsbSerialHidOptionsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--New", "--IOType=USBSerHID", "--VendorID=1234", "--ProductID=ABCD", "--NoUsbAutoOpen" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.TerminalSettings);

				Assert.AreEqual(Domain.TerminalType.Text,   main.StartArgs.TerminalSettings.Settings.TerminalType);
				Assert.AreEqual(Domain.IOType.UsbSerialHid, main.StartArgs.TerminalSettings.Settings.IOType);

				Assert.AreEqual("1234", main.StartArgs.TerminalSettings.Settings.IO.UsbSerialHidDevice.DeviceInfo.VendorIdString);
				Assert.AreEqual("ABCD", main.StartArgs.TerminalSettings.Settings.IO.UsbSerialHidDevice.DeviceInfo.ProductIdString);
				Assert.IsFalse(main.StartArgs.TerminalSettings.Settings.IO.UsbSerialHidDevice.AutoOpen);

				Assert.IsFalse(main.StartArgs.TerminalSettings.Settings.TerminalIsStarted);
				Assert.IsFalse(main.StartArgs.TerminalSettings.Settings.LogIsStarted);

				Assert.IsFalse(main.StartArgs.PerformOperationOnRequestedTerminal);
				Assert.AreEqual(Indices.InvalidDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.IsNullOrEmpty(main.StartArgs.RequestedTransmitFilePath);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);

				Assert.IsTrue (main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > ReplaceTerminalSettingsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > ReplaceTerminalSettingsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestReplaceTerminalSettingsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath, "--BaudRate=19200" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull   (main.StartArgs.WorkspaceSettings);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettings.SettingsFilePath);

				Assert.AreEqual(19200, main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.BaudRate);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);
				Assert.IsFalse(main.StartArgs.PerformOperationOnRequestedTerminal);

				Assert.IsTrue (main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > ReplaceTerminalSettingsInWorkspacePrepare
		//------------------------------------------------------------------------------------------
		// Tests > ReplaceTerminalSettingsInWorkspacePrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestReplaceTerminalSettingsInWorkspacePrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath, "--Terminal=2", "--DataBits=7" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(                             main.StartArgs.WorkspaceSettings);
				Assert.AreEqual (WorkspaceFilePath,           main.StartArgs.WorkspaceSettings.SettingsFilePath);
				Assert.AreEqual (2,                           main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.IsNotNull(                             main.StartArgs.TerminalSettings);
				Assert.AreEqual (MKY.IO.Ports.DataBits.Seven, main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.DataBits);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);
				Assert.IsFalse(main.StartArgs.PerformOperationOnRequestedTerminal);

				Assert.IsTrue (main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > TransmitFilePathOptionPrepare
		//------------------------------------------------------------------------------------------
		// Tests > TransmitFilePathOptionPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTransmitFilePathOptionPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath, "--TransmitFile=" + TerminalFilePath, "--KeepOpenOnError"})))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull   (main.StartArgs.WorkspaceSettings);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettings.SettingsFilePath);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.RequestedTransmitFilePath);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);
				Assert.IsTrue (main.StartArgs.PerformOperationOnRequestedTerminal);

				Assert.IsFalse(main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > TransmitFilePathOptionInWorkspacePrepare
		//------------------------------------------------------------------------------------------
		// Tests > TransmitFilePathOptionInWorkspacePrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTransmitFilePathOptionInWorkspacePrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath, "--TransmitFile=" + TerminalFilePath, "--Terminal=2", "--KeepOpenOnError"})))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.WorkspaceSettings);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettings.SettingsFilePath);
				Assert.IsNotNull(main.StartArgs.TerminalSettings);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.RequestedTransmitFilePath);
				Assert.AreEqual (2, main.StartArgs.RequestedDynamicTerminalIndex);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);
				Assert.IsTrue (main.StartArgs.PerformOperationOnRequestedTerminal);

				Assert.IsFalse(main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > EmptyOptionPrepare
		//------------------------------------------------------------------------------------------
		// Tests > EmptyOptionPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyOptionPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Empty" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNull(main.StartArgs.WorkspaceSettings);
				Assert.IsNull(main.StartArgs.TerminalSettings);

				Assert.IsFalse(main.StartArgs.ShowNewTerminalDialog);

				Assert.IsTrue (main.StartArgs.KeepOpen);
				Assert.IsTrue (main.StartArgs.KeepOpenOnError);
				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > TileOptionsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > TileOptionsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTileOptionsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--TileHorizontal" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsTrue (main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--TileVertical" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsTrue (main.StartArgs.TileVertical);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--TileHorizontal", "--TileVertical" })))
			{
				PrepareMainAndVerifyResult(main, Main.Result.CommandLineError);

				Assert.IsFalse(main.StartArgs.TileHorizontal);
				Assert.IsFalse(main.StartArgs.TileVertical);
			}
		}

		#endregion

		#region Tests > InvalidPrepare
		//------------------------------------------------------------------------------------------
		// Tests > InvalidPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestInvalidPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--Blablabla" })))
			{
				PrepareMainAndVerifyResult(main, Main.Result.CommandLineError);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "+r" })))
			{
				PrepareMainAndVerifyResult(main, Main.Result.CommandLineError);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "-+Recent" })))
			{
				PrepareMainAndVerifyResult(main, Main.Result.CommandLineError);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "+-Recent" })))
			{
				PrepareMainAndVerifyResult(main, Main.Result.CommandLineError);
			}
		}

		#endregion

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private static void PrepareMainAndVerifyResult(Model.Main main)
		{
			PrepareMainAndVerifyResult(main, Main.Result.Success);
		}

		private static void PrepareMainAndVerifyResult(Model.Main main, Model.Main.Result expectedMainResult)
		{
			Model.Main.Result mainResult = main.PrepareStart();
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
