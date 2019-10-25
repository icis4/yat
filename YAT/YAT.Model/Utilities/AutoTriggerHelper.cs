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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

using MKY;

namespace YAT.Model.Utilities
{
	/// <remarks>
	/// The automatic response feature is intentionally implemented using a byte sequence (and not
	/// e.g. a regular expression). Rationale:
	/// <list type="bullet">
	/// <item><description>I/O is a byte stream.</description></item>
	/// <item><description>Works for text as well as binary terminals.</description></item>
	/// <item><description>Regular expressions only make sense for text.</description></item>
	/// </list>
	/// </remarks>
	public class AutoTriggerHelper
	{
		/// <remarks>
		/// "Guidelines for Collections": "Do use byte arrays instead of collections of bytes."
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private byte[] triggerSequence;

		private Domain.SequenceQueue triggerQueue;

		/// <summary></summary>
		public AutoTriggerHelper()
		{
		}

		/// <summary></summary>
		public AutoTriggerHelper(byte[] triggerSequence)
		{
			lock (this)
			{
				this.triggerSequence = triggerSequence;
				this.triggerQueue = new Domain.SequenceQueue(this.triggerSequence);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TriggerSequence
		{
			get
			{
				return (this.triggerSequence);
			}
			set
			{
				lock (this)
				{
					if (!ArrayEx.ValuesEqual(this.triggerSequence, value))
					{
						this.triggerSequence = value;
						this.triggerQueue = new Domain.SequenceQueue(this.triggerSequence);
					}
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TriggerQueueAsArray()
		{
			lock (this)
			{
				return (this.triggerQueue.QueueAsArray());
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		public bool EnqueueAndMatchTrigger(byte b)
		{
			lock (this)
			{
				if (this.triggerQueue != null)
				{
					this.triggerQueue.Enqueue(b);
					return (this.triggerQueue.IsCompleteMatch);
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
