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
// MKY Version 1.0.30
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MKY.IO
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class TextEx
	{
		/// <summary>
		/// Converts a string array to a multi-line string, using <see cref="Environment.NewLine"/>
		/// as the end-of-line separator.
		/// </summary>
		public static string ToLines(string[] s, IFormatProvider formatProvider)
		{
			return (ToLines(s, formatProvider, Environment.NewLine));
		}

		/// <summary>
		/// Converts a string array to a multi-line string, using the given end-of-line separator.
		/// </summary>
		public static string ToLines(string[] s, IFormatProvider formatProvider, string newLine)
		{
			using (var sw = new StringWriter(formatProvider))
			{
				sw.NewLine = newLine;

				if (s != null)
				{
					for (int i = 0; i < s.Length; i++)
					{
						if (i < (s.Length - 1))
							sw.WriteLine(s[i]);
						else
							sw.Write(s[i]);
					}
				}

				return (sw.ToString());
			}
		}

		/// <summary>
		/// Converts a multi-line string to a string array.
		/// </summary>
		public static string[] FromLines(string s)
		{
			var lines = new List<string>();

			using (var sr = new StringReader(s))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
					lines.Add(line);
			}

			return (lines.ToArray());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
