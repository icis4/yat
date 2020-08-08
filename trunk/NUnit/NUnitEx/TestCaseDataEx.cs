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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NUnit.Framework;

namespace NUnitEx
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class TestCaseDataEx
	{
		/// <summary></summary>
		public static TestCaseData ToTestCase(TestCaseData metaDataBase, string nameToAppend, IEnumerable<string> categoriesToMerge, params object[] args)
		{
			// Arguments:
			var tc = new TestCaseData(args);

			// Name:
			tc.SetName(metaDataBase.TestName + ((nameToAppend != null) ? (nameToAppend) : ""));

			// Category(ies):
			{
				foreach (var cat in metaDataBase.Categories)
					tc.SetCategory((string)cat);
			}
			if (categoriesToMerge != null)
			{
				foreach (var catToMerge in categoriesToMerge)
					tc.SetCategory(catToMerge);
			}

			return (tc);
		}

		/// <summary></summary>
		public static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TestCaseData metaDataToMerge, params object[] args)
		{
			// Arguments:
			var tc = new TestCaseData(args);

			// Name:
			tc.SetName(descriptor.Name + ((metaDataToMerge != null) ? (metaDataToMerge.TestName) : ""));

			// Category(ies):
			{
				foreach (var cat in descriptor.Categories)
					tc.SetCategory(cat);
			}
			if (metaDataToMerge != null)
			{
				foreach (var catToMerge in metaDataToMerge.Categories)
					tc.SetCategory((string)catToMerge);
			}

			return (tc);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
