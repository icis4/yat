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
using System.Globalization;
using System.IO;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.File"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class FileEx
	{
		/// <summary>
		/// Default extension for backup files.
		/// </summary>
		public const string BackupFileExtension = ".bak";

		/// <summary>
		/// Returns a unique file name for a file specified by path.
		/// </summary>
		public static string MakeUniqueFileName(string path)
		{
			return (MakeUniqueFileName(path, ""));
		}

		/// <summary>
		/// Returns a unique file name for a file specified by path, unique part is separated by separator string.
		/// </summary>
		public static string MakeUniqueFileName(string path, string separator)
		{
			string dir  = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
			string name = Path.GetFileNameWithoutExtension(path);
			string ext  = Path.GetExtension(path);

			if (File.Exists(dir + name + ext))
			{
				int index = -1; // Initialize to -1 and increment before first use.
				string unique = "";
				do
				{
					index++; // No need to check for overflow, that is checked by the .NET runtime.
					unique = index.ToString(CultureInfo.InvariantCulture);
				}
				while (File.Exists(dir + name + separator + unique + ext));

				return (dir + name + separator + unique + ext);
			}
			return (dir + name + ext);
		}

		/// <summary>
		/// Tries to delete file <paramref name="filePath"/>.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if file successfully saved.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool TryDelete(string filePath)
		{
			// No need to check whether string is valid, 'File.Exists()' returns <c>false</c>
			// in such cases.
			if (File.Exists(filePath))
			{
				try
				{
					File.Delete(filePath);
					return (true);
				}
				catch
				{
					return (false);
				}
			}
			else
			{
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
