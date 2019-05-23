//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

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
	public class Main : IDisposable
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
			"Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.",
			"Copyright � 2003-2009 Matthias Kl�y.",
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
			"  -3      Application start error",
			"  -4      Application exit error",
			"  -5      Unhandled exception",
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

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
			// parse command line args if there are
			if (commandLineArgs.Length > 0)
				_commandLineError = (!ParseCommandLineArgs(commandLineArgs));
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~Main()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

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
			return (Run(true));
		}

		public MainResult Run(bool runWithView)
		{
			// Show command line help in case of error
			if (_commandLineError)
			{
				WriteHelp();
				return (MainResult.CommandLineArgsError);
			}

			// Show command line help if requested
			if (_commandLineHelpIsRequested)
			{
				WriteHelp();
				return (MainResult.OK);
			}

			// Create model and view and run application
			MainResult mainResult;
			using (Model.Main model = new Model.Main(_requestedFilePath))
			{
				if (runWithView)
				{
					using (Gui.Forms.Main view = new Gui.Forms.Main(model))
					{
						// Start the Win32 message loop on the current thread and the main form
						// \attention This call does not return until the application exits
						Application.Run(view);
					}
					mainResult = MainResult.OK;
				}
				else // Non-view application for automated test usage
				{
					if (model.Start())
					{
						if (model.Exit())
							mainResult = MainResult.OK;
						else
							mainResult = MainResult.ApplicationExitError;
					}
					else
					{
						mainResult = MainResult.ApplicationStartError;
					}
				}
			}
			// Dispose of model and view to ensure immediate release of resources

			return (mainResult);
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

			if ((argsParsed = ParseArgsForHelp(commandLineArgs)) < 0)   return (false); else argsParsedTotal += argsParsed;
			if ((argsParsed = ParseArgsForFile(commandLineArgs)) < 0)   return (false); else argsParsedTotal += argsParsed;
			if ((argsParsed = ParseArgsForRecent(commandLineArgs)) < 0) return (false); else argsParsedTotal += argsParsed;

			if (argsParsedTotal != commandLineArgs.Length)
				return (false);

			return (CheckParsedArgsForConsistency());
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

		// check whether parsed command line args are consistent
		bool CheckParsedArgsForConsistency()
		{
			// nothing to check yet, always return true
			return (true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================