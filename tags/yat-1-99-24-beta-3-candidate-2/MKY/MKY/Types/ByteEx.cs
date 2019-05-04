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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Types
{
	/// <summary>
	/// Byte/byte utility methods.
	/// </summary>
	public class ByteEx
	{
		/// <summary>
		/// Converts value into binary string (e.g. "00010100").
		/// </summary>
		public static string ConvertToBinaryString(byte value)
		{
			return (UInt64Ex.ConvertToNumericBaseString(2, value, byte.MaxValue));
		}

		/// <summary>
		/// Converts value into octal string (e.g. "024").
		/// </summary>
		public static string ConvertToOctalString(byte value)
		{
			return (UInt64Ex.ConvertToNumericBaseString(8, value, byte.MaxValue));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================