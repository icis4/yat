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
// NUnit Version 1.0.21
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using NUnit.Framework;

namespace NUnit
{
	/// <summary></summary>
	[TestFixture]
	public class ContextDetectorTest
	{
		/// <summary></summary>
		[Test]
		public void TestIsInNUnit()
		{
			Assert.IsTrue(ContextDetector.IsRunningInNUnit);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
