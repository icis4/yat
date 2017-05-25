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
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
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
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
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
				Assert.That(main.StartArgs.TerminalSettingsHandler,  Is.Null);
				Assert.That(main.StartArgs.ShowNewTerminalDialog,    Is.True);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.That(main.StartArgs.RequestedDynamicTerminalIndex,       Is.EqualTo(Indices.DefaultDynamicIndex));
				Assert.That(main.StartArgs.RequestedTransmitFilePath,           Is.Null.Or.Empty);

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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(TerminalFilePath));
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                    Is.False);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { TerminalFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(TerminalFilePath));
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                    Is.False);
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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath, Is.EqualTo(WorkspaceFilePath));
				Assert.That(main.StartArgs.TerminalSettingsHandler,                   Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                     Is.False);
			}

			using (Main main = new Main(new CommandLineArgs(new string[] { WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath, Is.EqualTo(WorkspaceFilePath));
				Assert.That(main.StartArgs.TerminalSettingsHandler,                   Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                     Is.False);
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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath, Is.EqualTo(WorkspaceFilePath));
				Assert.That(main.StartArgs.TerminalSettingsHandler,                   Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                     Is.False);
			}

			// Terminal only:
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(Terminal1FilePath));
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                    Is.False);
			}

			// Workspace + Terminal = Terminal (the last argument is used):
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + WorkspaceFilePath, "--Open=" + Terminal1FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(Terminal1FilePath));
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                    Is.False);
			}

			// Terminal + Workspace = Workspace (the last argument is used):
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath, "--Open=" + WorkspaceFilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath, Is.EqualTo(WorkspaceFilePath));
				Assert.That(main.StartArgs.TerminalSettingsHandler,                   Is.Not.Null); // By default the last terminal in the workspace.
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                     Is.False);
			}

			// Terminal1 + Terminal2 = Terminal2 (the last argument is used):
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + Terminal1FilePath, "--Open=" + Terminal2FilePath })))
			{
				PrepareMainAndVerifyResult(main);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(Terminal2FilePath));
				Assert.That(main.StartArgs.ShowNewTerminalDialog,                    Is.False);
			}

			// Invalid file:
			string invalidFilePath = "MyFile.txt";
			using (Main main = new Main(new CommandLineArgs(new string[] { "--Open=" + invalidFilePath })))
			{
				PrepareMainAndVerifyResult(main, MainResult.CommandLineError);

				Assert.That(main.StartArgs.WorkspaceSettingsHandler, Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,  Is.Null);
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

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalType, Is.EqualTo(Domain.TerminalType.Binary));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IOType,       Is.EqualTo(Domain.IOType.SerialPort));

				Assert.That((int)main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.PortId, Is.EqualTo(5)); // COM5

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.BaudRate,    Is.EqualTo(MKY.IO.Serial.SerialPort.SerialCommunicationSettings.BaudRateDefault));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.DataBits,    Is.EqualTo(MKY.IO.Ports.DataBits.Seven));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.Parity,      Is.EqualTo(System.IO.Ports.Parity.Even));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.StopBits,    Is.EqualTo(MKY.IO.Serial.SerialPort.SerialCommunicationSettings.StopBitsDefault));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.FlowControl, Is.EqualTo(MKY.IO.Serial.SerialPort.SerialFlowControl.Software));

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AliveMonitor.Enabled,  Is.EqualTo(MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorDefault.Enabled));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AliveMonitor.Interval, Is.EqualTo(MKY.IO.Serial.SerialPort.SerialPortSettings.AliveMonitorDefault.Interval));

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AutoReopen.Enabled,  Is.EqualTo(MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault.Enabled));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.AutoReopen.Interval, Is.EqualTo(MKY.IO.Serial.SerialPort.SerialPortSettings.AutoReopenDefault.Interval));

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalIsStarted, Is.False);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.LogIsOn,           Is.False);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.That(main.StartArgs.RequestedDynamicTerminalIndex,       Is.EqualTo(Indices.DefaultDynamicIndex));
				Assert.That(main.StartArgs.RequestedTransmitFilePath,           Is.Null.Or.Empty);

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

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalType, Is.EqualTo(Domain.TerminalType.Text));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IOType,       Is.EqualTo(Domain.IOType.TcpAutoSocket));

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.RemoteHost, Is.EqualTo((MKY.Net.IPHostEx)MKY.Net.IPHost.Localhost));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.RemotePort, Is.EqualTo(12345));

				Assert.That((MKY.Net.IPNetworkInterfaceEx)main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.LocalInterface, Is.EqualTo((MKY.Net.IPNetworkInterfaceEx)MKY.Net.IPNetworkInterface.Any));
				Assert.That(                              main.StartArgs.TerminalSettingsHandler.Settings.IO.Socket.LocalPort,      Is.EqualTo(56789));

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalIsStarted, Is.True);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.LogIsOn,           Is.True);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.That(main.StartArgs.RequestedDynamicTerminalIndex,       Is.EqualTo(Indices.DefaultDynamicIndex));
				Assert.That(main.StartArgs.RequestedTransmitFilePath,           Is.Null.Or.Empty);

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

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalType, Is.EqualTo(Domain.TerminalType.Text));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IOType,       Is.EqualTo(Domain.IOType.UsbSerialHid));

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.DeviceInfo.VendorIdString,  Is.EqualTo("1234"));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.DeviceInfo.ProductIdString, Is.EqualTo("ABCD"));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.FlowControl,                Is.EqualTo(MKY.IO.Serial.Usb.SerialHidFlowControl.None));
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.UsbSerialHidDevice.AutoOpen,                   Is.False);

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.TerminalIsStarted, Is.False);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.LogIsOn,           Is.False);

				Assert.That(main.StartArgs.PerformOperationOnRequestedTerminal, Is.False);
				Assert.That(main.StartArgs.RequestedDynamicTerminalIndex,       Is.EqualTo(Indices.DefaultDynamicIndex));
				Assert.That(main.StartArgs.RequestedTransmitFilePath,           Is.Null.Or.Empty);

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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(TerminalFilePath));

				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.BaudRate, Is.EqualTo(19200));

				Assert.That(main.StartArgs.ShowNewTerminalDialog,               Is.False);
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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath, Is.EqualTo(WorkspaceFilePath));
				Assert.That(main.StartArgs.RequestedDynamicTerminalIndex,             Is.EqualTo(2));
				Assert.That(main.StartArgs.TerminalSettingsHandler,                   Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.Settings.IO.SerialPort.Communication.DataBits, Is.EqualTo(MKY.IO.Ports.DataBits.Seven));

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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(TerminalFilePath));
				Assert.That(main.StartArgs.RequestedTransmitText,                    Is.EqualTo(text));

				Assert.That(main.StartArgs.ShowNewTerminalDialog,               Is.False);
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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath, Is.EqualTo(WorkspaceFilePath));
				Assert.That(main.StartArgs.TerminalSettingsHandler,                   Is.Not.Null);
				Assert.That(main.StartArgs.RequestedTransmitText,                     Is.EqualTo(text));
				Assert.That(main.StartArgs.RequestedDynamicTerminalIndex,             Is.EqualTo(2));

				Assert.That(main.StartArgs.ShowNewTerminalDialog,               Is.False);
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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                 Is.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.TerminalSettingsHandler.SettingsFilePath, Is.EqualTo(TerminalFilePath));
				Assert.That(main.StartArgs.RequestedTransmitFilePath,                Is.EqualTo(filePath));

				Assert.That(main.StartArgs.ShowNewTerminalDialog,               Is.False);
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

				Assert.That(main.StartArgs.WorkspaceSettingsHandler,                  Is.Not.Null);
				Assert.That(main.StartArgs.WorkspaceSettingsHandler.SettingsFilePath, Is.EqualTo(WorkspaceFilePath));
				Assert.That(main.StartArgs.TerminalSettingsHandler,                   Is.Not.Null);
				Assert.That(main.StartArgs.RequestedTransmitFilePath,                 Is.EqualTo(filePath));
				Assert.That(main.StartArgs.RequestedDynamicTerminalIndex,             Is.EqualTo(2));

				Assert.That(main.StartArgs.ShowNewTerminalDialog,               Is.False);
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
			Assert.That(mainResult, Is.EqualTo(expectedMainResult));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
