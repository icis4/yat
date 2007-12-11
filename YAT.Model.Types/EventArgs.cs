using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Model.Types
{
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
