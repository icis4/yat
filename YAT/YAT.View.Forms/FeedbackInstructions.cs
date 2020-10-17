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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

namespace YAT.View.Forms
{
	/// <summary></summary>
	public enum FeedbackType
	{
		/// <summary></summary>
		Support,

		/// <summary></summary>
		Feature,

		/// <summary></summary>
		Bug,

		/// <summary></summary>
		AnyOther
	}

	/// <summary></summary>
	public partial class FeedbackInstructions : Form
	{
		/// <summary></summary>
		public FeedbackInstructions(FeedbackType feedbackType)
		{
			InitializeComponent();

			StringBuilder sb;

			// Form:
			sb = new StringBuilder();
			sb.Append(ApplicationEx.CommonName); // Fixed to "YAT" as in text further below.
			switch (feedbackType)
			{
				case FeedbackType.Support:  sb.Append(" Support Request"); break;
				case FeedbackType.Feature:  sb.Append(" Feature Request"); break;
				case FeedbackType.Bug:      sb.Append(" Bug Submission" ); break;

				case FeedbackType.AnyOther:
				default:                    sb.Append(" Feedback"       ); break;
			}
			Text = sb.ToString();

			// Intro:
			sb = new StringBuilder();
			switch (feedbackType)
			{
				case FeedbackType.Support:  sb.Append("Support for "              + ApplicationEx.CommonName + " can be requested online."); break;
				case FeedbackType.Feature:  sb.Append("Features and changes for " + ApplicationEx.CommonName + " can be requested online."); break;
				case FeedbackType.Bug:      sb.Append("Bugs for "                 + ApplicationEx.CommonName + " can be submitted online."); break;

				case FeedbackType.AnyOther:
				default:                    sb.Append("Any other feedback for "   + ApplicationEx.CommonName + " can be sent by email."   ); break;
			}
			switch (feedbackType)
			{
				case FeedbackType.Support:
				case FeedbackType.Feature:
				case FeedbackType.Bug:      sb.Append(" Follow the link below and proceed according to the instructions."); break;

				case FeedbackType.AnyOther:
				default:                    sb.Append(" Click the link below to write the email."); break;
			}
			linkLabel_Intro.Text = sb.ToString();

			// SourceForge remarks:
			sb = new StringBuilder();
			sb.Append("If you have a SourceForge.net account, you are advised to log on to SourceForge.");
			switch (feedbackType)
			{
				case FeedbackType.Support:  sb.Append(" You can then participate in or start a discussion, or send an email via the website's integrated email service."); break;
				case FeedbackType.Feature:  sb.Append(" You will then get email notifications about the progress of the feature request."                               ); break;
				case FeedbackType.Bug:      sb.Append(" You will then get email notifications about the progress of the bug submission."                                ); break;

				case FeedbackType.AnyOther:
				default:                    sb.Append(" You can then send an email via the website's integrated email service."); break;
			}
			sb.AppendLine();
			sb.Append("If you don't have a SourceForge.net account,");
			switch (feedbackType)
			{
				case FeedbackType.Support:  sb.Append(" you can participate in or start a discussion, or contact " + ApplicationEx.CommonName + " via the email address stated in the [About] dialog."); break;

				case FeedbackType.Feature:
				case FeedbackType.Bug:      sb.Append(" you can optionally provide a email address to get email notifications."); break;

				case FeedbackType.AnyOther:
				default:                    sb.Append(" contact " + ApplicationEx.CommonName + " via the email address stated in the [About] dialog."); break;
			}
			linkLabel_SourceForgeRemark.Text = sb.ToString();

			// Links:
			string link = "";
			switch (feedbackType)
			{                                       // No idea why SF uses "projects" instead of "p" here...
				case FeedbackType.Support:  link = "https://sourceforge.net/projects/y-a-terminal/support";       break;
				case FeedbackType.Feature:  link = "https://sourceforge.net/p/y-a-terminal/feature-requests/";    break;
				case FeedbackType.Bug:      link = "https://sourceforge.net/p/y-a-terminal/bugs/";                break;

				case FeedbackType.AnyOther:
				default:                    link = "https://sourceforge.net/u/y-a-terminal/profile/send_message"; break;
			}
			linkLabel_Link.Text = link;
			linkLabel_Link.Links.Add(0, link.Length, link);

			// Instructions:
			sb = new StringBuilder();
			switch (feedbackType)
			{
				case FeedbackType.Support:
				{
					sb.AppendLine("1. Check existing discussions using [Search Discussion] or entering forums.");
					sb.AppendLine("2. If topic already exists, you may [Post] additional comments to the discussion.");
					sb.AppendLine("3. If topic does not yet exist, select [Create Topic].");
					sb.AppendLine("4. Fill in the subject.");
					sb.AppendLine("5. Provide as much information as possible into the description:");
					sb.AppendLine("    > Environment (" + ApplicationEx.CommonName + " version and settings, devices, system,...).");
					sb.AppendLine("    > What doesn't work as expected.");
					sb.AppendLine("    > What you want to achieve.");
					sb.AppendLine("6. Optionally add attachment(s).");
					sb.AppendLine("7. [Post]");
					break;
				}

				case FeedbackType.Feature:
				case FeedbackType.Bug:
				{
					sb.AppendLine("1. Check existing tickets using [Searches > New | Open ...].");
					sb.AppendLine("2. If issue already exists, you may [Post] additional comments to the ticket.");
					sb.AppendLine("3. If issue does not yet exist, select [Create Ticket].");
					sb.AppendLine("4. Fill in a descriptive title.");
					sb.AppendLine("5. Fill in as much information as possible into the text field:");
					sb.AppendLine("    > Environment (" + ApplicationEx.CommonName + " version and settings, devices, system,...).");
					if (feedbackType == FeedbackType.Feature) {
						sb.AppendLine("    > Expected behavior of the new or changed feature.");
						sb.AppendLine("    > Use case(s) of the new or changed feature.");
					} else {
						sb.AppendLine("    > Any useful information (condition, sequence,...) to *reproduce* the bug.");
						sb.AppendLine("    > If given, the output of the 'Unhandled Exception' dialog.");
					}
					sb.AppendLine("6. Optionally add attachment(s).");
					sb.AppendLine("7. [Save]");
					break;
				}

				case FeedbackType.AnyOther:
				default:
				{
					sb.AppendLine("Thanks for taking your time to make " + ApplicationEx.CommonName + " even better.");
					sb.AppendLine("Any kind of feedback is highly appreciated!");
					sb.AppendLine("");
					sb.AppendLine("Don't forget to describe your environment");
					sb.AppendLine("(" + ApplicationEx.CommonName + " version and settings, devices, system,...).");
					break;
				}
			}
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
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(Parent, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
