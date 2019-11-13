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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using MKY;
using MKY.Diagnostics;

using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="Terminal"/>.
	/// </remarks>
	public abstract partial class Terminal : IDisposable, IDisposableEx
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Queue<DataSendItem> sendDataQueue = new Queue<DataSendItem>();
		private Queue<byte> conflateDataQueue = new Queue<byte>();
		private bool sendDataThreadRunFlag;
		private AutoResetEvent sendDataThreadEvent;
		private Thread sendDataThread;
		private object sendDataThreadSyncObj = new object();

		private Queue<FileSendItem> sendFileQueue = new Queue<FileSendItem>();
		private bool sendFileThreadRunFlag;
		private AutoResetEvent sendFileThreadEvent;
		private Thread sendFileThread;
		private object sendFileThreadSyncObj = new object();

		private bool sendingIsOngoing;

		private bool breakState;
		private object breakStateSyncObj = new object();

		private System.Timers.Timer periodicXOnTimer;

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		#region Break/Resume
		//------------------------------------------------------------------------------------------
		// Break/Resume
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the current break state.
		/// </summary>
		public virtual bool BreakState
		{
			get
			{
				lock (this.breakStateSyncObj)
					return (this.breakState);
			}
		}

		#endregion

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Send Data
		//------------------------------------------------------------------------------------------
		// Send Data
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			// AssertNotDisposed() is called by DoSendData().

			DoSendData(new RawDataSendItem(data));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendText(string data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.Text.ToParseMode();

			DoSendData(new TextDataSendItem(data, defaultRadix, parseMode, SendMode.Text, false));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLine(string dataLine, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.Text.ToParseMode();

			DoSendData(new TextDataSendItem(dataLine, defaultRadix, parseMode, SendMode.Text, true));
		}

		/// <remarks>
		/// Required to allow sending multi-line commands in a single operation. Otherwise, using
		/// <see cref="SendTextLine"/>, sending gets mixed-up because of the following sequence:
		///  1. First line gets sent/enqueued.
		///  2. Second line gets sent/enqueued.
		///  3. Response to first line is received and displayed
		///     and so on, mix-up among sent and received lines...
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLines(string[] dataLines, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.Text.ToParseMode();

			var l = new List<TextDataSendItem>(dataLines.Length); // Preset the required capacity to improve memory management.
			foreach (string dataLine in dataLines)
				l.Add(new TextDataSendItem(dataLine, defaultRadix, parseMode, SendMode.Text, true));

			DoSendData(l.ToArray());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public abstract void SendFileLine(string dataLine, Radix defaultRadix = Parser.Parser.DefaultRadixDefault);

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendDataItem"/> instead.
		/// </remarks>
		protected void DoSendData(DataSendItem item)
		{
			DoSendData(new DataSendItem[] { item });
		}

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendDataItem"/> instead.
		/// </remarks>
		protected void DoSendData(IEnumerable<DataSendItem> items)
		{
			AssertNotDisposed();

			// Each send request shall resume a pending break condition:
			ResumeBreak();

			if (TerminalSettings.Send.SignalXOnBeforeEachTransmission)
				RequestSignalInputXOn();

			// Enqueue the items for sending:
			lock (this.sendDataQueue) // Lock is required because Queue<T> is not synchronized.
			{
				foreach (var item in items)
					this.sendDataQueue.Enqueue(item);
			}

			// Signal thread:
			SignalSendDataThreadSafely();
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not invoked
		/// on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events. However,
		/// since the 'OnDisplayElements...' methods synchronously invoke the event, it will take
		/// some time until the send queue is checked again. During this time, no more new events
		/// are invoked, instead, outgoing data is buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="DoSendData(IEnumerable{DataSendItem})"/> above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendDataThread()
		{
			DebugThreadState("SendDataThread() has started.");

			try
			{
				// Outer loop, processes data after a signal was received:
				while (!IsDisposed && this.sendDataThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.sendDataThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendDataThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the send queue.
					// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.sendDataThreadRunFlag && IsReadyToSend_Internal && (this.sendDataQueue.Count > 0))
					{                                                                          // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						DataSendItem[] pendingItems;
						lock (this.sendDataQueue) // Lock is required because Queue<T> is not synchronized.
						{
							pendingItems = this.sendDataQueue.ToArray();
							this.sendDataQueue.Clear();
						}

						if (pendingItems.Length > 0)
						{
							this.ioChangedEventHelper.Initialize();
							this.sendingIsOngoing = true;

							foreach (var item in pendingItems)
							{
								DebugMessage(@"Processing item """ + item.ToString() + @""" of " + pendingItems.Length + " send item(s)...");

								ProcessSendDataItem(item);

								if (BreakSendData)
								{
									if (this.ioChangedEventHelper.EventMustBeRaised)
										OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.

									break;
								}

								// \remind (2017-09-16 / MKY) related to FR #262 "IIOProvider should be..."
								// In case of many pending items, 'EventMustBeRaised' will become 'true',
								// e.g. due to RaiseEventIfTotalTimeLagIsAboveThreshold(). This indicates
								// that there are really many pending items, and this foreach-loop would
								// result in kind of freezing all other threads => Yield!
								if (this.ioChangedEventHelper.EventMustBeRaised)
									Thread.Sleep(1); // Yield to other threads to e.g. allow refreshing of view.
							}                        // Note that Thread.Sleep(TimeSpan.Zero) is not sufficient.

							this.sendingIsOngoing = false;
							if (this.ioChangedEventHelper.EventMustBeRaised)
								OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendDataThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("SendDataThread() has terminated.");
		}

		/// <remarks>
		/// Break if requested or terminal has stopped or closed.
		/// </remarks>
		protected virtual bool BreakSendData
		{
			get
			{
				return (BreakState || !(!IsDisposed && this.sendDataThreadRunFlag && IsTransmissive)); // Check 'IsDisposed' first!
			}
		}

		/// <summary></summary>
		protected virtual void ProcessSendDataItem(DataSendItem item)
		{
			var rsi = (item as RawDataSendItem);
			if (rsi != null)
			{
				ProcessRawDataSendItem(rsi);
			}
			else
			{
				var psi = (item as TextDataSendItem);
				if (psi != null)
					ProcessTextDataSendItem(psi);
				else
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + item.GetType() + "' is a send item type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void ProcessRawDataSendItem(RawDataSendItem item)
		{
			ForwardPacketToRawTerminal(item.Data); // Nothing for further processing, simply forward.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
		protected virtual void ProcessTextDataSendItem(TextDataSendItem item)
		{
			bool hasSucceeded;
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;

			using (var p = new Parser.Parser(TerminalSettings.IO.Endianness, item.ParseMode))
				hasSucceeded = p.TryParse(item.Data, out parseResult, out textSuccessfullyParsed, item.DefaultRadix);

			if (hasSucceeded)
				ProcessParserResult(parseResult, item.IsLine);
			else
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(item.Data, textSuccessfullyParsed)));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected virtual void ProcessParserResult(Parser.Result[] results, bool sendEol = false)
		{
			bool performLineRepeat    = false; // \remind For binary terminals, this is rather a 'PacketRepeat'.
			bool lineRepeatIsInfinite = (TerminalSettings.Send.DefaultLineRepeat == Settings.SendSettings.LineRepeatInfinite);
			int  lineRepeatRemaining  =  TerminalSettings.Send.DefaultLineRepeat;
			bool isFirstRepetition    = true;

			do // Process at least once, potentially repeat.
			{
				// --- Initialize the line/packet ---

				DateTime lineBeginTimeStamp = DateTime.Now; // \remind For binary terminals, this is rather a 'PacketBegin'.
				bool performLineDelay       = false;        // \remind For binary terminals, this is rather a 'PacketDelay'.
				int lineDelay               = TerminalSettings.Send.DefaultLineDelay;
				bool performLineInterval    = false;        // \remind For binary terminals, this is rather a 'PacketInterval'.
				int lineInterval            = TerminalSettings.Send.DefaultLineInterval;

				// --- Process the line/packet ---

				foreach (var result in results)
				{
					var byteResult = (result as Parser.BytesResult);
					if (byteResult != null)
					{
						// Raise the 'IOChanged' event if a large chunk is about to be sent:
						if (this.ioChangedEventHelper.RaiseEventIfChunkSizeIsAboveThreshold(byteResult.Bytes.Length))
							OnIOChanged(EventArgs.Empty);

						// For performance reasons, collect as many chunks as possible into a larger chunk:
						AppendToPendingPacketWithoutForwardingToRawTerminalYet(byteResult.Bytes);
					}
					else // if keyword result (will not occur if keywords are disabled while parsing)
					{
						var keywordResult = (result as Parser.KeywordResult);
						if (keywordResult != null)
						{
							switch (keywordResult.Keyword)
							{
								// Process line related keywords:
								case Parser.Keyword.NoEol: // \remind Only needed for text terminals.
								{
									sendEol = false;
									break;
								}

								case Parser.Keyword.LineDelay: // \remind For binary terminals, this is rather a 'PacketDelay'.
								{
									performLineDelay = true;

									if (!ArrayEx.IsNullOrEmpty(keywordResult.Args)) // If arg is non-existant, the default value will be used.
										lineDelay = keywordResult.Args[0];

									break;
								}

								case Parser.Keyword.LineInterval: // \remind For binary terminals, this is rather a 'PacketInterval'.
								{
									performLineInterval = true;

									if (!ArrayEx.IsNullOrEmpty(keywordResult.Args)) // If arg is non-existant, the default value will be used.
										lineInterval = keywordResult.Args[0];

									break;
								}

							////case Parser.Keyword.Repeat: is yet pending (FR #13) and requires parser support for strings.
							////{
							////}

								case Parser.Keyword.LineRepeat: // \remind For binary terminals, this is rather a 'PacketRepeat'.
								{
									if (isFirstRepetition)
									{
										performLineRepeat = true;

										if (!ArrayEx.IsNullOrEmpty(keywordResult.Args)) // If arg is non-existant, the default value will be used.
										{
											lineRepeatIsInfinite = (keywordResult.Args[0] == Settings.SendSettings.LineRepeatInfinite);
											lineRepeatRemaining  =  keywordResult.Args[0];
										}
									}
									else
									{
										Thread.Sleep(TimeSpan.Zero); // Make sure the application stays responsive while repeating.
									}

									break;
								}

								// Process in-line keywords:
								default:
								{
									ProcessInLineKeywords(keywordResult);
									break;
								}
							}
						}
					}

					// Raise the 'IOChanged' event if sending already takes quite long:
					if (this.ioChangedEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
						OnIOChanged(EventArgs.Empty);
				}

				// --- Finalize the line/packet ---

				ProcessLineEnd(sendEol);

				DateTime lineEndTimeStamp = DateTime.Now; // \remind For binary terminals, this is rather a 'packetEndTimeStamp'.

				// --- Perform line/packet related post-processing ---

				// Break if requested or terminal has stopped or closed!
				// Note that breaking is done prior to a potential Sleep() or repeat.
				if (BreakState || !(!IsDisposed && this.sendDataThreadRunFlag && IsTransmissive)) // Check 'IsDisposed' first!
					break;

				ProcessLineDelayOrInterval(performLineDelay, lineDelay, performLineInterval, lineInterval, lineBeginTimeStamp, lineEndTimeStamp);

				// Process repeat:
				if (!lineRepeatIsInfinite)
				{
					if (lineRepeatRemaining > 0)
						lineRepeatRemaining--;
				}

				isFirstRepetition = false;
			}
			while (performLineRepeat && (lineRepeatIsInfinite || (lineRepeatRemaining > 0)));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InLine", Justification = "It's 'in line' and not inline!")]
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Agreed, could be refactored. Could be.")]
		protected virtual void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Clear:
				{
					ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

					// Wait some time to allow previous data being transmitted.
					// Wait quite long as the 'DataSent' event will take time.
					// This even has the advantage that data is quickly shown.
					Thread.Sleep(150);

					this.ClearRepositories();
					break;
				}

				case Parser.Keyword.Delay:
				{
					ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

					int delay = TerminalSettings.Send.DefaultDelay;
					if (!ArrayEx.IsNullOrEmpty(result.Args))
						delay = result.Args[0];

					// Raise the 'IOChanged' event if sending is about to be delayed:
					if (this.ioChangedEventHelper.RaiseEventIfDelayIsAboveThreshold(delay))
						OnIOChanged(EventArgs.Empty);

					Thread.Sleep(delay);
					break;
				}

				case Parser.Keyword.IOSettings:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							if (result.Args.Length > 0)
							{
								MKY.IO.Ports.BaudRateEx baudRate;
								if (MKY.IO.Ports.BaudRateEx.TryFrom(result.Args[0], out baudRate))
									setting.Communication.BaudRate = baudRate;

								if (result.Args.Length > 1)
								{
									MKY.IO.Ports.DataBitsEx dataBits;
									if (MKY.IO.Ports.DataBitsEx.TryFrom(result.Args[1], out dataBits))
										setting.Communication.DataBits = dataBits;

									if (result.Args.Length > 2)
									{
										MKY.IO.Ports.ParityEx parity;
										if (MKY.IO.Ports.ParityEx.TryFrom(result.Args[2], out parity))
											setting.Communication.Parity = parity;

										if (result.Args.Length > 3)
										{
											MKY.IO.Ports.StopBitsEx stopBits;   // 1.5 is not (yet) supported as the keyword args are limited to int.
											if (MKY.IO.Ports.StopBitsEx.TryFrom(result.Args[3], out stopBits))
												setting.Communication.StopBits = stopBits;

											if (result.Args.Length > 4)
											{
												MKY.IO.Serial.SerialPort.SerialFlowControlEx flowControl;
												if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryFrom(result.Args[4], out flowControl))
													setting.Communication.FlowControl = flowControl;
											}
										}
									}
								}
							}
						}

						if (setting.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing serial COM port settings has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Changing I/O settings is yet limited to serial COM ports (limitation #71).", true));
					}
					break;
				}

				case Parser.Keyword.Baud:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.BaudRateEx baudRate;
							if (MKY.IO.Ports.BaudRateEx.TryFrom(result.Args[0], out baudRate))
								setting.Communication.BaudRate = baudRate;
						}

						if (setting.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing baud rate has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Baud rate can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.StopBits:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.StopBitsEx stopBits;
							if (MKY.IO.Ports.StopBitsEx.TryFrom(result.Args[0], out stopBits))
								setting.Communication.StopBits = stopBits;
						}

						if (setting.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing stop bits has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Stop bits can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.DataBits:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.DataBitsEx dataBits;
							if (MKY.IO.Ports.DataBitsEx.TryFrom(result.Args[0], out dataBits))
								setting.Communication.DataBits = dataBits;
						}

						if (setting.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing data bits has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Data bits can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.Parity:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.ParityEx parity;
							if (MKY.IO.Ports.ParityEx.TryFrom(result.Args[0], out parity))
								setting.Communication.Parity = parity;
						}

						if (setting.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing parity has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Parity can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FlowControl:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var port = (MKY.IO.Serial.SerialPort.SerialPort)this.UnderlyingIOProvider;
						var setting = port.Settings;

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Serial.SerialPort.SerialFlowControlEx flowControl;
							if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryFrom(result.Args[0], out flowControl))
								setting.Communication.FlowControl = flowControl;
						}

						if (setting.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryApplySettings(port, setting, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(Direction.Bidir, "Changing flow control has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Flow control can only be changed on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FramingErrorsOn:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = false;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FramingErrorsOff:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = true;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.FramingErrorsRestore:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = TerminalSettings.IO.SerialPort.IgnoreFramingErrors;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.OutputBreakOn:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = true;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.OutputBreakOff:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = false;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.OutputBreakToggle:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.ToggleOutputBreak();
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Break is only supported on serial COM ports.", true));
					}
					break;
				}

				case Parser.Keyword.ReportId:
				{
					if (TerminalSettings.IO.IOType == IOType.UsbSerialHid)
					{
						ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...

						byte reportId = TerminalSettings.IO.UsbSerialHidDevice.ReportFormat.Id;
						if (!ArrayEx.IsNullOrEmpty(result.Args))
							reportId = (byte)result.Args[0];

						var device = (MKY.IO.Usb.SerialHidDevice)this.UnderlyingIOInstance;
						device.ActiveReportId = reportId;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, "Report ID can only be used with USB Ser/HID.", true));
					}
					break;
				}

				default: // = Unknown or not-yet-supported keyword.
				{
					InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, MessageHelper.InvalidExecutionPreamble + "The '" + (Parser.KeywordEx)result.Keyword + "' keyword is unknown! " + MessageHelper.SubmitBug));
					break;
				}
			}
		}

		/// <remarks>For binary terminals, this is rather a 'ProcessPacketEnd'.</remarks>
		protected virtual void ProcessLineEnd(bool sendEol)
		{
			UnusedArg.PreventAnalysisWarning(sendEol); // Doesn't need to be handled for the 'neutral' terminal base.

			ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...
		}

		/// <remarks>For binary terminals, this is rather a 'ProcessPacketDelayOrInterval'.</remarks>
		protected virtual int ProcessLineDelayOrInterval(bool performLineDelay, int lineDelay, bool performLineInterval, int lineInterval, DateTime lineBeginTimeStamp, DateTime lineEndTimeStamp)
		{
			int effectiveDelay = 0;

			if (performLineInterval) // 'Interval' has precendence over 'Delay' as it requires more accuracy.
			{
				var elapsed = (lineEndTimeStamp - lineBeginTimeStamp);
				effectiveDelay = lineInterval - (int)elapsed.TotalMilliseconds;
			}
			else if (performLineDelay)
			{
				effectiveDelay = lineDelay;
			}

			if (effectiveDelay > 0)
			{
				// Raise the 'IOChanged' event if sending is about to be delayed for too long:
				if (this.ioChangedEventHelper.RaiseEventIfDelayIsAboveThreshold(effectiveDelay))
					OnIOChanged(EventArgs.Empty);

				Thread.Sleep(effectiveDelay);
				return (effectiveDelay);
			}
			else
			{
				return (0);
			}
		}

		/// <summary>
		/// Creates a parser error message which can be displayed in the terminal.
		/// </summary>
		/// <param name="textToParse">The string to be parsed.</param>
		/// <param name="successfullyParsed">The substring that could successfully be parsed.</param>
		/// <returns>The error message to display.</returns>
		protected static string CreateParserErrorMessage(string textToParse, string successfullyParsed)
		{
			var sb = new StringBuilder();

			sb.Append(@"""");
			sb.Append(    textToParse);
			sb.Append(             @"""");
			if (successfullyParsed != null)
			{
				sb.Append(            " is invalid at position ");
				sb.Append(                                    (successfullyParsed.Length + 1).ToString(CultureInfo.CurrentCulture) + ".");
				if (successfullyParsed.Length > 0)
				{
					sb.Append(                                           @" Only """);
					sb.Append(                                                     successfullyParsed);
					sb.Append(                                                                     @""" is valid.");
				}
			}
			else
			{
				sb.Append(            " is invalid.");
			}

			return (sb.ToString());
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private member <see cref="conflateDataQueue"/>.
		/// </remarks>
		protected void AppendToPendingPacketWithoutForwardingToRawTerminalYet(byte[] data)
		{
			AssertNotDisposed();

			lock (this.conflateDataQueue)
			{
				foreach (byte b in data)
					this.conflateDataQueue.Enqueue(b);
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private member <see cref="conflateDataQueue"/>.
		/// </remarks>
		protected void AppendToPendingPacketAndForwardToRawTerminal(ReadOnlyCollection<byte> data)
		{
			// AssertNotDisposed() is called by 'AppendToPendingPacketAndForwardToRawTerminal()' below.

			byte[] a = new byte[data.Count];
			data.CopyTo(a, 0);

			AppendToPendingPacketAndForwardToRawTerminal(a);
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private member <see cref="conflateDataQueue"/>.
		/// </remarks>
		protected void AppendToPendingPacketAndForwardToRawTerminal(byte[] data)
		{
			// AssertNotDisposed() is called by 'ForwardPendingPacketToRawTerminal()' below.

			lock (this.conflateDataQueue)
			{
				foreach (byte b in data)
					this.conflateDataQueue.Enqueue(b);
			}

			ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...
		}

		/// <remarks>
		/// Not the best approach to require to call this method at so many locations...
		/// </remarks>
		/// <remarks>
		/// This method shall not be overridden as it accesses the private member <see cref="conflateDataQueue"/>.
		/// </remarks>
		protected void ForwardPendingPacketToRawTerminal()
		{
			// AssertNotDisposed() is called by 'ForwardPacketToRawTerminal()' below.

			// Retrieve pending data:
			byte[] data;
			lock (this.conflateDataQueue)
			{
				data = this.conflateDataQueue.ToArray();
				this.conflateDataQueue.Clear();
			}

			ForwardPacketToRawTerminal(data);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual void ForwardPacketToRawTerminal(byte[] data)
		{
		#if (WITH_SCRIPTING)
			// Invoke plug-in interface which potentially modifies the data or even cancels the packet:
			var e = new ModifiablePacketEventArgs(data);
			OnSendingPacket(e);
			if (e.Cancel)
				return;
		#endif

			// Forward packet to underlying terminal:
			try
			{
				this.rawTerminal.Send(data); // Forwards the potentially modified data.
			}
			catch (ThreadAbortException ex)
			{
				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				var sb = new StringBuilder();
				sb.AppendLine("'ThreadAbortException' while trying to forward data to the underlying RawTerminal.");
				sb.AppendLine("This exception is ignored as it can happen during closing of the terminal or application.");
				sb.AppendLine("Confirming the abort, i.e. Thread.ResetAbort() will be called...");
				sb.AppendLine();
				DebugEx.WriteException(GetType(), ex, sb.ToString());

				Thread.ResetAbort();
			}
			catch (ObjectDisposedException ex)
			{
				var sb = new StringBuilder();
				sb.AppendLine("'ObjectDisposedException' while trying to forward data to the underlying RawTerminal.");
				sb.AppendLine("This exception is ignored as it can happen during closing of the terminal or application.");
				sb.AppendLine();
				DebugEx.WriteException(GetType(), ex, sb.ToString());
			}
			catch (Exception ex)
			{
				var leadMessage = "Unable to send data:";
				DebugEx.WriteException(GetType(), ex, leadMessage);
				OnIOError(new IOErrorEventArgs(IOErrorSeverity.Fatal, IODirection.Tx, leadMessage + Environment.NewLine + ex.Message));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual bool TryApplySettings(MKY.IO.Serial.SerialPort.SerialPort port, MKY.IO.Serial.SerialPort.SerialPortSettings settings, out Exception exception)
		{
			try
			{
				// Attention:
				// Similar code exists in Model.Terminal.ApplySettings() but including change of terminal settings (.yat file).
				// Changes here may have to be applied there too.

				if (port.IsStarted) // Port is started, stop and restart it with the new settings:
				{
					port.Stop(); // Attention, do not Stop() the whole terminal as that will also stop the currently ongoing send thread!
					port.Settings = settings;
					port.Start();
				}
				else // Port is stopped, simply set the new settings:
				{
					port.Settings = settings;
				}

				exception = null;
				return (true);
			}
			catch (Exception ex)
			{
				exception = ex;
				return (false);
			}
		}

		#endregion

		#region Send File
		//------------------------------------------------------------------------------------------
		// Send File
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendFile(string filePath, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendFile().

			DoSendFile(filePath, defaultRadix);
		}

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendFileItem"/> instead.
		/// </remarks>
		/// <remarks>
		/// Separate "Do...()" method for symmetricity with <see cref="DoSendData(IEnumerable{DataSendItem})"/>.
		/// </remarks>
		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		protected void DoSendFile(string filePath, Radix defaultRadix)
		{
			AssertNotDisposed();

			// Enqueue the items for sending:
			lock (this.sendFileQueue) // Lock is required because Queue<T> is not synchronized.
			{
				this.sendFileQueue.Enqueue(new FileSendItem(filePath, defaultRadix));
			}

			// Signal thread:
			SignalSendFileThreadSafely();
		}

		/// <remarks>
		/// Will be signaled by <see cref="DoSendFile(string, Radix)"/> above.
		/// </remarks>
		/// <remarks>
		/// Separate thread (and not integrated into <see cref="SendDataThread"/>) because that
		/// thread queues <see cref="TextDataSendItem"/> objects, thus some kind of a two-level
		/// infrastructure is required (SendFile => SendData). The considered \!(SendFile("..."))
		/// keyword doesn't help either since the file may again contain keywords, thus again some
		/// kind of a two-level infrastructure is required.
		/// </remarks>
		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Threading.WaitHandle.#WaitOne(System.Int32)", Justification = "Installer indeed targets .NET 3.5 SP1.")]
		private void SendFileThread()
		{
			DebugThreadState("SendFileThread() has started.");

			try
			{
				// Outer loop, processes data after a signal was received:
				while (!IsDisposed && this.sendFileThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.sendFileThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendFileThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the send queue.
					// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.sendFileThreadRunFlag && IsReadyToSend_Internal && (this.sendFileQueue.Count > 0))
					{                                                                      // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue, since it is very
						// likely that more data is to be enqueued, thus resulting in larger chunks processed.
						// Subsequently, yield to other threads to allow processing the data.
						Thread.Sleep(TimeSpan.Zero);

						FileSendItem[] pendingItems;
						lock (this.sendFileQueue) // Lock is required because Queue<T> is not synchronized.
						{
							pendingItems = this.sendFileQueue.ToArray();
							this.sendFileQueue.Clear();
						}

						if (pendingItems.Length > 0)
						{
							foreach (var item in pendingItems)
							{
								DebugMessage(@"Processing item """ + item.ToString() + @""" of " + pendingItems.Length + " send item(s)...");

								ProcessSendFileItem(item);

								if (BreakSendFile)
								{
									OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
									break;
								}

								// \remind (2017-09-16 / MKY) related to FR #262 "IIOProvider should be..."
								// No need to yield here (like done in SendDataThread()) since...
								// ...it is very unlikely that very many files are sent at once.
								// ...and the for-loop in ProcessSendFileItem() already yields.
							}
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "SendFileThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("SendFileThread() has terminated.");
		}

		/// <remarks>
		/// Break if requested or terminal has stopped or closed.
		/// </remarks>
		protected virtual bool BreakSendFile
		{
			get
			{
				return (BreakState || !(!IsDisposed && this.sendFileThreadRunFlag && IsTransmissive)); // Check 'IsDisposed' first!
			}
		}

		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		protected abstract void ProcessSendFileItem(FileSendItem item);

		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		protected virtual void ProcessSendTextFileItem(FileSendItem item)
		{
			ProcessSendTextFileItem(item, Encoding.Default);
		}

		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		protected virtual void ProcessSendTextFileItem(FileSendItem item, Encoding encodingFallback)
		{
			using (var sr = new StreamReader(item.FilePath, encodingFallback, true))
			{                             // Automatically detect encoding from BOM, otherwise use fallback.
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
						continue;

					SendFileLine(line, item.DefaultRadix);

					if (BreakSendFile)
					{
						OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
						break;
					}

					Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
				}
			}
		}

		/// <summary></summary>
		protected virtual void ProcessSendXmlFileItem(FileSendItem item)
		{
			string[] lines;
			XmlReaderHelper.LinesFromFile(item.FilePath, out lines); // Read all at once for simplicity.
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
					continue;

				SendFileLine(line, item.DefaultRadix);

				if (BreakSendFile)
				{
					OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
					break;
				}

				Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
			}
		}

		#endregion

		#region Break/Resume
		//------------------------------------------------------------------------------------------
		// Break/Resume
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Breaks all currently ongoing operations in the terminal.
		/// </summary>
		public virtual void Break()
		{
			lock (this.breakStateSyncObj)
				this.breakState = true;
		}

		/// <summary>
		/// Resumes all currently suspended operations in the terminal.
		/// </summary>
		public virtual void ResumeBreak()
		{
			lock (this.breakStateSyncObj)
				this.breakState = false;
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Send Threads
		//------------------------------------------------------------------------------------------
		// Send Threads
		//------------------------------------------------------------------------------------------

		private void CreateAndStartSendThreads()
		{
			lock (this.sendDataThreadSyncObj)
			{
				DebugThreadState("SendDataThread() gets created...");

				if (this.sendDataThread == null)
				{
					this.sendDataThreadRunFlag = true;
					this.sendDataThreadEvent = new AutoResetEvent(false);
					this.sendDataThread = new Thread(new ThreadStart(SendDataThread));
					this.sendDataThread.Name = "Terminal [" + (1000 + this.instanceId) + "] Send Data Thread";
					this.sendDataThread.Start();  // Offset of 1000 to distinguish this ID from the 'real' terminal ID.

					DebugThreadState("...successfully created.");
				}
			#if (DEBUG)
				else
				{
					DebugThreadState("...failed as it already exists.");
				}
			#endif
			}

			lock (this.sendFileThreadSyncObj)
			{
				DebugThreadState("SendFileThread() gets created...");

				if (this.sendFileThread == null)
				{
					this.sendFileThreadRunFlag = true;
					this.sendFileThreadEvent = new AutoResetEvent(false);
					this.sendFileThread = new Thread(new ThreadStart(SendFileThread));
					this.sendFileThread.Name = "Terminal [" + (1000 + this.instanceId) + "] Send File Thread";
					this.sendFileThread.Start();  // Offset of 1000 to distinguish this ID from the 'real' terminal ID.

					DebugThreadState("...successfully created.");
				}
			#if (DEBUG)
				else
				{
					DebugThreadState("...failed as it already exists.");
				}
			#endif
			}
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopSendThreads()
		{
			lock (this.sendFileThreadSyncObj)
			{
				if (this.sendFileThread != null)
				{
					DebugThreadState("SendFileThread() gets stopped...");

					this.sendFileThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.sendFileThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendFileThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalSendFileThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.sendFileThread.Abort(); // This is only the fall-back in case joining fails for too long.
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

					this.sendFileThread = null;
				}
			#if (DEBUG)
				else // (this.sendFileThread == null)
				{
					DebugThreadState("...not necessary as it doesn't exist anymore.");
				}
			#endif

				if (this.sendFileThreadEvent != null)
				{
					try     { this.sendFileThreadEvent.Close(); }
					finally { this.sendFileThreadEvent = null; }
				}
			} // lock (sendFileThreadSyncObj)

			lock (this.sendDataThreadSyncObj)
			{
				if (this.sendDataThread != null)
				{
					DebugThreadState("SendDataThread() gets stopped...");

					this.sendDataThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.sendDataThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.sendDataThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalSendDataThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.sendDataThread.Abort(); // This is only the fall-back in case joining fails for too long.
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

					this.sendDataThread = null;
				}
			#if (DEBUG)
				else // (this.sendDataThread == null)
				{
					DebugThreadState("...not necessary as it doesn't exist anymore.");
				}
			#endif

				if (this.sendDataThreadEvent != null)
				{
					try     { this.sendDataThreadEvent.Close(); }
					finally { this.sendDataThreadEvent = null; }
				}
			} // lock (sendDataThreadSyncObj)
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendDataThreadSafely()
		{
			try
			{
				if (this.sendDataThreadEvent != null)
					this.sendDataThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalSendFileThreadSafely()
		{
			try
			{
				if (this.sendFileThreadEvent != null)
					this.sendFileThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
