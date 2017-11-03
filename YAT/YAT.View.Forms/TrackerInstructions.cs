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
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

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
				default:                  sb.Append(" you can optionally provide a email address to get email notifications."); break;
			}
			linkLabel_SourceForgeRemark.Text = sb.ToString();

			// Links:
			string link = "";
			switch (tracker)
			{
				case TrackerType.Support: link   = "https://sourceforge.net/projects/y-a-terminal/support";    break;
				case TrackerType.Feature: link   = "https://sourceforge.net/p/y-a-terminal/feature-requests/"; break;
				case TrackerType.Bug:
				default:                  link   = "https://sourceforge.net/p/y-a-terminal/bugs/";             break;
			}
			linkLabel_Link.Text = link;
			linkLabel_Link.Links.Add(0, link.Length, link);

			// Instructions:
			sb = new StringBuilder();
			sb.AppendLine("1. [Create Ticket]");
			sb.AppendLine("2. Fill in a title.");
			sb.AppendLine("3. Fill in as much information as possible into the text field:");
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
					sb.AppendLine("    > Expected behavior of the new feature.");
					sb.AppendLine("    > Use case(s) of the new feature.");
					break;
				}

				case TrackerType.Bug:
				default:
				{
					sb.AppendLine("    > YAT version you are using.");
					sb.AppendLine("    > Procedure to reproduce the bug.");
					sb.AppendLine("    > Any useful additional information.");
					break;
				}
			}
			sb.AppendLine("4. Optionally add attachment(s).");
			sb.AppendLine("5. [Save]");
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
