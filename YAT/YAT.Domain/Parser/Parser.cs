//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
using MKY.Diagnostics;
using MKY.IO;
using MKY.Text;

#endregion

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Parser : IDisposable
	{
		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
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
			@"Three letter radix identifiers are also allowed, e.g. ""\hex"" alternative to ""\h""" + Environment.NewLine +
			Environment.NewLine +
			@"In addition, C-style escape sequences are supported:" + Environment.NewLine +
			@"""\r\n"" alternative to ""<CR><LF>""" + Environment.NewLine +
			@"""\0"" alternative to ""<NUL>"" or \d(0) or \h(0)" + Environment.NewLine +
			@"""\01"" alternative to \o(1)" + Environment.NewLine +
			@"""\12"" alternative to \d(12)" + Environment.NewLine +
			@"""\0x1A"" or ""\x1A"" alternative to \h(1A)" + Environment.NewLine +
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
			@"Delay ""Send something\!(" + (KeywordEx)Keyword.Delay + @")Send delayed"" according to advanced settings" + Environment.NewLine +
			@"Delay after ""Send something and then delay\!(" + (KeywordEx)Keyword.LineDelay + @")"" according to advanced settings" + Environment.NewLine +
			@"Repeat ""Send something multiple times\!(" + (KeywordEx)Keyword.LineRepeat + @")"" according to advanced settings" + Environment.NewLine +
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
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for recursion.")]
			public abstract bool TryParse(Parser parser, int parseChar, ref FormatException formatException);

			/// <summary></summary>
			protected static void ChangeState(Parser parser, ParserState state)
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
					// Dispose of managed resources if requested:
					if (disposing)
					{
					}

					// Set state to disposed:
					this.isDisposed = true;
				}
			}

			/// <summary></summary>
			~ParserState()
			{
				Dispose(false);
			}

			/// <summary></summary>
			public bool IsDisposed
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
				if (!IsDisposed)
				{
					// In any case, dispose of the writer as it was created in the constructor:
					if (this.contiguousWriter != null)
						this.contiguousWriter.Dispose();

					// Dispose of managed resources if requested:
					if (disposing)
					{
					}

					// Set state to disposed:
					this.contiguousWriter = null;
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
						int nextChar = CharEx.InvalidChar;
						try
						{
							nextChar = parser.Reader.Peek();
						}
						catch (ObjectDisposedException ex)
						{
							DebugEx.WriteException(GetType(), ex); // Debug use only.
						}

						switch (nextChar)
						{
							case '(': // "\b(...)" is used for binary values, e.g. "\b(010110001)".
							{
								parser.SetDefaultRadix(Radix.Bin);
								ChangeState(parser, new OpeningState());
								return (true);
							}

							case StreamEx.EndOfStream: // Just "\b" is used for c-style backspace.
							default:
							{
								parser.ByteArrayWriter.WriteByte((byte)'\b');
								parser.EndByteArray();
								parser.HasFinished = true;
								ChangeState(parser, null);
								return (true);
							}
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
						if ((parser.modes & Modes.Keywords) == Modes.Keywords)
						{
							parser.IsKeywordParser = true;
							ChangeState(parser, new OpeningState());
							return (true);
						}
						else
						{
							// Keywords are disabled, therefore return the escape sequence.
							byte[] b = parser.Encoding.GetBytes(new char[] { '\\', '!' });
							parser.ByteArrayWriter.Write(b, 0, b.Length);
							parser.EndByteArray();
							parser.HasFinished = true;
							ChangeState(parser, null);
							return (true);
						}
					}

					case '0':
					{
						int nextChar = CharEx.InvalidChar;
						try
						{
							nextChar = parser.Reader.Peek();
						}
						catch (ObjectDisposedException ex)
						{
							DebugEx.WriteException(GetType(), ex); // Debug use only.
						}

						switch (nextChar)
						{
							case '0': // "\0<value>" is used for c-style octal notation.
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

							case 'x': // "\0x.." is used for c-style hexadecimal notation.
							case 'X':
							{
								int thisChar = CharEx.InvalidChar;
								try
								{
									thisChar = parser.Reader.Read(); // Consume 'x' or 'X'.
								}
								catch (ObjectDisposedException ex)
								{
									DebugEx.WriteException(GetType(), ex); // Debug use only.
								}

								if (thisChar != CharEx.InvalidChar)
								{
									parser.SetDefaultRadix(Radix.Hex);
									ChangeState(parser, new NumericState());
									return (true);
								}
								else // Consider it successful if there is just "\0x" without any numeric value.
								{
									parser.EndByteArray();
									parser.HasFinished = true;
									ChangeState(parser, null);
									return (true);
								}
							}

							case StreamEx.EndOfStream: // Just "\0" is used for c-style <NUL>.
							default:
							{
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

					// For case 'b' or 'B' (C-style backspace) see top of this switch-case.

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

					case 'x': // C-style hexadecimal value, e.g. "\x1A".
					case 'X':
					{
						parser.SetDefaultRadix(Radix.Hex);
						ChangeState(parser, new NumericState());
						return (true);
					}

					case '1': // C-style decimal value, e.g. "\12".
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
				if (!IsDisposed)
				{
					// In any case, dispose of the writer as it was created in the constructor:
					if (this.mnemonicWriter != null)
						this.mnemonicWriter.Dispose();

					// Dispose of managed resources if requested:
					if (disposing)
					{
					}

					// Set state to disposed:
					this.mnemonicWriter = null;
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
			/// Parses <paramref name="value"/> for ASCII mnemonics.
			/// </summary>
			/// <param name="parser">Parser to retrieve settings.</param>
			/// <param name="value">String to be parsed.</param>
			/// <param name="result">Array containing the resulting bytes.</param>
			/// <param name="formatException">Returned if invalid string format.</param>
			/// <returns>Byte array containing the values encoded in Encoding.Default.</returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
			public static bool TryParseAsciiMnemonic(Parser parser, string value, out byte[] result, ref FormatException formatException)
			{
				MemoryStream bytes = new MemoryStream();
				string[] tokens = value.Split(' ');
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
						formatException = new FormatException(@"""" + t + @""" is no ASCII mnemonic!");
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
				if (!IsDisposed)
				{
					// In any case, dispose of the writer as it was created in the constructor:
					if (this.numericWriter != null)
						this.numericWriter.Dispose();

					// Dispose of managed resources if requested:
					if (disposing)
					{
					}

					// Set state to disposed:
					this.numericWriter = null;
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
			/// Parses <paramref name="value"/> for ASCII mnemonics.
			/// </summary>
			/// <param name="parser">Parser to retrieve settings.</param>
			/// <param name="value">String to be parsed.</param>
			/// <param name="result">Array containing the resulting bytes.</param>
			/// <param name="formatException">Returned if invalid string format.</param>
			/// <returns>Byte array containing the values encoded in Encoding.Default.</returns>
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using UInt64 for orthogonality with UInt64Ex.")]
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
			public static bool TryParseNumericValue(Parser parser, string value, out byte[] result, ref FormatException formatException)
			{
				switch (parser.Radix)
				{
					case Radix.Oct:
					{
						UInt64 tempResult;
						if (UInt64Ex.TryParseOctal(value, out tempResult))
						{
							result = UInt64Ex.ConvertToByteArray(tempResult);
							return (true);
						}
						else
						{
							formatException = new FormatException(@"""" + value + @""" is no valid octal value!");
						}
						break;
					}

					case Radix.Dec:
					{
						UInt64 tempResult;
						if (UInt64.TryParse(value, out tempResult))
						{
							result = UInt64Ex.ConvertToByteArray(tempResult);
							return (true);
						}
						else
						{
							formatException = new FormatException(@"""" + value + @""" is no valid decimal value!");
						}
						break;
					}

					case Radix.Hex:
					{
						MemoryStream bytes = new MemoryStream();
						string errorString = null;
						foreach (string s in StringEx.SplitFixedLength(value, 2))
						{
							byte b;
							if (byte.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
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
							formatException = new FormatException(@"Substring """ + errorString + @""" of """ + value + @""" is no valid hexadecimal value!");
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

		private Endianness endianness = Endianness.BigEndian;
		private Encoding encoding = Encoding.Default;
		private Radix defaultRadix = Radix.String;
		private Modes modes = Modes.All;

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public Parser(Endianness endianness)
		{
			this.endianness = endianness;
		}

		/// <summary></summary>
		public Parser(Encoding encoding)
		{
			this.encoding = encoding;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public Parser(Endianness endianness, Encoding encoding)
		{
			this.endianness = endianness;
			this.encoding   = encoding;
		}

		/// <summary></summary>
		public Parser(Radix defaultRadix)
		{
			this.defaultRadix = defaultRadix;
		}

		/// <summary></summary>
		public Parser(Encoding encoding, Radix defaultRadix)
		{
			this.encoding     = encoding;
			this.defaultRadix = defaultRadix;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public Parser(Endianness endianness, Encoding encoding, Radix defaultRadix)
		{
			this.endianness   = endianness;
			this.encoding     = encoding;
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
				// In any case, dispose of the writers as they were created in the constructor:
				if (this.reader != null)
					this.reader.Dispose();

				if (this.byteArrayWriter != null)
					this.byteArrayWriter.Dispose();

				if (this.state != null)
					this.state.Dispose();

				if (this.nestedChildParser != null)
					this.nestedChildParser.Dispose();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.reader = null;
				this.byteArrayWriter = null;
				this.state = null;
				this.nestedChildParser = null;
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Parser()
		{
			Dispose(false);
		}

		/// <summary></summary>
		public bool IsDisposed
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
		public virtual Endianness Endianness
		{
			get { return (this.endianness); }
		}

		/// <summary></summary>
		public virtual Encoding Encoding
		{
			get { return (this.encoding); }
		}

		/// <summary></summary>
		/// <remarks>Radix: public get, private set.</remarks>
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

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
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
			// AssertNotDisposed() is called by 'Parse()' below.

			string parsed;
			return (Parse(s, out parsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual byte[] Parse(string s, out string parsed)
		{
			// AssertNotDisposed() is called by 'Parse()' below.

			Result[] resultResult = Parse(s, Modes.AllByteArrayResults, out parsed);
			MemoryStream byteResult = new MemoryStream();
			foreach (Result r in resultResult)
			{
				ByteArrayResult bar = r as ByteArrayResult;
				if (bar != null)
				{
					byte[] a = bar.ByteArray;
					byteResult.Write(a, 0, a.Length);
				}
			}
			return (byteResult.ToArray());
		}

		/// <summary></summary>
		public virtual Result[] Parse(string s, Modes modes)
		{
			// AssertNotDisposed() is called by 'Parse()' below.

			string parsed;
			return (Parse(s, modes, out parsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual Result[] Parse(string s, Modes modes, out string parsed)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] result;
			FormatException formatException = new FormatException("");
			if (!TryParse(s, modes, out result, out parsed, ref formatException))
				throw (formatException);
			return (result);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, out byte[] result)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			string parsed;
			return (TryParse(s, out result, out parsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, out byte[] result, out string parsed)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			return (TryParse(s, Modes.AllByteArrayResults, out result, out parsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, Modes modes, out byte[] result, out string parsed)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] resultResult;
			bool tryResult = TryParse(s, modes, out resultResult, out parsed);

			MemoryStream byteResult = new MemoryStream();
			foreach (Result r in resultResult)
			{
				ByteArrayResult bar = r as ByteArrayResult;
				if (bar != null)
				{
					byte[] a = bar.ByteArray;
					byteResult.Write(a, 0, a.Length);
				}
			}
			result = byteResult.ToArray();

			return (tryResult);
		}

		/// <summary></summary>
		public virtual bool TryParse(string s)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			return (TryParse(s, Modes.All));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, Modes modes)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, modes, out result));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, Modes modes, out Result[] result)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			string parsed;
			return (TryParse(s, modes, out result, out parsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, Modes modes, out string parsed)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, modes, out result, out parsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, Modes modes, out string parsed, ref FormatException formatException)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, modes, out result, out parsed, ref formatException));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, Modes modes, out Result[] result, out string parsed)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, modes, out result, out parsed, ref formatException));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, Modes modes, out Result[] result, out string parsed, ref FormatException formatException)
		{
			AssertNotDisposed();

			InitializeTopLevelParse(s, modes);

			while (!HasFinished)
			{
				int c = CharEx.InvalidChar;
				try
				{
					c = this.Reader.Read();
				}
				catch (ObjectDisposedException ex)
				{
					DebugEx.WriteException(GetType(), ex); // Debug use only.
				}

				if (!this.state.TryParse(this, c, ref formatException))
				{
					EndByteArray();

					string remaining = null;
					try
					{
						remaining = this.Reader.ReadToEnd();
					}
					catch (ObjectDisposedException ex)
					{
						DebugEx.WriteException(GetType(), ex); // Debug use only.
					}

					if (remaining == null)
					{
						// Signal that parsing resulted in a severe stream error:
						parsed = null;
						result = null;
						return (false);
					}
					else
					{
						// Signal that parsing resulted in a parse error and
						//   return the part of the string that could be parsed:
						parsed = StringEx.Left(s, s.Length - remaining.Length - 1);
						result = this.resultList.ToArray();
						return (false);
					}
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		protected virtual bool TryParseContiguousRadixToken(string token, Radix radix, out byte[] result, ref FormatException formatException)
		{
			AssertNotDisposed();

			// String.
			if (radix == Radix.String)
			{
				result = this.encoding.GetBytes(token);
				return (true);
			}

			// Char.
			if (radix == Radix.Char)
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
			switch (radix)
			{
				case Radix.Bin: success = UInt64Ex.TryParseBinary(tokenValue, out value); break;
				case Radix.Oct: success = UInt64Ex.TryParseOctal (tokenValue, out value); break;
				case Radix.Dec: success = ulong.TryParse         (tokenValue, out value); break;
				case Radix.Hex: success = ulong.TryParse         (tokenValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value); break;
				default: throw (new ArgumentOutOfRangeException("radix", radix, @"Unknown radix """ + radix + @""""));
			}
			if (success)
			{
				bool useBigEndian = (this.endianness == Endianness.BigEndian);
				result = UInt64Ex.ConvertToByteArray(value, negative, useBigEndian);
				return (true);
			}

			// FormatException.
			string readable = "";
			switch (radix)
			{
				case Radix.Bin: readable = "binary value";      break;
				case Radix.Oct: readable = "octal value";       break;
				case Radix.Dec: readable = "decimal value";     break;
				case Radix.Hex: readable = "hexadecimal value"; break;
				default: throw (new ArgumentOutOfRangeException("radix", radix, @"Unknown radix """ + radix + @""""));
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

		private void InitializeTopLevelParse(string value, Modes modes)
		{
			this.modes           = modes;

			this.reader          = new StringReader(value);
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
			this.modes           = parent.modes;

			this.reader          = parent.reader;
			this.byteArrayWriter = new MemoryStream();
			this.resultList      = parent.resultList;
			this.state           = parserState;

			this.parentParser    = parent;
			this.isKeywordParser = false;

			this.hasFinished     = false;
		}

		/// <summary>
		/// Parses <paramref name="value"/> for one or more values in the specified base
		/// <paramref name="radix"/>, separated with spaces.
		/// </summary>
		/// <param name="value">String to be parsed.</param>
		/// <param name="radix">Numeric radix.</param>
		/// <param name="result">Array containing the resulting bytes.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		/// <exception cref="OverflowException">Thrown if a value cannot be converted into bytes.</exception>
		private bool TryParseContiguousRadix(string value, Radix radix, out byte[] result, ref FormatException formatException)
		{
			MemoryStream bytes = new MemoryStream();
			if (radix == Radix.String)
			{
				byte[] b;
				if (TryParseContiguousRadixToken(value, Radix.String, out b, ref formatException))
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
				string[] tokens = value.Split(' ');
				foreach (string token in tokens)
				{
					if (token.Length > 0)
					{
						byte[] b;
						if (TryParseContiguousRadixToken(token, radix, out b, ref formatException))
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
		/// Parses <paramref name="value"/> for keywords.
		/// </summary>
		/// <param name="value">String to be parsed.</param>
		/// <param name="result">Array containing the results.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Non-static for orthogonality with other TryParse() methods.")]
		private bool TryParseContiguousKeywords(string value, out Result[] result, ref FormatException formatException)
		{
			List<Result> resultList = new List<Result>();
			string[] tokens = value.Split(' ');
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
