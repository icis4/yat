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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
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
			"Copyright © 2003-2012 Matthias Kläy.",
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
				mainResult = MainResult.CommandLineError;
				showHelp = true;
			}
			else
			{
				showLogo = this.commandLineArgs.ShowLogo;
				showHelp = this.commandLineArgs.HelpIsRequested;
			}

			// Run application.
			if (!showHelp)
			{
				if (runWithView)
					mainResult = RunWithView();
				else
					mainResult = RunWithoutView();

				if (mainResult == MainResult.CommandLineError)
					showHelp = true;
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
			}

			return (mainResult);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > Run With View
		//------------------------------------------------------------------------------------------
		// Private Methods > Run With View
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private MainResult RunWithView()
		{
			MainResult mainResult = MainResult.Success;

			AppDomain curentDomainWithView = AppDomain.CurrentDomain;
			curentDomainWithView.UnhandledException += new UnhandledExceptionEventHandler(curentDomainWithView_UnhandledException);

			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				try
				{
					Gui.Forms.WelcomeScreen welcomeScreen = new Gui.Forms.WelcomeScreen();
					if (welcomeScreen.ShowDialog() != DialogResult.OK)
						return (Controller.MainResult.ApplicationSettingsError);
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occured while preparing " + Application.ProductName + ".";
					if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, false, true) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
						Application.Restart();

					return (MainResult.UnhandledException);
				}

				try
				{
					// If everything is fine so far, start main application including view.
					using (Gui.Forms.Main view = new Gui.Forms.Main(model))
					{
						// Assume unhandled exceptions and attach the application to the respective handler.
						mainResult = MainResult.UnhandledException;
						Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

						// Start the Win32 message loop on the current thread and the main form.
						//
						// \attention:
						// This call does not return until the application exits.
						Application.Run(view);

						Application.ThreadException -= new ThreadExceptionEventHandler(Application_ThreadException);

						Model.MainResult result = view.MainResult;
						mainResult = ConvertToMainResult(result);
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occured while loading " + Application.ProductName + ".";
					if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, false, true) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
						Application.Restart();

					return (MainResult.UnhandledException);
				}

				return (mainResult);
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="Application.ThreadException"/>, it is possible to continue operation.
		/// </remarks>
		private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			string message = "An unhandled synchronous exception occured while running" + Application.ProductName + ".";
			Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(e.Exception, message, false, true);
			
			if      (result == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
				Application.Restart();
			else if (result == Gui.Forms.UnhandledExceptionResult.Exit)
				Application.Exit();
			// else do nothing.
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit.
		/// </remarks>
		private void curentDomainWithView_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			string message = "An unhandled asynchronous exception occured in " + Application.ProductName + ".";
			Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, true, false);

			if (result == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
				Application.Restart();
			else
				Application.Exit();
		}

		#endregion

		#region Private Methods > Run Without View
		//------------------------------------------------------------------------------------------
		// Private Methods > Run Without View
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private MainResult RunWithoutView()
		{
			MainResult mainResult = MainResult.Success;

			MKY.Win32.Console.Attach();

			AppDomain curentDomainWithoutView = AppDomain.CurrentDomain;
			curentDomainWithoutView.UnhandledException += new UnhandledExceptionEventHandler(curentDomainWithoutView_UnhandledException);

			// Create model and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				try
				{
					Model.MainResult modelResult = model.Start();
					if (modelResult == Model.MainResult.Success)
					{
						modelResult = model.Exit();
						mainResult = ConvertToMainResult(modelResult);
					}
					else
					{
						mainResult = ConvertToMainResult(modelResult);
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occured while running " + Application.ProductName + ".";
					Console.WriteLine(message);

					MKY.Diagnostics.ConsoleEx.WriteException(this.GetType(), ex);

					mainResult = MainResult.UnhandledException;
				}
			} // Dispose of model to ensure immediate release of resources.

			MKY.Win32.Console.Detach();

			return (mainResult);
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit.
		/// </remarks>
		private void curentDomainWithoutView_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous exception occured in " + Application.ProductName + ".";
			Console.WriteLine(message);

			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.WriteException(this.GetType(), ex);
		}

		#endregion

		#region Private Methods > Console
		//------------------------------------------------------------------------------------------
		// Private Methods > Console
		//------------------------------------------------------------------------------------------

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
			foreach (string line in Return)
				Console.WriteLine(line);

			Console.WriteLine();
			Console.WriteLine(new String('=', (Console.WindowWidth - 1))); // ==========...
		}

		#endregion

		#region Private Methods > Result
		//------------------------------------------------------------------------------------------
		// Private Methods > Result
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		private MainResult ConvertToMainResult(Model.MainResult result)
		{
			switch (result)
			{
				case Model.MainResult.Success:               return (MainResult.Success);
				case Model.MainResult.CommandLineError:      return (MainResult.CommandLineError);
				case Model.MainResult.ApplicationStartError: return (MainResult.ApplicationStartError);
				case Model.MainResult.ApplicationRunError:   return (MainResult.ApplicationRunError);
				case Model.MainResult.ApplicationExitError:  return (MainResult.ApplicationExitError);
				default:                                     return (MainResult.UnhandledException); // Covers 'Model.MainResult.UnhandledException'.
			}
		}

		/// <summary></summary>
		private Model.MainResult ConvertToMainResult(MainResult result)
		{
			switch (result)
			{
				case MainResult.Success:               return (Model.MainResult.Success);
				case MainResult.CommandLineError:      return (Model.MainResult.CommandLineError);
				case MainResult.ApplicationStartError: return (Model.MainResult.ApplicationStartError);
				case MainResult.ApplicationRunError:   return (Model.MainResult.ApplicationRunError);
				case MainResult.ApplicationExitError:  return (Model.MainResult.ApplicationExitError);
				default:                               return (Model.MainResult.UnhandledException); // Covers 'MainResult.UnhandledException'.
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
