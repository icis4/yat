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
using System.Text;
using System.Windows.Forms;

namespace YATInfra.SVNHooks
{
	public enum Result
	{
		UnhandledException = -2,
		CommandLineError = -1,
		Success = 0,
		Cancel = 1,
		TimeStampFileError = 2
	}

	public static class TimeStampFileHelper
	{
		public const string FileName = ".timestamp";

		/// <remarks>
		/// The "o" format corresponds to "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK".
		/// But milliseconds make little sense for a file time stamp.
		/// </remarks>
		public const string Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

		public static Result ValidateLine(string filePath, string line, out DateTime timeStamp, out string fileNamePattern)
		{
			timeStamp = DateTime.MinValue;
			fileNamePattern = null;

			var lineSplit = line.Split('|');
			if (lineSplit.Length != 2)
			{
				var caption = ".timestamp File Error";
				var message = new StringBuilder();
				message.AppendLine("A .timestamp file must contain lines separated by '|'!");
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
				var message = new StringBuilder();
				message.AppendLine("A .timestamp file must contain valid time stamps!");
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

			fileNamePattern = lineSplit[1];
			if (string.IsNullOrWhiteSpace(fileNamePattern))
			{
				var caption = ".timestamp File Error";
				var message = new StringBuilder();
				message.AppendLine("A .timestamp file must contain valid file name patterns!");
				message.AppendLine();
				message.AppendLine("File Name Pattern:");
				message.AppendLine(fileNamePattern);
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

	public static class DebuggerHookHelper
	{
		public static readonly string Message =
			"This is an intended break while running SVN." + Environment.NewLine +
			Environment.NewLine +
			"It allows debugging hooks without without having to change the hook's command line in the SVN settings:" + Environment.NewLine +
			Environment.NewLine +
			"    1. Attach the Debugger to TortoiseSVN [Ctrl+Alt+P]." + Environment.NewLine +
			"    2. [Debug > Break All] or [Ctrl+Alt+Break]" + Environment.NewLine +
			"        and/or set breakpoints as needed." + Environment.NewLine +
			"    3. [Debug > Continue] or [F5]." + Environment.NewLine +
			"    4. Confirm this message with [OK].";

		public static bool Once; // = false;
	}
}

//==================================================================================================
// End of
// $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/!-Doc.Developer/Template.cs $
//==================================================================================================
