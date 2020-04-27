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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of send:
////#define DEBUG_SEND

#endif // DEBUG

#endregion

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
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="Terminal"/>.
	/// </remarks>
	/// <remarks>
	///
	/// Design Criteria
	/// ---------------
	///
	/// Decoupling from calling thread (typically the main thread) is required for:
	///  > All below calling sequences (in order to not potentially block the main thread).
	///
	/// Sequencing is required for the following calling sequences, because all...
	///  >                            [ByteArray > Item  > Raw] ...chunks of a byte array...
	///  >                [Line/LinePart/EOL/... > Items > Raw] ...chunks of a line/linepart/EOL...
	///  >        [Lines > Line                  > Items > Raw] ...lines of a multi-line command...
	///  > [File > Lines > Line                  > Items > Raw] ...lines of a file...
	///                                                         ...must be sequential.
	/// At the same time, concurrency may make sense when the following keywords are active, because...
	///  > <code>\!(Delay(...))</code>
	///  > <code>\!(LineDelay(...))</code>
	///  > <code>\!(LineInterval(...))</code>                   ...any kind of delaying must not block other commands...
	///  > <code>\!(LineRepeat(...))</code>                     ...same as any kind of repeating...
	/// ...as well as sending a command while a file is being sent.
	///
	/// Furthermore, commands shall be "kept together", no matter whether single- or multi-line text or file.
	///
	/// Potential Appproaches
	/// ---------------------
	///
	/// 0. Implementation as up to YAT 2.1.0:
	///     >                                                             [ByteArray > sendDataQueue > SendDataThread > Item  > Raw]
	///     >                                                 [Line/LinePart/EOL/... > sendDataQueue > SendDataThread > Items > Raw]
	///     >                                         [Lines > Line                  > sendDataQueue > SendDataThread > Items > Raw]
	///     > [File > sendFileQueue > SendFileThread > Lines > Line                  > sendDataQueue > SendDataThread > Items > Raw]
	///       +/- Purely sequential, but not capable of concurrent sending.
	///       +/- Implementation relies on non-concurrent sending, no prevention of line(s) being requested while file is ongoing.
	/// 1. Invocation only when needed:
	///     > Same as above, but <code>\!(LineRepeat(...))</code> results in Invoke().
	///        +  Good enough in most cases.
	///        +  Easy upgrade path from implementation as up to YAT 2.1.0.
	///        -  Inconsistent.
	///       --- An embedded e.g. <code>\!(LineRepeat(5))</code> will no longer be sequential with subsequent lines!
	/// 2. Immediate invocation per request:
	///     >                                                  [ByteArray > Invoke() > Item  > Raw]
	///     >                                      [Line/LinePart/EOL/... > Invoke() > Items > Raw]
	///     >                   [Lines > Invoke() > Line                             > Items > Raw]
	///     > [File > Invoke() > Lines            > Line                             > Items > Raw]
	///       --- Invoke() doesn't guarantee sequence! Quickly requesting two different commands,
	///           e.g. by quickly clicking two buttons, may result in wrong command order!
	///       --- Requirement of commands being "kept together" is no longer met!
	/// 3. Immediate invocation per request with addition sequence number handling and locks:
	///     >                                                                                                  [ByteArray > Invoke(seqNum) > WaitFor(seqNum) > Item  > Confirm(seqNum) > Raw]
	///     >                                                                                      [Line/LinePart/EOL/... > Invoke(seqNum) > WaitFor(seqNum) > Items > Confirm(seqNum) > Raw]
	///     >                                           [Lines > Invoke(seqNum) > WaitFor(seqNum) > Line                  > Invoke(seqNum) > WaitFor(seqNum) > Items > Confirm(seqNum) > Raw]
	///     > [File > Invoke(seqNum) > WaitFor(seqNum) > Lines                                    > Line                  > Invoke(seqNum) > WaitFor(seqNum) > Items > Confirm(seqNum) > Raw]
	///        +  No longer need for two subsequent queues in case of file sending.
	///        +  Purely sequential implementation for each request, should also be relatively easy to debug.
	///       +++ All requirements met, configurable behavior implementable.
	///        -  Requires quite some refactoring...
	///
	/// Conclusion
	/// ----------
	///
	/// > The sum of all requirements conflict with each other, especially the request to allow concurrent sending.
	/// > An additional [Settings... > Advanced... > Send > Allow concurrent sending] is required.
	/// > Only approach 3. fulfills the requirements.
	///
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

		/// <summary>
		/// The sequence number of the next send request.
		/// </summary>
		/// <remarks>
		/// The number is needed to properly sequence send requests. See design consideration above.
		/// </remarks>
		/// <remarks>
		/// "A\!(LineRepeat)" throughput is approx. 5'000 lines per second (YAT 2.1.0). This will
		/// also be the uppermost limit when sending different commands from a script. Using an
		/// enormous safety margin of 100'000 lines per second this results in a worst case of
		/// 3'153'600'000'000 lines per year (31'536'000‬ seconds per year). Thus, <see cref="int"/>
		/// is not good enough, but <see cref="long"/> is more than good enough, its 9E+18 results
		/// in approx. 300E+9 years!
		///
		/// Note that <see cref="Interlocked.Increment(ref long)"/> is used for incrementing. That
		/// method will "handle an overflow condition by wrapping" and "no exception is thrown".
		/// Such loop around would not work in the (unlikely) case where a repeating command e.g.
		/// has sequence number 1 and single-line commands loop around. But since this will not
		/// happen in magnitude of years this case is neglected for ease of implementation.
		///
		/// Also note that <see cref="ulong"/> is not used since <see cref="Interlocked"/> only
		/// supports <see cref="long"/>.
		/// </remarks>
		private long previousRequestedSequenceNumber = -1;
		private long nextPermittedSequenceNumber; // = 0;
		private ManualResetEvent nextPermittedSequenceNumberEvent = new ManualResetEvent(false);

		private bool sendThreadsArePermitted;

		private int sendingIsOngoingCount; // = 0;
		private object sendingIsOngoingCountSyncObj = new object();
		private int sendingIsBusyCount; // = 0;
		private object sendingIsBusyCountSyncObj = new object();

		private ManualResetEvent packetGateEvent = new ManualResetEvent(false);
		private object packetGateSyncObj = new object();

		private bool breakState;
		private object breakStateSyncObj = new object();

		private System.Timers.Timer periodicXOnTimer;

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (IsTransmissive);

				// Until YAT 2.1.0, this property was implemented as 'IsReadyToSend_Internal'
				// as (IsTransmissive && !this.sendingIsOngoing) and it also signalled
				// 'EventMustBeRaisedBecauseStatusHasBeenAccessed()'. This was necessary as
				// until YAT 2.1.0 it was not possible to run multiple commands concurrently.
				// With YAT 2.1.1 this became possible, but keeping this property because its
				// meaning still makes sense, e.g. send related controls can adapt to this send
				// specific property instead of using the more general 'IsTransmissive'.
			}
		}

		/// <summary></summary>
		public virtual bool SendingIsOngoing
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (IsTransmissive && (this.sendingIsOngoingCount > 0)); // No need to lock (this.sendingIsOngoingSyncObj), retrieving only.
			}
		}

		/// <remarks>
		/// Opposed to <see cref="SendingIsOngoing"/>, this property only becomes <c>true</c> when
		/// sending has been ongoing for more than <see cref="SendingIsBusyChangedEventHelper.Threshold"/>,
		/// or is about to be ongoing for more than <see cref="SendingIsBusyChangedEventHelper.Threshold"/>.
		/// </remarks>
		public virtual bool SendingIsBusy
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (SendingIsOngoing && (this.sendingIsBusyCount > 0)); // No need to lock (this.sendingIsOngoingSyncObj), retrieving only.
			}
		}

		/// <summary>
		/// Returns the current break state.
		/// </summary>
		public virtual bool BreakState
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				lock (this.breakStateSyncObj)
					return (this.breakState);
			}
		}

		/// <remarks>
		/// Break if requested or terminal has stopped or closed.
		/// </remarks>
		protected virtual bool DoBreak
		{
			get
			{
				return (BreakState || !(!IsDisposed && this.sendThreadsArePermitted && IsTransmissive)); // Check 'IsDisposed' first!
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber);
			var asyncInvoker = new Action<byte[], long>(DoSend);
			asyncInvoker.BeginInvoke(data, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSend(byte[] data, long sequenceNumber)
		{
			DebugSend(string.Format("Sending of {0} bytes of raw data has been invoked with sequence number {1}.", data.Length, sequenceNumber));

			if (TryEnterRequestGate(sequenceNumber))
			{
				try
				{
					DebugSend(string.Format("Sending of {0} bytes of raw data has been permitted (sequence number = {1}).", data.Length, sequenceNumber));

					var sendingIsBusyChangedEvent = new SendingIsBusyChangedEventHelper(DateTime.Now);
					DoSendPre(sendingIsBusyChangedEvent.EventMustBeRaised); // Always false for raw data. If needed,
					DoSendRawData(sendingIsBusyChangedEvent, data);         // event will be raised by DoSendRawData().
					DoSendPost(sendingIsBusyChangedEvent.EventMustBeRaised);

					DebugSend(string.Format("Sending of {0} bytes of raw data has been completed (sequence number = {1}).", data.Length, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendText(string data, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertNotDisposed();

			var parseMode = TerminalSettings.Send.Text.ToParseMode(); // Get setting at request/invocation.
			var item = new TextSendItem(data, defaultRadix, parseMode, SendMode.Text, false);

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber);
			var asyncInvoker = new Action<TextSendItem, long>(DoSendText);
			asyncInvoker.BeginInvoke(item, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendText(TextSendItem item, long sequenceNumber)
		{
			DebugSend(string.Format(@"Sending of text ""{0}"" has been invoked (sequence number = {1}).", item.Data, sequenceNumber));

			if (TryEnterRequestGate(sequenceNumber))
			{
				try
				{
					DebugSend(string.Format(@"Sending of text ""{0}"" has been permitted (sequence number = {1}).", item.Data, sequenceNumber));

					var sendingIsBusyChangedEvent = new SendingIsBusyChangedEventHelper(DateTime.Now);
					DoSendPre(sendingIsBusyChangedEvent.EventMustBeRaised); // Always false for text items. If needed,
					DoSendTextItem(sendingIsBusyChangedEvent, item);        // event will be raised by DoSendTextItem().
					DoSendPost(sendingIsBusyChangedEvent.EventMustBeRaised);

					DebugSend(string.Format(@"Sending of text ""{0}"" has been completed (sequence number = {1}).", item.Data, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLine(string dataLine, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertNotDisposed();

			var parseMode = TerminalSettings.Send.Text.ToParseMode(); // Get setting at request/invocation.
			var item = new TextSendItem(dataLine, defaultRadix, parseMode, SendMode.Text, true);

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber);
			var asyncInvoker = new Action<TextSendItem, long>(DoSendTextLine);
			asyncInvoker.BeginInvoke(item, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendTextLine(TextSendItem item, long sequenceNumber)
		{
			DebugSend(string.Format(@"Sending of text line ""{0}"" has been invoked (sequence number = {1}).", item.Data, sequenceNumber));

			if (TryEnterRequestGate(sequenceNumber))
			{
				try
				{
					DebugSend(string.Format(@"Sending of text line ""{0}"" has been permitted (sequence number = {1}).", item.Data, sequenceNumber));

					var sendingIsBusyChangedEvent = new SendingIsBusyChangedEventHelper(DateTime.Now);
					DoSendPre(sendingIsBusyChangedEvent.EventMustBeRaised); // Always false for text items. If needed,
					DoSendTextItem(sendingIsBusyChangedEvent, item);        // event will be raised by DoSendTextItem().
					DoSendPost(sendingIsBusyChangedEvent.EventMustBeRaised);

					DebugSend(string.Format(@"Sending of text line ""{0}"" has been completed (sequence number = {1}).", item.Data, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}
		}

		/// <remarks>
		/// Required to allow sending multi-line commands "kept together".
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLines(string[] dataLines, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertNotDisposed();

			var parseMode = TerminalSettings.Send.Text.ToParseMode(); // Get setting at request/invocation.
			var items = new List<TextSendItem>(dataLines.Length); // Preset the required capacity to improve memory management.
			foreach (string dataLine in dataLines)
				items.Add(new TextSendItem(dataLine, defaultRadix, parseMode, SendMode.Text, true));

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber);
			var asyncInvoker = new Action<List<TextSendItem>, long>(DoSendTextLines);
			asyncInvoker.BeginInvoke(items, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendTextLines(List<TextSendItem> items, long sequenceNumber)
		{
			DebugSend(string.Format("Sending of {0} text lines has been invoked (sequence number = {1}).", items.Count, sequenceNumber));

			if (TryEnterRequestGate(sequenceNumber))
			{
				try
				{
					DebugSend(string.Format("Sending of {0} text lines has been permitted (sequence number = {1}).", items.Count, sequenceNumber));

					var sendingIsBusyChangedEvent = new SendingIsBusyChangedEventHelper(DateTime.Now);
					DoSendPre(sendingIsBusyChangedEvent.EventMustBeRaised); // Always false for text items. If needed,
					                                                      //// event will be raised by DoSendTextItem().
					foreach (var item in items)
						DoSendTextItem(sendingIsBusyChangedEvent, item);

					DoSendPost(sendingIsBusyChangedEvent.EventMustBeRaised);

					DebugSend(string.Format("Sending of {0} text lines has been completed (sequence number = {1}).", items.Count, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendFile(string filePath, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertNotDisposed();

			var item = new FileSendItem(filePath, defaultRadix);

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber);
			var asyncInvoker = new Action<FileSendItem, long>(DoSendFile);
			asyncInvoker.BeginInvoke(item, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendFile(FileSendItem item, long sequenceNumber)
		{
			DebugSend(string.Format(@"Sending of ""{0}"" has been invoked (sequence number = {1}).", item.FilePath, sequenceNumber));

			if (TryEnterRequestGate(sequenceNumber))
			{
				try
				{
					DebugSend(string.Format(@"Sending of ""{0}"" has been permitted (sequence number = {1}).", item.FilePath, sequenceNumber));

					var sendingIsBusyChangedEvent = new SendingIsBusyChangedEventHelper(DateTime.Now);
					DoSendPre(sendingIsBusyChangedEvent.EventMustBeRaised); // Always false for file items. If needed,
					DoSendFileItem(sendingIsBusyChangedEvent, item);        // event will be raised by DoSendFileItem().
					DoSendPost(sendingIsBusyChangedEvent.EventMustBeRaised);

					DebugSend(string.Format(@"Sending of ""{0}"" has been completed (sequence number = {1}).", item.FilePath, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary>
		/// Disposes the processing state.
		/// </summary>
		protected virtual void DisposeSend()
		{
			this.nextPermittedSequenceNumberEvent?.Dispose();
			this.nextPermittedSequenceNumberEvent = null;

			this.packetGateEvent?.Dispose();
			this.packetGateEvent = null;
		}

		/// <summary></summary>
		protected virtual void PermitSendThreads()
		{
			this.sendThreadsArePermitted = true;
		}

		/// <summary></summary>
		protected virtual void BreakSendThreads()
		{
			// Clear flag telling threads to stop...
			this.sendThreadsArePermitted = false;

			// ...then signal threads:
			this.nextPermittedSequenceNumberEvent?.Set(); // Handle 'null' because this method is called during
			this.packetGateEvent?.Set();                  // shutdown/dispose and may be called multiple times.
		}

		/// <summary></summary>
		protected virtual void DoSendPre(bool raiseSendingIsBusyChangedEvent)
		{
			// Each send request shall resume a pending break condition:
			ResumeBreak();

			if (TerminalSettings.Send.SignalXOnBeforeEachTransmission)
				RequestSignalInputXOn();

			if (raiseSendingIsBusyChangedEvent)
				OnThisRequestSendingIsBusyChanged(true);

			OnThisRequestSendingIsOngoingChanged(true);
		}

		/// <summary></summary>
		protected virtual void DoSendPost(bool raiseSendingIsBusyChangedEvent)
		{
			OnThisRequestSendingIsOngoingChanged(false);

			if (raiseSendingIsBusyChangedEvent)
				OnThisRequestSendingIsBusyChanged(false);
		}

		/// <remarks>
		/// Named "RawData" following "TextItem" terminology ("raw" vs. "text" and .
		/// <list type="bullet">
		/// <item><description>"Raw" instead of "Text".</description></item>
		/// <item><description>"Data" instead of "Item" as there is no 'RawItem'.</description></item>
		/// </list>
		/// </remarks>
		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendRawData(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, byte[] data)
		{
			// Raise the 'IOIsBusyChanged' event if a large chunk is about to be sent:
			if (sendingIsBusyChangedEventHelper.RaiseEventIfChunkSizeIsAboveThreshold(data.Length, this.terminalSettings.IO.RoughlyEstimatedMaxBytesPerMillisecond))
				OnThisRequestSendingIsBusyChanged(true);

			ForwardPacketToRawTerminal(data); // Nothing for further processing, simply forward.
		}

		/// <summary></summary>
		protected virtual bool DoTryParse(string textToParse, Radix radix, Parser.Mode parseMode, out Parser.Result[] parseResult, out string textSuccessfullyParsed)
		{
			using (var p = new Parser.Parser(TerminalSettings.IO.Endianness, parseMode))
				return (p.TryParse(textToParse, out parseResult, out textSuccessfullyParsed, radix));
		}

		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendTextItem(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, TextSendItem item)
		{
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;
			if (DoTryParse(item.Data, item.DefaultRadix, item.ParseMode, out parseResult, out textSuccessfullyParsed))
				DoSendText(sendingIsBusyChangedEventHelper, parseResult, item.IsLine);
			else
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(item.Data, textSuccessfullyParsed)));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected virtual void DoSendText(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, Parser.Result[] parseResult, bool isLine = false)
		{
			var performLineRepeat    = false; // \remind For binary terminals, this is rather a 'PacketRepeat'.
			var lineRepeatIsInfinite = (TerminalSettings.Send.DefaultLineRepeat == Settings.SendSettings.LineRepeatInfinite);
			var lineRepeatRemaining  =  TerminalSettings.Send.DefaultLineRepeat;
			var isFirstRepetition    = true;

			do // Process at least once, potentially repeat:
			{
				// --- Initialize the line/packet ---

				if (TryEnterPacketGate())
				{
					var lineBeginTimeStamp  = DateTime.Now;      // \remind For binary terminals, this is rather a 'PacketBegin'.
					var lineEndTimeStamp    = DateTime.MinValue; // \remind For binary terminals, this is rather a 'PacketBegin'.
					var performLineDelay    = false;             // \remind For binary terminals, this is rather a 'PacketDelay'.
					var lineDelay           = TerminalSettings.Send.DefaultLineDelay;
					var performLineInterval = false;             // \remind For binary terminals, this is rather a 'PacketInterval'.
					var lineInterval        = TerminalSettings.Send.DefaultLineInterval;
					var conflateDataQueue   = new Queue<byte>();

					try
					{
						// --- Process the line/packet ---

						foreach (var result in parseResult)
						{
							var doBreak = false;

							var bytesResult = (result as Parser.BytesResult);
							if (bytesResult != null)
							{
								// Raise the 'IOIsBusyChanged' event if a large chunk is about to be sent:
								if (sendingIsBusyChangedEventHelper.RaiseEventIfChunkSizeIsAboveThreshold(bytesResult.Bytes.Length, this.terminalSettings.IO.RoughlyEstimatedMaxBytesPerMillisecond))
									OnThisRequestSendingIsBusyChanged(true);

								// For performance reasons, as well as joining text terminal EOL with line content,
								// collect as many chunks as possible into a larger chunk:
								AppendToPendingPacketWithoutForwardingToRawTerminalYet(bytesResult.Bytes, conflateDataQueue);
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
											isLine = false;
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

									////case Parser.Keyword.Repeat: is yet pending (FR #13) and requires parser support for strings (FR #404).
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
											ProcessInLineKeywords(sendingIsBusyChangedEventHelper, keywordResult, conflateDataQueue, ref doBreak);

											break;
										}
									}
								}
							}

							if (DoBreak || doBreak) // (global || local)
								break;

							// Raise the 'IOIsBusyChanged' event if sending already takes quite long:
							if (sendingIsBusyChangedEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
								OnThisRequestSendingIsBusyChanged(true);
						}

						// --- Finalize the line/packet ---

						ProcessLineEnd(isLine, conflateDataQueue);

						lineEndTimeStamp = DateTime.Now; // \remind For binary terminals, this is rather a 'packetEndTimeStamp'.
					}
					finally
					{
						LeavePacketGate(); // Not the best approach to require this call at so many locations...
					}

					// --- Perform line/packet related post-processing ---

					// Break if requested or terminal has stopped or closed! Must be done prior to a potential Sleep() or repeat!
					if (DoBreak)
						break;

					ProcessLineDelayOrInterval(sendingIsBusyChangedEventHelper, performLineDelay, lineDelay, performLineInterval, lineInterval, lineBeginTimeStamp, lineEndTimeStamp);

					// Process repeat:
					if (!lineRepeatIsInfinite)
					{
						if (lineRepeatRemaining > 0)
							lineRepeatRemaining--;
					}

					isFirstRepetition = false;
				}
			}
			while (performLineRepeat && (lineRepeatIsInfinite || (lineRepeatRemaining > 0)));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Directly referring to given object for performance reasons.")]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InLine", Justification = "It's 'in line' and not inline!")]
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Agree, could be refactored. Could be.")]
		protected virtual void ProcessInLineKeywords(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, Parser.KeywordResult result, Queue<byte> conflateDataQueue, ref bool doBreakSend)
		{
			doBreakSend = false;

			switch (result.Keyword)
			{
				case Parser.Keyword.Clear:
				{
					ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
				////LeavePacketGate() must not be called, clearing is yet about to happen and packet shall resume afterwards.
					{
						// Wait some time to allow previous data being transmitted.
						// Wait quite long as the 'DataSent' event will take time.
						// This even has the advantage that data is quickly shown.
						Thread.Sleep(150);

						this.ClearRepositories();
					}
				////doBreakSend = !TryEnterPacketGate() must not be called, see above.

					break;
				}

				case Parser.Keyword.Delay:
				{
					ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
					LeavePacketGate();                                    // Not the best approach to require this call at so many locations...
					{
						int delay = TerminalSettings.Send.DefaultDelay;
						if (!ArrayEx.IsNullOrEmpty(result.Args))
							delay = result.Args[0];

						// Raise the 'IOIsBusyChanged' event if sending is about to be delayed:
						if (sendingIsBusyChangedEventHelper.RaiseEventIfDelayIsAboveThreshold(delay))
							OnThisRequestSendingIsBusyChanged(true);

						Thread.Sleep(delay);
					}
					doBreakSend = !TryEnterPacketGate();

					break;
				}

				case Parser.Keyword.TimeStamp:
				{
					var format = TerminalSettings.Display.TimeStampFormat;
					var useUtc = TerminalSettings.Display.TimeStampUseUtc;

				////if (!ArrayEx.IsNullOrEmpty(result.Args)) // with argument is yet pending (FR #400) and requires parser support for strings (FR #404).
				////	format = result.Args[0];

					var now = DateTime.Now;                                                        // No enclosure!
					var de = new DisplayElement.TimeStampInfo(now, Direction.Tx, format, useUtc, "", "");
					var text = de.Text;

					Parser.Result[] parseResult;
					string textSuccessfullyParsed;                 // A date/time format may conflict with YAT syntax.
					if (DoTryParse(text, Radix.String, Parser.Mode.NoEscapes, out parseResult, out textSuccessfullyParsed))
						AppendTimeStampToPendingPacketWithoutForwardingToRawTerminalYet(parseResult, conflateDataQueue);
					else
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(text, textSuccessfullyParsed)));

					break;
				}

				case Parser.Keyword.Port:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var settings = new MKY.IO.Serial.SerialPort.SerialPortSettings(TerminalSettings.IO.SerialPort); // Clone to ensure decoupling while
						settings.ClearChanged();                                                                        // changing and reapplying settings.

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.SerialPortId portId;
							if (MKY.IO.Ports.SerialPortId.TryFrom(result.Args[0], out portId))
								settings.PortId = portId;
						}

						if (settings.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryChangeSettingsOnTheFly(settings, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Bidir, "Changing the serial COM port has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "This keyword only applies to serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.PortSettings:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var settings = new MKY.IO.Serial.SerialPort.SerialPortSettings(TerminalSettings.IO.SerialPort); // Clone to ensure decoupling while
						settings.ClearChanged();                                                                        // changing and reapplying settings.

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							if (result.Args.Length > 0)
							{
								MKY.IO.Ports.BaudRateEx baudRate;
								if (MKY.IO.Ports.BaudRateEx.TryFrom(result.Args[0], out baudRate))
									settings.Communication.BaudRate = baudRate;

								if (result.Args.Length > 1)
								{
									MKY.IO.Ports.DataBitsEx dataBits;
									if (MKY.IO.Ports.DataBitsEx.TryFrom(result.Args[1], out dataBits))
										settings.Communication.DataBits = dataBits;

									if (result.Args.Length > 2)
									{
										MKY.IO.Ports.ParityEx parity;
										if (MKY.IO.Ports.ParityEx.TryFrom(result.Args[2], out parity))
											settings.Communication.Parity = parity;

										if (result.Args.Length > 3)
										{
											MKY.IO.Ports.StopBitsEx stopBits;   // 1.5 is not (yet) supported as the keyword args are limited to integer values (FR #404).
											if (MKY.IO.Ports.StopBitsEx.TryFrom(result.Args[3], out stopBits))
												settings.Communication.StopBits = stopBits;

											if (result.Args.Length > 4)
											{
												MKY.IO.Serial.SerialPort.SerialFlowControlEx flowControl;
												if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryFrom(result.Args[4], out flowControl))
													settings.Communication.FlowControl = flowControl;
											}
										}
									}
								}
							}
						}

						if (settings.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryChangeSettingsOnTheFly(settings, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Bidir, "Changing serial COM port settings has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "This keyword only applies to serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.Baud:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var settings = new MKY.IO.Serial.SerialPort.SerialPortSettings(TerminalSettings.IO.SerialPort); // Clone to ensure decoupling while
						settings.ClearChanged();                                                                        // changing and reapplying settings.

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.BaudRateEx baudRate;
							if (MKY.IO.Ports.BaudRateEx.TryFrom(result.Args[0], out baudRate))
								settings.Communication.BaudRate = baudRate;
						}

						if (settings.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryChangeSettingsOnTheFly(settings, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Bidir, "Changing baud rate has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Baud rate can only be changed on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.StopBits:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var settings = new MKY.IO.Serial.SerialPort.SerialPortSettings(TerminalSettings.IO.SerialPort); // Clone to ensure decoupling while
						settings.ClearChanged();                                                                        // changing and reapplying settings.

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.StopBitsEx stopBits;
							if (MKY.IO.Ports.StopBitsEx.TryFrom(result.Args[0], out stopBits))
								settings.Communication.StopBits = stopBits;
						}

						if (settings.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryChangeSettingsOnTheFly(settings, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Bidir, "Changing stop bits has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Stop bits can only be changed on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.DataBits:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var settings = new MKY.IO.Serial.SerialPort.SerialPortSettings(TerminalSettings.IO.SerialPort); // Clone to ensure decoupling while
						settings.ClearChanged();                                                                        // changing and reapplying settings.

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.DataBitsEx dataBits;
							if (MKY.IO.Ports.DataBitsEx.TryFrom(result.Args[0], out dataBits))
								settings.Communication.DataBits = dataBits;
						}

						if (settings.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryChangeSettingsOnTheFly(settings, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Bidir, "Changing data bits has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Data bits can only be changed on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.Parity:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var settings = new MKY.IO.Serial.SerialPort.SerialPortSettings(TerminalSettings.IO.SerialPort); // Clone to ensure decoupling while
						settings.ClearChanged();                                                                        // changing and reapplying settings.

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Ports.ParityEx parity;
							if (MKY.IO.Ports.ParityEx.TryFrom(result.Args[0], out parity))
								settings.Communication.Parity = parity;
						}

						if (settings.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryChangeSettingsOnTheFly(settings, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Bidir, "Changing parity has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Parity can only be changed on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.FlowControl:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						var settings = new MKY.IO.Serial.SerialPort.SerialPortSettings(TerminalSettings.IO.SerialPort); // Clone to ensure decoupling while
						settings.ClearChanged();                                                                        // changing and reapplying settings.

						if (!ArrayEx.IsNullOrEmpty(result.Args))
						{
							MKY.IO.Serial.SerialPort.SerialFlowControlEx flowControl;
							if (MKY.IO.Serial.SerialPort.SerialFlowControlEx.TryFrom(result.Args[0], out flowControl))
								settings.Communication.FlowControl = flowControl;
						}

						if (settings.Communication.HaveChanged)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

							Exception ex;
							if (!TryChangeSettingsOnTheFly(settings, out ex))
								InlineDisplayElement(IODirection.Bidir, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Bidir, "Changing flow control has failed! " + ex.Message));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Flow control can only be changed on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.FramingErrorsOn:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = false;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.FramingErrorsOff:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = true;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.FramingErrorsRestore:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.IgnoreFramingErrors = TerminalSettings.IO.SerialPort.IgnoreFramingErrors;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.OutputBreakOn:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = true;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Break is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.OutputBreakOff:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = false;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Break is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.OutputBreakToggle:
				{
					if (TerminalSettings.IO.IOType == IOType.SerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						var port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.ToggleOutputBreak();
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Break is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.ReportId:
				{
					if (TerminalSettings.IO.IOType == IOType.UsbSerialHid)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						byte reportId = TerminalSettings.IO.UsbSerialHidDevice.ReportFormat.Id;
						if (!ArrayEx.IsNullOrEmpty(result.Args))
							reportId = (byte)result.Args[0];

						var device = (MKY.IO.Usb.SerialHidDevice)this.UnderlyingIOInstance;
						device.ActiveReportId = reportId;
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Report ID can only be used with USB Ser/HID.", true));
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
		protected virtual void ProcessLineEnd(bool sendEol, Queue<byte> conflateDataQueue)
		{
			UnusedArg.PreventAnalysisWarning(sendEol); // Doesn't need to be handled for the 'neutral' terminal base.

			ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		protected virtual bool TryEnterRequestGate(long sequenceNumber)
		{
			while (!IsDisposed && this.sendThreadsArePermitted) // Check 'IsDisposed' first!
			{
				if (TerminalSettings.Send.AllowConcurrency)
				{
					return (true);
				}
				else
				{
					if (sequenceNumber == Interlocked.Read(ref this.nextPermittedSequenceNumber))
					{
						this.nextPermittedSequenceNumberEvent.Reset();
						return (true);
					}

					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						this.nextPermittedSequenceNumberEvent.WaitOne(staticRandom.Next(50, 200));
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in TryEnterSequenceGateAsRequired()!");
						break;
					}
				}
			}

			DebugSend(string.Format("TryEnterRequestGate() has determined to break because 'IsDisposed' = {0} / 'this.sendThreadsRunFlag' = {1}", IsDisposed, this.sendThreadsArePermitted));
			return (false);
		}

		/// <summary></summary>
		protected virtual void LeaveRequestGate()
		{
			if (!IsDisposed && this.sendThreadsArePermitted) // Check 'IsDisposed' first!
			{
				if (TerminalSettings.Send.AllowConcurrency)
				{
					// PENDING !!!

					// Nothing to do, no need to handle 'nextPermittedSequenceNumber' ?!?
					// If 'AllowConcurrency' gets disabled, will parent recreate the terminal ?!? No !!! But it probably must...
				}
				else
				{
					Interlocked.Increment(ref this.nextPermittedSequenceNumber);
					this.nextPermittedSequenceNumberEvent.Set();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		protected virtual bool TryEnterPacketGate()
		{
			while (!IsDisposed && this.sendThreadsArePermitted) // Check 'IsDisposed' first!
			{
				if (Monitor.TryEnter(this.packetGateSyncObj))
				{
					this.packetGateEvent.Reset();
					return (true);
				}

				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					this.packetGateEvent.WaitOne(staticRandom.Next(50, 200));
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in TryEnterSequenceGateAsRequired()!");
					break;
				}
			}

			DebugSend(string.Format("TryEnterPacketGate() has determined to break because 'IsDisposed' = {0} / 'this.sendThreadsRunFlag' = {1}", IsDisposed, this.sendThreadsArePermitted));
			return (false);
		}

		/// <summary></summary>
		protected virtual void LeavePacketGate()
		{
			Monitor.Exit(this.packetGateSyncObj);

			if (!IsDisposed && this.sendThreadsArePermitted) // Check 'IsDisposed' first!
				this.packetGateEvent.Set();
		}

		/// <remarks>For binary terminals, this is rather a 'ProcessPacketDelayOrInterval'.</remarks>
		protected virtual int ProcessLineDelayOrInterval(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, bool performLineDelay, int lineDelay, bool performLineInterval, int lineInterval, DateTime lineBeginTimeStamp, DateTime lineEndTimeStamp)
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
				// Raise the 'IOIsBusyChanged' event if sending is about to be delayed for too long:
				if (sendingIsBusyChangedEventHelper.RaiseEventIfDelayIsAboveThreshold(effectiveDelay))
					OnThisRequestSendingIsBusyChanged(true);

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
				sb.Append(                                    (successfullyParsed.Length + 1).ToString(CultureInfo.CurrentCulture) + "!");
				if (successfullyParsed.Length > 0)
				{
					sb.Append(                                           @" Only """);
					sb.Append(                                                     successfullyParsed);
					sb.Append(                                                                     @""" is valid.");
				}
			}
			else
			{
				sb.Append(            " is invalid!");
			}

			return (sb.ToString());
		}

		/// <remarks>Explicitly named 'TimeStamp' to make purpose more obvious. Could also be renamed to 'ByteResults'.</remarks>
		protected virtual void AppendTimeStampToPendingPacketWithoutForwardingToRawTerminalYet(Parser.Result[] parseResult, Queue<byte> conflateDataQueue)
		{
			lock (conflateDataQueue)
			{
				foreach (var result in parseResult)
				{
					var bytesResult = (result as Parser.BytesResult);
					if (bytesResult != null)
						AppendToPendingPacketWithoutForwardingToRawTerminalYet(bytesResult.Bytes, conflateDataQueue);
					else
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A time stamp to be appended must only consist of 'Parser.BytesResult'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected static void AppendToPendingPacketWithoutForwardingToRawTerminalYet(byte[] data, Queue<byte> conflateDataQueue)
		{
			lock (conflateDataQueue)
			{
				foreach (byte b in data)
					conflateDataQueue.Enqueue(b);
			}
		}

	////Kept because referred to by TextTerminal.ProcessInLineKeywords().
	////
	/////// <summary></summary>
	////protected virtual void AppendToPendingPacketAndForwardToRawTerminal(byte[] data, Queue<byte> conflateDataQueue)
	////{
	////	lock (conflateDataQueue)
	////	{
	////		foreach (byte b in data)
	////			conflateDataQueue.Enqueue(b);
	////	}
	////
	////	ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
	////}

		/// <remarks>
		/// Not the best approach to require to call this method at so many locations...
		/// </remarks>
		/// <remarks>
		/// Named 'packet' rather than 'chunk' to emphasize difference to <see cref="RawChunkSent"/>
		/// which corresponds to the chunks effectively sent by the underlying I/O instance.
		/// </remarks>
		protected virtual void ForwardPendingPacketToRawTerminal(Queue<byte> conflateDataQueue)
		{
			// Retrieve pending data:
			byte[] data;
			lock (conflateDataQueue)
			{
				data = conflateDataQueue.ToArray();
				conflateDataQueue.Clear();
			}

			ForwardPacketToRawTerminal(data);
		}

		/// <remarks>
		/// Named 'packet' rather than 'chunk' to emphasize difference to <see cref="RawChunkSent"/>
		/// which corresponds to the chunks effectively sent by the underlying I/O instance.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual void ForwardPacketToRawTerminal(byte[] data)
		{
		#if (WITH_SCRIPTING)
			// Invoke plug-in interface which potentially modifies the data or even cancels the whole packet:
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
		protected virtual bool TryChangeSettingsOnTheFly(MKY.IO.Serial.SerialPort.SerialPortSettings settings, out Exception exception)
		{
			var port = (this.UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
			if (port != null)
			{
				try
				{
					// Attention:
					// Similar code exists in Model.Terminal.ApplyTerminalSettings().
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
				}
				catch (Exception ex)
				{
					exception = ex;
					return (false);
				}
			}

			// Reflect the change in the settings:
			TerminalSettings.IO.SerialPort = settings;
			exception = null;
			return (true);
		}

		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected abstract void DoSendFileItem(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, FileSendItem item);

		/// <remarks>
		/// Text terminals are <see cref="Encoding"/> aware, binary terminals are not.
		/// </remarks>
		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendTextFileItem(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, FileSendItem item)
		{
			DoSendTextFileItem(sendingIsBusyChangedEventHelper, item, Encoding.Default);
		}

		/// <remarks>
		/// Text terminals are <see cref="Encoding"/> aware, binary terminals are not.
		/// </remarks>
		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendTextFileItem(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, FileSendItem item, Encoding encodingFallback)
		{
			using (var sr = new StreamReader(item.FilePath, encodingFallback, true))
			{                             // Automatically detect encoding from BOM, otherwise use fallback.
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
						continue;

					DoSendFileLine(sendingIsBusyChangedEventHelper, line, item.DefaultRadix);

					if (DoBreak)
						break;

					Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
				}
			}
		}

		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendXmlFileItem(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, FileSendItem item)
		{
			string[] lines;
			XmlReaderHelper.LinesFromFile(item.FilePath, out lines); // Read file at once for simplicity. Minor limitation:
			foreach (string line in lines)                           // 'sendingIsBusyChangedEventHelper.RaiseEventIf...' will
			{                                                        // only be evaluated at DoSendFileLine() below.
				if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
					continue;

				DoSendFileLine(sendingIsBusyChangedEventHelper, line, item.DefaultRadix);

				if (DoBreak)
					break;

				Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
			}
		}

		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendFileLine(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, string line, Radix defaultRadix)
		{
			// Raise the 'IOIsBusyChanged' event if sending already takes quite long, i.e. file cannot be sent within threshold:
			if (sendingIsBusyChangedEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
				OnThisRequestSendingIsBusyChanged(true);

			var parseMode = TerminalSettings.Send.File.ToParseMode();
			var item = new TextSendItem(line, defaultRadix, parseMode, SendMode.File, true);
			DoSendTextItem(sendingIsBusyChangedEventHelper, item);
		}

		/// <remarks>
		/// <see cref="FileSendItem.DefaultRadix"/> is not used for sending raw files.
		/// This fact is considered in 'View.Controls.SendFile.SetRecentAndCommandControls()'.
		/// Changes in behavior here will have to be adapted in that control method as well.
		/// </remarks>
		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendFileChunk(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, byte[] chunk)
		{
			// Raise the 'IOIsBusyChanged' event if sending already takes quite long, i.e. file cannot be sent within threshold:
			if (sendingIsBusyChangedEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
				OnThisRequestSendingIsBusyChanged(true);

			DoSendRawData(sendingIsBusyChangedEventHelper, chunk);
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

		#region Event Raising
		//------------------------------------------------------------------------------------------
		// Event Raising
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void OnThisRequestSendingIsOngoingChanged(bool value)
		{
			bool raiseEvent = false;

			lock (this.sendingIsOngoingCountSyncObj)
			{
				if (value)
					this.sendingIsOngoingCount++;
				else
					this.sendingIsOngoingCount--;

				if (this.sendingIsOngoingCount <= 0)
					raiseEvent = true;
			}

			if (raiseEvent)
				OnSendingIsOngoingChanged(new EventArgs<bool>(value));
		}

		/// <summary></summary>
		protected virtual void OnSendingIsOngoingChanged(EventArgs<bool> e)
		{
			this.eventHelper.RaiseSync<EventArgs<bool>>(SendingIsOngoingChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnThisRequestSendingIsBusyChanged(bool value)
		{
			bool raiseEvent = false;

			lock (this.sendingIsBusyCountSyncObj)
			{
				if (value)
					this.sendingIsBusyCount++;
				else
					this.sendingIsBusyCount--;

				if (this.sendingIsBusyCount <= 0)
					raiseEvent = true;
			}

			if (raiseEvent)
				OnSendingIsBusyChanged(new EventArgs<bool>(value));
		}

		/// <summary></summary>
		protected virtual void OnSendingIsBusyChanged(EventArgs<bool> e)
		{
			this.eventHelper.RaiseSync<EventArgs<bool>>(SendingIsBusyChanged, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG_SEND")]
		private void DebugSend(string message)
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
