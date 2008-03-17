using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	public partial class TrackerInstructions : Form
	{
		public enum Tracker
		{
			Support,
			Feature,
			Bug
		}

		public TrackerInstructions(Tracker tracker)
		{
			InitializeComponent();

			string text = "";
			string textLink = "";
			int start = 0;

			// form
			text = ApplicationInfo.ProductName;
			switch (tracker)
			{
				case Tracker.Support: text += " Support Request"; break;
				case Tracker.Feature: text += " Feature Request"; break;
				case Tracker.Bug:     text += " Bug Submission";  break;
			}

			// intro
			switch (tracker)
			{
				case Tracker.Support: text = "Support for YAT can be requested online.";  break;
				case Tracker.Feature: text = "Features for YAT can be requested online."; break;
				case Tracker.Bug:     text = "Bugs for YAT can be submitted online.";     break;
			}
			text += " Follow the link below and" + Environment.NewLine +
				    "proceed according to the instructions.";
			linkLabel_Intro.Text = text;

			// SourceForge remarks
			text = "If you have a SourceForge.net account, log in to SourceForge before" + Environment.NewLine +
				   "you proceed. You will then get email notifications about the progress" + Environment.NewLine;
			switch (tracker)
			{
				case Tracker.Support: text += "of the support request."; break;
				case Tracker.Feature: text += "of the feature request."; break;
				case Tracker.Bug:     text += "of the bug submission.";  break;
			}
			text += Environment.NewLine;
			text += "If you don't have a SourceForge.net account, make sure to provide a" + Environment.NewLine +
				    "valid email address.";
			linkLabel_SourceForgeRemark.Text = text;

			// link
			linkLabel_Link.Text = "";
			switch (tracker)
			{
				case Tracker.Support: textLink = "http://sourceforge.net/tracker/?group_id=193033&atid=943798"; break;
				case Tracker.Feature: textLink = "http://sourceforge.net/tracker/?group_id=193033&atid=943800"; break;
				case Tracker.Bug:     textLink = "http://sourceforge.net/tracker/?group_id=193033&atid=943797"; break;
			}
			start = linkLabel_Link.Text.Length;
			linkLabel_Link.Text += textLink;
			linkLabel_Link.Links.Add(start, textLink.Length, textLink);

			// instructions
			text = "1. Click on \"Submit New\"" + Environment.NewLine +
				   "2. Select a \"Category\"" + Environment.NewLine +
				   "3. Select a \"Group\", i.e. the YAT version you are using" + Environment.NewLine +
				   "4. Fill in \"Summary\"" + Environment.NewLine +
				   "5. Fill in \"Detailed Description\"" + Environment.NewLine +
				   "6. Click on \"SUBMIT\"";
			linkLabel_Instructions.Text = text;
		}

		private void button_Close_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
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

		#endregion
	}
}
