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
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using NUnit;
using NUnit.Framework;

namespace YAT.Model.Test
{
	/// <summary></summary>
	[TestFixture]
	public class StressTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > HeavySimultaneousUsage
		//------------------------------------------------------------------------------------------
		// Tests > HeavySimultaneousUsage
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts 100 terminals in parallel and lets them work on 100 data firing devices.
		/// </summary>
		[Test]
		[StressCategory, MinuteDurationCategory(1)]
		public virtual void HeavySimultaneousUsage()
		{
			// Nothing yet (waiting for PowerShell intro to build stress test script).
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
