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
// YAT Version 2.1.1 Development
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

using MKY.Collections.Generic;

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test
{
	/// <summary></summary>
	public static class GenericTestData
	{
		#region LoopbackPairs
		//==========================================================================================
		// LoopbackPairs
		//==========================================================================================

		/// <param name="lp">
		/// Quadruple of...
		/// ...Pair(terminalSettingsDelegateA, terminalSettingsArgumentA)...
		/// ...Pair(terminalSettingsDelegateB, terminalSettingsArgumentB)...
		/// ...string testCaseName...
		/// ...string[] testCaseCategories.
		/// </param>
		private static IEnumerable<TestCaseData> SerialPortLoopbackPairsData(Quadruple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> lp)
		{
		////foreach (var data in TestCasesData) // TestCaseData(...) kept for symmetricity with YAT.Model.Test.
			{
				// Arguments:
				var args = new List<object>(2); // data.Arguments);
				args.Add(lp.Value1);
				args.Add(lp.Value2);
				var tcd = new TestCaseData(args.ToArray()); // TestCaseData(Pair settingsDescriptorA, Pair settingsDescriptorB).

				// Name:
				tcd.SetName(lp.Value3); // + data.TestName);

				// Category(ies):
				foreach (string cat in lp.Value4)
					tcd.SetCategory(cat);

				yield return (tcd);
			}
		}

		/// <summary></summary>
		public static IEnumerable TestEnvironmentSerialPortLoopbackPairs
		{
			get
			{
				var loopbackPairs = Utilities.TransmissionSettings.SerialPortLoopbackPairs;
				if (loopbackPairs.Count() > 0)
				{
					foreach (var lp in loopbackPairs)
					{
						foreach (var tc in SerialPortLoopbackPairsData(lp))
							yield return (tc);
					}
				}
				else
				{
					var tcd = new TestCaseData(null);
					tcd.SetName("*NO* serial COM port loopback pairs are available => FIX OR ACCEPT YELLOW BAR");
					yield return (tcd); // Test is mandatory, it shall not be excludable. 'LoopbackPairsAreAvailable' is to be probed in tests.
				}
			}
		}

		#endregion

		#region LoopbackSelfs
		//==========================================================================================
		// LoopbackSelfs
		//==========================================================================================

		/// <param name="lp">
		/// Triple of...
		/// ...Pair(terminalSettingsDelegate, terminalSettingsArgument)...
		/// ...string testCaseName...
		/// ...string[] testCaseCategories.
		/// </param>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		private static IEnumerable<TestCaseData> SerialPortLoopbackSelfsData(Triple<Pair<Utilities.TerminalSettingsDelegate<string>, string>, string, string[]> lp)
		{
		////foreach (var data in TestCasesData) // TestCaseData(...) kept for symmetricity with YAT.Model.Test.
			{
				// Arguments:
				var args = new List<object>(1); // data.Arguments);
				args.Add(lp.Value1);
				var tcd = new TestCaseData(args.ToArray()); // TestCaseData(Pair settingsDescriptor).

				// Name:
				tcd.SetName(lp.Value2); // + data.TestName);

				// Category(ies):
				foreach (string cat in lp.Value3)
					tcd.SetCategory(cat);

				yield return (tcd);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
		public static IEnumerable TestEnvironmentSerialPortLoopbackSelfs
		{
			get
			{
				var loopbackSelfs = Utilities.TransmissionSettings.SerialPortLoopbackSelfs;
				if (loopbackSelfs.Count() > 0)
				{
					foreach (var ls in loopbackSelfs)
					{
						foreach (var tc in SerialPortLoopbackSelfsData(ls))
							yield return (tc);
					}
				}
				else
				{
					var tcd = new TestCaseData(null);
					tcd.SetName("*NO* serial COM port loopback selfs are available => FIX OR ACCEPT YELLOW BAR");
					yield return (tcd); // Test is mandatory, it shall not be excludable. 'LoopbackSelfsAreAvailable' is to be probed in tests.
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
