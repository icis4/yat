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
// MKY Version 1.0.22
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows.Forms;

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// An improved <see cref="ToolStripComboBox"/> that additionally provides:
	/// <list type="bullet">
	/// <item><description>The [Ctrl+Backspace] shortcut.</description></item>
	/// <item><description>Restore of cursor position and text selection on getting focus, same behavior as <see cref="TextBox"/>.</description></item>
	/// </list>
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class ToolStripComboBoxEx : ToolStripComboBox, IOnFormDeactivateWorkaround
	{
		private bool hasFocusAndFormDeactivateWorkaroundHasNotYetBeenApplied; // = false;

		private int lastSelectionStart = ControlEx.InvalidIndex;
		private int lastSelectionLength; // = 0;

		/// <remarks>
		/// Based on https://stackoverflow.com/questions/1124639/winforms-textbox-using-ctrl-backspace-to-delete-whole-word.
		/// </remarks>
		/// <remarks>
		/// In case of pressing a modifier key (e.g. [Shift]), this method is invoked twice! Both
		/// invocations will state msg=0x100 (WM_KEYDOWN)! See:
		/// https://msdn.microsoft.com/en-us/library/system.windows.forms.control.processcmdkey.aspx:
		/// The ProcessCmdKey method first determines whether the control has a ContextMenu, and if
		/// so, enables the ContextMenu to process the command key. If the command key is not a menu
		/// shortcut and the control has a parent, the key is passed to the parent's ProcessCmdKey
		/// method. The net effect is that command keys are "bubbled" up the control hierarchy. In
		/// addition to the key the user pressed, the key data also indicates which, if any, modifier
		/// keys were pressed at the same time as the key. Modifier keys include the SHIFT, CTRL, and
		/// ALT keys.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			// Attention:
			// Similar code exists in ComboBoxEx.ProcessCmdKey().
			// Similar code exists in TextBoxEx.ProcessCmdKey().
			// Changes here may have to be applied there too.

			if (keyData == (Keys.Control | Keys.Back))
			{
				if (DropDownStyle != ComboBoxStyle.DropDownList)
				{
					if (SelectionStart > 0)
					{
						int i = (SelectionStart - 1);

						// Potentially trim white space:
						if (char.IsWhiteSpace(Text, i))
							i = (StringEx.StartIndexOfSameCharacterClass(Text, i) - 1);

						// Find previous marker:
						if (i > 0)
							i = StringEx.StartIndexOfSameCharacterClass(Text, i);
						else
							i = 0; // Limit i as it may become -1 on trimming above.

						// Remove until previous marker or the beginning:
						Text = Text.Remove(i, SelectionStart - i);
						SelectionStart = i;
						return (true);
					}
					else
					{
						return (true); // Ignore to prevent a white box being placed.
					}
				}
			}

			return (base.ProcessCmdKey(ref msg, keyData));
		}

		/// <summary>
		/// Implements the same selection behaviour on getting focus as <see cref="TextBox"/>.
		/// </summary>
		/// <remarks>
		/// Attention: Thanks to the Microsoft guys, the implementation is trickier than necessary!
		/// 
		/// Event sequence on entering the control using [Tab]:
		///  1. 'Enter'
		///  2. 'LostFocus' !!!
		///  3. 'GotFocus'
		/// 
		/// Event sequence on entering the control when switching among MDI children (e.g. using [Ctrl+Tab]):
		///  1. 'Enter'
		///  2. 'LostFocus' !!!
		///  3. 'GotFocus'
		/// 
		/// Event sequence on entering the control when changing applications (e.g. using [Alt+Tab]):
		///      No 'Enter' !!!
		///  1. 'LostFocus' !!!
		///  2. 'GotFocus'
		/// 
		/// (Note that this is "slightly" different to what is stated at
		/// https://docs.microsoft.com/en-us/dotnet/framework/winforms/order-of-events-in-windows-forms.)
		/// 
		/// Event sequence on leaving the control using [Tab]:
		///  1. 'Leave'     and values are OK.
		///  2. 'LostFocus' but values invalidly are 0/0 !!!
		/// 
		/// Event sequence on entering the control when switching among MDI children (e.g. using [Ctrl+Tab]):
		///  1. 'LostFocus' but values invalidly are 0/0 !!!
		///  2. 'Leave'     but values invalidly are 0/0 !!!
		/// 
		/// Event sequence on leaving the control when changing applications (e.g. using [Alt+Tab]):
		///  1. 'LostFocus' but values invalidly are 0/0 !!!
		/// 
		/// Resulting constraints and solution/workaround:
		///  a') 'OnLostFocus' is called each time just before 'OnGotFocus', thus focus state would have to be kept as well.
		///  a)  'OnLostFocus' cannot keep the values anyway, for whatever reason...
		///        => 'OnLeave' is OK for [Tab], but [Ctrl+Tab] and [Alt+Tab] must be notified from parent form.
		///  b') 'OnEnter' couldn't restore the values, for whatever reason it is too early.
		///  b)  'OnEnter' cannot restore the values anyway, since it isn't called on [Alt+Tab].
		///        => 'OnGotFocus' is OK.
		/// </remarks>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected override void OnGotFocus(EventArgs e)
		{
			// Attention:
			// Same code exists in ComboBoxEx.OnGotFocus().
			// Changes here will have to be applied there too.

			base.OnGotFocus(e);
			this.hasFocusAndFormDeactivateWorkaroundHasNotYetBeenApplied = true; // [HasFocus = true]

			if (this.lastSelectionStart != ControlEx.InvalidIndex)
			{
				SelectionStart  = this.lastSelectionStart;
				SelectionLength = this.lastSelectionLength;
			}
		}

		/// <summary>
		/// Implements the same selection behaviour on getting focus as <see cref="TextBox"/>.
		/// </summary>
		/// <remarks>
		/// Attention: See remarks in <see cref="OnGotFocus(EventArgs)"/>!
		/// </remarks>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected override void OnLeave(EventArgs e)
		{
			// Attention:
			// Same code exists in ComboBoxEx.OnLeave().
			// Changes here will have to be applied there too.

			if (this.hasFocusAndFormDeactivateWorkaroundHasNotYetBeenApplied)
			{
				this.lastSelectionStart  = SelectionStart;
				this.lastSelectionLength = SelectionLength;

			}

			base.OnLeave(e);
		}

		/// <summary>
		/// Implements the same selection behaviour on getting focus as <see cref="TextBox"/>.
		/// </summary>
		/// <remarks>
		/// Attention: See remarks in <see cref="OnGotFocus(EventArgs)"/>!
		/// </remarks>
		public virtual void OnFormDeactivateWorkaround()
		{
			// Attention:
			// Same code exists in ComboBoxEx.OnFormDeactivateWorkaround().
			// Changes here will have to be applied there too.

			this.lastSelectionStart  = SelectionStart;
			this.lastSelectionLength = SelectionLength;

			this.hasFocusAndFormDeactivateWorkaroundHasNotYetBeenApplied = false; // [WorkaroundHasBeenApplied = true] == [WorkaroundHasNotYetBeenApplied = false]
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
