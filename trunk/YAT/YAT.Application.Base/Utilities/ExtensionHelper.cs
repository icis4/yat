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
// YAT Version 2.2.0 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
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

		private static string terminalExtension  = ".yat";
		private static string workspaceExtension = ".yaw";

		/// <summary>
		/// Allows to alter the file extension used for terminal files,
		/// e.g. ".ab1" instead of ".yat".
		/// </summary>
		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsTerminalFile(string)"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string TerminalExtension
		{
			get { return (terminalExtension); }
			set { terminalExtension = value;  }
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsTerminalFile(string)"/>.
		/// </remarks>
		public static bool IsTerminalExtension(string extension)
		{
			return (PathEx.Equals(extension, TerminalExtension));
		}

		/// <summary></summary>
		public static bool IsTerminalFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsTerminalExtension(extension));
		}

		/// <summary></summary>
		public static string TerminalFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Terminal Files (*" + TerminalExtension + ")|*" + TerminalExtension); }
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
		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsWorkspaceFile(string)"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string WorkspaceExtension
		{
			get { return (workspaceExtension); }
			set { workspaceExtension = value;  }
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsWorkspaceFile(string)"/>.
		/// </remarks>
		public static bool IsWorkspaceExtension(string extension)
		{
			return (PathEx.Equals(extension, WorkspaceExtension));
		}

		/// <summary></summary>
		public static bool IsWorkspaceFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsWorkspaceExtension(extension));
		}

		/// <summary></summary>
		public static string WorkspaceFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Workspace Files (*" + WorkspaceExtension + ")|*" + WorkspaceExtension); }
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
						"All " + ApplicationEx.CommonName + " Files (*" + TerminalExtension + ";*" + WorkspaceExtension + ")|*" + TerminalExtension + ";*" + WorkspaceExtension);
			}
		}

		/// <summary></summary>
		public static int TerminalOrWorkspaceFilesFilterDefault
		{
			get { return (3); }
		}

		#endregion

		#region Scripting
		//------------------------------------------------------------------------------------------
		// Scripting
		//------------------------------------------------------------------------------------------

	#if (WITH_SCRIPTING)

		private static string scriptExtension = ".cs";

		/// <summary>
		/// Allows to alter the file extension used for script files,
		/// e.g. ".xy" instead of ".cs".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string ScriptExtension
		{
			get { return (scriptExtension); }
			set { scriptExtension = value;  }
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsScriptFile(string)"/>.
		/// </remarks>
		public static bool IsScriptExtension(string extension)
		{
			return (PathEx.Equals(extension, ScriptExtension));
		}

		/// <summary></summary>
		public static bool IsScriptFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsScriptExtension(extension));
		}

		/// <summary></summary>
		public static string ScriptFilesFilter
		{
			get { return ("C# Files (*" + ScriptExtension + ")|*" + ScriptExtension); }
		}

		/// <summary></summary>
		public static int ScriptFilesFilterDefault
		{
			get { return (1); }
		}

	#endif // WITH_SCRIPTING

		#endregion

		#region Command/CommandPages
		//------------------------------------------------------------------------------------------
		// Command/CommandPages
		//------------------------------------------------------------------------------------------

		private static string commandExtension      = ".yac";
		private static string commandPageExtension  = ".yacp";
		private static string commandPagesExtension = ".yacps";

		/// <summary>
		/// Allows to alter the file extension used for command files,
		/// e.g. ".ab3" instead of ".yac".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with file extensions such as '.ab1'...")]
		public static string CommandExtension
		{
			get { return (commandExtension); }
			set { commandExtension = value;  }
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsCommandFile(string)"/>.
		/// </remarks>
		public static bool IsCommandExtension(string extension)
		{
			return (PathEx.Equals(extension, CommandExtension));
		}

		/// <summary></summary>
		public static bool IsCommandFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsCommandExtension(extension));
		}

		/// <summary></summary>
		public static string CommandFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Command Files (*" + CommandExtension + ")|*" + CommandExtension); }
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
		public static string CommandPageExtension
		{
			get { return (commandPageExtension); }
			set { commandPageExtension = value;  }
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsCommandPageFile(string)"/>.
		/// </remarks>
		public static bool IsCommandPageFileExtension(string extension)
		{
			return (PathEx.Equals(extension, CommandPageExtension));
		}

		/// <summary></summary>
		public static bool IsCommandPageFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsCommandPageFileExtension(extension));
		}

		/// <summary></summary>
		public static string CommandPageFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Command Page Files (*" + CommandPageExtension + ")|*" + CommandPageExtension); }
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
		public static string CommandPagesExtension
		{
			get { return (commandPagesExtension); }
			set { commandPagesExtension = value;  }
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsCommandPagesFile(string)"/>.
		/// </remarks>
		public static bool IsCommandPagesExtension(string extension)
		{
			return (PathEx.Equals(extension, CommandPagesExtension));
		}

		/// <summary></summary>
		public static bool IsCommandPagesFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsCommandPagesExtension(extension));
		}

		/// <remarks>
		/// Prepended "Multiple" to clearly distinguish from single command page.
		/// </remarks>
		public static string CommandPagesFilesFilter
		{
			get { return (ApplicationEx.CommonName + " Multiple Command Pages Files (*" + CommandPagesExtension + ")|*" + CommandPagesExtension); }
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
						"All " + ApplicationEx.CommonName + " Command Page(s) Files (*" + CommandPageExtension + ";*" + CommandPagesExtension + ")|*" + CommandPageExtension + ";*" + CommandPagesExtension);
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
						"All " + ApplicationEx.CommonName + " Command Page(s) Files (*" + CommandPageExtension + ";*" + CommandPagesExtension + ")|*" + CommandPageExtension + ";*" + CommandPagesExtension +
						"|" +
						TerminalFilesFilter +
						"|" +
						"All " + ApplicationEx.CommonName + " Command Page(s) Sources (*" + CommandPageExtension + ";*" + CommandPagesExtension + ";*" + TerminalExtension + ")|*" + CommandPageExtension + ";*" + CommandPagesExtension + ";*" + TerminalExtension);
			}
		}

		/// <summary></summary>
		public static int CommandPageOrPagesOrTerminalFilesFilterDefault
		{
			get { return (3); }
		}

		#endregion

		#region Text/Binary/Send/Log/Monitor/Plot
		//------------------------------------------------------------------------------------------
		// Text/Binary/Send/Log/Monitor/Plot
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsTextFile(string)"/>.
		/// </remarks>
		public static bool IsTextExtension(string extension)
		{
			if (PathEx.Equals(extension, ".txt"))  return (true);
			if (PathEx.Equals(extension, ".text")) return (true);
			if (PathEx.Equals(extension, ".log"))  return (true);

			return (false);
		}

		/// <summary></summary>
		public static bool IsTextFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsTextExtension(extension));
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsBinaryFile(string)"/>.
		/// </remarks>
		public static bool IsBinaryExtension(string extension)
		{
			if (PathEx.Equals(extension, ".dat"))    return (true);
			if (PathEx.Equals(extension, ".bin"))    return (true);
			if (PathEx.Equals(extension, ".binary")) return (true);

			return (false);
		}

		/// <summary></summary>
		public static bool IsBinaryFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsBinaryExtension(extension));
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsRtfFile(string)"/>.
		/// </remarks>
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

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsXmlFile(string)"/>.
		/// </remarks>
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
		public static string TextSendExtensionDefault
		{
			get { return (".txt"); }
		}

		/// <summary></summary>
		public static ReadOnlyCollection<string> ControlLogExtensions
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
		public static string ControlLogExtensionDefault
		{
			get { return (".log"); } // = NeatLogExtensionDefault
		}

		/// <summary></summary>
		public static ReadOnlyCollection<string> NeatLogExtensions
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
		public static string NeatLogExtensionDefault
		{
			get { return (".log"); } // = ControlLogExtensionDefault
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
		public static string BinarySendExtensionDefault
		{
			get { return (".dat"); } // = RawLogExtensionDefault
		}

		/// <remarks>
		/// Only applies to log files, i.e. does not contain text file extensions (which are also sendable by binary terminals).
		/// </remarks>
		public static ReadOnlyCollection<string> RawLogExtensions
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
		public static string RawLogExtensionDefault
		{
			get { return (".dat"); } // = BinarySendExtensionDefault
		}

		/// <summary></summary>
		public static string MonitorExtensionDefault
		{
			get { return (".rtf"); }
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsImageFile(string)"/>.
		/// </remarks>
		public static bool IsImageExtension(string extension)
		{
			ImageFormat format;
			return (IsImageExtension(extension, out format));
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsImageFile(string)"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool IsImageExtension(string extension, out ImageFormat format)
		{
			if (PathEx.Equals(extension, ".png")) { format = ImageFormat.Png; return (true); }
			if (PathEx.Equals(extension, ".bmp")) { format = ImageFormat.Bmp; return (true); }
			if (PathEx.Equals(extension, ".emf")) { format = ImageFormat.Emf; return (true); }
			if (PathEx.Equals(extension, ".wmf")) { format = ImageFormat.Wmf; return (true); }

			format = null;
			return (false);
		}

		/// <summary></summary>
		public static bool IsImageFile(string filePath)
		{
			ImageFormat format;
			return (IsImageFile(filePath, out format));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool IsImageFile(string filePath, out ImageFormat format)
		{
			string extension = Path.GetExtension(filePath);
			return (IsImageExtension(extension, out format));
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsSvgFile(string)"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Svg", Justification = "SVG is a file format.")]
		public static bool IsSvgExtension(string extension)
		{
			return (PathEx.Equals(extension, ".svg"));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Svg", Justification = "SVG is a file format.")]
		public static bool IsSvgFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsSvgExtension(extension));
		}

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsPdfFile(string)"/>.
		/// </remarks>
		public static bool IsPdfExtension(string extension)
		{
			return (PathEx.Equals(extension, ".pdf"));
		}

		/// <summary></summary>
		public static bool IsPdfFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return (IsPdfExtension(extension));
		}

		/// <remarks>
		/// Applies to [Plot > Save].
		/// </remarks>
		public static string PlotFilesFilter
		{
			get
			{
				return ("Images (*.png;*.bmp;*.svg;*.emf;*.wmf)|*.png;*.bmp;*.svg;*.emf;*.wmf" +
						"|" +
						"Portable Documents (*.pdf)|*.pdf" +
						"|" +
						"All Files (*.*)|*.*");
			}
		}

		/// <summary></summary>
		public static int PlotFilesFilterHelper(string extension)
		{
			if      (IsImageFile(extension))
				return (1);
			else if (IsPdfExtension(extension))
				return (2);
			else
				return (3);
		}

		/// <summary></summary>
		public static string PlotExtensionDefault
		{
			get { return (".png"); }
		}

		#endregion

		#region Executable
		//------------------------------------------------------------------------------------------
		// Executable
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Intentionally replicating term 'Extension' for better distinction with <see cref="IsExecutableFile(string)"/>.
		/// </remarks>
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
