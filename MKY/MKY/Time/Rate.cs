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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of update:
////#define DEBUG_UPDATE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#endregion

namespace MKY.Time
{
	/// <summary></summary>
	public class Rate
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private double window;
		private double interval;

		private int numberOfIntervals;
		private int[] itemsPerInterval;

		private Queue<TimeStampItem<int>> queue;
		private int value;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="Rate"/> class setting both
		/// <see cref="Interval"/> and <see cref="Window"/> to a second.
		/// </summary>
		public Rate()
			: this(1000)
		{
		}

		/// <param name="interval">The interval to calculate the value of the rate.</param>
		/// <remarks><see cref="Window"/> will be set to <paramref name="interval"/>.</remarks>
		public Rate(double interval)
			: this(interval, interval)
		{
		}

		/// <param name="interval">The interval to calculate the value of the rate.</param>
		/// <param name="window"><see cref="Window"/>.</param>
		public Rate(double interval, double window)
		{
			if (window < interval)
				throw (new ArgumentOutOfRangeException("window", window, MessageHelper.InvalidExecutionPreamble + "'Window' = '" + window + "' must be equal or larger than 'Interval' = '" + interval + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

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
		/// The interval to calculate the value of the rate, in milliseconds.
		/// </summary>
		/// <remarks>Set to 0.001 to get <see cref="Value"/> in items per microsecond.</remarks>
		/// <remarks>Set to 1 to get <see cref="Value"/> in items per millisecond.</remarks>
		/// <remarks>Set to 1000 to get <see cref="Value"/> in items per second.</remarks>
		public double Interval
		{
			get { return (this.interval); }
			set
			{
				if (value > window)
					throw (new ArgumentOutOfRangeException("value", value, MessageHelper.InvalidExecutionPreamble + "'Interval' must be equal or less than 'Window' = '" + window + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.interval = value;
				UpdateNumberOfIntervals();
			}
		}

		/// <summary>
		/// The window to calculate the rate, in milliseconds.
		/// </summary>
		/// <remarks>
		/// If window is larger than <see cref="Interval"/>, a value is calculated for each interval
		/// and the values are weighed. The value of the less recent interval is weighed most, more
		/// recent intervals are weighed less.
		/// </remarks>
		public double Window
		{
			get { return (this.window); }
			set
			{
				if (value < interval)
					throw (new ArgumentOutOfRangeException("value", value, MessageHelper.InvalidExecutionPreamble + "'Window' must be equal or larger than 'Interval' = '" + interval + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.window = value;
				UpdateNumberOfIntervals();
			}
		}

		/// <summary>
		/// The resulting rate value, in items per interval.
		/// </summary>
		/// <remarks>
		/// Using <c>int</c> rather than <c>double</c> for three reasons:
		/// <list type="bullet">
		/// <item><description>Indication whether value has changed on update is straight-forward with an <c>int</c>.</description></item>
		/// <item><description>Client can set the interval such it fits its needs.</description></item>
		/// <item><description>Client does not need to round the value.</description></item>
		/// </list>
		/// </remarks>
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
			int value;
			return (Update(item, out value));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool Update(int item, out int value)
		{
			var now = DateTime.Now; // A single time stamp for the whole operation.

			if (item != 0)
				return (AddItemAndUpdate(now, item, out value));
			else // There is no need to add zero-items to queue; improves performance especially in case of frequent updates.
				return (UpdateWithoutAdd(now, out value));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool AddItemAndUpdate(DateTime timeStamp, int item, out int value)
		{
			AddItemToQueue(timeStamp, item);
			RemoveObsoleteItemsFromQueue(timeStamp);
			var hasChanged = CalculateValueFromQueue(timeStamp, out value);

			DebugUpdate(string.Format(CultureInfo.InvariantCulture, "Updating with {0} at {1:ss.fff} {2} {3}", item, timeStamp, (hasChanged ? "has changed to" : "has remained at"), value));

			return (hasChanged);
		}

		/// <summary></summary>
		public virtual bool Update(DateTime timeStamp)
		{
			int value;
			return (Update(timeStamp, out value));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool Update(DateTime timeStamp, out int value)
		{
			return (UpdateWithoutAdd(timeStamp, out value));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool UpdateWithoutAdd(DateTime timeStamp, out int value)
		{
			RemoveObsoleteItemsFromQueue(timeStamp);
			var hasChanged = CalculateValueFromQueue(timeStamp, out value);

			DebugUpdate(string.Format(CultureInfo.InvariantCulture, "Updating at {0:ss.fff} {1} {2}", timeStamp, (hasChanged ? "has changed to" : "has remained at"), this.value));

			return (hasChanged);
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
			this.numberOfIntervals = (int)(Math.Ceiling(this.window / this.interval));
			this.itemsPerInterval = ArrayEx.CreateAndInitializeInstance(this.numberOfIntervals, 0); // Single array instance for performance optimization.
		}                                                                                           // Makes no sense to allocate and discard on each update.

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

		private bool CalculateValueFromQueue(DateTime timeStamp, out int value)
		{
			// Get items from queue:
			TimeStampItem<int>[] qa;
			lock (this.queue) // Lock is required because Queue<T> is not synchronized and whole queue is accessed via ToArray().
				qa = this.queue.ToArray();

			// Prepare calculation:
			Array.Clear(this.itemsPerInterval, 0, this.itemsPerInterval.Length);

			unchecked
			{
				// Count number of items within each interval:
				foreach (TimeStampItem<int> tsi in qa) // Queue will only contain items within the window.
				{
					TimeSpan ts = (timeStamp - tsi.TimeStamp);
					int i = Int32Ex.Limit((int)(Math.Round(ts.TotalMilliseconds / this.interval)), 0, (this.itemsPerInterval.Length - 1));
					this.itemsPerInterval[i] += tsi.Item;
				}

				// Weigh and sum up the intervals:
				int weight = this.numberOfIntervals;
				int weighedSum = 0;
				int sumOfWeights = 0;
				foreach (int itemsOfInterval in this.itemsPerInterval)
				{
					weighedSum += (itemsOfInterval * weight);
					sumOfWeights += weight;
					weight--;
				}

				// Evaluate the rate:
				value = (int)(Math.Round((double)weighedSum / sumOfWeights));
			}

			if (this.value != value)
			{
				this.value = value;
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_UPDATE")]
		private static void DebugUpdate(string message)
		{
			Debug.WriteLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
