//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.ObjectModel;

using MKY.Collections.ObjectModel;

namespace YAT.Model.Utilities
{
	/// <summary></summary>
	public class AutoResponseHelper
	{
		private ReadOnlyCollection<byte> sequence;
		private Domain.SequenceQueue queue;

		/// <summary></summary>
		public AutoResponseHelper()
		{
		}

		/// <summary></summary>
		public AutoResponseHelper(ReadOnlyCollection<byte> sequence)
		{
			lock (this)
			{
				this.sequence = sequence;
				this.queue = new Domain.SequenceQueue(this.sequence);
			}
		}

		/// <summary></summary>
		public ReadOnlyCollection<byte> Sequence
		{
			set
			{
				lock (this)
				{
					if (!ReadOnlyCollectionEx.ValuesEqual<byte>(this.sequence, value))
					{
						this.sequence = value;
						this.queue = new Domain.SequenceQueue(this.sequence);
					}
				}
			}
		}

		/// <summary></summary>
		public bool EnqueueAndMatch(byte b)
		{
			lock (this)
			{
				if (this.queue != null)
				{
					this.queue.Enqueue(b);
					return (this.queue.IsCompleteMatch);
				}
				else
				{
					return (false);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
