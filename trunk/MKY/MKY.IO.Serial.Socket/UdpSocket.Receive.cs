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
// Copyright © 2007-2021 Matthias Kläy.
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

	// Enable debugging of receiving:
////#define DEBUG_RECEIVE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
//// "System.Net" as well as "ALAZ.SystemEx.NetEx" are explicitly used for more obvious distinction.
//// "System.Net.Sockets" as well.
using System.Threading;

using MKY.Collections.Generic;
using MKY.Diagnostics;
using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <remarks>
	/// This partial class implements the receive part of <see cref="UdpSocket"/>.
	/// </remarks>
	public partial class UdpSocket
	{
		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void BeginReceiveIfEnabled()
		{
			lock (this.socketSyncObj)
			{
				// Ensure that async receive is no longer initiated after close/dispose:
				if (IsUndisposed && (GetStateSynchronized() == SocketState.Opened)) // Check disposal state first!
				{
				////var state = new AsyncReceiveState(this.socket, this.localPort, this.localFilter, this.localFilter.IPv4MaskBytes); <= Commented-out to prevent FxCopy message CA1811 "AvoidUncalledPrivateCode" (Microsoft.Performance).
					var state = new AsyncReceiveState(this.socket, this.localFilter.IPv4MaskBytes);
					DebugReceive(string.Format("Beginning receive on local port {0} filtered for {1}...", this.localPort, this.localFilter));
					this.socket.BeginReceive(new AsyncCallback(ReceiveCallback), state);
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Partially same code for multiple exceptions.")]
		private void ReceiveCallback(IAsyncResult ar)
		{
			DebugReceive("Receive callback...");

			var asyncState = (AsyncReceiveState)(ar.AsyncState);

			// Ensure that async receive is discarded after close/dispose:
			if (IsUndisposed && (asyncState.Socket != null) && (GetStateSynchronized() == SocketState.Opened)) // Check disposal state first!
			{
				var addressNone = IPAddressEx.GetNoneOfFamily(LocalInterface);
				var remoteEndPoint = new System.Net.IPEndPoint(addressNone, System.Net.IPEndPoint.MinPort);
				byte[] data;                // EndReceive() will populate the object with address and port of the sender.
				bool discard = false;
				try
				{
					// Note:
					// Using async state to forward information from main to callback
					// for not having to lock the [socketSyncObj] on accessing members.

				////DebugReceive(string.Format("...ending receive on local port {0}...", asyncState.LocalPort)); <= Commented-out to prevent FxCopy message CA1811 "AvoidUncalledPrivateCode" (Microsoft.Performance).
					DebugReceive(string.Format("...ending receive on local port {0}...",       this.LocalPort)); // Consequently, using members for debug output.
					data = asyncState.Socket.EndReceive(ar, ref remoteEndPoint);
					DebugReceive(string.Format("...{0} byte(s) received from {1}.", ((data != null) ? data.Length : 0), remoteEndPoint));

					if (IPFilterEx.IsIPv4Refused(asyncState.LocalFilterIPv4MaskBytes, remoteEndPoint.Address))
					{
					////DebugReceive(string.Format("Byte(s) are discarded since received data is filtered for {0}.", asyncState.LocalFilter)); <= Commented-out to prevent FxCopy message CA1811 "AvoidUncalledPrivateCode" (Microsoft.Performance).
						DebugReceive(string.Format("Byte(s) are discarded since received data is filtered for {0}.",       this.LocalFilter)); // Consequently, using members for debug output.
						discard = true;
					}
				}
				catch (Exception ex) // Using general catch to reset state for further processing.
				{
					var socketException = ex as System.Net.Sockets.SocketException;
					if (socketException != null)
					{
						if (socketException.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionReset)
						{
							SocketReset(); // Required after this exception.
							OnIOError(new IOErrorEventArgs(ErrorSeverity.Acceptable, Direction.Input, ex.Message));
						}                                             // Acceptable as situation may recover.
						else
						{
							SocketError();
							OnIOError(new IOErrorEventArgs(ErrorSeverity.Fatal, ex.Message));
						}
					}
					else if ((ex is ObjectDisposedException) ||
					         (ex is NullReferenceException))
					{
						if (ex is ObjectDisposedException)
							DebugEx.WriteException(this.GetType(), ex, "The underlying UDP/IP socket has been disposed in the meantime.");

						if (ex is NullReferenceException)
							DebugEx.WriteException(this.GetType(), ex, "The underlying UDP/IP socket no longer exists in the meantime.");

						SocketError();
					////signalErrorArgs is not required as Stop() or Dispose() must have been invoked intentionally.
					}
					else
					{
						throw; // Rethrow!
					}

					// Reset state for further processing:
					data = null;
					remoteEndPoint.Address = addressNone;
					remoteEndPoint.Port = System.Net.IPEndPoint.MinPort;
					discard = true;
				}

				// Handle data:
				if ((data != null) && (!discard))
				{
					lock (this.receiveQueue) // Lock is required because "Queue<T>" is not synchronized.
					{
						DebugReceive(string.Format("Enqueuing {0} byte(s)...", data.Length));

						foreach (byte b in data)
							this.receiveQueue.Enqueue(new Pair<byte, System.Net.IPEndPoint>(b, remoteEndPoint));

						// Note that individual bytes are enqueued, not array of bytes. Analysis has
						// shown that this is faster than enqueuing arrays, since this callback will
						// mostly be called with rather low numbers of bytes.
					}

					SignalReceiveThreadSafely();
				}

				// Handle server connection:
				if ((this.socketType == UdpSocketType.Server) && (!discard))
				{
					// Set the remote end point to the sender of the first or most recently received data, depending on send mode:

					bool hasChanged = false;

					lock (this.socketSyncObj)
					{
						bool updateRemoteEndPoint = false;

						switch (this.serverSendMode)
						{
							case UdpServerSendMode.None:                                                                   /* Do nothing. */     break;
							case UdpServerSendMode.First:      if (IPAddressEx.EqualsNone(this.remoteHost.Address)) updateRemoteEndPoint = true; break;
							case UdpServerSendMode.MostRecent:                                                      updateRemoteEndPoint = true; break;
							default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.serverSendMode.ToString() + "' is a UDP/IP server send mode that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}

						if (updateRemoteEndPoint)
						{                        // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
							if (!this.remoteHost.Equals(remoteEndPoint.Address)) {
								this.remoteHost = remoteEndPoint.Address;
								hasChanged = true;
							}

							if (this.remotePort != remoteEndPoint.Port) {
								this.remotePort = remoteEndPoint.Port;
								hasChanged = true;
							}
						}
					}

					if (hasChanged) {
						OnIOChanged(new EventArgs<DateTime>(DateTime.Now));
					}
				} // if (IsServer)

				BeginReceiveIfEnabled(); // Continue receiving in case the socket is still ready or ready again.

			} // if (IsUndisposed && ...)
			else
			{
				DebugReceive("...discarded.");
			}
		}

		/// <summary>
		/// Asynchronously manage incoming events to prevent potential deadlocks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of received data will generate many events
		/// handled by <see cref="ReceiveCallback"/>. However, since <see cref="OnDataReceived"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, incoming data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="ReceiveCallback"/> event above.
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
						if (!this.receiveThreadEvent.WaitOne(SocketBase.Random.Next(50, 200)))
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
								System.Net.IPEndPoint remoteEndPoint = null;
								List<byte> data;

								lock (this.receiveQueue) // Lock is required because "Queue<T>" is not synchronized.
								{
									data = new List<byte>(this.receiveQueue.Count); // Preset the required capacity to improve memory management.

									while (this.receiveQueue.Count > 0)
									{
										Pair<byte, System.Net.IPEndPoint> item;

										// First, peek to check what end point the data refers to:
										item = this.receiveQueue.Peek();

										if (remoteEndPoint == null) {
											remoteEndPoint = item.Value2;
										}
										else if (remoteEndPoint != item.Value2) {
											break; // Break as soon as data of a different end point is available.
										}          // Receiving from different end point will continue immediately.

										// If still the same end point, dequeue the item to acknowledge it's gone:
										item = this.receiveQueue.Dequeue();
										data.Add(item.Value1);
									}
								}

								DebugReceive(string.Format("...{0} byte(s) from {1} dequeued", data.Count, remoteEndPoint));
								OnDataReceived(new SocketDataReceivedEventArgs(data.ToArray(), remoteEndPoint));
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
				// Don't try to set and notify a state change, or even restart the socket!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreads("ReceiveThread() has terminated.");
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
		private void DebugReceive(string message)
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
