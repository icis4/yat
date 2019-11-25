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
using System.Text.RegularExpressions;

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
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TriggerSequence { get; }

		private Domain.SequenceQueue triggerSequenceQueue;

		/// <summary></summary>
		public string TriggerText { get; }

		/// <summary></summary>
		public string TriggerRegexPattern { get; }

		/// <summary></summary>
		public Regex TriggerRegex { get; }

		/// <summary></summary>
		public AutoTriggerHelper(byte[] triggerSequence)
		{
			lock (this)
			{
				TriggerSequence = triggerSequence;

				this.triggerSequenceQueue = new Domain.SequenceQueue(TriggerSequence);
			}
		}

		/// <summary></summary>
		public AutoTriggerHelper(string triggerText)
		{
			lock (this)
			{
				TriggerText = triggerText;
			}
		}

		/// <summary></summary>
		public AutoTriggerHelper(string triggerRegexPattern, Regex triggerRegex)
		{
			lock (this)
			{
				TriggerRegexPattern = triggerRegexPattern;
				TriggerRegex = triggerRegex;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TriggerSequenceQueueAsArray()
		{
			lock (this)
			{
				return (this.triggerSequenceQueue.QueueAsArray());
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		public virtual bool EnqueueAndMatchTrigger(byte b)
		{
			lock (this)
			{
				if (this.triggerSequenceQueue != null)
				{
					this.triggerSequenceQueue.Enqueue(b);
					return (this.triggerSequenceQueue.IsCompleteMatch);
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			lock (this)
			{
				if (this.triggerSequenceQueue != null)
					this.triggerSequenceQueue.Reset();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
