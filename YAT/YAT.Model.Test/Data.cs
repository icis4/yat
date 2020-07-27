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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using NUnit;
using NUnit.Framework;

using YAT.Settings.Model;

#endregion

namespace YAT.Model.Test
{
	/// <remarks>Just named "Data" instead of "TestCaseData" for compactness.</remarks>
	public static class Data
	{
		/// <remarks>Explicitly using two settings for "Pair" test cases, instead of enumerable generic number of settings.</remarks>
		public static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TestCaseData metaDataToMerge, TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, params object[] args)
		{
			return (TestCaseDataEx.ToTestCase(descriptor, metaDataToMerge, settingsA, settingsB, args));
		}

		/// <remarks>Explicitly using a single setting for "Self" test cases, instead of enumerable generic number of settings.</remarks>
		public static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TestCaseData metaDataToMerge, TerminalSettingsRoot settings, params object[] args)
		{
			return (TestCaseDataEx.ToTestCase(descriptor, metaDataToMerge, settings, args));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
