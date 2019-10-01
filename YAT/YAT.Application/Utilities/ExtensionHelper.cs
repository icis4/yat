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
// YAT Version 2.1.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY.IO;

#endregion

namespace YAT.Application.Utilities
{
	/// <summary>
	/// Defines extension filters for open/save dialogs.
	/// </summary>
	public static class ExtensionHelper
	{
		#region Terminal/Workspace
		//------------------------------------------------------------------------------------------
		// Terminal/Workspace
		//------------------------------------------------------------------------------------------

		private static string terminalFile  = ".yat";
		private static string workspaceFile = ".yaw";

		/// <summary>
		/// Allows to alter the file extension used for terminal files,
		/// e.g. ".ab1" instead of ".yat".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string TerminalFile
		{
			get { return (terminalFile); }
			set { terminalFile = value;  }
		}

		/// <summary></summary>
		public static bool IsTerminalFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (PathEx.Equals(extension, TerminalFile));
		}

		/// <summary></summary>
		public static string TerminalFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Terminal Files (*" + TerminalFile + ")|*" + TerminalFile); }
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string WorkspaceFile
		{
			get { return (workspaceFile); }
			set { workspaceFile = value;  }
		}

		/// <summary></summary>
		public static bool IsWorkspaceFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (PathEx.Equals(extension, WorkspaceFile));
		}

		/// <summary></summary>
		public static string WorkspaceFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Workspace Files (*" + WorkspaceFile + ")|*" + WorkspaceFile); }
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
						"All " + ApplicationEx.CommonName + " Files (*" + TerminalFile + ";*" + WorkspaceFile + ")|*" + TerminalFile + ";*" + WorkspaceFile);
			}
		}

		/// <summary></summary>
		public static int TerminalOrWorkspaceFilesFilterDefault
		{
			get { return (3); }
		}

		#endregion

		#region Command/CommandPages
		//------------------------------------------------------------------------------------------
		// Command/CommandPages
		//------------------------------------------------------------------------------------------

		private static string commandFile  = ".yac";
		private static string commandPageFile = ".yacp";
		private static string commandPagesFile = ".yacps";

		/// <summary>
		/// Allows to alter the file extension used for command files,
		/// e.g. ".ab3" instead of ".yac".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string CommandFile
		{
			get { return (commandFile); }
			set { commandFile = value;  }
		}

		/// <summary></summary>
		public static bool IsCommandFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (PathEx.Equals(extension, CommandFile));
		}

		/// <summary></summary>
		public static string CommandFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Command Files (*" + CommandFile + ")|*" + CommandFile); }
		}

		/// <summary></summary>
		public static int CommandFilesFilterDefault
		{
			get { return (1); }
		}

		/// <summary>
		/// Allows to alter the file extension used for command page files,
		/// e.g. ".ab4" instead of ".yacp".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string CommandPageFile
		{
			get { return (commandPageFile); }
			set { commandPageFile = value;  }
		}

		/// <summary></summary>
		public static bool IsCommandPageFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (PathEx.Equals(extension, CommandPageFile));
		}

		/// <summary></summary>
		public static string CommandPageFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Command Page Files (*" + CommandPageFile + ")|*" + CommandPageFile); }
		}

		/// <summary></summary>
		public static int CommandPageFilesFilterDefault
		{
			get { return (1); }
		}

		/// <summary>
		/// Allows to alter the file extension used for command pages files,
		/// e.g. ".ab5" instead of ".yacps".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string CommandPagesFile
		{
			get { return (commandPagesFile); }
			set { commandPagesFile = value;  }
		}

		/// <summary></summary>
		public static bool IsCommandPagesFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (PathEx.Equals(extension, CommandPagesFile));
		}

		/// <remarks>
		/// Prepended "Multiple" to clearly distinguish from single command page.
		/// </remarks>
		public static string CommandPagesFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Multiple Command Pages Files (*" + CommandPagesFile + ")|*" + CommandPagesFile); }
		}

		/// <summary></summary>
		public static int CommandPagesFilesFilterDefault
		{
			get { return (1); }
		}

		/// <summary></summary>
		public static string CommandPageOrPagesFilesFilter
		{
			get
			{
				return (CommandPageFilesFilter +
						"|" +
						CommandPagesFilesFilter +
						"|" +
						"All " + ApplicationEx.CommonName + " Command Page(s) Files (*" + CommandPageFile + ";*" + CommandPagesFile + ")|*" + CommandPageFile + ";*" + CommandPagesFile);
			}
		}

		/// <summary></summary>
		public static int CommandPageOrPagesFilesFilterDefault
		{
			get { return (3); }
		}

		/// <summary></summary>
		public static string CommandPageOrPagesOrTerminalFilesFilter
		{
			get
			{
				return (CommandPageFilesFilter +
						"|" +
						CommandPagesFilesFilter +
						"|" +
						"All " + ApplicationEx.CommonName + " Command Page(s) Files (*" + CommandPageFile + ";*" + CommandPagesFile + ")|*" + CommandPageFile + ";*" + CommandPagesFile +
						"|" +
						TerminalFilesFilter +
						"|" +
						"All " + ApplicationEx.CommonName + " Command Page(s) Sources (*" + CommandPageFile + ";*" + CommandPagesFile + ";*" + TerminalFile + ")|*" + CommandPageFile + ";*" + CommandPagesFile + ";*" + TerminalFile);
			}
		}

		/// <summary></summary>
		public static int CommandPageOrPagesOrTerminalFilesFilterDefault
		{
			get { return (3); }
		}

		#endregion

		#region Text/Binary/Send/Log/Monitor
		//------------------------------------------------------------------------------------------
		// Text/Binary/Send/Log/Monitor
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static bool IsTextExtension(string extension)
		{
			if (PathEx.Equals(extension, ".txt"))
				return (true);

			if (PathEx.Equals(extension, ".text"))
				return (true);

			if (PathEx.Equals(extension, ".log"))
				return (true);

			return (false);
		}

		/// <summary></summary>
		public static bool IsTextFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsTextExtension(extension));
		}

		/// <summary></summary>
		public static bool IsBinaryExtension(string extension)
		{
			if (PathEx.Equals(extension, ".dat"))
				return (true);

			if (PathEx.Equals(extension, ".bin"))
				return (true);

			if (PathEx.Equals(extension, ".binary"))
				return (true);

			return (false);
		}

		/// <summary></summary>
		public static bool IsBinaryFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsBinaryExtension(extension));
		}

		/// <summary></summary>
		public static bool IsRtfExtension(string extension)
		{
			return (PathEx.Equals(extension, ".rtf"));
		}

		/// <summary></summary>
		public static bool IsRtfFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsRtfExtension(extension));
		}

		/// <summary></summary>
		public static bool IsXmlExtension(string extension)
		{
			return (PathEx.Equals(extension, ".xml"));
		}

		/// <summary></summary>
		public static bool IsXmlFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsXmlExtension(extension));
		}

		/// <summary></summary>
		public static bool IsFileTypeThatCanOnlyBeOpenedWhenCompleted(string filePath)
		{
			return (IsRtfExtension(filePath) || IsXmlFile(filePath));
		}

		/// <remarks>
		/// Applies to [Send File] as well as [Monitor > Save].
		/// </remarks>
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
		public static int TextFilesFilterHelper(string extension)
		{
			if      (IsTextFile(extension))
				return (1);
			else if (IsRtfExtension(extension))
				return (2);
			else if (IsXmlExtension(extension))
				return (3);
			else
				return (4);
		}

		/// <summary></summary>
		public static string TextSendFilesDefault
		{
			get { return (".txt"); }
		}

		/// <summary></summary>
		public static ReadOnlyCollection<string> PortLogFileExtensionsWithDot
		{
			get
			{                            // See comment below!
				var l = new List<string>(4); // Preset the required capacity to improve memory management.
				l.Add(".txt");
				l.Add(".text");
				l.Add(".log");
				l.Add(".rtf");
			////l.Add(".xml"); not supported (yet).
				return (l.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static string PortLogFilesDefault
		{
			get { return (".log"); } // = NeatLogFilesDefault
		}

		/// <summary></summary>
		public static ReadOnlyCollection<string> NeatLogFileExtensionsWithDot
		{
			get
			{
				var l = new List<string>(5); // Preset the required capacity to improve memory management.
				l.Add(".txt");
				l.Add(".text");
				l.Add(".log");
				l.Add(".rtf");
				l.Add(".xml");
				return (l.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static string NeatLogFilesDefault
		{
			get { return (".log"); } // = PortLogFilesDefault
		}

		/// <remarks>
		/// Only applies to [Send File], i.e. does contain text file extensions.
		/// </remarks>
		public static string BinarySendFilesFilter
		{
			get
			{
				return ("Binary Files (*.dat;*.bin;*.binary)|*.dat;*.bin;*.binary" +
						"|" +
						"Text Files (*.txt;*.text;*.log)|*.txt;*.text;*.log" +
						"|" +
						"XML Files (*.xml)|*.xml" +
						"|" +
						"All Files (*.*)|*.*");
			}
		}

		/// <remarks>
		/// Only applies to [Send File], i.e. does contain text file extensions.
		/// </remarks>
		public static int BinarySendFilesFilterHelper(string extension)
		{
			if      (IsBinaryFile(extension))
				return (1);
			else if (IsTextExtension(extension))
				return (2);
			else if (IsXmlExtension(extension))
				return (3);
			else
				return (4);
		}

		/// <summary></summary>
		public static string BinarySendFilesDefault
		{
			get { return (".dat"); } // = RawLogFilesDefault
		}

		/// <remarks>
		/// Only applies to log files, i.e. does not contain text file extensions (which are also sendable by binary terminals).
		/// </remarks>
		public static ReadOnlyCollection<string> RawLogFileExtensionsWithDot
		{
			get
			{
				var l = new List<string>(4); // Preset the required capacity to improve memory management.
				l.Add(".dat");
				l.Add(".bin");
				l.Add(".binary");
				l.Add(".xml");
				return (l.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static string RawLogFilesDefault
		{
			get { return (".dat"); } // = BinarySendFilesDefault
		}

		/// <summary></summary>
		public static string MonitorFilesDefault
		{
			get { return (".rtf"); }
		}

		#endregion

		#region Executable
		//------------------------------------------------------------------------------------------
		// Executable
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static bool IsExecutableExtension(string extension)
		{
			return (PathEx.Equals(extension, ".exe"));
		}

		/// <summary></summary>
		public static bool IsExecutableFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsExecutableExtension(extension));
		}

		#endregion

		#region All
		//------------------------------------------------------------------------------------------
		// All
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static string AllFilesFilter
		{
			get { return ("All Files (*.*)|*.*"); }
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
