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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using NUnit.Framework;

namespace NUnitEx
{
	/// <summary></summary>
	public static class CategoryStrings
	{
		/// <summary></summary>
		public static readonly string Interactive = "Interactive";

		/// <summary></summary>
		public static readonly string Endurance = "Endurance";

		/// <summary></summary>
		public static readonly string Stress = "Stress";

		/// <summary></summary>
		public static readonly string Duration = "Duration";
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class InteractiveCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public InteractiveCategoryAttribute()
			: base(CategoryStrings.Interactive)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class EnduranceCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public EnduranceCategoryAttribute()
			: base(CategoryStrings.Endurance)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class StressCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public StressCategoryAttribute()
			: base(CategoryStrings.Stress)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute will be derived for specialization.")]
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class DurationCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		private readonly TimeSpan duration;

		/// <summary></summary>
		public DurationCategoryAttribute(int days, int hours, int minutes, int seconds, int milliseconds, string name)
			: this(new TimeSpan(days, hours, minutes, seconds, milliseconds), name)
		{
		}

		/// <summary></summary>
		public DurationCategoryAttribute(TimeSpan duration, string name)
			: base(name)
		{
			this.duration = duration;
		}

		/// <summary></summary>
		public virtual TimeSpan Duration
		{
			get { return (this.duration); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute will be derived for specialization.")]
	[CLSCompliant(false)]
	public class SecondDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public SecondDurationCategoryAttribute(int seconds)
			: base(0, 0, 0, seconds, 0, CategoryStrings.Duration + " is around " + seconds.ToString(CultureInfo.CurrentCulture) + " second" + ((seconds == 1) ? "" : "s"))
		{
		}

		/// <summary></summary>
		public int Seconds
		{
			get { return ((int)Duration.TotalSeconds); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute will be derived for specialization.")]
	[CLSCompliant(false)]
	public class MinuteDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public MinuteDurationCategoryAttribute(int minutes)
			: base(0, 0, minutes, 0, 0, CategoryStrings.Duration + " is around " + minutes.ToString(CultureInfo.CurrentCulture) + " minute" + ((minutes == 1) ? "" : "s"))
		{
		}

		/// <summary></summary>
		public int Minutes
		{
			get { return ((int)Duration.TotalMinutes); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute will be derived for specialization.")]
	[CLSCompliant(false)]
	public class HourDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public HourDurationCategoryAttribute(int hours)
			: base(0, hours, 0, 0, 0, CategoryStrings.Duration + " is around " + hours.ToString(CultureInfo.CurrentCulture) + " hour" + ((hours == 1) ? "" : "s"))
		{
		}

		/// <summary></summary>
		public int Hours
		{
			get { return ((int)Duration.TotalHours); }
		}
	}

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

			return (new InfiniteDurationCategoryAttribute());
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

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class InfiniteDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public InfiniteDurationCategoryAttribute()
			: base(TimeSpan.Zero, CategoryStrings.Duration + " is infinite")
		{
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
