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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Collections.Generic;
using MKY.IO.Ports.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	public static class GlueCharsOfLineTestData
	{
		#region Test Environment
		//==========================================================================================
		// Test Environment
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static IEnumerable TestEnvironmentSerialPortLoopbackSelfs
		{
			get
			{
				foreach (var loopbackSettings in Utilities.TransmissionSettings.SerialPortLoopbackSelfs)
				{
					// Arguments:
					var tcd = new TestCaseData(loopbackSettings.Value1); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB, Utilities.TestSet command, int transmissionCount).

					// Name:
					tcd.SetName(loopbackSettings.Value2);

					// Category(ies):
					foreach (string cat in loopbackSettings.Value3)
						tcd.SetCategory(cat);

					yield return (tcd);
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class GlueCharsOfLineTest
	{
		#region TestDefault
		//==========================================================================================
		// TestDefault
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(GlueCharsOfLineTestData), "TestEnvironmentSerialPortLoopbackSelfs")] // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is probed below.
		public virtual void TestDefault(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor)
		{
			if (!ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);
			using (var terminal = new Domain.TextTerminal(settings)) // Glueing is enabled by default.
			{
				Assert.That(terminal.Start(), Is.True, "Terminal could not be started");
				Utilities.WaitForConnection(terminal, terminal);

				// Send:
				var beganAt = DateTime.Now;
				var file = SendFilesProvider.FilePaths_StressText.StressFiles[StressTestCase.Normal]; // 300 lines will take about 10..15 seconds.
				var fileTimeout = 15000;
				var fileByteCount = file.Item2;
				var fileLineCount = file.Item3;
				var fileLineByteCount = (fileByteCount / fileLineCount); // Fixed to default of <CR><LF>.
				terminal.SendFile(file.Item1);
				Utilities.WaitForSendingAndVerifyCounts(terminal, fileByteCount, fileLineCount, fileTimeout);
				Utilities.WaitForReceivingAndVerifyCounts(terminal, fileByteCount, fileLineCount);
				var endedAt = DateTime.Now;
				var duration = (endedAt - beganAt);
				Assert.That(duration.TotalMilliseconds, Is.LessThan(fileTimeout));

				// Verify:
				var lines = terminal.RepositoryToDisplayLines(RepositoryType.Bidir);
				var previousLineTimeStamp = DateTime.MinValue;
				foreach (var line in lines)
				{
					Assert.That(line.ByteCount, Is.EqualTo(fileLineByteCount));
					Assert.That(line.CharCount, Is.EqualTo(fileLineByteCount));

					Assert.That(line.TimeStamp, Is.GreaterThanOrEqualTo(previousLineTimeStamp));
					previousLineTimeStamp = line.TimeStamp;
				}

				terminal.Stop();
				Utilities.WaitForDisconnection(terminal);
			} // using (terminal)
		}

		#endregion

		#region TestInfinite
		//==========================================================================================
		// TestInfinite
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(GlueCharsOfLineTestData), "TestEnvironmentSerialPortLoopbackSelfs")] // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is probed below.
		public virtual void TestInfinite(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor)
		{
			if (!ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);
			var gcol = settings.TextTerminal.GlueCharsOfLine;
			gcol.Enabled = true;
			gcol.Timeout = Timeout.Infinite;
			settings.TextTerminal.GlueCharsOfLine = gcol;
			using (var terminal = new Domain.TextTerminal(settings)) // Glueing is enabled by default.
			{
				Assert.That(terminal.Start(), Is.True, "Terminal could not be started");
				Utilities.WaitForConnection(terminal, terminal);

				// PENDING

				terminal.Stop();
				Utilities.WaitForDisconnection(terminal);
			} // using (terminal)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
