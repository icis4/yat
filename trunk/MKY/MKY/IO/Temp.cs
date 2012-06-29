//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
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
		public static string MakeTempPath(Type type)
		{
			return (MakeTempPath(type, true));
		}

		private static string MakeTempPath(Type type, bool outputPathToDebug)
		{
			// Results in e.g. "MKY".
			string root = string.Empty;
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
		public static void CleanTempPath(Type type)
		{
			CleanTempPath(type, true);
		}

		private static void CleanTempPath(Type type, bool outputPathToDebug)
		{
			string path = MakeTempPath(type, false);

			if (Directory.Exists(path))
				Directory.Delete(path, true);

			if (outputPathToDebug)
				Debug.WriteLine(@"Temporary path         """ + path + @""" cleaned.");
		}

		/// <summary></summary>
		public static string MakeTempFileName(Type type, string extension)
		{
			return (MakeTempFileName(type, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFileName(Type type, string name, string extension)
		{
			return (MakeTempFileName(type, name, "", extension, true));
		}

		/// <summary></summary>
		public static string MakeTempFileName(Type type, string name, string postfix, string extension)
		{
			return (MakeTempFileName(type, name, postfix, extension, true));
		}

		private static string MakeTempFileName(Type type, string name, string postfix, string extension, bool outputFileNameToDebug)
		{
			string fileName = type.FullName;

			if ((name != null) && (name.Length > 0))
				fileName += "-" + name;

			if ((postfix != null) && (postfix.Length > 0))
				fileName += "-" + postfix;

			fileName += extension;

			if (outputFileNameToDebug)
				Debug.WriteLine(@"Temporary file name is """ + fileName + @""".");

			return (fileName);
		}

		/// <summary></summary>
		public static string MakeTempFilePath(Type type, string extension)
		{
			return (MakeTempFilePath(type, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFilePath(Type type, string name, string extension)
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
				const int Max = 999999999; // Pretty close to int.MaxValue, and ridiculously long for a number of file in a directory.
				string istr = "";
				int i = 0;
				for (i = 0; i <= Max; i++)
				{
					istr = i.ToString(NumberFormatInfo.InvariantInfo);
					filePath = MakeTempPath(type, false) + Path.DirectorySeparatorChar + MakeTempFileName(type, name, istr, extension, false);
					if (!File.Exists(filePath))
						break;
				}

				if (i >= Max)
					throw (new OverflowException("Failed to create a temporary file name because there already are " + istr + " files in the directory"));
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
