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

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test
{
	/// <summary></summary>
	public static class GenericData
	{
		/// <remarks>Explicitly using two settings for "Pair" test cases, instead of enumerable generic number of settings.</remarks>
		private static IEnumerable<TestCaseData> ToTestCase(Descriptor descriptor, TerminalSettings settingsA, TerminalSettings settingsB)
		{
			// Arguments:
			var args = new List<object>(2); // Preset the required capacity to improve memory management.
			args.Add(settingsA);
			args.Add(settingsB);
			var tc = new TestCaseData(args.ToArray()); // TestCaseData(TerminalSettings settingsA, TerminalSettings settingsB).

			// Name:
			tc.SetName(descriptor.Name);

			// Category(ies):
			foreach (string cat in descriptor.Categories)
				tc.SetCategory(cat);

			yield return (tc);
		}

		/// <remarks>Explicitly using a single setting for "Self" test cases, instead of enumerable generic number of settings.</remarks>
		private static IEnumerable<TestCaseData> ToTestCases(Descriptor descriptor, TerminalSettings settings)
		{
			// Arguments:
			var args = new List<object>(1); // Preset the required capacity to improve memory management.
			args.Add(settings);
			var tc = new TestCaseData(args.ToArray()); // TestCaseData(TerminalSettings settings).

			// Name:
			tc.SetName(descriptor.Name);

			// Category(ies):
			foreach (string cat in descriptor.Categories)
				tc.SetCategory(cat);

			yield return (tc);
		}

		#region SerialPort
		//==========================================================================================
		// SerialPort
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize variational manner of this item.")]
		public static IEnumerable TestCasesSerialPortLoopbackPairs_Text
		{
			get
			{
				var loopbackPairs = Descriptors.SerialPortLoopbackPairs;
				if (loopbackPairs.Count() > 0)
				{
					foreach (var descriptor in loopbackPairs)
					{
						var settingsA = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortA);
						var settingsB = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortB);

						foreach (var tc in ToTestCase(descriptor, settingsA, settingsB))
							yield return (tc);
					}
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
				var loopbackSelfs = Descriptors.SerialPortLoopbackSelfs;
				if (loopbackSelfs.Count() > 0)
				{
					foreach (var descriptor in loopbackSelfs)
					{
						var settings = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.Port);

						foreach (var tc in ToTestCases(descriptor, settings))
							yield return (tc);
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
