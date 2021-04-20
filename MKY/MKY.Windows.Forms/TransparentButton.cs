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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a transparent box. It can be used to handle user input on an invisible rectangle.
	/// </summary>
	/// <remarks>
	/// Due to some reason, <see cref="ButtonBase"/> doesn't support transparency. In contrast,
	/// <see cref="Panel"/> does (as shown in <see cref="TransparentPanel"/>). In order to provide
	/// button-like functionality in a transparent way, this <see cref="TransparentButton"/> can
	/// be used.
	/// </remarks>
	[DefaultProperty("Text")]
	public class TransparentButton : Control, IButtonControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary>
		/// Default value of the <see cref="BorderStyle"/> property.
		/// </summary>
		public const BorderStyle BorderStyleDefault = BorderStyle.None;

		/// <summary>
		/// Default value of the <see cref="UseMnemonic"/> property.
		/// </summary>
		public const bool UseMnemonicDefault = true;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private BorderStyle borderStyle = BorderStyleDefault;
		private bool useMnemonic = UseMnemonicDefault;

		private DialogResult dialogResult = DialogResult.None;

		private bool hasFocus = false;

		#endregion

		#region Transparency
		//==========================================================================================
		// Transparency
		//==========================================================================================

		/// <summary>
		/// Gets the required creation parameters when the control handle is created.
		/// </summary>
		[Browsable(false)]
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x20; // 0x20 = WS_EX_TRANSPARENT = one of the 'Extended Window Styles' of the Win32 API.
				return (cp);
			}
		}

		#endregion

		#region Border
		//==========================================================================================
		// Border
		//==========================================================================================

		/// <summary>
		/// Gets or sets the border style for the control.
		/// </summary>
		/// <value>
		/// One of the <see cref="BorderStyle"/> values. The default is specified by
		/// <see cref="BorderStyleDefault"/>.
		/// </value>
		[DefaultValue(BorderStyleDefault)]
		public BorderStyle BorderStyle
		{
			get { return (this.borderStyle); }
			set { this.borderStyle = value; }
		}

		#endregion

		#region Focus
		//==========================================================================================
		// Focus
		//==========================================================================================

		/// <summary>
		/// Raises the <see cref="System.Windows.Forms.Control.GotFocus"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnGotFocus(EventArgs e)
		{
			this.hasFocus = true;

			Invalidate(); // Signal that focus rectangle shall be drawn.

			base.OnGotFocus(e);
		}

		/// <summary>
		/// Raises the <see cref="System.Windows.Forms.Control.LostFocus"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnLostFocus(EventArgs e)
		{
			this.hasFocus = false;

			if (Parent != null)
				Parent.Invalidate(); // Required to remove focus rectangle again.

			base.OnLostFocus(e);
		}

		#endregion

		#region Paint
		//==========================================================================================
		// Paint
		//==========================================================================================

		/// <summary>
		/// Paints the foreground of the control.
		/// </summary>
		/// <param name="e">A <see cref="PaintEventArgs"/> that contains information about the control to paint.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			// Overlay the focus rectangle if the control has focus:
			if (this.hasFocus)
				ControlPaint.DrawFocusRectangle(e.Graphics, e.ClipRectangle);
		}

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="e">A <see cref="PaintEventArgs"/> that contains information about the control to paint.</param>
		[SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "'pevent'? What weird name is this, sorry .NET guys...")]
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// Draw border as requested:
			switch (this.borderStyle)
			{
				case BorderStyle.FixedSingle: ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);  break;
				case BorderStyle.Fixed3D:     ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, SystemColors.ControlDark, ButtonBorderStyle.Outset); break;
				default: break; // Do nothing, includes 'BorderStyle.None'.
			}
		}

		#endregion

		#region HotKey
		//==========================================================================================
		// HotKey
		//==========================================================================================

		/// <summary>
		/// Gets or sets a value indicating whether the first character that is preceded by an
		/// ampersand (&amp;) is used as the mnemonic key of the control.
		/// </summary>
		/// <value>
		/// <c>true</c> if the first character that is preceded by an ampersand (&amp;) is used as
		/// the mnemonic key of the control; otherwise, <c>false</c>. The default is specified by
		/// <see cref="UseMnemonicDefault"/>.
		/// </value>
		[DefaultValue(UseMnemonicDefault)]
		public bool UseMnemonic
		{
			get { return (this.useMnemonic); }
			set { this.useMnemonic = value;  }
		}

		/// <summary>
		/// Processes a mnemonic character.
		/// </summary>
		/// <param name="charCode">The character to process.</param>
		/// <returns> <c>true</c> if the character was processed as a mnemonic by the control;
		/// otherwise, <c>false</c>.</returns>
		protected override bool ProcessMnemonic(char charCode)
		{
			if (CanSelect && UseMnemonic && IsMnemonic(charCode, Text))
			{
				PerformClick();
				return (true);
			}

			return (base.ProcessMnemonic(charCode));
		}

		#endregion

		#region IButtonControl
		//==========================================================================================
		// IButtonControl
		//==========================================================================================

		/// <summary>
		/// Gets or sets the value returned to the parent form when the button is clicked.
		/// </summary>
		/// <value>
		/// One of the <see cref="DialogResult"/> values.
		/// </value>
		public DialogResult DialogResult
		{
			get
			{
				return (this.dialogResult);
			}

			set
			{
				if (Enum.IsDefined(typeof(DialogResult), value))
					this.dialogResult = value;
				else
					this.dialogResult = DialogResult.None;
			}
		}

		/// <summary>
		/// Notifies a control that it is the default button so that its appearance and behavior is adjusted accordingly.
		/// </summary>
		/// <param name="value">
		/// <c>true</c> if the control should behave as a default button; otherwise, <c>false</c>.
		/// </param>
		public void NotifyDefault(bool value)
		{
			UnusedArg.PreventAnalysisWarning(value, "No handling needed (yet).");
		}

		/// <summary>
		/// Generates a <see cref="Control.Click"/> event for the control.
		/// </summary>
		public void PerformClick()
		{
			if (CanSelect)
				OnClick(EventArgs.Empty);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
