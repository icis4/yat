using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace YAT.Model
{
	/// <summary></summary>
	public class WorkspaceEventArgs : EventArgs
	{
		/// <summary></summary>
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
		public readonly string FilePath;
		/// <summary></summary>
		public readonly bool AutoSave;

		/// <summary></summary>
		public SavedEventArgs(string filePath)
		{
			FilePath = filePath;
			AutoSave = false;
		}

		/// <summary></summary>
		public SavedEventArgs(string filePath, bool autoSave)
		{
			FilePath = filePath;
			AutoSave = autoSave;
		}
	}

	/// <summary></summary>
	public class StatusTextEventArgs : EventArgs
	{
		/// <summary></summary>
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
		public readonly string Text = "";
		/// <summary></summary>
		public readonly string Caption = "";
		/// <summary></summary>
		public readonly MessageBoxButtons Buttons = MessageBoxButtons.OK;
		/// <summary></summary>
		public readonly MessageBoxIcon Icon = MessageBoxIcon.Information;
		/// <summary></summary>
		public readonly MessageBoxDefaultButton DefaultButton = MessageBoxDefaultButton.Button1;
		/// <summary></summary>
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
		public DialogResult Result;

		/// <summary></summary>
		public DialogEventArgs()
		{
		}
	}
}
