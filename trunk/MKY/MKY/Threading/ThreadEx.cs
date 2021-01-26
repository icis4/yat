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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace MKY.Threading
{
	/// <summary>
	/// <see cref="Thread"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ThreadEx
	{
		/// <summary>
		/// The sleep interval default. High enough to not load the system more than necessary.
		/// </summary>
		public const int SleepIntervalDefault = 10;

		/// <summary>
		/// Sleeps until <paramref name="at"/>.
		/// </summary>
		/// <remarks>
		/// Use a larger <paramref name="sleepInterval"/> when sleep time is long.
		/// Use a smaller <paramref name="sleepInterval"/> to increase accuracy.
		/// Use <c>0</c> for maximum accuracy though at highest system load.
		/// </remarks>
		/// <remarks>
		/// Explicitly named 'At' to prevent ambiguity among
		/// <see cref="SleepUntilAt(DateTime, int)"/> and
		/// <see cref="SleepUntilOffset(DateTime, int, int)"/> with default argument.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void SleepUntilAt(DateTime at, int sleepInterval = SleepIntervalDefault)
		{
			var now = DateTime.Now;
			while (now < at)
			{
				Thread.Sleep(sleepInterval);
				now = DateTime.Now;
			}
		}

		/// <summary>
		/// Sleeps until <paramref name="offset"/> relative to <paramref name="baseline"/>.
		/// </summary>
		/// <remarks>
		/// Use a larger <paramref name="sleepInterval"/> when sleep time is long.
		/// Use a smaller <paramref name="sleepInterval"/> to increase accuracy.
		/// Use <c>0</c> for maximum accuracy though at highest system load.
		/// </remarks>
		/// <remarks>
		/// Explicitly named 'Offset' to prevent ambiguity among
		/// <see cref="SleepUntilAt(DateTime, int)"/> and
		/// <see cref="SleepUntilOffset(DateTime, int, int)"/> with default argument.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void SleepUntilOffset(DateTime baseline, TimeSpan offset, int sleepInterval = SleepIntervalDefault)
		{
			var at = baseline + offset;
			SleepUntilAt(at, sleepInterval);
		}

		/// <summary>
		/// Sleeps until <paramref name="offset"/> relative to <paramref name="baseline"/>.
		/// </summary>
		/// <remarks>
		/// Use a larger <paramref name="sleepInterval"/> when sleep time is long.
		/// Use a smaller <paramref name="sleepInterval"/> to increase accuracy.
		/// Use <c>0</c> for maximum accuracy though at highest system load.
		/// </remarks>
		/// <remarks>
		/// Explicitly named 'Offset' to prevent ambiguity among
		/// <see cref="SleepUntilAt(DateTime, int)"/> and
		/// <see cref="SleepUntilOffset(DateTime, int, int)"/> with default argument.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void SleepUntilOffset(DateTime baseline, int offset, int sleepInterval = SleepIntervalDefault)
		{
			SleepUntilOffset(baseline, TimeSpan.FromMilliseconds(offset), sleepInterval);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
