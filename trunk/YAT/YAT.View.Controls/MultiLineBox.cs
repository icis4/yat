//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Text;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY.Windows.Forms;

using YAT.Model.Types;
using YAT.View.Utilities;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
	public partial class MultiLineBox : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.Parser.Modes parseMode;

		private Command command;
		private Command commandInEdit;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public MultiLineBox(Command command, Point startupLocation, Domain.Parser.Modes parseMode)
		{
			InitializeComponent();

			SuspendLayout();
			Location = startupLocation;
			ResumeLayout();

			this.parseMode = parseMode;

			this.command = command;
			this.commandInEdit = new Command(command);

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

		/// <summary></summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			// Ctrl+Enter = Confirm the edited text and close the box.
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
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
		/// </remarks>
		private void MultiLineBox_Shown(object sender, EventArgs e)
		{
			SetControls();

			// Move cursor to the end.
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
			if (!this.isSettingControls)
			{
				// Retrieve lines from text box with Environment.NewLine:
				StringReader reader = new StringReader(textBox_Lines.Text);
				List<string> multiLineText = new List<string>();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					multiLineText.Add(line);
				}

				// Validate each line:
				bool isValid = true;
				int textLength = 0;
				foreach (string s in multiLineText)
				{
					int invalidTextStart;
					int invalidTextLength;
					if (!Validation.ValidateText(this, "text", s, /* FR#238 add this.defaultRadix */ this.parseMode, out invalidTextStart, out invalidTextLength))
					{
						invalidTextStart += textLength;
						invalidTextLength = textBox_Lines.Text.Length - invalidTextStart;
						textBox_Lines.Select(invalidTextStart, invalidTextLength);
						isValid = false;
						break;
					}
					textLength += s.Length + Environment.NewLine.Length;
				}

				if (isValid)
				{
					this.commandInEdit.MultiLineText = multiLineText.ToArray();
					this.commandInEdit.SetDescriptionFromSingleLineText(); // Enforce "<N lines...> [...] [...] ..." description.
					SetControls();
				}
				else
				{
					e.Cancel = true;
				}
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

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			if (this.commandInEdit.IsSingleLineText)
			{
				textBox_Lines.Text = this.commandInEdit.SingleLineText;
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < this.commandInEdit.MultiLineText.Length; i++)
				{
					sb.Append(this.commandInEdit.MultiLineText[i]);
					if (i < (this.commandInEdit.MultiLineText.Length - 1))
						sb.AppendLine();
				}
				textBox_Lines.Text = sb.ToString();
			}

			this.isSettingControls.Leave();
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
