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
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Diagnostics
{
	/// <summary>
	/// Stopwatch utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class StopwatchEx
	{
		/// <summary>
		/// Converts the given number of <see cref="Stopwatch"/> ticks into the corresponding
		/// number of milliseconds, using <see cref="Stopwatch.Frequency"/>.
		/// </summary>
		/// <remarks>
		/// A similar method exists as <see cref="TimeSpanEx.TicksToTime"/>.
		/// </remarks>
		public static int TicksToTime(long ticks)
		{
			return ((int)((ticks * 1000 / Stopwatch.Frequency) + 0.5)); // Simple rounding is good enough.
		}

		/// <summary>
		/// Converts the given number of milliseconds into the corresponding number of
		/// <see cref="Stopwatch"/> ticks, using <see cref="Stopwatch.Frequency"/>.
		/// </summary>
		/// <remarks>
		/// A similar method exists as <see cref="TimeSpanEx.TimeToTicks"/>.
		/// </remarks>
		public static long TimeToTicks(int time)
		{
			return (Stopwatch.Frequency * time / 1000);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
