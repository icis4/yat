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
		public static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TestCaseData metaDataToMerge, TerminalSettings settingsA, TerminalSettings settingsB)
		{
			return (TestCaseDataEx.ToTestCase(descriptor, metaDataToMerge, settingsA, settingsB));
		}

		/// <remarks>Explicitly using a single setting for "Self" test cases, instead of enumerable generic number of settings.</remarks>
		public static TestCaseData ToTestCase(TestCaseDescriptor descriptor, TestCaseData metaDataToMerge, TerminalSettings settings)
		{
			return (TestCaseDataEx.ToTestCase(descriptor, metaDataToMerge, settings));
		}

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
						foreach (var descriptor in loopbackPairs)
						{
							var settingsA = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortA);
							var settingsB = Settings.GetSerialPortSettings(TerminalType.Text, descriptor.PortB);
							yield return (ToTestCase(descriptor, null, settingsA, settingsB));
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
