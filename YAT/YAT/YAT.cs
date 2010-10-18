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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Forms;

using MKY.System;
using MKY.System.Event;

namespace YAT
{
	/// <summary>
	/// Application main class of YAT.
	/// </summary>
	/// <remarks>
	/// This class is separated into its own .exe project for those who want to use YAT components
	/// within their own application context.
	/// Sealed to prevent FxCop "CA1052:StaticHolderTypesShouldBeSealeds".
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	sealed public class YAT
	{
		/// <remarks>
		/// Prevent FxCop "CA1053:StaticHolderTypesShouldNotHaveConstructors".
		/// </remarks>
		private YAT()
		{
		}

		/// <summary>
		/// Displays welcome screen and starts YAT.
		/// </summary>
		/// <param name="commandLineArgs">An array containing the command line arguments.</param>
		/// <returns>
		/// The application's exit code according to <see cref="Controller.MainResult"/>.
		/// </returns>
		/// <remarks>
		/// If built as release, unhandled exceptions are caught here and shown to the user in an
		/// exception dialog. The user can then choose to send in the exception as feedback.
		/// In case of debug, unhandled exceptions are intentionally not handled here. Instead,
		/// they are handled by the development environment.
		/// </remarks>
		public Controller.MainResult Run(string[] commandLineArgs)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		#if (!DEBUG)
			EventHelper.UnhandledException += new EventHandler<EventHelper.UnhandledExceptionEventArgs>(EventHelper_UnhandledException);
		#endif
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
				if (MessageBox.Show("An unhandled exception occured while loading " + Application.ProductName + "." + Environment.NewLine +
									"Show detailed information?",
									Application.ProductName,
									MessageBoxButtons.YesNoCancel,
									MessageBoxIcon.Stop,
									MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					Gui.Forms.UnhandledException f = new Gui.Forms.UnhandledException(ex);
					f.ShowDialog();
				}
				return (Controller.MainResult.UnhandledException);
			}
		#endif
		#if (!DEBUG)
			try
			{
		#endif
				Controller.Main main = new Controller.Main(commandLineArgs);
				main.Run();
		#if (!DEBUG)
			}
			catch (Exception ex)
			{
				if (MessageBox.Show("An unhandled exception occured in " + Application.ProductName + "." + Environment.NewLine +
									"Show detailed information?",
									Application.ProductName,
									MessageBoxButtons.YesNoCancel,
									MessageBoxIcon.Stop,
									MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					Gui.Forms.UnhandledException f = new Gui.Forms.UnhandledException(ex);
					f.ShowDialog();
				}
				return (Controller.MainResult.UnhandledException);
			}
			finally
			{
				EventHelper.UnhandledException -= new EventHandler<EventHelper.UnhandledExceptionEventArgs>(EventHelper_UnhandledException);
			}
		#endif
			return (Controller.MainResult.Success);
		}

		#if (!DEBUG)
		private void EventHelper_UnhandledException(object sender, EventHelper.UnhandledExceptionEventArgs e)
		{
			if (MessageBox.Show("An unhandled exception occured in " + Application.ProductName + "." + Environment.NewLine +
								"Show detailed information?",
								Application.ProductName,
								MessageBoxButtons.YesNoCancel,
								MessageBoxIcon.Stop,
								MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				Gui.Forms.UnhandledException f = new Gui.Forms.UnhandledException(e.UnhandledException);
				f.ShowDialog();
			}
		}
		#endif

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <param name="commandLineArgs">An array containing the command line arguments.</param>
		/// <returns>
		/// The application's exit code according to <see cref="Controller.MainResult"/>.
		/// </returns>
		[STAThread]
		private static int Main(string[] commandLineArgs)
		{
			YAT yat = new YAT();
			Controller.MainResult result = yat.Run(commandLineArgs);
			return ((int)result);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
