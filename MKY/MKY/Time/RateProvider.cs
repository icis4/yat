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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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

		/// <summary></summary>
	public class RateProvider : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

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
			this.updateTicker.Elapsed += new System.Timers.ElapsedEventHandler(updateTicker_Elapsed);
			this.updateTicker.Start();
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.updateTicker != null)
						this.updateTicker.Dispose();
				}

				// Set state to disposed:
				this.updateTicker = null;
				this.isDisposed = true;
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

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
		}

#endif // DEBUG

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
		public int UpdateInterval
		{
			get { AssertNotDisposed(); return (this.updateInterval); }
			set { AssertNotDisposed(); this.updateInterval = value;  }
		}

		/// <summary></summary>
		public int RateInterval
		{
			get { AssertNotDisposed(); return (this.rate.Interval); }
			set { AssertNotDisposed(); this.rate.Interval = value;  }
		}

		/// <summary></summary>
		public int RateWindow
		{
			get { AssertNotDisposed(); return (this.rate.Window); }
			set { AssertNotDisposed(); this.rate.Window = value;  }
		}

		/// <summary></summary>
		public int RateValue
		{
			get { AssertNotDisposed(); return (this.rate.Value); }
			set { AssertNotDisposed(); this.rate.Value = value;  }
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
			if (System.Threading.Monitor.TryEnter(timer_Elapsed_SyncObj)) // Not using 'System.Threading' to prevent conflicts with 'System.Timers'.
			{
				try
				{
					// Ensure not to forward events during closing anymore.
					if (!this.isDisposed && (this.updateTicker != null) && this.updateTicker.Enabled)
					{
						Update(e.SignalTime);
					}
				}
				finally
				{
					System.Threading.Monitor.Exit(timer_Elapsed_SyncObj); // Not using 'System.Threading' to prevent conflicts with 'System.Timers'.
				}
			} // Monitor.TryEnter()
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
