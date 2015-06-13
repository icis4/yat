//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#endregion

namespace MKY.Time
{
	/// <summary></summary>
	public class RateHelper
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int interval;
		private int window;

		private Queue<TimeStampItem<int>> queue = new Queue<TimeStampItem<int>>();
		private object queueSyncObj = new object();
		private int value;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RateHelper()
			: this(1000, 5000)
		{
		}

		/// <summary></summary>
		public RateHelper(int interval)
			: this(interval, interval)
		{
		}

		/// <summary></summary>
		public RateHelper(int interval, int window)
		{
			this.interval = interval;
			this.window = window;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public int Interval
		{
			get { return (this.interval); }
			set { this.interval = value;  }
		}

		/// <summary></summary>
		public int Window
		{
			get { return (this.window); }
			set { this.window = value;  }
		}

		/// <summary></summary>
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
			lock (this.queueSyncObj)
				this.queue.Clear();
		}

		private void AddValueToQueue(int value)
		{
			lock (this.queueSyncObj)
				this.queue.Enqueue(new TimeStampItem<int>(value));
		}

		private void RemoveObsoleteFromQueue()
		{
			RemoveObsoleteFromQueue(DateTime.Now);
		}

		private void RemoveObsoleteFromQueue(DateTime now)
		{
			bool isWithinWindow = true;
			DateTime otherEndOfWindow = (now - TimeSpan.FromMilliseconds(this.window));

			lock (this.queueSyncObj)
			{
				while ((this.queue.Count > 0) && isWithinWindow)
				{
					TimeStampItem<int> tsi = this.queue.Peek();
					if (tsi.TimeStamp < otherEndOfWindow)
						this.queue.Dequeue();
					else
						isWithinWindow = false;
				}
			}
		}

		private bool CalculateValueFromQueue()
		{
			return (CalculateValueFromQueue(0));
		}

		private bool CalculateValueFromQueue(int value)
		{
			int oldValue = this.value;
			int newValue = 0;

			// If value was 0 before, only consider the current value.
			if (oldValue <= 0)
			{
				newValue = value;
			}
			else
			{
				// Count number of items within each interval.
				int numberOfIntervals = (int)(this.window / this.interval);
				int[] valuePerInterval = ArrayEx.CreateAndInitializeInstance<int>(numberOfIntervals, 0);
				DateTime now = DateTime.Now;

				TimeStampItem<int>[] qa;
				lock (this.queueSyncObj)
					qa = this.queue.ToArray();

				foreach (TimeStampItem<int> tsi in qa)
				{
					TimeSpan ts = (now - tsi.TimeStamp);
					int i = Int32Ex.LimitToBounds((int)(ts.TotalMilliseconds / this.interval), 0, numberOfIntervals - 1);
					valuePerInterval[i] += tsi.Item;
				}

				// Weigh and sum up the intervals.
				int weight = numberOfIntervals;
				int weighedSum = 0;
				int sumOfWeights = 0;
				foreach (int valueOfInterval in valuePerInterval)
				{
					weighedSum += (valueOfInterval * weight);
					sumOfWeights += weight;
					weight--;
				}

				// Evaluate the rate.
				newValue = (int)((double)weighedSum / sumOfWeights);
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
