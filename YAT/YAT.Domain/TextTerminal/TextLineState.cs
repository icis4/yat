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

using System.Collections.Generic;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public class TextLineState
	{
		/// <summary></summary>
		public byte[]                            EolSequence                                { get; }

		/// <summary></summary>
		public List<byte>                        PendingMultiBytesToDecode                  { get; private set; }

		/// <summary></summary>
		public Dictionary<string, SequenceQueue> EolOfGivenDevice                           { get; private set; }

		/// <summary></summary>
		public Dictionary<string, bool>          EolOfLastLineOfGivenDeviceWasCompleteMatch { get; private set; }

		/// <summary></summary>
		public DisplayElementCollection          RetainedUnconfirmedHiddenEolElements       { get; private set; }

		/// <summary></summary>
		public TextLineState(byte[] eolSequence)
		{
			EolSequence = eolSequence;

			Reset();
		}

		/// <summary></summary>
		public TextLineState(TextLineState rhs)
		{
			EolSequence = rhs.EolSequence;

			Reset();
		}

		/// <summary>
		/// Resets the state, i.e. restarts processing with an empty repository.
		/// </summary>
		public virtual void Reset()
		{
			PendingMultiBytesToDecode                  = new List<byte>(4); // Preset the required capacity to improve memory management; 4 is the maximum value for multi-byte characters.

			EolOfGivenDevice                           = new Dictionary<string, SequenceQueue>(); // No preset needed, the default initial capacity is good enough.
			EolOfLastLineOfGivenDeviceWasCompleteMatch = new Dictionary<string, bool>();          // No preset needed, the default initial capacity is good enough.

			RetainedUnconfirmedHiddenEolElements       = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
		}

		/// <summary>
		/// Notify the end of a line, i.e. continues processing with the next line.
		/// </summary>
		public virtual void NotifyLineEnd(string formerDevice, bool eolWasCompleteMatch)
		{
			if (EolOfGivenDevice.ContainsKey(formerDevice))
			{
				if (EolOfGivenDevice[formerDevice].IsCompleteMatch)
					EolOfGivenDevice[formerDevice].Reset();

				// Keep EOL state when incomplete. Subsequent lines
				// need this to handle broken/pending EOL characters.
			}
			else                                                                    // It is OK to only access or add to the collection,
			{                                                                       // this will not lead to excessive use of memory,
				EolOfGivenDevice.Add(formerDevice, new SequenceQueue(EolSequence)); // since there is only a given number of devices.
			}                                                                       // Applies to TCP and UDP terminals only.

			if (eolWasCompleteMatch) // Keep unconfirmed hidden elements! They shall be delay-shown in case EOL is indeed unconfirmed!
				RetainedUnconfirmedHiddenEolElements = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.

			if (EolOfLastLineOfGivenDeviceWasCompleteMatch.ContainsKey(formerDevice))
				EolOfLastLineOfGivenDeviceWasCompleteMatch[formerDevice] = eolWasCompleteMatch;
			else
				EolOfLastLineOfGivenDeviceWasCompleteMatch.Add(formerDevice, eolWasCompleteMatch); // Same as above, it is OK to only access or add to the collection.
		}

		/// <summary></summary>
		public virtual bool EolOfLastLineWasCompleteMatch(string dev)
		{
			if (EolOfLastLineOfGivenDeviceWasCompleteMatch.ContainsKey(dev))
				return (EolOfLastLineOfGivenDeviceWasCompleteMatch[dev]);
			else
				return (true); // Cleared monitors mean that last line was complete!
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
