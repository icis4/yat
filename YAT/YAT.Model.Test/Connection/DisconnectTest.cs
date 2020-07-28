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
using System.Threading;

using MKY.Settings;
using MKY.Text;

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

		private static IEnumerable<TestCaseData> Tests
		{
			get
			{
				for (char disconnectIdentifier = 'A'; disconnectIdentifier <= 'B'; disconnectIdentifier++)
					yield return (new TestCaseData(disconnectIdentifier).SetName("_Disconnect" + disconnectIdentifier));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPLoopbackPairs_Text
		{
			get
			{
				foreach (var descriptor in Domain.Test.Environment.IPLoopbackPairs) // Upper level grouping shall be 'by I/O'.
				{
					var settingsA = Domain.Test.Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketTypeA, descriptor.LocalInterface);
					var settingsB = Domain.Test.Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketTypeB, descriptor.LocalInterface);

					foreach (var t in Tests)
						yield return (Domain.Test.Data.ToTestCase(descriptor, t, settingsA, settingsB, t.Arguments));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesIPLoopbackSelfs_Text
		{
			get
			{
				foreach (var descriptor in Domain.Test.Environment.IPLoopbackSelfs) // Upper level grouping shall be 'by I/O'.
				{
					var settings = Domain.Test.Settings.GetIPLoopbackSettings(TerminalType.Text, descriptor.SocketType, descriptor.LocalInterface);

					foreach (var t in Tests)
						yield return (Domain.Test.Data.ToTestCase(descriptor, t, settings, t.Arguments));
				}
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

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(DisconnectTestData), "TestCasesIPLoopbackPairs_Text")]
		public virtual void IPLoopbackPairs(TerminalSettings settingsA, TerminalSettings settingsB, char disconnectIdentifier)
		{
			// IPLoopbackPairs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerifyAndDisconnect(settingsA, settingsB, disconnectIdentifier);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[Test, TestCaseSource(typeof(DisconnectTestData), "TestCasesIPLoopbackSelfs_Text")]
		public static void IPLoopbackSelfs(TerminalSettings settings, char disconnectIdentifier)
		{
			// IPLoopbackSelfs are always made available by 'Utilities', no need to check for this.

			TransmitAndVerifyAndDisconnect(settings, null, disconnectIdentifier);
		}

		private static void TransmitAndVerifyAndDisconnect(TerminalSettings settingsA, TerminalSettings settingsB, char disconnectIdentifier)
		{
			using (var terminalA = new Terminal(Settings.Create(settingsA)))
			{
				terminalA.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminalA.Start())
				{
					if (Utilities.TerminalMessageInputRequestResultsInExclude) {
						Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
					//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
					}
					else {
						Assert.Fail(@"Failed to start """ + terminalA.Caption + @"""");
					}
				}
				Utilities.WaitForStart(terminalA);

				if (settingsB != null) // Loopback pair:
				{
					using (var terminalB = new Terminal(Settings.Create(settingsB)))
					{
						terminalB.MessageInputRequest += Utilities.TerminalMessageInputRequest;
						if (!terminalB.Start())
						{
							if (Utilities.TerminalMessageInputRequestResultsInExclude) {
								Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
							//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
							}
							else {
								Assert.Fail(@"Failed to start """ + terminalB.Caption + @"""");
							}
						}
						Utilities.WaitForConnection(terminalA, terminalB);

						TransmitAndVerifyAndDisconnect(terminalA, terminalB, disconnectIdentifier, false);
					}
				}
				else // Loopback self:
				{
					TransmitAndVerifyAndDisconnect(terminalA, terminalA, disconnectIdentifier, true);
				}
			}
		}

		private static void TransmitAndVerifyAndDisconnect(Terminal terminalA, Terminal terminalB, char disconnectIdentifier, bool isSameTerminal)
		{
			var encoding = ((EncodingEx)terminalA.SettingsRoot.TextTerminal.Encoding).Encoding;

			using (var parser = new Domain.Parser.Parser(encoding, Domain.Parser.Mode.AllEscapes))
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

				terminalA.SendText(textAB);// Initial A => B
				expectedTotalByteCountA += byteCountAB;
				expectedTotalLineCountA++;
				Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, expectedTotalByteCountA, expectedTotalLineCountA);

				terminalB.SendText(textBA); // Response B => A
				expectedTotalByteCountB += byteCountBA;
				expectedTotalLineCountB++;
				if (isSameTerminal) {
					expectedTotalByteCountB *= 2;
					expectedTotalLineCountB *= 2;
				}
				Utilities.WaitForTransmissionAndVerifyCounts(terminalB, terminalA, expectedTotalByteCountB, expectedTotalLineCountB);

				if (disconnectIdentifier == 'A')
				{
					// Send repeating B => A then close A:
					terminalB.SendText(@"B => A\!(LineRepeat)");
					Thread.Sleep(333); // i.e. several hundred lines.
					terminalA.StopIO();
					Utilities.WaitForDisconnection(terminalA);
					Thread.Sleep(100); // Make sure that B has finished processing too.
					                 //// Note that WaitForDisconnection() would not work for connection-less UDP sockets.
					terminalA.ClearRepositories();
					terminalB.ClearRepositories();
					terminalA.ResetIOCountAndRate();
					terminalB.ResetIOCountAndRate();

					terminalA.StartIO();
					Utilities.WaitForConnection(terminalA, terminalB);
				}
				else
				{
					// Send repeating A => B then close B:
					terminalA.SendText(@"A => B\!(LineRepeat)");
					Thread.Sleep(333); // i.e. several hundred lines.
					terminalB.StopIO();
					Utilities.WaitForDisconnection(terminalB);
					Thread.Sleep(100); // Make sure that A has finished processing too.
					                 //// Note that WaitForDisconnection() would not work for connection-less UDP sockets.
					terminalB.ClearRepositories();
					terminalA.ClearRepositories();
					terminalB.ResetIOCountAndRate();
					terminalA.ResetIOCountAndRate();

					terminalB.StartIO();
					Utilities.WaitForConnection(terminalB, terminalA);
				}

				expectedTotalByteCountA = 0;
				expectedTotalLineCountA = 0;
				expectedTotalByteCountB = 0;
				expectedTotalLineCountB = 0;

				terminalA.SendText(textAB); // Subsequent A => B
				expectedTotalByteCountA += byteCountAB;
				expectedTotalLineCountA++;
				Utilities.WaitForTransmissionAndVerifyCounts(terminalA, terminalB, expectedTotalByteCountA, expectedTotalLineCountA);

				terminalB.SendText(textBA); // Response B => A
				expectedTotalByteCountB += byteCountBA;
				expectedTotalLineCountB++;
				if (isSameTerminal) {
					expectedTotalByteCountB *= 2;
					expectedTotalLineCountB *= 2;
				}
				Utilities.WaitForTransmissionAndVerifyCounts(terminalB, terminalA, expectedTotalByteCountB, expectedTotalLineCountB);
			} // using (parser)
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
