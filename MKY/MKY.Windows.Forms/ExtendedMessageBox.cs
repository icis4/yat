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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using MKY.Collections;

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Extends <see cref="MessageBox"/> with optional checkbox and link label.
	/// </summary>
	/// <remarks><para>
	/// The API follows the API of <see cref="MessageBox"/>, i.e. based on static methods.
	/// </para><para>
	/// The layout best follows the layout of a <see cref="MessageBox"/>:
	/// <list type="bullet">
	/// <item><description>Buttons are right-aligned.</description></item>
	/// <item><description>Minimum height is 133 pixels (on Win10 screen) = 140 pixels (in Visual Studio designer).</description></item>
	/// <item><description>Minimum width is 124 pixels (on Win10 screen) = 126 pixels (in Visual Studio designer).</description></item>
	/// <item><description>Maximum width is 412 pixels (on Win10 screen) = 426 pixels (in Visual Studio designer).</description></item>
	/// </list></para><para>
	/// <see cref="Control.RightToLeft"/> is not supported.
	/// </para></remarks>
	public partial class ExtendedMessageBox : Form
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Dictionary<MessageBoxButtons, int>                         staticButtonCountLookup;
		private static Dictionary<MessageBoxButtons, Tuple<string, DialogResult>> staticButtonALookup;
		private static Dictionary<MessageBoxButtons, Tuple<string, DialogResult>> staticButtonBLookup;
		private static Dictionary<MessageBoxButtons, Tuple<string, DialogResult>> staticButtonCLookup;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic, and anyway, performance isn't an issue here.")]
		static ExtendedMessageBox()
		{
			staticButtonCountLookup = new Dictionary<MessageBoxButtons, int>(6); // Preset the required capacity to improve memory management.
			staticButtonCountLookup.Add(MessageBoxButtons.OK,               1);
			staticButtonCountLookup.Add(MessageBoxButtons.OKCancel,         2);
			staticButtonCountLookup.Add(MessageBoxButtons.AbortRetryIgnore, 3);
			staticButtonCountLookup.Add(MessageBoxButtons.YesNoCancel,      3);
			staticButtonCountLookup.Add(MessageBoxButtons.YesNo,            2);
			staticButtonCountLookup.Add(MessageBoxButtons.RetryCancel,      2);

			// A is the right-most button and always visible:
			staticButtonALookup = new Dictionary<MessageBoxButtons, Tuple<string, DialogResult>>(6); // Preset the required capacity to improve memory management.
			staticButtonALookup.Add(MessageBoxButtons.OK,               new Tuple<string, DialogResult>("OK",      DialogResult.OK));     // [OK] and standard
			staticButtonALookup.Add(MessageBoxButtons.OKCancel,         new Tuple<string, DialogResult>("Cancel",  DialogResult.Cancel)); // [Cancel] do not have
			staticButtonALookup.Add(MessageBoxButtons.AbortRetryIgnore, new Tuple<string, DialogResult>("&Ignore", DialogResult.Ignore)); // & on a "MessageBox".
			staticButtonALookup.Add(MessageBoxButtons.YesNoCancel,      new Tuple<string, DialogResult>("&Cancel", DialogResult.Cancel));
			staticButtonALookup.Add(MessageBoxButtons.YesNo,            new Tuple<string, DialogResult>("&No",     DialogResult.No));
			staticButtonALookup.Add(MessageBoxButtons.RetryCancel,      new Tuple<string, DialogResult>("&Cancel",  DialogResult.Cancel));

			staticButtonBLookup = new Dictionary<MessageBoxButtons, Tuple<string, DialogResult>>(5); // Preset the required capacity to improve memory management.
			staticButtonBLookup.Add(MessageBoxButtons.OKCancel,         new Tuple<string, DialogResult>("OK",      DialogResult.OK));
			staticButtonBLookup.Add(MessageBoxButtons.AbortRetryIgnore, new Tuple<string, DialogResult>("&Retry",  DialogResult.Retry));
			staticButtonBLookup.Add(MessageBoxButtons.YesNoCancel,      new Tuple<string, DialogResult>("&No",     DialogResult.No));
			staticButtonBLookup.Add(MessageBoxButtons.YesNo,            new Tuple<string, DialogResult>("&Yes",    DialogResult.Yes));
			staticButtonBLookup.Add(MessageBoxButtons.RetryCancel,      new Tuple<string, DialogResult>("&Retry",  DialogResult.Retry));

			staticButtonCLookup = new Dictionary<MessageBoxButtons, Tuple<string, DialogResult>>(2); // Preset the required capacity to improve memory management.
			staticButtonCLookup.Add(MessageBoxButtons.AbortRetryIgnore, new Tuple<string, DialogResult>("&Abort",  DialogResult.Abort));
			staticButtonCLookup.Add(MessageBoxButtons.YesNoCancel,      new Tuple<string, DialogResult>("&Yes",    DialogResult.Yes));
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default
		/// button and optional checkbox and link label.
		/// </summary>
		/// <param name="owner">
		/// An implementation of <see cref="IWin32Window"/> that will own the modal dialog box.
		/// </param>
		/// <param name="text">
		/// The text to display in the message box.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="buttons">
		/// One of the <see cref="MessageBoxButtons"/> values that specifies which buttons to display in the message box.
		/// </param>
		/// <param name="icon">
		/// One of the <see cref="MessageBoxIcon"/> values that specifies which icon to display in the message box.
		/// </param>
		/// <param name="defaultButton">
		/// One of the <see cref="MessageBoxDefaultButton"/> values that specifies the default button for the message box.
		/// </param>
		/// <returns>
		/// One of the <see cref="DialogResult"/> values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			return (Show(owner, text, null, caption, buttons, icon, defaultButton));
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default
		/// button and optional checkbox and link label.
		/// </summary>
		/// <param name="owner">
		/// An implementation of <see cref="IWin32Window"/> that will own the modal dialog box.
		/// </param>
		/// <param name="text">
		/// The text to display in the message box.
		/// </param>
		/// <param name="links">
		/// An optional collection of links, resulting in use of a <see cref="LinkLabel"/>.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="buttons">
		/// One of the <see cref="MessageBoxButtons"/> values that specifies which buttons to display in the message box.
		/// </param>
		/// <param name="icon">
		/// One of the <see cref="MessageBoxIcon"/> values that specifies which icon to display in the message box.
		/// </param>
		/// <param name="defaultButton">
		/// One of the <see cref="MessageBoxDefaultButton"/> values that specifies the default button for the message box.
		/// </param>
		/// <returns>
		/// One of the <see cref="DialogResult"/> values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string text, ICollection<LinkLabel.Link> links, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			bool checkValue = false;
			return (Show(owner, text, links, caption, null, ref checkValue, buttons, icon, defaultButton));
		}

		/// <summary>
		/// Displays a status box in front of the specified object and with the specified
		/// status and caption and returns the result.
		/// </summary>
		/// <param name="owner">
		/// An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.
		/// </param>
		/// <param name="text">
		/// The text to display in the message box.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="buttons">
		/// One of the <see cref="MessageBoxButtons"/> values that specifies which buttons to display in the message box.
		/// </param>
		/// <param name="icon">
		/// One of the <see cref="MessageBoxIcon"/> values that specifies which icon to display in the message box.
		/// </param>
		/// <param name="defaultButton">
		/// One of the <see cref="MessageBoxDefaultButton"/> values that specifies the default button for the message box.
		/// </param>
		/// <param name="checkText">
		/// The text of the setting check box.
		/// </param>
		/// <param name="checkValue">
		/// The value of the setting.
		/// </param>
		/// <returns>
		/// One of the <see cref="DialogResult"/> values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Setting is required to be received, modified and returned.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string text, string caption, string checkText, ref bool checkValue, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			return (Show(owner, text, null, caption, checkText, ref checkValue, buttons, icon, defaultButton));
		}

		/// <summary>
		/// Displays a status box in front of the specified object and with the specified
		/// status and caption and returns the result.
		/// </summary>
		/// <param name="owner">
		/// An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.
		/// </param>
		/// <param name="text">
		/// The text to display in the message box.
		/// </param>
		/// <param name="links">
		/// An optional collection of links, resulting in use of a <see cref="LinkLabel"/>.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="buttons">
		/// One of the <see cref="MessageBoxButtons"/> values that specifies which buttons to display in the message box.
		/// </param>
		/// <param name="icon">
		/// One of the <see cref="MessageBoxIcon"/> values that specifies which icon to display in the message box.
		/// </param>
		/// <param name="defaultButton">
		/// One of the <see cref="MessageBoxDefaultButton"/> values that specifies the default button for the message box.
		/// </param>
		/// <param name="checkText">
		/// The text of the setting check box.
		/// </param>
		/// <param name="checkValue">
		/// The value of the setting.
		/// </param>
		/// <returns>
		/// One of the <see cref="DialogResult"/> values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Setting is required to be received, modified and returned.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string text, ICollection<LinkLabel.Link> links, string caption, string checkText, ref bool checkValue, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			var box = new ExtendedMessageBox(text, links, caption, checkText, ref checkValue, buttons, icon, defaultButton);

			DialogResult dr;

			ContextMenuStripShortcutModalFormWorkaround.EnterModalForm();
			try
			{
				dr = box.ShowDialog(owner);
			}
			finally
			{
				ContextMenuStripShortcutModalFormWorkaround.LeaveModalForm();
			}

			checkValue = box.CheckValue;
			return (dr);
		}

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <remarks>Default constructor needed for designer support.</remarks>
		public ExtendedMessageBox()
		{
			InitializeComponent();
		}

		/// <summary></summary>
		protected ExtendedMessageBox(string text, ICollection<LinkLabel.Link> links, string caption, string checkText, ref bool checkValue, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			InitializeComponent();

			Text = caption;

			int designerLabelHeight;
			var linksEnabled = !ICollectionEx.IsNullOrEmpty((ICollection)links);
			if (linksEnabled)
			{
				label_Text.Visible = false;
				label_Text.Enabled = false;

				designerLabelHeight = linkLabel_TextWithLink.Height;
				linkLabel_TextWithLink.Text = text;

				foreach (var link in links)
					linkLabel_TextWithLink.Links.Add(link);
			}
			else
			{
				linkLabel_TextWithLink.Visible = false;
				linkLabel_TextWithLink.Enabled = false;

				designerLabelHeight = label_Text.Height;
				label_Text.Text = text;
			}

			var designerCheckHeight = checkBox_Check.Height;
			var checkEnabled = !string.IsNullOrEmpty(checkText);
			AdjustCheck(checkEnabled, checkText, checkValue);
			AdjustLayout(designerLabelHeight, linksEnabled, designerCheckHeight, checkEnabled, buttons, icon);
			AdjustButtons(buttons, defaultButton);
			AdjustIcon(icon);
		}

		private void AdjustCheck(bool checkEnabled, string checkText, bool checkValue)
		{
			if (checkEnabled)
			{
				checkBox_Check.Visible = true;
				checkBox_Check.Enabled = true;
				checkBox_Check.Text    = checkText;
				checkBox_Check.Checked = checkValue;
			}
			else
			{
				checkBox_Check.Visible = false;
				checkBox_Check.Enabled = false;
				checkBox_Check.Text    = null;
				checkBox_Check.Checked = false;
			}
		}

		private void AdjustLayout(int designerLabelHeight, bool linksEnabled, int designerCheckHeight, bool checkEnabled, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			// "Suspend/ResumeLayout()" must not be done, the below adjustment relies on layouting!

			var label = (linksEnabled ? linkLabel_TextWithLink : label_Text);

			if (icon == MessageBoxIcon.None)
			{
				const int offsetTop = (34 - 25); // Designer retrieved values.
				label.Top -= offsetTop; // Move up a bit, or a bit more if already adjusted for more than a line.

				const int offsetLeft = (58 - 9); // Designer retrieved values.
				label.Left -= offsetLeft; // Align the label to the left.

				var maximumSize = label.MaximumSize; // width = 331
				maximumSize.Width += offsetLeft; // Adjust the maximum width accordingly.
				label.MaximumSize = maximumSize; // width = 372

				if (label.Height < (designerLabelHeight * 2)) // < 26
					Height = MinimumSize.Height;
				else
					Height = (MinimumSize.Height + designerLabelHeight);
			}
			else if (label.Height >= (designerLabelHeight * 2)) // >= 26
			{
				int offsetTop = (designerLabelHeight / 2); // = 13/2 ~= 7
				label.Top -= offsetTop; // Align the first two lines with the icon.
			}

			var superfluousWidthText = (label.MaximumSize.Width - label.Width);
			var superfluousWidthCheck = (checkEnabled ? (checkBox_Check.MaximumSize.Width - checkBox_Check.Width) : Width);

			var offsetAmongButtons = (button_A.Left - button_B.Left); // = 81
			var offsetMultiplier = (staticButtonCountLookup[buttons] - 1);
			var minimumWidthButtons = (MinimumSize.Width + (offsetAmongButtons * offsetMultiplier)); // = 128/209/390
			var superfluousWidthButtons = (Width - minimumWidthButtons);

			var superfluousWidth = Math.Min(superfluousWidthText, Math.Min(superfluousWidthCheck, superfluousWidthButtons));
			if (superfluousWidth > 0) {
				Width -= superfluousWidth;
			}
			                                //// "missing" must be 0 or above.
			var missingHeigthText = Math.Max(0, (label.Height - (designerLabelHeight * 2)));  // Account for multi-line text.
			var missingHeigthCheck = (checkEnabled ? (30 + (checkBox_Check.Height - designerCheckHeight)) : 0);

			var missingHeigth = (missingHeigthText + missingHeigthCheck);
			if (missingHeigth > 0) {
				Height += missingHeigth;
			}

			// [Check] is anchored to "Top" for design convenience, thus adjust to new height:
			if (checkEnabled) {
				checkBox_Check.Top = (panel_Lower.Top - 24 - (checkBox_Check.Height - designerCheckHeight)); // Account for multi-line text.
			}
			else {
				checkBox_Check.Top = (panel_Lower.Top + 6); // Keep hidden underneath panel.
			}
		}

		private void AdjustButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
		{
			Tuple<string, DialogResult> lookup;

			// A is the right-most button and always visible:
			Button button = button_A;
			if (staticButtonALookup.TryGetValue(buttons, out lookup))
			{
				button.Text         = lookup.Item1;
				button.DialogResult = lookup.Item2;

				if (button.DialogResult == DialogResult.Cancel)
					CancelButton = button;

				// "AcceptButton" see further below.
			}
			else
			{
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + buttons + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			button = button_B;
			if (staticButtonBLookup.TryGetValue(buttons, out lookup))
			{
				button.Text         = lookup.Item1;
				button.DialogResult = lookup.Item2;

				if (button.DialogResult == DialogResult.Cancel)
					CancelButton = button;

				// "AcceptButton" see further below.
			}
			else
			{
				button.Enabled = false;
				button.Visible = false;
			}

			button = button_C;
			if (staticButtonCLookup.TryGetValue(buttons, out lookup))
			{
				button.Text         = lookup.Item1;
				button.DialogResult = lookup.Item2;

				if (button.DialogResult == DialogResult.Cancel)
					CancelButton = button;

				// "AcceptButton" see further below.
			}
			else
			{
				button.Enabled = false;
				button.Visible = false;
			}

			switch (defaultButton)
			{
				case MessageBoxDefaultButton.Button1:
				{       // Not using "Visible" because "the control might not be visible to the user if it is obscured behind other controls".
					if      (button_C.Enabled) { AcceptButton = button_C; button_C.Select(); } // The respresentation of the selected button slightly
					else if (button_B.Enabled) { AcceptButton = button_B; button_B.Select(); } // differs from a "MessageBox", there is no dotted line
					else if (button_A.Enabled) { AcceptButton = button_A; button_A.Select(); } // around the button. Accepting this minor difference.
					else                       { throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + MessageBoxDefaultButton.Button1 + "' is not visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug)); }
					break;
				}

				case MessageBoxDefaultButton.Button2:
				{
					if      (button_C.Enabled) { AcceptButton = button_B; button_B.Select(); }
					else if (button_B.Enabled) { AcceptButton = button_A; button_A.Select(); }
					else                       { throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + MessageBoxDefaultButton.Button2 + "' is not visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug)); }
					break;
				}

				case MessageBoxDefaultButton.Button3:
				{
					if      (button_C.Enabled) { AcceptButton = button_A; button_A.Select(); }
					else                       { throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + MessageBoxDefaultButton.Button3 + "' is not visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug)); }
					break;
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + defaultButton + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void AdjustIcon(MessageBoxIcon icon)
		{
			if (icon == MessageBoxIcon.None)
			{
				pictureBox_Icon.Visible = false;
			}
			else
			{
				Icon image;

				switch (icon)
				{
					case MessageBoxIcon.Error:       image = SystemIcons.Error;       break; // Covers "Hand" and "Stop".
					case MessageBoxIcon.Question:    image = SystemIcons.Question;    break;
					case MessageBoxIcon.Exclamation: image = SystemIcons.Exclamation; break; // Covers "Warning".
					case MessageBoxIcon.Information: image = SystemIcons.Information; break; // Covers "Asterisk".

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + icon + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				pictureBox_Icon.Image = image.ToBitmap();
			}
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual string Caption
		{
			get { return (Text); }
			set { Text = value;  }
		}

		/// <remarks>
		/// Named "Message" rather than just "Text" to disambiguate <see cref="Form.Text"/>.
		/// </remarks>
		public virtual string MessageText
		{
			get { return (label_Text.Text); }
			set { label_Text.Text = value;  }
		}

		/// <summary></summary>
		public virtual string LinkText
		{
			get { return (linkLabel_TextWithLink.Text); }
			set { linkLabel_TextWithLink.Text = value;  }
		}

		/// <summary></summary>
		protected virtual bool CheckValue
		{
			get { return (checkBox_Check.Checked); }
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void linkLabel_TextWithLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
