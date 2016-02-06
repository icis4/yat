//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
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
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;
using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Terminal;

#endregion

namespace YAT.Model.Test.Transmission
{
	/// <summary></summary>
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
			Commands = new List<Pair<Pair<string, string>, TimeSpan>>();
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"S",  @"S S       0.00 g"), TimeSpan.FromSeconds(90.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"SI", @"S S       0.00 g"), TimeSpan.FromSeconds(15.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"I1", @"I1 A ""0123"" ""2.30"" ""2.22"" ""2.33"" ""2.20"""), TimeSpan.FromSeconds(50.0 / 1000)));
			Commands.Add(new Pair<Pair<string, string>, TimeSpan>(new Pair<string, string>(@"I6", @"ES"),  TimeSpan.FromSeconds(15.0 / 1000)));
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

					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,        1).SetName(stimulus +  "_1"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected,       10).SetName(stimulus + "_10"), null));
					yield return (new KeyValuePair<TestCaseData, string>(new TestCaseData(stimulus, expected, loops01m).SetName(stimulus + "_" + loops01m), category01m));
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
				List<Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>> devs = new List<Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>>();

				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ||
					!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedTextSerialPortMTSicsDeviceASettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsConnected, "SerialPort_DeviceA_"));
				}

				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
				{
					Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedTextSerialPortMTSicsDeviceBSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsConnected, "SerialPort_DeviceB_"));
				}

				// Add device in any case => 'Ignore' is issued if device is not available.
				{
					Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedTextTcpAutoSocketMTSicsDeviceSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.Net.Test.ConfigurationCategoryStrings.MTSicsDeviceIsAvailable, "TcpAutoSocket_Device_"));
				}

				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected ||
					!MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedTextUsbSerialHidMTSicsDeviceASettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsConnected, "UsbSerialHid_DeviceA_"));
				}

				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
				{
					Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDelegate = new Pair<Utilities.TerminalSettingsDelegate<string>, string>(Utilities.GetStartedTextUsbSerialHidMTSicsDeviceBSettings, null);
					devs.Add(new Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsConnected, "UsbSerialHid_DeviceB_"));
				}

				foreach (Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string> dev in devs)
				{
					foreach (KeyValuePair<TestCaseData, string> kvp in TestCasesWithDurationCategory)
					{
						// Arguments:
						List<object> args = new List<object>(kvp.Key.Arguments);
						args.Insert(0, dev.Value1); // Insert the settings delegate at the beginning.
						TestCaseData tcd = new TestCaseData(args.ToArray());

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
	[TestFixture]
	public class MTSicsDeviceTest
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
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

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
		[Test, TestCaseSource(typeof(MTSicsDeviceTestData), "TestCases")]
		public virtual void PerformTransmission(Pair<Utilities.TerminalSettingsDelegate<string>, string> settingsDescriptor, string stimulus, string expected, int transmissionCount)
		{
			TerminalSettingsRoot settings = settingsDescriptor.Value1(settingsDescriptor.Value2);

			// Ensure that EOL is displayed, otherwise the EOL bytes are not available for verification.
			settings.TextTerminal.ShowEol = true;

			// Create terminals from settings:
			using (Terminal terminal = new Terminal(settings))
			{
				terminal.MessageInputRequest += new EventHandler<MessageInputEventArgs>(Utilities.TerminalMessageInputRequest);
				if (!terminal.Start())
				{
					if (Utilities.TerminalMessageInputRequestResultsInExclude) {
						Assert.Ignore(Utilities.TerminalMessageInputRequestResultsInExcludeText);
						// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
					}
					else {
						Assert.Fail(@"Failed to start """ + terminal.Caption + @"""");
					}
				}
				Utilities.WaitForConnection(terminal);

				// Prepare stimulus and expected:
				Types.Command stimulusCommand = new Types.Command(stimulus);
				
				List<byte> l = new List<byte>();
				foreach (char c in expected.ToCharArray())
					l.Add((byte)c); // ASCII only!

				l.Add(0x0D); // <CR>
				l.Add(0x0A); // <LF>

				byte[] expectedBytes = l.ToArray();
				int expectedTotalByteCount = 0;

				for (int i = 0; i < transmissionCount; i++)
				{
					// Send stimulus to device:
					Trace.WriteLine(@">> """ + stimulus + @""" (" + i + ")");
					terminal.SendText(stimulusCommand);
					expectedTotalByteCount += expectedBytes.Length;
					Utilities.WaitForTransmission(terminal, i + 1, expectedTotalByteCount); // i = transmission count equals line count.

					// Verify response:
					Domain.DisplayLine lastLine = terminal.LastDisplayLineAuxiliary(Domain.RepositoryType.Rx);
					byte[] actualBytes = lastLine.ElementsToOrigin();
					Assert.True(ArrayEx.ValuesEqual(expectedBytes, actualBytes), "Unexpected respose from device! Should be " + ArrayEx.ElementsToString(expectedBytes) + " but is " + ArrayEx.ElementsToString(actualBytes));
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
