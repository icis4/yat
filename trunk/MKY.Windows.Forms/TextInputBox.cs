//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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

		private bool _isStartingUp = true;

		private string _inputText = "";
		private string _inputText_Form = "";

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
			_inputText_Form = initialInputText;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string InputText
		{
			get { return (_inputText); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		private void TextInputBox_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// initially set controls and validate its contents where needed
				SetControls();

				// move cursor to end
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
			_inputText_Form = textBox_InputText.Text;
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			_inputText = _inputText_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			textBox_InputText.Text = _inputText_Form;
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
