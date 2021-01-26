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
// YAT Version 2.4.0
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY.Diagnostics;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Form indeed deals with exceptions.")]
	public partial class UnhandledException : Form
	{
		private string multiLineText;

		/// <summary></summary>
		public UnhandledException(Exception exception, string title, string originMessage)
		{
			InitializeComponent();

			// Set form title/caption:
			Text = title;

			// Adapt explanation text:
			label_Explanation.Text =
				"An unhandled exception occurred while running " + ApplicationEx.CommonName + ". " +
				"Please report this exception according to [Instructions on Bug Submission] below " +
				"to give valuable feedback to continuously improve " + ApplicationEx.CommonName + ".";

			// Compose exception text:
			using (var sw = new StringWriter(CultureInfo.CurrentCulture))
			{
				sw.WriteLine(originMessage);
				sw.WriteLine();
				sw.WriteLine(ApplicationEx.ProductCaptionAndVersion);
				sw.WriteLine();

				AnyWriter.WriteException(sw, null, exception);

				this.multiLineText = sw.ToString();
			}
		}

		private void UnhandledException_Load(object sender, EventArgs e)
		{
			textBox_Exception.Text = this.multiLineText;
		}

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an explicit user interaction.")]
		private void button_CopyToClipboard_Click(object sender, EventArgs e)
		{
			bool retry = false;
			do
			{
				try
				{
					Clipboard.SetDataObject(this.multiLineText, true);
				}
				catch (ExternalException)
				{
					string message =
						"Unhandled exception data could not be copied onto clipboard." + Environment.NewLine + Environment.NewLine +
						"Ensure that clipboard is not in use and try again.";

					DialogResult userInput = MessageBoxEx.Show
					(
						this,
						message,
						"Clipboard Error",
						MessageBoxButtons.RetryCancel,
						MessageBoxIcon.Error
					);

					retry = (userInput == DialogResult.Retry);
				}
				catch (ThreadStateException)
				{
					string message =
						"Asynchronous unhandled exception data cannot be copied onto clipboard." + Environment.NewLine + Environment.NewLine +
						"Copy the exception information manually.";

					MessageBoxEx.Show
					(
						this,
						message,
						"Asynchronous Thread",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information
					);

					retry = false;
				}
				catch (Exception ex)
				{
					string message =
						"Unhandled exception data could not be copied onto clipboard."                    + Environment.NewLine + Environment.NewLine +
						"Copy the exception information manually and add the system error message below." + Environment.NewLine + Environment.NewLine +
						"System error message:" + Environment.NewLine + ex.Message;

					MessageBoxEx.Show
					(
						this,
						message,
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					retry = false;
				}
			}
			while (retry);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Instructions_Click(object sender, EventArgs e)
		{
			var f = new FeedbackInstructions(FeedbackType.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
