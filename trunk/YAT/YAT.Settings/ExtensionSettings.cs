//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY.IO;

namespace YAT.Settings
{
	/// <summary>
	/// Defines extension filters for open/save dialogs.
	/// </summary>
	public static class ExtensionSettings
	{
		private static string terminalFile  = ".yat";
		private static string workspaceFile = ".yaw";

		/// <summary>
		/// Allows to alter the file extension used for terminal files,
		/// e.g. ".ab1" instead of ".yat".
		/// </summary>
		public static string TerminalFile
		{
			get { return (terminalFile); }
			set { terminalFile = value; }
		}

		/// <summary></summary>
		public static bool IsTerminalFile(string extension)
		{
			if (PathEx.Equals(extension, TerminalFile))
				return (true);
			else
				return (false);
		}

		/// <summary></summary>
		public static string TerminalFilesFilter
		{
			get { return ("Terminal Files (*" + TerminalFile + ")|*" + TerminalFile); }
		}

		/// <summary>
		/// Allows to alter the file extension used for workspace files,
		/// e.g. ".ab2" instead of ".yaw".
		/// </summary>
		public static string WorkspaceFile
		{
			get { return (workspaceFile); }
			set { workspaceFile = value; }
		}

		/// <summary></summary>
		public static bool IsWorkspaceFile(string extension)
		{
			if (PathEx.Equals(extension, WorkspaceFile))
				return (true);
			else
				return (false);
		}

		/// <summary></summary>
		public static string WorkspaceFilesFilter
		{
			get { return ("Workspace Files (*" + WorkspaceFile + ")|*" + WorkspaceFile); }
		}

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
		public static string TextFilesDefault
		{
			get { return (".txt"); }
		}

		/// <summary></summary>
		public static string LogFilesDefault
		{
			get { return (".log"); }
		}

		/// <summary></summary>
		public static string MonitorFilesDefault
		{
			get { return (".rtf"); }
		}

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
		public static string BinaryFilesFilter
		{
			get
			{
				return ("Binary Files (*.dat;*.bin;*.binary)|*.dat;*.bin;*.binary" +
						"|" +
						"All Files (*.*)|*.*");
			}
		}

		/// <summary></summary>
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

		/// <summary></summary>
		public static string BinaryFilesDefault
		{
			get { return (".dat"); }
		}

		/// <summary></summary>
		public static string ExecutableFiles
		{
			get { return (".exe"); }
		}

		/// <summary></summary>
		public static bool IsExecutableFile(string extension)
		{
			if (PathEx.Equals(extension, ExecutableFiles))
				return (true);
			else
				return (false);
		}

		/// <summary></summary>
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
