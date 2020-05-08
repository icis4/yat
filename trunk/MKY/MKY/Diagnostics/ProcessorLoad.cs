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

#if (DEBUG)

	// Enable debugging of update:
////#define DEBUG_UPDATE

#endif // DEBUG

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MKY.Diagnostics
{
	/// <summary>
	/// A better way to measure the processor time than using a <see cref="PerformanceCounter"/>,
	/// because that...
	/// ...is pretty resource hungry, and...
	/// ...is language dependent!
	///
	/// Saying hello to StyleCop ;-.
	/// </summary>
	/// <remarks>
	/// Based on http://stackoverflow.com/questions/19756454/calculating-process-cpu-usage-from-process-totalprocessortime.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	public static class ProcessorLoad
	{
		private const int MinUpdateIntervalDefault = 100;

		private static int staticMinUpdateInterval = MinUpdateIntervalDefault;
		private static long staticLastUpdateTimestamp; // Ticks as defined by 'Stopwatch'.
		private static TimeSpan staticLastProcessorTime = TimeSpan.Zero;
		private static object staticSyncObj = new object();

		/// <summary>
		/// The current processor load percentage of the current process.
		/// </summary>
		public static int CurrentPercentage { get; private set; }

		/// <summary>Initializes the processor load evaluation.</summary>
		/// <remarks>Call once as soon as the application is ready.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void Initialize(int minUpdateInterval = MinUpdateIntervalDefault)
		{
			lock (staticSyncObj)
			{
				staticMinUpdateInterval = minUpdateInterval;
				staticLastUpdateTimestamp = Stopwatch.GetTimestamp();
				staticLastProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
			}
		}

		/// <summary>Updates the processor load evaluation.</summary>
		/// <remarks>Call in regular intervals, e.g. each second.</remarks>
		/// <remarks>Keep in mind that updating also takes time, below a millisecond, but still time.</remarks>
		/// <returns>The current processor load percentage of the current process.</returns>
		public static int Update()
		{
			int result;

			DebugUpdate("Updating...");

			lock (staticSyncObj)
			{
				long currentUpdateTimestamp = Stopwatch.GetTimestamp();
				int intervalTime;

				unchecked
				{
					intervalTime = StopwatchEx.TicksToTime(currentUpdateTimestamp - staticLastUpdateTimestamp);
				}

				if (intervalTime >= staticMinUpdateInterval)
				{
					staticLastUpdateTimestamp = currentUpdateTimestamp;

					TimeSpan currentProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
					TimeSpan intervalProcessorTime = currentProcessorTime - staticLastProcessorTime;
					staticLastProcessorTime = currentProcessorTime;

					if (intervalTime > 0)
					{
						int percentage = (int)(100.0 * (intervalProcessorTime.TotalMilliseconds / intervalTime));
						CurrentPercentage = Math.Min(percentage, 100);
					}
				}

				result = CurrentPercentage;
			}

			DebugUpdate("...done");

			return (result);
		}

		/// <remarks>
		/// <c>private</c> because <see cref="ConditionalAttribute"/> only works locally.
		/// </remarks>
		[Conditional("DEBUG_UPDATE")]
		private static void DebugUpdate(string message)
		{
			Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo) + " : " + message);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
