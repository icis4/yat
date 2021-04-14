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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MKY.Windows.Forms.Test
{
	/// <summary></summary>
	public static class MessageBoxesTest
	{
		/// <summary></summary>
		public static void Run(IWin32Window owner)
		{
			var text = "A";

			var caption = "TC00 'Default'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption), DialogResult.OK);

			caption = "TC10 'Min N'"; // "N" = "None" but that would be too wide to keep minimum.
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);

			caption = "TC11 'Min 1'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);

			caption = "TC12 'Min 2'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information), DialogResult.Yes);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information), DialogResult.Yes);

			caption = "TC13 'Min 3'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Information), DialogResult.Abort);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Information), DialogResult.Abort);

			text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmno___"; // Cropped for not exceeding two lines.

			caption = "TC20 'Wide None'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);

			caption = "TC21 'Wide Icon'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);

			text = "A" + Environment.NewLine + "B";

			caption = "TC30 'Dual None'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);

			caption = "TC31 'Dual Icon'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);

			text = "A" + Environment.NewLine + "B" + Environment.NewLine + "C" + Environment.NewLine + "D" + Environment.NewLine + "E" + Environment.NewLine + "F" + Environment.NewLine + "G" + Environment.NewLine + "H";

			caption = "TC40 'Multi None'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);

			caption = "TC41 'Multi Icon'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);

			text = "A";

			caption = "TC51 'Default Button 1'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1), DialogResult.Yes);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1), DialogResult.Yes);

			caption = "TC52 'Default Button 2'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2), DialogResult.No);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2), DialogResult.No);

			caption = "TC53 'Default Button 3'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3), DialogResult.Cancel);
			AssertThat(MessageBoxEx      .Show(owner, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3), DialogResult.Cancel);

			caption = "TC61 'Check Keep'";
			var checkText = "Keep";
			var checkValue = false;
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, checkText, ref checkValue, MessageBoxButtons.OK, MessageBoxIcon.Information), DialogResult.OK);
			AssertThat(checkValue, false);

			text = "A" + Environment.NewLine + "B" + Environment.NewLine + "C" + Environment.NewLine + "D" + Environment.NewLine + "E" + Environment.NewLine + "F" + Environment.NewLine + "G" + Environment.NewLine + "H";

			caption = "TC62 'Check Keep Multi None'";
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, checkText, ref checkValue, MessageBoxButtons.OK, MessageBoxIcon.None), DialogResult.OK);
			AssertThat(checkValue, false);

			caption = "TC63 'Check Change Multi'";
			checkText = "Uncheck before confirming - Uncheck before confirming" + Environment.NewLine + "Uncheck before confirming - Uncheck before confirming" + Environment.NewLine + "Uncheck before confirming - Uncheck before confirming";
			checkValue = true;
			AssertThat(ExtendedMessageBox.Show(owner, text, caption, checkText, ref checkValue, MessageBoxButtons.OK, MessageBoxIcon.Warning), DialogResult.OK);
			AssertThat(checkValue, false);

			caption = "TC71 'Links'";
			var links = new List<LinkLabel.Link>();
			var textBefore = "YAT ";
			var textLink = "home";
			var textAfter = " is a short link and ";
			text = textBefore;
			var linkStart = text.Length;
			text += textLink;
			links.Add(new LinkLabel.Link(linkStart, textLink.Length, "https://sourceforge.net/projects/y-a-terminal/"));
			text += textAfter;
			linkStart = text.Length;
			textLink = "https://sourceforge.net/p/y-a-terminal/code/HEAD/tree/trunk/MKY/MKY.Windows.Forms.Test/ExtendedMessageBoxTest/";
			textAfter = " is a quite long one. And also works for links to the ";
			text += textLink;
			links.Add(new LinkLabel.Link(linkStart, textLink.Length, textLink));
			text += textAfter;
			linkStart = text.Length;
			textLink = "local file system";
			textAfter = ".";
			text += textLink;
			links.Add(new LinkLabel.Link(linkStart, textLink.Length, @"C:\"));
			text += textAfter;
			AssertThat(ExtendedMessageBox.Show(owner, text, links, caption, MessageBoxButtons.OK, MessageBoxIcon.Error), DialogResult.OK);
		}

		private static void AssertThat(DialogResult actual, DialogResult expected)
		{
			Trace.Assert(actual == expected);
		}

		private static void AssertThat(bool actual, bool expected)
		{
			Trace.Assert(actual == expected);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
