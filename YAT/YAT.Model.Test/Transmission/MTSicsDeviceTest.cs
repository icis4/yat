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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;
using MKY.Settings;
using MKY.Windows.Forms;

using NUnit.Framework;
using NUnitEx;

using YAT.Domain.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Model.Test.Transmission
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	public static class MTSicsDeviceTestData
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
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic, and anyway, performance isn't an issue here.")]
		static MTSicsDeviceTestData()
		{
			Commands = new List<Pair<Pair<string, string>, TimeSpan>>(4); // Preset the required capacity to improve memory management.
			                                                                                                                                          // Time in ms per cycle, including overhead.
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"S",  @"S S       0.00 g"),                                  TimeSpan.FromSeconds(150.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"SI", @"S S       0.00 g"),                                  TimeSpan.FromSeconds(125.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"I1", @"I1 A ""0123"" ""2.30"" ""2.22"" ""2.33"" ""2.20"""), TimeSpan.FromSeconds(125.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"I6", @"ES"),                                                TimeSpan.FromSeconds(125.0 / 1000)));
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		private static IEnumerable<KeyValuePair<TestCaseData, string>> TestsWithDurationCategory
		{
			get
			{
				// An additional test set of MinuteDurationCategoryAttribute(1) makes little sense.
				// That just prolongs a test run of MTSicsDeviceTest.Transmission() to approx. 15
				// minutes at little to no additional test coverage.
				// Such test set of MinuteDurationCategoryAttribute(1) resulted in 400 or 480 loops.
				// Thus, the sets were e.g. 1/10/400/4000/24000/576000 loops. Instead of such long
				// set, the 10 loops test was replaced by sets of 2/5/20 which should give better
				// test coverage.

				string category15m = new StandardDurationCategory.Minutes15Attribute().Name;
				string category01h = new StandardDurationCategory.Hour1Attribute()    .Name;
				string category24h = new StandardDurationCategory.Hours24Attribute()  .Name;

				foreach (var c in Commands)
				{
					// Get stimulus and expected:
					string stimulus = c.Value1.Value1;
					string expected = c.Value1.Value2;

					// Calculate number of transmissions based on the expected time available/required:
					int loops15m = (int)((      900.0 * 1000) / c.Value2.TotalMilliseconds);
					int loops01h = (int)(( 1 * 3600.0 * 1000) / c.Value2.TotalMilliseconds);
					int loops24h = (int)((24 * 3600.0 * 1000) / c.Value2.TotalMilliseconds);

					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,        1).SetName(stimulus +  "_1"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,        2).SetName(stimulus +  "_2"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,        5).SetName(stimulus +  "_5"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,       20).SetName(stimulus + "_20"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected, loops15m).SetName(stimulus + "_" + loops15m), category15m));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected, loops01h).SetName(stimulus + "_" + loops01h), category01h));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected, loops24h).SetName(stimulus + "_" + loops24h), category24h));
				}
			}
		}

		private static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TerminalSettings settings, KeyValuePair<TestCaseData, string> kvp)
		{
			var tc = Domain.Test.Data.ToTestCase(descriptor, kvp.Key, settings, kvp.Key.Arguments);

			if (!string.IsNullOrEmpty(kvp.Value))
				tc.Categories.Add(kvp.Value);

			return (tc);
		}

		/// <summary></summary>
		public static int DeviceCount
		{
			get
			{
				var count =
				(
					Domain.Test.Environment.MTSicsSerialPortDevices.Count() +
					Domain.Test.Environment.MTSicsIPDevices        .Count() +
					Domain.Test.Environment.MTSicsUsbDevices       .Count()
				);
				return (count);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				if (DeviceCount > 0)
				{
					foreach (var descriptor in Domain.Test.Environment.MTSicsSerialPortDevices)
					{
						var settings = Domain.Test.Settings.GetMTSicsSerialPortDeviceSettings(descriptor.Port);

						foreach (var kvp in TestsWithDurationCategory)
							yield return (ToTestCase(descriptor, settings, kvp));
					}

					foreach (var descriptor in Domain.Test.Environment.MTSicsIPDevices)
					{
						var settings = Domain.Test.Settings.GetMTSicsIPDeviceSettings(descriptor.Port);

						foreach (var kvp in TestsWithDurationCategory)
							yield return (ToTestCase(descriptor, settings, kvp));
					}

					foreach (var descriptor in Domain.Test.Environment.MTSicsUsbDevices)
					{
						var settings = Domain.Test.Settings.GetMTSicsUsbSerialHidDeviceSettings(descriptor.DeviceInfo);

						foreach (var kvp in TestsWithDurationCategory)
							yield return (ToTestCase(descriptor, settings, kvp));
					}
				}
				else
				{
					var na = new TestCaseData(null);
					na.SetName("*NO* MT-SICS devices are available => FIX OR ACCEPT YELLOW BAR");
					yield return (na); // Test is mandatory, it shall not be excludable. 'MTSicsDevicesCount' is to be probed in tests.
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// It can be argued that this test would be better located in YAT.Domain.Test. It currently is
	/// located here because line counts and rates are calculated in <see cref="Terminal"/> and are
	/// required when evaluating the test result.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "MT-SICS is a name.")]
	[TestFixture]
	public class MTSicsDeviceTest
	{
		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Don't care, it's for debugging only...")]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// \remind (2016-05-26 / MKY) should be guarded by if (isRunningFromGui) to prevent the message box in case of automatic test runs.
			// \remind (2017-10-09 / MKY) even better to be replaced by a runtime check for availability of a weighing capable MT-SICS device.
			var dr = MessageBoxEx.Show
			(
				"This test requires a weighing MT-SICS device:" + Environment.NewLine +
				" > Serial COM Port: Start WeightSim if no load cell connected." + Environment.NewLine +
				" > TCP/IP Socket: Start device simulation." + Environment.NewLine +
				" > USB Ser/HID: Start WeightSim if no load cell connected.",
				"Precondition",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
			);

			if (dr != DialogResult.OK)
				Assert.Ignore("Tester has canceled");

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

		/// <remarks>
		/// Test is optional, it automatically adjusts to the available MT-SICS devices.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(MTSicsDeviceTestData), "TestCases")]
		public virtual void Transmission(TerminalSettings settings, string stimulus, string expected, int transmissionCount)
		{
			if (MTSicsDeviceTestData.DeviceCount <= 0)
				Assert.Ignore("No MT-SICS devices are available, therefore this test is excluded. Ensure that at least one MT-SICS devices is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			// Ensure that EOL is displayed, otherwise the EOL bytes are not available for verification:
			settings.TextTerminal.ShowEol = true;

			// Create terminals from settings:
			using (var terminal = new Terminal(Settings.Create(settings)))
			{
				terminal.MessageInputRequest += Utilities.TerminalMessageInputRequest;
				if (!terminal.Start())
				{
					if (Utilities.TerminalMessageInputRequestResultsInExclude) {
						Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
					//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
					}
					else {
						Assert.Fail(@"Failed to start """ + terminal.Caption + @"""");
					}
				}
				Utilities.WaitForOpen(terminal);

				// Prepare stimulus and expected:
				var stimulusCommand = new Types.Command(stimulus);

				var l = new List<byte>(expected.Length); // Preset the required capacity to improve memory management.
				foreach (char c in expected.ToCharArray())
					l.Add((byte)c); // ASCII only!

				l.Add(0x0D); // <CR>
				l.Add(0x0A); // <LF>

				var expectedBytes = l.ToArray();
				int expectedTotalRxByteCount = 0;

				for (int i = 0; i < transmissionCount; i++)
				{
					// Send stimulus to device:
					Trace.WriteLine(@">> """ + stimulus + @""" (" + i + ")");
					terminal.SendText(stimulusCommand);
					expectedTotalRxByteCount += expectedBytes.Length;
					Utilities.WaitForReceivingAndAssertCounts(terminal, expectedTotalRxByteCount, i + 1); // i = transmission count equals line count.

					// Verify response:
					var lastLine = terminal.LastDisplayLineAuxiliary(Domain.RepositoryType.Rx);
					var actualBytes = lastLine.ElementsToOrigin();
					Assert.That(ArrayEx.ValuesEqual(expectedBytes, actualBytes), "Unexpected response from device! Should be " + ArrayEx.ValuesToString(expectedBytes) + " but is " + ArrayEx.ValuesToString(actualBytes));
					Trace.WriteLine(@"<< """ + expected + @"""");
					terminal.ClearLastDisplayLineAuxiliary(Domain.RepositoryType.Rx);
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
