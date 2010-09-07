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
using System.IO;
using System.Windows.Forms;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Utilities;

namespace YAT.Controller
{
	/// <summary>
	/// Application controller main class of YAT.
	/// </summary>
	/// <remarks>
	/// This class is separated into its own project for those who want to use YAT components within
	/// their own context.
	/// </remarks>
	public class Main : IDisposable
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly string[] Title =
		{
			ApplicationInfo.ProductName + " - Version " + Application.ProductVersion,
			"YAT - Yet Another Terminal.",
			"Operate and debug serial communications.",
			"Supports RS-232/422/423/485 as well as TCP-Client/Server/AutoSocket, UDP and USB Ser/HID",
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2010 Matthias Kläy.",
		};

		private static readonly string[] FileOptions =
		{
			"Usage:         ",
			"  YAT[.exe] [<WorkspaceSettings>.yaw|<TerminalSettings>.yat]",
			"               ",
			"Usage examples:",
			"  YAT MyWorkspace.yaw",
			"          Start YAT and open given workspace settings.",
			"  YAT MyTerminal.yat",
			"          Start YAT and open given terminal settings.",
		};

		private static readonly string[] AdvancedOptions =
		{
			"Advanced usage:",
			"  YAT[.exe] [/r] [/t[n] file]",
			"               ",
			"  /r           ",
			"  -r           Open most recent file according to file list.",
			"               ",
			"  /t[n] file   ",
			"  -t[n] file   Automatically transmit the given file on terminal n.",
			"               ",
			"Advanced usage examples:",
			"  YAT /r       Start YAT and open most recent file.",
		};

		private static readonly string[] RecentArg =
		{
			"/r",
			"-r",
		};

		private static readonly string[] TransmitArg =
		{
			"/t",
			"-t",
		};

		private static readonly string[] Help =
		{
			"  /?      ",
			"  -?      ",
			"  -h      ",
			"  --help  Display this help text",
		};

		private static readonly string[] HelpArg =
		{
			"/?",
			"-?",
			"-h",
			"--help",
		};

		private static readonly string[] Return =
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

		private bool isDisposed;

		// Command line.
		private bool commandLineError;
		private bool commandLineHelpIsRequested;

		// Command line options.
		private Model.CommandLineOptions commandLineOptions = new Model.CommandLineOptions();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Main()
		{
		}

		public Main(string[] commandLineArgs)
		{
			// Parse command line args if there are any.
			if (commandLineArgs.Length > 0)
				this.commandLineError = (!ParseCommandLineArgs(commandLineArgs));
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
			if (!this.isDisposed)
			{
				if (disposing)
				{
					// Nothing to do yet.
				}

				this.isDisposed = true;
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
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public virtual bool CommandLineError
		{
			get { return (this.commandLineError); }
		}

		public virtual bool CommandLineHelpIsRequested
		{
			get { return (this.commandLineHelpIsRequested); }
		}

		public virtual string RequestedFilePath
		{
			get { return (this.commandLineOptions.RequestedFilePath); }
		}

		public virtual int RequestedTerminalId
		{
			get { return (this.commandLineOptions.RequestedTerminalId); }
		}

		public virtual string RequestedTransmitFilePath
		{
			get { return (this.commandLineOptions.RequestedTransmitFilePath); }
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
			// Show command line help in case of error.
			if (this.commandLineError)
			{
				WriteHelp();
				return (MainResult.CommandLineArgsError);
			}

			// Show command line help if requested.
			if (this.commandLineHelpIsRequested)
			{
				WriteHelp();
				return (MainResult.Success);
			}

			// Create model and view and run application.
			MainResult mainResult;
			using (Model.Main model = new Model.Main(this.commandLineOptions))
			{
				if (runWithView)
				{
					using (Gui.Forms.Main view = new Gui.Forms.Main(model))
					{
						// Start the Win32 message loop on the current thread and the main form.
						// \attention This call does not return until the application exits.
						Application.Run(view);
					}

					mainResult = MainResult.Success;
				}
				else // Non-view application for automated test usage.
				{
					if (model.Start())
					{
						if (model.Exit())
							mainResult = MainResult.Success;
						else
							mainResult = MainResult.ApplicationExitError;
					}
					else
					{
						mainResult = MainResult.ApplicationStartError;
					}
				}
			} // Dispose of model and view to ensure immediate release of resources.

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

			if ((argsParsed = ParseArgsForHelp(commandLineArgs)) < 0)     return (false); else argsParsedTotal += argsParsed;
			if ((argsParsed = ParseArgsForFile(commandLineArgs)) < 0)     return (false); else argsParsedTotal += argsParsed;
			if ((argsParsed = ParseArgsForRecent(commandLineArgs)) < 0)   return (false); else argsParsedTotal += argsParsed;
			if ((argsParsed = ParseArgsForTransmit(commandLineArgs)) < 0) return (false); else argsParsedTotal += argsParsed;

			if (argsParsedTotal != commandLineArgs.Length)
				return (false);

			return (CheckParsedArgsForConsistency());
		}

		// Write help text onto console.
		private static void WriteHelp()
		{
			foreach (string line in Title)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in FileOptions)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in AdvancedOptions)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in Help)
				Console.WriteLine(line);
			Console.WriteLine();

			foreach (string line in Return)
				Console.WriteLine(line);
			Console.WriteLine();
		}

		// Parse args for help.
		private int ParseArgsForHelp(string[] commandLineArgs)
		{
			int argsParsed = 0;
			foreach (string arg in commandLineArgs)
			{
				// Check for help args.
				foreach (string helpArg in HelpArg)
				{
					if (string.Compare(arg, helpArg, StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.commandLineHelpIsRequested = true;
						argsParsed++;
					}
				}
			}
			return (argsParsed);
		}

		// Parse args for file.
		private int ParseArgsForFile(string[] commandLineArgs)
		{
			int argsParsed = 0;
			foreach (string arg in commandLineArgs)
			{
				// Check for file args.
				string filePath = arg.Trim('"');
				if (File.Exists(filePath))
				{
					// Check for workspace file args.
					if (ExtensionSettings.IsWorkspaceFile(Path.GetExtension(filePath)))
					{
						// Trim optional quotes around file path.
						this.commandLineOptions.RequestedFilePath = filePath;
						argsParsed++;
					}

					// Check for terminal file args.
					if (ExtensionSettings.IsTerminalFile(Path.GetExtension(filePath)))
					{
						// Trim optional quotes around file path.
						this.commandLineOptions.RequestedFilePath = filePath;
						argsParsed++;
					}
				}
			}
			return (argsParsed);
		}

		// Parse args for recent.
		private int ParseArgsForRecent(string[] commandLineArgs)
		{
			int argsParsed = 0;
			foreach (string arg in commandLineArgs)
			{
				foreach (string recentArg in RecentArg)
				{
					if (string.Compare(arg, recentArg, StringComparison.OrdinalIgnoreCase) == 0)
					{
						ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();
						bool recentsReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
						if (recentsReady)
						{
							this.commandLineOptions.RequestedFilePath = ApplicationSettings.LocalUser.RecentFiles.FilePaths[0].Item;
							argsParsed++;
						}
					}
				}
			}
			return (argsParsed);
		}

		// Parse args for transmit.
		private int ParseArgsForTransmit(string[] commandLineArgs)
		{
			int argsParsed = 0;
			foreach (string arg in commandLineArgs)
			{
				foreach (string transmitArg in TransmitArg)
				{
					if ((arg.Length >= 2) && (string.Compare(arg.Substring(0, 2), transmitArg, StringComparison.OrdinalIgnoreCase) == 0))
					{
						int terminalId;
						if ((arg.Length) >= 3 && (int.TryParse(arg.Substring(2, 1), out terminalId)))
							this.commandLineOptions.RequestedTerminalId = terminalId;

						string[] subargs = arg.Split(' ');
						if (subargs.Length >= 2)
						{
							// Trim optional quotes around file path.
							string filePath = subargs[1].Trim('"');
							if (File.Exists(filePath))
								this.commandLineOptions.RequestedTransmitFilePath = filePath;
						}

						argsParsed++;
					}
				}
			}
			return (argsParsed);
		}

		// Check whether parsed command line args are consistent.
		private static bool CheckParsedArgsForConsistency()
		{
			// Nothing to check yet, always return <c>true</c>.
			return (true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
