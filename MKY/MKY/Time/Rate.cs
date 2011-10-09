//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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

using MKY.Event;

#endregion

namespace MKY.Time
{
	#region RateEventArgs
	//==================================================================================================
	// RateEventArgs
	//==================================================================================================

	/// <summary></summary>
	public class RateEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly int Rate;

		/// <summary></summary>
		public RateEventArgs(int rate)
		{
			Rate = rate;
		}
	}

	#endregion

	/// <summary></summary>
	public class Rate : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private System.Timers.Timer timer;
		private object timer_Elapsed_SyncObj = new object();

		private int tick;
		private int interval;
		private int window;

		private Queue<TimeStampItem<int>> queue = new Queue<TimeStampItem<int>>();
		private int value;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when the tick interval elapsed or the time span was reset.")]
		public event EventHandler<RateEventArgs> Changed;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Rate()
			: this (100, 1000, 5000)
		{
		}

		/// <summary></summary>
		public Rate(int tick, int interval, int window)
		{
			this.tick = tick;
			this.interval = interval;
			this.window = window;

			this.timer = new System.Timers.Timer();
			this.timer.AutoReset = true;
			this.timer.Interval = this.tick;
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
			this.timer.Start();
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					if (this.timer != null)
						this.timer.Dispose();
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Rate()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public int Tick
		{
			get { AssertNotDisposed(); return (this.tick); }
			set { AssertNotDisposed(); this.tick = value;  }
		}

		/// <summary></summary>
		public int Interval
		{
			get { AssertNotDisposed(); return (this.interval); }
			set { AssertNotDisposed(); this.interval = value;  }
		}

		/// <summary></summary>
		public int Window
		{
			get { AssertNotDisposed(); return (this.window); }
			set { AssertNotDisposed(); this.window = value;  }
		}

		/// <summary></summary>
		public int Value
		{
			get { AssertNotDisposed(); return (this.value); }
			set { AssertNotDisposed(); this.value = value;  }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool Update(int value)
		{
			AssertNotDisposed();

			AddValueToQueue(value);
			RemoveObsoleteFromQueue();
			return (CalculateValueFromQueueAndSignalIfChanged(value));
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			AssertNotDisposed();

			this.queue.Clear();
			CalculateValueFromQueueAndSignalIfChanged();
		}

		private void AddValueToQueue(int value)
		{
			this.queue.Enqueue(new TimeStampItem<int>(value));
		}

		private void RemoveObsoleteFromQueue()
		{
			bool isWithinWindow = true;
			DateTime otherEndOfWindow = DateTime.Now - TimeSpan.FromMilliseconds(this.window);
			while ((this.queue.Count > 0) && isWithinWindow)
			{
				TimeStampItem<int> tsi = this.queue.Peek();
				if (tsi.TimeStamp < otherEndOfWindow)
					this.queue.Dequeue();
				else
					isWithinWindow = false;
			}
		}


		private bool CalculateValueFromQueueAndSignalIfChanged()
		{
			return (CalculateValueFromQueueAndSignalIfChanged(0));
		}

		private bool CalculateValueFromQueueAndSignalIfChanged(int value)
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
				foreach (TimeStampItem<int> tsi in this.queue.ToArray())
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
				OnRateChanged(new RateEventArgs(this.value));
				return (true);
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator int(Rate rate)
		{
			return (rate.value);
		}

		#endregion

		#region Timer Event Handlers
		//==========================================================================================
		// Timer Event Handlers
		//==========================================================================================

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure not to forward events during closing anymore.
			if (!this.isDisposed && (this.timer != null) && this.timer.Enabled)
			{
				// Ensure that only one timer elapsed event thread is active at a time.
				// Without this exclusivity, two receive threads could create a race condition.
				if (Monitor.TryEnter(timer_Elapsed_SyncObj))
				{
					try
					{
						RemoveObsoleteFromQueue();
						CalculateValueFromQueueAndSignalIfChanged();
					}
					finally
					{
						Monitor.Exit(timer_Elapsed_SyncObj);
					}
				}
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnRateChanged(RateEventArgs e)
		{
			EventHelper.FireSync<RateEventArgs>(Changed, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
