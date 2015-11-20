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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	public enum TrackerType
	{
		/// <summary></summary>
		Feature,

		/// <summary></summary>
		Bug
	}

	/// <summary></summary>
	public partial class TrackerInstructions : Form
	{
		/// <summary></summary>
		public TrackerInstructions(TrackerType tracker)
		{
			InitializeComponent();

			StringBuilder sb;

			// Form:
			sb = new StringBuilder();
			sb.Append(ApplicationInfo.ProductName);
			switch (tracker)
			{
				case TrackerType.Feature: sb.Append(" Feature Request"); break;
				case TrackerType.Bug:     sb.Append(" Bug Submission");  break;
			}
			Text = sb.ToString();

			// Intro:
			sb = new StringBuilder();
			switch (tracker)
			{
				case TrackerType.Feature: sb.Append("Features for YAT can be requested online."); break;
				case TrackerType.Bug:     sb.Append("Bugs for YAT can be submitted online.");     break;
			}
			sb.Append(" Follow the link below and proceed according to the instructions.");
			linkLabel_Intro.Text = sb.ToString();

			// SourceForge remarks:
			sb = new StringBuilder();
			sb.Append("If you have a SourceForge.net account, log in to SourceForge before");
			sb.Append(" you proceed. You will then get email notifications about the progress");
			switch (tracker)
			{
				case TrackerType.Feature: sb.Append(" of the feature request."); break;
				case TrackerType.Bug:     sb.Append(" of the bug submission.");  break;
			}
			sb.AppendLine();
			sb.Append("If you don't have a SourceForge.net account,");
			sb.Append(" you may provide a valid email address to get the notifications.");
			linkLabel_SourceForgeRemark.Text = sb.ToString();

			// Links:
			string filteredAndSortedLink = "";
			string plainLink = "";
			switch (tracker)
			{
				case TrackerType.Feature:
				{
					filteredAndSortedLink = "http://sourceforge.net/p/y-a-terminal/feature-requests/search/?q=!status%3Aclosed-rejected+%26%26+!status%3Aclosed-duplicate+%26%26+!status%3Awont-fix+%26%26+!status%3Aclosed-fixed+%26%26+!status%3Aclosed&limit=25&page=0&sort=_priority_s%20desc";
					plainLink             = "http://sourceforge.net/p/y-a-terminal/feature-requests/";
					break;
				}
				case TrackerType.Bug:
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

			// Instructions:
			sb = new StringBuilder();
			sb.AppendLine("1. Select 'Create Ticket'");
			sb.AppendLine("2. Fill in a 'Title'");
			sb.AppendLine("3. Fill in as much information as possible into 'Description'");
			switch (tracker)
			{
				case TrackerType.Feature:
					sb.AppendLine("    > YAT version you are using");
					sb.AppendLine("    > Expected behaviour of the new feature");
					sb.AppendLine("    > Use case(s) of the new feature");
					break;
				case TrackerType.Bug:
					sb.AppendLine("    > YAT version you are using");
					sb.AppendLine("    > Procedure to reproduce the bug");
					sb.AppendLine("    > Any additional useful information");
					break;
			}
			sb.AppendLine("4. Optionally add one or more labels");
			sb.AppendLine("5. Optionally add one or more attachments");
			sb.AppendLine("6. Select 'Save'");
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
			if (link != null)
			{
				Exception ex;
				if (!MKY.Net.Browser.BrowseUri(link, out ex))
				{
					MessageBox.Show
					(
						this.Parent,
						"Unable to open link." + Environment.NewLine + Environment.NewLine +
						"System message:" + Environment.NewLine + ex.Message,
						"System Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
				}
			}
			else
			{
				throw (new InvalidOperationException("Invalid link, program execution should never get here, please report this bug!"));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
