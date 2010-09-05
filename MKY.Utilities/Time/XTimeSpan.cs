//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.Utilities.Time
{
	/// <summary>
	/// DateTime utility methods.
	/// </summary>
	public static class XTimeSpan
	{
		/// <summary>
		/// Returns time span formatted with "d:hh:mm:ss".
		/// </summary>
		public static string FormatTimeSpan(TimeSpan timeSpan)
		{
			return (FormatTimeSpan(timeSpan, false));
		}

		/// <summary>
		/// Returns time span formatted with "d:hh:mm:ss[.hh]".
		/// </summary>
		public static string FormatTimeSpan(TimeSpan timeSpan, bool hundredths)
		{
			StringBuilder sb = new StringBuilder();

			if (hundredths)
			{
				sb.Insert(0, (timeSpan.Milliseconds / 10).ToString("D2"));
				sb.Insert(0, ".");
			}

			sb.Insert(0, timeSpan.Seconds.ToString("D2"));
			sb.Insert(0, ":");
			sb.Insert(0, timeSpan.Minutes.ToString());
			if (timeSpan.TotalHours >= 1)
			{
				sb.Insert(0, ":");
				sb.Insert(0, timeSpan.Hours.ToString());

				if (timeSpan.TotalDays >= 1)
				{
					sb.Insert(0, "days ");
					sb.Insert(0, timeSpan.Days.ToString());
				}
			}

			return (sb.ToString());
		}
	}

	/// <summary></summary>
	public class TimeSpanEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly TimeSpan TimeSpan;

		/// <summary></summary>
		public TimeSpanEventArgs(TimeSpan timeSpan)
		{
			TimeSpan = timeSpan;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
