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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.Path"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class PathEx
	{
		#region Is...()

		/// <summary>
		/// Returns whether the directory or file path is defined.
		/// </summary>
		public static bool IsDefined(string path)
		{
			// String validation:
			if (string.IsNullOrEmpty(path))
				return (false);

			return (true);
		}

		/// <summary>
		/// Returns whether the directory or file path is valid.
		/// </summary>
		/// <remarks>
		/// This method only checks whether the path is valid but not whether it actually exists.
		/// Use <see cref="Directory.Exists"/> or <see cref="File.Exists"/> to check existence.
		/// </remarks>
		public static bool IsValid(string path)
		{
			// String validation:
			if (string.IsNullOrEmpty(path))
				return (false);

			// Path validation:
			return (!string.IsNullOrEmpty(Path.GetFullPath(path)));
		}

		#endregion

		#region ...Extension()

		/// <summary>
		/// Normalizes an extension, i.e. prepends a '.' if missing.
		/// </summary>
		public static string NormalizeExtension(string extension)
		{
			if (string.IsNullOrEmpty(extension))
				return (extension);

			if (extension[0] == '.') // Already normalized.
				return (extension);

			// Normalize:
			return (extension.Insert(0, "."));
		}

		/// <summary>
		/// Denormalizes an extension, i.e. removes a '.' if apparent.
		/// </summary>
		public static string DenormalizeExtension(string extension)
		{
			if (string.IsNullOrEmpty(extension))
				return (extension);

			if (extension[0] != '.') // Already denormalized.
				return (extension);

			// Denormalize:
			return (extension.TrimStart('.'));
		}

		#endregion

		#region InvalidPathRoot

		/// <summary>
		/// Returns an invalid path root.
		/// </summary>
		public static string InvalidPathRoot
		{
			get
			{
				if (EnvironmentEx.IsWindows)
				{
					// Explicitly implemented to emphasize logic and order of the operation below.
					// Do not use A, B and C because they have a dedicated meaning on Windows.
					// Implemented as const string to improve performance when calling this method.
					const string RootLetters = @"Z Y X W V U T S R Q P O N M L K J I H G F E D";
					List<string> existingPathRoots = new List<string>(Directory.GetLogicalDrives());
					foreach (string rootLetter in RootLetters.Split())
					{
						string currentPathRoot = rootLetter + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
						if (!existingPathRoots.Contains(currentPathRoot))
							return (currentPathRoot);
					}

					// All local path roots seem valid, create a random remote path root:
					while (true)
					{
						// E.g. \\SomeRandName\
						string randomPathRoot = Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + Path.GetRandomFileName() + Path.DirectorySeparatorChar;
						if (!Directory.Exists(randomPathRoot))
							return (randomPathRoot);
					}
				}
				else
				{
					while (true)
					{
						// E.g. /SomeRandName/
						string randomPathRoot = Path.DirectorySeparatorChar + Path.GetRandomFileName() + Path.DirectorySeparatorChar;
						if (!Directory.Exists(randomPathRoot))
							return (randomPathRoot);
					}
				}
			}
		}

		#endregion

		#region ConvertPathToPlatform()

		/// <summary>
		/// Convert non-platform separators according to platform.
		/// </summary>
		public static string ConvertToPlatform(string path)
		{
			// e.g. replace '/' by '\'
			return (path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
		}

		#endregion

		#region Equals()

		/// <summary>
		/// Compares two specified path string dependent on the operating systems policies.
		/// </summary>
		public static bool Equals(string pathA, string pathB)
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
					return (string.Compare(pathA, pathB, StringComparison.OrdinalIgnoreCase) == 0);

				case PlatformID.Unix:
				case PlatformID.MacOSX:
				default:
					return (string.Compare(pathA, pathB, StringComparison.Ordinal) == 0);
			}
		}

		#endregion

		#region Limit...()

		/// <summary>
		/// Limits a directory or file path to the specified max length.
		/// </summary>
		public static string LimitPath(string path, int length)
		{
			string limitedPath;

			if (path.Length <= length)                 // Path string too long ?
				return (path);

			if (path.IndexOf(Path.VolumeSeparatorChar) < 0)
			{                                          // Local path ?
				limitedPath = StringEx.Left(path, 3) + "..." +
				              StringEx.Right(path, Math.Max(length - 6, 0));
			}
			else                                       // Network path !
			{
				int separatorPosition = path.Substring(4).IndexOf(Path.DirectorySeparatorChar);
				if ((separatorPosition >= 0) && (separatorPosition < length - 4))
				{
					limitedPath = StringEx.Left(path, separatorPosition) + "..." +
					              StringEx.Right(path, Math.Max(length - 4 - separatorPosition, 0));
				}
				else
				{
					limitedPath = StringEx.Left(path, 5) + "..." +
					              StringEx.Right(path, Math.Max(length - 8, 0));
				}
			}

			return (StringEx.Right(limitedPath, length));
		}

		#endregion

		#region Append...()

		/// <summary>
		/// Appends a directory or file path to the specified max length.
		/// </summary>
		/// <example>
		/// <code>
		/// string filePath = "C:\\Temp\\MyFile.txt";
		/// filePath = PathEx.AppendPostfixToFileName(filePath, "_ABC");
		/// </code>
		/// 'filePath' will now be "C:\\Temp\\MyFile_ABC.txt".
		/// </example>
		/// <param name="filePath">Path to the file.</param>
		/// <param name="fileNamePostfix">Postfix that shall be appended to the file name.</param>
		/// <returns>The resulting file path.</returns>
		public static string AppendPostfixToFileName(string filePath, string fileNamePostfix)
		{
			string extension = Path.GetExtension(filePath);
			string filePathWithoutExtension = filePath.Substring(0, (filePath.Length - extension.Length));
			return (filePathWithoutExtension + fileNamePostfix + extension);
		}

		#endregion

		#region Compare...()

		/// <summary>
		/// Compares <paramref name="directoryPath2"/> relative to <paramref name="directoryPath1"/>
		/// and returns relative path of <paramref name="directoryPath2"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPath2"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareDirectoryPaths(string directoryPath1, string directoryPath2)
		{
			if (!Path.IsPathRooted(directoryPath1))
				return (new PathCompareResult(false));

			if (!Path.IsPathRooted(directoryPath2))
				return (new PathCompareResult(false, directoryPath2));

			return (DoCompareDirectoryPaths(directoryPath1, directoryPath2));
		}

		/// <summary>
		/// Compares <paramref name="filePath"/> relative to <paramref name="directoryPath"/> and
		/// returns relative path of file.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePath"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareDirectoryAndFilePaths(string directoryPath, string filePath)
		{
			if (!Path.IsPathRooted(directoryPath))
				return (new PathCompareResult(false));

			if (!Path.IsPathRooted(filePath))
				return (new PathCompareResult(false, filePath));

			string fileName = Path.GetFileName(filePath);
			PathCompareResult pcr = DoCompareDirectoryPaths(directoryPath, Path.GetDirectoryName(filePath));
			pcr.RelativePath += (Path.DirectorySeparatorChar + fileName);
			return (pcr);
		}

		/// <summary>
		/// Compares <paramref name="directoryPath"/> relative to <paramref name="filePath"/> and
		/// returns relative path of <paramref name="directoryPath"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPath"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareFileAndDirectoryPaths(string filePath, string directoryPath)
		{
			if (!Path.IsPathRooted(filePath))
				return (new PathCompareResult(false));

			if (!Path.IsPathRooted(directoryPath))
				return (new PathCompareResult(false, directoryPath));

			return (DoCompareDirectoryPaths(Path.GetDirectoryName(filePath), directoryPath));
		}

		/// <summary>
		/// Compares <paramref name="filePath2"/> relative to <paramref name="filePath1"/> and
		/// returns relative path of file2.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePath2"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareFilePaths(string filePath1, string filePath2)
		{
			if (!Path.IsPathRooted(filePath1))
				return (new PathCompareResult(false));

			if (!Path.IsPathRooted(filePath2))
				return (new PathCompareResult(false, filePath2));

			string fileName2 = Path.GetFileName(filePath2);
			PathCompareResult pcr = DoCompareDirectoryPaths(Path.GetDirectoryName(filePath1), Path.GetDirectoryName(filePath2));
			pcr.RelativePath += (Path.DirectorySeparatorChar + fileName2);
			return (pcr);
		}

		#endregion

		#region Combine...()

		/// <summary>
		/// Resolves <paramref name="directoryPath2"/> relative to <paramref name="directoryPath1"/>
		/// and returns absolute path of <paramref name="directoryPath2"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPath2"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineDirectoryPaths(string directoryPath1, string directoryPath2)
		{
			if (Path.IsPathRooted(directoryPath2))
				return (directoryPath2);

			if (!Path.IsPathRooted(directoryPath1))
				return (directoryPath1);

			if (string.IsNullOrEmpty(directoryPath1) || string.IsNullOrEmpty(directoryPath2))
				return (directoryPath1);

			return (DoCombineDirectoryPaths(directoryPath1, directoryPath2));
		}

		/// <summary>
		/// Resolves <paramref name="filePath"/> relative to <paramref name="directoryPath"/> and
		/// returns absolute path of file.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePath"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineDirectoryAndFilePaths(string directoryPath, string filePath)
		{
			if (Path.IsPathRooted(filePath))
				return (filePath);

			if (!Path.IsPathRooted(directoryPath))
				return (directoryPath);

			if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(filePath))
				return (directoryPath);

			string fileName = Path.GetFileName(filePath);
			string absolutePath = DoCombineDirectoryPaths(directoryPath, Path.GetDirectoryName(filePath));

			string combined = Path.Combine(absolutePath, fileName);
			return (combined);
		}

		/// <summary>
		/// Resolves <paramref name="directoryPath"/> relative to <paramref name="filePath"/> and
		/// returns absolute path of <paramref name="directoryPath"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPath"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineFileAndDirectoryPaths(string filePath, string directoryPath)
		{
			if (Path.IsPathRooted(directoryPath))
				return (directoryPath);

			if (!Path.IsPathRooted(filePath))
				return (filePath);

			if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(directoryPath))
				return (filePath);

			return (DoCombineDirectoryPaths(Path.GetDirectoryName(filePath), directoryPath));
		}

		/// <summary>
		/// Resolves <paramref name="filePath2"/> relative to <paramref name="filePath1"/> and
		/// returns absolute path of file2.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePath2"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineFilePaths(string filePath1, string filePath2)
		{
			if (Path.IsPathRooted(filePath2))
				return (filePath2);

			if (!Path.IsPathRooted(filePath1))
				return (filePath1);

			if (string.IsNullOrEmpty(filePath1) || string.IsNullOrEmpty(filePath2))
				return (filePath1);

			string fileName2 = Path.GetFileName(filePath2);
			string absolutePath = DoCombineDirectoryPaths(Path.GetDirectoryName(filePath1), Path.GetDirectoryName(filePath2));

			string combined = Path.Combine(absolutePath, fileName2);
			return (combined);
		}

		#endregion

		#region DoCompareDirectoryPaths()

		/// <summary>
		/// Returns relation between the two absolute directory paths.
		/// </summary>
		public static PathCompareResult DoCompareDirectoryPaths(string pathA, string pathB)
		{
			// Do not check for reference equality because complete result needs to be retrieved anyway.

			// Convert paths to platform if needed.
			pathA = ConvertToPlatform(pathA);
			pathB = ConvertToPlatform(pathB);

			// Create infos.
			DirectoryInfo pathInfoA = null;
			DirectoryInfo pathInfoB = null;

			string dirPathA = "";
			string dirPathB = "";
			DirectoryInfo dirInfoA = null;
			DirectoryInfo dirInfoB = null;

			DoPrepareDirectoryPath(pathA, out pathInfoA, out dirPathA, out dirInfoA);
			DoPrepareDirectoryPath(pathB, out pathInfoB, out dirPathB, out dirInfoB);

			if ((dirInfoA != null) && (dirInfoB != null))
			{
				// Check whether both directories share the same root.
				if (dirInfoA.Root.FullName != dirInfoB.Root.FullName)
					return (new PathCompareResult(false));

				// Get common directory, make sure only directory part is used.
				List<string> dirInfosA = new List<string>();
				List<string> dirInfosB = new List<string>();

				DirectoryInfo tempDirInfoA = dirInfoA;
				DirectoryInfo tempDirInfoB = dirInfoB;

				while (tempDirInfoA != null)
				{
					dirInfosA.Add(tempDirInfoA.FullName);
					tempDirInfoA = tempDirInfoA.Parent;
				}
				while (tempDirInfoB != null)
				{
					dirInfosB.Add(tempDirInfoB.FullName);
					tempDirInfoB = tempDirInfoB.Parent;
				}

				// Reverse lists.
				dirInfosA.Reverse();
				dirInfosB.Reverse();

				// Get common directory, make sure only directory part is used.
				int i = 0;
				while ((dirInfosA.Count > i) && (dirInfosB.Count > i) &&
				       (StringEx.EqualsOrdinalIgnoreCase(dirInfosA[i], dirInfosB[i])))
				{
					i++;
				}

				int commonDirectoryCount = i;

				string commonPath = "";
				if (commonDirectoryCount > 0)
					commonPath = dirInfosA[commonDirectoryCount - 1];

				DirectoryInfo commonDI = new DirectoryInfo(commonPath);

				// Check whether both paths are equal.
				if (PathEx.Equals(dirPathA, dirPathB))
					return (new PathCompareResult(commonPath, commonDirectoryCount, 0, true, 0, "."));

				// Check whether one of the two is the others subdirectory.
				DirectoryInfo di = commonDI;
				StringBuilder relativePath = new StringBuilder();
				if (PathEx.Equals(commonPath, dirPathA))
				{
					int nearRelativeDirectoryCount = 0;
					di = dirInfoB;
					while ((di != null) && (di.FullName != commonPath))
					{
						nearRelativeDirectoryCount++;

						if (relativePath.Length > 0)       // Actually, stepping in is done by stepping out.
						{
							relativePath.Insert(0, Path.DirectorySeparatorChar);
							relativePath.Insert(0, di.Name);
						}
						else
						{
							relativePath.Append(di.Name);
						}

						di = di.Parent;
					}
					return (new PathCompareResult(commonPath, commonDirectoryCount, nearRelativeDirectoryCount, true, nearRelativeDirectoryCount, relativePath.ToString()));
				}
				if (PathEx.Equals(commonPath, dirPathB))
				{
					int nearRelativeDirectoryCount = 0;
					di = dirInfoA;
					while ((di != null) && (di.FullName != commonPath))
					{
						di = di.Parent;
						if (di != null)
						{
							nearRelativeDirectoryCount--;

							if (relativePath.Length > 0)
								relativePath.Append(Path.DirectorySeparatorChar);

							relativePath.Append("..");
						}
					}
					return (new PathCompareResult(commonPath, commonDirectoryCount, nearRelativeDirectoryCount, true, nearRelativeDirectoryCount, relativePath.ToString()));
				}

				// In case of far relation, first step out to common path, then step into path B.
				int farRelativeDirectoryCount = 0;
				di = dirInfoA;
				while ((di != null) && (di.FullName != commonPath)) // Step out to common path.
				{
					di = di.Parent;
					if (di != null)
					{
						farRelativeDirectoryCount++;

						if (relativePath.Length > 0)
							relativePath.Append(Path.DirectorySeparatorChar);

						relativePath.Append("..");
					}
				}
				int commonPartIndex = relativePath.Length;
				di = dirInfoB;
				while ((di != null) && (di.FullName != commonPath)) // Step into path B
				{
					farRelativeDirectoryCount++;

					if (relativePath.Length > commonPartIndex) // Actually, stepping in is done by stepping out.
					{
						relativePath.Insert(commonPartIndex, Path.DirectorySeparatorChar);
						relativePath.Insert(commonPartIndex + 1, di.Name);
					}
					else if (relativePath.Length > 0)
					{
						relativePath.Append(Path.DirectorySeparatorChar);
						relativePath.Append(di.Name);
					}
					else
					{
						relativePath.Append(di.Name);
					}

					di = di.Parent;
				}
				return (new PathCompareResult(commonPath, commonDirectoryCount, farRelativeDirectoryCount, relativePath.ToString()));
			}

			return (new PathCompareResult(false));
		}

		#endregion

		#region DoCombineDirectoryPaths()

		/// <summary>
		/// Takes the first directory path and combines it with the second directory
		/// path also taking "." and ".." into account.
		/// </summary>
		public static string DoCombineDirectoryPaths(string pathA, string pathB)
		{
			// Convert paths to platform if needed.
			pathA = ConvertToPlatform(pathA);
			pathB = ConvertToPlatform(pathB);

			// Create infos.
			DirectoryInfo pathInfoA = null;

			string dirPathA = "";
			DirectoryInfo dirInfoA = null;

			DoPrepareDirectoryPath(pathA, out pathInfoA, out dirPathA, out dirInfoA);

			if ((pathInfoA != null) && (pathB.Length > 0))
			{
				DirectoryInfo pathInfoResult = null;

				string dirPathResult = "";
				DirectoryInfo dirInfoResult = null;

				// Trim leading '\'.
				string s = pathB.TrimStart(Path.DirectorySeparatorChar);

				// Check whether relative path points to any parent directory.
				if ((s.Length >= 2) && (StringEx.EqualsOrdinalIgnoreCase(s.Substring(0, 2), "..")))
				{
					DirectoryInfo pathInfoParent = pathInfoA;

					do
					{
						// Detect invalidly long relative paths.
						if ((s.Length >= 3) && (StringEx.EqualsOrdinalIgnoreCase(s.Substring(0, 3), "...")))
							break;

						s = s.Remove(0, 2);
						pathInfoParent = pathInfoParent.Parent;

						// ".." or "..\".
						if ((s.Length == 0) || (PathEx.Equals(s, Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))))
						{
							return (pathInfoParent.FullName);
						}

						// "..\<.. or Path>".
						if ((s.Length >= 1) && (PathEx.Equals(s.Substring(0, 1), Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))))
							s = s.Remove(0, 1);
						else
							break;
					}
					while ((s.Length >= 2) && (PathEx.Equals(s.Substring(0, 2), "..")));

					if (pathInfoParent != null)
						DoPrepareDirectoryPath(Path.Combine(pathInfoParent.FullName, s), out pathInfoResult, out dirPathResult, out dirInfoResult);
				}

				// Check whether relative path points to current directory.
				else if ((s.Length >= 1) && (PathEx.Equals(s.Substring(0, 1), ".")))
				{
					s = s.Remove(0, 1);

					// "." or ".\".
					if ((s.Length == 0) || (PathEx.Equals(s, Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))))
					{
						return (dirPathA);
					}

					// ".\<Path>".
					if (PathEx.Equals(s.Substring(0, 1), Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
					{
						string combined = dirPathA + s.Substring(1);
						DoPrepareDirectoryPath(combined, out pathInfoResult, out dirPathResult, out dirInfoResult);
					}
				}

				// Use System.IO.Path.Combine() for the easy cases.
				else
				{
					string combined = Path.Combine(dirPathA, pathB);
					DoPrepareDirectoryPath(combined, out pathInfoResult, out dirPathResult, out dirInfoResult);
				}

				if (pathInfoResult != null)
					return (dirPathResult);
			}

			// In case the second path was invalid, return the the first if possible.
			if (pathInfoA != null)
				return (dirPathA);
			else
				return ("");
		}

		#endregion

		#region DoPrepareDirectoryPath()

		private static void DoPrepareDirectoryPath(string path, out DirectoryInfo pathInfo, out string dirPath, out DirectoryInfo dirInfo)
		{
			try
			{
				// DirectoryInfo throws if path contains invalid characters.
				pathInfo = new DirectoryInfo(path);
			}
			catch (ArgumentException ex)
			{
				Diagnostics.DebugEx.WriteException(typeof(PathEx), ex);
				pathInfo = null;
			}

			// Get directory and file name.
			if (pathInfo != null)
			{
				DirectoryInfo temp = new DirectoryInfo(path);

				// Make sure parent directory and directory name is properly returned.
				if (temp.Parent != null)
					dirPath = Path.Combine(temp.Parent.FullName, temp.Name);
				else
					dirPath = temp.Root.FullName;

				dirInfo = new DirectoryInfo(dirPath);
			}
			else
			{
				dirInfo = null;
				dirPath = "";
			}
		}

		#endregion

		#region DoPrepareFilePath()

		private static void DoPrepareFilePath(string path, out DirectoryInfo pathInfo, out string dirPath, out DirectoryInfo dirInfo, out string fileName)
		{
			try
			{
				// DirectoryInfo throws if path contains invalid characters.
				pathInfo = new DirectoryInfo(path);
			}
			catch (ArgumentException ex)
			{
				Diagnostics.DebugEx.WriteException(typeof(PathEx), ex);
				pathInfo = null;
			}

			// Get directory and file name.
			if (pathInfo != null)
			{
				dirPath = Path.GetDirectoryName(path);
				dirInfo = new DirectoryInfo(dirPath);

				fileName = Path.GetFileName(path);
			}
			else
			{
				dirInfo = null;
				dirPath = "";

				fileName = "";
			}
		}

		#endregion

		#region Swap()

		/// <summary>
		/// Swaps two existing files.
		/// </summary>
		public static bool SwapExistingFiles(string filePathA, string filePathB)
		{
			if (!File.Exists(filePathA))
				return (false);

			if (!File.Exists(filePathB))
				return (false);

			// Both files exist, swap them:
			string filePathTemp = FileEx.MakeUniqueFileName(filePathA);
			File.Move(filePathA, filePathTemp);
			File.Move(filePathB, filePathA);
			File.Move(filePathTemp, filePathB);
			return (true);
		}

		#endregion
	}

	#region PathCompareResult

	/// <summary>
	/// Structure to hold the complete result of a directory comparison.
	/// </summary>
	public struct PathCompareResult
	{
		/// <summary>True if directories share a common path, i.e. also a common root.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public bool HaveCommon;

		/// <summary>Common path, e.g. "C:\MyDir".</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public string CommonPath;

		/// <summary>Number of common directories, e.g. "C:\MyDir" results in 1.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public int CommonDirectoryCount;

		/// <summary>True if directories are relative, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2".</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public bool AreRelative;

		/// <summary>Number of relative directories, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2" results in 2.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public int RelativeDirectoryCount;

		/// <summary>True if directories are near relative, e.g. "C:\MyDir" and "C:\MyDir\MySubDir".</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public bool AreNearRelative;

		/// <summary>Number of near relative directories, e.g. "C:\MyDir" and "C:\MyDir\MySubDir" results in 1.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public int NearRelativeDirectoryCount;

		/// <summary>Relative path between the two.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public string RelativePath;

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(bool haveCommon)
			: this(haveCommon, "")
		{
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(bool haveCommon, string relativePath)
		{
			HaveCommon = haveCommon;
			CommonPath = null;
			CommonDirectoryCount = 0;
			AreRelative = false;
			RelativeDirectoryCount = 0;
			AreNearRelative = false;
			NearRelativeDirectoryCount = 0;
			RelativePath = relativePath;
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(string commonPath, int commonDirectoryCount, int relativeDirectoryCount, string relativePath)
			: this(commonPath, commonDirectoryCount, relativeDirectoryCount, false, 0, relativePath)
		{
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(string commonPath, int commonDirectoryCount, int relativeDirectoryCount, bool areNearRelative, int nearRelativeDirectoryCount, string relativePath)
		{
			HaveCommon = true;
			CommonPath = commonPath;
			CommonDirectoryCount = commonDirectoryCount;
			AreRelative = true;
			RelativeDirectoryCount = relativeDirectoryCount;
			AreNearRelative = areNearRelative;
			NearRelativeDirectoryCount = nearRelativeDirectoryCount;
			RelativePath = relativePath;
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			PathCompareResult other = (PathCompareResult)obj;
			return
			(
				(HaveCommon                 == other.HaveCommon) &&
				PathEx.Equals(CommonPath,      other.CommonPath) &&
				(CommonDirectoryCount       == other.CommonDirectoryCount) &&
				(AreRelative                == other.AreRelative) &&
				(RelativeDirectoryCount     == other.RelativeDirectoryCount) &&
				(AreNearRelative            == other.AreNearRelative) &&
				(NearRelativeDirectoryCount == other.NearRelativeDirectoryCount) &&
				PathEx.Equals(RelativePath,    other.RelativePath)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				HaveCommon                .GetHashCode() ^
				CommonPath                .GetHashCode() ^
				CommonDirectoryCount      .GetHashCode() ^
				AreRelative               .GetHashCode() ^
				RelativeDirectoryCount    .GetHashCode() ^
				AreNearRelative           .GetHashCode() ^
				NearRelativeDirectoryCount.GetHashCode() ^
				RelativePath              .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PathCompareResult lhs, PathCompareResult rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(PathCompareResult lhs, PathCompareResult rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
