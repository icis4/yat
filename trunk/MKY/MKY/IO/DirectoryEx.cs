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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Security.Permissions;

#endregion

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="Directory"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DirectoryEx
	{
		/// <summary>
		/// Determines whether the given path can be written to.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static bool IsWritable(string path)
		{
			var set = new PermissionSet(PermissionState.None);
			var fileWritePermission = new FileIOPermission(FileIOPermissionAccess.Write, path);
			set.AddPermission(fileWritePermission);
			return (set.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet));
		}

		/// <summary>
		/// Tries to open the given path with the system's file browser/explorer.
		/// </summary>
		/// <remarks>
		/// Named 'Browse' instead of 'Open' to emphasize that system browser/explorer will be used.
		/// </remarks>
		/// <param name="path">Directory to browse.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool TryBrowse(string path)
		{
			Exception exceptionOnFailure;
			return (TryBrowse(path, out exceptionOnFailure));
		}

		/// <summary>
		/// Tries to open the given path with the system's file browser/explorer.
		/// </summary>
		/// <remarks>
		/// Named 'Browse' instead of 'Open' to emphasize that system browser/explorer will be used.
		/// </remarks>
		/// <param name="path">Directory to browse.</param>
		/// <param name="exceptionOnFailure">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryBrowse(string path, out Exception exceptionOnFailure)
		{
			try
			{
				Process.Start(path);
				exceptionOnFailure = null;
				return (true);
			}
			catch (Exception ex)
			{
				exceptionOnFailure = ex;
				return (false);
			}
		}

		/// <summary>
		/// Makes all files within a directory writable, including or excluding sub-directories.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void MakeAllFilesWritable(string path, bool recursive = true)
		{
			if (Directory.Exists(path))
			{
				// Recurse into sub-directories:
				if (recursive)
				{
					foreach (string directoryName in Directory.GetDirectories(path))
						MakeAllFilesWritable(Path.Combine(path, directoryName), recursive); // Recursion!
				}

				// Make files of this directory writable:
				foreach (string fileName in Directory.GetFiles(path))
				{
					File.SetAttributes(Path.Combine(path, fileName), FileAttributes.Normal);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
