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
// MKY Version 1.0.28 Development
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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Utility methods extending <see cref="BitConverter"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class BitConverterEx
	{
		/// <summary></summary>
		[CLSCompliant(false)]
		public static uint ToUInt32(BitVector32 bits, int bit)
		{
			if (bits[bit])
				return (1u << bit);
			else
				return (0);
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public static uint ToUInt32(BitVector32 bits, int bit1, int bit2)
		{
			return (ToUInt32(bits, bit1) + ToUInt32(bits, bit2));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Directly referring to given object for performance reasons.")]
		[CLSCompliant(false)]
		public static void FromUInt32(ref BitVector32 bits, uint value, int bit)
		{
			bits[bit] = ((value & (1u << bit)) != 0);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Directly referring to given object for performance reasons.")]
		[CLSCompliant(false)]
		public static void FromUInt32(ref BitVector32 bits, uint value, int bit1, int bit2)
		{
			FromUInt32(ref bits, value, bit1);
			FromUInt32(ref bits, value, bit2);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
