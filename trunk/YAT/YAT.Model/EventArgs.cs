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
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

namespace YAT.Model
{
	/// <summary></summary>
	public class WorkspaceEventArgs : EventArgs
	{
		private Workspace workspace;

		/// <summary></summary>
		public WorkspaceEventArgs(Workspace workspace)
		{
			this.workspace = workspace;
		}

		/// <summary></summary>
		public Workspace Workspace
		{
			get { return (this.workspace); }
		}
	}

	/// <summary></summary>
	public class TerminalEventArgs : EventArgs
	{
		private Terminal terminal;

		/// <summary></summary>
		public TerminalEventArgs(Terminal terminal)
		{
			this.terminal = terminal;
		}

		/// <summary></summary>
		public Terminal Terminal
		{
			get { return (this.terminal); }
		}
	}

	/// <summary></summary>
	public class SavedEventArgs : EventArgs
	{
		private string filePath;
		private bool isAutoSave;

		/// <summary></summary>
		public SavedEventArgs(string filePath)
			: this(filePath, false)
		{
		}

		/// <summary></summary>
		public SavedEventArgs(string filePath, bool isAutoSave)
		{
			this.filePath = filePath;
			this.isAutoSave = isAutoSave;
		}

		/// <summary></summary>
		public string FilePath
		{
			get { return (this.filePath); }
		}

		/// <summary>
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </summary>
		public bool IsAutoSave
		{
			get { return (this.isAutoSave); }
		}
	}

	/// <summary></summary>
	public class ClosedEventArgs : EventArgs
	{
		private bool isParentClose;

		/// <summary></summary>
		public ClosedEventArgs()
			: this(false)
		{
		}

		/// <summary></summary>
		public ClosedEventArgs(bool isParentClose)
		{
			this.isParentClose = isParentClose;
		}

		/// <summary></summary>
		public bool IsParentClose
		{
			get { return (this.isParentClose); }
		}
	}

	/// <summary></summary>
	public class StatusTextEventArgs : EventArgs
	{
		private string text;

		/// <summary></summary>
		public StatusTextEventArgs(string text)
		{
			this.text = text;
		}

		/// <summary></summary>
		public string Text
		{
			get { return (this.text); }
		}
	}

	/// <summary></summary>
	public class MessageInputEventArgs : EventArgs
	{
		private string text;
		private string caption;
		private MessageBoxButtons buttons;
		private MessageBoxIcon icon;
		private MessageBoxDefaultButton defaultButton;
		private DialogResult result;

		/// <summary></summary>
		public MessageInputEventArgs(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
			: this(text, caption, buttons, icon, MessageBoxDefaultButton.Button1)
		{
		}

		/// <summary></summary>
		public MessageInputEventArgs(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			this.text = text;
			this.caption = caption;
			this.buttons = buttons;
			this.icon = icon;
			this.defaultButton = defaultButton;
		}

		/// <summary></summary>
		public string Text
		{
			get { return (this.text); }
		}

		/// <summary></summary>
		public string Caption
		{
			get { return (this.caption); }
		}

		/// <summary></summary>
		public MessageBoxButtons Buttons
		{
			get { return (this.buttons); }
		}

		/// <summary></summary>
		public MessageBoxIcon Icon
		{
			get { return (this.icon); }
		}

		/// <summary></summary>
		public MessageBoxDefaultButton DefaultButton
		{
			get { return (this.defaultButton); }
		}

		/// <summary></summary>
		public DialogResult Result
		{
			get { return (this.result); }
			set { this.result = value;  }
		}
	}

	/// <summary></summary>
	public class DialogEventArgs : EventArgs
	{
		private DialogResult result;

		/// <summary></summary>
		public DialogEventArgs()
		{
		}

		/// <summary></summary>
		public DialogResult Result
		{
			get { return (this.result); }
			set { this.result = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
