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
// MKY Version 1.0.28 Development
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

using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// An improved <see cref="TextBox"/> that additionally provides:
	/// <list type="bullet">
	/// <item><description>The [Ctrl+Backspace] shortcut.</description></item>
	/// <item><description>The [Ctrl+A] shortcut also if <see cref="TextBoxBase.ReadOnly"/> or <see cref="TextBox.Multiline"/> is <c>true</c>.</description></item>
	/// <item><description>Setting <see cref="Text"/> doesn't select all if <see cref="TextBoxBase.ReadOnly"/> or <see cref="TextBox.Multiline"/> is <c>true</c>.</description></item>
	/// </list>
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class TextBoxEx : TextBox
	{
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
			// Similar code exists in...
			// ...ComboBoxEx.ProcessCmdKey().
			// ...ToolStripComboBoxEx.ProcessCmdKey().
			// Changes here may have to be applied there too.

			if (ShortcutsEnabled)
			{
				if (keyData == (Keys.Control | Keys.Back))
				{
					if (!ReadOnly)
					{
						if (SelectionStart > 0)
						{
							int i = (SelectionStart - 1);

							// Potentially trim white space:
							if (char.IsWhiteSpace(Text, i))
								i = (StringEx.IndexOfSameCharacterClass(Text, i) - 1);

							// Find previous marker:
							if (i > 0)
								i = StringEx.IndexOfSameCharacterClass(Text, i);
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
				else if (keyData == (Keys.Control | Keys.A))
				{
					if (ReadOnly || Multiline) // Limitation of standard "TextBox".
					{
						SelectAll();
						return (true);
					}
				}
			}

			return (base.ProcessCmdKey(ref msg, keyData));
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public override string Text
		{
			get
			{
				return (base.Text);
			}

			set
			{
				base.Text = value;

				if (ReadOnly || Multiline) // Limitation of standard "TextBox".
				{
					SelectionLength = 0; // DeselectAll() doesn't work, setting the properties works.
					SelectionStart = 0;
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
