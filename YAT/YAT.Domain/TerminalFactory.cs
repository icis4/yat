//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.3.90 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

using MKY;

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
			throw (new TypeLoadException(MessageHelper.InvalidExecutionPreamble + "'" + settings.TerminalType + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public static Terminal RecreateTerminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			switch (settings.TerminalType)
			{
				case TerminalType.Text:   return (new TextTerminal  (settings, terminal));
				case TerminalType.Binary: return (new BinaryTerminal(settings, terminal));
			}
			throw (new ArgumentOutOfRangeException("settings", settings, MessageHelper.InvalidExecutionPreamble + "'" + settings.TerminalType + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
