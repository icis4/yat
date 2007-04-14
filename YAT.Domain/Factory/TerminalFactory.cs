using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain.Factory
{
	public static class TerminalFactory
	{
		public static Terminal CreateTerminal(Settings.TerminalSettings settings)
		{
			switch (settings.TerminalType)
			{
				case TerminalType.Text:   return (new TextTerminal(settings));
				case TerminalType.Binary: return (new BinaryTerminal(settings));
				default: throw (new TypeLoadException("Unknown terminal type"));
			}
		}

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
