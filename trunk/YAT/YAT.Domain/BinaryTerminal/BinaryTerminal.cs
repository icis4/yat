//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;

using MKY;
using MKY.Collections.Generic;
using MKY.Diagnostics;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\BinaryTerminal for better separation of the implementation files.
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
			private bool isDisposed;

			private int timeout;
			private System.Threading.Timer timer;

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
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary></summary>
			protected virtual void Dispose(bool disposing)
			{
				DebugEventManagement.DebugNotifyAllEventRemains(this);

				if (!this.isDisposed)
				{
					// Dispose of managed resources if requested:
					if (disposing)
					{
						// In the 'normal' case, the timer is stopped in Stop().
						StopAndDisposeTimer();
					}

					// Set state to disposed:
					this.isDisposed = true;
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
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Implemented the same as every other IDisposable implementation.")]
			public bool IsDisposed
			{
				get { return (this.isDisposed); }
			}

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (this.isDisposed)
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
			[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
			public virtual void Stop()
			{
				AssertNotDisposed();

				StopAndDisposeTimer();
			}

			private void CreateAndStartTimer()
			{
				this.timer = new System.Threading.Timer(new System.Threading.TimerCallback(timer_Timeout), null, this.timeout, System.Threading.Timeout.Infinite);
			}

			private void StopAndDisposeTimer()
			{
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
			}

			private void timer_Timeout(object obj)
			{
				Stop();
				OnTimeout(EventArgs.Empty);
			}

			/// <summary></summary>
			protected virtual void OnTimeout(EventArgs e)
			{
				AssertNotDisposed();

				EventHelper.FireSync(Elapsed, this, e);
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
			Data,
			DataExceeded,
			End
		}

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Private class.")]
		private class LineState : IDisposable
		{
			private bool isDisposed;

			public LinePosition         LinePosition;
			public DisplayLinePart      LineElements;
			public SequenceQueue        SequenceAfter;
			public SequenceQueue        SequenceBefore;
			public List<DisplayElement> PendingSequenceBeforeElements;
			public DateTime             TimeStamp;
			public LineBreakTimer       LineBreakTimer;

			public LineState(SequenceQueue sequenceAfter, SequenceQueue sequenceBefore, DateTime timeStamp, LineBreakTimer lineBreakTimer)
			{
				LinePosition                  = LinePosition.Begin;
				LineElements                  = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				SequenceAfter                 = sequenceAfter;
				SequenceBefore                = sequenceBefore;
				PendingSequenceBeforeElements = new List<DisplayElement>();
				TimeStamp                     = timeStamp;
				LineBreakTimer                = lineBreakTimer;
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary></summary>
			protected virtual void Dispose(bool disposing)
			{
				if (!this.isDisposed)
				{
					// Dispose of managed resources if requested:
					if (disposing)
					{
						// In the 'normal' case, the timer is stopped in ExecuteLineEnd().
						if (this.LineBreakTimer != null)
						{
							// \remind (2016-09-08 / mky)
							// Ensure to free referenced resources such as the 'Elapsed' event handler.
							// Whole timer handling should be encapsulated into the 'LineState' class.
							EventHandlerHelper.RemoveEventHandler(this.LineBreakTimer, "Elapsed");

							this.LineBreakTimer.Dispose();
						}
					}

					// Set state to disposed:
					this.LineBreakTimer = null;
					this.isDisposed = true;
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
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Implemented the same as every other IDisposable implementation.")]
			public bool IsDisposed
			{
				get { return (this.isDisposed); }
			}

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (this.isDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
			}

			#endregion

			public virtual void Reset()
			{
				AssertNotDisposed();

				LinePosition                  = LinePosition.Begin;
				LineElements                  = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				SequenceAfter                  .Reset();
				SequenceBefore                 .Reset();
				PendingSequenceBeforeElements = new List<DisplayElement>();
				TimeStamp                     = DateTime.Now;
			}
		}

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Private class.")]
		private class BidirLineState
		{
			public bool IsFirstLine;
			public string PortStamp;
			public IODirection Direction;

			public BidirLineState()
			{
				IsFirstLine = true;
				PortStamp   = "";
				Direction   = IODirection.None;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstLine = rhs.IsFirstLine;
				PortStamp   = rhs.PortStamp;
				Direction   = rhs.Direction;
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
				                                               // \remind (2016-09-08 / mky)
				if (this.txLineState.LineBreakTimer != null)   // Ensure to free referenced resources such as the 'Elapsed' event handler.
					this.txLineState.LineBreakTimer.Dispose(); // Whole timer handling should be encapsulated into the 'LineState' class.

				this.txLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				this.txLineState.LineBreakTimer.Elapsed += txTimedLineBreak_Elapsed;

				// Rx:

				this.rxLineState = casted.rxLineState;
				                                               // \remind (2016-09-08 / mky)
				if (this.rxLineState.LineBreakTimer != null)   // Ensure to free referenced resources such as the 'Elapsed' event handler.
					this.rxLineState.LineBreakTimer.Dispose(); // Whole timer handling should be encapsulated into the 'LineState' class.

				this.rxLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				this.rxLineState.LineBreakTimer.Elapsed += rxTimedLineBreak_Elapsed;

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

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------

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
						if (this.txLineState.LineElements.DataCount > 0)
							OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.DataSpace());
					}

					string info = (Parser.KeywordEx)(result.Keyword) + " keyword is not supported for binary terminals";
					OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(info));
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

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			using (Parser.Parser p = new Parser.Parser(TerminalSettings.IO.Endianness))
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

				this.txLineState = new LineState(new SequenceQueue(txSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), DateTime.Now, t);

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

				this.rxLineState = new LineState(new SequenceQueue(rxSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), DateTime.Now, t);
			}

			this.bidirLineState = new BidirLineState();
		}

		private void ExecuteLineBegin(LineState lineState, DateTime ts, string ps, IODirection d, DisplayElementCollection elements)
		{
			DisplayLinePart lp = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			lp.Add(new DisplayElement.LineStart()); // Direction may be both!

			if (TerminalSettings.Display.ShowDate || TerminalSettings.Display.ShowTime ||
				TerminalSettings.Display.ShowPort || TerminalSettings.Display.ShowDirection)
			{
				DisplayLinePart info;
				PrepareLineBeginInfo(ts, ps, d, out info);
				lp.AddRange(info);
			}

			lineState.LineElements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
			elements.AddRange(lp);

			lineState.LinePosition = LinePosition.Data;
			lineState.TimeStamp = ts;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void ExecuteData(Settings.BinaryDisplaySettings displaySettings, LineState lineState, IODirection d, byte b, DisplayElementCollection elements, out List<DisplayElement> elementsForNextLine)
		{
			elementsForNextLine = null;

			// Convert data:
			DisplayElement de = ByteToElement(b, d);
			DisplayLinePart lp = new DisplayLinePart(); // Default behaviour regarding initial capacity is OK.

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((displaySettings.SequenceLineBreakBefore.Enabled && (lineState.LineElements.DataCount > 0) &&
				(lineState.LinePosition != LinePosition.End)))   // Also skip if line has just been brokwn.
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

					lineState.LinePosition = LinePosition.End;
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
				AddSpaceIfNecessary(lineState, d, lp);
				lp.Add(de);
			}

			if (lineState.LinePosition != LinePosition.DataExceeded)
			{
				lineState.LineElements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elements.AddRange(lp);
			}

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((displaySettings.SequenceLineBreakAfter.Enabled) &&
				(lineState.LinePosition != LinePosition.End))
			{
				lineState.SequenceAfter.Enqueue(b);
				if (lineState.SequenceAfter.IsCompleteMatch) // No need to check for partly matches.
					lineState.LinePosition = LinePosition.End;
			}

			if ((displaySettings.LengthLineBreak.Enabled) &&
				(lineState.LinePosition != LinePosition.End))
			{
				if (lineState.LineElements.DataCount >= displaySettings.LengthLineBreak.Length)
					lineState.LinePosition = LinePosition.End;
			}

			if (lineState.LinePosition != LinePosition.End)
			{
				if ((lineState.LineElements.DataCount >= TerminalSettings.Display.MaxBytePerLineCount) &&
					(lineState.LinePosition != LinePosition.DataExceeded))
				{
					lineState.LinePosition = LinePosition.DataExceeded;

					string message = "Maximal number of bytes per line exceeded! Check the end-of-line settings or increase the limit in the advanced terminal settings.";
					lineState.LineElements.Add(new DisplayElement.ErrorInfo((Direction)d, message, true));
					elements.Add              (new DisplayElement.ErrorInfo((Direction)d, message, true));
				}
			}
		}

		private void TreatSequenceBeforeAsNormal(LineState lineState, IODirection d, DisplayLinePart lp)
		{
			if (lineState.PendingSequenceBeforeElements.Count > 0)
			{
				foreach (DisplayElement dePending in lineState.PendingSequenceBeforeElements)
				{
					AddSpaceIfNecessary(lineState, d, lp);
					lp.Add(dePending);
				}

				lineState.PendingSequenceBeforeElements.Clear();
			}
		}

		private void AddSpaceIfNecessary(LineState lineState, IODirection d, DisplayLinePart lp)
		{
			if (ElementsAreSeparate(d))
			{
				if (lineState.LineElements.DataCount > 0)
					lp.Add(new DisplayElement.DataSpace());
			}
		}

		private void ExecuteLineEnd(LineState lineState, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			// Note: Code sequence the same as ExecuteLineEnd() of TextTerminal for better comparability.

			DisplayLine line = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			lineState.Reset(); // Reset line state, it is no longer needed.

			// Process length:
			DisplayLinePart lp = new DisplayLinePart(); // Default behaviour regarding initial capacity is OK.

			if (TerminalSettings.Display.ShowLength)
			{
				DisplayLinePart info;
				PrepareLineEndInfo(lineState.LineElements.DataCount, out info);
				lp.AddRange(info);
			}

			lp.Add(new DisplayElement.LineBreak()); // Direction may be both!

			// Finalize elements and line:
			elements.AddRange(lp.Clone()); // Clone elements because they are needed again right below.
			line.AddRange(lp);
			lines.Add(line);
		}

		/// <summary></summary>
		protected override void ProcessRawChunk(RawChunk raw, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			Settings.BinaryDisplaySettings displaySettings;
			switch (raw.Direction)
			{
				case IODirection.Tx: displaySettings = BinaryTerminalSettings.TxDisplay; break;
				case IODirection.Rx: displaySettings = BinaryTerminalSettings.RxDisplay; break;
				default: throw (new NotSupportedException("Program execution should never get here, '" + raw.Direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			LineState lineState;
			switch (raw.Direction)
			{
				case IODirection.Tx: lineState = this.txLineState; break;
				case IODirection.Rx: lineState = this.rxLineState; break;
				default: throw (new NotSupportedException("Program execution should never get here, '" + raw.Direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			foreach (byte b in raw.Data)
			{
				// In case of reload, timed line breaks are executed here:
				if (IsReloading && displaySettings.TimedLineBreak.Enabled)
					ExecuteTimedLineBreakOnReload(displaySettings, lineState, raw.TimeStamp, elements, lines);

				// Line begin and time stamp:
				if (lineState.LinePosition == LinePosition.Begin)
				{
					ExecuteLineBegin(lineState, raw.TimeStamp, raw.PortStamp, raw.Direction, elements);

					if (displaySettings.TimedLineBreak.Enabled)
						lineState.LineBreakTimer.Start();
				}
				else
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.LineBreakTimer.Restart(); // Restart as timeout refers to time after last received byte.
				}

				// Data:
				List<DisplayElement> elementsForNextLine;
				ExecuteData(displaySettings, lineState, raw.Direction, b, elements, out elementsForNextLine);

				// Line end and length:
				if (lineState.LinePosition == LinePosition.End)
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.LineBreakTimer.Stop();

					ExecuteLineEnd(lineState, elements, lines);

					// In case of a pending immediately insert the sequence into a new line:
					if ((elementsForNextLine != null) && (elementsForNextLine.Count > 0))
					{
						ExecuteLineBegin(lineState, raw.TimeStamp, raw.PortStamp, raw.Direction, elements);

						foreach (DisplayElement de in elementsForNextLine)
						{
							if (de.Origin != null) // Foreach element where origin exists.
							{
								foreach (Pair<byte[], string> origin in de.Origin)
								{
									foreach (byte originByte in origin.Value1)
									{
										List<DisplayElement> elementsForNextLineDummy;
										ExecuteData(displaySettings, lineState, raw.Direction, originByte, elements, out elementsForNextLineDummy);

										// Note that 're.Direction' above is OK, this function is processing all in the same direction.
									}
								}
							}
						}
					}
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too long for one line.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too long for one line.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too long for one line.")]
		private void ExecuteTimedLineBreakOnReload(Settings.BinaryDisplaySettings displaySettings, LineState lineState,
		                                           DateTime ts, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			if (lineState.LineElements.Count > 0)
			{
				TimeSpan span = ts - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout) {
					ExecuteLineEnd(lineState, elements, lines);
				}
			}
			lineState.TimeStamp = ts;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		private void ProcessAndSignalPortAndDirectionLineBreak(string ps, IODirection d)
		{
			if (TerminalSettings.Display.PortLineBreakEnabled ||
				TerminalSettings.Display.DirectionLineBreakEnabled)
			{
				if (this.bidirLineState.IsFirstLine)
				{
					this.bidirLineState.IsFirstLine = false;
				}
				else // is subsequent line
				{
					if (!StringEx.EqualsOrdinalIgnoreCase(ps, this.bidirLineState.PortStamp) ||
						(d != this.bidirLineState.Direction))
					{
						LineState lineState;

						if (d == this.bidirLineState.Direction)
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.txLineState; break;
								case IODirection.Rx: lineState = this.rxLineState; break;
								default: throw (new NotSupportedException("Program execution should never get here, '" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else // Attention: Direction changed => Use opposite state.
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.rxLineState; break; // Reversed!
								case IODirection.Rx: lineState = this.txLineState; break;
								default: throw (new NotSupportedException("Program execution should never get here, '" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}

						if ((lineState.LineElements != null) && (lineState.LineElements.Count > 0))
						{
							DisplayElementCollection elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
							List<DisplayLine> lines = new List<DisplayLine>();

							ExecuteLineEnd(lineState, elements, lines);

							OnDisplayElementsProcessed(this.bidirLineState.Direction, elements);
							OnDisplayLinesProcessed   (this.bidirLineState.Direction, lines);
						}
					} // a line break has been detected
				} // is subsequent line
			} // a line break is active

			this.bidirLineState.PortStamp = ps;
			this.bidirLineState.Direction = d;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		private void ProcessAndSignalTimedLineBreak(IODirection d)
		{
			LineState lineState;
			switch (d)
			{
				case IODirection.Tx: lineState = this.txLineState; break;
				case IODirection.Rx: lineState = this.rxLineState; break;
				default: throw (new NotSupportedException("Program execution should never get here, '" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (lineState.LineElements.Count > 0)
			{
				DisplayElementCollection elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				List<DisplayLine> lines = new List<DisplayLine>();

				ExecuteLineEnd(lineState, elements, lines);

				OnDisplayElementsProcessed(d, elements);
				OnDisplayLinesProcessed(d, lines);
			}
		}

		/// <summary></summary>
		protected override void ProcessAndSignalRawChunk(RawChunk raw)
		{
			// Check whether port or direction has changed:
			ProcessAndSignalPortAndDirectionLineBreak(raw.PortStamp, raw.Direction);

			// Process the raw chunk:
			base.ProcessAndSignalRawChunk(raw);
		}

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		/// <remarks>Ensure that states are completely reset.</remarks>
		public override bool ReloadRepositories()
		{
			AssertNotDisposed();
			
			InitializeStates();
			return (base.ReloadRepositories());
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

		/// <summary></summary>
		public override string ToString()
		{
			AssertNotDisposed();
			
			return (ToString(""));
		}

		/// <summary></summary>
		public override string ToString(string indent)
		{
			AssertNotDisposed();
			
			return (indent + "> Type: BinaryTerminal" + Environment.NewLine + base.ToString(indent));
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
			ReloadRepositories();
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
			ProcessAndSignalTimedLineBreak(IODirection.Tx);
		}

		private void rxTimedLineBreak_Elapsed(object sender, EventArgs e)
		{
			ProcessAndSignalTimedLineBreak(IODirection.Rx);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
