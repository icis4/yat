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
// NUnit Version 1.0.22 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Globalization;

namespace NUnitEx
{
	/// <summary>
	/// A selection of standard duration categories, kind-of-logarithmically distributed.
	/// </summary>
	public static class StandardDurationCategory
	{
		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Second1Attribute : SecondDurationCategoryAttribute
		{
			/// <summary></summary>
			public Second1Attribute()
				: base(1)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Seconds4Attribute : SecondDurationCategoryAttribute
		{
			/// <summary></summary>
			public Seconds4Attribute()
				: base(4)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Seconds15Attribute : SecondDurationCategoryAttribute
		{
			/// <summary></summary>
			public Seconds15Attribute()
				: base(15)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Minute1Attribute : MinuteDurationCategoryAttribute
		{
			/// <summary></summary>
			public Minute1Attribute()
				: base(1)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Minutes4Attribute : MinuteDurationCategoryAttribute
		{
			/// <summary></summary>
			public Minutes4Attribute()
				: base(4)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Minutes15Attribute : MinuteDurationCategoryAttribute
		{
			/// <summary></summary>
			public Minutes15Attribute()
				: base(15)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Hour1Attribute : HourDurationCategoryAttribute
		{
			/// <summary></summary>
			public Hour1Attribute()
				: base(1)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Hours5Attribute : HourDurationCategoryAttribute
		{
			/// <summary></summary>
			public Hours5Attribute()
				: base(5)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Hours24Attribute : HourDurationCategoryAttribute
		{
			/// <summary></summary>
			public Hours24Attribute()
				: base(24)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Day1Attribute : DayDurationCategoryAttribute
		{
			/// <summary></summary>
			public Day1Attribute()
				: base(1)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Days7Attribute : DayDurationCategoryAttribute
		{
			/// <summary></summary>
			public Days7Attribute()
				: base(7)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class Days30Attribute : DayDurationCategoryAttribute
		{
			/// <summary></summary>
			public Days30Attribute()
				: base(30)
			{
			}
		}

		/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
		[CLSCompliant(false)]
		public sealed class InfiniteAttribute : InfiniteDurationCategoryAttribute
		{
			/// <summary></summary>
			public InfiniteAttribute()
			{
			}
		}

		/// <summary>
		/// Returns the most appropriate standard attribute.
		/// </summary>
		[CLSCompliant(false)]
		public static DurationCategoryAttribute AttributeFrom(TimeSpan ts)
		{
			// Everything up to twice the value is accepted:

			if (ts.TotalSeconds < 2)
				return (new Second1Attribute());

			if (ts.TotalSeconds < 8)
				return (new Seconds4Attribute());

			if (ts.TotalSeconds < 30)
				return (new Seconds15Attribute());

			if (ts.TotalMinutes < 2)
				return (new Minute1Attribute());

			if (ts.TotalMinutes < 8)
				return (new Minutes4Attribute());

			if (ts.TotalMinutes < 30)
				return (new Minutes15Attribute());

			if (ts.TotalHours < 2)
				return (new Hour1Attribute());

			if (ts.TotalHours < 10)
				return (new Hours5Attribute());

			if (ts.TotalHours < 48)
				return (new Hours24Attribute());

			if (ts.TotalDays < 2)
				return (new Day1Attribute());

			if (ts.TotalDays < 14)
				return (new Days7Attribute());

			if (ts.TotalDays < 60)
				return (new Days30Attribute());

			return (new InfiniteAttribute());
		}

		/// <summary>
		/// Returns the corresponding name suffix.
		/// </summary>
		public static string CaptionFrom(TimeSpan ts)
		{
			// Prevent "0 s" captions, everything two thirds of a second shall be identified as "1 s":

			if (ts.TotalMilliseconds < 666.666)
				return (string.Format(CultureInfo.CurrentCulture, "{0:F0} ms", ts.TotalMilliseconds));

			// Everything up to twice the boundary:

			if (ts.TotalSeconds < 120)
				return (string.Format(CultureInfo.CurrentCulture, "{0:F0} s", ts.TotalSeconds));

			if (ts.TotalMinutes < 120)                                    // m is meter!
				return (string.Format(CultureInfo.CurrentCulture, "{0:F0} min", ts.TotalMinutes));

			if (ts.TotalHours < 48)
				return (string.Format(CultureInfo.CurrentCulture, "{0:F0} h", ts.TotalHours));

			return (string.Format(CultureInfo.CurrentCulture, "{0:F0} d", ts.TotalDays));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
