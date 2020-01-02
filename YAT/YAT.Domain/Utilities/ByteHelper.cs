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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Globalization;
using System.Text;

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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string FormatHexString(byte value, bool showRadix = true)
		{
			return (FormatHexString(new byte[] { value }, showRadix));
		}

		/// <summary>
		/// Formats the given values into a hexadecimal string (e.g. "0Ah FFh 20h").
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'FFh'...")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string FormatHexString(IEnumerable<byte> values, bool showRadix = true)
		{
			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (byte b in values)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(" ");

				if (showRadix)
					sb.Append(b.ToString("X2", CultureInfo.InvariantCulture) + "h");
				else
					sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
