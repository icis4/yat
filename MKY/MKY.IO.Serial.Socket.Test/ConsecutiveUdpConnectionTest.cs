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
// MKY Version 1.0.20
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using MKY.IO.Serial.Socket;

using NUnit;
using NUnit.Framework;

namespace MKY.IO.Serial.Socket.Test
{
	/// <summary></summary>
	[TestFixture]
	public class ConsecutiveUdpConnectionTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > ConsecutiveConnectAndShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutiveConnectAndShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <remarks>
		/// A cycle takes around 5 seconds. 10 cycles around a minute.
		/// </remarks>
		[Test]
		[Repeat(10)]
		[EnduranceCategory, MinuteDurationCategory(1)]
		public virtual void TestConsecutiveConnectAndShutdownEndurance01Minute()
		{
			SimpleUdpConnectionTest t = new SimpleUdpConnectionTest();
			t.TestConnectAndShutdown();
			t.TestConnectAndShutdown();
			t.TestConnectAndShutdown();
			t.TestConnectAndShutdown();
			t.TestConnectAndShutdown();
			t.TestConnectAndShutdown();
			t.TestConnectAndShutdown();
			t.TestConnectAndShutdown();
		}

		/// <summary></summary>
		[Test]
		[Repeat(10)]
		[EnduranceCategory, MinuteDurationCategory(10)]
		public virtual void TestConsecutiveConnectAndShutdownEndurance10Minutes()
		{
			TestConsecutiveConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test]
		[Repeat(6)]
		[EnduranceCategory, MinuteDurationCategory(60)]
		public virtual void TestConsecutiveConnectAndShutdownEndurance60Minutes()
		{
			TestConsecutiveConnectAndShutdownEndurance10Minutes();
		}

		/// <summary></summary>
		[Test]
		[Repeat(int.MaxValue)]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveConnectAndShutdownEnduranceForever()
		{
			TestConsecutiveConnectAndShutdownEndurance60Minutes();
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
