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

		#region Tests > ConsecutiveServerClientConnectAndShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutiveServerClientConnectAndShutdown()
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// A cycle takes around 5 seconds. 10 cycles around a minute.
		/// </remarks>
		[Test]
		[Repeat(10)]
		[EnduranceCategory, MinuteDurationCategory(1)]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance01Minute()
		{
			var t = new SimpleUdpConnectionTest();
			t.TestServerClientConnectAndClientShutdown();
			t.TestServerClientConnectAndServerShutdown();
			t.TestServerClientConnectAndServerShutdown();
			t.TestServerClientConnectAndClientShutdown();
			t.TestServerClientConnectAndClientShutdown();
			t.TestServerClientConnectAndServerShutdown();
			t.TestServerClientConnectAndServerShutdown();
			t.TestServerClientConnectAndClientShutdown();
		}

		/// <summary></summary>
		[Test]
		[Repeat(10)]
		[EnduranceCategory, MinuteDurationCategory(10)]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance10Minutes()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test]
		[Repeat(6)]
		[EnduranceCategory, MinuteDurationCategory(60)]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance60Minutes()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance10Minutes();
		}

		/// <summary></summary>
		[Test]
		[Repeat(int.MaxValue)]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEnduranceForever()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance60Minutes();
		}

		#endregion

		#region Tests > ConsecutivePairSocketPairSocketConnectAndShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutivePairSocketPairSocketConnectAndShutdown()
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// A cycle takes around 5 seconds. 10 cycles around a minute.
		/// </remarks>
		[Test]
		[Repeat(10)]
		[EnduranceCategory, MinuteDurationCategory(1)]
		public virtual void TestConsecutivePairSocketPairSocketConnectAndShutdownEndurance01Minute()
		{
			var t = new SimpleUdpConnectionTest();
			t.TestPairSocketPairSocketConnectAndPairSocketBShutdown();
			t.TestPairSocketPairSocketConnectAndPairSocketAShutdown();
			t.TestPairSocketPairSocketConnectAndPairSocketAShutdown();
			t.TestPairSocketPairSocketConnectAndPairSocketBShutdown();
			t.TestPairSocketPairSocketConnectAndPairSocketBShutdown();
			t.TestPairSocketPairSocketConnectAndPairSocketAShutdown();
			t.TestPairSocketPairSocketConnectAndPairSocketAShutdown();
			t.TestPairSocketPairSocketConnectAndPairSocketBShutdown();
		}

		/// <summary></summary>
		[Test]
		[Repeat(10)]
		[EnduranceCategory, MinuteDurationCategory(10)]
		public virtual void TestConsecutivePairSocketPairSocketConnectAndShutdownEndurance10Minutes()
		{
			TestConsecutivePairSocketPairSocketConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test]
		[Repeat(6)]
		[EnduranceCategory, MinuteDurationCategory(60)]
		public virtual void TestConsecutivePairSocketPairSocketConnectAndShutdownEndurance60Minutes()
		{
			TestConsecutivePairSocketPairSocketConnectAndShutdownEndurance10Minutes();
		}

		/// <summary></summary>
		[Test]
		[Repeat(int.MaxValue)]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutivePairSocketPairSocketConnectAndShutdownEnduranceForever()
		{
			TestConsecutivePairSocketPairSocketConnectAndShutdownEndurance60Minutes();
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
