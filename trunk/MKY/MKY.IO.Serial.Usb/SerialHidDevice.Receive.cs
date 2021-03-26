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
// MKY Version 1.0.29
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Diagnostics;

#endregion

namespace MKY.IO.Serial.Usb
{
	/// <remarks>
	/// This partial class implements the receive part of <see cref="SerialHidDevice"/>.
	/// </remarks>
	public partial class SerialHidDevice
	{
		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="device_DataReceived"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="device_DataReceived"/> event above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		private void ReceiveThread()
		{
			DebugThreads("...ReceiveThread() has started.");

			try
			{
				// Outer loop, runs when signaled as well as periodically checking the state:
				while (IsUndisposed && this.receiveThreadRunFlag) // Check disposal state first!
				{
					// A signal will only be received a while after initialization, thus waiting for
					// signal first. Also, placing this code first in the outer loop makes the logic
					// of the two loops more obvious.
					try
					{
						// WaitOne() would wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the state again.
						// The period can be quite long, as an event signal will immediately resume.
						if (!this.receiveThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue; // to periodically check state.
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in ReceiveThread()!");
						break;
					}

					// Inner loop, runs as long as there are items in the queue:
					while (IsUndisposed && this.receiveThreadRunFlag && (this.receiveQueue.Count > 0)) // Check disposal state first!
					{                                                // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero); // "TimeSpan.Zero" = 100% CPU is OK as receiving shall happen as fast as possible.

						if (Monitor.TryEnter(this.dataEventSyncObj, 10)) // Allow a short time to enter, as sending
						{                                                // could be busy mostly locking the object.
							try
							{
								byte[] data;
								lock (this.receiveQueue) // Lock is required because "Queue<T>" is not synchronized.
								{
									data = this.receiveQueue.ToArray();
									this.receiveQueue.Clear();
								}

								OnDataReceived(new SerialDataReceivedEventArgs(data, Info, this.device.ReportFormat.UseId, this.device.ActiveReportId));
							}
							finally
							{
								Monitor.Exit(this.dataEventSyncObj);
							}
						}
					////else // Monitor.TryEnter()
					////{
					////	DebugMessage("ReceiveThread() monitor has timed out, trying again...");
					////}

						// Note the Thread.Sleep(TimeSpan.Zero) further above.

						// Saying hello to StyleCop ;-.
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "ReceiveThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the device!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreads("ReceiveThread() has terminated.");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
