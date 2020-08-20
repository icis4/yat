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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Settings;

using NUnit.Framework;

using YAT.Domain;
using YAT.Domain.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.Connection
{
	/// <summary></summary>
	public static class DisconnectTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <remarks>
		/// Test cases are generated by code rather than using <see cref="ValuesAttribute"/> for
		/// being able to name the test cases in a human readable way.
		/// </remarks>
		private static IEnumerable<TestCaseData> Tests
		{
			get
			{
				for (char disconnectIdentifier = 'A'; disconnectIdentifier <= 'B'; disconnectIdentifier++)
					yield return (new TestCaseData(disconnectIdentifier).SetName("_Disconnect" + disconnectIdentifier));
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPSocketPairs_Text
		{
			get
			{
				foreach (var tc in Domain.Test.Data.ToIPSocketPairsTestCases(TerminalType.Text, Tests))
					yield return (tc);
			}
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPSocketSelfs_Text
		{
			get
			{
				foreach (var tc in Domain.Test.Data.ToIPSocketSelfsTestCases(TerminalType.Text, Tests))
					yield return (tc);
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DisconnectTest
	{
		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(DisconnectTestData), "TestCasesIPSocketPairs_Text")]
		public virtual void TestIPSocketPairs(TerminalSettings settingsA, TerminalSettings settingsB, char disconnectIdentifier)
		{
			// IPSocketPairs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerifyAndDisconnect(settingsA, settingsB, disconnectIdentifier);
		}

		/// <remarks>Separation into multiple tests for grouping 'by I/O' to ease test development and manual execution.</remarks>
		[Test, TestCaseSource(typeof(DisconnectTestData), "TestCasesIPSocketSelfs_Text")]
		public virtual void TestIPSocketSelfs(TerminalSettings settings, char disconnectIdentifier)
		{
			// IPSocketSelfs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerifyAndDisconnect(settings, null, disconnectIdentifier);
		}

		private static void TransmitAndVerifyAndDisconnect(TerminalSettings settingsA, TerminalSettings settingsB, char disconnectIdentifier)
		{
			using (var terminalA = new Terminal(Settings.Create(settingsA)))
			{
				try
				{
					terminalA.MessageInputRequest += Utilities.TerminalMessageInputRequest;
					if (!terminalA.Launch())
					{
						if (Utilities.TerminalMessageInputRequestResultsInExclude) {
							Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
						//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
						}
						else {
							Assert.Fail(@"Failed to launch """ + terminalA.Caption + @"""");
						}
					}
					Utilities.WaitForStart(terminalA);

					if (settingsB != null) // Loopback pair:
					{
						using (var terminalB = new Terminal(Settings.Create(settingsB)))
						{
							try
							{
								terminalB.MessageInputRequest += Utilities.TerminalMessageInputRequest;
								if (!terminalB.Launch())
								{
									if (Utilities.TerminalMessageInputRequestResultsInExclude) {
										Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
									//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
									}
									else {
										Assert.Fail(@"Failed to launch """ + terminalB.Caption + @"""");
									}
								}
								Utilities.WaitForConnection(terminalA, terminalB);

								TransmitAndVerifyAndDisconnect(terminalA, terminalB, disconnectIdentifier);
							}
							finally // Properly stop even in case of an exception, e.g. a failed assertion.
							{
								terminalB.Stop();
								Utilities.WaitForStop(terminalB);
							}
						} // using (terminalB)
					}
					else // Loopback self:
					{
						TransmitAndVerifyAndDisconnect(terminalA, terminalA, disconnectIdentifier);
					}
				}
				finally // Properly stop even in case of an exception, e.g. a failed assertion.
				{
					terminalA.Stop();
					Utilities.WaitForStop(terminalA);
				}
			} // using (terminalA)
		}

		private static void TransmitAndVerifyAndDisconnect(Terminal terminalA, Terminal terminalB, char disconnectIdentifier)
		{
			Encoding parserEncoding;
			Endianness parserEndianness;
			Domain.Parser.Mode parserMode;
			Utilities.AssertMatchingParserSettingsForSendText(terminalA, terminalB, out parserEncoding, out parserEndianness, out parserMode);
			using (var parser = new Domain.Parser.Parser(parserEncoding, parserEndianness, parserMode))
			{
				byte[] parseResult;

				var textAB = "A => B";
				var textBA = "B => A";

				Assert.That(parser.TryParse(textAB, out parseResult));
				int textByteCountAB = parseResult.Length;
				Assert.That(parser.TryParse(textBA, out parseResult));
				int textByteCountBA = parseResult.Length;

				Assert.That(parser.TryParse(terminalA.SettingsRoot.TextTerminal.TxEol, out parseResult));
				int eolByteCountAB = parseResult.Length;
				Assert.That(parser.TryParse(terminalB.SettingsRoot.TextTerminal.TxEol, out parseResult));
				int eolByteCountBA = parseResult.Length;

				int byteCountAB = (textByteCountAB + eolByteCountAB);
				int byteCountBA = (textByteCountBA + eolByteCountBA);

				int expectedTotalByteCountA = 0;
				int expectedTotalLineCountA = 0;
				int expectedTotalByteCountB = 0;
				int expectedTotalLineCountB = 0;

				// Initial ping-pong:
				terminalA.SendText(textAB);// Initial A => B
				expectedTotalByteCountA += byteCountAB;
				expectedTotalLineCountA++;
				Utilities.WaitForTransmissionAndAssertCounts(terminalA, terminalB, expectedTotalByteCountA, expectedTotalLineCountA);

				terminalB.SendText(textBA); // Response B => A
				expectedTotalByteCountB += byteCountBA;
				expectedTotalLineCountB++;

				if (terminalA == terminalB) // Loopback self:
				{
					expectedTotalByteCountB *= 2;
					expectedTotalLineCountB *= 2;
				}

				Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountB, expectedTotalLineCountB);

				if (disconnectIdentifier == 'A')
				{
					// Send repeating B => A then close A:
					terminalB.SendText(@"B => A\!(LineRepeat)");
					Thread.Sleep(333); // Unlimited, i.e. up to several thousand lines.
					terminalA.Stop();
					Utilities.WaitForStop(terminalA);

					// Stop repeating:
					Thread.Sleep(333); // Make sure that B can deal with disconnected A. Note that WaitForDisconnection() would not work for connection-less UDP sockets.
					terminalB.Stop();
					Utilities.WaitForStop(terminalB);

					// Cleanup and start again:
					terminalB.ClearRepositories();
					terminalA.ClearRepositories();
					terminalB.ResetCountAndRate();
					terminalA.ResetCountAndRate();

					terminalB.Start();
					terminalA.Start();
					Utilities.WaitForConnection(terminalB, terminalA);
				}
				else       // Identifier == 'B')
				{
					// Send repeating A => B then close B:
					terminalA.SendText(@"A => B\!(LineRepeat)");
					Thread.Sleep(333); // Unlimited, i.e. up to several thousand lines.
					terminalB.Stop();
					Utilities.WaitForStop(terminalB);

					// Stop repeating:
					Thread.Sleep(333); // Make sure that A can deal with disconnected B. Note that WaitForDisconnection() would not work for connection-less UDP sockets.
					terminalA.Stop();
					Utilities.WaitForStop(terminalA);

					// Cleanup and start again:
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();
					terminalA.ResetCountAndRate();
					terminalB.ResetCountAndRate();

					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalB, terminalA);
				}

				// Verify cleanup:
				expectedTotalByteCountA = 0;
				expectedTotalLineCountA = 0;
				expectedTotalByteCountB = 0;
				expectedTotalLineCountB = 0;
				Utilities.AssertCounts(terminalA, terminalB, expectedTotalByteCountA, expectedTotalLineCountA);
				Utilities.AssertCounts(terminalB, terminalA, expectedTotalByteCountB, expectedTotalLineCountB);

				// Subsequent ping-pong:
				terminalA.SendText(textAB); // Subsequent A => B
				expectedTotalByteCountA += byteCountAB;
				expectedTotalLineCountA++;
				Utilities.WaitForTransmissionAndAssertCounts(terminalA, terminalB, expectedTotalByteCountA, expectedTotalLineCountA);

				terminalB.SendText(textBA); // Response B => A
				expectedTotalByteCountB += byteCountBA;
				expectedTotalLineCountB++;

				if (terminalA == terminalB) // Loopback self:
				{
					expectedTotalByteCountB *= 2;
					expectedTotalLineCountB *= 2;
				}

				Utilities.WaitForTransmissionAndAssertCounts(terminalB, terminalA, expectedTotalByteCountB, expectedTotalLineCountB);
			} // using (parser)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
