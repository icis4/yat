//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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

using MKY;

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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't seem to be able to deal with abbreviations or extensions such as '.ab1'...")]
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't seem to be able to deal with abbreviations or extensions such as '.ab1'...")]
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

		#endregion

		#region Text/Binary/Send/Monitor/Log
		//------------------------------------------------------------------------------------------
		// Text/Binary/Send/Monitor/Log
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static bool IsTextExtension(string extension)
		{
			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".txt"))
				return (true);

			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".text"))
				return (true);

			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".log"))
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
			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".dat"))
				return (true);

			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".bin"))
				return (true);

			if (StringEx.EqualsOrdinalIgnoreCase(extension, ".binary"))
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
			return (StringEx.EqualsOrdinalIgnoreCase(extension, ".rtf"));
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
			return (StringEx.EqualsOrdinalIgnoreCase(extension, ".xml"));
		}

		/// <summary></summary>
		public static bool IsXmlFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsXmlExtension(extension));
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
		public static ReadOnlyCollection<string> TextFilesWithDot
		{
			get
			{
				List<string> l = new List<string>(5); // Preset the required capacity to improve memory management.
				l.Add(".txt");
				l.Add(".text");
				l.Add(".log");
				l.Add(".rtf");
				l.Add(".xml");
				return (l.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static string TextFilesDefault
		{
			get { return (".txt"); }
		}

		/// <summary></summary>
		public static string NeatLogFilesDefault
		{
			get { return (".log"); }
		}

		/// <summary></summary>
		public static string BinaryFilesFilter
		{
			get
			{
				return ("Binary Files (*.dat;*.bin;*.binary)|*.dat;*.bin;*.binary" +
						"|" +
						"XML Files (*.xml)|*.xml" +
						"|" +
						"All Files (*.*)|*.*");
			}
		}

		/// <summary></summary>
		public static int BinaryFilesFilterHelper(string extension)
		{
			if      (IsBinaryFile(extension))
				return (1);
			else if (IsXmlExtension(extension))
				return (2);
			else
				return (3);
		}

		/// <summary></summary>
		public static ReadOnlyCollection<string> BinaryFilesWithDot
		{
			get
			{
				List<string> l = new List<string>(4); // Preset the required capacity to improve memory management.
				l.Add(".dat");
				l.Add(".bin");
				l.Add(".binary");
				l.Add(".xml");
				return (l.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static string BinaryFilesDefault
		{
			get { return (".dat"); }
		}

		/// <summary></summary>
		public static string RawLogFilesDefault
		{
			get { return (".dat"); } // = BinaryFilesDefault
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
			return (StringEx.EqualsOrdinalIgnoreCase(extension, ".exe"));
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
