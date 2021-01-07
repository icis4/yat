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
// YAT Version 2.2.0 Development
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
using System.Linq;

using MKY;
using MKY.Collections;
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

		/// <remarks>
		/// <c>private</c> rather than <c>protected override</c> because method depends on code
		/// sequence in constructors.
		/// </remarks>
		private void InitializeProcess()
		{
			using (var p = new Parser.Parser((EncodingEx)BinaryTerminalSettings.EncodingFixed, TerminalSettings.IO.Endianness, Parser.Mode.RadixAndAsciiEscapes))
			{
				// Tx states:
				{
					byte[] txSequenceBreakBefore;
					if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakBefore.Sequence, out txSequenceBreakBefore))
						txSequenceBreakBefore = null;

					byte[] txSequenceBreakAfter;
					if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakAfter.Sequence, out txSequenceBreakAfter))
						txSequenceBreakAfter = null;

					this.binaryTxState      = new BinaryUnidirState(txSequenceBreakBefore, txSequenceBreakAfter);
					this.binaryBidirTxState = new BinaryUnidirState(txSequenceBreakBefore, txSequenceBreakAfter);
				}

				// Rx states:
				{
					byte[] rxSequenceBreakBefore;
					if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakBefore.Sequence, out rxSequenceBreakBefore))
						rxSequenceBreakBefore = null;

					byte[] rxSequenceBreakAfter;
					if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakAfter.Sequence, out rxSequenceBreakAfter))
						rxSequenceBreakAfter = null;

					this.binaryBidirRxState = new BinaryUnidirState(rxSequenceBreakBefore, rxSequenceBreakAfter);
					this.binaryRxState      = new BinaryUnidirState(rxSequenceBreakBefore, rxSequenceBreakAfter);
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
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!"               + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="binaryTxState"/>, <see cref="binaryRxState"/>,
		/// <see cref="binaryBidirTxState"/>, <see cref="binaryBidirRxState"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unidir", Justification = "Orthogonality with 'Bidir'.")]
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
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!"               + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Temporary WITH_SCRIPTING.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected override void ProcessByteOfChunk(RepositoryType repositoryType,
		                                           byte b, DateTime ts, string dev, IODirection dir,
		                                           bool isFirstByteOfChunk, bool isLastByteOfChunk,
		                                           ref DisplayElementCollection elementsToAdd, ref DisplayLineCollection linesToAdd,
	#if (WITH_SCRIPTING)
		                                           ref ScriptLineCollection receivedScriptLinesToAdd,
	#endif
		                                           out bool breakChunk)
		{
			breakChunk = false; // \Remind (2020-05-16 / MKK): Does 'elementsForNextLine' really work in every case?

			var processState = GetProcessState(repositoryType);
			var lineState = processState.Line; // Convenience shortcut.

			var elementsForNextLine = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
		#if (WITH_SCRIPTING)
			var linePositionEndAppliesToScriptLines = false;
		#endif

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
				DoLineBegin(repositoryType, processState, ts, dev, dir, ref elementsToAdd);
			}

			if (lineState.Position == LinePosition.Content)
			{
			#if (WITH_SCRIPTING)
				DoLineContent(repositoryType, processState, b, ts, dev, dir, ref elementsToAdd, ref elementsForNextLine, out linePositionEndAppliesToScriptLines);
			#else
				DoLineContent(repositoryType, processState, b, ts, dev, dir, ref elementsToAdd, ref elementsForNextLine);
			#endif
			}

			if (lineState.Position == LinePosition.End)                                             // Implicitly means 'AppliesToScriptingIfFramed' since flag will
			{                                                                                       // only be set when sequence before/after is active and complete.
			#if (WITH_SCRIPTING)
				DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd, linePositionEndAppliesToScriptLines, ref receivedScriptLinesToAdd);
			#else
				DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd);
			#endif

				// In case of elements for next line immediately flush and start the new line:
				if (!ICollectionEx.IsNullOrEmpty(elementsForNextLine))
				{
					Flush(repositoryType, elementsToAdd, linesToAdd);
					                                         //// Potentially same time stamp as end of previous line, since time stamp belongs to chunk.
					DoLineBegin(repositoryType, processState, ts, dev, dir, ref elementsToAdd);

					foreach (var de in elementsForNextLine)
					{
						if (de.Origin != null) // Foreach element where origin exists.
						{
							foreach (var origin in de.Origin)
							{
								foreach (var originByte in origin.Value1)
								{
									DisplayElementCollection elementsForNextLineDummy = null;
								#if (WITH_SCRIPTING)
									bool linePositionEndAlsoAppliesToScriptingDummy;
									DoLineContent(repositoryType, processState, originByte, ts, dev, dir, ref elementsToAdd, ref elementsForNextLineDummy, out linePositionEndAlsoAppliesToScriptingDummy);
								#else
									DoLineContent(repositoryType, processState, originByte, ts, dev, dir, ref elementsToAdd, ref elementsForNextLineDummy);
								#endif
								}
							}
						}
					} // foreach (elementForNextLine)
				} // if (has elementsForNextLine)
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Line breaks like length based "word wrap" only apply to scripting if the message is not framed, i.e.:
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </summary>
		protected override bool IsNotFramedAndThusAppliesToScriptLines
		{
			get
			{                                                                    // 'ScriptLines' only apply to Rx.
				var binaryDisplaySettings = GetBinaryDisplaySettings(IODirection.Rx);
				return (!(binaryDisplaySettings.SequenceLineBreakBefore.Enabled || binaryDisplaySettings.SequenceLineBreakAfter.Enabled));
			}
		}

	#endif

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
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
				if (TerminalSettings.Display.ShowTimeSpan)  { lineState.Elements.ReplaceTimeSpan( ts - TimeSpanBase,                               TerminalSettings.Display.TimeSpanFormat,                                            left, right); doReplace = true; }
				if (TerminalSettings.Display.ShowTimeDelta) { lineState.Elements.ReplaceTimeDelta(ts - processState.Overall.PreviousLineTimeStamp, TerminalSettings.Display.TimeDeltaFormat,                                           left, right); doReplace = true; }

				if (doReplace)
					FlushReplaceAlreadyBeganLine(repositoryType, lineState);
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected override void DoLineBegin(RepositoryType repositoryType, ProcessState processState,
		                                    DateTime ts, string dev, IODirection dir,
		                                    ref DisplayElementCollection elementsToAdd)
		{
			base.DoLineBegin(repositoryType, processState, ts, dev, dir, ref elementsToAdd);

			var lineState = processState.Line; // Convenience shortcut.
			var lp = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.

			lp.Add(new DisplayElement.LineStart());

			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowDevice    ||
			    TerminalSettings.Display.ShowDirection)
			{
				DisplayElementCollection info;
				PrepareLineBeginInfo(ts, (ts - TimeSpanBase), (ts - processState.Overall.PreviousLineTimeStamp), dev, dir, out info);
				lp.AddRange(info);
			}

			lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.

			CreateCollectionIfIsNull(ref elementsToAdd);
			elementsToAdd.AddRange(lp);
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Temporary WITH_SCRIPTING.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void DoLineContent(RepositoryType repositoryType, ProcessState processState,
		                           byte b, DateTime ts, string dev, IODirection dir,
	#if (WITH_SCRIPTING)
		                           ref DisplayElementCollection elementsToAdd, ref DisplayElementCollection elementsForNextLine,
		                           out bool linePositionEndAppliesToScriptLines)
	#else
		                           ref DisplayElementCollection elementsToAdd, ref DisplayElementCollection elementsForNextLine)
	#endif
		{
		#if (WITH_SCRIPTING)
			linePositionEndAppliesToScriptLines = false;
		#endif

			var lineState   = processState.Line;   // Convenience shortcut.
		#if (WITH_SCRIPTING)
			var scriptState = processState.Script; // Convenience shortcut.
		#endif

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

					CreateCollectionIfIsNull(ref elementsForNextLine);
					ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryUnidirState, dir, elementsForNextLine);

					lineState.Position = LinePosition.End;
				#if (WITH_SCRIPTING)
					linePositionEndAppliesToScriptLines = true;
				#endif
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

			if (!lineState.Exceeded)
			{
				var totalByteCount = (lineState.Elements.ByteCount + lp.ByteCount);
				if (totalByteCount <= TerminalSettings.Display.MaxLineLength)
				{
					lineState.Elements.AddRange(lp.Clone()); // Clone elements because they are needed again a line below.

					CreateCollectionIfIsNull(ref elementsToAdd);
					elementsToAdd.AddRange(lp);
				}
				else
				{
					lineState.Exceeded = true; // Keep in mind and notify once:
					                //// Using term "bytes" rather than "octets" as that is more common, and .NET uses 'Byte' as well.
					var message = "Maximal number of bytes per line exceeded! Check the line break settings in Terminal > Settings > Binary or increase the limit in Terminal > Settings > Advanced.";
					lineState.Elements.Add(new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));

					CreateCollectionIfIsNull(ref elementsToAdd);
					elementsToAdd.Add(     new DisplayElement.ErrorInfo(ts, (Direction)dir, message, true));
				}
			}

		#if (WITH_SCRIPTING)
			// Apply to scripting:
			if (!IsReloading && ScriptRunIsActive)
			{
				if (!IsByteToHide(b))
					scriptState.Data.Add(b);
			}
		#endif

			// Only continue evaluation if no line break detected yet (cannot have more than one line break):
			//  1. Evaluate the tricky case: Sequence before.
			//  2. Evaluate the other easy case: Sequence after.
			//  3. Evaluate the easiest case: Length line break.
			if ((lineState.Position != LinePosition.End) && (binaryDisplaySettings.SequenceLineBreakAfter.Enabled))
			{
				if (!binaryUnidirState.SequenceAfterOfGivenDevice.ContainsKey(dev))                                            // It is OK to only access or add to the collection,
					binaryUnidirState.SequenceAfterOfGivenDevice.Add(dev, new SequenceQueue(binaryUnidirState.SequenceAfter)); // this will not lead to excessive use of memory,
				                                                                                                               // since there is only a given number of devices.
				binaryUnidirState.SequenceAfterOfGivenDevice[dev].Enqueue(b);                                                  // Applies to TCP and UDP server terminals only.
				if (binaryUnidirState.SequenceAfterOfGivenDevice[dev].IsCompleteMatch) // No need to check for partly matches.
				{
					lineState.Position = LinePosition.End;
				#if (WITH_SCRIPTING)
					linePositionEndAppliesToScriptLines = true;
				#endif
				}
			}

			if ((lineState.Position != LinePosition.End) && (binaryDisplaySettings.LengthLineBreak.Enabled))
			{
				if (lineState.Elements.ByteCount >= binaryDisplaySettings.LengthLineBreak.Length)
				{
					lineState.Position = LinePosition.End;
				#if (WITH_SCRIPTING)
					linePositionEndAppliesToScriptLines = IsNotFramedAndThusAppliesToScriptLines; // Length line breaks, i.e. "word wrap", shall not effect scripting. If ever needed, an [advanced configuration of scripting behavior] shall be added.
				#endif
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Temporary WITH_SCRIPTING.")]
		protected override void DoLineEnd(RepositoryType repositoryType, ProcessState processState,
		                                  DateTime ts, IODirection dir,
	#if (WITH_SCRIPTING)
		                                  ref DisplayElementCollection elementsToAdd, ref DisplayLineCollection linesToAdd,
		                                  bool appliesToScriptLines, ref ScriptLineCollection receivedScriptLinesToAdd)
	#else
		                                  ref DisplayElementCollection elementsToAdd, ref DisplayLineCollection linesToAdd)
	#endif
		{
			var lineState = processState.Line; // Convenience shortcut.

			var binaryUnidirState = GetBinaryUnidirState(repositoryType, lineState.Direction);

			// In case of e.g. a timed line break, retained potential sequence elements can be treated as non-sequence:
			ReleaseRetainedUnconfirmedHiddenSequenceBefore(lineState, binaryUnidirState, dir, lineState.Elements);

			// Note that it is OK to release the elements above, as binary terminals always show all bytes.
			// This is opposed to text terminals where potential EOL elements are potentially hidden.

			// Process line length/duration:
			var lineEnd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			if (TerminalSettings.Display.ShowLength || TerminalSettings.Display.ShowDuration) // Meaning: "byte count" and "line duration".
			{
				var length = lineState.Elements.ByteCount;
				var duration = (ts - lineState.TimeStamp);

				DisplayElementCollection info;
				PrepareLineEndInfo(length, duration, out info);
				lineEnd.AddRange(info);
			}

			lineEnd.Add(new DisplayElement.LineBreak());

			CreateCollectionIfIsNull(ref elementsToAdd);
			elementsToAdd.AddRange(lineEnd.Clone()); // Clone elements because they are needed again right below.

			// Finalize line:
			var l = new DisplayLine(lineState.Elements.Count + lineEnd.Count); // Preset the required capacity to improve memory management.
			l.AddRange(lineState.Elements.Clone()); // Clone to ensure decoupling.
			l.AddRange(lineEnd);

			if (lineState.Highlight)
				l.Highlight = true;

			CreateCollectionIfIsNull(ref linesToAdd);
			linesToAdd.Add(l);

		#if (WITH_SCRIPTING)
			// Apply to scripting:                                                     // 'ScriptLines' only apply to Rx.
			if (!IsReloading && ScriptRunIsActive && (repositoryType == RepositoryType.Rx))
			{
				if (appliesToScriptLines)
				{
					var scriptState = processState.Script; // Convenience shortcut.
					var data = new List<byte>(scriptState.Data); // Clone to ensure decoupling.

					var removeXOnXOff = (TerminalSettings.IO.SerialPort.Communication.FlowControlUsesXOnXOff && TerminalSettings.CharHide.HideXOnXOff);
					if (removeXOnXOff) // XOn/XOff doesn't make much sense for binary terminals, but users may still use it.
					{
						data.RemoveAll(b => b == MKY.IO.Serial.XOnXOff.XOnByte);
						data.RemoveAll(b => b == MKY.IO.Serial.XOnXOff.XOffByte);
					}

					var duration = (ts - scriptState.TimeStamp); // Attention, the script state's time stamp must be taken! It may differ from the displayed time stamp!

					CreateCollectionIfIsNull(ref receivedScriptLinesToAdd);
					receivedScriptLinesToAdd.Add(new ScriptLine(scriptState.TimeStamp, scriptState.Device, data.ToArray(), duration));
				}
				else
				{
					// This display line end shall not result in a script line end.
				}
			}
		#endif

			// Notify:
		#if (WITH_SCRIPTING)
			base.DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd, appliesToScriptLines, ref receivedScriptLinesToAdd);
		#else
			base.DoLineEnd(repositoryType, processState, ts, dir, ref elementsToAdd, ref linesToAdd);
		#endif
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
