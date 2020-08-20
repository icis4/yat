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
// YAT Version 2.2.0 Development
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
using System.Globalization;
using System.Threading;

using MKY.Net.Test;
using MKY.Threading;

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	[TestFixture]
	public class BreakTest
	{
		#region TestDelayKeyword
		//==========================================================================================
		// TestDelayKeyword
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestDelayKeyword()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
			using (var terminalTx = TerminalFactory.CreateTerminal(settingsA))
			{
				try
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					using (var terminalRx = TerminalFactory.CreateTerminal(settingsB))
					{
						try
						{
							Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started!");
							Utilities.WaitForConnection(terminalTx, terminalRx);

							const int DelayTime = 2000; // 3 * IsSendingForSomeTime is 1200 ms

							string textDelayed = string.Format(CultureInfo.InvariantCulture, @"ABC\!(Delay({0}))DEF", DelayTime);
							int textDelayedByteCount = 3; // Only ABC must be sent.
							string textCompleted = @"DEF";
							int textCompletedByteCount = (3 + 2); // Fixed to default of <CR><LF>.
							int expectedTotalByteCount = 0;
							int expectedTotalLineCount = 0;

							// Send:
							terminalTx.SendTextLine(textDelayed);
							expectedTotalByteCount += textDelayedByteCount;
							expectedTotalLineCount++;
							Utilities.WaitForIsSendingForSomeTime(terminalTx);
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Break:
							terminalTx.Break();
							Thread.Sleep(DelayTime); // Delay itself cannot be breaked, only subsequent data.
							Utilities.WaitForIsNoLongerSending(terminalTx);
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Send again to resume break:
							terminalTx.SendTextLine(textCompleted);
							expectedTotalByteCount += textCompletedByteCount;
						////expectedTotalLineCount++ does not apply, only already started line gets completed.
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Wait to ensure that no operation is ongoing anymore and verify again:
							Utilities.WaitForReverification();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Refresh and verify again:
							terminalTx.RefreshRepositories();
							terminalRx.RefreshRepositories();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);
						}
						finally // Properly stop even in case of an exception, e.g. a failed assertion.
						{
							terminalRx.Stop();
							Utilities.WaitForStop(terminalRx);
						}
					} // using (terminalB)
				}
				finally // Properly stop even in case of an exception, e.g. a failed assertion.
				{
					terminalTx.Stop();
					Utilities.WaitForStop(terminalTx);
				}
			} // using (terminalA)
		}

		#endregion

		#region TestLineDelayAndIntervalKeywords
		//==========================================================================================
		// TestLineDelayAndIntervalKeywords
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestLineDelayAndIntervalKeywords([Values(@"\!(LineDelay(2000))", @"\!(LineInterval(2000))")] string keyword)
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
			using (var terminalTx = TerminalFactory.CreateTerminal(settingsA))
			{
				try
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					using (var terminalRx = TerminalFactory.CreateTerminal(settingsB))
					{
						try
						{
							Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started!");
							Utilities.WaitForConnection(terminalTx, terminalRx);

							const int DelayTime = 2000; // 3 * IsSendingForSomeTime is 1200 ms
													  //// Value must match argument values.
							string[] textDelayed = new string[] { "ABC" + keyword, "DEF" };
							string textNormal = "XYZ";
							int textLineByteCount = (3 + 2); // Fixed to default of <CR><LF>.
							int expectedTotalByteCount = 0;
							int expectedTotalLineCount = 0;

							// Send:
							terminalTx.SendTextLines(textDelayed);
							expectedTotalByteCount += textLineByteCount;
							expectedTotalLineCount++;
							Utilities.WaitForIsSendingForSomeTime(terminalTx);
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Break:
							terminalTx.Break();
							Thread.Sleep(DelayTime); // Delay itself cannot be breaked, only subsequent data.
							Utilities.WaitForIsNoLongerSending(terminalTx);
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Send again to resume break:
							terminalTx.SendTextLine(textNormal);
							expectedTotalByteCount += textLineByteCount;
							expectedTotalLineCount++;
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Wait to ensure that no operation is ongoing anymore and verify again:
							Utilities.WaitForReverification();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Refresh and verify again:
							terminalTx.RefreshRepositories();
							terminalRx.RefreshRepositories();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);
						}
						finally // Properly stop even in case of an exception, e.g. a failed assertion.
						{
							terminalRx.Stop();
							Utilities.WaitForStop(terminalRx);
						}
					} // using (terminalB)
				}
				finally // Properly stop even in case of an exception, e.g. a failed assertion.
				{
					terminalTx.Stop();
					Utilities.WaitForStop(terminalTx);
				}
			} // using (terminalA)
		}

		#endregion

		#region TestLineRepeatKeyword
		//==========================================================================================
		// TestLineRepeatKeyword
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestLineRepeatKeyword()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
			settingsA.Display.MaxLineCount = 10000; // Running in NUnit revealed ~2000 lines.
			using (var terminalTx = TerminalFactory.CreateTerminal(settingsA))
			{
				try
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					settingsB.Display.MaxLineCount = 10000; // Running in NUnit revealed ~2000 lines.
					using (var terminalRx = TerminalFactory.CreateTerminal(settingsB))
					{
						try
						{
							Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started!");
							Utilities.WaitForConnection(terminalTx, terminalRx);

							string textRepeat = @"ABC\!(LineRepeat)";
							string textNormal = "XYZ";
							int textLineByteCount = (3 + 2); // Fixed to default of <CR><LF>.

							// Send:
							terminalTx.SendTextLine(textRepeat);
							Utilities.WaitForIsSendingForSomeTime(terminalTx);

							// Break:
							terminalTx.Break();
							Utilities.WaitForIsNoLongerSending(terminalTx);
							Thread.Sleep(500); // Wait some more for Rx to complete.

							// Verify Tx/Rx:
							var txByteCount = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
							var txLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
							var rxByteCount = terminalRx.GetRepositoryByteCount(RepositoryType.Rx);
							var rxLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
							Assert.That(rxByteCount, Is.EqualTo(txByteCount));
							Assert.That(rxByteCount, Is.GreaterThan(500 * textLineByteCount));
							Assert.That(rxLineCount, Is.EqualTo(txLineCount));
							Assert.That(rxLineCount, Is.GreaterThan(500)); // Running in NUnit revealed ~2000 lines.
							Assert.That(rxLineCount, Is.EqualTo((int)(Math.Round((double)txByteCount / textLineByteCount))));

							// Send again to resume break:
							terminalTx.SendTextLine(textNormal);
							var expectedTotalByteCount = (txByteCount + textLineByteCount);
							var expectedTotalLineCount = (txLineCount + 1);
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Wait to ensure that no operation is ongoing anymore and verify again:
							Utilities.WaitForReverification();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Refresh and verify again:
							terminalTx.RefreshRepositories();
							terminalRx.RefreshRepositories();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);
						}
						finally // Properly stop even in case of an exception, e.g. a failed assertion.
						{
							terminalRx.Stop();
							Utilities.WaitForStop(terminalRx);
						}
					} // using (terminalB)
				}
				finally // Properly stop even in case of an exception, e.g. a failed assertion.
				{
					terminalTx.Stop();
					Utilities.WaitForStop(terminalTx);
				}
			} // using (terminalA)
		}

		#endregion

		#region TestSendFile
		//==========================================================================================
		// TestSendFile
		//==========================================================================================

		/// <summary></summary>
		[Test] // Test is mandatory, it shall not be excludable. 'IPv4LoopbackIsAvailable' is probed below.
		public virtual void TestSendFile()
		{
			if (!ConfigurationProvider.Configuration.IPv4LoopbackIsAvailable)
				Assert.Ignore("No IPv4 loopback is available, therefore this test is excluded. Ensure that IPv4 loopback is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settingsA = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
			settingsA.Display.MaxLineCount = 10000; // Running in NUnit revealed 800..1000 lines.
			settingsA.TextTerminal.LineSendDelay = new TextLineSendDelaySettingTuple(true, 1, 1); // Delay of 1 ms per line.
			using (var terminalTx = TerminalFactory.CreateTerminal(settingsA))
			{
				try
				{
					Assert.That(terminalTx.Start(), Is.True, "Terminal A could not be started!");

					var settingsB = Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
					settingsB.Display.MaxLineCount = 10000; // Running in NUnit revealed 800..1000 lines.
					using (var terminalRx = TerminalFactory.CreateTerminal(settingsB))
					{
						try
						{
							Assert.That(terminalRx.Start(), Is.True, "Terminal B could not be started!");
							Utilities.WaitForConnection(terminalTx, terminalRx);

							// Send:
							var initial = DateTime.Now;
							var fi = Files.TextSendFile.Item[StressFile.Huge]; // 10000 lines would take about 10..20 seconds.
							terminalTx.SendFile(fi.Path);
							Utilities.WaitForIsSendingForSomeTime(terminalTx);

							// Break:
							ThreadEx.SleepUntilOffset(initial, 2500); // About fourth the lines must have already been sent at breaking below.
							terminalTx.Break();
							Utilities.WaitForIsNoLongerSending(terminalTx);
							Thread.Sleep(500); // Wait some more for Rx to complete.

							// Verify Tx/Rx:
							var txByteCount = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
							var txLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
							var rxByteCount = terminalRx.GetRepositoryByteCount(RepositoryType.Rx);
							var rxLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
							Assert.That(rxByteCount, Is.EqualTo(txByteCount));
							Assert.That(rxByteCount, Is.GreaterThan(500 * fi.LineByteCount));
							Assert.That(rxLineCount, Is.EqualTo(txLineCount));
							Assert.That(rxLineCount, Is.GreaterThan(500)); // Running in NUnit revealed 800..1000 lines.
							Assert.That(rxLineCount, Is.EqualTo((int)(Math.Round((double)txByteCount / fi.LineByteCount))));

							// Send again to resume break:
							string textNormal = "123"; // File contains letters.
							int textLineByteCount = (3 + 2); // Fixed to default of <CR><LF>.
							terminalTx.SendTextLine(textNormal);
							var expectedTotalByteCount = (txByteCount + textLineByteCount);
							var expectedTotalLineCount = (txLineCount + 1);
							Utilities.WaitForTransmissionAndAssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Wait to ensure that no operation is ongoing anymore and verify again:
							Utilities.WaitForReverification();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);

							// Refresh and verify again:
							terminalTx.RefreshRepositories();
							terminalRx.RefreshRepositories();
							Utilities.AssertCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount);
						}
						finally // Properly stop even in case of an exception, e.g. a failed assertion.
						{
							terminalRx.Stop();
							Utilities.WaitForStop(terminalRx);
						}
					} // using (terminalB)
				}
				finally // Properly stop even in case of an exception, e.g. a failed assertion.
				{
					terminalTx.Stop();
					Utilities.WaitForStop(terminalTx);
				}
			} // using (terminalA)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
