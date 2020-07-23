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
// Copyright © 2007-2020 Matthias Kläy.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
//// 'System.Net' as well as 'ALAZ.SystemEx.NetEx' are explicitly used for more obvious distinction.
using System.Threading;

using MKY.Collections.Generic;
using MKY.Diagnostics;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="TcpServer"/>.
	/// </remarks>
	public partial class TcpServer
	{
		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
		////AssertUndisposed() is called by 'IsStarted' below.

			if (IsStarted)
			{
				lock (this.socketConnections) // Directly locking the list is OK, it is kept throughout the lifetime of an object.
				{
					foreach (var connection in this.socketConnections)
						connection.BeginSend(data);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void StartDataSentThread()
		{
			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread == null)
				{
					this.dataSentThreadRunFlag = true;
					this.dataSentThreadEvent = new AutoResetEvent(false);
					this.dataSentThread = new Thread(new ThreadStart(DataSentThread));
					this.dataSentThread.Name = ToShortEndPointString() + " DataSent Thread";
					this.dataSentThread.Start();
				}
			}
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopDataSentThread()
		{
			lock (this.dataSentThreadSyncObj)
			{
				if (this.dataSentThread != null)
				{
					DebugThreadState("DataSentThread() gets stopped...");

					this.dataSentThreadRunFlag = false;

					// Ensure that thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.dataSentThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.dataSentThread.Join(interval = SocketBase.Random.Next(5, 20)))
						{
							SignalDataSentThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;           // Thread.Abort() must not be used whenever possible!
								this.dataSentThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreadState("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreadState("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadState("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.dataSentThread = null;
				}

				if (this.dataSentThreadEvent != null)
				{
					try     { this.dataSentThreadEvent.Close(); }
					finally { this.dataSentThreadEvent = null; }
				}
			}
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalDataSentThreadSafely()
		{
			try
			{
				if (this.dataSentThreadEvent != null)
					this.dataSentThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not
		/// invoked on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		private void DataSentThread()
		{
			DebugThreadState("SendThread() has started.");

			try
			{
				// Outer loop, runs when signaled as well as periodically checking the state:
				while (IsUndisposed && this.dataSentThreadRunFlag) // Check disposal state first!
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
						if (!this.dataSentThreadEvent.WaitOne(SocketBase.Random.Next(50, 200)))
							continue; // to periodically check state.
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in DataSentThread()!");
						break;
					}

					// Inner loop, runs as long as there are items in the queue:
					while (IsUndisposed && this.dataSentThreadRunFlag && (this.dataSentQueue.Count > 0)) // Check disposal state first!
					{                                                 // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as sending shall happen as fast as possible.

						// Synchronize the send/receive events to prevent mix-ups at the event
						// sinks, i.e. the send/receive operations shall be synchronized with
						// signaling of them.
						// But attention, do not simply lock() the sync obj. Instead, just try
						// to get the lock or try again later. The thread = direction that get's
						// the lock first, shall also be the one to signal first:

						if (Monitor.TryEnter(this.dataEventSyncObj, 10)) // Allow a short time to enter, as receiving
						{                                                // could be busy mostly locking the object.
							try
							{
								System.Net.IPEndPoint remoteEndPoint = null;
								List<byte> data;

								lock (this.dataSentQueue) // Lock is required because Queue<T> is not synchronized.
								{
									data = new List<byte>(this.dataSentQueue.Count); // Preset the required capacity to improve memory management.

									while (this.dataSentQueue.Count > 0)
									{
										Pair<byte, System.Net.IPEndPoint> item;

										// First, peek to check whether data refers to a different end point:
										item = this.dataSentQueue.Peek();

										if (remoteEndPoint == null)
											remoteEndPoint = item.Value2;
										else if (remoteEndPoint != item.Value2)
											break; // Break as soon as data of a different end point is available.

										// If still the same end point, dequeue the item to acknowledge it's gone:
										item = this.dataSentQueue.Dequeue();
										data.Add(item.Value1);
									}
								}

								OnDataSent(new SocketDataSentEventArgs(data.ToArray(), remoteEndPoint));
							}
							finally
							{
								Monitor.Exit(this.dataEventSyncObj);
							}
						}
						else // Monitor.TryEnter()
						{
							DebugMessage("DataSentThread() monitor has timed out, trying again...");
						}

						// Note the Thread.Sleep(TimeSpan.Zero) above.

						// Saying hello to StyleCop ;-.
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "DataSentThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the socket!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("SendThread() has terminated.");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
