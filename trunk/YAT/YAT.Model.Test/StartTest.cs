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
// YAT 2.0 Gamma 3 Development Version 1.99.53
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

using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY.IO;
using MKY.Settings;

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

		// Current:

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string TerminalFilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM1_Closed_Default];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string Terminal1FilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM1_Closed_Default];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string Terminal2FilePath = SettingsFilesProvider.FilePaths_Current.TerminalFilePaths[TerminalSettingsTestCase.T_00_COM2_Closed_Default];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string WorkspaceFilePath = SettingsFilesProvider.FilePaths_Current.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias];

		// Empty:

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string EmptyTerminalFilePath = SettingsFilesProvider.FilePaths_Empty.TerminalFilePaths[TerminalSettingsTestCase.T_Empty];

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string EmptyWorkspaceFilePath = SettingsFilesProvider.FilePaths_Empty.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_Empty];

		#endregion

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;

			// Ensure that recent files are contained:
			ApplicationSettings.LocalUserSettings.RecentFiles = new Settings.RecentFileSettings();
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Add(TerminalFilePath);
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Add(WorkspaceFilePath);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
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

		#region Tests > EmptyPrepare
		//------------------------------------------------------------------------------------------
		// Tests > EmptyPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyPrepare()
		{
			using (Main main = new Main())
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.True);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.AreEqual(Indices.DefaultDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.That(main.StartArgs.RequestedTransmitFilePath, Is.Null.Or.Empty);

				Assert.That(main.StartArgs.KeepOpen,        Is.True);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			using (Main main = new Main(TerminalFilePath))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
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
			using (Main main = new Main(WorkspaceFilePath))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Not.Null);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Not.Null);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}
		}

		#endregion

		#region Tests > EmptyTerminalFilePathPrepare
		//------------------------------------------------------------------------------------------
		// Tests > EmptyTerminalFilePathPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyTerminalFilePathPrepare()
		{
			using (Main main = new Main(EmptyTerminalFilePath))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { EmptyTerminalFilePath })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}
		}

		#endregion

		#region Tests > EmptyWorkspaceFilePathPrepare
		//------------------------------------------------------------------------------------------
		// Tests > EmptyWorkspaceFilePathPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEmptyWorkspaceFilePathPrepare()
		{
			using (Main main = new Main(EmptyWorkspaceFilePath))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { EmptyWorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
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
			// Workspace only:
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Not.Null);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			// Terminal only:
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (Terminal1FilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			// Workspace + Terminal = Terminal (the last argument is used):
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + WorkspaceFilePath, "--Open=" + Terminal1FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (Terminal1FilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			// Terminal + Workspace = Workspace (the last argument is used):
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath, "--Open=" + WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Not.Null);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			// Terminal1 + Terminal2 = Terminal2 (the last argument is used):
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath, "--Open=" + Terminal2FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (Terminal2FilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
			}

			// Invalid file:
			string invalidFilePath = "MyFile.txt";
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + invalidFilePath })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Null);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Recent" })))
			{
				PrepareMainAndVerifyResult(main);

				bool hasRecent = ((main.StartArgs.TerminalSettingsHandler != null) || ((main.StartArgs.WorkspaceSettingsHandler != null)));
				Assert.That(hasRecent, Is.True);
				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--New" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--New", "--TerminalType=Binary", "--SerialPort=5", "--DataBits=7", "--Parity=E", "--FlowControl=Software" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);

				Assert.AreEqual(Domain.TerminalType.Binary, main.StartArgs.TerminalSettingsHandler.Settings.TerminalType);
				Assert.AreEqual(Domain.IOType.SerialPort,   main.StartArgs.TerminalSettingsHandler.Settings.IOType);

				Assert.AreEqual(5, main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.PortId); // COM5

				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialCommunicationSettings.BaudRateDefault,   main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.BaudRate);
				Assert.AreEqual(MKY.IO.Ports.DataBits.Seven,                                            main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.DataBits);
				Assert.AreEqual(System.IO.Ports.Parity.Even,                                            main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.Parity);
				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialCommunicationSettings.StopBitsDefault,   main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.StopBits);
				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialFlowControl.Software,                    main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.FlowControl);

				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorDefault.Enabled,  main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AliveMonitor.Enabled);
				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorDefault.Interval, main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AliveMonitor.Interval);

				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault.Enabled,  main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AutoReopen.Enabled);
				Assert.AreEqual(MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault.Interval, main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AutoReopen.Interval);

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalIsStarted, Is.False);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.LogIsOn, Is.False);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.AreEqual(Indices.DefaultDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.That(main.StartArgs.RequestedTransmitFilePath, Is.Null.Or.Empty);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);

				Assert.That(main.StartArgs.KeepOpen,        Is.True);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--New", "--IOType=TCPAutoSocket", "--RemotePort=12345", "--LocalPort=56789", "--OpenTerminal", "--LogOn" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);

				Assert.AreEqual(Domain.TerminalType.Text,    main.StartArgs.TerminalSettingsHandler.Settings.TerminalType);
				Assert.AreEqual(Domain.IOType.TcpAutoSocket, main.StartArgs.TerminalSettingsHandler.Settings.IOType);

				Assert.AreEqual((MKY.Net.IPHostEx)MKY.Net.IPHost.Localhost, (MKY.Net.IPHostEx)main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.RemoteHost);
				Assert.AreEqual(12345, main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.RemotePort);

				Assert.AreEqual((MKY.Net.IPNetworkInterfaceEx)MKY.Net.IPNetworkInterface.Any, (MKY.Net.IPNetworkInterfaceEx)main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.LocalInterface);
				Assert.AreEqual(56789, main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.LocalPort);

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalIsStarted, Is.True);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.LogIsOn, Is.True);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.AreEqual(Indices.DefaultDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.That(main.StartArgs.RequestedTransmitFilePath, Is.Null.Or.Empty);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);

				Assert.That(main.StartArgs.KeepOpen,        Is.True);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--New", "--IOType=USBSerHID", "--VendorID=1234", "--ProductID=ABCD", "--NoUsbAutoOpen" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);

				Assert.AreEqual(Domain.TerminalType.Text,   main.StartArgs.TerminalSettingsHandler.Settings.TerminalType);
				Assert.AreEqual(Domain.IOType.UsbSerialHid, main.StartArgs.TerminalSettingsHandler.Settings.IOType);

				Assert.AreEqual("1234", main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.DeviceInfo.VendorIdString);
				Assert.AreEqual("ABCD", main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.DeviceInfo.ProductIdString);
				Assert.AreEqual(MKY.IO.Serial.Usb.SerialHidFlowControl.None, main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.FlowControl);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.AutoOpen, Is.False);

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalIsStarted, Is.False);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.LogIsOn, Is.False);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.AreEqual(Indices.DefaultDynamicIndex, main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.That(main.StartArgs.RequestedTransmitFilePath, Is.Null.Or.Empty);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);

				Assert.That(main.StartArgs.KeepOpen,        Is.True);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath, "--BaudRate=19200" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);

				Assert.AreEqual(19200, main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.BaudRate);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);

				Assert.That(main.StartArgs.KeepOpen,        Is.True);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath, "--DynamicTerminalIndex=2", "--DataBits=7" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(                                  main.StartArgs.WorkspaceSettingsHandler, Is.Not.Null);
				Assert.AreEqual (WorkspaceFilePath,           main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath);
				Assert.AreEqual (2,                           main.StartArgs.RequestedDynamicTerminalIndex);
				Assert.That(                                  main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (MKY.IO.Ports.DataBits.Seven, main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.DataBits);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);

				Assert.That(main.StartArgs.KeepOpen,        Is.True);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
			}
		}

		#endregion

		#region Tests > TransmitTextOptionPrepare
		//------------------------------------------------------------------------------------------
		// Tests > TransmitTextOptionPrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTransmitTextOptionPrepare()
		{
			string text = @"Send something\!(Delay)Send delayed";

			using (Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath, "--TransmitText=" + text, "--KeepOpenOnError"})))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);
				Assert.AreEqual (text, main.StartArgs.RequestedTransmitText);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.True);

				Assert.That(main.StartArgs.KeepOpen,        Is.False);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
			}
		}

		#endregion

		#region Tests > TransmitTextOptionInWorkspacePrepare
		//------------------------------------------------------------------------------------------
		// Tests > TransmitTextOptionInWorkspacePrepare
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTransmitTextOptionInWorkspacePrepare()
		{
			string text = @"Send something\!(Delay)Send delayed";

			using (Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath, "--TransmitText=" + text, "--DynamicTerminalIndex=2", "--KeepOpenOnError"})))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Not.Null);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (text, main.StartArgs.RequestedTransmitText);
				Assert.AreEqual (2, main.StartArgs.RequestedDynamicTerminalIndex);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.True);

				Assert.That(main.StartArgs.KeepOpen,        Is.False);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			string filePath = Temp.MakeTempFilePath(GetType());
			File.Create(filePath); // File must exist!

			using (Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath, "--TransmitFile=" + filePath, "--KeepOpenOnError"})))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (TerminalFilePath, main.StartArgs.TerminalSettingsHandler.SettingsFilePath);
				Assert.AreEqual (filePath, main.StartArgs.RequestedTransmitFilePath);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.True);

				Assert.That(main.StartArgs.KeepOpen,        Is.False);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			string filePath = Temp.MakeTempFilePath(GetType());
			File.Create(filePath); // File must exist!

			using (Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath, "--TransmitFile=" + filePath, "--DynamicTerminalIndex=2", "--KeepOpenOnError"})))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Not.Null);
				Assert.AreEqual (WorkspaceFilePath, main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Not.Null);
				Assert.AreEqual (filePath, main.StartArgs.RequestedTransmitFilePath);
				Assert.AreEqual (2, main.StartArgs.RequestedDynamicTerminalIndex);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);
				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.True);

				Assert.That(main.StartArgs.KeepOpen,        Is.False);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Empty" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler, Is.Null);

				Assert.That(main.StartArgs.ShowNewTerminalDialog, Is.False);

				Assert.That(main.StartArgs.KeepOpen,        Is.True);
				Assert.That(main.StartArgs.KeepOpenOnError, Is.True);
				Assert.That(main.StartArgs.TileHorizontal,  Is.False);
				Assert.That(main.StartArgs.TileVertical,    Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--TileHorizontal" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.TileHorizontal, Is.True);
				Assert.That(main.StartArgs.TileVertical,   Is.False);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { "--TileVertical" })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.TileHorizontal, Is.False);
				Assert.That(main.StartArgs.TileVertical,   Is.True);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { "--TileHorizontal", "--TileVertical" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

				Assert.That(main.StartArgs.TileHorizontal, Is.False);
				Assert.That(main.StartArgs.TileVertical,   Is.False);
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
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Blablabla" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { "+r" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { "-+Recent" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { "+-Recent" })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);
			}
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private static void PrepareMainAndVerifyResult(Main main)
		{
			PrepareMainAndVerifyResult(main, MainResult.Success);
		}

		private static void PrepareMainAndVerifyResult(Main main, MainResult expectedMainResult)
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
