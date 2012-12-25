//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Settings;
using MKY.Windows.Forms;

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

		private const int DefaultWidth = 80;

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
				// Finalize managed resources.

				if (disposing)
				{
					// Dispose of unmanaged resources.
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
		public bool IsDisposed
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

		/// <summary>
		/// This is the main run method for normal operation.
		/// </summary>
		public virtual MainResult RunNormally()
		{
			return (Run(false));
		}

		/// <summary>
		/// This is the main run method for console operation.
		/// </summary>
		public virtual MainResult RunFromConsole()
		{
			return (Run(true));
		}

		private MainResult Run(bool runFromConsole)
		{
			bool showLogo = true;
			bool showView = true;
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
				showView = this.commandLineArgs.ShowView;
				showHelp = this.commandLineArgs.HelpIsRequested;
			}

			// Show help or run application.
			if (showHelp)
			{
				if (showView)
					ShowMessageBoxHelp(showLogo);
				else
					ShowConsoleHelp(showLogo);
			}
			else
			{
				mainResult = Run(runFromConsole, showView);
			}

			return (mainResult);
		}

		/// <summary>
		/// This is the main run method that supports all run options. Do not directly call
		/// this  method for normal or console operation. Call <see cref="RunNormally"/> or
		/// <see cref="RunFromConsole"/> instead. Call this method directly for automated
		/// testing purposes.
		/// </summary>
		/// <remarks>
		/// There are the following use cases to run YAT. This YAT.Controller.Run() method
		/// supports all these use cases as also shown below:
		/// 
		/// 1. 'Normal' GUI operation
		///    > Start YAT from the Windows start menu
		///    > Equal to start YAT.exe directly
		///    ==> Run(false, true);
		/// 
		/// 2. 'File' triggered GUI operation
		///    > Start YAT by executing a .yaw or .yat file
		///    > Uses file type relationship as defined by YAT.Setup
		///    ==> Run(false, true);
		/// 
		/// 3. 'cmd console' operation with GUI
		///    > Must use 'YATConsole' to ensure that output is properly routed back to console
		///    ==> Run(true, true);
		/// 
		/// 4. 'PowerShell' operation with GUI
		///    > Must use 'YATConsole' to ensure that output is properly routed back to PowerShell
		///    ==> Run(true, true);
		/// 
		/// 5. 'cmd console' operation with console only, no GUI at all
		///    > Must use 'YATConsole' with the -NoView/-nv option
		///    ==> Run(true, false);
		/// 
		/// 6. 'PowerShell' operation with console only, no GUI at all
		///    > Must use 'YATConsole' with the -NoView/-nv option
		///    ==> Run(true, false);
		/// 
		/// 7. YAT testing
		///    ==> Run(false, true) or Run(true, true) to test the GUI (e.g. GUI stress test)
		///    ==> Run(false, false) or Run(true, false) to test the behavior (e.g. controller test)
		/// 
		/// </remarks>
		public virtual MainResult Run(bool runFromConsole, bool runWithView)
		{
			MainResult mainResult = MainResult.Success;

			if      (!runFromConsole &&  runWithView)
				RunFullyWithView();                        // 1, 2, 7
			else if ( runFromConsole &&  runWithView)
				RunWithViewButOutputErrorsOnConsole();     // 3, 4, 7
			else if ( runFromConsole && !runWithView)
				RunFullyFromConsole();                     // 5, 6, 7
			else
				RunInvisible();                            // 7

			if (mainResult == MainResult.CommandLineError)
			{
				bool showLogo = this.commandLineArgs.ShowLogo;

				if (runWithView)
					ShowMessageBoxHelp(showLogo);
				else
					ShowConsoleHelp(showLogo);
			}

			return (mainResult);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > RunFullyWithView
		//------------------------------------------------------------------------------------------
		// Private Methods > RunFullyWithView
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private MainResult RunFullyWithView()
		{
			AppDomain curentDomain = AppDomain.CurrentDomain;
			curentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunFullyWithView_curentDomain_UnhandledException);

			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				try
				{
					// Application settings must be created and closed on main thread, otherwise
					// there will be a synchronization exception on exit.
					if (!ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadSharedWriteIfOwned))
						return (MainResult.ApplicationSettingsError);
				
					// ApplicationSettings are loaded while showing the welcome screen, and will
					// be closed when exiting or disposing the main model.
					Gui.Forms.WelcomeScreen welcomeScreen = new Gui.Forms.WelcomeScreen();
					if (welcomeScreen.ShowDialog() != DialogResult.OK)
						return (MainResult.ApplicationSettingsError);
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
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
						Application.ThreadException += new ThreadExceptionEventHandler(RunFullyWithView_Application_ThreadException);

						// Start the Win32 message loop on the current thread and the main form.
						//
						// \attention:
						// This call does not return until the application exits.
						Application.Run(view);

						Application.ThreadException -= new ThreadExceptionEventHandler(RunFullyWithView_Application_ThreadException);

						Model.MainResult result = view.MainResult;
						return (ConvertToMainResult(result));
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while running " + Application.ProductName + ".";
					if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, false, true) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
						Application.Restart();

					return (MainResult.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="Application.ThreadException"/>, it is possible to continue operation.
		/// </remarks>
		private void RunFullyWithView_Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			string message = "An unhandled synchronous exception occurred while running" + Application.ProductName + ".";
			Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(e.Exception, message, false, true);
			
			if      (result == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
				Application.Restart();
			else if (result == Gui.Forms.UnhandledExceptionResult.Exit)
				Application.Exit();
			//// else do nothing.
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit.
		/// </remarks>
		private void RunFullyWithView_curentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			string message = "An unhandled asynchronous exception occurred while running " + Application.ProductName + ".";
			Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, true, false);

			if (result == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
				Application.Restart();
			else
				Application.Exit();
		}

		#endregion

		#region Private Methods > RunWithViewButOutputErrorsOnConsole
		//------------------------------------------------------------------------------------------
		// Private Methods > RunWithViewButOutputErrorsOnConsole
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private MainResult RunWithViewButOutputErrorsOnConsole()
		{
			AppDomain curentDomain = AppDomain.CurrentDomain;
			curentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_curentDomain_UnhandledException);

			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				try
				{
					// Application settings must be created and closed on main thread, otherwise
					// there will be a synchronization exception on exit.
					if (!ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadSharedWriteIfOwned))
						return (MainResult.ApplicationSettingsError);

					// ApplicationSettings are loaded while showing the welcome screen, and will
					// be closed when exiting or disposing the main model.
					Gui.Forms.WelcomeScreen welcomeScreen = new Gui.Forms.WelcomeScreen();
					if (welcomeScreen.ShowDialog() != DialogResult.OK)
						return (MainResult.ApplicationSettingsError);
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
					Console.WriteLine(message);

					if (ex != null)
						MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex);

					return (MainResult.UnhandledException);
				}

				try
				{
					// If everything is fine so far, start main application including view.
					using (Gui.Forms.Main view = new Gui.Forms.Main(model))
					{
						// Assume unhandled exceptions and attach the application to the respective handler.
						Application.ThreadException += new ThreadExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_Application_ThreadException);

						// Start the Win32 message loop on the current thread and the main form.
						//
						// \attention:
						// This call does not return until the application exits.
						Application.Run(view);

						Application.ThreadException -= new ThreadExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_Application_ThreadException);

						Model.MainResult result = view.MainResult;
						return (ConvertToMainResult(result));
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while running " + Application.ProductName + ".";
					Console.WriteLine(message);

					if (ex != null)
						MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex);

					return (MainResult.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="Application.ThreadException"/>, it is possible to continue operation.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			string message = "An unhandled synchronous exception occurred while running" + Application.ProductName + ".";
			Console.WriteLine(message);

			Exception ex = e.Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex);
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_curentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous exception occurred while running " + Application.ProductName + ".";
			Console.WriteLine(message);

			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex);
		}

		#endregion

		#region Private Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------
		// Private Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private MainResult RunFullyFromConsole()
		{
			AppDomain curentDomain = AppDomain.CurrentDomain;
			curentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunFullyFromConsole_curentDomain_UnhandledException);

			// Create model and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				try
				{
					if (ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadShared))
					{
						// Don't care about result,
						//   either settings have been loaded or settings have been set to defaults.
						ApplicationSettings.Load();

						// The ApplicationSettings will be closed when exiting or disposing the main model.
					}
					else
					{
						return (MainResult.ApplicationSettingsError);
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
					Console.WriteLine(message);

					if (ex != null)
						MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex);

					return (MainResult.UnhandledException);
				}

				try
				{
					Model.MainResult modelResult = model.Start();
					if (modelResult == Model.MainResult.Success)
						modelResult = model.Exit();

					return (ConvertToMainResult(modelResult));
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while running " + Application.ProductName + ".";
					Console.WriteLine(message);

					MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex);

					return (MainResult.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit.
		/// </remarks>
		private void RunFullyFromConsole_curentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous exception occurred while running " + Application.ProductName + ".";
			Console.WriteLine(message);

			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex);
		}

		#endregion

		#region Private Methods > RunInvisible
		//------------------------------------------------------------------------------------------
		// Private Methods > RunInvisible
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private MainResult RunInvisible()
		{
			// Create model and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				try
				{
					if (ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadShared))
					{
						// Don't care about result,
						// either settings have been loaded or settings have been set to defaults.
						ApplicationSettings.Load();

						// The ApplicationSettings will be closed when exiting or disposing the main model.
					}
					else
					{
						return (MainResult.ApplicationSettingsError);
					}
				}
				catch
				{
					return (MainResult.UnhandledException);
				}

				try
				{
					Model.MainResult modelResult = model.Start();
					if (modelResult == Model.MainResult.Success)
						modelResult = model.Exit();

					return (ConvertToMainResult(modelResult));
				}
				catch
				{
					return (MainResult.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		#endregion

		#region Private Methods > MessageBox
		//------------------------------------------------------------------------------------------
		// Private Methods > MessageBox
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowMessageBoxHelp(bool showLogo)
		{
			StringBuilder sb = new StringBuilder();

			if (showLogo)
			{
				foreach (string line in Logo)
					sb.AppendLine(line);

				sb.AppendLine();
			}

			sb.Append(this.commandLineArgs.GetHelpText(DefaultWidth));
			sb.AppendLine();

			foreach (string line in Return)
				sb.AppendLine(line);

			MessageBox.Show
				(
				sb.ToString(),
				ApplicationInfo.ProductNameLong,
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning
				);
		}

		#endregion

		#region Private Methods > Console
		//------------------------------------------------------------------------------------------
		// Private Methods > Console
		//------------------------------------------------------------------------------------------

		private void ShowConsoleHelp(bool showLogo)
		{
			if (showLogo)
				WriteLogoToConsole();

			WriteHelpToConsole();
			WriteReturnToConsole();
		}

		/// <remarks>
		/// Output must be limited to <see cref="Console.WindowWidth"/> - 1 to ensure that lines
		/// that exactly match the number of characters do not lead to an empty line (due to the
		/// NewLine which is added).
		/// </remarks>
		private void WriteLogoToConsole()
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine(new string('=', (Console.WindowWidth - 1))); // ==========...
			Console.WriteLine();

			foreach (string line in Logo)
				Console.WriteLine(line);

			Console.WriteLine();
			Console.WriteLine(new string('-', (Console.WindowWidth - 1))); // ----------...
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
			Console.WriteLine(new string('=', (Console.WindowWidth - 1))); // ==========...
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
