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
// NUnit Version 1.0.22
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using NUnit.Framework;

namespace NUnitEx
{
	/// <summary></summary>
	[TestFixture]
	public class TestExecutionContextInformation
	{
		/// <summary></summary>
		[Test]
		public virtual void EstimateTotalExecutionTime()
		{
			// Try to estimate the total execution time based on the number of tests...
		////Framework.Internal.TestExecutionContext.CurrentContext.CurrentTest => https://github.com/nunit/nunit/issues/2914
			// ...taking the duration categories into account.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
