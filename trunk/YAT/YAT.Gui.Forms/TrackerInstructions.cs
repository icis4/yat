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
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Windows.Forms;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class TrackerInstructions : Form
	{
		/// <summary></summary>
		public enum Tracker
		{
			/// <summary></summary>
			Support,

			/// <summary></summary>
			Feature,

			/// <summary></summary>
			Bug
		}

		/// <summary></summary>
		public TrackerInstructions(Tracker tracker)
		{
			InitializeComponent();

			StringBuilder sb;

			// Form.
			sb = new StringBuilder();
			sb.Append(Application.ProductName);
			switch (tracker)
			{
				case Tracker.Support: sb.Append(" Support Request"); break;
				case Tracker.Feature: sb.Append(" Feature Request"); break;
				case Tracker.Bug:     sb.Append(" Bug Submission");  break;
			}
			Text = sb.ToString();

			// Intro.
			sb = new StringBuilder();
			switch (tracker)
			{
				case Tracker.Support: sb.Append("Support for YAT can be requested online.");  break;
				case Tracker.Feature: sb.Append("Features for YAT can be requested online."); break;
				case Tracker.Bug:     sb.Append("Bugs for YAT can be submitted online.");     break;
			}
			sb.Append(" Follow the link below and proceed according to the instructions.");
			linkLabel_Intro.Text = sb.ToString();

			// SourceForge remarks.
			sb = new StringBuilder();
			sb.Append("If you have a SourceForge.net account, log in to SourceForge before");
			sb.Append(" you proceed. You will then get email notifications about the progress");
			switch (tracker)
			{
				case Tracker.Support: sb.Append(" of the support request."); break;
				case Tracker.Feature: sb.Append(" of the feature request."); break;
				case Tracker.Bug:     sb.Append(" of the bug submission.");  break;
			}
			sb.AppendLine();
			sb.Append("If you don't have a SourceForge.net account,");
			sb.Append(" you may provide a valid email address to get the notifications.");
			linkLabel_SourceForgeRemark.Text = sb.ToString();

			// Links.
			string filteredAndSortedLink = "";
			string plainLink = "";
			switch (tracker)
			{
				case Tracker.Support:
				{
					filteredAndSortedLink = "http://sourceforge.net/tracker/?words=tracker_browse&sort=priority&sortdir=desc&offset=0&group_id=193033&atid=943798&assignee=&status=1&category=&artgroup=&keyword=&submitter=&artifact_id=";
					plainLink             = "http://sourceforge.net/tracker/?group_id=193033&atid=943798";
					break;
				}
				case Tracker.Feature:
				{
					filteredAndSortedLink = "http://sourceforge.net/tracker/?words=tracker_browse&sort=priority&sortdir=desc&offset=0&group_id=193033&atid=943800&assignee=&status=1&category=&artgroup=&keyword=&submitter=&artifact_id=";
					plainLink             = "http://sourceforge.net/tracker/?group_id=193033&atid=943800";
					break;
				}
				case Tracker.Bug:
				{
					filteredAndSortedLink = "http://sourceforge.net/tracker/?words=tracker_browse&sort=priority&sortdir=desc&offset=0&group_id=193033&atid=943797&assignee=&status=1&category=&artgroup=&keyword=&submitter=&artifact_id=";
					plainLink             = "http://sourceforge.net/tracker/?group_id=193033&atid=943797";
					break;
				}
			}
			linkLabel_FilteredAndSortedLink.Text = filteredAndSortedLink;
			linkLabel_FilteredAndSortedLink.Links.Add(0, filteredAndSortedLink.Length, filteredAndSortedLink);
			linkLabel_PlainLink.Text = plainLink;
			linkLabel_PlainLink.Links.Add(0, plainLink.Length, plainLink);

			// Instructions.
			sb = new StringBuilder();
			sb.AppendLine("1. Choose 'Add New'");
			sb.AppendLine("2. Select a 'Category'");
			sb.AppendLine("3. Select a 'Group', i.e. the YAT version you are using");
			sb.AppendLine("4. Fill in 'Summary'");
			sb.AppendLine("5. Fill in 'Description'");
			sb.AppendLine("6. Choose 'Add'");
			linkLabel_Instructions.Text = sb.ToString();
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
			if ((link != null) && (link.StartsWith("http://", StringComparison.Ordinal)))
				MKY.Net.Browser.BrowseUri(link);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
