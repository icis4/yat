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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using MKY.Diagnostics;
using MKY.Windows.Forms;

using YAT.Utilities;

#endregion

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Form indeed deals with exceptions.")]
	public partial class UnhandledException : System.Windows.Forms.Form
	{
		private Exception exception;
		private string title;
		private string originMessage;

		private string exceptionText;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public UnhandledException(Exception exception, string title, string originMessage)
		{
			InitializeComponent();

			// Keep information.
			this.exception = exception;
			this.title = title;
			this.originMessage = originMessage;

			// Set form title.
			Text = this.title;

			// Compose exception text.
			StringWriter text = new StringWriter(CultureInfo.InvariantCulture);
			try
			{
				text.WriteLine(this.originMessage);
				text.WriteLine();
				text.WriteLine(ApplicationInfo.ProductNameAndBuildNameAndVersion);
				text.WriteLine();

				AnyWriter.WriteException(text, null, this.exception);
			}
			catch (Exception ex)
			{
				string errorText = "Error, unhandled exception data could not be retrieved.";
				DebugEx.WriteException(this.GetType(), ex, errorText);
				text.Write(errorText);
			}
			this.exceptionText = text.ToString();
		}

		private void UnhandledException_Load(object sender, EventArgs e)
		{
			textBox_Exception.Text = this.exceptionText;

			// Don't know why it is necessary to manually deselect the text.
			// DeselectAll() isn't sufficient, settings the properties works.
			textBox_Exception.SelectionLength = 0;
			textBox_Exception.SelectionStart = 0;
		}

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation always succeeds.")]
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
					DialogResult userInput = MessageBox.Show
						(
						this,
						"Unhandled exception data could not be copied onto clipboard." + Environment.NewLine +
						"Ensure that clipboard is not in use and try again.",
						"Clipboard Error",
						MessageBoxButtons.RetryCancel,
						MessageBoxIcon.Error
						);
					retry = (userInput == DialogResult.Retry);
				}
				catch (Exception ex)
				{
					MessageBox.Show
						(
						this,
						"Unhandled exception data could not be copied onto clipboard." + Environment.NewLine +
						"Copy the information above manually to the bug description and add the system error message below." + Environment.NewLine + Environment.NewLine +
						"System error message:" + Environment.NewLine +
						ex.Message,
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					retry = false;
				}
			}
			while (retry);
		}

		private void button_Instructions_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Bug);
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
