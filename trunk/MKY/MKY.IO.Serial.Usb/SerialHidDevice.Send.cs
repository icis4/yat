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
// Copyright © 2003-2019 Matthias Kläy.
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
	/// This partial class implements the send part of <see cref="SerialHidDevice"/>.
	/// </remarks>
	public partial class SerialHidDevice
	{
		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual bool Send(byte data)
		{
			// AssertNotDisposed() is called by 'Send()' below.

			return (Send(new byte[] { data }));
		}

		/// <summary></summary>
		public virtual bool Send(byte[] data)
		{
			// AssertNotDisposed() is called by 'IsStarted' below.

			if (IsTransmissive)
			{
				foreach (byte b in data)
				{
					// Wait until there is space in the send queue:
					while (this.sendQueue.Count >= SendQueueFixedCapacity) // No lock required, just checking for full.
					{
						if (IsDisposed || !IsTransmissive) // Check 'IsDisposed' first!
							return (false);

						Thread.Sleep(TimeSpan.Zero); // Yield to other threads to allow dequeuing.
					}

					// There is space for at least one byte:
					lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
					{
						this.sendQueue.Enqueue(b);
					}
				}

				// Signal thread:
				SignalSendThreadSafely();

				return (true);
			}
			else
			{
				return (false);
			}
		}

		// Attention:
		// XOn/XOff handling is implemented in MKY.IO.Serial.SerialPort.SerialPort too!
		// Changes here must most likely be applied there too.

		/// <summary></summary>
		protected virtual void AssumeOutputXOn()
		{
			this.iXOnXOffHelper.OutputIsXOn = true;

			OnIOControlChanged(new EventArgs<DateTime>(DateTime.Now));
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOn state.
		/// </summary>
		public virtual void SignalInputXOn()
		{
			AssertNotDisposed();

			Send(XOnXOff.XOnByte);
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOff state.
		/// </summary>
		public virtual void SignalInputXOff()
		{
			AssertNotDisposed();

			Send(XOnXOff.XOffByte);
		}

		/// <summary>
		/// Toggles the input XOn/XOff state.
		/// </summary>
		public virtual void ToggleInputXOnXOff()
		{
			// AssertNotDisposed() and FlowControlUsesXOnXOff { get; } are called by the
			// 'InputIsXOn' property.

			if (InputIsXOn)
				SignalInputXOff();
			else
				SignalInputXOn();
		}

		/// <summary>
		/// Resets the XOn/XOff signaling count.
		/// </summary>
		public virtual void ResetXOnXOffCount()
		{
			AssertNotDisposed();

			this.iXOnXOffHelper.ResetCounts();

			OnIOControlChanged(new EventArgs<DateTime>(DateTime.Now));
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not
		/// invoked on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnDataSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendThread()
		{
			bool isXOffStateOldAndErrorHasBeenSignaled = false;

			DebugThreadState("SendThread() has started.");

			try
			{
				// Outer loop, processes data after a signal was received:
				while (!IsDisposed && this.sendThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.sendThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the send queue.
					// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.sendThreadRunFlag && IsTransmissive && (this.sendQueue.Count > 0))
					{                                                              // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						// Handle XOff state:
						if (this.settings.FlowControlUsesXOnXOff && !OutputIsXOn)
						{
							// Attention:
							// XOn/XOff handling is implemented in MKY.IO.Serial.SerialPort.SerialPort too!
							// Changes here must most likely be applied there too.

							// Control bytes must be sent even in case of XOff! XOn has precedence over XOff.
							if (this.sendQueue.Contains(XOnXOff.XOnByte)) // No lock required, not modifying anything.
							{
								SendXOnOrXOffAndNotify(XOnXOff.XOnByte);

								lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
								{
									if (this.sendQueue.Peek() == XOnXOff.XOnByte) // If XOn is upfront...
										this.sendQueue.Dequeue();                 // ...acknowlege it's gone.
								}

								break; // Let other threads do their job and wait until signaled again.
							}
							else if (this.sendQueue.Contains(XOnXOff.XOffByte)) // No lock required, not modifying anything.
							{
								SendXOnOrXOffAndNotify(XOnXOff.XOffByte);

								lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
								{
									if (this.sendQueue.Peek() == XOnXOff.XOffByte) // If XOff is upfront...
										this.sendQueue.Dequeue();                  // ...acknowlege it's gone.
								}

								break; // Let other threads do their job and wait until signaled again.
							}
							else
							{
								if (!isXOffStateOldAndErrorHasBeenSignaled)
								{
									InvokeXOffErrorEvent();
									isXOffStateOldAndErrorHasBeenSignaled = true;
								}

								break; // Let other threads do their job and wait until signaled again.
							}

							// Control bytes must be sent even in case of XOff!
						}

						// --- No XOff state, ready to send: ---

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
								byte[] data;
								lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
								{
									data = this.sendQueue.ToArray();
									this.sendQueue.Clear();
								}

								this.device.Send(data);

								if (this.settings.FlowControlUsesXOnXOff)
									HandleXOnOrXOffAndNotify(data);

								// Note the Thread.Sleep(TimeSpan.Zero) above.
							}
							finally
							{
								Monitor.Exit(this.dataEventSyncObj);
							}
						}
						else // Monitor.TryEnter()
						{
							DebugMessage("SendThread() monitor has timed out!");
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the device!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("SendThread() has terminated.");
		}

		// Attention:
		// XOn/XOff handling is implemented in MKY.IO.Serial.SerialPort.SerialPort too!
		// Changes here must most likely be applied there too.

		private void HandleXOnOrXOffAndNotify(byte[] data)
		{
			bool signalIOControlChanged = false;

			foreach (byte b in data)
			{
				if (XOnXOff.IsXOnOrXOffByte(b))
				{
					this.iXOnXOffHelper.XOnOrXOffSent(b);
					signalIOControlChanged = true; // XOn/XOff count has changed.
				}
			}

			if (signalIOControlChanged)
				OnIOControlChanged(new EventArgs<DateTime>(DateTime.Now));
		}

		private void SendXOnOrXOffAndNotify(byte b)
		{
			this.device.Send(b);

			if (this.iXOnXOffHelper.XOnOrXOffSent(b))
				OnIOControlChanged(new EventArgs<DateTime>(DateTime.Now));
		}

		private void InvokeXOffErrorEvent()
		{
			OnIOError
			(
				new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"XOff state, retaining data..."
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
