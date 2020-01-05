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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

namespace MKY.Collections.Specialized
{
	/// <summary>
	/// Collection of values of a histogram.
	/// </summary>
	/// <typeparam name="T">The type of the values of the histogram</typeparam>
	public abstract class Histogram<T>
		where T : IComparable<T>
	{
		/// <summary></summary>
		public const int DefaultBinCount = 101;

		/// <summary></summary>
		public readonly int MaxBinCount; // Must be initialized with up to 'int.MaxValue'.

		/// <summary></summary>
		public readonly bool AutoRearrange; // = false;

		/// <summary></summary>
		public T Min { get; protected set; } // = default(T);

		/// <summary></summary>
		public T Max { get; protected set; } // = default(T);

		/// <summary></summary>
		public T BinSize { get; protected set; } // = default(T);

		/// <summary></summary>
		protected List<T> Values = new List<T>(); // No preset needed, the default behavior is good enough.

		/// <summary></summary>
		protected List<long> Bins; // = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class with equally
		/// distributed bins, fixed to the given arguments.
		/// </summary>
		public Histogram(T min, T max, int binCount)
		{
			Min = min;
			Max = max;
			MaxBinCount = binCount;
			InitializeBins(binCount);
			AutoRearrange = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class with equally
		/// distributed bins, automatically rearranging up to <paramref name="maxBinCount"/>.
		/// </summary>
		public Histogram(int initialBinCount, int maxBinCount)
		{
			MaxBinCount = maxBinCount;
			InitializeBins(initialBinCount);
			AutoRearrange = true;
		}

		private void InitializeBins(int binCount)
		{
			Bins = new List<long>(binCount);
			for (int i = 0; i < binCount; i++)
				Bins.Add(0);
		}

		/// <summary>
		/// Gets the current bin count.
		/// </summary>
		public virtual int BinCount
		{
			get { return (Bins.Count); }
		}

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="value"/>.
		/// </summary>
		protected abstract int GetIndex(T value);

		/// <summary>
		/// Adds an item to the histogram.
		/// </summary>
		public virtual void Add(T item)
		{
			Values.Add(item);

			if ((item.CompareTo(Min) >= 0) && (item.CompareTo(Max) <= 0))
				Bins[GetIndex(item)]++;
			else if (AutoRearrange)
				Rearrange();
			else
				throw (new ArgumentOutOfRangeException("item", item, "The value of the item is outside 'Min'/'Max' and 'AutoRearrange' is 'false'!"));
		}

		/// <summary>
		/// Rearranges the collection up to <see cref="MaxBinCount"/>.
		/// Needed when <see cref="AutoRearrange"/> is <c>true</c>.
		/// </summary>
		protected abstract void Rearrange();
	}

	/// <summary>
	/// Collection of values of type <see cref="double"/> of a histogram.
	/// </summary>
	public class HistogramDouble : Histogram<double>
	{
		/// <summary></summary>
		public const double DefaultMin = -0.5;

		/// <summary></summary>
		public const double DefaultMax = 100.5;

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramDouble"/> class with equally
		/// distributed bins, fixed to the given arguments.
		/// </summary>
		public HistogramDouble(double min = DefaultMin, double max = DefaultMax, int binCount = DefaultBinCount)
			: base(min, max, binCount)
		{
			BinSize = ((max - min) / binCount);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramDouble"/> class with equally
		/// distributed bins, automatically rearranging up to <paramref name="maxBinCount"/>.
		/// </summary>
		public HistogramDouble(int initialBinCount, int maxBinCount)
			: base(initialBinCount, maxBinCount)
		{
		////BinSize = 0.0 = default(T);
		}

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="value"/>.
		/// </summary>
		protected override int GetIndex(double value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Rearranges the collection up to <see cref="Histogram{T}.MaxBinCount"/>.
		/// Needed when <see cref="Histogram{T}.AutoRearrange"/> is <c>true</c>.
		/// </summary>
		protected override void Rearrange()
		{
			throw new NotImplementedException();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
