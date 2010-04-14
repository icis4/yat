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
//==================================================================================================

using System.IO;

namespace MKY.Utilities.Guid
{
	/// <summary>
	/// Some GUID utilities.
	/// </summary>
	public static class XGuid
	{
		/// <summary>
		/// Creates and returns GUID from terminal file path if possible, new GUID otherwise.
		/// </summary>
		public static System.Guid CreateGuidFromFilePath(string filePath)
		{
			return (CreateGuidFromFilePath(filePath, "", ""));
		}

		/// <summary>
		/// Creates and returns GUID from terminal file path if possible, new GUID otherwise.
		/// </summary>
		public static System.Guid CreateGuidFromFilePath(string filePath, string prefix)
		{
			return (CreateGuidFromFilePath(filePath, prefix, ""));
		}

		/// <summary>
		/// Creates and returns GUID from terminal file path if possible, new GUID otherwise.
		/// </summary>
		public static System.Guid CreateGuidFromFilePath(string filePath, string prefix, string postfix)
		{
			// file path may look like ".\Terminal-dcf25dde-947a-4470-8567-b0dde2459933.yat"
			//                             Length: ^1     ^8   ^13  ^18  ^23          ^36
			string fileName = Path.GetFileNameWithoutExtension(filePath);

			// do some basic checks to minimize probablity of exception below
			bool tryCreate = true;
			if (tryCreate && (fileName.Length < (prefix.Length + 32))) // GUID string contains at least 32 chars
				tryCreate = false;
			if (tryCreate && (string.Compare(fileName.Substring(0, prefix.Length), prefix) != 0))
				tryCreate = false;

			if (tryCreate)
			{
				// retrieve GUID string and try to create GUID from it
				string guidString = fileName.Substring(prefix.Length);
				try
				{
					return (new System.Guid(guidString));
				}
				catch
				{
				}
			}

			// Create new GUID
			return (System.Guid.NewGuid());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
