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

using System.IO;

using MKY.IO;
using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.FileHandling
{
	/// <summary></summary>
	public class Utilities
	{
		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		internal static DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler()
		{
			return (new DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot>(Test.Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()));
		}

		#endregion

		#region Start
		//==========================================================================================
		// Start
		//==========================================================================================

		/// <summary></summary>
		internal static void StartAndCreateDefaultTerminal(out Main main, out Workspace workspace, out Terminal terminal)
		{
			main = new Main();
			main.Start();              // Creates empty workspace
			workspace = main.Workspace;
			workspace.CreateNewTerminal(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler());
			terminal = workspace.ActiveTerminal;
		}

		#endregion

		#region File Verification
		//==========================================================================================
		// File Verification
		//==========================================================================================

		/// <summary></summary>
		internal static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected)
		{
			VerifyFiles(                        prefix,           workspace,      workspaceFileExpected, new Terminal[] {  }, new bool[] { });
		}

		/// <summary></summary>
		internal static void VerifyFiles(               Workspace workspace, bool workspaceFileExpected, Terminal terminal, bool terminalFileExpected)
		{
			VerifyFiles(                            "",           workspace,      workspaceFileExpected,          terminal,      terminalFileExpected);
		}

		/// <summary></summary>
		internal static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, Terminal terminal, bool terminalFileExpected, bool terminalFileAutoExpected = true)
		{
			VerifyFiles(                        prefix,           workspace,      workspaceFileExpected,                           true,          terminal,      terminalFileExpected,      terminalFileAutoExpected);
		}

		/// <summary></summary>
		internal static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, bool workspaceFileAutoExpected, Terminal terminal, bool terminalFileExpected, bool terminalFileAutoExpected)
		{
			VerifyFiles(                        prefix,           workspace,      workspaceFileExpected,  workspaceFileAutoExpected, new Terminal[] { terminal }, new bool[] { terminalFileExpected }, new bool[] { terminalFileAutoExpected });
		}

		/// <summary></summary>
		internal static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, Terminal[] terminals, bool[] terminalFilesExpected)
		{
			VerifyFiles(                        prefix,           workspace,      workspaceFileExpected,                          true,             terminals,        terminalFilesExpected, new bool[] { true });
		}

		/// <summary></summary>
		internal static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, Terminal[] terminals, bool[] terminalFilesExpected, bool[] terminalFilesAutoExpected)
		{
			VerifyFiles(                        prefix,           workspace,      workspaceFileExpected,                           true,            terminals,        terminalFilesExpected, terminalFilesAutoExpected);
		}

		/// <summary></summary>
		internal static void VerifyFiles(string prefix, Workspace workspace, bool workspaceFileExpected, bool workspaceFileAutoExpected, Terminal[] terminals, bool[] terminalFilesExpected, bool[] terminalFilesAutoExpected)
		{
			// Verify workspace file:
			if (workspaceFileExpected)
			{
				Assert.That(workspace.SettingsFilePath, Is.Not.Null.And.Not.Empty, prefix + "Workspace settings file path is empty!");
				Assert.That(File.Exists(workspace.SettingsFilePath), Is.True, prefix + "Workspace file doesn't exist!");

				if (workspaceFileAutoExpected)
					Assert.That(workspace.SettingsRoot.AutoSaved, Is.True, prefix + "Workspace file not auto saved!");
				else
					Assert.That(workspace.SettingsRoot.AutoSaved, Is.False, prefix + "Workspace file must not be auto saved!");
			}
			else
			{
				Assert.That(File.Exists(workspace.SettingsFilePath), Is.False, prefix + "Workspace file exists unexpectantly!");
			}

			// Verify terminal file(s):
			for (int i = 0; i < terminals.Length; i++)
			{
				if (terminalFilesExpected[i])
				{
					Assert.That(terminals[i].SettingsFilePath, Is.Not.Null.And.Not.Empty, prefix + "Terminal settings file path is empty!");
					Assert.That(File.Exists(terminals[i].SettingsFilePath), Is.True, prefix + "Terminal file doesn't exist!");

					if (terminalFilesAutoExpected[i])
						Assert.That(terminals[i].SettingsRoot.AutoSaved, Is.True, prefix + "Terminal file not auto saved!");
					else
						Assert.That(terminals[i].SettingsRoot.AutoSaved, Is.False, prefix + "Terminal file must not be auto saved!");
				}
				else
				{
					Assert.That(File.Exists(terminals[i].SettingsFilePath), Is.False, prefix + "Terminal file exists unexpectantly!");
				}
			}

			// Verify application settings:
			if (workspaceFileExpected)
				Assert.That(PathEx.Equals(ApplicationSettings.LocalUserSettings.AutoWorkspace.FilePath, workspace.SettingsFilePath), prefix + "Workspace file path not set!");
			else
				//// Note that the application settings may still contain a former workspace file path.
				//// This is required to test certain use cases with normal and command line execution.
				//// Therefore, do not check the local user settings, instead, check that the workspace file path is reset.
				Assert.That(workspace.SettingsFilePath, Is.Null.Or.Empty, prefix + "Workspace file path not reset!");

			// Verify recent settings:
			if (workspaceFileExpected && (!workspaceFileAutoExpected))
				Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(workspace.SettingsFilePath), Is.True, prefix + "Workspace file path doesn't exist in recent files!");
			else
				Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(workspace.SettingsFilePath), Is.False, prefix + "Workspace file path must not be in recent files!");

			for (int i = 0; i < terminals.Length; i++)
			{
				if (terminalFilesExpected[i] && (!terminalFilesAutoExpected[i]))
					Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(terminals[i].SettingsFilePath), Is.True, prefix + "Terminal file path doesn't exist in recent files!");
				else
					Assert.That(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Contains(terminals[i].SettingsFilePath), Is.False, prefix + "Terminal file path must not be in recent files!");
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
