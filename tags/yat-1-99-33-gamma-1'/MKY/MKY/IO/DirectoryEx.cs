//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.File"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DirectoryEx
	{
		/// <summary>
		/// Makes all files within a directory writable, including all sub-directories.
		/// </summary>
		public static void MakeAllFilesWritable(string path)
		{
			MakeAllFilesWritable(path, true);
		}

		/// <summary>
		/// Makes all files within a directory writable.
		/// </summary>
		public static void MakeAllFilesWritable(string path, bool recursive)
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
