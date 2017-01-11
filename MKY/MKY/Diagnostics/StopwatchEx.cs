﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
		/// Converts the given number of <see cref="Stopwatch"/> ticks into milliseconds.
		/// </summary>
		public static int TicksToTime(long ticks)
		{
			return ((int)((ticks * 1000 / Stopwatch.Frequency) + 0.5));
		}

		/// <summary>
		/// Converts the given number of milliseconds into <see cref="Stopwatch"/> ticks.
		/// </summary>
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
