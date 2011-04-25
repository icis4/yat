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
// Copyright © 2010-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace NUnit
{
	/// <summary></summary>
	public static class CategoryStrings
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
		public static readonly string Interactive = "Interactive";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
		public static readonly string Endurance = "Endurance";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
		public static readonly string Stress = "Stress";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
		public static readonly string Duration = "Duration";
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class InteractiveCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public InteractiveCategoryAttribute()
			: base(CategoryStrings.Interactive)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class EnduranceCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public EnduranceCategoryAttribute()
			: base(CategoryStrings.Endurance)
		{
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class StressCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public StressCategoryAttribute()
			: base(CategoryStrings.Stress)
		{
		}
	}

	/// <summary></summary>
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class DurationCategoryAttribute : CategoryAttribute
	{
		/// <summary></summary>
		public readonly TimeSpan Duration;

		/// <summary></summary>
		public DurationCategoryAttribute(int days, int hours, int minutes, int seconds, int milliseconds, string name)
			: this(new TimeSpan(days, hours, minutes, seconds, milliseconds), name)
		{
		}

		/// <summary></summary>
		public DurationCategoryAttribute(TimeSpan duration, string name)
			: base(name)
		{
			Duration = duration;
		}
	}

	/// <summary></summary>
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class MinuteDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public MinuteDurationCategoryAttribute(int minutes)
			: base(0, 0, minutes, 0, 0, CategoryStrings.Duration + " is approx. " + minutes.ToString() + " minute(s)")
		{
		}
	}

	/// <summary></summary>
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SecondDurationCategoryAttribute : DurationCategoryAttribute
	{
		/// <summary></summary>
		public SecondDurationCategoryAttribute(int seconds)
			: base(0, 0, 0, seconds, 0, CategoryStrings.Duration + " is approx. " + seconds.ToString() + " second(s)")
		{
		}
	}

	/// <summary></summary>
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "NUnit", Justification = "NUnit is a name")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
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
