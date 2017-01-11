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
// YAT 2.0 Gamma 2 Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
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

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Scope = "member", Target = "YAT.View.Forms.WelcomeScreen.#InitializeComponent()", Justification = "The timer is only used for a well-defined interval.")]

#endregion

namespace YAT.View.Forms
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
		public WelcomeScreen()
		{
			InitializeComponent();

			const int Margin = 8;
			int width = Width;
			int width2 = 0;

			label_Name.Text = ApplicationEx.ProductNameAndBuildName;
			width2 = label_Name.Left + label_Name.Width + Margin;
			if (width < width2)
				width = width2;

			label_Version.Text = "Version " + ApplicationEx.ProductVersion;
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
				ApplicationSettings.Load(); // Don't care about result, either the settings have been loaded or they have been set to defaults.

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

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
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
