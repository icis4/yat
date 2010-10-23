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
using System.IO;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Guid for consistency with the Sytem.Guid class.
namespace MKY
{
	/// <summary>
	/// Some GUID utilities.
	/// </summary>
	public static class GuidEx
	{
		/// <summary>
		/// Creates and returns GUID from terminal file path if possible, new GUID otherwise.
		/// </summary>
		public static Guid CreateGuidFromFilePath(string filePath)
		{
			return (CreateGuidFromFilePath(filePath, "", ""));
		}

		/// <summary>
		/// Creates and returns GUID from terminal file path if possible, new GUID otherwise.
		/// </summary>
		public static Guid CreateGuidFromFilePath(string filePath, string prefix)
		{
			return (CreateGuidFromFilePath(filePath, prefix, ""));
		}

		/// <summary>
		/// Creates and returns GUID from terminal file path if possible, new GUID otherwise.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public static Guid CreateGuidFromFilePath(string filePath, string prefix, string postfix)
		{
			// File path may look like ".\Terminal-dcf25dde-947a-4470-8567-b0dde2459933.yat".
			//                             Length: ^1     ^8   ^13  ^18  ^23          ^36
			string fileName = Path.GetFileNameWithoutExtension(filePath);

			// Do some basic checks to minimize probablity of exception below.
			bool tryCreate = true;
			if (tryCreate && (fileName.Length < (prefix.Length + 32))) // GUID string contains at least 32 chars.
				tryCreate = false;
			if (tryCreate && (string.Compare(fileName.Substring(0, prefix.Length), prefix) != 0))
				tryCreate = false;

			if (tryCreate)
			{
				// Retrieve GUID string and try to create GUID from it.
				string guidString = fileName.Substring(prefix.Length);
				try
				{
					return (new Guid(guidString));
				}
				catch { }
			}

			// Create new GUID.
			return (Guid.NewGuid());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
