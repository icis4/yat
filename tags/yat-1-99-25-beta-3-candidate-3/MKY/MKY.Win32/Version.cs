//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
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
			if (!EnvironmentEx.IsWindows())
				return (false);

			// Windows Vista is version 6.0.
			System.Version versionXP = new System.Version(6, 0);

			System.Version environmentVersion = Environment.OSVersion.Version;
			return (environmentVersion >= versionXP);
		}

		/// <summary>
		/// Find out if the current operating system is Windows XP or later.
		/// </summary>
		public static bool IsWindowsXpOrLater()
		{
			if (!EnvironmentEx.IsWindows())
				return (false);

			// Windows XP is version 5.1.
			System.Version versionXP = new System.Version(5, 1);

			System.Version environmentVersion = Environment.OSVersion.Version;
			return (environmentVersion >= versionXP);
		}

		/// <summary>
		/// Find out if the current operating system is Windows 98 Standard Edition.
		/// </summary>
		public static bool IsWindows98Standard()
		{
			if (!EnvironmentEx.IsWindows())
				return (false);

			// Windows 98 Standard Edition is version 4.10 with a build number less than 2183.
			System.Version version98 = new System.Version(4, 10);
			System.Version versionAbove98 = new System.Version(4, 10, 2183);

			System.Version environmentVersion = Environment.OSVersion.Version;
			return ((environmentVersion >= version98) && (environmentVersion < versionAbove98));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================