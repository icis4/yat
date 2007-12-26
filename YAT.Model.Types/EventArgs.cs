using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Model.Types
{
	/// <summary></summary>
	public class PredefinedCommandEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly int Page;
		/// <summary></summary>
		public readonly int Command;

		/// <summary></summary>
		public PredefinedCommandEventArgs(int command)
		{
			Page = 1;
			Command = command;
		}

		/// <summary></summary>
		public PredefinedCommandEventArgs(int page, int command)
		{
			Page = page;
			Command = command;
		}
	}
}
