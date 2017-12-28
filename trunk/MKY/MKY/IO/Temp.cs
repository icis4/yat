﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.22
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
using System.Globalization;
using System.IO;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods to deal with temporary files during testing.
	/// </summary>
	public static class Temp
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string MakeTempPath(Type type, bool outputPathToDebug = true)
		{
			// Results in e.g. "MKY".
			string root = "";
			string[] splits = type.Namespace.Split('.');
			if (splits.Length > 0)
				root = splits[0];

			// Results in e.g. "D:\Temp\MKY\MKY.Test\MKY.Test.MyClass".
			string path;
			if (string.IsNullOrEmpty(root))
				path = Path.GetTempPath()                                      + type.Namespace + Path.DirectorySeparatorChar + type.Name;
			else
				path = Path.GetTempPath() + root + Path.DirectorySeparatorChar + type.Namespace + Path.DirectorySeparatorChar + type.Name;

			if (outputPathToDebug)
				Debug.WriteLine(@"Temporary path is      """ + path + @""".");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return (path);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void CleanTempPath(Type type, bool outputPathToDebug = true)
		{
			string path = MakeTempPath(type, false);

			if (Directory.Exists(path))
			{
				DirectoryEx.MakeAllFilesWritable(path, true);
				Directory.Delete(path, true);
			}

			if (outputPathToDebug)
				Debug.WriteLine(@"Temporary path         """ + path + @""" cleaned.");
		}

		/// <param name="type">The type of an object, is used to retrieve namespace and type name.</param>
		/// <param name="extension">The desired file extension, must include the dot as it is the case in similar I/O methods of .NET.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string MakeTempFileName(Type type, string extension = null)
		{
			return (MakeTempFileName(type, null, extension));
		}

		/// <param name="type">The type of an object, is used to retrieve namespace and type name.</param>
		/// <param name="name">An additional name that is appended to the file name.</param>
		/// <param name="extension">The desired file extension, must include the dot as it is the case in similar I/O methods of .NET.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string MakeTempFileName(Type type, string name, string extension = null)
		{
			return (MakeTempFileName(type, name, null, extension, true));
		}

		/// <param name="type">The type of an object, is used to retrieve namespace and type name.</param>
		/// <param name="name">An additional name that is appended to the file name.</param>
		/// <param name="postfix">Yet another postfix to the file name.</param>
		/// <param name="extension">The desired file extension, must include the dot as it is the case in similar I/O methods of .NET.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string MakeTempFileName(Type type, string name, string postfix, string extension = null)
		{
			return (MakeTempFileName(type, name, postfix, extension, true));
		}

		private static string MakeTempFileName(Type type, string name, string postfix, string extension, bool outputFileNameToDebug)
		{
			string fileName = type.FullName;

			if (!string.IsNullOrEmpty(name))
				fileName += "-" + name;

			if (!string.IsNullOrEmpty(postfix))
				fileName += "-" + postfix;

			if (!string.IsNullOrEmpty(extension))
				fileName += extension;

			if (outputFileNameToDebug)
				Debug.WriteLine(@"Temporary file name is """ + fileName + @""".");

			return (fileName);
		}

		/// <param name="type">The type of an object, is used to retrieve namespace and type name.</param>
		/// <param name="extension">The desired file extension, must include the dot as it is the case in similar I/O methods of .NET.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string MakeTempFilePath(Type type, string extension = null)
		{
			return (MakeTempFilePath(type, null, extension));
		}

		/// <param name="type">The type of an object, is used to retrieve namespace and type name.</param>
		/// <param name="name">An additional name that is appended to the file name.</param>
		/// <param name="extension">The desired file extension, must include the dot as it is the case in similar I/O methods of .NET.</param>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string MakeTempFilePath(Type type, string name, string extension = null)
		{
			return (MakeTempFilePath(type, name, extension, true));
		}

		private static string MakeTempFilePath(Type type, string name, string extension, bool outputFilePathToDebug)
		{
			string filePath = MakeTempPath(type, false) + Path.DirectorySeparatorChar + MakeTempFileName(type, name, "", extension, false);

			// Postfix the file name with a unique number if the file already exists.
			// Do not use a GUID:
			//  > The file name gets very long.
			//  > It's more difficult to find related files.
			if (File.Exists(filePath))
			{
				const int Max = 999999999; // Pretty close to int.MaxValue, and ridiculously long for the number of files in a directory...
				string istr = "";
				int i = 0;
				for (i = 0; i <= Max; i++)
				{
					istr = i.ToString(CultureInfo.InvariantCulture);
					filePath = MakeTempPath(type, false) + Path.DirectorySeparatorChar + MakeTempFileName(type, name, istr, extension, false);
					if (!File.Exists(filePath))
						break;
				}

				if (i >= Max)
					throw (new OverflowException("Failed to create a temporary file name because there already are " + istr + " files in the directory!"));
			}

			// Optionally output the file path for debugging purposes.
			if (outputFilePathToDebug)
				Debug.WriteLine(@"Temporary file path is """ + filePath + @""".");

			return (filePath);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
