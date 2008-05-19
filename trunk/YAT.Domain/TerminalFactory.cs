using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Domain.Factory
{
	/// <summary></summary>
	public static class TerminalFactory
	{
		/// <summary></summary>
		public static Terminal CreateTerminal(Settings.TerminalSettings settings)
		{
			switch (settings.TerminalType)
			{
				case TerminalType.Text:   return (new TextTerminal(settings));
				case TerminalType.Binary: return (new BinaryTerminal(settings));
				default: throw (new TypeLoadException("Unknown terminal type"));
			}
		}

		/// <summary></summary>
		public static Terminal RecreateTerminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			switch (settings.TerminalType)
			{
				case TerminalType.Text:   return (new TextTerminal(settings, terminal));
				case TerminalType.Binary: return (new BinaryTerminal(settings, terminal));
				default: throw (new NotImplementedException("Unknown terminal type"));
			}
		}
	}
}
