//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		/// <summary></summary>
		public static int TicksToTimeout(long ticks)
		{
			return ((int)(ticks * 1000 / Stopwatch.Frequency));
		}

		/// <summary></summary>
		public static long TimeoutToTicks(int timeoutMs)
		{
			return (Stopwatch.Frequency * timeoutMs / 1000);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
