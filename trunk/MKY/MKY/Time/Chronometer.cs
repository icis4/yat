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
// MKY Version 1.0.26 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Diagnostics;

#endregion

namespace MKY.Time
{
	/// <summary></summary>
	public class Chronometer : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow autonomously ignoring exceptions when disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Chronometer).FullName);

		private System.Timers.Timer timer;

		private TimeSpan accumulatedTimeSpan = TimeSpan.Zero;
		private DateTime startTimeStamp = DateTime.Now;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when the tick interval elapsed or the time span was reset.")]
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
			this.timer.Elapsed += timer_Elapsed;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.timer != null)
					{
						this.timer.Dispose();
						EventHandlerHelper.RemoveAllEventHandlers(this.timer);
					}
				}

				// Set state to disposed:
				this.timer = null;
				IsDisposed = true;
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Chronometer()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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
			get { AssertNotDisposed(); return (this.timer.Interval);        }
			set { AssertNotDisposed();         this.timer.Interval = value; }
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
					return (CalculateTimeSpan(DateTime.Now));
			}
		}

		/// <summary></summary>
		protected virtual TimeSpan CalculateTimeSpan(DateTime now)
		{
			return (this.accumulatedTimeSpan + (now - this.startTimeStamp));
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Start()
		{
			Start(DateTime.Now);
		}

		/// <remarks>
		/// This overload is useful if multiple objects need to be synchronized in terms of time.
		/// </remarks>
		public virtual void Start(DateTime now)
		{
			AssertNotDisposed();

			if (!this.timer.Enabled)
			{
				this.timer.Start();
				this.startTimeStamp = now;

				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
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
			Reset(DateTime.Now);
		}

		/// <remarks>
		/// This overload is useful if multiple objects need to be synchronized in terms of time.
		/// </remarks>
		public virtual void Reset(DateTime now)
		{
			AssertNotDisposed();

			this.startTimeStamp = now;
			this.accumulatedTimeSpan = TimeSpan.Zero;

			OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan.Zero));
		}

		/// <summary></summary>
		public virtual void Restart()
		{
			Restart(DateTime.Now);
		}

		/// <remarks>
		/// This overload is useful if multiple objects need to be synchronized in terms of time.
		/// </remarks>
		public virtual void Restart(DateTime now)
		{
			AssertNotDisposed();

			Stop();
			Reset(now);
			Start(now);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method!

			return (TimeSpanEx.FormatInvariantThousandthsEnforceMinutes(TimeSpan));
		}

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
					// Ensure not to forward events during closing anymore:
					if (!IsDisposed && (this.timer != null) && this.timer.Enabled)
					{
						OnTimeSpanChanged(new TimeSpanEventArgs(CalculateTimeSpan(e.SignalTime)));
					}
				}
				finally
				{
					Monitor.Exit(timer_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTimeSpanChanged(TimeSpanEventArgs e)
		{
			this.eventHelper.RaiseSync<TimeSpanEventArgs>(TimeSpanChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
