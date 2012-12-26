//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using MKY;

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
		public static bool IsTerminalFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (StringEx.EqualsOrdinalIgnoreCase(extension, TerminalFile));
		}

		/// <summary></summary>
		public static string TerminalFilesFilter
		{
			get { return ("Terminal Files (*" + TerminalFile + ")|*" + TerminalFile); }
		}

		/// <summary></summary>
		public static int TerminalFilesFilterDefault
		{
			get { return (1); }
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
		public static bool IsWorkspaceFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (StringEx.EqualsOrdinalIgnoreCase(extension, WorkspaceFile));
		}

		/// <summary></summary>
		public static string WorkspaceFilesFilter
		{
			get { return ("Workspace Files (*" + WorkspaceFile + ")|*" + WorkspaceFile); }
		}

		/// <summary></summary>
		public static int WorkspaceFilesFilterDefault
		{
			get { return (1); }
		}

		/// <summary></summary>
		public static string TerminalOrWorkspaceFilesFilter
		{
			get
			{
				return (TerminalFilesFilter +
						"|" +
						WorkspaceFilesFilter +
						"|" +
						"Terminal and Workspace Files (*" + TerminalFile + ";*" + WorkspaceFile + ")|*" + TerminalFile + ";*" + WorkspaceFile);
			}
		}

		/// <summary></summary>
		public static int TerminalOrWorkspaceFilesFilterDefault
		{
			get { return (3); }
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
		public static ReadOnlyCollection<string> TextFilesWithDot
		{
			get
			{
				List<string> l = new List<string>();
				l.Add(".txt");
				l.Add(".text");
				l.Add(".log");
				l.Add(".rtf");
				l.Add(".xml");
				return (l.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static int TextFilesFilterDefault
		{
			get { return (1); }
		}

		/// <summary></summary>
		public static string TextFilesDefault
		{
			get { return (".txt"); }
		}

		/// <summary></summary>
		public static int LogFilesFilterDefault
		{
			get { return (1); }
		}

		/// <summary></summary>
		public static string LogFilesDefault
		{
			get { return (".log"); }
		}

		/// <summary></summary>
		public static int MonitorFilesFilterDefault
		{
			get { return (2); }
		}

		/// <summary></summary>
		public static string MonitorFilesDefault
		{
			get { return (".rtf"); }
		}

		/// <summary></summary>
		public static bool IsTextFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);

			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".txt"))
				return (true);

			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".text"))
				return (true);

			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".log"))
				return (true);

			return (false);
		}

		/// <summary></summary>
		public static bool IsRtfFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (StringEx.EqualsOrdinalIgnoreCase(extension, ".rtf"));
		}

		/// <summary></summary>
		public static bool IsXmlFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (StringEx.EqualsOrdinalIgnoreCase(extension, ".xml"));
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
		public static ReadOnlyCollection<string> BinaryFilesWithDot
		{
			get
			{
				List<string> l = new List<string>();
				l.Add(".dat");
				l.Add(".bin");
				l.Add(".binary");
				return (l.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static int BinaryFilesFilterDefault
		{
			get { return (1); }
		}

		/// <summary></summary>
		public static string BinaryFilesDefault
		{
			get { return (".dat"); }
		}

		/// <summary></summary>
		public static string ExecutableFile
		{
			get { return (".exe"); }
		}

		/// <summary></summary>
		public static bool IsExecutableFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (StringEx.EqualsOrdinalIgnoreCase(extension, ExecutableFile));
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
