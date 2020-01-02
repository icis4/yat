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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of receive request:
////#define DEBUG_RECEIVE_REQUEST

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Diagnostics;

#endregion

namespace MKY.IO.Serial.SerialPort
{
	/// <remarks>
	/// This partial class implements the receive part of <see cref="SerialPort"/>.
	/// </remarks>
	public partial class SerialPort
	{
		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <remarks>
		/// Will be signaled by <see cref="port_DataReceived"/> event above, or by XOn/XOff while
		/// sending.
		/// </remarks>
		private void ReceiveThread()
		{
			DebugThreadState("ReceiveThread() has started.");

			try
			{
				// Outer loop, processes data after a signal has been received:
				while (!IsDisposed && this.receiveThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.receiveThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in ReceiveThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the receive queue.
					// Ensure not to forward events during disposing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.receiveThreadRunFlag && (this.receiveQueue.Count > 0))
					{                                               // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						if (Monitor.TryEnter(this.dataEventSyncObj, 10)) // Allow a short time to enter, as sending
						{                                                // could be busy mostly locking the object.
							try
							{
								byte[] data;
								lock (this.receiveQueue) // Lock is required because Queue<T> is not synchronized.
								{
									data = this.receiveQueue.ToArray();
									this.receiveQueue.Clear();
								}

								DebugReceiveRequest("Signaling " + data.Length.ToString() + " byte(s) received...");
								OnDataReceived(new SerialDataReceivedEventArgs(data, PortId));
								DebugReceiveRequest("...signaling done");

								// Note the Thread.Sleep(TimeSpan.Zero) above.
							}
							finally
							{
								Monitor.Exit(this.dataEventSyncObj);
							}
						}
						else // Monitor.TryEnter()
						{
							DebugMessage("ReceiveThread() monitor has timed out!");
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "ReceiveThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the port!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("ReceiveThread() has terminated.");
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG_RECEIVE_REQUEST")]
		private void DebugReceiveRequest(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
