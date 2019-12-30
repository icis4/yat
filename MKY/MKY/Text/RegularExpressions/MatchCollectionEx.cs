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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MKY.Text.RegularExpressions
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class MatchCollectionEx
	{
		/// <summary>
		/// Unfolds the captures of the given matches to a string array.
		/// </summary>
		public static string[] UnfoldCapturesToStringArray(MatchCollection matches)
		{
			var values = new List<string>(); // No preset needed, the default behavior is good enough.

			foreach (Match m in matches)
			{
				for (int gnum = 1; gnum <= m.Groups.Count; gnum++)
				{
					foreach (Capture c in m.Groups[gnum].Captures)
						values.Add(c.Value);
				}
			}

			return (values.ToArray());
		}

		/// <summary>
		/// Unfolds the captures of the given matches to a tuple array.
		/// </summary>
		public static Tuple<Match, Group, Capture>[] UnfoldCapturesToTupleArray(MatchCollection matches)
		{
			var values = new List<Tuple<Match, Group, Capture>>(); // No preset needed, the default behavior is good enough.

			foreach (Match m in matches)
			{
				for (int gnum = 1; gnum <= m.Groups.Count; gnum++)
				{
					foreach (Capture c in m.Groups[gnum].Captures)
						values.Add(new Tuple<Match, Group, Capture>(m, m.Groups[gnum], c));
				}
			}

			return (values.ToArray());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
