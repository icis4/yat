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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2018 Matthias Kläy.
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
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;
using MKY.Settings;
using MKY.Windows.Forms;

using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Terminal;

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
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static MTSicsDeviceTestData()
		{
			Commands = new List<Pair<Pair<string, string>, TimeSpan>>(4); // Preset the required capacity to improve memory management.                Time in ms per cycle, including overhead.
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

		private static IEnumerable<KeyValuePair<TestCaseData, string>> TestCasesWithDurationCategory
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

				string category10m = new NUnit.MinuteDurationCategoryAttribute(10).Name;
				string category60m = new NUnit.MinuteDurationCategoryAttribute(60).Name;
				string category24h = new NUnit.HourDurationCategoryAttribute(24).Name;

				foreach (var c in Commands)
				{
					// Get stimulus and expected:
					string stimulus = c.Value1.Value1;
					string expected = c.Value1.Value2;

					// Calculate number of transmissions based on the expected time available/required:
					int loops10m = (int)((      600.0 * 1000) / c.Value2.TotalMilliseconds);
					int loops60m = (int)((     3600.0 * 1000) / c.Value2.TotalMilliseconds);
					int loops24h = (int)((24 * 3600.0 * 1000) / c.Value2.TotalMilliseconds);

					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,        1).SetName(stimulus +  "_1"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,        2).SetName(stimulus +  "_2"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,        5).SetName(stimulus +  "_5"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,       20).SetName(stimulus + "_20"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected, loops10m).SetName(stimulus + "_" + loops10m), category10m));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected, loops60m).SetName(stimulus + "_" + loops60m), category60m));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected, loops24h).SetName(stimulus + "_" + loops24h), category24h));
				}
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				var devs = new List<Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>>();

				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ||
					!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					var settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedSerialPortMTSicsDeviceATextSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsConnected, "SerialPort_MTSicsDeviceA_"));
				}

				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
				{
					var settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedSerialPortMTSicsDeviceBTextSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsConnected, "SerialPort_MTSicsDeviceB_"));
				}

				// Add device in any case => 'Ignore' is issued if device is not available.
				{
					var settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedTcpAutoSocketMTSicsDeviceTextSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.Net.Test.ConfigurationCategoryStrings.MTSicsDeviceIsAvailable, "TcpAutoSocket_MTSicsDevice_"));
				}

				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ||
					!MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					var settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedUsbSerialHidMTSicsDeviceATextSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsConnected, "UsbSerialHid_MTSicsDeviceA_"));
				}

				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
				{
					var settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedUsbSerialHidMTSicsDeviceBTextSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsConnected, "UsbSerialHid_MTSicsDeviceB_"));
				}

				foreach (var dev in devs)
				{
					foreach (var kvp in TestCasesWithDurationCategory)
					{
						// Arguments:
						var args = new List<object>(kvp.Key.Arguments);
						args.Insert(0, dev.Value1); // Insert the settings delegate at the beginning.
						var tcd = new TestCaseData(args.ToArray());

						// Category:
						tcd.SetCategory(dev.Value2); // Set device category.

						if (!string.IsNullOrEmpty(kvp.Value))
							tcd.SetCategory(kvp.Value); // Set predefined duration category.

						// Name:
						tcd.SetName(dev.Value3 + kvp.Key.TestName); // Also prepend device indicator.

						yield return (tcd);
					}
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
			// \remind (2017-10-09 / MKY) even better to be eliminated and moved to related tests as attributes.
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
				Assert.Fail("User cancel!");

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

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
		[Test, TestCaseSource(typeof(MTSicsDeviceTestData), "TestCases")]
		public virtual void Transmission(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor, string stimulus, string expected, int transmissionCount)
		{
			var settings = settingsDescriptor.Value1(settingsDescriptor.Value2);

			// Ensure that EOL is displayed, otherwise the EOL bytes are not available for verification:
			settings.TextTerminal.ShowEol = true;

			// Create terminals from settings:
			using (var terminal = new Terminal(settings))
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
				int expectedTotalByteCount = 0;

				for (int i = 0; i < transmissionCount; i++)
				{
					// Send stimulus to device:
					Trace.WriteLine(@">> """ + stimulus + @""" (" + i + ")");
					terminal.SendText(stimulusCommand);
					expectedTotalByteCount += expectedBytes.Length;
					Utilities.WaitForReceiving(terminal, expectedTotalByteCount, i + 1); // i = transmission count equals line count.

					// Verify response:
					var lastLine = terminal.LastDisplayLineAuxiliary(Domain.RepositoryType.Rx);
					var actualBytes = lastLine.ElementsToOrigin();
					Assert.That(ArrayEx.ElementsEqual(expectedBytes, actualBytes), "Unexpected response from device! Should be " + ArrayEx.ElementsToString(expectedBytes) + " but is " + ArrayEx.ElementsToString(actualBytes));
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
