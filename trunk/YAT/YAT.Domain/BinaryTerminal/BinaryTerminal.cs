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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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

		#region Line Break Timer
		//------------------------------------------------------------------------------------------
		// Line Break Timer
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		private class LineBreakTimer : IDisposable
		{
			private bool isDisposed;

			private int timeout;
			private System.Threading.Timer timer;

			/// <summary></summary>
			public event EventHandler Timeout;

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

			/// <summary></summary>
			~LineBreakTimer()
			{
				Dispose(false);

				System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
			}

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

				EventHelper.FireSync(Timeout, this, e);
			}
		}

		#endregion

		#region Line State
		//------------------------------------------------------------------------------------------
		// Line State
		//------------------------------------------------------------------------------------------

		private enum LinePosition
		{
			Begin,
			Data,
			End
		}

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Private class.")]
		private class LineState : IDisposable
		{
			private bool isDisposed;

			public LinePosition         LinePosition;
			public DisplayLine          LineElements;
			public SequenceQueue        SequenceAfter;
			public SequenceQueue        SequenceBefore;
			public List<DisplayElement> PendingSequenceBeforeElements;
			public DateTime             TimeStamp;
			public LineBreakTimer       LineBreakTimer;

			public LineState(SequenceQueue sequenceAfter, SequenceQueue sequenceBefore, DateTime timeStamp, LineBreakTimer lineBreakTimer)
			{
				LinePosition                  = LinePosition.Begin;
				LineElements                  = new DisplayLine();
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
							this.LineBreakTimer.Dispose();
					}

					// Set state to disposed:
					this.LineBreakTimer = null;
					this.isDisposed = true;
				}
			}

			/// <summary></summary>
			~LineState()
			{
				Dispose(false);

				System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
			}

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
				LineElements                  = new DisplayLine();
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
			public IODirection Direction;

			public BidirLineState(bool isFirstLine, IODirection direction)
			{
				IsFirstLine = isFirstLine;
				Direction   = direction;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstLine = rhs.IsFirstLine;
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
			AttachBinaryTerminalSettings(settings.BinaryTerminal);
			Initialize();
		}

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachBinaryTerminalSettings(settings.BinaryTerminal);

			var casted = (terminal as BinaryTerminal);
			if (casted != null)
			{
				this.txLineState = casted.txLineState;
				this.txLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				this.txLineState.LineBreakTimer.Timeout += new EventHandler(txTimer_Timeout);

				this.rxLineState = casted.rxLineState;
				this.rxLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				this.rxLineState.LineBreakTimer.Timeout += new EventHandler(rxTimer_Timeout);

				this.bidirLineState = new BidirLineState(casted.bidirLineState);
			}
			else
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			InitializeStates();
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
				// In any case, dispose of the state objects as they were created in the constructor.
				if (this.txLineState != null)
					this.txLineState.Dispose();

				if (this.rxLineState != null)
					this.rxLineState.Dispose();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					DetachBinaryTerminalSettings();
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

		/// <summary></summary>
		public Settings.BinaryTerminalSettings BinaryTerminalSettings
		{
			get
			{
				AssertNotDisposed();

				return (TerminalSettings.BinaryTerminal);
			}
			set
			{
				AssertNotDisposed();

				AttachBinaryTerminalSettings(value);
				ApplyBinaryTerminalSettings();
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
		protected override void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Eol:
				case Parser.Keyword.NoEol:
				{
					// Add space if necessary.
					if (ElementsAreSeparate(IODirection.Tx))
					{
						if (this.txLineState.LineElements.DataCount > 0)
							OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.Space());
					}

					OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo((Parser.KeywordEx)(result.Keyword) + " keyword is not supported for binary terminals"));
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
				t.Timeout += new EventHandler(txTimer_Timeout);

				this.txLineState = new LineState(new SequenceQueue(txSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), DateTime.Now, t);

				// Rx:

				byte[] rxSequenceBreakAfter;
				if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakAfter.Sequence, out rxSequenceBreakAfter))
					rxSequenceBreakAfter = null;

				byte[] rxSequenceBreakBefore;
				if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakBefore.Sequence, out rxSequenceBreakBefore))
					rxSequenceBreakBefore = null;

				t = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				t.Timeout += new EventHandler(rxTimer_Timeout);

				this.rxLineState = new LineState(new SequenceQueue(rxSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), DateTime.Now, t);
			}

			this.bidirLineState = new BidirLineState(true, IODirection.Tx);
		}

		private void ExecuteLineBegin(Settings.BinaryDisplaySettings displaySettings, LineState lineState, DateTime ts, string ps, IODirection d, DisplayElementCollection elements)
		{
			if (TerminalSettings.Display.ShowDate || TerminalSettings.Display.ShowTime ||
				TerminalSettings.Display.ShowPort || TerminalSettings.Display.ShowDirection)
			{
				DisplayLinePart lp = new DisplayLinePart();

				if (TerminalSettings.Display.ShowDate)
					lp.Add(new DisplayElement.DateInfo(ts));

				if (TerminalSettings.Display.ShowTime)
					lp.Add(new DisplayElement.TimeInfo(ts));

				if (TerminalSettings.Display.ShowPort)
					lp.Add(new DisplayElement.PortInfo((Direction)d, ps));

				if (TerminalSettings.Display.ShowDirection)
					lp.Add(new DisplayElement.DirectionInfo((Direction)d));

				lp.Add(new DisplayElement.LeftMargin());

				lineState.LineElements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elements.AddRange(lp);
			}

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
			DisplayLinePart lp = new DisplayLinePart();

			// Evaluate line breaks:
			//  1. Evaluate the easiest case: Length line break.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the tricky case: Sequence before.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((displaySettings.LengthLineBreak.Enabled) &&
				(lineState.LinePosition != LinePosition.End))
			{
				if (lineState.LineElements.DataCount >= displaySettings.LengthLineBreak.Length)
					lineState.LinePosition = LinePosition.End;
			}

			if ((displaySettings.SequenceLineBreakAfter.Enabled) &&
				(lineState.LinePosition != LinePosition.End))
			{
				lineState.SequenceAfter.Enqueue(b);
				if (lineState.SequenceAfter.IsCompleteMatch) // No need to check for partly matches.
					lineState.LinePosition = LinePosition.End;
			}

			if ((displaySettings.SequenceLineBreakBefore.Enabled && (lineState.LineElements.DataCount > 0) &&
				(lineState.LinePosition != LinePosition.End)))   // Also skip if line has just been brokwn.
			{
				lineState.SequenceBefore.Enqueue(b);
				if (lineState.SequenceBefore.IsCompleteMatch)
				{
					// Sequence is complete, move them to the next line:
					lineState.PendingSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.

					elementsForNextLine = new List<DisplayElement>();
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

			lineState.LineElements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
			elements.AddRange(lp);
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
					lp.Add(new DisplayElement.Space());
			}
		}

		private void ExecuteLineEnd(LineState lineState, IODirection d, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			DisplayLinePart lp = new DisplayLinePart();

			if (TerminalSettings.Display.ShowLength)
			{
				int length = 0;
				foreach (DisplayElement e in lineState.LineElements)
				{
					if (e.IsData)
						length += e.DataCount;
				}
				lp.Add(new DisplayElement.RightMargin());
				lp.Add(new DisplayElement.Length(length));
			}

			lp.Add(new DisplayElement.LineBreak((Direction)d));

			lineState.LineElements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
			elements.AddRange(lp);

			DisplayLine line = new DisplayLine();
			line.AddRange(lineState.LineElements.Clone()); // Clone elements to ensure decoupling.
			lineState.Reset();

			lines.Add(line);
		}

		/// <summary></summary>
		protected override void ProcessRawElement(RawElement re, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			Settings.BinaryDisplaySettings displaySettings;
			switch (re.Direction)
			{
				case IODirection.Tx: displaySettings = BinaryTerminalSettings.TxDisplay; break;
				case IODirection.Rx: displaySettings = BinaryTerminalSettings.RxDisplay; break;
				default: throw (new NotSupportedException("Program execution should never get here, '" + re.Direction + "' is an invalid direction." + Environment.NewLine + Environment.NewLine + MKY.Windows.Forms.ApplicationEx.SubmitBugMessage));
			}

			LineState lineState;
			switch (re.Direction)
			{
				case IODirection.Tx: lineState = this.txLineState; break;
				case IODirection.Rx: lineState = this.rxLineState; break;
				default: throw (new NotSupportedException("Program execution should never get here, '" + re.Direction + "' is an invalid direction." + Environment.NewLine + Environment.NewLine + MKY.Windows.Forms.ApplicationEx.SubmitBugMessage));
			}

			foreach (byte b in re.Data)
			{
				// In case of reload, timed line breaks are executed here:
				if (IsReloading && displaySettings.TimedLineBreak.Enabled)
					ExecuteTimedLineBreakOnReload(displaySettings, lineState, re.TimeStamp, re.Direction, elements, lines);

				// Line begin and time stamp:
				if (lineState.LinePosition == LinePosition.Begin)
				{
					ExecuteLineBegin(displaySettings, lineState, re.TimeStamp, re.PortStamp, re.Direction, elements);

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
				ExecuteData(displaySettings, lineState, re.Direction, b, elements, out elementsForNextLine);

				// Line end and length:
				if (lineState.LinePosition == LinePosition.End)
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.LineBreakTimer.Stop();

					ExecuteLineEnd(lineState, re.Direction, elements, lines);

					// In case of a pending immediately insert the sequence into a new line:
					if ((elementsForNextLine != null) && (elementsForNextLine.Count > 0))
					{
						ExecuteLineBegin(displaySettings, lineState, re.TimeStamp, re.PortStamp, re.Direction, elements);

						foreach (DisplayElement de in elementsForNextLine)
						{
							foreach (Pair<byte[], string> origin in de.Origin)
							{
								foreach (byte originByte in origin.Value1)
								{
									List<DisplayElement> elementsForNextLineDummy;
									ExecuteData(displaySettings, lineState, re.Direction, originByte, elements, out elementsForNextLineDummy);
									// Note that 're.Direction' abive is OK, this function is processing all in the same direction.
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
		                                           DateTime ts, IODirection d, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			if (lineState.LineElements.Count > 0)
			{
				TimeSpan span = ts - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout) {
					ExecuteLineEnd(lineState, d, elements, lines);
				}
			}
			lineState.TimeStamp = ts;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		private void ProcessAndSignalDirectionLineBreak(IODirection d)
		{
			LineState lineState;
			switch (d)
			{
				case IODirection.Tx: lineState = this.rxLineState; break; // Reversed!
				case IODirection.Rx: lineState = this.txLineState; break;
				default: throw (new NotSupportedException("Program execution should never get here, '" + d + "' is an invalid direction." + Environment.NewLine + Environment.NewLine + MKY.Windows.Forms.ApplicationEx.SubmitBugMessage));
			}

			if (TerminalSettings.Display.DirectionLineBreakEnabled)
			{
				if (this.bidirLineState.IsFirstLine)
				{
					this.bidirLineState.IsFirstLine = false;
				}
				else
				{
					if ((lineState.LineElements.Count > 0) &&
						(d != this.bidirLineState.Direction))
					{
						DisplayElementCollection elements = new DisplayElementCollection();
						List<DisplayLine> lines = new List<DisplayLine>();

						ExecuteLineEnd(lineState, d, elements, lines);

						OnDisplayElementsProcessed(this.bidirLineState.Direction, elements);
						OnDisplayLinesProcessed   (this.bidirLineState.Direction, lines);
					}
				}
			}
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
				default: throw (new NotSupportedException("Program execution should never get here, '" + d + "' is an invalid direction." + Environment.NewLine + Environment.NewLine + MKY.Windows.Forms.ApplicationEx.SubmitBugMessage));
			}

			if (lineState.LineElements.Count > 0)
			{
				DisplayElementCollection elements = new DisplayElementCollection();
				List<DisplayLine> lines = new List<DisplayLine>();

				ExecuteLineEnd(lineState, d, elements, lines);

				OnDisplayElementsProcessed(d, elements);
				OnDisplayLinesProcessed(d, lines);
			}
		}

		/// <summary></summary>
		protected override void ProcessAndSignalRawElement(RawElement re)
		{
			// Check whether direction has changed:
			ProcessAndSignalDirectionLineBreak(re.Direction);

			// Process the raw element:
			base.ProcessAndSignalRawElement(re);
		}

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <remarks>Ensure that line states are completely reset.</remarks>
		public override void ReloadRepositories()
		{
			AssertNotDisposed();
			
			Initialize();
			base.ReloadRepositories();
		}

		/// <summary></summary>
		/// <remarks>Ensure that line states are completely reset.</remarks>
		protected override void ClearMyRepository(RepositoryType repository)
		{
			Initialize();
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

		private void AttachBinaryTerminalSettings(Settings.BinaryTerminalSettings binaryTerminalSettings)
		{
			TerminalSettings.BinaryTerminal = binaryTerminalSettings;

			BinaryTerminalSettings.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(BinaryTerminalSettings_Changed);
		}

		private void DetachBinaryTerminalSettings()
		{
			BinaryTerminalSettings.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(BinaryTerminalSettings_Changed);
		}

		private void ApplyBinaryTerminalSettings()
		{
			InitializeStates();
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

		private void txTimer_Timeout(object sender, EventArgs e)
		{
			ProcessAndSignalTimedLineBreak(IODirection.Tx);
		}

		private void rxTimer_Timeout(object sender, EventArgs e)
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
