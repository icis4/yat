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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

using MKY;
using MKY.Text;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Text protocol terminal.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class TextTerminal : Terminal
	{
		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
		//==========================================================================================

		/// <summary></summary>
		public static readonly string KeywordHelp =
			@"For text terminals, the following additional keywords are supported :" + Environment.NewLine +
			Environment.NewLine +
			@"Send EOL sequence ""OK\!(" + (Parser.KeywordEx)Parser.Keyword.Eol + @")""" + Environment.NewLine +
			@"Do not send EOL ""OK\!(" + (Parser.KeywordEx)Parser.Keyword.NoEol + @")""" + Environment.NewLine +
			@"""\!(" + (Parser.KeywordEx)Parser.Keyword.NoEol + @")"" is useful if your text protocol does have an EOL sequence" + Environment.NewLine +
			@"  except for a few special commands (e.g. synchronization commands).";

		#endregion

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
			Data,
			DataExceeded,
			End
		}

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
		private class LineState
		{
			public LinePosition    LinePosition;
			public DisplayLinePart LineElements;
			public DisplayLinePart EolElements;
			public SequenceQueue   Eol;

			public LineState(SequenceQueue eol)
			{
				LinePosition = LinePosition.Begin;
				LineElements = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				EolElements  = new DisplayLinePart(); // Default behavior regarding initial capacity is OK.
				Eol = eol;
			}

			public virtual void Reset()
			{
				LinePosition = LinePosition.Begin;
				LineElements = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				EolElements  = new DisplayLinePart(); // Default behavior regarding initial capacity is OK.
				Eol.Reset();
			}
		}

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
		private class BidirLineState
		{
			public bool IsFirstLine;
			public string PortStamp;
			public IODirection Direction;

			public BidirLineState()
			{
				IsFirstLine = true;
				PortStamp   = null;
				Direction   = IODirection.None;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstLine = rhs.IsFirstLine;
				PortStamp   = rhs.PortStamp;
				Direction   = rhs.Direction;
			}
		}

		#endregion

		#region Types > Line Send Delay
		//------------------------------------------------------------------------------------------
		// Types > Line Send Delay
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
		private class LineSendDelayState
		{
			public int LineCount;

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

		private BidirLineState bidirLineState;

		private LineSendDelayState lineSendDelayState;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TextTerminal(Settings.TerminalSettings settings)
			: base(settings)
		{
			AttachTextTerminalSettings();
			InitializeStates();
		}

		/// <summary></summary>
		public TextTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachTextTerminalSettings();

			var casted = (terminal as TextTerminal);
			if (casted != null)
			{
				this.rxMultiByteDecodingStream = casted.rxMultiByteDecodingStream;
				this.txLineState               = casted.txLineState;
				this.rxLineState               = casted.rxLineState;

				this.bidirLineState = new BidirLineState(casted.bidirLineState);

				this.lineSendDelayState = new LineSendDelayState(casted.lineSendDelayState);
			}
			else
			{
				InitializeStates();
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					DetachTextTerminalSettings();
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private Settings.TextTerminalSettings TextTerminalSettings
		{
			get
			{
				if (TerminalSettings != null)
					return (TerminalSettings.TextTerminal);
				else
					return (null);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > Open
		//------------------------------------------------------------------------------------------
		// Methods > Open
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public override bool Start()
		{
			bool success = base.Start();

			if (success)
				this.lineSendDelayState.Reset();

			return (success);
		}

		#endregion

		#region Methods > Parse
		//------------------------------------------------------------------------------------------
		// Methods > Parse
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Tries to parse <paramref name="s"/>, taking the current settings into account.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public override bool TryParse(string s, out byte[] result, Radix defaultRadix = Radix.String)
		{
			using (Parser.SubstitutionParser p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, TerminalSettings.Send.ToParseMode()))
				return (p.TryParse(s, out result, defaultRadix));
		}

		#endregion

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void ProcessParsableSendItem(ParsableSendItem item)
		{
			string textToParse = item.Data;

			// Check for EOL comment indicators:
			if (TextTerminalSettings.EolComment.SkipComment)
			{
				foreach (string marker in TextTerminalSettings.EolComment.Indicators)
				{
					int index = StringEx.IndexOfOutsideDoubleQuotes(textToParse, marker, StringComparison.Ordinal);
					if (index >= 0)
					{
						textToParse = StringEx.Left(textToParse, index);

						if (TextTerminalSettings.EolComment.SkipWhiteSpace)
							textToParse = textToParse.TrimEnd(null); // 'null' means white-spaces.
					}
				}
			}

			// Parse the item string:
			bool hasSucceeded;
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;

			using (Parser.SubstitutionParser p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, TerminalSettings.Send.ToParseMode()))
				hasSucceeded = p.TryParse(textToParse, out parseResult, out textSuccessfullyParsed, item.DefaultRadix);

			if (hasSucceeded)
				ProcessParserResult(parseResult, item.IsLine);
			else
				OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(textToParse, textSuccessfullyParsed)));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		protected override void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Eol:
				{
					ForwardDataToRawTerminal(this.txLineState.Eol.Sequence);
					break;
				}

				default:
				{
					base.ProcessInLineKeywords(result);
					break;
				}
			}
		}

		/// <summary></summary>
		protected override void ProcessLineEnd(bool sendEol)
		{
			if (sendEol)
				ForwardDataToRawTerminal(this.txLineState.Eol.Sequence);
		}

		/// <summary></summary>
		protected override int ProcessLineDelayOrInterval(bool performLineDelay, bool performLineInterval, DateTime lineBeginTimeStamp, DateTime lineEndTimeStamp)
		{
			int accumulatedLineDelay = base.ProcessLineDelayOrInterval(performLineDelay, performLineInterval, lineBeginTimeStamp, lineEndTimeStamp);

			if (TerminalSettings.TextTerminal.LineSendDelay.Enabled)
			{
				this.lineSendDelayState.LineCount++;
				if (this.lineSendDelayState.LineCount >= TerminalSettings.TextTerminal.LineSendDelay.LineInterval)
				{
					this.lineSendDelayState.Reset();

					int delay = TerminalSettings.TextTerminal.LineSendDelay.Delay;
					if (delay > accumulatedLineDelay)
					{
						delay -= accumulatedLineDelay;
						Thread.Sleep(delay);
						accumulatedLineDelay += delay;
					}
				}
			}

			return (accumulatedLineDelay);
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			this.rxMultiByteDecodingStream = new List<byte>(4); // Preset the initial capacity to improve memory management, 4 is the maximum value for multi-byte characters.

			byte[] txEol;
			byte[] rxEol;
			using (Parser.SubstitutionParser p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, Parser.Modes.AllByteArrayResults))
			{
				if (!p.TryParse(TextTerminalSettings.TxEol, out txEol))
				{
					// In case of an invalid EOL sequence, default it. This should never happen,
					// YAT verifies the EOL sequence when the user enters it. However, the user might
					// manually edit the EOL sequence in a settings file.
					TextTerminalSettings.TxEol = Settings.TextTerminalSettings.DefaultEol;
					txEol = p.Parse(TextTerminalSettings.TxEol);
				}
				if (!p.TryParse(TextTerminalSettings.RxEol, out rxEol))
				{
					// In case of an invalid EOL sequence, default it. This should never happen,
					// YAT verifies the EOL sequence when the user enters it. However, the user might
					// manually edit the EOL sequence in a settings file.
					TextTerminalSettings.RxEol = Settings.TextTerminalSettings.DefaultEol;
					rxEol = p.Parse(TextTerminalSettings.RxEol);
				}
			}
			this.txLineState = new LineState(new SequenceQueue(txEol));
			this.rxLineState = new LineState(new SequenceQueue(rxEol));

			this.bidirLineState = new BidirLineState();

			this.lineSendDelayState = new LineSendDelayState();
		}

		/// <summary></summary>
		protected override DisplayElement ByteToElement(byte b, IODirection d, Radix r)
		{
			switch (r)
			{
				// Bin/Oct/Dec/Hex:
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					return (base.ByteToElement(b, d, r));
				}

				// Char/String/Unicode:
				case Radix.Char:
				case Radix.String:
				case Radix.Unicode:
				{
					Encoding e = (EncodingEx)TextTerminalSettings.Encoding;
					if (e.IsSingleByte)
					{
						if ((b < 0x20) || (b == 0x7F)) // Control chars.
						{
							return (base.ByteToElement(b, d, r));
						}
						else if (b == 0x20) // Space.
						{
							return (base.ByteToElement(b, d, r));
						}
						else if ((b == 0xFF) && TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
						{
							return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
						}
						else
						{
							char[] c = new char[1]; // 'IsSingleByte'.
							if (e.GetDecoder().GetChars(new byte[] { b }, 0, 1, c, 0, true) == 1)
							{
								if (r != Radix.Unicode)
								{
									switch (d)
									{
										case IODirection.Tx: return (new DisplayElement.TxData(b, c[0].ToString(CultureInfo.InvariantCulture))); // 'IsSingleByte'.
										case IODirection.Rx: return (new DisplayElement.RxData(b, c[0].ToString(CultureInfo.InvariantCulture))); // 'IsSingleByte'.
										default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
									}
								}
								else // Unicode:
								{
									switch (d)
									{
										case IODirection.Tx: return (new DisplayElement.TxData(b, UnicodeValueToNumericString(c[0]))); // 'IsSingleByte'.
										case IODirection.Rx: return (new DisplayElement.RxData(b, UnicodeValueToNumericString(c[0]))); // 'IsSingleByte'.
										default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
									}
								}
							}
							else // Something went seriously wrong!
							{
								throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "byte " + b.ToString("X2", CultureInfo.InvariantCulture) + " is invalid for encoding " + ((EncodingEx)e).ToString() + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
					}
					else // MultiByte
					{
						this.rxMultiByteDecodingStream.Add(b);
						byte[] decodingArray = this.rxMultiByteDecodingStream.ToArray();

						if (!((EndiannessEx)TerminalSettings.IO.Endianness).IsSameAsMachine)
							decodingArray = decodingArray.Reverse().ToArray();

						int charCount = e.GetCharCount(decodingArray, 0, decodingArray.Length);

						// If decoding array can be decoded into something useful, decode it.
						if (charCount == 1)
						{
							char[] c = new char[1]; // 'charCount' is 1.
							if (e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, c, 0, true) == 1)
							{
								// Ensure that 'unknown' character 0xFFFD is not decoded yet.
								int code = c[0];
								if (code != 0xFFFD)
								{
									this.rxMultiByteDecodingStream.Clear();

									if ((code < 0x20) || (code == 0x7F)) // Control chars.
									{
										return (base.ByteToElement(b, d, r));
									}
									else if (code == 0x20) // Space.
									{
										return (base.ByteToElement(b, d, r));
									}
									else if ((code == 0xFF) && TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
									{
										return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
									}
									else
									{
										if (r != Radix.Unicode)
										{
											switch (d)
											{                                                                     // 'charCount' is 1.
												case IODirection.Tx: return (new DisplayElement.TxData(decodingArray, c[0].ToString(CultureInfo.InvariantCulture), decodingArray.Length));
												case IODirection.Rx: return (new DisplayElement.RxData(decodingArray, c[0].ToString(CultureInfo.InvariantCulture), decodingArray.Length));
												default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
											}
										}
										else // Unicode:
										{
											switch (d)
											{                                                                                                 // 'charCount' is 1.
												case IODirection.Tx: return (new DisplayElement.TxData(decodingArray, UnicodeValueToNumericString(c[0]), decodingArray.Length));
												case IODirection.Rx: return (new DisplayElement.RxData(decodingArray, UnicodeValueToNumericString(c[0]), decodingArray.Length));
												default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
											}
										}
									}
								}
								else
								{
									// 'unknown' character 0xFFFD, do not reset stream.
								}
							}
						}
						else
						{
							// Nothing useful to decode into, reset stream.
							this.rxMultiByteDecodingStream.Clear();
						}

						// Nothing to decode (yet).
						return (new DisplayElement.Nonentity());
					} // MultiByte
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is an invalid radix!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		private void ExecuteLineBegin(LineState lineState, DateTime ts, string ps, IODirection d, DisplayElementCollection elements)
		{
			DisplayLinePart lp = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			lp.Add(new DisplayElement.LineStart()); // Direction may be both!

			if (TerminalSettings.Display.ShowDate || TerminalSettings.Display.ShowTime ||
				TerminalSettings.Display.ShowPort || TerminalSettings.Display.ShowDirection)
			{
				DisplayLinePart info;
				PrepareLineBeginInfo(ts, ps, d, out info);
				lp.AddRange(info);
			}

			lineState.LineElements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
			elements.AddRange(lp);

			lineState.LinePosition = LinePosition.Data;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void ExecuteData(LineState lineState, IODirection d, byte b, DisplayElementCollection elements)
		{
			var lp = new DisplayLinePart(); // Default behavior regarding initial capacity is OK.

			// Convert data:
			var de = ByteToElement(b, d);

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started:
			lineState.Eol.Enqueue(b);
			if (lineState.Eol.IsCompleteMatch)
			{
				if (TextTerminalSettings.ShowEol)
				{
					if (de.IsData)
						lineState.EolElements.Add(de); // No clone needed as element has just been created.

					// Normal case, EOL consists of a single sequence of control characters:
					if ((lineState.EolElements.Count == 1) && (lineState.EolElements[0].OriginCount == lineState.Eol.Sequence.Length))
					{
						// Unfold the elements into single elements for correct processing:
						var l = new List<DisplayElement>(lineState.EolElements.DataCount); // Preset the required capacity to improve memory management.
						foreach (var item in lineState.EolElements)
						{
							foreach (var originItem in item.Origin)
								l.Add(item.RecreateFromOriginItem(originItem));
						}

						// Add them as separate items:
						foreach (var item in l)
						{
							AddSpaceIfNecessary(lineState, d, lp);
							lp.Add(item); // No clone needed as all items have just been recreated futher above.
						}
					}
					else
					{
						// Ensure that only as many elements as EOL contains are marked as EOL.
						// Note that sequence might look like <CR><CR><LF>, only the last two are EOL!
					
						// Unfold the elements into single elements for correct processing:
						var l = new List<DisplayElement>(lineState.EolElements.DataCount); // Preset the required capacity to improve memory management.
						foreach (var item in lineState.EolElements)
						{
							foreach (var originItem in item.Origin)
								l.Add(item.RecreateFromOriginItem(originItem));
						}

						// Count data:
						int dataCount = 0;
						foreach (var item in l)
							dataCount += item.DataCount;

						// Mark only true EOL elements as EOL:
						int firstEolIndex = dataCount - lineState.Eol.Sequence.Length;
						int currentIndex = 0;
						foreach (var item in l)
						{
							currentIndex += item.DataCount;

							if (currentIndex > firstEolIndex)
								item.IsEol = true;

							AddSpaceIfNecessary(lineState, d, lp);
							lp.Add(item); // No clone needed as all items have just been recreated futher above.
						}
					}
				}

				lineState.EolElements.Clear();
				lineState.LinePosition = LinePosition.End;
			}
			else if (lineState.Eol.IsPartlyMatchContinued)
			{
				// Keep EOL elements and delay them until EOL is complete:
				if (de.IsData)
					lineState.EolElements.Add(de); // No clone needed as element has just been created further above.
			}
			else if (lineState.Eol.IsPartlyMatchBeginning)
			{
				// Previous was no match, previous EOL elements can be treated as normal:
				TreatEolAsNormal(lineState, lp);

				// Keep EOL elements and delay them until EOL is complete:
				if (de.IsData)
					lineState.EolElements.Add(de); // No clone needed as element has just been created further above.
			}
			else
			{
				// No match at all, previous EOL elements can be treated as normal:
				TreatEolAsNormal(lineState, lp);

				// Add non-EOL element:
				AddSpaceIfNecessary(lineState, d, lp);
				lp.Add(de); // No clone needed as element has just been created further above.
			}

			if (lineState.LinePosition != LinePosition.DataExceeded)
			{
				lineState.LineElements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elements.AddRange(lp);
			}

			// Only continue evaluation if no line break detected yet (cannot have more than one line break).
			if (lineState.LinePosition != LinePosition.End)
			{
				if ((lineState.LineElements.DataCount >= TerminalSettings.Display.MaxBytePerLineCount) &&
					(lineState.LinePosition != LinePosition.DataExceeded))
				{
					lineState.LinePosition = LinePosition.DataExceeded;

					string message = "Maximal number of bytes per line exceeded! Check the end-of-line settings or increase the limit in the advanced terminal settings.";
					lineState.LineElements.Add(new DisplayElement.ErrorInfo((Direction)d, message, true));
					elements.Add              (new DisplayElement.ErrorInfo((Direction)d, message, true));
				}
			}
		}

		private void AddSpaceIfNecessary(LineState lineState, IODirection d, DisplayLinePart lp)
		{
			if (ElementsAreSeparate(d))
			{
				if (lineState.LineElements.DataCount > 0)
					lp.Add(new DisplayElement.DataSpace());
			}
		}

		private static void TreatEolAsNormal(LineState lineState, DisplayLinePart lp)
		{
			if (lineState.EolElements.Count > 0)
			{
				lp.AddRange(lineState.EolElements.Clone()); // Clone elements to ensure decoupling.
				lineState.EolElements.Clear();
			}
		}

		private void ExecuteLineEnd(LineState lineState, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			// Note: Code sequence the same as ExecuteLineEnd() of BinaryTerminal for better comparability.

			DisplayLine line = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			// Process line content:
			int eolLength = lineState.Eol.Sequence.Length;
			if (TextTerminalSettings.ShowEol || (eolLength <= 0) || (!lineState.Eol.IsCompleteMatch))
			{
				line.AddRange(lineState.LineElements.Clone()); // Clone elements to ensure decoupling.
			}
			else // Remove EOL:
			{
				// Traverse elements reverse and count EOL elements to be removed:
				int eolCount = 0;
				DisplayElement[] des = lineState.LineElements.Clone().ToArray(); // Clone elements to ensure decoupling.
				for (int i = (des.Length - 1); i >= 0; i--)
				{
					if (des[i].IsEol)
						eolCount++;
					else
						break; // Break at last non-EOL element.
				}

				// Now, traverse elements forward and add elements to line:
				for (int i = 0; i < (des.Length - eolCount); i++)
					line.Add(des[i]); // No clone needed as items have just been cloned futher above.

				// Finally, remove EOL from elements:
				if (elements.Count >= eolCount)
					elements.RemoveAtEnd(eolCount);
			}

			// Process line length:
			DisplayLinePart lp = new DisplayLinePart(); // Default behavior regarding initial capacity is OK.
			if (TerminalSettings.Display.ShowLength)
			{
				DisplayLinePart info;
				PrepareLineEndInfo(line.DataCount, out info);
				lp.AddRange(info);
			}
			lp.Add(new DisplayElement.LineBreak()); // Direction may be both!

			// Reset line state, it is no longer needed:
			lineState.Reset();

			// Finalize elements and line:
			elements.AddRange(lp.Clone()); // Clone elements because they are needed again right below.
			line.AddRange(lp);
			lines.Add(line);
		}

		/// <summary></summary>
		protected override void ProcessRawChunk(RawChunk raw, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			LineState lineState;
			switch (raw.Direction)
			{
				case IODirection.Tx: lineState = this.txLineState; break;
				case IODirection.Rx: lineState = this.rxLineState; break;
				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + raw.Direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			foreach (byte b in raw.Data)
			{
				// Line begin and time stamp:
				if (lineState.LinePosition == LinePosition.Begin)
					ExecuteLineBegin(lineState, raw.TimeStamp, raw.PortStamp, raw.Direction, elements);

				// Data:
				ExecuteData(lineState, raw.Direction, b, elements);

				// Line end and length:
				if (lineState.LinePosition == LinePosition.End)
					ExecuteLineEnd(lineState, elements, lines);
			}
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		private void ProcessAndSignalPortOrDirectionLineBreak(string ps, IODirection d)
		{
			if (TerminalSettings.Display.PortLineBreakEnabled ||
				TerminalSettings.Display.DirectionLineBreakEnabled)
			{
				if (this.bidirLineState.IsFirstLine)
				{
					this.bidirLineState.IsFirstLine = false;
				}
				else // is subsequent line
				{
					if (!StringEx.EqualsOrdinalIgnoreCase(ps, this.bidirLineState.PortStamp) ||
						(d != this.bidirLineState.Direction))
					{
						LineState lineState;

						if (d == this.bidirLineState.Direction)
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.txLineState; break;
								case IODirection.Rx: lineState = this.rxLineState; break;
								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else // Attention: Direction changed => Use opposite state.
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.rxLineState; break; // Reversed!
								case IODirection.Rx: lineState = this.txLineState; break;
								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}

						if ((lineState.LineElements != null) && (lineState.LineElements.Count > 0))
						{
							DisplayElementCollection elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
							List<DisplayLine> lines = new List<DisplayLine>();

							ExecuteLineEnd(lineState, elements, lines);

							OnDisplayElementsProcessed(this.bidirLineState.Direction, elements);
							OnDisplayLinesProcessed   (this.bidirLineState.Direction, lines);
						}
					} // a line break has been detected
				} // is subsequent line
			} // a line break is active

			this.bidirLineState.PortStamp = ps;
			this.bidirLineState.Direction = d;
		}

		/// <summary></summary>
		protected override void ProcessAndSignalRawChunk(RawChunk raw)
		{
			// Check whether port or direction has changed:
			ProcessAndSignalPortOrDirectionLineBreak(raw.PortStamp, raw.Direction);

			// Process the raw chunk:
			base.ProcessAndSignalRawChunk(raw);
		}

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		/// <remarks>Ensure that states are completely reset.</remarks>
		public override bool RefreshRepositories()
		{
			AssertNotDisposed();

			InitializeStates();
			return (base.RefreshRepositories());
		}

		/// <remarks>Ensure that states are completely reset.</remarks>
		protected override void ClearMyRepository(RepositoryType repository)
		{
			AssertNotDisposed();

			InitializeStates();
			base.ClearMyRepository(repository);
		}

		#endregion

		#region Methods > ToString
		//------------------------------------------------------------------------------------------
		// Methods > ToString
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			AssertNotDisposed();
			
			return (ToString(""));
		}

		/// <summary></summary>
		public override string ToString(string indent)
		{
			AssertNotDisposed();
			
			return (indent + "> Type: TextTerminal" + Environment.NewLine + base.ToString(indent));
		}

		#endregion

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachTextTerminalSettings()
		{
			if (TextTerminalSettings != null)
				TextTerminalSettings.Changed += TextTerminalSettings_Changed;
		}

		private void DetachTextTerminalSettings()
		{
			if (TextTerminalSettings != null)
				TextTerminalSettings.Changed -= TextTerminalSettings_Changed;
		}

		private void ApplyTextTerminalSettings()
		{
			InitializeStates();
			RefreshRepositories();
		}

		#endregion

		#region Settings Events
		//==========================================================================================
		// Settings Events
		//==========================================================================================

		private void TextTerminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyTextTerminalSettings();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
