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
// YAT Version 2.4.1
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Model.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine", Justification = "What's wrong with 'MultiLine'?")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
	public partial class MultiLineBox : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.RadixEx defaultRadix;
		private Domain.Parser.Mode parseMode;

		private Command command;
		private Command commandInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <remarks>
		/// This mini-form must be located in 'Controls' rather than 'Forms' because other controls
		/// use it.
		/// </remarks>
		public MultiLineBox(Command command, Domain.Radix defaultRadix, Domain.Parser.Mode parseMode)
		{
			InitializeComponent();

			this.defaultRadix = defaultRadix;
			this.parseMode = parseMode;

			this.command = command;
			this.commandInEdit = new Command(command); // Clone to ensure decoupling.

			// SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Command CommandResult
		{
			get { return (this.command); }
		}

		#endregion

		#region Form Special Keys
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

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
			// [Ctrl+Enter] = confirm the edited text and close the box.
			if (keyData == (Keys.Control | Keys.Enter))
			{
				if (ValidateChildren())
					AcceptAndClose();

				return (true);
			}

			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void MultiLineBox_Shown(object sender, EventArgs e)
		{
			SetControls();

			// Move cursor to the end:
			textBox_Lines.SelectionStart = textBox_Lines.Text.Length;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textBox_Lines_TextChanged(object sender, EventArgs e)
		{
			bool empty = string.IsNullOrEmpty(textBox_Lines.Text);
			label_Remarks.Visible = empty;
			button_OK.Enabled = !empty;
		}

		private void textBox_Lines_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			var multiLineText = new List<string>();

			// Retrieve lines from text box with Environment.NewLine:
			using (var reader = new StringReader(textBox_Lines.Text))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
					multiLineText.Add(line);
			}

			// Validate each line:
			bool isValid = true;
			int textLength = 0;
			foreach (string line in multiLineText)
			{
				int invalidTextStart;
				int invalidTextLength;
				if (!Utilities.ValidationHelper.ValidateText(this, "multi-line text", line, out invalidTextStart, out invalidTextLength, this.parseMode, this.defaultRadix))
				{
					invalidTextStart += textLength;
					invalidTextLength = textBox_Lines.Text.Length - invalidTextStart;
					textBox_Lines.Select(invalidTextStart, invalidTextLength);
					isValid = false;
					break;
				}
				textLength += line.Length + Environment.NewLine.Length;
			}

			if (isValid)
			{
				this.commandInEdit.MultiLineText = multiLineText.ToArray();
				SetControls();
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void textBox_Lines_Leave(object sender, EventArgs e)
		{
			bool empty = string.IsNullOrEmpty(textBox_Lines.Text);
			label_Remarks.Visible = empty;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			AcceptAndClose();
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				var c = this.commandInEdit;
				if (c.IsSingleLineText)
				{
					textBox_Lines.Text = c.SingleLineText;
				}
				else if (c.IsMultiLineText)
				{
					var sb = new StringBuilder();
					for (int i = 0; i < c.MultiLineText.Length; i++)
					{
						sb.Append(c.MultiLineText[i]);
						if (i < (c.MultiLineText.Length - 1))
							sb.AppendLine();
					}
					textBox_Lines.Text = sb.ToString();
				}
				else
				{
					textBox_Lines.Text = ""; // Happens for a yet to be <Define...> command.
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void AcceptAndClose()
		{
			this.command = this.commandInEdit;
			DialogResult = DialogResult.OK;
			Close();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
