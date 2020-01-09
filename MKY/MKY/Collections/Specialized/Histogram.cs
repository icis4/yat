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
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MKY.Collections.Specialized
{
	/// <summary>
	/// How the historgram shall be limited.
	/// </summary>
	public enum HistogramOutOfBoundsBehavior
	{
		/// <summary>
		/// Ignores items outside <see cref="Histogram{T}.Min"/> and <see cref="Histogram{T}.Max"/>
		/// in case <see cref="Histogram{T}.AutoAdjust"/> is <c>false</c>.
		/// </summary>
		Ignore,

		/// <summary>
		/// Throws an exception for items outside <see cref="Histogram{T}.Min"/> and <see cref="Histogram{T}.Max"/>
		/// in case <see cref="Histogram{T}.AutoAdjust"/> is <c>false</c>.
		/// </summary>
		Throw,

		/// <summary>
		/// Items outside <see cref="Histogram{T}.Min"/> and <see cref="Histogram{T}.Max"/> let the
		/// histogram adjust, this is the case when <see cref="Histogram{T}.AutoAdjust"/> is <c>true</c>.
		/// </summary>
		Adjust
	}

	/// <summary>
	/// Collection of items of a histogram.
	/// </summary>
	/// <typeparam name="T">The type of the items of the histogram</typeparam>
	public abstract class Histogram<T> where T : IComparable<T>, IEquatable<T>
	{
		/// <summary></summary>
		public const HistogramOutOfBoundsBehavior DefaultOutOfBoundsBehavior = HistogramOutOfBoundsBehavior.Ignore;

		/// <summary></summary>
		public readonly int MaxBinCount; // Must be initialized with up to 'int.MaxValue'.

		/// <summary></summary>
		public readonly bool AutoAdjust; // = false;

		/// <summary></summary>
		public readonly HistogramOutOfBoundsBehavior OutOfBoundsBehavior; // = Ignore;

		/// <summary>The value corresponding to the smallest item, as well as the lower limit of the first bin.</summary>
		public T Min { get; protected set; } // = default(T);

		/// <summary>The value corresponding to the biggest item, as well as the upper limit of the last bin.</summary>
		public T Max { get; protected set; } // = default(T);

		/// <summary></summary>
		public T BinSize { get; protected set; } // = default(T);

		/// <summary></summary>
		protected List<T> Items = new List<T>(); // No preset needed, the default behavior is good enough.

		/// <summary></summary>
		protected List<long> BinCounts; // = null;

		/// <summary></summary>
		protected List<T> BinValuesLowerLimit; // null;

		/// <summary></summary>
		protected List<T> BinValuesMidPoint; // null;

		/// <summary></summary>
		protected List<T> BinValuesUpperLimit; // null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class with equally
		/// distributed bins, fixed to the given arguments.
		/// </summary>
		public Histogram(T min, T max, int binCount, HistogramOutOfBoundsBehavior outOfBoundsBehavior = DefaultOutOfBoundsBehavior)
		{
			Min = min;
			Max = max;
			MaxBinCount = binCount;
			BinSize = CalculateBinSize(min, max, binCount);
			InitializeBins(binCount, binCount);
			AutoAdjust = false;
			OutOfBoundsBehavior = outOfBoundsBehavior;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class with equally
		/// distributed bins, automatically adjusting <see cref="BinSize"/> as well as
		/// <see cref="BinCount"/> up to <paramref name="maxBinCount"/>.
		/// </summary>
		public Histogram(int maxBinCount)
		{
		////Min = default(T);
		////Max = default(T);
			MaxBinCount = maxBinCount;
		////BinSize = default(T);
		////InitializeBins(maxBinCount, 1);
			AutoAdjust = true;
			OutOfBoundsBehavior = HistogramOutOfBoundsBehavior.Adjust;
		}

		/// <summary></summary>
		protected void InitializeBins(int maxBinCount, int binCount)
		{
			BinCounts           = new List<long>(maxBinCount); // Preset the required capacity to improve memory management.
			BinValuesLowerLimit = new List<T>(   maxBinCount); // Preset the required capacity to improve memory management.
			BinValuesMidPoint   = new List<T>(   maxBinCount); // Preset the required capacity to improve memory management.
			BinValuesUpperLimit = new List<T>(   maxBinCount); // Preset the required capacity to improve memory management.

			for (int i = 0; i < binCount; i++)
			{
				BinCounts.Add(0);

				T lowerLimit, midPoint, upperLimit;
				CalculateBinValues(i, out lowerLimit, out midPoint, out upperLimit);
				BinValuesLowerLimit.Add(lowerLimit);
				BinValuesMidPoint  .Add(midPoint);
				BinValuesUpperLimit.Add(upperLimit);
			}
		}

		/// <summary>
		/// Gets the current number of bins.
		/// </summary>
		public virtual int BinCount
		{
			get { return (BinCounts.Count); }
		}

		/// <summary>
		/// Gets the current values of the bins.
		/// </summary>
		public virtual ReadOnlyCollection<T> ValuesLowerLimit
		{
			get { return (BinValuesLowerLimit.AsReadOnly()); }
		}

		/// <summary>
		/// Gets the current values of the bins.
		/// </summary>
		public virtual ReadOnlyCollection<T> ValuesMidPoint
		{
			get { return (BinValuesMidPoint.AsReadOnly()); }
		}

		/// <summary>
		/// Gets the current values of the bins.
		/// </summary>
		public virtual ReadOnlyCollection<T> ValuesUpperLimit
		{
			get { return (BinValuesUpperLimit.AsReadOnly()); }
		}

		/// <summary>
		/// Gets the current counts.
		/// </summary>
		public virtual ReadOnlyCollection<long> Counts
		{
			get { return (BinCounts.AsReadOnly()); }
		}

		/// <summary>
		/// Calculates the current size of a bin.
		/// </summary>
		protected abstract T CalculateBinSize(T min, T max, int binCount);

		/// <summary>
		/// Calculates the current number of bins.
		/// </summary>
		protected abstract int CalculateBinCount(T min, T max, T binSize);

		/// <summary>
		/// Calculates the values of the given bin.
		/// </summary>
		protected abstract void CalculateBinValues(int index, out T lowerLimit, out T midPoint, out T upperLimit);

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="item"/>.
		/// </summary>
		protected abstract int GetIndex(T item);

		/// <summary>
		/// Increments the corresponding bin.
		/// </summary>
		protected virtual void IncrementBin(int index)
		{
			BinCounts[index]++;
		}

		/// <summary>
		/// Increments the corresponding bin.
		/// </summary>
		protected virtual void IncrementBin(T item)
		{
			BinCounts[GetIndex(item)]++;
		}

		/// <summary>
		/// Adds an item to the histogram.
		/// </summary>
		public virtual void Add(T item)
		{
			if (AutoAdjust)
			{
				Items.Add(item);

				AdjustAsNeededAndIncrementBin(item);
			}
			else
			{
				if ((item.CompareTo(Min) >= 0) && (item.CompareTo(Max) <= 0))
				{
					Items.Add(item);

					if (BinSize.Equals(default(T))) // Just a single bin yet, 'BinSize' is yet default(T):
						IncrementBin(0);
					else
						IncrementBin(item);
				}
				else // Out-of-bounds:
				{
					if (OutOfBoundsBehavior == HistogramOutOfBoundsBehavior.Ignore) {
						// Ignore.
					}
					else {                  // HistogramOutOfBoundsBehavior.Throw)
						throw (new ArgumentOutOfRangeException("item", item, "The value of the item is outside 'Min'/'Max' and 'AutoAdjust' is 'false'!"));
					}
				}
			}
		}

		/// <summary>
		/// Adds items to the histogram.
		/// </summary>
		public virtual void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
				Add(item);
		}

		/// <summary>
		/// Gets the current number of items added by <see cref="Add(T)"/> and <see cref="AddRange(IEnumerable{T})"/>.
		/// </summary>
		public virtual int ItemCount
		{
			get { return (Items.Count); }
		}

		/// <summary>
		/// Lets the collection adjust as needed.
		/// </summary>
		/// <param name="item">
		/// The item just added that triggered calling this method.
		/// </param>
		protected virtual void AdjustAsNeededAndIncrementBin(T item)
		{
			if (Items.Count == 1) // Just a single bin yet, 'Min'/'Max'/'BinSize' are yet default(T):
			{
				Min = item;
				Max = item;
				InitializeBins(MaxBinCount, 1); // Initialize a first bin at the first value (given by Min/Max).
				IncrementBin(0);
			}
			else if ((item.CompareTo(Min) >= 0) && (item.CompareTo(Max) <= 0)) // Item is within bounds:
			{
				if (BinSize.Equals(default(T))) // Just a single bin yet, 'BinSize' is yet default(T):
				{
					IncrementBin(0);
				}
				else if (BinCount < MaxBinCount) // Adjusting is possible:
				{
					int index;
					if (IsWithinProximityOfBinValue(item, out index))
					{
						IncrementBin(index);
					}
					else if (IsWithinProximityOfPreviouslyContainedItem(item))
					{
						IncrementBin(item);
					}
					else
					{
						AdjustByBinSizeAndReincrementBins(BinCount + 1); // Gradually increase up to 'MaxBinCount' which will be handled below.
					}
				}
				else // Adjusting is no longer possible:
				{
					if (BinCount > MaxBinCount)
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'BinCount' must never exceed 'MaxBinCount'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

					IncrementBin(item);
				}
			}
			else // Item is outside bounds:
			{
				var previousMin = Min;
				var previousMax = Max;

				if (item.CompareTo(Min) < 0) { Min = item; }
				if (item.CompareTo(Max) > 0) { Max = item; }

				if (BinSize.Equals(default(T))) // Just a single bin yet, 'BinSize' is yet default(T):
				{
					AdjustByBinSizeAndReincrementBins(2); // Add new bin for new non-proximate item.
				}
				else
				{
					var binCountKeepingBinSize = CalculateBinCount(Min, Max, BinSize);
					if (binCountKeepingBinSize.CompareTo(MaxBinCount) <= 0)
						AdjustByBinCountKeeptingBinSizeAndIncrementBin(previousMin, previousMax, item);
					else
						AdjustByBinSizeAndReincrementBins(MaxBinCount);
				}
			}
		}

		/// <summary>
		/// Evaluates whether the value is within the proximity of a bin value.
		/// </summary>
		protected virtual bool IsWithinProximityOfBinValue(T value, out int index)
		{
			//                     value
			//                |- proximity -|
			// |-----    v    -----|-----    v    -----|-----    v    -----|
			// |----- (i - 1) -----|----- (  i  ) -----|----- (i + 1) -----|

			var i = GetIndex(value);

			T lower;
			T upper;
			GetProximity(value, out lower, out upper);

			index = (i);
			var binValue = BinValuesMidPoint[index];
			if ((binValue.CompareTo(lower) >= 0) && (binValue.CompareTo(upper) <= 0))
				return (true);

			if (i > 0)
			{
				index = (i - 1);
				binValue = BinValuesMidPoint[index];
				if ((binValue.CompareTo(lower) >= 0) && (binValue.CompareTo(upper) <= 0))
					return (true);
			}

			if (i < (BinCount - 1))
			{
				index = (i + 1);
				binValue = BinValuesMidPoint[index];
				if ((binValue.CompareTo(lower) >= 0) && (binValue.CompareTo(upper) <= 0))
					return (true);
			}

			return (false);
		}

		/// <summary>
		/// Evaluates whether the value is within the proximity of a previously contained item.
		/// </summary>
		protected virtual bool IsWithinProximityOfPreviouslyContainedItem(T value)
		{
			T lower;
			T upper;
			GetProximity(value, out lower, out upper);

			for (int i = 0; i < (ItemCount - 1); i++) // Don't include the just added item!
			{
				var item = Items[i];
				if ((item.CompareTo(lower) >= 0) && (item.CompareTo(upper) <= 0))
					return (true);
			}

			return (false);
		}

		/// <summary>
		/// Adjusts the histogram by increasing <see cref="BinCount"/>, keeping <see cref="BinSize"/>.
		/// </summary>
		protected virtual void AdjustByBinCountKeeptingBinSizeAndIncrementBin(T previousMin, T previousMax, T item)
		{
			int binCountToInsert = CalculateBinCount(Min, previousMin, BinSize);
			int binCountToAdd    = CalculateBinCount(previousMax, Max, BinSize);

			for (int i = (binCountToInsert - 1); i >= 0; i--)
			{
				BinCounts.Insert(0, 0);

				T lowerLimit, midPoint, upperLimit;
				CalculateBinValues(i, out lowerLimit, out midPoint, out upperLimit);
				BinValuesLowerLimit.Insert(0, lowerLimit);
				BinValuesMidPoint  .Insert(0, midPoint);
				BinValuesUpperLimit.Insert(0, upperLimit);
			}

			int offset = BinCount; // Already taking the above inserted bins into account.

			for (int i = offset; i < (offset + binCountToAdd); i++)
			{
				BinCounts.Add(0);

				T lowerLimit, midPoint, upperLimit;
				CalculateBinValues(i, out lowerLimit, out midPoint, out upperLimit);
				BinValuesLowerLimit.Add(lowerLimit);
				BinValuesMidPoint  .Add(midPoint);
				BinValuesUpperLimit.Add(upperLimit);
			}

			IncrementBin(item);
		}

		/// <summary>
		/// Adjusts the histogram by increasing <see cref="BinSize"/>, keeping <see cref="Min"/> and <see cref="Max"/>.
		/// </summary>
		protected virtual void AdjustByBinSizeAndReincrementBins(int binCount)
		{
			BinSize = CalculateBinSize(Min, Max, binCount);
			InitializeBins(binCount, binCount);

			foreach (var item in Items)
				IncrementBin(item);
		}

		/// <summary>
		/// Gets the proximity around a <paramref name="value"/> in the histogram,
		/// taking <see cref="MaxBinCount"/> into account.
		/// </summary>
		protected abstract void GetProximity(T value, out T lower, out T upper);
	}

	/// <summary>
	/// Collection of items of type <see cref="int"/> of a histogram.
	/// </summary>
	public class HistogramInt32 : Histogram<int>
	{
		/// <summary></summary>
		public const int DefaultMin = -100;

		/// <summary></summary>
		public const int DefaultMax = +100;

		/// <summary></summary>
		public const int DefaultBinCount = 11;

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramInt32"/> class with equally
		/// distributed bins, fixed to the given arguments.
		/// </summary>
		public HistogramInt32(int min = DefaultMin, int max = DefaultMax, int binCount = DefaultBinCount, HistogramOutOfBoundsBehavior outOfBoundsBehavior = DefaultOutOfBoundsBehavior)
			: base(min, max, binCount, outOfBoundsBehavior)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramInt32"/> class with equally
		/// distributed bins, automatically rearranging up to <paramref name="maxBinCount"/>.
		/// </summary>
		public HistogramInt32(int maxBinCount)
			: base(maxBinCount)
		{
		}

		/// <summary>
		/// Calculates the current size of a bin.
		/// </summary>
		protected override int CalculateBinSize(int min, int max, int binCount)
		{
			return ((max - min) / binCount);
		}

		/// <summary>
		/// Calculates the current number of bins.
		/// </summary>
		protected override int CalculateBinCount(int min, int max, int binSize)
		{
			return ((int)Math.Round((double)(max - min) / binSize));
		}

		/// <summary>
		/// Calculates the values of the given bin.
		/// </summary>
		protected override void CalculateBinValues(int index, out int lowerLimit, out int midPoint, out int upperLimit)
		{
			var half = (BinSize / 2);
			midPoint = (Min + (index * BinSize) + half); // Calculating based on mid-point in an attempt the reduce rounding errors.
			lowerLimit = (midPoint - half);
			upperLimit = (midPoint + half);
		}

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="item"/>.
		/// </summary>
		protected override int GetIndex(int item)
		{
			if (BinCount <= 1) // i.e. 'BinSize' may still be 0.0 = default(T).
				return (0);

			// Min                 item
			//                       v
			// |----- (i - 1) -----|----- (  i  ) -----|----- (i + 1) -----|

			var unlimited = ((int)((double)(item - Min) / BinSize));
			return (Int32Ex.Limit(unlimited, 0, (BinCount - 1)));
		}

		/// <summary>
		/// Gets the proximity around a <paramref name="value"/> in the histogram,
		/// taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetProximity(int value, out int lower, out int upper)
		{
			var half = (int)(Math.Round((double)(((Max - Min) / MaxBinCount) / 2)));
			lower = (value - half);
			upper = (value + half);
		}
	}

	/// <summary>
	/// Collection of items of type <see cref="long"/> of a histogram.
	/// </summary>
	public class HistogramInt64 : Histogram<long>
	{
		/// <summary></summary>
		public const long DefaultMin = -100;

		/// <summary></summary>
		public const long DefaultMax = +100;

		/// <summary></summary>
		public const int DefaultBinCount = 11;

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramInt64"/> class with equally
		/// distributed bins, fixed to the given arguments.
		/// </summary>
		public HistogramInt64(long min = DefaultMin, long max = DefaultMax, int binCount = DefaultBinCount, HistogramOutOfBoundsBehavior outOfBoundsBehavior = DefaultOutOfBoundsBehavior)
			: base(min, max, binCount, outOfBoundsBehavior)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramInt64"/> class with equally
		/// distributed bins, automatically rearranging up to <paramref name="maxBinCount"/>.
		/// </summary>
		public HistogramInt64(int maxBinCount)
			: base(maxBinCount)
		{
		}

		/// <summary>
		/// Calculates the current size of a bin.
		/// </summary>
		protected override long CalculateBinSize(long min, long max, int binCount)
		{
			return ((max - min) / binCount);
		}

		/// <summary>
		/// Calculates the current number of bins.
		/// </summary>
		protected override int CalculateBinCount(long min, long max, long binSize)
		{
			return ((int)Math.Round((double)(max - min) / binSize));
		}

		/// <summary>
		/// Calculates the values of the given bin.
		/// </summary>
		protected override void CalculateBinValues(int index, out long lowerLimit, out long midPoint, out long upperLimit)
		{
			var half = (BinSize / 2);
			midPoint = (Min + (index * BinSize) + half); // Calculating based on mid-point in an attempt the reduce rounding errors.
			lowerLimit = (midPoint - half);
			upperLimit = (midPoint + half);
		}

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="item"/>.
		/// </summary>
		protected override int GetIndex(long item)
		{
			if (BinCount <= 1) // i.e. 'BinSize' may still be 0.0 = default(T).
				return (0);

			// Min                 item
			//                       v
			// |----- (i - 1) -----|----- (  i  ) -----|----- (i + 1) -----|

			var unlimited = ((int)((double)(item - Min) / BinSize));
			return (Int32Ex.Limit(unlimited, 0, (BinCount - 1)));
		}

		/// <summary>
		/// Gets the proximity around a <paramref name="value"/> in the histogram,
		/// taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetProximity(long value, out long lower, out long upper)
		{
			var half = (long)(Math.Round((double)(((Max - Min) / MaxBinCount) / 2)));
			lower = (value - half);
			upper = (value + half);
		}
	}

	/// <summary>
	/// Collection of items of type <see cref="double"/> of a histogram.
	/// </summary>
	public class HistogramDouble : Histogram<double>
	{
		/// <summary></summary>
		public const double DefaultMin = -100.0;

		/// <summary></summary>
		public const double DefaultMax = +100.0;

		/// <summary></summary>
		public const int DefaultBinCount = 11;

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramDouble"/> class with equally
		/// distributed bins, fixed to the given arguments.
		/// </summary>
		public HistogramDouble(double min = DefaultMin, double max = DefaultMax, int binCount = DefaultBinCount, HistogramOutOfBoundsBehavior outOfBoundsBehavior = DefaultOutOfBoundsBehavior)
			: base(min, max, binCount, outOfBoundsBehavior)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramDouble"/> class with equally
		/// distributed bins, automatically rearranging up to <paramref name="maxBinCount"/>.
		/// </summary>
		public HistogramDouble(int maxBinCount)
			: base(maxBinCount)
		{
		}

		/// <summary>
		/// Calculates the current size of a bin.
		/// </summary>
		protected override double CalculateBinSize(double min, double max, int binCount)
		{
			return ((max - min) / binCount);
		}

		/// <summary>
		/// Calculates the current number of bins.
		/// </summary>
		protected override int CalculateBinCount(double min, double max, double binSize)
		{
			return ((int)Math.Round((max - min) / binSize));
		}

		/// <summary>
		/// Calculates the values of the given bin.
		/// </summary>
		protected override void CalculateBinValues(int index, out double lowerLimit, out double midPoint, out double upperLimit)
		{
			var half = (BinSize / 2);
			midPoint = (Min + (index * BinSize) + half); // Calculating based on mid-point in an attempt the reduce rounding errors.
			lowerLimit = (midPoint - half);
			upperLimit = (midPoint + half);
		}

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="item"/>.
		/// </summary>
		protected override int GetIndex(double item)
		{
			if (BinCount <= 1) // i.e. 'BinSize' may still be 0.0 = default(T).
				return (0);

			// Min                 item
			//                       v
			// |----- (i - 1) -----|----- (  i  ) -----|----- (i + 1) -----|

			var unlimited = ((int)((item - Min) / BinSize));
			return (Int32Ex.Limit(unlimited, 0, (BinCount - 1)));
		}

		/// <summary>
		/// Gets the proximity around a <paramref name="value"/> in the histogram,
		/// taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetProximity(double value, out double lower, out double upper)
		{
			var half = (((Max - Min) / MaxBinCount) / 2);
			lower = (value - half);
			upper = (value + half);
		}
	}

	/// <summary>
	/// Collection of items of type <see cref="decimal"/> of a histogram.
	/// </summary>
	public class HistogramDecimal : Histogram<decimal>
	{
		/// <summary></summary>
		public const decimal DefaultMin = -100.0m;

		/// <summary></summary>
		public const decimal DefaultMax = +100.0m;

		/// <summary></summary>
		public const int DefaultBinCount = 11;

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramDecimal"/> class with equally
		/// distributed bins, fixed to the given arguments.
		/// </summary>
		public HistogramDecimal(decimal min = DefaultMin, decimal max = DefaultMax, int binCount = DefaultBinCount, HistogramOutOfBoundsBehavior outOfBoundsBehavior = DefaultOutOfBoundsBehavior)
			: base(min, max, binCount, outOfBoundsBehavior)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HistogramDecimal"/> class with equally
		/// distributed bins, automatically rearranging up to <paramref name="maxBinCount"/>.
		/// </summary>
		public HistogramDecimal(int maxBinCount)
			: base(maxBinCount)
		{
		}

		/// <summary>
		/// Calculates the current size of a bin.
		/// </summary>
		protected override decimal CalculateBinSize(decimal min, decimal max, int binCount)
		{
			return ((max - min) / binCount);
		}

		/// <summary>
		/// Calculates the current number of bins.
		/// </summary>
		protected override int CalculateBinCount(decimal min, decimal max, decimal binSize)
		{
			return ((int)Math.Round((max - min) / binSize));
		}

		/// <summary>
		/// Calculates the values of the given bin.
		/// </summary>
		protected override void CalculateBinValues(int index, out decimal lowerLimit, out decimal midPoint, out decimal upperLimit)
		{
			var half = (BinSize / 2);
			midPoint = (Min + (index * BinSize) + half); // Calculating based on mid-point in an attempt the reduce rounding errors.
			lowerLimit = (midPoint - half);
			upperLimit = (midPoint + half);
		}

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="item"/>.
		/// </summary>
		protected override int GetIndex(decimal item)
		{
			if (BinCount <= 1) // i.e. 'BinSize' may still be 0.0 = default(T).
				return (0);

			// Min                 item
			//                       v
			// |----- (i - 1) -----|----- (  i  ) -----|----- (i + 1) -----|

			var unlimited = ((int)((item - Min) / BinSize));
			return (Int32Ex.Limit(unlimited, 0, (BinCount - 1)));
		}

		/// <summary>
		/// Gets the proximity around a <paramref name="value"/> in the histogram,
		/// taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetProximity(decimal value, out decimal lower, out decimal upper)
		{
			var half = (((Max - Min) / MaxBinCount) / 2);
			lower = (value - half);
			upper = (value + half);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
