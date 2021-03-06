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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Collections.Specialized
{
	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of the items of the collection.</typeparam>
	public abstract class TimedMovingAverage<T>
	{
		/// <summary>
		/// The interval of the average in milliseconds.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ms", Justification = "Emphasize difference among 'ms' and 'ticks'.")]
		public int IntervalMs { get; protected set; } // = 0;

		/// <summary>
		/// The interval of the average in ticks.
		/// </summary>
		public long IntervalTicks { get; protected set; } // = 0;

		private Queue<Tuple<T, long>> queue; // = null;
		private bool valueHasToBeCalculated; // = false;
		private T valueCache; // = default(T);

		/// <summary>
		/// Initializes a new instance of the <see cref="TimedMovingAverage{T}"/> class.
		/// </summary>
		/// <param name="intervalMs">The interval of the average in milliseconds.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="intervalMs"/> is equal or less than 0 ms.</exception>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ms", Justification = "Emphasize difference among 'ms' and 'ticks'.")]
		protected TimedMovingAverage(int intervalMs)
		{
			if (intervalMs <= 0)
				throw (new ArgumentOutOfRangeException("intervalMs", intervalMs, MessageHelper.InvalidExecutionPreamble + "Interval must be at least 1 ms!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			IntervalMs = intervalMs;
			IntervalTicks = TimeSpanEx.TimeToTicks(intervalMs);

			this.queue = new Queue<Tuple<T, long>>();
		}

		/// <summary>
		/// The current value of the moving average.
		/// </summary>
		public T Value
		{
			get
			{
				if (this.valueHasToBeCalculated)
					return (Calculate());
				else
					return (this.valueCache);
			}
		}

		/// <summary>
		/// Enqueues a value.
		/// </summary>
		public virtual void Enqueue(T item)
		{
			Enqueue(item, DateTime.Now.Ticks);
		}

		/// <summary>
		/// Enqueues a value.
		/// </summary>
		public virtual void Enqueue(T item, DateTime itemTimeStamp)
		{
			Enqueue(item, itemTimeStamp.Ticks);
		}

		/// <summary>
		/// Enqueues a value.
		/// </summary>
		public virtual void Enqueue(T item, long itemTicks)
		{
			var tickLimit = (itemTicks - IntervalTicks);
			while ((this.queue.Count > 0) && (this.queue.Peek().Item2 < tickLimit))
				this.queue.Dequeue();

			this.queue.Enqueue(new Tuple<T, long>(item, itemTicks));
			this.valueHasToBeCalculated = true;
		}

		/// <summary>
		/// Enqueues a value and immediately calculates the average.
		/// </summary>
		public virtual T EnqueueAndCalculate(T item)
		{
			Enqueue(item);
			return (Calculate());
		}

		/// <summary>
		/// Calculates the average.
		/// </summary>
		public virtual T Calculate()
		{
			if (this.valueHasToBeCalculated)
			{
				this.valueHasToBeCalculated = false;

				this.valueCache = Avg(this.queue.ToArray());
			}

			return (this.valueCache);
		}

		/// <summary>
		/// Averages the given items; must be implemented by the deriving class.
		/// </summary>
		/// <remarks>
		/// Required to not have to use the dynamic keyword (which would require a reference to the
		/// "Microsoft.CSharp" assembly) nor <see cref="System.Linq.Expressions.Expression"/> (which
		/// has much less performance).
		/// See e.g. https://stackoverflow.com/questions/8122611/c-sharp-adding-two-generic-values
		/// for background and discussion.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Avg", Justification = "Short form of operation, same as e.g. 'Add' or 'Div'.")]
		public abstract T Avg(Tuple<T, long>[] items);
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class TimedMovingAverageInt32 : TimedMovingAverage<int>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimedMovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public TimedMovingAverageInt32(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override int Avg(Tuple<int, long>[] items)
		{
			int sum = 0;
			foreach (var item in items)
				sum += item.Item1;

			int avg = (int)(Math.Round((double)sum / items.Length));
			return (avg);
		}
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class TimedMovingAverageInt64 : TimedMovingAverage<long>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimedMovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public TimedMovingAverageInt64(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override long Avg(Tuple<long, long>[] items)
		{
			long sum = 0;
			foreach (var item in items)
				sum += item.Item1;

			long avg = (long)(Math.Round((double)sum / items.Length));
			return (avg);
		}
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class TimedMovingAverageDouble : TimedMovingAverage<double>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimedMovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public TimedMovingAverageDouble(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override double Avg(Tuple<double, long>[] items)
		{
			double sum = 0.0;
			foreach (var item in items)
				sum += item.Item1;

			return (sum / items.Length);
		}
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class TimedMovingAverageTimeSpan : TimedMovingAverage<TimeSpan>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimedMovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public TimedMovingAverageTimeSpan(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override TimeSpan Avg(Tuple<TimeSpan, long>[] items)
		{
			TimeSpan sum = TimeSpan.Zero;
			foreach (var item in items)
				sum += item.Item1;

			TimeSpan avg = TimeSpan.FromMilliseconds(Math.Round(sum.TotalMilliseconds / items.Length));
			return (avg);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
