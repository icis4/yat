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
// MKY Version 1.0.29
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MKY.Globalization
{
	/// <summary>
	/// Some CultureInfo utilities.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class CultureInfoEx
	{
		/// <summary>
		/// Returns the most appropriate culture info from a collection of culture information.
		/// </summary>
		public static CultureInfo GetMostAppropriateCulture(IEnumerable<CultureInfo> items)
		{
			CultureInfo ci;
			var l = new List<CultureInfo>(items);

			// Verify that list contains items
			if (l.Count <= 0)
				return (null);

			// 1st prio: The current culture
			ci = CultureInfo.CurrentCulture;
			if (l.Contains(ci))
				return (ci);

			// 2nd prio: The culture of the UI
			ci = CultureInfo.CurrentUICulture;
			if (l.Contains(ci))
				return (ci);

			// 3rd prio: English (United States)
			ci = CultureInfo.GetCultureInfo("en-US");
			if (l.Contains(ci))
				return (ci);

			// 4th prio: The first entry in the list
			return (l[0]);
		}

		/// <summary>
		/// Returns the most appropriate UI culture info from a collection of culture information.
		/// </summary>
		public static CultureInfo GetMostAppropriateUICulture(IEnumerable<CultureInfo> items)
		{
			CultureInfo ci;
			var l = new List<CultureInfo>(items);

			// Verify that list contains items
			if (l.Count <= 0)
				return (null);

			// 1st prio: The culture of the UI
			ci = CultureInfo.CurrentUICulture;
			if (l.Contains(ci))
				return (ci);

			// 2nd prio: The current culture
			ci = CultureInfo.CurrentCulture;
			if (l.Contains(ci))
				return (ci);

			// 3rd prio: English (United States)
			ci = CultureInfo.GetCultureInfo("en-US");
			if (l.Contains(ci))
				return (ci);

			// 4th prio: The first entry in the list
			return (l[0]);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
