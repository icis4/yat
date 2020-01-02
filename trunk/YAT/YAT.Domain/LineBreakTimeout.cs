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
// YAT Version 2.1.1 Development
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY;
using MKY.Diagnostics;

#endregion

namespace YAT.Domain
{
	/// <summary></summary>
	public class LineBreakTimeout : IDisposable, IDisposableEx
	{
		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(LineBreakTimeout).FullName);

		private int timeout;
		private Timer timer;
		private object timerSyncObj = new object();

		/// <summary></summary>
		public event EventHandler Elapsed;

		/// <summary></summary>
		public LineBreakTimeout(int timeout)
		{
			this.timeout = timeout;
		}

		#region Disposal
		//--------------------------------------------------------------------------------------
		// Disposal
		//--------------------------------------------------------------------------------------

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
			////Debug.WriteLine("Remind (2016-09-08 / MKY) 'Elapsed' event handler not yet free'd, whole timer handling should be encapsulated into the 'LineState' class.");
			////DebugEventManagement.DebugWriteAllEventRemains(this); => Intended implementation commented out to prevent 100+ lines of debug output (Terminal.ToString()).
				this.eventHelper.DiscardAllEventsAndExceptions();  // => Remind output also commented out to prevent 75 lines of debug output at startup.

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the timer is stopped in Stop().
					StopAndDisposeTimer();
				}

				// Set state to disposed:
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
		~LineBreakTimeout()
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

		/// <summary></summary>
		public virtual void Start()
		{
			AssertNotDisposed();

			CreateAndStartTimer();
		}

		/// <summary></summary>
		public virtual void Restart()
		{
			// AssertNotDisposed() is called by methods below.

			Stop();
			Start();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			StopAndDisposeTimer();
		}

		private void CreateAndStartTimer()
		{
			lock (this.timerSyncObj)
			{
				if (this.timer == null)
				{
					this.timer = new Timer(new TimerCallback(timer_Timeout), null, this.timeout, Timeout.Infinite);
				}
			}
		}

		private void StopAndDisposeTimer()
		{
			lock (this.timerSyncObj)
			{
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
			}
		}

		private void timer_Timeout(object obj)
		{
			// Non-periodic timer, only a single timeout event thread can be active at a time.
			// There is no need to synchronize callbacks to this event handler.

			lock (this.timerSyncObj)
			{
				if ((this.timer == null) || (IsDisposed))
					return; // Handle overdue event callbacks.
			}

			Stop();

			OnElapsed(EventArgs.Empty);
		}

		/// <summary></summary>
		protected virtual void OnElapsed(EventArgs e)
		{
			this.eventHelper.RaiseSync(Elapsed, this, e);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
