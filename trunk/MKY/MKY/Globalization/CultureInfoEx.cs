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
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;
using System.Globalization;

namespace MKY.Globalization
{
	/// <summary>
	/// Some CultureInfo utilities.
	/// </summary>
	public static class CultureInfoEx
	{
		/// <summary>
		/// Returns the most appropriate culture info from a collection of culture infos.
		/// </summary>
		public static CultureInfo GetMostAppropriateCultureInfo(IEnumerable<CultureInfo> cultureInfos)
		{
			CultureInfo ci;
			List<CultureInfo> l = new List<CultureInfo>(cultureInfos);

			// Verify that list contains items
			if (l.Count <= 0)
				return (null);

			// 1st prio: The culture of the user interface
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
