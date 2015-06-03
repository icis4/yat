//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1' Version 1.99.33
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
using System.Collections.Generic;
using System.Text;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public static class TerminalFactory
	{
		/// <summary></summary>
		public static Terminal CreateTerminal(Settings.TerminalSettings settings)
		{
			switch (settings.TerminalType)
			{
				case TerminalType.Text:   return (new TextTerminal  (settings));
				case TerminalType.Binary: return (new BinaryTerminal(settings));
			}
			throw (new TypeLoadException("Program execution should never get here, " + settings.TerminalType + " is an invalid terminal type, please report this bug!"));
		}

		/// <summary></summary>
		public static Terminal RecreateTerminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			switch (settings.TerminalType)
			{
				case TerminalType.Text:   return (new TextTerminal  (settings, terminal));
				case TerminalType.Binary: return (new BinaryTerminal(settings, terminal));
			}
			throw (new ArgumentOutOfRangeException("settings", settings, "Program execution should never get here, " + settings.TerminalType + " is an invalid terminal type, please report this bug!"));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
