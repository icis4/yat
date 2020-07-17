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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.Collections.Generic;
using MKY.Settings;

using NUnit.Framework;

using YAT.Domain.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.Transmission
{
	/// <summary></summary>
	public static class OneWayTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly string[] TestTextLines = new string[]
		{
			"1a2a3a4a5a6a7a8a9a0a1a2a3a4a5a6a7a8a9a0a1a2a3a4a5a6a7a8a9a0a1a2a3a4a5a6a7a8a9a0a",
			"1b2b3b4b5b6b7b8b9b0b1b2b3b4b5b6b7b8b9b0b1b2b3b4b5b6b7b8b9b0b1b2b3b4b5b6b7b8b9b0b",
			"1c2c3c4c5c6c7c8c9c0c1c2c3c4c5c6c7c8c9c0c1c2c3c4c5c6c7c8c9c0c1c2c3c4c5c6c7c8c9c0c",
			"1d2d3d4d5d6d7d8d9d0d1d2d3d4d5d6d7d8d9d0d1d2d3d4d5d6d7d8d9d0d1d2d3d4d5d6d7d8d9d0d",
			"1e2e3e4e5e6e7e8e9e0e1e2e3e4e5e6e7e8e9e0e1e2e3e4e5e6e7e8e9e0e1e2e3e4e5e6e7e8e9e0e",
			"1f2f3f4f5f6f7f8f9f0f1f2f3f4f5f6f7f8f9f0f1f2f3f4f5f6f7f8f9f0f1f2f3f4f5f6f7f8f9f0f",
			"1g2g3g4g5g6g7g8g9g0g1g2g3g4g5g6g7g8g9g0g1g2g3g4g5g6g7g8g9g0g1g2g3g4g5g6g7g8g9g0g",
			"1h2h3h4h5h6h7h8h9h0h1h2h3h4h5h6h7h8h9h0h1h2h3h4h5h6h7h8h9h0h1h2h3h4h5h6h7h8h9h0h",
			"1i2i3i4i5i6i7i8i9i0i1i2i3i4i5i6i7i8i9i0i1i2i3i4i5i6i7i8i9i0i1i2i3i4i5i6i7i8i9i0i",
			"1j2j3j4j5j6j7j8j9j0j1j2j3j4j5j6j7j8j9j0j1j2j3j4j5j6j7j8j9j0j1j2j3j4j5j6j7j8j9j0j",
			"1k2k3k4k5k6k7k8k9k0k1k2k3k4k5k6k7k8k9k0k1k2k3k4k5k6k7k8k9k0k1k2k3k4k5k6k7k8k9k0k",
			"1l2l3l4l5l6l7l8l9l0l1l2l3l4l5l6l7l8l9l0l1l2l3l4l5l6l7l8l9l0l1l2l3l4l5l6l7l8l9l0l",
			"1m2m3m4m5m6m7m8m9m0m1m2m3m4m5m6m7m8m9m0m1m2m3m4m5m6m7m8m9m0m1m2m3m4m5m6m7m8m9m0m",
			"1n2n3n4n5n6n7n8n9n0n1n2n3n4n5n6n7n8n9n0n1n2n3n4n5n6n7n8n9n0n1n2n3n4n5n6n7n8n9n0n",
			"1o2o3o4o5o6o7o8o9o0o1o2o3o4o5o6o7o8o9o0o1o2o3o4o5o6o7o8o9o0o1o2o3o4o5o6o7o8o9o0o",
			"1p2p3p4p5p6p7p8p9p0p1p2p3p4p5p6p7p8p9p0p1p2p3p4p5p6p7p8p9p0p1p2p3p4p5p6p7p8p9p0p",
			"1q2q3q4q5q6q7q8q9q0q1q2q3q4q5q6q7q8q9q0q1q2q3q4q5q6q7q8q9q0q1q2q3q4q5q6q7q8q9q0q",
			"1r2r3r4r5r6r7r8r9r0r1r2r3r4r5r6r7r8r9r0r1r2r3r4r5r6r7r8r9r0r1r2r3r4r5r6r7r8r9r0r",
			"1s2s3s4s5s6s7s8s9s0s1s2s3s4s5s6s7s8s9s0s1s2s3s4s5s6s7s8s9s0s1s2s3s4s5s6s7s8s9s0s",
			"1t2t3t4t5t6t7t8t9t0t1t2t3t4t5t6t7t8t9t0t1t2t3t4t5t6t7t8t9t0t1t2t3t4t5t6t7t8t9t0t",
			"1u2u3u4u5u6u7u8u9u0u1u2u3u4u5u6u7u8u9u0u1u2u3u4u5u6u7u8u9u0u1u2u3u4u5u6u7u8u9u0u",
			"1v2v3v4v5v6v7v8v9v0v1v2v3v4v5v6v7v8v9v0v1v2v3v4v5v6v7v8v9v0v1v2v3v4v5v6v7v8v9v0v",
			"1w2w3w4w5w6w7w8w9w0w1w2w3w4w5w6w7w8w9w0w1w2w3w4w5w6w7w8w9w0w1w2w3w4w5w6w7w8w9w0w",
			"1x2x3x4x5x6x7x8x9x0x1x2x3x4x5x6x7x8x9x0x1x2x3x4x5x6x7x8x9x0x1x2x3x4x5x6x7x8x9x0x",
			"1y2y3y4y5y6y7y8y9y0y1y2y3y4y5y6y7y8y9y0y1y2y3y4y5y6y7y8y9y0y1y2y3y4y5y6y7y8y9y0y",
			"1z2z3z4z5z6z7z8z9z0z1z2z3z4z5z6z7z8z9z0z1z2z3z4z5z6z7z8z9z0z1z2z3z4z5z6z7z8z9z0z"
		};

		private static readonly Utilities.TestSet SingleLine;
		private static readonly Utilities.TestSet DoubleLine;
		private static readonly Utilities.TestSet TripleLine;
		private static readonly Utilities.TestSet MultiLine;

		private static readonly Utilities.TestSet MultiEol;
		private static readonly Utilities.TestSet MixedEol;

		private static readonly Utilities.TestSet EolParts;
		private static readonly Utilities.TestSet EolOnly;

		private static readonly Utilities.TestSet SingleNoEol;
		private static readonly Utilities.TestSet DoubleNoEol;
		private static readonly Utilities.TestSet StillEol1;
		private static readonly Utilities.TestSet StillEol2;
		private static readonly Utilities.TestSet StillEol3;

		private static readonly Utilities.TestSet ControlChar1;
		private static readonly Utilities.TestSet ControlChar2;
		private static readonly Utilities.TestSet ControlChar3;

		private static readonly Utilities.TestSet Clear1;
		private static readonly Utilities.TestSet Clear2;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic, and anyway, performance isn't an issue here.")]
		static OneWayTestData()
		{
			SingleLine   = new Utilities.TestSet(new Types.Command(TestTextLines[0]));
			DoubleLine   = new Utilities.TestSet(new Types.Command(new string[] { TestTextLines[0], TestTextLines[1] } ));
			TripleLine   = new Utilities.TestSet(new Types.Command(new string[] { TestTextLines[0], TestTextLines[1], TestTextLines[2] }));
			MultiLine    = new Utilities.TestSet(new Types.Command(TestTextLines));
			                                                                                                       //// LineStart + EOL + LineBreak result in three more elements.
			MultiEol     = new Utilities.TestSet(new Types.Command(@"A\!(EOL)B<CR><LF>C<CR><LF>D"), 4, new int[] { 4, 4, 4, 4 }, new int[] { 3, 3, 3, 3 }, true);
			MixedEol     = new Utilities.TestSet(new Types.Command(@"A\!(EOL)BC<CR><LF>D"),         3, new int[] { 4, 4, 4    }, new int[] { 3, 4, 3    }, true);

			EolParts     = new Utilities.TestSet(new Types.Command(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F"), 4, new int[] { 4, 4, 5, 8 }, new int[] { 4, 3, 4, 7 }, true);
			EolOnly      = new Utilities.TestSet(new Types.Command(new string[] { "A", "B", "", "C" }),                4, new int[] { 4, 4, 3, 4 }, new int[] { 3, 3, 2, 3 }, true);

			SingleNoEol  = new Utilities.TestSet(new Types.Command(@"A\!(NoEOL)"),                                 0, new int[] { 2 },    new int[] { 1 },    true); // 1st line will not get completed.
			DoubleNoEol  = new Utilities.TestSet(new Types.Command(new string[] { @"A\!(NoEOL)", @"B\!(NoEOL)" }), 0, new int[] { 2 },    new int[] { 2 },    true); // 1st line will not get completed.
			StillEol1    = new Utilities.TestSet(new Types.Command(@"<CR><LF>\!(NoEOL)"),                          1, new int[] { 3 },    new int[] { 2 },    true); // 2nd line will be empty.
			StillEol2    = new Utilities.TestSet(new Types.Command(@"A<CR><LF>\!(NoEOL)"),                         1, new int[] { 4 },    new int[] { 3 },    true); // 2nd line will be empty.
			StillEol3    = new Utilities.TestSet(new Types.Command(@"A<CR><LF>B\!(NoEOL)"),                        1, new int[] { 4, 2 }, new int[] { 3, 1 }, true); // 2nd line will not get completed.

			ControlChar1 = new Utilities.TestSet(new Types.Command(@"\h(00)<CR><LF>\h(00)A<CR><LF>A\h(00)<CR><LF>A\h(00)A"), 4, new int[] { 3, 5, 4, 6 }, new int[] { 3, 4, 4, 5 }, true);
			ControlChar2 = new Utilities.TestSet(new Types.Command(@"\h(7F)<CR><LF>\h(7F)A<CR><LF>A\h(7F)<CR><LF>A\h(7F)A"), 4, new int[] { 3, 5, 4, 6 }, new int[] { 3, 4, 4, 5 }, true);
			ControlChar3 = new Utilities.TestSet(new Types.Command(@"\h(FF)<CR><LF>\h(FF)A<CR><LF>A\h(FF)<CR><LF>A\h(FF)A"), 4, new int[] { 4, 4, 4, 4 }, new int[] { 3, 4, 4, 5 }, true); // A non-breaking space isn't a control character.

			Clear1       = new Utilities.TestSet(new Types.Command(@"A<CR><LF>B<CR><LF>C\!(Clear)"),          3, new int[] { 4, 4, 4 }, new int[] { 3, 3, 3 }, false, true);
			Clear2       = new Utilities.TestSet(new Types.Command(@"A<CR><LF>B<CR><LF>C\!(Clear)\!(NoEOL)"), 2, new int[] { 4, 4, 3 }, new int[] { 3, 3, 1 }, false, true);
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		private static IEnumerable<TestCaseData> TestCasesCommandData
		{
			get
			{
				yield return (new TestCaseData(SingleLine,   1).SetName("_SingleLine"));
				yield return (new TestCaseData(DoubleLine,   1).SetName("_DoubleLine"));
				yield return (new TestCaseData(DoubleLine,   2).SetName("_DoubleLineDouble"));

				yield return (new TestCaseData(TripleLine,   1).SetName("_TripleLine"));
				yield return (new TestCaseData(TripleLine,   3).SetName("_TripleLineTriple"));

				yield return (new TestCaseData(MultiLine,    1).SetName("_MultiLine"));
				yield return (new TestCaseData(MultiLine,    5).SetName("_MultiLineMulti"));

				yield return (new TestCaseData(MultiEol,     1).SetName("_MultiEol"));
				yield return (new TestCaseData(MixedEol,     1).SetName("_MixedEol"));

				yield return (new TestCaseData(EolParts,     1).SetName("_EolParts"));
				yield return (new TestCaseData(EolOnly,      1).SetName("_EolOnly"));

				yield return (new TestCaseData(SingleNoEol,  1).SetName("_SingleNoEol"));
				yield return (new TestCaseData(DoubleNoEol,  1).SetName("_DoubleNoEol"));
				yield return (new TestCaseData(StillEol1,    1).SetName("_StillEol1"));
				yield return (new TestCaseData(StillEol2,    1).SetName("_StillEol2"));
				yield return (new TestCaseData(StillEol3,    1).SetName("_StillEol3"));

				yield return (new TestCaseData(ControlChar1, 1).SetName("_ControlChar1"));
				yield return (new TestCaseData(ControlChar2, 1).SetName("_ControlChar2"));
				yield return (new TestCaseData(ControlChar3, 1).SetName("_ControlChar3"));

				yield return (new TestCaseData(Clear1,       1).SetName("_Clear1"));
				yield return (new TestCaseData(Clear2,       1).SetName("_Clear2"));
			}
		}

		/// <param name="loopbackSettings">
		/// Quadruple of...
		/// ...Pair(terminalSettingsDelegateA, terminalSettingsArgumentA)...
		/// ...Pair(terminalSettingsDelegateB, terminalSettingsArgumentB)...
		/// ...string testCaseName...
		/// ...string[] testCaseCategories.
		/// </param>
		private static IEnumerable<TestCaseData> TestCases(Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> loopbackSettings)
		{
			foreach (var commandData in TestCasesCommandData) // TestCaseData(Utilities.TestSet command, int transmissionCount).
			{
				// Arguments:
				var args = new List<object>(commandData.Arguments);
				args.Insert(0, loopbackSettings.Value1); // Insert the settings descriptor A at the beginning.
				args.Insert(1, loopbackSettings.Value2); // Insert the settings descriptor B at second.
				var tcd = new TestCaseData(args.ToArray()); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB, Utilities.TestSet command, int transmissionCount).

				// Name:
				tcd.SetName(loopbackSettings.Value3 + commandData.TestName);

				// Category(ies):
				foreach (string cat in loopbackSettings.Value4)
					tcd.SetCategory(cat);

				yield return (tcd);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesSerialPortLoopbackPairs
		{
			get
			{
				foreach (var lp in Utilities.TransmissionSettings.SerialPortLoopbackPairs)
				{
					foreach (var tc in TestCases(lp))
						yield return (tc);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static IEnumerable TestCasesSerialPortLoopbackSelfs
		{
			get
			{
				foreach (var ls in Utilities.TransmissionSettings.SerialPortLoopbackSelfs)
				{
					foreach (var tc in TestCases(ls))
						yield return (tc);
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesIPLoopbackPairs
		{
			get
			{
				foreach (var lp in Utilities.TransmissionSettings.IPLoopbackPairs)
				{
					foreach (var tc in TestCases(lp))
						yield return (tc);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static IEnumerable TestCasesIPLoopbackSelfs
		{
			get
			{
				foreach (var ls in Utilities.TransmissionSettings.IPLoopbackSelfs)
				{
					foreach (var tc in TestCases(ls))
						yield return (tc);
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here because line counts and rates are calculated in <see cref="Terminal"/>
	/// and required when evaluating the test result.
	/// </remarks>
	[TestFixture]
	public class OneWayTest
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(OneWayTestData), "TestCasesSerialPortLoopbackPairs")]
		public virtual void SerialPortLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(OneWayTestData), "TestCasesSerialPortLoopbackSelfs")]
		public virtual void SerialPortLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                            Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                            Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(OneWayTestData), "TestCasesIPLoopbackPairs")]
		public virtual void IPLoopbackPairs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                    Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                    Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		/// <remarks>Separation into multiple tests for easier handling and execution.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		[Test, TestCaseSource(typeof(OneWayTestData), "TestCasesIPLoopbackSelfs")]
		public static void IPLoopbackSelfs(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                   Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                   Utilities.TestSet testSet, int transmissionCount)
		{
			TransmitAndVerify(settingsDescriptorA, settingsDescriptorB, testSet, transmissionCount);
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		private static void TransmitAndVerify(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorA,
		                                      Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptorB,
		                                      Utilities.TestSet testSet, int transmissionCount)
		{
			var settingsA = settingsDescriptorA.Value1(settingsDescriptorA.Value2);

			if (settingsA.IO.IOTypeIsUdpSocket) // Revert to default EOL which is mandatory for this test case.
			{
				settingsA.TextTerminal.TxEol = TextTerminalSettings.EolDefault;
				settingsA.TextTerminal.RxEol = TextTerminalSettings.EolDefault;
			}

			using (var terminalA = new Terminal(settingsA))
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

				if (settingsDescriptorB.Value1 != null) // Loopback pair:
				{
					var settingsB = settingsDescriptorB.Value1(settingsDescriptorB.Value2);

					if (settingsB.IO.IOTypeIsUdpSocket) // Revert to default EOL which is mandatory for this test case.
					{
						settingsB.TextTerminal.TxEol = TextTerminalSettings.EolDefault;
						settingsB.TextTerminal.RxEol = TextTerminalSettings.EolDefault;
					}

					using (var terminalB = new Terminal(settingsB))
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

						TransmitAndVerify(terminalA, terminalB, testSet, transmissionCount);
					}
				}
				else // Loopback self:
				{
					TransmitAndVerify(terminalA, terminalA, testSet, transmissionCount);
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
		private static void TransmitAndVerify(Terminal terminalA, Terminal terminalB, Utilities.TestSet testSet, int transmissionCount)
		{
			for (int cycle = 1; cycle <= transmissionCount; cycle++)
			{
				// Send test command:
				terminalA.SendText(testSet.Command);
				if (testSet.ExpectedAlsoApplyToA) {
					Utilities.WaitForTransmissionCycleAndVerifyCounts(terminalA, terminalB, testSet, cycle);
				}
				else if (testSet.ClearedIsExpectedInTheEnd && (terminalA == terminalB)) { // Clear* on loopback self:
				////Utilities.WaitForReceivingCycleAndVerifyCounts(terminalB, testSet, cycle) doesn't work because clear will also be applied to Rx at an arbitrary moment.
				}
				else {
					Utilities.WaitForReceivingCycleAndVerifyCounts(terminalB, testSet, cycle);
				}

				// Verify transmission:
				if (testSet.ClearedIsExpectedInTheEnd && (terminalA == terminalB)) { // Clear* on loopback self:
				////Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
				////                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
				////                      testSet, cycle)  doesn't work because clear will also be applied to Rx at an arbitrary moment.
				}
				else {
					Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
					                      terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
					                      testSet, cycle);
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
