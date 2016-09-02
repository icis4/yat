﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace MKY.IO.Serial.Socket
{
	/// <summary></summary>
	public static class SocketDefaults
	{
		/// <summary></summary>
		public const int FrameSize = 1024;

		/// <summary></summary>
		public const int SocketBufferSize = 2 * FrameSize;

		/// <summary></summary>
		public const int MessageBufferSize = 16 * FrameSize;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
