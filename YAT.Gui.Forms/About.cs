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

using YAT.Utilities;

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

			// Form
			text = "About ";
			text += ApplicationInfo.ProductName;
			text += " - Yet Another Terminal";
			Text = text;

			// Title
			text = ApplicationInfo.ProductName;
			text += " - Version " + Application.ProductVersion;
			linkLabel_Title.Text = text;

			// Copyright
			linkLabel_Copyright.Text = "";
			textBefore = "Copyright © 2003-2004 ";
			textLink =                         "HSR Hochschule für Technik Rapperswil";
			textAfter =                                                             "." + Environment.NewLine +
			             "Copyright © 2003-2010 Matthias Kläy.";
			linkLabel_Copyright.Text += textBefore;
			start = linkLabel_Copyright.Text.Length;
			linkLabel_Copyright.Text += textLink;
			linkLabel_Copyright.Links.Add(start, textLink.Length, "http://www.hsr.ch/");
			linkLabel_Copyright.Text += textAfter;

			// Trademark
			linkLabel_Trademark.Text = "All rights reserved.";

			// Description
			linkLabel_Description.Text = "";
			textBefore = "YAT is a by-product of the ";
			textLink =                              "Swiss federal KTI/CTI";
			textAfter =                                                 @" project 6542.1 FHS-ET ""BBP - Balance Based Pipetting"" between" + Environment.NewLine;
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

			// Platform
			linkLabel_Platform.Text = "For .NET 3.5 on Windows 2000 and later. Currently running on .NET runtime " + System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion() + ".";

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

			// VSPE
			linkLabel_VSPE.Text = "";
			textBefore = "If you're also looking for a tool to create and manage additional virtual COM ports, check out"  + Environment.NewLine;
			textLink =   "Eterlogic Virtual Serial Ports Emulator";
			textAfter =                                          ".";
			linkLabel_VSPE.Text += textBefore;
			start = linkLabel_VSPE.Text.Length;
			linkLabel_VSPE.Text += textLink;
			linkLabel_VSPE.Links.Add(start, textLink.Length, "http://www.eterlogic.com/Products.VSPE.html");
			linkLabel_VSPE.Text += textAfter;

			// Environment
			linkLabel_Environment.Text = "";
			textBefore = "YAT is developed with" + Environment.NewLine +
						 "   > Microsoft Visual Studio 2008" + Environment.NewLine +
						 "   > AnkhSVN" + Environment.NewLine +
						 "   > NUnit" + Environment.NewLine +
						 "   > System.IO.Ports serial port extension by Matthias Kläy" + Environment.NewLine +

						 "   > System.Net.Sockets socket extension ";
			textLink =                                            "ALAZ";
			textAfter =                                               " by Andre Luis Azevedo";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://www.codeproject.com/cs/internet/AsyncSocketServerandClien.asp");
			linkLabel_Environment.Text += textAfter + Environment.NewLine;

			textBefore = "   > USB HID library based on ";
			textLink =                                 "GenericHid";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://www.lvr.com/hidpage.htm");

			textBefore =                                         "/";
			textLink =                                            "UsbLibrary";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://www.codeproject.com/KB/cs/USB_HID.aspx?msg=2816038");

			textBefore =                                                    "/";
			textLink =                                                       "UsbHid";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://www.florian-leitner.de/index.php/category/usb-hid-library/");

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
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "   > Terminal font ";
			textLink = "DejaVu";
			linkLabel_Environment.Text += textBefore;
			start = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(start, textLink.Length, "http://dejavu.sourceforge.net/");

			// Home
			linkLabel_Home.Text = "";
			textBefore = "Visit YAT on ";
			textLink =                "SourceForge.net";
			textAfter =                              ". Tell us if you like it or why you don't.";
			linkLabel_Home.Text += textBefore;
			start = linkLabel_Home.Text.Length;
			linkLabel_Home.Text += textLink;
			linkLabel_Home.Links.Add(start, textLink.Length, "http://sourceforge.net/projects/y-a-terminal/");
			linkLabel_Home.Text += textAfter;

			// Author
			linkLabel_Author.Text = "2009, Matthias Kläy";

			// License
			linkLabel_License.Text = "";
			textBefore = "YAT is licensed under the ";
			textLink =                             "GNU LGPL";
			textAfter =                                    ".";
			linkLabel_License.Text += textBefore;
			start = linkLabel_License.Text.Length;
			linkLabel_License.Text += textLink;
			linkLabel_License.Links.Add(start, textLink.Length, "http://www.gnu.org/licenses/lgpl.html");
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
				MKY.Utilities.Net.Browser.BrowseUrl(link);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
