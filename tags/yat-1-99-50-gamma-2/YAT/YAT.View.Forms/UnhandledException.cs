﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		private Exception exception;
		private string title;
		private string originMessage;

		private string exceptionText;

		/// <summary></summary>
		public UnhandledException(Exception exception, string title, string originMessage)
		{
			InitializeComponent();

			// Keep information:
			this.exception = exception;
			this.title = title;
			this.originMessage = originMessage;

			// Set form title/caption:
			Text = this.title;

			// Compose exception text:
			using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				sw.WriteLine(this.originMessage);
				sw.WriteLine();
				sw.WriteLine(ApplicationEx.ProductNameAndBuildNameAndVersion);
				sw.WriteLine();

				AnyWriter.WriteException(sw, null, this.exception);

				this.exceptionText = sw.ToString();
			}
		}

		private void UnhandledException_Load(object sender, EventArgs e)
		{
			textBox_Exception.Text = this.exceptionText;

			// Don't know why it is necessary to manually deselect the text.
			// DeselectAll() isn't sufficient, setting the properties works.
			textBox_Exception.SelectionLength = 0;
			textBox_Exception.SelectionStart = 0;
		}

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an explicit user interaction.")]
		private void button_CopyToClipboard_Click(object sender, EventArgs e)
		{
			bool retry = false;
			do
			{
				try
				{
					Clipboard.SetDataObject(this.exceptionText, true);
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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Instructions_Click(object sender, EventArgs e)
		{
			View.Forms.TrackerInstructions f = new View.Forms.TrackerInstructions(View.Forms.TrackerType.Bug);
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
