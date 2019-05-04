﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author: stroppel-1$
// $Date: Donnerstag, 5. September 2013 11:02:41$
// $Revision: 2$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.9
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

namespace MKY.IO.Serial
{
	/// <summary></summary>
	public static class XOnXOff
	{
		/// <summary></summary>
		public const byte XOnByte = 0x11;

		/// <summary></summary>
		public const byte XOffByte = 0x13;

		/// <summary></summary>
		public const string XOnDescription = "XOn = 11h (DC1)";

		/// <summary></summary>
		public const string XOffDescription = "XOff = 13h (DC3)";

		/// <summary>
		/// Returns whether the given byte is an XOn or XOff byte.
		/// </summary>
		public static bool IsXOnOrXOffByte(byte b)
		{
			return ((b == XOnByte) || (b == XOffByte));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================