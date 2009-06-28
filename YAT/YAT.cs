//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;

namespace YAT
{
	/// <summary>
	/// Application main class of YAT.
	/// </summary>
	/// <remarks>
	/// This class is separated into its own .exe project for those who want to use YAT
	/// components within their own application context.
	/// </remarks>
	public class YAT
	{
		/// <summary>
		/// Displays welcome screen and starts YAT.
		/// </summary>
		/// <remarks>
		/// If built as release, unhandled exceptions are caught here and shown to the user
		/// in an exception dialog. The user can then choose to send in the exception as
		/// feedback. In case of debug, unhandled exceptions are intentionally not handled
		/// here but by the development environment instead.
		/// </remarks>
		public Controller.MainResult Run(string[] commandLineArgs)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

		#if (!DEBUG)
			EventHelper.InstallUnhandledExceptionCallback(UnhandledExceptionCallback);
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
		#endif

			return (Controller.MainResult.OK);
		}

	#if (!DEBUG)
		private void UnhandledExceptionCallback(Exception ex)
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
		}
	#endif

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] commandLineArgs)
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
