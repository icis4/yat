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
// YAT 2.0 Gamma 2 Development Version 1.99.35
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

using MKY;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Text;

#endregion

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	internal abstract class ParserState : IDisposable
	{
		private bool isDisposed;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPasstylesByReference", MessageId = "2#", Justification = "Required for recursion.")]
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

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
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
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion
	}

	/// <summary>
	/// This state is the default, it handles contiguous sequences. The state terminates when
	/// entering one of the other states.
	/// </summary>
	internal class DefaultState : ParserState
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

				parser.CommitPendingBytes();
				// Commit bytes and then change state:
				parser.NestedParser = parser.GetParser(new EscapeState(), parser);
				ChangeState(parser, new NestedState());
			}
			else if (parseChar == '<')               // ASCII mnemonic sequence.
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				parser.CommitPendingBytes();
				// Commit bytes and then change state:
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
					byte[] result;

					if (!parser.TryParseContiguousRadix(contiguousString, parser.Radix, out result, ref formatException))
						return (false);

					foreach (byte b in result)
						parser.BytesWriter.WriteByte(b);

					parser.CommitPendingBytes();
				}
				else
				{
					Result[] result;

					if (!parser.TryParseContiguousKeywords(contiguousString, out result, ref formatException))
						return (false);

					parser.CommitResult(result);
				}
			}
			return (true);
		}
	}

	/// <summary>
	/// This state handles a nested context, i.e. is something similar to a stack. The state
	/// terminates when the nested context has terminated.
	/// </summary>
	internal class NestedState : ParserState
	{
		/// <summary></summary>
		public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
		{
			bool parseCharHasBeenParsed = parser.NestedParser.State.TryParse(parser.NestedParser, parseChar, ref formatException);

			if (parser.NestedParser.HasFinished) // Regain parser "focus".
			{
				ChangeState(parser, new DefaultState());

				if (!parseCharHasBeenParsed) // Again try to parse the character with the 'new' parser:
					parseCharHasBeenParsed = parser.State.TryParse(parser, parseChar, ref formatException);
			}

			return (parseCharHasBeenParsed);
		}
	}

	/// <summary>
	/// This state handles an escaping '\' and then passes control to a nested parser.
	/// </summary>
	internal class EscapeState : ParserState
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
						nextChar = parser.CharReader.Peek();
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
							parser.BytesWriter.WriteByte((byte)'\b');
							parser.CommitPendingBytes();
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
					if ((parser.Modes & Modes.Keywords) == Modes.Keywords)
					{
						parser.IsKeywordParser = true;
						ChangeState(parser, new OpeningState());
						return (true);
					}
					else
					{
						// Keywords are disabled, therefore return the escape sequence.
						byte[] b = parser.Encoding.GetBytes(new char[] { '\\', '!' });
						parser.BytesWriter.Write(b, 0, b.Length);
						parser.CommitPendingBytes();
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
						nextChar = parser.CharReader.Peek();
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
							ChangeState(parser, new NumericValueState());
							return (true);
						}

						case 'x': // "\0x.." is used for c-style hexadecimal notation.
						case 'X':
						{
							int thisChar = CharEx.InvalidChar;
							try
							{
								thisChar = parser.CharReader.Read(); // Consume 'x' or 'X'.
							}
							catch (ObjectDisposedException ex)
							{
								DebugEx.WriteException(GetType(), ex); // Debug use only.
							}

							if (thisChar != CharEx.InvalidChar)
							{
								parser.SetDefaultRadix(Radix.Hex);
								ChangeState(parser, new NumericValueState());
								return (true);
							}
							else // Consider it successful if there is just "\0x" without any numeric value.
							{
								parser.CommitPendingBytes();
								parser.HasFinished = true;
								ChangeState(parser, null);
								return (true);
							}
						}

						case StreamEx.EndOfStream: // Just "\0" is used for c-style <NUL>.
						default:
						{
							parser.BytesWriter.WriteByte((byte)'\0');
							parser.CommitPendingBytes();
							parser.HasFinished = true;
							ChangeState(parser, null);
							return (true);
						}
					}
				}

				case 'a': // C-style bell.
				case 'A':
				{
					parser.BytesWriter.WriteByte((byte)'\a');
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				// For case 'b' or 'B' (C-style backspace) see top of this switch-case.

				case 't': // C-style tab.
				case 'T':
				{
					parser.BytesWriter.WriteByte((byte)'\t');
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case 'v': // C-style vertical tab.
				case 'V':
				{
					parser.BytesWriter.WriteByte((byte)'\v');
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case 'n': // C-style <LF>.
				case 'N':
				{
					parser.BytesWriter.WriteByte((byte)'\n');
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case 'r': // C-style <CR>.
				case 'R':
				{
					parser.BytesWriter.WriteByte((byte)'\r');
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case 'f': // C-style <FF>.
				case 'F':
				{
					parser.BytesWriter.WriteByte((byte)'\f');
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case 'x': // C-style hexadecimal value, e.g. "\x1A".
				case 'X':
				{
					parser.SetDefaultRadix(Radix.Hex);
					ChangeState(parser, new NumericValueState());
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
					ChangeState(parser, new NumericValueState(parseChar));
					return (true);
				}

				case '\\':                              // "\\" results in "\".
				{
					byte[] b = parser.Encoding.GetBytes(new char[] { '\\' });
					parser.BytesWriter.Write(b, 0, b.Length);
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case '<':                              // "\<" results in "<".
				{
					byte[] b = parser.Encoding.GetBytes(new char[] { '<' });
					parser.BytesWriter.Write(b, 0, b.Length);
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case '>':                              // "\>" results in ">".
				{
					byte[] b = parser.Encoding.GetBytes(new char[] { '>' });
					parser.BytesWriter.Write(b, 0, b.Length);
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case '(':                              // "\(" results in "(".
				{
					byte[] b = parser.Encoding.GetBytes(new char[] { '(' });
					parser.BytesWriter.Write(b, 0, b.Length);
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case ')':                              // "\)" results in ")".
				{
					byte[] b = parser.Encoding.GetBytes(new char[] { ')' });
					parser.BytesWriter.Write(b, 0, b.Length);
					parser.CommitPendingBytes();
					parser.HasFinished = true;
					ChangeState(parser, null);
					return (true);
				}

				case StreamEx.EndOfStream:
				{
					parser.CommitPendingBytes();
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

	/// <summary>
	/// This state handles an opening '(' and then passes control to a nested parser.
	/// </summary>
	internal class OpeningState : ParserState
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

	/// <summary>
	/// This state handles a sequence of ASCII mnemonics. The sequence may consist of any number
	/// of subsequent characters that form valid ASCII mnemonics. The state terminates with the
	/// closing '>'.
	/// </summary>
	internal class AsciiMnemonicState : ParserState
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
					parser.BytesWriter.WriteByte(b);

				parser.CommitPendingBytes();

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
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPasstylesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		public bool TryParseAsciiMnemonic(Parser parser, string value, out byte[] result, ref FormatException formatException)
		{
			MemoryStream bytes = new MemoryStream();
			string[] items = value.Split(' ');
			foreach (string t in items)
			{
				if (t.Length == 0)
					continue;

				byte code;
				if (Ascii.TryParse(t, out code)) // \ToDo: Also allow contiguous notation such as <CRLF>.
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

	/// <summary>
	/// This state handles a sequence of numeric values in one of the supported radices. The
	/// sequence may consist of any number of subsequent values. The state terminates as soon as
	/// a non-supported character is found.
	/// </summary>
	/// <remarks>
	/// +/- signs are not allowed, neither are decimal points nor separators such as the apostroph.
	/// </remarks>
	internal class NumericValueState : ParserState
	{
		private StringWriter valueWriter;

		/// <summary></summary>
		public NumericValueState()
		{
			this.valueWriter = new StringWriter(CultureInfo.InvariantCulture);
		}

		/// <summary></summary>
		public NumericValueState(int parseChar)
			: this()
		{
			this.valueWriter.Write((char)parseChar);
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
				if (this.valueWriter != null)
					this.valueWriter.Dispose();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.valueWriter = null;
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
				case Radix.Bin:
				{
					if ((parseChar >= '0') && (parseChar <= '1'))
					{
						this.valueWriter.Write((char)parseChar);
						return (true);
					}
					break;
				}

				case Radix.Oct:
				{
					if ((parseChar >= '0') && (parseChar <= '7'))
					{
						this.valueWriter.Write((char)parseChar);
						return (true);
					}
					break;
				}

				case Radix.Dec:
				{
					if ((parseChar >= '0') && (parseChar <= '9'))
					{
						this.valueWriter.Write((char)parseChar);
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
						this.valueWriter.Write((char)parseChar);
						return (true);
					}
					break;
				}

				default:
				{
					throw (new InvalidOperationException("Program execution should never get here, radix " + parser.Radix + " is not supported, please report this bug!"));
				}
			}

			// No more valid character found, try to process numeric value.
			byte[] result;
			if (parser.TryParseAndConvertNumericItem(this.valueWriter.ToString(), out result, ref formatException))
			{
				foreach (byte b in result)
					parser.BytesWriter.WriteByte(b);

				parser.CommitPendingBytes();

				parser.HasFinished = true;
				ChangeState(parser, null);
				return (false); // Return 'false' to indicate that the current 'parseChar' has not been processed yet!
			}
			else
			{
				parser.HasFinished = true;
				ChangeState(parser, null);
				return (false); // Return 'false' to indicate that the current 'parseChar' has not been processed yet!
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
