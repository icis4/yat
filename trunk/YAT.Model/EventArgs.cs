using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace YAT.Model
{
	/// <summary></summary>
	public class StatusTextEventArgs : EventArgs
	{
		public readonly string Text;

		public StatusTextEventArgs(string text)
		{
			Text = text;
		}
	}

	/// <summary></summary>
	public class MessageInputEventArgs : EventArgs
	{
		public readonly string Text = "";
		public readonly string Caption = "";
		public readonly MessageBoxButtons Buttons = MessageBoxButtons.OK;
		public readonly MessageBoxIcon Icon = MessageBoxIcon.Information;
		public readonly MessageBoxDefaultButton DefaultButton = MessageBoxDefaultButton.Button1;
		public DialogResult Result;

		public MessageInputEventArgs(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			Text = text;
			Caption = caption;
			Buttons = buttons;
			Icon = icon;
		}

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
		public DialogEventArgs()
		{
		}
	}

	/// <summary></summary>
	public class TerminalSavedEventArgs : EventArgs
	{
		public readonly string FilePath;
		public readonly bool AutoSave;

		public TerminalSavedEventArgs(string filePath)
		{
			FilePath = filePath;
			AutoSave = false;
		}

		public TerminalSavedEventArgs(string filePath, bool autoSave)
		{
			FilePath = filePath;
			AutoSave = autoSave;
		}
	}
}
