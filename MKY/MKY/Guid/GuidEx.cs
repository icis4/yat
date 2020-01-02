//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Text.RegularExpressions;

#endregion

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Guid for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Some GUID utilities.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class GuidEx
	{
		private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

		/// <summary>
		/// A compiled <see cref="Regex"/> that can be used for parsing a GUID from a string.
		/// </summary>
		public static readonly Regex Regex = new Regex(@"[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}", Options);

		/// <summary>
		/// Tries to create and return a <see cref="Guid"/> object from the string specified.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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
		/// Tries to create and return a <see cref="Guid"/> object from the string specified.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		/// <remarks>
		/// To be replaced by .NET 4+ "Guid.TryParse()" after upgrading to .NET 4+.
		/// </remarks>
		public static bool TryParseTolerantly(string s, out Guid guid)
		{
			var m = Regex.Match(s.Trim());
			if (m.Success)
				return (TryParse(m.Value, out guid));

			guid = Guid.Empty;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
