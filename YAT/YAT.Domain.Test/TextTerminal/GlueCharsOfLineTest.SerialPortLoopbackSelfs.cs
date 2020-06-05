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
using System.Linq;
using System.Threading;

using MKY.Collections.Generic;
using MKY.IO.Ports.Test;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.TextTerminal
{
	/// <summary></summary>
	[TestFixture]
	public class GlueCharsOfLineTest_SerialPortLoopbackSelfs
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(GenericTestData), "TestEnvironmentSerialPortLoopbackSelfs")] // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is probed below.
		public virtual void TestDefault(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor)
		{
			if (!ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);

			TestDefaultOrInfiniteTimeout(settings);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(GenericTestData), "TestEnvironmentSerialPortLoopbackSelfs")] // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is probed below.
		public virtual void TestInfiniteTimeout(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor)
		{
			if (!ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);
			var gcol = settings.TextTerminal.GlueCharsOfLine;
			gcol.Enabled = true;
			gcol.Timeout = Timeout.Infinite;
			settings.TextTerminal.GlueCharsOfLine = gcol;

			TestDefaultOrInfiniteTimeout(settings);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(GenericTestData), "TestEnvironmentSerialPortLoopbackSelfs")] // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is probed below.
		public virtual void TestMinimumTimout(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor)
		{
			if (!ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);
			var gcol = settings.TextTerminal.GlueCharsOfLine;
			gcol.Timeout = 1;
			settings.TextTerminal.GlueCharsOfLine = gcol;

			TestMinimumTimoutOrTestDisabled(settings);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(GenericTestData), "TestEnvironmentSerialPortLoopbackSelfs")] // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is probed below.
		public virtual void TestDisabled(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor)
		{
			if (!ConfigurationProvider.Configuration.LoopbackSelfsAreAvailable)
				Assert.Ignore("No serial COM port loopback selfs are available, therefore this test is excluded. Ensure that at least one serial COM port loopback self is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);
			var gcol = settings.TextTerminal.GlueCharsOfLine;
			gcol.Enabled = false;
			settings.TextTerminal.GlueCharsOfLine = gcol;

			TestMinimumTimoutOrTestDisabled(settings);
		}

		#endregion

		#region Test Verifications
		//==========================================================================================
		// Test Verifications
		//==========================================================================================

		/// <summary></summary>
		protected virtual void TestDefaultOrInfiniteTimeout(Settings.TerminalSettings settings)
		{
			using (var terminal = new Domain.TextTerminal(settings)) // Glueing is enabled by default.
			{
				Assert.That(terminal.Start(), Is.True, "Terminal could not be started");
				Utilities.WaitForConnection(terminal, terminal);

				// Send:
				var beganAt = DateTime.Now;
				var file = SendFilesProvider.FilePaths_StressText.StressFiles[StressTestCase.Normal]; // 300 lines will take about 9..12 seconds.
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
				VerifyDefaultOrInfiniteTimeout(terminal, fileLineCount, fileLineByteCount);

				// Refresh and verify again:
				terminal.RefreshRepositories();
				VerifyDefaultOrInfiniteTimeout(terminal, fileLineCount, fileLineByteCount);

				terminal.Stop();
				Utilities.WaitForDisconnection(terminal);
			} // using (terminal)
		}

		/// <summary></summary>
		protected virtual void VerifyDefaultOrInfiniteTimeout(Domain.TextTerminal terminal, int fileLineCount, int fileLineByteCount)
		{
			var displayLines = terminal.RepositoryToDisplayLines(RepositoryType.Bidir);
			Assert.That(displayLines.Count, Is.EqualTo(fileLineCount * 2));

			var previousLineTimeStamp = DateTime.MinValue;
			foreach (var dl in displayLines)
			{
				Assert.That(dl.ByteCount, Is.EqualTo(fileLineByteCount));
				Assert.That(dl.CharCount, Is.EqualTo(fileLineByteCount));

				Assert.That(dl.TimeStamp, Is.GreaterThanOrEqualTo(previousLineTimeStamp));
				previousLineTimeStamp = dl.TimeStamp;
			}
		}

		/// <summary></summary>
		protected virtual void TestMinimumTimoutOrTestDisabled(Settings.TerminalSettings settings)
		{
			using (var terminal = new Domain.TextTerminal(settings)) // Glueing is enabled by default.
			{
				Assert.That(terminal.Start(), Is.True, "Terminal could not be started");
				Utilities.WaitForConnection(terminal, terminal);

				// Send:
				var beganAt = DateTime.Now;
				var file = SendFilesProvider.FilePaths_StressText.StressFiles[StressTestCase.Normal]; // 300 lines will take about 9..12 seconds.
				var fileTimeout = 15000;
				var fileByteCount = file.Item2;
				var fileLineCount = file.Item3;
				var fileLineByteCount = (fileByteCount / fileLineCount); // Fixed to default of <CR><LF>.
				terminal.SendFile(file.Item1);                        // ByteCount only, lines are expected to be broken more.
				Utilities.WaitForSendingAndVerifyByteCount(terminal, fileByteCount, fileTimeout);
				Utilities.WaitForReceivingAndVerifyByteCount(terminal, fileByteCount);
				var endedAt = DateTime.Now;
				var duration = (endedAt - beganAt);
				Assert.That(duration.TotalMilliseconds, Is.LessThan(fileTimeout));

				// Verify:
				VerifyMinimumTimoutOrTestDisabled(terminal, fileLineCount, fileLineByteCount);

				// Refresh and verify again:
				terminal.RefreshRepositories();
				VerifyMinimumTimoutOrTestDisabled(terminal, fileLineCount, fileLineByteCount);

				terminal.Stop();
				Utilities.WaitForDisconnection(terminal);
			} // using (terminal)
		}

		/// <summary></summary>
		protected virtual void VerifyMinimumTimoutOrTestDisabled(Domain.TextTerminal terminal, int fileLineCount, int fileLineByteCount)
		{
			var displayLines = terminal.RepositoryToDisplayLines(RepositoryType.Bidir);   // At least 2 * 300 lines, but rather * 1.5 = 900 lines.
			Assert.That(displayLines.Count, Is.GreaterThan((int)Math.Round(fileLineCount * 2 * 1.25)));
				                                                                                //// Using 2 * 300 * 1.25 = 750 lines, i.e. something inbetween.
			var previousLineTimeStamp = DateTime.MinValue;
			foreach (var dl in displayLines)
			{
				Assert.That(dl.ByteCount, Is.LessThanOrEqualTo(fileLineByteCount));
				Assert.That(dl.CharCount, Is.LessThanOrEqualTo(fileLineByteCount));

				Assert.That(dl.TimeStamp, Is.GreaterThanOrEqualTo(previousLineTimeStamp));
				previousLineTimeStamp = dl.TimeStamp;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
