﻿//==================================================================================================
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
		[EnduranceCategory, StandardDurationCategory.Minute1Attribute]
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
		[Repeat(15)]
		[EnduranceCategory, StandardDurationCategory.Minutes15Attribute]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance15Minutes()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test]
		[Repeat(int.MaxValue)]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEnduranceForever()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance01Minute();
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
		[EnduranceCategory, StandardDurationCategory.Minute1Attribute]
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
		[Repeat(15)]
		[EnduranceCategory, StandardDurationCategory.Minutes15Attribute]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance15Minutes()
		{
			TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test]
		[Repeat(int.MaxValue)]
		[EnduranceCategory, InfiniteDurationCategory]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEnduranceForever()
		{
			TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance01Minute();
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
