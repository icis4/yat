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
// MKY Version 1.0.27
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

namespace MKY.Settings
{
	/// <summary></summary>
	public class SettingsEventArgs : EventArgs
	{
		/// <summary></summary>
		public SettingsItem Source { get; }

		/// <summary></summary>
		public SettingsEventArgs Inner { get; }

		/// <summary></summary>
		public SettingsEventArgs(SettingsItem source)
		{
			Source = source;
		}

		/// <summary></summary>
		public SettingsEventArgs(SettingsItem source, SettingsEventArgs inner)
		{
			Source = source;
			Inner = inner;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
