using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace HSR.YAT.Domain.Parser
{
	/// <summary>
	/// Parser.
	/// </summary>
	public class Parser
	{
		#region Help
		//------------------------------------------------------------------------------------------
		// Help
		//------------------------------------------------------------------------------------------

		public static readonly string FormatHelp =
			"The following formats are supported (type without quotation marks):" + Environment.NewLine +
			Environment.NewLine +
			"Default \"OK\"" + Environment.NewLine +
			"Binary \"\\b(01001111)\\b(01001011)\"" + Environment.NewLine +
			"Octal \"\\o(117)\\o(113)\"" + Environment.NewLine +
			"Decimal \"\\d(79)\\d(75)\"" + Environment.NewLine +
			"Hexadecimal \"\\h(4F)\\h(4B)\"" + Environment.NewLine +
			"Character \"\\c(O)\\c(K)\"" + Environment.NewLine +
			"String \"\\s(OK)\"" + Environment.NewLine +
			"ASCII controls (0x00 to 0x1F) \"<CR><LF>\"" + Environment.NewLine +
			Environment.NewLine +
			"Characters are case insensitive (e.g. \\H = \\h / 4f = 4F / <lf> = <LF>)" + Environment.NewLine +
			"Brakets can hold contiguous data (e.g. \\d(79 75)) separated by spaces, except for strings" + Environment.NewLine +
			"Control characters can be nested (e.g. \\d(79 \\h(4B) 79))" + Environment.NewLine +
			"Three letter radix identifiers are also allowed (e.g. \\hex instead of \\h" + Environment.NewLine +
			Environment.NewLine +
			"Type \\\\ to send a backspace" + Environment.NewLine +
			"Type \\< to send an opening angle bracket" + Environment.NewLine +
			"Type \\) to send a closing parenthesis";

		public static readonly string KeywordHelp =
			"In addition, the following keyword is supported:" + Environment.NewLine +
			Environment.NewLine +
			"Delay \"Send something\\!(" + (XKeyword)Keyword.Delay + "(10s))Send delayed by 10 seconds\"" + Environment.NewLine +
			"Delay \"Send something\\!(" + (XKeyword)Keyword.Delay + "(500ms))Send delayed by 500 milliseconds\"";

		#endregion

		#region States
		//------------------------------------------------------------------------------------------
		// States
		//------------------------------------------------------------------------------------------

		protected abstract class ParserState
		{
			public abstract bool TryParse(Parser parser, int parseChar, ref FormatException formatException);

			protected void ChangeState(Parser parser, ParserState state)
			{
				parser.State = state;
			}
		}

		/// <summary>
		/// Parses default.
		/// </summary>
		protected class DefaultState : ParserState
		{
			private StringWriter _contiguous;

			public DefaultState()
			{
				_contiguous = new StringWriter();
			}

			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				if ((parseChar < 0) ||                   // end of parse string
					(parseChar == ')' && !parser.IsTopLevel))
				{
					if (!TryWriteContiguous(parser, ref formatException))
						return (false);

					parser.HasFinished = true;
					ChangeState(parser, null);
				}
				else if (parseChar == '\\')              // escape sequence
				{
					if (!TryWriteContiguous(parser, ref formatException))
						return (false);

					parser.EndByteArray();
					parser.NestedParser = parser.GetParser(new EscapeState(), parser);
					ChangeState(parser, new NestedState());
				}
				else if (parseChar == '<')               // ascii mnemonic sequence
				{
					if (!TryWriteContiguous(parser, ref formatException))
						return (false);

					parser.EndByteArray();
					parser.NestedParser = parser.GetParser(new AsciiMnemonicState(), parser);
					ChangeState(parser, new NestedState());
				}
				else                                     // write contiguous string
				{
					_contiguous.Write((char)parseChar);
				}
				return (true);
			}

			private bool TryWriteContiguous(Parser parser, ref FormatException formatException)
			{
				if (_contiguous.ToString() != string.Empty)
				{
					if (!parser.IsKeywordParser)
					{
						byte[] a;

						if (!parser.TryParseContiguousRadix(_contiguous.ToString(), parser.Radix, out a, ref formatException))
							return (false);

						foreach (byte b in a)
							parser.ByteArrayWriter.WriteByte(b);

						parser.EndByteArray();
					}
					else
					{
						Result[] a;

						if (!parser.TryParseContiguousKeywords(_contiguous.ToString(), out a, ref formatException))
							return (false);

						parser.ResultList.AddRange(a);
					}
				}
				return (true);
			}
		}

		/// <summary>
		/// Parses escape control.
		/// </summary>
		protected class EscapeState : ParserState
		{
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				switch (parseChar)
				{
					case 'b':
					case 'B':
					{
						parser.SetDefaultRadix(Radix.Bin);
						ChangeState(parser, new OpeningState());
						return (true);
					}
					case 'o':
					case 'O':
					{
						parser.SetDefaultRadix(Radix.Oct);
						ChangeState(parser, new OpeningState());
						return (true);
					}
					case 'd':
					case 'D':
					{
						parser.SetDefaultRadix(Radix.Dec);
						ChangeState(parser, new OpeningState());
						return (true);
					}
					case 'h':
					case 'H':
					{
						parser.SetDefaultRadix(Radix.Hex);
						ChangeState(parser, new OpeningState());
						return (true);
					}
					case 'c':
					case 'C':
					{
						parser.SetDefaultRadix(Radix.Char);
						ChangeState(parser, new OpeningState());
						return (true);
					}
					case 's':
					case 'S':
					{
						parser.SetDefaultRadix(Radix.String);
						ChangeState(parser, new OpeningState());
						return (true);
					}
					case '!':
					{
						if ((parser._parseMode & ParseMode.Keywords) == ParseMode.Keywords)
						{
							parser.IsKeywordParser = true;
							ChangeState(parser, new OpeningState());
							return (true);
						}
						else
						{
							formatException = new FormatException
								(
								"Keywords are not allowd in this sequence"
								);
							return (false);
						}
					}
					case '\\':                              // "\\" results in "\"
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '\\' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}
					case '<':                              // "\<" results in "<"
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '<' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}
					case '>':                              // "\>" results in ">"
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '>' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}
					case '(':                              // "\(" results in "("
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '(' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}
					case ')':                              // "\)" results in ")"
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { ')' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}
					default:
					{
						formatException = new FormatException
							(
							"Character '" + (char)parseChar + "'" +
							"[\\d(" + parseChar + ")] is " +
							"not a valid character"
							);
						return (false);
					}
				}
			}
		}

		protected class OpeningState : ParserState
		{
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				if (parseChar == '(')
				{
					ChangeState(parser, new DefaultState());
					return (true);
				}
				else
				{
					formatException = new FormatException("Missing opening parenthesis \"(\"");
					return (false);
				}
			}
		}

		protected class AsciiMnemonicState : ParserState
		{
			private StringWriter _mnemonic;

			public AsciiMnemonicState()
			{
				_mnemonic = new StringWriter();
			}

			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				if ((parseChar < 0) || (parseChar == '>'))
				{
					byte[] a;

					if (!TryParseAsciiMnemonic(parser, _mnemonic.ToString(), out a, ref formatException))
						return (false);

					foreach (byte b in a)
						parser.ByteArrayWriter.WriteByte(b);

					parser.EndByteArray();

					parser.HasFinished = true;
					ChangeState(parser, null);
				}
				else                                     // write contiguous string
				{
					_mnemonic.Write((char)parseChar);
				}
				return (true);
			}

			/// <summary>
			/// Parses "parseString" for ascii mnemonics.
			/// </summary>
			/// <param name="parseString">String to be parsed</param>
			/// <returns>Bytearray containing the values encoded in Encoding.Default.</returns>
			/// <exception cref="FormatException">Thrown if invalid string format.</exception>
			public static bool TryParseAsciiMnemonic(Parser parser, string parseString, out byte[] result, ref FormatException formatException)
			{
				MemoryStream bytes = new MemoryStream();
				string[] tokens = parseString.Split(' ');
				foreach (string t in tokens)
				{
					if (t == string.Empty) continue;

					byte code;
					if (Utilities.Types.Ascii.TryParse(t, out code))
					{
						char c = Convert.ToChar(code);
						byte[] b = parser.Encoding.GetBytes(new char[] { c });
						bytes.Write(b, 0, b.Length);
					}
					else
					{
						result = new byte[] { };
						formatException = new FormatException("\"" + t + "\" is no ascii mnemonic");
						return (false);
					}
				}
				result = bytes.ToArray();
				return (true);
			}
		}

		protected class NestedState : ParserState
		{
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				if (!parser.NestedParser.State.TryParse(parser.NestedParser, parseChar, ref formatException))
					return (false);

				if (parser.NestedParser.HasFinished)     // regain parser "focus"
					ChangeState(parser, new DefaultState());

				return (true);
			}
		}

		#endregion

		#region Attributes
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private Encoding _encoding = Encoding.Default;
		private Radix _defaultRadix = Radix.String;
		private ParseMode _parseMode = ParseMode.All;

		private StringReader _reader = null;
		private MemoryStream _byteArrayWriter = null;
		private List<Result> _resultList = null;
		private ParserState _state = null;

		private Parser _parentParser = null;
		private Parser _nestedChildParser = null;
		private bool _isKeywordParser = false;

		private bool _hasFinished = false;

		#endregion

		#region Object Lifetime
		//------------------------------------------------------------------------------------------
		// Object Lifetime
		//------------------------------------------------------------------------------------------

		public Parser()
		{
		}

		public Parser(Encoding encoding)
		{
			_encoding = encoding;
		}

		public Parser(Radix defaultRadix)
		{
			_defaultRadix = defaultRadix;
		}

		public Parser(Encoding encoding, Radix defaultRadix)
		{
			_encoding = encoding;
			_defaultRadix = defaultRadix;
		}

		protected Parser(ParserState parserState, Parser parent)
		{
			InitializeNestedParse(parserState, parent);
		}

		#endregion

		#region Factory
		//------------------------------------------------------------------------------------------
		// Factory
		//------------------------------------------------------------------------------------------

		protected virtual Parser GetParser(ParserState parserState, Parser parent)
		{
			Parser child = new Parser(parserState, parent);
			return (child);
		}

		#endregion

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		private StringReader Reader
		{
			get { return (_reader); }
		}

		private MemoryStream ByteArrayWriter
		{
			get { return (_byteArrayWriter); }
		}

		private List<Result> ResultList
		{
			get { return (_resultList); }
		}

		private ParserState State
		{
			get { return (_state); }
			set { _state = value; }
		}

		public Encoding Encoding
		{
			get { return (_encoding); }
		}

		// Radix: public get, private set
		public Radix Radix
		{
			get { return (_defaultRadix); }
		}

		private void SetDefaultRadix(Radix defaultRadix)
		{
			_defaultRadix = defaultRadix;
		}

		public bool IsTopLevel
		{
			get { return (_parentParser == null); }
		}

		private Parser Parent
		{
			get { return (_parentParser); }
		}

		private Parser NestedParser
		{
			get { return (_nestedChildParser); }
			set { _nestedChildParser = value; }
		}

		private bool IsKeywordParser
		{
			get { return (_isKeywordParser); }
			set { _isKeywordParser = value; }
		}

		private bool HasFinished
		{
			get { return (_hasFinished); }
			set { _hasFinished = value; }
		}

		#endregion

		#region Public Methods
		//------------------------------------------------------------------------------------------
		// Public Methods
		//------------------------------------------------------------------------------------------

		public byte[] Parse(string s)
		{
			string parsed;
			return (Parse(s, out parsed));
		}

		public byte[] Parse(string s, out string parsed)
		{
			Result[] resultResult = Parse(s, ParseMode.AllByteArrayResults, out parsed);
			MemoryStream byteResult = new MemoryStream();
			foreach (Result r in resultResult)
			{
				if (r is ByteArrayResult)
				{
					byte[] a = ((ByteArrayResult)r).ByteArray;
					byteResult.Write(a, 0, a.Length);
				}
			}
			return (byteResult.ToArray());
		}

		public Result[] Parse(string s, ParseMode mode)
		{
			string parsed;
			return (Parse(s, mode, out parsed));
		}

		public Result[] Parse(string s, ParseMode mode, out string parsed)
		{
			Result[] result;
			FormatException formatException = new FormatException("");
			if (!TryParse(s, mode, out result, out parsed, ref formatException))
				throw (formatException);
			return (result);
		}

		public bool TryParse(string s, out byte[] result)
		{
			string parsed;
			return (TryParse(s, out result, out parsed));
		}

		public bool TryParse(string s, out byte[] result, out string parsed)
		{
			Result[] resultResult;
			bool tryResult = TryParse(s, ParseMode.AllByteArrayResults, out resultResult, out parsed);

			MemoryStream byteResult = new MemoryStream();
			foreach (Result r in resultResult)
			{
				if (r is ByteArrayResult)
				{
					byte[] a = ((ByteArrayResult)r).ByteArray;
					byteResult.Write(a, 0, a.Length);
				}
			}
			result = byteResult.ToArray();

			return (tryResult);
		}

		public bool TryParse(string s)
		{
			return (TryParse(s, ParseMode.All));
		}

		public bool TryParse(string s, ParseMode mode)
		{
			string parsed;
			return (TryParse(s, mode, out parsed));
		}

		public bool TryParse(string s, ParseMode mode, out Result[] result)
		{
			string parsed;
			return (TryParse(s, mode, out result, out parsed));
		}

		public bool TryParse(string s, ParseMode mode, out string parsed)
		{
			Result[] result;
			return (TryParse(s, mode, out result, out parsed));
		}

		public bool TryParse(string s, ParseMode mode, out string parsed, ref FormatException formatException)
		{
			Result[] result;
			return (TryParse(s, mode, out result, out parsed, ref formatException));
		}

		public bool TryParse(string s, ParseMode mode, out Result[] result, out string parsed)
		{
			FormatException formatException = new FormatException("");
			return (TryParse(s, mode, out result, out parsed, ref formatException));
		}

		public bool TryParse(string s, ParseMode mode, out Result[] result, out string parsed, ref FormatException formatException)
		{
			InitializeTopLevelParse(s, mode);

			while (!HasFinished)
			{
				if (!_state.TryParse(this, _reader.Read(), ref formatException))
				{
					EndByteArray();

					// return part of string that could be parsed
					parsed = Utilities.Types.XString.Left(s, s.Length - _reader.ReadToEnd().Length - 1);
					result = _resultList.ToArray();
					return (false);
				}
			}

			EndByteArray();

			parsed = s;
			result = _resultList.ToArray();
			return (true);
		}

		#endregion

		#region Protected Methods
		//------------------------------------------------------------------------------------------
		// Protected Methods
		//------------------------------------------------------------------------------------------

		protected void EndByteArray()
		{
			if (_byteArrayWriter.Length > 0)
			{
				_resultList.Add(new ByteArrayResult(_byteArrayWriter.ToArray()));
				_byteArrayWriter = new MemoryStream();
			}
		}

		protected virtual bool TryParseContiguousRadixToken(string token, Radix parseRadix, out byte[] result, ref FormatException formatException)
		{
			// String
			if (parseRadix == Radix.String)
			{
				result = _encoding.GetBytes(token);
				return (true);
			}

			// Char
			if (parseRadix == Radix.Char)
			{
				char c;
				if (char.TryParse(token, out c))
				{
					result = _encoding.GetBytes(new char[] { c });
					return (true);
				}
				else
				{
					formatException = new FormatException("Substring \"" + token + "\" does not contain a valid single character");
					result = new byte[] { };
					return (false);
				}
			}

			// Bin/Oct/Dec/Hex
			int utf32;
			bool success;
			switch (parseRadix)
			{
				case Radix.Bin: success = Utilities.Types.XInt.TryParseBinary(token, out utf32); break;
				case Radix.Oct: success = Utilities.Types.XInt.TryParseOctal(token, out utf32); break;
				case Radix.Dec: success = int.TryParse(token, out utf32); break;
				case Radix.Hex: success = int.TryParse(token, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out utf32); break;
				default: throw (new NotImplementedException("Unknown radix \"" + parseRadix + "\""));
			}
			if (success)
			{
				string utf16 = char.ConvertFromUtf32(utf32);
				result = _encoding.GetBytes(utf16);
				return (true);
			}

			// FormatException
			string readable = "";
			switch (parseRadix)
			{
				case Radix.Bin: readable = "binary value"; break;
				case Radix.Oct: readable = "octal value"; break;
				case Radix.Dec: readable = "decimal value"; break;
				case Radix.Hex: readable = "hexadecimal value"; break;
				default: throw (new NotImplementedException("Unknown radix \"" + parseRadix + "\""));
			}
			formatException = new FormatException("Substring \"" + token + "\" contains no valid UTF-32 " + readable);
			result = new byte[] { };
			return (false);
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void InitializeTopLevelParse(string parseString, ParseMode parseMode)
		{
			_parseMode = parseMode;

			_reader = new StringReader(parseString);
			_byteArrayWriter = new MemoryStream();
			_resultList = new List<Result>();
			_state = new DefaultState();

			_isKeywordParser = false;

			_hasFinished = false;
		}

		private void InitializeNestedParse(ParserState parserState, Parser parent)
		{
			_encoding = parent._encoding;
			_defaultRadix = parent._defaultRadix;
			_parseMode = parent._parseMode;

			_reader = parent._reader;
			_byteArrayWriter = new MemoryStream();
			_resultList = parent._resultList;
			_state = parserState;

			_parentParser = parent;
			_isKeywordParser = false;

			_hasFinished = false;
		}

		/// <summary>
		/// Parses "parseString" for one or more values in the specified base
		/// "parseRadix", separated with spaces.
		/// </summary>
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="parseRadix">Numeric radix.</param>
		/// <returns>Bytearray containing the values encoded in Encoding.Default.</returns>
		/// <exception cref="FormatException">Thrown if invalid string format.</exception>
		/// <exception cref="OverflowException">Thrown if a value cannot be converted into bytes.</exception>
		private bool TryParseContiguousRadix(string parseString, Radix parseRadix, out byte[] result, ref FormatException formatException)
		{
			MemoryStream bytes = new MemoryStream();
			if (parseRadix == Radix.String)
			{
				byte[] b;
				if (TryParseContiguousRadixToken(parseString, Radix.String, out b, ref formatException))
				{
					bytes.Write(b, 0, b.Length);
				}
				else
				{
					result = new byte[] { };
					return (false);
				}
			}
			else
			{
				string[] tokens = parseString.Split(' ');
				foreach (string token in tokens)
				{
					if (token != string.Empty)
					{
						byte[] b;
						if (TryParseContiguousRadixToken(token, parseRadix, out b, ref formatException))
						{
							bytes.Write(b, 0, b.Length);
						}
						else
						{
							result = new byte[] { };
							return (false);
						}
					}
				}
			}
			result = bytes.ToArray();
			return (true);
		}

		/// <summary>
		/// Parses "parseString" for keywords.
		/// </summary>
		/// <param name="parseString">String to be parsed</param>
		/// <returns>Bytearray containing the values encoded in Encoding.Default.</returns>
		/// <exception cref="FormatException">Thrown if invalid string format.</exception>
		private bool TryParseContiguousKeywords(string parseString, out Result[] result, ref FormatException formatException)
		{
			List<Result> resultList = new List<Result>();
			string[] tokens = parseString.Split(' ');
			foreach (string t in tokens)
			{
				if (t == string.Empty) continue;

				try
				{
					resultList.Add(new KeywordResult((XKeyword)t));
				}
				catch
				{
					result = new Result[] { };
					formatException = new FormatException("\"" + t + "\" is no keyword");
					return (false);
				}
			}
			result = resultList.ToArray();
			return (true);
		}

		#endregion
	}
}
