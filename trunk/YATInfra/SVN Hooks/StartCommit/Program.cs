//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/!-Doc.Developer/Template.cs $
// $Revision: 3643 $
// $Date: 2021-01-26 12:14:15 +0100 (Di., 26 Jan 2021) $
// $Author: maettu_this $
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

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace YATInfra.SVNHooks.StartCommit
{
	public static class Program
	{
		/// <summary>
		/// The 'Start-Commit' hook.
		/// </summary>
		/// <param name="args">
		/// Args are PATH MESSAGEFILE CWD (from "Client Side Hook Scripts"
		/// at https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-dug-settings.html).
		/// </param>
		/// <returns>
		/// <c>0</c> on success; any other value on error.
		/// </returns>
		private static int Main(string[] args)
		{
			try
			{
				var result = ValidateArgs(args);
				if (result != Result.Success) {
					return ((int)result);
				}

				return ((int)ProcessArgsAndUpdateTimeStamps(args));
			}
			catch (Exception ex)
			{
				var caption = "'Start-Commit' Hook Unhandled Exception";
				var message = new StringBuilder();
				message.AppendLine("The 'Start-Commit' hook has resulted in an unhandled exception!");
				message.AppendLine();
				message.AppendLine(ex.Message);
				MessageBox.Show(message.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return ((int)Result.UnhandledException);
			}
		}

		private static Result ValidateArgs(string[] args)
		{
			if ((args == null) || (args.Length != 3))
			{
				var caption = "'Start-Commit' Hook Error";
				var message = new StringBuilder();
				message.AppendLine("The 'Start-Commit' hook requires 3 arguments 'PATH MESSAGEFILE CWD'!");
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
		private static Result ProcessArgsAndUpdateTimeStamps(string[] args)
		{
			var pathToStartPaths = args[0];
			var startPaths = File.ReadAllLines(pathToStartPaths);
			if (startPaths.Length < 1) {
				return (Result.Success); // Nothing to do.
			}

			// The start paths (args[0]) is what is needed. The file will contain directory as well
			// as file paths, e.g.:
			//
			// D:/Workspace/YAT/Trunk/YAT/YAT.sln
			// D:/Workspace/YAT/Trunk/YAT/YAT
			//
			// Attention: / instead of \ is used by SVN!

			foreach (var startPath in startPaths)
			{
				if (File.Exists(startPath))
				{
					var directoryPath = Path.GetDirectoryName(startPath);
					var result = UpdateTimeStampsIfGiven(directoryPath);
					if (result != Result.Success) {
						return (result);
					}
				}
				else if (Directory.Exists(startPath))
				{
					var result = UpdateTimeStampsIfGiven(startPath); // The given start directory.
					if (result != Result.Success) {
						return (result);
					}

					var directoryPaths = Directory.GetDirectories(startPath, "*", SearchOption.AllDirectories);
					foreach (var directoryPath in directoryPaths) // The subdirectories of the given start directory.
					{
						result = UpdateTimeStampsIfGiven(directoryPath);
						if (result != Result.Success) {
							return (result);
						}
					}
				}
			}

			return (Result.Success);
		}

		private static Result UpdateTimeStampsIfGiven(string directoryPath)
		{
			directoryPath = Path.GetFullPath(directoryPath); // Remember: / instead of \ is used by SVN!

			var timeStampFilePath = Path.Combine(directoryPath, TimeStampFileHelper.FileName);
			if (File.Exists(timeStampFilePath))
			{
				var timeStampLinesHaveChanged = false;
				var timeStampLines = File.ReadAllLines(timeStampFilePath);
				for (int i = 0; i < timeStampLines.Length; i++)
				{
					if (!string.IsNullOrWhiteSpace(timeStampLines[i])) // Allow leading, separating and trailing empty lines.
					{
						DateTime timeStampOld;
						string fileNamePattern;
						var result = TimeStampFileHelper.ValidateLine(timeStampFilePath, timeStampLines[i], out timeStampOld, out fileNamePattern);
						if (result != Result.Success) {
							return (result);
						}

						var filePathsCovered = Directory.GetFiles(directoryPath, fileNamePattern);
						foreach (var filePathCovered in filePathsCovered)
						{
							if (timeStampFilePath == filePathCovered) // This is the case for "*" file name pattern.
							{
								continue;
							}

							var timeStampNew = File.GetLastWriteTime(filePathCovered); // "File.GetLastWriteTime": The value is expressed in local time.
							if (timeStampNew != timeStampOld)                          // Corresponds to the value shown by e.g. the Windows Explorer.
							{                                                          // This eases manually setting the time stamp in the file.
								var timeStampString = timeStampNew.ToString(TimeStampFileHelper.Format);
								timeStampLines[i] = string.Format("{0}|{1}", timeStampString, fileNamePattern);
								timeStampLinesHaveChanged = true;

								var caption = "'Start-Commit' Hook Notification";
								var message = new StringBuilder();
								message.AppendLine("The 'Start-Commit' hook has updated a time stamp.");
								message.AppendLine();
								message.AppendLine("File:");
								message.AppendLine(filePathCovered);
								message.AppendLine();
								message.AppendLine("Time Stamp:");
								message.AppendLine(timeStampString);
								var dr = MessageBox.Show(message.ToString(), caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
								if (dr != DialogResult.OK) {
									return (Result.Cancel);
								}

								break; // Single change per pattern is sufficient.
							}
						}
					}
				}

				if (timeStampLinesHaveChanged)
				{
					File.WriteAllLines(timeStampFilePath, timeStampLines, Encoding.UTF8);

					// Note that "File.WriteAllLines()" appends a trailing empty line. While not ideal and typically not done
					// with e.g. XML files, this is also typically done for source code files, thus considered acceptable.
					// https://stackoverflow.com/questions/11689337/net-file-writealllines-leaves-empty-line-at-the-end-of-file
					// half way down the page shows a "WriteAllLinesBetter()", but as mentioned above, not needed here.
				}
			}

			return (Result.Success);
		}
	}
}

//==================================================================================================
// End of
// $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/!-Doc.Developer/Template.cs $
//==================================================================================================
