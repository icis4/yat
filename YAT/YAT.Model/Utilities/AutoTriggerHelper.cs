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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
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
		public byte[] Sequence { get; }

	////private Domain.SequenceQueue txSequenceQueue is not needed (yet) as trigger by specification is only active on receive-path.
		private Domain.SequenceQueue bidirSequenceQueue;
		private Domain.SequenceQueue rxSequenceQueue;

		/// <summary></summary>
		public string TextOrRegexPattern { get; }

		/// <summary></summary>
		public bool TextOrRegexCaseSensitive { get; }

		/// <summary></summary>
		public bool TextOrRegexWholeWord { get; }

		/// <summary></summary>
		public Regex Regex { get; }

		/// <summary></summary>
		public AutoTriggerHelper(byte[] sequence)
		{
			lock (this)
			{
				Sequence = sequence;

				this.bidirSequenceQueue = new Domain.SequenceQueue(this.Sequence);
				this.rxSequenceQueue    = new Domain.SequenceQueue(this.Sequence);
			}
		}

		/// <summary></summary>
		public AutoTriggerHelper(string text, bool caseSensitive, bool wholeWord)
		{
			lock (this)
			{
				TextOrRegexPattern       = text;
				TextOrRegexCaseSensitive = caseSensitive;
				TextOrRegexWholeWord     = wholeWord;
			}
		}

		/// <summary></summary>
		public AutoTriggerHelper(string regexPattern, bool caseSensitive, bool wholeWord, Regex triggerRegex)
		{
			lock (this)
			{
				TextOrRegexPattern       = regexPattern;
				TextOrRegexCaseSensitive = caseSensitive;
				TextOrRegexWholeWord     = wholeWord;
				Regex                    = triggerRegex;
			}
		}

		private Domain.SequenceQueue GetSequenceQueue(Domain.RepositoryType repositoryType)
		{
			lock (this)
			{
				switch (repositoryType)
				{
					case Domain.RepositoryType.Tx:    throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MKY.MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));

				////case Domain.RepositoryType.Tx:    return (this.txSequenceQueue) is not needed (yet) as trigger by specification is only active on receive-path.
					case Domain.RepositoryType.Bidir: return (this.bidirSequenceQueue);
					case Domain.RepositoryType.Rx:    return (this.rxSequenceQueue);

					case Domain.RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MKY.MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
					default:                          throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MKY.MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!"               + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual void Reset(Domain.RepositoryType repositoryType)
		{
			lock (this)
			{
				var q = GetSequenceQueue(repositoryType);
				if (q != null)
					q.Reset();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		public virtual bool EnqueueAndMatch(Domain.RepositoryType repositoryType, byte b)
		{
			lock (this)
			{
				var q = GetSequenceQueue(repositoryType);
				if (q != null)
				{
					q.Enqueue(b);
					return (q.IsCompleteMatch);
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool TextTriggerSuccess(string input)
		{
			if (Regex == null) // Text only:
			{
				StringComparison comparisonType;
				if (TextOrRegexCaseSensitive)
					comparisonType = StringComparison.CurrentCulture;
				else
					comparisonType = StringComparison.CurrentCultureIgnoreCase;

				if (TextOrRegexWholeWord)
					return (StringEx.IndexOfWholeWord(input, TextOrRegexPattern, comparisonType) >= 0);
				else
					return (input.IndexOf(TextOrRegexPattern, comparisonType) >= 0); // Using string.IndexOf() because string.Contains()
			}                                                                               // does not allow controlling culture and case.
			else // Regex enabled:
			{
				if (!string.IsNullOrEmpty(input))
					return (Regex.Match(input).Success);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual int TextTriggerCount(string input)
		{
			MatchCollection matchesDummy;
			return (TextTriggerCount(input, out matchesDummy));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual int TextTriggerCount(string input, out MatchCollection matches)
		{
			if (Regex == null) // Text only:
			{
				matches = null;

				StringComparison comparisonType;
				if (TextOrRegexCaseSensitive)
					comparisonType = StringComparison.CurrentCulture;
				else
					comparisonType = StringComparison.CurrentCultureIgnoreCase;

				if (TextOrRegexWholeWord)
					return (StringEx.ContainingWholeWordCount(input, TextOrRegexPattern, comparisonType));
				else
					return (StringEx.ContainingCount(input, TextOrRegexPattern, comparisonType));
			}
			else // Regex enabled:
			{
				matches = Regex.Matches(input);

				return (matches.Count);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
