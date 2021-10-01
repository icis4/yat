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

namespace YATInfra.SVNHooks.PostUpdate
{
	public class Program
	{
		private enum Result
		{
			UnhandledException = -2,
			CommandLineError = -1,
			Success = 0,
			TimeStampFileError = 1
		}

		private const string TimeStampFileName = ".timestamp";

		/// <summary>
		/// Mains the specified arguments.
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
				var caption = "'Post-Update' Unhandled Exception";
				var message = new StringBuilder("The 'Post-Update' hook has resulted in an unhandled exception!");
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
				var caption = "'Post-Update' Error";
				var message = new StringBuilder("The 'Post-Update' hook requires 6 arguments 'PATH DEPTH REVISION ERROR CWD RESULTPATH'!");
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
			// D:/Workspace/YAT/Trunk/YAT
			// D:/Workspace/YAT/Trunk/YAT/netrtfwriter/README.txt

			foreach (var resultPath in resultPaths)
			{
				if (File.Exists(resultPath))
				{
					var directoryPath = Path.GetDirectoryName(resultPath);
					var timeStampFilePath = Path.Combine(directoryPath, TimeStampFileName);
					if (File.Exists(timeStampFilePath))
					{
						var timeStampLines = File.ReadAllLines(timeStampFilePath);
						foreach (var timeStampLine in timeStampLines)
						{
							if (!string.IsNullOrWhiteSpace(timeStampLine)) // Allow leading, separating or trailing empty lines.
							{
								DateTime timeStamp;
								string searchPattern;
								var result = ValidateTimeStampLine(timeStampFilePath, timeStampLine, out timeStamp, out searchPattern);
								if (result != Result.Success) {
									return (result);
								}

								var filePaths = Directory.GetFiles(directoryPath, searchPattern);
								foreach (var filePath in filePaths)
								{
									if (filePath != timeStampFilePath) // This is the case when pattern is "*".
									{
										File.SetCreationTime(filePath, timeStamp);
										File.SetLastWriteTime(filePath, timeStamp);
										File.SetLastAccessTime(filePath, timeStamp);
									}
								}
							}
						}
					}
				}
			}

			return (Result.Success);
		}

		private static Result ValidateTimeStampLine(string filePath, string line, out DateTime timeStamp, out string searchPattern)
		{
			timeStamp = DateTime.MinValue;
			searchPattern = null;

			var lineSplit = line.Split('|');
			if (lineSplit.Length != 2)
			{
				var caption = ".timestamp File Error";
				var message = new StringBuilder("A .timestamp file must contain lines separated by '|'!");
				message.AppendLine();
				message.AppendLine("Line:");
				message.AppendLine(line);
				message.AppendLine();
				message.AppendLine("File:");
				message.AppendLine(filePath);
				MessageBox.Show(message.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return (Result.TimeStampFileError);
			}

			var timeStampString = lineSplit[0];
			if (!DateTime.TryParse(timeStampString, out timeStamp))
			{
				var caption = ".timestamp File Error";
				var message = new StringBuilder("A .timestamp file must contain valid time stamps!");
				message.AppendLine();
				message.AppendLine("Time Stamp:");
				message.AppendLine(timeStampString);
				message.AppendLine();
				message.AppendLine("Line:");
				message.AppendLine(line);
				message.AppendLine();
				message.AppendLine("File:");
				message.AppendLine(filePath);
				MessageBox.Show(message.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return (Result.TimeStampFileError);
			}

			searchPattern = lineSplit[1];
			if (string.IsNullOrWhiteSpace(searchPattern))
			{
				var caption = ".timestamp File Error";
				var message = new StringBuilder("A .timestamp file must contain valid file search patterns!");
				message.AppendLine();
				message.AppendLine("File Search Pattern:");
				message.AppendLine(searchPattern);
				message.AppendLine();
				message.AppendLine("Line:");
				message.AppendLine(line);
				message.AppendLine();
				message.AppendLine("File:");
				message.AppendLine(filePath);
				MessageBox.Show(message.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return (Result.TimeStampFileError);
			}

			return (Result.Success);
		}
	}
}

//==================================================================================================
// End of
// $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/!-Doc.Developer/Template.cs $
//==================================================================================================
