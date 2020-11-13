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
	public class RateProvider : DisposableBase
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
		private System.Timers.Timer updateTicker; // Ambiguity with 'System.Threading.Timer'.

		private string diagnosticsName;

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
			: this(rateInterval, rateWindow, updateInterval, null)
		{
		}

		/// <summary></summary>
		public RateProvider(double rateInterval, double rateWindow, double updateInterval, string diagnosticsName)
		{
			this.rate = new Rate(rateInterval, rateWindow);

			this.updateInterval = updateInterval;

			this.updateTicker = new System.Timers.Timer(); // 'Timers.Timer' rather than 'Threading.Timer' because 'e.SignalTime' is needed.
			this.updateTicker.Interval = this.updateInterval;
			this.updateTicker.AutoReset = true; // Periodic!
			this.updateTicker.Elapsed += updateTicker_Periodic_Elapsed;

			this.diagnosticsName = diagnosticsName;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				DebugMessage("Disposing...");

				if (this.updateTicker != null) {
					EventHandlerHelper.RemoveAllEventHandlers(this.updateTicker);
					this.updateTicker.Dispose();
					this.updateTicker = null;
				}

				DebugMessage("...successfully disposed.");
			}

		////base.Dispose(disposing) of 'DisposableBase' doesn't need and cannot be called since abstract.
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
			get { AssertUndisposed(); return (this.updateInterval);        }
			set { AssertUndisposed();         this.updateInterval = value; }
		}

		/// <summary></summary>
		public double RateInterval
		{
			get { AssertUndisposed(); return (this.rate.Interval);        }
			set { AssertUndisposed();         this.rate.Interval = value; }
		}

		/// <summary></summary>
		public double RateWindow
		{
			get { AssertUndisposed(); return (this.rate.Window);        }
			set { AssertUndisposed();         this.rate.Window = value; }
		}

		/// <summary></summary>
		public int RateValue
		{
			get { AssertUndisposed(); return (this.rate.Value);        }
		}

		/// <summary></summary>
		public string DiagnosticsName
		{
			get { return (this.diagnosticsName); } // AssertUndisposed() shall not be called from this simple get-property.
			set { this.diagnosticsName = value;  } // AssertUndisposed() shall not be called from this simple set-property.
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Start()
		{
			this.updateTicker.Start();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			this.updateTicker.Stop();
		}

		/// <summary></summary>
		public virtual void StartStop()
		{
			if (this.updateTicker.Enabled)
				this.updateTicker.Stop();
			else
				this.updateTicker.Start();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool Update(int value, bool allowChangedEvent = true)
		{
			AssertUndisposed();

			bool hasChanged = this.rate.Update(value);

			if (hasChanged && allowChangedEvent)
				OnRateChanged(new RateEventArgs(this.rate.Value));

			return (hasChanged);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool Update(DateTime endOfWindow, bool allowChangedEvent = true)
		{
			AssertUndisposed();

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
			AssertUndisposed();

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
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
				return (UpdateInterval.ToString(CultureInfo.CurrentCulture));
			else
				return (base.ToString());
		}

		#endregion

		#region Timer Event Handlers
		//==========================================================================================
		// Timer Event Handlers
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private object updateTicker_Periodic_Elapsed_SyncObj = new object();

		private void updateTicker_Periodic_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(updateTicker_Periodic_Elapsed_SyncObj))
			{
				try
				{
					// Ensure not to forward events during closing anymore:
					if (IsUndisposed && (this.updateTicker != null) && this.updateTicker.Enabled)
					{
						Update(e.SignalTime);
					}
				}
				finally
				{
					Monitor.Exit(updateTicker_Periodic_Elapsed_SyncObj);
				}
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("updateTicker_Elapsed() monitor has timed out, skipping this concurrent event.");
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
		/// Name 'DebugWriteLine' would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named 'Message' for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. 'Common' for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
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
					(!string.IsNullOrEmpty(this.diagnosticsName) ? "[" + this.diagnosticsName + "]" : ""),
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
