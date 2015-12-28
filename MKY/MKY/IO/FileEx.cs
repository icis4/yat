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
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
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
		/// Returns the size of the given file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// The size of the given file.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "fi", Justification = "Required to force exception.")]
		public static long Size(string filePath)
		{
			try
			{
				FileInfo fi = new FileInfo(filePath);
				return (fi.Length);
			}
			catch
			{
				return (0);
			}
		}

		/// <summary>
		/// Checks whether the given file is readable.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// Returns <c>true</c> if file is readable.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "fi", Justification = "Required to force exception.")]
		public static bool IsReadable(string filePath)
		{
			try
			{
				// Force exception if file is not accessible:
				FileInfo fi = new FileInfo(filePath);
				return (fi.Exists);
			}
			catch
			{
				return (false);
			}
		}

		/// <summary>
		/// Checks whether the given file is read-only.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// Returns <c>true</c> if file is read-only.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool IsReadOnly(string filePath)
		{
			try
			{
				FileInfo fi = new FileInfo(filePath);
				return (fi.Exists && fi.IsReadOnly);
			}
			catch
			{
				return (false);
			}
		}

		/// <summary>
		/// Checks whether the given file is writeable.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// Returns <c>true</c> if file is writeable.
		/// </returns>
		public static bool IsWritable(string filePath)
		{
			return (!IsReadOnly(filePath));
		}

		/// <summary>
		/// Checks whether the given file is findable, e.g. via the system's PATH variable.
		/// </summary>
		/// <param name="fileName">The file name, typically an executable.</param>
		/// <returns>
		/// Returns <c>true</c> if file is findable.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool IsFindable(string fileName)
		{
			string filePath;
			bool result = IsFindable(fileName, out filePath);
			UnusedLocal.PreventAnalysisWarning(filePath);
			return (result);
		}

		/// <summary>
		/// Checks whether the given file is findable, e.g. via the system's PATH variable.
		/// </summary>
		/// <param name="fileName">The file name, typically an executable.</param>
		/// <param name="filePath">The path to the file.</param>
		/// <returns>
		/// Returns <c>true</c> if file is findable.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool IsFindable(string fileName, out string filePath)
		{
			try
			{
				Process p = new Process();
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.FileName = "WHERE";
				p.StartInfo.Arguments = fileName;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.CreateNoWindow = true;
				p.Start();

				string whereResult = p.StandardOutput.ReadToEnd();

				p.WaitForExit();

				if (p.ExitCode == 0)
				{
					filePath = whereResult.Substring(0, whereResult.IndexOf(Environment.NewLine));
					return (true);
				}
				else
				{
					filePath = null;
					return (false);
				}
			}
			catch
			{
				filePath = null;
				return (false);
			}
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
