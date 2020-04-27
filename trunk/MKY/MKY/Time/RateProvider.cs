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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

using MKY.Diagnostics;

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
		public int Rate { get; }

		/// <summary></summary>
		public RateEventArgs(int rate)
		{
			Rate = rate;
		}
	}

	#endregion

	/// <summary></summary>
	public class RateProvider : IDisposable, IDisposableEx
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		/// <remarks> \remind (2019-08-22 / MKY)
		///
		/// Explicitly setting <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/>
		/// to handle/workaround a similar issue as described in <see cref="Chronometer"/>.
		///
		/// Temporarily disabling this handling/workaround can be useful for debugging, i.e. to
		/// continue program execution even in case of exceptions and let the debugger handle it.
		/// </remarks>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(RateProvider).FullName, exceptionHandling: EventHelper.ExceptionHandlingMode.DiscardDisposedTarget);
	////private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(RateProvider).FullName); // See remarks above!

		private Rate rate;

		private double updateInterval;
		private System.Timers.Timer updateTicker; // Not "using" 'System.Timers' to prevent conflicts with 'System.Threading'.

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
		public RateProvider(double rateInterval, double updateInterval)
			: this(rateInterval, rateInterval, updateInterval)
		{
		}

		/// <summary></summary>
		public RateProvider(double rateInterval, double rateWindow, double updateInterval)
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

				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.updateTicker != null)
					{
						EventHandlerHelper.RemoveAllEventHandlers(this.updateTicker);
						this.updateTicker.Dispose();
					}
				}

				// Set state to disposed:
				this.updateTicker = null;
				IsDisposed = true;

				DebugMessage("...successfully disposed.");
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
		public double UpdateInterval
		{
			get { AssertNotDisposed(); return (this.updateInterval);        }
			set { AssertNotDisposed();         this.updateInterval = value; }
		}

		/// <summary></summary>
		public double RateInterval
		{
			get { AssertNotDisposed(); return (this.rate.Interval);        }
			set { AssertNotDisposed();         this.rate.Interval = value; }
		}

		/// <summary></summary>
		public double RateWindow
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool Update(int value, bool allowChangedEvent = true)
		{
			AssertNotDisposed();

			bool hasChanged = this.rate.Update(value);

			if (hasChanged && allowChangedEvent)
				OnRateChanged(new RateEventArgs(this.rate.Value));

			return (hasChanged);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool Update(DateTime endOfWindow, bool allowChangedEvent = true)
		{
			AssertNotDisposed();

			int value;
			bool hasChanged = this.rate.Update(endOfWindow, out value);

			if (hasChanged && allowChangedEvent)
				OnRateChanged(new RateEventArgs(value));

			return (hasChanged);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void Reset(bool allowChangedEvent = true)
		{
			AssertNotDisposed();

			this.rate.Reset();

			if (allowChangedEvent)
				OnRateChanged(new RateEventArgs(this.rate.Value));
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
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (UpdateInterval.ToString(CultureInfo.CurrentCulture));
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
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("updateTicker_Elapsed() monitor has timed out!");
			}
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that somthing will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"",
					"",
					message
				)
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
