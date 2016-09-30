//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
			throw (new TypeLoadException(MessageHelper.InvalidExecutionPreamble + "'" + settings.TerminalType + "' is an invalid terminal type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public static Terminal RecreateTerminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			switch (settings.TerminalType)
			{
				case TerminalType.Text:   return (new TextTerminal  (settings, terminal));
				case TerminalType.Binary: return (new BinaryTerminal(settings, terminal));
			}
			throw (new ArgumentOutOfRangeException("settings", settings, MessageHelper.InvalidExecutionPreamble + "'" + settings.TerminalType + "' is an invalid terminal type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
