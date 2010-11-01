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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using YAT.Settings.Terminal;

namespace YAT.Model.Test
{
	/// <summary></summary>
	public enum TransmissionType
	{
		/// <summary></summary>
		SerialPort,
		/// <summary></summary>
		TcpAutoSocketOnIPv4Loopback,
		/// <summary></summary>
		TcpAutoSocketOnIPv6Loopback,
		/// <summary></summary>
		TcpAutoSocketOnSpecificIPv4Interface,
		/// <summary></summary>
		TcpAutoSocketOnSpecificIPv6Interface,

		// USB Ser/HID not possible since YAT only implements the host side.
	}

	/// <summary></summary>
	public static class TransmissionTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly string[] TestCommandLines = new string[]
			{
				@"1a2a3a4a5a6a7a8a9a0",
				@"1b2b3b4b5b6b7b8b9b0",
				@"1c2c3c4c5c6c7c8c9c0",
				@"1d2d3d4d5d6d7d8d9d0",
				@"1e2e3e4e5e6e7e8e9e0",
				@"1f2f3f4f5f6f7f8f9f0",
				@"1g2g3g4g5g6g7g8g9g0",
				@"1h2h3h4h5h6h7h8h9h0",
				@"1i2i3i4i5i6i7i8i9i0",
				@"1j2j3j4j5j6j7j8j9j0",
			};

		private static readonly Utilities.TestSet SingleLineCommand;
		private static readonly Utilities.TestSet DoubleLineCommand;
		private static readonly Utilities.TestSet TripleLineCommand;
		private static readonly Utilities.TestSet MultiLineCommand;

		private static readonly Utilities.TestSet MultiEolCommand;
		private static readonly Utilities.TestSet MixedEolCommand;

		private static readonly Utilities.TestSet EolPartsCommand;

		private static readonly Utilities.TestSet SingleNoEolCommand;
		private static readonly Utilities.TestSet DoubleNoEolCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		static TransmissionTestData()
		{
			SingleLineCommand  = new Utilities.TestSet(new Types.Command(TestCommandLines[0]));
			DoubleLineCommand  = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1] } ));
			TripleLineCommand  = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1], TestCommandLines[2] }));
			MultiLineCommand   = new Utilities.TestSet(new Types.Command(TestCommandLines));

			MultiEolCommand    = new Utilities.TestSet(new Types.Command(@"A\!(Eol)B<CR><LF>C<CR><LF>D"), 4, new int[] { 2, 2, 2, 2 }, new int[] { 1, 1, 1, 1 }); // Eol results in one element since ShowEol is switched off.
			MixedEolCommand    = new Utilities.TestSet(new Types.Command(@"A\!(Eol)BC<CR><LF>D"),         3, new int[] { 2, 2, 2    }, new int[] { 1, 2, 1    }); // Eol results in one element since ShowEol is switched off.

			EolPartsCommand    = new Utilities.TestSet(new Types.Command(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F"), 4, new int[] { 3, 2, 3, 6 }, new int[] { 2, 1, 2, 5 });

			SingleNoEolCommand = new Utilities.TestSet(new Types.Command(@"A\!(NoEol)"), 1, new int[] { 1 }, new int[] { 1 });                                 // There is always 1 line.
			DoubleNoEolCommand = new Utilities.TestSet(new Types.Command(new string[] { @"A\!(NoEol)", @"B\!(NoEol)" }), 1, new int[] { 1 }, new int[] { 2 }); // There is always 1 line.
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<KeyValuePair<TransmissionType, string>> TransmissionTypes
		{
			get
			{
				yield return (new KeyValuePair<TransmissionType, string>(TransmissionType.SerialPort,                           MKY.IO.Ports.Test.SettingsCategoryStrings.SerialPortsAreInterconnected));
				yield return (new KeyValuePair<TransmissionType, string>(TransmissionType.TcpAutoSocketOnIPv4Loopback,          MKY.Net.Test.SettingsCategoryStrings.IPv4LoopbackIsAvailable));
				yield return (new KeyValuePair<TransmissionType, string>(TransmissionType.TcpAutoSocketOnIPv6Loopback,          MKY.Net.Test.SettingsCategoryStrings.IPv6LoopbackIsAvailable));
				yield return (new KeyValuePair<TransmissionType, string>(TransmissionType.TcpAutoSocketOnSpecificIPv4Interface, MKY.Net.Test.SettingsCategoryStrings.SpecificIPv4InterfaceIsAvailable));
				yield return (new KeyValuePair<TransmissionType, string>(TransmissionType.TcpAutoSocketOnSpecificIPv6Interface, MKY.Net.Test.SettingsCategoryStrings.SpecificIPv6InterfaceIsAvailable));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (KeyValuePair<TransmissionType, string> kvp in TransmissionTypes)
				{
					yield return (new TestCaseData(kvp.Key, SingleLineCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_SingleLineTransmission"));

					yield return (new TestCaseData(kvp.Key, DoubleLineCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_DoubleLineTransmission"));
					yield return (new TestCaseData(kvp.Key, DoubleLineCommand, 2).SetCategory(kvp.Value).SetName(kvp.Key + "_DoubleLineDoubleTransmission"));

					yield return (new TestCaseData(kvp.Key, TripleLineCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_TripleLineTransmission"));
					yield return (new TestCaseData(kvp.Key, TripleLineCommand, 3).SetCategory(kvp.Value).SetName(kvp.Key + "_TripleLineTripleTransmission"));

					yield return (new TestCaseData(kvp.Key, MultiLineCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_MultiLineTransmission"));
					yield return (new TestCaseData(kvp.Key, MultiLineCommand, TestCommandLines.Length).SetCategory(kvp.Value).SetName(kvp.Key + "_MultiLineMultiTransmission"));

					yield return (new TestCaseData(kvp.Key, MultiEolCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_MultiEolTransmission"));
					yield return (new TestCaseData(kvp.Key, MixedEolCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_MixedEolTransmission"));
					yield return (new TestCaseData(kvp.Key, EolPartsCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_EolPartsTransmission"));

					yield return (new TestCaseData(kvp.Key, SingleNoEolCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_SingleNoEolTransmission"));
					yield return (new TestCaseData(kvp.Key, DoubleNoEolCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_DoubleNoEolTransmission"));
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class TransmissionTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(TransmissionTestData), "TestCases")]
		public virtual void PerformCommandTransmission(TransmissionType transmissionType, Utilities.TestSet testSet, int transmissionCount)
		{
			switch (transmissionType)
			{
				case TransmissionType.SerialPort:
					PerformCommandTransmission(Utilities.GetStartedTextSerialPortASettings(), Utilities.GetStartedTextSerialPortBSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnIPv4Loopback:
					PerformCommandTransmission(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(), Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnIPv6Loopback:
					PerformCommandTransmission(Utilities.GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(), Utilities.GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv4Interface:
					PerformCommandTransmission(Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings(), Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv6Interface:
					PerformCommandTransmission(Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings(), Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings(), testSet, transmissionCount);
					break;
			}
		}

		#endregion

		#region Transmission
		//==========================================================================================
		// Transmission
		//==========================================================================================

		private void PerformCommandTransmission(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(settingsA))
			{
				using (Terminal terminalB = new Terminal(settingsB))
				{
					// Start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					for (int i = 0; i < transmissionCount; i++)
					{
						// Send test command
						terminalA.SendText(testSet.Command);
						Utilities.WaitForTransmission(terminalA, terminalB, testSet);

						// Verify transmission
						Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
						                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
						                      testSet, i + 1);
					}
				}
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
