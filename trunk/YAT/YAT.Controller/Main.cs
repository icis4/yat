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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.IO;
using System.Windows.Forms;

using MKY;
using MKY.Event;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Utilities;

#endregion

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

		private static readonly string[] Logo =
		{
			ApplicationInfo.ProductNameLong + ".",
			"Operate and debug serial communications.",
			"Supports RS-232/422/423/485...",
			"...as well as TCP-Client/Server/AutoSocket, UDP and USB Ser/HID",
			"",
			"Visit YAT at http://sourceforge.net/projects/y-a-terminal.",
			"Contact YAT by mailto:y-a-terminal@users.sourceforge.net.",
			"",
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2011 Matthias Kläy.",
			"All rights reserved.",
			"",
			"YAT is licensed under the GNU LGPL.",
			"See http://www.gnu.org/licenses/lgpl.html for license details.",
			"",
			ApplicationInfo.ProductNameAndBuildNameAndVersion,
		};

		private static readonly string[] Return =
		{
			"Return values:",
			"",
			"   0      Successful exit",
			"  -1      Command line error",
			"  -2      Application settings error",
			"  -3      Application start error",
			"  -4      Application run error",
			"  -5      Application exit error",
			"  -6      Unhandled exception",
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		// Command line options.
		private string[] commandLineArgsStrings;
		private CommandLineArgs commandLineArgs;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Main()
		{
			Initialize(null);
		}

		/// <summary></summary>
		public Main(string[] commandLineArgsStrings)
		{
			Initialize(commandLineArgsStrings);
		}

		private void Initialize(string[] commandLineArgsStrings)
		{
			this.commandLineArgsStrings = commandLineArgsStrings;
			this.commandLineArgs = new CommandLineArgs(this.commandLineArgsStrings);
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

		// Intentionally don't directly return the args strings or object. The requested operation
		// could also be requested by some other means than the command line, e.g. by a config file.

		/// <summary></summary>
		public virtual bool CommandLineIsValid
		{
			get { return (this.commandLineArgs.IsValid); }
		}

		/// <summary></summary>
		public virtual bool CommandLineHelpIsRequested
		{
			get { return (this.commandLineArgs.HelpIsRequested); }
		}

		/// <summary></summary>
		public virtual bool CommandLineLogoIsRequested
		{
			get { return (!this.commandLineArgs.NoLogo); }
		}

		/// <summary></summary>
		public virtual string RequestedFilePath
		{
			get { return (this.commandLineArgs.RequestedFilePath); }
		}

		/// <summary></summary>
		public virtual bool MostRecentIsRequested
		{
			get { return (this.commandLineArgs.MostRecentIsRequested); }
		}

		/// <summary></summary>
		public virtual int RequestedSequentialTerminalIndex
		{
			get { return (this.commandLineArgs.RequestedSequentialTerminalIndex); }
		}

		/// <summary></summary>
		public virtual string RequestedTransmitFilePath
		{
			get { return (this.commandLineArgs.RequestedTransmitFilePath); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual MainResult Run()
		{
			return (Run(true));
		}

		/// <summary></summary>
		public virtual MainResult Run(bool runWithView)
		{
			bool showLogo = true;
			bool showHelp = false;
			MainResult mainResult = MainResult.Success;

			// Check command line arguments.
			if (!this.commandLineArgs.IsValid)
			{
				showHelp = true;
				mainResult = MainResult.CommandLineArgsError;
			}
			else
			{
				showLogo = this.commandLineArgs.ShowLogo;
				showHelp = this.commandLineArgs.HelpIsRequested;
			}

			// Handle command line arguments that result in a command line output.
			if (showHelp)
			{
				MKY.Win32.Console.Attach();

				if (showLogo)
					WriteLogoToConsole();

				WriteHelpToConsole();
				WriteReturnToConsole();

				MKY.Win32.Console.Detach();
				return (mainResult);
			}

			if (runWithView)
				return (RunWithView());
			else
				return (RunWithoutView());
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		/// <summary></summary>
		private MainResult RunWithView()
		{
			MainResult mainResult = MainResult.Success;

			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
#if (!DEBUG)
				try
				{
#endif
					Gui.Forms.WelcomeScreen welcomeScreen = new Gui.Forms.WelcomeScreen();
					if (welcomeScreen.ShowDialog() != DialogResult.OK)
						return (Controller.MainResult.ApplicationSettingsError);
#if (!DEBUG)
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occured while loading " + Application.ProductName + ".";
					if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
						Application.Restart();

					return (MainResult.UnhandledException);
				}
#endif
#if (!DEBUG)
				try
				{
#endif
					// If everything is fine so far, start main application including view.
					using (Gui.Forms.Main view = new Gui.Forms.Main(model))
					{
						// Start the Win32 message loop on the current thread and the main form.
						//
						// \attention:
						// This call does not return until the application exits.
						Application.Run(view);
					}
#if (!DEBUG)
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occured while running " + Application.ProductName + ".";
					if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
						Application.Restart();

					return (MainResult.UnhandledException);
				}
#endif
				return (mainResult);
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		private MainResult RunWithoutView()
		{
			MainResult mainResult = MainResult.Success;

			// Create model and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				MKY.Win32.Console.Attach();
#if (!DEBUG)
				model.UnhandledException += new EventHandler<EventHelper.UnhandledExceptionEventArgs>(model_UnhandledException);
				try
				{
#endif
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
#if (!DEBUG)
				}
				catch (Exception ex)
				{
					MKY.Diagnostics.ConsoleEx.WriteException(this.GetType(), ex);
					mainResult = MainResult.UnhandledException;
				}
				model.UnhandledException -= new EventHandler<EventHelper.UnhandledExceptionEventArgs>(model_UnhandledException);
#endif
				MKY.Win32.Console.Detach();

				return (mainResult);
			} // Dispose of model to ensure immediate release of resources.
		}

#if (!DEBUG)
		private void model_UnhandledException(object sender, EventHelper.UnhandledExceptionEventArgs e)
		{
			MKY.Diagnostics.ConsoleEx.WriteException(this.GetType(), e.UnhandledException);
		}
#endif

		/// <remarks>
		/// Output must be limited to <see cref="Console.WindowWidth"/> - 1 to ensure that lines
		/// that exactly match the number of characters do not lead to an empty line (due to the
		/// NewLine which is added).
		/// </remarks>
		private void WriteLogoToConsole()
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine(new String('=', (Console.WindowWidth - 1))); // ==========...
			Console.WriteLine();

			foreach (string line in Logo)
				Console.WriteLine(line);

			Console.WriteLine();
			Console.WriteLine(new String('-', (Console.WindowWidth - 1))); // ----------...
			Console.WriteLine();
		}

		/// <remarks>
		/// Output must be limited to <see cref="Console.WindowWidth"/> - 1 to ensure that lines
		/// that exactly match the number of characters do not lead to an empty line (due to the
		/// NewLine which is added).
		/// </remarks>
		private void WriteHelpToConsole()
		{
			Console.Write(this.commandLineArgs.GetHelpText(Console.WindowWidth - 1));
		}

		/// <remarks>
		/// Output must be limited to <see cref="Console.WindowWidth"/> - 1 to ensure that lines
		/// that exactly match the number of characters do not lead to an empty line (due to the
		/// NewLine which is added).
		/// </remarks>
		private void WriteReturnToConsole()
		{
			Console.WriteLine();
			Console.WriteLine();
			foreach (string line in Return)
				Console.WriteLine(line);

			Console.WriteLine();
			Console.WriteLine(new String('=', (Console.WindowWidth - 1))); // ==========...
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
