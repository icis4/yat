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
// MKY Version 1.0.27
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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY.ComponentModel;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a simple text input box similar to <see cref="MessageBox"/>.
	/// </summary>
	public partial class TextInputBox : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string inputText = "";
		private string inputTextInEdit = "";

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public new event EventHandler<StringCancelEventArgs> Validating;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		protected TextInputBox(string text, string caption, string initialInputText)
		{
			InitializeComponent();

			Text = caption;
			label_Text.Text = text;
			this.inputTextInEdit = initialInputText;

			// SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string InputText
		{
			get { return (this.inputText); }
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
		private void TextInputBox_Shown(object sender, EventArgs e)
		{
			SetControls();

			// Move cursor to the end.
			textBox_InputText.SelectionStart = textBox_InputText.Text.Length;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textBox_InputText_Validating(object sender, CancelEventArgs e)
		{
			var text = textBox_InputText.Text;
			var args = new StringCancelEventArgs(text);

			OnValidating(args);

			if (args.Cancel)
				e.Cancel = true;
			else
				this.inputTextInEdit = text;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.inputText = this.inputTextInEdit;
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
			textBox_InputText.Text = this.inputTextInEdit;
		}

		#endregion

		#region Public Static Methods
		//==========================================================================================
		// Public Static Methods
		//==========================================================================================

		/// <summary>
		/// Displays a input box in front of the specified object and with the specified
		/// text and caption and returns the result.
		/// </summary>
		/// <param name="owner">
		/// An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.
		/// </param>
		/// <param name="text">
		/// The text to display in the input box.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the input box.
		/// </param>
		/// <param name="initialInputText">
		/// The initial text that is in the input box.
		/// </param>
		/// <param name="inputText">
		/// The text that was entered in the input box.
		/// </param>
		/// <returns>
		/// One of the System.Windows.Forms.DialogResult values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Text that is input needs to be returned in addition to the dialog result.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string text, string caption, string initialInputText, out string inputText)
		{
			return (Show(owner, text, caption, initialInputText, null, out inputText));
		}

		/// <summary>
		/// Displays a input box in front of the specified object and with the specified
		/// text and caption and returns the result.
		/// </summary>
		/// <param name="owner">
		/// An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.
		/// </param>
		/// <param name="text">
		/// The text to display in the input box.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the input box.
		/// </param>
		/// <param name="initialInputText">
		/// The initial text that is in the input box.
		/// </param>
		/// <param name="inputValidationCallback">
		/// Input validation callback.
		/// </param>
		/// <param name="inputText">
		/// The text that was entered in the input box.
		/// </param>
		/// <returns>
		/// One of the System.Windows.Forms.DialogResult values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Text that is input needs to be returned in addition to the dialog result.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string text, string caption, string initialInputText, EventHandler<StringCancelEventArgs> inputValidationCallback, out string inputText)
		{
			DialogResult dr;
			var tib = new TextInputBox(text, caption, initialInputText);

			ContextMenuStripShortcutModalFormWorkaround.EnterModalForm();
			try
			{
				if (inputValidationCallback != null)
					tib.Validating += inputValidationCallback;

				dr = tib.ShowDialog(owner);
			}
			finally
			{
				if (inputValidationCallback != null)
					tib.Validating -= inputValidationCallback;

				ContextMenuStripShortcutModalFormWorkaround.LeaveModalForm();
			}

			if (dr == DialogResult.OK)
				inputText = tib.InputText;
			else
				inputText = "";

			return (dr);
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnValidating(StringCancelEventArgs e)
		{
			EventHelper.RaiseSync(Validating, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
