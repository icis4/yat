using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Gui
{
	/// <summary></summary>
	public class TerminalSavedEventArgs : EventArgs
	{
		public readonly string FilePath;

		public TerminalSavedEventArgs(string filePath)
		{
			FilePath = filePath;
		}
	}

	/// <summary></summary>
	public class PredefinedCommandEventArgs : EventArgs
	{
		public readonly int Page;
		public readonly int Command;

		public PredefinedCommandEventArgs(int command)
		{
			Page = 1;
			Command = command;
		}

		public PredefinedCommandEventArgs(int page, int command)
		{
			Page = page;
			Command = command;
		}
	}
}
