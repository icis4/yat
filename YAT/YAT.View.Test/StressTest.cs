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
// YAT Version 2.3.90 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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

using MKY.IO;
using MKY.Settings;
using MKY.Text;

using NUnit;
using NUnit.Framework;

using YAT.Application.Utilities;
using YAT.Domain;
using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Settings.Application;
using YAT.Settings.Model;

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
			// Temporary in-memory application settings seem useless for this YAT.Controller based
			// test, as YAT.Controller will retrieve the application settings, that's its job...
			// However, this test creates terminal settings *before* calling Main.Run(...), and
			// some of the settings (e.g. LogSettings.RootPath) rely on the application settings
			// (e.g. ApplicationSettings.LocalUserSettings.Paths.LogFiles). Thus, without creating
			// the settings here, an InvalidOperationException would be thrown when instantiating
			// the terminal settings, as access to the settings is invalid before they got created.

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
		/// Starts an instance of YAT with view and verifies that contents are properly displayed.
		/// </summary>
		[Test]
		[StressCategory]
		[MinuteDurationCategory(1)]
		public virtual void TestTransmissionDisplay()
		{
			// Preparation:

			string workspaceSettingsFilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionHelper.WorkspaceExtension);
			string terminalSettings1FilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionHelper.TerminalExtension);
			string terminalSettings2FilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ExtensionHelper.TerminalExtension);

			var workspaceSettingsHandler = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			var terminalSettingsHandler1 = new DocumentSettingsHandler<TerminalSettingsRoot>();
			var terminalSettingsHandler2 = new DocumentSettingsHandler<TerminalSettingsRoot>();

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

			var terminalSettings1Item = new TerminalSettingsItem();
			var terminalSettings2Item = new TerminalSettingsItem();

			terminalSettings1Item.FilePath = terminalSettingsHandler1.SettingsFilePath;
			terminalSettings1Item.FixedId = 1;
			terminalSettings2Item.FilePath = terminalSettingsHandler2.SettingsFilePath;
			terminalSettings2Item.FixedId = 2;

			workspaceSettingsHandler.Settings.TerminalSettings.Add(terminalSettings1Item);
			workspaceSettingsHandler.Settings.TerminalSettings.Add(terminalSettings2Item);

			workspaceSettingsHandler.Settings.Workspace.Layout = WorkspaceLayout.TileVertical;

			workspaceSettingsHandler.Save();
			Trace.WriteLine("Workspace file created:");
			Trace.Indent();
			Trace.WriteLine(workspaceSettingsFilePath);
			Trace.Unindent();

			string transmitFilePath = Temp.MakeTempFilePath(GetType(), Guid.NewGuid().ToString(), ".txt");
			using (var transmitFile = new StreamWriter(transmitFilePath, false, EncodingEx.EnvironmentRecommendedUTF8Encoding))
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
						"ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz"
					};

				for (int i = 0; i < 500; i++)
				{
					foreach (string content in contents)
						transmitFile.WriteLine(content);
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
				@"-di=0", // DynamicId
				@"-ke"    // KeepOpenOnError
			};

			using (var m = new Controller.Main(args))
			{
				var result = m.Run(false, true, ApplicationSettingsFileAccess.None, false); // <= see YAT.Controller.Test.TestFixtureSetUp() for background why without welcome screen.
				Assert.That(result, Is.EqualTo(Controller.MainResult.Success));
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
