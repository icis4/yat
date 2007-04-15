using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain
{
	/// <summary></summary>
	public class TerminalErrorEventArgs : EventArgs
	{
		public readonly string Message;

		public TerminalErrorEventArgs(string message)
		{
			Message = message;
		}
	}

	/// <summary></summary>
	public class RawElementEventArgs : EventArgs
	{
		public readonly RawElement Element;

		public RawElementEventArgs(RawElement element)
		{
			Element = element;
		}
	}

	/// <summary></summary>
	public class DisplayElementsEventArgs : EventArgs
	{
		public readonly List<DisplayElement> Elements;

		public DisplayElementsEventArgs(List<DisplayElement> elements)
		{
			Elements = elements;
		}
	}

	/// <summary></summary>
	public class DisplayLinesEventArgs : EventArgs
	{
		public readonly List<List<DisplayElement>> Lines;

		public DisplayLinesEventArgs(List<List<DisplayElement>> lines)
		{
			Lines = lines;
		}
	}

	/// <summary></summary>
	public class RepositoryEventArgs : EventArgs
	{
		public readonly RepositoryType Repository;

		public RepositoryEventArgs(RepositoryType repository)
		{
			Repository = repository;
		}
	}
}
