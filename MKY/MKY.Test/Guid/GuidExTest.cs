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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;

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
				yield return (new TestCaseData(true, @".\Prefix-dcf25dde-947a-4470-8567-b0dde2459933-Postfix.ext", "Prefix-", "-Postfix", new System.Guid("dcf25dde-947a-4470-8567-b0dde2459933")));
				yield return (new TestCaseData(true, @".\Prefix-dcf25dde-947a-4470-8567-b0dde2459933.ext",         "Prefix-", "",         new System.Guid("dcf25dde-947a-4470-8567-b0dde2459933")));
				yield return (new TestCaseData(true,        @".\dcf25dde-947a-4470-8567-b0dde2459933-Postfix.ext", "",        "-Postfix", new System.Guid("dcf25dde-947a-4470-8567-b0dde2459933")));
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
		[Test, TestCaseSource(typeof(GuidExTestData), "TestCases")]
		public virtual void TestTryCreateGuidFromFilePath(bool isValid, string filePath, string prefix, string postfix, System.Guid expectedGuid)
		{
			System.Guid actualGuid;
			if (GuidEx.TryCreateGuidFromFilePath(filePath, prefix, postfix, out actualGuid))
			{
				Assert.IsTrue(isValid);
				Assert.AreEqual(expectedGuid, actualGuid);
			}
			else
			{
				Assert.IsFalse(isValid);
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
