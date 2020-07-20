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
// YAT Version 2.2.0 Development
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

	// Enable debugging of send related events:
////#define DEBUG_IS_SENDING

	// Enable debugging of break related properties and events:
////#define DEBUG_BREAK

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
	public abstract partial class Terminal
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

		/// <summary></summary>
		protected bool SendThreadsArePermitted { get; private set; }

		private int isSendingCount; // = 0;
		private object isSendingCountSyncObj = new object();
		private int isSendingForSomeTimeCount; // = 0;
		private object isSendingForSomeTimeCountSyncObj = new object();

		private ManualResetEvent packetGateEvent = new ManualResetEvent(false);
		private object packetGateSyncObj = new object();

		private bool breakState;
		private object breakStateSyncObj = new object();

		private Timer periodicXOnTimer;

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
			////AssertUndisposed() shall not be called from this simple get-property.

				if (TerminalSettings.Send.AllowConcurrency)
					return (IsTransmissive);
				else
					return (!IsSending);

				// Until YAT 2.1.0, this property was implemented as 'IsReadyToSend_Internal'
				// as (IsTransmissive && !IsSending) and it also signalled
				// 'EventMustBeRaisedBecauseStatusHasBeenAccessed()'. This was necessary as
				// until YAT 2.1.0 it was not possible to run multiple commands concurrently.
				// With YAT 2.1.1 this became possible, but keeping this property because its
				// meaning still makes sense, e.g. send related controls can adapt to this send
				// specific property instead of using the more general 'IsTransmissive'.
			}
		}

		/// <summary></summary>
		public virtual bool IsSending
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (IsTransmissive && (this.isSendingCount > 0)); // No need to lock (this.isSendingCountSyncObj), retrieving only.
			}
		}

		/// <remarks>
		/// Opposed to <see cref="IsSending"/>, this property only becomes <c>true</c> when
		/// sending has been ongoing for more than <see cref="ForSomeTimeEventHelper.Threshold"/>,
		/// or is about to be ongoing for more than <see cref="ForSomeTimeEventHelper.Threshold"/>.
		/// </remarks>
		public virtual bool IsSendingForSomeTime
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (IsSending && (this.isSendingForSomeTimeCount > 0)); // No need to lock (this.isSendingForSomeTimeCountSyncObj), retrieving only.
			}
		}

		/// <summary>
		/// Returns the current break state.
		/// </summary>
		public virtual bool BreakState
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
				if (IsUndisposed && SendThreadsArePermitted && IsTransmissive) // Check disposal state first!
					return (BreakState);
				else
					return (true); // Indicate to break in any case.
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
			AssertUndisposed();

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber); // Loop-around is OK, see remarks at variable definition.
			var asyncInvoker = new Action<byte[], long>(DoSend);
			asyncInvoker.BeginInvoke(data, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSend(byte[] data, long sequenceNumber)
		{
			DebugSend(string.Format("Sending of {0} byte(s) of raw data has been invoked with sequence number {1}.", data.Length, sequenceNumber));

			EnterRequestPre();

			var forSomeTimeEventHelper = new ForSomeTimeEventHelper(DateTime.Now);
			if (TryEnterRequestGate(forSomeTimeEventHelper, sequenceNumber)) // Note that behavior depends on the 'AllowConcurrency' setting.
			{
				try
				{
					DebugSend(string.Format("Sending of {0} byte(s) of raw data has been permitted (sequence number = {1}).", data.Length, sequenceNumber));

					DoSendPre();
					DoSendRawData(forSomeTimeEventHelper, data);
					DoSendPost();

					DebugSend(string.Format("Sending of {0} byte(s) of raw data has been completed (sequence number = {1}).", data.Length, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}

			LeaveRequestPost(forSomeTimeEventHelper.EventMustBeRaised); // This flag can only get set once.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendText(string text, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertUndisposed();

			var parseMode = TerminalSettings.Send.Text.ToParseMode(); // Get setting at request/invocation.
			var item = new TextSendItem(text, defaultRadix, parseMode, SendMode.Text, false);

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber); // Loop-around is OK, see remarks at variable definition.
			var asyncInvoker = new Action<TextSendItem, long>(DoSendText);
			asyncInvoker.BeginInvoke(item, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendText(TextSendItem item, long sequenceNumber)
		{
			DebugSend(string.Format(@"Sending of text ""{0}"" has been invoked (sequence number = {1}).", item.Text, sequenceNumber));

			EnterRequestPre();

			var forSomeTimeEventHelper = new ForSomeTimeEventHelper(DateTime.Now);
			if (TryEnterRequestGate(forSomeTimeEventHelper, sequenceNumber)) // Note that behavior depends on the 'AllowConcurrency' setting.
			{
				try
				{
					DebugSend(string.Format(@"Sending of text ""{0}"" has been permitted (sequence number = {1}).", item.Text, sequenceNumber));

					DoSendPre();
					DoSendTextItem(forSomeTimeEventHelper, item);
					DoSendPost();

					DebugSend(string.Format(@"Sending of text ""{0}"" has been completed (sequence number = {1}).", item.Text, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}

			LeaveRequestPost(forSomeTimeEventHelper.EventMustBeRaised); // This flag can only get set once.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLine(string line, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertUndisposed();

			var parseMode = TerminalSettings.Send.Text.ToParseMode(); // Get setting at request/invocation.
			var item = new TextSendItem(line, defaultRadix, parseMode, SendMode.Text, true);

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber); // Loop-around is OK, see remarks at variable definition.
			var asyncInvoker = new Action<TextSendItem, long>(DoSendTextLine);
			asyncInvoker.BeginInvoke(item, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendTextLine(TextSendItem item, long sequenceNumber)
		{
			DebugSend(string.Format(@"Sending of text line ""{0}"" has been invoked (sequence number = {1}).", item.Text, sequenceNumber));

			EnterRequestPre();

			var forSomeTimeEventHelper = new ForSomeTimeEventHelper(DateTime.Now);
			if (TryEnterRequestGate(forSomeTimeEventHelper, sequenceNumber)) // Note that behavior depends on the 'AllowConcurrency' setting.
			{
				try
				{
					DebugSend(string.Format(@"Sending of text line ""{0}"" has been permitted (sequence number = {1}).", item.Text, sequenceNumber));

					DoSendPre();
					DoSendTextItem(forSomeTimeEventHelper, item);
					DoSendPost();

					DebugSend(string.Format(@"Sending of text line ""{0}"" has been completed (sequence number = {1}).", item.Text, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}

			LeaveRequestPost(forSomeTimeEventHelper.EventMustBeRaised); // This flag can only get set once.
		}

		/// <remarks>
		/// Required to allow sending multi-line commands "kept together".
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendTextLines(string[] lines, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertUndisposed();

			var parseMode = TerminalSettings.Send.Text.ToParseMode(); // Get setting at request/invocation.
			var items = new List<TextSendItem>(lines.Length); // Preset the required capacity to improve memory management.
			foreach (string line in lines)
				items.Add(new TextSendItem(line, defaultRadix, parseMode, SendMode.Text, true));

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber); // Loop-around is OK, see remarks at variable definition.
			var asyncInvoker = new Action<List<TextSendItem>, long>(DoSendTextLines);
			asyncInvoker.BeginInvoke(items, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendTextLines(List<TextSendItem> items, long sequenceNumber)
		{
			DebugSend(string.Format("Sending of {0} text lines has been invoked (sequence number = {1}).", items.Count, sequenceNumber));

			EnterRequestPre();

			var forSomeTimeEventHelper = new ForSomeTimeEventHelper(DateTime.Now);
			if (TryEnterRequestGate(forSomeTimeEventHelper, sequenceNumber)) // Note that behavior depends on the 'AllowConcurrency' setting.
			{
				try
				{
					DebugSend(string.Format("Sending of {0} text lines has been permitted (sequence number = {1}).", items.Count, sequenceNumber));

					DoSendPre();

					foreach (var item in items)
						DoSendTextItem(forSomeTimeEventHelper, item);

					DoSendPost();

					DebugSend(string.Format("Sending of {0} text lines has been completed (sequence number = {1}).", items.Count, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}

			LeaveRequestPost(forSomeTimeEventHelper.EventMustBeRaised); // This flag can only get set once.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void SendFile(string filePath, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			AssertUndisposed();

			var item = new FileSendItem(filePath, defaultRadix);

			var sequenceNumber = Interlocked.Increment(ref this.previousRequestedSequenceNumber); // Loop-around is OK, see remarks at variable definition.
			var asyncInvoker = new Action<FileSendItem, long>(DoSendFile);
			asyncInvoker.BeginInvoke(item, sequenceNumber, null, null);
		}

		/// <remarks>This method will be called asynchronously.</remarks>
		protected virtual void DoSendFile(FileSendItem item, long sequenceNumber)
		{
			DebugSend(string.Format(@"Sending of ""{0}"" has been invoked (sequence number = {1}).", item.FilePath, sequenceNumber));

			EnterRequestPre();

			var forSomeTimeEventHelper = new ForSomeTimeEventHelper(DateTime.Now);
			if (TryEnterRequestGate(forSomeTimeEventHelper, sequenceNumber)) // Note that behavior depends on the 'AllowConcurrency' setting.
			{
				try
				{
					DebugSend(string.Format(@"Sending of ""{0}"" has been permitted (sequence number = {1}).", item.FilePath, sequenceNumber));

					DoSendPre();
					DoSendFileItem(forSomeTimeEventHelper, item);
					DoSendPost();

					DebugSend(string.Format(@"Sending of ""{0}"" has been completed (sequence number = {1}).", item.FilePath, sequenceNumber));
				}
				finally
				{
					LeaveRequestGate();
				}
			}

			LeaveRequestPost(forSomeTimeEventHelper.EventMustBeRaised); // This flag can only get set once.
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
			if (this.nextPermittedSequenceNumberEvent != null) {
				this.nextPermittedSequenceNumberEvent.Dispose();
				this.nextPermittedSequenceNumberEvent = null;
			}

			if (this.packetGateEvent != null) {
				this.packetGateEvent.Dispose();
				this.packetGateEvent = null;
			}
		}

		/// <summary></summary>
		protected virtual void PermitSendThreads()
		{
			SendThreadsArePermitted = true;
		}

		/// <summary></summary>
		protected virtual void BreakSendThreads()
		{
			// Clear flag telling threads to stop...
			SendThreadsArePermitted = false;

			// ...then signal threads:
			this.nextPermittedSequenceNumberEvent?.Set(); // Handle 'null' because this method is called during
			this.packetGateEvent?.Set();                  // shutdown/dispose and may be called multiple times.
		}

		/// <summary></summary>
		protected virtual void EnterRequestPre()
		{
			ResumeBreak(); // Each send request shall resume a pending break condition.

			IncrementIsSendingChanged();
		}

		/// <summary></summary>
		protected virtual void LeaveRequestPost(bool decrementIsSendingForSomeTime)
		{
			DecrementIsSendingChanged();

			if (decrementIsSendingForSomeTime)
				DecrementIsSendingForSomeTimeChanged();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		protected virtual bool TryEnterRequestGate(ForSomeTimeEventHelper forSomeTimeEventHelper, long sequenceNumber)
		{
			while (!DoBreak)
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

					if (forSomeTimeEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold()) // Signal wait operation if needed.
						IncrementIsSendingForSomeTimeChanged();

					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						this.nextPermittedSequenceNumberEvent.WaitOne(StaticRandom.Next(50, 200));
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

			DebugSend("TryEnterRequestGate() has determined to break");

			return (false);
		}

		/// <summary></summary>
		protected virtual void LeaveRequestGate()
		{
			if (IsUndisposed)
			{
				if (TerminalSettings.Send.AllowConcurrency)
				{
					// Nothing to do, no need to handle 'nextPermittedSequenceNumber', as changing
					// the 'AllowConcurrency' setting will lead to TerminalFactory.RecreateTerminal().
				}
				else
				{
					Interlocked.Increment(ref this.nextPermittedSequenceNumber); // Loop-around is OK, see remarks at variable definition.
					this.nextPermittedSequenceNumberEvent.Set();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		protected virtual bool TryEnterPacketGate(ForSomeTimeEventHelper forSomeTimeEventHelper)
		{
			while (!DoBreak)
			{
				if (Monitor.TryEnter(this.packetGateSyncObj))
				{
					this.packetGateEvent.Reset();
					return (true);
				}

				if (forSomeTimeEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold()) // Signal wait operation if needed.
					IncrementIsSendingForSomeTimeChanged();

				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
					this.packetGateEvent.WaitOne(StaticRandom.Next(50, 200));
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in TryEnterSequenceGateAsRequired()!");
					break;
				}
			}

			DebugSend("TryEnterPacketGate() has determined to break");

			return (false);
		}

		/// <summary></summary>
		protected virtual void LeavePacketGate()
		{
			if (IsUndisposed)
			{
				Monitor.Exit(this.packetGateSyncObj);

				this.packetGateEvent.Set();
			}
		}

		/// <summary></summary>
		protected virtual void DoSendPre()
		{
			if (TerminalSettings.Send.SignalXOnBeforeEachTransmission)
				RequestSignalInputXOn();
		}

		/// <summary></summary>
		protected virtual void DoSendPost()
		{
			// Nothing to do so far.
		}

		/// <remarks>
		/// Named "RawData" following "TextItem" terminology ("raw" vs. "text" and .
		/// <list type="bullet">
		/// <item><description>"Raw" instead of "Text".</description></item>
		/// <item><description>"Data" instead of "Item" as there is no 'RawItem'.</description></item>
		/// </list>
		/// </remarks>
		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'vs.' is a correct English abbreviation.")]
		protected virtual void DoSendRawData(ForSomeTimeEventHelper forSomeTimeEventHelper, byte[] data)
		{
			// Raise the 'IsSendingForSomeTimeChanged' event if a large chunk is about to be sent:
			if (forSomeTimeEventHelper.RaiseEventIfChunkSizeIsAboveThreshold(data.Length, this.terminalSettings.IO.RoughlyEstimatedMaxBytesPerMillisecond))
				IncrementIsSendingForSomeTimeChanged();

			DebugSend(string.Format("Sending {0} byte(s) of raw data by directly forwarding to raw terminal.", data.Length));

			ForwardPacketToRawTerminal(data); // Nothing for further processing, simply forward.
		}

		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendTextItem(ForSomeTimeEventHelper forSomeTimeEventHelper, TextSendItem item)
		{
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;
			if (TryParse(item.Text, item.DefaultRadix, item.ParseMode, out parseResult, out textSuccessfullyParsed))
				DoSendText(forSomeTimeEventHelper, parseResult, item.IsLine);
			else
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(item.Text, textSuccessfullyParsed)));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected virtual void DoSendText(ForSomeTimeEventHelper forSomeTimeEventHelper, Parser.Result[] parseResult, bool isLine = false)
		{
			var performLineRepeat    = false; // \remind For binary terminals, this is rather a 'PacketRepeat'.
			var lineRepeatIsInfinite = (TerminalSettings.Send.DefaultLineRepeat == Settings.SendSettings.LineRepeatInfinite);
			var lineRepeatRemaining  =  TerminalSettings.Send.DefaultLineRepeat;
			var isFirstRepetition    = true;

			do // Process at least once, potentially repeat:
			{
				DebugSend(string.Format("Sending {0} parser result(s) of {1}.", parseResult.Length, (isLine ? "a text line" : "text")));

				// --- Initialize the line/packet ---

				if (TryEnterPacketGate(forSomeTimeEventHelper))
				{
					var lineBeginTimeStamp    = DateTime.Now;      // \remind For binary terminals, this is rather a 'PacketBegin'.
					var lineEndTimeStamp      = DateTime.MinValue; // \remind For binary terminals, this is rather a 'PacketBegin'.
					var performLineDelay      = false;             // \remind For binary terminals, this is rather a 'PacketDelay'.
					var lineDelay             = TerminalSettings.Send.DefaultLineDelay;
					var performLineInterval   = false;             // \remind For binary terminals, this is rather a 'PacketInterval'.
					var lineInterval          = TerminalSettings.Send.DefaultLineInterval;
					var conflateDataQueue     = new Queue<byte>();
					var doBreak               = false;
					var packetGateAlreadyLeft = false;

					try
					{
						// --- Process the line/packet ---

						foreach (var result in parseResult)
						{
							var bytesResult = (result as Parser.BytesResult);
							if (bytesResult != null)
							{
								// Raise the 'IsSendingForSomeTimeChanged' event if a large chunk is about to be sent:
								if (forSomeTimeEventHelper.RaiseEventIfChunkSizeIsAboveThreshold(bytesResult.Bytes.Length, this.terminalSettings.IO.RoughlyEstimatedMaxBytesPerMillisecond))
									IncrementIsSendingForSomeTimeChanged();

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
												// Actively yield to other threads to make sure app stays responsive while looping:
												Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as sending shall happen as fast as possible.
											}

											break;
										}

										// Process in-line keywords:
										default:
										{
											ProcessInLineKeywords(forSomeTimeEventHelper, keywordResult, conflateDataQueue, ref doBreak, ref packetGateAlreadyLeft);

											break;
										}
									}
								}
							}

							if (DoBreak || doBreak) // (overall || local)
								break; // foreach (result)

							// Raise the 'IsSendingForSomeTimeChanged' event if sending already takes quite long:
							if (forSomeTimeEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
								IncrementIsSendingForSomeTimeChanged();
						} // foreach (result)

						// --- Finalize the line/packet ---

						ProcessLineEnd(forSomeTimeEventHelper, isLine, conflateDataQueue, ref doBreak);

						lineEndTimeStamp = DateTime.Now; // \remind For binary terminals, this is rather a 'packetEndTimeStamp'.
					}
					finally
					{
						if (!packetGateAlreadyLeft)
							LeavePacketGate();
					}

					// Break if requested or terminal has stopped or closed! Must be done prior to a potential Sleep() below!
					if (DoBreak || doBreak) // (overall || local)
						break; // do/while (repeat)

					// --- Perform line/packet related post-processing ---

					ProcessLineDelayOrInterval(forSomeTimeEventHelper, performLineDelay, lineDelay, performLineInterval, lineInterval, lineBeginTimeStamp, lineEndTimeStamp);

					// Process repeat:
					if (!lineRepeatIsInfinite)
					{
						if (lineRepeatRemaining > 0)
							lineRepeatRemaining--;
					}

					isFirstRepetition = false;
				} // if (TryEnterPacketGate())

				// Break if requested or terminal has stopped or closed! Must be done prior to a potential repeat below!
				if (DoBreak)
					break; // do/while (repeat)
			}
			while (performLineRepeat && (lineRepeatIsInfinite || (lineRepeatRemaining > 0)));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Directly referring to given object for performance reasons.")]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InLine", Justification = "It's 'in line' and not inline!")]
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Agree, could be refactored. Could be.")]
		protected virtual void ProcessInLineKeywords(ForSomeTimeEventHelper forSomeTimeEventHelper, Parser.KeywordResult result, Queue<byte> conflateDataQueue, ref bool doBreak, ref bool packetGateAlreadyLeft)
		{
			doBreak = false;
			packetGateAlreadyLeft = false;

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
				////doBreak = !TryEnterPacketGate() must not be called, see above.
				////packetGateAlreadyLeft = doBreak;

					break;
				}

				case Parser.Keyword.Delay:
				{
					ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
					LeavePacketGate();
					{
						int delay = TerminalSettings.Send.DefaultDelay;
						if (!ArrayEx.IsNullOrEmpty(result.Args))
							delay = result.Args[0];

						// Raise the 'IsSendingForSomeTimeChanged' event if sending is about to be delayed:
						if (forSomeTimeEventHelper.RaiseEventIfDelayIsAboveThreshold(delay))
							IncrementIsSendingForSomeTimeChanged();

						Thread.Sleep(delay);
					}
					doBreak = !TryEnterPacketGate(forSomeTimeEventHelper);
					packetGateAlreadyLeft = doBreak;

					break;
				}

				case Parser.Keyword.TimeStamp:
				{
					var format = TerminalSettings.Display.TimeStampFormat;
					var useUtc = TerminalSettings.Display.TimeStampUseUtc;

				////if (!ArrayEx.IsNullOrEmpty(result.Args)) // with argument is yet pending (FR #400) and requires parser support for strings (FR #404).
				////	format = result.Args[0];

					var now = DateTime.Now;                                          // No enclosure!
					var de = new DisplayElement.TimeStampInfo(now, format, useUtc, "", "");
					var text = de.Text;

					Parser.Result[] parseResult;
					string textSuccessfullyParsed;               // A date/time format may conflict with YAT syntax.
					if (TryParse(text, Radix.String, Parser.Mode.NoEscapes, out parseResult, out textSuccessfullyParsed))
						AppendTimeStampToPendingPacketWithoutForwardingToRawTerminalYet(parseResult, conflateDataQueue);
					else
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(text, textSuccessfullyParsed)));

					break;
				}

				case Parser.Keyword.Port:
				{
					if (IsSerialPort)
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
					if (IsSerialPort)
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
					if (IsSerialPort)
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
					if (IsSerialPort)
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
					if (IsSerialPort)
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
					if (IsSerialPort)
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
					if (IsSerialPort)
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

				case Parser.Keyword.RtsOn:
				{
					if (IsSerialPort)
					{
						if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesRtsCtsAutomatically)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
							{
								var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
								if (port != null)
									port.Flush(0); // Just best-effort flush, no additional wait time.
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
							{
								var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
								if (port != null)
									port.RtsEnable = true; // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else
						{
							InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the RTS signal is not possible when automatic hardware or RS-485 flow control is active.", true));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the RTS signal is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.RtsOff:
				{
					if (IsSerialPort)
					{
						if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesRtsCtsAutomatically)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
							{
								var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
								if (port != null)
									port.Flush(0); // Just best-effort flush, no additional wait time.
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
							{
								var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
								if (port != null)
									port.RtsEnable = false; // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else
						{
							InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the RTS signal is not possible when automatic hardware or RS-485 flow control is active.", true));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the RTS signal is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.RtsToggle:
				{
					if (IsSerialPort)
					{
						if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesRtsCtsAutomatically)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
							{
								var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
								if (port != null)
									port.Flush(0); // Just best-effort flush, no additional wait time.
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
							{
								var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
								if (port != null)
									port.ToggleRts(); // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else
						{
							InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the RTS signal is not possible when automatic hardware or RS-485 flow control is active.", true));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the RTS signal is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.DtrOn:
				{
					if (IsSerialPort)
					{
						if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
							{
								var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
								if (port != null)
									port.Flush(0); // Just best-effort flush, no additional wait time.
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
							{
								var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
								if (port != null)
									port.DtrEnable = true; // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else
						{
							InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the DTR signal is not possible with the current settings.", true));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the DTR signal is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.DtrOff:
				{
					if (IsSerialPort)
					{
						if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
							{
								var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
								if (port != null)
									port.Flush(0); // Just best-effort flush, no additional wait time.
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
							{
								var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
								if (port != null)
									port.DtrEnable = false; // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else
						{
							InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the DTR signal is not possible with the current settings.", true));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the DTR signal is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.DtrToggle:
				{
					if (IsSerialPort)
					{
						if (!TerminalSettings.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically)
						{
							ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
							{
								var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
								if (port != null)
									port.Flush(0); // Just best-effort flush, no additional wait time.
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
							{
								var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
								if (port != null)
									port.ToggleDtr(); // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else
						{
							InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the DTR signal is not possible with the current settings.", true));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Modifying the DTR signal is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.OutputBreakOn:
				{
					if (IsSerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
						{
							var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
							if (port != null)
								port.Flush(0); // Just best-effort flush, no additional wait time.
							else
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
						{
							var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
							if (port != null)
								port.OutputBreak = true; // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
							else
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Output break is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.OutputBreakOff:
				{
					if (IsSerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
						{
							var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
							if (port != null)
								port.Flush(0); // Just best-effort flush, no additional wait time.
							else
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
						{
							var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
							if (port != null)
								port.OutputBreak = false; // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
							else
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Output break is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.OutputBreakToggle:
				{
					if (IsSerialPort)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
						{
							var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
							if (port != null)
								port.Flush(0); // Just best-effort flush, no additional wait time.
							else
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
						{
							var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
							if (port != null)
								port.ToggleOutputBreak(); // Note the serial port timing related limitation at MKY.IO.Serial.SerialPort.StartThreads().
							else
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Output break is only supported on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.FramingErrorsOn:
				{
					if (IsSerialPort)
					{
						var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
						if (port != null)
							port.IgnoreFramingErrors = false;
						else
							throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.FramingErrorsOff:
				{
					if (IsSerialPort)
					{
						var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
						if (port != null)
							port.IgnoreFramingErrors = true;
						else
							throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.FramingErrorsRestore:
				{
					if (IsSerialPort)
					{
						var port = (UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
						if (port != null)
							port.IgnoreFramingErrors = TerminalSettings.IO.SerialPort.IgnoreFramingErrors;
						else
							throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist or does not implement 'ISerialPort'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
					else
					{
						InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(DateTime.Now, Direction.Tx, "Framing errors can only be configured on serial COM ports.", true));
					}

					break;
				}

				case Parser.Keyword.ReportId:
				{
					if (IsUsbSerialHid)
					{
						ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...

						byte reportId = TerminalSettings.IO.UsbSerialHidDevice.ReportFormat.Id;
						if (!ArrayEx.IsNullOrEmpty(result.Args))
							reportId = (byte)result.Args[0];

						var device = (UnderlyingIOInstance as MKY.IO.Usb.SerialHidDevice);
						if (device != null)
							device.ActiveReportId = reportId;
						else
							throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying USB Ser/HID device object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Directly referring to given object for performance reasons.")]
		protected virtual void ProcessLineEnd(ForSomeTimeEventHelper forSomeTimeEventHelper, bool appendEol, Queue<byte> conflateDataQueue, ref bool doBreak)
		{
			if (!doBreak)
				ForwardPendingPacketToRawTerminal(conflateDataQueue); // Not the best approach to require this call at so many locations...
			else
				BreakPendingPacket(conflateDataQueue);
		}

		/// <remarks>For binary terminals, this is rather a 'ProcessPacketDelayOrInterval'.</remarks>
		protected virtual int ProcessLineDelayOrInterval(ForSomeTimeEventHelper forSomeTimeEventHelper, bool performLineDelay, int lineDelay, bool performLineInterval, int lineInterval, DateTime lineBeginTimeStamp, DateTime lineEndTimeStamp)
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
				// Raise the 'IsSendingForSomeTimeChanged' event if sending is about to be delayed for too long:
				if (forSomeTimeEventHelper.RaiseEventIfDelayIsAboveThreshold(effectiveDelay))
					IncrementIsSendingForSomeTimeChanged();

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
		protected virtual void BreakPendingPacket(Queue<byte> conflateDataQueue)
		{
			// Retrieve pending data:
			byte[] data;
			lock (conflateDataQueue)
			{
				data = conflateDataQueue.ToArray();
				conflateDataQueue.Clear();
			}

			string message;
			if (data.Length <= 1)
				message = data.Length + " byte not sent anymore due to break."; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.
			else
				message = data.Length + " bytes not sent anymore due to break."; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.

			InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, message, true));

			// Note a suboptimality (bug #352) described in Terminal.InlineDisplayElement():
			// The above warning message may be appended at a suboptiomal location, e.g.
			// at the end of an already EOL'd line. Currently accepting this suboptimality.
		}

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
			var port = (UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
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
			else
			{
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying serial port object does not exist!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			// Reflect the change in the settings:
			TerminalSettings.IO.SerialPort = settings;
			exception = null;
			return (true);
		}

		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected abstract void DoSendFileItem(ForSomeTimeEventHelper forSomeTimeEventHelper, FileSendItem item);

		/// <remarks>
		/// Text terminals are <see cref="Encoding"/> aware, binary terminals are not.
		/// </remarks>
		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendTextFileItem(ForSomeTimeEventHelper forSomeTimeEventHelper, FileSendItem item)
		{
			DoSendTextFileItem(forSomeTimeEventHelper, item, Encoding.Default); // The system's ANSI code page is good enough as fallback.
		}

		/// <remarks>
		/// Text terminals are <see cref="Encoding"/> aware, binary terminals are not.
		/// </remarks>
		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendTextFileItem(ForSomeTimeEventHelper forSomeTimeEventHelper, FileSendItem item, Encoding encodingFallback)
		{
			using (var sr = new StreamReader(item.FilePath, encodingFallback, true))
			{                             // Automatically detect encoding from BOM, otherwise use fallback.
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
						continue;

					DoSendFileLine(forSomeTimeEventHelper, line, item.DefaultRadix);

					if (DoBreak)
						break;

					// Actively yield to other threads to make sure app stays responsive while looping:
					Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as DoSendFileLine()
				}                                // will sleep depending on state of the event helper.
			}
		}

		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendXmlFileItem(ForSomeTimeEventHelper forSomeTimeEventHelper, FileSendItem item)
		{
			string[] lines;
			XmlReaderHelper.LinesFromFile(item.FilePath, out lines); // Read file at once for simplicity. Minor limitation:
			foreach (string line in lines)                           // 'forSomeTimeEventHelper.RaiseEventIf...' will
			{                                                        // only be evaluated at DoSendFileLine() below.
				if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
					continue;

				DoSendFileLine(forSomeTimeEventHelper, line, item.DefaultRadix);

				if (DoBreak)
					break;

				// Actively yield to other threads to make sure app stays responsive while looping:
				Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as DoSendFileLine()
			}                                // will sleep depending on state of the event helper.
		}

		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendFileLine(ForSomeTimeEventHelper forSomeTimeEventHelper, string line, Radix defaultRadix)
		{
			// Raise the 'IsSendingForSomeTimeChanged' event if sending already takes quite long, i.e. file cannot be sent within threshold:
			if (forSomeTimeEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
				IncrementIsSendingForSomeTimeChanged();

			// Send the file line:
			var parseMode = TerminalSettings.Send.File.ToParseMode();
			var item = new TextSendItem(line, defaultRadix, parseMode, SendMode.File, true);
			DoSendTextItem(forSomeTimeEventHelper, item);
		}

		/// <remarks>
		/// <see cref="FileSendItem.DefaultRadix"/> is not used for sending raw files.
		/// This fact is considered in 'View.Controls.SendFile.SetRecentAndCommandControls()'.
		/// Changes in behavior here will have to be adapted in that control method as well.
		/// </remarks>
		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendFileChunk(ForSomeTimeEventHelper forSomeTimeEventHelper, byte[] chunk)
		{
			// Raise the 'IsSendingForSomeTimeChanged' event if sending already takes quite long, i.e. file cannot be sent within threshold:
			if (forSomeTimeEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold())
				IncrementIsSendingForSomeTimeChanged();

			// Send the file chunk:
			DoSendRawData(forSomeTimeEventHelper, chunk);
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
			DebugBreakLead("Break has been requested");

			lock (this.breakStateSyncObj)
				this.breakState = true;

			DebugBreakTail(" and is active now");
		}

		/// <summary>
		/// Resumes all currently suspended operations in the terminal.
		/// </summary>
		public virtual void ResumeBreak()
		{
			DebugBreakLead("Resume from break has been requested");

			lock (this.breakStateSyncObj)
				this.breakState = false;

			DebugBreakTail(" and break is inactive now");
		}

		#endregion

		#region Event Raising
		//------------------------------------------------------------------------------------------
		// Event Raising
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void IncrementIsSendingChanged()
		{
			OnIsSendingChanged(true);
		}

		/// <summary></summary>
		protected virtual void DecrementIsSendingChanged()
		{
			OnIsSendingChanged(false);
		}

		/// <summary></summary>
		private void OnIsSendingChanged(bool value)
		{
			bool raiseEvent = false;

			lock (this.isSendingCountSyncObj)
			{
				if (value)
				{
					this.isSendingCount++; // No need to handle overflow, almost impossible to happen with YAT.
					                     //// And if it happens nevertheless, an OverflowException is OK.
					raiseEvent = true; // Always raise, because another send request might already have resumed a
				}                      // signalled break condition, thus the break condition must be signalled again!
				else
				{
					if (this.isSendingCount > 0) // Allow unsymmetrical decrement but
						this.isSendingCount--;   // counter must not become negative.

					if (this.isSendingCount == 0)
						raiseEvent = true;
				}
			}

			DebugIsSending(string.Format(CultureInfo.InvariantCulture, "OnIsSendingChanged({0}) resulting in count = {1} and raise = {2}", value, this.isSendingCount, raiseEvent));

			if (raiseEvent)
				OnIsSendingChanged(new EventArgs<bool>(value));
		}

		/// <summary></summary>
		protected virtual void OnIsSendingChanged(EventArgs<bool> e)
		{
			this.eventHelper.RaiseSync<EventArgs<bool>>(IsSendingChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void IncrementIsSendingForSomeTimeChanged()
		{
			OnIsSendingForSomeTimeChanged(true);
		}

		/// <summary></summary>
		protected virtual void DecrementIsSendingForSomeTimeChanged()
		{
			OnIsSendingForSomeTimeChanged(false);
		}

		/// <summary></summary>
		private void OnIsSendingForSomeTimeChanged(bool value)
		{
			bool raiseEvent = false;

			lock (this.isSendingForSomeTimeCountSyncObj)
			{
				if (value)
				{
					this.isSendingForSomeTimeCount++; // No need to handle overflow, almost impossible to happen with YAT.
					                                //// And if it happens nevertheless, an OverflowException is OK.
					raiseEvent = true; // Always raise, because another send request might already have resumed a
				}                      // signalled break condition, thus the break condition must be signalled again!
				else
				{
					if (this.isSendingForSomeTimeCount > 0) // Allow unsymmetrical decrement but
						this.isSendingForSomeTimeCount--;   // counter must not become negative.

					if (this.isSendingForSomeTimeCount == 0)
						raiseEvent = true;
				}
			}

			DebugIsSending(string.Format(CultureInfo.InvariantCulture, "OnIsSendingForSomeTimeChanged({0}) resulting in count = {1} and raise = {2}", value, this.isSendingForSomeTimeCount, raiseEvent));

			if (raiseEvent)
				OnIsSendingForSomeTimeChanged(new EventArgs<bool>(value));
		}

		/// <summary></summary>
		protected virtual void OnIsSendingForSomeTimeChanged(EventArgs<bool> e)
		{
			this.eventHelper.RaiseSync<EventArgs<bool>>(IsSendingForSomeTimeChanged, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_SEND")]
		private void DebugSend(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_IS_SENDING")]
		private void DebugIsSending(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_BREAK")]
		private void DebugBreakLead(string message)
		{
			DebugMessageLead(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_BREAK")]
		private static void DebugBreakTail(string message)
		{
			Debug.WriteLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
