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
// YAT 2.0 Gamma 1'' Version 1.99.34
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
using System.Diagnostics;
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
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
		// warnings for each undocumented member below. Documenting each member makes little sense
		// since they pretty much tell their purpose and documentation tags between the members
		// makes the code less readable.
		#pragma warning disable 1591

		/// <summary>
		/// Enumeration of all the result return codes.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Intentionally using nested type, instead of replicating the parent's name to 'MainResult'.")]
		public enum Result
		{
			Success                  =  0,
			CommandLineError         = -1,
			ApplicationSettingsError = -2,
			ApplicationStartError    = -3,
			ApplicationRunError      = -4,
			ApplicationExitError     = -5,
			UnhandledException       = -6,
		}

		#pragma warning restore 1591

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int DefaultTextWidth = 80;

		private static readonly string[] ResultText =
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

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
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
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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
		public virtual Result PrepareRun()
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
					return (Result.Success);
				else
					return (Result.CommandLineError);
			}
			else
			{
				return (Result.Success);
			}
		}

		/// <summary>
		/// This is the main run method for normal operation.
		/// </summary>
		public virtual Result Run()
		{
			AssertNotDisposed();

			return (Run(false));
		}

		/// <summary>
		/// This is the main run method for console operation.
		/// </summary>
		public virtual Result RunFromConsole()
		{
			AssertNotDisposed();

			return (Run(true));
		}

		/// <summary>
		/// This is the main run method.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:SingleLineCommentMustBePrecededByBlankLine", Justification = "Consistent section titles.")]
		private Result Run(bool runFromConsole)
		{
			Result result;
			bool showLogo;
			bool showView;
			bool showHelp;

			// Process and validate command line arguments:
			if (this.commandLineArgs != null)
				this.commandLineArgs.ProcessAndValidate();

			// In normal operation this is the location where the command line arguments are
			// processed and validated for a first time, even BEFORE the application settings have
			// been created/loaded. Then they will be processed and validated for a second time
			// AFTER the application settings were created/loaded. This second processing happens
			// in YAT.Model.Main.ProcessCommandLineArgsIntoStartRequests().
			//
			// In case of automated testing, the command line arguments will be processed and
			// validated in PrepareRun() above, or also in YAT.Model.Main.

			// Prio 0 = None:
			if (this.commandLineArgs == null || this.commandLineArgs.NoArgs)
			{
				result = Result.Success;
				showLogo = true;
				showView = true; // By default, start YAT with view.
				showHelp = false;
			}
			// Prio 1 = Invalid:
			else if ((this.commandLineArgs != null) && (!this.commandLineArgs.IsValid))
			{
				result = Result.CommandLineError;
				showLogo = true;
				showView = false; // YAT will not be started, instead the help will be shown.
				showHelp = true;
			}
			// Prio 2 = Valid:
			else
			{
				result = Result.Success;
				showLogo = this.commandLineArgs.ShowLogo;
				showView = this.commandLineArgs.ShowView;
				showHelp = this.commandLineArgs.HelpIsRequested;
			}

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
				result = Run(runFromConsole, showView);
			}

			return (result);
		}

		/// <summary>
		/// This is the main run method that supports all run options.
		/// </summary>
		/// <remarks>
		/// Do not directly call this method for normal or console operation. Call <see cref="Run()"/>
		/// or <see cref="RunFromConsole"/> instead. Call this method directly for automated testing
		/// purposes only.
		/// 
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
		public virtual Result Run(bool runFromConsole, bool runWithView)
		{
			AssertNotDisposed();

			Result result;

			if (!runFromConsole && runWithView)
			{
				result = RunFullyWithView();                        // 1, 2, 7
			}
			else
			{
				// Default 'NonInteractive' in case of console or invisible execution:
				this.commandLineArgs.Override("NonInteractive", true);

				if (     runFromConsole && runWithView)
					result = RunWithViewButOutputErrorsOnConsole(); // 3, 4, 7
				else if (runFromConsole && !runWithView)
					result = RunFullyFromConsole();                 // 5, 6, 7
				else
					result = RunInvisible();                        //       7
			}

			if (result == Result.CommandLineError)
			{
				if (runWithView)
					ShowMessageBoxHelp(true);
				else
					ShowConsoleHelp(true);
			}

			return (result);
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
		private Result RunFullyWithView()
		{
		#if (!DEBUG) // Only handle exceptions in case of 'Release', otherwise let them by handled by the debugger.
			// Assume unhandled asynchronous non-synchronized exceptions and attach the application to the respective handler.
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunFullyWithView_currentDomain_UnhandledException);
		#endif
			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				ApplicationEx.EnableVisualStylesAndSetTextRenderingIfNotAlreadyDoneSo();

				try
				{
					// Application settings must be created and closed on main thread, otherwise
					// there will be a synchronization exception on exit.
					if (!ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadSharedWriteIfOwned))
						return (Result.ApplicationSettingsError);
				
					// ApplicationSettings are loaded while showing the welcome screen, and will
					// be closed when exiting or disposing the main model.
					Gui.Forms.WelcomeScreen welcomeScreen = new Gui.Forms.WelcomeScreen();
					if (welcomeScreen.ShowDialog() != DialogResult.OK)
						return (Result.ApplicationSettingsError);
				}
				catch (Exception ex)
				{
					if (this.commandLineArgs.Interactive)
					{
						string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
						if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, Gui.Forms.UnhandledExceptionType.Synchronous, true) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
							Application.Restart();
					}
					return (Result.UnhandledException);
				}

				try
				{
					// If everything is fine so far, start main application including view.
					using (Gui.Forms.Main view = new Gui.Forms.Main(model))
					{
						// Assume unhandled asynchronous synchronized exceptions and attach the application to the respective handler.
						Application.ThreadException += new ThreadExceptionEventHandler(RunFullyWithView_Application_ThreadException);

						// Start the Win32 message loop on the current thread and the main form.
						//
						// \attention:
						// This call does not return until the application exits.
						Application.Run(view);

						Application.ThreadException -= new ThreadExceptionEventHandler(RunFullyWithView_Application_ThreadException);

						Model.Main.Result viewResult = view.Result;
						return (ConvertToMainResult(viewResult));
					}
				}
				catch (Exception ex)
				{
					if (this.commandLineArgs.Interactive)
					{
						string message = "An unhandled synchronous exception occurred while running " + Application.ProductName + ".";
						if (Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, Gui.Forms.UnhandledExceptionType.Synchronous, true) == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
							Application.Restart();
					}
					return (Result.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="Application.ThreadException"/>, it is possible to continue operation.
		/// </remarks>
		private void RunFullyWithView_Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			if (this.commandLineArgs.Interactive)
			{
				string message = "An unhandled asynchronous synchronized exception occurred while running " + Application.ProductName + ".";
				Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(e.Exception, message, Gui.Forms.UnhandledExceptionType.AsynchronousSynchronized, true);

				if (result == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
					Application.Restart();
				else if (result == Gui.Forms.UnhandledExceptionResult.Continue)
					return; // Ignore, do nothing.
				else
					Application.Exit();
			}
			else
			{
				Application.Exit();
			}
		}

#if (!DEBUG) // Only handle exceptions in case of 'Release', otherwise let them by handled by the debugger.
		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart.
		/// </remarks>
		private void RunFullyWithView_currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (this.commandLineArgs.Interactive)
			{
				Exception ex = e.ExceptionObject as Exception;
				string message = "An unhandled asynchronous non-synchronized exception occurred while running " + Application.ProductName + ".";
				Gui.Forms.UnhandledExceptionResult result = Gui.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, Gui.Forms.UnhandledExceptionType.AsynchronousNonSynchronized, false);

				if (result == Gui.Forms.UnhandledExceptionResult.ExitAndRestart)
					Application.Restart();
				else if (result == Gui.Forms.UnhandledExceptionResult.Continue)
					return; // Ignore, do nothing.
				else
					Application.Exit();
			}
			else
			{
				Application.Exit();
			}
		}
#endif // !DEBUG

		#endregion

		#region Private Methods > RunWithViewButOutputErrorsOnConsole
		//------------------------------------------------------------------------------------------
		// Private Methods > RunWithViewButOutputErrorsOnConsole
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private Result RunWithViewButOutputErrorsOnConsole()
		{
		#if (!DEBUG) // Only handle exceptions in case of 'Release', otherwise let them by handled by the debugger.
			// Assume unhandled asynchronous non-synchronized exceptions and attach the application to the respective handler.
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_currentDomain_UnhandledException);
		#endif
			// Create model and view and run application.
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				ApplicationEx.EnableVisualStylesAndSetTextRenderingIfNotAlreadyDoneSo();

				try
				{
					// Application settings must be created and closed on main thread, otherwise
					// there will be a synchronization exception on exit.
					if (!ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadSharedWriteIfOwned))
						return (Result.ApplicationSettingsError);

					// ApplicationSettings are loaded while showing the welcome screen, and will
					// be closed when exiting or disposing the main model.
					Gui.Forms.WelcomeScreen welcomeScreen = new Gui.Forms.WelcomeScreen();
					if (welcomeScreen.ShowDialog() != DialogResult.OK)
						return (Result.ApplicationSettingsError);
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
					Console.Error.WriteLine(message);

					if (ex != null)
						MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (Result.UnhandledException);
				}

				try
				{
					// If everything is fine so far, start main application including view.
					using (Gui.Forms.Main view = new Gui.Forms.Main(model))
					{
						// Assume unhandled asynchronous synchronized exceptions and attach the application to the respective handler.
						Application.ThreadException += new ThreadExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_Application_ThreadException);

						// Start the Win32 message loop on the current thread and the main form.
						//
						// \attention:
						// This call does not return until the application exits.
						Application.Run(view);

						Application.ThreadException -= new ThreadExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_Application_ThreadException);

						Model.Main.Result viewResult = view.Result;
						return (ConvertToMainResult(viewResult));
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while running " + Application.ProductName + ".";
					Console.Error.WriteLine(message);

					if (ex != null)
						MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (Result.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

		/// <remarks>
		/// In case of an <see cref="Application.ThreadException"/>, it is possible to continue operation.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous synchronized exception occurred while running " + Application.ProductName + ".";
			Console.Error.WriteLine(message);

			Exception ex = e.Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.
		}

#if (!DEBUG) // Only handle exceptions in case of 'Release', otherwise let them by handled by the debugger.
		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous non-synchronized exception occurred while running " + Application.ProductName + ".";
			Console.Error.WriteLine(message);

			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.
		}
#endif // !DEBUG

		#endregion

		#region Private Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------
		// Private Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private Result RunFullyFromConsole()
		{
		#if (!DEBUG) // Only handle exceptions in case of 'Release', otherwise let them by handled by the debugger.
			// Assume unhandled asynchronous non-synchronized exceptions and attach the application to the respective handler.
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += new UnhandledExceptionEventHandler(RunFullyFromConsole_currentDomain_UnhandledException);
		#endif
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
						return (Result.ApplicationSettingsError);
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
					Console.Error.WriteLine(message);

					if (ex != null)
						MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (Result.UnhandledException);
				}

				try
				{
					Model.Main.Result modelResult = model.Start();
					if (modelResult == Model.Main.Result.Success)
						modelResult = model.Exit();

					return (ConvertToMainResult(modelResult));
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while running " + Application.ProductName + ".";
					Console.Error.WriteLine(message);

					MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (Result.UnhandledException);
				}
			} // Dispose of model to ensure immediate release of resources.
		}

#if (!DEBUG) // Only handle exceptions in case of 'Release', otherwise let them by handled by the debugger.
		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart.
		/// </remarks>
		private void RunFullyFromConsole_currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = "An unhandled asynchronous non-synchronized exception occurred while running " + Application.ProductName + ".";
			Console.Error.WriteLine(message);

			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.
		}
#endif // !DEBUG

		#endregion

		#region Private Methods > RunInvisible
		//------------------------------------------------------------------------------------------
		// Private Methods > RunInvisible
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private Result RunInvisible()
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
						return (Result.ApplicationSettingsError);
					}
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while preparing " + Application.ProductName + ".";
					MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex, message);

					return (Result.UnhandledException);
				}

				try
				{
					Model.Main.Result modelResult = model.Start();
					if (modelResult == Model.Main.Result.Success)
						modelResult = model.Exit();

					return (ConvertToMainResult(modelResult));
				}
				catch (Exception ex)
				{
					string message = "An unhandled synchronous exception occurred while running " + Application.ProductName + ".";
					MKY.Diagnostics.ConsoleEx.Error.WriteException(GetType(), ex, message);

					return (Result.UnhandledException);
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
				foreach (string line in ApplicationInfo.ProductLogo)
					sb.AppendLine(line);

				sb.AppendLine();
			}

			sb.Append(this.commandLineArgs.GetHelpText(DefaultTextWidth));
			sb.AppendLine();

			foreach (string line in ResultText)
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
			Console.Out.WriteLine();
			Console.Out.WriteLine();
			Console.Out.WriteLine(new string('=', (Console.WindowWidth - 1))); // ==========...
			Console.Out.WriteLine();

			Console.Out.WriteLine(ApplicationInfo.ProductNameAndBuildNameAndVersion);

			Console.Out.WriteLine();
			Console.Out.WriteLine(new string('-', (Console.WindowWidth - 1))); // ----------...
			Console.Out.WriteLine();

			foreach (string line in ApplicationInfo.ProductLogo)
				Console.Out.WriteLine(line);

			Console.Out.WriteLine();
			Console.Out.WriteLine(new string('-', (Console.WindowWidth - 1))); // ----------...
			Console.Out.WriteLine();
		}

		/// <remarks>
		/// Output must be limited to <see cref="Console.WindowWidth"/> - 1 to ensure that lines
		/// that exactly match the number of characters do not lead to an empty line (due to the
		/// NewLine which is added).
		/// </remarks>
		private void WriteHelpToConsole()
		{
			Console.Out.Write(this.commandLineArgs.GetHelpText(Console.WindowWidth - 1));
		}

		/// <remarks>
		/// Output must be limited to <see cref="Console.WindowWidth"/> - 1 to ensure that lines
		/// that exactly match the number of characters do not lead to an empty line (due to the
		/// NewLine which is added).
		/// </remarks>
		private static void WriteReturnToConsole()
		{
			Console.Out.WriteLine();

			foreach (string line in ResultText)
				Console.Out.WriteLine(line);

			Console.Out.WriteLine();
			Console.Out.WriteLine(new string('=', (Console.WindowWidth - 1))); // ==========...
		}

		#endregion

		#region Private Methods > Result
		//------------------------------------------------------------------------------------------
		// Private Methods > Result
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		private static Result ConvertToMainResult(Model.Main.Result result)
		{
			switch (result)
			{
				case Model.Main.Result.Success:               return (Result.Success);
				case Model.Main.Result.CommandLineError:      return (Result.CommandLineError);
				case Model.Main.Result.ApplicationStartError: return (Result.ApplicationStartError);
				case Model.Main.Result.ApplicationRunError:   return (Result.ApplicationRunError);
				case Model.Main.Result.ApplicationExitError:  return (Result.ApplicationExitError);
				default:                                      return (Result.UnhandledException); // Covers 'Model.Main.Result.UnhandledException'.
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
