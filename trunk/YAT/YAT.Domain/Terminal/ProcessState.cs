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
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
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
		/// Resets the state, i.e. restarts processing with empty repository.
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
		/// <summary></summary>
		public bool        IsFirstLine           { get; protected set; }

		/// <summary></summary>
		public string      Device                { get; set; }

		/// <summary></summary>
		public IODirection Direction             { get; set; }

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line" of the previous line.</remarks>
		public DateTime    PreviousLineTimeStamp { get; protected set; }

		/// <summary></summary>
		public OverallState()
		{
			Reset();
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with empty repository.
		/// </summary>
		public virtual void Reset()
		{
			Initialize();
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with empty repository.
		/// </summary>
		private void Initialize()
		{
			IsFirstLine           = true;
			Device                = null;
			Direction             = IODirection.None;
			PreviousLineTimeStamp = DateTime.MinValue;
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

	/// <summary></summary>
	public class LineState
	{
		/// <summary></summary>
		public LinePosition             Position     { get; set; }

		/// <summary></summary>
		public bool                     IsFirstChunk { get; set; } // Cannot be 'protected set', see 'IsFirstChunk = false'.

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line".</remarks>
		public DateTime                 TimeStamp    { get; set; }

		/// <summary></summary>
		public string                   Device       { get; set; }

		/// <summary></summary>
		public IODirection              Direction    { get; set; }

		/// <summary></summary>
		public DisplayElementCollection Elements     { get; protected set; }

		/// <summary></summary>
		public LineState()
		{
			Reset();
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with empty repository.
		/// </summary>
		public virtual void Reset()
		{
			Initialize(LinePosition.Begin, DateTime.MinValue, null, IODirection.None);
		}

		private void Initialize(LinePosition pos, DateTime ts, string dev, IODirection dir)
		{
			Position     = pos;
			IsFirstChunk = true;
			TimeStamp    = ts;
			Device       = dev;
			Direction    = dir;
			Elements     = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
		}

		/// <summary>
		/// Notify the begin of a line, i.e. start processing of a line.
		/// </summary>
		public virtual void NotifyLineBegin(DateTime ts, string dev, IODirection dir)
		{
			Initialize(LinePosition.Content, ts, dev, dir);
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd()
		{
			Position = LinePosition.Begin;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
