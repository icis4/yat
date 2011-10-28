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

using System;
using System.IO;
using System.Windows.Forms;

using MKY;
using MKY.Event;

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
			"Return codes:",
			"",
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

		// Command line options.
		private string[] commandLineArgs;
		private CommandLineArgs commandLineOptions;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Main()
		{
		}

		/// <summary></summary>
		public Main(string[] commandLineArgs)
		{
			this.commandLineArgs = commandLineArgs;
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

		/// <summary></summary>
		public virtual bool CommandLineIsInvalid
		{
			get { return (this.commandLineOptions.IsInvalid); }
		}

		/// <summary></summary>
		public virtual bool CommandLineHelpIsRequested
		{
			get { return (this.commandLineOptions.HelpIsRequested); }
		}

		/// <summary></summary>
		public virtual string RequestedFilePath
		{
			get { return (this.commandLineOptions.RequestedFilePath); }
		}

		/// <summary></summary>
		public virtual int RequestedSequentialTerminalIndex
		{
			get { return (this.commandLineOptions.RequestedSequentialTerminalIndex); }
		}

		/// <summary></summary>
		public virtual string RequestedTransmitFilePath
		{
			get { return (this.commandLineOptions.RequestedTransmitFilePath); }
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

			// Process command line argumens
			this.commandLineOptions = new CommandLineArgs(this.commandLineArgs);
			if (this.commandLineOptions.IsInvalid)
			{
				showHelp = true;
				mainResult = MainResult.CommandLineArgsError;
			}
			else
			{
				showLogo = this.commandLineOptions.ShowLogo;
				showHelp = this.commandLineOptions.HelpIsRequested;
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
			using (Model.Main model = new Model.Main(this.commandLineOptions))
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
			using (Model.Main model = new Model.Main(this.commandLineOptions))
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

		private void WriteLogoToConsole()
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("===============================================================================");
			Console.WriteLine();

			foreach (string line in Logo)
				Console.WriteLine(line);

			Console.WriteLine();
			Console.WriteLine("-------------------------------------------------------------------------------");
			Console.WriteLine();
		}

		private void WriteHelpToConsole()
		{
			Console.Write(this.commandLineOptions.GetHelpText());
		}

		private void WriteReturnToConsole()
		{
			Console.WriteLine();
			foreach (string line in Return)
				Console.WriteLine(line);

			Console.WriteLine();
			Console.WriteLine("===============================================================================");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
