using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YAT
{
	/// <summary>
	/// Application main class of YAT.
	/// </summary>
	public class YAT
	{
		public YAT(string[] args)
		{
			Gui.Forms.WelcomeScreen welcomeScreen;
			Gui.Forms.Main app;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			#if (!DEBUG)
			try
			{
			#endif

			welcomeScreen = new Gui.Forms.WelcomeScreen();

			if (welcomeScreen.ShowDialog() != DialogResult.OK)
				return;

			app = new Gui.Forms.Main(args, welcomeScreen.ApplicationSettingsLoaded);

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
				return;
			}
			#endif

			#if (!DEBUG)
			try
			{
			#endif

			Application.Run(app);

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
				return;
			}
			#endif
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			new YAT(args);
		}
	}
}
