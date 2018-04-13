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
// YAT Version 2.0.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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
using System.IO;
using System.Threading;

using MKY;
using MKY.Diagnostics;

using YAT.Application.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Binary protocol terminal.
	/// </summary>
	public class BinaryTerminal : Terminal
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		#region Types > Line Break Timer
		//------------------------------------------------------------------------------------------
		// Types > Line Break Timer
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		private class LineBreakTimer : IDisposable
		{
			/// <summary>
			/// A dedicated event helper to allow autonomously ignoring exceptions when disposed.
			/// </summary>
			private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(LineBreakTimer).FullName);

			private int timeout;
			private Timer timer;
			private object timerSyncObj = new object();

			/// <summary></summary>
			public event EventHandler Elapsed;

			/// <summary></summary>
			public LineBreakTimer(int timeout)
			{
				this.timeout = timeout;
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			public bool IsDisposed { get; protected set; }

			/// <summary></summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary></summary>
			protected virtual void Dispose(bool disposing)
			{
				if (!IsDisposed)
				{
					Debug.WriteLine("Remind (2016-09-08 / MKY) 'Elapsed' event handler not yet free'd, whole timer handling should be encapsulated into the 'LineState' class.");
					DebugEventManagement.DebugWriteAllEventRemains(this);
					this.eventHelper.DiscardAllEventsAndExceptions();

					// Dispose of managed resources if requested:
					if (disposing)
					{
						// In the 'normal' case, the timer is stopped in Stop().
						StopAndDisposeTimer();
					}

					// Set state to disposed:
					IsDisposed = true;
				}
			}

		#if (DEBUG)

			/// <remarks>
			/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
			/// "Types that declare disposable members should also implement IDisposable. If the type
			///  does not own any unmanaged resources, do not implement a finalizer on it."
			/// 
			/// Well, true for best performance on finalizing. However, it's not easy to find missing
			/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
			/// is kept for DEBUG, indicating missing calls.
			/// 
			/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
			/// </remarks>
			~LineBreakTimer()
			{
				Dispose(false);

				DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
			}

		#endif // DEBUG

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (IsDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
			}

			#endregion

			/// <summary></summary>
			public virtual void Start()
			{
				AssertNotDisposed();

				CreateAndStartTimer();
			}

			/// <summary></summary>
			public virtual void Restart()
			{
				// AssertNotDisposed() is called by methods below.

				Stop();
				Start();
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
			public virtual void Stop()
			{
				AssertNotDisposed();

				StopAndDisposeTimer();
			}

			private void CreateAndStartTimer()
			{
				lock (this.timerSyncObj)
				{
					if (this.timer == null)
					{
						this.timer = new System.Threading.Timer(new System.Threading.TimerCallback(timer_Timeout), null, this.timeout, System.Threading.Timeout.Infinite);
					}
				}
			}

			private void StopAndDisposeTimer()
			{
				lock (this.timerSyncObj)
				{
					if (this.timer != null)
					{
						this.timer.Dispose();
						this.timer = null;
					}
				}
			}

			private void timer_Timeout(object obj)
			{
				// Non-periodic timer, only a single timeout event thread can be active at a time.
				// There is no need to synchronize callbacks to this event handler.

				lock (this.timerSyncObj)
				{
					if ((this.timer == null) || (IsDisposed))
						return; // Handle overdue event callbacks.
				}

				Stop();

				OnElapsed(EventArgs.Empty);
			}

			/// <summary></summary>
			protected virtual void OnElapsed(EventArgs e)
			{
				this.eventHelper.RaiseSync(Elapsed, this, e);
			}
		}

		#endregion

		#region Types > Line State
		//------------------------------------------------------------------------------------------
		// Types > Line State
		//------------------------------------------------------------------------------------------

		private enum LinePosition
		{
			Begin,
			Content,
			ContentExceeded,
			End
		}

		private class LineState : IDisposable
		{
			public LinePosition         Position                      { get; set; }
			public DisplayLinePart      Elements                      { get; set; }
			public SequenceQueue        SequenceAfter                 { get; set; }
			public SequenceQueue        SequenceBefore                { get; set; }
			public List<DisplayElement> PendingSequenceBeforeElements { get; set; }
			public DateTime             TimeStamp                     { get; set; }
			public bool                 Highlight                     { get; set; }

			public LineBreakTimer       BreakTimer                    { get; set; }

			public LineState(SequenceQueue sequenceAfter, SequenceQueue sequenceBefore, LineBreakTimer breakTimer)
			{
				Position                      = LinePosition.Begin; // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
				Elements                      = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				SequenceAfter                 = sequenceAfter;
				SequenceBefore                = sequenceBefore;
				PendingSequenceBeforeElements = new List<DisplayElement>();
				TimeStamp                     = DateTime.Now;
				Highlight                     = false;

				BreakTimer                    = breakTimer;
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			public bool IsDisposed { get; protected set; }

			/// <summary></summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary></summary>
			protected virtual void Dispose(bool disposing)
			{
				if (!IsDisposed)
				{
					// Dispose of managed resources if requested:
					if (disposing)
					{
						// In the 'normal' case, the timer is stopped in ExecuteLineEnd().
						if (BreakTimer != null)
						{
							BreakTimer.Dispose();
							EventHandlerHelper.RemoveAllEventHandlers(BreakTimer);

							// \remind (2016-09-08 / MKY)
							// Whole timer handling should be encapsulated into the 'LineState' class.
						}
					}

					// Set state to disposed:
					BreakTimer = null;
					IsDisposed = true;
				}
			}

		#if (DEBUG)

			/// <remarks>
			/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
			/// "Types that declare disposable members should also implement IDisposable. If the type
			///  does not own any unmanaged resources, do not implement a finalizer on it."
			/// 
			/// Well, true for best performance on finalizing. However, it's not easy to find missing
			/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
			/// is kept for DEBUG, indicating missing calls.
			/// 
			/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
			/// </remarks>
			~LineState()
			{
				Dispose(false);

				DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
			}

		#endif // DEBUG

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (IsDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
			}

			#endregion

			public virtual void Reset()
			{
				AssertNotDisposed();

				Position                      = LinePosition.Begin; // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
				Elements                      = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				SequenceAfter                  .Reset();
				SequenceBefore                 .Reset();
				PendingSequenceBeforeElements = new List<DisplayElement>();
				TimeStamp                     = DateTime.Now;
				Highlight                     = false;
			}
		}

		private class BidirLineState
		{
			public bool IsFirstChunk          { get; set; }
			public bool IsFirstLine           { get; set; }
			public string PortStamp           { get; set; }
			public IODirection Direction      { get; set; }
			public DateTime LastLineTimeStamp { get; set; }

			public BidirLineState()
			{
				IsFirstChunk      = true;
				IsFirstLine       = true;
				PortStamp         = null;
				Direction         = IODirection.None;
				LastLineTimeStamp = DateTime.Now;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstChunk      = rhs.IsFirstChunk;
				IsFirstLine       = rhs.IsFirstLine;
				PortStamp         = rhs.PortStamp;
				Direction         = rhs.Direction;
				LastLineTimeStamp = rhs.LastLineTimeStamp;
			}
		}

		#endregion

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private LineState txLineState;
		private LineState rxLineState;

		private BidirLineState bidirLineState;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings)
			: base(settings)
		{
			AttachBinaryTerminalSettings();
			InitializeStates();
		}

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachBinaryTerminalSettings();

			var casted = (terminal as BinaryTerminal);
			if (casted != null)
			{
				// Tx:

				this.txLineState = casted.txLineState;
				                                         //// \remind (2016-09-08 / MKY)
				if (this.txLineState.BreakTimer != null)   // Ensure to free referenced resources such as the 'Elapsed' event handler.
					this.txLineState.BreakTimer.Dispose(); // Whole timer handling should be encapsulated into the 'LineState' class.

				this.txLineState.BreakTimer = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				this.txLineState.BreakTimer.Elapsed += txTimedLineBreak_Elapsed;

				// Rx:

				this.rxLineState = casted.rxLineState;
				                                         //// \remind (2016-09-08 / MKY)
				if (this.rxLineState.BreakTimer != null)   // Ensure to free referenced resources such as the 'Elapsed' event handler.
					this.rxLineState.BreakTimer.Dispose(); // Whole timer handling should be encapsulated into the 'LineState' class.

				this.rxLineState.BreakTimer = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				this.rxLineState.BreakTimer.Elapsed += rxTimedLineBreak_Elapsed;

				// Bidir:

				this.bidirLineState = new BidirLineState(casted.bidirLineState);
			}
			else
			{
				InitializeStates();
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					DetachBinaryTerminalSettings();

					if (this.txLineState != null)
						this.txLineState.Dispose();

					if (this.rxLineState != null)
						this.rxLineState.Dispose();
				}

				// Set state to disposed:
				this.txLineState = null;
				this.rxLineState = null;
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private Settings.BinaryTerminalSettings BinaryTerminalSettings
		{
			get
			{
				if (TerminalSettings != null)
					return (TerminalSettings.BinaryTerminal);
				else
					return (null);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > Send Data
		//------------------------------------------------------------------------------------------
		// Methods > Send Data
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override void SendFileLine(string dataLine, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.File.ToParseMode();

			DoSendData(new TextDataSendItem(dataLine, defaultRadix, parseMode, true));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1022:PositiveSignsMustBeSpacedCorrectly", Justification = "What's wrong with closing parenthesis_+_quote? Bug in StyleCop?")]
		protected override void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Eol:
				case Parser.Keyword.NoEol:
				{
					// Add space if necessary:
					if (ElementsAreSeparate(IODirection.Tx))
					{
						if (this.txLineState.Elements.ByteCount > 0)
							OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.DataSpace());
					}

					string info = (Parser.KeywordEx)(result.Keyword) + " keyword is not supported for binary terminals";
					OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, info));
					break;
				}

				default:
				{
					base.ProcessInLineKeywords(result);
					break;
				}
			}
		}

		#endregion

		#region Methods > Send File
		//------------------------------------------------------------------------------------------
		// Methods > Send File
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected override void ProcessSendFileItem(FileSendItem item)
		{
			try
			{
				if (ExtensionHelper.IsXmlFile(item.FilePath))
				{
					ProcessSendXmlFileItem(item);
				}
				else if (ExtensionHelper.IsTextFile(item.FilePath))
				{
					ProcessSendTextFileItem(item);
				}
				else // By default treat as binary file:
				{
					using (FileStream fs = File.OpenRead(item.FilePath))
					{
						long remaining = fs.Length;
						while (remaining > 0)
						{
							byte[] a = new byte[1024]; // 1 KB chunks.
							int n = fs.Read(a, 0, a.Length);
							Array.Resize<byte>(ref a, n);
							Send(a);
							remaining -= n;

							if (BreakSendFile)
							{
								OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
								break;
							}

							Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
						}
					}

					// Note that 'item.DefaultRadix' is not used for sending binary files.
					// This fact is considered in 'View.Controls.SendFile.SetRecentAndCommandControls()'.
					// Changes in behavior above will have to be adapted in that control method as well.
				}
			}
			catch (Exception ex)
			{
				OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, @"Error reading file """ + item.FilePath + @""": " + ex.Message));
			}
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			using (var p = new Parser.Parser(TerminalSettings.IO.Endianness))
			{
				LineBreakTimer t;

				// Tx:

				byte[] txSequenceBreakAfter;
				if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakAfter.Sequence, out txSequenceBreakAfter))
					txSequenceBreakAfter = null;

				byte[] txSequenceBreakBefore;
				if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakBefore.Sequence, out txSequenceBreakBefore))
					txSequenceBreakBefore = null;

				t = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				t.Elapsed += txTimedLineBreak_Elapsed;

				if (this.txLineState != null) // Ensure to free referenced resources such as the 'Elapsed' event handler of the timer.
					this.txLineState.Dispose();

				this.txLineState = new LineState(new SequenceQueue(txSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), t);

				// Rx:

				byte[] rxSequenceBreakAfter;
				if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakAfter.Sequence, out rxSequenceBreakAfter))
					rxSequenceBreakAfter = null;

				byte[] rxSequenceBreakBefore;
				if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakBefore.Sequence, out rxSequenceBreakBefore))
					rxSequenceBreakBefore = null;

				t = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				t.Elapsed += rxTimedLineBreak_Elapsed;

				if (this.rxLineState != null) // Ensure to free referenced resources such as the 'Elapsed' event handler of the timer.
					this.rxLineState.Dispose();

				this.rxLineState = new LineState(new SequenceQueue(rxSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), t);
			}

			// Bidir:

			this.bidirLineState = new BidirLineState();
		}

		private void ExecuteLineBegin(LineState lineState, DateTime ts, string ps, IODirection d, DisplayElementCollection elements)
		{
			if (this.bidirLineState.IsFirstLine) // Properly initialize the time delta:
				this.bidirLineState.LastLineTimeStamp = ts;
			                                        //// Using the exact type to prevent potential mismatch in case the type one day defines its own value!
			var lp = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			lp.Add(new DisplayElement.LineStart()); // Direction may be both!

			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowPort || TerminalSettings.Display.ShowDirection)
			{
				DisplayLinePart info;
				PrepareLineBeginInfo(ts, (ts - InitialTimeStamp), (ts - this.bidirLineState.LastLineTimeStamp), ps, d, out info);
				lp.AddRange(info);
			}

			lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
			elements.AddRange(lp);

			lineState.Position = LinePosition.Content;
			lineState.TimeStamp = ts;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void ExecuteContent(Settings.BinaryDisplaySettings displaySettings, LineState lineState, IODirection d, byte b, DisplayElementCollection elements, out List<DisplayElement> elementsForNextLine)
		{
			elementsForNextLine = null;

			// Convert content:
			var de = ByteToElement(b, d);
			de.Highlight = lineState.Highlight;

			var lp = new DisplayLinePart(); // Default initial capacity is OK.

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((displaySettings.SequenceLineBreakBefore.Enabled && (lineState.Elements.ByteCount > 0) &&
				(lineState.Position != LinePosition.End)))   // Also skip if line has just been brokwn.
			{
				lineState.SequenceBefore.Enqueue(b);
				if (lineState.SequenceBefore.IsCompleteMatch)
				{
					// Sequence is complete, move them to the next line:
					lineState.PendingSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.

					elementsForNextLine = new List<DisplayElement>(lineState.PendingSequenceBeforeElements.Capacity); // Preset the required capacity to improve memory management.
					foreach (DisplayElement dePending in lineState.PendingSequenceBeforeElements)
						elementsForNextLine.Add(dePending.Clone());

					lineState.Position = LinePosition.End;
				}
				else if (lineState.SequenceBefore.IsPartlyMatchContinued)
				{
					// Keep sequence elements and delay them until sequence is either complete or refused:
					lineState.PendingSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else if (lineState.SequenceBefore.IsPartlyMatchBeginning)
				{
					// Previous was no match, previous sequence can be treated as normal:
					TreatSequenceBeforeAsNormal(lineState, d, lp);

					// Keep sequence elements and delay them until sequence is either complete or refused:
					lineState.PendingSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else
				{
					// No match at all, previous sequence can be treated as normal:
					TreatSequenceBeforeAsNormal(lineState, d, lp);
				}
			}

			// Add current element if it wasn't consumed above:
			if (de != null)
			{
				AddSpaceIfNecessary(lineState, d, lp, de);
				lp.Add(de);
			}

			if (lineState.Position != LinePosition.ContentExceeded)
			{
				lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elements.AddRange(lp);
			}

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((displaySettings.SequenceLineBreakAfter.Enabled) &&
				(lineState.Position != LinePosition.End))
			{
				lineState.SequenceAfter.Enqueue(b);
				if (lineState.SequenceAfter.IsCompleteMatch) // No need to check for partly matches.
					lineState.Position = LinePosition.End;
			}

			if ((displaySettings.LengthLineBreak.Enabled) &&
				(lineState.Position != LinePosition.End))
			{
				if (lineState.Elements.ByteCount >= displaySettings.LengthLineBreak.Length)
					lineState.Position = LinePosition.End;
			}

			if (lineState.Position != LinePosition.End)
			{
				if ((lineState.Elements.ByteCount >= TerminalSettings.Display.MaxBytePerLineCount) &&
					(lineState.Position != LinePosition.ContentExceeded))
				{
					lineState.Position = LinePosition.ContentExceeded;
					                                     //// Using term "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.
					string message = "Maximal number of bytes per line exceeded! Check the end-of-line settings or increase the limit in the advanced terminal settings.";
					lineState.Elements.Add(new DisplayElement.ErrorInfo((Direction)d, message, true));
					elements.Add          (new DisplayElement.ErrorInfo((Direction)d, message, true));
				}
			}
		}

		private void TreatSequenceBeforeAsNormal(LineState lineState, IODirection d, DisplayLinePart lp)
		{
			if (lineState.PendingSequenceBeforeElements.Count > 0)
			{
				foreach (var de in lineState.PendingSequenceBeforeElements)
				{
					AddSpaceIfNecessary(lineState, d, lp, de);
					lp.Add(de);
				}

				lineState.PendingSequenceBeforeElements.Clear();
			}
		}

		private void AddSpaceIfNecessary(LineState lineState, IODirection d, DisplayLinePart lp, DisplayElement de)
		{
			if (ElementsAreSeparate(d) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.ByteCount > 0) || (lp.ByteCount > 0))
					lp.Add(new DisplayElement.DataSpace());
			}
		}

		private void ExecuteLineEnd(LineState lineState, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			// Note: Code sequence the same as ExecuteLineEnd() of TextTerminal for better comparability.

			                                    // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
			var line = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			// Process line content:
			line.AddRange(lineState.Elements.Clone()); // Clone elements to ensure decoupling.

			// Process line length:
			var lp = new DisplayLinePart(); // Default initial capacity is OK.
			if (TerminalSettings.Display.ShowLength) // = byte count.
			{
				DisplayLinePart info;
				PrepareLineEndInfo(lineState.Elements.ByteCount, out info);
				lp.AddRange(info);
			}
			lp.Add(new DisplayElement.LineBreak()); // Direction may be both!

			// Finalize elements and line:
			elements.AddRange(lp.Clone()); // Clone elements because they are needed again right below.
			line.AddRange(lp);
			lines.Add(line);

			this.bidirLineState.IsFirstLine = false;
			this.bidirLineState.LastLineTimeStamp = lineState.TimeStamp;

			// Reset line state:
			lineState.Reset();
		}

		/// <summary></summary>
		protected override void ProcessRawChunk(RawChunk raw, bool highlight, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			if (lines.Count <= 0) // Properly initialize the time delta:
				this.bidirLineState.LastLineTimeStamp = raw.TimeStamp;

			Settings.BinaryDisplaySettings displaySettings;
			switch (raw.Direction)
			{
				case IODirection.Tx: displaySettings = BinaryTerminalSettings.TxDisplay; break;
				case IODirection.Rx: displaySettings = BinaryTerminalSettings.RxDisplay; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + raw.Direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			LineState lineState;
			switch (raw.Direction)
			{
				case IODirection.Tx: lineState = this.txLineState; break;
				case IODirection.Rx: lineState = this.rxLineState; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + raw.Direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (highlight) // Activate if needed, leave unchanged otherwise as it could have become highlighted by a previous raw chunk.
			{
				lineState.Highlight = true;
			}

			foreach (byte b in raw.Content)
			{
				// In case of reload, timed line breaks are executed here:
				if (IsReloading && displaySettings.TimedLineBreak.Enabled)
					ExecuteTimedLineBreakOnReload(displaySettings, lineState, raw.TimeStamp, elements, lines);

				// Line begin and time stamp:
				if (lineState.Position == LinePosition.Begin)
				{
					ExecuteLineBegin(lineState, raw.TimeStamp, raw.PortStamp, raw.Direction, elements);

					if (displaySettings.TimedLineBreak.Enabled)
						lineState.BreakTimer.Start();
				}
				else
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.BreakTimer.Restart(); // Restart as timeout refers to time after last received byte.
				}

				// Content:
				List<DisplayElement> elementsForNextLine = null;
				if (lineState.Position == LinePosition.Content)
					ExecuteContent(displaySettings, lineState, raw.Direction, b, elements, out elementsForNextLine);

				// Line end and length:
				if (lineState.Position == LinePosition.End)
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.BreakTimer.Stop();

					ExecuteLineEnd(lineState, elements, lines);

					// In case of a pending immediately insert the sequence into a new line:
					if ((elementsForNextLine != null) && (elementsForNextLine.Count > 0))
					{
						ExecuteLineBegin(lineState, raw.TimeStamp, raw.PortStamp, raw.Direction, elements);

						foreach (var de in elementsForNextLine)
						{
							if (de.Origin != null) // Foreach element where origin exists.
							{
								foreach (var origin in de.Origin)
								{
									foreach (var originByte in origin.Value1)
									{
										List<DisplayElement> elementsForNextLineDummy;
										ExecuteContent(displaySettings, lineState, raw.Direction, originByte, elements, out elementsForNextLineDummy);

										// Note that 're.Direction' above is OK, this function is processing all in the same direction.
									}
								}
							}
						}
					}
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		private void ExecuteTimedLineBreakOnReload(Settings.BinaryDisplaySettings displaySettings, LineState lineState,
		                                           DateTime ts, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			if (lineState.Elements.Count > 0)
			{
				var span = ts - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout) {
					ExecuteLineEnd(lineState, elements, lines);
				}
			}

			lineState.TimeStamp = ts;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		private void ProcessAndSignalPortAndDirectionLineBreak(string ps, IODirection d)
		{
			if (this.bidirLineState.IsFirstChunk)
			{
				this.bidirLineState.IsFirstChunk = false;
			}
			else // = 'IsSubsequentChunk'.
			{
				if (TerminalSettings.Display.PortLineBreakEnabled ||
					TerminalSettings.Display.DirectionLineBreakEnabled)
				{
					if (!StringEx.EqualsOrdinalIgnoreCase(ps, this.bidirLineState.PortStamp) || (d != this.bidirLineState.Direction))
					{
						LineState lineState;

						if (d == this.bidirLineState.Direction)
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.txLineState; break;
								case IODirection.Rx: lineState = this.rxLineState; break;

								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else // Attention: Direction changed => Use other state.
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.rxLineState; break; // Reversed!
								case IODirection.Rx: lineState = this.txLineState; break; // Reversed!

								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}

						if ((lineState.Elements != null) && (lineState.Elements.Count > 0))
						{
							var elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
							var lines = new List<DisplayLine>();

							ExecuteLineEnd(lineState, elements, lines);

							OnDisplayElementsProcessed(this.bidirLineState.Direction, elements);
							OnDisplayLinesProcessed   (this.bidirLineState.Direction, lines);
						}
					} // a line break has been detected
				} // a line break is active
			} // is subsequent chunk

			this.bidirLineState.PortStamp = ps;
			this.bidirLineState.Direction = d;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		private void ProcessAndSignalLineBreak(IODirection d)
		{
			LineState lineState;
			switch (d)
			{
				case IODirection.Tx: lineState = this.txLineState; break;
				case IODirection.Rx: lineState = this.rxLineState; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (lineState.Elements.Count > 0)
			{
				var elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				var lines = new List<DisplayLine>();

				ExecuteLineEnd(lineState, elements, lines);

				OnDisplayElementsProcessed(d, elements);
				OnDisplayLinesProcessed(d, lines);
			}
		}

		/// <summary></summary>
		protected override void ProcessAndSignalRawChunk(RawChunk raw, bool highlight)
		{
			// Check whether port or direction has changed:
			ProcessAndSignalPortAndDirectionLineBreak(raw.PortStamp, raw.Direction);

			// Process the raw chunk:
			base.ProcessAndSignalRawChunk(raw, highlight);

			// Enforce line break if requested:
			if (TerminalSettings.Display.ChunkLineBreakEnabled)
				ProcessAndSignalLineBreak(raw.Direction);
		}

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		/// <remarks>Ensure that states are completely reset.</remarks>
		public override bool RefreshRepositories()
		{
			AssertNotDisposed();

			InitializeStates();
			return (base.RefreshRepositories());
		}

		/// <remarks>Ensure that states are completely reset.</remarks>
		protected override void ClearMyRepository(RepositoryType repository)
		{
			AssertNotDisposed();

			InitializeStates();
			base.ClearMyRepository(repository);
		}

		#endregion

		#region Methods > ToString
		//------------------------------------------------------------------------------------------
		// Methods > ToString
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			// See below why AssertNotDisposed() is not called on such basic method!

			return (ToDiagnosticsString("")); // No 'real' ToString() method required yet.
		}

		/// <summary></summary>
		public override string ToDiagnosticsString(string indent)
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method!

			return (indent + "> Type: BinaryTerminal" + Environment.NewLine + base.ToDiagnosticsString(indent));
		}

		#endregion

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed += BinaryTerminalSettings_Changed;
		}

		private void DetachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed -= BinaryTerminalSettings_Changed;
		}

		private void ApplyBinaryTerminalSettings()
		{
			InitializeStates();
			RefreshRepositories();
		}

		#endregion

		#region Settings Events
		//==========================================================================================
		// Settings Events
		//==========================================================================================

		private void BinaryTerminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyBinaryTerminalSettings();
		}

		#endregion

		#region Timer Events
		//==========================================================================================
		// Timer Events
		//==========================================================================================

		private void txTimedLineBreak_Elapsed(object sender, EventArgs e)
		{
			ProcessAndSignalLineBreak(IODirection.Tx);
		}

		private void rxTimedLineBreak_Elapsed(object sender, EventArgs e)
		{
			ProcessAndSignalLineBreak(IODirection.Rx);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
