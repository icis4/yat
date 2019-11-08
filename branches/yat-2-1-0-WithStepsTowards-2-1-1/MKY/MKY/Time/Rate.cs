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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

namespace MKY.Time
{
	/// <summary></summary>
	public class Rate
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int interval;
		private int window;

		private Queue<TimeStampItem<int>> queue;
		private int value;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Rate()
			: this(1000, 5000)
		{
		}

		/// <summary></summary>
		public Rate(int interval)
			: this(interval, interval)
		{
		}

		/// <summary></summary>
		public Rate(int interval, int window)
		{
			this.interval = interval;
			this.window   = window;

			this.queue = new Queue<TimeStampItem<int>>(window); // Preset the assumed capacity to improve memory management.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// The interval to evaluate partial values of the rate.
		/// </summary>
		public int Interval
		{
			get { return (this.interval); }
			set { this.interval = value;  }
		}

		/// <summary>
		/// The total window to evaluate the rate.
		/// </summary>
		public int Window
		{
			get { return (this.window); }
			set { this.window = value;  }
		}

		/// <summary>
		/// The resulting rate value, in items per millisecond.
		/// </summary>
		public int Value
		{
			get { return (this.value); }
			set { this.value = value;  }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool Update(int value)
		{
			AddValueToQueue(value);
			RemoveObsoleteFromQueue();
			return (CalculateValueFromQueue(value));
		}

		/// <summary></summary>
		public virtual bool Update(DateTime endOfWindow)
		{
			RemoveObsoleteFromQueue(endOfWindow);
			return (CalculateValueFromQueue());
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			ClearQueue();
			CalculateValueFromQueue();
		}

		#region Methods > Private
		//------------------------------------------------------------------------------------------
		// Methods > Private
		//------------------------------------------------------------------------------------------

		private void ClearQueue()
		{
			lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
				this.queue.Clear();
		}

		private void AddValueToQueue(int value)
		{
			lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
				this.queue.Enqueue(new TimeStampItem<int>(value));
		}

		private void RemoveObsoleteFromQueue()
		{
			RemoveObsoleteFromQueue(DateTime.Now);
		}

		private void RemoveObsoleteFromQueue(DateTime endOfWindow)
		{
			DateTime beginningOfWindow = (endOfWindow - TimeSpan.FromMilliseconds(this.window));

			lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
			{
				while (this.queue.Count > 0)
				{
					var tsi = this.queue.Peek();
					if (tsi.TimeStamp < beginningOfWindow)
						this.queue.Dequeue();
					else
						break; // Front-most item is within window.
				}
			}
		}

		private bool CalculateValueFromQueue(int value = 0)
		{
			int oldValue = this.value;
			int newValue = 0;

			// If value was 0 before, only consider the current value:
			if (oldValue <= 0)
			{
				newValue = value;
			}
			else
			{
				unchecked
				{
					// Count number of items within each interval:
					int numberOfIntervals = (this.window / this.interval);
					int[] valuePerInterval = ArrayEx.CreateAndInitializeInstance(numberOfIntervals, 0);
					DateTime now = DateTime.Now;

					TimeStampItem<int>[] qa;
					lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
						qa = this.queue.ToArray();

					foreach (TimeStampItem<int> tsi in qa)
					{
						TimeSpan ts = (now - tsi.TimeStamp);
						int i = Int32Ex.Limit((int)(ts.TotalMilliseconds / this.interval), 0, Math.Max((numberOfIntervals - 1), 0)); // 'max' must be 0 or above.
						valuePerInterval[i] += tsi.Item;
					}

					// Weigh and sum up the intervals:
					int weight = numberOfIntervals;
					int weighedSum = 0;
					int sumOfWeights = 0;
					foreach (int valueOfInterval in valuePerInterval)
					{
						weighedSum += (valueOfInterval * weight);
						sumOfWeights += weight;
						weight--;
					}

					// Evaluate the rate:
					newValue = (int)Math.Round((double)weighedSum / sumOfWeights);
				}
			}

			if (newValue != oldValue)
			{
				this.value = newValue;
				return (true);
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
