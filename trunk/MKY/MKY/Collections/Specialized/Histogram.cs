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
		/// in case <see cref="Histogram{T}.AutoRearrange"/> is <c>false</c>.
		/// </summary>
		Ignore,

		/// <summary>
		/// Throws an excetions for values outside <see cref="Histogram{T}.Min"/> and <see cref="Histogram{T}.Max"/>
		/// in case <see cref="Histogram{T}.AutoRearrange"/> is <c>false</c>.
		/// </summary>
		Throw
	}

	/// <summary>
	/// Collection of values of a histogram.
	/// </summary>
	/// <typeparam name="T">The type of the values of the histogram</typeparam>
	public abstract class Histogram<T> where T : IComparable<T>
	{
		/// <summary></summary>
		public const HistogramOutOfBoundsBehavior DefaultOutOfBoundsBehavior = HistogramOutOfBoundsBehavior.Ignore;

		/// <summary></summary>
		public readonly int MaxBinCount; // Must be initialized with up to 'int.MaxValue'.

		/// <summary></summary>
		public readonly bool AutoRearrange; // = false;

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
			AutoRearrange = false;
			OutOfBoundsBehavior = outOfBoundsBehavior;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class with equally
		/// distributed bins, automatically rearranging up to <paramref name="maxBinCount"/>.
		/// </summary>
		public Histogram(int maxBinCount)
		{
		////Min = default(T);
		////Max = default(T);
			MaxBinCount = maxBinCount;
		////BinSize = default(T);
			InitializeBins(maxBinCount);
			AutoRearrange = true;
		}

		/// <summary></summary>
		protected void InitializeBins(int maxBinCount, int binCount = 0)
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

			// Rearrange if allowed:
			if (AutoRearrange)
			{
				Values.Add(value);
				Rearrange();

				if ((BinCount > 0) && (value.CompareTo(Min) >= 0) && (value.CompareTo(Max) <= 0))
				{
					IncrementBin(value);
					return;
				}
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
		/// Rearranges the collection up to <see cref="MaxBinCount"/>.
		/// Needed when <see cref="AutoRearrange"/> is <c>true</c>.
		/// </summary>
		protected virtual void Rearrange()
		{
			T min;
			T max;
			GetMinMaxFromValues(out min, out max);

			if (BinCount <= 0) // No bin yet, i.e. 'BinSize' is yet undefined:
			{
				if (Values.Count != 1)
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Exactly one value must be contained when Rearrange() can be called under the current condition!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				Min = min;
				Max = max;

				BinSize = CalculateBinSize(Min, Max, MaxBinCount);
				InitializeBins(MaxBinCount, 1);
				IncrementBin(Values[0]);
			}
			else if ((min.CompareTo(Min) < 0) || (max.CompareTo(Max) > 0)) // Either has changed => Rearrange:
			{
				var previousMin = Min;
				var previousMax = Max;

				Min = min;
				Max = max;

				var binCountWithCurrentBinSize = CalculateBinCount(Max, Min, BinSize);
				if (binCountWithCurrentBinSize.CompareTo(MaxBinCount) <= 0)
				{
					int binCountToInsert = CalculateBinCount(previousMin, min, BinSize);
					int binCountToAdd    = CalculateBinCount(max, previousMax, BinSize);

					for (int i = 0; i < binCountToInsert; i++)
						Bins.Insert(0, 0);

					for (int i = 0; i < binCountToAdd; i++)
						Bins.Add(0);
				}
				else // Rearrange to 'MaxBinCount':
				{
					BinSize = CalculateBinSize(Min, Max, MaxBinCount);
					InitializeBins(MaxBinCount, MaxBinCount);

					foreach (var value in Values)
						IncrementBin(value);
				}
			}
		}

		/// <summary>
		/// Get the minimum and maximum within <see cref="Values"/>.
		/// </summary>
		protected abstract void GetMinMaxFromValues(out T min, out T max);

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
		/// Get the minimum and maximum within <see cref="Histogram{T}.Values"/>.
		/// </summary>
		protected override void GetMinMaxFromValues(out int min, out int max)
		{
			Int32Ex.GetMinMax(Values, out min, out max);
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
		/// Get the minimum and maximum within <see cref="Histogram{T}.Values"/>.
		/// </summary>
		protected override void GetMinMaxFromValues(out long min, out long max)
		{
			Int64Ex.GetMinMax(Values, out min, out max);
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
		/// Get the minimum and maximum within <see cref="Histogram{T}.Values"/>.
		/// </summary>
		protected override void GetMinMaxFromValues(out double min, out double max)
		{
			DoubleEx.GetMinMax(Values, out min, out max);
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
		/// Get the minimum and maximum within <see cref="Histogram{T}.Values"/>.
		/// </summary>
		protected override void GetMinMaxFromValues(out decimal min, out decimal max)
		{
			DecimalEx.GetMinMax(Values, out min, out max);
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
