//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
	#region RateEventArgs
	//==================================================================================================
	// RateEventArgs
	//==================================================================================================

	/// <summary></summary>
	public class RateEventArgs : EventArgs
	{
		private int rate;

		/// <summary></summary>
		public RateEventArgs(int rate)
		{
			this.rate = rate;
		}

		/// <summary></summary>
		public int Rate
		{
			get { return (this.rate); }
		}
	}

	#endregion

	/// <remarks>
	/// There's an almost equal implementation in <see cref="RateHelper"/>. An object of that
	/// implementation should be used in this class, in order to reduce maintenance efforts.
	/// </remarks>
	public class Rate : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private System.Timers.Timer timer;

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
			: this(100, 1000, 5000)
		{
		}

		/// <summary></summary>
		public Rate(int tick, int interval)
			: this(tick, interval, interval)
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
				// In any case, dispose of the timer as it was created in the constructor:
				if (this.timer != null)
					this.timer.Dispose();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.timer = null;
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Rate()
		{
			Dispose(false);

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
		}

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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

			ClearQueue();
			CalculateValueFromQueueAndSignalIfChanged();
		}

		#region Methods > Private
		//------------------------------------------------------------------------------------------
		// Methods > Private
		//------------------------------------------------------------------------------------------

		private void ClearQueue()
		{
			lock (this.queue) // Lock is required because type is not synchronized and whole queue is accessed via ToArray().
				this.queue.Clear();
		}

		private void AddValueToQueue(int value)
		{
			lock (this.queue) // Lock is required because type is not synchronized and whole queue is accessed via ToArray().
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

			lock (this.queue) // Lock is required because type is not synchronized and whole queue is accessed via ToArray().
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

				TimeStampItem<int>[] qa;
				lock (this.queue) // Lock is required because type is not synchronized and whole queue is accessed via ToArray().
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
				OnRateChanged(new RateEventArgs(this.value));
				return (true);
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#endregion

		#region Timer Event Handlers
		//==========================================================================================
		// Timer Event Handlers
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object timer_Elapsed_SyncObj = new object();

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(timer_Elapsed_SyncObj))
			{
				try
				{
					// Ensure not to forward events during closing anymore.
					if (!this.isDisposed && (this.timer != null) && this.timer.Enabled)
					{
						RemoveObsoleteFromQueue(e.SignalTime);
						CalculateValueFromQueueAndSignalIfChanged();
					}
				}
				finally
				{
					Monitor.Exit(timer_Elapsed_SyncObj);
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
