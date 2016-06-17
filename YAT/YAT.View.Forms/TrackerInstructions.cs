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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Forms;

using MKY;

namespace YAT.View.Forms
{
	/// <summary></summary>
	public enum TrackerType
	{
		/// <summary></summary>
		Support,

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
			sb.Append(ApplicationEx.ProductName);
			switch (tracker)
			{
				case TrackerType.Support: sb.Append(" Support Request"); break;
				case TrackerType.Feature: sb.Append(" Feature Request"); break;
				case TrackerType.Bug:
				default:                  sb.Append(" Bug Submission");  break;
			}
			Text = sb.ToString();

			// Intro:
			sb = new StringBuilder();
			switch (tracker)
			{
				case TrackerType.Support: sb.Append("Support for YAT can be requested online.");  break;
				case TrackerType.Feature: sb.Append("Features for YAT can be requested online."); break;
				case TrackerType.Bug:
				default:                  sb.Append("Bugs for YAT can be submitted online.");     break;
			}
			sb.Append(" Follow the link below and proceed according to the instructions.");
			linkLabel_Intro.Text = sb.ToString();

			// SourceForge remarks:
			sb = new StringBuilder();
			sb.Append("If you have a SourceForge.net account, you are advised to log on to SourceForge.");
			switch (tracker)
			{
				case TrackerType.Support: sb.Append(" You can then send a support request via the website's integrated email service.");  break;
				case TrackerType.Feature: sb.Append(" You will then get email notifications about the progress of the feature request."); break;
				case TrackerType.Bug:
				default:                  sb.Append(" You will then get email notifications about the progress of the bug submission.");  break;
			}
			sb.AppendLine();
			sb.Append("If you don't have a SourceForge.net account,");
			switch (tracker)
			{
				case TrackerType.Support: sb.Append(" directly contact YAT by email."); break;
				case TrackerType.Feature:
				case TrackerType.Bug:
				default:                  sb.Append(" you may provide a valid email address to get email notifications."); break;
			}
			linkLabel_SourceForgeRemark.Text = sb.ToString();

			// Link captions:
			switch (tracker)
			{
				case TrackerType.Support:
				{
					linkLabel_DirectLinkCaption.Text = "Email:";
					linkLabel_RootLinkCaption.Text   = "Support page:";
					break;
				}

				case TrackerType.Feature:
				case TrackerType.Bug:
				default:
				{
					linkLabel_DirectLinkCaption.Text = "Filtered and sorted link:";
					linkLabel_RootLinkCaption.Text   = "Tracker page:";
					break;
				}
			}

			// Links:
			string directLink = "";
			string rootLink = "";
			switch (tracker)
			{
				case TrackerType.Support:
				{
					directLink = "mailto:y-a-terminal@users.sourceforge.net";
					rootLink   = "https://sourceforge.net/projects/y-a-terminal/support";
					break;
				}

				case TrackerType.Feature:
				{
					directLink = "https://sourceforge.net/projects/y-a-terminal/feature-requests/search/?q=!status%3Aclosed-rejected+%26%26+!status%3Aclosed-duplicate+%26%26+!status%3Awont-fix+%26%26+!status%3Aclosed-fixed+%26%26+!status%3Aclosed&limit=25&page=0&sort=_priority_s%20desc";
					rootLink   = "https://sourceforge.net/projects/y-a-terminal/feature-requests/";
					break;
				}

				case TrackerType.Bug:
				default:
				{
					directLink = "https://sourceforge.net/projects/y-a-terminal/bugs/?q={%22status%22%3A+{%22%24nin%22%3A+[%22closed-rejected%22%2C+%22closed-duplicate%22%2C+%22wont-fix%22%2C+%22closed-fixed%22%2C+%22closed%22]}}&limit=25&page=0&sort=_priority%20desc";
					rootLink   = "https://sourceforge.net/projects/y-a-terminal/bugs/";
					break;
				}
			}
			linkLabel_DirectLink.Text = directLink;
			linkLabel_DirectLink.Links.Add(0, directLink.Length, directLink);
			linkLabel_RootLink.Text = rootLink;
			linkLabel_RootLink.Links.Add(0, rootLink.Length, rootLink);

			// Instructions:
			sb = new StringBuilder();
			sb.AppendLine("1. Select 'Create Ticket'.");
			sb.AppendLine("2. Fill in a 'Title'.");
			sb.AppendLine("3. Fill in as much information as possible into 'Description':");
			switch (tracker)
			{
				case TrackerType.Support:
				{
					sb.AppendLine("    > YAT version you are using.");
					sb.AppendLine("    > What you want to achieve.");
					sb.AppendLine("    > What didn't work.");
					break;
				}

				case TrackerType.Feature:
				{
					sb.AppendLine("    > YAT version you are using.");
					sb.AppendLine("    > Expected behaviour of the new feature.");
					sb.AppendLine("    > Use case(s) of the new feature.");
					break;
				}

				case TrackerType.Bug:
				default:
				{
					sb.AppendLine("    > YAT version you are using.");
					sb.AppendLine("    > Procedure to reproduce the bug.");
					sb.AppendLine("    > Any additional useful information.");
					break;
				}
			}
			sb.AppendLine("4. Optionally add one or more labels.");
			sb.AppendLine("5. Optionally add one or more attachments.");
			sb.AppendLine("6. Select 'Save'.");
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

		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "YAT is not (yet) capable for RTL")]
		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var link = (e.Link.LinkData as string);
			if (link != null)
			{
				Exception ex;
				if (!MKY.Net.Browser.TryBrowseUri(link, out ex))
				{
					string message = "Unable to open link." + Environment.NewLine + Environment.NewLine +
					                 "System error message:" + Environment.NewLine + ex.Message;

					MessageBox.Show
					(
						this.Parent,
						message,
						"System Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
				}
			}
			else
			{
				throw (new InvalidOperationException("Program execution should never get here, link data is invalid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
