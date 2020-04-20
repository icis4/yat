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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;

using MKY.Threading;
using MKY.Time;

using NUnit.Framework;

#endregion

namespace MKY.Test.Time
{
	/// <summary></summary>
	[TestFixture]
	public class RateTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void TestCreateIntervalLessThanWindow()
		{
			var rate1 = new Rate();
			var rate2 = new Rate(1000, 1500); // Intentionally using an odd interval-window-ratio.
		}

		/// <summary></summary>
		[Test]
		public virtual void TestCreateIntervalEqualsWindow()
		{
			var rate1 = new Rate(1000);
			var rate2 = new Rate(1000, 1000);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestCreateIntervalLargerThanWindow()
		{
			Assert.That(CreateIntervalLargerThanWindow, Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		/// <summary></summary>
		protected virtual void CreateIntervalLargerThanWindow()
		{
			var rate = new Rate(1000, 750); // Intentionally using an odd interval-window-ratio.
		}

		/// <summary></summary>
		[Test]
		public virtual void TestIntervalLessThanWindow()
		{
			var rate = new Rate(1000, 4000);
			var initial = DateTime.Now;

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(4)); // 4 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(8)); // 8 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(16)); // 16 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(24)); // 24 per interval; weighed per interval throughout window.

			// --- 0 ~ 20 ms have passed since 'initial' ---

			ThreadEx.SleepUntilOffset(initial, 1500);
			Assert.That(rate.Update(DateTime.Now), Is.True);
			Assert.That(rate.Value, Is.GreaterThanOrEqualTo(12)); // 12..17 per interval; weighed per interval throughout window.
			Assert.That(rate.Value, Is.LessThanOrEqualTo(17));

			ThreadEx.SleepUntilOffset(initial, 3500);
			Assert.That(rate.Update(DateTime.Now), Is.True);
			Assert.That(rate.Value, Is.EqualTo(6)); // 6 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(14)); // 14 per interval; weighed per interval throughout window.

			ThreadEx.SleepUntilOffset(initial, 4500);
			Assert.That(rate.Update(DateTime.Now), Is.True);
			Assert.That(rate.Value, Is.EqualTo(6)); // 6 per interval; weighed per interval throughout window.

			ThreadEx.SleepUntilOffset(initial, 8000);
			Assert.That(rate.Update(DateTime.Now), Is.True);
			Assert.That(rate.Value, Is.EqualTo(0)); // Back to zero.
		}

		/// <remarks>Using an odd interval-window-ratio to check for related issues.</remarks>
		[Test]
		public virtual void TestIntervalLessThanWindowWithOddRatio()
		{
			var rate = new Rate(1000, 1500);
			var initial = DateTime.Now;

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(7)); // 7 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(13)); // 13 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(27)); // 27 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(40)); // 40 per interval; weighed per interval throughout window.

			// --- 0 ~ 20 ms have passed since 'initial' ---

			ThreadEx.SleepUntilOffset(initial, 500);
			Assert.That(rate.Update(DateTime.Now), Is.True);
			Assert.That(rate.Value, Is.GreaterThanOrEqualTo(20)); // Reduced to 20..37 per interval; weighed per interval throughout window.
			Assert.That(rate.Value, Is.LessThanOrEqualTo(37));

			ThreadEx.SleepUntilOffset(initial, 750);
			Assert.That(rate.Update(30), Is.True);
			Assert.That(rate.Value, Is.EqualTo(40)); // Again 40 per interval; weighed per interval throughout window.

			ThreadEx.SleepUntilOffset(initial, 1250);
			Assert.That(rate.Update(30), Is.True);
			Assert.That(rate.Value, Is.GreaterThanOrEqualTo(50)); // Increased to 50..60 per interval; weighed per interval throughout window.
			Assert.That(rate.Value, Is.LessThanOrEqualTo(60));

			ThreadEx.SleepUntilOffset(initial, 3000);
			Assert.That(rate.Update(DateTime.Now), Is.True);
			Assert.That(rate.Value, Is.EqualTo(0)); // Back to zero.
		}

		/// <summary></summary>
		[Test]
		public virtual void TestIntervalEqualsWindow()
		{
			var rate = new Rate(1000, 1000);
			var initial = DateTime.Now;

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(10)); // 10 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(20)); // 20 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(40)); // 40 per interval; weighed per interval throughout window.

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(60)); // 60 per interval; weighed per interval throughout window.

			// --- 0 ~ 20 ms have passed since 'initial' ---

			ThreadEx.SleepUntilOffset(initial, 500);
			Assert.That(rate.Update(DateTime.Now), Is.False);
			Assert.That(rate.Value, Is.EqualTo(60)); // Still 60 per interval.

			ThreadEx.SleepUntilOffset(initial, 750);
			Assert.That(rate.Update(30), Is.True);
			Assert.That(rate.Value, Is.EqualTo(90)); // 90 per interval; weighed per interval throughout window.

			ThreadEx.SleepUntilOffset(initial, 1250);
			Assert.That(rate.Update(30), Is.True);
			Assert.That(rate.Value, Is.EqualTo(60)); // Just most recent 2 x 30 per interval.

			ThreadEx.SleepUntilOffset(initial, 2500);
			Assert.That(rate.Update(DateTime.Now), Is.True);
			Assert.That(rate.Value, Is.EqualTo(0)); // Back to zero.
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
