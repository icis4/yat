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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public class ProcessState
	{
		/// <summary></summary>
		public OverallState Overall { get; set; }

		/// <summary></summary>
		public LineState    Line    { get; set; }

		/// <summary></summary>
		public ProcessState()
		{
			Overall = new OverallState();
			Line    = new LineState();
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			Overall.Reset();
			Line   .Reset();
		}

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyLineBegin(DateTime ts, string dev, IODirection dir)
		{
			Overall.NotifyLineBegin(ts);
			Line   .NotifyLineBegin(ts, dev, dir);
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd()
		{
			Overall.NotifyLineEnd(Line.TimeStamp);
			Line   .NotifyLineEnd();
		}
	}

	/// <summary></summary>
	public class OverallState
	{
		/// <remarks>Dedicated sub-item to make scope obvious.</remarks>
		public DeviceState    DeviceLineBreak       { get; private set; }

		/// <remarks>Dedicated sub-item to make scope obvious.</remarks>
		/// <remarks>Only applies to <see cref="RepositoryType.Bidir"/>, still here for simplicity.</remarks>
		public DirectionState DirectionLineBreak    { get; private set; }

		/// <summary></summary>
		public IODirection    LastChunkDirection    { get; private set; }

		/// <summary></summary>
		public DateTime       LastChunkTimeStamp    { get; private set; }

		/// <summary></summary>
		public DateTime       LastTxChunkTimeStamp  { get; private set; }

		/// <summary></summary>
		public DateTime       LastRxChunkTimeStamp  { get; private set; }

		/// <summary></summary>
		public bool           IsFirstLine           { get; private set; }

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line" of the previous line.</remarks>
		public DateTime       PreviousLineTimeStamp { get; private set; }

		/// <remarks>Only applies to <see cref="RepositoryType.Bidir"/>, still here for simplicity.</remarks>
		protected List<RawChunk> PostponedTxChunks  { get; private set; }

		/// <remarks>Only applies to <see cref="RepositoryType.Bidir"/>, still here for simplicity.</remarks>
		protected List<RawChunk> PostponedRxChunks  { get; private set; }

		/// <summary></summary>
		public OverallState()
		{
			DeviceLineBreak    = new DeviceState();
			DirectionLineBreak = new DirectionState();

			InitializeValues();
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			LastChunkDirection    = IODirection.None;
			LastChunkTimeStamp    = DisplayElement.TimeStampDefault;
			LastTxChunkTimeStamp  = DisplayElement.TimeStampDefault;
			LastRxChunkTimeStamp  = DisplayElement.TimeStampDefault;
			IsFirstLine           = true;
			PreviousLineTimeStamp = DisplayElement.TimeStampDefault;

			PostponedTxChunks = new List<RawChunk>(); // No preset needed, the default behavior is good enough.
			PostponedRxChunks = new List<RawChunk>(); // No preset needed, the default behavior is good enough.
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			DeviceLineBreak   .Reset();
			DirectionLineBreak.Reset();

			InitializeValues();
		}

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyChunk(RawChunk chunk)
		{
			LastChunkDirection = chunk.Direction;
			LastChunkTimeStamp = chunk.TimeStamp;

			switch (chunk.Direction)
			{
				case IODirection.Tx: LastTxChunkTimeStamp = chunk.TimeStamp; break;
				case IODirection.Rx: LastRxChunkTimeStamp = chunk.TimeStamp; break;

				default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A chunk must always be tied to Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>For orthogonality with <see cref="GetLastChunkTimeStamp(IODirection)"/> below.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		public virtual DateTime GetLastChunkTimeStamp()
		{
			return (LastChunkTimeStamp);
		}

		/// <summary></summary>
		public virtual DateTime GetLastChunkTimeStamp(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx: return (LastTxChunkTimeStamp);
				case IODirection.Rx: return (LastRxChunkTimeStamp);

				default: throw (new ArgumentOutOfRangeException(MessageHelper.InvalidExecutionPreamble + "Direction must be either Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyLineBegin(DateTime lineTimeStamp)
		{
			if (IsFirstLine)                           // Set the initial processing time stamp.
				PreviousLineTimeStamp = lineTimeStamp; // Note this differs from the terminal's time span
		}                                              // base which corresponds to the active connect time base.

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd(DateTime lineTimeStamp)
		{
			IsFirstLine = false;
			PreviousLineTimeStamp = lineTimeStamp;
		}

		/// <summary></summary>
		public virtual void AddPostponedChunk(RawChunk chunk)
		{
			switch (chunk.Direction)
			{
				case IODirection.Tx: PostponedTxChunks.Add(chunk); break;
				case IODirection.Rx: PostponedRxChunks.Add(chunk); break;

				default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A chunk must always be tied to Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual int GetPostponedChunkCount()
		{
			return (PostponedTxChunks.Count + PostponedRxChunks.Count);
		}

		/// <summary></summary>
		public virtual int GetPostponedChunkCount(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx: return (PostponedTxChunks.Count);
				case IODirection.Rx: return (PostponedRxChunks.Count);

				default: throw (new ArgumentOutOfRangeException(MessageHelper.InvalidExecutionPreamble + "Direction must be either Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual int GetPostponedByteCount()
		{
			int byteCount = 0;

			foreach (var chunk in PostponedTxChunks)
				byteCount += chunk.Content.Count;

			foreach (var chunk in PostponedRxChunks)
				byteCount += chunk.Content.Count;

			return (byteCount);
		}

		/// <summary></summary>
		public virtual DateTime GetFirstPostponedChunkTimeStamp(IODirection dir)
		{
			List<RawChunk> chunks;

			switch (dir)
			{
				case IODirection.Tx: chunks = PostponedTxChunks; break;
				case IODirection.Rx: chunks = PostponedRxChunks; break;

				default: throw (new ArgumentOutOfRangeException(MessageHelper.InvalidExecutionPreamble + "Direction must be either Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (chunks.Count > 0)
				return (chunks[0].TimeStamp);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "This method requires existance of at least one postponed chunk!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual RawChunk[] RemovePostponedChunks(IODirection dir)
		{
			RawChunk[] chunks;

			switch (dir)
			{
				case IODirection.Tx: chunks = PostponedTxChunks.ToArray(); PostponedTxChunks.Clear(); break;
				case IODirection.Rx: chunks = PostponedRxChunks.ToArray(); PostponedRxChunks.Clear(); break;

				default: throw (new ArgumentOutOfRangeException(MessageHelper.InvalidExecutionPreamble + "Direction must be either Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			return (chunks);
		}
	}

	/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
	public class DeviceState
	{
		/// <summary></summary>
		public bool   IsFirstChunk { get; set; } // Not 'protected set' as commented at 'IsFirstChunk = false'.

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		public string Device       { get; set; } // Not 'protected set' as commented at 'Device = dev'.

		/// <summary></summary>
		public DeviceState()
		{
			InitializeValues();
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			IsFirstChunk = true;
			Device       = null;
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			InitializeValues();
		}
	}

	/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
	public class DirectionState
	{
		/// <summary></summary>
		public bool        IsFirstChunk { get; set; } // Not 'protected set' as commented at 'IsFirstChunk = false'.

		/// <summary></summary>
		public IODirection Direction    { get; set; } // Not 'protected set' as commented at 'Direction = dir'.

		/// <summary></summary>
		public DirectionState()
		{
			InitializeValues();
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			IsFirstChunk = true;
			Direction    = IODirection.None;
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			InitializeValues();
		}
	}

	/// <summary></summary>
	public class LineState
	{
		/// <summary></summary>
		public LinePosition             Position  { get; set; }

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line".</remarks>
		public DateTime                 TimeStamp { get; set; }

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		public string                   Device    { get; set; }

		/// <summary></summary>
		public IODirection              Direction { get; set; }

		/// <summary></summary>
		public DisplayElementCollection Elements  { get; }

		/// <summary></summary>
		public LineState()
		{
			InitializeValues();
			Elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
		}

		/// <summary>
		/// Indicates that line is yet empty.
		/// </summary>
		public virtual bool IsYetEmpty
		{
			get { return ((Elements.ByteCount == 0) && (Elements.CharCount == 0)); }
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			Position  = LinePosition.Begin;
			TimeStamp = DisplayElement.TimeStampDefault;
			Device    = null;
			Direction = IODirection.None;
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty line.
		/// </summary>
		public virtual void Reset()
		{
			InitializeValues();
			Elements.Clear(); // The collection will already have enlarged to the effectively typical line length.
		}                     // Thus, for performance reasons, clearing rather than recreating the collection.

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyLineBegin(DateTime ts, string dev, IODirection dir)
		{
			Position  = LinePosition.Content;
			TimeStamp = ts;
			Device    = dev;
			Direction = dir;
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd()
		{
			Reset(); // Important to reset the state, as the line state is e.g. used for evaluation line breaks.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
