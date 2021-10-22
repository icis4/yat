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
// MKY Version 1.0.30
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

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Times for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// <see cref="DateTime"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DateTimeEx
	{
		/// <summary>
		/// Compares two specified <see cref="DateTime"/> objects ignoring anything below seconds.
		/// </summary>
		public static bool EqualsUptoSeconds(DateTime valueA, DateTime valueB)
		{
			if (valueA.Date   != valueB.Date)   return false;
			if (valueA.Hour   != valueB.Hour)   return false;
			if (valueA.Minute != valueB.Minute) return false;
			if (valueA.Second != valueB.Second) return false;

			return (true);
		}
	}

	/// <summary></summary>
	public class DateTimeEventArgs : EventArgs
	{
		/// <summary></summary>
		public DateTime DateTime { get; }

		/// <summary></summary>
		public DateTimeEventArgs(DateTime dateTime)
		{
			DateTime = dateTime;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
