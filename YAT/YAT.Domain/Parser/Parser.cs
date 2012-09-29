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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using System.Globalization;
using System.IO;
using System.Text;

using MKY;
using MKY.IO;
using MKY.Text;

#endregion

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Parser : IDisposable
	{
		#region Help
		//==========================================================================================
		// Help
		//==========================================================================================

		/// <summary></summary>
		public static readonly string FormatHelp =
			@"The following formats are supported (type without quotes):" + Environment.NewLine +
			Environment.NewLine +
			@"Default ""OK""" + Environment.NewLine +
			@"Binary ""\b(01001111 01001011)""" + Environment.NewLine +
			@"Octal ""\o(117 113)""" + Environment.NewLine +
			@"Decimal ""\d(79 75)""" + Environment.NewLine +
			@"Hexadecimal ""\h(4F 4B)""" + Environment.NewLine +
			@"Character ""\c(O K)""" + Environment.NewLine +
			@"String ""\s(OK)""" + Environment.NewLine +
			@"ASCII controls (0x00 to 0x1F) ""<CR><LF>""" + Environment.NewLine +
			Environment.NewLine +
			@"Format specifiers are case insensitive, e.g. ""\H"" = ""\h"", ""4f"" = ""4F"", ""<lf>"" = ""<LF>""" + Environment.NewLine +
			@"Formats can also be applied on each value, e.g. ""\d(79)\d(75)""" + Environment.NewLine +
			@"Formats can be nested, e.g. ""\d(79 \h(4B) 79)""" + Environment.NewLine +
			@"Three letter radix identifiers are also allowed, e.g. ""\hex"" instead of ""\h""" + Environment.NewLine +
			Environment.NewLine +
			@"In addition, C-style escape sequences are supported:" + Environment.NewLine +
			@"""\r\n"" instead of ""<CR><LF>""" + Environment.NewLine +
			@"""\0"" instead of ""<NUL>"" or \d(0) or \h(0)" + Environment.NewLine +
			@"""\01"" instead of \o(1)" + Environment.NewLine +
			@"""\12"" instead of \d(12)" + Environment.NewLine +
			@"""\0x1A"" or ""\x1A"" instead of \h(1A)" + Environment.NewLine +
			Environment.NewLine +
			@"Type \\ to send a backspace" + Environment.NewLine +
			@"Type \< to send an opening angle bracket" + Environment.NewLine +
			@"Type \) to send a closing parenthesis";

		/// <summary></summary>
		public static readonly string KeywordHelp =
			@"In addition, the following keywords are supported:" + Environment.NewLine +
			Environment.NewLine +
			@"Clear the monitors ""Send something\!(" + (KeywordEx)Keyword.Clear + @")""" + Environment.NewLine +
			Environment.NewLine +
			@"Delay sending ""Send something\!(" + (KeywordEx)Keyword.Delay + @")Send delayed"" according to advanced settings" + Environment.NewLine +
			Environment.NewLine +
			@"Output break state on ""\!(" + (KeywordEx)Keyword.OutputBreakOn + @")""" + Environment.NewLine +
			@"Output break state off ""\!(" + (KeywordEx)Keyword.OutputBreakOff + @")""" + Environment.NewLine +
			@"Output break state toggle ""\!(" + (KeywordEx)Keyword.OutputBreakToggle + @")""" + Environment.NewLine +
			@"Output break state only applies to serial COM ports";

		#endregion

		#region States
		//==========================================================================================
		// States
		//==========================================================================================

		/// <summary></summary>
		protected abstract class ParserState : IDisposable
		{
			private bool isDisposed;

			/// <summary></summary>
			public abstract bool TryParse(Parser parser, int parseChar, ref FormatException formatException);

			/// <summary></summary>
			protected void ChangeState(Parser parser, ParserState state)
			{
				parser.State = state;
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
						// Nothing to do in base class.
					}
					this.isDisposed = true;
				}
			}

			/// <summary></summary>
			~ParserState()
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
		}

		/// <summary>
		/// Parses default.
		/// </summary>
		protected class DefaultState : ParserState
		{
			private StringWriter contiguousWriter;

			/// <summary></summary>
			public DefaultState()
			{
				this.contiguousWriter = new StringWriter(CultureInfo.InvariantCulture);
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.contiguousWriter != null)
						this.contiguousWriter.Dispose();
				}
				base.Dispose(disposing);
			}

			#endregion

			/// <summary></summary>
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				AssertNotDisposed();

				if ((parseChar < 0) ||                   // End of parse string.
					(parseChar == ')' && !parser.IsTopLevel))
				{
					if (!TryWriteContiguous(parser, ref formatException))
						return (false);

					parser.HasFinished = true;
					ChangeState(parser, null);
				}
				else if (parseChar == '\\')              // Escape sequence.
				{
					if (!TryWriteContiguous(parser, ref formatException))
						return (false);

					parser.EndByteArray();
					parser.NestedParser = parser.GetParser(new EscapeState(), parser);
					ChangeState(parser, new NestedState());
				}
				else if (parseChar == '<')               // ASCII mnemonic sequence.
				{
					if (!TryWriteContiguous(parser, ref formatException))
						return (false);

					parser.EndByteArray();
					parser.NestedParser = parser.GetParser(new AsciiMnemonicState(), parser);
					ChangeState(parser, new NestedState());
				}
				else                                     // Compose contiguous string.
				{
					this.contiguousWriter.Write((char)parseChar);
				}
				return (true);
			}

			private bool TryWriteContiguous(Parser parser, ref FormatException formatException)
			{
				string contiguousString = this.contiguousWriter.ToString();
				if (contiguousString.Length > 0)
				{
					if (!parser.IsKeywordParser)
					{
						byte[] a;

						if (!parser.TryParseContiguousRadix(contiguousString, parser.Radix, out a, ref formatException))
							return (false);

						foreach (byte b in a)
							parser.ByteArrayWriter.WriteByte(b);

						parser.EndByteArray();
					}
					else
					{
						Result[] a;

						if (!parser.TryParseContiguousKeywords(contiguousString, out a, ref formatException))
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
			/// <summary></summary>
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				switch (parseChar)
				{
					case 'b':
					case 'B':
					{
						int nextChar = parser.Reader.Peek();
						if (nextChar == '(')
						{
							// \b(...) is used for binary values, e.g. \b(010110001).
							parser.SetDefaultRadix(Radix.Bin);
							ChangeState(parser, new OpeningState());
							return (true);
						}
						else
						{
							// Just \b is used for c-style backspace.
							parser.ByteArrayWriter.WriteByte((byte)'\b');
							parser.EndByteArray();
							parser.HasFinished = true;
							ChangeState(parser, null);
							return (true);
						}
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
						if ((parser.parseMode & ParseMode.Keywords) == ParseMode.Keywords)
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

					case '0': // C-style <NUL>.
					{
						int nextChar = parser.Reader.Peek();
						switch (nextChar)
						{
							case '0': // \0<value> is used for c-style octal notation.
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							{
								parser.SetDefaultRadix(Radix.Oct);
								ChangeState(parser, new NumericState());
								return (true);
							}

							case 'x': // \0x is used for c-style hexadecimal notation.
							case 'X':
							{
								parser.Reader.Read(); // Consume 'x' or 'X'.
								parser.SetDefaultRadix(Radix.Hex);
								ChangeState(parser, new NumericState());
								return (true);
							}

							default:
							{
								// Just \0 is used for c-style <NUL>.
								parser.ByteArrayWriter.WriteByte((byte)'\0');
								parser.EndByteArray();
								parser.HasFinished = true;
								ChangeState(parser, null);
								return (true);
							}
						}
					}

					case 'a': // C-style bell.
					case 'A':
					{
						parser.ByteArrayWriter.WriteByte((byte)'\a');
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					// For case 'b' or 'B' (C-style backspace) see above.

					case 't': // C-style tab.
					case 'T':
					{
						parser.ByteArrayWriter.WriteByte((byte)'\t');
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case 'v': // C-style vertical tab.
					case 'V':
					{
						parser.ByteArrayWriter.WriteByte((byte)'\v');
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case 'n': // C-style <LF>.
					case 'N':
					{
						parser.ByteArrayWriter.WriteByte((byte)'\n');
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case 'r': // C-style <CR>.
					case 'R':
					{
						parser.ByteArrayWriter.WriteByte((byte)'\r');
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case 'f': // C-style <FF>.
					case 'F':
					{
						parser.ByteArrayWriter.WriteByte((byte)'\f');
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case 'x': // C-style hexadecimal value, e.g. \x1A.
					case 'X':
					{
						parser.SetDefaultRadix(Radix.Hex);
						ChangeState(parser, new NumericState());
						return (true);
					}

					case '1': // C-style decimal value, e.g. \12.
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					{
						parser.SetDefaultRadix(Radix.Dec);
						ChangeState(parser, new NumericState(parseChar));
						return (true);
					}

					case '\\':                              // "\\" results in "\".
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '\\' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case '<':                              // "\<" results in "<".
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '<' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case '>':                              // "\>" results in ">".
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '>' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case '(':                              // "\(" results in "(".
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { '(' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case ')':                              // "\)" results in ")".
					{
						byte[] b = parser.Encoding.GetBytes(new char[] { ')' });
						parser.ByteArrayWriter.Write(b, 0, b.Length);
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					case StreamEx.EndOfStream:
					{
						parser.EndByteArray();
						parser.HasFinished = true;
						ChangeState(parser, null);
						return (true);
					}

					default:
					{
						formatException = new FormatException
							(
							@"Character '" + (char)parseChar + "'" +
							@"[\d(" + parseChar + ")] is " +
							@"not a valid character"
							);
						return (false);
					}
				}
			}
		}

		/// <summary></summary>
		protected class OpeningState : ParserState
		{
			/// <summary></summary>
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				if (parseChar == '(')
				{
					ChangeState(parser, new DefaultState());
					return (true);
				}
				else
				{
					formatException = new FormatException(@"Missing opening parenthesis ""(""!");
					return (false);
				}
			}
		}

		/// <summary></summary>
		protected class AsciiMnemonicState : ParserState
		{
			private StringWriter mnemonicWriter;

			/// <summary></summary>
			public AsciiMnemonicState()
			{
				this.mnemonicWriter = new StringWriter(CultureInfo.InvariantCulture);
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.mnemonicWriter != null)
						this.mnemonicWriter.Dispose();
				}
				base.Dispose(disposing);
			}

			#endregion

			/// <summary></summary>
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				AssertNotDisposed();

				if ((parseChar < 0) || (parseChar == '>')) // Process finished mnemonic string.
				{
					byte[] a;

					if (!TryParseAsciiMnemonic(parser, this.mnemonicWriter.ToString(), out a, ref formatException))
						return (false);

					foreach (byte b in a)
						parser.ByteArrayWriter.WriteByte(b);

					parser.EndByteArray();

					parser.HasFinished = true;
					ChangeState(parser, null);
				}
				else                                       // Compose contiguous string.
				{
					this.mnemonicWriter.Write((char)parseChar);
				}
				return (true);
			}

			/// <summary>
			/// Parses "parseString" for ascii mnemonics.
			/// </summary>
			/// <param name="parser">Parser to retrieve settings.</param>
			/// <param name="parseString">String to be parsed.</param>
			/// <param name="result">Array containing the resulting bytes.</param>
			/// <param name="formatException">Returned if invalid string format.</param>
			/// <returns>Bytearray containing the values encoded in Encoding.Default.</returns>
			public static bool TryParseAsciiMnemonic(Parser parser, string parseString, out byte[] result, ref FormatException formatException)
			{
				MemoryStream bytes = new MemoryStream();
				string[] tokens = parseString.Split(' ');
				foreach (string t in tokens)
				{
					if (t.Length == 0)
						continue;

					byte code;
					if (Ascii.TryParse(t, out code))
					{
						char c = Convert.ToChar(code);
						byte[] b = parser.Encoding.GetBytes(new char[] { c });
						bytes.Write(b, 0, b.Length);
					}
					else
					{
						result = new byte[] { };
						formatException = new FormatException(@"""" + t + @""" is no ascii mnemonic!");
						return (false);
					}
				}
				result = bytes.ToArray();
				return (true);
			}
		}

		/// <summary></summary>
		protected class NumericState : ParserState
		{
			private StringWriter numericWriter;

			/// <summary></summary>
			public NumericState()
			{
				this.numericWriter = new StringWriter(CultureInfo.InvariantCulture);
			}

			/// <summary></summary>
			public NumericState(int parseChar)
				: this()
			{
				this.numericWriter.Write((char)parseChar);
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.numericWriter != null)
						this.numericWriter.Dispose();
				}
				base.Dispose(disposing);
			}

			#endregion

			/// <summary></summary>
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				AssertNotDisposed();

				switch (parser.Radix)
				{
					case Radix.Oct:
					{
						if ((parseChar >= '0') && (parseChar <= '7'))
						{
							this.numericWriter.Write((char)parseChar);
							return (true);
						}
						break;
					}

					case Radix.Dec:
					{
						if ((parseChar >= '0') && (parseChar <= '9'))
						{
							this.numericWriter.Write((char)parseChar);
							return (true);
						}
						break;
					}

					case Radix.Hex:
					{
						if (((parseChar >= '0') && (parseChar <= '9')) ||
							((parseChar >= 'A') && (parseChar <= 'F')) ||
							((parseChar >= 'a') && (parseChar <= 'f')))
						{
							this.numericWriter.Write((char)parseChar);
							return (true);
						}
						break;
					}

					// No other numeric formats are handled by this parser so far.
					// Only C-style \0<oct>, \<dec>, \0x<hex> and \x<hex>.
					default:
					{
						// Also handled below.
						break;
					}
				}

				// No more valid character found, try to process numeric value.
				byte[] a;

				if (!TryParseNumericValue(parser, this.numericWriter.ToString(), out a, ref formatException))
					return (false);

				foreach (byte b in a)
					parser.ByteArrayWriter.WriteByte(b);

				parser.EndByteArray();

				parser.HasFinished = true;
				ChangeState(parser, null);
				return (true);
			}

			/// <summary>
			/// Parses "parseString" for ascii mnemonics.
			/// </summary>
			/// <param name="parser">Parser to retrieve settings.</param>
			/// <param name="parseString">String to be parsed.</param>
			/// <param name="result">Array containing the resulting bytes.</param>
			/// <param name="formatException">Returned if invalid string format.</param>
			/// <returns>Bytearray containing the values encoded in Encoding.Default.</returns>
			public static bool TryParseNumericValue(Parser parser, string parseString, out byte[] result, ref FormatException formatException)
			{
				switch (parser.Radix)
				{
					case Radix.Oct:
					{
						UInt64 value;
						if (UInt64Ex.TryParseOctal(parseString, out value))
						{
							result = UInt64Ex.ConvertToByteArray(value);
							return (true);
						}
						else
						{
							formatException = new FormatException(@"""" + parseString + @""" is no valid octal value!");
						}
						break;
					}

					case Radix.Dec:
					{
						UInt64 value;
						if (UInt64.TryParse(parseString, out value))
						{
							result = UInt64Ex.ConvertToByteArray(value);
							return (true);
						}
						else
						{
							formatException = new FormatException(@"""" + parseString + @""" is no valid decimal value!");
						}
						break;
					}

					case Radix.Hex:
					{
						MemoryStream bytes = new MemoryStream();
						string errorString = null;
						foreach (string s in StringEx.Split(parseString, 2))
						{
							byte b;
							if (byte.TryParse(s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out b))
								bytes.WriteByte(b);
							else
								errorString = s;
						}
						if (string.IsNullOrEmpty(errorString))
						{
							result = bytes.ToArray();
							return (true);
						}
						else
						{
							formatException = new FormatException(@"Substring """ + errorString + @""" of """ + parseString + @""" is no valid hexadecimal value!");
						}
						break;
					}
				}

				result = new byte[] { };
				return (false);
			}
		}

		/// <summary></summary>
		protected class NestedState : ParserState
		{
			/// <summary></summary>
			public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
			{
				if (!parser.NestedParser.State.TryParse(parser.NestedParser, parseChar, ref formatException))
					return (false);

				if (parser.NestedParser.HasFinished) // Regain parser "focus".
					ChangeState(parser, new DefaultState());

				return (true);
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Endianess endianess = Endianess.BigEndian;
		private Encoding encoding = Encoding.Default;
		private Radix defaultRadix = Radix.String;
		private ParseMode parseMode = ParseMode.All;

		private StringReader reader;
		private MemoryStream byteArrayWriter;
		private List<Result> resultList;
		private ParserState state;

		private Parser parentParser;
		private Parser nestedChildParser;
		private bool isKeywordParser;

		private bool hasFinished;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Parser()
		{
		}

		/// <summary></summary>
		public Parser(Endianess endianess)
		{
			this.endianess = endianess;
		}

		/// <summary></summary>
		public Parser(Encoding encoding)
		{
			this.encoding = encoding;
		}

		/// <summary></summary>
		public Parser(Endianess endianess, Encoding encoding)
		{
			this.endianess = endianess;
			this.encoding = encoding;
		}

		/// <summary></summary>
		public Parser(Radix defaultRadix)
		{
			this.defaultRadix = defaultRadix;
		}

		/// <summary></summary>
		public Parser(Encoding encoding, Radix defaultRadix)
		{
			this.encoding = encoding;
			this.defaultRadix = defaultRadix;
		}

		/// <summary></summary>
		public Parser(Endianess endianess, Encoding encoding, Radix defaultRadix)
		{
			this.endianess = endianess;
			this.encoding = encoding;
			this.defaultRadix = defaultRadix;
		}

		/// <summary></summary>
		protected Parser(ParserState parserState, Parser parent)
		{
			InitializeNestedParse(parserState, parent);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

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
					if (this.reader != null)
						this.reader.Dispose();

					if (this.byteArrayWriter != null)
						this.byteArrayWriter.Dispose();

					if (this.state != null)
						this.state.Dispose();

					if (this.nestedChildParser != null)
						this.nestedChildParser.Dispose();
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Parser()
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

		#endregion

		#region Factory
		//==========================================================================================
		// Factory
		//==========================================================================================

		/// <summary></summary>
		protected virtual Parser GetParser(ParserState parserState, Parser parent)
		{
			AssertNotDisposed();

			Parser child = new Parser(parserState, parent);
			return (child);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private StringReader Reader
		{
			get { return (this.reader); }
		}

		private MemoryStream ByteArrayWriter
		{
			get { return (this.byteArrayWriter); }
		}

		private List<Result> ResultList
		{
			get { return (this.resultList); }
		}

		private ParserState State
		{
			get { return (this.state); }
			set { this.state = value; }
		}

		/// <summary></summary>
		public virtual Endianess Endianess
		{
			get { return (this.endianess); }
		}

		/// <summary></summary>
		public virtual Encoding Encoding
		{
			get { return (this.encoding); }
		}

		/// <summary></summary>
		/// <remarks>Radix: public get, private set</remarks>
		public virtual Radix Radix
		{
			get { return (this.defaultRadix); }
		}

		private void SetDefaultRadix(Radix defaultRadix)
		{
			this.defaultRadix = defaultRadix;
		}

		/// <summary></summary>
		public virtual bool IsTopLevel
		{
			get { return (this.parentParser == null); }
		}

		private Parser Parent
		{
			get { return (this.parentParser); }
		}

		private Parser NestedParser
		{
			get { return (this.nestedChildParser); }
			set { this.nestedChildParser = value; }
		}

		private bool IsKeywordParser
		{
			get { return (this.isKeywordParser); }
			set { this.isKeywordParser = value; }
		}

		private bool HasFinished
		{
			get { return (this.hasFinished); }
			set { this.hasFinished = value; }
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual byte[] Parse(string s)
		{
			// AssertNotDisposed() is called below.

			string parsed;
			return (Parse(s, out parsed));
		}

		/// <summary></summary>
		public virtual byte[] Parse(string s, out string parsed)
		{
			// AssertNotDisposed() is called below.

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

		/// <summary></summary>
		public virtual Result[] Parse(string s, ParseMode mode)
		{
			// AssertNotDisposed() is called below.

			string parsed;
			return (Parse(s, mode, out parsed));
		}

		/// <summary></summary>
		public virtual Result[] Parse(string s, ParseMode mode, out string parsed)
		{
			// AssertNotDisposed() is called below.

			Result[] result;
			FormatException formatException = new FormatException("");
			if (!TryParse(s, mode, out result, out parsed, ref formatException))
				throw (formatException);
			return (result);
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, out byte[] result)
		{
			// AssertNotDisposed() is called below.

			string parsed;
			return (TryParse(s, out result, out parsed));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, out byte[] result, out string parsed)
		{
			// AssertNotDisposed() is called below.

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

		/// <summary></summary>
		public virtual bool TryParse(string s)
		{
			// AssertNotDisposed() is called below.

			return (TryParse(s, ParseMode.All));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, ParseMode mode)
		{
			// AssertNotDisposed() is called below.

			string parsed;
			return (TryParse(s, mode, out parsed));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, ParseMode mode, out Result[] result)
		{
			// AssertNotDisposed() is called below.

			string parsed;
			return (TryParse(s, mode, out result, out parsed));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, ParseMode mode, out string parsed)
		{
			// AssertNotDisposed() is called below.

			Result[] result;
			return (TryParse(s, mode, out result, out parsed));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, ParseMode mode, out string parsed, ref FormatException formatException)
		{
			// AssertNotDisposed() is called below.

			Result[] result;
			return (TryParse(s, mode, out result, out parsed, ref formatException));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, ParseMode mode, out Result[] result, out string parsed)
		{
			// AssertNotDisposed() is called below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, mode, out result, out parsed, ref formatException));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, ParseMode mode, out Result[] result, out string parsed, ref FormatException formatException)
		{
			AssertNotDisposed();

			InitializeTopLevelParse(s, mode);

			while (!HasFinished)
			{
				if (!this.state.TryParse(this, this.reader.Read(), ref formatException))
				{
					EndByteArray();

					// Return part of string that could be parsed
					parsed = StringEx.Left(s, s.Length - this.reader.ReadToEnd().Length - 1);
					result = this.resultList.ToArray();
					return (false);
				}
			}

			EndByteArray();

			parsed = s;
			result = this.resultList.ToArray();
			return (true);
		}

		#endregion

		#region Protected Methods
		//==========================================================================================
		// Protected Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual void EndByteArray()
		{
			AssertNotDisposed();

			if (this.byteArrayWriter.Length > 0)
			{
				this.resultList.Add(new ByteArrayResult(this.byteArrayWriter.ToArray()));
				this.byteArrayWriter = new MemoryStream();
			}
		}

		/// <summary></summary>
		protected virtual bool TryParseContiguousRadixToken(string token, Radix parseRadix, out byte[] result, ref FormatException formatException)
		{
			AssertNotDisposed();

			// String.
			if (parseRadix == Radix.String)
			{
				result = this.encoding.GetBytes(token);
				return (true);
			}

			// Char.
			if (parseRadix == Radix.Char)
			{
				char c;
				if (char.TryParse(token, out c))
				{
					result = this.encoding.GetBytes(new char[] { c });
					return (true);
				}
				else
				{
					formatException = new FormatException(@"Substring """ + token + @""" does not contain a valid single character!");
					result = new byte[] { };
					return (false);
				}
			}

			// Bin/Oct/Dec/Hex.
			bool negative = false;
			string tokenValue = token;
			if (StringEx.EqualsOrdinalIgnoreCase(token.Substring(0, 1), "-"))
			{
				negative = true;
				tokenValue = token.Substring(1);
			}

			ulong value;
			bool success;
			switch (parseRadix)
			{
				case Radix.Bin: success = UInt64Ex.TryParseBinary(tokenValue, out value); break;
				case Radix.Oct: success = UInt64Ex.TryParseOctal (tokenValue, out value); break;
				case Radix.Dec: success = ulong.TryParse         (tokenValue, out value); break;
				case Radix.Hex: success = ulong.TryParse         (tokenValue, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out value); break;
				default: throw (new ArgumentOutOfRangeException("parseRadix", parseRadix, @"Unknown radix """ + parseRadix + @""""));
			}
			if (success)
			{
				bool useBigEndian = (this.endianess == Endianess.BigEndian);
				result = UInt64Ex.ConvertToByteArray(value, negative, useBigEndian);
				return (true);
			}

			// FormatException.
			string readable = "";
			switch (parseRadix)
			{
				case Radix.Bin: readable = "binary value";      break;
				case Radix.Oct: readable = "octal value";       break;
				case Radix.Dec: readable = "decimal value";     break;
				case Radix.Hex: readable = "hexadecimal value"; break;
				default: throw (new ArgumentOutOfRangeException("parseRadix", parseRadix, @"Unknown radix """ + parseRadix + @""""));
			}
			formatException = new FormatException(@"Substring """ + token + @""" contains no valid " + readable);
			result = new byte[] { };
			return (false);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeTopLevelParse(string parseString, ParseMode parseMode)
		{
			this.parseMode       = parseMode;

			this.reader          = new StringReader(parseString);
			this.byteArrayWriter = new MemoryStream();
			this.resultList      = new List<Result>();
			this.state           = new DefaultState();

			this.isKeywordParser = false;

			this.hasFinished     = false;
		}

		private void InitializeNestedParse(ParserState parserState, Parser parent)
		{
			this.encoding        = parent.encoding;
			this.defaultRadix    = parent.defaultRadix;
			this.parseMode       = parent.parseMode;

			this.reader          = parent.reader;
			this.byteArrayWriter = new MemoryStream();
			this.resultList      = parent.resultList;
			this.state           = parserState;

			this.parentParser    = parent;
			this.isKeywordParser = false;

			this.hasFinished     = false;
		}

		/// <summary>
		/// Parses "parseString" for one or more values in the specified base
		/// "parseRadix", separated with spaces.
		/// </summary>
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="parseRadix">Numeric radix.</param>
		/// <param name="result">Array containing the resulting bytes.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns>Bytearray containing the values encoded in Encoding.Default.</returns>
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
					if (token.Length > 0)
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
		/// <param name="parseString">String to be parsed.</param>
		/// <param name="result">Array containing the results.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns>Bytearray containing the values encoded in Encoding.Default.</returns>
		private bool TryParseContiguousKeywords(string parseString, out Result[] result, ref FormatException formatException)
		{
			List<Result> resultList = new List<Result>();
			string[] tokens = parseString.Split(' ');
			foreach (string t in tokens)
			{
				if (t.Length == 0)
					continue;

				try
				{
					resultList.Add(new KeywordResult((KeywordEx)t));
				}
				catch (ArgumentException)
				{
					result = new Result[] { };
					formatException = new FormatException(@"""" + t + @""" is no keyword!");
					return (false);
				}
			}
			result = resultList.ToArray();
			return (true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
