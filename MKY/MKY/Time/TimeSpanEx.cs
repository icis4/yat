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
// MKY Version 1.0.28 Development
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
		/// Converts the given number of ticks into the corresponding number of milliseconds,
		/// using <see cref="TimeSpan.TicksPerMillisecond"/>.
		/// </summary>
		/// <remarks>
		/// A similar method exists as <see cref="Diagnostics.StopwatchEx.TicksToTime"/>.
		/// </remarks>
		public static int TicksToTime(long ticks)
		{
			return ((int)(ticks / TimeSpan.TicksPerMillisecond));
		}

		/// <summary>
		/// Converts the given number of milliseconds into the corresponding number of ticks,
		/// using <see cref="TimeSpan.TicksPerMillisecond"/>.
		/// </summary>
		/// <remarks>
		/// A similar method exists as <see cref="Diagnostics.StopwatchEx.TimeToTicks"/>.
		/// </remarks>
		public static long TimeToTicks(int time)
		{
			return (time * TimeSpan.TicksPerMillisecond);
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[[[d days ]h]h:]m]m:]s]s".
		/// </summary>
		/// <remarks>
		/// Is intended to be eliminated after having upgraded to .NET 4+, using its additional
		/// <see cref="TimeSpan"/> formatting capabilities instead.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't like the additional formats...")]
		public static string FormatInvariantSeconds(TimeSpan value)
		{
			return (FormatInvariant(value, false, false, false, false));
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[[[d days ]h]h:]m]m:]s]s.fff".
		/// </summary>
		/// <remarks>
		/// Is intended to be eliminated after having upgraded to .NET 4+, using its additional
		/// <see cref="TimeSpan"/> formatting capabilities instead.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't like the additional formats...")]
		public static string FormatInvariantThousandths(TimeSpan value)
		{
			return (FormatInvariant(value, false, true, true, true));
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[d days ]h]h:]m]m:ss".
		/// </summary>
		/// <remarks>
		/// Is intended to be eliminated after having upgraded to .NET 4+, using its additional
		/// <see cref="TimeSpan"/> formatting capabilities instead.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't like the additional formats...")]
		public static string FormatInvariantSecondsEnforceMinutes(TimeSpan value)
		{
			return (FormatInvariant(value, true, false, false, false));
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[d days ]h]h:]m]m:ss.fff".
		/// </summary>
		/// <remarks>
		/// Is intended to be eliminated after having upgraded to .NET 4+, using its additional
		/// <see cref="TimeSpan"/> formatting capabilities instead.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't like the additional formats...")]
		public static string FormatInvariantThousandthsEnforceMinutes(TimeSpan value)
		{
			return (FormatInvariant(value, true, true, true, true));
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[[[d days ]h]h:]m]m:]s]s[.f[f[f]]]".
		/// </summary>
		/// <remarks>
		/// Is intended to be eliminated after having upgraded to .NET 4+, using its additional
		/// <see cref="TimeSpan"/> formatting capabilities instead.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		private static string FormatInvariant(TimeSpan value, bool enforceMinutes = true, bool addTenths = false, bool addHundredths = false, bool addThousandths = false)
		{
			var sb = new StringBuilder();

			if (addThousandths)
			{
				sb.Insert(0, (Math.Abs(value.Milliseconds) / 1).ToString("D3", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}
			else if (addHundredths)
			{
				sb.Insert(0, (Math.Abs(value.Milliseconds) / 10).ToString("D2", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}
			else if (addTenths)
			{
				sb.Insert(0, (Math.Abs(value.Milliseconds) / 100).ToString("D1", CultureInfo.InvariantCulture));
				sb.Insert(0, ".");
			}

			if ((Math.Abs(value.TotalSeconds) <= 0.0) && (!enforceMinutes)) // There shall at least be "0":
			{
				sb.Insert(0, "0");
			}
			else
			{
				var addMinutes = ((Math.Abs(value.TotalMinutes) >= 1.0) || enforceMinutes);
				if (addMinutes) // There shall at least be "0:00":
				{
					sb.Insert(0, Math.Abs(value.Seconds).ToString("D2", CultureInfo.InvariantCulture));
					sb.Insert(0, ":");

					var addHours = (Math.Abs(value.TotalHours) >= 1.0);
					if (addHours) // There shall at least be "0:00:00":
					{
						sb.Insert(0, Math.Abs(value.Minutes).ToString("D2", CultureInfo.InvariantCulture));
						sb.Insert(0, ":");

						if (Math.Abs(value.TotalDays) >= 1.0)
						{
							sb.Insert(0, Math.Abs(value.Hours).ToString(CultureInfo.InvariantCulture));

							if (Math.Abs(value.TotalDays) < 2.0)
								sb.Insert(0, " day ");
							else
								sb.Insert(0, " days ");

							sb.Insert(0, value.Days.ToString(CultureInfo.InvariantCulture));
						}
						else
						{
							sb.Insert(0, value.Hours.ToString(CultureInfo.InvariantCulture));
						}
					}
					else
					{
						if (enforceMinutes && (Math.Abs(value.Minutes) <= 0.0) && (value.TotalSeconds < 0.0))
						{
							sb.Insert(0, value.Minutes.ToString(CultureInfo.InvariantCulture));
							sb.Insert(0, "-");
						}
						else
						{
							sb.Insert(0, value.Minutes.ToString(CultureInfo.InvariantCulture));
						}
					}
				}
				else
				{
					sb.Insert(0, value.Seconds.ToString(CultureInfo.InvariantCulture));
				}
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[d days ]h]h:]m]m:ss.fff"
		/// supporting additional formats "^d.ddd^", "^h.hhh^", "^mm.mmm^", "^sss.sss^", "^ffffff^".
		/// </summary>
		/// <remarks>
		/// Is intended to be eliminated after having upgraded to .NET 4+, using its additional
		/// <see cref="TimeSpan"/> formatting capabilities instead.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't like the additional formats...")]
		public static string FormatInvariantThousandthsEnforceMinutes(TimeSpan value, string additionalFormat)
		{
			string result;
			if (TryFormatInvariantAdditional(value, additionalFormat, out result))
				return (result);

			return (FormatInvariant(value, true, true, true, true));
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[[[d days ]h]h:]m]m:]s]s.fff"
		/// supporting additional formats "^d.ddd^", "^h.hhh^", "^mm.mmm^", "^sss.sss^", "^ffffff^".
		/// </summary>
		/// <remarks>
		/// Is intended to be eliminated after having upgraded to .NET 4+, using its additional
		/// <see cref="TimeSpan"/> formatting capabilities instead.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't like the additional formats...")]
		public static string FormatInvariantThousandths(TimeSpan value, string additionalFormat)
		{
			string result;
			if (TryFormatInvariantAdditional(value, additionalFormat, out result))
				return (result);

			return (FormatInvariant(value, false, true, true, true));
		}

		/// <summary>
		/// Returns <paramref name="value"/> formatted as "[[[[d days ]h]h:]m]m:ss.fff"
		/// supporting additional formats "^d.ddd^", "^h.hhh^", "^mm.mmm^", "^sss.sss^", "^ffffff^".
		/// </summary>
		/// <remarks>
		/// \remind (2017-06-10 / MKY)
		/// Additional formats shall be extended after upgrading to .NET 4+ as follows:
		///  > More flexibility, e.g. "^ss.sss^" and "^ss.ss^" and...
		///  > Combination with standard formats, i.e. split ^^ and then format each fragment individually.
		///                                                        => Dynamically create the format string.
		/// </remarks>
		/// <remarks>
		/// Output milliseconds for readability, even though last digit only provides limited accuracy.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't like the additional formats...")]
		private static bool TryFormatInvariantAdditional(TimeSpan value, string additionalFormat, out string result)
		{
			if      (StringEx.EqualsOrdinal(additionalFormat, "^ffffff^"))
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0:000000}", value.TotalMilliseconds);
				return (true);
			}
			else if (StringEx.EqualsOrdinal(additionalFormat, "^sss.sss^"))
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0:000.000}", value.TotalSeconds);
				return (true);
			}
			else if (StringEx.EqualsOrdinal(additionalFormat, "^mm.mmm^"))
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0:00.000}", value.TotalMinutes);
				return (true);
			}
			else if (StringEx.EqualsOrdinal(additionalFormat, "^h.hhh^"))
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0:0.000}", value.TotalHours);
				return (true);
			}
			else if (StringEx.EqualsOrdinal(additionalFormat, "^d.ddd^"))
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0:0.000}", value.TotalDays);
				return (true);
			}

			result = null;
			return (false);
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
