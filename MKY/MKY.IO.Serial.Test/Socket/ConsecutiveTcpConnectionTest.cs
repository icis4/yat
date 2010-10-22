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

		/// <summary></summary>
		/// <remarks>
		/// A cycle takes around 5 seconds. 10 cycles around a minute.
		/// </remarks>
		[Test, Repeat(10), Category("Endurance"), Category("Takes 1 Minute")]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance01Minute()
		{
			SimpleTcpConnectionTest t = new SimpleTcpConnectionTest();
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
		[Test, Repeat(10), Category("Endurance"), Category("Takes 10 Minutes")]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance10Minutes()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test, Repeat(6), Category("Endurance"), Category("Takes 60 Minutes")]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEndurance60Minutes()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance10Minutes();
		}

		/// <summary></summary>
		[Test, Repeat(int.MaxValue), Category("Endurance"), Category("Takes Forever")]
		public virtual void TestConsecutiveServerClientConnectAndShutdownEnduranceForever()
		{
			TestConsecutiveServerClientConnectAndShutdownEndurance60Minutes();
		}

		#endregion

		#region Tests > ConsecutiveAutoSocketAutoSocketConnectAndShutdown()
		//------------------------------------------------------------------------------------------
		// Tests > ConsecutiveAutoSocketAutoSocketConnectAndShutdown()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <remarks>
		/// A cycle takes around 15 seconds. 10 cycles around 3 minutes.
		/// </remarks>
		[Test, Repeat(10), Category("Endurance"), Category("Takes 1 Minute")]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance01Minute()
		{
			SimpleTcpConnectionTest t = new SimpleTcpConnectionTest();
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
		[Test, Repeat(3), Category("Endurance"), Category("Takes 10 Minutes")]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance10Minutes()
		{
			TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance01Minute();
		}

		/// <summary></summary>
		[Test, Repeat(6), Category("Endurance"), Category("Takes 60 Minutes")]
		public virtual void TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance60Minutes()
		{
			TestConsecutiveAutoSocketAutoSocketConnectAndShutdownEndurance10Minutes();
		}

		/// <summary></summary>
		[Test, Repeat(int.MaxValue), Category("Endurance"), Category("Takes Forever")]
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
