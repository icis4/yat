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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Media;
using System.Text;

using MKY;
using MKY.Text;

using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\TextTerminal for better separation of the implementation files.
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

		private List<byte> txPendingMultiBytesToDecode;
		private List<byte> rxPendingMultiBytesToDecode;

		private TextLineState txUnidirTextLineState;
		private TextLineState txBidirTextLineState;
		private TextLineState rxBidirTextLineState;
		private TextLineState rxUnidirTextLineState;

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
		protected override DisplayElement ByteToElement(byte b, DateTime ts, IODirection d, Radix r)
		{
			switch (r)
			{
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					return (base.ByteToElement(b, ts, d, r));
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

						if      ((b < 0x20) || (b == 0x7F))              // ASCII control characters.
						{
							return (base.ByteToElement(b, ts, d, r));
						}
						else if  (b == 0x20)                             // ASCII space.
						{
							return (base.ByteToElement(b, ts, d, r));
						}                                                // Special case.
						else if ((b == 0xFF) && TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
						{
							return (new DisplayElement.Nonentity());     // Return nothing, ignore the character, this results in hiding.
						}
						else                                             // ASCII and extended ASCII printable characters.
						{
							return (DecodeAndCreateElement(b, ts, d, r, e)); // 'IsSingleByte' always results in a single character per byte.
						}
					}
					else // 'IsMultiByte':
					{
						// \remind (2017-12-09 / MKY / bug #400)
						// YAT versions 1.99.70 and 1.99.80 used to take the endianness into account when encoding
						// and decoding multi-byte encoded characters. However, it was always done, but of course e.g.
						// UTF-8 is independent on endianness. The endianness would only have to be applied to single
						// multi-byte values, not multi-byte values split into multiple fragments. However, a .NET
						// 'Encoding' object does not tell whether the encoding is potentially endianness capable or
						// not. Thus, it was decided to again remove the character encoding endianness awareness.

						List<byte> pendingMultiBytesToDecode;
						switch (d)
						{
							case IODirection.Tx:    pendingMultiBytesToDecode = this.txPendingMultiBytesToDecode; break;
							case IODirection.Rx:    pendingMultiBytesToDecode = this.rxPendingMultiBytesToDecode; break;

							case IODirection.Bidir:
							case IODirection.None:  throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
						}

						if (((EncodingEx)e).IsUnicode)
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
										return (base.ByteToElement((byte)code, ts, d, r));
									}
									else if (code == 0x20)                           // ASCII space.
									{
										return (base.ByteToElement((byte)code, ts, d, r));
									}
									else                                             // ASCII printable character.
									{                                                            // 'effectiveCharCount' is 1 for sure.
										return (CreateDataElement(decodingArray, ts, d, r, chars[0]));
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

									return (CreateInvalidBytesWarning(decodingArray, ts, d, e));
								}
							}
							else // (effectiveCharCount > 1) => Code doesn't fit into a single u16 value, thus more than one character will be returned.
							{
								pendingMultiBytesToDecode.Clear();                   // Reset decoding stream.

								return (CreateOutsideUnicodePlane0Warning(decodingArray, ts, d, e));
							}
						}
						else if ((EncodingEx)e == SupportedEncoding.UTF7)
						{
							// Note that the following code is similar as above and below but with subtle differences
							// such as treatment of Base64 bytes, no treatment of 0xFF, no treatment of 0xFFFD, comment,...

							if (pendingMultiBytesToDecode.Count == 0)                // A first 'MultiByte' is either direct or lead byte.
							{
								if      ((b < 0x20) || (b == 0x7F))                  // ASCII control characters.
								{
									return (base.ByteToElement(b, ts, d, r));
								}
								else if (b == 0x20)                                  // ASCII space.
								{
									return (base.ByteToElement(b, ts, d, r));
								}
								else if (CharEx.IsValidForUTF7((char)b))
								{
									return (DecodeAndCreateElement(b, ts, d, r, e)); // 'IsMultiByte' but the current byte must result in a single character here.
								}
								else if (b == (byte)'+')                             // UTF-7 lead byte.
								{
									pendingMultiBytesToDecode.Clear();
									pendingMultiBytesToDecode.Add(b);

									return (new DisplayElement.Nonentity());         // Nothing to decode (yet).
								}
								else
								{
									return (CreateInvalidByteWarning(b, ts, d, e));
								}
							}
							else // (rxMultiByteDecodingStream.Count > 0) => Not lead byte.
							{
								if (b == (byte)'-')                                  // UTF-7 terminating byte.
								{
									pendingMultiBytesToDecode.Add(b);
									byte[] decodingArray = pendingMultiBytesToDecode.ToArray();
									pendingMultiBytesToDecode.Clear();

									int expectedCharCount = e.GetCharCount(decodingArray);
									char[] chars = new char[expectedCharCount];
									int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
									if (effectiveCharCount == expectedCharCount)
									{
										return (CreateDataElement(decodingArray, ts, d, r, chars));
									}
									else // Decoder has failed:
									{
										return (CreateInvalidBytesWarning(decodingArray, ts, d, e));
									}
								}
								else if (!CharEx.IsValidForBase64((char)b))          // Non-Base64 characters also terminates!
								{
									byte[] decodingArray = pendingMultiBytesToDecode.ToArray();
									pendingMultiBytesToDecode.Clear();

									int expectedCharCount = e.GetCharCount(decodingArray);
									char[] chars = new char[expectedCharCount];
									int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
									if (effectiveCharCount == expectedCharCount)
									{                                                                         // 'effectiveCharCount' is 1 for sure.
										DisplayElement encoded = CreateDataElement(decodingArray, ts, d, r, chars);

										// Note that the following code is similar as above and below but with subtle differences
										// such as treatment of a lead byte, no treatment of 0xFF, no treatment of 0xFFFD, comment,...

										DisplayElement direct;
										if      ((b < 0x20) || (b == 0x7F)) // ASCII control characters.
										{
											direct = base.ByteToElement(b, ts, d, r);
										}
										else if (b == 0x20)                 // ASCII space.
										{
											direct = base.ByteToElement(b, ts, d, r);
										}
										else if (CharEx.IsValidForUTF7((char)b))
										{
											direct = DecodeAndCreateElement(b, ts, d, r, e); // 'IsMultiByte' but the current byte must result in a single character here.
										}
										else
										{
											return (CreateInvalidByteWarning(b, ts, d, e));
										}

										// Combine into single element, accepting the limitation that a potential control character will be contained in a data element:

										var origin = new List<byte>(decodingArray.Length + 1); // Preset the required capacity to improve memory management.
										origin.AddRange(decodingArray);
										origin.Add(b);

										var text = (encoded.Text + direct.Text);

										switch (d)
										{
											case IODirection.Tx:    return (new DisplayElement.TxData(ts, origin.ToArray(), text));
											case IODirection.Rx:    return (new DisplayElement.RxData(ts, origin.ToArray(), text));

											case IODirection.Bidir:
											case IODirection.None:  throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
											default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
										}
									}
									else // Decoder has failed:
									{
										return (CreateInvalidBytesWarning(decodingArray, ts, d, e));
									}
								}
								else if (CharEx.IsValidForUTF7((char)b))     // UTF-7 trailing byte.
								{
									pendingMultiBytesToDecode.Add(b);

									return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
								}
								else
								{
									return (CreateInvalidByteWarning(b, ts, d, e));
								}
							} // direct or lead or trailing byte.
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
									return (base.ByteToElement(b, ts, d, r));
								}
								else if (b == 0x20)                          // ASCII space.
								{
									return (base.ByteToElement(b, ts, d, r));
								}
								else                                         // ASCII printable character.
								{
									return (DecodeAndCreateElement(b, ts, d, r, e)); // 'IsMultiByte' but the current byte must result in a single character here.
								}
							}
							else // (rxMultiByteDecodingStream.Count > 0) => Neither ASCII nor lead byte.
							{
								pendingMultiBytesToDecode.Add(b);

								byte[] decodingArray = pendingMultiBytesToDecode.ToArray();
								int expectedCharCount = e.GetCharCount(decodingArray);
								char[] chars = new char[expectedCharCount];
								int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
								if (effectiveCharCount == 1)
								{
									pendingMultiBytesToDecode.Clear();
									                                                    //// 'effectiveCharCount' is 1 for sure.
									return (CreateDataElement(decodingArray, ts, d, r, chars[0]));
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

										return (CreateInvalidBytesWarning(decodingArray, ts, d, e));
									}
								}
								else // (effectiveCharCount > 1)
								{
									pendingMultiBytesToDecode.Clear(); // Reset decoding stream.

									return (CreateInvalidBytesWarning(decodingArray, ts, d, e));
								}
							} // ASCII or lead or trailing byte
						} // Unicode/Non-Unicode
					} // MultiByte
				} // String/Char/Unicode

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement DecodeAndCreateElement(byte b, DateTime ts, IODirection d, Radix r, Encoding e)
		{
			int expectedCharCount = 1;
			char[] chars = new char[expectedCharCount];
			int effectiveCharCount = e.GetDecoder().GetChars(new byte[] { b }, 0, 1, chars, 0, true);
			if (effectiveCharCount == expectedCharCount)
			{                                                // 'effectiveCharCount' is 1 for sure.
				return (CreateDataElement(b, ts, d, r, chars[0]));
			}
			else // Decoder has failed:
			{
				return (CreateInvalidByteWarning(b, ts, d, e));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte origin, DateTime ts, IODirection d, Radix r, char c)
		{
			if (r != Radix.Unicode)
			{
				string text = c.ToString(CultureInfo.InvariantCulture);
				return (CreateDataElement(origin, ts, d, text));
			}
			else // Unicode:
			{
				string text = UnicodeValueToNumericString(c);
				return (CreateDataElement(origin, ts, d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, DateTime ts, IODirection d, Radix r, char c)
		{
			if (r != Radix.Unicode)
			{
				string text = c.ToString(CultureInfo.InvariantCulture);
				return (CreateDataElement(origin, ts, d, text));
			}
			else // Unicode:
			{
				string text = UnicodeValueToNumericString(c);
				return (CreateDataElement(origin, ts, d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, DateTime ts, IODirection d, Radix r, char[] text)
		{
			if (r != Radix.Unicode)
			{
				return (CreateDataElement(origin, ts, d, new string(text)));
			}
			else // Unicode:
			{
				return (CreateDataElement(origin, ts, d, new string(text)));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateInvalidByteWarning(byte b, DateTime ts, IODirection d, Encoding e)
		{
			var byteAsString = ByteHelper.FormatHexString(b, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append("'");
			sb.Append(byteAsString);
			sb.Append("' is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte!");

			return (new DisplayElement.ErrorInfo(ts, (Direction)d, sb.ToString(), true));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateInvalidBytesWarning(byte[] a, DateTime ts, IODirection d, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@"""");
			sb.Append(bytesAsString);
			sb.Append(@""" is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte sequence!");

			return (new DisplayElement.ErrorInfo(ts, (Direction)d, sb.ToString(), true));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateOutsideUnicodePlane0Warning(byte[] a, DateTime ts, IODirection d, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@"Byte sequence """);
			sb.Append(bytesAsString);
			sb.Append(@""" is outside the basic multilingual plane (plane 0) which is not yet supported but tracked as feature request #329.");

			return (new DisplayElement.ErrorInfo(ts, (Direction)d, sb.ToString(), true));
		}

		/// <remarks>This text specific implementation is based on <see cref="DisplayElementCollection.CharCount"/>.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected override void AddSpaceIfNecessary(LineState lineState, IODirection d, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(d) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.CharCount > 0) || (lp.CharCount > 0))
					lp.Add(new DisplayElement.ContentSpace((Direction)d));
			}
		}

		#endregion

		#region Process Elements
		//------------------------------------------------------------------------------------------
		// Process Elements
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes the processing state.
		/// </summary>
		protected override void InitializeProcess()
		{
			// Text unspecifics:
			base.InitializeProcess();

			// Text specifics:
			txPendingMultiBytesToDecode = new List<byte>(4); // Preset the required capacity to improve memory management; 4 is the maximum value for multi-byte characters.
			rxPendingMultiBytesToDecode = new List<byte>(4); // Preset the required capacity to improve memory management; 4 is the maximum value for multi-byte characters.

			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, Parser.Modes.RadixAndAsciiEscapes))
			{
				// Tx:
				{
					byte[] txEol;
					if (!p.TryParse(TextTerminalSettings.TxEol, out txEol))
					{
						// In case of an invalid EOL sequence, default it. This should never happen,
						// YAT verifies the EOL sequence when the user enters it. However, the user might
						// manually edit the EOL sequence in a settings file.
						TextTerminalSettings.TxEol = Settings.TextTerminalSettings.EolDefault;
						txEol = p.Parse(TextTerminalSettings.TxEol);
					}

					this.txUnidirTextLineState = new TextLineState(txEol);
					this.txBidirTextLineState  = new TextLineState(txEol);
				}

				// Rx:
				{
					byte[] rxEol;
					if (!p.TryParse(TextTerminalSettings.RxEol, out rxEol))
					{
						// In case of an invalid EOL sequence, default it. This should never happen,
						// YAT verifies the EOL sequence when the user enters it. However, the user might
						// manually edit the EOL sequence in a settings file.
						TextTerminalSettings.RxEol = Settings.TextTerminalSettings.EolDefault;
						rxEol = p.Parse(TextTerminalSettings.RxEol);
					}

					this.rxUnidirTextLineState = new TextLineState(rxEol);
					this.rxBidirTextLineState  = new TextLineState(rxEol);
				}
			}
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
				case RepositoryType.Tx:    this.txUnidirTextLineState.Reset();                                     break;
				case RepositoryType.Bidir: this.txBidirTextLineState .Reset(); this.rxBidirTextLineState .Reset(); break;
				case RepositoryType.Rx:                                        this.rxUnidirTextLineState.Reset(); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the quasi-private member
		/// <see cref="TextTerminalSettings"/>.
		/// </remarks>
		protected Settings.TextDisplaySettings GetTextDisplaySettings(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (TextTerminalSettings.TxDisplay);
				case IODirection.Rx:    return (TextTerminalSettings.RxDisplay);

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="txUnidirTextLineState"/>, <see cref="rxUnidirTextLineState"/>,
		/// <see cref="txBidirTextLineState"/>, <see cref="rxBidirTextLineState"/>.
		/// </remarks>
		protected TextLineState GetTextLineState(RepositoryType repositoryType, IODirection dir)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.txUnidirTextLineState);
				case RepositoryType.Rx:    return (this.rxUnidirTextLineState);

				case RepositoryType.Bidir: if (dir == IODirection.Tx) { return (this.txBidirTextLineState); }
				                           else                       { return (this.rxBidirTextLineState); }
				                           //// Invalid directions are asserted elsewhere.

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected override void DoRawByte(RepositoryType repositoryType,
		                                  byte b, DateTime ts, string dev, IODirection dir,
		                                  DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState        = GetProcessState(repositoryType);
			var lineState           = processState.Line; // Convenience shortcut.
			var textLineState       = GetTextLineState(repositoryType, dir);
			var textDisplaySettings = GetTextDisplaySettings(dir);

			if (lineState.Position == LinePosition.Begin)
			{
				DoLineBegin(repositoryType, processState, ts, dev, dir, elementsToAdd);
			}

			if (lineState.Position == LinePosition.Content)
			{
				DoLineContent(repositoryType, processState, textLineState, textDisplaySettings, b, ts, dev, dir, elementsToAdd);
			}

			if (lineState.Position == LinePosition.End)
			{
				DoLineEnd(repositoryType, processState, ts, elementsToAdd, linesToAdd);
			}
		}

		/// <summary></summary>
		protected override void DoLineBegin(RepositoryType repositoryType, ProcessState processState,
		                                    DateTime ts, string dev, IODirection dir,
		                                    DisplayElementCollection elementsToAdd)
		{
			base.DoLineBegin(repositoryType, processState, ts, dev, dir, elementsToAdd);

			var lineState = processState.Line; // Convenience shortcut.
			var lp = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.

			lp.Add(new DisplayElement.LineStart());

			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowDevice    ||
			    TerminalSettings.Display.ShowDirection)
			{
				DisplayElementCollection info;
				PrepareLineBeginInfo(ts, (ts - InitialTimeStamp), (ts - processState.Overall.PreviousLineTimeStamp), dev, dir, out info);
				lp.AddRange(info);
			}

			lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
			elementsToAdd.AddRange(lp);
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void DoLineContent(RepositoryType repositoryType, ProcessState processState, TextLineState textLineState, Settings.TextDisplaySettings textDisplaySettings,
		                           byte b, DateTime ts, string dev, IODirection dir,
		                           DisplayElementCollection elementsToAdd)
		{
			var lineState = processState.Line; // Convenience shortcut.

			// Convert content:
			DisplayElement de;
			bool isBackspace;
			if (!ControlCharacterHasBeenProcessed(b, ts, dir, out de, out isBackspace))
				de = ByteToElement(b, ts, dir); // Default conversion to value or ASCII mnemonic.

			var lp = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.

			// Prepare EOL:
			if (!textLineState.EolOfGivenDevice.ContainsKey(dev))                                      // It is OK to only access or add to the collection,
				textLineState.EolOfGivenDevice.Add(dev, new SequenceQueue(textLineState.EolSequence)); // this will not lead to excessive use of memory,
			                                                                                           // since there is only a given number of devices.
			// Add byte to EOL:                                                                        // Applies to TCP and UDP terminals only.
			textLineState.EolOfGivenDevice[dev].Enqueue(b);

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started:
			if (textLineState.EolOfGivenDevice[dev].IsCompleteMatch)
			{
				if (de.IsContent)
				{
					if (TextTerminalSettings.ShowEol)
					{
						AddSpaceIfNecessary(lineState, dir, lp, de);
						lp.Add(de); // No clone needed as element is no more used below.
					}
					else
					{
					////lineState.RetainedPotentialEolElements.Add(de); // Adding is useless, Confirm...() below will clear the elements anyway.

						ConfirmRetainedUnconfirmedHiddenEolElements(textLineState);
					}

					lineState.Position = LinePosition.End;
				}
				else
				{
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
				}
			}                                                                // Note the inverted implementation sequence:
			else if (textLineState.EolOfGivenDevice[dev].IsPartlyMatchContinued) //  1. CompleteMatch        (last trigger, above)
			{                                                                //  2. PartlyMatchContinued (intermediate, here)
				if (de.IsContent)                                            //  3. PartlyMatchBeginning (first trigger, below)
				{                                                            //  4. Unrelatd to EOL      (any time, further below)
					if (TextTerminalSettings.ShowEol)
					{
						AddSpaceIfNecessary(lineState, dir, lp, de);
						lp.Add(de.Clone()); // No clone needed as element is no more used below.
					}
					else
					{
						textLineState.RetainedUnconfirmedHiddenEolElements.Add(de); // No clone needed as element is no more used below.
					}
				}
				else
				{
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
				}
			}
			else if (textLineState.EolOfGivenDevice[dev].IsPartlyMatchBeginning)
			{
				// Previous was no match, retained potential EOL elements can be treated as non-EOL:
				ReleaseRetainedUnconfirmedHiddenEolElements(textLineState, lp);

				if (de.IsContent)
				{
					if (TextTerminalSettings.ShowEol)
					{
						AddSpaceIfNecessary(lineState, dir, lp, de);
						lp.Add(de.Clone()); // No clone needed as element is no more used below.
					}
					else
					{
						textLineState.RetainedUnconfirmedHiddenEolElements.Add(de); // No clone needed as element is no more used below.

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
						// Retained EOL elements shall be shown after a timeout of e.g. 150 ms => FR #364.
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
				ReleaseRetainedUnconfirmedHiddenEolElements(textLineState, lp);

				// Add the current element, which for sure is not related to EOL:
				AddSpaceIfNecessary(lineState, dir, lp, de);
				lp.Add(de); // No clone needed as element has just been created further above.
			}

			if (lineState.Position != LinePosition.ContentExceeded)
			{
				if (isBackspace)
				{
					// Note that backspace must be executed after EOL since...
					// ...unconfirmed hidden EOL elements may have to be released.
					// ...EOL could contain backspace, unlikely but possible.

					// Remove the just added backspace:
					int count = lp.Count;
					if ((count > 0) && (lp[count - 1] is DisplayElement.Nonentity))
					{
						lp.RemoveLast();
						RemoveSpaceIfNecessary(dir, lp);
					}
				}

				lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elementsToAdd.AddRange(lp);

				if (isBackspace)
				{
					// If the current line does contain a preceeding "true" character...
					if (lineState.Elements.DataContentCharCount > 0)
					{
						// ...remove it in the current line...
						lineState.Elements.RemoveLastDataContentChar();
						RemoveSpaceIfNecessary(dir, lineState.Elements);

						if (elementsToAdd.DataContentCharCount > 0)
						{
							// ..as well as in the pending elements:
							elementsToAdd.RemoveLastDataContentChar();
							RemoveSpaceIfNecessary(dir, elementsToAdd);
						}
						else
						{
							elementsToAdd.Clear(); // Whole line will be replaced, pending elements can be discarded.
							FlushReplaceAlreadyStartedLine(repositoryType, processState, elementsToAdd);
						}
					}
				}
			}

			// Only continue evaluation if no line break detected yet (cannot have more than one line break).
			if ((textDisplaySettings.LengthLineBreak.Enabled) &&
				(lineState.Position != LinePosition.End))
			{
				if (lineState.Elements.CharCount >= textDisplaySettings.LengthLineBreak.Length)
					lineState.Position = LinePosition.End;

				// Note that length line break shall be applied even when EOL has just started or is already ongoing,
				// since remaining hidden EOL elements will not result in additional lines.
			}

			if (lineState.Position != LinePosition.End)
			{
				if ((lineState.Elements.CharCount > TerminalSettings.Display.MaxLineLength) &&
					(lineState.Position != LinePosition.ContentExceeded))
				{
					lineState.Position = LinePosition.ContentExceeded;
					                                  //// Using term "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.
					var message = "Maximal number of bytes per line exceeded! Check the EOL (end-of-line) settings or increase the limit in the advanced terminal settings.";
					lineState.Elements.Add(new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));
					elementsToAdd.Add(     new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual bool ControlCharacterHasBeenProcessed(byte b, DateTime ts, IODirection dir, out DisplayElement de, out bool isBackspace)
		{
			isBackspace = false;

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
					if (!(TerminalSettings.CharReplace.ReplaceControlChars && TerminalSettings.CharReplace.ReplaceBackspace))
					{
						isBackspace = true;

						de = new DisplayElement.Nonentity();
						return (true);
					}

					break;
				}

				case 0x09: // <TAB> (tabulator)
				{
					if (!(TerminalSettings.CharReplace.ReplaceControlChars && TerminalSettings.CharReplace.ReplaceTab))
					{
						// Attention:
						// In order to get well aligned tab stops, tab characters must be data elements.
						// If they were control elements (i.e. sequence of data and control elements),
						// tabs would only get aligned within the respective control element,
						// thus resulting in misaligned tab stops.

						de = CreateDataElement(b, ts, dir, "\t");
						return (true);
					}

					break;
				}
			}

			de = null;
			return (false);
		}

		private static void ConfirmRetainedUnconfirmedHiddenEolElements(TextLineState textLineState)
		{
			if (textLineState.RetainedUnconfirmedHiddenEolElements.Count > 0)
			{
				textLineState.RetainedUnconfirmedHiddenEolElements.Clear();
			}
		}

		private static void ReleaseRetainedUnconfirmedHiddenEolElements(TextLineState textLineState, DisplayElementCollection lp)
		{
			if (textLineState.RetainedUnconfirmedHiddenEolElements.Count > 0)
			{
				lp.AddRange(textLineState.RetainedUnconfirmedHiddenEolElements); // No clone needed as collection is cleared below.
				textLineState.RetainedUnconfirmedHiddenEolElements.Clear();
			}
		}

		/// <summary></summary>
		protected override void DoLineEnd(RepositoryType repositoryType, ProcessState processState,
		                                  DateTime ts,
		                                  DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			// Note: Code sequence the same as DoLineEnd() of BinaryTerminal for better comparability.

			var lineState = processState.Line; // Convenience shortcut.
			var textLineState = GetTextLineState(repositoryType, lineState.Direction);
			var dev = lineState.Device;

			bool isEmptyLine    = ( lineState.Elements.CharCount == 0);
			bool isPendingEol   = (!textLineState.EolOfLastLineWasCompleteMatch(dev) &&  textLineState.EolIsAnyMatch(dev));
			bool isNotHiddenEol = ( textLineState.EolOfLastLineWasCompleteMatch(dev) && !textLineState.EolIsAnyMatch(dev));
			if (isEmptyLine && isPendingEol) // While intended empty lines must be shown, potentially suppress
			{                                // empty lines that only contain hidden pending EOL character(s):
				elementsToAdd.RemoveAtEndUntil(typeof(DisplayElement.LineStart));                      // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                                       //            All other elements must be removed as well!
				FlushClearAlreadyStartedLine(repositoryType, processState, elementsToAdd, linesToAdd); //            This is ensured by flushing here.
			}
			else if (isEmptyLine && isNotHiddenEol) // While intended empty lines must be shown, potentially suppress
			{                                       // empty lines that only contain hidden non-EOL character(s) (e.g. hidden 0x00):
				elementsToAdd.RemoveAtEndUntil(typeof(DisplayElement.LineStart));                      // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                                       //            All other elements must be removed as well!
				FlushClearAlreadyStartedLine(repositoryType, processState, elementsToAdd, linesToAdd); //            This is ensured by flushing here.
			}
			else // Not empty:
			{
				// Process line length:
				var lineEnd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
				if (TerminalSettings.Display.ShowLength || TerminalSettings.Display.ShowDuration) // Meaning: "byte count"/"char count" and "line duration".
				{
					int length;
					if (TerminalSettings.Display.LengthSelection == LengthSelection.CharCount)
						length = lineState.Elements.CharCount;
					else        // incl. Display.LengthSelection == LengthSelection.ByteCount)
						length = lineState.Elements.ByteCount;

					DisplayElementCollection info;
					PrepareLineEndInfo(length, (ts - lineState.TimeStamp), out info);
					lineEnd.AddRange(info);
				}

				lineEnd.Add(new DisplayElement.LineBreak());
				elementsToAdd.AddRange(lineEnd.Clone()); // Clone elements because they are needed again right below.

				// Finalize line:                // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
				var l = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
				l.AddRange(lineState.Elements); // No clone needed as elements are no more used and will be reset below.
				l.AddRange(lineEnd);
				linesToAdd.Add(l);
			}

			// Finalize the line:
			textLineState.NotifyLineEnd(dev, textLineState.EolIsCompleteMatch(dev));
			base.DoLineEnd(repositoryType, processState, ts, elementsToAdd, linesToAdd);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
