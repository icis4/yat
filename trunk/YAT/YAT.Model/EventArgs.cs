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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Forms;

namespace YAT.Model
{
	/// <summary></summary>
	public class WorkspaceEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly Workspace Workspace;

		/// <summary></summary>
		public WorkspaceEventArgs(Workspace workspace)
		{
			Workspace = workspace;
		}
	}

	/// <summary></summary>
	public class TerminalEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly Terminal Terminal;

		/// <summary></summary>
		public TerminalEventArgs(Terminal terminal)
		{
			Terminal = terminal;
		}
	}

	/// <summary></summary>
	public class SavedEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly string FilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly bool IsAutoSave;

		/// <summary></summary>
		public SavedEventArgs(string filePath)
		{
			FilePath = filePath;
			IsAutoSave = false;
		}

		/// <summary></summary>
		public SavedEventArgs(string filePath, bool isAutoSave)
		{
			FilePath = filePath;
			IsAutoSave = isAutoSave;
		}
	}

	/// <summary></summary>
	public class ClosedEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly bool IsParentClose;

		/// <summary></summary>
		public ClosedEventArgs()
		{
			IsParentClose = false;
		}

		/// <summary></summary>
		public ClosedEventArgs(bool isParentClose)
		{
			IsParentClose = isParentClose;
		}
	}

	/// <summary></summary>
	public class StatusTextEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly string Text;

		/// <summary></summary>
		public StatusTextEventArgs(string text)
		{
			Text = text;
		}
	}

	/// <summary></summary>
	public class MessageInputEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly string Text = "";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly string Caption = "";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly MessageBoxButtons Buttons = MessageBoxButtons.OK;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly MessageBoxIcon Icon = MessageBoxIcon.Information;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly MessageBoxDefaultButton DefaultButton = MessageBoxDefaultButton.Button1;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public DialogResult Result;

		/// <summary></summary>
		public MessageInputEventArgs(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			Text = text;
			Caption = caption;
			Buttons = buttons;
			Icon = icon;
		}

		/// <summary></summary>
		public MessageInputEventArgs(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			Text = text;
			Caption = caption;
			Buttons = buttons;
			Icon = icon;
			DefaultButton = defaultButton;
		}
	}

	/// <summary></summary>
	public class DialogEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public DialogResult Result;

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
