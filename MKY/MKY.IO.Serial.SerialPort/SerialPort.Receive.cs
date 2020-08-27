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

	// Enable debugging of thread state (send and receive threads):
////#define DEBUG_THREAD_STATE // Attention: Must also be activated in SerialPort[.Send].cs !!

	// Enable debugging of receiving:
////#define DEBUG_RECEIVE // Attention: Must also be activated in SerialPort.cs !!

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
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
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		private void ReceiveThread()
		{
			DebugThreadState("ReceiveThread() has started.");

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
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
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
						Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as receiving shall happen as fast as possible.

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

								DebugReceiveDequeue(data.Length);

								OnDataReceived(new SerialDataReceivedEventArgs(data, PortId));

								DebugReceiveSignal(data.Length);
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

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_RECEIVE")]
		private void DebugReceive(string format, params object[] args)
		{
			DebugMessage(format, args);
		}

	#if (DEBUG_RECEIVE)
		private int DebugReceive_enqueueCounter; // = 0;
		private int DebugReceive_dequeueCounter; // = 0;
		private int DebugReceive_signalCounter;  // = 0;
	#endif

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_RECEIVE")]
		private void DebugReceiveEnqueue(int count)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) enqueued for receiving, ", count);
		#if (DEBUG_RECEIVE)
			unchecked { DebugReceive_enqueueCounter += count; }
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) in total.", DebugReceive_enqueueCounter);
		#endif
			DebugMessage(sb.ToString());
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_RECEIVE")]
		private void DebugReceiveDequeue(int count)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) dequeued for receiving, ", count);
		#if (DEBUG_RECEIVE)
			unchecked { DebugReceive_dequeueCounter += count; }
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) in total.", DebugReceive_dequeueCounter);
		#endif
			DebugMessage(sb.ToString());
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_RECEIVE")]
		private void DebugReceiveSignal(int count)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) signaled for receiving, ", count);
		#if (DEBUG_RECEIVE)
			unchecked { DebugReceive_signalCounter += count; }
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) in total.", DebugReceive_signalCounter);
		#endif
			DebugMessage(sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
