//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;

using MKY;

using YAT.Settings.Application;
using YAT.Utilities;

#endregion

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class WelcomeScreen : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool finishedLoading;
		private ReaderWriterLockSlim finishedLoadingLock = new ReaderWriterLockSlim();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "The application settings timer just invokes an asynchronous thread that loads the settings.")]
		public WelcomeScreen()
		{
			InitializeComponent();

			const int Margin = 8;
			int width = Width;
			int width2 = 0;

			label_Name.Text = ApplicationInfo.ProductNameAndBuildName;
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
			VoidDelegateVoid asyncInvoker = new VoidDelegateVoid(LoadApplicationSettings);
			asyncInvoker.BeginInvoke(null, null);
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Loads the application settings on a concurrent thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void LoadApplicationSettings()
		{
			try
			{
				// Don't care about result,
				//   either the settings have been loaded or they have been set to defaults.
				ApplicationSettings.Load();

				DialogResult = DialogResult.OK;
			}
			catch
			{
				DialogResult = DialogResult.Abort;
			}

			this.finishedLoadingLock.EnterWriteLock();
			this.finishedLoading = true;
			this.finishedLoadingLock.ExitWriteLock();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

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
				// 75% opacity increase within a second.
				//  3% opacity increase per step.
				//   => 40ms ticks
				Opacity += 0.03;
				Refresh();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
