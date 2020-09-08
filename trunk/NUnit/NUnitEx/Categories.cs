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
	/// <remarks>
	/// A "Quick" category doesn't make much sense, as almost every short running test could be included
	/// in "Quick". It makes much more sense to exclude long running tests via the "Duration" categories.
	/// </remarks>
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
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class InteractiveCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public InteractiveCategoryAttribute()
			: base(CategoryStrings.Interactive)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class EnduranceCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public EnduranceCategoryAttribute()
			: base(CategoryStrings.Endurance)
		{
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class StressCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public StressCategoryAttribute()
			: base(CategoryStrings.Stress)
		{
		}
	}

	/// <summary></summary>
	[CLSCompliant(false)]
	public abstract class DurationCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		private readonly TimeSpan duration;

		/// <summary></summary>
		protected DurationCategoryAttribute(int days, int hours, int minutes, int seconds, int milliseconds, string name)
			: this(new TimeSpan(days, hours, minutes, seconds, milliseconds), name)
		{
		}

		/// <summary></summary>
		protected DurationCategoryAttribute(TimeSpan duration, string name)
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
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
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
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
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
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
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

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute will be derived for specialization.")]
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class DayDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public DayDurationCategoryAttribute(int days)
			: base(days, 0, 0, 0, 0, CategoryStrings.Duration + " is around " + days.ToString(CultureInfo.CurrentCulture) + " day" + ((days == 1) ? "" : "s"))
		{
		}

		/// <summary></summary>
		public int Days
		{
			get { return ((int)Duration.TotalDays); }
		}
	}

	/// <remarks>Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.</remarks>
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute will be derived by the standard specialization.")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class InfiniteDurationCategoryAttribute : DurationCategoryAttribute
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
