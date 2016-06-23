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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace MKY.IO
{
	/// <summary></summary>
	public static class TextEx
	{
		/// <summary>
		/// Converts a string array to a multi-line string, using <see cref="Environment.NewLine"/>
		/// as the end-of-line separator.
		/// </summary>
		public static string ToLines(string[] s)
		{
			return (ToLines(s, Environment.NewLine));
		}

		/// <summary>
		/// Converts a string array to a multi-line string, using the given end-of-line separator.
		/// </summary>
		public static string ToLines(string[] s, string newLine)
		{
			using (StringWriter sw = new StringWriter())
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
			List<string> lines = new List<string>();

			using (StringReader sr = new StringReader(s))
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
