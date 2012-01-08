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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NUnit;
using NUnit.Framework;

using MKY;
using MKY.Collections.Generic;

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

		private readonly string TerminalFilePath  = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM1_Closed_Default];
		private readonly string Terminal1FilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM1_Closed_Default];
		private readonly string Terminal2FilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM2_Closed_Default];
		private readonly string WorkspaceFilePath = SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias];

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool autoSaveWorkspaceToRestore;
		private Model.Settings.RecentFileSettings recentsToRestore;

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

			// Ensure that recents contains files.
			recentsToRestore = ApplicationSettings.LocalUser.RecentFiles;
			ApplicationSettings.LocalUser.RecentFiles = new Model.Settings.RecentFileSettings();
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.Add(TerminalFilePath);
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.Add(WorkspaceFilePath);
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
			ApplicationSettings.LocalUser.RecentFiles               = this.recentsToRestore;
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

				Assert.IsFalse(main.StartArgs.PerformActionOnRequestedTerminal);
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
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

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

		#region Tests > SerialPortOptionsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > SerialPortOptionsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSerialPortOptionsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--TerminalType=Binary", "--SerialPort=5", "--DataBits=7", "--Parity=E", "--FlowControl=Software" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.IsNotNull(main.StartArgs.TerminalSettings);

				Assert.AreEqual(Domain.TerminalType.Binary, main.StartArgs.TerminalSettings.Settings.TerminalType);
				Assert.AreEqual(Domain.IOType.SerialPort,   main.StartArgs.TerminalSettings.Settings.IOType);

				Assert.AreEqual(5, (int)main.StartArgs.TerminalSettings.Settings.IO.SerialPort.PortId); // COM5

				Assert.AreEqual(MKY.IO.Serial.SerialCommunicationSettings.BaudRateDefault,   main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.BaudRate);
				Assert.AreEqual(MKY.IO.Ports.DataBits.Seven,                                 main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.DataBits);
				Assert.AreEqual(System.IO.Ports.Parity.Even,                                 main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.Parity);
				Assert.AreEqual(MKY.IO.Serial.SerialCommunicationSettings.StopBitsDefault,   main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.StopBits);
				Assert.AreEqual(MKY.IO.Serial.SerialFlowControl.Software,                     main.StartArgs.TerminalSettings.Settings.IO.SerialPort.Communication.FlowControl);

				Assert.AreEqual(MKY.IO.Serial.SerialPortSettings.AutoReopenDefault.Enabled,  main.StartArgs.TerminalSettings.Settings.IO.SerialPort.AutoReopen.Enabled);
				Assert.AreEqual(MKY.IO.Serial.SerialPortSettings.AutoReopenDefault.Interval, main.StartArgs.TerminalSettings.Settings.IO.SerialPort.AutoReopen.Interval);

				Assert.IsFalse(main.StartArgs.TerminalSettings.Settings.TerminalIsStarted);
				Assert.IsFalse(main.StartArgs.TerminalSettings.Settings.LogIsStarted);

				Assert.IsFalse(main.StartArgs.PerformActionOnRequestedTerminal);
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

		#region Tests > TcpAutoSocketOptionsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > TcpAutoSocketOptionsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTcpAutoSocketOptionsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--IOType=TCPAutoSocket", "--RemotePort=12345", "--LocalPort=56789", "--OpenTerminal", "--BeginLog" })))
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

				Assert.IsFalse(main.StartArgs.PerformActionOnRequestedTerminal);
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

		#region Tests > TcpUsbSerialHidOptionsPrepare
		//------------------------------------------------------------------------------------------
		// Tests > TcpUsbSerialHidOptionsPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestUsbSerialHidOptionsPrepare()
		{
			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "--IOType=USBSerHID", "--VendorID=1234", "--ProductID=ABCD", "--NoUsbAutoOpen" })))
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

				Assert.IsFalse(main.StartArgs.PerformActionOnRequestedTerminal);
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
				Assert.IsFalse(main.StartArgs.PerformActionOnRequestedTerminal);

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
				Assert.IsFalse(main.StartArgs.PerformActionOnRequestedTerminal);

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
				Assert.IsTrue(main.StartArgs.PerformActionOnRequestedTerminal);

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
				Assert.IsTrue (main.StartArgs.PerformActionOnRequestedTerminal);

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
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

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
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "+r" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "-+Recent" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
			}

			using (Model.Main main = new Main(new CommandLineArgs(new string[] { "+-Recent" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
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
			PrepareMainAndVerifyResult(main, MainResult.Success);
		}

		private static void PrepareMainAndVerifyResult(Model.Main main, MainResult expectedMainResult)
		{
			MainResult mainResult = main.PrepareStart();
			Assert.AreEqual(expectedMainResult, mainResult);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
