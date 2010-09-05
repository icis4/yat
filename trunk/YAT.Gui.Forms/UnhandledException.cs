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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using MKY.Utilities.Diagnostics;
using MKY.Utilities.Windows.Forms;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Form indeed deals with exceptions.")]
	public partial class UnhandledException : System.Windows.Forms.Form
	{
		private Exception exeption;

		public UnhandledException(Exception exeption)
		{
			InitializeComponent();

			string textBefore = "";
			string textLink = "";
			string textAfter = "";
			int start = 0;

			linkLabel_Explanation.Text = "";
			textBefore = "An unhandled exception occured in YAT. Please report this exception to YAT > Tracker > Bugs on ";
			textLink =   "SourceForge.net";
			textAfter =                 " to give valuable feedback to continuously improve YAT.";
			linkLabel_Explanation.Text += textBefore;
			start = linkLabel_Explanation.Text.Length;
			linkLabel_Explanation.Text += textLink;
			linkLabel_Explanation.Links.Add(start, textLink.Length, "http://sourceforge.net/projects/y-a-terminal/");
			linkLabel_Explanation.Text += textAfter;

			this.exeption = exeption;
		}

		private void UnhandledException_Load(object sender, EventArgs e)
		{
			textBox_Type.Text = this.exeption.GetType().ToString();
			textBox_Message.Text = this.exeption.Message;
			textBox_Source.Text = this.exeption.Source;
			textBox_Stack.Text = this.exeption.StackTrace;
		}

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string link = e.Link.LinkData as string;
			if ((link != null) && (link.StartsWith("http://")))
				MKY.Utilities.Net.Browser.BrowseUri(link);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation always succeeds.")]
		private void button_CopyToClipboard_Click(object sender, EventArgs e)
		{
			StringWriter text = new StringWriter();
			try
			{
				text.Write(ApplicationInfo.ProductName);
				text.Write(" Version ");
				text.Write(Application.ProductVersion);
				text.WriteLine();
				text.WriteLine();

				AnyWriter.WriteException(text, null, this.exeption);
			}
			catch
			{
				text.Write("Error, unhandled exception data could not be retrieved.");
			}

			bool retry = false;
			do
			{
				try
				{
					Clipboard.SetDataObject(text.ToString(), true);
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
						"Unhandled exception data could not be copied onto clipboard." + Environment.NewLine + Environment.NewLine +
						"System message:" + Environment.NewLine +
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
			f.Location = XForm.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
