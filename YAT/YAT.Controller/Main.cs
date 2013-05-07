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
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
			"Operate, test and debug serial communications.",
			"Supports RS-232/422/423/485...",
			"...as well as TCP-Client/Server/AutoSocket, UDP and USB Ser/HID",
			"",
			"Visit YAT at http://sourceforge.net/projects/y-a-terminal.",
			"Contact YAT by mailto:y-a-terminal@users.sourceforge.net.",
			"",
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2013 Matthias Kläy.",
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
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "strings", Justification = "Emphasize the difference between the original strings and the resulting object.")]
		public Main(string[] commandLineArgsStrings)
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
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
			get { return ((this.commandLineArgs != null) && (this.commandLineArgs.IsValid)); }
		}

		/// <summary></summary>
		public virtual bool CommandLineHelpIsRequested
		{
			get { return ((this.commandLineArgs != null) && (this.commandLineArgs.HelpIsRequested)); }
		}

		/// <summary></summary>
		public virtual bool CommandLineLogoIsRequested
		{
			get { return ((this.commandLineArgs != null) && (!this.commandLineArgs.NoLogo)); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// This method is used to test the command line argument processing.
		/// </summary>
		public virtual MainResult PrepareRun()
		{
			AssertNotDisposed();

			// Process command line arguments, and evaluate them:
			// 
			// Note that this is the location where the command line arguments are processed and
			// validated in normal operation. In case of automated testing, they will be processed
			// and validated in PrepareRun() below or in YAT.Model.Main. Calling ProcessAndValidate()
			// multiple times doesn't matter, this case is handled with 'ArgsHandler'.
			if (this.commandLineArgs != null)
			{
				if (this.commandLineArgs.ProcessAndValidate())
					return (MainResult.Success);
				else
					return (MainResult.CommandLineError);
			}
			else
			{
				return (MainResult.Success);
			}
		}

		/// <summary>
		/// This is the main run method for normal operation.
		/// </summary>
		public virtual MainResult RunNormally()
		{
			AssertNotDisposed();

			return (Run(false));
		}

		/// <summary>
		/// This is the main run method for console operation.
		/// </summary>
		public virtual MainResult RunFromConsole()
		{
			AssertNotDisposed();

			return (Run(true));
		}

		/// <summary>
		/// This is the main run method.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:SingleLineCommentMustBePrecededByBlankLine", Justification = "Consistent section titles.")]
		private MainResult Run(bool runFromConsole)
		{
			MainResult mainResult;
			bool showLogo;
			bool showView;
			bool showHelp;

			// 
			// Process and validate command line arguments:
			// 
			// In normal operation this is the location where the command line arguments are
			// processed and validated for a first time. Then they will be processed and validated
			// for a second time after to application settings have been created/loaded. This second
			// processing happens in YAT.Model.Main.ProcessCommandLineArgsIntoStartRequests().
			// 
			// In case of automated testing, the command line arguments will be processed and
			// validated in PrepareRun() above, or also in YAT.Model.Main.
			// 
			if (this.commandLineArgs != null)
				this.commandLineArgs.ProcessAndValidate();

			// Prio 0 = None:
			if (this.commandLineArgs == null || this.commandLineArgs.NoArgs)
			{
				mainResult = MainResult.Success;
				showLogo = true;
				showView = true; // By default, start YAT with view.
				showHelp = false;
			}
			// Prio 1 = Invalid:
			else if ((this.commandLineArgs != null) && (!this.commandLineArgs.IsValid))
			{
				mainResult = MainResult.CommandLineError;
				showLogo = true;
				showView = false; // YAT will not be started, instead the help will be shown.
				showHelp = true;
			}
			// Prio 2 = Valid:
			else
			{
				mainResult = MainResult.Success;
				showLogo = this.commandLineArgs.ShowLogo;
				showView = this.commandLineArgs.ShowView;
				showHelp = this.commandLineArgs.HelpIsRequested;
			}

			// Default 'NonInteractive' option depending on execution origin:
			if (!this.commandLineArgs.OptionIsGiven("NonInteractive"))
				this.commandLineArgs.NonInteractive = runFromConsole;

			// Show help or run application:
			if (showHelp)
			{
				if (runFromConsole)
					ShowConsoleHelp(showLogo);
				else
					ShowMessageBoxHelp(showLogo);
			}
			else
			{
				mainResult = Run(runFromConsole, showView);
			}

			return (mainResult);
		}

		/// <summary>
		/// This is the main run method that supports all run options. Do not directly call
		/// this method for normal or console operation. Call <see cref="RunNormally"/> or
		/// <see cref="RunFromConsole"/> instead. Call this method directly for automated
		/// testing purposes only.
		/// </summary>
		/// <remarks>
		/// There are the following use cases to run YAT. This Run() method supports all these
		/// use cases as also shown below:
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
		/// 
		/// Handling of the application settings is related to these use cases.
		/// Handling is done as follows:
		/// 
		/// > In case of 'RunFullyWithView()' the application settings are created/loaded with
		///   <see cref="ApplicationSettingsFileAccess.ReadSharedWriteIfOwned"/> permissions.
		/// 
		/// > In case of 'RunWithViewButOutputErrorsOnConsole()' they are created/loaded with
		///   <see cref="ApplicationSettingsFileAccess.ReadSharedWriteIfOwned"/> permissions.
		/// 
		/// > In case of 'RunFullyFromConsole()' they are created/loaded with
		///   <see cref="ApplicationSettingsFileAccess.ReadShared"/> permissions.
		/// 
		/// > In case of 'RunInvisible()' they are created/loaded with
		///   <see cref="ApplicationSettingsFileAccess.ReadShared"/> permissions.
		/// 
		/// <see cref="ApplicationSettingsFileAccess.ReadSharedWriteIfOwned"/> means that the
		/// instance reads the application settings, but only the owner, i.e. the first instance
		/// that was started, also writes them.
		/// <see cref="ApplicationSettingsFileAccess.ReadShared"/> means that the instance only
		/// reads the application settings, independent on whether it is the first or subsequent
		/// instance.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, StyleCop doesn't seem to be able to deal with command line terms such as 'cmd' or 'nv'...")]
		public virtual MainResult Run(bool runFromConsole, bool runWithView)
		{
			AssertNotDisposed();

			MainResult mainResult;

			if      (!runFromConsole &&  runWithView)
				mainResult = RunFullyWithView();                        // 1, 2, 7
			else if ( runFromConsole &&  runWithView)
				mainResult = RunWithViewButOutputErrorsOnConsole();     // 3, 4, 7
			else if ( runFromConsole && !runWithView)
				mainResult = RunFullyFromConsole();                     // 5, 6, 7
			else
				mainResult = RunInvisible();                            // 7

			if (mainResult == MainResult.CommandLineError)
			{
				if (runWithView)
					ShowMessageBoxHelp(true);
				else
					ShowConsoleHelp(true);
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private MainResult RunFullyWithView()
		{
			AppDomain curentDomain = AppDomain.CurrentDomain;
			curentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunFullyWithView_curentDomain_UnhandledException);

			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				ApplicationEx.EnableVisualStylesAndSetTextRenderingIfNotAlreadyDoneSo();

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
					if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, Gui.Forms.UnhandledExceptionType.Synchronous, true) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
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
					if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, Gui.Forms.UnhandledExceptionType.Synchronous, true) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
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
			string message = "An unhandled asynchronous synchronized exception occurred while running " + Application.ProductName + ".";
			Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(e.Exception, message, Gui.Forms.UnhandledExceptionType.AsynchronousSynchronized, true);
			
			if      (result == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
				Application.Restart();
			else if (result == Gui.Forms.UnhandledExceptionResult.Exit)
				Application.Exit();
			//// else do nothing.
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart.
		/// </remarks>
		private void RunFullyWithView_curentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			string message = "An unhandled asynchronous non-synchronized exception occurred while running " + Application.ProductName + ".";
			Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, Gui.Forms.UnhandledExceptionType.AsynchronousNonSynchronized, false);

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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private MainResult RunWithViewButOutputErrorsOnConsole()
		{
			AppDomain curentDomain = AppDomain.CurrentDomain;
			curentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_curentDomain_UnhandledException);

			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				ApplicationEx.EnableVisualStylesAndSetTextRenderingIfNotAlreadyDoneSo();

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
						MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex); // Message has already been output onto console.

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
						MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex); // Message has already been output onto console.

					return (MainResult.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="Application.ThreadException"/>, it is possible to continue operation.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous synchronized exception occurred while running " + Application.ProductName + ".";
			Console.WriteLine(message);

			Exception ex = e.Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex); // Message has already been output onto console.
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_curentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous non-synchronized exception occurred while running " + Application.ProductName + ".";
			Console.WriteLine(message);

			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex); // Message has already been output onto console.
		}

		#endregion

		#region Private Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------
		// Private Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
						MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex); // Message has already been output onto console.

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

					MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex); // Message has already been output onto console.

					return (MainResult.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart.
		/// </remarks>
		private void RunFullyFromConsole_curentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous non-synchronized exception occurred while running " + Application.ProductName + ".";
			Console.WriteLine(message);

			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex); // Message has already been output onto console.
		}

		#endregion

		#region Private Methods > RunInvisible
		//------------------------------------------------------------------------------------------
		// Private Methods > RunInvisible
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
					MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex, message);

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
					MKY.Diagnostics.ConsoleEx.WriteException(GetType(), ex, message);

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

			MessageBoxEx.Show(sb.ToString(), ApplicationInfo.ProductNameLong, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
		private static void WriteLogoToConsole()
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
		private static void WriteReturnToConsole()
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
		private static MainResult ConvertToMainResult(Model.MainResult result)
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

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
