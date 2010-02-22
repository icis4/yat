//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Settings;

using YAT.Settings.Application;
using YAT.Settings.Workspace;
using YAT.Utilities;

namespace YAT.Gui.Forms
{
	public partial class WelcomeScreen : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// settings
		private System.Timers.Timer _settingsTimer = new System.Timers.Timer();

		private bool _applicationSettingsLoaded = false;
		private bool _applicationSettingsReady = false;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public WelcomeScreen()
		{
			InitializeComponent();

			const int margin = 8;
			int width = Width;
			int width2 = 0;

			label_Name.Text = ApplicationInfo.ProductName;
			width2 = label_Name.Left + label_Name.Width + margin;
			if (width < width2)
				width = width2;

			label_Version.Text = "Version " + Application.ProductVersion;
			width2 = label_Version.Left + label_Version.Width + margin;
			if (width < width2)
				width = width2;

			label_Status.Text  = "Loading settings...";

			if (Width < width)
				Width = width;

			_settingsTimer.Interval = 100;
			_settingsTimer.AutoReset = false;
			_settingsTimer.Elapsed += new System.Timers.ElapsedEventHandler(_applicationSettingsTimer_Elapsed);
			_settingsTimer.Start();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void WelcomeScreen_FormClosing(object sender, FormClosingEventArgs e)
		{
			timer_Opacity.Dispose();
			_settingsTimer.Dispose();
		}

		private void timer_Opacity_Tick(object sender, EventArgs e)
		{
			// - close welcome screen immediately if application settings successfully loaded
			// - close welcome screen after opacity transition if application settings
			//   could not be loaded successfully
			if (!_applicationSettingsLoaded && (Opacity < 1.00))
			{
				// opacity starts at 0.25 (25%)
				// 25% opacity increase per second in 1% steps
				// => 40ms ticks
				Opacity += 0.01;
				Refresh();
			}
			else if (_applicationSettingsReady)
			{
				timer_Opacity.Stop();

				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void _applicationSettingsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				_applicationSettingsLoaded = ApplicationSettings.Load();
			}
			catch
			{
			}
			finally
			{
				_applicationSettingsReady = true;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
