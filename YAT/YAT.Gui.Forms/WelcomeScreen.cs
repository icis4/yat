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
using System.Threading;
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

		/// <summary>
		/// Timer to trigger loading of the application settings.
		/// </summary>
		/// <remarks>
		/// In order to load settings in parallel to changing the opacity of the form, this is a
		/// standard system timer, not a <see cref="System.Windows.Forms.Timer"/>.
		/// </remarks>
		private System.Timers.Timer applicationSettingsTimer = new System.Timers.Timer();

		private bool finishedLoading;
		private ReaderWriterLockSlim finishedLoadingLock = new ReaderWriterLockSlim();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
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

			if (Width < width)
				Width = width;

			label_Status.Text = "Loading settings...";

			this.applicationSettingsTimer.Interval = 100;
			this.applicationSettingsTimer.AutoReset = false;
			this.applicationSettingsTimer.Elapsed += new System.Timers.ElapsedEventHandler(applicationSettingsTimer_Elapsed);
			this.applicationSettingsTimer.Start();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void WelcomeScreen_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.applicationSettingsTimer.Dispose();
		}

		private void timer_Opacity_Tick(object sender, EventArgs e)
		{
			bool finishedLoading;

			this.finishedLoadingLock.EnterReadLock();
			finishedLoading = this.finishedLoading;
			this.finishedLoadingLock.ExitReadLock();

			// Close welcome screen immediately if application settings have successfully been loaded.
			// Close welcome screen after opacity transition if application settings could not be loaded successfully.
			if (finishedLoading)
			{
				timer_Opacity.Stop();
				Close();
			}
			else if (Opacity < 1.00)
			{
				// Opacity starts at 0.25 (25%).
				// 25% opacity increase per second in 1% steps.
				//   => 40ms ticks
				Opacity += 0.01;
				Refresh();
			}
		}

		/// <summary>
		/// Loads the application settings on a concurrent thread.
		/// </summary>
		private void applicationSettingsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Always success:
			// Either settings have been loaded or settings have been set to defaults.
			ApplicationSettings.Load();
			DialogResult = DialogResult.OK;

			this.finishedLoadingLock.EnterWriteLock();
			this.finishedLoading = true;
			this.finishedLoadingLock.ExitWriteLock();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
