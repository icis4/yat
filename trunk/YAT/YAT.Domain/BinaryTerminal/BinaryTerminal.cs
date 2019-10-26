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
	/// <summary>
	/// <see cref="Terminal"/> implementation with binary semantics.
	/// </summary>
	/// <remarks>
	/// This class is implemented using partial classes separating sending/processing functionality.
	/// Using partial classes rather than aggregated sender, processor,... so far for these reasons:
	/// <list type="bullet">
	/// <item><description>Simpler for implementing text/binary specialization.</description></item>
	/// <item><description>Simpler for implementing synchronization among Tx and Rx.</description></item>
	/// <item><description>Less "Durchlauferhitzer", e.g. directly raising events.</description></item>
	/// </list>
	/// </remarks>
	public partial class BinaryTerminal : Terminal
	{
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

		private class LineState : IDisposable, IDisposableEx
		{
			public LinePosition             Position  { get; set; }
			public DisplayElementCollection Elements  { get; set; }
			public DateTime                 TimeStamp { get; set; }

			public SequenceQueue            SequenceAfter                                   { get; set; }
			public SequenceQueue            SequenceBefore                                  { get; set; }
			public DisplayElementCollection RetainedUnconfirmedHiddenSequenceBeforeElements { get; set; }

			public bool Highlight                        { get; set; }
			public bool FilterDetectedInFirstChunkOfLine { get; set; } // Line shall continuously get shown if filter is active from the first chunk.
			public bool FilterDetectedInSubsequentChunk  { get; set; } // Line shall be retained and delay-shown if filter is detected subsequently.
			public bool SuppressIfNotFiltered            { get; set; }
			public bool SuppressIfSubsequentlyTriggered  { get; set; }
			public bool SuppressForSure                  { get; set; }

			public LineBreakTimeout BreakTimeout { get; set; }

			public LineState(SequenceQueue sequenceAfter, SequenceQueue sequenceBefore, LineBreakTimeout breakTimeout)
			{
				Position  = LinePosition.Begin;
				Elements  = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
				TimeStamp = DateTime.Now;

				SequenceAfter                                   = sequenceAfter;
				SequenceBefore                                  = sequenceBefore;
				RetainedUnconfirmedHiddenSequenceBeforeElements = new DisplayElementCollection();

				Highlight                        = false;
				FilterDetectedInFirstChunkOfLine = false;
				FilterDetectedInSubsequentChunk  = false;
				SuppressIfNotFiltered            = false;
				SuppressIfSubsequentlyTriggered  = false;
				SuppressForSure                  = false;

				BreakTimeout = breakTimeout;
			}

			#region Disposal
			//--------------------------------------------------------------------------------------
			// Disposal
			//--------------------------------------------------------------------------------------

			/// <summary></summary>
			public bool IsDisposed { get; protected set; }

			/// <summary></summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary></summary>
			protected virtual void Dispose(bool disposing)
			{
				if (!IsDisposed)
				{
					// Dispose of managed resources if requested:
					if (disposing)
					{
						// In the 'normal' case, the timer is stopped in ExecuteLineEnd().
						if (BreakTimeout != null)
						{
							BreakTimeout.Dispose();
							EventHandlerHelper.RemoveAllEventHandlers(BreakTimeout);

							// \remind (2016-09-08 / MKY)
							// Whole timer handling should be encapsulated into the 'LineState' class.
						}
					}

					// Set state to disposed:
					BreakTimeout = null;
					IsDisposed = true;
				}
			}

		#if (DEBUG)
			/// <remarks>
			/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
			/// "Types that declare disposable members should also implement IDisposable. If the type
			///  does not own any unmanaged resources, do not implement a finalizer on it."
			///
			/// Well, true for best performance on finalizing. However, it's not easy to find missing
			/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
			/// is kept for DEBUG, indicating missing calls.
			///
			/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
			/// </remarks>
			~LineState()
			{
				Dispose(false);

				DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
			}
		#endif // DEBUG

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (IsDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
			}

			#endregion

			public virtual void Reset()
			{
				AssertNotDisposed();

				Position  = LinePosition.Begin;
				Elements  = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
				TimeStamp = DateTime.Now;

				SequenceAfter                                    .Reset();
				SequenceBefore                                   .Reset();
				RetainedUnconfirmedHiddenSequenceBeforeElements = new DisplayElementCollection();

				Highlight                        = false;
				FilterDetectedInFirstChunkOfLine = false;
				FilterDetectedInSubsequentChunk  = false;
				SuppressIfNotFiltered            = false;
				SuppressIfSubsequentlyTriggered  = false;
				SuppressForSure                  = false;
			}

			public virtual bool AnyFilterDetected
			{
				get { return (FilterDetectedInFirstChunkOfLine || FilterDetectedInSubsequentChunk); }
			}
		}

		private class BidirLineState
		{
			public bool IsFirstChunk          { get; set; }
			public bool IsFirstLine           { get; set; }
			public string Device              { get; set; }
			public IODirection Direction      { get; set; }
			public DateTime LastLineTimeStamp { get; set; }

			public BidirLineState()
			{
				IsFirstChunk      = true;
				IsFirstLine       = true;
				Device            = null;
				Direction         = IODirection.None;
				LastLineTimeStamp = DateTime.Now;
			}

			public BidirLineState(BidirLineState rhs)
			{
				IsFirstChunk      = rhs.IsFirstChunk;
				IsFirstLine       = rhs.IsFirstLine;
				Device            = rhs.Device;
				Direction         = rhs.Direction;
				LastLineTimeStamp = rhs.LastLineTimeStamp;
			}
		}

		#endregion

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private LineState txLineState;
		private LineState rxLineState;

		private BidirLineState bidirLineState;

		private object processSyncObj = new object();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings)
			: base(settings)
		{
			AttachBinaryTerminalSettings();
			InitializeStates();
		}

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachBinaryTerminalSettings();

			var casted = (terminal as BinaryTerminal);
			if (casted != null)
			{
				// Tx:

				this.txLineState = casted.txLineState;
				                                           //// \remind (2016-09-08 / MKY)
				if (this.txLineState.BreakTimeout != null)   // Ensure to free referenced resources such as the 'Elapsed' event handler.
					this.txLineState.BreakTimeout.Dispose(); // Whole timer handling should be encapsulated into the 'LineState' class.

				this.txLineState.BreakTimeout = new LineBreakTimeout(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				this.txLineState.BreakTimeout.Elapsed += txTimedLineBreakTimeout_Elapsed;

				// Rx:

				this.rxLineState = casted.rxLineState;
				                                           //// \remind (2016-09-08 / MKY)
				if (this.rxLineState.BreakTimeout != null)   // Ensure to free referenced resources such as the 'Elapsed' event handler.
					this.rxLineState.BreakTimeout.Dispose(); // Whole timer handling should be encapsulated into the 'LineState' class.

				this.rxLineState.BreakTimeout = new LineBreakTimeout(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				this.rxLineState.BreakTimeout.Elapsed += rxTimedLineBreakTimeout_Elapsed;

				// Bidir:

				this.bidirLineState = new BidirLineState(casted.bidirLineState);
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
					DetachBinaryTerminalSettings();

					if (this.txLineState != null)
						this.txLineState.Dispose();

					if (this.rxLineState != null)
						this.rxLineState.Dispose();
				}

				// Set state to disposed:
				this.txLineState = null;
				this.rxLineState = null;
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private Settings.BinaryTerminalSettings BinaryTerminalSettings
		{
			get
			{
				if (TerminalSettings != null)
					return (TerminalSettings.BinaryTerminal);
				else
					return (null);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		private void InitializeStates()
		{
			using (var p = new Parser.Parser(TerminalSettings.IO.Endianness, Parser.Modes.RadixAndAsciiEscapes))
			{
				LineBreakTimeout t;

				// Tx:

				byte[] txSequenceBreakAfter;
				if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakAfter.Sequence, out txSequenceBreakAfter))
					txSequenceBreakAfter = null;

				byte[] txSequenceBreakBefore;
				if (!p.TryParse(BinaryTerminalSettings.TxDisplay.SequenceLineBreakBefore.Sequence, out txSequenceBreakBefore))
					txSequenceBreakBefore = null;

				t = new LineBreakTimeout(BinaryTerminalSettings.TxDisplay.TimedLineBreak.Timeout);
				t.Elapsed += txTimedLineBreakTimeout_Elapsed;

				if (this.txLineState != null) // Ensure to free referenced resources such as the 'Elapsed' event handler of the timer.
					this.txLineState.Dispose();

				this.txLineState = new LineState(new SequenceQueue(txSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), t);

				// Rx:

				byte[] rxSequenceBreakAfter;
				if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakAfter.Sequence, out rxSequenceBreakAfter))
					rxSequenceBreakAfter = null;

				byte[] rxSequenceBreakBefore;
				if (!p.TryParse(BinaryTerminalSettings.RxDisplay.SequenceLineBreakBefore.Sequence, out rxSequenceBreakBefore))
					rxSequenceBreakBefore = null;

				t = new LineBreakTimeout(BinaryTerminalSettings.RxDisplay.TimedLineBreak.Timeout);
				t.Elapsed += rxTimedLineBreakTimeout_Elapsed;

				if (this.rxLineState != null) // Ensure to free referenced resources such as the 'Elapsed' event handler of the timer.
					this.rxLineState.Dispose();

				this.rxLineState = new LineState(new SequenceQueue(rxSequenceBreakAfter), new SequenceQueue(txSequenceBreakBefore), t);
			}

			// Bidir:

			this.bidirLineState = new BidirLineState();
		}

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		private void ExecuteLineBegin(LineState lineState, DateTime ts, string dev, IODirection dir, DisplayElementCollection elementsToAdd)
		{
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

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		private void ExecuteContent(Settings.BinaryDisplaySettings displaySettings, LineState lineState, IODirection dir, byte b, DisplayElementCollection elementsToAdd, out DisplayElementCollection elementsForNextLine)
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

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		private void ExecuteLineEnd(LineState lineState, DateTime ts, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
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

		/// <remarks>
		/// Named "Execute" instead of "Process" to better distinguish this local method from the overall "Process" methods.
		/// Also, the overall "Process" methods synchronize against <see cref="processSyncObj"/> whereas "Execute" don't.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		private void ExecuteTimedLineBreakOnReload(Settings.BinaryDisplaySettings displaySettings, LineState lineState, DateTime ts,
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

		/// <summary></summary>
		protected override void ProcessRawChunk(RawChunk chunk, LineChunkAttribute attribute, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			lock (this.processSyncObj) // Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
			{
				if (linesToAdd.Count <= 0) // Properly initialize the time delta:
					this.bidirLineState.LastLineTimeStamp = chunk.TimeStamp;

				Settings.BinaryDisplaySettings displaySettings;
				switch (chunk.Direction)
				{
					case IODirection.Tx: displaySettings = BinaryTerminalSettings.TxDisplay; break;
					case IODirection.Rx: displaySettings = BinaryTerminalSettings.RxDisplay; break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + chunk.Direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				LineState lineState;
				switch (chunk.Direction)
				{
					case IODirection.Tx: lineState = this.txLineState; break;
					case IODirection.Rx: lineState = this.rxLineState; break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + chunk.Direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				// Activate flags as needed, leave unchanged otherwise.
				// Note that each chunk will either have none or only have a single attribute activated.
				// But the line state has to deal with multiple chunks, thus multiples attribute may get activated.
				// Also note the limitations described in feature request #366 "Automatic response and action shall be...".
				if (attribute == LineChunkAttribute.Highlight)                       {                                                                                     lineState.Highlight                        = true;                                                                }
				if (attribute == LineChunkAttribute.Filter)                          { if (!lineState.AnyFilterDetected) { if (lineState.Position == LinePosition.Begin) { lineState.FilterDetectedInFirstChunkOfLine = true; } else { lineState.FilterDetectedInSubsequentChunk = true; } } }
				if (attribute == LineChunkAttribute.SuppressIfNotFiltered)           { if (!lineState.AnyFilterDetected) {                                                 lineState.SuppressIfNotFiltered            = true;                                                              } }
				if (attribute == LineChunkAttribute.SuppressIfSubsequentlyTriggered) {                                                                                     lineState.SuppressIfSubsequentlyTriggered  = true;                                                                }
				if (attribute == LineChunkAttribute.Suppress)                        {                                                                                     lineState.SuppressForSure                  = true;                                                                }

				// In both cases, filtering and suppression, the current implementation retains the line until it is
				// complete, i.e. until the final decision to filter or suppress could be done. This behavior differs
				// from the standard behavior which continuously shows data as it is coming in.
				//
				// Why this retaining approach? It would be possible to immediately display but then remove the line if it
				// is suppressed or not filtered. But that likely leads to flickering, thus the retaining approach. At the
				// price that there is no longer immediate feedback on single character transmission in case filtering or
				// suppression is active, except in case of filtering when the first chunk of a line already contains the
				// trigger, then the line is continuously shown ('FilterDetectedInFirstChunkOfLine').
				//
				// The test cases of [YAT - Test.ods]::[YAT.Model.Terminal] demonstrate the retaining approach.
				//
				// To change from retaining to continuous approach, the #if (DEBUG) around 'clearAlreadyStartedLine' will
				// have to be removed again. As a consequence, the flag can never get activated, thus excluding it (YAGNI).
				// Still, keeping the implementation to be prepared for potential reactivation (!YAGNI).
				//
				// Note that logging works fine even when filtering or suppression is active, since logging is only
				// triggered by the 'DisplayLinesSent/Received' events and thus not affected by the more tricky to handle
				// 'CurrentDisplayLineSent/ReceivedReplaced' and 'CurrentDisplayLineSent/ReceivedCleared' events.

				foreach (byte b in chunk.Content)
				{
					// In case of reload, timed line breaks are executed here:
					if (IsReloading && displaySettings.TimedLineBreak.Enabled)
						ExecuteTimedLineBreakOnReload(displaySettings, lineState, chunk.TimeStamp, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);

					// Line begin and time stamp:
					if (lineState.Position == LinePosition.Begin)
					{
						ExecuteLineBegin(lineState, chunk.TimeStamp, chunk.Device, chunk.Direction, elementsToAdd);

						if (displaySettings.TimedLineBreak.Enabled)
							lineState.BreakTimeout.Start();
					}
					else
					{
						if (displaySettings.TimedLineBreak.Enabled)
							lineState.BreakTimeout.Restart(); // Restart as timeout refers to time after last received byte.
					}

					// Content:
					DisplayElementCollection elementsForNextLine = null;
					if (lineState.Position == LinePosition.Content)
					{
						ExecuteContent(displaySettings, lineState, chunk.Direction, b, elementsToAdd, out elementsForNextLine);
					}

					// Line end and length:
					if (lineState.Position == LinePosition.End)
					{
						if (displaySettings.TimedLineBreak.Enabled)
							lineState.BreakTimeout.Stop();

						ExecuteLineEnd(lineState, chunk.TimeStamp, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);

						// In case of a pending element immediately insert the sequence into a new line:
						if ((elementsForNextLine != null) && (elementsForNextLine.Count > 0))
						{
							ExecuteLineBegin(lineState, chunk.TimeStamp, chunk.Device, chunk.Direction, elementsToAdd);

							foreach (var de in elementsForNextLine)
							{
								if (de.Origin != null) // Foreach element where origin exists.
								{
									foreach (var origin in de.Origin)
									{
										foreach (var originByte in origin.Value1)
										{
											DisplayElementCollection elementsForNextLineDummy;
											ExecuteContent(displaySettings, lineState, chunk.Direction, originByte, elementsToAdd, out elementsForNextLineDummy);

											// Note that 're.Direction' above is OK, this function is processing all in the same direction.
										}
									}
								}
							} // foreach (elementForNextLine)
						} // if (has elementsForNextLine)
					} // if (LinePosition.End)
				} // foreach (byte)
			} // lock (processSyncObj)
		}

		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		private void ProcessDeviceOrDirectionLineBreak(DateTime ts, string dev, IODirection dir, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			lock (this.processSyncObj) // Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
			{
				if (this.bidirLineState.IsFirstChunk)
				{
					this.bidirLineState.IsFirstChunk = false;
				}
				else // = 'IsSubsequentChunk'.
				{
					if (TerminalSettings.Display.DeviceLineBreakEnabled ||
						TerminalSettings.Display.DirectionLineBreakEnabled)
					{
						if (!StringEx.EqualsOrdinalIgnoreCase(dev, this.bidirLineState.Device) || (dir != this.bidirLineState.Direction))
						{
							LineState lineState;

							if (dir == this.bidirLineState.Direction)
							{
								switch (dir)
								{
									case IODirection.Tx: lineState = this.txLineState; break;
									case IODirection.Rx: lineState = this.rxLineState; break;

									default: throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
								}
							}
							else // Attention: Direction changed => Use other state.
							{
								switch (dir)
								{
									case IODirection.Tx: lineState = this.rxLineState; break; // Reversed!
									case IODirection.Rx: lineState = this.txLineState; break; // Reversed!

									default: throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
								}
							}

							if ((lineState.Elements != null) && (lineState.Elements.Count > 0))
							{
								ExecuteLineEnd(lineState, ts, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
							}
						} // a line break has been detected
					} // a line break is active
				} // is subsequent chunk

				this.bidirLineState.Device = dev;
				this.bidirLineState.Direction = dir;

			} // lock (processSyncObj)
		}

		private void ProcessChunkOrTimedLineBreak(DateTime ts, IODirection dir, DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd, ref bool clearAlreadyStartedLine)
		{
			lock (this.processSyncObj) // Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
			{
				LineState lineState;
				switch (dir)
				{
					case IODirection.Tx: lineState = this.txLineState; break;
					case IODirection.Rx: lineState = this.rxLineState; break;

					default: throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				if (lineState.Elements.Count > 0)
				{
					ExecuteLineEnd(lineState, ts, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
				}
			}
		}

		/// <summary></summary>
		protected override void ProcessAndSignalRawChunk(RawChunk chunk, LineChunkAttribute attribute)
		{
			// Check whether device or direction has changed:
			ProcessAndSignalDeviceOrDirectionLineBreak(chunk.TimeStamp, chunk.Device, chunk.Direction);

			// Process the raw chunk:
			base.ProcessAndSignalRawChunk(chunk, attribute);

			// Enforce line break if requested:
			if (TerminalSettings.Display.ChunkLineBreakEnabled)
				ProcessAndSignalChunkOrTimedLineBreak(chunk.TimeStamp, chunk.Direction);
		}

		private void ProcessAndSignalDeviceOrDirectionLineBreak(DateTime ts, string dev, IODirection dir)
		{
			var directionToSignal = this.bidirLineState.Direction;
			var elementsToAdd = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
			var linesToAdd = new DisplayLineCollection(); // No preset needed, the default initial capacity is good enough.

			bool clearAlreadyStartedLine = false;

			ProcessDeviceOrDirectionLineBreak(ts, dev, dir, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0)
				OnDisplayElementsAdded(directionToSignal, elementsToAdd);

			if (linesToAdd.Count > 0)
				OnDisplayLinesAdded(directionToSignal, linesToAdd);

			if (clearAlreadyStartedLine)
				OnCurrentDisplayLineCleared(directionToSignal);
		}

		private void ProcessAndSignalChunkOrTimedLineBreak(DateTime ts, IODirection dir)
		{
			var elementsToAdd = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
			var linesToAdd = new DisplayLineCollection(); // No preset needed, the default initial capacity is good enough.

			bool clearAlreadyStartedLine = false;

			ProcessChunkOrTimedLineBreak(ts, dir, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0)
				OnDisplayElementsAdded(dir, elementsToAdd);

			if (linesToAdd.Count > 0)
				OnDisplayLinesAdded(dir, linesToAdd);

			if (clearAlreadyStartedLine)
				OnCurrentDisplayLineCleared(dir);
		}

	#if (WITH_SCRIPTING)
		/// <remarks>
		/// Processing for scripting differs from "normal" processing for displaying because...
		/// ...received messages must not be impacted by 'DirectionLineBreak'.
		/// ...received data must not be processed individually, only as packets/messages.
		/// ...received data must not be reprocessed on <see cref="RefreshRepositories"/>.
		/// </remarks>
		protected override void ProcessAndSignalRawChunkForScripting(RawChunk chunk)
		{
			// Do nothing, as EnqueueReceivedMessageForScripting(), OnScriptPacketReceived() and
			// OnScriptMessageReceived() is invoked in ExecuteLineEnd() further above. See comment
			// regarding defect #129 "true support for binary terminals" for more information.
		}
	#endif // WITH_SCRIPTING

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
		protected override void ClearMyRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			InitializeStates();
			base.ClearMyRepository(repositoryType);
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
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (ToExtendedDiagnosticsString()); // No 'real' ToString() method required yet.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override string ToExtendedDiagnosticsString(string indent = "")
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (indent + "> Type: BinaryTerminal" + Environment.NewLine + base.ToExtendedDiagnosticsString(indent));
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Settings
		//------------------------------------------------------------------------------------------
		// Settings
		//------------------------------------------------------------------------------------------

		private void AttachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed += BinaryTerminalSettings_Changed;
		}

		private void DetachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed -= BinaryTerminalSettings_Changed;
		}

		private void ApplyBinaryTerminalSettings()
		{
			InitializeStates();
			RefreshRepositories();
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void BinaryTerminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyBinaryTerminalSettings();
		}

		#endregion

		#region Timer Events
		//------------------------------------------------------------------------------------------
		// Timer Events
		//------------------------------------------------------------------------------------------

		private void txTimedLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			ProcessAndSignalChunkOrTimedLineBreak(DateTime.Now, IODirection.Tx);
		}

		private void rxTimedLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			ProcessAndSignalChunkOrTimedLineBreak(DateTime.Now, IODirection.Rx);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
