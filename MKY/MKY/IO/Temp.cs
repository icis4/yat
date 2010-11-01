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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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
		public static string MakeTempPath(Type testType)
		{
			return (MakeTempPath(testType, true));
		}

		private static string MakeTempPath(Type testType, bool outputPathToDebugConsole)
		{
			// Results in e.g. "D:\Temp\MKY.Test".
			string path = Path.GetTempPath() + testType.Namespace;

			if (outputPathToDebugConsole)
				Debug.WriteLine(@"Temporary path is      """ + path + @"""");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return (path);
		}

		/// <summary></summary>
		public static void CleanTempPath(Type testType)
		{
			CleanTempPath(testType, true);
		}

		private static void CleanTempPath(Type testType, bool outputPathToDebugConsole)
		{
			string path = MakeTempPath(testType, false);

			if (Directory.Exists(path))
				Directory.Delete(path, true);

			if (outputPathToDebugConsole)
				Debug.WriteLine(@"Temporary path         """ + path + @""" cleaned");
		}

		/// <summary></summary>
		public static string MakeTempFileName(Type testType, string extension)
		{
			return (MakeTempFileName(testType, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFileName(Type testType, string name, string extension)
		{
			return (MakeTempFileName(testType, name, extension, true));
		}

		private static string MakeTempFileName(Type testType, string name, string extension, bool outputFileNameToDebugConsole)
		{
			string testTypeFullName = testType.FullName;
			string fileName;

			if ((name != null) && (name.Length > 0))
				fileName = testTypeFullName + "-" + name + extension;
			else
				fileName = testTypeFullName + extension;

			if (outputFileNameToDebugConsole)
				Debug.WriteLine(@"Temporary file name is """ + fileName + @"""");

			return (fileName);
		}

		/// <summary></summary>
		public static string MakeTempFilePath(Type testType, string extension)
		{
			return (MakeTempFilePath(testType, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFilePath(Type testType, string name, string extension)
		{
			return (MakeTempFilePath(testType, name, extension, true));
		}

		private static string MakeTempFilePath(Type testType, string name, string extension, bool outputFilePathToDebugConsole)
		{
			string filePath = MakeTempPath(testType, false) + Path.DirectorySeparatorChar + MakeTempFileName(testType, name, extension, false);

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
