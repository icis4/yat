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
using System.Linq;

using NUnit;
using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test
{
	/// <remarks>Just named "Data" instead of "TestCaseData" for compactness.</remarks>
	public static class Data
	{
		/// <remarks>Explicitly using two settings for "Pair" test cases, instead of enumerable generic number of settings.</remarks>
		public static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TestCaseData metaDataToMerge, TerminalSettings argSettingsA, TerminalSettings argSettingsB, params object[] argsTest)
		{
			var args = new List<object>(2 + argsTest.Length); // Preset the required capacity to improve memory management.
			args.Add(argSettingsA);
			args.Add(argSettingsB);
			args.AddRange(argsTest);
			return (TestCaseDataEx.ToTestCase(descriptor, metaDataToMerge, args.ToArray())); // Args must be given as a liner list of objects.
		}

		/// <remarks>Explicitly using a single setting for "Self" test cases, instead of enumerable generic number of settings.</remarks>
		public static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TestCaseData metaDataToMerge, TerminalSettings argSettings, params object[] argsTest)
		{
			var args = new List<object>(1 + argsTest.Length); // Preset the required capacity to improve memory management.
			args.Add(argSettings);
			args.AddRange(argsTest);
			return (TestCaseDataEx.ToTestCase(descriptor, metaDataToMerge, args.ToArray())); // Args must be given as a liner list of objects.
		}

		/// <remarks>Explicitly using two settings for "Pair" test cases, instead of enumerable generic number of settings.</remarks>
		public static IEnumerable<TestCaseData> ToTestCases(TestCaseDescriptor descriptor, TerminalSettings settingsA, TerminalSettings settingsB, IEnumerable<TestCaseData> tests)
		{
			if (tests != null)
			{
				foreach (var t in tests)
					yield return (ToTestCase(descriptor, t, settingsA, settingsB, t.Arguments));
			}
			else
			{
				yield return (ToTestCase(descriptor, null, settingsA, settingsB));
			}
		}

		/// <remarks>Explicitly using a single setting for "Self" test cases, instead of enumerable generic number of settings.</remarks>
		public static IEnumerable<TestCaseData> ToTestCases(TestCaseDescriptor descriptor, TerminalSettings settings, IEnumerable<TestCaseData> tests)
		{
			if (tests != null)
			{
				foreach (var t in tests)
					yield return (ToTestCase(descriptor, t, settings, t.Arguments));
			}
			else
			{
				yield return (ToTestCase(descriptor, null, settings));
			}
		}

		#region SerialPort
		//------------------------------------------------------------------------------------------
		// SerialPort
		//------------------------------------------------------------------------------------------

		private static IEnumerable<TestCaseData> ToSerialPortLoopbackPairsTestCases(TerminalType terminalType)
		{
			return (ToSerialPortLoopbackPairsTestCases(terminalType, null));
		}

		private static IEnumerable<TestCaseData> ToSerialPortLoopbackPairsTestCases(TerminalType terminalType, IEnumerable<TestCaseData> tests)
		{
			foreach (var descriptor in Environment.SerialPortLoopbackPairs) // Upper level grouping shall be 'by I/O'.
			{
				var settingsA = Settings.GetSerialPortSettings(terminalType, descriptor.PortA);
				var settingsB = Settings.GetSerialPortSettings(terminalType, descriptor.PortB);

				foreach (var tc in ToTestCases(descriptor, settingsA, settingsB, tests))
					yield return (tc);
			}
		}

		/// <summary></summary>
		public static IEnumerable<TestCaseData> ToSerialPortLoopbackPairsTestCases_Text(IEnumerable<TestCaseData> tests)
		{
			foreach (var tc in ToSerialPortLoopbackPairsTestCases(TerminalType.Text, tests))
				yield return (tc);
		}

		private static IEnumerable<TestCaseData> ToSerialPortLoopbackSelfsTestCases(TerminalType terminalType)
		{
			return (ToSerialPortLoopbackSelfsTestCases(terminalType, null));
		}

		private static IEnumerable<TestCaseData> ToSerialPortLoopbackSelfsTestCases(TerminalType terminalType, IEnumerable<TestCaseData> tests)
		{
			foreach (var descriptor in Environment.SerialPortLoopbackSelfs) // Upper level grouping shall be 'by I/O'.
			{
				var settings = Settings.GetSerialPortSettings(terminalType, descriptor.Port);

				foreach (var tc in ToTestCases(descriptor, settings, tests))
					yield return (tc);
			}
		}

		/// <summary></summary>
		public static IEnumerable<TestCaseData> ToSerialPortLoopbackSelfsTestCases_Text(IEnumerable<TestCaseData> tests)
		{
			foreach (var tc in ToSerialPortLoopbackSelfsTestCases(TerminalType.Text, tests))
				yield return (tc);
		}

		#endregion

		#region Socket
		//------------------------------------------------------------------------------------------
		// Socket
		//------------------------------------------------------------------------------------------

		private static IEnumerable<TestCaseData> ToIPSocketPairsTestCases(TerminalType terminalType)
		{
			return (ToIPSocketPairsTestCases(terminalType, null));
		}

		private static IEnumerable<TestCaseData> ToIPSocketPairsTestCases(TerminalType terminalType, IEnumerable<TestCaseData> tests)
		{
			foreach (var descriptor in Environment.IPSocketPairs) // Upper level grouping shall be 'by I/O'.
			{
				var settingsA = Settings.GetIPSocketSettings(terminalType, descriptor.SocketTypeA, descriptor.LocalInterface);
				var settingsB = Settings.GetIPSocketSettings(terminalType, descriptor.SocketTypeB, descriptor.LocalInterface);

				foreach (var tc in ToTestCases(descriptor, settingsA, settingsB, tests))
					yield return (tc);
			}
		}

		/// <summary></summary>
		public static IEnumerable<TestCaseData> ToIPSocketPairsTestCases_Text(IEnumerable<TestCaseData> tests)
		{
			foreach (var tc in ToIPSocketPairsTestCases(TerminalType.Text, tests))
				yield return (tc);
		}

		private static IEnumerable<TestCaseData> ToIPSocketSelfsTestCases(TerminalType terminalType)
		{
			return (ToIPSocketSelfsTestCases(terminalType, null));
		}

		private static IEnumerable<TestCaseData> ToIPSocketSelfsTestCases(TerminalType terminalType, IEnumerable<TestCaseData> tests)
		{
			foreach (var descriptor in Environment.IPSocketSelfs) // Upper level grouping shall be 'by I/O'.
			{
				var settings = Settings.GetIPSocketSettings(terminalType, descriptor.SocketType, descriptor.LocalInterface);

				foreach (var tc in ToTestCases(descriptor, settings, tests))
					yield return (tc);
			}
		}

		/// <summary></summary>
		public static IEnumerable<TestCaseData> ToIPSocketSelfsTestCases_Text(IEnumerable<TestCaseData> tests)
		{
			foreach (var tc in ToIPSocketSelfsTestCases(TerminalType.Text, tests))
				yield return (tc);
		}

		#endregion

		/// <summary></summary>
		public static class Generic
		{
			#region SerialPort
			//--------------------------------------------------------------------------------------
			// SerialPort
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
			public static IEnumerable TestCasesSerialPortLoopbackPairs_Text
			{
				get
				{
					var loopbackPairs = Environment.SerialPortLoopbackPairs;
					if (loopbackPairs.Count() > 0)
					{
						foreach (var tc in ToSerialPortLoopbackPairsTestCases(TerminalType.Text))
							yield return (tc);
					}
					else
					{
						var na = new TestCaseData(null);
						na.SetName("*NO* serial COM port loopback pairs are available => FIX OR ACCEPT YELLOW BAR");
						yield return (na); // Test is mandatory, it shall not be excludable. 'LoopbackPairsAreAvailable' is to be probed in tests.
					}
				}
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
			[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
			public static IEnumerable TestCasesSerialPortLoopbackSelfs_Text
			{
				get
				{
					var loopbackSelfs = Environment.SerialPortLoopbackSelfs;
					if (loopbackSelfs.Count() > 0)
					{
						foreach (var descriptor in loopbackSelfs)
						{
							var settings = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.Port);
							yield return (ToTestCase(descriptor, null, settings));
						}
					}
					else
					{
						var na = new TestCaseData(null);
						na.SetName("*NO* serial COM port loopback selfs are available => FIX OR ACCEPT YELLOW BAR");
						yield return (na); // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is to be probed in tests.
					}
				}
			}

			#endregion
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
