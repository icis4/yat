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
using System.Diagnostics;
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

		private static string MakeTempPath(Type type, bool outputPathToDebugConsole)
		{
			// Results in e.g. "D:\Temp\MKY.Test".
			string path = Path.GetTempPath() + type.Namespace;

			if (outputPathToDebugConsole)
				Debug.WriteLine(@"Temporary path is      """ + path + @"""");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return (path);
		}

		/// <summary></summary>
		public static void CleanTempPath(Type type)
		{
			CleanTempPath(type, true);
		}

		private static void CleanTempPath(Type type, bool outputPathToDebugConsole)
		{
			string path = MakeTempPath(type, false);

			if (Directory.Exists(path))
				Directory.Delete(path, true);

			if (outputPathToDebugConsole)
				Debug.WriteLine(@"Temporary path         """ + path + @""" cleaned");
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

		private static string MakeTempFileName(Type type, string name, string postfix, string extension, bool outputFileNameToDebugConsole)
		{
			string fileName = type.FullName;

			if ((name != null) && (name.Length > 0))
				fileName += "-" + name;

			if ((postfix != null) && (postfix.Length > 0))
				fileName += "-" + postfix;

			fileName += extension;

			if (outputFileNameToDebugConsole)
				Debug.WriteLine(@"Temporary file name is """ + fileName + @"""");

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

		private static string MakeTempFilePath(Type type, string name, string extension, bool outputFilePathToDebugConsole)
		{
			string filePath = MakeTempPath(type, false) + Path.DirectorySeparatorChar + MakeTempFileName(type, name, "", extension, false);
			if (File.Exists(filePath))
			{
				const int Max = 9999;
				int i = 0;
				for (i = 0; i <= Max; i++)
				{
					filePath = MakeTempPath(type, false) + Path.DirectorySeparatorChar + MakeTempFileName(type, name, i.ToString(), extension, false);
					if (!File.Exists(filePath))
						break;
				}

				if (i >= Max)
					throw (new FileNotFoundException("Failed to create temporary file name"));
			}

			if (outputFilePathToDebugConsole)
				Debug.WriteLine(@"Temporary file path is """ + filePath + @"""");

			return (filePath);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
