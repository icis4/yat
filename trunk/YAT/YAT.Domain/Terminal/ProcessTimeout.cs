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
// YAT Version 2.2.0 Development
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY;

#endregion

namespace YAT.Domain
{
	/// <summary></summary>
	public class ProcessTimeout : DisposableBase
	{
		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(ProcessTimeout).FullName);

		private int timeout;
		private Timer timer;
		private object timerSyncObj = new object();

		/// <summary></summary>
		public event EventHandler<EventArgs<DateTime>> Elapsed;

		/// <summary></summary>
		public ProcessTimeout(int timeout)
		{
			this.timeout = timeout;                                                // Prevents the timer from starting.
			this.timer = new Timer(new TimerCallback(timer_Timeout), null, Timeout.Infinite, Timeout.Infinite);
		}

		#region Disposal
		//--------------------------------------------------------------------------------------
		// Disposal
		//--------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
		////Debug.WriteLine("Remind (2016-09-08 / MKY) 'Elapsed' event handler not yet free'd, whole timer handling should be encapsulated into the 'LineState' class.");
		////DebugEventManagement.DebugWriteAllEventRemains(this); => Intended implementation commented out to prevent 100+ lines of debug output (Terminal.ToString()).
			this.eventHelper.DiscardAllEventsAndExceptions();  // => Remind output also commented out to prevent 75 lines of debug output at startup.

			// Dispose of managed resources:
			if (disposing)
			{
				// In the 'normal' case, the timer is stopped in Stop().
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
			}
		}

		#endregion

		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="at"/> is <see cref="DateTime.MinValue"/> or <see cref="DateTime.MaxValue"/>.
		/// </exception>
		public virtual void Start(DateTime at)
		{
			AssertUndisposed();

			lock (this.timerSyncObj)
				this.timer.Change(ToDueTime(at), Timeout.Infinite);
		}

		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="at"/> is <see cref="DateTime.MinValue"/> or <see cref="DateTime.MaxValue"/>.
		/// </exception>
		protected virtual int ToDueTime(DateTime at)
		{
			if (at == DateTime.MinValue)
				throw (new ArgumentOutOfRangeException("at", at, MessageHelper.InvalidExecutionPreamble + "'" + at + "' must not be 'DateTime.MinValue'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (at == DateTime.MaxValue)
				throw (new ArgumentOutOfRangeException("at", at, MessageHelper.InvalidExecutionPreamble + "'" + at + "' must not be 'DateTime.MaxValue'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			var offset = (int)(((at - DateTime.Now).TotalMilliseconds) + 0.5); // Simple rounding is good enough.
			var dueTime = (this.timeout + offset);
			if (dueTime > 0)
				return (dueTime);
			else
				return (0);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertUndisposed();

			lock (this.timerSyncObj)      // Prevents the timer from starting.
				this.timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void timer_Timeout(object obj)
		{
			// Non-periodic timer, only a single timeout event thread can be active at a time.
			// There is no need to synchronize callbacks to this event handler.

			var now = DateTime.Now; // Capture instant in time as soon as possible.

			lock (this.timerSyncObj)
			{
				if ((this.timer == null) || (IsInDisposal)) // Ensure to not handle async timer callback during closing anymore.
					return;
			}

			OnElapsed(new EventArgs<DateTime>(now));
		}

		/// <summary></summary>
		protected virtual void OnElapsed(EventArgs<DateTime> e)
		{
			this.eventHelper.RaiseSync(Elapsed, this, e);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
