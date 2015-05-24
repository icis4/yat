//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY;
using MKY.Collections.Generic;
using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;
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

		private static readonly string[] SerialPortsAreInterconnectedAB = new string[]
		{
			MKY.IO.Ports.Test.SettingsCategoryStrings.SerialPortAIsAvailable,
			MKY.IO.Ports.Test.SettingsCategoryStrings.SerialPortBIsAvailable,
			MKY.IO.Ports.Test.SettingsCategoryStrings.SerialPortsAreInterconnected
		};

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		public static IEnumerable<KeyValuePair<TransmissionType, string[]>> GetItems
		{
			get
			{
				yield return (new KeyValuePair<TransmissionType, string[]>(TransmissionType.SerialPort, SerialPortsAreInterconnectedAB));
				yield return (new KeyValuePair<TransmissionType, string[]>(TransmissionType.TcpAutoSocketOnIPv4Loopback,          new string[] { MKY.Net.Test.SettingsCategoryStrings.IPv4LoopbackIsAvailable }));
				yield return (new KeyValuePair<TransmissionType, string[]>(TransmissionType.TcpAutoSocketOnIPv6Loopback,          new string[] { MKY.Net.Test.SettingsCategoryStrings.IPv6LoopbackIsAvailable }));
				yield return (new KeyValuePair<TransmissionType, string[]>(TransmissionType.TcpAutoSocketOnSpecificIPv4Interface, new string[] { MKY.Net.Test.SettingsCategoryStrings.SpecificIPv4InterfaceIsAvailable }));
				yield return (new KeyValuePair<TransmissionType, string[]>(TransmissionType.TcpAutoSocketOnSpecificIPv6Interface, new string[] { MKY.Net.Test.SettingsCategoryStrings.SpecificIPv6InterfaceIsAvailable }));
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
		private static readonly Utilities.TestSet StillEolCommand1;
		private static readonly Utilities.TestSet StillEolCommand2;
		private static readonly Utilities.TestSet StillEolCommand3;

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

			SingleNoEolCommand  = new Utilities.TestSet(new Types.Command(@"A\!(NoEOL)"),                                 1, new int[] { 1 },    new int[] { 1 },    true); // There is always 1 line.
			DoubleNoEolCommand  = new Utilities.TestSet(new Types.Command(new string[] { @"A\!(NoEOL)", @"B\!(NoEOL)" }), 1, new int[] { 1 },    new int[] { 2 },    true); // There is always 1 line.
			StillEolCommand1    = new Utilities.TestSet(new Types.Command(@"<CR><LF>\!(NoEOL)"),                          1, new int[] { 1 },    new int[] { 0 },    true);
			StillEolCommand2    = new Utilities.TestSet(new Types.Command(@"A<CR><LF>\!(NoEOL)"),                         1, new int[] { 2 },    new int[] { 1 },    true);
			StillEolCommand3    = new Utilities.TestSet(new Types.Command(@"A<CR><LF>B\!(NoEOL)"),                        2, new int[] { 2, 1 }, new int[] { 1, 1 }, true);

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
		private static IEnumerable<KeyValuePair<TestCaseData, string[]>> TestCasesWithoutCategory
		{
			get
			{
				foreach (KeyValuePair<TransmissionType, string[]> kvp in TransmissionTypes.GetItems)
				{
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, SingleLineCommand,   1).SetName(kvp.Key + "_SingleLineTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, DoubleLineCommand,   1).SetName(kvp.Key + "_DoubleLineTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, DoubleLineCommand,   2).SetName(kvp.Key + "_DoubleLineDoubleTransmission"), kvp.Value));

					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, TripleLineCommand,   1).SetName(kvp.Key + "_TripleLineTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, TripleLineCommand,   3).SetName(kvp.Key + "_TripleLineTripleTransmission"), kvp.Value));

					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, MultiLineCommand,    1).SetName(kvp.Key + "_MultiLineTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, MultiLineCommand, TestCommandLines.Length).SetName(kvp.Key + "_MultiLineMultiTransmission"), kvp.Value));

					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, MultiEolCommand,     1).SetName(kvp.Key + "_MultiEolTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, MixedEolCommand,     1).SetName(kvp.Key + "_MixedEolTransmission"), kvp.Value));

					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, EolPartsCommand,     1).SetName(kvp.Key + "_EolPartsTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, EolOnlyCommand,      1).SetName(kvp.Key + "_EolOnlyTransmission"), kvp.Value));

					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, SingleNoEolCommand,  1).SetName(kvp.Key + "_SingleNoEolTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, DoubleNoEolCommand,  1).SetName(kvp.Key + "_DoubleNoEolTransmission"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, StillEolCommand1,    1).SetName(kvp.Key + "_StillEolCommand1"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, StillEolCommand2,    1).SetName(kvp.Key + "_StillEolCommand2"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, StillEolCommand3,    1).SetName(kvp.Key + "_StillEolCommand3"), kvp.Value));

					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, ControlCharCommand1, 1).SetName(kvp.Key + "_ControlCharCommand1"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, ControlCharCommand2, 1).SetName(kvp.Key + "_ControlCharCommand2"), kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, ControlCharCommand3, 1).SetName(kvp.Key + "_ControlCharCommand3"), kvp.Value));

					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, ClearCommand,        1).SetName(kvp.Key + "_ClearCommand"), kvp.Value));
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (KeyValuePair<TestCaseData, string[]> kvp in TestCasesWithoutCategory)
				{
					foreach (string cat in kvp.Value)
						kvp.Key.SetCategory(cat);

					yield return (kvp.Key);
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here because line counts and rates are calculated in <see cref="YAT.Model.Terminal"/>
	/// and required when evaluating the test result.
	/// </remarks>
	[TestFixture]
	public class OneWayTransmissionTest
	{
		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close temporary in-memory application settings.
			ApplicationSettings.Close();
		}

		#endregion

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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to verify.")]
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
		private static IEnumerable<KeyValuePair<TestCaseData, string[]>> TestCasesWithoutCategory
		{
			get
			{
				foreach (KeyValuePair<TransmissionType, string[]> kvp in TransmissionTypes.GetItems)
				{
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, PingPongCommand,   1).SetName(kvp.Key + "_PingPong1"),  kvp.Value));
					yield return (new KeyValuePair<TestCaseData, string[]>(new TestCaseData(kvp.Key, PingPongCommand,  10).SetName(kvp.Key + "_PingPong10"), kvp.Value));

				//	yield return (new TestCaseData(kvp.Key, PingPongCommand, 100).SetCategory(kvp.Value).SetName(kvp.Key + "_PingPong100"));
				//	Takes several minutes and doesn't reproduce bugs #3284550>#194 and #3480565>#221, therefore disabled.
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (KeyValuePair<TestCaseData, string[]> kvp in TestCasesWithoutCategory)
				{
					foreach (string cat in kvp.Value)
						kvp.Key.SetCategory(cat);

					yield return (kvp.Key);
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here because line counts and rates are calculated in <see cref="YAT.Model.Terminal"/>
	/// and required when evaluating the test result.
	/// </remarks>
	[TestFixture]
	public class TwoWayTransmissionTest
	{
		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close temporary in-memory application settings.
			ApplicationSettings.Close();
		}

		#endregion

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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to verify.")]
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

	/// <summary></summary>
	public static class MTSicsDeviceTransmissionTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly List<Pair<Pair<string, string>, TimeSpan>> Commands;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static MTSicsDeviceTransmissionTestData()
		{
			Commands = new List<Pair<Pair<string, string>, TimeSpan>>();
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"S", @"S +"),  TimeSpan.FromSeconds(90.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"SI", @"S +"), TimeSpan.FromSeconds(15.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"I1", @"I1 A ""0123"" ""2.30"" ""2.22"" ""2.33"" ""2.20"""), TimeSpan.FromSeconds(50.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"I6", @"ES"),  TimeSpan.FromSeconds(15.0 / 1000)));
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
				TransmissionType tt = TransmissionType.SerialPort;
				string categoryDev = MKY.IO.Ports.Test.SettingsCategoryStrings.MTSicsDeviceAIsConnected;
				string category01m = new NUnit.MinuteDurationCategoryAttribute( 1).Name;
				string category60m = new NUnit.MinuteDurationCategoryAttribute(60).Name;
				string category24h = new NUnit.HourDurationCategoryAttribute(24).Name;

				foreach (Pair<Pair<string, string>, TimeSpan> c in Commands)
				{
					// Get stimulus and expected:
					string stimulus = c.Value1.Value1;
					string expected = c.Value1.Value2;

					// Calculate number of transmissions based on the expected time available/required:
					int loops01m = (int)((       60.0 * 1000) / c.Value2.TotalMilliseconds);
					int loops60m = (int)((     3600.0 * 1000) / c.Value2.TotalMilliseconds);
					int loops24h = (int)((24 * 3600.0 * 1000) / c.Value2.TotalMilliseconds);

					yield return (new TestCaseData(tt, stimulus, expected,  1).SetCategory(categoryDev).SetName(tt + "_" + stimulus +  "_1"));
					yield return (new TestCaseData(tt, stimulus, expected, 10).SetCategory(categoryDev).SetName(tt + "_" + stimulus + "_10"));
					yield return (new TestCaseData(tt, stimulus, expected, loops01m).SetCategory(category01m).SetName(tt + "_" + stimulus + "_" + loops01m));
					yield return (new TestCaseData(tt, stimulus, expected, loops60m).SetCategory(category60m).SetName(tt + "_" + stimulus + "_" + loops60m));
					yield return (new TestCaseData(tt, stimulus, expected, loops24h).SetCategory(category24h).SetName(tt + "_" + stimulus + "_" + loops24h));
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here because line counts and rates are calculated in <see cref="YAT.Model.Terminal"/>
	/// and required when evaluating the test result.
	/// </remarks>
	[TestFixture]
	public class MTSicsDeviceTransmissionTest
	{
		#region Set Up Fixture
		//==========================================================================================
		// Set Up Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close temporary in-memory application settings.
			ApplicationSettings.Close();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(MTSicsDeviceTransmissionTestData), "TestCases")]
		public virtual void PerformTransmission(TransmissionType transmissionType, string stimulus, string expected, int transmissionCount)
		{
			switch (transmissionType)
			{
				case TransmissionType.SerialPort:
					PerformTransmission(Utilities.GetStartedTextMTSicsDeviceASettings(), stimulus, expected, transmissionCount);
					break;
			}
		}

		#endregion

		#region Transmission
		//==========================================================================================
		// Transmission
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to verify.")]
		private static void PerformTransmission(TerminalSettingsRoot settings, string stimulus, string expected, int transmissionCount)
		{
			// Ensure that EOL is displayed, otherwise the EOL bytes are not available for verification.
			settings.TextTerminal.ShowEol = true;

			// Create terminals from settings and check whether B receives from A:
			using (Terminal terminal = new Terminal(settings))
			{
				terminal.MessageInputRequest += new EventHandler<MessageInputEventArgs>(PerformTransmission_terminal_MessageInputRequest);

				// Prepare stimulus and expected:
				Types.Command stimulusCommand = new Types.Command(stimulus);
				
				List<byte> l = new List<byte>();
				foreach (char c in expected.ToCharArray())
					l.Add((byte)c); // ASCII only!

				l.Add(0x0D); // <CR>
				l.Add(0x0A); // <LF>

				byte[] expectedBytes = l.ToArray();

				// Required if COM1 is not available.
				terminal.Start();
				Utilities.WaitForConnection(terminal);

				for (int i = 0; i < transmissionCount; i++)
				{
					// Send stimulus to device:
					Trace.WriteLine(@">> """ + stimulus + @""" (" + i + ")");
					terminal.SendText(stimulusCommand);

					// Wait for response from device:
					Domain.DisplayLine lastLine;
					byte[] actualBytes;
					const int WaitInterval = 5;
					const int WaitTimeout = 3000;
					int timeout = 0;
					do                         // Initially wait to allow async send,
					{                          //   therefore, use do-while.
						Thread.Sleep(WaitInterval);
						timeout += WaitInterval;

						if (timeout >= WaitTimeout)
							Assert.Fail("Transmission timeout! Try to re-run test case.");

						lastLine = terminal.LastDisplayLineAuxiliary(Domain.RepositoryType.Rx);
						actualBytes = lastLine.ElementsToOrigin();
					}
					while (actualBytes.Length < expectedBytes.Length);

					// Verify response:
					Assert.True(ArrayEx.ValuesEqual(expectedBytes, actualBytes), "Unexpected respose from device! Should be " + ArrayEx.ElementsToString(expectedBytes) + " but is " + ArrayEx.ElementsToString(actualBytes));
					Trace.WriteLine(@"<< """ + expected + @"""");
					terminal.ClearLastDisplayLineAuxiliary(Domain.RepositoryType.Rx);
				}
			}
		}

		private static void PerformTransmission_terminal_MessageInputRequest(object sender, MessageInputEventArgs e)
		{
			Assert.Fail
			(
				"Unexpected message input request:" + Environment.NewLine + Environment.NewLine +
				e.Caption + Environment.NewLine + Environment.NewLine +
				e.Text
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
