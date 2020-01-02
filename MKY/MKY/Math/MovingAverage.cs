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

namespace MKY.Math
{
	/// <summary>
	/// Simple moving average based on <see cref="Queue{T}"/>.
	/// </summary>
	public abstract class MovingAverage<T>
	{
		/// <summary>
		/// The length of the average.
		/// </summary>
		public readonly int Length;

		private Queue<T> queue; // = null;
		private bool valueHasToBeCalculated; // = false;
		private T valueCache; // = default(T);

		/// <summary>
		/// Initializes a new instance of the <see cref="MovingAverage{T}"/> class.
		/// </summary>
		/// <param name="length">The length of the average.</param>
		public MovingAverage(int length)
		{
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

				this.valueCache = default(T);
				foreach (var item in this.queue.ToArray())
					this.valueCache = Add(this.valueCache, item);
			}

			return (this.valueCache);
		}

		/// <summary>
		/// Adds two values; must be implemented by the deriving class.
		/// </summary>
		/// <remarks>
		/// Required to not have to use the dynamic keyword (which would require a reference to the
		/// 'Microsoft.CSharp' assembly) nor <see cref="System.Linq.Expressions.Expression"/> (which
		/// is much less performant).
		/// See e.g. https://stackoverflow.com/questions/8122611/c-sharp-adding-two-generic-values
		/// for background and discussion.
		/// </remarks>
		public abstract T Add(T a, T b);
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
		public override int Add(int a, int b)
		{
			return (a + b);
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
		public override double Add(double a, double b)
		{
			return (a + b);
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
		public override TimeSpan Add(TimeSpan a, TimeSpan b)
		{
			return (a + b);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
