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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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

#if (DEBUG)

	// Enable debugging of welcome screen related Show():
////#define DEBUG_WELCOME_SCREEN_SHOW

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
#if (HANDLE_UNHANDLED_EXCEPTIONS)
using System.ComponentModel;
#endif
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms; // Note that several locations explicitly use 'System.Windows.Forms' to prevent naming conflicts with 'MKY.Windows.Forms' and 'YAT.Application'.

using MKY;
using MKY.Diagnostics;
using MKY.Settings;
using MKY.Threading;
using MKY.Windows.Forms; // Note that several locations explicitly use 'MKY.Windows.Forms' to prevent naming conflicts with 'System.Windows.Forms' and 'YAT.Application'.

using YAT.Settings.Application;
//// 'YAT.View.Forms' is explicitly used to prevent naming conflicts with same-named 'YAT.Application' classes like 'Main'.

#endregion

namespace YAT.Application
{
	/// <summary>
	/// Application main class of YAT.
	/// </summary>
	/// <remarks>
	/// This class is separated into its own project for those who want to use YAT components within
	/// their own context.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	public class Main
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
		#if (WITH_SCRIPTING)
			"-100      Script invalid content",
			"-101      Script stop on error",
			"-102      Script exit",
			"-103      Script user break",
			"-104      Script unhandled exception",
			"-105      Script invalid return value (legacy)",
			"-106      Script thread abort",
			"-107      Script remoting exception",
			"-108      Script invalid operation"
		#endif
		};

	#if (HANDLE_UNHANDLED_EXCEPTIONS)

		private static readonly string ObjectDisposedExceptionInMscorlibOrSystemMessage =
			"Such 'ObjectDisposedException' in the underlying system is an exeption " +
			"that YAT is aware of but cannot properly handle. " + ExceptionBackground;

		private static readonly string UnauthorizedAccessExceptionInEventLoopRunnerMessage =
			"Such 'UnauthorizedAccessException' in 'SerialStream.EventLoopRunner' is an exeption " +
			"that YAT is aware of but cannot properly handle. " + ExceptionBackground;

		private static readonly string ExceptionBackground =
			"It can happen when a serial COM port gets physically disconnected while it is open. " +
			"It happens due to a bug in the .NET 'SerialPort' class for which Microsoft only has " +
			"vague plans fixing. The issue is known for internal ports using the Microsoft " +
			"serial COM port driver, external USB/COM ports using the Microsoft USB CDC/ACM " +
			"(virtual serial port) driver as well as Microchip MCP2221 USB-to-UART/I2C bridges. " +
			"The issue is referred to by dozens of online blogs and articles. YAT is applying " +
			"several patches to try working around the issue, but apparently none of them has " +
			"succeeded in the current situation." +
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

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		// Intentionally don't directly return the args strings or object. The requested operation
		// could also be requested by some other means than the command line, e.g. by a config file.

		/// <remarks>
		/// Until the command line has been validated by <see cref="PrepareRun"/>,
		/// this property returns <c>false</c>.
		/// </remarks>
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
			if (this.commandLineArgs != null)
			{
				// Process and validate command line arguments:
				if (this.commandLineArgs.ProcessAndValidate())
					return (MainResult.Success);
				else
					return (MainResult.CommandLineError);

				// Note that this is the location where the command line arguments are processed
				// and validated in case of automated testing. In normal operation, they will be
				// processed and validated in Run() further below.
				//
				// Calling ProcessAndValidate() multiple times doesn't matter, this use case is
				// covered by 'ArgsHandler'.
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
			return (Run(false));
		}

		/// <summary>
		/// This is the main run method for console operation.
		/// </summary>
		public virtual MainResult RunFromConsole()
		{
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
			// in YAT.Model.Main.ProcessCommandLineArgsIntoLaunchRequests().
			//
			// In case of automated testing, the command line arguments will be processed and
			// validated in PrepareRun() above, and also in YAT.Model.Main.
			//
			// Calling ProcessAndValidate() multiple times doesn't matter, this use case is covered
			// by 'ArgsHandler'.

			// Prio 0 = None:
			if (this.commandLineArgs == null || this.commandLineArgs.HasNoArgs)
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
				result = Run(runFromConsole, showView, ApplicationSettingsFileAccess.ReadSharedWriteIfOwned, true);
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
		/// 1. 'Normal' operation with view
		///    > Start YAT from the Windows start menu
		///    > Equal to start YAT.exe directly
		///    ==> Run(false, true);
		///
		/// 2. 'File' triggered operation with view
		///    > Start YAT by executing a .yaw or .yat file
		///    > Uses file type relationship as defined by YAT.Setup
		///    ==> Run(false, true);
		///
		/// 3. 'cmd console' operation with view
		///    > Must use 'YATConsole' to ensure that output is properly routed back to console
		///    ==> Run(true, true);
		///
		/// 4. 'PowerShell' operation with view
		///    > Must use 'YATConsole' to ensure that output is properly routed back to PowerShell
		///    ==> Run(true, true);
		///
		/// 5. 'cmd console' operation with console only, no view at all
		///    > Must use 'YATConsole' with the -NoView/-nv option
		///    ==> Run(true, false);
		///
		/// 6. 'PowerShell' operation with console only, no view at all
		///    > Must use 'YATConsole' with the -NoView/-nv option
		///    ==> Run(true, false);
		///
		/// 7. YAT testing
		///    ==> Run(false, true) or Run(true, true) to test the view (e.g. view stress test)
		///    ==> Run(false, false) or Run(true, false) to test the behavior (e.g. application test)
		///
		/// Handling of the application settings is also related to these use cases.
		/// <see cref="ApplicationSettingsFileAccess.ReadSharedWriteIfOwned"/> means that the
		/// instance reads the application settings, but only the owner, i.e. the first instance
		/// that was started, also writes them.
		/// <see cref="ApplicationSettingsFileAccess.ReadShared"/> means that the instance only
		/// reads the application settings, independent on whether it is the first or subsequent
		/// instance.
		/// <see cref="ApplicationSettingsFileAccess.None"/> means that the instance uses temporary
		/// in-memory settings, i.e. neither reads nor writes from the application's file.
		/// </remarks>
		/// <param name="runFromConsole">See remarks above.</param>
		/// <param name="runWithView">See remarks above.</param>
		/// <param name="applicationSettingsFileAccess">See remarks above.</param>
		/// <param name="loadSettingsInWelcomeScreen">See YAT.Application.Test.TestFixtureSetUp() for background.</param>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "?!?")]
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, StyleCop isn't able to deal with command line terms such as 'cmd' or 'nv'...")]
		public virtual MainResult Run(bool runFromConsole, bool runWithView, ApplicationSettingsFileAccess applicationSettingsFileAccess, bool loadSettingsInWelcomeScreen)
		{
			MainThreadHelper.SetCurrentThread();

			MainResult result;

			if (!runFromConsole && runWithView)
			{
				result = RunFullyWithView(applicationSettingsFileAccess, loadSettingsInWelcomeScreen);                        // 1, 2, 7
			}
			else
			{
				// Default 'NonInteractive' in case of console or invisible execution:
				this.commandLineArgs.Override("NonInteractive", true);

				if (     runFromConsole && runWithView)
					result = RunWithViewButOutputErrorsOnConsole(applicationSettingsFileAccess, loadSettingsInWelcomeScreen); // 3, 4, 7
				else if (runFromConsole && !runWithView)
					result = RunFullyFromConsole(applicationSettingsFileAccess);                                              // 5, 6, 7
				else
					result = RunInvisible(applicationSettingsFileAccess);                                                     //       7
			}

			if (result == MainResult.CommandLineError)
			{
				if (runWithView)
					ShowMessageBoxHelp(true);
				else
					ShowConsoleHelp(true);
			}
			                                                                //// Two args required to format without leading
			DebugMessage("Exiting with {0} (0x{1:X}).", result, (int)result); // zeros, same as Visual Studio is doing, e.g.:
			                                                                  // "The program '[<ID>] <APP>.exe' has exited with code 0 (0x0)".
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
		private MainResult RunFullyWithView(ApplicationSettingsFileAccess applicationSettingsFileAccess, bool loadSettingsInWelcomeScreen)
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
					if (!ApplicationSettings.Create(applicationSettingsFileAccess))
						return (MainResult.ApplicationSettingsError);

					// Application settings must be loaded before main form is created, as the main
					// form's dimensions and location must be known prior to creating and showing it.
					if (loadSettingsInWelcomeScreen)
					{
						// Application settings are loaded while showing a welcome screen:
						DebugWelcomeScreenShow("Welcome screen...");
						using (View.Forms.WelcomeScreen welcomeScreen = new View.Forms.WelcomeScreen())
						{
							DebugWelcomeScreenShow("...showing...");
							var dr = welcomeScreen.ShowDialog();
							if (dr != DialogResult.OK)
							{
								DebugWelcomeScreenShow(string.Format(CultureInfo.InvariantCulture, "...failed with [{0}]!", dr));
								return (MainResult.ApplicationSettingsError);
							}
							DebugWelcomeScreenShow(string.Format(CultureInfo.InvariantCulture, "...done with [{0}].", dr));
						}
					}
					else // See YAT.Application.Test.TestFixtureSetUp() for background.
					{
						// Application settings are loaded synchronously here (solely used for testing purposes):
						ApplicationSettings.Load();
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

				if (IsSerialPortCausedObjectDisposedExceptionInMscorlibOrSystem(ex))
					message.Append(Environment.NewLine + Environment.NewLine + ObjectDisposedExceptionInMscorlibOrSystemMessage);
				else if (IsSerialPortCausedUnauthorizedAccessExceptionInEventLoopRunner(ex))
					message.Append(Environment.NewLine + Environment.NewLine + UnauthorizedAccessExceptionInEventLoopRunnerMessage);

				var result = View.Forms.UnhandledExceptionHandler.ProvideExceptionToUser(ex, message.ToString(), View.Forms.UnhandledExceptionType.AsynchronousNonSynchronized, !e.IsTerminating);
				switch (result)
				{
					case View.Forms.UnhandledExceptionResult.Continue:
					case View.Forms.UnhandledExceptionResult.ContinueAndIgnore:
						return; // Ignore exception and continue.

					case View.Forms.UnhandledExceptionResult.ExitAndRestart:
						TrySynchronize(new Action(System.Windows.Forms.Application.Restart)); // Is *not* synchronized => Try to synchronize Restart()!
						break;

					case View.Forms.UnhandledExceptionResult.Exit:
					default:
						TrySynchronize(new Action(System.Windows.Forms.Application.Exit)); // Is *not* synchronized => Try to synchronize Exit()!
						break;
				}
			}
			else
			{
				TrySynchronize(new Action(System.Windows.Forms.Application.Exit)); // Is *not* synchronized => Try to synchronize Exit()!
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
		private MainResult RunWithViewButOutputErrorsOnConsole(ApplicationSettingsFileAccess applicationSettingsFileAccess, bool loadSettingsInWelcomeScreen)
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
					if (!ApplicationSettings.Create(applicationSettingsFileAccess))
						return (MainResult.ApplicationSettingsError);

					// Application settings must be loaded before main form is created, as the main
					// form's dimensions and location must be known prior to creating and showing it.
					if (loadSettingsInWelcomeScreen)
					{
						// Application settings are loaded while showing a welcome screen:
						DebugWelcomeScreenShow("Welcome screen...");
						using (View.Forms.WelcomeScreen welcomeScreen = new View.Forms.WelcomeScreen())
						{
							DebugWelcomeScreenShow("...showing...");
							var dr = welcomeScreen.ShowDialog();
							if (dr != DialogResult.OK)
							{
								DebugWelcomeScreenShow(string.Format(CultureInfo.InvariantCulture, "...failed with [{0}]!", dr));
								return (MainResult.ApplicationSettingsError);
							}
							DebugWelcomeScreenShow(string.Format(CultureInfo.InvariantCulture, "...done with [{0}].", dr));
						}
					}
					else // See YAT.Application.Test.TestFixtureSetUp() for background.
					{
						// Application settings are loaded synchronously here (solely used for testing purposes):
						ApplicationSettings.Load();
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

				if (IsSerialPortCausedObjectDisposedExceptionInMscorlibOrSystem(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(ObjectDisposedExceptionInMscorlibOrSystemMessage);
				}
				else if (IsSerialPortCausedUnauthorizedAccessExceptionInEventLoopRunner(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(UnauthorizedAccessExceptionInEventLoopRunnerMessage);
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
		private MainResult RunFullyFromConsole(ApplicationSettingsFileAccess applicationSettingsFileAccess)
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
					if (ApplicationSettings.Create(applicationSettingsFileAccess))
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
					var modelResult = model.Launch();
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

				if (IsSerialPortCausedObjectDisposedExceptionInMscorlibOrSystem(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(ObjectDisposedExceptionInMscorlibOrSystemMessage);
				}
				else if (IsSerialPortCausedUnauthorizedAccessExceptionInEventLoopRunner(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(UnauthorizedAccessExceptionInEventLoopRunnerMessage);
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
		private MainResult RunInvisible(ApplicationSettingsFileAccess applicationSettingsFileAccess)
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
					if (ApplicationSettings.Create(applicationSettingsFileAccess))
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
					var modelResult = model.Launch();
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

				if (IsSerialPortCausedObjectDisposedExceptionInMscorlibOrSystem(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(ObjectDisposedExceptionInMscorlibOrSystemMessage);
				}
				else if (IsSerialPortCausedUnauthorizedAccessExceptionInEventLoopRunner(ex))
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine(UnauthorizedAccessExceptionInEventLoopRunnerMessage);
				}
			}
		}

	#endif

		#endregion

		#region Non-Public Methods > MessageBox
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > MessageBox
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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

		/// <summary>
		/// Related to ObjectDisposedException issue (bugs #224, #254, #293, #316, #317, #345, #382, #385, #387, #401,...).
		/// </summary>
		/// <remarks>
		/// Known error patterns:
		/// <![CDATA[
		/// Message:
		///     Safe handle has been closed
		/// Source:
		///     System
		/// Stack:
		///     at Microsoft.Win32.UnsafeNativeMethods.GetOverlappedResult(SafeFileHandle hFile, NativeOverlapped* lpOverlapped, Int32& lpNumberOfBytesTransferred, Boolean bWait)
		///     at System.IO.Ports.SerialStream.EventLoopRunner.WaitForCommEvent()
		///     ...
		///
		/// Message:
		///     Safe handle has been closed
		/// Source:
		///     mscorlib
		/// Stack:
		///     at System.StubHelpers.StubHelpers.SafeHandleC2NHelper(Object p This, IntPtr pCleanupWorkList)
		///     at Microsoft.Win32.UnsafeNativeMethods.WaitCommEvent(SafeFileHandle, hFile, Int32* lpEvtMask, Native...
		///     at System.IO.Ports.SerialStream.EventLoopRunner.WaitForCommEvent()
		///     ...
		///
		/// Message:
		///     Das SafeHandle wurde geschlossen.
		/// Source:
		///     mscorlib
		/// Stack:
		///     bei System.StubHelpers.StubHelpers.SafeHandleC2NHelper(Object pThis, IntPtr pCleanupWorkList)
		///     bei Microsoft.Win32.Win32Native.SetEvent(SafeWaitHandle handle)
		///     bei System.Threading.EventWaitHandle.Set()
		///     bei System.IO.Ports.SerialStream.AsyncFSCallback(UInt32 errorCode, UInt32 numBytes, NativeOverlapped pOverlapped)
		///     ...
		///
		/// Message:
		///     Das SafeHandle wurde geschlossen.
		/// Source:
		///     mscorlib
		/// Stack:
		///     bei System.StubHelpers.StubHelpers.SafeHandleC2NHelper(Object pThis, IntPtr pCleanupWorkList)
		///     bei Microsoft.Win32.UnsafeNativeMethods.GetOverlappedResult(SafeFileHandle hFile, NativeOverlapped* lpOverlapped, Int32& lpNumberOfBytesTransferred, Boolean bWait)
		///     bei System.IO.Ports.SerialStream.EventLoopRunner.WaitForCommEvent()
		///     ...
		/// ]]>
		/// Common:
		/// <list type="bullet">
		/// <item><description>Source is "System" or "mscorlib".</description></item>
		/// <item><description>Stack trace contains "SafeFileHandle" or "SafeWaitHandle".</description></item>
		/// <item><description>Stack trace contains "System.IO.Ports.SerialStream".</description></item>
		/// </list>
		/// </remarks>
		private static bool IsSerialPortCausedObjectDisposedExceptionInMscorlibOrSystem(Exception ex)
		{
			if (ex is ObjectDisposedException)
			{
				if ((ex.Source == "mscorlib") ||
				    (ex.Source == "System"))
				{
					if (ex.StackTrace.Contains("SafeFileHandle") ||
					    ex.StackTrace.Contains("SafeWaitHandle"))
					{
						if (ex.StackTrace.Contains("System.IO.Ports.SerialStream"))
						{
							return (true);
						}
					}
				}
			}

			return (false);
		}

		/// <summary>
		/// Related to UnauthorizedAccessException issue (bugs #442,...).
		/// </summary>
		/// <remarks>
		/// Known error patterns:
		/// <![CDATA[
		/// Message:
		///     Der Zugriff auf den Anschluss wurde verweigert.
		/// Source:
		///     System
		/// Stack:
		///     bei System.IO.Ports.InternalResources.WinIOError(Int32 errorCode, String str)
		///     bei System.IO.Port.SerialStream.EventLoopRunner.CallEvents(Int32 nativeEvents)
		///     bei System.IO.Port.SerialStream.EventLoopRunner.WaitForCommEvent()
		///     ...
		/// ]]>
		/// Common:
		/// <list type="bullet">
		/// <item><description>Source is "System".</description></item>
		/// <item><description>Stack trace contains "System.IO.Ports.SerialStream.EventLoopRunner".</description></item>
		/// </list>
		/// </remarks>
		private static bool IsSerialPortCausedUnauthorizedAccessExceptionInEventLoopRunner(Exception ex)
		{
			if (ex is UnauthorizedAccessException)
			{
				if ((ex.Source == "System"))
				{
					if (ex.StackTrace.Contains("System.IO.Ports.SerialStream.EventLoopRunner"))
					{
						return (true);
					}
				}
			}

			return (false);
		}

		// Note that there are several other issues with "System.IO.Ports.SerialPort":
		//  > IOException issue
		//  > ObjectDisposedException issue
		//  > UnauthorizedAccessException and deadlock issue (bugs #201, #205,...)
		// See "\MKY.IO.Ports\SerialPort\!-Doc\*.txt" for details.

	#endif

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG")]
		protected void DebugMessage(string format, params object[] args)
		{
			DebugMessage(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"",
					"",
					message
				)
			);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_WELCOME_SCREEN_SHOW")]
		private void DebugWelcomeScreenShow(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
