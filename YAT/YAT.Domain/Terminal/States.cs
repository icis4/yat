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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace YAT.Domain
{
	/// <summary></summary>
	public class DeviceOrDirectionState
	{
		/// <summary></summary>
		public bool IsFirstChunk     { get; set; }

		/// <summary></summary>
		public string Device         { get; set; }

		/// <summary></summary>
		public IODirection Direction { get; set; }

		/// <summary></summary>
		public DeviceOrDirectionState()
		{
			Reset();
		}

		/// <summary></summary>
		public void Reset()
		{
			IsFirstChunk = true;
			Device       = null;
			Direction    = IODirection.None;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
