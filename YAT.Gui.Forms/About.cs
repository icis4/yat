using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	public partial class About : System.Windows.Forms.Form
	{
		public About()
		{
			InitializeComponent();

			string text = "";
			string textBefore = "";
			string textLink = "";
			string textAfter = "";
			int start = 0;

			// form
			text = "About " + Application.ProductName;
			text += VersionInfo.ProductNamePostFix;
			text += " - Yet Another Terminal";
			Text = text;

			// title
			text = Application.ProductName;
			text += VersionInfo.ProductNamePostFix;
			text += " - Version " + Application.ProductVersion;
			linkLabel_Title.Text = text;

			// copyright
			linkLabel_Copyright.Text = "";
			textBefore = "Copyright © 2003-2004 ";
			textLink =                         "HSR Hochschule für Technik Rapperswil";
			textAfter =                                                             "." + Environment.NewLine +
			             "Copyright © 2003-2007 Matthias Kläy.";
			linkLabel_Copyright.Text += textBefore;
			start = linkLabel_Copyright.Text.Length;
			linkLabel_Copyright.Text += textLink;
			linkLabel_Copyright.Links.Add(start, textLink.Length, "http://www.hsr.ch/");
			linkLabel_Copyright.Text += textAfter;

			// trademark
			linkLabel_Trademark.Text = "All rights reserved.";

			// description
			linkLabel_Description.Text = "";
			textBefore = "YAT is a by-product of the ";
			textLink =                              "Swiss federal KTI/CTI";
			textAfter =                                                  " project 6542.1 FHS-ET \"BBP - Balance Based Pipetting\" between" + Environment.NewLine;
			linkLabel_Description.Text += textBefore;
			start = linkLabel_Description.Text.Length;
			linkLabel_Description.Text += textLink;
			linkLabel_Description.Links.Add(start, textLink.Length, "http://www.bbt.admin.ch/kti/");
			linkLabel_Description.Text += textAfter;

			textBefore = "HSR and ";
			textLink =           "Mettler-Toledo";
			textAfter =                        ". YAT was initially developed as XTerm232 due to the lack of a good RS-232 terminal.";
			linkLabel_Description.Text += textBefore;
			start = linkLabel_Description.Text.Length;
			linkLabel_Description.Text += textLink;
			linkLabel_Description.Links.Add(start, textLink.Length, "http://www.mt.com/");
			linkLabel_Description.Text += textAfter;

			// HHD
			linkLabel_HHD.Text = "";
			textBefore = "YAT is a terminal (a connection endpoint). If you're looking for a tool to monitor serial data between an application" + Environment.NewLine +
						 "and a device, or between two devices, check out ";
			textLink =                                                   "HHD Serial Monitor";
			textAfter =                                                                    ". It's worth the bucks.";
			linkLabel_HHD.Text += textBefore;
			start = linkLabel_HHD.Text.Length;
			linkLabel_HHD.Text += textLink;
			linkLabel_HHD.Links.Add(start, textLink.Length, "http://www.hhdsoftware.com/");
			linkLabel_HHD.Text += textAfter;

			// environment
			linkLabel_Environment.Text = "";
			textBefore = "YAT is developed with" + Environment.NewLine +
			             "   > Microsoft Visual Studio 2005" + Environment.NewLine +
			             "   > IO.Ports serial port extension by Matthias Kläy" + Environment.NewLine +
			             "   > Modified Net.Sockets socket extension ";
			textLink =                                              "ALAZ";
			textAfter =                                                 " by Andre Luis Azevedo";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://www.codeproject.com/cs/internet/AsyncSocketServerandClien.asp");
			linkLabel_Environment.Text += textAfter + Environment.NewLine;

			textBefore = "   > YAT icons based on ";
			textLink =                           "Nuvola";
			textAfter =                                " by David Vignoni";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://icon-king.com/?p=15");
			linkLabel_Environment.Text += textAfter;

			textBefore =                                                " edited in ";
			textLink =                                                             "GIMP";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://www.gimp.org/");
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "   > Toolbar icons from free common set by ";
			textLink =                                              "glyfx";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://www.glyfx.com/");

			// home
			linkLabel_Home.Text = "";
			textBefore = "Visit YAT on ";
			textLink =                "SourceForge.net";
			textAfter =                              ". Tell us if you like it or why you don't.";
			linkLabel_Home.Text += textBefore;
			start = linkLabel_Home.Text.Length;
			linkLabel_Home.Text += textLink;
			linkLabel_Home.Links.Add(start, textLink.Length, "http://sourceforge.net/projects/y-a-terminal/");
			linkLabel_Home.Text += textAfter;

			// author
			linkLabel_Author.Text = "2007, Matthias Kläy";

			// license
			linkLabel_License.Text = "";
			textBefore = "YAT is licensed under the ";
			textLink =                             "CC-GNU GPL";
			textAfter =                                      ".";
			linkLabel_License.Text += textBefore;
			start = linkLabel_License.Text.Length;
			linkLabel_License.Text += textLink;
			linkLabel_License.Links.Add(start, textLink.Length, "http://creativecommons.org/licenses/GPL/2.0/");
			linkLabel_License.Text += textAfter;
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
				Utilities.Net.Browser.BrowseUrl(link);
			}
		}

		#endregion
	}
}
