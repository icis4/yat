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

using MKY;

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

		/// <summary>
		/// Initializes the processing state.
		/// </summary>
		protected override void InitializeProcess()
		{
			// Binary unspecifics:
			base.InitializeProcess();

			// Binary specifics:
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
					this.rxBidirBinaryLineState  = new BinaryLineState(new SequenceQueue(rxSequenceBreakAfter), new SequenceQueue(rxSequenceBreakBefore));
				}
			}
		}

		/// <summary>
		/// Resets the processing state for the given <paramref name="repositoryType"/>.
		/// </summary>
		protected override void ResetProcess(RepositoryType repositoryType)
		{
			// Binary unspecifics:
			base.ResetProcess(repositoryType);

			// Binary specifics:
			switch (repositoryType)
			{
				case RepositoryType.Tx:    this.txUnidirBinaryLineState.Reset();                                       break;
				case RepositoryType.Bidir: this.txBidirBinaryLineState .Reset(); this.rxBidirBinaryLineState .Reset(); break;
				case RepositoryType.Rx:                                          this.rxUnidirBinaryLineState.Reset(); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the quasi-private member
		/// <see cref="BinaryTerminalSettings"/>.
		/// </remarks>
		protected Settings.BinaryDisplaySettings GetBinaryDisplaySettings(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (BinaryTerminalSettings.TxDisplay);
				case IODirection.Rx:    return (BinaryTerminalSettings.RxDisplay);

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="txUnidirBinaryLineState"/>, <see cref="rxUnidirBinaryLineState"/>,
		/// <see cref="txBidirBinaryLineState"/>, <see cref="rxBidirBinaryLineState"/>.
		/// </remarks>
		protected BinaryLineState GetBinaryLineState(RepositoryType repositoryType, IODirection dir)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.txUnidirBinaryLineState);
				case RepositoryType.Rx:    return (this.rxUnidirBinaryLineState);

				case RepositoryType.Bidir: if (dir == IODirection.Tx) { return (this.txBidirBinaryLineState); }
				                           else                       { return (this.rxBidirBinaryLineState); }
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
			var processState          = GetProcessState(repositoryType);
			var lineState             = processState.Line; // Convenience shortcut.
			var binaryLineState       = GetBinaryLineState(repositoryType, dir);
			var binaryDisplaySettings = GetBinaryDisplaySettings(dir);

			var elementsForNextLine = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.

			if (lineState.Position == LinePosition.Begin)
			{
				DoLineBegin(repositoryType, processState, ts, dev, dir, elementsToAdd);
			}

			if (lineState.Position == LinePosition.Content)
			{
				DoLineContent(processState, binaryLineState, binaryDisplaySettings, b, ts, dir, elementsToAdd, elementsForNextLine);
			}

			if (lineState.Position == LinePosition.End)
			{
				DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd);

				// In case of elements for next line immediately flush and start the new line:
				if (elementsForNextLine.Count > 0)
				{
					Flush(repositoryType, elementsToAdd, linesToAdd);
					                                         //// Potentially same time stamp as end of previous line, since time stamp belongs to chunk.
					DoLineBegin(repositoryType, processState, ts, dev, dir, elementsToAdd);

					foreach (var de in elementsForNextLine)
					{
						if (de.Origin != null) // Foreach element where origin exists.
						{
							foreach (var origin in de.Origin)
							{
								foreach (var originByte in origin.Value1)
								{
									DisplayElementCollection elementsForNextLineDummy = null;
									DoLineContent(processState, binaryLineState, binaryDisplaySettings, originByte, ts, dir, elementsToAdd, elementsForNextLineDummy);
								}
							}
						}
					} // foreach (elementForNextLine)
				} // if (has elementsForNextLine)
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
		private void DoLineContent(ProcessState processState, BinaryLineState binaryLineState, Settings.BinaryDisplaySettings binaryDisplaySettings,
		                           byte b, DateTime ts, IODirection dir,
		                           DisplayElementCollection elementsToAdd, DisplayElementCollection elementsForNextLine)
		{
			var lineState = processState.Line; // Convenience shortcut.

			// Convert content:
			var de = ByteToElement(b, ts, dir);

			var lp = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((binaryDisplaySettings.SequenceLineBreakBefore.Enabled && (lineState.Elements.ByteCount > 0) &&
				(lineState.Position != LinePosition.End)))             // Also skip if line has just been broken.
			{
				binaryLineState.SequenceBefore.Enqueue(b);
				if (binaryLineState.SequenceBefore.IsCompleteMatch)
				{
					// Sequence is complete, move them to the next line:
					binaryLineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.

					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryLineState, dir, elementsForNextLine);

					lineState.Position = LinePosition.End;
				}
				else if (binaryLineState.SequenceBefore.IsPartlyMatchContinued)
				{
					// Keep sequence elements and delay them until sequence is either complete or refused:
					binaryLineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else if (binaryLineState.SequenceBefore.IsPartlyMatchBeginning)
				{
					// Previous was no match, retained potential sequence elements can be treated as non-sequence:
					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryLineState, dir, lp);

					// Keep sequence elements and delay them until sequence is either complete or refused:
					binaryLineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else
				{
					// No match at all, retained potential sequence elements can be treated as non-sequence:
					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryLineState, dir, lp);
				}
			}

			// Add current element if it wasn't consumed above:
			if (de != null)
			{
				AddContentSeparatorIfNecessary(lineState, dir, lp, de);
				lp.Add(de); // No clone needed as element has just been created further above.
			}

			if (lineState.Position != LinePosition.ContentExceeded)
			{
				lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.
				elementsToAdd.AddRange(lp);
			}

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((binaryDisplaySettings.SequenceLineBreakAfter.Enabled) &&
			    (lineState.Position != LinePosition.End))
			{
				binaryLineState.SequenceAfter.Enqueue(b);
				if (binaryLineState.SequenceAfter.IsCompleteMatch) // No need to check for partly matches.
					lineState.Position = LinePosition.End;
			}

			if ((binaryDisplaySettings.LengthLineBreak.Enabled) &&
			    (lineState.Position != LinePosition.End))
			{
				if (lineState.Elements.ByteCount >= binaryDisplaySettings.LengthLineBreak.Length)
					lineState.Position = LinePosition.End;
			}

			if (lineState.Position != LinePosition.End)
			{
				if ((lineState.Elements.ByteCount > TerminalSettings.Display.MaxLineLength) &&
				    (lineState.Position != LinePosition.ContentExceeded))
				{
					lineState.Position = LinePosition.ContentExceeded;
					                                  //// Using term "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.
					var message = "Maximal number of bytes per line exceeded! Check the line break settings or increase the limit in the advanced terminal settings.";
					lineState.Elements.Add(new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));
					elementsToAdd.Add(     new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));
				}
			}
		}

		private void ReleaseRetainedUnconfirmedHiddenSequenceBefore(LineState lineState, BinaryLineState binaryLineState, IODirection dir, DisplayElementCollection lp)
		{
			if (binaryLineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Count > 0)
			{
				foreach (var de in binaryLineState.RetainedUnconfirmedHiddenSequenceBeforeElements)
				{
					AddContentSeparatorIfNecessary(lineState, dir, lp, de);
					lp.Add(de); // No clone needed as element is no more used below.
				}

				binaryLineState.RetainedUnconfirmedHiddenSequenceBeforeElements.Clear();
			}
		}

		/// <summary></summary>
		protected override void DoLineEnd(RepositoryType repositoryType, ProcessState processState,
		                                  DateTime ts, IODirection dir,
		                                  DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var lineState = processState.Line; // Convenience shortcut.
			BinaryLineState binaryLineState = GetBinaryLineState(repositoryType, lineState.Direction);

			// In case of e.g. a timed line break, retained potential sequence elements can be treated as non-sequence:
			ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryLineState, dir, lineState.Elements);

			// Note that it is OK to release the elements above, as binary terminals always show all bytes.
			// This is opposed to text terminals where potential EOL elements are potentially hidden.

			// Process line length:
			var lineEnd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			if (TerminalSettings.Display.ShowLength || TerminalSettings.Display.ShowDuration) // Meaning: "byte count" and "line duration".
			{
				var length = lineState.Elements.ByteCount;

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

			// Finalize the line:
			binaryLineState.NotifyLineEnd();
			base.DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
