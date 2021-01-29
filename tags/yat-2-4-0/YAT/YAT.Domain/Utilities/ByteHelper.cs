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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

#endregion

namespace YAT.Domain.Utilities
{
	/// <summary>
	/// Static utility class providing byte functionality for YAT.
	/// </summary>
	public static class ByteHelper
	{
		/// <summary>
		/// Formats the given value into a hexadecimal string (e.g. "0Ah").
		/// </summary>
		public static string FormatHexString(byte value, bool showRadix)
		{
			return (FormatHexString(new byte[] { value }, showRadix));
		}

		/// <summary>
		/// Formats the given values into a hexadecimal string (e.g. "0Ah FFh 20h").
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'FFh'...")]
		public static string FormatHexString(IEnumerable<byte> values, bool showRadix)
		{
			if (showRadix)
				return (ConvertEx.ToHexString(values, "h"));
			else
				return (ConvertEx.ToHexString(values));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
