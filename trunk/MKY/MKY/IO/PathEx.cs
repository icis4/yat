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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="Path"/>.
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
		/// Compares two specified path string, dependent on the operating systems policies.
		/// </summary>
		public static bool Equals(string pathA, string pathB)
		{
			return (string.Compare(pathA, pathB, ComparisonType) == 0); // string.Compare() accepts 'null' args.
		}

		/// <summary>
		/// Compares whether <paramref name="pathA"/> matches any <paramref name="pathB"/>,
		/// dependent on the operating systems policies.
		/// </summary>
		public static bool EqualsAny(string pathA, params string[] pathB)
		{
			return (EqualsAny(pathA, (IEnumerable<string>)pathB));
		}

		/// <summary>
		/// Compares whether <paramref name="pathA"/> matches any <paramref name="pathB"/>,
		/// dependent on the operating systems policies.
		/// </summary>
		public static bool EqualsAny(string pathA, IEnumerable<string> pathB)
		{
			foreach (string p in pathB)
			{
				if (Equals(pathA, p))
					return (true); // Match.
			}

			return (false); // No match.
		}

		#endregion

		#region Contains...()
		//------------------------------------------------------------------------------------------
		// Contains...()
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether <paramref name="path"/> matches one of the specified <paramref name="values"/>,
		/// dependent on the operating systems policies.
		/// </summary>
		/// <param name="path">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="path"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException">value is null.</exception>
		public static bool ContainsAny(string path, params string[] values)
		{
			return (ContainsAny(path, (IEnumerable<string>)values));
		}

		/// <summary>
		/// Determines whether <paramref name="path"/> matches one of the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="path">The string.</param>
		/// <param name="values">The strings to compare with.</param>
		/// <returns>true if <paramref name="path"/> matches the beginning of a comparing string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException">value is null.</exception>
		public static bool ContainsAny(string path, IEnumerable<string> values)
		{
			if (EnvironmentEx.IsWindows) // OrdinalIgnoreCase.
			{
				foreach (string v in values)
				{
					if (path.ToUpperInvariant().Contains(v.ToUpperInvariant()))
						return (true); // Match.
				}
			}
			else
			{
				foreach (string v in values)
				{
					if (path.Contains(v))
						return (true); // Match.
				}
			}

			return (false); // No match.
		}

		#endregion

		#region IsContained()
		//------------------------------------------------------------------------------------------
		// IsContained()
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns whether <paramref name="str"/> contains the given <paramref name="path"/>. The
		/// comparison is done in a platform independent way, i.e. any path designation is accepted.
		/// </summary>
		/// <remarks>
		/// Named 'IsContained' instead of 'Contains' as <paramref name="str"/> can be any string,
		/// not just a path string.
		/// </remarks>
		public static bool IsContainedInvariant(string str, string path)
		{
			if (string.IsNullOrEmpty(path))
				return (false);

			// First try without converting anything:
			if (str.IndexOf(path, ComparisonType) >= 0) // Using string.IndexOf() because string.Contains()
				return (true);                          // does not allow controlling culture and case.

			// No success, try to convert 'str' (as 'path' is more likely platform-correct):
			var strToPlatform = ConvertToPlatform(str);
			if (strToPlatform.IndexOf(path, ComparisonType) >= 0)
				return (true);

			// Still no success, try to convert 'path' instead:
			var pathToPlatform = ConvertToPlatform(path);
			if (str.IndexOf(pathToPlatform, ComparisonType) >= 0)
				return (true);

			// Last attempt, both converted:
			if (strToPlatform.IndexOf(pathToPlatform, ComparisonType) >= 0)
				return (true);

			return (false);
		}

		#endregion

		#region Is...()

		/// <summary>
		/// Returns whether the directory or file path is valid.
		/// </summary>
		/// <remarks>
		/// This method only checks whether the path is valid but not whether it actually exists.
		/// Use <see cref="Directory.Exists"/> or <see cref="File.Exists"/> to check existence.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for validation.")]
		public static bool IsValid(string path)
		{
			// String validation:
			if (string.IsNullOrEmpty(path))
			{
				return (false);
			}

			// Path validation:
			try
			{
				var fullPath = Path.GetFullPath(path);
				return (!string.IsNullOrEmpty(fullPath));
			}
			catch // Path.GetFullPath() throws if path is invalid.
			{
				return (false);
			}
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

		#region Limit()

		/// <summary>
		/// Limits a directory or file path to the specified max length. If the path exceed the
		/// max length, <see cref="StringEx.Ellipsis"/> are added before the last part of the path
		/// (same behavior as <see cref="System.Windows.Forms.TextFormatFlags.PathEllipsis"/>).
		/// </summary>
		/// <remarks>
		/// This function does not expand environment variables.
		/// </remarks>
		public static string Limit(string path, int length)
		{
			if (string.IsNullOrEmpty(path))            // Path string not valid at all ?
				return (path);

			if (path.Length <= length)                 // Path string too long ?
				return (path);

			string limitedPath;
			if (path.IndexOf(Path.VolumeSeparatorChar) < 0)
			{                                          // Local path ?
				limitedPath = StringEx.Left(path, 3) + StringEx.Ellipsis +
				              StringEx.Right(path, Math.Max(length - 6, 0));
			}
			else                                       // Network path !
			{
				int separatorPosition = path.Substring(4).IndexOf(Path.DirectorySeparatorChar);
				if ((separatorPosition >= 0) && (separatorPosition < length - 4))
				{
					limitedPath = StringEx.Left(path, separatorPosition) + StringEx.Ellipsis +
					              StringEx.Right(path, Math.Max(length - 4 - separatorPosition, 0));
				}
				else
				{
					limitedPath = StringEx.Left(path, 5) + StringEx.Ellipsis +
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
		/// <paramref name="filePath"/> will now be "C:\\Temp\\MyFile_ABC.txt".
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
		/// and returns relative path of <paramref name="directoryPathB"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPathB"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareDirectoryPaths(string directoryPathA, string directoryPathB)
		{
			if (!Path.IsPathRooted(directoryPathA)) return (new PathCompareResult(false));
			if (!Path.IsPathRooted(directoryPathB)) return (new PathCompareResult(false, directoryPathB));

			return (DoCompareDirectoryPaths(directoryPathA, directoryPathB));
		}

		/// <summary>
		/// Compares <paramref name="filePath"/> relative to <paramref name="directoryPath"/> and
		/// returns relative path of file.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePath"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareDirectoryAndFilePaths(string directoryPath, string filePath)
		{
			if (!Path.IsPathRooted(directoryPath)) return (new PathCompareResult(false));
			if (!Path.IsPathRooted(filePath))      return (new PathCompareResult(false, filePath));

			string fileName = Path.GetFileName(filePath);
			PathCompareResult pcr = DoCompareDirectoryPaths(directoryPath, GetDirectoryPath(filePath));
			pcr.RelativePath += (Path.DirectorySeparatorChar + fileName);
			return (pcr);
		}

		/// <summary>
		/// Compares <paramref name="directoryPath"/> relative to <paramref name="filePath"/> and
		/// returns relative path of <paramref name="directoryPath"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPath"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareFileAndDirectoryPaths(string filePath, string directoryPath)
		{
			if (!Path.IsPathRooted(filePath))      return (new PathCompareResult(false));
			if (!Path.IsPathRooted(directoryPath)) return (new PathCompareResult(false, directoryPath));

			return (DoCompareDirectoryPaths(GetDirectoryPath(filePath), directoryPath));
		}

		/// <summary>
		/// Compares <paramref name="filePathB"/> relative to <paramref name="filePathA"/> and
		/// returns relative path of fileB.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePathB"/> if it already is relative.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static PathCompareResult CompareFilePaths(string filePathA, string filePathB)
		{
			if (!Path.IsPathRooted(filePathA)) return (new PathCompareResult(false));
			if (!Path.IsPathRooted(filePathB)) return (new PathCompareResult(false, filePathB));

			PathCompareResult pcr = DoCompareDirectoryPaths(GetDirectoryPath(filePathA), GetDirectoryPath(filePathB));
			pcr.RelativePath += (Path.DirectorySeparatorChar + Path.GetFileName(filePathB));
			return (pcr);
		}

		#endregion

		#region Combine...()

		/// <summary>
		/// Resolves <paramref name="directoryPathB"/> relative to <paramref name="directoryPathA"/>
		/// and returns normalized absolute path of <paramref name="directoryPathB"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPathB"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineDirectoryPaths(string directoryPathA, string directoryPathB)
		{
			if ( Path.IsPathRooted(directoryPathB))   return (directoryPathB);
			if (!Path.IsPathRooted(directoryPathA))   return (null);

			if (string.IsNullOrEmpty(directoryPathB)) return (directoryPathA);

			return (DoCombineDirectoryPaths(directoryPathA, directoryPathB));
		}

		/// <summary>
		/// Resolves <paramref name="filePath"/> relative to <paramref name="directoryPath"/> and
		/// returns normalized absolute path of file.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePath"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineDirectoryAndFilePaths(string directoryPath, string filePath)
		{
			if ( Path.IsPathRooted(filePath))      return (filePath);
			if (!Path.IsPathRooted(directoryPath)) return (null);

			if (string.IsNullOrEmpty(filePath))    return (directoryPath);

			string fileName = Path.GetFileName(filePath);
			string absolutePath = DoCombineDirectoryPaths(directoryPath, GetDirectoryPath(filePath));
			string combined = Path.Combine(absolutePath, fileName);
			return (combined);
		}

		/// <summary>
		/// Resolves <paramref name="directoryPath"/> relative to <paramref name="filePath"/> and
		/// returns normalized absolute path of <paramref name="directoryPath"/>.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="directoryPath"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineFileAndDirectoryPaths(string filePath, string directoryPath)
		{
			if ( Path.IsPathRooted(directoryPath))   return (directoryPath);
			if (!Path.IsPathRooted(filePath))        return (null);

			if (string.IsNullOrEmpty(directoryPath)) return (filePath);

			return (DoCombineDirectoryPaths(GetDirectoryPath(filePath), directoryPath));
		}

		/// <summary>
		/// Resolves <paramref name="filePathB"/> relative to <paramref name="filePathA"/> and
		/// returns normalized absolute path of fileB.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="filePathB"/> if it is absolute.
		/// Why is this functionality not already provided by <see cref="Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public static string CombineFilePaths(string filePathA, string filePathB)
		{
			if ( Path.IsPathRooted(filePathB))   return (filePathB);
			if (!Path.IsPathRooted(filePathA))   return (null);

			if (string.IsNullOrEmpty(filePathB)) return (filePathA);

			string absolutePath = DoCombineDirectoryPaths(GetDirectoryPath(filePathA), GetDirectoryPath(filePathB));
			string combined = Path.Combine(absolutePath, Path.GetFileName(filePathB));
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
				var dirInfosA = new List<string>();
				var dirInfosB = new List<string>();

				var tempDirInfoA = dirInfoA;
				var tempDirInfoB = dirInfoB;

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

				var commonDI = new DirectoryInfo(commonPath);

				// Check whether both paths are equal:
				if (Equals(dirPathA, dirPathB))
					return (new PathCompareResult(commonPath, commonDirectoryCount, 0, true, 0, "."));

				// Check whether one of the two is the others subdirectory:
				var di = commonDI;
				var relativePath = new StringBuilder();
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
				dirPath = GetDirectoryPath(path);
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

		#region Distinct...()

		/// <summary>
		/// Retrieves the distinct (no duplicate) paths among all given paths.
		/// </summary>
		public static IEnumerable<string> Distinct(params IEnumerable<string>[] pathArgs)
		{
			var nonNullOrEmptyPaths = pathArgs.SelectMany(p => p).Where(p => !string.IsNullOrEmpty(p));
			return (Distinct(nonNullOrEmptyPaths));
		}

		/// <summary>
		/// Retrieves the distinct (no duplicate) paths among the given paths.
		/// </summary>
		public static IEnumerable<string> Distinct(IEnumerable<string> paths)
		{
			var nonNullOrEmptyPaths = paths.Select(p => p).Where(p => !string.IsNullOrEmpty(p));
			return (nonNullOrEmptyPaths.Distinct(Comparer));
		}

		/// <summary>
		/// Retrieves the distinct (no duplicate) directories among the given paths.
		/// </summary>
		public static IEnumerable<string> DistinctDirectories(StringCollection paths)
		{
			return (DistinctDirectories(paths.Cast<string>()));
		}

		/// <summary>
		/// Retrieves the distinct (no duplicate) directories among the given paths.
		/// </summary>
		public static IEnumerable<string> DistinctDirectories(IEnumerable<string> paths)
		{
			var nonNullOrEmptyPaths = paths.Select(p => p).Where(p => !string.IsNullOrEmpty(p));
			return (nonNullOrEmptyPaths.Select(p => GetDirectoryPath(p))
			                           .Distinct(Comparer));
		}

		#endregion

		#region Get...()

		/// <summary>
		/// Returns the directory information for the specified path string.
		/// </summary>
		/// <remarks>
		/// Same as <see cref="Path.GetDirectoryName"/>, but with the proper method name.
		/// </remarks>
		public static string GetDirectoryPath(string path)
		{
			return (Path.GetDirectoryName(path));
		}

		/// <summary>
		/// Returns the name of the given path.
		/// </summary>
		/// <remarks>
		/// Opposed to <see cref="Path.GetDirectoryName"/>, this method returns the directory
		/// name only, not the full directory path.
		/// </remarks>
		public static string GetDirectoryNameOnly(string path)
		{
			if (string.IsNullOrEmpty(path))
				return (null);

			if (File.Exists(path)) // Strip file name if path refers to a file.
				path = GetDirectoryPath(path);

			var di = new DirectoryInfo(path);
			return (di.Name);
		}

		/// <summary>
		/// Resolves the absolute location to the given file path and normalizes it, expanding environment variables.
		///  - If <see cref="Path.IsPathRooted"/>, simply expand environment variables.
		///  - Otherwise, expand environment variables and combine it with the <see cref="Environment.CurrentDirectory"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="filePath"/> is null.
		/// </exception>
		public static string GetNormalizedRootedExpandingEnvironmentVariables(string filePath)
		{
			return (GetNormalizedRootedExpandingEnvironmentVariables(Environment.CurrentDirectory, filePath));
		}

		/// <summary>
		/// Resolves the absolute location to the given file path and normalizes it, expanding environment variables.
		///  - If <see cref="Path.IsPathRooted"/>, simply expand environment variables.
		///  - Otherwise, expand environment variables and combine it with the given <paramref name="rootDirectory"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="filePath"/> is null.
		/// </exception>
		public static string GetNormalizedRootedExpandingEnvironmentVariables(string rootDirectory, string filePath)
		{
			if (Path.IsPathRooted(filePath))
				return (Path.GetFullPath(Environment.ExpandEnvironmentVariables(filePath)));
			else
				return (CombineDirectoryAndFilePaths(rootDirectory, Environment.ExpandEnvironmentVariables(filePath)));
		}

		#endregion

		#region TryGet...()

		/// <summary>
		/// Evaluates whether the given paths share a common parent and returns the common part.
		/// </summary>
		public static bool TryGetCommon(string pathA, string pathB, out string common)
		{
			var pcr = DoCompareDirectoryPaths(pathA, pathB);
			if (pcr.HaveCommon)
			{
				common = pcr.CommonPath;
				return (true);
			}
			else
			{
				common = null;
				return (false);
			}
		}

		#endregion

		#region ...Unique...()
		//------------------------------------------------------------------------------------------
		// ...Unique...()
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns a unique temporary file path, using a new guid (= globally unique).
		/// </summary>
		/// <remarks>
		/// <see cref="FileEx.GetUniqueFilePath"/> offers a similar method keeping the file name.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Naming same as 'Path.GetTempPath()'.")]
		public static string GetUniqueTempPath()
		{
			return (Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
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
		public bool HaveCommon { get; set; }

		/// <summary>Common path, e.g. "C:\MyDir".</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		public string CommonPath { get; set; }

		/// <summary>Number of common directories, e.g. "C:\MyDir" results in 1.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		public int CommonDirectoryCount { get; set; }

		/// <summary>True if directories are relative, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2".</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		public bool AreRelative { get; set; }

		/// <summary>Number of relative directories, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2" results in 2.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		public int RelativeDirectoryCount { get; set; }

		/// <summary>True if directories are near relative, e.g. "C:\MyDir" and "C:\MyDir\MySubDir".</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		public bool AreNearRelative { get; set; }

		/// <summary>Number of near relative directories, e.g. "C:\MyDir" and "C:\MyDir\MySubDir" results in 1.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Dir'...")]
		public int NearRelativeDirectoryCount { get; set; }

		/// <summary>Relative path between the two.</summary>
		public string RelativePath { get; set; }

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(bool haveCommon)
			: this(haveCommon, "")
		{
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(bool haveCommon, string relativePath)
		{
			HaveCommon                 = haveCommon;
			CommonPath                 = null;
			CommonDirectoryCount       = 0;
			AreRelative                = false;
			RelativeDirectoryCount     = 0;
			AreNearRelative            = false;
			NearRelativeDirectoryCount = 0;
			RelativePath               = relativePath;
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(string commonPath, int commonDirectoryCount, int relativeDirectoryCount, string relativePath)
			: this(commonPath, commonDirectoryCount, relativeDirectoryCount, false, 0, relativePath)
		{
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public PathCompareResult(string commonPath, int commonDirectoryCount, int relativeDirectoryCount, bool areNearRelative, int nearRelativeDirectoryCount, string relativePath)
		{
			HaveCommon                 = true;
			CommonPath                 = commonPath;
			CommonDirectoryCount       = commonDirectoryCount;
			AreRelative                = true;
			RelativeDirectoryCount     = relativeDirectoryCount;
			AreNearRelative            = areNearRelative;
			NearRelativeDirectoryCount = nearRelativeDirectoryCount;
			RelativePath               = relativePath;
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
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(PathCompareResult lhs, PathCompareResult rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
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
