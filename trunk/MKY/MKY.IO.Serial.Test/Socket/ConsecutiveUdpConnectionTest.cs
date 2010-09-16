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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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

using NUnit.Framework;

using MKY.IO.Serial;

namespace MKY.IO.Serial.Test.Socket
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
		[Test, Repeat(10)]
		public virtual void TestConsecutiveConnectAndShutdown()
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
		[Test, Repeat(10), Category("Endurance 10 Minutes")]
		public virtual void TestConsecutiveConnectAndShutdownEndurance10Minutes()
		{
			TestConsecutiveConnectAndShutdown();
		}

		/// <summary></summary>
		[Test, Repeat(6), Category("Endurance 60 Minutes")]
		public virtual void TestConsecutiveConnectAndShutdownEndurance60Minutes()
		{
			TestConsecutiveConnectAndShutdownEndurance10Minutes();
		}

		/// <summary></summary>
		[Test, Repeat(int.MaxValue), Category("Endurance Forever")]
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
