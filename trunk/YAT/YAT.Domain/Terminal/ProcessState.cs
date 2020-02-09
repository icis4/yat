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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

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
			Reset();
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			Overall = new OverallState();
			Line    = new LineState();
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
		public DeviceState    DeviceLineBreak       { get; protected set; }

		/// <remarks>Dedicated sub-item to make scope obvious.</remarks>
		public DirectionState DirectionLineBreak    { get; protected set; }

		/// <summary></summary>
		public bool           IsFirstLine           { get; protected set; }

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line" of the previous line.</remarks>
		public DateTime       PreviousLineTimeStamp { get; protected set; }

		/// <summary></summary>
		public OverallState()
		{
			Reset();
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			DeviceLineBreak       = new DeviceState();
			DirectionLineBreak    = new DirectionState();
			IsFirstLine           = true;
			PreviousLineTimeStamp = DisplayElement.TimeStampDefault;
		}

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyLineBegin(DateTime lineTimeStamp)
		{
			if (IsFirstLine)
				PreviousLineTimeStamp = lineTimeStamp; // Set the initial overall time stamp.
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd(DateTime lineTimeStamp)
		{
			IsFirstLine = false;
			PreviousLineTimeStamp = lineTimeStamp;
		}
	}

	/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
	public class DeviceState
	{
		/// <summary></summary>
		public bool        IsFirstChunk          { get; set; } // Not 'protected set' as commented at 'IsFirstChunk = false'.

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		public string      Device                { get; set; }

		/// <summary></summary>
		public DeviceState()
		{
			IsFirstChunk = true;
			Device       = null;
		}
	}

	/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
	public class DirectionState
	{
		/// <summary></summary>
		public bool        IsFirstChunk          { get; set; } // Not 'protected set' as commented at 'IsFirstChunk = false'.

		/// <summary></summary>
		public IODirection Direction             { get; set; }

		/// <summary></summary>
		public DirectionState()
		{
			IsFirstChunk = true;
			Direction    = IODirection.None;
		}
	}

	/// <summary></summary>
	public class LineState
	{
		/// <summary></summary>
		public LinePosition             Position     { get; set; }

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line".</remarks>
		public DateTime                 TimeStamp    { get; set; }

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		public string                   Device       { get; set; }

		/// <summary></summary>
		public IODirection              Direction    { get; set; }

		/// <summary></summary>
		public DisplayElementCollection Elements     { get; }

		/// <summary></summary>
		public LineState()
		{
			Initialize();
			Elements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void Initialize()
		{
			Position  = LinePosition.Begin;
			TimeStamp = DisplayElement.TimeStampDefault;
			Device    = null;
			Direction = IODirection.None;
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			Initialize();
			Elements.Clear();
		}

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
