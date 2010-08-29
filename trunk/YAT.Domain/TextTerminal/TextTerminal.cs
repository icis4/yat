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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MKY.Utilities.Text;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Text protocol terminal.
	/// </summary>
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
			@"Send EOL sequence ""OK\!(" + (Parser.XKeyword)Parser.Keyword.Eol + @")""" + Environment.NewLine +
			@"Do not send EOL ""OK\!(" + (Parser.XKeyword)Parser.Keyword.NoEol + @")""" + Environment.NewLine +
			@"""\!(" + (Parser.XKeyword)Parser.Keyword.NoEol + @")"" is useful if your text protocol does have an EOL sequence" + Environment.NewLine +
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "Private element.")]
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

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private LineState txLineState;
		private LineState rxLineState;

		private BidirLineState bidirLineState;

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
			if (disposing)
			{
				// Nothing to do (yet).
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

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public override void Send(string s)
		{
			AssertNotDisposed();
			Send(s, null);
		}

		/// <summary></summary>
		public override void SendLine(string line)
		{
			AssertNotDisposed();
			Send(line, TextTerminalSettings.TxEol);
		}

		private void Send(string s, string eol)
		{
			Parser.SubstitutionParser p = new Parser.SubstitutionParser(TerminalSettings.IO.Endianess, (XEncoding)TextTerminalSettings.Encoding);

			// Prepare EOL.
			MemoryStream eolWriter = new MemoryStream();
			foreach (Parser.Result result in p.Parse(eol, TextTerminalSettings.CharSubstitution, Parser.ParseMode.AllByteArrayResults))
			{
				if (result is Parser.ByteArrayResult)
				{
					byte[] a = ((Parser.ByteArrayResult)result).ByteArray;
					eolWriter.Write(a, 0, a.Length);
				}
			}
			byte[] eolByteArray = eolWriter.ToArray();

			// Parse string and execute keywords.
			bool sendEol = (eol != null);
			foreach (Parser.Result result in p.Parse(s, TextTerminalSettings.CharSubstitution, Parser.ParseMode.All))
			{
				if (result is Parser.ByteArrayResult)
				{
					Send(((Parser.ByteArrayResult)result).ByteArray);
				}
				else if (result is Parser.KeywordResult)
				{
					switch (((Parser.KeywordResult)result).Keyword)
					{
						case Parser.Keyword.Delay:
							// \fixme
							break;

						case Parser.Keyword.Eol:
							Send(eolByteArray);
							break;

						case Parser.Keyword.NoEol:
							sendEol = false;
							break;
					}
				}
			}

			// Finally send EOL.
			if (sendEol)
			{
				Send(eolByteArray);
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

			Parser.SubstitutionParser p = new Parser.SubstitutionParser(TerminalSettings.IO.Endianess, (XEncoding)TextTerminalSettings.Encoding);
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
		}

		/// <summary></summary>
		protected override DisplayElement ByteToElement(byte b, SerialDirection d, Radix r)
		{
			switch (r)
			{
				// Bin/Oct/Dec/Hex
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					return (base.ByteToElement(b, d, r));
				}

				// Char/String
				case Radix.Char:
				case Radix.String:
				{
					// Add byte to ElementState
					this.rxDecodingStream.Add(b);
					byte[] decodingArray = this.rxDecodingStream.ToArray();

					// Get encoding and retrieve char count
					Encoding e = (XEncoding)TextTerminalSettings.Encoding;
					int charCount = e.GetCharCount(decodingArray);

					// If decoding array can be decoded into something useful, decode it
					if (charCount == 1)
					{
						// Char count must be 1, otherwise something went wrong
						char[] chars = new char[charCount];
						if (e.GetDecoder().GetChars(decodingArray, 0, decodingArray.Length, chars, 0, true) == 1)
						{
							// Ensure that 'unknown' character 0xFFFD is not decoded yet
							int code = (int)chars[0];
							if (code != 0xFFFD)
							{
								this.rxDecodingStream.Clear();

								if ((code < 0x20) || (code == 0x7F)) // Control chars
								{
									return (base.ByteToElement(b, d, r));
								}
								else if (b == 0x20) // Space
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
								// 'unknown' character 0xFFFD, do not reset stream
							}
						}
					}
					else
					{
						// Nothing useful to decode into, reset stream
						this.rxDecodingStream.Clear();
					}

					// Nothing to decode (yet)
					return (new DisplayElement.NoData());
				}
				default: throw (new NotImplementedException("Unknown Radix"));
			}
		}

		private void ExecuteLineBegin(LineState lineState, DateTime ts, DisplayElementCollection elements)
		{
			if (TerminalSettings.Display.ShowTimeStamp)
			{
				DisplayLinePart lp = new DisplayLinePart();

				lp.Add(new DisplayElement.TimeStamp(ts));
				lp.Add(new DisplayElement.LeftMargin());

				// Attention: Clone elements because they are needed again below
				lineState.LineElements.AddRange(lp.Clone());
				elements.AddRange(lp);
			}
			lineState.LinePosition = LinePosition.Data;
		}

		private void ExecuteData(LineState lineState, SerialDirection d, byte b, DisplayElementCollection elements)
		{
			DisplayLinePart lp = new DisplayLinePart();

			// Add space if necessary
			if (ElementsAreSeparate(d))
			{
				int lineLength = 0;
				foreach (DisplayElement le in lineState.LineElements)
				{
					if (le.IsData)
						lineLength++;
				}
				if (lineLength > 0)
				{
					lp.Add(new DisplayElement.Space());
				}
			}

			// Process data
			DisplayElement de = ByteToElement(b, d);

			// Evaluate EOL, i.e. check whether EOL is about to start or has already started
			lineState.Eol.Enqueue(b);
			if (lineState.Eol.IsCompleteMatch)
			{
				if (de.IsData)
					lineState.EolElements.Add(de);

				// Retrieve EOL elements, marking them as EOL
				foreach (DisplayElement item in lineState.EolElements)
				{
					item.IsEol = true;
					lp.Add(item);
				}
				lineState.EolElements.Clear();
				lineState.LinePosition = LinePosition.End;
			}
			else if (lineState.Eol.IsPartlyMatch)
			{
				// Keep EOL elements but delay them until EOL is complete
				if (de.IsData)
					lineState.EolElements.Add(de);
			}
			else
			{
				// Retrieve potential EOL elements on incomplete EOL
				if (lineState.EolElements.Count > 0)
				{
					lp.AddRange(lineState.EolElements);
					lineState.EolElements.Clear();
				}

				// Add non-EOL data
				if (de.IsData)
					lp.Add(de);
			}

			// Return data
			// Attention: Clone elements because they are needed again below
			lineState.LineElements.AddRange(lp.Clone());
			elements.AddRange(lp);
		}

		private void ExecuteLineEnd(LineState lineState, SerialDirection d, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			// Process EOL
			int eolLength = lineState.Eol.Eol.Length;
			DisplayLine line = new DisplayLine();

			if (TextTerminalSettings.ShowEol || (eolLength <= 0) || (!lineState.Eol.IsCompleteMatch))
			{
				line.AddRange(lineState.LineElements);
			}
			else // Remove EOL
			{
				int eolAndWhiteCount = 0;
				DisplayElement[] des = lineState.LineElements.ToArray();

				// Traverse elements reverse and count EOL and white spaces to be removed
				for (int i = (des.Length - 1); i >= 0; i--)
				{
					// Detect last non-EOL data element
					if (des[i].IsData && !des[i].IsEol)
						break;

					eolAndWhiteCount++;
				}

				// Now traverse elements forward and add elements to line
				for (int i = 0; i < (des.Length - eolAndWhiteCount); i++)
					line.Add(des[i]);

				// Finally, remove EOL and white spaces from elements
				if (elements.Count >= eolAndWhiteCount)
					elements.RemoveAtEnd(eolAndWhiteCount);
			}

#if (FALSE)
			// \remind
			// Break debugger on SIR responses that are mixed up (more than 16 chars without EOL)
			// Used to debug an issue that is possibly related to #2455804
			//
			// 2009-08-16 / mky
			// Ran more than 50'000 SIR responses without getting a break here.
			//
			if ((d == SerialDirection.Rx) && (line.DataCount > 16))
				System.Diagnostics.Debugger.Break();
#endif

			// Process line length
			DisplayLinePart lp = new DisplayLinePart();
			if (TerminalSettings.Display.ShowLength)
			{
				lp.Add(new DisplayElement.RightMargin());
				lp.Add(new DisplayElement.LineLength(line.DataCount));
			}
			lp.Add(new DisplayElement.LineBreak(d));

			// Add line end to elements and return them
			elements.AddRange(lp);

			// Also add line end to line and return it
			// Attention: Clone elements because they've also needed above
			line.AddRange(lp.Clone());
			lines.Add(line);

			// Reset line state
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
				// Line begin and time stamp
				if (lineState.LinePosition == LinePosition.Begin)
					ExecuteLineBegin(lineState, re.TimeStamp, elements);

				// Data
				ExecuteData(lineState, re.Direction, b, elements);

				// Line end and length
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
					LineState lineState; // Attention: Direction changed => Use opposite state
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
			// Check whether direction has changed
			ProcessAndSignalDirectionLineBreak(re.Direction);

			// Process the raw element
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
			TerminalSettings.TextTerminal.Changed += new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(TextTerminalSettings_Changed);
		}

		private void DetachTextTerminalSettings()
		{
			TerminalSettings.TextTerminal.Changed -= new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(TextTerminalSettings_Changed);
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

		private void TextTerminalSettings_Changed(object sender, MKY.Utilities.Settings.SettingsEventArgs e)
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
