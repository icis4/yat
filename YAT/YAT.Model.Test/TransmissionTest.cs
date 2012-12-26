﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using YAT.Settings.Terminal;

#endregion

namespace YAT.Model.Test
{
	#region TransmissionType
	//==========================================================================================
	// TransmissionType
	//==========================================================================================

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum TransmissionType
	{
		SerialPort,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		TcpAutoSocketOnIPv4Loopback,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		TcpAutoSocketOnIPv6Loopback,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv4 is a common term, and even used by the .NET framework itself.")]
		TcpAutoSocketOnSpecificIPv4Interface,
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pv", Justification = "IPv6 is a common term, and even used by the .NET framework itself.")]
		TcpAutoSocketOnSpecificIPv6Interface,

		// USB Ser/HID not possible since YAT only implements the host side.
	}

	#pragma warning restore 1591

	#endregion

	/// <summary></summary>
	public static class TransmissionTypes
	{
		#region TransmissionTypes
		//==========================================================================================
		// TransmissionTypes
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		public static IEnumerable<KeyValuePair<TransmissionType, string>> GetItems
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

		#endregion
	}

	/// <summary></summary>
	public static class OneWayTransmissionTestData
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
		private static readonly Utilities.TestSet EolOnlyCommand;

		private static readonly Utilities.TestSet SingleNoEolCommand;
		private static readonly Utilities.TestSet DoubleNoEolCommand;

		private static readonly Utilities.TestSet ControlCharCommand1;
		private static readonly Utilities.TestSet ControlCharCommand2;
		private static readonly Utilities.TestSet ControlCharCommand3;

		private static readonly Utilities.TestSet ClearCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static OneWayTransmissionTestData()
		{
			SingleLineCommand   = new Utilities.TestSet(new Types.Command(TestCommandLines[0]));
			DoubleLineCommand   = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1] } ));
			TripleLineCommand   = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1], TestCommandLines[2] }));
			MultiLineCommand    = new Utilities.TestSet(new Types.Command(TestCommandLines));

			MultiEolCommand     = new Utilities.TestSet(new Types.Command(@"A\!(EOL)B<CR><LF>C<CR><LF>D"), 4, new int[] { 2, 2, 2, 2 }, new int[] { 1, 1, 1, 1 }, true); // Eol results in one element since ShowEol is switched off.
			MixedEolCommand     = new Utilities.TestSet(new Types.Command(@"A\!(EOL)BC<CR><LF>D"),         3, new int[] { 2, 2, 2    }, new int[] { 1, 2, 1    }, true); // Eol results in one element since ShowEol is switched off.

			EolPartsCommand     = new Utilities.TestSet(new Types.Command(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F"), 4, new int[] { 3, 2, 3, 6 }, new int[] { 2, 1, 2, 5 }, true);
			EolOnlyCommand      = new Utilities.TestSet(new Types.Command(new string[] { @"A", @"B", @"", @"C" }),            4, new int[] { 2, 2, 1, 2 }, new int[] { 1, 1, 0, 1 }, true);

			SingleNoEolCommand  = new Utilities.TestSet(new Types.Command(@"A\!(NoEOL)"), 1, new int[] { 1 }, new int[] { 1 }, true);                                 // There is always 1 line.
			DoubleNoEolCommand  = new Utilities.TestSet(new Types.Command(new string[] { @"A\!(NoEOL)", @"B\!(NoEOL)" }), 1, new int[] { 1 }, new int[] { 2 }, true); // There is always 1 line.

			ControlCharCommand1 = new Utilities.TestSet(new Types.Command(@"\h(00)<CR><LF>\h(00)A<CR><LF>A\h(00)<CR><LF>A\h(00)A"), 4, new int[] { 2, 3, 3, 4 }, new int[] { 1, 2, 2, 3 }, true);
			ControlCharCommand2 = new Utilities.TestSet(new Types.Command(@"\h(7F)<CR><LF>\h(7F)A<CR><LF>A\h(7F)<CR><LF>A\h(7F)A"), 4, new int[] { 2, 3, 3, 4 }, new int[] { 1, 2, 2, 3 }, true);
			ControlCharCommand3 = new Utilities.TestSet(new Types.Command(@"\h(FF)<CR><LF>\h(FF)A<CR><LF>A\h(FF)<CR><LF>A\h(FF)A"), 4, new int[] { 2, 2, 2, 2 }, new int[] { 1, 2, 2, 3 }, true); // A non-breaking space isn't a control character.

			ClearCommand        = new Utilities.TestSet(new Types.Command(@"A<CR><LF>B<CR><LF>C\!(Clear)\!(NoEOL)"), 3, new int[] { 1, 1, 1 }, new int[] { 0 }, false);
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (KeyValuePair<TransmissionType, string> kvp in TransmissionTypes.GetItems)
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
					yield return (new TestCaseData(kvp.Key, EolOnlyCommand,  1).SetCategory(kvp.Value).SetName(kvp.Key + "_EolOnlyTransmission"));

					yield return (new TestCaseData(kvp.Key, SingleNoEolCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_SingleNoEolTransmission"));
					yield return (new TestCaseData(kvp.Key, DoubleNoEolCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_DoubleNoEolTransmission"));

					yield return (new TestCaseData(kvp.Key, ControlCharCommand1, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_ControlCharCommand1"));
					yield return (new TestCaseData(kvp.Key, ControlCharCommand2, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_ControlCharCommand2"));
					yield return (new TestCaseData(kvp.Key, ControlCharCommand3, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_ControlCharCommand3"));

					yield return (new TestCaseData(kvp.Key, ClearCommand, 1).SetCategory(kvp.Value).SetName(kvp.Key + "_ClearCommand"));
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class OneWayTransmissionTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(OneWayTransmissionTestData), "TestCases")]
		public virtual void PerformTransmission(TransmissionType transmissionType, Utilities.TestSet testSet, int transmissionCount)
		{
			switch (transmissionType)
			{
				case TransmissionType.SerialPort:
					PerformTransmission(Utilities.GetStartedTextSerialPortASettings(), Utilities.GetStartedTextSerialPortBSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnIPv4Loopback:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(), Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnIPv6Loopback:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(), Utilities.GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv4Interface:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings(), Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv6Interface:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings(), Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings(), testSet, transmissionCount);
					break;
			}
		}

		#endregion

		#region Transmission
		//==========================================================================================
		// Transmission
		//==========================================================================================

		private static void PerformTransmission(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			// Create terminals from settings and check whether B receives from A:
			using (Terminal terminalA = new Terminal(settingsA))
			{
				using (Terminal terminalB = new Terminal(settingsB))
				{
					// Start and open terminals:
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					for (int i = 0; i < transmissionCount; i++)
					{
						// Send test command:
						terminalA.SendText(testSet.Command);
						Utilities.WaitForTransmission(terminalA, terminalB, testSet);

						// Verify transmission:
						Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
						                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
						                      testSet, i + 1);
					}
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	public static class TwoWayTransmissionTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly Utilities.TestSet PingPongCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static TwoWayTransmissionTestData()
		{
			PingPongCommand = new Utilities.TestSet(new Types.Command(@"ABC DE F"), 1, new int[] { 2 }, new int[] { 8 }, true);
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (KeyValuePair<TransmissionType, string> kvp in TransmissionTypes.GetItems)
				{
					yield return (new TestCaseData(kvp.Key, PingPongCommand,   1).SetCategory(kvp.Value).SetName(kvp.Key + "_PingPong1"));
					yield return (new TestCaseData(kvp.Key, PingPongCommand,  10).SetCategory(kvp.Value).SetName(kvp.Key + "_PingPong10"));

				//	yield return (new TestCaseData(kvp.Key, PingPongCommand, 100).SetCategory(kvp.Value).SetName(kvp.Key + "_PingPong100"));
				//	Takes several minutes and doesn't reproduce bugs #3284550 and #3480565, therefore disabled.
				}
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class TwoWayTransmissionTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(TwoWayTransmissionTestData), "TestCases")]
		public virtual void PerformTransmission(TransmissionType transmissionType, Utilities.TestSet testSet, int transmissionCount)
		{
			switch (transmissionType)
			{
				case TransmissionType.SerialPort:
					PerformTransmission(Utilities.GetStartedTextSerialPortASettings(), Utilities.GetStartedTextSerialPortBSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnIPv4Loopback:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(), Utilities.GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnIPv6Loopback:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(), Utilities.GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv4Interface:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings(), Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings(), testSet, transmissionCount);
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv6Interface:
					PerformTransmission(Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings(), Utilities.GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings(), testSet, transmissionCount);
					break;
			}
		}

		#endregion

		#region Transmission
		//==========================================================================================
		// Transmission
		//==========================================================================================

		private static void PerformTransmission(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			// Create terminals from settings and check whether B receives from A:
			using (Terminal terminalA = new Terminal(settingsA))
			{
				using (Terminal terminalB = new Terminal(settingsB))
				{
					// Start and open terminals:
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					for (int i = 0; i < transmissionCount; i++)
					{
						// Send 'Ping' test command A > B :
						terminalA.SendText(testSet.Command);
						Utilities.WaitForTransmission(terminalA, terminalB, testSet);

						// Verify transmission:
						Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
						                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
						                      testSet, i + 1);

						// Send 'Pong' test command B > A :
						terminalB.SendText(testSet.Command);
						Utilities.WaitForTransmission(terminalB, terminalA, testSet);

						// Verify transmission:
						Utilities.VerifyLines(terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
						                      terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
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
