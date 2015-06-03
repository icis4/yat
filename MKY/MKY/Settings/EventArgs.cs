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
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
		private SettingsItem source;
		private SettingsEventArgs inner;

		/// <summary></summary>
		public SettingsEventArgs(SettingsItem source)
		{
			this.source = source;
		}

		/// <summary></summary>
		public SettingsEventArgs(SettingsItem source, SettingsEventArgs inner)
		{
			this.source = source;
			this.inner = inner;
		}

		/// <summary></summary>
		public SettingsItem Source
		{
			get { return (this.source); }
		}

		/// <summary></summary>
		public SettingsEventArgs Inner
		{
			get { return (this.inner); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
