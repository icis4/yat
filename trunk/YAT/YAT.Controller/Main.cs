//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable unhandled exception handling:
////#define HANDLE_UNHANDLED_EXCEPTIONS // Disabled for 'Debug' => handled by the debugger.

#else // RELEASE

	// Enable unhandled exception handling:
	#define HANDLE_UNHANDLED_EXCEPTIONS // Enabled for 'Release' => handled by the application.

#endif // DEBUG|RELEASE

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
#if (HANDLE_UNHANDLED_EXCEPTIONS)
using System.ComponentModel;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Text;
#if (HANDLE_UNHANDLED_EXCEPTIONS)
using System.Threading;
#endif
using System.Windows.Forms; // Note that several locations explicitly use 'System.Windows.Forms' to prevent naming conflicts with 'MKY.Windows.Forms' and 'YAT.Application'.

using MKY;
using MKY.Diagnostics;
using MKY.Settings;
using MKY.Threading;
using MKY.Windows.Forms; // Note that several locations explicitly use 'MKY.Windows.Forms' to prevent naming conflicts with 'System.Windows.Forms' and 'YAT.Application'.

using YAT.Settings.Application;
//// 'YAT.View.Forms' is explicitly used to prevent naming conflicts with same-named 'YAT.Controller' classes like 'Main'.

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
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	public class Main : IDisposable
	{
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

		#if (HANDLE_UNHANDLED_EXCEPTIONS)
		private static readonly string ObjectDisposedExceptionInMscorlibMessage = 
			"Such 'ObjectDisposedException' in 'mscorlib' is an exeption that YAT is aware " +
			"of but cannot properly handle. It can happen when a serial COM port gets physically " +
			"disconnected while it is open. It happens due to a bug in the .NET 'SerialPort' " +
			"class for which Microsoft seems to have no plans fixing. The issue is known for " +
			"internal ports using the Microsoft serial COM port driver, external USB/COM ports " +
			"using the Microsoft USB CDC/ACM (virtual serial port) driver as well as Microchip " +
			"MCP2221 USB-to-UART/I2C bridges. The issue is referred to by dozens of online blogs " +
			"and articles. YAT is applying several patches to try working around the issue, but " +
			"apparently all of them have failed in the current situation." +
			Environment.NewLine + Environment.NewLine +
			"To prevent this issue, refrain from disconnecting a device while its port is open. " +
			"Or, manually close the port after the device got disconnected.";
		#endif

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Command line options:
		private string[] commandLineArgsStrings;
		private CommandLineArgs commandLineArgs;

	#if (HANDLE_UNHANDLED_EXCEPTIONS)
		// Invocation synchronization objects:
		private ISynchronizeInvoke mainThreadSynchronizer;
		private object mainThreadSynchronizerSyncObj = new object();
	#endif

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
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				IsDisposed = true;
			}
		}

	#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Main()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
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
		public virtual MainResult Run()
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
			MainResult result;
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
				result = MainResult.Success;
				showLogo = true;
				showView = true; // By default, start YAT with view.
				showHelp = false;
			}
			// Prio 1 = Invalid:
			else if ((this.commandLineArgs != null) && (!this.commandLineArgs.IsValid))
			{
				result = MainResult.CommandLineError;
				showLogo = true;
				showView = false; // YAT will not be started, instead the help will be shown.
				showHelp = true;
			}
			// Prio 2 = Valid:
			else
			{
				result = MainResult.Success;
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
		/// use cases as shown below:
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, StyleCop isn't able to deal with command line terms such as 'cmd' or 'nv'...")]
		public virtual MainResult Run(bool runFromConsole, bool runWithView)
		{
			AssertNotDisposed();

			MainThreadHelper.SetCurrentThread();

			MainResult result;

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

			if (result == MainResult.CommandLineError)
			{
				if (runWithView)
					ShowMessageBoxHelp(true);
				else
					ShowConsoleHelp(true);
			}

			return (result);
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Non-Public Methods > RunFullyWithView
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > RunFullyWithView
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Exceptions are only handled in case of 'Release', otherwise by the debugger.
		/// </remarks>
	#if (HANDLE_UNHANDLED_EXCEPTIONS)
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
	#endif
		private MainResult RunFullyWithView()
		{
			MessageHelper.RequestSupport =      "Support may be requested as described in 'Help > Request Support'.";
			MessageHelper.RequestFeature = "New features can be requested as described in 'Help > Request Feature'.";
			MessageHelper.RequestChange  = "Changes can also be requested as described in 'Help > Request Feature'.";
			MessageHelper.SubmitBug      =      "Please report this issue as described in 'Help > Submit Bug'.";
		#if (HANDLE_UNHANDLED_EXCEPTIONS)
			// Assume unhandled asynchronous non-synchronized exceptions and attach the application to the respective handler:
			AppDomain.CurrentDomain.UnhandledException += RunFullyWithView_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread;

			// Assume unhandled asynchronous non-synchronized exceptions during events and attach the application to the same handler:
			EventHelper.UnhandledExceptionOnNonMainThread += RunFullyWithView_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread;
		////EventHelper.UnhandledExceptionOnMainThread is not used as it is handled by the catch-all handler below.
		#endif
			// Create model and view and run application:
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				MKY.Windows.Forms.ApplicationEx.EnableVisualStylesAndSetTextRenderingIfNotInitializedYet();
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				try
			#endif
				{
					// Application settings must be created and closed on main thread, otherwise
					// there will be a synchronization exception on exit (settings are closed there):
					if (!ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadSharedWriteIfOwned))
						return (MainResult.ApplicationSettingsError);

					// Application settings are loaded while showing the welcome screen:
					using (View.Forms.WelcomeScreen welcomeScreen = new View.Forms.WelcomeScreen())
					{
						if (welcomeScreen.ShowDialog() != DialogResult.OK)
							return (MainResult.ApplicationSettingsError);
					}
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					if (this.commandLineArgs.Interactive)
					{
						var message = ToSynchronousExceptionMessage(ex, "preparing");
						if (View.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, View.Forms.UnhandledExceptionType.Synchronous, true) == View.Forms.UnhandledExceptionResult.ExitAndRestart)
							System.Windows.Forms.Application.Restart(); // Is synchronous => OK to call Restart().

						// Ignore exception and continue.
					}
					else
					{
						return (MainResult.UnhandledException);
					}
				}

				try
			#endif
				{
					// If everything is fine so far, start main application including view:
					Model.MainResult viewResult;
					using (View.Forms.Main view = new View.Forms.Main(model))
					{
					#if (HANDLE_UNHANDLED_EXCEPTIONS)
						lock (this.mainThreadSynchronizerSyncObj)
							this.mainThreadSynchronizer = view;

						// Assume unhandled asynchronous synchronized exceptions and attach the application to the respective handler:
						System.Windows.Forms.Application.ThreadException += RunFullyWithView_Application_ThreadException;
					#endif
						// Start the Win32 message loop on the current thread and the main form.
						//
						// Attention:
						// This call does not return until the application exits.
						System.Windows.Forms.Application.Run(view);
					#if (HANDLE_UNHANDLED_EXCEPTIONS)
						System.Windows.Forms.Application.ThreadException -= RunFullyWithView_Application_ThreadException;
					#endif
						viewResult = view.Result;

					#if (HANDLE_UNHANDLED_EXCEPTIONS)
						lock (this.mainThreadSynchronizerSyncObj)
							this.mainThreadSynchronizer = null;
					#endif
					}

					if (!ApplicationSettings.CloseAndDispose())
						return (MainResult.ApplicationSettingsError);

					return (Convert(viewResult));
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					if (this.commandLineArgs.Interactive)
					{
						var message = ToSynchronousExceptionMessage(ex, "running");                                                                   // Synchronous exceptions cannot be continued as the application has already exited.
						if (View.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message, View.Forms.UnhandledExceptionType.Synchronous, false) == View.Forms.UnhandledExceptionResult.ExitAndRestart)
							System.Windows.Forms.Application.Restart(); // Is synchronous => OK to call Restart().
					}

					return (MainResult.UnhandledException);
				}
			#endif
			} // Dispose of model to ensure immediate release of resources.

			// Do not detach the handler from currentDomain.UnhandledException. In case of an
			// exception, detaching may result in a message like "YAT.exe doesn't work anymore".
		}

	#if (HANDLE_UNHANDLED_EXCEPTIONS)

		/// <remarks>
		/// In case of a <see cref="System.Windows.Forms.Application.ThreadException"/>, it is possible to continue execution.
		/// </remarks>
		private void RunFullyWithView_Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			if (this.commandLineArgs.Interactive)
			{
				var message = ToThreadExceptionMessage(e.Exception);
				var result = View.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(e.Exception, message, View.Forms.UnhandledExceptionType.AsynchronousSynchronized, true);
				switch (result)
				{
					case View.Forms.UnhandledExceptionResult.Continue:
					case View.Forms.UnhandledExceptionResult.ContinueAndIgnore:
						return; // Ignore exception and continue.

					case View.Forms.UnhandledExceptionResult.ExitAndRestart:
						System.Windows.Forms.Application.Restart(); // Is synchronized => OK to call Restart().
						break;

					case View.Forms.UnhandledExceptionResult.Exit:
					default:
						System.Windows.Forms.Application.Exit(); // Is synchronized => OK to call Exit().
						break;
				}
			}
			else
			{
				System.Windows.Forms.Application.Exit(); // Is synchronized => OK to call Exit().
			}
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart if the <see cref="UnhandledExceptionEventArgs.IsTerminating"/> flag is set.
		/// </remarks>
		private void RunFullyWithView_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread(object sender, UnhandledExceptionEventArgs e)
		{
			if (this.commandLineArgs.Interactive)
			{
				var ex = (e.ExceptionObject as Exception);

				var message = new StringBuilder(ToUnhandledExceptionMessage(ex));
				if (IsObjectDisposedExceptionInMscorlib(ex))
					message.Append(Environment.NewLine + Environment.NewLine + ObjectDisposedExceptionInMscorlibMessage);

				var result = View.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message.ToString(), View.Forms.UnhandledExceptionType.AsynchronousNonSynchronized, !e.IsTerminating);
				switch (result)
				{
					case View.Forms.UnhandledExceptionResult.Continue:
					case View.Forms.UnhandledExceptionResult.ContinueAndIgnore:
						return; // Ignore exception and continue.

					case View.Forms.UnhandledExceptionResult.ExitAndRestart:
						TrySynchronize(new Action(System.Windows.Forms.Application.Restart)); // Is *not* synchronized => Try to synchronize Restart() !!!
						break;

					case View.Forms.UnhandledExceptionResult.Exit:
					default:
						TrySynchronize(new Action(System.Windows.Forms.Application.Exit)); // Is *not* synchronized => Try to synchronize Exit() !!!
						break;
				}
			}
			else
			{
				TrySynchronize(new Action(System.Windows.Forms.Application.Exit)); // Is *not* synchronized => Try to synchronize Exit() !!!
			}
		}

		/// <remarks>
		/// Using <see cref="ISynchronizeInvoke.Invoke"/> and not
		/// <see cref="ISynchronizeInvoke.BeginInvoke"/> since calling
		/// <see cref="System.Windows.Forms.Application.Exit()"/> or
		/// <see cref="System.Windows.Forms.Application.Restart()"/> are also a synchronous calls.
		/// </remarks>
		private void TrySynchronize(Action action)
		{
			ISynchronizeInvoke invoker = null;
			lock (this.mainThreadSynchronizerSyncObj)
			{
				if ((this.mainThreadSynchronizer != null) && (this.mainThreadSynchronizer.InvokeRequired))
					invoker = this.mainThreadSynchronizer;
			}

			if (invoker != null)
				invoker.Invoke(action, null);
			else
				action();
		}

	#endif

		#endregion

		#region Non-Public Methods > RunWithViewButOutputErrorsOnConsole
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > RunWithViewButOutputErrorsOnConsole
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Exceptions are only handled in case of 'Release', otherwise by the debugger.
		/// </remarks>
	#if (HANDLE_UNHANDLED_EXCEPTIONS)
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
	#endif
		private MainResult RunWithViewButOutputErrorsOnConsole()
		{
			MessageHelper.RequestSupport =      "Support may be requested as described in 'Help > Request Support'.";
			MessageHelper.RequestFeature = "New features can be requested as described in 'Help > Request Feature'.";
			MessageHelper.RequestChange  = "Changes can also be requested as described in 'Help > Request Feature'.";
			MessageHelper.SubmitBug      =      "Please report this issue as described in 'Help > Submit Bug'.";
		#if (HANDLE_UNHANDLED_EXCEPTIONS)
			// Assume unhandled asynchronous non-synchronized exceptions and attach the application to the respective handler:
			AppDomain.CurrentDomain.UnhandledException += RunWithViewButOutputErrorsOnConsole_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread;

			// Assume unhandled asynchronous non-synchronized exceptions during events and attach the application to the same handler:
			EventHelper.UnhandledExceptionOnNonMainThread += RunWithViewButOutputErrorsOnConsole_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread;
		////EventHelper.UnhandledExceptionOnMainThread is not used as it is handled by the catch-all handler below.
		#endif
			// Create model and view and run application:
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
				MKY.Windows.Forms.ApplicationEx.EnableVisualStylesAndSetTextRenderingIfNotInitializedYet();
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				try
			#endif
				{
					// Application settings must be created and closed on main thread, otherwise
					// there will be a synchronization exception on exit (settings are closed there):
					if (!ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadSharedWriteIfOwned))
						return (MainResult.ApplicationSettingsError);

					// Application settings are loaded while showing the welcome screen:
					using (View.Forms.WelcomeScreen welcomeScreen = new View.Forms.WelcomeScreen())
					{
						if (welcomeScreen.ShowDialog() != DialogResult.OK)
							return (MainResult.ApplicationSettingsError);
					}
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					Console.Error.WriteLine(ToSynchronousExceptionMessage(ex, "preparing"));
					ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (MainResult.UnhandledException);
				}

				try
			#endif
				{
					// If everything is fine so far, start main application including view:
					Model.MainResult viewResult;
					using (View.Forms.Main view = new View.Forms.Main(model))
					{
					#if (HANDLE_UNHANDLED_EXCEPTIONS)
						lock (this.mainThreadSynchronizerSyncObj)
							this.mainThreadSynchronizer = view;

						// Assume unhandled asynchronous synchronized exceptions and attach the application to the respective handler:
						System.Windows.Forms.Application.ThreadException += RunWithViewButOutputErrorsOnConsole_Application_ThreadException;
					#endif
						// Start the Win32 message loop on the current thread and the main form.
						//
						// Attention:
						// This call does not return until the application exits.
						System.Windows.Forms.Application.Run(view);
					#if (HANDLE_UNHANDLED_EXCEPTIONS)
						System.Windows.Forms.Application.ThreadException -= new ThreadExceptionEventHandler(RunWithViewButOutputErrorsOnConsole_Application_ThreadException);
					#endif
						viewResult = view.Result;

					#if (HANDLE_UNHANDLED_EXCEPTIONS)
						lock (this.mainThreadSynchronizerSyncObj)
							this.mainThreadSynchronizer = null;
					#endif
					}

					if (!ApplicationSettings.CloseAndDispose())
						return (MainResult.ApplicationSettingsError);

					return (Convert(viewResult));
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					Console.Error.WriteLine(ToSynchronousExceptionMessage(ex, "running"));
					ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (MainResult.UnhandledException);
				}
			#endif
			} // Dispose of model to ensure immediate release of resources.

			// Do not detach the handler from currentDomain.UnhandledException. In case of an
			// exception, detaching may result in a message like "YAT.exe doesn't work anymore".
		}

	#if (HANDLE_UNHANDLED_EXCEPTIONS)

		/// <remarks>
		/// In case of a <see cref="System.Windows.Forms.Application.ThreadException"/>, it is possible to continue execution.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			var ex = e.Exception;

			Console.Error.WriteLine(ToThreadExceptionMessage(ex));

			if (ex != null)
			{
				ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.
			}
		}

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart if the <see cref="UnhandledExceptionEventArgs.IsTerminating"/> flag is set.
		/// </remarks>
		private void RunWithViewButOutputErrorsOnConsole_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = (e.ExceptionObject as Exception);

			Console.Error.WriteLine(ToUnhandledExceptionMessage(ex));

			if (ex != null)
			{
				ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

				if (IsObjectDisposedExceptionInMscorlib(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(ObjectDisposedExceptionInMscorlibMessage);
				}
			}
		}

	#endif

		#endregion

		#region Non-Public Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > RunFullyFromConsole
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		/// <remarks>
		/// Exceptions are only handled in case of 'Release', otherwise by the debugger.
		/// </remarks>
	#if (HANDLE_UNHANDLED_EXCEPTIONS)
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
	#endif
		private MainResult RunFullyFromConsole()
		{
			MessageHelper.RequestSupport =      "Support may be requested at <sourceforge.net/projects/y-a-terminal/support/>.";
			MessageHelper.RequestFeature = "New features can be requested at <sourceforge.net/projects/y-a-terminal/feature-requests/>.";
			MessageHelper.RequestChange  = "Changes can also be requested at <sourceforge.net/projects/y-a-terminal/feature-requests/>.";
			MessageHelper.SubmitBug =           "Please report this issue at <sourceforge.net/projects/y-a-terminal/bugs/>.";
		#if (HANDLE_UNHANDLED_EXCEPTIONS)
			// Assume unhandled asynchronous non-synchronized exceptions and attach the application to the respective handler:
			AppDomain.CurrentDomain.UnhandledException += RunFullyFromConsole_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread;

			// Assume unhandled asynchronous non-synchronized exceptions during events and attach the application to the same handler:
			EventHelper.UnhandledExceptionOnNonMainThread += RunFullyFromConsole_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread;
		////EventHelper.UnhandledExceptionOnMainThread is not used as it is handled by the catch-all handler below.
		#endif
			// Create model and run application:
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				try
			#endif
				{
					if (ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadShared))
					{
						ApplicationSettings.Load(); // Don't care about result, either settings have been loaded or settings have been set to defaults.

						// Application settings will be closed when exiting main.
					}
					else
					{
						return (MainResult.ApplicationSettingsError);
					}
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					Console.Error.WriteLine(ToSynchronousExceptionMessage(ex, "preparing"));
					ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (MainResult.UnhandledException);
				}

				try
			#endif
				{
					var modelResult = model.Start();
					if (modelResult == Model.MainResult.Success)
						modelResult = model.Exit();

					ApplicationSettings.CloseAndDispose(); // Don't care about result, as upon creation above.

					return (Convert(modelResult));
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					Console.Error.WriteLine(ToSynchronousExceptionMessage(ex, "running"));
					ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

					return (MainResult.UnhandledException);
				}
			#endif
			} // Dispose of model to ensure immediate release of resources.

			// Do not detach the handler from currentDomain.UnhandledException. In case of an
			// exception, detaching may result in a message like "YAT.exe doesn't work anymore".
		}

	#if (HANDLE_UNHANDLED_EXCEPTIONS)

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart if the <see cref="UnhandledExceptionEventArgs.IsTerminating"/> flag is set.
		/// </remarks>
		private void RunFullyFromConsole_CurrentDomain_UnhandledException_Or_EventHelper_UnhandledExceptionOnNonMainThread(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = (e.ExceptionObject as Exception);

			Console.Error.WriteLine(ToUnhandledExceptionMessage(ex));

			if (ex != null)
			{
				ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

				if (IsObjectDisposedExceptionInMscorlib(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(ObjectDisposedExceptionInMscorlibMessage);
				}
			}
		}

	#endif

		#endregion

		#region Non-Public Methods > RunInvisible
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > RunInvisible
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Non-view application for automated test usage.
		/// </summary>
		/// <remarks>
		/// Exceptions are only handled in case of 'Release', otherwise by the debugger.
		/// </remarks>
	#if (HANDLE_UNHANDLED_EXCEPTIONS)
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
	#endif
		private MainResult RunInvisible()
		{
			MessageHelper.RequestSupport =      "Support may be requested at <sourceforge.net/projects/y-a-terminal/support/>.";
			MessageHelper.RequestFeature = "New features can be requested at <sourceforge.net/projects/y-a-terminal/feature-requests/>.";
			MessageHelper.RequestChange  = "Changes can also be requested at <sourceforge.net/projects/y-a-terminal/feature-requests/>.";
			MessageHelper.SubmitBug =           "Please report this issue at <sourceforge.net/projects/y-a-terminal/bugs/>.";
		#if (HANDLE_UNHANDLED_EXCEPTIONS)
			// Assume unhandled asynchronous non-synchronized exceptions and attach the application to the respective handler:
			AppDomain.CurrentDomain.UnhandledException += RunInvisible_CurrentDomain_UnhandledException_Or_UnhandledExceptionOnNonMainThread;

			// Assume unhandled asynchronous non-synchronized exceptions during events and attach the application to the same handler:
			EventHelper.UnhandledExceptionOnNonMainThread += RunInvisible_CurrentDomain_UnhandledException_Or_UnhandledExceptionOnNonMainThread;
		////EventHelper.UnhandledExceptionOnMainThread is not used as it is handled by the catch-all handler below.
		#endif
			// Create model and run application:
			using (Model.Main model = new Model.Main(this.commandLineArgs))
			{
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				try
			#endif
				{
					if (ApplicationSettings.Create(ApplicationSettingsFileAccess.ReadShared))
					{
						ApplicationSettings.Load(); // Don't care about result, either settings have been loaded or settings have been set to defaults.

						// Application settings will be closed when exiting main.
					}
					else
					{
						return (MainResult.ApplicationSettingsError);
					}
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					ConsoleEx.Error.WriteException(GetType(), ex, ToSynchronousExceptionMessage(ex, "preparing"));

					return (MainResult.UnhandledException);
				}

				try
			#endif
				{
					var modelResult = model.Start();
					if (modelResult == Model.MainResult.Success)
						modelResult = model.Exit();

					ApplicationSettings.CloseAndDispose(); // Don't care about result, as upon creation above.

					return (Convert(modelResult));
				}
			#if (HANDLE_UNHANDLED_EXCEPTIONS)
				catch (Exception ex)
				{
					ConsoleEx.Error.WriteException(GetType(), ex, ToSynchronousExceptionMessage(ex, "running"));

					return (MainResult.UnhandledException);
				}
			#endif
			} // Dispose of model to ensure immediate release of resources.

			// Do not detach the handler from currentDomain.UnhandledException. In case of an
			// exception, detaching may result in a message like "YAT.exe doesn't work anymore".
		}

	#if (HANDLE_UNHANDLED_EXCEPTIONS)

		/// <remarks>
		/// In case of an <see cref="AppDomain.UnhandledException"/>, the application must exit or restart if the <see cref="UnhandledExceptionEventArgs.IsTerminating"/> flag is set.
		/// </remarks>
		private void RunInvisible_CurrentDomain_UnhandledException_Or_UnhandledExceptionOnNonMainThread(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = (e.ExceptionObject as Exception);

			Console.Error.WriteLine(ToUnhandledExceptionMessage(ex));

			if (ex != null)
			{
				ConsoleEx.Error.WriteException(GetType(), ex); // Message has already been output onto console.

				if (IsObjectDisposedExceptionInMscorlib(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(ObjectDisposedExceptionInMscorlibMessage);
				}
			}
		}

	#endif

		#endregion

		#region Non-Public Methods > MessageBox
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > MessageBox
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowMessageBoxHelp(bool showLogo)
		{
			var sb = new StringBuilder();

			if (showLogo)
			{
				foreach (string line in ApplicationEx.ProductLogo)
					sb.AppendLine(line);

				sb.AppendLine();
			}

			sb.Append(this.commandLineArgs.GetHelpText(DefaultTextWidth));
			sb.AppendLine();

			foreach (string line in ResultText)
				sb.AppendLine(line);

			MessageBoxEx.Show
			(
				sb.ToString(),
				ApplicationEx.ProductName + " Help", // "YAT" or "YATConsole", as indicated in main title bar.
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		#endregion

		#region Non-Public Methods > Console
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Console
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

			Console.Out.WriteLine(ApplicationEx.ProductCaptionAndVersion);

			Console.Out.WriteLine();
			Console.Out.WriteLine(new string('-', (Console.WindowWidth - 1))); // ----------...
			Console.Out.WriteLine();

			foreach (string line in ApplicationEx.ProductLogo)
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

		#region Non-Public Methods > Result
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Result
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		private static MainResult Convert(Model.MainResult result)
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

		#region Non-Public Methods > Exceptions
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Exceptions
		//------------------------------------------------------------------------------------------

	#if (HANDLE_UNHANDLED_EXCEPTIONS)

		private static string ToSynchronousExceptionMessage(Exception ex, string state)
		{
			var message = new StringBuilder();
			message.Append("An unhandled synchronous ");
			message.Append(ToName(ex));           // "preparing" or "running"
			message.Append(" occurred while " + state + " " + System.Windows.Forms.Application.ProductName + ".");
			return (message.ToString());
		}

		private static string ToThreadExceptionMessage(Exception ex)
		{
			var message = new StringBuilder();
			message.Append("An unhandled asynchronous synchronized ");
			message.Append(ToName(ex));
			message.Append(" occurred while running " + System.Windows.Forms.Application.ProductName + ".");
			return (message.ToString());
		}

		private static string ToUnhandledExceptionMessage(Exception ex)
		{
			var message = new StringBuilder();
			message.Append("An unhandled asynchronous non-synchronized ");
			message.Append(ToName(ex));
			message.Append(" occurred while running " + System.Windows.Forms.Application.ProductName + ".");
			return (message.ToString());
		}

		private static string ToName(Exception ex)
		{
			if (ex != null)
				return ("'" + ex.GetType().Name + "'");
			else
				return ("exception");
		}

		private static bool IsObjectDisposedExceptionInMscorlib(Exception ex)
		{
			return ((ex is ObjectDisposedException) && (ex.Source == "mscorlib") && (ex.Message.Contains("SafeHandle")));
		}

	#endif

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
