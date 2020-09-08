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
// Copyright © 2010-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MKY
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ConvertEx
	{
		/// <summary>
		/// Converts the given values into a hexadecimal string (e.g. "0A").
		/// </summary>
		public static string ToHexString(byte value)
		{
			return (value.ToString("X2", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Converts the given values into a hexadecimal string (e.g. "0A FF 20").
		/// </summary>
		public static string ToHexString(IEnumerable<byte> values)
		{
			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (byte b in values)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(" ");

				sb.Append(ToHexString(b));
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Converts the given values into a hexadecimal string (e.g. "0x0A 0xFF 0x20").
		/// </summary>
		public static string ToHexString(string prefix, IEnumerable<byte> values)
		{
			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (byte b in values)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(" ");

				if (!string.IsNullOrEmpty(prefix))
					sb.Append(prefix);

				sb.Append(ToHexString(b));
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Converts the given values into a hexadecimal string (e.g. "0Ah FFh 20h").
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to deal with arbitrary string content...")]
		public static string ToHexString(IEnumerable<byte> values, string suffix)
		{
			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (byte b in values)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(" ");

				sb.Append(ToHexString(b));

				if (!string.IsNullOrEmpty(suffix))
					sb.Append(suffix);
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
