//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Event;

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
		public class LineBreakTimer : IDisposable
		{
			private bool isDisposed;

			private int timeout;
			private Timer timer;

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
					if (disposing)
					{
						if (this.timer != null)
							this.timer.Dispose();
					}
					this.isDisposed = true;
				}
			}

			/// <summary></summary>
			~LineBreakTimer()
			{
				Dispose(false);
			}

			/// <summary></summary>
			protected bool IsDisposed
			{
				get { return (this.isDisposed); }
			}

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (this.isDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
			}

			#endregion

			/// <summary></summary>
			public virtual void Start()
			{
				AssertNotDisposed();

				TimerCallback timerDelegate = new TimerCallback(this.timer_Timeout);
				this.timer = new Timer(timerDelegate, null, this.timeout, System.Threading.Timeout.Infinite);
			}

			/// <summary></summary>
			public virtual void Restart()
			{
				AssertNotDisposed();

				Stop();
				Start();
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
			public virtual void Stop()
			{
				AssertNotDisposed();
				this.timer = null;
			}

			private void timer_Timeout(object obj)
			{
				OnTimeout(new EventArgs());
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

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "Private class.")]
		private class LineState
		{
			private bool isDisposed;

			public LinePosition LinePosition;
			public DisplayLine LineElements;
			public EolQueue SequenceBreak;
			public DateTime TimeStamp;
			public LineBreakTimer LineBreakTimer;

			public LineState(EolQueue sequenceBreak, DateTime timeStamp, LineBreakTimer lineBreakTimer)
			{
				LinePosition = BinaryTerminal.LinePosition.Begin;
				LineElements = new DisplayLine();
				SequenceBreak = sequenceBreak;
				TimeStamp = timeStamp;
				LineBreakTimer = lineBreakTimer;
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
					if (disposing)
					{
						if (this.LineBreakTimer != null)
							this.LineBreakTimer.Dispose();
					}
					this.isDisposed = true;
				}
			}

			/// <summary></summary>
			~LineState()
			{
				Dispose(false);
			}

			/// <summary></summary>
			protected bool IsDisposed
			{
				get { return (this.isDisposed); }
			}

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (this.isDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
			}

			#endregion

			public virtual void Reset()
			{
				AssertNotDisposed();

				LinePosition = BinaryTerminal.LinePosition.Begin;
				LineElements.Clear();
				SequenceBreak.Reset();
				TimeStamp = DateTime.Now;
			}
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "Private class.")]
		private class BidirLineState
		{
			public bool IsFirstLine;
			public SerialDirection Direction;

			public BidirLineState(bool isFirstLine, SerialDirection direction)
			{
				IsFirstLine = isFirstLine;
				Direction = direction;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstLine = rhs.IsFirstLine;
				Direction = rhs.Direction;
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
			if (terminal is BinaryTerminal)
			{
				BinaryTerminal casted = (BinaryTerminal)terminal;

				this.txLineState = casted.txLineState;
				this.txLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				this.txLineState.LineBreakTimer.Timeout += new EventHandler(this.txTimer_Timeout);

				this.rxLineState = casted.rxLineState;
				this.rxLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				this.rxLineState.LineBreakTimer.Timeout += new EventHandler(this.rxTimer_Timeout);

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
			if (disposing)
			{
				if (this.txLineState != null)
					this.txLineState.Dispose();

				if (this.rxLineState != null)
					this.rxLineState.Dispose();
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

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			Parser.Parser p = new Parser.Parser(TerminalSettings.IO.Endianess);
			LineBreakTimer t;

			// tx
			byte[] txSequenceBreak;
			if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreak.Sequence, out txSequenceBreak))
				txSequenceBreak = null;

			t = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
			t.Timeout += new EventHandler(this.txTimer_Timeout);

			this.txLineState = new LineState(new EolQueue(txSequenceBreak), DateTime.Now, t);

			// rx
			byte[] rxSequenceBreak;
			if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreak.Sequence, out rxSequenceBreak))
				rxSequenceBreak = null;

			t = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
			t.Timeout += new EventHandler(this.rxTimer_Timeout);
			this.rxLineState = new LineState(new EolQueue(rxSequenceBreak), DateTime.Now, t);

			this.bidirLineState = new BidirLineState(true, SerialDirection.Tx);
		}

		private void ExecuteLineBegin(Settings.BinaryDisplaySettings displaySettings, LineState lineState, DateTime ts, DisplayElementCollection elements)
		{
			if (TerminalSettings.Display.ShowTimeStamp)
			{
				DisplayLinePart lp = new DisplayLinePart();

				lp.Add(new DisplayElement.TimeStamp(ts));
				lp.Add(new DisplayElement.LeftMargin());

				// Attention: Clone elements because they are needed again below
				lineState.LineElements.AddRange(lp.Clone());
				elements.AddRange(lp);
			}

			lineState.LinePosition = LinePosition.Data;
			lineState.TimeStamp = ts;

			if (displaySettings.TimedLineBreak.Enabled)
				lineState.LineBreakTimer.Start();
		}

		private void ExecuteLineEnd(LineState lineState, SerialDirection d, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			DisplayLinePart lp = new DisplayLinePart();

			lineState.LineBreakTimer.Stop();

			if (TerminalSettings.Display.ShowLength)
			{
				int lineLength = 0;
				foreach (DisplayElement de in lineState.LineElements)
				{
					if (de.IsData)
						lineLength++;
				}
				lp.Add(new DisplayElement.RightMargin());
				lp.Add(new DisplayElement.LineLength(lineLength));
			}
			lp.Add(new DisplayElement.LineBreak(d));

			// Return elements
			elements.AddRange(lp);

			// Return line
			// Attention: Clone elements because they've also needed above
			lineState.LineElements.AddRange(lp);

			DisplayLine line = new DisplayLine();
			line.AddRange(lineState.LineElements);
			lines.Add(line);

			lineState.Reset();
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1115:ParameterMustFollowComma", Justification = "Too long for one line.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too long for one line.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too long for one line.")]
		private void ExecuteTimedLineBreakOnReload(Settings.BinaryDisplaySettings displaySettings, LineState lineState,
		                                           SerialDirection d, DateTime ts, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			if (lineState.LineElements.Count > 0)
			{
				TimeSpan span = ts - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout)
					ExecuteLineEnd(lineState, d, elements, lines);
			}
			lineState.TimeStamp = ts;
		}

		private void ExecuteLengthLineBreak(Settings.BinaryDisplaySettings displaySettings, LineState lineState)
		{
			int lineLength = 0;
			foreach (DisplayElement de in lineState.LineElements)
			{
				if (de.IsData)
					lineLength++;
			}
			if (lineLength >= displaySettings.LengthLineBreak.LineLength)
			{
				lineState.LinePosition = LinePosition.End;
			}
		}

		private void ExecuteData(SerialDirection d, LineState lineState, byte b, DisplayElementCollection elements)
		{
			DisplayLinePart lp = new DisplayLinePart();

			// add space if necessary
			if (ElementsAreSeparate(d))
			{
				int lineLength = 0;
				foreach (DisplayElement de in lineState.LineElements)
				{
					if (de.IsData)
						lineLength++;
				}
				if (lineLength > 0)
				{
					lp.Add(new DisplayElement.Space());
				}
			}

			// add data
			lp.Add(ByteToElement(b, d));

			// return data
			lineState.LineElements.AddRange(lp);
			elements.AddRange(lp);

			// evaluate binary sequence break
			lineState.SequenceBreak.Enqueue(b);
			if (lineState.SequenceBreak.IsCompleteMatch)
				lineState.LinePosition = LinePosition.End;
		}

		/// <summary></summary>
		protected override void ProcessRawElement(RawElement re, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			Settings.BinaryDisplaySettings displaySettings;
			if (re.Direction == SerialDirection.Tx)
				displaySettings = BinaryTerminalSettings.TxDisplay;
			else
				displaySettings = BinaryTerminalSettings.RxDisplay;

			LineState lineState;
			if (re.Direction == SerialDirection.Tx)
				lineState = this.txLineState;
			else
				lineState = this.rxLineState;

			foreach (byte b in re.Data)
			{
				// in case of reload, timed line breaks are executed here
				if (Reload && displaySettings.TimedLineBreak.Enabled)
					ExecuteTimedLineBreakOnReload(displaySettings, lineState, re.Direction, re.TimeStamp, elements, lines);

				// line begin
				if (lineState.LinePosition == LinePosition.Begin)
				{
					ExecuteLineBegin(displaySettings, lineState, re.TimeStamp, elements);
				}
				else
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.LineBreakTimer.Restart();
				}

				// data
				ExecuteData(re.Direction, lineState, b, elements);

				// length line breaks
				if (displaySettings.LengthLineBreak.Enabled)
					ExecuteLengthLineBreak(displaySettings, lineState);

				// line end and length
				if (lineState.LinePosition == LinePosition.End)
					ExecuteLineEnd(lineState, re.Direction, elements, lines);
			}
		}

		private void ProcessAndSignalDirectionLineBreak(SerialDirection d)
		{
			LineState lineState;
			if (d == SerialDirection.Tx)
				lineState = this.rxLineState;
			else
				lineState = this.txLineState;

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
						OnDisplayLinesProcessed(this.bidirLineState.Direction, lines);
					}
				}
			}
			this.bidirLineState.Direction = d;
		}

		private void ProcessAndSignalTimedLineBreak(SerialDirection d)
		{
			LineState lineState;
			if (d == SerialDirection.Tx)
				lineState = this.txLineState;
			else
				lineState = this.rxLineState;

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
			// check whether direction has changed
			ProcessAndSignalDirectionLineBreak(re.Direction);

			// process the raw element
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
			
			return (indent + "- Type: BinaryTerminal" + Environment.NewLine + base.ToString(indent));
		}

		#endregion

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachBinaryTerminalSettings(Settings.BinaryTerminalSettings binaryTerminalSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(TerminalSettings.BinaryTerminal, binaryTerminalSettings))
				return;

			if (TerminalSettings.BinaryTerminal != null)
				DetachBinaryTerminalSettings();

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
			ProcessAndSignalTimedLineBreak(SerialDirection.Tx);
		}

		private void rxTimer_Timeout(object sender, EventArgs e)
		{
			ProcessAndSignalTimedLineBreak(SerialDirection.Rx);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
