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
// Copyright © 2003-2021 Matthias Kläy.
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

	// Enable debugging of update if 'DebugEnabled' is set:
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
	/// <remarks>
	/// A weighted sum is used to calculate the <see cref="Value"/> for better seeing changes.
	/// </remarks>
	public class Rate
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary>
		/// The default value for <see cref="Interval"/>.
		/// </summary>
		public const int IntervalDefault = 1000;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private double interval;
		private double window;

		private int numberOfBins;   // The number of bins is always integral, even in case of an odd interval-window-ratio.
		private double widthOfBins; // But the width will become less than the interval in case of an odd interval-window-ratio.
		private double binScaling;
		private int[] itemsPerBin;

		private Queue<TimeStampItem<int>> queue;
		private int value;

		private DateTime lastItemTimeStamp; // = DateTime.MinValue;

	#if (DEBUG)
		private bool debugEnabled; // = false;
	#endif

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
			: this(IntervalDefault)
		{
		}

		/// <param name="interval">The interval to calculate the value of the rate.</param>
		/// <remarks><see cref="Window"/> will be set to <paramref name="interval"/>.</remarks>
		public Rate(double interval)
			: this(interval, interval)
		{
		}

		/// <param name="interval">The interval to calculate the value of the rate.</param>
		/// <param name="window"><see cref="Window" />.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if 'window' is less than 'interval'.
		/// </exception>
		public Rate(double interval, double window)
		{
			if (window < interval)
				throw (new ArgumentOutOfRangeException("window", window, MessageHelper.InvalidExecutionPreamble + "'window' = '" + window + "' must be equal or larger than 'interval' = '" + interval + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			this.interval = interval;
			this.window   = window;
			UpdateBins();

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
		/// <remarks>Set to 1 to get <see cref="Value"/> in items per millisecond.</remarks>
		/// <remarks>Set to 1000 to get <see cref="Value"/> in items per second.</remarks>
		/// <remarks>Set to 60000 to get <see cref="Value"/> in items per minute.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if 'Interval' is greater than 'Window'.
		/// </exception>
		public virtual double Interval
		{
			get { return (this.interval); }
			set
			{
				if (value > window)
					throw (new ArgumentOutOfRangeException("value", value, MessageHelper.InvalidExecutionPreamble + "'Interval' must be equal or less than 'Window' = '" + window + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.interval = value;
				UpdateBins();
			}
		}

		/// <summary>
		/// The window to calculate the rate, in milliseconds.
		/// </summary>
		/// <remarks>
		/// If window is larger than <see cref="Interval"/>, a value is calculated for each interval
		/// and the values are weighted. The value of the less recent interval is weighed most, more
		/// recent intervals are weighted less.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if 'Window' is less than 'Interval'.
		/// </exception>
		public virtual double Window
		{
			get { return (this.window); }
			set
			{
				if (value < interval)
					throw (new ArgumentOutOfRangeException("value", value, MessageHelper.InvalidExecutionPreamble + "'Window' must be equal or larger than 'Interval' = '" + interval + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.window = value;
				UpdateBins();
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
		public virtual int Value
		{
			get { return (this.value); }
		}

		/// <summary>
		/// The time stamp of the last update.
		/// </summary>
		public virtual DateTime LastItemTimeStamp
		{
			get { return (this.lastItemTimeStamp); }
		}

		/// <summary></summary>
		public virtual bool LastItemWasWithinCurrentInterval
		{
			get
			{
				if (LastItemTimeStamp == default(DateTime))
					return (false);
				else
					return (((DateTime.Now - LastItemTimeStamp).TotalMilliseconds) < Interval);
			}
		}

		/// <summary></summary>
		public virtual int NumberOfIntervalsSinceLastItem
		{
			get
			{
				if (LastItemTimeStamp == default(DateTime))
					return (0);
				else
					return ((int)Math.Floor(((DateTime.Now - LastItemTimeStamp).TotalMilliseconds) / Interval));
			}
		}

	#if (DEBUG)

		/// <remarks>
		/// Flag in a addition to configuration item to allow selective debugging of one or multiple
		/// rate objects.
		/// </remarks>
		public virtual bool DebugEnabled
		{
			get { return (this.debugEnabled); }
			set { this.debugEnabled = value;  }
		}

	#endif

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

			this.lastItemTimeStamp = now; // While there may be no need to add, still use this time stamp.

			if (item == 0) // There is no need to add zero-items to the queue; improves performance especially in case of frequent updates.
				return (Update(now, out value));
			else
				return (Update(now, item, out value));
		}

		/// <summary></summary>
		public virtual bool Update(DateTime timeStamp, int item)
		{
			int value;
			return (Update(timeStamp, item, out value));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool Update(DateTime timeStamp, int item, out int value)
		{
			AddItemToQueue(timeStamp, item);
			RemoveObsoleteItemsFromQueue(timeStamp);
			var hasChanged = CalculateValueFromQueue(timeStamp, out value);

			DebugUpdate(string.Format(CultureInfo.InvariantCulture, "Updating with {0} at {1:ss.fff} {2} {3}", item, timeStamp, (hasChanged ? "has changed to" : "has remained at"), value));

			return (hasChanged);
		}

		/// <summary></summary>
		public virtual bool Update()
		{
			int value;
			return (Update(out value));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[CLSCompliant(false)]
		public virtual bool Update(out int value)
		{
			var now = DateTime.Now;
			return (Update(now, out value));
		}

		/// <summary></summary>
		public virtual bool Update(DateTime timeStamp)
		{
			int value;
			return (Update(timeStamp, out value));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[CLSCompliant(false)]
		public virtual bool Update(DateTime timeStamp, out int value)
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

		private void UpdateBins()
		{
			this.numberOfBins = (int)(Math.Ceiling(this.window / this.interval));
			this.widthOfBins  = (this.window / this.numberOfBins); // Bins must be equally distributed across window.
			this.binScaling   = (this.interval / this.widthOfBins); // Value must be scaled for odd-interval-window ratios.
			this.itemsPerBin  = ArrayEx.CreateAndInitializeInstance(this.numberOfBins, 0); // Single array instance for performance optimization.
		}                                                                                  // Makes no sense to allocate and discard on each update.

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
					if (tsi.TimeStamp <= beginningOfWindow) // Window must be ]window], i.e. excluding the lower limit.
						this.queue.Dequeue();               // Otherwise, the lowest bin typically contains a supernumerous item.
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
			Array.Clear(this.itemsPerBin, 0, this.itemsPerBin.Length);

			unchecked
			{
				// Count number of items within each interval bin, starting with the most recent item:
				foreach (TimeStampItem<int> tsi in qa) // Queue will only contain items within the window.
				{
					// Example: Interval = 1000, Window = 4000, UpdateInterval = 500, Items = { 1 }
					//
					// Window:
					// [     |     |     |     ]
					// 1  1  1  1  1  1  1  1  1
					//       ^ this item results in ts.TotalMilliseconds = 300 and must be also placed into most recent bin
					//    ^ this item results in ts.TotalMilliseconds = 3500 and must be placed into most recent bin
					// ^ this item got dequeued as it is <= window
					//
					// Intervals:
					// ]     ]     ]     ]     ]
					//    1  1  1  1  1  1  1  1
					//        ^ 2999 must result in index 2
					//       ^ 3000 must result in index 3
					//
					// Bins:
					// |  3  |  2  |  1  |  0  |

					TimeSpan ts = (timeStamp - tsi.TimeStamp);
					int index = (int)(Math.Floor(ts.TotalMilliseconds / this.widthOfBins));
					int indexSafe = Int32Ex.Limit(index, 0, (this.itemsPerBin.Length - 1));
					this.itemsPerBin[indexSafe] += tsi.Item;
				}

				// Weigh and sum up the interval bins:
				int weight = this.numberOfBins;
				double weightedSum = 0;
				int sumOfWeights = 0;
				foreach (int items in this.itemsPerBin)
				{
					weightedSum += (this.binScaling * items * weight);
					sumOfWeights += weight;
					weight--;
				}

				// Evaluate the rate:
				var valueUnrounded = (weightedSum / sumOfWeights);
				value = (int)(Math.Round(valueUnrounded));
			} // unchecked

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
		private void DebugUpdate(string message)
		{
		#if (DEBUG)
			if (DebugEnabled)
		#endif
			{
				Debug.WriteLine(message);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
