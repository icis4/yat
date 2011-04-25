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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Event;

namespace MKY.Time
{
	/// <summary></summary>
	public class Chronometer : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private System.Timers.Timer timer;

		private TimeSpan accumulatedTimeSpan = TimeSpan.Zero;
		private DateTime startTimeStamp = DateTime.Now;

		private object timer_Elapsed_SyncObj = new object();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised the tick interval elapsed or the time span was reset.")]
		public event EventHandler<TimeSpanEventArgs> TimeSpanChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Chronometer()
		{
			this.timer = new System.Timers.Timer();
			this.timer.AutoReset = true;
			this.timer.Interval = 1000;
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
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
		~Chronometer()
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
		public double Interval
		{
			get { AssertNotDisposed(); return (this.timer.Interval); }
			set { AssertNotDisposed(); this.timer.Interval = value;  }
		}

		/// <summary></summary>
		public TimeSpan TimeSpan
		{
			get
			{
				AssertNotDisposed();

				if (!this.timer.Enabled)
					return (this.accumulatedTimeSpan);
				else
					return (this.accumulatedTimeSpan + (DateTime.Now - this.startTimeStamp));
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Start()
		{
			AssertNotDisposed();

			if (!this.timer.Enabled)
			{
				this.timer.Start();
				this.startTimeStamp = DateTime.Now;
				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			if (this.timer.Enabled)
			{
				this.timer.Stop();
				this.accumulatedTimeSpan += (DateTime.Now - this.startTimeStamp);
				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		public virtual void StartStop()
		{
			if (!this.timer.Enabled)
				Start();
			else
				Stop();
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			AssertNotDisposed();

			this.startTimeStamp = DateTime.Now;
			this.accumulatedTimeSpan = TimeSpan.Zero;
			OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan.Zero));
		}

		/// <summary></summary>
		public virtual void Restart()
		{
			AssertNotDisposed();

			Stop();
			Reset();
			Start();
		}

		/// <summary></summary>
		public override string ToString()
		{
			AssertNotDisposed();
			return (TimeSpanEx.FormatTimeSpan(TimeSpan, true));
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
						OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
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
		protected virtual void OnTimeSpanChanged(TimeSpanEventArgs e)
		{
			EventHelper.SuspendUnhandledException();
			EventHelper.FireSync<TimeSpanEventArgs>(TimeSpanChanged, this, e);
			EventHelper.ResumeUnhandledException();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
