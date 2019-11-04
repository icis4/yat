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
using MKY.Diagnostics;
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
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		#region Types > Line State
		//------------------------------------------------------------------------------------------
		// Types > Line State
		//------------------------------------------------------------------------------------------

		private enum LinePosition
		{
			Begin,
			Content,
			ContentExceeded,
			End
		}

		private class LineState : IDisposable, IDisposableEx
		{
			public byte[] EolSequence { get; }

			public LinePosition             Position  { get; set; }
			public DisplayElementCollection Elements  { get; set; }
			public DateTime                 TimeStamp { get; set; }
			public string                   Device    { get; set; }

			public Dictionary<string, SequenceQueue> EolOfGivenDevice                           { get; set; }
			public DisplayElementCollection          RetainedUnconfirmedHiddenEolElements       { get; set; }
			public Dictionary<string, bool>          EolOfLastLineOfGivenDeviceWasCompleteMatch { get; set; }

			public bool Highlight                        { get; set; }
			public bool FilterDetectedInFirstChunkOfLine { get; set; } // Line shall continuously get shown if filter is active from the first chunk.
			public bool FilterDetectedInSubsequentChunk  { get; set; } // Line shall be retained and delay-shown if filter is detected subsequently.
			public bool SuppressIfNotFiltered            { get; set; }
			public bool SuppressIfSubsequentlyTriggered  { get; set; }
			public bool SuppressForSure                  { get; set; }

			public LineBreakTimeout BreakTimeout { get; set; }

			public LineState(byte[] eolSequence, LineBreakTimeout breakTimeout)
			{
				EolSequence = eolSequence;

				Position  = LinePosition.Begin;
				Elements  = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
				TimeStamp = DateTime.Now;
				Device    = null;

				EolOfGivenDevice                           = new Dictionary<string, SequenceQueue>(); // No preset needed, the default initial capacity is good enough.
				RetainedUnconfirmedHiddenEolElements       = new DisplayElementCollection();          // No preset needed, the default initial capacity is good enough.
				EolOfLastLineOfGivenDeviceWasCompleteMatch = new Dictionary<string, bool>();          // No preset needed, the default initial capacity is good enough.

				Highlight                        = false;
				FilterDetectedInFirstChunkOfLine = false;
				FilterDetectedInSubsequentChunk  = false;
				SuppressIfNotFiltered            = false;
				SuppressIfSubsequentlyTriggered  = false;
				SuppressForSure                  = false;

				BreakTimeout = breakTimeout;
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			public bool IsDisposed { get; protected set; }

			/// <summary></summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary></summary>
			protected virtual void Dispose(bool disposing)
			{
				if (!IsDisposed)
				{
					// Dispose of managed resources if requested:
					if (disposing)
					{
						// In the 'normal' case, the timer is stopped in ExecuteLineEnd().
						if (BreakTimeout != null)
						{
							BreakTimeout.Dispose();
							EventHandlerHelper.RemoveAllEventHandlers(BreakTimeout);

							// \remind (2016-09-08 / MKY)
							// Whole timer handling should be encapsulated into the 'LineState' class.
						}
					}

					// Set state to disposed:
					BreakTimeout = null;
					IsDisposed = true;
				}
			}

		#if (DEBUG)
			/// <remarks>
			/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
			/// "Types that declare disposable members should also implement IDisposable. If the type
			///  does not own any unmanaged resources, do not implement a finalizer on it."
			///
			/// Well, true for best performance on finalizing. However, it's not easy to find missing
			/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
			/// is kept for DEBUG, indicating missing calls.
			///
			/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
			/// </remarks>
			~LineState()
			{
				Dispose(false);

				DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
			}
		#endif // DEBUG

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (IsDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
			}

			#endregion

			public virtual void Reset(string formerDevice, bool eolWasCompleteMatch)
			{
				AssertNotDisposed();

				Position  = LinePosition.Begin;
				Elements  = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
				TimeStamp = DateTime.Now;
				Device    = null;

				if (EolOfGivenDevice.ContainsKey(formerDevice))
				{
					if (EolOfGivenDevice[formerDevice].IsCompleteMatch)
						EolOfGivenDevice[formerDevice].Reset();

					// Keep EOL state when incomplete. Subsequent lines
					// need this to handle broken/pending EOL characters.
				}
				else                                                                    // It is OK to only access or add to the collection,
				{                                                                       // this will not lead to excessive use of memory,
					EolOfGivenDevice.Add(formerDevice, new SequenceQueue(EolSequence)); // since there is only a given number of devices.
				}                                                                       // Applies to TCP and UDP terminals only.

				if (eolWasCompleteMatch) // Keep unconfirmed hidden elements! They shall be delay-shown in case EOL is indeed unconfirmed!
					RetainedUnconfirmedHiddenEolElements = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.

				if (EolOfLastLineOfGivenDeviceWasCompleteMatch.ContainsKey(formerDevice))
					EolOfLastLineOfGivenDeviceWasCompleteMatch[formerDevice] = eolWasCompleteMatch;
				else
					EolOfLastLineOfGivenDeviceWasCompleteMatch.Add(formerDevice, eolWasCompleteMatch); // Same as above, it is OK to only access or add to the collection.

				Highlight                        = false;
				FilterDetectedInFirstChunkOfLine = false;
				FilterDetectedInSubsequentChunk  = false;
				SuppressIfNotFiltered            = false;
				SuppressIfSubsequentlyTriggered  = false;
				SuppressForSure                  = false;
			}

			public virtual bool AnyFilterDetected
			{
				get { return (FilterDetectedInFirstChunkOfLine || FilterDetectedInSubsequentChunk); }
			}

			public virtual bool EolOfLastLineWasCompleteMatch(string dev)
			{
				if (EolOfLastLineOfGivenDeviceWasCompleteMatch.ContainsKey(dev))
					return (EolOfLastLineOfGivenDeviceWasCompleteMatch[dev]);
				else
					return (true); // Cleared monitors mean that last line was complete!
			}

			public virtual bool EolIsAnyMatch(string dev)
			{
				if (EolOfGivenDevice.ContainsKey(dev))
					return (EolOfGivenDevice[dev].IsAnyMatch);
				else
					return (false);
			}

			public virtual bool EolIsCompleteMatch(string dev)
			{
				if (EolOfGivenDevice.ContainsKey(dev))
					return (EolOfGivenDevice[dev].IsCompleteMatch);
				else
					return (false);
			}
		}

		private class BidirLineState
		{
			public bool IsFirstChunk          { get; set; }
			public bool IsFirstLine           { get; set; }
			public string Device              { get; set; }
			public IODirection Direction      { get; set; }
			public DateTime LastLineTimeStamp { get; set; }

			public BidirLineState()
			{
				IsFirstChunk      = true;
				IsFirstLine       = true;
				Device            = null;
				Direction         = IODirection.None;
				LastLineTimeStamp = DateTime.Now;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstChunk      = rhs.IsFirstChunk;
				IsFirstLine       = rhs.IsFirstLine;
				Device            = rhs.Device;
				Direction         = rhs.Direction;
				LastLineTimeStamp = rhs.LastLineTimeStamp;
			}
		}

	#if (WITH_SCRIPTING)

		private class ScriptMessageState
		{
			public byte[] EolSequence { get; }
			public SequenceQueue Eol  { get; }
			public List<byte>    Data { get; set; }

			public ScriptMessageState(byte[] eolSequence)
			{
				EolSequence = eolSequence;
				Eol = new SequenceQueue(EolSequence);
				Data = new List<byte>(128); // Preset the capacity to improve memory management. 128 is an arbitrary value.
			}

			public virtual void Reset()
			{
				Data = new List<byte>(128); // Preset the capacity to improve memory management. 128 is an arbitrary value.
			}
		}

	#endif // WITH_SCRIPTING

		#endregion

		#region Types > Line Send Delay
		//------------------------------------------------------------------------------------------
		// Types > Line Send Delay
		//------------------------------------------------------------------------------------------

		private class LineSendDelayState
		{
			public int LineCount { get; set; }

			public LineSendDelayState()
			{
				LineCount = 0;
			}

			public LineSendDelayState(LineSendDelayState rhs)
			{
				LineCount = rhs.LineCount;
			}

			public virtual void Reset()
			{
				LineCount = 0;
			}
		}

		#endregion

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private List<byte> rxMultiByteDecodingStream;

		private LineState txLineState;
		private LineState rxLineState;
	#if (WITH_SCRIPTING)
		private ScriptMessageState rxScriptMessageState;
	#endif

		private BidirLineState bidirLineState;
		private LineSendDelayState lineSendDelayState;

		private object processSyncObj = new object();

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Process Elements
		//------------------------------------------------------------------------------------------
		// Process Elements
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			this.rxMultiByteDecodingStream = new List<byte>(4); // Preset the required capacity to improve memory management; 4 is the maximum value for multi-byte characters.

			byte[] txEol;
			byte[] rxEol;
			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, Parser.Modes.RadixAndAsciiEscapes))
			{
				if (!p.TryParse(TextTerminalSettings.TxEol, out txEol))
				{
					// In case of an invalid EOL sequence, default it. This should never happen,
					// YAT verifies the EOL sequence when the user enters it. However, the user might
					// manually edit the EOL sequence in a settings file.
					TextTerminalSettings.TxEol = Settings.TextTerminalSettings.EolDefault;
					txEol = p.Parse(TextTerminalSettings.TxEol);
				}

				if (!p.TryParse(TextTerminalSettings.RxEol, out rxEol))
				{
					// In case of an invalid EOL sequence, default it. This should never happen,
					// YAT verifies the EOL sequence when the user enters it. However, the user might
					// manually edit the EOL sequence in a settings file.
					TextTerminalSettings.RxEol = Settings.TextTerminalSettings.EolDefault;
					rxEol = p.Parse(TextTerminalSettings.RxEol);
				}
			}

			LineBreakTimeout t;

			// Tx:

			t = new LineBreakTimeout(TextTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
			t.Elapsed += txTimedLineBreakTimeout_Elapsed;

			if (this.txLineState != null) // Ensure to free referenced resources such as the 'Elapsed' event handler of the timer.
				this.txLineState.Dispose();

			this.txLineState = new LineState(txEol, t);

			// Rx:

			t = new LineBreakTimeout(TextTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
			t.Elapsed += rxTimedLineBreakTimeout_Elapsed;

			if (this.rxLineState != null) // Ensure to free referenced resources such as the 'Elapsed' event handler of the timer.
				this.rxLineState.Dispose();

			this.rxLineState = new LineState(rxEol, t);
		#if (WITH_SCRIPTING)
			this.rxScriptMessageState = new ScriptMessageState(rxEol);
		#endif

			// Bidir:

			this.bidirLineState = new BidirLineState();
			this.lineSendDelayState = new LineSendDelayState();
		}

		/// <summary></summary>
		protected override DisplayElement ByteToElement(byte b, IODirection d, Radix r)
		{
			switch (r)
			{
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					return (base.ByteToElement(b, d, r));
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
							return (base.ByteToElement(b, d, r));
						}
						else if  (b == 0x20)                             // ASCII space.
						{
							return (base.ByteToElement(b, d, r));
						}                                                // Special case.
						else if ((b == 0xFF) && TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
						{
							return (new DisplayElement.Nonentity());     // Return nothing, ignore the character, this results in hiding.
						}
						else                                             // ASCII and extended ASCII printable characters.
						{
							return (DecodeAndCreateElement(b, d, r, e)); // 'IsSingleByte' always results in a single character per byte.
						}
					}
					else // 'IsMultiByte':
					{
						// \remind (2017-12-09 / MKY / bug #400)
						// YAT versions 1.99.70 and 1.99.80 used to take the endianness into account when encoding
						// and decoding multi-byte encoded characters. However, it was always done, but of course e.g.
						// UTF-8 is independent on endianness. The endianness would only have to be applied single
						// multi-byte values, not multi-byte values split into multiple fragments. However, a .NET
						// 'Encoding' object does not tell whether the encoding is potentially endianness capable or
						// not. Thus, it was decided to again remove the character encoding endianness awareness.

						if (((EncodingEx)e).IsUnicode)
						{
							// Note that the following code is similar as above and below but with subtle differences
							// such as no treatment of a lead byte, no treatment of 0xFF, treatment of 0xFFFD, comment,...

							this.rxMultiByteDecodingStream.Add(b);

							int remainingBytesInFragment = (this.rxMultiByteDecodingStream.Count % ((EncodingEx)e).UnicodeFragmentByteCount);
							if (remainingBytesInFragment > 0)
							{
								return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
							}

							byte[] decodingArray = this.rxMultiByteDecodingStream.ToArray();
							int expectedCharCount = e.GetCharCount(decodingArray);
							char[] chars = new char[expectedCharCount];
							int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
							if (effectiveCharCount == 1)
							{
								int code = chars[0];
								if (code != 0xFFFD) // Ensure that 'unknown' character 0xFFFD is not decoded yet.
								{
									this.rxMultiByteDecodingStream.Clear();

									if      ((code < 0x20) || (code == 0x7F))        // ASCII control characters.
									{
										return (base.ByteToElement((byte)code, d, r));
									}
									else if (code == 0x20)                           // ASCII space.
									{
										return (base.ByteToElement((byte)code, d, r));
									}
									else                                             // ASCII printable character.
									{                                                        // 'effectiveCharCount' is 1 for sure.
										return (CreateDataElement(decodingArray, d, r, chars[0]));
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
									this.rxMultiByteDecodingStream.Clear();          // Reset decoding stream.

									return (CreateInvalidBytesWarning(decodingArray, d, e));
								}
							}
							else // (effectiveCharCount > 1) => Code doesn't fit into a single u16 value, thus more than one character will be returned.
							{
								this.rxMultiByteDecodingStream.Clear();              // Reset decoding stream.

								return (CreateOutsideUnicodePlane0Warning(decodingArray, d, e));
							}
						}
						else if ((EncodingEx)e == SupportedEncoding.UTF7)
						{
							// Note that the following code is similar as above and below but with subtle differences
							// such as treatment of Base64 bytes, no treatment of 0xFF, no treatment of 0xFFFD, comment,...

							if (this.rxMultiByteDecodingStream.Count == 0)       // A first 'MultiByte' is either direct or lead byte.
							{
								if      ((b < 0x20) || (b == 0x7F))               // ASCII control characters.
								{
									return (base.ByteToElement(b, d, r));
								}
								else if (b == 0x20)                              // ASCII space.
								{
									return (base.ByteToElement(b, d, r));
								}
								else if (CharEx.IsValidForUTF7((char)b))
								{
									return (DecodeAndCreateElement(b, d, r, e)); // 'IsMultiByte' but the current byte must result in a single character here.
								}
								else if (b == (byte)'+')                         // UTF-7 lead byte.
								{
									this.rxMultiByteDecodingStream.Clear();
									this.rxMultiByteDecodingStream.Add(b);

									return (new DisplayElement.Nonentity());     // Nothing to decode (yet).
								}
								else
								{
									return (CreateInvalidByteWarning(b, d, e));
								}
							}
							else // (rxMultiByteDecodingStream.Count > 0) => Not lead byte.
							{
								if (b == (byte)'-')                              // UTF-7 terminating byte.
								{
									this.rxMultiByteDecodingStream.Add(b);
									byte[] decodingArray = this.rxMultiByteDecodingStream.ToArray();
									this.rxMultiByteDecodingStream.Clear();

									int expectedCharCount = e.GetCharCount(decodingArray);
									char[] chars = new char[expectedCharCount];
									int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
									if (effectiveCharCount == expectedCharCount)
									{
										return (CreateDataElement(decodingArray, d, r, chars));
									}
									else // Decoder has failed:
									{
										return (CreateInvalidBytesWarning(decodingArray, d, e));
									}
								}
								else if (!CharEx.IsValidForBase64((char)b))      // Non-Base64 characters also terminates!
								{
									byte[] decodingArray = this.rxMultiByteDecodingStream.ToArray();
									this.rxMultiByteDecodingStream.Clear();

									int expectedCharCount = e.GetCharCount(decodingArray);
									char[] chars = new char[expectedCharCount];
									int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
									if (effectiveCharCount == expectedCharCount)
									{                                                                         // 'effectiveCharCount' is 1 for sure.
										DisplayElement encoded = CreateDataElement(decodingArray, d, r, chars);

										// Note that the following code is similar as above and below but with subtle differences
										// such as treatment of a lead byte, no treatment of 0xFF, no treatment of 0xFFFD, comment,...

										DisplayElement direct;
										if      ((b < 0x20) || (b == 0x7F))              // ASCII control characters.
										{
											direct = base.ByteToElement(b, d, r);
										}
										else if (b == 0x20)                              // ASCII space.
										{
											direct = base.ByteToElement(b, d, r);
										}
										else if (CharEx.IsValidForUTF7((char)b))
										{
											direct = DecodeAndCreateElement(b, d, r, e); // 'IsMultiByte' but the current byte must result in a single character here.
										}
										else
										{
											return (CreateInvalidByteWarning(b, d, e));
										}

										// Combine into single element, accepting the limitation that a potential control character will be contained in a data element:

										var origin = new List<byte>(decodingArray.Length + 1); // Preset the required capacity to improve memory management.
										origin.AddRange(decodingArray);
										origin.Add(b);

										var text = (encoded.Text + direct.Text);

										switch (d)
										{
											case IODirection.Tx: return (new DisplayElement.TxData(origin.ToArray(), text));
											case IODirection.Rx: return (new DisplayElement.RxData(origin.ToArray(), text));

											default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
										}
									}
									else // Decoder has failed:
									{
										return (CreateInvalidBytesWarning(decodingArray, d, e));
									}
								}
								else if (CharEx.IsValidForUTF7((char)b))     // UTF-7 trailing byte.
								{
									this.rxMultiByteDecodingStream.Add(b);

									return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
								}
								else
								{
									return (CreateInvalidByteWarning(b, d, e));
								}
							} // direct or lead or trailing byte.
						}
						else // Non-Unicode DBCS/MBCS.
						{
							// Note that the following code is similar as several times above but with subtle differences
							// such as treatment of a lead byte, no treatment of 0xFF, no treatment of 0xFFFD, comment,...

							if (this.rxMultiByteDecodingStream.Count == 0)       // A first 'MultiByte' is either ASCII or lead byte.
							{
								if      (b >= 0x80)                              // DBCS/MBCS lead byte.
								{
									this.rxMultiByteDecodingStream.Add(b);

									return (new DisplayElement.Nonentity());     // Nothing to decode (yet).
								}
								else if ((b < 0x20) || (b == 0x7F))              // ASCII control characters.
								{
									return (base.ByteToElement(b, d, r));
								}
								else if (b == 0x20)                              // ASCII space.
								{
									return (base.ByteToElement(b, d, r));
								}
								else                                             // ASCII printable character.
								{
									return (DecodeAndCreateElement(b, d, r, e)); // 'IsMultiByte' but the current byte must result in a single character here.
								}
							}
							else // (rxMultiByteDecodingStream.Count > 0) => Neither ASCII nor lead byte.
							{
								this.rxMultiByteDecodingStream.Add(b);

								byte[] decodingArray = this.rxMultiByteDecodingStream.ToArray();
								int expectedCharCount = e.GetCharCount(decodingArray);
								char[] chars = new char[expectedCharCount];
								int effectiveCharCount = e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true);
								if (effectiveCharCount == 1)
								{
									this.rxMultiByteDecodingStream.Clear();
									                                                    //// 'effectiveCharCount' is 1 for sure.
									return (CreateDataElement(decodingArray, d, r, chars[0]));
								}
								else if (effectiveCharCount == 0)
								{
									if (decodingArray.Length < e.GetMaxByteCount(1))
									{
										return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
									}
									else
									{
										this.rxMultiByteDecodingStream.Clear(); // Reset decoding stream.

										return (CreateInvalidBytesWarning(decodingArray, d, e));
									}
								}
								else // (effectiveCharCount > 1)
								{
									this.rxMultiByteDecodingStream.Clear(); // Reset decoding stream.

									return (CreateInvalidBytesWarning(decodingArray, d, e));
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
		protected virtual DisplayElement DecodeAndCreateElement(byte b, IODirection d, Radix r, Encoding e)
		{
			int expectedCharCount = 1;
			char[] chars = new char[expectedCharCount];
			int effectiveCharCount = e.GetDecoder().GetChars(new byte[] { b }, 0, 1, chars, 0, true);
			if (effectiveCharCount == expectedCharCount)
			{                                            // 'effectiveCharCount' is 1 for sure.
				return (CreateDataElement(b, d, r, chars[0]));
			}
			else // Decoder has failed:
			{
				return (CreateInvalidByteWarning(b, d, e));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte origin, IODirection d, Radix r, char c)
		{
			if (r != Radix.Unicode)
			{
				string text = c.ToString(CultureInfo.InvariantCulture);
				return (CreateDataElement(origin, d, text));
			}
			else // Unicode:
			{
				string text = UnicodeValueToNumericString(c);
				return (CreateDataElement(origin, d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, IODirection d, Radix r, char c)
		{
			if (r != Radix.Unicode)
			{
				string text = c.ToString(CultureInfo.InvariantCulture);
				return (CreateDataElement(origin, d, text));
			}
			else // Unicode:
			{
				string text = UnicodeValueToNumericString(c);
				return (CreateDataElement(origin, d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, IODirection d, Radix r, char[] text)
		{
			if (r != Radix.Unicode)
			{
				return (CreateDataElement(origin, d, new string(text)));
			}
			else // Unicode:
			{
				return (CreateDataElement(origin, d, new string(text)));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateInvalidByteWarning(byte b, IODirection d, Encoding e)
		{
			var byteAsString = ByteHelper.FormatHexString(b, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append("'");
			sb.Append(byteAsString);
			sb.Append("' is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte!");

			return (new DisplayElement.ErrorInfo((Direction)d, sb.ToString(), true));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateInvalidBytesWarning(byte[] a, IODirection d, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@"""");
			sb.Append(bytesAsString);
			sb.Append(@""" is an invalid '");
			sb.Append(((EncodingEx)e).DisplayName);
			sb.Append("' byte sequence!");

			return (new DisplayElement.ErrorInfo((Direction)d, sb.ToString(), true));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateOutsideUnicodePlane0Warning(byte[] a, IODirection d, Encoding e)
		{
			var bytesAsString = ByteHelper.FormatHexString(a, TerminalSettings.Display.ShowRadix);

			var sb = new StringBuilder();
			sb.Append(@"Byte sequence """);
			sb.Append(bytesAsString);
			sb.Append(@""" is outside the basic multilingual plane (plane 0) which is not yet supported but tracked as feature request #329.");

			return (new DisplayElement.ErrorInfo((Direction)d, sb.ToString(), true));
		}

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		private void ExecuteLineBegin(LineState lineState, DateTime ts, string dev, IODirection dir, DisplayElementCollection elementsToAdd)
		{
			if (this.bidirLineState.IsFirstLine) // Properly initialize the time delta:
				this.bidirLineState.LastLineTimeStamp = ts;

			var lp = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.

			lp.Add(new DisplayElement.LineStart()); // Direction may be both!

			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowDevice    ||
			    TerminalSettings.Display.ShowDirection)
			{
				DisplayElementCollection info;
				PrepareLineBeginInfo(ts, (ts - InitialTimeStamp), (ts - this.bidirLineState.LastLineTimeStamp), dev, dir, out info);
				lp.AddRange(info);
			}

			if (lineState.SuppressForSure || lineState.SuppressIfSubsequentlyTriggered || lineState.SuppressIfNotFiltered)
			{
				lineState.Elements.AddRange(lp); // No clone needed as elements are not needed again.
			////elementsToAdd.AddRange(lp) shall not be done for (potentially) suppressed element. Doing so would lead to unnecessary flickering.
			}
			else
			{
				lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elementsToAdd.AddRange(lp);
			}

			lineState.Position  = LinePosition.Content;
			lineState.TimeStamp = ts;
			lineState.Device    = dev;
		}

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void ExecuteContent(Settings.TextDisplaySettings displaySettings, LineState lineState, string dev, IODirection dir, byte b, DisplayElementCollection elementsToAdd, out bool replaceAlreadyStartedLine)
		{
			// Convert content:
			DisplayElement de;
			bool isBackspace;
			if (!ControlCharacterHasBeenExecuted(b, dir, out de, out isBackspace))
				de = ByteToElement(b, dir); // Default conversion to value or ASCII mnemonic.

			de.Highlight = lineState.Highlight;

			var lp = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.

			// Prepare EOL:
			if (!lineState.EolOfGivenDevice.ContainsKey(dev))                                  // It is OK to only access or add to the collection,
				lineState.EolOfGivenDevice.Add(dev, new SequenceQueue(lineState.EolSequence)); // this will not lead to excessive use of memory,
			                                                                                   // since there is only a given number of devices.
			// Add byte to EOL:                                                                // Applies to TCP and UDP terminals only.
			lineState.EolOfGivenDevice[dev].Enqueue(b);

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started:
			if (lineState.EolOfGivenDevice[dev].IsCompleteMatch)
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

						ConfirmRetainedUnconfirmedHiddenEolElements(lineState);
					}

					lineState.Position = LinePosition.End;
				}
				else
				{
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
				}
			}                                                                // Note the inverted implementation sequence:
			else if (lineState.EolOfGivenDevice[dev].IsPartlyMatchContinued) //  1. CompleteMatch        (last trigger, above)
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
						lineState.RetainedUnconfirmedHiddenEolElements.Add(de); // No clone needed as element is no more used below.
					}
				}
				else
				{
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
				}
			}
			else if (lineState.EolOfGivenDevice[dev].IsPartlyMatchBeginning)
			{
				// Previous was no match, retained potential EOL elements can be treated as non-EOL:
				ReleaseRetainedUnconfirmedHiddenEolElements(lineState, lp);

				if (de.IsContent)
				{
					if (TextTerminalSettings.ShowEol)
					{
						AddSpaceIfNecessary(lineState, dir, lp, de);
						lp.Add(de.Clone()); // No clone needed as element is no more used below.
					}
					else
					{
						lineState.RetainedUnconfirmedHiddenEolElements.Add(de); // No clone needed as element is no more used below.

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
				ReleaseRetainedUnconfirmedHiddenEolElements(lineState, lp);

				// Add the current element, which for sure is not related to EOL:
				AddSpaceIfNecessary(lineState, dir, lp, de);
				lp.Add(de); // No clone needed as element has just been created further above.
			}

			replaceAlreadyStartedLine = false;

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

				if (lineState.SuppressForSure || lineState.SuppressIfSubsequentlyTriggered || lineState.SuppressIfNotFiltered)
				{
					lineState.Elements.AddRange(lp); // No clone needed as elements are not needed again.
				////elementsToAdd.AddRange(lp) shall not be done for (potentially) suppressed element. Doing so would lead to unnecessary flickering.
				}
				else
				{
					lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
					elementsToAdd.AddRange(lp);
				}

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
							elementsToAdd.Clear();
							replaceAlreadyStartedLine = true;
						}

						// Attention:
						//
						// Setting 'replaceAlreadyStartedLine' to 'true' will instruct the caller to
						// call OnCurrentDisplayLineReplaced(). However, that method will be called
						// *before* 'elementsToAdd' will get added by OnDisplayElement[s]Added() !!
						//
						// So, if 'elementsToAdd' contains a character to remove, it can be removed
						// there and the current line does not need to be replaced.
					}
				}
			}

			// Only continue evaluation if no line break detected yet (cannot have more than one line break).
			if ((displaySettings.LengthLineBreak.Enabled) &&
				(lineState.Position != LinePosition.End))
			{
				if (lineState.Elements.CharCount >= displaySettings.LengthLineBreak.Length)
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
					lineState.Elements.Add(new DisplayElement.ErrorInfo((Direction)dir, message, true));
					elementsToAdd.Add(     new DisplayElement.ErrorInfo((Direction)dir, message, true));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual bool ControlCharacterHasBeenExecuted(byte b, IODirection dir, out DisplayElement de, out bool isBackspace)
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

						de = CreateDataElement(b, dir, "\t");
						return (true);
					}

					break;
				}
			}

			de = null;
			return (false);
		}

		private void AddSpaceIfNecessary(LineState lineState, IODirection dir, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(dir) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.CharCount > 0) || (lp.CharCount > 0))
					lp.Add(new DisplayElement.DataSpace());
			}
		}

		private void RemoveSpaceIfNecessary(IODirection dir, DisplayElementCollection lp)
		{
			if (ElementsAreSeparate(dir))
			{
				int count = lp.Count;
				if ((count > 0) && (lp[count - 1] is DisplayElement.DataSpace))
					lp.RemoveLast();
			}
		}

		private static void ConfirmRetainedUnconfirmedHiddenEolElements(LineState lineState)
		{
			if (lineState.RetainedUnconfirmedHiddenEolElements.Count > 0)
			{
				lineState.RetainedUnconfirmedHiddenEolElements.Clear();
			}
		}

		private static void ReleaseRetainedUnconfirmedHiddenEolElements(LineState lineState, DisplayElementCollection lp)
		{
			if (lineState.RetainedUnconfirmedHiddenEolElements.Count > 0)
			{
				lp.AddRange(lineState.RetainedUnconfirmedHiddenEolElements); // No clone needed as collection is cleared below.
				lineState.RetainedUnconfirmedHiddenEolElements.Clear();
			}
		}

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		private void ExecuteLineEnd(LineState lineState, DateTime ts, string dev, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			// Note: Code sequence the same as ExecuteLineEnd() of BinaryTerminal for better comparability.

			bool isEmptyLine    = ( lineState.Elements.CharCount == 0);
			bool isPendingEol   = (!lineState.EolOfLastLineWasCompleteMatch(dev) &&  lineState.EolIsAnyMatch(dev));
			bool isNotHiddenEol = ( lineState.EolOfLastLineWasCompleteMatch(dev) && !lineState.EolIsAnyMatch(dev));
			if (isEmptyLine && isPendingEol) // While intended empty lines must be shown, potentially suppress
			{                                // empty lines that only contain hidden pending EOL character(s):
				elementsToAdd.RemoveAtEndUntil(typeof(DisplayElement.LineStart)); // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                  //            All other elements must be removed as well!
				clearAlreadyStartedLine = true;                                   //            This is signaled by setting 'clearAlreadyStartedLine'.
			}
			else if (isEmptyLine && isNotHiddenEol) // While intended empty lines must be shown, potentially suppress
			{                                       // empty lines that only contain hidden non-EOL character(s) (e.g. hidden 0x00):
				elementsToAdd.RemoveAtEndUntil(typeof(DisplayElement.LineStart)); // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                  //            All other elements must be removed as well!
				clearAlreadyStartedLine = true;                                   //            This is signaled by setting 'clearAlreadyStartedLine'.
			}
			else if (lineState.SuppressForSure || (lineState.SuppressIfNotFiltered && !lineState.AnyFilterDetected)) // Suppress line:
			{
			#if (DEBUG)
				// As described in 'ProcessRawChunk()', the current implementation retains the line until it is
				// complete, i.e. until the final decision to filter or suppress could be done. As a consequence,
				// the 'clearAlreadyStartedLine' can never get activated, thus excluding it (YAGNI).
				// Still, keeping the implementation to be prepared for potential reactivation (!YAGNI).

				elementsToAdd.RemoveAtEndUntil(typeof(DisplayElement.LineStart)); // Attention: 'elementsToAdd' likely doesn't contain all elements since line start!
				                                                                  //            All other elements must be removed as well!
				clearAlreadyStartedLine = true;                                   //            This is signaled by setting 'clearAlreadyStartedLine'.
			#endif
			}
			else
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
				lineEnd.Add(new DisplayElement.LineBreak()); // Direction may be both!

				// Finalize elements:
				if ((lineState.SuppressIfSubsequentlyTriggered && !lineState.SuppressForSure) ||    // Don't suppress line!
				    (lineState.SuppressIfNotFiltered && lineState.FilterDetectedInSubsequentChunk)) // Filter line!
				{                                                                                   // Both cases mean to delay-show the elements of the line.
					elementsToAdd.AddRange(lineState.Elements.Clone()); // Clone elements because they are needed again further below.
				}
				elementsToAdd.AddRange(lineEnd.Clone()); // Clone elements because they are needed again right below.

				// Finalize line:                // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
				var l = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
				l.AddRange(lineState.Elements); // No clone needed as elements are no more used and will be reset below.
				l.AddRange(lineEnd);
				l.TimeStamp = lineState.TimeStamp;
				linesToAdd.Add(l);
			}

			this.bidirLineState.IsFirstLine = false;
			this.bidirLineState.LastLineTimeStamp = lineState.TimeStamp;

			// Reset line state:
			lineState.Reset(dev, lineState.EolIsCompleteMatch(dev));
		}

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		private void ExecuteTimedLineBreakOnReload(Settings.TextDisplaySettings displaySettings, LineState lineState, DateTime ts, string dev,
		                                           DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			if (lineState.Elements.Count > 0)
			{
				var span = ts - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout)
					ExecuteLineEnd(lineState, ts, dev, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
			}

			lineState.TimeStamp = ts;
		}

		/// <summary></summary>
		protected virtual void ProcessRawByte(RawChunk chunk, LineChunkAttribute attribute)
		{
			lock (ChunkVsTimeoutSyncObj) // Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
			{
				// In case of reload, timed line breaks are executed here:
				if (IsReloading && displaySettings.TimedLineBreak.Enabled)
					EvaluateTimedLineBreakOnReload(displaySettings, lineState, chunk.TimeStamp, chunk.Device, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);

				// Line begin and time stamp:
				if (lineState.Position == LinePosition.Begin)
				{
					DoLineBegin(lineState, chunk.TimeStamp, chunk.Device, chunk.Direction, elementsToAdd);

					if (displaySettings.TimedLineBreak.Enabled)
						lineState.BreakTimeout.Start();
				}
				else
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.BreakTimeout.Restart(); // Restart as timeout refers to time after last received byte.
				}

				// Content:
				if (lineState.Position == LinePosition.Content)
				{
					DoContent(displaySettings, lineState, chunk.Device, chunk.Direction, b, elementsToAdd, out replaceAlreadyStartedLine);
				}

				// Line end and length:
				if (lineState.Position == LinePosition.End)
				{
					if (displaySettings.TimedLineBreak.Enabled)
						lineState.BreakTimeout.Stop();

					DoLineEnd(lineState, chunk.TimeStamp, chunk.Device, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
				}
			}
		}


		/// <summary></summary>
		protected override void ProcessRawChunk(RawChunk chunk, LineChunkAttribute attribute, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			lock (this.processSyncObj) // Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
			{
				Settings.TextDisplaySettings displaySettings;
				switch (chunk.Direction)
				{
					case IODirection.Tx: displaySettings = TextTerminalSettings.TxDisplay; break;
					case IODirection.Rx: displaySettings = TextTerminalSettings.RxDisplay; break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + chunk.Direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				LineState lineState;
				switch (chunk.Direction)
				{
					case IODirection.Tx: lineState = this.txLineState; break;
					case IODirection.Rx: lineState = this.rxLineState; break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + chunk.Direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				// Activate flags as needed, leave unchanged otherwise.
				// Note that each chunk will either have none or only have a single attribute activated.
				// But the line state has to deal with multiple chunks, thus multiples attribute may get activated.
				// Also note the limitations described in feature request #366 "Automatic response and action shall be...".
				if (attribute == LineChunkAttribute.Highlight)                       {                                                                                     lineState.Highlight                        = true;                                                                }
				if (attribute == LineChunkAttribute.Filter)                          { if (!lineState.AnyFilterDetected) { if (lineState.Position == LinePosition.Begin) { lineState.FilterDetectedInFirstChunkOfLine = true; } else { lineState.FilterDetectedInSubsequentChunk = true; } } }
				if (attribute == LineChunkAttribute.SuppressIfNotFiltered)           { if (!lineState.AnyFilterDetected) {                                                 lineState.SuppressIfNotFiltered            = true;                                                              } }
				if (attribute == LineChunkAttribute.SuppressIfSubsequentlyTriggered) {                                                                                     lineState.SuppressIfSubsequentlyTriggered  = true;                                                                }
				if (attribute == LineChunkAttribute.Suppress)                        {                                                                                     lineState.SuppressForSure                  = true;                                                                }

				// In both cases, filtering and suppression, the current implementation retains the line until it is
				// complete, i.e. until the final decision to filter or suppress could be done. This behavior differs
				// from the standard behavior which continuously shows data as it is coming in.
				//
				// Why this retaining approach? It would be possible to immediately display but then remove the line if it
				// is suppressed or not filtered. But that likely leads to flickering, thus the retaining approach. At the
				// price that there is no longer immediate feedback on single character transmission in case filtering or
				// suppression is active, except in case of filtering when the first chunk of a line already contains the
				// trigger, then the line is continuously shown ('FilterDetectedInFirstChunkOfLine').
				//
				// The test cases of [YAT - Test.ods]::[YAT.Model.Terminal] demonstrate the retaining approach.
				//
				// To change from retaining to continuous approach, the #if (DEBUG) around 'clearAlreadyStartedLine' will
				// have to be removed again. As a consequence, the flag can never get activated, thus excluding it (YAGNI).
				// Still, keeping the implementation to be prepared for potential reactivation (!YAGNI).
				//
				// Note that logging works fine even when filtering or suppression is active, since logging is only
				// triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus not affected by the more tricky to
				// handle 'CurrentDisplayLine[Tx|Bidir|Rx]Replaced' and 'CurrentDisplayLine[Tx|Bidir|Rx]Cleared' events.

				foreach (byte b in chunk.Content)
				{
					// In case of reload, timed line breaks are executed here:
					if (IsReloading && displaySettings.TimedLineBreak.Enabled)
						ExecuteTimedLineBreakOnReload(displaySettings, lineState, chunk.TimeStamp, chunk.Device, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);

					// Line begin and time stamp:
					if (lineState.Position == LinePosition.Begin)
					{
						ExecuteLineBegin(lineState, chunk.TimeStamp, chunk.Device, chunk.Direction, elementsToAdd);

						if (displaySettings.TimedLineBreak.Enabled)
							lineState.BreakTimeout.Start();
					}
					else
					{
						if (displaySettings.TimedLineBreak.Enabled)
							lineState.BreakTimeout.Restart(); // Restart as timeout refers to time after last received byte.
					}

					// Content:
					if (lineState.Position == LinePosition.Content)
					{
						bool replaceAlreadyStartedLine;

						ExecuteContent(displaySettings, lineState, chunk.Device, chunk.Direction, b, elementsToAdd, out replaceAlreadyStartedLine);

						if (replaceAlreadyStartedLine)
							OnCurrentDisplayLineReplaced(chunk.Direction, lineState.Elements.Clone()); // Clone the ensure decoupling.
					}

					// Line end and length:
					if (lineState.Position == LinePosition.End)
					{
						if (displaySettings.TimedLineBreak.Enabled)
							lineState.BreakTimeout.Stop();

						ExecuteLineEnd(lineState, chunk.TimeStamp, chunk.Device, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
					}
				} // foreach (byte)
			} // lock (processSyncObj)
		}

	#if (WITH_SCRIPTING)

		/// <remarks>
		/// Processing for scripting differs from "normal" processing for displaying because...
		/// ...received messages must not be impacted by 'DirectionLineBreak'.
		/// ...received data must not be processed individually, only as packets/messages.
		/// ...received data must not be reprocessed on <see cref="RefreshRepositories"/>.
		/// </remarks>
		protected override void ProcessAndSignalRawChunkForScripting(RawChunk chunk)
		{
			if (chunk.Direction == IODirection.Rx)
			{
				bool hideXOnXOff = (TerminalSettings.IO.SerialPort.Communication.FlowControlManagesXOnXOffManually &&
				                    TerminalSettings.CharHide.HideXOnXOff);

				foreach (byte b in chunk.Content)
				{
					var isXOnXOffChar = ((b == 0x11) || (b == 0x13));
					if (isXOnXOffChar && hideXOnXOff)
						continue; // Do nothing, ignore the character.

					this.rxScriptMessageState.Data.Add(b);
					this.rxScriptMessageState.Eol.Enqueue(b);

					if (this.rxScriptMessageState.Eol.IsCompleteMatch) // Processing and signalling
						ProcessAndSignalMessageForScripting();         // in case EOL != <nothing>.
				}
			}
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalMessageForScripting()
		{
			// Compose packet data:
			var data = this.rxScriptMessageState.Data.ToArray();

			// Remove EOL from data, as a script message shall not contain the EOL:
			int eolLength = (this.rxScriptMessageState.EolSequence.Length);
			int eolIndex  = (this.rxScriptMessageState.Data.Count - eolLength);
			this.rxScriptMessageState.Data.RemoveRange(eolIndex, eolLength);

			// Compose formatted message. Message format for scripting is fixed:
			// "For text terminals, received messages are decoded strings, using the configured encoding."
			var message = Format(this.rxScriptMessageState.Data.ToArray(), IODirection.Rx, Radix.String);

			// Notify:
			EnqueueReceivedMessageForScripting(message); // Enqueue before invoking event to
			                                             // have message ready for event.
			OnScriptPacketReceived(new PacketEventArgs(data));
			OnScriptMessageReceived(new MessageEventArgs(message));

			// Reset:
			this.rxScriptMessageState.Reset();
		}

	#endif // WITH_SCRIPTING

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
