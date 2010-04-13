//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public virtual void HeavySimultaneousUsage()
		{
			// nothing yet (waiting for PowerShell intro to build stress test script)
		}

		#endregion

		#endregion

	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
