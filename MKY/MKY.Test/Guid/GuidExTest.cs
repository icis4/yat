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

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

#endregion

namespace MKY.Test.Guid
{
	/// <summary></summary>
	public static class GuidExTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				yield return (new TestCaseData(true, @".\Prefix-dcf25dde-947a-4470-8567-b0dde2459933-Postfix.ext", new System.Guid("dcf25dde-947a-4470-8567-b0dde2459933")));
				yield return (new TestCaseData(true, @".\Prefix-dcf25dde-947a-4470-8567-b0dde2459933.ext",         new System.Guid("dcf25dde-947a-4470-8567-b0dde2459933")));
				yield return (new TestCaseData(true,        @".\dcf25dde-947a-4470-8567-b0dde2459933-Postfix.ext", new System.Guid("dcf25dde-947a-4470-8567-b0dde2459933")));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class GuidExTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > TryCreateGuidFromFilePath
		//------------------------------------------------------------------------------------------
		// Tests > TryCreateGuidFromFilePath
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		[Test, TestCaseSource(typeof(GuidExTestData), "TestCases")]
		public virtual void TestTryParseTolerantly(bool isValid, string s, System.Guid expectedGuid)
		{
			System.Guid actualGuid;
			if (GuidEx.TryParseTolerantly(s, out actualGuid))
			{
				Assert.That(isValid);
				Assert.That(actualGuid, Is.EqualTo(expectedGuid));
			}
			else
			{
				Assert.That(!isValid);
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
