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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

using MKY.Collections.Specialized;

using NUnit.Framework;

namespace MKY.Test.Collections.ObjectModel
{
	/// <summary></summary>
	[TestFixture]
	public class HistogramTest
	{
		/// <summary></summary>
		[Test]
		public virtual void TestHistogramDoubleDefault()
		{
			var histogram = new HistogramDouble();
			Assert.That(histogram.Min, Is.EqualTo(HistogramDouble.DefaultMin));
			Assert.That(histogram.Max, Is.EqualTo(HistogramDouble.DefaultMax));
			Assert.That(histogram.BinCount, Is.EqualTo(HistogramDouble.DefaultBinCount));
			Assert.That(histogram.BinSize, Is.GreaterThan(0.0));
			Assert.That(histogram.ValueCount, Is.EqualTo(0));

			histogram.AddRange(new double[] { -100.0, -99.99, -0.01, 0.0, 0.01, 99.99, 100.0 });
			Assert.That(histogram.Min, Is.EqualTo(HistogramDouble.DefaultMin));
			Assert.That(histogram.Max, Is.EqualTo(HistogramDouble.DefaultMax));
			Assert.That(histogram.BinCount, Is.EqualTo(HistogramDouble.DefaultBinCount));
			Assert.That(histogram.BinSize, Is.GreaterThan(0.0));
			Assert.That(histogram.ValueCount, Is.EqualTo(7));
			Assert.That(histogram.Counts.Count, Is.EqualTo(HistogramDouble.DefaultBinCount));
			Assert.That(histogram.Counts[0], Is.EqualTo(2));
			Assert.That(histogram.Counts[(int)Math.Ceiling((double)HistogramDouble.DefaultBinCount / 2)], Is.EqualTo(3));
			Assert.That(histogram.Counts[HistogramDouble.DefaultBinCount], Is.EqualTo(2));
		}

		/// <summary></summary>
		[Test]
		public virtual void TestHistogramDoubleAutoRearrange()
		{
			const int MaxBinCount = 256;

			var histogram = new HistogramDouble(MaxBinCount);
			Assert.That(histogram.Min, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.Max, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.BinCount, Is.EqualTo(0));
			Assert.That(histogram.BinSize, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.ValueCount, Is.EqualTo(0));

			histogram.Add(0.0);
			Assert.That(histogram.Min, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.Max, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.BinCount, Is.EqualTo(1));
			Assert.That(histogram.BinSize, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.ValueCount, Is.EqualTo(1));
			Assert.That(histogram.Counts.Count, Is.EqualTo(1));
			Assert.That(histogram.Counts[0], Is.EqualTo(1));

			histogram.Add(10.0);
			Assert.That(histogram.Min, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.Max, Is.EqualTo(10.0).Within(1).Ulps);
			Assert.That(histogram.BinCount, Is.EqualTo(2));
			Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(1).Ulps);
			Assert.That(histogram.ValueCount, Is.EqualTo(2));
			Assert.That(histogram.Counts.Count, Is.EqualTo(2));
			Assert.That(histogram.Counts[0], Is.EqualTo(1));
			Assert.That(histogram.Counts[1], Is.EqualTo(1));

			histogram.Add(20.0);
			Assert.That(histogram.Min, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.Max, Is.EqualTo(20.0).Within(1).Ulps);
			Assert.That(histogram.BinCount, Is.EqualTo(4));
			Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(1).Ulps);
			Assert.That(histogram.ValueCount, Is.EqualTo(3));
			Assert.That(histogram.Counts.Count, Is.EqualTo(3));
			Assert.That(histogram.Counts[0], Is.EqualTo(1));
			Assert.That(histogram.Counts[1], Is.EqualTo(1));
			Assert.That(histogram.Counts[3], Is.EqualTo(1));

			histogram.Add(20.0);
			Assert.That(histogram.Min, Is.EqualTo(0.0).Within(1).Ulps);
			Assert.That(histogram.Max, Is.EqualTo(20.0).Within(1).Ulps);
			Assert.That(histogram.BinCount, Is.EqualTo(4));
			Assert.That(histogram.BinSize, Is.EqualTo(5.0).Within(1).Ulps);
			Assert.That(histogram.ValueCount, Is.EqualTo(4));
			Assert.That(histogram.Counts.Count, Is.EqualTo(4));
			Assert.That(histogram.Counts[0], Is.EqualTo(1));
			Assert.That(histogram.Counts[1], Is.EqualTo(1));
			Assert.That(histogram.Counts[3], Is.EqualTo(2));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
