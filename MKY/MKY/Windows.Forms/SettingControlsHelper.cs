//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="System.Windows.Forms"/> utility methods.
	/// </summary>
	public struct SettingControlsHelper
	{
		private int count;

		/// <summary></summary>
		public bool IsSettingsControls
		{
			get { return (this.count <= 0); }
		}

		/// <summary></summary>
		public void Enter()
		{
			this.count++;
		}

		/// <summary></summary>
		public void Leave()
		{
			this.count--;

			if (this.count < 0)
				throw (new InvalidOperationException("SettingControlsHelper count has fallen below 0"));
		}

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator bool(SettingControlsHelper isSettingControls)
		{
			return (isSettingControls.IsSettingsControls);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
