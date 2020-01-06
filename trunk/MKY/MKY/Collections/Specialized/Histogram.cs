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
		/// Ignores values outside <see cref="Histogram{T}.Min"/> and <see cref="Histogram{T}.Max"/>
		/// in case <see cref="Histogram{T}.AutoGrow"/> is <c>false</c>.
		/// </summary>
		Ignore,

		/// <summary>
		/// Throws an excetions for values outside <see cref="Histogram{T}.Min"/> and <see cref="Histogram{T}.Max"/>
		/// in case <see cref="Histogram{T}.AutoGrow"/> is <c>false</c>.
		/// </summary>
		Throw
	}

	/// <summary>
	/// Collection of values of a histogram.
	/// </summary>
	/// <typeparam name="T">The type of the values of the histogram</typeparam>
	public abstract class Histogram<T> where T : IComparable<T>, IEquatable<T>
	{
		/// <summary></summary>
		public const HistogramOutOfBoundsBehavior DefaultOutOfBoundsBehavior = HistogramOutOfBoundsBehavior.Ignore;

		/// <summary></summary>
		public readonly int MaxBinCount; // Must be initialized with up to 'int.MaxValue'.

		/// <summary></summary>
		public readonly bool AutoGrow; // = false;

		/// <summary></summary>
		public readonly HistogramOutOfBoundsBehavior OutOfBoundsBehavior; // = Ignore;

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
		public Histogram(T min, T max, int binCount, HistogramOutOfBoundsBehavior outOfBoundsBehavior = DefaultOutOfBoundsBehavior)
		{
			Min = min;
			Max = max;
			MaxBinCount = binCount;
			BinSize = CalculateBinSize(min, max, binCount);
			InitializeBins(binCount, binCount);
			AutoGrow = false;
			OutOfBoundsBehavior = outOfBoundsBehavior;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class with equally
		/// distributed bins, automatically growing up to <paramref name="maxBinCount"/>.
		/// </summary>
		public Histogram(int maxBinCount)
		{
		////Min = default(T);
		////Max = default(T);
			MaxBinCount = maxBinCount;
		////BinSize = default(T);
			InitializeBins(maxBinCount, 1);
			AutoGrow = true;
		}

		/// <summary></summary>
		protected void InitializeBins(int maxBinCount, int binCount)
		{
			Bins = new List<long>(maxBinCount); // Preset the required capacity to improve memory management.
			for (int i = 0; i < binCount; i++)
				Bins.Add(0);
		}

		/// <summary>
		/// Gets the current number of bins.
		/// </summary>
		public virtual int BinCount
		{
			get { return (Bins.Count); }
		}

		/// <summary>
		/// Gets the current values of the bins.
		/// </summary>
		public virtual ReadOnlyCollection<T> BinValues
		{
			get
			{
				List<T> bins = new List<T>(BinCount);

				for (int i = 0; i < bins.Capacity; i++)
					bins.Add(MulAdd(i, BinSize, Min));
					        //// ((i * BinSize) + Min);
				return (bins.AsReadOnly());
			}
		}

		/// <summary>
		/// Gets the current counts.
		/// </summary>
		public virtual ReadOnlyCollection<long> Counts
		{
			get { return (Bins.AsReadOnly()); }
		}

		/// <summary>
		/// Calculates the current size of a bin.
		/// </summary>
		protected virtual T CalculateBinSize(T min, T max, int binCount)
		{            // (max - min) / binCount)
			return (SubDiv(max, min, binCount));
		}

		/// <summary>
		/// Calculates the current size of a bin.
		/// </summary>
		protected virtual int CalculateBinCount(T min, T max, T binSize)
		{                 // (max - min) / binSize)
			return (SubDivRound(max, min, binSize));
		}

		/// <summary>
		/// Gets the index of the bin corresponding to <paramref name="value"/>.
		/// </summary>
		protected virtual int GetIndex(T value)
		{                 // (value - Min) / BinSize)
			return (SubDivRound(value, Min, BinSize));
		}

		/// <summary>
		/// Increments the corresponding bin.
		/// </summary>
		protected virtual void IncrementBin(int index)
		{
			Bins[index]++;
		}

		/// <summary>
		/// Increments the corresponding bin.
		/// </summary>
		protected virtual void IncrementBin(T value)
		{
			Bins[GetIndex(value)]++;
		}

		/// <summary>
		/// Adds a value to the histogram.
		/// </summary>
		public virtual void Add(T value)
		{
			// Easy case shall be fast:
			if ((BinCount > 0) && (value.CompareTo(Min) >= 0) && (value.CompareTo(Max) <= 0))
			{
				Values.Add(value);
				IncrementBin(value);
				return;
			}

			// Grow if allowed:
			if (AutoGrow)
			{
				Values.Add(value);
				GrowAsNeededAndIncrementBin(value);
			}

			// Out-of-bounds:
			if (OutOfBoundsBehavior == HistogramOutOfBoundsBehavior.Ignore) {
				// Ignore.
			}
			else {                  // HistogramOutOfBoundsBehavior.Throw)
				throw (new ArgumentOutOfRangeException("item", value, "The value of the item is outside 'Min'/'Max' and 'AutoRearrange' is 'false'!"));
			}
		}

		/// <summary>
		/// Adds values to the histogram.
		/// </summary>
		public virtual void AddRange(IEnumerable<T> values)
		{
			foreach (var value in values)
				Add(value);
		}

		/// <summary>
		/// Gets the current number of values added by <see cref="Add(T)"/> and <see cref="AddRange(IEnumerable{T})"/>.
		/// </summary>
		public virtual int ValueCount
		{
			get { return (Values.Count); }
		}

		/// <summary>
		/// Lets the collection grow up to <see cref="MaxBinCount"/>.
		/// Needed when <see cref="AutoGrow"/> is <c>true</c>.
		/// </summary>
		/// <param name="value">
		/// The value just added that triggered calling this method.
		/// </param>
		protected virtual void GrowAsNeededAndIncrementBin(T value)
		{
			if (Values.Count == 1) // Just a single bin yet, 'Min'/'Max'/'BinSize' are yet default(T):
			{
				Min = Values[0];
				Max = Values[0];
				IncrementBin(0);
			}
			else if ((value.CompareTo(Min) >= 0) || (value.CompareTo(Max) <= 0)) // Value is within bounds:
			{
				if (BinSize.Equals(default(T))) // Just a single bin yet, 'BinSize' is yet default(T):
				{
					IncrementBin(0);
				}
				else
				{
					T lowerLimit;
					T upperLimit;
					GetValueProximity(value, out lowerLimit, out upperLimit);

					var valueIsWithinProximityOfAlreadyContainedValue = false;
					for (int i = 0; i < (Values.Count - 1); i++)
					{
						if ((value.CompareTo(lowerLimit) >= 0) || (value.CompareTo(upperLimit) <= 0))
						{
							valueIsWithinProximityOfAlreadyContainedValue = true;
							break;
						}
					}

					if (valueIsWithinProximityOfAlreadyContainedValue)
						IncrementBin(value);
					else if (BinCount < MaxBinCount)
						GrowByBinSizeAndReincrementBins(BinCount + 1); // Gradually increase...
					else
						GrowByBinSizeAndReincrementBins(MaxBinCount); // ...until 'MaxBinCount'.
				}
			}
			else // Value is outside bounds:
			{
				var previousMin = Min;
				var previousMax = Max;

				if (value.CompareTo(Min) < 0) { Min = value; }
				if (value.CompareTo(Max) > 0) { Max = value; }

				var binCountKeepingBinSize = CalculateBinCount(Max, Min, BinSize);
				if (binCountKeepingBinSize.CompareTo(MaxBinCount) <= 0)
					GrowByBinCountAndIncrementBin(previousMin, previousMax, value);
				else
					GrowByBinSizeAndReincrementBins(MaxBinCount);
			}
		}

		/// <summary>
		/// Grows the histogram by increasing <see cref="BinCount"/>, keeping <see cref="BinSize"/>.
		/// </summary>
		protected virtual void GrowByBinCountAndIncrementBin(T previousMin, T previousMax, T value)
		{
			int binCountToInsert = CalculateBinCount(previousMin, Min, BinSize);
			int binCountToAdd    = CalculateBinCount(Max, previousMax, BinSize);

			for (int i = 0; i < binCountToInsert; i++)
				Bins.Insert(0, 0);

			for (int i = 0; i < binCountToAdd; i++)
				Bins.Add(0);

			IncrementBin(value);
		}

		/// <summary>
		/// Grows the histogram by increasing <see cref="BinSize"/>, keeping <see cref="Min"/> and <see cref="Max"/>.
		/// </summary>
		protected virtual void GrowByBinSizeAndReincrementBins(int binCount)
		{
			BinSize = CalculateBinSize(Min, Max, MaxBinCount);
			InitializeBins(MaxBinCount, MaxBinCount);

			foreach (var v in Values)
				IncrementBin(v);
		}

		/// <summary>
		/// Gets the proximity around <paramref name="value"/>, taking <see cref="MaxBinCount"/> into account.
		/// </summary>
		protected abstract void GetValueProximity(T value, out T lower, out T upper);

		/// <summary>
		/// Evaluates <c>((a * b) + c)</c>.
		/// </summary>
		protected abstract T MulAdd(int a, T b, T c);

		/// <summary>
		/// Evaluates <c>((a - b) / c)</c>.
		/// </summary>
		protected abstract T SubDiv(T a, T b, int c);

		/// <summary>
		/// Evaluates <c>(int)(Math.Round((a - b) / c)))</c>.
		/// </summary>
		protected abstract int SubDivRound(T a, T b, T c);
	}

	/// <summary>
	/// Collection of values of type <see cref="int"/> of a histogram.
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
		/// Gets the proximity around <paramref name="value"/>, taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetValueProximity(int value, out int lower, out int upper)
		{
			var half = (int)(Math.Round((double)(((Max - Min) / MaxBinCount) / 2)));
			lower = (value - half);
			upper = (value + half);
		}

		/// <summary>
		/// Evaluates <c>((a * b) + c)</c>.
		/// </summary>
		protected override int MulAdd(int a, int b, int c)
		{
			return ((a * b) + c);
		}

		/// <summary>
		/// Evaluates <c>((a - b) / c)</c>.
		/// </summary>
		protected override int SubDiv(int a, int b, int c)
		{
			return ((a - b) / c);
		}

		/// <summary>
		/// Evaluates <c>(int)(Math.Round((a - b) / c)))</c>.
		/// </summary>
		protected override int SubDivRound(int a, int b, int c)
		{
			return ((int)(Math.Round((double)(a - b) / c)));
		}
	}

	/// <summary>
	/// Collection of values of type <see cref="long"/> of a histogram.
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
		/// Gets the proximity around <paramref name="value"/>, taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetValueProximity(long value, out long lower, out long upper)
		{
			var half = (long)(Math.Round((double)(((Max - Min) / MaxBinCount) / 2)));
			lower = (value - half);
			upper = (value + half);
		}

		/// <summary>
		/// Evaluates <c>((a * b) + c)</c>.
		/// </summary>
		protected override long MulAdd(int a, long b, long c)
		{
			return ((a * b) + c);
		}

		/// <summary>
		/// Evaluates <c>((a - b) / c)</c>.
		/// </summary>
		protected override long SubDiv(long a, long b, int c)
		{
			return ((a - b) / c);
		}

		/// <summary>
		/// Evaluates <c>(int)(Math.Round((a - b) / c)))</c>.
		/// </summary>
		protected override int SubDivRound(long a, long b, long c)
		{
			return ((int)(Math.Round((double)(a - b) / c)));
		}
	}

	/// <summary>
	/// Collection of values of type <see cref="double"/> of a histogram.
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
		/// Gets the proximity around <paramref name="value"/>, taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetValueProximity(double value, out double lower, out double upper)
		{
			var half = (((Max - Min) / MaxBinCount) / 2);
			lower = (value - half);
			upper = (value + half);
		}

		/// <summary>
		/// Evaluates <c>((a * b) + c)</c>.
		/// </summary>
		protected override double MulAdd(int a, double b, double c)
		{
			return ((a * b) + c);
		}

		/// <summary>
		/// Evaluates <c>((a - b) / c)</c>.
		/// </summary>
		protected override double SubDiv(double a, double b, int c)
		{
			return ((a - b) / c);
		}

		/// <summary>
		/// Evaluates <c>(int)(Math.Round((a - b) / c)))</c>.
		/// </summary>
		protected override int SubDivRound(double a, double b, double c)
		{
			return ((int)(Math.Round((a - b) / c)));
		}
	}

	/// <summary>
	/// Collection of values of type <see cref="decimal"/> of a histogram.
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
		/// Gets the proximity around <paramref name="value"/>, taking <see cref="Histogram{T}.MaxBinCount"/> into account.
		/// </summary>
		protected override void GetValueProximity(decimal value, out decimal lower, out decimal upper)
		{
			var half = (((Max - Min) / MaxBinCount) / 2);
			lower = (value - half);
			upper = (value + half);
		}

		/// <summary>
		/// Evaluates <c>((a * b) + c)</c>.
		/// </summary>
		protected override decimal MulAdd(int a, decimal b, decimal c)
		{
			return ((a * b) + c);
		}

		/// <summary>
		/// Evaluates <c>((a - b) / c)</c>.
		/// </summary>
		protected override decimal SubDiv(decimal a, decimal b, int c)
		{
			return ((a - b) / c);
		}

		/// <summary>
		/// Evaluates <c>(int)(Math.Round((a - b) / c)))</c>.
		/// </summary>
		protected override int SubDivRound(decimal a, decimal b, decimal c)
		{
			return ((int)(Math.Round((a - b) / c)));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
