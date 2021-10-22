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
// MKY Version 1.0.30
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

#endregion

namespace MKY.Time
{
	/// <summary></summary>
	public class Chronometer : DisposableBase
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
		/// to handle/workaround the following issue:
		///
		/// <![CDATA[
		/// System.Reflection.TargetInvocationException was unhandled by user code
		///   Message=Ein Aufrufziel hat einen Ausnahmefehler verursacht.
		///   Source=mscorlib
		///   StackTrace:
		///        bei System.RuntimeMethodHandle._InvokeMethodFast(Object target, Object[] arguments, SignatureStruct& sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner)
		///        bei System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
		///        bei System.Delegate.DynamicInvokeImpl(Object[] args)
		///        bei MKY.EventHelper.Item.InvokeOnCurrentThread(Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 595.
		///        bei MKY.EventHelper.Item.RaiseSync[TEventArgs](Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 399.
		///        bei MKY.Time.Chronometer.OnTimeSpanChanged(TimeSpanEventArgs e) in D:\Workspace\YAT\Trunk\MKY\MKY\Time\Chronometer.cs:Zeile 350.
		///        bei MKY.Time.Chronometer.timer_Elapsed(Object sender, ElapsedEventArgs e) in D:\Workspace\YAT\Trunk\MKY\MKY\Time\Chronometer.cs:Zeile 330.
		///        bei System.Timers.Timer.MyTimerCallback(Object state)
		///   InnerException:
		///        Message=Invoke oder BeginInvoke kann für ein Steuerelement erst aufgerufen werden, wenn das Fensterhandle erstellt wurde.
		///        Source=System.Windows.Forms
		///        StackTrace:
		///             bei System.Windows.Forms.Control.WaitForWaitHandle(WaitHandle waitHandle)
		///             bei System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous)
		///             bei System.Windows.Forms.Control.Invoke(Delegate method, Object[] args)
		///             bei MKY.EventHelper.Item.InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 567.
		///             bei MKY.EventHelper.Item.RaiseSync[TEventArgs](Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 397.
		///             bei YAT.Model.Terminal.OnIOConnectTimeChanged(TimeSpanEventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Model\Terminal.cs:Zeile 5258.
		///             bei YAT.Model.Terminal.totalConnectChrono_TimeSpanChanged(Object sender, TimeSpanEventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Model\Terminal.cs:Zeile 4204.
		/// ]]>
		///
		/// The chronometers get properly disposed of, but apparently there may still be pending
		/// asynchronous 'zombie' callback that later throw an exception. No feasible solution
		/// has been found.
		///
		/// Temporarily disabling this handling/workaround can be useful for debugging, i.e. to
		/// continue program execution even in case of exceptions and let the debugger handle it.
		/// </remarks>
		private readonly EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Chronometer).FullName, exceptionHandling: EventHelper.ExceptionHandlingMode.DiscardDisposedTarget);
	////private readonly EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Chronometer).FullName); // See remarks above!

		private System.Timers.Timer secondTicker; // Ambiguity with 'System.Threading.Timer'.
		private DateTime startTimeStamp = DateTime.Now;
		private TimeSpan accumulatedTimeSpan = TimeSpan.Zero;

		private string diagnosticsName;

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
			this.secondTicker = new System.Timers.Timer(); // "Timers.Timer" rather than "Threading.Timer" because "e.SignalTime" is needed.
			this.secondTicker.Interval = 1000;
			this.secondTicker.AutoReset = true; // Periodic!
			this.secondTicker.Elapsed += secondTicker_Periodic_Elapsed;
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

				if (this.secondTicker != null) {
					EventHandlerHelper.RemoveAllEventHandlers(this.secondTicker);
					this.secondTicker.Dispose();
					this.secondTicker = null;
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
		public double Interval
		{
			get { AssertUndisposed(); return (this.secondTicker.Interval);        }
			set { AssertUndisposed();         this.secondTicker.Interval = value; }
		}

		/// <summary></summary>
		public TimeSpan TimeSpan
		{
			get
			{
				AssertUndisposed();

				if (!this.secondTicker.Enabled)
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
			Start(DateTime.Now);
		}

		/// <remarks>
		/// This overload is useful if multiple objects need to be synchronized in terms of time.
		/// </remarks>
		public virtual void Start(DateTime now)
		{
			AssertUndisposed();

			if (!this.secondTicker.Enabled)
			{
				this.secondTicker.Start();
				this.startTimeStamp = now;

				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertUndisposed();

			if (this.secondTicker.Enabled)
			{
				this.secondTicker.Stop();
				this.accumulatedTimeSpan += (DateTime.Now - this.startTimeStamp);

				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		public virtual void StartStop()
		{
			if (!this.secondTicker.Enabled)
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
			AssertUndisposed();

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
			AssertUndisposed();

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
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
				return (TimeSpanEx.FormatInvariantThousandthsEnforceMinutes(TimeSpan));
			else
				return (base.ToString());
		}

		#endregion

		#region Timer Event Handlers
		//==========================================================================================
		// Timer Event Handlers
		//==========================================================================================

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private readonly object secondTicker_Periodic_Elapsed_SyncObj = new object();

		private void secondTicker_Periodic_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Ensure that only one timer elapsed event thread is active at a time. Because if the
			// execution takes longer than the timer interval, more and more timer threads will pend
			// here, and then be executed after the previous has been executed. This will require
			// more and more resources and lead to a drop in performance.
			if (Monitor.TryEnter(secondTicker_Periodic_Elapsed_SyncObj))
			{
				try
				{
					// Ensure not to forward events during closing anymore:
					if (IsUndisposed && (this.secondTicker != null) && this.secondTicker.Enabled)
					{
						OnTimeSpanChanged(new TimeSpanEventArgs(CalculateTimeSpan(e.SignalTime)));
					}
				}
				finally
				{
					Monitor.Exit(secondTicker_Periodic_Elapsed_SyncObj);
				}
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("secondTicker_Elapsed() monitor has timed out, skipping this concurrent event.");
			}
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
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
