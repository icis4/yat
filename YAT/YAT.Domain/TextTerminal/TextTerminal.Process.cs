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
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Media;
using System.Text;
using System.Threading;

using MKY;
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

		private LineBreakTimeout glueCharsOfLineTimeout;

		private object waitForResponseClearanceSyncObj = new object();
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

						if      ((b < 0x20) || (b == 0x7F))              // ASCII control characters.
						{
							return (base.ByteToElement(b, ts, dir, r, pendingMultiBytesToDecode));
						}
						else if  (b == 0x20)                             // ASCII space.
						{
							return (base.ByteToElement(b, ts, dir, r, pendingMultiBytesToDecode));
						}                                                // Special case.
						else if ((b == 0xFF) && TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
						{
							return (new DisplayElement.Nonentity());     // Return nothing, ignore the character, this results in hiding.
						}
						else                                             // ASCII and extended ASCII printable characters.
						{
							return (DecodeAndCreateElement(b, ts, dir, r, e)); // 'IsSingleByte' always results in a single character per byte.
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
									{                                                            // 'effectiveCharCount' is 1 for sure.
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

									return (CreateInvalidBytesWarning(decodingArray, ts, dir, e));
								}
							}
							else // (effectiveCharCount > 1) => Code doesn't fit into a single u16 value, thus more than one character will be returned.
							{
								pendingMultiBytesToDecode.Clear();                   // Reset decoding stream.

								return (CreateOutsideUnicodePlane0Warning(decodingArray, ts, dir, e));
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
									return (DecodeAndCreateElement(b, ts, dir, r, e)); // 'IsMultiByte' but the current byte must result in a single character here.
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
									                                                    //// 'effectiveCharCount' is 1 for sure.
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

										return (CreateInvalidBytesWarning(decodingArray, ts, dir, e));
									}
								}
								else // (effectiveCharCount > 1)
								{
									pendingMultiBytesToDecode.Clear(); // Reset decoding stream.

									return (CreateInvalidBytesWarning(decodingArray, ts, dir, e));
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement DecodeAndCreateElement(byte b, DateTime ts, IODirection dir, Radix r, Encoding e)
		{
			int expectedCharCount = 1;
			char[] chars = new char[expectedCharCount];
			int effectiveCharCount = e.GetDecoder().GetChars(new byte[] { b }, 0, 1, chars, 0, true);
			if (effectiveCharCount == expectedCharCount)
			{                                                // 'effectiveCharCount' is 1 for sure.
				return (CreateDataElement(b, ts, dir, r, chars[0]));
			}
			else // Decoder has failed:
			{
				return (CreateInvalidByteWarning(b, ts, dir, e));
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
		protected virtual DisplayElement CreateInvalidByteWarning(byte b, DateTime ts, IODirection dir, Encoding e)
		{
			var byteAsString = ByteHelper.FormatHexString(b, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append("'");
			sb.Append(byteAsString);
			sb.Append("' is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte!");

			return (new DisplayElement.ErrorInfo(ts, (Direction)dir, sb.ToString(), true));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateInvalidBytesWarning(byte[] a, DateTime ts, IODirection dir, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@"""");
			sb.Append(bytesAsString);
			sb.Append(@""" is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte sequence!");

			return (new DisplayElement.ErrorInfo(ts, (Direction)dir, sb.ToString(), true));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateOutsideUnicodePlane0Warning(byte[] a, DateTime ts, IODirection dir, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@"Byte sequence """);
			sb.Append(bytesAsString);
			sb.Append(@""" is outside the basic multilingual plane (plane 0) which is not yet supported but tracked as feature request #329.");

			return (new DisplayElement.ErrorInfo(ts, (Direction)dir, sb.ToString(), true));
		}

		/// <remarks>This text specific implementation is based on <see cref="DisplayElementCollection.CharCount"/>.</remarks>
		protected override void AddContentSeparatorIfNecessary(LineState lineState, IODirection dir, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(dir) && !string.IsNullOrEmpty(de.Text))
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

		/// <summary>
		/// Initializes the processing state.
		/// </summary>
		protected override void InitializeProcess()
		{
			// Text unspecifics:
			base.InitializeProcess();

			// Text specifics:
			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, Parser.Mode.RadixAndAsciiEscapes))
			{
				// Tx:
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

				// Rx:
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

			if (TextTerminalSettings.WaitForResponse.Enabled)
			{
				ResetWaitForResponse();
			}
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
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="textTxState"/>, <see cref="textRxState"/>,
		/// <see cref="textBidirTxState"/>, <see cref="textBidirRxState"/>.
		/// </remarks>
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
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Implements the text terminal specific <see cref="Settings.TextTerminalSettings.GlueCharsOfLine"/> functionality.
		/// </summary>
		protected override void ProcessChunk(RepositoryType repositoryType, RawChunk chunk, out bool partlyOrCompletelyPostponed)
		{
			ProcessGlueCharsOfLineIfNeeded(repositoryType, chunk, out partlyOrCompletelyPostponed);
			if (partlyOrCompletelyPostponed)
				return;
			else
				base.ProcessChunk(repositoryType, chunk, out partlyOrCompletelyPostponed);
		}

		/// <summary></summary>
		protected virtual void ProcessGlueCharsOfLineIfNeeded(RepositoryType repositoryType, RawChunk chunk, out bool partlyOrCompletelyPostponed)
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
						if (isServerSocket && TerminalSettings.Display.DeviceLineBreakEnabled) // Attention: This 'isServerSocket' restriction is also implemented at other locations!
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
								DebugGlueCharsOfLine(string.Format(CultureInfo.InvariantCulture, "Glueing determined to postpone whole {0} chunk of {1} byte(s) instead of performing device or direction line break.", chunk.Direction, chunk.Content.Count));

								PostponeChunk(repositoryType, chunk);
								partlyOrCompletelyPostponed = true;
								return;
							}
						}
					} // if (position != Begin)
				} // if (bidirIsAffected)
			} // if (GlueCharsOfLine.Enabled)

			partlyOrCompletelyPostponed = false;
		}

		/// <summary></summary>
		protected virtual bool GlueCharsOfLineTimeoutHasElapsed(DateTime instantInQuestion, DateTime lineTimeStamp)
		{
			if (TextTerminalSettings.GlueCharsOfLine.Timeout != Timeout.Infinite)
			{
				var duration = (instantInQuestion - lineTimeStamp).TotalMilliseconds;
				return (duration >= TextTerminalSettings.GlueCharsOfLine.Timeout);
			}
			else // Infinite:
			{
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected override void ProcessByteOfChunk(RepositoryType repositoryType,
		                                           byte b, DateTime ts, string dev, IODirection dir,
		                                           DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd,
		                                           out bool breakChunk)
		{
			var processState = GetProcessState(repositoryType);
			var lineState = processState.Line; // Convenience shortcut.

			// When glueing is enabled, potentially postpone remaining byte(s) of chunk until
			// previously postponed chunk(s) has been processed.
			if (lineState.Position == LinePosition.End)
			{
				ProcessGlueCharsOfLineIfNeeded(repositoryType, processState, lineState, ts, dir, out breakChunk);
				if (breakChunk)
					return;
			}

			// The first byte of a line will sequentially trigger the [Begin] as well as [Content]
			// condition below. In the normal case, the line will then contain the first displayed
			// element. However, when initially receiving a hidden e.g. <XOn>, the line will yet be
			// empty. Then, when subsequent bytes are received, even when seconds later, the line's
			// initial time stamp is kept. This is illogical, the time stamp of a hidden <XOn> shall
			// not define the time stamp of the line, thus handle such case by rebeginning the line.
			if (lineState.Position == LinePosition.Content)
				DoLineContentCheck(repositoryType, processState, ts, dir);

			if (lineState.Position == LinePosition.Begin)
				DoLineBegin(repositoryType, processState, ts, dev, dir, elementsToAdd);

			if (lineState.Position == LinePosition.Content)
				DoLineContent(repositoryType, processState, b, ts, dev, dir, elementsToAdd);

			if (lineState.Position == LinePosition.End)
				DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd);

			breakChunk = false;
		}

		/// <summary></summary>
		protected virtual void ProcessGlueCharsOfLineIfNeeded(RepositoryType repositoryType, ProcessState processState, LineState lineState, DateTime ts, IODirection dir, out bool breakChunk)
		{
			if (TextTerminalSettings.GlueCharsOfLine.Enabled)
			{
				if (repositoryType == RepositoryType.Bidir) // Glueing only applies to bidirectional processing.
				{
					var overallState = processState.Overall; // Convenience shortcut.
					var postponedChunkCount = overallState.GetPostponedChunkCount(dir);
					if (postponedChunkCount > 0)
					{
						var firstPostponedChunkTimeStamp = overallState.GetFirstPostponedChunkTimeStamp(dir);
						if ((firstPostponedChunkTimeStamp < ts) || GlueCharsOfLineTimeoutHasElapsed(firstPostponedChunkTimeStamp, lineState.TimeStamp))
						{
							DebugGlueCharsOfLine(string.Format(CultureInfo.InvariantCulture, "Glueing determined to break {0} chunk and postpone remaining bytes.", dir));

							breakChunk = true;
							return;
						}
					}
				}
			}

			breakChunk = false;
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
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
				if (TerminalSettings.Display.ShowTimeSpan)  { lineState.Elements.ReplaceTimeSpan( ts - InitialTimeStamp,                           TerminalSettings.Display.TimeSpanFormat,                                            left, right); doReplace = true; }
				if (TerminalSettings.Display.ShowTimeDelta) { lineState.Elements.ReplaceTimeDelta(ts - processState.Overall.PreviousLineTimeStamp, TerminalSettings.Display.TimeDeltaFormat,                                           left, right); doReplace = true; }

				if (doReplace)
				{
				////elementsToAdd.Clear() is not needed as only replace happens above.
					FlushReplaceAlreadyBeganLine(repositoryType, processState);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void DoLineContent(RepositoryType repositoryType, ProcessState processState,
		                           byte b, DateTime ts, string dev, IODirection dir,
		                           DisplayElementCollection elementsToAdd)
		{
			var lineState = processState.Line; // Convenience shortcut.

			var textUnidirState     = GetTextUnidirState(repositoryType, dir);
			var textDisplaySettings = GetTextDisplaySettings(dir);

			// Convert content:
			DisplayElement de;
			bool isBackspace;
			if (!ControlCharacterHasBeenProcessed(b, ts, dir, out de, out isBackspace))
				de = ByteToElement(b, ts, dir, textUnidirState.PendingMultiBytesToDecode); // Default conversion to value or ASCII mnemonic.

			var lp = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.

			// Prepare EOL:
			if (!textUnidirState.EolOfGivenDevice.ContainsKey(dev))                                      // It is OK to only access or add to the collection,
				textUnidirState.EolOfGivenDevice.Add(dev, new SequenceQueue(textUnidirState.EolSequence)); // this will not lead to excessive use of memory,
			                                                                                           // since there is only a given number of devices.
			// Add byte to EOL:                                                                        // Applies to TCP and UDP server terminals only.
			textUnidirState.EolOfGivenDevice[dev].Enqueue(b);

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started:
			if (textUnidirState.EolOfGivenDevice[dev].IsCompleteMatch)
			{
				if (de.IsContent)
				{
					if (TextTerminalSettings.ShowEol)
					{
						AddContentSeparatorIfNecessary(lineState, dir, lp, de);
						lp.Add(de); // No clone needed as element is no more used below.
					}
					else
					{
					////lineState.RetainedPotentialEolElements.Add(de); // Adding is useless, Confirm...() below will clear the elements anyway.

						ConfirmRetainedUnconfirmedHiddenEolElements(textUnidirState);
					}

					lineState.Position = LinePosition.End;

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
						AddContentSeparatorIfNecessary(lineState, dir, lp, de);
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
						AddContentSeparatorIfNecessary(lineState, dir, lp, de);
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
				ReleaseRetainedUnconfirmedHiddenEolElements(lineState, textUnidirState, dir, lp);

				// Add the current element, which for sure is not related to EOL:
				AddContentSeparatorIfNecessary(lineState, dir, lp, de);
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
						RemoveContentSeparatorIfNecessary(dir, lp);
					}
				}

				textUnidirState.NotifyShownCharCount(lp.CharCount);
				lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elementsToAdd.AddRange(lp);

				if (isBackspace)
				{
					// Note that backspace must be executed after adding above since...
					// ...character to be removed likely had already been contained before adding above.

					// If the current line does contain a preceeding "true" character...
					if (lineState.Elements.DataContentCharCount > 0)
					{
						// ...remove it in the current line...
						lineState.Elements.RemoveLastDataContentChar();
						RemoveContentSeparatorIfNecessary(dir, lineState.Elements);

						if (elementsToAdd.DataContentCharCount > 0)
						{
							// ..as well as in the pending elements:
							elementsToAdd.RemoveLastDataContentChar();
							RemoveContentSeparatorIfNecessary(dir, elementsToAdd);
						}
						else
						{
							elementsToAdd.Clear(); // Whole line will be replaced, pending elements can be discarded.
							FlushReplaceAlreadyBeganLine(repositoryType, processState);
						}

						// Don't forget to adjust state:
						textUnidirState.NotifyShownCharCount(-1);
					}
				}
			}

			// Only continue evaluation if no line break detected yet (cannot have more than one line break).
			if ((lineState.Position != LinePosition.End) && (textDisplaySettings.LengthLineBreak.Enabled))
			{
				if (lineState.Elements.CharCount >= textDisplaySettings.LengthLineBreak.Length)
					lineState.Position = LinePosition.End;

				// Note that length line break shall be applied even when EOL has just started or is already ongoing,
				// remaining hidden EOL elements will not result in additional lines.
			}

			if (lineState.Position != LinePosition.End)
			{
				if ((lineState.Position != LinePosition.ContentExceeded) && (lineState.Elements.CharCount > TerminalSettings.Display.MaxLineLength))
				{
					lineState.Position = LinePosition.ContentExceeded;

					var message = "Maximal number of characters per line exceeded! Check the EOL (end-of-line) settings or increase the limit in the advanced terminal settings.";
					lineState.Elements.Add(new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));
					elementsToAdd.Add(     new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
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
					if (RadixIsStringOrChar(dir)) // Attention: This logic is also implemented in the text settings!
					{
						bool replace = (TerminalSettings.CharReplace.ReplaceControlChars && TerminalSettings.CharReplace.ReplaceBackspace);
						if (!replace)
						{
							isBackspace = true;

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
					AddContentSeparatorIfNecessary(lineState, dir, lp, de);
					lp.Add(de); // No clone needed as element is no more used below.
				}

				textUnidirState.RetainedUnconfirmedHiddenEolElements.Clear();
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		protected override void DoLineEnd(RepositoryType repositoryType, ProcessState processState,
		                                  DateTime ts, IODirection dir,
		                                  DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			// Note: The test cases of [YAT - Test.ods]::[YAT.Domain.Terminal] cover the empty line cases.

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
				elementsToAdd.RemoveLastUntil(typeof(DisplayElement.LineStart));                     // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                                     //            All other elements must be removed as well!
				FlushClearAlreadyBeganLine(repositoryType, processState, elementsToAdd, linesToAdd); //            This is ensured by flushing here.
			}
			else if (isEmptyLineWithPendingEol && !isEmptyLineWithPendingEolToBeShown) // While intended empty lines must be shown, potentially suppress
			{                                                                          // empty lines that only contain hidden pending EOL character(s):
				elementsToAdd.RemoveLastUntil(typeof(DisplayElement.LineStart));                     // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                                     //            All other elements must be removed as well!
				FlushClearAlreadyBeganLine(repositoryType, processState, elementsToAdd, linesToAdd); //            This is ensured by flushing here.
			}
			else // Neither empty nor need to suppress:
			{
				// Process line length:
				var lineEnd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
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
			textUnidirState.NotifyLineEnd(dev);
			base.DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd);
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

				this.glueCharsOfLineTimeout = new LineBreakTimeout(TextTerminalSettings.GlueCharsOfLine.Timeout);
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
				this.glueCharsOfLineTimeout.Stop();
		}

		/// <remarks>
		/// This event handler must synchronize against <see cref="Terminal.ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void glueCharsOfLineTimeout_Elapsed(object sender, EventArgs e)
		{
			DebugGlueCharsOfLine("glueCharsOfLineTimeout_Elapsed");

			lock (ChunkVsTimedSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				if (IsInDisposal) // Ensure to not handle async timer callback during closing anymore.
					return;

			////if (TextTerminalSettings.GlueCharsOfLine.Enabled) is implicitly given.
				{
					ProcessAndSignalGlueCharsOfLineTimeout();
				}
			}
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalGlueCharsOfLineTimeout()
		{
			ProcessAndSignalGlueCharsOfLineTimeout(IODirection.Tx);
			ProcessAndSignalGlueCharsOfLineTimeout(IODirection.Rx);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalGlueCharsOfLineTimeout(IODirection dir)
		{
			var overallState = GetOverallState(RepositoryType.Bidir); // Glueing only applies to bidirectional processing.
			var postponedChunks = overallState.RemovePostponedChunks(dir);
			if (postponedChunks.Length > 0)
			{
				DebugGlueCharsOfLine(string.Format(CultureInfo.InvariantCulture, "Glueing determined to process {0} postponed and timed out chunk(s).", dir));

				ProcessChunksOfSameDirection(RepositoryType.Bidir, postponedChunks , dir); // Glueing only applies to bidirectional processing.
			}
		}

		#endregion

		#region WaitForResponse
		//------------------------------------------------------------------------------------------
		// WaitForResponse
		//------------------------------------------------------------------------------------------

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
		protected virtual bool GetLineClearance(ForSomeTimeEventHelper forSomeTimeEventHelper)
		{
			DebugWaitForResponse("Getting line clearance..."); //da mues doch IsSending scho aktiv si !!!

			var decrementIsRequired = false;

			while (!DoBreak)
			{
				if (ClearanceHasBeenGranted())
				{
					if (decrementIsRequired)
						DecrementIsSendingForSomeTimeChanged();

					return (true);
				}

				if (forSomeTimeEventHelper.RaiseEventIfTotalTimeLagIsAboveThreshold()) // Signal wait operation if needed.
				{
					IncrementIsSendingForSomeTimeChanged();
					decrementIsRequired = true;
				}

				try
				{
					// WaitOne() will wait forever if the underlying I/O provider has crashed, or
					// if the overlying client isn't able or forgets to call Stop() or Dispose().
					// Therefore, only wait for a certain period and then poll the run flag again.
					// The period can be quite long, as an event trigger will immediately resume.
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

			if (decrementIsRequired)
				DecrementIsSendingForSomeTimeChanged();

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

						DebugWaitForResponse(              "...granted due to timeout.");
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

				DebugWaitForResponse("...pending for counter to increase, or timeout to elapse.");
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
