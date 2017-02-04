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
// MKY Development Version 1.0.18
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

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.Directory"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DirectoryEx
	{
		/// <summary>
		/// Tries to open the given path with the system's explorer.
		/// </summary>
		/// <param name="directoryPath">File to open.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool TryOpen(string directoryPath)
		{
			Exception exception;
			return (TryOpen(directoryPath, out exception));
		}

		/// <summary>
		/// Tries to open the given path with the system's explorer.
		/// </summary>
		/// <param name="directoryPath">File to open.</param>
		/// <param name="exception">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool TryOpen(string directoryPath, out Exception exception)
		{
			try
			{
				Process.Start(directoryPath);
				exception = null;
				return (true);
			}
			catch (Exception ex)
			{
				exception = ex;
				return (false);
			}
		}

		/// <summary>
		/// Makes all files within a directory writable, including or excluding sub-directories.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
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
