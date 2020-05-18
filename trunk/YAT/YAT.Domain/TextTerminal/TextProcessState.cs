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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// Text processing requires uni-directional as well as bi-directional state. This is the
	/// uni-directional state. Opposed to <see cref="ProcessState"/> which is kept three times
	/// (Tx/Bidir/Rx), the binary terminal specific state is limited to a line state which is
	/// instatiated four times (Tx/TxBidir/RxBidir/Rx).
	/// </remarks>
	public class TextUnidirState
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[]                            EolSequence                          { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiBytes'?")]
		public List<byte>                        PendingMultiBytesToDecode            { get; private set; }

		/// <summary></summary>
		public int                               ShownCharCount                       { get; private set; }

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

		/// <summary></summary>
		public TextUnidirState(TextUnidirState rhs)
		{
			EolSequence = rhs.EolSequence;

			InitializeValues();
		}

		/// <summary>
		/// Indicates that line is yet empty.
		/// </summary>
		public virtual bool IsYetEmpty
		{
			get { return ((PendingMultiBytesToDecode.Count == 0) && (RetainedUnconfirmedHiddenEolElements.Count == 0)); }
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			PendingMultiBytesToDecode            = new List<byte>(4); // Preset the required capacity to improve memory management; 4 is the maximum value for multi-byte characters.
			ShownCharCount                       = 0;
			EolOfGivenDevice                     = new Dictionary<string, SequenceQueue>(); // No preset needed, the default behavior is good enough.
			RetainedUnconfirmedHiddenEolElements = new List<DisplayElement>(); // No preset needed, the default behavior is good enough.
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
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd(string dev)
		{
			ShownCharCount = 0;

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
				RetainedUnconfirmedHiddenEolElements.Clear();
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

	/// <remarks>
	/// Text processing requires uni-directional as well as bi-directional state. This is the
	/// bi-directional state.
	/// </remarks>
	public class TextBidirState
	{
		/// <summary></summary>
		public List<RawChunk>                                   PostponedChunks     { get; private set; }

		/// <summary></summary>
		public Tuple<List<byte>, DateTime, string, IODirection> PostponedChunkBytes { get; set; }

		/// <summary></summary>
		public TextBidirState()
		{
			InitializeValues();
		}

		/// <summary></summary>
		public TextBidirState(TextBidirState rhs)
		{
			PostponedChunks = new List<RawChunk>(rhs.PostponedChunks);

			if (rhs.PostponedChunkBytes == null)
			{
				PostponedChunkBytes = null;
			}
			else
			{
				PostponedChunkBytes = new Tuple<List<byte>, DateTime, string, IODirection>
				                      (
				                          new List<byte>(rhs.PostponedChunkBytes.Item1),
				                                         rhs.PostponedChunkBytes.Item2,
				                                         rhs.PostponedChunkBytes.Item3,
				                                         rhs.PostponedChunkBytes.Item4
				                      );
			}
		}

		/// <summary>
		/// Initializes the state.
		/// </summary>
		protected virtual void InitializeValues()
		{
			PostponedChunks     = new List<RawChunk>(); // No preset needed, the default behavior is good enough.
			PostponedChunkBytes = null;
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
