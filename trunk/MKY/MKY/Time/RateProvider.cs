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
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Diagnostics;

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
		public int Rate { get; }

		/// <summary></summary>
		public RateEventArgs(int rate)
		{
			Rate = rate;
		}
	}

	#endregion

		/// <summary></summary>
	public class RateProvider : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow autonomously ignoring exceptions when disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(RateProvider).FullName);

		private Rate rate;

		private int updateInterval;
		private System.Timers.Timer updateTicker; // Not using 'System.Timers' to prevent conflicts with 'System.Threading'.

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
		public RateProvider()
			: this(1000, 5000, 100)
		{
		}

		/// <summary></summary>
		public RateProvider(int rateInterval, int updateInterval)
			: this(rateInterval, rateInterval, updateInterval)
		{
		}

		/// <summary></summary>
		public RateProvider(int rateInterval, int rateWindow, int updateInterval)
		{
			this.rate = new Rate(rateInterval, rateWindow);

			this.updateInterval = updateInterval;
			this.updateTicker = new System.Timers.Timer();
			this.updateTicker.AutoReset = true;
			this.updateTicker.Interval = this.updateInterval;
			this.updateTicker.Elapsed += updateTicker_Elapsed;
			this.updateTicker.Start();
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
					if (this.updateTicker != null)
					{
						this.updateTicker.Dispose();
						EventHandlerHelper.RemoveAllEventHandlers(this.updateTicker);
					}
				}

				// Set state to disposed:
				this.updateTicker = null;
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
		~RateProvider()
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
		public int UpdateInterval
		{
			get { AssertNotDisposed(); return (this.updateInterval);        }
			set { AssertNotDisposed();         this.updateInterval = value; }
		}

		/// <summary></summary>
		public int RateInterval
		{
			get { AssertNotDisposed(); return (this.rate.Interval);        }
			set { AssertNotDisposed();         this.rate.Interval = value; }
		}

		/// <summary></summary>
		public int RateWindow
		{
			get { AssertNotDisposed(); return (this.rate.Window);        }
			set { AssertNotDisposed();         this.rate.Window = value; }
		}

		/// <summary></summary>
		public int RateValue
		{
			get { AssertNotDisposed(); return (this.rate.Value);        }
			set { AssertNotDisposed();         this.rate.Value = value; }
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

			bool hasChanged = this.rate.Update(value);

			if (hasChanged)
				OnRateChanged(new RateEventArgs(this.rate.Value));

			return (hasChanged);
		}

		/// <summary></summary>
		public virtual bool Update(DateTime endOfWindow)
		{
			AssertNotDisposed();

			bool hasChanged = this.rate.Update(endOfWindow);

			if (hasChanged)
				OnRateChanged(new RateEventArgs(this.rate.Value));

			return (hasChanged);
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			AssertNotDisposed();

			this.rate.Reset();

			OnRateChanged(new RateEventArgs(this.rate.Value));
		}

		#endregion

		#region Timer Event Handlers
		//==========================================================================================
		// Timer Event Handlers
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object timer_Elapsed_SyncObj = new object();

		private void updateTicker_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
					if (!IsDisposed && (this.updateTicker != null) && this.updateTicker.Enabled)
					{
						Update(e.SignalTime);
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
		protected virtual void OnRateChanged(RateEventArgs e)
		{
			this.eventHelper.RaiseSync<RateEventArgs>(Changed, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
