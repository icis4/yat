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
// YAT 2.0 Gamma 3 Version 1.99.70
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

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using MKY.IO;
using MKY.Settings;

using NUnit;
using NUnit.Framework;

using YAT.Application.Utilities;
using YAT.Domain;
using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.View.Test
{
	/// <summary></summary>
	[TestFixture]
	public class StressTest
	{
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
			// Preparation:

			string workspaceSettingsFilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionHelper.WorkspaceFile);
			string terminalSettings1FilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionHelper.TerminalFile);
			string terminalSettings2FilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionHelper.TerminalFile);

			DocumentSettingsHandler<WorkspaceSettingsRoot> workspaceSettingsHandler = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			DocumentSettingsHandler<TerminalSettingsRoot>  terminalSettingsHandler1 = new DocumentSettingsHandler<TerminalSettingsRoot>();
			DocumentSettingsHandler<TerminalSettingsRoot>  terminalSettingsHandler2 = new DocumentSettingsHandler<TerminalSettingsRoot>();

			workspaceSettingsHandler.SettingsFilePath = workspaceSettingsFilePath;
			terminalSettingsHandler1.SettingsFilePath = terminalSettings1FilePath;
			terminalSettingsHandler2.SettingsFilePath = terminalSettings2FilePath;

			terminalSettingsHandler1.Settings.IOType = IOType.TcpAutoSocket;
			terminalSettingsHandler2.Settings.IOType = IOType.TcpAutoSocket;

			terminalSettingsHandler1.Settings.Status.ShowConnectTime = true;
			terminalSettingsHandler1.Settings.Status.ShowCountAndRate = true;
			terminalSettingsHandler2.Settings.Status.ShowConnectTime = true;
			terminalSettingsHandler2.Settings.Status.ShowCountAndRate = true;

			terminalSettingsHandler1.Settings.TerminalIsStarted = true;
			terminalSettingsHandler2.Settings.TerminalIsStarted = true;

			terminalSettingsHandler1.Save();
			terminalSettingsHandler2.Save();
			Trace.WriteLine("Terminal files created:");
			Trace.Indent();
			Trace.WriteLine(terminalSettings1FilePath);
			Trace.WriteLine(terminalSettings2FilePath);
			Trace.Unindent();

			TerminalSettingsItem terminalSettings1Item = new TerminalSettingsItem();
			TerminalSettingsItem terminalSettings2Item = new TerminalSettingsItem();

			terminalSettings1Item.FilePath = terminalSettingsHandler1.SettingsFilePath;
			terminalSettings1Item.FixedIndex = 1;
			terminalSettings2Item.FilePath = terminalSettingsHandler2.SettingsFilePath;
			terminalSettings2Item.FixedIndex = 2;

			workspaceSettingsHandler.Settings.TerminalSettings.Add(terminalSettings1Item);
			workspaceSettingsHandler.Settings.TerminalSettings.Add(terminalSettings2Item);

			workspaceSettingsHandler.Settings.Workspace.Layout = WorkspaceLayout.TileVertical;

			workspaceSettingsHandler.Save();
			Trace.WriteLine("Workspace file created:");
			Trace.Indent();
			Trace.WriteLine(workspaceSettingsFilePath);
			Trace.Unindent();

			string transmitFilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ".txt");
			using (var transmitFile = new StreamWriter(transmitFilePath, false, Encoding.UTF8))
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

			// Run using absolute path:
			RunTransmission(workspaceSettingsFilePath, transmitFilePath);

			// Run using relative path:
			string currentDirectoryToRestore = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(transmitFilePath);
				RunTransmission(workspaceSettingsFilePath, Path.GetFileName(transmitFilePath));
			}
			finally
			{
				Environment.CurrentDirectory = currentDirectoryToRestore;
			}
		}

		private static void RunTransmission(string workspaceSettingsFilePath, string transmitFilePath)
		{
			string[] args = new string[]
			{
				@"""" + workspaceSettingsFilePath + @"""",
				@"-tf=""" + transmitFilePath + @"""",
				@"-di=0",
				@"-ke"
			};

			using (Controller.Main main = new Controller.Main(args))
			{
				Controller.MainResult mainResult = main.Run();
				Assert.That(mainResult, Is.EqualTo(Controller.MainResult.Success));
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