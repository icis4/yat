//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a simple text input box similar to <see cref="MessageBox"/>.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public partial class TextInputBox : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string inputText = "";
		private string inputText_Form = "";

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
			this.inputText_Form = initialInputText;
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
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void TextInputBox_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();

				// Move cursor to end
				textBox_InputText.SelectionStart = textBox_InputText.Text.Length;
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textBox_InputText_Validating(object sender, CancelEventArgs e)
		{
			this.inputText_Form = textBox_InputText.Text;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.inputText = this.inputText_Form;
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
			textBox_InputText.Text = this.inputText_Form;
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
		public static DialogResult Show(IWin32Window owner, string text, string caption, string initialInputText, out string inputText)
		{
			TextInputBox tib = new TextInputBox(text, caption, initialInputText);

			DialogResult dialogResult = tib.ShowDialog(owner);
			if (dialogResult == DialogResult.OK)
				inputText = tib.InputText;
			else
				inputText = "";
			
			return (dialogResult);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
