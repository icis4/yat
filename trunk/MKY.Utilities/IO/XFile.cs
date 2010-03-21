//==================================================================================================
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
using System.IO;

namespace MKY.Utilities.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.File"/>.
	/// </summary>
	public static class XFile
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
			string dir = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
			string name = Path.GetFileNameWithoutExtension(path);
			string ext = Path.GetExtension(path);

			if (File.Exists(dir + name + ext))
			{
				int index = -1;
				string unique = "";
				do
				{
					index++;
					unique = index.ToString();
				}
				while (File.Exists(dir + name + separator + unique + ext));
				return (dir + name + separator + unique + ext);
			}
			return (dir + name + ext);
		}

		/// <summary>
		/// Inidactes whether the given file steam is ready for seek/read/write.
		/// </summary>
		/// <remarks>
		/// The implementation intentionally checks the state step-by-step to emphasize the hierarchy.
		/// </remarks>
		public static bool IsReady(FileStream stream)
		{
			if (stream != null)
			{
				if (stream.CanSeek || stream.CanRead || stream.CanWrite)
				{
					if (stream.SafeFileHandle != null)
					{
						if (!stream.SafeFileHandle.IsInvalid && !stream.SafeFileHandle.IsClosed)
							return (true);
					}
				}
			}
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
