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
// MKY Version 1.0.20
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Times for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// DateTime utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class TimeSpanEx
	{
		/// <summary>
		/// Returns the value formatted as "[[[[[[d days ]h]h:]m]m:]s]s[.f[f[f]]]".
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ss' just happens to be a proper format string...")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public static string FormatInvariantTimeSpan(TimeSpan value, bool enforceMinutes = true, bool addTenths = false, bool addHundredths = false, bool addThousandths = false)
		{
			var sb = new StringBuilder();

			if (addThousandths)
			{
				sb.Insert(0, (value.Milliseconds / 1).ToString("D3", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}
			else if (addHundredths)
			{
				sb.Insert(0, (value.Milliseconds / 10).ToString("D2", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}
			else if (addTenths)
			{
				sb.Insert(0, (value.Milliseconds / 100).ToString("D1", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}

			if ((value.TotalSeconds <= 0.0) && (!enforceMinutes)) // There shall at least be "0":
			{
				sb.Insert(0, "0");
			}
			else
			{
				if (enforceMinutes) // There shall at least be "0:00":
				{
					sb.Insert(0, value.Seconds.ToString("D2", CultureInfo.InvariantCulture));
				}
				else
				{
					sb.Insert(0, value.Seconds.ToString(CultureInfo.InvariantCulture));
				}

				if ((value.TotalMinutes >= 1.0) || enforceMinutes)
				{
					sb.Insert(0, ":");
					sb.Insert(0, value.Minutes.ToString(CultureInfo.InvariantCulture));

					if (value.TotalHours >= 1.0)
					{
						sb.Insert(0, ":");
						sb.Insert(0, value.Hours.ToString(CultureInfo.InvariantCulture));

						if (value.TotalDays >= 1.0)
						{
							sb.Insert(0, " days ");
							sb.Insert(0, value.Days.ToString(CultureInfo.InvariantCulture));
						}
					}
				}
			}

			return (sb.ToString());
		}
	}

	/// <summary></summary>
	public class TimeSpanEventArgs : EventArgs
	{
		/// <summary></summary>
		public TimeSpan TimeSpan { get; }

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
