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
// MKY Version 1.0.19
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
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Char utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class CharEx
	{
		/// <summary>
		/// An invalid char is represented by -1.
		/// </summary>
		/// <remarks>
		/// Value corresponds to the value returned by <see cref="System.IO.StringReader.Read()"/>
		/// and the other read functions if no more characters can be read from the stream.
		/// Value also corresponds to <see cref="IO.StreamEx.EndOfStream"/>.
		/// </remarks>
		public const int InvalidChar = -1;

		/// <summary>
		/// Tries to convert the UTF-16 encoded value into the according ASCII code.
		/// </summary>
		public static bool TryConvertToByte(char value, out byte ascii)
		{
			try
			{
				ascii = Convert.ToByte(value);
				return (true);
			}
			catch (OverflowException)
			{
				ascii = 0x00;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================