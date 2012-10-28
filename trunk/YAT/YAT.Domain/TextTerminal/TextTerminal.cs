//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;
using MKY.Text;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Text protocol terminal.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class TextTerminal : Terminal
	{
		#region Help
		//==========================================================================================
		// Help
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

		#region Element State
		//==========================================================================================
		// Element State
		//==========================================================================================

		private List<byte> rxDecodingStream;

		#endregion

		#region Line State
		//==========================================================================================
		// Line State
		//==========================================================================================

		private enum LinePosition
		{
			Begin,
			Data,
			End
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
		private class LineState
		{
			public LinePosition LinePosition;
			public DisplayLinePart LineElements;
			public DisplayLinePart EolElements;
			public EolQueue Eol;

			public LineState(EolQueue eol)
			{
				LinePosition = TextTerminal.LinePosition.Begin;
				LineElements = new DisplayLinePart();
				EolElements = new DisplayLinePart();
				Eol = eol;
			}

			public virtual void Reset()
			{
				LinePosition = TextTerminal.LinePosition.Begin;
				LineElements.Clear();
				EolElements.Clear();
				Eol.Reset();
			}
		}

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
		private class BidirLineState
		{
			public bool IsFirstLine;
			public SerialDirection Direction;

			public BidirLineState(bool isFirstLine, SerialDirection direction)
			{
				IsFirstLine = isFirstLine;
				Direction = direction;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstLine = rhs.IsFirstLine;
				Direction = rhs.Direction;
			}
		}

		#endregion

		#region Line Send Delay
		//==========================================================================================
		// Line Send Delay
		//==========================================================================================

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
		private class LineSendDelayState
		{
			public int LineCount;

			public LineSendDelayState()
			{
				Reset();
			}

			public virtual void Reset()
			{
				LineCount = 0;
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

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
			AttachTextTerminalSettings(settings.TextTerminal);
			Initialize();
		}

		/// <summary></summary>
		public TextTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachTextTerminalSettings(settings.TextTerminal);
			if (terminal is TextTerminal)
			{
				TextTerminal casted = (TextTerminal)terminal;
				this.rxDecodingStream = casted.rxDecodingStream;
				this.txLineState = casted.txLineState;
				this.rxLineState = casted.rxLineState;

				this.bidirLineState = new BidirLineState(casted.bidirLineState);
			}
			else
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			InitializeStates();
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			// Finalize managed resources.

			if (disposing)
			{
				// Dispose of unmanaged resources.
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Settings.TextTerminalSettings TextTerminalSettings
		{
			get
			{
				AssertNotDisposed();
				return (TerminalSettings.TextTerminal);
			}
			set
			{
				AssertNotDisposed();
				AttachTextTerminalSettings(value);
				ApplyTextTerminalSettings();
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

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void ProcessParsableSendItem(ParsableSendItem item)
		{
			string data = item.Data;
			bool sendEol = item.IsLine;
			bool performLineDelay = false;

			Parser.SubstitutionParser p = new Parser.SubstitutionParser(TerminalSettings.IO.Endianess, (EncodingEx)TextTerminalSettings.Encoding);

			// Prepare EOL:
			byte[] eolByteArray = new byte[] { };
			if (item.IsLine)
			{
				MemoryStream eolWriter = new MemoryStream();
				foreach (Parser.Result result in p.Parse(TextTerminalSettings.TxEol, TextTerminalSettings.CharSubstitution, Parser.ParseMode.AllByteArrayResults))
				{
					if (result is Parser.ByteArrayResult)
					{
						byte[] a = ((Parser.ByteArrayResult)result).ByteArray;
						eolWriter.Write(a, 0, a.Length);
					}
				}
				eolByteArray = eolWriter.ToArray();
			}

			// Check for EOL comment indicators:
			if (TextTerminalSettings.EolComment.SkipComment)
			{
				foreach (string marker in TextTerminalSettings.EolComment.Indicators)
				{
					int index = StringEx.IndexOfOutsideDoubleQuotes(data, marker, StringComparison.Ordinal);
					if (index >= 0)
					{
						data = StringEx.Left(data, index);

						if (TextTerminalSettings.EolComment.SkipWhiteSpace)
							data = data.TrimEnd(null); // <c>null</c> means white-spaces.
					}
				}
			}

			// Parse string and execute keywords:
			foreach (Parser.Result result in p.Parse(data, TextTerminalSettings.CharSubstitution, Parser.ParseMode.All))
			{
				if      (result is Parser.ByteArrayResult)
				{
					ForwardDataToRawTerminal(((Parser.ByteArrayResult)result).ByteArray);
				}
				else if (result is Parser.KeywordResult)
				{
					Parser.KeywordResult keywordResult = (Parser.KeywordResult)result;
					switch (keywordResult.Keyword)
					{
						// Process end-of-line keywords:
						case Parser.Keyword.LineDelay:
						{
							performLineDelay = true;
							break;
						}

						case Parser.Keyword.NoEol:
						{
							sendEol = false;
							break;
						}

						// Process text terminal specific in-line keywords:
						case Parser.Keyword.Eol:
						{
							ForwardDataToRawTerminal(eolByteArray);
							break;
						}

						// Process other in-line keywords:
						default:
						{
							ProcessInLineKeywords(keywordResult);
							break;
						}
					}
				}
			}

			// Finalize the line:
			if (sendEol)
				ForwardDataToRawTerminal(eolByteArray);

			int accumulatedLineDelay = 0;
			if (performLineDelay)
			{
				int delay = TerminalSettings.Send.DefaultLineDelay;
				Thread.Sleep(delay);
				accumulatedLineDelay += delay;
			}

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
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			this.rxDecodingStream = new List<byte>();

			Parser.SubstitutionParser p = new Parser.SubstitutionParser(TerminalSettings.IO.Endianess, (EncodingEx)TextTerminalSettings.Encoding);
			byte[] txEol;
			if (!p.TryParse(TextTerminalSettings.TxEol, TextTerminalSettings.CharSubstitution, out txEol))
			{
				TextTerminalSettings.TxEol = Settings.TextTerminalSettings.DefaultEol;
				txEol = p.Parse(TextTerminalSettings.TxEol, TextTerminalSettings.CharSubstitution);
			}
			byte[] rxEol;
			if (!p.TryParse(TextTerminalSettings.RxEol, TextTerminalSettings.CharSubstitution, out rxEol))
			{
				TextTerminalSettings.RxEol = Settings.TextTerminalSettings.DefaultEol;
				rxEol = p.Parse(TextTerminalSettings.RxEol, TextTerminalSettings.CharSubstitution);
			}
			this.txLineState = new LineState(new EolQueue(txEol));
			this.rxLineState = new LineState(new EolQueue(rxEol));

			this.bidirLineState = new BidirLineState(true, SerialDirection.Tx);

			this.lineSendDelayState = new LineSendDelayState();
		}

		/// <summary></summary>
		protected override DisplayElement ByteToElement(byte b, SerialDirection d, Radix r)
		{
			switch (r)
			{
				// Bin/Oct/Dec/Hex.
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					return (base.ByteToElement(b, d, r));
				}

				// Char/String.
				case Radix.Char:
				case Radix.String:
				{
					// Add byte to ElementState.
					this.rxDecodingStream.Add(b);
					byte[] decodingArray = this.rxDecodingStream.ToArray();

					// Get encoding and retrieve char count.
					Encoding e = (EncodingEx)TextTerminalSettings.Encoding;
					int charCount = e.GetCharCount(decodingArray, 0, decodingArray.Length);

					// If decoding array can be decoded into something useful, decode it.
					if (charCount == 1)
					{
						// Char count must be 1, otherwise something went wrong.
						char[] chars = new char[charCount];
						if (e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true) == 1)
						{
							// Ensure that 'unknown' character 0xFFFD is not decoded yet.
							int code = (int)chars[0];
							if (code != 0xFFFD)
							{
								this.rxDecodingStream.Clear();

								if ((code < 0x20) || (code == 0x7F)) // Control chars.
								{
									return (base.ByteToElement(b, d, r));
								}
								else if (b == 0x20) // Space.
								{
									return (base.ByteToElement(b, d, r));
								}
								else
								{
									StringBuilder sb = new StringBuilder();
									sb.Append(chars, 0, charCount);
									if (d == SerialDirection.Tx)
										return (new DisplayElement.TxData(decodingArray, sb.ToString(), charCount));
									else
										return (new DisplayElement.RxData(decodingArray, sb.ToString(), charCount));
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
						this.rxDecodingStream.Clear();
					}

					// Nothing to decode (yet).
					return (new DisplayElement.NoData());
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, "Invalid radix"));
				}
			}
		}

		private void ExecuteLineBegin(LineState lineState, DateTime ts, DisplayElementCollection elements)
		{
			if (TerminalSettings.Display.ShowTimeStamp)
			{
				DisplayLinePart lp = new DisplayLinePart();

				lp.Add(new DisplayElement.TimeStamp(ts));
				lp.Add(new DisplayElement.LeftMargin());

				// Attention: Clone elements because they are needed again below.
				lineState.LineElements.AddRange(lp.Clone());
				elements.AddRange(lp);
			}
			lineState.LinePosition = LinePosition.Data;
		}

		private void ExecuteData(LineState lineState, SerialDirection d, byte b, DisplayElementCollection elements)
		{
			DisplayLinePart lp = new DisplayLinePart();

			// Add space if necessary.
			if (ElementsAreSeparate(d))
			{
				int lineLength = 0;
				foreach (DisplayElement e in lineState.LineElements)
				{
					if (e.IsData)
						lineLength += e.DataCount;
				}
				if (lineLength > 0)
				{
					lp.Add(new DisplayElement.Space());
				}
			}

			// Process data.
			DisplayElement de = ByteToElement(b, d);

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started.
			lineState.Eol.Enqueue(b);
			if (lineState.Eol.IsCompleteMatch)
			{
				if (de.IsData)
					lineState.EolElements.Add(de);

				// Normal case, EOL consists of a single sequence of control characters.
				if ((lineState.EolElements.Count == 1) && (lineState.EolElements[0].OriginCount == lineState.Eol.Eol.Length))
				{
					// Mark element as EOL.
					DisplayElement item = lineState.EolElements[0];
					item.IsEol = true;
					lp.Add(item);
				}
				else
				{
					// Ensure that only as many elements as EOL contains are marked as EOL.
					// Note that sequence mighty look like <CR><CR><LF>, only the last two are EOL!
					
					// Unfold the elements into single elements for easier processing.
					List<DisplayElement> l = new List<DisplayElement>();
					foreach (DisplayElement item in lineState.EolElements)
					{
						foreach (Pair<byte[], string> origin in item.Origin)
							l.Add(DisplayElement.Recreate(item, origin));
					}

					// Count data.
					int dataCount = 0;
					foreach (DisplayElement item in l)
						dataCount += item.DataCount;

					// Mark only true EOL element as EOL.
					int firstEolIndex = dataCount - lineState.Eol.Eol.Length;
					int currentIndex = 0;
					foreach (DisplayElement item in l)
					{
						currentIndex += item.DataCount;

						if (currentIndex > firstEolIndex)
							item.IsEol = true;

						lp.Add(item);
					}
				}

				lineState.EolElements.Clear();
				lineState.LinePosition = LinePosition.End;
			}
			else if (lineState.Eol.IsPartlyMatch)
			{
				// Keep EOL elements but delay them until EOL is complete.
				if (de.IsData)
					lineState.EolElements.Add(de);
			}
			else
			{
				// Retrieve potential EOL elements on incomplete EOL.
				if (lineState.EolElements.Count > 0)
				{
					lp.AddRange(lineState.EolElements);
					lineState.EolElements.Clear();
				}

				// Add non-EOL data.
				if (de.IsData)
					lp.Add(de);
			}

			// Return elements.
			//
			// \attention:
			// Clone elements because they are needed again a line below.
			lineState.LineElements.AddRange(lp.Clone());
			elements.AddRange(lp);
		}

		private void ExecuteLineEnd(LineState lineState, SerialDirection d, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			// Process EOL.
			int eolLength = lineState.Eol.Eol.Length;
			DisplayLine line = new DisplayLine();

			if (TextTerminalSettings.ShowEol || (eolLength <= 0) || (!lineState.Eol.IsCompleteMatch))
			{
				line.AddRange(lineState.LineElements);
			}
			else // Remove EOL.
			{
				int eolAndWhiteCount = 0;
				DisplayElement[] des = lineState.LineElements.ToArray();

				// Traverse elements reverse and count EOL and white spaces to be removed.
				for (int i = (des.Length - 1); i >= 0; i--)
				{
					// Detect last non-EOL data element.
					if (des[i].IsData && !des[i].IsEol)
						break;

					eolAndWhiteCount++;
				}

				// Now traverse elements forward and add elements to line.
				for (int i = 0; i < (des.Length - eolAndWhiteCount); i++)
					line.Add(des[i]);

				// Finally, remove EOL and white spaces from elements.
				if (elements.Count >= eolAndWhiteCount)
					elements.RemoveAtEnd(eolAndWhiteCount);
			}

#if (FALSE)
			// \remind:
			// Break debugger on SIR responses that are mixed up (more than 16 chars without EOL)
			// Used to debug an issue that is possibly related to #2455804
			//
			// 2009-08-16 / mky
			// Ran more than 50'000 SIR responses without getting a break here.
			//
			if ((d == SerialDirection.Rx) && (line.DataCount > 16))
				System.Diagnostics.Debugger.Break();
#endif

			// Process line length.
			DisplayLinePart lp = new DisplayLinePart();
			if (TerminalSettings.Display.ShowLength)
			{
				lp.Add(new DisplayElement.RightMargin());
				lp.Add(new DisplayElement.LineLength(line.DataCount));
			}
			lp.Add(new DisplayElement.LineBreak(d));

			// Add line end to elements and return them.
			elements.AddRange(lp);

			// Also add line end to line and return it.
			// Attention: Clone elements because they've also needed above.
			line.AddRange(lp.Clone());
			lines.Add(line);

			// Reset line state.
			lineState.Reset();
		}

		/// <summary></summary>
		protected override void ProcessRawElement(RawElement re, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			LineState lineState;
			if (re.Direction == SerialDirection.Tx)
				lineState = this.txLineState;
			else
				lineState = this.rxLineState;

			foreach (byte b in re.Data)
			{
				// Line begin and time stamp.
				if (lineState.LinePosition == LinePosition.Begin)
					ExecuteLineBegin(lineState, re.TimeStamp, elements);

				// Data.
				ExecuteData(lineState, re.Direction, b, elements);

				// Line end and length.
				if (lineState.LinePosition == LinePosition.End)
					ExecuteLineEnd(lineState, re.Direction, elements, lines);
			}
		}

		private void ProcessAndSignalDirectionLineBreak(SerialDirection d)
		{
			if (TerminalSettings.Display.DirectionLineBreakEnabled)
			{
				if (this.bidirLineState.IsFirstLine)
				{
					this.bidirLineState.IsFirstLine = false;
				}
				else
				{
					LineState lineState; // Attention: Direction changed => Use opposite state.
					if (d == SerialDirection.Tx)
						lineState = this.rxLineState;
					else
						lineState = this.txLineState;

					if ((lineState.LineElements.Count > 0) &&
						(d != this.bidirLineState.Direction))
					{
						DisplayElementCollection elements = new DisplayElementCollection();
						List<DisplayLine> lines = new List<DisplayLine>();

						ExecuteLineEnd(lineState, d, elements, lines);

						OnDisplayElementsProcessed(this.bidirLineState.Direction, elements);
						OnDisplayLinesProcessed(this.bidirLineState.Direction, lines);
					}
				}
			}
			this.bidirLineState.Direction = d;
		}

		/// <summary></summary>
		protected override void ProcessAndSignalRawElement(RawElement re)
		{
			// Check whether direction has changed.
			ProcessAndSignalDirectionLineBreak(re.Direction);

			// Process the raw element.
			base.ProcessAndSignalRawElement(re);
		}

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		/// <remarks>Ensure that line states are completely reset.</remarks>
		public override void ReloadRepositories()
		{
			AssertNotDisposed();
			
			Initialize();
			base.ReloadRepositories();
		}

		/// <summary></summary>
		/// <remarks>Ensure that line states are completely reset.</remarks>
		protected override void ClearMyRepository(RepositoryType repository)
		{
			Initialize();
			base.ClearMyRepository(repository);
		}

		#endregion

		#region Methods > ToString
		//------------------------------------------------------------------------------------------
		// Methods > ToString
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public override string ToString()
		{
			AssertNotDisposed();
			
			return (ToString(""));
		}

		/// <summary></summary>
		public override string ToString(string indent)
		{
			AssertNotDisposed();
			
			return (indent + "- Type: TextTerminal" + Environment.NewLine + base.ToString(indent));
		}

		#endregion

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachTextTerminalSettings(Settings.TextTerminalSettings textTerminalSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(TerminalSettings.TextTerminal, textTerminalSettings))
				return;

			if (TerminalSettings.TextTerminal != null)
				DetachTextTerminalSettings();

			TerminalSettings.TextTerminal = textTerminalSettings;
			TerminalSettings.TextTerminal.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(TextTerminalSettings_Changed);
		}

		private void DetachTextTerminalSettings()
		{
			TerminalSettings.TextTerminal.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(TextTerminalSettings_Changed);
		}

		private void ApplyTextTerminalSettings()
		{
			InitializeStates();
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
