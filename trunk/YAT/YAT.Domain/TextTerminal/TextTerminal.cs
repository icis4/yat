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
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using MKY;
using MKY.Text;

using YAT.Application.Utilities;
using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\TextTerminal for better separation of the implementation files.
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
			@"""\!(" + (Parser.KeywordEx)Parser.Keyword.NoEol + @")"" is useful if your text protocol does have an EOL sequence except for a few special commands (e.g. synchronization commands).";

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
			Content,
			ContentExceeded,
			End
		}

		private class LineState
		{
			public LinePosition    Position         { get; set; }
			public DisplayLinePart Elements         { get; set; }
			public DisplayLinePart EolElements      { get; set; }
			public SequenceQueue   Eol              { get; set; }

			public Dictionary<string, bool> EolOfLastLineOfGivenPortWasCompleteMatch { get; set; }

			public DateTime        TimeStamp        { get; set; }
			public bool            Highlight        { get; set; }

			public LineState(SequenceQueue eol)
			{
				Position         = LinePosition.Begin; // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
				Elements         = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				EolElements      = new DisplayLinePart(); // Default initial capacity is OK.
				Eol              = eol;

				EolOfLastLineOfGivenPortWasCompleteMatch = new Dictionary<string, bool>(); // Default initial capacity is OK.

				TimeStamp = DateTime.Now;
				Highlight = false;
			}

			public virtual void Reset(string portStamp, bool eolOfLastLineWasCompleteMatch)
			{
				Position         = LinePosition.Begin; // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
				Elements         = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
				EolElements      = new DisplayLinePart(); // Default initial capacity is OK.
				Eol.Reset();

				if (EolOfLastLineOfGivenPortWasCompleteMatch.ContainsKey(portStamp))
					EolOfLastLineOfGivenPortWasCompleteMatch[portStamp] = eolOfLastLineWasCompleteMatch;
				else
					EolOfLastLineOfGivenPortWasCompleteMatch.Add(portStamp, eolOfLastLineWasCompleteMatch);

				TimeStamp = DateTime.Now;
				Highlight = false;
			}
		}

		private class BidirLineState
		{
			public bool IsFirstChunk          { get; set; }
			public bool IsFirstLine           { get; set; }
			public string PortStamp           { get; set; }
			public IODirection Direction      { get; set; }
			public DateTime LastLineTimeStamp { get; set; }

			public BidirLineState()
			{
				IsFirstChunk      = true;
				IsFirstLine       = true;
				PortStamp         = null;
				Direction         = IODirection.None;
				LastLineTimeStamp = DateTime.Now;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstChunk      = rhs.IsFirstChunk;
				IsFirstLine       = rhs.IsFirstLine;
				PortStamp         = rhs.PortStamp;
				Direction         = rhs.Direction;
				LastLineTimeStamp = rhs.LastLineTimeStamp;
			}
		}

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

		/// <summary>
		/// Gets the Tx EOL sequence.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TxEolSequence
		{
			get
			{
				AssertNotDisposed();

				if (this.txLineState != null)
					return (this.txLineState.Eol.Sequence);
				else
					return (null);
			}
		}

		/// <summary>
		/// Gets the Rx EOL sequence.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] RxEolSequence
		{
			get
			{
				AssertNotDisposed();

				if (this.rxLineState != null)
					return (this.rxLineState.Eol.Sequence);
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override bool TryParseText(string s, out byte[] result, Radix defaultRadix = Radix.String)
		{
			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, TerminalSettings.Send.ToParseMode()))
				return (p.TryParse(s, out result, defaultRadix));
		}

		#endregion

		#region Methods > Send Data
		//------------------------------------------------------------------------------------------
		// Methods > Send Data
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override void SendFileLine(string dataLine, Radix defaultRadix = Parser.Parser.DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TextTerminalSettings.SendFile.ToParseMode();

			DoSendData(new TextDataSendItem(dataLine, defaultRadix, parseMode, true));
		}

		/// <remarks>
		/// \remind (2017-09-02 / MKY) there is a limitation in this implementation:
		/// This method will be called per item, not per complete line. But, regexes below are
		/// likely using beginning or end of line anchors ("^" and "$"). Well, a limitation.
		/// </remarks>
		protected override void ProcessTextDataSendItem(TextDataSendItem item)
		{
			string textToParse = item.Data;

			// Check for text exclusion patterns:
			if (TextTerminalSettings.TextExclusion.Enabled)
			{
				foreach (Regex r in TextTerminalSettings.TextExclusion.Regexes)
				{
					var m = r.Match(textToParse);
					if (m.Success)
						textToParse = textToParse.Remove(m.Index, m.Length);

					if (TextTerminalSettings.SendFile.SkipEmptyLines && string.IsNullOrEmpty(textToParse))
						return;
				}
			}

			// Parse the item string:
			bool hasSucceeded;
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;

			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, item.ParseMode))
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
					ForwardDataToRawTerminal(this.txLineState.Eol.Sequence); // In-line.
					break;
				}

				default:
				{
					base.ProcessInLineKeywords(result);
					break;
				}
			}
		}

		/// <remarks>
		/// \remind (2018-01-17 / MKY / FR#333) there is a limitation in this implementation:
		/// This method will be called sligtly after the line content has been forwarded to the raw
		/// terminal, thus may result in a short delay between content and EOL. With 9600 baud, no
		/// delay is noticable. With 115200 baud, delay is still not noticable most of the times,
		/// but sometimes it's 30 ms! For most use cases this doesn't matter. But still, it is not
		/// ideal, as behavior doesn't reproduce.
		/// On the other hand, there are also drawbacks in refactoring the current implementation.
		/// Already tried once in 2015 (while working between TextTerminal.cs SVN revisions 680 and
		/// 695), but reverted again.
		/// </remarks>
		protected override void ProcessLineEnd(bool sendEol)
		{
			if (sendEol)
				ForwardDataToRawTerminal(this.txLineState.Eol.Sequence); // End-of-line.
		}

		/// <summary></summary>
		protected override int ProcessLineDelayOrInterval(bool performLineDelay, int lineDelay, bool performLineInterval, int lineInterval, DateTime lineBeginTimeStamp, DateTime lineEndTimeStamp)
		{
			int accumulatedLineDelay = base.ProcessLineDelayOrInterval(performLineDelay, lineDelay, performLineInterval, lineInterval, lineBeginTimeStamp, lineEndTimeStamp);

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

		#region Methods > Send File
		//------------------------------------------------------------------------------------------
		// Methods > Send File
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected override void ProcessSendFileItem(FileSendItem item)
		{
			try
			{
				if (ExtensionHelper.IsXmlFile(item.FilePath))
				{
					string[] lines;
					XmlReaderHelper.LinesFromFile(item.FilePath, out lines); // Read all at once for simplicity.
					foreach (string line in lines)
					{
						if (string.IsNullOrEmpty(line) && TextTerminalSettings.SendFile.SkipEmptyLines)
							continue;

						SendFileLine(line);

						if (BreakSendFile)
						{
							OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
							break;
						}

						Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
					}
				}
				else if (ExtensionHelper.IsRtfFile(item.FilePath))
				{
					string[] lines;
					RtfReaderHelper.LinesFromRtfFile(item.FilePath, out lines); // Read all at once for simplicity.
					foreach (string line in lines)
					{
						if (string.IsNullOrEmpty(line) && TextTerminalSettings.SendFile.SkipEmptyLines)
							continue;

						SendFileLine(line);

						if (BreakSendFile)
						{
							OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
							break;
						}

						Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
					}
				}
				else
				{
					using (var sr = new StreamReader(item.FilePath, (EncodingEx)TextTerminalSettings.Encoding, true))
					{                             // Automatically detect encoding from BOM, otherwise use given setting.
						string line;
						while ((line = sr.ReadLine()) != null)
						{
							if (string.IsNullOrEmpty(line) && TextTerminalSettings.SendFile.SkipEmptyLines)
								continue;

							SendFileLine(line, item.DefaultRadix);

							if (BreakSendFile)
							{
								OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
								break;
							}

							Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
						}
					}
				}
			}
			catch (Exception ex)
			{
				OnDisplayElementProcessed(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, @"Error reading file """ + item.FilePath + @""": " + ex.Message));
			}
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
							char[] c = new char[1]; // 'IsSingleByte'!
							if (e.GetDecoder().GetChars(new byte[] { b }, 0, 1, c, 0, true) == 1)
							{
								if (r != Radix.Unicode)
								{
									switch (d)
									{                                                               // 'IsSingleByte'!
										case IODirection.Tx: return (new DisplayElement.TxData(b, c[0].ToString(CultureInfo.InvariantCulture)));
										case IODirection.Rx: return (new DisplayElement.RxData(b, c[0].ToString(CultureInfo.InvariantCulture)));

										default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
									}
								}
								else // Unicode:
								{
									switch (d)
									{                                                                                           // 'IsSingleByte'!
										case IODirection.Tx: return (new DisplayElement.TxData(b, UnicodeValueToNumericString(c[0])));
										case IODirection.Rx: return (new DisplayElement.RxData(b, UnicodeValueToNumericString(c[0])));

										default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
									}
								}
							}
							else // Something went seriously wrong!
							{
								throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "byte " + b.ToString("X2", CultureInfo.InvariantCulture) + " is invalid for encoding " + ((EncodingEx)e).ToString() + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
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

						this.rxMultiByteDecodingStream.Add(b);
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
									return (new DisplayElement.Nonentity()); // Ignore the character, this results in hiding.
								}
								else
								{
									if (r != Radix.Unicode)
									{
										switch (d)
										{                                                                               // 'effectiveCharCount' is 1 for sure!
											case IODirection.Tx: return (new DisplayElement.TxData(decodingArray, chars[0].ToString(CultureInfo.InvariantCulture), decodingArray.Length));
											case IODirection.Rx: return (new DisplayElement.RxData(decodingArray, chars[0].ToString(CultureInfo.InvariantCulture), decodingArray.Length));

											default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
										}
									}
									else // Unicode:
									{
										switch (d)
										{                                                                                                           // 'effectiveCharCount' is 1 for sure!
											case IODirection.Tx: return (new DisplayElement.TxData(decodingArray, UnicodeValueToNumericString(chars[0]), decodingArray.Length));
											case IODirection.Rx: return (new DisplayElement.RxData(decodingArray, UnicodeValueToNumericString(chars[0]), decodingArray.Length));

											default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
										}
									}
								}
							}
							else // Single 'unknown' character 0xFFFD:
							{
								return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
							}
						}
						else // (effectiveCharCount == 0) || (effectiveCharCount > 1)
						{
							bool isInvalid = false;

							// If one of the following conditions is given, the byte sequence is invalid:

							if (decodingArray.Length >= e.GetMaxByteCount(1))
							{
								isInvalid = true; // Nothing useful to decode even though maximum length reached.
							}
							else if (effectiveCharCount > 0)
							{
								foreach (char c in chars)
								{
									int code = c;
									if (code != 0xFFFD)
									{
										isInvalid = true; // Everything else than 'unknown' character 0xFFFD is invalid.
										break;
									}
								}
							}

							// If neither of the conditions is given, the byte sequence may not be complete yet:

							if (!isInvalid)
							{
								return (new DisplayElement.Nonentity()); // Nothing to decode (yet).
							}
							else
							{
								var sb = new StringBuilder(@"""");

								bool firstElement = true;
								foreach (byte invalid in this.rxMultiByteDecodingStream)
								{
									if (firstElement)
										firstElement = false;
									else
										sb.Append(" ");

									sb.Append(base.ByteToElement(invalid, d, Radix.Hex));
								}

								sb.Append(@""" is an invalid '");
								sb.Append(((EncodingEx)e).DisplayName);
								sb.Append("' byte sequence!");

								this.rxMultiByteDecodingStream.Clear(); // Reset decoding stream.

								return (new DisplayElement.ErrorInfo((Direction)d, sb.ToString(), true));
							}
						} // (effectiveCharCount == 0) || (effectiveCharCount > 1)
					} // MultiByte
				} // String/Char/Unicode

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' radix is missing here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		private void ExecuteLineBegin(LineState lineState, DateTime ts, string ps, IODirection d, DisplayElementCollection elements)
		{
			if (this.bidirLineState.IsFirstLine) // Properly initialize the time delta:
				this.bidirLineState.LastLineTimeStamp = ts;
			                                        //// Using the exact type to prevent potential mismatch in case the type one day defines its own value!
			var lp = new DisplayLinePart(DisplayLinePart.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			lp.Add(new DisplayElement.LineStart()); // Direction may be both!

			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowPort || TerminalSettings.Display.ShowDirection)
			{
				DisplayLinePart info;
				PrepareLineBeginInfo(ts, (ts - InitialTimeStamp), (ts - this.bidirLineState.LastLineTimeStamp), ps, d, out info);
				lp.AddRange(info);
			}

			lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
			elements.AddRange(lp);

			lineState.Position = LinePosition.Content;
			lineState.TimeStamp = ts;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void ExecuteContent(LineState lineState, IODirection d, byte b, DisplayElementCollection elements)
		{
			// Convert content:
			var de = ByteToElement(b, d);
			de.Highlight = lineState.Highlight;

			var lp = new DisplayLinePart(); // Default initial capacity is OK.

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started:
			lineState.Eol.Enqueue(b);
			if (lineState.Eol.IsCompleteMatch)
			{
				if (TextTerminalSettings.ShowEol)
				{
					if (de.IsContent)
						lineState.EolElements.Add(de); // No clone needed as element has just been created.
					else
						lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.

					// Normal case, EOL consists of a single sequence of control characters:
					if ((lineState.EolElements.Count == 1) && (lineState.EolElements[0].ByteCount == lineState.Eol.Sequence.Length))
					{
						// Unfold the elements into single elements for correct processing:
						var l = new List<DisplayElement>(lineState.EolElements.ByteCount); // Preset the required capacity to improve memory management.
						foreach (var item in lineState.EolElements)
						{
							foreach (var originItem in item.Origin)
								l.Add(item.RecreateFromOriginItem(originItem));
						}

						// Add them as separate items:
						foreach (var item in l)
						{
							AddSpaceIfNecessary(lineState, d, lp, item);
							lp.Add(item); // No clone needed as all items have just been recreated futher above.
						}
					}
					else
					{
						// Ensure that only as many elements as EOL contains are marked as EOL.
						// Note that sequence might look like <CR><CR><LF>, only the last two are EOL!
					
						// Unfold the elements into single elements for correct processing:
						var l = new List<DisplayElement>(lineState.EolElements.ByteCount); // Preset the required capacity to improve memory management.
						foreach (var item in lineState.EolElements)
						{
							foreach (var originItem in item.Origin)
								l.Add(item.RecreateFromOriginItem(originItem));
						}

						// Count content:
						int byteCount = 0;
						foreach (var item in l)
							byteCount += item.ByteCount;

						// Mark only true EOL elements as EOL:
						int firstEolIndex = (byteCount - lineState.Eol.Sequence.Length);
						int currentIndex = 0;
						foreach (var item in l)
						{
							currentIndex += item.ByteCount;

							if (currentIndex > firstEolIndex)
								item.IsEol = true;

							AddSpaceIfNecessary(lineState, d, lp, item);
							lp.Add(item); // No clone needed as all items have just been recreated futher above.
						}
					}
				}

				lineState.EolElements.Clear();
				lineState.Position = LinePosition.End;
			}
			else if (lineState.Eol.IsPartlyMatchContinued)
			{
				// Keep EOL elements and delay them until EOL is complete:
				if (de.IsContent)
					lineState.EolElements.Add(de); // No clone needed as element has just been created further above.
				else
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
			}
			else if (lineState.Eol.IsPartlyMatchBeginning)
			{
				// Previous was no match, previous EOL elements can be treated as normal:
				TreatEolAsNormal(lineState, lp);

				// Keep EOL elements and delay them until EOL is complete:
				if (de.IsContent)
					lineState.EolElements.Add(de); // No clone needed as element has just been created further above.
				else
					lp.Add(de); // Still add non-content element, could e.g. be a multi-byte error message.
			}
			else
			{
				// No match at all, previous EOL elements can be treated as normal:
				TreatEolAsNormal(lineState, lp);

				// Add non-EOL element:
				AddSpaceIfNecessary(lineState, d, lp, de);
				lp.Add(de); // No clone needed as element has just been created further above.
			}

			if (lineState.Position != LinePosition.ContentExceeded)
			{
				lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elements.AddRange(lp);
			}

			// Only continue evaluation if no line break detected yet (cannot have more than one line break).
			if (lineState.Position != LinePosition.End)
			{
				if ((lineState.Elements.ByteCount >= TerminalSettings.Display.MaxBytePerLineCount) &&
					(lineState.Position != LinePosition.ContentExceeded))
				{
					lineState.Position = LinePosition.ContentExceeded;
					                                     //// Using term "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.
					string message = "Maximal number of bytes per line exceeded! Check the end-of-line settings or increase the limit in the advanced terminal settings.";
					lineState.Elements.Add(new DisplayElement.ErrorInfo((Direction)d, message, true));
					elements.Add          (new DisplayElement.ErrorInfo((Direction)d, message, true));
				}
			}
		}

		private void AddSpaceIfNecessary(LineState lineState, IODirection d, DisplayLinePart lp, DisplayElement de)
		{
			if (ElementsAreSeparate(d) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.ByteCount > 0) || (lp.ByteCount > 0))
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

		private void ExecuteLineEnd(LineState lineState, string ps, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			// Note: Code sequence the same as ExecuteLineEnd() of BinaryTerminal for better comparability.

			                                    // Using the exact type to prevent potential mismatch in case the type one day defines its own value!
			var line = new DisplayLine(DisplayLine.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.

			// Process line content:
			int eolLength = lineState.Eol.Sequence.Length;
			if (TextTerminalSettings.ShowEol || (eolLength <= 0) || (!lineState.Eol.IsCompleteMatch))
			{
				line.AddRange(lineState.Elements.Clone()); // Clone elements to ensure decoupling.
			}
			else // Remove EOL:
			{
				// Traverse elements reverse and count EOL elements to be removed:
				int eolCount = 0;
				DisplayElement[] des = lineState.Elements.Clone().ToArray(); // Clone elements to ensure decoupling.
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
					elements.RemoveRangeAtEnd(eolCount);
			}

			// Process line length:
			var lp = new DisplayLinePart(); // Default initial capacity is OK.
			if (TerminalSettings.Display.ShowLength) // = byte count.
			{
				DisplayLinePart info;
				PrepareLineEndInfo(line.ByteCount, out info);
				lp.AddRange(info);
			}
			lp.Add(new DisplayElement.LineBreak()); // Direction may be both!

			// Potentially suppress empty lines that only contain hidden <CR><LF>:
			bool suppressEmptyLine = ((lineState.Elements.ByteCount == 0) &&                    // Empty line.
			                          (lineState.EolElements.ByteCount == 1) &&                 // EOL contained though, as a single content element.
			                           lineState.EolOfLastLineOfGivenPortWasCompleteMatch.ContainsKey(ps) &&
			                          !lineState.EolOfLastLineOfGivenPortWasCompleteMatch[ps]); // EOL of last line of the current port is still pending.
			if (suppressEmptyLine)
			{
				elements.RemoveAtEndUntilIncluding(typeof(DisplayElement.LineStart));
			}
			else
			{
				// Finalize elements and line:
				elements.AddRange(lp.Clone()); // Clone elements because they are needed again right below.
				line.AddRange(lp);
				lines.Add(line);
			}

			this.bidirLineState.IsFirstLine = false;
			this.bidirLineState.LastLineTimeStamp = lineState.TimeStamp;

			// Reset line state:
			lineState.Reset(ps, lineState.Eol.IsCompleteMatch);
		}

		/// <summary></summary>
		protected override void ProcessRawChunk(RawChunk raw, bool highlight, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			LineState lineState;
			switch (raw.Direction)
			{
				case IODirection.Tx: lineState = this.txLineState; break;
				case IODirection.Rx: lineState = this.rxLineState; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + raw.Direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (highlight) // Activate if needed, leave unchanged otherwise as it could have become highlighted by a previous raw chunk.
			{
				lineState.Highlight = true;
			}

			foreach (byte b in raw.Content)
			{
				// Line begin and time stamp:
				if (lineState.Position == LinePosition.Begin)
					ExecuteLineBegin(lineState, raw.TimeStamp, raw.PortStamp, raw.Direction, elements);

				// Content:
				if (lineState.Position == LinePosition.Content)
					ExecuteContent(lineState, raw.Direction, b, elements);

				// Line end and length:
				if (lineState.Position == LinePosition.End)
					ExecuteLineEnd(lineState, raw.PortStamp, elements, lines);
			}
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		private void ProcessAndSignalPortOrDirectionLineBreak(string ps, IODirection d)
		{
			if (this.bidirLineState.IsFirstChunk)
			{
				this.bidirLineState.IsFirstChunk = false;
			}
			else // = 'IsSubsequentChunk'.
			{
				if (TerminalSettings.Display.PortLineBreakEnabled ||
					TerminalSettings.Display.DirectionLineBreakEnabled)
				{
					if (!StringEx.EqualsOrdinalIgnoreCase(ps, this.bidirLineState.PortStamp) || (d != this.bidirLineState.Direction))
					{
						LineState lineState;

						if (d == this.bidirLineState.Direction)
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.txLineState; break;
								case IODirection.Rx: lineState = this.rxLineState; break;

								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}
						else // Attention: Direction changed => Use other state.
						{
							switch (d)
							{
								case IODirection.Tx: lineState = this.rxLineState; break; // Reversed!
								case IODirection.Rx: lineState = this.txLineState; break; // Reversed!

								default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}

						if ((lineState.Elements != null) && (lineState.Elements.Count > 0))
						{
							var elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
							var lines = new List<DisplayLine>();

							ExecuteLineEnd(lineState, ps, elements, lines);

							OnDisplayElementsProcessed(this.bidirLineState.Direction, elements);
							OnDisplayLinesProcessed   (this.bidirLineState.Direction, lines);
						}
					} // a line break has been detected
				} // a line break is active
			} // is subsequent chunk

			this.bidirLineState.PortStamp = ps;
			this.bidirLineState.Direction = d;
		}

		/// <summary></summary>
		protected override void ProcessAndSignalRawChunk(RawChunk raw, bool highlight)
		{
			// Check whether port or direction has changed:
			ProcessAndSignalPortOrDirectionLineBreak(raw.PortStamp, raw.Direction);

			// Process the raw chunk:
			base.ProcessAndSignalRawChunk(raw, highlight);
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
			// See below why AssertNotDisposed() is not called on such basic method!

			return (ToDiagnosticsString("")); // No 'real' ToString() method required yet.
		}

		/// <summary></summary>
		public override string ToDiagnosticsString(string indent)
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method!

			return (indent + "> Type: TextTerminal" + Environment.NewLine + base.ToDiagnosticsString(indent));
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
