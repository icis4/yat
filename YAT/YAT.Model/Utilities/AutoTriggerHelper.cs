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
using System.Text;
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
		public byte[] ByteSequence { get; }

	////private Domain.SequenceQueue txByteSequenceQueue is not needed (yet) as trigger by specification is only active on receive-path.
		private Domain.SequenceQueue bidirByteSequenceQueue;
		private Domain.SequenceQueue rxByteSequenceQueue;

		/// <summary></summary>
		public string TextOrRegexPattern { get; }

		/// <summary></summary>
		public bool TextOrRegexCaseSensitive { get; }

		/// <summary></summary>
		public bool TextOrRegexWholeWord { get; }

		/// <summary></summary>
		public Regex Regex { get; }

		/// <summary></summary>
		public AutoTriggerHelper(byte[] byteSequence)
		{
			lock (this)
			{
				ByteSequence = byteSequence;

				this.bidirByteSequenceQueue = new Domain.SequenceQueue(ByteSequence);
				this.rxByteSequenceQueue    = new Domain.SequenceQueue(ByteSequence);
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

		#region ByteSequence
		//==========================================================================================
		// ByteSequence
		//==========================================================================================

		private Domain.SequenceQueue GetByteSequenceQueue(Domain.RepositoryType repositoryType)
		{
			lock (this)
			{
				switch (repositoryType)
				{
					case Domain.RepositoryType.Tx:    throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MKY.MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));

				////case Domain.RepositoryType.Tx:    return (this.txByteSequenceQueue) is not needed (yet) as trigger by specification is only active on receive-path.
					case Domain.RepositoryType.Bidir: return (this.bidirByteSequenceQueue);
					case Domain.RepositoryType.Rx:    return (this.rxByteSequenceQueue);

					case Domain.RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MKY.MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
					default:                          throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MKY.MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!"               + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual void ResetByteSequence(Domain.RepositoryType repositoryType)
		{
			lock (this)
			{
				var q = GetByteSequenceQueue(repositoryType);
				if (q != null)
					q.Reset();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		public virtual bool EnqueueAndMatchByteSequence(Domain.RepositoryType repositoryType, byte b)
		{
			lock (this)
			{
				var q = GetByteSequenceQueue(repositoryType);
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
		public virtual string ByteSequenceToString()
		{
			var str = Domain.Utilities.ByteHelper.FormatHexString(ByteSequence, Domain.Settings.DisplaySettings.ShowRadixDefault);

			if (!string.IsNullOrEmpty(str))
				return (str);
			else
				return ("(empty)");
		}

		#endregion

		#region Text/Regex
		//==========================================================================================
		// Text/Regex
		//==========================================================================================

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

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			if (!ArrayEx.IsNullOrEmpty(ByteSequence))
				return (ByteSequenceToString());
			else
				return (TextOrRegexPattern);
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		/// <remarks>
		/// Limited to a single line to keep debug output compact, same as <see cref="ToString()"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder(indent);

			if (!ArrayEx.IsNullOrEmpty(ByteSequence))
			{
				sb.Append("Sequence = ");
				sb.Append(ByteSequenceToString());
				sb.Append(" | Queue (Bidir) = ");
				sb.Append(this.bidirByteSequenceQueue.QueueToString());
				sb.Append(" | Queue (Rx) = ");
				sb.Append(this.rxByteSequenceQueue.QueueToString());
			}
			else
			{
				sb.Append("TextOrRegexPattern = ");
				sb.Append(TextOrRegexPattern);
				sb.Append(" | CaseSensitive = ");
				sb.Append(TextOrRegexCaseSensitive.ToString());
				sb.Append(" | WholeWord = ");
				sb.Append(TextOrRegexWholeWord.ToString());
			}

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
