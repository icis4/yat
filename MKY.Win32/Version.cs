//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates Win32 version methods.
	/// </summary>
	public static class Version
	{
		/// <summary>
		/// Find out if the current operating system is Windows Vista or later.
		/// </summary>
		public static bool IsWindowsVistaOrLater()
		{
			OperatingSystem environment = Environment.OSVersion;

			//  Windows Vista is version 6.0.
			System.Version versionXP = new System.Version(6, 0);

			return (environment.Version >= versionXP);
		}

		/// <summary>
		/// Find out if the current operating system is Windows XP or later.
		/// </summary>
		public static bool IsWindowsXpOrLater()
		{
			OperatingSystem environment = Environment.OSVersion;

			//  Windows XP is version 5.1.
			System.Version versionXP = new System.Version(5, 1);

			return (environment.Version >= versionXP);
		}

		/// <summary>
		/// Find out if the current operating system is Windows 98 Standard Edition.
		/// </summary>
		public static bool IsWindows98Standard()
		{
			OperatingSystem environment = Environment.OSVersion;

			//  Windows 98 Standard Edition is version 4.10 with a build number less than 2183.
			System.Version version98 = new System.Version(4, 10);
			System.Version versionAbove98 = new System.Version(4, 10, 2183);

			return ((environment.Version >= version98) && (environment.Version < versionAbove98));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
