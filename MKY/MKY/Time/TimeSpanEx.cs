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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Times for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// DateTime utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class TimeSpanEx
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
				sb.Insert(0, (timeSpan.Milliseconds / 10).ToString("D2", NumberFormatInfo.InvariantInfo));
				sb.Insert(0, ".");
			}

			sb.Insert(0, timeSpan.Seconds.ToString("D2", NumberFormatInfo.InvariantInfo));
			sb.Insert(0, ":");
			if (timeSpan.TotalHours < 1)
			{
				sb.Insert(0, timeSpan.Minutes);
			}
			else
			{
				sb.Insert(0, timeSpan.Minutes.ToString("D2", NumberFormatInfo.InvariantInfo));
				sb.Insert(0, ":");
				if (timeSpan.TotalDays < 1)
				{
					sb.Insert(0, timeSpan.Hours);
				}
				else
				{
					sb.Insert(0, timeSpan.Hours.ToString("D2", NumberFormatInfo.InvariantInfo));
					sb.Insert(0, "days ");
					sb.Insert(0, timeSpan.Days);
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
