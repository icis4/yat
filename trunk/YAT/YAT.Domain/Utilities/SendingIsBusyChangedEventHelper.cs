﻿//==================================================================================================
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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace YAT.Domain.Utilities
{
	/// <summary>
	/// While sending, the 'SendingIsBusyChanged' event must be raised if intensive processing is
	/// done. This is required because a client may want to indicate that time intensive sending is
	/// currently ongoing and no further data shall be sent.
	/// The event shall be raised if the time lag will significantly be noticeable by the user
	/// (i.e. >= 400 ms). But the event shall be raised BEFORE the actual time lag. This helper
	/// struct manages the state and the various criteria.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
	public struct SendingIsBusyChangedEventHelper : IEquatable<SendingIsBusyChangedEventHelper>
	{
		/// <remarks>In milliseconds.</remarks>
		public const int Threshold = 400;

		private bool eventMustBeRaised;
		private DateTime initialTimeStamp;

		/// <summary></summary>
		public SendingIsBusyChangedEventHelper(DateTime initialTimeStamp)
		{
			this.eventMustBeRaised = false;
			this.initialTimeStamp = initialTimeStamp;
		}

		/// <summary></summary>
		public bool EventMustBeRaised
		{
			get { return (this.eventMustBeRaised); }
		}

		/// <remarks>Using term "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.</remarks>
		public static bool ChunkSizeIsAboveThreshold(int chunkSize, double bytesPerMillisecond)
		{
			return (chunkSize >= (Threshold * bytesPerMillisecond));
		}

		/// <remarks>Using term "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Funny, this *is* an event helper.")]
		public bool RaiseEventIfChunkSizeIsAboveThreshold(int chunkSize, double bytesPerMillisecond)
		{
			// Only let the event get raised if it hasn't been yet:
			if (!this.eventMustBeRaised && ChunkSizeIsAboveThreshold(chunkSize, bytesPerMillisecond))
			{
				this.eventMustBeRaised = true;
				return (true);
			}

			return (false);
		}

		/// <summary></summary>
		public static bool DelayIsAboveThreshold(int delay)
		{
			return (delay >= Threshold);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Funny, this *is* an event helper.")]
		public bool RaiseEventIfDelayIsAboveThreshold(int delay)
		{
			// Only let the event get raised if it hasn't been yet:
			if (!this.eventMustBeRaised && DelayIsAboveThreshold(delay))
			{
				this.eventMustBeRaised = true;
				return (true);
			}

			return (false);
		}

		/// <summary></summary>
		public bool TotalTimeLagIsAboveThreshold()
		{
			TimeSpan totalTimeLag = (DateTime.Now - this.initialTimeStamp);
			return (totalTimeLag.Milliseconds >= Threshold);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Funny, this *is* an event helper.")]
		public bool RaiseEventIfTotalTimeLagIsAboveThreshold()
		{
			// Only let the event get raised if it hasn't been yet:
			if (!this.eventMustBeRaised && TotalTimeLagIsAboveThreshold())
			{
				this.eventMustBeRaised = true;
				return (true);
			}

			return (false);
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

				hashCode =                    this.eventMustBeRaised.GetHashCode();
				hashCode = (hashCode * 397) ^ this.initialTimeStamp .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SendingIsBusyChangedEventHelper)
				return (Equals((SendingIsBusyChangedEventHelper)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SendingIsBusyChangedEventHelper other)
		{
			return
			(
				this.eventMustBeRaised.Equals(other.eventMustBeRaised) &&
				this.initialTimeStamp .Equals(other.initialTimeStamp)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(SendingIsBusyChangedEventHelper lhs, SendingIsBusyChangedEventHelper rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(SendingIsBusyChangedEventHelper lhs, SendingIsBusyChangedEventHelper rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
