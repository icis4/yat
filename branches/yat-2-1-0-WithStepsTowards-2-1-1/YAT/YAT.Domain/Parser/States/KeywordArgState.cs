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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using MKY;

#endregion

// This code is intentionally placed into the parser base namespace even though the file is located
// in a "\States" sub-directory. The sub-directory shall only group the implementation but not open
// another namespace. The state classes already contain "State" in their name.
namespace YAT.Domain.Parser
{
	/// <summary>
	/// This state handles one or more keyword arguments. The state terminates with the closing ')'
	/// or a separating ','/';'/'|'.
	/// </summary>
	/// <remarks>
	/// So far, this state can only deal with integer values. As soon as floating point, boolean,
	/// enum or string values are required, this state will have to be extended accordingly. Ideas
	/// for implementing support for string values: AllowEscape, ExpectFilePath.
	///
	/// String args will be required when completing FR #13, e.g. \!(Repeat(ABC, 5)).
	///
	/// Note that it was considered to implement this keyword argument handling using Regex, either
	/// a Regex per argument, or a Regex per keyword. However, the big advantage of the YAT-style
	/// parser is its capability to quickly identify the first invalid character, and thus provide
	/// a better error message to the user.
	/// </remarks>
	internal class KeywordArgState : ParserState
	{
		private enum InternalState
		{
			AtBeginning,
			AfterLeadingWhiteSpace,
			InDigits,
			AfterTrailingWhiteSpace
		}

		private Keyword keyword;
		private int[] previousArgs;

		private StringWriter valueWriter;
		private InternalState internalState; // = AtBeginning;
		private Radix radix = Radix.Dec;

		/// <summary></summary>
		public KeywordArgState(Keyword keyword, params int[] previousArgs)
		{
			this.keyword = keyword;
			this.previousArgs = previousArgs;

			this.valueWriter = new StringWriter(CultureInfo.InvariantCulture);
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.valueWriter != null)
						this.valueWriter.Dispose();
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

			if (parser.IsWhiteSpace(parseChar))
			{
				switch (this.internalState)
				{
					case InternalState.AtBeginning:
					{
						// Trim leading whitespace (before the argument):
						while (parser.IsWhiteSpace(parser.CharReader.Peek()))
							parser.CharReader.Read(); // Consume whitespace.

						this.internalState = InternalState.AfterLeadingWhiteSpace;
						return (true);
					}

					case InternalState.InDigits:
					{
						// Trim trailing whitespace (after the argument):
						while (parser.IsWhiteSpace(parser.CharReader.Peek()))
							parser.CharReader.Read(); // Consume whitespace.

						this.internalState = InternalState.AfterTrailingWhiteSpace;
						return (true);
					}

					default: // Something went seriously wrong!
					{
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid internal state '" + this.internalState + "' when parsing whitespace!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
			else if ((parseChar == ')') ||              // ')' => Explicit end of argument(s).
			         (parseChar == CharEx.InvalidChar)) // EOS => Implicit end of argument(s).
			{
				// Trim trailing whitespace (after the closing parenthesis):
				while (parser.IsWhiteSpace(parser.CharReader.Peek()))
					parser.CharReader.Read(); // Consume whitespace.

				var s = this.valueWriter.ToString();
				if (string.IsNullOrEmpty(s))
				{
					if (ArrayEx.IsNullOrEmpty(this.previousArgs)) // Truly empty arg () is OK.
					{
						parser.CommitResult(new KeywordResult(this.keyword));
						parser.HasFinished = true;
						return (true);
					}
					else
					{
						formatException = new FormatException("Empty arguments are no permitted.");
						return (false);
					}
				}

				int thisArg;
				if (!TryParseAndValidate(s, out thisArg, ref formatException))
					return (false);

				var l = new List<int>(); // Default capacity of 4 is OK in most cases.
				if (!ArrayEx.IsNullOrEmpty(this.previousArgs))
					l.AddRange(this.previousArgs);

				l.Add(thisArg);

				parser.CommitResult(new KeywordResult(this.keyword, l.ToArray()));
				parser.HasFinished = true;
				return (true);
			}
			else if ((parseChar == ',') ||
			         (parseChar == ';') ||
			         (parseChar == '|'))
			{
				var s = this.valueWriter.ToString();
				if (string.IsNullOrEmpty(s))
				{
					formatException = new FormatException("Empty arguments are no permitted.");
					return (false);
				}

				int thisArg;
				if (!TryParseAndValidate(s, out thisArg, ref formatException))
					return (false);

				var l = new List<int>(); // Default capacity of 4 is OK in most cases.
				if (!ArrayEx.IsNullOrEmpty(this.previousArgs))
					l.AddRange(this.previousArgs);

				l.Add(thisArg);

				if (!ArgIsAllowed(this.keyword, l.Count, ref formatException))
					return false;

				ChangeState(parser, new KeywordArgState(this.keyword, l.ToArray()));
				return (true);
			}
			else if ((parseChar == '0') && (this.internalState != InternalState.InDigits)) // Potential beginning of a C-style radix identifier.
			{
				if (!ArgIsAllowed(this.keyword, 0, ref formatException))
					return false;

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
						this.radix = Radix.Oct;
						break;
					}

					case 'x': // "\0x.." is used for C-style hexadecimal notation.
					case 'X':
					{
						parser.CharReader.Read(); // Consume 'x' or 'X'.

						this.radix = Radix.Hex;
						break;
					}

					case 'b': // "\0b.." is used for non-standard C-style binary notation.
					case 'B':
					{
						parser.CharReader.Read(); // Consume 'b' or 'B'.

						this.radix = Radix.Bin;
						break;
					}

					default:
					{
						if ((nextChar == ')') ||
						    (nextChar == ',') ||
						    parser.IsWhiteSpace(nextChar) ||
						    (nextChar == CharEx.InvalidChar)) // Just (0) or (0,...) or (0 ) or (0 is OK!
						{
							this.radix = Radix.Dec;
							this.valueWriter.Write((char)parseChar);
							break;
						}
						else
						{
							var sb = new StringBuilder();
							sb.Append(@"Character sequence """);
							sb.Append(parseChar);
							sb.Append(nextChar);
							sb.Append(@""" is not a valid numeric prefix. Valid are ""0"" (octal), ""0x"" (hexadecimal) and ""0b"" (binary).");

							formatException = new FormatException(sb.ToString());
							return (false);
						}
					}
				}

				this.internalState = InternalState.InDigits;
				return (true);
			}
			else if (((parseChar == '+') || (parseChar == '-')) && (this.internalState != InternalState.InDigits)) // Potential inital sign character.
			{
				if (!ArgIsAllowed(this.keyword, 0, ref formatException))
					return false;

				this.valueWriter.Write((char)parseChar);
				this.internalState = InternalState.InDigits;
				return (true);
			}
			else if ((this.internalState == InternalState.AtBeginning) ||
			         (this.internalState == InternalState.AfterLeadingWhiteSpace) ||
			         (this.internalState == InternalState.InDigits))
			{
				if (!ArgIsAllowed(this.keyword, 0, ref formatException))
					return false;

				switch (this.radix)
				{
					case Radix.Bin:
					{
						if ((parseChar >= '0') && (parseChar <= '1'))
						{
							this.valueWriter.Write((char)parseChar);
							this.internalState = InternalState.InDigits;
							return (true);
						}
						break;
					}

					case Radix.Oct:
					{
						if ((parseChar >= '0') && (parseChar <= '7'))
						{
							this.valueWriter.Write((char)parseChar);
							this.internalState = InternalState.InDigits;
							return (true);
						}
						break;
					}

					case Radix.Dec:
					{
						if ((parseChar >= '0') && (parseChar <= '9'))
						{
							this.valueWriter.Write((char)parseChar);
							this.internalState = InternalState.InDigits;
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
							this.internalState = InternalState.InDigits;
							return (true);
						}
						break;
					}

					default:
					{
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + parser.Radix + "' radix is not supported for keyword arguments!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				var sb = new StringBuilder();

				sb.Append("Character '");
				sb.Append((char)parseChar);
				sb.Append("' (0x");
				sb.Append(parseChar.ToString("X", CultureInfo.InvariantCulture));
				sb.Append(") is invalid for ");

				switch (this.radix)
				{
					case Radix.Bin: sb.Append("binary");      break;
					case Radix.Oct: sb.Append("octal");       break;
					case Radix.Dec: sb.Append("decimal");     break;
					case Radix.Hex: sb.Append("hexadecimal"); break;

					default: throw (new ArgumentOutOfRangeException("radix", radix, MessageHelper.InvalidExecutionPreamble + "'" + radix + "' radix is not supported for numeric values!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				sb.Append(" values.");

				formatException = new FormatException(sb.ToString());
				return (false);
			}
			else if (this.internalState == InternalState.AfterTrailingWhiteSpace)
			{
				var sb = new StringBuilder();

				sb.Append("Closing parenthesis expected instead of character '");
				sb.Append((char)parseChar);
				sb.Append("' (0x");
				sb.Append(parseChar.ToString("X", CultureInfo.InvariantCulture));
				sb.Append(").");

				formatException = new FormatException(sb.ToString());
				return (false);
			}
			else
			{
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid internal state '" + this.internalState + "' when parsing!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private bool ArgIsAllowed(Keyword k, int currentArgsCount, ref FormatException ex)
		{
			int maxArgsCount = ((KeywordEx)k).GetMaxArgsCount();

			if (currentArgsCount >= maxArgsCount)
			{
				var sb = new StringBuilder();
				sb.Append("Keyword '");
				sb.Append(this.keyword);

				if (currentArgsCount == 0)
				{
					sb.Append("' does not support arguments.");
				}
				else
				{
					sb.Append("' only supports up to ");
					sb.Append(maxArgsCount.ToString(CultureInfo.InvariantCulture));
					sb.Append(" arguments.");
				}

				ex = new FormatException(sb.ToString());
				return (false);
			}
			else
			{
				return (true);
			}
		}

		protected virtual bool TryParseAndValidate(string s, out int result, ref FormatException ex)
		{
			var me = (KeywordEx)this.keyword;

			int i = 0;
			if (!ArrayEx.IsNullOrEmpty(this.previousArgs))
				i = this.previousArgs.Length;

			if ((!TryParseNumericItem(s, this.radix, out result)) ||
			    (!me.Validate(i, result)))
			{
				var sb = new StringBuilder();
				sb.Append(@"""");

				switch (this.radix)
				{
					case Radix.Bin: sb.Append("0b"); break;
					case Radix.Oct: sb.Append("0");  break;
					case Radix.Dec: sb.Append("");   break;
					case Radix.Hex: sb.Append("0x"); break;

					default: throw (new ArgumentOutOfRangeException("radix", radix, MessageHelper.InvalidExecutionPreamble + "'" + radix + "' radix is not supported for numeric values!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				sb.Append(s);
				sb.Append(@"""");
				sb.Append(" is no valid ");
				sb.Append(i.ToString(CultureInfo.InvariantCulture));
				sb.Append(Int32Ex.ToEnglishSuffix(i));
				sb.Append(" argument for keyword '");
				sb.Append(this.keyword);
				sb.Append("'. Argument must be ");
				sb.Append(me.GetValidationFragment(i));
				sb.Append(".");

				ex = new FormatException(sb.ToString());
				return (false);
			}
			else
			{
				return (true);
			}
		}

		protected static bool TryParseNumericItem(string s, Radix radix, out int result)
		{
			bool isNegative = false;

			switch (s[0])
			{
				case '+': s = s.Remove(0, 1);                    break;
				case '-': s = s.Remove(0, 1); isNegative = true; break;
			}

			switch (radix)
			{
				case Radix.Bin:
				{
					ulong value;
					if (UInt64Ex.TryParseBinary(s, out value))
					{
						if (value <= int.MaxValue)
						{
							result = (int)value;

							if (isNegative)
								result = -result;

							return (true);
						}
					}

					break;
				}

				case Radix.Oct:
				{
					ulong value;
					if (UInt64Ex.TryParseOctal(s, out value))
					{
						if (value <= int.MaxValue)
						{
							result = (int)value;

							if (isNegative)
								result = -result;

							return (true);
						}
					}

					break;
				}

				case Radix.Dec:
				{
					if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
					{
						if (isNegative)
							result = -result;

						return (true);
					}

					break;
				}

				case Radix.Hex:
				{
					if (int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
					{
						if (isNegative)
							result = -result;

						return (true);
					}

					break; // Break switch-case.
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("radix", radix, MessageHelper.InvalidExecutionPreamble + "'" + radix + "' radix is not supported for numeric values!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			result = 0;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
