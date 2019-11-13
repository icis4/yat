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
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;

using MKY.Text;

using YAT.Application.Utilities;
using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="TextTerminal"/>.
	/// </remarks>
	public partial class TextTerminal
	{
		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Send Data
		//------------------------------------------------------------------------------------------
		// Send Data
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public override void SendFileLine(string dataLine, Radix defaultRadix)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.File.ToParseMode();

			DoSendData(new TextDataSendItem(dataLine, defaultRadix, parseMode, SendMode.File, true));
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

					// Reevaluate whether to skip the line on [Send File], it may have been non-empty when enqueuing, but now empty:
					if (string.IsNullOrEmpty(textToParse) && (item.SendMode == SendMode.File) && TerminalSettings.Send.File.SkipEmptyLines)
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
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(textToParse, textSuccessfullyParsed)));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		protected override void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Eol:
				{
					AppendToPendingPacketAndForwardToRawTerminal(this.txUnidirTextLineState.EolSequence); // In-line.
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
		/// \remind (2018-01-17 / MKY / FR #333) there is a limitation in this implementation:
		/// This method will be called slightly after the line content has been forwarded to the raw
		/// terminal, thus may result in a short delay between content and EOL. With 9600 baud, no
		/// delay is noticeable. With 115200 baud, delay is still not noticeable most of the times,
		/// but sometimes it's 30 ms! For most use cases this doesn't matter. But still, it is not
		/// ideal, as behavior doesn't reproduce.
		/// On the other hand, there are also drawbacks in refactoring the current implementation.
		/// Already tried once in 2015 (while working between TextTerminal.cs SVN revisions 680 and
		/// 695), but reverted again.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		protected override void ProcessLineEnd(bool sendEol)
		{
			if (sendEol)
				AppendToPendingPacketAndForwardToRawTerminal(this.txUnidirTextLineState.EolSequence); // EOL.
			else
				ForwardPendingPacketToRawTerminal(); // Not the best approach to require this call at so many locations...
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

		#region Send File
		//------------------------------------------------------------------------------------------
		// Send File
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected override void ProcessSendFileItem(FileSendItem item)
		{
			try
			{
				if (ExtensionHelper.IsXmlFile(item.FilePath))
				{
					ProcessSendXmlFileItem(item);
				}
				else if (ExtensionHelper.IsRtfFile(item.FilePath))
				{
					string[] lines;
					RtfReaderHelper.LinesFromRtfFile(item.FilePath, out lines); // Read all at once for simplicity.
					foreach (string line in lines)
					{
						if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
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
				else // By default treat as text file:
				{
					ProcessSendTextFileItem(item, (EncodingEx)TextTerminalSettings.Encoding);
				}
			}
			catch (Exception ex)
			{
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, @"Error reading file """ + item.FilePath + @""": " + ex.Message));
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
