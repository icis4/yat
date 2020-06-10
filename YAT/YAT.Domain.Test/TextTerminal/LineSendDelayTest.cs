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
// YAT Version 2.1.1 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Threading;

using MKY;
using MKY.Net.Test;
using MKY.Threading;

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	[TestFixture]
	public class LineSendDelayTest
	{
		#region TestDefault
		//==========================================================================================
		// TestDefault
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestDefault()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			const int DelayTime = 1000;

			var settingsA = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
			settingsA.TextTerminal.LineSendDelay = new TextLineSendDelaySettingTuple(true, DelayTime, 1); // Delay of 1000 ms per line.
			using (var terminalTx = new Domain.TextTerminal(settingsA))
			{
				Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started");

				var settingsB = Utilities.GetTcpAutoSocketOnIPv4LoopbackTextSettings();
				using (var terminalRx = new Domain.TextTerminal(settingsB))
				{
					Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started");
					Utilities.WaitForConnection(terminalTx, terminalRx);

					string[] textMulti = ArrayEx.CreateAndInitializeInstance(20, "ABC"); // 20 * 1000 ms = 20 s
					string textNormal = "ABC";
					int textLineByteCount = (3 + 2); // Fixed to default of <CR><LF>.
					int expectedTotalByteCount = 0;
					int expectedTotalLineCount = 0;

					// Send:
					var initial = DateTime.Now;
					terminalTx.SendTextLines(textMulti);

					// Break:
					ThreadEx.SleepUntilOffset(initial, 2500); // 3 lines must have already been sent at breaking below.
					terminalTx.Break();
					Thread.Sleep(DelayTime); // Delay itself cannot be breaked, only subsequent data.
					Utilities.WaitForIsNoLongerSending(terminalTx);

					// Verify;
					expectedTotalByteCount += (3 * textLineByteCount);
					expectedTotalLineCount +=  3;
					Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

					// Send again to resume break:
					terminalTx.SendTextLine(textNormal);
					expectedTotalByteCount += textLineByteCount;
					expectedTotalLineCount++;
					Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

					// Refresh and verify again:
					terminalTx.RefreshRepositories();
					terminalRx.RefreshRepositories();
					Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

					terminalRx.Stop();
					Utilities.WaitForDisconnection(terminalRx);
				} // using (terminalB)

				terminalTx.Stop();
				Utilities.WaitForDisconnection(terminalTx);
			} // using (terminalA)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================