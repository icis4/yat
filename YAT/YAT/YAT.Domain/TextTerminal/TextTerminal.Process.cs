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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of 'GlueCharsOfLine':
////#define DEBUG_GLUE_CHARS_OF_LINE

	// Enable debugging of 'WaitForResponse':
////#define DEBUG_WAIT_FOR_RESPONSE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;

using MKY;
using MKY.Collections;
using MKY.Diagnostics;
using MKY.Text;

using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the process part of <see cref="TextTerminal"/>.
	/// </remarks>
	public partial class TextTerminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private TextUnidirState textTxState;
		private TextUnidirState textBidirTxState;
		private TextUnidirState textBidirRxState;
		private TextUnidirState textRxState;

		private ProcessTimeout glueCharsOfLineTimeout;

		private readonly object waitForResponseClearanceSyncObj = new object();
		private int waitForResponseResponseCounter; // = 0 and will again be initialized to that.
		private int waitForResponseClearanceCounter; // = 0 but will be initialized to settings.
		private DateTime waitForResponseClearanceTimeStamp; // = MinValue and will again be initialized to that.
		private ManualResetEvent waitForResponseEvent = new ManualResetEvent(false);

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region ByteTo.../...Element
		//------------------------------------------------------------------------------------------
		// ByteTo.../...Element
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override DisplayElement ByteToElement(byte b, DateTime ts, IODirection dir, Radix r, List<byte> pendingMultiBytesToDecode)
		{
			switch (r)
			{
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					return (base.ByteToElement(b, ts, dir, r, pendingMultiBytesToDecode));
				}

				case Radix.String:
				case Radix.Char:
				case Radix.Unicode:
				{
					Encoding e = (EncodingEx)TextTerminalSettings.Encoding;
					if (e.IsSingleByte)
					{
						// Note that the following code is similar as several time below but with subtle differences
						// such as treatment of 0xFF, comment,...

						if      ((b < 0x20) || (b == 0x7F))          // ASCII control characters.
						{
							return (base.ByteToElement(b, ts, dir, r, pendingMultiBytesToDecode));
						}
						else if  (b == 0x20)                         // ASCII space.
						{
							return (base.ByteToElement(b, ts, dir, r, pendingMultiBytesToDecode));
						}                                            // Special case.
						else if ((b == 0xFF) && TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
						{
							return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
						}
						else                                         // ASCII and extended ASCII printable characters.
						{
							return (DecodeAndCreateElement(b, ts, dir, r, e)); // "IsSingleByte" always results in a single character per byte.
						}
					}
					else // "IsMultiByte":
					{
						// \remind (2017-12-09 / MKY / bug #400)
						// YAT versions 1.99.70 and 1.99.80 used to take the endianness into account when encoding
						// and decoding multi-byte encoded characters. However, it was always done, but of course e.g.
						// UTF-8 is independent on endianness. The endianness would only have to be applied to single
						// multi-byte values, not multi-byte values split into multiple fragments. However, a .NET
						// "Encoding" object does not tell whether the encoding is potentially endianness capable or
						// not. Thus, it was decided to again remove the character encoding endianness awareness.

						if (((EncodingEx)e).IsUnicode) // Note that UTF-7 is not a Unicode encoding.
						{
							// Note that the following code is similar as above and below but with subtle differences
							// such as no treatment of a lead byte, no treatment of 0xFF, treatment of 0xFFFD, comment,...

							pendingMultiBytesToDecode.Add(b);

							int remainingBytesInFragment = (pendingMultiBytesToDecode.Count % ((EncodingEx)e).UnicodeFragmentByteCount);
							if (remainingBytesInFragment > 0)
							{
								return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
							}

							byte[] decodingArray = pendingMultiBytesToDecode.ToArray();
							int expectedCharCount = e.GetCharCount(decodingArray);
							char[] chars = new char[expectedCharCount];
							int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
							if (effectiveCharCount == 1)
							{
								int code = chars[0];
								if (code != 0xFFFD) // Ensure that 'unknown' character 0xFFFD is not decoded yet.
								{
									pendingMultiBytesToDecode.Clear();

									if      ((code < 0x20) || (code == 0x7F))        // ASCII control characters.
									{
										return (base.ByteToElement((byte)code, ts, dir, r, pendingMultiBytesToDecode));
									}
									else if (code == 0x20)                           // ASCII space.
									{
										return (base.ByteToElement((byte)code, ts, dir, r, pendingMultiBytesToDecode));
									}
									else                                             // ASCII printable character.
									{                                                              // "effectiveCharCount" is 1 for sure.
										return (CreateDataElement(decodingArray, ts, dir, r, chars[0]));
									}
								}
								else // Single 'unknown' character 0xFFFD:
								{
									return (new DisplayElement.Nonentity());         // Nothing to decode (yet).
								}
							}
							else if (effectiveCharCount == 0)
							{
								if (decodingArray.Length < e.GetMaxByteCount(1))
								{
									return (new DisplayElement.Nonentity());         // Nothing to decode (yet).
								}
								else
								{
									pendingMultiBytesToDecode.Clear();               // Reset decoding stream.

									return (HandleInvalidBytes(decodingArray, ts, dir, e));
								}
							}
							else // (effectiveCharCount > 1) => Code doesn't fit into a single u16 value, thus more than one character will be returned.
							{
								pendingMultiBytesToDecode.Clear();                   // Reset decoding stream.

								return (HandleOutsideUnicodePlane0(decodingArray, ts, dir, e));
							}
						}
						else // Non-Unicode DBCS/MBCS.
						{
							// Note that the following code is similar as several times above but with subtle differences
							// such as treatment of a lead byte, no treatment of 0xFF, no treatment of 0xFFFD, comment,...

							if (pendingMultiBytesToDecode.Count == 0)        // A first 'MultiByte' is either ASCII or lead byte.
							{
								if      (b >= 0x80)                          // DBCS/MBCS lead byte.
								{
									pendingMultiBytesToDecode.Add(b);

									return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
								}
								else if ((b < 0x20) || (b == 0x7F))          // ASCII control characters.
								{
									return (base.ByteToElement(b, ts, dir, r, pendingMultiBytesToDecode));
								}
								else if (b == 0x20)                          // ASCII space.
								{
									return (base.ByteToElement(b, ts, dir, r, pendingMultiBytesToDecode));
								}
								else                                         // ASCII printable character.
								{
									return (DecodeAndCreateElement(b, ts, dir, r, e)); // "IsMultiByte" but the current byte must result in a single character here.
								}
							}
							else // (pendingMultiBytesToDecode.Count > 0) => Neither ASCII nor lead byte.
							{
								pendingMultiBytesToDecode.Add(b);

								byte[] decodingArray = pendingMultiBytesToDecode.ToArray();
								int expectedCharCount = e.GetCharCount(decodingArray);
								char[] chars = new char[expectedCharCount];
								int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
								if (effectiveCharCount == 1)
								{
									pendingMultiBytesToDecode.Clear();
									                                                    //// "effectiveCharCount" is 1 for sure.
									return (CreateDataElement(decodingArray, ts, dir, r, chars[0]));
								}
								else if (effectiveCharCount == 0)
								{
									if (decodingArray.Length < e.GetMaxByteCount(1))
									{
										return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
									}
									else
									{
										pendingMultiBytesToDecode.Clear(); // Reset decoding stream.

										return (HandleInvalidBytes(decodingArray, ts, dir, e));
									}
								}
								else // (effectiveCharCount > 1)
								{
									pendingMultiBytesToDecode.Clear(); // Reset decoding stream.

									return (HandleInvalidBytes(decodingArray, ts, dir, e));
								}
							} // ASCII or lead or trailing byte
						} // Unicode/Non-Unicode
					} // MultiByte
				} // String/Char/Unicode

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a radix that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement DecodeAndCreateElement(byte b, DateTime ts, IODirection dir, Radix r, Encoding e)
		{
			int expectedCharCount = 1;
			char[] chars = new char[expectedCharCount];
			int effectiveCharCount = e.GetDecoder().GetChars(new byte[] { b }, 0, 1, chars, 0, true);
			if (effectiveCharCount == expectedCharCount)
			{                                                  // "effectiveCharCount" is 1 for sure.
				return (CreateDataElement(b, ts, dir, r, chars[0]));
			}
			else // Decoder has failed:
			{
				return (HandleInvalidByte(b, ts, dir, e));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte origin, DateTime ts, IODirection dir, Radix r, char c)
		{
			if (r != Radix.Unicode)
			{
				string text = c.ToString(CultureInfo.InvariantCulture);
				return (CreateDataElement(origin, ts, dir, text));
			}
			else // Unicode:
			{
				string text = UnicodeValueToNumericString(c);
				return (CreateDataElement(origin, ts, dir, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, DateTime ts, IODirection dir, Radix r, char c)
		{
			if (r != Radix.Unicode)
			{
				string text = c.ToString(CultureInfo.InvariantCulture);
				return (CreateDataElement(origin, ts, dir, text));
			}
			else // Unicode:
			{
				string text = UnicodeValueToNumericString(c);
				return (CreateDataElement(origin, ts, dir, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, DateTime ts, IODirection dir, Radix r, char[] text)
		{
			if (r != Radix.Unicode)
			{
				return (CreateDataElement(origin, ts, dir, new string(text)));
			}
			else // Unicode:
			{
				return (CreateDataElement(origin, ts, dir, new string(text)));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement HandleInvalidByte(byte b, DateTime ts, IODirection dir, Encoding e)
		{
			switch (TextTerminalSettings.DecodingMismatchBehavior)
			{
				case DecodingMismatchBehavior.ComprehensiveWarning:                         return (CreateComprehensiveInvalidByteWarning(b, ts, dir, e));
				case DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning: return (CreateReplacementCharacterAndCompactInvalidByteWarning(b, ts, dir, e, "�"));
				case DecodingMismatchBehavior.UnicodeReplacementCharacter:                  return (CreateReplacementCharacterElement(b, ts, dir, "�"));
				case DecodingMismatchBehavior.QuestionMarkAndCompactWarning:                return (CreateReplacementCharacterAndCompactInvalidByteWarning(b, ts, dir, e, "?"));
				case DecodingMismatchBehavior.QuestionMark:                                 return (CreateReplacementCharacterElement(b, ts, dir, "?"));
				case DecodingMismatchBehavior.Discard:                                      return (CreateDiscardElement(b, ts, dir));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + TextTerminalSettings.DecodingMismatchBehavior.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement HandleInvalidBytes(byte[] a, DateTime ts, IODirection dir, Encoding e)
		{
			switch (TextTerminalSettings.DecodingMismatchBehavior)
			{
				case DecodingMismatchBehavior.ComprehensiveWarning:                         return (CreateComprehensiveInvalidBytesWarning(a, ts, dir, e));
				case DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning: return (CreateReplacementCharacterAndCompactInvalidBytesWarning(a, ts, dir, e, "�"));
				case DecodingMismatchBehavior.UnicodeReplacementCharacter:                  return (CreateReplacementCharacterElement(a, ts, dir, "�"));
				case DecodingMismatchBehavior.QuestionMarkAndCompactWarning:                return (CreateReplacementCharacterAndCompactInvalidBytesWarning(a, ts, dir, e, "?"));
				case DecodingMismatchBehavior.QuestionMark:                                 return (CreateReplacementCharacterElement(a, ts, dir, "?"));
				case DecodingMismatchBehavior.Discard:                                      return (CreateDiscardElement(a, ts, dir));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + TextTerminalSettings.DecodingMismatchBehavior.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement HandleOutsideUnicodePlane0(byte[] a, DateTime ts, IODirection dir, Encoding e)
		{
			switch (TextTerminalSettings.DecodingMismatchBehavior)
			{
				case DecodingMismatchBehavior.ComprehensiveWarning:                         return (CreateComprehensiveOutsideUnicodePlane0Warning(a, ts, dir, e));
				case DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning: return (CreateReplacementCharacterAndCompactInvalidBytesWarning(a, ts, dir, e, "�"));
				case DecodingMismatchBehavior.UnicodeReplacementCharacter:                  return (CreateReplacementCharacterElement(a, ts, dir, "�"));
				case DecodingMismatchBehavior.QuestionMarkAndCompactWarning:                return (CreateReplacementCharacterAndCompactInvalidBytesWarning(a, ts, dir, e, "?"));
				case DecodingMismatchBehavior.QuestionMark:                                 return (CreateReplacementCharacterElement(a, ts, dir, "?"));
				case DecodingMismatchBehavior.Discard:                                      return (CreateDiscardElement(a, ts, dir));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + TextTerminalSettings.DecodingMismatchBehavior.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateComprehensiveInvalidByteWarning(byte b, DateTime ts, IODirection dir, Encoding e)
		{
			var byteAsString = ByteHelper.FormatHexString(b, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@""""); // Not using [...] as that is the warning enclosure.
			sb.Append(byteAsString);
			sb.Append(@""" is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte!");

			return (new DisplayElement.WarningInfo(ts, (Direction)dir, b, sb.ToString()));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateComprehensiveInvalidBytesWarning(byte[] a, DateTime ts, IODirection dir, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@""""); // Not using [...] as that is the warning enclosure.
			sb.Append(bytesAsString);
			sb.Append(@""" is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte sequence!");

			return (new DisplayElement.WarningInfo(ts, (Direction)dir, a, sb.ToString()));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateComprehensiveOutsideUnicodePlane0Warning(byte[] a, DateTime ts, IODirection dir, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@""""); // Not using [...] as that is the warning enclosure.
			sb.Append(bytesAsString);
			sb.Append(@""" is a byte sequence outside the Unicode basic multilingual plane (plane 0)! Only Unicode plane 0 is supported by .NET Framework and thus " + ApplicationEx.CommonName + " (yet).");

			return (new DisplayElement.WarningInfo(ts, (Direction)dir, a, sb.ToString()));
		}

		/// <remarks>
		/// <see cref="DisplayElement"/> requires a string, thus using <c>string</c> instead of <c>char</c> for replacement character.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateReplacementCharacterAndCompactInvalidByteWarning(byte b, DateTime ts, IODirection dir, Encoding e, string replacementCharacter)
		{
			var byteAsString = ByteHelper.FormatHexString(b, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(replacementCharacter);
			sb.Append("["); // Manually adding [...] to be able to combine replacement character and byte value.
			sb.Append(byteAsString);
			sb.Append("]");

			return (new DisplayElement.WarningInfo(ts, (Direction)dir, b, sb.ToString(), omitBracketsAndLabel: true));
		}

		/// <remarks>
		/// <see cref="DisplayElement"/> requires a string, thus using <c>string</c> instead of <c>char</c> for replacement character.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateReplacementCharacterAndCompactInvalidBytesWarning(byte[] a, DateTime ts, IODirection dir, Encoding e, string replacementCharacter)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(replacementCharacter);
			sb.Append("["); // Manually adding [...] to be able to combine replacement character and byte sequence.
			sb.Append(bytesAsString);
			sb.Append("]");

			return (new DisplayElement.WarningInfo(ts, (Direction)dir, a, sb.ToString(), omitBracketsAndLabel: true));
		}

		/// <remarks>
		/// <see cref="DisplayElement"/> requires a string, thus using <c>string</c> instead of <c>char</c> for replacement character.
		/// </remarks>
		/// <remarks>
		/// Using <see cref="DisplayElement.WarningInfo"/> rather than <see cref="DisplayElement.TxData"/> or <see cref="DisplayElement.RxData"/> to enable coloring.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateReplacementCharacterElement(byte b, DateTime ts, IODirection dir, string replacementCharacter)
		{
			return (new DisplayElement.WarningInfo(ts, (Direction)dir, b, replacementCharacter, omitBracketsAndLabel: true));
		}

		/// <remarks>
		/// <see cref="DisplayElement"/> requires a string, thus using <c>string</c> instead of <c>char</c> for replacement character.
		/// </remarks>
		/// <remarks>
		/// Using <see cref="DisplayElement.WarningInfo"/> rather than <see cref="DisplayElement.TxData"/> or <see cref="DisplayElement.RxData"/> to enable coloring.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateReplacementCharacterElement(byte[] a, DateTime ts, IODirection dir, string replacementCharacter)
		{
			return (new DisplayElement.WarningInfo(ts, (Direction)dir, a, replacementCharacter, omitBracketsAndLabel: true));
		}

		/// <remarks>
		/// <see cref="DisplayElement"/> requires a string, thus using <c>string</c> instead of <c>char</c> for replacement character.
		/// </remarks>
		/// <remarks>
		/// Using <see cref="DisplayElement.WarningInfo"/> rather than <see cref="DisplayElement.TxData"/> or <see cref="DisplayElement.RxData"/> to enable coloring.
		/// Using <see cref="DisplayElement.WarningInfo"/> rather than <see cref="DisplayElement.Nonentity"/> to keep correct byte count.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDiscardElement(byte b, DateTime ts, IODirection dir)
		{
			return (new DisplayElement.WarningInfo(ts, (Direction)dir, b, "", omitBracketsAndLabel: true));
		}

		/// <remarks>
		/// <see cref="DisplayElement"/> requires a string, thus using <c>string</c> instead of <c>char</c> for replacement character.
		/// </remarks>
		/// <remarks>
		/// Using <see cref="DisplayElement.WarningInfo"/> rather than <see cref="DisplayElement.TxData"/> or <see cref="DisplayElement.RxData"/> to enable coloring.
		/// Using <see cref="DisplayElement.WarningInfo"/> rather than <see cref="DisplayElement.Nonentity"/> to keep correct byte count.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDiscardElement(byte[] a, DateTime ts, IODirection dir)
		{
			return (new DisplayElement.WarningInfo(ts, (Direction)dir, a, "", omitBracketsAndLabel: true));
		}

		/// <remarks>This text specific implementation is based on <see cref="DisplayElementCollection.CharCount"/>.</remarks>
		protected override void AddContentSeparatorAsNeeded(LineState lineState, IODirection dir, DisplayElementCollection lp, DisplayElement de)
		{
			if (RadixUsesContentSeparator(dir) && !string.IsNullOrEmpty(TerminalSettings.Display.ContentSeparatorCache) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.CharCount > 0) || (lp.CharCount > 0))
					lp.Add(new DisplayElement.ContentSeparator((Direction)dir, TerminalSettings.Display.ContentSeparatorCache));
			}
		}

		#endregion

		#region Process Elements
		//------------------------------------------------------------------------------------------
		// Process Elements
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// <c>private</c> rather than <c>protected override</c> because method depends on code
		/// sequence in constructors.
		/// </remarks>
		private void InitializeProcess()
		{
			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, Parser.Mode.RadixAndAsciiEscapes))
			{
				// Tx states:
				{
					byte[] eol;
					if (!p.TryParse(TextTerminalSettings.TxEol, out eol))
					{
						// In case of an invalid EOL sequence, default it. This should never happen,
						// YAT verifies the EOL sequence when the user enters it. However, the user might
						// manually edit the EOL sequence in a settings file.
						TextTerminalSettings.TxEol = Settings.TextTerminalSettings.EolDefault;
						eol = p.Parse(TextTerminalSettings.TxEol);
					}

					this.textTxState      = new TextUnidirState(eol);
					this.textBidirTxState = new TextUnidirState(eol);
				}

				// Rx states:
				{
					byte[] eol;
					if (!p.TryParse(TextTerminalSettings.RxEol, out eol))
					{
						// In case of an invalid EOL sequence, default it. This should never happen,
						// YAT verifies the EOL sequence when the user enters it. However, the user might
						// manually edit the EOL sequence in a settings file.
						TextTerminalSettings.RxEol = Settings.TextTerminalSettings.EolDefault;
						eol = p.Parse(TextTerminalSettings.RxEol);
					}

					this.textBidirRxState = new TextUnidirState(eol);
					this.textRxState      = new TextUnidirState(eol);
				}
			}

			// Bidir state:
			this.lineSendDelayState = new LineSendDelayState();

			// Timer dependents:
			InitializeGlueCharsOfLineTimeoutIfNeeded();
			InitializeWaitForResponseIfNeeded();
		}

		/// <summary>
		/// Disposes the processing state.
		/// </summary>
		protected override void DisposeProcess()
		{
			// Text unspecifics:
			base.DisposeProcess();

			// Text specifics:
			if (this.waitForResponseEvent != null) {
				this.waitForResponseEvent.Dispose();
				this.waitForResponseEvent = null;
			}

			DisposeGlueCharsOfLineTimeoutIfNeeded();
		////DisposeWaitForResponseIfNeeded() is not needed yet.
		}

		/// <summary>
		/// Resets the processing state for the given <paramref name="repositoryType"/>.
		/// </summary>
		protected override void ResetProcess(RepositoryType repositoryType)
		{
			// Text unspecifics:
			base.ResetProcess(repositoryType);

			// Text specifics:
			switch (repositoryType)
			{
				case RepositoryType.Tx:    this.textTxState     .Reset();                                break;
				case RepositoryType.Bidir: this.textBidirTxState.Reset(); this.textBidirRxState.Reset(); break;
				case RepositoryType.Rx:                                   this.textRxState     .Reset(); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!"               + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			ResetGlueCharsOfLineTimeoutIfNeeded(repositoryType);
		////ResetWaitForResponse() must not be called when resetting processing, as WaitForResponse also applies to sending.
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="textTxState"/>, <see cref="textRxState"/>,
		/// <see cref="textBidirTxState"/>, <see cref="textBidirRxState"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unidir", Justification = "Orthogonality with 'Bidir'.")]
		protected TextUnidirState GetTextUnidirState(RepositoryType repositoryType, IODirection dir)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.textTxState);
				case RepositoryType.Rx:    return (this.textRxState);

				case RepositoryType.Bidir: if (dir == IODirection.Tx) { return (this.textBidirTxState); }
				                           else                       { return (this.textBidirRxState); }
				                           //// Invalid directions are asserted elsewhere.

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!"               + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected override void SuspendChunkTimeouts(RepositoryType repositoryType, IODirection dir)
		{
			base.SuspendChunkTimeouts(repositoryType, dir);

			if (!IsReloading) // See comments further below.
				SuspendGlueCharsOfLineTimeoutIfNeeded(repositoryType, dir);
		}

		/// <summary></summary>
		protected override void ResumeChunkTimeouts(RepositoryType repositoryType, IODirection dir)
		{
			base.ResumeChunkTimeouts(repositoryType, dir);

			if (!IsReloading) // See comments further below.
				ResumeGlueCharsOfLineTimeoutIfNeeded(repositoryType, dir);
		}

		/// <summary></summary>
		protected override void ProcessChunkPre(RepositoryType repositoryType, RawChunk chunk)
		{
			base.ProcessChunkPre(repositoryType, chunk);

			// Glue time-outs are processed asynchronously, as they are only triggered
			// after a time-out. Except on reload, then glue time-outs are calculated.
			// Note that all bytes of a chunk have the same time stamp, thus no need to
			// check for time-out on each byte.

			if (IsReloading)
				ProcessAndSignalGlueCharsOfLineTimeoutOfTimedOutPostponedChunksOnReloadIfNeeded(chunk.TimeStamp);
		}

		/// <summary>
		/// Implements the text terminal specific <see cref="Settings.TextTerminalSettings.GlueCharsOfLine"/> functionality.
		/// </summary>
		protected override void ProcessChunk(RepositoryType repositoryType, RawChunk chunk, out PostponeResult postponeResult)
		{
			ProcessChunkPre(repositoryType, chunk, out postponeResult);
			if (postponeResult != PostponeResult.Nothing)
				return;
			else
				base.ProcessChunk(repositoryType, chunk, out postponeResult);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "'out' is preferred over return value in this particular case.")]
		protected virtual void ProcessChunkPre(RepositoryType repositoryType, RawChunk chunk, out PostponeResult postponeResult)
		{
			if (TextTerminalSettings.GlueCharsOfLine.Enabled)
			{
				if (repositoryType == RepositoryType.Bidir) // Glueing only applies to bidirectional processing.
				{
					var lineState = GetLineState(repositoryType);
					if (lineState.Position != LinePosition.Begin) // Postponing is only needed when already within a line.
					{
						var overallState = GetOverallState(repositoryType);
						var deviceOrDirectionHasChanged = false;

						var isServerSocket = TerminalSettings.IO.IOTypeIsServerSocket;
						if (isServerSocket && TerminalSettings.Display.DeviceLineBreakEnabled) // Attention: This "isServerSocket" restriction is also implemented at other locations!
						{
							if (!overallState.DeviceLineBreak.IsFirstChunk)
							{
								if (DeviceHasChanged(chunk.Device, overallState.DeviceLineBreak.Device))
									deviceOrDirectionHasChanged = true;
							}
						}

						if (TerminalSettings.Display.DirectionLineBreakEnabled)
						{
							if (!overallState.DirectionLineBreak.IsFirstChunk)
							{
								if (DirectionHasChanged(chunk.Direction, overallState.DirectionLineBreak.Direction))
									deviceOrDirectionHasChanged = true;
							}
						}

						if (deviceOrDirectionHasChanged)
						{
							var postponeLineBreak = !GlueCharsOfLineTimeoutHasElapsed(chunk.TimeStamp, lineState.TimeStamp);
							if (postponeLineBreak)
							{
								DebugGlueCharsOfLine(string.Format(CultureInfo.CurrentCulture, "Glueing determined to postpone whole {0} chunk of {1} byte(s) stamped {2:HH:mm:ss.fff} instead of performing device or direction line break.", chunk.Direction, chunk.Content.Count, chunk.TimeStamp));

								PostponeChunk(repositoryType, chunk);
								postponeResult = PostponeResult.CompleteChunk;
								return;
							}
						}
					} // if (position != Begin)
				} // if (bidirIsAffected)
			} // if (GlueCharsOfLine.Enabled)

			postponeResult = PostponeResult.Nothing;
		}

		/// <summary></summary>
		protected virtual bool GlueCharsOfLineTimeoutHasElapsed(DateTime instantInQuestion, DateTime startOfTimeout)
		{
			if (TextTerminalSettings.GlueCharsOfLine.Timeout != Timeout.Infinite)
			{
				var duration = (instantInQuestion - startOfTimeout).TotalMilliseconds;
				return (duration >= TextTerminalSettings.GlueCharsOfLine.Timeout);
			}
			else // Infinite:
			{
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Temporary WITH_SCRIPTING.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected override void ProcessByteOfChunk(RepositoryType repositoryType,
		                                           byte b, DateTime ts, string dev, IODirection dir,
		                                           bool isFirstByteOfChunk, bool isLastByteOfChunk,
		                                           ref DisplayElementCollection elementsToAdd, ref DisplayLineCollection linesToAdd,
	#if (WITH_SCRIPTING)
		                                           ref ScriptLineCollection receivedScriptLinesToAdd,
	#endif
		                                           out bool breakChunk)
		{
			var processState = GetProcessState(repositoryType);
			var lineState = processState.Line; // Convenience shortcut.

		#if (WITH_SCRIPTING)
			var linePositionEndAppliesToScriptLines = false;
		#endif

			// The first byte of a line will sequentially trigger the [Begin] as well as [Content]
			// condition below. In the normal case, the line will then contain the first displayed
			// element. However, when initially receiving a hidden e.g. <XOn>, the line will yet be
			// empty. Then, when subsequent bytes are received, even when seconds later, the line's
			// initial time stamp is kept. This is illogical, the time stamp of a hidden <XOn> shall
			// not define the time stamp of the line, thus handle such case by rebeginning the line.
			if (lineState.Position == LinePosition.Content)
				DoLineContentCheck(repositoryType, processState, ts, dir);

			if (lineState.Position == LinePosition.Begin)
				DoLineBegin(repositoryType, processState, ts, dev, dir, ref elementsToAdd);

			if (lineState.Position == LinePosition.Content)
			#if (WITH_SCRIPTING)
				DoLineContent(repositoryType, processState, b, ts, dev, dir, ref elementsToAdd, out linePositionEndAppliesToScriptLines);
			#else
				DoLineContent(repositoryType, processState, b, ts, dev, dir, ref elementsToAdd);
			#endif

			if (lineState.Position == LinePosition.End)                                             // Implicitly means 'AppliesToScriptingIfFramed' since flag
			{                                                                                       // will only be set when EOL (and BOL) is active and complete.
			#if (WITH_SCRIPTING)
				DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd, linePositionEndAppliesToScriptLines, ref receivedScriptLinesToAdd);
			#else
				DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd);
			#endif

				DoLineEndPost(repositoryType, processState, ts, dir, isLastByteOfChunk, out breakChunk);

				// When glueing is enabled, potentially postpone remaining byte(s) of chunk until
				// previously postponed chunk(s) of other direction has been processed.
			}
			else
			{
				breakChunk = false;
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Line breaks like length based "word wrap" only apply to scripting if the message is not framed, i.e.:
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </summary>
		protected override bool IsNotFramedAndThusAppliesToScriptLines
		{
			get
			{           // "ScriptLines" only apply to Rx.
				return (RxEolSequence.Length == 0); // Same result as (TextTerminalSettings.RxEol == ((EolEx)Eol.None).ToSequenceString());
			}
		}

	#endif

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "'out' is preferred over return value in this particular case.")]
		protected virtual void DoLineEndPost(RepositoryType repositoryType, ProcessState processState, DateTime ts, IODirection dir, bool isLastByteOfChunk, out bool breakChunk)
		{
			if (TextTerminalSettings.GlueCharsOfLine.Enabled)
			{
				if (repositoryType == RepositoryType.Bidir) // Glueing only applies to bidirectional processing.
				{
					if (!isLastByteOfChunk) // No need to evaluate break for last byte (i.e. nothing to break).
					{
						var overallState = processState.Overall; // Convenience shortcut.

						var otherDir = GetOtherDirection(dir);
						var postponedChunkCountOfOtherDir = overallState.GetPostponedChunkCount(otherDir);
						if (postponedChunkCountOfOtherDir > 0)
						{
							var firstPostponedChunkTimeStampOfOtherDir = overallState.GetFirstPostponedChunkTimeStamp(otherDir);
							if (firstPostponedChunkTimeStampOfOtherDir < ts)
							{
								DebugGlueCharsOfLine(string.Format(CultureInfo.CurrentCulture, "Glueing determined to break {0} chunk stamped {1:HH:mm:ss.fff} and postpone remaining bytes because first postponed {2} chunk is stamped {3:HH:mm:ss.fff}.", dir, ts, otherDir, firstPostponedChunkTimeStampOfOtherDir));

								breakChunk = true;
								return;
							}
						}
					}
				}
			}

			breakChunk = false;
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected virtual void DoLineContentCheck(RepositoryType repositoryType, ProcessState processState,
		                                          DateTime ts, IODirection dir)
		{
			var lineState = processState.Line; // Convenience shortcut.

			var textUnidirState = GetTextUnidirState(repositoryType, dir);

			var isYetEmpty = (lineState.IsYetEmpty && textUnidirState.IsYetEmpty);
			if (isYetEmpty)
			{
				var left  = TerminalSettings.Display.InfoEnclosureLeftCache;
				var right = TerminalSettings.Display.InfoEnclosureRightCache;

				var doReplace = false;

				if (TerminalSettings.Display.ShowTimeStamp) { lineState.Elements.ReplaceTimeStamp(ts,                                              TerminalSettings.Display.TimeStampFormat, TerminalSettings.Display.TimeStampUseUtc, left, right); doReplace = true; }
				if (TerminalSettings.Display.ShowTimeSpan)  { lineState.Elements.ReplaceTimeSpan( ts - TimeSpanBase,                               TerminalSettings.Display.TimeSpanFormat,                                            left, right); doReplace = true; }
				if (TerminalSettings.Display.ShowTimeDelta) { lineState.Elements.ReplaceTimeDelta(ts - processState.Overall.PreviousLineTimeStamp, TerminalSettings.Display.TimeDeltaFormat,                                           left, right); doReplace = true; }

				if (doReplace)
					FlushReplaceAlreadyBeganLine(repositoryType, lineState);
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected override void DoLineBegin(RepositoryType repositoryType, ProcessState processState,
		                                    DateTime ts, string dev, IODirection dir,
		                                    ref DisplayElementCollection elementsToAdd)
		{
			base.DoLineBegin(repositoryType, processState, ts, dev, dir, ref elementsToAdd);

			var lineState = processState.Line; // Convenience shortcut.
			var lp = new DisplayElementCollection(); // No preset needed, default behavior is good enough.

			lp.Add(new DisplayElement.LineStart());

			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowDevice    ||
			    TerminalSettings.Display.ShowDirection)
			{
				DisplayElementCollection info;
				PrepareLineBeginInfo(ts, (ts - TimeSpanBase), (ts - processState.Overall.PreviousLineTimeStamp), dev, dir, out info);
				lp.AddRange(info);
			}

			lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.

			CreateCollectionIfIsNull(ref elementsToAdd);
			elementsToAdd.AddRange(lp);
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Temporary WITH_SCRIPTING.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void DoLineContent(RepositoryType repositoryType, ProcessState processState,
		                           byte b, DateTime ts, string dev, IODirection dir,
	#if (WITH_SCRIPTING)
		                           ref DisplayElementCollection elementsToAdd,
		                           out bool linePositionEndAppliesToScriptLines)
	#else
		                           ref DisplayElementCollection elementsToAdd)
	#endif
		{
		#if (WITH_SCRIPTING)
			linePositionEndAppliesToScriptLines = false;
		#endif

			var lineState   = processState.Line;   // Convenience shortcut.
		#if (WITH_SCRIPTING)
			var scriptState = processState.Script; // Convenience shortcut.
		#endif

			var textUnidirState     = GetTextUnidirState(repositoryType, dir);
			var textDisplaySettings = GetTextDisplaySettings(dir);

			// Convert content:
			DisplayElement de;
			bool isBackspaceToExecute;
			if (!ControlCharacterHasBeenProcessed(b, ts, dir, out de, out isBackspaceToExecute))
				de = ByteToElement(b, ts, dir, textUnidirState.PendingMultiBytesToDecode); // Default conversion to value or ASCII mnemonic.

			var lp = new DisplayElementCollection(); // No preset needed, default behavior is good enough.

			// Prepare EOL:
			if (!textUnidirState.EolOfGivenDevice.ContainsKey(dev))                                        // It is OK to only access or add to the collection,
				textUnidirState.EolOfGivenDevice.Add(dev, new SequenceQueue(textUnidirState.EolSequence)); // this will not lead to excessive use of memory,
			                                                                                               // since there is only a given number of devices.
			// Add byte to EOL:                                                                            // Applies to TCP and UDP server terminals only.
			textUnidirState.EolOfGivenDevice[dev].Enqueue(b);

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started:
			if (textUnidirState.EolOfGivenDevice[dev].IsCompleteMatch)
			{
				if (de.IsContent)
				{
					if (TextTerminalSettings.ShowEol)
					{
						AddContentSeparatorAsNeeded(lineState, dir, lp, de);
						lp.Add(de); // No clone needed as element is no more used below.
					}
					else
					{
					////lineState.RetainedPotentialEolElements.Add(de); // Adding is useless, Confirm...() below will clear the elements anyway.

						ConfirmRetainedUnconfirmedHiddenEolElements(textUnidirState);
					}

					lineState.Position = LinePosition.End;
				#if (WITH_SCRIPTING)
					linePositionEndAppliesToScriptLines = true;
				#endif

					// This is the only location where the true EOL of a sent or received line is detected.
					// Other locations where 'Position.End' is involved more deal with YAT's display/monitor
					// line breaks, i.e. the other ways to trigger a line break (chunk, length, timed).

					if ((TextTerminalSettings.WaitForResponse.Enabled) && (repositoryType == RepositoryType.Rx))
					{                                                  // Rather than (dir == IODirection.Rx) which
						NotifyLineResponse();                          // would cover two repositories (Rx and Bidir).
					}
				}
				else
				{
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
				}
			}                                                                      // Note the inverted implementation sequence:
			else if (textUnidirState.EolOfGivenDevice[dev].IsPartlyMatchContinued) //  1. CompleteMatch        (last trigger, above)
			{                                                                      //  2. PartlyMatchContinued (intermediate, here)
				if (de.IsContent)                                                  //  3. PartlyMatchBeginning (first trigger, below)
				{                                                                  //  4. Unrelatd to EOL      (any time, further below)
					if (TextTerminalSettings.ShowEol)
					{
						AddContentSeparatorAsNeeded(lineState, dir, lp, de);
						lp.Add(de); // No clone needed as element is no more used below.
					}
					else
					{
						textUnidirState.RetainedUnconfirmedHiddenEolElements.Add(de); // No clone needed as element is no more used below.
					}
				}
				else
				{
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
				}
			}
			else if (textUnidirState.EolOfGivenDevice[dev].IsPartlyMatchBeginning)
			{
				// Previous was no match, retained potential EOL elements can be treated as non-EOL:
				ReleaseRetainedUnconfirmedHiddenEolElements(lineState, textUnidirState, dir, lp);

				if (de.IsContent)
				{
					if (TextTerminalSettings.ShowEol)
					{
						AddContentSeparatorAsNeeded(lineState, dir, lp, de);
						lp.Add(de); // No clone needed as element is no more used below.
					}
					else
					{
						textUnidirState.RetainedUnconfirmedHiddenEolElements.Add(de); // No clone needed as element is no more used below.

						// Potential but not yet confirmed EOL elements shall be retained until EOL
						// is either confirmed or dismissed, in order to...
						//
						// ...prevent flickering like:
						//  1. ABC
						//  2. ABC<CR>
						//  3. ABC
						//
						// ...significantly simplify the ExecuteLineEnd() implementation since it
						// is no longer needed to remove EOL from elements as it was the case until
						// SVN revision #2052 of this file.
						//
						// Nice-to-have refinement of this behavior:
						// Retained EOL elements shall be shown after a time-out of e.g. 150 ms => FR #364.
					}
				}
				else
				{
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
				}
			}
			else
			{
				// No match at all, retained potential EOL elements can be treated as non-EOL:
				ReleaseRetainedUnconfirmedHiddenEolElements(lineState, textUnidirState, dir, lp);

				// Add the current element, which for sure is not related to EOL:
				AddContentSeparatorAsNeeded(lineState, dir, lp, de);
				lp.Add(de); // No clone needed as element has just been created further above.
			}

			if (!lineState.Exceeded)
			{
				if (isBackspaceToExecute)
				{
					// Note that backspace must be executed after EOL since...
					// ...unconfirmed hidden EOL elements may have to be released.
					// ...EOL could contain backspace, unlikely but possible.

					// Remove the just added backspace:
					int count = lp.Count;
					if ((count > 0) && (lp[count - 1] is DisplayElement.Nonentity))
					{
						lp.RemoveLast();
						RemoveContentSeparatorAsNeeded(dir, lp);
					}
				}

				var totalCharCount = (lineState.Elements.CharCount + lp.CharCount);
				if (totalCharCount <= TerminalSettings.Display.MaxLineLength)
				{
					textUnidirState.NotifyShownCharCount(lp.CharCount);
					lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.

					CreateCollectionIfIsNull(ref elementsToAdd);
					elementsToAdd.AddRange(lp);
				}
				else
				{
					lineState.Exceeded = true; // Keep in mind and notify once:

					var message = "Maximal number of characters per line exceeded! Check the line break settings in Terminal > Settings > Text or increase the limit in Terminal > Settings > Advanced.";
					lineState.Elements.Add(new DisplayElement.WarningInfo(ts, (Direction)dir, message)); // Create two separate elements...

					CreateCollectionIfIsNull(ref elementsToAdd);
					elementsToAdd.Add(     new DisplayElement.WarningInfo(ts, (Direction)dir, message)); // ...to ensure decoupling.
				}

				if (isBackspaceToExecute)
				{
					// Note that backspace must be executed after adding above since...
					// ...character to be removed likely had already been contained before adding above.

					// If the current line does contain a preceeding "true" character...
					if (lineState.Elements.DataContentCharCount > 0)
					{
						// ...remove it in the current line...
						lineState.Elements.RemoveLastDataContentChar();
						RemoveContentSeparatorAsNeeded(dir, lineState.Elements);

						if (!ICollectionEx.IsNullOrEmpty(elementsToAdd))
						{
							if (elementsToAdd.DataContentCharCount > 0)
							{
								// ..as well as in the pending elements:
								elementsToAdd.RemoveLastDataContentChar();
								RemoveContentSeparatorAsNeeded(dir, elementsToAdd);
							}
							else
							{
								elementsToAdd.Clear(); // Whole line will be replaced, pending elements can be discarded.
								FlushReplaceAlreadyBeganLine(repositoryType, lineState);
							}
						}
						else
						{
							FlushReplaceAlreadyBeganLine(repositoryType, lineState);
						}

						// Don't forget to adjust state:
						textUnidirState.NotifyShownCharCount(-1);
					}
				}
			}

		#if (WITH_SCRIPTING)
			// Apply to scripting:
			if (!IsReloading && ScriptRunIsActive)
			{
				if (!IsByteToHide(b))        // Note this must not cover show/hide EOL/BOL, as script messages shall never include EOL.
					scriptState.Data.Add(b); // The EOL/BOL sequences are removed when processing script line/packet into script message.
			}
		#endif

			// Only continue evaluation if no line break detected yet (cannot have more than one line break):
			if ((lineState.Position != LinePosition.End) && (textDisplaySettings.LengthLineBreak.Enabled))
			{
				if (lineState.Elements.CharCount >= textDisplaySettings.LengthLineBreak.Length)
				{
					lineState.Position = LinePosition.End;
				#if (WITH_SCRIPTING)
					linePositionEndAppliesToScriptLines = IsNotFramedAndThusAppliesToScriptLines; // Length line breaks, i.e. "word wrap", shall not effect scripting. If ever needed, an [advanced configuration of scripting behavior] shall be added.
				#endif
				}

				// Note that length line break shall be applied even when EOL has just started or is already ongoing,
				// remaining hidden EOL elements will not result in additional lines.
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual bool ControlCharacterHasBeenProcessed(byte b, DateTime ts, IODirection dir, out DisplayElement de, out bool isBackspaceToExecute)
		{
			isBackspaceToExecute = false;

			switch (b)
			{
				case 0x07: // <BEL> (bell/beep)
				{
					if (TerminalSettings.CharAction.BeepOnBell)
					{
						if (!IsReloading)
							SystemSounds.Beep.Play();

						de = null;
						return (false); // Signal that character shall "normally" be formatted, independent on beeping or not.
					}

					break;
				}

				case 0x08: // <BS> (backspace)
				{
					if (RadixIsStringOrChar(dir)) // Attention: This logic is also implemented in the text settings!
					{
						bool replace = (TerminalSettings.CharReplace.ReplaceControlChars && TerminalSettings.CharReplace.ReplaceBackspace);
						if (!replace)
						{
							isBackspaceToExecute = true;

							de = new DisplayElement.Nonentity();
							return (true);
						}
					}

					break;
				}

				case 0x09: // <TAB> (tabulator)
				{
					if (RadixIsStringOrChar(dir)) // Attention: This logic is also implemented in the text settings!
					{
						bool replace = (TerminalSettings.CharReplace.ReplaceControlChars && TerminalSettings.CharReplace.ReplaceTab);
						if (!replace)
						{
							// Attention:
							// In order to get well aligned tab stops, tab characters must be data elements.
							// If they were control elements (i.e. sequence of data and control elements),
							// tabs would only get aligned within the respective control element,
							// thus resulting in misaligned tab stops.

							de = CreateDataElement(b, ts, dir, "\t");
							return (true);
						}
					}

					break;
				}
			}

			de = null;
			return (false);
		}

		/// <summary>
		/// Confirms the retained unconfirmed hidden EOL elements by discarding them.
		/// </summary>
		private static void ConfirmRetainedUnconfirmedHiddenEolElements(TextUnidirState textUnidirState)
		{
			if (textUnidirState.RetainedUnconfirmedHiddenEolElements.Count > 0)
			{
				textUnidirState.RetainedUnconfirmedHiddenEolElements.Clear();
			}
		}

		/// <summary>
		/// Releases the retained unconfirmed hidden EOL elements by adding them to <paramref name="lp"/>.
		/// </summary>
		private void ReleaseRetainedUnconfirmedHiddenEolElements(LineState lineState, TextUnidirState textUnidirState, IODirection dir, DisplayElementCollection lp)
		{
			if (textUnidirState.RetainedUnconfirmedHiddenEolElements.Count > 0)
			{
				foreach (var de in textUnidirState.RetainedUnconfirmedHiddenEolElements)
				{
					AddContentSeparatorAsNeeded(lineState, dir, lp, de);
					lp.Add(de); // No clone needed as element is no more used below.
				}

				textUnidirState.RetainedUnconfirmedHiddenEolElements.Clear();
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Temporary WITH_SCRIPTING.")]
		protected override void DoLineEnd(RepositoryType repositoryType, ProcessState processState,
		                                  DateTime ts, IODirection dir,
	#if (WITH_SCRIPTING)
		                                  ref DisplayElementCollection elementsToAdd, ref DisplayLineCollection linesToAdd,
		                                  bool appliesToScriptLines, ref ScriptLineCollection receivedScriptLinesToAdd)
	#else
		                                  ref DisplayElementCollection elementsToAdd, ref DisplayLineCollection linesToAdd)
	#endif
		{
			var lineState = processState.Line; // Convenience shortcut.

			var textUnidirState = GetTextUnidirState(repositoryType, lineState.Direction);
			var dev = lineState.Device;

			// Note that, in case of e.g. a timed line break, retained potential EOL elements will be handled by DoLineContent().
			// This is needed because potential EOL elements are potentially hidden. Opposed to binary terminals, where all bytes are shown.
			                                                              // This count corresponds to the current line. Non-SBCS characters are only counted if complete.
			var isEmptyLine                        = (lineState.Elements.CharCount == 0);
			var isEmptyLineWithHiddenNonEol        = (isEmptyLine && !textUnidirState.EolIsAnyMatch(dev));
			var isEmptyLineWithPendingEol          = (isEmptyLine &&  textUnidirState.EolIsAnyMatch(dev));                                       // No empty line formerly shown.
			var isEmptyLineWithPendingEolToBeShown = (isEmptyLine &&  textUnidirState.EolIsCompleteMatch(dev) && (textUnidirState.ShownCharCount == 0));

			if (isEmptyLineWithHiddenNonEol) // While intended empty lines must be shown, potentially suppress
			{                                // empty lines that only contain hidden non-EOL character(s) (e.g. hidden 0x00):
				if (!ICollectionEx.IsNullOrEmpty(elementsToAdd))
					elementsToAdd.RemoveLastUntil(typeof(DisplayElement.LineStart));                 // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                                   ////            All other elements must be removed as well!
				FlushClearAlreadyBeganLine(repositoryType, processState, elementsToAdd, linesToAdd); //            This is ensured by flushing here.
			}
			else if (isEmptyLineWithPendingEol && !isEmptyLineWithPendingEolToBeShown) // While intended empty lines must be shown, potentially suppress
			{                                                                          // empty lines that only contain hidden pending EOL character(s):
				if (!ICollectionEx.IsNullOrEmpty(elementsToAdd))
					elementsToAdd.RemoveLastUntil(typeof(DisplayElement.LineStart));                 // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                                   ////            All other elements must be removed as well!
				FlushClearAlreadyBeganLine(repositoryType, processState, elementsToAdd, linesToAdd); //            This is ensured by flushing here.
			}
			else // Neither empty nor need to suppress:
			{
				// Process line length/duration:
				var lineEnd = new DisplayElementCollection(); // No preset needed, default behavior is good enough.
				if (TerminalSettings.Display.ShowLength || TerminalSettings.Display.ShowDuration) // Meaning: "byte count"/"char count" and "line duration".
				{
					int length;
					if (TerminalSettings.Display.LengthSelection == LengthSelection.CharCount)
						length = lineState.Elements.CharCount;
					else        // incl. Display.LengthSelection == LengthSelection.ByteCount)
						length = lineState.Elements.ByteCount;

					var duration = (ts - lineState.TimeStamp);

					DisplayElementCollection info;
					PrepareLineEndInfo(length, duration, out info);
					lineEnd.AddRange(info);
				}

				lineEnd.Add(new DisplayElement.LineBreak());

				CreateCollectionIfIsNull(ref elementsToAdd);
				elementsToAdd.AddRange(lineEnd.Clone()); // Clone elements because they are needed again right below.

				// Finalize line:
				var l = new DisplayLine(lineState.Elements.Count + lineEnd.Count); // Preset the required capacity to improve memory management.
				l.AddRange(lineState.Elements.Clone()); // Clone to ensure decoupling.
				l.AddRange(lineEnd);

				if (lineState.Highlight)
					l.Highlight = true;

				CreateCollectionIfIsNull(ref linesToAdd);
				linesToAdd.Add(l);

			#if (WITH_SCRIPTING)
				// Apply to scripting:                                                     // "ScriptLines" only apply to Rx.
				if (!IsReloading && ScriptRunIsActive && (repositoryType == RepositoryType.Rx))
				{
					if (appliesToScriptLines)
					{
						var scriptState = processState.Script; // Convenience shortcut.
						var duration = (ts - scriptState.TimeStamp); // Attention, the script state's time stamp must be taken! It may differ from the displayed time stamp!

						CreateCollectionIfIsNull(ref receivedScriptLinesToAdd);                           // No clone needed on ToArray().
						receivedScriptLinesToAdd.Add(new ScriptLine(scriptState.TimeStamp, scriptState.Device, scriptState.Data.ToArray(), duration));
					}
					else
					{
						// This display line end shall not result in a script line end.
					}
				}
			#endif
			}

			// Notify:
			textUnidirState.NotifyLineEnd(dev);
		#if (WITH_SCRIPTING)
			base.DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd, appliesToScriptLines, ref receivedScriptLinesToAdd);
		#else
			base.DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd);
		#endif
		}

		#endregion

		#region GlueCharsOfLine
		//------------------------------------------------------------------------------------------
		// GlueCharsOfLine
		//------------------------------------------------------------------------------------------

		private void InitializeGlueCharsOfLineTimeoutIfNeeded()
		{
			if (TextTerminalSettings.GlueCharsOfLine.Enabled)
			{
				if (this.glueCharsOfLineTimeout != null) // Must be given by this terminal.
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'DisposeGlueCharsOfLineTimeoutIfNeeded()' must be called first!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.glueCharsOfLineTimeout = new ProcessTimeout(TextTerminalSettings.GlueCharsOfLine.Timeout);
				this.glueCharsOfLineTimeout.Elapsed += glueCharsOfLineTimeout_Elapsed;
			}
		}

		private void DisposeGlueCharsOfLineTimeoutIfNeeded()
		{
			if (this.glueCharsOfLineTimeout != null)
			{
				this.glueCharsOfLineTimeout.Elapsed -= glueCharsOfLineTimeout_Elapsed;
				this.glueCharsOfLineTimeout.Dispose();
			}

			this.glueCharsOfLineTimeout = null;
		}

		private void ResetGlueCharsOfLineTimeoutIfNeeded(RepositoryType repositoryType)
		{                                         // Glueing only applies to bidirectional processing.
			if ((repositoryType == RepositoryType.Bidir) && TextTerminalSettings.GlueCharsOfLine.Enabled)
			{
				this.glueCharsOfLineTimeout.Stop(); // Same as Suspend() below.
			}
		}

		/// <remarks>
		/// Chunk and timed processing is synchronized against <see cref="Terminal.ChunkVsTimedSyncObj"/>.
		/// Thus, time-outs can be suspended during chunk processing.
		/// </remarks>
		protected virtual void SuspendGlueCharsOfLineTimeoutIfNeeded(RepositoryType repositoryType, IODirection dir)
		{                                         // Glueing only applies to bidirectional processing.
			if ((repositoryType == RepositoryType.Bidir) && TextTerminalSettings.GlueCharsOfLine.Enabled)
			{
				this.glueCharsOfLineTimeout.Stop(); // Same as Reset() above.
			}
		}

		/// <remarks>
		/// Chunk and timed processing is synchronized against <see cref="Terminal.ChunkVsTimedSyncObj"/>.
		/// Thus, time-outs can be suspended during chunk processing.
		/// </remarks>
		protected virtual void ResumeGlueCharsOfLineTimeoutIfNeeded(RepositoryType repositoryType, IODirection dir)
		{                                         // Glueing only applies to bidirectional processing.
			if ((repositoryType == RepositoryType.Bidir) && TextTerminalSettings.GlueCharsOfLine.Enabled)
			{
				var overallState = GetOverallState(repositoryType);
				var previousChunkTimeStamp = overallState.GetPreviousChunkTimeStamp();
				                                                         //// Single time-out for both directions.
				this.glueCharsOfLineTimeout.Start(previousChunkTimeStamp); // Time-out is not dependent on line position,
			}                                                              // rather on number of postponed chunks.
		}

		/// <remarks>
		/// This event handler must synchronize against <see cref="Terminal.ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void glueCharsOfLineTimeout_Elapsed(object sender, EventArgs<DateTime> e)
		{
			// No need to synchronize this event, the 'ProcessTimeout' always is single-shot.

			if (IsReloading) // See remarks at Terminal.RefreshRepositories() for reason.
			{
				DebugGlueCharsOfLine("glueCharsOfLineTimeout_Elapsed is being ignored while reloading");
				return;
			}

			DebugGlueCharsOfLine("glueCharsOfLineTimeout_Elapsed");

			lock (ChunkVsTimedSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				if (IsInDisposal) // Ensure to not handle async timer callback during closing anymore.
					return;

			////if (TextTerminalSettings.GlueCharsOfLine.Enabled) is implicitly given.
					ProcessAndSignalGlueOfCharsTimeoutOfTimedOutPostponedChunks(e.Value);
			}
		}

		/// <remarks>Rather long name, but makes obvious what is being done.</remarks>
		protected virtual void ProcessAndSignalGlueOfCharsTimeoutOfTimedOutPostponedChunks(DateTime instantInQuestion)
		{
			ProcessAndSignalGlueOfCharsTimeout(instantInQuestion);
		}

		/// <remarks>Rather long name, but makes obvious what is being done.</remarks>
		protected virtual void ProcessAndSignalGlueOfCharsTimeoutOfRemainingPostponedChunks()
		{
			ProcessAndSignalGlueOfCharsTimeout(DateTime.MaxValue);
		}

		/// <summary></summary>
		private void ProcessAndSignalGlueOfCharsTimeout(DateTime instantInQuestion)
		{
			// Possible states that lead here:
			//  > Async time-out timer callback, i.e. "normal" time-out during "normal" execution.
			//  > ChunkPre() on reload, i.e. time-out inbetween chunks on reload.
			//  > FinishReload(), i.e. remaining after last chunk on reload.

		////if (repositoryType == RepositoryType.Bidir) is implicitly given.
			{
				var repositoryType = RepositoryType.Bidir;
				var processState = GetProcessState(repositoryType);
				var overallState = processState.Overall; // Convenience shortcut.
				if (overallState.GetPostponedChunkCount() > 0)
				{
					var lineState = processState.Line; // Convenience shortcut.
					if (lineState.Position != LinePosition.Begin) // "Begin" also applies if the next line has not been started yet, i.e. "LinePosition.None".
					{
						var lineHasTimedOut = GlueCharsOfLineTimeoutHasElapsed(instantInQuestion, lineState.TimeStamp);
						var lineDir = lineState.Direction;
						var otherDir = GetOtherDirection(lineDir);
						if ((overallState.GetPostponedChunkCount(otherDir) > 0) && lineHasTimedOut)
						{
							var firstPostponedChunkTimeStampOfOtherDir = overallState.GetFirstPostponedChunkTimeStamp(otherDir);

							DebugGlueCharsOfLine(string.Format(CultureInfo.InvariantCulture, "Glueing determined to perform timed {0} line break at {1} and process postponed chunk(s) starting with {2} because line of {3} has timed out.", lineDir, firstPostponedChunkTimeStampOfOtherDir, otherDir, lineState.TimeStamp));

							// Line break must be enforced, without ProcessPostponedChunks() would again determine to postpone direction or device line break.
							// Note that accurate time stamp of first postponed chunk rather than inaccurate time stamp of elapsed event is used to calculate time-out.
							ProcessAndSignalTimedLineBreak(repositoryType, firstPostponedChunkTimeStampOfOtherDir, lineDir);

							ProcessPostponedChunks(repositoryType, otherDir);

							// In neither above described case it is possible to have postponed chunks in both directions,
							// thus a single forced line break is sufficient, i.e. it is not necessary to loop/recurse here.
						}
						else
						{
							// Only timed out cases shall be handled here, other cases are handled elsewhere.
						}
					}
					else // (Position == LinePosition.Begin) // "Begin" also applies if the next line has not been started yet, i.e. "LinePosition.None".
					{
						var firstPostponedChunk = overallState.GetFirstPostponedChunk();
						var chunkHasTimedOut = GlueCharsOfLineTimeoutHasElapsed(instantInQuestion, firstPostponedChunk.TimeStamp);
						if (chunkHasTimedOut)
						{
							var initialDir = firstPostponedChunk.Direction;

							DebugGlueCharsOfLine(string.Format(CultureInfo.InvariantCulture, "Glueing determined to process timed out postponed chunk(s) starting with {0} because chunk of {1} has timed out.", initialDir, firstPostponedChunk.TimeStamp));

							ProcessPostponedChunks(repositoryType, initialDir);
						}
						else
						{
							// Only timed out cases shall be handled here, other cases are handled elsewhere.
						}
					}

					// If no other line break option is active, the line position would be within a line in
					// all cases, and the direction would always have changed among the current line and the
					// postponed timed out or remaining chunk. However, as other line break option(s) may
					// may be active, also "interfering" cases are considered, thus the 'else' clause above.
				}
			}
		}

		/// <remarks>Rather long name, but makes obvious what is being done.</remarks>
		protected virtual void ProcessAndSignalGlueCharsOfLineTimeoutOfTimedOutPostponedChunksOnReloadIfNeeded(DateTime instantInQuestion)
		{
			if (TextTerminalSettings.GlueCharsOfLine.Enabled)
				ProcessAndSignalGlueOfCharsTimeoutOfTimedOutPostponedChunks(instantInQuestion);
		}

		/// <remarks>Rather long name, but makes obvious what is being done.</remarks>
		protected virtual void ProcessAndSignalGlueCharsOfLineTimeoutOfRemainingPostponedChunksOnReloadIfNeeded()
		{
			if (TextTerminalSettings.GlueCharsOfLine.Enabled)
				ProcessAndSignalGlueOfCharsTimeoutOfRemainingPostponedChunks();
		}

		#endregion

		#region WaitForResponse
		//------------------------------------------------------------------------------------------
		// WaitForResponse
		//------------------------------------------------------------------------------------------

		private void InitializeWaitForResponseIfNeeded()
		{
			if (TextTerminalSettings.WaitForResponse.Enabled)
				ResetWaitForResponse();
		}

		/// <remarks>
		/// Could be extracted into a separate class or struct.
		/// </remarks>
		protected virtual void ResetWaitForResponse()
		{
			lock (this.waitForResponseClearanceSyncObj)
			{
				this.waitForResponseResponseCounter = 0;
				this.waitForResponseClearanceCounter = TextTerminalSettings.WaitForResponse.ClearanceLineCount;
				this.waitForResponseClearanceTimeStamp = DateTime.MinValue;
			}
		}

		/// <remarks>
		/// Could be extracted into a separate class or struct.
		/// </remarks>
		protected virtual void NotifyLineResponse()
		{
			lock (this.waitForResponseClearanceSyncObj)
			{
				this.waitForResponseResponseCounter++;
				if (this.waitForResponseResponseCounter >= TextTerminalSettings.WaitForResponse.ResponseLineCount)
				{
					this.waitForResponseResponseCounter = 0;
					this.waitForResponseClearanceCounter += TextTerminalSettings.WaitForResponse.ClearanceLineCount;

					DebugWaitForResponse(string.Format("Line clearance increased to {0}", this.waitForResponseClearanceCounter));

					this.waitForResponseEvent.Set();
				}
			}
		}

		/// <remarks>
		/// Could be extracted into a separate class or struct.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		protected virtual bool GetLineClearance(ForSomeTimeEventHelper forSomeTimeEventHelper)
		{
			DebugWaitForResponse("Getting line clearance...");

			while (!DoBreak)
			{
				if (ClearanceHasBeenGranted())
					return (true);

				if (forSomeTimeEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold()) // Signal wait operation if needed.
					IncrementIsSendingForSomeTimeChanged();

				try
				{
					// WaitOne() would wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the state again.
					// The period can be quite long, as an event signal will immediately resume.
					// And there is no need to check the result, logic above will tell how to proceed.
					this.waitForResponseEvent.WaitOne(StaticRandom.Next(50, 200));
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in GetLineClearance()!");
					break;
				}
			}

			DebugWaitForResponse("GetLineClearance() has determined to break");

			ResetWaitForResponse(); // Required to reset clearance counter.

			return (false);
		}

		private bool ClearanceHasBeenGranted()
		{
			lock (this.waitForResponseClearanceSyncObj)
			{
				var now = DateTime.Now;
				if ((this.waitForResponseClearanceCounter > 0) || ClearanceTimeoutHasElapsed(now)) // Involved calculations only needed if counter is 0.
				{
					// Handle supernumerous responses, e.g. additional responses received without sending anything:
					if (this.waitForResponseClearanceCounter > TextTerminalSettings.WaitForResponse.ClearanceLineCount) {
						this.waitForResponseClearanceCounter = TextTerminalSettings.WaitForResponse.ClearanceLineCount;

						DebugWaitForResponse(string.Format("...adjusted and...", this.waitForResponseClearanceCounter));
					}

					if (this.waitForResponseClearanceCounter > 0) {
						this.waitForResponseClearanceCounter--;

						DebugWaitForResponse(string.Format("...granted, {0} lines left.", this.waitForResponseClearanceCounter));
					}
					else { // timeoutElapsed

						DebugWaitForResponse(              "...granted due to time-out.");
					}

					this.waitForResponseClearanceTimeStamp = now;
					this.waitForResponseEvent.Reset();

					return (true);
				}
				else
				{
					return (false);
				}
			}
		}

		/// <remarks>
		/// Separation into method instead of complicated <c>if</c>-condition.
		/// </remarks>
		/// <remarks>
		/// Must only be called from within a <see cref="waitForResponseClearanceSyncObj"/> lock.
		/// </remarks>
		private bool ClearanceTimeoutHasElapsed(DateTime instantInQuestion)
		{
			if (TextTerminalSettings.WaitForResponse.Timeout != Timeout.Infinite)
			{
				var duration = (instantInQuestion - this.waitForResponseClearanceTimeStamp).TotalMilliseconds;
				if (duration > TextTerminalSettings.WaitForResponse.Timeout)
					return (true);

				DebugWaitForResponse("...pending for counter to increase, or time-out to elapse.");
			}
			else // Infinite:
			{
				DebugWaitForResponse("...pending for counter to increase infinitely.");
			}

			return (false);
		}

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_GLUE_CHARS_OF_LINE")]
		private void DebugGlueCharsOfLine(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_WAIT_FOR_RESPONSE")]
		private void DebugWaitForResponse(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
