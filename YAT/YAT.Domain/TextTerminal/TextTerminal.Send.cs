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
// YAT Version 2.4.0
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using MKY.Text;

using YAT.Application.Utilities;
using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="TextTerminal"/>.
	/// </remarks>
	public partial class TextTerminal
	{
		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected override void DoSendTextItem(ForSomeTimeEventHelper forSomeTimeEventHelper, TextSendItem item)
		{
			string textToParse = item.Text;

			// Check for text exclusion patterns:
			if (TextTerminalSettings.TextExclusion.Enabled && !string.IsNullOrEmpty(textToParse))
			{
				foreach (var r in TextTerminalSettings.TextExclusion.Regexes)
				{
					var m = r.Match(textToParse);
					if (m.Success)
						textToParse = textToParse.Remove(m.Index, m.Length);

					// Reevaluate whether to skip the line on [Send File], it may have been non-empty when enqueuing, but now empty:
					if (string.IsNullOrEmpty(textToParse) && (item.SendMode == SendMode.File) && TerminalSettings.Send.File.SkipEmptyLines)
						return;
				}
			}

			// Parse the item data:
			Parser.Result[] parseResult;
			string textSuccessfullyParsed;
			if (TryParse(textToParse, item.DefaultRadix, item.ParseMode, out parseResult, out textSuccessfullyParsed))
				DoSendText(forSomeTimeEventHelper, parseResult, item.IsLine);
			else
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, CreateParserErrorMessage(textToParse, textSuccessfullyParsed)));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		protected override void ProcessInLineKeywords(ForSomeTimeEventHelper forSomeTimeEventHelper, Parser.KeywordResult result, Queue<byte> conflateDataQueue, ref bool doBreak, ref bool packetGateAlreadyLeft)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Eol:
				{                                           // Complete and forward the pending packets.
					ProcessLineEnd(forSomeTimeEventHelper, true, conflateDataQueue, ref doBreak); // Would also result in completing and immediately forwarding, but not handle 'WaitForResponse'.
				////AppendToPendingPacketAndForwardToRawTerminal(this.txUnidirTextLineState.EolSequence, conflateDataQueue);
				////base.ProcessInLineKeywords(forSomeTimeEventHelper, result, conflateDataQueue) must not be called as keyword has fully been processed. Calling it would result in an error message!
					break;
				}

				default:
				{
					base.ProcessInLineKeywords(forSomeTimeEventHelper, result, conflateDataQueue, ref doBreak, ref packetGateAlreadyLeft);
					break;
				}
			}
		}

		/// <summary></summary>
		protected override void ProcessLineEnd(ForSomeTimeEventHelper forSomeTimeEventHelper, bool appendEol, Queue<byte> conflateDataQueue, ref bool doBreak)
		{
			if (appendEol)           // Just append the EOL, the base method will forward the completed line.
				AppendToPendingPacketWithoutForwardingToRawTerminalYet(TxEolSequence, conflateDataQueue);

			if (TextTerminalSettings.WaitForResponse.Enabled)
				doBreak = !GetLineClearance(forSomeTimeEventHelper); // See below why there is no need to handle result.

			base.ProcessLineEnd(forSomeTimeEventHelper, appendEol, conflateDataQueue, ref doBreak); // Call base method in any case. Minor limitation:
		}                                                                                           // Some overhead if no clearance and terminal got closed.

		/// <summary></summary>
		protected override int ProcessLineDelayOrInterval(ForSomeTimeEventHelper forSomeTimeEventHelper, bool performLineDelay, int lineDelay, bool performLineInterval, int lineInterval, DateTime lineBeginTimeStamp, DateTime lineEndTimeStamp)
		{
			int accumulatedLineDelay = base.ProcessLineDelayOrInterval(forSomeTimeEventHelper, performLineDelay, lineDelay, performLineInterval, lineInterval, lineBeginTimeStamp, lineEndTimeStamp);

			if (TerminalSettings.TextTerminal.LineSendDelay.Enabled)
			{
				this.lineSendDelayState.LineCount++;                                                     // Interval here means "each n lines".
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

		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected override void DoSendFileItem(ForSomeTimeEventHelper forSomeTimeEventHelper, FileSendItem item)
		{
			try
			{
				if (ExtensionHelper.IsXmlFile(item.FilePath))
				{
					DoSendXmlFileItem(forSomeTimeEventHelper, item);
				}
				else if (ExtensionHelper.IsRtfFile(item.FilePath))
				{
					DoSendRtfFileItem(forSomeTimeEventHelper, item);
				}
				else // By default treat as text file:
				{
					DoSendTextFileItem(forSomeTimeEventHelper, item, (EncodingEx)TextTerminalSettings.Encoding);
				}
			}
			catch (Exception ex)
			{
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, @"Error reading file """ + item.FilePath + @""": " + ex.Message));
			}
		}

		/// <remarks>
		/// <paramref name="forSomeTimeEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendRtfFileItem(ForSomeTimeEventHelper forSomeTimeEventHelper, FileSendItem item)
		{
			string[] lines;
			RtfReaderHelper.LinesFromRtfFile(item.FilePath, out lines); // Read file at once for simplicity. Minor limitation:
			foreach (string line in lines)                              // "forSomeTimeEventHelper.RaiseEventIf..." will
			{                                                           // only be evaluated at DoSendFileLine() below.
				if (string.IsNullOrEmpty(line) && TerminalSettings.Send.File.SkipEmptyLines)
					continue;

				DoSendFileLine(forSomeTimeEventHelper, line, item.DefaultRadix);

				if (DoBreak)
					break;

				// Actively yield to other threads to make sure app stays responsive while looping:
				Thread.Sleep(TimeSpan.Zero); // "TimeSpan.Zero" = 100% CPU is OK as DoSendFileLine()
			}                                // will sleep depending on state of the event helper.
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
