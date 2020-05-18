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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

using MKY;
using MKY.Text;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\BinaryTerminal for better separation of the implementation files.
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

		private BinaryUnidirState binaryTxState;
		private BinaryUnidirState binaryBidirTxState;
		private BinaryUnidirState binaryBidirRxState;
		private BinaryUnidirState binaryRxState;

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
			using (var p = new Parser.Parser((EncodingEx)BinaryTerminalSettings.EncodingFixed, TerminalSettings.IO.Endianness, Parser.Mode.RadixAndAsciiEscapes))
			{
				// Tx:
				{
					byte[] txSequenceBreakAfter;
					if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakAfter.Sequence, out txSequenceBreakAfter))
						txSequenceBreakAfter = null;

					byte[] txSequenceBreakBefore;
					if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakBefore.Sequence, out txSequenceBreakBefore))
						txSequenceBreakBefore = null;

					this.binaryTxState      = new BinaryUnidirState(txSequenceBreakAfter, txSequenceBreakBefore);
					this.binaryBidirTxState = new BinaryUnidirState(txSequenceBreakAfter, txSequenceBreakBefore);
				}

				// Rx:
				{
					byte[] rxSequenceBreakAfter;
					if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakAfter.Sequence, out rxSequenceBreakAfter))
						rxSequenceBreakAfter = null;

					byte[] rxSequenceBreakBefore;
					if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakBefore.Sequence, out rxSequenceBreakBefore))
						rxSequenceBreakBefore = null;

					this.binaryBidirRxState = new BinaryUnidirState(rxSequenceBreakAfter, rxSequenceBreakBefore);
					this.binaryRxState      = new BinaryUnidirState(rxSequenceBreakAfter, rxSequenceBreakBefore);
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
				case RepositoryType.Tx:    this.binaryTxState     .Reset();                                  break;
				case RepositoryType.Bidir: this.binaryBidirTxState.Reset(); this.binaryBidirRxState.Reset(); break;
				case RepositoryType.Rx:                                     this.binaryRxState     .Reset(); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="binaryTxState"/>, <see cref="binaryRxState"/>,
		/// <see cref="binaryBidirTxState"/>, <see cref="binaryBidirRxState"/>.
		/// </remarks>
		protected BinaryUnidirState GetBinaryUnidirState(RepositoryType repositoryType, IODirection dir)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.binaryTxState);
				case RepositoryType.Rx:    return (this.binaryRxState);

				case RepositoryType.Bidir: if (dir == IODirection.Tx) { return (this.binaryBidirTxState); }
				                           else                       { return (this.binaryBidirRxState); }
				                           //// Invalid directions are asserted elsewhere.

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected override void DoRawByte(RepositoryType repositoryType,
		                                  byte b, DateTime ts, string dev, IODirection dir,
		                                  DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			var lineState = processState.Line; // Convenience shortcut.

			var elementsForNextLine = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.

			// The first byte of a line will sequentially trigger the [Begin] as well as [Content]
			// condition below. In the normal case, the line will then contain the first displayed
			// element. However, when initially receiving a hidden e.g. <XOn>, the line will yet be
			// empty. Then, when subsequent bytes are received, even when seconds later, the line's
			// initial time stamp is kept. This is illogical, the time stamp of a hidden <XOn> shall
			// not define the time stamp of the line, thus handle such case by rebeginning the line.
			if (lineState.Position == LinePosition.Content)
			{
				DoLineContentCheck(repositoryType, processState, ts, dir);
			}

			if (lineState.Position == LinePosition.Begin)
			{
				DoLineBegin(repositoryType, processState, ts, dev, dir, elementsToAdd);
			}

			if (lineState.Position == LinePosition.Content)
			{
				DoLineContent(repositoryType, processState, b, ts, dev, dir, elementsToAdd, elementsForNextLine);
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
									DoLineContent(repositoryType, processState, originByte, ts, dev, dir, elementsToAdd, elementsForNextLineDummy);
								}
							}
						}
					} // foreach (elementForNextLine)
				} // if (has elementsForNextLine)
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		protected virtual void DoLineContentCheck(RepositoryType repositoryType, ProcessState processState,
		                                          DateTime ts, IODirection dir)
		{
			var lineState = processState.Line; // Convenience shortcut.
			if (lineState.IsYetEmpty)
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
		                           DisplayElementCollection elementsToAdd, DisplayElementCollection elementsForNextLine)
		{
			var lineState = processState.Line; // Convenience shortcut.

			var binaryUnidirState     = GetBinaryUnidirState(repositoryType, dir);
			var binaryDisplaySettings = GetBinaryDisplaySettings(dir);

			// Convert content:
			var de = ByteToElement(b, ts, dir, null); // This binary terminal implementation does not implement multi-byte encodings (yet).

			var lp = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.

			// Evaluate line breaks:
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			// Only continue evaluation if no line break detected yet (cannot have more than one line break).

			if ((binaryDisplaySettings.SequenceLineBreakBefore.Enabled && (lineState.Elements.ByteCount > 0) &&
				(lineState.Position != LinePosition.End)))             // Also skip if line has just been broken.
			{
				if (!binaryUnidirState.SequenceBeforeOfGivenDevice.ContainsKey(dev))                                             // It is OK to only access or add to the collection,
					binaryUnidirState.SequenceBeforeOfGivenDevice.Add(dev, new SequenceQueue(binaryUnidirState.SequenceBefore)); // this will not lead to excessive use of memory,
				                                                                                                                 // since there is only a given number of devices.
				binaryUnidirState.SequenceBeforeOfGivenDevice[dev].Enqueue(b);                                                   // Applies to TCP and UDP server terminals only.
				if (binaryUnidirState.SequenceBeforeOfGivenDevice[dev].IsCompleteMatch)
				{
					// Sequence is complete, move them to the next line:
					binaryUnidirState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.

					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryUnidirState, dir, elementsForNextLine);

					lineState.Position = LinePosition.End;
				}
				else if (binaryUnidirState.SequenceBeforeOfGivenDevice[dev].IsPartlyMatchContinued)
				{
					// Keep sequence elements and delay them until sequence is either complete or refused:
					binaryUnidirState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else if (binaryUnidirState.SequenceBeforeOfGivenDevice[dev].IsPartlyMatchBeginning)
				{
					// Previous was no match, retained potential sequence elements can be treated as non-sequence:
					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryUnidirState, dir, lp);

					// Keep sequence elements and delay them until sequence is either complete or refused:
					binaryUnidirState.RetainedUnconfirmedHiddenSequenceBeforeElements.Add(de); // No clone needed as element has just been created further above.

					de = null; // Indicate that element has been consumed.
				}
				else
				{
					// No match at all, retained potential sequence elements can be treated as non-sequence:
					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryUnidirState, dir, lp);
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

			if ((lineState.Position != LinePosition.End) && (binaryDisplaySettings.SequenceLineBreakAfter.Enabled))
			{
				if (!binaryUnidirState.SequenceAfterOfGivenDevice.ContainsKey(dev))                                            // It is OK to only access or add to the collection,
					binaryUnidirState.SequenceAfterOfGivenDevice.Add(dev, new SequenceQueue(binaryUnidirState.SequenceAfter)); // this will not lead to excessive use of memory,
				                                                                                                               // since there is only a given number of devices.
				binaryUnidirState.SequenceAfterOfGivenDevice[dev].Enqueue(b);                                                  // Applies to TCP and UDP server terminals only.
				if (binaryUnidirState.SequenceAfterOfGivenDevice[dev].IsCompleteMatch) // No need to check for partly matches.
					lineState.Position = LinePosition.End;
			}

			if ((lineState.Position != LinePosition.End) && (binaryDisplaySettings.LengthLineBreak.Enabled))
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

		private void ReleaseRetainedUnconfirmedHiddenSequenceBefore(LineState lineState, BinaryUnidirState binaryUnidirState, IODirection dir, DisplayElementCollection lp)
		{
			if (binaryUnidirState.RetainedUnconfirmedHiddenSequenceBeforeElements.Count > 0)
			{
				foreach (var de in binaryUnidirState.RetainedUnconfirmedHiddenSequenceBeforeElements)
				{
					AddContentSeparatorIfNecessary(lineState, dir, lp, de);
					lp.Add(de); // No clone needed as element is no more used below.
				}

				binaryUnidirState.RetainedUnconfirmedHiddenSequenceBeforeElements.Clear();
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
			var lineState = processState.Line; // Convenience shortcut.

			var binaryUnidirState = GetBinaryUnidirState(repositoryType, lineState.Direction);

			// In case of e.g. a timed line break, retained potential sequence elements can be treated as non-sequence:
			ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryUnidirState, dir, lineState.Elements);

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
