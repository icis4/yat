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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

using MKY.Collections.Specialized;

using NUnit.Framework;

namespace MKY.Test.Collections.Specialized
{
	/// <summary></summary>
	[TestFixture]
	public class HistogramTest
	{
		private const int Few = 10;

		/// <summary></summary>
		[Test]
		public virtual void TestHistogramDoubleDefault()
		{
			var histogram = new HistogramDouble();
			Assert.That(histogram.Min, Is.EqualTo(HistogramDouble.DefaultMin));
			Assert.That(histogram.Max, Is.EqualTo(HistogramDouble.DefaultMax));
			Assert.That(histogram.BinCount, Is.EqualTo(HistogramDouble.DefaultBinCount));
			Assert.That(histogram.BinSize, Is.GreaterThan(0.0));
			Assert.That(histogram.ItemCount, Is.EqualTo(0));

			histogram.AddRange(new double[] { -100.0, -99.99, -0.01, 0.0, 0.01, 99.99, 100.0 });
			var lowIndex = 0;
			var midIndex = (int)Math.Ceiling((double)(HistogramDouble.DefaultBinCount - 1) / 2);
			var highIndex = (HistogramDouble.DefaultBinCount - 1);
			Assert.That(histogram.Min, Is.EqualTo(HistogramDouble.DefaultMin));
			Assert.That(histogram.Max, Is.EqualTo(HistogramDouble.DefaultMax));
			Assert.That(histogram.BinCount, Is.EqualTo(HistogramDouble.DefaultBinCount));
			Assert.That(histogram.BinSize, Is.GreaterThan(0.0));
			Assert.That(histogram.ItemCount, Is.EqualTo(7));
			Assert.That(histogram.Counts.Count, Is.EqualTo(HistogramDouble.DefaultBinCount));
			Assert.That(histogram.Counts[lowIndex],  Is.EqualTo(2));
			Assert.That(histogram.Counts[midIndex],  Is.EqualTo(3));
			Assert.That(histogram.Counts[highIndex], Is.EqualTo(2));
		}

		/// <summary></summary>
		[Test]
		public virtual void TestHistogramDoubleAutoGrow()
		{
			const int MaxBinCount = 256; // Proximity for ~10.0 is +/- 0.1953, i.e. 0.01 is sufficient.
			const double DeltaWithinProximity = 0.01;

			// Immediately adding proximate values:
			{
				var histogram = new HistogramDouble(MaxBinCount);
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(1));
				Assert.That(histogram.BinSize, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(0));

				histogram.Add(0.0);
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(1));
				Assert.That(histogram.BinSize, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(1));
				Assert.That(histogram.Counts.Count, Is.EqualTo(1));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));

				histogram.Add(0.0 + DeltaWithinProximity);
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(0.0 + DeltaWithinProximity).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(2));
				Assert.That(histogram.BinSize, Is.EqualTo(DeltaWithinProximity / 2).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(2));
				Assert.That(histogram.Counts.Count, Is.EqualTo(2));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));
				Assert.That(histogram.Counts[1], Is.EqualTo(1));
			}

			// Later adding proximate values:
			{
				var histogram = new HistogramDouble(MaxBinCount);
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(1));
				Assert.That(histogram.BinSize, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(0));

				histogram.Add(0.0);
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(1));
				Assert.That(histogram.BinSize, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(1));
				Assert.That(histogram.Counts.Count, Is.EqualTo(1));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));

				histogram.Add(10.0);
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(10.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(2));
				Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(2));
				Assert.That(histogram.Counts.Count, Is.EqualTo(2));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));
				Assert.That(histogram.Counts[1], Is.EqualTo(1));

				histogram.Add(10.0 + DeltaWithinProximity); // = in proximity of item
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(10.0 + DeltaWithinProximity).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(2));
				Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(3));
				Assert.That(histogram.Counts.Count, Is.EqualTo(2));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));
				Assert.That(histogram.Counts[1], Is.EqualTo(2));

				histogram.Add(20.0);
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(20.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(4));
				Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(4));
				Assert.That(histogram.Counts.Count, Is.EqualTo(4));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));
				Assert.That(histogram.Counts[1], Is.EqualTo(2));
				Assert.That(histogram.Counts[3], Is.EqualTo(1));

				histogram.Add(20.0 - DeltaWithinProximity); // = in proximity of item
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(20.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(4));
				Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(5));
				Assert.That(histogram.Counts.Count, Is.EqualTo(4));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));
				Assert.That(histogram.Counts[1], Is.EqualTo(2));
				Assert.That(histogram.Counts[3], Is.EqualTo(2));

				histogram.Add(12.5 + DeltaWithinProximity); // = in proximity of bin value
				Assert.That(histogram.Min, Is.EqualTo(0.0).Within(Few).Ulps);
				Assert.That(histogram.Max, Is.EqualTo(20.0).Within(Few).Ulps);
				Assert.That(histogram.BinCount, Is.EqualTo(4));
				Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(Few).Ulps);
				Assert.That(histogram.ItemCount, Is.EqualTo(6));
				Assert.That(histogram.Counts.Count, Is.EqualTo(4));
				Assert.That(histogram.Counts[0], Is.EqualTo(1));
				Assert.That(histogram.Counts[1], Is.EqualTo(2));
				Assert.That(histogram.Counts[2], Is.EqualTo(1));
				Assert.That(histogram.Counts[3], Is.EqualTo(2));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
