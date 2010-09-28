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

using System.Diagnostics;
using System.IO;

namespace MKY.Utilities.Test
{
	/// <summary>
	/// Utility methods to deal with temporary files during testing.
	/// </summary>
	public static class XPath
	{
		/// <summary></summary>
		public static string MakeTempPath(object testObject)
		{
			return (MakeTempPath(testObject, true));
		}

		private static string MakeTempPath(object testObject, bool outputPathToDebugConsole)
		{
			// Results in e.g. "D:\Temp\MKY.Utilities.Test".
			string path = Path.GetTempPath() + testObject.GetType().Namespace;

			if (outputPathToDebugConsole)
				Debug.WriteLine(@"Temporary path is      """ + path + @"""");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return (path);
		}

		/// <summary></summary>
		public static void CleanTempPath(object testObject)
		{
			CleanTempPath(testObject, true);
		}

		private static void CleanTempPath(object testObject, bool outputPathToDebugConsole)
		{
			string path = MakeTempPath(testObject, false);

			if (Directory.Exists(path))
				Directory.Delete(path, true);

			if (outputPathToDebugConsole)
				Debug.WriteLine(@"Temporary path         """ + path + @""" cleaned");
		}

		/// <summary></summary>
		public static string MakeTempFileName(object testObject, string extension)
		{
			return (MakeTempFileName(testObject, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFileName(object testObject, string name, string extension)
		{
			return (MakeTempFileName(testObject, "", extension, true));
		}

		private static string MakeTempFileName(object testObject, string name, string extension, bool outputFileNameToDebugConsole)
		{
			string testObjectFullName = testObject.GetType().FullName;
			string fileName;

			if ((name != null) && (name.Length > 0))
				fileName = testObjectFullName + "-" + name + extension;
			else
				fileName = testObjectFullName + extension;

			if (outputFileNameToDebugConsole)
				Debug.WriteLine(@"Temporary file name is """ + fileName + @"""");

			return (fileName);
		}

		/// <summary></summary>
		public static string MakeTempFilePath(object testObject, string extension)
		{
			return (MakeTempFilePath(testObject, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFilePath(object testObject, string name, string extension)
		{
			return (MakeTempFilePath(testObject, "", extension, true));
		}

		private static string MakeTempFilePath(object testObject, string name, string extension, bool outputFilePathToDebugConsole)
		{
			string filePath = MakeTempPath(testObject, false) + Path.DirectorySeparatorChar + MakeTempFileName(testObject, name, extension, false);

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
