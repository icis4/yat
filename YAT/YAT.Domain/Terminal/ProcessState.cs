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
// Copyright © 2007-2021 Matthias Kläy.
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

	#if (WITH_SCRIPTING)
		/// <summary></summary>
		public ScriptState  Script  { get; set; }
	#endif

		/// <summary></summary>
		public ProcessState()
		{
			Overall = new OverallState();
			Line    = new LineState();
		#if (WITH_SCRIPTING)
			Script  = new ScriptState();
		#endif
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			Overall.Reset();
			Line   .Reset();
		#if (WITH_SCRIPTING)
			Script .Reset();
		#endif
		}

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyLineBegin(DateTime ts, string dev, IODirection dir)
		{
			Overall.NotifyLineBegin(ts);
			Line   .NotifyLineBegin(ts, dev, dir);
		#if (WITH_SCRIPTING)
			Script .NotifyLineBegin(ts, dev);
		#endif
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
	#if (WITH_SCRIPTING)
		public virtual void NotifyLineEnd(bool appliesToScriptLine)
	#else
		public virtual void NotifyLineEnd()
	#endif
		{
			Overall.NotifyLineEnd(Line.TimeStamp); ////, Line.Direction); is prepared for future use.
			Line   .NotifyLineEnd();
		#if (WITH_SCRIPTING)
			Script .NotifyLineEnd(appliesToScriptLine);
		#endif
		}
	}

	/// <summary></summary>
	public class OverallState
	{
		/// <remarks>Dedicated sub-item to make scope obvious.</remarks>
		public DeviceState       DeviceLineBreak          { get; private set; }

		/// <remarks>Dedicated sub-item to make scope obvious.</remarks>
		/// <remarks>Only applies to <see cref="RepositoryType.Bidir"/>, still here for simplicity.</remarks>
		public DirectionState    DirectionLineBreak       { get; private set; }

	/////// <summary></summary> is prepared for future use.
	////public IODirection       PreviousChunkDirection   { get; private set; } // Chunks: 1. Direction 2. TimeStamp

		/// <remarks><see cref="GetPreviousChunkTimeStamp()"/> shall be used to retrieve property.</remarks>
		private DateTime         PreviousChunkTimeStamp   { get; set; }

		/// <summary></summary>
		public DateTime          PreviousTxChunkTimeStamp { get; private set; }

		/// <summary></summary>
		public DateTime          PreviousRxChunkTimeStamp { get; private set; }

		/// <summary></summary>
		public bool              IsFirstLine              { get; private set; }

		/// <remarks>'TimeStamp' implicitly means 'OfBeginningOfLine' of the previous line.</remarks>
		public DateTime          PreviousLineTimeStamp    { get; private set; }

	/////// <summary></summary> is prepared for future use.
	////public IODirection       PreviousLineDirection    { get; private set; } // Lines: 1. TimeStamp 2. Direction

		/// <remarks>Only applies to <see cref="RepositoryType.Bidir"/>, still here for simplicity.</remarks>
		protected List<RawChunk> PostponedTxChunks        { get; private set; }

		/// <remarks>Only applies to <see cref="RepositoryType.Bidir"/>, still here for simplicity.</remarks>
		protected List<RawChunk> PostponedRxChunks        { get; private set; }

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
		////PreviousChunkDirection   = IODirection.None; is prepared for future use.
			PreviousChunkTimeStamp   = DisplayElement.TimeStampDefault;
			PreviousTxChunkTimeStamp = DisplayElement.TimeStampDefault;
			PreviousRxChunkTimeStamp = DisplayElement.TimeStampDefault;

			IsFirstLine              = true;
			PreviousLineTimeStamp    = DisplayElement.TimeStampDefault;
		////PreviousLineDirection    = IODirection.None; is prepared for future use.

			PostponedTxChunks        = new List<RawChunk>(); // No preset needed, the default behavior is good enough.
			PostponedRxChunks        = new List<RawChunk>(); // No preset needed, the default behavior is good enough.
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
		////PreviousChunkDirection = chunk.Direction; is prepared for future use.
			PreviousChunkTimeStamp = chunk.TimeStamp;

			switch (chunk.Direction)
			{
				case IODirection.Tx: PreviousTxChunkTimeStamp = chunk.TimeStamp; break;
				case IODirection.Rx: PreviousRxChunkTimeStamp = chunk.TimeStamp; break;

				default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A chunk must always be tied to Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>Method instead of property for orthogonality with <see cref="GetPreviousChunkTimeStamp(IODirection)"/> below.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "See remarks.")]
		public virtual DateTime GetPreviousChunkTimeStamp()
		{
			return (PreviousChunkTimeStamp);
		}

		/// <summary></summary>
		public virtual DateTime GetPreviousChunkTimeStamp(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx: return (PreviousTxChunkTimeStamp);
				case IODirection.Rx: return (PreviousRxChunkTimeStamp);

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
		public virtual void NotifyLineEnd(DateTime lineTimeStamp) ////, IODirection lineDirection) is prepared for future use.
		{
			IsFirstLine = false;
			PreviousLineTimeStamp = lineTimeStamp;
		////PreviousLineDirection = lineDirection; is prepared for future use.
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

		/// <remarks>Method instead of property for orthogonality with <see cref="GetPostponedChunkCount(IODirection)"/> below.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
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

		/// <remarks>Method instead of property for orthogonality with <see cref="GetPostponedChunkCount()"/> above.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		public virtual int GetPostponedByteCount()
		{
			int byteCount = 0;

			foreach (var chunk in PostponedTxChunks)
				byteCount += chunk.Content.Count;

			foreach (var chunk in PostponedRxChunks)
				byteCount += chunk.Content.Count;

			return (byteCount);
		}

		/// <remarks>Method instead of property for orthogonality with other "Postponed" methods above.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		public virtual RawChunk GetFirstPostponedChunk()
		{
			var firstPostponedTxChunkTimeStamp = DateTime.MaxValue;
			var firstPostponedRxChunkTimeStamp = DateTime.MaxValue;

			if (PostponedTxChunks.Count > 0)
				firstPostponedTxChunkTimeStamp = PostponedTxChunks[0].TimeStamp;

			if (PostponedRxChunks.Count > 0)
				firstPostponedRxChunkTimeStamp = PostponedRxChunks[0].TimeStamp;

			if (firstPostponedTxChunkTimeStamp < firstPostponedRxChunkTimeStamp)
				return (PostponedTxChunks[0]);

			if (firstPostponedRxChunkTimeStamp < firstPostponedTxChunkTimeStamp)
				return (PostponedRxChunks[0]);

			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "This method requires existance of at least one postponed chunk!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

	/// <remarks>Named 'Device' for simplicity even though using "I/O Device" for view.</remarks>
	public class DeviceState
	{
		/// <summary></summary>
		public bool   IsFirstChunk { get; set; } // Not 'protected set' as commented at 'IsFirstChunk = false'.

		/// <remarks>Named 'Device' for simplicity even though using "I/O Device" for view.</remarks>
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

	/// <remarks>Named 'Device' for simplicity even though using "I/O Device" for view.</remarks>
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

		/// <remarks>'TimeStamp' implicitly means 'OfBeginningOfLine'.</remarks>
		public DateTime                 TimeStamp { get; set; }

		/// <remarks>Named 'Device' for simplicity even though using "I/O Device" for view.</remarks>
		public string                   Device    { get; set; }

		/// <summary></summary>
		public IODirection              Direction { get; set; }

		/// <summary></summary>
		public DisplayElementCollection Elements  { get; }

		/// <summary></summary>
		public bool                     Exceeded  { get; set; }

		/// <summary></summary>
		public LineState()
		{
			InitializeValues();
			Elements = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
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
			Exceeded  = false;
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

#if (WITH_SCRIPTING)

	/// <summary></summary>
	public class ScriptState
	{
		/// <remarks>'TimeStamp' implicitly means 'OfBeginningOfLine'.</remarks>
		public DateTime   TimeStamp { get; set; }

		/// <remarks>Named 'Device' for simplicity even though using "I/O Device" for view.</remarks>
		public string     Device    { get; set; }

		/// <summary></summary>
		public List<byte> Data      { get; }

		/// <summary></summary>
		public ScriptState()
		{
			InitializeValues();
			Data = new List<byte>(); // No preset needed, the default behavior is good enough.
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			TimeStamp = DisplayElement.TimeStampDefault;
			Device    = null;
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty line.
		/// </summary>
		public virtual void Reset()
		{
			InitializeValues();
			Data.Clear(); // The collection will already have enlarged to the effectively typical line length.
		}                 // Thus, for performance reasons, clearing rather than recreating the collection.

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyLineBegin(DateTime ts, string dev)
		{
			if (Data.Count > 0)
				return; // Skip begin if script line has not ended yet.

			TimeStamp = ts;
			Device    = dev;
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd(bool appliesToScriptLines)
		{
			if (appliesToScriptLines)
				Reset(); // Important to reset the state, as the line state is e.g. used for evaluation whether a new line shall be started.
		}
	}

#endif // (WITH_SCRIPTING)
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
