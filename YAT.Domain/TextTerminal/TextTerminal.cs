using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MKY.Utilities.Text;
using MKY.Utilities.Types;

namespace YAT.Domain
{
	/// <summary>
	/// Text protocol terminal.
	/// </summary>
	public class TextTerminal : Terminal
	{
		#region Help
		//==========================================================================================
		// Help
		//==========================================================================================

		/// <summary></summary>
		public static readonly string KeywordHelp =
			"For text terminals, the following additional keywords are supported :" + Environment.NewLine +
			Environment.NewLine +
			"Send EOL sequence \"OK\\!(" + (Parser.XKeyword)Parser.Keyword.Eol + ")\"" + Environment.NewLine +
			"Do not send EOL \"OK\\!(" + (Parser.XKeyword)Parser.Keyword.NoEol + ")\"" + Environment.NewLine +
			"\"\\!(" + (Parser.XKeyword)Parser.Keyword.NoEol + ")\" is useful if your text protocol does have an EOL sequence" + Environment.NewLine +
			"  except for a few special commands (e.g. synchronization commands).";

		#endregion

		#region Element State
		//==========================================================================================
		// Element State
		//==========================================================================================

		private List<byte> _rxDecodingStream;

		#endregion

		#region Line State
		//==========================================================================================
		// Line State
		//==========================================================================================

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
			public EolQueue Eol;

			public LineState(EolQueue eol)
			{
				LinePosition = TextTerminal.LinePosition.Begin;
				LineElements = new List<DisplayElement>();
				Eol = eol;
			}

			public void Reset()
			{
				LinePosition = TextTerminal.LinePosition.Begin;
				LineElements.Clear();
				Eol.Reset();
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
		public TextTerminal(Settings.TerminalSettings settings)
			: base(settings)
		{
			AttachTextTerminalSettings(settings.TextTerminal);
			Initialize();
		}

		/// <summary></summary>
		public TextTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachTextTerminalSettings(settings.TextTerminal);
			if (terminal is TextTerminal)
			{
				TextTerminal casted = (TextTerminal)terminal;
				_rxDecodingStream = casted._rxDecodingStream;
				_txLineState = casted._txLineState;
				_rxLineState = casted._rxLineState;

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
		public Settings.TextTerminalSettings TextTerminalSettings
		{
			get
			{
				AssertNotDisposed();
				return (TerminalSettings.TextTerminal);
			}
			set
			{
				AssertNotDisposed();
				AttachTextTerminalSettings(value);
				ApplyTextTerminalSettings();
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

		/// <summary></summary>
		public override void Send(string s)
		{
			AssertNotDisposed();
			Send(s, null);
		}

		/// <summary></summary>
		public override void SendLine(string line)
		{
			AssertNotDisposed();
			Send(line, TextTerminalSettings.TxEol);
		}

		private void Send(string s, string eol)
		{
			Parser.SubstitutionParser p = new Parser.SubstitutionParser(TerminalSettings.IO.Endianess, (XEncoding)TextTerminalSettings.Encoding);

			// prepare eol
			MemoryStream eolWriter = new MemoryStream();;
			foreach (Parser.Result result in p.Parse(eol, TextTerminalSettings.CharSubstitution, Parser.ParseMode.AllByteArrayResults))
			{
				if (result is Parser.ByteArrayResult)
				{
					byte[] a = ((Parser.ByteArrayResult)result).ByteArray;
					eolWriter.Write(a, 0, a.Length);
				}
			}
			byte[] eolByteArray = eolWriter.ToArray();

			// parse string and execute keywords
			bool sendEol = (eol != null);
			foreach (Parser.Result result in p.Parse(s, TextTerminalSettings.CharSubstitution, Parser.ParseMode.All))
			{
				if (result is Parser.ByteArrayResult)
				{
					Send(((Parser.ByteArrayResult)result).ByteArray);
				}
				else if (result is Parser.KeywordResult)
				{
					switch (((Parser.KeywordResult)result).Keyword)
					{
						case Parser.Keyword.Delay:
							// \fixme
							break;

						case Parser.Keyword.Eol:
							Send(eolByteArray);
							break;

						case Parser.Keyword.NoEol:
							sendEol = false;
							break;
					}
				}
			}

			// finally send EOL
			if (sendEol)
			{
				Send(eolByteArray);
			}
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			_rxDecodingStream = new List<byte>();

			Parser.SubstitutionParser p = new Parser.SubstitutionParser(TerminalSettings.IO.Endianess, (XEncoding)TextTerminalSettings.Encoding);
			byte[] txEol;
			if (!p.TryParse(TextTerminalSettings.TxEol, TextTerminalSettings.CharSubstitution, out txEol))
			{
				TextTerminalSettings.TxEol = Settings.TextTerminalSettings.DefaultEol;
				txEol = p.Parse(TextTerminalSettings.TxEol, TextTerminalSettings.CharSubstitution);
			}
			byte[] rxEol;
			if (!p.TryParse(TextTerminalSettings.RxEol, TextTerminalSettings.CharSubstitution, out rxEol))
			{
				TextTerminalSettings.RxEol = Settings.TextTerminalSettings.DefaultEol;
				rxEol = p.Parse(TextTerminalSettings.RxEol, TextTerminalSettings.CharSubstitution);
			}
			_txLineState = new LineState(new EolQueue(txEol));
			_rxLineState = new LineState(new EolQueue(rxEol));

			_bidirLineState = new BidirLineState(true, SerialDirection.Tx);
		}

		/// <summary></summary>
		protected override DisplayElement ByteToElement(byte b, SerialDirection d, Radix r)
		{
			switch (r)
			{
				// Bin/Oct/Dec/Hex
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					if (((b < 0x20) || (b == 0x7F)) && (TextTerminalSettings.ControlCharRadix == ControlCharRadix.AsciiMnemonic))
					{
						string data = "";

						if (b < 0x20)
							data = "<" + Ascii.ConvertToMnemonic(b) + ">";
						if (b == 0x7F)
							data = "<DEL>";

						if (d == SerialDirection.Tx)
							return (new DisplayElement.TxControl(new ElementOrigin(b, d), data));
						else
							return (new DisplayElement.RxControl(new ElementOrigin(b, d), data));
					}
					else
					{
						return (base.ByteToElement(b, d, r));
					}
				}
				// Char/String
				case Radix.Char:
				case Radix.String:
				{
					// add byte to ElementState
					_rxDecodingStream.Add(b);
					byte[] decodingArray = _rxDecodingStream.ToArray();

					// get encoding and retrieve char count
					Encoding e = (XEncoding)TextTerminalSettings.Encoding;
					int charCount = e.GetCharCount(decodingArray);

					// if decoding array can be decoded into something useful, decode it
					if (charCount > 0)
					{
						_rxDecodingStream.Clear();

						// char count must be 1, otherwise something went wrong
						char[] chars = new char[1];
						if (e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true) == 1)
						{
							// treat ASCII control charaters separately
							int code = (int)chars[0];
							if (((code < 0x20) || (code == 0x7F)) && (TextTerminalSettings.ControlCharRadix == ControlCharRadix.AsciiMnemonic))
							{
								string data = "";

								if (code < 0x20)
									data = "<" + Ascii.ConvertToMnemonic((byte)code) + ">";
								if (code == 0x7F)
									data = "<DEL>";

								if (d == SerialDirection.Tx)
									return (new DisplayElement.TxControl(new ElementOrigin(b, d), data));
								else
									return (new DisplayElement.RxControl(new ElementOrigin(b, d), data));
							}
							else
							{
								StringBuilder sb = new StringBuilder();
								sb.Append(chars, 0, charCount);
								if (d == SerialDirection.Tx)
									return (new DisplayElement.TxData(new ElementOrigin(b, d), sb.ToString()));
								else
									return (new DisplayElement.RxData(new ElementOrigin(b, d), sb.ToString()));
							}
						}
					}

					// nothing to decode (yet)
					return (new DisplayElement.NoData());
				}
				default: throw (new NotImplementedException("Unknown Radix"));
			}
		}

		private void ExecuteLineBegin(LineState lineState, DateTime timeStamp, List<DisplayElement> elements)
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
		}

		private void ExecuteLineEnd(SerialDirection d, LineState lineState, List<DisplayElement> elements, List<List<DisplayElement>> lines)
		{
			// process EOL
			int eolLength = lineState.Eol.Eol.Length;
			List<DisplayElement> line = new List<DisplayElement>();

			if (TextTerminalSettings.ShowEol || (eolLength <= 0))
			{
				line.AddRange(lineState.LineElements);
			}
			else // remove EOL if desired
			{
				int eolElementCount = 0;
				int eolAndWhiteElementCount = 0;
				DisplayElement[] des = lineState.LineElements.ToArray();

				// traverse elements reverse and count EOL data and white space elements to be removed
				for (int i = (des.Length - 1); i >= 0; i--)
				{
					if (des[i].IsDataElement)
					{
						// detect last non-EOL data element
						if (eolElementCount >= eolLength)
							break;

						// loop through all EOL data elements
						eolElementCount++;
					}
					eolAndWhiteElementCount++;
				}

				// now traverse elements forward and add elements to line
				for (int i = 0; i < (des.Length - eolAndWhiteElementCount); i++)
					line.Add(des[i]);

				// finally remove EOL data and white space elements from elements
				if (elements.Count >= eolAndWhiteElementCount)
					elements.RemoveRange(elements.Count - eolAndWhiteElementCount, eolAndWhiteElementCount);
			}

			// process line length
			List<DisplayElement> l = new List<DisplayElement>();
			if (TerminalSettings.Display.ShowLength)
			{
				int lineLength = 0;
				foreach (DisplayElement de in line)
				{
					if (de.IsDataElement)
						lineLength++;
				}
				l.Add(new DisplayElement.RightMargin());
				l.Add(new DisplayElement.LineLength(lineLength));
			}
			l.Add(new DisplayElement.LineBreak());

			// add line end to elements and return them
			elements.AddRange(l);

			// also add line end to line and return it
			line.AddRange(l);
			lines.Add(line);

			// reset line state
			lineState.Reset();
		}

		private void ExecuteData(SerialDirection direction, LineState lineState, byte b, List<DisplayElement> elements)
		{
			List<DisplayElement> l = new List<DisplayElement>();

			// add space if necessary
			if (ElementsAreSeparate())
			{
				int lineLength = 0;
				foreach (DisplayElement le in lineState.LineElements)
				{
					if (le.IsDataElement)
						lineLength++;
				}
				if (lineLength > 0)
				{
					l.Add(new DisplayElement.Space());
				}
			}

			// add data
			DisplayElement de = ByteToElement(b, direction);
			if (de.IsDataElement)
				l.Add(de);

			// return data
			lineState.LineElements.AddRange(l);
			elements.AddRange(l);

			// evaluate EOL
			lineState.Eol.Enqueue(b);
			if (lineState.Eol.EolMatch())
				lineState.LinePosition = LinePosition.End;
		}

		/// <summary></summary>
		protected override void ProcessRawElement(RawElement re, List<DisplayElement> elements, List<List<DisplayElement>> lines)
		{
			LineState lineState;
			if (re.Direction == SerialDirection.Tx)
				lineState = _txLineState;
			else
				lineState = _rxLineState;

			foreach (byte b in re.Data)
			{
				// line begin and time stamp
				if (lineState.LinePosition == LinePosition.Begin)
					ExecuteLineBegin(lineState, re.TimeStamp, elements);

				// data
				ExecuteData(re.Direction, lineState, b, elements);

				// line end and length
				if (lineState.LinePosition == LinePosition.End)
					ExecuteLineEnd(re.Direction, lineState, elements, lines);
			}
		}

		private void ProcessAndSignalDirectionLineBreak(SerialDirection direction)
		{
			LineState lineState;
			if (direction == SerialDirection.Tx)
				lineState = _rxLineState;
			else
				lineState = _txLineState;

			if (TextTerminalSettings.DirectionLineBreakEnabled)
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

						ExecuteLineEnd(_bidirLineState.Direction, lineState, elements, lines);

						OnDisplayElementsProcessed(_bidirLineState.Direction, elements);
						OnDisplayLinesProcessed(_bidirLineState.Direction, lines);
					}
				}
			}
			_bidirLineState.Direction = direction;
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
			
			return (indent + "- Type: TextTerminal" + Environment.NewLine + base.ToString(indent));
		}

		#endregion

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachTextTerminalSettings(Settings.TextTerminalSettings textTerminalSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(TerminalSettings.TextTerminal, textTerminalSettings))
				return;

			if (TerminalSettings.TextTerminal != null)
				DetachTextTerminalSettings();

			TerminalSettings.TextTerminal = textTerminalSettings;
			TerminalSettings.TextTerminal.Changed += new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(TextTerminalSettings_Changed);
		}

		private void DetachTextTerminalSettings()
		{
			TerminalSettings.TextTerminal.Changed -= new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(TextTerminalSettings_Changed);
		}

		private void ApplyTextTerminalSettings()
		{
			InitializeStates();
		}

		#endregion

		#region Settings Events
		//==========================================================================================
		// Settings Events
		//==========================================================================================

		private void TextTerminalSettings_Changed(object sender, MKY.Utilities.Settings.SettingsEventArgs e)
		{
			ApplyTextTerminalSettings();
		}

		#endregion
	}
}
