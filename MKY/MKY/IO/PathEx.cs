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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.Path"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class PathEx
	{
		#region Platform

		/// <summary>
		/// The comparer dependent on the operating systems policies.
		/// </summary>
		public static StringComparer Comparer
		{
			get
			{
				if (EnvironmentEx.IsWindows)
					return (StringComparer.OrdinalIgnoreCase);
				else
					return (StringComparer.Ordinal);
			}
		}

		/// <summary>
		/// The comparison type dependent on the operating systems policies.
		/// </summary>
		public static StringComparison ComparisonType
		{
			get
			{
				if (EnvironmentEx.IsWindows)
					return (StringComparison.OrdinalIgnoreCase);
				else
					return (StringComparison.Ordinal);
			}
		}

		/// <summary>
		/// Convert non-platform separators according to platform.
		/// </summary>
		public static string ConvertToPlatform(string path)
		{
			// e.g. replace '/' by '\'
			return (path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
		}

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

		#region Equals()

		/// <summary>
		/// Compares two specified path string dependent on the operating systems policies.
		/// </summary>
		public static bool Equals(string pathA, string pathB)
		{
			return (string.Compare(pathA, pathB, ComparisonType) == 0);
		}

		#endregion

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
		/// Returns whether the directory or file path is valid, expanding environment variables.
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
			return (!string.IsNullOrEmpty(Path.GetFullPath(Environment.ExpandEnvironmentVariables(path))));
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Denormalize' is a correct English term.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Denormalize", Justification = "'Denormalize' is a correct English term.")]
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

		#region Limit...()

		/// <summary>
		/// Limits a directory or file path to the specified max length.
		/// </summary>
		/// <remarks>
		/// This function does not expand environment variables.
		/// </remarks>
		public static string Limit(string path, int length)
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
		/// <remarks>
		/// This function does not expand environment variables.
		/// </remarks>
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
		/// Compares <paramref name="directoryPathB"/> relative to <paramref name="directoryPathA"/>
		/// and returns relative path of <paramref name="directoryPathB"/>, expanding environment variables.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPathB"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareDirectoryPaths(string directoryPathA, string directoryPathB)
		{
			directoryPathA = Environment.ExpandEnvironmentVariables(directoryPathA);
			directoryPathB = Environment.ExpandEnvironmentVariables(directoryPathB);

			if (!Path.IsPathRooted(directoryPathA))
				return (new PathCompareResult(false));

			if (!Path.IsPathRooted(directoryPathB))
				return (new PathCompareResult(false, directoryPathB));

			return (DoCompareDirectoryPaths(directoryPathA, directoryPathB));
		}

		/// <summary>
		/// Compares <paramref name="filePath"/> relative to <paramref name="directoryPath"/> and
		/// returns relative path of file, expanding environment variables.
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
			directoryPath = Environment.ExpandEnvironmentVariables(directoryPath);
			filePath      = Environment.ExpandEnvironmentVariables(filePath);

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
		/// returns relative path of <paramref name="directoryPath"/>, expanding environment variables.
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
			filePath      = Environment.ExpandEnvironmentVariables(filePath);
			directoryPath = Environment.ExpandEnvironmentVariables(directoryPath);

			if (!Path.IsPathRooted(filePath))
				return (new PathCompareResult(false));

			if (!Path.IsPathRooted(directoryPath))
				return (new PathCompareResult(false, directoryPath));

			return (DoCompareDirectoryPaths(Path.GetDirectoryName(filePath), directoryPath));
		}

		/// <summary>
		/// Compares <paramref name="filePathB"/> relative to <paramref name="filePathA"/> and
		/// returns relative path of file2, expanding environment variables.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePathB"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareFilePaths(string filePathA, string filePathB)
		{
			filePathA = Environment.ExpandEnvironmentVariables(filePathA);
			filePathB = Environment.ExpandEnvironmentVariables(filePathB);

			if (!Path.IsPathRooted(filePathA))
				return (new PathCompareResult(false));

			if (!Path.IsPathRooted(filePathB))
				return (new PathCompareResult(false, filePathB));

			string fileName2 = Path.GetFileName(filePathB);
			PathCompareResult pcr = DoCompareDirectoryPaths(Path.GetDirectoryName(filePathA), Path.GetDirectoryName(filePathB));
			pcr.RelativePath += (Path.DirectorySeparatorChar + fileName2);
			return (pcr);
		}

		#endregion

		#region Combine...()

		/// <summary>
		/// Resolves <paramref name="directoryPathB"/> relative to <paramref name="directoryPathA"/>
		/// and returns absolute path of <paramref name="directoryPathB"/>, expanding environment variables.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPathB"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineDirectoryPaths(string directoryPathA, string directoryPathB)
		{
			directoryPathA = Environment.ExpandEnvironmentVariables(directoryPathA);
			directoryPathB = Environment.ExpandEnvironmentVariables(directoryPathB);

			if (Path.IsPathRooted(directoryPathB))
				return (directoryPathB);

			if (!Path.IsPathRooted(directoryPathA))
				return (null);

			if (string.IsNullOrEmpty(directoryPathB))
				return (directoryPathA);

			return (DoCombineDirectoryPaths(directoryPathA, directoryPathB));
		}

		/// <summary>
		/// Resolves <paramref name="filePath"/> relative to <paramref name="directoryPath"/> and
		/// returns absolute path of file, expanding environment variables.
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
			directoryPath = Environment.ExpandEnvironmentVariables(directoryPath);
			filePath      = Environment.ExpandEnvironmentVariables(filePath);

			if (Path.IsPathRooted(filePath))
				return (filePath);

			if (!Path.IsPathRooted(directoryPath))
				return (null);

			if (string.IsNullOrEmpty(filePath))
				return (directoryPath);

			string fileName = Path.GetFileName(filePath);
			string absolutePath = DoCombineDirectoryPaths(directoryPath, Path.GetDirectoryName(filePath));

			string combined = Path.Combine(absolutePath, fileName);
			return (combined);
		}

		/// <summary>
		/// Resolves <paramref name="directoryPath"/> relative to <paramref name="filePath"/> and
		/// returns absolute path of <paramref name="directoryPath"/>, expanding environment variables.
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
			filePath      = Environment.ExpandEnvironmentVariables(filePath);
			directoryPath = Environment.ExpandEnvironmentVariables(directoryPath);

			if (Path.IsPathRooted(directoryPath))
				return (directoryPath);

			if (!Path.IsPathRooted(filePath))
				return (null);

			if (string.IsNullOrEmpty(directoryPath))
				return (filePath);

			return (DoCombineDirectoryPaths(Path.GetDirectoryName(filePath), directoryPath));
		}

		/// <summary>
		/// Resolves <paramref name="filePathB"/> relative to <paramref name="filePathA"/> and
		/// returns absolute path of file2, expanding environment variables.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePathB"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineFilePaths(string filePathA, string filePathB)
		{
			filePathA = Environment.ExpandEnvironmentVariables(filePathA);
			filePathB = Environment.ExpandEnvironmentVariables(filePathB);

			if (Path.IsPathRooted(filePathB))
				return (filePathB);

			if (!Path.IsPathRooted(filePathA))
				return (null);

			if (string.IsNullOrEmpty(filePathB))
				return (filePathA);

			string fileName2 = Path.GetFileName(filePathB);
			string absolutePath = DoCombineDirectoryPaths(Path.GetDirectoryName(filePathA), Path.GetDirectoryName(filePathB));

			string combined = Path.Combine(absolutePath, fileName2);
			return (combined);
		}

		#endregion

		#region DoCompareDirectoryPaths()

		/// <summary>
		/// Returns relation between the two absolute directory paths.
		/// </summary>
		/// <remarks>
		/// This function does not expand environment variables.
		/// </remarks>
		private static PathCompareResult DoCompareDirectoryPaths(string pathA, string pathB)
		{
			// Do not check for reference equality because complete result needs to be retrieved anyway.

			// Convert paths to platform if needed:
			pathA = ConvertToPlatform(pathA);
			pathB = ConvertToPlatform(pathB);

			// Create infos:
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
				// Check whether both directories share the same root:
				if (dirInfoA.Root.FullName != dirInfoB.Root.FullName)
					return (new PathCompareResult(false));

				// Get common directory, make sure only directory part is used:
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

				// Reverse lists:
				dirInfosA.Reverse();
				dirInfosB.Reverse();

				// Get common directory, make sure only directory part is used:
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

				// Check whether both paths are equal:
				if (Equals(dirPathA, dirPathB))
					return (new PathCompareResult(commonPath, commonDirectoryCount, 0, true, 0, "."));

				// Check whether one of the two is the others subdirectory:
				DirectoryInfo di = commonDI;
				StringBuilder relativePath = new StringBuilder();
				if (Equals(commonPath, dirPathA))
				{
					int nearRelativeDirectoryCount = 0;
					di = dirInfoB;
					while ((di != null) && (di.FullName != commonPath))
					{
						nearRelativeDirectoryCount++;

						if (relativePath.Length > 0) // Actually, stepping in is done by stepping out.
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

				if (Equals(commonPath, dirPathB))
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

				// In case of far relation, first step out to common path, then step into path B:
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
		/// <remarks>
		/// This function does not expand environment variables.
		/// </remarks>
		private static string DoCombineDirectoryPaths(string pathA, string pathB)
		{
			// Convert paths to platform if needed:
			pathA = ConvertToPlatform(pathA);
			pathB = ConvertToPlatform(pathB);

			// Create infos:
			DirectoryInfo pathInfoA = null;

			string dirPathA = "";
			DirectoryInfo dirInfoA = null;

			DoPrepareDirectoryPath(pathA, out pathInfoA, out dirPathA, out dirInfoA);

			if ((pathInfoA != null) && (!string.IsNullOrEmpty(pathB)))
			{
				DirectoryInfo pathInfoResult = null;

				string dirPathResult = "";
				DirectoryInfo dirInfoResult = null;

				// Trim leading '\':
				string s = pathB.TrimStart(Path.DirectorySeparatorChar);

				// Check whether relative path points to any parent directory:
				if ((s.Length >= 2) && (StringEx.EqualsOrdinalIgnoreCase(s.Substring(0, 2), "..")))
				{
					DirectoryInfo pathInfoParent = pathInfoA;

					do
					{
						// Detect invalidly long relative paths:
						if ((s.Length >= 3) && (StringEx.EqualsOrdinalIgnoreCase(s.Substring(0, 3), "...")))
							break;

						s = s.Remove(0, 2);
						pathInfoParent = pathInfoParent.Parent;

						// ".." or "..\":
						if ((s.Length == 0) || (PathEx.Equals(s, Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))))
						{
							return (pathInfoParent.FullName);
						}

						// "..\<.. or Path>":
						if ((s.Length >= 1) && (PathEx.Equals(s.Substring(0, 1), Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))))
							s = s.Remove(0, 1);
						else
							break;
					}
					while ((s.Length >= 2) && (PathEx.Equals(s.Substring(0, 2), "..")));

					if (pathInfoParent != null)
						DoPrepareDirectoryPath(Path.Combine(pathInfoParent.FullName, s), out pathInfoResult, out dirPathResult, out dirInfoResult);
				}

				// Check whether relative path points to current directory:
				else if ((s.Length >= 1) && (PathEx.Equals(s.Substring(0, 1), ".")))
				{
					s = s.Remove(0, 1);

					// "." or ".\":
					if ((s.Length == 0) || (PathEx.Equals(s, Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))))
					{
						return (dirPathA);
					}

					// ".\<Path>":
					if (PathEx.Equals(s.Substring(0, 1), Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
					{
						string combined = dirPathA + s.Substring(1);
						DoPrepareDirectoryPath(combined, out pathInfoResult, out dirPathResult, out dirInfoResult);
					}
				}

				// Use System.IO.Path.Combine() for the easy cases:
				else
				{
					string combined = Path.Combine(dirPathA, pathB);
					DoPrepareDirectoryPath(combined, out pathInfoResult, out dirPathResult, out dirInfoResult);
				}

				if (pathInfoResult != null)
					return (dirPathResult);
			}

			// In case the second path was invalid, return the the first if possible:
			if (pathInfoA != null)
				return (dirPathA);
			else
				return ("");
		}

		#endregion

		#region DoPrepareDirectoryPath()

		/// <remarks>
		/// This function does not expand environment variables.
		/// </remarks>
		private static void DoPrepareDirectoryPath(string path, out DirectoryInfo pathInfo, out string dirPath, out DirectoryInfo dirInfo)
		{
			try
			{
				// Throws if path contains invalid characters:
				pathInfo = new DirectoryInfo(path);
			}
			catch (ArgumentException ex)
			{
				Diagnostics.DebugEx.WriteException(typeof(PathEx), ex, "Path contains invalid characters!");
				pathInfo = null;
			}

			// Get directory and file name:
			if (pathInfo != null)
			{
				DirectoryInfo temp = new DirectoryInfo(path);

				// Make sure parent directory and directory name is properly returned:
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

		/// <remarks>
		/// This function does not expand environment variables.
		/// </remarks>
		private static void DoPrepareFilePath(string path, out DirectoryInfo pathInfo, out string dirPath, out DirectoryInfo dirInfo, out string fileName)
		{
			try
			{
				// Throws if path contains invalid characters:
				pathInfo = new DirectoryInfo(path);
			}
			catch (ArgumentException ex)
			{
				Diagnostics.DebugEx.WriteException(typeof(PathEx), ex, "Path contains invalid characters!");
				pathInfo = null;
			}

			// Get directory and file name:
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

		#region Swap...()

		/// <summary>
		/// Swaps two existing files, expanding environment variables.
		/// </summary>
		public static bool SwapExistingFiles(string filePathA, string filePathB)
		{
			filePathA = Environment.ExpandEnvironmentVariables(filePathA);
			filePathB = Environment.ExpandEnvironmentVariables(filePathB);

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

		#region Distinct...()

		/// <summary>
		/// Retrieves the distinct (no duplicate) paths among all given paths, expanding environment variables.
		/// </summary>
		public static IEnumerable<string> Distinct(params IEnumerable<string>[] pathArgs)
		{
			int capacity = 0;
			foreach (IEnumerable<string> paths in pathArgs)
			{
				capacity += paths.Count();
			}

			var l = new List<string>(capacity); // Preset the initial capacity to improve memory management.
			foreach (IEnumerable<string> paths in pathArgs)
			{
				foreach (string path in paths)
					l.Add(Environment.ExpandEnvironmentVariables(path));
			}
			return (l.Distinct(Comparer));
		}

		/// <summary>
		/// Retrieves the distinct (no duplicate) directories of the given paths, expanding environment variables.
		/// </summary>
		public static IEnumerable<string> DistinctDirectories(IEnumerable<string> paths)
		{
			var directories = new List<string>(paths.Count()); // Preset the initial capacity to improve memory management.

			foreach (string path in paths)
				directories.Add(Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(path)));

			return (directories.Distinct(Comparer));
		}

		/// <summary>
		/// Retrieves the distinct (no duplicate) directories of the given paths, expanding environment variables.
		/// </summary>
		/// <remarks>
		/// Code duplication of <see cref="DistinctDirectories(IEnumerable{string})"/> above
		/// as <see cref="StringCollection"/> does not implement <see cref="IEnumerable{T}"/>.
		/// </remarks>
		public static IEnumerable<string> DistinctDirectories(StringCollection paths)
		{
			var directories = new List<string>(paths.Count); // Preset the initial capacity to improve memory management.

			foreach (string path in paths)
				directories.Add(Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(path)));

			return (directories.Distinct(Comparer));
		}

		#endregion
	}

	#region PathCompareResult

	/// <summary>
	/// Structure to hold the complete result of a directory comparison.
	/// </summary>
	public struct PathCompareResult : IEquatable<PathCompareResult>
	{
		/// <summary>True if directories share a common path, i.e. also a common root.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public bool HaveCommon;

		/// <summary>Common path, e.g. "C:\MyDir".</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public string CommonPath;

		/// <summary>Number of common directories, e.g. "C:\MyDir" results in 1.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public int CommonDirectoryCount;

		/// <summary>True if directories are relative, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2".</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public bool AreRelative;

		/// <summary>Number of relative directories, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2" results in 2.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public int RelativeDirectoryCount;

		/// <summary>True if directories are near relative, e.g. "C:\MyDir" and "C:\MyDir\MySubDir".</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		public bool AreNearRelative;

		/// <summary>Number of near relative directories, e.g. "C:\MyDir" and "C:\MyDir\MySubDir" results in 1.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
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
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

				hashCode =                     HaveCommon                         .GetHashCode();
				hashCode = (hashCode * 397) ^ (CommonPath   != null ? CommonPath  .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  CommonDirectoryCount;
				hashCode = (hashCode * 397) ^  AreRelative                        .GetHashCode();
				hashCode = (hashCode * 397) ^  RelativeDirectoryCount;
				hashCode = (hashCode * 397) ^  AreNearRelative                    .GetHashCode();
				hashCode = (hashCode * 397) ^  NearRelativeDirectoryCount;
				hashCode = (hashCode * 397) ^ (RelativePath != null ? RelativePath.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is PathCompareResult)
				return (Equals((PathCompareResult)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(PathCompareResult other)
		{
			return
			(
				HaveCommon                .Equals(other.HaveCommon)                 &&
				PathEx.Equals(CommonPath,         other.CommonPath)                 &&
				CommonDirectoryCount      .Equals(other.CommonDirectoryCount)       &&
				AreRelative               .Equals(other.AreRelative)                &&
				RelativeDirectoryCount    .Equals(other.RelativeDirectoryCount)     &&
				AreNearRelative           .Equals(other.AreNearRelative)            &&
				NearRelativeDirectoryCount.Equals(other.NearRelativeDirectoryCount) &&
				PathEx.Equals(RelativePath,       other.RelativePath)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PathCompareResult lhs, PathCompareResult rhs)
		{
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
