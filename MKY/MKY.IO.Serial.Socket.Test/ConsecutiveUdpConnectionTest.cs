//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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

using NUnit;
using NUnit.Framework;

using MKY.IO.Serial.Socket;

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
