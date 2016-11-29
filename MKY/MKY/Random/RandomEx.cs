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
// MKY Version 1.0.17
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

using System;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Times for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Random utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class RandomEx
	{
		private static Random staticRandom = new Random();

		/// <summary>
		/// This method solves an issue described in the MSDN description of <see cref="Random"/>:
		/// The default seed value is derived from the system clock and has finite resolution.
		/// As a result, different Random objects that are created in close succession by a call to
		/// the default constructor will have identical default seed values and, therefore, will
		/// produce identical sets of random numbers. This problem can be avoided by using a single
		/// Random object to generate all random numbers.
		/// </summary>
		public static int NextPseudoRandomSeed()
		{
			return (staticRandom.Next());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
