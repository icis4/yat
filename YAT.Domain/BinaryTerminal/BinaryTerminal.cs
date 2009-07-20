//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

using MKY.Utilities.Event;
using MKY.Utilities.Settings;

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
		public class LineBreakTimer
		{
			private int _timeout;
			private Timer _timer;

			/// <summary></summary>
			public event EventHandler Timeout;

			/// <summary></summary>
			public LineBreakTimer(int timeout)
			{
				_timeout = timeout;
			}

			/// <summary></summary>
			public void Start()
			{
				TimerCallback timerDelegate = new TimerCallback(_timer_Timeout);
				_timer = new Timer(timerDelegate, null, _timeout, System.Threading.Timeout.Infinite);
			}

			/// <summary></summary>
			public void Restart()
			{
				Stop();
				Start();
			}

			/// <summary></summary>
			public void Stop()
			{
				_timer = null; ;
			}

			private void _timer_Timeout(object obj)
			{
				OnTimeout(new EventArgs());
			}

			/// <summary></summary>
			protected virtual void OnTimeout(EventArgs e)
			{
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

		private class LineState
		{
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

			public void Reset()
			{
				LinePosition = BinaryTerminal.LinePosition.Begin;
				LineElements.Clear();
				SequenceBreak.Reset();
				TimeStamp = DateTime.Now;
			}
		}

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

		private LineState _txLineState;
		private LineState _rxLineState;

		private BidirLineState _bidirLineState;

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

				_txLineState = casted._txLineState;
				_txLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				_txLineState.LineBreakTimer.Timeout += new EventHandler(_txTimer_Timeout);

				_rxLineState = casted._rxLineState;
				_rxLineState.LineBreakTimer = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				_rxLineState.LineBreakTimer.Timeout += new EventHandler(_rxTimer_Timeout);

				_bidirLineState = new BidirLineState(casted._bidirLineState);
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
			t.Timeout += new EventHandler(_txTimer_Timeout);

			_txLineState = new LineState(new EolQueue(txSequenceBreak), DateTime.Now, t);

			// rx
			byte[] rxSequenceBreak;
			if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreak.Sequence, out rxSequenceBreak))
				rxSequenceBreak = null;

			t = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
			t.Timeout += new EventHandler(_rxTimer_Timeout);
			_rxLineState = new LineState(new EolQueue(rxSequenceBreak), DateTime.Now, t);

			_bidirLineState = new BidirLineState(true, SerialDirection.Tx);
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

		private void ExecuteLineEnd(LineState lineState, DisplayElementCollection elements, List<DisplayLine> lines)
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
			lp.Add(new DisplayElement.LineBreak());

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

		private void ExecuteTimedLineBreakOnReload(Settings.BinaryDisplaySettings displaySettings,
			                                       LineState lineState, DateTime ts,
												   DisplayElementCollection elements, List<DisplayLine> lines)
		{
			if (lineState.LineElements.Count > 0)
			{
				TimeSpan span = ts - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout)
					ExecuteLineEnd(lineState, elements, lines);
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

		private void ExecuteData(SerialDirection direction, LineState lineState, byte b, DisplayElementCollection elements)
		{
			DisplayLinePart lp = new DisplayLinePart();

			// add space if necessary
			if (ElementsAreSeparate(direction))
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
			lp.Add(ByteToElement(b, direction));

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
				lineState = _txLineState;
			else
				lineState = _rxLineState;

			foreach (byte b in re.Data)
			{
				// in case of reload, timed line breaks are executed here
				if (Reload && displaySettings.TimedLineBreak.Enabled)
					ExecuteTimedLineBreakOnReload(displaySettings, lineState, re.TimeStamp, elements, lines);

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
					ExecuteLineEnd(lineState, elements, lines);
			}
		}

		private void ProcessAndSignalDirectionLineBreak(SerialDirection direction)
		{
			LineState lineState;
			if (direction == SerialDirection.Tx)
				lineState = _rxLineState;
			else
				lineState = _txLineState;

			if (TerminalSettings.Display.DirectionLineBreakEnabled)
			{
				if (_bidirLineState.IsFirstLine)
				{
					_bidirLineState.IsFirstLine = false;
				}
				else
				{
					if ((lineState.LineElements.Count > 0) &&
						(direction != _bidirLineState.Direction))
					{
						DisplayElementCollection elements = new DisplayElementCollection();
						List<DisplayLine> lines = new List<DisplayLine>();

						ExecuteLineEnd(lineState, elements, lines);

						OnDisplayElementsProcessed(_bidirLineState.Direction, elements);
						OnDisplayLinesProcessed(_bidirLineState.Direction, lines);
					}
				}
			}
			_bidirLineState.Direction = direction;
		}

		private void ProcessAndSignalTimedLineBreak(SerialDirection direction)
		{
			LineState lineState;
			if (direction == SerialDirection.Tx)
				lineState = _txLineState;
			else
				lineState = _rxLineState;

			if (lineState.LineElements.Count > 0)
			{
				DisplayElementCollection elements = new DisplayElementCollection();
				List<DisplayLine> lines = new List<DisplayLine>();

				ExecuteLineEnd(lineState, elements, lines);

				OnDisplayElementsProcessed(direction, elements);
				OnDisplayLinesProcessed(direction, lines);
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
			BinaryTerminalSettings.Changed += new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(BinaryTerminalSettings_Changed);
		}

		private void DetachBinaryTerminalSettings()
		{
			BinaryTerminalSettings.Changed -= new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(BinaryTerminalSettings_Changed);
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

		private void BinaryTerminalSettings_Changed(object sender, MKY.Utilities.Settings.SettingsEventArgs e)
		{
			ApplyBinaryTerminalSettings();
		}

		#endregion

		#region Timer Events
		//==========================================================================================
		// Timer Events
		//==========================================================================================

		private void _txTimer_Timeout(object sender, EventArgs e)
		{
			ProcessAndSignalTimedLineBreak(SerialDirection.Tx);
		}

		private void _rxTimer_Timeout(object sender, EventArgs e)
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
