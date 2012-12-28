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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Utilities;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", Scope = "resource", Target = "YAT.Gui.Forms.About.resources", MessageId = "Toolbar", Justification = "What's wrong with 'Toolbar'? The web is full with this term!")]

#endregion

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class About : System.Windows.Forms.Form
	{
		/// <summary></summary>
		public About()
		{
			InitializeComponent();

			string textBefore = "";
			string textLink = "";
			string textAfter = "";
			int linkStart = 0;

			// Form.
			Text = ApplicationInfo.ProductNameLong;

			// Title.
			linkLabel_Title.Text = ApplicationInfo.ProductNameAndBuildNameAndVersion;

			// Copyright.
			linkLabel_Copyright.Text = "";
			textBefore = "Copyright © 2003-2004 ";
			textLink   =                       "HSR Hochschule für Technik Rapperswil";
			textAfter  =                                                            "." + Environment.NewLine +
			             "Copyright © 2003-2012 Matthias Kläy.";
			linkLabel_Copyright.Text += textBefore;
			linkStart = linkLabel_Copyright.Text.Length;
			linkLabel_Copyright.Text += textLink;
			linkLabel_Copyright.Links.Add(linkStart, textLink.Length, "http://www.hsr.ch/");
			linkLabel_Copyright.Text += textAfter;

			// Trademark.
			linkLabel_Trademark.Text = "All rights reserved.";

			// Description.
			linkLabel_Description.Text = "";
			textBefore = "YAT is a by-product of the ";
			textLink   =                            "Swiss federal KTI/CTI";
			textAfter  =                                                @" project 6542.1 FHS-ET ""BBP - Balance Based Pipetting"" between" + Environment.NewLine;
			linkLabel_Description.Text += textBefore;
			linkStart = linkLabel_Description.Text.Length;
			linkLabel_Description.Text += textLink;
			linkLabel_Description.Links.Add(linkStart, textLink.Length, "http://www.bbt.admin.ch/kti/");
			linkLabel_Description.Text += textAfter;

			textBefore = "HSR and ";
			textLink   =         "Mettler-Toledo";
			textAfter  =                       ". YAT was initially developed as XTerm232 due to the lack of a good RS-232 terminal.";
			linkLabel_Description.Text += textBefore;
			linkStart = linkLabel_Description.Text.Length;
			linkLabel_Description.Text += textLink;
			linkLabel_Description.Links.Add(linkStart, textLink.Length, "http://www.mt.com/");
			linkLabel_Description.Text += textAfter;

			// Platform.
			linkLabel_Platform.Text = "For .NET framework 3.5 on Windows 2000 and later. Currently running on .NET runtime " + Environment.Version + " (CLR version).";

			// HHD.
			linkLabel_HHD.Text = "";
			textBefore = "YAT is a terminal (a connection endpoint). If you're looking for a tool to monitor serial data between an application and a" + Environment.NewLine +
						 "device, or between two devices, check out ";
			textLink   =                                           "HHD Monitoring Studio";
			textAfter  =                                                                ". It's worth the bucks. Or ";
			linkLabel_HHD.Text += textBefore;
			linkStart = linkLabel_HHD.Text.Length;
			linkLabel_HHD.Text += textLink;
			linkLabel_HHD.Links.Add(linkStart, textLink.Length, "http://www.hhdsoftware.com/");
			linkLabel_HHD.Text += textAfter;
			textLink =                                                                                             "download the free edition";
			textAfter =                                                                                                                     ".";
			linkStart = linkLabel_HHD.Text.Length;
			linkLabel_HHD.Text += textLink;
			linkLabel_HHD.Links.Add(linkStart, textLink.Length, "http://www.serial-port-monitor.com/");
			linkLabel_HHD.Text += textAfter;

			// VSPE.
			linkLabel_VSPE.Text = "";
			textBefore = "If you're also looking for a tool to create and manage additional virtual COM ports, check out"  + Environment.NewLine;
			textLink   = "Eterlogic Virtual Serial Ports Emulator";
			textAfter  =                                         ". Supports virtual connected ports, mapping to TCP/IP, port sharing,...";
			linkLabel_VSPE.Text += textBefore;
			linkStart = linkLabel_VSPE.Text.Length;
			linkLabel_VSPE.Text += textLink;
			linkLabel_VSPE.Links.Add(linkStart, textLink.Length, "http://www.eterlogic.com/Products.VSPE.html");
			linkLabel_VSPE.Text += textAfter;

			// Environment.
			linkLabel_Environment.Text = "";
			textBefore = "YAT is developed with" + Environment.NewLine +
						 "   > Microsoft Visual Studio 2008" + Environment.NewLine +
						 "   > AnkhSVN" + Environment.NewLine +
						 "   > NUnit" + Environment.NewLine +
						 "   > System.IO.Ports serial port extension by Matthias Kläy" + Environment.NewLine +

						 "   > System.Net.Sockets socket extension ";
			textLink   =                                          "ALAZ";
			textAfter  =                                              " by Andre Luis Azevedo";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.codeproject.com/cs/internet/AsyncSocketServerandClien.asp");
			linkLabel_Environment.Text += textAfter + Environment.NewLine;

			textBefore = "   > USB Ser/HID library based on ";
			textLink   =                                   "GenericHid";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.lvr.com/hidpage.htm");

			textBefore =                                             "/";
			textLink   =                                              "UsbLibrary";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.codeproject.com/KB/cs/USB_HID.aspx?msg=2816038");

			textBefore =                                                        "/";
			textLink   =                                                         "UsbHid";
			textAfter  =                                                               " by Matthias Kläy";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.florian-leitner.de/index.php/category/usb-hid-library/");
			linkLabel_Environment.Text += textAfter + Environment.NewLine;

			textBefore = "   > YAT icons based on ";
			textLink   =                         "Nuvola";
			textAfter  =                               " by David Vignoni";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://icon-king.com/?p=15");
			linkLabel_Environment.Text += textAfter;

			textBefore =                                                " edited in ";
			textLink   =                                                           "GIMP";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.gimp.org/");
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "   > Toolbar icons from free common set by ";
			textLink   =                                            "glyfx";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.glyfx.com/");
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "   > Terminal font ";
			textLink   =                    "DejaVu";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://dejavu.sourceforge.net/");
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "   > Microsoft StyleCop" + Environment.NewLine +
						 "   > Microsoft FxCop";
			linkLabel_Environment.Text += textBefore;

			// Home.
			linkLabel_Home.Text = "";
			textBefore = "Visit YAT on ";
			textLink   =              "SourceForge.net";
			textAfter  =                             ". Tell us if you like it or why you don't.";
			linkLabel_Home.Text += textBefore;
			linkStart = linkLabel_Home.Text.Length;
			linkLabel_Home.Text += textLink;
			linkLabel_Home.Links.Add(linkStart, textLink.Length, "http://sourceforge.net/projects/y-a-terminal/");
			linkLabel_Home.Text += textAfter;

			// Author.
			linkLabel_Author.Text = "2012, Matthias Kläy";

			// License.
			linkLabel_License.Text = "";
			textBefore = "YAT is licensed under the ";
			textLink   =                           "GNU LGPL";
			textAfter  =                                   ".";
			linkLabel_License.Text += textBefore;
			linkStart = linkLabel_License.Text.Length;
			linkLabel_License.Text += textLink;
			linkLabel_License.Links.Add(linkStart, textLink.Length, "http://www.gnu.org/licenses/lgpl.html");
			linkLabel_License.Text += textAfter;
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

		#region Controls Event Handlers > Manual Testing
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Manual Testing
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Intentionally raising the most general exception to ensure that EVERY exception handler really catches it.")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void label_ExecuteManualTest1_Click(object sender, EventArgs e)
		{
			string message =
				"You have clicked on a hidden button that is used for YAT internal testing in 'Release' configuration." + Environment.NewLine + Environment.NewLine +
				"Would you like to immediately throw an exception to test that unhandled synchronous exceptions are handled properly?";

			if (MessageBoxEx.Show
				(
				this,
				message,
				"Execute manual 'Release' test?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				throw (new Exception("Unhandled synchronous exception test :: Outer exception", new Exception("Inner exception")));
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void label_ExecuteManualTest2_Click(object sender, EventArgs e)
		{
			string message =
				"You have clicked on a hidden button that is used for YAT internal testing in 'Release' configuration." + Environment.NewLine + Environment.NewLine +
				"Would you like to start a Windows.Forms timer throwing an exception to test that unhandled asynchronous synchronized exceptions are handled properly?";

			if (MessageBoxEx.Show
				(
				this,
				message,
				"Execute manual 'Release' test?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				timer_ExecuteManualTest2.Start();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Intentionally raising the most general exception to ensure that EVERY exception handler really catches it.")]
		private void timer_ExecuteManualTest2_Tick(object sender, EventArgs e)
		{
			timer_ExecuteManualTest2.Stop();
			throw (new Exception("Unhandled asynchronous synchronized exception test :: Outer exception", new Exception("Inner exception")));
		}

		private System.Threading.Timer timer_ExecuteManualTest3;
		private object timer_ExecuteManualTest3SyncObj = new object();

		private event EventHandler ExecuteManualTest3Event;

		/// <summary>
		/// Test case 3: Unhandled asynchronous non-synchronized exceptions.
		/// 
		/// Test case 3 doesn't work on a System.Threading timer callback directly.
		/// Such exceptions are not dispatched back onto main thread. Therefore, use
		/// EventHelper and a separate exception class for this test case.
		/// </summary>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void label_ExecuteManualTest3_Click(object sender, EventArgs e)
		{
			string message =
				"You have clicked on a hidden button that is used for YAT internal testing in 'Release' configuration." + Environment.NewLine + Environment.NewLine +
				"Would you like to start a System.Threading timer throwing an exception to test that unhandled asynchronous non-synchronized exceptions are handled properly?";

			if (MessageBoxEx.Show
				(
				this,
				message,
				"Execute manual 'Release' test?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
				)
				== DialogResult.Yes)
			{
				lock (this.timer_ExecuteManualTest3SyncObj)
				{
					this.timer_ExecuteManualTest3 = new System.Threading.Timer(new System.Threading.TimerCallback(timer_ExecuteManualTest3_Timeout), null, 100, System.Threading.Timeout.Infinite);
				}
			}
		}

		private void timer_ExecuteManualTest3_Timeout(object obj)
		{
			// Immediately dispose of the timer:
			timer_ExecuteManualTest3_Dispose();

			// Create event sink that acts as exception source:
			ExecuteManualTest3Class exceptionSource = new ExecuteManualTest3Class();
			ExecuteManualTest3Event += new EventHandler(exceptionSource.Execute);

			// Fire event synchronously:
			EventHelper.FireSync(ExecuteManualTest3Event, this, new EventArgs());
		}

		private void timer_ExecuteManualTest3_Dispose()
		{
			lock (this.timer_ExecuteManualTest3SyncObj)
			{
				if (this.timer_ExecuteManualTest3 != null)
				{
					this.timer_ExecuteManualTest3.Dispose();
					this.timer_ExecuteManualTest3 = null;
				}
			}
		}

		private class ExecuteManualTest3Class
		{
			public ExecuteManualTest3Class()
			{
			}

			[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Intentionally raising the most general exception to ensure that EVERY exception handler really catches it.")]
			public virtual void Execute(object sender, EventArgs e)
			{
				throw (new Exception("Unhandled asynchronous non-synchronized exception test :: Outer exception", new Exception("Inner exception")));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
