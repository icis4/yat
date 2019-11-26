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

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public class BinaryLineState
	{
		/// <summary></summary>
		public SequenceQueue            SequenceAfter                                   { get; set; }

		/// <summary></summary>
		public SequenceQueue            SequenceBefore                                  { get; set; }

		/// <summary></summary>
		public DisplayElementCollection RetainedUnconfirmedHiddenSequenceBeforeElements { get; set; }

		/// <summary></summary>
		public BinaryLineState(SequenceQueue sequenceAfter, SequenceQueue sequenceBefore)
		{
			SequenceAfter  = sequenceAfter;
			SequenceBefore = sequenceBefore;

			Initialize();
		}

		/// <summary></summary>
		public BinaryLineState(BinaryLineState rhs)
		{
			SequenceAfter  = rhs.SequenceAfter;
			SequenceBefore = rhs.SequenceBefore;

			Initialize();
		}

		/// <summary></summary>
		protected virtual void Initialize()
		{
			RetainedUnconfirmedHiddenSequenceBeforeElements = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			SequenceAfter .Reset();
			SequenceBefore.Reset();

			Initialize();
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd()
		{
			// Nothing to do, all items must be preserved across lines!
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
