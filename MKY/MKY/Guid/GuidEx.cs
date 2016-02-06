//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.IO;

#endregion

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Guid for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// Some GUID utilities.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class GuidEx
	{
		/// <summary>
		/// Tries to create and return a GUID from the file path if possible.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static bool TryParse(string s, out Guid guid)
		{
			try
			{
				guid = new Guid(s.Trim());
				return (true);
			}
			catch
			{
				guid = Guid.Empty;
				return (false);
			}
		}

		/// <summary>
		/// Tries to create and return a GUID from the file path if possible.
		/// </summary>
		public static bool TryCreateGuidFromFilePath(string filePath, out Guid guid)
		{
			return (TryCreateGuidFromFilePath(filePath, "", "", out guid));
		}

		/// <summary>
		/// Tries to create and return a GUID from the file path if possible.
		/// </summary>
		public static bool TryCreateGuidFromFilePath(string filePath, string prefix, out Guid guid)
		{
			return (TryCreateGuidFromFilePath(filePath, prefix, "", out guid));
		}

		/// <summary>
		/// Tries to create and return a GUID from the file path if possible.
		/// </summary>
		public static bool TryCreateGuidFromFilePath(string filePath, string prefix, string postfix, out Guid guid)
		{
			guid = Guid.Empty;

			// File path may look like ".\Prefix-dcf25dde-947a-4470-8567-b0dde2459933-Postfix.ext".
			//                           Length: ^1     ^8   ^13  ^18  ^23          ^36
			string fileName = Path.GetFileNameWithoutExtension(filePath);
			string actualPrefix    = StringEx.Left (fileName, prefix.Length);
			string actualInbetween = StringEx.Mid  (fileName, prefix.Length, (fileName.Length - postfix.Length - 1));
			string actualPostfix   = StringEx.Right(fileName, postfix.Length);

			// Do some basic checks to minimize probablity of exception below:
			if (actualInbetween.Length < 32) // GUID string contains at least 32 chars.
				return (false);

			if (!StringEx.EqualsOrdinalIgnoreCase(actualPrefix, prefix))
				return (false);

			if (!StringEx.EqualsOrdinalIgnoreCase(actualPostfix, postfix))
				return (false);

			return (TryParse(actualInbetween, out guid));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
