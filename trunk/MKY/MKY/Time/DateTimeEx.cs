﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Times for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// DateTime utility methods.
	/// </summary>
	public static class DateTimeEx
	{
		/// <summary>
		/// Returns time span formatted with "yyyy-mm-dd".
		/// </summary>
		public static string FormatInvariantDate(DateTime dateTime)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(dateTime.Year.ToString("D4"));
			sb.Append("-");
			sb.Append(dateTime.Month.ToString("D2"));
			sb.Append("-");
			sb.Append(dateTime.Day.ToString("D2"));

			return (sb.ToString());
		}

		/// <summary>
		/// Returns time formatted with "hh:mm:ss".
		/// </summary>
		public static string FormatInvariantTime(DateTime dateTime)
		{
			return (FormatInvariantTime(dateTime, false));
		}

		/// <summary>
		/// Returns time span formatted with "hh:mm:ss[.hh]".
		/// </summary>
		public static string FormatInvariantTime(DateTime dateTime, bool hundredths)
		{
			StringBuilder sb = new StringBuilder();

			if (hundredths)
			{
				sb.Insert(0, (dateTime.Millisecond / 10).ToString("D2"));
				sb.Insert(0, ".");
			}

			sb.Insert(0, dateTime.Second.ToString("D2"));
			sb.Insert(0, ":");
			sb.Insert(0, dateTime.Minute.ToString("D2"));
			sb.Insert(0, ":");
			sb.Insert(0, dateTime.Hour.ToString("D2"));

			return (sb.ToString());
		}
	}

	/// <summary></summary>
	public class DateTimeEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly DateTime DateTime;

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
