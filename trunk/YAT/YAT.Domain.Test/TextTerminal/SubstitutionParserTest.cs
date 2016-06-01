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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	public static class SubstitutionParserTestData
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
				// ToUpper.
				yield return (new TestCaseData(CharSubstitution.ToUpper, @"\c(A)\c(b)CdEfGhIiKlMnOpQrStUvWxYz<Cr><Lf>", new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x49, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x0D, 0x0A } ).SetName("ToUpper"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class SubstitutionParserTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Test SubstitutionParser
		//------------------------------------------------------------------------------------------
		// Test SubstitutionParser
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Justification = "The naming emphasizes the difference between bytes and other parameters.")]
		[Test, TestCaseSource(typeof(SubstitutionParserTestData), "TestCases")]
		public virtual void TestSubstitutionParser(CharSubstitution substitution, string s, byte[] expectedBytes)
		{
			using (Domain.Parser.SubstitutionParser p = new Domain.Parser.SubstitutionParser())
			{
				byte[] actualBytes = p.Parse(s);
				Assert.AreEqual(expectedBytes, actualBytes);
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
