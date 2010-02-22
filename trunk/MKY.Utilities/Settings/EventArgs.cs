//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Utilities.Settings
{
	/// <summary></summary>
	public class SettingsEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly Settings Source;

		/// <summary></summary>
		public readonly SettingsEventArgs Inner;

		/// <summary></summary>
		public SettingsEventArgs(Settings source)
		{
			Source = source;
		}

		/// <summary></summary>
		public SettingsEventArgs(Settings source, SettingsEventArgs inner)
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
