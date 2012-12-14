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
				case Tracker.Feature: sb.Append(" Feature Request"); break;
				case Tracker.Bug:     sb.Append(" Bug Submission");  break;
			}
			Text = sb.ToString();

			// Intro.
			sb = new StringBuilder();
			switch (tracker)
			{
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
				case Tracker.Feature:
				{
					filteredAndSortedLink = "http://sourceforge.net/p/y-a-terminal/feature-requests/search/?q=!status%3Aclosed-rejected+%26%26+!status%3Aclosed-duplicate+%26%26+!status%3Awont-fix+%26%26+!status%3Aclosed-fixed+%26%26+!status%3Aclosed&limit=25&page=0&sort=_priority_s%20desc";
					plainLink             = "http://sourceforge.net/p/y-a-terminal/feature-requests/";
					break;
				}
				case Tracker.Bug:
				{
					filteredAndSortedLink = "http://sourceforge.net/p/y-a-terminal/bugs/?q={%22status%22%3A+{%22%24nin%22%3A+[%22closed-rejected%22%2C+%22closed-duplicate%22%2C+%22wont-fix%22%2C+%22closed-fixed%22%2C+%22closed%22]}}&limit=25&page=0&sort=_priority%20desc";
					plainLink             = "http://sourceforge.net/p/y-a-terminal/bugs/";
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
