﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1 Version 1.99.32
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

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using MKY.IO;
using MKY.Settings;

using NUnit;
using NUnit.Framework;

using YAT.Controller;
using YAT.Domain;
using YAT.Model.Types;
using YAT.Model.Settings;
using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.Gui.Test
{
	/// <summary></summary>
	[TestFixture]
	public class StressTest
	{
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

			Temp.CleanTempPath(GetType());
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > TransmissionDisplay
		//------------------------------------------------------------------------------------------
		// Tests > TransmissionDisplay
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts an instance of YAT including GUI and verifies that contents are properly displayed.
		/// </summary>
		[Test]
		[StressCategory]
		[MinuteDurationCategory(1)]
		public virtual void TestTransmissionDisplay()
		{
			string workspaceSettingsFilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionSettings.WorkspaceFile);
			string terminalSettings1FilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionSettings.TerminalFile);
			string terminalSettings2FilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionSettings.TerminalFile);

			DocumentSettingsHandler<WorkspaceSettingsRoot> workspaceSettings = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			DocumentSettingsHandler<TerminalSettingsRoot> terminalSettings1 = new DocumentSettingsHandler<TerminalSettingsRoot>();
			DocumentSettingsHandler<TerminalSettingsRoot> terminalSettings2 = new DocumentSettingsHandler<TerminalSettingsRoot>();

			workspaceSettings.SettingsFilePath = workspaceSettingsFilePath;
			terminalSettings1.SettingsFilePath = terminalSettings1FilePath;
			terminalSettings2.SettingsFilePath = terminalSettings2FilePath;

			terminalSettings1.Settings.IOType = IOType.TcpAutoSocket;
			terminalSettings2.Settings.IOType = IOType.TcpAutoSocket;

			terminalSettings1.Settings.Status.ShowConnectTime = true;
			terminalSettings1.Settings.Status.ShowCountAndRate = true;
			terminalSettings2.Settings.Status.ShowConnectTime = true;
			terminalSettings2.Settings.Status.ShowCountAndRate = true;

			terminalSettings1.Settings.TerminalIsStarted = true;
			terminalSettings2.Settings.TerminalIsStarted = true;

			terminalSettings1.Save();
			terminalSettings2.Save();
			Trace.WriteLine("Terminal files created:");
			Trace.Indent();
			Trace.WriteLine(terminalSettings1FilePath);
			Trace.WriteLine(terminalSettings2FilePath);
			Trace.Unindent();

			TerminalSettingsItem terminalSettings1Item = new TerminalSettingsItem();
			TerminalSettingsItem terminalSettings2Item = new TerminalSettingsItem();

			terminalSettings1Item.FilePath = terminalSettings1.SettingsFilePath;
			terminalSettings1Item.FixedIndex = 1;
			terminalSettings2Item.FilePath = terminalSettings2.SettingsFilePath;
			terminalSettings2Item.FixedIndex = 2;

			workspaceSettings.Settings.TerminalSettings.Add(terminalSettings1Item);
			workspaceSettings.Settings.TerminalSettings.Add(terminalSettings2Item);

			workspaceSettings.Settings.Workspace.Layout = WorkspaceLayout.TileVertical;

			workspaceSettings.Save();
			Trace.WriteLine("Workspace file created:");
			Trace.Indent();
			Trace.WriteLine(workspaceSettingsFilePath);
			Trace.Unindent();

			string transmitFilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ".txt");
			using (StreamWriter transmitFile = new StreamWriter(transmitFilePath, false, Encoding.UTF8))
			{
				transmitFile.WriteLine("");
				transmitFile.WriteLine("Transmission test file generated by " + GetType().FullName + ".TestTransmissionDisplay()");
				transmitFile.WriteLine("");

				string[] contents = new string[]
					{
						"A",
						"ABC",
						"ABCDEFGHIJKLMNOPQRSTUVWXYZ",
						"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
						"ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz",
						"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
						"ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz",
					};

				for (int i = 0; i < 500; i++)
				{
					foreach (string s in contents)
						transmitFile.WriteLine(s);
				}
			}
			Trace.WriteLine("Transmit file created:");
			Trace.Indent();
			Trace.WriteLine(transmitFilePath);
			Trace.Unindent();

			string[] args = new string[]
			{
				@"""" + workspaceSettingsFilePath + @"""",
				@"-tf=""" + transmitFilePath + @"""",
				@"-t=0",
				@"-ke"
			};

			using (Controller.Main main = new Main(args))
			{
				Controller.Main.Result mainResult = main.Run();
				Assert.AreEqual(Controller.Main.Result.Success, mainResult);
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================