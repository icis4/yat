//==================================================================================================
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.Windows.Forms;

namespace YAT.Gui.Forms
{
	public partial class UnhandledException : System.Windows.Forms.Form
	{
		Exception _exeption;

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
			textAfter =                 " to give us valuable feedback to continuously improve YAT.";
			linkLabel_Explanation.Text += textBefore;
			start = linkLabel_Explanation.Text.Length;
			linkLabel_Explanation.Text += textLink;
			linkLabel_Explanation.Links.Add(start, textLink.Length, "http://sourceforge.net/projects/y-a-terminal/");
			linkLabel_Explanation.Text += textAfter;

			_exeption = exeption;
		}

		private void UnhandledException_Load(object sender, EventArgs e)
		{
			textBox_Type.Text = _exeption.GetType().ToString();
			textBox_Message.Text = _exeption.Message;
			textBox_Source.Text = _exeption.Source;
			textBox_Stack.Text = _exeption.StackTrace;
		}

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string link = e.Link.LinkData as string;
			if ((link != null) && (link.StartsWith("http://")))
			{
				MKY.Utilities.Net.Browser.BrowseUrl(link);
			}
		}

		private void button_CopyToClipboard_Click(object sender, EventArgs e)
		{
			StringWriter text = new StringWriter();
			text.WriteLine("Unhandled exception occured in YAT");
			text.WriteLine();
			text.WriteLine("Type:");
			text.WriteLine(_exeption.GetType().ToString());
			text.WriteLine();
			text.WriteLine("Message:");
			text.WriteLine(_exeption.Message);
			text.WriteLine();
			text.WriteLine("Source:");
			text.WriteLine(_exeption.Source);
			text.WriteLine();
			text.WriteLine("Stack:");
			text.WriteLine(_exeption.StackTrace);
			Clipboard.SetDataObject(text.ToString(), true);
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
