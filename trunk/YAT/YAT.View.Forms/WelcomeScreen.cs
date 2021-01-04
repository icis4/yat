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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of form/dialog related Show():
////#define DEBUG_SHOW

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

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
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public WelcomeScreen()
		{
			InitializeComponent();

			const int Margin = 8;
			int totalWidth = Width;
			int lineWidth = 0;

			label_Caption.Text = ApplicationEx.ProductCaption;
			lineWidth = label_Caption.Left + label_Caption.Width + Margin;

			if (totalWidth < lineWidth)
				totalWidth = lineWidth;
			          //// Using "Version" prefix for better appearance even though other locations no longer use "Version".
			label_Version.Text = "Version " + ApplicationEx.ProductVersionWithStabilityIndication;
			lineWidth = label_Version.Left + label_Version.Width + Margin;

			if (totalWidth < lineWidth)
				totalWidth = lineWidth;

			if (Width < totalWidth)
				Width = totalWidth;

			DebugShow("Invoking LoadApplicationSettingsAsync()...");
			label_Status.Text = "Loading settings...";
			backgroundWorker_LoadSettings.RunWorkerAsync();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private void backgroundWorker_LoadSettings_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			try
			{
				DebugShow("...loading application settings...");
				ApplicationSettings.Load(); // Don't care about result, either the settings have been loaded or they have been set to defaults.
				DebugShow("...successfully done.");

				DialogResult = DialogResult.OK;
				DebugShow(string.Format(CultureInfo.InvariantCulture, "Closing dialog, result is [{0}].", DialogResult));
			}
			catch
			{
				DebugShow("...failed!");

				DialogResult = DialogResult.Abort;
				DebugShow(string.Format(CultureInfo.InvariantCulture, "Closing dialog, result is [{0}]!", DialogResult));
			}

			// .NET WinForms:
			// "If the form is displayed as a dialog box, setting this property with a value from
			//  the DialogResult enumeration sets the value of the dialog box result for the form,
			// hides the modal dialog box, and returns control to the calling form."
			//
			// NUnit:
			// For whatever reason, the above doesn't happen in case of NUnit test execution.
			// Therefore, explicitly closing the form further below.
			//
			// Attention:
			// Close() cannot be called here, since this background worker is executed on different
			// thread than the main thread!

			timer_Opacity.Stop();
		}

		private void backgroundWorker_LoadSettings_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			Close(); // See background information further above.
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_Opacity_Tick(object sender, EventArgs e)
		{
			if (Opacity < 1.00)
			{
				DebugShow(string.Format(CultureInfo.InvariantCulture, "Opacity was {0:P0} and loading has not yet finished => increasing opacity.", Opacity));

				// Opacity starts at 0.25 (25¨%).
				// 75 % opacity increase within a second.
				//  3 % opacity increase per step.
				//    => 40 ms ticks
				Opacity += 0.03;
				Refresh();
			}
			else
			{
				DebugShow(string.Format(CultureInfo.InvariantCulture, "Opacity is {0:P0} but loading has not yet finished => stopping timer.", Opacity));

				timer_Opacity.Stop();
			}
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name 'DebugWriteLine' would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named 'Message' for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. 'Common' for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"",
					"",
					message
				)
			);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_SHOW")]
		private void DebugShow(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
