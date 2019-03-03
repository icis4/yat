﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.26 Development
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

using System.Diagnostics.CodeAnalysis;

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and obvious.")]
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
