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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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
	public class ConsecutiveTcpConnectionTest
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
		/// A cycle takes around 15 seconds. 4 cycles around a minute.
		/// </remarks>
		[Test]
		[Repeat(4)]
		[EnduranceCategory, MinuteDurationCategory(1)]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance01Minute()
		{
			var t = new SimpleTcpConnectionTest();
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

		#region Tests > ConsecutiveAutoSocketAutoSocketConnectAndShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutiveAutoSocketAutoSocketConnectAndShutdown()
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// A cycle takes around 15 seconds. 4 cycles around a minute.
		/// </remarks>
		[Test]
		[Repeat(4)]
		[EnduranceCategory, MinuteDurationCategory(1)]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance01Minute()
		{
			var t = new SimpleTcpConnectionTest();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketBShutdown();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketAShutdown();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketAShutdown();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketBShutdown();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketBShutdown();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketAShutdown();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketAShutdown();
			t.TestAutoSocketAutoSocketConnectAndAutoSocketBShutdown();
		}

		/// <summary></summary>
		[Test]
		[Repeat(10)]
		[EnduranceCategory, MinuteDurationCategory(10)]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance10Minutes()
		{
			TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test]
		[Repeat(6)]
		[EnduranceCategory, MinuteDurationCategory(60)]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance60Minutes()
		{
			TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance10Minutes();
		}

		/// <summary></summary>
		[Test]
		[Repeat(int.MaxValue)]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEnduranceForever()
		{
			TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance60Minutes();
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
