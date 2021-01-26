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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;

using MKY.Threading;
using MKY.Time;

using NUnit.Framework;
using NUnitEx;

#endregion

namespace MKY.Test.Time
{
	/// <summary></summary>
	[TestFixture]
	public class RateTest
	{
		private const double IntervalLong     =  60000;   // 1 min
		private const double IntervalNormal   =   1000;   // 1 s
		private const double IntervalShort    =      1;   // 1 ms

		private const double WindowEvenLong   = 120000;   // 2 min (test requires same ratio as 'normal')
		private const double WindowEvenNormal =   2000;   // 2 s
		private const double WindowEvenShort  =      2;   // 2 ms (test requires same ratio as 'normal')

		private const double WindowOddLong    =  90000;   // 1.5 min (test requires same ratio as 'normal')
		private const double WindowOddNormal  =   1500;   // 1.5 s
		private const double WindowOddShort   =      1.5; // 1.5 ms (test requires same ratio as 'normal')

		private const int UpdateIntervalNormal = 100; // 100 ms
		private const int UpdateItemNormal = 10;

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Create
		//------------------------------------------------------------------------------------------
		// Tests > Create
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestCreate_IntervalEqualsWindow()
		{
			var dummyRateToTestConstrutor        = new Rate();
			var dummyRateToTestConstrutorWith1ms = new Rate(1);
			var dummyRateToTestConstrutorWith1s  = new Rate(1000);
			var dummyRateToTestConstrutorWith1sb = new Rate(1000, 1000);

			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutor,        "Dummy variable improves code readability.");
			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutorWith1ms, "Dummy variable improves code readability.");
			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutorWith1s,  "Dummy variable improves code readability.");
			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutorWith1sb, "Dummy variable improves code readability.");
		}

		/// <summary></summary>
		[Test]
		public virtual void TestCreate_IntervalLessThanWindowWithEvenRatio()
		{
			var dummyRateToTestConstrutorWith1ms = new Rate(1, 4);
			var dummyRateToTestConstrutorWith1s  = new Rate(1000, 4000);

			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutorWith1ms, "Dummy variable improves code readability.");
			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutorWith1s,  "Dummy variable improves code readability.");
		}

		/// <summary></summary>
		[Test]
		public virtual void TestCreate_IntervalLessThanWindowWithOddRatio()
		{
			var dummyRateToTestConstrutorWith1ms = new Rate(1, 1.5);
			var dummyRateToTestConstrutorWith1s  = new Rate(1000, 1500);

			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutorWith1ms, "Dummy variable improves code readability.");
			UnusedLocal.PreventAnalysisWarning(dummyRateToTestConstrutorWith1s,  "Dummy variable improves code readability.");
		}

		/// <summary></summary>
		[Test, AssertThrowsCategory]
		public virtual void TestCreate_IntervalGreaterThanWindow()
		{
			Assert.That(CreateIntervalGreaterThanWindow1ms, Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(CreateIntervalGreaterThanWindow1s,  Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ms", Justification = "'ms' is the correct abbreviation for milliseconds.")]
		protected virtual void CreateIntervalGreaterThanWindow1ms()
		{
			var dummyRateToForceException = new Rate(1, 0.75); // Intentionally using an odd interval-window-ratio.

			UnusedLocal.PreventAnalysisWarning(dummyRateToForceException, "Dummy variable improves code readability.");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "s", Justification = "'s' is the correct abbreviation for seconds.")]
		protected virtual void CreateIntervalGreaterThanWindow1s()
		{
			var dummyRateToForceException = new Rate(1000, 750); // Intentionally using an odd interval-window-ratio.

			UnusedLocal.PreventAnalysisWarning(dummyRateToForceException, "Dummy variable improves code readability.");
		}

		#endregion

		#region Tests > UpdateTimed
		//------------------------------------------------------------------------------------------
		// Tests > UpdateTimed
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, Sequential] // Sequential for minutes, seconds and milliseconds.
		public virtual void TestUpdateTimed_IntervalEqualsWindow
		(
			[Values(IntervalLong, IntervalNormal, IntervalShort)] double interval,
			[Values(IntervalLong, IntervalNormal, IntervalShort)] double window
		)
		{
			var rate = new Rate(interval, window);
			var initial = DateTime.Now;

			Assert.That(rate.Update(10), Is.True); // Values are higher than sum of items because part of window is yet empty.
			Assert.That(rate.Value, Is.EqualTo(13)); // 13 per interval; weighted per bin throughout window.

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(27));

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(53));

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(80));

			if (DoubleEx.AlmostEquals(interval, IntervalNormal)) // Timing controlled tests are limited to work for intervals of 1 second, in order to guarentee timing accuracy.
			{
				// --- 0 ~ 20 ms have passed since 'initial' ---

				ThreadEx.SleepUntilOffset(initial, 100); // For sure in 1st bin, bins are 500 ms wide.
				Assert.That(rate.Update(), Is.False);
				Assert.That(rate.Value, Is.EqualTo(80)); // Still 80 per interval.
				Assert.That(rate.Update(20), Is.True);
				Assert.That(rate.Value, Is.EqualTo(107));

				ThreadEx.SleepUntilOffset(initial, 700); // For sure in 2nd bin, using 600 ms offsets for enough margin.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(53)); // Down to 53 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(107)); // Up to 107 per interval.

				ThreadEx.SleepUntilOffset(initial, 1300); // For sure in bin after the end of the initial window.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(27)); // Down to 27 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(80)); // Up to 80 per interval.

				ThreadEx.SleepUntilOffset(initial, 1900);
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(27)); // Again down to 27 per interval.

				ThreadEx.SleepUntilOffset(initial, 2500); // For sure a window after the last update.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(0)); // Back to zero.
			}
		}

		/// <summary></summary>
		[Test, Sequential] // Sequential for minutes, seconds and milliseconds.
		public virtual void TestUpdateTimed_IntervalLessThanWindowWithEvenRatio
		(
			[Values(  IntervalLong,   IntervalNormal,   IntervalShort)] double interval,
			[Values(WindowEvenLong, WindowEvenNormal, WindowEvenShort)] double window
		)
		{
			var rate = new Rate(interval, window);
			var initial = DateTime.Now;

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(8)); // 8 per interval; weighted per bin throughout window.

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(16));

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(32));

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(48));

			if (DoubleEx.AlmostEquals(interval, IntervalNormal)) // Timing controlled tests are limited to work for intervals of 1 second, in order to guarentee timing accuracy.
			{
				// --- 0 ~ 20 ms have passed since 'initial' ---

				ThreadEx.SleepUntilOffset(initial, 100); // For sure in 1st bin, bins are 500 ms wide.
				Assert.That(rate.Update(), Is.False);
				Assert.That(rate.Value, Is.EqualTo(48)); // Still 48 per interval.
				Assert.That(rate.Update(20), Is.True);
				Assert.That(rate.Value, Is.EqualTo(64));

				ThreadEx.SleepUntilOffset(initial, 700); // For sure in 2nd bin, using 600 ms offsets for enough margin.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(48)); // Down to 48 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(80)); // Up to 80 per interval.

				ThreadEx.SleepUntilOffset(initial, 1300);
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(56)); // Down to 56 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(88)); // Up to 88 per interval.

				ThreadEx.SleepUntilOffset(initial, 1900); // For sure in bin before the end of the initial window.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(56)); // Again down to 56 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(88)); // Again up to 88 per interval.

				ThreadEx.SleepUntilOffset(initial, 2500); // For sure in bin after the end of the initial window.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(48)); // Down to 48 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(80)); // Up to 80 per interval.

				ThreadEx.SleepUntilOffset(initial, 3100);
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(48)); // Again down to 48 per interval.

				ThreadEx.SleepUntilOffset(initial, 3700);
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(24)); // Down to 24 per interval.

				ThreadEx.SleepUntilOffset(initial, 4300);
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(8)); // Down to 8 per interval.

				ThreadEx.SleepUntilOffset(initial, 4900); // For sure a window after the last update.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(0)); // Back to zero.
			}
		}

		/// <summary></summary>
		[Test, Sequential] // Sequential for minutes, seconds and milliseconds.
		public virtual void TestUpdateTimed_IntervalLessThanWindowWithOddRatio
		(
			[Values( IntervalLong,  IntervalNormal,  IntervalShort)] double interval,
			[Values(WindowOddLong, WindowOddNormal, WindowOddShort)] double window
		)
		{
			var rate = new Rate(interval, window);
			var initial = DateTime.Now;

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(10)); // 10 per interval; weighted per bin throughout window.

			Assert.That(rate.Update(10), Is.True);
			Assert.That(rate.Value, Is.EqualTo(20));

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(40));

			Assert.That(rate.Update(20), Is.True);
			Assert.That(rate.Value, Is.EqualTo(60));

			if (DoubleEx.AlmostEquals(interval, IntervalNormal)) // Timing controlled tests are limited to work for intervals of 1 second, in order to guarentee timing accuracy.
			{
				// --- 0 ~ 20 ms have passed since 'initial' ---

				ThreadEx.SleepUntilOffset(initial, 100); // For sure in 1st bin, bins are 500 ms wide.
				Assert.That(rate.Update(), Is.False);
				Assert.That(rate.Value, Is.EqualTo(60)); // Still 60 per interval.
				Assert.That(rate.Update(20), Is.True);
				Assert.That(rate.Value, Is.EqualTo(80));

				ThreadEx.SleepUntilOffset(initial, 700); // For sure in 2nd bin, using 600 ms offsets for enough margin.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(53)); // Down to 53 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(93)); // Up to 93 per interval.

				ThreadEx.SleepUntilOffset(initial, 1300); // For sure in bin before the end of the initial window.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(53)); // Again down to 53 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(93)); // Again up to 93 per interval.

				ThreadEx.SleepUntilOffset(initial, 1900); // For sure in bin after the end of the initial window.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(40)); // Down to 40 per interval.
				Assert.That(rate.Update(40), Is.True);
				Assert.That(rate.Value, Is.EqualTo(80)); // Up to 80 per interval.

				ThreadEx.SleepUntilOffset(initial, 2500);
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(40)); // Again down to 40 per interval.

				ThreadEx.SleepUntilOffset(initial, 3100);
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(13)); // Down to 13 per interval.

				ThreadEx.SleepUntilOffset(initial, 3700); // For sure a window after the last update.
				Assert.That(rate.Update(), Is.True);
				Assert.That(rate.Value, Is.EqualTo(0)); // Back to zero.
			}
		}

		#endregion

		#region Tests > UpdateAbsolute
		//------------------------------------------------------------------------------------------
		// Tests > UpdateAbsolute
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Tested for 'normal' sub-second values only; testing sub-millisecond values is not
		/// feasible with <see cref="DateTime"/> and <see cref="TimeSpan"/>.
		/// </remarks>
		[Test]
		public virtual void TestUpdateAbsolute_IntervalEqualsWindow
		(
			[Values(      IntervalNormal)] double interval,
			[Values(      IntervalNormal)] double window,
			[Values(UpdateIntervalNormal)] int    updateInterval,
			[Values(    UpdateItemNormal)] int    item
		)
		{
			var rate = new Rate(interval, window);
			var initial = new DateTime(2000, 1, 1, 0, 0, 0, 0); // Midnight.
			var updateIncrement = new TimeSpan(0, 0, 0, 0, updateInterval);

			var now = initial;

			Assert.That(rate.Update(now), Is.False); // Must remain at 0.
			Assert.That(rate.Value, Is.EqualTo(0));

			var updatesPerInterval = ((int)interval / updateInterval); // 10.
			var updatesPerWindow = ((int)window / updateInterval); // 40.

			var expectedRampValues = new int[]{ 13, 27, 40, 53, 67, 73, 80, 87, 93, 100 };
			var expectedStableValue = (updatesPerInterval * item);

			// Ramp-up:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, item), Is.True); // Expecting change of ramp-up value on every update.
				Assert.That(rate.Value, Is.EqualTo(expectedRampValues[i]));

				now += updateIncrement;
			}

			// Stable:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, item), Is.False); // Expecting stable at 100.
				Assert.That(rate.Value, Is.EqualTo(expectedStableValue));

				now += updateIncrement;
			}

			// Ramp-down:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, 0), Is.True); // Expecting change of ramp-up value on every update.
				Assert.That(rate.Value, Is.EqualTo(100 - expectedRampValues[i]));

				now += updateIncrement;
			}
		}

		/// <remarks>
		/// Tested for 'normal' sub-second values only; testing sub-millisecond values is not
		/// feasible with <see cref="DateTime"/> and <see cref="TimeSpan"/>.
		/// </remarks>
		[Test]
		public virtual void TestUpdateAbsolute_IntervalLessThanWindowWithEvenRatio
		(
			[Values(      IntervalNormal)] double interval,
			[Values(    WindowEvenNormal)] double window,
			[Values(UpdateIntervalNormal)] int    updateInterval,
			[Values(    UpdateItemNormal)] int    item
		)
		{
			var rate = new Rate(interval, window);
			var initial = new DateTime(2000, 1, 1, 0, 0, 0, 0); // Midnight.
			var updateIncrement = new TimeSpan(0, 0, 0, 0, updateInterval);

			var now = initial;

			Assert.That(rate.Update(now), Is.False); // Must remain at 0.
			Assert.That(rate.Value, Is.EqualTo(0));

			var updatesPerInterval = ((int)interval / updateInterval); // 10.
			var updatesPerWindow = ((int)window / updateInterval); // 40.

			var expectedRampValues = new int[]{ 8, 16, 24, 32, 40, 46, 52, 58, 64, 70, 74, 78, 82, 86, 90, 92, 94, 96, 98, 100 };
			var expectedStableValue = (updatesPerInterval * item);

			// Ramp-up:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, item), Is.True); // Expecting change of ramp-up value on every update.
				Assert.That(rate.Value, Is.EqualTo(expectedRampValues[i]));

				now += updateIncrement;
			}

			// Stable:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, item), Is.False); // Expecting stable at 100.
				Assert.That(rate.Value, Is.EqualTo(expectedStableValue));

				now += updateIncrement;
			}

			// Ramp-down:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, 0), Is.True); // Expecting change of ramp-up value on every update.
				Assert.That(rate.Value, Is.EqualTo(100 - expectedRampValues[i]));

				now += updateIncrement;
			}
		}

		/// <remarks>
		/// Tested for 'normal' sub-second values only; testing sub-millisecond values is not
		/// feasible with <see cref="DateTime"/> and <see cref="TimeSpan"/>.
		/// </remarks>
		[Test]
		public virtual void TestUpdateAbsolute_IntervalLessThanWindowWithOddRatio
		(
			[Values(      IntervalNormal)] double interval,
			[Values(     WindowOddNormal)] double window,
			[Values(UpdateIntervalNormal)] int    updateInterval,
			[Values(    UpdateItemNormal)] int    item
		)
		{
			var rate = new Rate(interval, window);
			var initial = new DateTime(2000, 1, 1, 0, 0, 0, 0);
			var updateIncrement = new TimeSpan(0, 0, 0, 0, updateInterval);

			var now = initial;

			Assert.That(rate.Update(now), Is.False); // Must remain at 0.
			Assert.That(rate.Value, Is.EqualTo(0));

			now += updateIncrement; // 100 ms after midnight.

			var updatesPerInterval = ((int)interval / updateInterval); // 10.
			var updatesPerWindow = ((int)window / updateInterval); // 15.

			var expectedRampValues = new int[]{ 10, 20, 30, 40, 50, 57, 63, 70, 77, 83, 87, 90, 93, 97, 100 };
			var expectedStableValue = (updatesPerInterval * item);

			// Ramp-up:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, item), Is.True); // Expecting change of ramp-up value on every update.
				Assert.That(rate.Value, Is.EqualTo(expectedRampValues[i]));

				now += updateIncrement;
			}

			// Stable:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, item), Is.False); // Expecting stable at 100.
				Assert.That(rate.Value, Is.EqualTo(expectedStableValue));

				now += updateIncrement;
			}

			// Ramp-down:
			for (int i = 0; i < updatesPerWindow; i++)
			{
				Assert.That(rate.Update(now, 0), Is.True); // Expecting change of ramp-up value on every update.
				Assert.That(rate.Value, Is.EqualTo(100 - expectedRampValues[i]));

				now += updateIncrement;
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
