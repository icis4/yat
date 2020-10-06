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
using System.Diagnostics.CodeAnalysis;

namespace MKY.Collections.Specialized
{
	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of the items of the collection.</typeparam>
	public abstract class MovingAverage<T>
	{
		/// <summary>
		/// The length of the average.
		/// </summary>
		public int Length { get; protected set; } // = 0;

		private Queue<T> queue; // = null;
		private bool valueHasToBeCalculated; // = false;
		private T valueCache; // = default(T);

		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is equal or less than 0.</exception>
		protected MovingAverage(int length)
		{
			if (length <= 0)
				throw (new ArgumentOutOfRangeException("length", length, MessageHelper.InvalidExecutionPreamble + "Length must be at least 1!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			Length = length;

			this.queue = new Queue<T>(Length);
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
			while (this.queue.Count >= Length)
				this.queue.Dequeue();

			this.queue.Enqueue(item);
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
		/// 'Microsoft.CSharp' assembly) nor <see cref="System.Linq.Expressions.Expression"/> (which
		/// has much less performance).
		/// See e.g. https://stackoverflow.com/questions/8122611/c-sharp-adding-two-generic-values
		/// for background and discussion.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Avg", Justification = "Short form of operation, same as e.g. 'Add' or 'Div'.")]
		public abstract T Avg(T[] items);
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class MovingAverageInt32 : MovingAverage<int>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public MovingAverageInt32(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override int Avg(int[] items)
		{
			int sum = 0;
			foreach (var item in items)
				sum += item;

			int avg = (int)(Math.Round((double)sum / items.Length));
			return (avg);
		}
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class MovingAverageInt64 : MovingAverage<long>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public MovingAverageInt64(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override long Avg(long[] items)
		{
			long sum = 0;
			foreach (var item in items)
				sum += item;

			long avg = (long)(Math.Round((double)sum / items.Length));
			return (avg);
		}
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class MovingAverageDouble : MovingAverage<double>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public MovingAverageDouble(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override double Avg(double[] items)
		{
			double sum = 0.0;
			foreach (var item in items)
				sum += item;

			return (sum / items.Length);
		}
	}

	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public class MovingAverageTimeSpan : MovingAverage<TimeSpan>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public MovingAverageTimeSpan(int length)
			: base(length)
		{
		}

		/// <summary>
		/// Adds two values.
		/// </summary>
		public override TimeSpan Avg(TimeSpan[] items)
		{
			TimeSpan sum = TimeSpan.Zero;
			foreach (var item in items)
				sum += item;

			TimeSpan avg = TimeSpan.FromMilliseconds(Math.Round(sum.TotalMilliseconds / items.Length));
			return (avg);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
