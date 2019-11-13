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
	public class LineState
	{
		// Overall:

		/// <summary></summary>
		public bool     IsFirstLine           { get; set; }

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line" of the previous line.</remarks>
		public DateTime PreviousLineTimeStamp { get; set; }

		// Line itself:

		/// <summary></summary>
		public LinePosition             Position       { get; set; }

		/// <summary></summary>
		public bool                     IsFirstChunk   { get; set; }

		/// <remarks>"Time Stamp" implicitly means "of Beginning of Line".</remarks>
		public DateTime                 TimeStamp      { get; set; }

		/// <summary></summary>
		public string                   Device         { get; set; }

		/// <summary></summary>
		public IODirection              Direction      { get; set; }

		/// <summary></summary>
		public DisplayElementCollection Elements       { get; set; }

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
			// Overall:
			IsFirstLine           = true;
			PreviousLineTimeStamp = DateTime.MinValue;

			// Line itself:
			ResetLine();
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd(DateTime lineTimeStamp)
		{
			// Overall:
			IsFirstLine           = false;
			PreviousLineTimeStamp = lineTimeStamp;

			// Line itself:
			ResetLine();
		}

		private void ResetLine()
		{
			Position     = LinePosition.Begin;
			IsFirstChunk = true;
			TimeStamp    = DateTime.Now;
			Device       = null;
			Direction    = IODirection.None;
			Elements     = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
