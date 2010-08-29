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
using System.Windows.Forms;

using YAT.Settings.Application;
using YAT.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class WelcomeScreen : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// settings
		private System.Timers.Timer settingsTimer = new System.Timers.Timer();

		private bool applicationSettingsLoaded = false;
		private bool applicationSettingsReady = false;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public WelcomeScreen()
		{
			InitializeComponent();

			const int Margin = 8;
			int width = Width;
			int width2 = 0;

			label_Name.Text = ApplicationInfo.ProductName;
			width2 = label_Name.Left + label_Name.Width + Margin;
			if (width < width2)
				width = width2;

			label_Version.Text = "Version " + Application.ProductVersion;
			width2 = label_Version.Left + label_Version.Width + Margin;
			if (width < width2)
				width = width2;

			label_Status.Text  = "Loading settings...";

			if (Width < width)
				Width = width;

			this.settingsTimer.Interval = 100;
			this.settingsTimer.AutoReset = false;
			this.settingsTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.applicationSettingsTimer_Elapsed);
			this.settingsTimer.Start();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void WelcomeScreen_FormClosing(object sender, FormClosingEventArgs e)
		{
			timer_Opacity.Dispose();
			this.settingsTimer.Dispose();
		}

		private void timer_Opacity_Tick(object sender, EventArgs e)
		{
			// - close welcome screen immediately if application settings successfully loaded
			// - close welcome screen after opacity transition if application settings
			//   could not be loaded successfully
			if (!this.applicationSettingsLoaded && (Opacity < 1.00))
			{
				// opacity starts at 0.25 (25%)
				// 25% opacity increase per second in 1% steps
				// => 40ms ticks
				Opacity += 0.01;
				Refresh();
			}
			else if (this.applicationSettingsReady)
			{
				timer_Opacity.Stop();

				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void applicationSettingsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				this.applicationSettingsLoaded = ApplicationSettings.Load();
			}
			catch
			{
			}
			finally
			{
				this.applicationSettingsReady = true;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
