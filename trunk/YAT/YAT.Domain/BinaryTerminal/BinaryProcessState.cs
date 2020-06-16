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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// So far, only unidirectional state must be kept. Thus, opposed to <see cref="ProcessState"/>
	/// which is kept three times (Tx/Bidir/Rx), the binary terminal specific state is limited to a
	/// line state which is instantiated four times (Tx/TxBidir/RxBidir/Rx).
	/// </remarks>
	/// <remarks>
	/// Ideas for potential future refinement of this class:
	/// <list type="bullte">
	/// <item><description>Migrate this class to a "BinaryProcessState". Difficulty: Bidir requires an aggregated state.</description></item>
	/// <item><description>Split into "BinaryUnidirState" and "BinaryBidirState" classes.</description></item>
	/// <item><description>Add a "BinaryOverallState" in addition to this class.</description></item>
	/// <item><description>...</description></item>
	/// </list>
	/// State shall be renamed or migrated or whatever when meeting the need to do so.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unidir", Justification = "Orthogonality with 'Bidir'.")]
	public class BinaryUnidirState
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[]                            SequenceAfter                                   { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[]                            SequenceBefore                                  { get; }

		/// <remarks>
		/// To be preserved across lines, because sequence break also applies on active chunk or timed line break.
		/// </remarks>
		public Dictionary<string, SequenceQueue> SequenceAfterOfGivenDevice                      { get; private set; }

		/// <remarks>
		/// To be preserved across lines, because sequence break also applies on active chunk or timed line break.
		/// </remarks>
		public Dictionary<string, SequenceQueue> SequenceBeforeOfGivenDevice                     { get; private set; }

		/// <remarks>Must not be a <see cref="DisplayElementCollection"/> to prevent elements from being appended/merged.</remarks>
		public List<DisplayElement>              RetainedUnconfirmedHiddenSequenceBeforeElements { get; private set; }

		/// <summary></summary>
		public BinaryUnidirState(byte[] sequenceAfter, byte[] sequenceBefore)
		{
			SequenceAfter  = sequenceAfter;
			SequenceBefore = sequenceBefore;

			InitializeValues();
		}

		/// <summary></summary>
		public BinaryUnidirState(BinaryUnidirState rhs)
		{
			SequenceAfter  = rhs.SequenceAfter;
			SequenceBefore = rhs.SequenceBefore;

			InitializeValues();
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			SequenceAfterOfGivenDevice  = new Dictionary<string, SequenceQueue>(); // No preset needed, the default behavior is good enough.
			SequenceBeforeOfGivenDevice = new Dictionary<string, SequenceQueue>(); // No preset needed, the default behavior is good enough.

			RetainedUnconfirmedHiddenSequenceBeforeElements = new List<DisplayElement>(); // No preset needed, the default behavior is good enough.
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			InitializeValues();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
