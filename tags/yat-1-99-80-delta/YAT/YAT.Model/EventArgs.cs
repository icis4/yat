﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace YAT.Model
{
	/// <summary></summary>
	public class SavedEventArgs : EventArgs
	{
		/// <summary></summary>
		public string FilePath { get; }

		/// <summary>
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </summary>
		public bool IsAutoSave { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public SavedEventArgs(string filePath, bool isAutoSave = false)
		{
			FilePath = filePath;
			IsAutoSave = isAutoSave;
		}
	}

	/// <summary></summary>
	public class ClosedEventArgs : EventArgs
	{
		/// <summary></summary>
		public bool IsParentClose { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public ClosedEventArgs(bool isParentClose = false)
		{
			IsParentClose = isParentClose;
		}
	}

	/// <summary></summary>
	public class MessageInputEventArgs : EventArgs
	{
		/// <summary></summary>
		public string Text { get; }

		/// <summary></summary>
		public string Caption { get; }

		/// <summary></summary>
		public MessageBoxButtons Buttons { get; }

		/// <summary></summary>
		public MessageBoxIcon Icon { get; }

		/// <summary></summary>
		public MessageBoxDefaultButton DefaultButton { get; }

		/// <summary></summary>
		public DialogResult Result { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public MessageInputEventArgs(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			Text = text;
			Caption = caption;
			Buttons  = buttons;
			Icon = icon;
			DefaultButton = defaultButton;
		}
	}

	/// <summary></summary>
	public class DialogEventArgs : EventArgs
	{
		/// <summary></summary>
		public DialogResult Result { get; set; }

		/// <summary></summary>
		public DialogEventArgs()
		{
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================