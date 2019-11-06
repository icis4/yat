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
#if (WITH_SCRIPTING)
using System.Collections.Generic;
#endif
using System.Diagnostics.CodeAnalysis;
#if (WITH_SCRIPTING)
using System.Globalization;
using System.Text;
#endif

using MKY;
using MKY.Diagnostics;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the process part of <see cref="BinaryTerminal"/>.
	/// </remarks>
	public partial class BinaryTerminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private BinaryLineState txUnidirBinaryLineState;
		private BinaryLineState txBidirBinaryLineState;
		private BinaryLineState rxBidirBinaryLineState;
		private BinaryLineState rxUnidirBinaryLineState;

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Process Elements
		//------------------------------------------------------------------------------------------
		// Process Elements
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void InitializeProcess()
		{
			using (var p = new Parser.Parser(TerminalSettings.IO.Endianness, Parser.Modes.RadixAndAsciiEscapes))
			{
				// Tx:
				{
					byte[] txSequenceBreakAfter;
					if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakAfter.Sequence, out txSequenceBreakAfter))
						txSequenceBreakAfter = null;

					byte[] txSequenceBreakBefore;
					if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakBefore.Sequence, out txSequenceBreakBefore))
						txSequenceBreakBefore = null;

					this.txUnidirBinaryLineState = new BinaryLineState(new SequenceQueue(txSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore));
					this.txBidirBinaryLineState  = new BinaryLineState(new SequenceQueue(txSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore));
				}

				// Rx:
				{
					byte[] rxSequenceBreakAfter;
					if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakAfter.Sequence, out rxSequenceBreakAfter))
						rxSequenceBreakAfter = null;

					byte[] rxSequenceBreakBefore;
					if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakBefore.Sequence, out rxSequenceBreakBefore))
						rxSequenceBreakBefore = null;

					this.rxUnidirBinaryLineState = new BinaryLineState(new SequenceQueue(rxSequenceBreakAfter), new SequenceQueue(rxSequenceBreakBefore));
					this.rxBidirBinaryLineState = new BinaryLineState(new SequenceQueue(rxSequenceBreakAfter), new SequenceQueue(rxSequenceBreakBefore));
				}
			}
		}

		/// <summary></summary>
		protected override void ProcessRawByte(byte b, LineChunkAttribute attribute)
		{
			// Line begin and time stamp:
			if (lineState.Position == LinePosition.Begin)
				DoLineBegin(lineState, chunk.TimeStamp, chunk.Device, chunk.Direction, elementsToAdd);

			// Content:
			if (lineState.Position == LinePosition.Content)
				DoContent(displaySettings, lineState, chunk.Device, chunk.Direction, b, elementsToAdd, out replaceAlreadyStartedLine);

			// Line end and length:
			if (lineState.Position == LinePosition.End)
				DoLineEnd(lineState, chunk.TimeStamp, chunk.Device, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
		}

		private void DoLineBegin(LineState lineState, DateTime ts, string dev, IODirection dir, DisplayElementCollection elementsToAdd)
		{
			var lineState = GetLineState(repositoryType);
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

			lineState.Position = LinePosition.Content;
			lineState.TimeStamp = ts;
		}

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void DoContent(Settings.BinaryDisplaySettings displaySettings, LineState lineState, IODirection dir, byte b, DisplayElementCollection elementsToAdd, out DisplayElementCollection elementsForNextLine)
		{
			elementsForNextLine = null;

			// Convert content:
			var de = ByteToElement(b, dir);
			de.Highlight = lineState.Highlight;

			var lp = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((displaySettings.SequenceLineBreakBefore.Enabled && (lineState.Elements.ByteCount > 0) &&
				(lineState.Position != LinePosition.End)))       // Also skip if line has just been brokwn.
			{
				lineState.SequenceBefore.Enqueue(b);
				if (lineState.SequenceBefore.IsCompleteMatch)
				{
					// Sequence is complete, move them to the next line:
					lineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.

					elementsForNextLine = new DisplayElementCollection(lineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Capacity); // Preset the required capacity to improve memory management.
					foreach (DisplayElement dePending in lineState.RetainedUnconfirmedHiddenSequenceBeforeElements)
						elementsForNextLine.Add(dePending.Clone());

					lineState.Position = LinePosition.End;
				}
				else if (lineState.SequenceBefore.IsPartlyMatchContinued)
				{
					// Keep sequence elements and delay them until sequence is either complete or refused:
					lineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else if (lineState.SequenceBefore.IsPartlyMatchBeginning)
				{
					// Previous was no match, previous sequence can be treated as normal:
					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, lp);

					// Keep sequence elements and delay them until sequence is either complete or refused:
					lineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else
				{
					// No match at all, previous sequence can be treated as normal:
					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, lp);
				}
			}

			// Add current element if it wasn't consumed above:
			if (de != null)
			{
				AddSpaceIfNecessary(lineState, dir, lp, de);
				lp.Add(de); // No clone needed as element has just been created further above.
			}

			if (lineState.Position != LinePosition.ContentExceeded)
			{
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
			}

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((displaySettings.SequenceLineBreakAfter.Enabled) &&
				(lineState.Position != LinePosition.End))
			{
				lineState.SequenceAfter.Enqueue(b);
				if (lineState.SequenceAfter.IsCompleteMatch) // No need to check for partly matches.
					lineState.Position = LinePosition.End;
			}

			if ((displaySettings.LengthLineBreak.Enabled) &&
				(lineState.Position != LinePosition.End))
			{
				if (lineState.Elements.ByteCount >= displaySettings.LengthLineBreak.Length)
					lineState.Position = LinePosition.End;
			}

			if (lineState.Position != LinePosition.End)
			{
				if ((lineState.Elements.ByteCount > TerminalSettings.Display.MaxLineLength) &&
					(lineState.Position != LinePosition.ContentExceeded))
				{
					lineState.Position = LinePosition.ContentExceeded;
					                                  //// Using term "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.
					var message = "Maximal number of bytes per line exceeded! Check the line break settings or increase the limit in the advanced terminal settings.";
					lineState.Elements.Add(new DisplayElement.ErrorInfo((Direction)dir, message, true));
					elementsToAdd.Add(     new DisplayElement.ErrorInfo((Direction)dir, message, true));
				}
			}
		}

		private void AddSpaceIfNecessary(LineState lineState, IODirection dir, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(dir) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.ByteCount > 0) || (lp.ByteCount > 0))
					lp.Add(new DisplayElement.DataSpace());
			}
		}

		private static void ReleaseRetainedUnconfirmedHiddenSequenceBefore(LineState lineState, DisplayElementCollection lp)
		{
			if (lineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Count > 0)
			{
				lp.AddRange(lineState.RetainedUnconfirmedHiddenSequenceBeforeElements); // No clone needed as collection is cleared below.
				lineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Clear();
			}
		}

		private void DoLineEnd(LineState lineState, DateTime ts, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			// Note: Code sequence the same as ExecuteLineEnd() of TextTerminal for better comparability.

			// Potentially suppress line:
			if (lineState.SuppressForSure || (lineState.SuppressIfNotFiltered && !lineState.AnyFilterDetected)) // Suppress line:
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
				if (TerminalSettings.Display.ShowLength || TerminalSettings.Display.ShowDuration) // Meaning: "byte count" and "line duration".
				{
					DisplayElementCollection info;
					PrepareLineEndInfo(lineState.Elements.ByteCount, (ts - lineState.TimeStamp), out info);
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

			#if (WITH_SCRIPTING)

				if (!IsReloading)
				{
					// \ToDo: Currently limited to enqueing only after a display line has been completed.
					//        This limits cases where the direction changes in the same displayed line.
					//        This also limits cases where packet dimensions and display representation are different.
					//        Handling shall be improved when implementing defect #129 "true support for binary terminals".
					//        Consider to add a dedicated script setting, e.g. Binary > MessageSize.
					var data = new List<byte>(128); // Preset the capacity to improve memory management. 128 is an arbitrary value.
					var message = new StringBuilder();
					foreach (var de in l)
					{
						// Compose received message line for scripting:
						if ((de.Direction == Direction.Rx) && de.IsDataContent)
						{
							foreach (var b in de.ToOrigin())
							{
								data.Add(b);

								if (message.Length > 0)  // Message format for scripting is fixed:
									message.Append(" "); // "For binary terminals, received messages are
								                         //  hexadecimal values, separated with spaces."
								message.Append(b.ToString("X2", CultureInfo.InvariantCulture));
							}
						}

						// Handle potential direction changes inside the displayed line:
						if ((de.Direction != Direction.Rx) && ((data.Count > 0) || (message.Length > 0)))
						{
							EnqueueReceivedMessageForScripting(message.ToString()); // Enqueue before invoking event to
							                                                        // have message ready for event.
							OnScriptPacketReceived(new PacketEventArgs(data.ToArray()));
							OnScriptMessageReceived(new MessageEventArgs(message.ToString()));

							data = new List<byte>(128); // Preset the capacity to improve memory management. 128 is an arbitrary value.
							message = new StringBuilder();
						}
					}

					if ((data.Count > 0) || (message.Length > 0))
					{
						EnqueueReceivedMessageForScripting(message.ToString()); // Enqueue before invoking event to
						                                                        // have message ready for event.
						OnScriptPacketReceived(new PacketEventArgs(data.ToArray()));
						OnScriptMessageReceived(new MessageEventArgs(message.ToString()));
					}
				}
			#endif // WITH_SCRIPTING
			}

			this.bidirLineState.IsFirstLine = false;
			this.bidirLineState.LastLineTimeStamp = lineState.TimeStamp;

			// Reset line state:
			lineState.Reset();
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		private void DoTimedLineBreakOnReload(Settings.BinaryDisplaySettings displaySettings, LineState lineState, DateTime ts,
		                                           DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			if (lineState.Elements.Count > 0)
			{
				var span = ts - lineState.TimeStamp;
				if (span.TotalMilliseconds >= displaySettings.TimedLineBreak.Timeout)
					ExecuteLineEnd(lineState, ts, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
			}

			lineState.TimeStamp = ts;
		}

	#if (WITH_SCRIPTING)
		/// <remarks>
		/// Processing for scripting differs from "normal" processing for displaying because...
		/// ...received messages must not be impacted by 'DirectionLineBreak'.
		/// ...received data must not be processed individually, only as packets/messages.
		/// ...received data must not be reprocessed on <see cref="RefreshRepositories"/>.
		/// </remarks>
		protected override void ProcessRawChunkForScripting(RawChunk chunk)
		{
			// Do nothing, as EnqueueReceivedMessageForScripting(), OnScriptPacketReceived() and
			// OnScriptMessageReceived() is invoked in ExecuteLineEnd() further above. See comment
			// regarding defect #129 "true support for binary terminals" for more information.
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
