using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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
