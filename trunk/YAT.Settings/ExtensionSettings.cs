//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Settings
{
	/// <summary>
	/// Defines extension filters for open/save dialogs.
	/// </summary>
	public static class ExtensionSettings
	{
		private static string _terminalFileExtension  = ".yat";
		private static string _workspaceFileExtension = ".yaw";

		/// <summary>
		/// Allows to alter the file extension used for terminal files,
		/// e.g. ".ab1" instead of ".yat".
		/// </summary>
		public static string TerminalFiles
		{
			get { return (_terminalFileExtension); }
			set { _terminalFileExtension = value; }
		}

		public static bool IsTerminalFile(string extension)
		{
			if (extension == TerminalFiles)
				return (true);
			else
				return (false);
		}

		public static string TerminalFilesFilter
		{
			get { return ("Terminal Files (*" + TerminalFiles + ")|*" + TerminalFiles); }
		}

		/// <summary>
		/// Allows to alter the file extension used for workspace files,
		/// e.g. ".ab2" instead of ".yaw".
		/// </summary>
		public static string WorkspaceFiles
		{
			get { return (_workspaceFileExtension); }
			set { _workspaceFileExtension = value; }
		}

		public static bool IsWorkspaceFile(string extension)
		{
			if (extension == WorkspaceFiles)
				return (true);
			else
				return (false);
		}

		public static string WorkspaceFilesFilter
		{
			get { return ("Workspace Files (*" + WorkspaceFiles + ")|*" + WorkspaceFiles); }
		}

		public static string TextFilesFilter
		{
			get
			{
				return ("Text Files (*.txt;*.text;*.log)|*.txt;*.text;*.log" +
						"|" +
						"Rich Text Files (*.rtf)|*.rtf" +
						"|" +
						"XML Files (*.xml)|*.xml" +
						"|" +
						"All Files (*.*)|*.*");
			}
		}

		public static string[] TextFilesWithDot
		{
			get
			{
				List<string> l = new List<string>();
				l.Add(".txt");
				l.Add(".text");
				l.Add(".log");
				l.Add(".rtf");
				l.Add(".xml");
				return (l.ToArray());
			}
		}

		public static string TextFilesDefault
		{
			get { return (".txt"); }
		}

		public static string LogFilesDefault
		{
			get { return (".log"); }
		}

		public static string MonitorFilesDefault
		{
			get { return (".rtf"); }
		}

		public static bool IsTextFile(string extension)
		{
			switch (extension)
			{
				case ".txt":
				case ".text":
				case ".log":
					return (true);

				default:
					return (false);
			}
		}

		public static bool IsRtfFile(string extension)
		{
			switch (extension)
			{
				case ".rtf":
					return (true);

				default:
					return (false);
			}
		}

		public static bool IsXmlFile(string extension)
		{
			switch (extension)
			{
				case ".xml":
					return (true);

				default:
					return (false);
			}
		}

		public static string BinaryFilesFilter
		{
			get
			{
				return ("Binary Files (*.dat;*.bin;*.binary)|*.dat;*.bin;*.binary" +
						"|" +
						"All Files (*.*)|*.*");
			}
		}

		public static string[] BinaryFilesWithDot
		{
			get
			{
				List<string> l = new List<string>();
				l.Add(".dat");
				l.Add(".bin");
				l.Add(".binary");
				return (l.ToArray());
			}
		}

		public static string BinaryFilesDefault
		{
			get { return (".dat"); }
		}

		public static string ExecutableFiles
		{
			get { return (".exe"); }
		}

		public static bool IsExecutableFile(string extension)
		{
			if (extension == ExecutableFiles)
				return (true);
			else
				return (false);
		}

		public static string AllFilesFilter
		{
			get { return ("All Files (*.*)|*.*"); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
