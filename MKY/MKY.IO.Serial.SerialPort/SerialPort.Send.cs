﻿//==================================================================================================
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

	// Enable debugging of send request:
////#define DEBUG_SEND_REQUEST

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
using System.Globalization;
using System.IO;
using System.Threading;

using MKY.Diagnostics;
using MKY.Time;

#endregion

namespace MKY.IO.Serial.SerialPort
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="SerialPort"/>.
	/// </remarks>
	public partial class SerialPort
	{
		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <remarks>
		/// If the underlying buffer has space, this method will immediately return; otherwise
		/// this method will be blocking until there is space, or the I/O instance is stopped
		/// or gets disconnected/closed.
		/// </remarks>
		protected virtual bool Send(byte data)
		{
		////AssertUndisposed() is called by 'Send()' below.

			return (Send(new byte[] { data }));
		}

		/// <remarks>
		/// If the underlying buffer has space, this method will immediately return; otherwise
		/// this method will be blocking until there is space, or the I/O instance is stopped
		/// or gets disconnected/closed.
		/// </remarks>
		public virtual bool Send(byte[] data)
		{
		////AssertUndisposed() is called by 'IsStarted' below.

			if (IsTransmissive)
			{
				var initialTimeStamp = DateTime.Now;

				int sendBufferSize;
				if (this.settings.OutputBufferSize.Enabled)
					sendBufferSize = this.settings.OutputBufferSize.Size;
				else
					sendBufferSize = SendQueueInitialCapacity;

				DebugSendRequest("Enqueuing " + data.Length.ToString(CultureInfo.CurrentCulture) + " byte(s) for sending...");
				foreach (byte b in data)
				{
					// Wait until there is space in the send queue:
					while (this.sendQueue.Count >= sendBufferSize) // No lock required, just checking for full.
					{
						if (IsInDisposal || !IsTransmissive) // Check disposal state first!
							return (false);

						// Signal to ensure send thread keeps working:
						SignalSendThreadSafely();

						// Actively yield to other threads to allow dequeuing:
						var span = (DateTime.Now - initialTimeStamp);
						if (span.TotalMilliseconds < 4)
							Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as send
						else                             // a) is expected to potentially be blocking and
							Thread.Sleep(1);             // b) is short (max. 4 ms) yet.
					}                                    // But sleep if longer!

					// There is space for at least one byte:
					lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
					{
						this.sendQueue.Enqueue(b);
					}

					// Do not signal after each byte, this is a) not efficient and b) not desired
					// as send requests shall ideally be sent as a single chunk.
				}
				DebugSendRequest("...enqueuing done");

				// Signal thread to send the requested packet:
				SignalSendThreadSafely();

				return (true);
			}
			else
			{
				return (false);
			}
		}

		// Attention:
		// XOn/XOff handling is implemented in MKY.IO.Serial.Usb.SerialHidDevice too!
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
			AssertUndisposed();

			Send(XOnXOff.XOnByte);
		}

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOff state.
		/// </summary>
		public virtual void SignalInputXOff()
		{
			AssertUndisposed();

			Send(XOnXOff.XOffByte);
		}

		/// <summary>
		/// Toggles the input XOn/XOff state.
		/// </summary>
		public virtual void ToggleInputXOnXOff()
		{
			AssertUndisposed();

			if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
			{
				if (InputIsXOn)
					SignalInputXOff();
				else
					SignalInputXOn();
			}
		}

		/// <summary>
		/// Flushes the output buffer and waits for a time corresponding to the given number of frames.
		/// </summary>
		public virtual void Flush(int additionalWaitFrameCount)
		{
			AssertUndisposed();

			this.port.Flush();

			if (additionalWaitFrameCount > 0)
				Thread.Sleep((int)(Math.Ceiling(this.settings.Communication.FrameTime * additionalWaitFrameCount)));
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that software and/or hardware
		/// flow control is properly buffered and suspended if the communication counterpart
		/// requests so.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(byte[])"/>. However, since <see cref="OnDataSent"/> synchronously
		/// invokes the event, it will take some time until the send queue is checked again.
		/// During this time, no more new events are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(byte[])"/> method above, or by XOn/XOff while receiving.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that any exception leads to restart or reset of port.")]
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		private void SendThread()
		{
			// Calculate maximum baud defined send rate:
			double frameTime = this.settings.Communication.FrameTime;
			const int MaxFramesPerInterval = 48;                  // Interval shall be rather narrow enough to ensure being inside the limits
			double interval = (frameTime * MaxFramesPerInterval); // even for converters with buffers of just 56 bytes. 48 for two reasons:
			Rate maxBaudRatePerInterval = new Rate(interval);     //  1) 48/56 => 15% safety margin
			                                                      //  2) Same as SerialPortSettings.MaxChunkSizeDefault.Length
			// Typical values:
			//   baudRate  frameTime  interval
			// >     9600      ~1 ms     48 ms
			// >   115.2k     ~85 us      4 ms
			// >       1M     ~10 us    480 us

			bool intervalFourthIsAtLeastOneMillisecond = ((interval / 4) > 1.0);

			// Calculate maximum user defined send rate:
			Rate maxSendRate = new Rate(this.settings.MaxSendRate.Interval);

			bool isOutputBreakOldAndErrorHasBeenSignaled = false;
			bool isCtsInactiveOldAndErrorHasBeenSignaled = false;
			bool   isXOffStateOldAndErrorHasBeenSignaled = false;

			DebugThreadState("SendThread() has started.");

			try
			{
				// Outer loop, runs when signaled as well as periodically checking the state:
				while (IsUndisposed && this.sendThreadRunFlag) // Check disposal state first!
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
						if (!this.sendThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue; // to periodically check state.
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendThread()!");
						break;
					}

					// Inner loop, runs as long as there are items in the queue:
					                                             // 'IsOpen' is used instead of 'IsTransmissive' to allow handling break further below.
					while (IsUndisposed && this.sendThreadRunFlag && IsOpen && (this.sendQueue.Count > 0)) // Check disposal state first!
					{                                                       // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as sending shall happen as fast as possible
						                           //// and there will be 'yieldSomeMore' to reduce CPU consumption.
						bool isWriteTimeout = false;
						bool isOutputBreak  = false;

						// Handle output break state:
						if (this.port.OutputBreak) // No lock required, not modifying anything.
						{
							if (!isOutputBreakOldAndErrorHasBeenSignaled)
							{
								InvokeOutputBreakErrorEvent();
								isOutputBreakOldAndErrorHasBeenSignaled = true;
							}

							break; // Let other threads do their job and wait until signaled again.
						}

						// Handle inactive CTS line:
						if (this.settings.Communication.FlowControlUsesRtsCts && !this.port.CtsHolding) // No lock required, not modifying anything.
						{
							if (!isCtsInactiveOldAndErrorHasBeenSignaled)
							{
								InvokeCtsInactiveErrorEvent();
								isCtsInactiveOldAndErrorHasBeenSignaled = true;
							}

							break; // Let other threads do their job and wait until signaled again.
						}

						// Handle XOff state:
						if (this.settings.Communication.FlowControlManagesXOnXOffManually && !OutputIsXOn) // XOn/XOff information is not available for 'Software' or 'Combined'!
						{
							// Attention:
							// XOn/XOff handling is implemented in MKY.IO.Serial.Usb.SerialHidDevice too!
							// Changes here must most likely be applied there too.

							// Control bytes must be sent even in case of XOff! XOn has precedence over XOff.
							if (this.sendQueue.Contains(XOnXOff.XOnByte)) // No lock required, not modifying anything.
							{
								if (TryWriteXOnOrXOffAndNotify(XOnXOff.XOnByte, out isWriteTimeout, out isOutputBreak))
								{
									lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
									{
										if (this.sendQueue.Peek() == XOnXOff.XOnByte) // If XOn is upfront...
											this.sendQueue.Dequeue();                 // ...acknowlege it's gone.
									}
									break; // Let other threads do their job and wait until signaled again.
								}
							}
							else if (this.sendQueue.Contains(XOnXOff.XOffByte)) // No lock required, not modifying anything.
							{
								if (TryWriteXOnOrXOffAndNotify(XOnXOff.XOffByte, out isWriteTimeout, out isOutputBreak))
								{
									lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
									{
										if (this.sendQueue.Peek() == XOnXOff.XOffByte) // If XOff is upfront...
											this.sendQueue.Dequeue();                  // ...acknowlege it's gone.
									}
									break; // Let other threads do their job and wait until signaled again.
								}
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
						}

						// --- No break, no inactive CTS, no XOff state, ready to send: ---

						if (!isWriteTimeout && !isOutputBreak)
						{
							bool yieldSomeMore = false;

							// Synchronize the send/receive events to prevent mix-ups at the event
							// sinks, i.e. the send/receive operations shall be synchronized with
							// signaling of them.
							// But attention, do not simply lock() the 'dataSyncObj'. Instead, just
							// try to get the lock or try again later. The thread = direction that
							// get's the lock first, shall also be the one to signal first:

							if (Monitor.TryEnter(this.dataEventSyncObj, 10)) // Allow a short time to enter, as receiving
							{                                                // could be busy mostly locking the object.
								try
								{
									// By default, stuff as much data as possible into output buffer:
									int maxChunkSize = (this.port.WriteBufferSize - this.port.BytesToWrite);

									// Notes on sending:
									//
									// As soon as YAT started to write the maximum chunk size (in Q1/2016 ;-), data got lost
									// even for e.g. a local port loopback pair. All seems to work fine as long as small chunks
									// of e.g. 48 bytes some now and then are transmitted.
									//
									// For a while, I assumed data loss happens in the receive path. Therefore, I tried to use
									// async reading instead of the 'DataReceived' event, as suggested by online resources like
									// http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport written by
									// Ben Voigt.
									//
									// See 'port_DataReceived()' for more details on receiving.
									//
									// Finally (MKy/SSt/ATo in Q3/2016), the root cause for the data loss could be tracked down
									// to the physical limitations of the USB/COM and SPI/COM converter: If more data is sent
									// than the baud rate permits forwarding, the converter simply discards supernumerous data!
									// Of course, what else could it do... Actually, it could propagate the information back to
									// 'System.IO.Ports.SerialPort.BytesToWrite'. But that obviously isn't done...
									//
									// Solution: Limit output writing to baud rate :-)

									// Reduce chunk size if maximum is limited to baud rate:
									if ((maxChunkSize > 0) && this.settings.BufferMaxBaudRate)
									{
										int remainingSizeInInterval = (MaxFramesPerInterval - maxBaudRatePerInterval.Value);
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, Math.Max(remainingSizeInInterval, 0)); // 'max' must be 0 or above.
									}

									// Reduce chunk size if maximum send rate is specified:
									if ((maxChunkSize > 0) && this.settings.MaxSendRate.Enabled)
									{
										int remainingSizeInInterval = (this.settings.MaxSendRate.Size - maxSendRate.Value);
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, Math.Max(remainingSizeInInterval, 0)); // 'max' must be 0 or above.
									}

									// Further reduce chunk size if maximum is specified:
									if ((maxChunkSize > 0) && this.settings.MaxChunkSize.Enabled)
									{
										int maxChunkSizeSetting = this.settings.MaxChunkSize.Size;
										maxChunkSize = Int32Ex.Limit(maxChunkSize, 0, maxChunkSizeSetting); // 'Setting' is always above 0.
									}

									int effectiveChunkDataCount = 0;
									if (maxChunkSize > 0)
									{
										List<byte> effectiveChunkData;
										bool signalIOControlChanged;
										DateTime signalIOControlChangedTimeStamp;

										if (TryWriteChunkToPort(maxChunkSize, out effectiveChunkData, out isWriteTimeout, out isOutputBreak, out signalIOControlChanged, out signalIOControlChangedTimeStamp))
										{
											DebugSendRequest("Signaling " + effectiveChunkData.Count.ToString() + " byte(s) sent...");
											OnDataSent(new SerialDataSentEventArgs(effectiveChunkData.ToArray(), PortId));
											DebugSendRequest("...signaling done");

											effectiveChunkDataCount = effectiveChunkData.Count;
										}

										if (signalIOControlChanged)
										{
											OnIOControlChanged(new EventArgs<DateTime>(signalIOControlChangedTimeStamp));
										}
									}
									else
									{
										// Further above, Thread.Sleep(TimeSpan.Zero) already yields. However, a
										// maxChunkSize of 0 indicates that the thread is much faster than the
										// underlying sending, thus yield some more.

										yieldSomeMore = true;

										// Measurements (2020-04-24) counting the maximum number of loops with
										// maxChunkSize = 0 (i.e. doing nothing but wasting CPU) during sending
										// of a file at 9600 baud revealed:
										//  > Without additional yield: Up to ~12000 loops!
										//  > Current implementation with additional yield: Just ~32 loops max.
										//
										// Note: A small maxChunkSize (e.g. 1) indicates that output buffer and/or
										// send window is becoming available again, thus do not yield/sleep.
										//
										// Attention: yield/sleep must not happen within lock(dataEventSyncObj)!
									}

									// Update the send rates with the effective chunk size of the current interval.
									// This must be done no matter whether writing to port has succeeded or not!
									// Otherwise, on overload, a rate value may "get stuck" at the limit!
									// Note that 'Rate.Update(0)' does optimize, i.e. no need to further optimize.

									if (this.settings.BufferMaxBaudRate)
										maxBaudRatePerInterval.Update(effectiveChunkDataCount);

									if (this.settings.MaxSendRate.Enabled)
										maxSendRate.Update(effectiveChunkDataCount);
								}
								finally
								{
									Monitor.Exit(this.dataEventSyncObj);
								}
							}
							else // Monitor.TryEnter()
							{
								DebugMessage("SendThread() monitor has timed out, trying again...");
							}

							if (yieldSomeMore)
								Thread.Sleep((intervalFourthIsAtLeastOneMillisecond ? 1 : 0));
						}

						if (isWriteTimeout) // Timeout detected while trying to call System.IO.Ports.SerialPort.Write().
						{                   // May only be partial, some data may have been sent before timeout.
							if (this.settings.Communication.FlowControlUsesRtsCts && !this.port.CtsHolding)
							{
								if (!isCtsInactiveOldAndErrorHasBeenSignaled)
								{
									InvokeCtsInactiveErrorEvent();
									isCtsInactiveOldAndErrorHasBeenSignaled = true;
								}
							}
							else if (this.settings.Communication.FlowControlManagesXOnXOffManually && !OutputIsXOn)
							{
								if (!isXOffStateOldAndErrorHasBeenSignaled)
								{
									InvokeXOffErrorEvent();
									isXOffStateOldAndErrorHasBeenSignaled = true;
								}
							}
							else if (this.settings.Communication.FlowControlUsesXOnXOff) // Handle independent on '!OutputIsXOn' because XOn/XOff
							{                                                            // information not available for 'Software' or 'Combined'!
								if (!isXOffStateOldAndErrorHasBeenSignaled)
								{
									InvokeXOffErrorEvent();
									isXOffStateOldAndErrorHasBeenSignaled = true;
								}
							}
							else
							{
								// Do not output a warning in case of unspecified timeouts.
								// Such may happen when too much data is sent too quickly.
							}
						}
						else // !isWriteTimeout
						{
							isCtsInactiveOldAndErrorHasBeenSignaled = false;
							  isXOffStateOldAndErrorHasBeenSignaled = false;
						}

						if (isOutputBreak) // Output break detected *WHILE* trying to call System.IO.Ports.SerialPort.Write().
						{                  // May only be partial, some data may have been sent before break.
							if (!isOutputBreakOldAndErrorHasBeenSignaled)
							{
								InvokeOutputBreakErrorEvent();
								isOutputBreakOldAndErrorHasBeenSignaled = true;
							}
						}
						else
						{
							isOutputBreakOldAndErrorHasBeenSignaled = false;
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the port!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}
			catch (IOException ex) // The best way to detect a disconnected device is handling this exception...
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has detected shutdown of port.");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#if (!DEBUG)
			catch (Exception ex) // This best-effort approach shall only be done for RELEASE, in order to better detect and analyze the issue during DEBUG.
			{
				DebugEx.WriteException(GetType(), ex, "SendThread() has caught an unexpected exception! Restarting the port to try fixing the issue...");
				RestartOrResetPortAndThreadsAfterExceptionAndNotify();
			}
		#endif

			DebugThreadState("SendThread() has terminated.");
		}

		private bool TryWriteXOnOrXOffAndNotify(byte b, out bool isWriteTimeout, out bool isOutputBreak)
		{
			bool signalIOControlChanged;
			DateTime signalIOControlChangedTimeStamp;

			if (TryWriteByteToPort(b, out isWriteTimeout, out isOutputBreak, out signalIOControlChanged, out signalIOControlChangedTimeStamp))
			{
				OnDataSent(new SerialDataSentEventArgs(b, PortId)); // Skip I/O synchronization for simplicity.

				if (signalIOControlChanged)
					OnIOControlChanged(new EventArgs<DateTime>(signalIOControlChangedTimeStamp));

				return (true);
			}

			return (false);
		}

		/// <remarks>
		/// Attention, sending a whole chunk is implemented in <see cref="TryWriteChunkToPort"/> below.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool TryWriteByteToPort(byte b, out bool isWriteTimeout, out bool isOutputBreak, out bool signalIOControlChanged, out DateTime signalIOControlChangedTimeStamp)
		{
			isWriteTimeout         = false;
			isOutputBreak          = false;
			signalIOControlChanged = false;
			signalIOControlChangedTimeStamp = DateTime.MinValue; // Initial value is only kept on failure; value will be set to 'DateTime.Now' on success.

			bool writeSuccess      = false;
			Exception unhandled    = null;

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
			////Flush(0) is not required, flushing will happen after each transmission.

				this.port.RtsEnable = true;
			}

			// Note the remark in SerialPort.Write():
			// "If there are too many bytes in the output buffer and 'Handshake' is set to
			//  'XOnXOff' then the 'SerialPort' object may raise a 'TimeoutException' while
			//  it waits for the device to be ready to accept more data."

			try
			{
				DebugTransmission("Trying to write 1 byte to port...");

				// Try to write the byte, will raise a 'TimeoutException' if not possible:
				byte[] a = { b };
				this.port.Write(a, 0, 1); // Do not lock, may take some time!
				writeSuccess = true;

				DebugTransmission("...writing done.");

				// Handle XOn/XOff if required:
				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
				{
					if (XOnXOff.IsXOnOrXOffByte(b))
					{
						this.iXOnXOffHelper.XOnOrXOffSent(b);
						signalIOControlChanged = true; // XOn/XOff count has changed.
						signalIOControlChangedTimeStamp = DateTime.Now;
					}
				}
			}
			catch (TimeoutException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Timeout while writing to port!");
				isWriteTimeout = true;

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
					this.iXOnXOffHelper.OutputIsXOn = false;
			}
			catch (InvalidOperationException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Invalid operation while writing to port!");
				isOutputBreak = true;
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Unspecified error while writing to port!");
				unhandled = ex;
			}

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				Flush(1); // Single byte/frame.

				this.port.RtsEnable = false;
			}

			if (unhandled != null)
			{
				throw (unhandled); // Re-throw unhandled exception after restoring RTS.
			}

			return (writeSuccess);
		}

		/// <remarks>
		/// Attention, sending a single byte is implemented in <see cref="TryWriteByteToPort"/> above.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool TryWriteChunkToPort(int maxChunkSize, out List<byte> effectiveChunkData, out bool isWriteTimeout, out bool isOutputBreak, out bool signalIOControlChanged, out DateTime signalIOControlChangedTimeStamp)
		{
			isWriteTimeout         = false;
			isOutputBreak          = false;
			signalIOControlChanged = false;
			signalIOControlChangedTimeStamp = DateTime.MinValue; // Initial value is only kept on failure; value will be set to 'DateTime.Now' on success.

			bool writeSuccess      = false;
			Exception unhandled    = null;

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
			////Flush(0) is not required, flushing will happen after each transmission.

				this.port.RtsEnable = true;
			}

			// Note the remark in SerialPort.Write():
			// "If there are too many bytes in the output buffer and 'Handshake' is set to
			//  'XOnXOff' then the 'SerialPort' object may raise a 'TimeoutException' while
			//  it waits for the device to be ready to accept more data."

			try
			{
				// Retrieve chunk from the send queue. Retrieve and send as a whole to improve speed.
				// If sending fails, the port is either blocked by XOff or CTS, or closed.

				byte[] a;
				lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
				{
					a = this.sendQueue.ToArray();
				}

				int triedChunkSize = Math.Min(maxChunkSize, a.Length);
				effectiveChunkData = new List<byte>(triedChunkSize);

				DebugTransmission("Trying to write " + triedChunkSize + " byte(s) to port...");

				// Try to write the chunk, will raise a 'TimeoutException' if not possible:
				this.port.Write(a, 0, triedChunkSize); // Do not lock, may take some time!
				writeSuccess = true;

				DebugTransmission("...writing done.");

				// Finalize the write operation:
				lock (this.sendQueue) // Lock is required because Queue<T> is not synchronized.
				{
					for (int i = 0; i < triedChunkSize; i++)
					{
						byte b = this.sendQueue.Dequeue(); // Dequeue the chunk to acknowlege it's gone.

						effectiveChunkData.Add(b);

						// Handle XOn/XOff if required:
						if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
						{
							if (XOnXOff.IsXOnOrXOffByte(b))
							{
								this.iXOnXOffHelper.XOnOrXOffSent(b);
								signalIOControlChanged = true; // XOn/XOff count has changed.
								signalIOControlChangedTimeStamp = DateTime.Now;
							}
						}
					}
				}

				// Ensure not to lock the queue while potentially pending in Write(), that would
				// result in a severe performance drop because enqueuing was no longer possible.
			}
			catch (TimeoutException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Timeout while writing to port!");
				effectiveChunkData = null;
				isWriteTimeout = true;

				if (this.settings.Communication.FlowControlManagesXOnXOffManually) // XOn/XOff information is not available for 'Software' or 'Combined'!
					this.iXOnXOffHelper.OutputIsXOn = false;
			}
			catch (InvalidOperationException ex)
			{
				DebugEx.WriteException(GetType(), ex, "Invalid operation while writing to port!");
				effectiveChunkData = null;
				isOutputBreak = true;
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Unspecified error while writing to port!");
				effectiveChunkData = null;
				unhandled = ex;
			}

			if (this.settings.Communication.FlowControl == SerialFlowControl.RS485)
			{
				int maxFramesInFifo = 0;
				if (effectiveChunkData != null)
					maxFramesInFifo = Math.Min(effectiveChunkData.Count, 16); // Max 16 bytes/frames in FIFO.

				Flush(maxFramesInFifo);

				this.port.RtsEnable = false;
			}

			if (unhandled != null)
			{
				throw (unhandled); // Re-throw unhandled exception after restoring RTS.
			}

			return (writeSuccess);
		}

		private void InvokeOutputBreakErrorEvent()
		{
			OnIOError
			(
				new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"Output break state, retaining data..."
				)
			);
		}

		private void InvokeCtsInactiveErrorEvent()
		{
			OnIOError
			(
				new IOErrorEventArgs
				(
					ErrorSeverity.Acceptable,
					Direction.Output,
					"CTS inactive, retaining data..."
				)
			);
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_SEND_REQUEST")]
		private void DebugSendRequest(string message)
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
