//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Globalization;

using MKY;
using MKY.IO;

// This code is intentionally placed into the parser base namespace even though the file is located
// in a "\States" sub-directory. The sub-directory shall only group the implementation but not open
// another namespace. The state classes already contain "State" in their name.
namespace YAT.Domain.Parser
{
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
					int nextChar = parser.CharReader.Peek();
					switch (nextChar)
					{
						case '(': // "\b(...)" is used for binary values, e.g. "\b(010110001)".
						{
							parser.SetDefaultRadix(Radix.Bin);
							ChangeState(parser, new OpeningState());
							return (true);
						}

						case StreamEx.EndOfStream: // Just "\b" is used for C-style backspace.
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

				case 'u':
				case 'U':
				{
					int nextChar = parser.CharReader.Peek();
					switch (nextChar)
					{
						case '(': // "\u(....)" is used for sequential Unicode notation, e.g. "\u(0020 00AA 0020)".
						{
							parser.SetDefaultRadix(Radix.Unicode);
							ChangeState(parser, new OpeningState());
							return (true);
						}

						case '+': // "\U+...." is used for standard Unicode notation, e.g. "\U+0020".
						{
							int thisChar = parser.CharReader.Read(); // Consume '+'.
							if (thisChar != CharEx.InvalidChar)
							{
								parser.SetDefaultRadix(Radix.Unicode);
								ChangeState(parser, new NumericValueState());
								return (true);
							}
							else // Something went seriously wrong!
							{
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Failed to read '+' from input stream!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}

						default: // "\u" with neither "(" nor "+" is used for C-style Unicode notation, e.g. "\u0020"
						{
							parser.SetDefaultRadix(Radix.Unicode);
							ChangeState(parser, new NumericValueState());
							return (true);
						}
					}
				}

				case '!':
				{
					if ((parser.Modes & Modes.Keywords) != 0)
					{
						parser.IsKeywordParser = true;
						parser.DoProbe         = false; // Keywords cannot be probed (yet).
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
					int nextChar = parser.CharReader.Peek();
					switch (nextChar)
					{
						case '0': // "\0<value>" is used for C-style octal notation.
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

						case 'x': // "\0x.." is used for C-style hexadecimal notation.
						case 'X':
						{
							int thisChar = parser.CharReader.Read(); // Consume 'x' or 'X'.
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

						case StreamEx.EndOfStream: // Just "\0" is used for C-style <NUL>.
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

				// For case 'b' or 'B' (C-style backspace) see further up of this switch-case.

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

				// For case 'u' or 'U' (C-style Unicode) see further up of this switch-case.

				case 'x': // C-style hexadecimal notation, e.g. "\x1A".
				case 'X':
				{
					parser.SetDefaultRadix(Radix.Hex);
					ChangeState(parser, new NumericValueState());
					return (true);
				}

				case '1': // C-style decimal notation, e.g. "\12".
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

				case CharEx.InvalidChar:
				{
					formatException = new FormatException
					(
						"Incomplete escape sequence."
					);
					return (false);
				}

				default:
				{
					formatException = new FormatException
					(
						@"Character '" + (char)parseChar + "' " +
						@"(0x" + parseChar.ToString("X", CultureInfo.InvariantCulture) + ") " +
						@"is an invalid escape character."
					);
					return (false);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
