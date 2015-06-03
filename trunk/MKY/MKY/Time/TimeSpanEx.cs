//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'hh' and 'ss' just happen to be proper format strings...")]
		public static string FormatInvariantTimeSpan(TimeSpan timeSpan)
		{
			return (FormatInvariantTimeSpan(timeSpan, false, false, false));
		}

		/// <summary>
		/// Returns time span formatted with "d:hh:mm:ss[.f]".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'hh' and 'ss' just happen to be proper format strings...")]
		public static string FormatInvariantTimeSpan(TimeSpan timeSpan, bool tenths)
		{
			return (FormatInvariantTimeSpan(timeSpan, tenths, false, false));
		}

		/// <summary>
		/// Returns time span formatted with "d:hh:mm:ss[.f[f]]".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'hh' and 'ss' just happen to be proper format strings...")]
		public static string FormatInvariantTimeSpan(TimeSpan timeSpan, bool tenths, bool hundredths)
		{
			return (FormatInvariantTimeSpan(timeSpan, tenths, hundredths, false));
		}

		/// <summary>
		/// Returns time span formatted with "d:hh:mm:ss[.f[f[f]]]".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'hh' and 'ss' just happen to be proper format strings...")]
		public static string FormatInvariantTimeSpan(TimeSpan timeSpan, bool tenths, bool hundredths, bool thousandths)
		{
			StringBuilder sb = new StringBuilder();

			if (thousandths)
			{
				sb.Insert(0, (timeSpan.Milliseconds / 1).ToString("D3", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}
			else if (hundredths)
			{
				sb.Insert(0, (timeSpan.Milliseconds / 10).ToString("D2", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}
			else if (tenths)
			{
				sb.Insert(0, (timeSpan.Milliseconds / 100).ToString("D1", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}

			sb.Insert(0, timeSpan.Seconds.ToString("D2", CultureInfo.InvariantCulture));
			sb.Insert(0, ":");
			if (timeSpan.TotalHours < 1)
			{
				sb.Insert(0, timeSpan.Minutes);
			}
			else
			{
				sb.Insert(0, timeSpan.Minutes.ToString("D2", CultureInfo.InvariantCulture));
				sb.Insert(0, ":");
				if (timeSpan.TotalDays < 1)
				{
					sb.Insert(0, timeSpan.Hours);
				}
				else
				{
					sb.Insert(0, timeSpan.Hours.ToString("D2", CultureInfo.InvariantCulture));
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
		private TimeSpan timeSpan;

		/// <summary></summary>
		public TimeSpanEventArgs(TimeSpan timeSpan)
		{
			this.timeSpan = timeSpan;
		}

		/// <summary></summary>
		public TimeSpan TimeSpan
		{
			get { return (this.timeSpan); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
