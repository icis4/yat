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
// YAT Version 2.4.1
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// So far, only unidirectional state must be kept. Opposed to <see cref="ProcessState"/> which
	/// is kept three times (Tx/Bidir/Rx), the text terminal specific state is limited to a "line"
	/// state which is instantiated four times (Tx/TxBidir/RxBidir/Rx), "line" meaning "display line".
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unidir", Justification = "Orthogonality with 'Bidir'.")]
	public class TextUnidirState
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[]                            EolSequence                          { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiBytes'?")]
		public List<byte>                        PendingMultiBytesToDecode            { get; private set; }

		/// <remarks>Required to handle lines that only contain an EOL (=> show) or pending EOL (=> hide).</remarks>
		public int                               ShownCharCount                       { get; private set; }

		/// <remarks>Required to handle empty lines that got empty due to backspaces.</remarks>
		public bool                              HasEverShownChar                     { get; private set; }

		/// <summary></summary>
		public Dictionary<string, SequenceQueue> EolOfGivenDevice                     { get; private set; }

		/// <remarks>Must not be a <see cref="DisplayElementCollection"/> to prevent elements from being appended/merged.</remarks>
		public List<DisplayElement>              RetainedUnconfirmedHiddenEolElements { get; private set; }

		/// <summary></summary>
		public TextUnidirState(byte[] eolSequence)
		{
			EolSequence = eolSequence;

			InitializeValues();
		}

		/// <summary>
		/// Indicates that line is yet empty.
		/// </summary>
		public virtual bool IsYetEmpty
		{
			get { return ((PendingMultiBytesToDecode.Count == 0) && (ShownCharCount == 0) && (!HasEverShownChar) && (RetainedUnconfirmedHiddenEolElements.Count == 0)); }
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			PendingMultiBytesToDecode            = new List<byte>(4); // Preset the required capacity to improve memory management; 4 is the maximum value for multi-byte characters.
			ShownCharCount                       = 0;
			HasEverShownChar                     = false;
			EolOfGivenDevice                     = new Dictionary<string, SequenceQueue>(); // No preset needed, default behavior is good enough.
			RetainedUnconfirmedHiddenEolElements = new List<DisplayElement>(); // No preset needed, default behavior is good enough.
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			InitializeValues();
		}

		/// <summary>
		/// Notify added or removed <see cref="DisplayElement.CharCount"/> shown in the current line.
		/// </summary>
		public virtual void NotifyShownCharCount(int count)
		{
			ShownCharCount += count;

			if (count > 0)
				HasEverShownChar = true;
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd(string dev)
		{
			var eolOfGivenDeviceIsCompleteMatch = false;

			if (EolOfGivenDevice.ContainsKey(dev))
			{
				if (EolOfGivenDevice[dev].IsCompleteMatch) {
					EolOfGivenDevice[dev].Reset();

					eolOfGivenDeviceIsCompleteMatch = true;
				}

				// Keep EOL state when incomplete. Subsequent lines
				// need this to handle broken/pending EOL characters.
			}
			else                                                           // It is OK to only access or add to the collection,
			{                                                              // this will not lead to excessive use of memory,
				EolOfGivenDevice.Add(dev, new SequenceQueue(EolSequence)); // since there is only a given number of devices.
			}                                                              // Applies to TCP and UDP server terminals only.

			if (eolOfGivenDeviceIsCompleteMatch) // Otherwise keep unconfirmed hidden elements! They shall be delay-shown in case EOL is indeed unconfirmed!
			{
				RetainedUnconfirmedHiddenEolElements.Clear();
				ShownCharCount = 0; // Needed to hide empty lines that only contain a pending EOL, thus state must be kept across incomplete lines!
			}

			HasEverShownChar = false;
		}

		/// <summary></summary>
		public virtual bool EolIsAnyMatch(string dev)
		{
			if (EolOfGivenDevice.ContainsKey(dev))
				return (EolOfGivenDevice[dev].IsAnyMatch);
			else
				return (false);
		}

		/// <summary></summary>
		public virtual bool EolIsCompleteMatch(string dev)
		{
			if (EolOfGivenDevice.ContainsKey(dev))
				return (EolOfGivenDevice[dev].IsCompleteMatch);
			else
				return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
