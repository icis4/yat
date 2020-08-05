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

namespace NUnit
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
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute may be inherited for specialization.")]
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

		/// <summary></summary>
		public virtual int Days
		{
			get { return (this.duration.Days); }
		}

		/// <summary></summary>
		public virtual int Hours
		{
			get { return (this.duration.Hours); }
		}

		/// <summary></summary>
		public virtual int Minutes
		{
			get { return (this.duration.Minutes); }
		}

		/// <summary></summary>
		public virtual int Seconds
		{
			get { return (this.duration.Seconds); }
		}

		/// <summary></summary>
		public virtual int Milliseconds
		{
			get { return (this.duration.Milliseconds); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute may be inherited for specialization.")]
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class HourDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public HourDurationCategoryAttribute(int hours)
			: base(0, hours, 0, 0, 0, CategoryStrings.Duration + " is approx. " + hours.ToString(CultureInfo.CurrentCulture) + " hour" + ((hours == 1) ? "" : "s"))
		{
		}

		/// <summary></summary>
		public new int Hours
		{
			get { return ((int)Duration.TotalHours); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute may be inherited for specialization.")]
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class MinuteDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public MinuteDurationCategoryAttribute(int minutes)
			: base(0, 0, minutes, 0, 0, CategoryStrings.Duration + " is approx. " + minutes.ToString(CultureInfo.CurrentCulture) + " minute" + ((minutes == 1) ? "" : "s"))
		{
		}

		/// <summary></summary>
		public new int Minutes
		{
			get { return ((int)Duration.TotalMinutes); }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute may be inherited for specialization.")]
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SecondDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public SecondDurationCategoryAttribute(int seconds)
			: base(0, 0, 0, seconds, 0, CategoryStrings.Duration + " is approx. " + seconds.ToString(CultureInfo.CurrentCulture) + " second" + ((seconds == 1) ? "" : "s"))
		{
		}

		/// <summary></summary>
		public new int Seconds
		{
			get { return ((int)Duration.TotalSeconds); }
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
