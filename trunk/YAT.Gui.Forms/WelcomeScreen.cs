using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Settings;
using MKY.YAT.Settings.Application;
using MKY.YAT.Settings.Workspace;

namespace MKY.YAT.Gui.Forms
{
	public partial class WelcomeScreen : Form
	{
		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		// settings
		private System.Timers.Timer _settingsTimer = new System.Timers.Timer();

		private bool _applicationSettingsLoaded = false;
		private bool _applicationSettingsReady = false;

		//------------------------------------------------------------------------------------------
		// Object Lifetime
		//------------------------------------------------------------------------------------------

		public WelcomeScreen()
		{
			InitializeComponent();

			label_Name.Text = Application.ProductName;
			label_Name.Text += VersionInfo.ProductNamePostFix;

			label_Version.Text = "Version " + Application.ProductVersion;
			label_Status.Text  = "Loading settings...";

			_settingsTimer.Interval = 100;
			_settingsTimer.AutoReset = false;
			_settingsTimer.Elapsed += new System.Timers.ElapsedEventHandler(_applicationSettingsTimer_Elapsed);
			_settingsTimer.Start();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public bool ApplicationSettingsLoaded
		{
			get { return (_applicationSettingsLoaded); }
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

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
