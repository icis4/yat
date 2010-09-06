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
			// Results in e.g. D:\Temp\MKY.Utilities.Test\
			string path = Path.GetTempPath() + Path.DirectorySeparatorChar + testObject.GetType().Namespace;

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return (path);
		}

		/// <summary></summary>
		public static void CleanTempPath(object testObject)
		{
			string path = MakeTempPath(testObject);

			if (!Directory.Exists(path))
				Directory.Delete(path, true);
		}

		/// <summary></summary>
		public static string MakeTempFileName(object testObject, string extension)
		{
			return (MakeTempFileName(testObject, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFileName(object testObject, string name, string extension)
		{
			string testObjectFullName = testObject.GetType().FullName;

			if ((name != null) && (name.Length > 0))
				return (testObjectFullName + "-" + name + extension);
			else
				return (testObjectFullName + extension);
		}

		/// <summary></summary>
		public static string MakeTempFilePath(object testObject, string extension)
		{
			return (MakeTempFilePath(testObject, "", extension));
		}

		/// <summary></summary>
		public static string MakeTempFilePath(object testObject, string name, string extension)
		{
			return (MakeTempPath(testObject) + Path.DirectorySeparatorChar + MakeTempFileName(name, extension));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
