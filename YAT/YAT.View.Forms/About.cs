﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY.Windows.Forms;

#endregion

#region Module-level StyleCop suppressions
//==================================================================================================
// Module-level StyleCop suppressions
//==================================================================================================

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Large blocks of module-level FxCop suppressions which were copy-pasted out of FxCop.")]

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Azevedo")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Bies")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Compu")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Deja")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Eterlogic")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Haftmann")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Henrik")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Kläy")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Lammert")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Libre")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Ltech")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Mettler-Toledo")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Nuvola")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Pipetting")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "unidir")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.About.resources", MessageId = "Vignoni")]
#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class About : Form
	{
		#region Information and Links
		//==========================================================================================
		// Information and Links
		//==========================================================================================

		/// <summary></summary>
		public About()
		{
			InitializeComponent();

			string textBefore = "";
			string textLink = "";
			string textAfter = "";
			int linkStart = 0;

			// Form:
			Text = ApplicationEx.CommonNameLong; // Fixed to "YAT - Yet Another Terminal".

			// Title:
			linkLabel_Title.Text = ApplicationEx.ProductCaptionAndVersion;

			// Copyright:
			linkLabel_Copyright.Text = "";
			textBefore = "Copyright © 2003-2004 ";
			textLink   =                       "HSR Hochschule für Technik Rapperswil";
			textAfter  =                                                            "." + Environment.NewLine +
			             "Copyright © 2003-2019 Matthias Kläy.";
			linkLabel_Copyright.Text += textBefore;
			linkStart = linkLabel_Copyright.Text.Length;
			linkLabel_Copyright.Text += textLink;
			linkLabel_Copyright.Links.Add(linkStart, textLink.Length, "http://www.hsr.ch/");
			linkLabel_Copyright.Text += textAfter;

			// Trademark:
			linkLabel_Trademark.Text = "All rights reserved.";

			// Description:
			linkLabel_Description.Text = "";
			textBefore = "YAT is a by-product of the ";
			textLink   =                            "Swiss federal KTI/CTI";
			textAfter  =                                                @" project 6542.1 FHS-ET ""BBP - Balance Based Pipetting"" of HSR" + Environment.NewLine;
			linkLabel_Description.Text += textBefore;
			linkStart = linkLabel_Description.Text.Length;
			linkLabel_Description.Text += textLink;
			linkLabel_Description.Links.Add(linkStart, textLink.Length, "http://www.bbt.admin.ch/kti/");
			linkLabel_Description.Text += textAfter;

			textBefore = "and ";
			textLink   =     "Mettler-Toledo";
			textAfter  =                   ". YAT was initially developed as XTerm232, a response to the lack of a good RS-232 terminal.";
			linkLabel_Description.Text += textBefore;
			linkStart = linkLabel_Description.Text.Length;
			linkLabel_Description.Text += textLink;
			linkLabel_Description.Links.Add(linkStart, textLink.Length, "http://www.mt.com/");
			linkLabel_Description.Text += textAfter;

			// Platform:
			linkLabel_Platform.Text = "For " + ApplicationEx.PrerequisiteFramework + " on " + ApplicationEx.PrerequisiteWindowsOS + /* " or " + ApplicationEx.PrerequisiteOtherOS + */ "." + Environment.NewLine +
			                          "Currently running on .NET Runtime " + Environment.Version + " (CLR version).";

			// Serial monitoring:
			linkLabel_Monitoring.Text = "";
			textBefore = "YAT is a terminal (connection endpoint). If you are looking for a tool to monitor serial data between an application and" + Environment.NewLine +
			             "a device, or between two devices, check out ";
			textLink   =                                             "HHD Monitoring Studio";
			textAfter  =                                                                  ". It's worth the bucks. Or download the ";
			linkLabel_Monitoring.Text += textBefore;
			linkStart = linkLabel_Monitoring.Text.Length;
			linkLabel_Monitoring.Text += textLink;
			linkLabel_Monitoring.Links.Add(linkStart, textLink.Length, "http://www.hhdsoftware.com/");
			linkLabel_Monitoring.Text += textAfter;
			textLink  =                                                                                                           "free edition";
			textAfter =                                                                                                                       "." + Environment.NewLine +
			            "Alternatively, obtain a non-intrusive monitor/sniffer/spy cable e.g. ";
			linkStart = linkLabel_Monitoring.Text.Length;
			linkLabel_Monitoring.Text += textLink;
			linkLabel_Monitoring.Links.Add(linkStart, textLink.Length, "http://freeserialanalyzer.com/");
			linkLabel_Monitoring.Text += textAfter;
			textLink  =                                                                      "EZ-Tap";
			textAfter =                                                                            ", or, assemble your own cable as described" + Environment.NewLine +
			            "by e.g. ";
			linkStart = linkLabel_Monitoring.Text.Length;
			linkLabel_Monitoring.Text += textLink;
			linkLabel_Monitoring.Links.Add(linkStart, textLink.Length, "https://www.stratusengineering.com/product/ez-tap/");
			linkLabel_Monitoring.Text += textAfter;
			textLink  =         "Henrik Haftmann";
			textAfter =                        ", ";
			linkStart = linkLabel_Monitoring.Text.Length;
			linkLabel_Monitoring.Text += textLink;
			linkLabel_Monitoring.Links.Add(linkStart, textLink.Length, "https://www-user.tu-chemnitz.de/~heha/basteln/PC/serspy/");
			linkLabel_Monitoring.Text += textAfter;
			textLink  =                          "Lammert Bies";
			textAfter =                                      " or ";
			linkStart = linkLabel_Monitoring.Text.Length;
			linkLabel_Monitoring.Text += textLink;
			linkLabel_Monitoring.Links.Add(linkStart, textLink.Length, "https://www.lammertbies.nl/comm/cable/RS-232-spy-monitor.html");
			linkLabel_Monitoring.Text += textAfter;
			textLink  =                                          "CompuPhase";
			textAfter =                                                    " (bidir or unidir, full or half duplex, with or without control lines).";
			linkStart = linkLabel_Monitoring.Text.Length;
			linkLabel_Monitoring.Text += textLink;
			linkLabel_Monitoring.Links.Add(linkStart, textLink.Length, "http://www.compuphase.com/electronics/rs232split.htm");
			linkLabel_Monitoring.Text += textAfter;

			// Virtual serial ports:
			linkLabel_VirtualPorts.Text = "";
			textBefore = "If you're looking for a tool to create and manage virtual COM ports, check out ";
			textLink   =                                                                                "Eterlogic VSPE";
			textAfter  =                                                                                              ", ";
			linkLabel_VirtualPorts.Text += textBefore;
			linkStart = linkLabel_VirtualPorts.Text.Length;
			linkLabel_VirtualPorts.Text += textLink;
			linkLabel_VirtualPorts.Links.Add(linkStart, textLink.Length, "http://www.eterlogic.com/Products.VSPE.html");
			linkLabel_VirtualPorts.Text += textAfter;
			textLink  =                                                                                                 "TALtech TCP/Com";
			textAfter =                                                                                                                ", or" + Environment.NewLine;
			linkStart = linkLabel_VirtualPorts.Text.Length;
			linkLabel_VirtualPorts.Text += textLink;
			linkLabel_VirtualPorts.Links.Add(linkStart, textLink.Length, "http://www.taltech.com/tcpcom");
			linkLabel_VirtualPorts.Text += textAfter;
			textBefore = "the open source ";
			textLink   =                 "com0com+hub4com";
			textAfter  =                                ". They all support mapping COM to TCP/IP, port sharing, virtually connected ports,...";
			linkLabel_VirtualPorts.Text += textBefore;
			linkStart = linkLabel_VirtualPorts.Text.Length;
			linkLabel_VirtualPorts.Text += textLink;
			linkLabel_VirtualPorts.Links.Add(linkStart, textLink.Length, "https://sourceforge.net/projects/com0com/");
			linkLabel_VirtualPorts.Text += textAfter;

			// Terminal emulator:
			linkLabel_TerminalEmulator.Text = "";
			textBefore = "YAT is optimized for simple command sets of e.g. embedded systems. If you are looking for a terminal emulator," + Environment.NewLine +
			             "rather go for ";
			textLink   =               "PuTTY";
			textAfter  =                    ", ";
			linkLabel_TerminalEmulator.Text += textBefore;
			linkStart = linkLabel_TerminalEmulator.Text.Length;
			linkLabel_TerminalEmulator.Text += textLink;
			linkLabel_TerminalEmulator.Links.Add(linkStart, textLink.Length, "https://www.putty.org/");
			linkLabel_TerminalEmulator.Text += textAfter;
			textLink   =                      "Tera Term";
			textAfter  =                               ", ";
			linkStart = linkLabel_TerminalEmulator.Text.Length;
			linkLabel_TerminalEmulator.Text += textLink;
			linkLabel_TerminalEmulator.Links.Add(linkStart, textLink.Length, "https://osdn.net/projects/ttssh2/");
			linkLabel_TerminalEmulator.Text += textAfter;
			textLink   =                                 "SecureCRT";
			textAfter  =                                          " or the like.";
			linkStart = linkLabel_TerminalEmulator.Text.Length;
			linkLabel_TerminalEmulator.Text += textLink;
			linkLabel_TerminalEmulator.Links.Add(linkStart, textLink.Length, "https://www.vandyke.com/products/securecrt/");
			linkLabel_TerminalEmulator.Text += textAfter;

			// Environment:
			linkLabel_Environment.Text = "YAT is developed with..." + Environment.NewLine;

			textBefore = "...Microsoft ";
			textLink   =              "Visual Studio Community Edition";
			textAfter  =                                             "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "https://www.visualstudio.com/vs/community/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...";
			textLink   =    "NUnit";
			textAfter  =         "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.nunit.org/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...";
			textLink   =    "AnkhSVN";
			textAfter  =           "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "https://ankhsvn.open.collab.net/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...";
			textLink   =    "GhostDoc";
			textAfter  =            "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://submain.com/products/ghostdoc.aspx");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...System.IO.Ports serial COM port extension by Matthias Kläy..." + Environment.NewLine +

			             "...System.Net.Sockets socket extension ";
			textLink   =                                        "ALAZ";
			textAfter  =                                            " by Andre Luis Azevedo...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "https://www.codeproject.com/Articles/14155/An-Asynchronous-Socket-Server-and-Client");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...USB Ser/HID library by Matthias Kläy based on ";
			textLink   =                                                  "GenericHid";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://janaxelson.com/hidpage.htm");
			textBefore =                                                            "/";
			textLink   =                                                             "UsbLibrary";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.codeproject.com/KB/cs/USB_HID.aspx?msg=2816038");
			textBefore =                                                                       "/";
			textLink   =                                                                        "UsbHid";
			textAfter  =                                                                              "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.florian-leitner.de/index.php/category/usb-hid-library/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...YAT icons based on ";
			textLink   =                       "Nuvola";
			textAfter  =                             " by David Vignoni";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://icon-king.com/?p=15");
			linkLabel_Environment.Text += textAfter;
			textBefore =                                               " edited in ";
			textLink   =                                                          "GIMP";
			textAfter  =                                                              "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.gimp.org/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...";
			textLink   =    "FatCow";
			textAfter  =          " icons...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "http://www.fatcow.com/free-icons/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...terminal font ";
			textLink   =                  "DejaVu";
			textAfter  =                        "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "https://dejavu-fonts.github.io/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...";
			textLink   =    "RTF writer";
			textAfter  =              " by Matt Buckley and Thomson Reuters...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "https://sourceforge.net/projects/netrtfwriter/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...hosting and change management on ";
			textLink   =                                     "SourceForge.net";
			textAfter  =                                                    "...";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "https://sourceforge.net/");
			linkLabel_Environment.Text += textAfter;
			linkLabel_Environment.Text += Environment.NewLine;

			textBefore = "...documentation, test and release management using ";
			textLink   =                                                     "LibreOffice";
			textAfter  =                                                                ".";
			linkLabel_Environment.Text += textBefore;
			linkStart = linkLabel_Environment.Text.Length;
			linkLabel_Environment.Text += textLink;
			linkLabel_Environment.Links.Add(linkStart, textLink.Length, "https://www.documentfoundation.org/");
			linkLabel_Environment.Text += textAfter;

			// Home:
			linkLabel_Home.Text = "";
			textBefore = "Visit YAT at ";
			textLink   =              "SourceForge.net";
			linkLabel_Home.Text += textBefore;
			linkStart = linkLabel_Home.Text.Length;
			linkLabel_Home.Text += textLink;
			linkLabel_Home.Links.Add(linkStart, textLink.Length, "http://sourceforge.net/projects/y-a-terminal/");
			textBefore =                             ", or contact ";
			textLink   =                                          "y-a-terminal@users.sourceforge.net";
			textAfter  =                                                                            ". Feedback is welcome.";
			linkLabel_Home.Text += textBefore;
			linkStart = linkLabel_Home.Text.Length;
			linkLabel_Home.Text += textLink;
			linkLabel_Home.Links.Add(linkStart, textLink.Length, "mailto:y-a-terminal@users.sourceforge.net");
			linkLabel_Home.Text += textAfter;

			// Author:
			linkLabel_Author.Text = "2019, Matthias Kläy";

			// License:
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

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(Parent, e);
		}

		#endregion

		#region Manual Testing
		//==========================================================================================
		// Manual Testing
		//==========================================================================================

		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Intentionally raising the most general exception to ensure that EVERY exception handler really catches it.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void label_ExecuteManualTest1_Click(object sender, EventArgs e)
		{
			string message =
				"You have clicked on a hidden button that is used for YAT internal testing in 'Release' configuration." + Environment.NewLine + Environment.NewLine +
				"Would you like to immediately throw an exception to test that unhandled synchronous exceptions are handled properly?";

			if (TestExecutionIsIntended(message) &&
				TestPreconditionIsGiven(typeof(Exception)))
			{
				Cursor = Cursors.WaitCursor; // Verify that cursor is reset by the unhandled exception handler.

				throw (new InvalidOperationException("Unhandled synchronous exception test :: This is the outer exception.", new InvalidOperationException("This is the inner exception.")));

				// Using explicit exception types...
				// ...since no general exceptions should ever be thrown...
				// ...and since exception handling uses the general exception type to prevent subsequent exceptions.
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void label_ExecuteManualTest2_Click(object sender, EventArgs e)
		{
			string message =
				"You have clicked on a hidden button that is used for YAT internal testing in 'Release' configuration." + Environment.NewLine + Environment.NewLine +
				"Would you like to start a Windows.Forms timer throwing an exception to test that unhandled asynchronous synchronized exceptions are handled properly?";

			if (TestExecutionIsIntended(message) &&
				TestPreconditionIsGiven(typeof(Exception)))
			{
				Cursor = Cursors.WaitCursor; // Verify that cursor is reset by the unhandled exception handler.

				timer_ExecuteManualTest2.Start();
			}
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Intentionally raising the most general exception to ensure that EVERY exception handler really catches it.")]
		private void timer_ExecuteManualTest2_Tick(object sender, EventArgs e)
		{
			timer_ExecuteManualTest2.Stop();
			throw (new InvalidOperationException("Unhandled asynchronous synchronized exception test :: This is the outer exception.", new InvalidOperationException("This is the inner exception.")));

			// Using explicit exception types...
			// ...since no general exceptions should ever be thrown...
			// ...and since exception handling uses the general exception type to prevent subsequent exceptions.
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private System.Threading.Timer timer_ExecuteManualTest3; // Explicitly using 'System.Threading.Timer' to prevent naming conflict with 'System.Windows.Forms.Timer'.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object timer_ExecuteManualTest3SyncObj = new object();

		/// <summary>
		/// Test case 3: Unhandled asynchronous non-synchronized exceptions.
		/// </summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void label_ExecuteManualTest3_Click(object sender, EventArgs e)
		{
			string message =
				"You have clicked on a hidden button that is used for YAT internal testing in 'Release' configuration." + Environment.NewLine + Environment.NewLine +
				"Would you like to start a System.Threading timer throwing an exception to test that unhandled asynchronous non-synchronized exceptions are handled properly?";

			if (TestExecutionIsIntended(message) &&
				TestPreconditionIsGiven(typeof(Exception)))
			{
				Cursor = Cursors.WaitCursor; // Verify that cursor is reset by the unhandled exception handler.

				lock (this.timer_ExecuteManualTest3SyncObj)
				{                  // Explicitly using 'System.Threading.Timer' to prevent naming conflict with 'System.Windows.Forms.Timer'.
					this.timer_ExecuteManualTest3 = new System.Threading.Timer(new System.Threading.TimerCallback(timer_ExecuteManualTest3_Timeout), null, 100, System.Threading.Timeout.Infinite);
				}
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Intentionally raising the most general exception to ensure that EVERY exception handler really catches it.")]
		private void timer_ExecuteManualTest3_Timeout(object obj)
		{
			// Non-periodic timer, only a single timeout event thread can be active at a time.
			// There is no need to synchronize callbacks to this event handler.

			timer_ExecuteManualTest3_Dispose();
			throw (new InvalidOperationException("Unhandled asynchronous non-synchronized exception test :: This is the outer exception.", new InvalidOperationException("This is the inner exception.")));

			// Using explicit exception types...
			// ...since no general exceptions should ever be thrown...
			// ...and since exception handling uses the general exception type to prevent subsequent exceptions.
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

		private bool TestExecutionIsIntended(string message)
		{
			var dr = MessageBoxEx.Show
			(
				this,
				message,
				"Execute Manual 'Release' Test?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr == DialogResult.Yes);
		}

		private bool TestPreconditionIsGiven(Type upcomingExceptionType)
		{
			if (UnhandledExceptionHandler.ExceptionTypeIsIgnored(upcomingExceptionType))
			{
				string precondition =
					"The precondition for the test is not given. The upcoming exception type is currently being ignored." + Environment.NewLine + Environment.NewLine +
					"Confirm with [OK] to restore the precondition, or [Cancel] the test.";

				var dr = MessageBoxEx.Show
				(
					this,
					precondition,
					"Restore Manual 'Release' Test Precondition?",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Exclamation
				);

				if (dr != DialogResult.OK)
					return (false);

				UnhandledExceptionHandler.RevertIgnoredExceptionType(upcomingExceptionType);
			}

			return (true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
