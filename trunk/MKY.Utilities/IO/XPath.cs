//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MKY.Utilities.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.Path"/>.
	/// </summary>
	public static class XPath
	{
		#region LimitPath Method

		/// <summary>
		/// Limits a folder or file path to the specified max length.
		/// </summary>
		public static string LimitPath(string path, int length)
		{
			string limitedPath;

			if (path.Length <= length)                 // Path string too long ?
				return (path);

			if (path.IndexOf(Path.VolumeSeparatorChar) < 0)
			{                                          // Local path ?
				limitedPath = Types.XString.Left(path, 3) + "..." +
							  Types.XString.Right(path, Math.Max(length - 6, 0));
			}
			else                                       // Network path !
			{
				int separatorPosition = path.Substring(4).IndexOf(Path.DirectorySeparatorChar);
				if ((separatorPosition >= 0) && (separatorPosition < length - 4))
				{
					limitedPath = Types.XString.Left(path, separatorPosition) + "..." +
								  Types.XString.Right(path, Math.Max(length - 4 - separatorPosition, 0));
				}
				else
				{
					limitedPath = Types.XString.Left(path, 5) + "..." +
								  Types.XString.Right(path, Math.Max(length - 8, 0));
				}
			}

			return (Types.XString.Right(limitedPath, length));
		}

		#endregion

		#region Compare Methods

		/// <summary>
		/// Compares directoryPath2 relative to directoryPath1 and returns relative path of directory2.
		/// </summary>
		/// <remarks>
		/// Returns directoryPath2 if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static XPathCompareResult CompareDirectoryPaths(string directoryPath1, string directoryPath2)
		{
			if (!Path.IsPathRooted(directoryPath1))
				return (new XPathCompareResult(false));

			if (!Path.IsPathRooted(directoryPath2))
				return (new XPathCompareResult(false, directoryPath2));

			return (DoCompareDirectoryPaths(directoryPath1, directoryPath2));
		}

		/// <summary>
		/// Compares filePath relative to directoryPath and returns relative path of file.
		/// </summary>
		/// <remarks>
		/// Returns filePath if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static XPathCompareResult CompareDirectoryAndFilePaths(string directoryPath, string filePath)
		{
			if (!Path.IsPathRooted(directoryPath))
				return (new XPathCompareResult(false));

			if (!Path.IsPathRooted(filePath))
				return (new XPathCompareResult(false, filePath));

			string fileName = Path.GetFileName(filePath);
			XPathCompareResult pcr = DoCompareDirectoryPaths(directoryPath, Path.GetDirectoryName(filePath));
			pcr.RelativePath += (Path.DirectorySeparatorChar + fileName);
			return (pcr);
		}

		/// <summary>
		/// Compares directoryPath relative to filePath and returns relative path of directory2.
		/// </summary>
		/// <remarks>
		/// Returns directoryPath if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static XPathCompareResult CompareFileAndDirectoryPaths(string filePath, string directoryPath)
		{
			if (!Path.IsPathRooted(filePath))
				return (new XPathCompareResult(false));

			if (!Path.IsPathRooted(directoryPath))
				return (new XPathCompareResult(false, directoryPath));

			return (DoCompareDirectoryPaths(Path.GetDirectoryName(filePath), directoryPath));
		}

		/// <summary>
		/// Compares filePath2 relative to filePath1 and returns relative path of file2.
		/// </summary>
		/// <remarks>
		/// Returns filePath2 if it already is relative.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static XPathCompareResult CompareFilePaths(string filePath1, string filePath2)
		{
			if (!Path.IsPathRooted(filePath1))
				return (new XPathCompareResult(false));

			if (!Path.IsPathRooted(filePath2))
				return (new XPathCompareResult(false, filePath2));

			string fileName2 = Path.GetFileName(filePath2);
			XPathCompareResult pcr = DoCompareDirectoryPaths(Path.GetDirectoryName(filePath1), Path.GetDirectoryName(filePath2));
			pcr.RelativePath += (Path.DirectorySeparatorChar + fileName2);
			return (pcr);
		}

		#endregion

		#region Combine Methods

		/// <summary>
		/// Resolves directoryPath2 relative to directoryPath1 and returns absolute path of directory2.
		/// </summary>
		/// <remarks>
		/// Returns directoryPath2 if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static string CombineDirectoryPaths(string directoryPath1, string directoryPath2)
		{
			if (Path.IsPathRooted(directoryPath2))
				return (directoryPath2);

			if (!Path.IsPathRooted(directoryPath1))
				return (directoryPath1);

			return (DoCombineDirectoryPaths(directoryPath1, directoryPath2));
		}

		/// <summary>
		/// Resolves filePath relative to directoryPath and returns absolute path of file.
		/// </summary>
		/// <remarks>
		/// Returns filePath if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static string CombineDirectoryAndFilePaths(string directoryPath, string filePath)
		{
			if (Path.IsPathRooted(filePath))
				return (filePath);

			if (!Path.IsPathRooted(directoryPath))
				return (directoryPath);

			string fileName = Path.GetFileName(filePath);
			string absolutePath = DoCombineDirectoryPaths(directoryPath, Path.GetDirectoryName(filePath));

			//string root = Path.GetPathRoot(absolutePath);
			string combined = Path.Combine(absolutePath, fileName);
			return (combined);
			/*if (absolutePath != Path.GetPathRoot(absolutePath))
				return (absolutePath + Path.DirectorySeparatorChar + fileName);
			else
				return (absolutePath + fileName);*/
		}

		/// <summary>
		/// Resolves directoryPath relative to filePath and returns absolute path of directory2.
		/// </summary>
		/// <remarks>
		/// Returns directoryPath if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static string CombineFileAndDirectoryPaths(string filePath, string directoryPath)
		{
			if (Path.IsPathRooted(directoryPath))
				return (directoryPath);

			if (!Path.IsPathRooted(filePath))
				return (filePath);

			return (DoCombineDirectoryPaths(Path.GetDirectoryName(filePath), directoryPath));
		}

		/// <summary>
		/// Resolves filePath2 relative to filePath1 and returns absolute path of file2.
		/// </summary>
		/// <remarks>
		/// Returns filePath2 if it is absolute.
		/// Why is this functionality not already provided by <see cref="System.IO.Path"/>?
		/// Seems that the Microsoft guys were a bit lazy ;-)
		/// </remarks>
		public static string CombineFilePaths(string filePath1, string filePath2)
		{
			if (Path.IsPathRooted(filePath2))
				return (filePath2);

			if (!Path.IsPathRooted(filePath1))
				return (filePath1);

			string fileName2 = Path.GetFileName(filePath2);
			string absolutePath = DoCombineDirectoryPaths(Path.GetDirectoryName(filePath1), Path.GetDirectoryName(filePath2));

			string combined = Path.Combine(absolutePath, fileName2);
			return (combined);

			/*if (absolutePath != Path.GetPathRoot(absolutePath))
				return (absolutePath + Path.DirectorySeparatorChar + fileName2);
			else
				return (absolutePath + fileName2);*/
		}

		#endregion

		#region DoCompareDirectoryPaths Method

		/// <summary>
		/// Returns relation between the two absolute directory paths.
		/// </summary>
		public static XPathCompareResult DoCompareDirectoryPaths(string pathA, string pathB)
		{
			// do not check for reference equality because complete result
			// needs to be retrieved anyway

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
				// check whether both directories share the same root
				if (dirInfoA.Root.FullName != dirInfoB.Root.FullName)
					return (new XPathCompareResult(false));

				// get common directory, make sure only directory part is used
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

				// reverse lists
				dirInfosA.Reverse();
				dirInfosB.Reverse();

				// get common directory, make sure only directory part is used
				int i = 0;
				while ((dirInfosA.Count > i) && (dirInfosB.Count > i) &&
					   (dirInfosA[i] == dirInfosB[i]))
				{
					i++;
				}

				int commonDirectoryCount = i;

				string commonPath = "";
				if (commonDirectoryCount > 0)
					commonPath = dirInfosA[commonDirectoryCount - 1];

				DirectoryInfo commonDI = new DirectoryInfo(commonPath);

				// check whether both paths are equal
				if (dirPathA == dirPathB)
					return (new XPathCompareResult(commonPath, commonDirectoryCount, 0, 0, "."));

				// check whether one of the two is the others subdirectory
				DirectoryInfo di = commonDI;
				StringBuilder relativePath = new StringBuilder();
				if (commonPath == dirPathA)
				{
					int nearRelativeDirectoryCount = 0;
					di = dirInfoB;
					while ((di != null) && (di.FullName != commonPath))
					{
						nearRelativeDirectoryCount++;

						if (relativePath.Length > 0)       // Actually, stepping in is
						{                                  //   done by stepping out
							relativePath.Insert(0, Path.DirectorySeparatorChar);
							relativePath.Insert(0, di.Name);
						}
						else
						{
							relativePath.Append(di.Name);
						}

						di = di.Parent;
					}
					return (new XPathCompareResult(commonPath, commonDirectoryCount, nearRelativeDirectoryCount, nearRelativeDirectoryCount, relativePath.ToString()));
				}
				if (commonPath == dirPathB)
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
					return (new XPathCompareResult(commonPath, commonDirectoryCount, nearRelativeDirectoryCount, nearRelativeDirectoryCount, relativePath.ToString()));
				}

				// in case of far relation, first step out to common path, then step into path B
				int farRelativeDirectoryCount = 0;
				di = dirInfoA;
				while ((di != null) && (di.FullName != commonPath)) // step out to common path
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
				while ((di != null) && (di.FullName != commonPath)) // step into path B
				{
					farRelativeDirectoryCount++;

					if (relativePath.Length > commonPartIndex) // actually, stepping in is
					{                                          //   done by stepping out
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
				return (new XPathCompareResult(commonPath, commonDirectoryCount, farRelativeDirectoryCount, relativePath.ToString()));
			}

			return (new XPathCompareResult(false));
		}

		#endregion

		#region DoCombineDirectoryPaths Method

		/// <summary>
		/// Takes the first directory path and combines it with the second directory
		/// path also taking "." and ".." into account.
		/// </summary>
		public static string DoCombineDirectoryPaths(string pathA, string pathB)
		{
			DirectoryInfo pathInfoA = null;

			string dirPathA = "";
			DirectoryInfo dirInfoA = null;

			DoPrepareDirectoryPath(pathA, out pathInfoA, out dirPathA, out dirInfoA);

			if ((pathInfoA != null) && (pathB.Length > 0))
			{
				DirectoryInfo pathInfoResult = null;

				string dirPathResult = "";
				DirectoryInfo dirInfoResult = null;

				// Trim leading '\'
				string s = pathB.TrimStart(Path.DirectorySeparatorChar);

				// Check whether relative path points to any parent directory
				if ((s.Length >= 2) && (s.Substring(0, 2) == ".."))
				{
					DirectoryInfo pathInfoParent = pathInfoA;

					do
					{
						// Detect invalidly long relative paths
						if ((s.Length >= 3) && (s.Substring(0, 3) == "..."))
							break;

						s = s.Remove(0, 2);
						pathInfoParent = pathInfoParent.Parent;

						// ".." or "..\"
						if ((s.Length == 0) ||
							(s == Path.DirectorySeparatorChar.ToString()))
						{
							return (pathInfoParent.FullName);
						}

						// "..\<.. or Path>"
						if ((s.Length >= 1) && (s.Substring(0, 1) == Path.DirectorySeparatorChar.ToString()))
							s = s.Remove(0, 1);
						else
							break;
					}
					while ((s.Length >= 2) && (s.Substring(0, 2) == ".."));

					if (pathInfoParent != null)
						DoPrepareDirectoryPath(Path.Combine(pathInfoParent.FullName, s), out pathInfoResult, out dirPathResult, out dirInfoResult);
				}

				// Check whether relative path points to current directory
				else if ((s.Length >= 1) && (s.Substring(0, 1) == "."))
				{
					s = s.Remove(0, 1);

					// "." or ".\"
					if ((s.Length == 0) ||
						(s == Path.DirectorySeparatorChar.ToString()))
					{
						return (dirPathA);
					}

					// ".\<Path>"
					if (s.Substring(0, 1) == Path.DirectorySeparatorChar.ToString())
					{
						string combined = dirPathA + s.Substring(1);
						DoPrepareDirectoryPath(combined, out pathInfoResult, out dirPathResult, out dirInfoResult);
					}
				}

				// Use System.IO.Path.Combine() for the easy cases
				else
				{
					string combined = Path.Combine(dirPathA, pathB);
					DoPrepareDirectoryPath(combined, out pathInfoResult, out dirPathResult, out dirInfoResult);
				}

				if (pathInfoResult != null)
					return (dirPathResult);
			}

			// In case the second path was invalid, return the the first if possible
			if (pathInfoA != null)
				return (dirPathA);
			else
				return ("");
		}

		#endregion

		#region DoPrepareDirectoryPath Method

		private static void DoPrepareDirectoryPath(string path, out DirectoryInfo pathInfo, out string dirPath, out DirectoryInfo dirInfo)
		{
			try
			{
				// DirectoryInfo throws if path contains invalid characters
				pathInfo = new DirectoryInfo(path);
			}
			catch (Exception ex)
			{
				MKY.Utilities.Diagnostics.XDebug.WriteException(typeof(XPath), ex);
				pathInfo = null;
			}

			// Get directory and file name
			if (pathInfo != null)
			{
				DirectoryInfo temp = new DirectoryInfo(path);

				// Make sure parent directory and directory name is properly returned
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

		#region DoPrepareFilePath Method

		private static void DoPrepareFilePath(string path, out DirectoryInfo pathInfo, out string dirPath, out DirectoryInfo dirInfo, out string fileName)
		{
			try
			{
				// DirectoryInfo throws if path contains invalid characters
				pathInfo = new DirectoryInfo(path);
			}
			catch
			{
				pathInfo = null;
			}

			// Get directory and file name
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
	}

	#region XPathCompareResult Struct

	/// <summary>
	/// Structure to hold the complete result of a directory comparison.
	/// </summary>
	public struct XPathCompareResult
	{
		/// <summary>True if directories share a common path, i.e. also a common root.</summary>
		public bool HaveCommon;

		/// <summary>Common path, e.g. "C:\MyDir".</summary>
		public string CommonPath;

		/// <summary>Number of common directories, e.g. "C:\MyDir" results in 1.</summary>
		public int CommonDirectoryCount;

		/// <summary>True if directories are relative, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2".</summary>
		public bool AreRelative;

		/// <summary>Number of relative directories, e.g. "C:\MyDir\MySubDir1" and "C:\MyDir\MySubDir2" results in 2.</summary>
		public int RelativeDirectoryCount;

		/// <summary>True if directories are near relative, e.g. "C:\MyDir" and "C:\MyDir\MySubDir".</summary>
		public bool AreNearRelative;

		/// <summary>Number of near relative directories, e.g. "C:\MyDir" and "C:\MyDir\MySubDir" results in 1.</summary>
		public int NearRelativeDirectoryCount;

		/// <summary>Relative path between the two.</summary>
		public string RelativePath;

		/// <summary>Creates a directory info compare result structure.</summary>
		public XPathCompareResult(bool haveCommon)
			: this (haveCommon, "")
		{
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public XPathCompareResult(bool haveCommon, string relativePath)
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
		public XPathCompareResult(string commonPath, int commonDirectoryCount, int relativeDirectoryCount, string relativePath)
			: this (commonPath, commonDirectoryCount, relativeDirectoryCount, 0, relativePath)
		{
		}

		/// <summary>Creates a directory info compare result structure.</summary>
		public XPathCompareResult(string commonPath, int commonDirectoryCount, int relativeDirectoryCount, int nearRelativeDirectoryCount, string relativePath)
		{
			HaveCommon = true;
			CommonPath = commonPath;
			CommonDirectoryCount = commonDirectoryCount;
			AreRelative = true;
			RelativeDirectoryCount = relativeDirectoryCount;
			AreNearRelative = true;
			NearRelativeDirectoryCount = nearRelativeDirectoryCount;
			RelativePath = relativePath;
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
