using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Domain
{
	/// <summary></summary>
	public class TerminalErrorEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly string Message;

		/// <summary></summary>
		public TerminalErrorEventArgs(string message)
		{
			Message = message;
		}
	}

	/// <summary></summary>
	public class RawElementEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly RawElement Element;

		/// <summary></summary>
		public RawElementEventArgs(RawElement element)
		{
			Element = element;
		}
	}

	/// <summary></summary>
	public class DisplayElementsEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly List<DisplayElement> Elements;

		/// <summary></summary>
		public DisplayElementsEventArgs(List<DisplayElement> elements)
		{
			Elements = elements;
		}
	}

	/// <summary></summary>
	public class DisplayLinesEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly List<List<DisplayElement>> Lines;

		/// <summary></summary>
		public DisplayLinesEventArgs(List<List<DisplayElement>> lines)
		{
			Lines = lines;
		}
	}

	/// <summary></summary>
	public class RepositoryEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly RepositoryType Repository;

		/// <summary></summary>
		public RepositoryEventArgs(RepositoryType repository)
		{
			Repository = repository;
		}
	}
}
