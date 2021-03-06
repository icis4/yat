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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Debug SVN hooks without having to change the SVN hook's command line in the SVN settings:
////#define DEBUGGER_BREAK_HOOK

#endif

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

#endregion

namespace YATInfra.SVNHooks.PostUpdate
{
	public static class Program
	{
		/// <summary>
		/// The 'Post-Update' hook.
		/// </summary>
		/// <param name="args">
		/// Args are PATH DEPTH REVISION ERROR CWD RESULTPATH (from "Client Side Hook Scripts"
		/// at https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-dug-settings.html).
		/// </param>
		/// <returns>
		/// <c>0</c> on success; any other value on error.
		/// </returns>
		private static int Main(string[] args)
		{
		#if (DEBUGGER_BREAK_HOOK)
			if (!DebuggerHookHelper.Once)
			{
				DebuggerHookHelper.Once = true;
				MessageBox.Show(DebuggerHookHelper.Message, typeof(Program).FullName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		#endif

			try
			{
				var result = ValidateArgs(args);
				if (result != Result.Success) {
					return ((int)result);
				}

				return ((int)ProcessArgsAndRestoreTimeStamps(args));
			}
			catch (Exception ex)
			{
				var caption = "'Post-Update' Hook Unhandled Exception";
				var message = new StringBuilder();
				message.AppendLine("The 'Post-Update' hook has resulted in an unhandled exception!");
				message.AppendLine();
				message.AppendLine(ex.Message);
				MessageBox.Show(message.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return ((int)Result.UnhandledException);
			}
		}

		private static Result ValidateArgs(string[] args)
		{
			if ((args == null) || (args.Length != 6))
			{
				var caption = "'Post-Update' Hook Error";
				var message = new StringBuilder();
				message.AppendLine("The 'Post-Update' hook requires 6 arguments 'PATH DEPTH REVISION ERROR CWD RESULTPATH'!");
				if (args != null) {
					message.AppendLine();
					message.AppendLine("Args:");
					foreach (var arg in args) {
						message.AppendLine(arg);
					}
				}
				MessageBox.Show(message.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return (Result.CommandLineError);
			}

			return (Result.Success);
		}

		private static Result ProcessArgsAndRestoreTimeStamps(string[] args)
		{
			// Neither the start paths (args[0]) not working directory path(args[4]) is relevant,
			// they only contain information on where the user has started the update.

			var pathToResultPaths = args[5];
			var resultPaths = File.ReadAllLines(pathToResultPaths);
			if (resultPaths.Length < 1) {
				return (Result.Success); // Nothing to do.
			}

			// The result paths (args[5]) is what is needed. The file will contain directory as well
			// as file paths, e.g.:
			//
			// D:/Workspace/YAT/Trunk/YAT
			// D:/Workspace/YAT/Trunk/YAT/netrtfwriter/README.txt
			//
			// Attention: / instead of \ is used by SVN!

			foreach (var resultPath in resultPaths)
			{
				if (File.Exists(resultPath))
				{
					var result = RestoreTimeStampIfGiven(resultPath);
					if (result != Result.Success) {
						return (result);
					}
				}
			}

			return (Result.Success);
		}

		private static Result RestoreTimeStampIfGiven(string filePath)
		{
			filePath = Path.GetFullPath(filePath); // Remember: / instead of \ is used by SVN!

			var directoryPath = Path.GetDirectoryName(filePath);
			var timeStampFilePath = Path.Combine(directoryPath, TimeStampFileHelper.FileName);
			if (File.Exists(timeStampFilePath))
			{
				var timeStampLines = File.ReadAllLines(timeStampFilePath);
				foreach (var timeStampLine in timeStampLines)
				{
					if (!string.IsNullOrWhiteSpace(timeStampLine)) // Allow leading, separating and trailing empty lines.
					{
						DateTime timeStamp;
						string fileNamePattern;
						var result = TimeStampFileHelper.ValidateLine(timeStampFilePath, timeStampLine, out timeStamp, out fileNamePattern);
						if (result != Result.Success) {
							return (result);
						}

						var filePathsCovered = Directory.GetFiles(directoryPath, fileNamePattern);
						foreach (var filePathCovered in filePathsCovered)
						{
							if (filePathCovered == filePath)
							{
								File.SetCreationTime(filePath, timeStamp);   // "File.Set*Time": The value is expressed in local time.
								File.SetLastWriteTime(filePath, timeStamp);  // Corresponds to the value shown by e.g. the Windows Explorer.
								File.SetLastAccessTime(filePath, timeStamp); // This eases manually setting the time stamp in the file.
							}
						}
					}
				}
			}

			return (Result.Success);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
