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

namespace MKY.Time
{
	/// <summary></summary>
	public class Rate
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int window;
		private int interval;
		private int numberOfIntervals;

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

		/// <param name="interval">The interval to calculate the value of the rate</param>
		/// <remarks><see cref="Window"/> will be set to <paramref name="interval"/>.</remarks>
		public Rate(int interval)
			: this(interval, interval)
		{
		}

		/// <param name="interval">The interval to calculate the value of the rate.</param>
		/// <param name="window"><see cref="Window"/>.</param>
		public Rate(int interval, int window)
		{
			if (window < interval)
				throw (new ArgumentOutOfRangeException("window", window, MessageHelper.InvalidExecutionPreamble + "Value must be equal or larger than 'interval' = '" + interval + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			this.interval = interval;
			this.window   = window;
			UpdateNumberOfIntervals();

			this.queue = new Queue<TimeStampItem<int>>(); // No clue how many items to expect, the default behavior must be good enough.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// The interval to calculate the value of the rate.
		/// </summary>
		public int Interval
		{
			get { return (this.interval); }
			set
			{
				if (value > window)
					throw (new ArgumentOutOfRangeException("value", value, MessageHelper.InvalidExecutionPreamble + "Value must be equal or less than 'window' = '" + window + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.interval = value;
				UpdateNumberOfIntervals();
			}
		}

		/// <summary>
		/// The window to calculate the rate.
		/// </summary>
		/// <remarks>
		/// If window is larger than <see cref="Interval"/>, a value is calculated for each interval
		/// and the values are weighed. The value of the less recent interval is weighed most, more
		/// recent intervals are weighed less.
		/// </remarks>
		public int Window
		{
			get { return (this.window); }
			set
			{
				if (value < interval)
					throw (new ArgumentOutOfRangeException("value", value, MessageHelper.InvalidExecutionPreamble + "Value must be equal or larger than 'interval' = '" + interval + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.window = value;
				UpdateNumberOfIntervals();
			}
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
		public virtual bool Update(int item)
		{
			int newValue;
			return (Update(item, out newValue));
		}

		/// <summary></summary>
		public virtual bool Update(int item, out int newValue)
		{
			var now = DateTime.Now; // A single time stamp for the whole operation.
			AddItemToQueue(now, item);
			RemoveObsoleteItemsFromQueue(now);
			return (CalculateValueFromQueue(now, out newValue));
		}

		/// <summary></summary>
		public virtual bool Update(DateTime timeStamp)
		{
			int newValue;
			return (Update(timeStamp, out newValue));
		}

		/// <summary></summary>
		public virtual bool Update(DateTime timeStamp, out int newValue)
		{
			RemoveObsoleteItemsFromQueue(timeStamp);
			return (CalculateValueFromQueue(timeStamp, out newValue));
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			ClearQueue();
			ClearValue();
		}

		#region Methods > Private
		//------------------------------------------------------------------------------------------
		// Methods > Private
		//------------------------------------------------------------------------------------------

		private void UpdateNumberOfIntervals()
		{
			this.numberOfIntervals = (int)(Math.Ceiling((double)this.window / this.interval));
		}

		private void ClearQueue()
		{
			lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
				this.queue.Clear();
		}

		private void AddItemToQueue(DateTime timeStamp, int item)
		{
			lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
				this.queue.Enqueue(new TimeStampItem<int>(timeStamp, item));
		}

		private void RemoveObsoleteItemsFromQueue(DateTime endOfWindow)
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

		private bool CalculateValueFromQueue(DateTime timeStamp, out int newValue)
		{
			// Get items from queue:
			TimeStampItem<int>[] qa;
			lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
				qa = this.queue.ToArray();

			// Prepare calculation:
			int[] valuePerInterval = ArrayEx.CreateAndInitializeInstance(this.numberOfIntervals, 0); // Makes no sense to keep as member.
			                                                                                       //// Requires initialization to 0 anyway.
			unchecked
			{
				// Count number of items within each interval:
				foreach (TimeStampItem<int> tsi in qa) // Queue will only contain items within the window.
				{
					TimeSpan ts = (timeStamp - tsi.TimeStamp);
					int i = Int32Ex.Limit((int)(Math.Round(ts.TotalMilliseconds / this.interval)), 0, (valuePerInterval.Length - 1));
					valuePerInterval[i] += tsi.Item;
				}

				// Weigh and sum up the intervals:
				int weight = this.numberOfIntervals;
				int weighedSum = 0;
				int sumOfWeights = 0;
				foreach (int valueOfInterval in valuePerInterval)
				{
					weighedSum += (valueOfInterval * weight);
					sumOfWeights += weight;
					weight--;
				}

				// Evaluate the rate:
				newValue = (int)(Math.Round((double)weighedSum / sumOfWeights));
			}

			if (this.value != newValue)
			{
				this.value = newValue;
				return (true);
			}
			else
			{
				return (false);
			}
		}

		private void ClearValue()
		{
			this.value = 0;
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
