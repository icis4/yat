using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Utilities;

namespace YAT.Controller
{
	/// <summary>
	/// Application controller main class of YAT.
	/// </summary>
	/// <remarks>
	/// This class is separated into its own .exe project for those who want to use YAT
	/// components within their own context.
	/// </remarks>
	public class Main
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly string[] _Title =
		{
			ApplicationInfo.ProductName + " - Version " + Application.ProductVersion,
			"YAT - Yet Another Terminal",
			"RS-232/422/423/485 TCP/UDP terminal to operate and debug serial connections",
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2008 Matthias Kläy.",
		};

		private static readonly string[] _FileOptions =
		{
			"Usage:    ",
			"  YAT[.exe] [<WorkspaceSettings>.yaw|<TerminalSettings>.yat]",
			"          ",
			"Usage examples:",
			"  YAT MyWorkspace.yaw",
			"          Start YAT and open given workspace settings",
			"  YAT MyTerminal.yat",
			"          Start YAT and open given terminal settings",
		};

		private static readonly string[] _AdvancedOptions =
		{
			"Advanced usage:    ",
			"  YAT[.exe] [/r]",
			"          ",
			"  /r      ",
			"  -r      Open most recent file according to file list",
			"          ",
			"Advanced usage examples:",
			"  YAT /r  Start YAT and open most recent file",
		};

		private static readonly string[] _RecentArg =
		{
			"/r",
			"-r",
		};

		private static readonly string[] _Help =
		{
			"  /?      ",
			"  -?      ",
			"  -h      ",
			"  --help  Display this help text",
		};

		private static readonly string[] _HelpArg =
		{
			"/?",
			"-?",
			"-h",
			"--help",
		};

		private static readonly string[] _Return =
		{
			"Return codes:",
			"   0      Successful exit",
			"  -1      Command line argument error",
			"  -2      Application settings error",
			"  -3      Unhandled exception",
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// command line
		private bool _commandLineError = false;
		private bool _commandLineHelpIsRequested = false;

		private string _requestedFilePath = "";

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Main(string[] commandLineArgs)
		{
			// only parse 'real' command line args, i.e. skip first arg which is the .exe file path
			string[] realArgs = new string[commandLineArgs.Length - 1];
			for (int i = 1; i < commandLineArgs.Length; i++)
				realArgs[i - 1] = commandLineArgs[i];

			_commandLineError = (!ParseCommandLineArgs(realArgs));
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public bool CommandLineError
		{
			get { return (_commandLineError); }
		}

		public bool CommandLineHelpIsRequested
		{
			get { return (_commandLineHelpIsRequested); }
		}

		public string RequestedFilePath
		{
			get { return (_requestedFilePath); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public MainResult Run()
		{
			// show command line help in case of error
			if (_commandLineError)
			{
				WriteHelp();
				return (MainResult.CommandLineArgsError);
			}

			// show command line help if requested
			if (_commandLineHelpIsRequested)
			{
				WriteHelp();
				return (MainResult.OK);
			}

			// create model and view and run application
			using (Model.Main model = new Model.Main(_requestedFilePath))
			{
				using (Gui.Forms.Main view = new Gui.Forms.Main(model))
				{
					// start the Win32 message loop on the current thread and the main form
					// \attention This call does not return until the application exits
					Application.Run(view);
				}
			}
			// dispose model and view to ensure immediate release of resources

			return (MainResult.OK);
		}

		#endregion

		#region Command Line Args
		//==========================================================================================
		// Command Line Args
		//==========================================================================================

		private bool ParseCommandLineArgs(string[] commandLineArgs)
		{
			int argsParsed = 0;
			int argsParsedTotal = 0;

			if ((argsParsed = ParseArgsForHelp  (commandLineArgs)) < 0) return (false); else argsParsedTotal += argsParsed;
			if ((argsParsed = ParseArgsForFile  (commandLineArgs)) < 0) return (false); else argsParsedTotal += argsParsed;
			if ((argsParsed = ParseArgsForRecent(commandLineArgs)) < 0) return (false); else argsParsedTotal += argsParsed;

			if (argsParsedTotal != commandLineArgs.Length)
				return (false);

			return (true);
		}

		// write help text onto console
		private void WriteHelp()
		{
			foreach (string line in _Title)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in _FileOptions)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in _AdvancedOptions)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in _Help)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in _Return)
				Console.WriteLine(line);
			Console.WriteLine();
		}

		// parse args for help
		private int ParseArgsForHelp(string[] commandLineArgs)
		{
			int argsParsed = 0;
			foreach (string arg in commandLineArgs)
			{
				// check for help args
				foreach (string helpArg in _HelpArg)
				{
					if (string.Compare(arg, helpArg, true) == 0)
					{
						_commandLineHelpIsRequested = true;
						argsParsed++;
					}
				}
			}
			return (argsParsed);
		}

		// parse args for file
		private int ParseArgsForFile(string[] commandLineArgs)
		{
			int argsParsed = 0;
			foreach (string arg in commandLineArgs)
			{
				// check for workspace file args
				if (ExtensionSettings.IsWorkspaceFile(Path.GetExtension(arg)))
				{
					_requestedFilePath = arg;
					argsParsed++;
				}

				// check for terminal file args
				if (ExtensionSettings.IsTerminalFile(Path.GetExtension(arg)))
				{
					_requestedFilePath = arg;
					argsParsed++;
				}
			}
			return (argsParsed);
		}

		// parse args for recent
		private int ParseArgsForRecent(string[] commandLineArgs)
		{
			int argsParsed = 0;
			foreach (string arg in commandLineArgs)
			{
				foreach (string recentArg in _RecentArg)
				{
					if (string.Compare(arg, recentArg, true) == 0)
					{
						ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();
						bool recentsReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
						if (recentsReady)
						{
							_requestedFilePath = ApplicationSettings.LocalUser.RecentFiles.FilePaths[0].Item;
							argsParsed++;
						}
					}
				}
			}
			return (argsParsed);
		}

		#endregion
	}
}
