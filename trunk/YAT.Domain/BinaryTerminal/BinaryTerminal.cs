using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

using MKY.Utilities.Event;
using MKY.Utilities.Settings;

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
			public List<DisplayElement> LineElements;
			public DateTime TimeStamp;
			public LineBreakTimer LineBreakTimer;

			public LineState(DateTime timeStamp, LineBreakTimer lineBreakTimer)
			{
				LinePosition = BinaryTerminal.LinePosition.Begin;
				LineElements = new List<DisplayElement>();
				TimeStamp = timeStamp;
				LineBreakTimer = lineBreakTimer;
			}

			public void Reset()
			{
				LinePosition = BinaryTerminal.LinePosition.Begin;
				LineElements.Clear();
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
			LineBreakTimer t;

			t = new LineBreakTimer(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
			t.Timeout += new EventHandler(_txTimer_Timeout);
			_txLineState = new LineState(DateTime.Now, t);

			t = new LineBreakTimer(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
			t.Timeout += new EventHandler(_rxTimer_Timeout);
			_rxLineState = new LineState(DateTime.Now, t);

			_bidirLineState = new BidirLineState(true, SerialDirection.Tx);
		}

		private void ExecuteLineBegin(Settings.BinaryDisplaySettings displaySettings, LineState lineState, DateTime timeStamp, List<DisplayElement> elements)
		{
			if (TerminalSettings.Display.ShowTimeStamp)
			{
				List<DisplayElement> l = new List<DisplayElement>();

				l.Add(new DisplayElement.TimeStamp(timeStamp));
				l.Add(new DisplayElement.LeftMargin());

				lineState.LineElements.AddRange(l);
				elements.AddRange(l);
			}

			lineState.LinePosition = LinePosition.Data;
			lineState.TimeStamp = timeStamp;

			if (displaySettings.TimedLineBreak.Enabled)
				lineState.LineBreakTimer.Start();
		}

		private void ExecuteLineEnd(LineState lineState, List<DisplayElement> elements, List<List<DisplayElement>> lines)
		{
			List<DisplayElement> l = new List<DisplayElement>();

			lineState.LineBreakTimer.Stop();

			if (TerminalSettings.Display.ShowLength)
			{
				int lineLength = 0;
				foreach (DisplayElement de in lineState.LineElements)
				{
					if (de.IsDataElement)
						lineLength++;
				}
				l.Add(new DisplayElement.RightMargin());
				l.Add(new DisplayElement.LineLength(lineLength));
			}
			l.Add(new DisplayElement.LineBreak());

			// return elements
			elements.AddRange(l);

			// return line
			lineState.LineElements.AddRange(l);
			List<DisplayElement> line = new List<DisplayElement>();
			line.AddRange(lineState.LineElements);
			lines.Add(line);

			lineState.Reset();
		}

		private void ExecuteTimedLineBreakOnReload(Settings.BinaryDisplaySettings displaySettings, LineState lineState, DateTime timeStamp, List<DisplayElement> elements, List<List<DisplayElement>> lines)
		{
			if (lineState.LineElements.Count > 0)
			{
				TimeSpan span = timeStamp - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout)
					ExecuteLineEnd(lineState, elements, lines);
			}
			lineState.TimeStamp = timeStamp;
		}

		private void ExecuteLengthLineBreak(Settings.BinaryDisplaySettings displaySettings, LineState lineState)
		{
			int lineLength = 0;
			foreach (DisplayElement de in lineState.LineElements)
			{
				if (de.IsDataElement)
					lineLength++;
			}
			if (lineLength >= displaySettings.LengthLineBreak.LineLength)
			{
				lineState.LinePosition = LinePosition.End;
			}
		}

		private void ExecuteData(SerialDirection direction, LineState lineState, byte b, List<DisplayElement> elements)
		{
			List<DisplayElement> l = new List<DisplayElement>();

			// add space if necessary
			if (ElementsAreSeparate())
			{
				int lineLength = 0;
				foreach (DisplayElement de in lineState.LineElements)
				{
					if (de.IsDataElement)
						lineLength++;
				}
				if (lineLength > 0)
				{
					l.Add(new DisplayElement.Space());
				}
			}

			// add data
			l.Add(ByteToElement(b, direction));

			// return data
			lineState.LineElements.AddRange(l);
			elements.AddRange(l);
		}

		/// <summary></summary>
		protected override void ProcessRawElement(RawElement re, List<DisplayElement> elements, List<List<DisplayElement>> lines)
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

			if (BinaryTerminalSettings.DirectionLineBreakEnabled)
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
						List<DisplayElement> elements = new List<DisplayElement>();
						List<List<DisplayElement>> lines = new List<List<DisplayElement>>();

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
				List<DisplayElement> elements = new List<DisplayElement>();
				List<List<DisplayElement>> lines = new List<List<DisplayElement>>();

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
		public override void ReloadRepositories()
		{
			AssertNotDisposed();
			
			Initialize();
			base.ReloadRepositories();
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

		/// <summary></summary>
		/// <remarks>ToDo: Why isn't this function CLS compliant?</remarks>
		[CLSCompliant(false)]
		protected virtual void _txTimer_Timeout(object sender, EventArgs e)
		{
			ProcessAndSignalTimedLineBreak(SerialDirection.Tx);
		}

		/// <summary></summary>
		/// <remarks>ToDo: Why isn't this function CLS compliant?</remarks>
		[CLSCompliant(false)]
		protected virtual void _rxTimer_Timeout(object sender, EventArgs e)
		{
			ProcessAndSignalTimedLineBreak(SerialDirection.Rx);
		}

		#endregion
	}
}
