using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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

		private const string[] _HelpArg =
		{
			"/?",
			"-?",
			"-h",
			"-help"
		};

		private const string[] _CommandLineHelp =
		{
			"Usage: YAT.exe"
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// command line args
		private bool _commandLineHelpIsRequested = false;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Main(string[] commandLineArgs)
		{
			ParseCommandLineArgs(commandLineArgs);

			InitializeComponent();
			Initialize();
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public MainResult Run()
		{
			// show command line help if requested
			if (_commandLineHelpIsRequested)
			{
				foreach (string line in _CommandLineHelp)
					Console.WriteLine(line);

				return (MainResult.OK);
			}

			// create model and view and run application
			using (Model.Main model = new Model.Main())
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

		private void ParseCommandLineArgs(string[] commandLineArgs)
		{
			foreach (string arg in commandLineArgs)
			{
				foreach (string helpArg in _HelpArg)
				{
					if (string.Compare(arg, helpArg, true) == 0)
						_commandLineHelpIsRequested = true;
				}
			}
		}

		#endregion
	}
}
